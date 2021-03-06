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
using DBlog.TransitData;
using System.Text;
using DBlog.Tools.Web;
using DBlog.Data.Hibernate;

public partial class ShowImages : BlogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            images.OnGetDataSource += new EventHandler(images_OnGetDataSource);

            if (!IsPostBack)
            {
                GetData(sender, e);
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void GetData(object sender, EventArgs e)
    {
        images.CurrentPageIndex = 0;
        TransitPostImageQueryOptions options = GetOptions();
        images.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitPostImage>(
            "GetPostImagesCountEx", SessionManager.PostTicket, options);
        images_OnGetDataSource(sender, e);
        images.DataBind();
    }

    void images_OnGetDataSource(object sender, EventArgs e)
    {
        TransitPostImageQueryOptions options = GetOptions();
        images.DataSource = SessionManager.GetCachedCollection<TransitPostImage>(
            "GetPostImagesEx", SessionManager.PostTicket, options);
    }

    private TransitPostImageQueryOptions GetOptions()
    {
        TransitPostImageQueryOptions options = new TransitPostImageQueryOptions();
        options.PageNumber = images.CurrentPageIndex;
        options.PageSize = images.PageSize;
        options.SortExpression = "Counter.Count";
        options.SortDirection = WebServiceQuerySortDirection.Descending;
        options.Counters = true;
        return options;
    }

    public string GetComments(TransitImage image)
    {
        if (image.CommentsCount == 0)
            return string.Empty;

        return string.Format("{0} Comment{1}", image.CommentsCount,
            image.CommentsCount != 1 ? "s" : string.Empty);
    }

    public string GetCounter(TransitImage image)
    {
        if (!SessionManager.CountersEnabled)
            return string.Empty;

        if (image.Counter.Count == 0)
            return string.Empty;

        return string.Format("[{0}]", image.Counter.Count);
    }
}
