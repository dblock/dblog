<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowPost.aspx.cs"
 Inherits="ShowPost" Title="Post" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="TransitData" Namespace="DBlog.TransitData" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="TwitterScript" Src="TwitterScriptControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="TwitterShare" Src="TwitterShareControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="DisqusScript" Src="DisqusScriptControl.ascx" %>

<%@ Import Namespace="DBlog.TransitData" %>

<%@ OutputCache Duration="600" VaryByParam="*" VaryByCustom="Ticket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <Controls:TwitterScript id="twitterScript" runat="server" />
  <h3><asp:Label ID="posttitle" runat="server" /></h3>
  <div class="subtitle">
   <asp:UpdatePanel runat="server" ID="panelButtons" UpdateMode="Always" RenderMode="Inline">
    <ContentTemplate>
     <a href=".">Back</a> |
     <asp:Label ID="posttopics" runat="server" CssClass="topics" /> |
     <asp:Label ID="postcreated" runat="server" />
     <!--<asp:Label ID="postcounter" runat="server" />-->
     <!--<asp:Hyperlink ID="linkComment" runat="server" Text="Post Comment" /> |-->
     <span id="spanAdmin" runat="server">
      | <asp:Hyperlink ID="linkEdit" runat="server" Text="Edit" />
     </span>
     <!--<asp:LinkButton ID="linkPreferred" OnClick="linkPreferred_Click" runat="server" Enabled="true" Text="Favorites" />-->
     <Controls:TwitterShare id="twitterShare" runat="server" />
    </ContentTemplate>
   </asp:UpdatePanel>
  </div>
  <div class="post">
   <asp:Label ID="postbody" runat="server" />
    <asp:Panel ID="panelPicture" runat="server">
     <a href='' runat="server" id="linkimage">
      <asp:Image BorderWidth="0" runat="server" ID="postimage" />
     </a>
    </asp:Panel>
  </div>
  <asp:UpdatePanel runat="server" ID="panelImages" UpdateMode="Always" RenderMode="Inline">
   <ContentTemplate>
    <div class="images">
     <Controls:PagedList RepeatColumns="9" RepeatRows="1" runat="server" ID="images" RepeatDirection="Horizontal" 
      OnItemCommand="images_OnItemCommand" AllowCustomPaging="true">
      <pagerstyle cssclass="images_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev" />
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
          <!--
          <div>
           <%# GetComments((TransitImage) Eval("Image")) %>
          </div>
          -->
         </a>
         <div style='<%# SessionManager.IsAdministrator ? string.Empty : "display: none;" %>'>
          <asp:LinkButton runat="server" ID="togglePreferred" CommandArgument='<%# Eval("Image.Id") %>' 
           CommandName="TogglePreferred" Text='<%# ((bool) Eval("Image.Preferred")) ? "P" : "p" %>' />
          | <a href='EditImage.aspx?id=<%# Eval("Image.Id") %>&r=ShowPost.aspx?id=<%# Eval("Post.Id") %>'>edit</a>
         </div>
        </div>
      </ItemTemplate>
     </Controls:PagedList>
    </div>
   </ContentTemplate>
  </asp:UpdatePanel>
  <Controls:DisqusScript id="disqusComments" runat="server" />
  <asp:UpdatePanel runat="server" ID="panelComments" UpdateMode="Always" RenderMode="Inline">
   <ContentTemplate>
    <Controls:PagedGrid runat="server" ID="comments" BorderWidth="0" CssClass="table" 
     AllowPaging="false" AllowCustomPaging="false" CellPadding="2" ShowHeader="false" 
     AutoGenerateColumns="false" OnItemCommand="comments_ItemCommand">
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
             <%# SessionManager.Adjust((DateTime) Eval("CommentCreated")).ToString("dddd, dd MMMM yyyy") %>
             <a href='EditPostComment.aspx?sid=<%# Eval("PostId") %>&pid=<%# Eval("CommentId") %>&r=<%# Renderer.UrlEncode(UrlPathAndQuery) %>'>
              Reply
             </a>
             <asp:LinkButton id="commentDelete" runat="server" Text="&#187; Delete" Visible='<%# SessionManager.IsAdministrator %>' 
              CommandName="Delete" CommandArgument='<%# Eval("CommentId") %>' OnClientClick="return confirm('Are you sure you want to delete this comment?');" />
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
