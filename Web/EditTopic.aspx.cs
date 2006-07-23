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

public partial class EditTopic : AdminPage
{
    private TransitTopic mTopic = null;

    public TransitTopic Topic
    {
        get
        {
            if (mTopic == null)
            {
                mTopic = (RequestId > 0)
                    ? SessionManager.BlogService.GetTopicById(SessionManager.Ticket, RequestId)
                    : new TransitTopic();
            }

            return mTopic;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultButton(save);

            if (RequestId > 0)
            {
                inputName.Text = Topic.Name;
            }
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            Topic.Name = CheckInput("Name", inputName.Text);
            SessionManager.BlogService.CreateOrUpdateTopic(SessionManager.Ticket, Topic);
            Response.Redirect("ManageTopics.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
