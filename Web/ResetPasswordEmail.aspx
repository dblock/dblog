<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ResetPasswordEmail.aspx.cs"
 Inherits="ResetPasswordEmail" Title="Reset Password" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Reset Password
 </div>
 <div class="subtitle">
  <asp:HyperLink ID="linkCancel" runat="server" Text="&#171; Cancel" />
 </div>
 <asp:UpdatePanel runat="server" ID="panelResetPassword" UpdateMode="Always">
  <ContentTemplate>
   <table class="table">
    <tr>
     <td class="table_tr_td_label">
      e-mail:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputEmail" />
     </td>
    </tr>
    <tr>
     <td class="table_tr_td_label">
     </td>
     <td valign="top" class="table_tr_td_value">
      <div class="description">
       An e-mail with a link to reset your password will be sent to your registered e-mail address.
      </div>
     </td>
    </tr>
    <tr>
     <td class="table_tr_td_label">
     </td>
     <td valign="top" class="table_tr_td_value">
      <Controls:WorkingButton runat="server" ID="reset" Text="Reset" CssClass="button"
       OnClick="reset_Click" />
     </td>
    </tr>
   </table>
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
