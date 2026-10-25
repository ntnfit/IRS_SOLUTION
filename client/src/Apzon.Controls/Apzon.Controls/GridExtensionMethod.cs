using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;

namespace Apzon.Controls;

public static class GridExtensionMethod
{
	public static void ExcelToDataSource(this GridViewEx gv, List<string> key = null)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (!(gv.GridControl is GridControlEx gridControlEx))
			{
				WinCommons.Warning(Enums.MessageCode.CouldNotParseDataOnThisControl, "");
				return;
			}
			XtraFormEx xtraFormEx = gridControlEx.FindForm() as XtraFormEx;
			try
			{
				DataTable dataTable = null;
				if (dataObject == null)
				{
					dataTable = gridControlEx.ClipboardData;
				}
				else if (dataObject.GetData(DataFormats.UnicodeText) != null)
				{
					string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
					dataTable = (dataObject.GetDataPresent(DataFormats.Text) ? Function.PasteFromExcel(clipboardData) : gridControlEx.ClipboardData);
				}
				else
				{
					dataTable = gridControlEx.ClipboardData;
				}
				if (dataTable != null && (dataTable.Rows.Count > 1 || dataTable.Columns.Count > 1))
				{
					GridColumn focusedColumn = gv.FocusedColumn;
					gv.BeginInsertRow();
					gridControlEx.ClipboardData = dataTable;
					DataTable dataTable2 = gridControlEx.DataSourceTable();
					int num = gv.FocusedRowHandle;
					List<int> list = new List<int>();
					int num2 = ((gv.RowCount - num < dataTable.Rows.Count) ? gv.RowCount : (dataTable.Rows.Count + num));
					for (int i = num; i < num2; i++)
					{
						list.Add(gv.GetDataSourceRowIndex(i));
					}
					if (!(gv.FocusedColumn is GridColumnEx gridColumnEx))
					{
						return;
					}
					if (gridColumnEx.PrimaryKey || (key != null && key.Contains(gridColumnEx.FieldName)))
					{
						int rowCount = gv.RowCount;
						int count = dataTable.Rows.Count;
						if (rowCount - num < count)
						{
							for (int j = 0; j < count - rowCount + num; j++)
							{
								DataRow row = dataTable2.NewRow();
								dataTable2.Rows.Add(row);
								dataTable2.AcceptChanges();
								list.Add(dataTable2.Rows.Count - 1);
							}
							dataTable2.AcceptChanges();
							gridControlEx.DataSource = dataTable2;
							gridControlEx.RefreshDataSource();
						}
						int num3 = ((gv.Columns.Count > dataTable.Columns.Count) ? dataTable.Columns.Count : gv.Columns.Count);
						xtraFormEx?.ShowProgress(count);
						for (int k = 0; k < count; k++)
						{
							gv.FocusedColumn = focusedColumn;
							int colIndex = gridColumnEx.VisibleIndex;
							int num4 = gv.FindRowHandleByDataRow(dataTable2.Rows[list[k]]);
							if (num4 < 0)
							{
								continue;
							}
							gv.FocusedRowHandle = num4;
							num4 = gv.FindRowHandleByDataRow(dataTable2.Rows[list[k]]);
							gv.FocusedRowHandle = num4;
							for (int l = 0; l < num3; l++)
							{
								GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
								if (gridColumn == null)
								{
									break;
								}
								num4 = gv.FindRowHandleByDataRow(dataTable2.Rows[list[k]]);
								gv.FocusedRowHandle = num4;
								if (gridColumn.ColumnType == typeof(DateTime) && gridColumn.ColumnEdit.GetType() != typeof(RepositoryItemTimeEdit))
								{
									object obj = Function.ParseDateTime(dataTable.Rows[k][l], GlobalData.DateTimeFormat);
									if (obj == null)
									{
										obj = dataTable.Rows[k][l];
									}
									gv.SetFocusedRowCellValue(gridColumn, obj);
								}
								else
								{
									gv.SetFocusedRowCellValue(gridColumn, dataTable.Rows[k][l]);
								}
								colIndex++;
							}
							xtraFormEx?.SetValueProgress(k);
						}
						gv.EndInsertRow();
						dataTable2.AcceptChanges();
						gridControlEx.DataSource = dataTable2;
						gridControlEx.RefreshDataSource();
					}
					else
					{
						if (key == null)
						{
							key = (from GridColumnEx t in gv.Columns
								where t.PrimaryKey
								select t.FieldName).ToList();
						}
						xtraFormEx?.ShowProgress(list.Count);
						for (int num5 = 0; num5 < list.Count; num5++)
						{
							gv.FocusedColumn = focusedColumn;
							if (list[num5] < 0)
							{
								continue;
							}
							int colIndex2 = gridColumnEx.VisibleIndex;
							bool flag = false;
							int num6 = gv.FindRowHandleByDataRow(dataTable2.Rows[list[num5]]);
							if (key.Count == 0)
							{
								flag = true;
							}
							for (int num7 = 0; num7 < key.Count; num7++)
							{
								if (!string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num6, key[num7]))))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								continue;
							}
							gv.FocusedRowHandle = num6;
							for (int num8 = 0; num8 < dataTable.Columns.Count; num8++)
							{
								GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex2);
								if (gridColumn2 == null || gridColumn2.OptionsColumn.ReadOnly || !gridColumn2.OptionsColumn.AllowEdit)
								{
									continue;
								}
								num6 = (gv.FocusedRowHandle = gv.FindRowHandleByDataRow(dataTable2.Rows[list[num5]]));
								if (gridColumn2.ColumnType == typeof(DateTime) && gridColumn2.ColumnEdit.GetType() != typeof(RepositoryItemTimeEdit))
								{
									object obj2 = Function.ParseDateTime(dataTable.Rows[num5][num8], GlobalData.DateTimeFormat);
									if (obj2 == null)
									{
										obj2 = dataTable.Rows[num5][num8];
									}
									gv.SetRowCellValue(num6, gridColumn2, obj2);
								}
								else
								{
									gv.SetRowCellValue(num6, gridColumn2, dataTable.Rows[num5][num8]);
								}
								colIndex2++;
							}
							xtraFormEx?.SetValueProgress(num);
							num++;
						}
						gv.EndInsertRow();
						gridControlEx.DataSourceTable().AcceptChanges();
						gridControlEx.RefreshDataSource();
					}
					Clipboard.Clear();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
			}
			finally
			{
				xtraFormEx?.HideProgress(200);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
		}
		gv.EndInsertRow();
	}

	public static void ExcelToDataSource(this PivotGridControlEx grd)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			XtraFormEx xtraFormEx = grd.FindForm() as XtraFormEx;
			try
			{
				DataTable dataTable = null;
				if (dataObject == null)
				{
					dataTable = grd.ClipboardData;
				}
				else if (dataObject.GetData(DataFormats.UnicodeText) != null)
				{
					string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
					dataTable = (dataObject.GetDataPresent(DataFormats.Text) ? Function.PasteFromExcel(clipboardData) : grd.ClipboardData);
				}
				else
				{
					dataTable = grd.ClipboardData;
				}
				if (dataTable == null || (dataTable.Rows.Count <= 1 && dataTable.Columns.Count <= 1))
				{
					return;
				}
				_ = grd.Cells.FocusedCell;
				grd.ClipboardData = dataTable;
				DataTable dataTable2 = grd.DataSource as DataTable;
				if (!dataTable2.IsNotNull())
				{
					return;
				}
				int y = grd.Cells.FocusedCell.Y;
				int x = grd.Cells.FocusedCell.X;
				int num = ((grd.Cells.RowCount > dataTable.Rows.Count) ? dataTable.Rows.Count : grd.Cells.RowCount);
				int num2 = ((grd.Cells.ColumnCount > dataTable.Columns.Count) ? dataTable.Columns.Count : grd.Cells.ColumnCount);
				xtraFormEx?.ShowProgress(num);
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						PivotCellEventArgs cellInfo = grd.Cells.GetCellInfo(x + j, y + i);
						if (cellInfo != null && !cellInfo.DataField.Options.ReadOnly && cellInfo.DataField.Options.AllowEdit && cellInfo.DataField.FieldEdit != null && !cellInfo.DataField.FieldEdit.ReadOnly && cellInfo.DataField.FieldEdit.Editable)
						{
							PivotDrillDownDataSource pivotDrillDownDataSource = cellInfo.CreateDrillDownDataSource();
							for (int k = 0; k < pivotDrillDownDataSource.RowCount; k++)
							{
								pivotDrillDownDataSource.SetValue(k, cellInfo.DataField, dataTable.Rows[i][j]);
							}
						}
					}
					xtraFormEx?.SetValueProgress(i);
				}
				dataTable2.AcceptChanges();
				grd.DataSource = dataTable2;
				Clipboard.Clear();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
			}
			finally
			{
				xtraFormEx?.HideProgress(200);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
		}
	}

	public static DataTable DataSourceTable(this GridControl grd)
	{
		try
		{
			if (grd != null && grd.DataSource != null)
			{
				return grd.DataSource as DataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public static DataTable DataSourceTable(this GridControlEx grd)
	{
		try
		{
			if (grd != null && grd.DataSource != null)
			{
				return grd.DataSource as DataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public static bool IsExistColumn(this GridView view, string fieldName)
	{
		try
		{
			return view.Columns.FirstOrDefault((GridColumn t) => t.FieldName == fieldName) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsExistColumn(this GridViewEx view, string fieldName)
	{
		try
		{
			return view.Columns.FirstOrDefault((GridColumn t) => t.FieldName == fieldName) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void SetDefaultAndMaxLen(this DataTable dt, string tblName)
	{
		try
		{
			DataTable dataTable;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				dataTable = httpClientHelper.Get("GetDataStructure", new
				{
					table = tblName
				});
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int num = 0;
				string text = Function.ToString(dataTable.Rows[i]["DATA_TYPE"]);
				string text2 = Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]);
				Function.ToString(dataTable.Rows[i]["IS_NULLABLE"]).Equals("YES");
				string text3 = "";
				int length = Function.ToString(dataTable.Rows[i]["COLUMN_DEFAULT"]).Length;
				if (length >= 4)
				{
					text3 = Function.ToString(dataTable.Rows[i]["COLUMN_DEFAULT"]).Substring(2, length - 4);
				}
				switch (text)
				{
				default:
					num = Function.ParseInt(dataTable.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]);
					break;
				case "numeric":
				case "int":
				case "smallint":
					break;
				}
				for (int j = 0; j < dt.Columns.Count; j++)
				{
					if (!(dt.Columns[j].ColumnName == text2))
					{
						continue;
					}
					if (num > 0 && text == "")
					{
						switch (text)
						{
						default:
							dt.Columns[j].MaxLength = Function.ParseInt(num);
							break;
						case "int":
						case "bigint":
						case "smallint":
						case "decimal":
						case "datetime":
						case "date":
						case "numeric":
						case "float":
						case "bit":
							break;
						}
						break;
					}
					if (string.IsNullOrEmpty(text3))
					{
						continue;
					}
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					switch (text)
					{
					case "int":
					case "bigint":
					case "smallint":
						dt.Columns[j].DefaultValue = Function.ParseInt(text3.Replace("(", "").Replace(")", ""));
						break;
					case "decimal":
						dt.Columns[j].DefaultValue = Function.ParseDecimal(text3.Replace("(", "").Replace(")", ""));
						break;
					case "datetime":
					case "date":
					{
						DateTime? dateTime = Function.ParseDateTime(text3);
						if (dateTime.HasValue)
						{
							dt.Columns[j].DefaultValue = dateTime;
						}
						break;
					}
					case "numeric":
					case "float":
						dt.Columns[j].DefaultValue = Function.ParseDouble(text3.Replace("(", "").Replace(")", ""), invariantCulture.NumberFormat);
						break;
					case "bit":
						dt.Columns[j].DefaultValue = Function.ParseBool(text3);
						break;
					default:
						dt.Columns[j].DefaultValue = Function.ToString(text3.Replace("''", "'"));
						break;
					}
					break;
				}
			}
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static List<int> GetListIndexButtonPress(this MethodInfo[] methodsInfo)
	{
		List<int> list = new List<int>();
		try
		{
			if (methodsInfo != null && methodsInfo.Length != 0)
			{
				for (int i = 0; i < methodsInfo.Length; i++)
				{
					if (methodsInfo[i].Name.ToLower().Equals("raisebuttonpressed") || methodsInfo[i].Name.ToLower().Equals("raisebuttonclick"))
					{
						list.Add(i);
						if (list.Count == 2)
						{
							break;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public static List<int> GetListIndexKeyDown(this MethodInfo[] methodsInfo)
	{
		List<int> list = new List<int>();
		try
		{
			if (methodsInfo != null && methodsInfo.Length != 0)
			{
				for (int i = 0; i < methodsInfo.Length; i++)
				{
					if (methodsInfo[i].Name.ToLower().Equals("onkeydown"))
					{
						list.Add(i);
						return list;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}
}
