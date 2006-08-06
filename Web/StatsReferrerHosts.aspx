<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="StatsReferrerHosts.aspx.cs"
 Inherits="StatsReferrerHosts" Title="Statistics - Referrer Hosts" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Referrer Hosts
 </div>
 <div class="link" style="margin-bottom: 20px;">
    <a href="StatsSummary.aspx">Summary</a>
  | <a href="StatsHits.aspx?type=Hourly">Hourly</a>
  | <a href="StatsHits.aspx?type=Daily">Daily</a>
  | <a href="StatsHits.aspx?type=Weekly">Weekly</a>
  | <a href="StatsHits.aspx?type=Monthly">Monthly</a>
  | <a href="StatsHits.aspx?type=Yearly">Yearly</a>
  | <a href="StatsReferrerHosts.aspx" disabled="disabled">Referrer Hosts</a>
  | <a href="StatsReferrerSearchQueries.aspx">Search Queries</a>
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
    AllowPaging="true" AllowCustomPaging="true" PageSize="10" CellPadding="2" ShowHeader="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" Font-Bold="True" CssClass="table_tr_td" />
    <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next" PrevPageText="Prev"
     HorizontalAlign="Center" />
    <Columns>
     <asp:BoundColumn DataField="Id" Visible="false" />
     <asp:TemplateColumn ItemStyle-Width="25px">
      <itemtemplate>
       <img src="images/site/item.gif" />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn HeaderText="Referrer Host">
      <itemtemplate>
       <a href='http://<%# Renderer.Render(Eval("Name")) %>'>
        <%# Renderer.Render(Eval("Name")) %>
       </a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="120">
      <itemtemplate>
       <a target="_blank" href='<%# Renderer.Render(Eval("LastSource")) %>'>
        Referrer
       </a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="120">
      <itemtemplate>
       <a target="_blank" href='<%# Renderer.Render(Eval("LastUrl")) %>'>
        Destination
       </a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn>
      <itemtemplate>
       <%# Eval("RequestCount") %>
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </atlas:UpdatePanel>
</asp:Content>
