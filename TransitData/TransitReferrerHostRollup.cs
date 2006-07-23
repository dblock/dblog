using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitReferrerHostRollup : TransitObject
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

        private string mRollup;

        public string Rollup
        {
            get
            {
                return mRollup;
            }
            set
            {
                mRollup = value;
            }
        }

        public TransitReferrerHostRollup()
        {

        }

        public TransitReferrerHostRollup(DBlog.Data.ReferrerHostRollup o)
            : base(o.Id)
        {
            Rollup = o.Rollup;
            Name = o.Name;
        }

        public ReferrerHostRollup GetReferrerHostRollup(ISession session)
        {
            ReferrerHostRollup referrerhostrollup = (Id != 0) 
                ? (ReferrerHostRollup) session.Load(typeof(ReferrerHostRollup), Id) 
                : new ReferrerHostRollup();
            referrerhostrollup.Name = Name;
            referrerhostrollup.Rollup = Rollup;
            return referrerhostrollup;
        }
    }
}
