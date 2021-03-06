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
