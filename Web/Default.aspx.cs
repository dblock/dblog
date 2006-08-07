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

public partial class _Default : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                linkAtom.Attributes["title"] = string.Format("{0} {1}", 
                    SessionManager.GetSetting("title", "Untitled"),
                    linkAtom.Attributes["title"]);

                linkRss.Attributes["title"] = string.Format("{0} {1}",
                    SessionManager.GetSetting("title", "Untitled"),
                    linkRss.Attributes["title"]);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
