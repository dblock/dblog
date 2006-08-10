using System;
using System.Configuration;

namespace DBlog.Tools.Globalization
{
    public class Region
    {
        public const string UnknownArea = "Unknown";
        public const int UnknownOffset = 0xFF;

        private string mArea = UnknownArea;
        private int mOffset = UnknownOffset;

        public string Area
        {
            get
            {
                return mArea;
            }
            set
            {
                mArea = value;
            }
        }

        public int Offset
        {
            get
            {
                return mOffset;
            }
            set
            {
                mOffset = value;
            }
        }

        public Region(string area, int OffsetValue)
        {
            Area = area;
            Offset = OffsetValue;
        }

        public override string ToString()
        {
            return Offset == UnknownOffset ? Area : Area + " (UTC" + Offset.ToString() + ")";
        }


        public static Region[] Regions = { 
            new Region("Eniwetok", -12),
            new Region("Samoa", -11),
            new Region("Hawaii", -10),
            new Region("Alaska", -9),
            new Region("PST, Pacific US", -8),
            new Region("MST, Mountain US", -7),
            new Region("CST, Central US", -6),
            new Region("EST, Eastern US", -5),
            new Region("Atlantic, Canada", -4),
            new Region("Brazilia, Buenos Aries", -3),
            new Region("Mid-Atlantic", -2),
            new Region("Cape Verdes", -1),
            new Region("Greenwich Mean Time, Dublin", 0),
            new Region("Berlin, Rome", 1),
            new Region("Israel, Cairo", 2),
            new Region("Moscow, Kuwait", 3),
            new Region("Abu Dhabi, Muscat", 4),
            new Region("Islamabad, Karachi", 5),
            new Region("Almaty, Dhaka", 6),
            new Region("Bangkok, Jakarta", 7),
            new Region("Hong Kong, Beijing", 8),
            new Region("Tokyo, Osaka", 9),
            new Region("Sydney, Melbourne, Guam", 10),
            new Region("Magadan, Soloman Is.", 11),
            new Region("Fiji, Wellington, Auckland", 12),
			         new Region(Region.UnknownArea, Region.UnknownOffset)
        };

        public static Region ToRegion(int OffsetHoursValue)
        {
            return Regions[OffsetHoursValue + 12];
        }

        public Region SystemRegion
        {
            get
            {
                DateTime dtNow = DateTime.Now;
                TimeSpan dtAdjustment = dtNow.Subtract(dtNow.ToUniversalTime());

                return ToRegion((int)dtAdjustment.TotalHours -
                    (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dtNow) ? 1 : 0));
            }
        }

        public Region(string region)
        {
            if (region.StartsWith("UTC"))
            {
                region = region.Substring("UTC".Length, region.Length - "UTC".Length);
            }
               
            Region r = ToRegion(Convert.ToInt32(region));
            Area = r.Area;
            Offset = r.Offset;
        }

        public int LocalDelta
        {
            get
            {
                return Offset - SystemRegion.Offset;
            }
        }

        public DateTime UtcToUser(DateTime value)
        {
            return value.Add(new TimeSpan(0, Offset, 0, 0, 0));
        }

        public DateTime UserToUtc(DateTime value)
        {
            return value.Subtract(new TimeSpan(0, Offset, 0, 0, 0));
        }
    }
}
