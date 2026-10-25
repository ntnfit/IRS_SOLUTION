using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.ImportData;

namespace Apzon.Services.Repository.ImportData.BillOfMaterials;

public class ImportDataBillOfMaterials : BaseImportData
{
	public ImportDataBillOfMaterials(string connection, int userID, IUnitOfWork unitOfWork)
		: base(connection, userID, unitOfWork)
	{
	}

	public string SaveAProductTrees(DataRow dr, Enums.ImportActionEnum action, ImportDataImpls objectImport)
	{
		try
		{
			return action switch
			{
				Enums.ImportActionEnum.Add => AddBOM(dr, objectImport), 
				Enums.ImportActionEnum.Update => UpdateBOM(dr), 
				_ => "", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateBOM(DataRow dr)
	{
		try
		{
			string code = Function.ToString(dr["Code"]);
			DataSet billOfMaterials = _unitOfWork.ImportData.GetBillOfMaterials(code);
			if (billOfMaterials != null && billOfMaterials.Tables.Count == 2 && billOfMaterials.Tables[0] != null && billOfMaterials.Tables[0].Rows.Count > 0 && billOfMaterials.Tables[1] != null && billOfMaterials.Tables[1].Rows.Count > 0)
			{
				DataRow dataRow = billOfMaterials.Tables[0].Rows[0];
				dataRow["Qauntity"] = Function.ParseDouble(dr["Qauntity"]);
				dataRow["OcrCode"] = Function.ToString(dr["OcrCode"]);
				dataRow.Table.AcceptChanges();
				return _unitOfWork.BillOfMasterials.SaveDocNoSAPUpdate(billOfMaterials);
			}
			return "BOM table or BOM components table does not have any rows";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddBOM(DataRow dr, ImportDataImpls objectImport)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			if (objectImport.ListDaTaImport.Count == 2)
			{
				string code = Function.ToString(dr["Code"]);
				DataTable dTData = objectImport.ListDaTaImport[1].DTData;
				if (dTData.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["Father"]), code, ignoreCase: true) == 0))
				{
					List<DataRow> list = (from r in dTData.AsEnumerable()
						where string.Compare(Function.ToString(r["Father"]), code, ignoreCase: true) == 0
						select r).ToList();
					DataTable dataTable2 = dTData.Clone();
					for (int num = 0; num < list.Count; num++)
					{
						dataTable2.ImportRow(list[num]);
					}
					dataTable2.AcceptChanges();
					if (dataTable2 == null || dataTable2.Rows.Count == 0)
					{
						return "Cannot add BOM, cannot get components from import data.";
					}
					DataSet dataSet = new DataSet();
					dataSet.Tables.Clear();
					dataSet.Tables.Add(dataTable);
					dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
					dataSet.Tables.Add(dataTable2);
					return _unitOfWork.BillOfMasterials.SaveDocNoSAP(dataSet, Enums.TransactionType.A, "");
				}
				return "Cannot add BOM, no components found.";
			}
			return "BOM Table or components Table have no rows.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveAProductTrees_Lines(DataRow dr, Enums.ImportActionEnum action, ImportDataImpls objectImport)
	{
		try
		{
			return action switch
			{
				Enums.ImportActionEnum.None => "Import action 'ImportActionEnum.None' is invalid.", 
				Enums.ImportActionEnum.Add => AddBOMComponents(dr, objectImport), 
				Enums.ImportActionEnum.Update => UpdateBOMComponents(dr), 
				_ => "Import action 'Add and Update' is invalid. Must be 'Add' or 'Update'", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddBOMComponents(DataRow dr, ImportDataImpls objectImport)
	{
		try
		{
			string father = Function.ToString(dr["Father"]);
			if (objectImport.ListDaTaImport.Count == 2)
			{
				if (objectImport.ListDaTaImport[0].DTData.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["Code"]), father, ignoreCase: true) == 0))
				{
					return "";
				}
				return AddBOMFromComponentRow(dr, father);
			}
			if (objectImport.ListDaTaImport.Count == 1)
			{
				return AddBOMFromComponentRow(dr, father);
			}
			return "BOM Table or components Table have no rows.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateBOMComponents(DataRow dr)
	{
		try
		{
			string father = Function.ToString(dr["Father"]);
			int childNum = Function.ParseInt(dr["ChildNum"]);
			string code = Function.ToString(dr["Code"]);
			DataSet billOfMaterials = _unitOfWork.ImportData.GetBillOfMaterials(father);
			if (billOfMaterials == null || billOfMaterials.Tables.Count != 2 || billOfMaterials.Tables[0] == null || billOfMaterials.Tables[0].Rows.Count == 0 || billOfMaterials.Tables[1] == null || billOfMaterials.Tables[1].Rows.Count == 0)
			{
				return "Can not get BOM dataset or no data in BOM datatable";
			}
			DataTable dataTable = billOfMaterials.Tables[1];
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["Father"]), father, ignoreCase: true) == 0 && string.Compare(Function.ToString(r["Code"]), code, ignoreCase: true) == 0 && Function.ParseInt(r["ChildNum"]) == childNum);
			if (dataRow != null)
			{
				string text = UpdateDataToRow(dr, dataRow);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				dataTable.AcceptChanges();
				return _unitOfWork.BillOfMasterials.SaveDocNoSAP(billOfMaterials, Enums.TransactionType.U, "");
			}
			return "This component does not existed in BOM";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddBOMFromComponentRow(DataRow dr, string father)
	{
		try
		{
			if (!_unitOfWork.ImportData.ExistsBOM(father))
			{
				DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("ITT1");
				DataRow dataRow = bPTableStructure.NewRow();
				string text = UpdateDataToRow(dr, dataRow);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				dataRow["Type"] = 4;
				bPTableStructure.Rows.Add(dataRow);
				bPTableStructure.AcceptChanges();
				if (bPTableStructure == null || bPTableStructure.Rows.Count == 0)
				{
					return "Cannot add BOM, cannot get components from import data.";
				}
				DataTable bPTableStructure2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OITT");
				DataRow dataRow2 = bPTableStructure2.NewRow();
				dataRow2["Code"] = father;
				dataRow2["TreeType"] = "P";
				dataRow2["Qauntity"] = 1;
				dataRow2["Object"] = "66";
				bPTableStructure2.Rows.Add(dataRow2);
				bPTableStructure2.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				dataSet.Tables.Add(bPTableStructure2);
				bPTableStructure.DataSet?.Tables.Remove(bPTableStructure.TableName);
				dataSet.Tables.Add(bPTableStructure);
				return _unitOfWork.BillOfMasterials.SaveDocNoSAP(dataSet, Enums.TransactionType.A, "");
			}
			int childNum = Function.ParseInt(dr["ChildNum"]);
			string code = Function.ToString(dr["Code"]);
			if (!_unitOfWork.ImportData.ExistsBOMChild(father, childNum, code))
			{
				DataSet billOfMaterials = _unitOfWork.ImportData.GetBillOfMaterials(father);
				if (billOfMaterials == null || billOfMaterials.Tables.Count != 2 || billOfMaterials.Tables[0] == null || billOfMaterials.Tables[0].Rows.Count == 0 || billOfMaterials.Tables[1] == null || billOfMaterials.Tables[1].Rows.Count == 0)
				{
					return "Can not get BOM dataset or no data in BOM datatable";
				}
				DataTable dataTable = billOfMaterials.Tables[1];
				int num = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["ChildNum"]));
				DataRow dataRow3 = dataTable.NewRow();
				dataTable.Rows.Add(dataRow3);
				string text2 = UpdateDataToRow(dr, dataRow3);
				if (!string.IsNullOrEmpty(text2))
				{
					return text2;
				}
				dataRow3["ChildNum"] = num + 1;
				dataTable.AcceptChanges();
				return _unitOfWork.BillOfMasterials.SaveDocNoSAP(billOfMaterials, Enums.TransactionType.U, "");
			}
			return "This component existed in a BOM";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
