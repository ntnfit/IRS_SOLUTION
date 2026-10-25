using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_State : XtraFormEx
{
	private DataTable dt = new DataTable();

	private DataTable dtDelete = new DataTable();

	public string _CodeLoadFrom = string.Empty;

	private IContainer components;

	private GridControlEx grd_State;

	private GridViewEx gv_State;

	private GridColumnEx gcCode;

	private GridColumnEx gcName;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private GridColumnEx gcCountry;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcEcode;

	private GridColumnEx gcGNRECode;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_State()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_State"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_State" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			WinCommons._OpenProcessing();
			bt_OK.Enabled = false;
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		bt_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async void frm_State_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		string codeLoadFrom = _CodeLoadFrom;
		try
		{
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			dt = await _hp.GetAsync("GetState", new
			{
				Code = codeLoadFrom
			});
			if (dt == null || dt.Rows.Count <= 0)
			{
				await GetDefaultTable();
			}
			else
			{
				grd_State.DataSource = dt;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetDefaultTable()
	{
		try
		{
			DataTable dt = new DataTable
			{
				Columns = 
				{
					{
						"Code",
						typeof(string)
					},
					{
						"Country",
						typeof(string)
					},
					{
						"Name",
						typeof(string)
					},
					{
						"UserSign",
						typeof(int)
					},
					{
						"eCode",
						typeof(string)
					},
					{
						"GNRECode",
						typeof(string)
					}
				}
			};
			DataRow dr = dt.NewRow();
			dr["Code"] = string.Empty;
			dr["Country"] = _CodeLoadFrom;
			dr["Name"] = string.Empty;
			dr["UserSign"] = await GetUserSignByUserCode_tableOUSR();
			dr["eCode"] = string.Empty;
			dr["GNRECode"] = string.Empty;
			dt.Rows.Add(dr);
			grd_State.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return -1;
	}

	private async void gv_State_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			dt = grd_State.DataSource as DataTable;
			if (e.Column.FieldName == "Code")
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			await AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewRow()
	{
		_ = 1;
		try
		{
			DataTable dt = grd_State.DataSource as DataTable;
			if (dt == null && dt.Rows.Count < 0)
			{
				await GetDefaultTable();
				return;
			}
			dt = grd_State.DataSource as DataTable;
			string codeLoadFrom = _CodeLoadFrom;
			gv_State.SetFocusedRowCellValue("Country", codeLoadFrom);
			int num = await GetUserSignByUserCode_tableOUSR();
			gv_State.SetFocusedRowCellValue("UserSign", num);
			grd_State.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_State_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_State.BeginUpdate();
			gv_State.DeleteSelectedRows();
			gv_State.EndUpdate();
			dtDelete = grd_State.DataSource as DataTable;
			grd_State.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		dt = grd_State.DataSource as DataTable;
		try
		{
			WinCommons._OpenProcessing();
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "State");
			string text = await _helper.PostParamsAsJsonAsync("Save", dt);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_State));
		this.grd_State = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_State = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCountry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEcode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGNRECode = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_State).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_State).BeginInit();
		base.SuspendLayout();
		this.grd_State.AllowPasteExcel = true;
		this.grd_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_State.ClipboardData = null;
		this.grd_State.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_State.Location = new System.Drawing.Point(2, 2);
		this.grd_State.MainView = this.gv_State;
		this.grd_State.Name = "grd_State";
		this.grd_State.Size = new System.Drawing.Size(473, 242);
		this.grd_State.TabIndex = 0;
		this.grd_State.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_State });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_State.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_State.ColumnKey = null;
		this.gv_State.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcCode, this.gcName, this.gcCountry, this.gcUserSign, this.gcEcode, this.gcGNRECode });
		this.gv_State.DisableFormSetting = false;
		this.gv_State.DisplayException = false;
		this.gv_State.EditColumn = null;
		this.gv_State.FieldCurrency = null;
		this.gv_State.FieldRate = null;
		this.gv_State.FormId = null;
		this.gv_State.GridControl = this.grd_State;
		this.gv_State.IgnoreFilter = false;
		this.gv_State.IndexEditRow = -1;
		this.gv_State.IndicatorWidth = 20;
		this.gv_State.IsApzTable = false;
		this.gv_State.IsInsertNewRow = false;
		this.gv_State.IsLoadingData = false;
		this.gv_State.IsMappingContextAction = false;
		this.gv_State.ItemId = null;
		this.gv_State.LstColumnAllowBatch = null;
		this.gv_State.LstColumnNotAllowBatch = null;
		this.gv_State.LstColumnNotEditable = null;
		this.gv_State.LstColumNotSetting = null;
		this.gv_State.Name = "gv_State";
		this.gv_State.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_State.OptionsView.ShowGroupPanel = false;
		this.gv_State.UseBatchMod = false;
		this.gv_State.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_State_CustomDrawRowIndicator);
		this.gv_State.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_State_CellValueChanging);
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 1;
		this.gcCountry.Caption = "Country";
		this.gcCountry.CurrencyDateField = null;
		this.gcCountry.CurrencyField = null;
		this.gcCountry.CurrencyFixed = null;
		this.gcCountry.CurrencyFixedRate = 0.0;
		this.gcCountry.CurrencyRateField = null;
		this.gcCountry.FieldName = "Country";
		this.gcCountry.Name = "gcCountry";
		this.gcCountry.PrimaryKey = false;
		this.gcCountry.UnitField = null;
		this.gcCountry.Visible = true;
		this.gcCountry.VisibleIndex = 2;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gcUserSign.Visible = true;
		this.gcUserSign.VisibleIndex = 3;
		this.gcEcode.Caption = "eCode";
		this.gcEcode.CurrencyDateField = null;
		this.gcEcode.CurrencyField = null;
		this.gcEcode.CurrencyFixed = null;
		this.gcEcode.CurrencyFixedRate = 0.0;
		this.gcEcode.CurrencyRateField = null;
		this.gcEcode.FieldName = "eCode";
		this.gcEcode.Name = "gcEcode";
		this.gcEcode.PrimaryKey = false;
		this.gcEcode.UnitField = null;
		this.gcGNRECode.Caption = "GNRECode";
		this.gcGNRECode.CurrencyDateField = null;
		this.gcGNRECode.CurrencyField = null;
		this.gcGNRECode.CurrencyFixed = null;
		this.gcGNRECode.CurrencyFixedRate = 0.0;
		this.gcGNRECode.CurrencyRateField = null;
		this.gcGNRECode.FieldName = "GNRECode";
		this.gcGNRECode.Name = "gcGNRECode";
		this.gcGNRECode.PrimaryKey = false;
		this.gcGNRECode.UnitField = null;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(2, 247);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(80, 247);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(477, 273);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_State);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_State";
		this.Text = "States - Setup";
		base.Load += new System.EventHandler(this.frm_State_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_State).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_State).EndInit();
		base.ResumeLayout(false);
	}
}
