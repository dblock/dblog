<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UploadControl.ascx.cs" Inherits="UploadControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>

<asp:FileUpload runat="server" ID="inputUpload" CssClass="upload" />
<!--
<Controls:WorkingButton runat="server" ID="upload" Text="Upload" CssClass="button"
 OnClick="upload_Click" />
 -->
<div class="description">
 <asp:Label ID="labelData" runat="server" />
</div>

