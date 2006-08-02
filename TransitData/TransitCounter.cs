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

        public static long Increment<ObjectType, CounterType>(ISession session, int id) where CounterType : new()
        {
            CounterType associated = (CounterType)session.CreateCriteria(typeof(CounterType))
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
                counter = (Counter)associated.GetType()
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

            Counter counter = (Counter)association.GetType()
                .GetProperty("Counter").GetValue(association, null);

            return new TransitCounter(counter);
        }

        public static void IncrementHourlyCounter(ISession session, int count)
        {
            DateTime utcnow = DateTime.UtcNow;
            DateTime hournow = utcnow.Date.AddHours(utcnow.Hour);

            HourlyCounter counter = (HourlyCounter)session.CreateCriteria(typeof(HourlyCounter))
                .Add(Expression.Eq("DateTime", hournow))
                .UniqueResult();

            if (counter == null)
            {
                counter = new HourlyCounter();
                counter.DateTime = hournow;
                counter.RequestCount = 0;
            }

            counter.RequestCount += count;
            session.Save(counter);
        }

        public static void IncrementDailyCounter(ISession session, int count)
        {
            DateTime utcnow = DateTime.UtcNow.Date;

            DailyCounter counter = (DailyCounter)session.CreateCriteria(typeof(DailyCounter))
                .Add(Expression.Eq("DateTime", utcnow))
                .UniqueResult();

            if (counter == null)
            {
                counter = new DailyCounter();
                counter.DateTime = utcnow;
                counter.RequestCount = 0;
            }

            counter.RequestCount += count;
            session.Save(counter);
        }

        public static void IncrementWeeklyCounter(ISession session, int count)
        {
            DateTime utcnow = DateTime.UtcNow.Date;
            while (utcnow.DayOfWeek != DayOfWeek.Monday)
                utcnow = utcnow.AddDays(-1);

            WeeklyCounter counter = (WeeklyCounter)session.CreateCriteria(typeof(HourlyCounter))
                .Add(Expression.Eq("DateTime", utcnow))
                .UniqueResult();

            if (counter == null)
            {
                counter = new WeeklyCounter();
                counter.DateTime = utcnow;
                counter.RequestCount = 0;
            }

            counter.RequestCount += count;
            session.Save(counter);
        }

        public static void IncrementMonthlyCounter(ISession session, int count)
        {
            DateTime utcnow = DateTime.UtcNow.Date;
            utcnow = utcnow.AddDays(1 - utcnow.Day);

            MonthlyCounter counter = (MonthlyCounter)session.CreateCriteria(typeof(MonthlyCounter))
                .Add(Expression.Eq("DateTime", utcnow))
                .UniqueResult();

            if (counter == null)
            {
                counter = new MonthlyCounter();
                counter.DateTime = utcnow;
                counter.RequestCount = 0;
            }

            counter.RequestCount += count;
            session.Save(counter);
        }

        public static void IncrementYearlyCounter(ISession session, int count)
        {
            DateTime now = DateTime.UtcNow;
            DateTime utcnow = new DateTime(now.Year, 1, 1);

            YearlyCounter counter = (YearlyCounter)session.CreateCriteria(typeof(YearlyCounter))
                .Add(Expression.Eq("DateTime", utcnow))
                .UniqueResult();

            if (counter == null)
            {
                counter = new YearlyCounter();
                counter.DateTime = utcnow;
                counter.RequestCount = 0;
            }

            counter.RequestCount += count;
            session.Save(counter);
        }

        public static void IncrementCounters(ISession session, int count)
        {
            IncrementHourlyCounter(session, count);
            IncrementDailyCounter(session, count);
            IncrementWeeklyCounter(session, count);
            IncrementMonthlyCounter(session, count);
            IncrementYearlyCounter(session, count);
        }
    }
}
