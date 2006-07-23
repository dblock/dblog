using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Web.Services.Protocols;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogLoginTest : BlogCrudTest
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

        private TransitLogin mLogin = null;

        public BlogLoginTest()
        {
            mLogin = new TransitLogin();
            mLogin.Email = Guid.NewGuid().ToString().Substring(0, 31);
            mLogin.Name = Guid.NewGuid().ToString();
            mLogin.Password = Guid.NewGuid().ToString().Substring(0, 31);
            mLogin.Role = TransitLoginRole.Guest;
            mLogin.Username = Guid.NewGuid().ToString();
            mLogin.Website = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mLogin;
            }
            set
            {
                mLogin = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "Login";
            }
        }

    }
}
