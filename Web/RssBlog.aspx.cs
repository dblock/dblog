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

public partial class RssBlog : BlogPage
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
                SessionManager.BlogService.IncrementNamedCounter(
                    SessionManager.Ticket, "Rss", 1);

                TransitPostQueryOptions options = new TransitPostQueryOptions();
                options.PageNumber = 0;
                options.PageSize = 25;
                options.SortDirection = WebServiceQuerySortDirection.Descending;
                options.SortExpression = "Created";

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