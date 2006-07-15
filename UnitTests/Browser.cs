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
