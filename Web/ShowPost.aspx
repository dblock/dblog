<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowPost.aspx.cs"
 Inherits="ShowPost" Title="Post" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="TransitData" Namespace="DBlog.TransitData" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <table class="table_post">
  <tr>
   <td valign="top" class="table_post_tr_td_left">
    <div class="post_title">
     <asp:Label ID="posttitle" runat="server" />
    </div>
    <div class="post_subtitle">
     <a href="ShowBlog.aspx">Back</a> |
     <asp:Label ID="postcreated" runat="server" /> |
     <asp:Label ID="postcounter" runat="server" /> |
     <asp:Hyperlink ID="linkComment" runat="server" Text="New Comment" />
    </div>
    <div class="post_body">
     <asp:Label ID="postbody" runat="server" />
    </div>
   </td>
   <td class="table_post_tr_td_right">
    <asp:Panel ID="panelPicture" runat="server">
     <a href='' runat="server" id="linkimage">
      <asp:Image BorderWidth="0" runat="server" ID="postimage" />
     </a>
    </asp:Panel>
   </td>
  </tr>
 </table>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="images" CssClass="table" AllowCustomPaging="true"
    RepeatColumns="4" RepeatRows="3" CellPadding="2" ShowHeader="true" Font-Bold="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />
    <ItemTemplate>
     <a href='ShowImage.aspx?id=<%# Eval("Image.Id") %>&pid=<%# Eval("Post.Id") %>&index=<%# Eval("Index") %>'>
      <img border="0" alt='<%# Renderer.Render(Eval("Image.Description")) %>' src='ShowPicture.aspx?id=<%# Eval("Image.Id") %>' />
     </a>
     <div class="link_small">
      <a href='ShowImage.aspx?id=<%# Eval("Image.Id") %>&pid=<%# Eval("Post.Id") %>&index=<%# Eval("Index") %>'>
       <div>
        <%# Renderer.Render(Eval("Image.Name")) %>
        <%# GetCounter((TransitImage) Eval("Image")) %>
       </div>
       <div>
        <%# GetComments((TransitImage) Eval("Image")) %>
       </div>
      </a>
     </div>
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </atlas:UpdatePanel>
 <atlas:UpdatePanel runat="server" ID="panelComments" Mode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="comments" BorderWidth="0" CssClass="table" 
    AllowPaging="false" AllowCustomPaging="false" CellPadding="2" ShowHeader="false" 
    AutoGenerateColumns="false">
    <ItemStyle HorizontalAlign="Left" CssClass="table_tr_td" />
    <PagerStyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />    
    <Columns>
     <asp:TemplateColumn>
      <ItemTemplate>
       <table width="100%">
        <tr>
         <td width='<%# (int) Eval("CommentLevel") * 10 %>'></td>
         <td width="*">
          <div class="table_comments">
           <div class="table_comments_body">
            <%# Renderer.RenderEx(Eval("CommentText")) %>
           </div>
           <div class="table_comments_note">
            <a href='<%# Renderer.Render(Eval("CommentLoginWebsite")) %>'>
             <%# Renderer.Render(Eval("CommentLoginName")) %>
            </a>
            @
            <%# ((DateTime) Eval("CommentCreated")).ToString("d") %>
            <a href='EditPostComment.aspx?sid=<%# Eval("PostId") %>&pid=<%# Eval("CommentId") %>'>
             Reply
            </a>
           </div>
          </div>
         </td>
        </tr>
       </table>
      </ItemTemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </atlas:UpdatePanel> 
</asp:Content>
