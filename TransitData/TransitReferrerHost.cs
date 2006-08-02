using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Tools.Web;
using NHibernate.Expression;

namespace DBlog.TransitData
{
    public class TransitReferrerHost : TransitObject
    {
        private string mLastUrl;

        public string LastUrl
        {
            get
            {
                return mLastUrl;
            }
            set
            {
                mLastUrl = value;
            }
        }

        private string mLastSource;

        public string LastSource
        {
            get
            {
                return mLastSource;
            }
            set
            {
                mLastSource = value;
            }
        }

        private long mRequestCount;

        public long RequestCount
        {
            get
            {
                return mRequestCount;
            }
            set
            {
                mRequestCount = value;
            }
        }

        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        public TransitReferrerHost()
        {

        }

        public TransitReferrerHost(DBlog.Data.ReferrerHost o)
            : base(o.Id)
        {
            LastUrl = o.LastUrl;
            LastSource = o.LastSource;
            Name = o.Name;
            RequestCount = o.RequestCount;
        }

        public ReferrerHost GetReferrerHost(ISession session)
        {
            ReferrerHost rh = null;

            if (Id == 0)
            {
                rh = (ReferrerHost)session.CreateCriteria(typeof(ReferrerHost))
                    .Add(Expression.Eq("Name", Name))
                    .UniqueResult();

                if (rh == null)
                {
                    // find whether this is a dup host
                    ReferrerHostRollup duphost = (ReferrerHostRollup)
                        session.CreateSQLQuery(
                            "SELECT {R.*} FROM ReferrerHostRollup {R}" +
                            " WHERE '" + Renderer.SqlEncode(Name) + "' LIKE Name",
                            "R",
                            typeof(ReferrerHostRollup)).SetMaxResults(1).UniqueResult();

                    if (duphost != null)
                    {
                        rh = (ReferrerHost) session.CreateCriteria(typeof(ReferrerHost))
                            .Add(Expression.Eq("Name", duphost.Rollup))
                            .UniqueResult();
                    }
                }

                if (rh != null)
                {
                    // found an existing host
                    rh.RequestCount += RequestCount;
                }
                else
                {
                    rh = new ReferrerHost();
                    rh.RequestCount = RequestCount;
                }
            }
            else
            {
                rh = (ReferrerHost)session.Load(typeof(ReferrerHost), Id);
                rh.RequestCount = RequestCount;
            }

            rh.LastUrl = LastUrl;
            rh.LastSource = LastSource;
            rh.Name = Name;

            return rh;
        }
    }
}
