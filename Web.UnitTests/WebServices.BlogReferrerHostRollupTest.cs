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
    public class BlogReferrerHostRollupTest : BlogCrudTest
    {
        private TransitReferrerHostRollup mReferrerHostRollup = null;

        public BlogReferrerHostRollupTest()
        {
            mReferrerHostRollup = new TransitReferrerHostRollup();
            mReferrerHostRollup.Name = Guid.NewGuid().ToString();
            mReferrerHostRollup.Rollup = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mReferrerHostRollup;
            }
            set
            {
                mReferrerHostRollup = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "ReferrerHostRollup";
            }
        }

        [Test]
        public void ExistingReferrerHostRollupTest()
        {
            int count = 10;
            // create hosts
            TransitReferrerHost host = new TransitReferrerHost();
            host.LastSource = host.LastUrl = "http://localhost";
            host.RequestCount = 1;

            string root = Guid.NewGuid().ToString();
            for (int i = 0; i < count; i++)
            {
                host.Name = string.Format("www.{0}.{1}", root, i);
                Blog.CreateOrUpdateReferrerHost(Ticket, host);
                Console.WriteLine("Created {0}", host.Name);
            }

            // create a rollup, should merge hosts
            TransitReferrerHostRollup rollup = new TransitReferrerHostRollup();
            rollup.Name = string.Format("www.{0}.%", root);
            rollup.Rollup = string.Format("www.{0}.target", root);
            rollup.Id = Blog.CreateOrUpdateReferrerHostRollup(Ticket, rollup);

            // additional host will auto-rollup
            for (int i = 0; i < count; i++)
            {
                host.Name = string.Format("www.{0}.{1}", root, i);
                Blog.CreateOrUpdateReferrerHost(Ticket, host);
                Console.WriteLine("Created {0}", host.Name);
            }

            // get the rollup host
            TransitReferrerHost[] hosts = Blog.GetReferrerHosts(Ticket, null);

            bool found = false;
            foreach (TransitReferrerHost rh in hosts)
            {
                if (rh.Name == rollup.Rollup)
                {
                    Console.WriteLine("Found {0} with {1} hits", rh.Name, rh.RequestCount);
                    Assert.AreEqual(rh.RequestCount, count * 2);
                    found = true;
                    Blog.DeleteReferrerHost(Ticket, rh.Id);
                }
            }

            Assert.IsTrue(found);
            Blog.DeleteReferrerHostRollup(Ticket, rollup.Id);
        }
    }
}
