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

public partial class ViewFeedItemsControl : BlogControl
{
    private int mFeedId = 0;

    public int FeedId
    {
        get
        {
            return DBlog.Tools.Web.ViewState<int>.GetViewStateValue(
                ViewState, string.Format("{0}:FeedId", ID), mFeedId);
        }
        set
        {
            DBlog.Tools.Web.ViewState<int>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:FeedId", ID), value, ref mFeedId);
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
        grid.DataSource = SessionManager.GetCachedCollection<TransitFeedItem>(
            "GetFeedItems", SessionManager.Ticket, new TransitFeedItemQueryOptions(
                FeedId, grid.PageSize, grid.CurrentPageIndex));

        panelItems.Update();
    }

    private void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitFeedItem>(
            "GetFeedItemsCount", SessionManager.Ticket, new TransitFeedItemQueryOptions(FeedId));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
