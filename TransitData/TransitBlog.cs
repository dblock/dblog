using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Data.Hibernate;
using NHibernate.Expression;

namespace DBlog.TransitData
{
    public class TransitBlogQueryOptions : WebServiceQueryOptions
    {
        private int mTopicId = 0;

        public int TopicId
        {
            get
            {
                return mTopicId;
            }
            set
            {
                mTopicId = value;
            }
        }

        public TransitBlogQueryOptions()
        {

        }

        public TransitBlogQueryOptions(int topicid)
        {
            mTopicId = topicid;            
        }

        public TransitBlogQueryOptions(int topicid, int pagesize, int pagenumber)
            : base(pagesize, pagenumber)
        {
            mTopicId = topicid;
        }

        public override void Apply(ICriteria criteria)
        {
            if (mTopicId > 0)
            {
                criteria.Add(Expression.Eq("Topic.Id", TopicId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (mTopicId > 0)
            {
                query.Add(Expression.Eq("Topic.Id", TopicId));
            }

            base.Apply(query);
        }
    }

    public class TransitBlog : TransitObject
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

        private string mType;

        public string Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        private DateTime mCreated;

        public DateTime Created
        {
            get
            {
                return mCreated;
            }
            set
            {
                mCreated = value;
            }
        }

        private DateTime mModified;

        public DateTime Modified
        {
            get
            {
                return mModified;
            }
            set
            {
                mModified = value;
            }
        }

        private string mText;

        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
            }
        }

        private string mTopicName;

        public string TopicName
        {
            get
            {
                return mTopicName;
            }
            set
            {
                mTopicName = value;
            }
        }

        private int mTopicId;

        public int TopicId
        {
            get
            {
                return mTopicId;
            }
            set
            {
                mTopicId = value;
            }
        }

        private int mTemplateId;

        public int TemplateId
        {
            get
            {
                return mTemplateId;
            }
            set
            {
                mTemplateId = value;
            }
        }

        private int mOwnerLoginId;

        public int OwnerLoginId
        {
            get
            {
                return mOwnerLoginId;
            }
            set
            {
                mOwnerLoginId = value;
            }
        }

        public TransitBlog()
        {

        }

        public TransitBlog(DBlog.Data.Blog o)
            : base(o.Id)
        {
            Type = o.Type;
            Title = o.Title;
            Text = o.Text;
            OwnerLoginId = o.OwnerLogin.Id;
            Modified = o.Modified;
            Created = o.Created;
            TopicName = o.Topic.Name;
            TopicId = o.Topic.Id;
        }
    }
}
