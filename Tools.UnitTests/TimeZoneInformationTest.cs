using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Resources;
using System.IO;

namespace DBlog.Tools.UnitTests
{
    [TestFixture]
    public class TimezoneInformationTest
    {
        [Test]
        public void TestKnownTimeZones()
        {
            int previousHours = 14;
            TimeZoneInformation[] tzs = TimeZoneInformation.EnumZones();
            foreach (TimeZoneInformation ti in tzs)
            {
                Assert.IsTrue(ti.CurrentUtcBias.Minutes == 0 
                    || Math.Abs(ti.CurrentUtcBias.Minutes) == 30 
                    || Math.Abs(ti.CurrentUtcBias.Minutes) == 45,
                    string.Format("Minutes = {0}", ti.CurrentUtcBias.Minutes));
                Console.WriteLine("{0}: {1}", ti.DisplayName, ti.CurrentUtcBias);
                Assert.IsTrue(ti.CurrentUtcBias.Hours <= 13 && ti.CurrentUtcBias.Hours >= -12);
                Assert.IsTrue(previousHours + 1 >= ti.CurrentUtcBias.Hours); // adjust an extra hour for daylight savings
                previousHours = ti.CurrentUtcBias.Hours;
            }
        }

        [Test]
        public void TestTryParseTimezoneOffsetToTimeSpan()
        {
            TimeZoneInformation[] tzs = TimeZoneInformation.EnumZones();
            foreach (TimeZoneInformation ti in tzs)
            {
                string tz = ti.CurrentUtcBiasString;
                TimeSpan span = TimeSpan.Zero;
                Assert.IsTrue(TimeZoneInformation.TryParseTimezoneOffsetToTimeSpan(tz, out span),
                    string.Format("Error parsing {0}", tz));
                Console.WriteLine("{0}: {1} - {2}", ti.DisplayName, tz, span);
                Assert.AreEqual(span, ti.CurrentUtcBias);
            }
        }

        internal class OffsetTestData
        {
            public string Offset;
            public TimeSpan Result;

            public OffsetTestData(string offset, TimeSpan result)
            {
                Offset = offset;
                Result = result;
            }
        }

        [Test]
        public void TestTryParseTimezoneRegionToTimeSpan()
        {

            OffsetTestData[] testdata = 
            {
                new OffsetTestData("UTC0", TimeSpan.Zero),
                new OffsetTestData("UTC1", new TimeSpan(1, 0, 0)),
                new OffsetTestData("UTC+1", new TimeSpan(1, 0, 0)),
                new OffsetTestData("UTC-1", - new TimeSpan(1, 0, 0)),
                new OffsetTestData("UTC+1:30", new TimeSpan(1, 30, 0)),
                new OffsetTestData("UTC-1:30", - new TimeSpan(1, 30, 0)),
                new OffsetTestData("UTC+1:45", new TimeSpan(1, 45, 0)),
                new OffsetTestData("UTC1:45", new TimeSpan(1, 45, 0)),
                new OffsetTestData("UTC-1:45", - new TimeSpan(1, 45, 0)),
            };

            foreach (OffsetTestData td in testdata)
            {
                TimeSpan span = TimeSpan.Zero;
                Assert.IsTrue(TimeZoneInformation.TryParseTimezoneRegionToTimeSpan(td.Offset, out span),
                    string.Format("Error parsing {0}", td.Offset));
                Console.WriteLine("{0} -> {1}", td.Offset, span);
                Assert.AreEqual(span, td.Result);
            }
        }

    }
}
