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
using DBlog.Tools.Web;
using NHibernate;

namespace DBlog.TransitData.References
{
    public abstract class ExternalRenderer
    {
        protected int mId = 0;
        protected string mType = null;
        protected string mRootUrl = null;
        protected ISession mSession = null;

        public ExternalRenderer(ISession session, int id, string type)
        {
            mSession = session;
            mId = id;
            mType = type;
        }

        public abstract string Render(string value);

        protected string ReferUrl(string url, string content)
        {
            return string.Format("<a target='_blank' href='{0}ShowUrl.aspx?ObjectId={1}&amp;ObjectType={2}&amp;Url={3}'>{4}</a>",
                ConfigurationManager.AppSettings["url"],
                mId,
                mType,
                Renderer.UrlEncode(url),
                content);
        }
    }
}