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

public partial class SiteMapBlog : BlogPage
{
    protected override bool AutomaticTitle
    {
        get
        {
            return false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (SessionManager.CountersEnabled)
                {
                    SessionManager.BlogService.IncrementNamedCounter(
                        SessionManager.Ticket, "SiteMap", 1);
                }

                TransitPostQueryOptions options = new TransitPostQueryOptions(0, string.Empty);
                options.PageNumber = 0;
                options.PageSize = 100;
                options.SortDirection = WebServiceQuerySortDirection.Descending;
                options.SortExpression = "Created";
                options.PublishedOnly = true;
                options.DisplayedOnly = true;

                repeater.DataSource = SessionManager.GetCachedCollection<TransitPost>(
                    "GetPosts", SessionManager.PostTicket, options);
                repeater.DataBind();
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 400;
            Response.Status = ex.Message;
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {
        Response.ContentType = "text/xml";
        base.Render(writer);
    }
}