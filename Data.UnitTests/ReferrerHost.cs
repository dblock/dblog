using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class ReferrerHostTest : NHibernateCrudTest
    {
        private ReferrerHost mReferrerHost = null;

        public ReferrerHostTest()
        {
            mReferrerHost = new ReferrerHost();
            mReferrerHost.LastSource = Guid.NewGuid().ToString();
            mReferrerHost.LastUrl = Guid.NewGuid().ToString();
            mReferrerHost.Name = Guid.NewGuid().ToString();
            mReferrerHost.RequestCount = 10;
            mReferrerHost.Created = mReferrerHost.Updated = DateTime.UtcNow;
        }

        public override object Object
        {
            get 
            {
                return mReferrerHost;
            }
        }
    }
}
