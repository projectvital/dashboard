using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogMetadataCourseInstanceClassType : object
	{
		#region Members
		private long m_LogMetadataCourseInstanceClassTypeId = BaseObject.InvalidPrimaryKey;
		private string m_Name;
		#endregion
		#region Properties
		public long LogMetadataCourseInstanceClassTypeId
		{
			get
			{
				return m_LogMetadataCourseInstanceClassTypeId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogMetadataCourseInstanceClassTypeId = value;
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
				ins.Into("LogMetadataCourseInstanceClassType");
				if(LogMetadataCourseInstanceClassTypeId != null && BaseObject.IsPrimaryKeyValid(LogMetadataCourseInstanceClassTypeId))
					ins.Value("[LogMetadataCourseInstanceClassTypeId]", SqlValue.Convert(this.m_LogMetadataCourseInstanceClassTypeId));
				ins.Value("[Name]", SqlValue.Convert(this.m_Name));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogMetadataCourseInstanceClassTypeId == BaseObject.InvalidPrimaryKey)
				{
					LogMetadataCourseInstanceClassTypeId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogMetadataCourseInstanceClassTypeCollection : List<LogMetadataCourseInstanceClassType>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogMetadataCourseInstanceClassType";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogMetadataCourseInstanceClassType i_LogMetadataCourseInstanceClassType = new LogMetadataCourseInstanceClassType();
				
				i_LogMetadataCourseInstanceClassType.LogMetadataCourseInstanceClassTypeId = res.GetInt64(i, "LogMetadataCourseInstanceClassTypeId", -1);
				i_LogMetadataCourseInstanceClassType.Name = res.GetString(i, "Name", "");
				
				this.Add(i_LogMetadataCourseInstanceClassType);
			}
		}
	}
	
}
