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

public partial class EditEntry : AdminPage
{
    private TransitEntry mEntry = null;

    public TransitEntry Entry
    {
        get
        {
            if (mEntry == null)
            {
                mEntry = (RequestId > 0)
                    ? SessionManager.BlogService.GetEntryById(SessionManager.Ticket, RequestId)
                    : new TransitEntry();
            }

            return mEntry;
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
                inputTitle.Text = Entry.Title;
                inputText.Text = Entry.Text;
                inputTopic.Items.FindByValue(Entry.TopicId.ToString()).Selected = true;
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
        if (Entry.Id > 0)
        {
            foreach (HttpPostedFile file in e.PostedFiles)
            {
                TransitImage image = new TransitImage();
                image.Data = new BinaryReader(file.InputStream).ReadBytes(file.ContentLength);
                image.Name = Path.GetFileName(file.FileName);
                SessionManager.BlogService.CreateOrUpdateEntryImage(SessionManager.Ticket, Entry.Id, image);
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
            Entry.Title = CheckInput("Title", inputTitle.Text);
            Entry.TopicId = CheckInput("Topic", int.Parse(inputTopic.SelectedValue));
            Entry.Text = inputText.Text;
            Entry.IpAddress = Request.ServerVariables["REMOTE_ADDR"];
            Entry.Id = SessionManager.BlogService.CreateOrUpdateEntry(SessionManager.Ticket, Entry);
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
                    SessionManager.BlogService.DeleteEntryImage(SessionManager.Ticket, int.Parse(e.CommandArgument.ToString()));
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
        grid.DataSource = SessionManager.BlogService.GetEntryImages(
            SessionManager.Ticket, new TransitEntryImageQueryOptions(Entry.Id, grid.PageSize, grid.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid.VirtualItemCount = SessionManager.BlogService.GetEntryImagesCount(
            SessionManager.Ticket, new TransitEntryImageQueryOptions(Entry.Id));
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }
}
