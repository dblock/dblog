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
    public class BrowserTest : NHibernateCrudTest
    {
        private Browser mBrowser = null;

        public Browser Browser
        {
            get
            {
                return mBrowser;
            }
        }

        public BrowserTest()
        {
            mBrowser = new Browser();
            mBrowser.Name = Guid.NewGuid().ToString();
            mBrowser.Platform = Guid.NewGuid().ToString();
            mBrowser.Version = Guid.NewGuid().ToString().Substring(0, 6);
        }

        public override object Object
        {
            get 
            {
                return mBrowser;
            }
        }
    }
}
