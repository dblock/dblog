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
    public class RollupBrowserVersionPlatformTest : NHibernateCrudTest
    {
        private RollupBrowserVersionPlatform mRollupBrowserVersionPlatform = null;

        public RollupBrowserVersionPlatformTest()
        {
            BrowserVersionPlatformTest browserversionplatform = new BrowserVersionPlatformTest();
            AddDependentObject(browserversionplatform);

            mRollupBrowserVersionPlatform = new RollupBrowserVersionPlatform();
            mRollupBrowserVersionPlatform.BrowserVersionPlatform = browserversionplatform.BrowserVersionPlatform;
            mRollupBrowserVersionPlatform.RequestCount = 10;
        }

        public override object Object
        {
            get 
            {
                return mRollupBrowserVersionPlatform;
            }
        }
    }
}
