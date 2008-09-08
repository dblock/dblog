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
    public class LiveJournalRenderer : ExternalRenderer
    {
        public LiveJournalRenderer(ISession session)
            : base(session)
        {

        }

        static Regex LjExpression = new Regex("lj:(?<type>\\w+)=(?<name>\\w+)", RegexOptions.IgnoreCase);

        public override string Render(string value)
        {
            MatchEvaluator LjHandlerDelegate = new MatchEvaluator(LjHandler);
            return LjExpression.Replace(value, LjHandlerDelegate);
        }

        string LjHandler(Match ParameterMatch)
        {
            string type = ParameterMatch.Groups["type"].Value;
            string name = ParameterMatch.Groups["name"].Value;
            string path;

            switch (type)
            {
                case "user":
                case "rss":
                    path = "users";
                    break;
                case "community":
                    path = "community";
                    break;
                default:
                    return "[error: unknown lj type " + type + "]";
            }

            string url = string.Format("http://www.livejournal.com/{0}/{1}/", path, name);
            string content = string.Format(
                "<img src='{0}images/links/lj{1}.gif' border='0' align='absmiddle' width='16' height='16'/> {2}",
                    ConfigurationManager.AppSettings["url"],
                    type,
                    name);

            return ReferUrl(url, content);
        }
    }
}