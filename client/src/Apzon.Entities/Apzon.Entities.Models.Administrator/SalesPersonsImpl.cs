using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class SalesPersonsImpl
{
	public bool Active { get; set; }

	public double CommissionForSalesEmployee { get; set; }

	public int CommissionGroup { get; set; }

	public int EmployeeID { get; set; }

	public bool Locked { get; set; }

	public string Remarks { get; set; }

	public int SalesEmployeeCode { get; set; }

	public string SalesEmployeeName { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
