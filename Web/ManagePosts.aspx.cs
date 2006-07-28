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
using DBlog.Tools.Web;
using DBlog.TransitData;

public partial class admin_ManagePosts : BlogAdminPage
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
                    SessionManager.BlogService.DeletePost(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
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
        grid.DataSource = SessionManager.BlogService.GetPosts(
            SessionManager.Ticket, new TransitPostQueryOptions(grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetPostsCount(SessionManager.Ticket, null);
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public string Render(int id, string text)
    {
        string result = Renderer.RenderEx(text);

        result = new ReferencesRenderer((BlogPage)Page, id, "Post").Render(result);
        result = new LiveJournalRenderer((BlogPage)Page, id, "Post").Render(result);
        result = new MsnSpacesRenderer((BlogPage)Page, id, "Post").Render(result);

        return result;
    }
}
