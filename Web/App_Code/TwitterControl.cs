using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class TwitterControl : BlogControl
{
    public bool TwitterEnabled
    {
        get
        {
            return ! string.IsNullOrEmpty(SessionManager.GetSetting(
                "Twitter.Account", string.Empty));
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Visible = TwitterEnabled;
        }
    }
}