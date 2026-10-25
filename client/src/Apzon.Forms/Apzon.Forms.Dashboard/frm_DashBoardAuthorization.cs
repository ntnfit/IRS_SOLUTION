using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Dashboard;

public class frm_DashBoardAuthorization : XtraFormEx
{
	private string _user = "";

	private DataTable _dtAuth;

	private string _formController = "DashBoardAuth";

	private IContainer components;

	private GridControlEx grdUlist;

	private GridColumnEx gcU_NAME;

	private GridColumnEx gcUSER_CODE;

	private PanelControl panelControl1;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btUpdate;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx bt_FullAuth;

	private SimpleButtonEx bt_NoAuth;

	private SimpleButtonEx bt_ReadOnly;

	private SimpleButtonEx bt_CopyAuth;

	private GridViewEx gvUList;

	private GridControlEx grd_DashboardAuth;

	private GridViewEx gvDashboardAuth;

	private GridColumnEx colTabId;

	private GridColumnEx colTabName;

	private GridColumnEx colCate;

	private GridColumnEx colAuth;

	private RepositoryItemLookUpEdit rep_tl_authoriza;

	private GridColumnEx colUSER_CODE;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem expandAllToolStripMenuItem;

	private ToolStripMenuItem collapseAllToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btUpdate.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btUpdate.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_DashBoardAuthorization()
	{
		if (GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser))
		{
			InitializeComponent();
			return;
		}
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DashBoardAuthorization"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DashBoardAuthorization" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_DashBoardAuthorization_Load(object sender, EventArgs e)
	{
		LoadRepLkUpEditDatasource();
		await LoadData();
		SettingLanguage();
	}

	private void LoadRepLkUpEditDatasource()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(char));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = 'F';
			dataRow[1] = "Full Authorization";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = 'N';
			dataRow2[1] = "No Authorization";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3[0] = 'R';
			dataRow3[1] = "Read-Only";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			rep_tl_authoriza.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		FormMode = Enums.FormViewModeType.View;
		btCancel.Text = Resources.ButtonCancel;
		bt_CopyAuth.Text = Resources.OPRM_CopyAuth;
		bt_FullAuth.Text = Resources.OPRM_FullAuth;
		bt_NoAuth.Text = Resources.OPRM_NoAuth;
		bt_ReadOnly.Text = Resources.OPRM_ReadOnly;
		gvUList.Columns["U_NAME"].Caption = Resources.OUSR_U_NAME;
		gvUList.Columns["USER_CODE"].Caption = Resources.OUSR_USER_CODE;
	}

	private async Task LoadData()
	{
		await LoadUsers();
		await LoadAuthorities(_user);
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadUsers()
	{
		try
		{
			using HttpClientHelper<List<UsersImpl>> hp = new HttpClientHelper<List<UsersImpl>>(Configuration.ApiServer, "Users");
			List<UsersImpl> dataSource = await hp.GetAsync("ShowUsers");
			grdUlist.DataSource = dataSource;
			grdUlist.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadAuthorities(string user)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			_dtAuth = await hp.GetAsync("GetAllAuth", new { user });
			grd_DashboardAuth.DataSource = _dtAuth;
			grd_DashboardAuth.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDashboardAuth_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDashboardAuth_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDashboardAuth_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_CopyAuth_Click(object sender, EventArgs e)
	{
		try
		{
			frm_DBAuthorizationCopy frm_DBAuthorizationCopy2 = new frm_DBAuthorizationCopy();
			frm_DBAuthorizationCopy2._user = _user;
			frm_DBAuthorizationCopy2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_FullAuth_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "F";
			}
			grd_DashboardAuth.DataSource = _dtAuth;
			grd_DashboardAuth.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_NoAuth_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "N";
			}
			grd_DashboardAuth.DataSource = _dtAuth;
			grd_DashboardAuth.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_ReadOnly_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "R";
			}
			grd_DashboardAuth.DataSource = _dtAuth;
			grd_DashboardAuth.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Auth_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvAuth_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void chkFull_CheckedChanged(object sender, EventArgs e)
	{
	}

	private async void gvUList_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_user = ((gvUList.GetFocusedRowCellValue("USER_CODE") == DBNull.Value || gvUList.GetFocusedRowCellValue("USER_CODE") == null) ? "" : gvUList.GetFocusedRowCellValue("USER_CODE").ToString());
			if (_user != "")
			{
				await LoadAuthorities(_user);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btUpdate_Click(object sender, EventArgs e)
	{
		btUpdate.Enabled = false;
		bt_CopyAuth.Enabled = false;
		bt_FullAuth.Enabled = false;
		bt_NoAuth.Enabled = false;
		bt_ReadOnly.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				await SaveData();
				WinCommons._CloseProcessing();
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btUpdate.Enabled = true;
		bt_CopyAuth.Enabled = true;
		bt_FullAuth.Enabled = true;
		bt_NoAuth.Enabled = true;
		bt_ReadOnly.Enabled = true;
	}

	private async Task SaveData()
	{
		try
		{
			using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _formController);
			DataTable pr = (DataTable)grd_DashboardAuth.DataSource;
			HttpResult httpResult = hp.Post("SaveDoc", pr);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_user = ((gvUList.GetFocusedRowCellValue("USER_CODE") == DBNull.Value || gvUList.GetFocusedRowCellValue("USER_CODE") == null) ? "" : gvUList.GetFocusedRowCellValue("USER_CODE").ToString());
				if (_user != "")
				{
					await LoadAuthorities(_user);
				}
				FormMode = Enums.FormViewModeType.View;
				gvDashboardAuth.ExpandAllGroups();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		gvDashboardAuth.ExpandAllGroups();
	}

	private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		gvDashboardAuth.CollapseAllGroups();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DashBoardAuthorization));
		this.grdUlist = new Apzon.Controls.Grid.GridControlEx();
		this.gvUList = new Apzon.Controls.Grid.GridViewEx();
		this.gcU_NAME = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		this.btUpdate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_FullAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_NoAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_ReadOnly = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_CopyAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_DashboardAuth = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDashboardAuth = new Apzon.Controls.Grid.GridViewEx();
		this.colTabId = new Apzon.Controls.Grid.GridColumnEx();
		this.colTabName = new Apzon.Controls.Grid.GridColumnEx();
		this.colCate = new Apzon.Controls.Grid.GridColumnEx();
		this.colAuth = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_tl_authoriza = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grdUlist).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvUList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DashboardAuth).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDashboardAuth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_tl_authoriza).BeginInit();
		base.SuspendLayout();
		this.grdUlist.AllowPasteExcel = true;
		this.grdUlist.ClipboardData = null;
		this.grdUlist.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdUlist.Location = new System.Drawing.Point(0, 0);
		this.grdUlist.MainView = this.gvUList;
		this.grdUlist.Name = "grdUlist";
		this.grdUlist.Size = new System.Drawing.Size(322, 475);
		this.grdUlist.TabIndex = 0;
		this.grdUlist.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvUList });
		this.gvUList.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvUList.ColumnKey = null;
		this.gvUList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcU_NAME, this.gcUSER_CODE });
		this.gvUList.DisableFormSetting = false;
		this.gvUList.DisplayException = false;
		this.gvUList.EditColumn = null;
		this.gvUList.FieldCurrency = null;
		this.gvUList.FieldRate = null;
		this.gvUList.FormId = "oDashboardTabAuthorization";
		this.gvUList.GridControl = this.grdUlist;
		this.gvUList.IgnoreFilter = false;
		this.gvUList.IndexEditRow = -1;
		this.gvUList.IndicatorWidth = 35;
		this.gvUList.IsApzTable = false;
		this.gvUList.IsInsertNewRow = false;
		this.gvUList.IsLoadingData = false;
		this.gvUList.IsMappingContextAction = false;
		this.gvUList.ItemId = "OUSR";
		this.gvUList.LstColumnAllowBatch = null;
		this.gvUList.LstColumnNotAllowBatch = null;
		this.gvUList.LstColumnNotEditable = null;
		this.gvUList.LstColumNotSetting = null;
		this.gvUList.Name = "gvUList";
		this.gvUList.OptionsView.ShowGroupPanel = false;
		this.gvUList.UseBatchMod = false;
		this.gvUList.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvUList_FocusedRowObjectChanged);
		this.gcU_NAME.Caption = "Full name";
		this.gcU_NAME.CurrencyDateField = null;
		this.gcU_NAME.CurrencyField = null;
		this.gcU_NAME.CurrencyFixed = null;
		this.gcU_NAME.CurrencyFixedRate = 0.0;
		this.gcU_NAME.CurrencyRateField = null;
		this.gcU_NAME.FieldName = "U_NAME";
		this.gcU_NAME.Name = "gcU_NAME";
		this.gcU_NAME.OptionsColumn.AllowEdit = false;
		this.gcU_NAME.OptionsColumn.ReadOnly = true;
		this.gcU_NAME.PrimaryKey = false;
		this.gcU_NAME.UnitField = null;
		this.gcU_NAME.Visible = true;
		this.gcU_NAME.VisibleIndex = 0;
		this.gcUSER_CODE.Caption = "User name";
		this.gcUSER_CODE.CurrencyDateField = null;
		this.gcUSER_CODE.CurrencyField = null;
		this.gcUSER_CODE.CurrencyFixed = null;
		this.gcUSER_CODE.CurrencyFixedRate = 0.0;
		this.gcUSER_CODE.CurrencyRateField = null;
		this.gcUSER_CODE.FieldName = "USER_CODE";
		this.gcUSER_CODE.Name = "gcUSER_CODE";
		this.gcUSER_CODE.OptionsColumn.AllowEdit = false;
		this.gcUSER_CODE.OptionsColumn.ReadOnly = true;
		this.gcUSER_CODE.PrimaryKey = false;
		this.gcUSER_CODE.UnitField = null;
		this.gcUSER_CODE.Visible = true;
		this.gcUSER_CODE.VisibleIndex = 1;
		this.btUpdate.EditControl = null;
		this.btUpdate.EditValue = "OK";
		this.btUpdate.FieldName = null;
		this.btUpdate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btUpdate.ImageOptions.Image");
		this.btUpdate.IsSearchControl = false;
		this.btUpdate.Location = new System.Drawing.Point(327, 8);
		this.btUpdate.Name = "btUpdate";
		this.btUpdate.ReadOnly = false;
		this.btUpdate.SettingInVisible = false;
		this.btUpdate.SettingReadonly = false;
		this.btUpdate.Size = new System.Drawing.Size(75, 23);
		this.btUpdate.TabIndex = 2;
		this.btUpdate.Text = "OK";
		this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(408, 8);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 3;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.bt_FullAuth.EditControl = null;
		this.bt_FullAuth.EditValue = "Full Authorization";
		this.bt_FullAuth.FieldName = null;
		this.bt_FullAuth.IsSearchControl = false;
		this.bt_FullAuth.Location = new System.Drawing.Point(517, 8);
		this.bt_FullAuth.Name = "bt_FullAuth";
		this.bt_FullAuth.ReadOnly = false;
		this.bt_FullAuth.SettingInVisible = false;
		this.bt_FullAuth.SettingReadonly = false;
		this.bt_FullAuth.Size = new System.Drawing.Size(92, 23);
		this.bt_FullAuth.TabIndex = 5;
		this.bt_FullAuth.Text = "Full Authorization";
		this.bt_FullAuth.Click += new System.EventHandler(this.bt_FullAuth_Click);
		this.bt_NoAuth.EditControl = null;
		this.bt_NoAuth.EditValue = "No Authorization";
		this.bt_NoAuth.FieldName = null;
		this.bt_NoAuth.IsSearchControl = false;
		this.bt_NoAuth.Location = new System.Drawing.Point(615, 8);
		this.bt_NoAuth.Name = "bt_NoAuth";
		this.bt_NoAuth.ReadOnly = false;
		this.bt_NoAuth.SettingInVisible = false;
		this.bt_NoAuth.SettingReadonly = false;
		this.bt_NoAuth.Size = new System.Drawing.Size(90, 23);
		this.bt_NoAuth.TabIndex = 6;
		this.bt_NoAuth.Text = "No Authorization";
		this.bt_NoAuth.Click += new System.EventHandler(this.bt_NoAuth_Click);
		this.bt_ReadOnly.EditControl = null;
		this.bt_ReadOnly.EditValue = "Read-Only";
		this.bt_ReadOnly.FieldName = null;
		this.bt_ReadOnly.IsSearchControl = false;
		this.bt_ReadOnly.Location = new System.Drawing.Point(711, 8);
		this.bt_ReadOnly.Name = "bt_ReadOnly";
		this.bt_ReadOnly.ReadOnly = false;
		this.bt_ReadOnly.SettingInVisible = false;
		this.bt_ReadOnly.SettingReadonly = false;
		this.bt_ReadOnly.Size = new System.Drawing.Size(75, 23);
		this.bt_ReadOnly.TabIndex = 7;
		this.bt_ReadOnly.Text = "Read-Only";
		this.bt_ReadOnly.Click += new System.EventHandler(this.bt_ReadOnly_Click);
		this.bt_CopyAuth.EditControl = null;
		this.bt_CopyAuth.EditValue = "Copy Author&izations";
		this.bt_CopyAuth.FieldName = null;
		this.bt_CopyAuth.IsSearchControl = false;
		this.bt_CopyAuth.Location = new System.Drawing.Point(68, 8);
		this.bt_CopyAuth.Name = "bt_CopyAuth";
		this.bt_CopyAuth.ReadOnly = false;
		this.bt_CopyAuth.SettingInVisible = false;
		this.bt_CopyAuth.SettingReadonly = false;
		this.bt_CopyAuth.Size = new System.Drawing.Size(115, 23);
		this.bt_CopyAuth.TabIndex = 8;
		this.bt_CopyAuth.Text = "Copy Author&izations";
		this.bt_CopyAuth.Click += new System.EventHandler(this.bt_CopyAuth_Click);
		this.panelControl1.Controls.Add(this.btUpdate);
		this.panelControl1.Controls.Add(this.bt_CopyAuth);
		this.panelControl1.Controls.Add(this.btCancel);
		this.panelControl1.Controls.Add(this.bt_ReadOnly);
		this.panelControl1.Controls.Add(this.bt_FullAuth);
		this.panelControl1.Controls.Add(this.bt_NoAuth);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 475);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(794, 37);
		this.panelControl1.TabIndex = 9;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdUlist);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_DashboardAuth);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(794, 475);
		this.splitContainerControl1.SplitterPosition = 322;
		this.splitContainerControl1.TabIndex = 10;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_DashboardAuth.AllowPasteExcel = true;
		this.grd_DashboardAuth.ClipboardData = null;
		this.grd_DashboardAuth.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_DashboardAuth.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DashboardAuth.Location = new System.Drawing.Point(0, 0);
		this.grd_DashboardAuth.MainView = this.gvDashboardAuth;
		this.grd_DashboardAuth.Name = "grd_DashboardAuth";
		this.grd_DashboardAuth.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_tl_authoriza });
		this.grd_DashboardAuth.Size = new System.Drawing.Size(462, 475);
		this.grd_DashboardAuth.TabIndex = 0;
		this.grd_DashboardAuth.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDashboardAuth });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.expandAllToolStripMenuItem, this.collapseAllToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
		this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
		this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
		this.expandAllToolStripMenuItem.Text = "Expand All";
		this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
		this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
		this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
		this.collapseAllToolStripMenuItem.Text = "Collapse All";
		this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
		this.gvDashboardAuth.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDashboardAuth.ColumnKey = null;
		this.gvDashboardAuth.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.colTabId, this.colTabName, this.colCate, this.colAuth, this.colUSER_CODE });
		this.gvDashboardAuth.DisableFormSetting = false;
		this.gvDashboardAuth.DisplayException = false;
		this.gvDashboardAuth.EditColumn = null;
		this.gvDashboardAuth.FieldCurrency = null;
		this.gvDashboardAuth.FieldRate = null;
		this.gvDashboardAuth.FormId = "oDashboardTabAuthorization";
		this.gvDashboardAuth.GridControl = this.grd_DashboardAuth;
		this.gvDashboardAuth.GroupCount = 1;
		this.gvDashboardAuth.IgnoreFilter = false;
		this.gvDashboardAuth.IndexEditRow = -1;
		this.gvDashboardAuth.IndicatorWidth = 40;
		this.gvDashboardAuth.IsApzTable = false;
		this.gvDashboardAuth.IsInsertNewRow = false;
		this.gvDashboardAuth.IsLoadingData = false;
		this.gvDashboardAuth.IsMappingContextAction = false;
		this.gvDashboardAuth.ItemId = "OAUT";
		this.gvDashboardAuth.LstColumnAllowBatch = null;
		this.gvDashboardAuth.LstColumnNotAllowBatch = null;
		this.gvDashboardAuth.LstColumnNotEditable = null;
		this.gvDashboardAuth.LstColumNotSetting = null;
		this.gvDashboardAuth.Name = "gvDashboardAuth";
		this.gvDashboardAuth.OptionsView.ShowGroupPanel = false;
		this.gvDashboardAuth.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colCate, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gvDashboardAuth.UseBatchMod = false;
		this.gvDashboardAuth.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDashboardAuth_CustomDrawRowIndicator);
		this.gvDashboardAuth.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDashboardAuth_CellValueChanged);
		this.gvDashboardAuth.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDashboardAuth_CellValueChanging);
		this.colTabId.Caption = "Tab ID";
		this.colTabId.CurrencyDateField = null;
		this.colTabId.CurrencyField = null;
		this.colTabId.CurrencyFixed = null;
		this.colTabId.CurrencyFixedRate = 0.0;
		this.colTabId.CurrencyRateField = null;
		this.colTabId.FieldName = "TabId";
		this.colTabId.Name = "colTabId";
		this.colTabId.OptionsColumn.AllowEdit = false;
		this.colTabId.OptionsColumn.ReadOnly = true;
		this.colTabId.PrimaryKey = false;
		this.colTabId.UnitField = null;
		this.colTabName.Caption = "Tab Name";
		this.colTabName.CurrencyDateField = null;
		this.colTabName.CurrencyField = null;
		this.colTabName.CurrencyFixed = null;
		this.colTabName.CurrencyFixedRate = 0.0;
		this.colTabName.CurrencyRateField = null;
		this.colTabName.FieldName = "TabName";
		this.colTabName.Name = "colTabName";
		this.colTabName.OptionsColumn.AllowEdit = false;
		this.colTabName.OptionsColumn.ReadOnly = true;
		this.colTabName.PrimaryKey = false;
		this.colTabName.UnitField = null;
		this.colTabName.Visible = true;
		this.colTabName.VisibleIndex = 0;
		this.colCate.Caption = "Categories";
		this.colCate.CurrencyDateField = null;
		this.colCate.CurrencyField = null;
		this.colCate.CurrencyFixed = null;
		this.colCate.CurrencyFixedRate = 0.0;
		this.colCate.CurrencyRateField = null;
		this.colCate.FieldName = "CatName";
		this.colCate.Name = "colCate";
		this.colCate.OptionsColumn.AllowEdit = false;
		this.colCate.OptionsColumn.ReadOnly = true;
		this.colCate.PrimaryKey = false;
		this.colCate.UnitField = null;
		this.colCate.Visible = true;
		this.colCate.VisibleIndex = 1;
		this.colAuth.Caption = "Authorization";
		this.colAuth.ColumnEdit = this.rep_tl_authoriza;
		this.colAuth.CurrencyDateField = null;
		this.colAuth.CurrencyField = null;
		this.colAuth.CurrencyFixed = null;
		this.colAuth.CurrencyFixedRate = 0.0;
		this.colAuth.CurrencyRateField = null;
		this.colAuth.FieldName = "authoriza";
		this.colAuth.Name = "colAuth";
		this.colAuth.PrimaryKey = false;
		this.colAuth.UnitField = null;
		this.colAuth.Visible = true;
		this.colAuth.VisibleIndex = 1;
		this.rep_tl_authoriza.AutoHeight = false;
		this.rep_tl_authoriza.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_tl_authoriza.DisplayMember = "Name";
		this.rep_tl_authoriza.Name = "rep_tl_authoriza";
		this.rep_tl_authoriza.NullText = "";
		this.rep_tl_authoriza.ValueMember = "Code";
		this.colUSER_CODE.Caption = "USER_CODE";
		this.colUSER_CODE.CurrencyDateField = null;
		this.colUSER_CODE.CurrencyField = null;
		this.colUSER_CODE.CurrencyFixed = null;
		this.colUSER_CODE.CurrencyFixedRate = 0.0;
		this.colUSER_CODE.CurrencyRateField = null;
		this.colUSER_CODE.FieldName = "USER_CODE";
		this.colUSER_CODE.Name = "colUSER_CODE";
		this.colUSER_CODE.OptionsColumn.AllowEdit = false;
		this.colUSER_CODE.OptionsColumn.ReadOnly = true;
		this.colUSER_CODE.PrimaryKey = false;
		this.colUSER_CODE.UnitField = null;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(794, 512);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oDashboardTabAuthorization";
		base.IconOptions.ShowIcon = false;
		base.MinimizeBox = false;
		base.Name = "frm_DashBoardAuthorization";
		base.ShowInTaskbar = false;
		this.Text = "DashboardTabs Authorization";
		base.Load += new System.EventHandler(this.frm_DashBoardAuthorization_Load);
		((System.ComponentModel.ISupportInitialize)this.grdUlist).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvUList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DashboardAuth).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDashboardAuth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_tl_authoriza).EndInit();
		base.ResumeLayout(false);
	}
}
