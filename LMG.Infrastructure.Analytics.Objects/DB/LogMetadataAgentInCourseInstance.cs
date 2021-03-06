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
	public class LogMetadataAgentInCourseInstance : object
	{
		#region Members
		private long m_LogAgentId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseInstanceId = BaseObject.InvalidPrimaryKey;
		private long m_LogMetadataCourseProgrammeId;
		private string m_Group = null;
		private decimal? m_Score = null;
        private decimal? m_ScoreMax = null;
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
				//this is a foreign key field referencing 'LogAgent'
				m_LogAgentId = value;
			}
		}
		public LogAgent LogAgentIdResolved
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
				m_LogMetadataCourseProgrammeId = value;
			}
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
		public decimal? Score
		{
			get
			{
				return m_Score;
			}
			set
			{
				m_Score = value;
			}
		}
        public decimal? ScoreMax
        {
            get
            {
                return m_ScoreMax;
            }
            set
            {
                m_ScoreMax = value;
            }
        }
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataAgentInCourseInstance");
				ins.Value("[LogAgentId]", SqlValue.Convert(this.m_LogAgentId));
				ins.Value("[LogMetadataCourseInstanceId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceId));
				ins.Value("[LogMetadataCourseProgrammeId]", SqlValue.Convert(this.m_LogMetadataCourseProgrammeId));
				ins.Value("[Group]", SqlValue.Convert(this.m_Group));
				ins.Value("[Score]", SqlValue.Convert(this.m_Score));
                ins.Value("[ScoreMax]", SqlValue.Convert(this.m_ScoreMax));
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
	
	public class LogMetadataAgentInCourseInstanceCollection : List<LogMetadataAgentInCourseInstance>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataAgentInCourseInstance";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataAgentInCourseInstance i_LogMetadataAgentInCourseInstance = new LogMetadataAgentInCourseInstance();
				
				i_LogMetadataAgentInCourseInstance.LogAgentId = res.GetInt64(i, "LogAgentId", -1);
				i_LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = res.GetInt64(i, "LogMetadataCourseInstanceId", -1);
				i_LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = res.GetInt64(i, "LogMetadataCourseProgrammeId", -1);
				i_LogMetadataAgentInCourseInstance.Group = res.GetString(i, "Group", "");
				i_LogMetadataAgentInCourseInstance.Score = res.GetNullableDecimal(i, "Score");
                i_LogMetadataAgentInCourseInstance.ScoreMax = res.GetNullableDecimal(i, "ScoreMax");
				
				this.Add(i_LogMetadataAgentInCourseInstance);
			}
		}
	}
	
}
