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

public class Global : DBlog.Tools.Web.HostedApplication
{
    public Global()
    {

    }

    protected override void Application_Start(Object sender, EventArgs e)
    {
        base.Application_Start(sender, e);

        DBlog.Data.Hibernate.Session.Configuration.AddAssembly("DBlog.Data");

        using (DBlog.Data.Hibernate.Session.OpenConnection(WebService.GetNewConnection()))
        {
            CreateAdministrator();
        }

        DBlog.WebServices.Blog blog = new DBlog.WebServices.Blog();
        EventLog.WriteEntry(string.Format("Running with back-end web services {0}.", blog.GetVersion()), EventLogEntryType.Information);
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
        session.Flush();
    }
}
