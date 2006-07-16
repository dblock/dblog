<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BlogsViewControl.ascx.cs"
 Inherits="BlogsViewControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
 AllowPaging="true" AllowCustomPaging="true" PageSize="4">
 <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
 <HeaderStyle CssClass="table_tr_th" />
 <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
  PrevPageText="Prev" HorizontalAlign="Center" />
 <Columns>
  <asp:BoundColumn DataField="Id" Visible="false" />
  <asp:TemplateColumn HeaderText="Stuff">
   <itemtemplate>
    <%# Renderer.Render(Eval("Title")) %>
   </itemtemplate>
  </asp:TemplateColumn>
 </Columns>
</Controls:PagedGrid>
