using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.LandedCosts;

public class LandedCostImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public DateTime DocDate { get; set; }

	public DateTime DocDueDate { get; set; }

	public string CardCode { get; set; }

	public string SuppName { get; set; }

	public string AgentCode { get; set; }

	public string AgentName { get; set; }

	public string DocStatus { get; set; }

	public string Descr { get; set; }

	public string Ref1 { get; set; }

	public string DocCur { get; set; }

	public double DocRate { get; set; }

	public double ExpCustom { get; set; }

	public double ActCustom { get; set; }

	public double Vat1 { get; set; }

	public double Vat2 { get; set; }

	public double BeforeVat { get; set; }

	public double DocTotal { get; set; }

	public double CostSum { get; set; }

	public double ExCustomFC { get; set; }

	public double Vat1FC { get; set; }

	public double Vat2FC { get; set; }

	public double BeforVatFC { get; set; }

	public double AcCustomFC { get; set; }

	public double DocTotalFC { get; set; }

	public double CostSumFC { get; set; }

	public int Series { get; set; }

	public string incCustom { get; set; }

	public double Cost_Match { get; set; }

	public double C_Match_FC { get; set; }

	public string BillOfLad { get; set; }

	public int TrnspCode { get; set; }

	public int JdtNum { get; set; }

	public string JdtMemo { get; set; }

	public List<LandedCost_ItemLineImpl> ItemLines { get; set; }

	public List<LandedCost_CostLineImpl> CostLines { get; set; }

	public int AtcEntry { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
