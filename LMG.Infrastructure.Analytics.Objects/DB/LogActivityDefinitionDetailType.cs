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
	public class LogActivityDefinitionDetailType : object
	{
		#region Members
		private long m_LogActivityDefinitionDetailTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Label = null;
		#endregion
		#region Properties
		public long LogActivityDefinitionDetailTypeId
		{
			get
			{
				return m_LogActivityDefinitionDetailTypeId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogActivityDefinitionDetailTypeId = value;
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
				ins.Into("LogActivityDefinitionDetailType");
				if(LogActivityDefinitionDetailTypeId != null && BaseObject.IsPrimaryKeyValid(LogActivityDefinitionDetailTypeId))
					ins.Value("LogActivityDefinitionDetailTypeId", SqlValue.Convert(this.m_LogActivityDefinitionDetailTypeId));
				ins.Value("Label", SqlValue.Convert(this.m_Label));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogActivityDefinitionDetailTypeId == BaseObject.InvalidPrimaryKey)
				{
					LogActivityDefinitionDetailTypeId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogActivityDefinitionDetailTypeCollection : List<LogActivityDefinitionDetailType>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogActivityDefinitionDetailType";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogActivityDefinitionDetailType i_LogActivityDefinitionDetailType = new LogActivityDefinitionDetailType();
				
				i_LogActivityDefinitionDetailType.LogActivityDefinitionDetailTypeId = res.GetInt64(i, "LogActivityDefinitionDetailTypeId", -1);
				i_LogActivityDefinitionDetailType.Label = res.GetString(i, "Label", "");
				
				this.Add(i_LogActivityDefinitionDetailType);
			}
		}
	}
	
}
