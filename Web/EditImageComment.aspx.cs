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

public partial class EditImageComment : BlogUserPage
{
    private TransitImageComment mImageComment = null;

    public TransitImageComment ImageComment
    {
        get
        {
            if (mImageComment == null)
            {
                mImageComment = (RequestId > 0)
                    ? SessionManager.BlogService.GetImageCommentById(SessionManager.Ticket, RequestId)
                    : new TransitImageComment();
            }

            return mImageComment;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetDefaultButton(save);
                linkCancel.NavigateUrl = string.Format("ShowImage.aspx?id={0}&pid={1}", 
                    GetId("sid"), GetId("rid"));

                if (RequestId > 0)
                {
                    inputText.Text = ImageComment.CommentText;
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            TransitComment t_comment = new TransitComment();
            t_comment.Id = RequestId;
            t_comment.Text = CheckInput("Comment", inputText.Text);
            t_comment.IpAddress = Request.ServerVariables["REMOTE_ADDR"];
            t_comment.ParentCommentId = GetId("pid");
            ImageComment.Id = SessionManager.BlogService.CreateOrUpdateImageComment(
                SessionManager.Ticket, GetId("sid"), t_comment);
            Response.Redirect(string.Format("ShowImage.aspx?id={0}&pid={1}", 
                GetId("sid"), GetId("rid")));
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    protected override bool Index
    {
        get
        {
            return false;
        }
    }
}
