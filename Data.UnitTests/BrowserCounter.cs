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
    public class BrowserCounterTest : NHibernateCrudTest
    {
        private BrowserCounter mBrowserCounter = null;

        public BrowserCounterTest()
        {
            BrowserTest Browser = new BrowserTest();
            AddDependentObject(Browser);

            CounterTest counter = new CounterTest();
            AddDependentObject(counter);

            mBrowserCounter = new BrowserCounter();
            mBrowserCounter.Counter = counter.Counter;
            mBrowserCounter.Browser = Browser.Browser;
        }

        public override object Object
        {
            get
            {
                return mBrowserCounter;
            }
        }
    }
}
