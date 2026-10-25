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
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Retail;

public class frm_PosAssignment : XtraFormEx
{
	private const string FormController = "PosMaster";

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _preAdd;

	private bool _isLoading;

	private int _defPOS;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem toolStrip_SetDefault;

	private GridControlEx grdUser;

	private GridViewEx gvUser;

	private GridColumnEx gcUSER_CODE;

	private GridColumnEx gcUSERID;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdPos;

	private GridViewEx gvPos;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_Stores;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx3;

	private ToolStripMenuItem toolStrip_RemoveDef;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_PosAssignment()
	{
		InitializeComponent();
		OnLoadAsync += frm_PosAssignment_Load;
		FormId = (gvPos.FormId = "oPosAssignment");
		gvPos.ItemId = "APZ_POS1";
	}

	private async Task LoadInitForm()
	{
		try
		{
			DataTable dataTable = await _httpClient.RetailStore.LoadAllDoc();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			rep_Stores.DataSource = dataTable.Copy();
			rep_Stores.DisplayMember = "Code";
			rep_Stores.ValueMember = "Code";
			rep_Stores.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Stores.DropDownRows = dataTable.Rows.Count;
			rep_Stores.ForceInitialize();
			rep_Stores.PopulateColumns();
			for (int i = 0; i < rep_Stores.Columns.Count; i++)
			{
				LookUpColumnInfo lookUpColumnInfo = rep_Stores.Columns[i];
				if (lookUpColumnInfo.FieldName.ToUpper() == "CODE")
				{
					lookUpColumnInfo.Visible = true;
				}
				else
				{
					lookUpColumnInfo.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			GridControlEx gridControlEx = grdUser;
			gridControlEx.DataSource = await _httpClient.User.LoadData();
			grdUser.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdPos.DataSource as DataTable;
			string currentUser = Function.ToString(gvUser.GetFocusedRowCellValue("USER_CODE"));
			if (!dataTable.IsNotNull() || string.IsNullOrEmpty(currentUser))
			{
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_POS1");
			tableStructure.Columns.Add("Checked", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = tableStructure.NewRow();
				DataRow dataRow2 = dataTable.Rows[i];
				int num = Function.ParseInt(dataRow2["PosID"]);
				dataRow["UserCode"] = currentUser;
				dataRow["PosID"] = num;
				dataRow["IsDflt"] = ((num == _defPOS) ? "Y" : "N");
				dataRow["Checked"] = Function.ToString(dataRow2["Checked"]);
				tableStructure.Rows.Add(dataRow);
			}
			string text = await _httpClient.PosMasterClient.SaveAssignment(tableStructure);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				int num2 = gvUser.LocateByValue("USER_CODE", currentUser);
				if (num2 != int.MinValue)
				{
					gvUser.FocusedRowHandle = num2;
					gvUser.MakeRowVisible(num2);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_PosAssignment_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
			gvPos.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					await SaveData();
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_RetailStore_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btn_Save_Click(null, null);
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

	private async void gvUser_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!_preAdd)
			{
				await LoadPos();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPos()
	{
		_isLoading = true;
		try
		{
			string text = Function.ToString(gvUser.GetFocusedRowCellValue("USER_CODE"));
			if (!string.IsNullOrEmpty(text))
			{
				DataTable dataTable = await _httpClient.PosMasterClient.GetUserPosAssignment(text);
				grdPos.DataSource = dataTable;
				grdPos.RefreshDataSource();
				gvPos.ExpandAllGroups();
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["IsDflt"]) == "Y");
				if (dataRow != null)
				{
					_defPOS = Function.ParseInt(dataRow["PosID"]);
				}
				else
				{
					_defPOS = 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void toolStrip_SetDefault_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gvPos.GetFocusedRowCellValue("Checked")) == "Y")
			{
				int defPOS = Function.ParseInt(gvPos.GetFocusedRowCellValue("PosID"));
				_defPOS = defPOS;
				gvPos.RefreshData();
				FormMode = Enums.FormViewModeType.Update;
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CheckBeforeSetDefault, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvPos_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int rowHandle = e.RowHandle;
			if (Function.ParseInt(Function.ParseInt(gvPos.GetRowCellValue(rowHandle, "PosID"))) == _defPOS && _defPOS != 0)
			{
				e.Appearance.FontStyleDelta = FontStyle.Bold;
			}
			else
			{
				e.Appearance.FontStyleDelta = FontStyle.Regular;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void repositoryItemCheckEdit1_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvPos.GetFocusedRowCellValue("PosID")) == _defPOS && _defPOS != 0 && Function.ToString(e.NewValue) == "N")
			{
				e.Cancel = true;
				WinCommons.Warning(Enums.MessageCode.CannotUncheckDefault, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeDefaultToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gvPos.GetFocusedRowCellValue("Checked")) == "Y")
			{
				_defPOS = 0;
				gvPos.RefreshData();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(Function.ParseInt(gvPos.GetFocusedRowCellValue("PosID"))) == _defPOS && _defPOS != 0)
			{
				toolStrip_RemoveDef.Visible = true;
				toolStrip_SetDefault.Visible = false;
			}
			else
			{
				toolStrip_RemoveDef.Visible = false;
				toolStrip_SetDefault.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvUser_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_preAdd && !_isLoading && FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.frm_PosAssignment));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStrip_SetDefault = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStrip_RemoveDef = new System.Windows.Forms.ToolStripMenuItem();
		this.grdUser = new Apzon.Controls.Grid.GridControlEx();
		this.gvUser = new Apzon.Controls.Grid.GridViewEx();
		this.gcUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUSERID = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdPos = new Apzon.Controls.Grid.GridControlEx();
		this.gvPos = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Stores = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdPos).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvPos).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stores).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 395);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(105, 395);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStrip_SetDefault, this.toolStrip_RemoveDef });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(159, 48);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.toolStrip_SetDefault.Name = "toolStrip_SetDefault";
		this.toolStrip_SetDefault.Size = new System.Drawing.Size(158, 22);
		this.toolStrip_SetDefault.Text = "Set as Default";
		this.toolStrip_SetDefault.Click += new System.EventHandler(this.toolStrip_SetDefault_Click);
		this.toolStrip_RemoveDef.Name = "toolStrip_RemoveDef";
		this.toolStrip_RemoveDef.Size = new System.Drawing.Size(158, 22);
		this.toolStrip_RemoveDef.Text = "Remove Default";
		this.toolStrip_RemoveDef.Visible = false;
		this.toolStrip_RemoveDef.Click += new System.EventHandler(this.removeDefaultToolStripMenuItem_Click);
		this.grdUser.AllowPasteExcel = false;
		this.grdUser.ClipboardData = null;
		this.grdUser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdUser.Location = new System.Drawing.Point(0, 0);
		this.grdUser.MainView = this.gvUser;
		this.grdUser.Name = "grdUser";
		this.grdUser.Size = new System.Drawing.Size(171, 388);
		this.grdUser.TabIndex = 77;
		this.grdUser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvUser });
		this.gvUser.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvUser.ColumnKey = null;
		this.gvUser.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcUSER_CODE, this.gcUSERID });
		this.gvUser.DisableFormSetting = false;
		this.gvUser.DisplayException = false;
		this.gvUser.EditColumn = null;
		this.gvUser.FieldCurrency = null;
		this.gvUser.FieldRate = null;
		this.gvUser.FormId = "oPosAssignment";
		this.gvUser.GridControl = this.grdUser;
		this.gvUser.IgnoreFilter = false;
		this.gvUser.IndexEditRow = -1;
		this.gvUser.IndicatorWidth = 35;
		this.gvUser.IsApzTable = false;
		this.gvUser.IsInsertNewRow = false;
		this.gvUser.IsLoadingData = false;
		this.gvUser.IsMappingContextAction = false;
		this.gvUser.ItemId = "OUSR";
		this.gvUser.LstColumnAllowBatch = null;
		this.gvUser.LstColumnNotAllowBatch = null;
		this.gvUser.LstColumnNotEditable = null;
		this.gvUser.LstColumNotSetting = null;
		this.gvUser.Name = "gvUser";
		this.gvUser.OptionsView.ShowGroupPanel = false;
		this.gvUser.UseBatchMod = false;
		this.gvUser.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvUser_FocusedRowObjectChanged);
		this.gvUser.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvUser_BeforeLeaveRow);
		this.gcUSER_CODE.Caption = "User Code";
		this.gcUSER_CODE.CurrencyDateField = null;
		this.gcUSER_CODE.CurrencyField = null;
		this.gcUSER_CODE.CurrencyFixed = null;
		this.gcUSER_CODE.CurrencyFixedRate = 0.0;
		this.gcUSER_CODE.CurrencyRateField = null;
		this.gcUSER_CODE.FieldName = "USER_CODE";
		this.gcUSER_CODE.Name = "gcUSER_CODE";
		this.gcUSER_CODE.OptionsColumn.ReadOnly = true;
		this.gcUSER_CODE.PrimaryKey = false;
		this.gcUSER_CODE.UnitField = null;
		this.gcUSER_CODE.Visible = true;
		this.gcUSER_CODE.VisibleIndex = 1;
		this.gcUSER_CODE.Width = 91;
		this.gcUSERID.Caption = "User ID";
		this.gcUSERID.CurrencyDateField = null;
		this.gcUSERID.CurrencyField = null;
		this.gcUSERID.CurrencyFixed = null;
		this.gcUSERID.CurrencyFixedRate = 0.0;
		this.gcUSERID.CurrencyRateField = null;
		this.gcUSERID.FieldName = "USERID";
		this.gcUSERID.Name = "gcUSERID";
		this.gcUSERID.OptionsColumn.ReadOnly = true;
		this.gcUSERID.PrimaryKey = false;
		this.gcUSERID.UnitField = null;
		this.gcUSERID.Visible = true;
		this.gcUSERID.VisibleIndex = 0;
		this.gcUSERID.Width = 54;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdUser);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grdPos);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(974, 388);
		this.splitContainerControl1.SplitterPosition = 171;
		this.splitContainerControl1.TabIndex = 78;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdPos.AllowPasteExcel = true;
		this.grdPos.ClipboardData = null;
		this.grdPos.ContextMenuStrip = this.contextMenuStrip1;
		this.grdPos.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdPos.Location = new System.Drawing.Point(0, 0);
		this.grdPos.MainView = this.gvPos;
		this.grdPos.Name = "grdPos";
		this.grdPos.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Stores, this.repositoryItemCheckEdit1 });
		this.grdPos.Size = new System.Drawing.Size(793, 388);
		this.grdPos.TabIndex = 75;
		this.grdPos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvPos });
		this.gvPos.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvPos.ColumnKey = null;
		this.gvPos.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn1, this.gridColumn2, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3 });
		this.gvPos.DisableFormSetting = false;
		this.gvPos.DisplayException = false;
		this.gvPos.EditColumn = null;
		this.gvPos.FieldCurrency = null;
		this.gvPos.FieldRate = null;
		this.gvPos.FormId = "oPosAssignment";
		this.gvPos.GridControl = this.grdPos;
		this.gvPos.GroupCount = 1;
		this.gvPos.IgnoreFilter = false;
		this.gvPos.IndexEditRow = -1;
		this.gvPos.IndicatorWidth = 40;
		this.gvPos.IsApzTable = false;
		this.gvPos.IsInsertNewRow = false;
		this.gvPos.IsLoadingData = false;
		this.gvPos.IsMappingContextAction = false;
		this.gvPos.ItemId = "OPOS";
		this.gvPos.LstColumnAllowBatch = null;
		this.gvPos.LstColumnNotAllowBatch = null;
		this.gvPos.LstColumnNotEditable = null;
		this.gvPos.LstColumNotSetting = null;
		this.gvPos.Name = "gvPos";
		this.gvPos.OptionsView.ShowGroupPanel = false;
		this.gvPos.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx1, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gvPos.UseBatchMod = false;
		this.gvPos.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvPos_RowCellStyle);
		this.gvPos.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "PosCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 69;
		this.gridColumn2.Caption = "Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Dscription";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 85;
		this.gridColumnEx1.Caption = "Store";
		this.gridColumnEx1.ColumnEdit = this.rep_Stores;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Store";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.rep_Stores.AutoHeight = false;
		this.rep_Stores.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Stores.Name = "rep_Stores";
		this.rep_Stores.NullText = "";
		this.gridColumnEx2.Caption = "Assigned";
		this.gridColumnEx2.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Checked";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 2;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.repositoryItemCheckEdit1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.repositoryItemCheckEdit1_EditValueChanging);
		this.gridColumnEx3.Caption = "gridColumnEx3";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "PosID";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(975, 420);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oPosAssignment";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PosAssignment";
		this.Text = "POS Master Data";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_RetailStore_FormClosing);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdPos).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvPos).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stores).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
