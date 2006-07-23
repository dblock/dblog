<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditEntry.aspx.cs"
 Inherits="EditEntry" Title="Edit Entry" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="Upload" Src="UploadControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Entry
 </div>
 <div class="link">
  <a href="ManageEntries.aspx">&#187; Cancel</a>
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
    text:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" Rows="10" TextMode="MultiLine" ID="inputText" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    topic:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:DropDownList DataTextField="Name" DataValueField="Id" runat="server" CssClass="dropdown"
     ID="inputTopic" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    image:
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:Upload CssClass="upload" runat="server" ID="inputImage" />
    <div class="description">
     optional picture
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:WorkingButton runat="server" ID="save" Text="Save" CssClass="button" OnClick="save_Click" />
   </td>
  </tr>
 </table>
</asp:Content>
