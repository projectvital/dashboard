using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogAgentDashboardSession : object
	{
		#region Members
		private long m_LogAgentDashboardSessionId = BaseObject.InvalidPrimaryKey;
		private string m_Token;
		private long? m_LogAgentId = null;
		private DateTime? m_Timestamp = null;
		private DateTime? m_ExpirationTimestamp = null;
		#endregion
		#region Properties
		public long LogAgentDashboardSessionId
		{
			get
			{
				return m_LogAgentDashboardSessionId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogAgentDashboardSessionId = value;
			}
		}
		public string Token
		{
			get
			{
				return m_Token;
			}
			set
			{
				m_Token = value;
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
				m_LogAgentId = value;
			}
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
		public DateTime? ExpirationTimestamp
		{
			get
			{
				return m_ExpirationTimestamp;
			}
			set
			{
				if (!value.HasValue)
					m_ExpirationTimestamp = value;
				else
					m_ExpirationTimestamp = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogAgentDashboardSession");
				if(LogAgentDashboardSessionId != null && BaseObject.IsPrimaryKeyValid(LogAgentDashboardSessionId))
					ins.Value("[LogAgentDashboardSessionId]", SqlValue.Convert(this.m_LogAgentDashboardSessionId));
				ins.Value("[Token]", SqlValue.Convert(this.m_Token));
				ins.Value("[LogAgentId]", SqlValue.Convert(this.m_LogAgentId));
				ins.Value("[Timestamp]", SqlValue.Convert(this.m_Timestamp));
				ins.Value("[ExpirationTimestamp]", SqlValue.Convert(this.m_ExpirationTimestamp));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogAgentDashboardSessionId == BaseObject.InvalidPrimaryKey)
				{
					LogAgentDashboardSessionId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogAgentDashboardSessionCollection : List<LogAgentDashboardSession>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogAgentDashboardSession";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogAgentDashboardSession i_LogAgentDashboardSession = new LogAgentDashboardSession();
				
				i_LogAgentDashboardSession.LogAgentDashboardSessionId = res.GetInt64(i, "LogAgentDashboardSessionId", -1);
				i_LogAgentDashboardSession.Token = res.GetString(i, "Token", "");
				i_LogAgentDashboardSession.LogAgentId = res.GetNullableInt64(i, "LogAgentId");
				i_LogAgentDashboardSession.Timestamp = res.GetNullableDateTime(i, "Timestamp");
				i_LogAgentDashboardSession.ExpirationTimestamp = res.GetNullableDateTime(i, "ExpirationTimestamp");
				
				this.Add(i_LogAgentDashboardSession);
			}
		}
	}
	
}
