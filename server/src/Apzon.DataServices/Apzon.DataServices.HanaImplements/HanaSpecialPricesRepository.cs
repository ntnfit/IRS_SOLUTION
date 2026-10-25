using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Items;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaSpecialPricesRepository : SpecialPricesRepository
{
	public HanaSpecialPricesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string SaveDoc(DataSet docs, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			sessionId = GetSessionId(sessionId);
			if (docs != null && docs.Tables.Count == 4 && docs.Tables[0].IsNotNull())
			{
				string cardCode = Function.ToString(docs.Tables[0].Rows[0]["CardCode"]);
				bool flag = Function.ToString(docs.Tables[0].Rows[0]["IsSpecial"]).Equals("Y");
				DataTable currentData = GetCurrentData(cardCode);
				if (currentData.IsNotNull())
				{
					List<string> lstItemCode = (docs.Tables[1].IsNotNull() ? (from t in docs.Tables[1].AsEnumerable()
						select Function.ToString(t["ItemCode"])).ToList() : new List<string>());
					List<DataRow> list2 = (from t in currentData.AsEnumerable()
						where !lstItemCode.Contains(Function.ToString(t["ItemCode"]))
						select t).ToList();
					if (list2.Any())
					{
						foreach (DataRow item in list2)
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oSpecialPrices,
								OutObjectKey = true,
								Action = Enums.HttpAction.Delete,
								Key = new
								{
									CardCode = cardCode,
									ItemCode = Function.ToString(item["ItemCode"])
								}
							});
						}
					}
				}
				if (docs.Tables[1].IsNotNull())
				{
					if (!flag)
					{
						cardCode = "";
					}
					List<DataRow> lstUpdate = (from t in docs.Tables[1].AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"])) && Function.ToString(t["IsUpdate"]).Equals("Y")
						select t).ToList();
					int i;
					for (i = 0; i < lstUpdate.Count; i++)
					{
						if (currentData.IsNotNull() && currentData.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(Function.ToString(lstUpdate[i]["ItemCode"]))))
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oSpecialPrices,
								OutObjectKey = true,
								Action = Enums.HttpAction.Update,
								BodyObject = MappingJson(docs, lstUpdate[i], flag, EnumCommandType.Update),
								Key = new
								{
									CardCode = cardCode,
									ItemCode = Function.ToString(lstUpdate[i]["ItemCode"])
								}
							});
						}
						else
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oSpecialPrices,
								OutObjectKey = true,
								Action = Enums.HttpAction.AddNew,
								BodyObject = MappingJson(docs, lstUpdate[i], flag, EnumCommandType.Add)
							});
						}
					}
				}
				httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("WATCH", $"Save SpecialPrices Success\t {Function.ToString(httpResult.Content)}");
				}
				else
				{
					Logging.Write("WATCH", $"Save SpecialPrices Error \t {httpResult.Message}");
				}
				return httpResult.Message;
			}
			return "Data not provide";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Delete(string itemCode, string cardCode, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			httpResult = DataService.ProcessJson(ApzObjectTypes.oSpecialPrices, sessionId, "", Enums.HttpAction.Delete, $"(CardCode='{cardCode}',ItemCode='{itemCode}')");
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Remove SpecialPrices \t" + cardCode + ", " + itemCode));
			}
			else
			{
				Logging.Write("WATCH", string.Format("Remove SpecialPrices Error \t" + httpResult.Message));
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string MappingJson(DataSet model, DataRow rowData, bool isSpecialPrice, EnumCommandType type)
	{
		try
		{
			string itemCode = Function.ToString(rowData["ItemCode"]);
			string text = "{";
			if (type == EnumCommandType.Add)
			{
				text += Function.GetJsonString("ItemCode", Function.ToString(rowData["ItemCode"]));
				if (isSpecialPrice)
				{
					text += Function.GetJsonString("CardCode", Function.ToString(rowData["CardCode"]));
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Currency"])))
			{
				text += Function.GetJsonString("Currency", Function.ToString(rowData["Currency"]));
			}
			if (Function.ParseDouble(rowData["Discount"]) > 0.0)
			{
				text += Function.GetJsonString("DiscountPercent", Function.ParseDouble(rowData["Discount"]));
			}
			if (Function.ParseInt(rowData["ListNum"]) > 0)
			{
				text += Function.GetJsonString("PriceListNum", Function.ParseInt(rowData["ListNum"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["AutoUpdt"])))
			{
				text += Function.GetJsonString("AutoUpdate", Function.ToString(rowData["AutoUpdt"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString());
			}
			if (Function.ParseInt(rowData["SrcPrice"]) >= 0)
			{
				switch (Function.ParseInt(rowData["SrcPrice"]))
				{
				case 1:
					text += Function.GetJsonString("SourcePrice", SourceCurrencyEnum.sc_AdditionalCurrency1.ToString());
					if (Function.ParseDouble(rowData["AddPrice1"]) > 0.0)
					{
						text += Function.GetJsonString("Price", Function.ParseDouble(rowData["AddPrice1"]));
					}
					break;
				case 2:
					text += Function.GetJsonString("SourcePrice", SourceCurrencyEnum.sc_AdditionalCurrency2.ToString());
					if (Function.ParseDouble(rowData["AddPrice2"]) > 0.0)
					{
						text += Function.GetJsonString("Price", Function.ParseDouble(rowData["AddPrice2"]));
					}
					break;
				default:
					text += Function.GetJsonString("SourcePrice", SourceCurrencyEnum.sc_PrimaryCurrency.ToString());
					if (Function.ParseDouble(rowData["Price"]) > 0.0)
					{
						text += Function.GetJsonString("Price", Function.ParseDouble(rowData["Price"]));
					}
					break;
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Valid"])))
			{
				text += Function.GetJsonString("Valid", Function.ToString(rowData["Valid"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString());
			}
			if (Function.ParseDateTime(rowData["ValidFrom"]).HasValue)
			{
				text += Function.GetJsonString("ValidFrom", Function.ParseDateTimes(rowData["ValidFrom"]).ToString("yyyy-MM-dd"));
			}
			if (Function.ParseDateTime(rowData["ValidTo"]).HasValue)
			{
				text += Function.GetJsonString("ValidTo", Function.ParseDateTimes(rowData["ValidTo"]).ToString("yyyy-MM-dd"));
			}
			List<string> list = (from DataColumn c in model.Tables[1].Columns
				select c.ColumnName into t
				where t.StartsWith("U_")
				select t).ToList();
			if (list.Any())
			{
				List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
				for (int num = 0; num < list.Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (model.Tables[1].Columns[list[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (model.Tables[1].Columns[list[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = list[num];
					list2.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list2);
			}
			text += "\"SpecialPriceDataAreas\":[";
			if (model.Tables[2].IsNotNull())
			{
				List<DataRow> list3 = (from t in model.Tables[2].AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).ToList();
				if (list3.Any())
				{
					for (int num2 = 0; num2 < list3.Count; num2++)
					{
						int lineNum = Function.ParseInt(list3[num2]["LINENUM"]);
						text += "{";
						text += Function.GetJsonString("RowNumber", lineNum);
						if (!string.IsNullOrEmpty(Function.ToString(list3[num2]["AutoUpdt"])))
						{
							text += Function.GetJsonString("AutoUpdate", Function.ToString(list3[num2]["AutoUpdt"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString());
						}
						if (Function.ParseDouble(list3[num2]["Discount"]) > 0.0)
						{
							text += Function.GetJsonString("Discount", Function.ParseDouble(list3[num2]["Discount"]));
						}
						if (Function.ParseDouble(list3[num2]["Price"]) > 0.0)
						{
							text += Function.GetJsonString("SpecialPrice", Function.ParseDouble(list3[num2]["Price"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(list3[num2]["Currency"])))
						{
							text += Function.GetJsonString("PriceCurrency", Function.ToString(list3[num2]["Currency"]));
						}
						if (Function.ParseDateTime(list3[num2]["FromDate"]).HasValue)
						{
							text += Function.GetJsonString("DateFrom", Function.ParseDateTimes(list3[num2]["FromDate"]).ToString("yyyy-MM-dd"));
						}
						if (Function.ParseDateTime(list3[num2]["ToDate"]).HasValue)
						{
							text += Function.GetJsonString("Dateto", Function.ParseDateTimes(list3[num2]["ToDate"]).ToString("yyyy-MM-dd"));
						}
						if (Function.ParseInt(list3[num2]["ListNum"]) > 0)
						{
							text += Function.GetJsonString("PriceListNo", Function.ParseInt(list3[num2]["ListNum"]));
						}
						List<string> list4 = (from DataColumn c in model.Tables[2].Columns
							select c.ColumnName into t
							where t.StartsWith("U_")
							select t).ToList();
						if (list4.Any())
						{
							List<UserFieldsImpl> list5 = new List<UserFieldsImpl>();
							for (int num3 = 0; num3 < list4.Count; num3++)
							{
								UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
								if (model.Tables[2].Columns[list4[num3]].DataType == typeof(DateTime))
								{
									userFieldsImpl2.Value = Function.ToString(Function.ParseDateTime(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Date;
								}
								else if (model.Tables[2].Columns[list4[num3]].DataType == typeof(double))
								{
									userFieldsImpl2.Value = Function.ToString(Function.ParseDecimal(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl2.Value = Function.ToString(Function.ToString(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl2.Name = list4[num3];
								list5.Add(userFieldsImpl2);
							}
							text += DataService.MappingUdf(list5);
						}
						text += "\"SpecialPriceQuantityAreas\":[";
						if (model.Tables[3].IsNotNull())
						{
							List<DataRow> list6 = (from t in model.Tables[3].AsEnumerable()
								where Function.ToString(t["ItemCode"]).Equals(itemCode) && Function.ParseInt(t["SPP1LNum"]) == lineNum
								select t).ToList();
							if (list6.Any())
							{
								for (int num4 = 0; num4 < list6.Count; num4++)
								{
									text += "{";
									text += Function.GetJsonString("RowNumber", Function.ParseInt(list6[num4]["SPP1LNum"]));
									if (Function.ParseDouble(list6[num4]["Discount"]) > 0.0)
									{
										text += Function.GetJsonString("Discountin", Function.ParseDouble(list6[num4]["Discount"]));
									}
									if (Function.ParseDouble(list6[num4]["Price"]) > 0.0)
									{
										text += Function.GetJsonString("SpecialPrice", Function.ParseDouble(list6[num4]["Price"]));
									}
									if (!string.IsNullOrEmpty(Function.ToString(list6[num4]["Currency"])))
									{
										text += Function.GetJsonString("PriceCurrency", Function.ToString(list6[num4]["Currency"]));
									}
									if (Function.ParseDouble(list6[num4]["Amount"]) > 0.0)
									{
										text += Function.GetJsonString("Quantity", Function.ParseDouble(list6[num4]["Amount"]));
									}
									List<string> list7 = (from DataColumn c in model.Tables[3].Columns
										select c.ColumnName into t
										where t.StartsWith("U_")
										select t).ToList();
									if (list7.Any())
									{
										List<UserFieldsImpl> list8 = new List<UserFieldsImpl>();
										for (int num5 = 0; num5 < list7.Count; num5++)
										{
											UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl();
											if (model.Tables[3].Columns[list7[num5]].DataType == typeof(DateTime))
											{
												userFieldsImpl3.Value = Function.ToString(Function.ParseDateTime(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Date;
											}
											else if (model.Tables[3].Columns[list7[num5]].DataType == typeof(double))
											{
												userFieldsImpl3.Value = Function.ToString(Function.ParseDecimal(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
											}
											else
											{
												userFieldsImpl3.Value = Function.ToString(Function.ToString(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
											}
											userFieldsImpl3.Name = list7[num5];
											list8.Add(userFieldsImpl3);
										}
										text += DataService.MappingUdf(list8);
									}
									text += "},";
								}
							}
						}
						text += "]";
						text += "},";
					}
				}
			}
			text += "]";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override string SaveDataAreas(List<SpecialBP_LineSPP1Impl> docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			string cardCode = docs[0].CardCode;
			string itemCode = docs[0].ItemCode;
			string jsonObject = MappingJsonDataArea(docs);
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oSpecialPrices, sessionId, jsonObject, Enums.HttpAction.Update, $"(CardCode='{cardCode}',ItemCode='{itemCode}')");
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Save SpecialPrices Error \t" + httpResult.Message));
				return httpResult.Message;
			}
			Logging.Write("WATCH", string.Format("Save SpecialPrices \t" + cardCode + ", " + itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	private string MappingJsonDataArea(List<SpecialBP_LineSPP1Impl> docs)
	{
		string text = "";
		try
		{
			if (docs != null && docs.Count > 0)
			{
				text += "{ ";
				text += "\"SpecialPriceDataAreas\": [ ";
				for (int i = 0; i < docs.Count; i++)
				{
					text += "{ ";
					text += Function.GetJsonString("RowNumber", docs[i].LINENUM);
					if (!string.IsNullOrEmpty(docs[i].AutoUpdt))
					{
						text += Function.GetJsonString("AutoUpdate", docs[i].AutoUpdt.StartsWith("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString());
					}
					if (docs[i].Discount > 0.0)
					{
						text += Function.GetJsonString("Discount", docs[i].Discount);
					}
					if (docs[i].Price > 0.0)
					{
						text += Function.GetJsonString("SpecialPrice", docs[i].Price);
					}
					if (!string.IsNullOrEmpty(docs[i].Currency))
					{
						text += Function.GetJsonString("PriceCurrency", docs[i].Currency);
					}
					if (docs[i].FromDate.HasValue && docs[i].FromDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("DateFrom", Convert.ToDateTime(docs[i].FromDate).ToString("yyyy-MM-dd"));
					}
					if (docs[i].ToDate.HasValue && docs[i].ToDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("Dateto", Convert.ToDateTime(docs[i].ToDate).ToString("yyyy-MM-dd"));
					}
					if (docs[i].ListNum > 0)
					{
						text += Function.GetJsonString("PriceListNo", docs[i].ListNum);
					}
					text += DataService.MappingUdf(docs[i].UserFields);
					text += "}, ";
				}
				text += "], ";
				text += "}, ";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override string SaveQuantityAreas(List<SpecialBP_LineSPP2Impl> docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			string cardCode = docs[0].CardCode;
			string itemCode = docs[0].ItemCode;
			string jsonObject = MappingJsonQuantityArea(docs);
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oSpecialPrices, sessionId, jsonObject, Enums.HttpAction.Update, $"(CardCode='{cardCode}',ItemCode='{itemCode}')");
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Save SpecialPrices Error \t" + httpResult.Message));
				return httpResult.Message;
			}
			Logging.Write("WATCH", string.Format("Save SpecialPrices \t" + cardCode + ", " + itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	private string MappingJsonQuantityArea(List<SpecialBP_LineSPP2Impl> docs)
	{
		string text = "";
		try
		{
			if (docs != null && docs.Count > 0)
			{
				text += "{ ";
				text += "\"SpecialPriceDataAreas\": [ ";
				text += "{ ";
				text += Function.GetJsonString("RowNumber", docs[0].LINENUM);
				text += "\"SpecialPriceQuantityAreas\": [ ";
				for (int i = 0; i < docs.Count; i++)
				{
					text += "{ ";
					text += Function.GetJsonString("RowNumber", i);
					if (docs[i].Discount > 0.0)
					{
						text += Function.GetJsonString("Discountin", docs[i].Discount);
					}
					if (docs[i].Price > 0.0)
					{
						text += Function.GetJsonString("SpecialPrice", docs[i].Price);
					}
					if (!string.IsNullOrEmpty(docs[i].Currency))
					{
						text += Function.GetJsonString("PriceCurrency", docs[i].Currency);
					}
					if (docs[i].Amount > 0.0)
					{
						text += Function.GetJsonString("Quantity", docs[i].Amount);
					}
					text += Function.GetJsonString("SPDARowNumber", docs[i].LINENUM);
					text += DataService.MappingUdf(docs[i].UserFields);
					text += "}, ";
				}
				text += "], ";
				text += "}, ";
				text += "], ";
				text += "}, ";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
