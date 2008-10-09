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
using DBlog.WebServices;
using System.Collections.Specialized;

public partial class StatsSummary : BlogAdminPage
{
    private TransitStats mTransitStats;

    public TransitStats TransitStats
    {
        get
        {
            if (mTransitStats == null)
            {
                mTransitStats = SessionManager.GetCachedObject<TransitStats>("GetStats", SessionManager.Ticket, 0);
            }
            return mTransitStats;
        }
    }

    public void Page_Load()
    {
        try
        {
            if (!IsPostBack)
            {
                ListItemCollection c = new ListItemCollection();
                c.Add(new ListItem("Total Posts", TransitStats.PostsCount.ToString()));
                c.Add(new ListItem("Total Images", TransitStats.ImagesCount.ToString()));
                c.Add(new ListItem("Total Comments", TransitStats.CommentsCount.ToString()));
                c.Add(new ListItem("Rss Hits", string.Format("{0} since {1}",
                    TransitStats.RssCount.Count, TransitStats.RssCount.Created.ToString("d"))));
                c.Add(new ListItem("Atom Hits", string.Format("{0} since {1}", 
                    TransitStats.AtomCount.Count, TransitStats.AtomCount.Created.ToString("d"))));
                grid.DataSource = c;
                grid.DataBind();
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
