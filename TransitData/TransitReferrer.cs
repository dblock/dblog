using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitReferrer : TransitObject
    {
        private string mUrl;

        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                mUrl = value;
            }
        }

        private string mSource;

        public string Source
        {
            get
            {
                return mSource;
            }
            set
            {
                mSource = value;
            }
        }

        public TransitReferrer()
        {

        }

        public TransitReferrer(DBlog.Data.Referrer o)
            : base(o.Id)
        {
            Url = o.Url;
            Source = o.Source;
        }

        public Referrer GetReferrer(ISession session)
        {
            Referrer referrer = (Id != 0) ? (Referrer)session.Load(typeof(Referrer), Id) : new Referrer();
            referrer.Url = Url;
            referrer.Source = Source;
            return referrer;
        }
    }
}
