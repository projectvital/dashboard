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
using System.Text;
using System.Data;
using OriginDatabaseLib;
using System.IO;
using System.Configuration;


namespace LMG.Infrastructure.Analytics.Objects
{
    public class DatabaseAccess
    {
        private DateTime? m_LastAccessed = null;
        public DateTime? LastAccessed
        {
            get
            {
                return m_LastAccessed;
            }
            set { m_LastAccessed = value; }
        }

        //use id to reserve a DatabaseAccess to a specific threadID
        private int? m_ID = null;
        public int? ID
        {
            get
            {
                return m_ID;
            }
            set { m_ID = value; }
        }

        private SqlConnection m_connection = new SqlConnection();
        public SqlConnection Connection
        {
            get
            {
                EnsureConnection();
                return m_connection;
            }
            set { m_connection = value; }
        }

        public void EnsureConnection()
        {
            if (m_connection == null || !m_connection.ConnectionOpen)
            {

                string progress = "";
                string connectionString = null;
                try
                {
                    ConnectionStringSettings _studentConnection = ConfigurationManager.ConnectionStrings["LMGAnalyticsContext"];
                    if (_studentConnection == null)
                    {
                        //important: Provide connection string
                        connectionString = @"driver={SQL Server};server={IP}\\{DATABASE},{PORT};database={DATABASE INSTANCE};uid={USER};pwd={PASSWORD};";
                    }
                    else
                    {
                        connectionString = _studentConnection.ConnectionString;
                    }

                    progress += "connectionString = " + connectionString + System.Environment.NewLine;

                    m_connection = new SqlConnection();
                    m_connection.Timeout = 12000;
                    for (int i = 0; !m_connection.OpenConnection(connectionString, 10) && i < 3; i++)//Try to connect 3 times, before giving up
                        ;
                    if (!m_connection.ConnectionOpen)
                        throw new Exception("Error connecting to database.");

                    progress += "connection tried = " + m_connection.ConnectionOpen + System.Environment.NewLine;

                    SqlValue.setCurrentDriver(SqlODBCDriver.Driver.SQLServer);

                    progress += "driver set = " + SqlValue.CurrentDriver + System.Environment.NewLine;
                }
                catch (Exception ex)
                {
                    //SaveToEventLog(ex.Message, 0);
                    throw new Exception("Error connecting to database. " + progress + " // " + ex.Message /*+ " (" + connectionString + ")"*/, ex);
                }
            }
        }
    }
}
