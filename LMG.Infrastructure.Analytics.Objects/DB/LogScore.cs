using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogScore : object
	{
		#region Members
		private long m_LogScoreId = BaseObject.InvalidPrimaryKey;
		private double? m_Scaled = null;
		private double? m_Raw = null;
		private double? m_Min = null;
		private double? m_Max = null;
		#endregion
		#region Properties
		public long LogScoreId
		{
			get
			{
				return m_LogScoreId;
			}
			set
			{
				//this is the PRIMARY key
				m_LogScoreId = value;
			}
		}
		public double? Scaled
		{
			get
			{
				return m_Scaled;
			}
			set
			{
				m_Scaled = value;
			}
		}
		public double? Raw
		{
			get
			{
				return m_Raw;
			}
			set
			{
				m_Raw = value;
			}
		}
		public double? Min
		{
			get
			{
				return m_Min;
			}
			set
			{
				m_Min = value;
			}
		}
		public double? Max
		{
			get
			{
				return m_Max;
			}
			set
			{
				m_Max = value;
			}
		}
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("LogScore");
				if(LogScoreId != null && BaseObject.IsPrimaryKeyValid(LogScoreId))
					ins.Value("LogScoreId", SqlValue.Convert(this.m_LogScoreId));
				ins.Value("Scaled", SqlValue.Convert(this.m_Scaled));
				ins.Value("Raw", SqlValue.Convert(this.m_Raw));
				ins.Value("Min", SqlValue.Convert(this.m_Min));
				ins.Value("Max", SqlValue.Convert(this.m_Max));
				SqlResult res = Kernel.Connection.ExecuteQuery(ins);
				if(res.UpdatedRows != 1)
					return new ErrorLog("Error in insert");
				
				if(LogScoreId == BaseObject.InvalidPrimaryKey)
				{
					LogScoreId = Kernel.Connection.directOdbcConnection.LastInsertedId();
				}
			}
			catch(Exception ex)
			{
				return new ErrorLog("OnInsert error", ex);
			}
			return null;
		}
	}
	
	public class LogScoreCollection : List<LogScore>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogScore";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogScore i_LogScore = new LogScore();
				
				i_LogScore.LogScoreId = res.GetInt64(i, "LogScoreId", -1);
				i_LogScore.Scaled = res.GetNullableFloat64(i, "Scaled");
				i_LogScore.Raw = res.GetNullableFloat64(i, "Raw");
				i_LogScore.Min = res.GetNullableFloat64(i, "Min");
				i_LogScore.Max = res.GetNullableFloat64(i, "Max");
				
				this.Add(i_LogScore);
			}
		}
	}
	
}
