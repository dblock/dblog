<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditPost.aspx.cs"
 Inherits="EditPost" Title="Edit Post" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Post
 </div>
 <div class="link">
  <a href="ShowBlog.aspx">Cancel</a>
  <asp:Hyperlink ID="linkView" runat="server" Text="| View" />
 </div>
 <table class="table" cellpadding="2">
  <tr>
   <td class="table_tr_td_label">
    title:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputTitle" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    text:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" Rows="10" TextMode="MultiLine" ID="inputBody" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    topic:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:DropDownList DataTextField="Name" DataValueField="Id" runat="server" CssClass="dropdown"
     ID="inputTopic" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
    image server path:
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:TextBox runat="server" CssClass="textbox" ID="inputServerPath" />
    <div>
     <asp:Label ID="labelServerPath" runat="server" CssClass="description" />
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label" valign="top">
    image(s):
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:MultiFileUpload InputCssClass="upload" runat="server" ID="inputImages"
     OnFilesPosted="inputImages_FilesPosted" />
    <asp:HyperLink ID="addFile" runat="server" CssClass="sncore_form_label" NavigateUrl="#" Text="+" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:WorkingButton runat="server" ID="save" Text="Save" CssClass="button" OnClick="save_Click" />
   </td>
  </tr>
  <tr>
   <td colspan="2">
    <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always" RenderMode="Inline">
     <ContentTemplate>
      <Controls:PagedList runat="server" ID="grid" CssClass="table" AllowCustomPaging="true"
       RepeatColumns="4" RepeatRows="2" CellPadding="2" OnItemCommand="grid_ItemCommand"
       ShowHeader="true" Font-Bold="true">
       <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
       <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
       <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
        horizontalalign="Center" />
       <ItemTemplate>
        <img alt="" src='ShowPicture.aspx?id=<%# Eval("ImageId") %>' />
        <div>
         <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
          runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
        </div>
        <div>
         <a href='EditImage.aspx?id=<%# Eval("ImageId") %>'>Edit</a>
        </div>
       </ItemTemplate>
      </Controls:PagedList>
     </ContentTemplate>
    </atlas:UpdatePanel>
   </td>
  </tr>
 </table>
</asp:Content>
