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
using NHibernate.Criterion;
using System.Collections;
using System.Threading;
using System.Net.Mail;
using System.Diagnostics;
using System.Text;
using DBlog.Data;
using DBlog.Data.Hibernate;
using DBlog.TransitData;
using System.Collections.Generic;
using DBlog.Tools.Web;

public class SystemFeedUpdateService : DBlog.Data.Hibernate.HibernateService
{
    public SystemFeedUpdateService()
    {

    }

    public override void Run(ISession session)
    {
        IList<Feed> feeds = session.CreateCriteria(typeof(Feed)).List<Feed>();

        foreach (Feed feed in feeds)
        {
            try
            {
                ManagedFeed.Update(feed, session, true);
                session.Flush();
            }
            catch (Exception ex)
            {
                EventLogWriteEntry(
                    string.Format("Exception in FeedUpdateService with feed {0}: {1}\n{2}",
                        feed.Id, ex.Message, feed.Url), EventLogEntryType.Error);
            }
        }
    }
}
