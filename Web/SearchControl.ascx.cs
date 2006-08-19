using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.Data.Hibernate;
using System.Collections.Generic;
using DBlog.TransitData;

public partial class SearchControl : BlogControl
{
    public class SearchEventArgs : EventArgs
    {
        string mQuery = string.Empty;

        public string Query
        {
            get
            {
                return mQuery;
            }
        }

        public SearchEventArgs()
        {

        }

        public SearchEventArgs(string query)
        {
            mQuery = query;
        }
    }

    public delegate void SearchHandler(object sender, SearchEventArgs e);
    public event SearchHandler Search;

    public void inputSearch_Changed(object sender, EventArgs e)
    {
        if (Search != null)
        {
            Search(sender, new SearchEventArgs(inputSearch.Text));
        }
    }

    public string Query
    {
        get
        {
            return inputSearch.Text;
        }
    }
}
