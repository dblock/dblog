<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewHighlightsControl.ascx.cs"
 Inherits="ViewHighlightsControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<Controls:PagedGrid runat="server" ID="grid" CssClass="table_highlights" AutoGenerateColumns="false"
 AllowPaging="false">
 <ItemStyle HorizontalAlign="Center" CssClass="table_highlights_tr_td" />
 <AlternatingItemStyle HorizontalAlign="Center" CssClass="table_highlights_tr_td_alt" />
 <HeaderStyle CssClass="table_highlights_tr_th" />
 <Columns>
  <asp:BoundColumn DataField="Id" Visible="false" />
  <asp:TemplateColumn>
   <itemtemplate>
    <img src='ShowPicture.aspx?id=<%# Renderer.Render(Eval("ImageId")) %>&ShowThumbnail=false&IncrementCounter=false' width="32" height="32">
   </itemtemplate>
  </asp:TemplateColumn>
  <asp:TemplateColumn HeaderText="Highlights">
   <itemtemplate>
    <a href='<%# Renderer.Render(Eval("Url")) %>'><%# Renderer.Render(Eval("Title")) %></a>
   </itemtemplate>
  </asp:TemplateColumn>
 </Columns>
</Controls:PagedGrid>
