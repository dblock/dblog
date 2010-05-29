using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitReferenceQueryOptions : WebServiceQueryOptions
    {
        private string mSearchQuery = string.Empty;

        public string SearchQuery
        {
            get
            {
                return mSearchQuery;
            }
            set
            {
                mSearchQuery = value;
            }
        }

        public TransitReferenceQueryOptions()
        {
        }

        public TransitReferenceQueryOptions(
            string query)
        {
            mSearchQuery = query;
        }

        public TransitReferenceQueryOptions(
            string query, int pagesize, int pagenumber)
            : base(pagesize, pagenumber)
        {
            mSearchQuery = query;
        }

        public override void Apply(ICriteria criteria)
        {
            if (!string.IsNullOrEmpty(SearchQuery)) 
            {
                criteria.Add(Expression.Like("Word", string.Format("%{0}%", SearchQuery)));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (!string.IsNullOrEmpty(SearchQuery)) 
            {
                query.Add(Expression.Like("Word", string.Format("%{0}%", SearchQuery)));
            }

            base.Apply(query);
        }
    }

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
