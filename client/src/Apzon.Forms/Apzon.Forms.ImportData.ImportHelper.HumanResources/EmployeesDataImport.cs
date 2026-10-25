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

namespace Apzon.Forms.ImportData.ImportHelper.HumanResources;

internal class EmployeesDataImport : BaseDataImport
{
	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtEmpSource = null)
	{
		try
		{
			int empId;
			if (!(key is DataRow dataRow))
			{
				if (!dtEmpSource.IsNotNull())
				{
					return "No data to update found";
				}
				empId = Function.ParseInt(dtEmpSource.Rows[0]["empID"]);
			}
			else
			{
				empId = Function.ParseInt(dataRow["empID"]);
			}
			DataSet objectAndSubObjectTables = GetObjectAndSubObjectTables(key);
			if (objectAndSubObjectTables != null && objectAndSubObjectTables.Tables.Count == 10 && objectAndSubObjectTables.Tables[0] != null && objectAndSubObjectTables.Tables[1] != null && objectAndSubObjectTables.Tables[2] != null && objectAndSubObjectTables.Tables[3] != null && objectAndSubObjectTables.Tables[4] != null && objectAndSubObjectTables.Tables[5] != null && objectAndSubObjectTables.Tables[6] != null && objectAndSubObjectTables.Tables[7] != null && objectAndSubObjectTables.Tables[8] != null && objectAndSubObjectTables.Tables[9] != null)
			{
				if (objectAndSubObjectTables.Tables.Contains("dtDoc"))
				{
					if (dtEmpSource != null)
					{
						DataTable dataTable = objectAndSubObjectTables.Tables["dtDoc"];
						if (lstDtImportObject[0].ImportObject.Code == Enums.ImportObjectEnum.EmployeesInfo)
						{
							string text = UpdateDataToRow(dtEmpSource.Rows[0], dataTable.Rows[0]);
							if (!string.IsNullOrEmpty(text))
							{
								return text;
							}
						}
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
						dataTable.AcceptChanges();
					}
					DataTable dataTable2 = objectAndSubObjectTables.Tables["dtAbsence"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM1' or table have no columns";
					}
					ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesAbsenceInfo);
					if (importDataTableObject != null)
					{
						string empCol = "";
						DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow2 != null)
						{
							empCol = Function.ToString(dataRow2["Source"]);
						}
						EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol])
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num = 0; num < list.Count; num++)
							{
								DataTable dataTable3 = ImportHelper.PrepareImportData(list[num], importDataTableObject.MappingData, importDataTableObject.ImportObject);
								if (!dataTable3.IsNotNull())
								{
									continue;
								}
								int line = Function.ParseInt(dataTable3.Rows[0]["line"]);
								DataRow dataRow3 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => line == Function.ParseInt(r["line"]));
								if (dataRow3 != null)
								{
									string text2 = UpdateDataToRow(dataTable3.Rows[0], dataRow3);
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
					DataTable dataTable4 = objectAndSubObjectTables.Tables["dtEducation"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM2' or table have no columns";
					}
					ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesEducationInfo);
					if (importDataTableObject2 != null)
					{
						string empCol2 = "";
						DataRow dataRow4 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow4 != null)
						{
							empCol2 = Function.ToString(dataRow4["Source"]);
						}
						EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol2])
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
								int line2 = Function.ParseInt(dataTable5.Rows[0]["line"]);
								DataRow dataRow5 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => line2 == Function.ParseInt(r["line"]));
								if (dataRow5 != null)
								{
									string text3 = UpdateDataToRow(dataTable5.Rows[0], dataRow5);
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
					DataTable dataTable6 = objectAndSubObjectTables.Tables["dtReview"];
					if (dataTable6 == null || dataTable6.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM3' or table have no columns";
					}
					ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesReviewsInfo);
					if (importDataTableObject3 != null)
					{
						string empCol3 = "";
						DataRow dataRow6 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow6 != null)
						{
							empCol3 = Function.ToString(dataRow6["Source"]);
						}
						EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol3])
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num3 = 0; num3 < list3.Count; num3++)
							{
								DataTable dataTable7 = ImportHelper.PrepareImportData(list3[num3], importDataTableObject3.MappingData, importDataTableObject3.ImportObject);
								if (!dataTable7.IsNotNull())
								{
									continue;
								}
								int line3 = Function.ParseInt(dataTable7.Rows[0]["line"]);
								DataRow dataRow7 = dataTable6.AsEnumerable().FirstOrDefault((DataRow r) => line3 == Function.ParseInt(r["line"]));
								if (dataRow7 != null)
								{
									string text4 = UpdateDataToRow(dataTable7.Rows[0], dataRow7);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
								}
								else
								{
									dataTable6.ImportRow(dataTable7.Rows[0]);
								}
							}
							dataTable6.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDataTableObject3.ImportData);
						}
					}
					DataTable dataTable8 = objectAndSubObjectTables.Tables["dtPrevEmp"];
					if (dataTable8 == null || dataTable8.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM4' or table have no columns";
					}
					ImportDataTableObject importDataTableObject4 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo);
					if (importDataTableObject4 != null)
					{
						string empCol4 = "";
						DataRow dataRow8 = importDataTableObject4.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow8 != null)
						{
							empCol4 = Function.ToString(dataRow8["Source"]);
						}
						EnumerableRowCollection<DataRow> source4 = from r in importDataTableObject4.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol4])
							select r;
						if (source4.Any())
						{
							List<DataRow> list4 = source4.ToList();
							for (int num4 = 0; num4 < list4.Count; num4++)
							{
								DataTable dataTable9 = ImportHelper.PrepareImportData(list4[num4], importDataTableObject4.MappingData, importDataTableObject4.ImportObject);
								if (!dataTable9.IsNotNull())
								{
									continue;
								}
								int line4 = Function.ParseInt(dataTable9.Rows[0]["line"]);
								DataRow dataRow9 = dataTable8.AsEnumerable().FirstOrDefault((DataRow r) => line4 == Function.ParseInt(r["line"]));
								if (dataRow9 != null)
								{
									string text5 = UpdateDataToRow(dataTable9.Rows[0], dataRow9);
									if (!string.IsNullOrEmpty(text5))
									{
										return text5;
									}
								}
								else
								{
									dataTable8.ImportRow(dataTable9.Rows[0]);
								}
							}
							dataTable8.AcceptChanges();
							RemoveRowsFromDatatable(list4, importDataTableObject4.ImportData);
						}
					}
					DataTable dataTable10 = objectAndSubObjectTables.Tables["dtRole"];
					if (dataTable10 == null || dataTable10.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM6' or table have no columns";
					}
					ImportDataTableObject importDataTableObject5 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesRolesInfo);
					if (importDataTableObject5 != null)
					{
						string empCol5 = "";
						DataRow dataRow10 = importDataTableObject5.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow10 != null)
						{
							empCol5 = Function.ToString(dataRow10["Source"]);
						}
						EnumerableRowCollection<DataRow> source5 = from r in importDataTableObject5.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol5])
							select r;
						if (source5.Any())
						{
							List<DataRow> list5 = source5.ToList();
							for (int num5 = 0; num5 < list5.Count; num5++)
							{
								DataTable dataTable11 = ImportHelper.PrepareImportData(list5[num5], importDataTableObject5.MappingData, importDataTableObject5.ImportObject);
								if (!dataTable11.IsNotNull())
								{
									continue;
								}
								int line5 = Function.ParseInt(dataTable11.Rows[0]["line"]);
								DataRow dataRow11 = dataTable10.AsEnumerable().FirstOrDefault((DataRow r) => line5 == Function.ParseInt(r["line"]));
								if (dataRow11 != null)
								{
									string text6 = UpdateDataToRow(dataTable11.Rows[0], dataRow11);
									if (!string.IsNullOrEmpty(text6))
									{
										return text6;
									}
								}
								else
								{
									dataTable10.ImportRow(dataTable11.Rows[0]);
								}
							}
							dataTable10.AcceptChanges();
							RemoveRowsFromDatatable(list5, importDataTableObject5.ImportData);
						}
					}
					DataTable dataTable12 = objectAndSubObjectTables.Tables["dtHealth"];
					if (dataTable12 == null || dataTable12.Columns.Count == 0)
					{
						return "Dataset does not contain table 'APZ_OHEH' or table have no columns";
					}
					ImportDataTableObject importDataTableObject6 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesHealthInfo);
					if (importDataTableObject6 != null)
					{
						string empCol6 = "";
						DataRow dataRow12 = importDataTableObject6.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow12 != null)
						{
							empCol6 = Function.ToString(dataRow12["Source"]);
						}
						EnumerableRowCollection<DataRow> source6 = from r in importDataTableObject6.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol6])
							select r;
						if (source6.Any())
						{
							List<DataRow> list6 = source6.ToList();
							for (int num6 = 0; num6 < list6.Count; num6++)
							{
								DataTable dataTable13 = ImportHelper.PrepareImportData(list6[num6], importDataTableObject6.MappingData, importDataTableObject6.ImportObject);
								if (!dataTable13.IsNotNull())
								{
									continue;
								}
								int healthId = Function.ParseInt(dataTable13.Rows[0]["healthID"]);
								DataRow dataRow13 = dataTable12.AsEnumerable().FirstOrDefault((DataRow r) => healthId == Function.ParseInt(r["healthID"]));
								if (dataRow13 != null)
								{
									string text7 = UpdateDataToRow(dataTable13.Rows[0], dataRow13);
									if (!string.IsNullOrEmpty(text7))
									{
										return text7;
									}
								}
								else
								{
									dataTable12.ImportRow(dataTable13.Rows[0]);
								}
							}
							dataTable12.AcceptChanges();
							RemoveRowsFromDatatable(list6, importDataTableObject6.ImportData);
						}
					}
					DataTable dataTable14 = objectAndSubObjectTables.Tables["dtContract"];
					if (dataTable14 == null || dataTable14.Columns.Count == 0)
					{
						return "Dataset does not contain table 'APZ_OHEC' or table have no columns";
					}
					ImportDataTableObject importDataTableObject7 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesContractsInfo);
					if (importDataTableObject7 != null)
					{
						string empCol7 = "";
						DataRow dataRow14 = importDataTableObject7.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
						if (dataRow14 != null)
						{
							empCol7 = Function.ToString(dataRow14["Source"]);
						}
						EnumerableRowCollection<DataRow> source7 = from r in importDataTableObject7.ImportData.AsEnumerable()
							where empId == Function.ParseInt(r[empCol7])
							select r;
						if (source7.Any())
						{
							List<DataRow> list7 = source7.ToList();
							for (int num7 = 0; num7 < list7.Count; num7++)
							{
								DataTable dataTable15 = ImportHelper.PrepareImportData(list7[num7], importDataTableObject7.MappingData, importDataTableObject7.ImportObject);
								if (!dataTable15.IsNotNull())
								{
									continue;
								}
								int contractId = Function.ParseInt(dataTable15.Rows[0]["contractID"]);
								DataRow dataRow15 = dataTable14.AsEnumerable().FirstOrDefault((DataRow r) => contractId == Function.ParseInt(r["contractID"]));
								if (dataRow15 != null)
								{
									string text8 = UpdateDataToRow(dataTable15.Rows[0], dataRow15);
									if (!string.IsNullOrEmpty(text8))
									{
										return text8;
									}
								}
								else
								{
									dataTable14.ImportRow(dataTable15.Rows[0]);
								}
							}
							dataTable14.AcceptChanges();
							RemoveRowsFromDatatable(list7, importDataTableObject7.ImportData);
						}
					}
					return HttpClient.ImportDataClient.UpdateEmployees(objectAndSubObjectTables);
				}
				return "Dataset does not contain table 'dtDoc'";
			}
			return "Cannot get Employees details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObject(DataTable dtDoc, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataRow dataRow = dtDoc.Rows[0];
			int empId = Function.ParseInt(dataRow["empID"]);
			dataRow["UserSign"] = GlobalData.UserID;
			dtDoc.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtDoc.TableName = "dtDoc";
			dtDoc.DataSet?.Tables.Remove(dtDoc.TableName);
			dataSet.Tables.Add(dtDoc);
			DataTable tableStructure = WinFuntions.GetTableStructure("ATC1");
			tableStructure.TableName = "AttFile";
			tableStructure.DataSet?.Tables.Remove(tableStructure.TableName);
			dataSet.Tables.Add(tableStructure);
			DataTable tableStructure2 = WinFuntions.GetTableStructure("HEM1");
			ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesAbsenceInfo);
			if (importDataTableObject != null)
			{
				string empCol = "";
				DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow2 != null)
				{
					empCol = Function.ToString(dataRow2["Source"]);
				}
				EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol])
					select r;
				if (source.Any())
				{
					List<DataRow> list = source.ToList();
					for (int num = 0; num < list.Count; num++)
					{
						DataTable dataTable = ImportHelper.PrepareImportData(list[num], importDataTableObject.MappingData, importDataTableObject.ImportObject);
						if (dataTable.IsNotNull())
						{
							tableStructure2.ImportRow(dataTable.Rows[0]);
						}
					}
					tableStructure2.AcceptChanges();
					RemoveRowsFromDatatable(list, importDataTableObject.ImportData);
				}
			}
			tableStructure2.TableName = "dtAbsence";
			tableStructure2.DataSet?.Tables.Remove(tableStructure2.TableName);
			dataSet.Tables.Add(tableStructure2);
			DataTable tableStructure3 = WinFuntions.GetTableStructure("HEM2");
			ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesEducationInfo);
			if (importDataTableObject2 != null)
			{
				string empCol2 = "";
				DataRow dataRow3 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow3 != null)
				{
					empCol2 = Function.ToString(dataRow3["Source"]);
				}
				EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol2])
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						DataTable dataTable2 = ImportHelper.PrepareImportData(list2[num2], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
						if (dataTable2.IsNotNull())
						{
							tableStructure3.ImportRow(dataTable2.Rows[0]);
						}
					}
					tableStructure3.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
				}
			}
			tableStructure3.TableName = "dtEducation";
			tableStructure3.DataSet?.Tables.Remove(tableStructure3.TableName);
			dataSet.Tables.Add(tableStructure3);
			DataTable tableStructure4 = WinFuntions.GetTableStructure("HEM3");
			ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesReviewsInfo);
			if (importDataTableObject3 != null)
			{
				string empCol3 = "";
				DataRow dataRow4 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow4 != null)
				{
					empCol3 = Function.ToString(dataRow4["Source"]);
				}
				EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol3])
					select r;
				if (source3.Any())
				{
					List<DataRow> list3 = source3.ToList();
					for (int num3 = 0; num3 < list3.Count; num3++)
					{
						DataTable dataTable3 = ImportHelper.PrepareImportData(list3[num3], importDataTableObject3.MappingData, importDataTableObject3.ImportObject);
						if (dataTable3.IsNotNull())
						{
							tableStructure4.ImportRow(dataTable3.Rows[0]);
						}
					}
					tableStructure4.AcceptChanges();
					RemoveRowsFromDatatable(list3, importDataTableObject3.ImportData);
				}
			}
			tableStructure4.TableName = "dtReview";
			tableStructure4.DataSet?.Tables.Remove(tableStructure4.TableName);
			dataSet.Tables.Add(tableStructure4);
			DataTable tableStructure5 = WinFuntions.GetTableStructure("HEM4");
			ImportDataTableObject importDataTableObject4 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo);
			if (importDataTableObject4 != null)
			{
				string empCol4 = "";
				DataRow dataRow5 = importDataTableObject4.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow5 != null)
				{
					empCol4 = Function.ToString(dataRow5["Source"]);
				}
				EnumerableRowCollection<DataRow> source4 = from r in importDataTableObject4.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol4])
					select r;
				if (source4.Any())
				{
					List<DataRow> list4 = source4.ToList();
					for (int num4 = 0; num4 < list4.Count; num4++)
					{
						DataTable dataTable4 = ImportHelper.PrepareImportData(list4[num4], importDataTableObject4.MappingData, importDataTableObject4.ImportObject);
						if (dataTable4.IsNotNull())
						{
							tableStructure5.ImportRow(dataTable4.Rows[0]);
						}
					}
					tableStructure5.AcceptChanges();
					RemoveRowsFromDatatable(list4, importDataTableObject4.ImportData);
				}
			}
			tableStructure5.TableName = "dtPrevEmp";
			tableStructure5.DataSet?.Tables.Remove(tableStructure5.TableName);
			dataSet.Tables.Add(tableStructure5);
			DataTable tableStructure6 = WinFuntions.GetTableStructure("HEM6");
			ImportDataTableObject importDataTableObject5 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesRolesInfo);
			if (importDataTableObject5 != null)
			{
				string empCol5 = "";
				DataRow dataRow6 = importDataTableObject5.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow6 != null)
				{
					empCol5 = Function.ToString(dataRow6["Source"]);
				}
				EnumerableRowCollection<DataRow> source5 = from r in importDataTableObject5.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol5])
					select r;
				if (source5.Any())
				{
					List<DataRow> list5 = source5.ToList();
					for (int num5 = 0; num5 < list5.Count; num5++)
					{
						DataTable dataTable5 = ImportHelper.PrepareImportData(list5[num5], importDataTableObject5.MappingData, importDataTableObject5.ImportObject);
						if (dataTable5.IsNotNull())
						{
							tableStructure6.ImportRow(dataTable5.Rows[0]);
						}
					}
					tableStructure6.AcceptChanges();
					RemoveRowsFromDatatable(list5, importDataTableObject5.ImportData);
				}
			}
			tableStructure6.TableName = "dtRole";
			tableStructure6.DataSet?.Tables.Remove(tableStructure6.TableName);
			dataSet.Tables.Add(tableStructure6);
			DataTable tableStructure7 = WinFuntions.GetTableStructure("HTM1");
			tableStructure7.TableName = "dtTeams";
			tableStructure7.DataSet?.Tables.Remove(tableStructure7.TableName);
			dataSet.Tables.Add(tableStructure7);
			DataTable tableStructure8 = WinFuntions.GetTableStructure("APZ_OHEH");
			ImportDataTableObject importDataTableObject6 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesHealthInfo);
			if (importDataTableObject6 != null)
			{
				string empCol6 = "";
				DataRow dataRow7 = importDataTableObject6.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow7 != null)
				{
					empCol6 = Function.ToString(dataRow7["Source"]);
				}
				EnumerableRowCollection<DataRow> source6 = from r in importDataTableObject6.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol6])
					select r;
				if (source6.Any())
				{
					List<DataRow> list6 = source6.ToList();
					for (int num6 = 0; num6 < list6.Count; num6++)
					{
						DataTable dataTable6 = ImportHelper.PrepareImportData(list6[num6], importDataTableObject6.MappingData, importDataTableObject6.ImportObject);
						if (dataTable6.IsNotNull())
						{
							tableStructure8.ImportRow(dataTable6.Rows[0]);
						}
					}
					tableStructure8.AcceptChanges();
					RemoveRowsFromDatatable(list6, importDataTableObject6.ImportData);
				}
			}
			tableStructure8.TableName = "dtHealth";
			tableStructure8.DataSet?.Tables.Remove(tableStructure8.TableName);
			dataSet.Tables.Add(tableStructure8);
			DataTable tableStructure9 = WinFuntions.GetTableStructure("APZ_OHEC");
			ImportDataTableObject importDataTableObject7 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.EmployeesAbsenceInfo);
			if (importDataTableObject7 != null)
			{
				string empCol7 = "";
				DataRow dataRow8 = importDataTableObject7.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "empID");
				if (dataRow8 != null)
				{
					empCol7 = Function.ToString(dataRow8["Source"]);
				}
				EnumerableRowCollection<DataRow> source7 = from r in importDataTableObject7.ImportData.AsEnumerable()
					where empId == Function.ParseInt(r[empCol7])
					select r;
				if (source7.Any())
				{
					List<DataRow> list7 = source7.ToList();
					for (int num7 = 0; num7 < list7.Count; num7++)
					{
						DataTable dataTable7 = ImportHelper.PrepareImportData(list7[num7], importDataTableObject7.MappingData, importDataTableObject7.ImportObject);
						if (dataTable7.IsNotNull())
						{
							tableStructure9.ImportRow(dataTable7.Rows[0]);
						}
					}
					tableStructure9.AcceptChanges();
					RemoveRowsFromDatatable(list7, importDataTableObject7.ImportData);
				}
			}
			tableStructure9.TableName = "dtContract";
			tableStructure9.DataSet?.Tables.Remove(tableStructure9.TableName);
			dataSet.Tables.Add(tableStructure9);
			return HttpClient.ImportDataClient.AddEmployees(dataSet);
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
				return HttpClient.ImportDataClient.CheckExistEmployees(Function.ParseInt(dataRow["empID"]));
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
			int empID = Function.ParseInt(dataRow["empID"]);
			DataTable aEmployees = HttpClient.ImportDataClient.GetAEmployees(empID);
			if (!aEmployees.IsNotNull())
			{
				return new DataSet();
			}
			int atcEntry = Function.ParseInt(aEmployees.Rows[0]["AtcEntry"]);
			DataTable dataTable = HttpClient.ImportDataClient.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			DataTable dataTable2 = HttpClient.ImportDataClient.GetEmployeeAbsence(empID);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = WinFuntions.GetTableStructure("HEM1");
			}
			DataTable dataTable3 = HttpClient.ImportDataClient.GetEmployeeEducation(empID);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = WinFuntions.GetTableStructure("HEM2");
			}
			DataTable dataTable4 = HttpClient.ImportDataClient.GetEmployeeReview(empID);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = WinFuntions.GetTableStructure("HEM3");
			}
			DataTable dataTable5 = HttpClient.ImportDataClient.GetEmployeePrevEmp(empID);
			if (dataTable5 == null || dataTable5.Rows.Count == 0)
			{
				dataTable5 = WinFuntions.GetTableStructure("HEM4");
			}
			DataTable dataTable6 = HttpClient.ImportDataClient.GetEmployeeRole(empID);
			if (dataTable6 == null || dataTable6.Rows.Count == 0)
			{
				dataTable6 = WinFuntions.GetTableStructure("HEM6");
			}
			DataTable dataTable7 = HttpClient.ImportDataClient.GetEmployeeTeams(empID);
			if (dataTable7 == null || dataTable7.Rows.Count == 0)
			{
				dataTable7 = WinFuntions.GetTableStructure("HTM1");
			}
			DataTable dataTable8 = HttpClient.ImportDataClient.GetEmployeeHealth(empID);
			if (dataTable8 == null || dataTable8.Rows.Count == 0)
			{
				dataTable8 = WinFuntions.GetTableStructure("APZ_OHEH");
			}
			DataTable dataTable9 = HttpClient.ImportDataClient.GetEmployeeContract(empID);
			if (dataTable9 == null || dataTable9.Rows.Count == 0)
			{
				dataTable9 = WinFuntions.GetTableStructure("APZ_OHEC");
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			aEmployees.TableName = "dtDoc";
			aEmployees.DataSet?.Tables.Remove(aEmployees.TableName);
			dataSet.Tables.Add(aEmployees);
			dataTable.TableName = "AttFile";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			dataTable2.TableName = "dtAbsence";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			dataTable3.TableName = "dtEducation";
			dataTable3.DataSet?.Tables.Remove(dataTable3.TableName);
			dataSet.Tables.Add(dataTable3);
			dataTable4.TableName = "dtReview";
			dataTable4.DataSet?.Tables.Remove(dataTable4.TableName);
			dataSet.Tables.Add(dataTable4);
			dataTable5.TableName = "dtPrevEmp";
			dataTable5.DataSet?.Tables.Remove(dataTable5.TableName);
			dataSet.Tables.Add(dataTable5);
			dataTable6.TableName = "dtRole";
			dataTable6.DataSet?.Tables.Remove(dataTable6.TableName);
			dataSet.Tables.Add(dataTable6);
			dataTable7.TableName = "dtTeams";
			dataTable7.DataSet?.Tables.Remove(dataTable7.TableName);
			dataSet.Tables.Add(dataTable7);
			dataTable8.TableName = "dtHealth";
			dataTable8.DataSet?.Tables.Remove(dataTable8.TableName);
			dataSet.Tables.Add(dataTable8);
			dataTable9.TableName = "dtContract";
			dataTable9.DataSet?.Tables.Remove(dataTable9.TableName);
			dataSet.Tables.Add(dataTable9);
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
