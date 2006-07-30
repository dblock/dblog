<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageFeeds.aspx.cs"
 Inherits="admin_ManageFeeds" Title="Manage Feeds" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Feeds
 </div>
 <div class="link">
  <a href="EditFeed.aspx">&#187; Create New</a>
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
    AllowPaging="true" AllowCustomPaging="true" PageSize="10" CellPadding="2"
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
     <asp:TemplateColumn HeaderText="Feed">
      <itemtemplate>
       <a href='<%# Renderer.Render(Eval("Url")) %>' target="_blank">
        <%# Renderer.Render(Eval("Name")) %>
       </a>
       <div class="description">
        <%# Renderer.Render(Eval("Description")) %>
       </div>
       <div class="description" style="color: red;">
        <%# Renderer.Render(Eval("Exception")) %>
       </div>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <a href="EditFeed.aspx?id=<%# Eval("Id") %>">Edit</a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkUpdate" CommandName="Update" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Update" />
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </atlas:UpdatePanel>
</asp:Content>
