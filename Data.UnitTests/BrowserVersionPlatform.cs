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
    public class BrowserVersionPlatformTest : NHibernateCrudTest
    {
        private BrowserVersionPlatform mBrowserVersionPlatform = null;

        public BrowserVersionPlatform BrowserVersionPlatform
        {
            get
            {
                return mBrowserVersionPlatform;
            }
        }

        public BrowserVersionPlatformTest()
        {
            BrowserPlatformTest browserplatform = new BrowserPlatformTest();
            AddDependentObject(browserplatform);

            BrowserVersionTest browserversion = new BrowserVersionTest();
            AddDependentObject(browserversion);

            mBrowserVersionPlatform = new BrowserVersionPlatform();
            mBrowserVersionPlatform.BrowserPlatform = browserplatform.BrowserPlatform;
            mBrowserVersionPlatform.BrowserVersion = browserversion.BrowserVersion;
        }

        public override object Object
        {
            get 
            {
                return mBrowserVersionPlatform;
            }
        }
    }
}
