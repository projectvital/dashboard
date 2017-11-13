using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogActivity : object
	{
		#region Members
		private long m_LogActivityId = BaseObject.InvalidPrimaryKey;
		private string m_Id = null;
		private long? m_LogActivityDefinitionId = null;
		#endregion
		#region Properties
		public long LogActivityId
		{
			get
			{
				return m_LogActivityId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogActivityId = value;
			}
		}
		public string Id
		{
			get
			{
				return m_Id;
			}
			set
			{
				m_Id = value;
			}
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogActivity");
				if(LogActivityId != null && BaseObject.IsPrimaryKeyValid(LogActivityId))
					ins.Value("LogActivityId", SqlValue.Convert(this.m_LogActivityId));
				ins.Value("Id", SqlValue.Convert(this.m_Id));
				ins.Value("LogActivityDefinitionId", SqlValue.Convert(this.m_LogActivityDefinitionId));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogActivityId == BaseObject.InvalidPrimaryKey)
				{
					LogActivityId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogActivityCollection : List<LogActivity>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogActivity";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogActivity i_LogActivity = new LogActivity();
				
				i_LogActivity.LogActivityId = res.GetInt64(i, "LogActivityId", -1);
				i_LogActivity.Id = res.GetString(i, "Id", "");
				i_LogActivity.LogActivityDefinitionId = res.GetNullableInt64(i, "LogActivityDefinitionId");
				
				this.Add(i_LogActivity);
			}
		}
	}
	
}
