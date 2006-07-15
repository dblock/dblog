using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Messaging;
using Microsoft.Web.Services3.Design;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Web.Hosting;

namespace DBlog.Data.Hibernate
{
    public class WebService : SoapService
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
             Session.Configuration.GetProperty(
              "hibernate.connection.connection_string"));
        }
    }
}
