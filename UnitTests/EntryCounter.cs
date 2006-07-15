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
    public class EntryCounterTest : NHibernateCrudTest
    {
        private EntryCounter mEntryCounter = null;

        public EntryCounterTest()
        {
            CounterTest countertest = new CounterTest();
            AddDependentObject(countertest);

            EntryTest entrytest = new EntryTest();
            AddDependentObject(entrytest);

            mEntryCounter = new EntryCounter();
            mEntryCounter.Counter = countertest.Counter;
            mEntryCounter.Entry = entrytest.Entry;
        }

        public override object Object
        {
            get 
            {
                return mEntryCounter;
            }
        }
    }
}
