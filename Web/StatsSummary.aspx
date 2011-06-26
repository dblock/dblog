<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="StatsSummary.aspx.cs"
 Inherits="StatsSummary" Title="Statistics - Summary" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Summary
 </div>
 <div id="summaryLinks" class="link" style="margin-bottom: 20px;" runat="server">
    <a href="StatsSummary.aspx" disabled="disabled">Summary</a>
  | <a href="StatsHits.aspx?type=Hourly">Hourly</a>
  | <a href="StatsHits.aspx?type=Daily">Daily</a>
  | <a href="StatsHits.aspx?type=Weekly">Weekly</a>
  | <a href="StatsHits.aspx?type=Monthly">Monthly</a>
  | <a href="StatsHits.aspx?type=Yearly">Yearly</a>
  | <a href="StatsReferrerHosts.aspx">Referrer Hosts</a>
  | <a href="StatsReferrerSearchQueries.aspx">Search Queries</a>
 </div>
 <Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
  AllowPaging="true" AllowCustomPaging="true" PageSize="10" CellPadding="2" ShowHeader="true">
  <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
  <ItemStyle HorizontalAlign="Center" Font-Bold="True" CssClass="table_tr_td" />
  <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next" PrevPageText="Prev"
   HorizontalAlign="Center" />
  <Columns>
   <asp:TemplateColumn>
    <itemtemplate>
     <%# Renderer.Render(Eval("Text")) %>
    </itemtemplate>
   </asp:TemplateColumn>
   <asp:TemplateColumn>
    <itemtemplate>
     <%# Renderer.Render(Eval("Value")) %>
    </itemtemplate>
   </asp:TemplateColumn>
  </Columns>
 </Controls:PagedGrid>
</asp:Content>
