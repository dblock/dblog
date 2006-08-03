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

public partial class EditImage : BlogAdminPage
{
    private TransitImage mImage = null;

    public TransitImage Image
    {
        get
        {
            if (mImage == null)
            {
                mImage = (RequestId > 0)
                    ? SessionManager.BlogService.GetImageById(SessionManager.Ticket, RequestId)
                    : new TransitImage();
            }

            return mImage;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            inputImage.FilePosted += new UploadControl.FilePostedEventHandler(inputImage_FilePosted);
            if (!IsPostBack)
            {
                SetDefaultButton(save);

                if (RequestId > 0)
                {
                    inputFileName.Text = Image.Name;
                    inputDescription.Text = Image.Description;
                    inputImage.PostedFile = new UploadControl.HttpPostedFile(Image.Name);
                    image.ImageUrl = string.Format("ShowPicture.aspx?id={0}", RequestId);
                }
                else
                {
                    image.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void inputImage_FilePosted(object sender, UploadControl.HttpPostedFileEventArgs e)
    {
        inputFileName.Text = Path.GetFileName(e.PostedFile.FileName);
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            if (!inputImage.HasData)
            {
                throw new ArgumentException("Missing Image");
            }

            if (inputImage.HasNewData)
            {
                Image.Data = inputImage.PostedFile.Data;
            }

            Image.Name = CheckInput("FileName", inputFileName.Text);
            Image.Description = inputDescription.Text;
            SessionManager.BlogService.CreateOrUpdateImage(SessionManager.Ticket, Image);
            Response.Redirect("ManageImages.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
