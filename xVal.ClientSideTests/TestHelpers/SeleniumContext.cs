using System;
using Selenium;

namespace xVal.ClientSideTests.TestHelpers
{
    public class SeleniumContext : IDisposable
    {
        public DefaultSelenium Browser { get; private set; }
        
        public SeleniumContext()
        {
            Browser = new DefaultSelenium("localhost", 4444, "*iehta", "http://www.google.com/");
            Browser.Start();
        }

        public void Dispose()
        {
            Browser.Stop();
        }
    }
}