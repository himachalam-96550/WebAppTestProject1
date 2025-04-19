using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WebAppTestProject1
{
    public class Tests
    {

        
        [SetUp]
        public void Setup()
        {

            
        }

        [Test]
        public void Test1()
        {   // The downloaded files  and logs will be stored in the DownloadedFiles folder
            string downloadPath = @"C:\Users\suman\source\repos\WebAppTestProject1\DownloadedFiles\";
            string logPath = Path.Combine(downloadPath, "DownloadLog.txt");
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("disable-popup-blocking", "true");
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("https://orbiter-for-testing.azurewebsites.net/products/testApp?isInternal=false");
            var links = driver.FindElements(By.XPath("//ul[@class='list-group list-group-flush']/li/a"));
            using (StreamWriter logFile = new StreamWriter(logPath))
            {
                foreach (var link in links)
                {
                    string linkText = link.Text.Trim();
                    string url = link.GetAttribute("href");
                    if (string.IsNullOrEmpty(url))
                    {
                        logFile.WriteLine($"Missing Link: {linkText} has no href.");
                        continue;
                    }
                    string expectedFile = Path.Combine(downloadPath, linkText);
                    WebClient client = new WebClient();
                    try
                    {
                        client.DownloadFile(url, expectedFile);
                        Thread.Sleep(5000);
                        if (File.Exists(expectedFile))
                        {
                            Console.WriteLine($"Downloaded: {linkText}");
                        }
                        else
                        {
                            logFile.WriteLine($"File Missing After Download: Expected {linkText}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logFile.WriteLine($"Error downloading {linkText}: {ex.Message}");
                    }
                }
            }
            driver.Quit();
            Console.WriteLine("Download check complete. Log saved at: " + logPath);
        }

        [TearDown]
        public void TestTearDown()
        {
            
        }
    }
}