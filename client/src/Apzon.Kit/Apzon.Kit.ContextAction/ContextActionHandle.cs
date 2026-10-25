using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Kit.ContextAction;

public class ContextActionHandle : IContextActionHandle
{
	private DataTable ChartOfAccount;

	private DataTable BusinessPartner { get; }

	private DataTable BPAddresses { get; }

	private DataTable ContactEmployees { get; }

	private DataTable OcrTable { get; set; }

	private DataTable PrcTable { get; set; }

	private DataTable DocumentTable { get; }

	private DataTable DocumentLineTable { get; }

	private DataTable BatchSerialNumbersStruct { get; }

	private DataTable Items { get; }

	private DataTable ItemPrices { get; }

	private DataTable ItemWarehouses { get; }

	private DataTable JournalEntry { get; }

	private DataTable JournalEntryRows { get; }

	public DataTable UdoTable { get; set; }

	public DataTable VatGroupHeader { get; set; }

	public DataTable VatGroupLines { get; set; }

	public List<BPImpls> GetMappingBusinesPartner(DataTable source, DataSet dsAction)
	{
		List<BPImpls> list = new List<BPImpls>();
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable dataTable = new DataTable();
		if (dsAction.Tables.Count >= 3)
		{
			dataTable = dsAction.Tables[2];
		}
		DataTable dtMapping = new DataTable();
		DataTable dtMapping2 = new DataTable();
		DataTable dtMapping3 = new DataTable();
		DataRow[] array = dataTable.Select("MappingType='2' AND TargetItem='H'");
		if (array.Length != 0)
		{
			dtMapping = array.CopyToDataTable();
		}
		array = dataTable.Select("MappingType='2' AND TargetItem='A'");
		if (array.Length != 0)
		{
			dtMapping2 = array.CopyToDataTable();
		}
		array = dataTable.Select("MappingType='2' AND TargetItem='C'");
		if (array.Length != 0)
		{
			dtMapping3 = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] array2 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
		ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping2, source);
		ContextActionMappingImpl[] mappingSourceField2 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping3, source);
		string text = Function.ToString(dsAction.Tables[0].Rows[0]["Method"]);
		string sourceField = Function.ToString(dsAction.Tables[0].Rows[0]["ObjectId"]);
		if (text == "U")
		{
			List<ContextActionMappingImpl> list2 = array2.ToList();
			list2.Add(new ContextActionMappingImpl
			{
				SourceField = sourceField,
				TargetField = "OldCardCode"
			});
			array2 = list2.ToArray();
		}
		List<string> list3 = new List<string>();
		int num = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			if (!string.IsNullOrEmpty(array2[i].SourceField))
			{
				list3.Add("ISNULL(" + array2[i].SourceField + ",'')='{" + num + "}'");
				num++;
			}
		}
		if (BusinessPartner != null && !BusinessPartner.ExistsColumn("OldCardCode"))
		{
			BusinessPartner.Columns.Add("OldCardCode", typeof(string));
		}
		DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in array2
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = BusinessPartner.Clone();
			dataTable2.Rows.Add(dataTable2.NewRow());
		}
		foreach (DataRow row in dataTable2.Rows)
		{
			List<object> list4 = new List<object>();
			for (int num2 = 0; num2 < array2.Length; num2++)
			{
				if (!string.IsNullOrEmpty(array2[num2].SourceField))
				{
					list4.Add((row[array2[num2].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToStringNotTrim(row[array2[num2].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array3 = source.Select(string.Format(string.Join(" AND ", list3), list4.ToArray()));
			if (array3.Length != 0)
			{
				DataTable dataTable3 = array3.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable4 = array3.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField2
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable5 = BusinessPartner.Clone();
				DataRow dataRow2 = dataTable5.NewRow();
				for (int num3 = 0; num3 < array2.Length; num3++)
				{
					if (!string.IsNullOrEmpty(array2[num3].SourceField))
					{
						dataRow2[array2[num3].TargetField] = row[array2[num3].SourceField];
					}
					if (array2[num3].DfltValue != null && dataRow2[array2[num3].TargetField] == DBNull.Value)
					{
						dataRow2[array2[num3].TargetField] = array2[num3].DfltValue;
					}
				}
				dataTable5.Rows.Add(dataRow2);
				DataTable dataTable6 = BPAddresses.Clone();
				for (int num4 = 0; num4 < dataTable3.Rows.Count; num4++)
				{
					DataRow dataRow3 = dataTable6.NewRow();
					for (int num5 = 0; num5 < mappingSourceField.Length; num5++)
					{
						if (!string.IsNullOrEmpty(mappingSourceField[num5].TargetField) && !string.IsNullOrEmpty(mappingSourceField[num5].SourceField))
						{
							dataRow3[mappingSourceField[num5].TargetField] = dataTable3.Rows[num4][mappingSourceField[num5].SourceField];
						}
						if (mappingSourceField[num5].DfltValue != null && dataRow3[mappingSourceField[num5].TargetField] == DBNull.Value)
						{
							dataRow3[mappingSourceField[num5].TargetField] = mappingSourceField[num5].DfltValue;
						}
					}
					dataTable6.Rows.Add(dataRow3);
				}
				DataTable dataTable7 = ContactEmployees.Clone();
				for (int num6 = 0; num6 < dataTable4.Rows.Count; num6++)
				{
					DataRow dataRow4 = dataTable7.NewRow();
					for (int num7 = 0; num7 < mappingSourceField2.Length; num7++)
					{
						if (!string.IsNullOrEmpty(mappingSourceField2[num7].TargetField) && !string.IsNullOrEmpty(mappingSourceField2[num7].SourceField))
						{
							dataRow4[mappingSourceField2[num7].TargetField] = dataTable4.Rows[num6][mappingSourceField2[num7].SourceField];
						}
						if (mappingSourceField2[num7].DfltValue != null && dataRow4[mappingSourceField2[num7].TargetField] == DBNull.Value)
						{
							dataRow4[mappingSourceField2[num7].TargetField] = mappingSourceField2[num7].DfltValue;
						}
					}
					dataTable7.Rows.Add(dataRow4);
				}
				BPImpls bPImpls = dataRow2.ToBusinessPartner();
				if (bPImpls == null)
				{
					throw new Exception("Could not get data - business Partner - general");
				}
				bPImpls.Addresses = dataTable6.ToBpAddresses();
				bPImpls.ListContactPerson = dataTable7.ToContactEmployees();
				list.Add(bPImpls);
			}
			if (list3.Count == 0)
			{
				break;
			}
		}
		return list;
	}

	public ContextActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable dtHeader)
	{
		switch (actionType)
		{
		case Enums.ContextActionActionTypeEx.ChartOfAccount:
			ChartOfAccount = dtHeader;
			break;
		case Enums.ContextActionActionTypeEx.UserDefinedObject:
			UdoTable = dtHeader;
			break;
		}
	}

	public List<ChartOfAccountsImpl> GetMappingChartOfAccount(DataTable source, DataSet dsAction)
	{
		List<ChartOfAccountsImpl> list = new List<ChartOfAccountsImpl>();
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable dataTable = new DataTable();
		if (dsAction.Tables.Count >= 3)
		{
			dataTable = dsAction.Tables[2];
		}
		DataTable dtMapping = new DataTable();
		DataRow[] array = dataTable.Select("TargetItem='H'");
		if (array.Length != 0)
		{
			dtMapping = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
		DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = BusinessPartner.Clone();
			dataTable2.Rows.Add(dataTable2.NewRow());
		}
		foreach (DataRow row in dataTable2.Rows)
		{
			DataRow dataRow2 = ChartOfAccount.Clone().NewRow();
			for (int num = 0; num < mappingSourceField.Length; num++)
			{
				if (!string.IsNullOrEmpty(mappingSourceField[num].SourceField))
				{
					dataRow2[mappingSourceField[num].TargetField] = row[mappingSourceField[num].SourceField];
				}
				if (mappingSourceField[num].DfltValue != null && dataRow2[mappingSourceField[num].TargetField] == DBNull.Value)
				{
					dataRow2[mappingSourceField[num].TargetField] = mappingSourceField[num].DfltValue;
				}
			}
			ChartOfAccountsImpl item = EntitiesHelper<ChartOfAccountsImpl>.ToEntity(dataRow2);
			list.Add(item);
		}
		return list;
	}

	public ContextActionQuery GetQuery(DataSet ds, DataTable inputSource)
	{
		ContextActionQuery contextActionQuery = new ContextActionQuery();
		DataTable dataTable = ds.Tables[0];
		Enums.ContextActionDataFrom contextActionDataFrom = (Enums.ContextActionDataFrom)(Function.ParseChar(dataTable.Rows[0]["DataFrom"]) ?? 'T');
		string text = Function.ToString(dataTable.Rows[0]["DataString"]);
		string text2 = Function.ToString(ds.Tables[0].Rows[0]["DataParamType"]);
		DataTable dataTable2 = new DataTable();
		List<QueryParamater> list = new List<QueryParamater>();
		DataTable dataTable3 = ds.Tables[1];
		if (dataTable3.IsNotNull())
		{
			EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable3.AsEnumerable()
				where Function.ToString(t["ParamType"]) == "S"
				select t;
			if (text2 == "T")
			{
				string text3 = Function.ToString(ds.Tables[0].Rows[0]["TempTblName"]);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = "tbltemp";
				}
				foreach (DataRow item in enumerableRowCollection)
				{
					DataColumn dataColumn = new DataColumn();
					string text4 = (dataColumn.ColumnName = Function.ToString(item["SourceField"]));
					if (!string.IsNullOrEmpty(text4))
					{
						string text6 = Function.ToString(item["DefaultValue"]);
						if (!string.IsNullOrEmpty(text6))
						{
							dataColumn.DefaultValue = text6;
						}
						if (inputSource != null && inputSource.ExistsColumn(text4))
						{
							dataColumn.DataType = inputSource.Columns[text4].DataType;
						}
						else
						{
							dataColumn.DataType = typeof(string);
						}
						if (!dataTable2.ExistsColumn(text4))
						{
							dataTable2.Columns.Add(dataColumn);
						}
					}
				}
				if (dataTable2.Columns.Count == 0 && inputSource != null)
				{
					dataTable2 = inputSource.Clone();
				}
				dataTable2.TableName = text3;
				SetTemporaryTableValue(inputSource, dataTable2);
			}
			else
			{
				foreach (DataRow item2 in enumerableRowCollection)
				{
					QueryParamater queryParamater = new QueryParamater
					{
						ParaName = Function.ToString(item2["ParamName"])
					};
					string sourceField = Function.ToString(item2["SourceField"]);
					if (!string.IsNullOrEmpty(sourceField) && inputSource.ExistsColumn(sourceField))
					{
						string value = string.Join(",", from t in inputSource.AsEnumerable()
							select Function.ToString(t[sourceField]));
						if (string.IsNullOrEmpty(value))
						{
							value = Function.ToString(item2["DefaultValue"]);
						}
						queryParamater.Value = value;
					}
					list.Add(queryParamater);
				}
			}
		}
		contextActionQuery.DataFrom = contextActionDataFrom;
		contextActionQuery.DataParamType = text2;
		contextActionQuery.Paramaters = list;
		switch (contextActionDataFrom)
		{
		case Enums.ContextActionDataFrom.DefinedQuery:
			contextActionQuery.QueryId = Function.ParseInt(text);
			break;
		case Enums.ContextActionDataFrom.QueryText:
			contextActionQuery.QueryText = text;
			break;
		}
		contextActionQuery.TemporaryTable = dataTable2;
		return contextActionQuery;
	}

	private void SetTemporaryTableValue(DataTable ui, DataTable dtTemporary)
	{
		try
		{
			foreach (DataRow row in ui.Rows)
			{
				DataRow dataRow2 = dtTemporary.NewRow();
				foreach (DataColumn column in dtTemporary.Columns)
				{
					if (ui.ExistsColumn(column.ColumnName))
					{
						dataRow2[column.ColumnName] = row[column.ColumnName];
					}
				}
				dtTemporary.Rows.Add(dataRow2);
			}
			dtTemporary.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public ContextActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable header, DataTable detail1, DataTable detail2)
	{
		switch (actionType)
		{
		case Enums.ContextActionActionTypeEx.Items:
			Items = header;
			ItemPrices = detail1;
			ItemWarehouses = detail2;
			break;
		case Enums.ContextActionActionTypeEx.BusinessBusinessPartner:
			BusinessPartner = header;
			BPAddresses = detail1;
			ContactEmployees = detail2;
			break;
		}
	}

	public static object SetDefaultMapping(ContextActionMappingImpl[] mapHeaderField, int i, DataTable dtDoc)
	{
		if (dtDoc.Columns[mapHeaderField[i].TargetField].DataType == typeof(DateTime) || dtDoc.Columns[mapHeaderField[i].TargetField].DataType == typeof(DateTime?))
		{
			if (Function.ToString(mapHeaderField[i].DfltValue).ToUpper() == "DATETIME.TODAY")
			{
				return DateTime.Today;
			}
			return mapHeaderField[i].DfltValue;
		}
		return mapHeaderField[i].DfltValue;
	}

	public ContextActionHandle(Enums.ContextActionActionTypeEx actionType, DataTable dtHeader, DataTable dtLine)
	{
		switch (actionType)
		{
		case Enums.ContextActionActionTypeEx.DistributionRule:
			OcrTable = dtHeader;
			PrcTable = dtLine;
			break;
		case Enums.ContextActionActionTypeEx.VatGroup:
			VatGroupHeader = dtHeader;
			VatGroupLines = dtLine;
			break;
		case Enums.ContextActionActionTypeEx.JournalEntry:
			JournalEntry = dtHeader;
			JournalEntryRows = dtLine;
			break;
		}
	}

	public List<DataSet> GetMappingDistributeRule(DataTable source, DataSet dsAction)
	{
		try
		{
			if (!source.IsNotNull())
			{
				throw new Exception("Datasource is Empty");
			}
			DataTable dataTable = new DataTable();
			if (dsAction.Tables.Count >= 3)
			{
				dataTable = dsAction.Tables[2];
			}
			DataTable dtMapping = new DataTable();
			DataTable dtMapping2 = new DataTable();
			DataRow[] array = dataTable.Select("TargetItem='H'");
			if (array.Length != 0)
			{
				dtMapping = array.CopyToDataTable();
			}
			array = dataTable.Select("TargetItem='I'");
			if (array.Length != 0)
			{
				dtMapping2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
			ContextActionMappingImpl[] mappingSourceField2 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping2, source);
			DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable2.IsNotNull())
			{
				dataTable2 = OcrTable.Clone();
				dataTable2.Rows.Add(dataTable2.NewRow());
			}
			List<DataSet> list = new List<DataSet>();
			foreach (DataRow item in dataTable2.Rows)
			{
				DataTable dataTable3 = OcrTable.Clone();
				DataRow dataRow = dataTable3.NewRow();
				for (int num = 0; num < mappingSourceField.Length; num++)
				{
					if (!dataTable3.ExistsColumn(mappingSourceField[num].TargetField))
					{
						continue;
					}
					DataColumn dataColumn = dataTable3.Columns[mappingSourceField[num].TargetField];
					if (!string.IsNullOrEmpty(mappingSourceField[num].SourceField))
					{
						if (dataColumn.DataType == typeof(DateTime))
						{
							DateTime? dateTime = Function.ParseDateTime(item[mappingSourceField[num].SourceField]);
							if (dateTime.HasValue)
							{
								dataRow[mappingSourceField[num].TargetField] = dateTime;
							}
						}
						else
						{
							dataRow[mappingSourceField[num].TargetField] = item[mappingSourceField[num].SourceField];
						}
					}
					if (mappingSourceField[num].DfltValue == null || dataRow[mappingSourceField[num].TargetField] != DBNull.Value)
					{
						continue;
					}
					if (dataColumn.DataType == typeof(DateTime))
					{
						DateTime? dateTime2 = Function.ParseDateTime(mappingSourceField[num].DfltValue);
						if (dateTime2.HasValue)
						{
							dataRow[mappingSourceField[num].TargetField] = dateTime2;
						}
					}
					else
					{
						dataRow[mappingSourceField[num].TargetField] = mappingSourceField[num].DfltValue;
					}
				}
				ContextActionMappingImpl[] grpBatch = mappingSourceField.Where((ContextActionMappingImpl x) => Function.ToString(x.SumType) == "").ToArray();
				DataRow[] array2 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in grpBatch
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in grpBatch
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				DataTable dataTable4 = PrcTable.Clone();
				if (array2.Length != 0)
				{
					DataTable dataTable5 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField2
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					for (int num2 = 0; num2 < dataTable5.Rows.Count; num2++)
					{
						DataRow dataRow2 = dataTable4.NewRow();
						for (int num3 = 0; num3 < mappingSourceField2.Length; num3++)
						{
							if (!string.IsNullOrEmpty(mappingSourceField2[num3].TargetField) && !string.IsNullOrEmpty(mappingSourceField2[num3].SourceField))
							{
								dataRow2[mappingSourceField2[num3].TargetField] = dataTable5.Rows[num2][mappingSourceField2[num3].SourceField];
							}
							if (mappingSourceField2[num3].DfltValue != null && dataRow2[mappingSourceField2[num3].TargetField] == DBNull.Value)
							{
								dataRow2[mappingSourceField2[num3].TargetField] = mappingSourceField2[num3].DfltValue;
							}
						}
						dataTable4.Rows.Add(dataRow2);
					}
				}
				dataTable3.Rows.Add(dataRow);
				DataSet dataSet = new DataSet();
				dataSet.Tables.AddRange(new DataTable[2]
				{
					dataTable3.Copy(),
					dataTable4.Copy()
				});
				list.Add(dataSet);
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw ex;
		}
	}

	public ContextActionHandle()
	{
	}

	public ContextActionHandle(DataTable document, DataTable lines, DataTable batchserial)
	{
		DocumentTable = document;
		DocumentLineTable = lines;
		BatchSerialNumbersStruct = batchserial;
	}

	public List<DocumentsImpl> GetMappingDocument(DataTable source, DataSet ds)
	{
		DataTable dataTable = ds.Tables[0];
		Enums.ContextActionMethod contextActionMethod = (Enums.ContextActionMethod)(Function.ParseChar(dataTable.Rows[0]["Method"]) ?? 'A');
		string text = Function.ToString(dataTable.Rows[0]["ObjectId"]);
		Enums.ContextActionDocumentType objtype = (Enums.ContextActionDocumentType)Function.ParseInt(dataTable.Rows[0]["ObjType"]);
		DataTable dtMapping = new DataTable();
		if (ds.Tables.Count >= 3)
		{
			dtMapping = ds.Tables[2];
		}
		GetMappingDocument(source, dtMapping, out var mapHeaderField, out var mapItemField, out var mapBatchField);
		if (contextActionMethod == Enums.ContextActionMethod.Update)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("DocEntry is missing (mapping object id)");
			}
			ContextActionMappingImpl contextActionMappingImpl = mapHeaderField.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "DocEntry");
			if (contextActionMappingImpl != null)
			{
				contextActionMappingImpl.SourceField = text;
			}
			else
			{
				ContextActionMappingImpl contextActionMappingImpl2 = new ContextActionMappingImpl();
				contextActionMappingImpl2.TargetField = "DocEntry";
				contextActionMappingImpl2.SourceField = text;
				contextActionMappingImpl2.DfltValue = 0;
				List<ContextActionMappingImpl> list = mapHeaderField.ToList();
				list.Add(contextActionMappingImpl2);
				mapHeaderField = list.ToArray();
			}
		}
		List<DocumentsImpl> list2 = new List<DocumentsImpl>();
		DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = DocumentTable.Clone();
			dataTable2.Rows.Add(dataTable2.NewRow());
		}
		foreach (DataRow row in dataTable2.Rows)
		{
			DataTable mappingLines = GetMappingLines(source, mapHeaderField, row, mapItemField);
			if (mappingLines.IsNotNull())
			{
				DocumentsImpl document = GetDocument(source, objtype, mapHeaderField, row, mapItemField, mappingLines, mapBatchField, contextActionMethod);
				list2.Add(document);
			}
		}
		return list2;
	}

	public static DataTable GetMappingLines(DataTable source, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] mapItemField)
	{
		ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
		DataRow[] array = (from r in source.AsEnumerable()
			where new NTuple<object>(from map in groupcol
				where !string.IsNullOrEmpty(map.SourceField)
				select new
				{
					Name = map.SourceField,
					Value = item[map.SourceField]
				}).Equals(new NTuple<object>(from map in groupcol
				where !string.IsNullOrEmpty(map.SourceField)
				select new
				{
					Name = map.SourceField,
					Value = r[map.SourceField]
				}))
			select (r)).ToArray();
		if (array.Length != 0)
		{
			return array.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
		}
		return new DataTable();
	}

	public DocumentsImpl GetDocument(DataTable source, Enums.ContextActionDocumentType objtype, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] mapItemField, DataTable line, ContextActionMappingImpl[] mapBatchField, Enums.ContextActionMethod method)
	{
		DataTable dataTable = DocumentTable.Clone();
		DataRow dataRow = dataTable.NewRow();
		for (int i = 0; i < mapHeaderField.Length; i++)
		{
			if (!string.IsNullOrEmpty(mapHeaderField[i].SourceField))
			{
				dataRow[mapHeaderField[i].TargetField] = item[mapHeaderField[i].SourceField];
			}
			if (mapHeaderField[i].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapHeaderField[i].DfltValue)) && dataRow[mapHeaderField[i].TargetField] == DBNull.Value)
			{
				dataRow[mapHeaderField[i].TargetField] = SetDefaultMapping(mapHeaderField, i, dataTable);
			}
		}
		ContextActionMappingImpl[] array = mapItemField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable2 = line.Clone();
		if (array.Length != 0)
		{
			ContextActionMappingImpl[] groupcolLine = mapItemField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			foreach (IGrouping<NTuple<object>, DataRow> item3 in from r in line.AsEnumerable()
				group r by new NTuple<object>(from map in groupcolLine
					where !string.IsNullOrEmpty(map.SourceField)
					select new
					{
						Name = map.SourceField,
						Value = r[map.SourceField]
					}))
			{
				DataRow dataRow2 = dataTable2.NewRow();
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				object[] values = item3.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item2)
					{
						list.Add(item2);
					}
				}
				ContextActionMappingImpl[] array2 = groupcolLine;
				foreach (ContextActionMappingImpl col in array2)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow2[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array2 = array;
				foreach (ContextActionMappingImpl col2 in array2)
				{
					dataRow2[col2.SourceField] = item3.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable2.Rows.Add(dataRow2);
			}
		}
		if (dataTable2.IsNotNull())
		{
			line = dataTable2;
		}
		DocumentsImpl documentsImpl = dataRow.ToDocument();
		if (documentsImpl != null)
		{
			documentsImpl.Lines = new List<Document_LinesImpl>();
			for (int i2 = 0; i2 < line.Rows.Count; i2++)
			{
				DataRow dataRow3 = DocumentLineTable.Clone().NewRow();
				if (method == Enums.ContextActionMethod.AddNew)
				{
					dataRow3["LineNum"] = i2;
				}
				for (int num2 = 0; num2 < mapItemField.Length; num2++)
				{
					if (!string.IsNullOrEmpty(mapItemField[num2].TargetField) && !string.IsNullOrEmpty(mapItemField[num2].SourceField) && line.Rows[i2][mapItemField[num2].SourceField] != DBNull.Value)
					{
						dataRow3[mapItemField[num2].TargetField] = line.Rows[i2][mapItemField[num2].SourceField];
					}
					if (mapItemField[num2].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapItemField[num2].DfltValue)) && dataRow3[mapItemField[num2].TargetField] == DBNull.Value)
					{
						dataRow3[mapItemField[num2].TargetField] = SetDefaultMapping(mapItemField, num2, DocumentLineTable);
					}
				}
				List<Document_LinesImpl> list2 = new DataRow[1] { dataRow3 }.CopyToDataTable().ToDocumentLine();
				if (list2.Count != 1)
				{
					continue;
				}
				Document_LinesImpl document_LinesImpl = list2[0];
				if (mapBatchField.Length != 0)
				{
					ContextActionMappingImpl[] grpBatch = mapItemField.Where((ContextActionMappingImpl x) => Function.ToString(x.SumType) == "").ToArray();
					DataRow[] array3 = (from r in source.AsEnumerable()
						where new NTuple<object>(from map in grpBatch
							where !string.IsNullOrEmpty(map.SourceField)
							select new
							{
								Name = map.SourceField,
								Value = line.Rows[i2][map.SourceField]
							}).Equals(new NTuple<object>(from map in grpBatch
							where !string.IsNullOrEmpty(map.SourceField)
							select new
							{
								Name = map.SourceField,
								Value = r[map.SourceField]
							}))
						select (r)).ToArray();
					if (array3.Length != 0)
					{
						DataTable dataTable3 = DocumentsHelper.BatchSerialNumbersStruct.Clone();
						if (!dataTable3.Columns.Contains("ToBinAbs"))
						{
							dataTable3.Columns.Add("ToBinAbs", typeof(int));
						}
						for (int num3 = 0; num3 < array3.Length; num3++)
						{
							DataRow dataRow4 = dataTable3.NewRow();
							dataTable3.Rows.Add(dataRow4);
							for (int num4 = 0; num4 < mapBatchField.Length; num4++)
							{
								if (string.IsNullOrEmpty(mapBatchField[num4].TargetField))
								{
									continue;
								}
								if (!string.IsNullOrEmpty(mapBatchField[num4].SourceField))
								{
									if (dataTable3.Columns[mapBatchField[num4].TargetField].DataType == typeof(DateTime))
									{
										if (Function.ParseDateTime(array3[num3][mapBatchField[num4].SourceField]).HasValue)
										{
											dataRow4[mapBatchField[num4].TargetField] = Function.ParseDateTime(array3[num3][mapBatchField[num4].SourceField]);
										}
										else
										{
											dataRow4[mapBatchField[num4].TargetField] = DBNull.Value;
										}
									}
									else
									{
										dataRow4[mapBatchField[num4].TargetField] = array3[num3][mapBatchField[num4].SourceField];
									}
								}
								if (mapBatchField[num4].DfltValue != null && DBNull.Value == dataRow4[mapBatchField[num4].TargetField])
								{
									if (dataTable3.Columns[mapBatchField[num4].TargetField].DataType == typeof(DateTime))
									{
										if (Function.ParseDateTime(mapBatchField[num4].DfltValue).HasValue)
										{
											dataRow4[mapBatchField[num4].TargetField] = Function.ParseDateTime(mapBatchField[num4].DfltValue);
										}
									}
									else
									{
										dataRow4[mapBatchField[num4].TargetField] = mapBatchField[num4].DfltValue;
									}
								}
								dataRow4.AcceptChanges();
							}
							dataRow4["BaseLineNumber"] = document_LinesImpl.LineNum;
							dataRow4["TranType"] = 1;
						}
						document_LinesImpl.LoadDataBatch(dataTable3, GetObjectTypesFromContext(objtype));
					}
				}
				documentsImpl.Lines.Add(document_LinesImpl);
			}
			return documentsImpl;
		}
		throw new Exception("Could not get data - document header");
	}

	public ApzObjectTypes GetObjectTypesFromContext(Enums.ContextActionDocumentType objtype)
	{
		return objtype switch
		{
			Enums.ContextActionDocumentType.oQuotations => ApzObjectTypes.oQuotations, 
			Enums.ContextActionDocumentType.oOrders => ApzObjectTypes.oOrders, 
			Enums.ContextActionDocumentType.oDeliveryNotes => ApzObjectTypes.oDeliveryNotes, 
			Enums.ContextActionDocumentType.oReturns => ApzObjectTypes.oReturns, 
			Enums.ContextActionDocumentType.oInvoices => ApzObjectTypes.oInvoices, 
			Enums.ContextActionDocumentType.oReserveInvoices => ApzObjectTypes.oReserveInvoices, 
			Enums.ContextActionDocumentType.oCreditNotes => ApzObjectTypes.oCreditNotes, 
			Enums.ContextActionDocumentType.oPurchaseRequest => ApzObjectTypes.oPurchaseRequest, 
			Enums.ContextActionDocumentType.oPurchaseQuotations => ApzObjectTypes.oPurchaseQuotations, 
			Enums.ContextActionDocumentType.oPurchaseOrders => ApzObjectTypes.oPurchaseOrders, 
			Enums.ContextActionDocumentType.oGoodsReceiptPO => ApzObjectTypes.oPurchaseDeliveryNotes, 
			Enums.ContextActionDocumentType.oGoodsReturns => ApzObjectTypes.oPurchaseReturns, 
			Enums.ContextActionDocumentType.oAPInvoices => ApzObjectTypes.oPurchaseInvoices, 
			Enums.ContextActionDocumentType.oAPCreditNotes => ApzObjectTypes.oPurchaseCreditNotes, 
			Enums.ContextActionDocumentType.oAPReserveInvoice => ApzObjectTypes.oAPReserveInvoice, 
			Enums.ContextActionDocumentType.oInventoryTransferRequest => ApzObjectTypes.oInventoryTransferRequest, 
			Enums.ContextActionDocumentType.oInventoryTransfer => ApzObjectTypes.oStockTransfer, 
			Enums.ContextActionDocumentType.oGoodsReceipt => ApzObjectTypes.oInventoryGenEntry, 
			Enums.ContextActionDocumentType.oGoodsIssue => ApzObjectTypes.oInventoryGenExit, 
			Enums.ContextActionDocumentType.oDownPaymentRequest => ApzObjectTypes.oDownPayments, 
			Enums.ContextActionDocumentType.oPurchaseDownPaymentRequest => ApzObjectTypes.oPurchaseDownPayments, 
			Enums.ContextActionDocumentType.oDraft => ApzObjectTypes.oDrafts, 
			Enums.ContextActionDocumentType.oReturnRequest => ApzObjectTypes.oReturnRequest, 
			_ => ApzObjectTypes.oQuotations, 
		};
	}

	public void GetMappingDocument(DataTable source, DataTable dtMapping, out ContextActionMappingImpl[] mapHeaderField, out ContextActionMappingImpl[] mapItemField, out ContextActionMappingImpl[] mapBatchField)
	{
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable dtMapping2 = new DataTable();
		DataTable source2 = new DataTable();
		DataTable dtMapping3 = new DataTable();
		DataRow[] array = dtMapping.Select("MappingType='D' AND TargetItem='H'");
		if (array.Length != 0)
		{
			dtMapping2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='D' AND TargetItem='I'");
		if (array.Length != 0)
		{
			source2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='D' AND TargetItem='B'");
		if (array.Length != 0)
		{
			dtMapping3 = array.CopyToDataTable();
		}
		mapHeaderField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping2, source);
		List<ContextActionMappingImpl> list = (from t in source2.AsEnumerable()
			where new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToList();
		list.AddRange((from t in source2.AsEnumerable()
			where !new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToArray());
		mapItemField = ContextActionDocumentHelper.ResetSourceFieldByDataSource(list.ToArray(), source);
		mapBatchField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping3, source);
		ContextActionMappingImpl[] array2 = mapHeaderField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable = source.Clone();
		if (array2.Length != 0)
		{
			ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in source.AsEnumerable()
				group r by new NTuple<object>(from map in groupcol
					where !string.IsNullOrEmpty(map.SourceField)
					select new
					{
						Name = map.SourceField,
						Value = r[map.SourceField]
					}))
			{
				DataRow dataRow = dataTable.NewRow();
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				object[] values = item2.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
					{
						list2.Add(item);
					}
				}
				ContextActionMappingImpl[] array3 = groupcol;
				foreach (ContextActionMappingImpl col in array3)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list2.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array3 = array2;
				foreach (ContextActionMappingImpl col2 in array3)
				{
					dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable.Rows.Add(dataRow);
			}
		}
		dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable.IsNotNull())
		{
			dataTable = DocumentTable.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		if (dataTable.IsNotNull())
		{
			source = dataTable;
		}
	}

	public List<ItemsImpl> GetMappingItem(DataTable source, DataSet dsAction)
	{
		List<ItemsImpl> list = new List<ItemsImpl>();
		DataTable dataTable = new DataTable();
		if (dsAction.Tables.Count >= 3)
		{
			dataTable = dsAction.Tables[2];
		}
		string text = Function.ToString(dsAction.Tables[0].Rows[0]["Method"]);
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable dtMapping = new DataTable();
		DataTable dtMapping2 = new DataTable();
		DataTable dtMapping3 = new DataTable();
		DataRow[] array = dataTable.Select("MappingType='4' AND TargetItem='H'");
		if (array.Length != 0)
		{
			dtMapping = array.CopyToDataTable();
		}
		array = dataTable.Select("MappingType='4' AND TargetItem='P'");
		if (array.Length != 0)
		{
			dtMapping2 = array.CopyToDataTable();
		}
		array = dataTable.Select("MappingType='4' AND TargetItem='W'");
		if (array.Length != 0)
		{
			dtMapping3 = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
		ContextActionMappingImpl[] mappingSourceField2 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping2, source);
		ContextActionMappingImpl[] mappingSourceField3 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping3, source);
		List<string> list2 = new List<string>();
		int num = 0;
		for (int i = 0; i < mappingSourceField.Length; i++)
		{
			if (!string.IsNullOrEmpty(mappingSourceField[i].SourceField))
			{
				list2.Add("ISNULL(" + mappingSourceField[i].SourceField + ",'')='{" + num + "}'");
				num++;
			}
		}
		DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = Items.Clone();
			dataTable2.Rows.Add(dataTable2.NewRow());
		}
		int num2 = 1;
		foreach (DataRow row in dataTable2.Rows)
		{
			List<object> list3 = new List<object>();
			for (int num3 = 0; num3 < mappingSourceField.Length; num3++)
			{
				if (!string.IsNullOrEmpty(mappingSourceField[num3].SourceField))
				{
					list3.Add((row[mappingSourceField[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToStringNotTrim(row[mappingSourceField[num3].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", list2), list3.ToArray()));
			if (array2.Length != 0)
			{
				DataTable dataTable3 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField2
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable4 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField3
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				num2++;
				DataTable dataTable5 = Items.Clone();
				DataRow dataRow2 = dataTable5.NewRow();
				for (int num4 = 0; num4 < mappingSourceField.Length; num4++)
				{
					if (!string.IsNullOrEmpty(mappingSourceField[num4].SourceField))
					{
						dataRow2[mappingSourceField[num4].TargetField] = row[mappingSourceField[num4].SourceField];
					}
					if (mappingSourceField[num4].DfltValue != null && dataRow2[mappingSourceField[num4].TargetField] == DBNull.Value)
					{
						dataRow2[mappingSourceField[num4].TargetField] = mappingSourceField[num4].DfltValue;
					}
				}
				dataTable5.Rows.Add(dataRow2);
				DataTable dataTable6 = ItemPrices.Clone();
				for (int num5 = 0; num5 < dataTable3.Rows.Count; num5++)
				{
					DataRow dataRow3 = dataTable6.NewRow();
					for (int num6 = 0; num6 < mappingSourceField2.Length; num6++)
					{
						if (!string.IsNullOrEmpty(mappingSourceField2[num6].TargetField) && !string.IsNullOrEmpty(mappingSourceField2[num6].SourceField))
						{
							dataRow3[mappingSourceField2[num6].TargetField] = dataTable3.Rows[num5][mappingSourceField2[num6].SourceField];
						}
						if (mappingSourceField2[num6].DfltValue != null && dataRow3[mappingSourceField2[num6].TargetField] == DBNull.Value)
						{
							dataRow3[mappingSourceField2[num6].TargetField] = mappingSourceField2[num6].DfltValue;
						}
					}
					dataTable6.Rows.Add(dataRow3);
				}
				DataTable dataTable7 = ItemWarehouses.Clone();
				for (int num7 = 0; num7 < dataTable4.Rows.Count; num7++)
				{
					DataRow dataRow4 = dataTable7.NewRow();
					for (int num8 = 0; num8 < mappingSourceField3.Length; num8++)
					{
						if (!string.IsNullOrEmpty(mappingSourceField3[num8].TargetField) && !string.IsNullOrEmpty(mappingSourceField3[num8].SourceField))
						{
							dataRow4[mappingSourceField3[num8].TargetField] = dataTable3.Rows[num7][mappingSourceField3[num8].SourceField];
						}
						if (mappingSourceField3[num8].DfltValue != null && dataRow4[mappingSourceField3[num8].TargetField] == DBNull.Value)
						{
							dataRow4[mappingSourceField3[num8].TargetField] = mappingSourceField3[num8].DfltValue;
						}
					}
					dataTable7.Rows.Add(dataRow4);
				}
				ItemsImpl itemsImpl = dataRow2.ToItems();
				if (itemsImpl == null)
				{
					throw new Exception("Could not get data - items - general");
				}
				if (itemsImpl.validFor != "Y" && itemsImpl.frozenFor != "Y")
				{
					itemsImpl.validFor = "Y";
				}
				itemsImpl.ListITM1 = dataTable6.ToItemPrices();
				itemsImpl.OitwLines = dataTable7.ToItemWarehouses();
				itemsImpl.APZ_SAVE_ACTION = ((text == "U") ? "U" : "A");
				list.Add(itemsImpl);
			}
			if (list2.Count == 0)
			{
				break;
			}
		}
		return list;
	}

	public List<JournalEntriesImpl> GetMappingJournalEntry(DataTable source, DataTable dtMapping)
	{
		List<JournalEntriesImpl> list = new List<JournalEntriesImpl>();
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable source2 = new DataTable();
		DataTable source3 = new DataTable();
		DataRow[] array = dtMapping.Select("MappingType='J' AND TargetItem='H'");
		if (array.Length != 0)
		{
			source2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='J' AND TargetItem='I'");
		if (array.Length != 0)
		{
			source3 = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] array2 = (from t in source2.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		ContextActionMappingImpl[] array3 = (from t in source3.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		List<string> list2 = new List<string>();
		int num = 0;
		for (int num2 = 0; num2 < array2.Length; num2++)
		{
			if (!string.IsNullOrEmpty(array2[num2].SourceField))
			{
				list2.Add("ISNULL(" + array2[num2].SourceField + ",'')='{" + num + "}'");
				num++;
			}
		}
		DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in array2
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable.IsNotNull())
		{
			dataTable = JournalEntry.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		foreach (DataRow row in dataTable.Rows)
		{
			List<object> list3 = new List<object>();
			for (int num3 = 0; num3 < array2.Length; num3++)
			{
				if (!string.IsNullOrEmpty(array2[num3].SourceField))
				{
					list3.Add((row[array2[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToStringNotTrim(row[array2[num3].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array4 = source.Select(string.Format(string.Join(" AND ", list2), list3.ToArray()));
			if (array4.Length != 0)
			{
				DataTable dataTable2 = array4.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in array3
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable3 = JournalEntry.Clone();
				DataRow dataRow2 = dataTable3.NewRow();
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					if (!string.IsNullOrEmpty(array2[num4].SourceField))
					{
						dataRow2[array2[num4].TargetField] = row[array2[num4].SourceField];
					}
					if (Function.ToString(array2[num4].DfltValue) != "" && dataRow2[array2[num4].TargetField] == DBNull.Value)
					{
						dataRow2[array2[num4].TargetField] = array2[num4].DfltValue;
					}
				}
				dataTable3.Rows.Add(dataRow2);
				DataTable dataTable4 = JournalEntryRows.Clone();
				for (int num5 = 0; num5 < dataTable2.Rows.Count; num5++)
				{
					DataRow dataRow3 = dataTable4.NewRow();
					dataRow3["Line_ID"] = num5;
					for (int num6 = 0; num6 < array3.Length; num6++)
					{
						if (!string.IsNullOrEmpty(array3[num6].TargetField) && !string.IsNullOrEmpty(array3[num6].SourceField))
						{
							dataRow3[array3[num6].TargetField] = dataTable2.Rows[num5][array3[num6].SourceField];
						}
						if (Function.ToString(array3[num6].DfltValue) != "" && dataRow3[array3[num6].TargetField] == DBNull.Value)
						{
							dataRow3[array3[num6].TargetField] = array3[num6].DfltValue;
						}
					}
					dataTable4.Rows.Add(dataRow3);
				}
				JournalEntriesImpl journalEntriesImpl = EntitiesHelper<JournalEntriesImpl>.ToEntity(dataRow2);
				if (journalEntriesImpl == null)
				{
					throw new Exception("Could not get data - journal entry - general");
				}
				journalEntriesImpl.Lines = EntitiesHelper<JournalEntries_LinesImpl>.ToEntities(dataTable4);
				list.Add(journalEntriesImpl);
			}
			if (list2.Count == 0)
			{
				break;
			}
		}
		return list;
	}

	public DataTable GetmappingSourceSaleEmployee(DataTable dtStruct, DataTable source, DataTable dtMapping)
	{
		try
		{
			if (dtStruct == null || dtStruct.Columns.Count == 0)
			{
				throw new Exception("Data structure is missing");
			}
			if (!source.IsNotNull())
			{
				throw new Exception("Data source is missing");
			}
			if (!dtMapping.IsNotNull())
			{
				throw new Exception("Mapping Fields is missing");
			}
			ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = dtStruct.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			DataTable dataTable2 = dtStruct.Clone();
			DataRow dataRow = dataTable2.NewRow();
			for (int num = 0; num < mappingSourceField.Length; num++)
			{
				if (!dataTable2.ExistsColumn(mappingSourceField[num].TargetField))
				{
					continue;
				}
				DataColumn dataColumn = dataTable2.Columns[mappingSourceField[num].TargetField];
				if (!string.IsNullOrEmpty(mappingSourceField[num].SourceField))
				{
					if (dataColumn.DataType == typeof(DateTime))
					{
						DateTime? dateTime = Function.ParseDateTime(dataTable.Rows[0][mappingSourceField[num].SourceField]);
						if (dateTime.HasValue)
						{
							dataRow[mappingSourceField[num].TargetField] = dateTime;
						}
					}
					else
					{
						dataRow[mappingSourceField[num].TargetField] = dataTable.Rows[0][mappingSourceField[num].SourceField];
					}
				}
				if (mappingSourceField[num].DfltValue == null || dataRow[mappingSourceField[num].TargetField] != DBNull.Value)
				{
					continue;
				}
				if (dataColumn.DataType == typeof(DateTime))
				{
					DateTime? dateTime2 = Function.ParseDateTime(mappingSourceField[num].DfltValue);
					if (dateTime2.HasValue)
					{
						dataRow[mappingSourceField[num].TargetField] = dateTime2;
					}
				}
				else
				{
					dataRow[mappingSourceField[num].TargetField] = mappingSourceField[num].DfltValue;
				}
			}
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw ex;
		}
	}

	public DataSet GetSourceTask(DataSet ds, DataTable source, DataTable dtStruct)
	{
		try
		{
			if (!source.IsNotNull())
			{
				throw new Exception("Datasource is Empty");
			}
			DataTable dataTable = new DataTable();
			if (ds.Tables.Count >= 3)
			{
				dataTable = ds.Tables[2];
			}
			DataTable dtMapping = new DataTable();
			DataRow[] array = dataTable.Select("MappingType='A' AND TargetItem='H'");
			if (array.Length != 0)
			{
				dtMapping = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
			DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable2.IsNotNull())
			{
				dataTable2 = dtStruct.Clone();
				dataTable2.Rows.Add(dataTable2.NewRow());
			}
			int num = 1;
			foreach (DataRow row in dataTable2.Rows)
			{
				num++;
				DataTable dataTable3 = dtStruct.Clone();
				DataRow dataRow2 = dataTable3.NewRow();
				for (int num2 = 0; num2 < mappingSourceField.Length; num2++)
				{
					if (!dataTable3.ExistsColumn(mappingSourceField[num2].TargetField))
					{
						continue;
					}
					DataColumn dataColumn = dataTable3.Columns[mappingSourceField[num2].TargetField];
					if (!string.IsNullOrEmpty(mappingSourceField[num2].SourceField))
					{
						if (dataColumn.DataType == typeof(DateTime))
						{
							DateTime? dateTime = Function.ParseDateTime(row[mappingSourceField[num2].SourceField]);
							if (dateTime.HasValue)
							{
								dataRow2[mappingSourceField[num2].TargetField] = dateTime;
							}
						}
						else
						{
							dataRow2[mappingSourceField[num2].TargetField] = row[mappingSourceField[num2].SourceField];
						}
					}
					if (mappingSourceField[num2].DfltValue == null || dataRow2[mappingSourceField[num2].TargetField] != DBNull.Value)
					{
						continue;
					}
					if (dataColumn.DataType == typeof(DateTime))
					{
						DateTime? dateTime2 = Function.ParseDateTime(mappingSourceField[num2].DfltValue);
						if (dateTime2.HasValue)
						{
							dataRow2[mappingSourceField[num2].TargetField] = dateTime2;
						}
					}
					else
					{
						dataRow2[mappingSourceField[num2].TargetField] = mappingSourceField[num2].DfltValue;
					}
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["Closed"])))
				{
					dataRow2["Closed"] = "N";
				}
				dataRow2["Action"] = "T";
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Details"])))
				{
					if (Function.ToString(dataRow2["Closed"]) == "Y")
					{
						dataRow2["CloseDate"] = DateTime.Now;
						dataRow2["status"] = 2;
					}
					else
					{
						dataRow2["status"] = 1;
					}
					if (!Function.ParseDateTime(dataRow2["Recontact"]).HasValue)
					{
						dataRow2["Recontact"] = DateTime.Today;
					}
					dataTable3.Rows.Add(dataRow2);
					return new DataSet
					{
						Tables = { dataTable3.Copy() }
					};
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw ex;
		}
		return null;
	}

	public List<DataSet> GetMappingUserDefinedObject(DataTable source, DataSet ds)
	{
		List<DataSet> list = new List<DataSet>();
		if (!source.IsNotNull())
		{
			throw new Exception("Data source is missing");
		}
		DataTable dataTable = ds.Tables[0];
		DataTable dataTable2 = new DataTable();
		if (ds.Tables.Count >= 3)
		{
			dataTable2 = ds.Tables[2];
		}
		if (dataTable2.IsNotNull())
		{
			ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dataTable2, source);
			string udoCode = Function.ToString(dataTable.Rows[0]["UdoCode"]);
			ContextActionMappingImpl[] array = mappingSourceField.Where((ContextActionMappingImpl t) => t.TargetItem == udoCode).ToArray();
			DataTable dataTable3 = source.DefaultView.ToTable(distinct: true, (from t in array
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			DataSet udoStruct = GetUdoStruct(udoCode, array, mappingSourceField);
			if (dataTable3.IsNotNull())
			{
				foreach (DataRow row in dataTable3.Rows)
				{
					DataSet udo = GetUdo(udoStruct, udoCode, array, row, mappingSourceField, source);
					list.Add(udo);
					if (array.Length == 0)
					{
						break;
					}
				}
				return list;
			}
			throw new Exception("Header's data mapping is missing");
		}
		throw new Exception("Data mapping is missing");
	}

	private DataSet GetUdo(DataSet dsUdo, string udoCode, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] map, DataTable source)
	{
		DataSet dataSet = dsUdo.Clone();
		var array = (from t in map
			where t.TargetItem != udoCode
			group t by t.TargetItem into t
			select new
			{
				Key = t.Key,
				Items = t
			}).ToArray();
		DataRow dataRow = dataSet.Tables[0].NewRow();
		dataRow.ItemArray = dsUdo.Tables[0].Rows[0].ItemArray;
		dataSet.Tables[0].Rows.Add(dataRow);
		DataTable dataTable = dataSet.Tables[udoCode];
		DataRow dataRow2 = dataTable.NewRow();
		foreach (ContextActionMappingImpl contextActionMappingImpl in mapHeaderField)
		{
			if (!string.IsNullOrEmpty(contextActionMappingImpl.SourceField))
			{
				object obj = item[contextActionMappingImpl.SourceField];
				if (item.Table.Columns[contextActionMappingImpl.SourceField].DataType == typeof(DateTime))
				{
					DateTime? dateTime = Function.ParseDateTime(obj);
					if (dateTime.HasValue)
					{
						dataRow2[contextActionMappingImpl.TargetField] = dateTime;
					}
				}
				else
				{
					dataRow2[contextActionMappingImpl.TargetField] = obj;
				}
			}
			if (contextActionMappingImpl.DfltValue != null && DBNull.Value == dataRow2[contextActionMappingImpl.TargetField])
			{
				dataRow2[contextActionMappingImpl.TargetField] = contextActionMappingImpl.DfltValue;
			}
		}
		dataTable.Rows.Add(dataRow2);
		var array2 = array;
		foreach (var anon in array2)
		{
			DataTable dataTable2 = dataSet.Tables[anon.Key];
			ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			DataRow[] array3 = (from r in source.AsEnumerable()
				where new NTuple<object>(from m in groupcol
					where !string.IsNullOrEmpty(m.SourceField)
					select new
					{
						Name = m.SourceField,
						Value = item[m.SourceField]
					}).Equals(new NTuple<object>(from m in groupcol
					where !string.IsNullOrEmpty(m.SourceField)
					select new
					{
						Name = m.SourceField,
						Value = r[m.SourceField]
					}))
				select (r)).ToArray();
			if (array3.Length == 0)
			{
				continue;
			}
			int num2 = 1;
			DataRow[] array4 = array3;
			foreach (DataRow dataRow3 in array4)
			{
				DataRow dataRow4 = dataTable2.NewRow();
				foreach (ContextActionMappingImpl item2 in anon.Items)
				{
					if (!string.IsNullOrEmpty(item2.SourceField))
					{
						dataRow4[item2.TargetField] = dataRow3[item2.SourceField];
					}
					if (item2.DfltValue != null && DBNull.Value == dataRow4[item2.TargetField])
					{
						dataRow4[item2.TargetField] = item2.DfltValue;
					}
				}
				dataRow4["LineId"] = num2;
				dataTable2.Rows.Add(dataRow4);
				num2++;
			}
		}
		return dataSet;
	}

	private DataSet GetUdoStruct(string udoCode, ContextActionMappingImpl[] mapHeaderField, ContextActionMappingImpl[] map)
	{
		DataSet dataSet = new DataSet();
		string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
		dataSet.Tables.Add(UdoTable.Copy());
		DataTable dataTable = new DataTable(tableName);
		string text = Function.ToString(UdoTable.Rows[0]["TYPE"]);
		if (!(text == "1"))
		{
			if (text == "3")
			{
				dataTable.Columns.Add("DocEntry", typeof(int));
			}
		}
		else
		{
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
		}
		for (int i = 0; i < mapHeaderField.Length; i++)
		{
			string targetField = mapHeaderField[i].TargetField;
			if (!dataTable.ExistsColumn(targetField))
			{
				dataTable.Columns.Add(targetField, typeof(object));
			}
		}
		dataSet.Tables.Add(dataTable);
		var array = (from t in map
			where t.TargetItem != udoCode
			group t by t.TargetItem into t
			select new
			{
				Key = t.Key,
				Items = t
			}).ToArray();
		foreach (var anon in array)
		{
			DataTable dataTable2 = new DataTable(anon.Key);
			if (!(text == "1"))
			{
				if (text == "3")
				{
					dataTable2.Columns.Add("DocEntry", typeof(int));
				}
			}
			else
			{
				dataTable2.Columns.Add("Code", typeof(string));
			}
			dataTable2.Columns.Add("LineId", typeof(int));
			foreach (ContextActionMappingImpl item in anon.Items)
			{
				string targetField2 = item.TargetField;
				if (!dataTable2.ExistsColumn(targetField2))
				{
					dataTable2.Columns.Add(targetField2, typeof(object));
				}
			}
			dataSet.Tables.Add(dataTable2);
		}
		return dataSet;
	}

	public List<VatGroupsImpl> GetMappingVatGroups(DataTable source, DataSet dsAction)
	{
		List<VatGroupsImpl> list = new List<VatGroupsImpl>();
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable dataTable = new DataTable();
		if (dsAction.Tables.Count >= 3)
		{
			dataTable = dsAction.Tables[2];
		}
		DataTable dtMapping = new DataTable();
		DataTable dtMapping2 = new DataTable();
		DataRow[] array = dataTable.Select(" TargetItem='H'");
		if (array.Length != 0)
		{
			dtMapping = array.CopyToDataTable();
		}
		array = dataTable.Select("TargetItem='I'");
		if (array.Length != 0)
		{
			dtMapping2 = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] mappingSourceField = ContextActionDocumentHelper.GetMappingSourceField(dtMapping, source);
		ContextActionMappingImpl[] mappingSourceField2 = ContextActionDocumentHelper.GetMappingSourceField(dtMapping2, source);
		List<string> list2 = new List<string>();
		int num = 0;
		for (int i = 0; i < mappingSourceField.Length; i++)
		{
			if (!string.IsNullOrEmpty(mappingSourceField[i].SourceField))
			{
				list2.Add("ISNULL(" + mappingSourceField[i].SourceField + ",'')='{" + num + "}'");
				num++;
			}
		}
		DataTable dataTable2 = source.DefaultView.ToTable(distinct: true, (from t in mappingSourceField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = VatGroupHeader.Clone();
			dataTable2.Rows.Add(dataTable2.NewRow());
		}
		foreach (DataRow row in dataTable2.Rows)
		{
			List<object> list3 = new List<object>();
			for (int num2 = 0; num2 < mappingSourceField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mappingSourceField[num2].SourceField))
				{
					list3.Add((row[mappingSourceField[num2].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToStringNotTrim(row[mappingSourceField[num2].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", list2), list3.ToArray()));
			if (array2.Length != 0)
			{
				DataTable dataTable3 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mappingSourceField2
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable4 = VatGroupHeader.Clone();
				DataRow dataRow2 = dataTable4.NewRow();
				for (int num3 = 0; num3 < mappingSourceField.Length; num3++)
				{
					if (!string.IsNullOrEmpty(mappingSourceField[num3].SourceField))
					{
						dataRow2[mappingSourceField[num3].TargetField] = row[mappingSourceField[num3].SourceField];
					}
					if (mappingSourceField[num3].DfltValue != null && dataRow2[mappingSourceField[num3].TargetField] == DBNull.Value)
					{
						dataRow2[mappingSourceField[num3].TargetField] = mappingSourceField[num3].DfltValue;
					}
				}
				dataTable4.Rows.Add(dataRow2);
				DataTable dataTable5 = VatGroupLines.Clone();
				for (int num4 = 0; num4 < dataTable3.Rows.Count; num4++)
				{
					DataRow dataRow3 = dataTable5.NewRow();
					for (int num5 = 0; num5 < mappingSourceField2.Length; num5++)
					{
						if (!string.IsNullOrEmpty(mappingSourceField2[num5].TargetField) && !string.IsNullOrEmpty(mappingSourceField2[num5].SourceField))
						{
							dataRow3[mappingSourceField2[num5].TargetField] = dataTable3.Rows[num4][mappingSourceField2[num5].SourceField];
						}
						if (mappingSourceField2[num5].DfltValue != null && dataRow3[mappingSourceField2[num5].TargetField] == DBNull.Value)
						{
							dataRow3[mappingSourceField2[num5].TargetField] = mappingSourceField2[num5].DfltValue;
						}
					}
					dataTable5.Rows.Add(dataRow3);
				}
				VatGroupsImpl vatGroupsImpl = EntitiesHelper<VatGroupsImpl>.ToEntity(dataRow2);
				if (vatGroupsImpl == null)
				{
					throw new Exception("Could not get data - vat group - general");
				}
				vatGroupsImpl.VatGroups_Lines = EntitiesHelper<VatGroups_LinesImpl>.ToEntities(dataTable5);
				list.Add(vatGroupsImpl);
			}
			if (list2.Count == 0)
			{
				break;
			}
		}
		return list;
	}
}
