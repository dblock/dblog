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

public partial class EditReferrerHostRollup : BlogAdminPage
{
    private TransitReferrerHostRollup mReferrerHostRollup = null;

    public TransitReferrerHostRollup ReferrerHostRollup
    {
        get
        {
            if (mReferrerHostRollup == null)
            {
                mReferrerHostRollup = (RequestId > 0)
                    ? SessionManager.BlogService.GetReferrerHostRollupById(SessionManager.Ticket, RequestId)
                    : new TransitReferrerHostRollup();
            }

            return mReferrerHostRollup;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultButton(save);

            if (RequestId > 0)
            {
                inputName.Text = ReferrerHostRollup.Name;
                inputRollup.Text = ReferrerHostRollup.Rollup;
            }
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            ReferrerHostRollup.Name = CheckInput("Name", inputName.Text);
            ReferrerHostRollup.Rollup = inputRollup.Text;
            SessionManager.BlogService.CreateOrUpdateReferrerHostRollup(SessionManager.Ticket, ReferrerHostRollup);
            Response.Redirect("ManageReferrerHostRollups.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
