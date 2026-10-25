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

public class frm_HRDepartment : XtraFormEx
{
	public RefreshLookupDepartment refreshDepartment;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_Depart;

	private GridViewEx gv_Depart;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remarks;

	private GridColumnEx cl_UserSign;

	private GridColumnEx cl_Father;

	private ContextMenuStrip contextMenuStrip_Department;

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

	public frm_HRDepartment()
	{
		InitializeComponent();
		FormId = "oHRDepartment";
		gv_Depart.FormId = FormId;
		gv_Depart.ItemId = "OUDP";
		OnLoadAsync += frm_HRDepartment_Load;
	}

	private async Task frm_HRDepartment_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshDepart();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
				await SaveDepartment();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshDepart();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveDepartment()
	{
		try
		{
			DataTable dataTable = (grd_Depart.DataSource as DataTable).Copy();
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
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Name"]).Length > 20) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
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
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _dtHelp.Post("SaveDepartment", dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (refreshDepartment != null && !((XtraForm)refreshDepartment.Target).IsDisposed)
						{
							refreshDepartment();
						}
						FormMode = Enums.FormViewModeType.View;
						await RefreshDepart();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_Depart.DataSource = dataTable.Clone();
				grd_Depart.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveDepartment(int objID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = hp.Post("RemoveDepartment", objID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Depart.BeginUpdate();
				gv_Depart.DeleteSelectedRows();
				gv_Depart.EndUpdate();
				if (refreshDepartment != null && !((XtraForm)refreshDepartment.Target).IsDisposed)
				{
					refreshDepartment();
				}
				await RefreshDepart();
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

	private async Task RefreshDepart()
	{
		try
		{
			GridControlEx gridControlEx = grd_Depart;
			gridControlEx.DataSource = await GetNewDepartment();
			grd_Depart.DataSourceTable.AcceptChanges();
			grd_Depart.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetNewDepartment()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await helper.GetAsync("GetDepartment");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void AddNewRow()
	{
		try
		{
			gv_Depart.BeginInsertRow();
			DataTable dataTable = grd_Depart.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["code"] = 0;
				dataRow["Name"] = "";
				dataRow["Remarks"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Depart.DataSource = dataTable;
				grd_Depart.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OUDP");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["code"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remarks"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_Depart.DataSource = tableStructure;
				grd_Depart.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Depart.EndInsertRow();
		}
	}

	private void gv_Depart_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_Depart.RowCount; i++)
			{
				num = gv_Depart.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_Depart.GetRowCellValue(e.RowHandle, "name"));
			if (e.Column.FieldName == "name" && text != "" && e.RowHandle == num)
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

	private void gv_Depart_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Depart_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_Depart.GetDataRow(gv_Depart.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
		}
	}

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Depart.GetFocusedRowCellValue("Code"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveDepartment(num);
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
			await RefreshDepart();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRDepartment));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Depart = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Department = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Depart = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Father = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Depart).BeginInit();
		this.contextMenuStrip_Department.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Depart).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 307);
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
		this.btnCancel.Location = new System.Drawing.Point(71, 307);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 73;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Depart.AllowPasteExcel = true;
		this.grd_Depart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Depart.ClipboardData = null;
		this.grd_Depart.ContextMenuStrip = this.contextMenuStrip_Department;
		this.grd_Depart.Location = new System.Drawing.Point(4, 3);
		this.grd_Depart.MainView = this.gv_Depart;
		this.grd_Depart.Name = "grd_Depart";
		this.grd_Depart.Size = new System.Drawing.Size(590, 298);
		this.grd_Depart.TabIndex = 72;
		this.grd_Depart.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Depart });
		this.contextMenuStrip_Department.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_Department.Name = "contextMenuStrip_Department";
		this.contextMenuStrip_Department.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_Depart.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Depart.ColumnKey = null;
		this.gv_Depart.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.cl_Code, this.cl_name, this.cl_Remarks, this.cl_UserSign, this.cl_Father });
		this.gv_Depart.DisableFormSetting = false;
		this.gv_Depart.DisplayException = false;
		this.gv_Depart.EditColumn = null;
		this.gv_Depart.FieldCurrency = null;
		this.gv_Depart.FieldRate = null;
		this.gv_Depart.FormId = "oHRDepartment";
		this.gv_Depart.GridControl = this.grd_Depart;
		this.gv_Depart.IgnoreFilter = false;
		this.gv_Depart.IndexEditRow = -1;
		this.gv_Depart.IndicatorWidth = 30;
		this.gv_Depart.IsApzTable = false;
		this.gv_Depart.IsInsertNewRow = false;
		this.gv_Depart.IsLoadingData = false;
		this.gv_Depart.IsMappingContextAction = false;
		this.gv_Depart.ItemId = "OUDP";
		this.gv_Depart.LstColumnAllowBatch = null;
		this.gv_Depart.LstColumnNotAllowBatch = null;
		this.gv_Depart.LstColumnNotEditable = null;
		this.gv_Depart.LstColumNotSetting = null;
		this.gv_Depart.Name = "gv_Depart";
		this.gv_Depart.OptionsView.RowAutoHeight = true;
		this.gv_Depart.OptionsView.ShowGroupPanel = false;
		this.gv_Depart.UseBatchMod = false;
		this.gv_Depart.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Depart_CustomDrawRowIndicator);
		this.gv_Depart.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Depart_CellValueChanged);
		this.gv_Depart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_Depart_MouseUp);
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
		this.cl_name.Caption = "Departments Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "Name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = false;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 384;
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
		this.cl_Remarks.Width = 288;
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
		this.cl_Father.Caption = "Father";
		this.cl_Father.CurrencyDateField = null;
		this.cl_Father.CurrencyField = null;
		this.cl_Father.CurrencyFixed = null;
		this.cl_Father.CurrencyFixedRate = 0.0;
		this.cl_Father.CurrencyRateField = null;
		this.cl_Father.FieldName = "Father";
		this.cl_Father.Name = "cl_Father";
		this.cl_Father.OptionsColumn.ReadOnly = true;
		this.cl_Father.PrimaryKey = false;
		this.cl_Father.UnitField = null;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(598, 329);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Depart);
		base.FormId = "oHRDepartment";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRDepartment";
		this.Text = "Departments - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_Depart).EndInit();
		this.contextMenuStrip_Department.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Depart).EndInit();
		base.ResumeLayout(false);
	}
}
