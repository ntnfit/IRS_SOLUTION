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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.DMS;

public class frm_ListOfDistributors : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _id;

	private int _empId;

	private string _routeCode = string.Empty;

	public DataTable _dtSource = new DataTable();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemCheckEdit chk_Active;

	private GridColumnEx gc_CardCode;

	private GridColumnEx gc_CardName;

	private GridColumnEx gc_CardType;

	private GridColumnEx gc_Address;

	private LabelLine lbl_CardCode;

	private SimpleButtonEx btn_Search;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem menu_Remove;

	private LabelLine lbl_CardName;

	private RepositoryItemButtonEdit rep_CardCode;

	private ButtonEditEx btn_CardCode;

	private ButtonEditEx btn_CardName;

	private GridColumnEx gc_Id;

	private GridColumnEx gc_RouteCode;

	private GridColumnEx gc_EmpId;

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_ListOfDistributors(int id, string routeCode, int empId, DataTable dt)
	{
		InitializeComponent();
		_id = id;
		_routeCode = routeCode;
		_empId = empId;
		_dtSource = dt;
	}

	private async Task LoadData()
	{
		try
		{
			if (_dtSource.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from row in _dtSource.AsEnumerable()
					where Function.ParseInt(row["Id"]) == _id && Function.ToString(row["RouteCode"]) == _routeCode && Function.ParseInt(row["EmpId"]) == _empId
					select row;
				if (source.ToList().Count > 0)
				{
					grd_Doc.DataSource = source.CopyToDataTable();
				}
				else
				{
					grd_Doc.DataSource = _dtSource.Copy().Clone();
				}
			}
			else
			{
				DataTable dataSource = await _httpClient.SalesRouteClient.GetListDistributors(_id, _routeCode, _empId);
				grd_Doc.DataSource = dataSource;
			}
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowDistributors(int buttonIndex, bool isCardCode)
	{
		try
		{
			if (buttonIndex == 0)
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Distributors, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				if (isCardCode)
				{
					btn_CardCode.Text = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
				}
				else
				{
					btn_CardName.Text = Function.ToString(frm_CFL2.ChooseItem["CardName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Id"] = _id;
			dataRow["RouteCode"] = _routeCode;
			dataRow["EmpId"] = _empId;
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grd_Doc.DataSource = dataSourceTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_Distributors_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await LoadData();
		AddNewRow();
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				grd_Doc.DataSourceTable.AcceptChanges();
				DataRow dataRow = grd_Doc.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow row) => string.IsNullOrEmpty(Function.ToString(row["CardCode"])) && string.IsNullOrEmpty(Function.ToString(row["CardName"])));
				if (grd_Doc.DataSourceTable.Rows.Count == 1 && dataRow != null)
				{
					btn_OK.Enabled = true;
					WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
					return;
				}
				_dtSource = grd_Doc.DataSourceTable;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void frm_Distributors_FormClosing(object sender, FormClosingEventArgs e)
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			string filter = $"(CardCode = '' or CardCode like '%{btn_CardCode.Text.Trim()}%') and (CardName = '' or CardName like '%{btn_CardName.Text.Trim()}%')";
			DataTable dataTable = await _httpClient.SalesRouteClient.GetListDistributors(_id, _routeCode, _empId);
			if (!string.IsNullOrEmpty(btn_CardCode.Text) || !string.IsNullOrEmpty(btn_CardName.Text))
			{
				DataRow[] array = dataTable.Select(filter);
				if (!array.IsNotNull())
				{
					grd_Doc.DataSourceTable.Clear();
					btn_Search.Enabled = true;
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					return;
				}
				grd_Doc.DataSource = array.CopyToDataTable();
			}
			else
			{
				grd_Doc.DataSource = dataTable;
			}
			grd_Doc.RefreshDataSource();
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void rep_CardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_CardCode))))
				{
					frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
					frm_BusinessMasterData2.MdiParent = MdiParent;
					frm_BusinessMasterData2.Show();
				}
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Distributors, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				int focusedRowHandle = gv_Doc.FocusedRowHandle;
				gv_Doc.SetRowCellValue(focusedRowHandle, gc_CardCode, frm_CFL2.ChooseItem["CardCode"]);
				gv_Doc.SetRowCellValue(focusedRowHandle, gc_CardName, frm_CFL2.ChooseItem["CardName"]);
				gv_Doc.SetRowCellValue(focusedRowHandle, gc_CardType, frm_CFL2.ChooseItem["CardType"]);
				gv_Doc.SetRowCellValue(focusedRowHandle, gc_Address, frm_CFL2.ChooseItem["Address"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			menu_Remove.Enabled = dataSourceTable.IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.DeleteDataSourceRowSelected();
			grd_Doc.RefreshDataSource();
			if (!grd_Doc.DataSourceTable.IsNotNull())
			{
				AddNewRow();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_CardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0 || !string.IsNullOrEmpty(btn_CardCode.Text))
			{
				ShowDistributors(e.Button.Index, isCardCode: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_CardName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0 || !string.IsNullOrEmpty(btn_CardName.Text))
			{
				ShowDistributors(e.Button.Index, isCardCode: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowUpdated(object sender, RowObjectEventArgs e)
	{
		AddNewRow();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_ListOfDistributors));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CardType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_CardCode = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_CardName = new Apzon.Controls.Controls.LabelLine();
		this.btn_CardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_CardName = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardName.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(4, 33);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.chk_Active, this.rep_CardCode });
		this.grd_Doc.Size = new System.Drawing.Size(700, 320);
		this.grd_Doc.TabIndex = 3;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menu_Remove });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.menu_Remove.Name = "menu_Remove";
		this.menu_Remove.Size = new System.Drawing.Size(117, 22);
		this.menu_Remove.Text = "Remove";
		this.menu_Remove.Click += new System.EventHandler(this.menu_Remove_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gc_Id, this.gc_RouteCode, this.gc_EmpId, this.gc_CardCode, this.gc_CardName, this.gc_CardType, this.gc_Address });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gv_Doc_RowUpdated);
		this.gv_Doc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseDown);
		this.gc_Id.Caption = "Id";
		this.gc_Id.CurrencyDateField = null;
		this.gc_Id.CurrencyField = null;
		this.gc_Id.CurrencyFixed = null;
		this.gc_Id.CurrencyFixedRate = 0.0;
		this.gc_Id.CurrencyRateField = null;
		this.gc_Id.FieldName = "Id";
		this.gc_Id.Name = "gc_Id";
		this.gc_Id.OptionsColumn.AllowEdit = false;
		this.gc_Id.OptionsColumn.ReadOnly = true;
		this.gc_Id.PrimaryKey = true;
		this.gc_Id.UnitField = null;
		this.gc_RouteCode.Caption = "Route Code";
		this.gc_RouteCode.CurrencyDateField = null;
		this.gc_RouteCode.CurrencyField = null;
		this.gc_RouteCode.CurrencyFixed = null;
		this.gc_RouteCode.CurrencyFixedRate = 0.0;
		this.gc_RouteCode.CurrencyRateField = null;
		this.gc_RouteCode.FieldName = "RouteCode";
		this.gc_RouteCode.Name = "gc_RouteCode";
		this.gc_RouteCode.OptionsColumn.AllowEdit = false;
		this.gc_RouteCode.OptionsColumn.ReadOnly = true;
		this.gc_RouteCode.PrimaryKey = false;
		this.gc_RouteCode.UnitField = null;
		this.gc_EmpId.Caption = "Employee Id";
		this.gc_EmpId.CurrencyDateField = null;
		this.gc_EmpId.CurrencyField = null;
		this.gc_EmpId.CurrencyFixed = null;
		this.gc_EmpId.CurrencyFixedRate = 0.0;
		this.gc_EmpId.CurrencyRateField = null;
		this.gc_EmpId.FieldName = "EmpId";
		this.gc_EmpId.Name = "gc_EmpId";
		this.gc_EmpId.OptionsColumn.AllowEdit = false;
		this.gc_EmpId.OptionsColumn.ReadOnly = true;
		this.gc_EmpId.PrimaryKey = false;
		this.gc_EmpId.UnitField = null;
		this.gc_CardCode.Caption = "Card Code";
		this.gc_CardCode.ColumnEdit = this.rep_CardCode;
		this.gc_CardCode.CurrencyDateField = null;
		this.gc_CardCode.CurrencyField = null;
		this.gc_CardCode.CurrencyFixed = null;
		this.gc_CardCode.CurrencyFixedRate = 0.0;
		this.gc_CardCode.CurrencyRateField = null;
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.OptionsColumn.ReadOnly = true;
		this.gc_CardCode.PrimaryKey = false;
		this.gc_CardCode.UnitField = null;
		this.gc_CardCode.Visible = true;
		this.gc_CardCode.VisibleIndex = 0;
		this.gc_CardCode.Width = 100;
		this.rep_CardCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.ReadOnly = true;
		this.rep_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardCode_ButtonClick);
		this.gc_CardName.Caption = "Card Name";
		this.gc_CardName.CurrencyDateField = null;
		this.gc_CardName.CurrencyField = null;
		this.gc_CardName.CurrencyFixed = null;
		this.gc_CardName.CurrencyFixedRate = 0.0;
		this.gc_CardName.CurrencyRateField = null;
		this.gc_CardName.FieldName = "CardName";
		this.gc_CardName.Name = "gc_CardName";
		this.gc_CardName.OptionsColumn.ReadOnly = true;
		this.gc_CardName.PrimaryKey = false;
		this.gc_CardName.UnitField = null;
		this.gc_CardName.Visible = true;
		this.gc_CardName.VisibleIndex = 1;
		this.gc_CardName.Width = 125;
		this.gc_CardType.Caption = "Card Type";
		this.gc_CardType.CurrencyDateField = null;
		this.gc_CardType.CurrencyField = null;
		this.gc_CardType.CurrencyFixed = null;
		this.gc_CardType.CurrencyFixedRate = 0.0;
		this.gc_CardType.CurrencyRateField = null;
		this.gc_CardType.FieldName = "CardType";
		this.gc_CardType.Name = "gc_CardType";
		this.gc_CardType.OptionsColumn.ReadOnly = true;
		this.gc_CardType.PrimaryKey = false;
		this.gc_CardType.UnitField = null;
		this.gc_CardType.Visible = true;
		this.gc_CardType.VisibleIndex = 2;
		this.gc_Address.Caption = "Address";
		this.gc_Address.CurrencyDateField = null;
		this.gc_Address.CurrencyField = null;
		this.gc_Address.CurrencyFixed = null;
		this.gc_Address.CurrencyFixedRate = 0.0;
		this.gc_Address.CurrencyRateField = null;
		this.gc_Address.FieldName = "Address";
		this.gc_Address.Name = "gc_Address";
		this.gc_Address.OptionsColumn.ReadOnly = true;
		this.gc_Address.PrimaryKey = false;
		this.gc_Address.UnitField = null;
		this.gc_Address.Visible = true;
		this.gc_Address.VisibleIndex = 3;
		this.gc_Address.Width = 163;
		this.chk_Active.AutoHeight = false;
		this.chk_Active.DisplayValueChecked = "Y";
		this.chk_Active.DisplayValueUnchecked = "N";
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.ValueChecked = "Y";
		this.chk_Active.ValueUnchecked = "N";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(4, 358);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(82, 358);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.lbl_CardCode.EditControl = null;
		this.lbl_CardCode.EditValue = "Card Code";
		this.lbl_CardCode.FieldName = null;
		this.lbl_CardCode.HideWhenEmpty = false;
		this.lbl_CardCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CardCode.Location = new System.Drawing.Point(12, 10);
		this.lbl_CardCode.Name = "lbl_CardCode";
		this.lbl_CardCode.ReadOnly = false;
		this.lbl_CardCode.SettingInVisible = false;
		this.lbl_CardCode.SettingReadonly = false;
		this.lbl_CardCode.ShowUnderLine = true;
		this.lbl_CardCode.Size = new System.Drawing.Size(80, 20);
		this.lbl_CardCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CardCode.TabIndex = 0;
		this.lbl_CardCode.Utext = "Card Code";
		this.btn_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(477, 7);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 2;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lbl_CardName.EditControl = null;
		this.lbl_CardName.EditValue = "Card Name";
		this.lbl_CardName.FieldName = null;
		this.lbl_CardName.HideWhenEmpty = false;
		this.lbl_CardName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CardName.Location = new System.Drawing.Point(252, 10);
		this.lbl_CardName.Name = "lbl_CardName";
		this.lbl_CardName.ReadOnly = false;
		this.lbl_CardName.SettingInVisible = false;
		this.lbl_CardName.SettingReadonly = false;
		this.lbl_CardName.ShowUnderLine = true;
		this.lbl_CardName.Size = new System.Drawing.Size(80, 20);
		this.lbl_CardName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CardName.TabIndex = 0;
		this.lbl_CardName.Utext = "Card Name";
		this.btn_CardCode.EditControl = null;
		this.btn_CardCode.FieldName = null;
		this.btn_CardCode.IsSearchControl = false;
		this.btn_CardCode.Location = new System.Drawing.Point(89, 7);
		this.btn_CardCode.Name = "btn_CardCode";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_CardCode.SettingInVisible = false;
		this.btn_CardCode.SettingReadonly = false;
		this.btn_CardCode.Size = new System.Drawing.Size(146, 20);
		this.btn_CardCode.TabIndex = 6;
		this.btn_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_CardCode_ButtonClick);
		this.btn_CardName.EditControl = null;
		this.btn_CardName.FieldName = null;
		this.btn_CardName.IsSearchControl = false;
		this.btn_CardName.Location = new System.Drawing.Point(325, 7);
		this.btn_CardName.Name = "btn_CardName";
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_CardName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_CardName.SettingInVisible = false;
		this.btn_CardName.SettingReadonly = false;
		this.btn_CardName.Size = new System.Drawing.Size(146, 20);
		this.btn_CardName.TabIndex = 7;
		this.btn_CardName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_CardName_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(710, 384);
		base.Controls.Add(this.btn_CardName);
		base.Controls.Add(this.btn_CardCode);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lbl_CardName);
		base.Controls.Add(this.lbl_CardCode);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListOfDistributors";
		base.ShowInTaskbar = false;
		this.Text = "List of Distributors";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Distributors_FormClosing);
		base.Load += new System.EventHandler(this.frm_Distributors_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
