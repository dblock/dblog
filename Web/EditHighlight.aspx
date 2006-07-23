<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditHighlight.aspx.cs"
 Inherits="EditHighlight" Title="Edit Highlight" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="Upload" Src="UploadControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Highlight
 </div>
 <div class="link">
  <a href="ManageHighlights.aspx">&#187; Cancel</a>
 </div>
 <table class="table">
  <tr>
   <td class="table_tr_td_label">
    title:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputTitle" />
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
    image:
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:Upload CssClass="upload" runat="server" ID="inputImage" />
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
