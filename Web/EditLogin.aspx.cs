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

public partial class EditLogin : BlogPage
{
    public string ReturnUrl
    {
        get
        {
            string result = Request.QueryString["r"];
            if (string.IsNullOrEmpty(result)) return "ManageLogins.aspx";
            return result;
        }
    }

    private TransitLogin mLogin = null;

    public TransitLogin Login
    {
        get
        {
            if (mLogin == null)
            {
                mLogin = (RequestId > 0)
                    ? SessionManager.GetCachedObject<TransitLogin>("GetLoginById", SessionManager.Ticket, RequestId)
                    : new TransitLogin();
            }

            return mLogin;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetDefaultButton(save);

                linkCancel.NavigateUrl = ReturnUrl;

                panelAdmin.Visible = SessionManager.IsAdministrator;

                if (RequestId > 0)
                {
                    inputName.Text = Login.Name;
                    inputUsername.Text = Login.Username;
                    inputEmail.Text = Login.Email;
                    inputPassword.Attributes["value"] = Login.Password;
                    inputPassword2.Attributes["value"] = Login.Password;
                    inputAdministrator.Checked = (Login.Role == TransitLoginRole.Administrator);
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            inputPassword.Attributes["value"] = inputPassword.Text;
            inputPassword2.Attributes["value"] = inputPassword2.Text;

            Login.Name = (string.IsNullOrEmpty(inputUsername.Text)) 
                ? CheckInput("Name", inputName.Text) 
                : inputName.Text;

            Login.Username = inputUsername.Text;
            Login.Email = SessionManager.IsAdministrator ? inputEmail.Text : CheckInput("Email", inputEmail.Text);
            if (string.IsNullOrEmpty(Login.Username)) Login.Username = Login.Email;
            Login.Password = SessionManager.IsAdministrator ? inputPassword.Text : CheckInput("Password", inputPassword.Text);

            if (inputPassword.Text != inputPassword2.Text)
            {
                throw new Exception("Passwords Don't Match");
            }

            Login.Role = inputAdministrator.Checked ? TransitLoginRole.Administrator : TransitLoginRole.Guest;
            SessionManager.BlogService.CreateOrUpdateLogin(SessionManager.Ticket, Login);
            SessionManager.Invalidate<TransitLogin>();

            if (!SessionManager.IsLoggedIn)
            {
                SessionManager.Login(
                    SessionManager.BlogService.Login(Login.Email, Login.Password),
                    true);
            }

            Response.Redirect(ReturnUrl);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected override bool Index
    {
        get
        {
            return false;
        }
    }
}
