using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.ImportData;

namespace Apzon.Services.Repository.ImportData.HumanResources;

public class ImportDataEmployees : BaseImportData
{
	public ImportDataEmployees(string connection, int userID, IUnitOfWork unitOfWork)
		: base(connection, userID, unitOfWork)
	{
	}

	public string ImportEmployees(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				return AddEmployees(dataTable, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateEmployees(LstDTImportObject, 0, dataTable);
			case Enums.ImportActionEnum.AddUpdate:
			{
				int empID = Function.ParseInt(dataTable.Rows[0]["empID"]);
				if (_unitOfWork.ImportData.CheckExistEmployees(empID))
				{
					return UpdateEmployees(LstDTImportObject, 0, dataTable);
				}
				return AddEmployees(dataTable, LstDTImportObject);
			}
			default:
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateEmployees(List<ImportDTImpl> LstDTImportObject, int empID = 0, DataTable dtEmpSource = null)
	{
		try
		{
			if (empID == 0 && dtEmpSource.IsNotNull())
			{
				empID = Function.ParseInt(dtEmpSource.Rows[0]["empID"]);
			}
			DataSet employeesAndSubObjectTables = GetEmployeesAndSubObjectTables(empID);
			if (employeesAndSubObjectTables != null && employeesAndSubObjectTables.Tables.Count == 10 && employeesAndSubObjectTables.Tables[0] != null && employeesAndSubObjectTables.Tables[1] != null && employeesAndSubObjectTables.Tables[2] != null && employeesAndSubObjectTables.Tables[3] != null && employeesAndSubObjectTables.Tables[4] != null && employeesAndSubObjectTables.Tables[5] != null && employeesAndSubObjectTables.Tables[6] != null && employeesAndSubObjectTables.Tables[7] != null && employeesAndSubObjectTables.Tables[8] != null && employeesAndSubObjectTables.Tables[9] != null)
			{
				if (employeesAndSubObjectTables.Tables.Contains("dtDoc"))
				{
					if (dtEmpSource != null)
					{
						DataTable dataTable = employeesAndSubObjectTables.Tables["dtDoc"];
						string text = UpdateDataToRow(dtEmpSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = _userID;
						dataTable.AcceptChanges();
					}
					DataTable dataTable2 = employeesAndSubObjectTables.Tables["dtAbsence"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM1' or table have no columns";
					}
					ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesAbsenceInfo);
					if (importDTImpl != null)
					{
						EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num = 0; num < list.Count; num++)
							{
								int line = Function.ParseInt(list[num]["line"]);
								DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => line == Function.ParseInt(r["line"]));
								if (dataRow != null)
								{
									string text2 = UpdateDataToRow(list[num], dataRow);
									if (!string.IsNullOrEmpty(text2))
									{
										return text2;
									}
								}
								else
								{
									dataTable2.ImportRow(list[num]);
								}
							}
							dataTable2.AcceptChanges();
							RemoveRowsFromDatatable(list, importDTImpl.DTData);
						}
					}
					DataTable dataTable3 = employeesAndSubObjectTables.Tables["dtEducation"];
					if (dataTable3 == null || dataTable3.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM2' or table have no columns";
					}
					ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesEducationInfo);
					if (importDTImpl2 != null)
					{
						EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source2.Any())
						{
							List<DataRow> list2 = source2.ToList();
							for (int num2 = 0; num2 < list2.Count; num2++)
							{
								int line2 = Function.ParseInt(list2[num2]["line"]);
								DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow r) => line2 == Function.ParseInt(r["line"]));
								if (dataRow2 != null)
								{
									string text3 = UpdateDataToRow(list2[num2], dataRow2);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
								}
								else
								{
									dataTable3.ImportRow(list2[num2]);
								}
							}
							dataTable3.AcceptChanges();
							RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
						}
					}
					DataTable dataTable4 = employeesAndSubObjectTables.Tables["dtReview"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM3' or table have no columns";
					}
					ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesReviewsInfo);
					if (importDTImpl3 != null)
					{
						EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num3 = 0; num3 < list3.Count; num3++)
							{
								int line3 = Function.ParseInt(list3[num3]["line"]);
								DataRow dataRow3 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => line3 == Function.ParseInt(r["line"]));
								if (dataRow3 != null)
								{
									string text4 = UpdateDataToRow(list3[num3], dataRow3);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
								}
								else
								{
									dataTable4.ImportRow(list3[num3]);
								}
							}
							dataTable4.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDTImpl3.DTData);
						}
					}
					DataTable dataTable5 = employeesAndSubObjectTables.Tables["dtPrevEmp"];
					if (dataTable5 == null || dataTable5.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM4' or table have no columns";
					}
					ImportDTImpl importDTImpl4 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo);
					if (importDTImpl4 != null)
					{
						EnumerableRowCollection<DataRow> source4 = from r in importDTImpl4.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source4.Any())
						{
							List<DataRow> list4 = source4.ToList();
							for (int num4 = 0; num4 < list4.Count; num4++)
							{
								int line4 = Function.ParseInt(list4[num4]["line"]);
								DataRow dataRow4 = dataTable5.AsEnumerable().FirstOrDefault((DataRow r) => line4 == Function.ParseInt(r["line"]));
								if (dataRow4 != null)
								{
									string text5 = UpdateDataToRow(list4[num4], dataRow4);
									if (!string.IsNullOrEmpty(text5))
									{
										return text5;
									}
								}
								else
								{
									dataTable5.ImportRow(list4[num4]);
								}
							}
							dataTable5.AcceptChanges();
							RemoveRowsFromDatatable(list4, importDTImpl4.DTData);
						}
					}
					DataTable dataTable6 = employeesAndSubObjectTables.Tables["dtRole"];
					if (dataTable6 == null || dataTable6.Columns.Count == 0)
					{
						return "Dataset does not contain table 'HEM6' or table have no columns";
					}
					ImportDTImpl importDTImpl5 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesRolesInfo);
					if (importDTImpl5 != null)
					{
						EnumerableRowCollection<DataRow> source5 = from r in importDTImpl5.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source5.Any())
						{
							List<DataRow> list5 = source5.ToList();
							for (int num5 = 0; num5 < list5.Count; num5++)
							{
								int line5 = Function.ParseInt(list5[num5]["line"]);
								DataRow dataRow5 = dataTable6.AsEnumerable().FirstOrDefault((DataRow r) => line5 == Function.ParseInt(r["line"]));
								if (dataRow5 != null)
								{
									string text6 = UpdateDataToRow(list5[num5], dataRow5);
									if (!string.IsNullOrEmpty(text6))
									{
										return text6;
									}
								}
								else
								{
									dataTable6.ImportRow(list5[num5]);
								}
							}
							dataTable6.AcceptChanges();
							RemoveRowsFromDatatable(list5, importDTImpl5.DTData);
						}
					}
					DataTable dataTable7 = employeesAndSubObjectTables.Tables["dtHealth"];
					if (dataTable7 == null || dataTable7.Columns.Count == 0)
					{
						return "Dataset does not contain table 'APZ_OHEH' or table have no columns";
					}
					ImportDTImpl importDTImpl6 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesHealthInfo);
					if (importDTImpl6 != null)
					{
						EnumerableRowCollection<DataRow> source6 = from r in importDTImpl6.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source6.Any())
						{
							List<DataRow> list6 = source6.ToList();
							for (int num6 = 0; num6 < list6.Count; num6++)
							{
								int healthID = Function.ParseInt(list6[num6]["healthID"]);
								DataRow dataRow6 = dataTable7.AsEnumerable().FirstOrDefault((DataRow r) => healthID == Function.ParseInt(r["healthID"]));
								if (dataRow6 != null)
								{
									string text7 = UpdateDataToRow(list6[num6], dataRow6);
									if (!string.IsNullOrEmpty(text7))
									{
										return text7;
									}
								}
								else
								{
									dataTable7.ImportRow(list6[num6]);
								}
							}
							dataTable7.AcceptChanges();
							RemoveRowsFromDatatable(list6, importDTImpl6.DTData);
						}
					}
					DataTable dataTable8 = employeesAndSubObjectTables.Tables["dtContract"];
					if (dataTable8 == null || dataTable8.Columns.Count == 0)
					{
						return "Dataset does not contain table 'APZ_OHEC' or table have no columns";
					}
					ImportDTImpl importDTImpl7 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesContractsInfo);
					if (importDTImpl7 != null)
					{
						EnumerableRowCollection<DataRow> source7 = from r in importDTImpl7.DTData.AsEnumerable()
							where empID == Function.ParseInt(r["empID"])
							select r;
						if (source7.Any())
						{
							List<DataRow> list7 = source7.ToList();
							for (int num7 = 0; num7 < list7.Count; num7++)
							{
								int contractID = Function.ParseInt(list7[num7]["contractID"]);
								DataRow dataRow7 = dataTable8.AsEnumerable().FirstOrDefault((DataRow r) => contractID == Function.ParseInt(r["contractID"]));
								if (dataRow7 != null)
								{
									string text8 = UpdateDataToRow(list7[num7], dataRow7);
									if (!string.IsNullOrEmpty(text8))
									{
										return text8;
									}
								}
								else
								{
									dataTable8.ImportRow(list7[num7]);
								}
							}
							dataTable8.AcceptChanges();
							RemoveRowsFromDatatable(list7, importDTImpl7.DTData);
						}
					}
					return _unitOfWork.ImportData.UpdateEmployees(employeesAndSubObjectTables);
				}
				return "Dataset does not contain table 'dtDoc'";
			}
			return "Cannot get Employees details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddEmployees(DataTable dtDoc, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataRow dataRow = dtDoc.Rows[0];
			int empID = Function.ParseInt(dataRow["empID"]);
			dataRow["UserSign"] = _userID;
			dtDoc.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtDoc.TableName = "dtDoc";
			dtDoc.DataSet?.Tables.Remove(dtDoc.TableName);
			dataSet.Tables.Add(dtDoc);
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			bPTableStructure.TableName = "AttFile";
			bPTableStructure.DataSet?.Tables.Remove(bPTableStructure.TableName);
			dataSet.Tables.Add(bPTableStructure);
			DataTable bPTableStructure2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM1");
			ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesAbsenceInfo);
			if (importDTImpl != null)
			{
				EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source.Any())
				{
					List<DataRow> list = source.ToList();
					for (int num = 0; num < list.Count; num++)
					{
						bPTableStructure2.ImportRow(list[num]);
					}
					bPTableStructure2.AcceptChanges();
					RemoveRowsFromDatatable(list, importDTImpl.DTData);
				}
			}
			bPTableStructure2.TableName = "dtAbsence";
			bPTableStructure2.DataSet?.Tables.Remove(bPTableStructure2.TableName);
			dataSet.Tables.Add(bPTableStructure2);
			DataTable bPTableStructure3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM2");
			ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesEducationInfo);
			if (importDTImpl2 != null)
			{
				EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						bPTableStructure3.ImportRow(list2[num2]);
					}
					bPTableStructure3.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
				}
			}
			bPTableStructure3.TableName = "dtEducation";
			bPTableStructure3.DataSet?.Tables.Remove(bPTableStructure3.TableName);
			dataSet.Tables.Add(bPTableStructure3);
			DataTable bPTableStructure4 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM3");
			ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesReviewsInfo);
			if (importDTImpl3 != null)
			{
				EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source3.Any())
				{
					List<DataRow> list3 = source3.ToList();
					for (int num3 = 0; num3 < list3.Count; num3++)
					{
						bPTableStructure4.ImportRow(list3[num3]);
					}
					bPTableStructure4.AcceptChanges();
					RemoveRowsFromDatatable(list3, importDTImpl3.DTData);
				}
			}
			bPTableStructure4.TableName = "dtReview";
			bPTableStructure4.DataSet?.Tables.Remove(bPTableStructure4.TableName);
			dataSet.Tables.Add(bPTableStructure4);
			DataTable bPTableStructure5 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM4");
			ImportDTImpl importDTImpl4 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo);
			if (importDTImpl4 != null)
			{
				EnumerableRowCollection<DataRow> source4 = from r in importDTImpl4.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source4.Any())
				{
					List<DataRow> list4 = source4.ToList();
					for (int num4 = 0; num4 < list4.Count; num4++)
					{
						bPTableStructure5.ImportRow(list4[num4]);
					}
					bPTableStructure5.AcceptChanges();
					RemoveRowsFromDatatable(list4, importDTImpl4.DTData);
				}
			}
			bPTableStructure5.TableName = "dtPrevEmp";
			bPTableStructure5.DataSet?.Tables.Remove(bPTableStructure5.TableName);
			dataSet.Tables.Add(bPTableStructure5);
			DataTable bPTableStructure6 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM6");
			ImportDTImpl importDTImpl5 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesRolesInfo);
			if (importDTImpl5 != null)
			{
				EnumerableRowCollection<DataRow> source5 = from r in importDTImpl5.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source5.Any())
				{
					List<DataRow> list5 = source5.ToList();
					for (int num5 = 0; num5 < list5.Count; num5++)
					{
						bPTableStructure6.ImportRow(list5[num5]);
					}
					bPTableStructure6.AcceptChanges();
					RemoveRowsFromDatatable(list5, importDTImpl5.DTData);
				}
			}
			bPTableStructure6.TableName = "dtRole";
			bPTableStructure6.DataSet?.Tables.Remove(bPTableStructure6.TableName);
			dataSet.Tables.Add(bPTableStructure6);
			DataTable bPTableStructure7 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HTM1");
			bPTableStructure7.TableName = "dtTeams";
			bPTableStructure7.DataSet?.Tables.Remove(bPTableStructure7.TableName);
			dataSet.Tables.Add(bPTableStructure7);
			DataTable bPTableStructure8 = _unitOfWork.BusinessMasterData.GetBPTableStructure("APZ_OHEH");
			ImportDTImpl importDTImpl6 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesHealthInfo);
			if (importDTImpl6 != null)
			{
				EnumerableRowCollection<DataRow> source6 = from r in importDTImpl6.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source6.Any())
				{
					List<DataRow> list6 = source6.ToList();
					for (int num6 = 0; num6 < list6.Count; num6++)
					{
						bPTableStructure8.ImportRow(list6[num6]);
					}
					bPTableStructure8.AcceptChanges();
					RemoveRowsFromDatatable(list6, importDTImpl6.DTData);
				}
			}
			bPTableStructure8.TableName = "dtHealth";
			bPTableStructure8.DataSet?.Tables.Remove(bPTableStructure8.TableName);
			dataSet.Tables.Add(bPTableStructure8);
			DataTable bPTableStructure9 = _unitOfWork.BusinessMasterData.GetBPTableStructure("APZ_OHEC");
			ImportDTImpl importDTImpl7 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.EmployeesContractsInfo);
			if (importDTImpl7 != null)
			{
				EnumerableRowCollection<DataRow> source7 = from r in importDTImpl7.DTData.AsEnumerable()
					where empID == Function.ParseInt(r["empID"])
					select r;
				if (source7.Any())
				{
					List<DataRow> list7 = source7.ToList();
					for (int num7 = 0; num7 < list7.Count; num7++)
					{
						bPTableStructure9.ImportRow(list7[num7]);
					}
					bPTableStructure9.AcceptChanges();
					RemoveRowsFromDatatable(list7, importDTImpl7.DTData);
				}
			}
			bPTableStructure9.TableName = "dtContract";
			bPTableStructure9.DataSet?.Tables.Remove(bPTableStructure9.TableName);
			dataSet.Tables.Add(bPTableStructure9);
			return _unitOfWork.ImportData.AddEmployees(dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string ImportEmployeesChildObject(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			int empID = Function.ParseInt(dataTable.Rows[0]["empID"]);
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				return AddEmployeesFromChilds(empID, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateEmployees(LstDTImportObject, empID);
			case Enums.ImportActionEnum.AddUpdate:
				if (_unitOfWork.ImportData.CheckExistEmployees(empID))
				{
					return UpdateEmployees(LstDTImportObject, empID);
				}
				return AddEmployeesFromChilds(empID, LstDTImportObject);
			default:
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddEmployeesFromChilds(int empID, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("OHEM");
			DataRow dataRow = bPTableStructure.NewRow();
			dataRow["empID"] = empID;
			bPTableStructure.Rows.Add(dataRow);
			bPTableStructure.AcceptChanges();
			return AddEmployees(bPTableStructure, LstDTImportObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataSet GetEmployeesAndSubObjectTables(int empID)
	{
		try
		{
			DataTable aEmployees = _unitOfWork.ImportData.GetAEmployees(empID);
			int atcEntry = Function.ParseInt(aEmployees.Rows[0]["AtcEntry"]);
			DataTable dataTable = _unitOfWork.ImportData.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			}
			DataTable dataTable2 = _unitOfWork.ImportData.GetEmployeeAbsence(empID);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM1");
			}
			DataTable dataTable3 = _unitOfWork.ImportData.GetEmployeeEducation(empID);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM2");
			}
			DataTable dataTable4 = _unitOfWork.ImportData.GetEmployeeReview(empID);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM3");
			}
			DataTable dataTable5 = _unitOfWork.ImportData.GetEmployeePrevEmp(empID);
			if (dataTable5 == null || dataTable5.Rows.Count == 0)
			{
				dataTable5 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM4");
			}
			DataTable dataTable6 = _unitOfWork.ImportData.GetEmployeeRole(empID);
			if (dataTable6 == null || dataTable6.Rows.Count == 0)
			{
				dataTable6 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HEM6");
			}
			DataTable dataTable7 = _unitOfWork.ImportData.GetEmployeeTeams(empID);
			if (dataTable7 == null || dataTable7.Rows.Count == 0)
			{
				dataTable7 = _unitOfWork.BusinessMasterData.GetBPTableStructure("HTM1");
			}
			DataTable dataTable8 = _unitOfWork.ImportData.GetEmployeeHealth(empID);
			if (dataTable8 == null || dataTable8.Rows.Count == 0)
			{
				dataTable8 = _unitOfWork.BusinessMasterData.GetBPTableStructure("APZ_OHEH");
			}
			DataTable dataTable9 = _unitOfWork.ImportData.GetEmployeeContract(empID);
			if (dataTable9 == null || dataTable9.Rows.Count == 0)
			{
				dataTable9 = _unitOfWork.BusinessMasterData.GetBPTableStructure("APZ_OHEC");
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
			return new DataSet();
		}
	}
}
