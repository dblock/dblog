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
using DBlog.WebServices;

public class MasterPage : System.Web.UI.MasterPage
{
    private SessionManager mSessionManager = null;

    protected SessionManager SessionManager
    {
        get
        {
            if (mSessionManager == null)
            {
                if (Page is BlogPage)
                {
                    mSessionManager = ((BlogPage)Page).SessionManager;
                }
                else
                {
                    mSessionManager = new SessionManager(this);
                }
            }
            return mSessionManager;
        }
    }

    public MasterPage()
    {

    }

    public void ReportException(Exception ex)
    {
        object notice = FindControl("noticeMenu");
        notice.GetType().GetProperty("Exception").SetValue(notice, ex, null);
    }

    public virtual void OnPagePreInit(EventArgs e)
    {

    }
}
