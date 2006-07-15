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
    public class BrowserVersionTest : NHibernateCrudTest
    {
        private BrowserVersion mBrowserVersion = null;

        public BrowserVersion BrowserVersion
        {
            get
            {
                return mBrowserVersion;
            }
        }

        public BrowserVersionTest()
        {
            BrowserTest browser = new BrowserTest();
            AddDependentObject(browser);

            mBrowserVersion = new BrowserVersion();
            mBrowserVersion.Browser = browser.Browser;
        }

        public override object Object
        {
            get 
            {
                return mBrowserVersion;
            }
        }
    }
}
