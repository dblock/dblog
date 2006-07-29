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

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
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
        panelComments.Update();
        comments.DataSource = SessionManager.BlogService.GetImageComments(
            SessionManager.Ticket, new TransitImageCommentQueryOptions(
                PostImage.ImageId, comments.AllowPaging ? comments.PageSize : 0, 
                comments.AllowPaging ? comments.CurrentPageIndex : 0));
    }

    public void GetDataImages(object sender, EventArgs e)
    {
        images.CurrentPageIndex = 0;

        if (RequestId > 0 && ! IsPostBack)
        {            
            // HACK: navigate images to find the one we're looking for
            List<TransitPostImage> list = SessionManager.BlogService.GetPostImages(SessionManager.Ticket,
                new TransitPostImageQueryOptions(GetId("pid")));

            int index = 0;
            foreach (TransitPostImage img in list)
            {
                if (img.ImageId == RequestId)
                {
                    images.CurrentPageIndex = index;
                    break;
                }

                index++;
            }
        }

        images.VirtualItemCount = SessionManager.BlogService.GetPostImagesCount(
            SessionManager.Ticket, new TransitPostImageQueryOptions(GetId("pid")));
        images_OnGetDataSource(sender, e);
        images.DataBind();
        panelImages.Update();
    }

    public void GetDataComments(object sender, EventArgs e)
    {
        labelName.Text = PostImage.ImageName;

        panelPicture.Update();

        comments.Visible = (PostImage.ImageCommentsCount > 0);
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.BlogService.GetImageCommentsCount(
            SessionManager.Ticket, new TransitImageCommentQueryOptions(PostImage.ImageId));
        comments_OnGetDataSource(sender, e);
        comments.DataBind();

        panelComments.Update();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        List<TransitPostImage> list = SessionManager.BlogService.GetPostImages(
            SessionManager.Ticket, new TransitPostImageQueryOptions(
                GetId("pid"), images.PageSize, images.CurrentPageIndex));

        if (list.Count > 0)
        {
            PostImage = list[0];

            linkComment.NavigateUrl = string.Format("EditImageComment.aspx?sid={0}&rid={1}",
                PostImage.ImageId, GetId("pid"));

            linkBack.NavigateUrl = string.Format("ShowPost.aspx?id={0}", GetId("pid"));
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