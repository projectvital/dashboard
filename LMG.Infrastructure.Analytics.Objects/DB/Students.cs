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

namespace LMG.Infrastructure.Analytics.Objects.DB
{
	public class Students : object
	{
		#region Members
		private long m_Id;
		private string m_FirstName;
		private string m_LastName;
		private string m_EmailAddress;
		private string m_Password;
		private string m_PasswordSalt;
		private bool m_IsDeleted;
		private string m_Group;
		#endregion
		#region Properties
		public long Id
		{
			get
			{
				return m_Id;
			}
			set
			{
				m_Id = value;
			}
		}
		public string FirstName
		{
			get
			{
				return m_FirstName;
			}
			set
			{
				m_FirstName = value;
			}
		}
		public string LastName
		{
			get
			{
				return m_LastName;
			}
			set
			{
				m_LastName = value;
			}
		}
		public string EmailAddress
		{
			get
			{
				return m_EmailAddress;
			}
			set
			{
				m_EmailAddress = value;
			}
		}
		public string Password
		{
			get
			{
				return m_Password;
			}
			set
			{
				m_Password = value;
			}
		}
		public string PasswordSalt
		{
			get
			{
				return m_PasswordSalt;
			}
			set
			{
				m_PasswordSalt = value;
			}
		}
		public bool IsDeleted
		{
			get
			{
				return m_IsDeleted;
			}
			set
			{
				m_IsDeleted = value;
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
		#endregion
		public ErrorLog OnInsert()
		{
			try
			{
				SqlQueryInsert ins = new SqlQueryInsert();
				ins.Into("Students");
				ins.Value("Id", SqlValue.Convert(this.m_Id));
				ins.Value("FirstName", SqlValue.Convert(this.m_FirstName));
				ins.Value("LastName", SqlValue.Convert(this.m_LastName));
				ins.Value("EmailAddress", SqlValue.Convert(this.m_EmailAddress));
				ins.Value("Password", SqlValue.Convert(this.m_Password));
				ins.Value("PasswordSalt", SqlValue.Convert(this.m_PasswordSalt));
				ins.Value("IsDeleted", SqlValue.Convert(this.m_IsDeleted));
				ins.Value("Group", SqlValue.Convert(this.m_Group));
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
	
	public class StudentsCollection : List<Students>
	{
		public void FillCollection(string where, string orderBy)
		{
			string sql = "SELECT * FROM Students";
			if(!String.IsNullOrEmpty(where)) sql += " WHERE " + where; 
			if(!String.IsNullOrEmpty(orderBy)) sql += " ORDER BY " + orderBy; 
			SqlResult res = Kernel.Connection.ExecuteQuery(sql);
			for(int i = 0 ; i < res.RowCount ; i++)
			{
				Students i_Students = new Students();
				
				i_Students.Id = res.GetInt64(i, "Id", -1);
				i_Students.FirstName = res.GetString(i, "FirstName", "");
				i_Students.LastName = res.GetString(i, "LastName", "");
				i_Students.EmailAddress = res.GetString(i, "EmailAddress", "");
				i_Students.Password = res.GetString(i, "Password", "");
				i_Students.PasswordSalt = res.GetString(i, "PasswordSalt", "");
				i_Students.IsDeleted = res.GetBoolean(i, "IsDeleted", false);
				i_Students.Group = res.GetString(i, "Group", "");
				
				this.Add(i_Students);
			}
		}
	}
	
}
