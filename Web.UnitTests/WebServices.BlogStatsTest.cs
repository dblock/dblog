using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DBlog.Tools;
using DBlog.Web.UnitTests.WebServices.Blog;
using NUnit;
using System.Drawing;
using DBlog.Tools.Drawing;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogStatsTest : BlogTest
    {
        public BlogStatsTest()
        {

        }

        [Test]
        public void CreateOrUpdateStatsTest()
        {
            TransitReferrerHost host = new TransitReferrerHost();
            host.LastSource = Guid.NewGuid().ToString();
            host.LastUrl = Guid.NewGuid().ToString();
            host.Name = Guid.NewGuid().ToString();
            host.RequestCount = 1;
            host.Id = Blog.CreateOrUpdateReferrerHost(Ticket, host);

            TransitBrowser browser = new TransitBrowser();
            browser.Name = Guid.NewGuid().ToString();
            browser.Platform = Guid.NewGuid().ToString();
            browser.Version = Guid.NewGuid().ToString().Substring(0, 10);
            browser.Id = Blog.CreateOrUpdateBrowser(Ticket, browser);

            List<TransitReferrerHost> hosts = new List<TransitReferrerHost>();
            hosts.Add(host);

            List<TransitBrowser> browsers = new List<TransitBrowser>();
            browsers.Add(browser);

            int result = Blog.CreateOrUpdateStats(Ticket, browsers.ToArray(), hosts.ToArray());
            Assert.AreEqual(result, Math.Max(browsers.Count, hosts.Count));

            Blog.DeleteReferrerHost(Ticket, host.Id);
            Blog.DeleteBrowser(Ticket, browser.Id);
        }
    }
}
