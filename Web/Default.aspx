<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Title="" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <link rel="stylesheet" href="Style.css" type="text/css">
 <meta http-equiv="refresh" content="2;url=ShowBlog.aspx">
 <link id="linkRss" runat="server" rel="alternate" type="application/rss+xml" title="(RSS)" href="RssBlog.aspx" /> 
 <link id="linkAtom" runat="server" rel="alternate" type="application/atom+xml" title="(Atom)" href="RssBlog.aspx" /> 
</head>
<body bgcolor="#FFFFFF">
 <center>
  <table>
   <tr>
    <td align="left" height="100">
     &nbsp;</td>
   </tr>
   <tr>
    <td align="center" width="600" height="100">
     <a href="ShowBlog.aspx">
      <img src='<% Response.Write(Renderer.Render(SessionManager.GetSetting("image", "images/blog/blog.gif"))); %>'
       width='<% Response.Write(Renderer.Render(SessionManager.GetSetting("imagewidth", "72"))); %>'
       height='<% Response.Write(Renderer.Render(SessionManager.GetSetting("imageheight", "49"))); %>'
       align="absmiddle" alt='<% Response.Write(Renderer.Render(SessionManager.GetSetting("description", ""))); %>'
       border="0"></a>
     <div style="margin: 10px;">
      <% Response.Write(Renderer.Render(SessionManager.GetSetting("title", ""))); %>
     </div>
     <div style="margin: 10px; font-weight: bold;">
      <a href="ShowBlog.aspx">click click click</a>
     </div>
    </td>
   </tr>
   <tr>
    <td align="center">
     <font size="0.75em">
      <% Response.Write(Renderer.Render(SessionManager.GetSetting("description", ""))); %>
      <br>
      <% Response.Write(Renderer.Render(SessionManager.GetSetting("copyright", ""))); %>
     </font>
    </td>
   </tr>
  </table>
 </center>
</body>
</html>
