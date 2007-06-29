<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoticeControl.ascx.cs" Inherits="NoticeControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<Tools:PersistentPanel id="panelNotice" runat="server" Visible="false">
 <script language="javascript">
  function CollapseExpandDetail(id)
  {
   var panel = document.getElementById(id);
   panel.style.cssText = (panel.style.cssText == "") ? "display: none;" : "";
  }
 </script>
 <table runat="server" ID="tableNotice" cellpadding="4">
  <tr>
   <td>
    <a href="#" runat="server" id="linkCollapseExpand">
     <asp:Image ID="imageMessage" BorderWidth="0" Width="24" Height="24" runat="server" />
    </a>
   </td>
   <td class="table_notice_tr_td">
    <asp:Label ID="labelMessage" runat="server" />
    <div class="sncore_description" runat="server" id="divDetail" style="display: none;">
     <asp:Label ID="labelDetail" runat="server" />
    </div>
   </td>
  </tr>
 </table>
</Tools:PersistentPanel>
