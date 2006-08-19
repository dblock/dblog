using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace DBlog.Data.Hibernate
{
    public class StringCriteria
    {
        private ISession mSession = null;
        private string mTable = string.Empty;
        private StringBuilder mSubQuery = new StringBuilder();
        private string mOrderBy = string.Empty;
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
            mOrderBy = string.Format("ORDER BY {0} {1}", orderby.Replace("'", "''"), 
                direction == WebServiceQuerySortDirection.Ascending ? "ASC" : "DESC");
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
            query.AppendLine(mOrderBy);
            IQuery iq = mSession.CreateSQLQuery(query.ToString(), mTable, mType);
            return iq;
        }
    }
}
