﻿using System;
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
            //WebBrowsingAndScraping.SeleniumBrowsingAndScraping();

            //DatabaseConnections.MSAccessConnection();
            //DatabaseConnections.SQLiteConnection();
            DatabaseConnections.MySQLConnection();
		}
    }
}