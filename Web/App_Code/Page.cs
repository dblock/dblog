using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.Tools.Web;
using System.Reflection;
using System.Text.RegularExpressions;
using DBlog.WebServices;
using Microsoft.Web.UI;

public class BlogPage : DBlog.Tools.Web.Page
{
    private HtmlMeta mMetaDescription = null;
    protected SessionManager mSessionManager = null;

    protected override void OnLoad(EventArgs e)
    {
        if (Header != null)
        {
            Header.Controls.Add(MetaDescription);
        }

        if (!IsPostBack)
        {
            CounterCache.Increment(Cache, SessionManager);
        }

        base.OnLoad(e);
    }

    public virtual SessionManager SessionManager
    {
        get
        {
            if (mSessionManager == null)
            {
                mSessionManager = new SessionManager(this);
            }
            return mSessionManager;
        }
    }

    public BlogPage()
    {

    }

    public void ReportException(Exception ex)
    {
        if (Master == null) throw ex;
        object notice = Master.FindControl("noticeMenu");
        if (notice == null) throw ex;
        notice.GetType().GetProperty("Exception").SetValue(notice, ex, null);
    }

    public void ReportInfo(string message, bool htmlencode)
    {
        if (Master == null) throw new Exception(message);
        object notice = Master.FindControl("noticeMenu");
        if (notice == null) throw new Exception(message);
        notice.GetType().GetProperty("Info").SetValue(notice, message, null);
        notice.GetType().GetProperty("HtmlEncode").SetValue(notice, htmlencode, null);
    }

    public HtmlMeta MetaDescription
    {
        get
        {
            if (mMetaDescription == null)
            {
                mMetaDescription = new HtmlMeta();
                mMetaDescription.Name = "description";
                mMetaDescription.Content = SessionManager.GetSetting(
                    "description", string.Empty);
            }
            return mMetaDescription;
        }
    }

    public void ReportInfo(string message)
    {
        ReportInfo(message, false);
    }

    public void ReportWarning(string message)
    {
        if (Master == null) throw new Exception(message);
        object notice = Master.FindControl("noticeMenu");
        if (notice == null) throw new Exception(message);
        notice.GetType().GetProperty("Warning").SetValue(notice, message, null);
    }

    public string CheckInput(string name, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(string.Format("Missing {0}", name));
        }

        return value;
    }

    public int CheckInput(string name, int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException(string.Format("Missing {0}", name));
        }

        return value;
    }
}
