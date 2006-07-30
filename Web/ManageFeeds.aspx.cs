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

public partial class admin_ManageFeeds : BlogAdminPage
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

    public void grid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            switch(e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeleteFeed(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    ReportInfo("Item Deleted");
                    GetData(source, e);
                    break;
                case "Update":
                    int count = SessionManager.BlogService.UpdateFeed(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    ReportInfo(string.Format("Feed Updated With {0} New Item(s)", count));
                    grid_OnGetDataSource(source, e);
                    grid.DataBind();                    
                    break;
            }
        }
        catch(Exception ex)
        {
            ReportException(ex);
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.BlogService.GetFeeds(
            SessionManager.Ticket, new TransitFeedQueryOptions(grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetFeedsCount(
            SessionManager.Ticket, null);
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
