using Selenium;
using Xunit;
using xVal.ClientSidePlugins.TestHelpers;

namespace xVal.ClientSidePlugins.Plugins.jQuery.Validate.Tests
{
    /// <summary>
    /// IMPORTANT: To run these tests, you need to be running:
    ///    * Selenium Server on localhost:4444 (download Selenium RC from http://seleniumhq.org/download/)
    ///    * The xVal.ClientSidePlugins site on localhost:49792 (just go to "Debug"->"Start without debugging")
    /// </summary>
    public class AllPossibleRulesTests : IUseFixture<SeleniumContext>
    {
        private const string Url = "Test?viewPath=/Plugins/jQuery.Validate/Tests/AllPossibleRules.aspx";
        private DefaultSelenium Browser;
        void IUseFixture<SeleniumContext>.SetFixture(SeleniumContext data)
        {
            Browser = data.Browser;
        }

        [Fact]
        public void RequiredField_Is_Required_And_Shows_Custom_Error_Message()
        {
            TestFieldValidation("myprefix.RequiredField", "", "something", "This is a custom error message for a required field");
        }

        [Fact]
        public void DataType_EmailAddress_Enforced()
        {
            TestFieldValidation("myprefix.DataType_EmailAddress_Field", "blah", "blah@domain.com", "Please enter a valid email address.");
        }

        [Fact]
        public void DataType_CreditCardLuhn_Enforced()
        {
            TestFieldValidation("myprefix.DataType_CreditCardLuhn_Field", "4111 11111111 1112", "4111-1111 11111111", "Please enter a valid credit card number.");
        }

        [Fact]
        public void DataType_Integer_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Integer_Field", "32x", "-137", "Please enter a whole number.");
        }

        [Fact]
        public void DataType_Decimal_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Decimal_Field", "-32x3", "-323", "Please enter a valid number.");
            TestFieldValidation("myprefix.DataType_Decimal_Field", "32x3.442", "323.442", "Please enter a valid number.");
        }

        [Fact]
        public void DataType_Date_Enforced() // Not too impressed with how jQuery Validate handles date formats. Will need to review this. For example, it accepts 05/09/2x and rejects 2008-09-05.
        {
            TestFieldValidation("myprefix.DataType_Date_Field", "05/0x9/2008", "05/09/2008", "Please enter a valid date.");
        }

        [Fact]
        public void DataType_DateTime_Enforced()
        {
            TestFieldValidation("myprefix.DataType_DateTime_Field", "05/09/2008 3:4", "05/09/2008 3:44", "Please enter a valid date and time.");
        }

        [Fact]
        public void DataType_Currency_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Currency_Field", "£4509101.", "4509101", "Please enter a currency value.");
            TestFieldValidation("myprefix.DataType_Currency_Field", "4509101.381", "£ 4,509,101.38", "Please enter a currency value.");
        }

        [Fact]
        public void Regex_Enforced()
        {
            TestFieldValidation("myprefix.Regex_Field", "X12", "   B938_", "Enter a value of the form 'X123'");
            TestFieldValidation("myprefix.Regex_CaseInsensitive_Field", "AB1C", "...aUI", "Enter a value of the form 'aBc'");
        }

        [Fact]
        public void Range_Int_Enforced()
        {
            TestFieldValidation("myprefix.Range_Int_Field", "3", "6", "Please enter a value between 5 and 10.");
        }

        [Fact]
        public void Range_Decimal_Enforced()
        {
            TestFieldValidation("myprefix.Range_Decimal_Field", "10.99", "10.98", "Please enter a value less than or equal to 10.98.");
        }

        [Fact]
        public void Range_String_Enforced()
        {
            TestFieldValidation("myprefix.Range_String_Field", "aardvarj", "aardvarl", "Please enter a value alphabetically between 'aardvark' and 'antelope'.");
        }

        [Fact]
        public void Range_DateTime_Enforced()
        {
            TestFieldValidation("myprefix.Range_DateTime_Field", "blah", "January 01, 2008", "Please enter a valid date in yyyy/mm/dd format.");
            TestFieldValidation("myprefix.Range_DateTime_Field", "01/01/2000", "01/01/2002", "Please enter a date no earlier than 2001-02-01 17:04:59.");
        }

        [Fact]
        public void StringLength_Min_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Min_Field", "ab", "abc", "Please enter at least 3 characters.");
        }

        [Fact]
        public void StringLength_Max_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Max_Field", "abcdefg", "abcdef", "Please enter no more than 6 characters.");
        }

        [Fact]
        public void StringLength_Range_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Range_Field", "abc", "abcd", "Please enter a value between 4 and 7 characters long.");
            TestFieldValidation("myprefix.StringLength_Range_Field", "abcdefgh", "abcdefg", "Please enter a value between 4 and 7 characters long.");
        }

        private void TestFieldValidation(string inputField, string invalidValue, string validValue, string expectedFailureMessage)
        {
            Browser.Open(Url);
            string inputFieldID = inputField.Replace(".", "_"); // Match ASP.NET MVC behavior

            // Force validation failure
            Browser.Type(inputFieldID, invalidValue);
            Browser.KeyUp(inputFieldID, "\t");
            Browser.Click("//input[@type='submit']");
            var failureMessageElementLocator = "//span[@htmlfor='" + inputFieldID + "']";
            AssertPresentAndVisible(Browser, failureMessageElementLocator);
            Assert.Equal(expectedFailureMessage, Browser.GetText(failureMessageElementLocator));

            // Now try to satisfy validation
            Browser.Type(inputFieldID, validValue);
            Browser.KeyUp(inputFieldID, "\t");
            AssertNotPresentAndVisible(Browser, failureMessageElementLocator);
        }

        public static void AssertPresentAndVisible(DefaultSelenium browser, string locator)
        {
            Assert.True(browser.IsElementPresent(locator));
            Assert.True(browser.IsVisible(locator));
        }

        public static void AssertNotPresentAndVisible(DefaultSelenium browser, string locator)
        {
            if (browser.IsElementPresent(locator))
                Assert.False(browser.IsVisible(locator));
        }
    }
}