using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.MasterData;

public class ChartOfAccountsImpl
{
	public int Levels { get; set; }

	public string PurpCode { get; set; }

	public string ActType { get; set; }

	public string ActCurr { get; set; }

	public string Postable { get; set; }

	public string VatChange { get; set; }

	public string MultiLink { get; set; }

	public int BPLID { get; set; }

	public string BPLName { get; set; }

	public double Balance { get; set; }

	public double FcTotal { get; set; }

	public double SysTotal { get; set; }

	public string Budget { get; set; }

	public string CashBox { get; set; }

	public int category { get; set; }

	public string AcctCode { get; set; }

	public string ExportCode { get; set; }

	public int DatevAcct { get; set; }

	public string DatevAutoA { get; set; }

	public string DatevFirst { get; set; }

	public string DfltVat { get; set; }

	public string Details { get; set; }

	public string Dim2Relvnt { get; set; }

	public string Dim3Relvnt { get; set; }

	public string Dim4Relvnt { get; set; }

	public string Dim5Relvnt { get; set; }

	public string Dim1Relvnt { get; set; }

	public string AccntntCod { get; set; }

	public int ExtrMatch { get; set; }

	public string FatherNum { get; set; }

	public string FrgnName { get; set; }

	public string FormatCode { get; set; }

	public int IntrMatch { get; set; }

	public string FrozenFor { get; set; }

	public DateTime FrozenFrom { get; set; }

	public DateTime FrozenTo { get; set; }

	public string FrozenComm { get; set; }

	public string Advance { get; set; }

	public string OverCode { get; set; }

	public string OverCode2 { get; set; }

	public string OverCode3 { get; set; }

	public string OverCode4 { get; set; }

	public string OverCode5 { get; set; }

	public string OverType { get; set; }

	public string LocManTran { get; set; }

	public string AcctName { get; set; }

	public string PlngLevel { get; set; }

	public string Fixed { get; set; }

	public string Project { get; set; }

	public string PrjRelvnt { get; set; }

	public string Protected { get; set; }

	public string RateTrans { get; set; }

	public string RealAcct { get; set; }

	public string RefCode { get; set; }

	public string RevalMatch { get; set; }

	public string ExmIncome { get; set; }

	public string ValidFor { get; set; }

	public DateTime ValidFrom { get; set; }

	public DateTime ValidTo { get; set; }

	public string ValidComm { get; set; }

	public string TransCode { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATRegNum { get; set; }
}
