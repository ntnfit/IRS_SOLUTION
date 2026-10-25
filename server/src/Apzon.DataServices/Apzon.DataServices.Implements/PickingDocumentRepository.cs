using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.HDBConnection;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class PickingDocumentRepository : Repository<DocumentsImpl>, IPickingDocument, IRepository<DocumentsImpl>
{
	private const string TableDoc = "APZ_OPKL";

	private const string TableLine = "APZ_PKL1";

	private const string TableBatch = "APZ_PKL2";

	private readonly IUnitOfWork _unitOfWork;

	private readonly Apzon.Services.IUnitOfWork _servicesUnitOfWork;

	public PickingDocumentRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
		_servicesUnitOfWork = serviceUnitOfWork;
	}

	public HttpResult Save(DataSet ds, string sessionid)
	{
		try
		{
			using Apzon.HDBConnection.Transaction transaction = new Apzon.HDBConnection.Transaction(System.Transactions.IsolationLevel.RepeatableRead, 59.0);
			int num = Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"]);
			DataTable dataTable = ds.Tables["APZ_PKL2"];
			DataTable source = ds.Tables["APZ_PKL1"];
			DataTable removed = new DataTable();
			if (num > 0)
			{
				DataTable dataTable2 = ExecuteDataTable("select T0.\"ItemCode\",T0.\"MdAbsEntry\",T0.\"SysNumber\",T1.\"OrderEntry\",T1.\"OrderLine\",T0.\"Confirmed\",T0.\"Quantity\" from apz_pkl2 T0 inner join apz_pkl1 T1 on T0.\"AbsEntry\" =T1.\"AbsEntry\" AND T0.\"PickEntry\"=T1.\"PickEntry\" where T0.\"AbsEntry\"=@AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", num));
				if (dataTable2.IsNotNull())
				{
					removed = dataTable2.Clone();
					DataRow[] array = (from exists in dataTable2.AsEnumerable()
						join batch in dataTable.AsEnumerable() on new
						{
							ItemCode = Function.ToString(exists["ItemCode"]),
							MdAbsEntry = Function.ParseInt(exists["MdAbsEntry"])
						} equals new
						{
							ItemCode = Function.ToString(batch["ItemCode"]),
							MdAbsEntry = Function.ParseInt(batch["MdAbsEntry"])
						} into grp
						from item in grp.DefaultIfEmpty()
						where item == null
						select removed.LoadDataRow(new object[7]
						{
							Function.ToString(exists["ItemCode"]),
							Function.ParseInt(exists["MdAbsEntry"]),
							Function.ParseInt(exists["SysNumber"]),
							Function.ParseInt(exists["OrderEntry"]),
							Function.ParseInt(exists["OrderLine"]),
							Function.ToString(exists["Confirmed"]),
							Function.ParseDouble(exists["Quantity"])
						}, fAcceptChanges: true)).ToArray();
					if (array.Length != 0)
					{
						removed = array.CopyToDataTable();
					}
				}
			}
			HttpResult httpResult = ServiceUnitOfWork.Picking.Save(ds);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			foreach (var item in from t in source.AsEnumerable()
				group t by new
				{
					docentry = Function.ParseInt(t["OrderEntry"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				})
			{
				int num2 = Function.ParseInt(item.Key.docentry);
				int num3 = 0;
				string query = "select DocEntry from ordr where DocNum=@DocNum";
				DataTable dataTable3 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@DocNum", num2));
				if (dataTable3.IsNotNull())
				{
					num3 = Function.ParseInt(dataTable3.Rows[0]["DocEntry"]);
				}
				if (num3 <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "No matching record found.Order " + num2);
				}
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.Lines = new List<Document_LinesImpl>();
				documentsImpl.DocEntry = num3;
				documentsImpl.DocStatus = "O";
				foreach (DataRow item2 in item.Items)
				{
					int num4 = Function.ParseInt(item2["OrderLine"]);
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					document_LinesImpl.DocEntry = num2;
					document_LinesImpl.LineNum = num4;
					document_LinesImpl.LineStatus = "O";
					int num5 = Function.ParseInt(item2["PickEntry"]);
					DataRow[] array2 = dataTable.Select($"PickEntry= '{num5}'");
					if (array2.Length != 0)
					{
						string text = Function.ToString(item2["ManBatchSerial"]);
						if (text == "B")
						{
							document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
							DataRow[] array3 = array2;
							foreach (DataRow dataRow in array3)
							{
								BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
								batchNumbersImpl.BatchNumber = Function.ToString(dataRow["DistNumber"]);
								batchNumbersImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
								batchNumbersImpl.BaseLineNumber = num4;
								document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
							}
						}
						else if (text == "S")
						{
							document_LinesImpl.SerialNumbers = new List<SerialNumbersImpl>();
							DataRow[] array3 = array2;
							foreach (DataRow dataRow2 in array3)
							{
								SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
								serialNumbersImpl.InternalSerialNumber = Function.ToString(dataRow2["DistNumber"]);
								serialNumbersImpl.Quantity = Function.ParseDouble(dataRow2["Quantity"]);
								serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow2["SysNumber"]);
								serialNumbersImpl.BatchID = Function.ToString(dataRow2["LotNumber"]);
								serialNumbersImpl.ManufacturerSerialNumber = Function.ToString(dataRow2["MnfSerial"]);
								serialNumbersImpl.BaseLineNumber = num4;
								document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
							}
						}
					}
					documentsImpl.Lines.Add(document_LinesImpl);
				}
				if (dataTable.IsNotNull() && !documentsImpl.Lines.Any((Document_LinesImpl t) => (t.SerialNumbers != null && t.SerialNumbers.Count > 0) || (t.BatchNumbers != null && t.BatchNumbers.Count > 0)))
				{
					throw new Exception("Could not get data batch/serial");
				}
				if (documentsImpl.Lines.Count > 0)
				{
					HttpResult httpResult2 = _unitOfWork.Documents.UpdateDataByService(documentsImpl, sessionid, ApzObjectTypes.oOrders, ApzObjectTypes.oOrders, "", removed);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						throw new Exception(httpResult2.Message);
					}
					continue;
				}
				return new HttpResult(Enums.MessageCode.Error, "Contents is empty");
			}
			transaction.Commit();
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveList(List<DataSet> model, string sessionId)
	{
		try
		{
			return _servicesUnitOfWork.Picking.SaveList(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Cancel(int absEntry, string sessionId)
	{
		try
		{
			if (absEntry > 0)
			{
				HttpResult httpResult = ServiceUnitOfWork.Picking.Cancel(absEntry);
				string.IsNullOrEmpty(httpResult.Message);
				return httpResult;
			}
			return new HttpResult(Enums.MessageCode.Error, Messages.D404);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
