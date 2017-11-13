using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogStatementLink : object
	{
		#region Members
		private Guid? m_LogStatementId = null;
		private string m_TableName = null;
		private string m_TableId = null;
		#endregion
		#region Properties
		public Guid? LogStatementId
		{
			get
			{
				return m_LogStatementId;
			}
			set
			{
				m_LogStatementId = value;
			}
		}
		public string TableName
		{
			get
			{
				return m_TableName;
			}
			set
			{
				m_TableName = value;
			}
		}
		public string TableId
		{
			get
			{
				return m_TableId;
			}
			set
			{
				m_TableId = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogStatementLink");
				ins.Value("LogStatementId", SqlValue.Convert(this.m_LogStatementId));
				ins.Value("TableName", SqlValue.Convert(this.m_TableName));
				ins.Value("TableId", SqlValue.Convert(this.m_TableId));
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
	
	public class LogStatementLinkCollection : List<LogStatementLink>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogStatementLink";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogStatementLink i_LogStatementLink = new LogStatementLink();
				
				i_LogStatementLink.LogStatementId = res.GetNullableGuid(i, "LogStatementId");
				i_LogStatementLink.TableName = res.GetString(i, "TableName", "");
				i_LogStatementLink.TableId = res.GetString(i, "TableId", "");
				
				this.Add(i_LogStatementLink);
			}
		}
	}
	
}
