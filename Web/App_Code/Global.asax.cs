using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Diagnostics;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data;
using DBlog.Data.Hibernate;
using DBlog.WebServices;
using DBlog.TransitData;
using System.Configuration;

public class Global : DBlog.Tools.Web.HostedApplication
{
    private SystemFeedUpdateService mFeedUpdateService = new SystemFeedUpdateService();

    public Global()
    {

    }

    protected override void Application_Start(Object sender, EventArgs e)
    {
        HttpContext.Current.Cache.Insert("Pages", DateTime.Now, null,
           System.DateTime.MaxValue, System.TimeSpan.Zero,
           System.Web.Caching.CacheItemPriority.NotRemovable,
           null);

        base.Application_Start(sender, e);

        DBlog.Data.Hibernate.Session.Configuration.AddAssembly("DBlog.Data");
        DBlog.Data.Hibernate.Session.Configuration.AddAssembly("DBlog.Data.Hibernate");

        using (DBlog.Data.Hibernate.Session.OpenConnection(WebService.GetNewConnection()))
        {
            CreateAdministrator();
        }

        DBlog.WebServices.Blog blog = new DBlog.WebServices.Blog();

        if (EventLogEnabled)
        {
            EventLog.WriteEntry(string.Format("Running with back-end web services {0}.", blog.GetVersion()), EventLogEntryType.Information);
        }

        if (ServicesEnabled)
        {
            mFeedUpdateService.Start();
        }
    }

    protected void Session_Start(Object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        DBlog.Data.Hibernate.Session.BeginRequest();

        string path = Request.Path.Substring(Request.ApplicationPath.Length).Trim("/".ToCharArray());

        // rewrite ShowPost.aspx link to a slug
        if (path == "ShowPost.aspx" && ! string.IsNullOrEmpty(Request["id"]))
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(WebService.GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                int id = 0;
                if (int.TryParse(Request["Id"], out id))
                {
                    Post post = session.Load<Post>(id);
                    if (post != null && !string.IsNullOrEmpty(post.Slug))
                    {
                        Response.RedirectPermanent(post.Slug);
                    }
                }
            }
        }
        // rewrite a slug link to a ShowPost.aspx internal url
        else if (path.IndexOf('.') < 0)
        {
            string[] parts = Request.Path.Split('/');
            string slug = parts[parts.Length - 1];
            if (! String.IsNullOrEmpty(slug))
            {
                HttpContext.Current.RewritePath(string.Format("ShowPost.aspx?slug={0}", slug));
            }
        }
    }

    protected void Application_EndRequest(Object sender, EventArgs e)
    {
        DBlog.Data.Hibernate.Session.EndRequest();
        GC.Collect();
    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {

    }

    protected override void Application_Error(Object sender, EventArgs e)
    {
        base.Application_Error(sender, e);
    }

    protected override void Application_End(Object sender, EventArgs e)
    {
        if (ServicesEnabled)
        {
            mFeedUpdateService.Stop();
        }

        base.Application_End(sender, e);
    }

    protected void Session_End(Object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Create an administrator.
    /// </summary>
    private void CreateAdministrator()
    {
        ISession session = DBlog.Data.Hibernate.Session.Current;
        ITransaction t = session.BeginTransaction();

        try
        {
            int adminCount = session.CreateCriteria(typeof(Login))
                .Add(Expression.Eq("Role", TransitLoginRole.Administrator.ToString()))
                .SetProjection(Projections.Count("Id"))
                .UniqueResult<int>();

            if (adminCount == 0)
            {
                Login admin = new Login();
                admin.Name = admin.Username = "Administrator";
                admin.Role = TransitLoginRole.Administrator.ToString();
                admin.Password = ManagedLogin.GetPasswordHash(string.Empty);
                session.Save(admin);
                session.Flush();

                if (EventLogEnabled)
                {
                    EventLog.WriteEntry(string.Format(
                        "Created an Administrator user with id={0}.",
                            admin.Id),
                        EventLogEntryType.Information);
                }
            }

            t.Commit();
        }
        catch
        {
            t.Rollback();
            throw;
        }

        session.Flush();
    }

    public override string GetVaryByCustomString(HttpContext context, string arg)
    {
        if (arg == "Ticket")
        {
            HttpCookie sDBlogAuthCookie = context.Request.Cookies[SessionManager.sDBlogAuthCookieName];
            HttpCookie sDBlogPostCookie = context.Request.Cookies[SessionManager.sDBlogPostCookieName];
            int authLoginId = sDBlogAuthCookie != null ? ManagedLogin.GetLoginId(sDBlogAuthCookie.Value) : 0;
            int postLoginId = sDBlogPostCookie != null ? ManagedLogin.GetLoginId(sDBlogPostCookie.Value) : 0;
            return string.Format("{0}:{1}", authLoginId, postLoginId);
        }

        return base.GetVaryByCustomString(context, arg);
    }
}
