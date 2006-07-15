using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace DBlog.TransitData
{
    public enum TransitSortDirection
    {
        Ascending,
        Descending
    }

    [Serializable()]
    public class TransitObject
    {
        private int mId;

        public TransitObject()
        {
            Id = 0;
        }

        public TransitObject(int id)
        {
            Id = id;
        }

        public int Id
        {
            get
            {
                return mId;
            }
            set
            {
                mId = value;
            }
        }
    }
}
