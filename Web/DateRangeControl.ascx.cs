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

public partial class DateRangeControl : BlogControl
{
    public class DateRangeEventArgs : EventArgs
    {
        DateTime mDateStart = DateTime.MinValue;
        DateTime mDateEnd = DateTime.MinValue;

        public DateTime DateStart
        {
            get
            {
                return mDateStart;
            }
        }

        public DateTime DateEnd
        {
            get
            {
                return mDateEnd;
            }
        }

        public DateRangeEventArgs()
        {

        }

        public DateRangeEventArgs(DateTime start, DateTime end)
        {
            mDateStart = start;
            mDateEnd = end;
        }
    }

    public delegate void DateRangeHandler(object sender, DateRangeEventArgs e);
    public event DateRangeHandler DateRangeChanged;

    public void inputDateRange_Changed(object sender, EventArgs e)
    {
        if (DateRangeChanged != null)
        {
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;

            foreach (DateTime dt in inputCalendar.SelectedDates)
            {
                if (start == DateTime.MinValue || dt < start) start = dt;
                if (end == DateTime.MaxValue || dt > end) end = dt.AddDays(1);
            }

            if (start != DateTime.MinValue) start = SessionManager.Region.UserToUtc(start);
            if (end != DateTime.MaxValue) end = SessionManager.Region.UserToUtc(end);

            DateRangeChanged(sender, new DateRangeEventArgs(start, end));
        }
    }

    public void reset_Click(object sender, EventArgs e)
    {
        inputCalendar.SelectedDates.Clear();
        inputDateRange_Changed(sender, e);
    }

    public void today_Click(object sender, EventArgs e)
    {
        inputCalendar.SelectedDates.Clear();
        inputCalendar.SelectedDate = DateTime.UtcNow;
        inputDateRange_Changed(sender, e);
    }

}
