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
using Microsoft.Web.UI;

public class Control : System.Web.UI.UserControl
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
                    mSessionManager = ((Page)Page).SessionManager;
                }
                else
                {
                    mSessionManager = new SessionManager(Page);
                }
            }
            return mSessionManager;
        }
    }

    public Control()
    {

    }

    public void ReportException(Exception ex)
    {
        object notice = Page.Master.FindControl("noticeMenu");
        notice.GetType().GetProperty("Exception").SetValue(notice, ex, null);
    }
}
