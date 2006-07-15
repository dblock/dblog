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
    public class LoginTest : NHibernateCrudTest
    {
        private Login mLogin = null;

        public Login Login
        {
            get
            {
                return mLogin;
            }
        }

        public LoginTest()
        {
            mLogin = new Login();

            mLogin.Email = Guid.NewGuid().ToString().Substring(0, 31);
            mLogin.Name = Guid.NewGuid().ToString();
            mLogin.Password = Guid.NewGuid().ToString().Substring(0, 31);
            mLogin.Role = Guid.NewGuid().ToString();
            mLogin.Username = Guid.NewGuid().ToString();
            mLogin.Website = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get
            {
                return mLogin;
            }
        }
    }
}
