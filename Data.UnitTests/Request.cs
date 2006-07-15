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
    public class RequestTest : NHibernateCrudTest
    {
        private Request mRequest = null;

        public RequestTest()
        {
            BrowserVersionPlatformTest browserversionplatform = new BrowserVersionPlatformTest();
            AddDependentObject(browserversionplatform);

            mRequest = new Request();
            mRequest.BrowserVersionPlatform = browserversionplatform.BrowserVersionPlatform;
            mRequest.DateTime = DateTime.UtcNow;
            mRequest.IpAddress = "127.0.0.1";
        }

        public override object Object
        {
            get 
            {
                return mRequest;
            }
        }
    }
}
