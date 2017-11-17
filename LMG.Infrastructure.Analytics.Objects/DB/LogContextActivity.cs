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
	public class LogContextActivity : object
	{
		#region Members
		private long m_LogContextId = BaseObject.InvalidPrimaryKey;
		private long m_LogContextActivityTypeId = BaseObject.InvalidPrimaryKey;
		private long m_LogActivityId = BaseObject.InvalidPrimaryKey;
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
				//this is a foreign key field referencing 'LogContext'
				m_LogContextId = value;
			}
		}
		public LogContext LogContextIdResolved
		{
			get;
			set;
		}
		public long LogContextActivityTypeId
		{
			get
			{
				return m_LogContextActivityTypeId;
			}
			set
			{
				//this is a foreign key field referencing 'LogContextActivityType'
				m_LogContextActivityTypeId = value;
			}
		}
		public LogContextActivityType LogContextActivityTypeIdResolved
		{
			get;
			set;
		}
		public long LogActivityId
		{
			get
			{
				return m_LogActivityId;
			}
			set
			{
				//this is a foreign key field referencing 'LogActivity'
				m_LogActivityId = value;
			}
		}
		public LogActivity LogActivityIdResolved
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
				ins.Into("LogContextActivity");
				ins.Value("LogContextId", SqlValue.Convert(this.m_LogContextId));
				ins.Value("LogContextActivityTypeId", SqlValue.Convert(this.m_LogContextActivityTypeId));
				ins.Value("LogActivityId", SqlValue.Convert(this.m_LogActivityId));
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
	
	public class LogContextActivityCollection : List<LogContextActivity>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogContextActivity";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogContextActivity i_LogContextActivity = new LogContextActivity();
				
				i_LogContextActivity.LogContextId = res.GetInt64(i, "LogContextId", -1);
				i_LogContextActivity.LogContextActivityTypeId = res.GetInt64(i, "LogContextActivityTypeId", -1);
				i_LogContextActivity.LogActivityId = res.GetInt64(i, "LogActivityId", -1);
				
				this.Add(i_LogContextActivity);
			}
		}
	}
	
}
