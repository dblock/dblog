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

public partial class AtomImage : BlogPage
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
                        CreateOrUpdateImage(sender, e);
                        break;
                    case "PUT":
                        UpdateImage(sender, e);
                        break;
                    case "GET":
                        GetImage(sender, e);
                        break;
                    case "DELETE":
                        DeleteImage(sender, e);
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

    private AtomEntry GetImage(TransitImage image)
    {
        AtomEntry atomEntry = new AtomEntry();
        atomEntry.Id = new AtomId(new Uri(string.Format("{0}Image/{1}", SessionManager.WebsiteUrl, image.Id)));
        atomEntry.Title = new AtomTextConstruct(image.Name);
        atomEntry.UpdatedOn = DateTime.UtcNow;
        atomEntry.Summary = new AtomTextConstruct();
        atomEntry.Content = new AtomContent("", "image/jpg");
        atomEntry.Content.Source = new Uri(string.Format("{0}ShowPicture.aspx?id={1}&ShowThumbnail=false", SessionManager.WebsiteUrl, image.Id));
        atomEntry.Links.Add(new AtomLink(new Uri(string.Format("{0}AtomImage.aspx?id={1}", SessionManager.WebsiteUrl, image.Id)), "edit"));
        AtomLink atomEntryUri = new AtomLink(new Uri(string.Format("{0}ShowPicture.aspx?id={1}&ShowThumbnail=false", SessionManager.WebsiteUrl, image.Id)), "edit-media");
        atomEntryUri.ContentType = "image/jpg";
        atomEntry.Links.Add(atomEntryUri);
        return atomEntry;
    }

    private void CreateOrUpdateImage(object sender, EventArgs e)
    {
        SessionManager.BasicAuth();

        if (!SessionManager.IsAdministrator)
        {
            throw new ManagedLogin.AccessDeniedException();
        }

        TransitImage image = (RequestId > 0)
            ? SessionManager.BlogService.GetImageById(SessionManager.Ticket, RequestId)
            : new TransitImage();

        image.Name = string.Format("{0}.jpg", Request.Headers["Slug"]);
        image.Data = new byte[Request.InputStream.Length];
        Request.InputStream.Read(image.Data, 0, (int)Request.InputStream.Length);

        image.Id = SessionManager.BlogService.CreateOrUpdateImage(SessionManager.Ticket, image);

        Response.ContentType = "application/atom+xml;type=entry;charset=\"utf-8\"";
        Response.StatusCode = 201;
        Response.StatusDescription = "Created";
        string location = string.Format("{0}AtomImage.aspx?id={1}", SessionManager.WebsiteUrl, image.Id);
        Response.Headers.Add("Location", location);

        AtomEntry atomEntry = GetImage(image);
        atomEntry.Save(Response.OutputStream);
        Response.End();
    }

    private void UpdateImage(object sender, EventArgs e)
    {
        SessionManager.BasicAuth();

        if (!SessionManager.IsAdministrator)
        {
            throw new ManagedLogin.AccessDeniedException();
        }

        TransitImage image = SessionManager.BlogService.GetImageById(SessionManager.Ticket, RequestId);
        image.Data = new byte[Request.InputStream.Length];
        Request.InputStream.Read(image.Data, 0, (int)Request.InputStream.Length);
        SessionManager.BlogService.CreateOrUpdateImage(SessionManager.Ticket, image);

        Response.StatusCode = 200;
        Response.End();
    }

    public void GetImage(object sender, EventArgs e)
    {
        TransitImage image = SessionManager.BlogService.GetImageById(SessionManager.Ticket, RequestId);

        Response.ContentType = "application/atom+xml;type=entry;charset=\"utf-8\"";
        Response.Headers.Add("ETag", string.Format("\"{0}\"", Guid.NewGuid().ToString()));

        AtomEntry atomEntry = GetImage(image);
        atomEntry.Save(Response.OutputStream);
        Response.End();
    }

    public void DeleteImage(object sender, EventArgs e)
    {
        SessionManager.BasicAuth();

        if (!SessionManager.IsAdministrator)
        {
            throw new ManagedLogin.AccessDeniedException();
        }

        SessionManager.BlogService.DeleteImage(
            SessionManager.Ticket, RequestId);

        Response.StatusCode = 200;
        Response.End();
    }
}