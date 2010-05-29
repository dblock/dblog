using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;

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
            ReferrerSearchQuery rsq = null;

            if (Id == 0)
            {
                rsq = (ReferrerSearchQuery)session.CreateCriteria(typeof(ReferrerSearchQuery))
                    .Add(Expression.Eq("SearchQuery", SearchQuery))
                    .UniqueResult();

                if (rsq == null)
                {
                    rsq = new ReferrerSearchQuery();
                    rsq.RequestCount = RequestCount;
                }
                else
                {
                    rsq.RequestCount += RequestCount;
                }
            }
            else
            {
                rsq = (ReferrerSearchQuery)session.Load(typeof(ReferrerSearchQuery), Id);
                rsq.RequestCount = RequestCount;
            }

            rsq.SearchQuery = SearchQuery;
            return rsq;
        }
    }
}
