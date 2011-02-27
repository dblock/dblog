using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DisquisControl : BlogControl
{
    public bool DisquisEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(SessionManager.GetSetting(
                "Disquis.Shortname", string.Empty));
        }
    }

    public string DisquisDeveloper
    {
        get
        {
            return SessionManager.GetSetting("Disquis.Developer", string.Empty);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Visible = DisquisEnabled;
        }
    }
}