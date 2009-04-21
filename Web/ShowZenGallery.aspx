<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowZenGallery.aspx.cs"
    Inherits="ShowZenGallery" Title="Blog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <object id="flashObject" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"
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
</asp:Content>
