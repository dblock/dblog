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
    public class DailyCounterTest : NHibernateCrudTest
    {
        private DailyCounter mDailyCounter = null;

        public DailyCounterTest()
        {
            mDailyCounter = new DailyCounter();
            mDailyCounter.DateTime = DateTime.UtcNow;
            mDailyCounter.RequestCount = 10;
        }

        public override object Object
        {
            get
            {
                return mDailyCounter;
            }
        }
    }
}
