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
    public class PlatformTest : NHibernateCrudTest
    {
        private Platform mPlatform = null;

        public Platform Platform
        {
            get
            {
                return mPlatform;
            }
        }

        public PlatformTest()
        {
            mPlatform = new Platform();
            mPlatform.Name = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mPlatform;
            }
        }
    }
}
