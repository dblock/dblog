<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="StatsHits.aspx.cs"
 Inherits="StatsHits" Title="Statistics - Hits" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <atlas:UpdatePanel runat="server" ID="panelChart" Mode="Always">
  <ContentTemplate>
   <div class="title">
    <asp:Label ID="labelChartType" runat="server" Text="Daily" />
   </div>
   <div class="link">
    <asp:LinkButton OnClick="linkHourly_Click" ID="linkHourly" runat="server" Text="Hourly" />
    | <asp:LinkButton OnClick="linkDaily_Click" ID="linkDaily" runat="server" Text="Daily" />
    | <asp:LinkButton OnClick="linkWeekly_Click" ID="linkWeekly" runat="server" Text="Weekly" />
    | <asp:LinkButton OnClick="linkMonthly_Click" ID="linkMonthly" runat="server" Text="Monthly" />
    | <asp:LinkButton OnClick="linkYearly_Click" ID="linkYearly" runat="server" Text="Yearly" />
    | <asp:HyperLink ID="linkRefererHosts" runat="server" Text="Referer Hosts" NavigateUrl="StatsReferrerHosts.aspx" />
    | <asp:HyperLink ID="linkRefererQueries" runat="server" Text="Search Queries" NavigateUrl="StatsReferrerSearchQueries.aspx" />
   </div>
   <table class="table" style="border: none;">
    <tr>
     <td>
      <img runat="server" id="imageStats" src="StatsChart.aspx?type=Daily" />
     </td>
    </tr>
   </table>
  </ContentTemplate>
 </atlas:UpdatePanel>
 <table class="table">
  <tr>
   <td class="description">
    note: all counter times are UTC
   </td>
  </tr>
 </table>
</asp:Content>
