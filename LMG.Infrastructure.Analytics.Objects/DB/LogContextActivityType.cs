using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogContextActivityType : object
	{
		#region Members
		private long m_LogContextActivityTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Type = null;
		#endregion
		#region Properties
		public long LogContextActivityTypeId
		{
			get
			{
				return m_LogContextActivityTypeId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogContextActivityTypeId = value;
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogContextActivityType");
				if(LogContextActivityTypeId != null && BaseObject.IsPrimaryKeyValid(LogContextActivityTypeId))
					ins.Value("LogContextActivityTypeId", SqlValue.Convert(this.m_LogContextActivityTypeId));
				ins.Value("Type", SqlValue.Convert(this.m_Type));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogContextActivityTypeId == BaseObject.InvalidPrimaryKey)
				{
					LogContextActivityTypeId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogContextActivityTypeCollection : List<LogContextActivityType>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogContextActivityType";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogContextActivityType i_LogContextActivityType = new LogContextActivityType();
				
				i_LogContextActivityType.LogContextActivityTypeId = res.GetInt64(i, "LogContextActivityTypeId", -1);
				i_LogContextActivityType.Type = res.GetString(i, "Type", "");
				
				this.Add(i_LogContextActivityType);
			}
		}
	}
	
}
