<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowImage.aspx.cs"
 Inherits="ShowImage" Title="Image" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <atlas:UpdatePanel ID="panelPicture" runat="server" Mode="Conditional">
  <ContentTemplate>
   <div class="title">
    <asp:Label ID="labelName" runat="server" />
   </div>
   <div class="subtitle">
    <asp:HyperLink ID="linkBack" runat="server" text="Back" />
    | <asp:HyperLink ID="linkComment" runat="server" text="New Comment" />
    | <asp:Label ID="labelCount" runat="server" />    
   </div>
  </ContentTemplate>
 </atlas:UpdatePanel>
 <atlas:UpdatePanel runat="server" ID="panelImages" Mode="Conditional" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="images" CssClass="table" AllowCustomPaging="true"
    RepeatColumns="1" RepeatRows="1" CellPadding="2" ShowHeader="true" Font-Bold="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />
    <ItemTemplate>
     <a href='ShowPicture.aspx?id=<%# Eval("Image.Id") %>&ShowThumbnail=false'>
      <img src='ShowPicture.aspx?id=<%# Eval("Image.Id") %>&ShowThumbnail=false' border="0" />
     </a>
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </atlas:UpdatePanel>
 <atlas:UpdatePanel runat="server" ID="panelComments" Mode="Conditional" RenderMode="Inline">
  <ContentTemplate>   
   <Controls:PagedGrid runat="server" ID="comments" BorderWidth="0" CssClass="table" 
    AllowPaging="false" AllowCustomPaging="false" CellPadding="2" ShowHeader="false" 
    AutoGenerateColumns="false">
    <ItemStyle HorizontalAlign="Left" CssClass="table_tr_td" />
    <PagerStyle cssclass="table_pager" position="Bottom" nextpagetext="Next" prevpagetext="Prev"
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
            <%# SessionManager.Region.UtcToUser((DateTime) Eval("CommentCreated")).ToString("f") %>
            <a href='EditImageComment.aspx?sid=<%# Eval("ImageId") %>&pid=<%# Eval("CommentId") %>&rid=<%# GetId("pid") %>'>
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
