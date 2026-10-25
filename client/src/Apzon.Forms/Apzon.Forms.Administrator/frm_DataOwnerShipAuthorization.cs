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
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_DataOwnerShipAuthorization : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string Privilege = "";

	private IContainer components;

	private GridControlEx grd_Employess;

	private GridViewEx gv_Employees;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gcObject;

	private GridColumnEx gcPeer;

	private GridColumnEx gcManager;

	private GridColumnEx gcSubord;

	private GridColumnEx gcDept;

	private GridColumnEx gcBranch;

	private GridColumnEx gcTeam;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_FullAuthorization;

	private SimpleButtonEx btn_ReadOnly;

	private SimpleButtonEx btn_NoAuthorization;

	private GroupControl groupControl1;

	private SplitContainerControl splitContainerControl1;

	private RepositoryItemLookUpEdit rep_authorization;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private RepositoryItemLookUpEdit rep_Object;

	private GridColumnEx gcCmpny;

	private SimpleButton btn_SupperUser;

	private LabelControlEx lb_SupperUser;

	private SimpleButtonEx btn_Copy;

	public Enums.FormViewModeType ViewMode { get; set; } = Enums.FormViewModeType.View;

	public frm_DataOwnerShipAuthorization()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DataOwnerShipAuthorization"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DataOwnerShipAuthorization" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_FullAuthorization.Enabled = true;
				btn_NoAuthorization.Enabled = true;
				btn_ReadOnly.Enabled = true;
				Privilege = "R";
			}
			else
			{
				Privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_DataOwnerShipAuthorization_Load(object sender, EventArgs e)
	{
		btn_OK.Text = Resources.ButtonOK;
		LoadCbx();
		LoadObject();
		await LoadEmployees();
	}

	private void LoadObject()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = 19;
			dataRow["Name"] = "A/P Credit Memo";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 204;
			dataRow["Name"] = "A/P Down Payment";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 18;
			dataRow["Name"] = "A/P Invoice";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 14;
			dataRow["Name"] = "A/R Credit";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 203;
			dataRow["Name"] = "A/R Down Payment";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 13;
			dataRow["Name"] = "A/R Invoice";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 15;
			dataRow["Name"] = "Delivery";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 20;
			dataRow["Name"] = "Goods Receipt PO";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 21;
			dataRow["Name"] = "Goods Return";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 97;
			dataRow["Name"] = "Opportunity";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 22;
			dataRow["Name"] = "Purchase Order";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 540000006;
			dataRow["Name"] = "Purchase Quotation";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1470000113;
			dataRow["Name"] = "Purchase Request";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 234000031;
			dataRow["Name"] = "Return Request";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 16;
			dataRow["Name"] = "Return";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 17;
			dataRow["Name"] = "Sales Order";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 23;
			dataRow["Name"] = "Sales Quotations";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 2;
			dataRow["Name"] = "Busness Partners";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 4;
			dataRow["Name"] = "Items";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1470000128;
			dataRow["Name"] = "Task Management";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1470000129;
			dataRow["Name"] = "Event Management";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 19000102;
			dataRow["Name"] = "Note Management";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1470000132;
			dataRow["Name"] = "Project Management";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1250000025;
			dataRow["Name"] = "Blanket Agreement";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 1470000130;
			dataRow["Name"] = "Tracking Issue";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 171;
			dataRow["Name"] = "Employees";
			dataTable.Rows.Add(dataRow);
			rep_Object.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadEmployees()
	{
		try
		{
			DataTable dataTable = await _httpClient.Commons.GetAllEmployees();
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("userId"))
			{
				DataRow[] array = dataTable.Select("userid<>0");
				if (array.IsNotNull())
				{
					grd_Employess.DataSource = array.CopyToDataTable();
				}
				else
				{
					grd_Employess.DataSource = null;
				}
				grd_Employess.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadCbx()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "F";
			dataRow["Name"] = "Full";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "R";
			dataRow["Name"] = "Read-Only";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "N";
			dataRow["Name"] = "None";
			dataTable.Rows.Add(dataRow);
			rep_authorization.DataSource = dataTable;
			rep_authorization.DisplayMember = "Name";
			rep_authorization.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Employees_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int empId = Function.ParseInt(gv_Employees.GetFocusedRowCellValue("empID"));
			DataTable dataSource = await _httpClient.OwnerAuthorize.GetOwnerShipAuthorization(empId);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			DataTable dataSourceTable = grd_Employess.DataSourceTable;
			if (dataSourceTable.IsNotNull() && dataSourceTable.ExistsColumn("SUPERUSER"))
			{
				DataRow dataRow = dataSourceTable.Select("empID=" + empId).FirstOrDefault();
				if (dataRow != null && Function.ToString(dataRow["SUPERUSER"]).Equals("Y"))
				{
					btn_FullAuthorization.Enabled = false;
					btn_NoAuthorization.Enabled = false;
					btn_ReadOnly.Enabled = false;
					gv_Doc.OptionsBehavior.Editable = false;
					lb_SupperUser.Visible = true;
					btn_SupperUser.Visible = true;
					return;
				}
			}
			btn_FullAuthorization.Enabled = true;
			btn_NoAuthorization.Enabled = true;
			btn_ReadOnly.Enabled = true;
			gv_Doc.OptionsBehavior.Editable = true;
			lb_SupperUser.Visible = false;
			btn_SupperUser.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_FullAuthorization_Click(object sender, EventArgs e)
	{
		SetAuthorize("F");
	}

	private void SetAuthorize(string rule)
	{
		try
		{
			if (gv_Doc.RowCount > 0)
			{
				for (int i = 0; i < gv_Doc.RowCount; i++)
				{
					gv_Doc.SetRowCellValue(i, "Peer", rule);
					gv_Doc.SetRowCellValue(i, "Manager", rule);
					gv_Doc.SetRowCellValue(i, "Subord", rule);
					gv_Doc.SetRowCellValue(i, "Dept", rule);
					gv_Doc.SetRowCellValue(i, "Branch", rule);
					gv_Doc.SetRowCellValue(i, "Team", rule);
					gv_Doc.SetRowCellValue(i, "Cmpny", rule);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ReadOnly_Click(object sender, EventArgs e)
	{
		SetAuthorize("R");
	}

	private void btn_NoAuthorization_Click(object sender, EventArgs e)
	{
		SetAuthorize("N");
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		ViewMode = Enums.FormViewModeType.Update;
		btn_OK.Text = Resources.ButtonUpdate;
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_OK.Text.Equals(Resources.ButtonOK))
			{
				Close();
				return;
			}
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				string text = await _httpClient.OwnerAuthorize.Save(dataSourceTable);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				btn_OK.Text = Resources.ButtonOK;
				ViewMode = Enums.FormViewModeType.View;
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
		if (btn_OK.Text.Equals(Resources.ButtonOK))
		{
			Close();
		}
		else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void gv_Doc_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			int objTyp = Function.ParseInt(gv_Doc.GetListSourceRowCellValue(e.ListSourceRow, "Object"));
			if (!(rep_Object.DataSource as DataTable).AsEnumerable().Any((DataRow s) => Function.ParseInt(s["Code"]) == objTyp))
			{
				e.Visible = false;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Employees.GetFocusedRowCellValue("SUPERUSER")) == "Y")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Employees_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Employees.GetRowCellValue(e.RowHandle, "SUPERUSER")) == "Y")
			{
				e.Appearance.ForeColor = Color.Maroon;
				e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Copy_Click(object sender, EventArgs e)
	{
		try
		{
			frm_DataOwnerShipAuthorizationCopy frm = new frm_DataOwnerShipAuthorizationCopy();
			frm.EmpId = Function.ParseInt(gv_Employees.GetFocusedRowCellValue("empID"));
			DataRow[] array = (from t in grd_Employess.DatasourceTable().AsEnumerable()
				where Function.ParseInt(t["empId"]) != frm.EmpId
				select t).ToArray();
			if (array.Length != 0)
			{
				frm.Employees = array.CopyToDataTable();
			}
			frm.ShowDialog(this);
			gv_Employees_FocusedRowObjectChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_DataOwnerShipAuthorization));
		this.grd_Employess = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Employees = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_FullAuthorization = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ReadOnly = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NoAuthorization = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcObject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Object = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPeer = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_authorization = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcManager = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSubord = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDept = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTeam = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCmpny = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_SupperUser = new DevExpress.XtraEditors.SimpleButton();
		this.lb_SupperUser = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_Copy = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Employess).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employees).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Object).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_authorization).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.grd_Employess.AllowPasteExcel = false;
		this.grd_Employess.ClipboardData = null;
		this.grd_Employess.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Employess.Location = new System.Drawing.Point(2, 23);
		this.grd_Employess.MainView = this.gv_Employees;
		this.grd_Employess.Name = "grd_Employess";
		this.grd_Employess.Size = new System.Drawing.Size(200, 353);
		this.grd_Employess.TabIndex = 0;
		this.grd_Employess.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Employees });
		this.gv_Employees.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Employees.ColumnKey = null;
		this.gv_Employees.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gv_Employees.DisableFormSetting = false;
		this.gv_Employees.DisplayException = false;
		this.gv_Employees.EditColumn = null;
		this.gv_Employees.FieldCurrency = null;
		this.gv_Employees.FieldRate = null;
		this.gv_Employees.FormId = "oDataOwnerShipAuthorization";
		this.gv_Employees.GridControl = this.grd_Employess;
		this.gv_Employees.IgnoreFilter = false;
		this.gv_Employees.IndexEditRow = -1;
		this.gv_Employees.IndicatorWidth = 35;
		this.gv_Employees.IsApzTable = false;
		this.gv_Employees.IsInsertNewRow = false;
		this.gv_Employees.IsLoadingData = false;
		this.gv_Employees.IsMappingContextAction = false;
		this.gv_Employees.ItemId = "OHEM";
		this.gv_Employees.LstColumnAllowBatch = null;
		this.gv_Employees.LstColumnNotAllowBatch = null;
		this.gv_Employees.LstColumnNotEditable = null;
		this.gv_Employees.LstColumNotSetting = null;
		this.gv_Employees.Name = "gv_Employees";
		this.gv_Employees.OptionsBehavior.Editable = false;
		this.gv_Employees.OptionsView.ShowGroupPanel = false;
		this.gv_Employees.UseBatchMod = false;
		this.gv_Employees.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Employees_RowCellStyle);
		this.gv_Employees.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Employees_FocusedRowObjectChanged);
		this.gridColumn1.Caption = "empID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "empID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "FullName";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "FullName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 150;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 386);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(80, 20);
		this.btn_OK.TabIndex = 2;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(89, 386);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(80, 20);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_FullAuthorization.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_FullAuthorization.EditControl = null;
		this.btn_FullAuthorization.EditValue = "Full Authorization";
		this.btn_FullAuthorization.Enabled = false;
		this.btn_FullAuthorization.FieldName = null;
		this.btn_FullAuthorization.IsSearchControl = false;
		this.btn_FullAuthorization.Location = new System.Drawing.Point(827, 386);
		this.btn_FullAuthorization.Name = "btn_FullAuthorization";
		this.btn_FullAuthorization.ReadOnly = false;
		this.btn_FullAuthorization.SettingInVisible = false;
		this.btn_FullAuthorization.SettingReadonly = false;
		this.btn_FullAuthorization.Size = new System.Drawing.Size(100, 20);
		this.btn_FullAuthorization.TabIndex = 3;
		this.btn_FullAuthorization.Text = "Full Authorization";
		this.btn_FullAuthorization.Click += new System.EventHandler(this.btn_FullAuthorization_Click);
		this.btn_ReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ReadOnly.EditControl = null;
		this.btn_ReadOnly.EditValue = "Read Only";
		this.btn_ReadOnly.Enabled = false;
		this.btn_ReadOnly.FieldName = null;
		this.btn_ReadOnly.IsSearchControl = false;
		this.btn_ReadOnly.Location = new System.Drawing.Point(935, 386);
		this.btn_ReadOnly.Name = "btn_ReadOnly";
		this.btn_ReadOnly.ReadOnly = false;
		this.btn_ReadOnly.SettingInVisible = false;
		this.btn_ReadOnly.SettingReadonly = false;
		this.btn_ReadOnly.Size = new System.Drawing.Size(100, 20);
		this.btn_ReadOnly.TabIndex = 4;
		this.btn_ReadOnly.Text = "Read Only";
		this.btn_ReadOnly.Click += new System.EventHandler(this.btn_ReadOnly_Click);
		this.btn_NoAuthorization.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NoAuthorization.EditControl = null;
		this.btn_NoAuthorization.EditValue = "No Authorization";
		this.btn_NoAuthorization.Enabled = false;
		this.btn_NoAuthorization.FieldName = null;
		this.btn_NoAuthorization.IsSearchControl = false;
		this.btn_NoAuthorization.Location = new System.Drawing.Point(1038, 386);
		this.btn_NoAuthorization.Name = "btn_NoAuthorization";
		this.btn_NoAuthorization.ReadOnly = false;
		this.btn_NoAuthorization.SettingInVisible = false;
		this.btn_NoAuthorization.SettingReadonly = false;
		this.btn_NoAuthorization.Size = new System.Drawing.Size(100, 20);
		this.btn_NoAuthorization.TabIndex = 5;
		this.btn_NoAuthorization.Text = "No Authorization";
		this.btn_NoAuthorization.Click += new System.EventHandler(this.btn_NoAuthorization_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_authorization, this.rep_Object });
		this.grd_Doc.Size = new System.Drawing.Size(922, 378);
		this.grd_Doc.TabIndex = 6;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcObject, this.gcPeer, this.gcManager, this.gcSubord, this.gcDept, this.gcBranch, this.gcTeam, this.gcCmpny });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oDataOwnerShipAuthorization";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOW";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_Doc_CustomRowFilter);
		this.gcObject.Caption = "Document";
		this.gcObject.ColumnEdit = this.rep_Object;
		this.gcObject.CurrencyDateField = null;
		this.gcObject.CurrencyField = null;
		this.gcObject.CurrencyFixed = null;
		this.gcObject.CurrencyFixedRate = 0.0;
		this.gcObject.CurrencyRateField = null;
		this.gcObject.FieldName = "Object";
		this.gcObject.Name = "gcObject";
		this.gcObject.OptionsColumn.AllowEdit = false;
		this.gcObject.OptionsColumn.ReadOnly = true;
		this.gcObject.PrimaryKey = true;
		this.gcObject.UnitField = null;
		this.gcObject.Visible = true;
		this.gcObject.VisibleIndex = 0;
		this.gcObject.Width = 171;
		this.rep_Object.AutoHeight = false;
		this.rep_Object.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Object.DisplayMember = "Name";
		this.rep_Object.Name = "rep_Object";
		this.rep_Object.NullText = "";
		this.rep_Object.ValueMember = "Code";
		this.gcPeer.Caption = "Peer";
		this.gcPeer.ColumnEdit = this.rep_authorization;
		this.gcPeer.CurrencyDateField = null;
		this.gcPeer.CurrencyField = null;
		this.gcPeer.CurrencyFixed = null;
		this.gcPeer.CurrencyFixedRate = 0.0;
		this.gcPeer.CurrencyRateField = null;
		this.gcPeer.FieldName = "Peer";
		this.gcPeer.Name = "gcPeer";
		this.gcPeer.PrimaryKey = false;
		this.gcPeer.UnitField = null;
		this.gcPeer.Visible = true;
		this.gcPeer.VisibleIndex = 1;
		this.rep_authorization.AutoHeight = false;
		this.rep_authorization.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_authorization.Name = "rep_authorization";
		this.rep_authorization.NullText = "";
		this.gcManager.Caption = "Manager";
		this.gcManager.ColumnEdit = this.rep_authorization;
		this.gcManager.CurrencyDateField = null;
		this.gcManager.CurrencyField = null;
		this.gcManager.CurrencyFixed = null;
		this.gcManager.CurrencyFixedRate = 0.0;
		this.gcManager.CurrencyRateField = null;
		this.gcManager.FieldName = "Manager";
		this.gcManager.Name = "gcManager";
		this.gcManager.PrimaryKey = false;
		this.gcManager.UnitField = null;
		this.gcManager.Visible = true;
		this.gcManager.VisibleIndex = 2;
		this.gcSubord.Caption = "Subordinate";
		this.gcSubord.ColumnEdit = this.rep_authorization;
		this.gcSubord.CurrencyDateField = null;
		this.gcSubord.CurrencyField = null;
		this.gcSubord.CurrencyFixed = null;
		this.gcSubord.CurrencyFixedRate = 0.0;
		this.gcSubord.CurrencyRateField = null;
		this.gcSubord.FieldName = "Subord";
		this.gcSubord.Name = "gcSubord";
		this.gcSubord.PrimaryKey = false;
		this.gcSubord.UnitField = null;
		this.gcSubord.Visible = true;
		this.gcSubord.VisibleIndex = 3;
		this.gcDept.Caption = "Department";
		this.gcDept.ColumnEdit = this.rep_authorization;
		this.gcDept.CurrencyDateField = null;
		this.gcDept.CurrencyField = null;
		this.gcDept.CurrencyFixed = null;
		this.gcDept.CurrencyFixedRate = 0.0;
		this.gcDept.CurrencyRateField = null;
		this.gcDept.FieldName = "Dept";
		this.gcDept.Name = "gcDept";
		this.gcDept.PrimaryKey = false;
		this.gcDept.UnitField = null;
		this.gcDept.Visible = true;
		this.gcDept.VisibleIndex = 4;
		this.gcBranch.Caption = "Branch";
		this.gcBranch.ColumnEdit = this.rep_authorization;
		this.gcBranch.CurrencyDateField = null;
		this.gcBranch.CurrencyField = null;
		this.gcBranch.CurrencyFixed = null;
		this.gcBranch.CurrencyFixedRate = 0.0;
		this.gcBranch.CurrencyRateField = null;
		this.gcBranch.FieldName = "Branch";
		this.gcBranch.Name = "gcBranch";
		this.gcBranch.PrimaryKey = false;
		this.gcBranch.UnitField = null;
		this.gcBranch.Visible = true;
		this.gcBranch.VisibleIndex = 5;
		this.gcTeam.Caption = "Team";
		this.gcTeam.ColumnEdit = this.rep_authorization;
		this.gcTeam.CurrencyDateField = null;
		this.gcTeam.CurrencyField = null;
		this.gcTeam.CurrencyFixed = null;
		this.gcTeam.CurrencyFixedRate = 0.0;
		this.gcTeam.CurrencyRateField = null;
		this.gcTeam.FieldName = "Team";
		this.gcTeam.Name = "gcTeam";
		this.gcTeam.PrimaryKey = false;
		this.gcTeam.UnitField = null;
		this.gcTeam.Visible = true;
		this.gcTeam.VisibleIndex = 6;
		this.gcCmpny.Caption = "Company";
		this.gcCmpny.ColumnEdit = this.rep_authorization;
		this.gcCmpny.CurrencyDateField = null;
		this.gcCmpny.CurrencyField = null;
		this.gcCmpny.CurrencyFixed = null;
		this.gcCmpny.CurrencyFixedRate = 0.0;
		this.gcCmpny.CurrencyRateField = null;
		this.gcCmpny.FieldName = "Cmpny";
		this.gcCmpny.Name = "gcCmpny";
		this.gcCmpny.PrimaryKey = false;
		this.gcCmpny.UnitField = null;
		this.gcCmpny.Visible = true;
		this.gcCmpny.VisibleIndex = 7;
		this.groupControl1.Controls.Add(this.grd_Employess);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(204, 378);
		this.groupControl1.TabIndex = 7;
		this.groupControl1.Text = "Employees";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1136, 378);
		this.splitContainerControl1.SplitterPosition = 204;
		this.splitContainerControl1.TabIndex = 8;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_SupperUser.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SupperUser.Appearance.BackColor = System.Drawing.Color.Maroon;
		this.btn_SupperUser.Appearance.Options.UseBackColor = true;
		this.btn_SupperUser.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_SupperUser.Location = new System.Drawing.Point(711, 388);
		this.btn_SupperUser.Name = "btn_SupperUser";
		this.btn_SupperUser.Size = new System.Drawing.Size(23, 18);
		this.btn_SupperUser.TabIndex = 9;
		this.btn_SupperUser.Visible = false;
		this.lb_SupperUser.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SupperUser.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lb_SupperUser.Appearance.Options.UseFont = true;
		this.lb_SupperUser.EditControl = null;
		this.lb_SupperUser.EditValue = "Supper User";
		this.lb_SupperUser.FieldName = null;
		this.lb_SupperUser.Location = new System.Drawing.Point(740, 393);
		this.lb_SupperUser.Name = "lb_SupperUser";
		this.lb_SupperUser.ReadOnly = false;
		this.lb_SupperUser.SettingInVisible = false;
		this.lb_SupperUser.SettingReadonly = false;
		this.lb_SupperUser.Size = new System.Drawing.Size(69, 13);
		this.lb_SupperUser.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SupperUser.TabIndex = 10;
		this.lb_SupperUser.Text = "Supper User";
		this.lb_SupperUser.Visible = false;
		this.btn_Copy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Copy.EditControl = null;
		this.btn_Copy.EditValue = "Copy Authorization";
		this.btn_Copy.Enabled = false;
		this.btn_Copy.FieldName = null;
		this.btn_Copy.IsSearchControl = false;
		this.btn_Copy.Location = new System.Drawing.Point(217, 386);
		this.btn_Copy.Name = "btn_Copy";
		this.btn_Copy.ReadOnly = false;
		this.btn_Copy.SettingInVisible = false;
		this.btn_Copy.SettingReadonly = false;
		this.btn_Copy.Size = new System.Drawing.Size(100, 20);
		this.btn_Copy.TabIndex = 11;
		this.btn_Copy.Text = "Copy Authorization";
		this.btn_Copy.Click += new System.EventHandler(this.btn_Copy_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1143, 412);
		base.Controls.Add(this.btn_Copy);
		base.Controls.Add(this.lb_SupperUser);
		base.Controls.Add(this.btn_SupperUser);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_NoAuthorization);
		base.Controls.Add(this.btn_ReadOnly);
		base.Controls.Add(this.btn_FullAuthorization);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oDataOwnerShipAuthorization";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DataOwnerShipAuthorization.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DataOwnerShipAuthorization";
		this.Text = "Data Ownership Authorization";
		base.Load += new System.EventHandler(this.frm_DataOwnerShipAuthorization_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Employess).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employees).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Object).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_authorization).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
