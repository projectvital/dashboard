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
	public class LogStatement : object
	{
		#region Members
		private Guid m_LogStatementId;
		private long? m_LogAgentId = null;
		private long? m_LogVerbId = null;
		private long? m_TargetLogAgentId = null;
		private long? m_TargetLogActivityId = null;
		private Guid? m_TargetLogStatementId = null;
		private long? m_LogResultId = null;
		private long? m_LogContextId = null;
		private DateTime? m_Timestamp = null;
		private DateTime? m_StoredTimestamp = null;
		private long? m_AuthorityLogAgentId = null;
		#endregion
		#region Properties
		public Guid LogStatementId
		{
			get
			{
				return m_LogStatementId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogStatementId = value;
			}
		}
		public long? LogAgentId
		{
			get
			{
				return m_LogAgentId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgent'
				m_LogAgentId = value;
			}
		}
		public LogAgent LogAgentIdResolved
		{
			get;
			set;
		}
		public long? LogVerbId
		{
			get
			{
				return m_LogVerbId;
			}
			set
			{
				//this is a foreign key field referencing 'LogVerb'
				m_LogVerbId = value;
			}
		}
		public LogVerb LogVerbIdResolved
		{
			get;
			set;
		}
		public long? TargetLogAgentId
		{
			get
			{
				return m_TargetLogAgentId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgent'
				m_TargetLogAgentId = value;
			}
		}
		public LogAgent TargetLogAgentIdResolved
		{
			get;
			set;
		}
		public long? TargetLogActivityId
		{
			get
			{
				return m_TargetLogActivityId;
			}
			set
			{
				//this is a foreign key field referencing 'LogActivity'
				m_TargetLogActivityId = value;
			}
		}
		public LogActivity TargetLogActivityIdResolved
		{
			get;
			set;
		}
		public Guid? TargetLogStatementId
		{
			get
			{
				return m_TargetLogStatementId;
			}
			set
			{
				//this is a foreign key field referencing 'LogStatement'
				m_TargetLogStatementId = value;
			}
		}
		public LogStatement TargetLogStatementIdResolved
		{
			get;
			set;
		}
		public long? LogResultId
		{
			get
			{
				return m_LogResultId;
			}
			set
			{
				//this is a foreign key field referencing 'LogResult'
				m_LogResultId = value;
			}
		}
		public LogResult LogResultIdResolved
		{
			get;
			set;
		}
		public long? LogContextId
		{
			get
			{
				return m_LogContextId;
			}
			set
			{
				//this is a foreign key field referencing 'LogContext'
				m_LogContextId = value;
			}
		}
		public LogContext LogContextIdResolved
		{
			get;
			set;
		}
		public DateTime? Timestamp
		{
			get
			{
				return m_Timestamp;
			}
			set
			{
				if (!value.HasValue)
					m_Timestamp = value;
				else
					m_Timestamp = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
			}
		}
		public DateTime? StoredTimestamp
		{
			get
			{
				return m_StoredTimestamp;
			}
			set
			{
				if (!value.HasValue)
					m_StoredTimestamp = value;
				else
					m_StoredTimestamp = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
			}
		}
		public long? AuthorityLogAgentId
		{
			get
			{
				return m_AuthorityLogAgentId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgent'
				m_AuthorityLogAgentId = value;
			}
		}
		public LogAgent AuthorityLogAgentIdResolved
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
				ins.Into("LogStatement");
				if(LogStatementId != null && BaseObject.IsPrimaryKeyValid(LogStatementId))
					ins.Value("LogStatementId", SqlValue.Convert(this.m_LogStatementId));
				ins.Value("LogAgentId", SqlValue.Convert(this.m_LogAgentId));
				ins.Value("LogVerbId", SqlValue.Convert(this.m_LogVerbId));
				ins.Value("TargetLogAgentId", SqlValue.Convert(this.m_TargetLogAgentId));
				ins.Value("TargetLogActivityId", SqlValue.Convert(this.m_TargetLogActivityId));
				ins.Value("TargetLogStatementId", SqlValue.Convert(this.m_TargetLogStatementId));
				ins.Value("LogResultId", SqlValue.Convert(this.m_LogResultId));
				ins.Value("LogContextId", SqlValue.Convert(this.m_LogContextId));
				ins.Value("Timestamp", SqlValue.Convert(this.m_Timestamp));
				ins.Value("StoredTimestamp", SqlValue.Convert(this.m_StoredTimestamp));
				ins.Value("AuthorityLogAgentId", SqlValue.Convert(this.m_AuthorityLogAgentId));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogStatementCollection : List<LogStatement>
	{
		public void FillCollection(string where, string orderBy, decimal? limit = null)
		{
			//string sql = "SELECT " + ((limit.HasValue)?"TOP " + limit.Value:"") + " * FROM LogStatement";
            string sql = "SELECT " + ((limit.HasValue) ? "TOP " + limit.Value : "") + " * FROM LogStatement";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogStatement i_LogStatement = new LogStatement();
				
				i_LogStatement.LogStatementId = res.GetGuid(i, "LogStatementId", Guid.Empty);
				i_LogStatement.LogAgentId = res.GetNullableInt64(i, "LogAgentId");
				i_LogStatement.LogVerbId = res.GetNullableInt64(i, "LogVerbId");
				i_LogStatement.TargetLogAgentId = res.GetNullableInt64(i, "TargetLogAgentId");
				i_LogStatement.TargetLogActivityId = res.GetNullableInt64(i, "TargetLogActivityId");
				i_LogStatement.TargetLogStatementId = res.GetNullableGuid(i, "TargetLogStatementId");
				i_LogStatement.LogResultId = res.GetNullableInt64(i, "LogResultId");
				i_LogStatement.LogContextId = res.GetNullableInt64(i, "LogContextId");
				i_LogStatement.Timestamp = res.GetNullableDateTime(i, "Timestamp");
				i_LogStatement.StoredTimestamp = res.GetNullableDateTime(i, "StoredTimestamp");
				i_LogStatement.AuthorityLogAgentId = res.GetNullableInt64(i, "AuthorityLogAgentId");
				
				this.Add(i_LogStatement);
			}
		}
	}
	
}
