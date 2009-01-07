<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<% using(Html.BeginForm()) { %>
    <div>Name: <%= Html.TextBox("Name") %> <%= Html.ValidationMessage("Name") %> </div>
    <div>Age: <%= Html.TextBox("Age") %> <%= Html.ValidationMessage("Age")%> </div>
    <input type="submit" />
<% } %>    
