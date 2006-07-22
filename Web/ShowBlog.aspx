<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowBlog.aspx.cs"
 Inherits="ShowBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" TagName="Blogs" Src="ViewBlogsControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="Topics" Src="ViewTopicsControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <atlas:UpdatePanel Mode="Conditional" runat="server" ID="panelBlogs" RenderMode="Inline">
   <ContentTemplate>
    <Controls:Blogs runat="server" ID="blogs" />
   </ContentTemplate>
  </atlas:UpdatePanel>
</asp:Content>
