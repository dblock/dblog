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
    public class YearlyCounterTest : NHibernateCrudTest
    {
        private YearlyCounter mYearlyCounter = null;

        public YearlyCounterTest()
        {
            mYearlyCounter = new YearlyCounter();
            mYearlyCounter.DateTime = DateTime.UtcNow;
            mYearlyCounter.RequestCount = 10;
        }

        public override object Object
        {
            get
            {
                return mYearlyCounter;
            }
        }
    }
}
