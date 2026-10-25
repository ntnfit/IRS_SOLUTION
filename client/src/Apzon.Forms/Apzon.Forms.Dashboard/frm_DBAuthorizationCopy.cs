using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Dashboard;

public class frm_DBAuthorizationCopy : XtraFormEx
{
	public string _user = "";

	private IContainer components;

	private GridControlEx grd_Copy;

	private GridColumnEx gcUserCode;

	private GridColumnEx gcName;

	private SimpleButtonEx bt_Cancel;

	private SimpleButtonEx bt_Update;

	private GridColumnEx gcDepartment;

	private GridViewEx gv_Copy;

	public frm_DBAuthorizationCopy()
	{
		InitializeComponent();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btUpdate_Click(object sender, EventArgs e)
	{
		try
		{
			List<string> list = new List<string>();
			list.Add(_user);
			int[] selectedRows = gv_Copy.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				string text = ((gv_Copy.GetRowCellValue(selectedRows[i], "USER_CODE") == DBNull.Value || gv_Copy.GetRowCellValue(selectedRows[i], "USER_CODE") == null) ? "" : gv_Copy.GetRowCellValue(selectedRows[i], "USER_CODE").ToString());
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(text);
				}
			}
			if (list.Count == 1)
			{
				Close();
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			using HttpClientHelper<List<string>> hp = new HttpClientHelper<List<string>>(Configuration.ApiServer, "DashBoardAuth");
			string text2 = await hp.PostAsJsonAsync("CopyAuth", list);
			if (string.IsNullOrEmpty(text2))
			{
				WinCommons.Success(Enums.MessageCode.AuthozationCopySuccess, "");
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_DBAuthorizationCopy_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardAuth");
			DataTable dataSource = await hp.GetAsync("GetUserForCopy", new
			{
				user = _user
			});
			grd_Copy.DataSource = dataSource;
			grd_Copy.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DBAuthorizationCopy));
		this.grd_Copy = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Copy = new Apzon.Controls.Grid.GridViewEx();
		this.gcUserCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDepartment = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).BeginInit();
		base.SuspendLayout();
		this.grd_Copy.AllowPasteExcel = true;
		this.grd_Copy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Copy.ClipboardData = null;
		this.grd_Copy.Location = new System.Drawing.Point(4, 3);
		this.grd_Copy.MainView = this.gv_Copy;
		this.grd_Copy.Name = "grd_Copy";
		this.grd_Copy.Size = new System.Drawing.Size(428, 300);
		this.grd_Copy.TabIndex = 5;
		this.grd_Copy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Copy });
		this.gv_Copy.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Copy.ColumnKey = null;
		this.gv_Copy.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcUserCode, this.gcName, this.gcDepartment });
		this.gv_Copy.DisableFormSetting = false;
		this.gv_Copy.DisplayException = false;
		this.gv_Copy.EditColumn = null;
		this.gv_Copy.FieldCurrency = null;
		this.gv_Copy.FieldRate = null;
		this.gv_Copy.FormId = "oDashBoardTabAuthCopy";
		this.gv_Copy.GridControl = this.grd_Copy;
		this.gv_Copy.IgnoreFilter = false;
		this.gv_Copy.IndexEditRow = -1;
		this.gv_Copy.IndicatorWidth = 35;
		this.gv_Copy.IsApzTable = false;
		this.gv_Copy.IsInsertNewRow = false;
		this.gv_Copy.IsLoadingData = false;
		this.gv_Copy.IsMappingContextAction = false;
		this.gv_Copy.ItemId = "OCOPY";
		this.gv_Copy.LstColumnAllowBatch = null;
		this.gv_Copy.LstColumnNotAllowBatch = null;
		this.gv_Copy.LstColumnNotEditable = null;
		this.gv_Copy.LstColumNotSetting = null;
		this.gv_Copy.Name = "gv_Copy";
		this.gv_Copy.OptionsSelection.MultiSelect = true;
		this.gv_Copy.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Copy.OptionsView.ShowDetailButtons = false;
		this.gv_Copy.OptionsView.ShowGroupPanel = false;
		this.gv_Copy.UseBatchMod = false;
		this.gcUserCode.Caption = "User Code";
		this.gcUserCode.CurrencyDateField = null;
		this.gcUserCode.CurrencyField = null;
		this.gcUserCode.CurrencyFixed = null;
		this.gcUserCode.CurrencyFixedRate = 0.0;
		this.gcUserCode.CurrencyRateField = null;
		this.gcUserCode.FieldName = "USER_CODE";
		this.gcUserCode.Name = "gcUserCode";
		this.gcUserCode.OptionsColumn.AllowEdit = false;
		this.gcUserCode.OptionsColumn.ReadOnly = true;
		this.gcUserCode.PrimaryKey = false;
		this.gcUserCode.UnitField = null;
		this.gcUserCode.Visible = true;
		this.gcUserCode.VisibleIndex = 1;
		this.gcName.Caption = "U_NAME";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "U_NAME";
		this.gcName.Name = "gcName";
		this.gcName.OptionsColumn.AllowEdit = false;
		this.gcName.OptionsColumn.ReadOnly = true;
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 3;
		this.gcDepartment.Caption = "Department";
		this.gcDepartment.CurrencyDateField = null;
		this.gcDepartment.CurrencyField = null;
		this.gcDepartment.CurrencyFixed = null;
		this.gcDepartment.CurrencyFixedRate = 0.0;
		this.gcDepartment.CurrencyRateField = null;
		this.gcDepartment.FieldName = "Department";
		this.gcDepartment.Name = "gcDepartment";
		this.gcDepartment.OptionsColumn.AllowEdit = false;
		this.gcDepartment.OptionsColumn.ReadOnly = true;
		this.gcDepartment.PrimaryKey = false;
		this.gcDepartment.UnitField = null;
		this.gcDepartment.Visible = true;
		this.gcDepartment.VisibleIndex = 2;
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(88, 311);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 7;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.btCancel_Click);
		this.bt_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Update.EditControl = null;
		this.bt_Update.EditValue = "OK";
		this.bt_Update.FieldName = null;
		this.bt_Update.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Update.ImageOptions.Image");
		this.bt_Update.IsSearchControl = false;
		this.bt_Update.Location = new System.Drawing.Point(7, 311);
		this.bt_Update.Name = "bt_Update";
		this.bt_Update.ReadOnly = false;
		this.bt_Update.SettingInVisible = false;
		this.bt_Update.SettingReadonly = false;
		this.bt_Update.Size = new System.Drawing.Size(75, 23);
		this.bt_Update.TabIndex = 6;
		this.bt_Update.Text = "OK";
		this.bt_Update.Click += new System.EventHandler(this.btUpdate_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(434, 340);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_Update);
		base.Controls.Add(this.grd_Copy);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.FormId = "oDashBoardTabAuthCopy";
		base.IconOptions.ShowIcon = false;
		base.MinimizeBox = false;
		base.Name = "frm_DBAuthorizationCopy";
		this.Text = "DashBoard Tabs Authorization Copy";
		base.Load += new System.EventHandler(this.frm_DBAuthorizationCopy_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).EndInit();
		base.ResumeLayout(false);
	}
}
