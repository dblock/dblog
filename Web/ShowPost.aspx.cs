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

public partial class ShowPost : BlogPage
{
    private TransitPost mPost = null;

    public TransitPost Post
    {
        get
        {
            if (mPost == null)
            {
                mPost = (RequestId > 0)
                    ? SessionManager.BlogService.GetPostById(SessionManager.Ticket, RequestId)
                    : new TransitPost();
            }

            return mPost;
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
                SessionManager.BlogService.IncrementPostCounter(
                    SessionManager.Ticket, RequestId);
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
        comments.DataSource = SessionManager.BlogService.GetPostComments(
            SessionManager.Ticket, new TransitPostCommentQueryOptions(
                Post.Id, comments.AllowPaging ? comments.PageSize : 0, 
                comments.AllowPaging ? comments.CurrentPageIndex : 0));
    }

    public void GetData(object sender, EventArgs e)
    {
        TransitPost post = Post;

        linkComment.NavigateUrl = string.Format("EditPostComment.aspx?sid={0}", post.Id);

        posttitle.Text = post.Title;
        postbody.Text = post.Body;
        postcreated.Text = post.Created.ToString("d");
        postcounter.Text = string.Format("{0} Click{1}", post.Counter.Count,
            post.Counter.Count != 1 ? "s" : string.Empty);
        panelPicture.Visible = (post.ImageId != 0 && post.ImagesCount <= 1);
        postimage.ImageUrl = string.Format("ShowPicture.aspx?id={0}", post.ImageId);
        linkimage.HRef = string.Format("ShowImage.aspx?id={0}&pid={1}", post.ImageId, post.Id);

        images.Visible = (post.ImagesCount > 1);
        images.CurrentPageIndex = 0;
        images.VirtualItemCount = SessionManager.BlogService.GetPostImagesCount(
            SessionManager.Ticket, new TransitPostImageQueryOptions(Post.Id));
        images_OnGetDataSource(sender, e);
        images.DataBind();

        comments.Visible = (post.CommentsCount > 0);
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.BlogService.GetPostCommentsCount(
            SessionManager.Ticket, new TransitPostCommentQueryOptions(Post.Id));
        comments_OnGetDataSource(sender, e);
        comments.DataBind();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        images.DataSource = SessionManager.BlogService.GetPostImages(
            SessionManager.Ticket, new TransitPostImageQueryOptions(Post.Id, images.PageSize, images.CurrentPageIndex));
    }

    public string GetImageLink(string name, int comments_count)
    {
        StringBuilder result = new StringBuilder(name);

        if (comments_count > 0)
        {
            result.AppendFormat(" | {0} Comment{1}", comments_count,
                comments_count != 1 ? "s" : string.Empty);
        }

        return result.ToString();
    }
}
