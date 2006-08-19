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
using DBlog.Data.Hibernate;

public partial class ShowBlog : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            DBlogMaster master = (DBlogMaster)this.Master;
            master.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);
            master.Search += new SearchControl.SearchHandler(search_Search);

            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

            if (!IsPostBack)
            {
                TopicId = RequestId;
                Query = Request.Params["q"];
                GetData(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    private int mTopicId = 0;
    private string mQuery = string.Empty;

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

    public string Query
    {
        get
        {
            return DBlog.Tools.Web.ViewState<string>.GetViewStateValue(
                EnableViewState, ViewState, "Query", mQuery);
        }
        set
        {
            DBlog.Tools.Web.ViewState<string>.SetViewStateValue(
                EnableViewState, ViewState, "Query", value, ref mQuery);
        }
    }

    public void search_Search(object sender, SearchControl.SearchEventArgs e)
    {
        try
        {
            Query = e.Query;
            GetData(sender, e);
            panelPosts.Update();
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void topics_TopicChanged(object sender, ViewTopicsControl.TopicChangedEventArgs e)
    {
        try
        {
            TopicId = e.TopicId;
            GetData(sender, e);
            panelPosts.Update();
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        TransitPostQueryOptions options = new TransitPostQueryOptions(TopicId, Query, grid.PageSize, grid.CurrentPageIndex);
        options.SortDirection = string.IsNullOrEmpty(sortdirection) 
            ? WebServiceQuerySortDirection.Descending
            : (WebServiceQuerySortDirection)Enum.Parse(typeof(WebServiceQuerySortDirection), sortdirection);

        options.SortExpression = string.IsNullOrEmpty(sortexpression) 
            ? "Created" 
            : sortexpression;

        grid.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            (string.IsNullOrEmpty(sortexpression) || sortexpression.IndexOf('.') < 0) ? "GetPosts" : "GetPostsEx",
            SessionManager.PostTicket, options);
    }

    public void GetData(object sender, EventArgs e)
    {
        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            (string.IsNullOrEmpty(sortexpression) || sortexpression.IndexOf('.') < 0) ? "GetPostsCount" : "GetPostsCountEx",
            SessionManager.PostTicket, new TransitPostQueryOptions(TopicId, Query));
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
