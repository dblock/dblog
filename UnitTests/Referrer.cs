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
    public class ReferrerTest : NHibernateCrudTest
    {
        private Referrer mReferrer = null;

        public Referrer Referrer
        {
            get
            {
                return mReferrer;
            }
        }

        public ReferrerTest()
        {
            mReferrer = new Referrer();
            mReferrer.Source = Guid.NewGuid().ToString();
            mReferrer.Url = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mReferrer;
            }
        }
    }
}
