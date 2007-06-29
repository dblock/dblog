<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditReferrerHostRollup.aspx.cs"
 Inherits="EditReferrerHostRollup" Title="Edit ReferrerHostRollup" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  ReferrerHostRollup
 </div>
 <div class="link">
  <a href="ManageReferrerHostRollups.aspx">Cancel</a>
 </div>
 <asp:UpdatePanel runat="server" ID="panelReferrerHostRollup" UpdateMode="Always">
  <ContentTemplate>
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
      rollup:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputRollup" />
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
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
