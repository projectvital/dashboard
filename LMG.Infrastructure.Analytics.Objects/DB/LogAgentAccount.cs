using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogAgentAccount : object
	{
		#region Members
		private long m_LogAgentAccountId = BaseObject.InvalidPrimaryKey;
		private string m_Homepage = null;
		private string m_Name = null;
		#endregion
		#region Properties
		public long LogAgentAccountId
		{
			get
			{
				return m_LogAgentAccountId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogAgentAccountId = value;
			}
		}
		public string Homepage
		{
			get
			{
				return m_Homepage;
			}
			set
			{
				m_Homepage = value;
			}
		}
		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogAgentAccount");
				if(LogAgentAccountId != null && BaseObject.IsPrimaryKeyValid(LogAgentAccountId))
					ins.Value("LogAgentAccountId", SqlValue.Convert(this.m_LogAgentAccountId));
				ins.Value("Homepage", SqlValue.Convert(this.m_Homepage));
				ins.Value("Name", SqlValue.Convert(this.m_Name));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogAgentAccountId == BaseObject.InvalidPrimaryKey)
				{
					LogAgentAccountId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogAgentAccountCollection : List<LogAgentAccount>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogAgentAccount";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogAgentAccount i_LogAgentAccount = new LogAgentAccount();
				
				i_LogAgentAccount.LogAgentAccountId = res.GetInt64(i, "LogAgentAccountId", -1);
				i_LogAgentAccount.Homepage = res.GetString(i, "Homepage", "");
				i_LogAgentAccount.Name = res.GetString(i, "Name", "");
				
				this.Add(i_LogAgentAccount);
			}
		}
	}
	
}
