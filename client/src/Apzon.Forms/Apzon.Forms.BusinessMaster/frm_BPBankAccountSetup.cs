using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.BusinessMaster;

public class frm_BPBankAccountSetup : XtraFormEx
{
	public delegate void UpdateBankFormAccount(dynamic sender);

	public DataTable DtSource = new DataTable();

	public bool IsButtonClick;

	public bool IsUpdate;

	public int DflRow;

	public UpdateBankFormAccount UpdateBankFormAccountMethod;

	private IContainer components;

	private GridControlEx grd_BPBankAcct;

	private GridViewEx gv_BPBankAcct;

	private GridColumnEx gc_Address_Address;

	private GridColumnEx gc_Address_CardCode;

	private GridColumnEx gc_Address_AdresType;

	private GridColumnEx gc_Address_Street;

	private GridColumnEx gc_Address_StreetNo;

	private GridColumnEx gc_Address_Block;

	private GridColumnEx gc_Address_City;

	private GridColumnEx gc_Address_ZipCode;

	private GridColumnEx gc_Address_County;

	private GridColumnEx gc_Address_State;

	private GridColumnEx gc_Address_Country;

	private GridColumnEx gc_Address_Building;

	private GridColumnEx gc_Address_GlblLocNum;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Default;

	private RepositoryItemButtonEdit rep_BankCode;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private SimpleButtonEx btn_Cancel;

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
		case Enums.FormViewModeType.Update:
			btn_Add.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Add.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_BPBankAccountSetup()
	{
		InitializeComponent();
	}

	private void frm_BPBankAccountSetup_Load(object sender, EventArgs e)
	{
		try
		{
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool IsValidBankCode(string bankCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
		DataTable dataTable = httpClientHelper.Get("IsValidBankCode", new { bankCode });
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			return true;
		}
		return false;
	}

	private void gv_BPBankAcct_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			AddNewRow();
			if (grd_BPBankAcct.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && dataTable.AsEnumerable().Any((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["BankCode"]))))
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

	private void gv_BPBankAcct_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (gv_BPBankAcct.GetDataSourceRowIndex(e.RowHandle) == DflRow)
		{
			e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
		}
	}

	private void gv_BPBankAcct_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void rep_BankCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Banks, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_BPBankAcct.SetFocusedRowCellValue("BankCode", Function.ToString(frm_CFL2.ChooseItem["BankCode"]));
				gv_BPBankAcct.SetFocusedRowCellValue("Country", Function.ToString(frm_CFL2.ChooseItem["CountryCod"]));
				gv_BPBankAcct.SetFocusedRowCellValue("BankKey", Function.ToString(frm_CFL2.ChooseItem["AbsEntry"]));
				gv_BPBankAcct.SetFocusedRowCellValue("SwiftNum", Function.ToString(frm_CFL2.ChooseItem["SwiftNum"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_BPBankAcct.GetSelectedRows();
			gv_BPBankAcct.DeleteSelectedRows();
			DataTable dataTable = grd_BPBankAcct.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable != null)
			{
				if (selectedRows.Contains(DflRow))
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"])));
					if (dataRow != null)
					{
						DflRow = dataTable.Rows.IndexOf(dataRow);
					}
					else
					{
						DflRow = -1;
					}
				}
			}
			else
			{
				DflRow = -1;
			}
			if (dataTable.IsNotNull() && dataTable.AsEnumerable().Any((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["BankCode"]))))
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			gv_BPBankAcct.LayoutChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Default_Click(object sender, EventArgs e)
	{
		IsButtonClick = true;
		try
		{
			if (grd_BPBankAcct.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DflRow = gv_BPBankAcct.GetFocusedDataSourceRowIndex();
				gv_BPBankAcct.LayoutChanged();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btAdd_Click(object sender, EventArgs e)
	{
		try
		{
			if (ValidateData())
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					IsButtonClick = true;
					Close();
					return;
				}
				DtSource = grd_BPBankAcct.DataSource as DataTable;
				IsUpdate = true;
				UpdateBankFormAccountMethod(this);
				IsUpdate = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		IsButtonClick = false;
		Close();
	}

	public bool ValidateData()
	{
		if (grd_BPBankAcct.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
		{
			if (dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["Account"])) && !string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"]))))
			{
				WinCommons.Error(Enums.MessageCode.AccountCannotBeEmpty, "");
				return false;
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num]["BankCode"])) && !IsValidBankCode(Function.ToString(dataTable.Rows[num]["BankCode"])))
				{
					WinCommons.Error(Enums.MessageCode.InvalidBankCode, "");
					return false;
				}
			}
		}
		return true;
	}

	private void LoadData()
	{
		grd_BPBankAcct.DataSource = DtSource;
		grd_BPBankAcct.RefreshDataSource();
		AddNewRow();
	}

	private void AddNewRow()
	{
		DataTable dataTable = grd_BPBankAcct.DataSource as DataTable;
		if (dataTable == null)
		{
			dataTable = new DataTable();
		}
		if (dataTable.Rows.Count <= 0 || !dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"]))))
		{
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.AcceptChanges();
			grd_BPBankAcct.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_BPBankAccountSetup));
		this.grd_BPBankAcct = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_BPBankAcct = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Address_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BankCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Address_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_AdresType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_Street = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_StreetNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_Block = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_City = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_ZipCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_County = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_State = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_Country = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_Building = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_GlblLocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Default = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_BPBankAcct).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_BPBankAcct).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BankCode).BeginInit();
		base.SuspendLayout();
		this.grd_BPBankAcct.AllowPasteExcel = true;
		this.grd_BPBankAcct.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_BPBankAcct.ClipboardData = null;
		this.grd_BPBankAcct.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_BPBankAcct.Location = new System.Drawing.Point(4, 4);
		this.grd_BPBankAcct.MainView = this.gv_BPBankAcct;
		this.grd_BPBankAcct.Name = "grd_BPBankAcct";
		this.grd_BPBankAcct.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_BankCode });
		this.grd_BPBankAcct.Size = new System.Drawing.Size(644, 299);
		this.grd_BPBankAcct.TabIndex = 70;
		this.grd_BPBankAcct.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BPBankAcct });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.removeRowToolStripMenuItem.Text = "Remove row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_BPBankAcct.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BPBankAcct.ColumnKey = null;
		this.gv_BPBankAcct.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[22]
		{
			this.gc_Address_Address, this.gc_Address_CardCode, this.gc_Address_AdresType, this.gc_Address_Street, this.gc_Address_StreetNo, this.gc_Address_Block, this.gc_Address_City, this.gc_Address_ZipCode, this.gc_Address_County, this.gc_Address_State,
			this.gc_Address_Country, this.gc_Address_Building, this.gc_Address_GlblLocNum, this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7,
			this.gridColumn8, this.gridColumn9
		});
		this.gv_BPBankAcct.DisableFormSetting = false;
		this.gv_BPBankAcct.DisplayException = false;
		this.gv_BPBankAcct.EditColumn = null;
		this.gv_BPBankAcct.FieldCurrency = null;
		this.gv_BPBankAcct.FieldRate = null;
		this.gv_BPBankAcct.FormId = "oBpBankAccountSetup";
		this.gv_BPBankAcct.GridControl = this.grd_BPBankAcct;
		this.gv_BPBankAcct.IgnoreFilter = false;
		this.gv_BPBankAcct.IndexEditRow = -1;
		this.gv_BPBankAcct.IndicatorWidth = 40;
		this.gv_BPBankAcct.IsApzTable = false;
		this.gv_BPBankAcct.IsInsertNewRow = false;
		this.gv_BPBankAcct.IsLoadingData = false;
		this.gv_BPBankAcct.IsMappingContextAction = false;
		this.gv_BPBankAcct.ItemId = "OBAC";
		this.gv_BPBankAcct.LstColumnAllowBatch = null;
		this.gv_BPBankAcct.LstColumnNotAllowBatch = null;
		this.gv_BPBankAcct.LstColumnNotEditable = null;
		this.gv_BPBankAcct.LstColumNotSetting = null;
		this.gv_BPBankAcct.Name = "gv_BPBankAcct";
		this.gv_BPBankAcct.OptionsCustomization.AllowSort = false;
		this.gv_BPBankAcct.OptionsView.ColumnAutoWidth = false;
		this.gv_BPBankAcct.OptionsView.ShowGroupPanel = false;
		this.gv_BPBankAcct.UseBatchMod = false;
		this.gv_BPBankAcct.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_BPBankAcct_CustomDrawRowIndicator);
		this.gv_BPBankAcct.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_BPBankAcct_RowStyle);
		this.gv_BPBankAcct.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_BPBankAcct_CellValueChanged);
		this.gc_Address_Address.Caption = "Bank Code";
		this.gc_Address_Address.ColumnEdit = this.rep_BankCode;
		this.gc_Address_Address.CurrencyDateField = null;
		this.gc_Address_Address.CurrencyField = null;
		this.gc_Address_Address.CurrencyFixed = null;
		this.gc_Address_Address.CurrencyFixedRate = 0.0;
		this.gc_Address_Address.CurrencyRateField = null;
		this.gc_Address_Address.FieldName = "BankCode";
		this.gc_Address_Address.Name = "gc_Address_Address";
		this.gc_Address_Address.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_Address.PrimaryKey = true;
		this.gc_Address_Address.UnitField = null;
		this.gc_Address_Address.Visible = true;
		this.gc_Address_Address.VisibleIndex = 0;
		this.rep_BankCode.AutoHeight = false;
		this.rep_BankCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BankCode.Name = "rep_BankCode";
		this.rep_BankCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BankCode_ButtonPressed);
		this.gc_Address_CardCode.Caption = "Country";
		this.gc_Address_CardCode.CurrencyDateField = null;
		this.gc_Address_CardCode.CurrencyField = null;
		this.gc_Address_CardCode.CurrencyFixed = null;
		this.gc_Address_CardCode.CurrencyFixedRate = 0.0;
		this.gc_Address_CardCode.CurrencyRateField = null;
		this.gc_Address_CardCode.FieldName = "Country";
		this.gc_Address_CardCode.Name = "gc_Address_CardCode";
		this.gc_Address_CardCode.OptionsColumn.AllowEdit = false;
		this.gc_Address_CardCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_CardCode.OptionsColumn.ReadOnly = true;
		this.gc_Address_CardCode.PrimaryKey = false;
		this.gc_Address_CardCode.UnitField = null;
		this.gc_Address_CardCode.Visible = true;
		this.gc_Address_CardCode.VisibleIndex = 1;
		this.gc_Address_AdresType.Caption = "Account No.";
		this.gc_Address_AdresType.CurrencyDateField = null;
		this.gc_Address_AdresType.CurrencyField = null;
		this.gc_Address_AdresType.CurrencyFixed = null;
		this.gc_Address_AdresType.CurrencyFixedRate = 0.0;
		this.gc_Address_AdresType.CurrencyRateField = null;
		this.gc_Address_AdresType.FieldName = "Account";
		this.gc_Address_AdresType.Name = "gc_Address_AdresType";
		this.gc_Address_AdresType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_AdresType.PrimaryKey = false;
		this.gc_Address_AdresType.UnitField = null;
		this.gc_Address_AdresType.Visible = true;
		this.gc_Address_AdresType.VisibleIndex = 2;
		this.gc_Address_Street.Caption = "Branch";
		this.gc_Address_Street.CurrencyDateField = null;
		this.gc_Address_Street.CurrencyField = null;
		this.gc_Address_Street.CurrencyFixed = null;
		this.gc_Address_Street.CurrencyFixedRate = 0.0;
		this.gc_Address_Street.CurrencyRateField = null;
		this.gc_Address_Street.FieldName = "Branch";
		this.gc_Address_Street.Name = "gc_Address_Street";
		this.gc_Address_Street.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_Street.PrimaryKey = false;
		this.gc_Address_Street.UnitField = null;
		this.gc_Address_Street.Visible = true;
		this.gc_Address_Street.VisibleIndex = 3;
		this.gc_Address_StreetNo.Caption = "BIC/SWIFT Code";
		this.gc_Address_StreetNo.CurrencyDateField = null;
		this.gc_Address_StreetNo.CurrencyField = null;
		this.gc_Address_StreetNo.CurrencyFixed = null;
		this.gc_Address_StreetNo.CurrencyFixedRate = 0.0;
		this.gc_Address_StreetNo.CurrencyRateField = null;
		this.gc_Address_StreetNo.FieldName = "SwiftNum";
		this.gc_Address_StreetNo.Name = "gc_Address_StreetNo";
		this.gc_Address_StreetNo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_StreetNo.PrimaryKey = false;
		this.gc_Address_StreetNo.UnitField = null;
		this.gc_Address_StreetNo.Visible = true;
		this.gc_Address_StreetNo.VisibleIndex = 4;
		this.gc_Address_StreetNo.Width = 91;
		this.gc_Address_Block.Caption = "Bank Internal ID";
		this.gc_Address_Block.CurrencyDateField = null;
		this.gc_Address_Block.CurrencyField = null;
		this.gc_Address_Block.CurrencyFixed = null;
		this.gc_Address_Block.CurrencyFixedRate = 0.0;
		this.gc_Address_Block.CurrencyRateField = null;
		this.gc_Address_Block.FieldName = "BankKey";
		this.gc_Address_Block.Name = "gc_Address_Block";
		this.gc_Address_Block.OptionsColumn.AllowEdit = false;
		this.gc_Address_Block.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_Block.OptionsColumn.ReadOnly = true;
		this.gc_Address_Block.PrimaryKey = false;
		this.gc_Address_Block.UnitField = null;
		this.gc_Address_Block.Visible = true;
		this.gc_Address_Block.VisibleIndex = 5;
		this.gc_Address_Block.Width = 88;
		this.gc_Address_City.Caption = "Bank Account Name";
		this.gc_Address_City.CurrencyDateField = null;
		this.gc_Address_City.CurrencyField = null;
		this.gc_Address_City.CurrencyFixed = null;
		this.gc_Address_City.CurrencyFixedRate = 0.0;
		this.gc_Address_City.CurrencyRateField = null;
		this.gc_Address_City.FieldName = "AcctName";
		this.gc_Address_City.Name = "gc_Address_City";
		this.gc_Address_City.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_City.PrimaryKey = false;
		this.gc_Address_City.UnitField = null;
		this.gc_Address_City.Visible = true;
		this.gc_Address_City.VisibleIndex = 6;
		this.gc_Address_City.Width = 105;
		this.gc_Address_ZipCode.Caption = "Ctrl Int. ID";
		this.gc_Address_ZipCode.CurrencyDateField = null;
		this.gc_Address_ZipCode.CurrencyField = null;
		this.gc_Address_ZipCode.CurrencyFixed = null;
		this.gc_Address_ZipCode.CurrencyFixedRate = 0.0;
		this.gc_Address_ZipCode.CurrencyRateField = null;
		this.gc_Address_ZipCode.FieldName = "ControlKey";
		this.gc_Address_ZipCode.Name = "gc_Address_ZipCode";
		this.gc_Address_ZipCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_ZipCode.PrimaryKey = false;
		this.gc_Address_ZipCode.UnitField = null;
		this.gc_Address_ZipCode.Visible = true;
		this.gc_Address_ZipCode.VisibleIndex = 7;
		this.gc_Address_County.Caption = "IBAN";
		this.gc_Address_County.CurrencyDateField = null;
		this.gc_Address_County.CurrencyField = null;
		this.gc_Address_County.CurrencyFixed = null;
		this.gc_Address_County.CurrencyFixedRate = 0.0;
		this.gc_Address_County.CurrencyRateField = null;
		this.gc_Address_County.FieldName = "IBAN";
		this.gc_Address_County.Name = "gc_Address_County";
		this.gc_Address_County.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_County.PrimaryKey = false;
		this.gc_Address_County.UnitField = null;
		this.gc_Address_County.Visible = true;
		this.gc_Address_County.VisibleIndex = 8;
		this.gc_Address_State.Caption = "Mandate ID";
		this.gc_Address_State.CurrencyDateField = null;
		this.gc_Address_State.CurrencyField = null;
		this.gc_Address_State.CurrencyFixed = null;
		this.gc_Address_State.CurrencyFixedRate = 0.0;
		this.gc_Address_State.CurrencyRateField = null;
		this.gc_Address_State.FieldName = "MandateID";
		this.gc_Address_State.Name = "gc_Address_State";
		this.gc_Address_State.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_State.PrimaryKey = false;
		this.gc_Address_State.UnitField = null;
		this.gc_Address_State.Visible = true;
		this.gc_Address_State.VisibleIndex = 9;
		this.gc_Address_Country.Caption = "Date of Signature";
		this.gc_Address_Country.CurrencyDateField = null;
		this.gc_Address_Country.CurrencyField = null;
		this.gc_Address_Country.CurrencyFixed = null;
		this.gc_Address_Country.CurrencyFixedRate = 0.0;
		this.gc_Address_Country.CurrencyRateField = null;
		this.gc_Address_Country.FieldName = "SignDate";
		this.gc_Address_Country.Name = "gc_Address_Country";
		this.gc_Address_Country.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_Country.PrimaryKey = false;
		this.gc_Address_Country.UnitField = null;
		this.gc_Address_Country.Visible = true;
		this.gc_Address_Country.VisibleIndex = 10;
		this.gc_Address_Country.Width = 95;
		this.gc_Address_Building.Caption = "Street";
		this.gc_Address_Building.CurrencyDateField = null;
		this.gc_Address_Building.CurrencyField = null;
		this.gc_Address_Building.CurrencyFixed = null;
		this.gc_Address_Building.CurrencyFixedRate = 0.0;
		this.gc_Address_Building.CurrencyRateField = null;
		this.gc_Address_Building.FieldName = "Street";
		this.gc_Address_Building.Name = "gc_Address_Building";
		this.gc_Address_Building.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_Building.PrimaryKey = false;
		this.gc_Address_Building.UnitField = null;
		this.gc_Address_Building.Visible = true;
		this.gc_Address_Building.VisibleIndex = 11;
		this.gc_Address_GlblLocNum.Caption = "Street No.";
		this.gc_Address_GlblLocNum.CurrencyDateField = null;
		this.gc_Address_GlblLocNum.CurrencyField = null;
		this.gc_Address_GlblLocNum.CurrencyFixed = null;
		this.gc_Address_GlblLocNum.CurrencyFixedRate = 0.0;
		this.gc_Address_GlblLocNum.CurrencyRateField = null;
		this.gc_Address_GlblLocNum.FieldName = "StreetNo";
		this.gc_Address_GlblLocNum.Name = "gc_Address_GlblLocNum";
		this.gc_Address_GlblLocNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Address_GlblLocNum.PrimaryKey = false;
		this.gc_Address_GlblLocNum.UnitField = null;
		this.gc_Address_GlblLocNum.Visible = true;
		this.gc_Address_GlblLocNum.VisibleIndex = 12;
		this.gridColumn1.Caption = "Zip Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ZipCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 13;
		this.gridColumn2.Caption = "Block";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Block";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 14;
		this.gridColumn3.Caption = "City";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "City";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 15;
		this.gridColumn4.Caption = "County";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "County";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 16;
		this.gridColumn5.Caption = "State";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "State";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 17;
		this.gridColumn6.Caption = "User No. 1";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "UsrNumber1";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 18;
		this.gridColumn7.Caption = "User No. 2";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "UsrNumber2";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 19;
		this.gridColumn8.Caption = "User No. 3";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "UsrNumber3";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 20;
		this.gridColumn9.Caption = "User No. 4";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "UsrNumber4";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 21;
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(4, 313);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(68, 20);
		this.btn_Add.TabIndex = 71;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btAdd_Click);
		this.btn_Default.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Default.EditControl = null;
		this.btn_Default.EditValue = "Set as Default";
		this.btn_Default.FieldName = null;
		this.btn_Default.IsSearchControl = false;
		this.btn_Default.Location = new System.Drawing.Point(552, 313);
		this.btn_Default.Name = "btn_Default";
		this.btn_Default.ReadOnly = false;
		this.btn_Default.SettingInVisible = false;
		this.btn_Default.SettingReadonly = false;
		this.btn_Default.Size = new System.Drawing.Size(96, 20);
		this.btn_Default.TabIndex = 72;
		this.btn_Default.Text = "Set as Default";
		this.btn_Default.Click += new System.EventHandler(this.btn_Default_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(75, 313);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(68, 20);
		this.btn_Cancel.TabIndex = 73;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(651, 342);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Default);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.grd_BPBankAcct);
		base.FormId = "oBpBankAccountSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BPBankAccountSetup";
		this.Text = "Business Partner Bank Account Setup";
		base.Load += new System.EventHandler(this.frm_BPBankAccountSetup_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_BPBankAcct).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_BPBankAcct).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BankCode).EndInit();
		base.ResumeLayout(false);
	}
}
