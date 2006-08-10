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
using System.Text.RegularExpressions;
using DBlog.Tools.Drawing.Exif;
using DBlog.Tools.Drawing;
using System.IO;
using System.Drawing;

public partial class ShowImage : BlogPage
{
    private TransitPostImage mPostImage = null;
    private int mImageId = 0;
    private EXIFMetaData mEXIFMetaData = null;

    public int ImageId
    {
        get
        {
            return DBlog.Tools.Web.ViewState<int>.GetViewStateValue(
                ViewState, string.Format("{0}:ImageId", ID), mImageId);
        }
        set
        {
            DBlog.Tools.Web.ViewState<int>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:ImageId", ID), value, ref mImageId);
        }
    }

    public TransitPostImage PostImage
    {
        get
        {
            return mPostImage;
        }
        set
        {
            mPostImage = value;
            ImageId = mPostImage.Image.Id;
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

    public EXIFMetaData ImageEXIFMetaData
    {
        get
        {
            if (mEXIFMetaData == null)
            {
                TransitImage image = SessionManager.BlogService.GetImageWithBitmapById(
                    SessionManager.PostTicket, ImageId);

                if (image.Data != null)
                {
                    mEXIFMetaData = new EXIFMetaData(new Bitmap(
                        new MemoryStream(image.Data)).PropertyItems);
                }
                else if (image.Data == null && ! string.IsNullOrEmpty(image.Path))
                {
                    mEXIFMetaData = new EXIFMetaData(new Bitmap(Path.Combine(Path.Combine(
                        SessionManager.GetSetting("Images", string.Empty),
                        image.Path), image.Name)).PropertyItems);
                }
            }

            return mEXIFMetaData;
        }
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
        }
        else
        {
            TransitImage image = SessionManager.BlogService.GetImageById(
                SessionManager.PostTicket, RequestId);
            TransitPostImage postimage = new TransitPostImage();
            postimage.Image = image;
            postimage.Post = null;
            postimage.Id = RequestId;
            list = new List<TransitPostImage>();
            list.Add(postimage);
        }

        linkBack.NavigateUrl = ReturnUrl;

        if (list.Count > 0)
        {
            PostImage = list[0];

            linkComment.NavigateUrl = string.Format("EditImageComment.aspx?sid={0}&rid={1}",
                PostImage.Image.Id, GetId("pid"));
        }

        GetEXIFData(sender, e);
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

    public string ReturnUrl
    {
        get
        {
            string result = Request.QueryString["r"];
            int pid = GetId("pid");
            if (string.IsNullOrEmpty(result) && (pid > 0)) return string.Format("ShowPost.aspx?id={0}", pid);
            if (string.IsNullOrEmpty(result)) return "ShowBlog.aspx";
            return result;
        }
    }

    public void GetEXIFData(object sender, EventArgs e)
    {
        if (exif.Visible)
        {
            EXIFMetaData metadata = ImageEXIFMetaData;
            exif.DataSource = (metadata != null) ? metadata.EXIFPropertyItems : null;
            exif.DataBind();
        }

        panelEXIF.Update();
    }

    public void linkEXIF_Click(object sender, EventArgs e)
    {
        try
        {
            exif.Visible = !exif.Visible;
            GetEXIFData(sender, e);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
