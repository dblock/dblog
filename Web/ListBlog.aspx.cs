using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using DBlog.TransitData;
using DBlog.Tools.Web;
using System.Text;
using DBlog.Data.Hibernate;

public partial class ListBlog : BlogPage
{
    private const int PageSize = 100;

    protected override void OnLoad(EventArgs e)
    {
        if (Header != null)
        {
            HtmlMeta noindex = new HtmlMeta();
            noindex.Name = "robots";
            noindex.Content = "noindex";
            Header.Controls.Add(noindex);
        }

        base.OnLoad(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GetData(sender, e);
    }

    public int CurrentPageIndex
    {
        get
        {
            int page = 0;
            int.TryParse(Request["page"], out page);
            return page;
        }
    }

    public void GetData(object sender, EventArgs e)
    {
        // total number of items
        int total = SessionManager.GetCachedCollectionCount("GetPostsCount",
            SessionManager.PostTicket, null);
        // number of items left
        int left = total - ((CurrentPageIndex + 1) * PageSize);
        // previous link
        linkPrev.Enabled = (CurrentPageIndex > 0);
        if (CurrentPageIndex > 0)
        {
            linkPrev.Text = string.Format("&#171; Prev {0}", PageSize);
            linkPrev.NavigateUrl = string.Format("ListBlog.aspx?page={0}", CurrentPageIndex - 1);
        }
        // next link
        linkNext.Enabled = (left > 0);
        if (left > 0)
        {
            linkNext.NavigateUrl = string.Format("ListBlog.aspx?page={0}", CurrentPageIndex + 1);
            linkNext.Text = string.Format("Next {0} &#187;", PageSize > left ? left : PageSize);
        }
        TransitPostQueryOptions options = new TransitPostQueryOptions(PageSize, CurrentPageIndex);
        options.SortDirection = WebServiceQuerySortDirection.Descending;
        options.SortExpression = "Created";
        grid.DataSource = SessionManager.GetCachedCollection<TransitPost>(
            "GetPosts", SessionManager.PostTicket, options);
        grid.DataBind();
    }
}
