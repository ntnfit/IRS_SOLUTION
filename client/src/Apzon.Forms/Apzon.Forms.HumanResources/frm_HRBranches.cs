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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_HRBranches : XtraFormEx
{
	public RefreshLookupBranches refeshBranches;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_Branches;

	private GridViewEx gv_Branches;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_Remarks;

	private GridColumnEx cl_UserSign;

	private ContextMenuStrip contextMenuStrip_Branches;

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

	public frm_HRBranches()
	{
		InitializeComponent();
		FormId = "oHRBranches";
		gv_Branches.FormId = FormId;
		gv_Branches.ItemId = "OUBR";
		OnLoadAsync += frm_HRBranches_Load;
	}

	private async Task frm_HRBranches_Load(object sender, EventArgs e)
	{
		try
		{
			await Refresher();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
				await SaveBranches();
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

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Branches.GetFocusedRowCellValue("Code"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveBranches(num);
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
			await Refresher();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void gv_Branches_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_Branches.RowCount; i++)
			{
				num = gv_Branches.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_Branches.GetRowCellValue(e.RowHandle, "name"));
			if (e.Column.FieldName == "name" && text != "" && e.RowHandle == num)
			{
				AddNewRow();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Branches_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Branches_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_Branches.GetDataRow(gv_Branches.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Branches.BeginInsertRow();
			DataTable dataTable = grd_Branches.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = 0;
				dataRow["Name"] = "";
				dataRow["Remarks"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Branches.DataSource = dataTable;
				grd_Branches.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OUBR");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["code"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remarks"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_Branches.DataSource = tableStructure;
				grd_Branches.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Branches.EndInsertRow();
		}
	}

	public async Task SaveBranches()
	{
		try
		{
			DataTable dataTable = (grd_Branches.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["code"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["Remarks"])))
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
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).Length > 20) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						if (dataTable.Rows[num]["UserSign"] == DBNull.Value)
						{
							dataTable.Rows[num]["UserSign"] = GlobalData.UserID;
						}
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _help.Post("SaveBranches", dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (refeshBranches != null && !((XtraForm)refeshBranches.Target).IsDisposed)
						{
							refeshBranches();
						}
						FormMode = Enums.FormViewModeType.View;
						await Refresher();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_Branches.DataSource = dataTable.Clone();
				grd_Branches.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void RemoveBranches(int ID)
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = _helper.Post("RemoveBranches", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Branches.BeginUpdate();
				gv_Branches.DeleteSelectedRows();
				gv_Branches.EndUpdate();
				await Refresher();
				if (refeshBranches != null && !((XtraForm)refeshBranches.Target).IsDisposed)
				{
					refeshBranches();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task Refresher()
	{
		try
		{
			GridControlEx gridControlEx = grd_Branches;
			gridControlEx.DataSource = await GetBranches();
			grd_Branches.DataSourceTable.AcceptChanges();
			grd_Branches.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetBranches()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await _help.GetAsync("GetBranches");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await Refresher();
			FormMode = Enums.FormViewModeType.View;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRBranches));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Branches = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Branches = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Branches = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Branches).BeginInit();
		this.contextMenuStrip_Branches.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Branches).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 301);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 74;
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
		this.btnCancel.Location = new System.Drawing.Point(69, 301);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 73;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Branches.AllowPasteExcel = true;
		this.grd_Branches.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Branches.ClipboardData = null;
		this.grd_Branches.ContextMenuStrip = this.contextMenuStrip_Branches;
		this.grd_Branches.Location = new System.Drawing.Point(2, 3);
		this.grd_Branches.MainView = this.gv_Branches;
		this.grd_Branches.Name = "grd_Branches";
		this.grd_Branches.Size = new System.Drawing.Size(506, 292);
		this.grd_Branches.TabIndex = 72;
		this.grd_Branches.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Branches });
		this.contextMenuStrip_Branches.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_Branches.Name = "contextMenuStrip_Branches";
		this.contextMenuStrip_Branches.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_Branches.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Branches.ColumnKey = null;
		this.gv_Branches.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_Code, this.cl_Name, this.cl_Remarks, this.cl_UserSign });
		this.gv_Branches.DisableFormSetting = false;
		this.gv_Branches.DisplayException = false;
		this.gv_Branches.EditColumn = null;
		this.gv_Branches.FieldCurrency = null;
		this.gv_Branches.FieldRate = null;
		this.gv_Branches.FormId = "oHRBranches";
		this.gv_Branches.GridControl = this.grd_Branches;
		this.gv_Branches.IgnoreFilter = false;
		this.gv_Branches.IndexEditRow = -1;
		this.gv_Branches.IndicatorWidth = 30;
		this.gv_Branches.IsApzTable = false;
		this.gv_Branches.IsInsertNewRow = false;
		this.gv_Branches.IsLoadingData = false;
		this.gv_Branches.IsMappingContextAction = false;
		this.gv_Branches.ItemId = "OUBR";
		this.gv_Branches.LstColumnAllowBatch = null;
		this.gv_Branches.LstColumnNotAllowBatch = null;
		this.gv_Branches.LstColumnNotEditable = null;
		this.gv_Branches.LstColumNotSetting = null;
		this.gv_Branches.Name = "gv_Branches";
		this.gv_Branches.OptionsView.RowAutoHeight = true;
		this.gv_Branches.OptionsView.ShowGroupPanel = false;
		this.gv_Branches.UseBatchMod = false;
		this.gv_Branches.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Branches_CustomDrawRowIndicator);
		this.gv_Branches.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Branches_CellValueChanged);
		this.gv_Branches.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_Branches_MouseUp);
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
		this.cl_Name.Caption = "Branches Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 0;
		this.cl_Name.Width = 403;
		this.cl_Remarks.Caption = "Description";
		this.cl_Remarks.CurrencyDateField = null;
		this.cl_Remarks.CurrencyField = null;
		this.cl_Remarks.CurrencyFixed = null;
		this.cl_Remarks.CurrencyFixedRate = 0.0;
		this.cl_Remarks.CurrencyRateField = null;
		this.cl_Remarks.FieldName = "Remarks";
		this.cl_Remarks.Name = "cl_Remarks";
		this.cl_Remarks.PrimaryKey = false;
		this.cl_Remarks.UnitField = null;
		this.cl_Remarks.Visible = true;
		this.cl_Remarks.VisibleIndex = 1;
		this.cl_Remarks.Width = 661;
		this.cl_UserSign.Caption = "UserSign";
		this.cl_UserSign.CurrencyDateField = null;
		this.cl_UserSign.CurrencyField = null;
		this.cl_UserSign.CurrencyFixed = null;
		this.cl_UserSign.CurrencyFixedRate = 0.0;
		this.cl_UserSign.CurrencyRateField = null;
		this.cl_UserSign.FieldName = "UserSign";
		this.cl_UserSign.Name = "cl_UserSign";
		this.cl_UserSign.OptionsColumn.ReadOnly = true;
		this.cl_UserSign.PrimaryKey = false;
		this.cl_UserSign.UnitField = null;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(511, 323);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Branches);
		base.FormId = "oHRBranches";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRBranches";
		this.Text = "Branches - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_Branches).EndInit();
		this.contextMenuStrip_Branches.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Branches).EndInit();
		base.ResumeLayout(false);
	}
}
