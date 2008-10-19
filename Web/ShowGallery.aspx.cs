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

public partial class ShowGallery : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                TransitPermalink permalink = SessionManager.GetCachedObject<TransitPermalink>(
                    "GetPermalinkBySource", SessionManager.Ticket, new TransitPermalinkQueryOptions(RequestId, "Gallery"));

                linkRedirect.NavigateUrl = string.Format(
                    "Show{0}.aspx?id={1}", permalink.TargetType, permalink.TargetId);

                Response.StatusCode = 301;
                Response.RedirectLocation = linkRedirect.NavigateUrl;
                Response.End();
            }
        }
        catch (Exception ex)
        {
            panelRedirect.Visible = false;
            ReportException(new Exception(string.Format(
                "This permalink may not exist. The following error was returned." + 
                "<br>{0}", ex.Message)));
        }
    }
}
