using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogMetadataCourseProgramme : object
	{
		#region Members
		private long m_LogMetadataCourseProgrammeId = BaseObject.InvalidPrimaryKey;
		private string m_Name = null;
		#endregion
		#region Properties
		public long LogMetadataCourseProgrammeId
		{
			get
			{
				return m_LogMetadataCourseProgrammeId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataCourseProgrammeId = value;
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
				ins.Into("LogMetadataCourseProgramme");
				if(LogMetadataCourseProgrammeId != null && BaseObject.IsPrimaryKeyValid(LogMetadataCourseProgrammeId))
					ins.Value("[LogMetadataCourseProgrammeId]", SqlValue.Convert(this.m_LogMetadataCourseProgrammeId));
				ins.Value("[Name]", SqlValue.Convert(this.m_Name));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataCourseProgrammeId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataCourseProgrammeId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataCourseProgrammeCollection : List<LogMetadataCourseProgramme>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataCourseProgramme";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataCourseProgramme i_LogMetadataCourseProgramme = new LogMetadataCourseProgramme();
				
				i_LogMetadataCourseProgramme.LogMetadataCourseProgrammeId = res.GetInt64(i, "LogMetadataCourseProgrammeId", -1);
				i_LogMetadataCourseProgramme.Name = res.GetString(i, "Name", "");
				
				this.Add(i_LogMetadataCourseProgramme);
			}
		}
	}
	
}
