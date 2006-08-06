using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using WebChart;
using System.Collections.Generic;
using System.Drawing;
using DBlog.Tools.Web;
using DBlog.TransitData;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

public partial class StatsChart : BlogPicturePage
{
    public TransitStats.Type RequestType
    {
        get
        {
            return (TransitStats.Type)Enum.Parse(typeof(TransitStats.Type), Request.Params["type"]);
        }
    }

    public override PicturePage.PicturePageType PageType
    {
        get
        {
            return PicturePageType.Bitmap;
        }
    }

    public override PicturePage.Picture GetPictureWithBitmap(int id, DateTime ifModifiedSince)
    {
        return GetPictureWithBitmap(id);
    }

    public override PicturePage.Picture GetPictureWithThumbnail(int id, DateTime ifModifiedSince)
    {
        throw new NotImplementedException();
    }

    public override PicturePage.Picture GetPictureWithBitmap(int id)
    {
        ChartEngine engine = new ChartEngine();
        engine.Size = new Size(570, 300);
        engine.GridLines = WebChart.GridLines.None;
        engine.ShowXValues = true;
        engine.ShowYValues = true;
        engine.LeftChartPadding = 50;
        engine.BottomChartPadding = 50;
        engine.XAxisFont.StringFormat.LineAlignment = StringAlignment.Center;
        engine.XAxisFont.StringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
        engine.XAxisFont.ForeColor = engine.YAxisFont.ForeColor = Color.Black;

        ChartCollection charts = new ChartCollection(engine);
        engine.Charts = charts;

        List<TransitCounter> counters = SessionManager.GetCachedCollection<TransitCounter>(
            "GetStatsSummary", SessionManager.Ticket, new TransitStatsQueryOptions(RequestType));

        string format;

        switch (RequestType)
        {
            case TransitStats.Type.Daily:
                format = "MMM d";
                break;
            case TransitStats.Type.Hourly:
                format = "htt";
                break;
            case TransitStats.Type.Monthly:
                format = "MMM";
                break;
            case TransitStats.Type.Weekly:
                format = "MMM dd";
                break;
            case TransitStats.Type.Yearly:
                format = "yyyy";
                break;
            default:
                throw new ArgumentOutOfRangeException("type");
        }

        Color fill = Color.Black;

        ColumnChart chart = new ColumnChart();
        chart.ShowLineMarkers = false;
        chart.ShowLegend = true;
        chart.Line.Color = Color.White;
        chart.Line.Width = 2;
        chart.Fill.Color = engine.Border.Color = fill;
        chart.Fill.LinearGradientMode = LinearGradientMode.Vertical;
        chart.MaxColumnWidth = 100;

        foreach (TransitCounter counter in counters)
        {
            chart.Data.Add(new ChartPoint(counter.Created.ToString(format), counter.Count));
        }

        charts.Add(chart);

        PicturePage.Picture picture = new PicturePage.Picture();
        picture.Id = 0;
        picture.Modified = DateTime.UtcNow;
        picture.Name = RequestType.ToString();

        MemoryStream ds = new MemoryStream();
        engine.GetBitmap().Save(ds, ImageFormat.Png);
        picture.Bitmap = new byte[ds.Length];
        MemoryStream ms = new MemoryStream(picture.Bitmap);
        ds.WriteTo(ms);

        return picture;
    }

    public override PicturePage.Picture GetPictureWithThumbnail(int id)
    {
        throw new NotImplementedException();
    }

    public override PicturePage.Picture GetRandomPictureWithThumbnail()
    {
        throw new NotImplementedException();
    }
}
