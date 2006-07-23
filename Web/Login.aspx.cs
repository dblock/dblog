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

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

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

    public void button_Click(object sender, EventArgs e)
    {
        try
        {
            string ticket = SessionManager.BlogService.Login(inputUsername.Text, inputPassword.Text);
            SessionManager.Login(ticket, inputRememberMe.Checked);
            Response.Redirect(ReturnUrl);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}