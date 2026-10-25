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

public class HanaSTDPriceListsRepository : STDPriceListsRepository
{
	public HanaSTDPriceListsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService, IUnitOfWork datauUnitOfWork)
		: base(serviceUnitOfWork, dataService, datauUnitOfWork)
	{
	}

	public override string Save(DataTable dt, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num = Function.ParseInt(dt.Rows[i]["ListNum"]);
				string jsonObject = MappingJson(dt.Rows[i]);
				if (num > 0)
				{
					httpResult = DataService.ProcessJson(ApzObjectTypes.oPriceLists, sessionId, jsonObject, Enums.HttpAction.Update, num);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						Logging.Write("WATCH", string.Format("Update PriceList Error \t" + httpResult.Message));
						return httpResult.Message;
					}
					Logging.Write("WATCH", string.Format("Update PriceList \t" + num));
				}
				else
				{
					httpResult = DataService.ProcessJson(ApzObjectTypes.oPriceLists, sessionId, jsonObject);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						Logging.Write("WATCH", string.Format("Add PriceList Error \t" + httpResult.Message));
						return httpResult.Message;
					}
					Logging.Write("WATCH", string.Format("Add PriceList \t" + httpResult.Content));
				}
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Delete(int listNum, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oPriceLists, sessionId, "", Enums.HttpAction.Delete, listNum);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Delete PriceList \t" + httpResult.Content));
				return httpResult.Message;
			}
			Logging.Write("WATCH", string.Format("Delete PriceList Error \t" + httpResult.Message));
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string SaveItemPrice(DataTable dt, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string key = Function.ToString(dt.Rows[i]["ItemCode"]);
				string bodyObject = MappingJsonItemPrice(dt.Rows[i]);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oItems,
					OutObjectKey = true,
					Action = Enums.HttpAction.Update,
					BodyObject = bodyObject,
					Key = key
				});
			}
			httpResult = DataService.ProcessMutiTransaction(sessionId, list, isTryProcess: false, replaceOnPatch: false);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Update Item Price Success\t" + httpResult.Content));
				return httpResult.Message;
			}
			Logging.Write("WATCH", string.Format("Update Item Price Error \t" + httpResult.Message));
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string MappingJson(DataRow row)
	{
		string text = "";
		try
		{
			int num = Function.ParseInt(row["ListNum"]);
			text += "{ ";
			if (num > 0)
			{
				text += Function.GetJsonString("PriceListNo", Function.ParseInt(row["ListNum"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RoundSys"])))
			{
				text += Function.GetJsonString("RoundingMethod", (Function.ParseInt(row["RoundSys"]) switch
				{
					0 => BoRoundingMethod.borm_NoRounding, 
					1 => BoRoundingMethod.borm_RoundToFullDecAmount, 
					2 => BoRoundingMethod.borm_RoundToFullAmount, 
					3 => BoRoundingMethod.borm_RoundToFullTensAmount, 
					4 => BoRoundingMethod.borm_FixedEnding, 
					5 => BoRoundingMethod.borm_FixedInterval, 
					_ => BoRoundingMethod.borm_NoRounding, 
				}).ToString());
			}
			if (Function.ParseInt(row["GroupCode"]) > 0)
			{
				text += Function.GetJsonString("GroupNum", Function.ParseInt(row["GroupCode"]));
			}
			if (row["BASE_NUM"] != null && row["BASE_NUM"] != DBNull.Value)
			{
				int num2 = Function.ParseInt(row["BASE_NUM"]);
				if (num2 < 0)
				{
					num2 = -num2;
				}
				text += Function.GetJsonString("BasePriceList", num2);
			}
			if (Function.ParseDecimal(row["Factor"]) > 0m)
			{
				text += Function.GetJsonString("Factor", Function.ParseDecimal(row["Factor"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["ListName"])))
			{
				text += Function.GetJsonString("PriceListName", Function.ToString(row["ListName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["IsGrossPrc"])))
			{
				text += Function.GetJsonString("IsGrossPrice", ((row["IsGrossPrc"].ToString() == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO).ToString());
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["ValidFor"])))
			{
				BoYesNoEnum boYesNoEnum;
				if (Function.ToString(row["ValidFor"]) == "Y")
				{
					boYesNoEnum = BoYesNoEnum.tYES;
					if (Function.ParseDateTime(row["ValidFrom"]).HasValue)
					{
						text += Function.GetJsonString("ValidFrom", Convert.ToDateTime(row["ValidFrom"]).ToString("yyyy-MM-dd"));
					}
					if (Function.ParseDateTime(row["ValidTo"]).HasValue)
					{
						text += Function.GetJsonString("ValidTo", Convert.ToDateTime(row["ValidTo"]).ToString("yyyy-MM-dd"));
					}
				}
				else
				{
					boYesNoEnum = BoYesNoEnum.tNO;
				}
				text += Function.GetJsonString("Active", boYesNoEnum.ToString());
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["AddCurr1"])))
			{
				text += Function.GetJsonString("DefaultAdditionalCurrency1", Function.ToString(row["AddCurr1"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["AddCurr2"])))
			{
				text += Function.GetJsonString("DefaultAdditionalCurrency2", Function.ToString(row["AddCurr2"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["PrimCurr"])))
			{
				text += Function.GetJsonString("DefaultPrimeCurrency", Function.ToString(row["PrimCurr"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RoundRule"])))
			{
				text += Function.GetJsonString("RoundingRule", Function.ToString(Function.ToString(row["RoundRule"]) switch
				{
					"R" => BoRoundingRule.borrRoundOff, 
					"C" => BoRoundingRule.borrRoundUp, 
					"F" => BoRoundingRule.borrRoundDown, 
					_ => BoRoundingRule.borrRoundOff, 
				}));
			}
			if (Function.ParseDouble(row["ExtAmount"]) > 0.0)
			{
				text += Function.GetJsonString("FixedAmount", Function.ParseDecimal(row["ExtAmount"]));
			}
			text += DataService.MappingUdf(row);
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	private string MappingJsonItemPrice(DataRow row)
	{
		if (row == null)
		{
			throw new NullReferenceException("Data save can not be null");
		}
		string text = "{\"ItemPrices\":[{";
		text += Function.GetJsonString("PriceList", row["PriceList"]);
		text += Function.GetJsonString("Price", row["Price"]);
		text += Function.GetJsonString("Currency", row["Currency"]);
		text += Function.GetJsonString("AdditionalPrice1", row["AddPrice1"]);
		if (!string.IsNullOrEmpty(Function.ToString(row["Currency1"])))
		{
			text += Function.GetJsonString("AdditionalCurrency1", row["Currency1"]);
		}
		text += Function.GetJsonString("AdditionalPrice2", row["AddPrice2"]);
		if (!string.IsNullOrEmpty(Function.ToString(row["Currency2"])))
		{
			text += Function.GetJsonString("AdditionalCurrency2", row["Currency2"]);
		}
		if (Function.ParseInt(row["BasePLNum"]) > 0)
		{
			text += Function.GetJsonString("BasePriceList", row["BasePLNum"]);
		}
		if (Function.ParseDecimal(row["Factor"]) > 0m)
		{
			text += Function.GetJsonString("Factor", row["Factor"]);
		}
		return text + "}]}";
	}
}
