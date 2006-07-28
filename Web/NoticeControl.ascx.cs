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
using System.Web.Services.Protocols;
using DBlog.Tools.Web;
using System.Diagnostics;

public partial class NoticeControl : BlogControl
{
    private string mCssClass = "notice";
    private string mStyle = string.Empty;
    private bool mHtmlEncode = true;
    private string mMessage = string.Empty;
    private NoticeKind mNoticeKind = NoticeKind.Info;

    public enum NoticeKind
    {
        Info,
        Warning,
        Error,
        Question
    };

    public bool HtmlEncode
    {
        get
        {
            return DBlog.Tools.Web.ViewState<bool>.GetViewStateValue(
                EnableViewState, ViewState, "HtmlEncode", mHtmlEncode);
        }
        set
        {
            DBlog.Tools.Web.ViewState<bool>.SetViewStateValue(
                EnableViewState, ViewState, "HtmlEncode", value, ref mHtmlEncode);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string Style
    {
        get
        {
            return DBlog.Tools.Web.ViewState<string>.GetViewStateValue(
                EnableViewState, ViewState, "Style", mStyle);
        }
        set
        {
            DBlog.Tools.Web.ViewState<string>.SetViewStateValue(
                EnableViewState, ViewState, "Style", value, ref mStyle);
        }
    }

    public string CssClass
    {
        get
        {
            return DBlog.Tools.Web.ViewState<string>.GetViewStateValue(
                EnableViewState, ViewState, "CssClass", mCssClass);
        }
        set
        {
            DBlog.Tools.Web.ViewState<string>.SetViewStateValue(
                EnableViewState, ViewState, "CssClass", value, ref mCssClass);
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        tableNotice.Attributes["class"] = string.Format("{0}_{1}", CssClass, Kind.ToString().ToLower());
        labelMessage.Text = HtmlEncode ? Renderer.Render(Message) : Message;
        panelNotice.Visible = ! string.IsNullOrEmpty(Message);
        imageMessage.ImageUrl = string.Format("images/site/{0}.gif", Kind.ToString().ToLower());
        base.OnPreRender(e);
    }

    public Exception Exception
    {
        set
        {
            Kind = NoticeKind.Error;
#if DEBUG
            string message = value.Message;
#else
            string message = value.Message.Split('\n')[0];
            int colon = message.IndexOf(':');
            if (colon >= 0) message = message.Substring(colon + 1);
#endif

            string reportbugurl = string.Format("mailto:{0}?subject={1}&body={2}",
                       SessionManager.GetSetting(
                            "email", "admin@localhost.com"),
                            Renderer.UrlEncode(Request.Url.PathAndQuery),
                            Renderer.UrlEncode(message));

            Message = string.Format("{0}<br>This may be a bug. If you believe you should not be getting this error, " +
                "<a href={1}>click here</a> to report it.", message, reportbugurl);

            HtmlEncode = false;
        }
    }

    protected string Message
    {
        get
        {
            return DBlog.Tools.Web.ViewState<string>.GetViewStateValue(
                EnableViewState, ViewState, "Message", mMessage);
        }
        set
        {
            DBlog.Tools.Web.ViewState<string>.SetViewStateValue(
                EnableViewState, ViewState, "Message", value, ref mMessage);

            panelNotice.Visible = ! string.IsNullOrEmpty(value);
        }
    }

    public NoticeKind Kind
    {
        get
        {
            return DBlog.Tools.Web.ViewState<NoticeKind>.GetViewStateValue(
                EnableViewState, ViewState, "NoticeKind", mNoticeKind);
        }
        set
        {
            DBlog.Tools.Web.ViewState<NoticeKind>.SetViewStateValue(
                EnableViewState, ViewState, "NoticeKind", value, ref mNoticeKind);
        }
    }

    public string Warning
    {
        set
        {
            Kind = NoticeKind.Warning;
            Message = value;
        }
    }

    public string Info
    {
        set
        {
            Kind = NoticeKind.Info;
            Message = value;
        }
    }

    public string Question
    {
        set
        {
            Kind = NoticeKind.Question;
            Message = value;
        }
    }

    public new string Error
    {
        set
        {
            Kind = NoticeKind.Error;
            Message = value;
        }
    }
}
