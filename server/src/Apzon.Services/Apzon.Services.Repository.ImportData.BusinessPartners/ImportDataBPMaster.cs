using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.ImportData;

namespace Apzon.Services.Repository.ImportData.BusinessPartners;

public class ImportDataBPMaster : BaseImportData
{
	public ImportDataBPMaster(string connection, int userID, IUnitOfWork unitOfWork)
		: base(connection, userID, unitOfWork)
	{
	}

	public string ImportBusinessPartners(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			DataTable tableColumns = _unitOfWork.Commons.GetTableColumns("OCRD");
			tableColumns.Rows.Clear();
			tableColumns.AcceptChanges();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = tableColumns.NewRow();
				UpdateDataToRow(dataTable.Rows[i], dataRow);
				tableColumns.Rows.Add(dataRow);
			}
			tableColumns.AcceptChanges();
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				return AddBP(tableColumns, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateBP(LstDTImportObject, "", tableColumns);
			case Enums.ImportActionEnum.AddUpdate:
			{
				string cardCode = Function.ToString(dataTable.Rows[0]["CardCode"]);
				if (_unitOfWork.ImportData.CheckExistBP(cardCode))
				{
					return UpdateBP(LstDTImportObject, "", dataTable);
				}
				return AddBP(dataTable, LstDTImportObject);
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

	private string UpdateBP(List<ImportDTImpl> LstDTImportObject, string cardCode = "", DataTable dtBPSource = null)
	{
		try
		{
			if (cardCode == "" && dtBPSource.IsNotNull())
			{
				cardCode = Function.ToString(dtBPSource.Rows[0]["CardCode"]);
			}
			DataSet bPAndSubObjectTables = GetBPAndSubObjectTables(cardCode);
			if (bPAndSubObjectTables != null && bPAndSubObjectTables.Tables.Count == 7 && bPAndSubObjectTables.Tables[0] != null && bPAndSubObjectTables.Tables[1] != null && bPAndSubObjectTables.Tables[2] != null && bPAndSubObjectTables.Tables[3] != null && bPAndSubObjectTables.Tables[4] != null && bPAndSubObjectTables.Tables[5] != null && bPAndSubObjectTables.Tables[6] != null)
			{
				if (bPAndSubObjectTables.Tables.Contains("BP"))
				{
					if (dtBPSource != null)
					{
						DataTable dataTable = bPAndSubObjectTables.Tables["BP"];
						int num = Function.ParseInt(dataTable.Rows[0]["Series"]);
						string text = UpdateDataToRow(dtBPSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["Series"] = num;
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = _userID;
						dataTable.AcceptChanges();
					}
					DataTable dataTable2 = bPAndSubObjectTables.Tables["Address"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'CRD1' or table have no columns";
					}
					ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPAddresses);
					if (importDTImpl != null)
					{
						EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								string address = Function.ToString(list[num2]["Address"]);
								string adresType = Function.ToString(list[num2]["AdresType"]);
								DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(address, Function.ToString(r["Address"]), ignoreCase: true) == 0 && string.Compare(adresType, Function.ToString(r["AdresType"]), ignoreCase: true) == 0);
								if (dataRow != null)
								{
									string text2 = UpdateDataToRow(list[num2], dataRow);
									if (!string.IsNullOrEmpty(text2))
									{
										return text2;
									}
								}
								else
								{
									dataTable2.ImportRow(list[num2]);
								}
							}
							dataTable2.AcceptChanges();
							RemoveRowsFromDatatable(list, importDTImpl.DTData);
						}
					}
					DataTable dataTable3 = bPAndSubObjectTables.Tables["CntPrsn"];
					if (dataTable3 == null || dataTable3.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OCPR' or table have no columns";
					}
					ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.Contacts);
					if (importDTImpl2 != null)
					{
						EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
							select r;
						if (source2.Any())
						{
							List<DataRow> list2 = source2.ToList();
							for (int num3 = 0; num3 < list2.Count; num3++)
							{
								string name = Function.ToString(list2[num3]["Name"]);
								DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(name, Function.ToString(r["Name"]), ignoreCase: true) == 0);
								if (dataRow2 != null)
								{
									int num4 = Function.ParseInt(dataRow2["CntctCode"]);
									string text3 = UpdateDataToRow(list2[num3], dataRow2);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
									dataRow2["CntctCode"] = num4;
								}
								else
								{
									list2[num3]["CntctCode"] = -2;
									list2[num3].Table.AcceptChanges();
									dataTable3.ImportRow(list2[num3]);
								}
							}
							dataTable3.AcceptChanges();
							RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
						}
					}
					DataTable dataTable4 = bPAndSubObjectTables.Tables["BankAcct"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OCRB' or table have no columns";
					}
					ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPBankAccounts);
					if (importDTImpl3 != null)
					{
						EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num5 = 0; num5 < list3.Count; num5++)
							{
								string country = Function.ToString(list3[num5]["Country"]);
								string bankCode = Function.ToString(list3[num5]["BankCode"]);
								string account = Function.ToString(list3[num5]["Account"]);
								DataRow dataRow3 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(country, Function.ToString(r["Country"]), ignoreCase: true) == 0 && string.Compare(bankCode, Function.ToString(r["BankCode"]), ignoreCase: true) == 0 && string.Compare(account, Function.ToString(r["Account"]), ignoreCase: true) == 0);
								if (dataRow3 != null)
								{
									int num6 = Function.ParseInt(dataRow3["AbsEntry"]);
									string text4 = UpdateDataToRow(list3[num5], dataRow3);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
									dataRow3["AbsEntry"] = num6;
								}
								else
								{
									list3[num5]["AbsEntry"] = -2;
									list3[num5].Table.AcceptChanges();
									dataTable4.ImportRow(list3[num5]);
								}
							}
							dataTable4.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDTImpl3.DTData);
						}
					}
					DataTable dataTable5 = bPAndSubObjectTables.Tables["PmntDate"];
					if (dataTable5 == null || dataTable5.Columns.Count == 0)
					{
						return "Dataset does not contain table 'CRD5' or table have no columns";
					}
					ImportDTImpl importDTImpl4 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPPaymentDates);
					if (importDTImpl4 != null)
					{
						EnumerableRowCollection<DataRow> source4 = from r in importDTImpl4.DTData.AsEnumerable()
							where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
							select r;
						if (source4.Any())
						{
							List<DataRow> list4 = source4.ToList();
							for (int num7 = 0; num7 < list4.Count; num7++)
							{
								string pmntDate = Function.ToString(list4[num7]["PmntDate"]);
								if (Function.ParseInt(pmntDate) > 31 || Function.ParseInt(pmntDate) < 1)
								{
									return "Invalid date, must between (1 - 31)";
								}
								if (dataTable5.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(pmntDate, Function.ToString(r["PmntDate"]), ignoreCase: true) == 0) == null)
								{
									dataTable5.ImportRow(list4[num7]);
								}
							}
							dataTable5.AcceptChanges();
							RemoveRowsFromDatatable(list4, importDTImpl4.DTData);
						}
					}
					return _unitOfWork.BusinessMasterData.SaveDocDataSet(bPAndSubObjectTables, "U");
				}
				return "Dataset does not contain table 'BP'";
			}
			return "Cannot get BP details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddBP(DataTable dtBP, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataRow dataRow = dtBP.Rows[0];
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
			int bPManualSeries = _unitOfWork.ImportData.GetBPManualSeries("2", subType);
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
			dataRow["UserSign"] = _userID;
			dtBP.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtBP.TableName = "BP";
			dtBP.DataSet?.Tables.Remove(dtBP.TableName);
			dataSet.Tables.Add(dtBP);
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			bPTableStructure.TableName = "AttFile";
			bPTableStructure.DataSet?.Tables.Remove(bPTableStructure.TableName);
			dataSet.Tables.Add(bPTableStructure);
			DataTable bPTableStructure2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("CRD1");
			ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPAddresses);
			if (importDTImpl != null)
			{
				EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
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
			bPTableStructure2.TableName = "Address";
			bPTableStructure2.DataSet?.Tables.Remove(bPTableStructure2.TableName);
			dataSet.Tables.Add(bPTableStructure2);
			DataTable bPTableStructure3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OCPR");
			ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.Contacts);
			if (importDTImpl2 != null)
			{
				EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						list2[num2]["CntctCode"] = -2;
						list2[num2].Table.AcceptChanges();
						bPTableStructure3.ImportRow(list2[num2]);
					}
					bPTableStructure3.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
				}
			}
			bPTableStructure3.TableName = "CntPrsn";
			bPTableStructure3.DataSet?.Tables.Remove(bPTableStructure3.TableName);
			dataSet.Tables.Add(bPTableStructure3);
			DataTable bPTableStructure4 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OCRB");
			ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPBankAccounts);
			if (importDTImpl3 != null)
			{
				EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
					select r;
				if (source3.Any())
				{
					List<DataRow> list3 = source3.ToList();
					for (int num3 = 0; num3 < list3.Count; num3++)
					{
						list3[num3]["AbsEntry"] = -2;
						list3[num3].Table.AcceptChanges();
						bPTableStructure4.ImportRow(list3[num3]);
					}
					bPTableStructure4.AcceptChanges();
					RemoveRowsFromDatatable(list3, importDTImpl3.DTData);
				}
			}
			bPTableStructure4.TableName = "BankAcct";
			bPTableStructure4.DataSet?.Tables.Remove(bPTableStructure4.TableName);
			dataSet.Tables.Add(bPTableStructure4);
			DataTable bPTableStructure5 = _unitOfWork.BusinessMasterData.GetBPTableStructure("CRD5");
			ImportDTImpl importDTImpl4 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.BPPaymentDates);
			if (importDTImpl4 != null)
			{
				EnumerableRowCollection<DataRow> source4 = from r in importDTImpl4.DTData.AsEnumerable()
					where string.Compare(cardCode, Function.ToString(r["CardCode"]), ignoreCase: true) == 0
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
			bPTableStructure5.TableName = "PmntDate";
			bPTableStructure5.DataSet?.Tables.Remove(bPTableStructure5.TableName);
			dataSet.Tables.Add(bPTableStructure5);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("OldCode", typeof(string)));
			dataTable.TableName = "OtherTable";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			dataSet.AcceptChanges();
			return _unitOfWork.BusinessMasterData.SaveDocDataSet(dataSet, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string ImportBPChildObject(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			string cardCode = Function.ToString(dataTable.Rows[0]["CardCode"]);
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				return AddBPFromChilds(cardCode, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateBP(LstDTImportObject, cardCode);
			case Enums.ImportActionEnum.AddUpdate:
				if (_unitOfWork.ImportData.CheckExistBP(cardCode))
				{
					return UpdateBP(LstDTImportObject, cardCode);
				}
				return AddBPFromChilds(cardCode, LstDTImportObject);
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

	private string AddBPFromChilds(string cardCode, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("OCRD");
			DataRow dataRow = bPTableStructure.NewRow();
			dataRow["CardCode"] = cardCode;
			dataRow["CardName"] = cardCode;
			dataRow["CardType"] = "C";
			dataRow["GroupCode"] = 100;
			bPTableStructure.Rows.Add(dataRow);
			bPTableStructure.AcceptChanges();
			return AddBP(bPTableStructure, LstDTImportObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataSet GetBPAndSubObjectTables(string cardCode)
	{
		try
		{
			DataTable aBP = _unitOfWork.ImportData.GetABP(cardCode);
			int atcEntry = Function.ParseInt(aBP.Rows[0]["AtcEntry"]);
			DataTable dataTable = _unitOfWork.ImportData.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			}
			DataTable dataTable2 = _unitOfWork.ImportData.GetBPAddress(cardCode);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("CRD1");
			}
			DataTable dataTable3 = _unitOfWork.ImportData.GetBPContact(cardCode);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OCPR");
			}
			DataTable dataTable4 = _unitOfWork.ImportData.GetBPBankAccount(cardCode);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OCRB");
			}
			DataTable dataTable5 = _unitOfWork.ImportData.GetBPPaymentDate(cardCode);
			if (dataTable5 == null || dataTable5.Rows.Count == 0)
			{
				dataTable5 = _unitOfWork.BusinessMasterData.GetBPTableStructure("CRD5");
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
			return new DataSet();
		}
	}
}
