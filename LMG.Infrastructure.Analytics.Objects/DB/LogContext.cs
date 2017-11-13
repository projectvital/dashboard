using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogContext : object
	{
		#region Members
		private long m_LogContextId = BaseObject.InvalidPrimaryKey;
		private Guid? m_Registration = null;
		private long? m_InstructorLogAgentId = null;
		private long? m_TeamLogAgentId = null;
		private string m_Revision = null;
		private string m_Platform = null;
		private string m_Language = null;
		private Guid? m_RefLogStatementId = null;
		#endregion
		#region Properties
		public long LogContextId
		{
			get
			{
				return m_LogContextId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogContextId = value;
			}
		}
		public Guid? Registration
		{
			get
			{
				return m_Registration;
			}
			set
			{
				m_Registration = value;
			}
		}
		public long? InstructorLogAgentId
		{
			get
			{
				return m_InstructorLogAgentId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgent'
				m_InstructorLogAgentId = value;
			}
		}
		public LogAgent InstructorLogAgentIdResolved
		{
			get;
			set;
		}
		public long? TeamLogAgentId
		{
			get
			{
				return m_TeamLogAgentId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgent'
				m_TeamLogAgentId = value;
			}
		}
		public LogAgent TeamLogAgentIdResolved
		{
			get;
			set;
		}
		public string Revision
		{
			get
			{
				return m_Revision;
			}
			set
			{
				m_Revision = value;
			}
		}
		public string Platform
		{
			get
			{
				return m_Platform;
			}
			set
			{
				m_Platform = value;
			}
		}
		public string Language
		{
			get
			{
				return m_Language;
			}
			set
			{
				m_Language = value;
			}
		}
		public Guid? RefLogStatementId
		{
			get
			{
				return m_RefLogStatementId;
			}
			set
			{
				//this is a foreign key field referencing 'LogStatement'
				m_RefLogStatementId = value;
			}
		}
		public LogStatement RefLogStatementIdResolved
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
				ins.Into("LogContext");
				if(LogContextId != null && BaseObject.IsPrimaryKeyValid(LogContextId))
					ins.Value("LogContextId", SqlValue.Convert(this.m_LogContextId));
				ins.Value("Registration", SqlValue.Convert(this.m_Registration));
				ins.Value("InstructorLogAgentId", SqlValue.Convert(this.m_InstructorLogAgentId));
				ins.Value("TeamLogAgentId", SqlValue.Convert(this.m_TeamLogAgentId));
				ins.Value("Revision", SqlValue.Convert(this.m_Revision));
				ins.Value("Platform", SqlValue.Convert(this.m_Platform));
				ins.Value("Language", SqlValue.Convert(this.m_Language));
				ins.Value("RefLogStatementId", SqlValue.Convert(this.m_RefLogStatementId));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogContextId == BaseObject.InvalidPrimaryKey)
				{
					LogContextId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogContextCollection : List<LogContext>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogContext";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogContext i_LogContext = new LogContext();
				
				i_LogContext.LogContextId = res.GetInt64(i, "LogContextId", -1);
				i_LogContext.Registration = res.GetNullableGuid(i, "Registration");
				i_LogContext.InstructorLogAgentId = res.GetNullableInt64(i, "InstructorLogAgentId");
				i_LogContext.TeamLogAgentId = res.GetNullableInt64(i, "TeamLogAgentId");
				i_LogContext.Revision = res.GetString(i, "Revision", "");
				i_LogContext.Platform = res.GetString(i, "Platform", "");
				i_LogContext.Language = res.GetString(i, "Language", "");
				i_LogContext.RefLogStatementId = res.GetNullableGuid(i, "RefLogStatementId");
				
				this.Add(i_LogContext);
			}
		}
	}
	
}
