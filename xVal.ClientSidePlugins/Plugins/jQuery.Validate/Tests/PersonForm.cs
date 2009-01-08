using Selenium;
using Xunit;
using xVal.ClientSidePlugins.TestHelpers;

namespace xVal.ClientSidePlugins.Plugins.jQuery.Validate.Tests
{
    public class PersonForm : IUseFixture<SeleniumContext>
    {
        private const string UrlFormat = "Test?viewPath=/Plugins/jQuery.Validate/Tests/{0}.aspx";
        private DefaultSelenium Browser;
        void IUseFixture<SeleniumContext>.SetFixture(SeleniumContext data)
        {
            Browser = data.Browser;
        }

        [Fact]
        public void NameFieldIsRequired()
        {
            TestFieldValidation("Person", "person.Name", "", "Steve", "This field is required.");
        }

        [Fact]
        public void AgeFieldCannotContainLetters()
        {
            TestFieldValidation("Person", "person.Age", "1x", "1", "Please enter a value between 0 and 150.");
        }

        [Fact]
        public void AgeFieldCannotBeNegative()
        {
            TestFieldValidation("Person", "person.Age", "-1", "0", "Please enter a value between 0 and 150.");
        }

        [Fact]
        public void AgeFieldCannotBeOver150()
        {
            TestFieldValidation("Person", "person.Age", "151", "150", "Please enter a value between 0 and 150.");
        }

        private void TestFieldValidation(string testPage, string inputField, string invalidValue, string validValue, string expectedFailureMessage)
        {
            Browser.Open(string.Format(UrlFormat, testPage));

            // Force validation failure
            Browser.Type(inputField, invalidValue);
            Browser.KeyUp(inputField, "\t");
            Browser.Click("//input[@type='submit']");
            var failureMessageElementLocator = "//span[@htmlfor='" + inputField + "']";
            AssertPresentAndVisible(Browser, failureMessageElementLocator);
            Assert.Equal(expectedFailureMessage, Browser.GetText(failureMessageElementLocator));

            // Now try to satisfy validation
            Browser.Type(inputField, validValue);
            Browser.KeyUp(inputField, "\t");
            AssertNotPresentAndVisible(Browser, failureMessageElementLocator);
        }

        public static void AssertPresentAndVisible(DefaultSelenium browser, string locator)
        {
            if (browser.IsElementPresent(locator))
                Assert.True(browser.IsVisible(locator));
        }

        public static void AssertNotPresentAndVisible(DefaultSelenium browser, string locator)
        {
            if (browser.IsElementPresent(locator))
                Assert.False(browser.IsVisible(locator));
        }
    }
}