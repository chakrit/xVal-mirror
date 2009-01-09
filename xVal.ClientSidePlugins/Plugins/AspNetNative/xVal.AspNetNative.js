﻿var xVal = xVal || {
    Plugins: {},
    AttachValidator: function(elementPrefix, rulesConfig, pluginName) {
        if (pluginName != null)
            this.Plugins[pluginName].AttachValidator(elementPrefix, rulesConfig);
        else
            for (var key in this.Plugins) {
            this.Plugins[key].AttachValidator(elementPrefix, rulesConfig);
            return;
        }
    }
};

var Page_Validators;
var Page_ValidationActive;

xVal.Plugins["AspNetNative"] = {
    AttachValidator: function(elementPrefix, rulesConfig) {
        Page_Validators = new Array();

        for (var i = 0; i < rulesConfig.Fields.length; i++) {
            var fieldName = rulesConfig.Fields[i].FieldName;
            var fieldRules = rulesConfig.Fields[i].FieldRules;

            // Is there a matching DOM element?
            var elemId = (elementPrefix ? elementPrefix + "." : "") + fieldName;
            var elem = document.getElementById(elemId);

            if (elem) {
                for (var j = 0; j < fieldRules.length; j++) {
                    var ruleName = fieldRules[j].RuleName;
                    var ruleParams = fieldRules[j].RuleParameters;
                    var errorText = (typeof (fieldRules[j].Message) == 'undefined' ? null : fieldRules[j].Message);
                    this._attachRuleToDOMElement(ruleName, ruleParams, errorText, elem);
                }
            }
        }

        Page_ValidationActive = false;
        if (typeof (ValidatorOnLoad) == "function")
            ValidatorOnLoad();
    },

    _attachRuleToDOMElement: function(ruleName, ruleParams, errorText, element) {
        var ruleConfig = this._getAspNetRuleConfig(ruleName, ruleParams, errorText);
        if (ruleConfig == null)
            return;

        // Find parent form and ensure it's enabled for validation
        var parentForm = element;
        while (parentForm.tagName != "FORM") {
            parentForm = parentForm.parentNode;
            if (parentForm == null)
                alert("Error: Element " + element.id + " is not in a form");
        }
        this._ensureValidationEnabledOnForm(parentForm);

        var messageContainer = this._createMessageContainer(element, ruleConfig.errorMessage);

        Page_Validators[Page_Validators.length] = messageContainer;

        messageContainer.controltovalidate = element.id;
        messageContainer.errormessage = ruleConfig.errorMessage;
        messageContainer.display = "Dynamic";
        messageContainer.evaluationfunction = ruleConfig.evaluationFunction;
        for (var i = 0; i < ruleConfig.params.length; i++)
            messageContainer[ruleConfig.params[i].name] = ruleConfig.params[i].value;
    },

    _getAspNetRuleConfig: function(ruleName, ruleParams, fixedErrorText) {
        switch (ruleName) {
            case "Required":
                return {
                    evaluationFunction: "RequiredFieldValidatorEvaluateIsValid",
                    params: [{ name: "initialvalue", value: ""}],
                    errorMessage: fixedErrorText || "Please enter a value."
                };
            case "Range":
                var message;
                var min = null, max = null;
                if (ruleParams.Type != "datetime") {
                    min = typeof (ruleParams.Min) == 'undefined' ? null : ruleParams.Min;
                    max = typeof (ruleParams.Max) == 'undefined' ? null : ruleParams.Max;
                } else {
                    if (typeof (ruleParams.MinYear) != 'undefined')
                        min = new Date(ruleParams.MinYear, ruleParams.MinMonth - 1, ruleParams.MinDay, ruleParams.MinHour, ruleParams.MinMinute, ruleParams.MinSecond);
                    if (typeof (ruleParams.MaxYear) != 'undefined')
                        max = new Date(ruleParams.MaxYear, ruleParams.MaxMonth - 1, ruleParams.MaxDay, ruleParams.MaxHour, ruleParams.MaxMinute, ruleParams.MaxSecond);
                }
                if (min != null) {
                    if (max != null)
                        message = "Please enter a value between " + min + " and " + max + ".";
                    else
                        message = "Please enter a value of at least " + min + ".";
                }
                else
                    message = "Please enter a value no more than " + max + ".";

                var aspNetNativeType = ruleParams.Type == "string" ? "String" :
                                       ruleParams.Type == "integer" ? "Integer" :
                                       ruleParams.Type == "decimal" ? "Double" :
                                       ruleParams.Type == "datetime" ? "Date" : alert("Unknown range type:" + ruleParams.Type);
                if (aspNetNativeType != "Date") {
                    min = "" + (min || Number.MIN_VALUE);
                    max = "" + (max || Number.MAX_VALUE);
                }
                return {
                    evaluationFunction: ruleParams.Type != "datetime" ? "RangeValidatorEvaluateIsValid" : "xVal_AspNetNative_Range_DateTime",
                    params: [{ name: "decimalchar", value: "." },
                             { name: "type", value: aspNetNativeType },
                             { name: "minimumvalue", value: min },
                             { name: "maximumvalue", value: max}],
                    errorMessage: fixedErrorText || message
                };
            case "RegEx":
                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: ruleParams.Pattern },
                             { name: "options", value: typeof (ruleParams.Options) == 'undefined' ? "" : ruleParams.Options}],
                    errorMessage: fixedErrorText || "Please enter a valid value."
                };
            case "StringLength":
                var min = typeof (ruleParams.MinLength) == 'undefined' ? null : ruleParams.MinLength;
                var max = typeof (ruleParams.MaxLength) == 'undefined' ? null : ruleParams.MaxLength;
                var pattern = "^.{" + (min || "0") + "," + (max || "") + "}$";
                var message;
                if (min != null) {
                    if (max != null)
                        message = "Please enter a value between " + min + " and " + max + " characters long.";
                    else
                        message = "Please enter a value at least " + min + " characters long.";
                }
                else
                    message = "Please enter a value no more than " + max + " characters long.";

                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: pattern },
                             { name: "options", value: ""}],
                    errorMessage: fixedErrorText || message
                };

            case "DataType":
                if (ruleParams.Type == "CreditCardLuhn") {
                    return {
                        evaluationFunction: "xVal_AspNetNative_CreditCardLuhn",
                        params: [],
                        errorMessage: fixedErrorText || "Please enter a valid credit card number."
                    };
                }

                var pattern, message;
                switch (ruleParams.Type) {
                    case "EmailAddress":
                        pattern = "^[\\w\\.=-]+@[\\w\\.-]+\\.[\\w]{2,}$";
                        message = "Please enter a valid email address.";
                        break;
                    case "Integer":
                        pattern = "^\\-?\\d+$";
                        message = "Please enter a number.";
                        break;
                    case "Decimal":
                        pattern = "^\\-?\\d+(\\.\\d+)?$";
                        message = "Please enter a decimal number.";
                        break;
                    case "Date":
                        pattern = "^(\\d{1,2}[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s](\\d{2}|\\d{4}))|((\\d{2}|\\d{4})[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s]\\d{1,2})$";
                        message = "Please enter a valid date.";
                        break;
                    case "DateTime":
                        pattern = "^(\\d{1,2}[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s](\\d{2}|\\d{4}))|((\\d{2}|\\d{4})[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s]\\d{1,2})\\s+\\d{1,2}\\:\\d{2}(\\:\\d{2})?$";
                        message = "Please enter a valid date and time.";
                        break;
                    case "Currency":
                        pattern = "^\\-?\\D?\\s?\\-?\\s?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$";
                        message = "Please enter a currency value.";
                        break;
                }
                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: pattern },
                             { name: "options", value: "i"}],
                    errorMessage: fixedErrorText || message
                };
        }
        return null;
    },

    _hideElementOnChange: function(elementToWatch, elementToHide) {
        var handler = function() { elementToHide.style.display = "none"; };
        if (elementToWatch.addEventListener)
            elementToWatch.addEventListener('change', handler, false);
        else
            elementToWatch.attachEvent('onchange', handler);
    },

    _createMessageContainer: function(element, initialText) {
        // Is there an existing message container with htmlfor="elementid"?
        // If so, we'll put the messages next to it
        var insertAfterElem = element;
        var spans = document.getElementsByTagName("SPAN");
        for (var i = 0; i < spans.length; i++) {
            if (spans[i].getAttribute("htmlfor") == element.id) {
                insertAfterElem = spans[i];
                this._hideElementOnChange(element, insertAfterElem);
                break;
            }
        }

        var result = document.createElement("span");
        result.id = element.id + "_Msg";
        result.innerHTML = initialText;
        result.style.color = "Red";
        result.style.display = "none";
        if (insertAfterElem.nextSibling)
            insertAfterElem.parentNode.insertBefore(result, insertAfterElem.nextSibling);
        else
            insertAfterElem.parentNode.appendChild(result);
        return result;
    },

    _ensureValidationEnabledOnForm: function(formElement) {
        if (!formElement._xVal_ValidationEnabledOnForm) {
            formElement._xVal_ValidationEnabledOnForm = true;

            formElement.onsubmit = function() {
                return (Page_ValidationActive ? ValidatorCommonOnSubmit() : false);
            };

            var inputControls = formElement.getElementsByTagName("INPUT");
            for (var i = 0; i < inputControls.length; i++) {
                if (inputControls[i].type && (inputControls[i].type.toLowerCase() == 'submit')) {
                    inputControls[i].onclick = function() {
                        WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name || this.id || "", "", true, "", "", false, false));
                    };
                }
            }
        }
    }
};

function xVal_AspNetNative_RegEx(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    var regex = val.options == "" ? new RegExp(val.pattern) : new RegExp(val.pattern, val.options);
    return regex.test(value);
}

function xVal_AspNetNative_Range_DateTime(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    var min = val.minimumvalue;
    var max = val.maximumvalue;

    var parsedValue = Date.parse(value);
    if (isNaN(parsedValue))
        return false;
    else
        parsedValue = new Date(parsedValue);
    if (min != null)
        if (parsedValue < min) return false;
    if (max != null)
        if (parsedValue > max) return false;
    return true;
}

function xVal_AspNetNative_CreditCardLuhn(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    value = value.replace(/\D/g, "");
    if (value == "") return false;
    var sum = 0;
    for (var i = value.length - 2; i >= 0; i -= 2)
        sum += Array(0, 2, 4, 6, 8, 1, 3, 5, 7, 9)[parseInt(value.charAt(i), 10)];
    for (var i = value.length - 1; i >= 0; i -= 2)
        sum += parseInt(value.charAt(i), 10);
    return (sum % 10) == 0;
}