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

public partial class ResetPasswordEmail : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
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
            string email = SessionManager.BlogService.ResetLoginPasswordEmail(inputEmail.Text);
            ReportInfo(string.Format("An e-mail has been sent to '{0}'.", email));
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
