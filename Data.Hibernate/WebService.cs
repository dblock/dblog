using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Web.Hosting;
using DBlog.Tools.Web;

namespace DBlog.Data.Hibernate
{
    public class WebService : System.Web.Services.WebService
    {
        private EventLog mEventLog = null;

        public WebService()
        {

        }

        public void EventLogWriteEntry(string message, EventLogEntryType type)
        {
            if (HostedApplication.EventLogEnabled)
            {
                EventLog.WriteEntry(message, type);
            }
        }        

        private EventLog EventLog
        {
            get
            {
                if (mEventLog == null && HostedApplication.EventLogEnabled)
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
