var xVal = xVal || {
    Plugins: {},
    AttachValidator: function(rulesetName, elementPrefix, pluginName) {
        if (pluginName != null)
            this.Plugins[pluginName].AttachValidator(rulesetName, elementPrefix);
        else
            for (var key in this.Plugins) {
                this.Plugins[key].AttachValidator(rulesetName, elementPrefix);
                return;
            }
    }
};

xVal.Plugins["jquery.validate"] = {
    AttachValidator: function(rulesetName, elementPrefix) {
        var self = this;
        var rulesets = $("*[name=" + rulesetName + "]")
                       .filter(function() { return this.tagName == "XVAL:RULESET"; });
        rulesets.each(function() {
            $("*[forfield]", $(this)).each(function() {
                // Attach this rule to a DOM element, if we can find one
                var rule = $(this);
                var correspondingId = (elementPrefix ? elementPrefix + "." : "") + rule.attr("forfield");
                var correspondingElement = $("#" + correspondingId);
                if (correspondingElement)
                    self._attachRuleToDOMElement(rule, correspondingElement);
            });
        });
    },

    _attachRuleToDOMElement: function(rule, element) {
        var parentForm = element.parents("form");
        if (parentForm.length != 1)
            alert("Error: Element " + element.attr("id") + " is not in a form");
        this._ensureFormIsMarkedForValidation(parentForm[0]);
        this._associateNearbyValidationMessageSpanWithElement(element);

        switch (rule[0].tagName) {
            case "REQUIRED":
                element.rules("add", { required: true });
                break;

            case "NUMERICRANGE":
                if (typeof (rule.attr("min")) == 'undefined')
                    element.rules("add", { max: rule.attr("max") });
                else if (typeof (rule.attr("max")) == 'undefined')
                    element.rules("add", { min: rule.attr("min") });
                else
                    element.rules("add", { range: [rule.attr("min"), rule.attr("max")] });
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