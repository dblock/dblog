using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Web.Hosting;

namespace DBlog.Data.Hibernate
{
    public class WebService : System.Web.Services.WebService
    {
        private EventLog mEventLog = null;

        public WebService()
        {

        }

        public EventLog EventLog
        {
            get
            {
                if (mEventLog == null)
                {
                    string eventLogName = HostingEnvironment.ApplicationVirtualPath.Trim("/".ToCharArray());
                    if (eventLogName.Length == 0) eventLogName = HostingEnvironment.SiteName;
                    if (eventLogName.Length == 0) eventLogName = "Application";

                    if (!EventLog.SourceExists(eventLogName))
                    {
                        EventLog.CreateEventSource(eventLogName, "Application");
                    }

                    mEventLog = new EventLog();
                    mEventLog.Source = eventLogName;
                }
                return mEventLog;
            }
        }

        public static IDbConnection GetNewConnection()
        {
            return new SqlConnection(
                DBlog.Data.Hibernate.Session.Configuration.GetProperty(
                    "connection.connection_string"));
        }
    }
}
