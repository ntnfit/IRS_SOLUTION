using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Reports;

public class frm_ReportFormSettings : XtraFormEx
{
	private bool isClose;

	private bool isNotChangeSetting;

	private bool isLoadForm;

	public DataTable dtStructure = new DataTable("Structure");

	public DataTable dtValidValue = new DataTable("ValidData");

	public bool isSave;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private GridHitInfo downHitInfo;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_CopyFormSetting;

	private CheckBox chk_SetDefault;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_New;

	private LookUpEdit lue_FormSetting;

	private SimpleButtonEx btn_EditName;

	private SimpleButtonEx btn_EditValidValue;

	private GridControlEx grd_Setting;

	private GridViewEx gv_Setting;

	private SimpleButtonEx btn_Del;

	private GridControlEx grid { get; set; }

	private TreeEx tree { get; set; }

	public PivotGridControlEx pivot { get; set; }

	private string itemid { get; set; }

	private string formid { get; set; }

	private string viewType { get; set; }

	public int id { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_ReportFormSettings()
	{
		InitializeComponent();
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && !DisableAuthorization)
		{
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportFormSettings"))
			{
				throw new Exception(Resources.Msg_FormAccessDenied);
			}
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportFormSettings" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
		}
	}

	public frm_ReportFormSettings(string tableType, GridControlEx gridControl, TreeEx treeView, string itemId, string formId, int settingId = 0, DataTable dtStruc = null, DataTable dtValid = null)
	{
		viewType = tableType;
		grid = gridControl;
		tree = treeView;
		itemid = itemId;
		formid = formId;
		id = settingId;
		if (dtStruc != null)
		{
			dtStructure = dtStruc;
		}
		if (dtValid != null)
		{
			dtValidValue = dtValid;
		}
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && !DisableAuthorization)
		{
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportFormSettings"))
			{
				throw new Exception(Resources.Msg_FormAccessDenied);
			}
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportFormSettings" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
		}
	}

	private async void frm_FormSettings_Load(object sender, EventArgs e)
	{
		isLoadForm = true;
		btn_OK.Enabled = false;
		btn_New.Enabled = false;
		btn_Del.Enabled = false;
		try
		{
			gv_Setting.DisableFormSetting = true;
			await GetListFormSetting();
			if (viewType.Equals("1"))
			{
				await LoadSettingGridView();
			}
			else if (viewType.Equals("2"))
			{
				await LoadSettingTreeView();
			}
			else if (viewType.Equals("3"))
			{
				await LoadSettingPivotGrid();
			}
			if (id == 0)
			{
				btn_New.Visible = false;
				btn_Del.Visible = false;
				chk_SetDefault.Checked = true;
				chk_SetDefault.Enabled = false;
				FormMode = Enums.FormViewModeType.Add;
			}
			gv_Setting.OptionsCustomization.AllowSort = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		btn_New.Enabled = true;
		btn_Del.Enabled = true;
		isLoadForm = false;
	}

	private async Task LoadSettingGridView()
	{
		_ = 2;
		try
		{
			gv_Setting.ShowLoadingPanel();
			GridViewEx gv_Data = grid.DefaultView as GridViewEx;
			DataTable dt = grid.DatasourceTable();
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(id, formid, itemid, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (!ds.IsHasStructure())
			{
				return;
			}
			if (dtStructure.IsNotNull())
			{
				int num = gv_Data.Columns.Max((GridColumn m) => m.VisibleIndex) + 1;
				foreach (GridColumnEx column in gv_Data.Columns)
				{
					string colName = column.FieldName;
					DataRow dataRow = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName)
						select c).FirstOrDefault();
					if (dataRow != null)
					{
						dataRow["Caption"] = column.Caption;
						dataRow["Width"] = column.Width;
						dataRow["GroupIndex"] = column.GroupIndex;
						dataRow["Visible"] = column.Visible;
						dataRow["VisIndex"] = column.VisibleIndex;
						dataRow["ReadOnly"] = column.OptionsColumn.ReadOnly;
						if (!column.Visible)
						{
							dataRow["VisIndex"] = num;
							num++;
						}
						continue;
					}
					DataRow dataRow2 = dtStructure.NewRow();
					dataRow2["ColumnName"] = colName;
					dataRow2["DataType"] = (dt.IsExistsColumn(colName) ? GetColumnType(dt.Columns[colName].DataType) : Function.ToString(column.ColumnType));
					dataRow2["Caption"] = column.Caption;
					dataRow2["Width"] = column.Width;
					dataRow2["Visible"] = column.Visible;
					dataRow2["GroupIndex"] = column.GroupIndex;
					dataRow2["VisIndex"] = column.VisibleIndex;
					if (!column.Visible)
					{
						dataRow2["VisIndex"] = num;
						num++;
					}
					dataRow2["ReadOnly"] = column.OptionsColumn.ReadOnly;
					dataRow2["AllowEdit"] = column.OptionsColumn.AllowEdit;
					dataRow2["AllowMove"] = column.OptionsColumn.AllowMove;
					dataRow2["AllowSize"] = column.OptionsColumn.AllowSize;
					dataRow2["AllowSort"] = column.OptionsColumn.AllowSort;
					dtStructure.Rows.Add(dataRow2);
				}
				dtStructure.Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
				if (dtStructure.Rows.Count > gv_Data.Columns.Count)
				{
					List<DataRow> list = new List<DataRow>();
					foreach (DataRow row in dtStructure.Rows)
					{
						string fieldName = Function.ToString(row["ColumnName"]);
						if (!gv_Data.IsExistColumn(fieldName))
						{
							list.Add(row);
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
				DataView defaultView = dtStructure.DefaultView;
				dtStructure.DefaultView.Sort = "Visible DESC, VisIndex ASC";
				dtStructure = defaultView.ToTable("Structure");
				DataTable dataTable = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable = ds.Tables[2].Copy();
					dataTable.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
			else
			{
				if (ds.Tables[1] == null)
				{
					return;
				}
				if (dtStructure == null)
				{
					dtStructure = new DataTable();
				}
				int num2 = gv_Data.Columns.Max((GridColumn m) => m.VisibleIndex) + 1;
				for (int num3 = 0; num3 < gv_Data.Columns.Count; num3++)
				{
					string colName2 = gv_Data.Columns[num3].FieldName;
					DataRow dataRow4 = (from c in ds.Tables[1].AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow4 != null)
					{
						dataRow4["Caption"] = gv_Data.Columns[num3].Caption;
						dataRow4["Width"] = gv_Data.Columns[num3].Width;
						dataRow4["Visible"] = gv_Data.Columns[num3].Visible;
						dataRow4["GroupIndex"] = gv_Data.Columns[num3].GroupIndex;
						dataRow4["VisIndex"] = gv_Data.Columns[num3].VisibleIndex;
						if (!gv_Data.Columns[num3].Visible)
						{
							dataRow4["VisIndex"] = num2;
							num2++;
						}
						continue;
					}
					DataRow dataRow5 = ds.Tables[1].NewRow();
					DataRow dataRow6 = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow6 != null)
					{
						WinFuntions.DataRowToDataRow(dataRow6, dataRow5);
						dataRow5["Caption"] = gv_Data.Columns[num3].Caption;
						dataRow5["Width"] = gv_Data.Columns[num3].Width;
						dataRow5["Visible"] = gv_Data.Columns[num3].Visible;
						dataRow5["GroupIndex"] = gv_Data.Columns[num3].GroupIndex;
						dataRow5["VisIndex"] = gv_Data.Columns[num3].VisibleIndex;
						if (!gv_Data.Columns[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
					}
					else
					{
						dataRow5["ColumnName"] = colName2;
						dataRow5["DataType"] = (dt.IsExistsColumn(colName2) ? GetColumnType(dt.Columns[colName2].DataType) : Function.ToString(gv_Data.Columns[num3].ColumnType));
						dataRow5["Caption"] = gv_Data.Columns[num3].Caption;
						dataRow5["Width"] = gv_Data.Columns[num3].Width;
						dataRow5["Visible"] = gv_Data.Columns[num3].Visible;
						dataRow5["GroupIndex"] = gv_Data.Columns[num3].GroupIndex;
						dataRow5["VisIndex"] = gv_Data.Columns[num3].VisibleIndex;
						if (!gv_Data.Columns[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
						dataRow5["ReadOnly"] = gv_Data.Columns[num3].OptionsColumn.ReadOnly;
						dataRow5["AllowEdit"] = gv_Data.Columns[num3].OptionsColumn.AllowEdit;
						dataRow5["AllowMove"] = gv_Data.Columns[num3].OptionsColumn.AllowMove;
						dataRow5["AllowSize"] = gv_Data.Columns[num3].OptionsColumn.AllowSize;
						dataRow5["AllowSort"] = gv_Data.Columns[num3].OptionsColumn.AllowSort;
					}
					ds.Tables[1].Rows.Add(dataRow5);
				}
				ds.Tables[1].Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
				DataView defaultView2 = ds.Tables[1].DefaultView;
				ds.Tables[1].DefaultView.Sort = "Visible DESC, VisIndex ASC";
				dtStructure = defaultView2.ToTable("Structure");
				DataTable dataTable2 = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable2 = ds.Tables[2].Copy();
					dataTable2.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable2);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Setting.HideLoadingPanel();
		}
	}

	private async Task LoadSettingTreeView()
	{
		_ = 2;
		try
		{
			DataTable dt = tree.DataSourceTable();
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(id, formid, itemid, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (!ds.IsHasStructure())
			{
				return;
			}
			if (dtStructure.IsNotNull())
			{
				int num = tree.Columns.Max((TreeListColumn m) => m.VisibleIndex) + 1;
				foreach (TreeListColumn column in tree.Columns)
				{
					string colName = column.FieldName;
					DataRow dataRow = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName)
						select c).FirstOrDefault();
					if (dataRow != null)
					{
						dataRow["Caption"] = column.Caption;
						dataRow["Width"] = column.Width;
						dataRow["Visible"] = column.Visible;
						dataRow["VisIndex"] = column.VisibleIndex;
						dataRow["ReadOnly"] = column.OptionsColumn.ReadOnly;
						if (!column.Visible)
						{
							dataRow["VisIndex"] = num;
							num++;
						}
						continue;
					}
					DataRow dataRow2 = dtStructure.NewRow();
					dataRow2["ColumnName"] = colName;
					dataRow2["DataType"] = GetColumnType(dt.Columns[colName].DataType);
					dataRow2["Caption"] = column.Caption;
					dataRow2["Width"] = column.Width;
					dataRow2["Visible"] = column.Visible;
					dataRow2["VisIndex"] = column.VisibleIndex;
					if (!column.Visible)
					{
						dataRow2["VisIndex"] = num;
						num++;
					}
					dataRow2["ReadOnly"] = column.OptionsColumn.ReadOnly;
					dataRow2["AllowEdit"] = column.OptionsColumn.AllowEdit;
					dataRow2["AllowMove"] = column.OptionsColumn.AllowMove;
					dataRow2["AllowSize"] = column.OptionsColumn.AllowSize;
					dataRow2["AllowSort"] = column.OptionsColumn.AllowSort;
					dataRow2["Key"] = tree.KeyFieldName.Equals(column.FieldName);
					dataRow2["Parent"] = tree.ParentFieldName.Equals(column.FieldName);
					dtStructure.Rows.Add(dataRow2);
				}
				dtStructure.Rows[0]["FilterString"] = tree.ActiveFilterString;
				if (dtStructure.Rows.Count > tree.Columns.Count)
				{
					List<DataRow> list = new List<DataRow>();
					foreach (DataRow row in dtStructure.Rows)
					{
						string text = Function.ToString(row["ColumnName"]);
						if (!tree.IsExistColumn(text) && tree.KeyFieldName.Equals(text) && tree.ParentFieldName.Equals(text))
						{
							list.Add(row);
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
				DataView defaultView = dtStructure.DefaultView;
				dtStructure.DefaultView.Sort = "VisIndex ASC";
				dtStructure = defaultView.ToTable("Structure");
				DataTable dataTable = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable = ds.Tables[2].Copy();
					dataTable.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
			else
			{
				if (ds.Tables[1] == null)
				{
					return;
				}
				if (dtStructure == null)
				{
					dtStructure = new DataTable();
				}
				int num2 = tree.Columns.Max((TreeListColumn m) => m.VisibleIndex) + 1;
				for (int num3 = 0; num3 < tree.Columns.Count; num3++)
				{
					string colName2 = tree.Columns[num3].FieldName;
					DataRow dataRow4 = (from c in ds.Tables[1].AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow4 != null)
					{
						dataRow4["Caption"] = tree.Columns[num3].Caption;
						dataRow4["Width"] = tree.Columns[num3].Width;
						dataRow4["Visible"] = tree.Columns[num3].Visible;
						dataRow4["VisIndex"] = tree.Columns[num3].VisibleIndex;
						if (!tree.Columns[num3].Visible)
						{
							dataRow4["VisIndex"] = num2;
							num2++;
						}
						continue;
					}
					DataRow dataRow5 = ds.Tables[1].NewRow();
					DataRow dataRow6 = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow6 != null)
					{
						WinFuntions.DataRowToDataRow(dataRow6, dataRow5);
						dataRow5["Caption"] = tree.Columns[num3].Caption;
						dataRow5["Width"] = tree.Columns[num3].Width;
						dataRow5["Visible"] = tree.Columns[num3].Visible;
						dataRow5["Key"] = tree.KeyFieldName.Equals(tree.Columns[num3].FieldName);
						dataRow5["Parent"] = tree.ParentFieldName.Equals(tree.Columns[num3].FieldName);
						dataRow5["VisIndex"] = tree.Columns[num3].VisibleIndex;
						if (!tree.Columns[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
					}
					else
					{
						dataRow5["ColumnName"] = colName2;
						dataRow5["DataType"] = (dt.IsExistsColumn(colName2) ? GetColumnType(dt.Columns[colName2].DataType) : Function.ToString(tree.Columns[num3].ColumnType));
						dataRow5["Caption"] = tree.Columns[num3].Caption;
						dataRow5["Width"] = tree.Columns[num3].Width;
						dataRow5["Visible"] = tree.Columns[num3].Visible;
						dataRow5["VisIndex"] = tree.Columns[num3].VisibleIndex;
						if (!tree.Columns[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
						dataRow5["ReadOnly"] = tree.Columns[num3].OptionsColumn.ReadOnly;
						dataRow5["AllowEdit"] = tree.Columns[num3].OptionsColumn.AllowEdit;
						dataRow5["AllowMove"] = tree.Columns[num3].OptionsColumn.AllowMove;
						dataRow5["AllowSize"] = tree.Columns[num3].OptionsColumn.AllowSize;
						dataRow5["AllowSort"] = tree.Columns[num3].OptionsColumn.AllowSort;
						dataRow5["Key"] = tree.KeyFieldName.Equals(tree.Columns[num3].FieldName);
						dataRow5["Parent"] = tree.ParentFieldName.Equals(tree.Columns[num3].FieldName);
					}
					ds.Tables[1].Rows.Add(dataRow5);
				}
				ds.Tables[1].Rows[0]["FilterString"] = tree.ActiveFilterString;
				DataView defaultView2 = ds.Tables[1].DefaultView;
				ds.Tables[1].DefaultView.Sort = "VisIndex ASC";
				dtStructure = defaultView2.ToTable("Structure");
				DataTable dataTable2 = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable2 = ds.Tables[2].Copy();
					dataTable2.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable2);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSettingPivotGrid()
	{
		_ = 2;
		try
		{
			DataTable dt = pivot.DataSourceTable();
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(id, formid, itemid, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (!ds.IsHasStructure())
			{
				return;
			}
			if (dtStructure.IsNotNull())
			{
				int num = pivot.Fields.Count;
				foreach (PivotGridField field in pivot.Fields)
				{
					string colName = field.FieldName;
					DataRow dataRow = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName)
						select c).FirstOrDefault();
					if (dataRow != null)
					{
						dataRow["Caption"] = field.Caption;
						dataRow["Width"] = field.Width;
						dataRow["Visible"] = field.Visible;
						dataRow["VisIndex"] = field.AreaIndex;
						dataRow["ReadOnly"] = field.Options.ReadOnly;
						dataRow["AllowSize"] = true;
						switch (field.Area)
						{
						case PivotArea.RowArea:
							dataRow["PivotArea"] = "row";
							break;
						case PivotArea.ColumnArea:
							dataRow["PivotArea"] = "column";
							break;
						case PivotArea.FilterArea:
							dataRow["PivotArea"] = "filter";
							break;
						case PivotArea.DataArea:
							dataRow["PivotArea"] = "data";
							break;
						default:
							dataRow["PivotArea"] = "row";
							break;
						}
						if (!field.Visible)
						{
							dataRow["VisIndex"] = num;
							num++;
						}
						continue;
					}
					DataRow dataRow2 = dtStructure.NewRow();
					dataRow2["ColumnName"] = colName;
					dataRow2["Caption"] = field.Caption;
					dataRow2["DataType"] = GetColumnType(dt.Columns[colName].DataType);
					dataRow2["Width"] = field.Width;
					dataRow2["Visible"] = field.Visible;
					dataRow2["VisIndex"] = field.AreaIndex;
					dataRow2["ReadOnly"] = field.Options.ReadOnly;
					dataRow2["AllowEdit"] = true;
					dataRow2["AllowMove"] = true;
					dataRow2["AllowSize"] = true;
					dataRow2["AllowSort"] = true;
					switch (field.Area)
					{
					case PivotArea.RowArea:
						dataRow2["PivotArea"] = "row";
						break;
					case PivotArea.ColumnArea:
						dataRow2["PivotArea"] = "column";
						break;
					case PivotArea.FilterArea:
						dataRow2["PivotArea"] = "filter";
						break;
					case PivotArea.DataArea:
						dataRow2["PivotArea"] = "data";
						break;
					default:
						dataRow2["PivotArea"] = "row";
						break;
					}
					if (!field.Visible)
					{
						dataRow2["VisIndex"] = num;
						num++;
					}
					dtStructure.Rows.Add(dataRow2);
				}
				dtStructure.Rows[0]["FilterString"] = pivot.ActiveFilterString;
				if (dtStructure.Rows.Count > pivot.Fields.Count)
				{
					List<DataRow> list = new List<DataRow>();
					foreach (DataRow row in dtStructure.Rows)
					{
						string colname = Function.ToString(row["ColumnName"]);
						if (pivot.Fields.Cast<PivotGridField>().FirstOrDefault((PivotGridField t) => t.FieldName == colname) == null)
						{
							list.Add(row);
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
				DataView defaultView = dtStructure.DefaultView;
				dtStructure.DefaultView.Sort = "VisIndex ASC";
				dtStructure = defaultView.ToTable("Structure");
				DataTable dataTable = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable = ds.Tables[2].Copy();
					dataTable.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
			else
			{
				if (ds.Tables[1] == null)
				{
					return;
				}
				if (dtStructure == null)
				{
					dtStructure = new DataTable();
				}
				int num2 = pivot.Fields.Count;
				for (int num3 = 0; num3 < pivot.Fields.Count; num3++)
				{
					string colName2 = pivot.Fields[num3].FieldName;
					DataRow dataRow4 = (from c in ds.Tables[1].AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow4 != null)
					{
						dataRow4["Caption"] = pivot.Fields[num3].Caption;
						dataRow4["Width"] = pivot.Fields[num3].Width;
						dataRow4["Visible"] = pivot.Fields[num3].Visible;
						dataRow4["VisIndex"] = pivot.Fields[num3].AreaIndex;
						if (!pivot.Fields[num3].Visible)
						{
							dataRow4["VisIndex"] = num2;
							num2++;
						}
						continue;
					}
					DataRow dataRow5 = ds.Tables[1].NewRow();
					DataRow dataRow6 = (from c in dtStructure.AsEnumerable()
						where c.Field<string>("ColumnName").Equals(colName2)
						select c).FirstOrDefault();
					if (dataRow6 != null)
					{
						WinFuntions.DataRowToDataRow(dataRow6, dataRow5);
						dataRow5["Caption"] = pivot.Fields[num3].Caption;
						dataRow5["Width"] = pivot.Fields[num3].Width;
						dataRow5["Visible"] = pivot.Fields[num3].Visible;
						dataRow5["VisIndex"] = pivot.Fields[num3].AreaIndex;
						switch (pivot.Fields[num3].Area)
						{
						case PivotArea.RowArea:
							dataRow5["PivotArea"] = "row";
							break;
						case PivotArea.ColumnArea:
							dataRow5["PivotArea"] = "column";
							break;
						case PivotArea.FilterArea:
							dataRow5["PivotArea"] = "filter";
							break;
						case PivotArea.DataArea:
							dataRow5["PivotArea"] = "data";
							break;
						default:
							dataRow5["PivotArea"] = "row";
							break;
						}
						if (!pivot.Fields[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
					}
					else
					{
						dataRow5["ColumnName"] = colName2;
						dataRow5["DataType"] = (dt.IsExistsColumn(colName2) ? GetColumnType(dt.Columns[colName2].DataType) : Function.ToString(pivot.Fields[num3].DataType));
						dataRow5["Caption"] = pivot.Fields[num3].Caption;
						dataRow5["Width"] = pivot.Fields[num3].Width;
						dataRow5["Visible"] = pivot.Fields[num3].Visible;
						dataRow5["VisIndex"] = pivot.Fields[num3].AreaIndex;
						if (!pivot.Fields[num3].Visible)
						{
							dataRow5["VisIndex"] = num2;
							num2++;
						}
						dataRow5["ReadOnly"] = pivot.Fields[num3].Options.ReadOnly;
						dataRow5["AllowEdit"] = pivot.Fields[num3].Options.AllowEdit;
						dataRow5["AllowMove"] = pivot.Fields[num3].Options.AllowDrag;
						dataRow5["AllowSort"] = pivot.Fields[num3].Options.AllowSort;
						switch (pivot.Fields[num3].Area)
						{
						case PivotArea.RowArea:
							dataRow5["PivotArea"] = "row";
							break;
						case PivotArea.ColumnArea:
							dataRow5["PivotArea"] = "column";
							break;
						case PivotArea.FilterArea:
							dataRow5["PivotArea"] = "filter";
							break;
						case PivotArea.DataArea:
							dataRow5["PivotArea"] = "data";
							break;
						default:
							dataRow5["PivotArea"] = "row";
							break;
						}
					}
					ds.Tables[1].Rows.Add(dataRow5);
				}
				ds.Tables[1].Rows[0]["FilterString"] = pivot.ActiveFilterString;
				DataView defaultView2 = ds.Tables[1].DefaultView;
				ds.Tables[1].DefaultView.Sort = "VisIndex ASC";
				dtStructure = defaultView2.ToTable("Structure");
				DataTable dataTable2 = new DataTable();
				if (ds.Tables.Count > 2)
				{
					dataTable2 = ds.Tables[2].Copy();
					dataTable2.TableName = "Valid";
					ds.Tables.RemoveAt(2);
				}
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dtStructure.Copy());
				ds.Tables.Add(dataTable2);
				await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
				ds.DisposeDataSet();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Setting_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		isClose = true;
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				isClose = true;
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			btn_OK.Enabled = false;
			await SaveData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
	}

	private async Task SaveData()
	{
		WinCommons._OpenProcessing();
		try
		{
			DataSet ds = new DataSet();
			DataTable dataTable = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).CopyToDataTable();
			new DataTable();
			DataTable dataTable2 = dtValidValue.Copy();
			DataTable dataTable3 = grd_Setting.DataSourceTable();
			if (dataTable3.IsNotNull())
			{
				for (int i = 0; i < dataTable3.Rows.Count; i++)
				{
					dataTable3.Rows[i]["ID"] = dataTable.Rows[0]["ID"];
					dataTable3.Rows[i]["VisIndex"] = i;
				}
				DataTable dataTable4 = dataTable3.Copy();
				dataTable.Rows[0]["Usersign2"] = GlobalData.UserName;
				dataTable.TableName = "OFST";
				dataTable4.TableName = "FST1";
				dataTable2.TableName = "FST2";
				ds.Tables.Add(dataTable);
				ds.Tables.Add(dataTable4);
				ds.Tables.Add(dataTable2);
				new HttpResult();
				HttpResult httpResult = await _httpClient.FormSettingClient.Save(ds);
				ds.DisposeDataSet();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
					FormMode = Enums.FormViewModeType.View;
					btn_New.Enabled = true;
					btn_Del.Enabled = true;
					FormMode = Enums.FormViewModeType.View;
					isSave = true;
					id = Function.ParseInt(httpResult.Content);
					await GetListFormSetting();
					dtStructure = grd_Setting.DataSourceTable().Copy();
					WinCommons._CloseProcessing();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetListFormSetting()
	{
		isNotChangeSetting = true;
		try
		{
			DataTable dataTable = await _httpClient.FormSettingClient.GetListFormSetting(formid, itemid, GlobalData.UserName);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				if (id == 0)
				{
					dataTable.Rows.Clear();
					DataRow dataRow = dataTable.NewRow();
					dataRow["ID"] = 0;
					dataRow["Name"] = "New Setting";
					dataRow["FormId"] = formid;
					dataRow["ItemId"] = itemid;
					dataRow["SetDefault"] = true;
					dataRow["UserSign"] = GlobalData.UserName;
					dataTable.Rows.Add(dataRow);
				}
				lue_FormSetting.Properties.Columns.Clear();
				lue_FormSetting.Properties.DataSource = null;
				lue_FormSetting.Properties.DataSource = dataTable;
				lue_FormSetting.Properties.DisplayMember = "Name";
				lue_FormSetting.Properties.ValueMember = "ID";
				lue_FormSetting.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				lue_FormSetting.Properties.PopulateColumns();
				for (int i = 0; i < lue_FormSetting.Properties.Columns.Count; i++)
				{
					if (!lue_FormSetting.Properties.Columns[i].FieldName.Equals("Name"))
					{
						lue_FormSetting.Properties.Columns[i].Visible = false;
					}
				}
				lue_FormSetting.EditValue = id;
			}
			if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
			{
				chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
			}
			if (id != 0)
			{
				dtValidValue = await _httpClient.FormSettingClient.GetValidData(formid, itemid, GlobalData.UserName, id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isNotChangeSetting = false;
	}

	public string GetColumnType(Type dataType)
	{
		if (dataType == typeof(string))
		{
			return "nvarchar";
		}
		if (dataType == typeof(int))
		{
			return "int";
		}
		if (dataType == typeof(decimal) || dataType == typeof(double))
		{
			return "numeric";
		}
		if (dataType == typeof(DateTime))
		{
			return "datetime";
		}
		if (dataType == typeof(bool))
		{
			return "bit";
		}
		return "nvarchar";
	}

	private void btn_New_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
			{
				return;
			}
			WinCommons._OpenProcessing();
			isNotChangeSetting = true;
			string value = XtraInputBox.Show("Enter a new name", "Edit Name Settings", "");
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			DataTable dataTable = (DataTable)lue_FormSetting.Properties.DataSource;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["ID"] = 0;
			dataRow["Name"] = value;
			dataRow["FormId"] = formid;
			dataRow["ItemId"] = itemid;
			dataRow["Usersign"] = GlobalData.UserName;
			dataTable.Rows.Add(dataRow);
			lue_FormSetting.EditValue = 0;
			FormMode = Enums.FormViewModeType.Add;
			btn_New.Enabled = false;
			btn_Del.Enabled = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isNotChangeSetting = false;
	}

	private void btn_EditName_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow dataRow = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).FirstOrDefault();
			if (dataRow != null)
			{
				string text = Function.ToString(dataRow["Name"]);
				string text2 = XtraInputBox.Show("Enter a new name", "Edit Name Settings", text);
				dataRow["Name"] = (string.IsNullOrEmpty(text2) ? text : text2);
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_FormSettings_FormClosing(object sender, FormClosingEventArgs e)
	{
		_ = 3;
		try
		{
			if (!isClose && FormMode != Enums.FormViewModeType.View)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
				case DialogResult.Yes:
					await SaveData();
					if (FormMode != Enums.FormViewModeType.View)
					{
						e.Cancel = true;
					}
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				}
			}
			if (!isSave)
			{
				return;
			}
			DataSet dsNew = new DataSet();
			DataTable dataTable = new DataTable("Data");
			GridViewEx gridViewEx = (GridViewEx)grid.DefaultView;
			if (viewType.Equals("1"))
			{
				dataTable = grid.DatasourceTable();
				gridViewEx.GroupSummary.Clear();
				if (formid != "oSalaryResult" && formid != "oSalaryResultView")
				{
					gridViewEx.ColumnsEx.Clear();
				}
				else
				{
					foreach (GridColumn item in gridViewEx.Columns.Where((GridColumn r) => r.FieldName.StartsWith("U_")).ToList())
					{
						gridViewEx.Columns.Remove(item);
					}
				}
			}
			else if (viewType.Equals("2"))
			{
				dataTable = tree.DataSourceTable();
				tree.Columns.Clear();
			}
			else if (viewType.Equals("3"))
			{
				dataTable = pivot.DataSourceTable();
				pivot.Fields.Clear();
			}
			dsNew.Tables.Add(grd_Setting.DataSourceTable().Copy());
			dsNew.Tables.Add(dataTable.Copy());
			dsNew.Tables.Add(dtValidValue.Copy());
			if (viewType.Equals("1"))
			{
				await GridViewHelper.GenerateGrid(grid, gridViewEx, dsNew, null, expand: true);
			}
			else if (viewType.Equals("2"))
			{
				await TreeListHelper.GenerateTreeList(tree, dsNew, null, expand: true);
			}
			else if (viewType.Equals("3"))
			{
				await PivotGridHelper.GeneratePivotGrid(pivot, dsNew, null, expand: true);
			}
			dsNew.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lue_FormSetting_EditValueChanged(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (isNotChangeSetting || (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK))
			{
				return;
			}
			WinCommons._OpenProcessing();
			int selectedId = Function.ParseInt(lue_FormSetting.EditValue);
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(selectedId, formid, itemid, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (!ds.IsHasStructure())
			{
				return;
			}
			isLoadForm = true;
			dtStructure = ds.Tables[1].Copy();
			await GridViewHelper.GenerateGrid(grd_Setting, gv_Setting, ds, null, expand: true);
			dtValidValue = await _httpClient.FormSettingClient.GetValidData(formid, itemid, GlobalData.UserName, selectedId);
			if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
			{
				chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
			}
			id = Function.ParseInt(lue_FormSetting.EditValue);
			ds.DisposeDataSet();
			FormMode = Enums.FormViewModeType.View;
			isSave = true;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isLoadForm = false;
	}

	private void lue_FormSetting_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add && !isNotChangeSetting)
			{
				WinCommons.Warning(Enums.MessageCode.SaveDataBeforeChanging, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditValidValue_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow[] array = grd_Setting.DatasourceTable().Select("DataType = 'validvalue' OR DataType = 'validcolor' OR DataType = 'color'");
			if (array != null && array.Length != 0)
			{
				frm_ValidValue frm_ValidValue2 = new frm_ValidValue(array.CopyToDataTable(), dtValidValue);
				string text = Function.ToString(gv_Setting.GetFocusedRowCellValue("DataType"));
				if (text.Equals("validvalue") || text.Equals("validcolor") || text.Equals("color"))
				{
					frm_ValidValue2.dfColumn = Function.ToString(gv_Setting.GetFocusedRowCellValue("ColumnName"));
				}
				frm_ValidValue2._id = id;
				frm_ValidValue2.ShowDialog(this);
				if (frm_ValidValue2.isSave)
				{
					dtValidValue = frm_ValidValue2._dtData.Copy();
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Setting_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			downHitInfo = null;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
				if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
				{
					downHitInfo = gridHitInfo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Setting_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Button == MouseButtons.Left && downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridViewEx?.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
					downHitInfo = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Setting_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
			{
				GridHitInfo gridHitInfo2 = mainView.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Setting_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow)
	{
		try
		{
			if (sourceRow != targetRow)
			{
				DataTable dataTable = grd_Setting.DatasourceTable();
				DataRow dataRow = dataTable.Rows[sourceRow];
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				dataTable.Rows.Remove(dataRow);
				dataTable.Rows.InsertAt(dataRow2, targetRow);
				dataTable.AcceptChanges();
				gv_Setting.FocusedRowHandle = targetRow;
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Del_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_OK.Enabled = false;
				WinCommons._OpenProcessing();
				int num = Function.ParseInt(lue_FormSetting.EditValue);
				new HttpResult();
				HttpResult httpResult = await _httpClient.FormSettingClient.Delete(num);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
					FormMode = Enums.FormViewModeType.View;
					btn_OK.Enabled = true;
					btn_New.Enabled = true;
					btn_Del.Enabled = true;
					FormMode = Enums.FormViewModeType.View;
					isSave = true;
					await GetListFormSetting();
					dtStructure = grd_Setting.DataSourceTable().Copy();
					WinCommons._CloseProcessing();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
			btn_OK.Enabled = true;
			isNotChangeSetting = false;
		}
	}

	private void chk_SetDefault_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoadForm)
			{
				DataRow dataRow = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["SetDefault"] = chk_SetDefault.Checked;
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CopyFormSetting_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(lue_FormSetting.EditValue);
			if (num == 0)
			{
				WinCommons.Warning(Enums.MessageCode.SaveDataBeforeCopy, "");
			}
			else if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				frm_ReportFormSetting_Copy frm_ReportFormSetting_Copy2 = new frm_ReportFormSetting_Copy();
				frm_ReportFormSetting_Copy2.userName = GlobalData.UserName;
				frm_ReportFormSetting_Copy2.idFormSetting = num;
				frm_ReportFormSetting_Copy2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ReportFormSettings));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CopyFormSetting = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_SetDefault = new System.Windows.Forms.CheckBox();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_New = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lue_FormSetting = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_EditName = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditValidValue = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Setting = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Setting = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Del = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Setting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Setting).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 461);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 461);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 9;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_CopyFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyFormSetting.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_CopyFormSetting.Appearance.Options.UseBackColor = true;
		this.btn_CopyFormSetting.EditControl = null;
		this.btn_CopyFormSetting.EditValue = "Copy";
		this.btn_CopyFormSetting.FieldName = null;
		this.btn_CopyFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyFormSetting.ImageOptions.Image");
		this.btn_CopyFormSetting.IsSearchControl = false;
		this.btn_CopyFormSetting.Location = new System.Drawing.Point(777, 461);
		this.btn_CopyFormSetting.Name = "btn_CopyFormSetting";
		this.btn_CopyFormSetting.ReadOnly = false;
		this.btn_CopyFormSetting.SettingInVisible = false;
		this.btn_CopyFormSetting.SettingReadonly = false;
		this.btn_CopyFormSetting.Size = new System.Drawing.Size(72, 22);
		this.btn_CopyFormSetting.TabIndex = 10;
		this.btn_CopyFormSetting.Text = "Copy";
		this.btn_CopyFormSetting.Click += new System.EventHandler(this.btn_CopyFormSetting_Click);
		this.chk_SetDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_SetDefault.AutoSize = true;
		this.chk_SetDefault.Location = new System.Drawing.Point(504, 464);
		this.chk_SetDefault.Name = "chk_SetDefault";
		this.chk_SetDefault.Size = new System.Drawing.Size(94, 17);
		this.chk_SetDefault.TabIndex = 11;
		this.chk_SetDefault.Text = "Set as Default";
		this.chk_SetDefault.UseVisualStyleBackColor = true;
		this.chk_SetDefault.CheckedChanged += new System.EventHandler(this.chk_SetDefault_CheckedChanged);
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Setting";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(168, 464);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(52, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 31;
		this.labelLine1.Utext = "Setting";
		this.btn_New.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_New.EditControl = null;
		this.btn_New.EditValue = "";
		this.btn_New.FieldName = null;
		this.btn_New.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_New.ImageOptions.Image");
		this.btn_New.IsSearchControl = false;
		this.btn_New.Location = new System.Drawing.Point(427, 461);
		this.btn_New.Name = "btn_New";
		this.btn_New.ReadOnly = false;
		this.btn_New.SettingInVisible = false;
		this.btn_New.SettingReadonly = false;
		this.btn_New.Size = new System.Drawing.Size(26, 23);
		this.btn_New.TabIndex = 32;
		this.btn_New.ToolTip = "Create New Setting";
		this.btn_New.Click += new System.EventHandler(this.btn_New_Click);
		this.lue_FormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lue_FormSetting.Location = new System.Drawing.Point(216, 462);
		this.lue_FormSetting.Name = "lue_FormSetting";
		this.lue_FormSetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_FormSetting.Properties.NullText = "";
		this.lue_FormSetting.Size = new System.Drawing.Size(172, 20);
		this.lue_FormSetting.TabIndex = 35;
		this.lue_FormSetting.EditValueChanged += new System.EventHandler(this.lue_FormSetting_EditValueChanged);
		this.lue_FormSetting.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.lue_FormSetting_EditValueChanging);
		this.btn_EditName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_EditName.EditControl = null;
		this.btn_EditName.EditValue = "";
		this.btn_EditName.FieldName = null;
		this.btn_EditName.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditName.ImageOptions.Image");
		this.btn_EditName.IsSearchControl = false;
		this.btn_EditName.Location = new System.Drawing.Point(394, 461);
		this.btn_EditName.Name = "btn_EditName";
		this.btn_EditName.ReadOnly = false;
		this.btn_EditName.SettingInVisible = false;
		this.btn_EditName.SettingReadonly = false;
		this.btn_EditName.Size = new System.Drawing.Size(27, 23);
		this.btn_EditName.TabIndex = 36;
		this.btn_EditName.ToolTip = "Edit Setting Name";
		this.btn_EditName.Click += new System.EventHandler(this.btn_EditName_Click);
		this.btn_EditValidValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditValidValue.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_EditValidValue.Appearance.Options.UseBackColor = true;
		this.btn_EditValidValue.EditControl = null;
		this.btn_EditValidValue.EditValue = "Edit Valid Value";
		this.btn_EditValidValue.FieldName = null;
		this.btn_EditValidValue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditValidValue.ImageOptions.Image");
		this.btn_EditValidValue.IsSearchControl = false;
		this.btn_EditValidValue.Location = new System.Drawing.Point(670, 461);
		this.btn_EditValidValue.Name = "btn_EditValidValue";
		this.btn_EditValidValue.ReadOnly = false;
		this.btn_EditValidValue.SettingInVisible = false;
		this.btn_EditValidValue.SettingReadonly = false;
		this.btn_EditValidValue.Size = new System.Drawing.Size(101, 23);
		this.btn_EditValidValue.TabIndex = 37;
		this.btn_EditValidValue.Text = "Edit Valid Value";
		this.btn_EditValidValue.ToolTip = "Preview";
		this.btn_EditValidValue.Click += new System.EventHandler(this.btn_EditValidValue_Click);
		this.grd_Setting.AllowDrop = true;
		this.grd_Setting.AllowPasteExcel = true;
		this.grd_Setting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Setting.ClipboardData = null;
		this.grd_Setting.Location = new System.Drawing.Point(-3, -1);
		this.grd_Setting.MainView = this.gv_Setting;
		this.grd_Setting.Name = "grd_Setting";
		this.grd_Setting.Size = new System.Drawing.Size(852, 456);
		this.grd_Setting.TabIndex = 38;
		this.grd_Setting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Setting });
		this.grd_Setting.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_Setting_DragDrop);
		this.grd_Setting.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_Setting_DragOver);
		this.gv_Setting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Setting.ColumnKey = null;
		this.gv_Setting.DisableFormSetting = false;
		this.gv_Setting.DisplayException = false;
		this.gv_Setting.EditColumn = null;
		this.gv_Setting.FieldCurrency = null;
		this.gv_Setting.FieldRate = null;
		this.gv_Setting.FormId = "oReportFormSetting";
		this.gv_Setting.GridControl = this.grd_Setting;
		this.gv_Setting.IgnoreFilter = false;
		this.gv_Setting.IndexEditRow = -1;
		this.gv_Setting.IndicatorWidth = 35;
		this.gv_Setting.IsApzTable = false;
		this.gv_Setting.IsInsertNewRow = false;
		this.gv_Setting.IsLoadingData = false;
		this.gv_Setting.IsMappingContextAction = false;
		this.gv_Setting.ItemId = "ODATA";
		this.gv_Setting.LstColumnAllowBatch = null;
		this.gv_Setting.LstColumnNotAllowBatch = null;
		this.gv_Setting.LstColumnNotEditable = null;
		this.gv_Setting.LstColumNotSetting = null;
		this.gv_Setting.Name = "gv_Setting";
		this.gv_Setting.OptionsView.ColumnAutoWidth = false;
		this.gv_Setting.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_Setting.OptionsView.ShowGroupPanel = false;
		this.gv_Setting.UseBatchMod = false;
		this.gv_Setting.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Setting_CellValueChanged);
		this.gv_Setting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Setting_MouseDown);
		this.gv_Setting.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Setting_MouseMove);
		this.btn_Del.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Del.EditControl = null;
		this.btn_Del.EditValue = "";
		this.btn_Del.FieldName = null;
		this.btn_Del.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Del.ImageOptions.Image");
		this.btn_Del.IsSearchControl = false;
		this.btn_Del.Location = new System.Drawing.Point(459, 461);
		this.btn_Del.Name = "btn_Del";
		this.btn_Del.ReadOnly = false;
		this.btn_Del.SettingInVisible = false;
		this.btn_Del.SettingReadonly = false;
		this.btn_Del.Size = new System.Drawing.Size(26, 23);
		this.btn_Del.TabIndex = 39;
		this.btn_Del.ToolTip = "Delete Setting";
		this.btn_Del.Click += new System.EventHandler(this.btn_Del_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(850, 486);
		base.Controls.Add(this.btn_Del);
		base.Controls.Add(this.grd_Setting);
		base.Controls.Add(this.btn_EditValidValue);
		base.Controls.Add(this.btn_EditName);
		base.Controls.Add(this.lue_FormSetting);
		base.Controls.Add(this.btn_New);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.chk_SetDefault);
		base.Controls.Add(this.btn_CopyFormSetting);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oReportFormSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReportFormSettings";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Form Settings";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FormSettings_FormClosing);
		base.Load += new System.EventHandler(this.frm_FormSettings_Load);
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Setting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Setting).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
