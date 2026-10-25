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
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_AllowanceType : XtraFormEx
{
	private const string FormController = "AllowanceType";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_AllowanceType;

	private GridViewEx gv_AllowanceType;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remark;

	private ContextMenuStrip contextMenuStrip_AllowanceType;

	private ToolStripMenuItem toolStripMenuItemRemove;

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

	public frm_AllowanceType()
	{
		InitializeComponent();
		FormId = "oAllowanceType";
		gv_AllowanceType.FormId = FormId;
		gv_AllowanceType.ItemId = "APZ_OHAT";
		gv_AllowanceType.ChangeLogType = Enums.ChangeLog.clAllowanceType;
		gv_AllowanceType.IsApzTable = true;
		gv_AllowanceType.ShowingEditor += GvAllowanceTypeOnShowingEditor;
		OnLoadAsync += frm_AllowanceType_Load;
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ToString(gv_AllowanceType.GetFocusedRowCellValue("Code"));
				Enums.ChangeLog changeLog = Enums.ChangeLog.clAllowanceType;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLog;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void GvAllowanceTypeOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_AllowanceType.FocusedColumn.FieldName == "Code")
		{
			e.Cancel = true;
		}
	}

	private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_AllowanceType.GetFocusedRowCellValue("Code"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveAllowanceType(num);
			}
		}
		catch (Exception ex)
		{
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
				return;
			}
			WinCommons._OpenProcessing();
			btn_Save.Enabled = false;
			await SaveAllowanceType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_AllowanceType_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_AllowanceType.RowCount; i++)
			{
				num = gv_AllowanceType.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_AllowanceType.GetRowCellValue(e.RowHandle, "name"));
			if (e.Column.FieldName == "name" && text != "" && e.RowHandle == num)
			{
				AddNewAllowanceType();
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

	private void gv_AllowanceType_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void AddNewAllowanceType()
	{
		try
		{
			gv_AllowanceType.BeginInsertRow();
			DataTable dataTable = grd_AllowanceType.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = 0;
				dataRow["Name"] = "";
				dataRow["Remarks"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_AllowanceType.DataSource = dataTable;
				grd_AllowanceType.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHAT");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remarks"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_AllowanceType.DataSource = tableStructure;
				grd_AllowanceType.RefreshDataSource();
			}
			gv_AllowanceType.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_AllowanceType.EndInsertRow();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshAllowanceType();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveAllowanceType()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_AllowanceType.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["code"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Warning(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
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
						WinCommons.Warning(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						DataRow dataRow2 = dataTable.Rows[num];
						dataRow2["UserSign2"] = GlobalData.UserID;
						dataRow2["UpdateDate"] = DateTime.Today;
					}
					dataTable.AcceptChanges();
					HttpResult httpResult = await _httpClient.AllowanceType.SaveAllowanceType(dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						await RefreshAllowanceType();
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_AllowanceType.DataSource = dataTable.Clone();
				grd_AllowanceType.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveAllowanceType(int code)
	{
		try
		{
			string text = await _httpClient.AllowanceType.RemoveAllowanceType(code);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_AllowanceType.BeginUpdate();
				gv_AllowanceType.DeleteSelectedRows();
				gv_AllowanceType.EndUpdate();
				grd_AllowanceType.DataSourceTable.AcceptChanges();
				grd_AllowanceType.RefreshDataSource();
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

	private async Task RefreshAllowanceType()
	{
		try
		{
			GridControlEx gridControlEx = grd_AllowanceType;
			gridControlEx.DataSource = await GetNewAllowanceType();
			grd_AllowanceType.DataSourceTable.AcceptChanges();
			grd_AllowanceType.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewAllowanceType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetNewAllowanceType()
	{
		try
		{
			return await _httpClient.AllowanceType.GetAllowanceType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task frm_AllowanceType_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await RefreshAllowanceType();
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_AllowanceType, "APZ_OHAT"));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_AllowanceType));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_AllowanceType = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_AllowanceType = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_AllowanceType = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_AllowanceType).BeginInit();
		this.contextMenuStrip_AllowanceType.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_AllowanceType).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 299);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 82;
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
		this.btnCancel.Location = new System.Drawing.Point(100, 299);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 81;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_AllowanceType.AllowPasteExcel = true;
		this.grd_AllowanceType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_AllowanceType.ClipboardData = null;
		this.grd_AllowanceType.ContextMenuStrip = this.contextMenuStrip_AllowanceType;
		this.grd_AllowanceType.Location = new System.Drawing.Point(3, 3);
		this.grd_AllowanceType.MainView = this.gv_AllowanceType;
		this.grd_AllowanceType.Name = "grd_AllowanceType";
		this.grd_AllowanceType.Size = new System.Drawing.Size(541, 290);
		this.grd_AllowanceType.TabIndex = 80;
		this.grd_AllowanceType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_AllowanceType });
		this.contextMenuStrip_AllowanceType.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.contextMenuStrip_AllowanceType.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemRemove });
		this.contextMenuStrip_AllowanceType.Name = "contextMenuStrip_AllowanceType";
		this.contextMenuStrip_AllowanceType.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
		this.toolStripMenuItemRemove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItemRemove.Text = "Remove";
		this.toolStripMenuItemRemove.Click += new System.EventHandler(this.toolStripMenuItemRemove_Click);
		this.gv_AllowanceType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_AllowanceType.ColumnKey = null;
		this.gv_AllowanceType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_Code, this.cl_name, this.cl_Remark });
		this.gv_AllowanceType.DisableFormSetting = false;
		this.gv_AllowanceType.DisplayException = false;
		this.gv_AllowanceType.EditColumn = null;
		this.gv_AllowanceType.FieldCurrency = null;
		this.gv_AllowanceType.FieldRate = null;
		this.gv_AllowanceType.FormId = "oAllowanceType";
		this.gv_AllowanceType.GridControl = this.grd_AllowanceType;
		this.gv_AllowanceType.IgnoreFilter = false;
		this.gv_AllowanceType.IndexEditRow = -1;
		this.gv_AllowanceType.IndicatorWidth = 30;
		this.gv_AllowanceType.IsApzTable = false;
		this.gv_AllowanceType.IsInsertNewRow = false;
		this.gv_AllowanceType.IsLoadingData = false;
		this.gv_AllowanceType.IsMappingContextAction = false;
		this.gv_AllowanceType.ItemId = "APZ_OHAT";
		this.gv_AllowanceType.LevelIndent = 0;
		this.gv_AllowanceType.LstColumnAllowBatch = null;
		this.gv_AllowanceType.LstColumnNotAllowBatch = null;
		this.gv_AllowanceType.LstColumnNotEditable = null;
		this.gv_AllowanceType.LstColumNotSetting = null;
		this.gv_AllowanceType.Name = "gv_AllowanceType";
		this.gv_AllowanceType.OptionsView.RowAutoHeight = true;
		this.gv_AllowanceType.OptionsView.ShowGroupPanel = false;
		this.gv_AllowanceType.PreviewIndent = 0;
		this.gv_AllowanceType.UseBatchMod = false;
		this.gv_AllowanceType.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_AllowanceType_CustomDrawRowIndicator);
		this.gv_AllowanceType.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_AllowanceType_CellValueChanged);
		this.cl_Code.Caption = "Code";
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.OptionsColumn.ReadOnly = true;
		this.cl_Code.PrimaryKey = false;
		this.cl_Code.UnitField = null;
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
		this.cl_name.Width = 182;
		this.cl_Remark.Caption = "Description";
		this.cl_Remark.CurrencyDateField = null;
		this.cl_Remark.CurrencyField = null;
		this.cl_Remark.CurrencyFixed = null;
		this.cl_Remark.CurrencyFixedRate = 0.0;
		this.cl_Remark.CurrencyRateField = null;
		this.cl_Remark.FieldName = "Remarks";
		this.cl_Remark.Name = "cl_Remark";
		this.cl_Remark.PrimaryKey = false;
		this.cl_Remark.UnitField = null;
		this.cl_Remark.Visible = true;
		this.cl_Remark.VisibleIndex = 1;
		this.cl_Remark.Width = 309;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(547, 325);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_AllowanceType);
		base.FormId = "oAllowanceType";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AllowanceType";
		this.Text = "Allowance Type";
		((System.ComponentModel.ISupportInitialize)this.grd_AllowanceType).EndInit();
		this.contextMenuStrip_AllowanceType.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_AllowanceType).EndInit();
		base.ResumeLayout(false);
	}
}
