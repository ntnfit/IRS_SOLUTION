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

public class frm_HREmpPosition : XtraFormEx
{
	public RefreshLookupPos refreshPos;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	private GridControlEx grd_EmpPos;

	private GridViewEx gv_EmpPos;

	private GridColumnEx cl_posID;

	private GridColumnEx cl_name;

	private GridColumnEx cl_descriptio;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx cl_LocFields;

	private ContextMenuStrip contextMenuStrip_EmpPos;

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
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_HREmpPosition()
	{
		InitializeComponent();
		FormId = "oHREmpPosition";
		gv_EmpPos.FormId = FormId;
		gv_EmpPos.ItemId = "OHPS";
		OnLoadAsync += frm_HREmpPosition_Load;
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
				await SaveEmployeePosition();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		}
	}

	private void gv_EmpPos_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_EmpPos.RowCount; i++)
			{
				num = gv_EmpPos.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_EmpPos.GetRowCellValue(e.RowHandle, "Name"));
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

	private void gv_EmpPos_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_EmpPos_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_EmpPos.GetDataRow(gv_EmpPos.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
		}
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_EmpPos.GetFocusedRowCellValue("posID"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveEmployeePosition(num);
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

	private void AddNewRow()
	{
		try
		{
			gv_EmpPos.BeginInsertRow();
			DataTable dataTable = grd_EmpPos.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["posID"] = 0;
				dataRow["name"] = "";
				dataRow["descriptio"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_EmpPos.DataSource = dataTable;
				grd_EmpPos.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OHPS");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["posID"] = 0;
				dataRow2["name"] = "";
				dataRow2["descriptio"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_EmpPos.DataSource = tableStructure;
				grd_EmpPos.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_EmpPos.EndInsertRow();
		}
	}

	public async Task<DataTable> GetEmployeePosition()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await helper.GetAsync("GetEmpPos");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task SaveEmployeePosition()
	{
		try
		{
			DataTable dataTable = grd_EmpPos.DataSourceTable.Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["posID"]) == 0);
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
					HttpResult httpResult = _help.Post("SaveEmpPos", dataTable);
					if (string.IsNullOrWhiteSpace(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (refreshPos != null && !((XtraForm)refreshPos.Target).IsDisposed)
						{
							refreshPos();
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
				grd_EmpPos.DataSource = dataTable.Clone();
				grd_EmpPos.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void RemoveEmployeePosition(int ID)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = helper.Post("RemoveEmpPos", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_EmpPos.BeginUpdate();
				gv_EmpPos.DeleteSelectedRows();
				gv_EmpPos.EndUpdate();
				if (refreshPos != null && !((XtraForm)refreshPos.Target).IsDisposed)
				{
					refreshPos();
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

	public async Task RefreshData()
	{
		try
		{
			GridControlEx gridControlEx = grd_EmpPos;
			gridControlEx.DataSource = await GetEmployeePosition();
			grd_EmpPos.DataSourceTable.AcceptChanges();
			grd_EmpPos.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_HREmpPosition_Load(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HREmpPosition));
		this.grd_EmpPos = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_EmpPos = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_EmpPos = new Apzon.Controls.Grid.GridViewEx();
		this.cl_posID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_LocFields = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_EmpPos).BeginInit();
		this.contextMenuStrip_EmpPos.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_EmpPos).BeginInit();
		base.SuspendLayout();
		this.grd_EmpPos.AllowPasteExcel = true;
		this.grd_EmpPos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_EmpPos.ClipboardData = null;
		this.grd_EmpPos.ContextMenuStrip = this.contextMenuStrip_EmpPos;
		this.grd_EmpPos.Location = new System.Drawing.Point(3, 4);
		this.grd_EmpPos.MainView = this.gv_EmpPos;
		this.grd_EmpPos.Name = "grd_EmpPos";
		this.grd_EmpPos.Size = new System.Drawing.Size(571, 301);
		this.grd_EmpPos.TabIndex = 70;
		this.grd_EmpPos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_EmpPos });
		this.contextMenuStrip_EmpPos.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_EmpPos.Name = "contextMenuStrip_EmpPos";
		this.contextMenuStrip_EmpPos.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_EmpPos.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_EmpPos.ColumnKey = null;
		this.gv_EmpPos.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_posID, this.cl_name, this.cl_descriptio, this.cl_LocFields });
		this.gv_EmpPos.DisableFormSetting = false;
		this.gv_EmpPos.DisplayException = false;
		this.gv_EmpPos.EditColumn = null;
		this.gv_EmpPos.FieldCurrency = null;
		this.gv_EmpPos.FieldRate = null;
		this.gv_EmpPos.FormId = "oHREmpPosition";
		this.gv_EmpPos.GridControl = this.grd_EmpPos;
		this.gv_EmpPos.IgnoreFilter = false;
		this.gv_EmpPos.IndexEditRow = -1;
		this.gv_EmpPos.IndicatorWidth = 30;
		this.gv_EmpPos.IsApzTable = false;
		this.gv_EmpPos.IsInsertNewRow = false;
		this.gv_EmpPos.IsLoadingData = false;
		this.gv_EmpPos.IsMappingContextAction = false;
		this.gv_EmpPos.ItemId = "OHPS";
		this.gv_EmpPos.LstColumnAllowBatch = null;
		this.gv_EmpPos.LstColumnNotAllowBatch = null;
		this.gv_EmpPos.LstColumnNotEditable = null;
		this.gv_EmpPos.LstColumNotSetting = null;
		this.gv_EmpPos.Name = "gv_EmpPos";
		this.gv_EmpPos.OptionsView.RowAutoHeight = true;
		this.gv_EmpPos.OptionsView.ShowGroupPanel = false;
		this.gv_EmpPos.UseBatchMod = false;
		this.gv_EmpPos.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_EmpPos_CustomDrawRowIndicator);
		this.gv_EmpPos.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_EmpPos_CellValueChanged);
		this.gv_EmpPos.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_EmpPos_MouseUp);
		this.cl_posID.Caption = "posID";
		this.cl_posID.CurrencyDateField = null;
		this.cl_posID.CurrencyField = null;
		this.cl_posID.CurrencyFixed = null;
		this.cl_posID.CurrencyFixedRate = 0.0;
		this.cl_posID.CurrencyRateField = null;
		this.cl_posID.FieldName = "posID";
		this.cl_posID.Name = "cl_posID";
		this.cl_posID.OptionsColumn.ReadOnly = true;
		this.cl_posID.PrimaryKey = false;
		this.cl_posID.UnitField = null;
		this.cl_name.Caption = "Position Name";
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
		this.cl_LocFields.Caption = "LocFields";
		this.cl_LocFields.CurrencyDateField = null;
		this.cl_LocFields.CurrencyField = null;
		this.cl_LocFields.CurrencyFixed = null;
		this.cl_LocFields.CurrencyFixedRate = 0.0;
		this.cl_LocFields.CurrencyRateField = null;
		this.cl_LocFields.FieldName = "LocFields";
		this.cl_LocFields.Name = "cl_LocFields";
		this.cl_LocFields.OptionsColumn.ReadOnly = true;
		this.cl_LocFields.PrimaryKey = false;
		this.cl_LocFields.UnitField = null;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 311);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 73;
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
		this.btnCancel.Location = new System.Drawing.Point(70, 311);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 72;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(579, 337);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_EmpPos);
		base.FormId = "oHREmpPosition";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HREmpPosition";
		this.Text = "Employee Position - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_EmpPos).EndInit();
		this.contextMenuStrip_EmpPos.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_EmpPos).EndInit();
		base.ResumeLayout(false);
	}
}
