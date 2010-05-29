using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Data.Hibernate;
using NHibernate.Criterion;

namespace DBlog.TransitData
{
    public class TransitFeedItemQueryOptions : WebServiceQueryOptions
    {
        private int mFeedId = 0;

        public int FeedId
        {
            get
            {
                return mFeedId;
            }
            set
            {
                mFeedId = value;
            }
        }

        public TransitFeedItemQueryOptions()
        {
        }

        public TransitFeedItemQueryOptions(
            int feedid)
        {
            mFeedId = feedid;
        }

        public TransitFeedItemQueryOptions(
            int feedid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mFeedId = feedid;
        }

        public TransitFeedItemQueryOptions(
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {

        }

        public override void Apply(ICriteria criteria)
        {
            if (FeedId != 0)
            {
                criteria.Add(Expression.Eq("Feed.Id", FeedId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (FeedId != 0)
            {
                query.Add(Expression.Eq("Feed.Id", FeedId));
            }

            base.Apply(query);
        }
    }

    public class TransitFeedItem : TransitObject
    {
        private string mTitle;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
            }
        }

        private string mLink;

        public string Link
        {
            get
            {
                return mLink;
            }
            set
            {
                mLink = value;
            }
        }

        private string mDescription;

        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
            }
        }

        private int mFeedId;

        public int FeedId
        {
            get
            {
                return mFeedId;
            }
            set
            {
                mFeedId = value;
            }
        }

        public TransitFeedItem()
        {

        }

        public TransitFeedItem(DBlog.Data.FeedItem o)
            : base(o.Id)
        {
            Link = o.Link;
            Title = o.Title;
            Description = o.Description;
            FeedId = o.Feed.Id;
        }

        public FeedItem GetFeedItem(ISession session)
        {
            FeedItem feeditem = (Id != 0) ? (FeedItem)session.Load(typeof(FeedItem), Id) : new FeedItem();
            feeditem.Title = Title;
            feeditem.Link = Link;
            feeditem.Description = Description;
            feeditem.Feed = (FeedId != 0) ? (Feed) session.Load(typeof(Feed), FeedId) : null;
            return feeditem;
        }
    }
}
