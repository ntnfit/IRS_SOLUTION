using System;

namespace Apzon.Entities.Models.Documents;

public class DocumentSBAImpl
{
	public int AbsID { get; set; }

	public string BpCode { get; set; }

	public string BpName { get; set; }

	public string BpType { get; set; }

	public int CntctCode { get; set; }

	public string Tel1 { get; set; }

	public string E_MailL { get; set; }

	public int Series { get; set; }

	public int Number { get; set; }

	public char? Method { get; set; }

	public int ListNum { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public DateTime? TermDate { get; set; }

	public DateTime? SignDate { get; set; }

	public string Descript { get; set; }

	public char? Type { get; set; }

	public char? UseDiscnt { get; set; }

	public string Status { get; set; }

	public int Owner { get; set; }

	public char? Renewal { get; set; }

	public decimal SettleProb { get; set; }

	public int RemindVal { get; set; }

	public char? RemindUnit { get; set; }

	public string Remarks { get; set; }

	public int AmendedTo { get; set; }

	public int AtchEntry { get; set; }

	public string Handwrtten { get; set; }

	public string PayMethod { get; set; }

	public int GroupNum { get; set; }

	public string PIndicator { get; set; }
}
