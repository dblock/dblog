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
    public class NamedCounterTest : NHibernateCrudTest
    {
        private NamedCounter mNamedCounter = null;

        public NamedCounterTest()
        {
            CounterTest counter = new CounterTest();
            AddDependentObject(counter);

            mNamedCounter = new NamedCounter();
            mNamedCounter.Name = Guid.NewGuid().ToString();
            mNamedCounter.Counter = counter.Counter;
        }

        public override object Object
        {
            get 
            {
                return mNamedCounter;
            }
        }
    }
}
