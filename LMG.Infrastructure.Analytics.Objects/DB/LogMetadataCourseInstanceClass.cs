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
	public class LogMetadataCourseInstanceClass : object
	{
		#region Members
		private long m_LogMetadataCourseInstanceClassId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseInstanceId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseProgrammeId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseInstanceClassTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Group = null;
		private long? m_LogMetadataTeacherId = null;
		private DateTime m_FromDate;
		private DateTime m_UntilDate;
		#endregion
		#region Properties
		public long LogMetadataCourseInstanceClassId
		{
			get
			{
				return m_LogMetadataCourseInstanceClassId;
			}
			set
			{
				//this is the PRIMARY key
				//this is a foreign key field referencing 'LogMetadataCourseInstanceClass'
				m_LogMetadataCourseInstanceClassId = value;
			}
		}
		public LogMetadataCourseInstanceClass LogMetadataCourseInstanceClassIdResolved
		{
			get;
			set;
		}
		public long LogMetadataCourseInstanceId
		{
			get
			{
				return m_LogMetadataCourseInstanceId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataCourseInstance'
				m_LogMetadataCourseInstanceId = value;
			}
		}
		public LogMetadataCourseInstance LogMetadataCourseInstanceIdResolved
		{
			get;
			set;
		}
		public long LogMetadataCourseProgrammeId
		{
			get
			{
				return m_LogMetadataCourseProgrammeId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataCourseProgramme'
				m_LogMetadataCourseProgrammeId = value;
			}
		}
		public LogMetadataCourseProgramme LogMetadataCourseProgrammeIdResolved
		{
			get;
			set;
		}
		public long LogMetadataCourseInstanceClassTypeId
		{
			get
			{
				return m_LogMetadataCourseInstanceClassTypeId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataCourseInstanceClassType'
				m_LogMetadataCourseInstanceClassTypeId = value;
			}
		}
		public LogMetadataCourseInstanceClassType LogMetadataCourseInstanceClassTypeIdResolved
		{
			get;
			set;
		}
		public string Group
		{
			get
			{
				return m_Group;
			}
			set
			{
				m_Group = value;
			}
		}
		public long? LogMetadataTeacherId
		{
			get
			{
				return m_LogMetadataTeacherId;
			}
			set
			{
				//this is a foreign key field referencing 'LogMetadataTeacher'
				m_LogMetadataTeacherId = value;
			}
		}
		public LogMetadataTeacher LogMetadataTeacherIdResolved
		{
			get;
			set;
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataCourseInstanceClass");
				if(LogMetadataCourseInstanceClassId != null && BaseObject.IsPrimaryKeyValid(LogMetadataCourseInstanceClassId))
					ins.Value("[LogMetadataCourseInstanceClassId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceClassId));
				ins.Value("[LogMetadataCourseInstanceId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceId));
				ins.Value("[LogMetadataCourseProgrammeId]", SqlValue.Convert(this.m_LogMetadataCourseProgrammeId));
				ins.Value("[LogMetadataCourseInstanceClassTypeId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceClassTypeId));
				ins.Value("[Group]", SqlValue.Convert(this.m_Group));
				ins.Value("[LogMetadataTeacherId]", SqlValue.Convert(this.m_LogMetadataTeacherId));
				ins.Value("[FromDate]", SqlValue.Convert(this.m_FromDate));
				ins.Value("[UntilDate]", SqlValue.Convert(this.m_UntilDate));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataCourseInstanceClassId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataCourseInstanceClassId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataCourseInstanceClassCollection : List<LogMetadataCourseInstanceClass>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataCourseInstanceClass";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataCourseInstanceClass i_LogMetadataCourseInstanceClass = new LogMetadataCourseInstanceClass();
				
				i_LogMetadataCourseInstanceClass.LogMetadataCourseInstanceClassId = res.GetInt64(i, "LogMetadataCourseInstanceClassId", -1);
				i_LogMetadataCourseInstanceClass.LogMetadataCourseInstanceId = res.GetInt64(i, "LogMetadataCourseInstanceId", -1);
				i_LogMetadataCourseInstanceClass.LogMetadataCourseProgrammeId = res.GetInt64(i, "LogMetadataCourseProgrammeId", -1);
				i_LogMetadataCourseInstanceClass.LogMetadataCourseInstanceClassTypeId = res.GetInt64(i, "LogMetadataCourseInstanceClassTypeId", -1);
				i_LogMetadataCourseInstanceClass.Group = res.GetString(i, "Group", "");
				i_LogMetadataCourseInstanceClass.LogMetadataTeacherId = res.GetNullableInt64(i, "LogMetadataTeacherId");
				i_LogMetadataCourseInstanceClass.FromDate = res.GetDateTime(i, "FromDate", DateTime.MinValue);
				i_LogMetadataCourseInstanceClass.UntilDate = res.GetDateTime(i, "UntilDate", DateTime.MinValue);
				
				this.Add(i_LogMetadataCourseInstanceClass);
			}
		}
	}
	
}
