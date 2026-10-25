using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Reports;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using SAPbobsCOM;

namespace Apzon.Forms.Helper;

public static class PivotGridHelper
{
	public static bool dfShowGroupPanel = false;

	public static int dfWidth = 75;

	public static bool dfVisible = false;

	public static bool dfReadOnly = false;

	public static bool dfAllowEdit = true;

	public static DefaultBoolean dfAllowMove = DefaultBoolean.True;

	public static bool dfAllowSize = true;

	public static bool dfAutoHeightHeader = true;

	public static bool dfAppearanceEvenRow = true;

	public static DefaultBoolean dfAllowSort = DefaultBoolean.True;

	public static NewItemRowPosition dfNewItemRowPosition = NewItemRowPosition.None;

	public static async Task GeneratePivotGrid(PivotGridControlEx pivot, DataSet ds, List<UserFieldsImpl> lstUDF = null, bool expand = false, bool filterRow = false, string tableName = "", Dictionary<string, PivotGridField> tblFormular = null)
	{
		_ = 1;
		try
		{
			if (pivot != null)
			{
				pivot.EditValueChanged += Pivot_EditValueChanged;
			}
			if (ds.IsHasStructure())
			{
				if (pivot.Fields.Count > 0 && pivot.Fields.Count == ds.Tables[1].Columns.Count)
				{
					pivot.DataSource = ds.Tables[1];
					pivot.RefreshData();
					if (expand)
					{
						pivot.ExpandAll();
					}
					if (!ds.Tables[0].IsExistsColumn("Expression"))
					{
						return;
					}
					DataRow[] array = ds.Tables[0].Select("[Expression] <> ''");
					if (array.Length == 0)
					{
						return;
					}
					try
					{
						DataRow[] array2 = array;
						foreach (DataRow dataRow in array2)
						{
							string name = Function.ToString(dataRow["ColumnName"]);
							ds.Tables[1].Columns[name].Expression = Function.ToString(dataRow["Expression"]);
						}
						return;
					}
					catch (Exception ex)
					{
						WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
						return;
					}
				}
				if (ds.IsHasValidValue())
				{
					await GeneratePivotGrid(pivot, ds.Tables[0], ds.Tables[1], ds.Tables[2], lstUDF, expand, tableName, tblFormular);
				}
				else
				{
					await GeneratePivotGrid(pivot, ds.Tables[0], ds.Tables[1], null, lstUDF, expand, tableName, tblFormular);
				}
			}
			else
			{
				if (!ds.IsNotNull())
				{
					return;
				}
				if (pivot.Fields.Count > 0 && pivot.Fields.Count == ds.Tables[0].Columns.Count)
				{
					pivot.DataSource = ds.Tables[0];
					pivot.RefreshData();
				}
				else
				{
					pivot.BeginUpdate();
					pivot.Fields.Clear();
					foreach (DataColumn column in ds.Tables[0].Columns)
					{
						PivotGridField pivotGridField = new PivotGridField();
						pivotGridField.FieldName = column.ColumnName;
						pivotGridField.Caption = column.Caption;
						pivotGridField.Visible = true;
						pivotGridField.Options.AllowFilter = DefaultBoolean.True;
						pivotGridField.Width = dfWidth;
						pivotGridField.BestFit();
						pivotGridField.Area = PivotArea.RowArea;
						pivotGridField.Options.AllowHide = DefaultBoolean.True;
						pivotGridField.Options.ShowInCustomizationForm = true;
						pivot.Fields.Add(pivotGridField);
					}
					pivot.DataSource = ds.Tables[0];
					pivot.RefreshData();
					pivot.EndUpdate();
				}
				if (expand)
				{
					pivot.ExpandAll();
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static void Pivot_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null || e.RowField == null || !(sender is PivotGridControlEx pivotGridControlEx))
			{
				return;
			}
			PivotDrillDownDataSource pivotDrillDownDataSource = e.CreateDrillDownDataSource();
			int columnCount = pivotGridControlEx.Cells.ColumnCount;
			if (e.ColumnIndex < columnCount - 1 && !(e.Editor is CalcEdit))
			{
				return;
			}
			for (int i = 0; i < columnCount; i++)
			{
				for (int j = 0; j < pivotDrillDownDataSource.RowCount; j++)
				{
					if (pivotDrillDownDataSource[j] != null && e.ColumnIndex == i)
					{
						double num = Function.ParseDouble(e.Editor.EditValue);
						pivotDrillDownDataSource[j][e.DataField] = num;
						pivotGridControlEx.DataSourceTable().AcceptChanges();
						pivotGridControlEx.RefreshData();
						return;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static async Task GeneratePivotGrid(PivotGridControlEx pivot, DataTable dtStructure, DataTable dt, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, bool expand = false, string tableName = "", Dictionary<string, PivotGridField> tblFormular = null)
	{
		try
		{
			pivot.DataSource = null;
			pivot.Fields.Clear();
			pivot.OptionsView.ShowColumnGrandTotals = false;
			pivot.OptionsView.ShowGrandTotalsForSingleValues = false;
			pivot.OptionsView.ShowColumnTotals = false;
			pivot.OptionsView.ShowRowTotals = false;
			pivot.OptionsView.ShowRowGrandTotals = false;
			if (dt == null || dt.Columns.Count <= 0)
			{
				await GenerateColumn(pivot, dtStructure, dtValidValue, lstUDF, tableName);
			}
			else
			{
				int idx = 0;
				string filterString = "";
				int i;
				for (i = 0; i < dt.Columns.Count; i++)
				{
					PivotGridField pivotGridField = pivot.Fields.Add();
					pivotGridField.Name = $"cl_{dt.Columns[i].ColumnName}";
					pivotGridField.FieldName = dt.Columns[i].ColumnName;
					DataRow dataRow = (dtStructure.Rows[0].IsExistsColumn("ColumnName") ? (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").ToUpper().Equals(dt.Columns[i].ColumnName.ToUpper())
						select c).FirstOrDefault() : null);
					if (dataRow != null)
					{
						pivotGridField.Caption = (dataRow.IsExistsColumn("Caption") ? Function.ToString(dataRow["Caption"]) : dt.Columns[i].ColumnName);
						pivotGridField.Width = (dataRow.IsExistsColumn("Width") ? Function.ParseInt(dataRow["Width"]) : dfWidth);
						if (dataRow.IsExistsColumn("PivotArea") && !string.IsNullOrEmpty(Function.ToString(dataRow["PivotArea"])))
						{
							switch (Function.ToString(dataRow["PivotArea"]).ToLower())
							{
							case "data":
								pivotGridField.Area = PivotArea.DataArea;
								break;
							case "column":
								pivotGridField.Area = PivotArea.ColumnArea;
								break;
							case "filter":
								pivotGridField.Area = PivotArea.FilterArea;
								break;
							case "row":
								pivotGridField.Area = PivotArea.RowArea;
								break;
							default:
								pivotGridField.Area = PivotArea.RowArea;
								break;
							}
						}
						if (dataRow.IsExistsColumn("Visible"))
						{
							pivotGridField.Visible = Function.ParseBool(dataRow["Visible"]);
						}
						else
						{
							pivotGridField.Visible = dfVisible;
						}
						if (!pivotGridField.Visible)
						{
							pivotGridField.Options.ShowInCustomizationForm = true;
						}
						pivotGridField.AreaIndex = (pivotGridField.Visible ? idx : (-1));
						idx = (pivotGridField.Visible ? (idx + 1) : idx);
						pivotGridField.Options.ReadOnly = (dataRow.IsExistsColumn("ReadOnly") ? Function.ParseBool(dataRow["ReadOnly"]) : dfReadOnly);
						pivotGridField.Options.AllowEdit = (dataRow.IsExistsColumn("AllowEdit") ? Function.ParseBool(dataRow["AllowEdit"]) : dfAllowEdit);
						pivotGridField.Options.AllowDrag = ((!Function.ParseBool(dataRow["AllowMove"])) ? DefaultBoolean.False : DefaultBoolean.True);
						pivotGridField.Options.AllowSort = ((!dataRow.IsExistsColumn("AllowSort") || !Function.ParseBool(dataRow["AllowSort"])) ? dfAllowSort : DefaultBoolean.True);
						if (dataRow.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(dataRow["FormatType"])))
						{
							string text = Function.ToString(dataRow["FormatType"]).ToLower();
							if (!(text == "numeric"))
							{
								if (text == "datetime")
								{
									pivotGridField.ValueFormat.FormatType = FormatType.DateTime;
									pivotGridField.CellFormat.FormatType = FormatType.DateTime;
								}
								else
								{
									pivotGridField.ValueFormat.FormatType = FormatType.None;
									pivotGridField.CellFormat.FormatType = FormatType.None;
								}
							}
							else
							{
								pivotGridField.ValueFormat.FormatType = FormatType.Numeric;
								pivotGridField.CellFormat.FormatType = FormatType.Numeric;
							}
						}
						if (dataRow.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["DisplayFormat"])))
						{
							if (Function.ParseInt(dataRow["DisplayFormat"]) > 0)
							{
								DisplayFormat(pivotGridField, Function.ParseInt(dataRow["DisplayFormat"]));
							}
							else
							{
								pivotGridField.ValueFormat.FormatString = Function.ToString(dataRow["DisplayFormat"]);
								pivotGridField.CellFormat.FormatString = Function.ToString(dataRow["DisplayFormat"]);
							}
						}
						string format = pivotGridField.ValueFormat.FormatString;
						if (dataRow.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryFormat"])))
						{
							format = Function.ToString(dataRow["SummaryFormat"]);
						}
						if (dataRow.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryType"])))
						{
							pivot.OptionsView.ShowColumnGrandTotals = true;
							pivot.OptionsView.ShowColumnGrandTotalHeader = true;
							pivot.OptionsView.ShowColumnTotals = true;
							GetSummary(pivotGridField, Function.ToString(dataRow["SummaryType"]).ToLower(), format);
						}
						if (dataRow.IsExistsColumn("GroupSummaryType") && !string.IsNullOrEmpty(Function.ToString(dataRow["GroupSummaryType"])))
						{
							pivot.OptionsView.ShowRowGrandTotals = true;
							if (dataRow.IsExistsColumn("GroupSummaryShowInFooter"))
							{
								Function.ParseBool(dataRow["GroupSummaryShowInFooter"]);
							}
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
							pivotGridField.Appearance.Cell.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
							pivotGridField.Appearance.Header.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
							pivotGridField.Appearance.CellTotal.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
							pivotGridField.Appearance.CellGrandTotal.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
						}
						if (dataRow.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(dataRow["ForeColor"])))
						{
							pivotGridField.Appearance.Cell.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
							pivotGridField.Appearance.Header.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
							pivotGridField.Appearance.CellTotal.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
							pivotGridField.Appearance.CellGrandTotal.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
						}
						if (dataRow.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(dataRow["FilterString"])))
						{
							filterString = filterString + Function.ToString(dataRow["FilterString"]) + " ";
						}
					}
					else
					{
						pivotGridField.Caption = dt.Columns[i].ColumnName;
						pivotGridField.Width = dfWidth;
						pivotGridField.Visible = dfVisible;
						pivotGridField.Options.ReadOnly = dfReadOnly;
						pivotGridField.Options.AllowEdit = dfAllowEdit;
						pivotGridField.Options.AllowDrag = dfAllowMove;
						pivotGridField.Options.AllowSort = dfAllowSort;
						if (dfVisible)
						{
							pivotGridField.AreaIndex = idx;
							idx++;
						}
						else
						{
							pivotGridField.AreaIndex = -1;
						}
					}
					await LoadUdfColumns(pivot, pivotGridField, lstUDF, dataRow, dtValidValue, dt, tableName);
				}
				pivot.DataSource = dt;
				pivot.RefreshData();
				if (dtStructure.IsExistsColumn("VisIndex"))
				{
					foreach (DataRow row in dtStructure.Rows)
					{
						string fieldName = Function.ToString(row["ColumnName"]);
						if (pivot.Fields[fieldName].Visible)
						{
							int areaIndex = Function.ParseInt(row["VisIndex"]);
							pivot.Fields[fieldName].AreaIndex = areaIndex;
						}
					}
				}
				pivot.ActiveFilterString = filterString;
			}
			if (expand)
			{
				pivot.ExpandAll();
			}
			if (pivot.FormatRules.Count == 0)
			{
				await ExtensionMethods.ReloadFormatRulePivot(pivot, pivot.FormId);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static async Task GenerateColumn(PivotGridControlEx pivot, DataTable dtStructure, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, string tableName = "", Dictionary<string, GridViewEx> tblFormular = null)
	{
		int idx = 0;
		string filterString = "";
		DataTable dt = new DataTable();
		foreach (DataRow row in dtStructure.Rows)
		{
			dt.Columns.Add(Function.ToString(row["ColumnName"]), row.IsExistsColumn("DataType") ? GetColumnType(Function.ToString(row["DataType"])) : typeof(string));
		}
		foreach (DataRow row2 in dtStructure.Rows)
		{
			PivotGridField pivotGridField = pivot.Fields.Add();
			pivotGridField.FieldName = (row2.IsExistsColumn("ColumnName") ? Function.ToString(row2["ColumnName"]) : "");
			pivotGridField.Name = $"cl_{pivotGridField.FieldName}";
			pivotGridField.Caption = (row2.IsExistsColumn("Caption") ? Function.ToString(row2["Caption"]) : pivotGridField.FieldName);
			pivotGridField.Width = (row2.IsExistsColumn("Width") ? Function.ParseInt(row2["Width"]) : dfWidth);
			if (row2.IsExistsColumn("PivotArea") && !string.IsNullOrEmpty(Function.ToString(row2["PivotArea"])))
			{
				switch (Function.ToString(row2["PivotArea"]).ToLower())
				{
				case "data":
					pivotGridField.Area = PivotArea.DataArea;
					break;
				case "column":
					pivotGridField.Area = PivotArea.ColumnArea;
					break;
				case "filter":
					pivotGridField.Area = PivotArea.FilterArea;
					break;
				case "row":
					pivotGridField.Area = PivotArea.RowArea;
					break;
				default:
					pivotGridField.Area = PivotArea.RowArea;
					break;
				}
			}
			if (row2.IsExistsColumn("Visible"))
			{
				pivotGridField.Visible = Function.ParseBool(row2["Visible"]);
			}
			else
			{
				pivotGridField.Visible = dfVisible;
			}
			if (!pivotGridField.Visible)
			{
				pivotGridField.Options.ShowInCustomizationForm = true;
			}
			pivotGridField.AreaIndex = (pivotGridField.Visible ? idx : (-1));
			idx = (pivotGridField.Visible ? (idx + 1) : idx);
			pivotGridField.Options.ReadOnly = (row2.IsExistsColumn("ReadOnly") ? Function.ParseBool(row2["ReadOnly"]) : dfReadOnly);
			pivotGridField.Options.AllowEdit = (row2.IsExistsColumn("AllowEdit") ? Function.ParseBool(row2["AllowEdit"]) : dfAllowEdit);
			pivotGridField.Options.AllowDrag = ((!Function.ParseBool(row2["AllowMove"])) ? DefaultBoolean.False : DefaultBoolean.True);
			pivotGridField.Options.AllowSort = ((!row2.IsExistsColumn("AllowSort") || !Function.ParseBool(row2["AllowSort"])) ? dfAllowSort : DefaultBoolean.True);
			if (row2.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(row2["FormatType"])))
			{
				string text = Function.ToString(row2["FormatType"]).ToLower();
				if (!(text == "numeric"))
				{
					if (text == "datetime")
					{
						pivotGridField.ValueFormat.FormatType = FormatType.DateTime;
					}
					else
					{
						pivotGridField.ValueFormat.FormatType = FormatType.None;
					}
				}
				else
				{
					pivotGridField.ValueFormat.FormatType = FormatType.Numeric;
				}
			}
			if (row2.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(row2["DisplayFormat"])))
			{
				if (Function.ParseInt(row2["DisplayFormat"]) > 0)
				{
					DisplayFormat(pivotGridField, Function.ParseInt(row2["DisplayFormat"]));
				}
				else
				{
					pivotGridField.ValueFormat.FormatString = Function.ToString(row2["DisplayFormat"]);
					pivotGridField.CellFormat.FormatString = Function.ToString(row2["DisplayFormat"]);
				}
			}
			string format = pivotGridField.ValueFormat.FormatString;
			if (row2.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(row2["SummaryFormat"])))
			{
				format = Function.ToString(row2["SummaryFormat"]);
			}
			if (row2.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(row2["SummaryType"])))
			{
				GetSummary(pivotGridField, Function.ToString(row2["SummaryType"]).ToLower(), format);
			}
			if (row2.IsExistsColumn("GroupSummaryType") && !string.IsNullOrEmpty(Function.ToString(row2["GroupSummaryType"])) && row2.IsExistsColumn("GroupSummaryShowInFooter"))
			{
				Function.ParseBool(row2["GroupSummaryShowInFooter"]);
			}
			if (row2.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(row2["Expression"])))
			{
				try
				{
					dt.Columns[pivotGridField.FieldName].Expression = Function.ToString(row2["Expression"]);
				}
				catch (Exception ex)
				{
					WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
				}
			}
			if (row2.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(row2["BackColor"])))
			{
				pivotGridField.Appearance.Cell.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
				pivotGridField.Appearance.Header.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
				pivotGridField.Appearance.CellTotal.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
				pivotGridField.Appearance.CellGrandTotal.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
			}
			if (row2.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(row2["ForeColor"])))
			{
				pivotGridField.Appearance.Cell.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
				pivotGridField.Appearance.Header.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
				pivotGridField.Appearance.CellTotal.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
				pivotGridField.Appearance.CellGrandTotal.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
			}
			if (row2.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(row2["FilterString"])))
			{
				filterString = filterString + Function.ToString(row2["FilterString"]) + " ";
			}
			await LoadUdfColumns(pivot, pivotGridField, lstUDF, row2, dtValidValue, dt, tableName);
		}
		pivot.ActiveFilterString = filterString;
		pivot.DataSource = dt;
		pivot.RefreshData();
	}

	private static void DisplayFormat(PivotGridField col, int value)
	{
		col.Tag = value;
		switch ((Enums.DisplayFormatType)value)
		{
		case Enums.DisplayFormatType.DateTime:
			col.ValueFormat.FormatType = FormatType.DateTime;
			col.ValueFormat.FormatString = GlobalData.DateTimeFormat;
			col.CellFormat.FormatType = FormatType.DateTime;
			col.CellFormat.FormatString = GlobalData.DateTimeFormat;
			break;
		case Enums.DisplayFormatType.Percent:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)} %";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)} %";
			break;
		case Enums.DisplayFormatType.Qty:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Rate:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Unit:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Amount:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec ?? 6)}";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Price:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec ?? 6)}";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Time:
			col.ValueFormat.FormatString = "##:##";
			col.ValueFormat.FormatType = FormatType.Custom;
			col.CellFormat.FormatString = "##:##";
			col.CellFormat.FormatType = FormatType.Custom;
			break;
		case Enums.DisplayFormatType.Integer:
			col.ValueFormat.FormatType = FormatType.Numeric;
			col.ValueFormat.FormatString = "n0";
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = "n0";
			break;
		}
	}

	public static Type GetColumnType(string sqlType)
	{
		return sqlType.ToLower() switch
		{
			"int" => typeof(int), 
			"smallint" => typeof(short), 
			"numeric" => typeof(decimal), 
			"decimal" => typeof(decimal), 
			"money" => typeof(decimal), 
			"float" => typeof(decimal), 
			"datetime" => typeof(DateTime), 
			"date" => typeof(DateTime), 
			"bit" => typeof(bool), 
			_ => typeof(string), 
		};
	}

	public static void GetSummary(PivotGridField col, string summaryType, string format)
	{
		col.Options.ShowCustomTotals = true;
		col.Options.ShowGrandTotal = true;
		col.Options.ShowTotals = true;
		col.TotalsVisibility = PivotTotalsVisibility.CustomTotals;
		col.TotalValueFormat.FormatString = format;
		switch (summaryType)
		{
		case "sum":
			col.SummaryType = PivotSummaryType.Sum;
			col.CustomTotals.Add(PivotSummaryType.Sum);
			break;
		case "average":
			col.SummaryType = PivotSummaryType.Average;
			col.CustomTotals.Add(PivotSummaryType.Average);
			break;
		case "min":
			col.SummaryType = PivotSummaryType.Min;
			col.CustomTotals.Add(PivotSummaryType.Min);
			break;
		case "max":
			col.SummaryType = PivotSummaryType.Max;
			col.CustomTotals.Add(PivotSummaryType.Max);
			break;
		case "count":
			col.SummaryType = PivotSummaryType.Count;
			col.CustomTotals.Add(PivotSummaryType.Count);
			break;
		default:
			col.SummaryType = PivotSummaryType.Sum;
			col.CustomTotals.Add(PivotSummaryType.Sum);
			break;
		}
	}

	public static async Task LoadUdfColumns(PivotGridControlEx pivot, PivotGridField col, List<UserFieldsImpl> lstUDF, DataRow dr, DataTable dtValidValue, DataTable data, string tableName = "", Dictionary<string, PivotGridField> tblFormular = null)
	{
		_ = 1;
		try
		{
			string udfName = ((!string.IsNullOrEmpty(col.FieldName) && col.FieldName.StartsWith("U_")) ? col.FieldName.Substring(2, col.FieldName.Length - 2).ToLower() : "");
			UserFieldsImpl udf = ((lstUDF != null && lstUDF.Count > 0) ? lstUDF.Where((UserFieldsImpl o) => o.Name.ToLower().Equals(udfName)).FirstOrDefault() : null);
			if (udf == null)
			{
				await LoadRepository(pivot, col, dr, dtValidValue, data);
				return;
			}
			switch (udf.Type)
			{
			case BoFieldTypes.db_Alpha:
				if (!string.IsNullOrEmpty(udf.LinkedTable))
				{
					List<Common.Table_Code> list = await HttpClientHelperFactory.GetHttpClient().Udf.GetLinkedTable(udf.LinkedTable, GlobalData.UserName, GlobalData.DefaultBranch);
					list.Insert(0, new Common.Table_Code("", ""));
					RepositoryItemLookUpEdit fieldEdit = new RepositoryItemLookUpEdit
					{
						DataSource = list,
						ValueMember = "Code",
						DisplayMember = "Name",
						NullText = "",
						BestFitMode = BestFitMode.BestFitResizePopup
					};
					col.FieldEdit = fieldEdit;
				}
				break;
			case BoFieldTypes.db_Memo:
				if (udf.SubType == BoFldSubTypes.st_Link)
				{
					RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
					repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
					col.FieldEdit = repositoryItemAttachFile;
				}
				else
				{
					RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
					repositoryItemMemoExEdit.NullText = "";
					repositoryItemMemoExEdit.ShowIcon = false;
					repositoryItemMemoExEdit.BestFitWidth = col.Width;
					col.FieldEdit = repositoryItemMemoExEdit;
				}
				break;
			case BoFieldTypes.db_Date:
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.UnboundType = UnboundColumnType.String;
					RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
					repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
					col.FieldEdit = repositoryItemTextEdit2;
				}
				else
				{
					col.UnboundType = UnboundColumnType.DateTime;
					RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
					repositoryItemDateEditEx.AllowMouseWheel = false;
					col.FieldEdit = repositoryItemDateEditEx;
				}
				break;
			case BoFieldTypes.db_Float:
			{
				RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.AllowMouseWheel = false;
				col.CellFormat.FormatType = FormatType.Numeric;
				col.ValueFormat.FormatType = FormatType.Numeric;
				col.FieldEdit = repositoryItemCalcEdit2;
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.PercentageRateFormat;
					col.CellFormat.Format = GlobalData.PercentageRateFormat;
					col.ValueFormat.Format = GlobalData.PercentageRateFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Quantity)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.QuantitiesFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.QuantitiesFormat;
					col.CellFormat.Format = GlobalData.QuantitiesFormat;
					col.ValueFormat.Format = GlobalData.QuantitiesFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.RateAccuracyFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.RateAccuracyFormat;
					col.CellFormat.Format = GlobalData.RateAccuracyFormat;
					col.ValueFormat.Format = GlobalData.RateAccuracyFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Sum)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.TotalsAccuracyFormat;
					col.CellFormat.Format = GlobalData.TotalsAccuracyFormat;
					col.ValueFormat.Format = GlobalData.TotalsAccuracyFormat;
				}
				else
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.UnitPriceFormat;
					col.CellFormat.Format = GlobalData.UnitPriceFormat;
					col.ValueFormat.Format = GlobalData.UnitPriceFormat;
				}
				break;
			}
			case BoFieldTypes.db_Numeric:
			{
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
					col.FieldEdit = repositoryItemTextEdit;
					break;
				}
				RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				repositoryItemCalcEdit.EditFormat.Format = GlobalData.UnitPriceFormat;
				repositoryItemCalcEdit.AllowMouseWheel = false;
				col.CellFormat.FormatType = FormatType.Numeric;
				col.CellFormat.Format = GlobalData.UnitPriceFormat;
				col.FieldEdit = repositoryItemCalcEdit;
				break;
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
			col.FieldEdit = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task LoadRepository(PivotGridControlEx view, PivotGridField col, DataRow dr, DataTable dtValidValue, DataTable data)
	{
		if (dr == null || !dr.IsExistsColumn("DataType"))
		{
			return;
		}
		switch (Function.ToString(dr["DataType"]).ToLower())
		{
		case "alpha":
		case "memo":
		{
			RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
			repositoryItemMemoExEdit.NullText = "";
			repositoryItemMemoExEdit.ShowIcon = false;
			repositoryItemMemoExEdit.BestFitWidth = col.Width;
			col.FieldEdit = repositoryItemMemoExEdit;
			break;
		}
		case "nvarchar":
			if (col.Area == PivotArea.DataArea)
			{
				col.SummaryType = PivotSummaryType.Min;
			}
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
				repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
				col.FieldEdit = repositoryItemTextEdit2;
			}
			break;
		case "int":
		{
			RepositoryItemCalcEdit repositoryItemCalcEdit4 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit4.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit4.DisplayFormat.FormatString = "d";
			repositoryItemCalcEdit4.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit4.EditFormat.FormatString = "d";
			repositoryItemCalcEdit4.Mask.MaskType = MaskType.Numeric;
			repositoryItemCalcEdit4.Mask.EditMask = "d";
			repositoryItemCalcEdit4.AllowMouseWheel = false;
			repositoryItemCalcEdit4.Buttons.Clear();
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.FormatString = "d";
			col.FieldEdit = repositoryItemCalcEdit4;
			break;
		}
		case "decimal":
		case "money":
		{
			RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.QuantitiesFormat;
			repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.EditFormat.Format = GlobalData.QuantitiesFormat;
			repositoryItemCalcEdit2.AllowMouseWheel = false;
			col.CellFormat.FormatType = FormatType.Numeric;
			col.CellFormat.Format = GlobalData.QuantitiesFormat;
			col.FieldEdit = repositoryItemCalcEdit2;
			break;
		}
		case "numeric":
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
				col.FieldEdit = repositoryItemTextEdit;
			}
			else
			{
				RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
				col.CellFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.AllowMouseWheel = false;
				repositoryItemCalcEdit.DisplayFormat.FormatString = col.CellFormat.FormatString;
				repositoryItemCalcEdit.EditFormat.FormatString = col.CellFormat.FormatString;
				col.FieldEdit = repositoryItemCalcEdit;
			}
			break;
		case "datetime":
		case "date":
		{
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
				repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.DisplayFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.EditFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.EditFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.AllowMouseWheel = false;
				col.CellFormat.FormatType = FormatType.DateTime;
				col.CellFormat.FormatString = "HH:mm";
				col.FieldEdit = repositoryItemDateEdit;
				break;
			}
			if (col.Area == PivotArea.DataArea)
			{
				col.SummaryType = PivotSummaryType.Min;
			}
			string dateTimeFormat = GlobalData.DateTimeFormat;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])))
			{
				if (Function.ParseInt(dr["DisplayFormat"]) > 0)
				{
					DisplayFormat(col, Function.ParseInt(dr["DisplayFormat"]));
				}
				else
				{
					col.ValueFormat.FormatString = Function.ToString(dr["DisplayFormat"]);
					col.CellFormat.FormatString = Function.ToString(dr["DisplayFormat"]);
				}
			}
			RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
			repositoryItemDateEditEx.AllowMouseWheel = false;
			col.CellFormat.FormatType = FormatType.DateTime;
			col.CellFormat.FormatString = dateTimeFormat;
			col.FieldEdit = repositoryItemDateEditEx;
			break;
		}
		case "float":
		{
			RepositoryItemCalcEdit repositoryItemCalcEdit3 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit3.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit3.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit3.AllowMouseWheel = false;
			col.CellFormat.FormatType = FormatType.Numeric;
			col.FieldEdit = repositoryItemCalcEdit3;
			if (dr.IsExistsColumn("SubType"))
			{
				if ("percentage".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit3.DisplayFormat.Format = GlobalData.PercentageRateFormat;
					col.CellFormat.Format = GlobalData.PercentageRateFormat;
				}
				else if ("quantity".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit3.DisplayFormat.Format = GlobalData.QuantitiesFormat;
					repositoryItemCalcEdit3.EditFormat.Format = GlobalData.QuantitiesFormat;
					col.CellFormat.Format = GlobalData.QuantitiesFormat;
				}
				else if ("rate".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit3.DisplayFormat.Format = GlobalData.RateAccuracyFormat;
					repositoryItemCalcEdit3.EditFormat.Format = GlobalData.RateAccuracyFormat;
					col.CellFormat.Format = GlobalData.RateAccuracyFormat;
				}
				else if ("sum".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit3.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
					repositoryItemCalcEdit3.EditFormat.Format = GlobalData.TotalsAccuracyFormat;
					col.CellFormat.Format = GlobalData.TotalsAccuracyFormat;
				}
				else if ("price".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit3.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit3.EditFormat.Format = GlobalData.UnitPriceFormat;
					col.CellFormat.Format = GlobalData.UnitPriceFormat;
				}
			}
			break;
		}
		case "bit":
		{
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit.AutoHeight = false;
			col.FieldEdit = repositoryItemCheckEdit;
			data.ChangeColumnDataType(col.FieldName, typeof(bool));
			break;
		}
		case "link":
		{
			RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
			repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
			col.FieldEdit = repositoryItemAttachFile;
			break;
		}
		case "validvalue":
		{
			string text = Function.ToString(dr["SubType"]);
			if (!string.IsNullOrEmpty(text))
			{
				DataTable data2 = (await HttpClientHelperFactory.GetHttpClient().Commons.ExecuteQuery(text)).Data;
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
				repositoryItemLookUpEdit.DataSource = data2;
				if (data2 != null && data2.Columns.Count >= 2)
				{
					repositoryItemLookUpEdit.ValueMember = data2.Columns[0].ColumnName;
					repositoryItemLookUpEdit.DisplayMember = data2.Columns[1].ColumnName;
				}
				repositoryItemLookUpEdit.NullText = string.Empty;
				repositoryItemLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;
				col.FieldEdit = repositoryItemLookUpEdit;
			}
			else
			{
				if (!dtValidValue.IsNotNull() || !dtValidValue.Rows[0].IsExistsColumn("ColumnName"))
				{
					break;
				}
				DataRow[] array = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
				if (!array.IsNotNull())
				{
					break;
				}
				DataTable dataSource2 = array.CopyToDataTable();
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource2,
					ValueMember = "Value",
					DisplayMember = "Description",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				repositoryItemLookUpEdit2.PopulateColumns();
				for (int num = 0; num < repositoryItemLookUpEdit2.Columns.Count; num++)
				{
					if (!repositoryItemLookUpEdit2.Columns[num].FieldName.Equals("Description"))
					{
						repositoryItemLookUpEdit2.Columns[num].Visible = false;
					}
				}
				col.FieldEdit = repositoryItemLookUpEdit2;
			}
			break;
		}
		case "linkedtable":
			if (!string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				List<Common.Table_Code> dataSource = await HttpClientHelperFactory.GetHttpClient().Udf.GetLinkedTable(Function.ToString(dr["SubType"]), GlobalData.UserName, GlobalData.DefaultBranch);
				RepositoryItemLookUpEdit fieldEdit = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource,
					ValueMember = "Code",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				col.FieldEdit = fieldEdit;
			}
			break;
		case "sub":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.DisplayFormat.FormatType = col.CellFormat.FormatType;
			repositoryItemButtonEdit.DisplayFormat.FormatString = col.CellFormat.FormatString;
			repositoryItemButtonEdit.EditFormat.FormatType = col.CellFormat.FormatType;
			repositoryItemButtonEdit.EditFormat.FormatString = col.CellFormat.FormatString;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowSubForm(view, dr);
			};
			repositoryItemButtonEdit.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowSubForm(view, dr);
				}
			};
			col.FieldEdit = repositoryItemButtonEdit;
			break;
		}
		default:
			if (col.Area == PivotArea.DataArea)
			{
				col.SummaryType = PivotSummaryType.Min;
			}
			break;
		}
	}

	private static void ShowSubForm(PivotGridControlEx view, DataRow dr)
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
				text = array[0].Trim();
				string[] array2 = array[1].Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Substring(1).Trim();
					if (text2.ToLower().Contains("username"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.UserName));
					}
					else if (text2.ToLower().Contains("branch"))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), GlobalData.DefaultBranch));
					}
					else if (array2[i].Contains("="))
					{
						int num = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1).Replace("'", "")));
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
			frm.MdiParent = view.FindForm().MdiParent;
			frm.WindowState = FormWindowState.Normal;
			frm.StartPosition = FormStartPosition.CenterScreen;
			frm.Show();
			view.FindForm().FormClosing += (object s, FormClosingEventArgs e) =>
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
}
