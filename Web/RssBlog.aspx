<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RssBlog.aspx.cs"
 Inherits="RssBlog" Title="Rss Blog" %>

<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<rss version="2.0">
  <channel>
    <title><% Response.Write(SessionManager.GetSetting("title", "Untitled")); %></title>
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
       <title><%# Renderer.RenderEx(Eval("Title")) %></title>
       <pubDate><%# Renderer.ToRfc822((DateTime) Eval("Modified")) %></pubDate>
       <description>
         <![CDATA[
          <link rel="stylesheet" href="/blog.css">
          <%# Renderer.RenderEx(Eval("Body")) %>
         ]]>
       </description>
       <category><%# Renderer.Render(Eval("TopicName")) %></category>
       <link><%# SessionManager.GetSetting("url", string.Empty) %>ShowPost.aspx?Id=<%# Eval("Id") %></link>
       <guid isPermaLink="false"><%# SessionManager.GetSetting("url", string.Empty) %>Post/<%# Eval("Id") %></guid>
      </item>
     </ItemTemplate>
    </asp:Repeater>    
  </channel>
</rss>
