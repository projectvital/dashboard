using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogExtension : object
	{
		#region Members
		private long? m_LogContextId = null;
		private long? m_LogResultId = null;
		private long? m_LogActivityDefinitionId = null;
		private long? m_LogActivityId = null;
		private string m_Uri = null;
		private string m_Token = null;
		#endregion
		#region Properties
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
		public long? LogActivityDefinitionId
		{
			get
			{
				return m_LogActivityDefinitionId;
			}
			set
			{
				//this is a foreign key field referencing 'LogActivityDefinition'
				m_LogActivityDefinitionId = value;
			}
		}
		public LogActivityDefinition LogActivityDefinitionIdResolved
		{
			get;
			set;
		}
		public long? LogActivityId
		{
			get
			{
				return m_LogActivityId;
			}
			set
			{
				//this is a foreign key field referencing 'LogActivity'
				m_LogActivityId = value;
			}
		}
		public LogActivity LogActivityIdResolved
		{
			get;
			set;
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogExtension");
				ins.Value("LogContextId", SqlValue.Convert(this.m_LogContextId));
				ins.Value("LogResultId", SqlValue.Convert(this.m_LogResultId));
				ins.Value("LogActivityDefinitionId", SqlValue.Convert(this.m_LogActivityDefinitionId));
				ins.Value("LogActivityId", SqlValue.Convert(this.m_LogActivityId));
				ins.Value("Uri", SqlValue.Convert(this.m_Uri));
				ins.Value("Token", SqlValue.Convert(this.m_Token));
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
	
	public class LogExtensionCollection : List<LogExtension>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogExtension";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogExtension i_LogExtension = new LogExtension();
				
				i_LogExtension.LogContextId = res.GetNullableInt64(i, "LogContextId");
				i_LogExtension.LogResultId = res.GetNullableInt64(i, "LogResultId");
				i_LogExtension.LogActivityDefinitionId = res.GetNullableInt64(i, "LogActivityDefinitionId");
				i_LogExtension.LogActivityId = res.GetNullableInt64(i, "LogActivityId");
				i_LogExtension.Uri = res.GetString(i, "Uri", "");
				i_LogExtension.Token = res.GetString(i, "Token", "");
				
				this.Add(i_LogExtension);
			}
		}
	}
	
}
