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
using System.Collections.Generic;
using DBlog.Tools.Web;

public partial class ShowImage : BlogPage
{
    private TransitPostImage mPostImage = null;

    public TransitPostImage PostImage
    {
        get
        {
            return mPostImage;
        }
        set
        {
            mPostImage = value;
        }
    }

    public bool HasAccess
    {
        get
        {
            int pid = GetId("pid");
            if (pid == 0)
                return true;

            string key = string.Format("{0}:{1}:PostAccess", SessionManager.PostTicket, pid);
            object result = Cache[key];
            if (result == null)
            {
                result = SessionManager.BlogService.HasAccessToPost(
                    SessionManager.PostTicket, pid);
                Cache.Insert(key, result, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
            }
            return (bool)result;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!HasAccess)
            {
                Response.Redirect(string.Format("Login.aspx?r={0}&cookie={1}",
                    Renderer.UrlEncode(Request.Url.PathAndQuery), SessionManager.sDBlogPostCookieName));
            }

            comments.OnGetDataSource += new EventHandler(comments_OnGetDataSource);
            images.OnGetDataSource += new EventHandler(images_OnGetDataSource);

            if (!IsPostBack)
            {
                GetDataImages(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void comments_OnGetDataSource(object sender, EventArgs e)
    {
        if (PostImage == null)
            return;

        panelComments.Update();
        comments.DataSource = SessionManager.BlogService.GetImageComments(
            SessionManager.PostTicket, new TransitImageCommentQueryOptions(
                PostImage.Image.Id, comments.AllowPaging ? comments.PageSize : 0, 
                comments.AllowPaging ? comments.CurrentPageIndex : 0));
    }

    public void GetDataImages(object sender, EventArgs e)
    {
        images.CurrentPageIndex = 0;
        int pid = GetId("pid");
        int index = GetId("index");

        if (RequestId > 0 && ! IsPostBack)
        {
            if (pid > 0)
            {
                images.CurrentPageIndex = index;
                images.VirtualItemCount = SessionManager.BlogService.GetPostImagesCount(
                    SessionManager.PostTicket, new TransitPostImageQueryOptions(pid));
            }
            else
            {
                images.VirtualItemCount = 1;
            }
        }

        images_OnGetDataSource(sender, e);
        images.DataBind();
        panelImages.Update();
    }

    public void GetDataComments(object sender, EventArgs e)
    {
        if (PostImage == null)
            return;

        labelName.Text = PostImage.Image.Name;

        labelCount.Text = string.Format("{0} Click{1}",
            PostImage.Image.Counter.Count,
            PostImage.Image.Counter.Count != 1 ? "s" : string.Empty);

        panelPicture.Update();

        comments.Visible = (PostImage.Image.CommentsCount > 0);
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.BlogService.GetImageCommentsCount(
            SessionManager.PostTicket, new TransitImageCommentQueryOptions(PostImage.Image.Id));
        comments_OnGetDataSource(sender, e);
        comments.DataBind();

        panelComments.Update();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        int pid = GetId("pid");

        List<TransitPostImage> list = null;
        if (pid > 0)
        {
            list = SessionManager.BlogService.GetPostImages(
                SessionManager.PostTicket, new TransitPostImageQueryOptions(
                    pid, images.PageSize, images.CurrentPageIndex));

            linkBack.NavigateUrl = string.Format("ShowPost.aspx?id={0}", GetId("pid"));
        }
        else
        {
            TransitPostImage image = SessionManager.BlogService.GetPostImageById(SessionManager.PostTicket,
                RequestId);
            list = new List<TransitPostImage>();
            list.Add(image);

            linkBack.NavigateUrl = "ShowBlog.aspx";
        }

        if (list.Count > 0)
        {
            PostImage = list[0];

            linkComment.NavigateUrl = string.Format("EditImageComment.aspx?sid={0}&rid={1}",
                PostImage.Image.Id, GetId("pid"));
        }

        GetDataComments(sender, e);

        images.DataSource = list;
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
