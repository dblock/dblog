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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            repeater.DataSource = SessionManager.BlogService.GetPosts(
                SessionManager.Ticket, new TransitPostQueryOptions(25, 0));
            repeater.DataBind();
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {
        Response.ContentType = "text/xml";
        base.Render(writer);
    }
}