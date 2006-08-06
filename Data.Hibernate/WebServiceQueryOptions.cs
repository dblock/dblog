using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Reflection;
using NHibernate.Expression;

namespace DBlog.Data.Hibernate
{
    public enum WebServiceQuerySortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public class WebServiceQueryOptions
    {
        private int mPageSize = -1;
        private int mPageNumber = 0;
        private WebServiceQuerySortDirection mSortDirection = WebServiceQuerySortDirection.Ascending;
        private string mSortExpression = string.Empty;

        public string SortExpression
        {
            get
            {
                return mSortExpression;
            }
            set
            {
                mSortExpression = value;
            }
        }

        public WebServiceQuerySortDirection SortDirection
        {
            get
            {
                return mSortDirection;
            }
            set
            {
                mSortDirection = value;
            }
        }

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

        public virtual void Apply(IQuery query)
        {
            if (PageSize > 0)
            {
                query.SetMaxResults(PageSize);
                query.SetFirstResult(FirstResult);
            }
        }

        public virtual void Apply(ICriteria criteria)
        {
            if (PageSize > 0)
            {
                criteria.SetMaxResults(PageSize);
                criteria.SetFirstResult(FirstResult);
            }

            if (! string.IsNullOrEmpty(SortExpression))
            {
                criteria.AddOrder((SortDirection == WebServiceQuerySortDirection.Ascending)
                    ? Order.Asc(SortExpression)
                    : Order.Desc(SortExpression));
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
