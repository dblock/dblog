<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Title="" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<%@ OutputCache Duration="600" VaryByParam="*" VaryByCustom="Ticket" %>

<!--[if lt IE 7 ]><html class="ie ie6" lang="en"> <![endif]-->
<!--[if IE 7 ]><html class="ie ie7" lang="en"> <![endif]-->
<!--[if IE 8 ]><html class="ie ie8" lang="en"> <![endif]-->
<!--[if (gte IE 9)|!(IE)]><!--><html lang="en"> <!--<![endif]-->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <meta charset="utf-8" />
 <!--[if lt IE 9]>
    <script src="javascripts/html5.js"></script>
 <![endif]-->
 <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" /> 
 <link rel="stylesheet" href="stylesheets/base.css" />
 <link rel="stylesheet" href="stylesheets/skeleton.css" />
 <link rel="stylesheet" href="stylesheets/layout.css" />
 <link rel="shortcut icon" href="images/favicon.ico" />
 <link rel="apple-touch-icon" href="images/apple-touch-icon.png" />
 <link rel="apple-touch-icon" sizes="72x72" href="images/apple-touch-icon-72x72.png" />
 <link rel="apple-touch-icon" sizes="114x114" href="images/apple-touch-icon-114x114.png" />
 <meta http-equiv="refresh" content="1;url=ShowBlog.aspx">
 <link id="linkRss" runat="server" rel="alternate" type="application/rss+xml" title="(RSS)" href="RssBlog.aspx" /> 
 <link id="linkAtom" runat="server" rel="alternate" type="application/atom+xml" title="(Atom)" href="AtomPost.aspx" /> 
 <link id="linkAtomPost" runat="server" rel="service" type="application/atomsvc+xml" href="AtomSvc.aspx">
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
