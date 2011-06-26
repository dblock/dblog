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

public partial class AtomPost : BlogPage
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
                        SessionManager.Ticket, "Atom", 1);
                }

                switch (Request.HttpMethod)
                {
                    case "POST":
                    case "PUT":
                        CreateOrUpdatePost(sender, e);
                        break;
                    case "GET":
                        if (RequestId > 0)
                        {
                            GetPost(sender, e);
                        }
                        else
                        {
                            GetPosts(sender, e);
                        }
                        break;
                    case "DELETE":
                        DeletePost(sender, e);
                        break;
                    default:
                        throw new NotSupportedException(Request.HttpMethod);
                }
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

    private AtomEntry GetPost(TransitPost post)
    {
        AtomEntry atomEntry = new AtomEntry();
        atomEntry.Title = new AtomTextConstruct(post.Title);
        foreach (TransitTopic topic in post.Topics)
        {
            atomEntry.Categories.Add(new AtomCategory(topic.Name));
        }
        atomEntry.Content = new AtomContent(post.BodyXHTML, "html");
        atomEntry.PublishedOn = post.Created;
        atomEntry.UpdatedOn = post.Modified;
        atomEntry.Id = new AtomId(new Uri(string.Format("{0}Post/{1}", SessionManager.WebsiteUrl, post.Id)));
        atomEntry.Links.Add(new AtomLink(new Uri(string.Format("{0}AtomBlog.aspx?id={1}", SessionManager.WebsiteUrl, post.Id)), "edit"));
        AtomLink atomEntryUri = new AtomLink(new Uri(string.Format("{0}ShowPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id)), "alternate");
        atomEntryUri.ContentType = "text/html";
        atomEntry.Links.Add(atomEntryUri);
        return atomEntry;
    }

    private void GetPost(object sender, EventArgs e)
    {
        Response.ContentType = "application/atom+xml;type=entry;charset=\"utf-8\"";
        TransitPost post = SessionManager.BlogService.GetPostById(SessionManager.Ticket, RequestId);
        AtomEntry atomEntry = GetPost(post);
        atomEntry.Save(Response.OutputStream);
        Response.End();
    }

    private void CreateOrUpdatePost(object sender, EventArgs e)
    {
        SessionManager.BasicAuth();

        if (!SessionManager.IsAdministrator)
        {
            throw new ManagedLogin.AccessDeniedException();
        }

        AtomEntry atomEntry = new AtomEntry();
        atomEntry.Load(Request.InputStream);

        TransitPost post = (RequestId > 0) 
            ? SessionManager.BlogService.GetPostById(SessionManager.Ticket, RequestId) 
            : new TransitPost();

        post.Title = atomEntry.Title.Content;

        List<TransitTopic> topics = new List<TransitTopic>();
        foreach (AtomCategory category in atomEntry.Categories)
        {
            TransitTopic topic = SessionManager.BlogService.GetTopicByName(SessionManager.Ticket, category.Term);
            if (topic == null)
            {
                topic = new TransitTopic();
                topic.Name = category.Term;
            }
            topics.Add(topic);
        }

        post.Topics = topics.ToArray();
        post.Body = atomEntry.Content.Content;
        post.Publish = true;
        post.Display = true;
        post.Sticky = false;
        post.Export = false;
        
        if (atomEntry.PublishedOn != DateTime.MinValue)
            post.Created = atomEntry.PublishedOn;
        if (atomEntry.UpdatedOn != DateTime.MinValue)
            post.Modified = atomEntry.UpdatedOn;
        
        post.Id = SessionManager.BlogService.CreateOrUpdatePost(SessionManager.Ticket, post);

        Response.ContentType = "application/atom+xml;type=entry;charset=\"utf-8\"";
        Response.StatusCode = 201;
        Response.StatusDescription = "Created";
        string location = string.Format("{0}AtomPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id);
        Response.Headers.Add("Location", location);
        Response.Headers.Add("Content-Location", location);
        Response.Headers.Add("ETag", string.Format("\"{0}\"", Guid.NewGuid().ToString()));

        atomEntry.Id = new AtomId(new Uri(string.Format("{0}Post/{1}", SessionManager.WebsiteUrl, post.Id)));
        atomEntry.Links.Add(new AtomLink(new Uri(string.Format("{0}AtomPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id))));
        atomEntry.Links.Add(new AtomLink(new Uri(string.Format("{0}AtomPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id)), "edit"));
        AtomLink atomEntryUri = new AtomLink(new Uri(string.Format("{0}ShowPost.aspx?id={1}", SessionManager.WebsiteUrl, post.Id)), "alternate");
        atomEntryUri.ContentType = "text/html";
        atomEntry.Links.Add(atomEntryUri);
        atomEntry.Save(Response.OutputStream);

        Response.End();
    }

    private void GetPosts(object sender, EventArgs e)
    {
        TransitPostQueryOptions options = new TransitPostQueryOptions();
        options.PageNumber = 0;
        options.PageSize = 25;
        options.SortDirection = WebServiceQuerySortDirection.Descending;
        options.SortExpression = "Created";
        options.PublishedOnly = true;
        options.DisplayedOnly = true;

        Response.ContentType = "application/atom+xml;charset=\"utf-8\"";

        AtomFeed feed = new AtomFeed();
        feed.Title = new AtomTextConstruct(SessionManager.GetSetting("title", "Untitled"));

        List<TransitPost> posts = SessionManager.GetCachedCollection<TransitPost>(
            "GetPosts", SessionManager.PostTicket, options);

        foreach (TransitPost post in posts)
        {
            AtomEntry atomEntry = GetPost(post);
            feed.AddEntry(atomEntry);
        }

        feed.Save(Response.OutputStream);
        Response.End();
    }

    public void DeletePost(object sender, EventArgs e)
    {
        SessionManager.BasicAuth();

        if (!SessionManager.IsAdministrator)
        {
            throw new ManagedLogin.AccessDeniedException();
        }

        SessionManager.BlogService.DeletePost(
            SessionManager.Ticket, RequestId);

        Response.StatusCode = 200;
        Response.End();
    }
}