<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowPost.aspx.cs"
 Inherits="ShowPost" Title="Post" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="TransitData" Namespace="DBlog.TransitData" Assembly="DBlog.Tools" %>

<%@ Import Namespace="DBlog.TransitData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <table class="table_post">
  <tr>
   <td valign="top" class="table_post_tr_td_left">
    <div class="post_title">
     <asp:Label ID="posttitle" runat="server" />
    </div>
    <div class="post_subtitle">
     <asp:UpdatePanel runat="server" ID="panelButtons" UpdateMode="Always" RenderMode="Inline">
      <ContentTemplate>
       <a href="ShowBlog.aspx">Back</a> |
       <asp:Label ID="posttopics" runat="server" /> |
       <asp:Label ID="postcreated" runat="server" /> |
       <asp:Label ID="postcounter" runat="server" /> |
       <asp:Hyperlink ID="linkComment" runat="server" Text="Post Comment" /> |
       <span id="spanAdmin" runat="server">
        <asp:Hyperlink ID="linkEdit" runat="server" Text="Edit" /> |
       </span>
       <asp:LinkButton ID="linkPreferred" OnClick="linkPreferred_Click" runat="server" Enabled="true" Text="Favorites" />
      </ContentTemplate>
     </asp:UpdatePanel>
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
 <asp:UpdatePanel runat="server" ID="panelImages" UpdateMode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="images" CssClass="table" AllowCustomPaging="true"
    RepeatColumns="4" RepeatRows="3" CellPadding="2" ShowHeader="true" Font-Bold="true"
    OnItemCommand="images_OnItemCommand">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />
    <ItemTemplate>
     <a href='<%# GetImageUri((DBlog.TransitData.TransitPostImage) Container.DataItem) %>'>
      <img border="0" alt='<%# Renderer.Render(Eval("Image.Description")) %>' src='ShowPicture.aspx?id=<%# Eval("Image.Id") %>' />
     </a>
     <div class="link_small">
      <a href='<%# GetImageUri((DBlog.TransitData.TransitPostImage) Container.DataItem) %>'>
       <div>
        <%# Renderer.Render(Eval("Image.Name")) %>
        <%# GetCounter((TransitImage) Eval("Image")) %>
       </div>
       <div>
        <%# GetComments((TransitImage) Eval("Image")) %>
       </div>
      </a>
      <div style='<%# SessionManager.IsAdministrator ? string.Empty : "display: none;" %>'>
       <asp:LinkButton runat="server" ID="togglePreferred" CommandArgument='<%# Eval("Image.Id") %>' 
        CommandName="TogglePreferred" Text='<%# ((bool) Eval("Image.Preferred")) ? "P" : "p" %>' />
       | <a href='EditImage.aspx?id=<%# Eval("Image.Id") %>&r=ShowPost.aspx?id=<%# Eval("Post.Id") %>'>edit</a>
      </div>
     </div>
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </asp:UpdatePanel>
 <asp:UpdatePanel runat="server" ID="panelComments" UpdateMode="Always" RenderMode="Inline">
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
            <%# RenderEx((string) Eval("CommentText"), (int) Eval("PostId")) %>
           </div>
           <div class="table_comments_note">
            <a href='<%# Renderer.Render(Eval("CommentLoginWebsite")) %>'>
             <%# Renderer.Render(Eval("CommentLoginName")) %>
            </a>
            @
            <%# SessionManager.Adjust((DateTime) Eval("CommentCreated")).ToString("f") %>
            <a href='EditPostComment.aspx?sid=<%# Eval("PostId") %>&pid=<%# Eval("CommentId") %>&r=<%# Renderer.UrlEncode(Request.Url.PathAndQuery) %>'>
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
 </asp:UpdatePanel> 
</asp:Content>
