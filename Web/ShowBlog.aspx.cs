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
using System.Text.RegularExpressions;
using DBlog.TransitData;
using DBlog.Tools.Web;

public partial class ShowBlog : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DBlogMaster master = (DBlogMaster) this.Master;
        master.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);

        grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

        if (!IsPostBack)
        {
            TopicId = RequestId;
            GetData(sender, e);
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

    public void topics_TopicChanged(object sender, ViewTopicsControl.TopicChangedEventArgs e)
    {
        GetData(sender, e);
        panelPosts.Update();
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.BlogService.GetPosts(
            SessionManager.Ticket, new TransitPostQueryOptions(TopicId, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetPostsCount(SessionManager.Ticket, new TransitPostQueryOptions(TopicId));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public string Render(int id, string type, string text)
    {
        string result = Renderer.RenderEx(text);

        result = new ReferencesRenderer((BlogPage)Page, id, type).Render(result);
        result = new LiveJournalRenderer((BlogPage)Page, id, type).Render(result);
        result = new MsnSpacesRenderer((BlogPage)Page, id, type).Render(result);

        return result;
    }
}
