using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ShippingRepository : Repository<DataSet>, IShipping
{
	private readonly IUnitOfWork _unitOfWork;

	private const string TableDoc = "APZ_SHIP";

	private const string TableLine = "APZ_SHIP1";

	public ShippingRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<HttpResult> Save(DataSet ds, string sessionId)
	{
		try
		{
			Logging.Write("TRACE", "Start - save shipping");
			int num = Function.ParseInt(ds.Tables[0].Rows[0]["DocEntry"]);
			HttpResult httpResult = ServiceUnitOfWork.Shipping.Save(ds);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			Logging.Write("TRACE", "Save shipping -OK-" + num);
			num = Function.ParseInt(httpResult.Content);
			DataTable dataTable = ExecuteDataTable("apz_shipping_get_transfer", CommandType.StoredProcedure, new SqlParameter("@DocEntry", num));
			if (dataTable.IsNotNull())
			{
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.DocDate = DateTime.Today;
				documentsImpl.Lines = new List<Document_LinesImpl>();
				documentsImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				var enumerable = from t in dataTable.AsEnumerable()
					group t by new
					{
						ItemCode = Function.ToString(t["ItemCode"]),
						Whs = Function.ToString(t["WhsCode"]),
						ToWhs = Function.ToString(t["OrderWhs"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					};
				int num2 = 0;
				foreach (var item in enumerable)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					document_LinesImpl.ItemCode = Function.ToString(item.Key.ItemCode);
					document_LinesImpl.FromWhsCod = Function.ToString(item.Key.Whs).ToUpper();
					if (string.IsNullOrEmpty(documentsImpl.Filler))
					{
						documentsImpl.Filler = document_LinesImpl.FromWhsCod;
					}
					document_LinesImpl.WhsCode = Function.ToString(item.Key.ToWhs).ToUpper();
					if (string.IsNullOrEmpty(documentsImpl.ToWhsCode))
					{
						documentsImpl.ToWhsCode = document_LinesImpl.WhsCode;
					}
					int num3 = 0;
					double num4 = 0.0;
					document_LinesImpl.SerialNumbers = new List<SerialNumbersImpl>();
					foreach (DataRow item2 in item.Items)
					{
						int num5 = Function.ParseInt(item2["BinAbs"]);
						int num6 = Function.ParseInt(item2["DftBinAbs"]);
						if ((num5 == 0 || num6 == 0 || !num5.Equals(num6)) && (num5 != 0 || num6 != 0 || !document_LinesImpl.FromWhsCod.Equals(documentsImpl.ToWhsCode)))
						{
							double num7 = Function.ParseDouble(item2["Quantity"]);
							num4 += num7;
							SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
							serialNumbersImpl.BatchID = Function.ToString(item2["LotNumber"]);
							serialNumbersImpl.InternalSerialNumber = Function.ToString(item2["DistNumber"]);
							serialNumbersImpl.SystemSerialNumber = Function.ParseInt(item2["SysNumber"]);
							serialNumbersImpl.Quantity = num7;
							serialNumbersImpl.BaseLineNumber = num2;
							document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(item2["BinAbs"]);
							documentLinesBinAllocationsImpl.BaseLineNumber = num2;
							documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
							documentLinesBinAllocationsImpl.Quantity = num7;
							documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num3;
							if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
							{
								documentsImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
							}
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BinAbsEntry = Function.ParseInt(item2["DftBinAbs"]);
							documentLinesBinAllocationsImpl2.BaseLineNumber = num2;
							documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
							documentLinesBinAllocationsImpl2.Quantity = num7;
							documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num3;
							if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
							{
								documentsImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
							}
							num3++;
						}
					}
					document_LinesImpl.Quantity = num4;
					document_LinesImpl.LineNum = num2;
					documentsImpl.Lines.Add(document_LinesImpl);
					num2++;
				}
				List<Document_LinesImpl> lines = documentsImpl.Lines;
				if (lines != null && lines.Count > 0 && !string.IsNullOrEmpty(_unitOfWork.StockTransfer.Save(documentsImpl, sessionId, isDraft: false).Message))
				{
					Logging.Write("TRACE", "could not transfer");
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
