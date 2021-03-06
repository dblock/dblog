using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.Tools.Web;

public class BlogAdminPage : BlogPage
{
    public BlogAdminPage()
    {

    }

    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!SessionManager.IsAdministrator)
            {
                Response.Redirect(string.Format("./Login.aspx?r={0}&access=admin",
                    Renderer.UrlEncode(UrlPathAndQuery)));
            }
        }

        base.OnLoad(e);
    }

    protected override bool Index
    {
        get
        {
            return false;
        }
    }
}
