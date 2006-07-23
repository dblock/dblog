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

public partial class EditLogin : AdminPage
{
    private TransitLogin mLogin = null;

    public TransitLogin Login
    {
        get
        {
            if (mLogin == null)
            {
                mLogin = (RequestId > 0)
                    ? SessionManager.BlogService.GetLoginById(SessionManager.Ticket, RequestId)
                    : new TransitLogin();
            }

            return mLogin;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultButton(save);

            if (RequestId > 0)
            {
                inputName.Text = Login.Name;
                inputUsername.Text = Login.Username;
                inputEmail.Text = Login.Email;
                inputPassword.Attributes["value"] = Login.Password;
                inputAdministrator.Checked = (Login.Role == TransitLoginRole.Administrator);
            }
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            Login.Name = inputName.Text;
            Login.Username = inputUsername.Text;
            Login.Email = inputEmail.Text;
            Login.Password = inputPassword.Text;
            Login.Role = inputAdministrator.Checked ? TransitLoginRole.Administrator : TransitLoginRole.Guest;

            if (string.IsNullOrEmpty(Login.Name))
            {
                throw new ArgumentException("Missing Name");
            }

            SessionManager.BlogService.CreateOrUpdateLogin(SessionManager.Ticket, Login);
            Response.Redirect("ManageLogins.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
