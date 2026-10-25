using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Production;

public class IssueForProduction_Impl_OIGE
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public string DocType { get; set; }

	public string CANCELED { get; set; }

	public string Handwrtten { get; set; }

	public string Printed { get; set; }

	public string DocStatus { get; set; }

	public string ObjType { get; set; }

	public DateTime DocDate { get; set; }

	public DateTime DocDueDate { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string Address { get; set; }

	public string NumAtCard { get; set; }

	public decimal VatSum { get; set; }

	public decimal VatSumFC { get; set; }

	public decimal DiscPrcnt { get; set; }

	public decimal DiscSum { get; set; }

	public decimal DiscSumFC { get; set; }

	public string DocCur { get; set; }

	public decimal DocRate { get; set; }

	public decimal DocTotal { get; set; }

	public decimal DocTotalFC { get; set; }

	public decimal PaidToDate { get; set; }

	public decimal PaidFC { get; set; }

	public decimal GrosProfit { get; set; }

	public decimal GrosProfFC { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Comments { get; set; }

	public string JrnlMemo { get; set; }

	public int TransId { get; set; }

	public int ReceiptNum { get; set; }

	public int GroupNum { get; set; }

	public TimeSpan DocTime { get; set; }

	public int SlpCode { get; set; }

	public int TrnspCode { get; set; }

	public int CntctCode { get; set; }

	public decimal VatSumSy { get; set; }

	public decimal DiscSumSy { get; set; }

	public decimal DocTotalSy { get; set; }

	public decimal PaidSys { get; set; }

	public DateTime UpdateDate { get; set; }

	public string IsICT { get; set; }

	public DateTime CreateDate { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public int FinncPriod { get; set; }

	public int UserSign { get; set; }

	public int UserSign2 { get; set; }

	public DateTime VatDate { get; set; }

	public decimal PaidSum { get; set; }

	public decimal PaidSumFc { get; set; }

	public decimal PaidSumSc { get; set; }

	public string CurSource { get; set; }

	public decimal Cashremainder { get; set; }

	public string IsRIV { get; set; }

	public string Requester { get; set; }

	public string ReqName { get; set; }

	public short Branch { get; set; }

	public short Department { get; set; }

	public DateTime ReqDate { get; set; }

	public int AtcEntry { get; set; }

	public List<IssueForProductionLine_Impl_IGE1> Line_IGE1 { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }
}
