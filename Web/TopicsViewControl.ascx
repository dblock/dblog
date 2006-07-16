<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopicsViewControl.ascx.cs"
 Inherits="TopicsViewControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<Controls:PagedGrid runat="server" ID="grid" CssClass="table_topics" AutoGenerateColumns="false"
 AllowPaging="false" OnSelectedIndexChanged="grid_SelectedIndexChanged">
 <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
 <AlternatingItemStyle HorizontalAlign="Center" CssClass="table_topics_tr_td_alt" />
 <HeaderStyle CssClass="table_topics_tr_th" />
 <SelectedItemStyle HorizontalAlign="Center" CssClass="table_topics_tr_td_sel" />
 <Columns>
  <asp:BoundColumn DataField="Id" Visible="false" />
  <asp:TemplateColumn HeaderText="Stuff">
   <itemtemplate>
    <asp:LinkButton CommandName="Select" id="linkTopic" runat="server" Text='<%# Renderer.Render(Eval("Name")) %>' />
   </itemtemplate>
  </asp:TemplateColumn>
 </Columns>
</Controls:PagedGrid>
