<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageReferences.aspx.cs"
 Inherits="admin_ManageReferences" Title="Manage References" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  References
 </div>
 <div class="link">
  <a href="EditReference.aspx">Create New</a>
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always">
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
     <asp:TemplateColumn HeaderText="Word" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <a href='<%# Renderer.Render(Eval("Url")) %>' target="_blank">
        <%# Renderer.Render(Eval("Word")) %>
       </a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn HeaderText="Url" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <%# Renderer.Render(Eval("Url")) %>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn HeaderText="Result" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <%# Renderer.Render(Eval("Result")) %>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <a href="EditReference.aspx?id=<%# Eval("Id") %>">Edit</a>
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
 </atlas:UpdatePanel>
</asp:Content>
