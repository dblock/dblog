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
using DBlog.Data.Hibernate;
using DBlog.TransitData;

public partial class ViewFeedsControl : BlogControl
{
    private TransitFeedType mTransitFeedType = TransitFeedType.Unknown;

    public TransitFeedType Type
    {
        get
        {
            return mTransitFeedType;
        }
        set
        {
            mTransitFeedType = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

            if (!IsPostBack)
            {
                GetData(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.GetCachedCollection<TransitFeed>(
            "GetFeeds", SessionManager.Ticket, new TransitFeedQueryOptions(Type));
    }

    private void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
