using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitEntry : TransitObject
    {
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

        private string mIpAddress;

        public string IpAddress
        {
            get
            {
                return mIpAddress;
            }
            set
            {
                mIpAddress = value;
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

        public TransitEntry()
        {

        }

        public TransitEntry(ISession session, DBlog.Data.Entry o)
            : base(o.Id)
        {
            Title = o.Title;
            Text = o.Text;
            IpAddress = o.IpAddress;
            OwnerLoginId = o.OwnerLogin.Id;
            TopicId = o.Topic.Id;
            EntryImage img = AssociatedTransitObject<EntryImage>.GetAssociatedObject(session, "Entry", o.Id);
            if (img != null) ImageId = img.Image.Id;
            Created = o.Created;
            Modified = o.Modified;
        }

        public Entry GetEntry(ISession session)
        {
            Entry topic = (Id != 0) ? (Entry)session.Load(typeof(Entry), Id) : new Entry();
            topic.Title = Title;
            topic.Text = Text;
            topic.IpAddress = IpAddress;
            topic.OwnerLogin = (OwnerLoginId > 0) ? (Login)session.Load(typeof(Login), OwnerLoginId) : null;
            topic.Topic = (Topic)session.Load(typeof(Topic), TopicId);
            return topic;
        }
    }
}
