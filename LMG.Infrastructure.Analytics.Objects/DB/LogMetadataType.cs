using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogMetadataType : object
	{
		#region Members
		private long m_LogMetadataTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Descr;
		#endregion
		#region Properties
		public long LogMetadataTypeId
		{
			get
			{
				return m_LogMetadataTypeId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataTypeId = value;
			}
		}
		public string Descr
		{
			get
			{
				return m_Descr;
			}
			set
			{
				m_Descr = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataType");
				if(LogMetadataTypeId != null && BaseObject.IsPrimaryKeyValid(LogMetadataTypeId))
					ins.Value("[LogMetadataTypeId]", SqlValue.Convert(this.m_LogMetadataTypeId));
				ins.Value("[Descr]", SqlValue.Convert(this.m_Descr));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataTypeId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataTypeId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataTypeCollection : List<LogMetadataType>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataType";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataType i_LogMetadataType = new LogMetadataType();
				
				i_LogMetadataType.LogMetadataTypeId = res.GetInt64(i, "LogMetadataTypeId", -1);
				i_LogMetadataType.Descr = res.GetString(i, "Descr", "");
				
				this.Add(i_LogMetadataType);
			}
		}
	}
	
}
