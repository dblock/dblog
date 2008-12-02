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
    public class MonthlyCounterTest : NHibernateCrudTest
    {
        private MonthlyCounter mMonthlyCounter = null;

        public MonthlyCounterTest()
        {
            mMonthlyCounter = new MonthlyCounter();
            mMonthlyCounter.DateTime = DateTime.UtcNow;
            mMonthlyCounter.RequestCount = 10;
        }

        public override object Object
        {
            get
            {
                return mMonthlyCounter;
            }
        }
    }
}
