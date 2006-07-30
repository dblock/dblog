<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewFeedsControl.ascx.cs"
 Inherits="ViewFeedsControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="FeedItems" Src="ViewFeedItemsControl.ascx" %>
<Controls:PagedGrid runat="server" ID="grid" CssClass="table_feeds" AutoGenerateColumns="false"
 AllowPaging="false" ShowHeader="false" BorderWidth="0">
 <ItemStyle HorizontalAlign="Center" CssClass="table_feeds_tr_td" />
 <Columns>
  <asp:BoundColumn DataField="Id" Visible="false" />
  <asp:TemplateColumn>
   <itemtemplate>
    <div class="table_feeds_tr_th">
     <%# Renderer.Render(Eval("Name")) %>
    </div>
    <div class="description">
     <%# Renderer.Render(Eval("Description")) %>
    </div>
    <Controls:FeedItems runat="server" ID='feeditems' FeedId='<%# Eval("Id") %>' />
   </itemtemplate>
  </asp:TemplateColumn>
 </Columns>
</Controls:PagedGrid>
