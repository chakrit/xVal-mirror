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
                    this._attachRuleToDOMElement(ruleName, ruleParams, $(elem));
                }
            }
        }
    },

    _attachRuleToDOMElement: function(ruleName, ruleParams, element) {
        var parentForm = element.parents("form");
        if (parentForm.length != 1)
            alert("Error: Element " + element.attr("id") + " is not in a form");
        this._ensureFormIsMarkedForValidation(parentForm[0]);
        this._associateNearbyValidationMessageSpanWithElement(element);

        switch (ruleName) {
            case "Required":
                element.rules("add", { required: true });
                break;

            case "NumericRange":
                if (typeof (ruleParams.Min) == 'undefined')
                    element.rules("add", { max: ruleParams.Max });
                else if (typeof (ruleParams.Max) == 'undefined')
                    element.rules("add", { min: ruleParams.Min });
                else
                    element.rules("add", { range: [ruleParams.Min, ruleParams.Max] });
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
        if (!formElement.isMarkedForValidation) {
            formElement.isMarkedForValidation = true;
            $(formElement).validate({
                errorClass: "field-validation-error",
                errorElement: "span",
                highlight: function(element) { $(element).addClass("input-validation-error"); },
                unhighlight: function(element) { $(element).removeClass("input-validation-error"); }
            });
        }
    }
};