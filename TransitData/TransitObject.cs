using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using NHibernate;
using NHibernate.Expression;

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

        public static T GetRandomElement<T>(IList<T> collection)
        {
            if (collection == null)
                return default(T);

            if (collection.Count == 0)
                return default(T);

            return collection[new Random().Next() % collection.Count];
        }

        public static int GetRandomElementId<T>(IList<T> collection)
        {
            object r = GetRandomElement(collection);
            if (r == null) return 0;
            return (int) r.GetType().GetProperty("Id").GetValue(r, null);
        }
    }

    public class AssociatedTransitObject<AssociatedType>
    {
        public static AssociatedType GetAssociatedObject(ISession session, string table, int id)
        {
            return (AssociatedType)TransitObject.GetRandomElement<AssociatedType>(
                session.CreateCriteria(typeof(AssociatedType))
                    .Add(Expression.Eq(string.Format("{0}.Id", table), id))
                    .List<AssociatedType>());
        }
    }
}
