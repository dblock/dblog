<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="About.aspx.cs"
 Inherits="About" Title="Blog - About" %>

<%@ OutputCache Duration="30" VaryByParam="*" VaryByCustom="Ticket" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <p class="title">
  <asp:Label ID="labelTitle" runat="Server" />
  <font size="-2">
    <div><asp:Label ID="labelDescription" runat="Server" /> | 
    <asp:Label ID="labelCopyright" runat="Server" /> | 
    up for <asp:Label ID="labelUptime" runat="Server" /></div>
  </font>
 </p>
 <p>
    <img src="images/site/about.jpg" />
 </p>
 <p>
  <asp:Label ID="labelDetails" runat="Server" />
 </p>
 <div class="title">
  Contact
 </div>
 <div>
  Please don't heistate to <asp:LinkButton ID="linkEmail" runat="Server" Text="email" /> or find me on 
  <asp:Hyperlink ID="linkTwitter" runat="Server" Text="twitter" />.
 </div>
</asp:Content>
