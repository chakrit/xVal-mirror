﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="xVal.ClientSidePlugins.TestHelpers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.2.6.js") %>"></script>
        <script type="text/javascript" src="<%= ResolveUrl("~/Plugins/jQuery.Validate/jquery.validate.js") %>"></script>
        <script type="text/javascript" src="<%= ResolveUrl("~/Plugins/jQuery.Validate/xVal.jquery.validate.js?nocache=" + DateTime.Now.Ticks) %>"></script>
        <script type="text/javascript" src="<%= ResolveUrl("~/Messages/xVal.Messages.ForUnitTests.js") %>"></script>
        <script type="text/javascript">
            function EqualsFixedStringRule(value, element, params) {
                return (value == params.mustMatch);
            }
        </script>        
    </head>
    <body>
        <% using(Html.BeginForm()) { %>
            <table border="0">
                <% foreach(var fieldName in SampleRuleSets.AllPossibleRules.Keys) { %>
                    <tr>
                        <td><%= fieldName %></td>
                        <td><%= Html.TextBox("myprefix." + fieldName) %></td>
                    </tr>
                <% } %>
            </table>
            <input type="submit" />
        <% } %>
        <%= Html.ClientSideValidation("myprefix", SampleRuleSets.AllPossibleRules) %>
    </body>
</html>
