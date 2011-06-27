<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowImage.aspx.cs"
 Inherits="ShowImage" Title="Image" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<%@ OutputCache Duration="3600" VaryByParam="*"%>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <asp:UpdatePanel ID="panelPicture" runat="server" UpdateMode="Conditional">
  <ContentTemplate>
   <div class="title">
    <asp:Label ID="labelName" runat="server" />
   </div>
   <div class="subtitle">
    <asp:HyperLink ID="linkBack" runat="server" text="&#171; Back" />
    <!--| <asp:HyperLink ID="linkComment" runat="server" text="Post Comment" />-->
    | <asp:Label ID="labelCount" runat="server" />
    | <asp:LinkButton ID="linkEXIF" runat="server" OnClick="linkEXIF_Click" Text="EXIF" />
   </div>
  </ContentTemplate>
 </asp:UpdatePanel>
 <asp:UpdatePanel runat="server" ID="panelEXIF" UpdateMode="Conditional">
  <ContentTemplate>
   <Controls:PagedList Visible="false" runat="server" ID="exif" CssClass="table" AutoGenerateColumns="false"
    AllowCustomPaging="false"  RepeatRows="100" CellPadding="2" ShowHeader="true" RepeatColumns="2">
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <ItemTemplate>
      <%# Renderer.Render(Eval("Code")) %>:
      <b><%# Renderer.Render(Eval("Data")) %></b>   
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </asp:UpdatePanel>
 <asp:UpdatePanel runat="server" ID="panelImages" UpdateMode="Conditional" RenderMode="Inline">
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
 </asp:UpdatePanel>
 <asp:UpdatePanel runat="server" ID="panelComments" UpdateMode="Conditional" RenderMode="Inline">
  <ContentTemplate>   
   <Controls:PagedGrid runat="server" ID="comments" BorderWidth="0" CssClass="table" 
    AllowPaging="false" AllowCustomPaging="false" CellPadding="2" ShowHeader="false" 
    AutoGenerateColumns="false" OnItemCommand="comments_ItemCommand">
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
            <%# RenderEx((string) Eval("CommentText"), (int) Eval("ImageId")) %>
           </div>
           <div class="table_comments_note">
            <a href='<%# Renderer.Render(Eval("CommentLoginWebsite")) %>'>
             <%# Renderer.Render(Eval("CommentLoginName")) %>
            </a>
            @
            <%# SessionManager.Adjust((DateTime) Eval("CommentCreated")).ToString("dddd, dd MMMM yyyy") %>
            <a href='EditImageComment.aspx?sid=<%# Eval("ImageId") %>&pid=<%# Eval("CommentId") %>&r=<%# Renderer.UrlEncode(Request.Url.PathAndQuery) %>'>
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
