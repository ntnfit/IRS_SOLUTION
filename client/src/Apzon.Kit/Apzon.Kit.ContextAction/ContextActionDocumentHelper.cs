using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Kit.ContextAction;

public static class ContextActionDocumentHelper
{
	public static void LoadDataBatch(this Document_LinesImpl line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		List<DataColumn> list = (from DataColumn t in dtBatchSerial.Columns
			where Function.ToString(t.ColumnName).StartsWith("U_")
			select t).ToList();
		string text = Function.ToString(dtBatchSerial.Rows[0]["ObjType"]);
		DataTable dtBatchResult = new DataTable();
		dtBatchResult.Columns.Add("DistNumber", typeof(string));
		dtBatchResult.Columns.Add("LotNumber", typeof(string));
		dtBatchResult.Columns.Add("MnfSerial", typeof(string));
		dtBatchResult.Columns.Add("LogQty", typeof(double));
		if (!(text == "10000044"))
		{
			if (!(text == "10000045"))
			{
				return;
			}
			line.SerialNumbers = new List<SerialNumbersImpl>();
			if (line.BinAllocations == null)
			{
				line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
			}
			DataRow[] array = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).ToArray();
			foreach (DataRow dataRow in array)
			{
				SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
				serialNumbersImpl.BatchID = Function.ToString(dataRow["LotNumber"]);
				serialNumbersImpl.BaseLineNumber = Function.ParseInt(dataRow["BaseLineNumber"]);
				serialNumbersImpl.ReceptionDate = Function.ParseDateTime(dataRow["InDate"]);
				serialNumbersImpl.ExpiryDate = Function.ParseDateTime(dataRow["ExpDate"]);
				serialNumbersImpl.InternalSerialNumber = Function.ToString(dataRow["DistNumber"]);
				serialNumbersImpl.Location = Function.ToString(dataRow["Location"]);
				serialNumbersImpl.ManufacturerSerialNumber = Function.ToString(dataRow["MnfSerial"]);
				serialNumbersImpl.ManufactureDate = Function.ParseDateTime(dataRow["MnfDate"]);
				serialNumbersImpl.Notes = Function.ToString(dataRow["Notes"]);
				serialNumbersImpl.Quantity = 1.0;
				serialNumbersImpl.TrackingNote = Function.ParseInt(dataRow["TrackingNt"]);
				serialNumbersImpl.TrackingNoteLine = Function.ParseInt(dataRow["TrackiNtLn"]);
				serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item5 in list)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = item5.ColumnName;
						Type dataType = item5.DataType;
						if (dataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							userFieldsImpl.Value = Function.ParseDateTime(dataRow[item5.ColumnName]);
						}
						else if (dataType == typeof(float))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item5.ColumnName]);
						}
						else if (dataType == typeof(double))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item5.ColumnName]);
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl.Value = Function.ToString(dataRow[item5.ColumnName]);
						}
						serialNumbersImpl.UserFields.Add(userFieldsImpl);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl);
			}
			DataRow[] array2 = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == -1
				select t).ToArray();
			foreach (DataRow dataRow2 in array2)
			{
				SerialNumbersImpl serialNumbersImpl2 = new SerialNumbersImpl();
				serialNumbersImpl2.BatchID = Function.ToString(dataRow2["LotNumber"]);
				serialNumbersImpl2.BaseLineNumber = Function.ParseInt(dataRow2["BaseLineNumber"]);
				serialNumbersImpl2.ReceptionDate = Function.ParseDateTime(dataRow2["InDate"]);
				serialNumbersImpl2.ExpiryDate = Function.ParseDateTime(dataRow2["ExpDate"]);
				serialNumbersImpl2.InternalSerialNumber = Function.ToString(dataRow2["DistNumber"]);
				serialNumbersImpl2.Location = Function.ToString(dataRow2["Location"]);
				serialNumbersImpl2.ManufacturerSerialNumber = Function.ToString(dataRow2["MnfSerial"]);
				serialNumbersImpl2.ManufactureDate = Function.ParseDateTime(dataRow2["MnfDate"]);
				serialNumbersImpl2.Notes = Function.ToString(dataRow2["Notes"]);
				serialNumbersImpl2.Quantity = 0.0;
				serialNumbersImpl2.TrackingNote = Function.ParseInt(dataRow2["TrackingNt"]);
				serialNumbersImpl2.TrackingNoteLine = Function.ParseInt(dataRow2["TrackiNtLn"]);
				serialNumbersImpl2.SystemSerialNumber = Function.ParseInt(dataRow2["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl2.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item6 in list)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						userFieldsImpl2.Name = item6.ColumnName;
						Type dataType2 = item6.DataType;
						if (dataType2 == typeof(DateTime))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
							userFieldsImpl2.Value = Function.ParseDateTime(dataRow2[item6.ColumnName]);
						}
						else if (dataType2 == typeof(float))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Float;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item6.ColumnName]);
						}
						else if (dataType2 == typeof(double))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item6.ColumnName]);
						}
						else
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl2.Value = Function.ToString(dataRow2[item6.ColumnName]);
						}
						serialNumbersImpl2.UserFields.Add(userFieldsImpl2);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl2);
			}
			if (formObjectType == ApzObjectTypes.oOrders)
			{
				return;
			}
			int j;
			for (j = 0; j < dtBatchSerial.Rows.Count; j++)
			{
				if (Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]) > 0)
				{
					SerialNumbersImpl item = line.SerialNumbers.AsEnumerable().FirstOrDefault((SerialNumbersImpl t) => Function.ToString(t.InternalSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["DistNumber"]) && Function.ToString(t.BatchID) == Function.ToString(dtBatchSerial.Rows[j]["LotNumber"]) && Function.ToString(t.ManufacturerSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["MnfSerial"]));
					DocumentLinesBinAllocationsImpl item2 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.SerialNumbers.IndexOf(item),
						BinActionType = ((Function.ParseInt(dtBatchSerial.Rows[j]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
					};
					line.BinAllocations.Add(item2);
				}
			}
			return;
		}
		line.BatchNumbers = new List<BatchNumbersImpl>();
		if (line.BinAllocations == null)
		{
			line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
		}
		DataRow[] batchResult = (from row in (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).AsEnumerable()
			group row by new
			{
				DistNumber = Function.ToString(row["DistNumber"])
			} into g
			select dtBatchResult.LoadDataRow(new object[4]
			{
				g.Key.DistNumber,
				"",
				"",
				Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"])))
			}, fAcceptChanges: false)).ToArray();
		int j2;
		for (j2 = 0; j2 < batchResult.Length; j2++)
		{
			DataRow dataRow3 = dtBatchSerial.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]) == Function.ToString(batchResult[j2]["DistNumber"]) && Function.ParseInt(t["TranType"]) == 1);
			if (dataRow3 == null)
			{
				continue;
			}
			BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(dataRow3["DistNumber"]),
				BaseLineNumber = Function.ParseInt(dataRow3["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(dataRow3["InDate"]),
				ExpiryDate = Function.ParseDateTime(dataRow3["ExpDate"]),
				InternalSerialNumber = Function.ToString(dataRow3["LotNumber"]),
				Location = Function.ToString(dataRow3["Location"]),
				ManufacturerSerialNumber = Function.ToString(dataRow3["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(dataRow3["MnfDate"]),
				Notes = Function.ToString(dataRow3["Notes"]),
				Quantity = Function.ParseDouble(batchResult[j2]["LogQty"]),
				TrackingNote = Function.ParseInt(dataRow3["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(dataRow3["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item7 in list)
				{
					UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl
					{
						Name = item7.ColumnName
					};
					Type dataType3 = item7.DataType;
					if (dataType3 == typeof(DateTime))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Date;
						userFieldsImpl3.Value = Function.ParseDateTime(dataRow3[item7.ColumnName]);
					}
					else if (dataType3 == typeof(float))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Float;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item7.ColumnName]);
					}
					else if (dataType3 == typeof(double))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item7.ColumnName]);
					}
					else
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl3.Value = Function.ToString(dataRow3[item7.ColumnName]);
					}
					batchNumbersImpl.UserFields.Add(userFieldsImpl3);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl);
		}
		DataRow[] array3 = (from t in dtBatchSerial.AsEnumerable()
			where Function.ParseInt(t["TranType"]) == -1
			select t).ToArray();
		for (int num3 = 0; num3 < array3.Length; num3++)
		{
			BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(array3[num3]["DistNumber"]),
				BaseLineNumber = Function.ParseInt(array3[num3]["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(array3[num3]["InDate"]),
				ExpiryDate = Function.ParseDateTime(array3[num3]["ExpDate"]),
				InternalSerialNumber = Function.ToString(array3[num3]["LotNumber"]),
				Location = Function.ToString(array3[num3]["Location"]),
				ManufacturerSerialNumber = Function.ToString(array3[num3]["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(array3[num3]["MnfDate"]),
				Notes = Function.ToString(array3[num3]["Notes"]),
				Quantity = 0.0,
				TrackingNote = Function.ParseInt(array3[num3]["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(array3[num3]["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl2.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item8 in list)
				{
					UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl
					{
						Name = item8.ColumnName
					};
					Type dataType4 = item8.DataType;
					if (dataType4 == typeof(DateTime))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Date;
						userFieldsImpl4.Value = Function.ParseDateTime(array3[num3][item8.ColumnName]);
					}
					else if (dataType4 == typeof(float))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Float;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item8.ColumnName]);
					}
					else if (dataType4 == typeof(double))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item8.ColumnName]);
					}
					else
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl4.Value = Function.ToString(array3[num3][item8.ColumnName]);
					}
					batchNumbersImpl2.UserFields.Add(userFieldsImpl4);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl2);
		}
		if (formObjectType == ApzObjectTypes.oOrders)
		{
			return;
		}
		int j3;
		for (j3 = 0; j3 < dtBatchSerial.Rows.Count; j3++)
		{
			BatchNumbersImpl batchNumbersImpl3 = line.BatchNumbers.AsEnumerable().FirstOrDefault((BatchNumbersImpl t) => Function.ToString(t.BatchNumber) == Function.ToString(dtBatchSerial.Rows[j3]["DistNumber"]));
			if (batchNumbersImpl3 != null)
			{
				if (Function.ParseInt(dtBatchSerial.Rows[j3]["BinAbs"]) > 0)
				{
					DocumentLinesBinAllocationsImpl item3 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j3]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j3]["BinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j3]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl3),
						BinActionType = BinActionTypeEnum.batFromWarehouse
					};
					line.BinAllocations.Add(item3);
				}
				if (formObjectType == ApzObjectTypes.oStockTransfer && Function.ParseInt(dtBatchSerial.Rows[j3]["ToBinAbs"]) > 0)
				{
					DocumentLinesBinAllocationsImpl item4 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j3]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j3]["ToBinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j3]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl3),
						BinActionType = BinActionTypeEnum.batToWarehouse
					};
					line.BinAllocations.Add(item4);
				}
			}
		}
	}

	public static ContextActionMappingImpl[] GetMappingSourceField(DataTable dtMapping, DataTable dtSource)
	{
		ContextActionMappingImpl[] array = (from t in dtMapping.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		foreach (ContextActionMappingImpl item in array.Where((ContextActionMappingImpl t) => !dtSource.Columns.Contains(Function.ToString(t.SourceField))))
		{
			item.SourceField = "";
		}
		return array;
	}

	public static ContextActionMappingImpl[] ResetSourceFieldByDataSource(ContextActionMappingImpl[] mappingField, DataTable dtSource)
	{
		foreach (ContextActionMappingImpl item in mappingField.Where((ContextActionMappingImpl t) => !dtSource.Columns.Contains(Function.ToString(t.SourceField))))
		{
			item.SourceField = "";
		}
		return mappingField;
	}
}
