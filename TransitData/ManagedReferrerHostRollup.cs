using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Net;
using System.Web;
using System.IO;
using DBlog.Tools.Web;

namespace DBlog.TransitData
{
    public class ManagedReferrerHostRollup
    {
        protected ReferrerHostRollup mReferrerHostRollup = null;

        public ManagedReferrerHostRollup(ReferrerHostRollup ReferrerHostRollup)
        {
            mReferrerHostRollup = ReferrerHostRollup;
        }

        public void RollupExistingReferrerHosts(ISession session)
        {
            // find a host that is exactly the target
            ReferrerHost targethost = (ReferrerHost) session.CreateCriteria(typeof(ReferrerHost))
                .Add(Expression.Eq("Name", mReferrerHostRollup.Rollup))
                .UniqueResult();

            if (targethost == null)
            {
                targethost = new ReferrerHost();
                targethost.Name = mReferrerHostRollup.Rollup;
                targethost.RequestCount = 0;
                targethost.LastSource = targethost.LastUrl = "http://localhost/";
                targethost.Created = targethost.Updated = DateTime.UtcNow;
                session.Save(targethost);
            }

            IList hosts = session.CreateSQLQuery(
                    "SELECT {R.*} FROM ReferrerHost {R}" +
                    " WHERE Name LIKE '" + Renderer.SqlEncode(mReferrerHostRollup.Name) + "'",
                    "R",
                    typeof(ReferrerHost)).List();

            foreach (ReferrerHost host in hosts)
            {
                if (host != targethost)
                {
                    targethost.LastSource = host.LastSource;
                    targethost.LastUrl = host.LastUrl;
                    targethost.RequestCount += host.RequestCount;
                    session.Delete(host);
                }
            }

            session.Save(targethost);
            session.Flush();
        }
    }
}
