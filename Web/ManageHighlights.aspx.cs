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

public partial class admin_ManageHighlights : BlogAdminPage
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
                    SessionManager.BlogService.DeleteHighlight(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitHighlight>();
                    ReportInfo("Item Deleted");
                    GetData(source, e);
                    break;
                case "Up":
                    SessionManager.BlogService.MoveHighlight(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()), -1);
                    SessionManager.Invalidate<TransitHighlight>();
                    GetData(source, e);
                    break;
                case "Down":
                    SessionManager.BlogService.MoveHighlight(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()), 1);
                    SessionManager.Invalidate<TransitHighlight>();
                    GetData(source, e);
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
        grid.DataSource = SessionManager.GetCachedCollection<TransitHighlight>(
            "GetHighlights", SessionManager.Ticket, new WebServiceQueryOptions(grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitHighlight>(
            "GetHighlightsCount", SessionManager.Ticket, null);
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public int Count
    {
        get
        {
            return grid.VirtualItemCount;
        }
    }
}
