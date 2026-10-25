using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.ImportData;

namespace Apzon.Services.Repository.ImportData.Inventory;

public class ImportDataItemMaster : BaseImportData
{
	private string _autoITW = "N";

	public ImportDataItemMaster(string connection, int userID, IUnitOfWork unitOfWork)
		: base(connection, userID, unitOfWork)
	{
		DataTable allGeneralSetting = unitOfWork.GeneralSetting.GetAllGeneralSetting();
		if (allGeneralSetting.IsNotNull())
		{
			_autoITW = Function.ToString(allGeneralSetting.Rows[0]["AutoITW"]);
		}
	}

	public string ImportItems(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
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
				return AddItem(dataTable, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateItem(LstDTImportObject, "", dataTable);
			case Enums.ImportActionEnum.AddUpdate:
			{
				string itemCode = Function.ToString(dataTable.Rows[0]["ItemCode"]);
				if (_unitOfWork.ImportData.CheckExistItem(itemCode))
				{
					return UpdateItem(LstDTImportObject, "", dataTable);
				}
				return AddItem(dataTable, LstDTImportObject);
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

	private string UpdateItem(List<ImportDTImpl> LstDTImportObject, string itemCode = "", DataTable dtItemSource = null)
	{
		try
		{
			if (itemCode == "" && dtItemSource.IsNotNull())
			{
				itemCode = Function.ToString(dtItemSource.Rows[0]["ItemCode"]);
			}
			DataSet itemAndSubObjectTables = GetItemAndSubObjectTables(itemCode);
			if (itemAndSubObjectTables != null && itemAndSubObjectTables.Tables.Count == 6 && itemAndSubObjectTables.Tables[0] != null && itemAndSubObjectTables.Tables[1] != null && itemAndSubObjectTables.Tables[2] != null && itemAndSubObjectTables.Tables[3] != null && itemAndSubObjectTables.Tables[4] != null && itemAndSubObjectTables.Tables[5] != null)
			{
				if (itemAndSubObjectTables.Tables.Contains("OITM"))
				{
					int ugpEntry = -1;
					if (dtItemSource != null)
					{
						DataTable dataTable = itemAndSubObjectTables.Tables["OITM"];
						int num = Function.ParseInt(dataTable.Rows[0]["Series"]);
						string text = UpdateDataToRow(dtItemSource.Rows[0], dataTable.Rows[0]);
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
						dataTable.Rows[0]["Series"] = num;
						dataTable.Rows[0]["UpdateDate"] = DateTime.Now;
						dataTable.Rows[0]["UserSign2"] = _userID;
						dataTable.AcceptChanges();
						ugpEntry = Function.ParseInt(dataTable.Rows[0]["UgpEntry"]);
					}
					DataTable dataTable2 = itemAndSubObjectTables.Tables["OITW"];
					if (dataTable2 == null || dataTable2.Columns.Count == 0)
					{
						return "Dataset does not contain table 'OITW' or table have no columns";
					}
					ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.ItemWarehouseInfo);
					if (importDTImpl != null)
					{
						EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
							select r;
						if (source.Any())
						{
							List<DataRow> list = source.ToList();
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								string whsCode = Function.ToString(list[num2]["WhsCode"]);
								DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(whsCode, Function.ToString(r["WhsCode"]), ignoreCase: true) == 0);
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
					DataTable dataTable3 = itemAndSubObjectTables.Tables["ITM1"];
					if (dataTable3 == null || dataTable3.Columns.Count == 0)
					{
						return "Dataset does not contain table 'ITM1' or table have no columns";
					}
					ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.Items_Prices);
					if (importDTImpl2 != null)
					{
						EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
							select r;
						if (source2.Any())
						{
							List<DataRow> list2 = source2.ToList();
							for (int num3 = 0; num3 < list2.Count; num3++)
							{
								int priceList = Function.ParseInt(list2[num3]["PriceList"]);
								DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow r) => priceList == Function.ParseInt(r["PriceList"]));
								if (dataRow2 != null)
								{
									double num4 = Function.ParseDouble(dataRow2["Price"]);
									string text3 = UpdateDataToRow(list2[num3], dataRow2);
									if (!string.IsNullOrEmpty(text3))
									{
										return text3;
									}
									double num5 = Function.ParseDouble(dataRow2["Price"]);
									if (num5 != num4 && num5 > 0.0)
									{
										dataRow2["Ovrwritten"] = "Y";
									}
								}
								else
								{
									dataTable3.ImportRow(list2[num3]);
								}
							}
							dataTable3.AcceptChanges();
							RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
						}
					}
					DataTable dataTable4 = itemAndSubObjectTables.Tables["ITM12"];
					if (dataTable4 == null || dataTable4.Columns.Count == 0)
					{
						return "Dataset does not contain table 'ITM12' or table have no columns";
					}
					ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.ItemUnitOfMeasurement);
					if (importDTImpl3 != null)
					{
						EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
							where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
							select r;
						if (source3.Any())
						{
							List<DataRow> list3 = source3.ToList();
							for (int num6 = 0; num6 < list3.Count; num6++)
							{
								string uomType = Function.ToString(list3[num6]["UomType"]);
								int uomEntry = Function.ParseInt(list3[num6]["UomEntry"]);
								DataRow dataRow3 = dataTable4.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["UomType"]), uomType, ignoreCase: true) == 0 && Function.ParseInt(r["UomEntry"]) == uomEntry);
								if (dataRow3 != null)
								{
									string text4 = UpdateDataToRow(list3[num6], dataRow3);
									if (!string.IsNullOrEmpty(text4))
									{
										return text4;
									}
									continue;
								}
								dataTable4.ImportRow(list3[num6]);
								dataTable4.AcceptChanges();
								if (!dataTable4.AsEnumerable().Any((DataRow r) => Function.ToString(r["UomType"]) == "I"))
								{
									DataRow dataRow4 = dataTable4.NewRow();
									dataRow4["ItemCode"] = itemCode;
									dataRow4["UomType"] = "I";
									dataRow4["UomEntry"] = _unitOfWork.ImportData.ItemGetDefaultInventoryUomEntry(ugpEntry);
									dataTable4.Rows.Add(dataRow4);
									dataTable4.AcceptChanges();
								}
							}
							dataTable4.AcceptChanges();
							RemoveRowsFromDatatable(list3, importDTImpl3.DTData);
						}
					}
					return _unitOfWork.Items.SaveDoc(itemAndSubObjectTables, "U");
				}
				return "Dataset does not contain table 'OITM'";
			}
			return "Cannot get OITM details, wrong number of table or dataset is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddItem(DataTable dtItem, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataRow dataRow = dtItem.Rows[0];
			string itemCode = Function.ToString(dataRow["ItemCode"]);
			int manualSeries = _unitOfWork.ImportData.GetManualSeries("4");
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
			dataRow["UserSign"] = _userID;
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
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("OITW");
			ImportDTImpl importDTImpl = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.ItemWarehouseInfo);
			if (importDTImpl != null)
			{
				EnumerableRowCollection<DataRow> source = from r in importDTImpl.DTData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
					select r;
				if (source.Any())
				{
					List<DataRow> list = source.ToList();
					for (int num = 0; num < list.Count; num++)
					{
						bPTableStructure.ImportRow(list[num]);
					}
					bPTableStructure.AcceptChanges();
					RemoveRowsFromDatatable(list, importDTImpl.DTData);
				}
				else if (_autoITW == "Y")
				{
					DataTable wareHouse = _unitOfWork.ImportData.GetWareHouse();
					if (wareHouse != null && wareHouse.Rows.Count > 0)
					{
						for (int num2 = 0; num2 < wareHouse.Rows.Count; num2++)
						{
							DataRow dataRow2 = bPTableStructure.NewRow();
							dataRow2["WhsCode"] = Function.ToString(wareHouse.Rows[num2]["WhsCode"]);
							dataRow2["WhsName"] = Function.ToString(wareHouse.Rows[num2]["WhsName"]);
							dataRow2["CreateDate"] = DateTime.Now;
							dataRow2["UpdateDate"] = DateTime.Now;
							dataRow2["UserSign"] = _userID;
							dataRow2["UserSign2"] = _userID;
							dataRow2["Locked"] = "N";
							dataRow2["WasCounted"] = "N";
							dataRow2["Locked"] = "N";
							dataRow2["Object"] = "31";
							dataRow2["DftBinEnfd"] = "N";
							dataRow2["Freezed"] = "N";
							dataRow2["MinStock"] = 0;
							dataRow2["MaxStock"] = 0;
							dataRow2["MinOrder"] = 0;
							bPTableStructure.Rows.Add(dataRow2);
						}
					}
				}
			}
			bPTableStructure.TableName = "OITW";
			bPTableStructure.DataSet?.Tables.Remove(bPTableStructure.TableName);
			dataSet.Tables.Add(bPTableStructure);
			DataTable dataTable = _unitOfWork.ImportData.GetDefaultItemPrice();
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _unitOfWork.BusinessMasterData.GetBPTableStructure("ITM1");
			}
			ImportDTImpl importDTImpl2 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.Items_Prices);
			if (importDTImpl2 != null)
			{
				EnumerableRowCollection<DataRow> source2 = from r in importDTImpl2.DTData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
					select r;
				if (source2.Any())
				{
					List<DataRow> list2 = source2.ToList();
					for (int num3 = 0; num3 < list2.Count; num3++)
					{
						int priceList = Function.ParseInt(list2[num3]["PriceList"]);
						DataRow dataRow3 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => priceList == Function.ParseInt(r["PriceList"]));
						if (dataRow3 != null)
						{
							string text = UpdateDataToRow(list2[num3], dataRow3);
							if (!string.IsNullOrEmpty(text))
							{
								return text;
							}
							if (Function.ParseDouble(dataRow3["Price"]) > 0.0)
							{
								dataRow3["Ovrwritten"] = "Y";
							}
							if (Function.ParseDouble(dataRow3["AddPrice1"]) > 0.0)
							{
								dataRow3["Ovrwrite1"] = "Y";
							}
							if (Function.ParseDouble(dataRow3["AddPrice2"]) > 0.0)
							{
								dataRow3["Ovrwrite2"] = "Y";
							}
						}
						else
						{
							dataTable.ImportRow(list2[num3]);
						}
					}
					dataTable.AcceptChanges();
					RemoveRowsFromDatatable(list2, importDTImpl2.DTData);
				}
			}
			dataTable.TableName = "ITM1";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			DataTable bPTableStructure2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("ITM12");
			ImportDTImpl importDTImpl3 = LstDTImportObject.FirstOrDefault((ImportDTImpl t) => t.ImportObjectCode == Enums.ImportObjectEnum.ItemUnitOfMeasurement);
			if (importDTImpl3 != null)
			{
				EnumerableRowCollection<DataRow> source3 = from r in importDTImpl3.DTData.AsEnumerable()
					where string.Compare(itemCode, Function.ToString(r["ItemCode"]), ignoreCase: true) == 0
					select r;
				if (source3.Any())
				{
					List<DataRow> lstRows = source3.ToList();
					RemoveRowsFromDatatable(lstRows, importDTImpl3.DTData);
				}
			}
			bPTableStructure2.TableName = "ITM12";
			bPTableStructure2.DataSet?.Tables.Remove(bPTableStructure2.TableName);
			dataSet.Tables.Add(bPTableStructure2);
			DataTable bPTableStructure3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			bPTableStructure3.TableName = "ATC1";
			bPTableStructure3.DataSet?.Tables.Remove(bPTableStructure3.TableName);
			dataSet.Tables.Add(bPTableStructure3);
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add(new DataColumn("OldCode", typeof(string)));
			dataTable2.TableName = "OtherTable";
			dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
			dataSet.Tables.Add(dataTable2);
			return _unitOfWork.Items.SaveDoc(dataSet, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string ImportItemChildObject(DataRow dr, Enums.ImportActionEnum action, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			dataTable.ImportRow(dr);
			dataTable.AcceptChanges();
			string itemCode = Function.ToString(dataTable.Rows[0]["ItemCode"]);
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				return AddItemFromChilds(itemCode, LstDTImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateItem(LstDTImportObject, itemCode);
			case Enums.ImportActionEnum.AddUpdate:
				if (_unitOfWork.ImportData.CheckExistItem(itemCode))
				{
					return UpdateItem(LstDTImportObject, itemCode);
				}
				return AddItemFromChilds(itemCode, LstDTImportObject);
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

	private string AddItemFromChilds(string itemCode, List<ImportDTImpl> LstDTImportObject)
	{
		try
		{
			DataTable bPTableStructure = _unitOfWork.BusinessMasterData.GetBPTableStructure("OITM");
			DataRow dataRow = bPTableStructure.NewRow();
			dataRow["ItemCode"] = itemCode;
			dataRow["ItemName"] = itemCode;
			dataRow["FrgnName"] = itemCode;
			dataRow["ItemType"] = "I";
			dataRow["ItmsGrpCod"] = 100;
			bPTableStructure.Rows.Add(dataRow);
			bPTableStructure.AcceptChanges();
			return AddItem(bPTableStructure, LstDTImportObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataSet GetItemAndSubObjectTables(string itemCode)
	{
		try
		{
			DataTable aItem = _unitOfWork.ImportData.GetAItem(itemCode);
			if (aItem == null)
			{
				return new DataSet();
			}
			int atcEntry = Function.ParseInt(aItem.Rows[0]["AtcEntry"]);
			DataTable dataTable = _unitOfWork.ImportData.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = _unitOfWork.BusinessMasterData.GetBPTableStructure("ATC1");
			}
			DataTable dataTable2 = _unitOfWork.ImportData.GetItemWarehouse(itemCode);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = _unitOfWork.BusinessMasterData.GetBPTableStructure("OITW");
			}
			DataTable dataTable3 = _unitOfWork.ImportData.GetItemPrice(itemCode);
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = _unitOfWork.BusinessMasterData.GetBPTableStructure("ITM1");
			}
			DataTable dataTable4 = _unitOfWork.ImportData.GetItemUoM(itemCode);
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = _unitOfWork.BusinessMasterData.GetBPTableStructure("ITM12");
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
			return new DataSet();
		}
	}
}
