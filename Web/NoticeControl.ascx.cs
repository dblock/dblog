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
using System.Text;

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
        linkCollapseExpand.Attributes["onclick"] = string.Format("CollapseExpandDetail('{0}')", divDetail.ClientID);
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
        imageMessage.ImageUrl = string.Format("images/site/{0}.gif", Kind.ToString().ToLower());
        base.OnPreRender(e);
    }

    public Exception Exception
    {
        set
        {
            Kind = NoticeKind.Error;
            HtmlEncode = false;

            string detail = value.Message;
            string message = value.Message.Split('\n')[0];

            Exception ex_detail = value.InnerException;
            while (ex_detail != null)
            {
                detail = detail + "\n" + ex_detail.Message;
                ex_detail = ex_detail.InnerException;
            }
            
            string reportbugurl = string.Format("mailto:{0}?subject={1}&body={2}",
                       SessionManager.GetSetting(
                            "email", "admin@localhost.com"),
                            Renderer.UrlEncode(Request.Url.PathAndQuery),
                            Renderer.UrlEncode(message));

            Message = string.Format("{0}<br><small>This may be a bug. If you believe you should not be getting this error, " +
                "please <a href={1}>click here</a> to report it.</small>", message, reportbugurl);

            Detail = detail;

            StringBuilder s = new StringBuilder();
            s.AppendFormat("User-raised exception from {0}: {1}\n{2}", value.Source, value.Message, value.StackTrace);
            if (Request != null && !string.IsNullOrEmpty(Request.RawUrl)) s.AppendFormat("\nUrl: {0}", Request.RawUrl);
            if (Request != null && Request.UrlReferrer != null) s.AppendFormat("\nReferrer: {0}", Request.UrlReferrer);
            if (Request != null && !string.IsNullOrEmpty(Request.UserAgent)) s.AppendFormat("\nUser-agent: {0}", Request.UserAgent);

            SessionManager.EventLog.WriteEntry(s.ToString(), EventLogEntryType.Warning);

            Exception ex_eventlog = value.InnerException;
            while (ex_eventlog != null)
            {
                SessionManager.EventLog.WriteEntry(string.Format("User-raised inner-exception from {0}: {1}\n{2}",
                    ex_eventlog.Source, ex_eventlog.Message, ex_eventlog.StackTrace),
                    EventLogEntryType.Warning);

                ex_eventlog = ex_eventlog.InnerException;
            }
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
            labelMessage.Text = HtmlEncode ? Renderer.Render(Message) : Message;
        }
    }

    protected string Detail
    {
        get
        {
            return DBlog.Tools.Web.ViewState<string>.GetViewStateValue(
                EnableViewState, ViewState, "Detail", mMessage);
        }
        set
        {
            DBlog.Tools.Web.ViewState<string>.SetViewStateValue(
                EnableViewState, ViewState, "Detail", value, ref mMessage);

            panelNotice.Visible = !string.IsNullOrEmpty(value);
            labelDetail.Text = HtmlEncode ? Renderer.Render(Detail) : Detail;
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

            panelNotice.CssClass = string.Format("{0}_{1}", CssClass, value.ToString().ToLower());
            imageMessage.ImageUrl = string.Format("images/site/{0}.gif", value.ToString().ToLower());
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
