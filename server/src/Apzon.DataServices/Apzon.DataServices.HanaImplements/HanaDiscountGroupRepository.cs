using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaDiscountGroupRepository : DiscountGroupRepository
{
	public HanaDiscountGroupRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public override async Task<string> UpdateData(string sessionId, DataSet ds)
	{
		try
		{
			string jsonObject = Mapping(ds, EnumCommandType.Update);
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oDiscountGroups, sessionId, jsonObject, Enums.HttpAction.Update, ds.Tables[0].Rows[0]["AbsEntry"]);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override async Task<string> AddNew(string sessionId, DataSet ds)
	{
		try
		{
			string jsonObject = Mapping(ds, EnumCommandType.Add);
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oDiscountGroups, sessionId, jsonObject, Enums.HttpAction.AddNew, ds.Tables[0].Rows[0]["AbsEntry"]);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Mapping(DataSet ds, EnumCommandType commandType)
	{
		string text = "{";
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			text = text + "\"AbsEntry\":" + Function.ParseInt(dataRow["AbsEntry"]) + ",";
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Type"])))
			{
				text = Function.ToString(dataRow["Type"]) switch
				{
					"A" => text + Function.GetJsonString("Type", "dgt_AllBPs"), 
					"C" => text + Function.GetJsonString("Type", "dgt_CustomerGroup"), 
					"V" => text + Function.GetJsonString("Type", "dgt_VendorGroup"), 
					"S" => text + Function.GetJsonString("Type", "dgt_SpecificBP"), 
					_ => text + Function.GetJsonString("Type", "dgt_AllBPs"), 
				};
			}
			text = (string.IsNullOrEmpty(Function.ToString(dataRow["ObjCode"])) ? (text + Function.GetJsonString("ObjectCode", string.Empty)) : (text + Function.GetJsonString("ObjectCode", Function.ToString(dataRow["ObjCode"]))));
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["DiscRel"])))
			{
				switch (Function.ToString(dataRow["DiscRel"]))
				{
				case "L":
					text += Function.GetJsonString("DiscountRelations", DiscountGroupRelationsEnum.dgrLowestDiscount.ToString());
					break;
				case "H":
					text += Function.GetJsonString("DiscountRelations", DiscountGroupRelationsEnum.dgrHighestDiscount.ToString());
					break;
				case "A":
					text += Function.GetJsonString("DiscountRelations", DiscountGroupRelationsEnum.dgrAverageDiscount.ToString());
					break;
				case "S":
					text += Function.GetJsonString("DiscountRelations", DiscountGroupRelationsEnum.dgrDiscountTotals.ToString());
					break;
				case "M":
					text += Function.GetJsonString("DiscountRelations", DiscountGroupRelationsEnum.dgrMultipliedDiscount.ToString());
					break;
				}
			}
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["ValidFor"])) && !(Function.ToString(dataRow["ValidFor"]) == "N")) ? (text + Function.GetJsonString("Active", "tYES")) : (text + Function.GetJsonString("Active", "tNO")));
			text += "\"DiscountGroupLineCollection\":[";
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				text += "{";
				DataRow dataRow2 = dataTable.Rows[i];
				text += Function.GetJsonString("AbsEntry", Function.ParseInt(dataRow["AbsEntry"]));
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ObjType"])))
				{
					switch (Function.ParseInt(dataRow2["ObjType"]))
					{
					case 0:
						text += Function.GetJsonString("ObjectType", DiscountGroupBaseObjectEnum.dgboNone.ToString());
						break;
					case 52:
						text += Function.GetJsonString("ObjectType", DiscountGroupBaseObjectEnum.dgboItemGroups.ToString());
						break;
					case 8:
						text += Function.GetJsonString("ObjectType", DiscountGroupBaseObjectEnum.dgboItemProperties.ToString());
						break;
					case 43:
						text += Function.GetJsonString("ObjectType", DiscountGroupBaseObjectEnum.dgboManufacturer.ToString());
						break;
					case 4:
						text += Function.GetJsonString("ObjectType", DiscountGroupBaseObjectEnum.dgboItems.ToString());
						break;
					}
				}
				text = (string.IsNullOrEmpty(Function.ToString(dataRow2["ObjKey"])) ? (text + Function.GetJsonString("ObjectCode", string.Empty)) : (text + Function.GetJsonString("ObjectCode", Function.ToString(dataRow2["ObjKey"]))));
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["DiscType"])))
				{
					string text2 = Function.ToString(dataRow2["DiscType"]);
					if (!(text2 == "D"))
					{
						if (text2 == "P")
						{
							text += Function.GetJsonString("DiscountType", DiscountGroupDiscountTypeEnum.dgdt_Variable.ToString());
						}
					}
					else
					{
						text += Function.GetJsonString("DiscountType", DiscountGroupDiscountTypeEnum.dgdt_Fixed.ToString());
					}
				}
				text += Function.GetJsonString("Discount", Function.ParseDecimal(dataRow2["Discount"]));
				text += Function.GetJsonString("PaidQuantity", Function.ParseDecimal(dataRow2["PayFor"]));
				text += Function.GetJsonString("FreeQuantity", Function.ParseDecimal(dataRow2["ForFree"]));
				text += Function.GetJsonString("MaximumFreeQuantity", Function.ParseDecimal(dataRow2["UpTo"]));
				text += "},";
			}
			text += "],";
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
