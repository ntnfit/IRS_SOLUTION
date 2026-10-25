using System;

namespace Apzon.Entities.Models.Return;

public class ORDRImpl
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

	public double VatSum { get; set; }

	public double VatSumFC { get; set; }

	public double DiscPrcnt { get; set; }

	public double DiscSum { get; set; }

	public double DiscSumFC { get; set; }

	public string DocCur { get; set; }

	public double DocRate { get; set; }

	public double DocTotal { get; set; }

	public double DocTotalFC { get; set; }

	public double PaidToDate { get; set; }

	public double PaidFC { get; set; }

	public double GrosProfit { get; set; }

	public double GrosProfFC { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Comments { get; set; }

	public string JrnlMemo { get; set; }

	public int TransId { get; set; }

	public int ReceiptNum { get; set; }

	public int GroupNum { get; set; }

	public DateTime DocTime { get; set; }

	public int SlpCode { get; set; }

	public int TrnspCode { get; set; }

	public int CntctCode { get; set; }

	public double VatSumSy { get; set; }

	public double DiscSumSy { get; set; }

	public double DocTotalSy { get; set; }

	public double PaidSys { get; set; }

	public DateTime UpdateDate { get; set; }

	public string IsICT { get; set; }

	public DateTime CreateDate { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public int FinncPriod { get; set; }

	public int UserSign { get; set; }

	public int UserSign2 { get; set; }

	public DateTime VatDate { get; set; }

	public double PaidSum { get; set; }

	public double PaidSumFc { get; set; }

	public double PaidSumSc { get; set; }

	public string CurSource { get; set; }

	public double Cashremainder { get; set; }

	public string IsRIV { get; set; }

	public string TypeDisc { get; set; }

	public string Confirmed { get; set; }

	public string PoPrss { get; set; }

	public string ShipToCode { get; set; }

	public string PayToCode { get; set; }

	public double TotalBefore { get; set; }
}
