using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Required 'using' statements for Selenium Web Scraping
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome; 
using OpenQA.Selenium.Support.UI; 
using SeleniumExtras.WaitHelpers;
// If u use HtmlAgilityPack to parse data
using HtmlAgilityPack;

namespace HarvestingFW.V_01
{
	internal class SeleniumWebScraping
	{
		public static void SeleniumBrowsingAndParsing()
		{
			// Use Chrome browsing to scrape web data or download the page source when it can't be parsed using HTML or JSON

			// Essential NuGet Packages for Chrome browsing are -
			// Selenium.WebDriver (Core Selenium package)
			// Selenium.WebDriver.ChromeDriver (Chrome-specific driver)
			// DotNetSeleniumExtras.WaitHelpers (Helper for explicit waits)

			ChromeDriver driver = new ChromeDriver(); // call chrome browser
			driver.Navigate().GoToUrl(""); // define the url
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)); //wait until browser is ready
			
			do
			{

			}
			while (!driver.PageSource.Contains("")); //wait until browser find specific class or text

			wait.Until(d => d.PageSource.Contains("")); //another way to wait
			wait.Until(ExpectedConditions.ElementExists(By.XPath(".//div[@id='column-content']"))); //another way to wait
			// There are several ways to wait browser loading. U can find out urself too.

			// Now we got the PageSource Data, we need to parse that data
			// There are two ways to parse. U can use what u like.

			//Using HtmlAgilityPack
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(driver.PageSource);

			//Using DOM element selector
			IWebElement div = driver.FindElement(By.XPath(".//div[@class='']"));

			driver.Quit();
		}
	}
}