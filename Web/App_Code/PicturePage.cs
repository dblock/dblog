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

public abstract class PicturePage : DBlog.Tools.Web.PicturePage
{
    protected SessionManager mSessionManager = null;

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

    public PicturePage()
    {

    }

    public void ReportException(Exception ex)
    {
        Response.Write(ex.Message);
    }
}
