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
using DBlog.TransitData.References;

public partial class ShowComments : BlogAdminPage
{
    public void Page_Load(object sender, EventArgs e)
    {
        try
        {
            comments.OnGetDataSource += new EventHandler(comments_OnGetDataSource);
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

    public void comments_OnGetDataSource(object sender, EventArgs e)
    {
        comments.DataSource = SessionManager.GetCachedCollection<TransitAssociatedComment>(
            "GetAssociatedComments", SessionManager.PostTicket, new WebServiceQueryOptions(
                comments.PageSize, comments.CurrentPageIndex));
    }

    public void GetData(object sender, EventArgs e)
    {
        GetCommentsData(sender, e);
    }

    void GetCommentsData(object sender, EventArgs e)
    {
        comments.CurrentPageIndex = 0;
        comments.VirtualItemCount = SessionManager.GetCachedCollectionCount<TransitAssociatedComment>(
            "GetAssociatedCommentsCount", SessionManager.PostTicket, new WebServiceQueryOptions());
        comments_OnGetDataSource(sender, e);
        comments.DataBind();
    }

    public void comments_ItemCommand(object sender, DataGridCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName)
            {
                case "Delete":
                    SessionManager.BlogService.DeleteComment(SessionManager.Ticket,
                        int.Parse(e.CommandArgument.ToString()));
                    SessionManager.Invalidate<TransitImageComment>();
                    SessionManager.Invalidate<TransitPostComment>();
                    SessionManager.Invalidate<TransitComment>();
                    SessionManager.Invalidate<TransitAssociatedComment>();
                    GetData(sender, e);
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
