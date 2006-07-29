<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditLogin.aspx.cs"
 Inherits="EditLogin" Title="Edit Login" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Login
 </div>
 <div class="subtitle">
  <asp:HyperLink ID="linkCancel" runat="server" Text="Cancel" />
 </div>
 <atlas:UpdatePanel runat="server" ID="panelLogin" Mode="Always">
  <ContentTemplate>
   <asp:Panel ID="panelAdmin" runat="server">
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
      </td>
      <td valign="top" class="table_tr_td_value">
       <asp:CheckBox runat="server" CssClass="checkbox" ID="inputAdministrator" Text="Administrator" />
      </td>
     </tr>
    </table>   
   </asp:Panel>
   <table class="table">
    <tr>
     <td class="table_tr_td_label">
      name:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputName" />
      <div class="description">
       this is how other users will see you
      </div>
     </td>
    </tr>
    <tr>
     <td class="table_tr_td_label">
      e-mail:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputEmail" />
      <div class="description">
       e-mail will not published
      </div>
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
      confirm password:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputPassword2" TextMode="password" />
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
 </atlas:UpdatePanel>
</asp:Content>
