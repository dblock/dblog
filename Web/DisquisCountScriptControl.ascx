<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisquisCountScriptControl.ascx.cs"
    Inherits="DisquisCountScriptControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<script type="text/javascript">
    var disqus_shortname = '<% Response.Write(Renderer.Render(SessionManager.GetSetting("Disquis.Shortname", ""))); %>';
    <% if (! String.IsNullOrEmpty(DisquisDeveloper)) Response.Write(String.Format("var disqus_developer='{0}';\n", DisquisDeveloper)); %>

    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function () {
        var s = document.createElement('script'); s.async = true;
        s.type = 'text/javascript';
        s.src = 'http://' + disqus_shortname + '.disqus.com/count.js';
        (document.getElementsByTagName('HEAD')[0] || document.getElementsByTagName('BODY')[0]).appendChild(s);
    }());
</script>
