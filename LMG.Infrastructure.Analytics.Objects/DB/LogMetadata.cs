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
	public class LogMetadata : object
	{
		#region Members
		private long m_LogMetadataId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Value;
		#endregion
		#region Properties
		public long LogMetadataId
		{
			get
			{
				return m_LogMetadataId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataId = value;
			}
		}
		public long LogMetadataTypeId
		{
			get
			{
				return m_LogMetadataTypeId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataType'
				m_LogMetadataTypeId = value;
			}
		}
		public LogMetadataType LogMetadataTypeIdResolved
		{
			get;
			set;
		}
		public string Value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadata");
				if(LogMetadataId != null && BaseObject.IsPrimaryKeyValid(LogMetadataId))
					ins.Value("[LogMetadataId]", SqlValue.Convert(this.m_LogMetadataId));
				ins.Value("[LogMetadataTypeId]", SqlValue.Convert(this.m_LogMetadataTypeId));
				ins.Value("[Value]", SqlValue.Convert(this.m_Value));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataCollection : List<LogMetadata>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadata";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadata i_LogMetadata = new LogMetadata();
				
				i_LogMetadata.LogMetadataId = res.GetInt64(i, "LogMetadataId", -1);
				i_LogMetadata.LogMetadataTypeId = res.GetInt64(i, "LogMetadataTypeId", -1);
				i_LogMetadata.Value = res.GetString(i, "Value", "");
				
				this.Add(i_LogMetadata);
			}
		}
	}
	
}
