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
using DBlog.WebServices;

public partial class StatsHits : BlogAdminPage
{
    public void Page_Load()
    {
        try
        {
            if (!IsPostBack)
            {
                object type = Request.Params["type"];
                SetChartType(type == null ? TransitStats.Type.Daily : (TransitStats.Type)Enum.Parse(typeof(TransitStats.Type), type.ToString()));
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    void SetChartType(TransitStats.Type type)
    {
        try
        {
            imageStats.Src = string.Format("StatsChart.aspx?type={0}", type);
            labelChartType.Text = type.ToString();

            linkDaily.Enabled = (type != TransitStats.Type.Daily);
            linkHourly.Enabled = (type != TransitStats.Type.Hourly);
            linkMonthly.Enabled = (type != TransitStats.Type.Monthly);
            linkYearly.Enabled = (type != TransitStats.Type.Yearly);
            linkWeekly.Enabled = (type != TransitStats.Type.Weekly);
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void linkYearly_Click(object sender, EventArgs e)
    {
        SetChartType(TransitStats.Type.Yearly);
    }

    public void linkHourly_Click(object sender, EventArgs e)
    {
        SetChartType(TransitStats.Type.Hourly);
    }

    public void linkMonthly_Click(object sender, EventArgs e)
    {
        SetChartType(TransitStats.Type.Monthly);
    }

    public void linkWeekly_Click(object sender, EventArgs e)
    {
        SetChartType(TransitStats.Type.Weekly);
    }

    public void linkDaily_Click(object sender, EventArgs e)
    {
        SetChartType(TransitStats.Type.Daily);
    }
}
