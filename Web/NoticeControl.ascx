<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoticeControl.ascx.cs" Inherits="NoticeControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<Tools:PersistentPanel id="panelNotice" runat="server" Visible="false">
 <table runat="server" ID="tableNotice" cellpadding="4">
  <tr>
   <td class="table_notice_tr_td">
    <asp:Image ID="imageMessage" Width="24" Height="24" runat="server" />
   </td>
   <td class="table_notice_tr_td">
    <asp:Label ID="labelMessage" runat="server" />
   </td>
  </tr>
 </table>
</Tools:PersistentPanel>
