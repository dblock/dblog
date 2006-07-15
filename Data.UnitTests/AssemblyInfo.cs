using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.UnitTests
{
    [TestFixture]
    public class AssemblyInfoCrudTest : NHibernateCrudTest
    {
        private AssemblyInfo mAssemblyInfo = null;

        public AssemblyInfoCrudTest()
        {
            mAssemblyInfo = new AssemblyInfo();
        }

        public override object Object
        {
            get 
            {
                return mAssemblyInfo;
            }
        }

        public override int ObjectId
        {
            get 
            {
                return mAssemblyInfo.Id;
            }
        }
    }
}
