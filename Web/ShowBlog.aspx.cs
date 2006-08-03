using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using DBlog.TransitData;
using DBlog.Tools.Web;
using System.Text;

public partial class ShowBlog : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            DBlogMaster master = (DBlogMaster)this.Master;
            master.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);

            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

            if (!IsPostBack)
            {
                TopicId = RequestId;
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

    public void topics_TopicChanged(object sender, ViewTopicsControl.TopicChangedEventArgs e)
    {
        TopicId = e.TopicId;
        GetData(sender, e);
        panelPosts.Update();
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            "GetPosts", SessionManager.PostTicket, new TransitPostQueryOptions(
                TopicId, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            "GetPostsCount", SessionManager.PostTicket, new TransitPostQueryOptions(TopicId));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public void grid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeletePost(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
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

    public string GetCounter(long count)
    {
        return string.Format("{0} Click{1}", count,
            count != 1 ? "s" : string.Empty);
    }

    public string GetLink(int comments_count, int images_count)
    {
        StringBuilder result = new StringBuilder();

        if (images_count > 1)
        {
            result.AppendFormat(" | {0} Image{1}", images_count
                , images_count == 1 ? string.Empty : "s");
        }

        if (comments_count > 0)
        {
            result.AppendFormat(" | {0} Comment{1}", comments_count
                , comments_count == 1 ? string.Empty : "s");
        }

        return result.ToString();
    }
}
