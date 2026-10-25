using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.PM;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;

namespace Apzon.Controls.Form;

public class frm_FormatRule : XtraFormEx
{
	public TreeEx currentTreeView;

	public GridViewEx currentGridView;

	public PivotGridControlEx currentPivotView;

	private const string OkString = "OK";

	private const string SaveString = "Save";

	private bool init;

	private bool isadd;

	private Enums.FormViewModeType _formView;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPage xtraTabPage1;

	private LabelLine labelLine2;

	private CheckEditEx check_ApplyRow;

	private SimpleButton btn_Save;

	private SimpleButton btn_Cancel;

	private Label label1;

	private PropertyGridControl propertyGridControl1;

	private SimpleButton btn_Remove;

	private SimpleButton btn_Add;

	private ImageComboBoxEdit icb_Column;

	private SimpleButton btn_EditRule;

	private ListBoxControl lsb_Rule;

	private SimpleButton btn_CopyFormatRule;

	private ImageComboBoxEdit ic_Row;

	private LabelLine icb_Row;

	private ImageComboBoxEdit ic_Measure;

	private LabelLine labelLine1;

	private Enums.FormViewModeType FormView
	{
		get
		{
			return _formView;
		}
		set
		{
			try
			{
				_formView = value;
				switch (value)
				{
				case Enums.FormViewModeType.Add:
					btn_Save.Text = "Save";
					break;
				case Enums.FormViewModeType.View:
					btn_Save.Text = "OK";
					break;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		}
	}

	private TreeListFormatRule TreeCurrentCondition
	{
		get
		{
			if (lsb_Rule.SelectedItem != null)
			{
				return ((TreeItemExpressionFormatRule)lsb_Rule.SelectedItem).TreeListFormatRule;
			}
			return null;
		}
	}

	private GridFormatRule GridCurrentCondition
	{
		get
		{
			if (lsb_Rule.SelectedItem != null)
			{
				return ((GridItemExpressionFormatRule)lsb_Rule.SelectedItem).GridFormatRule;
			}
			return null;
		}
	}

	private PivotGridFormatRule PivotCurrentCondition
	{
		get
		{
			if (lsb_Rule.SelectedItem != null)
			{
				return ((PivotItemExpressionFormatRule)lsb_Rule.SelectedItem).PivotFormatRule;
			}
			return null;
		}
	}

	public frm_FormatRule()
	{
		InitializeComponent();
	}

	public void InitTree(TreeEx view)
	{
		currentTreeView = view;
		InitTreeConditions();
		InitTreeColumns();
		ic_Row.Visible = false;
		ic_Measure.Visible = false;
		icb_Row.Visible = false;
		labelLine1.Visible = false;
		FormView = Enums.FormViewModeType.View;
		if (lsb_Rule.Items.Count > 0)
		{
			lsb_Rule.SelectedIndex = 0;
		}
	}

	private void InitTreeColumns()
	{
		if (currentTreeView == null)
		{
			return;
		}
		icb_Column.Properties.Items.Add("", "", 0);
		foreach (TreeListColumnEx column in currentTreeView.Columns)
		{
			icb_Column.Properties.Items.Add(new ImageComboBoxItem(column.GetTextCaption(), column, -1));
		}
	}

	private void InitTreeConditions()
	{
		try
		{
			if (currentTreeView == null)
			{
				return;
			}
			lsb_Rule.BeginUpdate();
			try
			{
				lsb_Rule.Items.Clear();
				foreach (TreeListFormatRule formatRule in currentTreeView.FormatRules)
				{
					TreeItemExpressionFormatRule treeItemExpressionFormatRule = new TreeItemExpressionFormatRule(formatRule);
					if (treeItemExpressionFormatRule.IsExpressionCondition)
					{
						lsb_Rule.Items.Add(treeItemExpressionFormatRule);
					}
				}
			}
			finally
			{
				lsb_Rule.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorTree(TreeListFormatRule condition)
	{
		try
		{
			if (!(condition.Rule is FormatConditionRuleExpression { Expression: var expression } formatConditionRuleExpression))
			{
				return;
			}
			using ExpressionEditorForm expressionEditorForm = new FormatRuleExpressionEditorForm(formatConditionRuleExpression, null, formatConditionRuleExpression.Expression);
			expressionEditorForm.StartPosition = FormStartPosition.CenterParent;
			if (expressionEditorForm.ShowDialog(this) == DialogResult.OK)
			{
				formatConditionRuleExpression.Expression = expressionEditorForm.Expression;
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeleteTreeRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeleteTreeRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteTreeRuleNull()
	{
		try
		{
			currentTreeView.FormatRules.Remove(TreeCurrentCondition);
			lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
			SelectObjectTreeRule();
			FormView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorTree()
	{
		try
		{
			if (TreeCurrentCondition != null)
			{
				ShowEditorTree(TreeCurrentCondition);
				lsb_Rule.Refresh();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SelectObjectTreeRule()
	{
		try
		{
			init = true;
			if (TreeCurrentCondition == null)
			{
				propertyGridControl1.Enabled = false;
				propertyGridControl1.SelectedObject = null;
				propertyGridControl1.Visible = false;
			}
			else
			{
				propertyGridControl1.Enabled = true;
				propertyGridControl1.SelectedObject = ((FormatConditionRuleExpression)TreeCurrentCondition.Rule).Appearance;
				propertyGridControl1.Visible = true;
				check_ApplyRow.Checked = TreeCurrentCondition.ApplyToRow;
				icb_Column.EditValue = TreeCurrentCondition.Column;
			}
			init = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void InitGrid(GridViewEx view)
	{
		currentGridView = view;
		InitGridConditions();
		InitGridColumns();
		ic_Row.Visible = false;
		ic_Measure.Visible = false;
		icb_Row.Visible = false;
		labelLine1.Visible = false;
		FormView = Enums.FormViewModeType.View;
		if (lsb_Rule.Items.Count > 0)
		{
			lsb_Rule.SelectedIndex = 0;
		}
	}

	private void InitGridColumns()
	{
		if (currentGridView == null)
		{
			return;
		}
		icb_Column.Properties.Items.Add("", "", 0);
		foreach (GridColumnEx column in currentGridView.Columns)
		{
			icb_Column.Properties.Items.Add(new ImageComboBoxItem(column.GetTextCaption(), column, -1));
		}
	}

	private void InitGridConditions()
	{
		try
		{
			if (currentGridView == null)
			{
				return;
			}
			lsb_Rule.BeginUpdate();
			try
			{
				lsb_Rule.Items.Clear();
				foreach (GridFormatRule formatRule in currentGridView.FormatRules)
				{
					GridItemExpressionFormatRule gridItemExpressionFormatRule = new GridItemExpressionFormatRule(formatRule);
					if (gridItemExpressionFormatRule.IsExpressionCondition)
					{
						lsb_Rule.Items.Add(gridItemExpressionFormatRule);
					}
				}
			}
			finally
			{
				lsb_Rule.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorGrid(GridFormatRule condition)
	{
		try
		{
			if (!(condition.Rule is FormatConditionRuleExpression { Expression: var expression } formatConditionRuleExpression))
			{
				return;
			}
			using ExpressionEditorForm expressionEditorForm = new FormatRuleExpressionEditorForm(formatConditionRuleExpression, null, formatConditionRuleExpression.Expression);
			expressionEditorForm.StartPosition = FormStartPosition.CenterParent;
			if (expressionEditorForm.ShowDialog(this) == DialogResult.OK)
			{
				formatConditionRuleExpression.Expression = expressionEditorForm.Expression;
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeleteGridRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeleteGridRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteGridRuleNull()
	{
		try
		{
			currentGridView.FormatRules.Remove(GridCurrentCondition);
			lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
			SelectObjectGridRule();
			FormView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorGrid()
	{
		try
		{
			if (GridCurrentCondition != null)
			{
				ShowEditorGrid(GridCurrentCondition);
				lsb_Rule.Refresh();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SelectObjectGridRule()
	{
		try
		{
			init = true;
			if (GridCurrentCondition == null)
			{
				propertyGridControl1.Enabled = false;
				propertyGridControl1.SelectedObject = null;
				propertyGridControl1.Visible = false;
			}
			else
			{
				propertyGridControl1.Enabled = true;
				propertyGridControl1.SelectedObject = ((FormatConditionRuleExpression)GridCurrentCondition.Rule).Appearance;
				propertyGridControl1.Visible = true;
				check_ApplyRow.Checked = GridCurrentCondition.ApplyToRow;
				icb_Column.EditValue = GridCurrentCondition.Column;
			}
			init = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void InitPivot(PivotGridControlEx view)
	{
		currentPivotView = view;
		InitPivotConditions();
		InitPivotField();
		check_ApplyRow.Visible = false;
		FormView = Enums.FormViewModeType.View;
		if (lsb_Rule.Items.Count > 0)
		{
			lsb_Rule.SelectedIndex = 0;
		}
	}

	private void InitPivotField()
	{
		if (currentPivotView == null)
		{
			return;
		}
		icb_Column.Properties.Items.Add("", "", 0);
		ic_Measure.Properties.Items.Add("", "", 0);
		ic_Row.Properties.Items.Add("", "", 0);
		foreach (PivotGridField field in currentPivotView.Fields)
		{
			icb_Column.Properties.Items.Add(new ImageComboBoxItem(field.FieldName, field, -1));
			ic_Row.Properties.Items.Add(new ImageComboBoxItem(field.FieldName, field, -1));
			ic_Measure.Properties.Items.Add(new ImageComboBoxItem(field.FieldName, field, -1));
		}
	}

	private void InitPivotConditions()
	{
		try
		{
			if (currentPivotView == null)
			{
				return;
			}
			lsb_Rule.BeginUpdate();
			try
			{
				lsb_Rule.Items.Clear();
				foreach (PivotGridFormatRule formatRule in currentPivotView.FormatRules)
				{
					PivotItemExpressionFormatRule pivotItemExpressionFormatRule = new PivotItemExpressionFormatRule(formatRule);
					if (pivotItemExpressionFormatRule.IsExpressionCondition)
					{
						lsb_Rule.Items.Add(pivotItemExpressionFormatRule);
					}
				}
			}
			finally
			{
				lsb_Rule.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorPivot(PivotGridFormatRule condition)
	{
		try
		{
			if (!(condition.Rule is FormatConditionRuleExpression { Expression: var expression } formatConditionRuleExpression))
			{
				return;
			}
			using ExpressionEditorForm expressionEditorForm = new FormatRuleExpressionEditorForm(formatConditionRuleExpression, null, formatConditionRuleExpression.Expression);
			expressionEditorForm.StartPosition = FormStartPosition.CenterParent;
			if (expressionEditorForm.ShowDialog(this) == DialogResult.OK)
			{
				formatConditionRuleExpression.Expression = expressionEditorForm.Expression;
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeletePivotRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(expressionEditorForm.Expression))
				{
					DeletePivotRuleNull();
				}
				if (!string.Equals(expressionEditorForm.Expression, expression))
				{
					FormView = Enums.FormViewModeType.Add;
				}
				else
				{
					FormView = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeletePivotRuleNull()
	{
		try
		{
			currentPivotView.FormatRules.Remove(PivotCurrentCondition);
			lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
			SelectObjectPivotRule();
			FormView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEditorPivot()
	{
		try
		{
			if (PivotCurrentCondition != null)
			{
				ShowEditorPivot(PivotCurrentCondition);
				lsb_Rule.Refresh();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SelectObjectPivotRule()
	{
		try
		{
			init = true;
			if (PivotCurrentCondition == null)
			{
				propertyGridControl1.Enabled = false;
				propertyGridControl1.SelectedObject = null;
				propertyGridControl1.Visible = false;
			}
			else
			{
				propertyGridControl1.Enabled = true;
				propertyGridControl1.SelectedObject = ((FormatConditionRuleExpression)PivotCurrentCondition.Rule).Appearance;
				propertyGridControl1.Visible = true;
				if (!isadd)
				{
					icb_Column.EditValue = ((((FormatRuleFieldIntersectionSettings)PivotCurrentCondition.Settings).Column != null) ? ((FormatRuleFieldIntersectionSettings)PivotCurrentCondition.Settings).Column : null);
					ic_Measure.EditValue = PivotCurrentCondition.Measure;
					ic_Row.EditValue = ((((FormatRuleFieldIntersectionSettings)PivotCurrentCondition.Settings).Row != null) ? ((FormatRuleFieldIntersectionSettings)PivotCurrentCondition.Settings).Row : null);
				}
			}
			init = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FormatRule_Load(object sender, EventArgs e)
	{
	}

	private void btn_Add_Click(object sender, EventArgs e)
	{
		try
		{
			if (currentTreeView != null)
			{
				TreeListFormatRule treeListFormatRule = new TreeListFormatRule();
				treeListFormatRule.Rule = new FormatConditionRuleExpression();
				currentTreeView.FormatRules.Add(treeListFormatRule);
				int count = lsb_Rule.Items.Count;
				InitTreeConditions();
				lsb_Rule.SelectedIndex = count;
				ShowEditorTree();
			}
			if (currentGridView != null)
			{
				GridFormatRule gridFormatRule = new GridFormatRule();
				gridFormatRule.Rule = new FormatConditionRuleExpression();
				currentGridView.FormatRules.Add(gridFormatRule);
				int count2 = lsb_Rule.Items.Count;
				InitGridConditions();
				lsb_Rule.SelectedIndex = count2;
				ShowEditorGrid();
			}
			if (currentPivotView != null)
			{
				isadd = true;
				PivotGridFormatRule pivotGridFormatRule = new PivotGridFormatRule();
				pivotGridFormatRule.Rule = new FormatConditionRuleExpression();
				currentPivotView.FormatRules.Add(pivotGridFormatRule);
				int count3 = lsb_Rule.Items.Count;
				InitPivotConditions();
				lsb_Rule.SelectedIndex = count3;
				icb_Column.EditValue = null;
				ic_Row.EditValue = null;
				ic_Measure.EditValue = null;
				ShowEditorPivot();
				isadd = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			if (currentTreeView != null)
			{
				if (TreeCurrentCondition == null)
				{
					return;
				}
				currentTreeView.FormatRules.Remove(TreeCurrentCondition);
				lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
				SelectObjectTreeRule();
			}
			if (currentGridView != null)
			{
				if (GridCurrentCondition == null)
				{
					return;
				}
				currentGridView.FormatRules.Remove(GridCurrentCondition);
				lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
				SelectObjectGridRule();
			}
			if (currentPivotView != null)
			{
				if (PivotCurrentCondition == null)
				{
					return;
				}
				currentPivotView.FormatRules.Remove(PivotCurrentCondition);
				lsb_Rule.Items.RemoveAt(lsb_Rule.SelectedIndex);
				if (lsb_Rule.Items.Count < 1)
				{
					icb_Column.EditValue = null;
					ic_Row.EditValue = null;
					ic_Measure.EditValue = null;
				}
				SelectObjectPivotRule();
			}
			FormView = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditRule_Click(object sender, EventArgs e)
	{
		if (currentTreeView != null)
		{
			ShowEditorTree();
		}
		if (currentGridView != null)
		{
			ShowEditorGrid();
		}
		if (currentPivotView != null)
		{
			ShowEditorPivot();
		}
	}

	private void lsb_Rule_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (currentTreeView != null)
		{
			SelectObjectTreeRule();
		}
		if (currentGridView != null)
		{
			SelectObjectGridRule();
		}
		if (currentPivotView != null)
		{
			SelectObjectPivotRule();
		}
	}

	private void icb_Column_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (currentTreeView != null)
			{
				if (TreeCurrentCondition == null || init)
				{
					return;
				}
				TreeListColumnEx column = icb_Column.EditValue as TreeListColumnEx;
				TreeCurrentCondition.Column = column;
			}
			if (currentGridView != null)
			{
				if (GridCurrentCondition == null || init)
				{
					return;
				}
				GridColumnEx column2 = icb_Column.EditValue as GridColumnEx;
				GridCurrentCondition.Column = column2;
			}
			if (currentPivotView != null)
			{
				if (PivotCurrentCondition == null || init)
				{
					return;
				}
				PivotGridField column3 = icb_Column.EditValue as PivotGridField;
				PivotGridField row = ic_Row.EditValue as PivotGridField;
				PivotCurrentCondition.Settings = new FormatRuleFieldIntersectionSettings
				{
					Column = column3,
					Row = row
				};
			}
			FormView = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lsb_Rule_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (currentTreeView != null)
		{
			ShowEditorTree();
		}
		if (currentGridView != null)
		{
			ShowEditorGrid();
		}
		if (currentPivotView != null)
		{
			ShowEditorPivot();
		}
	}

	private void check_ApplyRow_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (currentTreeView != null)
			{
				if (TreeCurrentCondition == null || init)
				{
					return;
				}
				TreeCurrentCondition.ApplyToRow = check_ApplyRow.Checked;
			}
			if (currentGridView != null)
			{
				if (GridCurrentCondition == null || init)
				{
					return;
				}
				GridCurrentCondition.ApplyToRow = check_ApplyRow.Checked;
			}
			FormView = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			switch (FormView)
			{
			case Enums.FormViewModeType.Add:
			{
				WinCommons._OpenProcessing();
				List<FormatRule> lst = new List<FormatRule>();
				string fmrId = "";
				string itmId = "";
				string usgId = "";
				if (currentTreeView != null)
				{
					fmrId = Function.ToString(currentTreeView.FormId);
					itmId = Function.ToString(currentTreeView.ItemId);
					usgId = GlobalData.UserName;
					foreach (TreeListFormatRule formatRule4 in currentTreeView.FormatRules)
					{
						FormatRule formatRule = new FormatRule();
						formatRule.FormId = Function.ToString(currentTreeView.FormId);
						formatRule.ItemId = Function.ToString(currentTreeView.ItemId);
						formatRule.UserCode = GlobalData.UserName;
						if (string.Equals(Function.ToString(formatRule4.RuleType), "#FormatConditionRuleExpression"))
						{
							TreeItemExpressionFormatRule treeItemExpressionFormatRule = new TreeItemExpressionFormatRule(formatRule4);
							if (!string.IsNullOrEmpty(Function.ToString(formatRule4.Column)))
							{
								formatRule.ColumnId = Function.ToString(formatRule4.Column.FieldName);
							}
							else
							{
								formatRule.ColumnId = "";
							}
							formatRule.ApplyToRow = Function.ParseBool(formatRule4.ApplyToRow);
							formatRule.Expression = Function.ToString(treeItemExpressionFormatRule.Rule.Expression);
							formatRule.ExpressionId = 0;
							formatRule.BackColor = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.BackColor.Name);
							formatRule.BackColor2 = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.BackColor2.Name);
							formatRule.BorderColor = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.BorderColor.Name);
							formatRule.ForeColor = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.ForeColor.Name);
							formatRule.GradientMode = GradientModeValue(treeItemExpressionFormatRule.Rule.Appearance.GradientMode);
							formatRule.Image = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.Image);
							formatRule.FontSizeDelta = Function.ParseInt(treeItemExpressionFormatRule.Rule.Appearance.FontSizeDelta);
							formatRule.FontStyleDelta = FontStyleDeltaValue(treeItemExpressionFormatRule.Rule.Appearance.FontStyleDelta);
							formatRule.Font = Function.ToString(treeItemExpressionFormatRule.Rule.Appearance.Font.Name);
							formatRule.Bold = Function.ParseBool(treeItemExpressionFormatRule.Rule.Appearance.Font.Bold);
							formatRule.Italic = Function.ParseBool(treeItemExpressionFormatRule.Rule.Appearance.Font.Italic);
							formatRule.Size = Function.ParseInt(treeItemExpressionFormatRule.Rule.Appearance.Font.Size);
							formatRule.UnderLine = Function.ParseBool(treeItemExpressionFormatRule.Rule.Appearance.Font.Underline);
							formatRule.Strikeout = Function.ParseBool(treeItemExpressionFormatRule.Rule.Appearance.Font.Strikeout);
							formatRule.GdiCharSet = Function.ParseByte(treeItemExpressionFormatRule.Rule.Appearance.Font.GdiCharSet);
							formatRule.GdiVerticalFont = Function.ParseBool(treeItemExpressionFormatRule.Rule.Appearance.Font.GdiVerticalFont);
							formatRule.Unit = UnitValue(treeItemExpressionFormatRule.Rule.Appearance.Font.Unit);
							formatRule.HAlignment = HAlignmentValue(treeItemExpressionFormatRule.Rule.Appearance.TextOptions.HAlignment);
							formatRule.HotkeyPrefix = HotkeyPrefixValue(treeItemExpressionFormatRule.Rule.Appearance.TextOptions.HotkeyPrefix);
							formatRule.Trimming = TrimmingValue(treeItemExpressionFormatRule.Rule.Appearance.TextOptions.Trimming);
							formatRule.VAlignment = VertAlignmentValue(treeItemExpressionFormatRule.Rule.Appearance.TextOptions.VAlignment);
							formatRule.WordWrap = WordWrapValue(treeItemExpressionFormatRule.Rule.Appearance.TextOptions.WordWrap);
							lst.Add(formatRule);
						}
					}
				}
				if (currentGridView != null)
				{
					fmrId = Function.ToString(currentGridView.FormId);
					itmId = Function.ToString(currentGridView.ItemId);
					usgId = GlobalData.UserName;
					foreach (GridFormatRule formatRule5 in currentGridView.FormatRules)
					{
						FormatRule formatRule2 = new FormatRule();
						formatRule2.FormId = Function.ToString(currentGridView.FormId);
						formatRule2.ItemId = Function.ToString(currentGridView.ItemId);
						formatRule2.UserCode = GlobalData.UserName;
						if (string.Equals(Function.ToString(formatRule5.RuleType), "#FormatConditionRuleExpression"))
						{
							GridItemExpressionFormatRule gridItemExpressionFormatRule = new GridItemExpressionFormatRule(formatRule5);
							if (!string.IsNullOrEmpty(Function.ToString(formatRule5.Column)))
							{
								formatRule2.ColumnId = Function.ToString(formatRule5.Column.FieldName);
							}
							else
							{
								formatRule2.ColumnId = "";
							}
							formatRule2.ApplyToRow = Function.ParseBool(formatRule5.ApplyToRow);
							formatRule2.Expression = Function.ToString(gridItemExpressionFormatRule.Rule.Expression);
							formatRule2.ExpressionId = 0;
							formatRule2.BackColor = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.BackColor.Name);
							formatRule2.BackColor2 = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.BackColor2.Name);
							formatRule2.BorderColor = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.BorderColor.Name);
							formatRule2.ForeColor = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.ForeColor.Name);
							formatRule2.GradientMode = GradientModeValue(gridItemExpressionFormatRule.Rule.Appearance.GradientMode);
							formatRule2.Image = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.Image);
							formatRule2.FontSizeDelta = Function.ParseInt(gridItemExpressionFormatRule.Rule.Appearance.FontSizeDelta);
							formatRule2.FontStyleDelta = FontStyleDeltaValue(gridItemExpressionFormatRule.Rule.Appearance.FontStyleDelta);
							formatRule2.Font = Function.ToString(gridItemExpressionFormatRule.Rule.Appearance.Font.Name);
							formatRule2.Bold = Function.ParseBool(gridItemExpressionFormatRule.Rule.Appearance.Font.Bold);
							formatRule2.Italic = Function.ParseBool(gridItemExpressionFormatRule.Rule.Appearance.Font.Italic);
							formatRule2.Size = Function.ParseInt(gridItemExpressionFormatRule.Rule.Appearance.Font.Size);
							formatRule2.UnderLine = Function.ParseBool(gridItemExpressionFormatRule.Rule.Appearance.Font.Underline);
							formatRule2.Strikeout = Function.ParseBool(gridItemExpressionFormatRule.Rule.Appearance.Font.Strikeout);
							formatRule2.GdiCharSet = Function.ParseByte(gridItemExpressionFormatRule.Rule.Appearance.Font.GdiCharSet);
							formatRule2.GdiVerticalFont = Function.ParseBool(gridItemExpressionFormatRule.Rule.Appearance.Font.GdiVerticalFont);
							formatRule2.Unit = UnitValue(gridItemExpressionFormatRule.Rule.Appearance.Font.Unit);
							formatRule2.HAlignment = HAlignmentValue(gridItemExpressionFormatRule.Rule.Appearance.TextOptions.HAlignment);
							formatRule2.HotkeyPrefix = HotkeyPrefixValue(gridItemExpressionFormatRule.Rule.Appearance.TextOptions.HotkeyPrefix);
							formatRule2.Trimming = TrimmingValue(gridItemExpressionFormatRule.Rule.Appearance.TextOptions.Trimming);
							formatRule2.VAlignment = VertAlignmentValue(gridItemExpressionFormatRule.Rule.Appearance.TextOptions.VAlignment);
							formatRule2.WordWrap = WordWrapValue(gridItemExpressionFormatRule.Rule.Appearance.TextOptions.WordWrap);
							lst.Add(formatRule2);
						}
					}
				}
				if (lst.Count > 0)
				{
					using HttpClientHelper<FormatRule> helper = new HttpClientHelper<FormatRule>(Configuration.ApiServer, "FormatRule");
					string text = await helper.PostParamsAsJsonAsync("Saves", lst);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormView = Enums.FormViewModeType.View;
					}
				}
				else
				{
					using HttpClientHelper<FormatRule> helper = new HttpClientHelper<FormatRule>(Configuration.ApiServer, "FormatRule");
					string text2 = await helper.PostParamsAsJsonAsync("Delete", new Tuple<string, string, string>(fmrId, itmId, usgId));
					if (!string.IsNullOrEmpty(text2))
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormView = Enums.FormViewModeType.View;
					}
				}
				if (currentPivotView != null)
				{
					fmrId = Function.ToString(currentPivotView.FormId);
					itmId = Function.ToString(currentPivotView.ItemId);
					usgId = GlobalData.UserName;
					foreach (PivotGridFormatRule formatRule6 in currentPivotView.FormatRules)
					{
						FormatRule formatRule3 = new FormatRule();
						formatRule3.FormId = Function.ToString(currentPivotView.FormId);
						formatRule3.ItemId = Function.ToString(currentPivotView.ItemId);
						formatRule3.UserCode = GlobalData.UserName;
						if (string.Equals(Function.ToString(formatRule6.RuleType), "#FormatConditionRuleExpression"))
						{
							PivotItemExpressionFormatRule pivotItemExpressionFormatRule = new PivotItemExpressionFormatRule(formatRule6);
							if (!string.IsNullOrEmpty(Function.ToString(icb_Column.EditValue)))
							{
								formatRule3.ColumnId = Function.ToString(icb_Column.EditValue);
							}
							else
							{
								formatRule3.ColumnId = "";
							}
							if (!string.IsNullOrEmpty(Function.ToString(ic_Measure.EditValue)))
							{
								formatRule3.Measure = Function.ToString(ic_Measure.EditValue);
							}
							else
							{
								formatRule3.Measure = "";
							}
							if (!string.IsNullOrEmpty(Function.ToString(ic_Row.EditValue)))
							{
								formatRule3.RowId = Function.ToString(ic_Row.EditValue);
							}
							else
							{
								formatRule3.RowId = "";
							}
							formatRule3.Expression = Function.ToString(pivotItemExpressionFormatRule.Rule.Expression);
							formatRule3.ExpressionId = 0;
							formatRule3.BackColor = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.BackColor.Name);
							formatRule3.BackColor2 = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.BackColor2.Name);
							formatRule3.BorderColor = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.BorderColor.Name);
							formatRule3.ForeColor = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.ForeColor.Name);
							formatRule3.GradientMode = GradientModeValue(pivotItemExpressionFormatRule.Rule.Appearance.GradientMode);
							formatRule3.Image = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.Image);
							formatRule3.FontSizeDelta = Function.ParseInt(pivotItemExpressionFormatRule.Rule.Appearance.FontSizeDelta);
							formatRule3.FontStyleDelta = FontStyleDeltaValue(pivotItemExpressionFormatRule.Rule.Appearance.FontStyleDelta);
							formatRule3.Font = Function.ToString(pivotItemExpressionFormatRule.Rule.Appearance.Font.Name);
							formatRule3.Bold = Function.ParseBool(pivotItemExpressionFormatRule.Rule.Appearance.Font.Bold);
							formatRule3.Italic = Function.ParseBool(pivotItemExpressionFormatRule.Rule.Appearance.Font.Italic);
							formatRule3.Size = Function.ParseInt(pivotItemExpressionFormatRule.Rule.Appearance.Font.Size);
							formatRule3.UnderLine = Function.ParseBool(pivotItemExpressionFormatRule.Rule.Appearance.Font.Underline);
							formatRule3.Strikeout = Function.ParseBool(pivotItemExpressionFormatRule.Rule.Appearance.Font.Strikeout);
							formatRule3.GdiCharSet = Function.ParseByte(pivotItemExpressionFormatRule.Rule.Appearance.Font.GdiCharSet);
							formatRule3.GdiVerticalFont = Function.ParseBool(pivotItemExpressionFormatRule.Rule.Appearance.Font.GdiVerticalFont);
							formatRule3.Unit = UnitValue(pivotItemExpressionFormatRule.Rule.Appearance.Font.Unit);
							formatRule3.HAlignment = HAlignmentValue(pivotItemExpressionFormatRule.Rule.Appearance.TextOptions.HAlignment);
							formatRule3.HotkeyPrefix = HotkeyPrefixValue(pivotItemExpressionFormatRule.Rule.Appearance.TextOptions.HotkeyPrefix);
							formatRule3.Trimming = TrimmingValue(pivotItemExpressionFormatRule.Rule.Appearance.TextOptions.Trimming);
							formatRule3.VAlignment = VertAlignmentValue(pivotItemExpressionFormatRule.Rule.Appearance.TextOptions.VAlignment);
							formatRule3.WordWrap = WordWrapValue(pivotItemExpressionFormatRule.Rule.Appearance.TextOptions.WordWrap);
							lst.Add(formatRule3);
						}
					}
				}
				if (lst.Count > 0)
				{
					using HttpClientHelper<FormatRule> helper = new HttpClientHelper<FormatRule>(Configuration.ApiServer, "FormatRule");
					string text3 = await helper.PostParamsAsJsonAsync("Saves", lst);
					if (!string.IsNullOrEmpty(text3))
					{
						WinCommons.Error(Enums.MessageCode.None, text3);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormView = Enums.FormViewModeType.View;
					}
				}
				else
				{
					using HttpClientHelper<FormatRule> helper = new HttpClientHelper<FormatRule>(Configuration.ApiServer, "FormatRule");
					string text4 = await helper.PostParamsAsJsonAsync("Delete", new Tuple<string, string, string>(fmrId, itmId, usgId));
					if (!string.IsNullOrEmpty(text4))
					{
						WinCommons.Error(Enums.MessageCode.None, text4);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormView = Enums.FormViewModeType.View;
					}
				}
				WinCommons._CloseProcessing();
				break;
			}
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void propertyGridControl1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormView = Enums.FormViewModeType.Add;
	}

	private void btn_CopyFormatRule_Click(object sender, EventArgs e)
	{
		try
		{
			if (currentTreeView != null)
			{
				frm_FormatRule_Copy frm_FormatRule_Copy2 = new frm_FormatRule_Copy();
				frm_FormatRule_Copy2.Item = new Tuple<TreeEx, string>(currentTreeView, currentTreeView.ItemId);
				frm_FormatRule_Copy2.FormId = currentTreeView.FormId;
				frm_FormatRule_Copy2.UserName = GlobalData.UserName;
				frm_FormatRule_Copy2.ShowDialog();
			}
			if (currentGridView != null)
			{
				frm_FormatRule_Copy frm_FormatRule_Copy3 = new frm_FormatRule_Copy();
				frm_FormatRule_Copy3.ItemGrid = new Tuple<GridViewEx, string>(currentGridView, currentGridView.ItemId);
				frm_FormatRule_Copy3.FormId = currentGridView.FormId;
				frm_FormatRule_Copy3.UserName = GlobalData.UserName;
				frm_FormatRule_Copy3.ShowDialog();
			}
			if (currentPivotView != null)
			{
				frm_FormatRule_Copy frm_FormatRule_Copy4 = new frm_FormatRule_Copy();
				frm_FormatRule_Copy4.ItemPivot = new Tuple<PivotGridControlEx, string>(currentPivotView, currentPivotView.ItemId);
				frm_FormatRule_Copy4.FormId = currentPivotView.FormId;
				frm_FormatRule_Copy4.UserName = GlobalData.UserName;
				frm_FormatRule_Copy4.ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ic_Measure_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (currentPivotView != null)
			{
				if (PivotCurrentCondition == null || init)
				{
					return;
				}
				PivotGridField measure = ic_Measure.EditValue as PivotGridField;
				PivotCurrentCondition.Measure = measure;
			}
			FormView = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ic_Row_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (currentPivotView != null)
			{
				if (PivotCurrentCondition == null || init)
				{
					return;
				}
				PivotGridField column = icb_Column.EditValue as PivotGridField;
				PivotGridField row = ic_Row.EditValue as PivotGridField;
				PivotCurrentCondition.Settings = new FormatRuleFieldIntersectionSettings
				{
					Column = column,
					Row = row
				};
			}
			FormView = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int WordWrapValue(WordWrap value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				WordWrap.NoWrap => 1, 
				WordWrap.Wrap => 2, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int VertAlignmentValue(VertAlignment value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				VertAlignment.Top => 1, 
				VertAlignment.Center => 2, 
				VertAlignment.Bottom => 3, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int TrimmingValue(Trimming value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				Trimming.None => 1, 
				Trimming.Character => 2, 
				Trimming.Word => 3, 
				Trimming.EllipsisCharacter => 4, 
				Trimming.EllipsisWord => 5, 
				Trimming.EllipsisPath => 6, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int HotkeyPrefixValue(HKeyPrefix value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				HKeyPrefix.None => 1, 
				HKeyPrefix.Show => 2, 
				HKeyPrefix.Hide => 3, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int HAlignmentValue(HorzAlignment value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				HorzAlignment.Near => 1, 
				HorzAlignment.Center => 2, 
				HorzAlignment.Far => 3, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int UnitValue(GraphicsUnit value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				GraphicsUnit.Display => 1, 
				GraphicsUnit.Pixel => 2, 
				GraphicsUnit.Point => 3, 
				GraphicsUnit.Inch => 4, 
				GraphicsUnit.Document => 5, 
				GraphicsUnit.Millimeter => 6, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int FontStyleDeltaValue(FontStyle value)
	{
		try
		{
			int num = 0;
			return value switch
			{
				FontStyle.Bold => 1, 
				FontStyle.Italic => 2, 
				FontStyle.Underline => 3, 
				FontStyle.Strikeout => 4, 
				_ => 0, 
			};
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private int GradientModeValue(LinearGradientMode value)
	{
		try
		{
			int result = 0;
			switch (value)
			{
			case LinearGradientMode.Horizontal:
				result = 0;
				break;
			case LinearGradientMode.Vertical:
				result = 1;
				break;
			case LinearGradientMode.ForwardDiagonal:
				result = 2;
				break;
			case LinearGradientMode.BackwardDiagonal:
				result = 3;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.frm_FormatRule));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
		this.lsb_Rule = new DevExpress.XtraEditors.ListBoxControl();
		this.btn_EditRule = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Remove = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Add = new DevExpress.XtraEditors.SimpleButton();
		this.label1 = new System.Windows.Forms.Label();
		this.propertyGridControl1 = new DevExpress.XtraVerticalGrid.PropertyGridControl();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.check_ApplyRow = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.icb_Column = new DevExpress.XtraEditors.ImageComboBoxEdit();
		this.btn_CopyFormatRule = new DevExpress.XtraEditors.SimpleButton();
		this.ic_Row = new DevExpress.XtraEditors.ImageComboBoxEdit();
		this.icb_Row = new Apzon.Controls.Controls.LabelLine();
		this.ic_Measure = new DevExpress.XtraEditors.ImageComboBoxEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lsb_Rule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.propertyGridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_ApplyRow.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.icb_Column.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ic_Row.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ic_Measure.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.label1);
		this.splitContainerControl1.Panel2.Controls.Add(this.propertyGridControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(972, 554);
		this.splitContainerControl1.SplitterPosition = 241;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(972, 241);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.lsb_Rule);
		this.xtraTabPage1.Controls.Add(this.btn_EditRule);
		this.xtraTabPage1.Controls.Add(this.btn_Remove);
		this.xtraTabPage1.Controls.Add(this.btn_Add);
		this.xtraTabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(970, 211);
		this.xtraTabPage1.Text = "Format Condition Rule Expression";
		this.lsb_Rule.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lsb_Rule.Cursor = System.Windows.Forms.Cursors.Default;
		this.lsb_Rule.Location = new System.Drawing.Point(0, 41);
		this.lsb_Rule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.lsb_Rule.Name = "lsb_Rule";
		this.lsb_Rule.Size = new System.Drawing.Size(967, 166);
		this.lsb_Rule.TabIndex = 12;
		this.lsb_Rule.SelectedIndexChanged += new System.EventHandler(this.lsb_Rule_SelectedIndexChanged);
		this.lsb_Rule.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lsb_Rule_MouseDoubleClick);
		this.btn_EditRule.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditRule.ImageOptions.Image");
		this.btn_EditRule.Location = new System.Drawing.Point(70, 4);
		this.btn_EditRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_EditRule.Name = "btn_EditRule";
		this.btn_EditRule.Size = new System.Drawing.Size(142, 30);
		this.btn_EditRule.TabIndex = 11;
		this.btn_EditRule.Text = "Expression Editor";
		this.btn_EditRule.Click += new System.EventHandler(this.btn_EditRule_Click);
		this.btn_Remove.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Remove.ImageOptions.Image");
		this.btn_Remove.Location = new System.Drawing.Point(37, 4);
		this.btn_Remove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Remove.Name = "btn_Remove";
		this.btn_Remove.Size = new System.Drawing.Size(26, 30);
		this.btn_Remove.TabIndex = 8;
		this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.Location = new System.Drawing.Point(4, 4);
		this.btn_Add.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.Size = new System.Drawing.Size(27, 30);
		this.btn_Add.TabIndex = 7;
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(4, 5);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(192, 16);
		this.label1.TabIndex = 1;
		this.label1.Text = "Appearance  for the current rule";
		this.propertyGridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.propertyGridControl1.Cursor = System.Windows.Forms.Cursors.Default;
		this.propertyGridControl1.Location = new System.Drawing.Point(1, 25);
		this.propertyGridControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.propertyGridControl1.Name = "propertyGridControl1";
		this.propertyGridControl1.OptionsView.MinRowAutoHeight = 12;
		this.propertyGridControl1.Size = new System.Drawing.Size(967, 281);
		this.propertyGridControl1.TabIndex = 0;
		this.propertyGridControl1.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.propertyGridControl1_CellValueChanged);
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Colunm";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 604);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(61, 25);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "Colunm";
		this.check_ApplyRow.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.check_ApplyRow.EditControl = null;
		this.check_ApplyRow.FieldName = null;
		this.check_ApplyRow.IsSearchControl = false;
		this.check_ApplyRow.Location = new System.Drawing.Point(61, 636);
		this.check_ApplyRow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.check_ApplyRow.Name = "check_ApplyRow";
		this.check_ApplyRow.Properties.Caption = "Apply To Row";
		this.check_ApplyRow.SettingInVisible = false;
		this.check_ApplyRow.SettingReadonly = false;
		this.check_ApplyRow.Size = new System.Drawing.Size(112, 24);
		this.check_ApplyRow.TabIndex = 5;
		this.check_ApplyRow.CheckedChanged += new System.EventHandler(this.check_ApplyRow_CheckedChanged);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.Location = new System.Drawing.Point(71, 769);
		this.btn_Save.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(91, 30);
		this.btn_Save.TabIndex = 6;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(169, 769);
		this.btn_Cancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(91, 30);
		this.btn_Cancel.TabIndex = 7;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.icb_Column.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.icb_Column.Location = new System.Drawing.Point(61, 601);
		this.icb_Column.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.icb_Column.Name = "icb_Column";
		this.icb_Column.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.icb_Column.Size = new System.Drawing.Size(895, 22);
		this.icb_Column.TabIndex = 8;
		this.icb_Column.SelectedIndexChanged += new System.EventHandler(this.icb_Column_SelectedIndexChanged);
		this.btn_CopyFormatRule.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyFormatRule.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyFormatRule.ImageOptions.Image");
		this.btn_CopyFormatRule.Location = new System.Drawing.Point(696, 769);
		this.btn_CopyFormatRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_CopyFormatRule.Name = "btn_CopyFormatRule";
		this.btn_CopyFormatRule.Size = new System.Drawing.Size(270, 30);
		this.btn_CopyFormatRule.TabIndex = 9;
		this.btn_CopyFormatRule.Text = "Copy Format Condition Rule Expression";
		this.btn_CopyFormatRule.Click += new System.EventHandler(this.btn_CopyFormatRule_Click);
		this.ic_Row.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ic_Row.Location = new System.Drawing.Point(59, 720);
		this.ic_Row.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.ic_Row.Name = "ic_Row";
		this.ic_Row.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ic_Row.Size = new System.Drawing.Size(895, 22);
		this.ic_Row.TabIndex = 17;
		this.ic_Row.SelectedIndexChanged += new System.EventHandler(this.ic_Row_SelectedIndexChanged);
		this.icb_Row.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.icb_Row.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.icb_Row.Appearance.Options.UseFont = true;
		this.icb_Row.EditControl = null;
		this.icb_Row.EditValue = "Row ";
		this.icb_Row.FieldName = null;
		this.icb_Row.HideWhenEmpty = false;
		this.icb_Row.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.icb_Row.Location = new System.Drawing.Point(2, 724);
		this.icb_Row.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
		this.icb_Row.Name = "icb_Row";
		this.icb_Row.ReadOnly = false;
		this.icb_Row.SettingInVisible = false;
		this.icb_Row.SettingReadonly = false;
		this.icb_Row.ShowUnderLine = true;
		this.icb_Row.Size = new System.Drawing.Size(61, 25);
		this.icb_Row.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.icb_Row.TabIndex = 16;
		this.icb_Row.Utext = "Row ";
		this.ic_Measure.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ic_Measure.Location = new System.Drawing.Point(59, 663);
		this.ic_Measure.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.ic_Measure.Name = "ic_Measure";
		this.ic_Measure.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ic_Measure.Size = new System.Drawing.Size(895, 22);
		this.ic_Measure.TabIndex = 15;
		this.ic_Measure.SelectedIndexChanged += new System.EventHandler(this.ic_Measure_SelectedIndexChanged);
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Measure ";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 667);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(61, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 14;
		this.labelLine1.Utext = "Measure ";
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(972, 799);
		base.Controls.Add(this.ic_Row);
		base.Controls.Add(this.icb_Row);
		base.Controls.Add(this.ic_Measure);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_CopyFormatRule);
		base.Controls.Add(this.icb_Column);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.check_ApplyRow);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.splitContainerControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oFormatRule";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_FormatRule.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		base.Name = "frm_FormatRule";
		this.Text = "Format  Rule";
		base.Load += new System.EventHandler(this.FormatRule_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lsb_Rule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.propertyGridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_ApplyRow.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.icb_Column.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ic_Row.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ic_Measure.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
