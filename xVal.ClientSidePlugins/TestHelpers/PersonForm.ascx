<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<% using(Html.BeginForm()) { %>
    <div>Name: <%= Html.TextBox("person.Name") %> <%= Html.ValidationMessage("person.Name")%> </div>
    <div>Age: <%= Html.TextBox("person.Age")%> <%= Html.ValidationMessage("person.Age")%> </div>
    <input type="submit" />
<% } %>    
