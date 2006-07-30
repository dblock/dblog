using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NHibernate;
using NHibernate.Expression;
using System.Collections;
using System.Threading;
using System.Net.Mail;
using System.Diagnostics;
using System.Text;
using DBlog.Data;
using DBlog.Data.Hibernate;
using DBlog.TransitData;

public class SystemFeedUpdateService : DBlog.Data.Hibernate.HibernateService
{
    public SystemFeedUpdateService()
    {

    }

    public override void Run(ISession session)
    {
        IList feeds = session.CreateCriteria(typeof(Feed)).List();

        foreach (Feed feed in feeds)
        {
            try
            {
                ManagedFeed.Update(feed, session, true);
                session.Flush();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(
                    string.Format("Exception in FeedUpdateService with feed {0}: {1}",
                        feed.Id, ex.Message), EventLogEntryType.Error);
            }
        }
    }
}
