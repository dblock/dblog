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
    public class PostCounterTest : NHibernateCrudTest
    {
        private PostCounter mPostCounter = null;

        public PostCounterTest()
        {
            CounterTest countertest = new CounterTest();
            AddDependentObject(countertest);

            PostTest Posttest = new PostTest();
            AddDependentObject(Posttest);

            mPostCounter = new PostCounter();
            mPostCounter.Counter = countertest.Counter;
            mPostCounter.Post = Posttest.Post;
        }

        public override object Object
        {
            get 
            {
                return mPostCounter;
            }
        }
    }
}
