using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ProductionOrdersRepository : Repository<ProductionOder_Impl_OWOR>, Apzon.DataServices.Interfaces.IProductionOrders, IRepository<ProductionOder_Impl_OWOR>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	private static string _hasAttachment;

	public ProductionOrdersRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	private bool HasAttachment()
	{
		if (string.IsNullOrEmpty(_hasAttachment))
		{
			_hasAttachment = (ServiceUnitOfWork.Commons.ExecuteDataTable("select top 0 * from OWOR;", CommandType.Text).Columns.Contains("AtcEntry") ? "Y" : "N");
		}
		return _hasAttachment == "Y";
	}

	protected virtual HttpResult SaveAttachment(ProductionOder_Impl_OWOR po, string sessionId)
	{
		return ServiceUnitOfWork.Files.SaveAttachmentFiles(po.AttachmentFiles, new Dictionary<string, object> { { "DocEntry,int", po.DocEntry } }, "OWOR");
	}

	public HttpResult Save(ProductionOder_Impl_OWOR po, string sessionId)
	{
		try
		{
			bool mapAttachment = false;
			if (HasAttachment() && po.AttachmentFiles != null && po.AttachmentFiles.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(po, sessionId);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult;
				}
				po.AtcEntry = Function.ParseInt(httpResult.Content);
				mapAttachment = true;
			}
			sessionId = GetSessionId(sessionId);
			if (po.DocEntry <= 0)
			{
				return Add(po, sessionId, mapAttachment);
			}
			return Update(po, sessionId, mapAttachment);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveProPlanning(List<ProductionOder_Impl_OWOR> po, string sessionId)
	{
		HttpResult httpResult = new HttpResult(Enums.MessageCode.Success);
		try
		{
			sessionId = GetSessionId(sessionId);
			if (po.Count > 0)
			{
				for (int i = 0; i < po.Count; i++)
				{
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = po[i];
					if (productionOder_Impl_OWOR.DocEntry <= 0)
					{
						httpResult = Add(productionOder_Impl_OWOR, sessionId);
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							break;
						}
					}
					else
					{
						httpResult = Update(productionOder_Impl_OWOR, sessionId);
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							break;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return httpResult;
	}

	public virtual async Task<HttpResult> SaveList(List<ProductionOder_Impl_OWOR> oDocs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (oDocs != null && oDocs.Count > 0)
			{
				string text = string.Empty;
				for (int i = 0; i < oDocs.Count; i++)
				{
					string text2 = ((oDocs[i].DocEntry <= 0) ? ("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(oDocs[i], EnumCommandType.Add) + "</dis:AddObject>") : ("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(oDocs[i], EnumCommandType.Update) + "</dis:UpdateObject>"));
					text += text2;
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult UpdateStatus(List<ProductionOder_Impl_OWOR> oDocs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (oDocs != null && oDocs.Count > 0)
			{
				string text = string.Empty;
				for (int i = 0; i < oDocs.Count; i++)
				{
					string text2 = "";
					text2 = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXmlStatus(oDocs[i], EnumCommandType.Update) + "</dis:UpdateObject></env:Body></env:Envelope>";
					text += text2;
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
				XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Closed PO Error ", xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							Logging.Write("WATCH", "Closed PO Success ", string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult Add(ProductionOder_Impl_OWOR po, string sessionId, bool mapAttachment = false)
	{
		try
		{
			string xmlObject = MappingXml(po, EnumCommandType.Add, mapAttachment);
			XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oProductionOrders, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new production order - DocEntry - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public virtual HttpResult Update(ProductionOder_Impl_OWOR po, string sessionId, bool mapAttachment = false)
	{
		try
		{
			string xmlObject = MappingXml(po, EnumCommandType.Update, mapAttachment);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oProductionOrders, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Update production order - {num} ");
					ProcessProductionOrderLine(sessionId, num, po.Line_WOR1);
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	private string MappingService(ProductionOder_Impl_OWOR model, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			text += "<ProductionOrder>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + " <AbsoluteEntry>" + Function.ParseInt(model.DocEntry) + "</AbsoluteEntry>";
			}
			if (Function.ParseInt(model.Series) > 0 && commandType == EnumCommandType.Add)
			{
				text = text + "<Series>" + Function.ToString(model.Series) + "</Series>";
			}
			if (commandType == EnumCommandType.Add && model.Series.HasValue)
			{
				_ = model.Series.Value;
				if (_unitOfWork.Common.GetIsManualSerial(model.Series.Value))
				{
					text = text + "<DocumentNumber>" + Function.ToString(model.DocNum) + "</DocumentNumber>";
				}
			}
			if (!string.IsNullOrEmpty(model.ItemCode))
			{
				text = text + "<ItemNo>" + Function.RawText(model.ItemCode) + "</ItemNo>";
			}
			if (!string.IsNullOrEmpty(model.Status))
			{
				text = model.Status switch
				{
					"R" => text + "<ProductionOrderStatus>boposReleased</ProductionOrderStatus>", 
					"L" => text + "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>", 
					"C" => text + "<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>", 
					_ => text + "<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>", 
				};
			}
			if (!string.IsNullOrEmpty(model.Type))
			{
				text = model.Type switch
				{
					"D" => text + "<ProductionOrderType>bopotDisassembly</ProductionOrderType>", 
					"S" => text + "<ProductionOrderType>bopotStandard</ProductionOrderType>", 
					"P" => text + "<ProductionOrderType>bopotSpecial</ProductionOrderType>", 
					_ => text + "<ProductionOrderType>bopotStandard</ProductionOrderType>", 
				};
			}
			if (model.PlannedQty.HasValue)
			{
				text = text + "<PlannedQuantity>" + Function.ParseDecimal(model.PlannedQty) + "</PlannedQuantity>";
			}
			if (model.PostDate.HasValue && model.PostDate > new DateTime(1900, 1, 1))
			{
				text = text + "<PostingDate>" + Convert.ToDateTime(model.PostDate).ToString("yyyyMMdd") + "</PostingDate>";
			}
			if (model.StartDate.HasValue && model.StartDate > new DateTime(1900, 1, 1))
			{
				text = text + "<StartDate>" + Convert.ToDateTime(model.StartDate).ToString("yyyyMMdd") + "</StartDate>";
			}
			if (model.DueDate.HasValue && model.DueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd") + "</DueDate>";
			}
			if (model.OriginAbs > 0)
			{
				text = text + "<ProductionOrderOriginEntry>" + Function.ParseInt(model.OriginAbs) + "</ProductionOrderOriginEntry>";
			}
			if (model.OriginAbs > 0)
			{
				text = model.OriginType switch
				{
					"M" => text + "<ProductionOrderOrigin>bopooManual</ProductionOrderOrigin>", 
					"R" => text + "<ProductionOrderOrigin>bopooMRP</ProductionOrderOrigin>", 
					"S" => text + "<ProductionOrderOrigin>bopooSalesOrder</ProductionOrderOrigin>", 
					_ => text + "<ProductionOrderOrigin>bopooManual</ProductionOrderOrigin>", 
				};
			}
			if (!string.IsNullOrEmpty(model.Comments))
			{
				text = text + "<Remarks>" + Function.ToString(model.Comments) + "</Remarks>";
			}
			if (model.CloseDate.HasValue && model.CloseDate > new DateTime(1900, 1, 1))
			{
				text = text + "<ClosingDate>" + Convert.ToDateTime(model.CloseDate).ToString("yyyyMMdd") + "</ClosingDate>";
			}
			if (!string.IsNullOrEmpty(model.CardCode))
			{
				text = text + "<CustomerCode>" + Function.ToString(model.CardCode) + "</CustomerCode>";
			}
			if (!string.IsNullOrEmpty(model.Warehouse))
			{
				text = text + "<Warehouse>" + Function.ToString(model.Warehouse) + "</Warehouse>";
			}
			if (!string.IsNullOrEmpty(model.JrnlMemo))
			{
				text = text + "<JournalRemarks>" + Function.ToString(model.JrnlMemo) + "</JournalRemarks>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode))
			{
				text = text + "<DistributionRule>" + Function.ToString(model.OcrCode) + "</DistributionRule>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode2))
			{
				text = text + "<DistributionRule2>" + Function.ToString(model.OcrCode2) + "</DistributionRule2>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode3))
			{
				text = text + "<DistributionRule3>" + Function.ToString(model.OcrCode3) + "</DistributionRule3>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode4))
			{
				text = text + "<DistributionRule4>" + Function.ToString(model.OcrCode4) + "</DistributionRule4>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode5))
			{
				text = text + "<DistributionRule5>" + Function.ToString(model.OcrCode5) + "</DistributionRule5>";
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text = text + "<Project>" + Function.ToString(model.Project) + "</Project>";
			}
			text += DataService.MappingUdf(model.UserFields);
			if (model.Line_WOR1 != null && model.Line_WOR1.Count > 0)
			{
				text += "<ProductionOrderLines>";
				for (int i = 0; i < model.Line_WOR1.Count; i++)
				{
					text += "<ProductionOrderLine>";
					ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = model.Line_WOR1[i];
					if (productionOder_LineImpl_WOR.LineNum >= 0)
					{
						text = text + "<LineNumber>" + Function.ParseInt(productionOder_LineImpl_WOR.LineNum) + "</LineNumber>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.ItemCode))
					{
						text = text + "<ItemNo>" + Function.RawText(productionOder_LineImpl_WOR.ItemCode) + "</ItemNo>";
					}
					if (productionOder_LineImpl_WOR.BaseQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.BaseQty)) > 1E-07)
					{
						text = text + "<BaseQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.BaseQty) + "</BaseQuantity>";
					}
					if (productionOder_LineImpl_WOR.PlannedQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.PlannedQty)) > 1E-07)
					{
						text = text + "<PlannedQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.PlannedQty) + "</PlannedQuantity>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.IssueType))
					{
						text = text + "<ProductionOrderIssueType>" + (productionOder_LineImpl_WOR.IssueType.Equals("M") ? "im_Manual" : "im_Backflush") + "</ProductionOrderIssueType>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.wareHouse))
					{
						text = text + "<Warehouse>" + Function.ToString(productionOder_LineImpl_WOR.wareHouse) + "</Warehouse>";
					}
					if (productionOder_LineImpl_WOR.LocCode.HasValue && !productionOder_LineImpl_WOR.LocCode.Equals(0))
					{
						text = text + "<LocationCode>" + Function.ToString(productionOder_LineImpl_WOR.LocCode) + "</LocationCode>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.Project))
					{
						text = text + "<Project>" + Function.ToString(productionOder_LineImpl_WOR.Project) + "</Project>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode))
					{
						text = text + "<DistributionRule>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode) + "</DistributionRule>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode2))
					{
						text = text + "<DistributionRule2>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode2) + "</DistributionRule2>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode3))
					{
						text = text + "<DistributionRule3>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode3) + "</DistributionRule3>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode4))
					{
						text = text + "<DistributionRule4>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode4) + "</DistributionRule4>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode5))
					{
						text = text + "<DistributionRule5>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode5) + "</DistributionRule5>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.WipActCode))
					{
						text = text + "<WipAccount>" + Function.ToString(productionOder_LineImpl_WOR.WipActCode) + "</WipAccount>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.ItemType))
					{
						string itemType = productionOder_LineImpl_WOR.ItemType;
						if (itemType == "4")
						{
							text += "<ItemType>pit_Item</ItemType>";
						}
						else
						{
							text = ((!(itemType == "290")) ? (text + "<ItemType>pit_Text</ItemType>") : (text + "<ItemType>pit_Resource</ItemType>"));
						}
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.LineText))
					{
						text = text + "<LineText>" + Function.ToString(productionOder_LineImpl_WOR.LineText) + "</LineText>";
					}
					if (productionOder_LineImpl_WOR.AdditQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.PlannedQty)) > 1E-07)
					{
						text = text + "<AdditionalQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.AdditQty) + "</AdditionalQuantity>";
					}
					if (productionOder_LineImpl_WOR.StartDate.HasValue && productionOder_LineImpl_WOR.StartDate > new DateTime(1900, 1, 1))
					{
						text = text + "<StartDate>" + Convert.ToDateTime(productionOder_LineImpl_WOR.StartDate).ToString("yyyyMMdd") + "</StartDate>";
					}
					if (productionOder_LineImpl_WOR.EndDate.HasValue && productionOder_LineImpl_WOR.EndDate > new DateTime(1900, 1, 1))
					{
						text = text + "<EndDate>" + Convert.ToDateTime(productionOder_LineImpl_WOR.EndDate).ToString("yyyyMMdd") + "</EndDate>";
					}
					text += DataService.MappingUdf(productionOder_LineImpl_WOR.UserFields);
					text += "</ProductionOrderLine>";
				}
				text += "</ProductionOrderLines>";
			}
			return text + "</ProductionOrder>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public virtual string MappingXml(ProductionOder_Impl_OWOR model, EnumCommandType commandType, bool mapAttachment = false)
	{
		string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><AbsoluteEntry>" + Function.ParseInt(model.DocEntry) + "</AbsoluteEntry></QueryParams>";
			}
			text += "<ProductionOrders>\r\n                             <row>";
			if (Function.ParseInt(model.Series) > 0 && commandType == EnumCommandType.Add)
			{
				text = text + "<Series>" + Function.ToString(model.Series) + "</Series>";
			}
			if (commandType == EnumCommandType.Add && model.Series.HasValue)
			{
				_ = model.Series.Value;
				if (_unitOfWork.Common.GetIsManualSerial(model.Series.Value))
				{
					text = text + "<DocumentNumber>" + Function.ToString(model.DocNum) + "</DocumentNumber>";
				}
			}
			if (!string.IsNullOrEmpty(model.ItemCode))
			{
				text = text + "<ItemNo>" + Function.RawText(model.ItemCode) + "</ItemNo>";
			}
			if (!string.IsNullOrEmpty(model.Status))
			{
				text = model.Status switch
				{
					"R" => text + "<ProductionOrderStatus>boposReleased</ProductionOrderStatus>", 
					"L" => text + "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>", 
					"C" => text + "<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>", 
					_ => text + "<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>", 
				};
			}
			if (!string.IsNullOrEmpty(model.Type))
			{
				text = model.Type switch
				{
					"D" => text + "<ProductionOrderType>bopotDisassembly</ProductionOrderType>", 
					"S" => text + "<ProductionOrderType>bopotStandard</ProductionOrderType>", 
					"P" => text + "<ProductionOrderType>bopotSpecial</ProductionOrderType>", 
					_ => text + "<ProductionOrderType>bopotStandard</ProductionOrderType>", 
				};
			}
			if (model.PlannedQty.HasValue && Math.Abs(Function.ParseDouble(model.PlannedQty)) > 1E-07)
			{
				text = text + "<PlannedQuantity>" + Function.ParseDecimal(model.PlannedQty) + "</PlannedQuantity>";
			}
			if (model.PostDate.HasValue && model.PostDate > new DateTime(1900, 1, 1))
			{
				text = text + "<PostingDate>" + Convert.ToDateTime(model.PostDate).ToString("yyyyMMdd") + "</PostingDate>";
			}
			if (model.StartDate.HasValue && model.StartDate > new DateTime(1900, 1, 1))
			{
				text = text + "<StartDate>" + Convert.ToDateTime(model.StartDate).ToString("yyyyMMdd") + "</StartDate>";
			}
			if (model.DueDate.HasValue && model.DueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd") + "</DueDate>";
			}
			if (model.OriginAbs > 0)
			{
				text = text + "<ProductionOrderOriginEntry>" + Function.ParseInt(model.OriginAbs) + "</ProductionOrderOriginEntry>";
			}
			if (model.OriginAbs > 0)
			{
				text = model.OriginType switch
				{
					"M" => text + "<ProductionOrderOrigin>bopooManual</ProductionOrderOrigin>", 
					"R" => text + "<ProductionOrderOrigin>bopooMRP</ProductionOrderOrigin>", 
					"S" => text + "<ProductionOrderOrigin>bopooSalesOrder</ProductionOrderOrigin>", 
					_ => text + "<ProductionOrderOrigin>bopooManual</ProductionOrderOrigin>", 
				};
			}
			if (HasAttachment())
			{
				text = ((!mapAttachment || model.AtcEntry == 0 || model.AttachmentFiles == null || model.AttachmentFiles.Count <= 0) ? (text + "<AttachmentEntry>null</AttachmentEntry>") : (text + "<AttachmentEntry>" + model.AtcEntry + "</AttachmentEntry>"));
			}
			if (!string.IsNullOrEmpty(model.Comments))
			{
				text = text + "<Remarks>" + Function.ToString(model.Comments) + "</Remarks>";
			}
			if (model.CloseDate.HasValue && model.CloseDate > new DateTime(1900, 1, 1))
			{
				text = text + "<ClosingDate>" + Convert.ToDateTime(model.CloseDate).ToString("yyyyMMdd") + "</ClosingDate>";
			}
			if (!string.IsNullOrEmpty(model.CardCode))
			{
				text = text + "<CustomerCode>" + Function.ToString(model.CardCode) + "</CustomerCode>";
			}
			if (!string.IsNullOrEmpty(model.Warehouse))
			{
				text = text + "<Warehouse>" + Function.ToString(model.Warehouse) + "</Warehouse>";
			}
			if (!string.IsNullOrEmpty(model.JrnlMemo))
			{
				text = text + "<JournalRemarks>" + Function.ToString(model.JrnlMemo) + "</JournalRemarks>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode))
			{
				text = text + "<DistributionRule>" + Function.ToString(model.OcrCode) + "</DistributionRule>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode2))
			{
				text = text + "<DistributionRule2>" + Function.ToString(model.OcrCode2) + "</DistributionRule2>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode3))
			{
				text = text + "<DistributionRule3>" + Function.ToString(model.OcrCode3) + "</DistributionRule3>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode4))
			{
				text = text + "<DistributionRule4>" + Function.ToString(model.OcrCode4) + "</DistributionRule4>";
			}
			if (!string.IsNullOrEmpty(model.OcrCode5))
			{
				text = text + "<DistributionRule5>" + Function.ToString(model.OcrCode5) + "</DistributionRule5>";
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text = text + "<Project>" + Function.ToString(model.Project) + "</Project>";
			}
			text += DataService.MappingUdf(model.UserFields, commandType);
			text += "</row>\r\n                        </ProductionOrders>";
			if (model.Line_WOR1 != null && model.Line_WOR1.Count > 0)
			{
				model.Line_WOR1 = model.Line_WOR1.OrderBy((ProductionOder_LineImpl_WOR1 t) => t.LineNum).ToList();
				text += "<ProductionOrders_Lines>";
				for (int num = 0; num < model.Line_WOR1.Count; num++)
				{
					text += "<row>";
					ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = model.Line_WOR1[num];
					if (productionOder_LineImpl_WOR.LineNum >= 0)
					{
						text = text + "<LineNumber>" + Function.ParseInt(productionOder_LineImpl_WOR.LineNum) + "</LineNumber>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.ItemCode))
					{
						text = text + "<ItemNo>" + Function.RawText(productionOder_LineImpl_WOR.ItemCode) + "</ItemNo>";
					}
					if (productionOder_LineImpl_WOR.BaseQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.BaseQty)) > 1E-07)
					{
						text = text + "<BaseQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.BaseQty) + "</BaseQuantity>";
					}
					if (productionOder_LineImpl_WOR.PlannedQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.PlannedQty)) > 1E-07)
					{
						text = text + "<PlannedQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.PlannedQty) + "</PlannedQuantity>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.IssueType))
					{
						text = text + "<ProductionOrderIssueType>" + (productionOder_LineImpl_WOR.IssueType.Equals("M") ? "im_Manual" : "im_Backflush") + "</ProductionOrderIssueType>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.wareHouse))
					{
						text = text + "<Warehouse>" + Function.ToString(productionOder_LineImpl_WOR.wareHouse) + "</Warehouse>";
					}
					if (productionOder_LineImpl_WOR.LocCode.HasValue && !productionOder_LineImpl_WOR.LocCode.Equals(0))
					{
						text = text + "<LocationCode>" + Function.ToString(productionOder_LineImpl_WOR.LocCode) + "</LocationCode>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.Project))
					{
						text = text + "<Project>" + Function.ToString(productionOder_LineImpl_WOR.Project) + "</Project>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode))
					{
						text = text + "<DistributionRule>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode) + "</DistributionRule>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode2))
					{
						text = text + "<DistributionRule2>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode2) + "</DistributionRule2>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode3))
					{
						text = text + "<DistributionRule3>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode3) + "</DistributionRule3>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode4))
					{
						text = text + "<DistributionRule4>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode4) + "</DistributionRule4>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode5))
					{
						text = text + "<DistributionRule5>" + Function.ToString(productionOder_LineImpl_WOR.OcrCode5) + "</DistributionRule5>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.WipActCode))
					{
						text = text + "<WipAccount>" + Function.ToString(productionOder_LineImpl_WOR.WipActCode) + "</WipAccount>";
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.ItemType))
					{
						string itemType = productionOder_LineImpl_WOR.ItemType;
						if (itemType == "4")
						{
							text += "<ItemType>pit_Item</ItemType>";
						}
						else
						{
							text = ((!(itemType == "290")) ? (text + "<ItemType>pit_Text</ItemType>") : (text + "<ItemType>pit_Resource</ItemType>"));
						}
					}
					if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.LineText))
					{
						text = text + "<LineText>" + Function.ToString(productionOder_LineImpl_WOR.LineText) + "</LineText>";
					}
					if (productionOder_LineImpl_WOR.AdditQty.HasValue && Math.Abs(Function.ParseDouble(productionOder_LineImpl_WOR.PlannedQty)) > 1E-07)
					{
						text = text + "<AdditionalQuantity>" + Function.ParseDecimal(productionOder_LineImpl_WOR.AdditQty) + "</AdditionalQuantity>";
					}
					if (productionOder_LineImpl_WOR.StartDate.HasValue && productionOder_LineImpl_WOR.StartDate > new DateTime(1900, 1, 1))
					{
						text = text + "<StartDate>" + Convert.ToDateTime(productionOder_LineImpl_WOR.StartDate).ToString("yyyyMMdd") + "</StartDate>";
					}
					if (productionOder_LineImpl_WOR.EndDate.HasValue && productionOder_LineImpl_WOR.EndDate > new DateTime(1900, 1, 1))
					{
						text = text + "<EndDate>" + Convert.ToDateTime(productionOder_LineImpl_WOR.EndDate).ToString("yyyyMMdd") + "</EndDate>";
					}
					text += DataService.MappingUdf(productionOder_LineImpl_WOR.UserFields, commandType);
					text += "</row>";
				}
				text += "</ProductionOrders_Lines>";
				if (model.Line_WOR1 != null && model.Line_WOR1.Count > 0)
				{
					text += "<BatchNumbers>";
					for (int num2 = 0; num2 < model.Line_WOR1.Count; num2++)
					{
						if (model.Line_WOR1[num2].BatchNumbers == null || model.Line_WOR1[num2].BatchNumbers.Count <= 0)
						{
							continue;
						}
						for (int num3 = 0; num3 < model.Line_WOR1[num2].BatchNumbers.Count; num3++)
						{
							text += "<row>";
							BatchNumbersImpl batchNumbersImpl = model.Line_WOR1[num2].BatchNumbers[num3];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								text = text + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(batchNumbersImpl.Notes) + "</Notes>";
							}
							if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
							{
								text = text + "<Quantity>" + batchNumbersImpl.Quantity + "</Quantity>";
							}
							if (batchNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(batchNumbersImpl.UserFields, commandType);
							text += "</row>";
						}
					}
					text += "</BatchNumbers>";
				}
				if (model.Line_WOR1 != null && model.Line_WOR1.Count > 0)
				{
					text += "<SerialNumbers>";
					for (int num4 = 0; num4 < model.Line_WOR1.Count; num4++)
					{
						if (model.Line_WOR1[num4].SerialNumbers == null || model.Line_WOR1[num4].SerialNumbers.Count <= 0)
						{
							continue;
						}
						for (int num5 = 0; num5 < model.Line_WOR1[num4].SerialNumbers.Count; num5++)
						{
							text += "<row>";
							SerialNumbersImpl serialNumbersImpl = model.Line_WOR1[num4].SerialNumbers[num5];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text = text + "<BatchID>" + Function.ToString(serialNumbersImpl.BatchID) + "</BatchID>";
							}
							if (serialNumbersImpl.SystemSerialNumber > 0)
							{
								text = text + "<SystemSerialNumber>" + serialNumbersImpl.SystemSerialNumber + "</SystemSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
							}
							if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
							{
								text = text + "<Quantity>" + Function.ParseDecimal(serialNumbersImpl.Quantity) + "</Quantity>";
							}
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(serialNumbersImpl.UserFields, commandType);
							text += "</row>";
						}
					}
					text += "</SerialNumbers>";
				}
			}
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string MappingXmlStatus(ProductionOder_Impl_OWOR model, EnumCommandType type)
	{
		try
		{
			string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>";
			if (type == EnumCommandType.Update)
			{
				text = text + "<QueryParams><AbsoluteEntry>" + Function.ParseInt(model.DocEntry) + "</AbsoluteEntry></QueryParams>";
			}
			text += "<ProductionOrders>\r\n                             <row>";
			if (!string.IsNullOrEmpty(model.Status))
			{
				switch (model.Status)
				{
				case "R":
					text += "<ProductionOrderStatus>boposReleased</ProductionOrderStatus>";
					break;
				case "L":
					text += "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>";
					text = ((!Function.ParseDateTime(model.CloseDate).HasValue || !(Function.ParseDateTime(model.CloseDate) > new DateTime(1900, 1, 1))) ? (text + "<ClosingDate>" + DateTime.Today.ToString("yyyyMMdd") + "</ClosingDate>") : (text + "<ClosingDate>" + Function.ParseDateTime(model.CloseDate).Value.ToString("yyyyMMdd") + "</ClosingDate>"));
					break;
				case "C":
					text += "<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>";
					break;
				default:
					text += "<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>";
					break;
				}
			}
			text += "</row>\r\n                        </ProductionOrders>";
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string UpdateStatusByDI(int docEntry, string status, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			string text = "";
			text = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
			text += "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>";
			text = text + "<QueryParams><AbsoluteEntry>" + docEntry + "</AbsoluteEntry></QueryParams>";
			text += "<ProductionOrders>\r\n                             <row>";
			if (!string.IsNullOrEmpty(status))
			{
				text = status switch
				{
					"R" => text + "<ProductionOrderStatus>boposReleased</ProductionOrderStatus>", 
					"L" => text + "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>", 
					"C" => text + "<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>", 
					"PD" => text + "<U_Pending>Y</U_Pending>", 
					"ND" => text + "<U_Pending>N</U_Pending>", 
					_ => text + "<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>", 
				};
			}
			text += "</row>\r\n                        </ProductionOrders>";
			text += "</BO></BOM>";
			text += "</dis:UpdateObject></env:Body></env:Envelope>";
			empty += text;
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + empty + "</env:Envelopes>";
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
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public async Task<HttpResult> ReceiptForBatch(Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable> model, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			double quantity = 0.0;
			DocumentsImpl item = model.Item2;
			DocumentsImpl item2 = model.Item3;
			List<string> list = new List<string>();
			List<BatchNumbersImpl> list2 = new List<BatchNumbersImpl>();
			if (item != null && item.Lines != null && item.Lines.Count > 0)
			{
				foreach (Document_LinesImpl line in item.Lines)
				{
					foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
					{
						if (!isExistBatch(batchNumber.BatchNumber))
						{
							list2.Add(batchNumber);
							line.Quantity -= batchNumber.Quantity;
						}
					}
					if (list2.Count > 0)
					{
						foreach (BatchNumbersImpl item3 in list2)
						{
							line.BatchNumbers.Remove(item3);
						}
					}
					list.AddRange(line.BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
				}
				if (list.Count > 0)
				{
					quantity = GetBatchCost(string.Join(",", list.ToArray()));
				}
			}
			double totalIss = item.Lines.Sum((Document_LinesImpl m) => m.Quantity);
			getNextGenEntry(item.BPLId.ToString());
			string text = "";
			string text2 = "";
			if (list.Count > 0)
			{
				text = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(item, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
				text2 = MappingStdPriceItem(model.Item1, getDetailItem(model.Item1), quantity);
			}
			empty = empty + "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
			empty += text;
			empty += text2;
			empty = empty + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(item2, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			empty += XmlCloseProductionOrder(model.Item1, null, model.Item6, totalIss);
			empty += "</env:Body>";
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + empty + "</env:Envelopes>";
			Logging.Write("WATCH", "Start receipt PO", model.Item1.ToString());
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("WATCH", "Receipt Batch Error", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				UpdateUdoProPlan(model.Item1, model.Item6);
				List<XElement> list3 = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list3.Count > 0)
				{
					List<string> list4 = new List<string>();
					list4.AddRange(list3.Select((XElement t) => Function.ToString(t.Value)));
					if (list4.Count > 0)
					{
						Logging.Write("WATCH", "Receipt Batch", string.Join(",", list4.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list4.ToArray()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public async Task<HttpResult> IssueBatch(DocumentsImpl docIssue, string sessionId)
	{
		string text = "";
		List<string> list = new List<string>();
		List<BatchNumbersImpl> list2 = new List<BatchNumbersImpl>();
		if (docIssue != null && docIssue.Lines != null && docIssue.Lines.Count > 0)
		{
			foreach (Document_LinesImpl line in docIssue.Lines)
			{
				foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
				{
					if (!isExistBatch(batchNumber.BatchNumber))
					{
						list2.Add(batchNumber);
						line.Quantity -= batchNumber.Quantity;
					}
				}
				if (list2.Count > 0)
				{
					foreach (BatchNumbersImpl item in list2)
					{
						line.BatchNumbers.Remove(item);
					}
				}
				list.AddRange(line.BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
			}
			if (list.Count > 0)
			{
				text += "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                        <env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
				text = text + "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docIssue, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
				text += "</env:Body></env:Envelopes>";
				XmlElement xmlElement = (await DataService.ProcessAsync(text)).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Issue Batch Error: " + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list3 = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list3.Count > 0)
					{
						List<string> list4 = new List<string>();
						list4.AddRange(list3.Select((XElement t) => Function.ToString(t.Value)));
						if (list4.Count > 0)
						{
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list4.ToArray()));
						}
					}
				}
			}
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public async Task<HttpResult> ReceiptBatch(int docentry, DocumentsImpl docIssue, DocumentsImpl docReceipt, string sessionId)
	{
		string text = "";
		List<string> list = new List<string>();
		if (docIssue != null && docIssue.Lines != null && docIssue.Lines.Count > 0)
		{
			foreach (Document_LinesImpl line in docIssue.Lines)
			{
				list.AddRange(line.BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
			}
		}
		string text2 = "";
		if (list.Count > 0)
		{
			double batchCost = GetBatchCost(string.Join(",", list.ToArray()));
			text2 = MappingStdPriceItem(docentry, getDetailItem(docentry), batchCost);
		}
		text += "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
		text = text + "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
		text += text2;
		text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docReceipt, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
		text += "</env:Body></env:Envelopes>";
		XmlElement xmlElement = (await DataService.ProcessAsync(text)).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				Logging.Write("ERROR", "Receipt Batch Error: " + xElement2.Value);
				return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			List<XElement> list2 = (from t in xElement.Descendants()
				where t.Name.LocalName.Equals("RetKey")
				select t).ToList();
			if (list2.Count > 0)
			{
				List<string> list3 = new List<string>();
				list3.AddRange(list2.Select((XElement t) => Function.ToString(t.Value)));
				if (list3.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list3.ToArray()));
				}
			}
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	private string UpdateUdoProPlan(int docentry, DataTable dtReceipt)
	{
		try
		{
			int num = Function.ParseInt(dtReceipt.Rows[0]["DocEntry"]);
			DataRow[] array = dtReceipt.Select("U_DelStatus <> 'N'");
			if (array != null && array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					string text = "";
					string text2 = Function.ToString(dataRow["U_DelStatus"]);
					int num2 = Function.ParseInt(dataRow["LineId"]);
					string text3 = Function.ToString(dataRow["U_BatchNo"]);
					text3 = string.Join(";", text3.Split(';').ToList().Distinct()
						.ToArray());
					if (!string.IsNullOrEmpty(text3) && text3.StartsWith(";"))
					{
						text3 = text3.Substring(1, text3.Length - 1);
					}
					if (!string.IsNullOrEmpty(text3))
					{
						if (text2.Equals("P"))
						{
							int num3 = Function.ParseInt(dataRow["U_ID"]);
							int num4 = Function.ParseInt(dataRow["U_PlanId"]);
							text = string.Format("UPDATE [dbo].[@PWB4] \r\n                                                SET U_BatchNo = ISNULL((NULLIF(CAST (U_BatchNo AS NVARCHAR(MAX)),'') + ';'),'') + '{0}' \r\n                                                WHERE DocEntry = {1} AND U_ID = {2} AND U_PlanId = {3} AND NOT EXISTS\r\n                                                    ( SELECT 1 FROM [dbo].[@PWB4] WHERE DocEntry = {1} AND U_ID = {2} AND U_PlanId = {3} AND U_BatchNo LIKE '%{0}%' )", text3, num, num3, num4);
						}
						else
						{
							text = string.Format("UPDATE [dbo].[@PWB4] \r\n                                                SET U_BatchNo = ISNULL((NULLIF(CAST (U_BatchNo AS NVARCHAR(MAX)),'') + ';'),'') + '{0}' \r\n                                                WHERE DocEntry = {1} AND LineId = {2} AND NOT EXISTS\r\n                                                    ( SELECT 1 FROM [dbo].[@PWB4] WHERE DocEntry = {1} AND LineId = {2} AND U_BatchNo LIKE '%{0}%' )", text3, num, num2);
						}
						if (ExecuteNonQuery(text, CommandType.Text) <= 0)
						{
							Logging.Write("ERROR", "Update batch receipt Error: " + text);
						}
					}
				}
			}
			DataRow[] array3 = dtReceipt.Select("U_DelStatus = 'P'");
			if (array3 != null && array3.Length != 0)
			{
				string text4 = $"UPDATE [dbo].[@PWB2]\r\n                                            SET U_BatchNo = \r\n                                                (SELECT U_BatchNo\r\n                                                    FROM [dbo].[@PWB4] a\r\n                                                    WHERE a.U_PlanId = [dbo].[@PWB2].U_FatherId\r\n                                                        AND a.DocEntry = [dbo].[@PWB2].DocEntry\r\n                                                        AND a.U_ID = [dbo].[@PWB2].U_ID\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\tAND a.U_Width = [dbo].[@PWB2].U_Width\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\tAND a.U_Length = [dbo].[@PWB2].U_Length\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\tAND a.U_QtyUoM = [dbo].[@PWB2].U_QtyUoM\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\tAND a.U_UnitUoM = [dbo].[@PWB2].U_UnitUoM)\r\n                                            WHERE [dbo].[@PWB2].U_DelStatus = 'P' AND [dbo].[@PWB2].DocEntry = {num} ";
				text4 += $"UPDATE OWOR SET U_IssBatchP =\r\n                                                (SELECT U_BatchNo FROM [@pwb2] a \r\n\t\t\t\t\t                            WHERE U_PwNo = OWOR.DocEntry) \r\n\t                                        WHERE OWOR.DocEntry IN (SELECT U_PwNo\r\n\t\t\t\t\t\t\t                                        FROM [@pwb2] x \r\n\t\t\t\t\t\t\t                                        WHERE x.DocEntry = {num} AND U_DelStatus = 'P') ";
				if (ExecuteNonQuery(text4, CommandType.Text) <= 0)
				{
					Logging.Write("ERROR", "Update batch receipt Error: " + text4);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public async Task<HttpResult> AddReceiptForPO(int docEntry, DocumentsImpl goodsReceipt, DataTable dtReceipt, string sessionId)
	{
		try
		{
			string query = "SELECT U_ReBatch FROM dbo.OWOR WHERE DocEntry = " + docEntry;
			string text = ExecuteScarlar(query, CommandType.Text);
			_ = string.Empty;
			string text2 = "";
			text2 += "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
			text2 = text2 + "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
			text2 += "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
			text2 += "<BOM>\r\n                            <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                            <AdmInfo>\r\n                            <Object>oProductionOrders</Object>\r\n                            </AdmInfo>";
			text2 = text2 + "<QueryParams><AbsoluteEntry>" + docEntry + "</AbsoluteEntry></QueryParams>";
			text2 += "<ProductionOrders>\r\n                             <row>";
			if (dtReceipt.IsNotNull())
			{
				bool flag = true;
				for (int i = 0; i < dtReceipt.Rows.Count; i++)
				{
					int num = Function.ParseInt(dtReceipt.Rows[i]["U_QtyUoM"]);
					int num2 = Function.ParseInt(dtReceipt.Rows[i]["Confirmed"]);
					int num3 = Function.ParseInt(dtReceipt.Rows[i]["ConfirmReport"]);
					flag = ("True".Equals(Function.ToString(dtReceipt.Rows[i]["Checked"])) && num2 + num3 >= num) & flag;
				}
				if (flag)
				{
					text2 += "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>";
				}
			}
			if (goodsReceipt != null && goodsReceipt.Lines.Count > 0 && goodsReceipt.Lines[0].BatchNumbers.Count > 0)
			{
				foreach (BatchNumbersImpl batchNumber in goodsReceipt.Lines[0].BatchNumbers)
				{
					text = text + ";" + batchNumber.BatchNumber;
				}
				if (!string.IsNullOrEmpty(text) && text.StartsWith(";"))
				{
					text = text.Substring(1, text.Length - 1);
				}
				text2 = text2 + "<U_ReBatch>" + text + "</U_ReBatch>";
			}
			text2 += "</row>\r\n                        </ProductionOrders>";
			text2 += "</BO></BOM>";
			text2 += "</dis:UpdateObject>";
			text2 += "</env:Body></env:Envelopes>";
			XmlElement xmlElement = (await DataService.ProcessAsync(text2)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Close PO: " + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public double GetBatchCost(string batch)
	{
		try
		{
			string query = "apz_productionforbatch_cost";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(batch))
			{
				list.Add(new SqlParameter("@batch", batch));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ParseDouble(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw ex;
		}
		return -1.0;
	}

	private string GetResourceByPo(int docEntry)
	{
		try
		{
			string query = "Apz_ProductionConfirm_GetResourceByPo";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docentry", docEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw ex;
		}
		return "";
	}

	private string getDetailItem(int docEntry)
	{
		string query = $"SELECT Top 1 ItemCode FROM WOR1 WHERE DocEntry = {docEntry} AND ItemType = 4 ORDER BY LineNum ASC ";
		return ExecuteScarlar(query, CommandType.Text, (SqlParameter[])null);
	}

	private string MappingStdPriceItem(int docentry, string itemCode, double quantity)
	{
		string text = "";
		try
		{
			text += "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
			text += "<BOM><BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oItems</Object>\r\n                              </AdmInfo>";
			text += $"<QueryParams><ItemCode>{itemCode}</ItemCode></QueryParams>";
			text += "<Items>";
			text += "<row>";
			text += $"<AvgStdPrice>{Function.ToString(quantity)}</AvgStdPrice>";
			text += "</row>";
			text += "</Items>";
			text += "</BO></BOM>";
			return text + "</dis:UpdateObject>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private string MappingSpecialItem(int docentry, string item, double quantity)
	{
		string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>";
		try
		{
			text = text + "<QueryParams><AbsoluteEntry>" + Function.ParseInt(docentry) + "</AbsoluteEntry></QueryParams>";
			text += "<ProductionOrders_Lines>";
			text += "<row>";
			text = text + "<ItemNo>" + Function.RawText(item) + "</ItemNo>";
			text = text + "<PlannedQuantity>" + quantity + "</PlannedQuantity>";
			text += "<ItemType>pit_Item</ItemType>";
			text += "</row>";
			text += "</ProductionOrders_Lines>";
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string XmlCloseProductionOrder(int docEntry, DocumentsImpl goodsReceipt, DataTable dtReceipt, double totalIss, string remark = "")
	{
		try
		{
			string query = "SELECT U_ReBatch, U_IssuedQty, PlannedQty, CmpltQty FROM dbo.OWOR WHERE DocEntry = " + docEntry;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (!dataTable.IsNotNull())
			{
				throw new Exception("Cannot get PO infomation!");
			}
			string text = Function.ToString(dataTable.Rows[0]["U_ReBatch"]);
			totalIss += Function.ParseDouble(dataTable.Rows[0]["U_IssuedQty"]);
			double num = Function.ParseDouble(dataTable.Rows[0]["PlannedQty"]);
			double num2 = Function.ParseDouble(dataTable.Rows[0]["CmpltQty"]);
			_ = string.Empty;
			string text2 = "";
			text2 += "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
			text2 += "<BOM>\r\n                            <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                            <AdmInfo>\r\n                            <Object>oProductionOrders</Object>\r\n                            </AdmInfo>";
			text2 = text2 + "<QueryParams><AbsoluteEntry>" + docEntry + "</AbsoluteEntry></QueryParams>";
			text2 += "<ProductionOrders>\r\n                             <row>";
			if (goodsReceipt != null && goodsReceipt.Lines.Count > 0 && goodsReceipt.Lines[0].BatchNumbers.Count > 0)
			{
				foreach (Document_LinesImpl line in goodsReceipt.Lines)
				{
					foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
					{
						text = text + ";" + batchNumber.BatchNumber;
					}
					num2 += line.Quantity;
				}
			}
			if (!string.IsNullOrEmpty(text) && text.StartsWith(";"))
			{
				text = text.Substring(1, text.Length - 1);
			}
			if (num - num2 < 3.0 && num - num2 > -3.0)
			{
				text2 += "<ProductionOrderStatus>boposClosed</ProductionOrderStatus>";
				text2 = text2 + "<Remarks>" + remark + "</Remarks>";
			}
			text2 = text2 + "<U_ReBatch>" + text + "</U_ReBatch>";
			text2 = text2 + "<U_IssuedQty>" + totalIss + "</U_IssuedQty>";
			text2 += "</row>\r\n                        </ProductionOrders>";
			text2 += "</BO></BOM>";
			return text2 + "</dis:UpdateObject>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	private string getNextGenEntry(string branch)
	{
		SqlParameter sqlParameter = new SqlParameter("@BPLId", branch);
		DataTable dataTable = ExecuteDataTable("SELECT NextNumber FROM dbo.NNM1 WHERE ObjectCode='59' AND (BPLId is NULL OR( BPLId is not null and BPLId=@BPLId))", CommandType.Text, sqlParameter);
		if (dataTable.IsNotNull())
		{
			return Function.ParseInt(dataTable.Rows[0]["NextNumber"]).ToString();
		}
		return "";
	}

	public DataTable GetLineByFather(string father)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Father", father);
			return ExecuteDataTable("SELECT [Father]\r\n                                      ,[ChildNum]\r\n                                      ,a.Code\r\n                                      ,[Quantity]\r\n                                      ,[Warehouse]\r\n                                      ,[Price]\r\n                                      ,[Currency]\r\n                                      ,a.PriceList\r\n                                      ,[OrigPrice]\r\n                                      ,[OrigCurr]\r\n                                      ,[IssueMthd]\r\n                                      ,[Uom]\r\n                                      ,[Comment]\r\n                                      ,[LogInstanc]\r\n                                      ,a.Object\r\n                                      ,a.OcrCode\r\n                                      ,a.OcrCode2\r\n                                      ,a.OcrCode3\r\n                                      ,a.OcrCode4\r\n                                      ,a.OcrCode5\r\n                                      ,[PrncpInput]\r\n                                      ,a.Project\r\n                                      ,[Type]\r\n                                      ,[WipActCode]\r\n                                      ,[AddQuantit]\r\n                                      ,[LineText]\r\n\t                                  ,(ISNULL(a.Quantity,0)/ISNULL(NULLIF(b.Qauntity,0),1)) AS RateQty\r\n                                  FROM [ITT1] a LEFT JOIN OITT b ON a.Father=b.Code WHERE Father=@Father", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	private bool isExistBatch(string batchNo)
	{
		try
		{
			string query = $"SELECT DistNumber FROM APZ_OITW_LOCATION WHERE DistNumber = N'{batchNo}'";
			return !string.IsNullOrEmpty(ExecuteScarlar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public bool IsExistsPO(int docEntry)
	{
		try
		{
			if (docEntry > 0)
			{
				string query = $"SELECT 1 FROM OWOR WHERE DocEntry = {docEntry}";
				return !string.IsNullOrEmpty(ExecuteScarlar(query, CommandType.Text));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, $"SELECT 1 FROM OWOR WHERE DocEntry = {docEntry}");
		}
		return false;
	}

	private double GetStockBatch(string batchNo, string whs)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(batchNo))
			{
				list.Add(new SqlParameter("@BatchNo", batchNo));
			}
			if (!string.IsNullOrEmpty(whs))
			{
				list.Add(new SqlParameter("@WhsCode", whs));
			}
			return Function.ParseDouble(ExecuteScarlar("Apz_ProductionConfirm_GetStockBatch", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0.0;
	}

	private string XmlUpdateRSCostThanhPham(string resource, double cost)
	{
		try
		{
			string sql = $"UPDATE ORSC\r\n                                                SET StdCost1={cost}, StdCost2=a.CostVal2, StdCost3=a.CostVal3, StdCost4=a.CostVal4, StdCost5=a.CostVal5, StdCost6=a.CostVal6, StdCost7=a.CostVal7,\r\n                                                    StdCost8=a.CostVal8, StdCost9=a.CostVal9, StdCost10=a.CostVal10\r\n                                                FROM ORSB a\r\n                                                WHERE a.ResGrpCod = [ORSC].ResGrpCod\r\n                                                    AND [ORSC].ResCode = '{resource}'";
			return _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string XmlUpdateRSCostThuHoi(string resource, double cost)
	{
		try
		{
			string sql = string.Format(XmlUpdateRSCostThuHoiQuery(), cost, resource);
			return _unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw ex;
		}
	}

	protected virtual string XmlUpdateRSCostThuHoiQuery()
	{
		return "UPDATE ORSC\r\n                                                SET StdCost1={0}, StdCost2=0, StdCost3=0, StdCost4=0, StdCost5=0, StdCost6=0, StdCost7=0, StdCost8=0, StdCost9=0, StdCost10=0\r\n                                                FROM ORSB a\r\n                                                WHERE a.ResGrpCod = [ORSC].ResGrpCod\r\n                                                    AND [ORSC].ResCode = '{1}'";
	}

	public async Task<HttpResult> ConfirmProduction(Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable, string> model, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			double num = 0.0;
			DocumentsImpl item = model.Item2;
			DocumentsImpl item2 = model.Item3;
			string item3 = model.Item5;
			string text = GetResourceByPo(model.Item1);
			if (string.IsNullOrEmpty(text))
			{
				text = model.Item4;
			}
			List<string> lstBatch = new List<string>();
			List<BatchNumbersImpl> list = new List<BatchNumbersImpl>();
			if (item != null && item.Lines != null && item.Lines.Count > 0)
			{
				foreach (Document_LinesImpl line in item.Lines)
				{
					foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
					{
						double stockBatch = GetStockBatch(batchNumber.BatchNumber, line.WhsCode);
						if (stockBatch <= 0.0)
						{
							list.Add(batchNumber);
							line.Quantity -= batchNumber.Quantity;
						}
						else if (stockBatch != batchNumber.Quantity)
						{
							double num2 = stockBatch - batchNumber.Quantity;
							batchNumber.Quantity = stockBatch;
							line.Quantity += num2;
						}
					}
					if (list.Count > 0)
					{
						foreach (BatchNumbersImpl item4 in list)
						{
							line.BatchNumbers.Remove(item4);
						}
					}
					lstBatch.AddRange(line.BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
				}
				if (!string.IsNullOrEmpty(item3))
				{
					item3 = string.Join(",", item3.Split(';'));
					num = GetBatchCost(item3);
					Logging.Write("WATCH", "Batch cost = " + num, item3);
					if (num < 0.0)
					{
						return new HttpResult(Enums.MessageCode.Error, "Batch cost error!");
					}
				}
			}
			double totalIss = item.Lines.Sum((Document_LinesImpl m) => m.Quantity);
			string text2 = "";
			if (lstBatch.Count > 0)
			{
				text2 = $"<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">{_unitOfWork.Documents.Mapping(item, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit)}</dis:AddObject>";
			}
			if (item2.Lines.Count > 0 && item2.Lines[0].BatchNumbers.Count > 0)
			{
				text2 += XmlUpdateRSCostThuHoi(text, num);
				text2 = text2 + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(item2, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			}
			empty = empty + "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">";
			empty += text2;
			empty += XmlCloseProductionOrder(model.Item1, item2, model.Item6, totalIss, model.Item7);
			empty += "</env:Body>";
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + empty + "</env:Envelopes>";
			Logging.Write("WATCH", "Start receipt PO", model.Item1.ToString());
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Receipt Batch Error", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				UpdateUdoProPlan(model.Item1, model.Item6);
				List<XElement> list2 = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list2.Count > 0)
				{
					List<string> list3 = new List<string>();
					list3.AddRange(list2.Select((XElement t) => Function.ToString(t.Value)));
					if (list3.Count > 0)
					{
						Logging.Write("WATCH", "Receipt Batch Success", string.Join(",", list3.ToArray()));
						if (lstBatch.Count > 0)
						{
							list3.RemoveAt(0);
						}
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list3.ToArray()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	private bool isExistTransfer(string batchNo)
	{
		try
		{
			string query = $"SELECT * FROM dbo.WTQ1 a INNER JOIN dbo.OWTQ b ON b.DocEntry = a.DocEntry WHERE U_DistNumber = N'{batchNo}'";
			return !string.IsNullOrEmpty(ExecuteScarlar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public virtual async Task<HttpResult> PackingBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId)
	{
		try
		{
			if (model != null)
			{
				DocumentsImpl docIssue = model.Item1;
				DocumentsImpl docReceipt = model.Item2;
				ProductionOder_Impl_OWOR production = model.Item3;
				if (docIssue != null && docReceipt != null && production != null)
				{
					List<Document_LinesImpl> lines = docIssue.Lines;
					List<string> batch = new List<string>();
					for (int i = 0; i < lines.Count; i++)
					{
						foreach (BatchNumbersImpl batchNumber in lines[i].BatchNumbers)
						{
							string query = "Apz_PackingBatch_Check";
							List<SqlParameter> list = new List<SqlParameter>();
							list.Add(new SqlParameter("@BatchNo", batchNumber.BatchNumber));
							string text = Function.ToString(ExecuteScarlar(query, CommandType.StoredProcedure, list.ToArray()));
							if (!string.IsNullOrEmpty(text))
							{
								return new HttpResult(Enums.MessageCode.Error, text);
							}
						}
						batch.AddRange(lines[i].BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
					}
					int docentryPro = 0;
					Logging.Write("WATCH", "Start Packing Batch...");
					string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                            <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(production, EnumCommandType.Add) + "</dis:AddObject></env:Body></env:Envelopes>";
					XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
					XElement xElement = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement2 != null)
						{
							Logging.Write("ERROR", "Packing Batch Error", xElement2.Value);
							return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
						}
					}
					else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
					{
						List<XElement> list2 = (from t in xElement.Descendants()
							where t.Name.LocalName.Equals("RetKey")
							select t).ToList();
						if (list2.Count > 0)
						{
							List<string> list3 = new List<string>();
							list3.AddRange(list2.Select((XElement t) => Function.ToString(t.Value)));
							if (list3.Count > 0)
							{
								docentryPro = Function.ParseInt(list3[0]);
							}
						}
					}
					if (docentryPro <= 0)
					{
						Logging.Write("ERROR", "Packing Batch Error", "Could not add PO " + docentryPro);
						return new HttpResult(Enums.MessageCode.Error, "Could not add new production order!");
					}
					double batchCost = GetBatchCost(string.Join(",", batch.ToArray()));
					docIssue.Ref2 = docentryPro.ToString();
					docIssue.Comments = "Packing batch!";
					docReceipt.Ref2 = docentryPro.ToString();
					docIssue.Comments = "Packing batch!";
					if (docReceipt.Lines != null && docReceipt.Lines.Count > 0)
					{
						for (int num = 0; num < docReceipt.Lines.Count; num++)
						{
							docReceipt.Lines[num].BaseEntry = docentryPro;
							docReceipt.Lines[num].BaseRef = docentryPro;
							UserFieldsImpl userFieldsImpl = docReceipt.Lines[num].BatchNumbers[0].UserFields.FirstOrDefault((UserFieldsImpl u) => u.Name.Equals("U_PwNo") || u.Name.Equals("PwNo"));
							if (userFieldsImpl != null)
							{
								userFieldsImpl.Value = docentryPro;
							}
						}
					}
					ProductionOder_Impl_OWOR model2 = new ProductionOder_Impl_OWOR
					{
						DocEntry = docentryPro,
						Status = "R"
					};
					ProductionOder_Impl_OWOR model3 = new ProductionOder_Impl_OWOR
					{
						DocEntry = docentryPro,
						Status = "L"
					};
					string text2 = "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXmlStatus(model2, EnumCommandType.Update) + "</dis:UpdateObject>";
					text2 = text2 + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docIssue, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
					text2 += XmlUpdateRSCostThuHoi(production.Line_WOR1[0].ItemCode, batchCost);
					text2 = text2 + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docReceipt, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
					text2 += $"<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">{MappingXmlStatus(model3, EnumCommandType.Update)}</dis:UpdateObject>";
					text2 += "</env:Body>";
					string xmlObject2 = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text2 + "</env:Envelopes>";
					XmlElement xmlElement2 = DataService.Process(xmlObject2).Content as XmlElement;
					XElement xElement3 = XElement.Parse(xmlElement2.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement2.OuterXml) && xmlElement2.OuterXml.Contains("Fault"))
					{
						XElement xElement4 = xElement3.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement4 != null)
						{
							Logging.Write("ERROR", "Packing Batch Error on Issue Receipt", xElement4.Value);
							string text3 = UpdateStatusByDI(docentryPro, "C", sessionId);
							Logging.Write("WATCH", $"Result Cancel Production Order {docentryPro}", text3);
							return new HttpResult(Enums.MessageCode.Error, xElement4.Value);
						}
					}
					else if (!string.IsNullOrEmpty(xmlElement2.OuterXml) && xmlElement2.OuterXml.Contains("RetKey"))
					{
						List<XElement> list4 = (from t in xElement3.Descendants()
							where t.Name.LocalName.Equals("RetKey")
							select t).ToList();
						if (list4.Count > 0)
						{
							List<string> list5 = new List<string>();
							list5.AddRange(list4.Select((XElement t) => Function.ToString(t.Value)));
							if (list5.Count > 0)
							{
								Logging.Write("WATCH", "Packing Batch Success", string.Join(",", list5.ToArray()));
								return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list5.ToArray()));
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual async Task<HttpResult> ConfirmBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId)
	{
		try
		{
			if (model != null)
			{
				DocumentsImpl docIssue = model.Item1;
				DocumentsImpl docReceipt = model.Item2;
				ProductionOder_Impl_OWOR production = model.Item3;
				if (docIssue != null && docReceipt != null && production != null)
				{
					List<Document_LinesImpl> lines = docIssue.Lines;
					List<string> batch = new List<string>();
					for (int i = 0; i < lines.Count; i++)
					{
						batch.AddRange(lines[i].BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
					}
					int docentryPro = 0;
					Logging.Write("WATCH", "Start Confirm Batch...");
					string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                            <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(production, EnumCommandType.Add) + "</dis:AddObject></env:Body></env:Envelopes>";
					XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
					XElement xElement = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement2 != null)
						{
							Logging.Write("ERROR", "Packing Batch Error", xElement2.Value);
							return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
						}
					}
					else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
					{
						List<XElement> list = (from t in xElement.Descendants()
							where t.Name.LocalName.Equals("RetKey")
							select t).ToList();
						if (list.Count > 0)
						{
							List<string> list2 = new List<string>();
							list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
							if (list2.Count > 0)
							{
								docentryPro = Function.ParseInt(list2[0]);
							}
						}
					}
					if (docentryPro <= 0)
					{
						Logging.Write("ERROR", "Confirm Batch Error", "Could not add PO " + docentryPro);
						return new HttpResult(Enums.MessageCode.Error, "Could not add new production order!");
					}
					double batchCost = GetBatchCost(string.Join(",", batch.ToArray()));
					docIssue.Ref2 = docentryPro.ToString();
					docIssue.Comments = "Confirm batch!";
					docReceipt.Ref2 = docentryPro.ToString();
					docIssue.Comments = "Confirm batch!";
					if (docReceipt.Lines != null && docReceipt.Lines.Count > 0)
					{
						foreach (Document_LinesImpl line in docReceipt.Lines)
						{
							line.BaseEntry = docentryPro;
							line.BaseRef = docentryPro;
							foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
							{
								UserFieldsImpl userFieldsImpl = batchNumber.UserFields.FirstOrDefault((UserFieldsImpl u) => u.Name != null && u.Name.Equals("PwNo"));
								if (userFieldsImpl != null)
								{
									userFieldsImpl.Value = docentryPro;
									continue;
								}
								userFieldsImpl = new UserFieldsImpl();
								userFieldsImpl.Name = "PwNo";
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								userFieldsImpl.Value = docentryPro;
								batchNumber.UserFields.Add(userFieldsImpl);
							}
						}
					}
					ProductionOder_Impl_OWOR model2 = new ProductionOder_Impl_OWOR
					{
						DocEntry = docentryPro,
						Status = "R"
					};
					ProductionOder_Impl_OWOR model3 = new ProductionOder_Impl_OWOR
					{
						DocEntry = docentryPro,
						Status = "L"
					};
					string text = "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                    <SessionID>" + sessionId + "</SessionID>\r\n                                                                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                                                <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXmlStatus(model2, EnumCommandType.Update) + "</dis:UpdateObject>";
					text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docIssue, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
					text += XmlUpdateRSCostThuHoi(production.Line_WOR1[0].ItemCode, batchCost);
					text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docReceipt, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
					text += $"<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">{MappingXmlStatus(model3, EnumCommandType.Update)}</dis:UpdateObject>";
					text += "</env:Body>";
					string text2 = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
					XmlElement xmlElement2 = DataService.Process(text2).Content as XmlElement;
					XElement xElement3 = XElement.Parse(xmlElement2.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement2.OuterXml) && xmlElement2.OuterXml.Contains("Fault"))
					{
						XElement xElement4 = xElement3.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement4 != null)
						{
							Logging.Write("ERROR", "Confirm Batch Error on Issue Receipt", xElement4.Value);
							Logging.Write("ERROR", text2);
							string text3 = UpdateStatusByDI(docentryPro, "C", sessionId);
							Logging.Write("WATCH", $"Result Cancel Production Order {docentryPro}", text3);
							return new HttpResult(Enums.MessageCode.Error, xElement4.Value);
						}
					}
					else if (!string.IsNullOrEmpty(xmlElement2.OuterXml) && xmlElement2.OuterXml.Contains("RetKey"))
					{
						List<XElement> list3 = (from t in xElement3.Descendants()
							where t.Name.LocalName.Equals("RetKey")
							select t).ToList();
						if (list3.Count > 0)
						{
							List<string> list4 = new List<string>();
							list4.AddRange(list3.Select((XElement t) => Function.ToString(t.Value)));
							if (list4.Count > 0)
							{
								Logging.Write("WATCH", "Confirm Batch Success", string.Join(",", list4.ToArray()));
								return new HttpResult(Enums.MessageCode.Success, "", docentryPro);
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult SaveProductionPlan(Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable> model, string sessionId)
	{
		try
		{
			List<ProductionOder_Impl_OWOR> item = model.Item1;
			DocumentsImpl item2 = model.Item2;
			_ = model.Item3;
			string text = string.Empty;
			if (item != null && item.Count > 0)
			{
				for (int i = 0; i < item.Count; i++)
				{
					string text2 = "";
					text2 = ((item[i].DocEntry <= 0) ? ("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(item[i], EnumCommandType.Add) + "</dis:AddObject>") : ("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(item[i], EnumCommandType.Update) + "</dis:UpdateObject>"));
					text += text2;
				}
			}
			string text3 = string.Empty;
			if (item2 != null && item2.Lines != null && item2.Lines.Count > 0 && 0 <= 0)
			{
				text3 = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.StockTransferRequest.MappingXml(ApzObjectTypes.oInventoryTransferRequest, item2, isDraft: false) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + text3 + "</env:Body></env:Envelopes>";
			Logging.Write("WATCH", "Start save production plan");
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Add production planning error", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Add production planning ", string.Join(",", list2.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public string Remove(int entry, string sessionId)
	{
		return string.Empty;
	}

	bool Apzon.DataServices.Interfaces.IProductionOrders.Exists(int entry, string sessionId)
	{
		return false;
	}

	public virtual HttpResult Adjust(DocumentsImpl[] docs, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Add production adjust ");
			string text = "";
			if (docs[0].Lines != null && docs[0].Lines.Count > 0)
			{
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[0], EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
			}
			if (docs[1].Lines != null && docs[1].Lines.Count > 0)
			{
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[1], EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Add production adjust error", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Add production adjust success", string.Join(",", list2.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual void ProcessProductionOrderLine(string sessionId, int docEntry, List<ProductionOder_LineImpl_WOR1> dtLine)
	{
		try
		{
			string byParams = GetByParams(sessionId, docEntry);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[ProcessProductionOrderLine].[GetByPr] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ProductionOrder"));
			if (string.IsNullOrEmpty(Function.ToString(xElement)))
			{
				return;
			}
			string text = ProcessPoLines(xElement, dtLine, docEntry);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			text = text.Replace("xmlns=\"\"", "");
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProductionOrdersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ProductionOrdersService\">" + Function.ToString(text) + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement3 != null)
				{
					throw new Exception(xElement3.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public string ProcessPoLines(XElement poXml, List<ProductionOder_LineImpl_WOR1> lines, int docEntry)
	{
		try
		{
			XElement xElement = poXml.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ProductionOrderLines"));
			if (xElement != null && xElement.Descendants().Any((XElement t) => t.Name.LocalName.Equals("ProductionOrderLine")))
			{
				xElement.Descendants().Remove();
				int num = 0;
				foreach (ProductionOder_LineImpl_WOR1 line in lines)
				{
					string text = "<ProductionOrderLine>";
					text = text + "<DocumentAbsoluteEntry>" + docEntry + "</DocumentAbsoluteEntry>";
					text = text + "<LineNumber>" + line.LineNum + "</LineNumber>";
					text = text + "<ItemNo>" + Function.RawText(line.ItemCode) + "</ItemNo>";
					text = text + "<BaseQuantity>" + line.BaseQty + "</BaseQuantity>";
					text = text + "<PlannedQuantity>" + line.PlannedQty + "</PlannedQuantity>";
					text = text + "<IssuedQuantity>" + line.IssuedQty + "</IssuedQuantity>";
					text = text + "<ProductionOrderIssueType>" + (line.IssueType.Equals("B") ? "im_Backflush" : "im_Manual") + "</ProductionOrderIssueType>";
					text = text + "<Warehouse>" + line.wareHouse.Trim() + "</Warehouse>";
					text = text + "<VisualOrder>" + num + "</VisualOrder>";
					text = text + "<DistributionRule>" + Function.ToString(line.OcrCode) + "</DistributionRule>";
					text = text + "<Project>" + line.Project.Trim() + "</Project>";
					text = text + "<DistributionRule2>" + Function.ToString(line.OcrCode2) + "</DistributionRule2>";
					text = text + "<DistributionRule3>" + Function.ToString(line.OcrCode3) + "</DistributionRule3>";
					text = text + "<DistributionRule4>" + Function.ToString(line.OcrCode4) + "</DistributionRule4>";
					text = text + "<DistributionRule5>" + Function.ToString(line.OcrCode5) + "</DistributionRule5>";
					text = text + "<UoMEntry>" + Function.ParseInt(line.UomEntry) + "</UoMEntry>";
					text = text + "<UoMCode>" + Function.ParseInt(line.UomCode) + "</UoMCode>";
					text = text + "<WipAccount>" + Function.ToString(line.WipActCode) + "</WipAccount>";
					string text2 = "";
					string itemType = line.ItemType;
					if (itemType == "4")
					{
						text2 = "pit_Item";
					}
					else
					{
						text2 = ((!(itemType == "290")) ? "pit_Text" : "pit_Resource");
					}
					text = text + "<ItemType>" + text2 + "</ItemType>";
					text = text + "<AdditionalQuantity>" + Function.ParseDecimal(line.AdditQty) + "</AdditionalQuantity>";
					text = text + "<StartDate>" + Convert.ToDateTime(line.StartDate).ToString("yyyyMMdd") + "</StartDate>";
					text = text + "<EndDate>" + Convert.ToDateTime(line.EndDate).ToString("yyyyMMdd") + "</EndDate>";
					text += DataService.MappingUdf(line.UserFields);
					text += "<BatchNumbers>";
					if (line.BatchNumbers != null && line.BatchNumbers.Count > 0)
					{
						for (int num2 = 0; num2 < line.BatchNumbers.Count; num2++)
						{
							text += "<row>";
							BatchNumbersImpl batchNumbersImpl = line.BatchNumbers[num2];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								text = text + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(batchNumbersImpl.Notes) + "</Notes>";
							}
							text = text + "<Quantity>" + Function.ParseDecimal(batchNumbersImpl.Quantity) + "</Quantity>";
							if (batchNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(batchNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</BatchNumbers>";
					text += "<SerialNumbers>";
					if (line.SerialNumbers != null && line.SerialNumbers.Count > 0)
					{
						for (int num3 = 0; num3 < line.SerialNumbers.Count; num3++)
						{
							text += "<row>";
							SerialNumbersImpl serialNumbersImpl = line.SerialNumbers[num3];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text = text + "<BatchID>" + Function.ToString(serialNumbersImpl.BatchID) + "</BatchID>";
							}
							if (serialNumbersImpl.SystemSerialNumber > 0)
							{
								text = text + "<SystemSerialNumber>" + serialNumbersImpl.SystemSerialNumber + "</SystemSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
							}
							text = text + "<Quantity>" + Function.ParseDecimal(serialNumbersImpl.Quantity) + "</Quantity>";
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(serialNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</SerialNumbers>";
					text += "</ProductionOrderLine>";
					int num4 = text.IndexOf('<');
					if (num4 > 0)
					{
						text = text.Remove(0, num4);
					}
					xElement.Add(XElement.Parse(text));
					num++;
				}
				return Function.ToString(poXml);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public virtual string GetByParams(string sessionId, int docEntry)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProductionOrdersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"ProductionOrdersService\">\r\n                                     <ProductionOrderParams><AbsoluteEntry>" + docEntry + "</AbsoluteEntry></ProductionOrderParams>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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
			return xmlElement.OuterXml;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public async Task<HttpResult> SaveList(List<Tuple<DataTable, DataTable>> lstDocs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (lstDocs != null && lstDocs.Count > 0)
			{
				StringBuilder request = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>");
				request.Append(sessionId);
				request.Append("</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">");
				Logging.Write("TRACE", "*** Import Prd Order: Start Mapping XML");
				Task[] array = new Task[lstDocs.Count];
				StringBuilder[] sb = new StringBuilder[lstDocs.Count];
				for (int i = 0; i < lstDocs.Count; i++)
				{
					int ti = i;
					sb[ti] = new StringBuilder();
					if (Function.ParseInt(lstDocs[ti].Item1.Rows[0]["DocEntry"]) == 0)
					{
						array[ti] = Task.Run(() => MappingXmlDataTable(lstDocs[ti].Item1.Rows[0], lstDocs[ti].Item2, EnumCommandType.Add, ref sb[ti]));
					}
					else
					{
						array[ti] = Task.Run(() => MappingXmlDataTable(lstDocs[ti].Item1.Rows[0], lstDocs[ti].Item2, EnumCommandType.Update, ref sb[ti]));
					}
				}
				await Task.WhenAll(array);
				Logging.Write("TRACE", "*** Import Prd Order: End Mapping XML");
				for (int num = 0; num < lstDocs.Count; num++)
				{
					if (Function.ParseInt(lstDocs[num].Item1.Rows[0]["DocEntry"]) == 0)
					{
						request.Append("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
						request.Append(sb[num].ToString());
						request.Append("</dis:AddObject>");
					}
					else
					{
						request.Append("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
						request.Append(sb[num].ToString());
						request.Append("</dis:UpdateObject>");
					}
				}
				request.Append("</env:Body></env:Envelopes>");
				Logging.Write("TRACE", "*** Import Prd Order: Start call SAP API. " + lstDocs.Count + " Entries.");
				XmlElement xmlElement = (await DataService.ProcessAsync(request.ToString())).Content as XmlElement;
				Logging.Write("TRACE", "*** Import Prd Order: Finished call SAP API");
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public async Task<HttpResult> SaveDocsPrdImportTools(List<Tuple<DataTable, DataTable>> lstDocs, string sessionId)
	{
		try
		{
			string lstupdStatDoc = "";
			sessionId = GetSessionId(sessionId);
			if (lstDocs != null && lstDocs.Count > 0)
			{
				Logging.Write("TRACE", "*** Import Prd Order: Start add one prd order a time. " + lstDocs.Count + " Entries.");
				string error = "";
				string success = "";
				Stopwatch st = new Stopwatch();
				st.Start();
				StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <SessionID>");
				stringBuilder.Append(sessionId);
				stringBuilder.Append("</SessionID>\r\n                                </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">");
				for (int i = 0; i < lstDocs.Count; i++)
				{
					stringBuilder.Append("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
					stringBuilder.Append(MappingXmlDataTable(lstDocs[i].Item1.Rows[0], lstDocs[i].Item2, EnumCommandType.Add));
					stringBuilder.Append("</dis:AddObject>");
				}
				stringBuilder.Append("</env:Body></env:Envelopes>");
				st.Stop();
				Logging.Write("TRACE", "Mapping: " + st.Elapsed.TotalSeconds);
				st.Reset();
				st.Start();
				XmlElement xmlElement = (await DataService.ProcessAsync(stringBuilder.ToString())).Content as XmlElement;
				st.Stop();
				Logging.Write("TRACE", "Save: " + st.Elapsed.TotalSeconds);
				st.Reset();
				st.Start();
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					int num = xmlElement.OuterXml.IndexOf("<env:Text xml:lang=\"en\">");
					int num2 = xmlElement.OuterXml.IndexOf("</env:Text>");
					if (num >= 0 && num2 > 0)
					{
						error += xmlElement.OuterXml.Substring(num + 24, num2 - num - 24);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							success = success + string.Join(",", list2.ToArray()) + ",";
							List<ProductionOder_Impl_OWOR> list3 = new List<ProductionOder_Impl_OWOR>();
							for (int num3 = 0; num3 < lstDocs.Count; num3++)
							{
								ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
								productionOder_Impl_OWOR.DocEntry = Function.ParseInt(list2[num3]);
								productionOder_Impl_OWOR.CloseDate = Function.ParseDateTime(lstDocs[num3].Item1.Rows[0]["CloseDate"]);
								productionOder_Impl_OWOR.Status = "R";
								list3.Add(productionOder_Impl_OWOR);
							}
							if (!string.IsNullOrEmpty(_unitOfWork.ProductionOrders.UpdateStatus(list3, sessionId).Message))
							{
								for (int num4 = 0; num4 < list3.Count; num4++)
								{
									string text = (list3[num4].CloseDate.HasValue ? list3[num4].CloseDate.Value.ToString("yyyyMMdd") : "nulll");
									lstupdStatDoc = lstupdStatDoc + list3[num4].DocEntry + "," + text + ";";
								}
							}
						}
					}
				}
				st.Stop();
				Logging.Write("TRACE", "Return xml: " + st.Elapsed.TotalSeconds);
				if (!string.IsNullOrEmpty(success))
				{
					Logging.Write("TRACE", "*** Imported Prd Order: " + success);
				}
				if (success.EndsWith(","))
				{
					success.Remove(success.Length - 1);
				}
				if (!string.IsNullOrEmpty(error))
				{
					return new HttpResult(Enums.MessageCode.Error, error);
				}
				return new HttpResult(Enums.MessageCode.Success, "", lstupdStatDoc);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult CancelDoc(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CancelObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>oProductionOrders</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <AbsoluteEntry>" + Function.ToString(docEntry) + "</AbsoluteEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CancelObject>\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Cancel Production Order Error\t{xElement2.Value} ");
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Cancel Production Order\t{num}");
					return new HttpResult(Enums.MessageCode.Success, "", num.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	private Task MappingXmlDataTable(DataRow doc, DataTable item2, EnumCommandType commandType, ref StringBuilder xml)
	{
		xml.Append("<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>");
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				xml.Append("<QueryParams><AbsoluteEntry>");
				xml.Append(Function.ParseInt(doc["DocEntry"]));
				xml.Append("</AbsoluteEntry></QueryParams>");
			}
			xml.Append("<ProductionOrders><row>");
			int num = Function.ParseInt(doc["Series"]);
			if (num > 0 && commandType == EnumCommandType.Add)
			{
				xml.Append("<Series>");
				xml.Append(num);
				xml.Append("</Series>");
			}
			xml.Append("<ItemNo>");
			xml.Append(Function.RawText(doc["ItemCode"]));
			xml.Append("</ItemNo>");
			switch (Function.RawText(doc["Status"]).ToUpper())
			{
			case "R":
				xml.Append("<ProductionOrderStatus>boposReleased</ProductionOrderStatus>");
				break;
			case "L":
				xml.Append("<ProductionOrderStatus>boposClosed</ProductionOrderStatus>");
				break;
			case "C":
				xml.Append("<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>");
				break;
			default:
				xml.Append("<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>");
				break;
			}
			xml.Append("<ProductionOrderType>bopotStandard</ProductionOrderType>");
			xml.Append("<PlannedQuantity>");
			xml.Append(Function.ParseDecimal(doc["PlannedQty"]));
			xml.Append("</PlannedQuantity>");
			xml.Append("<PostingDate>");
			xml.Append(Function.RawText(doc["StPostDate"]));
			xml.Append("</PostingDate>");
			xml.Append("<DueDate>");
			xml.Append(Function.RawText(doc["StDueDate"]));
			xml.Append("</DueDate>");
			xml.Append("<Warehouse>");
			xml.Append(Function.RawText(doc["Warehouse"]));
			xml.Append("</Warehouse>");
			string value = Function.RawText(doc["Project"]);
			if (!string.IsNullOrEmpty(value))
			{
				xml.Append("<Project>");
				xml.Append(value);
				xml.Append("</Project>");
			}
			xml.Append(DataService.MappingUdf(doc, commandType));
			xml.Append("</row></ProductionOrders>");
			if (item2 != null && item2.Rows.Count > 0)
			{
				xml.Append("<ProductionOrders_Lines>");
				for (int i = 0; i < item2.Rows.Count; i++)
				{
					xml.Append("<row>");
					DataRow dataRow = item2.Rows[i];
					xml.Append("<ItemNo>");
					xml.Append(Function.RawText(dataRow["ItemCode"]));
					xml.Append("</ItemNo>");
					xml.Append("<BaseQuantity>");
					xml.Append(Function.ParseDecimal(dataRow["BaseQty"]));
					xml.Append("</BaseQuantity>");
					xml.Append("<PlannedQuantity>");
					xml.Append(Function.ParseDecimal(dataRow["PlannedQty"]));
					xml.Append("</PlannedQuantity>");
					if (Function.RawText(dataRow["IssueType"]).ToUpper() == "M")
					{
						xml.Append("<ProductionOrderIssueType>im_Manual</ProductionOrderIssueType>");
					}
					else
					{
						xml.Append("<ProductionOrderIssueType>im_Backflush</ProductionOrderIssueType>");
					}
					xml.Append("<Warehouse>");
					xml.Append(Function.RawText(dataRow["wareHouse"]));
					xml.Append("</Warehouse>");
					if (!string.IsNullOrEmpty(Function.RawText(dataRow["Project"])))
					{
						xml.Append("<Project>");
						xml.Append(Function.RawText(dataRow["Project"]));
						xml.Append("</Project>");
					}
					xml.Append("<ItemType>pit_Item</ItemType>");
					xml.Append(DataService.MappingUdf(dataRow, commandType));
					xml.Append("</row>");
				}
				xml.Append("</ProductionOrders_Lines>");
			}
			xml.Append("</BO></BOM>");
			return Task.FromResult(0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return Task.FromResult(-1);
		}
	}

	private StringBuilder MappingXmlDataTable(DataRow doc, DataTable item2, EnumCommandType commandType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oProductionOrders</Object>\r\n                              </AdmInfo>");
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<QueryParams><AbsoluteEntry>");
				stringBuilder.Append(Function.ParseInt(doc["DocEntry"]));
				stringBuilder.Append("</AbsoluteEntry></QueryParams>");
			}
			stringBuilder.Append("<ProductionOrders><row>");
			int num = Function.ParseInt(doc["Series"]);
			if (num > 0 && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append("<Series>");
				stringBuilder.Append(num);
				stringBuilder.Append("</Series>");
			}
			stringBuilder.Append("<ItemNo>");
			stringBuilder.Append(Function.RawText(doc["ItemCode"]));
			stringBuilder.Append("</ItemNo>");
			switch (Function.RawText(doc["Status"]).ToUpper())
			{
			case "R":
				stringBuilder.Append("<ProductionOrderStatus>boposReleased</ProductionOrderStatus>");
				break;
			case "L":
				stringBuilder.Append("<ProductionOrderStatus>boposClosed</ProductionOrderStatus>");
				break;
			case "C":
				stringBuilder.Append("<ProductionOrderStatus>boposCancelled</ProductionOrderStatus>");
				break;
			default:
				stringBuilder.Append("<ProductionOrderStatus>boposPlanned</ProductionOrderStatus>");
				break;
			}
			stringBuilder.Append("<ProductionOrderType>bopotStandard</ProductionOrderType>");
			stringBuilder.Append("<PlannedQuantity>");
			stringBuilder.Append(Function.ParseDecimal(doc["PlannedQty"]));
			stringBuilder.Append("</PlannedQuantity>");
			stringBuilder.Append("<PostingDate>");
			stringBuilder.Append(Function.RawText(doc["StPostDate"]));
			stringBuilder.Append("</PostingDate>");
			stringBuilder.Append("<DueDate>");
			stringBuilder.Append(Function.RawText(doc["StDueDate"]));
			stringBuilder.Append("</DueDate>");
			stringBuilder.Append("<Warehouse>");
			stringBuilder.Append(Function.RawText(doc["Warehouse"]));
			stringBuilder.Append("</Warehouse>");
			string value = Function.RawText(doc["Project"]);
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.Append("<Project>");
				stringBuilder.Append(value);
				stringBuilder.Append("</Project>");
			}
			stringBuilder.Append(DataService.MappingUdf(doc, commandType));
			stringBuilder.Append("</row></ProductionOrders>");
			if (item2 != null && item2.Rows.Count > 0)
			{
				stringBuilder.Append("<ProductionOrders_Lines>");
				for (int i = 0; i < item2.Rows.Count; i++)
				{
					stringBuilder.Append("<row>");
					DataRow dataRow = item2.Rows[i];
					stringBuilder.Append("<ItemNo>");
					stringBuilder.Append(Function.RawText(dataRow["ItemCode"]));
					stringBuilder.Append("</ItemNo>");
					stringBuilder.Append("<BaseQuantity>");
					stringBuilder.Append(Function.ParseDecimal(dataRow["BaseQty"]));
					stringBuilder.Append("</BaseQuantity>");
					stringBuilder.Append("<PlannedQuantity>");
					stringBuilder.Append(Function.ParseDecimal(dataRow["PlannedQty"]));
					stringBuilder.Append("</PlannedQuantity>");
					if (Function.RawText(dataRow["IssueType"]).ToUpper() == "M")
					{
						stringBuilder.Append("<ProductionOrderIssueType>im_Manual</ProductionOrderIssueType>");
					}
					else
					{
						stringBuilder.Append("<ProductionOrderIssueType>im_Backflush</ProductionOrderIssueType>");
					}
					stringBuilder.Append("<Warehouse>");
					stringBuilder.Append(Function.RawText(dataRow["wareHouse"]));
					stringBuilder.Append("</Warehouse>");
					if (!string.IsNullOrEmpty(Function.RawText(dataRow["Project"])))
					{
						stringBuilder.Append("<Project>");
						stringBuilder.Append(Function.RawText(dataRow["Project"]));
						stringBuilder.Append("</Project>");
					}
					stringBuilder.Append("<ItemType>pit_Item</ItemType>");
					stringBuilder.Append(DataService.MappingUdf(dataRow, commandType));
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</ProductionOrders_Lines>");
			}
			stringBuilder.Append("</BO></BOM>");
			return stringBuilder;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new StringBuilder();
		}
	}
}
