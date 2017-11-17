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
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogVerb : object
	{
		#region Members
		private long m_LogVerbId = BaseObject.InvalidPrimaryKey;
		private string m_Uri = null;
		#endregion
		#region Properties
		public long LogVerbId
		{
			get
			{
				return m_LogVerbId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogVerbId = value;
			}
		}
		public string Uri
		{
			get
			{
				return m_Uri;
			}
			set
			{
				m_Uri = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogVerb");
				if(LogVerbId != null && BaseObject.IsPrimaryKeyValid(LogVerbId))
					ins.Value("LogVerbId", SqlValue.Convert(this.m_LogVerbId));
				ins.Value("Uri", SqlValue.Convert(this.m_Uri));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogVerbId == BaseObject.InvalidPrimaryKey)
				{
					LogVerbId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogVerbCollection : List<LogVerb>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogVerb";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogVerb i_LogVerb = new LogVerb();
				
				i_LogVerb.LogVerbId = res.GetInt64(i, "LogVerbId", -1);
				i_LogVerb.Uri = res.GetString(i, "Uri", "");
				
				this.Add(i_LogVerb);
			}
		}
	}
	
}
