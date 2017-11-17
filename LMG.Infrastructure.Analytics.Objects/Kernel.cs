/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OriginDatabaseLib;
using System.Configuration;
using OriginDatabaseLib;
using System.Threading;

namespace LMG.Infrastructure.Analytics.Objects
{
    public static class Kernel
    {
        private static void SaveToEventLog(string message, int eventID)
        {
            string sSource;
            string sLog;
            string sEvent;

            sSource = "LMG.Infrastructure.Analytics::AnalyticsWorker";
            sLog = "Application";
            sEvent = message;

            if (!System.Diagnostics.EventLog.SourceExists(sSource))
                System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
            System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Error, eventID);
        }

        public static void EnsureConnection()
        {
            var connection = Kernel.Connection;//Initialize the connection to ensure proper driver usage.
        }
        public static SqlConnection Connection
        {
            get
            {
                return DatabaseAccess.Connection;
            }
        }

        private static string m_databaseAccessLock = "";
        private static List<DatabaseAccess> m_CachePoolList = null;

        private static string m_platformVersion;
        public static string PlatformVersion
        {
            get {
                if (string.IsNullOrWhiteSpace(m_platformVersion))
                {
                    m_platformVersion = ConfigurationManager.AppSettings["PlatformVersion"];
                }
                return m_platformVersion; 
            
            }
            set { m_platformVersion = value; }
        }

        public static DatabaseAccess DatabaseAccess
        {
            get
            {
                DatabaseAccess outDatabaseAccess = null;

                lock (m_databaseAccessLock)
                {
                    if (m_CachePoolList == null)
                        m_CachePoolList = new List<DatabaseAccess>();


                    if (m_CachePoolList != null)
                    {
                        int ID = Thread.CurrentThread.ManagedThreadId;
                        int positionInList = -1;

                        for (int i = m_CachePoolList.Count - 1; i >= 0; i--)//Start at the end: the recently used ones are there!
                        {
                            DatabaseAccess da = m_CachePoolList[i];

                            if (da.ID == ID)
                            {
                                outDatabaseAccess = da;
                                positionInList = i;
                                break;
                            }
                        }

                        if (outDatabaseAccess != null)
                        {//if we found it in cache: move it to the end of the list; by doing so the recently used ones are always at the end
                            if (positionInList < m_CachePoolList.Count - 1)//no need to move when already at then end of the list
                            {
                                m_CachePoolList.Remove(outDatabaseAccess);
                                m_CachePoolList.Add(outDatabaseAccess);
                            }
                        }
                        else
                        {//not in there? ==> create a new one and add it at the end of the list
                            outDatabaseAccess = new DatabaseAccess();
                            outDatabaseAccess.ID = ID;
                            m_CachePoolList.Add(outDatabaseAccess);
                        }

                        //Set the last accessed (helps cleaning up when needed):
                        outDatabaseAccess.LastAccessed = DateTime.Now;

                        //If the list becomes too long: start cleaning up; check the first one: clean it up when the LastAccessed time was at least 2,5 minutes ago and there is no transaction active:
                        if (m_CachePoolList.Count > 15)
                        {
                            if (m_CachePoolList[0].LastAccessed != null
                                 && DateTime.Now > m_CachePoolList[0].LastAccessed.Value.AddSeconds(150)
                                 && !m_CachePoolList[0].Connection.TransactionActive())
                            {
                                m_CachePoolList[0].Connection.CloseConnection();//the destructor doesn't seem to work
                                m_CachePoolList.RemoveAt(0);
                            }
                        }

                    }
                }

                return outDatabaseAccess;
            }
        }

        public static bool TransactionActive()
        {
            return Connection.TransactionActive();
        }
        public static void TransactionBegin()
        {
            Connection.TransactionBegin();
        }
        public static void TransactionCommit()
        {
            Connection.TransactionCommit();
        }
        public static void TransactionRollback()
        {
            Connection.TransactionRollback();
        }
        public static string MakeSqlSafe(object obj)
        {
            SqlValue.setCurrentDriver(OriginDatabaseLib.SqlODBCDriver.Driver.SQLServer);
            return SqlValue.Convert(obj);
        }


        public static string GetConnectionDictionaryInfo()
        {
            lock (m_databaseAccessLock)
            {
                /*if (m_ActivePoolDictionary == null)
                    return "m_ActivePoolDictionary == null";
                else*/
                if (m_CachePoolList == null)
                    return "m_CachePoolList == null";
                else if (m_CachePoolList.Count == 0)
                    return "; m_CachePoolList contains " + m_CachePoolList.Count + " entries.";
                else
                {
                    return "; m_CachePoolList contains " + m_CachePoolList.Count + " entries; oldest: " + m_CachePoolList[0].ID + " - " + m_CachePoolList[0].LastAccessed.Value.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }

        }
    }
}