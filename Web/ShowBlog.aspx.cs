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

public partial class ShowBlog : BlogPage
{
    private HtmlMeta mMetaDescription = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Header != null)
            {
                Header.Controls.Add(MetaDescription);
            }

            DBlogMaster master = (DBlogMaster)this.Master;
            master.TopicChanged += new ViewTopicsControl.TopicChangedHandler(topics_TopicChanged);
            master.Search += new SearchControl.SearchHandler(search_Search);
            master.DateRangeChanged += new DateRangeControl.DateRangeHandler(master_DateRangeChanged);

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

        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        grid.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            (string.IsNullOrEmpty(sortexpression) || sortexpression.IndexOf('.') < 0) ? "GetPosts" : "GetPostsEx",
            SessionManager.PostTicket, options);
    }

    private TransitPostQueryOptions GetOptions()
    {
        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        TransitPostQueryOptions options = new TransitPostQueryOptions(TopicId, Query, grid.PageSize, grid.CurrentPageIndex);
        
        options.DateStart = DateStart;
        options.DateEnd = DateEnd;

        options.PublishedOnly = ! SessionManager.IsAdministrator;

        options.SortDirection = string.IsNullOrEmpty(sortdirection)
            ? WebServiceQuerySortDirection.Descending
            : (WebServiceQuerySortDirection)Enum.Parse(typeof(WebServiceQuerySortDirection), sortdirection);

        options.SortExpression = string.IsNullOrEmpty(sortexpression)
            ? "Created"
            : sortexpression;

        return options;
    }

    public void GetData(object sender, EventArgs e)
    {
        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            (string.IsNullOrEmpty(sortexpression) || sortexpression.IndexOf('.') < 0) ? "GetPostsCount" : "GetPostsCountEx",
            SessionManager.PostTicket, GetOptions());

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

    public string GetImagesLink(int images_count)
    {
        if (images_count > 1)
        {
            return string.Format("&#187; {0} more image{1} after the cut ...", images_count
                , images_count == 1 ? string.Empty : "s");
        }

        return string.Empty;
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

    public string GetPostLink(int images_count, int id, int image_id)
    {
        if (images_count == 1 && image_id > 0)
        {
            return string.Format("ShowImage.aspx?id={0}&pid={1}", image_id, id);
        }
        else
        {
            return string.Format("ShowPost.aspx?id={0}", id);
        }
    }

    public HtmlMeta MetaDescription
    {
        get
        {
            if (mMetaDescription == null)
            {
                mMetaDescription = new HtmlMeta();
                mMetaDescription.Name = "description";
                mMetaDescription.Content = SessionManager.GetSetting(
                    "description", string.Empty);
            }
            return mMetaDescription;
        }
    }
}
