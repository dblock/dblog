<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowEntry.aspx.cs"
 Inherits="ShowEntry" Title="Blog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <asp:Panel ID="panelRedirect" runat="Server">
  Please <asp:HyperLink ID="linkRedirect" runat="server" Text="click here" /> 
  if you're not redirected automatically.
 </asp:Panel>
</asp:Content>