using System;

namespace Apzon.Entities.Models.Financials;

public class AccountingSalaryImpl
{
	public int DocNum { get; set; }

	public string Status { get; set; }

	public string Period { get; set; }

	public DateTime? DocDate { get; set; }

	public DateTime? OutGoingDate { get; set; }

	public string Remark { get; set; }

	public string Remark1 { get; set; }

	public int TransId { get; set; }

	public string DocEntry { get; set; }
}
