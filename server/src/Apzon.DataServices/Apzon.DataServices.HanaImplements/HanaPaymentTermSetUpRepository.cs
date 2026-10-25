using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

internal class HanaPaymentTermSetUpRepository : PaymentTermSetupRepository
{
	public HanaPaymentTermSetUpRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string Save(DataTable dtDoc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in dtDoc.Rows)
			{
				if (Function.ParseInt(row["GroupNum"]) != 0)
				{
					list.Add(new BodyRequestObject
					{
						BodyObject = Mapping(row),
						Action = Enums.HttpAction.Update,
						ObjectType = ApzObjectTypes.oPaymentTermsTypes,
						Key = Function.ParseInt(row["GroupNum"])
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						BodyObject = Mapping(row),
						Action = Enums.HttpAction.AddNew,
						ObjectType = ApzObjectTypes.oPaymentTermsTypes,
						OutObjectKey = true
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Mapping(DataRow row)
	{
		try
		{
			string text = "{";
			if (Function.ParseInt(row["GroupNum"]) != 0)
			{
				text += Function.GetJsonString("GroupNumber", Function.ParseInt(row["GroupNum"]));
			}
			text += Function.GetJsonString("PaymentTermsGroupName", Function.RawText(row["PymntGroup"]));
			text = Function.ToString(row["PayDuMonth"]) switch
			{
				"E" => text + Function.GetJsonString("StartFrom", BoPayTermDueTypes.pdt_MonthEnd), 
				"H" => text + Function.GetJsonString("StartFrom", BoPayTermDueTypes.pdt_HalfMonth), 
				"Y" => text + Function.GetJsonString("StartFrom", BoPayTermDueTypes.pdt_MonthStart), 
				_ => text + Function.GetJsonString("StartFrom", BoPayTermDueTypes.pdt_None), 
			} + Function.GetJsonString("NumberOfAdditionalMonths", Function.ParseInt(row["ExtraMonth"]));
			text += Function.GetJsonString("NumberOfAdditionalDays", Function.ParseInt(row["ExtraDays"]));
			text += Function.GetJsonString("CreditLimit", Function.ParseDecimal(row["CredLimit"]));
			text += Function.GetJsonString("GeneralDiscount", Function.ParseDecimal(row["VolumDscnt"]));
			text += Function.GetJsonString("InterestOnArrears", Function.ParseDecimal(row["LatePyChrg"]));
			if (Function.ParseLong(row["ListNum"]) != 0L)
			{
				text += Function.GetJsonString("PriceListNo", Function.ParseLong(row["ListNum"]));
			}
			text += Function.GetJsonString("LoadLimit", Function.ParseDecimal(row["ObligLimit"]));
			text = Function.ToString(row["OpenRcpt"]) switch
			{
				"3" => text + Function.GetJsonString("OpenReceipt", BoOpenIncPayment.oip_Cash), 
				"1" => text + Function.GetJsonString("OpenReceipt", BoOpenIncPayment.oip_Checks), 
				"4" => text + Function.GetJsonString("OpenReceipt", BoOpenIncPayment.oip_Credit), 
				"2" => text + Function.GetJsonString("OpenReceipt", BoOpenIncPayment.oip_BankTransfer), 
				_ => text + Function.GetJsonString("OpenReceipt", BoOpenIncPayment.oip_No), 
			};
			if (!string.IsNullOrEmpty(Function.ToString(row["DiscCode"])))
			{
				text += Function.GetJsonString("DiscountCode", Function.RawText(row["DiscCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["DunningCod"])))
			{
				text += Function.GetJsonString("DunningCode", Function.RawText(row["DunningCod"]));
			}
			text = Function.ToString(row["BslineDate"]) switch
			{
				"P" => text + Function.GetJsonString("BaselineDate", BoBaselineDate.bld_PostingDate), 
				"S" => text + Function.GetJsonString("BaselineDate", BoBaselineDate.bld_SystemDate), 
				"C" => text + Function.GetJsonString("BaselineDate", BoBaselineDate.bld_ClosingDate), 
				_ => text + Function.GetJsonString("BaselineDate", BoBaselineDate.bld_TaxDate), 
			} + Function.GetJsonString("NumberOfToleranceDays", Function.ParseLong(row["TolDays"]));
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override string Remove(int groupNum, string sessionId)
	{
		try
		{
			List<BodyRequestObject> lstBodyRequestObjects = new List<BodyRequestObject>
			{
				new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oPaymentTermsTypes,
					OutObjectKey = true,
					Key = groupNum,
					Action = Enums.HttpAction.Delete
				}
			};
			return DataService.ProcessMutiTransaction(sessionId, lstBodyRequestObjects).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
