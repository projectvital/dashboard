using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogMetadataTeacher : object
	{
		#region Members
		private long m_LogMetadataTeacherId = BaseObject.InvalidPrimaryKey;
		private string m_Name;
		#endregion
		#region Properties
		public long LogMetadataTeacherId
		{
			get
			{
				return m_LogMetadataTeacherId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataTeacherId = value;
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogMetadataTeacher");
				if(LogMetadataTeacherId != null && BaseObject.IsPrimaryKeyValid(LogMetadataTeacherId))
					ins.Value("[LogMetadataTeacherId]", SqlValue.Convert(this.m_LogMetadataTeacherId));
				ins.Value("[Name]", SqlValue.Convert(this.m_Name));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataTeacherId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataTeacherId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataTeacherCollection : List<LogMetadataTeacher>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataTeacher";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataTeacher i_LogMetadataTeacher = new LogMetadataTeacher();
				
				i_LogMetadataTeacher.LogMetadataTeacherId = res.GetInt64(i, "LogMetadataTeacherId", -1);
				i_LogMetadataTeacher.Name = res.GetString(i, "Name", "");
				
				this.Add(i_LogMetadataTeacher);
			}
		}
	}
	
}
