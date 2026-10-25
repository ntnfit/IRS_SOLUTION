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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_HRRolesSetup : XtraFormEx
{
	public RefreshLookupRole RefreshRole;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private const string FormController = "States";

	private IContainer components;

	private ContextMenuStrip contextMenuStrip_Roles;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private ToolStripMenuItem toolStripMenuItem_Refresh;

	private GridControlEx grd_Roles;

	private GridViewEx gv_Roles;

	private GridColumnEx cl_typeID;

	private GridColumnEx cl_name;

	private GridColumnEx cl_descriptio;

	private GridColumnEx cl_locked;

	private PanelControl panelControl1;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

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
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Busy:
			btn_Save.Enabled = false;
			btnCancel.Enabled = false;
			break;
		case Enums.FormViewModeType.Add:
			break;
		}
	}

	public frm_HRRolesSetup()
	{
		InitializeComponent();
		FormId = "oHRRolesSetup";
		gv_Roles.FormId = FormId;
		gv_Roles.ItemId = "OHTY";
		OnLoadAsync += frm_HRRolesSetup_Load;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task frm_HRRolesSetup_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshData()
	{
		try
		{
			GridControlEx gridControlEx = grd_Roles;
			gridControlEx.DataSource = await GetNewRoles();
			grd_Roles.DataSourceTable.AcceptChanges();
			grd_Roles.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetNewRoles()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await helper.GetAsync("GetNewRoles");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void gv_Roles_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Roles.BeginInsertRow();
			DataTable dataTable = grd_Roles.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (dataTable.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["name"]))))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["typeID"] = 0;
					dataRow["name"] = "";
					dataRow["descriptio"] = "";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Roles.DataSource = dataTable;
					grd_Roles.RefreshDataSource();
				}
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OHTY");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["typeID"] = 0;
				dataRow2["name"] = "";
				dataRow2["descriptio"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_Roles.DataSource = tableStructure;
				grd_Roles.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Roles.EndInsertRow();
		}
	}

	private void gv_Roles_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			for (int i = 0; i < gv_Roles.RowCount; i++)
			{
				gv_Roles.GetVisibleRowHandle(i);
			}
			if (e.Column.FieldName == "Name")
			{
				AddNewRow();
			}
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

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Roles.GetFocusedRowCellValue("locked")).Equals("Y"))
			{
				WinCommons.Error(Enums.MessageCode.CannotRemoveSystemData, "");
			}
			int num = Function.ParseInt(gv_Roles.GetFocusedRowCellValue("typeID"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveRoles(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void RemoveRoles(int objID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = hp.Post("RemoveRoles", objID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Roles.BeginUpdate();
				gv_Roles.DeleteSelectedRows();
				gv_Roles.EndUpdate();
				if (RefreshRole != null && !((XtraForm)RefreshRole.Target).IsDisposed)
				{
					RefreshRole();
				}
				await RefreshData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Roles_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Roles.GetFocusedRowCellValue("locked")).Equals("Y"))
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			WinCommons._OpenProcessing();
			btn_Save.Enabled = false;
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async Task Save()
	{
		try
		{
			DataTable dataTable = (grd_Roles.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["typeID"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["descriptio"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					dataTable = dataTable.Copy();
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["name"]).Length > 20) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _help.Post("SaveNewRoles", dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (RefreshRole != null && !((XtraForm)RefreshRole.Target).IsDisposed)
						{
							RefreshRole();
						}
						FormMode = Enums.FormViewModeType.View;
						await RefreshData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_Roles.DataSource = dataTable.Clone();
				grd_Roles.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void toolStripMenuItem_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async void toolStripMenuItem_Add_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await AddNewRowRole();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private async Task AddNewRowRole()
	{
		try
		{
			DataTable dataSourceTable = grd_Roles.DataSourceTable;
			dataSourceTable.Rows.Clear();
			dataSourceTable.AcceptChanges();
			dataSourceTable.Clear();
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["name"] = "";
			dataRow["descriptio"] = "";
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grd_Roles.DataSource = dataSourceTable;
			grd_Roles.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Roles_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_Roles.GetDataRow(gv_Roles.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRRolesSetup));
		this.contextMenuStrip_Roles = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.grd_Roles = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Roles = new Apzon.Controls.Grid.GridViewEx();
		this.cl_typeID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_locked = new Apzon.Controls.Grid.GridColumnEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip_Roles.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Roles).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Roles).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		base.SuspendLayout();
		this.contextMenuStrip_Roles.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_Roles.Name = "contextMenuStrip_Roles";
		this.contextMenuStrip_Roles.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "R&emove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.grd_Roles.AllowPasteExcel = true;
		this.grd_Roles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Roles.ClipboardData = null;
		this.grd_Roles.ContextMenuStrip = this.contextMenuStrip_Roles;
		this.grd_Roles.Location = new System.Drawing.Point(0, 0);
		this.grd_Roles.MainView = this.gv_Roles;
		this.grd_Roles.Name = "grd_Roles";
		this.grd_Roles.Size = new System.Drawing.Size(512, 278);
		this.grd_Roles.TabIndex = 66;
		this.grd_Roles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Roles });
		this.gv_Roles.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Roles.ColumnKey = null;
		this.gv_Roles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_typeID, this.cl_name, this.cl_descriptio, this.cl_locked });
		this.gv_Roles.DisableFormSetting = false;
		this.gv_Roles.DisplayException = false;
		this.gv_Roles.EditColumn = null;
		this.gv_Roles.FieldCurrency = null;
		this.gv_Roles.FieldRate = null;
		this.gv_Roles.FormId = "oHRRolesSetup";
		this.gv_Roles.GridControl = this.grd_Roles;
		this.gv_Roles.IgnoreFilter = false;
		this.gv_Roles.IndexEditRow = -1;
		this.gv_Roles.IndicatorWidth = 30;
		this.gv_Roles.IsApzTable = false;
		this.gv_Roles.IsInsertNewRow = false;
		this.gv_Roles.IsLoadingData = false;
		this.gv_Roles.IsMappingContextAction = false;
		this.gv_Roles.ItemId = "OHTY";
		this.gv_Roles.LstColumnAllowBatch = null;
		this.gv_Roles.LstColumnNotAllowBatch = null;
		this.gv_Roles.LstColumnNotEditable = null;
		this.gv_Roles.LstColumNotSetting = null;
		this.gv_Roles.Name = "gv_Roles";
		this.gv_Roles.OptionsView.RowAutoHeight = true;
		this.gv_Roles.OptionsView.ShowGroupPanel = false;
		this.gv_Roles.UseBatchMod = false;
		this.gv_Roles.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Roles_CustomDrawRowIndicator);
		this.gv_Roles.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Roles_ShowingEditor);
		this.gv_Roles.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Roles_CellValueChanged);
		this.gv_Roles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_Roles_MouseUp);
		this.cl_typeID.Caption = "typeID";
		this.cl_typeID.CurrencyDateField = null;
		this.cl_typeID.CurrencyField = null;
		this.cl_typeID.CurrencyFixed = null;
		this.cl_typeID.CurrencyFixedRate = 0.0;
		this.cl_typeID.CurrencyRateField = null;
		this.cl_typeID.FieldName = "typeID";
		this.cl_typeID.Name = "cl_typeID";
		this.cl_typeID.OptionsColumn.ReadOnly = true;
		this.cl_typeID.PrimaryKey = false;
		this.cl_typeID.UnitField = null;
		this.cl_name.Caption = "Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = true;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_descriptio.Caption = "Description";
		this.cl_descriptio.CurrencyDateField = null;
		this.cl_descriptio.CurrencyField = null;
		this.cl_descriptio.CurrencyFixed = null;
		this.cl_descriptio.CurrencyFixedRate = 0.0;
		this.cl_descriptio.CurrencyRateField = null;
		this.cl_descriptio.FieldName = "descriptio";
		this.cl_descriptio.Name = "cl_descriptio";
		this.cl_descriptio.PrimaryKey = false;
		this.cl_descriptio.UnitField = null;
		this.cl_descriptio.Visible = true;
		this.cl_descriptio.VisibleIndex = 1;
		this.cl_locked.Caption = "locked";
		this.cl_locked.CurrencyDateField = null;
		this.cl_locked.CurrencyField = null;
		this.cl_locked.CurrencyFixed = null;
		this.cl_locked.CurrencyFixedRate = 0.0;
		this.cl_locked.CurrencyRateField = null;
		this.cl_locked.FieldName = "locked";
		this.cl_locked.Name = "cl_locked";
		this.cl_locked.OptionsColumn.ReadOnly = true;
		this.cl_locked.PrimaryKey = false;
		this.cl_locked.UnitField = null;
		this.panelControl1.Controls.Add(this.btn_Save);
		this.panelControl1.Controls.Add(this.btnCancel);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(512, 309);
		this.panelControl1.TabIndex = 65;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 284);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 61;
		this.btn_Save.Text = "OK";
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
		this.btnCancel.Location = new System.Drawing.Point(103, 284);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 60;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(512, 309);
		base.Controls.Add(this.grd_Roles);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oHRRolesSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRRolesSetup";
		this.Text = "Roles - Setup";
		this.contextMenuStrip_Roles.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Roles).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Roles).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
