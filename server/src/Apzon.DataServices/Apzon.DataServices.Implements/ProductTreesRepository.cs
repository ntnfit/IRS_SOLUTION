using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Production;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ProductTreesRepository : Repository<BillOfMasterials_Impl_OITT>, IProductTrees, IRepository<BillOfMasterials_Impl_OITT>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public ProductTreesRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public virtual string Save(BillOfMasterials_Impl_OITT doc, string sessionId)
	{
		try
		{
			string text = "";
			bool flag = Exists(doc.Code, sessionId);
			if (!flag)
			{
				text = MappingXml(doc);
			}
			else
			{
				text = MappingXml(doc, EnumCommandType.Update);
				IEnumerable<int> enumerable = doc.Line_IIT1.Select((BillOfMasterials_LineImpl_ITT1 i) => i.ChildNum);
				if (enumerable.Any())
				{
					string text2 = string.Join(",", enumerable);
					string sql = "delete from ITT1 where \"Father\"='" + doc.Code + "' and \"ChildNum\" not in (" + text2 + ")";
					text = _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true) + "\n" + text;
				}
			}
			string text3 = (doc.IsUpdatePrice ? MappingXmlItemPrice(doc.Code, doc.PriceList, doc.ItemPrice, doc.Currency, sessionId) : "");
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                            <env:Header>\r\n                            <SessionID>" + sessionId + "</SessionID>\r\n                           </env:Header>\r\n                            <env:Body>\r\n                                " + text3 + "\r\n                                " + text + "\r\n                           </env:Body>\r\n                           </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string arg = Function.ToString(xElement3.Value);
					if (flag)
					{
						Logging.Write("WATCH", $"Update BOM - {arg}");
					}
					else
					{
						Logging.Write("WATCH", $"Add new BOM - {arg}");
					}
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public virtual string MappingXmlService(BillOfMasterials_Impl_OITT doc, EnumCommandType cmd)
	{
		try
		{
			string text = "";
			text = ((cmd != EnumCommandType.Add) ? (text + "<Update xmlns = \"ProductTreesService\">") : (text + "<Add xmlns = \"ProductTreesService\">"));
			text += "<ProductTree>";
			text = text + "<TreeCode>" + Function.RawText(doc.Code) + "</TreeCode>";
			text = text + "<ProductDescription>" + Function.RawText(doc.Name) + "</ProductDescription>";
			switch (doc.TreeType)
			{
			case "A":
				text += "<TreeType>iAssemblyTree</TreeType>";
				break;
			case "S":
				text += "<TreeType>iSalesTree</TreeType>";
				break;
			case "T":
				text += "<TreeType>iTemplateTree</TreeType>";
				break;
			case "I":
				text += "<TreeType>iIngredient</TreeType>";
				break;
			}
			text = text + "<Quantity>" + doc.Qauntity + "</Quantity>";
			text = text + "<DistributionRule>" + Function.RawText(doc.OcrCode) + "</DistributionRule>";
			text = text + "<Project>" + Function.RawText(doc.Project) + "</Project>";
			text = text + "<DistributionRule2>" + Function.RawText(doc.OcrCode2) + "</DistributionRule2>";
			text = text + "<DistributionRule3>" + Function.RawText(doc.OcrCode3) + "</DistributionRule3>";
			text = text + "<DistributionRule4>" + Function.RawText(doc.OcrCode4) + "</DistributionRule4>";
			text = text + "<DistributionRule5>" + Function.RawText(doc.OcrCode5) + "</DistributionRule5>";
			text = text + "<PriceList>" + doc.PriceList + "</PriceList>";
			text = text + "<Warehouse>" + Function.RawText(doc.ToWH) + "</Warehouse>";
			text = ((!(Function.ParseDouble(doc.PlAvgSize) <= 0.0)) ? (text + "<PlanAvgProdSize>" + Function.ParseDouble(doc.PlAvgSize) + "</PlanAvgProdSize>") : (text + "<PlanAvgProdSize>1</PlanAvgProdSize>"));
			text = ((!(doc.HideComp == "Y")) ? (text + "<HideBOMComponentsInPrintout>tNO</HideBOMComponentsInPrintout>") : (text + "<HideBOMComponentsInPrintout>tYES</HideBOMComponentsInPrintout>"));
			text += DataService.MappingUdf(doc.UserFields, cmd);
			text += "<ProductTreeLines>";
			if (doc.Line_IIT1 != null && doc.Line_IIT1.Count > 0)
			{
				for (int i = 0; i < doc.Line_IIT1.Count; i++)
				{
					text += "<ProductTreeLine>";
					BillOfMasterials_LineImpl_ITT1 billOfMasterials_LineImpl_ITT = doc.Line_IIT1[i];
					string text2 = Function.ToString(billOfMasterials_LineImpl_ITT.Type);
					if (text2 == "290")
					{
						text += "<ItemType>pit_Resource</ItemType>";
					}
					else
					{
						text = ((!(text2 == "-18")) ? (text + "<ItemType>pit_Item</ItemType>") : (text + "<ItemType>pit_Text</ItemType>"));
					}
					text = text + "<ParentItem>" + Function.RawText(billOfMasterials_LineImpl_ITT.Father) + "</ParentItem>";
					text = text + "<LineText>" + Function.RawText(billOfMasterials_LineImpl_ITT.LineText) + "</LineText>";
					if (billOfMasterials_LineImpl_ITT.Type != -18)
					{
						text = text + "<ItemCode>" + Function.RawText(billOfMasterials_LineImpl_ITT.Code) + "</ItemCode>";
						if (cmd == EnumCommandType.Update)
						{
							text = text + "<ChildNum>" + billOfMasterials_LineImpl_ITT.ChildNum + "</ChildNum>";
						}
						text = text + "<Quantity>" + billOfMasterials_LineImpl_ITT.Quantity + "</Quantity>";
						text = text + "<Warehouse>" + billOfMasterials_LineImpl_ITT.Warehouse + "</Warehouse>";
						text = text + "<Price>" + billOfMasterials_LineImpl_ITT.Price + "</Price>";
						text = text + "<Currency>" + billOfMasterials_LineImpl_ITT.Currency + "</Currency>";
						text = ((!(billOfMasterials_LineImpl_ITT.IssueMthd == "B")) ? (text + "<IssueMethod>im_Manual</IssueMethod>") : (text + "<IssueMethod>im_Backflush</IssueMethod>"));
						text = text + "<Comment>" + Function.RawText(billOfMasterials_LineImpl_ITT.Comment) + "</Comment>";
						text = text + "<PriceList>" + billOfMasterials_LineImpl_ITT.PriceList + "</PriceList>";
						text = text + "<DistributionRule>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode) + "</DistributionRule>";
						text = text + "<Project>" + Function.RawText(billOfMasterials_LineImpl_ITT.Project) + "</Project>";
						text = text + "<DistributionRule2>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode2) + "</DistributionRule2>";
						text = text + "<DistributionRule3>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode3) + "</DistributionRule3>";
						text = text + "<DistributionRule4>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode4) + "</DistributionRule4>";
						text = text + "<DistributionRule5>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode5) + "</DistributionRule5>";
						text = text + "<WipAccount>" + billOfMasterials_LineImpl_ITT.WipActCode + "</WipAccount>";
						text = text + "<AdditionalQuantity>" + billOfMasterials_LineImpl_ITT.AddQuantit + "</AdditionalQuantity>";
						text += DataService.MappingUdf(billOfMasterials_LineImpl_ITT.UserFields, cmd);
					}
					text += "</ProductTreeLine>";
				}
			}
			text += "</ProductTreeLines>";
			text += "</ProductTree>";
			return (cmd != EnumCommandType.Add) ? (text + "</Update>") : (text + "</Add>");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", Function.GetExceptionAndLineNum(ex));
			return "";
		}
	}

	private string Add(BillOfMasterials_Impl_OITT doc, string sessionId)
	{
		try
		{
			string text = MappingXml(doc);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					UpdateItemPrice(doc, sessionId);
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add new BOM - {arg}");
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public void UpdateItemPrice(BillOfMasterials_Impl_OITT doc, string sessionId)
	{
		try
		{
			string text = _unitOfWork.STDPriceLists.UpdatePriceFromBOM(doc.Code, doc.PriceList, doc.ItemPrice, doc.Currency, sessionId);
			if (!string.IsNullOrEmpty(text))
			{
				Logging.Write("ERROR", text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private string Update(BillOfMasterials_Impl_OITT doc, string sessionId)
	{
		try
		{
			string text = MappingXml(doc);
			string text2 = MappingRemoveXml(doc.Code);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text2 + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"update BOM - {arg}");
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private void ProcessBomLine()
	{
	}

	public virtual bool Exists(string treeCode, string sessionId)
	{
		try
		{
			if (string.IsNullOrEmpty(treeCode))
			{
				return false;
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n\r\n                             <env:Header>\r\n\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n\r\n                             </env:Header>\r\n\r\n                             <env:Body>\r\n\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\r\n                               <Object>oProductTrees</Object> \r\n\r\n                               <TreeCode>" + treeCode + "</TreeCode> \r\n\r\n                              </dis:GetByKey>\r\n\r\n                             </env:Body>\r\n\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("TreeCode"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public string SaveBomVersion(DataSet dtset, string sessionId)
	{
		try
		{
			if (dtset == null || dtset.Tables.Count < 2 || dtset.Tables[0].Rows.Count == 0 || dtset.Tables[1].Rows.Count == 0)
			{
				return "Data can't null!";
			}
			if (!Exists(Function.ToString(dtset.Tables[0].Rows[0]["Code"]), sessionId))
			{
				return AddDataSet(dtset, sessionId);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string MappingXml(BillOfMasterials_Impl_OITT model, EnumCommandType command = EnumCommandType.Add)
	{
		try
		{
			string text = ((command == EnumCommandType.Add) ? "AddObject" : "UpdateObject");
			string text2 = "<dis:" + text + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                          <BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oProductTrees</Object>\r\n                         </AdmInfo>";
			if (command == EnumCommandType.Update)
			{
				text2 = text2 + "<QueryParams><TreeCode>" + Function.RawText(model.Code) + "</TreeCode></QueryParams>";
			}
			text2 += "<ProductTrees>\r\n                            <row>";
			if (!string.IsNullOrEmpty(model.Code))
			{
				text2 = text2 + "<TreeCode>" + Function.RawText(model.Code) + "</TreeCode>";
			}
			if (!string.IsNullOrEmpty(model.TreeType))
			{
				text2 = model.TreeType switch
				{
					"A" => text2 + "<TreeType>iAssemblyTree</TreeType>", 
					"S" => text2 + "<TreeType>iSalesTree</TreeType>", 
					"P" => text2 + "<TreeType>iProductionTree</TreeType>", 
					"T" => text2 + "<TreeType>iTemplateTree</TreeType>", 
					"I" => text2 + "<TreeType>iIngredient</TreeType>", 
					_ => text2 + "<TreeType>iNotATree</TreeType>", 
				};
			}
			if (!model.Qauntity.Equals(0m))
			{
				text2 = text2 + "<Quantity>" + Function.ParseDecimal(model.Qauntity) + "</Quantity>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode))
			{
				text2 = text2 + "<DistributionRule>" + Function.RawText(model.OcrCode) + "</DistributionRule>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode2))
			{
				text2 = text2 + "<DistributionRule2>" + Function.RawText(model.OcrCode2) + "</DistributionRule2>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode3))
			{
				text2 = text2 + "<DistributionRule3>" + Function.RawText(model.OcrCode3) + "</DistributionRule3>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode4))
			{
				text2 = text2 + "<DistributionRule4>" + Function.RawText(model.OcrCode4) + "</DistributionRule4>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode5))
			{
				text2 = text2 + "<DistributionRule5>" + Function.RawText(model.OcrCode5) + "</DistributionRule5>";
			}
			if (!model.PriceList.Equals(0))
			{
				text2 = text2 + "<PriceList>" + Function.ParseInt(model.PriceList) + "</PriceList>";
			}
			if (!string.IsNullOrEmpty(model.ToWH))
			{
				text2 = text2 + "<Warehouse>" + Function.RawText(model.ToWH) + "</Warehouse>";
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text2 = text2 + "<Project>" + Function.RawText(model.Project) + "</Project>";
			}
			text2 = ((!(Function.ParseDouble(model.PlAvgSize) <= 0.0)) ? (text2 + "<PlanAvgProdSize>" + Function.ParseDecimal(model.PlAvgSize) + "</PlanAvgProdSize>") : (text2 + "<PlanAvgProdSize>1</PlanAvgProdSize>"));
			text2 = ((!(model.HideComp == "Y")) ? (text2 + "<HideBOMComponentsInPrintout>tNO</HideBOMComponentsInPrintout>") : (text2 + "<HideBOMComponentsInPrintout>tYES</HideBOMComponentsInPrintout>"));
			text2 += DataService.MappingUdf(model.UserFields, command);
			text2 += "</row>\r\n                        </ProductTrees>";
			if (model.Line_IIT1 != null && model.Line_IIT1.Count > 0)
			{
				text2 += "<ProductTrees_Lines>";
				for (int i = 0; i < model.Line_IIT1.Count; i++)
				{
					text2 += "<row>";
					BillOfMasterials_LineImpl_ITT1 billOfMasterials_LineImpl_ITT = model.Line_IIT1[i];
					if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Father))
					{
						text2 = text2 + "<ParentItem>" + Function.RawText(billOfMasterials_LineImpl_ITT.Father) + "</ParentItem>";
					}
					if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.LineText))
					{
						text2 = text2 + "<LineText>" + Function.RawText(billOfMasterials_LineImpl_ITT.LineText) + "</LineText>";
					}
					string text3 = Function.ToString(billOfMasterials_LineImpl_ITT.Type);
					if (text3 == "290")
					{
						text2 += "<ItemType>pit_Resource</ItemType>";
					}
					else
					{
						text2 = ((!(text3 == "-18")) ? (text2 + "<ItemType>pit_Item</ItemType>") : (text2 + "<ItemType>pit_Text</ItemType>"));
					}
					if (billOfMasterials_LineImpl_ITT.Type != -18)
					{
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Code))
						{
							text2 = text2 + "<ItemCode>" + Function.RawText(billOfMasterials_LineImpl_ITT.Code) + "</ItemCode>";
						}
						if (command == EnumCommandType.Update)
						{
							text2 = text2 + "<ChildNum>" + billOfMasterials_LineImpl_ITT.ChildNum + "</ChildNum>";
						}
						if (!billOfMasterials_LineImpl_ITT.Quantity.Equals(0m))
						{
							text2 = text2 + "<Quantity>" + Function.ParseDecimal(billOfMasterials_LineImpl_ITT.Quantity) + "</Quantity>";
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Warehouse))
						{
							text2 = text2 + "<Warehouse>" + Function.RawText(billOfMasterials_LineImpl_ITT.Warehouse) + "</Warehouse>";
						}
						if (!billOfMasterials_LineImpl_ITT.Price.Equals(0m))
						{
							text2 = text2 + "<Price>" + Function.ParseDecimal(billOfMasterials_LineImpl_ITT.Price) + "</Price>";
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.Currency))
						{
							text2 = text2 + "<Currency>" + Function.RawText(billOfMasterials_LineImpl_ITT.Currency) + "</Currency>";
						}
						if (!string.IsNullOrEmpty(billOfMasterials_LineImpl_ITT.IssueMthd))
						{
							text2 = ((!(billOfMasterials_LineImpl_ITT.IssueMthd == "B")) ? (text2 + "<IssueMethod>im_Manual</IssueMethod>") : (text2 + "<IssueMethod>im_Backflush</IssueMethod>"));
						}
						text2 = text2 + "<Comment>" + Function.RawText(billOfMasterials_LineImpl_ITT.Comment) + "</Comment>";
						if (!billOfMasterials_LineImpl_ITT.PriceList.Equals(0))
						{
							text2 = text2 + "<PriceList>" + Function.ParseInt(billOfMasterials_LineImpl_ITT.PriceList) + "</PriceList>";
						}
						text2 = text2 + "<DistributionRule>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode) + "</DistributionRule>";
						text2 = text2 + "<Project>" + Function.RawText(billOfMasterials_LineImpl_ITT.Project) + "</Project>";
						text2 = text2 + "<DistributionRule2>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode2) + "</DistributionRule2>";
						text2 = text2 + "<DistributionRule3>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode3) + "</DistributionRule3>";
						text2 = text2 + "<DistributionRule4>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode4) + "</DistributionRule4>";
						text2 = text2 + "<DistributionRule5>" + Function.RawText(billOfMasterials_LineImpl_ITT.OcrCode5) + "</DistributionRule5>";
						text2 = text2 + "<WipAccount>" + Function.RawText(billOfMasterials_LineImpl_ITT.WipActCode) + "</WipAccount>";
						if (!billOfMasterials_LineImpl_ITT.AddQuantit.Equals(0m))
						{
							text2 = text2 + "<AdditionalQuantity>" + Function.ParseInt(billOfMasterials_LineImpl_ITT.AddQuantit) + "</AdditionalQuantity>";
						}
						text2 += DataService.MappingUdf(billOfMasterials_LineImpl_ITT.UserFields, command);
					}
					text2 += "</row>";
				}
				text2 += "</ProductTrees_Lines>";
			}
			return text2 + "</BO></BOM>\r\n                         </dis:" + text + ">";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string AddDataSet(DataSet bom, string sessionId)
	{
		try
		{
			string text = MappingDataSet(bom);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add new BOM - {arg}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Save error!";
	}

	public virtual string MappingDataSet(DataSet bom)
	{
		try
		{
			string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oProductTrees</Object>\r\n                          </AdmInfo>";
			text += "<ProductTrees>\r\n                            <row>";
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Code"])))
			{
				text = text + "<TreeCode>" + Function.ToString(bom.Tables[0].Rows[0]["Code"]) + "</TreeCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["TreeType"])))
			{
				text = Function.ToString(bom.Tables[0].Rows[0]["TreeType"]) switch
				{
					"A" => text + "<TreeType>iAssemblyTree</TreeType>", 
					"S" => text + "<TreeType>iSalesTree</TreeType>", 
					"P" => text + "<TreeType>iProductionTree</TreeType>", 
					"T" => text + "<TreeType>iTemplateTree</TreeType>", 
					"I" => text + "<TreeType>iIngredient</TreeType>", 
					_ => text + "<TreeType>iNotATree</TreeType>", 
				};
			}
			if (Function.ParseDouble(bom.Tables[0].Rows[0]["Qauntity"]) > 0.0)
			{
				text = text + "<Quantity>" + Function.ParseDouble(bom.Tables[0].Rows[0]["Qauntity"]) + "</Quantity>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode"])))
			{
				text = text + "<DistributionRule>" + Function.RawText(Function.ToString(bom.Tables[0].Rows[0]["OcrCode"])) + "</DistributionRule>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode2"])))
			{
				text = text + "<DistributionRule2>" + Function.RawText(Function.ToString(bom.Tables[0].Rows[0]["OcrCode2"])) + "</DistributionRule2>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode3"])))
			{
				text = text + "<DistributionRule3>" + Function.RawText(Function.ToString(bom.Tables[0].Rows[0]["OcrCode3"])) + "</DistributionRule3>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode4"])))
			{
				text = text + "<DistributionRule4>" + Function.RawText(Function.ToString(bom.Tables[0].Rows[0]["OcrCode4"])) + "</DistributionRule4>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["OcrCode5"])))
			{
				text = text + "<DistributionRule5>" + Function.RawText(Function.ToString(bom.Tables[0].Rows[0]["OcrCode5"])) + "</DistributionRule5>";
			}
			if (Function.ParseInt(bom.Tables[0].Rows[0]["PriceList"]) > 0)
			{
				text = text + "<PriceList>" + Function.ParseInt(bom.Tables[0].Rows[0]["PriceList"]) + "</PriceList>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["ToWH"])))
			{
				text = text + "<Warehouse>" + Function.ToString(bom.Tables[0].Rows[0]["ToWH"]) + "</Warehouse>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Project"])))
			{
				text = text + "<Project>" + Function.ToString(bom.Tables[0].Rows[0]["Project"]) + "</Project>";
			}
			text += "</row>\r\n                        </ProductTrees>";
			if (bom.Tables[1] != null && bom.Tables[1].Rows.Count > 0)
			{
				text += "<ProductTrees_Lines>";
				for (int i = 0; i < bom.Tables[1].Rows.Count; i++)
				{
					text += "<row>";
					if (Function.ParseInt(bom.Tables[1].Rows[i]["Type"]) != -18)
					{
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Code"])))
						{
							text = text + "<ItemCode>" + Function.RawText(bom.Tables[1].Rows[i]["Code"]) + "</ItemCode>";
						}
						if (Function.ParseDouble(bom.Tables[1].Rows[i]["Quantity"]) > 0.0)
						{
							text = text + "<Quantity>" + Function.ParseDouble(bom.Tables[1].Rows[i]["Quantity"]) + "</Quantity>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Warehouse"])))
						{
							text = text + "<Warehouse>" + Function.ToString(bom.Tables[1].Rows[i]["Warehouse"]) + "</Warehouse>";
						}
						if (Function.ParseDouble(bom.Tables[1].Rows[i]["Price"]) > 0.0)
						{
							text = text + "<Price>" + Function.ParseDouble(bom.Tables[1].Rows[i]["Price"]) + "</Price>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Currency"])))
						{
							text = text + "<Currency>" + Function.ToString(bom.Tables[1].Rows[i]["Currency"]) + "</Currency>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["IssueMthd"])))
						{
							text = ((!(Function.ToString(bom.Tables[1].Rows[i]["IssueMthd"]) == "B")) ? (text + "<IssueMethod>im_Manual</IssueMethod>") : (text + "<IssueMethod>im_Backflush</IssueMethod>"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Comment"])))
						{
							text = text + "<Comment>" + Function.ToString(bom.Tables[1].Rows[i]["Comment"]) + "</Comment>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[0].Rows[0]["Code"])))
						{
							text = text + "<ParentItem>" + Function.ToString(bom.Tables[0].Rows[0]["Code"]) + "</ParentItem>";
						}
						if (Function.ParseInt(bom.Tables[1].Rows[i]["PriceList"]) > 0)
						{
							text = text + "<PriceList>" + Function.ParseInt(bom.Tables[1].Rows[i]["PriceList"]) + "</PriceList>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode"])))
						{
							text = text + "<DistributionRule>" + Function.ToString(bom.Tables[1].Rows[i]["OcrCode"]) + "</DistributionRule>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode2"])))
						{
							text = text + "<DistributionRule2>" + Function.ToString(bom.Tables[1].Rows[i]["OcrCode2"]) + "</DistributionRule2>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode3"])))
						{
							text = text + "<DistributionRule3>" + Function.ToString(bom.Tables[1].Rows[i]["OcrCode3"]) + "</DistributionRule3>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode4"])))
						{
							text = text + "<DistributionRule4>" + Function.ToString(bom.Tables[1].Rows[i]["OcrCode4"]) + "</DistributionRule4>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["OcrCode5"])))
						{
							text = text + "<DistributionRule5>" + Function.ToString(bom.Tables[1].Rows[i]["OcrCode5"]) + "</DistributionRule5>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["WipActCode"])))
						{
							text = text + "<WipAccount>" + Function.ToString(bom.Tables[1].Rows[i]["WipActCode"]) + "</WipAccount>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["Type"])))
						{
							switch (Function.ParseInt(bom.Tables[1].Rows[i]["Type"]))
							{
							case 4:
								text += "<ItemType>pit_Item</ItemType>";
								break;
							case 290:
								text += "<ItemType>pit_Resource</ItemType>";
								break;
							}
						}
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["LineText"])))
						{
							text = text + "<LineText>" + Function.ToString(bom.Tables[1].Rows[i]["LineText"]) + "</LineText>";
						}
						if (Function.ParseDouble(bom.Tables[1].Rows[i]) > 0.0)
						{
							text = text + "<AdditionalQuantity>" + Function.ParseDouble(bom.Tables[1].Rows[i]) + "</AdditionalQuantity>";
						}
					}
					else
					{
						text += "<ItemType>pit_Text</ItemType>";
						if (!string.IsNullOrEmpty(Function.ToString(bom.Tables[1].Rows[i]["LineText"])))
						{
							text = text + "<LineText>" + Function.ToString(bom.Tables[1].Rows[i]["LineText"]) + "</LineText>";
						}
					}
					text += "</row>";
				}
				text += "</ProductTrees_Lines>";
			}
			text += "</BO></BOM>";
			Logging.Write(text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public void Dispose()
	{
	}

	public virtual string DeleteDoc(string code, string sessionId)
	{
		string result = "";
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>oProductTrees</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <TreeCode>" + Function.RawText(code) + "</TreeCode>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:RemoveObject>\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Remove Product tree Error\t{xElement2.Value} ");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Remove Product tree\t{arg}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private static string MappingRemoveXml(string code)
	{
		return "<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>oProductTrees</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <TreeCode>" + Function.RawText(code) + "</TreeCode>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:RemoveObject>";
	}

	private string MappingXmlItemPrice(string itemCode, int priceList, double price, string currency, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>oItems</Object> \r\n                               <ItemCode>" + itemCode + "</ItemCode> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Items_Prices"));
			XElement xElement4 = xElement3.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("row") && t.Descendants().FirstOrDefault((XElement c) => c.Name.LocalName.Equals("PriceList")).Value.ToString() == priceList.ToString());
			if (xElement4 == null)
			{
				xElement3.Add(XElement.Parse($"<row>\r\n                        <PriceList>{priceList}</PriceList>\r\n                        <Price>{price}</Price>\r\n                        <Currency>{currency}</Currency>\r\n                        </row>"));
			}
			else
			{
				xElement4.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Price")).SetValue(price);
				xElement4.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Currency")).SetValue(currency);
			}
			return $"<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                    <BOM>\r\n                                    <BO>\r\n                                    <AdmInfo>\r\n                                    <Object>oItems</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams><ItemCode>{itemCode}</ItemCode></QueryParams>\r\n                                    {xElement3}\r\n                                    </BO>\r\n                                    </BOM>\r\n                                  </dis:UpdateObject>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}
}
