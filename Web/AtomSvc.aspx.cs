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
using System.Text.RegularExpressions;
using DBlog.TransitData;
using DBlog.Tools.Web;
using System.Text;
using DBlog.Data.Hibernate;

public partial class AtomSvc : BlogPage
{
    protected override bool AutomaticTitle
    {
        get
        {
            return false;
        }
    }

    public string GetRssTitle()
    {
        return SessionManager.GetSetting("title", "Untitled");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SessionManager.BlogService.IncrementNamedCounter(
                    SessionManager.Ticket, "AtomSvc", 1);
                GetTopics(sender, e);
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 400;
            Response.StatusDescription = ex.Message;
        }
    }

    private void GetTopics(object sender, EventArgs e)
    {
        categories.DataSource = SessionManager.GetCachedCollection<TransitTopic>(
            "GetTopics", SessionManager.Ticket, null);
        categories.DataBind();
    }

    protected override void Render(HtmlTextWriter writer)
    {
        Response.ContentType = "application/atomsvc+xml";
        base.Render(writer);
    }
}
