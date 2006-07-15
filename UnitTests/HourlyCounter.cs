using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.UnitTests
{
    [TestFixture]
    public class HourlyCounterTest : NHibernateCrudTest
    {
        private HourlyCounter mHourlyCounter = null;

        public HourlyCounterTest()
        {
            mHourlyCounter = new HourlyCounter();
            mHourlyCounter.DateTime = DateTime.UtcNow;
            mHourlyCounter.RequestCount = 10;
        }

        public override object Object
        {
            get 
            {
                return mHourlyCounter;
            }
        }
    }
}
