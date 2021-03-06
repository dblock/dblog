<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ListBlog.aspx.cs"
 Inherits="ListBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<%@ OutputCache Duration="600" VaryByParam="*" VaryByCustom="Ticket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <ul>
  <li><a href="SiteMapBlog.aspx">Google Site Map</a></li>
 </ul>
 <ul>
  <asp:Repeater ID="grid" runat="server">
   <ItemTemplate>
    <li>
     <a href='<%# Eval("LinkUri") %>'>
      <b><%# RenderEx((string) Eval("Title"), (int) Eval("Id")) %></b>
     </a>
     &#187; <%# SessionManager.Adjust((DateTime) Eval("Created")).ToString("dddd, dd MMMM yyyy") %>
     <span style="<%# (bool) Eval("Publish") ? "display: none;" : "" %>">
      <a href='EditPost.aspx?id=<%# Eval("Id") %>'>
       &#187; edit draft
      </a>
     </span>
     <span style="<%# (bool) Eval("Display") ? "display: none;" : "" %>">
      <a href='EditPost.aspx?id=<%# Eval("Id") %>'>
       &#187 hidden
      </a>
     </span>
    </li>
   </ItemTemplate>
  </asp:Repeater>
 </ul>
 <asp:HyperLink ID="linkPrev" runat="server" Text="&#171; Prev" NavigateUrl="." />
 | <asp:HyperLink ID="linkNext" runat="server" Text="Next &#187;" NavigateUrl="." />
</asp:Content>
