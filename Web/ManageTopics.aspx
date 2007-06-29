<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageTopics.aspx.cs"
 Inherits="admin_ManageTopics" Title="Manage Topics" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Topics
 </div>
 <div class="link">
  <a href="EditTopic.aspx">&#187; Create New</a>
 </div>
 <asp:UpdatePanel runat="server" ID="panelGrid" UpdateMode="Always">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
    AllowPaging="true" AllowCustomPaging="true" PageSize="20" CellPadding="2"
    OnItemCommand="grid_ItemCommand" ShowHeader="true">
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
     <asp:TemplateColumn HeaderText="Topic" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <%# Renderer.Render(Eval("Name")) %>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <a href="EditTopic.aspx?id=<%# Eval("Id") %>">Edit</a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
