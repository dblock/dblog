<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SiteMapBlog.aspx.cs"
 Inherits="SiteMapBlog" Title="Google SiteMap" %>
<%@ OutputCache Duration="600" VaryByParam="*" VaryByCustom="Ticket" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
 <asp:Repeater id="repeater" runat="server">
  <ItemTemplate>
   <url>
    <loc><%# SessionManager.WebsiteUrl %><%# Eval("LinkUri")%></loc>
    <lastmod><%# ((DateTime) Eval("Modified")).ToString("s") %>+00:00</lastmod>
    <changefreq>never</changefreq>
   </url>
  </ItemTemplate>
 </asp:Repeater>    
</urlset>
