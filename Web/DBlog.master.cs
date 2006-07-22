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

public partial class DBlogMaster : MasterPage
{
    public event ViewTopicsControl.TopicChangedHandler TopicChanged;

    protected void Page_Load(object sender, EventArgs e)
    {
        topics.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);

        if (!IsPostBack)
        {
            panelAdmin.Visible = SessionManager.IsAdministrator;
        }
    }

    public void topics_TopicChanged(object sender, ViewTopicsControl.TopicChangedEventArgs e)
    {
        if (TopicChanged != null)
        {
            TopicChanged(sender, e);
        }
        else
        {
            Response.Redirect(string.Format("ShowBlog.aspx?id={0}", e.TopicId));
            panelTopics.Update();
        }
    }

    public void linkLogout_Click(object sender, EventArgs e)
    {
        try
        {
            SessionManager.Logout();
            Response.Redirect("ShowBlog.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
