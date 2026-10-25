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

namespace Apzon.Forms.ImportData.ImportHelper.BusinessPartners;

internal class BpMasterDataImport : BaseDataImport
{
	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtBpSource = null)
	{
		try
		{
			string cardCode;
			if (!(key is DataRow dataRow))
			{
				if (!dtBpSource.IsNotNull())
				{
					return "No data to update found";
				}
				cardCode = Function.ToString(dtBpSource.Rows[0]["CardCode"]);
			}
			else
			{
				cardCode = Function.ToString(dataRow["CardCode"]);
			}
			DataSet objectAndSubObjectTables = GetObjectAndSubObjectTables(key);
			if (objectAndSubObjectTables != null && objectAndSubObjectTables.Tables.Count == 7 && objectAndSubObjectTables.Tables[0] != null && objectAndSubObjectTables.Tables[1] != null && objectAndSubObjectTables.Tables[2] != null && objectAndSubObjectTables.Tables[3] != null && objectAndSubObjectTables.Tables[4] != null && objectAndSubObjectTables.Tables[5] != null && objectAndSubObjectTables.Tables[6] != null)
			{
				if (objectAndSubObjectTables.Tables.Contains("BP"))
				{
					if (dtBpSource != null)
					{
						DataTable dataTable = objectAndSubObjectTables.Tables["BP"];
						int num = Function.ParseInt(dataTable.Rows[0]["Series"]);
						string text = UpdateDataToRow(dtBpSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["Series"] = num;
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
						dataTable.AcceptChanges();
					}
					DataTable dataTable2 = objectAndSubObjectTables.Tables["Address"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'CRD1' or table have no columns";
					}
					ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPAddresses);
					if (importDataTableObject != null)
					{
						string cardCol = "";
						DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
						if (dataRow2 != null)
						{
							cardCol = Function.ToString(dataRow2["Source"]);
						}
						EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r[cardCol]), ignoreCase: true) == 0
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								DataTable dataTable3 = ImportHelper.PrepareImportData(list[num2], importDataTableObject.MappingData, importDataTableObject.ImportObject);
								if (!dataTable3.IsNotNull())
								{
									continue;
								}
								string address = Function.ToString(dataTable3.Rows[0]["Address"]);
								string adresType = Function.ToString(dataTable3.Rows[0]["AdresType"]);
								DataRow dataRow3 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(address, Function.ToString(r["Address"]), ignoreCase: true) == 0 && string.Compare(adresType, Function.ToString(r["AdresType"]), ignoreCase: true) == 0);
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
					DataTable dataTable4 = objectAndSubObjectTables.Tables["CntPrsn"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OCPR' or table have no columns";
					}
					ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.Contacts);
					if (importDataTableObject2 != null)
					{
						string cardCol2 = "";
						DataRow dataRow4 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
						if (dataRow4 != null)
						{
							cardCol2 = Function.ToString(dataRow4["Source"]);
						}
						EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r[cardCol2]), ignoreCase: true) == 0
							select r;
						if (source2.Any())
						{
							List<DataRow> list2 = source2.ToList();
							for (int num3 = 0; num3 < list2.Count; num3++)
							{
								DataTable dataTable5 = ImportHelper.PrepareImportData(list2[num3], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
								if (!dataTable5.IsNotNull())
								{
									continue;
								}
								string name = Function.ToString(dataTable5.Rows[0]["Name"]);
								DataRow dataRow5 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(name, Function.ToString(r["Name"]), ignoreCase: true) == 0);
								if (dataRow5 != null)
								{
									int num4 = Function.ParseInt(dataRow5["CntctCode"]);
									string text3 = UpdateDataToRow(dataTable5.Rows[0], dataRow5);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
									dataRow5["CntctCode"] = num4;
								}
								else
								{
									dataTable5.Rows[0]["CntctCode"] = -2;
									dataTable5.AcceptChanges();
									dataTable4.ImportRow(dataTable5.Rows[0]);
								}
							}
							dataTable4.AcceptChanges();
							RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
						}
					}
					DataTable dataTable6 = objectAndSubObjectTables.Tables["BankAcct"];
					if (dataTable6 == null || dataTable6.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OCRB' or table have no columns";
					}
					ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPBankAccounts);
					if (importDataTableObject3 != null)
					{
						string cardCol3 = "";
						DataRow dataRow6 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
						if (dataRow6 != null)
						{
							cardCol3 = Function.ToString(dataRow6["Source"]);
						}
						EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r[cardCol3]), ignoreCase: true) == 0
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num5 = 0; num5 < list3.Count; num5++)
							{
								DataTable dataTable7 = ImportHelper.PrepareImportData(list3[num5], importDataTableObject3.MappingData, importDataTableObject3.ImportObject);
								if (!dataTable7.IsNotNull())
								{
									continue;
								}
								string country = Function.ToString(dataTable7.Rows[0]["Country"]);
								string bankCode = Function.ToString(dataTable7.Rows[0]["BankCode"]);
								string account = Function.ToString(dataTable7.Rows[0]["Account"]);
								DataRow dataRow7 = dataTable6.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(country, Function.ToString(r["Country"]), ignoreCase: true) == 0 && string.Compare(bankCode, Function.ToString(r["BankCode"]), ignoreCase: true) == 0 && string.Compare(account, Function.ToString(r["Account"]), ignoreCase: true) == 0);
								if (dataRow7 != null)
								{
									int num6 = Function.ParseInt(dataRow7["AbsEntry"]);
									string text4 = UpdateDataToRow(dataTable7.Rows[0], dataRow7);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
									dataRow7["AbsEntry"] = num6;
								}
								else
								{
									dataTable7.Rows[0]["AbsEntry"] = -2;
									dataTable7.AcceptChanges();
									dataTable6.ImportRow(dataTable7.Rows[0]);
								}
							}
							dataTable6.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDataTableObject3.ImportData);
						}
					}
					DataTable dataTable8 = objectAndSubObjectTables.Tables["PmntDate"];
					if (dataTable8 == null || dataTable8.Columns.Count == 0)
					{
						return "Dataset does not contain table 'CRD5' or table have no columns";
					}
					ImportDataTableObject importDataTableObject4 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPPaymentDates);
					if (importDataTableObject4 != null)
					{
						string cardCol4 = "";
						DataRow dataRow8 = importDataTableObject4.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
						if (dataRow8 != null)
						{
							cardCol4 = Function.ToString(dataRow8["Source"]);
						}
						EnumerableRowCollection<DataRow> source4 = from r in importDataTableObject4.ImportData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r[cardCol4]), ignoreCase: true) == 0
							select r;
						if (source4.Any())
						{
							List<DataRow> list4 = source4.ToList();
							for (int num7 = 0; num7 < list4.Count; num7++)
							{
								DataTable dataTable9 = ImportHelper.PrepareImportData(list4[num7], importDataTableObject4.MappingData, importDataTableObject4.ImportObject);
								if (dataTable9.IsNotNull())
								{
									string pmntDate = Function.ToString(dataTable9.Rows[0]["PmntDate"]);
									if (Function.ParseInt(pmntDate) > 31 || Function.ParseInt(pmntDate) < 1)
									{
										return "Invalid date, must between (1 - 31)";
									}
									if (dataTable8.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(pmntDate, Function.ToString(r["PmntDate"]), ignoreCase: true) == 0) == null)
									{
										dataTable8.ImportRow(dataTable9.Rows[0]);
									}
								}
							}
							dataTable8.AcceptChanges();
							RemoveRowsFromDatatable(list4, importDataTableObject4.ImportData);
						}
					}
					return HttpClient.ImportDataClient.UpdateBP(objectAndSubObjectTables);
				}
				return "Dataset does not contain table 'BP'";
			}
			return "Cannot get BP details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObject(DataTable dtBp, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataRow dataRow = dtBp.Rows[0];
			string cardCode = Function.ToString(dataRow["CardCode"]);
			string text = Function.ToString(dataRow["CardType"]);
			if (text != "C" && text != "S" && text != "L")
			{
				text = "C";
			}
			string subType = "S";
			if (text == "C" || text == "L")
			{
				subType = "C";
			}
			int bPManualSeries = HttpClient.ImportDataClient.GetBPManualSeries("2", subType);
			dataRow["Series"] = bPManualSeries;
			dataRow["CardType"] = text;
			string text2 = Function.ToString(dataRow["CmpPrivate"]);
			if (text2 != "C" && text2 != "I" && text2 != "G")
			{
				dataRow["CmpPrivate"] = "C";
			}
			dataRow["ObjType"] = 2;
			dataRow["CreateDate"] = DateTime.Now;
			dataRow["DataSource"] = "M";
			dataRow["UserSign"] = GlobalData.UserID;
			dtBp.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtBp.TableName = "BP";
			dtBp.DataSet?.Tables.Remove(dtBp.TableName);
			dataSet.Tables.Add(dtBp);
			DataTable tableStructure = WinFuntions.GetTableStructure("ATC1");
			tableStructure.TableName = "AttFile";
			tableStructure.DataSet?.Tables.Remove(tableStructure.TableName);
			dataSet.Tables.Add(tableStructure);
			DataTable tableStructure2 = WinFuntions.GetTableStructure("CRD1");
			ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPAddresses);
			if (importDataTableObject != null)
			{
				string cardCol = "";
				DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
				if (dataRow2 != null)
				{
					cardCol = Function.ToString(dataRow2["Source"]);
				}
				EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r[cardCol]), ignoreCase: true) == 0
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
			tableStructure2.TableName = "Address";
			tableStructure2.DataSet?.Tables.Remove(tableStructure2.TableName);
			dataSet.Tables.Add(tableStructure2);
			DataTable tableStructure3 = WinFuntions.GetTableStructure("OCPR");
			ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.Contacts);
			if (importDataTableObject2 != null)
			{
				string cardCol2 = "";
				DataRow dataRow3 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
				if (dataRow3 != null)
				{
					cardCol2 = Function.ToString(dataRow3["Source"]);
				}
				EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r[cardCol2]), ignoreCase: true) == 0
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						DataTable dataTable2 = ImportHelper.PrepareImportData(list2[num2], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
						dataTable2.Rows[0]["CntctCode"] = -2;
						dataTable2.AcceptChanges();
						if (dataTable2.IsNotNull())
						{
							tableStructure3.ImportRow(dataTable2.Rows[0]);
						}
					}
					tableStructure3.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
				}
			}
			tableStructure3.TableName = "CntPrsn";
			tableStructure3.DataSet?.Tables.Remove(tableStructure3.TableName);
			dataSet.Tables.Add(tableStructure3);
			DataTable tableStructure4 = WinFuntions.GetTableStructure("OCRB");
			ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPBankAccounts);
			if (importDataTableObject3 != null)
			{
				string cardCol3 = "";
				DataRow dataRow4 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
				if (dataRow4 != null)
				{
					cardCol3 = Function.ToString(dataRow4["Source"]);
				}
				EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r[cardCol3]), ignoreCase: true) == 0
					select r;
				if (source3.Any())
				{
					List<DataRow> list3 = source3.ToList();
					for (int num3 = 0; num3 < list3.Count; num3++)
					{
						DataTable dataTable3 = ImportHelper.PrepareImportData(list3[num3], importDataTableObject3.MappingData, importDataTableObject3.ImportObject);
						if (dataTable3.IsNotNull())
						{
							dataTable3.Rows[0]["AbsEntry"] = -2;
							dataTable3.AcceptChanges();
							tableStructure4.ImportRow(dataTable3.Rows[0]);
						}
					}
					tableStructure4.AcceptChanges();
					RemoveRowsFromDatatable(list3, importDataTableObject3.ImportData);
				}
			}
			tableStructure4.TableName = "BankAcct";
			tableStructure4.DataSet?.Tables.Remove(tableStructure4.TableName);
			dataSet.Tables.Add(tableStructure4);
			DataTable tableStructure5 = WinFuntions.GetTableStructure("CRD5");
			ImportDataTableObject importDataTableObject4 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.BPPaymentDates);
			if (importDataTableObject4 != null)
			{
				string cardCol4 = "";
				DataRow dataRow5 = importDataTableObject4.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "CardCode");
				if (dataRow5 != null)
				{
					cardCol4 = Function.ToString(dataRow5["Source"]);
				}
				EnumerableRowCollection<DataRow> source4 = from r in importDataTableObject4.ImportData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r[cardCol4]), ignoreCase: true) == 0
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
			tableStructure5.TableName = "PmntDate";
			tableStructure5.DataSet?.Tables.Remove(tableStructure5.TableName);
			dataSet.Tables.Add(tableStructure5);
			DataTable dataTable5 = new DataTable();
			dataTable5.Columns.Add(new DataColumn("OldCode", typeof(string)));
			dataTable5.TableName = "OtherTable";
			dataTable5.DataSet?.Tables.Remove(dataTable5.TableName);
			dataSet.Tables.Add(dataTable5);
			dataSet.AcceptChanges();
			return HttpClient.ImportDataClient.AddBP(dataSet);
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
			string value = Function.ToString(dataRow["CardCode"]);
			DataTable tableStructure = WinFuntions.GetTableStructure("OCRD");
			DataRow dataRow2 = tableStructure.NewRow();
			dataRow2["CardCode"] = value;
			dataRow2["CardName"] = value;
			dataRow2["CardType"] = "C";
			dataRow2["GroupCode"] = 100;
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
				return HttpClient.ImportDataClient.CheckExistBP(Function.ToString(dataRow["CardCode"]));
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
			string cardCode = Function.ToString(dataRow["CardCode"]);
			DataTable aBP = HttpClient.ImportDataClient.GetABP(cardCode);
			int atcEntry = Function.ParseInt(aBP.Rows[0]["AtcEntry"]);
			DataTable dataTable = HttpClient.ImportDataClient.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			DataTable dataTable2 = HttpClient.ImportDataClient.GetBPAddress(cardCode);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = WinFuntions.GetTableStructure("CRD1");
			}
			DataTable dataTable3 = HttpClient.ImportDataClient.GetBPContact(cardCode);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = WinFuntions.GetTableStructure("OCPR");
			}
			DataTable dataTable4 = HttpClient.ImportDataClient.GetBPBankAccount(cardCode);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = WinFuntions.GetTableStructure("OCRB");
			}
			DataTable dataTable5 = HttpClient.ImportDataClient.GetBPPaymentDate(cardCode);
			if (dataTable5 == null || dataTable5.Rows.Count == 0)
			{
				dataTable5 = WinFuntions.GetTableStructure("CRD5");
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			aBP.TableName = "BP";
			aBP.DataSet?.Tables.Remove(aBP.TableName);
			dataSet.Tables.Add(aBP);
			dataTable.TableName = "AttFile";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			dataTable2.TableName = "Address";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			dataTable3.TableName = "CntPrsn";
			dataTable3.DataSet?.Tables.Remove(dataTable3.TableName);
			dataSet.Tables.Add(dataTable3);
			dataTable4.TableName = "BankAcct";
			dataTable4.DataSet?.Tables.Remove(dataTable4.TableName);
			dataSet.Tables.Add(dataTable4);
			dataTable5.TableName = "PmntDate";
			dataTable5.DataSet?.Tables.Remove(dataTable5.TableName);
			dataSet.Tables.Add(dataTable5);
			DataTable dataTable6 = new DataTable
			{
				Columns = 
				{
					new DataColumn("OldCode", typeof(string))
				},
				TableName = "OtherTable"
			};
			dataTable6.DataSet?.Tables.Remove(dataTable6.TableName);
			dataSet.Tables.Add(dataTable6);
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
