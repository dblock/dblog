using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitReferrerSearchQuery : TransitObject
    {
        private string mSearchQuery;

        public string SearchQuery
        {
            get
            {
                return mSearchQuery;
            }
            set
            {
                mSearchQuery = value;
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

        public TransitReferrerSearchQuery()
        {

        }

        public TransitReferrerSearchQuery(DBlog.Data.ReferrerSearchQuery o)
            : base(o.Id)
        {
            SearchQuery = o.SearchQuery;
            RequestCount = o.RequestCount;
        }

        public ReferrerSearchQuery GetReferrerSearchQuery(ISession session)
        {
            ReferrerSearchQuery referrersearchquery = (Id != 0) ? (ReferrerSearchQuery)session.Load(typeof(ReferrerSearchQuery), Id) : new ReferrerSearchQuery();
            referrersearchquery.SearchQuery = SearchQuery;
            referrersearchquery.RequestCount = RequestCount;
            return referrersearchquery;
        }
    }
}
