using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Forms.Commons;

namespace Apzon.Forms.ImportData.ImportHelper.BillOfMaterials;

internal class BillOfMaterialsDataImport : BaseDataImport
{
	protected override string AddObject(DataTable dtImport, List<ImportDataTableObject> LstDTImportObject)
	{
		try
		{
			if (LstDTImportObject.Count == 2)
			{
				string code = Function.ToString(dtImport.Rows[0]["Code"]);
				DataTable importData = LstDTImportObject[1].ImportData;
				string fatherCol = "";
				DataRow dataRow = LstDTImportObject[1].MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Father");
				if (dataRow != null)
				{
					fatherCol = Function.ToString(dataRow["Source"]);
				}
				EnumerableRowCollection<DataRow> source = from r in importData.AsEnumerable()
					where string.Compare(Function.ToString(r[fatherCol]), code, ignoreCase: true) == 0
					select r;
				if (source.Any())
				{
					List<DataRow> list = source.ToList();
					DataTable tableStructure = WinFuntions.GetTableStructure("ITT1");
					for (int num = 0; num < list.Count(); num++)
					{
						DataTable dataTable = ImportHelper.PrepareImportData(list[num], LstDTImportObject[1].MappingData, LstDTImportObject[1].ImportObject);
						if (dataTable.IsNotNull())
						{
							tableStructure.ImportRow(dataTable.Rows[0]);
						}
					}
					tableStructure.AcceptChanges();
					if (tableStructure == null || tableStructure.Rows.Count == 0)
					{
						return "Cannot add BOM, cannot get components from import data.";
					}
					RemoveRowsFromDatatable(list, LstDTImportObject[1].ImportData);
					DataSet dataSet = new DataSet();
					dataSet.Tables.Clear();
					dataSet.Tables.Add(dtImport);
					tableStructure.DataSet?.Tables.Remove(tableStructure.TableName);
					dataSet.Tables.Add(tableStructure);
					return HttpClient.ImportDataClient.ProcessBOM("A", dataSet);
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

	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtImport = null)
	{
		try
		{
			string code;
			if (!(key is DataRow dataRow))
			{
				if (!dtImport.IsNotNull())
				{
					return "No data to update found";
				}
				code = Function.ToString(dtImport.Rows[0]["Code"]);
			}
			else
			{
				code = Function.ToString(dataRow["Code"]);
			}
			DataSet objectAndSubObjectTables = GetObjectAndSubObjectTables(key);
			if (objectAndSubObjectTables != null && objectAndSubObjectTables.Tables.Count == 2 && objectAndSubObjectTables.Tables[0] != null && objectAndSubObjectTables.Tables[0].Rows.Count > 0 && objectAndSubObjectTables.Tables[1] != null && objectAndSubObjectTables.Tables[1].Rows.Count > 0)
			{
				DataRow dataRow2 = objectAndSubObjectTables.Tables[0].Rows[0];
				DataRow dataRow3 = dtImport.Rows[0];
				dataRow2["Qauntity"] = Function.ParseDouble(dataRow3["Qauntity"]);
				dataRow2["OcrCode"] = Function.ToString(dataRow3["OcrCode"]);
				dataRow2.Table.AcceptChanges();
				DataTable dataTable = objectAndSubObjectTables.Tables[1];
				ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.ProductTrees_Lines);
				if (importDataTableObject != null)
				{
					string codeCol = "";
					DataRow dataRow4 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Father");
					if (dataRow4 != null)
					{
						codeCol = Function.ToString(dataRow4["Source"]);
					}
					EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
						where string.Compare(code, Function.ToString(r[codeCol]), ignoreCase: true) == 0
						select r;
					if (source.Any())
					{
						List<DataRow> list = source.ToList();
						for (int num = 0; num < list.Count; num++)
						{
							DataTable dataTable2 = ImportHelper.PrepareImportData(list[num], importDataTableObject.MappingData, importDataTableObject.ImportObject);
							if (!dataTable2.IsNotNull())
							{
								continue;
							}
							string father = Function.ToString(dataTable2.Rows[0]["Father"]);
							int childNum = Function.ParseInt(dataTable2.Rows[0]["ChildNum"]);
							DataRow dataRow5 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(father, Function.ToString(r["Father"]), ignoreCase: true) == 0 && childNum == Function.ParseInt(r["ChildNum"]));
							if (dataRow5 != null)
							{
								string text = UpdateDataToRow(dataTable2.Rows[0], dataRow5);
								if (!string.IsNullOrEmpty(text))
								{
									return text;
								}
							}
							else
							{
								dataTable.ImportRow(dataTable2.Rows[0]);
							}
						}
						dataTable.AcceptChanges();
						RemoveRowsFromDatatable(list, importDataTableObject.ImportData);
					}
					return HttpClient.ImportDataClient.ProcessBOM("U", objectAndSubObjectTables);
				}
				return "BOM components table does not have any rows";
			}
			return "BOM table or BOM components table does not have any rows";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObjectFromChilds(object key, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			if (!(key is DataRow dataRow))
			{
				return "key is null";
			}
			string value = Function.ToString(dataRow["Code"]);
			DataTable tableStructure = WinFuntions.GetTableStructure("OCRD");
			DataRow dataRow2 = tableStructure.NewRow();
			dataRow2["Code"] = value;
			dataRow2["TreeType"] = "P";
			dataRow2["PriceList"] = 0;
			dataRow2["DataSource"] = "N";
			dataRow2["Object"] = "66";
			dataRow2["PlAvgSize"] = 1;
			dataRow2["Qauntity"] = 1;
			tableStructure.Rows.Add(dataRow2);
			tableStructure.AcceptChanges();
			return AddObject(tableStructure, lstDtImportObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override bool CheckExist(object key)
	{
		try
		{
			if (key is DataRow dataRow)
			{
				return HttpClient.ImportDataClient.ExistsBOM(Function.ToString(dataRow["Code"]));
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	protected override DataSet GetObjectAndSubObjectTables(object key)
	{
		try
		{
			if (!(key is DataRow dataRow))
			{
				return new DataSet();
			}
			return HttpClient.ImportDataClient.GetBillOfMaterials(Function.ToString(dataRow["Code"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
