<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditReference.aspx.cs"
 Inherits="EditReference" Title="Edit Reference" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Reference
 </div>
 <div class="link">
  <a href="ManageReferences.aspx">Cancel</a>
 </div>
 <asp:UpdatePanel runat="server" ID="panelReference" UpdateMode="Always">
  <ContentTemplate>
   <table class="table">
    <tr>
     <td class="table_tr_td_label">
      word:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputWord" />
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
      result:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputResult" />
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
