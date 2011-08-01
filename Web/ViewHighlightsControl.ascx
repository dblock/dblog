<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewHighlightsControl.ascx.cs"
    Inherits="ViewHighlightsControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<ul>
 <Controls:PagedList runat="server" ID="grid" RepeatRows="0">
  <ItemTemplate><li><a href='<%# Renderer.Render(Eval("Url")) %>'><%# Renderer.Render(Eval("Title")) %></a>
    <img src='ShowPicture.aspx?id=<%# Renderer.Render(Eval("ImageId")) %>&ShowThumbnail=false&IncrementCounter=false' width="16" height="16">
   </li></ItemTemplate>
 </Controls:PagedList>
</ul>
