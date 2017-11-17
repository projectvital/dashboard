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
	public class LogMetadataCourseInstance : object
	{
		#region Members
		private long m_LogMetadataCourseInstanceId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseId = BaseObject.InvalidPrimaryKey;
		private string m_AcademicYear;
		private DateTime m_FromDate;
		private DateTime m_UntilDate;
		private string m_ImportId;
		#endregion
		#region Properties
		public long LogMetadataCourseInstanceId
		{
			get
			{
				return m_LogMetadataCourseInstanceId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataCourseInstanceId = value;
			}
		}
		public long LogMetadataCourseId
		{
			get
			{
				return m_LogMetadataCourseId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataCourse'
				m_LogMetadataCourseId = value;
			}
		}
		public LogMetadataCourse LogMetadataCourseIdResolved
		{
			get;
			set;
		}
		public string AcademicYear
		{
			get
			{
				return m_AcademicYear;
			}
			set
			{
				m_AcademicYear = value;
			}
		}
		public DateTime FromDate
		{
			get
			{
				return m_FromDate;
			}
			set
			{
				m_FromDate = value;
			}
		}
		public DateTime UntilDate
		{
			get
			{
				return m_UntilDate;
			}
			set
			{
				m_UntilDate = value;
			}
		}
		public string ImportId
		{
			get
			{
				return m_ImportId;
			}
			set
			{
				m_ImportId = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataCourseInstance");
				if(LogMetadataCourseInstanceId != null && BaseObject.IsPrimaryKeyValid(LogMetadataCourseInstanceId))
					ins.Value("[LogMetadataCourseInstanceId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceId));
				ins.Value("[LogMetadataCourseId]", SqlValue.Convert(this.m_LogMetadataCourseId));
				ins.Value("[AcademicYear]", SqlValue.Convert(this.m_AcademicYear));
				ins.Value("[FromDate]", SqlValue.Convert(this.m_FromDate));
				ins.Value("[UntilDate]", SqlValue.Convert(this.m_UntilDate));
				ins.Value("[ImportId]", SqlValue.Convert(this.m_ImportId));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataCourseInstanceId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataCourseInstanceId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataCourseInstanceCollection : List<LogMetadataCourseInstance>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataCourseInstance";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataCourseInstance i_LogMetadataCourseInstance = new LogMetadataCourseInstance();
				
				i_LogMetadataCourseInstance.LogMetadataCourseInstanceId = res.GetInt64(i, "LogMetadataCourseInstanceId", -1);
				i_LogMetadataCourseInstance.LogMetadataCourseId = res.GetInt64(i, "LogMetadataCourseId", -1);
				i_LogMetadataCourseInstance.AcademicYear = res.GetString(i, "AcademicYear", "");
				i_LogMetadataCourseInstance.FromDate = res.GetDateTime(i, "FromDate", DateTime.MinValue);
				i_LogMetadataCourseInstance.UntilDate = res.GetDateTime(i, "UntilDate", DateTime.MinValue);
				i_LogMetadataCourseInstance.ImportId = res.GetString(i, "ImportId", "");
				
				this.Add(i_LogMetadataCourseInstance);
			}
		}
	}
	
}
