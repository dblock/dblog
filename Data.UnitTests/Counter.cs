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
    public class CounterTest : NHibernateCrudTest
    {
        private Counter mCounter = null;

        public CounterTest()
        {
            mCounter = new Counter();
            mCounter.Created = DateTime.UtcNow;
        }

        public Counter Counter
        {
            get
            {
                return mCounter;
            }
        }

        public override object Object
        {
            get 
            {
                return mCounter;
            }
        }
    }
}
