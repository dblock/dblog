using System;
using NUnit.Framework;
using DBlog.Data;
using NHibernate;
using NHibernate.Cfg;

namespace DBlog.Data.Hibernate.UnitTests
{
    /// <summary>
    /// NHibernate test foundation.
    /// </summary>
    public class NHibernateTest
    {
        private static ISessionFactory mFactory = null;
        private ISession mSession;

        public NHibernateTest()
        {

        }

        public static ISessionFactory Factory
        {
            get
            {
                if (mFactory == null)
                {
                    NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
                    cfg.Configure();
                    cfg.AddAssembly("DBlog.Data");
                    cfg.AddAssembly("DBlog.Data.Hibernate");
                    mFactory = cfg.BuildSessionFactory();
                }
                return mFactory;
            }
            set
            {
                mFactory = value;
            }
        }

        public ISession Session
        {
            get
            {
                return mSession;
            }
            set
            {
                mSession = value;
            }
        }

        [SetUp]
        public void SetUp()
        {
            Session = Factory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            Session.Close();
        }

    }
}
