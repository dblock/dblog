using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;

namespace DBlog.TransitData
{
    public class TransitCounter : TransitObject
    {
        private long mCount = 0;

        public long Count
        {
            get
            {
                return mCount;
            }
            set
            {
                mCount = value;
            }
        }

        private DateTime mCreated = DateTime.UtcNow;

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

        public TransitCounter()
        {

        }

        public TransitCounter(DBlog.Data.Counter o)
            : base(o.Id)
        {
            Created = o.Created;
            Count = o.Count;
        }

        public Counter GetCounter(ISession session)
        {
            Counter Counter = (Id != 0) ? (Counter)session.Load(typeof(Counter), Id) : new Counter();
            Counter.Count = Count;
            Counter.Created = Created;
            return Counter;
        }

        public static long Increment<ObjectType, CounterType>(ISession session, int id) where CounterType: new()
        {
            CounterType associated = (CounterType) session.CreateCriteria(typeof(CounterType))
                .Add(Expression.Eq(string.Format("{0}.Id", typeof(ObjectType).Name), id))
                .UniqueResult();

            Counter counter = null;
            if (associated == null)
            {
                associated = new CounterType();
                counter = new Counter();
                counter.Count = 0;                
                counter.Created = DateTime.UtcNow;
                associated.GetType().GetProperty("Counter").SetValue(
                    associated, counter, null);
                associated.GetType().GetProperty(typeof(ObjectType).Name).SetValue(
                    associated, session.Load(typeof(ObjectType), id), null);
            }
            else
            {
                counter = (Counter) associated.GetType()
                    .GetProperty("Counter").GetValue(associated, null);
            }

            counter.Count++;
            session.Save(counter);
            session.Save(associated);

            return counter.Count;
        }

        public static TransitCounter GetAssociatedCounter<ObjectType, CounterType>(ISession session, int id)
        {
            CounterType association = (CounterType)session.CreateCriteria(typeof(CounterType))
                .Add(Expression.Eq(string.Format("{0}.Id", typeof(ObjectType).Name), id))
                .UniqueResult();

            if (association == null)
            {
                return new TransitCounter();
            }

            Counter counter = (Counter) association.GetType()
                .GetProperty("Counter").GetValue(association, null);

            return new TransitCounter(counter);
        }
    }
}
