<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AtomBlog.aspx.cs" Inherits="AtomBlog"
    Title="Atom Blog" %>

<%@ Import Namespace="DBlog.TransitData" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<feed xml:lang="en-us" version="0.3" xmlns="http://purl.org/atom/ns#">
 <title><% Response.Write(GetRssTitle()); %></title>
 <link rel="alternate" type="application/xhtml+xml" href="<% Response.Write(SessionManager.GetSetting("url", string.Empty)); %>AtomPost.aspx" />
 <asp:Repeater id="repeater" runat="server">
   <ItemTemplate>
    <entry>
     <id><%# SessionManager.WebsiteUrl %>Post/<%# Eval("Id") %></id>
     <title><%# Renderer.Render(Eval("Title")) %></title>
     <created><%# ((DateTime) Eval("Created")).ToString("s") %> GMT</created>
     <modified><%# ((DateTime) Eval("Modified")).ToString("s") %> GMT</modified>
     <issued><%# ((DateTime) Eval("Created")).ToString("s") %> GMT</issued>
     <author>
      <name><%# SessionManager.GetSetting("author", string.Empty) %></name>
     </author>
     <%# GetCategories((TransitTopic[]) Eval("Topics")) %>
     <content type="text/html" mode="xhtml">
      <body xmlns="http://www.w3.org/1999/xhtml">
       <![CDATA[
        <link rel="stylesheet" href='<%# SessionManager.WebsiteUrl %>Style.css' />
        <%# Eval("BodyXHTML") %>
       ]]>
      </body>
     </content>
     <link rel="alternate" type="text/html" href='<%# SessionManager.WebsiteUrl %>ShowPost.aspx?Id=<%# Eval("Id") %>' />
     <link rel="edit" href='<%# SessionManager.WebsiteUrl %>AtomPost.aspx?Id=<%# Eval("Id") %>' />
    </entry>
   </ItemTemplate>
  </asp:Repeater>    
</feed>
