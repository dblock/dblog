using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.TransitData;
using DBlog.Tools.Web;

public partial class BlogLogin : BlogPage
{
    public enum AccessType
    {
        Default,
        User, // user page, login as any user
        Admin, // admin page, must be admin to login
        Denied // access denied for this user, need to login with an existing one
    };

    private AccessType AccessDeniedType
    {
        get
        {
            string access = (string)Request["access"];
            return (string.IsNullOrEmpty(access) 
                ? AccessType.Default 
                : (AccessType) Enum.Parse(typeof(AccessType), access, true));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                linkNewUser.NavigateUrl = string.Format("EditLogin.aspx?r={0}", Renderer.UrlEncode(ReturnUrl));

                switch (AccessDeniedType)
                {
                    case AccessType.Admin:
                        linkNewUser.Enabled = false;
                        ReportInfo("You must be an administrator to open this page, please log-in with an administrative account.");
                        break;
                    case AccessType.User:
                        linkNewUser.Enabled = true;
                        ReportInfo("You must be logged-in to open this page. Please register first.");
                        break;
                    case AccessType.Denied:
                        linkNewUser.Enabled = false;
                        ReportException(new Exception("Access Denied. Please log-in with a valid username and password."));
                        break;
                }
            }

        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public string ReturnUrl
    {
        get
        {
            string result = Request.QueryString["r"];
            if (string.IsNullOrEmpty(result)) return "ShowBlog.aspx";
            return result;
        }
    }

    public string CookieName
    {
        get
        {
            string result = Request.QueryString["cookie"];
            if (string.IsNullOrEmpty(result)) return SessionManager.sDBlogAuthCookieName;
            return result;
        }
    }

    public void button_Click(object sender, EventArgs e)
    {
        try
        {
            string ticket = SessionManager.BlogService.Login(inputUsername.Text, inputPassword.Text);
            SessionManager.Login(ticket, inputRememberMe.Checked, CookieName);
            Response.Redirect(ReturnUrl);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
