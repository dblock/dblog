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

public partial class AtomBlog : BlogPage
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
                    SessionManager.Ticket, "Atom", 1);

                repeater.DataSource = SessionManager.GetCachedCollection<TransitPost>(
                    "GetPosts", SessionManager.PostTicket, new TransitPostQueryOptions(25, 0));
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