using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogExtensionMetadata : object
	{
		#region Members
		private string m_LogExtensionUri;
		private string m_LogExtensionToken;
		private long m_LogMetadataId = BaseObject.InvalidPrimaryKey;
		#endregion
		#region Properties
		public string LogExtensionUri
		{
			get
			{
				return m_LogExtensionUri;
			}
			set
			{
				m_LogExtensionUri = value;
			}
		}
		public string LogExtensionToken
		{
			get
			{
				return m_LogExtensionToken;
			}
			set
			{
				m_LogExtensionToken = value;
			}
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
				ins.Into("LogExtensionMetadata");
				ins.Value("LogExtensionUri", SqlValue.Convert(this.m_LogExtensionUri));
				ins.Value("LogExtensionToken", SqlValue.Convert(this.m_LogExtensionToken));
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
	
	public class LogExtensionMetadataCollection : List<LogExtensionMetadata>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogExtensionMetadata";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogExtensionMetadata i_LogExtensionMetadata = new LogExtensionMetadata();
				
				i_LogExtensionMetadata.LogExtensionUri = res.GetString(i, "LogExtensionUri", "");
				i_LogExtensionMetadata.LogExtensionToken = res.GetString(i, "LogExtensionToken", "");
				i_LogExtensionMetadata.LogMetadataId = res.GetInt64(i, "LogMetadataId", -1);
				
				this.Add(i_LogExtensionMetadata);
			}
		}
	}
	
}
