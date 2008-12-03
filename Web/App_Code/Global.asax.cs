using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Diagnostics;
using NHibernate;
using NHibernate.Expression;
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

    public bool ServicesEnabled
    {
        get
        {
            bool b_result = true;
            
            object result = ConfigurationManager.AppSettings["Services.Enabled"];

            if (result != null)
            {
                bool.TryParse(result.ToString(), out b_result);
            }

            return b_result;
        }
    }

    protected override void Application_Start(Object sender, EventArgs e)
    {
        base.Application_Start(sender, e);

        DBlog.Data.Hibernate.Session.Configuration.AddAssembly("DBlog.Data");
        DBlog.Data.Hibernate.Session.Configuration.AddAssembly("DBlog.Data.Hibernate");

        using (DBlog.Data.Hibernate.Session.OpenConnection(WebService.GetNewConnection()))
        {
            CreateAdministrator();
        }

        DBlog.WebServices.Blog blog = new DBlog.WebServices.Blog();
        EventLog.WriteEntry(string.Format("Running with back-end web services {0}.", blog.GetVersion()), EventLogEntryType.Information);

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
    }

    protected void Application_EndRequest(Object sender, EventArgs e)
    {
        DBlog.Data.Hibernate.Session.EndRequest();
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
                EventLog.WriteEntry(string.Format(
                    "Created an Administrator user with id={0}.", 
                        admin.Id),
                    EventLogEntryType.Information);
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
}
