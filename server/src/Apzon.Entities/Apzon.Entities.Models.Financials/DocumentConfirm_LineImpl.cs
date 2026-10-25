using System;

namespace Apzon.Entities.Models.Financials;

public class DocumentConfirm_LineImpl
{
	public int DocEntry { get; set; }

	public int LineId { get; set; }

	public int VisOrder { get; set; }

	public string Object { get; set; }

	public int LogInst { get; set; }

	public string U_DocEntry { get; set; }

	public string U_DocType { get; set; }

	public DateTime U_Date { get; set; }

	public int U_Time { get; set; }

	public string U_Status { get; set; }
}
