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
    public class ReferrerHostRollupTest : NHibernateCrudTest
    {
        private ReferrerHostRollup mReferrerHostRollup = null;

        public ReferrerHostRollupTest()
        {
            mReferrerHostRollup = new ReferrerHostRollup();
            mReferrerHostRollup.Name = Guid.NewGuid().ToString();
            mReferrerHostRollup.Rollup = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mReferrerHostRollup;
            }
        }
    }
}
