using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Web.Hosting;

namespace DBlog.Data.Hibernate
{
    public abstract class SystemService
    {
        private Thread mThread = null;
        private bool mIsStopping = false;
        private EventLog mEventLog = null;

        public SystemService()
        {

        }

        public bool IsStopping
        {
            get
            {
                return mIsStopping;
            }
        }

        public static IDbConnection GetNewConnection()
        {
            return new SqlConnection(
                Session.Configuration.GetProperty(
                    "hibernate.connection.connection_string"));
        }

        public void Start()
        {
            mThread = new Thread(ThreadProc);
            mThread.Start(this);
        }

        public void Stop()
        {
            mIsStopping = true;

            if (mThread != null)
            {
                if (mThread.IsAlive)
                {
                    Thread.Sleep(500);
                    mThread.Abort();
                }
                mThread.Join();
            }            
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

        public static void ThreadProc(object service)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            SystemService s = (SystemService) service;
            Thread.Sleep(1000 * 30); // let the system come up
            s.Run();
        }

        public abstract void Run();
    }
}
