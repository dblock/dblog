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
using DBlog.TransitData.References;
using DBlog.Tools.Web.Html;

public partial class ShowBlog : BlogPage
{
    private HtmlMeta mHtmlMetaDescription = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Header != null)
            {
                Header.Controls.Add(HtmlMetaDescription);
            }

            DBlogMaster master = (DBlogMaster)this.Master;
            master.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);
            master.Search += new SearchControl.SearchHandler(search_Search);
            master.DateRangeChanged += new DateRangeControl.DateRangeHandler(master_DateRangeChanged);

            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

            if (!IsPostBack)
            {
                String topicName = Request.Params["t"];
                if (! string.IsNullOrEmpty(topicName))
                {
                    TransitTopic topic = SessionManager.GetCachedObject<TransitTopic>("GetTopicByName", SessionManager.Ticket, topicName);
                    if (topic == null)
                    {
                        throw new Exception("Invalid topic: " + topicName);
                    }
                    TopicId = topic.Id;
                }
                else
                {
                    TopicId = RequestId;
                }
                Query = Request.Params["q"];
                GetData(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void master_DateRangeChanged(object sender, DateRangeControl.DateRangeEventArgs e)
    {
        try
        {
            DateStart = e.DateStart;
            DateEnd = e.DateEnd;
            GetData(sender, e);
            panelPosts.Update();
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    private int mTopicId = 0;
    private string mQuery = string.Empty;
    private DateTime mDateStart = DateTime.MinValue;
    private DateTime mDateEnd = DateTime.MaxValue;

    public DateTime DateStart
    {
        get
        {
            return DBlog.Tools.Web.ViewState<DateTime>.GetViewStateValue(
                EnableViewState, ViewState, "DateStart", mDateStart);
        }
        set
        {
            DBlog.Tools.Web.ViewState<DateTime>.SetViewStateValue(
                EnableViewState, ViewState, "DateStart", value, ref mDateStart);
        }
    }

    public DateTime DateEnd
    {
        get
        {
            return DBlog.Tools.Web.ViewState<DateTime>.GetViewStateValue(
                EnableViewState, ViewState, "DateEnd", mDateEnd);
        }
        set
        {
            DBlog.Tools.Web.ViewState<DateTime>.SetViewStateValue(
                EnableViewState, ViewState, "DateEnd", value, ref mDateEnd);
        }
    }

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
            grid.RepeatRows = 20;
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
        TransitPostQueryOptions options = GetOptions();

        grid.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            "GetPosts", SessionManager.PostTicket, options);

        GetCriteria();
    }

    private TransitPostQueryOptions GetOptions()
    {
        TransitPostQueryOptions options = new TransitPostQueryOptions(TopicId, Query, grid.PageSize, grid.CurrentPageIndex);
        
        options.DateStart = DateStart;
        options.DateEnd = DateEnd;

        options.PublishedOnly = ! SessionManager.IsAdministrator;
        options.DisplayedOnly = ! SessionManager.IsAdministrator;

        options.SortDirection = WebServiceQuerySortDirection.Descending;
        options.SortExpression = string.IsNullOrEmpty(options.Query) ? "Created" : "Rank";

        return options;
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPost>(
            "GetPostsCount", SessionManager.PostTicket, GetOptions());

        if (grid.VirtualItemCount == 0)
        {
            labelPosts.Text = "No Posts";
            labelPosts.Visible = true;
        }
        else
        {
            labelPosts.Visible = false;
        }

        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    private void GetCriteria()
    {
        StringBuilder queryText = new StringBuilder();
        if (!string.IsNullOrEmpty(Query))
        {
            queryText.AppendFormat("Found {0} post{1} with \"{2}\"", 
                grid.VirtualItemCount, grid.VirtualItemCount == 1 ? "" : "s",
                Renderer.Render(Query));

            if (TopicId != 0)
            {
                queryText.AppendFormat(" in \"{0}\"", Renderer.Render(
                    SessionManager.GetCachedObject<TransitTopic>("GetTopicById", SessionManager.Ticket, TopicId).Name));
            }

            queryText.Append(".");
        }
        else if (TopicId > 0)
        {
            queryText.AppendFormat("{0} post{1} in \"{2}\" ...", grid.VirtualItemCount,
                grid.VirtualItemCount == 1 ? "" : "s", Renderer.Render(
                SessionManager.GetCachedObject<TransitTopic>("GetTopicById", SessionManager.Ticket, TopicId).Name));
        }

        if (queryText.Length > 0)
        {
            labelCriteria.Text = queryText.ToString();
            labelCriteria.Visible = true;
        }
        else
        {
            labelCriteria.Visible = false;
        }
    }

    public string GetTopics(TransitTopic[] topics)
    {
        StringBuilder sb = new StringBuilder();
        foreach (TransitTopic topic in topics)
        {
            if (sb.Length != 0) sb.Append(", ");
            sb.Append(Renderer.Render(topic.Name));
        }
        return sb.ToString();
    }

    public void list_ItemCommand(object source, DataListCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    {
                        SessionManager.BlogService.DeletePost(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                        SessionManager.Invalidate<TransitPost>();
                        ReportInfo("Item Deleted");
                        GetData(source, e);
                    }
                    break;
                case "Display":
                    {
                        TransitPost t_post = SessionManager.BlogService.GetPostById(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                        t_post.Display = ! t_post.Display;
                        SessionManager.BlogService.CreateOrUpdatePost(SessionManager.Ticket, t_post);
                        ReportInfo(t_post.Display ? "Post Displayed" : "Post Hidden");
                        GetData(source, e);
                        break;
                    }
                case "Sticky":
                    {
                        TransitPost t_post = SessionManager.BlogService.GetPostById(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                        t_post.Sticky = ! t_post.Sticky;
                        SessionManager.BlogService.CreateOrUpdatePost(SessionManager.Ticket, t_post);
                        ReportInfo(t_post.Sticky ? "Post Stuck" : "Post Unstuck");
                        GetData(source, e);
                        break;
                    }
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

    public string GetImagesLink(int images_count)
    {
        if (images_count > 1)
        {
            return string.Format("&#187; {0} more image{1} after the cut ...", images_count
                , images_count == 1 ? string.Empty : "s");
        }

        return string.Empty;
    }

    public string GetImagesShortLink(int images_count)
    {
        StringBuilder result = new StringBuilder();

        if (images_count > 1)
        {
            result.AppendFormat(" | {0} Image{1}", images_count
                , images_count == 1 ? string.Empty : "s");
        }

        return result.ToString();
    }

    public string GetPostLink(int images_count, int id, string link, int image_id)
    {
        if (images_count == 1 && image_id > 0)
        {
            return string.Format("./ShowImage.aspx?id={0}&pid={1}", image_id, id);
        }
        else
        {
            return link;
        }
    }

    public HtmlMeta HtmlMetaDescription
    {
        get
        {
            if (mHtmlMetaDescription == null)
            {
                mHtmlMetaDescription = new HtmlMeta();
                mHtmlMetaDescription.Name = "description";
                mHtmlMetaDescription.Content = SessionManager.GetSetting(
                    "description", string.Empty);
            }
            return mHtmlMetaDescription;
        }
    }

    public override string RenderEx(string text, int id)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

 	    return base.RenderEx(Cutter.Cut(text, id), id);
    }    
}
