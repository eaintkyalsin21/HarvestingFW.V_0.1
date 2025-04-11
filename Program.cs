using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace HarvestingFW.V_01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //SeleniumWebScraping.SeleniumBrowsingAndParsing();
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://substack.com/api/v1/category/public/153/paid?page=20");
		}
    }
}