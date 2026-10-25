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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_JobType : XtraFormEx
{
	private const string FormController = "HRMQualifications";

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_JobType;

	private GridViewEx gv_JobType;

	private GridColumnEx cl_Id;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remark;

	private ContextMenuStrip contextMenuStrip_Remove;

	private ToolStripMenuItem toolStripMenuItem_Remove;

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

	public frm_JobType()
	{
		InitializeComponent();
		OnLoadAsync += frm_JobType_Load;
		FormId = "oJobType";
		gv_JobType.FormId = FormId;
		gv_JobType.ItemId = "APZ_OHJT";
	}

	private async Task frm_JobType_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshJobType();
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
			int num = Function.ParseInt(gv_JobType.GetFocusedRowCellValue("Id"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveJobType(num);
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
				await SaveJobType();
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
		Close();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshJobType();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveJobType()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_JobType.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Id"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["Remark"])))
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
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "HRMQualifications");
					HttpResult httpResult = await _dtHelp.PostAsync("SaveJobType", dataTable);
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						await RefreshJobType();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_JobType.DataSource = dataTable.Clone();
				grd_JobType.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveJobType(int ID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "HRMQualifications");
			string text = hp.Post("RemoveJobType", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_JobType.BeginUpdate();
				gv_JobType.DeleteSelectedRows();
				gv_JobType.EndUpdate();
				await RefreshJobType();
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

	private async Task RefreshJobType()
	{
		try
		{
			GridControlEx gridControlEx = grd_JobType;
			gridControlEx.DataSource = await GetNewJobType();
			grd_JobType.DataSourceTable.AcceptChanges();
			grd_JobType.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRowJobType();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetNewJobType()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			return await helper.GetAsync("GetJobType");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void AddNewRowJobType()
	{
		try
		{
			gv_JobType.BeginInsertRow();
			DataTable dataTable = grd_JobType.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Id"] = 0;
				dataRow["Name"] = "";
				dataRow["Remark"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_JobType.DataSource = dataTable;
				grd_JobType.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHJT");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Id"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remark"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_JobType.DataSource = tableStructure;
				grd_JobType.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_JobType.EndInsertRow();
		}
	}

	private void gv_JobType_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_JobType.RowCount; i++)
			{
				num = gv_JobType.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_JobType.GetRowCellValue(e.RowHandle, "name"));
			if (e.Column.FieldName == "name" && text != "" && e.RowHandle == num)
			{
				AddNewRowJobType();
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

	private void gv_JobType_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_JobType_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_JobType.GetDataRow(gv_JobType.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
		}
	}

	private async void frm_JobType_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveJobType();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JobType));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_JobType = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Remove = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_JobType = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_JobType).BeginInit();
		this.contextMenuStrip_Remove.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_JobType).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 321);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 79;
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
		this.btnCancel.Location = new System.Drawing.Point(70, 321);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 78;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_JobType.AllowPasteExcel = true;
		this.grd_JobType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_JobType.ClipboardData = null;
		this.grd_JobType.ContextMenuStrip = this.contextMenuStrip_Remove;
		this.grd_JobType.Location = new System.Drawing.Point(3, 2);
		this.grd_JobType.MainView = this.gv_JobType;
		this.grd_JobType.Name = "grd_JobType";
		this.grd_JobType.Size = new System.Drawing.Size(516, 313);
		this.grd_JobType.TabIndex = 77;
		this.grd_JobType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_JobType });
		this.contextMenuStrip_Remove.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_Remove.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_Remove.Name = "contextMenuStrip_Remove";
		this.contextMenuStrip_Remove.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_JobType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_JobType.ColumnKey = null;
		this.gv_JobType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_Id, this.cl_name, this.cl_Remark });
		this.gv_JobType.DisableFormSetting = false;
		this.gv_JobType.DisplayException = false;
		this.gv_JobType.EditColumn = null;
		this.gv_JobType.FieldCurrency = null;
		this.gv_JobType.FieldRate = null;
		this.gv_JobType.FormId = "oJobType";
		this.gv_JobType.GridControl = this.grd_JobType;
		this.gv_JobType.IgnoreFilter = false;
		this.gv_JobType.IndexEditRow = -1;
		this.gv_JobType.IndicatorWidth = 30;
		this.gv_JobType.IsApzTable = false;
		this.gv_JobType.IsInsertNewRow = false;
		this.gv_JobType.IsLoadingData = false;
		this.gv_JobType.IsMappingContextAction = false;
		this.gv_JobType.ItemId = "APZ_OHJT";
		this.gv_JobType.LstColumnAllowBatch = null;
		this.gv_JobType.LstColumnNotAllowBatch = null;
		this.gv_JobType.LstColumnNotEditable = null;
		this.gv_JobType.LstColumNotSetting = null;
		this.gv_JobType.Name = "gv_JobType";
		this.gv_JobType.OptionsView.RowAutoHeight = true;
		this.gv_JobType.OptionsView.ShowGroupPanel = false;
		this.gv_JobType.UseBatchMod = false;
		this.gv_JobType.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_JobType_CustomDrawRowIndicator);
		this.gv_JobType.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_JobType_CellValueChanged);
		this.gv_JobType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_JobType_MouseUp);
		this.cl_Id.Caption = "ID";
		this.cl_Id.CurrencyDateField = null;
		this.cl_Id.CurrencyField = null;
		this.cl_Id.CurrencyFixed = null;
		this.cl_Id.CurrencyFixedRate = 0.0;
		this.cl_Id.CurrencyRateField = null;
		this.cl_Id.FieldName = "Id";
		this.cl_Id.Name = "cl_Id";
		this.cl_Id.OptionsColumn.ReadOnly = true;
		this.cl_Id.PrimaryKey = false;
		this.cl_Id.UnitField = null;
		this.cl_name.Caption = "Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "Name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = true;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 384;
		this.cl_Remark.Caption = "Description";
		this.cl_Remark.CurrencyDateField = null;
		this.cl_Remark.CurrencyField = null;
		this.cl_Remark.CurrencyFixed = null;
		this.cl_Remark.CurrencyFixedRate = 0.0;
		this.cl_Remark.CurrencyRateField = null;
		this.cl_Remark.FieldName = "Remark";
		this.cl_Remark.Name = "cl_Remark";
		this.cl_Remark.PrimaryKey = false;
		this.cl_Remark.UnitField = null;
		this.cl_Remark.Visible = true;
		this.cl_Remark.VisibleIndex = 1;
		this.cl_Remark.Width = 288;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(524, 344);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_JobType);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JobType";
		this.Text = "Job Types";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JobType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_JobType).EndInit();
		this.contextMenuStrip_Remove.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_JobType).EndInit();
		base.ResumeLayout(false);
	}
}
