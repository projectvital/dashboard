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
	public class LogActivityDefinitionDetail : object
	{
		#region Members
		private long m_LogActivityDefinitionDetailId = BaseObject.InvalidPrimaryKey;
		private long m_LogActivityDefinitionDetailTypeId = BaseObject.InvalidPrimaryKey;
		private long? m_LogActivityId = null;
		private string m_Language = null;
		private string m_Label = null;
		#endregion
		#region Properties
		public long LogActivityDefinitionDetailId
		{
			get
			{
				return m_LogActivityDefinitionDetailId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogActivityDefinitionDetailId = value;
			}
		}
		public long LogActivityDefinitionDetailTypeId
		{
			get
			{
				return m_LogActivityDefinitionDetailTypeId;
			}
			set
			{
				//this is a foreign key field referencing 'LogActivityDefinitionDetailType'
				m_LogActivityDefinitionDetailTypeId = value;
			}
		}
		public LogActivityDefinitionDetailType LogActivityDefinitionDetailTypeIdResolved
		{
			get;
			set;
		}
		public long? LogActivityId
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
		public string Language
		{
			get
			{
				return m_Language;
			}
			set
			{
				m_Language = value;
			}
		}
		public string Label
		{
			get
			{
				return m_Label;
			}
			set
			{
				m_Label = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogActivityDefinitionDetail");
				if(LogActivityDefinitionDetailId != null && BaseObject.IsPrimaryKeyValid(LogActivityDefinitionDetailId))
					ins.Value("LogActivityDefinitionDetailId", SqlValue.Convert(this.m_LogActivityDefinitionDetailId));
				ins.Value("LogActivityDefinitionDetailTypeId", SqlValue.Convert(this.m_LogActivityDefinitionDetailTypeId));
				ins.Value("LogActivityId", SqlValue.Convert(this.m_LogActivityId));
				ins.Value("Language", SqlValue.Convert(this.m_Language));
				ins.Value("Label", SqlValue.Convert(this.m_Label));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogActivityDefinitionDetailId == BaseObject.InvalidPrimaryKey)
				{
					LogActivityDefinitionDetailId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogActivityDefinitionDetailCollection : List<LogActivityDefinitionDetail>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogActivityDefinitionDetail";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogActivityDefinitionDetail i_LogActivityDefinitionDetail = new LogActivityDefinitionDetail();
				
				i_LogActivityDefinitionDetail.LogActivityDefinitionDetailId = res.GetInt64(i, "LogActivityDefinitionDetailId", -1);
				i_LogActivityDefinitionDetail.LogActivityDefinitionDetailTypeId = res.GetInt64(i, "LogActivityDefinitionDetailTypeId", -1);
				i_LogActivityDefinitionDetail.LogActivityId = res.GetNullableInt64(i, "LogActivityId");
				i_LogActivityDefinitionDetail.Language = res.GetString(i, "Language", "");
				i_LogActivityDefinitionDetail.Label = res.GetString(i, "Label", "");
				
				this.Add(i_LogActivityDefinitionDetail);
			}
		}
	}
	
}
