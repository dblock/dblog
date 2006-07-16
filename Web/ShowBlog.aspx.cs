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
using System.Text.RegularExpressions;
using DBlog.TransitData;

public partial class ShowBlog : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DBlogMaster master = (DBlogMaster) this.Master;
        master.TopicChanged += new TopicsViewControl.TopicChangedHandler(topics_TopicChanged);
    }

    public void topics_TopicChanged(object sender, TopicsViewControl.TopicChangedEventArgs e)
    {
        blogs.TopicId = e.TopicId;
        blogs.GetData(sender, e);
        panelBlogs.Update();
    }
}