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

public class frm_HREmpStatus : XtraFormEx
{
	public RefreshLookupStatus refreshStatus;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	private GridControlEx grd_EmpStatus;

	private GridViewEx gv_EmpStatus;

	private GridColumnEx cl_statusID;

	private GridColumnEx cl_name;

	private GridColumnEx cl_descriptio;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip_EmpStatus;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private ToolStripMenuItem toolStripMenuItem_Refresh;

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
			btn_Save.Enabled = true;
			btnCancel.Enabled = true;
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

	public frm_HREmpStatus()
	{
		InitializeComponent();
		OnLoadAsync += frm_HREmpStatus_Load;
		FormId = "oHREmpStatus";
		gv_EmpStatus.FormId = FormId;
		gv_EmpStatus.ItemId = "OHST";
	}

	private async Task frm_HREmpStatus_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshStatus();
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
			int num = Function.ParseInt(gv_EmpStatus.GetFocusedRowCellValue("statusID"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveStatus(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void toolStripMenuItem_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await RefreshStatus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void AddNewRow()
	{
		try
		{
			gv_EmpStatus.BeginInsertRow();
			DataTable dataTable = grd_EmpStatus.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["statusID"] = 0;
				dataRow["name"] = "";
				dataRow["descriptio"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_EmpStatus.DataSource = dataTable;
				grd_EmpStatus.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OHST");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["statusID"] = 0;
				dataRow2["name"] = "";
				dataRow2["descriptio"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_EmpStatus.DataSource = tableStructure;
				grd_EmpStatus.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_EmpStatus.EndInsertRow();
		}
	}

	private void gv_EmpStatus_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_EmpStatus.RowCount; i++)
			{
				num = gv_EmpStatus.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_EmpStatus.GetRowCellValue(e.RowHandle, "Name"));
			if (e.Column.FieldName == "Name" && text != "" && e.RowHandle == num)
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

	private void gv_EmpStatus_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_EmpStatus_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_EmpStatus.GetDataRow(gv_EmpStatus.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
		}
	}

	public async void RemoveStatus(int ID)
	{
		try
		{
			using HttpClientHelper<string> help = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = help.Post("RemoveEmpStatus", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_EmpStatus.DeleteSelectedRows();
				if (refreshStatus != null && !((XtraForm)refreshStatus.Target).IsDisposed)
				{
					refreshStatus();
				}
				await RefreshStatus();
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

	private async Task SaveStatus()
	{
		try
		{
			DataTable dataTable = (grd_EmpStatus.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["statusID"]) == 0);
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
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _dtHelp.Post("SaveEmpStatus", dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (refreshStatus != null && !((XtraForm)refreshStatus.Target).IsDisposed)
						{
							refreshStatus();
						}
						FormMode = Enums.FormViewModeType.View;
						await RefreshStatus();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_EmpStatus.DataSource = dataTable.Clone();
				grd_EmpStatus.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				await SaveStatus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetStatus()
	{
		try
		{
			using HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await help.GetAsync("GetEmpStatus");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task RefreshStatus()
	{
		try
		{
			GridControlEx gridControlEx = grd_EmpStatus;
			gridControlEx.DataSource = await GetStatus();
			grd_EmpStatus.DataSourceTable.AcceptChanges();
			grd_EmpStatus.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HREmpStatus));
		this.grd_EmpStatus = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_EmpStatus = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_EmpStatus = new Apzon.Controls.Grid.GridViewEx();
		this.cl_statusID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_EmpStatus).BeginInit();
		this.contextMenuStrip_EmpStatus.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_EmpStatus).BeginInit();
		base.SuspendLayout();
		this.grd_EmpStatus.AllowPasteExcel = true;
		this.grd_EmpStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_EmpStatus.ClipboardData = null;
		this.grd_EmpStatus.ContextMenuStrip = this.contextMenuStrip_EmpStatus;
		this.grd_EmpStatus.Location = new System.Drawing.Point(5, 3);
		this.grd_EmpStatus.MainView = this.gv_EmpStatus;
		this.grd_EmpStatus.Name = "grd_EmpStatus";
		this.grd_EmpStatus.Size = new System.Drawing.Size(558, 360);
		this.grd_EmpStatus.TabIndex = 71;
		this.grd_EmpStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_EmpStatus });
		this.contextMenuStrip_EmpStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_EmpStatus.Name = "contextMenuStrip_EmpStatus";
		this.contextMenuStrip_EmpStatus.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_EmpStatus.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_EmpStatus.ColumnKey = null;
		this.gv_EmpStatus.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_statusID, this.cl_name, this.cl_descriptio });
		this.gv_EmpStatus.DisableFormSetting = false;
		this.gv_EmpStatus.DisplayException = false;
		this.gv_EmpStatus.EditColumn = null;
		this.gv_EmpStatus.FieldCurrency = null;
		this.gv_EmpStatus.FieldRate = null;
		this.gv_EmpStatus.FormId = "oEmployeeStatusSetup";
		this.gv_EmpStatus.GridControl = this.grd_EmpStatus;
		this.gv_EmpStatus.IgnoreFilter = false;
		this.gv_EmpStatus.IndexEditRow = -1;
		this.gv_EmpStatus.IndicatorWidth = 30;
		this.gv_EmpStatus.IsApzTable = false;
		this.gv_EmpStatus.IsInsertNewRow = false;
		this.gv_EmpStatus.IsLoadingData = false;
		this.gv_EmpStatus.IsMappingContextAction = false;
		this.gv_EmpStatus.ItemId = "OESS";
		this.gv_EmpStatus.LstColumnAllowBatch = null;
		this.gv_EmpStatus.LstColumnNotAllowBatch = null;
		this.gv_EmpStatus.LstColumnNotEditable = null;
		this.gv_EmpStatus.LstColumNotSetting = null;
		this.gv_EmpStatus.Name = "gv_EmpStatus";
		this.gv_EmpStatus.OptionsView.RowAutoHeight = true;
		this.gv_EmpStatus.OptionsView.ShowGroupPanel = false;
		this.gv_EmpStatus.UseBatchMod = false;
		this.gv_EmpStatus.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_EmpStatus_CustomDrawRowIndicator);
		this.gv_EmpStatus.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_EmpStatus_CellValueChanged);
		this.gv_EmpStatus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_EmpStatus_MouseUp);
		this.cl_statusID.Caption = "statusID";
		this.cl_statusID.CurrencyDateField = null;
		this.cl_statusID.CurrencyField = null;
		this.cl_statusID.CurrencyFixed = null;
		this.cl_statusID.CurrencyFixedRate = 0.0;
		this.cl_statusID.CurrencyRateField = null;
		this.cl_statusID.FieldName = "statusID";
		this.cl_statusID.Name = "cl_statusID";
		this.cl_statusID.OptionsColumn.ReadOnly = true;
		this.cl_statusID.PrimaryKey = false;
		this.cl_statusID.UnitField = null;
		this.cl_name.Caption = "Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = false;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 403;
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
		this.cl_descriptio.Width = 661;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 369);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 75;
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
		this.btnCancel.Location = new System.Drawing.Point(72, 369);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 74;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(569, 394);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_EmpStatus);
		base.FormId = "oEmployeeStatusSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HREmpStatus";
		this.Text = "Employee Statuses - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_EmpStatus).EndInit();
		this.contextMenuStrip_EmpStatus.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_EmpStatus).EndInit();
		base.ResumeLayout(false);
	}
}
