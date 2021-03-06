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
    public class WeeklyCounterTest : NHibernateCrudTest
    {
        private WeeklyCounter mWeeklyCounter = null;

        public WeeklyCounterTest()
        {
            mWeeklyCounter = new WeeklyCounter();
            mWeeklyCounter.DateTime = DateTime.UtcNow;
            mWeeklyCounter.RequestCount = 10;
        }

        public override object Object
        {
            get
            {
                return mWeeklyCounter;
            }
        }
    }
}
