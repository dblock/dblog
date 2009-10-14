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
using System.Text;
using DBlog.Tools.Web;
using DBlog.Data.Hibernate;
using DBlog.TransitData.References;

public partial class ResetPassword : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (RequestId <= 0)
                {
                    throw new Exception(string.Format("Invalid login id '{0}'.",
                        Request["id"]));
                }

                if (string.IsNullOrEmpty(Request["hash"]))
                {
                    throw new Exception(string.Format("Missing hash."));
                }

                string email = Request["username"];
                inputEmail.Text = email;
                linkCancel.NavigateUrl = string.Format("Login.aspx?username={0}", Renderer.UrlEncode(email));
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void reset_Click(object sender, EventArgs e)
    {
        try
        {
            if (inputPassword.Text != inputPassword2.Text)
            {
                throw new Exception("Passwords don't match.");
            }

            if (inputPassword.Text.Length < 4)
            {
                throw new Exception("Password must be at least 4 characters long.");
            }

            SessionManager.BlogService.ResetLoginPassword(RequestId, Request["hash"], inputPassword.Text);
            ReportInfo(string.Format("Your password has been reset. You may now <a href='Login.aspx?username={0}'>login</a>.",
                Renderer.UrlEncode(Request["username"])));
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
