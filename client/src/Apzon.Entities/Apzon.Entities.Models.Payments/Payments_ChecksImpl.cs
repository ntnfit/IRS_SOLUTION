using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Payments;

public class Payments_ChecksImpl
{
	public string AcctNum { get; set; }

	public string BankCode { get; set; }

	public string Branch { get; set; }

	public int CheckAbsEntry { get; set; }

	public string CheckAccount { get; set; }

	public int CheckNumber { get; set; }

	public double CheckSum { get; set; }

	public int Count { get; set; }

	public string CountryCode { get; set; }

	public string Details { get; set; }

	public DateTime DueDate { get; set; }

	public int LineNum { get; set; }

	public bool ManualCheck { get; set; }

	public bool Trnsfrable { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
