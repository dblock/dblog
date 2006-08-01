using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitPlatform : TransitObject
    {
        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        public TransitPlatform()
        {

        }

        public TransitPlatform(DBlog.Data.Platform o)
            : base(o.Id)
        {
            Name = o.Name;
        }

        public Platform GetPlatform(ISession session)
        {
            Platform platform = (Id != 0) ? (Platform)session.Load(typeof(Platform), Id) : new Platform();
            platform.Name = Name;
            return platform;
        }
    }
}
