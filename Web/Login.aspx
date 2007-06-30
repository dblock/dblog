<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="Login.aspx.cs"
 Inherits="BlogLogin" Title="Login" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Login
 </div>
 <div class="subtitle">
  <asp:HyperLink ID="linkNewUser" runat="server" Text="I don't have a login &#187; Register" />
 </div>
 <asp:UpdatePanel runat="server" ID="panelLogin" UpdateMode="Always">
  <ContentTemplate>
   <table class="table">
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
      <asp:TextBox runat="server" CssClass="textbox" ID="inputPassword" TextMode="password" />
     </td>
    </tr>
    <tr>
     <td class="table_tr_td_label">
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:CheckBox runat="server" CssClass="checkbox" ID="inputRememberMe" Text="remember me"
       Checked="false" />
     </td>
    </tr>
    <tr>
     <td class="table_tr_td_label">
     </td>
     <td valign="top" class="table_tr_td_value">
      <Controls:WorkingButton runat="server" ID="button" Text="Login" CssClass="button"
       OnClick="button_Click" />
     </td>
    </tr>
   </table>
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
