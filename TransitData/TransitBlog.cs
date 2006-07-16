using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Data.Hibernate;
using NHibernate.Expression;

namespace DBlog.TransitData
{
    public enum TransitBlogType
    {
        Unknown,
        Entry,
        Gallery
    };

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

            criteria.AddOrder(Order.Desc("Created"));
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

        private TransitBlogType mType;

        public TransitBlogType Type
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

        private int mImageId;

        public int ImageId
        {
            get
            {
               return mImageId;
            }
            set
            {
               mImageId = value;
            }
        }

        public TransitBlog()
        {

        }

        public TransitBlog(ISession session, DBlog.Data.Blog o)
            : base(o.Id)
        {
            Type = (TransitBlogType) Enum.Parse(typeof(TransitBlogType), o.Type);
            Title = o.Title;
            Text = o.Text;
            OwnerLoginId = o.OwnerLogin.Id;
            Modified = o.Modified;
            Created = o.Created;
            TopicName = o.Topic.Name;
            TopicId = o.Topic.Id;

            switch(Type)
            {
                case TransitBlogType.Entry:
                    EntryImage ei = AssociatedTransitObject<EntryImage>.GetAssociatedObject(session, "Entry", Id);
                    if (ei != null) ImageId = ei.Image.Id;
                    break;
                case TransitBlogType.Gallery:
                    GalleryImage gi = AssociatedTransitObject<GalleryImage>.GetAssociatedObject(session, "Gallery", Id);
                    if (gi != null) ImageId = gi.Image.Id;
                    break;
            }
        }
    }
}
