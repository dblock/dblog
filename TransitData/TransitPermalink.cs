using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Tools;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPermalinkQueryOptions : WebServiceQueryOptions
    {
        private int mSourceId = 0;

        public int SourceId
        {
            get
            {
                return mSourceId;
            }
            set
            {
                mSourceId = value;
            }
        }

        private string mSourceType = string.Empty;

        public string SourceType
        {
            get
            {
                return mSourceType;
            }
            set
            {
                mSourceType = value;
            }
        }

        public TransitPermalinkQueryOptions()
        {
        }

        public TransitPermalinkQueryOptions(
            int id, string type)
        {
            mSourceId = id;
            mSourceType = type;
        }

        public override void Apply(ICriteria criteria)
        {
            if (SourceId != 0) criteria.Add(Expression.Eq("SourceId", SourceId));
            if (! string.IsNullOrEmpty(SourceType)) criteria.Add(Expression.Eq("SourceType", SourceType));
            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (SourceId != 0) query.Add(Expression.Eq("SourceId", SourceId));
            if (!string.IsNullOrEmpty(SourceType)) query.Add(Expression.Eq("SourceType", SourceType));
            base.Apply(query);
        }
    }

    public class TransitPermalink : TransitObject
    {
        private string mSourceType;

        public string SourceType
        {
            get
            {
                return mSourceType;
            }
            set
            {
                mSourceType = value;
            }
        }

        private string mTargetType;

        public string TargetType
        {
            get
            {
                return mTargetType;
            }
            set
            {
                mTargetType = value;
            }
        }

        private int mSourceId;

        public int SourceId
        {
            get
            {
                return mSourceId;
            }
            set
            {
                mSourceId = value;
            }
        }

        private int mTargetId;

        public int TargetId
        {
            get
            {
                return mTargetId;
            }
            set
            {
                mTargetId = value;
            }
        }

        public TransitPermalink()
        {

        }

        public TransitPermalink(DBlog.Data.Permalink o)
            : base(o.Id)
        {
            TargetType = o.TargetType;
            SourceType = o.SourceType;
            SourceId = o.SourceId;
            TargetId = o.TargetId;
        }

        public Permalink GetPermalink(ISession session)
        {
            Permalink permalink = (Id != 0) ? (Permalink)session.Load(typeof(Permalink), Id) : new Permalink();
            permalink.SourceId = SourceId;
            permalink.TargetId = TargetId;
            permalink.SourceType = SourceType;
            permalink.TargetType = TargetType;
            return permalink;
        }
    }
}
