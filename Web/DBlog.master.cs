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
    public event SearchControl.SearchHandler Search;
    public event DateRangeControl.DateRangeHandler DateRangeChanged;

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
            // topics.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);
            searchBox.Search += new SearchControl.SearchHandler(searchBox_Search);
            dates.DateRangeChanged += new DateRangeControl.DateRangeHandler(dates_DateRangeChanged);

            if (!IsPostBack)
            {
                panelAdmin.Visible = SessionManager.IsAdministrator;
                linkLogout2.Visible = SessionManager.IsLoggedIn;

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


                if (SessionManager.CountersEnabled)
                {
                    TransitCounter c = Counter;
                    labelCounter.Text = string.Format("{0} click{1} since {2}",
                        c.Count, c.Count != 1 ? "s" : string.Empty, c.Created.ToString("d"));
                }

                linkAtomPost.Href = string.Format("{0}AtomSvc.aspx",
                    SessionManager.WebsiteUrl);
                linkAtomPost.Attributes["title"] = string.Format("{0} {1}",
                    SessionManager.GetSetting("title", "Untitled"),
                    linkAtom.Attributes["title"]);

                linkAtom.Href = string.Format("{0}AtomBlog.aspx",
                    SessionManager.WebsiteUrl);
                linkAtom.Attributes["title"] = string.Format("{0} {1}",
                    SessionManager.GetSetting("title", "Untitled"),
                    linkAtom.Attributes["title"]);

                linkRss.Href = string.Format("{0}RssBlog.aspx",
                    SessionManager.WebsiteUrl);
                linkRss.Attributes["title"] = string.Format("{0} {1}",
                    SessionManager.GetSetting("title", "Untitled"),
                    linkRss.Attributes["title"]);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void dates_DateRangeChanged(object sender, DateRangeControl.DateRangeEventArgs e)
    {
        try
        {
            if (DateRangeChanged != null)
            {
                DateRangeChanged(sender, e);
            }
            else if (e.DateEnd != DateTime.MaxValue && e.DateStart != DateTime.MinValue)
            {
                Response.Redirect(string.Format("?start={0}&end={1}", Renderer.UrlEncode(e.DateStart), Renderer.UrlEncode(e.DateEnd)));
                panelDates.Update();
            }
            else
            {
                Response.Redirect(".");
                panelDates.Update();
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }        
    }

    void searchBox_Search(object sender, SearchControl.SearchEventArgs e)
    {
        try
        {
            if (Search != null)
            {
                Search(sender, e);
            }
            else
            {
                Response.Redirect(string.Format("?q={0}", Renderer.UrlEncode(e.Query)));
                panelSearch.Update();
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
                Response.Redirect(string.Format("?id={0}", e.TopicId));
                // panelTopics.Update();
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
            Response.Redirect(".");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void linkContact_Click(object sender, EventArgs e)
    {
        Page.ClientScript.RegisterStartupScript(this.GetType(), "email",
            string.Format("<script language='JavaScript'>window.location.href='mailto:{0}';</script>",
            SessionManager.GetSetting("email", "admin@localhost.com")));
    }

    public void linkInvalidateCache_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Cache.Insert("Pages", DateTime.Now, null,
            System.DateTime.MaxValue, System.TimeSpan.Zero,
            System.Web.Caching.CacheItemPriority.NotRemovable,
            null);
    }

    public void linkReslug_Click(object sender, EventArgs e)
    {
        int i = SessionManager.BlogService.GeneratePostSlugs(SessionManager.Ticket);
        linkReslug.Text = string.Format("Reslug ({0})", i);
    }
}
