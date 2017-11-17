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
	public class LogMetadataActivityInCourse : object
	{
		#region Members
		private string m_LogActivityUrl;
		private string m_Name = null;
		private int? m_TimeBlock = null;
		private long m_LogMetadataCourseId = BaseObject.InvalidPrimaryKey;
		private string m_Chapter = null;
        private string m_ActivityType = null;
        private string m_ObjectId = null;
		#endregion
		#region Properties
		public string LogActivityUrl
		{
			get
			{
				return m_LogActivityUrl;
			}
			set
			{
				m_LogActivityUrl = value;
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
		public int? TimeBlock
		{
			get
			{
				return m_TimeBlock;
			}
			set
			{
				m_TimeBlock = value;
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
		public string Chapter
		{
			get
			{
				return m_Chapter;
			}
			set
			{
				m_Chapter = value;
			}
		}
        public string ActivityType
        {
            get { return m_ActivityType; }
            set { m_ActivityType = value; }
        }
        public string ObjectId
        {
            get { return m_ObjectId; }
            set { m_ObjectId = value; }
        }
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataActivityInCourse");
				ins.Value("[LogActivityUrl]", SqlValue.Convert(this.m_LogActivityUrl));
				ins.Value("[Name]", SqlValue.Convert(this.m_Name));
				ins.Value("[TimeBlock]", SqlValue.Convert(this.m_TimeBlock));
				ins.Value("[LogMetadataCourseId]", SqlValue.Convert(this.m_LogMetadataCourseId));
				ins.Value("[Chapter]", SqlValue.Convert(this.m_Chapter));
                ins.Value("[ActivityType]", SqlValue.Convert(this.m_ActivityType));
                ins.Value("[ObjectId]", SqlValue.Convert(this.m_ObjectId));
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
	
	public class LogMetadataActivityInCourseCollection : List<LogMetadataActivityInCourse>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataActivityInCourse";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataActivityInCourse i_LogMetadataActivityInCourse = new LogMetadataActivityInCourse();
				
				i_LogMetadataActivityInCourse.LogActivityUrl = res.GetString(i, "LogActivityUrl", "");
				i_LogMetadataActivityInCourse.Name = res.GetString(i, "Name", "");
				i_LogMetadataActivityInCourse.TimeBlock = res.GetNullableInt32(i, "TimeBlock");
				i_LogMetadataActivityInCourse.LogMetadataCourseId = res.GetInt64(i, "LogMetadataCourseId", -1);
				i_LogMetadataActivityInCourse.Chapter = res.GetString(i, "Chapter", "");
                i_LogMetadataActivityInCourse.ActivityType = res.GetString(i, "ActivityType", "");
                i_LogMetadataActivityInCourse.ObjectId = res.GetString(i, "ObjectId", "");
				
				this.Add(i_LogMetadataActivityInCourse);
			}
		}
	}
	
}
