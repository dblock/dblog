<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowBlog.aspx.cs"
 Inherits="ShowBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" TagName="Topics" Src="ViewTopicsControl.ascx" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <atlas:UpdatePanel Mode="Conditional" runat="server" ID="panelPosts" RenderMode="Inline">
   <ContentTemplate>
    <Controls:PagedGrid runat="server" ID="grid" CssClass="table" BorderWidth="0px" AutoGenerateColumns="false"
     AllowPaging="true" AllowCustomPaging="true" PageSize="7" ShowHeader="false">
     <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
     <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
      PrevPageText="Prev" HorizontalAlign="Center" />
     <Columns>
      <asp:BoundColumn DataField="Id" Visible="false" />
      <asp:TemplateColumn>
       <itemtemplate>
        <table class="table_post">
         <tr>
          <td valign="top" colspan="2" class="table_post_tr_th">
           <%# this.Render((int) Eval("Id"), "Post", (string) Eval("Title")) %>
          </td>
         </tr>
         <tr>
          <td class="table_post_tr_td">
           <asp:Panel id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
            <img src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
           </asp:Panel>
          </td>
          <td valign="top" class="table_post_tr_td">
           <%# this.Render((int)Eval("Id"), "Post", (string)Eval("Body"))%>
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
