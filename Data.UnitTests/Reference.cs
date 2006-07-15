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
    public class ReferenceTest : NHibernateCrudTest
    {
        private Reference mReference = null;

        public ReferenceTest()
        {
            mReference = new Reference();
            mReference.Result = Guid.NewGuid().ToString();
            mReference.Url = Guid.NewGuid().ToString();
            mReference.Word = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mReference;
            }
        }
    }
}
