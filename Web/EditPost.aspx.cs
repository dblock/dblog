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
using System.Collections.Generic;
using DBlog.TransitData;
using System.IO;
using System.Collections.ObjectModel;
using DBlog.Tools.Drawing;

public partial class EditPost : BlogAdminPage
{
    private TransitPost mPost = null;
    private int mPostId = 0;

    public int PostId
    {
        get
        {
            return DBlog.Tools.Web.ViewState<int>.GetViewStateValue(
                ViewState, string.Format("{0}:PostId", ID), mPostId);
        }
        set
        {
            DBlog.Tools.Web.ViewState<int>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:PostId", ID), value, ref mPostId);
        }
    }

    public TransitPost Post
    {
        get
        {
            if (mPost == null)
            {
                mPost = (PostId > 0)
                    ? SessionManager.BlogService.GetPostById(SessionManager.Ticket, PostId)
                    : new TransitPost();
            }

            return mPost;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);
            this.addFile.Attributes["onclick"] = inputImages.GetAddFileScriptReference() + "return false;";

            if (!IsPostBack)
            {
                PostId = RequestId;

                SetDefaultButton(save);

                inputTopic.DataSource = SessionManager.BlogService.GetTopics(SessionManager.Ticket, null);
                inputTopic.DataBind();

                if (PostId > 0)
                {
                    GetData(sender, e);
                    inputTitle.Text = Post.Title;
                    inputBody.Text = Post.Body;
                    inputTopic.Items.FindByValue(Post.TopicId.ToString()).Selected = true;
                }
                else
                {
                    grid.Visible = false;
                    inputTopic.Items.Insert(0, new ListItem(string.Empty, "0"));
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        linkView.Visible = (PostId > 0);
        linkView.NavigateUrl = string.Format("ShowPost.aspx?id={0}", PostId);
        labelServerPath.Text = string.Format("Under {0}", SessionManager.GetSetting("Images", string.Empty));
        base.OnPreRender(e);
    }

    public void inputImages_FilesPosted(object sender, DBlog.Tools.WebControls.FilesPostedEventArgs e)
    {
        if (PostId > 0)
        {
            foreach (HttpPostedFile file in e.PostedFiles)
            {
                TransitImage image = new TransitImage();
                image.Data = new BinaryReader(file.InputStream).ReadBytes(file.ContentLength);
                image.Name = Path.GetFileName(file.FileName);
                SessionManager.BlogService.CreateOrUpdatePostImage(SessionManager.Ticket, PostId, image);
            }

            if (e.PostedFiles.Count > 0)
            {
                grid.Visible = true;
                GetData(sender, e);
            }
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            Post.Title = CheckInput("Title", inputTitle.Text);
            Post.TopicId = CheckInput("Topic", int.Parse(inputTopic.SelectedValue));
            Post.Body = inputBody.Text;
            Post.Id = PostId = SessionManager.BlogService.CreateOrUpdatePost(
                SessionManager.Ticket, Post);

            if (!string.IsNullOrEmpty(inputServerPath.Text))
            {
                string fullpath = Path.Combine(
                    SessionManager.GetSetting("Images", string.Empty),
                    inputServerPath.Text);

                ArrayList filenames = new ArrayList();
                filenames.AddRange(Directory.GetFiles(fullpath, "*.jpg"));
                filenames.AddRange(Directory.GetFiles(fullpath, "*.gif"));

                foreach (string filename in filenames)
                {
                    ThumbnailBitmap bitmap = new ThumbnailBitmap(filename);

                    TransitImage image = new TransitImage();
                    image.Thumbnail = bitmap.Thumbnail;
                    image.Name = Path.GetFileName(filename);
                    image.Path = inputServerPath.Text;
                    
                    SessionManager.BlogService.CreateOrUpdatePostImage(
                        SessionManager.Ticket, PostId, image);
                }

                grid.Visible = true;
                GetData(sender, e);
            }

            ReportInfo("Item Saved");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void grid_ItemCommand(object source, DataListCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeletePostImage(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
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

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.BlogService.GetPostImages(
            SessionManager.Ticket, new TransitPostImageQueryOptions(
                PostId, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetPostImagesCount(
            SessionManager.Ticket, new TransitPostImageQueryOptions(PostId));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
