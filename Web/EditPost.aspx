<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="EditPost.aspx.cs"
 Inherits="EditPost" Title="Edit Post" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Web" TagName="SelectDate" Src="SelectDateControl.ascx" %>
<%@ Register TagPrefix="Web" TagName="SelectTime" Src="SelectTimeControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Post
 </div>
 <div class="link">
  <a href="ShowBlog.aspx">Back</a>
  | <asp:Hyperlink ID="linkView" runat="server" Text="View" />
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
    <ajaxToolkitHTMLEditor:Editor ID="inputBody" runat="server" Height="400px" Width="600px" InitialCleanUp="true" />
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
    created:
   </td>
   <td class="sncore_form_value">
    <Web:SelectDate ID="inputCreatedDate" runat="server" />
    <Web:SelectTime ID="inputCreatedTime" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:CheckBox ID="inputPublish" runat="server" Text="Publish" CssClass="checkbox" />
    <div class="description">
     uncheck to save as draft
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:CheckBox ID="inputDisplay" runat="server" Text="Display" CssClass="checkbox" />
    <div class="description">
     uncheck not to show post in listings
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:CheckBox ID="inputSticky" runat="server" Text="Sticky" CssClass="checkbox" />
    <div class="description">
     check to keep post on top
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <asp:CheckBox ID="inputExport" runat="server" Text="Export" CssClass="checkbox" />
    <div class="description">
     check to export album
    </div>
   </td>
  </tr>
  <tr>
   <td class="table_tr_td_label">
   </td>
   <td valign="top" class="table_tr_td_value">
    <Controls:WorkingButton runat="server" ID="save" Text="Save" CssClass="button" OnClick="save_Click" />
   </td>
  </tr>
 </table> 
 <asp:UpdatePanel runat="server" ID="panelImages" UpdateMode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="images" CssClass="table" AllowCustomPaging="true"
    RepeatColumns="4" RepeatRows="2" CellPadding="2" OnItemCommand="images_ItemCommand"
    ShowHeader="true" Font-Bold="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />
    <ItemTemplate>
     <img alt="" src='ShowPicture.aspx?id=<%# Eval("Image.Id") %>' />
     <div>
      <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
       runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
     </div>
     <div>
      <a href='EditImage.aspx?id=<%# Eval("Image.Id") %>'>Edit</a>
     </div>
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </asp:UpdatePanel>
 <div class="title">
  Logins
 </div>
 <asp:UpdatePanel runat="server" ID="panelLogins" UpdateMode="Always" RenderMode="Inline">
  <ContentTemplate>
   <table class="table" cellpadding="2">
    <tr>
     <td class="table_tr_td_label">
      add:
     </td>
     <td valign="top" class="table_tr_td_value">
      <asp:TextBox runat="server" CssClass="textbox" ID="inputLogin" />
     </td>
    </tr>
    <tr>
     <td>
     </td>
     <td valign="top" class="table_tr_td_value">
      <Controls:WorkingButton runat="server" ID="loginAdd" Text="Add" 
       CssClass="button" OnClick="loginAdd_Click" />
     </td>
    </tr>
   </table>
   <Controls:PagedGrid runat="server" ID="logins" CssClass="table" AutoGenerateColumns="false"
    AllowPaging="true" AllowCustomPaging="true" PageSize="10" CellPadding="2"
    OnItemCommand="login_ItemCommand" ShowHeader="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" Font-Bold="True" CssClass="table_tr_td" />
    <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next" PrevPageText="Prev"
     HorizontalAlign="Center" />
    <Columns>
     <asp:BoundColumn DataField="Id" Visible="false" />
     <asp:TemplateColumn ItemStyle-Width="25px">
      <itemtemplate>
       <img src="images/site/item.gif" />
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn HeaderText="Username" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <%# Renderer.Render(Eval("Login.Username")) %>
       <%# string.IsNullOrEmpty((string) Eval("Login.Email")) ? string.Empty : "(" + Renderer.Render(Eval("Login.Email")) + ")" %>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkRemove" CommandName="Remove" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Remove" OnClientClick="return confirm('Are you sure you want to do this?');" />
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
