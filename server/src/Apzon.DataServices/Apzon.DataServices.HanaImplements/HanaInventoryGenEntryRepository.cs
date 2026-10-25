using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaInventoryGenEntryRepository : InventoryGenEntryRepository
{
	public HanaInventoryGenEntryRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string AutoReportComplete(DataSet dt, string sessionId)
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
			_ = "<env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                               <SessionID>" + sessionId + "</SessionID>\r\n                          </env:Header>\r\n                          <env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(documentsImpl, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			return "Have some exception!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
