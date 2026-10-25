using System.Data;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaChartOfAccountRepository : ChartOfAccountRepository
{
	public HanaChartOfAccountRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override string CustomMapping(DataRow dr)
	{
		string text = "";
		BoAccountTypes boAccountTypes = BoAccountTypes.at_Other;
		switch (Function.ToString(dr["ActType"]))
		{
		case "I":
			boAccountTypes = BoAccountTypes.at_Revenues;
			break;
		case "E":
			boAccountTypes = BoAccountTypes.at_Expenses;
			break;
		case "N":
			boAccountTypes = BoAccountTypes.at_Other;
			break;
		}
		text += Function.GetJsonString("AccountType", Function.ToString(boAccountTypes));
		text += Function.GetJsonString("ActiveAccount", (Function.ToString(dr["Postable"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
		text += Function.GetJsonString("AllowChangeVatGroup", (Function.ToString(dr["VatChange"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
		text += Function.GetJsonString("BudgetAccount", (Function.ToString(dr["Budget"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("CashAccount", (Function.ToString(dr["CashBox"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("LiableForAdvances", (Function.ToString(dr["Advance"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
		text += Function.GetJsonString("LockManualTransaction", (Function.ToString(dr["LocManTran"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("Protected", (Function.ToString(dr["Protected"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("RateConversion", (Function.ToString(dr["RateTrans"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
		text += Function.GetJsonString("ReconciledAccount", (Function.ToString(dr["RealAcct"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("RevaluationCoordinated", (Function.ToString(dr["RevalMatch"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("TaxExemptAccount", (Function.ToString(dr["ExmIncome"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
		text += Function.GetJsonString("ValidRemarks", (Function.ToString(dr["ValidFor"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("FrozenFor", (Function.ToString(dr["FrozenFor"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("ProjectRelevant", (Function.ToString(dr["PrjRelvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DistributionRule2Relevant", (Function.ToString(dr["Dim2Relvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DistributionRule3Relevant", (Function.ToString(dr["Dim3Relvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DistributionRule4Relevant", (Function.ToString(dr["Dim4Relvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DistributionRule5Relevant", (Function.ToString(dr["Dim5Relvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DistributionRuleRelevant", (Function.ToString(dr["Dim1Relvnt"]) == "Y") ? "Y" : "N");
		text += Function.GetJsonString("DatevAutoAccount", (Function.ToString(dr["DatevAutoA"]) == "Y") ? "Y" : "N");
		if (Function.ToString(dr["Category"]) != "")
		{
			text += Function.GetJsonString("Category", Function.ToString(dr["Category"]));
		}
		return text;
	}
}
