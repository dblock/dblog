<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BlogsViewControl.ascx.cs"
 Inherits="BlogsViewControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<Controls:PagedGrid runat="server" ID="grid" CssClass="table" BorderWidth="0px" AutoGenerateColumns="false"
 AllowPaging="true" AllowCustomPaging="true" PageSize="7" ShowHeader="false">
 <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
 <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
  PrevPageText="Prev" HorizontalAlign="Center" />
 <Columns>
  <asp:BoundColumn DataField="Id" Visible="false" />
  <asp:TemplateColumn>
   <itemtemplate>
    <table class="table_blog">
     <tr>
      <td valign="top" colspan="2" class="table_blog_tr_th">
       <%# this.Render((int) Eval("Id"), Eval("Type").ToString(), (string) Eval("Title")) %>
      </td>
     </tr>
     <tr>
      <td class="table_blog_tr_td">
       <asp:Panel id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
        <img src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
       </asp:Panel>
      </td>
      <td valign="top" class="table_blog_tr_td">
       <%# this.Render((int) Eval("Id"), Eval("Type").ToString(), (string) Eval("Text")) %>
      </td>
     </tr>
    </table>
   </itemtemplate>
  </asp:TemplateColumn>
 </Columns>
</Controls:PagedGrid>
