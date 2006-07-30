<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowBlog.aspx.cs"
 Inherits="ShowBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" TagName="Topics" Src="ViewTopicsControl.ascx" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <atlas:UpdatePanel Mode="Conditional" runat="server" ID="panelPosts" RenderMode="Inline">
   <ContentTemplate>
    <Controls:PagedGrid runat="server" ID="grid" CssClass="table" BorderWidth="0px" AutoGenerateColumns="false"
     AllowPaging="true" AllowCustomPaging="true" PageSize="7" ShowHeader="false" OnItemCommand="grid_ItemCommand">
     <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
     <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
      PrevPageText="Prev" HorizontalAlign="Center" />
     <Columns>
      <asp:BoundColumn DataField="Id" Visible="false" />
      <asp:TemplateColumn>
       <itemtemplate>
        <table class="table_post">
         <tr>
          <td valign="top" class="table_post_tr_td_left">
           <div class="post_title">
            <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
             <%# this.Render((int) Eval("Id"), "Post", (string) Eval("Title")) %>
            </a>
           </div>
           <div class="post_subtitle">
            <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
             Read
            </a>
            | <%# ((DateTime) Eval("Created")).ToString("d") %>
            <%# GetLink((int)Eval("CommentsCount"), (int)Eval("ImagesCount"))%>
            <a href='EditPostComment.aspx?sid=<%# Eval("Id") %>'>
             | New Comment
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
            <%# this.Render((int)Eval("Id"), "Post", (string)Eval("Body"))%>
           </div>
          </td>
          <td class="table_post_tr_td_right">
           <asp:Panel Width="100%" id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
            <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
             <img border="0" src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
            </a>
           </asp:Panel>
          </td>
         </tr>
        </table>
       </itemtemplate>
      </asp:TemplateColumn>
     </Columns>
    </Controls:PagedGrid>
   </ContentTemplate>
  </atlas:UpdatePanel>
</asp:Content>
