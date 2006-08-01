<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AtomBlog.aspx.cs"
 Inherits="AtomBlog" Title="Atom Blog" %>

<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<feed version="0.3" xml:lang="en-us" xmlns="http://purl.org/atom/ns#" >
 <title><% Response.Write(SessionManager.GetSetting("title", "Untitled")); %></title>
 <modified></modified>
 <link rel="alternate" type="application/xhtml+xml" href="<% Response.Write(SessionManager.GetSetting("url", string.Empty)); %>" />
  <asp:Repeater id="repeater" runat="server">
   <ItemTemplate>
    <entry>
     <id><%# SessionManager.GetSetting("url", string.Empty) %>Post/<%# Eval("Id") %></id>
     <title><%# Renderer.RenderEx(Eval("Title")) %></title>
     <created><%# ((DateTime) Eval("Created")).ToString("s") %> GMT</created>
     <modified><%# ((DateTime) Eval("Modified")).ToString("s") %> GMT</modified>
     <issued><%# ((DateTime) Eval("Created")).ToString("s") %> GMT</issued>
     <author>
      <name><%# SessionManager.GetSetting("author", string.Empty) %></name>
     </author>
     <content type="text/html" mode="xml">
      <body xmlns="http://www.w3.org/1999/xhtml">
       <![CDATA[
        <link rel="stylesheet" href='<%# SessionManager.GetSetting("url", string.Empty) %>Style.css'>
        <%# Renderer.RenderEx(Eval("Body")) %>
       ]]>
      </body>
     </content>
     <link rel="alternate" type="text/html" href='<%# SessionManager.GetSetting("url", string.Empty) %>ShowPost.aspx?Id=<%# Eval("Id") %>' />
    </entry>
   </ItemTemplate>
  </asp:Repeater>    
</feed>
