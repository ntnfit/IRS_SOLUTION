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

namespace Apzon.Forms.ImportData.ImportHelper.Inventory;

internal class ItemMasterDataImport : BaseDataImport
{
	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtItemSource = null)
	{
		try
		{
			string itemCode;
			if (!(key is DataRow dataRow))
			{
				if (!dtItemSource.IsNotNull())
				{
					return "No data to update found";
				}
				itemCode = Function.ToString(dtItemSource.Rows[0]["ItemCode"]);
			}
			else
			{
				itemCode = Function.ToString(dataRow["ItemCode"]);
			}
			DataSet objectAndSubObjectTables = GetObjectAndSubObjectTables(key);
			if (objectAndSubObjectTables != null && objectAndSubObjectTables.Tables.Count == 6 && objectAndSubObjectTables.Tables[0] != null && objectAndSubObjectTables.Tables[1] != null && objectAndSubObjectTables.Tables[2] != null && objectAndSubObjectTables.Tables[3] != null && objectAndSubObjectTables.Tables[4] != null && objectAndSubObjectTables.Tables[5] != null)
			{
				if (objectAndSubObjectTables.Tables.Contains("OITM"))
				{
					int ugpEntry = -1;
					if (dtItemSource != null)
					{
						DataTable dataTable = objectAndSubObjectTables.Tables["OITM"];
						int num = Function.ParseInt(dataTable.Rows[0]["Series"]);
						string text = UpdateDataToRow(dtItemSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["Series"] = num;
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
						dataTable.AcceptChanges();
						ugpEntry = Function.ParseInt(dataTable.Rows[0]["UgpEntry"]);
					}
					DataTable dataTable2 = objectAndSubObjectTables.Tables["OITW"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OITW' or table have no columns";
					}
					ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.ItemWarehouseInfo);
					if (importDataTableObject != null)
					{
						string cardCol = "";
						DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
						if (dataRow2 != null)
						{
							cardCol = Function.ToString(dataRow2["Source"]);
						}
						EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r[cardCol]), ignoreCase: true) == 0
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
								string whsCode = Function.ToString(dataTable3.Rows[0]["WhsCode"]);
								DataRow dataRow3 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(whsCode, Function.ToString(r["WhsCode"]), ignoreCase: true) == 0);
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
					DataTable dataTable4 = objectAndSubObjectTables.Tables["ITM1"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'ITM1' or table have no columns";
					}
					ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.Items_Prices);
					if (importDataTableObject2 != null)
					{
						string cardCol2 = "";
						DataRow dataRow4 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
						if (dataRow4 != null)
						{
							cardCol2 = Function.ToString(dataRow4["Source"]);
						}
						EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r[cardCol2]), ignoreCase: true) == 0
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
								int priceList = Function.ParseInt(dataTable5.Rows[0]["PriceList"]);
								DataRow dataRow5 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => priceList == Function.ParseInt(r["PriceList"]));
								if (dataRow5 != null)
								{
									double num4 = Function.ParseDouble(dataRow5["Price"]);
									string text3 = UpdateDataToRow(dataTable5.Rows[0], dataRow5);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
									double num5 = Function.ParseDouble(dataRow5["Price"]);
									if (num5 != num4 && num5 > 0.0)
									{
										dataRow5["Ovrwritten"] = "Y";
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
					DataTable dataTable6 = objectAndSubObjectTables.Tables["ITM12"];
					if (dataTable6 == null || dataTable6.Columns.Count == 0)
					{
						return "Dataset does not contain table 'ITM12' or table have no columns";
					}
					ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.ItemUnitOfMeasurement);
					if (importDataTableObject3 != null)
					{
						string cardCol3 = "";
						DataRow dataRow6 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
						if (dataRow6 != null)
						{
							cardCol3 = Function.ToString(dataRow6["Source"]);
						}
						EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r[cardCol3]), ignoreCase: true) == 0
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num6 = 0; num6 < list3.Count; num6++)
							{
								DataTable dataTable7 = ImportHelper.PrepareImportData(list3[num6], importDataTableObject3.MappingData, importDataTableObject3.ImportObject);
								if (!dataTable7.IsNotNull())
								{
									continue;
								}
								string uomType = Function.ToString(dataTable7.Rows[0]["UomType"]);
								int uomEntry = Function.ParseInt(dataTable7.Rows[0]["UomEntry"]);
								DataRow dataRow7 = dataTable6.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["UomType"]), uomType, ignoreCase: true) == 0 && Function.ParseInt(r["UomEntry"]) == uomEntry);
								if (dataRow7 != null)
								{
									string text4 = UpdateDataToRow(dataTable7.Rows[0], dataRow7);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
									continue;
								}
								dataTable6.ImportRow(dataTable7.Rows[0]);
								dataTable6.AcceptChanges();
								if (!dataTable6.AsEnumerable().Any((DataRow r) => Function.ToString(r["UomType"]) == "I"))
								{
									DataRow dataRow8 = dataTable6.NewRow();
									dataRow8["ItemCode"] = itemCode;
									dataRow8["UomType"] = "I";
									dataRow8["UomEntry"] = HttpClient.ImportDataClient.ItemGetDefaultInventoryUomEntry(ugpEntry);
									dataTable6.Rows.Add(dataRow8);
									dataTable6.AcceptChanges();
								}
							}
							dataTable6.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDataTableObject3.ImportData);
						}
					}
					return HttpClient.ImportDataClient.UpdateItem(objectAndSubObjectTables);
				}
				return "Dataset does not contain table 'OITM'";
			}
			return "Cannot get OITM details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObject(DataTable dtItem, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataRow dataRow = dtItem.Rows[0];
			string itemCode = Function.ToString(dataRow["ItemCode"]);
			int manualSeries = HttpClient.ImportDataClient.GetManualSeries("4");
			dataRow["Series"] = manualSeries;
			dataRow["ObjType"] = "4";
			dataRow["validFor"] = "Y";
			dataRow["frozenFor"] = "N";
			dataRow["ByWh"] = "N";
			dataRow["CreateDate"] = DateTime.Now;
			dataRow["DataSource"] = "M";
			dataRow["TreeQty"] = 1;
			dataRow["EvalSystem"] = "A";
			dataRow["SalFactor1"] = 1;
			dataRow["SalFactor2"] = 1;
			dataRow["SalFactor3"] = 1;
			dataRow["SalFactor4"] = 1;
			dataRow["PurFactor1"] = 1;
			dataRow["PurFactor2"] = 1;
			dataRow["PurFactor3"] = 1;
			dataRow["PurFactor4"] = 1;
			dataRow["PurPackUn"] = 1;
			dataRow["SalPackUn"] = 1;
			dataRow["UserSign"] = GlobalData.UserID;
			if (Function.ParseInt(dataRow["NumInSale"]) <= 0)
			{
				dataRow["NumInSale"] = 1;
			}
			if (Function.ParseInt(dataRow["NumInBuy"]) <= 0)
			{
				dataRow["NumInBuy"] = 1;
			}
			dtItem.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dtItem.TableName = "OITM";
			dtItem.DataSet?.Tables.Remove(dtItem.TableName);
			dataSet.Tables.Add(dtItem);
			DataTable tableStructure = WinFuntions.GetTableStructure("OITW");
			ImportDataTableObject importDataTableObject = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.ItemWarehouseInfo);
			if (importDataTableObject != null)
			{
				string cardCol = "";
				DataRow dataRow2 = importDataTableObject.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
				if (dataRow2 != null)
				{
					cardCol = Function.ToString(dataRow2["Source"]);
				}
				EnumerableRowCollection<DataRow> source = from r in importDataTableObject.ImportData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r[cardCol]), ignoreCase: true) == 0
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
				else if (GlobalData.GeneralSetting.AutoITW == "Y")
				{
					DataTable wareHouse = HttpClient.ImportDataClient.GetWareHouse();
					if (wareHouse != null && wareHouse.Rows.Count > 0)
					{
						for (int num2 = 0; num2 < wareHouse.Rows.Count; num2++)
						{
							DataRow dataRow3 = tableStructure.NewRow();
							dataRow3["WhsCode"] = Function.ToString(wareHouse.Rows[num2]["WhsCode"]);
							dataRow3["WhsName"] = Function.ToString(wareHouse.Rows[num2]["WhsName"]);
							dataRow3["CreateDate"] = DateTime.Now;
							dataRow3["UpdateDate"] = DateTime.Now;
							dataRow3["UserSign"] = GlobalData.UserID;
							dataRow3["UserSign2"] = GlobalData.UserID;
							dataRow3["Locked"] = "N";
							dataRow3["WasCounted"] = "N";
							dataRow3["Locked"] = "N";
							dataRow3["Object"] = "31";
							dataRow3["DftBinEnfd"] = "N";
							dataRow3["Freezed"] = "N";
							dataRow3["MinStock"] = 0;
							dataRow3["MaxStock"] = 0;
							dataRow3["MinOrder"] = 0;
							tableStructure.Rows.Add(dataRow3);
						}
					}
				}
			}
			tableStructure.TableName = "OITW";
			tableStructure.DataSet?.Tables.Remove(tableStructure.TableName);
			dataSet.Tables.Add(tableStructure);
			DataTable dataTable2 = HttpClient.ImportDataClient.GetDefaultItemPrice();
			if (dataTable2 == null || dataTable2.Columns.Count == 0)
			{
				dataTable2 = WinFuntions.GetTableStructure("ITM1");
			}
			ImportDataTableObject importDataTableObject2 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.Items_Prices);
			if (importDataTableObject2 != null)
			{
				string cardCol2 = "";
				DataRow dataRow4 = importDataTableObject2.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
				if (dataRow4 != null)
				{
					cardCol2 = Function.ToString(dataRow4["Source"]);
				}
				EnumerableRowCollection<DataRow> source2 = from r in importDataTableObject2.ImportData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r[cardCol2]), ignoreCase: true) == 0
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num3 = 0; num3 < list2.Count; num3++)
					{
						DataTable dataTable3 = ImportHelper.PrepareImportData(list2[num3], importDataTableObject2.MappingData, importDataTableObject2.ImportObject);
						if (!dataTable3.IsNotNull())
						{
							continue;
						}
						int priceList = Function.ParseInt(dataTable3.Rows[0]["PriceList"]);
						DataRow dataRow5 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => priceList == Function.ParseInt(r["PriceList"]));
						if (dataRow5 != null)
						{
							string text = UpdateDataToRow(dataTable3.Rows[0], dataRow5);
							if (!string.IsNullOrEmpty(text))
							{
								return text;
							}
							if (Function.ParseDouble(dataRow5["Price"]) > 0.0)
							{
								dataRow5["Ovrwritten"] = "Y";
							}
							if (Function.ParseDouble(dataRow5["AddPrice1"]) > 0.0)
							{
								dataRow5["Ovrwrite1"] = "Y";
							}
							if (Function.ParseDouble(dataRow5["AddPrice2"]) > 0.0)
							{
								dataRow5["Ovrwrite2"] = "Y";
							}
						}
						else
						{
							dataTable2.ImportRow(dataTable3.Rows[0]);
						}
					}
					dataTable2.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDataTableObject2.ImportData);
				}
			}
			dataTable2.TableName = "ITM1";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			DataTable tableStructure2 = WinFuntions.GetTableStructure("ITM12");
			ImportDataTableObject importDataTableObject3 = lstDtImportObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == Enums.ImportObjectEnum.ItemUnitOfMeasurement);
			if (importDataTableObject3 != null)
			{
				string cardCol3 = "";
				DataRow dataRow6 = importDataTableObject3.MappingData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == "ItemCode");
				if (dataRow6 != null)
				{
					cardCol3 = Function.ToString(dataRow6["Source"]);
				}
				EnumerableRowCollection<DataRow> source3 = from r in importDataTableObject3.ImportData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r[cardCol3]), ignoreCase: true) == 0
					select r;
				if (source3.Any())
				{
					List<DataRow> lstRows = source3.ToList();
					RemoveRowsFromDatatable(lstRows, importDataTableObject3.ImportData);
				}
			}
			tableStructure2.TableName = "ITM12";
			tableStructure2.DataSet?.Tables.Remove(tableStructure2.TableName);
			dataSet.Tables.Add(tableStructure2);
			DataTable tableStructure3 = WinFuntions.GetTableStructure("ATC1");
			tableStructure3.TableName = "ATC1";
			tableStructure3.DataSet?.Tables.Remove(tableStructure3.TableName);
			dataSet.Tables.Add(tableStructure3);
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add(new DataColumn("OldCode", typeof(string)));
			dataTable4.TableName = "OtherTable";
			dataTable4.DataSet?.Tables.Remove(dataTable4.TableName);
			dataSet.Tables.Add(dataTable4);
			return HttpClient.ImportDataClient.AddItem(dataSet);
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
			string value = Function.ToString(dataRow["ItemCode"]);
			DataTable tableStructure = WinFuntions.GetTableStructure("OITM");
			DataRow dataRow2 = tableStructure.NewRow();
			dataRow2["ItemCode"] = value;
			dataRow2["ItemName"] = value;
			dataRow2["FrgnName"] = value;
			dataRow2["ItemType"] = "I";
			dataRow2["ItmsGrpCod"] = 100;
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
				return HttpClient.ImportDataClient.CheckExistItem(Function.ToString(dataRow["ItemCode"]));
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
			string itemCode = Function.ToString(dataRow["ItemCode"]);
			DataTable aItem = HttpClient.ImportDataClient.GetAItem(itemCode);
			if (aItem == null)
			{
				WinCommons.Error(Enums.MessageCode.ItemDoesNotExistsOrCannotGetItemDetailTable, "");
				return new DataSet();
			}
			int atcEntry = Function.ParseInt(aItem.Rows[0]["AtcEntry"]);
			DataTable dataTable = HttpClient.ImportDataClient.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			DataTable dataTable2 = HttpClient.ImportDataClient.GetItemWarehouse(itemCode);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = WinFuntions.GetTableStructure("OITW");
			}
			DataTable dataTable3 = HttpClient.ImportDataClient.GetItemPrice(itemCode);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = WinFuntions.GetTableStructure("ITM1");
			}
			DataTable dataTable4 = HttpClient.ImportDataClient.GetItemUoM(itemCode);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = WinFuntions.GetTableStructure("ITM12");
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			aItem.TableName = "OITM";
			aItem.DataSet?.Tables.Remove(aItem.TableName);
			dataSet.Tables.Add(aItem);
			dataTable2.TableName = "OITW";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			dataTable3.TableName = "ITM1";
			dataTable3.DataSet?.Tables.Remove(dataTable3.TableName);
			dataSet.Tables.Add(dataTable3);
			dataTable4.TableName = "ITM12";
			dataTable4.DataSet?.Tables.Remove(dataTable4.TableName);
			dataSet.Tables.Add(dataTable4);
			dataTable.TableName = "ATC1";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			DataTable dataTable5 = new DataTable
			{
				Columns = 
				{
					new DataColumn("OldCode", typeof(string))
				},
				TableName = "OtherTable"
			};
			dataTable5.DataSet?.Tables.Remove(dataTable5.TableName);
			dataSet.Tables.Add(dataTable5);
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
