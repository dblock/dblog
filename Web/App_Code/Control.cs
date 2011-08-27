using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.WebServices;

public class BlogControl : System.Web.UI.UserControl
{
    private SessionManager mSessionManager = null;

    public SessionManager SessionManager
    {
        get
        {
            if (mSessionManager == null)
            {
                if (Page is Page)
                {
                    mSessionManager = ((BlogPage)Page).SessionManager;
                }
                else
                {
                    mSessionManager = new SessionManager(Page);
                }
            }
            return mSessionManager;
        }
    }

    public BlogControl()
    {

    }

    public void ReportException(Exception ex)
    {
        object notice = Page.Master.FindControl("noticeMenu");
        notice.GetType().GetProperty("Exception").SetValue(notice, ex, null);
    }

    public string UrlPathAndQuery
    {
        get
        {
            string[] pathWithQuery = Request.Url.PathAndQuery.Split("?".ToCharArray());
            pathWithQuery[0] = VirtualPathUtility.ToAppRelative(pathWithQuery[0]).Replace("~/", "");
            return string.Join("?", pathWithQuery);
        }
    }
}
