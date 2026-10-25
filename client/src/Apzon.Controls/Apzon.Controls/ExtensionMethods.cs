using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PM;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Controls;

public static class ExtensionMethods
{
	private static readonly string[] listNotDisable = new string[5] { "ctm_CopyCell", "ctm_CopyTable", "ctm_CopyRows", "ctm_SaveFormsetting", "ctm_ReloadFormsetting" };

	public static async Task SaveFormSetting(this GridViewEx gv, string formId = "", string itemId = "")
	{
		List<FormSetting> lst = new List<FormSetting>();
		DataTable dataTable = gv.GridControl.DataSource as DataTable;
		Dictionary<string, Tuple<bool, bool>> dictionary = new Dictionary<string, Tuple<bool, bool>>();
		if (GlobalData.FormSettings.ContainsKey(gv.FormId) && GlobalData.FormSettings[gv.FormId].Tables.Contains(itemId))
		{
			dictionary = (from r in GlobalData.FormSettings[gv.FormId].Tables[itemId].AsEnumerable()
				where Function.ParseBool(r["Visible"])
				select r).ToDictionary((DataRow r) => Function.ToString(r["ColumnId"]), (DataRow r) => new Tuple<bool, bool>(Function.ParseBool(r["Editable"]), Function.ParseBool(r["IsReadOnly"])));
		}
		foreach (GridColumnEx col in gv.Columns.ToList())
		{
			if (((gv.ItemId.Equals("APZ_FCT1") || gv.ItemId.Equals("PFCT1")) && col.Name.Length > 1 && (col.Name.StartsWith("P") || col.Name.StartsWith("Q")) && Function.ParseInt(col.Name.Substring(1)) > 0) || lst.Exists((FormSetting t) => t.ColumnId.Equals(col.FieldName)))
			{
				continue;
			}
			FormSetting formSetting = new FormSetting();
			formSetting.FormId = ((!string.IsNullOrEmpty(formId)) ? formId : gv.FormId);
			formSetting.ItemId = ((!string.IsNullOrEmpty(itemId)) ? itemId : gv.ItemId);
			formSetting.ColumnId = Function.ToString(col.FieldName);
			if (col.FieldName.StartsWith("U_") && dataTable != null && !dataTable.Columns.Contains(col.FieldName))
			{
				formSetting.UserSign = "-";
				gv.Columns.Remove(col);
				continue;
			}
			formSetting.UserSign = GlobalData.UserName;
			formSetting.ColumnName = Function.ToString(col.Caption);
			FormSetting formSetting2 = formSetting;
			string text;
			if (col.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left)
			{
				text = "L";
			}
			else
			{
				text = ((col.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Right) ? "R" : "N");
			}
			formSetting2.Fixed = text;
			formSetting.Width = Function.ParseInt(col.Width);
			formSetting.Visible = Function.ParseBool(col.Visible);
			formSetting.VisIndex = Function.ParseInt(col.VisibleIndex);
			if (gv.LstColumnNotEditable != null && gv.LstColumnNotEditable.Contains(col.FieldName))
			{
				formSetting.Editable = false;
				formSetting.NeverEditable = true;
			}
			else
			{
				formSetting.Editable = col.Visible && (dictionary.ContainsKey(col.FieldName) ? dictionary[col.FieldName].Item1 : (!col.OptionsColumn.ReadOnly));
				formSetting.NeverEditable = false;
			}
			formSetting.IsReadOnly = (dictionary.ContainsKey(col.FieldName) ? dictionary[col.FieldName].Item2 : col.OptionsColumn.ReadOnly);
			formSetting.VisSetting = true;
			formSetting.AllowChSet = true;
			formSetting.UserUpdate = GlobalData.UserID;
			formSetting.UpdateDate = DateTime.Today;
			formSetting.LngCode = "-";
			formSetting.GroupIndex = col.GroupIndex;
			lst.Add(formSetting);
		}
		if (lst.Count <= 0)
		{
			return;
		}
		using HttpClientHelper<FormSetting> helper = new HttpClientHelper<FormSetting>(Configuration.ApiServer, "FormSetting");
		string text2 = await helper.PostParamsAsJsonAsync("SaveData", lst);
		if (!string.IsNullOrEmpty(text2))
		{
			WinCommons.Error(Enums.MessageCode.None, text2);
			return;
		}
		if (GlobalData.FormSettings.ContainsKey(lst[0].FormId) && GlobalData.FormSettings[lst[0].FormId].Tables.Contains(lst[0].ItemId))
		{
			GlobalData.FormSettings[lst[0].FormId].Tables.Remove(lst[0].ItemId);
		}
		WinCommons.Success(Enums.MessageCode.None, "Save form setting success");
	}

	public static async Task SaveFormSetting(this TreeEx gv)
	{
		try
		{
			List<FormSetting> lst = new List<FormSetting>();
			foreach (TreeListColumnEx col in from treeListColumn in gv.Columns.AsEnumerable()
				where treeListColumn.GetType() == typeof(TreeListColumnEx)
				select treeListColumn)
			{
				if (((!gv.ItemId.Equals("APZ_FCT1") && !gv.ItemId.Equals("PFCT1")) || col.Name.Length <= 1 || (!col.Name.StartsWith("P") && !col.Name.StartsWith("Q")) || Function.ParseInt(col.Name.Substring(1)) <= 0) && !lst.Exists((FormSetting t) => t.ColumnId.Equals(col.FieldName)))
				{
					FormSetting formSetting = new FormSetting();
					formSetting.FormId = gv.FormId;
					formSetting.ItemId = gv.ItemId;
					formSetting.ColumnId = Function.ToString(col.FieldName);
					formSetting.ColumnName = Function.ToString(col.Caption);
					formSetting.UserSign = GlobalData.UserName;
					formSetting.Width = Function.ParseInt(col.Width);
					formSetting.Visible = Function.ParseBool(col.Visible);
					formSetting.VisIndex = Function.ParseInt(col.VisibleIndex);
					FormSetting formSetting2 = formSetting;
					string text;
					if (col.Fixed == DevExpress.XtraTreeList.Columns.FixedStyle.Left)
					{
						text = "L";
					}
					else
					{
						text = ((col.Fixed == DevExpress.XtraTreeList.Columns.FixedStyle.Right) ? "R" : "N");
					}
					formSetting2.Fixed = text;
					formSetting.Editable = col.OptionsColumn.AllowEdit;
					formSetting.IsReadOnly = col.OptionsColumn.ReadOnly;
					formSetting.VisSetting = true;
					formSetting.AllowChSet = true;
					formSetting.UserUpdate = GlobalData.UserID;
					formSetting.UpdateDate = DateTime.Today;
					formSetting.LngCode = "-";
					formSetting.GroupIndex = -1;
					lst.Add(formSetting);
				}
			}
			if (lst.Count <= 0)
			{
				return;
			}
			using HttpClientHelper<FormSetting> helper = new HttpClientHelper<FormSetting>(Configuration.ApiServer, "FormSetting");
			string text2 = await helper.PostParamsAsJsonAsync("SaveData", lst);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
				return;
			}
			if (GlobalData.FormSettings.ContainsKey(lst[0].FormId) && GlobalData.FormSettings[lst[0].FormId].Tables.Contains(lst[0].ItemId))
			{
				GlobalData.FormSettings[lst[0].FormId].Tables.Remove(lst[0].ItemId);
			}
			WinCommons.Success(Enums.MessageCode.None, "Save Formsetting success");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static EditorContainer GetActiveGridControl(this Control control)
	{
		try
		{
			if (control != null)
			{
				if (control is XtraForm)
				{
					return null;
				}
				if (control.GetType() == typeof(GridControlEx))
				{
					return control as GridControlEx;
				}
				if (control.GetType() == typeof(TreeEx))
				{
					return control as TreeEx;
				}
				if (control.GetType() == typeof(PivotGridControlEx))
				{
					return control as PivotGridControlEx;
				}
				if (control is ContainerControl)
				{
					EditorContainer activeGridControlFromChild = GetActiveGridControlFromChild((control as ContainerControl).ActiveControl);
					if (activeGridControlFromChild != null)
					{
						return activeGridControlFromChild;
					}
				}
				return control.Parent.GetActiveGridControlFromFather();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	private static EditorContainer GetActiveGridControlFromFather(this Control control)
	{
		try
		{
			if (control != null)
			{
				if (control is XtraForm)
				{
					return null;
				}
				if (control.GetType() == typeof(GridControlEx))
				{
					return control as GridControlEx;
				}
				if (control.GetType() == typeof(TreeEx))
				{
					return control as TreeEx;
				}
				if (control.GetType() == typeof(PivotGridControlEx))
				{
					return control as PivotGridControlEx;
				}
				return control.Parent.GetActiveGridControlFromFather();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	private static EditorContainer GetActiveGridControlFromChild(Control control)
	{
		if (control != null)
		{
			if (control is XtraForm)
			{
				return null;
			}
			if (control.GetType() == typeof(GridControlEx))
			{
				return control as GridControlEx;
			}
			if (control.GetType() == typeof(TreeEx))
			{
				return control as TreeEx;
			}
			if (control.GetType() == typeof(PivotGridControlEx))
			{
				return control as PivotGridControlEx;
			}
			if (control is ContainerControl)
			{
				return GetActiveGridControlFromChild((control as ContainerControl).ActiveControl);
			}
			return control.Parent.GetActiveGridControlFromFather();
		}
		return null;
	}

	public static GridViewEx GetActiveGridView(this XtraFormEx form)
	{
		if (form == null)
		{
			return null;
		}
		if (form.ActiveControl is GridControlEx && form.ActiveControl is GridControlEx gridControlEx)
		{
			return gridControlEx.DefaultView as GridViewEx;
		}
		return null;
	}

	public static TreeEx GetActiveTreeView(this XtraFormEx form)
	{
		if (form == null)
		{
			return null;
		}
		if (form.ActiveControl is TreeEx)
		{
			return form.ActiveControl as TreeEx;
		}
		return null;
	}

	public static PivotGridControlEx GetActivePivot(this XtraFormEx form)
	{
		if (form == null)
		{
			return null;
		}
		if (form.ActiveControl is PivotGridControlEx)
		{
			return form.ActiveControl as PivotGridControlEx;
		}
		return null;
	}

	public static async Task ReloadFormatRuleTree(TreeEx tree, string formId)
	{
		try
		{
			if (tree == null)
			{
				return;
			}
			if (tree == null)
			{
				return;
			}
			tree.FormatRules.Clear();
			DataTable dataTable = await GetFormatRule(formId, tree.ItemId);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				FormatRule setting = new FormatRule
				{
					FormId = Function.ToString(dataTable.Rows[i]["FormId"]),
					ItemId = Function.ToString(dataTable.Rows[i]["ItemId"]),
					ColumnId = Function.ToString(dataTable.Rows[i]["ColumnId"]),
					UserCode = Function.ToString(dataTable.Rows[i]["UserCode"]),
					ApplyToRow = Function.ParseBool(dataTable.Rows[i]["ApplyToRow"]),
					Expression = Function.ToString(dataTable.Rows[i]["Expression"]),
					BackColor = Function.ToString(dataTable.Rows[i]["BackColor"]),
					BackColor2 = Function.ToString(dataTable.Rows[i]["BackColor2"]),
					BorderColor = Function.ToString(dataTable.Rows[i]["BorderColor"]),
					ForeColor = Function.ToString(dataTable.Rows[i]["ForeColor"]),
					GradientMode = Function.ParseInt(dataTable.Rows[i]["GradientMode"]),
					FontSizeDelta = Function.ParseInt(dataTable.Rows[i]["FontSizeDelta"]),
					FontStyleDelta = Function.ParseInt(dataTable.Rows[i]["FontSizeDelta"]),
					Image = Function.ToString(dataTable.Rows[i]["Image"]),
					Font = Function.ToString(dataTable.Rows[i]["Font"]),
					Bold = Function.ParseBool(dataTable.Rows[i]["Bold"]),
					Italic = Function.ParseBool(dataTable.Rows[i]["Italic"]),
					Strikeout = Function.ParseBool(dataTable.Rows[i]["Strikeout"]),
					Size = Function.ParseFloat(dataTable.Rows[i]["Size"]),
					UnderLine = Function.ParseBool(dataTable.Rows[i]["UnderLine"]),
					GdiCharSet = Function.ParseByte(dataTable.Rows[i]["GdiCharSet"]),
					GdiVerticalFont = Function.ParseBool(dataTable.Rows[i]["GdiVerticalFont"]),
					Unit = Function.ParseInt(dataTable.Rows[i]["Unit"]),
					HAlignment = Function.ParseInt(dataTable.Rows[i]["HAlignment"]),
					HotkeyPrefix = Function.ParseInt(dataTable.Rows[i]["HotkeyPrefix"]),
					Trimming = Function.ParseInt(dataTable.Rows[i]["Trimming"]),
					VAlignment = Function.ParseInt(dataTable.Rows[i]["VAlignment"]),
					WordWrap = Function.ParseInt(dataTable.Rows[i]["WordWrap"])
				};
				TreeListFormatRule treeListFormatRule = new TreeListFormatRule();
				if (tree.Columns.Where((TreeListColumn t) => t.FieldName.Equals(setting.ColumnId)).ToList().Count > 0)
				{
					treeListFormatRule.Column = tree.Columns[setting.ColumnId];
				}
				treeListFormatRule.ApplyToRow = setting.ApplyToRow;
				FormatConditionRuleExpression rule = new FormatConditionRuleExpression();
				FormatCondition(rule, setting);
				treeListFormatRule.Rule = rule;
				tree.FormatRules.Add(treeListFormatRule);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task ReloadFormatRuleGrid(GridViewEx grid, string formId)
	{
		try
		{
			if (grid == null)
			{
				return;
			}
			if (grid == null || grid.Columns.Count == 0 || string.IsNullOrEmpty(formId))
			{
				return;
			}
			if (!grid.FormatRules.Where((GridFormatRule t) => t.Rule is FormatConditionRuleIconSet).Any())
			{
				grid.FormatRules.Clear();
			}
			DataTable dataTable = await GetFormatRule(formId, grid.ItemId);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				FormatRule setting = new FormatRule
				{
					FormId = Function.ToString(dataTable.Rows[num]["FormId"]),
					ItemId = Function.ToString(dataTable.Rows[num]["ItemId"]),
					ColumnId = Function.ToString(dataTable.Rows[num]["ColumnId"]),
					UserCode = Function.ToString(dataTable.Rows[num]["UserCode"]),
					ApplyToRow = Function.ParseBool(dataTable.Rows[num]["ApplyToRow"]),
					Expression = Function.ToString(dataTable.Rows[num]["Expression"]),
					BackColor = Function.ToString(dataTable.Rows[num]["BackColor"]),
					BackColor2 = Function.ToString(dataTable.Rows[num]["BackColor2"]),
					BorderColor = Function.ToString(dataTable.Rows[num]["BorderColor"]),
					ForeColor = Function.ToString(dataTable.Rows[num]["ForeColor"]),
					GradientMode = Function.ParseInt(dataTable.Rows[num]["GradientMode"]),
					FontSizeDelta = Function.ParseInt(dataTable.Rows[num]["FontSizeDelta"]),
					FontStyleDelta = Function.ParseInt(dataTable.Rows[num]["FontSizeDelta"]),
					Image = Function.ToString(dataTable.Rows[num]["Image"]),
					Font = Function.ToString(dataTable.Rows[num]["Font"]),
					Bold = Function.ParseBool(dataTable.Rows[num]["Bold"]),
					Italic = Function.ParseBool(dataTable.Rows[num]["Italic"]),
					Strikeout = Function.ParseBool(dataTable.Rows[num]["Strikeout"]),
					Size = Function.ParseFloat(dataTable.Rows[num]["Size"]),
					UnderLine = Function.ParseBool(dataTable.Rows[num]["UnderLine"]),
					GdiCharSet = Function.ParseByte(dataTable.Rows[num]["GdiCharSet"]),
					GdiVerticalFont = Function.ParseBool(dataTable.Rows[num]["GdiVerticalFont"]),
					Unit = Function.ParseInt(dataTable.Rows[num]["Unit"]),
					HAlignment = Function.ParseInt(dataTable.Rows[num]["HAlignment"]),
					HotkeyPrefix = Function.ParseInt(dataTable.Rows[num]["HotkeyPrefix"]),
					Trimming = Function.ParseInt(dataTable.Rows[num]["Trimming"]),
					VAlignment = Function.ParseInt(dataTable.Rows[num]["VAlignment"]),
					WordWrap = Function.ParseInt(dataTable.Rows[num]["WordWrap"])
				};
				GridFormatRule gridFormatRule = new GridFormatRule();
				if (grid.Columns.Where((GridColumn t) => t.FieldName.Equals(setting.ColumnId)).ToList().Count > 0)
				{
					gridFormatRule.Column = grid.Columns[setting.ColumnId];
				}
				gridFormatRule.ApplyToRow = setting.ApplyToRow;
				FormatConditionRuleExpression rule = new FormatConditionRuleExpression();
				FormatCondition(rule, setting);
				gridFormatRule.Rule = rule;
				grid.FormatRules.Add(gridFormatRule);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task ReloadFormatRulePivot(PivotGridControlEx pivot, string formId)
	{
		try
		{
			if (pivot == null)
			{
				return;
			}
			if (pivot == null || string.IsNullOrEmpty(formId))
			{
				return;
			}
			pivot.FormatRules.Clear();
			DataTable dataTable = await GetFormatRule(formId, pivot.ItemId);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				FormatRule setting = new FormatRule
				{
					FormId = Function.ToString(dataTable.Rows[i]["FormId"]),
					ItemId = Function.ToString(dataTable.Rows[i]["ItemId"]),
					ColumnId = Function.ToString(dataTable.Rows[i]["ColumnId"]),
					RowId = Function.ToString(dataTable.Rows[i]["RowId"]),
					Measure = Function.ToString(dataTable.Rows[i]["Measure"]),
					UserCode = Function.ToString(dataTable.Rows[i]["UserCode"]),
					ApplyToRow = Function.ParseBool(dataTable.Rows[i]["ApplyToRow"]),
					Expression = Function.ToString(dataTable.Rows[i]["Expression"]),
					BackColor = Function.ToString(dataTable.Rows[i]["BackColor"]),
					BackColor2 = Function.ToString(dataTable.Rows[i]["BackColor2"]),
					BorderColor = Function.ToString(dataTable.Rows[i]["BorderColor"]),
					ForeColor = Function.ToString(dataTable.Rows[i]["ForeColor"]),
					GradientMode = Function.ParseInt(dataTable.Rows[i]["GradientMode"]),
					FontSizeDelta = Function.ParseInt(dataTable.Rows[i]["FontSizeDelta"]),
					FontStyleDelta = Function.ParseInt(dataTable.Rows[i]["FontSizeDelta"]),
					Image = Function.ToString(dataTable.Rows[i]["Image"]),
					Font = Function.ToString(dataTable.Rows[i]["Font"]),
					Bold = Function.ParseBool(dataTable.Rows[i]["Bold"]),
					Italic = Function.ParseBool(dataTable.Rows[i]["Italic"]),
					Strikeout = Function.ParseBool(dataTable.Rows[i]["Strikeout"]),
					Size = Function.ParseFloat(dataTable.Rows[i]["Size"]),
					UnderLine = Function.ParseBool(dataTable.Rows[i]["UnderLine"]),
					GdiCharSet = Function.ParseByte(dataTable.Rows[i]["GdiCharSet"]),
					GdiVerticalFont = Function.ParseBool(dataTable.Rows[i]["GdiVerticalFont"]),
					Unit = Function.ParseInt(dataTable.Rows[i]["Unit"]),
					HAlignment = Function.ParseInt(dataTable.Rows[i]["HAlignment"]),
					HotkeyPrefix = Function.ParseInt(dataTable.Rows[i]["HotkeyPrefix"]),
					Trimming = Function.ParseInt(dataTable.Rows[i]["Trimming"]),
					VAlignment = Function.ParseInt(dataTable.Rows[i]["VAlignment"]),
					WordWrap = Function.ParseInt(dataTable.Rows[i]["WordWrap"])
				};
				PivotGridFormatRule pivotGridFormatRule = new PivotGridFormatRule();
				PivotGridField pivotGridField = pivot.Fields.Cast<PivotGridField>().FirstOrDefault((PivotGridField t) => t.FieldName.Equals(setting.ColumnId));
				PivotGridField pivotGridField2 = pivot.Fields.Cast<PivotGridField>().FirstOrDefault((PivotGridField t) => t.FieldName.Equals(setting.RowId));
				if (pivotGridField != null && pivotGridField2 != null)
				{
					PivotGridField column = pivot.Fields[setting.ColumnId];
					PivotGridField row = pivot.Fields[setting.RowId];
					pivotGridFormatRule.Settings = new FormatRuleFieldIntersectionSettings
					{
						Column = column,
						Row = row
					};
				}
				pivotGridFormatRule.Measure = pivot.Fields[setting.Measure];
				FormatConditionRuleExpression rule = new FormatConditionRuleExpression();
				FormatCondition(rule, setting);
				pivotGridFormatRule.Rule = rule;
				pivot.FormatRules.Add(pivotGridFormatRule);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void FormatCondition(FormatConditionRuleExpression rule, FormatRule setting)
	{
		try
		{
			rule.Expression = setting.Expression;
			if (setting.BackColor != "0")
			{
				if (isMatchColor(setting.BackColor))
				{
					rule.Appearance.BackColor = ColorTranslator.FromHtml("#" + setting.BackColor);
				}
				else
				{
					rule.Appearance.BackColor = Color.FromName(setting.BackColor);
				}
			}
			else
			{
				rule.Appearance.BackColor = Color.Empty;
			}
			if (setting.BackColor2 != "0")
			{
				if (isMatchColor(setting.BackColor2))
				{
					rule.Appearance.BackColor2 = ColorTranslator.FromHtml("#" + setting.BackColor2);
				}
				else
				{
					rule.Appearance.BackColor2 = Color.FromName(setting.BackColor2);
				}
			}
			else
			{
				rule.Appearance.BackColor2 = Color.Empty;
			}
			if (setting.BorderColor != "0")
			{
				if (isMatchColor(setting.BorderColor))
				{
					rule.Appearance.BorderColor = ColorTranslator.FromHtml("#" + setting.BorderColor);
				}
				else
				{
					rule.Appearance.BorderColor = Color.FromName(setting.BorderColor);
				}
			}
			else
			{
				rule.Appearance.BorderColor = Color.Empty;
			}
			if (setting.ForeColor != "0")
			{
				if (isMatchColor(setting.ForeColor))
				{
					rule.Appearance.ForeColor = ColorTranslator.FromHtml("#" + setting.ForeColor);
				}
				else
				{
					rule.Appearance.ForeColor = Color.FromName(setting.ForeColor);
				}
			}
			else
			{
				rule.Appearance.ForeColor = Color.Empty;
			}
			rule.Appearance.GradientMode = ((Function.ToString(setting.GradientMode) != null) ? GradientModeValue(setting.GradientMode) : AppearanceObject.EmptyAppearance.GradientMode);
			rule.Appearance.FontSizeDelta = ((setting.FontSizeDelta > 0) ? setting.FontSizeDelta : AppearanceObject.EmptyAppearance.FontSizeDelta);
			rule.Appearance.FontStyleDelta = ((setting.FontStyleDelta > 0) ? FontStyleDeltaValue(setting.FontStyleDelta) : AppearanceObject.EmptyAppearance.FontStyleDelta);
			GraphicsUnit unit = ((setting.Unit > 0) ? UnitValue(setting.Unit) : GraphicsUnit.Point);
			FontStyle fontStyle = FontStyle.Regular;
			FontStyle fontStyle2 = FontStyle.Regular;
			FontStyle fontStyle3 = FontStyle.Regular;
			FontStyle fontStyle4 = FontStyle.Regular;
			if (setting.Bold)
			{
				fontStyle = FontStyle.Bold;
			}
			if (setting.Italic)
			{
				fontStyle2 = FontStyle.Italic;
			}
			if (setting.UnderLine)
			{
				fontStyle3 = FontStyle.Underline;
			}
			if (setting.Strikeout)
			{
				fontStyle4 = FontStyle.Strikeout;
			}
			float num = 0f;
			num = ((!(setting.Size > 0f)) ? AppearanceObject.DefaultFont.Size : setting.Size);
			rule.Appearance.Font = new Font(setting.Font, num, fontStyle | fontStyle2 | fontStyle3 | fontStyle4, unit, setting.GdiCharSet, setting.GdiVerticalFont);
			rule.Appearance.TextOptions.HAlignment = HAlignmentValue(setting.HAlignment);
			rule.Appearance.TextOptions.HotkeyPrefix = HotkeyPrefixValue(setting.HotkeyPrefix);
			rule.Appearance.TextOptions.Trimming = TrimmingValue(setting.Trimming);
			rule.Appearance.TextOptions.VAlignment = VertAlignmentValue(setting.VAlignment);
			rule.Appearance.TextOptions.WordWrap = WordWrapValue(setting.WordWrap);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static bool isMatchColor(string str)
	{
		try
		{
			return Regex.IsMatch(str, "\\A\\b[0-9a-fA-F]+\\b\\Z");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private static WordWrap WordWrapValue(int value)
	{
		try
		{
			WordWrap result = WordWrap.Default;
			switch (value)
			{
			case 0:
				result = WordWrap.Default;
				break;
			case 1:
				result = WordWrap.NoWrap;
				break;
			case 2:
				result = WordWrap.Wrap;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return WordWrap.Default;
		}
	}

	private static VertAlignment VertAlignmentValue(int value)
	{
		try
		{
			VertAlignment result = VertAlignment.Default;
			switch (value)
			{
			case 0:
				result = VertAlignment.Default;
				break;
			case 1:
				result = VertAlignment.Top;
				break;
			case 2:
				result = VertAlignment.Center;
				break;
			case 3:
				result = VertAlignment.Bottom;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return VertAlignment.Default;
		}
	}

	private static Trimming TrimmingValue(int value)
	{
		try
		{
			Trimming result = Trimming.Default;
			switch (value)
			{
			case 0:
				result = Trimming.Default;
				break;
			case 1:
				result = Trimming.None;
				break;
			case 2:
				result = Trimming.Character;
				break;
			case 3:
				result = Trimming.Word;
				break;
			case 4:
				result = Trimming.EllipsisCharacter;
				break;
			case 5:
				result = Trimming.EllipsisWord;
				break;
			case 6:
				result = Trimming.EllipsisPath;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return Trimming.Default;
		}
	}

	private static HKeyPrefix HotkeyPrefixValue(int value)
	{
		try
		{
			HKeyPrefix result = HKeyPrefix.Default;
			switch (value)
			{
			case 0:
				result = HKeyPrefix.Default;
				break;
			case 1:
				result = HKeyPrefix.None;
				break;
			case 2:
				result = HKeyPrefix.Show;
				break;
			case 3:
				result = HKeyPrefix.Hide;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return HKeyPrefix.Default;
		}
	}

	private static HorzAlignment HAlignmentValue(int value)
	{
		try
		{
			HorzAlignment result = HorzAlignment.Default;
			switch (value)
			{
			case 0:
				result = HorzAlignment.Default;
				break;
			case 1:
				result = HorzAlignment.Near;
				break;
			case 2:
				result = HorzAlignment.Center;
				break;
			case 3:
				result = HorzAlignment.Far;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return HorzAlignment.Default;
		}
	}

	private static GraphicsUnit UnitValue(int value)
	{
		try
		{
			GraphicsUnit result = GraphicsUnit.World;
			switch (value)
			{
			case 0:
				result = GraphicsUnit.World;
				break;
			case 1:
				result = GraphicsUnit.Display;
				break;
			case 2:
				result = GraphicsUnit.Pixel;
				break;
			case 3:
				result = GraphicsUnit.Point;
				break;
			case 4:
				result = GraphicsUnit.Inch;
				break;
			case 5:
				result = GraphicsUnit.Document;
				break;
			case 6:
				result = GraphicsUnit.Millimeter;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return GraphicsUnit.World;
		}
	}

	private static FontStyle FontStyleDeltaValue(int value)
	{
		try
		{
			FontStyle result = FontStyle.Regular;
			switch (value)
			{
			case 1:
				result = FontStyle.Bold;
				break;
			case 2:
				result = FontStyle.Italic;
				break;
			case 3:
				result = FontStyle.Underline;
				break;
			case 4:
				result = FontStyle.Strikeout;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return FontStyle.Regular;
		}
	}

	private static LinearGradientMode GradientModeValue(int value)
	{
		try
		{
			LinearGradientMode result = LinearGradientMode.Horizontal;
			switch (value)
			{
			case 0:
				result = LinearGradientMode.Horizontal;
				break;
			case 1:
				result = LinearGradientMode.Vertical;
				break;
			case 2:
				result = LinearGradientMode.ForwardDiagonal;
				break;
			case 3:
				result = LinearGradientMode.BackwardDiagonal;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return LinearGradientMode.Horizontal;
		}
	}

	public static GridControlEx GetGridControlExByName(this Control control, string gridName)
	{
		try
		{
			if (control.GetType() == typeof(GridControlEx))
			{
				if (control is GridControlEx gridControlEx && gridControlEx.DefaultView.Name.ToUpper().Equals(gridName.ToUpper()))
				{
					return gridControlEx;
				}
			}
			else
			{
				foreach (Control control2 in control.Controls)
				{
					GridControlEx gridControlExByName = control2.GetGridControlExByName(gridName);
					if (gridControlExByName != null)
					{
						return gridControlExByName.GetGridControlExByName(gridName);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static async Task<DataTable> GetFormatRule(string formId, string itemId)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormatRule");
			return await helper.GetAsync("GetFormatRule", new
			{
				formId = formId,
				itemId = itemId,
				user = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public static void DisableAllControl(this Control.ControlCollection controls)
	{
		try
		{
			foreach (Control control in controls)
			{
				switch (control.GetControlType())
				{
				case "SimpleButton":
					control.Enabled = false;
					break;
				case "CheckEdit":
					control.Enabled = false;
					break;
				case "RadioGroup":
					control.Enabled = false;
					break;
				case "TextEdit":
					((TextEdit)control).ReadOnly = true;
					break;
				case "ContextMenuStrip":
					((ContextMenuStrip)control).Enabled = false;
					break;
				case "GridControlEx":
					foreach (GridColumnEx column in ((GridViewEx)((GridControlEx)control).MainView).Columns)
					{
						column.OptionsColumn.AllowEdit = false;
						column.OptionsColumn.ReadOnly = true;
					}
					break;
				}
				if (control.Controls.Count > 0)
				{
					control.Controls.DisableAllControl();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static string GetControlType(this Control ctl)
	{
		if (ctl is TextEdit)
		{
			return "TextEdit";
		}
		if (ctl is CheckEdit)
		{
			return "CheckEdit";
		}
		if (ctl is SimpleButton)
		{
			return "SimpleButton";
		}
		if (ctl is GridControlEx)
		{
			return "GridControlEx";
		}
		if (ctl is RadioGroup)
		{
			return "RadioGroup";
		}
		if (ctl is ContextMenuStrip)
		{
			return "ContextMenuStrip";
		}
		return "";
	}

	public static RepositoryItemLookUpEdit SetDataSource(this RepositoryItemLookUpEdit look, Dictionary<string, string> dic, bool hideCodeColumn = false)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (KeyValuePair<string, string> item in dic)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = item.Key;
			dataRow["Name"] = item.Value;
			dataTable.Rows.Add(dataRow);
		}
		look.DataSource = dataTable;
		look.DisplayMember = "Name";
		look.ValueMember = "Code";
		if (hideCodeColumn)
		{
			look.PopulateColumns();
			if (look.Columns.Count > 0)
			{
				look.Columns[0].Visible = false;
			}
		}
		return look;
	}

	public static LookUpEdit SetDataSource(this LookUpEdit look, Dictionary<string, string> dic)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (KeyValuePair<string, string> item in dic)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = item.Key;
			dataRow["Name"] = item.Value;
			dataTable.Rows.Add(dataRow);
		}
		look.Properties.DataSource = dataTable;
		look.Properties.DisplayMember = "Name";
		look.Properties.ValueMember = "Code";
		return look;
	}

	public static LookUpEdit SetDataSource(this LookUpEdit look, DataTable dt, string display, string value, bool emptyRow = false, bool emptyTop = true, bool hideOtherColumns = false)
	{
		if ((dt.IsNotNull() & emptyRow) && dt.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t[value]))) == null)
		{
			DataRow dataRow = dt.NewRow();
			dataRow[display] = DBNull.Value;
			dataRow[value] = DBNull.Value;
			if (emptyTop)
			{
				dt.Rows.Add(dataRow);
			}
			else
			{
				dt.Rows.InsertAt(dataRow, 0);
			}
		}
		look.Properties.DataSource = dt;
		look.Properties.DisplayMember = display;
		look.Properties.ValueMember = value;
		if (hideOtherColumns)
		{
			look.Properties.ForceInitialize();
			look.Properties.PopulateColumns();
			for (int num = 0; num < look.Properties.Columns.Count; num++)
			{
				LookUpColumnInfo lookUpColumnInfo = look.Properties.Columns[num];
				if (lookUpColumnInfo.FieldName == value || lookUpColumnInfo.FieldName == display)
				{
					lookUpColumnInfo.Visible = true;
				}
				else
				{
					lookUpColumnInfo.Visible = false;
				}
			}
		}
		return look;
	}

	public static RepositoryItemLookUpEdit SetDataSource(this RepositoryItemLookUpEdit look, DataTable dt, string display, string value, bool emptyRow = false, bool emptyTop = true)
	{
		if ((dt.IsNotNull() & emptyRow) && dt.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t[value]))) == null)
		{
			DataRow dataRow = dt.NewRow();
			dataRow[display] = DBNull.Value;
			dataRow[value] = DBNull.Value;
			if (emptyTop)
			{
				dt.Rows.Add(dataRow);
			}
			else
			{
				dt.Rows.InsertAt(dataRow, 0);
			}
		}
		look.DataSource = dt;
		look.DisplayMember = display;
		look.ValueMember = value;
		return look;
	}

	public static RepositoryItemLookUpEdit SetDataSource(this RepositoryItemLookUpEdit look, Dictionary<string, string> dic, string display, string value, bool emptyRow = false, bool emptyTop = true)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (KeyValuePair<string, string> item in dic)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = item.Key;
			dataRow["Name"] = item.Value;
			dataTable.Rows.Add(dataRow);
		}
		look.SetDataSource(dataTable, display, value, emptyRow, emptyTop);
		return look;
	}

	public static RepositoryItemLookUpEdit SetDataSourceObject(this RepositoryItemLookUpEdit look)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = (int)value;
			string text = Function.ToString(value);
			dataRow["Name"] = ((value <= ApzObjectTypes.oNone) ? "" : Function.SpaceBeforeCapital(text.Substring(1, text.Length - 1)));
			dataTable.Rows.Add(dataRow);
		}
		look.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
		return look;
	}

	public static LookUpEdit SetDataSourceObject(this LookUpEdit look)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = (int)value;
			string text = Function.ToString(value);
			dataRow["Name"] = Function.SpaceBeforeCapital(text.Substring(1, text.Length - 1));
			dataTable.Rows.Add(dataRow);
		}
		look.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
		return look;
	}

	public static LookUpEdit SetDataSource(this LookUpEdit look, string[,] arrSource, bool isStringCode, object defaultValue = null, bool isShort = false)
	{
		try
		{
			look.Properties.DisplayMember = "Name";
			look.Properties.ValueMember = "Code";
			look.Properties.NullText = "";
			look.Properties.Columns.Clear();
			LookUpColumnInfo column = new LookUpColumnInfo("Name", "Name");
			look.Properties.Columns.Add(column);
			DataTable dataTable = new DataTable();
			if (isStringCode)
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			}
			else if (isShort)
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(short)));
			}
			else
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
			}
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			int length = arrSource.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				if (isStringCode)
				{
					dataRow["Code"] = Function.ToString(arrSource[i, 0]);
				}
				else if (isShort)
				{
					dataRow["Code"] = Function.ParseShort(arrSource[i, 0]);
				}
				else
				{
					dataRow["Code"] = Function.ParseInt(arrSource[i, 0]);
				}
				dataRow["Name"] = arrSource[i, 1].ToString();
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			look.Properties.DataSource = dataTable;
			if (defaultValue != null)
			{
				if (isStringCode)
				{
					look.EditValue = Function.ToString(defaultValue);
				}
				else if (isShort)
				{
					look.EditValue = Function.ParseShort(defaultValue);
				}
				else
				{
					look.EditValue = Function.ParseInt(defaultValue);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return look;
	}

	public static RepositoryItemLookUpEdit SetDataSource(this RepositoryItemLookUpEdit look, string[,] arrSource, bool isStringCode, bool isShort = false)
	{
		try
		{
			look.DisplayMember = "Name";
			look.ValueMember = "Code";
			look.NullText = "";
			look.Columns.Clear();
			LookUpColumnInfo column = new LookUpColumnInfo("Name", "Name");
			look.Columns.Add(column);
			int length = arrSource.GetLength(0);
			DataTable dataTable = new DataTable();
			if (isStringCode)
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			}
			else if (isShort)
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(short)));
			}
			else
			{
				dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
			}
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			for (int i = 0; i < length; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				if (isStringCode)
				{
					dataRow["Code"] = arrSource[i, 0].ToString();
				}
				else if (isShort)
				{
					dataRow["Code"] = Function.ParseShort(arrSource[i, 0]);
				}
				else
				{
					dataRow["Code"] = Function.ParseInt(arrSource[i, 0]);
				}
				dataRow["Name"] = arrSource[i, 1].ToString();
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			look.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return look;
	}

	public static LookUpEdit SetDataSourceByList(this LookUpEdit luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.Properties.DataSource = list;
			luk.Properties.DisplayMember = "Name";
			luk.Properties.ValueMember = "Code";
			luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.Properties.ForceInitialize();
			luk.Properties.PopulateColumns();
			luk.Properties.Columns["Code"].Visible = false;
			luk.Properties.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return luk;
	}

	public static void GetFormatControl(Enums.DisplayFormatType type, out MaskType marksType, out FormatType formatType, out string formatString)
	{
		marksType = MaskType.None;
		formatType = FormatType.None;
		formatString = "";
		switch (type)
		{
		case Enums.DisplayFormatType.Amount:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.QuantitiesFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.DateTime:
			marksType = MaskType.DateTime;
			formatType = FormatType.DateTime;
			formatString = GlobalData.DateTimeFormat;
			break;
		case Enums.DisplayFormatType.Percent:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.PercentageRateFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.Price:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.UnitPriceFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.Qty:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.QuantitiesFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.Rate:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.PercentageRateFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.Unit:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = $"n{GlobalData.QuantitiesFormat.NumberDecimalDigits}";
			break;
		case Enums.DisplayFormatType.Time:
			marksType = MaskType.DateTime;
			formatType = FormatType.DateTime;
			formatString = GlobalData.TimeFormat;
			break;
		case Enums.DisplayFormatType.Integer:
			marksType = MaskType.Numeric;
			formatType = FormatType.Numeric;
			formatString = "n0";
			break;
		}
	}

	public static DataTable DatasourceTable(this GridControl grd)
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

	public static int FindRowHandleByDataRow(this GridViewEx view, DataRow row)
	{
		try
		{
			if (row != null)
			{
				for (int i = 0; i < view.DataRowCount; i++)
				{
					if (view.GetDataRow(i) == row)
					{
						return i;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return int.MinValue;
	}

	public static Tuple<bool, bool> CheckExistProperty(string propertyName, Control control)
	{
		try
		{
			bool flag = false;
			PropertyInfo[] properties = control.GetType().GetProperties();
			if (properties.Any((PropertyInfo x) => x.Name == propertyName))
			{
				flag = false;
				return Tuple.Create(item1: true, flag);
			}
			int num = properties.FindIndex((PropertyInfo x) => x.Name == "Properties");
			if (num == -1)
			{
				return Tuple.Create(item1: false, item2: false);
			}
			bool flag2 = properties[num].PropertyType.GetMember(propertyName).Any((MemberInfo x) => x.Name == propertyName);
			flag = true;
			if (flag2)
			{
				return Tuple.Create(item1: true, flag);
			}
			return Tuple.Create(item1: false, flag);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return Tuple.Create(item1: false, item2: false);
		}
	}

	public static void EnableDisableControl(this Control.ControlCollection control, bool isEnable, bool readAuth = false)
	{
		try
		{
			if (control == null)
			{
				return;
			}
			int count = control.Count;
			for (int i = 0; i < count; i++)
			{
				dynamic val = control[i];
				string text = val.GetType().Name;
				_ = (string)val.Name;
				dynamic val2 = ExtensionMethods.CheckExistProperty("IsSearchControl", val);
				bool flag = val2.Item1;
				bool flag2 = val2.Item2;
				if (flag)
				{
					if (flag2)
					{
						if (val.Properties.IsSearchControl == true)
						{
							continue;
						}
					}
					else if (val.IsSearchControl == true)
					{
						continue;
					}
				}
				switch (text)
				{
				case "TextBox":
				case "TextEdit":
				case "TextBoxEx":
				case "TextEditEx":
				case "SpinEdit":
				case "SpinEditEx":
				case "ComboBoxEditEx":
				case "DateEdit":
				case "DateEditEx":
				case "CalcEdit":
				case "CalcEditEx":
				case "CheckEdit":
				case "CheckEditEx":
				case "RadioGroup":
				case "LookUpEdit":
				case "LookUpEditEx":
				case "TimeEdit":
				case "TimeEditEx":
				case "TimeSpanEditEx":
				case "CheckedComboBoxEdit":
				case "GridLookUpEdit":
				case "MemoEdit":
				case "MemoEditEx":
				case "MemoExEdit":
				case "MemoExEditEx":
				case "ComboBoxEdit":
				case "RichTextBox":
				case "RichEditControl":
				case "HourEdit":
				case "LookUpEditDefineNew":
				case "CheckedComboBoxEditDefineNew":
				case "LookUpEditSimple":
				case "LookUpEditCombo":
				case "TextEditExPrice":
				case "BreadCrumbEditEx":
				case "RadioGroupEx":
					val.ReadOnly = !isEnable;
					break;
				case "PictureEdit":
				case "CustomOnPaintSimpleButton":
					val.Enabled = isEnable;
					break;
				case "ButtonEdit":
				case "ButtonEditEx":
				case "ChooseFromListEx":
				case "ButtonEditCFL":
				case "ButtonEditAttach":
				case "ButtonEditDistrRule":
				{
					val.ReadOnly = !isEnable;
					EditorButton editorButton3 = ((EditorButtonCollection)val.Properties.Buttons).FirstOrDefault((EditorButton t) => !t.IsLeft);
					if (editorButton3 != null)
					{
						editorButton3.Visible = isEnable;
					}
					break;
				}
				case "DropDownButton":
				case "DropDownButtonEx":
					val.Enabled = isEnable;
					break;
				case "Button":
				case "ButtonEx":
				case "SimpleButton":
				case "SimpleButtonEx":
					if (val.FindForm().CancelButton != val)
					{
						val.Enabled = isEnable;
					}
					break;
				case "TreeList":
				case "TreeView":
				case "TreeEx":
				{
					val.ReadOnly = !isEnable;
					RepositoryItemCollection repositoryItemCollection2 = val.RepositoryItems;
					for (int num = 0; num < repositoryItemCollection2.Count; num++)
					{
						if (repositoryItemCollection2[num] is RepositoryItemButtonEdit { Buttons: var buttons2 })
						{
							EditorButton editorButton2 = buttons2.FirstOrDefault((EditorButton t) => !t.IsLeft);
							if (editorButton2 != null)
							{
								editorButton2.Visible = isEnable;
							}
						}
					}
					if (val.ContextMenuStrip != null)
					{
						if (!isEnable & readAuth)
						{
							ExtensionMethods.HideContextMenuAuthRead(val.ContextMenuStrip);
						}
						else
						{
							val.ContextMenuStrip.Enabled = isEnable;
						}
					}
					break;
				}
				case "GridControl":
				case "GridControlEx":
				{
					ViewRepositoryCollection viewRepositoryCollection = val.ViewCollection;
					for (int j = 0; j < viewRepositoryCollection.Count; j++)
					{
						if (viewRepositoryCollection[j] is GridViewEx gridViewEx)
						{
							if (!isEnable & readAuth)
							{
								gridViewEx.SettingReadonly = true;
							}
							gridViewEx.ReadOnly = !isEnable;
						}
						else
						{
							(viewRepositoryCollection[j] as GridView).OptionsBehavior.ReadOnly = !isEnable;
						}
					}
					RepositoryItemCollection repositoryItemCollection = val.RepositoryItems;
					for (int k = 0; k < repositoryItemCollection.Count; k++)
					{
						if (repositoryItemCollection[k] is RepositoryItemButtonEdit { Buttons: var buttons })
						{
							EditorButton editorButton = buttons.FirstOrDefault((EditorButton t) => !t.IsLeft);
							if (editorButton != null)
							{
								editorButton.Visible = isEnable;
							}
						}
					}
					if (val.ContextMenuStrip != null)
					{
						if (!isEnable & readAuth)
						{
							ExtensionMethods.HideContextMenuAuthRead(val.ContextMenuStrip);
						}
						else
						{
							val.ContextMenuStrip.Enabled = isEnable;
						}
					}
					break;
				}
				case "PivotGridControl":
				{
					RepositoryItemCollection repositoryItemCollection3 = val.RepositoryItems;
					for (int num2 = 0; num2 < repositoryItemCollection3.Count; num2++)
					{
						if (repositoryItemCollection3[num2] is RepositoryItemButtonEdit { Buttons: var buttons3 })
						{
							EditorButton editorButton4 = buttons3.FirstOrDefault((EditorButton t) => !t.IsLeft);
							if (editorButton4 != null)
							{
								editorButton4.Visible = isEnable;
							}
						}
					}
					if (val.ContextMenuStrip != null)
					{
						if (!isEnable & readAuth)
						{
							ExtensionMethods.HideContextMenuAuthRead(val.ContextMenuStrip);
						}
						else
						{
							val.ContextMenuStrip.Enabled = isEnable;
						}
					}
					break;
				}
				default:
					if (val.Controls.Count > 0)
					{
						ExtensionMethods.EnableDisableControl(val.Controls, isEnable, readAuth);
					}
					break;
				case "Label":
				case "LabelLine":
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private static void HideContextMenuAuthRead(ContextMenuStrip ctx)
	{
		(from i in ctx.Items.OfType<ToolStripItem>()
			select i.Name).ToList();
		foreach (ToolStripItem item in ctx.Items)
		{
			if (!listNotDisable.Contains(item.Name))
			{
				item.Visible = false;
			}
		}
	}

	public static async Task SetControlMaxLength(this XtraFormEx form, Dictionary<string, Control> dicControls)
	{
		try
		{
			List<string> list = dicControls.Keys.Select((string text) => Function.ToString(text.Split('.')[0])).Distinct().ToList();
			foreach (string table in list)
			{
				DataTable dataTable;
				using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
				{
					dataTable = await hp.GetAsync("GetDataStructure", new { table });
				}
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					string key = table + "." + Function.ToString(dataTable.Rows[num]["COLUMN_NAME"]);
					if (dicControls.TryGetValue(key, out var value) && Function.ParseInt(dataTable.Rows[num]["CHARACTER_MAXIMUM_LENGTH"]) > 0)
					{
						if (value is TextEdit)
						{
							((TextEdit)value).Properties.MaxLength = Function.ParseInt(dataTable.Rows[num]["CHARACTER_MAXIMUM_LENGTH"]);
						}
						if (value is MemoExEdit)
						{
							((MemoExEdit)value).Properties.MaxLength = Function.ParseInt(dataTable.Rows[num]["CHARACTER_MAXIMUM_LENGTH"]);
						}
						if (value is MemoEdit)
						{
							((MemoEdit)value).Properties.MaxLength = Function.ParseInt(dataTable.Rows[num]["CHARACTER_MAXIMUM_LENGTH"]);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void SetGridColumnMaxLength(this GridViewEx gv, string tableName)
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				dataTable = httpClientHelper.Get("GetTableColumnSchema", new { tableName });
			}
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				string fieldName = Function.ToString(row["COLUMN_NAME"]);
				if (gv.Columns[fieldName] != null && Function.ToString(row["DATA_TYPE"]) == "nvarchar")
				{
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.MaxLength = Function.ParseInt(row["CHARACTER_MAXIMUM_LENGTH"]);
					gv.GridControl.RepositoryItems.Add(repositoryItemTextEdit);
					gv.Columns[fieldName].ColumnEdit = repositoryItemTextEdit;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static string[] GetDataCurrency(this string value)
	{
		try
		{
			char[] array = new char[14]
			{
				'-', '+', '0', '1', '2', '3', '4', '5', '6', '7',
				'8', '9', '\0', '\0'
			};
			array[12] = Convert.ToChar(GlobalData.GeneralSetting.DecSep);
			array[13] = Convert.ToChar(GlobalData.GeneralSetting.ThousSep);
			char[] numberCharacter = array;
			if (string.IsNullOrEmpty(value) || value.Length == 0)
			{
				return null;
			}
			value = value.ToLower().Trim();
			if (value.Length == 0)
			{
				return new string[2] { "", "" };
			}
			string currency = "";
			List<char> list = value.ToCharArray().ToList();
			int num = list.FindIndex((char t) => numberCharacter.Contains(t));
			string text;
			if (num > 0)
			{
				currency = value.Substring(0, num - 1).Trim();
				text = value.Substring(num).Trim();
			}
			else
			{
				int num2 = list.FindLastIndex((char t) => numberCharacter.Contains(t));
				if (num2 < list.Count - 1)
				{
					currency = value.Substring(num2 + 1).Trim();
				}
				text = value.Substring(0, num2 + 1).Trim();
			}
			if (GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToLower().Equals(currency)) != null)
			{
				return new string[2] { text, currency };
			}
			return new string[2] { text, "" };
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static bool AddControllProcessSource(this IControlEx control, string fieldName)
	{
		try
		{
			if (control == null)
			{
				return true;
			}
			if (!(control.FindForm() is XtraFormEx { LstControlSource: var lstControlSource } xtraFormEx))
			{
				return true;
			}
			if (lstControlSource.Contains(control))
			{
				if (string.IsNullOrEmpty(fieldName))
				{
					lstControlSource.Remove(control);
					return false;
				}
			}
			else if (!string.IsNullOrEmpty(fieldName))
			{
				lstControlSource.Add(control);
			}
			if (xtraFormEx.DataSource != null)
			{
				xtraFormEx.IsSetDataSouceControl = true;
				control.SetValueControlFromDataSource(fieldName, xtraFormEx.DataSource);
				xtraFormEx.IsSetDataSouceControl = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public static void SetValueControlFromDataSource(this IControlEx con, string fieldName, object sourceForm)
	{
		try
		{
			if (con == null || sourceForm == null || string.IsNullOrEmpty(fieldName))
			{
				return;
			}
			object obj;
			if (sourceForm is DataTable)
			{
				DataTable dataTable = sourceForm as DataTable;
				if (!dataTable.IsNotNull())
				{
					return;
				}
				obj = (dataTable.Columns.Contains(fieldName) ? dataTable.Rows[0][fieldName] : null);
				if (con is BaseEdit)
				{
					(con as BaseEdit).EditValue = obj;
				}
				else if (con is LabelControl)
				{
					con.Text = Function.ToString(obj);
				}
				else if (con is LabelLine)
				{
					(con as LabelLine).Utext = Function.ToString(obj);
				}
				else if (con is RichEditControlEx)
				{
					RichEditControlEx richEditControlEx = con as RichEditControlEx;
					string text = Function.ToString(obj);
					if (text.Contains("w:wordDocument"))
					{
						richEditControlEx.WordMLText = text;
					}
					else
					{
						richEditControlEx.HtmlText = text;
					}
				}
				return;
			}
			if (sourceForm is DataRow)
			{
				DataRow dataRow = sourceForm as DataRow;
				obj = (dataRow.Table.Columns.Contains(fieldName) ? dataRow[fieldName] : null);
				if (con is BaseEdit)
				{
					(con as BaseEdit).EditValue = obj;
				}
				else if (con is LabelControl)
				{
					con.Text = Function.ToString(obj);
				}
				else if (con is LabelLine)
				{
					(con as LabelLine).Utext = Function.ToString(obj);
				}
				else if (con is RichEditControlEx)
				{
					RichEditControlEx richEditControlEx2 = con as RichEditControlEx;
					string text2 = Function.ToString(obj);
					if (text2.Contains("w:wordDocument"))
					{
						richEditControlEx2.WordMLText = text2;
					}
					else
					{
						richEditControlEx2.HtmlText = text2;
					}
				}
				return;
			}
			obj = ((sourceForm.GetType().GetProperty(fieldName) != null) ? sourceForm.GetType().GetProperty(fieldName).GetValue(sourceForm) : null);
			if (con is BaseEdit)
			{
				(con as BaseEdit).EditValue = obj;
			}
			else if (con is LabelControl)
			{
				con.Text = Function.ToString(obj);
			}
			else if (con is LabelLine)
			{
				(con as LabelLine).Utext = Function.ToString(obj);
			}
			else if (con is RichEditControlEx)
			{
				RichEditControlEx richEditControlEx3 = con as RichEditControlEx;
				string text3 = Function.ToString(obj);
				if (text3.Contains("w:wordDocument"))
				{
					richEditControlEx3.WordMLText = text3;
				}
				else
				{
					richEditControlEx3.HtmlText = text3;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static bool AddControllProcessTitleSource(this IControlEx control, string fieldName)
	{
		try
		{
			if (control == null)
			{
				return true;
			}
			if (!(control.FindForm() is XtraFormEx { LstControlTittleSource: var lstControlTittleSource } xtraFormEx))
			{
				return true;
			}
			if (lstControlTittleSource.Contains(control))
			{
				if (string.IsNullOrEmpty(fieldName))
				{
					lstControlTittleSource.Remove(control);
					return false;
				}
			}
			else if (!string.IsNullOrEmpty(fieldName))
			{
				lstControlTittleSource.Add(control);
			}
			if (xtraFormEx.DataSourceTittle != null)
			{
				control.SetValueControlFromDataSourceTitle(fieldName, xtraFormEx.DataSourceTittle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public static void SetValueControlFromDataSourceTitle(this IControlEx control, string fieldName, DataTable sourceTitle)
	{
		try
		{
			if (control == null || !sourceTitle.IsNotNull() || string.IsNullOrEmpty(fieldName) || !sourceTitle.Columns.Contains("ColumnId") || !sourceTitle.Columns.Contains("ColumnName"))
			{
				return;
			}
			DataRow dataRow = sourceTitle.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(fieldName));
			if (dataRow == null)
			{
				return;
			}
			bool flag = Function.ParseBool(dataRow["Visible"]);
			bool flag2 = !Function.ParseBool(dataRow["Editable"]);
			control.Text = Function.ToString(dataRow["ColumnName"]);
			if (control.Visible)
			{
				control.Visible = flag;
			}
			control.SettingInVisible = !flag;
			control.ReadOnly = flag2;
			control.SettingReadonly = flag2;
			if (control.EditControl != null)
			{
				if (control.EditControl.Visible)
				{
					control.EditControl.Visible = flag;
				}
				control.EditControl.SettingInVisible = !flag;
				control.EditControl.ReadOnly = flag2;
				control.EditControl.SettingReadonly = flag2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void ChangeDataSourceWhenEditValueControl(this IControlEx control, object value, Enums.EventId eventId = Enums.EventId.OnControlValueChanged)
	{
		try
		{
			if (control == null)
			{
				return;
			}
			string fieldName = control.FieldName;
			if (!(control.FindForm() is XtraFormEx xtraFormEx))
			{
				return;
			}
			if (fieldName == null)
			{
				fieldName = "";
			}
			if (xtraFormEx.IsSetDataSouceControl)
			{
				RaiseControlEvent(xtraFormEx, new ControlChangedEventArgs(control, value, fieldName), eventId);
				return;
			}
			object dataSource = xtraFormEx.DataSource;
			if (dataSource == null)
			{
				RaiseControlEvent(xtraFormEx, new ControlChangedEventArgs(control, value, fieldName), eventId);
				return;
			}
			if (string.IsNullOrEmpty(fieldName))
			{
				RaiseControlEvent(xtraFormEx, new ControlChangedEventArgs(control, value, fieldName), eventId);
				return;
			}
			IControlEx[] array = xtraFormEx.LstControlSource.Where((IControlEx t) => t.FieldName.Equals(fieldName) && t != control).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				array[num].EditValue = value;
			}
			dataSource.ChangeSourceObjectByKey(fieldName, value);
			RaiseControlEvent(xtraFormEx, new ControlChangedEventArgs(control, value, fieldName), eventId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void ControlValueChanging(this IControlEx control, object newValue, object oldValue, CancelEventArgs cancelEventArgs)
	{
		try
		{
			if (control != null)
			{
				string fieldName = control.FieldName;
				if (control.FindForm() is XtraFormEx { IsSetDataSouceControl: false } xtraFormEx)
				{
					ControlChangingEventArgs e = new ControlChangingEventArgs(control, newValue, oldValue, fieldName ?? "");
					xtraFormEx.RaiseControlValueChanging(e);
					cancelEventArgs.Cancel = e.Cancel;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void ChangeSourceObjectByKey(this object sourceData, string fieldName, object value)
	{
		if (sourceData == null)
		{
			return;
		}
		if (sourceData is DataTable)
		{
			DataTable dataTable = sourceData as DataTable;
			if (dataTable.IsNotNull() && dataTable.Columns.Contains(fieldName))
			{
				dataTable.Rows[0][fieldName] = (string.IsNullOrEmpty(Function.ToString(value)) ? DBNull.Value : value);
			}
		}
		else if (sourceData is DataRow)
		{
			DataRow dataRow = sourceData as DataRow;
			if (dataRow.Table.Columns.Contains(fieldName))
			{
				dataRow[fieldName] = (string.IsNullOrEmpty(Function.ToString(value)) ? DBNull.Value : value);
			}
		}
		else
		{
			PropertyInfo property = sourceData.GetType().GetProperty(fieldName);
			if (property != null)
			{
				property.SetValue(sourceData, value);
			}
		}
	}

	private static void RaiseControlEvent(XtraFormEx form, ControlChangedEventArgs e, Enums.EventId eventId)
	{
		if (eventId == Enums.EventId.OnControlValueChanged)
		{
			form.RaiseControlValueChanged(e);
		}
		else
		{
			form.HandleControlEvent(eventId, e.FieldName);
		}
	}

	public static string PathSaveFile(string fileType, Enums.ExportExt fileExten)
	{
		string text = string.Empty;
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = string.Format("{0} Files (*.{1}*)|*.{1}", fileType, Function.ToString(fileExten).ToLower());
		saveFileDialog.FilterIndex = 1;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			text = saveFileDialog.FileName;
		}
		if (FileIO.FileExist(text))
		{
			FileIO.DeleteFile(text);
		}
		return text;
	}

	public static string GetDisplayNameUnitByCode(this short? unitCode, Enums.MeasureType measureUnitType)
	{
		switch (measureUnitType)
		{
		case Enums.MeasureType.LengthAndWidth:
		{
			LengthAndWidthImpl lengthAndWidthImpl = GlobalData.LengthWidthUnits.FirstOrDefault((LengthAndWidthImpl t) => t.UnitCode == unitCode);
			if (lengthAndWidthImpl != null)
			{
				return lengthAndWidthImpl.UnitDisply;
			}
			break;
		}
		case Enums.MeasureType.Weight:
		{
			WeightImpl weightImpl = GlobalData.WeightUnits.FirstOrDefault((WeightImpl t) => t.UnitCode == unitCode);
			if (weightImpl != null)
			{
				return weightImpl.UnitDisply;
			}
			break;
		}
		}
		return "";
	}

	public static short? GetUnitCodeByDisplayName(this string displayName, Enums.MeasureType measureUnitType)
	{
		switch (measureUnitType)
		{
		case Enums.MeasureType.LengthAndWidth:
		{
			LengthAndWidthImpl lengthAndWidthImpl = GlobalData.LengthWidthUnits.FirstOrDefault((LengthAndWidthImpl t) => t.UnitDisply.Equals(displayName));
			if (lengthAndWidthImpl != null)
			{
				return Function.ParseShort(lengthAndWidthImpl.UnitCode);
			}
			break;
		}
		case Enums.MeasureType.Weight:
		{
			WeightImpl weightImpl = GlobalData.WeightUnits.FirstOrDefault((WeightImpl t) => t.UnitDisply.Equals(displayName));
			if (weightImpl != null)
			{
				return Function.ParseShort(weightImpl.UnitCode);
			}
			break;
		}
		}
		return null;
	}

	public static double GetExchangeData(this short? toUnit, short? fromUnit, Enums.MeasureType measureType)
	{
		switch (measureType)
		{
		case Enums.MeasureType.LengthAndWidth:
		{
			LengthAndWidthImpl lengthAndWidthImpl = GlobalData.LengthWidthUnits.FirstOrDefault((LengthAndWidthImpl t) => t.UnitCode == fromUnit);
			if (lengthAndWidthImpl == null)
			{
				return 1.0;
			}
			LengthAndWidthImpl lengthAndWidthImpl2 = GlobalData.LengthWidthUnits.FirstOrDefault((LengthAndWidthImpl t) => t.UnitCode == toUnit);
			if (lengthAndWidthImpl2 == null)
			{
				return 1.0;
			}
			return Function.ParseDouble(lengthAndWidthImpl.SizeInMM / lengthAndWidthImpl2.SizeInMM);
		}
		case Enums.MeasureType.Weight:
		{
			WeightImpl weightImpl = GlobalData.WeightUnits.FirstOrDefault((WeightImpl t) => t.UnitCode == fromUnit);
			if (weightImpl == null)
			{
				return 1.0;
			}
			WeightImpl weightImpl2 = GlobalData.WeightUnits.FirstOrDefault((WeightImpl t) => t.UnitCode == toUnit);
			if (weightImpl2 == null)
			{
				return 1.0;
			}
			return Function.ParseDouble(weightImpl.WightInMG / weightImpl2.WightInMG);
		}
		default:
			return 1.0;
		}
	}

	public static int GetQuarter(this DateTime date)
	{
		if (date.Month >= 4 && date.Month <= 6)
		{
			return 2;
		}
		if (date.Month >= 7 && date.Month <= 9)
		{
			return 3;
		}
		if (date.Month >= 10 && date.Month <= 12)
		{
			return 4;
		}
		return 1;
	}
}
