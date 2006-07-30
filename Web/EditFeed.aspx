<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditFeed.aspx.cs"
 Inherits="EditFeed" Title="Edit Feed" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="Upload" Src="UploadControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Feed
 </div>
 <div class="link">
  <a href="ManageFeeds.aspx">Cancel</a>
 </div>
 <table class="table">
  <tr>
   <td class="table_tr_td_label">
    name:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputName" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    url:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputUrl" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    description:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputDescription" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    xsl:
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:Upload CssClass="upload" runat="server" ID="inputXsl" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    interval:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:DropDownList DataTextField="text" DataValueField="value" 
     CssClass="dropdown" ID="inputInterval" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    type:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:DropDownList CssClass="dropdown" ID="inputType" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    username:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputUsername" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    password:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputPassword" TextMode="Password" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:WorkingButton runat="server" ID="save" Text="Save" CssClass="button"
     OnClick="save_Click" />
   </td>
  </tr>
 </table>
</asp:Content>
