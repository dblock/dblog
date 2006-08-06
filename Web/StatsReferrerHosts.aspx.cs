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

public partial class StatsReferrerHosts : BlogPage
{
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
        grid.DataSource = SessionManager.GetCachedCollection<TransitReferrerHost>(
            "GetReferrerHosts", SessionManager.Ticket, new WebServiceQueryOptions(grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            "GetReferrerHostsCount", SessionManager.Ticket, null);
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
