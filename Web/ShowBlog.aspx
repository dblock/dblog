<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowBlog.aspx.cs"
 Inherits="ShowBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" TagName="Topics" Src="ViewTopicsControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="Search" Src="SearchControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="DateRange" Src="DateRangeControl.ascx" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="panelPosts" RenderMode="Inline">
   <ContentTemplate>
    <asp:Label ID="labelPosts" CssClass="title" runat="server" Visible="false" />
    <Controls:PagedGrid runat="server" ID="grid" CssClass="table" 
     AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" 
     PageSize="7" ShowHeader="false" OnItemCommand="grid_ItemCommand" 
     BorderWidth="0" BorderColor="White">
     <ItemStyle CssClass="table_tr_td" />
     <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
      PrevPageText="Prev" HorizontalAlign="Center" />
     <Columns>
      <asp:BoundColumn DataField="Id" Visible="false" />
      <asp:TemplateColumn>
       <itemtemplate>
        <div class="post_title">
         <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
          <%# Renderer.RenderEx(Eval("Title")) %>
         </a>
        </div>
        <div class="post_subtitle">
         <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
          Read
         </a>
         | <%# SessionManager.Region.UtcToUser((DateTime) Eval("Created")).ToString("f") %>
         <%# GetLink((int)Eval("CommentsCount"), (int)Eval("ImagesCount"))%>
         <a href='EditPostComment.aspx?sid=<%# Eval("Id") %>'>
          | Post Comment
         </a>
         | <%# GetCounter((long) Eval("Counter.Count")) %>
         <span id="Span1" runat="server" style='<%# (bool) SessionManager.IsAdministrator ? String.Empty : "display: none;" %>'>
          | <a href='EditPost.aspx?id=<%# Eval("Id") %>'>
           Edit
          </a>
          | <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
           runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
         </span>
        </div>
        <div class="post_body">
         <%# Renderer.RenderEx(Eval("Body")) %>
        </div>
        <asp:Panel CssClass="post_image" Width="100%" id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
         <a href='<%# GetPostLink((int) Eval("ImagesCount"), (int) Eval("Id"), (int) Eval("ImageId")) %>'>
          <img border="0" src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
         </a>
         <div class="link">
          <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
           <%# GetImagesLink((int) Eval("ImagesCount")) %>
          </a>
         </div>
        </asp:Panel>
       </itemtemplate>
      </asp:TemplateColumn>
     </Columns>
    </Controls:PagedGrid>
   </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>
