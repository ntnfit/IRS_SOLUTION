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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_Vehicle : XtraFormEx
{
	private DataTable _dtdoc;

	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx Code;

	private GridColumnEx gd_name;

	private GridColumnEx VehicleNo;

	private GridColumnEx Type;

	private RepositoryItemLookUpEdit rep_lookType;

	private GridColumnEx gd_Load;

	private RepositoryItemCalcEdit rep_caLoad;

	private GridColumnEx Driver;

	private GridColumnEx Phone;

	private GridColumnEx Remarks;

	private GridColumnEx BPLId;

	private RepositoryItemLookUpEdit rep_lookBra;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem menu_AddNew;

	private ToolStripMenuItem menu_Delete;

	private ToolStripMenuItem menu_Duplicate;

	public string DocCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Vehicle()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Vehicle"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Vehicle" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				contextMenuStrip1.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_Vehicle_Load(object sender, EventArgs e)
	{
		try
		{
			if (WinFuntions.GetTableStructure("APZ_VHCE").Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				WinFuntions.DisableAllControl(Controls);
				contextMenuStrip1.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			else
			{
				LoadLookupBranch();
				LoadLookupType();
				await GetList();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				await Saves();
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
		Close();
	}

	public async Task REFRESH()
	{
		try
		{
			_dtdoc = grd_Doc.DataSourceTable;
			if (_dtdoc == null && _dtdoc.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow row = _dtdoc.NewRow();
			_dtdoc.Rows.Add(row);
			_dtdoc.AcceptChanges();
			grd_Doc.DataSource = _dtdoc;
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetList()
	{
		try
		{
			GridControlEx gridControlEx = grd_Doc;
			gridControlEx.DataSource = await _httpClient.Vehicle.GetList();
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Saves()
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_Ok.Enabled = false;
				_dtdoc = grd_Doc.DataSourceTable;
				HttpResult httpResult = await _httpClient.Vehicle.Saves(_dtdoc);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await GetList();
					btn_Ok.Enabled = true;
					FormMode = Enums.FormViewModeType.View;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_Ok.Enabled = true;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		btn_Ok.Enabled = true;
		return 1;
	}

	private async void LoadLookupType()
	{
		try
		{
			DataTable dataTable = await _httpClient.Vehicle.GetDataType();
			rep_lookType.DataSource = dataTable;
			if (dataTable.IsNotNull())
			{
				gv_Doc.SetFocusedRowCellValue("Type", dataTable.Rows[0]["Code"]);
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupBranch()
	{
		try
		{
			DataTable dataTable = await _httpClient.Vehicle.GetDataBranch();
			rep_lookBra.DataSource = dataTable;
			if (dataTable.IsNotNull())
			{
				gv_Doc.SetFocusedRowCellValue("BPLId", dataTable.Rows[0]["BPLId"]);
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				foreach (DataRow item in gv_Doc.GetListDataSourceSelectedRow())
				{
					string text = await _httpClient.Vehicle.DeleteDocs(Function.ToString(item["Code"]));
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						await GetList();
						continue;
					}
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
					WinCommons.Error(Enums.MessageCode.DeleteError, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			string ID = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "Code"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["Code"]) == ID
				select t).ToArray();
			if (array != null && array.Length != 0)
			{
				for (int num = 0; num < array.Count(); num++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow.ItemArray = array[num].ItemArray;
					dataSourceTable.Rows.Add(dataRow);
				}
				grd_Doc.DataSource = dataSourceTable;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Vehicle_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				menu_AddNew.Enabled = gv_Doc.Columns.Count > 0;
				menu_Delete.Enabled = gv_Doc.RowCount > 0;
				menu_Duplicate.Enabled = gv_Doc.RowCount > 0;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_Vehicle));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddNew = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gd_name = new Apzon.Controls.Grid.GridColumnEx();
		this.VehicleNo = new Apzon.Controls.Grid.GridColumnEx();
		this.Type = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gd_Load = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_caLoad = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Driver = new Apzon.Controls.Grid.GridColumnEx();
		this.Phone = new Apzon.Controls.Grid.GridColumnEx();
		this.Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.BPLId = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookBra = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caLoad).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookBra).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(2, 3);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_lookType, this.rep_lookBra, this.rep_caLoad });
		this.grd_Doc.Size = new System.Drawing.Size(875, 227);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddNew, this.menu_Delete, this.menu_Duplicate });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
		this.menu_AddNew.Name = "menu_AddNew";
		this.menu_AddNew.Size = new System.Drawing.Size(124, 22);
		this.menu_AddNew.Text = "Add New";
		this.menu_AddNew.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.menu_Delete.Name = "menu_Delete";
		this.menu_Delete.Size = new System.Drawing.Size(124, 22);
		this.menu_Delete.Text = "Delete";
		this.menu_Delete.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.menu_Duplicate.Name = "menu_Duplicate";
		this.menu_Duplicate.Size = new System.Drawing.Size(124, 22);
		this.menu_Duplicate.Text = "Duplicate";
		this.menu_Duplicate.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.Code, this.gd_name, this.VehicleNo, this.Type, this.gd_Load, this.Driver, this.Phone, this.Remarks, this.BPLId });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oVehicle";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_VHCE";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gv_Doc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseDown);
		this.Code.Caption = "Code";
		this.Code.CurrencyDateField = null;
		this.Code.CurrencyField = null;
		this.Code.CurrencyFixed = null;
		this.Code.CurrencyFixedRate = 0.0;
		this.Code.CurrencyRateField = null;
		this.Code.FieldName = "Code";
		this.Code.Name = "Code";
		this.Code.PrimaryKey = true;
		this.Code.UnitField = null;
		this.Code.Visible = true;
		this.Code.VisibleIndex = 0;
		this.gd_name.Caption = "Name";
		this.gd_name.CurrencyDateField = null;
		this.gd_name.CurrencyField = null;
		this.gd_name.CurrencyFixed = null;
		this.gd_name.CurrencyFixedRate = 0.0;
		this.gd_name.CurrencyRateField = null;
		this.gd_name.FieldName = "Name";
		this.gd_name.Name = "gd_name";
		this.gd_name.PrimaryKey = false;
		this.gd_name.UnitField = null;
		this.gd_name.Visible = true;
		this.gd_name.VisibleIndex = 1;
		this.VehicleNo.Caption = "Vehicle No.";
		this.VehicleNo.CurrencyDateField = null;
		this.VehicleNo.CurrencyField = null;
		this.VehicleNo.CurrencyFixed = null;
		this.VehicleNo.CurrencyFixedRate = 0.0;
		this.VehicleNo.CurrencyRateField = null;
		this.VehicleNo.FieldName = "VehicleNo";
		this.VehicleNo.Name = "VehicleNo";
		this.VehicleNo.PrimaryKey = false;
		this.VehicleNo.UnitField = null;
		this.VehicleNo.Visible = true;
		this.VehicleNo.VisibleIndex = 2;
		this.Type.Caption = "Type";
		this.Type.ColumnEdit = this.rep_lookType;
		this.Type.CurrencyDateField = null;
		this.Type.CurrencyField = null;
		this.Type.CurrencyFixed = null;
		this.Type.CurrencyFixedRate = 0.0;
		this.Type.CurrencyRateField = null;
		this.Type.FieldName = "Type";
		this.Type.Name = "Type";
		this.Type.PrimaryKey = false;
		this.Type.UnitField = null;
		this.Type.Visible = true;
		this.Type.VisibleIndex = 3;
		this.rep_lookType.AutoHeight = false;
		this.rep_lookType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookType.DisplayMember = "Name";
		this.rep_lookType.Name = "rep_lookType";
		this.rep_lookType.NullText = "";
		this.rep_lookType.ShowHeader = false;
		this.rep_lookType.ValueMember = "Code";
		this.gd_Load.Caption = "Load(Kg)";
		this.gd_Load.ColumnEdit = this.rep_caLoad;
		this.gd_Load.CurrencyDateField = null;
		this.gd_Load.CurrencyField = null;
		this.gd_Load.CurrencyFixed = null;
		this.gd_Load.CurrencyFixedRate = 0.0;
		this.gd_Load.CurrencyRateField = null;
		this.gd_Load.FieldName = "Load";
		this.gd_Load.Name = "gd_Load";
		this.gd_Load.PrimaryKey = false;
		this.gd_Load.UnitField = null;
		this.gd_Load.Visible = true;
		this.gd_Load.VisibleIndex = 4;
		this.rep_caLoad.AutoHeight = false;
		this.rep_caLoad.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caLoad.Name = "rep_caLoad";
		this.Driver.Caption = "Driver";
		this.Driver.CurrencyDateField = null;
		this.Driver.CurrencyField = null;
		this.Driver.CurrencyFixed = null;
		this.Driver.CurrencyFixedRate = 0.0;
		this.Driver.CurrencyRateField = null;
		this.Driver.FieldName = "Driver";
		this.Driver.Name = "Driver";
		this.Driver.PrimaryKey = false;
		this.Driver.UnitField = null;
		this.Driver.Visible = true;
		this.Driver.VisibleIndex = 5;
		this.Phone.Caption = "Phone Number";
		this.Phone.CurrencyDateField = null;
		this.Phone.CurrencyField = null;
		this.Phone.CurrencyFixed = null;
		this.Phone.CurrencyFixedRate = 0.0;
		this.Phone.CurrencyRateField = null;
		this.Phone.FieldName = "Phone";
		this.Phone.Name = "Phone";
		this.Phone.PrimaryKey = false;
		this.Phone.UnitField = null;
		this.Phone.Visible = true;
		this.Phone.VisibleIndex = 6;
		this.Remarks.Caption = "Remarks";
		this.Remarks.CurrencyDateField = null;
		this.Remarks.CurrencyField = null;
		this.Remarks.CurrencyFixed = null;
		this.Remarks.CurrencyFixedRate = 0.0;
		this.Remarks.CurrencyRateField = null;
		this.Remarks.FieldName = "Remarks";
		this.Remarks.Name = "Remarks";
		this.Remarks.PrimaryKey = false;
		this.Remarks.UnitField = null;
		this.Remarks.Visible = true;
		this.Remarks.VisibleIndex = 7;
		this.BPLId.Caption = "Branch";
		this.BPLId.ColumnEdit = this.rep_lookBra;
		this.BPLId.CurrencyDateField = null;
		this.BPLId.CurrencyField = null;
		this.BPLId.CurrencyFixed = null;
		this.BPLId.CurrencyFixedRate = 0.0;
		this.BPLId.CurrencyRateField = null;
		this.BPLId.FieldName = "BPLId";
		this.BPLId.Name = "BPLId";
		this.BPLId.PrimaryKey = false;
		this.BPLId.UnitField = null;
		this.BPLId.Visible = true;
		this.BPLId.VisibleIndex = 8;
		this.rep_lookBra.AutoHeight = false;
		this.rep_lookBra.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookBra.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookBra.DisplayMember = "BPLName";
		this.rep_lookBra.Name = "rep_lookBra";
		this.rep_lookBra.NullText = "";
		this.rep_lookBra.ValueMember = "BPLId";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 235);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 20);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 235);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(878, 261);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oVehicle";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Vehicle";
		this.Text = "Vehicle";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Vehicle_FormClosing);
		base.Load += new System.EventHandler(this.frm_Vehicle_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caLoad).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookBra).EndInit();
		base.ResumeLayout(false);
	}
}
