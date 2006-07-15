using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogLoginTest : BlogTest
    {
        [Test]
        public void GetTicketTest()
        {
            string ticket = Blog.Login("Administrator", string.Empty);
            Assert.IsFalse(string.IsNullOrEmpty(ticket),
                "Login failed for Administrator with a blank password.\n" +
                "Please make sure this account exists for unit tests.");

            // insert into Login VALUES ( 'Administrator', '', 'Administrator', 'Administrator', '1B2M2Y8AsgTpgAmY7PhCfg==', '' )

            Console.WriteLine(string.Format("Ticket: {0} bytes", ticket.Length));
        }

        [Test]
        [ExpectedException(typeof(SoapException))]
        public void InvalidLoginTest()
        {
            Blog.Login(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }
    }
}
