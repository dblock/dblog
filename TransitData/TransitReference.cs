using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitReference : TransitObject
    {
        private string mWord;

        public string Word
        {
            get
            {
                return mWord;
            }
            set
            {
                mWord = value;
            }
        }

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

        private string mResult;

        public string Result
        {
            get
            {
                return mResult;
            }
            set
            {
                mResult = value;
            }
        }

        public TransitReference()
        {

        }

        public TransitReference(DBlog.Data.Reference o)
            : base(o.Id)
        {
            Url = o.Url;
            Word = o.Word;
            Result = o.Result;
        }

        public Reference GetReference(ISession session)
        {
            Reference reference = (Id != 0) ? (Reference)session.Load(typeof(Reference), Id) : new Reference();
            reference.Word = Word;
            reference.Url = Url;
            reference.Result = Result;
            return reference;
        }
    }
}
