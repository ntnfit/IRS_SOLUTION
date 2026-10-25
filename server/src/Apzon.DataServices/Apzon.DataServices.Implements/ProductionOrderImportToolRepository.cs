using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ProductionOrderImportToolRepository : Repository<DataTable>, IProductionOrderImportTool, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	private readonly string commentsRcReceipt = "Imp Tool PC: Receipt RC";

	private readonly string commentsRcIssue = "Imp Tool PC: Issue RC";

	private readonly string commentsRcReceiptFor = "Imp Tool PC: Receipt for Issue RC";

	private readonly string commentsRcReceiptPDA = "Imp Tool PDA: Receipt RC";

	private readonly string commentsRcIssuePDA = "Imp Tool PDA: Issue RC";

	private readonly string commentsRcReceiptForPDA = "Imp Tool PDA: Receipt for Issue RC";

	public ProductionOrderImportToolRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<HttpResult> SaveFG(DataTable dt, string sessionId)
	{
		Logging.Write("TRACE", "*** Import Prd Order: Start import Production Order");
		try
		{
			HttpResult fgDocToSave = GetFgDocToSave(dt);
			if (string.IsNullOrEmpty(fgDocToSave.Message))
			{
				List<Tuple<DataTable, DataTable>> lstDocs = fgDocToSave.Content as List<Tuple<DataTable, DataTable>>;
				HttpResult httpResult = await _unitOfWork.ProductionOrders.SaveDocsPrdImportTools(lstDocs, sessionId);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("TRACE", "*** Import Prd Order: End import Production Order");
				}
				return httpResult;
			}
			Logging.Write("TRACE", "*** Import Prd Order: End import Production Order");
			return fgDocToSave;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			Logging.Write("TRACE", "*** Import Prd Order: End import Production Order");
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<string> SaveFGSerial(DataSet ds, string sessionId)
	{
		Logging.Write("TRACE", "*** Import Prd Order: Start import Production Order serial");
		try
		{
			DataTable dtDocs = ds.Tables[0];
			List<int> lstSerial = new List<int>();
			for (int i = 0; i < dtDocs.Rows.Count; i++)
			{
				lstSerial.Add(Function.ParseInt(dtDocs.Rows[i]["DocEntry"]));
			}
			HttpResult fgDocToSave = GetFgDocToSave(dtDocs);
			if (string.IsNullOrEmpty(fgDocToSave.Message))
			{
				List<Tuple<DataTable, DataTable>> lstDocs = fgDocToSave.Content as List<Tuple<DataTable, DataTable>>;
				HttpResult httpResult = await _unitOfWork.ProductionOrders.SaveList(lstDocs, sessionId);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("TRACE", "*** Import Prd Order: End import Production Order serial");
					return httpResult.Message;
				}
				Logging.Write("TRACE", "*** Import Prd Order: Imported Prd Order serial: " + httpResult.Content.ToString());
				List<ProductionOder_Impl_OWOR> list = new List<ProductionOder_Impl_OWOR>();
				string[] array = httpResult.Content.ToString().Split(',');
				for (int j = 0; j < dtDocs.Rows.Count; j++)
				{
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
					productionOder_Impl_OWOR.DocEntry = Function.ParseInt(array[j]);
					productionOder_Impl_OWOR.CloseDate = Function.ParseDateTime(dtDocs.Rows[j]["CloseDate"]);
					productionOder_Impl_OWOR.Status = "R";
					list.Add(productionOder_Impl_OWOR);
				}
				string message = _unitOfWork.ProductionOrders.UpdateStatus(list, sessionId).Message;
				if (string.IsNullOrEmpty(message))
				{
					for (int k = 0; k < dtDocs.Rows.Count; k++)
					{
						string text = UpdateSerials(ds.Tables[1], Function.ParseInt(array[k]), lstSerial[k]);
						if (!string.IsNullOrEmpty(text))
						{
							Logging.Write("ERROR", "SaveFGSerial()->UpdateSerials(): " + text);
						}
					}
				}
				Logging.Write("TRACE", "*** Import Prd Order: End import Production Order serial");
				return message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			Logging.Write("TRACE", "*** Import Prd Order: End import Production Order serial");
			return ex.Message;
		}
	}

	private HttpResult GetFgDocToSave(DataTable dt)
	{
		try
		{
			List<Tuple<DataTable, DataTable>> list = new List<Tuple<DataTable, DataTable>>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = Function.ToString(dt.Rows[i]["ItemCode"]);
				if (!list2.Contains(text.ToLower()))
				{
					list2.Add(text.ToLower());
				}
			}
			string bomCodes = string.Join(",", list2);
			DataTable prdOrdrLineFromBOM = GetPrdOrdrLineFromBOM(bomCodes);
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				DataTable dataTable = new DataTable();
				dt.Rows[j]["Status"] = "P";
				dt.Rows[j]["DocEntry"] = 0;
				string itemCode = Function.ToString(dt.Rows[j]["ItemCode"]);
				double num = Function.ParseDouble(dt.Rows[j]["PlannedQty"]);
				dataTable = (from r in prdOrdrLineFromBOM.AsEnumerable()
					where Function.ToString(r["Father"]).ToLower() == itemCode.ToLower()
					select r).CopyToDataTable();
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Get no components for Item '" + itemCode + "'");
				}
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					dataTable.Rows[num2]["PlannedQty"] = Function.ParseDouble(dataTable.Rows[num2]["BaseQty"]) * num;
					dataTable.Rows[num2]["IssueType"] = "M";
				}
				dataTable.AcceptChanges();
				DataTable dataTable2 = dt.Clone();
				dataTable2.ImportRow(dt.Rows[j]);
				dataTable2.AcceptChanges();
				list.Add(new Tuple<DataTable, DataTable>(dataTable2, dataTable));
			}
			return new HttpResult(Enums.MessageCode.Success, "", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string UpdateSerials(DataTable dtSerial, int docEntry, int serialId)
	{
		try
		{
			int serialEntrByDocEntry = GetSerialEntrByDocEntry(docEntry);
			List<DataRow> list = (from r in dtSerial.AsEnumerable()
				where Function.ParseInt(r["ID"]) == serialId
				select r).ToList();
			if (serialEntrByDocEntry > 0 && list.Count > 0)
			{
				List<SqlParameter> list2 = new List<SqlParameter>();
				string text = "Delete [@SERIAL_1] where DocEntry=" + serialEntrByDocEntry + "\r\n                        Insert into [@SERIAL_1] ([DocEntry]\r\n           ,[LineId]\r\n           ,[VisOrder]\r\n           ,[Object]\r\n           ,[LogInst]\r\n           ,[U_SerialNo]) Values ";
				for (int num = 0; num < list.Count; num++)
				{
					text = text + Environment.NewLine + "(" + serialEntrByDocEntry + "," + (num + 1) + "," + (num + 1) + ",'SERIAL',null,";
					text = text + "@serial" + num + "),";
					list2.Add(new SqlParameter("@serial" + num, Function.ToString(list[num]["Serial"])));
				}
				if (text.EndsWith(","))
				{
					text = text.Remove(text.Length - 1);
				}
				ExecuteNonQuery(text, CommandType.Text, list2.ToArray());
				return "";
			}
			return "SaveFGSerial()->UpdateSerials(): Cannot get serial entry or no serial found";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private int GetSerialEntrByDocEntry(int docEntry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select U_SerialNo From OWOR Where docentry=@docEntry", CommandType.Text, new SqlParameter("@docEntry", docEntry));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return 0;
	}

	private DataTable GetProductionOrderLineFromBOM(string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select * From ITT1 Where Father=@itemCode", CommandType.Text, new SqlParameter("@itemCode", itemCode));
			if (dataTable.IsNotNull())
			{
				DataTable tableStructure = _unitOfWork.Common.GetTableStructure("WOR1");
				if (tableStructure != null && tableStructure.Columns.Count > 0)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = tableStructure.NewRow();
						dataRow["DocEntry"] = 0;
						dataRow["LineNum"] = i;
						dataRow["ItemCode"] = Function.ToString(dataTable.Rows[i]["Code"]);
						dataRow["BaseQty"] = Function.ParseDouble(dataTable.Rows[i]["Quantity"]);
						dataRow["PlannedQty"] = Function.ParseDouble(dataTable.Rows[i]["Quantity"]);
						dataRow["IssuedQty"] = 0;
						dataRow["wareHouse"] = Function.ToString(dataTable.Rows[i]["Warehouse"]);
						dataRow["Project"] = Function.ToString(dataTable.Rows[i]["Project"]);
						dataRow["ItemType"] = 4;
						SetUDFValuesFromBOMLine(dataTable.Rows[i], dataRow);
						tableStructure.Rows.Add(dataRow);
					}
					tableStructure.AcceptChanges();
					return tableStructure;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	private DataTable GetPrdOrdrLineFromBOM(string bomCodes)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetPrdOrdrLineFromBOM", CommandType.StoredProcedure, new SqlParameter("@bomCodes", bomCodes));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	private void SetUDFValuesFromBOMLine(DataRow source, DataRow dest)
	{
		try
		{
			for (int i = 0; i < source.Table.Columns.Count; i++)
			{
				DataColumn dataColumn = source.Table.Columns[i];
				if (dest.Table.Columns.Contains(dataColumn.ColumnName) && dest.Table.Columns[dataColumn.ColumnName].DataType.Equals(dataColumn.DataType))
				{
					DataColumn dataColumn2 = dest.Table.Columns[dataColumn.ColumnName];
					dest[dataColumn2] = Function.GetValueByType(dataColumn2.DataType.Name.ToLower(), source[i]);
				}
			}
			dest.Table.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public HttpResult SaveTempFG(DataTable dt, string sessionId, string user)
	{
		try
		{
			string text = "";
			DataTable dataTable = ExecuteDataTable("Select top 1 userid from ousr where USER_CODE=@user", CommandType.Text, new SqlParameter("@user", user));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot get user id.");
			}
			using (Transaction transaction = new Transaction())
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					text = SaveATempFG(dt.Rows[i], Function.ParseInt(dataTable.Rows[0][0]));
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					return new HttpResult(Enums.MessageCode.Error, text);
				}
				transaction.Commit();
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string SaveATempFG(DataRow row, int userSign)
	{
		string text = Function.ToString(row["U_LotNo"]);
		string text2 = Function.ToString(row["U_PlnCode"]);
		string text3 = Function.ToString(row["ItemCode"]);
		try
		{
			DataTable dataTable = ExecuteDataTable("select 1 from oitm a inner join oitt b on a.itemcode=b.code where a.itemcode = @itemCode and b.TreeType = 'P'", CommandType.Text, new SqlParameter("@itemCode", text3));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return "ItemCode: '" + text3 + "' is invalid or not a BOM or not a Production BOM";
			}
			if (!ExistsTempFG(text2, text, text3))
			{
				string query = "insert into [APZ_IM_OWOR] \r\n                                (ItemCode,\r\n                                PlannedQty,\r\n                                Warehouse,\r\n                                StDueDate,\r\n                                StPostDate,\r\n                                U_PlnCode,\r\n                                U_LotNo,\r\n                                U_SeriesNo,\r\n                                U_SerialNo,\r\n                                U_OriginType,\r\n                                U_OriginCode,\r\n                                U_OriginLine,\r\n                                CreateDate,\r\n                                UserSign) \r\n\r\n                                values (@ItemCode,\r\n                                @PlannedQty,\r\n                                @Warehouse,\r\n                                @StDueDate,\r\n                                @StPostDate,\r\n                                @U_PlnCode,\r\n                                @U_LotNo,\r\n                                @U_SeriesNo,\r\n                                @U_SerialNo,\r\n                                @U_OriginType,\r\n                                @U_OriginCode,\r\n                                @U_OriginLine,\r\n                                @CreateDate,\r\n                                @UserSign\r\n                                )";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ItemCode", Function.ToString(row["ItemCode"])));
				list.Add(new SqlParameter("@PlannedQty", Function.ParseDecimal(row["PlannedQty"])));
				list.Add(new SqlParameter("@Warehouse", Function.ToString(row["Warehouse"])));
				list.Add(new SqlParameter("@StDueDate", Function.ToString(row["StDueDate"])));
				list.Add(new SqlParameter("@StPostDate", Function.ToString(row["StPostDate"])));
				list.Add(new SqlParameter("@U_PlnCode", text2));
				list.Add(new SqlParameter("@U_LotNo", text));
				list.Add(new SqlParameter("@U_SeriesNo", Function.ToString(row["U_SeriesNo"])));
				list.Add(new SqlParameter("@U_SerialNo", Function.ParseInt(row["U_SerialNo"])));
				list.Add(new SqlParameter("@U_OriginType", Function.ToString(row["U_OriginType"])));
				list.Add(new SqlParameter("@U_OriginCode", Function.ParseInt(row["U_OriginCode"])));
				list.Add(new SqlParameter("@U_OriginLine", Function.ParseInt(row["U_OriginLine"])));
				list.Add(new SqlParameter("@CreateDate", DateTime.Today));
				list.Add(new SqlParameter("@UserSign", userSign));
				if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) == 1)
				{
					DataTable dataTable2 = ExecuteDataTable("Select Max(docentry) from APZ_IM_OWOR", CommandType.Text);
					if (dataTable2 != null && dataTable2.Rows.Count == 1)
					{
						int num = Function.ParseInt(dataTable2.Rows[0][0]);
						ExecuteNonQuery("APZ_ProductionOrderImportTool_SerialGenerate", CommandType.StoredProcedure, new SqlParameter("@EntryID", num));
						return "";
					}
					return "(" + text3 + "," + text2 + "," + text + "):Cannot get max DocEntry from temp table";
				}
				return "(" + text3 + "," + text2 + "," + text + "):Error occurred, no rows inserted";
			}
			return "(" + text3 + "," + text2 + "," + text + ") existsted.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "(" + text3 + "," + text2 + "," + text + "):" + ex.Message;
		}
	}

	private bool ExistsTempFG(string plnCode, string lotNo, string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select top 1 1 from owor where u_plnCode=@plnCode and u_lotno=@lotNo and itemCode=@itemCode union all select top 1 1 from apz_im_owor where u_plnCode=@plnCode and u_lotno=@lotNo and itemCode=@itemCode", CommandType.Text, new SqlParameter("@plnCode", plnCode ?? ""), new SqlParameter("@lotNo", lotNo ?? ""), new SqlParameter("@itemCode", itemCode ?? ""));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return false;
		}
	}

	public async Task<string> SaveComp(DataTable dt, string sessionId)
	{
		try
		{
			new List<int>();
			DataTable tableStructure = _unitOfWork.Common.GetTableStructure("WOR1");
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string lotNo = Function.ToString(dt.Rows[i]["U_LotNo"]);
				string planCode = Function.ToString(dt.Rows[i]["U_PlnCode"]);
				string line = Function.ToString(dt.Rows[i]["U_ProductionLine"]);
				DataTable productionOrderLineByLotNoLine = GetProductionOrderLineByLotNoLine(planCode, lotNo, line);
				if (!productionOrderLineByLotNoLine.IsNotNull())
				{
					continue;
				}
				for (int j = 0; j < productionOrderLineByLotNoLine.Rows.Count; j++)
				{
					DateTime? dateTime = Function.ParseDateTime(dt.Rows[i]["U_Date"]);
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						productionOrderLineByLotNoLine.Rows[j]["U_Date"] = dateTime.Value;
					}
					else
					{
						productionOrderLineByLotNoLine.Rows[j]["U_Date"] = DBNull.Value;
					}
					productionOrderLineByLotNoLine.Rows[j]["U_Shift"] = Function.ToString(dt.Rows[i]["U_Shift"]);
					productionOrderLineByLotNoLine.Rows[j]["U_Time"] = Function.ParseSAPHour(dt.Rows[i]["U_Time"]);
					productionOrderLineByLotNoLine.AcceptChanges();
					tableStructure.ImportRow(productionOrderLineByLotNoLine.Rows[j]);
				}
				tableStructure.AcceptChanges();
			}
			using (Transaction transaction = new Transaction())
			{
				string text = "";
				for (int k = 0; k < tableStructure.Rows.Count; k++)
				{
					text = UpdateTimeToLine(tableStructure.Rows[k]);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				transaction.Commit();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private string UpdateTimeToLine(DataRow row)
	{
		try
		{
			string query = "UPDATE WOR1 set     \r\n                                U_Date=@U_Date\r\n                                ,U_Shift=@U_Shift\r\n                                ,U_Time=@U_Time\r\n                            Where LineNum=@lineNum and DocEntry=@docEntry";
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@U_Date", Function.ParseDateTime(row["U_Date"])), new SqlParameter("@U_Shift", Function.ToString(row["U_Shift"])), new SqlParameter("@U_Time", Function.ParseInt(row["U_Time"])), new SqlParameter("@lineNum", Function.ParseInt(row["LineNum"])), new SqlParameter("@docEntry", Function.ParseInt(row["DocEntry"])));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private DataTable GetProductionOrderLineByLotNoLine(string planCode, string lotNo, string line)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[3]
			{
				new SqlParameter("@planCode", planCode),
				new SqlParameter("@lotNo", lotNo),
				new SqlParameter("@line", line)
			};
			string query = "select a.*  from WOR1 a inner join OWOR b on a.DocEntry=b.DocEntry \r\n                                where b.U_LotNo=@lotNo and a.U_ProductionLine=@line and b.U_PlnCode=@planCode";
			return ExecuteDataTable(query, CommandType.Text, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return null;
		}
	}

	private DataTable GetProductionOrderByDocEntry(int docEntry)
	{
		try
		{
			return ExecuteDataTable("select * from OWOR where docentry=@docEntry", CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return null;
		}
	}

	public string SaveTransfer(DataTable dt, string sessionId)
	{
		try
		{
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = 0;
			DataTable tableStructure = _unitOfWork.Common.GetTableStructure("WTR1");
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["DocEntry"] = 0;
				dataRow2["ItemCode"] = Function.ToString(dataRow["ItemCode"]);
				dataRow2["LineNum"] = i;
				dataRow2["LineStatus"] = "O";
				dataRow2["Dscription"] = Function.ToString(dataRow["ItemName"]);
				dataRow2["Quantity"] = Function.ParseDouble(dataRow["RequestQty"]);
				dataRow2["FromWhsCod"] = Function.ToString(dataRow["DfltWH"]);
				dataRow2["WhsCode"] = Function.ToString(dataRow["wareHouse"]);
				dataRow2["AcctCode"] = "";
				dataRow2["U_PlnCode"] = Function.ToString(dataRow["U_PlnCode"]);
				dataRow2["U_LotNo"] = Function.ToString(dataRow["U_LotNo"]);
				dataRow2["U_ProductionLine"] = Function.ToString(dataRow["U_ProductionLine"]);
				tableStructure.Rows.Add(dataRow2);
			}
			documentsImpl.Lines = tableStructure.ToDocumentLine();
			return _unitOfWork.StockTransfer.Save(documentsImpl, sessionId, isDraft: false).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public string SaveRC(DataTable dt, string sessionId)
	{
		string text = "ProductionOrderImportToolRepository->SaveRC():";
		Logging.Write("TRACE", "[Start add Receipt complete from PDA]");
		try
		{
			List<UserFieldsImpl> listField = _unitOfWork.UserFields.GetListField("IGE1", sessionId);
			List<UserFieldsImpl> listField2 = _unitOfWork.UserFields.GetListField("IGN1", sessionId);
			DataTable tableStructure = _unitOfWork.Common.GetTableStructure("IGN1");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["LineNum"] = 0;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			DataTable tableStructure2 = _unitOfWork.Common.GetTableStructure("IGE1");
			DataRow dataRow2 = tableStructure2.NewRow();
			dataRow2["DocEntry"] = 0;
			dataRow2["LineNum"] = 0;
			tableStructure2.Rows.Add(dataRow2);
			tableStructure2.AcceptChanges();
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocType = "I";
			documentsImpl.ObjType = 59;
			documentsImpl.DocDate = DateTime.Today;
			documentsImpl.Comments = commentsRcReceiptPDA;
			documentsImpl.JrnlMemo = commentsRcReceiptPDA;
			DocumentsImpl documentsImpl2 = new DocumentsImpl();
			documentsImpl2.DocType = "I";
			documentsImpl2.ObjType = 60;
			documentsImpl2.DocDate = DateTime.Today;
			documentsImpl2.Comments = commentsRcIssuePDA;
			documentsImpl2.JrnlMemo = commentsRcIssuePDA;
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			List<Document_LinesImpl> list2 = new List<Document_LinesImpl>();
			int num = 0;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Quantity", typeof(double)));
			dataTable.AcceptChanges();
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable2.AcceptChanges();
			DataTable DTusedSerial = new DataTable();
			DTusedSerial.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			DTusedSerial.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			DTusedSerial.AcceptChanges();
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable3.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable3.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable3.Columns.Add(new DataColumn("Quantity", typeof(double)));
			dataTable3.Columns.Add(new DataColumn("BinAbs", typeof(int)));
			dataTable3.AcceptChanges();
			List<int> lstDocEntry = new List<int>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int entry = Function.ParseInt(dt.Rows[i]["DocEntry"]);
				if (!lstDocEntry.Any((int e) => e == entry))
				{
					lstDocEntry.Add(entry);
				}
			}
			DataTable tableStructureQuantityDocEntry = GetTableStructureQuantityDocEntry();
			int i2;
			for (i2 = 0; i2 < lstDocEntry.Count; i2++)
			{
				new List<DataRow>();
				EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
					where Function.ParseInt(r["DocEntry"]) == lstDocEntry[i2]
					select r;
				if (!source.Any())
				{
					return "No row to save. DocEntry:" + lstDocEntry[i2];
				}
				List<DataRow> source2 = source.ToList();
				double num2 = source2.Where((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]));
				double num3 = source2.Where((DataRow r) => Function.ParseDouble(r["RjQuantity"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["RjQuantity"]));
				DataRow dataRow3 = tableStructureQuantityDocEntry.NewRow();
				dataRow3["DocEntry"] = lstDocEntry[i2];
				dataRow3["Quantity"] = num2 + num3;
				tableStructureQuantityDocEntry.Rows.Add(dataRow3);
			}
			tableStructureQuantityDocEntry.AcceptChanges();
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add("ItemCode", typeof(string));
			dataTable4.Columns.Add("WhsCode", typeof(string));
			dataTable4.Columns.Add("ManBatchSer", typeof(string));
			dataTable4.Columns.Add("BnSDistNum", typeof(string));
			dataTable4.Columns.Add("Quantity", typeof(string));
			dataTable4.Columns.Add("BinAbs", typeof(int));
			dataTable4.AcceptChanges();
			DocumentsImpl doc = new DocumentsImpl();
			doc.DocType = "I";
			doc.ObjType = 59;
			doc.DocDate = DateTime.Today;
			doc.Comments = commentsRcReceiptForPDA;
			doc.JrnlMemo = commentsRcReceiptForPDA;
			string text2 = AddReceiptLineForIssueQuantity(tableStructureQuantityDocEntry, ref doc, dataTable4);
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
			string binCode = Function.ToString(dt.Rows[0]["BinCode"]);
			string wareHouse = Function.ToString(dt.Rows[0]["Warehouse"]);
			int binAbs = -1;
			DataTable whs = GetWhs(binCode);
			if (whs.IsNotNull())
			{
				wareHouse = Function.ToString(whs.Rows[0]["WhsCode"]);
				binAbs = Function.ParseInt(whs.Rows[0]["AbsEntry"]);
			}
			int num4 = 0;
			for (int num5 = 0; num5 < lstDocEntry.Count; num5++)
			{
				int docEntry = lstDocEntry[num5];
				List<DataRow> list3 = new List<DataRow>();
				EnumerableRowCollection<DataRow> source3 = from r in dt.AsEnumerable()
					where Function.ParseInt(r["DocEntry"]) == docEntry
					select r;
				if (!source3.Any())
				{
					return "No row to save. DocEntry:" + docEntry;
				}
				list3 = source3.ToList();
				DataRow dataRow4 = list3[0];
				int num6 = Function.ParseInt(dataRow4["DocNum"]);
				string acctCode = Function.ToString(dataRow4["AcctCode"]);
				double num7 = list3.Where((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]));
				double num8 = list3.Where((DataRow r) => Function.ParseDouble(r["RjQuantity"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["RjQuantity"]));
				double num9 = num7 + num8;
				string text3 = Function.ToString(dataRow4["U_LotNo"]);
				string text4 = Function.ToString(dataRow4["U_PlnCode"]);
				string docItemCode = Function.ToString(dataRow4["ItemCode"]);
				if (num9 <= 0.0)
				{
					return "Quantity is missing";
				}
				if (num7 > 0.0)
				{
					IEnumerable<DataRow> source4 = list3.Where((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]) > 0.0);
					List<DataRow> lstRow = new List<DataRow>();
					if (source4.Any())
					{
						lstRow = source4.ToList();
					}
					Tuple<string, Document_LinesImpl> receiptLineForPDA = GetReceiptLineForPDA(num, num6, docEntry, acctCode, wareHouse, text3, text4, dataRow, listField2, sessionId, num7, docItemCode, ref DTusedSerial, binAbs, lstRow);
					if (!string.IsNullOrEmpty(receiptLineForPDA.Item1))
					{
						return receiptLineForPDA.Item1;
					}
					receiptLineForPDA.Item2.TranType = "C";
					list.Add(receiptLineForPDA.Item2);
					num++;
				}
				if (num8 > 0.0)
				{
					IEnumerable<DataRow> source5 = list3.Where((DataRow r) => Function.ParseDouble(r["RjQuantity"]) > 0.0);
					List<DataRow> lstRow2 = new List<DataRow>();
					if (source5.Any())
					{
						lstRow2 = source5.ToList();
					}
					Tuple<string, Document_LinesImpl> receiptLineForPDA2 = GetReceiptLineForPDA(num, num6, docEntry, acctCode, wareHouse, text3, text4, dataRow, listField2, sessionId, num8, docItemCode, ref DTusedSerial, binAbs, lstRow2);
					if (!string.IsNullOrEmpty(receiptLineForPDA2.Item1))
					{
						return receiptLineForPDA2.Item1;
					}
					receiptLineForPDA2.Item2.TranType = "R";
					list.Add(receiptLineForPDA2.Item2);
					num++;
				}
				DataTable manualProductionOrderLine = GetManualProductionOrderLine(docEntry);
				if (!manualProductionOrderLine.IsNotNull())
				{
					continue;
				}
				for (int num10 = 0; num10 < manualProductionOrderLine.Rows.Count; num10++)
				{
					double num11 = Function.ParseDouble(manualProductionOrderLine.Rows[num10]["BaseQty"]);
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					document_LinesImpl.LineNum = num4;
					document_LinesImpl.BaseRef = num6;
					document_LinesImpl.BaseType = 202;
					document_LinesImpl.BaseEntry = docEntry;
					document_LinesImpl.BaseLine = Function.ParseInt(manualProductionOrderLine.Rows[num10]["LineNum"]);
					document_LinesImpl.WhsCode = Function.ToString(manualProductionOrderLine.Rows[num10]["wareHouse"]);
					document_LinesImpl.AcctCode = acctCode;
					document_LinesImpl.Quantity = Function.ParseDouble((decimal)num11 * (decimal)num9);
					if (!(document_LinesImpl.Quantity > 0.0))
					{
						continue;
					}
					dataRow2["U_LotNo"] = text3;
					dataRow2["U_PlnCode"] = text4;
					dataRow2["U_ProductionLine"] = Function.ToString(manualProductionOrderLine.Rows[num10]["U_ProductionLine"]);
					DateTime? dateTime = Function.ParseDateTime(manualProductionOrderLine.Rows[num10]["U_Date"]);
					DateTime dateTime2;
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						dateTime2 = dateTime.Value;
					}
					else
					{
						dateTime2 = new DateTime(1900, 1, 1);
					}
					dataRow2["U_Date"] = dateTime2;
					dataRow2["U_Shift"] = Function.ToString(manualProductionOrderLine.Rows[num10]["U_Shift"]);
					dataRow2["U_Time"] = Function.ParseSAPHour(manualProductionOrderLine.Rows[num10]["U_Time"]);
					dataRow2.AcceptChanges();
					document_LinesImpl.UserFields = CommonRepository.DataRowToUserFields(dataRow2, listField, sessionId, _unitOfWork);
					string itemCode = Function.ToString(manualProductionOrderLine.Rows[num10]["ItemCode"]);
					string[] array = GetBatchSerialString(itemCode).Split(',');
					string text5 = array[0];
					string text6 = array[1];
					List<DocumentLinesBinAllocationsImpl> binAllocs = new List<DocumentLinesBinAllocationsImpl>();
					if (text5 == "Y")
					{
						Tuple<string, List<BatchNumbersImpl>> batchQuantityForIssue = GetBatchQuantityForIssue(itemCode, document_LinesImpl.Quantity, document_LinesImpl.WhsCode, num4, dataTable, dataTable4);
						if (batchQuantityForIssue.Item1 != "")
						{
							return batchQuantityForIssue.Item1;
						}
						document_LinesImpl.BatchNumbers = batchQuantityForIssue.Item2;
						if (IsBinWarehouse(document_LinesImpl.WhsCode))
						{
							GetBinAllocsByBatch(batchQuantityForIssue.Item2, itemCode, document_LinesImpl.WhsCode, ref binAllocs, dataTable3);
						}
					}
					else if (text6 == "Y")
					{
						Tuple<string, List<SerialNumbersImpl>> serialQuantityForIssue = GetSerialQuantityForIssue(itemCode, document_LinesImpl.Quantity, document_LinesImpl.WhsCode, num4, dataTable4, dataTable2);
						if (serialQuantityForIssue.Item1 != "")
						{
							return serialQuantityForIssue.Item1;
						}
						document_LinesImpl.SerialNumbers = serialQuantityForIssue.Item2;
						IsBinWarehouse(document_LinesImpl.WhsCode);
					}
					list2.Add(document_LinesImpl);
					num4++;
				}
			}
			documentsImpl.Lines = list;
			List<DocumentsImpl> list4 = new List<DocumentsImpl>();
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				doc.ObjType = 59;
				list4.Add(doc);
			}
			if (list2.Count > 0)
			{
				documentsImpl2.ObjType = 60;
				documentsImpl2.Lines = list2;
				list4.Add(documentsImpl2);
			}
			documentsImpl.ObjType = 59;
			list4.Add(documentsImpl);
			for (int num12 = 0; num12 < list4.Count; num12++)
			{
				if (list4[num12].Lines == null || list4[num12].Lines.Count == 0)
				{
					string text7 = "";
					switch (list4[num12].ObjType)
					{
					case 59:
						text7 = "Receipt";
						break;
					case 60:
						text7 = "Issue";
						break;
					}
					return text + text7 + " Document have now lines";
				}
			}
			if (!ValidWarehouse(documentsImpl.Lines[0].WhsCode))
			{
				return text + " [OIGN.ToWhsCode] '" + documentsImpl.ToWhsCode + "' is invalid warehouse";
			}
			HttpResult httpResult = _unitOfWork.Documents.AddListDocument(list4, sessionId);
			if (httpResult != null)
			{
				Logging.Write("TRACE", "***[End add Receipt complete from PDA]");
				return httpResult.Message;
			}
			Logging.Write("TRACE", "***[End add Receipt complete from PDA]");
			return text + " Null HttpResult from AddListDocument()";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			Logging.Write("TRACE", "***[End add Receipt complete from PDA]");
			return ex.Message;
		}
	}

	private bool ValidWarehouse(string toWhsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select top 1 1 From OWHS where (isnull(Locked,'N')) <> 'Y' AND (isnull(Nettable,'N')) = 'Y' AND  whscode=@whsCode", CommandType.Text, new SqlParameter("@whsCode", toWhsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return false;
		}
	}

	public string SaveRCFromPC(DataTable dt, string sessionId)
	{
		try
		{
			List<DocumentsImpl> lstDoc = new List<DocumentsImpl>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				DateTime? dateTime = Function.ParseDateTime(dataRow["PostingDate"]);
				dataRow["PostingDate"] = ((!dateTime.HasValue) ? DateTime.Today : dateTime.Value.Date);
			}
			dt.AcceptChanges();
			string text = AddDocToList(ref lstDoc, dt, sessionId);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return _unitOfWork.Documents.AddListDocument(lstDoc, sessionId).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private string AddDocToList(ref List<DocumentsImpl> lstDoc, DataTable dt, string sessionId)
	{
		try
		{
			List<UserFieldsImpl> listField = _unitOfWork.UserFields.GetListField("IGN1", sessionId);
			DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["PostingDate"]);
			DataTable tableStructure = _unitOfWork.Common.GetTableStructure("IGN1");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["LineNum"] = 0;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocType = "I";
			documentsImpl.ObjType = 59;
			documentsImpl.DocDate = ((!dateTime.HasValue) ? DateTime.Today : dateTime.Value);
			documentsImpl.Comments = commentsRcReceipt;
			documentsImpl.JrnlMemo = commentsRcReceipt;
			documentsImpl.BPLId = Function.ParseInt(dt.Rows[0]["BPLId"]);
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Quantity", typeof(double)));
			dataTable.AcceptChanges();
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("Quantity", typeof(double)));
			dataTable2.AcceptChanges();
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable3.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dataTable3.Columns.Add(new DataColumn("DistNumber", typeof(string)));
			dataTable3.AcceptChanges();
			int num = 0;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int docEntry = Function.ParseInt(dt.Rows[i]["DocEntry"]);
				int docNum = Function.ParseInt(dt.Rows[i]["DocNum"]);
				string acctCode = Function.ToString(dt.Rows[i]["AcctCode"]);
				double num2 = Function.ParseDouble(dt.Rows[i]["CmpQuantiy"]);
				double num3 = Function.ParseDouble(dt.Rows[i]["RjQuantity"]);
				string lotNo = Function.ToString(dt.Rows[i]["U_LotNo"]);
				string plnCode = Function.ToString(dt.Rows[i]["U_PlnCode"]);
				string docItemCode = Function.ToString(dt.Rows[i]["ItemCode"]);
				string wareHouse = Function.ToString(dt.Rows[i]["Warehouse"]);
				int usedSerialCount = 0;
				if (num2 > 0.0)
				{
					Tuple<string, Document_LinesImpl> receiptLineForPC = GetReceiptLineForPC(num, docNum, docEntry, acctCode, wareHouse, lotNo, plnCode, dataRow, sessionId, num2, docItemCode, ref usedSerialCount, listField);
					if (!string.IsNullOrEmpty(receiptLineForPC.Item1))
					{
						return receiptLineForPC.Item1;
					}
					receiptLineForPC.Item2.TranType = "C";
					list.Add(receiptLineForPC.Item2);
					num++;
				}
				if (num3 > 0.0)
				{
					Tuple<string, Document_LinesImpl> receiptLineForPC2 = GetReceiptLineForPC(num, docNum, docEntry, acctCode, wareHouse, lotNo, plnCode, dataRow, sessionId, num3, docItemCode, ref usedSerialCount, listField);
					if (!string.IsNullOrEmpty(receiptLineForPC2.Item1))
					{
						return receiptLineForPC2.Item1;
					}
					receiptLineForPC2.Item2.TranType = "R";
					list.Add(receiptLineForPC2.Item2);
					num++;
				}
			}
			documentsImpl.Lines = list;
			lstDoc.Add(documentsImpl);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private Tuple<string, Document_LinesImpl> GetReceiptLineForPDA(int lineNum, int docNum, int docEntry, string acctCode, string wareHouse, string lotNo, string plnCode, DataRow rowIG1, List<UserFieldsImpl> lstUdfIGE1, string sessionId, double qty, string docItemCode, ref DataTable DTusedSerial, int binAbs, List<DataRow> lstRow = null)
	{
		try
		{
			Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
			document_LinesImpl.LineNum = lineNum;
			document_LinesImpl.BaseRef = docNum;
			document_LinesImpl.BaseType = 202;
			document_LinesImpl.BaseEntry = docEntry;
			document_LinesImpl.BaseLine = -1;
			document_LinesImpl.AcctCode = acctCode;
			document_LinesImpl.WhsCode = wareHouse;
			rowIG1["U_LotNo"] = lotNo;
			rowIG1["U_PlnCode"] = plnCode;
			rowIG1.Table.AcceptChanges();
			document_LinesImpl.UserFields = CommonRepository.DataRowToUserFields(rowIG1, lstUdfIGE1, sessionId, _unitOfWork);
			document_LinesImpl.Quantity = qty;
			List<DocumentLinesBinAllocationsImpl> list = new List<DocumentLinesBinAllocationsImpl>();
			string[] array = GetBatchSerialString(docItemCode).Split(',');
			string text = array[0];
			string text2 = array[1];
			if (text == "Y")
			{
				BatchNumbersImpl batchNumbersByProductionID = GetBatchNumbersByProductionID(docEntry, qty, lineNum);
				if (IsBinWarehouse(wareHouse))
				{
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
					documentLinesBinAllocationsImpl.BaseLineNumber = lineNum;
					documentLinesBinAllocationsImpl.BinAbsEntry = binAbs;
					documentLinesBinAllocationsImpl.Quantity = qty;
					documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
					if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
					{
						list.Add(documentLinesBinAllocationsImpl);
					}
				}
				batchNumbersByProductionID.BaseLineNumber = lineNum;
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.BatchNumbers.Add(batchNumbersByProductionID);
			}
			else if (text2 == "Y")
			{
				Tuple<string, List<SerialNumbersImpl>> serialNumbersPDA = GetSerialNumbersPDA(docEntry, ref DTusedSerial, lstRow, lineNum);
				if (serialNumbersPDA.Item1 != "")
				{
					return new Tuple<string, Document_LinesImpl>(serialNumbersPDA.Item1, null);
				}
				document_LinesImpl.SerialNumbers = serialNumbersPDA.Item2;
				if (IsBinWarehouse(wareHouse))
				{
					for (int i = 0; i < document_LinesImpl.SerialNumbers.Count; i++)
					{
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl2.BaseLineNumber = lineNum;
						documentLinesBinAllocationsImpl2.BinAbsEntry = binAbs;
						documentLinesBinAllocationsImpl2.Quantity = 1.0;
						documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = i;
						if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
						{
							list.Add(documentLinesBinAllocationsImpl2);
						}
					}
				}
			}
			else if (IsBinWarehouse(wareHouse))
			{
				DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl3 = new DocumentLinesBinAllocationsImpl();
				documentLinesBinAllocationsImpl3.BaseLineNumber = lineNum;
				documentLinesBinAllocationsImpl3.BinAbsEntry = binAbs;
				documentLinesBinAllocationsImpl3.Quantity = qty;
				if (documentLinesBinAllocationsImpl3.BinAbsEntry > 0)
				{
					list.Add(documentLinesBinAllocationsImpl3);
				}
			}
			if (list.Count > 0)
			{
				document_LinesImpl.BinAllocations = list;
			}
			return new Tuple<string, Document_LinesImpl>("", document_LinesImpl);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, Document_LinesImpl>(ex.Message, null);
		}
	}

	private Tuple<string, Document_LinesImpl> GetReceiptLineForPC(int lineNum, int docNum, int docEntry, string acctCode, string wareHouse, string lotNo, string plnCode, DataRow rowIG1, string sessionId, double qty, string docItemCode, ref int usedSerialCount, List<UserFieldsImpl> lstUdfIGN1)
	{
		try
		{
			Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
			document_LinesImpl.LineNum = lineNum;
			document_LinesImpl.BaseRef = docNum;
			document_LinesImpl.BaseType = 202;
			document_LinesImpl.BaseEntry = docEntry;
			document_LinesImpl.BaseLine = -1;
			document_LinesImpl.AcctCode = acctCode;
			document_LinesImpl.WhsCode = wareHouse;
			rowIG1["U_LotNo"] = lotNo;
			rowIG1["U_PlnCode"] = plnCode;
			rowIG1.Table.AcceptChanges();
			document_LinesImpl.UserFields = CommonRepository.DataRowToUserFields(rowIG1, lstUdfIGN1, sessionId, _unitOfWork);
			document_LinesImpl.Quantity = qty;
			string[] array = GetBatchSerialString(docItemCode).Split(',');
			string text = array[0];
			string text2 = array[1];
			if (text == "Y")
			{
				BatchNumbersImpl batchNumbersByProductionID = GetBatchNumbersByProductionID(docEntry, qty, lineNum);
				batchNumbersByProductionID.BaseLineNumber = lineNum;
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.BatchNumbers.Add(batchNumbersByProductionID);
			}
			else if (text2 == "Y")
			{
				Tuple<string, List<SerialNumbersImpl>> serialNumbersPC = GetSerialNumbersPC(docEntry, Function.ParseInt(qty), usedSerialCount, lineNum);
				if (serialNumbersPC.Item1 != "")
				{
					return new Tuple<string, Document_LinesImpl>(serialNumbersPC.Item1, null);
				}
				document_LinesImpl.SerialNumbers = serialNumbersPC.Item2;
				usedSerialCount = serialNumbersPC.Item2.Count;
			}
			return new Tuple<string, Document_LinesImpl>("", document_LinesImpl);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, Document_LinesImpl>(ex.Message, null);
		}
	}

	private string GetBatchSerialString(string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select ManBtchNum,ManSerNum From oitm where itemcode=@itemCode", CommandType.Text, new SqlParameter("@itemCode", itemCode));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]) + "," + Function.ToString(dataTable.Rows[0][1]);
			}
			return "N,N";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "N,N";
		}
	}

	private Tuple<string, List<BatchNumbersImpl>> GetBatchQuantityForIssue(string itemCode, double quantity, string whsCode, int lineNum, DataTable dtUsedBatch, DataTable dtNewBatchSerial)
	{
		try
		{
			List<BatchNumbersImpl> list = new List<BatchNumbersImpl>();
			DataTable dataTable = ExecuteDataTable("APZ_ProductionOrderImportTool_GetBatchQuantityForIssue", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("Instock", typeof(double)));
				dataTable.Columns.Add(new DataColumn("DistNumber", typeof(string)));
				dataTable.AcceptChanges();
			}
			if (dtNewBatchSerial.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from r in dtNewBatchSerial.AsEnumerable()
					where string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["WhsCode"]), whsCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["ManBatchSer"]), "B", ignoreCase: true) == 0
					select r;
				if (source.Any())
				{
					DataRow[] array = source.ToArray();
					for (int num = 0; num < array.Length; num++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Instock"] = Function.ParseDouble(array[num]["Quantity"]);
						dataRow["DistNumber"] = Function.ToString(array[num]["BnSDistNum"]);
						dataTable.Rows.Add(dataRow);
					}
					dataTable.AcceptChanges();
				}
			}
			double num2 = 0.0;
			if (dataTable.IsNotNull())
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					column.ReadOnly = false;
				}
				EnumerableRowCollection<DataRow> source2 = from r in dtUsedBatch.AsEnumerable()
					where Function.ToString(r["ItemCode"]) == itemCode && Function.ToString(r["WhsCode"]) == whsCode
					select r;
				if (source2.Any())
				{
					for (int num3 = dataTable.Rows.Count - 1; num3 >= 0; num3--)
					{
						DataRow dataRow2 = dataTable.Rows[num3];
						string distNumber = Function.ToString(dataRow2["DistNumber"]);
						DataRow dataRow3 = source2.FirstOrDefault((DataRow r) => Function.ToString(r["DistNumber"]) == distNumber);
						if (dataRow3 != null)
						{
							dataRow2["Instock"] = Function.ParseDouble(dataRow2["Instock"]) - Function.ParseDouble(dataRow3["Quantity"]);
						}
						if (Function.ParseDouble(dataRow2["Instock"]) <= 0.0)
						{
							dataTable.Rows.Remove(dataRow2);
						}
						dataTable.AcceptChanges();
					}
				}
				for (int num4 = 0; num4 < dataTable.Rows.Count; num4++)
				{
					double num5 = Function.ParseDouble(dataTable.Rows[num4]["Instock"]);
					double num6 = quantity - num2;
					double num7 = num5 - num6;
					if (!(num6 > 0.0))
					{
						break;
					}
					if (num7 >= 0.0)
					{
						BatchNumbersImpl btNums = new BatchNumbersImpl();
						num2 += num6;
						btNums.AddmisionDate = DateTime.Today;
						btNums.BatchNumber = Function.ToString(dataTable.Rows[num4]["DistNumber"]);
						btNums.Quantity = num6;
						DataRow dataRow4 = source2.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["DistNumber"]) == btNums.BatchNumber);
						if (dataRow4 != null)
						{
							dataRow4["Quantity"] = Function.ParseDouble(dataRow4["Quantity"]) + btNums.Quantity;
							dataRow4.Table.AcceptChanges();
						}
						else
						{
							dataRow4 = dtUsedBatch.NewRow();
							dataRow4["ItemCode"] = itemCode;
							dataRow4["WhsCode"] = whsCode;
							dataRow4["DistNumber"] = btNums.BatchNumber;
							dataRow4["Quantity"] = btNums.Quantity;
							dtUsedBatch.Rows.Add(dataRow4);
						}
						btNums.BaseLineNumber = lineNum;
						list.Add(btNums);
						break;
					}
					BatchNumbersImpl btNums2 = new BatchNumbersImpl();
					num2 += num5;
					btNums2.AddmisionDate = DateTime.Today;
					btNums2.BatchNumber = Function.ToString(dataTable.Rows[num4]["DistNumber"]);
					btNums2.Quantity = num5;
					DataRow dataRow5 = source2.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["DistNumber"]) == btNums2.BatchNumber);
					if (dataRow5 != null)
					{
						dataRow5["Quantity"] = Function.ParseDouble(dataRow5["Quantity"]) + btNums2.Quantity;
					}
					else
					{
						dataRow5 = dtUsedBatch.NewRow();
						dataRow5["ItemCode"] = itemCode;
						dataRow5["WhsCode"] = whsCode;
						dataRow5["DistNumber"] = btNums2.BatchNumber;
						dataRow5["Quantity"] = btNums2.Quantity;
						dtUsedBatch.Rows.Add(dataRow5);
					}
					dataRow5.Table.AcceptChanges();
					btNums2.BaseLineNumber = lineNum;
					list.Add(btNums2);
				}
				if (Math.Abs(quantity - num2) > 1E-09)
				{
					return new Tuple<string, List<BatchNumbersImpl>>("Not enough quantity in batch numbers for issue item [" + itemCode + "]", new List<BatchNumbersImpl>());
				}
				return new Tuple<string, List<BatchNumbersImpl>>("", list);
			}
			return new Tuple<string, List<BatchNumbersImpl>>("No available batch number that have quantity greater than 0 for issue item [" + itemCode + "]", new List<BatchNumbersImpl>());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, List<BatchNumbersImpl>>(ex.Message, new List<BatchNumbersImpl>());
		}
	}

	private Tuple<string, List<SerialNumbersImpl>> GetSerialQuantityForIssue(string itemCode, double quantity, string whsCode, int lineNum, DataTable dtNewBatchSerial, DataTable usedSerial)
	{
		try
		{
			List<SerialNumbersImpl> list = new List<SerialNumbersImpl>();
			DataTable dataTable = ExecuteDataTable("APZ_ProductionOrderImportTool_GetSerialForIssue", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("Instock", typeof(double)));
				dataTable.Columns.Add(new DataColumn("DistNumber", typeof(string)));
				dataTable.AcceptChanges();
			}
			if (dtNewBatchSerial.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from r in dtNewBatchSerial.AsEnumerable()
					where string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["WhsCode"]), whsCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["ManBatchSer"]), "S", ignoreCase: true) == 0
					select r;
				if (source.Any())
				{
					DataRow[] array = source.ToArray();
					for (int num = 0; num < array.Length; num++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Instock"] = Function.ParseDouble(array[num]["Quantity"]);
						dataRow["DistNumber"] = Function.ToString(array[num]["BnSDistNum"]);
						dataTable.Rows.Add(dataRow);
					}
					dataTable.AcceptChanges();
				}
			}
			if (dataTable.IsNotNull())
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					column.ReadOnly = false;
				}
				EnumerableRowCollection<DataRow> source2 = from r in usedSerial.AsEnumerable()
					where Function.ToString(r["ItemCode"]) == itemCode && Function.ToString(r["WhsCode"]) == whsCode
					select r;
				if (source2.Any())
				{
					for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow dataRow2 = dataTable.Rows[num2];
						string distNumber = Function.ToString(dataRow2["DistNumber"]);
						if (source2.FirstOrDefault((DataRow r) => Function.ToString(r["DistNumber"]) == distNumber) != null)
						{
							dataTable.Rows.Remove(dataRow2);
						}
						dataTable.AcceptChanges();
					}
				}
				if ((double)dataTable.Rows.Count < quantity)
				{
					return new Tuple<string, List<SerialNumbersImpl>>("Not enough quantity of serial numbers for issue item [" + itemCode + "]", new List<SerialNumbersImpl>());
				}
				for (int num3 = 0; (double)num3 < quantity; num3++)
				{
					SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
					serialNumbersImpl.InternalSerialNumber = Function.ToString(dataTable.Rows[num3]["DistNumber"]);
					serialNumbersImpl.Quantity = 1.0;
					serialNumbersImpl.BaseLineNumber = lineNum;
					list.Add(serialNumbersImpl);
				}
				return new Tuple<string, List<SerialNumbersImpl>>("", list);
			}
			return new Tuple<string, List<SerialNumbersImpl>>("item code - '" + itemCode + "', warehouse Code - '" + whsCode + "'  : Get no serial number to issue components", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, List<SerialNumbersImpl>>(ex.Message, new List<SerialNumbersImpl>());
		}
	}

	private BatchNumbersImpl GetBatchNumbersByProductionID(int docEntry, double quantity, int lineNum)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_ProductionOrderImportTool_GetBatchNumbersByPrdOrdrID", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
			if (dataTable.IsNotNull())
			{
				BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
				batchNumbersImpl.AddmisionDate = DateTime.Today;
				batchNumbersImpl.BatchNumber = Function.ToString(dataTable.Rows[0][0]);
				batchNumbersImpl.Quantity = quantity;
				batchNumbersImpl.BaseLineNumber = lineNum;
				batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
				batchNumbersImpl.UserFields.Add(new UserFieldsImpl
				{
					Name = "U_BoxTem",
					Value = Function.ToString(dataTable.Rows[0][1])
				});
				return batchNumbersImpl;
			}
			return new BatchNumbersImpl();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new BatchNumbersImpl();
		}
	}

	private Tuple<string, List<SerialNumbersImpl>> GetSerialNumbersPC(int docEntry, int quantity, int usedQty = 0, int lineNum = 0)
	{
		try
		{
			List<SerialNumbersImpl> list = new List<SerialNumbersImpl>();
			DataTable dataTable = ExecuteDataTable("APZ_ProductionOrderImportTool_GetSerialNumbersByProductionID", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
			if (dataTable.IsNotNull())
			{
				if (dataTable.Rows.Count - usedQty < quantity)
				{
					return new Tuple<string, List<SerialNumbersImpl>>("Production order no - '" + docEntry + "': Get not enough serial number to receipt finish goods", list);
				}
				DataTable dataTable2 = dataTable.Clone();
				for (int i = usedQty; i < quantity + usedQty; i++)
				{
					dataTable2.ImportRow(dataTable.Rows[i]);
				}
				dataTable2.AcceptChanges();
				for (int j = 0; j < quantity; j++)
				{
					SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
					serialNumbersImpl.InternalSerialNumber = Function.ToString(dataTable2.Rows[j][0]);
					serialNumbersImpl.Quantity = 1.0;
					serialNumbersImpl.BaseLineNumber = lineNum;
					serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
					serialNumbersImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_BoxTem",
						Value = Function.ToString(dataTable2.Rows[j][1])
					});
					list.Add(serialNumbersImpl);
				}
				return new Tuple<string, List<SerialNumbersImpl>>("", list);
			}
			return new Tuple<string, List<SerialNumbersImpl>>("Production order no - '" + docEntry + "': Get no serial number to receipt finish goods", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, List<SerialNumbersImpl>>(ex.Message, new List<SerialNumbersImpl>());
		}
	}

	private Tuple<string, List<SerialNumbersImpl>> GetSerialNumbersPDA(int docEntry, ref DataTable DTUsedSerial, List<DataRow> lstRow, int lineNum = 0)
	{
		try
		{
			List<SerialNumbersImpl> list = new List<SerialNumbersImpl>();
			DataTable productionOrderData = GetProductionOrderData(docEntry);
			string itemCode = Function.ToString(productionOrderData.Rows[0]["ItemCode"]);
			if (lstRow != null && lstRow.Count > 0)
			{
				for (int i = 0; i < lstRow.Count; i++)
				{
					string distNumber = Function.ToString(lstRow[i]["DistNumber"]);
					if (!DTUsedSerial.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["DistNumber"]), distNumber, ignoreCase: true) == 0))
					{
						SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
						serialNumbersImpl.InternalSerialNumber = distNumber;
						serialNumbersImpl.Quantity = 1.0;
						serialNumbersImpl.BaseLineNumber = lineNum;
						list.Add(serialNumbersImpl);
						DataRow dataRow = DTUsedSerial.NewRow();
						dataRow["ItemCode"] = itemCode;
						dataRow["DistNumber"] = distNumber;
						DTUsedSerial.Rows.Add(dataRow);
						DTUsedSerial.AcceptChanges();
						continue;
					}
					return new Tuple<string, List<SerialNumbersImpl>>("(ItemCode, Serial Number):('" + itemCode + "',,'" + distNumber + "') duplicated", list);
				}
			}
			return new Tuple<string, List<SerialNumbersImpl>>("", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, List<SerialNumbersImpl>>(ex.Message, new List<SerialNumbersImpl>());
		}
	}

	private string GetBinAllocsByBatch(List<BatchNumbersImpl> list, string itemCode, string whsCode, ref List<DocumentLinesBinAllocationsImpl> binAllocs, DataTable DTUsedBin)
	{
		try
		{
			int defaultBinAbs = GetDefaultBinAbs(whsCode);
			for (int i = 0; i < list.Count; i++)
			{
				BatchNumbersImpl batch = list[i];
				DataTable binDataOfBatch = GetBinDataOfBatch(batch.BatchNumber, itemCode, whsCode);
				if (!binDataOfBatch.IsNotNull())
				{
					continue;
				}
				EnumerableRowCollection<DataRow> source = from r in DTUsedBin.AsEnumerable()
					where Function.ToString(r["ItemCode"]) == itemCode && Function.ToString(r["WhsCode"]) == whsCode && string.Compare(Function.ToString(r["DistNumber"]), batch.BatchNumber, ignoreCase: true) == 0
					select r;
				if (source.Any())
				{
					for (int num = binDataOfBatch.Rows.Count - 1; num >= 0; num--)
					{
						DataRow dataRow = binDataOfBatch.Rows[num];
						int binAbs = Function.ParseInt(dataRow["BinAbs"]);
						if (binAbs != defaultBinAbs)
						{
							DataRow dataRow2 = source.FirstOrDefault((DataRow r) => Function.ParseInt(r["BinAbs"]) == binAbs);
							if (dataRow2 != null)
							{
								dataRow["Quantity"] = Function.ParseDouble(dataRow["Quantity"]) - Function.ParseDouble(dataRow2["Quantity"]);
							}
							if (Function.ParseDouble(dataRow["Quantity"]) <= 0.0)
							{
								binDataOfBatch.Rows.Remove(dataRow);
							}
							binDataOfBatch.AcceptChanges();
						}
					}
				}
				double quantity = batch.Quantity;
				double num2 = 0.0;
				for (int num3 = 0; num3 < binDataOfBatch.Rows.Count; num3++)
				{
					int binAbs2 = Function.ParseInt(binDataOfBatch.Rows[num3]["BinAbs"]);
					if (binAbs2 == defaultBinAbs)
					{
						continue;
					}
					double num4 = Function.ParseDouble(binDataOfBatch.Rows[num3]["Quantity"]);
					double num5 = quantity - num2;
					double num6 = num4 - num5;
					if (!(num5 > 0.0))
					{
						break;
					}
					if (num6 >= 0.0)
					{
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl.BaseLineNumber = batch.BaseLineNumber;
						documentLinesBinAllocationsImpl.BinAbsEntry = binAbs2;
						documentLinesBinAllocationsImpl.Quantity = num5;
						documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = i;
						if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
						{
							binAllocs.Add(documentLinesBinAllocationsImpl);
							DataRow dataRow3 = DTUsedBin.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["WhsCode"]), whsCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["DistNumber"]), batch.BatchNumber, ignoreCase: true) == 0 && Function.ParseInt(r["BinAbs"]) == binAbs2);
							if (dataRow3 == null)
							{
								dataRow3 = DTUsedBin.NewRow();
								dataRow3["ItemCode"] = itemCode;
								dataRow3["WhsCode"] = whsCode;
								dataRow3["DistNumber"] = batch.BatchNumber;
								dataRow3["BinAbs"] = binAbs2;
								dataRow3["Quantity"] = num5;
								DTUsedBin.Rows.Add(dataRow3);
							}
							else
							{
								dataRow3["Quantity"] = Function.ParseDouble(dataRow3["Quantity"]) + num5;
							}
							DTUsedBin.AcceptChanges();
						}
						num2 += num5;
						break;
					}
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
					documentLinesBinAllocationsImpl2.BaseLineNumber = batch.BaseLineNumber;
					documentLinesBinAllocationsImpl2.BinAbsEntry = binAbs2;
					documentLinesBinAllocationsImpl2.Quantity = num4;
					documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = i;
					if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
					{
						binAllocs.Add(documentLinesBinAllocationsImpl2);
						DataRow dataRow4 = DTUsedBin.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["WhsCode"]), whsCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["DistNumber"]), batch.BatchNumber, ignoreCase: true) == 0 && Function.ParseInt(r["BinAbs"]) == binAbs2);
						if (dataRow4 == null)
						{
							dataRow4 = DTUsedBin.NewRow();
							dataRow4["ItemCode"] = itemCode;
							dataRow4["WhsCode"] = whsCode;
							dataRow4["DistNumber"] = batch.BatchNumber;
							dataRow4["BinAbs"] = binAbs2;
							dataRow4["Quantity"] = num4;
							DTUsedBin.Rows.Add(dataRow4);
						}
						else
						{
							dataRow4["Quantity"] = Function.ParseDouble(dataRow4["Quantity"]) + num4;
						}
						DTUsedBin.AcceptChanges();
					}
					num2 += num4;
				}
				if (num2 < quantity)
				{
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl3 = new DocumentLinesBinAllocationsImpl();
					documentLinesBinAllocationsImpl3.BaseLineNumber = batch.BaseLineNumber;
					documentLinesBinAllocationsImpl3.BinAbsEntry = defaultBinAbs;
					documentLinesBinAllocationsImpl3.Quantity = quantity - num2;
					documentLinesBinAllocationsImpl3.SerialAndBatchNumbersBaseLine = i;
					if (documentLinesBinAllocationsImpl3.BinAbsEntry > 0)
					{
						binAllocs.Add(documentLinesBinAllocationsImpl3);
						DTUsedBin.AcceptChanges();
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private DataTable GetBinDataOfBatch(string distNumber, string itemCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetBinDataOfBatch", CommandType.StoredProcedure, new SqlParameter("@distNumber", distNumber), new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetWhs(string binCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@binCode", binCode);
			return ExecuteDataTable("Apz_InventoryTransfer_GetWhsByBin", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	private DataTable GetTableStructureQuantityDocEntry()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("Quantity", typeof(double));
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	private DataTable GetProductionOrderData(int docEntry)
	{
		try
		{
			return ExecuteDataTable("select * from OWOR where docEntry=@docEntry", CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	private string AddReceiptLineForIssueQuantity(DataTable dt, ref DocumentsImpl doc, DataTable dtNewBatchSerialForIssue)
	{
		try
		{
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("WhsCode", typeof(string));
			dataTable.Columns.Add("Quantity", typeof(double));
			dataTable.Columns.Add("AcctCode", typeof(string));
			dataTable.Columns.Add("AvgPrice", typeof(double));
			dataTable.AcceptChanges();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int docEntry = Function.ParseInt(dt.Rows[i]["DocEntry"]);
				double quantity = Function.ParseDouble(dt.Rows[i]["Quantity"]);
				DataTable allProductionOrderLine = GetAllProductionOrderLine(docEntry, quantity);
				if (!allProductionOrderLine.IsNotNull())
				{
					continue;
				}
				for (int j = 0; j < allProductionOrderLine.Rows.Count; j++)
				{
					string itemCode = Function.ToString(allProductionOrderLine.Rows[j]["ItemCode"]);
					string WhsCode = Function.ToString(allProductionOrderLine.Rows[j]["wareHouse"]);
					double num = Function.ParseDouble(allProductionOrderLine.Rows[j]["ProductionQty"]);
					string value = Function.ToString(allProductionOrderLine.Rows[j]["AcctCode"]);
					double num2 = Function.ParseDouble(allProductionOrderLine.Rows[j]["AvgPrice"]);
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["ItemCode"]), itemCode, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["WhsCode"]), WhsCode, ignoreCase: true) == 0);
					if (dataRow == null)
					{
						dataRow = dataTable.NewRow();
						dataRow["ItemCode"] = itemCode;
						dataRow["WhsCode"] = WhsCode;
						dataRow["Quantity"] = num;
						dataRow["AcctCode"] = value;
						dataRow["AvgPrice"] = num2;
						dataTable.Rows.Add(dataRow);
					}
					else
					{
						dataRow["Quantity"] = Function.ParseDouble(dataRow["Quantity"]) + num;
					}
				}
			}
			dataTable.AcceptChanges();
			DateTime now = DateTime.Now;
			int num3 = 0;
			for (int num4 = 0; num4 < dataTable.Rows.Count; num4++)
			{
				string text = Function.ToString(dataTable.Rows[num4]["ItemCode"]);
				string text2 = Function.ToString(dataTable.Rows[num4]["WhsCode"]);
				string acctCode = Function.ToString(dataTable.Rows[num4]["AcctCode"]);
				double priceBefDi = Function.ParseDouble(dataTable.Rows[num4]["AvgPrice"]);
				double num5 = Function.ParseDouble(dataTable.Rows[num4]["Quantity"]);
				double availQtyByItemWarehouse = GetAvailQtyByItemWarehouse(text, text2);
				double num6 = num5 - availQtyByItemWarehouse;
				if (!(num6 > 0.0))
				{
					continue;
				}
				int defaultBinAbs = GetDefaultBinAbs(text2);
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.LineNum = num3;
				document_LinesImpl.ItemCode = text;
				document_LinesImpl.BaseRef = 0;
				document_LinesImpl.BaseType = -1;
				document_LinesImpl.BaseEntry = 0;
				document_LinesImpl.BaseLine = -1;
				document_LinesImpl.AcctCode = acctCode;
				document_LinesImpl.WhsCode = text2;
				document_LinesImpl.Quantity = num6;
				document_LinesImpl.PriceBefDi = priceBefDi;
				List<DocumentLinesBinAllocationsImpl> list2 = new List<DocumentLinesBinAllocationsImpl>();
				string[] array = GetBatchSerialString(text).Split(',');
				string text3 = array[0];
				string text4 = array[1];
				if (text3 == "Y")
				{
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
					string text5 = now.ToString("yyMMddHHmmssfff-");
					batchNumbersImpl.BatchNumber = text5 + num3.ToString("0000");
					batchNumbersImpl.Quantity = num6;
					batchNumbersImpl.BaseLineNumber = num3;
					DataRow dataRow2 = dtNewBatchSerialForIssue.NewRow();
					dataRow2["ItemCode"] = text;
					dataRow2["WhsCode"] = text2;
					dataRow2["ManBatchSer"] = "B";
					dataRow2["Quantity"] = num6;
					dataRow2["BnSDistNum"] = batchNumbersImpl.BatchNumber;
					if (IsBinWarehouse(text2))
					{
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl.BaseLineNumber = num3;
						documentLinesBinAllocationsImpl.BinAbsEntry = defaultBinAbs;
						documentLinesBinAllocationsImpl.Quantity = num5;
						documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
						if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
						{
							dataRow2["BinAbs"] = defaultBinAbs;
							list2.Add(documentLinesBinAllocationsImpl);
						}
					}
					document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
					document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					dtNewBatchSerialForIssue.Rows.Add(dataRow2);
				}
				else if (text4 == "Y")
				{
					List<SerialNumbersImpl> list3 = new List<SerialNumbersImpl>();
					for (int num7 = 0; (double)num7 < num6; num7++)
					{
						SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
						string text6 = now.ToString("yyMMddHHmmssfff-");
						serialNumbersImpl.InternalSerialNumber = text6 + num3.ToString("0000") + "-" + num7;
						serialNumbersImpl.Quantity = 1.0;
						serialNumbersImpl.BaseLineNumber = num3;
						list3.Add(serialNumbersImpl);
						DataRow dataRow3 = dtNewBatchSerialForIssue.NewRow();
						dataRow3["ItemCode"] = text;
						dataRow3["WhsCode"] = text2;
						dataRow3["ManBatchSer"] = "S";
						dataRow3["Quantity"] = 1;
						dataRow3["BnSDistNum"] = serialNumbersImpl.InternalSerialNumber;
						if (IsBinWarehouse(text2))
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BaseLineNumber = num3;
							documentLinesBinAllocationsImpl2.BinAbsEntry = defaultBinAbs;
							documentLinesBinAllocationsImpl2.Quantity = 1.0;
							documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num7;
							if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
							{
								dataRow3["BinAbs"] = defaultBinAbs;
								list2.Add(documentLinesBinAllocationsImpl2);
							}
						}
						dtNewBatchSerialForIssue.Rows.Add(dataRow3);
					}
					document_LinesImpl.SerialNumbers = list3;
				}
				else
				{
					DataRow dataRow4 = dtNewBatchSerialForIssue.NewRow();
					dataRow4["ItemCode"] = text;
					dataRow4["WhsCode"] = text2;
					dataRow4["ManBatchSer"] = "N";
					dataRow4["Quantity"] = num6;
					dataRow4["BnSDistNum"] = "";
					if (IsBinWarehouse(text2))
					{
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl3 = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl3.BaseLineNumber = num3;
						documentLinesBinAllocationsImpl3.BinAbsEntry = defaultBinAbs;
						documentLinesBinAllocationsImpl3.Quantity = num6;
						if (documentLinesBinAllocationsImpl3.BinAbsEntry > 0)
						{
							dataRow4["BinAbs"] = defaultBinAbs;
							list2.Add(documentLinesBinAllocationsImpl3);
						}
					}
				}
				if (list2.Count > 0)
				{
					document_LinesImpl.BinAllocations = list2;
				}
				list.Add(document_LinesImpl);
				num3++;
			}
			doc.Lines = list;
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private int GetDefaultBinAbs(string WhsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select DftBinAbs From OWHS where WhsCode=@WhsCode", CommandType.Text, new SqlParameter("@WhsCode", WhsCode));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return 0;
	}

	private double GetAvailQtyByItemWarehouse(string itemCode, string whsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select OnHand From OITW where itemcode=@itemCode and whscode=@whsCode", CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
			if (dataTable.IsNotNull())
			{
				return Function.ParseDouble(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return 0.0;
	}

	private DataTable GetAllProductionOrderLine(int docEntry, double quantity)
	{
		try
		{
			return ExecuteDataTable("APZ_ProductionOrderImportTool_GetAllProductionOrderLine", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@quantity", quantity));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public string SaveClose(DataTable dt, string sessionId)
	{
		try
		{
			List<ProductionOder_Impl_OWOR> list = (List<ProductionOder_Impl_OWOR>)CommonRepository.TableToEntity("ProductionOder_Impl_OWOR", dt, sessionId, _unitOfWork);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Status = "L";
			}
			return _unitOfWork.ProductionOrders.UpdateStatus(list, sessionId).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private DataTable GetProductionOrderLine(int DocEntry)
	{
		try
		{
			return ExecuteDataTable("Select * From WOR1 Where docentry=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", DocEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	private DataTable GetManualProductionOrderLine(int DocEntry)
	{
		try
		{
			return ExecuteDataTable("Select * From WOR1 Where docentry=@DocEntry and IssueType='M'", CommandType.Text, new SqlParameter("@DocEntry", DocEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public bool IsBinWarehouse(string whsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select isnull(BinActivat,'N') from owhs where whsCode=@whsCode", CommandType.Text, new SqlParameter("@whsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]) == "Y";
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return false;
		}
	}

	public void Dispose()
	{
		Dispose();
	}

	public string SaveInteruptCodeRC(string intrCode, string sessionId)
	{
		try
		{
			using Transaction transaction = new Transaction();
			DataTable dataTable = ExecuteDataTable("SELECT DistNumber,c.ItmsGrpCod,a.ItemCode FROM OSRN a INNER JOIN OITM b ON a.ItemCode=b.ItemCode INNER JOIN OITB c on b.ItmsGrpCod = c.ItmsGrpCod WHERE RTRIM(LTRIM(isnull(U_PalletNo, ''))) = '' AND c.U_InterruptCode = @intrCode", CommandType.Text, new SqlParameter("@intrCode", intrCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string text = DateTime.Now.ToString("yyyyMMddHHmmss");
				string text2 = intrCode + "-" + text;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					string text3 = Function.ToString(dataTable.Rows[i]["ItemCode"]);
					string text4 = Function.ToString(dataTable.Rows[i]["DistNumber"]);
					if (ExecuteNonQuery("UPDATE OSRN SET U_PalletNo = @palletNo WHERE DistNumber=@distNumber AND ItemCode=@itemCode", CommandType.Text, new SqlParameter("@palletNo", text2), new SqlParameter("@itemCode", text3), new SqlParameter("@distNumber", text4)) <= 0)
					{
						return "Lỗi khi tạo pallet: pallet:'" + text2 + "', ItemCode:'" + text3 + "', Serial:'" + text4 + "'";
					}
				}
				transaction.Commit();
				return "";
			}
			return "Lỗi khi tạo pallet: không tìm thấy serial";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "Lỗi khi tạo pallet";
		}
	}

	public string WithdrawOfGooods(DataTable dt, string sessionId)
	{
		try
		{
			if (dt.IsNotNull())
			{
				string text = "";
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (Function.ParseBool(dt.Rows[i]["Exist"]))
					{
						text = text + "," + dt.Rows[i]["DistNumber"];
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					return "";
				}
				text = text.Substring(1, text.Length - 1);
				ExecuteNonQuery("Apz_ProductionOrder_WithdrawOfGooods", CommandType.StoredProcedure, new SqlParameter("@barcode", text));
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<string> ReportCompletion(DataTable dt, string sessionId)
	{
		string text = "ProductionOrderImportToolRepository->ReportCompletion():";
		Logging.Write("TRACE", "[Start add ReportCompletion from HT]");
		try
		{
			if (!dt.IsNotNull())
			{
				Logging.Write("WATCH", "Empty Data");
				return "";
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocType = "I";
			documentsImpl.ObjType = 59;
			documentsImpl.DocDate = DateTime.Today;
			documentsImpl.Comments = commentsRcReceiptPDA;
			documentsImpl.JrnlMemo = commentsRcReceiptPDA;
			documentsImpl.BPLId = Function.ParseInt(dt.Rows[0]["BPLId"]);
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			int num = 0;
			List<DataColumn> list2 = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select t).ToList();
			foreach (var item3 in (from t in dt.AsEnumerable()
				where Function.ParseInt(t["DocEntry"]) > 0 && !Function.ParseBool(t["Exist"])
				group t by new
				{
					docentry = Function.ParseInt(t["DocEntry"])
				} into g
				select new
				{
					DocEntry = Function.ParseInt(g.Key.docentry),
					Items = g.Select((DataRow x) => x).Distinct()
				}).ToList())
			{
				int docEntry = item3.DocEntry;
				List<DataRow> list3 = item3.Items.ToList();
				DataRow dataRow = list3[0];
				int baseRef = Function.ParseInt(dataRow["DocNum"]);
				string acctCode = Function.ToString(dataRow["AcctCode"]);
				double num2 = list3.Where((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["CmpQuantiy"]));
				double num3 = list3.Where((DataRow r) => Function.ParseDouble(r["RjQuantity"]) > 0.0).Sum((DataRow r) => Function.ParseDouble(r["RjQuantity"]));
				string whsCode = Function.ToString(dataRow["Warehouse"]);
				string text2 = Function.ToString(dataRow["ItemCode"]);
				double num4 = num2 + num3;
				if (num4 <= 0.0)
				{
					return "Quantity is missing";
				}
				if (!(num2 > 0.0))
				{
					continue;
				}
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.LineNum = num;
				document_LinesImpl.BaseRef = baseRef;
				document_LinesImpl.BaseType = 202;
				document_LinesImpl.BaseEntry = docEntry;
				document_LinesImpl.BaseLine = -1;
				document_LinesImpl.AcctCode = acctCode;
				document_LinesImpl.WhsCode = whsCode;
				document_LinesImpl.Quantity = num4;
				document_LinesImpl.ItemCode = text2;
				string text3 = "N";
				string text4 = "Y";
				List<SqlParameter> list4 = new List<SqlParameter>();
				list4.Add(new SqlParameter("@itemCode", text2));
				DataTable dataTable = ExecuteDataTable("SELECT ManBtchNum,ManSerNum FROM OITM WHERE ItemCode = @itemCode", CommandType.Text, list4.ToArray());
				if (dataTable.IsNotNull())
				{
					text3 = Function.ToString(dataTable.Rows[0]["ManBtchNum"]);
					text4 = Function.ToString(dataTable.Rows[0]["ManSerNum"]);
				}
				if (text3.Equals("N") && text4.Equals("Y"))
				{
					List<SerialNumbersImpl> list5 = new List<SerialNumbersImpl>();
					if (list3.Count > 0)
					{
						for (int num5 = 0; num5 < list3.Count; num5++)
						{
							string internalSerialNumber = Function.ToString(list3[num5]["DistNumber"]);
							SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
							serialNumbersImpl.InternalSerialNumber = internalSerialNumber;
							serialNumbersImpl.Quantity = 1.0;
							serialNumbersImpl.BaseLineNumber = num;
							List<UserFieldsImpl> list6 = new List<UserFieldsImpl>();
							foreach (DataColumn item4 in list2)
							{
								UserFieldsImpl item = new UserFieldsImpl
								{
									Name = item4.ColumnName,
									Value = Function.ToString(dataRow[item4.ColumnName])
								};
								list6.Add(item);
							}
							serialNumbersImpl.UserFields = list6;
							list5.Add(serialNumbersImpl);
						}
					}
					document_LinesImpl.SerialNumbers = list5;
				}
				else if (text3.Equals("Y") && text4.Equals("N"))
				{
					List<BatchNumbersImpl> list7 = new List<BatchNumbersImpl>();
					if (list3.Count > 0)
					{
						for (int num6 = 0; num6 < list3.Count; num6++)
						{
							string batchNumber = Function.ToString(list3[num6]["DistNumber"]);
							BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
							batchNumbersImpl.BatchNumber = batchNumber;
							batchNumbersImpl.Quantity = Function.ParseDouble(list3[num6]["CmpQuantiy"]);
							batchNumbersImpl.BaseLineNumber = num;
							List<UserFieldsImpl> list8 = new List<UserFieldsImpl>();
							foreach (DataColumn item5 in list2)
							{
								UserFieldsImpl item2 = new UserFieldsImpl
								{
									Name = item5.ColumnName,
									Value = Function.ToString(dataRow[item5.ColumnName])
								};
								list8.Add(item2);
							}
							batchNumbersImpl.UserFields = list8;
							list7.Add(batchNumbersImpl);
						}
					}
					document_LinesImpl.BatchNumbers = list7;
				}
				document_LinesImpl.TranType = "C";
				list.Add(document_LinesImpl);
				num++;
			}
			documentsImpl.Lines = list;
			documentsImpl.ObjType = 59;
			string text5 = "DECLARE @DocEntry int =(select isnull((select max(docentry) from NFV_PALLET),0)+1)\r\n                            INSERT INTO NFV_PALLET(DocEntry,DistNumber,ItemCode) VALUES ";
			List<string> list9 = new List<string>();
			for (int num7 = 0; num7 < dt.Rows.Count; num7++)
			{
				list9.Add(string.Format("(@DocEntry,'{0}','{1}')", Function.ToString(dt.Rows[num7]["DistNumber"]), Function.ToString(dt.Rows[num7]["ItemCode"])));
			}
			text5 += string.Join(",", list9.ToArray());
			string xmlExecuteSQL = _unitOfWork.UDOs.GetXmlExecuteSQL(text5, "", isGetBody: true);
			string text6 = "";
			if (documentsImpl != null && documentsImpl.Lines.Count > 0)
			{
				text6 = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(documentsImpl, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text6 + xmlExecuteSQL + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
					return xElement2.Value;
				}
			}
			else
			{
				if (!xmlElement.OuterXml.Contains("RetKey"))
				{
					Logging.Write("TRACE", "***[Add pallet document]");
					return "";
				}
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num8 = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new document - Report Completion - DocEntry - {num8}");
					return "";
				}
			}
			Logging.Write("TRACE", "***[End add ReportCompletion from HT]");
			return text + " Null HttpResult from AddNew";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			Logging.Write("TRACE", "***[End add ReportCompletion from HT]");
			return ex.Message;
		}
	}
}
