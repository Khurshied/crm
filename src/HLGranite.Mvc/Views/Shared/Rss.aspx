<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rss.aspx.cs" Inherits="HLGranite.Mvc.Views.Shared.Rss" %>

<rss version="2.0" xmlns:dc="http://purl.org/dc/elements/1.1/">
    <channel>
        <title><%=Html.Encode(ViewData.Model.Title) %></title>
        <description><%=ViewData.Model.Description %></description>
        <link><%=ViewData.Model.Url%></link>
        <language><%=ViewData.Model.Language%></language>
       <%foreach (HLGranite.Mvc.Models.FeedItem item in ViewData.Model.Items)
         {%>
        <item>
            <dc:creator><%=Html.Encode(item.Creator)%></dc:creator>
            <title><%=Html.Encode(item.Title)%></title>
            <description><%=Html.Encode(item.Description)%></description>
            <link><%=item.Url %></link>
            <pubDate><%=item.Published.ToString("R") %></pubDate>
            <% foreach (string tag in item.Tags)
               { %>
                <category><%=tag %></category>
            <% } %>
        </item>
        <%
          } %>
    </channel>
</rss>