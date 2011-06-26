using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using DBlog.Tools.Web;
using System.Web.Caching;
using System.Collections.Generic;
using DBlog.WebServices;
using DBlog.TransitData;
using DBlog.Data.Hibernate;
using System.Diagnostics;
using System.Text;
using DBlog.Tools;

public class SessionManager
{
    public const string sDBlogAuthCookieName = "DBlog.authcookie";
    public const string sDBlogPostCookieName = "DBlog.postcookie";

    const string sDBlogImpersonateCookieName = "DBlog.impersonatecookie";
    const string sDBlogRememberLogin = "DBlog.rememberlogin";

    private EventLog mEventLog = null;
    private Cache mCache = null;
    private HttpRequest mRequest = null;
    private HttpResponse mResponse = null;
    private string mTicket = string.Empty;
    private string mPostTicket = string.Empty;
    private Blog mBlogService = null;
    private TransitLogin mLoginRecord = null;
    private TransitLogin mPostLoginRecord = null;
    private TimeSpan mUtcOffset = TimeSpan.Zero;

    public class TypeCacheDependency<TransitType> : CacheDependency
    {
        public TypeCacheDependency()
            : base(null, new string[] { GetTypeCacheKey() }, null)
        {

        }

        public static string GetTypeCacheKey()
        {
            return string.Format("type:{0}", typeof(TransitType).Name);
        }
    }

    public TypeCacheDependency<TransitType> GetTransitTypeCacheDependency<TransitType>()
    {
        string key = TypeCacheDependency<TransitType>.GetTypeCacheKey();
        if (Cache[key] == null)
        {
            Cache[key] = DateTime.UtcNow;
#if DEBUG
                Debug.WriteLine(string.Format("Added cache dependency key: {0}", key));
#endif
        }
        return new TypeCacheDependency<TransitType>();
    }

    public Cache Cache
    {
        get
        {
            return mCache;
        }
    }

    public HttpRequest Request
    {
        get
        {
            return mRequest;
        }
    }

    public HttpResponse Response
    {
        get
        {
            return mResponse;
        }
    }

    public string WebsiteUrl
    {
        get
        {
            string url = GetSetting("url", String.Empty);
            if (!url.EndsWith("/")) url += "/";
            return url;
        }
    }

    public string GetSetting(string name, string defaultvalue)
    {
        object result = ConfigurationManager.AppSettings[name];

        if (result == null)
        {
            result = defaultvalue;
        }

        return result.ToString();
    }

    public SessionManager(System.Web.UI.Page page)
        : this(page.Cache, page.Request, page.Response)
    {

    }

    public SessionManager(System.Web.UI.MasterPage page)
        : this(page.Cache, page.Request, page.Response)
    {

    }

    public SessionManager(Cache cache, HttpRequest request, HttpResponse response)
    {
        mCache = cache;
        mRequest = request;
        mResponse = response;

        CacheTicket(sDBlogAuthCookieName, ref mTicket);
        CacheTicket(sDBlogPostCookieName, ref mPostTicket);

        if (!string.IsNullOrEmpty(mTicket) && string.IsNullOrEmpty(mPostTicket))
        {
            // use main ticket as default (avoid login twice for admins)
            mPostTicket = mTicket;
        }

        TimeZoneInformation.TryParseTimezoneRegionToTimeSpan(
            ConfigurationManager.AppSettings["region"], out mUtcOffset);
    }

    private void CacheTicket(string name, ref string ticket)
    {
        HttpCookie authcookie = Request.Cookies[name];
        if (authcookie != null)
        {
            try
            {
                // cache a verified ticket for an hour
                string key = string.Format("ticket:{0}", authcookie.Value);
                ticket = (string)Cache[key];
                if (string.IsNullOrEmpty(ticket))
                {
                    ManagedLogin.GetLoginId(authcookie.Value);
                    ticket = authcookie.Value;
                    Cache.Insert(key, ticket, null,
                        DateTime.Now.AddHours(1), TimeSpan.Zero);
                }
            }
            catch
            {

            }
        }
    }

    public void Invalidate<TransitType>()
    {
        string key = TypeCacheDependency<TransitType>.GetTypeCacheKey();
        Cache[key] = DateTime.UtcNow;
#if DEBUG
            Debug.WriteLine(string.Format("Invalidated cache dependency: {0}", key));
#endif
    }

    public string Ticket
    {
        get
        {
            if (string.IsNullOrEmpty(mTicket))
            {
                return string.Empty;
            }

            return mTicket;
        }
    }

    public string PostTicket
    {
        get
        {
            return mPostTicket;
        }
    }

    public void Logout()
    {
        HttpCookie impersonateCookie = Request.Cookies[sDBlogImpersonateCookieName];
        if (impersonateCookie != null)
        {
            Login(impersonateCookie.Value, false);
            Response.Cookies[sDBlogImpersonateCookieName].Value = string.Empty;
            Response.Cookies[sDBlogImpersonateCookieName].Expires = new DateTime(1970, 1, 1);
            return;
        }

        if (IsLoggedIn)
        {
            Cache.Remove(string.Format("ticket:{0}", Ticket));
            Cache.Remove(string.Format("login:{0}", Ticket));
            Response.Cookies[sDBlogAuthCookieName].Value = string.Empty;
            Response.Cookies[sDBlogAuthCookieName].Expires = new DateTime(1970, 1, 1);
            mTicket = string.Empty;
            mLoginRecord = null;
        }
    }

    public bool IsLoggedIn
    {
        get
        {
            return LoginRecord != null;
        }
    }

    public TransitLogin GetLoginRecord(string ticket)
    {
        TransitLogin result = null;

        if (!string.IsNullOrEmpty(ticket))
        {
            try
            {
                result = (TransitLogin)Cache[string.Format("login:{0}", ticket)];
                if (result == null)
                {
                    result = BlogService.GetLogin(ticket);
                    Cache.Insert(string.Format("login:{0}", ticket),
                        result, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
                }
            }
            catch
            {

            }
        }

        return result;
    }

    public TransitLogin LoginRecord
    {
        get
        {
            if (mLoginRecord == null)
            {
                mLoginRecord = GetLoginRecord(Ticket);
            }
            return mLoginRecord;
        }
    }

    public TransitLogin PostLoginRecord
    {
        get
        {
            if (mPostLoginRecord == null)
            {
                mPostLoginRecord = GetLoginRecord(PostTicket);
            }
            return mPostLoginRecord;
        }
    }

    public bool IsImpersonating
    {
        get
        {
            return Request.Cookies[sDBlogImpersonateCookieName] != null;
        }
    }

    public bool IsAdministrator
    {
        get
        {
            if (!IsLoggedIn)
                return false;

            TransitLogin t_login = LoginRecord;

            if (t_login == null)
                return false;

            return t_login.IsAdministrator;
        }
    }

    public void Impersonate(string newticket)
    {
        HttpCookie impersonateCookie = Request.Cookies[sDBlogImpersonateCookieName];
        if (impersonateCookie != null)
        {
            throw new Exception("You are already impersonating a user. Logout first.");
        }

        Response.Cookies[sDBlogImpersonateCookieName].Value = Ticket;

        Login(newticket, false);
    }

    public void Login(string ticket, bool rememberme)
    {
        Login(ticket, rememberme, sDBlogAuthCookieName);
    }

    public void Login(string ticket, bool rememberme, string name)
    {
        HttpCookie c = new HttpCookie(name);
        c.Value = ticket;
        if (rememberme)
        {
            c.Expires = DateTime.Today.AddYears(1);
        }
        Response.Cookies.Add(c);
    }

    public bool RememberLogin
    {
        get
        {
            HttpCookie c = Request.Cookies[sDBlogRememberLogin];
            if (c == null) return false;
            return bool.Parse(c.Value);
        }
        set
        {
            HttpCookie c = new HttpCookie(sDBlogRememberLogin);
            c.Value = value.ToString();
            Response.Cookies.Add(c);
        }
    }

    public Blog BlogService
    {
        get
        {
            if (mBlogService == null)
            {
                mBlogService = (DBlog.WebServices.Blog)HttpContext.Current.Cache["DBlog.SessionManager.BlogService"];
                if (mBlogService == null)
                {
                    mBlogService = new DBlog.WebServices.Blog();
                    HttpContext.Current.Cache["DBlog.SessionManager.BlogService"] = mBlogService;
                }
            }

            return mBlogService;
        }
    }

    public TransitType GetCachedObject<TransitType>(string invoke, string ticket, int id)
    {
        try
        {
            string key = string.Format("{0}:{1}:{2}",
                string.IsNullOrEmpty(ticket) ? 0 : ticket.GetHashCode(),
                invoke, id);

            TransitType result = (TransitType)Cache[key];
            
            if (result == null || IsAdministrator)
            {
                object[] args = { ticket, id };
                result = (TransitType)BlogService.GetType().GetMethod(invoke).Invoke(BlogService, args);
                Cache.Insert(key, result, GetTransitTypeCacheDependency<TransitType>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("{0}: {1}", invoke, ex.Message), ex);
        }
    }

    public TransitType GetCachedObject<TransitType>(string invoke, string ticket, WebServiceQueryOptions options)
    {
        try
        {
            string key = string.Format("{0}:{1}:{2}",
                string.IsNullOrEmpty(ticket) ? 0 : ticket.GetHashCode(),
                invoke, options.GetHashCode());

            TransitType result = (TransitType)Cache[key];

            if (result == null || IsAdministrator)
            {
                object[] args = { ticket, options };
                result = (TransitType)BlogService.GetType().GetMethod(invoke).Invoke(BlogService, args);
                Cache.Insert(key, result, GetTransitTypeCacheDependency<TransitType>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("{0}: {1}", invoke, ex.Message), ex);
        }
    }

    public List<TransitType> GetCachedCollection<TransitType>(
        string invoke, string ticket, WebServiceQueryOptions options)
    {
        try
        {
            string key = string.Format("{0}:{1}:{2}",
                string.IsNullOrEmpty(ticket) ? 0 : ticket.GetHashCode(),
                options == null ? 0 : options.GetHashCode(),
                invoke);

            List<TransitType> result = (List<TransitType>)Cache[key];

            if (result == null || IsAdministrator)
            {
                object[] args = { ticket, options };
                result = (List<TransitType>)BlogService.GetType().GetMethod(invoke).Invoke(BlogService, args);
                Cache.Insert(key, result, GetTransitTypeCacheDependency<TransitType>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("{0}: {1}", invoke, ex.Message), ex);
        }
    }

    public int GetCachedCollectionCount<TransitType>(
        string invoke, string ticket, WebServiceQueryOptions options)
    {
        try
        {
            string key = string.Format("{0}:{1}:{2}",
                string.IsNullOrEmpty(ticket) ? 0 : ticket.GetHashCode(),
                options == null ? 0 : options.GetHashCode(),
                invoke);

            object count = Cache[key];

            if (count == null || IsAdministrator)
            {
                object[] args = { ticket, options };
                count = BlogService.GetType().GetMethod(invoke).Invoke(BlogService, args);
                Cache.Insert(key, count, GetTransitTypeCacheDependency<TransitType>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
            }

            return (int) count;
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("{0}: {1}", invoke, ex.Message), ex);
        }
    }

    public bool CountersEnabled
    {
        get
        {
            return HostedApplication.IsEnabled("Counters");
        }
    }

    public EventLog EventLog
    {
        get
        {
            if (mEventLog == null && HostedApplication.EventLogEnabled)
            {
                mEventLog = HostedApplication.CreateEventLog();
            }
            return mEventLog;
        }
    }

    public void EventLogWriteEntry(string message, EventLogEntryType type)
    {
        if (HostedApplication.EventLogEnabled)
        {
            EventLog.WriteEntry(message, type);
        }
    }

    /// <summary>
    /// Do basic authentication.
    /// </summary>
    public bool BasicAuth()
    {
        string authHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
            return false;
        if (! authHeader.StartsWith("basic ", StringComparison.InvariantCultureIgnoreCase))
            return false;
        string userNameAndPassword = Encoding.Default.GetString(Convert.FromBase64String(authHeader.Substring(6)));
        string[] parts = userNameAndPassword.Split(':');
        if (parts.Length != 2) throw new ManagedLogin.AccessDeniedException();
        mTicket = mPostTicket = BlogService.Login(parts[0], parts[1]);
        return true;
    }

    public string BasicAuthRealm
    {
        get
        {
            return "DBlog";
        }
    }

    public DateTime Adjust(DateTime dt)
    {
        return dt.Add(mUtcOffset);
    }

    public DateTime ToUTC(DateTime dt)
    {
        return dt.Add(-mUtcOffset);
    }
}
