using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Caching;

public class CounterCache
{
    static TimeSpan CounterFlushSpan = new TimeSpan(0, 5, 0);
    static string CounterCacheKey = "__countercache";

    private DateTime mLastFlush;
    private int mCounter = 0;

    public DateTime LastFlush
    {
        get
        {
            return mLastFlush;
        }
        set
        {
            mLastFlush = value;
        }
    }

    public int Counter
    {
        get
        {
            return mCounter;
        }
        set
        {
            mCounter = value;
        }
    }

    public int Increment()
    {
        return (++mCounter);
    }

    public int Increment(int count)
    {
        return (mCounter += count);
    }

    public CounterCache()
    {
        mCounter = 0;
        mLastFlush = DateTime.UtcNow;
    }

    public bool Expired
    {
        get
        {
            return LastFlush.Add(CounterFlushSpan) <= DateTime.UtcNow;
        }
    }

    public void Flush(SessionManager manager)
    {
        manager.BlogService.IncrementHourlyCounter(manager.Ticket, Counter);
        Counter = 0;
        LastFlush = DateTime.UtcNow;
    }

    public static int Increment(Cache cache, SessionManager manager)
    {
        CounterCache cc = (CounterCache) cache[CounterCacheKey];

        if (cc == null)
        {
            cc = new CounterCache();
            cc.Counter = 0;
            cache.Insert(CounterCacheKey, cc);
        }

        int result = cc.Increment();

        if (cc.Expired)
        {
            cc.Flush(manager);
        }

        return result;
    }
}
