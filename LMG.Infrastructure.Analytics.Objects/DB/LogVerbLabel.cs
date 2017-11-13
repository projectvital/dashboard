using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OriginDatabaseLib;
using LMG.Infrastructure.Analytics.Objects.DB.Base;

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class LogVerbLabel : object
	{
		#region Members
		private long? m_LogVerbId = null;
		private string m_Language = null;
		private string m_Label = null;
		#endregion
		#region Properties
		public long? LogVerbId
		{
			get
			{
				return m_LogVerbId;
			}
			set
			{
				//this is a foreign key field referencing 'LogVerb'
				m_LogVerbId = value;
			}
		}
		public LogVerb LogVerbIdResolved
		{
			get;
			set;
		}
		public string Language
		{
			get
			{
				return m_Language;
			}
			set
			{
				m_Language = value;
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
				ins.Into("LogVerbLabel");
				ins.Value("LogVerbId", SqlValue.Convert(this.m_LogVerbId));
				ins.Value("Language", SqlValue.Convert(this.m_Language));
				ins.Value("Label", SqlValue.Convert(this.m_Label));
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
	
	public class LogVerbLabelCollection : List<LogVerbLabel>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM LogVerbLabel";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				LogVerbLabel i_LogVerbLabel = new LogVerbLabel();
				
				i_LogVerbLabel.LogVerbId = res.GetNullableInt64(i, "LogVerbId");
				i_LogVerbLabel.Language = res.GetString(i, "Language", "");
				i_LogVerbLabel.Label = res.GetString(i, "Label", "");
				
				this.Add(i_LogVerbLabel);
			}
		}
	}
	
}
