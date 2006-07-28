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
                Response.Redirect(string.Format("Login.aspx?r={0}", 
                    Renderer.UrlEncode(Request.Url.PathAndQuery)));
            }
        }

        base.OnLoad(e);
    }

    public int CheckInput(string name, int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException(string.Format("Missing {0}", name));
        }

        return value;
    }

    public string CheckInput(string name, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(string.Format("Missing {0}", name));
        }

        return value;
    }
}
