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

public partial class EditEntry : AdminPage
{
    private TransitEntry mEntry = null;
    private TransitImage mImage = null;

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

    public TransitImage Image
    {
        get
        {
            if (mImage == null)
            {
                mImage = (RequestId > 0 && Entry.ImageId > 0)
                    ? SessionManager.BlogService.GetImageById(SessionManager.Ticket, Entry.ImageId)
                    : new TransitImage();
            }

            return mImage;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultButton(save);

            inputTopic.DataSource = SessionManager.BlogService.GetTopics(SessionManager.Ticket, null);
            inputTopic.DataBind();

            if (RequestId > 0)
            {
                inputTitle.Text = Entry.Title;
                inputText.Text = Entry.Text;
                inputImage.PostedFile = new UploadControl.HttpPostedFile(Image.Name);
                inputTopic.Items.FindByValue(Entry.TopicId.ToString()).Selected = true;
            }
            else
            {
                inputTopic.Items.Insert(0, new ListItem(string.Empty, "0"));
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

            if (inputImage.HasData)
            {
                Image.Data = inputImage.PostedFile.Data;
                Image.Name = Path.GetFileName(inputImage.PostedFile.FileName);
            }

            if (inputImage.HasNewData)
            {
                SessionManager.BlogService.CreateOrUpdateEntryWithImage(SessionManager.Ticket, Entry, inputImage.HasData ? Image : null);
            }
            else
            {
                SessionManager.BlogService.CreateOrUpdateEntry(SessionManager.Ticket, Entry);
            }

            Response.Redirect("ManageEntries.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
