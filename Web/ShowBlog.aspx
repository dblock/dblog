<%@ Page Language="C#" MasterPageFile="~/DBlog.master" AutoEventWireup="true" CodeFile="ShowBlog.aspx.cs"
 Inherits="ShowBlog" Title="Blog" %>

<%@ Register TagPrefix="Controls" TagName="Topics" Src="ViewTopicsControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="Search" Src="SearchControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="DateRange" Src="DateRangeControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="TwitterScript" Src="TwitterScriptControl.ascx" %>
<%@ Register TagPrefix="Controls" TagName="TwitterShare" Src="TwitterShareControl.ascx" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Controls" TagName="DisqusCountScript" Src="DisqusCountScriptControl.ascx" %>

<%@ Import Namespace="DBlog.TransitData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <Controls:TwitterScript id="twitterScript" runat="server" />
  <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="panelPosts" RenderMode="Inline">
   <ContentTemplate>
    <asp:Label ID="labelPosts" CssClass="title" runat="server" Visible="false" />
    <div>
        <asp:Label ID="labelCriteria" CssClass="subtitle" runat="server" Visible="false" />
    </div>
    <Controls:PagedGrid runat="server" ID="grid" CssClass="table" 
     AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" 
     PageSize="7" ShowHeader="false" OnItemCommand="grid_ItemCommand" 
     BorderWidth="0" BorderColor="White">
     <ItemStyle CssClass="table_tr_td" />
     <PagerStyle CssClass="table_pager" Position="TopAndBottom" NextPageText="Next"
      PrevPageText="Prev" HorizontalAlign="Center" />
     <Columns>
      <asp:BoundColumn DataField="Id" Visible="false" />
      <asp:TemplateColumn>
       <itemtemplate>
        <div class="post_title">
         <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
          <%# RenderEx((string) Eval("Title"), (int) Eval("Id")) %>
         </a>
         <span style="<%# (bool) Eval("Publish") ? "display: none;" : "" %>">
          <a href='EditPost.aspx?id=<%# Eval("Id") %>'>
           <img src="images/site/draft.gif" alt="Draft" border="0" />
          </a>
         </span>
         <span style="<%# (bool) Eval("Display") ? "display: none;" : "" %>">
          <img src="images/site/hidden.gif" alt="Hidden" border="0" />
         </span>
         <span style="<%# (bool) Eval("Sticky") ? "" : "display: none;" %>">
          <img src="images/site/sticky.gif" alt="Sticky" border="0" />
         </span>
        </div>
        <div class="post_subtitle">
         <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
          Read
         </a>
         | <%# GetTopics((TransitTopic[]) Eval("Topics")) %>
         | <%# SessionManager.Adjust((DateTime) Eval("Created")).ToString("dddd, dd MMMM yyyy") %>
         <%# GetImagesShortLink((int)Eval("ImagesCount"))%>
         | <a href="ShowPost.aspx?id=<%# Eval("Id") %>#disqus_thread" data-disqus-identifier="Post_<%# Eval("Id") %>">Post Comment</a>
         | <%# GetCounter((long) Eval("Counter.Count")) %>
         <span id="SpanEditPost" runat="server" style='<%# (bool) SessionManager.IsAdministrator ? String.Empty : "display: none;" %>'>
          | <a href='EditPost.aspx?id=<%# Eval("Id") %>'>
           Edit
          </a>
          | <asp:LinkButton ID="linkDelete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
           runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to do this?');" />
          | <asp:LinkButton ID="linkToggleSticky" CommandName="Sticky" CommandArgument='<%# Eval("Id") %>'
           runat="server" Text='<%# (bool) Eval("Sticky") ? "Unstick" : "Stick" %>' />
          | <asp:LinkButton ID="linkToggleDisplay" CommandName="Display" CommandArgument='<%# Eval("Id") %>'
           runat="server" Text='<%# (bool) Eval("Display") ? "Hide" : "Show" %>' />
         </span>
         <Controls:TwitterShare id="twitterShare" runat="server" 
           Url='<%# String.Format("{0}ShowPost.aspx?id={1}", SessionManager.WebsiteUrl, Eval("Id")) %>'
           Text='<%# Eval("Title") %>'
           />
        </div>
        <div class="post_body">
         <%# RenderEx((string) Eval("Body"), (int) Eval("Id")) %>
        </div>
        <asp:Panel CssClass="post_image" Width="100%" id="panelPicture" runat="server" visible='<%# (int) Eval("ImageId") > 0 %>'>
         <a href='<%# GetPostLink((int) Eval("ImagesCount"), (int) Eval("Id"), (int) Eval("ImageId")) %>'>
          <img border="0" src='ShowPicture.aspx?Id=<%# Eval("ImageId") %>' />
         </a>
         <div class="link">
          <a href='ShowPost.aspx?id=<%# Eval("Id") %>'>
           <%# GetImagesLink((int) Eval("ImagesCount")) %>
          </a>
         </div>
        </asp:Panel>
       </itemtemplate>
      </asp:TemplateColumn>
     </Columns>
    </Controls:PagedGrid>
   </ContentTemplate>
  </asp:UpdatePanel>
  <Controls:DisqusCountScript id="disqusCommentsCount" runat="server" />
</asp:Content>
