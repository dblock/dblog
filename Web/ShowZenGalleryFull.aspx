﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowZenGalleryFull.aspx.cs"
    Inherits="ShowZenGalleryFull" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="Style.css" type="text/css">
</head>
<body style="vertical-align: middle; text-align: center; background-color: White;">
    <form id="form1" runat="server">
    <center>
        <table>
            <tr>
                <td>
                    <object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"
                        width="800" height="600">
                        <param name="movie" value="galleries/ZenGallery.swf">
                        <param name="quality" value="high">
                        <param name="scale" value="noscale">
                        <param name="bgcolor" value="#FFFFFF">
                        <param name="flashVars" value="XMLFile=<% Response.Write(GalleryXml); %>">
                        <embed src="galleries/ZenGallery.swf" width="800" height="600" flashvars="XMLFile=<% Response.Write(GalleryXml); %>"
                            quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash"
                            scale="noscale" bgcolor="#FFFFFF"></embed>
                    </object>
                </td>
                <td align="center" width="200">
                    <a href="ShowBlog.aspx">
                        <img src='<% Response.Write(Renderer.Render(SessionManager.GetSetting("image", "images/blog/blog.gif"))); %>'
                            width='<% Response.Write(Renderer.Render(SessionManager.GetSetting("imagewidth", "72"))); %>'
                            height='<% Response.Write(Renderer.Render(SessionManager.GetSetting("imageheight", "49"))); %>'
                            align="absmiddle" alt='<% Response.Write(Renderer.Render(SessionManager.GetSetting("description", ""))); %>'
                            border="0"></a>
                    <div style="margin: 10px;">
                        <% Response.Write(Renderer.Render(SessionManager.GetSetting("title", ""))); %>
                    </div>
                    <div style="font-weight: bold;">
                        <a href="ShowBlog.aspx">Blog</a>
                    </div>
                    <div style="font-weight: bold;">
                        <asp:LinkButton OnClick="linkContact_Click" ID="linkContact" runat="server" Text="Contact" />
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
    </form>
</body>
</html>
