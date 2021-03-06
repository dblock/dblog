<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageHighlights.aspx.cs"
 Inherits="admin_ManageHighlights" Title="Manage Highlights" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Highlights
 </div>
 <div class="link">
  <a href="EditHighlight.aspx">&#187; Create New</a>
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
       <img src="images/site/item.gif" alt='<%# Renderer.Render(Eval("Position")) %>' />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn>
      <itemtemplate>
       <img src='ShowPicture.aspx?id=<%# Renderer.Render(Eval("ImageId")) %>&ShowThumbnail=false&IncrementCounter=false' 
        alt='<%# Renderer.Render(Eval("Description")) %>' width="32" height="32" />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn HeaderText="Highlight">
      <itemtemplate>
       <a href='<%# Renderer.Render(Eval("Url")) %>' target="_blank">
        <%# Renderer.Render(Eval("Title")) %>
       </a>
       <div class="description">
        <%# Renderer.Render(Eval("Description")) %>
       </div>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <a href="EditHighlight.aspx?id=<%# Eval("Id") %>">Edit</a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="50px">
      <itemtemplate>
       <asp:LinkButton ID="linkUp" CommandName="Up" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Up" Visible='<%# (int) Eval("Position") != 1 %>' />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="50px">
      <itemtemplate>
       <asp:LinkButton ID="linkDown" CommandName="Down" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Down" Visible='<%# (int) Eval("Position") != Count %>' />
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
