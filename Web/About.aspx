<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="About.aspx.cs"
 Inherits="About" Title="Blog - About" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  <asp:Label ID="labelTitle" runat="Server" />
 </div>
 <div class="subtitle">
  <asp:Label ID="labelDescription" runat="Server" />
 </div>
 <div class="subtitle">
  <asp:Label ID="labelCopyright" runat="Server" />
 </div>
 <div class="title">
  Contact
 </div>
 <div class="subtitle">
  <asp:LinkButton ID="linkEmail" runat="Server" Text="E-Mail" />
 </div>
 <div class="title">
  Software
 </div>
 <div class="subtitle">
  DBlog.NET, written in C#, ASP.NET by dB.
  <br />
  <a href="http://www.dblock.org/" target="_blank">www.dblock.org</a>
 </div>
</asp:Content>
