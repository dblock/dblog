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

public partial class admin_ManageImages : BlogAdminPage
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

    public void grid_ItemCommand(object source, DataListCommandEventArgs e)
    {
        try
        {
            switch(e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeleteImage(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitImage>();
                    ReportInfo("Item Deleted");
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
        grid.DataSource = SessionManager.GetCachedCollection<TransitImage>(
            "GetImages", SessionManager.Ticket,  new TransitImageQueryOptions(true, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitImage>(
            "GetImagesCount", SessionManager.Ticket, new TransitImageQueryOptions(true));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
