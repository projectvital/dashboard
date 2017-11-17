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
	public class LogMetadataCourseInstanceTimeBlock : object
	{
		#region Members
		private long m_LogMetadataCourseInstanceId = BaseObject.InvalidPrimaryKey;
		private int m_TimeBlock;
		private DateTime m_FromDate;
		private DateTime m_UntilDate;
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
				//this is a foreign key field referencing 'LogMetadataCourseInstance'
				m_LogMetadataCourseInstanceId = value;
			}
		}
		public LogMetadataCourseInstance LogMetadataCourseInstanceIdResolved
		{
			get;
			set;
		}
		public int TimeBlock
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
				ins.Into("LogMetadataCourseInstanceTimeBlock");
				ins.Value("[LogMetadataCourseInstanceId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceId));
				ins.Value("[TimeBlock]", SqlValue.Convert(this.m_TimeBlock));
				ins.Value("[FromDate]", SqlValue.Convert(this.m_FromDate));
				ins.Value("[UntilDate]", SqlValue.Convert(this.m_UntilDate));
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
	
	public class LogMetadataCourseInstanceTimeBlockCollection : List<LogMetadataCourseInstanceTimeBlock>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataCourseInstanceTimeBlock";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataCourseInstanceTimeBlock i_LogMetadataCourseInstanceTimeBlock = new LogMetadataCourseInstanceTimeBlock();
				
				i_LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId = res.GetInt64(i, "LogMetadataCourseInstanceId", -1);
				i_LogMetadataCourseInstanceTimeBlock.TimeBlock = res.GetInt32(i, "TimeBlock", 0);
				i_LogMetadataCourseInstanceTimeBlock.FromDate = res.GetDateTime(i, "FromDate", DateTime.MinValue);
				i_LogMetadataCourseInstanceTimeBlock.UntilDate = res.GetDateTime(i, "UntilDate", DateTime.MinValue);
				
				this.Add(i_LogMetadataCourseInstanceTimeBlock);
			}
		}
	}
	
}
