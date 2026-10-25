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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;
using SAPbobsCOM;

namespace Apzon.Forms.Helper;

public static class VGridControlHelper
{
	public static bool dfShowGroupPanel = false;

	public static int dfHeight = 25;

	public static bool dfVisible = false;

	public static bool dfReadOnly = false;

	public static bool dfAllowEdit = true;

	public static bool dfAllowMove = true;

	public static bool dfAllowSize = true;

	public static bool dfAutoHeightHeader = true;

	public static bool dfAppearanceEvenRow = true;

	public static DefaultBoolean dfAllowSort = DefaultBoolean.True;

	public static NewItemRowPosition dfNewItemRowPosition = NewItemRowPosition.None;

	public static async Task GenerateVGridControl(VGridControlEx vgrid, DataSet ds, List<UserFieldsImpl> lstUDF = null, bool expand = false, bool filterRow = false, string tableName = "", Dictionary<string, EditorRow> tblFormular = null)
	{
		_ = 1;
		try
		{
			if (ds.IsHasStructure())
			{
				if (vgrid.Rows.Count > 0 && vgrid.Rows.Count == ds.Tables[1].Columns.Count)
				{
					vgrid.DataSource = ds.Tables[1];
					vgrid.RefreshDataSource();
					vgrid.BestFit();
					if (expand)
					{
						vgrid.ExpandAllRows();
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
					await GenerateVGridControl(vgrid, ds.Tables[0], ds.Tables[1], ds.Tables[2], lstUDF, expand, tableName, tblFormular);
				}
				else
				{
					await GenerateVGridControl(vgrid, ds.Tables[0], ds.Tables[1], null, lstUDF, expand, tableName, tblFormular);
				}
			}
			else
			{
				if (!ds.IsNotNull())
				{
					return;
				}
				if (vgrid.Rows.Count > 0 && vgrid.Rows.Count == ds.Tables[0].Columns.Count)
				{
					vgrid.DataSource = ds.Tables[0];
					vgrid.RefreshDataSource();
				}
				else
				{
					vgrid.BeginUpdate();
					vgrid.Rows.Clear();
					foreach (DataColumn column in ds.Tables[0].Columns)
					{
						EditorRow editorRow = new EditorRow();
						editorRow.Properties.FieldName = column.ColumnName;
						editorRow.Properties.Caption = column.Caption;
						editorRow.Visible = true;
						editorRow.Height = dfHeight;
						vgrid.Rows.Add(editorRow);
					}
					vgrid.DataSource = ds.Tables[0];
					vgrid.RefreshDataSource();
					vgrid.EndUpdate();
				}
				if (expand)
				{
					vgrid.ExpandAllRows();
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public static async Task GenerateVGridControl(VGridControlEx vgrid, DataTable dtStructure, DataTable dt, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, bool expand = false, string tableName = "", Dictionary<string, EditorRow> tblFormular = null)
	{
		try
		{
			vgrid.DataSource = null;
			vgrid.Rows.Clear();
			if (dt == null || dt.Columns.Count <= 0)
			{
				await GenerateColumn(vgrid, dtStructure, dtValidValue, lstUDF, tableName);
			}
			else
			{
				int idx = 0;
				string filterString = "";
				int i;
				for (i = 0; i < dt.Columns.Count; i++)
				{
					EditorRow col = new EditorRow();
					vgrid.Rows.Add(col);
					col.Name = $"cl_{dt.Columns[i].ColumnName}";
					col.Properties.FieldName = dt.Columns[i].ColumnName;
					DataRow dr = (dtStructure.Rows[0].IsExistsColumn("ColumnName") ? (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").ToUpper().Equals(dt.Columns[i].ColumnName.ToUpper())
						select c).FirstOrDefault() : null);
					if (dr != null)
					{
						col.Properties.Caption = (dr.IsExistsColumn("Caption") ? Function.ToString(dr["Caption"]) : dt.Columns[i].ColumnName);
						col.Height = (dr.IsExistsColumn("Width") ? Function.ParseInt(dr["Width"]) : dfHeight);
						if (dr.IsExistsColumn("Visible"))
						{
							col.Visible = Function.ParseBool(dr["Visible"]);
						}
						else
						{
							col.Visible = dfVisible;
						}
						if (!col.Visible)
						{
							col.OptionsRow.ShowInCustomizationForm = true;
						}
						col.Index = (col.Visible ? idx : (-1));
						idx = (col.Visible ? (idx + 1) : idx);
						col.Properties.ReadOnly = (dr.IsExistsColumn("ReadOnly") ? Function.ParseBool(dr["ReadOnly"]) : dfReadOnly);
						col.Properties.AllowEdit = (dr.IsExistsColumn("AllowEdit") ? Function.ParseBool(dr["AllowEdit"]) : dfAllowEdit);
						col.OptionsRow.AllowMove = (dr.IsExistsColumn("AllowMove") ? Function.ParseBool(dr["AllowMove"]) : dfAllowMove);
						col.OptionsRow.AllowSize = (dr.IsExistsColumn("AllowSize") ? Function.ParseBool(dr["AllowSize"]) : dfAllowSize);
						if (dr.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(dr["FormatType"])))
						{
							string text = Function.ToString(dr["FormatType"]).ToLower();
							if (!(text == "numeric"))
							{
								if (text == "datetime")
								{
									col.Properties.Format.FormatType = FormatType.DateTime;
								}
								else
								{
									col.Properties.Format.FormatType = FormatType.None;
								}
							}
							else
							{
								col.Properties.Format.FormatType = FormatType.Numeric;
							}
						}
						if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])))
						{
							if (Function.ParseInt(dr["DisplayFormat"]) > 0)
							{
								DisplayFormat(col, Function.ParseInt(dr["DisplayFormat"]));
							}
							else
							{
								col.Properties.Format.FormatString = Function.ToString(dr["DisplayFormat"]);
							}
						}
						if (dr.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(dr["Fixed"])))
						{
							string text = Function.ToString(dr["Fixed"]).ToUpper();
							if (!(text == "T"))
							{
								if (text == "B")
								{
									col.Fixed = FixedStyle.Bottom;
								}
							}
							else
							{
								col.Fixed = FixedStyle.Top;
							}
						}
						if (dr.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(dr["Expression"])))
						{
							try
							{
								dt.Columns[i].Expression = Function.ToString(dr["Expression"]);
							}
							catch (Exception ex)
							{
								WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
							}
						}
						if (dr.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(dr["BackColor"])))
						{
							vgrid.RecordCellStyle += (object s, GetCustomRowCellStyleEventArgs e) =>
							{
								if (e.Row.Properties.FieldName.Equals(col.Properties.FieldName))
								{
									e.Appearance.BackColor = Color.FromName(Function.ToString(dr["BackColor"]));
								}
							};
						}
						if (dr.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(dr["ForeColor"])))
						{
							vgrid.RecordCellStyle += (object s, GetCustomRowCellStyleEventArgs e) =>
							{
								if (e.Row.Properties.FieldName.Equals(col.Properties.FieldName))
								{
									e.Appearance.ForeColor = Color.FromName(Function.ToString(dr["ForeColor"]));
								}
							};
						}
						if (dr.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(dr["FilterString"])))
						{
							filterString = filterString + Function.ToString(dr["FilterString"]) + " ";
						}
					}
					else
					{
						col.Properties.Caption = dt.Columns[i].ColumnName;
						col.Height = dfHeight;
						col.Visible = dfVisible;
						col.Properties.ReadOnly = dfReadOnly;
						col.Properties.AllowEdit = dfAllowEdit;
						col.OptionsRow.AllowMove = dfAllowMove;
						col.OptionsRow.AllowSize = dfAllowSize;
						if (dfVisible)
						{
							col.Index = idx;
							idx++;
						}
						else
						{
							col.Index = -1;
						}
					}
					await LoadUdfColumns(vgrid, col, lstUDF, dr, dtValidValue, tableName);
				}
				vgrid.DataSource = dt;
				vgrid.RefreshDataSource();
				vgrid.BestFit();
				if (dtStructure.IsExistsColumn("VisIndex"))
				{
					foreach (DataRow row in dtStructure.Rows)
					{
						string name = string.Format("cl_{0}", Function.ToString(row["ColumnName"]));
						if (vgrid.Rows[name].Visible)
						{
							int index = Function.ParseInt(row["VisIndex"]);
							vgrid.Rows[name].Index = index;
						}
					}
				}
				vgrid.FilterCriteria = filterString;
				vgrid.FilterString = filterString;
			}
			if (expand)
			{
				vgrid.ExpandAllRows();
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static async Task GenerateColumn(VGridControlEx vgrid, DataTable dtStructure, DataTable dtValidValue = null, List<UserFieldsImpl> lstUDF = null, string tableName = "", Dictionary<string, GridViewEx> tblFormular = null)
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
			EditorRow editorRow = new EditorRow();
			vgrid.Rows.Add(editorRow);
			editorRow.Properties.FieldName = (row2.IsExistsColumn("ColumnName") ? Function.ToString(row2["ColumnName"]) : "");
			editorRow.Name = $"cl_{editorRow.Properties.FieldName}";
			editorRow.Properties.Caption = (row2.IsExistsColumn("Caption") ? Function.ToString(row2["Caption"]) : editorRow.Properties.FieldName);
			editorRow.Height = (row2.IsExistsColumn("Width") ? Function.ParseInt(row2["Width"]) : dfHeight);
			if (row2.IsExistsColumn("Visible"))
			{
				editorRow.Visible = Function.ParseBool(row2["Visible"]);
			}
			else
			{
				editorRow.Visible = dfVisible;
			}
			if (!editorRow.Visible)
			{
				editorRow.OptionsRow.ShowInCustomizationForm = true;
			}
			editorRow.Index = (editorRow.Visible ? idx : (-1));
			idx = (editorRow.Visible ? (idx + 1) : idx);
			editorRow.Properties.ReadOnly = (row2.IsExistsColumn("ReadOnly") ? Function.ParseBool(row2["ReadOnly"]) : dfReadOnly);
			editorRow.Properties.AllowEdit = (row2.IsExistsColumn("AllowEdit") ? Function.ParseBool(row2["AllowEdit"]) : dfAllowEdit);
			editorRow.OptionsRow.AllowMove = (row2.IsExistsColumn("AllowMove") ? Function.ParseBool(row2["AllowMove"]) : dfAllowMove);
			editorRow.OptionsRow.AllowSize = (row2.IsExistsColumn("AllowSize") ? Function.ParseBool(row2["AllowSize"]) : dfAllowSize);
			if (row2.IsExistsColumn("FormatType") && !string.IsNullOrEmpty(Function.ToString(row2["FormatType"])))
			{
				string text = Function.ToString(row2["FormatType"]).ToLower();
				if (!(text == "numeric"))
				{
					if (text == "datetime")
					{
						editorRow.Properties.Format.FormatType = FormatType.DateTime;
					}
					else
					{
						editorRow.Properties.Format.FormatType = FormatType.None;
					}
				}
				else
				{
					editorRow.Properties.Format.FormatType = FormatType.Numeric;
				}
			}
			if (row2.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(row2["DisplayFormat"])))
			{
				if (Function.ParseInt(row2["DisplayFormat"]) > 0)
				{
					DisplayFormat(editorRow, Function.ParseInt(row2["DisplayFormat"]));
				}
				else
				{
					editorRow.Properties.Format.FormatString = Function.ToString(row2["DisplayFormat"]);
				}
			}
			if (row2.IsExistsColumn("Fixed") && !string.IsNullOrEmpty(Function.ToString(row2["Fixed"])))
			{
				string text = Function.ToString(row2["Fixed"]).ToUpper();
				if (!(text == "T"))
				{
					if (text == "B")
					{
						editorRow.Fixed = FixedStyle.Bottom;
					}
				}
				else
				{
					editorRow.Fixed = FixedStyle.Top;
				}
			}
			if (row2.IsExistsColumn("Expression") && !string.IsNullOrEmpty(Function.ToString(row2["Expression"])))
			{
				try
				{
					dt.Columns[editorRow.Properties.FieldName].Expression = Function.ToString(row2["Expression"]);
				}
				catch (Exception ex)
				{
					WinCommons.Warning(Enums.MessageCode.None, "Expression Error: " + ex.Message);
				}
			}
			if (row2.IsExistsColumn("BackColor") && !string.IsNullOrEmpty(Function.ToString(row2["BackColor"])))
			{
				editorRow.Appearance.BackColor = Color.FromName(Function.ToString(row2["BackColor"]));
			}
			if (row2.IsExistsColumn("ForeColor") && !string.IsNullOrEmpty(Function.ToString(row2["ForeColor"])))
			{
				editorRow.Appearance.ForeColor = Color.FromName(Function.ToString(row2["ForeColor"]));
			}
			if (row2.IsExistsColumn("FilterString") && !string.IsNullOrEmpty(Function.ToString(row2["FilterString"])))
			{
				filterString = filterString + Function.ToString(row2["FilterString"]) + " ";
			}
			await LoadUdfColumns(vgrid, editorRow, lstUDF, row2, dtValidValue, tableName);
		}
		vgrid.FilterCriteria = filterString;
		vgrid.FilterString = filterString;
		vgrid.DataSource = dt;
		vgrid.RefreshDataSource();
		vgrid.BestFit();
	}

	private static void DisplayFormat(EditorRow col, int value)
	{
		switch ((Enums.DisplayFormatType)value)
		{
		case Enums.DisplayFormatType.DateTime:
			col.Properties.Format.FormatType = FormatType.DateTime;
			col.Properties.Format.FormatString = GlobalData.DateTimeFormat;
			break;
		case Enums.DisplayFormatType.Percent:
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)} %";
			break;
		case Enums.DisplayFormatType.Qty:
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Rate:
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Unit:
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Time:
			col.Properties.Format.FormatString = "##:##";
			col.Properties.Format.FormatType = FormatType.Custom;
			break;
		case Enums.DisplayFormatType.Integer:
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = "n0";
			break;
		case Enums.DisplayFormatType.Price:
		case Enums.DisplayFormatType.Amount:
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

	public static async Task LoadUdfColumns(VGridControlEx vgrid, EditorRow col, List<UserFieldsImpl> lstUDF, DataRow dr, DataTable dtValidValue, string tableName = "", Dictionary<string, EditorRow> tblFormular = null)
	{
		_ = 1;
		try
		{
			string udfName = ((!string.IsNullOrEmpty(col.Properties.FieldName) && col.Properties.FieldName.StartsWith("U_")) ? col.Properties.FieldName.Substring(2, col.Properties.FieldName.Length - 2).ToLower() : "");
			UserFieldsImpl udf = ((lstUDF != null && lstUDF.Count > 0) ? lstUDF.Where((UserFieldsImpl o) => o.Name.ToLower().Equals(udfName)).FirstOrDefault() : null);
			if (udf == null)
			{
				await LoadRepository(vgrid, col, dr, dtValidValue);
				return;
			}
			switch (udf.Type)
			{
			case BoFieldTypes.db_Alpha:
				col.Properties.UnboundType = UnboundColumnType.String;
				if (!string.IsNullOrEmpty(udf.LinkedTable))
				{
					List<Common.Table_Code> list = await HttpClientHelperFactory.GetHttpClient().Udf.GetLinkedTable(udf.LinkedTable, GlobalData.UserName, GlobalData.DefaultBranch);
					list.Insert(0, new Common.Table_Code("", ""));
					RepositoryItemLookUpEdit rowEdit = new RepositoryItemLookUpEdit
					{
						DataSource = list,
						ValueMember = "Code",
						DisplayMember = "Name",
						NullText = "",
						BestFitMode = BestFitMode.BestFitResizePopup
					};
					col.Properties.RowEdit = rowEdit;
				}
				break;
			case BoFieldTypes.db_Memo:
				if (udf.SubType == BoFldSubTypes.st_Link)
				{
					col.Properties.UnboundType = UnboundColumnType.String;
					RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
					repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
					col.Properties.RowEdit = repositoryItemAttachFile;
				}
				else
				{
					RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
					repositoryItemMemoExEdit.NullText = "";
					repositoryItemMemoExEdit.ShowIcon = false;
					repositoryItemMemoExEdit.BestFitWidth = vgrid.RecordWidth;
					col.Properties.RowEdit = repositoryItemMemoExEdit;
				}
				break;
			case BoFieldTypes.db_Date:
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.Properties.UnboundType = UnboundColumnType.String;
					RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
					repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
					col.Properties.RowEdit = repositoryItemTextEdit2;
				}
				else
				{
					col.Properties.UnboundType = UnboundColumnType.DateTime;
					RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
					repositoryItemDateEditEx.AllowMouseWheel = false;
					col.Properties.RowEdit = repositoryItemDateEditEx;
				}
				break;
			case BoFieldTypes.db_Float:
			{
				col.Properties.UnboundType = UnboundColumnType.Decimal;
				RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.AllowMouseWheel = false;
				col.Properties.Format.FormatType = FormatType.Numeric;
				col.Properties.RowEdit = repositoryItemCalcEdit2;
				if (udf.SubType == BoFldSubTypes.st_Percentage)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.PercentageRateFormat;
					col.Properties.Format.Format = GlobalData.PercentageRateFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Quantity)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.QuantitiesFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.QuantitiesFormat;
					col.Properties.Format.Format = GlobalData.QuantitiesFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Rate)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.RateAccuracyFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.RateAccuracyFormat;
					col.Properties.Format.Format = GlobalData.RateAccuracyFormat;
				}
				else if (udf.SubType == BoFldSubTypes.st_Sum)
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.TotalsAccuracyFormat;
					col.Properties.Format.Format = GlobalData.TotalsAccuracyFormat;
				}
				else
				{
					repositoryItemCalcEdit2.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit2.EditFormat.Format = GlobalData.UnitPriceFormat;
					col.Properties.Format.Format = GlobalData.UnitPriceFormat;
				}
				break;
			}
			case BoFieldTypes.db_Numeric:
				if (udf.SubType == BoFldSubTypes.st_Time)
				{
					col.Properties.UnboundType = UnboundColumnType.Decimal;
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
					col.Properties.RowEdit = repositoryItemTextEdit;
				}
				else
				{
					col.Properties.UnboundType = UnboundColumnType.Decimal;
					RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
					repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
					repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit.EditFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit.AllowMouseWheel = false;
					col.Properties.Format.FormatType = FormatType.Numeric;
					col.Properties.Format.Format = GlobalData.UnitPriceFormat;
					col.Properties.RowEdit = repositoryItemCalcEdit;
				}
				break;
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
			col.Properties.RowEdit = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task LoadRepository(VGridControlEx vgrid, EditorRow col, DataRow dr, DataTable dtValidValue)
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
			col.Properties.UnboundType = UnboundColumnType.String;
			RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
			repositoryItemMemoExEdit.NullText = "";
			repositoryItemMemoExEdit.ShowIcon = false;
			repositoryItemMemoExEdit.BestFitWidth = vgrid.RecordWidth;
			col.Properties.RowEdit = repositoryItemMemoExEdit;
			break;
		}
		case "nvarchar":
			col.Properties.UnboundType = UnboundColumnType.String;
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
				col.Properties.RowEdit = repositoryItemTextEdit;
			}
			break;
		case "int":
		{
			col.Properties.UnboundType = UnboundColumnType.Integer;
			RepositoryItemCalcEdit repositoryItemCalcEdit3 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit3.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit3.DisplayFormat.FormatString = "d";
			repositoryItemCalcEdit3.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit3.EditFormat.FormatString = "d";
			repositoryItemCalcEdit3.Mask.MaskType = MaskType.Numeric;
			repositoryItemCalcEdit3.Mask.EditMask = "d";
			repositoryItemCalcEdit3.AllowMouseWheel = false;
			repositoryItemCalcEdit3.Buttons.Clear();
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatString = "d";
			col.Properties.RowEdit = repositoryItemCalcEdit3;
			break;
		}
		case "decimal":
		case "money":
		{
			col.Properties.UnboundType = UnboundColumnType.Decimal;
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.QuantitiesFormat;
			repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.EditFormat.Format = GlobalData.QuantitiesFormat;
			repositoryItemCalcEdit.AllowMouseWheel = false;
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.Format.Format = GlobalData.QuantitiesFormat;
			col.Properties.RowEdit = repositoryItemCalcEdit;
			break;
		}
		case "numeric":
		{
			col.Properties.UnboundType = UnboundColumnType.Decimal;
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
				repositoryItemTextEdit2.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit2.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit2.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit2.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit2.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit2.Mask.MaskType = MaskType.RegEx;
				col.Properties.RowEdit = repositoryItemTextEdit2;
				break;
			}
			RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
			col.Properties.Format.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.AllowMouseWheel = false;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])))
			{
				if (Function.ParseInt(dr["DisplayFormat"]) > 0)
				{
					DisplayFormat(col, Function.ParseInt(dr["DisplayFormat"]));
				}
				else
				{
					col.Properties.Format.FormatString = Function.ToString(dr["DisplayFormat"]);
				}
			}
			col.Properties.RowEdit = repositoryItemCalcEdit2;
			break;
		}
		case "datetime":
		case "date":
		{
			if (dr.IsExistsColumn("SubType") && "time".Equals(Function.ToString(dr["SubType"]).ToLower()))
			{
				col.Properties.UnboundType = UnboundColumnType.DateTime;
				RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
				repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.DisplayFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.EditFormat.FormatType = FormatType.DateTime;
				repositoryItemDateEdit.EditFormat.FormatString = "HH:mm";
				repositoryItemDateEdit.AllowMouseWheel = false;
				col.Properties.Format.FormatType = FormatType.DateTime;
				col.Properties.Format.FormatString = "HH:mm";
				col.Properties.RowEdit = repositoryItemDateEdit;
				break;
			}
			col.Properties.UnboundType = UnboundColumnType.DateTime;
			string dateTimeFormat = GlobalData.DateTimeFormat;
			if (dr.IsExistsColumn("DisplayFormat") && !string.IsNullOrEmpty(Function.ToString(dr["DisplayFormat"])))
			{
				if (Function.ParseInt(dr["DisplayFormat"]) > 0)
				{
					DisplayFormat(col, Function.ParseInt(dr["DisplayFormat"]));
				}
				else
				{
					col.Properties.Format.FormatString = Function.ToString(dr["DisplayFormat"]);
				}
			}
			RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
			repositoryItemDateEditEx.AllowMouseWheel = false;
			col.Properties.Format.FormatType = FormatType.DateTime;
			col.Properties.Format.FormatString = dateTimeFormat;
			col.Properties.RowEdit = repositoryItemDateEditEx;
			break;
		}
		case "float":
		{
			col.Properties.UnboundType = UnboundColumnType.Decimal;
			RepositoryItemCalcEdit repositoryItemCalcEdit4 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit4.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit4.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit4.AllowMouseWheel = false;
			col.Properties.Format.FormatType = FormatType.Numeric;
			col.Properties.RowEdit = repositoryItemCalcEdit4;
			if (dr.IsExistsColumn("SubType"))
			{
				if ("percentage".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit4.DisplayFormat.Format = GlobalData.PercentageRateFormat;
					col.Properties.Format.Format = GlobalData.PercentageRateFormat;
				}
				else if ("quantity".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit4.DisplayFormat.Format = GlobalData.QuantitiesFormat;
					repositoryItemCalcEdit4.EditFormat.Format = GlobalData.QuantitiesFormat;
					col.Properties.Format.Format = GlobalData.QuantitiesFormat;
				}
				else if ("rate".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit4.DisplayFormat.Format = GlobalData.RateAccuracyFormat;
					repositoryItemCalcEdit4.EditFormat.Format = GlobalData.RateAccuracyFormat;
					col.Properties.Format.Format = GlobalData.RateAccuracyFormat;
				}
				else if ("sum".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit4.DisplayFormat.Format = GlobalData.TotalsAccuracyFormat;
					repositoryItemCalcEdit4.EditFormat.Format = GlobalData.TotalsAccuracyFormat;
					col.Properties.Format.Format = GlobalData.TotalsAccuracyFormat;
				}
				else if ("price".Equals(Function.ToString(dr["SubType"]).ToLower()))
				{
					repositoryItemCalcEdit4.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					repositoryItemCalcEdit4.EditFormat.Format = GlobalData.UnitPriceFormat;
					col.Properties.Format.Format = GlobalData.UnitPriceFormat;
				}
			}
			break;
		}
		case "bit":
		{
			col.Properties.UnboundType = UnboundColumnType.Boolean;
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit.AutoHeight = false;
			col.Properties.RowEdit = repositoryItemCheckEdit;
			break;
		}
		case "link":
		{
			col.Properties.UnboundType = UnboundColumnType.String;
			RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
			repositoryItemAttachFile.ObjType = ApzObjectTypes.oNone;
			col.Properties.RowEdit = repositoryItemAttachFile;
			break;
		}
		case "validvalue":
		{
			string text2 = Function.ToString(dr["SubType"]);
			if (!string.IsNullOrEmpty(text2))
			{
				DataTable data = (await _httpClient.Commons.ExecuteQuery(text2)).Data;
				_httpClient = null;
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
				repositoryItemLookUpEdit.DataSource = data;
				if (data != null && data.Columns.Count >= 2)
				{
					repositoryItemLookUpEdit.ValueMember = data.Columns[0].ColumnName;
					repositoryItemLookUpEdit.DisplayMember = data.Columns[1].ColumnName;
				}
				repositoryItemLookUpEdit.NullText = string.Empty;
				repositoryItemLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;
				col.Properties.RowEdit = repositoryItemLookUpEdit;
			}
			else
			{
				if (!dtValidValue.IsNotNull())
				{
					break;
				}
				DataRow[] array = dtValidValue.Select($"ColumnName = '{col.Properties.FieldName}'");
				if (!array.IsNotNull())
				{
					break;
				}
				DataTable dataSource = array.CopyToDataTable();
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource,
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
				col.Properties.RowEdit = repositoryItemLookUpEdit2;
			}
			break;
		}
		case "validcolor":
		{
			if (!dtValidValue.IsNotNull())
			{
				break;
			}
			DataRow[] array2 = dtValidValue.Select($"ColumnName = '{col.Properties.FieldName}'");
			if (!array2.IsNotNull())
			{
				break;
			}
			DataTable dt = array2.CopyToDataTable();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit
			{
				DataSource = dt,
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
			col.Properties.RowEdit = repositoryItemLookUpEdit3;
			vgrid.RecordCellStyle += (object s, GetCustomRowCellStyleEventArgs e) =>
			{
				try
				{
					if (e.RecordIndex >= 0)
					{
						string text3 = Function.ToString(vgrid.GetCellValue(e.Row, e.RecordIndex));
						{
							foreach (DataRow row in dt.Rows)
							{
								if (text3.Equals(Function.ToString(row["Value"]), StringComparison.OrdinalIgnoreCase))
								{
									string text4 = Function.ToString(row["BackColor"]);
									string text5 = Function.ToString(row["ForeColor"]);
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
			col.Properties.UnboundType = UnboundColumnType.String;
			if (!string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				_httpClient = HttpClientHelperFactory.GetHttpClient();
				List<Common.Table_Code> dataSource2 = await _httpClient.Udf.GetLinkedTable(Function.ToString(dr["SubType"]), GlobalData.UserName, GlobalData.DefaultBranch);
				RepositoryItemLookUpEdit rowEdit = new RepositoryItemLookUpEdit
				{
					DataSource = dataSource2,
					ValueMember = "Code",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				col.Properties.RowEdit = rowEdit;
			}
			break;
		case "linkedudo":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.Properties.UnboundType = UnboundColumnType.String;
			RepositoryItemButtonEdit repositoryItemButtonEdit5 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit5.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit5.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit5.EditFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit5.EditFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit5.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit5.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				ToolTip = "Linked to User Defined Object",
				IsLeft = true
			});
			string subType2 = Function.ToString(dr["SubType"]);
			string udoCode = subType2;
			repositoryItemButtonEdit5.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (subType2.StartsWith("@"))
					{
						udoCode = Function.ToString(vgrid.GetCellValue(vgrid.GetRowByFieldName(subType2.Substring(1)), vgrid.FocusedRecord));
					}
					string text3 = await _httpClient.Udo.GetTypeByCode(udoCode);
					int docEntry = -1;
					string code = "";
					if (text3.Equals("3"))
					{
						docEntry = Function.ParseInt(vgrid.GetCellValue(col, vgrid.FocusedRecord));
					}
					else
					{
						code = Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord));
					}
					frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry, code);
					frm_UDO2.dataRow = (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord];
					frm_UDO2.ShowDialog();
					if (frm_UDO2.ChoosedEntry != null)
					{
						vgrid.SetCellValue(col, vgrid.FocusedRecord, frm_UDO2.ChoosedEntry);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.Properties.RowEdit = repositoryItemButtonEdit5;
			break;
		}
		case "linkedobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit4 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit4.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit4.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit4.EditFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit4.EditFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit4.Buttons.Clear();
			string subType = Function.ToString(dr["SubType"]);
			repositoryItemButtonEdit4.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject ",
				Tag = subType
			});
			repositoryItemButtonEdit4.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					int num3 = 0;
					num3 = ((!subType.StartsWith("@")) ? Function.ParseInt(subType) : Function.ParseInt(vgrid.GetCellValue(vgrid.GetRowByFieldName(subType.Substring(1)), vgrid.FocusedRecord)));
					string text3 = Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord));
					if (!string.IsNullOrEmpty(text3))
					{
						((XtraFormEx)vgrid.FindForm()).DrillDown((ApzObjectTypes)num3, (ApzObjectTypes)num3, text3);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.Properties.RowEdit = repositoryItemButtonEdit4;
			break;
		}
		case "linkedcflobject":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit3 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit3.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit3.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit3.EditFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit3.EditFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit3.Buttons[0].Kind = ButtonPredefines.Search;
			string text = Function.ToString(dr["SubType"]);
			string[] str = text.Split(',');
			string store2 = "";
			string type = str[0];
			if (str.Length > 1)
			{
				store2 = Function.ToString(str[1]);
			}
			repositoryItemButtonEdit3.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Glyph,
				Image = Resources.right,
				IsLeft = true,
				ToolTip = "Linked to SystemObject ",
				Tag = type
			});
			repositoryItemButtonEdit3.Tag = type;
			repositoryItemButtonEdit3.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (e.Button.Index == 0)
					{
						if (string.IsNullOrEmpty(store2))
						{
							int num3 = 0;
							num3 = ((!type.StartsWith("@")) ? Function.ParseInt(type) : Function.ParseInt(vgrid.GetCellValue(vgrid.GetRowByFieldName(type.Substring(1)), vgrid.FocusedRecord)));
							string txtSeach = Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord));
							frm_CFL frm_CFL2 = new frm_CFL((Enums.TypeCFL)num3, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
							frm_CFL2.Text = $"{frm_CFL2.Text} - {col.Properties.Caption}";
							frm_CFL2.ShowDialog();
							DataRow chooseItem = frm_CFL2.ChooseItem;
							if (chooseItem != null)
							{
								WinCommons._OpenProcessing();
								if (chooseItem.IsExistsColumn(col.Properties.FieldName))
								{
									WinFuntions.DataRowToDataRow(chooseItem, (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord]);
								}
								else
								{
									vgrid.SetCellValue(col, vgrid.FocusedRecord, chooseItem[0]);
								}
								WinCommons._CloseProcessing();
							}
						}
						else
						{
							List<Common.ParameterValue> list = new List<Common.ParameterValue>();
							string[] array3 = store2.Split(' ');
							if (array3.Length > 1 && array3[1].StartsWith("@"))
							{
								DataRow dataRow = (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord];
								store2 = array3[0].Trim();
								array3[1].Split(',');
								for (int i = 0; i < str.Length; i++)
								{
									string text3 = str[i].Substring(1).Trim();
									if (text3.ToLower().Contains("username"))
									{
										list.Add(new Common.ParameterValue(str[i].Trim(), GlobalData.UserName));
									}
									else if (text3.ToLower().Contains("branch"))
									{
										list.Add(new Common.ParameterValue(str[i].Trim(), GlobalData.DefaultBranch));
									}
									else if (dataRow.IsExistsColumn(text3))
									{
										list.Add(new Common.ParameterValue(str[i].Trim(), dataRow[text3]));
									}
									else if (str[i].Contains("="))
									{
										int num4 = str[i].IndexOf('=');
										list.Add(new Common.ParameterValue(str[i].Substring(0, num4).Trim(), str[i].Substring(num4 + 1).Replace("'", "")));
									}
									else
									{
										list.Add(new Common.ParameterValue("", str[i].Trim().Replace("'", "")));
									}
								}
							}
							frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord)), store2, Enums.ItemObjectType.NoItemType, list, "");
							frm_CFL3.Text = $"{frm_CFL3.Text} - {col.Properties.Caption}";
							frm_CFL3.ShowDialog(vgrid.FindForm());
							DataRow chooseItem2 = frm_CFL3.ChooseItem;
							if (chooseItem2 != null)
							{
								WinCommons._OpenProcessing();
								if (chooseItem2.IsExistsColumn(col.Properties.FieldName))
								{
									WinFuntions.DataRowToDataRow(chooseItem2, (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord]);
								}
								else
								{
									vgrid.SetCellValue(col, vgrid.FocusedRecord, chooseItem2[0]);
								}
								WinCommons._CloseProcessing();
							}
						}
					}
					else if (e.Button.Index == 1)
					{
						int num5 = 0;
						num5 = ((!type.StartsWith("@")) ? Function.ParseInt(type) : Function.ParseInt(vgrid.GetCellValue(vgrid.GetRowByFieldName(type.Substring(1)), vgrid.FocusedRecord)));
						string text4 = Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord));
						if (!string.IsNullOrEmpty(text4))
						{
							((XtraFormEx)vgrid.FindForm()).DrillDown((ApzObjectTypes)num5, (ApzObjectTypes)num5, text4);
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.Properties.RowEdit = repositoryItemButtonEdit3;
			break;
		}
		case "linkedstore":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.Properties.UnboundType = UnboundColumnType.String;
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
					frm_ReportForm2.mappingParams = Function.ToString(store.Split(' ')[1]);
					frm_ReportForm2.isExecuteWhenLoad = true;
					frm_ReportForm2.dataRow = (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord];
					frm_ReportForm2.ShowDialog(vgrid.FindForm());
					btnStore.Buttons[0].Enabled = true;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
			col.Properties.RowEdit = btnStore;
			break;
		}
		case "linkedcrystal":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			col.Properties.UnboundType = UnboundColumnType.String;
			RepositoryItemButtonEdit btnCrytal = new RepositoryItemButtonEdit();
			btnCrytal.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			btnCrytal.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			btnCrytal.EditFormat.FormatType = col.Properties.Format.FormatType;
			btnCrytal.EditFormat.FormatString = col.Properties.Format.FormatString;
			string fileName = Function.ToString(dr["SubType"]);
			btnCrytal.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					btnCrytal.Buttons[0].Enabled = false;
					string text3 = await WinFuntions.ReportViewerDatarowParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{fileName}", (vgrid.DataSource as DataTable).Rows[0]);
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
			col.Properties.RowEdit = btnCrytal;
			break;
		}
		case "cfl":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit2.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit2.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit2.EditFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit2.EditFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowChooseFromList(vgrid, col, dr);
			};
			repositoryItemButtonEdit2.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowChooseFromList(vgrid, col, dr);
				}
			};
			col.Properties.RowEdit = repositoryItemButtonEdit2;
			break;
		}
		case "sub":
		{
			if (string.IsNullOrEmpty(Function.ToString(dr["SubType"])))
			{
				break;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.DisplayFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit.DisplayFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit.EditFormat.FormatType = col.Properties.Format.FormatType;
			repositoryItemButtonEdit.EditFormat.FormatString = col.Properties.Format.FormatString;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs v) =>
			{
				ShowSubForm(vgrid, dr);
			};
			repositoryItemButtonEdit.KeyDown += (object s, KeyEventArgs v) =>
			{
				if (v.KeyCode == Keys.Tab && s is ButtonEdit buttonEdit && string.IsNullOrEmpty(Function.ToString(buttonEdit.EditValue)))
				{
					ShowSubForm(vgrid, dr);
				}
			};
			col.Properties.RowEdit = repositoryItemButtonEdit;
			break;
		}
		default:
			col.Properties.UnboundType = UnboundColumnType.String;
			break;
		}
	}

	private static void ShowChooseFromList(VGridControlEx vgrid, EditorRow col, DataRow dr)
	{
		try
		{
			string text = Function.ToString(dr["SubType"]);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int num = 0;
			if (text.StartsWith("@"))
			{
				num = Function.ParseInt(vgrid.GetCellValue(vgrid.GetRowByFieldName(text.Substring(1)), vgrid.FocusedRecord));
			}
			else if (Function.ParseInt(text) > 0)
			{
				num = Function.ParseInt(text);
			}
			if (num > 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL((Enums.TypeCFL)num, false, Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.Text = $"{frm_CFL2.Text} - {col.Properties.Caption}";
				frm_CFL2.ShowDialog();
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					WinCommons._OpenProcessing();
					if (chooseItem.IsExistsColumn(col.Properties.FieldName))
					{
						WinFuntions.DataRowToDataRow(chooseItem, (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord]);
					}
					else
					{
						vgrid.SetCellValue(col, vgrid.FocusedRecord, chooseItem[0]);
					}
					WinCommons._CloseProcessing();
				}
				return;
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			string[] array = text.Split(' ');
			if (array.Length > 1 && array[1].StartsWith("@"))
			{
				DataRow dataRow = (vgrid.DataSource as DataTable).Rows[0];
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
					else if (dataRow.IsExistsColumn(text2))
					{
						list.Add(new Common.ParameterValue(array2[i].Trim(), dataRow[text2]));
					}
					else if (array2[i].Contains("="))
					{
						int num2 = array2[i].IndexOf('=');
						list.Add(new Common.ParameterValue(array2[i].Substring(0, num2), array2[i].Substring(num2 + 1).Replace("'", "")));
					}
					else
					{
						list.Add(new Common.ParameterValue("", array2[i].Trim().Replace("'", "")));
					}
				}
			}
			frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, Function.ToString(vgrid.GetCellValue(col, vgrid.FocusedRecord)), text, Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL3.Text = $"{frm_CFL3.Text} - {col.Properties.Caption}";
			frm_CFL3.ShowDialog(vgrid.FindForm());
			DataRow chooseItem2 = frm_CFL3.ChooseItem;
			if (chooseItem2 != null)
			{
				WinCommons._OpenProcessing();
				if (chooseItem2.IsExistsColumn(col.Properties.FieldName))
				{
					WinFuntions.DataRowToDataRow(chooseItem2, (vgrid.DataSource as DataTable).Rows[vgrid.FocusedRecord]);
				}
				else
				{
					vgrid.SetCellValue(col, vgrid.FocusedRecord, chooseItem2[0]);
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

	private static void ShowSubForm(VGridControlEx view, DataRow dr)
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
