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
using System.Text;

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
        try
        {
            topics.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);

            if (!IsPostBack)
            {
                panelAdmin.Visible = SessionManager.IsAdministrator;

                if (SessionManager.LoginRecord != null)
                {
                    labelUsername.Text = string.Format("logged in as {0}", Renderer.Render(
                        string.IsNullOrEmpty(SessionManager.LoginRecord.Name)
                            ? SessionManager.LoginRecord.Username
                            : SessionManager.LoginRecord.Name));
                }

                if (SessionManager.PostLoginRecord != null)
                {
                    labelPostUsername.Text = string.Format("post access as {0}", Renderer.Render(
                        string.IsNullOrEmpty(SessionManager.PostLoginRecord.Name)
                            ? SessionManager.PostLoginRecord.Username
                            : SessionManager.PostLoginRecord.Name));
                }

                imageMain.ImageUrl = SessionManager.GetSetting("image", imageMain.ImageUrl);
                imageMain.Width = int.Parse(SessionManager.GetSetting("imagewidth", imageMain.Width.ToString()));
                imageMain.Height = int.Parse(SessionManager.GetSetting("imageheight", imageMain.Height.ToString()));

                TransitCounter c = Counter;
                labelCounter.Text = string.Format("{0} click{1} since {2}",
                    c.Count, c.Count != 1 ? "s" : string.Empty, c.Created.ToString("d"));
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void topics_TopicChanged(object sender, ViewTopicsControl.TopicChangedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            ReportException(ex);
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
