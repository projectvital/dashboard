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
	public class LogResult : object
	{
		#region Members
		private long m_LogResultId = BaseObject.InvalidPrimaryKey;
		private bool? m_IsCompleted = null;
		private bool? m_IsSuccess = null;
		private string m_Response = null;
		private long? m_DurationTicks = null;
		private long? m_LogScoreId = null;
		#endregion
		#region Properties
		public long LogResultId
		{
			get
			{
				return m_LogResultId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogResultId = value;
			}
		}
		public bool? IsCompleted
		{
			get
			{
				return m_IsCompleted;
			}
			set
			{
				m_IsCompleted = value;
			}
		}
		public bool? IsSuccess
		{
			get
			{
				return m_IsSuccess;
			}
			set
			{
				m_IsSuccess = value;
			}
		}
		public string Response
		{
			get
			{
				return m_Response;
			}
			set
			{
				m_Response = value;
			}
		}
		public long? DurationTicks
		{
			get
			{
				return m_DurationTicks;
			}
			set
			{
				m_DurationTicks = value;
			}
		}
		public long? LogScoreId
		{
			get
			{
				return m_LogScoreId;
			}
			set
			{
				//this is a foreign key field referencing 'LogScore'
				m_LogScoreId = value;
			}
		}
		public LogScore LogScoreIdResolved
		{
			get;
			set;
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogResult");
				if(LogResultId != null && BaseObject.IsPrimaryKeyValid(LogResultId))
					ins.Value("LogResultId", SqlValue.Convert(this.m_LogResultId));
				ins.Value("IsCompleted", SqlValue.Convert(this.m_IsCompleted));
				ins.Value("IsSuccess", SqlValue.Convert(this.m_IsSuccess));
				ins.Value("Response", SqlValue.Convert(this.m_Response));
				ins.Value("DurationTicks", SqlValue.Convert(this.m_DurationTicks));
				ins.Value("LogScoreId", SqlValue.Convert(this.m_LogScoreId));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogResultId == BaseObject.InvalidPrimaryKey)
				{
					LogResultId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogResultCollection : List<LogResult>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogResult";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogResult i_LogResult = new LogResult();
				
				i_LogResult.LogResultId = res.GetInt64(i, "LogResultId", -1);
				i_LogResult.IsCompleted = res.GetNullableBoolean(i, "IsCompleted");
				i_LogResult.IsSuccess = res.GetNullableBoolean(i, "IsSuccess");
				i_LogResult.Response = res.GetString(i, "Response", "");
				i_LogResult.DurationTicks = res.GetNullableInt64(i, "DurationTicks");
				i_LogResult.LogScoreId = res.GetNullableInt64(i, "LogScoreId");
				
				this.Add(i_LogResult);
			}
		}
	}
	
}
