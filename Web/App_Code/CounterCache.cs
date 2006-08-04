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
using System.Diagnostics;

public class CounterCache
{
    static TimeSpan CounterFlushSpan = new TimeSpan(0, 1, 0);
    static string CounterCacheKey = "__countercache";

    private DateTime mLastFlush = DateTime.UtcNow;
    private List<HttpRequest> mRequests = new List<HttpRequest>();
    private List<int> mPostCounters = new List<int>();
    private List<int> mImageCounters = new List<int>();

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
        List<TransitBrowser> browsers = new List<TransitBrowser>();
        List<TransitReferrerHost> rhs = new List<TransitReferrerHost>();
        List<TransitReferrerSearchQuery> rsqs = new List<TransitReferrerSearchQuery>();

        // TODO: use a unique sorted collection

        foreach(HttpRequest request in mRequests)
        {
            TransitBrowser browser = new TransitBrowser();
            browser.Name = request.Browser.Browser;
            browser.Platform = request.Browser.Platform;
            browser.Version = request.Browser.Version;
            browsers.Add(browser);

            // don't track navigation between pages
            if (request.UrlReferrer != null && request.UrlReferrer.Host != request.Url.Host)
            {
                TransitReferrerHost rh = new TransitReferrerHost();
                rh.Name = request.UrlReferrer.Host;
                rh.LastSource = request.UrlReferrer.ToString();
                rh.LastUrl = request.Url.ToString();
                rh.RequestCount = 1;
                rhs.Add(rh);

                string q = request.QueryString["q"];
                if (string.IsNullOrEmpty(q)) q = request.QueryString["s"];
                if (string.IsNullOrEmpty(q)) q = request.QueryString["search"];
                if (string.IsNullOrEmpty(q)) q = request.QueryString["query"];

                if (!string.IsNullOrEmpty(q))
                {
                    TransitReferrerSearchQuery trsq = new TransitReferrerSearchQuery();
                    trsq.RequestCount = 1;
                    trsq.SearchQuery = q;
                    rsqs.Add(trsq);
                }                    
            }
        }

        try
        {
            manager.BlogService.CreateOrUpdateStats(
                manager.Ticket, browsers.ToArray(), rhs.ToArray(), rsqs.ToArray());
        }
        catch (Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("CreateOrUpdateStats failed. {0}",
                ex.Message, EventLogEntryType.Error));            
        }

        mRequests.Clear();

        try
        {
            manager.BlogService.IncrementPostCounters(manager.Ticket, mPostCounters.ToArray());
        }
        catch (Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("IncrementPostCounters failed. {0}",
                ex.Message, EventLogEntryType.Error));
        }

        mPostCounters.Clear();

        try
        {
            manager.BlogService.IncrementImageCounters(manager.Ticket, mImageCounters.ToArray());
        }
        catch (Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("IncrementImageCounters failed. {0}",
                ex.Message, EventLogEntryType.Error));
        }

        mImageCounters.Clear();

        LastFlush = DateTime.UtcNow;
    }

    public int Add(HttpRequest request)
    {
        mRequests.Add(request);
        return mRequests.Count;
    }

    public int AddPostCounter(int id)
    {
        mPostCounters.Add(id);
        return mPostCounters.Count;
    }

    public int AddImageCounter(int id)
    {
        mImageCounters.Add(id);
        return mImageCounters.Count;
    }

    public static CounterCache GetCounterCache(Cache cache, SessionManager manager)
    {
        CounterCache cc = (CounterCache)cache[CounterCacheKey];

        if (cc == null)
        {
            cc = new CounterCache();
            cache.Insert(CounterCacheKey, cc);
        }

        if (cc.Expired)
        {
            lock (cc)
            {
                if (cc.Expired)
                {
                    cc.Flush(manager);
                }
            }
        }

        return cc;
    }

    public static int Increment(HttpRequest request, Cache cache, SessionManager manager)
    {
        return GetCounterCache(cache, manager).Add(request);
    }

    public static int IncrementImageCounter(int id, Cache cache, SessionManager manager)
    {
        return GetCounterCache(cache, manager).AddImageCounter(id);
    }

    public static int IncrementPostCounter(int id, Cache cache, SessionManager manager)
    {
        return GetCounterCache(cache, manager).AddPostCounter(id);
    }
}
