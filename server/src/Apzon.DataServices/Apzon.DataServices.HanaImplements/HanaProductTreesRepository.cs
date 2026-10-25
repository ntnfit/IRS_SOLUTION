using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apzon.DataServices.HanaImplements;

public class HanaProductTreesRepository : ProductTreesRepository
{
	public HanaProductTreesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override bool Exists(string treeCode, string sessionId)
	{
		try
		{
			List<string> tableName = Function.GetTableName(ApzObjectTypes.oProductTrees);
			if (string.IsNullOrEmpty(treeCode))
			{
				return false;
			}
			if (tableName.Count == 0)
			{
				throw new Exception("Can not check exitsts data");
			}
			return Function.ParseInt(ExecuteScarlar("select count(1) from OITT where \"Code\" = @treeCode;", CommandType.Text, new SqlParameter("@treeCode", treeCode))) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public override string Save(BillOfMasterials_Impl_OITT doc, string sessionId)
	{
		try
		{
			string text = "";
			HttpResult httpResult = new HttpResult();
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (doc.IsUpdatePrice)
			{
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oItems,
					OutObjectKey = true,
					Action = Enums.HttpAction.Update,
					BodyObject = MappingJsonItemPrice(doc.Code, doc.PriceList, doc.ItemPrice, doc.Currency, sessionId),
					Key = doc.Code
				});
			}
			bool flag = Exists(doc.Code, sessionId);
			if (!flag)
			{
				text = MappingXmlService(doc, EnumCommandType.Add);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oProductTrees,
					Action = Enums.HttpAction.AddNew,
					BodyObject = text
				});
			}
			else
			{
				text = MappingXmlService(doc, EnumCommandType.Update);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oProductTrees,
					Action = Enums.HttpAction.Update,
					BodyObject = text,
					Key = doc.Code
				});
			}
			httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Execute ERROR: " + doc.Code, httpResult.Message);
				return httpResult.Message;
			}
			if (flag)
			{
				Logging.Write("WATCH", $"Update BOM - {doc.Code}");
			}
			else
			{
				Logging.Write("WATCH", $"Add new BOM - {doc.Code}");
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string AddDataSet(DataSet bom, string sessionId)
	{
		try
		{
			string xmlObject = MappingDataSet(bom);
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oProductTrees, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
			{
				int num = Function.ParseInt(httpResult.Content);
				Logging.Write("WATCH", $"Add new BOM - {num}");
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Save error!";
	}

	public override string MappingXmlService(BillOfMasterials_Impl_OITT doc, EnumCommandType cmd)
	{
		try
		{
			string text = "";
			text += "{";
			text += Function.GetJsonString("TreeCode", Function.ToString(doc.Code));
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text += Function.GetJsonString("ProductDescription", Function.ToString(doc.Name));
			}
			text = doc.TreeType switch
			{
				"A" => text + Function.GetJsonString("TreeType", "iAssemblyTree"), 
				"S" => text + Function.GetJsonString("TreeType", "iSalesTree"), 
				"P" => text + Function.GetJsonString("TreeType", "iProductionTree"), 
				"T" => text + Function.GetJsonString("TreeType", "iTemplateTree"), 
				"I" => text + Function.GetJsonString("TreeType", "iIngredient"), 
				_ => text + Function.GetJsonString("TreeType", "iNotATree"), 
			};
			if (doc.Qauntity != 0m)
			{
				text += Function.GetJsonString("Quantity", doc.Qauntity);
			}
			if (!string.IsNullOrEmpty(doc.OcrCode))
			{
				text += Function.GetJsonString("DistributionRule", Function.ToString(doc.OcrCode));
			}
			if (!string.IsNullOrEmpty(doc.OcrCode2))
			{
				text += Function.GetJsonString("DistributionRule2", Function.ToString(doc.OcrCode2));
			}
			if (!string.IsNullOrEmpty(doc.OcrCode3))
			{
				text += Function.GetJsonString("DistributionRule3", Function.ToString(doc.OcrCode3));
			}
			if (!string.IsNullOrEmpty(doc.OcrCode4))
			{
				text += Function.GetJsonString("DistributionRule4", Function.ToString(doc.OcrCode4));
			}
			if (!string.IsNullOrEmpty(doc.OcrCode5))
			{
				text += Function.GetJsonString("DistributionRule5", Function.ToString(doc.OcrCode5));
			}
			if (doc.PriceList != 0)
			{
				text += Function.GetJsonString("PriceList", doc.PriceList);
			}
			if (!string.IsNullOrEmpty(doc.ToWH))
			{
				text += Function.GetJsonString("Warehouse", Function.ToString(doc.ToWH));
			}
			if (!string.IsNullOrEmpty(doc.Project))
			{
				text += Function.GetJsonString("Project", Function.RawText(doc.Project));
			}
			text = ((!(Function.ParseDouble(doc.PlAvgSize) <= 0.0)) ? (text + Function.GetJsonString("PlanAvgProdSize", Function.ParseDouble(doc.PlAvgSize))) : (text + Function.GetJsonString("PlanAvgProdSize", 1)));
			text = ((!(doc.HideComp == "Y")) ? (text + Function.GetJsonString("HideBOMComponentsInPrintout", "tNO")) : (text + Function.GetJsonString("HideBOMComponentsInPrintout", "tYES")));
			text += DataService.MappingUdf(doc.UserFields, cmd);
			text += "\"ProductTreeLines\":[";
			if (doc.Line_IIT1 != null && doc.Line_IIT1.Count > 0)
			{
				for (int i = 0; i < doc.Line_IIT1.Count; i++)
				{
					text += "{";
					BillOfMasterials_LineImpl_ITT1 billOfMasterials_LineImpl_ITT = doc.Line_IIT1[i];
					text = billOfMasterials_LineImpl_ITT.Type switch
					{
						4 => text + Function.GetJsonString("ItemType", "pit_Item"), 
						290 => text + Function.GetJsonString("ItemType", "pit_Resource"), 
						-18 => text + Function.GetJsonString("ItemType", "pit_Text"), 
						_ => text + Function.GetJsonString("ItemType", "pit_Item"), 
					};
					if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Father))
					{
						text += Function.GetJsonString("ParentItem", Function.ToString(billOfMasterials_LineImpl_ITT.Father));
					}
					if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.LineText))
					{
						text += Function.GetJsonString("LineText", Function.ToString(billOfMasterials_LineImpl_ITT.LineText));
					}
					if (billOfMasterials_LineImpl_ITT.Type != -18)
					{
						text += Function.GetJsonString("ItemCode", Function.ToString(billOfMasterials_LineImpl_ITT.Code));
						text += Function.GetJsonString("ChildNum", (cmd == EnumCommandType.Add) ? i : billOfMasterials_LineImpl_ITT.ChildNum);
						if (billOfMasterials_LineImpl_ITT.Quantity != 0m)
						{
							text += Function.GetJsonString("Quantity", billOfMasterials_LineImpl_ITT.Quantity);
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Warehouse))
						{
							text += Function.GetJsonString("Warehouse", Function.ToString(billOfMasterials_LineImpl_ITT.Warehouse));
						}
						if (billOfMasterials_LineImpl_ITT.Price != 0m)
						{
							text += Function.GetJsonString("Price", billOfMasterials_LineImpl_ITT.Price);
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Currency))
						{
							text += Function.GetJsonString("Currency", Function.ToString(billOfMasterials_LineImpl_ITT.Currency));
						}
						text = ((!(billOfMasterials_LineImpl_ITT.IssueMthd == "B")) ? (text + Function.GetJsonString("IssueMethod", "im_Manual")) : (text + Function.GetJsonString("IssueMethod", "im_Backflush")));
						text += Function.GetJsonString("Comment", Function.ToString(billOfMasterials_LineImpl_ITT.Comment));
						if (billOfMasterials_LineImpl_ITT.PriceList != 0)
						{
							text += Function.GetJsonString("PriceList", billOfMasterials_LineImpl_ITT.PriceList);
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.OcrCode))
						{
							text += Function.GetJsonString("DistributionRule", Function.ToString(billOfMasterials_LineImpl_ITT.OcrCode));
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Project))
						{
							text += Function.GetJsonString("Project", Function.ToString(billOfMasterials_LineImpl_ITT.Project));
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.OcrCode2))
						{
							text += Function.GetJsonString("DistributionRule2", Function.ToString(billOfMasterials_LineImpl_ITT.OcrCode2));
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.OcrCode3))
						{
							text += Function.GetJsonString("DistributionRule3", Function.ToString(billOfMasterials_LineImpl_ITT.OcrCode3));
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.OcrCode4))
						{
							text += Function.GetJsonString("DistributionRule4", Function.ToString(billOfMasterials_LineImpl_ITT.OcrCode4));
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.OcrCode5))
						{
							text += Function.GetJsonString("DistributionRule5", Function.ToString(billOfMasterials_LineImpl_ITT.OcrCode5));
						}
						text = (string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.WipActCode) ? (text + "\"WipAccount\":null,") : (text + Function.GetJsonString("WipAccount", Function.ToString(billOfMasterials_LineImpl_ITT.WipActCode))));
						if (billOfMasterials_LineImpl_ITT.AddQuantit != 0m)
						{
							text += Function.GetJsonString("AdditionalQuantity", billOfMasterials_LineImpl_ITT.AddQuantit);
						}
						text += DataService.MappingUdf(billOfMasterials_LineImpl_ITT.UserFields, cmd);
					}
					text += "},";
				}
			}
			text += "],";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", Function.GetExceptionAndLineNum(ex));
			return "";
		}
	}

	public override string MappingDataSet(DataSet bom)
	{
		try
		{
			string text = "{";
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Code"])))
			{
				text += Function.GetJsonString("TreeCode", Function.ToString(bom.Tables[0].Rows[0]["Code"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["TreeType"])))
			{
				text = Function.ToString(bom.Tables[0].Rows[0]["TreeType"]) switch
				{
					"A" => text + Function.GetJsonString("TreeType", "iAssemblyTree"), 
					"S" => text + Function.GetJsonString("TreeType", "iSalesTree"), 
					"P" => text + Function.GetJsonString("TreeType", "iProductionTree"), 
					"T" => text + Function.GetJsonString("TreeType", "iTemplateTree"), 
					"I" => text + Function.GetJsonString("TreeType", "iIngredient"), 
					_ => text + Function.GetJsonString("TreeType", "iNotATree"), 
				};
			}
			if (Function.ParseDouble(bom.Tables[0].Rows[0]["Qauntity"]) > 0.0)
			{
				text += Function.GetJsonString("Quantity", Function.ParseDouble(bom.Tables[0].Rows[0]["Qauntity"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode"])))
			{
				text += Function.GetJsonString("DistributionRule", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["OcrCode"])));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode2"])))
			{
				text += Function.GetJsonString("DistributionRule2", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["OcrCode2"])));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode3"])))
			{
				text += Function.GetJsonString("DistributionRule3", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["OcrCode3"])));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode4"])))
			{
				text += Function.GetJsonString("DistributionRule4", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["OcrCode4"])));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode5"])))
			{
				text += Function.GetJsonString("DistributionRule5", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["OcrCode5"])));
			}
			if (Function.ParseInt(bom.Tables[0].Rows[0]["PriceList"]) > 0)
			{
				text += Function.GetJsonString("PriceList", Function.ParseInt(bom.Tables[0].Rows[0]["PriceList"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["ToWH"])))
			{
				text += Function.GetJsonString("Warehouse", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["ToWH"])));
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Project"])))
			{
				text += Function.GetJsonString("Project", Function.ToString(Function.ToString(bom.Tables[0].Rows[0]["Project"])));
			}
			if (bom.Tables[1] != null && bom.Tables[1].Rows.Count > 0)
			{
				text += "\"ProductTreeLines\":[";
				for (int i = 0; i < bom.Tables[1].Rows.Count; i++)
				{
					text += "{";
					string text2 = Function.ToString(bom.Tables[1].Rows[i]["Type"]);
					if (text2 == "4")
					{
						text += Function.GetJsonString("ItemType", "pit_Item");
					}
					else
					{
						text = ((!(text2 == "290")) ? (text + Function.GetJsonString("ItemType", "pit_Text")) : (text + Function.GetJsonString("ItemType", "pit_Resource")));
					}
					if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Code"])))
					{
						text += Function.GetJsonString("ParentItem", Function.ToString(bom.Tables[0].Rows[0]["Code"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["LineText"])))
					{
						text += Function.GetJsonString("LineText", Function.ToString(bom.Tables[1].Rows[i]["LineText"]));
					}
					if (Function.ToString(bom.Tables[1].Rows[i]["Type"]) != "-18")
					{
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Code"])))
						{
							text += Function.GetJsonString("ItemCode", Function.ToString(bom.Tables[1].Rows[i]["Code"]));
						}
						if (Function.ParseDouble(bom.Tables[1].Rows[i]["Quantity"]) > 0.0)
						{
							text += Function.GetJsonString("Quantity", Function.ParseDouble(bom.Tables[1].Rows[i]["Quantity"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Warehouse"])))
						{
							text += Function.GetJsonString("Warehouse", Function.ToString(bom.Tables[1].Rows[i]["Warehouse"]));
						}
						if (Function.ParseDouble(bom.Tables[1].Rows[i]["Price"]) > 0.0)
						{
							text += Function.GetJsonString("Price", Function.ParseDouble(bom.Tables[1].Rows[i]["Price"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Currency"])))
						{
							text += Function.GetJsonString("Currency", Function.ToString(bom.Tables[1].Rows[i]["Currency"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["IssueMthd"])))
						{
							text = ((!(Function.ToString(bom.Tables[1].Rows[i]["IssueMthd"]) == "B")) ? (text + Function.GetJsonString("IssueMethod", "im_Manual")) : (text + Function.GetJsonString("IssueMethod", "im_Backflush")));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Comment"])))
						{
							text += Function.GetJsonString("Comment", Function.ToString(bom.Tables[1].Rows[i]["Comment"]));
						}
						if (Function.ParseInt(bom.Tables[1].Rows[i]["PriceList"]) > 0)
						{
							text = text + "\"PriceList\":" + Function.ParseInt(bom.Tables[1].Rows[i]["PriceList"]) + ",";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode"])))
						{
							text += Function.GetJsonString("DistributionRule", Function.ToString(bom.Tables[1].Rows[i]["OcrCode"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode2"])))
						{
							text += Function.GetJsonString("DistributionRule2", Function.ToString(bom.Tables[1].Rows[i]["OcrCode2"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode3"])))
						{
							text += Function.GetJsonString("DistributionRule3", Function.ToString(bom.Tables[1].Rows[i]["OcrCode3"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode4"])))
						{
							text += Function.GetJsonString("DistributionRule4", Function.ToString(bom.Tables[1].Rows[i]["OcrCode4"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode5"])))
						{
							text += Function.GetJsonString("DistributionRule5", Function.ToString(bom.Tables[1].Rows[i]["OcrCode5"]));
						}
						text = (string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["WipActCode"])) ? (text + "\"WipAccount\":null,") : (text + Function.GetJsonString("WipAccount", Function.ToString(bom.Tables[1].Rows[i]["WipActCode"]))));
						if (Function.ParseDouble(bom.Tables[1].Rows[i]) > 0.0)
						{
							text = text + "\"AdditionalQuantity\":" + Function.ParseDouble(bom.Tables[1].Rows[i]) + ",";
						}
					}
					text += "},";
				}
				text += "]";
			}
			text += "}";
			Logging.Write(text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override string DeleteDoc(string code, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oProductTrees,
				OutObjectKey = true,
				Key = code,
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private string MappingJsonItemPrice(string itemCode, int priceList, double price, string currency, string sessionId)
	{
		HttpResult dataObjectFromService = DataService.GetDataObjectFromService(ApzObjectTypes.oItems, sessionId, Enums.HttpAction.Get, itemCode);
		if (!string.IsNullOrEmpty(dataObjectFromService.Message))
		{
			throw new DataException(dataObjectFromService.Message);
		}
		dynamic val = JsonConvert.DeserializeObject(Function.ToString(dataObjectFromService.Content));
		List<JObject> list = new List<JObject>();
		if (val.ItemPrices != null)
		{
			for (int i = 0; i < val.ItemPrices.Count; i++)
			{
				list.Add(val.ItemPrices[i]);
			}
		}
		dynamic val2 = list.FirstOrDefault((JObject t) => Function.ParseInt(t["PriceList"]) == priceList);
		if (val2 == null)
		{
			val2 = new JObject();
			list.Add(val2);
		}
		val2.PriceList = priceList;
		val2.Price = price;
		val2.Currency = currency;
		val.ItemPrices = new JArray(list);
		return "{\"ItemPrices\":" + JsonConvert.SerializeObject(list) + "}";
	}
}
