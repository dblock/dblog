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

public partial class ViewTopicsControl : BlogControl
{
    public class TopicChangedEventArgs : EventArgs
    {
        int mTopicId = 0;

        public int TopicId
        {
            get
            {
                return mTopicId;
            }
        }

        public TopicChangedEventArgs()
        {

        }

        public TopicChangedEventArgs(int id)
        {
            mTopicId = id;
        }
    }

    public delegate void TopicChangedHandler(object sender, TopicChangedEventArgs e);
    public event TopicChangedHandler TopicChanged;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            grid.OnGetDataSource += new EventHandler(grid_OnGetDataSource);

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

    public void  grid_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TopicChanged != null)
        {
            TopicChanged(sender, new TopicChangedEventArgs(TopicId));
        }
    }

    void grid_OnGetDataSource(object sender, EventArgs e)
    {
        grid.DataSource = SessionManager.BlogService.GetTopics(
            SessionManager.Ticket, null);
    }

    private void GetData(object sender, EventArgs e)
    {
        grid.CurrentPageIndex = 0;
        grid_OnGetDataSource(sender, e);
        grid.DataBind();
    }

    public int TopicId
    {
        get
        {
            if (grid.SelectedIndex < 0)
            {
                return 0;
            }

            return int.Parse(grid.SelectedItem.Cells[0].Text);
        }
    }
}
