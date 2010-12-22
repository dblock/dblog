<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TwitterFollowMeControl.ascx.cs" Inherits="TwitterFollowMeControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>

<a href='http://www.twitter.com/<% Response.Write(Renderer.Render(SessionManager.GetSetting("Twitter.Account", string.Empty))); %>'><img 
 src="http://twitter-badges.s3.amazonaws.com/t_small-b.png" border="0" alt="Follow me on Twitter"/></a>