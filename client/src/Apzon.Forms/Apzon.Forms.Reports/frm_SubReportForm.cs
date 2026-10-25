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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Reports;

public class frm_SubReportForm : XtraFormEx
{
	public IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private TreeEx _tree;

	private int structureId;

	private DataTable defaultStructure = new DataTable("Structure");

	private DataTable defaultValid = new DataTable("Valid");

	public bool isExecuteWhenLoad;

	public List<Common.ParameterValue> defaultParamValue = new List<Common.ParameterValue>();

	public string _fileName = string.Empty;

	public string ItemId = string.Empty;

	private string tableName = string.Empty;

	public DataRow dataRow;

	public string _tableType = "1";

	private bool isNotChangeSetting;

	private bool isLoadForm;

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private List<string> lstFormOpen = new List<string>();

	private IContainer components;

	private GridControlEx grd_Data;

	private SimpleButtonEx btn_Execute;

	private SimpleButtonEx btn_Cancel;

	private GridViewEx gv_Data;

	private CheckBox chk_SetDefault;

	private LabelLine lbl_FormSetting;

	private LookUpEdit lue_FormSetting;

	private SimpleButtonEx btn_SaveFormSetting;

	private PanelControl panelControl1;

	public frm_SubReportForm()
	{
		InitializeComponent();
		gv_Data.FormId = ApzObjectTypes.oSubReportForm.ToString();
	}

	public frm_SubReportForm(string storeName, List<Common.ParameterValue> lstParamValue = null)
	{
		InitializeComponent();
		FormId = ApzObjectTypes.oSubReportForm.ToString();
		ItemId = storeName;
		gv_Data.FormId = ApzObjectTypes.oSubReportForm.ToString();
		gv_Data.ItemId = storeName;
		if (lstParamValue != null && lstParamValue.Count > 0)
		{
			defaultParamValue = lstParamValue;
		}
	}

	private async void frm_SubReportForm_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrEmpty(ItemId))
			{
				if (string.IsNullOrEmpty(tableName))
				{
					lstUdf = await _httpClient.Udf.GetListField(tableName);
				}
				await GetListFormSetting();
				if (_tableType.Equals("2"))
				{
					_tree = new TreeEx();
					panelControl1.Controls.Clear();
					panelControl1.Controls.Add(_tree);
					_tree.Dock = DockStyle.Fill;
				}
				bt_Execute_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_Execute_Click(object sender, EventArgs e)
	{
		_ = 7;
		try
		{
			WinCommons._OpenProcessing();
			isNotChangeSetting = true;
			btn_Execute.Enabled = false;
			DataSetResult dataSetResult = await _httpClient.Commons.ExecuteStore(ItemId, defaultParamValue);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				defaultStructure = ds.Tables[0].Copy();
			}
			if (ds.IsHasValidValue())
			{
				defaultValid = ds.Tables[2].Copy();
			}
			if (lue_FormSetting.EditValue != null && Function.ParseInt(lue_FormSetting.EditValue) > 0)
			{
				DataSet dsNew = new DataSet();
				DataTable dataTable = new DataTable("Data");
				if (ds.IsNotNull() && ds.Tables.Count == 1)
				{
					dataTable = ds.Tables[0].Copy();
				}
				else if (ds.IsNotNull() && ds.Tables.Count > 1)
				{
					dataTable = ds.Tables[1].Copy();
				}
				dsNew.Tables.Add(defaultStructure.Copy());
				dsNew.Tables.Add(dataTable.Copy());
				dsNew.Tables.Add(defaultValid.Copy());
				if (_tableType.Equals("1"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, dsNew, lstUdf, expand: true);
				}
				dsNew.DisposeDataSet();
				return;
			}
			DataSet dsStruc = await _httpClient.FormSettingClient.GetStructure(0, ApzObjectTypes.oSubReportForm.ToString(), ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "0");
			if (dsStruc.IsNotNull() && dsStruc.Tables[0].IsNotNull())
			{
				structureId = Function.ParseInt(dsStruc.Tables[0].Rows[0]["ID"]);
				dsStruc.Tables[0].TableName = "Structure";
				defaultStructure = dsStruc.Tables[0].Copy();
				if (dsStruc.IsHasStructure())
				{
					dsStruc.Tables[1].TableName = "Valid";
					defaultValid = dsStruc.Tables[1].Copy();
					dsStruc.Tables.RemoveAt(1);
				}
				for (int i = 0; i < ds.Tables.Count; i++)
				{
					dsStruc.Tables.Add(ds.Tables[i].Copy());
				}
				dsStruc.Tables.Add(defaultValid.Copy());
				if (!_tableType.Equals("2"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsStruc, lstUdf, expand: true);
				}
				else
				{
					await TreeListHelper.GenerateTreeList(_tree, dsStruc, lstUdf, expand: true);
				}
			}
			else
			{
				if (ds.IsHasStructure() && ds.Tables[0].IsExistsColumn("ID"))
				{
					structureId = Function.ParseInt(ds.Tables[0].Rows[0]["ID"]);
					int num = Function.ParseInt(ds.Tables[0].Rows[1]["ID"]);
					if (structureId != num)
					{
						structureId = 0;
					}
				}
				if (!_tableType.Equals("2"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, ds, lstUdf, expand: true);
				}
				else
				{
					await TreeListHelper.GenerateTreeList(_tree, ds, lstUdf, expand: true);
				}
			}
			ds.DisposeDataSet();
			dsStruc.DisposeDataSet();
			lue_FormSetting.EditValue = structureId;
			if (structureId == 0)
			{
				chk_SetDefault.Visible = false;
				btn_SaveFormSetting.Visible = false;
				lbl_FormSetting.Visible = false;
				lue_FormSetting.Visible = false;
			}
			else if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
			{
				chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btn_Execute.Enabled = true;
			isNotChangeSetting = false;
			WinCommons._CloseProcessing();
		}
	}

	private async void btn_SaveFormSetting_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveFormSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lue_FormSetting_EditValueChanged(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (isNotChangeSetting)
			{
				return;
			}
			WinCommons._OpenProcessing();
			isLoadForm = true;
			structureId = Function.ParseInt(lue_FormSetting.EditValue);
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(structureId, ApzObjectTypes.oSubReportForm.ToString(), ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (ds.IsHasStructure())
			{
				defaultStructure = ds.Tables[1].Copy();
				defaultValid = await _httpClient.FormSettingClient.GetValidData(ApzObjectTypes.oSubReportForm.ToString(), ItemId, GlobalData.UserName, structureId);
				DataSet dsNew = new DataSet();
				DataTable dataTable = new DataTable("Data");
				if (_tableType.Equals("1"))
				{
					dataTable = grd_Data.DatasourceTable();
					gv_Data.GroupSummary.Clear();
					gv_Data.ColumnsEx.Clear();
				}
				else if (_tableType.Equals("2"))
				{
					dataTable = _tree.DataSourceTable();
					_tree.Columns.Clear();
				}
				dsNew.Tables.Add(defaultStructure.Copy());
				dsNew.Tables.Add(dataTable.Copy());
				dsNew.Tables.Add(defaultValid.Copy());
				if (_tableType.Equals("1"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, dsNew, lstUdf, expand: true);
				}
				dsNew.DisposeDataSet();
				if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
				{
					chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
				}
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
			isLoadForm = false;
			WinCommons._CloseProcessing();
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
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveFormSetting()
	{
		WinCommons._OpenProcessing();
		try
		{
			btn_Execute.Enabled = false;
			btn_SaveFormSetting.Enabled = false;
			DataSet ds = new DataSet();
			DataTable dataTable = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).CopyToDataTable();
			new DataTable();
			DataTable dataTable2 = defaultValid.Copy();
			if (defaultStructure.IsNotNull())
			{
				if (_tableType.Equals("1"))
				{
					int num = gv_Data.Columns.Max((GridColumn m) => m.VisibleIndex) + 1;
					foreach (GridColumnEx column in gv_Data.Columns)
					{
						string colName = column.FieldName;
						DataRow dataRow = (from c in defaultStructure.AsEnumerable()
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
						}
					}
					defaultStructure.Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
					DataView defaultView = defaultStructure.DefaultView;
					defaultStructure.DefaultView.Sort = "VisIndex ASC";
					defaultStructure = defaultView.ToTable("Structure");
				}
				else if (_tableType.Equals("2"))
				{
					int num2 = _tree.Columns.Max((TreeListColumn m) => m.VisibleIndex) + 1;
					foreach (TreeListColumn column2 in _tree.Columns)
					{
						string colName2 = column2.FieldName;
						DataRow dataRow2 = (from c in defaultStructure.AsEnumerable()
							where c.Field<string>("ColumnName").Equals(colName2)
							select c).FirstOrDefault();
						if (dataRow2 != null)
						{
							dataRow2["Caption"] = column2.Caption;
							dataRow2["Width"] = column2.Width;
							dataRow2["Visible"] = column2.Visible;
							dataRow2["VisIndex"] = column2.VisibleIndex;
							if (!column2.Visible)
							{
								dataRow2["VisIndex"] = num2;
								num2++;
							}
						}
					}
					defaultStructure.Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
					DataView defaultView2 = defaultStructure.DefaultView;
					defaultStructure.DefaultView.Sort = "VisIndex ASC";
					defaultStructure = defaultView2.ToTable("Structure");
				}
				DataTable dataTable3 = defaultStructure.Copy();
				dataTable.Rows[0]["Usersign2"] = GlobalData.UserName;
				dataTable.TableName = "OFST";
				dataTable3.TableName = "FST1";
				dataTable2.TableName = "FST2";
				ds.Tables.Add(dataTable);
				ds.Tables.Add(dataTable3);
				ds.Tables.Add(dataTable2);
				new HttpResult();
				HttpResult httpResult = await _httpClient.FormSettingClient.Save(ds);
				ds.DisposeDataSet();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotLoadStructure, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_Execute.Enabled = true;
			btn_SaveFormSetting.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	private async Task GetListFormSetting()
	{
		try
		{
			isLoadForm = true;
			DataTable dataTable = await _httpClient.FormSettingClient.GetListFormSetting(FormId, ItemId, GlobalData.UserName);
			if (dataTable.IsNotNull())
			{
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
				chk_SetDefault.Visible = true;
				btn_SaveFormSetting.Visible = true;
				lbl_FormSetting.Visible = true;
				lue_FormSetting.Visible = true;
			}
			else
			{
				chk_SetDefault.Visible = false;
				btn_SaveFormSetting.Visible = false;
				lbl_FormSetting.Visible = false;
				lue_FormSetting.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isLoadForm = false;
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			if (!string.IsNullOrEmpty(_fileName) && defaultParamValue != null)
			{
				string text = await WinFuntions.ReportViewerListParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{_fileName}", defaultParamValue);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			if (!string.IsNullOrEmpty(_fileName) && defaultParamValue != null)
			{
				string text = await WinFuntions.ReportViewerListParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{_fileName}", defaultParamValue);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public new async void FORMSETTING()
	{
		try
		{
			foreach (Form openForm in Application.OpenForms)
			{
				if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(string.Format("{0} - {1}", "Form Settings", Text)))
				{
					openForm.Activate();
					return;
				}
			}
			if ((_tableType.Equals("1") && grd_Data != null && grd_Data.DatasourceTable().Columns.Count > 0) || (_tableType.Equals("2") && _tree != null && _tree.DataSourceTable().Columns.Count > 0))
			{
				WinCommons._OpenProcessing();
				frm_ReportFormSettings frm = new frm_ReportFormSettings(_tableType, grd_Data, _tree, ItemId, ApzObjectTypes.oSubReportForm.ToString(), structureId, defaultStructure, defaultValid);
				frm.Text = string.Format("{0} - {1}", "Form Settings", Text);
				frm.MdiParent = MdiParent;
				frm.WindowState = FormWindowState.Normal;
				frm.StartPosition = FormStartPosition.CenterScreen;
				frm.FormClosed += async (object s, FormClosedEventArgs v) =>
				{
					if (frm.isSave)
					{
						isNotChangeSetting = true;
						defaultStructure = frm.dtStructure;
						structureId = frm.id;
						defaultValid = frm.dtValidValue;
						await GetListFormSetting();
						lue_FormSetting.EditValue = structureId;
						isNotChangeSetting = false;
					}
				};
				frm.FormClosed += (object s, FormClosedEventArgs e) =>
				{
					lstFormOpen.Remove(frm.Text);
				};
				frm.Show();
				lstFormOpen.Add(frm.Text);
				WinCommons._CloseProcessing();
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

	private void frm_SubReportForm_Activated(object sender, EventArgs e)
	{
		try
		{
			if (lstFormOpen.Count <= 0)
			{
				return;
			}
			foreach (Form openForm in Application.OpenForms)
			{
				if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(lstFormOpen[0]))
				{
					openForm.Activate();
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

	private void frm_SubReportForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (lstFormOpen.Count <= 0)
			{
				return;
			}
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(lstFormOpen[0]))
				{
					openForm.Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_SubReportForm));
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Execute = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_SetDefault = new System.Windows.Forms.CheckBox();
		this.lbl_FormSetting = new Apzon.Controls.Controls.LabelLine();
		this.lue_FormSetting = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_SaveFormSetting = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		base.SuspendLayout();
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Data.Location = new System.Drawing.Point(2, 2);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(773, 459);
		this.grd_Data.TabIndex = 1;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oSubReport";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsSelection.MultiSelect = true;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.btn_Execute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Execute.FieldName = null;
		this.btn_Execute.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Execute.ImageOptions.Image");
		this.btn_Execute.IsSearchControl = false;
		this.btn_Execute.Location = new System.Drawing.Point(2, 471);
		this.btn_Execute.Name = "btn_Execute";
		this.btn_Execute.Size = new System.Drawing.Size(75, 23);
		this.btn_Execute.TabIndex = 2;
		this.btn_Execute.Text = "Refresh";
		this.btn_Execute.Click += new System.EventHandler(this.bt_Execute_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 471);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Close";
		this.btn_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.chk_SetDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_SetDefault.AutoSize = true;
		this.chk_SetDefault.Location = new System.Drawing.Point(642, 474);
		this.chk_SetDefault.Name = "chk_SetDefault";
		this.chk_SetDefault.Size = new System.Drawing.Size(94, 17);
		this.chk_SetDefault.TabIndex = 38;
		this.chk_SetDefault.Text = "Set as Default";
		this.chk_SetDefault.UseVisualStyleBackColor = true;
		this.chk_SetDefault.CheckedChanged += new System.EventHandler(this.chk_SetDefault_CheckedChanged);
		this.lbl_FormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_FormSetting.FieldName = null;
		this.lbl_FormSetting.HideWhenEmpty = false;
		this.lbl_FormSetting.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FormSetting.Location = new System.Drawing.Point(358, 474);
		this.lbl_FormSetting.Name = "lbl_FormSetting";
		this.lbl_FormSetting.ShowUnderLine = true;
		this.lbl_FormSetting.Size = new System.Drawing.Size(77, 20);
		this.lbl_FormSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FormSetting.TabIndex = 36;
		this.lbl_FormSetting.Utext = "Form Setting";
		this.lue_FormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lue_FormSetting.Location = new System.Drawing.Point(431, 472);
		this.lue_FormSetting.Name = "lue_FormSetting";
		this.lue_FormSetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_FormSetting.Properties.NullText = "";
		this.lue_FormSetting.Size = new System.Drawing.Size(172, 20);
		this.lue_FormSetting.TabIndex = 37;
		this.lue_FormSetting.EditValueChanged += new System.EventHandler(this.lue_FormSetting_EditValueChanged);
		this.btn_SaveFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveFormSetting.FieldName = null;
		this.btn_SaveFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveFormSetting.ImageOptions.Image");
		this.btn_SaveFormSetting.IsSearchControl = false;
		this.btn_SaveFormSetting.Location = new System.Drawing.Point(609, 470);
		this.btn_SaveFormSetting.Name = "btn_SaveFormSetting";
		this.btn_SaveFormSetting.Size = new System.Drawing.Size(27, 23);
		this.btn_SaveFormSetting.TabIndex = 39;
		this.btn_SaveFormSetting.ToolTip = "Save Form Setting";
		this.btn_SaveFormSetting.Click += new System.EventHandler(this.btn_SaveFormSetting_Click);
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.grd_Data);
		this.panelControl1.Location = new System.Drawing.Point(2, 1);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(777, 463);
		this.panelControl1.TabIndex = 40;
		base.AcceptButton = this.btn_Execute;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(781, 496);
		base.Controls.Add(this.panelControl1);
		base.Controls.Add(this.btn_SaveFormSetting);
		base.Controls.Add(this.chk_SetDefault);
		base.Controls.Add(this.lue_FormSetting);
		base.Controls.Add(this.lbl_FormSetting);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Execute);
		base.FormId = "oSubReport";
		base.Name = "frm_SubReportForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.ShowMdiChildCaptionInParentTitle = true;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Sub Report Form";
		base.Activated += new System.EventHandler(this.frm_SubReportForm_Activated);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SubReportForm_FormClosing);
		base.Load += new System.EventHandler(this.frm_SubReportForm_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
