<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RssBlog.aspx.cs"
 Inherits="RssBlog" Title="Rss Blog" %>
<%@ Import Namespace="DBlog.TransitData" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<%@ OutputCache Duration="600" VaryByParam="*" VaryByCustom="Ticket" %>

<rss version="2.0">
  <channel>
    <title><% Response.Write(GetRssTitle()); %></title>
    <description><% Response.Write(SessionManager.GetSetting("description", string.Empty)); %></description>
    <link><% Response.Write(SessionManager.GetSetting("url", string.Empty)); %></link>
    <language>en-us</language>
    <image>
      <url><% Response.Write(SessionManager.GetSetting("image", string.Empty)); %></url>
      <title><% Response.Write(SessionManager.GetSetting("title", string.Empty)); %></title>
      <link><% Response.Write(SessionManager.GetSetting("url", string.Empty)); %></link>
      <width><% Response.Write(SessionManager.GetSetting("imagewidth", string.Empty)); %></width>
      <height><% Response.Write(SessionManager.GetSetting("imageheight", string.Empty)); %></height>
    </image>
    <asp:Repeater id="repeater" runat="server">
     <ItemTemplate>
      <item>
       <title><%# Renderer.Render(Eval("Title")) %></title>
       <pubDate><%# Renderer.ToRfc822((DateTime) Eval("Created")) %></pubDate>
       <description>
         <![CDATA[
          <!--[if lt IE 7 ]><html class="ie ie6" lang="en"> <![endif]-->
          <!--[if IE 7 ]><html class="ie ie7" lang="en"> <![endif]-->
          <!--[if IE 8 ]><html class="ie ie8" lang="en"> <![endif]-->
          <!--[if (gte IE 9)|!(IE)]><!--><html lang="en"> <!--<![endif]-->
          <link rel="stylesheet" href="<%# SessionManager.WebsiteUrl %>stylesheets/base.css" />
          <link rel="stylesheet" href="<%# SessionManager.WebsiteUrl %>stylesheets/skeleton.css" />
          <link rel="stylesheet" href="<%# SessionManager.WebsiteUrl %>stylesheets/layout.css" />
          <link rel="apple-touch-icon" href="<%# SessionManager.WebsiteUrl %>images/apple-touch-icon.png" />
          <link rel="apple-touch-icon" sizes="72x72" href="<%# SessionManager.WebsiteUrl %>images/apple-touch-icon-72x72.png" />
          <link rel="apple-touch-icon" sizes="114x114" href="<%# SessionManager.WebsiteUrl %>images/apple-touch-icon-114x114.png" />
          <%# Eval("BodyXHTML") %>
         ]]>
       </description>
       <%# GetCategories((TransitTopic[]) Eval("Topics")) %>
       <link><%# SessionManager.WebsiteUrl %><%# Eval("LinkUri")%></link>
       <guid isPermaLink="false"><%# SessionManager.WebsiteUrl %>Post/<%# Eval("Id") %></guid>
      </item>
     </ItemTemplate>
    </asp:Repeater>    
  </channel>
</rss>
