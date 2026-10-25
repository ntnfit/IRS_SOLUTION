using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Properties;
using Apzon.Forms.Reports;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using SAPbobsCOM;

namespace Apzon.Forms.Helper;

public static class GridViewHelper
{
	public static bool dfShowGroupPanel = false;

	public static int dfWidth = 75;

	public static bool dfVisible = false;

	public static bool dfReadOnly = false;

	public static bool dfAllowEdit = true;

	public static bool dfAllowMove = true;

	public static bool dfAllowSize = true;

	public static bool dfAutoHeightHeader = true;

	public static bool dfAppearanceEvenRow = true;

	public static DefaultBoolean dfAllowSort = DefaultBoolean.True;

	public static NewItemRowPosition dfNewItemRowPosition = NewItemRowPosition.None;

	public static async Task GenerateGrid(GridControlEx grid, GridViewEx view, DataSet ds, List<UserFieldsImpl> lstUDF = null, bool expand = false, bool filterRow = false, string tableName = "", Dictionary<string, GridViewEx> tblFormular = null)
	{
		_ = 4;
		try
		{
			if (ds.IsHasStructure())
			{
				if (view.Columns.Count > 0 && view.Columns.Count == ds.Tables[1].Columns.Count)
				{
					try
					{
						if (grid.DataSource is DataTable dataTable)
						{
							for (int i = 0; i < dataTable.Columns.Count; i++)
							{
								ds.Tables[1].Columns[dataTable.Columns[i].ColumnName].DefaultValue = dataTable.Columns[i].DefaultValue;
							}
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
					grid.DataSource = ds.Tables[1];
					grid.RefreshDataSource();
					if (expand)
					{
						view.ExpandAllGroups();
					}
					if (ds.Tables[0].IsExistsColumn("Expression"))
					{
						DataRow[] array = ds.Tables[0].Select("[Expression] <> ''");
						if (array.Length != 0)
						{
							try
							{
								DataRow[] array2 = array;
								foreach (DataRow dataRow in array2)
								{
									string name = Function.ToString(dataRow["ColumnName"]);
									ds.Tables[1].Columns[name].Expression = Function.ToString(dataRow["Expression"]);
								}
							}
							catch (Exception ex2)
							{
								WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex2.Message);
							}
						}
					}
				}
				else if (!ds.IsHasValidValue())
				{
					await GenerateGrid(grid, view, ds.Tables[0], ds.Tables[1], null, lstUDF, expand, tableName, tblFormular);
				}
				else
				{
					await GenerateGrid(grid, view, ds.Tables[0], ds.Tables[1], ds.Tables[2], lstUDF, expand, tableName, tblFormular);
				}
			}
			else if (ds.IsNotNull())
			{
				if (view.Columns.Count > 0 && view.Columns.Count == ds.Tables[0].Columns.Count)
				{
					grid.DataSource = ds.Tables[0];
					grid.RefreshDataSource();
				}
				else
				{
					view.BeginUpdate();
					view.OptionsView.ShowGroupPanel = dfShowGroupPanel;
					view.OptionsView.ColumnAutoWidth = ds.Tables[0].Columns.Count < 7;
					view.OptionsView.EnableAppearanceEvenRow = true;
					view.OptionsFilter.AllowFilterEditor = true;
					view.OptionsFind.AllowFindPanel = true;
					view.OptionsView.BestFitMaxRowCount = 100;
					view.OptionsMenu.ShowConditionalFormattingItem = true;
					view.Columns.Clear();
					DataTable dtFormula = null;
					if (!string.IsNullOrEmpty(tableName))
					{
						using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
						dtFormula = await client.GetAsync("GetFormulaSearch", new
						{
							fieldName = "",
							tblName = tableName
						});
					}
					foreach (DataColumn column in ds.Tables[0].Columns)
					{
						GridColumnEx col = view.ColumnsEx.AddEx();
						col.Name = $"cl_{column.ColumnName}";
						col.FieldName = column.ColumnName;
						col.Caption = column.Caption;
						col.Visible = true;
						col.OptionsFilter.AllowFilter = true;
						col.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
						col.Width = dfWidth;
						await LoadUdfColumns(view, col, lstUDF, null, null, null, null, dtFormula?.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["FieldName"]) == col.FieldName), tblFormular);
					}
					if (!view.OptionsView.ShowColumnHeaders)
					{
						view.OptionsView.ShowColumnHeaders = true;
					}
					grid.DataSource = ds.Tables[0];
					grid.RefreshDataSource();
					await view.ReloadFormSetting();
					view.EndUpdate();
					view.BestFitColumns();
				}
				if (expand)
				{
					view.ExpandAllGroups();
				}
			}
			if (filterRow)
			{
				view.OptionsView.ShowAutoFilterRow = true;
			}
			if (dfAutoHeightHeader)
			{
				view.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.ErrorLineNumber(ex3);
		}
	}

	public static async Task GenerateGrid(GridControlEx grid, GridViewEx view, DataTable dtStructure, DataTable dt, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, bool expand = false, string tableName = "", Dictionary<string, GridViewEx> tblFormular = null)
	{
		try
		{
			grid.DataSource = null;
			grid.ViewCollection.Clear();
			if (view.FormId != "oSalaryResult" && view.FormId != "oSalaryResultView")
			{
				view.Columns.Clear();
			}
			grid.ViewCollection.Add(view);
			grid.MainView = view;
			view.ShowLoadingPanel();
			view.BeginUpdate();
			view.OptionsView.BestFitMaxRowCount = 100;
			view.OptionsView.ShowGroupPanel = dfShowGroupPanel;
			view.OptionsView.NewItemRowPosition = dfNewItemRowPosition;
			view.OptionsView.RowAutoHeight = true;
			view.OptionsView.EnableAppearanceEvenRow = dfAppearanceEvenRow;
			view.GridControl = grid;
			view.OptionsFilter.AllowFilterEditor = true;
			view.OptionsFind.AllowFindPanel = true;
			view.OptionsFind.FindNullPrompt = "Nhập để tìm kiếm...";
			view.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.Default;
			view.OptionsBehavior.AllowPartialGroups = DefaultBoolean.False;
			view.OptionsMenu.ShowConditionalFormattingItem = true;
			if (dt == null || dt.Columns.Count <= 0 || !dtStructure.IsNotNull())
			{
				await GenerateColumn(grid, view, dtStructure, dtValidValue, lstUDF, tableName, tblFormular);
			}
			else
			{
				int count = dt.Rows.Count;
				if (count > 9999)
				{
					view.IndicatorWidth = 55;
				}
				else if (count > 999)
				{
					view.IndicatorWidth = 45;
				}
				else
				{
					view.IndicatorWidth = 35;
				}
				int idx = 0;
				string filterString = "";
				List<GridColumnEx> lstGroupColumn = new List<GridColumnEx>();
				DataTable dtFormula = null;
				if (!string.IsNullOrEmpty(tableName))
				{
					using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
					dtFormula = await client.GetAsync("GetFormulaSearch", new
					{
						fieldName = "",
						tblName = tableName
					});
				}
				int i;
				for (i = 0; i < dt.Columns.Count; i++)
				{
					if ((view.FormId == "oSalaryResult" || view.FormId == "oSalaryResultView") && !dt.Columns[i].ColumnName.StartsWith("U_"))
					{
						continue;
					}
					GridColumnEx col = view.ColumnsEx.AddEx();
					col.Name = $"cl_{dt.Columns[i].ColumnName}";
					col.FieldName = dt.Columns[i].ColumnName;
					col.OptionsFilter.AllowFilter = true;
					col.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
					DataRow dataRow = (dtStructure.Rows[0].IsExistsColumn("ColumnName") ? (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").ToUpper().Equals(dt.Columns[i].ColumnName.ToUpper())
						select c).FirstOrDefault() : null);
					if (dataRow != null)
					{
						col.Caption = (dataRow.IsExistsColumn("Caption") ? dataRow["Caption"].ToString() : dt.Columns[i].ColumnName);
						col.ToolTip = (dataRow.IsExistsColumn("Caption") ? Function.ToString(dataRow["Caption"]) : dt.Columns[i].ColumnName);
						col.Width = (dataRow.IsExistsColumn("Width") ? Function.ParseInt(dataRow["Width"]) : dfWidth);
						if (dataRow.IsExistsColumn("Visible"))
						{
							col.Visible = Function.ParseBool(dataRow["Visible"]);
						}
						else
						{
							col.Visible = dfVisible;
						}
						col.VisibleIndex = (col.Visible ? idx : (-1));
						idx = (col.Visible ? (idx + 1) : idx);
						col.OptionsColumn.ReadOnly = (dataRow.IsExistsColumn("ReadOnly") ? Function.ParseBool(dataRow["ReadOnly"]) : dfReadOnly);
						col.OptionsColumn.AllowEdit = (dataRow.IsExistsColumn("AllowEdit") ? Function.ParseBool(dataRow["AllowEdit"]) : dfAllowEdit);
						col.OptionsColumn.AllowMove = (dataRow.IsExistsColumn("AllowMove") ? Function.ParseBool(dataRow["AllowMove"]) : dfAllowMove);
						col.OptionsColumn.AllowSize = (dataRow.IsExistsColumn("AllowSize") ? Function.ParseBool(dataRow["AllowSize"]) : dfAllowSize);
						col.OptionsColumn.AllowSort = ((!dataRow.IsExistsColumn("AllowSort") || !Function.ParseBool(dataRow["AllowSort"])) ? dfAllowSort : DefaultBoolean.True);
						if (dataRow.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(dataRow["FormatType"])))
						{
							string text = Function.ToString(dataRow["FormatType"]).ToLower();
							if (!(text == "numeric"))
							{
								if (text == "datetime")
								{
									col.DisplayFormat.FormatType = FormatType.DateTime;
								}
								else
								{
									col.DisplayFormat.FormatType = FormatType.None;
								}
							}
							else
							{
								col.DisplayFormat.FormatType = FormatType.Numeric;
							}
						}
						if (dataRow.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["DisplayFormat"])))
						{
							if (Function.ParseInt(dataRow["DisplayFormat"]) > 0)
							{
								col.DisplayFormatType = (Enums.DisplayFormatType)Function.ParseInt(dataRow["DisplayFormat"]);
							}
							else
							{
								col.DisplayFormat.FormatString = Function.ToString(dataRow["DisplayFormat"]);
							}
						}
						if (dataRow.IsExistsColumn("GroupIndex") && Function.ParseInt(dataRow["GroupIndex"]) >= 0)
						{
							int num = Function.ParseInt(dataRow["GroupIndex"]);
							if (num < lstGroupColumn.Count)
							{
								lstGroupColumn[num] = col;
							}
							else
							{
								while (lstGroupColumn.Count < num)
								{
									lstGroupColumn.Add(null);
								}
								lstGroupColumn.Add(col);
							}
						}
						if (dataRow.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(dataRow["Fixed"])))
						{
							string text = Function.ToString(dataRow["Fixed"]).ToUpper();
							if (!(text == "L"))
							{
								if (text == "R")
								{
									col.Fixed = FixedStyle.Right;
								}
							}
							else
							{
								col.Fixed = FixedStyle.Left;
							}
						}
						string format = col.DisplayFormat.FormatString;
						if (dataRow.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryFormat"])))
						{
							format = Function.ToString(dataRow["SummaryFormat"]);
						}
						if (dataRow.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryType"])))
						{
							view.OptionsView.ShowFooter = true;
							string summaryMode = "A";
							if (dataRow.IsExistsColumn("SummaryMode") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryMode"])))
							{
								summaryMode = Function.ToString(dataRow["SummaryMode"]);
							}
							GetSummary(view, col, Function.ToString(dataRow["SummaryType"]).ToLower(), format, summaryMode, Function.ToString(dataRow["SubType"]));
						}
						if (dataRow.IsExistsColumn("GroupSummaryType") && !string.IsNullOrEmpty(Function.ToString(dataRow["GroupSummaryType"])))
						{
							bool showInFooter = !dataRow.IsExistsColumn("GroupSummaryShowInFooter") || Function.ParseBool(dataRow["GroupSummaryShowInFooter"]);
							GetGroupSummary(view, col, Function.ToString(dataRow["GroupSummaryType"]).ToLower(), format, showInFooter, Function.ToString(dataRow["SubType"]));
						}
						if (dataRow.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(dataRow["Expression"])))
						{
							try
							{
								dt.Columns[i].Expression = Function.ToString(dataRow["Expression"]);
							}
							catch (Exception ex)
							{
								WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
							}
						}
						if (dataRow.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(dataRow["BackColor"])))
						{
							col.AppearanceCell.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
							col.AppearanceHeader.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
						}
						if (dataRow.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(dataRow["ForeColor"])))
						{
							col.AppearanceCell.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
							col.AppearanceHeader.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
						}
						if (dataRow.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(dataRow["FilterString"])))
						{
							filterString = filterString + Function.ToString(dataRow["FilterString"]) + " ";
						}
					}
					else
					{
						col.Caption = dt.Columns[i].ColumnName;
						col.Width = dfWidth;
						col.Visible = dfVisible;
						col.OptionsColumn.ReadOnly = dfReadOnly;
						col.OptionsColumn.AllowEdit = dfAllowEdit;
						col.OptionsColumn.AllowMove = dfAllowMove;
						col.OptionsColumn.AllowSize = dfAllowSize;
						col.OptionsColumn.AllowSort = dfAllowSort;
						if (dfVisible)
						{
							col.VisibleIndex = idx;
							idx++;
						}
						else
						{
							col.VisibleIndex = -1;
						}
					}
					await LoadUdfColumns(view, col, lstUDF, dataRow, dtValidValue, dt.Columns[i], dt, dtFormula?.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["FieldName"]) == col.FieldName), tblFormular);
				}
				if (lstGroupColumn.Any())
				{
					view.BeginLockFocusRowChange();
					for (int num2 = 0; num2 < lstGroupColumn.Count; num2++)
					{
						if (lstGroupColumn[num2] != null)
						{
							lstGroupColumn[num2].GroupIndex = num2;
						}
					}
					view.EndLockFocusRowChange();
				}
				if (!view.OptionsView.ShowColumnHeaders)
				{
					view.OptionsView.ShowColumnHeaders = true;
				}
				grid.DataSource = dt;
				grid.RefreshDataSource();
				if (dtStructure.IsExistsColumn("VisIndex"))
				{
					List<DataRow> list = new List<DataRow>();
					foreach (DataRow row in dtStructure.Rows)
					{
						string fieldName = Function.ToString(row["ColumnName"]);
						if (!view.IsExistColumn(fieldName))
						{
							list.Add(row);
						}
						else if (view.Columns[fieldName].Visible)
						{
							int visibleIndex = Function.ParseInt(row["VisIndex"]);
							view.Columns[fieldName].VisibleIndex = visibleIndex;
						}
					}
					if (list.Count > 0)
					{
						foreach (DataRow item in list)
						{
							dtStructure.Rows.Remove(item);
						}
					}
				}
				view.ActiveFilterString = filterString;
			}
			if (expand)
			{
				view.ExpandAllGroups();
			}
			view.OptionsView.ColumnHeaderAutoHeight = ((!dfAutoHeightHeader) ? DefaultBoolean.Default : DefaultBoolean.True);
			view.OptionsView.ColumnAutoWidth = view.ColumnsEx.Where((GridColumn c) => c.Visible).ToList().Count < 7;
			view.Appearance.FocusedRow.BackColor = ApzonCommon.colorFocusRow;
			if (view.FormatRules.Count == 0)
			{
				await ExtensionMethods.ReloadFormatRuleGrid(view, view.FormId);
			}
			await view.ReloadFormSetting();
			view.EndUpdate();
			view.HideLoadingPanel();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		finally
		{
			view.EndLockFocusRowChange();
		}
	}

	private static async Task GenerateColumn(GridControlEx grid, GridViewEx view, DataTable dtStructure, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, string tableName = null, Dictionary<string, GridViewEx> tblFormular = null)
	{
		view.IndicatorWidth = 35;
		int idx = 0;
		string filterString = "";
		DataTable dt = new DataTable();
		foreach (DataRow row in dtStructure.Rows)
		{
			dt.Columns.Add(Function.ToString(row["ColumnName"]), row.IsExistsColumn("DataType") ? GetColumnType(Function.ToString(row["DataType"])) : typeof(string));
		}
		DataTable dtFormula = null;
		if (!string.IsNullOrEmpty(tableName))
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			dtFormula = await client.GetAsync("GetFormulaSearch", new
			{
				fieldName = "",
				tblName = tableName
			});
		}
		List<DataRow> list = (from t in dtStructure.AsEnumerable()
			orderby Function.ParseInt(t["GroupIndex"]) descending
			select t).ToList();
		view.BeginLockFocusRowChange();
		foreach (DataRow item in list)
		{
			GridColumnEx col = (GridColumnEx)view.Columns.Add();
			col.FieldName = (item.IsExistsColumn("ColumnName") ? Function.ToString(item["ColumnName"]) : "");
			col.Name = $"cl_{col.FieldName}";
			col.OptionsFilter.AllowFilter = true;
			col.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
			col.Caption = (item.IsExistsColumn("Caption") ? Function.ToString(item["Caption"]) : col.FieldName);
			col.ToolTip = (item.IsExistsColumn("Caption") ? Function.ToString(item["Caption"]) : col.FieldName);
			col.Width = (item.IsExistsColumn("Width") ? Function.ParseInt(item["Width"]) : dfWidth);
			if (item.IsExistsColumn("Visible"))
			{
				col.Visible = Function.ParseBool(item["Visible"]);
			}
			else
			{
				col.Visible = dfVisible;
			}
			col.VisibleIndex = (col.Visible ? idx : (-1));
			idx = (col.Visible ? (idx + 1) : idx);
			col.OptionsColumn.ReadOnly = (item.IsExistsColumn("ReadOnly") ? Function.ParseBool(item["ReadOnly"]) : dfReadOnly);
			col.OptionsColumn.AllowEdit = (item.IsExistsColumn("AllowEdit") ? Function.ParseBool(item["AllowEdit"]) : dfAllowEdit);
			col.OptionsColumn.AllowMove = (item.IsExistsColumn("AllowMove") ? Function.ParseBool(item["AllowMove"]) : dfAllowMove);
			col.OptionsColumn.AllowSize = (item.IsExistsColumn("AllowSize") ? Function.ParseBool(item["AllowSize"]) : dfAllowSize);
			col.OptionsColumn.AllowSort = ((!item.IsExistsColumn("AllowSort") || !Function.ParseBool(item["AllowSort"])) ? dfAllowSort : DefaultBoolean.True);
			if (item.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(item["FormatType"])))
			{
				string text = Function.ToString(item["FormatType"]).ToLower();
				if (!(text == "numeric"))
				{
					if (text == "datetime")
					{
						col.DisplayFormat.FormatType = FormatType.DateTime;
					}
					else
					{
						col.DisplayFormat.FormatType = FormatType.None;
					}
				}
				else
				{
					col.DisplayFormat.FormatType = FormatType.Numeric;
				}
			}
			if (item.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(item["DisplayFormat"])))
			{
				if (Function.ParseInt(item["DisplayFormat"]) > 0)
				{
					col.DisplayFormatType = (Enums.DisplayFormatType)Function.ParseInt(item["DisplayFormat"]);
				}
				else
				{
					col.DisplayFormat.FormatString = Function.ToString(item["DisplayFormat"]);
				}
			}
			if (item.IsExistsColumn("GroupIndex") && Function.ParseInt(item["GroupIndex"]) >= 0)
			{
				col.GroupIndex = Function.ParseInt(item["GroupIndex"]);
			}
			if (item.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(item["Fixed"])))
			{
				string text = Function.ToString(item["Fixed"]);
				if (!(text == "L"))
				{
					if (text == "R")
					{
						col.Fixed = FixedStyle.Right;
					}
				}
				else
				{
					col.Fixed = FixedStyle.Left;
				}
			}
			string format = col.DisplayFormat.FormatString;
			if (item.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(item["SummaryFormat"])))
			{
				format = Function.ToString(item["SummaryFormat"]);
			}
			if (item.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(item["SummaryType"])))
			{
				view.OptionsView.ShowFooter = true;
				string summaryMode = "A";
				if (item.IsExistsColumn("SummaryMode") && !string.IsNullOrEmpty(Function.ToString(item["SummaryMode"])))
				{
					summaryMode = Function.ToString(item["SummaryMode"]);
				}
				GetSummary(view, col, Function.ToString(item["SummaryType"]).ToLower(), format, summaryMode, Function.ToString(item["SubType"]));
			}
			if (item.IsExistsColumn("GroupSummaryType") && !string.IsNullOrEmpty(Function.ToString(item["GroupSummaryType"])))
			{
				bool showInFooter = !item.IsExistsColumn("GroupSummaryShowInFooter") || Function.ParseBool(item["GroupSummaryShowInFooter"]);
				GetGroupSummary(view, col, Function.ToString(item["GroupSummaryType"]).ToLower(), format, showInFooter, Function.ToString(item["SubType"]));
			}
			if (item.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(item["Expression"])))
			{
				try
				{
					dt.Columns[col.FieldName].Expression = Function.ToString(item["Expression"]);
				}
				catch (Exception ex)
				{
					WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
				}
			}
			if (item.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(item["BackColor"])))
			{
				col.AppearanceCell.BackColor = Color.FromName(Function.ToString(item["BackColor"]));
				col.AppearanceHeader.BackColor = Color.FromName(Function.ToString(item["BackColor"]));
			}
			if (item.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(item["ForeColor"])))
			{
				col.AppearanceCell.ForeColor = Color.FromName(Function.ToString(item["ForeColor"]));
				col.AppearanceHeader.ForeColor = Color.FromName(Function.ToString(item["ForeColor"]));
			}
			if (item.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(item["FilterString"])))
			{
				filterString = filterString + Function.ToString(item["FilterString"]) + " ";
			}
			await LoadUdfColumns(view, col, lstUDF, item, dtValidValue, dt.Columns[col.FieldName], dt, dtFormula?.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["FieldName"]) == col.FieldName), tblFormular);
		}
		view.EndLockFocusRowChange();
		view.ActiveFilterString = filterString;
		grid.DataSource = dt;
	}

	private static void view_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public static void GetSummary(GridViewEx view, GridColumnEx col, string summaryType, string format, string summaryMode, string subType = "")
	{
		SummaryItemType summaryType2;
		switch (summaryType)
		{
		case "sum":
			summaryType2 = SummaryItemType.Sum;
			break;
		case "average":
			summaryType2 = SummaryItemType.Average;
			break;
		case "min":
			summaryType2 = SummaryItemType.Min;
			break;
		case "max":
			summaryType2 = SummaryItemType.Max;
			break;
		case "count":
			summaryType2 = SummaryItemType.Count;
			break;
		case "custom":
		{
			if (string.IsNullOrEmpty(subType))
			{
				summaryType2 = SummaryItemType.None;
				break;
			}
			summaryType2 = SummaryItemType.Custom;
			object val = null;
			List<string> list = (from m in subType.Split(',')
				select m.Trim().Replace("{", "").Replace("}", "")).ToList();
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			foreach (string item in list)
			{
				string[] array = item.Split('-');
				if (array.Length != 0)
				{
					if (array.Length == 1 && !dictionary.ContainsKey(-1))
					{
						dictionary[-1] = array[0];
					}
					else if (array.Length > 1)
					{
						dictionary[Function.ParseInt(array[0])] = array[1];
					}
				}
			}
			string field = "";
			if (dictionary.ContainsKey(-1))
			{
				field = dictionary[-1];
			}
			else
			{
				field = dictionary.FirstOrDefault().Value;
			}
			view.CustomSummaryCalculate += (object sender, CustomSummaryEventArgs e) =>
			{
				try
				{
					if (e.IsTotalSummary && ((GridColumnSummaryItem)e.Item).FieldName == col.FieldName)
					{
						switch (e.SummaryProcess)
						{
						case CustomSummaryProcess.Start:
							val = null;
							break;
						case CustomSummaryProcess.Calculate:
							if (val == null)
							{
								val = view.GetRowCellValue(e.RowHandle, view.Columns[field]);
							}
							break;
						case CustomSummaryProcess.Finalize:
							e.TotalValue = val;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			break;
		}
		default:
			summaryType2 = SummaryItemType.None;
			break;
		}
		SummaryMode mode = summaryMode switch
		{
			"A" => SummaryMode.AllRows, 
			"S" => SummaryMode.Selection, 
			"M" => SummaryMode.Mixed, 
			_ => SummaryMode.AllRows, 
		};
		col.Summary.Add(new GridColumnSummaryItem
		{
			SummaryType = summaryType2,
			DisplayFormat = format,
			Mode = mode
		});
	}

	public static void GetGroupSummary(GridViewEx view, GridColumnEx col, string groupSummaryType, string format, bool showInFooter, string subType = "")
	{
		view.OptionsMenu.EnableFooterMenu = true;
		view.OptionsMenu.ShowGroupSummaryEditorItem = true;
		col.AllowSummaryMenu = true;
		GridGroupSummaryItem gridGroupSummaryItem = new GridGroupSummaryItem();
		gridGroupSummaryItem.FieldName = col.FieldName;
		gridGroupSummaryItem.DisplayFormat = format;
		gridGroupSummaryItem.ShowInGroupColumnFooter = col;
		if (showInFooter)
		{
			view.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.True;
			view.OptionsBehavior.AllowPartialGroups = DefaultBoolean.True;
		}
		col.SummaryItem.FieldName = col.FieldName;
		switch (groupSummaryType)
		{
		case "sum":
			gridGroupSummaryItem.SummaryType = SummaryItemType.Sum;
			break;
		case "average":
			gridGroupSummaryItem.SummaryType = SummaryItemType.Average;
			break;
		case "min":
			gridGroupSummaryItem.SummaryType = SummaryItemType.Min;
			break;
		case "max":
			gridGroupSummaryItem.SummaryType = SummaryItemType.Max;
			break;
		case "count":
			gridGroupSummaryItem.SummaryType = SummaryItemType.Count;
			break;
		case "custom":
		{
			if (string.IsNullOrEmpty(subType))
			{
				gridGroupSummaryItem.SummaryType = SummaryItemType.None;
				break;
			}
			gridGroupSummaryItem.SummaryType = SummaryItemType.Custom;
			object val = null;
			List<string> list = (from m in subType.Split(',')
				select m.Trim().Replace("{", "").Replace("}", "")).ToList();
			Dictionary<int, string> lstField = new Dictionary<int, string>();
			foreach (string item in list)
			{
				string[] array = item.Split('-');
				if (array.Length != 0)
				{
					if (array.Length == 1 && !lstField.ContainsKey(-1))
					{
						lstField[-1] = array[0];
					}
					else if (array.Length > 1)
					{
						lstField[Function.ParseInt(array[0])] = array[1];
					}
				}
			}
			view.CustomSummaryCalculate += (object sender, CustomSummaryEventArgs e) =>
			{
				try
				{
					if (e.IsGroupSummary && ((GridGroupSummaryItem)e.Item).FieldName == col.FieldName)
					{
						switch (e.SummaryProcess)
						{
						case CustomSummaryProcess.Start:
							val = null;
							break;
						case CustomSummaryProcess.Calculate:
							if (val == null && lstField.ContainsKey(e.GroupLevel))
							{
								val = view.GetRowCellValue(e.RowHandle, view.Columns[lstField[e.GroupLevel]]);
							}
							break;
						case CustomSummaryProcess.Finalize:
							e.TotalValue = val;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			break;
		}
		default:
			gridGroupSummaryItem.SummaryType = SummaryItemType.None;
			break;
		}
		view.GroupSummary.Add(gridGroupSummaryItem);
	}

	public static Type GetColumnType(string sqlType)
	{
		return sqlType.ToLower() switch
		{
			"int" => typeof(int), 
			"smallint" => typeof(short), 
			"bigint" => typeof(int), 
			"numeric" => typeof(decimal), 
			"decimal" => typeof(decimal), 
			"money" => typeof(decimal), 
			"float" => typeof(double), 
			"datetime" => typeof(DateTime), 
			"date" => typeof(DateTime), 
			"bit" => typeof(bool), 
			_ => typeof(string), 
		};
	}

	public static void SetColumnDefaultData(DataColumn col, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (col.DataType == typeof(string))
			{
				col.DefaultValue = value;
			}
			else if (col.DataType == typeof(int) || col.DataType == typeof(int) || col.DataType == typeof(long))
			{
				col.DefaultValue = Function.ParseInt(value);
			}
			else if (col.DataType == typeof(double) || col.DataType == typeof(decimal))
			{
				col.DefaultValue = Function.ParseDouble(value);
			}
			else if (col.DataType == typeof(DateTime))
			{
				col.DefaultValue = Function.ParseDateTime(value);
			}
		}
	}

	public static async Task LoadUdfColumns(GridViewEx view, GridColumnEx col, List<UserFieldsImpl> lstUDF, DataRow dr, DataTable dtValidValue, DataColumn dataCol, DataTable data, DataRow formula = null, Dictionary<string, GridViewEx> tblFormular = null)
	{
		try
		{
			string udfName = ((!string.IsNullOrEmpty(col.FieldName) && col.FieldName.StartsWith("U_")) ? col.FieldName.Substring(2, col.FieldName.Length - 2).ToLower() : "");
			UserFieldsImpl udf = ((lstUDF != null && lstUDF.Count > 0) ? lstUDF.Where((UserFieldsImpl o) => o.Name.ToLower().Equals(udfName)).FirstOrDefault() : null);
			if (udf == null)
			{
				await LoadRepository(view, col, dr, dtValidValue, data);
				return;
			}
			SetColumnDefaultData(dataCol, udf.DefaultValue);
			IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();
			if (!string.IsNullOrEmpty(udf.LinkedUDO))
			{
				col.UnboundType = UnboundColumnType.Integer;
				RepositoryItemButtonEdit btn = new RepositoryItemButtonEdit();
				btn.DisplayFormat.FormatType = FormatType.Numeric;
				btn.DisplayFormat.FormatString = "n0";
				btn.EditFormat.FormatType = FormatType.Numeric;
				btn.EditFormat.FormatString = "n0";
				btn.Buttons[0].Kind = ButtonPredefines.Search;
				btn.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Resources.right,
					IsLeft = true,
					ToolTip = "Linked to UDO " + udf.Description,
					Tag = udf.LinkedUDO
				});
				btn.Tag = udf.LinkedUDO;
				string udoType = await _httpClient.Udo.GetTypeByCode(udf.LinkedUDO);
				btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						int docEntry = -1;
						string code = "";
						if (udoType.Equals("3"))
						{
							docEntry = Function.ParseInt(view.GetFocusedRowCellValue(col.FieldName));
						}
						else
						{
							code = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
						}
						frm_UDO frm_UDO2 = new frm_UDO(udf.LinkedUDO, docEntry, code);
						frm_UDO2.Text = udf.Description;
						frm_UDO2.dataRow = view.GetDataRow(view.FocusedRowHandle);
						frm_UDO2.ShowDialog();
						if (frm_UDO2.ChoosedEntry != null)
						{
							view.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				col.ColumnEdit = btn;
			}
			else if (!string.IsNullOrEmpty(udf.LinkedObject))
			{
				col.UnboundType = UnboundColumnType.String;
				col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				RepositoryItemButtonEdit btn2 = new RepositoryItemButtonEdit();
				btn2.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
				btn2.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
				btn2.EditFormat.FormatType = col.DisplayFormat.FormatType;
				btn2.EditFormat.FormatString = col.DisplayFormat.FormatString;
				btn2.Buttons.Clear();
				btn2.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Resources.right,
					IsLeft = true,
					ToolTip = "Linked to System Object " + Function.ToString((ApzObjectTypes)Function.ParseInt(udf.LinkedObject)),
					Tag = udf.LinkedObject
				});
				btn2.Tag = udf.LinkedObject;
				btn2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						int num = Function.ParseInt(btn2.Tag);
						string text = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
						if (!string.IsNullOrEmpty(text))
						{
							((XtraFormEx)view.GridControl.FindForm()).DrillDown((ApzObjectTypes)num, (ApzObjectTypes)num, text);
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				col.ColumnEdit = btn2;
			}
			switch (udf.Type)
			{
			case BoFieldTypes.db_Alpha:
				col.UnboundType = UnboundColumnType.String;
				if (!string.IsNullOrEmpty(udf.LinkedTable))
				{
					GridColumnEx gridColumnEx = col;
					gridColumnEx.ColumnEdit = await WinFuntions.CreateEditorLinkedTable(view, col, udf.LinkedTable);
					col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				break;
			case BoFieldTypes.db_Memo:
			{
				if (udf.SubType == BoFldSubTypes.st_Link)
				{
					col.UnboundType = UnboundColumnType.String;
					RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
					repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
					col.ColumnEdit = repositoryItemAttachFile;
					break;
				}
				col.UnboundType = UnboundColumnType.String;
				RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
				repositoryItemMemoExEdit.NullText = "";
				repositoryItemMemoExEdit.ShowIcon = false;
				repositoryItemMemoExEdit.BestFitWidth = col.Width;
				repositoryItemMemoExEdit.WordWrap = true;
				repositoryItemMemoExEdit.AcceptsReturn = true;
				repositoryItemMemoExEdit.ParseEditValue += Memo_ParseEditValue;
				repositoryItemMemoExEdit.FormatEditValue += Memo_FormatEditValue;
				repositoryItemMemoExEdit.Popup += Memo_Popup;
				col.ColumnEdit = repositoryItemMemoExEdit;
				break;
			}
			case BoFieldTypes.db_Date:
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.UnboundType = UnboundColumnType.String;
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
					col.ColumnEdit = repositoryItemTextEdit;
				}
				else
				{
					col.UnboundType = UnboundColumnType.DateTime;
					RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
					repositoryItemDateEditEx.Name = col.Name;
					repositoryItemDateEditEx.AllowMouseWheel = false;
					col.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					col.ColumnEdit = repositoryItemDateEditEx;
				}
				break;
			case BoFieldTypes.db_Float:
			{
				col.UnboundType = UnboundColumnType.Decimal;
				RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.AllowMouseWheel = false;
				col.ColumnEdit = repositoryItemCalcEdit2;
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Percent;
				}
				else if (udf.SubType == BoFldSubTypes.st_Price)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Price;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Rate;
				}
				else if (udf.SubType == BoFldSubTypes.st_Sum)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Amount;
				}
				else
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Qty;
				}
				break;
			}
			case BoFieldTypes.db_Numeric:
			{
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.UnboundType = UnboundColumnType.String;
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					RepositoryItemTimeEditEx columnEdit = new RepositoryItemTimeEditEx();
					col.ColumnEdit = columnEdit;
					break;
				}
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Percent;
				}
				else if (udf.SubType == BoFldSubTypes.st_Price)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Price;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Rate;
				}
				else if (udf.SubType == BoFldSubTypes.st_Sum)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Amount;
				}
				else
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Qty;
				}
				col.UnboundType = UnboundColumnType.Decimal;
				RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit.AllowMouseWheel = false;
				col.DisplayFormat.FormatType = FormatType.Numeric;
				col.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				col.ColumnEdit = repositoryItemCalcEdit;
				break;
			}
			}
			if (formula != null)
			{
				string type = Function.ToString(formula["FormulaType"]).ToUpper();
				switch (type)
				{
				case "CBX":
				{
					RepositoryItemLookUpEdit r = new RepositoryItemLookUpEdit();
					string sql = Function.ToString(formula["Formula"]);
					DataTable dataTable = (DataTable)(r.DataSource = (await _httpClient.Commons.ExecuteQuery(sql)).Data);
					if (dataTable != null && dataTable.Columns.Count >= 2)
					{
						r.ValueMember = dataTable.Columns[0].ColumnName;
						r.DisplayMember = dataTable.Columns[1].ColumnName;
					}
					r.NullText = string.Empty;
					r.BestFitMode = BestFitMode.BestFitResizePopup;
					col.ColumnEdit = r;
					break;
				}
				case "CFL":
				case "MSCFL":
				{
					RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
					repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
					repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs v) =>
					{
						try
						{
							string text = Function.ToString(formula["Formula"]);
							List<string> paramFromSqlString = Function.GetParamFromSqlString(text);
							if (paramFromSqlString != null && paramFromSqlString.Any())
							{
								foreach (string item in paramFromSqlString)
								{
									if (!string.IsNullOrEmpty(item))
									{
										string[] array = (item.StartsWith("@") ? item.Substring(1).Replace(" ", "") : item.Replace("@", "").Replace(" ", "")).Split('.');
										string key = Function.ToString(formula["DependTable"]);
										string fieldName = "";
										if (array.Length > 1)
										{
											key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : Function.ToString(formula["DependTable"]));
											fieldName = array[1];
										}
										else if (array.Length != 0)
										{
											fieldName = array[0];
										}
										if (tblFormular != null && tblFormular[key] != null)
										{
											GridView gridView = tblFormular[key];
											int rowHandle = ((!(gridView.Name == view.Name)) ? gridView.FocusedRowHandle : view.FocusedRowHandle);
											string arg = Function.ToString(gridView.GetRowCellValue(rowHandle, fieldName));
											text = text.Replace(item, $"'{arg}'");
										}
									}
								}
							}
							bool flag = type == "MSCFL";
							frm_CFL frm = new frm_CFL(Enums.TypeCFL.UserDefinedQuery, flag, Function.ToString(view.GetFocusedRowCellValue(col.FieldName)), text, Enums.ItemObjectType.NoItemType, null, view.FormId + view.ItemId + col.FieldName);
							try
							{
								if (flag)
								{
									frm.ButtonEditMultiSelect = true;
									frm_CFL frm_CFL2 = frm;
									frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
									{
										frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
										string[] array2 = Function.ToString(view.GetFocusedRowCellValue(col.FieldName)).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
										frm.gv_CFL.ClearSelection();
										GridColumn column = new GridColumn();
										foreach (GridColumn column2 in frm.gv_CFL.Columns)
										{
											if (column2.FieldName == frm.grd_CFL.DataSourceTable.Columns[0].ColumnName)
											{
												column = column2;
												break;
											}
										}
										string[] array3 = array2;
										foreach (string text2 in array3)
										{
											int rowHandle2 = frm.gv_CFL.LocateByDisplayText(0, column, text2.Trim());
											frm.gv_CFL.SelectRow(rowHandle2);
										}
									}));
								}
								frm.ShowDialog(view.GridControl.FindForm());
								object obj2 = null;
								if (flag && frm.ChooseItems != null && frm.ChooseItems.Count > 0)
								{
									List<DataRow> chooseItems = frm.ChooseItems;
									obj2 = string.Join(",", chooseItems.Select((DataRow x) => Function.ToString(x[0])));
								}
								else if (!flag && frm.ChooseItem != null)
								{
									obj2 = frm.ChooseItem[0];
								}
								if (obj2 != null)
								{
									view.SetFocusedRowCellValue(col, obj2);
								}
							}
							finally
							{
								if (frm != null)
								{
									((IDisposable)frm).Dispose();
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						}
					};
					col.ColumnEdit = repositoryItemButtonEdit2;
					break;
				}
				case "LKO":
				{
					int obj = Function.ParseInt(formula["DependTable"]);
					col.UnboundType = UnboundColumnType.String;
					col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
					RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
					repositoryItemButtonEdit.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
					repositoryItemButtonEdit.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
					repositoryItemButtonEdit.EditFormat.FormatType = col.DisplayFormat.FormatType;
					repositoryItemButtonEdit.EditFormat.FormatString = col.DisplayFormat.FormatString;
					repositoryItemButtonEdit.Buttons.Clear();
					repositoryItemButtonEdit.Buttons.Add(new EditorButton
					{
						Kind = ButtonPredefines.Glyph,
						Image = Resources.right,
						IsLeft = true,
						ToolTip = "Linked to System Object " + Function.ToString((ApzObjectTypes)obj),
						Tag = obj
					});
					repositoryItemButtonEdit.Tag = obj;
					repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
					{
						try
						{
							string text = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
							if (!string.IsNullOrEmpty(text))
							{
								((XtraFormEx)view.GridControl.FindForm()).DrillDown((ApzObjectTypes)obj, (ApzObjectTypes)obj, text);
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
					};
					col.ColumnEdit = repositoryItemButtonEdit;
					break;
				}
				case "LKU":
				{
					string udo = Function.ToString(formula["DependTable"]);
					col.UnboundType = UnboundColumnType.Integer;
					RepositoryItemButtonEdit btn = new RepositoryItemButtonEdit();
					btn.DisplayFormat.FormatType = FormatType.Numeric;
					btn.DisplayFormat.FormatString = "n0";
					btn.EditFormat.FormatType = FormatType.Numeric;
					btn.EditFormat.FormatString = "n0";
					btn.Buttons[0].Kind = ButtonPredefines.Search;
					btn.Buttons.Add(new EditorButton
					{
						Kind = ButtonPredefines.Glyph,
						Image = Resources.right,
						IsLeft = true,
						ToolTip = "Linked to UDO " + udo,
						Tag = udo
					});
					btn.Tag = udo;
					string udoType2 = await _httpClient.Udo.GetTypeByCode(udo);
					btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
					{
						try
						{
							int docEntry = -1;
							string code = "";
							if (udoType2.Equals("3"))
							{
								docEntry = Function.ParseInt(view.GetFocusedRowCellValue(col.FieldName));
							}
							else
							{
								code = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
							}
							frm_UDO frm_UDO2 = new frm_UDO(udo, docEntry, code);
							frm_UDO2.dataRow = view.GetDataRow(view.FocusedRowHandle);
							frm_UDO2.ShowDialog();
							if (frm_UDO2.ChoosedEntry != null)
							{
								view.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
					};
					col.ColumnEdit = btn;
					break;
				}
				}
			}
			if (udf.ValidValues == null || udf.ValidValues.Count <= 0)
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				ValueMember = "Value",
				DisplayMember = "Description",
				NullText = "",
				BestFitMode = BestFitMode.BestFitResizePopup
			};
			switch (udf.Type)
			{
			case BoFieldTypes.db_Alpha:
				repositoryItemLookUpEdit.DataSource = udf.ValidValues;
				break;
			case BoFieldTypes.db_Numeric:
				repositoryItemLookUpEdit.DataSource = udf.ValidValues.Select((ValidValuesMDImpl item) => new
				{
					Value = Function.ParseInt(item.Value),
					Description = item.Description
				});
				break;
			case BoFieldTypes.db_Float:
				repositoryItemLookUpEdit.DataSource = udf.ValidValues.Select((ValidValuesMDImpl item) => new
				{
					Value = Function.ParseDecimal(item.Value),
					Description = item.Description
				});
				break;
			default:
				repositoryItemLookUpEdit.DataSource = udf.ValidValues;
				break;
			}
			col.ColumnEdit = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task LoadRepository(GridViewEx view, GridColumnEx col, DataRow dr, DataTable dtValidValue, DataTable data)
	{
		if (dr == null || !dr.IsExistsColumn("DataType"))
		{
			return;
		}
		IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();
		switch (Function.ToString(dr["DataType"]).ToLower())
		{
		case "alpha":
		case "memo":
		{
			col.UnboundType = UnboundColumnType.String;
			RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
			repositoryItemMemoExEdit.NullText = "";
			repositoryItemMemoExEdit.ShowIcon = false;
			repositoryItemMemoExEdit.BestFitWidth = col.Width;
			repositoryItemMemoExEdit.WordWrap = true;
			repositoryItemMemoExEdit.AcceptsReturn = true;
			repositoryItemMemoExEdit.AllowDropDownWhenReadOnly = DefaultBoolean.True;
			repositoryItemMemoExEdit.ParseEditValue += Memo_ParseEditValue;
			repositoryItemMemoExEdit.FormatEditValue += Memo_FormatEditValue;
			repositoryItemMemoExEdit.Popup += Memo_Popup;
			col.ColumnEdit = repositoryItemMemoExEdit;
			break;
		}
		case "memowrap":
		{
			col.UnboundType = UnboundColumnType.String;
			RepositoryItemMemoEdit repositoryItemMemoEdit = new RepositoryItemMemoEdit();
			repositoryItemMemoEdit.NullText = "";
			repositoryItemMemoEdit.BestFitWidth = col.Width;
			repositoryItemMemoEdit.WordWrap = true;
			repositoryItemMemoEdit.AcceptsReturn = true;
			repositoryItemMemoEdit.ParseEditValue += Memo_ParseEditValue;
			repositoryItemMemoEdit.FormatEditValue += Memo_FormatEditValue;
			view.OptionsView.RowAutoHeight = true;
			repositoryItemMemoEdit.AutoHeight = true;
			col.ColumnEdit = repositoryItemMemoEdit;
			break;
		}
		case "nvarchar":
			col.UnboundType = UnboundColumnType.String;
			break;
		case "int":
		{
			col.UnboundType = UnboundColumnType.Integer;
			RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.DisplayFormat.FormatString = "d";
			repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.EditFormat.FormatString = "d";
			repositoryItemCalcEdit2.Mask.MaskType = MaskType.Numeric;
			repositoryItemCalcEdit2.Mask.EditMask = "d";
			repositoryItemCalcEdit2.AllowMouseWheel = false;
			repositoryItemCalcEdit2.Buttons.Clear();
			col.DisplayFormat.FormatType = FormatType.Numeric;
			col.DisplayFormat.FormatString = "d";
			col.ColumnEdit = repositoryItemCalcEdit2;
			break;
		}
		case "decimal":
		case "float":
		case "money":
		case "smallint":
		case "numeric":
		{
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				col.UnboundType = UnboundColumnType.String;
				col.DisplayFormatType = Enums.DisplayFormatType.Time;
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
				col.ColumnEdit = repositoryItemTextEdit;
				break;
			}
			col.UnboundType = UnboundColumnType.Decimal;
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.AllowMouseWheel = false;
			col.ColumnEdit = repositoryItemCalcEdit;
			_ = col.DisplayFormat.FormatString;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])) && Function.ParseInt(dr["DisplayFormat"]) <= 0)
			{
				string formatString = Function.ToString(dr["DisplayFormat"]);
				repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.DisplayFormat.FormatString = formatString;
				repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.EditFormat.FormatString = formatString;
			}
			break;
		}
		case "datetime":
		case "date":
		{
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				col.UnboundType = UnboundColumnType.DateTime;
				RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
				repositoryItemDateEdit.Name = col.Name;
				repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.DisplayFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.EditFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.EditFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.AllowMouseWheel = false;
				col.DisplayFormat.FormatType = FormatType.DateTime;
				col.DisplayFormat.FormatString = "HH:mm";
				col.ColumnEdit = repositoryItemDateEdit;
				break;
			}
			col.UnboundType = UnboundColumnType.DateTime;
			col.DisplayFormatType = Enums.DisplayFormatType.DateTime;
			RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
			repositoryItemDateEditEx.Name = col.Name;
			repositoryItemDateEditEx.AllowMouseWheel = false;
			col.DisplayFormat.FormatType = FormatType.DateTime;
			col.ColumnEdit = repositoryItemDateEditEx;
			string text = GlobalData.DateTimeFormat;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])) && Function.ParseInt(dr["DisplayFormat"]) <= 0)
			{
				text = Function.ToString(dr["DisplayFormat"]);
				repositoryItemDateEditEx.DisplayFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEditEx.DisplayFormat.FormatString = text;
				repositoryItemDateEditEx.EditFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEditEx.EditFormat.FormatString = text;
				repositoryItemDateEditEx.Mask.MaskType = MaskType.DateTime;
				repositoryItemDateEditEx.Mask.EditMask = text;
				repositoryItemDateEditEx.Mask.UseMaskAsDisplayFormat = true;
			}
			col.DisplayFormat.FormatString = text;
			break;
		}
		case "time":
		{
			col.UnboundType = UnboundColumnType.String;
			col.DisplayFormatType = Enums.DisplayFormatType.Time;
			RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
			repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
			repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
			repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
			repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
			repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
			repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
			col.ColumnEdit = repositoryItemTextEdit2;
			break;
		}
		case "bit":
		{
			col.UnboundType = UnboundColumnType.Boolean;
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit.AutoHeight = false;
			col.ColumnEdit = repositoryItemCheckEdit;
			data.ChangeColumnDataType(col.FieldName, typeof(bool));
			view.DoubleClick += (object s, EventArgs e) =>
			{
				try
				{
					Point pt = view.GridControl.PointToClient(Control.MousePosition);
					GridHitInfo gridHitInfo = view.CalcHitInfo(pt);
					if (gridHitInfo.Column == col && gridHitInfo.InColumn)
					{
						DataTable dataTable = view.GridControl.DataSourceTable();
						bool flag = dataTable.Select("[" + col.FieldName + "]= 'TRUE'").Length != 0;
						foreach (DataRow row in dataTable.Rows)
						{
							row[col.FieldName] = !flag;
						}
						dataTable.AcceptChanges();
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			break;
		}
		case "link":
		{
			col.UnboundType = UnboundColumnType.String;
			RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
			repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
			col.ColumnEdit = repositoryItemAttachFile;
			break;
		}
		case "validvalue":
		{
			string text2 = Function.ToString(dr["SubType"]);
			if (!string.IsNullOrEmpty(text2))
			{
				DataTable data2 = (await _httpClient.Commons.ExecuteQuery(text2)).Data;
				_httpClient = null;
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit();
				repositoryItemLookUpEdit2.DataSource = data2;
				if (data2 != null && data2.Columns.Count >= 2)
				{
					repositoryItemLookUpEdit2.ValueMember = data2.Columns[0].ColumnName;
					repositoryItemLookUpEdit2.DisplayMember = data2.Columns[1].ColumnName;
				}
				repositoryItemLookUpEdit2.NullText = string.Empty;
				repositoryItemLookUpEdit2.BestFitMode = BestFitMode.BestFitResizePopup;
				col.ColumnEdit = repositoryItemLookUpEdit2;
			}
			else
			{
				if (!dtValidValue.IsNotNull())
				{
					break;
				}
				DataRow[] array3 = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
				if (!array3.IsNotNull())
				{
					break;
				}
				DataTable dt2 = array3.CopyToDataTable();
				RepositoryItemLookUpEdit repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit
				{
					DataSource = dt2,
					ValueMember = "Value",
					DisplayMember = "Description",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				repositoryItemLookUpEdit3.PopulateColumns();
				for (int num2 = 0; num2 < repositoryItemLookUpEdit3.Columns.Count; num2++)
				{
					if (!repositoryItemLookUpEdit3.Columns[num2].FieldName.Equals("Description"))
					{
						repositoryItemLookUpEdit3.Columns[num2].Visible = false;
					}
				}
				col.ColumnEdit = repositoryItemLookUpEdit3;
				if (!dt2.IsExistsColumn("BackColor") || string.IsNullOrEmpty(Function.ToString(dt2.Rows[0])))
				{
					break;
				}
				view.RowCellStyle += (object s, RowCellStyleEventArgs e) =>
				{
					try
					{
						if (e.Column.FieldName == col.FieldName)
						{
							foreach (DataRow row2 in dt2.Rows)
							{
								string text3 = Function.ToString(row2["Value"]);
								if (e.CellValue != null && text3.Equals(e.CellValue.ToString(), StringComparison.OrdinalIgnoreCase))
								{
									string text4 = Function.ToString(row2["BackColor"]);
									string text5 = Function.ToString(row2["ForeColor"]);
									if (!string.IsNullOrEmpty(text4))
									{
										e.Appearance.BackColor = Color.FromName(text4);
									}
									if (!string.IsNullOrEmpty(text5))
									{
										e.Appearance.ForeColor = Color.FromName(text5);
									}
								}
							}
							return;
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
				};
			}
			break;
		}
		case "validcolor":
		{
			if (!dtValidValue.IsNotNull())
			{
				break;
			}
			DataRow[] array = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
			if (!array.IsNotNull())
			{
				break;
			}
			DataTable dt = array.CopyToDataTable();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				DataSource = dt,
				ValueMember = "Value",
				DisplayMember = "Description",
				NullText = "",
				BestFitMode = BestFitMode.BestFitResizePopup
			};
			repositoryItemLookUpEdit.PopulateColumns();
			for (int num = 0; num < repositoryItemLookUpEdit.Columns.Count; num++)
			{
				if (!repositoryItemLookUpEdit.Columns[num].FieldName.Equals("Description"))
				{
					repositoryItemLookUpEdit.Columns[num].Visible = false;
				}
			}
			col.ColumnEdit = repositoryItemLookUpEdit;
			view.RowStyle += (object s, RowStyleEventArgs e) =>
			{
				try
				{
					if (e.RowHandle >= 0)
					{
						string text3 = Function.ToString(view.GetRowCellValue(e.RowHandle, col.FieldName));
						{
							foreach (DataRow row3 in dt.Rows)
							{
								if (text3.Equals(Function.ToString(row3["Value"]), StringComparison.OrdinalIgnoreCase))
								{
									string text4 = Function.ToString(row3["BackColor"]);
									string text5 = Function.ToString(row3["ForeColor"]);
									if (!string.IsNullOrEmpty(text4))
									{
										e.Appearance.BackColor = Color.FromName(text4);
									}
									if (!string.IsNullOrEmpty(text5))
									{
										e.Appearance.ForeColor = Color.FromName(text5);
									}
									e.HighPriority = true;
								}
							}
							return;
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			break;
		}
		case "color":
		{
			if (!dtValidValue.IsNotNull())
			{
				break;
			}
			DataRow[] array4 = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
			if (!array4.IsNotNull())
			{
				break;
			}
			DataTable dt3 = array4.CopyToDataTable();
			view.RowCellStyle += (object s, RowCellStyleEventArgs e) =>
			{
				try
				{
					if (e.Column.FieldName == col.FieldName)
					{
						foreach (DataRow row4 in dt3.Rows)
						{
							string text3 = Function.ToString(row4["Value"]);
							if (e.CellValue != null && text3.Equals(e.CellValue.ToString(), StringComparison.OrdinalIgnoreCase))
							{
								string text4 = Function.ToString(row4["BackColor"]);
								string text5 = Function.ToString(row4["ForeColor"]);
								if (!string.IsNullOrEmpty(text4))
								{
									e.Appearance.BackColor = Color.FromName(text4);
								}
								if (!string.IsNullOrEmpty(text5))
								{
									e.Appearance.ForeColor = Color.FromName(text5);
								}
							}
						}
						return;
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			break;
		}
		case "linkedtable":
			col.UnboundType = UnboundColumnType.String;
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			if (!string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				List<Common.Table_Code> dataSource = await _httpClient.Udf.GetLinkedTable(Function.ToString(dr["SubType"]), GlobalData.UserName, GlobalData.DefaultBranch);
				RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource,
					ValueMember = "Code",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				col.ColumnEdit = columnEdit;
			}
			break;
		case "linkedudo":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			RepositoryItemButtonEdit repositoryItemButtonEdit10 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit10.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit10.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit10.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit10.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit10.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit10.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				ToolTip = "Linked to User Defined Object",
				IsLeft = true
			});
			string subType = Function.ToString(dr["SubType"]);
			string udoCode = subType;
			repositoryItemButtonEdit10.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (subType.StartsWith("@"))
					{
						udoCode = Function.ToString(view.GetFocusedRowCellValue(subType.Substring(1)));
					}
					string text3 = await _httpClient.Udo.GetTypeByCode(udoCode);
					int docEntry = -1;
					string code = "";
					if (text3.Equals("3"))
					{
						docEntry = Function.ParseInt(view.GetFocusedRowCellValue(col.FieldName));
					}
					else
					{
						code = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
					}
					frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry, code);
					frm_UDO2.dataRow = view.GetDataRow(view.FocusedRowHandle);
					frm_UDO2.ShowDialog();
					if (frm_UDO2.ChoosedEntry != null)
					{
						view.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit10;
			break;
		}
		case "linkedobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit repositoryItemButtonEdit9 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit9.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit9.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit9.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit9.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit9.Buttons.Clear();
			repositoryItemButtonEdit9.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject "
			});
			repositoryItemButtonEdit9.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					int num3 = 0;
					string empty = string.Empty;
					string text3 = Function.ToString(dr["SubType"]);
					string fieldName = col.FieldName;
					if (text3.Split(' ').Length > 1 && text3.Split(' ')[1].StartsWith("#"))
					{
						fieldName = Function.ToString(text3.Split(' ')[1].Substring(1));
						text3 = Function.ToString(text3.Split(' ')[0]);
					}
					if (text3.StartsWith("@"))
					{
						string text4 = Function.ToString(view.GetFocusedRowCellValue(text3.Substring(1)));
						if (text4.StartsWith("W"))
						{
							num3 = 417;
							empty = text4 + "=" + Function.ToString(view.GetFocusedRowCellValue(fieldName));
						}
						else
						{
							num3 = Function.ParseInt(view.GetFocusedRowCellValue(text3.Substring(1)));
							empty = Function.ToString(view.GetFocusedRowCellValue(fieldName));
						}
					}
					else if (text3.StartsWith("W"))
					{
						num3 = 417;
						empty = text3 + "=" + Function.ToString(view.GetFocusedRowCellValue(fieldName));
					}
					else
					{
						num3 = Function.ParseInt(text3);
						empty = Function.ToString(view.GetFocusedRowCellValue(fieldName));
					}
					if (!string.IsNullOrEmpty(empty))
					{
						((XtraFormEx)view.GridControl.FindForm()).DrillDown((ApzObjectTypes)num3, (ApzObjectTypes)num3, empty);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit9;
			break;
		}
		case "linkedcflobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit repositoryItemButtonEdit8 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit8.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit8.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit8.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit8.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit8.Buttons[0].Kind = ButtonPredefines.Search;
			string[] array2 = Function.ToString(dr["SubType"]).Split(',');
			string store2 = "";
			string type = array2[0];
			if (array2.Length > 1)
			{
				store2 = Function.ToString(array2[1]);
			}
			repositoryItemButtonEdit8.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject ",
				Tag = type
			});
			repositoryItemButtonEdit8.Tag = type;
			repositoryItemButtonEdit8.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (e.Button.Index == 0)
					{
						ShowChooseFromList(view, col, dr, store2);
					}
					else if (e.Button.Index == 1)
					{
						int num3 = 0;
						num3 = ((!type.StartsWith("@")) ? Function.ParseInt(type) : Function.ParseInt(view.GetFocusedRowCellValue(type.Substring(1))));
						string text3 = Function.ToString(view.GetFocusedRowCellValue(col.FieldName));
						if (!string.IsNullOrEmpty(text3))
						{
							((XtraFormEx)view.GridControl.FindForm()).DrillDown((ApzObjectTypes)num3, (ApzObjectTypes)num3, text3);
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit8;
			break;
		}
		case "linkedstore":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit btnStore = new RepositoryItemButtonEdit();
			btnStore.Buttons.Clear();
			btnStore.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true
			});
			string store = Function.ToString(dr["SubType"]);
			btnStore.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					btnStore.Buttons[0].Enabled = false;
					string text3 = Function.ToString(store.Split(' ')[0]);
					string name = "frm_" + text3;
					frm_ReportForm frm_ReportForm2 = new frm_ReportForm(text3);
					frm_ReportForm2.Name = name;
					frm_ReportForm2.dataRow = view.GetDataRow(view.FocusedRowHandle);
					frm_ReportForm2.mappingParams = Function.ToString(store.Split(' ')[1]);
					frm_ReportForm2.isExecuteWhenLoad = true;
					frm_ReportForm2.ShowDialog(view.GridControl.FindForm());
					btnStore.Buttons[0].Enabled = true;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = btnStore;
			break;
		}
		case "linkedcrystal":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit btnCrytal = new RepositoryItemButtonEdit();
			btnCrytal.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			btnCrytal.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			btnCrytal.EditFormat.FormatType = col.DisplayFormat.FormatType;
			btnCrytal.EditFormat.FormatString = col.DisplayFormat.FormatString;
			string fileName = Function.ToString(dr["SubType"]);
			btnCrytal.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					btnCrytal.Buttons[0].Enabled = false;
					string text3 = await WinFuntions.ReportViewerDatarowParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{fileName}", view.GetDataRow(view.FocusedRowHandle));
					if (!string.IsNullOrEmpty(text3))
					{
						WinCommons.Error(Enums.MessageCode.None, text3);
					}
					btnCrytal.Buttons[0].Enabled = true;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = btnCrytal;
			break;
		}
		case "cfl":
		case "mscfl":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit7 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit7.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit7.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit7.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit7.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit7.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit7.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowChooseFromList(view, col, dr, "", Function.ToString(dr["DataType"]).ToLower() == "mscfl");
			};
			repositoryItemButtonEdit7.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowChooseFromList(view, col, dr, "", Function.ToString(dr["DataType"]).ToLower() == "mscfl");
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit7;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "sub":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit6 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit6.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit6.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit6.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit6.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit6.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit6.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowSubForm(view, dr);
			};
			repositoryItemButtonEdit6.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowSubForm(view, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit6;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webex":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit5 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit5.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit5.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit5.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit5.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit5.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit5.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowDefaultBrowser(view, col, dr);
			};
			repositoryItemButtonEdit5.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowDefaultBrowser(view, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit5;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "web":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit4 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit4.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit4.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit4.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit4.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit4.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit4.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(view, col, dr);
			};
			repositoryItemButtonEdit4.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(view, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit4;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webtimer":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit3 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit3.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit3.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit3.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit3.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit3.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit3.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(view, col, dr, Enums.WebType.Address, isTimer: true);
			};
			repositoryItemButtonEdit3.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(view, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit3;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webhtml":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit2.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit2.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit2.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit2.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(view, col, dr, Enums.WebType.Html);
			};
			repositoryItemButtonEdit2.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(view, col, dr, Enums.WebType.Html);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit2;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webhtmltimer":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.DisplayFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit.DisplayFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit.EditFormat.FormatType = col.DisplayFormat.FormatType;
			repositoryItemButtonEdit.EditFormat.FormatString = col.DisplayFormat.FormatString;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(view, col, dr, Enums.WebType.Html, isTimer: true);
			};
			repositoryItemButtonEdit.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(view, col, dr, Enums.WebType.Html, isTimer: true);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		default:
			col.UnboundType = UnboundColumnType.String;
			break;
		}
	}

	private static void ShowChooseFromList(GridViewEx view, GridColumnEx col, DataRow dr, string storeName = "", bool multiSelect = false)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			if (!string.IsNullOrEmpty(storeName))
			{
				text = storeName;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int num = 0;
			if (text.StartsWith("@"))
			{
				num = Function.ParseInt(view.GetFocusedRowCellValue(text.Substring(1)));
			}
			else if (Function.ParseInt(text) > 0)
			{
				num = Function.ParseInt(text);
			}
			if (num > 0)
			{
				using (frm_CFL frm_CFL2 = new frm_CFL((Enums.TypeCFL)num, multiSelect, Function.ToString(view.GetFocusedRowCellValue(col.FieldName)), "", Enums.ItemObjectType.NoItemType, null, ""))
				{
					frm_CFL2.Text = $"{frm_CFL2.Text} - {col.Caption}";
					frm_CFL2.ShowDialog();
					DataRow chooseItem = frm_CFL2.ChooseItem;
					if (chooseItem != null)
					{
						WinCommons._OpenProcessing();
						int focusedRowHandle = view.FocusedRowHandle;
						if (chooseItem.IsExistsColumn(col.FieldName))
						{
							WinFuntions.DataRowToGridView(chooseItem, view, focusedRowHandle);
						}
						else
						{
							view.SetRowCellValue(focusedRowHandle, col, chooseItem[0]);
						}
						WinCommons._CloseProcessing();
					}
					return;
				}
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			string[] array = text.Split(' ');
			if (array.Length > 1 && array[1].StartsWith("@"))
			{
				DataRow dataRow = view.GetDataRow(view.FocusedRowHandle);
				text = array[0].Trim();
				string[] array2 = array[1].Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Substring(1).Trim();
					if (array2[i].Contains("="))
					{
						int num2 = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num2), array2[i].Substring(num2 + 1).Replace("'", "")));
					}
					else if (text2.ToLower().Contains("username"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.UserName));
					}
					else if (text2.ToLower().Contains("branch"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.DefaultBranch));
					}
					else if (dataRow.IsExistsColumn(text2))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), dataRow[text2]));
					}
					else
					{
						list.Add(new Common.ParameterValue("", array2[i].Trim().Replace("'", "")));
					}
				}
			}
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, multiSelect, Function.ToString(view.GetFocusedRowCellValue(col.FieldName)), text, Enums.ItemObjectType.NoItemType, list, "");
			try
			{
				frm.Text = $"{frm.Text} - {col.Caption}";
				if (multiSelect)
				{
					frm.ButtonEditMultiSelect = true;
					frm_CFL frm_CFL3 = frm;
					frm_CFL3.AfterLoadForm = (Action)Delegate.Combine(frm_CFL3.AfterLoadForm, (Action)(() =>
					{
						frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
						string[] array3 = Function.ToString(view.GetFocusedRowCellValue(col.FieldName)).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						frm.gv_CFL.ClearSelection();
						GridColumn column = new GridColumn();
						foreach (GridColumn column2 in frm.gv_CFL.Columns)
						{
							if (column2.FieldName == frm.grd_CFL.DataSourceTable.Columns[0].ColumnName)
							{
								column = column2;
								break;
							}
						}
						string[] array4 = array3;
						foreach (string text3 in array4)
						{
							int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text3.Trim());
							frm.gv_CFL.SelectRow(rowHandle);
						}
					}));
				}
				frm.ShowDialog(view.GridControl.FindForm());
				object obj = null;
				if (multiSelect && frm.ChooseItems != null && frm.ChooseItems.Count > 0)
				{
					List<DataRow> chooseItems = frm.ChooseItems;
					obj = string.Join(",", chooseItems.Select((DataRow r) => Function.ToString(r[0])));
				}
				else if (!multiSelect && frm.ChooseItem != null)
				{
					obj = frm.ChooseItem[0];
				}
				if (obj != null)
				{
					view.SetFocusedRowCellValue(col, obj);
				}
			}
			finally
			{
				if (frm != null)
				{
					((IDisposable)frm).Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void ShowSubForm(GridViewEx view, DataRow dr)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			string[] array = text.Split(' ');
			if (array.Length > 1 && array[1].StartsWith("@"))
			{
				DataRow dataRow = view.GetDataRow(view.FocusedRowHandle);
				text = array[0].Trim();
				string[] array2 = array[1].Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Substring(1).Trim();
					if (array2[i].Contains("="))
					{
						int num = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1).Replace("'", "")));
					}
					else if (text2.ToLower().Contains("username"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.UserName));
					}
					else if (text2.ToLower().Contains("branch"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.DefaultBranch));
					}
					else if (dataRow.IsExistsColumn(text2))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), dataRow[text2]));
					}
					else
					{
						list.Add(new Common.ParameterValue("", array2[i].Trim().Replace("'", "")));
					}
				}
			}
			string text3 = Function.ToString(dr["Caption"]);
			foreach (Form openForm in Application.OpenForms)
			{
				if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_SubReportForm) && openForm.Tag.ToString().Equals(text + "_" + text3))
				{
					openForm.Activate();
					return;
				}
			}
			frm_SubReportForm frm = new frm_SubReportForm(text, list);
			frm.Name = text + "_" + text3;
			frm.Tag = text + "_" + text3;
			frm.Text = string.Format("{0} - {1}", "Sub Report Form", text3);
			frm.MdiParent = view.GridControl.FindForm().MdiParent;
			frm.WindowState = FormWindowState.Normal;
			frm.StartPosition = FormStartPosition.CenterScreen;
			frm.Show();
			view.GridControl.FindForm().FormClosing += (object s, FormClosingEventArgs e) =>
			{
				if (frm != null)
				{
					frm.Close();
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static async void ShowDefaultBrowser(GridViewEx view, GridColumnEx col, DataRow dr)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			string fieldName = col.FieldName;
			if (text.StartsWith("#"))
			{
				fieldName = text.Trim().Replace("#", "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					DataRow dataRow = view.GetDataRow(view.FocusedRowHandle);
					text = array[0].Trim();
					string[] array2 = array[1].Split(',');
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i].Substring(1).Trim();
						if (array2[i].Contains("="))
						{
							int num = array2[i].IndexOf('=');
							list.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1).Replace("'", "")));
						}
						else if (text2.ToLower().Contains("username"))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.UserName));
						}
						else if (text2.ToLower().Contains("branch"))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.DefaultBranch));
						}
						else if (dataRow.IsExistsColumn(text2))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), dataRow[text2]));
						}
						else
						{
							list.Add(new Common.ParameterValue("", array2[i].Trim().Replace("'", "")));
						}
					}
				}
				DataSetResult dataSetResult = await HttpClientHelperFactory.GetHttpClient().Commons.ExecuteStore(text, list);
				if (!string.IsNullOrEmpty(dataSetResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				}
				if (dataSetResult.Data.IsNotNull() && dataSetResult.Data.Tables[0].IsNotNull())
				{
					string text3 = Function.ToString(dataSetResult.Data.Tables[0].Rows[0][0]);
					if (!text3.StartsWith("http"))
					{
						text3 = "http://www.google.com/maps/place/" + HttpUtility.UrlEncode(text3);
					}
					Process.Start(text3);
				}
			}
			else
			{
				string text4 = Function.ToString(view.GetFocusedRowCellValue(fieldName));
				if (!text4.StartsWith("http"))
				{
					text4 = "http://www.google.com/maps/place/" + HttpUtility.UrlEncode(text4);
				}
				Process.Start(text4);
			}
		}
		catch (Win32Exception ex)
		{
			if (ex.ErrorCode == -2147467259)
			{
				MessageBox.Show(ex.Message);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static void ShowWebBrowser(GridViewEx view, GridColumnEx col, DataRow dr, Enums.WebType type = Enums.WebType.Address, bool isTimer = false)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			string fieldName = col.FieldName;
			if (text.StartsWith("#"))
			{
				fieldName = text.Trim().Replace("#", "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					DataRow dataRow = view.GetDataRow(view.FocusedRowHandle);
					text = array[0].Trim();
					string[] array2 = array[1].Split(',');
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i].Substring(1).Trim();
						if (array2[i].Contains("="))
						{
							int num = array2[i].IndexOf('=');
							list.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1).Replace("'", "")));
						}
						else if (text2.ToLower().Contains("username"))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.UserName));
						}
						else if (text2.ToLower().Contains("branch"))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.DefaultBranch));
						}
						else if (dataRow.IsExistsColumn(text2))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), dataRow[text2]));
						}
						else
						{
							list.Add(new Common.ParameterValue("", array2[i].Trim().Replace("'", "")));
						}
					}
				}
				frm_ChromeWebBrowser frm_ChromeWebBrowser2 = new frm_ChromeWebBrowser();
				frm_ChromeWebBrowser2.Text = col.Caption;
				frm_ChromeWebBrowser2.storeName = text;
				frm_ChromeWebBrowser2.lstpr = list;
				frm_ChromeWebBrowser2.type = type;
				frm_ChromeWebBrowser2.isTimer = isTimer;
				frm_ChromeWebBrowser2.ShowDialog(view.GridControl.FindForm());
				return;
			}
			string text3 = Function.ToString(view.GetFocusedRowCellValue(fieldName));
			switch (type)
			{
			case Enums.WebType.Address:
			{
				if (!text3.StartsWith("http"))
				{
					text3 = "http://www.google.com/maps/place/" + HttpUtility.UrlEncode(text3);
				}
				frm_ChromeWebBrowser frm_ChromeWebBrowser4 = new frm_ChromeWebBrowser();
				frm_ChromeWebBrowser4.Text = col.Caption;
				frm_ChromeWebBrowser4.url = text3;
				frm_ChromeWebBrowser4.isTimer = isTimer;
				frm_ChromeWebBrowser4.ShowDialog(view.GridControl.FindForm());
				break;
			}
			case Enums.WebType.Html:
			{
				frm_ChromeWebBrowser frm_ChromeWebBrowser3 = new frm_ChromeWebBrowser();
				frm_ChromeWebBrowser3.Text = col.Caption;
				frm_ChromeWebBrowser3.url = text3;
				frm_ChromeWebBrowser3.type = Enums.WebType.Html;
				frm_ChromeWebBrowser3.isTimer = isTimer;
				frm_ChromeWebBrowser3.ShowDialog(view.GridControl.FindForm());
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void Memo_Popup(object sender, EventArgs e)
	{
		try
		{
			if (sender is IPopupControl { PopupWindow: MemoExPopupForm { ActiveControl: TextBoxMaskBox activeControl } })
			{
				string text = Convert.ToString(activeControl.EditValue);
				text = text.Replace("(newline)", Environment.NewLine);
				activeControl.EditValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void Memo_FormatEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			text = text.Replace("(newline)", Environment.NewLine);
			e.Value = text;
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void Memo_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			text = text.Replace(Environment.NewLine, "(newline)");
			e.Value = text;
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
