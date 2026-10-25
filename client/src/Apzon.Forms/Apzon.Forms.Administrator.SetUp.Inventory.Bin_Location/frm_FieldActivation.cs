using System;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_FieldActivation : XtraFormEx
{
	private const string FormController = "BinLocFieldActivation";

	private DataTable _dtOriginal;

	private IContainer components;

	private XtraTabControlEx tabBinAtt;

	private XtraTabPageEx tabSublevel;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grdWHSub;

	private GridColumnEx colSublevel;

	private GridColumnEx colName;

	private GridColumnEx colActive;

	private GridControlEx grdBinAtt;

	private GridViewEx gvBinLocAttr;

	private GridColumnEx colAtt;

	private GridColumnEx colAName;

	private GridColumnEx colAActive;

	private SimpleButtonEx btnOk;

	private SimpleButtonEx btnCancel;

	private RepositoryItemCheckEdit CbAtivated;

	private RepositoryItemCheckEdit CbAActivated;

	private GridViewEx gvWHSub;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_FieldActivation()
	{
		InitializeComponent();
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		return -1;
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocFieldActivation");
			DataTable dataTable = await helper.GetAsync("GetObfc");
			dataTable.Columns.Add("Title", typeof(string));
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ToString(row["FldType"]).Equals("S"))
				{
					row["Title"] = "Warehouse Sublevel " + row["FldNum"];
				}
				else
				{
					row["Title"] = "Bin Location Attribute " + row["FldNum"];
				}
			}
			dataTable.AcceptChanges();
			grdWHSub.DataSource = dataTable.Select("FldType = 'S'").CopyToDataTable();
			grdBinAtt.DataSource = dataTable.Select("FldType = 'A'").CopyToDataTable();
			grdWHSub.RefreshDataSource();
			grdBinAtt.RefreshDataSource();
			_dtOriginal = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> UpdateData()
	{
		try
		{
			DataTable dataSourceTable = grdWHSub.DataSourceTable;
			DataTable dataSourceTable2 = grdBinAtt.DataSourceTable;
			DataTable pr = dataSourceTable.AsEnumerable().Union(dataSourceTable2.AsEnumerable()).CopyToDataTable();
			for (int num = dataSourceTable.Rows.Count - 1; num >= 0; num--)
			{
				if (Function.ToString(dataSourceTable.Rows[num]["Activated"]) != Function.ToString(_dtOriginal.Rows[num]["Activated"]) && Function.ToString(dataSourceTable.Rows[num]["Activated"]) == "N")
				{
					pr = dataSourceTable.AsEnumerable().Reverse().Union(dataSourceTable2.AsEnumerable())
						.CopyToDataTable();
					break;
				}
			}
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "BinLocFieldActivation");
			return await hp.PostAsync("Save", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private async Task<DataTable> GetDataToUpdate()
	{
		try
		{
			DataTable dataTable = grdWHSub.DataSource as DataTable;
			DataTable dataTable2 = grdBinAtt.DataSource as DataTable;
			if (dataTable.IsNotNull() && dataTable2.IsNotNull())
			{
				DataTable dtSave = new DataTable
				{
					Columns = 
					{
						{
							"AbsEntry",
							typeof(int)
						},
						{
							"DispName",
							typeof(string)
						},
						{
							"Activated",
							typeof(string)
						},
						{
							"UserSign",
							typeof(int)
						},
						{
							"FldType",
							typeof(string)
						}
					}
				};
				DataTable dtNew = dataTable.AsEnumerable().Union(dataTable2.AsEnumerable()).CopyToDataTable();
				int num = await GetUserSignByUserCode_tableOUSR();
				for (int i = 0; i < dtNew.Rows.Count; i++)
				{
					if (Function.ToString(dtNew.Rows[i]["DispName"]) != Function.ToString(_dtOriginal.Rows[i]["DispName"]) || Function.ToString(dtNew.Rows[i]["Activated"]) != Function.ToString(_dtOriginal.Rows[i]["Activated"]))
					{
						DataRow dataRow = dtSave.NewRow();
						dataRow["AbsEntry"] = dtNew.Rows[i]["AbsEntry"];
						dataRow["DispName"] = dtNew.Rows[i]["DispName"];
						dataRow["Activated"] = dtNew.Rows[i]["Activated"];
						dataRow["UserSign"] = num;
						dataRow["FldType"] = dtNew.Rows[i]["FldType"];
						dtSave.Rows.Add(dataRow);
					}
				}
				dtSave.AcceptChanges();
				return dtSave;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	private async Task<string> CheckDeactive(DataTable data)
	{
		try
		{
			foreach (DataRow row in data.Rows)
			{
				if (!Function.ToString(row["Activated"]).Equals("N"))
				{
					continue;
				}
				using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "BinLocFieldActivation");
				string text = await hp.GetAsync("IsFieldExist", new
				{
					absEntry = Function.ToString(row["AbsEntry"]),
					fldType = Function.ToString(row["FldType"])
				});
				if (!string.IsNullOrEmpty(text))
				{
					if (text.Equals("Exist"))
					{
						return "You cannot continue with deactivation, associated warehouse sublevel or bin location attribute code exist.";
					}
					return "Cannot check data to save.";
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private void EnableDisableCOntrol(bool isEnable)
	{
		btnOk.Enabled = isEnable;
		GridViewEx gridViewEx = gvWHSub;
		bool readOnly = (gvBinLocAttr.ReadOnly = !isEnable);
		gridViewEx.ReadOnly = readOnly;
	}

	private async void frm_FieldActivation_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void gvBinLocAttr_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		EnableDisableCOntrol(isEnable: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				DataTable dt = grdWHSub.DataSource as DataTable;
				DataTable dt2 = grdBinAtt.DataSource as DataTable;
				if (!dt.IsNotNull() || !dt2.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
					EnableDisableCOntrol(isEnable: true);
					return;
				}
				WinCommons._OpenProcessing();
				string text = await UpdateData();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableCOntrol(isEnable: true);
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		Close();
	}

	private void gvWHSub_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (gvWHSub.FocusedRowHandle == 0 && gvWHSub.FocusedColumn == colActive)
		{
			e.Cancel = true;
		}
	}

	private void CbAActivated_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			CheckEdit checkEdit = sender as CheckEdit;
			int focusedRowHandle = gvWHSub.FocusedRowHandle;
			DataTable dataTable = grdWHSub.DataSource as DataTable;
			if (!dataTable.IsNotNull() || checkEdit == null)
			{
				return;
			}
			if (checkEdit.Checked)
			{
				for (int i = 1; i < focusedRowHandle; i++)
				{
					dataTable.Rows[i]["Activated"] = "Y";
				}
			}
			else
			{
				for (int j = focusedRowHandle + 1; j < dataTable.Rows.Count; j++)
				{
					dataTable.Rows[j]["Activated"] = "N";
				}
			}
			dataTable.AcceptChanges();
			grdWHSub.DataSource = dataTable;
			grdWHSub.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvBinLocAttr_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gvWHSub_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gvWHSub_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column == colActive && e.RowHandle == 0)
		{
			e.Appearance.BackColor = Color.LightGray;
		}
	}

	private async void frm_FieldActivation_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Update)
		{
			return;
		}
		switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
		{
		case DialogResult.Cancel:
			e.Cancel = true;
			break;
		case DialogResult.Yes:
		{
			e.Cancel = true;
			EnableDisableCOntrol(isEnable: false);
			WinCommons._OpenProcessing();
			string text = await UpdateData();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				EnableDisableCOntrol(isEnable: true);
			}
			break;
		}
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
		this.tabBinAtt = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabSublevel = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdWHSub = new Apzon.Controls.Grid.GridControlEx();
		this.gvWHSub = new Apzon.Controls.Grid.GridViewEx();
		this.colSublevel = new Apzon.Controls.Grid.GridColumnEx();
		this.colName = new Apzon.Controls.Grid.GridColumnEx();
		this.colActive = new Apzon.Controls.Grid.GridColumnEx();
		this.CbAActivated = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdBinAtt = new Apzon.Controls.Grid.GridControlEx();
		this.gvBinLocAttr = new Apzon.Controls.Grid.GridViewEx();
		this.colAtt = new Apzon.Controls.Grid.GridColumnEx();
		this.colAName = new Apzon.Controls.Grid.GridColumnEx();
		this.colAActive = new Apzon.Controls.Grid.GridColumnEx();
		this.CbAtivated = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.tabBinAtt).BeginInit();
		this.tabBinAtt.SuspendLayout();
		this.tabSublevel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdWHSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvWHSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CbAActivated).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdBinAtt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinLocAttr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CbAtivated).BeginInit();
		base.SuspendLayout();
		this.tabBinAtt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabBinAtt.IsSearchControl = false;
		this.tabBinAtt.Location = new System.Drawing.Point(0, 0);
		this.tabBinAtt.Name = "tabBinAtt";
		this.tabBinAtt.SelectedTabPage = this.tabSublevel;
		this.tabBinAtt.Size = new System.Drawing.Size(718, 304);
		this.tabBinAtt.TabIndex = 0;
		this.tabBinAtt.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabSublevel, this.xtraTabPage2 });
		this.tabSublevel.Controls.Add(this.grdWHSub);
		this.tabSublevel.EditControl = null;
		this.tabSublevel.EditValue = "Warehouse Sublevels";
		this.tabSublevel.FieldName = null;
		this.tabSublevel.IsSearchControl = false;
		this.tabSublevel.Name = "tabSublevel";
		this.tabSublevel.ReadOnly = false;
		this.tabSublevel.SettingInVisible = false;
		this.tabSublevel.SettingReadonly = false;
		this.tabSublevel.Size = new System.Drawing.Size(716, 279);
		this.tabSublevel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabSublevel.Text = "Warehouse Sublevels";
		this.grdWHSub.AllowPasteExcel = true;
		this.grdWHSub.ClipboardData = null;
		this.grdWHSub.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdWHSub.Location = new System.Drawing.Point(0, 0);
		this.grdWHSub.MainView = this.gvWHSub;
		this.grdWHSub.Name = "grdWHSub";
		this.grdWHSub.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.CbAActivated });
		this.grdWHSub.Size = new System.Drawing.Size(716, 279);
		this.grdWHSub.TabIndex = 0;
		this.grdWHSub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvWHSub });
		this.gvWHSub.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvWHSub.ColumnKey = null;
		this.gvWHSub.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.colSublevel, this.colName, this.colActive });
		this.gvWHSub.DisableFormSetting = false;
		this.gvWHSub.DisplayException = false;
		this.gvWHSub.EditColumn = null;
		this.gvWHSub.FieldCurrency = null;
		this.gvWHSub.FieldRate = null;
		this.gvWHSub.FormId = null;
		this.gvWHSub.GridControl = this.grdWHSub;
		this.gvWHSub.IgnoreFilter = false;
		this.gvWHSub.IndexEditRow = -1;
		this.gvWHSub.IndicatorWidth = 40;
		this.gvWHSub.IsApzTable = false;
		this.gvWHSub.IsInsertNewRow = false;
		this.gvWHSub.IsLoadingData = false;
		this.gvWHSub.IsMappingContextAction = false;
		this.gvWHSub.ItemId = null;
		this.gvWHSub.LstColumnAllowBatch = null;
		this.gvWHSub.LstColumnNotAllowBatch = null;
		this.gvWHSub.LstColumnNotEditable = null;
		this.gvWHSub.LstColumNotSetting = null;
		this.gvWHSub.Name = "gvWHSub";
		this.gvWHSub.OptionsCustomization.AllowSort = false;
		this.gvWHSub.OptionsSelection.MultiSelect = true;
		this.gvWHSub.OptionsView.ShowGroupPanel = false;
		this.gvWHSub.UseBatchMod = false;
		this.gvWHSub.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvWHSub_RowCellStyle);
		this.gvWHSub.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvWHSub_ShowingEditor);
		this.gvWHSub.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvWHSub_CellValueChanging);
		this.colSublevel.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colSublevel.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colSublevel.AppearanceCell.Options.UseBackColor = true;
		this.colSublevel.AppearanceCell.Options.UseTextOptions = true;
		this.colSublevel.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colSublevel.AppearanceHeader.BackColor = System.Drawing.Color.LightGray;
		this.colSublevel.AppearanceHeader.BackColor2 = System.Drawing.Color.LightGray;
		this.colSublevel.AppearanceHeader.Options.UseBackColor = true;
		this.colSublevel.Caption = "Sublevel";
		this.colSublevel.CurrencyDateField = null;
		this.colSublevel.CurrencyField = null;
		this.colSublevel.CurrencyFixed = null;
		this.colSublevel.CurrencyFixedRate = 0.0;
		this.colSublevel.CurrencyRateField = null;
		this.colSublevel.FieldName = "Title";
		this.colSublevel.Name = "colSublevel";
		this.colSublevel.OptionsColumn.AllowEdit = false;
		this.colSublevel.OptionsColumn.AllowFocus = false;
		this.colSublevel.PrimaryKey = true;
		this.colSublevel.UnitField = null;
		this.colSublevel.Visible = true;
		this.colSublevel.VisibleIndex = 0;
		this.colSublevel.Width = 154;
		this.colName.Caption = "Name";
		this.colName.CurrencyDateField = null;
		this.colName.CurrencyField = null;
		this.colName.CurrencyFixed = null;
		this.colName.CurrencyFixedRate = 0.0;
		this.colName.CurrencyRateField = null;
		this.colName.FieldName = "DispName";
		this.colName.Name = "colName";
		this.colName.PrimaryKey = false;
		this.colName.UnitField = null;
		this.colName.Visible = true;
		this.colName.VisibleIndex = 1;
		this.colName.Width = 198;
		this.colActive.Caption = "Active";
		this.colActive.ColumnEdit = this.CbAActivated;
		this.colActive.CurrencyDateField = null;
		this.colActive.CurrencyField = null;
		this.colActive.CurrencyFixed = null;
		this.colActive.CurrencyFixedRate = 0.0;
		this.colActive.CurrencyRateField = null;
		this.colActive.FieldName = "Activated";
		this.colActive.Name = "colActive";
		this.colActive.OptionsColumn.FixedWidth = true;
		this.colActive.PrimaryKey = false;
		this.colActive.UnitField = null;
		this.colActive.Visible = true;
		this.colActive.VisibleIndex = 2;
		this.colActive.Width = 68;
		this.CbAActivated.AutoHeight = false;
		this.CbAActivated.Name = "CbAActivated";
		this.CbAActivated.ValueChecked = "Y";
		this.CbAActivated.ValueUnchecked = "N";
		this.CbAActivated.EditValueChanged += new System.EventHandler(this.CbAActivated_EditValueChanged);
		this.xtraTabPage2.Controls.Add(this.grdBinAtt);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Bin Locations Attributes";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(716, 279);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Bin Locations Attributes";
		this.grdBinAtt.AllowPasteExcel = false;
		this.grdBinAtt.ClipboardData = null;
		this.grdBinAtt.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdBinAtt.Location = new System.Drawing.Point(0, 0);
		this.grdBinAtt.MainView = this.gvBinLocAttr;
		this.grdBinAtt.Name = "grdBinAtt";
		this.grdBinAtt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.CbAtivated });
		this.grdBinAtt.Size = new System.Drawing.Size(716, 279);
		this.grdBinAtt.TabIndex = 1;
		this.grdBinAtt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBinLocAttr });
		this.gvBinLocAttr.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBinLocAttr.ColumnKey = null;
		this.gvBinLocAttr.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.colAtt, this.colAName, this.colAActive });
		this.gvBinLocAttr.DisableFormSetting = false;
		this.gvBinLocAttr.DisplayException = false;
		this.gvBinLocAttr.EditColumn = null;
		this.gvBinLocAttr.FieldCurrency = null;
		this.gvBinLocAttr.FieldRate = null;
		this.gvBinLocAttr.FormId = null;
		this.gvBinLocAttr.GridControl = this.grdBinAtt;
		this.gvBinLocAttr.IgnoreFilter = false;
		this.gvBinLocAttr.IndexEditRow = -1;
		this.gvBinLocAttr.IndicatorWidth = 40;
		this.gvBinLocAttr.IsApzTable = false;
		this.gvBinLocAttr.IsInsertNewRow = false;
		this.gvBinLocAttr.IsLoadingData = false;
		this.gvBinLocAttr.IsMappingContextAction = false;
		this.gvBinLocAttr.ItemId = null;
		this.gvBinLocAttr.LstColumnAllowBatch = null;
		this.gvBinLocAttr.LstColumnNotAllowBatch = null;
		this.gvBinLocAttr.LstColumnNotEditable = null;
		this.gvBinLocAttr.LstColumNotSetting = null;
		this.gvBinLocAttr.Name = "gvBinLocAttr";
		this.gvBinLocAttr.OptionsCustomization.AllowSort = false;
		this.gvBinLocAttr.OptionsView.ShowGroupPanel = false;
		this.gvBinLocAttr.UseBatchMod = false;
		this.gvBinLocAttr.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvBinLocAttr_CustomDrawRowIndicator);
		this.gvBinLocAttr.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBinLocAttr_CellValueChanging);
		this.colAtt.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colAtt.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colAtt.AppearanceCell.Options.UseBackColor = true;
		this.colAtt.AppearanceCell.Options.UseTextOptions = true;
		this.colAtt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colAtt.AppearanceHeader.BackColor = System.Drawing.Color.LightGray;
		this.colAtt.AppearanceHeader.BackColor2 = System.Drawing.Color.LightGray;
		this.colAtt.AppearanceHeader.Options.UseBackColor = true;
		this.colAtt.Caption = "Attribute";
		this.colAtt.CurrencyDateField = null;
		this.colAtt.CurrencyField = null;
		this.colAtt.CurrencyFixed = null;
		this.colAtt.CurrencyFixedRate = 0.0;
		this.colAtt.CurrencyRateField = null;
		this.colAtt.FieldName = "Title";
		this.colAtt.Name = "colAtt";
		this.colAtt.OptionsColumn.AllowEdit = false;
		this.colAtt.OptionsColumn.AllowFocus = false;
		this.colAtt.PrimaryKey = false;
		this.colAtt.UnitField = null;
		this.colAtt.Visible = true;
		this.colAtt.VisibleIndex = 0;
		this.colAtt.Width = 139;
		this.colAName.Caption = "Name";
		this.colAName.CurrencyDateField = null;
		this.colAName.CurrencyField = null;
		this.colAName.CurrencyFixed = null;
		this.colAName.CurrencyFixedRate = 0.0;
		this.colAName.CurrencyRateField = null;
		this.colAName.FieldName = "DispName";
		this.colAName.Name = "colAName";
		this.colAName.PrimaryKey = false;
		this.colAName.UnitField = null;
		this.colAName.Visible = true;
		this.colAName.VisibleIndex = 1;
		this.colAName.Width = 162;
		this.colAActive.Caption = "Active";
		this.colAActive.ColumnEdit = this.CbAtivated;
		this.colAActive.CurrencyDateField = null;
		this.colAActive.CurrencyField = null;
		this.colAActive.CurrencyFixed = null;
		this.colAActive.CurrencyFixedRate = 0.0;
		this.colAActive.CurrencyRateField = null;
		this.colAActive.FieldName = "Activated";
		this.colAActive.Name = "colAActive";
		this.colAActive.OptionsColumn.FixedWidth = true;
		this.colAActive.PrimaryKey = false;
		this.colAActive.UnitField = null;
		this.colAActive.Visible = true;
		this.colAActive.VisibleIndex = 2;
		this.colAActive.Width = 68;
		this.CbAtivated.AutoHeight = false;
		this.CbAtivated.Name = "CbAtivated";
		this.CbAtivated.ValueChecked = "Y";
		this.CbAtivated.ValueUnchecked = "N";
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 310);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 1;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 310);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(718, 343);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.tabBinAtt);
		base.FormId = "oBinLocFieldActivation";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FieldActivation";
		this.Text = "Bin Location Field Activation";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FieldActivation_FormClosing);
		base.Load += new System.EventHandler(this.frm_FieldActivation_Load);
		((System.ComponentModel.ISupportInitialize)this.tabBinAtt).EndInit();
		this.tabBinAtt.ResumeLayout(false);
		this.tabSublevel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdWHSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvWHSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CbAActivated).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdBinAtt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinLocAttr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CbAtivated).EndInit();
		base.ResumeLayout(false);
	}
}
