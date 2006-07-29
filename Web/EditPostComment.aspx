<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditPostComment.aspx.cs"
 Inherits="EditPostComment" Title="Edit Post Comment" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Post Comment
 </div>
 <div class="link">
  <asp:Hyperlink ID="linkCancel" runat="server" Text="Cancel" />
 </div>
 <table class="table" cellpadding="2">
  <tr>
   <td class="table_tr_td_label">
    comment:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" Rows="10" TextMode="MultiLine" ID="inputText" />
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
