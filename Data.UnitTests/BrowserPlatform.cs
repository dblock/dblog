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
    public class BrowserPlatformTest : NHibernateCrudTest
    {
        private BrowserPlatform mBrowserPlatform = null;

        public BrowserPlatform BrowserPlatform
        {
            get
            {
                return mBrowserPlatform;
            }
        }

        public BrowserPlatformTest()
        {
            PlatformTest platform = new PlatformTest();
            AddDependentObject(platform);

            BrowserTest browser = new BrowserTest();
            AddDependentObject(browser);

            mBrowserPlatform = new BrowserPlatform();
            mBrowserPlatform.Platform = platform.Platform;
            mBrowserPlatform.Browser = browser.Browser;
        }

        public override object Object
        {
            get 
            {
                return mBrowserPlatform;
            }
        }
    }
}
