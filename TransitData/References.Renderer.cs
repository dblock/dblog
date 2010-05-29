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
using NHibernate.Criterion;
using DBlog.Data;

namespace DBlog.TransitData.References
{
    public class ReferencesRenderer : ExternalRenderer
    {
        public ReferencesRenderer(ISession session)
            : base(session)
        {

        }

        private string RefHandler(Match ParameterMatch)
        {
            string word = ParameterMatch.Value;

            Reference reference = (Reference) mSession.CreateCriteria(typeof(Reference))
                .Add(Expression.Eq("Word", word)).UniqueResult();

            if (reference == null)
            {
                return word;
            }

            string image = "link";

            if (reference.Url.IndexOf("citysearch.com/") > 0)
                image = "citysearch";

            string content = string.Format("<img src='{0}images/links/{1}.gif' border='0' align='absmiddle' width='16' height='16' /> {2}",
                ConfigurationManager.AppSettings["url"],
                image,
                reference.Result);

            return ReferUrl(reference.Url, content);
        }

        private static Regex RefExpression = new Regex(@"\w+\:\w+", RegexOptions.IgnoreCase);

        public override string Render(string value)
        {
            MatchEvaluator RefHandlerDelegate = new MatchEvaluator(RefHandler);
            return RefExpression.Replace(value, RefHandlerDelegate);
        }
    }
}