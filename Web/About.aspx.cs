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
using DBlog.WebServices;
using System.Collections.Specialized;
using DBlog.Tools.Web;

public partial class About : BlogPage
{
    public void Page_Load()
    {
        try
        {
            if (!IsPostBack)
            {
                labelTitle.Text = Renderer.Render(SessionManager.GetSetting("Title", "Blog"));
                labelDescription.Text = Renderer.Render(SessionManager.GetSetting("Description", string.Empty));
                labelCopyright.Text = Renderer.Render(SessionManager.GetSetting("Copyright", string.Empty));
                linkEmail.OnClientClick = string.Format("location.href='mailto:{0}';", Renderer.Render(SessionManager.GetSetting("Email", string.Empty)));
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
