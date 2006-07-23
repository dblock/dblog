using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitTopic : TransitObject
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

        public TransitTopic()
        {

        }

        public TransitTopic(DBlog.Data.Topic o)
            : base(o.Id)
        {
            Name = o.Name;
        }

        public Topic GetTopic(ISession session)
        {
            Topic topic = (Id != 0) ? (Topic)session.Load(typeof(Topic), Id) : new Topic();
            topic.Name = Name;
            return topic;
        }
    }
}
