using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitStatsQueryOptions : WebServiceQueryOptions
    {
        private TransitStats.Type mType = TransitStats.Type.Hourly;

        public TransitStats.Type Type
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

        public TransitStatsQueryOptions(TransitStats.Type type)
        {
            Type = type;
        }
    }

    public class TransitStats : TransitObject
    {
        private int mPostsCount = 0;

        public int PostsCount
        {
            get
            {
                return mPostsCount;
            }
            set
            {
                mPostsCount = value;
            }
        }

        private int mImagesCount = 0;

        public int ImagesCount
        {
            get
            {
                return mImagesCount;
            }
            set
            {
                mImagesCount = value;
            }
        }

        private int mCommentsCount = 0;

        public int CommentsCount
        {
            get
            {
                return mCommentsCount;
            }
            set
            {
                mCommentsCount = value;
            }
        }

        private TransitCounter mRssCount = null;

        public TransitCounter RssCount
        {
            get
            {
                return mRssCount;
            }
            set
            {
                mRssCount = value;
            }
        }

        private TransitCounter mAtomCount = null;

        public TransitCounter AtomCount
        {
            get
            {
                return mAtomCount;
            }
            set
            {
                mAtomCount = value;
            }
        }

        public enum Type
        {
            Hourly = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Yearly = 4
        }

        public static List<TransitCounter> GetSummaryHourly(ISession session)
        {
            List<TransitCounter> result = new List<TransitCounter>();
            DateTime now = DateTime.UtcNow;
            DateTime ts = now.AddHours(-24);
            while (ts <= now)
            {
                DateTime ts_current = new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0);
                HourlyCounter c = (HourlyCounter)session.CreateCriteria(typeof(HourlyCounter))
                    .Add(Expression.Eq("DateTime", ts_current))
                    .UniqueResult();

                result.Add((c == null) ? new TransitCounter(ts_current, 0) : new TransitCounter(c.DateTime, c.RequestCount));
                ts = ts.AddHours(1);
            }

            return result;
        }

        public static List<TransitCounter> GetSummaryDaily(ISession session)
        {
            List<TransitCounter> result = new List<TransitCounter>();
            DateTime now = DateTime.UtcNow;
            DateTime ts = now.AddDays(-14);
            while (ts <= now)
            {
                DateTime ts_current = new DateTime(ts.Year, ts.Month, ts.Day, 0, 0, 0);
                DailyCounter c = (DailyCounter)session.CreateCriteria(typeof(DailyCounter))
                    .Add(Expression.Eq("DateTime", ts_current))
                    .UniqueResult();

                result.Add((c == null) ? new TransitCounter(ts_current, 0) : new TransitCounter(c.DateTime, c.RequestCount));
                ts = ts.AddDays(1);
            }

            return result;
        }

        public static List<TransitCounter> GetSummaryWeekly(ISession session)
        {
            List<TransitCounter> result = new List<TransitCounter>();
            DateTime now = DateTime.UtcNow;
            DateTime ts = now.AddMonths(-2);

            while (ts.DayOfWeek != DayOfWeek.Sunday)
                ts = ts.AddDays(-1);

            while (ts <= now)
            {
                DateTime ts_current = new DateTime(ts.Year, ts.Month, ts.Day, 0, 0, 0);
                WeeklyCounter c = (WeeklyCounter)session.CreateCriteria(typeof(WeeklyCounter))
                    .Add(Expression.Eq("DateTime", ts_current))
                    .UniqueResult();

                result.Add((c == null) ? new TransitCounter(ts_current, 0) : new TransitCounter(c.DateTime, c.RequestCount));
                ts = ts.AddDays(7);
            }

            return result;
        }

        public static List<TransitCounter> GetSummaryMonthly(ISession session)
        {
            List<TransitCounter> result = new List<TransitCounter>();
            DateTime now = DateTime.UtcNow;
            DateTime ts = now.AddMonths(-12);

            while (ts <= now)
            {
                DateTime ts_current = new DateTime(ts.Year, ts.Month, 1, 0, 0, 0);
                MonthlyCounter c = (MonthlyCounter)session.CreateCriteria(typeof(MonthlyCounter))
                    .Add(Expression.Eq("DateTime", ts_current))
                    .UniqueResult();

                result.Add((c == null) ? new TransitCounter(ts_current, 0) : new TransitCounter(c.DateTime, c.RequestCount));
                ts = ts.AddMonths(1);
            }

            return result;
        }

        public static List<TransitCounter> GetSummaryYearly(ISession session)
        {
            List<TransitCounter> result = new List<TransitCounter>();
            DateTime now = DateTime.UtcNow;

            for (int i = -5; i <= 0; i++)
            {
                DateTime ts_current = new DateTime(now.Year + i, 1, 1, 0, 0, 0);
                YearlyCounter c = (YearlyCounter)session.CreateCriteria(typeof(YearlyCounter))
                    .Add(Expression.Eq("DateTime", ts_current))
                    .UniqueResult();

                result.Add((c == null) ? new TransitCounter(ts_current, 0) : new TransitCounter(c.DateTime, c.RequestCount));
            }

            return result;
        }

        public TransitStats(ISession session)
        {
            ImagesCount = (int)session.CreateQuery("SELECT COUNT(i) FROM Image i").UniqueResult();
            PostsCount = (int)session.CreateQuery("SELECT COUNT(p) FROM Post p").UniqueResult();
            CommentsCount = (int)session.CreateQuery("SELECT COUNT(c) FROM Comment c").UniqueResult();
            AtomCount = TransitCounter.GetNamedCounter(session, "Atom");
            RssCount = TransitCounter.GetNamedCounter(session, "Rss");
        }
    }
}
