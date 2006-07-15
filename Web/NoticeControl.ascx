<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoticeControl.ascx.cs" Inherits="NoticeControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<Tools:PersistentPanel id="panelNotice" runat="server" Visible="false">
 <table width="100%">
  <tr>
   <td>
    <asp:Image ID="imageMessage" Width="24" Height="24" runat="server" />
   </td>
   <td>
    <asp:Label ID="labelMessage" runat="server" />
   </td>
  </tr>
 </table>
</Tools:PersistentPanel>
