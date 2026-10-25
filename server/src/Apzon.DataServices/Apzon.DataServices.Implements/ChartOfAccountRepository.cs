using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ChartOfAccountRepository : MasterRepository, IChartOfAccount, IDisposable
{
	protected new readonly IUnitOfWork _unitOfWork;

	protected ApzObjectTypes _objectType = ApzObjectTypes.oChartOfAccounts;

	protected string _keyFieldName = "AcctCode";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string>
	{
		{ "Code", "AcctCode" },
		{ "AcctCurrency", "ActCurr" },
		{ "BPLID", "BPLID" },
		{ "DataExportCode", "ExportCode" },
		{ "DatevAccount", "DatevAcct" },
		{ "DefaultVatGroup", "DfltVat" },
		{ "Details", "Details" },
		{ "ExternalCode", "AccntntCod" },
		{ "FatherAccountKey", "FatherNum" },
		{ "ForeignName", "FrgnName" },
		{ "FormatCode", "FormatCode" },
		{ "FrozenFrom", "FrozenFrom" },
		{ "FrozenRemarks", "FrozenComm" },
		{ "FrozenTo", "FrozenTo" },
		{ "LoadingFactorCode", "OverCode" },
		{ "LoadingFactorCode2", "OverCode2" },
		{ "LoadingFactorCode3", "OverCode3" },
		{ "LoadingFactorCode4", "OverCode4" },
		{ "LoadingFactorCode5", "OverCode5" },
		{ "Name", "AcctName" },
		{ "PlanningLevel", "PlngLevel" },
		{ "ProjectCode", "Project" },
		{ "TransactionCode", "TransCode" },
		{ "ValidFrom", "ValidFrom" },
		{ "ValidTo", "ValidTo" }
	};

	public ChartOfAccountRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public string Delete(int code, string sessionId)
	{
		try
		{
			return base.Remove(code, sessionId, _objectType).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult Save(List<ChartOfAccountsImpl> coa, string sessionId)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, _objectType.ToString());
			DataTable tableStructure = _unitOfWork.Common.GetTableStructure("OACT");
			for (int i = 0; i < coa.Count; i++)
			{
				DataRow row = tableStructure.NewRow();
				Function.ObjectToRow(coa[i], row);
				tableStructure.Rows.Add(row);
			}
			return base.Save(tableStructure, sessionId, _objectType, _keyFieldName, DicFieldTags);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
		text = text + "<AccountType>" + Function.ToString(boAccountTypes) + "</AccountType>";
		text = text + "<ActiveAccount>" + ((Function.ToString(dr["Postable"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)) + "</ActiveAccount>";
		text = text + "<AllowChangeVatGroup>" + ((Function.ToString(dr["VatChange"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)) + "</AllowChangeVatGroup>";
		text = text + "<BudgetAccount>" + ((Function.ToString(dr["Budget"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</BudgetAccount>";
		text = text + "<CashAccount>" + ((Function.ToString(dr["CashBox"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</CashAccount>";
		text = text + "<LiableForAdvances>" + ((Function.ToString(dr["Advance"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)) + "</LiableForAdvances>";
		text = text + "<LockManualTransaction>" + ((Function.ToString(dr["LocManTran"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</LockManualTransaction>";
		text = text + "<Protected>" + ((Function.ToString(dr["Protected"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</Protected>";
		text = text + "<RateConversion>" + ((Function.ToString(dr["RateTrans"]) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES)) + "</RateConversion>";
		text = text + "<ReconciledAccount>" + ((Function.ToString(dr["RealAcct"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</ReconciledAccount>";
		text = text + "<RevaluationCoordinated>" + ((Function.ToString(dr["RevalMatch"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</RevaluationCoordinated>";
		text = text + "<TaxExemptAccount>" + ((Function.ToString(dr["ExmIncome"]) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO)) + "</TaxExemptAccount>";
		text = text + "<ValidRemarks>" + ((Function.ToString(dr["ValidFor"]) == "Y") ? "Y" : "N") + "</ValidRemarks>";
		text = text + "<FrozenFor>" + ((Function.ToString(dr["FrozenFor"]) == "Y") ? "Y" : "N") + "</FrozenFor>";
		text = text + "<ProjectRelevant>" + ((Function.ToString(dr["PrjRelvnt"]) == "Y") ? "Y" : "N") + "</ProjectRelevant>";
		text = text + "<DistributionRule2Relevant>" + ((Function.ToString(dr["Dim2Relvnt"]) == "Y") ? "Y" : "N") + "</DistributionRule2Relevant>";
		text = text + "<DistributionRule3Relevant>" + ((Function.ToString(dr["Dim3Relvnt"]) == "Y") ? "Y" : "N") + "</DistributionRule3Relevant>";
		text = text + "<DistributionRule4Relevant>" + ((Function.ToString(dr["Dim4Relvnt"]) == "Y") ? "Y" : "N") + "</DistributionRule4Relevant>";
		text = text + "<DistributionRule5Relevant>" + ((Function.ToString(dr["Dim5Relvnt"]) == "Y") ? "Y" : "N") + "</DistributionRule5Relevant>";
		text = text + "<DistributionRuleRelevant>" + ((Function.ToString(dr["Dim1Relvnt"]) == "Y") ? "Y" : "N") + "</DistributionRuleRelevant>";
		text = text + "<DatevAutoAccount>" + ((Function.ToString(dr["DatevAutoA"]) == "Y") ? "Y" : "N") + "</DatevAutoAccount>";
		if (Function.ToString(dr["Category"]) != "")
		{
			text = text + "<Category>" + Function.ToString(dr["Category"]) + "</Category>";
		}
		return text;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
