using System;
using System.Text;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
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
