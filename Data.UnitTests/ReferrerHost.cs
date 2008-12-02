using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
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
