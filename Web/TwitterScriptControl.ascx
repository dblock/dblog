<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TwitterScriptControl.ascx.cs"
    Inherits="TwitterScriptControl" %>

<script type="text/javascript">
    function pageLoad() {
        $.getScript("http://platform.twitter.com/widgets.js");
        disqusCountScript();
    }
</script>

