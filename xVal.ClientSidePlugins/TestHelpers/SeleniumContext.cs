using System;
using Selenium;

namespace xVal.ClientSidePlugins.TestHelpers
{
    internal class SeleniumContext : IDisposable
    {
        private const string SeleniumBrowserType = "*iehta";
        private const string SeleniumHost = "localhost";
        private const int SeleniumPort = 4444;
        private const string TestServerHost = "localhost";
        private const int TestServerPort = 49792;
        private const string TestServerVDir = "/";
        private readonly static string TestRootUrl = "http://" + TestServerHost + ":" + TestServerPort + TestServerVDir;

        public DefaultSelenium Browser { get; private set; }
        
        public SeleniumContext()
        {
            Browser = new DefaultSelenium(SeleniumHost, SeleniumPort, SeleniumBrowserType, TestRootUrl);
            Browser.Start();
        }

        public void Dispose()
        {
            Browser.Stop();
        }
    }
}