using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DisqusControl : BlogControl
{
    public bool DisqusEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(SessionManager.GetSetting(
                "Disqus.Shortname", string.Empty));
        }
    }

    public string DisqusDeveloper
    {
        get
        {
            return SessionManager.GetSetting("Disqus.Developer", string.Empty);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Visible = DisqusEnabled;
        }
    }
}