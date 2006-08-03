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
using DBlog.Tools.WebControls;
using System.IO;

public partial class UploadControl : BlogControl
{
    public class HttpPostedFileEventArgs : EventArgs
    {
        public HttpPostedFile PostedFile = null;

        public HttpPostedFileEventArgs(HttpPostedFile file)
        {
            PostedFile = file;
        }
    }

    public delegate void FilePostedEventHandler(object sender, HttpPostedFileEventArgs e);
    public event FilePostedEventHandler FilePosted;

    [Serializable]
    public class HttpPostedFile
    {
        public int ContentLength = 0;
        public string FileName = string.Empty;
        public string ContentType = string.Empty;
        public byte[] Data = null;

        public HttpPostedFile(System.Web.HttpPostedFile value)
            : this(value.FileName,
                value.ContentLength,
                value.ContentType,
                new BinaryReader(value.InputStream).ReadBytes(value.ContentLength))
        {

        }

        public HttpPostedFile(string filename)
            : this(filename, 0, string.Empty, null)
        {

        }

        public HttpPostedFile(string filename, int contentlength, string contenttype, byte[] data)
        {
            ContentLength = contentlength;
            FileName = filename;
            ContentType = contenttype;
            Data = data;
        }
    }

    private HttpPostedFile mPostedFile = null;
    private bool mHasNewData = false;

    public string CssClass
    {
        get
        {
            return inputUpload.CssClass;
        }
        set
        {
            inputUpload.CssClass = value;
        }
    }

    public HttpPostedFile PostedFile
    {
        get
        {
            return DBlog.Tools.Web.ViewState<HttpPostedFile>.GetViewStateValue(
                ViewState, string.Format("{0}:PostedFile", ID), mPostedFile);
        }
        set
        {
            DBlog.Tools.Web.ViewState<HttpPostedFile>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:PostedFile", ID), value, ref mPostedFile);
        }
    }

    public bool HasNewData
    {
        get
        {
            return DBlog.Tools.Web.ViewState<bool>.GetViewStateValue(
                ViewState, string.Format("{0}:HasNewData", ID), mHasNewData);
        }
        set
        {
            DBlog.Tools.Web.ViewState<bool>.SetViewStateValue(
                EnableViewState, ViewState, string.Format("{0}:HasNewData", ID), value, ref mHasNewData);
        }
    }


    public bool HasData
    {
        get
        {
            return PostedFile != null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (IsPostBack && inputUpload.HasFile)
            {
                upload_Click(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void upload_Click(object sender, EventArgs e)
    {
        try
        {
            HasNewData = true;
            if (!inputUpload.HasFile)
            {
                PostedFile = null;
            }
            else
            {
                PostedFile = new HttpPostedFile(inputUpload.PostedFile);
            }

            if (FilePosted != null)
            {
                FilePosted(sender, new HttpPostedFileEventArgs(PostedFile));
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        labelData.Text = HasData ? string.Format("{0}{1}", Path.GetFileName(PostedFile.FileName),
            PostedFile.ContentLength > 0 ? string.Format(" ({0} bytes)", PostedFile.ContentLength) : string.Empty)
            : string.Empty;
        base.OnPreRender(e);
    }
}
