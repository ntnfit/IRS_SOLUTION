using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaProductionOrdersRepository : ProductionOrdersRepository
{
	public HanaProductionOrdersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(ProductionOder_Impl_OWOR po, string sessionId)
	{
		if (po.DocEntry <= 0 || po.AtcEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(po.AttachmentFiles, sessionId);
		}
		return _unitOfWork.Attachment2.Update(po.AtcEntry, po.AttachmentFiles, sessionId);
	}

	public override async Task<HttpResult> SaveList(List<ProductionOder_Impl_OWOR> oDocs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			sessionId = GetSessionId(sessionId);
			if (oDocs != null && oDocs.Count > 0)
			{
				_ = string.Empty;
				for (int i = 0; i < oDocs.Count; i++)
				{
					if (IsExistsPO(oDocs[i].DocEntry))
					{
						string bodyObject = MappingXml(oDocs[i], EnumCommandType.Update);
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oProductionOrders,
							OutObjectKey = true,
							Key = oDocs[i].DocEntry,
							Action = Enums.HttpAction.Update,
							BodyObject = bodyObject
						});
					}
					else
					{
						string bodyObject = MappingXml(oDocs[i], EnumCommandType.Add);
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oProductionOrders,
							OutObjectKey = true,
							Action = Enums.HttpAction.AddNew,
							BodyObject = bodyObject
						});
					}
				}
				HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult;
				}
				if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
				{
					int num = Function.ParseInt(httpResult.Content);
					return new HttpResult(Enums.MessageCode.Success, "", num);
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

	public override string MappingXml(ProductionOder_Impl_OWOR model, EnumCommandType commandType, bool mapAttachment = false)
	{
		string text = "{";
		try
		{
			if (Function.ParseInt(model.Series) > 0 && commandType == EnumCommandType.Add)
			{
				text += Function.GetJsonString("Series", Function.ToString(model.Series));
			}
			if (commandType == EnumCommandType.Add && model.Series.HasValue)
			{
				_ = model.Series.Value;
				if (_unitOfWork.Common.GetIsManualSerial(model.Series.Value))
				{
					text += Function.GetJsonString("DocumentNumber", Function.ToString(model.DocNum));
				}
			}
			if (!string.IsNullOrEmpty(model.ItemCode))
			{
				text += Function.GetJsonString("ItemNo", Function.ToString(model.ItemCode));
			}
			if (!string.IsNullOrEmpty(model.Status))
			{
				text = model.Status switch
				{
					"R" => text + Function.GetJsonString("ProductionOrderStatus", "boposReleased"), 
					"L" => text + Function.GetJsonString("ProductionOrderStatus", "boposClosed"), 
					"C" => text + Function.GetJsonString("ProductionOrderStatus", "boposCancelled"), 
					_ => text + Function.GetJsonString("ProductionOrderStatus", "boposPlanned"), 
				};
			}
			if (!string.IsNullOrEmpty(model.Type))
			{
				text = model.Type switch
				{
					"D" => text + Function.GetJsonString("ProductionOrderType", "bopotDisassembly"), 
					"S" => text + Function.GetJsonString("ProductionOrderType", "bopotStandard"), 
					"P" => text + Function.GetJsonString("ProductionOrderType", "bopotSpecial"), 
					_ => text + Function.GetJsonString("ProductionOrderType", "bopotStandard"), 
				};
			}
			if (model.PlannedQty.HasValue && !model.PlannedQty.Equals(0))
			{
				text += Function.GetJsonString("PlannedQuantity", Function.ToString((decimal)Function.ParseDouble(model.PlannedQty)));
			}
			if (model.PostDate.HasValue && model.PostDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("PostingDate", Convert.ToDateTime(model.PostDate).ToString("yyyyMMdd"));
			}
			if (model.StartDate.HasValue && model.StartDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("StartDate", Convert.ToDateTime(model.StartDate).ToString("yyyyMMdd"));
			}
			if (model.DueDate.HasValue && model.DueDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DueDate", Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd"));
			}
			if (model.OriginAbs > 0)
			{
				text += Function.GetJsonString("ProductionOrderOriginEntry", Function.ParseInt(model.OriginAbs));
			}
			if (model.OriginAbs > 0)
			{
				text = model.OriginType switch
				{
					"M" => text + Function.GetJsonString("ProductionOrderOrigin", "bopooManual"), 
					"R" => text + Function.GetJsonString("ProductionOrderOrigin", "bopooMRP"), 
					"S" => text + Function.GetJsonString("ProductionOrderOrigin", "bopooSalesOrder"), 
					_ => text + Function.GetJsonString("ProductionOrderOrigin", "bopooManual"), 
				};
			}
			if (mapAttachment)
			{
				text = ((model.AtcEntry == 0 || model.AttachmentFiles == null || model.AttachmentFiles.Count <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", Function.ParseInt(model.AtcEntry))));
			}
			if (!string.IsNullOrEmpty(model.Comments))
			{
				text += Function.GetJsonString("Remarks", Function.ToString(model.Comments));
			}
			if (model.CloseDate.HasValue && model.CloseDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("ClosingDate", Convert.ToDateTime(model.CloseDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(model.CardCode))
			{
				text += Function.GetJsonString("CustomerCode", Function.ToString(model.CardCode));
			}
			if (!string.IsNullOrEmpty(model.Warehouse))
			{
				text += Function.GetJsonString("Warehouse", Function.ToString(model.Warehouse));
			}
			if (!string.IsNullOrEmpty(model.JrnlMemo))
			{
				text += Function.GetJsonString("JournalRemarks", Function.ToString(model.JrnlMemo));
			}
			if (!string.IsNullOrEmpty(model.OcrCode))
			{
				text += Function.GetJsonString("DistributionRule", Function.ToString(model.OcrCode));
			}
			if (!string.IsNullOrEmpty(model.OcrCode2))
			{
				text += Function.GetJsonString("DistributionRule2", Function.ToString(model.OcrCode2));
			}
			if (!string.IsNullOrEmpty(model.OcrCode3))
			{
				text += Function.GetJsonString("DistributionRule3", Function.ToString(model.OcrCode3));
			}
			if (!string.IsNullOrEmpty(model.OcrCode4))
			{
				text += Function.GetJsonString("DistributionRule4", Function.ToString(model.OcrCode4));
			}
			if (!string.IsNullOrEmpty(model.OcrCode5))
			{
				text += Function.GetJsonString("DistributionRule5", Function.ToString(model.OcrCode5));
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text += Function.GetJsonString("Project", Function.ToString(model.Project));
			}
			if (Function.ParseInt(model.UserSign) > 0)
			{
				text += Function.GetJsonString("UserSignature", Function.ParseInt(model.UserSign));
			}
			text += DataService.MappingUdf(model.UserFields, commandType);
			if (model.Line_WOR1 != null && model.Line_WOR1.Count > 0)
			{
				text += "\"ProductionOrderLines\":[";
				for (int i = 0; i < model.Line_WOR1.Count; i++)
				{
					text += "{";
					ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = model.Line_WOR1[i];
					string itemType = productionOder_LineImpl_WOR.ItemType;
					if (itemType == "4")
					{
						text += Function.GetJsonString("ItemType", "pit_Item");
					}
					else
					{
						text = ((!(itemType == "290")) ? (text + Function.GetJsonString("ItemType", "pit_Text")) : (text + Function.GetJsonString("ItemType", "pit_Resource")));
					}
					if (productionOder_LineImpl_WOR.LineNum >= 0)
					{
						text += Function.GetJsonString("LineNumber", Function.ParseInt(productionOder_LineImpl_WOR.LineNum));
					}
					if (productionOder_LineImpl_WOR.ItemType == "-18")
					{
						text += Function.GetJsonString("LineText", Function.ToString(productionOder_LineImpl_WOR.ItemCode));
					}
					else
					{
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.ItemCode))
						{
							text += Function.GetJsonString("ItemNo", Function.ToString(productionOder_LineImpl_WOR.ItemCode));
						}
						text += Function.GetJsonString("ItemName", Function.ToString(productionOder_LineImpl_WOR.ItemName));
						if (productionOder_LineImpl_WOR.BaseQty.HasValue && !productionOder_LineImpl_WOR.BaseQty.Equals(0))
						{
							text += Function.GetJsonString("BaseQuantity", Function.ToString((decimal)Function.ParseDouble(productionOder_LineImpl_WOR.BaseQty)));
						}
						if (productionOder_LineImpl_WOR.PlannedQty.HasValue && !productionOder_LineImpl_WOR.PlannedQty.Equals(0))
						{
							text += Function.GetJsonString("PlannedQuantity", Function.ToString((decimal)Function.ParseDouble(productionOder_LineImpl_WOR.PlannedQty)));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.IssueType))
						{
							text += Function.GetJsonString("ProductionOrderIssueType", productionOder_LineImpl_WOR.IssueType.Equals("M") ? "im_Manual" : "im_Backflush");
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.wareHouse))
						{
							text += Function.GetJsonString("Warehouse", Function.ToString(productionOder_LineImpl_WOR.wareHouse));
						}
						if (productionOder_LineImpl_WOR.LocCode.HasValue && !productionOder_LineImpl_WOR.LocCode.Equals(0))
						{
							text += Function.GetJsonString("LocationCode", Function.ToString(productionOder_LineImpl_WOR.LocCode));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.Project))
						{
							text += Function.GetJsonString("Project", Function.ToString(productionOder_LineImpl_WOR.Project));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode))
						{
							text += Function.GetJsonString("DistributionRule", Function.ToString(productionOder_LineImpl_WOR.OcrCode));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode2))
						{
							text += Function.GetJsonString("DistributionRule2", Function.ToString(productionOder_LineImpl_WOR.OcrCode2));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode3))
						{
							text += Function.GetJsonString("DistributionRule3", Function.ToString(productionOder_LineImpl_WOR.OcrCode3));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode4))
						{
							text += Function.GetJsonString("DistributionRule4", Function.ToString(productionOder_LineImpl_WOR.OcrCode4));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.OcrCode5))
						{
							text += Function.GetJsonString("DistributionRule5", Function.ToString(productionOder_LineImpl_WOR.OcrCode5));
						}
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR.WipActCode))
						{
							text += Function.GetJsonString("WipAccount", Function.ToString(productionOder_LineImpl_WOR.WipActCode));
						}
						if (productionOder_LineImpl_WOR.AdditQty.HasValue && !productionOder_LineImpl_WOR.AdditQty.Equals(0))
						{
							text += Function.GetJsonString("AdditionalQuantity", Function.ToString((decimal)Function.ParseDouble(productionOder_LineImpl_WOR.AdditQty)));
						}
						if (productionOder_LineImpl_WOR.StartDate.HasValue && productionOder_LineImpl_WOR.StartDate > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("StartDate", Convert.ToDateTime(productionOder_LineImpl_WOR.StartDate).ToString("yyyyMMdd"));
						}
						if (productionOder_LineImpl_WOR.EndDate.HasValue && productionOder_LineImpl_WOR.EndDate > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("EndDate", Convert.ToDateTime(productionOder_LineImpl_WOR.EndDate).ToString("yyyyMMdd"));
						}
						text += DataService.MappingUdf(productionOder_LineImpl_WOR.UserFields, commandType);
						if (model.Line_WOR1[i].BatchNumbers != null && model.Line_WOR1[i].BatchNumbers.Count > 0)
						{
							text += "\"BatchNumbers\":[";
							for (int j = 0; j < model.Line_WOR1[i].BatchNumbers.Count; j++)
							{
								text += "{";
								BatchNumbersImpl batchNumbersImpl = model.Line_WOR1[i].BatchNumbers[j];
								if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
								{
									text += Function.GetJsonString("BatchNumber", Function.ToString(batchNumbersImpl.BatchNumber));
								}
								if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
								{
									text += Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(batchNumbersImpl.ManufacturerSerialNumber));
								}
								if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
								{
									text += Function.GetJsonString("InternalSerialNumber", Function.ToString(batchNumbersImpl.InternalSerialNumber));
								}
								if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("ExpiryDate", Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd"));
								}
								if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("ManufacturingDate", Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd"));
								}
								if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("AddmisionDate", Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd"));
								}
								if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
								{
									text += Function.GetJsonString("Location", Function.ToString(batchNumbersImpl.Location));
								}
								if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
								{
									text += Function.GetJsonString("Notes", Function.ToString(batchNumbersImpl.Notes));
								}
								if (commandType == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
								{
									text += Function.GetJsonString("Quantity", Function.ParseDouble(batchNumbersImpl.Quantity));
								}
								if (batchNumbersImpl.BaseLineNumber >= 0)
								{
									text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(batchNumbersImpl.BaseLineNumber));
								}
								if (0 < batchNumbersImpl.TrackingNote)
								{
									text += Function.GetJsonString("TrackingNote", Function.ToString(batchNumbersImpl.TrackingNote));
								}
								if (0 < batchNumbersImpl.TrackingNoteLine)
								{
									text += Function.GetJsonString("TrackingNoteLine", Function.ToString(batchNumbersImpl.TrackingNoteLine));
								}
								text += DataService.MappingUdf(batchNumbersImpl.UserFields, commandType);
								text += "},";
							}
							text += "],";
						}
						if (model.Line_WOR1[i].SerialNumbers != null && model.Line_WOR1[i].SerialNumbers.Count > 0)
						{
							text += "\"SerialNumbers\":[";
							for (int k = 0; k < model.Line_WOR1[i].SerialNumbers.Count; k++)
							{
								text += "{";
								SerialNumbersImpl serialNumbersImpl = model.Line_WOR1[i].SerialNumbers[k];
								if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
								{
									text += Function.GetJsonString("BatchID", Function.ToString(serialNumbersImpl.BatchID));
								}
								if (serialNumbersImpl.SystemSerialNumber > 0)
								{
									text += Function.GetJsonString("SystemSerialNumber", serialNumbersImpl.SystemSerialNumber);
								}
								if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
								{
									text += Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(serialNumbersImpl.ManufacturerSerialNumber));
								}
								if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
								{
									text += Function.GetJsonString("InternalSerialNumber", Function.ToString(serialNumbersImpl.InternalSerialNumber));
								}
								if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("ExpiryDate", Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd"));
								}
								if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("ManufacturingDate", Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd"));
								}
								if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
								{
									text += Function.GetJsonString("ReceptionDate", Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd"));
								}
								if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
								{
									text += Function.GetJsonString("Location", Function.ToString(serialNumbersImpl.Location));
								}
								if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
								{
									text += Function.GetJsonString("Notes", Function.ToString(serialNumbersImpl.Notes));
								}
								if (commandType == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
								{
									text += Function.GetJsonString("Quantity", Function.ParseDouble(serialNumbersImpl.Quantity));
								}
								if (serialNumbersImpl.BaseLineNumber >= 0)
								{
									text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(serialNumbersImpl.BaseLineNumber));
								}
								if (0 < serialNumbersImpl.TrackingNote)
								{
									text += Function.GetJsonString("TrackingNote", Function.ToString(serialNumbersImpl.TrackingNote));
								}
								if (0 < serialNumbersImpl.TrackingNoteLine)
								{
									text += Function.GetJsonString("TrackingNoteLine", Function.ToString(serialNumbersImpl.TrackingNoteLine));
								}
								text += DataService.MappingUdf(serialNumbersImpl.UserFields, commandType);
								text += "},";
							}
							text += "],";
						}
					}
					text += "},";
				}
				text += "],";
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override string MappingXmlStatus(ProductionOder_Impl_OWOR model, EnumCommandType type)
	{
		try
		{
			string text = "{";
			if (!string.IsNullOrEmpty(model.Status))
			{
				switch (model.Status)
				{
				case "R":
					text += Function.GetJsonString("ProductionOrderStatus", "boposReleased");
					break;
				case "L":
					text += Function.GetJsonString("ProductionOrderStatus", "boposClosed");
					text = ((!Function.ParseDateTime(model.CloseDate).HasValue || !(Function.ParseDateTime(model.CloseDate) > new DateTime(1900, 1, 1))) ? (text + Function.GetJsonString("ClosingDate", DateTime.Today.ToString("yyyyMMdd"))) : (text + Function.GetJsonString("ClosingDate", Function.ParseDateTime(model.CloseDate).Value.ToString("yyyyMMdd"))));
					break;
				case "C":
					text += Function.GetJsonString("ProductionOrderStatus", "boposCancelled");
					break;
				default:
					text += Function.GetJsonString("ProductionOrderStatus", "boposPlanned");
					break;
				}
			}
			text += "},\r\n                        ],";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override HttpResult Add(ProductionOder_Impl_OWOR po, string sessionId, bool mapAttachment)
	{
		try
		{
			string jsonObject = MappingXml(po, EnumCommandType.Add, mapAttachment);
			return DataService.ProcessJson(ApzObjectTypes.oProductionOrders, sessionId, jsonObject, Enums.HttpAction.AddNew, po.DocEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Update(ProductionOder_Impl_OWOR po, string sessionId, bool mapAttachment)
	{
		try
		{
			string jsonObject = MappingXml(po, EnumCommandType.Update, mapAttachment);
			return DataService.ProcessJson(ApzObjectTypes.oProductionOrders, sessionId, jsonObject, Enums.HttpAction.Update, po.DocEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult UpdateStatus(List<ProductionOder_Impl_OWOR> oDocs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			sessionId = GetSessionId(sessionId);
			if (oDocs != null && oDocs.Count > 0)
			{
				_ = string.Empty;
				for (int i = 0; i < oDocs.Count; i++)
				{
					string text = "";
					text = MappingXmlStatus(oDocs[i], EnumCommandType.Update);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						Key = oDocs[i].DocEntry,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						BodyObject = text
					});
				}
				Logging.Write("WATCH", string.Format("Start update ...", ApzObjectTypes.oProductionOrders));
				HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("ERROR", "Closed PO Error ", httpResult.Message);
					return httpResult;
				}
				string text2 = Function.ToString(httpResult.Content);
				Logging.Write("WATCH", "Closed PO Success ", text2);
				return new HttpResult(Enums.MessageCode.Success, "", text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult();
	}

	public override async Task<HttpResult> PackingBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (model != null)
			{
				DocumentsImpl item = model.Item1;
				DocumentsImpl item2 = model.Item2;
				ProductionOder_Impl_OWOR item3 = model.Item3;
				if (item != null && item2 != null && item3 != null)
				{
					List<Document_LinesImpl> lines = item.Lines;
					List<string> list2 = new List<string>();
					for (int i = 0; i < lines.Count; i++)
					{
						foreach (BatchNumbersImpl batchNumber in lines[i].BatchNumbers)
						{
							string query = "Apz_PackingBatch_Check";
							List<SqlParameter> list3 = new List<SqlParameter>();
							list3.Add(new SqlParameter("@BatchNo", batchNumber.BatchNumber));
							string text = Function.ToString(ExecuteScarlar(query, CommandType.StoredProcedure, list3.ToArray()));
							if (!string.IsNullOrEmpty(text))
							{
								return new HttpResult(Enums.MessageCode.Error, text);
							}
						}
						list2.AddRange(lines[i].BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
					}
					int num = 0;
					Logging.Write("WATCH", "Start Packing Batch...");
					string bodyObject = MappingXml(item3, EnumCommandType.Add);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject
					});
					HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						Logging.Write("ERROR", "Packing Batch Error", httpResult.Message);
						return httpResult;
					}
					if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
					{
						int num2 = Function.ParseInt(httpResult.Content);
						return new HttpResult(Enums.MessageCode.Success, "", num2);
					}
					if (num <= 0)
					{
						Logging.Write("ERROR", "Packing Batch Error", "Could not add PO " + num);
						return new HttpResult(Enums.MessageCode.Error, "Could not add new production order!");
					}
					double batchCost = GetBatchCost(string.Join(",", list2.ToArray()));
					item.Ref2 = num.ToString();
					item.Comments = "Packing batch!";
					item2.Ref2 = num.ToString();
					item.Comments = "Packing batch!";
					if (item2.Lines != null && item2.Lines.Count > 0)
					{
						for (int num3 = 0; num3 < item2.Lines.Count; num3++)
						{
							item2.Lines[num3].BaseEntry = num;
							item2.Lines[num3].BaseRef = num;
							UserFieldsImpl userFieldsImpl = item2.Lines[num3].BatchNumbers[0].UserFields.FirstOrDefault((UserFieldsImpl u) => u.Name.Equals("U_PwNo") || u.Name.Equals("PwNo"));
							if (userFieldsImpl != null)
							{
								userFieldsImpl.Value = num;
							}
						}
					}
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR
					{
						DocEntry = num,
						Status = "R"
					};
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR2 = new ProductionOder_Impl_OWOR
					{
						DocEntry = num,
						Status = "L"
					};
					List<BodyRequestObject> list4 = new List<BodyRequestObject>();
					string bodyObject2 = MappingXmlStatus(productionOder_Impl_OWOR, EnumCommandType.Update);
					list4.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = productionOder_Impl_OWOR.DocEntry,
						BodyObject = bodyObject2
					});
					string bodyObject3 = _unitOfWork.Documents.Mapping(item, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit);
					list4.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oInventoryGenExit,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject3
					});
					string bodyObject4 = XmlUpdateRSCostThuHoi(item3.Line_WOR1[0].ItemCode, batchCost);
					list4.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oResources,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = item3.Line_WOR1[0].DocEntry,
						BodyObject = bodyObject4
					});
					string bodyObject5 = _unitOfWork.Documents.Mapping(item2, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry);
					list4.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oInventoryGenEntry,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject5
					});
					string bodyObject6 = MappingXmlStatus(productionOder_Impl_OWOR2, EnumCommandType.Update);
					list4.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = productionOder_Impl_OWOR2.DocEntry,
						BodyObject = bodyObject6
					});
					HttpResult httpResult2 = DataService.ProcessMutiTransaction(sessionId, list4);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						string text2 = UpdateStatusByDI(num, "C", sessionId);
						Logging.Write("ERROR", "Packing Batch Error on Issue Receipt", httpResult.Message);
						Logging.Write("WATCH", $"Result Cancel Production Order {num}", text2);
						return httpResult2;
					}
					int num4 = Function.ParseInt(httpResult2.Content);
					Logging.Write("WATCH", string.Format("Packing Batch Success", num4));
					return new HttpResult(Enums.MessageCode.Success, "", num4);
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

	protected override string XmlUpdateRSCostThuHoiQuery()
	{
		return "UPDATE ORSC\r\n                                                SET \"StdCost1\"={0}, \"StdCost2\"=0, \"StdCost3\"=0, \"StdCost4\"=0, \"StdCost5\"=0, \"StdCost6\"=0, \"StdCost7\"=0, \"StdCost8\"=0, \"StdCost9\"=0, \"StdCost10\"=0\r\n                                                FROM ORSB a\r\n                                                WHERE a.\"ResGrpCod\" = ORSC.\"ResGrpCod\"\r\n                                                    AND ORSC.\"ResCode\" = '{1}';";
	}

	public override async Task<HttpResult> ConfirmBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId)
	{
		try
		{
			if (model != null)
			{
				DocumentsImpl item = model.Item1;
				DocumentsImpl item2 = model.Item2;
				ProductionOder_Impl_OWOR item3 = model.Item3;
				if (item != null && item2 != null && item3 != null)
				{
					List<Document_LinesImpl> lines = item.Lines;
					List<string> list = new List<string>();
					for (int i = 0; i < lines.Count; i++)
					{
						list.AddRange(lines[i].BatchNumbers.Select((BatchNumbersImpl t) => t.BatchNumber));
					}
					int num = 0;
					Logging.Write("WATCH", "Start Confirm Batch...");
					string xmlObject = MappingXml(item3, EnumCommandType.Add);
					HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oProductionOrders, sessionId, xmlObject);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						Logging.Write("ERROR", "Packing Batch Error", httpResult.Message);
						return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
					}
					if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
					{
						num = Function.ParseInt(httpResult.Content);
					}
					if (num <= 0)
					{
						Logging.Write("ERROR", "Confirm Batch Error", "Could not add PO " + num);
						return new HttpResult(Enums.MessageCode.Error, "Could not add new production order!");
					}
					double batchCost = GetBatchCost(string.Join(",", list.ToArray()));
					item.Ref2 = num.ToString();
					item.Comments = "Confirm batch!";
					item2.Ref2 = num.ToString();
					item.Comments = "Confirm batch!";
					if (item2.Lines != null && item2.Lines.Count > 0)
					{
						foreach (Document_LinesImpl line in item2.Lines)
						{
							line.BaseEntry = num;
							line.BaseRef = num;
							foreach (BatchNumbersImpl batchNumber in line.BatchNumbers)
							{
								UserFieldsImpl userFieldsImpl = batchNumber.UserFields.FirstOrDefault((UserFieldsImpl u) => u.Name != null && u.Name.Equals("PwNo"));
								if (userFieldsImpl != null)
								{
									userFieldsImpl.Value = num;
									continue;
								}
								userFieldsImpl = new UserFieldsImpl();
								userFieldsImpl.Name = "PwNo";
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								userFieldsImpl.Value = num;
								batchNumber.UserFields.Add(userFieldsImpl);
							}
						}
					}
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR
					{
						DocEntry = num,
						Status = "R"
					};
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR2 = new ProductionOder_Impl_OWOR
					{
						DocEntry = num,
						Status = "L"
					};
					List<BodyRequestObject> list2 = new List<BodyRequestObject>();
					string bodyObject = MappingXmlStatus(productionOder_Impl_OWOR, EnumCommandType.Update);
					list2.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = productionOder_Impl_OWOR.DocEntry,
						BodyObject = bodyObject
					});
					string bodyObject2 = _unitOfWork.Documents.Mapping(item, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit);
					list2.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oInventoryGenExit,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject2
					});
					string bodyObject3 = XmlUpdateRSCostThuHoi(item3.Line_WOR1[0].ItemCode, batchCost);
					list2.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oResources,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = item3.Line_WOR1[0].DocEntry,
						BodyObject = bodyObject3
					});
					string bodyObject4 = _unitOfWork.Documents.Mapping(item2, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry);
					list2.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oInventoryGenEntry,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject4
					});
					string bodyObject5 = MappingXmlStatus(productionOder_Impl_OWOR2, EnumCommandType.Update);
					list2.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProductionOrders,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = productionOder_Impl_OWOR2.DocEntry,
						BodyObject = bodyObject5
					});
					HttpResult httpResult2 = DataService.ProcessMutiTransaction(sessionId, list2);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						string text = UpdateStatusByDI(num, "C", sessionId);
						Logging.Write("ERROR", "Confirm Batch Error on Issue Receipt", httpResult2.Message);
						Logging.Write("WATCH", $"Result Cancel Production Order {num}", text);
						return new HttpResult(Enums.MessageCode.Error, httpResult2.Message);
					}
					int num2 = Function.ParseInt(httpResult2.Content);
					Logging.Write("WATCH", "Confirm Batch Success", Function.ToString(num2));
					return new HttpResult(Enums.MessageCode.Success, "", num2);
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

	public override HttpResult SaveProductionPlan(Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable> model, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			List<ProductionOder_Impl_OWOR> item = model.Item1;
			DocumentsImpl item2 = model.Item2;
			_ = model.Item3;
			_ = string.Empty;
			if (item != null && item.Count > 0)
			{
				for (int i = 0; i < item.Count; i++)
				{
					string text = "";
					if (IsExistsPO(item[i].DocEntry))
					{
						text = MappingXml(item[i], EnumCommandType.Update);
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oProductionOrders,
							OutObjectKey = true,
							Action = Enums.HttpAction.Update,
							Key = item[i].DocEntry,
							BodyObject = text
						});
					}
					else
					{
						text = MappingXml(item[i], EnumCommandType.Add);
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oProductionOrders,
							OutObjectKey = true,
							Action = Enums.HttpAction.AddNew,
							BodyObject = text
						});
					}
				}
			}
			string empty = string.Empty;
			if (item2 != null && item2.Lines != null && item2.Lines.Count > 0 && 0 <= 0)
			{
				empty = _unitOfWork.StockTransferRequest.MappingXml(ApzObjectTypes.oInventoryTransferRequest, item2, isDraft: false);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oInventoryTransferRequest,
					OutObjectKey = true,
					Action = Enums.HttpAction.AddNew,
					BodyObject = empty
				});
			}
			Logging.Write("WATCH", "Start save production plan");
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Add production planning error", httpResult.Message);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
			{
				Logging.Write("WATCH", "Add production planning ", Function.ToString(httpResult.Content));
				return new HttpResult(Enums.MessageCode.Success, "", Function.ToString(httpResult.Content));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public override HttpResult Adjust(DocumentsImpl[] docs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			Logging.Write("WATCH", "Start Add production adjust ");
			if (docs[0].Lines != null && docs[0].Lines.Count > 0)
			{
				string bodyObject = _unitOfWork.Documents.Mapping(docs[0], EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oInventoryGenExit,
					OutObjectKey = true,
					Action = Enums.HttpAction.AddNew,
					BodyObject = bodyObject
				});
			}
			if (docs[1].Lines != null && docs[1].Lines.Count > 0)
			{
				string bodyObject2 = _unitOfWork.Documents.Mapping(docs[1], EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry);
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oInventoryGenEntry,
					OutObjectKey = true,
					Action = Enums.HttpAction.AddNew,
					BodyObject = bodyObject2
				});
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Add production adjust error", httpResult.Message);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
			{
				string text = Function.ToString(httpResult.Content);
				Logging.Write("WATCH", "Add production adjust success", text);
				return new HttpResult(Enums.MessageCode.Success, "", text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public override HttpResult CancelDoc(int docEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, new List<BodyRequestObject>
			{
				new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oProductionOrders,
					Action = Enums.HttpAction.Cancel,
					Key = docEntry
				}
			});
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", $"Cancel Production Order Error\t{httpResult.Message} ");
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			Logging.Write("WATCH", $"Cancel Production Order\t{docEntry}");
			return new HttpResult(Enums.MessageCode.Success, "", docEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
