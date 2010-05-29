using System;
using System.Text;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System.Collections;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class Data : NHibernateTest
    {
        public Data()
        {

        }

        [Test]
        public void TestSystem()
        {
            Console.WriteLine(string.Format("Connection: {0}", 
                base.Session.Connection.ConnectionString));
        }
    }
}
