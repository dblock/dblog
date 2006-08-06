<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowImages.aspx.cs"
 Inherits="ShowImages" Title="Popular Images" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="TransitData" Namespace="DBlog.TransitData" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <div class="title">
  Popular Images
 </div>
 <atlas:UpdatePanel runat="server" ID="panelGrid" Mode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedList runat="server" ID="images" CssClass="table" AllowCustomPaging="true"
    RepeatColumns="4" RepeatRows="3" CellPadding="2" ShowHeader="true" Font-Bold="true">
    <HeaderStyle CssClass="table_tr_th" HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" CssClass="table_tr_td" />
    <pagerstyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />
    <ItemTemplate>
     <a href='ShowImage.aspx?id=<%# Eval("Image.Id") %>'>
      <img border="0" alt='<%# Renderer.Render(Eval("Image.Description")) %>' src='ShowPicture.aspx?id=<%# Eval("Image.Id") %>' />
     </a>
     <div class="link_small">
      <a href='ShowImage.aspx?id=<%# Eval("Image.Id") %>'>
       <div>
        <%# Renderer.Render(Eval("Image.Name")) %>
        <%# GetCounter((TransitImage) Eval("Image")) %>
       </div>
       <div>
        <%# GetComments((TransitImage) Eval("Image")) %>
       </div>
      </a>
     </div>
    </ItemTemplate>
   </Controls:PagedList>
  </ContentTemplate>
 </atlas:UpdatePanel>
</asp:Content>
