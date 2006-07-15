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

public class SessionManager
{
    const string sDBlogAuthCookieName = "DBlog.authcookie";
    const string sDBlogImpersonateCookieName = "DBlog.impersonatecookie";
    const string sDBlogRememberLogin = "DBlog.rememberlogin";

    private Cache mCache = null;
    private HttpRequest mRequest = null;
    private HttpResponse mResponse = null;
    private string mTicket = string.Empty;
    private Blog mBlogService = null;
    private TransitLogin mLoginRecord = null;

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
            return GetSetting("url", "http://localhost/DBlog");
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

        HttpCookie authcookie = Request.Cookies[sDBlogAuthCookieName];
        if (authcookie != null)
        {
            try
            {
                // cache a verified ticket for an hour
                mTicket = (string) Cache[string.Format("ticket:{0}", authcookie.Value)];
                if (string.IsNullOrEmpty(mTicket))
                {
                    ManagedLogin.GetLoginId(authcookie.Value);
                    mTicket = authcookie.Value;
                    Cache.Insert(string.Format("ticket:{0}", authcookie.Value),
                        mTicket, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
                }
            }
            catch
            {

            }
        }
    }

    public string Ticket
    {
        get
        {
            return mTicket;
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

    private TransitLogin LoginRecord
    {
        get
        {
            if (mLoginRecord == null)
            {
                if (! string.IsNullOrEmpty(Ticket))
                {
                    try
                    {
                        mLoginRecord = (TransitLogin) Cache[string.Format("login:{0}", Ticket)];
                        if (mLoginRecord == null)
                        {
                            mLoginRecord = BlogService.GetLogin(Ticket);
                            Cache.Insert(string.Format("login:{0}", Ticket),
                                mLoginRecord, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return mLoginRecord;
        }
    }

    public bool IsImpersonating
    {
        get
        {
            return Request.Cookies[sDBlogImpersonateCookieName] != null;
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
        HttpCookie c = new HttpCookie(sDBlogAuthCookieName);
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
}
