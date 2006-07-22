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
using DBlog.Tools;
using DBlog.TransitData;
using DBlog.Tools.Web;

public partial class ViewBlogsControl : Control
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
    
    private int mTopicId = 0;

    public int TopicId
    {
        get
        {
            return DBlog.Tools.Web.ViewState<int>.GetViewStateValue(
                EnableViewState, ViewState, "TopicId", mTopicId);
        }
        set
        {
            DBlog.Tools.Web.ViewState<int>.SetViewStateValue(
                EnableViewState, ViewState, "TopicId", value, ref mTopicId);
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.BlogService.GetBlogs(
            SessionManager.Ticket, new TransitBlogQueryOptions(TopicId, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetBlogsCount(SessionManager.Ticket, new TransitBlogQueryOptions(TopicId));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public string Render(int id, string type, string text)
    {
        string result = Renderer.RenderEx(text);

        result = new ReferencesRenderer((Page) Page, id, type).Render(result);
        result = new LiveJournalRenderer((Page)Page, id, type).Render(result);
        result = new MsnSpacesRenderer((Page)Page, id, type).Render(result);

        return result;
    }
}
