var xVal = xVal || {
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

xVal.Plugins["jquery.validate"] = {
    AttachValidator: function(elementPrefix, rulesConfig) {
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
                    this._attachRuleToDOMElement(ruleName, ruleParams, errorText, $(elem));
                }
            }
        }
    },

    _attachRuleToDOMElement: function(ruleName, ruleParams, errorText, element) {
        var parentForm = element.parents("form");
        if (parentForm.length != 1)
            alert("Error: Element " + element.attr("id") + " is not in a form");
        this._ensureFormIsMarkedForValidation($(parentForm[0]));
        this._associateNearbyValidationMessageSpanWithElement(element);

        switch (ruleName) {
            case "Required":
                element.rules("add", { required: true, messages: { required: errorText} });
                break;

            case "Range":
                if (typeof (ruleParams.Min) == 'undefined')
                    element.rules("add", { max: ruleParams.Max, messages: { max: errorText} });
                else if (typeof (ruleParams.Max) == 'undefined')
                    element.rules("add", { min: ruleParams.Min, messages: { min: errorText} });
                else
                    element.rules("add", { range: [ruleParams.Min, ruleParams.Max], messages: { range: errorText} });
                break;
        }
    },

    _associateNearbyValidationMessageSpanWithElement: function(element) {
        // If there's a <span class='field-validation-error'> soon after, it's probably supposed to display the error message
        // jquery.validation goes looking for attributes called "htmlfor" and "generated", set as follows
        var nearbyMessages = element.nextAll("span.field-validation-error");
        if (nearbyMessages.length > 0) {
            $(nearbyMessages[0]).attr("generated", "true")
                                .attr("htmlfor", element.attr("id"));
        }
    },

    _ensureFormIsMarkedForValidation: function(formElement) {
        if (!formElement.data("isMarkedForValidation")) {
            formElement.data("isMarkedForValidation", true);
            formElement.validate({
                errorClass: "field-validation-error",
                errorElement: "span",
                highlight: function(element) { $(element).addClass("input-validation-error"); },
                unhighlight: function(element) { $(element).removeClass("input-validation-error"); }
            });
        }
    }
};