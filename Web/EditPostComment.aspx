<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditPostComment.aspx.cs"
 Inherits="EditPostComment" Title="Edit Post Comment" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Post Comment
 </div>
 <div class="link">
  <asp:Hyperlink ID="linkCancel" runat="server" Text="&#171; Cancel" />
 </div>
 <table class="table" cellpadding="2">
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    from: <asp:Label ID="labelPostUsername" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    comment:
   </td>
   <td valign="top" class="table_tr_td_value">
    <ajaxToolkitHTMLEditor:Editor ID="inputText" runat="server" Height="400px" Width="600px" InitialCleanUp="true" />
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
