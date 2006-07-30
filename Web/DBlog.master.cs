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
using DBlog.Tools.Web;
using DBlog.TransitData;

public partial class DBlogMaster : MasterPage
{
    public event ViewTopicsControl.TopicChangedHandler TopicChanged;

    private TransitCounter Counter
    {
        get
        {
            string key = string.Format("{0}:counter", ID);
            TransitCounter counter = (TransitCounter)Cache[key];
            if (counter == null)
            {
                counter = SessionManager.BlogService.GetHourlyCountSum(SessionManager.Ticket);
                Cache.Insert(key, counter, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
            }
            return counter;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        topics.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);

        if (!IsPostBack)
        {
            panelAdmin.Visible = SessionManager.IsAdministrator;
            labelUsername.Text = SessionManager.IsLoggedIn ? 
                string.Format("logged in as {0}", Renderer.Render(SessionManager.LoginRecord.Name)) 
                : string.Empty;

            TransitCounter c = Counter;
            labelCounter.Text = string.Format("{0} click{1} since {2}",
                c.Count, c.Count != 1 ? "s" : string.Empty, c.Created.ToString("d"));
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
