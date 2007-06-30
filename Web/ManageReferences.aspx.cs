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

public partial class admin_ManageReferences : BlogAdminPage
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
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeleteReference(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    ReportInfo("Item Deleted");
                    GetData(source, e);
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        WebServiceQueryOptions options = new WebServiceQueryOptions(grid.PageSize, grid.CurrentPageIndex);
        options.SortExpression = "Id";
        options.SortDirection = WebServiceQuerySortDirection.Descending;
        if (!string.IsNullOrEmpty(inputSearch.Text))
        {
            grid.DataSource = SessionManager.BlogService.SearchReferences(
                SessionManager.Ticket, inputSearch.Text, options);
        }
        else
        {
            grid.DataSource = SessionManager.BlogService.GetReferences(
                SessionManager.Ticket, options);
        }
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        if (!string.IsNullOrEmpty(inputSearch.Text))
        {
            SessionManager.BlogService.SearchReferencesCount(SessionManager.Ticket, inputSearch.Text);
        }
        else
        {
            grid.VirtualItemCount = SessionManager.BlogService.GetReferencesCount(SessionManager.Ticket);
        }
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public void search_Click(object sender, EventArgs e)
    {
        GetData(sender, e);
    }
}
