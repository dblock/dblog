using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class PermalinkTest : NHibernateCrudTest
    {
        private Permalink mPermalink = null;

        public Permalink Permalink
        {
            get
            {
                return mPermalink;
            }
        }

        public PermalinkTest()
        {
            mPermalink = new Permalink();
            mPermalink.SourceId = new Random().Next();
            mPermalink.TargetId = new Random().Next();
            mPermalink.SourceType = Guid.NewGuid().ToString();
            mPermalink.TargetType = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get
            {
                return mPermalink;
            }
        }
    }
}
