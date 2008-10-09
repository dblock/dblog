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

public class BlogUserPage : BlogPage
{
    public BlogUserPage()
    {

    }

    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!SessionManager.IsLoggedIn)
            {
                Response.Redirect(string.Format("Login.aspx?r={0}&access=user",
                    Renderer.UrlEncode(Request.Url.PathAndQuery)));
            }
        }

        base.OnLoad(e);
    }
}
