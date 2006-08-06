using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Reflection;

namespace DBlog.Data.Hibernate
{
    public class WebServiceQueryOptions
    {
        private int mPageSize = -1;
        private int mPageNumber = 0;

        public int PageSize
        {
            get
            {
                return mPageSize;
            }
            set
            {
                mPageSize = value;
            }
        }

        public int PageNumber
        {
            get
            {
                return mPageNumber;
            }
            set
            {
                mPageNumber = value;
            }
        }

        public int FirstResult
        {
            get
            {
                return PageSize * PageNumber;
            }
        }

        public WebServiceQueryOptions()
        {
        }

        public WebServiceQueryOptions(int pagesize, int pagenumber)
        {
            PageSize = pagesize;
            PageNumber = pagenumber;
        }

        public virtual void Apply(ICriteria criteria)
        {
            if (PageSize > 0)
            {
                criteria.SetMaxResults(PageSize);
                criteria.SetFirstResult(FirstResult);
            }
        }

        public virtual void Apply(CountQuery query)
        {

        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public static int GetHashCode(object o)
        {
            StringBuilder hash = new StringBuilder();
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propertyvalue = property.GetValue(o, null);
                hash.AppendLine(propertyvalue == null ? string.Empty : propertyvalue.ToString());
            }

            return hash.ToString().GetHashCode();
        }
    };
}
