using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogAgentMetadata : object
	{
		#region Members
		private long m_LogAgentId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataId = BaseObject.InvalidPrimaryKey;
		#endregion
		#region Properties
		public long LogAgentId
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
		public long LogMetadataId
		{
			get
			{
				return m_LogMetadataId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadata'
				m_LogMetadataId = value;
			}
		}
		public LogMetadata LogMetadataIdResolved
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
				ins.Into("LogAgentMetadata");
				ins.Value("LogAgentId", SqlValue.Convert(this.m_LogAgentId));
				ins.Value("LogMetadataId", SqlValue.Convert(this.m_LogMetadataId));
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
	
	public class LogAgentMetadataCollection : List<LogAgentMetadata>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogAgentMetadata";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogAgentMetadata i_LogAgentMetadata = new LogAgentMetadata();
				
				i_LogAgentMetadata.LogAgentId = res.GetInt64(i, "LogAgentId", -1);
				i_LogAgentMetadata.LogMetadataId = res.GetInt64(i, "LogMetadataId", -1);
				
				this.Add(i_LogAgentMetadata);
			}
		}
	}
	
}
