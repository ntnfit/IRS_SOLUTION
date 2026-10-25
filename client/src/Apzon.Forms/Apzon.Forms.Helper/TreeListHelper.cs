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
using Apzon.Controls.Tree;
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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using SAPbobsCOM;

namespace Apzon.Forms.Helper;

public static class TreeListHelper
{
	public static int dfWidth = 100;

	public static bool dfVisible = false;

	public static bool dfReadOnly = false;

	public static bool dfAllowEdit = true;

	public static bool dfAllowMove = true;

	public static bool dfAllowSize = true;

	public static bool dfAllowSort = true;

	public static bool fixedColor = true;

	public static bool dfAutoHeightHeader = true;

	public static Color fatherColor = Color.FromArgb(215, 239, 250);

	public static async Task GenerateTreeList(TreeEx tre, DataSet ds, List<UserFieldsImpl> lstUDF = null, bool expand = false, bool filterRow = false, bool styleRow = true)
	{
		_ = 2;
		try
		{
			if (ds.IsHasStructure())
			{
				if (tre.Columns.Count > 0 && tre.Columns.Count == ds.Tables[1].Columns.Count)
				{
					tre.DataSource = ds.Tables[1];
					tre.RefreshDataSource();
					if (expand)
					{
						tre.ExpandAll();
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
							catch (Exception ex)
							{
								WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
							}
						}
					}
				}
				else if (!ds.IsHasValidValue())
				{
					await GenerateTreeList(tre, ds.Tables[0], ds.Tables[1], null, lstUDF, expand, styleRow);
				}
				else
				{
					await GenerateTreeList(tre, ds.Tables[0], ds.Tables[1], ds.Tables[2], lstUDF, expand, styleRow);
				}
			}
			else if (ds.IsNotNull())
			{
				if (tre.Columns.Count > 0 && tre.Columns.Count == ds.Tables[0].Columns.Count)
				{
					tre.DataSource = ds.Tables[0];
					tre.RefreshDataSource();
				}
				else
				{
					tre.BeginUpdate();
					tre.OptionsView.AutoWidth = ds.Tables[0].Columns.Count < 7;
					tre.OptionsFilter.AllowFilterEditor = true;
					tre.OptionsBehavior.EnableFiltering = true;
					tre.OptionsNavigation.UseTabKey = true;
					tre.OptionsNavigation.AutoMoveRowFocus = true;
					tre.OptionsNavigation.AutoFocusNewNode = true;
					tre.OptionsFilter.FilterMode = FilterMode.Matches;
					tre.OptionsFind.AllowFindPanel = true;
					tre.BestFitVisibleOnly = true;
					tre.OptionsMenu.ShowConditionalFormattingItem = true;
					foreach (DataColumn column in ds.Tables[0].Columns)
					{
						TreeListColumnEx treeListColumnEx = tre.ColumnsEx.AddEx();
						treeListColumnEx.Name = "cl_" + column.ColumnName;
						treeListColumnEx.FieldName = column.ColumnName;
						treeListColumnEx.Caption = column.Caption;
						treeListColumnEx.Visible = true;
						treeListColumnEx.OptionsFilter.AllowFilter = true;
						treeListColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
						treeListColumnEx.OptionsColumn.AllowSort = false;
					}
					if (!tre.OptionsView.ShowColumns)
					{
						tre.OptionsView.ShowColumns = true;
					}
					tre.DataSource = ds.Tables[0];
					tre.RefreshDataSource();
					await tre.ReloadFormSetting();
					tre.EndUpdate();
				}
				if (expand)
				{
					tre.ExpandAll();
				}
			}
			if (filterRow)
			{
				tre.OptionsView.ShowAutoFilterRow = true;
			}
			if (dfAutoHeightHeader)
			{
				tre.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public static async Task GenerateTreeList(TreeEx tre, DataTable dtStructure, DataTable dt, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, bool expand = true, bool styleRow = true)
	{
		try
		{
			tre.BeginUpdate();
			tre.DataSource = null;
			tre.Columns.Clear();
			tre.OptionsView.AutoWidth = dtStructure.IsNotNull() && dtStructure.Rows[0].IsExistsColumn("Visible") && dtStructure.Select("Visible = true").Length < 7;
			tre.BestFitVisibleOnly = true;
			tre.OptionsFilter.AllowFilterEditor = true;
			tre.OptionsBehavior.EnableFiltering = true;
			tre.OptionsNavigation.UseTabKey = true;
			tre.OptionsNavigation.AutoMoveRowFocus = true;
			tre.OptionsNavigation.AutoFocusNewNode = true;
			tre.OptionsFilter.FilterMode = FilterMode.Matches;
			tre.OptionsFind.AllowFindPanel = true;
			tre.OptionsFind.FindNullPrompt = "Nhập để tìm kiếm...";
			tre.OptionsMenu.ShowConditionalFormattingItem = true;
			if (dt == null || dt.Columns.Count <= 0)
			{
				await GenerateColumn(tre, dtStructure, dtValidValue, lstUDF);
			}
			else
			{
				int count = dt.Rows.Count;
				if (count > 9999)
				{
					tre.IndicatorWidth = 55;
				}
				else if (count > 999)
				{
					tre.IndicatorWidth = 45;
				}
				else
				{
					tre.IndicatorWidth = 35;
				}
				int idx = 0;
				string filterString = "";
				int i;
				for (i = 0; i < dt.Columns.Count; i++)
				{
					TreeListColumnEx treeListColumnEx = tre.ColumnsEx.AddEx();
					treeListColumnEx.FieldName = dt.Columns[i].ColumnName;
					treeListColumnEx.OptionsFilter.AllowFilter = true;
					treeListColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
					treeListColumnEx.OptionsColumn.AllowSort = false;
					DataRow dataRow = ((dtStructure.IsNotNull() && dtStructure.Rows[0].IsExistsColumn("ColumnName")) ? (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(dt.Columns[i].ColumnName)
						select c).FirstOrDefault() : null);
					if (dataRow != null)
					{
						treeListColumnEx.Caption = (dataRow.IsExistsColumn("Caption") ? Function.ToString(dataRow["Caption"]) : dt.Columns[i].ColumnName);
						treeListColumnEx.ToolTip = (dataRow.IsExistsColumn("Caption") ? Function.ToString(dataRow["Caption"]) : dt.Columns[i].ColumnName);
						treeListColumnEx.Width = (dataRow.IsExistsColumn("Width") ? Function.ParseInt(dataRow["Width"]) : dfWidth);
						if (dataRow.IsExistsColumn("Visible"))
						{
							treeListColumnEx.Visible = Function.ParseBool(dataRow["Visible"]);
						}
						else
						{
							treeListColumnEx.Visible = dfVisible;
						}
						treeListColumnEx.VisibleIndex = (treeListColumnEx.Visible ? idx : (-1));
						idx = (treeListColumnEx.Visible ? (idx + 1) : idx);
						treeListColumnEx.OptionsColumn.ReadOnly = (dataRow.IsExistsColumn("ReadOnly") ? Function.ParseBool(dataRow["ReadOnly"]) : dfReadOnly);
						treeListColumnEx.OptionsColumn.AllowEdit = (dataRow.IsExistsColumn("AllowEdit") ? Function.ParseBool(dataRow["AllowEdit"]) : dfAllowEdit);
						treeListColumnEx.OptionsColumn.AllowMove = (dataRow.IsExistsColumn("AllowMove") ? Function.ParseBool(dataRow["AllowMove"]) : dfAllowMove);
						treeListColumnEx.OptionsColumn.AllowSize = (dataRow.IsExistsColumn("AllowSize") ? Function.ParseBool(dataRow["AllowSize"]) : dfAllowSize);
						treeListColumnEx.OptionsColumn.AllowSort = (dataRow.IsExistsColumn("AllowSort") ? Function.ParseBool(dataRow["AllowSort"]) : dfAllowSort);
						if (dataRow.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(dataRow["FormatType"])))
						{
							string text = Function.ToString(dataRow["FormatType"]).ToLower();
							if (!(text == "numeric"))
							{
								if (text == "datetime")
								{
									treeListColumnEx.Format.FormatType = FormatType.DateTime;
								}
								else
								{
									treeListColumnEx.Format.FormatType = FormatType.None;
								}
							}
							else
							{
								treeListColumnEx.Format.FormatType = FormatType.Numeric;
							}
						}
						if (dataRow.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["DisplayFormat"])))
						{
							if (Function.ParseInt(dataRow["DisplayFormat"]) > 0)
							{
								treeListColumnEx.DisplayFormatType = (Enums.DisplayFormatType)Function.ParseInt(dataRow["DisplayFormat"]);
							}
							else
							{
								treeListColumnEx.Format.FormatString = Function.ToString(dataRow["DisplayFormat"]);
							}
						}
						if (dataRow.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(dataRow["Fixed"])))
						{
							string text = Function.ToString(dataRow["Fixed"]);
							if (!(text == "L"))
							{
								if (text == "R")
								{
									treeListColumnEx.Fixed = FixedStyle.Right;
								}
							}
							else
							{
								treeListColumnEx.Fixed = FixedStyle.Left;
							}
						}
						string format = treeListColumnEx.Format.FormatString;
						if (dataRow.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryFormat"])))
						{
							format = Function.ToString(dataRow["SummaryFormat"]);
						}
						if (dataRow.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(dataRow["SummaryType"])))
						{
							tre.OptionsView.ShowSummaryFooter = true;
							GetSummary(treeListColumnEx, Function.ToString(dataRow["SummaryType"]).ToLower(), format);
						}
						if (dataRow.IsExistsColumn("Parent") && Function.ParseBool(dataRow["Parent"]))
						{
							tre.ParentFieldName = dt.Columns[i].ColumnName;
						}
						if (dataRow.IsExistsColumn("Key") && Function.ParseBool(dataRow["Key"]))
						{
							tre.KeyFieldName = dt.Columns[i].ColumnName;
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
							treeListColumnEx.AppearanceCell.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
							treeListColumnEx.AppearanceHeader.BackColor = Color.FromName(Function.ToString(dataRow["BackColor"]));
						}
						if (dataRow.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(dataRow["ForeColor"])))
						{
							treeListColumnEx.AppearanceCell.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
							treeListColumnEx.AppearanceHeader.ForeColor = Color.FromName(Function.ToString(dataRow["ForeColor"]));
						}
						if (dataRow.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(dataRow["FilterString"])))
						{
							filterString = filterString + Function.ToString(dataRow["FilterString"]) + " ";
						}
					}
					else
					{
						treeListColumnEx.Caption = dt.Columns[i].ColumnName;
						treeListColumnEx.Width = dfWidth;
						treeListColumnEx.Visible = dfVisible;
						treeListColumnEx.VisibleIndex = (treeListColumnEx.Visible ? idx : (-1));
						idx = (treeListColumnEx.Visible ? (idx + 1) : idx);
						treeListColumnEx.OptionsColumn.ReadOnly = dfReadOnly;
						treeListColumnEx.OptionsColumn.AllowEdit = dfAllowEdit;
						treeListColumnEx.OptionsColumn.AllowMove = dfAllowMove;
						treeListColumnEx.OptionsColumn.AllowSize = dfAllowSize;
						treeListColumnEx.OptionsColumn.AllowSort = dfAllowSort;
					}
					await LoadUdfColumns(tre, treeListColumnEx, lstUDF, dataRow, dtValidValue, dt);
				}
				if (!tre.OptionsView.ShowColumns)
				{
					tre.OptionsView.ShowColumns = true;
				}
				tre.DataSource = dt;
				tre.RefreshDataSource();
				if (dtStructure.IsExistsColumn("VisIndex"))
				{
					foreach (DataRow row in dtStructure.Rows)
					{
						string colname = Function.ToString(row["ColumnName"]);
						if (!colname.Equals(tre.ParentFieldName) && !colname.Equals(tre.KeyFieldName) && tre.Columns.Any((TreeListColumn m) => m.FieldName.Equals(colname)) && tre.Columns[colname].Visible)
						{
							int visibleIndex = Function.ParseInt(row["VisIndex"]);
							tre.Columns[colname].VisibleIndex = visibleIndex;
						}
					}
				}
				tre.ActiveFilterString = filterString;
			}
			if (expand)
			{
				tre.ExpandAll();
			}
			tre.CustomDrawNodeIndicator += tre_CustomDrawNodeIndicator;
			if (fixedColor && styleRow)
			{
				tre.NodeCellStyle += tree_NodeCellStyle;
				tre.Appearance.FocusedRow.BackColor = ApzonCommon.colorFocusRow;
			}
			if (tre.FormatRules.Count == 0)
			{
				await ExtensionMethods.ReloadFormatRuleTree(tre, tre.FormId);
			}
			await tre.ReloadFormSetting();
			tre.EndUpdate();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static async Task GenerateColumn(TreeEx tre, DataTable dtStructure, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null)
	{
		tre.BeginUpdate();
		tre.IndicatorWidth = 35;
		int idx = 0;
		string filterString = "";
		DataTable dt = new DataTable();
		foreach (DataRow row in dtStructure.Rows)
		{
			dt.Columns.Add(Function.ToString(row["ColumnName"]), row.IsExistsColumn("DataType") ? GetColumnType(Function.ToString(row["DataType"])) : typeof(string));
		}
		foreach (DataRow row2 in dtStructure.Rows)
		{
			TreeListColumnEx treeListColumnEx = tre.ColumnsEx.AddEx();
			treeListColumnEx.FieldName = (row2.IsExistsColumn("ColumnName") ? Function.ToString(row2["ColumnName"]) : "");
			treeListColumnEx.OptionsFilter.AllowFilter = true;
			treeListColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
			treeListColumnEx.Caption = (row2.IsExistsColumn("Caption") ? Function.ToString(row2["Caption"]) : treeListColumnEx.FieldName);
			treeListColumnEx.ToolTip = (row2.IsExistsColumn("Caption") ? Function.ToString(row2["Caption"]) : treeListColumnEx.FieldName);
			treeListColumnEx.Width = (row2.IsExistsColumn("Width") ? Function.ParseInt(row2["Width"]) : dfWidth);
			if (row2.IsExistsColumn("Visible"))
			{
				treeListColumnEx.Visible = Function.ParseBool(row2["Visible"]);
			}
			else
			{
				treeListColumnEx.Visible = dfVisible;
			}
			treeListColumnEx.VisibleIndex = (treeListColumnEx.Visible ? idx : (-1));
			idx = (treeListColumnEx.Visible ? (idx + 1) : idx);
			treeListColumnEx.OptionsColumn.ReadOnly = (row2.IsExistsColumn("ReadOnly") ? Function.ParseBool(row2["ReadOnly"]) : dfReadOnly);
			treeListColumnEx.OptionsColumn.AllowEdit = (row2.IsExistsColumn("AllowEdit") ? Function.ParseBool(row2["AllowEdit"]) : dfAllowEdit);
			treeListColumnEx.OptionsColumn.AllowMove = (row2.IsExistsColumn("AllowMove") ? Function.ParseBool(row2["AllowMove"]) : dfAllowMove);
			treeListColumnEx.OptionsColumn.AllowSize = (row2.IsExistsColumn("AllowSize") ? Function.ParseBool(row2["AllowSize"]) : dfAllowSize);
			treeListColumnEx.OptionsColumn.AllowSort = (row2.IsExistsColumn("AllowSort") ? Function.ParseBool(row2["AllowSort"]) : dfAllowSort);
			if (row2.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(row2["FormatType"])))
			{
				string text = Function.ToString(row2["FormatType"]).ToLower();
				if (!(text == "numeric"))
				{
					if (text == "datetime")
					{
						treeListColumnEx.Format.FormatType = FormatType.DateTime;
					}
					else
					{
						treeListColumnEx.Format.FormatType = FormatType.None;
					}
				}
				else
				{
					treeListColumnEx.Format.FormatType = FormatType.Numeric;
				}
			}
			if (row2.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(row2["DisplayFormat"])))
			{
				if (Function.ParseInt(row2["DisplayFormat"]) > 0)
				{
					treeListColumnEx.DisplayFormatType = (Enums.DisplayFormatType)Function.ParseInt(row2["DisplayFormat"]);
				}
				else
				{
					treeListColumnEx.Format.FormatString = Function.ToString(row2["DisplayFormat"]);
				}
			}
			if (row2.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(row2["Fixed"])))
			{
				string text = Function.ToString(row2["Fixed"]).ToUpper();
				if (!(text == "L"))
				{
					if (text == "R")
					{
						treeListColumnEx.Fixed = FixedStyle.Right;
					}
				}
				else
				{
					treeListColumnEx.Fixed = FixedStyle.Left;
				}
			}
			string format = treeListColumnEx.Format.FormatString;
			if (row2.IsExistsColumn("SummaryFormat") && !string.IsNullOrEmpty(Function.ToString(row2["SummaryFormat"])))
			{
				format = Function.ToString(row2["SummaryFormat"]);
			}
			if (row2.IsExistsColumn("SummaryType") && !string.IsNullOrEmpty(Function.ToString(row2["SummaryType"])))
			{
				tre.OptionsView.ShowSummaryFooter = true;
				GetSummary(treeListColumnEx, Function.ToString(row2["SummaryType"]).ToLower(), format);
			}
			if (row2.IsExistsColumn("Parent") && Function.ParseBool(row2["Parent"]))
			{
				tre.ParentFieldName = treeListColumnEx.FieldName;
			}
			if (row2.IsExistsColumn("Key") && Function.ParseBool(row2["Key"]))
			{
				tre.KeyFieldName = treeListColumnEx.FieldName;
			}
			if (row2.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(row2["Expression"])))
			{
				try
				{
					dt.Columns[treeListColumnEx.FieldName].Expression = Function.ToString(row2["Expression"]);
				}
				catch (Exception ex)
				{
					WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
				}
			}
			if (row2.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(row2["BackColor"])))
			{
				treeListColumnEx.AppearanceCell.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
				treeListColumnEx.AppearanceHeader.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
			}
			if (row2.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(row2["ForeColor"])))
			{
				treeListColumnEx.AppearanceCell.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
				treeListColumnEx.AppearanceHeader.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
			}
			if (row2.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(row2["FilterString"])))
			{
				filterString = filterString + Function.ToString(row2["FilterString"]) + " ";
			}
			await LoadUdfColumns(tre, treeListColumnEx, lstUDF, row2, dtValidValue, dt);
		}
		tre.ActiveFilterString = filterString;
		tre.DataSource = dt;
		tre.EndUpdate();
	}

	private static void tre_CustomDrawNodeIndicator(object sender, CustomDrawNodeIndicatorEventArgs e)
	{
		try
		{
			TreeList treeList = sender as TreeList;
			(e.ObjectArgs as IndicatorObjectInfoArgs).DisplayText = (treeList.GetVisibleIndexByNode(e.Node) + 1).ToString();
			e.ImageIndex = -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void tree_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (e.Node.Level == 0)
			{
				e.Appearance.BackColor = fatherColor;
			}
		}
		catch (Exception)
		{
		}
	}

	public static void GetSummary(TreeListColumn col, string summaryType, string format)
	{
		col.SummaryFooterStrFormat = format;
		switch (summaryType)
		{
		case "sum":
			col.SummaryFooter = SummaryItemType.Sum;
			break;
		case "average":
			col.SummaryFooter = SummaryItemType.Average;
			break;
		case "min":
			col.SummaryFooter = SummaryItemType.Min;
			break;
		case "max":
			col.SummaryFooter = SummaryItemType.Max;
			break;
		case "count":
			col.SummaryFooter = SummaryItemType.Count;
			break;
		default:
			col.SummaryFooter = SummaryItemType.None;
			break;
		}
		col.AllNodesSummary = true;
	}

	public static UnboundColumnType GetUnboundColumnType(string sqlType)
	{
		return sqlType.ToLower() switch
		{
			"int" => UnboundColumnType.Integer, 
			"smallint" => UnboundColumnType.Integer, 
			"numeric" => UnboundColumnType.Decimal, 
			"decimal" => UnboundColumnType.Decimal, 
			"money" => UnboundColumnType.Decimal, 
			"float" => UnboundColumnType.Decimal, 
			"datetime" => UnboundColumnType.DateTime, 
			"date" => UnboundColumnType.DateTime, 
			"bit" => UnboundColumnType.Boolean, 
			_ => UnboundColumnType.String, 
		};
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

	public static async Task LoadUdfColumns(TreeEx tree, TreeListColumnEx col, List<UserFieldsImpl> lstUDF, DataRow dr, DataTable dtValidValue, DataTable data)
	{
		try
		{
			string udfName = ((!string.IsNullOrEmpty(col.FieldName)) ? col.FieldName.Substring(2, col.FieldName.Length - 2).ToLower() : "");
			UserFieldsImpl udf = ((lstUDF != null && lstUDF.Count > 0) ? lstUDF.Where((UserFieldsImpl o) => o.Name.ToLower().Equals(udfName)).FirstOrDefault() : null);
			if (udf == null)
			{
				await LoadRepository(tree, col, dr, dtValidValue, data);
				return;
			}
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
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
					ToolTip = "Link UDO",
					Tag = udf.LinkedUDO
				});
				btn.Tag = udf.LinkedUDO;
				string udoType = await httpClient.Udo.GetTypeByCode(udf.LinkedUDO);
				btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						int docEntry = -1;
						if (udoType.Equals("3"))
						{
							docEntry = Function.ParseInt(dr[col.FieldName]);
						}
						else
						{
							Function.ToString(dr[col.FieldName]);
						}
						frm_UDO frm_UDO2 = new frm_UDO(udf.LinkedUDO, docEntry);
						frm_UDO2.Text = udf.Description;
						frm_UDO2.dataRow = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
						frm_UDO2.ShowDialog();
						if (frm_UDO2.ChoosedEntry != null)
						{
							tree.FocusedNode.SetValue(col.FieldName, frm_UDO2.ChoosedEntry);
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
				btn2.DisplayFormat.FormatType = col.Format.FormatType;
				btn2.DisplayFormat.FormatString = col.Format.FormatString;
				btn2.EditFormat.FormatType = col.Format.FormatType;
				btn2.EditFormat.FormatString = col.Format.FormatString;
				btn2.Buttons.Clear();
				btn2.Buttons.Add(new EditorButton
				{
					Kind = ButtonPredefines.Glyph,
					Image = Resources.right,
					IsLeft = true,
					ToolTip = "Linked to SystemObject " + Function.ToString((ApzObjectTypes)Function.ParseInt(udf.LinkedObject)),
					Tag = udf.LinkedObject
				});
				btn2.Tag = udf.LinkedObject;
				btn2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
				{
					try
					{
						int num = Function.ParseInt(btn2.Tag);
						string text = Function.ToString(tree.FocusedNode.GetDisplayText(col.FieldName));
						if (!string.IsNullOrEmpty(text))
						{
							((XtraFormEx)tree.FindForm()).DrillDown((ApzObjectTypes)num, (ApzObjectTypes)num, text);
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
					RepositoryItemLookUpEdit columnEdit = await WinFuntions.CreateEditorLinkedTable(tree, col, udf.LinkedTable);
					col.ColumnEdit = columnEdit;
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
				repositoryItemMemoExEdit.WordWrap = false;
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
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					col.UnboundType = UnboundColumnType.String;
					RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
					repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
					col.ColumnEdit = repositoryItemTextEdit2;
				}
				else
				{
					col.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					col.UnboundType = UnboundColumnType.DateTime;
					RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
					repositoryItemDateEditEx.Name = col.Name;
					repositoryItemDateEditEx.AllowMouseWheel = false;
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
				col.UnboundType = UnboundColumnType.Decimal;
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.DisplayFormatType = Enums.DisplayFormatType.Time;
					col.UnboundType = UnboundColumnType.String;
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
				col.Format.FormatType = FormatType.Numeric;
				col.Format.Format = GlobalData.UnitPriceFormat;
				col.ColumnEdit = repositoryItemCalcEdit;
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
			col.ColumnEdit = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task LoadRepository(TreeEx tree, TreeListColumnEx col, DataRow dr, DataTable dtValidValue, DataTable data)
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
			repositoryItemMemoExEdit.WordWrap = false;
			repositoryItemMemoExEdit.AcceptsReturn = true;
			repositoryItemMemoExEdit.ParseEditValue += Memo_ParseEditValue;
			repositoryItemMemoExEdit.FormatEditValue += Memo_FormatEditValue;
			repositoryItemMemoExEdit.Popup += Memo_Popup;
			col.ColumnEdit = repositoryItemMemoExEdit;
			break;
		}
		case "nvarchar":
			col.UnboundType = UnboundColumnType.String;
			break;
		case "int":
		{
			col.UnboundType = UnboundColumnType.Integer;
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.DisplayFormat.FormatString = "d";
			repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.EditFormat.FormatString = "d";
			repositoryItemCalcEdit.Mask.MaskType = MaskType.Numeric;
			repositoryItemCalcEdit.AllowMouseWheel = false;
			repositoryItemCalcEdit.Buttons.Clear();
			col.Format.FormatType = FormatType.Numeric;
			col.Format.FormatString = "d";
			col.ColumnEdit = repositoryItemCalcEdit;
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
			RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit2.AllowMouseWheel = false;
			col.ColumnEdit = repositoryItemCalcEdit2;
			_ = col.Format.FormatString;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])) && Function.ParseInt(dr["DisplayFormat"]) <= 0)
			{
				string formatString = Function.ToString(dr["DisplayFormat"]);
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.DisplayFormat.FormatString = formatString;
				repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.EditFormat.FormatString = formatString;
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
				col.Format.FormatType = FormatType.DateTime;
				col.Format.FormatString = "HH:mm";
				col.ColumnEdit = repositoryItemDateEdit;
				break;
			}
			col.UnboundType = UnboundColumnType.DateTime;
			RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
			repositoryItemDateEditEx.Name = col.Name;
			repositoryItemDateEditEx.AllowMouseWheel = false;
			col.Format.FormatType = FormatType.DateTime;
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
			col.Format.FormatString = text;
			break;
		}
		case "bit":
		{
			col.UnboundType = UnboundColumnType.Boolean;
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit.AutoHeight = false;
			col.ColumnEdit = repositoryItemCheckEdit;
			data.ChangeColumnDataType(col.FieldName, typeof(bool));
			tree.DoubleClick += (object s, EventArgs e) =>
			{
				try
				{
					Point pt = tree.PointToClient(Control.MousePosition);
					if (tree.CalcHitInfo(pt).Column == col)
					{
						DataTable dataTable = tree.DataSourceTable();
						if (!(dataTable.Columns[col.FieldName].DataType != typeof(bool)))
						{
							bool flag = dataTable.Select("[" + col.FieldName + "]= 'TRUE'").Length != 0;
							foreach (DataRow row in dataTable.Rows)
							{
								row[col.FieldName] = !flag;
							}
							dataTable.AcceptChanges();
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
				DataRow[] array4 = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
				if (!array4.IsNotNull())
				{
					break;
				}
				DataTable dt3 = array4.CopyToDataTable();
				dt3.Columns.Remove("ColumnName");
				RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
				{
					DataSource = dt3,
					ValueMember = "Value",
					DisplayMember = "Description",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				col.ColumnEdit = columnEdit;
				if (!dt3.IsExistsColumn("BackColor") || string.IsNullOrEmpty(Function.ToString(dt3.Rows[0])))
				{
					break;
				}
				tree.NodeCellStyle += (object s, GetCustomNodeCellStyleEventArgs e) =>
				{
					try
					{
						if (e.Column.FieldName == col.FieldName)
						{
							string text3 = Function.ToString(e.Node.GetValue(col.FieldName));
							{
								foreach (DataRow row2 in dt3.Rows)
								{
									if (text3.Equals(Function.ToString(row2["Value"])))
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
			DataRow[] array2 = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
			if (!array2.IsNotNull())
			{
				break;
			}
			DataTable dt2 = array2.CopyToDataTable();
			dt2.Columns.Remove("ColumnName");
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				DataSource = dt2,
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
			tree.NodeCellStyle += (object s, GetCustomNodeCellStyleEventArgs e) =>
			{
				try
				{
					string text3 = Function.ToString(e.Node.GetValue(col.FieldName));
					foreach (DataRow row3 in dt2.Rows)
					{
						if (text3.Equals(Function.ToString(row3["Value"])))
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
			DataRow[] array = dtValidValue.Select($"ColumnName = '{col.FieldName}'");
			if (!array.IsNotNull())
			{
				break;
			}
			DataTable dt = array.CopyToDataTable();
			tree.NodeCellStyle += (object s, GetCustomNodeCellStyleEventArgs e) =>
			{
				try
				{
					if (e.Column.FieldName == col.FieldName)
					{
						string text3 = Function.ToString(e.Node.GetValue(col.FieldName));
						{
							foreach (DataRow row4 in dt.Rows)
							{
								if (text3.Equals(Function.ToString(row4["Value"])))
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
				RepositoryItemLookUpEdit columnEdit2 = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource,
					ValueMember = "Code",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				col.ColumnEdit = columnEdit2;
			}
			break;
		case "linkedudo":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			RepositoryItemButtonEdit repositoryItemButtonEdit12 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit12.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit12.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit12.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit12.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit12.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit12.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to User Defined Object"
			});
			string subType = Function.ToString(dr["SubType"]);
			string udoCode = subType;
			repositoryItemButtonEdit12.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (subType.StartsWith("@"))
					{
						udoCode = Function.ToString(tree.FocusedNode.GetDisplayText(subType.Substring(1)));
					}
					string text3 = await _httpClient.Udo.GetTypeByCode(udoCode);
					int docEntry = -1;
					string code = "";
					if (text3.Equals("3"))
					{
						docEntry = Function.ParseInt(tree.FocusedNode.GetDisplayText(col.FieldName));
					}
					else
					{
						code = Function.ToString(tree.FocusedNode.GetDisplayText(col.FieldName));
					}
					frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry, code);
					frm_UDO2.dataRow = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
					frm_UDO2.ShowDialog();
					if (frm_UDO2.ChoosedEntry != null)
					{
						TreeListNode focusedNode = tree.FocusedNode;
						TreeListColumn columnID = tree.Columns[col.FieldName];
						focusedNode.SetValue(columnID, frm_UDO2.ChoosedEntry);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit12;
			break;
		}
		case "linkedobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.Integer;
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit repositoryItemButtonEdit11 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit11.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit11.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit11.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit11.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit11.Buttons.Clear();
			repositoryItemButtonEdit11.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject "
			});
			repositoryItemButtonEdit11.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					int num2 = 0;
					string empty = string.Empty;
					string text3 = Function.ToString(dr["SubType"]);
					string columnID = col.FieldName;
					if (text3.Split(' ').Length > 1 && text3.Split(' ')[1].StartsWith("#"))
					{
						columnID = Function.ToString(text3.Split(' ')[1].Substring(1));
						text3 = Function.ToString(text3.Split(' ')[0]);
					}
					num2 = ((!text3.StartsWith("@")) ? Function.ParseInt(text3) : Function.ParseInt(tree.FocusedNode.GetDisplayText(text3.Substring(1))));
					empty = Function.ToString(tree.FocusedNode.GetDisplayText(columnID));
					if (!string.IsNullOrEmpty(empty))
					{
						((XtraFormEx)tree.FindForm()).DrillDown((ApzObjectTypes)num2, (ApzObjectTypes)num2, empty);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit11;
			break;
		}
		case "linkedcflobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			RepositoryItemButtonEdit repositoryItemButtonEdit10 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit10.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit10.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit10.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit10.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit10.Buttons[0].Kind = ButtonPredefines.Search;
			string[] array3 = Function.ToString(dr["SubType"]).Split(',');
			string type = array3[0];
			string store2 = "";
			if (array3.Length > 1)
			{
				store2 = Function.ToString(array3[1]);
			}
			repositoryItemButtonEdit10.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject ",
				Tag = type
			});
			repositoryItemButtonEdit10.Tag = type;
			repositoryItemButtonEdit10.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (e.Button.Index == 0)
					{
						ShowChooseFromList(tree, col, dr, store2);
					}
					else if (e.Button.Index == 1)
					{
						int num2 = 0;
						num2 = ((!type.StartsWith("@")) ? Function.ParseInt(type) : Function.ParseInt(tree.FocusedNode.GetDisplayText(type.Substring(1))));
						string text3 = Function.ToString(Function.ParseInt(dr[col.FieldName]));
						if (!string.IsNullOrEmpty(text3))
						{
							((XtraFormEx)tree.FindForm()).DrillDown((ApzObjectTypes)num2, (ApzObjectTypes)num2, text3);
						}
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
		case "linkedstore":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			RepositoryItemButtonEdit repositoryItemButtonEdit9 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit9.Buttons.Clear();
			repositoryItemButtonEdit9.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true
			});
			string store = Function.ToString(dr["SubType"]);
			repositoryItemButtonEdit9.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					string text3 = Function.ToString(store.Split(' ')[0]);
					string name = "frm_" + text3;
					frm_ReportForm frm_ReportForm2 = new frm_ReportForm(text3);
					frm_ReportForm2.Name = name;
					frm_ReportForm2.mappingParams = Function.ToString(store.Split(' ')[1]);
					frm_ReportForm2.isExecuteWhenLoad = true;
					frm_ReportForm2.dataRow = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
					frm_ReportForm2.isExecuteWhenLoad = true;
					frm_ReportForm2.ShowDialog(tree.FindForm());
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
		case "linkedcrystal":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.UnboundType = UnboundColumnType.String;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			RepositoryItemButtonEdit repositoryItemButtonEdit8 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit8.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit8.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit8.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit8.EditFormat.FormatString = col.Format.FormatString;
			string fileName = Function.ToString(dr["SubType"]);
			repositoryItemButtonEdit8.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					string text3 = await WinFuntions.ReportViewerDatarowParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{fileName}", ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row);
					if (!string.IsNullOrEmpty(text3))
					{
						WinCommons.Error(Enums.MessageCode.None, text3);
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
		case "cfl":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit7 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit7.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit7.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit7.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit7.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit7.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit7.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowChooseFromList(tree, col, dr);
			};
			repositoryItemButtonEdit7.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowChooseFromList(tree, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit7;
			break;
		}
		case "sub":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit6 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit6.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit6.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit6.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit6.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit6.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit6.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowSubForm(tree, col, dr);
			};
			repositoryItemButtonEdit6.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowSubForm(tree, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit6;
			break;
		}
		case "webex":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit5 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit5.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit5.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit5.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit5.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit5.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit5.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowDefaultBrowser(tree, col, dr);
			};
			repositoryItemButtonEdit5.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowDefaultBrowser(tree, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit5;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "web":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit4 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit4.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit4.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit4.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit4.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit4.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit4.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(tree, col, dr);
			};
			repositoryItemButtonEdit4.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(tree, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit4;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webtimer":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit3 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit3.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit3.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit3.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit3.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit3.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit3.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(tree, col, dr, Enums.WebType.Address, isTimer: true);
			};
			repositoryItemButtonEdit3.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(tree, col, dr);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit3;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webhtml":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit2.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit2.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit2.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit2.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(tree, col, dr, Enums.WebType.Html);
			};
			repositoryItemButtonEdit2.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(tree, col, dr, Enums.WebType.Html);
				}
			};
			col.ColumnEdit = repositoryItemButtonEdit2;
			col.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			break;
		}
		case "webhtmltimer":
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.DisplayFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit.DisplayFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit.EditFormat.FormatType = col.Format.FormatType;
			repositoryItemButtonEdit.EditFormat.FormatString = col.Format.FormatString;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowWebBrowser(tree, col, dr, Enums.WebType.Html, isTimer: true);
			};
			repositoryItemButtonEdit.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowWebBrowser(tree, col, dr, Enums.WebType.Html, isTimer: true);
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

	private static void ShowChooseFromList(TreeEx tree, TreeListColumnEx col, DataRow dr, string storeName = "")
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
				num = Function.ParseInt(tree.FocusedNode.GetDisplayText(text.Substring(1)));
			}
			else if (Function.ParseInt(text) > 0)
			{
				num = Function.ParseInt(text);
			}
			if (num > 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL((Enums.TypeCFL)num, false, Function.ToString(tree.FocusedNode.GetDisplayText(col.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.Text = $"{frm_CFL2.Text} - {col.Caption}";
				frm_CFL2.ShowDialog();
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					WinCommons._OpenProcessing();
					if (chooseItem.IsExistsColumn(col.FieldName))
					{
						WinFuntions.DataRowToTreeView(chooseItem, tree);
					}
					else
					{
						WinFuntions.TreeNodeSetValue(tree.FocusedNode, col, chooseItem[0]);
					}
					WinCommons._CloseProcessing();
				}
				return;
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			string[] array = text.Split(' ');
			if (array.Length > 1 && array[1].StartsWith("@"))
			{
				DataRow row = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
				text = array[0];
				string[] array2 = array[1].Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Substring(1);
					if (array2[i].Contains("="))
					{
						int num2 = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num2), array2[i].Substring(num2 + 1)));
					}
					else if (text2.ToLower().Contains("username"))
					{
						list.Add(new Common.ParameterValue(array2[i], GlobalData.UserName));
					}
					else if (text2.ToLower().Contains("branch"))
					{
						list.Add(new Common.ParameterValue(array2[i], GlobalData.DefaultBranch));
					}
					else if (row.IsExistsColumn(text2))
					{
						list.Add(new Common.ParameterValue(array2[i], row[text2]));
					}
					else
					{
						list.Add(new Common.ParameterValue("", array2[i]));
					}
				}
				frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, Function.ToString(tree.FocusedNode.GetDisplayText(col.FieldName)), text, Enums.ItemObjectType.NoItemType, list, "");
				frm_CFL3.ShowDialog(tree.FindForm());
				DataRow chooseItem2 = frm_CFL3.ChooseItem;
				if (chooseItem2 != null)
				{
					WinCommons._OpenProcessing();
					if (chooseItem2.IsExistsColumn(col.FieldName))
					{
						WinFuntions.DataRowToTreeView(chooseItem2, tree);
					}
					else
					{
						WinFuntions.TreeNodeSetValue(tree.FocusedNode, col, chooseItem2[0]);
					}
					WinCommons._CloseProcessing();
				}
				return;
			}
			frm_CFL frm_CFL4 = new frm_CFL(Enums.TypeCFL.UserDefinedCfl, false, Function.ToString(tree.FocusedNode.GetDisplayText(col.FieldName)), text, Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL4.ShowDialog(tree.FindForm());
			DataRow chooseItem3 = frm_CFL4.ChooseItem;
			if (chooseItem3 != null)
			{
				WinCommons._OpenProcessing();
				if (chooseItem3.IsExistsColumn(col.FieldName))
				{
					WinFuntions.DataRowToTreeView(chooseItem3, tree);
				}
				else
				{
					WinFuntions.TreeNodeSetValue(tree.FocusedNode, col, chooseItem3[0]);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void ShowSubForm(TreeEx tree, TreeListColumnEx col, DataRow dr)
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
				DataRow row = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
				text = array[0];
				string[] array2 = array[1].Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Substring(1);
					if (array2[i].Contains("="))
					{
						int num = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1)));
					}
					else if (text2.ToLower().Contains("username"))
					{
						list.Add(new Common.ParameterValue(array2[i], GlobalData.UserName));
					}
					else if (text2.ToLower().Contains("branch"))
					{
						list.Add(new Common.ParameterValue(array2[i], GlobalData.DefaultBranch));
					}
					else if (row.IsExistsColumn(text2))
					{
						list.Add(new Common.ParameterValue(array2[i], row[text2]));
					}
					else
					{
						list.Add(new Common.ParameterValue("", array2[i]));
					}
				}
			}
			string caption = col.Caption;
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(frm_SubReportForm) && openForm.Tag.ToString().Equals(text + "_" + caption))
				{
					openForm.Activate();
					return;
				}
			}
			frm_SubReportForm frm = new frm_SubReportForm(text, list);
			frm.Name = text + "_" + caption;
			frm.Tag = text + "_" + caption;
			frm.Text = string.Format("{0} - {1}", "Sub Report Form", caption);
			frm.MdiParent = tree.FindForm().MdiParent;
			frm.WindowState = FormWindowState.Normal;
			frm.StartPosition = FormStartPosition.CenterScreen;
			frm.Show();
			tree.FindForm().FormClosing += (object s, FormClosingEventArgs e) =>
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

	private static async void ShowDefaultBrowser(TreeEx tree, TreeListColumnEx col, DataRow dr)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			string columnID = col.FieldName;
			if (text.StartsWith("#"))
			{
				columnID = text.Trim().Replace("#", "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					DataRow row = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
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
						else if (row.IsExistsColumn(text2))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), row[text2]));
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
				string text4 = Function.ToString(tree.FocusedNode.GetDisplayText(columnID));
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

	private static void ShowWebBrowser(TreeEx tree, TreeListColumnEx col, DataRow dr, Enums.WebType type = Enums.WebType.Address, bool isTimer = false)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			string columnID = col.FieldName;
			if (text.StartsWith("#"))
			{
				columnID = text.Trim().Replace("#", "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				string[] array = text.Split(' ');
				if (array.Length > 1 && array[1].StartsWith("@"))
				{
					DataRow row = ((DataRowView)tree.GetDataRecordByNode(tree.FocusedNode)).Row;
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
						else if (row.IsExistsColumn(text2))
						{
							list.Add(new Common.ParameterValue(array2[i].Trim(), row[text2]));
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
				frm_ChromeWebBrowser2.ShowDialog(tree.FindForm());
				return;
			}
			string text3 = Function.ToString(tree.FocusedNode.GetDisplayText(columnID));
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
				frm_ChromeWebBrowser4.ShowDialog(tree.FindForm());
				break;
			}
			case Enums.WebType.Html:
			{
				frm_ChromeWebBrowser frm_ChromeWebBrowser3 = new frm_ChromeWebBrowser();
				frm_ChromeWebBrowser3.Text = col.Caption;
				frm_ChromeWebBrowser3.url = text3;
				frm_ChromeWebBrowser3.type = Enums.WebType.Html;
				frm_ChromeWebBrowser3.isTimer = isTimer;
				frm_ChromeWebBrowser3.ShowDialog(tree.FindForm());
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

	public static DataTable DataSourceTable(this TreeList tv)
	{
		try
		{
			if (tv != null && tv.DataSource != null)
			{
				return tv.DataSource as DataTable;
			}
		}
		catch
		{
		}
		return new DataTable();
	}

	public static DataTable DataSourceTable(this TreeEx tv)
	{
		try
		{
			if (tv != null && tv.DataSource != null)
			{
				return tv.DataSource as DataTable;
			}
		}
		catch
		{
		}
		return new DataTable();
	}
}
