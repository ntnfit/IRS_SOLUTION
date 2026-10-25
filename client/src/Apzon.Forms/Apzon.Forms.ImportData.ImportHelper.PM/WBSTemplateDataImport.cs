using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Forms.Commons;
using Apzon.Global;

namespace Apzon.Forms.ImportData.ImportHelper.PM;

internal class WBSTemplateDataImport : BaseDataImport
{
	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtWBSTemplateSource = null)
	{
		try
		{
			string code;
			if (!(key is DataRow dataRow))
			{
				if (!dtWBSTemplateSource.IsNotNull())
				{
					return "No data to update found";
				}
				code = Function.ToString(dtWBSTemplateSource.Rows[0]["Code"]);
			}
			else
			{
				code = Function.ToString(dataRow["Code"]);
			}
			DataSet objectAndSubObjectTables = GetObjectAndSubObjectTables(key);
			if (objectAndSubObjectTables != null && objectAndSubObjectTables.Tables.Count == 3 && objectAndSubObjectTables.Tables[0] != null && objectAndSubObjectTables.Tables[1] != null && objectAndSubObjectTables.Tables[2] != null)
			{
				if (objectAndSubObjectTables.Tables.Contains("dtDoc"))
				{
					if (dtWBSTemplateSource != null)
					{
						DataTable dataTable = objectAndSubObjectTables.Tables["dtDoc"];
						DataRow dataRow2 = objectAndSubObjectTables.Tables[0].Rows[0];
						object value = dataRow2["CreateDate"];
						object value2 = dataRow2["CreateTime"];
						object value3 = dataRow2["UserSign"];
						object value4 = dataRow2["DocEntry"];
						string text = UpdateDataToRow(dtWBSTemplateSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["DocEntry"] = value4;
						dataTable.Rows[0]["CreateDate"] = value;
						dataTable.Rows[0]["CreateTime"] = value2;
						dataTable.Rows[0]["UserSign"] = value3;
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now.Date;
						dataTable.Rows[0]["UpdateTime"] = DateTime.Now.Hour + ((DateTime.Now.Minute < 10) ? ("0" + DateTime.Now.Minute) : DateTime.Now.Minute.ToString());
						dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
						dataTable.AcceptChanges();
					}
					DataTable dataTable2 = objectAndSubObjectTables.Tables["dtWBS"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'dtWBS' or table have no columns";
					}
					ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.WBSTemplateDetails);
					if (importDataTableObject != null)
					{
						string codeCol = "";
						DataRow dataRow3 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Code");
						if (dataRow3 != null)
						{
							codeCol = Function.ToString(dataRow3["Source"]);
						}
						EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
							where code == Function.ToString(r[codeCol])
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num = 0; num < list.Count(); num++)
							{
								DataTable dataTable3 = ImportHelper.PrepareImportData(list[num], importDataTableObject.MappingData, importDataTableObject.ImportObject);
								if (!dataTable3.IsNotNull())
								{
									continue;
								}
								int line = Function.ParseInt(dataTable3.Rows[0]["LineId"]);
								DataRow dataRow4 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => line == Function.ParseInt(r["LineId"]));
								if (dataRow4 != null)
								{
									string text2 = UpdateDataToRow(dataTable3.Rows[0], dataRow4);
									if (!string.IsNullOrEmpty(text2))
									{
										return text2;
									}
								}
								else
								{
									dataTable2.ImportRow(dataTable3.Rows[0]);
								}
							}
							dataTable2.AcceptChanges();
							RemoveRowsFromDatatable(list, importDataTableObject.ImportData);
						}
					}
					DataTable dataTable4 = objectAndSubObjectTables.Tables["dtTask"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'dtTask' or table have no columns";
					}
					ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.WBSTemplateTask);
					if (importDataTableObject2 != null)
					{
						string codeCol2 = "";
						DataRow dataRow5 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Code");
						if (dataRow5 != null)
						{
							codeCol2 = Function.ToString(dataRow5["Source"]);
						}
						EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
							where code == Function.ToString(r[codeCol2])
							select r;
						if (source2.Any())
						{
							List<DataRow> list2 = source2.ToList();
							for (int num2 = 0; num2 < list2.Count; num2++)
							{
								DataTable dataTable5 = ImportHelper.PrepareImportData(list2[num2], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
								if (!dataTable5.IsNotNull())
								{
									continue;
								}
								int line2 = Function.ParseInt(dataTable5.Rows[0]["LineId"]);
								DataRow dataRow6 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => line2 == Function.ParseInt(r["LineId"]));
								if (dataRow6 != null)
								{
									string text3 = UpdateDataToRow(dataTable5.Rows[0], dataRow6);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
								}
								else
								{
									dataTable4.ImportRow(dataTable5.Rows[0]);
								}
							}
							dataTable4.AcceptChanges();
							RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
						}
					}
					return HttpClient.WBSTemplate.Update(objectAndSubObjectTables).Result.Message;
				}
				return "Dataset does not contain table 'dtDoc'";
			}
			return "Cannot get WBSTemplate details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObject(DataTable dtWbsTemplate, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataRow dataRow = dtWbsTemplate.Rows[0];
			string code = Function.ToString(dataRow["Code"]);
			dataRow["CreateDate"] = DateTime.Now.Date;
			dataRow["CreateTime"] = DateTime.Now.Hour + ((DateTime.Now.Minute < 10) ? ("0" + DateTime.Now.Minute) : DateTime.Now.Minute.ToString());
			dataRow["UserSign"] = GlobalData.UserID;
			dtWbsTemplate.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtWbsTemplate.TableName = "dtDoc";
			dtWbsTemplate.DataSet?.Tables.Remove(dtWbsTemplate.TableName);
			dataSet.Tables.Add(dtWbsTemplate);
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TASK");
			ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.WBSTemplateTask);
			if (importDataTableObject != null)
			{
				string codeCol = "";
				DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Code");
				if (dataRow2 != null)
				{
					codeCol = Function.ToString(dataRow2["Source"]);
				}
				EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
					where code == Function.ToString(r[codeCol])
					select r;
				if (source.Any())
				{
					List<DataRow> list = source.ToList();
					for (int num = 0; num < list.Count; num++)
					{
						DataTable dataTable = ImportHelper.PrepareImportData(list[num], importDataTableObject.MappingData, importDataTableObject.ImportObject);
						if (dataTable.IsNotNull())
						{
							tableStructure.ImportRow(dataTable.Rows[0]);
						}
					}
					tableStructure.AcceptChanges();
					RemoveRowsFromDatatable(list, importDataTableObject.ImportData);
				}
			}
			tableStructure.TableName = "dtTask";
			tableStructure.DataSet?.Tables.Remove(tableStructure.TableName);
			dataSet.Tables.Add(tableStructure);
			DataTable tableStructure2 = WinFuntions.GetTableStructure("APZ_WBS1");
			ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.WBSTemplateDetails);
			if (importDataTableObject2 != null)
			{
				string codeCol2 = "";
				DataRow dataRow3 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "Code");
				if (dataRow3 != null)
				{
					codeCol2 = Function.ToString(dataRow3["Source"]);
				}
				EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
					where code == Function.ToString(r[codeCol2])
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						DataTable dataTable2 = ImportHelper.PrepareImportData(list2[num2], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
						if (dataTable2.IsNotNull())
						{
							tableStructure2.ImportRow(dataTable2.Rows[0]);
						}
					}
					tableStructure2.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
				}
			}
			tableStructure2.TableName = "dtWBS";
			tableStructure2.DataSet?.Tables.Remove(tableStructure2.TableName);
			dataSet.Tables.Add(tableStructure2);
			return HttpClient.WBSTemplate.Add(dataSet).Result.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
			DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
			DataRow dataRow2 = tableStructure.NewRow();
			dataRow2["empID"] = Function.ParseInt(dataRow["empID"]);
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
				return HttpClient.ImportDataClient.CheckExistWBSTemplate(Function.ToString(dataRow["Code"]));
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
			string code = Function.ToString(dataRow["Code"]);
			DataSet aWBSTemplate = HttpClient.ImportDataClient.GetAWBSTemplate(code);
			DataTable dataTable = aWBSTemplate.Tables[0];
			DataTable dataTable2 = aWBSTemplate.Tables[2];
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = WinFuntions.GetTableStructure("APZ_TASK");
			}
			DataTable dataTable3 = aWBSTemplate.Tables[1];
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = WinFuntions.GetTableStructure("APZ_WBS1");
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dataTable.TableName = "dtDoc";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			dataTable2.TableName = "dtTask";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			dataTable3.TableName = "dtWBS";
			dataTable3.DataSet?.Tables.Remove(dataTable3.TableName);
			dataSet.Tables.Add(dataTable3);
			dataSet.AcceptChanges();
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}
}
