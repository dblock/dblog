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
using DBlog.TransitData.References;
using DBlog.Tools.Web.Html;

public class BlogPage : DBlog.Tools.Web.Page
{
    protected SessionManager mSessionManager = null;
    
    protected virtual bool AutomaticTitle
    {
        get
        {
            return true;
        }
    }

    protected virtual bool Index
    {
        get
        {
            return true;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        if (! Index && Header != null)
        {
            HtmlMeta noindex = new HtmlMeta();
            noindex.Name = "robots";
            noindex.Content = "noindex";
            Header.Controls.Add(noindex);
        }

        if (!IsPostBack)
        {
            if (AutomaticTitle)
            {
                Page.Title = string.Format("{0} - {1}", SessionManager.GetSetting(
                    "title", "Blog"), Page.Title);
            }

            CounterCache.Increment(Request, Cache, SessionManager);
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

    protected override void OnPreInit(EventArgs e)
    {
        if (Master != null && Master is MasterPage)
        {
            ((MasterPage)Master).OnPagePreInit(e);
        }

        base.OnPreInit(e);
    }

    public virtual string RenderEx(string text, int id)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        ReferencesRedirector redirector = new ReferencesRedirector(id, "Post");
        return Renderer.RenderEx(Cutter.DeleteCut(text), ConfigurationManager.AppSettings["url"], redirector.ReferUri);
    }
}
