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

public partial class ShowPost : BlogPage
{
    private TransitPost mPost = null;

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

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            images.OnGetDataSource += new EventHandler(images_OnGetDataSource);
            comments.OnGetDataSource += new EventHandler(comments_OnGetDataSource);

            if (!IsPostBack)
            {
                if (!HasAccess)
                {
                    Response.Redirect(string.Format("Login.aspx?r={0}&cookie={1}",
                        Renderer.UrlEncode(Request.Url.PathAndQuery), SessionManager.sDBlogPostCookieName));
                }

                CounterCache.IncrementPostCounter(RequestId, Cache, SessionManager);

                GetData(sender, e);
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

        linkComment.NavigateUrl = string.Format("EditPostComment.aspx?sid={0}", post.Id);

        posttitle.Text = Renderer.RenderEx(post.Title);
        postbody.Text = Renderer.RenderEx(post.Body);
        postcreated.Text = post.Created.ToString("d");
        postcounter.Text = string.Format("{0} Click{1}", post.Counter.Count,
            post.Counter.Count != 1 ? "s" : string.Empty);
        panelPicture.Visible = (post.ImageId != 0 && post.ImagesCount <= 1);
        postimage.ImageUrl = string.Format("ShowPicture.aspx?id={0}", post.ImageId);
        linkimage.HRef = string.Format("ShowImage.aspx?id={0}&pid={1}", post.ImageId, post.Id);

        images.Visible = (post.ImagesCount > 1);
        images.CurrentPageIndex = 0;
        images.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            "GetPostImagesCount", SessionManager.PostTicket, new TransitPostImageQueryOptions(Post.Id));
        images_OnGetDataSource(sender, e);
        images.DataBind();

        comments.Visible = (post.CommentsCount > 0);
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.GetCachedCollectionCount(
            "GetPostCommentsCount", SessionManager.PostTicket, new TransitPostCommentQueryOptions(Post.Id));
        comments_OnGetDataSource(sender, e);
        comments.DataBind();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        images.DataSource = SessionManager.GetCachedCollection<TransitPostImage>(
            "GetPostImages", SessionManager.PostTicket, new TransitPostImageQueryOptions(Post.Id, images.PageSize, images.CurrentPageIndex));
    }

    public string GetComments(TransitImage image)
    {
        if (image.CommentsCount == 0)
            return string.Empty;

        return string.Format("{0} Comment{1}", image.CommentsCount,
            image.CommentsCount != 1 ? "s" : string.Empty);
    }

    public string GetCounter(TransitImage image)
    {
        if (image.Counter.Count == 0)
            return string.Empty;

        return string.Format("[{0}]", image.Counter.Count);
    }
}
