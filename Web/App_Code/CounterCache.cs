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
using System.Collections.Generic;
using DBlog.TransitData;

public class CounterCache
{
    static TimeSpan CounterFlushSpan = new TimeSpan(0, 5, 0);
    static string CounterCacheKey = "__countercache";

    private DateTime mLastFlush;
    private List<HttpRequest> mRequests = new List<HttpRequest>();

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

    public CounterCache()
    {
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
        manager.BlogService.IncrementCounters(manager.Ticket, mRequests.Count);

        foreach(HttpRequest request in mRequests)
        {
            TransitBrowser browser = new TransitBrowser();
            browser.Name = request.Browser.Browser;
            browser.Platform = request.Browser.Platform;
            browser.Version = request.Browser.Version;

            browser.Id = manager.BlogService.CreateOrUpdateBrowser(manager.Ticket, browser);
            manager.BlogService.IncrementBrowserCounter(manager.Ticket, browser.Id);
        }

        mRequests.Clear();
        LastFlush = DateTime.UtcNow;
    }

    public int Add(HttpRequest request)
    {
        mRequests.Add(request);
        return mRequests.Count;
    }

    public static int Increment(HttpRequest request, Cache cache, SessionManager manager)
    {
        CounterCache cc = (CounterCache) cache[CounterCacheKey];

        if (cc == null)
        {
            cc = new CounterCache();
            cache.Insert(CounterCacheKey, cc);
        }


        int result = cc.Add(request);

        if (cc.Expired)
        {
            cc.Flush(manager);
        }

        return result;
    }
}
