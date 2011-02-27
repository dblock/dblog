<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisquisScriptControl.ascx.cs"
    Inherits="DisquisScriptControl" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<div id="disqus_thread"></div>
<script type="text/javascript">
    var disqus_shortname = '<% Response.Write(Renderer.Render(SessionManager.GetSetting("Disquis.Shortname", ""))); %>';

    <% if (! String.IsNullOrEmpty(DisquisId)) Response.Write(String.Format("var disqus_identifier='{0}';\n", DisquisId)); %>
    <% if (! String.IsNullOrEmpty(DisquisUrl)) Response.Write(String.Format("var disqus_url='{0}';\n", DisquisUrl)); %>
    <% if (! String.IsNullOrEmpty(DisquisDeveloper)) Response.Write(String.Format("var disqus_developer='{0}';\n", DisquisDeveloper)); %>

    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function () {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();
</script>
<noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>