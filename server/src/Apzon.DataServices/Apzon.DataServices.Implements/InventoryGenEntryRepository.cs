using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InventoryGenEntryRepository : Repository<DocumentsImpl>, IInventoryGenEntry, IRepository<DocumentsImpl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public InventoryGenEntryRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public async Task<string> Save(DocumentsImpl doc, string sessionId)
	{
		try
		{
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string AutoReportComplete(DataSet dt, string sessionId)
	{
		try
		{
			if (dt == null || dt.Tables.Count < 2 || dt.Tables[0].Rows.Count == 0 || dt.Tables[1].Rows.Count == 0)
			{
				return "Data aren't provide!";
			}
			DocumentsImpl documentsImpl = new DocumentsImpl
			{
				DocEntry = 0,
				DocNum = 0,
				BPLId = Function.ParseInt(dt.Tables[0].Rows[0]["BplID"]),
				DocDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["DocDate"]),
				DocDueDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["DocDueDate"]),
				ObjType = 59,
				Comments = "Receipt from production",
				GroupNum = -1,
				JrnlMemo = "Receipt from production",
				DocStatus = "O",
				Series = Function.ParseInt(dt.Tables[0].Rows[0]["Series"]),
				DocRate = 1.0,
				DocType = "I",
				Printed = "N",
				DocCur = Function.ToString(dt.Tables[0].Rows[0]["DocCur"]),
				UpdateDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["UpdateDate"]),
				CreateDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["CreateDate"]),
				VatDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["VatDate"]),
				ReqDate = Function.ParseDateTimes(dt.Tables[0].Rows[0]["ReqDate"]),
				Lines = new List<Document_LinesImpl>()
			};
			DataTable dataTable = dt.Tables[1].Copy();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int num = Function.ParseInt(dataTable.Rows[i]["DocEntry"]);
				if (num == 0)
				{
					continue;
				}
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl
				{
					BaseEntry = Function.ParseInt(dataTable.Rows[i]["DocEntry"]),
					BaseLine = -1,
					Remark = Function.ToString(dataTable.Rows[i]["Barcode"]),
					BaseType = 202,
					Quantity = Function.ParseDouble(dataTable.Rows[i]["Quantity"]),
					WhsCode = Function.ToString(dataTable.Rows[i]["WhsCode"])
				};
				for (int j = i + 1; j < dataTable.Rows.Count; j++)
				{
					if (num == Function.ParseInt(dataTable.Rows[j]["DocEntry"]))
					{
						document_LinesImpl.Remark = document_LinesImpl.Remark + " - " + Function.ToString(dataTable.Rows[j]["Barcode"]);
						document_LinesImpl.Quantity += Function.ParseDouble(dataTable.Rows[j]["Quantity"]);
					}
					dataTable.Rows.RemoveAt(j);
				}
				documentsImpl.Lines.Add(document_LinesImpl);
			}
			string text = "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                               <SessionID>" + sessionId + "</SessionID>\r\n                          </env:Header>\r\n                          <env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(documentsImpl, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			string text2 = "";
			for (int k = 0; k < dt.Tables[1].Rows.Count; k++)
			{
				text2 += string.Format("Update apz_POBarCode set CompleteQTY = (isnull(CompleteQTY,0) + {0}) where Barcode = '{1}'   ", Function.ParseInt(dt.Tables[1].Rows[k]["Quantity"]), Function.ToString(dt.Tables[1].Rows[k]["Barcode"]));
			}
			text += _unitOfWork.UDOs.GetXmlExecuteSQL(text2, "", isGetBody: true);
			text += "</env:Body>";
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
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
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						return "";
					}
				}
			}
			return "Have some exception!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string MappingXml(DocumentsImpl issue, string sessionid)
	{
		return string.Empty;
	}

	public string Close(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Reopen(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Cancel(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Remove(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public bool Exists(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}
}
