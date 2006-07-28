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

public partial class EditPost : BlogAdminPage
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
        grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);
        this.addFile.Attributes["onclick"] = inputImages.GetAddFileScriptReference() + "return false;";

        if (!IsPostBack)
        {
            SetDefaultButton(save);

            inputTopic.DataSource = SessionManager.BlogService.GetTopics(SessionManager.Ticket, null);
            inputTopic.DataBind();

            if (RequestId > 0)
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

    public void inputImages_FilesPosted(object sender, DBlog.Tools.WebControls.FilesPostedEventArgs e)
    {
        if (Post.Id > 0)
        {
            foreach (HttpPostedFile file in e.PostedFiles)
            {
                TransitImage image = new TransitImage();
                image.Data = new BinaryReader(file.InputStream).ReadBytes(file.ContentLength);
                image.Name = Path.GetFileName(file.FileName);
                SessionManager.BlogService.CreateOrUpdatePostImage(SessionManager.Ticket, Post.Id, image);
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
            Post.Id = SessionManager.BlogService.CreateOrUpdatePost(SessionManager.Ticket, Post);
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
            SessionManager.Ticket, new TransitPostImageQueryOptions(Post.Id, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetPostImagesCount(
            SessionManager.Ticket, new TransitPostImageQueryOptions(Post.Id));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
