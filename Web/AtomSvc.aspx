<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AtomSvc.aspx.cs" Inherits="AtomSvc"
    Title="Atom Svc" %>
<service xmlns="http://www.w3.org/2007/app" xmlns:atom="http://www.w3.org/2005/Atom">
  <workspace>
    <atom:title><% Response.Write(GetRssTitle()); %></atom:title>
    <collection href="<% Response.Write(SessionManager.WebsiteUrl); %>AtomPost.aspx">
      <atom:title>Posts</atom:title>
      <accept>application/atom+xml;type=entry</accept>
      <categories>
       <asp:Repeater id="categories" runat="server">
        <ItemTemplate>
         <atom:category term="<%# Eval("Name") %>" />    
        </ItemTemplate>
       </asp:Repeater>
      </categories>
    </collection>
    <collection href="<% Response.Write(SessionManager.WebsiteUrl); %>AtomImage.aspx">
      <atom:title>Images</atom:title>
      <accept>image/jpeg</accept>
      <accept>image/gif</accept>
      <accept>image/png</accept>
    </collection>
  </workspace>
</service>
