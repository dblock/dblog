using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogPropertiesTest : BlogTest
    {
        [Test]
        public void GetPropertiesTest()
        {
            Console.WriteLine(string.Format("Blog service: {0} ({1})", Blog.GetTitle(), Blog.GetVersion()));
            Assert.AreEqual(Blog.Url, Url);
            Assert.IsFalse(string.IsNullOrEmpty(Blog.GetVersion()));
            Assert.IsFalse(string.IsNullOrEmpty(Blog.GetTitle()));
            Assert.IsFalse(string.IsNullOrEmpty(Blog.GetCopyright()));
            Assert.IsFalse(string.IsNullOrEmpty(Blog.GetDescription()));
        }

        [Test]
        public void GetUptimeTest()
        {
            long uptime = Blog.GetUptime();
            Console.WriteLine(string.Format("Uptime: {0}", uptime));
            Assert.IsTrue(uptime > 0);
            Thread.Sleep(100);
            long uptime2 = Blog.GetUptime();
            Assert.IsTrue(uptime2 > uptime);
        }
    }
}
