using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using DBlog.Tools.Web;

namespace DBlog.Data.Hibernate
{
    public class StringCriteria
    {
        private ISession mSession = null;
        private string mTable = string.Empty;
        private StringBuilder mSubQuery = new StringBuilder();
        private List<string> mOrderBy = new List<string>();
        private Type mType;
        private string[] mAdditionalTables = null;

        public StringCriteria(ISession session, string table, Type type)
            : this(session, table, type, null)
        {

        }

        public StringCriteria(ISession session, string table, Type type, string[] addtables)
        {
            mSession = session;
            mTable = table;
            mType = type;
            mAdditionalTables = addtables;
        }

        public void Add(string sqlquery)
        {
            mSubQuery.Append(mSubQuery.Length > 0 ? " AND " : " WHERE ");
            mSubQuery.Append(sqlquery);
        }

        public void AddOrder(string orderby, WebServiceQuerySortDirection direction)
        {
            foreach (char c in orderby)
            {
                if (!Char.IsLetterOrDigit(c) && c != '.')
                {
                    throw new Exception("Invalid character in order");
                }
            }

            mOrderBy.Add(string.Format("{0} {1}", Renderer.SqlEncode(orderby), 
                direction == WebServiceQuerySortDirection.Ascending ? "ASC" : "DESC"));
        }

        public IQuery CreateQuery()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT {" + mTable + ".*} FROM " + mTable + " {" + mTable + "}");

            if (mAdditionalTables != null)
            {
                foreach (string table in mAdditionalTables)
                {
                    query.AppendFormat(", {0}", table);
                }
            }

            query.AppendLine(mSubQuery.ToString());

            if (mOrderBy.Count > 0)
            {
                query.Append(" ORDER BY ");
                for (int i = 0; i < mOrderBy.Count; i++)
                {
                    if (i != 0) query.Append(", ");
                    query.Append(mOrderBy[i]);
                }
            }

            return mSession.CreateSQLQuery(query.ToString()).AddEntity(mTable, mType, LockMode.None);
        }
    }
}
