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
using System.IO;

public partial class EditHighlight : BlogAdminPage
{
    private TransitHighlight mHighlight = null;
    private TransitImage mHighlightImage = null;

    public TransitHighlight Highlight
    {
        get
        {
            if (mHighlight == null)
            {
                mHighlight = (RequestId > 0)
                    ? SessionManager.BlogService.GetHighlightById(SessionManager.Ticket, RequestId)
                    : new TransitHighlight();
            }

            return mHighlight;
        }
    }

    public TransitImage HighlightImage
    {
        get
        {
            if (mHighlightImage == null)
            {
                mHighlightImage = (RequestId > 0)
                    ? SessionManager.BlogService.GetImageById(SessionManager.Ticket, Highlight.ImageId)
                    : new TransitImage();
            }

            return mHighlightImage;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultButton(save);

            if (RequestId > 0)
            {
                inputTitle.Text = Highlight.Title;
                inputUrl.Text = Highlight.Url;
                inputDescription.Text = Highlight.Description;
                inputImage.PostedFile = new UploadControl.HttpPostedFile(HighlightImage.Name);
            }
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            if (! inputImage.HasData)
            {
                throw new ArgumentException("Missing Image");
            }

            if (inputImage.HasNewData)
            {
                HighlightImage.Data = inputImage.PostedFile.Data;
                HighlightImage.Name = Path.GetFileName(inputImage.PostedFile.FileName);
                Highlight.ImageId = SessionManager.BlogService.CreateOrUpdateImage(SessionManager.Ticket, HighlightImage);
            }

            Highlight.Title = CheckInput("Title", inputTitle.Text);
            Highlight.Url = CheckInput("Url", inputUrl.Text);
            Highlight.Description = inputDescription.Text;
            SessionManager.BlogService.CreateOrUpdateHighlight(SessionManager.Ticket, Highlight);
            Response.Redirect("ManageHighlights.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
