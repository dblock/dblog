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

public partial class ShowUrl : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string url = (string) Request.QueryString["Url"];                
                linkRedirect.NavigateUrl = url;

                switch((string) Request.QueryString["ObjectType"])
                {
                    case "Post":
                        SessionManager.BlogService.IncrementPostCounter(
                            SessionManager.Ticket, int.Parse(Request.QueryString["ObjectId"]));
                        break;
                }

                Response.Redirect(url, true);
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
