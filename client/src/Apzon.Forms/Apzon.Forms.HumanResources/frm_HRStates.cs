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

public class frm_HRStates : XtraFormEx
{
	public RefreshLookupStates RefreshStates;

	public Func<string, Task> RefreshAllStates;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private const string FormController = "States";

	public string CountryCode;

	public int Type;

	private IContainer components;

	private GridControlEx grd_States;

	private GridViewEx gv_States;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_name;

	private GridColumnEx cl_UserSign;

	private GridColumnEx cl_eCode;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx cl_Country;

	private GridColumnEx cl_GNRECode;

	private ContextMenuStrip contextMenuStrip_States;

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
		case Enums.FormViewModeType.Busy:
			btn_Save.Enabled = false;
			btnCancel.Enabled = false;
			break;
		case Enums.FormViewModeType.Add:
			break;
		}
	}

	public frm_HRStates()
	{
		InitializeComponent();
		FormId = "oHRStates";
		gv_States.FormId = FormId;
		gv_States.ItemId = "OHTM";
		OnLoadAsync += frm_HRStates_Load;
	}

	private async Task frm_HRStates_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshData();
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
				await SaveStates();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_States_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
			gv_States.BeginInsertRow();
			DataTable dataTable = grd_States.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_States.DataSource = dataTable;
				grd_States.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OCST");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = "";
				dataRow2["Name"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_States.DataSource = tableStructure;
				grd_States.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_States.EndInsertRow();
		}
	}

	private void gv_States_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_States.RowCount; i++)
			{
				num = gv_States.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_States.GetRowCellValue(e.RowHandle, "code"));
			if (e.Column.FieldName == "code" && text != "" && e.RowHandle == num)
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

	private void gv_States_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_States.GetDataRow(gv_States.FocusedRowHandle) == null)
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
			string text = Function.ToString(gv_States.GetFocusedRowCellValue("Code"));
			Function.ParseInt(gv_States.GetFocusedRowCellValue("UserSign"));
			if (!(text == "") && text != null && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveStates(text);
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

	private async void RemoveStates(string code)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = hp.Post("RemoveStates", code);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_States.BeginUpdate();
				gv_States.DeleteSelectedRows();
				gv_States.EndUpdate();
				if (RefreshStates != null && !((XtraForm)RefreshStates.Target).IsDisposed)
				{
					RefreshStates(CountryCode, Type);
				}
				if (RefreshAllStates != null && !((XtraForm)RefreshAllStates.Target).IsDisposed)
				{
					RefreshAllStates(CountryCode);
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
			grd_States.DataSource = LoadStates(CountryCode);
			grd_States.DataSourceTable.AcceptChanges();
			grd_States.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetStates()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await helper.GetAsync("GetStates");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public DataTable LoadStates(string CountryCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return httpClientHelper.Get("LoadStates", new { CountryCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	private async Task SaveStates()
	{
		try
		{
			DataTable dataTable = (grd_States.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Country"]) == "");
				if (string.IsNullOrEmpty(Function.ToString(dataRow["Code"])) && string.IsNullOrEmpty(Function.ToString(dataRow["Name"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["Code"])).Distinct().Count() != dataTable.Rows.Count)
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
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]).Length > 3) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Code"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.CodeCannotBeEmpty, "");
						return;
					}
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						if (dataTable.Rows[num]["UserSign"] == DBNull.Value)
						{
							dataTable.Rows[num]["UserSign"] = GlobalData.UserID;
						}
						dataTable.Rows[num]["Country"] = CountryCode;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _help.Post("SaveStates", dataTable);
					if (string.IsNullOrWhiteSpace(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (RefreshStates != null && !((XtraForm)RefreshStates.Target).IsDisposed)
						{
							RefreshStates(CountryCode, Type);
						}
						if (RefreshAllStates != null && !((XtraForm)RefreshAllStates.Target).IsDisposed)
						{
							RefreshAllStates(CountryCode);
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
				grd_States.DataSource = dataTable.Clone();
				grd_States.RefreshDataSource();
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRStates));
		this.grd_States = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_States = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_States = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Country = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_eCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_GNRECode = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_States).BeginInit();
		this.contextMenuStrip_States.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_States).BeginInit();
		base.SuspendLayout();
		this.grd_States.AllowPasteExcel = true;
		this.grd_States.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_States.ClipboardData = null;
		this.grd_States.ContextMenuStrip = this.contextMenuStrip_States;
		this.grd_States.Location = new System.Drawing.Point(3, 1);
		this.grd_States.MainView = this.gv_States;
		this.grd_States.Name = "grd_States";
		this.grd_States.Size = new System.Drawing.Size(553, 310);
		this.grd_States.TabIndex = 69;
		this.grd_States.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_States });
		this.contextMenuStrip_States.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_States.Name = "contextMenuStrip_States";
		this.contextMenuStrip_States.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_States.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_States.ColumnKey = null;
		this.gv_States.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.cl_Code, this.cl_Country, this.cl_name, this.cl_UserSign, this.cl_eCode, this.cl_GNRECode });
		this.gv_States.DisableFormSetting = false;
		this.gv_States.DisplayException = false;
		this.gv_States.EditColumn = null;
		this.gv_States.FieldCurrency = null;
		this.gv_States.FieldRate = null;
		this.gv_States.FormId = "oHRStates";
		this.gv_States.GridControl = this.grd_States;
		this.gv_States.IgnoreFilter = false;
		this.gv_States.IndexEditRow = -1;
		this.gv_States.IndicatorWidth = 30;
		this.gv_States.IsApzTable = false;
		this.gv_States.IsInsertNewRow = false;
		this.gv_States.IsLoadingData = false;
		this.gv_States.IsMappingContextAction = false;
		this.gv_States.ItemId = "OHTM";
		this.gv_States.LstColumnAllowBatch = null;
		this.gv_States.LstColumnNotAllowBatch = null;
		this.gv_States.LstColumnNotEditable = null;
		this.gv_States.LstColumNotSetting = null;
		this.gv_States.Name = "gv_States";
		this.gv_States.OptionsView.RowAutoHeight = true;
		this.gv_States.OptionsView.ShowGroupPanel = false;
		this.gv_States.UseBatchMod = false;
		this.gv_States.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_States_CustomDrawRowIndicator);
		this.gv_States.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_States_CellValueChanged);
		this.gv_States.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_States_MouseUp);
		this.cl_Code.Caption = "Code";
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.PrimaryKey = true;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 0;
		this.cl_Code.Width = 260;
		this.cl_Country.Caption = "Country";
		this.cl_Country.CurrencyDateField = null;
		this.cl_Country.CurrencyField = null;
		this.cl_Country.CurrencyFixed = null;
		this.cl_Country.CurrencyFixedRate = 0.0;
		this.cl_Country.CurrencyRateField = null;
		this.cl_Country.FieldName = "Country";
		this.cl_Country.Name = "cl_Country";
		this.cl_Country.OptionsColumn.ReadOnly = true;
		this.cl_Country.PrimaryKey = false;
		this.cl_Country.UnitField = null;
		this.cl_name.Caption = "Name";
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
		this.cl_name.VisibleIndex = 1;
		this.cl_name.Width = 412;
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
		this.cl_eCode.Caption = "eCode";
		this.cl_eCode.CurrencyDateField = null;
		this.cl_eCode.CurrencyField = null;
		this.cl_eCode.CurrencyFixed = null;
		this.cl_eCode.CurrencyFixedRate = 0.0;
		this.cl_eCode.CurrencyRateField = null;
		this.cl_eCode.FieldName = "eCode";
		this.cl_eCode.Name = "cl_eCode";
		this.cl_eCode.OptionsColumn.ReadOnly = true;
		this.cl_eCode.PrimaryKey = false;
		this.cl_eCode.UnitField = null;
		this.cl_GNRECode.Caption = "GNRECode";
		this.cl_GNRECode.CurrencyDateField = null;
		this.cl_GNRECode.CurrencyField = null;
		this.cl_GNRECode.CurrencyFixed = null;
		this.cl_GNRECode.CurrencyFixedRate = 0.0;
		this.cl_GNRECode.CurrencyRateField = null;
		this.cl_GNRECode.FieldName = "GNRECode";
		this.cl_GNRECode.Name = "cl_GNRECode";
		this.cl_GNRECode.OptionsColumn.ReadOnly = true;
		this.cl_GNRECode.PrimaryKey = false;
		this.cl_GNRECode.UnitField = null;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 315);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 68;
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
		this.btnCancel.Location = new System.Drawing.Point(68, 315);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 67;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(560, 338);
		base.Controls.Add(this.grd_States);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oHRStates";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRStates";
		this.Text = "States - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_States).EndInit();
		this.contextMenuStrip_States.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_States).EndInit();
		base.ResumeLayout(false);
	}
}
