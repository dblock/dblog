<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleAnalyticsControl.ascx.cs" Inherits="GoogleAnalyticsControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<script type="text/javascript">
    function googleAnalyticsScript() {
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', '<% Response.Write(Renderer.Render(SessionManager.GetSetting("GoogleAnalytics.Account", ""))); %>']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    }
</script>