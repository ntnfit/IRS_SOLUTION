using System;

namespace Apzon.Entities.Models.Financials;

public class DocumentConfirmImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public int Period { get; set; }

	public short Instance { get; set; }

	public int Series { get; set; }

	public string Handwrtten { get; set; }

	public string Canceled { get; set; }

	public string Object { get; set; }

	public int LogInst { get; set; }

	public int UserSign { get; set; }

	public string Transfered { get; set; }

	public string Status { get; set; }

	public DateTime CreateDate { get; set; }

	public int CreateTime { get; set; }

	public DateTime UpdateDate { get; set; }

	public int UpdateTime { get; set; }

	public string DataSource { get; set; }

	public string RequestStatus { get; set; }

	public string Creator { get; set; }

	public string Remark { get; set; }

	public string U_UserCode { get; set; }

	public string U_DocEntry { get; set; }

	public string U_Style { get; set; }
}
