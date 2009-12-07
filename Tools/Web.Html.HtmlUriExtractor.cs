using System;
using System.Globalization;
using System.IO;
using Sgml;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace DBlog.Tools.Web.Html
{
    public class HtmlUri
    {
        public Uri Uri;
        public HtmlGenericControl Control;

        public HtmlUri(Uri uri, HtmlGenericControl control)
        {
            Uri = uri;
            Control = control;
        }
    }

    /// <summary>
    /// This class extracts all links from an HTML body.
    /// </summary>
    public class HtmlUriExtractor : HtmlUrlBasedExtractor
    {
        private List<HtmlUri> mUris = new List<HtmlUri>();

        public List<HtmlUri> Uris
        {
            get
            {
                return mUris;
            }
            set
            {
                mUris = value;
            }
        }

        public HtmlUriExtractor(TextReader reader)
            : this(reader, null)
        {

        }

        private static string[] tags = { "a", "link" };

        public HtmlUriExtractor(string content)
            : this(content, null)
        {

        }

        public HtmlUriExtractor(string content, Uri root)
            : base(tags, content, root)
        {

        }

        public HtmlUriExtractor(TextReader reader, Uri root)
            : base(tags, reader, root)
        {

        }

        protected override void OnTagProcessed(HtmlGenericControl tag)
        {
            string href = tag.Attributes["href"];

            if (string.IsNullOrEmpty(href))
                return;

            Uri uri = null;

            if (BaseHref != null)
            {
                if (Uri.TryCreate(BaseHref, href, out uri))
                    mUris.Add(new HtmlUri(uri, tag));
            }
            else
            {
                if (Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out uri))
                    mUris.Add(new HtmlUri(uri, tag));
            }
        }

        public static List<HtmlUri> Extract(string html)
        {
            return Extract(html, null);
        }

        public static List<HtmlUri> Extract(string html, Uri root)
        {
            HtmlUriExtractor ex = new HtmlUriExtractor(html, root);
            while (!ex.EOF) ex.Read();
            return ex.Uris;
        }

        public static string TryCreate(Uri baseuri, string relativeuri)
        {
            return TryCreate(baseuri, relativeuri, relativeuri);
        }

        public static string TryCreate(Uri baseuri, string relativeuri, string defaultvalue)
        {
            try
            {
                Uri result = null;
                if (Uri.TryCreate(baseuri, relativeuri, out result))
                    return result.OriginalString;
            }
            catch (UriFormatException)
            {
                // TryCreate chokes on "mailto:foo (at) bar.com"
            }

            return defaultvalue;
        }
    }
}
