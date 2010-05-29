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
    public class LoginCounterTest : NHibernateCrudTest
    {
        private LoginCounter mLoginCounter = null;

        public LoginCounterTest()
        {
            LoginTest login = new LoginTest();
            AddDependentObject(login);

            CounterTest counter = new CounterTest();
            AddDependentObject(counter);

            mLoginCounter = new LoginCounter();
            mLoginCounter.Counter = counter.Counter;
            mLoginCounter.Login = login.Login;
        }

        public override object Object
        {
            get 
            {
                return mLoginCounter;
            }
        }
    }
}
