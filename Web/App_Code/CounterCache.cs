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
    static TimeSpan CounterFlushSpan = new TimeSpan(0, 0, 60);

    private DateTime mLastFlush = DateTime.UtcNow;
    private List<HttpRequest> mRequests = new List<HttpRequest>();
    private List<int> mPostCounters = new List<int>();
    private List<int> mImageCounters = new List<int>();

    private static CounterCache s_CounterCache = new CounterCache();

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
        if (mRequests == null)
            return;

        List<TransitBrowser> browsers = new List<TransitBrowser>();
        List<TransitReferrerHost> rhs = new List<TransitReferrerHost>();
        List<TransitReferrerSearchQuery> rsqs = new List<TransitReferrerSearchQuery>();

        // TODO: use a unique sorted collection

        foreach(HttpRequest request in mRequests)
        {
            try
            {
                if (request.Browser != null)
                {
                    TransitBrowser browser = new TransitBrowser();
                    browser.Name = request.Browser.Browser;
                    browser.Platform = request.Browser.Platform;
                    browser.Version = request.Browser.Version;
                    browsers.Add(browser);
                }

                string host = string.Empty;
                try
                {
                    host = request.Url.Host;
                }
                catch (ArgumentException)
                {
                    // host isn't available on localhost
                }

                string urlreferrer = string.Empty;
                try
                {
                    if (request.UrlReferrer != null)
                    {
                        urlreferrer = request.UrlReferrer.Host;
                    }
                }
                catch (ArgumentException)
                {
                    // referrer host not available
                }

                // don't track navigation between pages
                if (! string.IsNullOrEmpty(urlreferrer) && urlreferrer != host)
                {
                    TransitReferrerHost rh = new TransitReferrerHost();
                    rh.Name = request.UrlReferrer.Host;
                    rh.LastSource = request.UrlReferrer.ToString();
                    rh.LastUrl = request.RawUrl;
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
            catch (Exception ex)
            {
                manager.BlogService.EventLog.WriteEntry(string.Format("CreateOrUpdateStats for a single request failed. {0}",
                    ex.Message, EventLogEntryType.Warning));
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

        mRequests = new List<HttpRequest>();

        try
        {
            manager.BlogService.IncrementPostCounters(manager.Ticket, mPostCounters.ToArray());
        }
        catch (Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("IncrementPostCounters failed. {0}",
                ex.Message, EventLogEntryType.Error));
        }

        mPostCounters = new List<int>();

        try
        {
            manager.BlogService.IncrementImageCounters(manager.Ticket, mImageCounters.ToArray());
        }
        catch (Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("IncrementImageCounters failed. {0}",
                ex.Message, EventLogEntryType.Error));
        }

        mImageCounters = new List<int>();

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
        try
        {
            if (s_CounterCache.Expired)
            {
                lock (s_CounterCache)
                {
                    if (s_CounterCache.Expired)
                    {
                        s_CounterCache.Flush(manager);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            manager.BlogService.EventLog.WriteEntry(string.Format("GetCounterCache failed to flush the cache. {0}",
                ex.Message, EventLogEntryType.Error));
            
            s_CounterCache = new CounterCache();
        }

        return s_CounterCache;
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
