<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UploadControl.ascx.cs" Inherits="UploadControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>

<asp:FileUpload runat="server" ID="inputUpload" CssClass="upload" />
<div class="link">
 <asp:Label ID="labelData" runat="server" />
 <asp:LinkButton ID="upload" runat="Server" OnClick="upload_Click" Text="&#187; Update / Clear" />
</div>

