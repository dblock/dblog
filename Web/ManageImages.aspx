<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageImages.aspx.cs"
 Inherits="admin_ManageImages" Title="Manage Images" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Images
 </div>
 <div class="link">
  <a href="EditImage.aspx">Upload New</a>
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
    AllowCustomPaging="true" RepeatColumns="4" RepeatRows="3" CellPadding="2" OnItemCommand="grid_ItemCommand" 
    ShowHeader="true" Font-Bold="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next" PrevPageText="Prev"
     HorizontalAlign="Center" />
    <ItemTemplate>
     <a href='ShowImage.aspx?id=<%# Eval("Id") %>&r=ManageImages.aspx'>
      <img border="0" src='ShowPicture.aspx?id=<%# Eval("Id") %>' 
       alt='<%# Renderer.Render(Eval("Description")) %>' />
     </a>
     <div>
      <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
       runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
     </div>
     <div>
      <a href='EditImage.aspx?id=<%# Eval("Id") %>'>Edit</a>
     </div>
     </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </atlas:UpdatePanel>
</asp:Content>
