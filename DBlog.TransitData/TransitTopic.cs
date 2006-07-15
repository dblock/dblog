using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;

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
            Type = o.Type;
            Name = o.Name;
        }
    }
}
