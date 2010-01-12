<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowComments.aspx.cs"
 Inherits="ShowComments" Title="Blog Comments" %>

<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="TransitData" Namespace="DBlog.TransitData" Assembly="DBlog.Tools" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <asp:UpdatePanel runat="server" ID="panelComments" UpdateMode="Always" RenderMode="Inline">
  <ContentTemplate>
   <Controls:PagedGrid runat="server" ID="comments" BorderWidth="0" CssClass="table" 
    AllowPaging="true" AllowCustomPaging="true" CellPadding="2" ShowHeader="false" 
    AutoGenerateColumns="false" PageSize="10">
    <ItemStyle HorizontalAlign="Left" CssClass="table_tr_td" />
    <PagerStyle cssclass="table_pager" position="TopAndBottom" nextpagetext="Next" prevpagetext="Prev"
     horizontalalign="Center" />    
    <Columns>
     <asp:TemplateColumn>
      <ItemTemplate>
       <div class="table_comments">
        <div class="table_comments_body">
         <%# RenderEx((string) Eval("CommentText"), (int) Eval("AssociatedId")) %>
        </div>
        <div class="table_comments_note">
         <%# SessionManager.Adjust((DateTime) Eval("CommentCreated")).ToString("f") %>
         <a href='Show<%# Eval("AssociatedType") %>.aspx?id=<%# Eval("AssociatedId") %>'>
          View
         </a>
        </div>
       </div>
      </ItemTemplate>
     </asp:TemplateColumn>
    </Columns>
   </Controls:PagedGrid>
  </ContentTemplate>
 </asp:UpdatePanel> 
</asp:Content>
