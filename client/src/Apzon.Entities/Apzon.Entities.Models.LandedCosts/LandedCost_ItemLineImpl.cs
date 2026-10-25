using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.LandedCosts;

public class LandedCost_ItemLineImpl
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public int BaseType { get; set; }

	public int BaseEntry { get; set; }

	public string ItemCode { get; set; }

	public string Dscription { get; set; }

	public double Quantity { get; set; }

	public double PriceFOB { get; set; }

	public double Rate { get; set; }

	public double Custom { get; set; }

	public double CustomFC { get; set; }

	public double Cost { get; set; }

	public double CostFC { get; set; }

	public double PriceAtWH { get; set; }

	public double PricAtWHFC { get; set; }

	public double LineTotal { get; set; }

	public double TotalFrgn { get; set; }

	public double Volume { get; set; }

	public int VolumeUoM { get; set; }

	public double Weight1 { get; set; }

	public int Weight1UnitCode { get; set; }

	public double Weight2 { get; set; }

	public int Weight2UnitCode { get; set; }

	public string CardCode { get; set; }

	public string Reference { get; set; }

	public double FactNoCust { get; set; }

	public double FacWthCust { get; set; }

	public string UseBaseUn { get; set; }

	public string BlockNum { get; set; }

	public string ImportLog { get; set; }

	public string OrigWhs { get; set; }

	public string WhsCode { get; set; }

	public int ReleaseNum { get; set; }

	public double VarCosts { get; set; }

	public double ConstCosts { get; set; }

	public double VarCostsFR { get; set; }

	public double CnstCostFR { get; set; }

	public double UserCustom { get; set; }

	public double UsrCusomFC { get; set; }

	public double FobValue { get; set; }

	public double FobValueFC { get; set; }

	public double TtlExpndLC { get; set; }

	public double TtlExpndFC { get; set; }

	public double TtlCostLC { get; set; }

	public double TtlCostFC { get; set; }

	public double TtlCustLC { get; set; }

	public double TtlCustFC { get; set; }

	public double TtlVolume { get; set; }

	public int OrigLine { get; set; }

	public int BaseRowNum { get; set; }

	public double TtlCustSC { get; set; }

	public double TtlExpndSC { get; set; }

	public double FobnLaC { get; set; }

	public double FobnLaCFC { get; set; }

	public string Project { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public int PriceList { get; set; }

	public string CostOH { get; set; }

	public string StockEval { get; set; }

	public int OrigRow { get; set; }

	public string Currency { get; set; }

	public double CustRate { get; set; }

	public string VatGroup { get; set; }

	public double VatPrcnt { get; set; }

	public double ExciseSum { get; set; }

	public double ExcisSumFC { get; set; }

	public string ExcInStk { get; set; }

	public double CustomSum { get; set; }

	public double CustSumFC { get; set; }

	public string CstmInStk { get; set; }

	public double CstmVatSum { get; set; }

	public double CstmVatFC { get; set; }

	public string CstmVatStk { get; set; }

	public string CCDNumber { get; set; }

	public double FobVal2 { get; set; }

	public double FobVal2FC { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public int UnitCode { get; set; }

	public int UnitCode1 { get; set; }

	public int UnitCode2 { get; set; }
}
