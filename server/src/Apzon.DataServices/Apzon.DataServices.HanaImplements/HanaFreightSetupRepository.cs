using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaFreightSetupRepository : FreightSetupReopsitory
{
	public HanaFreightSetupRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string Save(DataTable doc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in doc.Rows)
			{
				int num = Function.ParseInt(row["ExpnsCode"]);
				Enums.HttpAction action = ((num == 0) ? Enums.HttpAction.AddNew : ((num > 0) ? Enums.HttpAction.Update : Enums.HttpAction.Delete));
				list.Add(new BodyRequestObject
				{
					BodyObject = Mapping(row),
					Action = action,
					ObjectType = ApzObjectTypes.oAdditionalExpenses,
					Key = Function.ParseInt(Math.Abs(num)),
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public string Mapping(DataRow row)
	{
		try
		{
			string text = "{";
			int num = Function.ParseInt(row["ExpnsCode"]);
			if (num != 0)
			{
				text += Function.GetJsonString("ExpensCode", Function.ParseInt(Math.Abs(num)));
			}
			if (num >= 0)
			{
				text += Function.GetJsonString("Name", row["ExpnsName"]);
				text += Function.GetJsonString("RevenuesAccount", row["RevAcct"]);
				text += Function.GetJsonString("ExpenseAccount", row["ExpnsAcct"]);
				text += Function.GetJsonString("OutputVATGroup", row["VatGroupI"]);
				text += Function.GetJsonString("InputVATGroup", row["VatGroupO"]);
				text += Function.GetJsonString("FixedAmountRevenues", row["RevFixSum"]);
				text += Function.GetJsonString("FixedAmountExpenses", row["ExpFixSum"]);
				text += Function.GetJsonString("DistributionMethod", row["DistrbMthd"]);
				text += Function.GetJsonString("WTLiable", row["WTLiable"]);
				text += Function.GetJsonString("DistributionRule", row["OcrCode"]);
				text += Function.GetJsonString("Project", row["Project"]);
				text += Function.GetJsonString("LastPurchasePrice", row["LstPchPrce"]);
				text += Function.GetJsonString("Stock", row["Stock"]);
				text += Function.GetJsonString("DrawingMethod", row["BaseMethod"]);
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "";
		}
	}
}
