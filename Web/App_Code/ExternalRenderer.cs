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
using DBlog.TransitData;
using System.Collections.Generic;
using System.Web.Caching;
using DBlog.WebServices;
using DBlog.Tools.Web;

public abstract class ExternalRenderer
{
    protected BlogPage mPage = null;
    protected int mId = 0;
    protected string mType = null;

    public ExternalRenderer(BlogPage page, int id, string type)
    {
        mPage = page;
        mId = id;
        mType = type;
    }

    public abstract string Render(string value);

    protected string ReferUrl(string url, string content)
    {
        return string.Format("<a target='_blank' href='{0}ShowUrl.aspx?ObjectId={1}&amp;ObjectType={2}&amp;Url={3}'>{4}</a>",
            mPage.SessionManager.GetSetting("url", string.Empty),
            mId,
            mType,
            Renderer.UrlEncode(url),
            content);
    }
}
