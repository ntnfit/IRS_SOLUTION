using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_DataOwnerShipAuthorizationCopy : XtraFormEx
{
	private readonly IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GroupControl groupControl1;

	private GridControlEx grd_Employess;

	private GridViewEx gv_Employees;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	public int EmpId { get; set; }

	public DataTable Employees { get; set; }

	public frm_DataOwnerShipAuthorizationCopy()
	{
		InitializeComponent();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			List<int> list = new List<int>();
			list.Add(EmpId);
			int[] selectedRows = gv_Employees.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				int num = Function.ParseInt(gv_Employees.GetRowCellValue(selectedRows[i], "empID"));
				if (num != 0)
				{
					list.Add(num);
				}
			}
			if (list.Count > 1)
			{
				HttpResult httpResult = await httpClient.OwnerAuthorize.CopyOwner(list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_DataOwnerShipAuthorizationCopy_Load(object sender, EventArgs e)
	{
		try
		{
			grd_Employess.DataSource = Employees;
			grd_Employess.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_DataOwnerShipAuthorizationCopy));
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Employess = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Employees = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Employess).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employees).BeginInit();
		base.SuspendLayout();
		this.groupControl1.Controls.Add(this.btn_Cancel);
		this.groupControl1.Controls.Add(this.btn_OK);
		this.groupControl1.Controls.Add(this.grd_Employess);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(413, 490);
		this.groupControl1.TabIndex = 8;
		this.groupControl1.Text = "Employees";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 455);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 455);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.grd_Employess.AllowPasteExcel = true;
		this.grd_Employess.ClipboardData = null;
		this.grd_Employess.Location = new System.Drawing.Point(2, 23);
		this.grd_Employess.MainView = this.gv_Employees;
		this.grd_Employess.Name = "grd_Employess";
		this.grd_Employess.Size = new System.Drawing.Size(411, 423);
		this.grd_Employess.TabIndex = 0;
		this.grd_Employess.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Employees });
		this.gv_Employees.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Employees.ColumnKey = null;
		this.gv_Employees.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gv_Employees.DisableFormSetting = false;
		this.gv_Employees.DisplayException = false;
		this.gv_Employees.EditColumn = null;
		this.gv_Employees.FieldCurrency = null;
		this.gv_Employees.FieldRate = null;
		this.gv_Employees.FormId = "oDataOwnerShipAuthorization";
		this.gv_Employees.GridControl = this.grd_Employess;
		this.gv_Employees.IgnoreFilter = false;
		this.gv_Employees.IndexEditRow = -1;
		this.gv_Employees.IndicatorWidth = 35;
		this.gv_Employees.IsApzTable = false;
		this.gv_Employees.IsInsertNewRow = false;
		this.gv_Employees.IsLoadingData = false;
		this.gv_Employees.IsMappingContextAction = false;
		this.gv_Employees.ItemId = "OHEM";
		this.gv_Employees.LstColumnAllowBatch = null;
		this.gv_Employees.LstColumnNotAllowBatch = null;
		this.gv_Employees.LstColumnNotEditable = null;
		this.gv_Employees.LstColumNotSetting = null;
		this.gv_Employees.Name = "gv_Employees";
		this.gv_Employees.OptionsBehavior.Editable = false;
		this.gv_Employees.OptionsSelection.MultiSelect = true;
		this.gv_Employees.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Employees.OptionsView.ShowGroupPanel = false;
		this.gv_Employees.UseBatchMod = false;
		this.gridColumn1.Caption = "empID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "empID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "FullName";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "FullName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 150;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(413, 490);
		base.Controls.Add(this.groupControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DataOwnerShipAuthorizationCopy.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DataOwnerShipAuthorizationCopy";
		base.ShowInTaskbar = false;
		this.Text = "Data Ownership Authorization - Copy";
		base.Load += new System.EventHandler(this.frm_DataOwnerShipAuthorizationCopy_Load);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Employess).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employees).EndInit();
		base.ResumeLayout(false);
	}
}
