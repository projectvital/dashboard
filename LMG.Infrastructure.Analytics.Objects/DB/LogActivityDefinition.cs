using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogActivityDefinition : object
	{
		#region Members
		private long m_LogActivityDefinitionId = BaseObject.InvalidPrimaryKey;
		private string m_Type = null;
		private string m_MoreInfo = null;
		#endregion
		#region Properties
		public long LogActivityDefinitionId
		{
			get
			{
				return m_LogActivityDefinitionId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogActivityDefinitionId = value;
			}
		}
		public string Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
			}
		}
		public string MoreInfo
		{
			get
			{
				return m_MoreInfo;
			}
			set
			{
				m_MoreInfo = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogActivityDefinition");
				if(LogActivityDefinitionId != null && BaseObject.IsPrimaryKeyValid(LogActivityDefinitionId))
					ins.Value("LogActivityDefinitionId", SqlValue.Convert(this.m_LogActivityDefinitionId));
				ins.Value("Type", SqlValue.Convert(this.m_Type));
				ins.Value("MoreInfo", SqlValue.Convert(this.m_MoreInfo));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogActivityDefinitionId == BaseObject.InvalidPrimaryKey)
				{
					LogActivityDefinitionId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogActivityDefinitionCollection : List<LogActivityDefinition>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogActivityDefinition";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogActivityDefinition i_LogActivityDefinition = new LogActivityDefinition();
				
				i_LogActivityDefinition.LogActivityDefinitionId = res.GetInt64(i, "LogActivityDefinitionId", -1);
				i_LogActivityDefinition.Type = res.GetString(i, "Type", "");
				i_LogActivityDefinition.MoreInfo = res.GetString(i, "MoreInfo", "");
				
				this.Add(i_LogActivityDefinition);
			}
		}
	}
	
}
