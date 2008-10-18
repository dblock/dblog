using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Reflection;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogReferrerHostTest : BlogCrudTest
    {
        private TransitReferrerHost mReferrerHost = null;

        public BlogReferrerHostTest()
        {
            mReferrerHost = new TransitReferrerHost();
            mReferrerHost.Name = Guid.NewGuid().ToString();
            mReferrerHost.LastSource = Guid.NewGuid().ToString();
            mReferrerHost.LastUrl = Guid.NewGuid().ToString();
            mReferrerHost.RequestCount = 1;
        }

        public override bool IsServiceCount
        {
            get
            {
                return true;
            }
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mReferrerHost;
            }
            set
            {
                mReferrerHost = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "ReferrerHost";
            }
        }

        [Test]
        public void CreateOrUpdateDupTest()
        {
            TransitReferrerHost host = new TransitReferrerHost();
            host.LastSource = Guid.NewGuid().ToString();
            host.LastUrl = Guid.NewGuid().ToString();
            host.Name = Guid.NewGuid().ToString();
            host.RequestCount = 1;
            host.Id = Blog.CreateOrUpdateReferrerHost(Ticket, host);

            TransitReferrerHostRollup rollup = new TransitReferrerHostRollup();
            rollup.Name = Guid.NewGuid().ToString();
            rollup.Rollup = host.Name;
            rollup.Id = Blog.CreateOrUpdateReferrerHostRollup(Ticket, rollup);

            TransitReferrerHost host2 = new TransitReferrerHost();
            host2.LastSource = host2.LastUrl = Guid.NewGuid().ToString();
            host2.Name = rollup.Name;
            host2.RequestCount = 1;
            host2.Id = Blog.CreateOrUpdateReferrerHost(Ticket, host2);

            Assert.AreEqual(host.Id, host2.Id, "Rollup did not translate host.");

            Blog.DeleteReferrerHost(Ticket, host.Id);
            Blog.DeleteReferrerHostRollup(Ticket, rollup.Id);
        }

        [Test]
        public void CreateOrUpdateDupExTest()
        {
            TransitReferrerHost host = new TransitReferrerHost();
            host.LastSource = Guid.NewGuid().ToString();
            host.LastUrl = Guid.NewGuid().ToString();
            host.Name = Guid.NewGuid().ToString();
            host.RequestCount = 1;
            host.Id = Blog.CreateOrUpdateReferrerHost(Ticket, host);

            string rollupname = Guid.NewGuid().ToString();

            TransitReferrerHostRollup rollup = new TransitReferrerHostRollup();
            rollup.Name = string.Format("%.{0}", rollupname);
            rollup.Rollup = host.Name;
            rollup.Id = Blog.CreateOrUpdateReferrerHostRollup(Ticket, rollup);

            TransitReferrerHost host2 = new TransitReferrerHost();
            host2.LastSource = host2.LastUrl = Guid.NewGuid().ToString();
            host2.Name = string.Format("www.{0}", rollupname);
            host2.RequestCount = 1;
            host2.Id = Blog.CreateOrUpdateReferrerHost(Ticket, host2);

            Assert.AreEqual(host.Id, host2.Id, "Rollup did not translate a LIKE host.");

            Blog.DeleteReferrerHost(Ticket, host.Id);
            Blog.DeleteReferrerHostRollup(Ticket, rollup.Id);
        }

    }
}
