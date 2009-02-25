using System;
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
            TestFieldValidation("myprefix.DataType_EmailAddress_Field", "blah", "blah@domain.com", "DATATYPE_EMAIL");
        }

        [Fact]
        public void DataType_CreditCardLuhn_Enforced()
        {
            TestFieldValidation("myprefix.DataType_CreditCardLuhn_Field", "4111 11111111 1112", "4111-1111 11111111", "DATATYPE_CREDITCARDLUHN");
        }

        [Fact]
        public void DataType_Integer_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Integer_Field", "32x", "-137", "DATATYPE_INTEGER");
        }

        [Fact]
        public void DataType_Decimal_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Decimal_Field", "-32x3", "-323", "DATATYPE_DECIMAL");
            TestFieldValidation("myprefix.DataType_Decimal_Field", "32x3.442", "323.442", "DATATYPE_DECIMAL");
        }

        [Fact]
        public void DataType_Date_Enforced() // Not too impressed with how jQuery Validate handles date formats. Will need to review this. For example, it accepts 05/09/2x and rejects 2008-09-05.
        {
            TestFieldValidation("myprefix.DataType_Date_Field", "05/0x9/2008", "05/09/2008", "DATATYPE_DATE");
        }

        [Fact]
        public void DataType_DateTime_Enforced()
        {
            TestFieldValidation("myprefix.DataType_DateTime_Field", "05/09/2008 3:4", "05/09/2008 3:44", "DATATYPE_DATETIME");
        }

        [Fact]
        public void DataType_Currency_Enforced()
        {
            TestFieldValidation("myprefix.DataType_Currency_Field", "£4509101.", "4509101", "DATATYPE_CURRENCY");
            TestFieldValidation("myprefix.DataType_Currency_Field", "4509101.381", "£ 4,509,101.38", "DATATYPE_CURRENCY");
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
            TestFieldValidation("myprefix.Range_Int_Field", "3", "6", "RANGE_NUMERIC_MINMAX:5,10");
        }

        [Fact]
        public void Range_Decimal_Enforced()
        {
            TestFieldValidation("myprefix.Range_Decimal_Field", "10.99", "10.98", "RANGE_NUMERIC_MAX:10.98");
        }

        [Fact]
        public void Range_String_Enforced()
        {
            TestFieldValidation("myprefix.Range_String_Field", "aardvarj", "aardvarl", "RANGE_STRING_MINMAX:aardvark,antelope");
        }

        [Fact]
        public void Range_DateTime_Enforced()
        {
            TestFieldValidation("myprefix.Range_DateTime_Field", "blah", "January 01, 2008", "DATATYPE_DATE");
            TestFieldValidation("myprefix.Range_DateTime_Field", "01/01/2000", "01/01/2002", "RANGE_DATETIME_MIN:2001-02-19 17:04:59");
        }

        [Fact]
        public void StringLength_Min_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Min_Field", "ab", "abc", "STRINGLENGTH_MIN:3");
        }

        [Fact]
        public void StringLength_Max_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Max_Field", "abcdefg", "abcdef", "STRINGLENGTH_MAX:6");
        }

        [Fact]
        public void StringLength_Range_Enforced()
        {
            TestFieldValidation("myprefix.StringLength_Range_Field", "abc", "abcd", "STRINGLENGTH_MINMAX:4,7");
            TestFieldValidation("myprefix.StringLength_Range_Field", "abcdefgh", "abcdefg", "STRINGLENGTH_MINMAX:4,7");
        }

        [Fact]
        public void Comparison_Equals_Enforced()
        {
            TestFieldValidation("myprefix.Comparison_Equals", "bla", "blah", "COMPARISON_EQUALS:RequiredField",
                // Setup first: populate the RequiredField box first
                browser => browser.Type("myprefix_RequiredField", "blah")
            );
        }

        [Fact]
        public void Comparison_DoesNotEqual_Enforced()
        {
            TestFieldValidation("myprefix.Comparison_DoesNotEqual", "blah", "blah2", "COMPARISON_DOESNOTEQUAL:RequiredField",
                // Setup first: populate the RequiredField box first
                browser => browser.Type("myprefix_RequiredField", "blah")
            );
        }

        [Fact]
        public void Custom_Enforced()
        {
            TestFieldValidation("myprefix.Custom", "hey", "hello", "Please enter the string 'hello'");
        }

        private void TestFieldValidation(string inputField, string invalidValue, string validValue, string expectedFailureMessage)
        {
            TestFieldValidation(inputField, invalidValue, validValue, expectedFailureMessage, null);
        }

        private void TestFieldValidation(string inputField, string invalidValue, string validValue, string expectedFailureMessage, Action<ISelenium> additionalSetup)
        {
            Browser.Open(Url);
            if (additionalSetup != null)
                additionalSetup(Browser);
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