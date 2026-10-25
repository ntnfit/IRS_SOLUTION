using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Inventory;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaPickListsRepository : PickListsRepository
{
	public HanaPickListsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService, IUnitOfWork serviceUnitOfWork)
		: base(unitOfWork, dataService, serviceUnitOfWork)
	{
	}

	protected override async Task<HttpResult> AddNew(PickListsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc);
			return DataService.AddObject(ApzObjectTypes.oPickLists, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task<HttpResult> UpdateData(PickListsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc);
			return DataService.UpdateObject(ApzObjectTypes.oPickLists, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string Mapping(PickListsImpl doc)
	{
		string text = "{";
		try
		{
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text += Function.GetJsonString("Name", Function.ToString(doc.Name));
			}
			if (!string.IsNullOrEmpty(doc.OwnerCode))
			{
				text += Function.GetJsonString("OwnerCode", Function.ToString(doc.OwnerCode));
			}
			if (doc.PickDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("PickDate", doc.PickDate.ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(doc.Remarks))
			{
				text += Function.GetJsonString("Remarks", Function.ToString(doc.Remarks));
			}
			if (!string.IsNullOrEmpty(doc.UseBaseUn))
			{
				text = ((!(doc.UseBaseUn == "Y")) ? (text + Function.GetJsonString("UseBaseUn", "tNO")) : (text + Function.GetJsonString("UseBaseUn", "tYES")));
			}
			text += DataService.MappingUdf(doc.UserFields);
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				text += "\"PickListsLines\":[";
				for (int i = 0; i < doc.Lines.Count; i++)
				{
					PickLists_LineImpl pickLists_LineImpl = doc.Lines[i];
					text += "{";
					text += Function.GetJsonString("AbsoluteEntry", 5);
					text += Function.GetJsonString("OrderEntry", Function.ParseInt(pickLists_LineImpl.OrderEntry));
					text += Function.GetJsonString("OrderRowID", Function.ParseInt(pickLists_LineImpl.OrderLine));
					text += Function.GetJsonString("PickedQuantity", Function.ParseDecimal(pickLists_LineImpl.PickQtty));
					text += Function.GetJsonString("ReleasedQuantity", Function.ParseDecimal(pickLists_LineImpl.RelQtty));
					if (pickLists_LineImpl.BaseObject != 0)
					{
						text += Function.GetJsonString("BaseObjectType", Function.ParseInt(pickLists_LineImpl.BaseObject));
					}
					text += DataService.MappingUdf(pickLists_LineImpl.UserFields);
					text += "},";
				}
				text += "],";
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "\"BatchNumbers\": [";
					for (int j = 0; j < doc.Lines.Count; j++)
					{
						if (doc.Lines[j].BatchNumbers == null || doc.Lines[j].BatchNumbers.Count <= 0)
						{
							continue;
						}
						for (int k = 0; k < doc.Lines[j].BatchNumbers.Count; k++)
						{
							text += "{";
							BatchNumbersImpl batchNumbersImpl = doc.Lines[j].BatchNumbers[k];
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
								text += Function.GetJsonString("ExpiryDate", Function.ParseDateTimes(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd"));
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text += Function.GetJsonString("ManufacturingDate", Function.ParseDateTimes(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd"));
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text += Function.GetJsonString("AddmisionDate", Function.ParseDateTimes(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd"));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text += Function.GetJsonString("Location", Function.ToString(batchNumbersImpl.Location));
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								text += Function.GetJsonString("Notes", Function.ToString(batchNumbersImpl.Notes));
							}
							if (!batchNumbersImpl.Quantity.Equals(0.0))
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
							text += DataService.MappingUdf(batchNumbersImpl.UserFields);
							text += "},";
						}
					}
					text += "],";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "\"SerialNumbers\": [";
					for (int l = 0; l < doc.Lines.Count; l++)
					{
						if (doc.Lines[l].SerialNumbers == null || doc.Lines[l].SerialNumbers.Count <= 0)
						{
							continue;
						}
						for (int m = 0; m < doc.Lines[l].SerialNumbers.Count; m++)
						{
							text += "{";
							SerialNumbersImpl serialNumbersImpl = doc.Lines[l].SerialNumbers[m];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text += Function.GetJsonString("BatchID", Function.ToString(serialNumbersImpl.BatchID));
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
								text += Function.GetJsonString("ExpiryDate", Function.ParseDateTimes(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd"));
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								text += Function.GetJsonString("ManufacturingDate", Function.ParseDateTimes(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd"));
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								text += Function.GetJsonString("ReceptionDate", Function.ParseDateTimes(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd"));
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text += Function.GetJsonString("Location", Function.ToString(serialNumbersImpl.Location));
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text += Function.GetJsonString("Notes", Function.ToString(serialNumbersImpl.Notes));
							}
							if (!serialNumbersImpl.Quantity.Equals(0.0))
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
							text += DataService.MappingUdf(serialNumbersImpl.UserFields);
							text += "},";
						}
					}
					text += "],";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "\"DocumentLinesBinAllocations\": [";
					for (int n = 0; n < doc.Lines.Count; n++)
					{
						if (doc.Lines[n].BinAllocations == null || doc.Lines[n].BinAllocations.Count <= 0)
						{
							continue;
						}
						for (int num = 0; num < doc.Lines[n].BinAllocations.Count; num++)
						{
							text += "{";
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[n].BinAllocations[num];
							if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
							{
								text += Function.GetJsonString("BinAbsEntry", Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry));
							}
							if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
							{
								text += Function.GetJsonString("Quantity", Function.ParseDouble(documentLinesBinAllocationsImpl.Quantity));
							}
							if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
							{
								text += Function.GetJsonString("SerialAndBatchNumbersBaseLine", Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine));
							}
							if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
							{
								text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber));
							}
							text += "}";
						}
					}
					text += "]";
				}
			}
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
