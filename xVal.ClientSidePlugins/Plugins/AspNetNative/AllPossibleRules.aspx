<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="xVal.ClientSidePlugins.TestHelpers"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(typeof(System.Web.UI.Page), "WebForms.js") %>"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(typeof(System.Web.UI.Page), "WebUIValidation.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Plugins/AspNetNative/xVal.AspNetNative.js") %>"></script>
</head>
<body>
    <% using(Html.BeginForm()) { %>
        Generated at <%= DateTime.Now.ToLongTimeString() %>
        
        <table border="0">
            <% foreach(var fieldName in SampleRuleSets.AllPossibleRules.Keys) { %>
                <tr>
                    <td><%= fieldName %></td>
                    <td><%= Html.TextBox("myprefix." + fieldName) %></td>
                </tr>
            <% } %>
        </table>        
        
        <input type="submit" value="Post now" />
    <% } %>
    
    <script type="text/javascript">
        xVal.AttachValidator("myprefix", <%= Html.ClientSideValidationRules(SampleRuleSets.AllPossibleRules) %>);
    </script>
</body>
</html>
