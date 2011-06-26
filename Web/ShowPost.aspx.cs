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
using DBlog.TransitData;
using System.Text;
using DBlog.Tools.Web;
using DBlog.Data.Hibernate;
using DBlog.TransitData.References;

public partial class ShowPost : BlogPage
{
    private TransitPost mPost = null;
    private bool mPreferredOnly = false;

    public bool PreferredOnly
    {
        get
        {
            return DBlog.Tools.Web.ViewState<bool>.GetViewStateValue(
                ViewState, string.Format("{0}:Preferred", ID), mPreferredOnly);
        }
        set
        {
            DBlog.Tools.Web.ViewState<bool>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:Preferred", ID), value, ref mPreferredOnly);
        }
    }

    public bool PreferredOnlyFromQueryString
    {
        get
        {
            object p = Request["PreferredOnly"];
            if (p == null) return false;
            bool pb = false;
            bool.TryParse(p.ToString(), out pb);
            return pb;
        }
    }

    public TransitPost Post
    {
        get
        {
            if (mPost == null)
            {
                if (RequestId > 0)
                {
                    mPost = SessionManager.GetCachedObject<TransitPost>(
                        "GetPostById", SessionManager.PostTicket, RequestId);
                }
                else
                {
                    mPost = new TransitPost();
                }
            }

            return mPost;
        }
    }

    public bool HasAccess
    {
        get
        {
            string key = string.Format("{0}:{1}:PostAccess", SessionManager.PostTicket, RequestId);
            object result = Cache[key];
            if (result == null)
            {
                result = SessionManager.BlogService.HasAccessToPost(
                    SessionManager.PostTicket, RequestId);
                Cache.Insert(key, result, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
            }
            return (bool)result;
        }
    }

    public void comments_ItemCommand(object sender, DataGridCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeleteComment(SessionManager.Ticket, 
                        int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitPostComment>();
                    SessionManager.Invalidate<TransitComment>();
                    GetCommentsData(sender, e);                
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            images.OnGetDataSource += new EventHandler(images_OnGetDataSource);
            comments.OnGetDataSource += new EventHandler(comments_OnGetDataSource);

            if (!IsPostBack)
            {
                PreferredOnly = PreferredOnlyFromQueryString;

                if (!HasAccess)
                {
                    Response.Redirect(string.Format("Login.aspx?r={0}&cookie={1}&access=denied",
                        Renderer.UrlEncode(Request.Url.PathAndQuery), SessionManager.sDBlogPostCookieName));
                }

                if (SessionManager.CountersEnabled)
                {
                    CounterCache.IncrementPostCounter(RequestId, Cache, SessionManager);
                }

                GetData(sender, e);

                spanAdmin.Visible = SessionManager.IsAdministrator;
                linkEdit.NavigateUrl = string.Format("EditPost.aspx?id={0}", RequestId);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void comments_OnGetDataSource(object sender, EventArgs e)
    {
        comments.DataSource = SessionManager.GetCachedCollection<TransitPostComment>(
            "GetPostComments", SessionManager.PostTicket, new TransitPostCommentQueryOptions(
                Post.Id, comments.AllowPaging ? comments.PageSize : 0,
                comments.AllowPaging ? comments.CurrentPageIndex : 0));
    }

    public void GetData(object sender, EventArgs e)
    {
        TransitPost post = Post;

        // linkComment.NavigateUrl = string.Format("EditPostComment.aspx?sid={0}", post.Id);

        Title = posttitle.Text = Renderer.Render(post.Title);
        postbody.Text = RenderEx(post.Body, post.Id);
        posttopics.Text = GetTopics(post.Topics);
        postcreated.Text = post.Created.ToString("d");

        if (SessionManager.CountersEnabled)
        {
            postcounter.Text = string.Format("{0} Click{1}", post.Counter.Count,
                post.Counter.Count != 1 ? "s" : string.Empty);
        }
        else
        {
            postcounter.Visible = false;
        }

        panelPicture.Visible = (post.ImageId != 0 && post.ImagesCount <= 1);
        postimage.ImageUrl = string.Format("ShowPicture.aspx?id={0}", post.ImageId);
        linkimage.HRef = string.Format("ShowImage.aspx?id={0}&pid={1}", post.ImageId, post.Id);

        twitterShare.Url = string.Format("{0}ShowPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id);
        twitterShare.Text = post.Title;

        disqusComments.DisqusId = string.Format("Post_{0}", post.Id);
        disqusComments.DisqusUrl = string.Format("{0}ShowPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id);

        GetImagesData(sender, e);
        GetCommentsData(sender, e);
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

    void GetCommentsData(object sender, EventArgs e)
    {
        TransitPost post = Post;

        comments.Visible = (post.CommentsCount > 0);
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPostComment>(
            "GetPostCommentsCount", SessionManager.PostTicket, new TransitPostCommentQueryOptions(Post.Id));
        comments_OnGetDataSource(sender, e);
        comments.DataBind();
    }

    void GetImagesData(object sender, EventArgs e)
    {
        TransitPost post = Post;

        TransitPostImageQueryOptions imagesoptions = new TransitPostImageQueryOptions(Post.Id);
        imagesoptions.PreferredOnly = PreferredOnly;
        images.Visible = (post.ImagesCount > 1);
        images.CurrentPageIndex = 0;
        images.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPostImage>(
            "GetPostImagesCountEx", SessionManager.PostTicket, imagesoptions);
        images_OnGetDataSource(sender, e);
        images.DataBind();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        string sortexpression = Request.Params["SortExpression"];
        string sortdirection = Request.Params["SortDirection"];

        TransitPostImageQueryOptions options = new TransitPostImageQueryOptions(
            Post.Id, images.PageSize, images.CurrentPageIndex);

        options.SortDirection = string.IsNullOrEmpty(sortdirection)
            ? WebServiceQuerySortDirection.Ascending
            : (WebServiceQuerySortDirection)Enum.Parse(typeof(WebServiceQuerySortDirection), sortdirection);

        options.SortExpression = string.IsNullOrEmpty(sortexpression)
            ? "Image.Image_Id"
            : sortexpression;

        options.PreferredOnly = PreferredOnly;

        images.DataSource = SessionManager.GetCachedCollection<TransitPostImage>(
            "GetPostImagesEx", SessionManager.PostTicket, options);
    }

    public string GetComments(TransitImage image)
    {
        if (image == null)
            return string.Empty;

        if (image.CommentsCount == 0)
            return string.Empty;

        return string.Format("{0} Comment{1}", image.CommentsCount,
            image.CommentsCount != 1 ? "s" : string.Empty);
    }

    public string GetCounter(TransitImage image)
    {
        if (image == null)
            return String.Empty;

        if (image.Counter.Count == 0)
            return string.Empty;

        return string.Format("[{0}]", image.Counter.Count);
    }

    public void linkPreferred_Click(object sender, EventArgs e)
    {
        try
        {
            PreferredOnly = ! PreferredOnly;
            GetImagesData(sender, e);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        linkPreferred.Text = PreferredOnly ? "Show All" : "Favorites";
        base.OnPreRender(e);
    }

    public void images_OnItemCommand(object sender, DataListCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "TogglePreferred":
                    TransitImage image = SessionManager.GetCachedObject<TransitImage>(
                        "GetImageById", SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    image.Preferred = !image.Preferred;
                    SessionManager.BlogService.CreateOrUpdateImageAttributes(SessionManager.Ticket, image);
                    SessionManager.Invalidate<TransitImage>();
                    images_OnGetDataSource(sender, e);
                    ((LinkButton)e.CommandSource).Text = image.Preferred ? "P" : "p";
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public string GetImageUri(TransitPostImage ti)
    {
        StringBuilder result = new StringBuilder();
        result.AppendFormat("ShowImage.aspx?id={0}&pid={1}&index={2}",
            ti.Image.Id, ti.Post.Id, ti.Index);
        if (PreferredOnly) result.Append("&PreferredOnly=true");
        if (!string.IsNullOrEmpty(Request.Params["SortExpression"])) result.AppendFormat("&SortExpression={0}", Request.Params["SortExpression"]);
        if (!string.IsNullOrEmpty(Request.Params["SortDirection"])) result.AppendFormat("&SortDirection={0}", Request.Params["SortDirection"]);
        return result.ToString();
    }
}
