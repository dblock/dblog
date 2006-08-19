<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchControl.ascx.cs"
 Inherits="SearchControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<div>
 Search
</div>
<asp:TextBox ID="inputSearch" runat="server" AutoPostBack="true" OnTextChanged="inputSearch_Changed"
 CssClass="search_textbox" />
