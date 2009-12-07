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
using System.IO;
using System.Xml;
using Argotic.Extensions.Core;
using Argotic.Syndication;
using System.Collections.Generic;
using System.Threading;

public partial class AtomBlog : BlogPage
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
        int topic_id = GetId("topicid");

        string title = SessionManager.GetSetting("title", "Untitled");

        if (topic_id > 0)
        {
            TransitTopic topic = SessionManager.GetCachedObject<TransitTopic>(
                "GetTopicById", SessionManager.Ticket, topic_id);

            title = string.Format("{0}: {1}", title, Renderer.Render(topic.Name));
        }

        return title;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SessionManager.BlogService.IncrementNamedCounter(
                    SessionManager.Ticket, "Atom", 1);

                GetPosts(sender, e);
            }
        }
        catch (ManagedLogin.AccessDeniedException)
        {
            Response.StatusCode = 401;
            Response.StatusDescription = "Access Denied";
            Response.AddHeader("WWW-Authenticate", string.Format("BASIC Realm={0}", SessionManager.BasicAuthRealm));
        }
        catch (ThreadAbortException)
        {

        }
        catch (Exception ex)
        {
            Response.StatusCode = 400;
            Response.StatusDescription = ex.Message;
        }
    }

    private void GetPosts(object sender, EventArgs e)
    {
        TransitPostQueryOptions options = new TransitPostQueryOptions(
            GetId("topicid"), string.Empty);
        options.PageNumber = 0;
        options.PageSize = 25;
        options.SortDirection = WebServiceQuerySortDirection.Descending;
        options.SortExpression = "Created";
        options.PublishedOnly = true;
        options.DisplayedOnly = true;

        repeater.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            "GetPosts", SessionManager.PostTicket, options);
        repeater.DataBind();
    }

    public string GetCategories(TransitTopic[] topics)
    {
        StringBuilder sb = new StringBuilder();
        foreach (TransitTopic topic in topics)
        {
            sb.AppendFormat("<category term=\"{0}\" />",
                Renderer.Render(topic.Name));
        }
        return sb.ToString();
    }

    protected override void Render(HtmlTextWriter writer)
    {
        Response.ContentType = "text/xml";
        base.Render(writer);
    }
}