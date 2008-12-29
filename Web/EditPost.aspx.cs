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
using DBlog.Tools.Web;

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
                    ? SessionManager.GetCachedObject<TransitPost>("GetPostById", SessionManager.Ticket, PostId)
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
            logins.OnGetDataSource += new EventHandler(logins_OnGetDataSource);
            this.addFile.Attributes["onclick"] = inputImages.GetAddFileScriptReference() + "return false;";

            if (!IsPostBack)
            {
                PostId = RequestId;

                SetDefaultButton(save);
                PageManager.SetDefaultButton(loginAdd, panelLogins.Controls);

                inputTopic.DataSource = SessionManager.GetCachedCollection<TransitTopic>(
                    "GetTopics", SessionManager.Ticket, null);
                inputTopic.DataBind();

                if (PostId > 0)
                {
                    GetDataImages(sender, e);
                    GetDataLogins(sender, e);
                    inputTitle.Text = Post.Title;
                    inputBody.Text = Post.RawBody;
                    inputTopic.Items.FindByValue(Post.TopicId.ToString()).Selected = true;
                    inputCreatedDate.SelectedDate = SessionManager.Region.UtcToUser(Post.Created).Date;
                    inputCreatedTime.SelectedTime = SessionManager.Region.UtcToUser(Post.Created).TimeOfDay;
                    inputPublish.Checked = Post.Publish;
                    inputDisplay.Checked = Post.Display;
                    inputSticky.Checked = Post.Sticky;
                }
                else
                {
                    images.Visible = false;
                    logins.Visible = false;
                    DateTime utcnow = DateTime.UtcNow;
                    inputCreatedDate.SelectedDate = SessionManager.Region.UtcToUser(utcnow).Date;
                    inputCreatedTime.SelectedTime = SessionManager.Region.UtcToUser(utcnow).TimeOfDay;
                    inputTopic.Items.Insert(0, new ListItem(string.Empty, "0"));
                    inputPublish.Checked = true;
                    inputDisplay.Checked = true;
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void logins_OnGetDataSource(object sender, EventArgs e)
    {
        logins.DataSource = SessionManager.GetCachedCollection<TransitPostLogin>(
            "GetPostLogins", SessionManager.Ticket, new TransitPostLoginQueryOptions(
                PostId, images.PageSize, images.CurrentPageIndex));
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
                SessionManager.Invalidate<TransitPostImage>();
                SessionManager.Invalidate<TransitPost>();
                images.Visible = true;
                GetDataImages(sender, e);
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
            Post.Publish = inputPublish.Checked;
            Post.Display = inputDisplay.Checked;
            Post.Sticky = inputSticky.Checked;
            Post.Created = SessionManager.Region.UserToUtc(inputCreatedDate.SelectedDate.Add(inputCreatedTime.SelectedTime));
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

                List<TransitPostImage> deleted = SessionManager.GetCachedCollection<TransitPostImage>(
                    "GetPostImages", SessionManager.Ticket, new TransitPostImageQueryOptions(Post.Id));

                List<TransitPostImage> updated = new List<TransitPostImage>();

                foreach (string filename in filenames)
                {

                    TransitImage image = new TransitImage();
                    image.Name = Path.GetFileName(filename);
                    image.Path = inputServerPath.Text;

                    for (int i = 0; i < deleted.Count; i++)
                    {
                        if (deleted[i].Image.Name == image.Name)
                        {
                            image = deleted[i].Image;                           
                            deleted.RemoveAt(i);
                            break;
                        }
                    }

                    ThumbnailBitmap bitmap = new ThumbnailBitmap(filename);
                    image.Thumbnail = bitmap.Thumbnail;
                    
                    SessionManager.BlogService.CreateOrUpdatePostImage(
                        SessionManager.Ticket, PostId, image);
                }

                foreach (TransitPostImage dimage in deleted)
                {
                    SessionManager.BlogService.DeletePostImage(
                        SessionManager.Ticket, dimage.Id);
                }

                SessionManager.Invalidate<TransitPostImage>();

                images.Visible = true;
                GetDataImages(sender, e);
            }

            if (! string.IsNullOrEmpty(inputLogin.Text))
            {
                loginAdd_Click(sender, e);
            }

            SessionManager.Invalidate<TransitPost>();
            ReportInfo("Post Saved");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void images_ItemCommand(object source, DataListCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeletePostImage(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitPost>();
                    SessionManager.Invalidate<TransitPostImage>();
                    ReportInfo("Item Deleted");
                    GetDataImages(source, e);
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void login_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Remove":
                    SessionManager.BlogService.DeletePostLogin(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitLogin>();
                    ReportInfo("Login Removed");
                    GetDataLogins(source, e);
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }


    void images_OnGetDataSource(object sender, EventArgs e)
    {
        List<TransitPostImage> tp_images = SessionManager.GetCachedCollection<TransitPostImage>(
            "GetPostImages", SessionManager.Ticket, new TransitPostImageQueryOptions(
                PostId, images.PageSize, images.CurrentPageIndex));

        images.DataSource = tp_images;

        if (!IsPostBack && tp_images != null && tp_images.Count > 0)
        {
            inputServerPath.Text = tp_images[0].Image.Path;
        }
    }

    public void GetDataImages(object sender, EventArgs e)
    {
        images.CurrentPageIndex = 0;
        images.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPostImage>(
            "GetPostImagesCount", SessionManager.Ticket, new TransitPostImageQueryOptions(PostId));
        images.Visible = (images.VirtualItemCount > 0);
        images_OnGetDataSource(sender, e);
        images.DataBind();
    }

    public void GetDataLogins(object sender, EventArgs e)
    {
        logins.CurrentPageIndex = 0;
        logins.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPostLogin>(
            "GetPostLoginsCount", SessionManager.Ticket, new TransitPostLoginQueryOptions(PostId));
        logins.Visible = (logins.VirtualItemCount > 0);
        logins_OnGetDataSource(sender, e);
        logins.DataBind();
    }

    public void loginAdd_Click(object sender, EventArgs e)
    {
        try
        {
            TransitLogin t_login = SessionManager.BlogService.GetLoginByUsername(
                SessionManager.Ticket, inputLogin.Text);
            SessionManager.BlogService.CreateOrUpdatePostLogin(
                SessionManager.Ticket, PostId, t_login);
            SessionManager.Invalidate<TransitPostLogin>();
            GetDataLogins(sender, e);
            ReportInfo(string.Format("Added {0}", inputLogin.Text));
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
