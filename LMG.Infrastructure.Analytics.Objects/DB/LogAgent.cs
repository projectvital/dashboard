/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogAgent : object
	{
		#region Members
		private long m_LogAgentId = BaseObject.InvalidPrimaryKey;
		private long? m_StudentId = null;
		private string m_Name = null;
		private string m_Mbox = null;
		private string m_Mbox_sha1sum = null;
		private string m_OpenId = null;
		private long? m_LogAgentAccountId = null;
		private Guid? m_AnonymousKey = null;
		private string m_Password = null;
		private string m_PasswordSalt = null;
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
				//this is the PRIMARY key
				m_LogAgentId = value;
			}
		}
		public long? StudentId
		{
			get
			{
				return m_StudentId;
			}
			set
			{
				m_StudentId = value;
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
		public string Mbox
		{
			get
			{
				return m_Mbox;
			}
			set
			{
				m_Mbox = value;
			}
		}
		public string Mbox_sha1sum
		{
			get
			{
				return m_Mbox_sha1sum;
			}
			set
			{
				m_Mbox_sha1sum = value;
			}
		}
		public string OpenId
		{
			get
			{
				return m_OpenId;
			}
			set
			{
				m_OpenId = value;
			}
		}
		public long? LogAgentAccountId
		{
			get
			{
				return m_LogAgentAccountId;
			}
			set
			{
				//this is a foreign key field referencing 'LogAgentAccount'
				m_LogAgentAccountId = value;
			}
		}
		public LogAgentAccount LogAgentAccountIdResolved
		{
			get;
			set;
		}
		public Guid? AnonymousKey
		{
			get
			{
				return m_AnonymousKey;
			}
			set
			{
				m_AnonymousKey = value;
			}
		}
		public string Password
		{
			get
			{
				return m_Password;
			}
			set
			{
				m_Password = value;
			}
		}
		public string PasswordSalt
		{
			get
			{
				return m_PasswordSalt;
			}
			set
			{
				m_PasswordSalt = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogAgent");
				if(LogAgentId != null && BaseObject.IsPrimaryKeyValid(LogAgentId))
					ins.Value("[LogAgentId]", SqlValue.Convert(this.m_LogAgentId));
				ins.Value("[StudentId]", SqlValue.Convert(this.m_StudentId));
				ins.Value("[Name]", SqlValue.Convert(this.m_Name));
				ins.Value("[Mbox]", SqlValue.Convert(this.m_Mbox));
				ins.Value("[Mbox_sha1sum]", SqlValue.Convert(this.m_Mbox_sha1sum));
				ins.Value("[OpenId]", SqlValue.Convert(this.m_OpenId));
				ins.Value("[LogAgentAccountId]", SqlValue.Convert(this.m_LogAgentAccountId));
				ins.Value("[AnonymousKey]", SqlValue.Convert(this.m_AnonymousKey));
				ins.Value("[Password]", SqlValue.Convert(this.m_Password));
				ins.Value("[PasswordSalt]", SqlValue.Convert(this.m_PasswordSalt));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogAgentId == BaseObject.InvalidPrimaryKey)
				{
					LogAgentId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogAgentCollection : List<LogAgent>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogAgent";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogAgent i_LogAgent = new LogAgent();
				
				i_LogAgent.LogAgentId = res.GetInt64(i, "LogAgentId", -1);
				i_LogAgent.StudentId = res.GetNullableInt64(i, "StudentId");
				i_LogAgent.Name = res.GetString(i, "Name", "");
				i_LogAgent.Mbox = res.GetString(i, "Mbox", "");
				i_LogAgent.Mbox_sha1sum = res.GetString(i, "Mbox_sha1sum", "");
				i_LogAgent.OpenId = res.GetString(i, "OpenId", "");
				i_LogAgent.LogAgentAccountId = res.GetNullableInt64(i, "LogAgentAccountId");
				i_LogAgent.AnonymousKey = res.GetNullableGuid(i, "AnonymousKey");
				i_LogAgent.Password = res.GetString(i, "Password", "");
				i_LogAgent.PasswordSalt = res.GetString(i, "PasswordSalt", "");
				
				this.Add(i_LogAgent);
			}
		}
	}
	
}
