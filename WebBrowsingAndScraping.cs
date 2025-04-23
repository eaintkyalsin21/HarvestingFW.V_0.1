using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Essential using statements 
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome; 
using OpenQA.Selenium.Support.UI; 
using SeleniumExtras.WaitHelpers;
using HtmlAgilityPack;
using System.Threading;

namespace HarvestingFW.V_01
{
	internal class WebBrowsingAndScraping
	{
		public static void SeleniumBrowsingAndScraping()
		{
			// Essential NuGet Packages
			// Selenium.WebDriver (Selenium package)
			// Selenium.WebDriver.ChromeDriver (Chrome-specific driver)
			// DotNetSeleniumExtras.WaitHelpers (Helper for explicit waits)

			ChromeDriver driver = new ChromeDriver();
			driver.Navigate().GoToUrl("https://www.ferdamalastofa.is/en/moya/news");
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
			
			do
			{

			}
			while (!driver.PageSource.Contains("entries all")); //wait until browser find specific element

			// Other ways to wait
			wait.Until(d => d.PageSource.Contains("entries all"));
			wait.Until(ExpectedConditions.ElementExists(By.XPath(".//div[@class='entries all']")));

			// Select item from dropdown list
			IWebElement select = driver.FindElement(By.XPath(".//select[@id='year']"));
			foreach (IWebElement option in select.FindElements(By.TagName("option")))
			{
				if (option.Text.Trim() == "2024")
				{
					option.Click();
					Thread.Sleep(TimeSpan.FromSeconds(1));
					wait.Until(d => d.FindElement(By.Id("year")).FindElement(By.XPath(".//option[@selected='selected']")).Text.Trim() == "2024");
					wait.Until(d => d.Url.Contains("2024")); //another way to wait
					break;
				}
			}

			// Parse Data with DOM Element Selector
			IWebElement mainDiv = driver.FindElement(By.XPath(".//div[@class='entries all']"));
			foreach (IWebElement nodeDiv in mainDiv.FindElements(By.XPath(".//div[contains(@class, 'news__entry')]")))
			{
				string title = "", titleUrl = "", image = "", date = "";

				if (nodeDiv.FindElement(By.XPath(".//div[@class='content']//h2[@class='entryTitle']")) != null)
					title = nodeDiv.FindElement(By.XPath(".//div[@class='content']//h2[@class='entryTitle']")).Text.Trim();

				if (nodeDiv.FindElement(By.TagName("a")) != null)
					titleUrl = nodeDiv.FindElement(By.TagName("a")).GetAttribute("href");	

				if (nodeDiv.FindElement(By.ClassName("entryImage")).FindElement(By.TagName("img")) != null)
					image = nodeDiv.FindElement(By.ClassName("entryImage")).FindElement(By.TagName("img")).GetAttribute("src");

				if (nodeDiv.FindElement(By.XPath(".//div[@class='content']//div[@class='date']")) != null)
					date = nodeDiv.FindElement(By.XPath(".//div[@class='content']//div[@class='date']")).Text.Trim();

				Console.WriteLine($"Title: {title} \r\nTitle URL: {titleUrl} \r\nImage: {image} \r\nDate: {date} \r\nCurrent Datetime: {DateTime.Now}\r\n");
			}

			// Prase Data with HtmlAgilityPack
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(driver.PageSource);

			HtmlNode RootDiv = doc.DocumentNode.SelectSingleNode(".//div[@class='entries all']");
			foreach (HtmlNode childDiv in RootDiv.SelectNodes(".//div[contains(@class, 'news__entry')]"))
			{
				string title = "", titleUrl = "", image = "", date = "";

				if (childDiv.SelectSingleNode(".//div[@class='content']//h2[@class='entryTitle']") != null)
					title = childDiv.SelectSingleNode(".//div[@class='content']//h2[@class='entryTitle']").InnerText.Trim();

				if (childDiv.SelectSingleNode(".//a") != null)
					titleUrl = childDiv.SelectSingleNode(".//a").Attributes["href"].Value;

				if (childDiv.SelectSingleNode(".//div[@class='entryImage']//img") != null)
					image = childDiv.SelectSingleNode(".//div[@class='entryImage']//img").Attributes["src"].Value;

				if (childDiv.SelectSingleNode(".//div[@class='content']//div[@class='date']") != null)
					date = childDiv.SelectSingleNode(".//div[@class='content']//div[@class='date']").InnerText.Trim();

				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"Title: {title} \r\nTitle URL: {titleUrl} \r\nImage: {image} \r\nDate: {date} \r\nCurrent Datetime: {DateTime.Now}\r\n");
				Console.ResetColor();
			}

			// Scroll to page bottom in a loop — Use for 'Load More' or infinite scroll pages (eg. https://unsplash.com/s/users/book)
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

			long pageHeight1 = Convert.ToInt64(js.ExecuteScript("return document.body.scrollHeight")); //whole page height (visible + non visible)
			int maxScrolls = 3;

			while (true)
			{
				js.ExecuteScript("window.scrollBy(0, 500);");
				Thread.Sleep(TimeSpan.FromSeconds(1));

				long scrolledHeight = Convert.ToInt64(js.ExecuteScript("return window.pageYOffset")); //how much u scrolled
				long pageHeight2 = Convert.ToInt64(js.ExecuteScript("return document.body.scrollHeight")); //whole page height (visible + non visible)
				long visibleHeight = Convert.ToInt64(js.ExecuteScript("return window.innerHeight")); //visible page height

				bool isAtBottom = (scrolledHeight + visibleHeight) >= pageHeight2;

				if (isAtBottom)
				{
					if (pageHeight2 == pageHeight1)
					{
						maxScrolls--;
					}
					else
					{
						maxScrolls = 3;
						pageHeight1 = pageHeight2;
					}

					if (maxScrolls <= 0)
					{
						Console.WriteLine("Scrolled until page bottom!");
						break;
					}
				}
			}

			// For Paginations, do not click when u can create link (eg. https://www.example/page=1, https://www.example/page=2)
			// Button Click
			while (true)
			{
				if (driver.FindElement(By.XPath(".//a[@class='stepper next']")) != null)
				{
					// Parse data here first

					// JS button click
					IWebElement nextButton = driver.FindElement(By.XPath(".//a[@class='stepper next']"));
					string pageNumber = driver.FindElement(By.XPath(".//span[@class='pagerNumber disabled']")).Text.Trim();
					((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", nextButton);
					Thread.Sleep(TimeSpan.FromSeconds(1));
					wait.Until(d => d.FindElement(By.XPath(".//span[@class='pagerNumber disabled']")).Text.Trim() != pageNumber);

					// Normal button click
					IWebElement nextClick = driver.FindElement(By.XPath(".//a[@class='stepper next']"));
					string oldUrl = driver.Url;
					nextClick.Click();
					Thread.Sleep(TimeSpan.FromSeconds(1));
					wait.Until(d => d.Url != oldUrl); //another way to wait

					bool nextButtonVisible;
					try
					{
						nextButtonVisible = driver.FindElement(By.XPath(".//a[@class='stepper next']")).Displayed;
					}
					catch (NoSuchElementException)
					{
						nextButtonVisible = false;
					}

					if (!nextButtonVisible)
					{
						Console.WriteLine("Clicked all next pages!");
						break;
					}
				}
			}

			driver.Quit();
		}
	}
}