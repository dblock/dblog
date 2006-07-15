using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class ReferrerSearchQueryTest : NHibernateCrudTest
    {
        private ReferrerSearchQuery mReferrerSearchQuery = null;

        public ReferrerSearchQueryTest()
        {
            mReferrerSearchQuery = new ReferrerSearchQuery();
            mReferrerSearchQuery.RequestCount = 10;
            mReferrerSearchQuery.SearchQuery = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mReferrerSearchQuery;
            }
        }
    }
}
