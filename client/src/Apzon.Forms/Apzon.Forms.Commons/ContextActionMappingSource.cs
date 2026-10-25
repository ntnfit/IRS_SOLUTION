using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.UserControls.BatchAndSerial;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.Commons;

public static class ContextActionMappingSource
{
	public static object ToType(this object obj, Type type)
	{
		object obj2 = Activator.CreateInstance(type);
		PropertyInfo[] properties = obj.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			try
			{
				obj2.GetType().GetProperty(propertyInfo.Name).SetValue(obj2, propertyInfo.GetValue(obj, null), null);
			}
			catch (Exception)
			{
			}
		}
		return obj2;
	}

	public static void MappingSource(this GridViewEx gv, DataTable source, ContextActionMappingImpl[] mapping, DataTable srcChild = null, ContextActionMappingImpl[] mappingChilds = null, ucBatchAndSerial uc = null, bool multirow = true)
	{
		if (!source.IsNotNull())
		{
			return;
		}
		if (!multirow)
		{
			MappingValue(source, mapping, 0, gv);
			return;
		}
		GridControlEx gridControlEx = gv.GridControl as GridControlEx;
		ContextActionMappingImpl[] array = mapping.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable = source.Clone();
		if (array.Length != 0)
		{
			ContextActionMappingImpl[] groupcol = mapping.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
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
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				object[] values = item2.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
					{
						list.Add(item);
					}
				}
				ContextActionMappingImpl[] array2 = groupcol;
				foreach (ContextActionMappingImpl col in array2)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array2 = array;
				foreach (ContextActionMappingImpl col2 in array2)
				{
					dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable.Rows.Add(dataRow);
			}
		}
		if (dataTable.IsNotNull())
		{
			source = dataTable;
		}
		int count = source.Rows.Count;
		if (gridControlEx != null)
		{
			List<string> list2 = new List<string>();
			ContextActionMappingImpl[] array3 = mapping.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			for (int num2 = 0; num2 < array3.Length; num2++)
			{
				if (!string.IsNullOrEmpty(array3[num2].SourceField))
				{
					list2.Add("ISNULL(" + array3[num2].SourceField + ",'')='{" + num2 + "}'");
				}
			}
			try
			{
				List<GridColumn> list3 = gv.Columns.Where((GridColumn t) => mapping.Any((ContextActionMappingImpl contextActionMappingImpl) => contextActionMappingImpl.TargetField == t.FieldName)).ToList();
				int num3 = list3.FindIndex((GridColumn gridColumn) => gridColumn.FieldName == "ItemCode");
				if (num3 > 0)
				{
					GridColumn value = list3[num3];
					list3[num3] = list3[0];
					list3[0] = value;
				}
				num3 = list3.FindIndex((GridColumn gridColumn) => gridColumn.FieldName == "LineTotal");
				if (num3 > 0)
				{
					GridColumn value2 = list3[num3];
					list3[num3] = list3[list3.Count - 1];
					list3[list3.Count - 1] = value2;
				}
				List<ContextActionMappingImpl> list4 = new List<ContextActionMappingImpl>();
				foreach (GridColumn x in list3)
				{
					if (!(x.FieldName == "LineVat") && !(x.FieldName == "VatSumFrgn"))
					{
						list4.Add(mapping.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == x.FieldName));
					}
				}
				for (int num4 = 0; num4 < count; num4++)
				{
					int num5 = (gv.FocusedRowHandle = gv.RowCount - 1);
					if (gv.FocusedRowHandle != num5)
					{
						return;
					}
					gv.BeginInsertRow();
					MappingValue(source, list4.ToArray(), num4, gv);
					if (uc == null || (Function.ToString(gv.GetFocusedRowCellValue(uc.FieldType)) != "B" && Function.ToString(gv.GetFocusedRowCellValue(uc.FieldType)) != "S") || mappingChilds == null || mappingChilds.Length == 0 || !srcChild.IsNotNull() || uc == null)
					{
						continue;
					}
					ContextActionMappingImpl[] grpBatch = array3.Where((ContextActionMappingImpl contextActionMappingImpl) => Function.ToString(contextActionMappingImpl.SumType) == "").ToArray();
					int i1 = num4;
					DataRow[] array4 = (from r in srcChild.AsEnumerable()
						where new NTuple<object>(from map in grpBatch
							where !string.IsNullOrEmpty(map.SourceField)
							select new
							{
								Name = map.SourceField,
								Value = source.Rows[i1][map.SourceField]
							}).Equals(new NTuple<object>(from map in grpBatch
							where !string.IsNullOrEmpty(map.SourceField)
							select new
							{
								Name = map.SourceField,
								Value = r[map.SourceField]
							}))
						select (r)).ToArray();
					if (array4.Length == 0)
					{
						continue;
					}
					uc.gvBatches.BeginInsertRow();
					try
					{
						for (int num7 = 0; num7 < array4.Length; num7++)
						{
							uc.gvBatches.FocusedRowHandle = num7;
							MappingValue(array4.CopyToDataTable(), mappingChilds, num7, uc.gvBatches);
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
					uc.gvBatches.EndInsertRow();
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				WinCommons.ErrorLineNumber(ex2);
			}
		}
		gv.EndInsertRow();
	}

	public static void MappingValue(DataTable source, ContextActionMappingImpl[] mapping, int sourceIndex, GridViewEx gv)
	{
		GridControl gridControl = gv.GridControl;
		if (!gridControl.DataSourceTable().IsNotNull())
		{
			return;
		}
		gv.IsMappingContextAction = true;
		for (int i = 0; i < mapping.Length; i++)
		{
			if (string.IsNullOrEmpty(mapping[i].TargetField))
			{
				continue;
			}
			GridColumn gridColumn = gv.Columns[mapping[i].TargetField];
			if (gridColumn == null)
			{
				continue;
			}
			object obj = null;
			if (!string.IsNullOrEmpty(mapping[i].SourceField))
			{
				obj = ((!(gridColumn.ColumnType == typeof(DateTime))) ? source.Rows[sourceIndex][mapping[i].SourceField] : ((object)Function.ParseDateTime(source.Rows[sourceIndex][mapping[i].SourceField])));
			}
			if (string.IsNullOrEmpty(Function.ToString(obj)))
			{
				if (gridColumn.ColumnType == typeof(DateTime))
				{
					if (Function.ToString(mapping[i].DfltValue).ToUpper() == "DATETIME.TODAY")
					{
						obj = DateTime.Today;
					}
					else
					{
						DateTime? dateTime = Function.ParseDateTime(mapping[i].DfltValue);
						obj = ((!dateTime.HasValue) ? DBNull.Value : ((object)dateTime));
					}
				}
				else
				{
					obj = mapping[i].DfltValue;
				}
			}
			int focusedRowHandle = gv.FocusedRowHandle;
			gv.RemoveMappingField(mapping[i].TargetField);
			gv.SetRowCellValue(gv.FocusedRowHandle, gridColumn, obj);
			gv.AddMappingField(mapping[i].TargetField);
			gv.FocusedRowHandle = focusedRowHandle;
			gridControl.RefreshDataSource();
		}
		gv.IsMappingContextAction = false;
	}

	public static object GetValueSource(DataRow sourcerow, ContextActionMappingImpl mapping, Type type = null)
	{
		object obj = null;
		if (mapping == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(mapping.SourceField))
		{
			obj = ((!(type == typeof(DateTime))) ? sourcerow[mapping.SourceField] : ((object)Function.ParseDateTime(sourcerow[mapping.SourceField])));
		}
		if (string.IsNullOrEmpty(Function.ToString(obj)))
		{
			obj = mapping.DfltValue;
		}
		return obj;
	}
}
