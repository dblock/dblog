<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewFeedItemsControl.ascx.cs"
 Inherits="ViewFeedItemsControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:UpdatePanel ID="panelItems" runat="server" UpdateMode="Conditional">
 <ContentTemplate>
  <Controls:PagedGrid runat="server" ID="grid" CssClass="table_feeditems" AutoGenerateColumns="false"
   AllowPaging="true" AllowCustomPaging="true" ShowHeader="false" PageSize="5">
   <ItemStyle HorizontalAlign="Center" CssClass="table_feeditems_tr_td" />
   <AlternatingItemStyle HorizontalAlign="Center" CssClass="table_feeditems_tr_td_alt" />
   <PagerStyle CssClass="table_pager" Position="Bottom" 
    HorizontalAlign="Center" PageButtonCount="3" />
    <Columns>
    <asp:BoundColumn DataField="Id" Visible="false" />
    <asp:TemplateColumn>
     <itemtemplate>
      <a href='<%# Renderer.Render(Eval("Link")) %>'>
       <%# Renderer.Render(Eval("Title")) %>
      </a>
     </itemtemplate>
    </asp:TemplateColumn>
   </Columns>
  </Controls:PagedGrid>
 </ContentTemplate>
</asp:UpdatePanel>
