<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ManageEntries.aspx.cs"
 Inherits="admin_ManageEntries" Title="Manage Entries" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Entries
 </div>
 <div class="link">
  <a href="EditEntry.aspx">&#187; Post New</a>
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="grid" CssClass="table" AutoGenerateColumns="false"
    AllowPaging="true" AllowCustomPaging="true" PageSize="10" CellPadding="2"
    OnItemCommand="grid_ItemCommand" ShowHeader="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" Font-Bold="True" CssClass="table_tr_td" />
    <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next" PrevPageText="Prev"
     HorizontalAlign="Center" />
    <Columns>
     <asp:BoundColumn DataField="Id" Visible="false" />
     <asp:TemplateColumn HeaderText="Entry" ItemStyle-Font-Bold="False">
      <itemtemplate>
       <table class="table_blog">
        <tr>
         <td valign="top" colspan="2" class="table_blog_tr_th">
          <%# this.Render((int) Eval("Id"), (string) Eval("Title")) %>
         </td>
        </tr>
        <tr>
         <td class="table_blog_tr_td">
          <asp:Panel id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
           <img src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
          </asp:Panel>
         </td>
         <td valign="top" class="table_blog_tr_td">
          <%# this.Render((int) Eval("Id"), (string) Eval("Text")) %>
         </td>
        </tr>
       </table>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <a href="EditEntry.aspx?id=<%# Eval("Id") %>">Edit</a>
      </itemtemplate>
     </asp:TemplateColumn>
     <asp:TemplateColumn ItemStyle-Width="100px">
      <itemtemplate>
       <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
        runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
      </itemtemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </atlas:UpdatePanel>
</asp:Content>
