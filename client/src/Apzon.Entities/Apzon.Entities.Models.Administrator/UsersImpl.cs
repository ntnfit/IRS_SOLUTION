using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class UsersImpl
{
	public int USERID { get; set; }

	public string PASSWORD { get; set; }

	public string USER_CODE { get; set; }

	public string U_NAME { get; set; }

	public int INTERNAL_K { get; set; }

	public int GROUPS { get; set; }

	public string SUPERUSER { get; set; }

	public string E_Mail { get; set; }

	public string PortNum { get; set; }

	public string Fax { get; set; }

	public string Locked { get; set; }

	public int Department { get; set; }

	public int Branch { get; set; }

	public int userSign { get; set; }

	public DateTime? createDate { get; set; }

	public int userSign2 { get; set; }

	public DateTime? updateDate { get; set; }

	public DateTime? lastLogin { get; set; }

	public string DfltsGroup { get; set; }

	public string WallPaper { get; set; }

	public string WllPprDsp { get; set; }

	public bool IsChangePassword { get; set; }

	public List<UserBranchAssignmentImpls> UserBranchAssignment { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public int empID { get; set; }

	public string Name { get; set; }

	public string NameBranch { get; set; }

	public DataTable DTAuth { get; set; }
}
