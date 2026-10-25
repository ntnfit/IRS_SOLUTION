using System;
using System.Collections.Generic;
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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Reports;

public class frm_ReportFormSetting_Copy : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Copy;

	private GridViewEx gv_Copy;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	public string userName { get; set; }

	public int idFormSetting { get; set; }

	public frm_ReportFormSetting_Copy()
	{
		InitializeComponent();
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_FormSetting_Copy_Load(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			btn_Save.Enabled = false;
			await LoadData();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btn_Save.Enabled = true;
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			await CopyFormSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			DataSet ds = await _httpClient.FormSettingClient.GetUserForCopy(userName, idFormSetting);
			await GridViewHelper.GenerateGrid(grd_Copy, gv_Copy, ds, null, expand: true);
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyFormSetting()
	{
		try
		{
			List<string> list = new List<string>();
			int[] selectedRows = gv_Copy.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				list.AddRange(from t in selectedRows
					select Function.ToString(gv_Copy.GetRowCellValue(t, "USER_CODE")) into user
					where !string.IsNullOrWhiteSpace(user)
					select user);
				string listUser = string.Join(",", list.ToArray());
				HttpResult httpResult = await _httpClient.FormSettingClient.CopyReportFormSetting(listUser, idFormSetting);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
					Close();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ReportFormSetting_Copy));
		this.grd_Copy = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Copy = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).BeginInit();
		base.SuspendLayout();
		this.grd_Copy.AllowPasteExcel = true;
		this.grd_Copy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Copy.ClipboardData = null;
		this.grd_Copy.Location = new System.Drawing.Point(3, 5);
		this.grd_Copy.MainView = this.gv_Copy;
		this.grd_Copy.Name = "grd_Copy";
		this.grd_Copy.Size = new System.Drawing.Size(428, 300);
		this.grd_Copy.TabIndex = 8;
		this.grd_Copy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Copy });
		this.gv_Copy.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Copy.ColumnKey = null;
		this.gv_Copy.DisableFormSetting = true;
		this.gv_Copy.DisplayException = false;
		this.gv_Copy.EditColumn = null;
		this.gv_Copy.FieldCurrency = null;
		this.gv_Copy.FieldRate = null;
		this.gv_Copy.FormId = "oReportFormSetting";
		this.gv_Copy.GridControl = this.grd_Copy;
		this.gv_Copy.IgnoreFilter = false;
		this.gv_Copy.IndexEditRow = -1;
		this.gv_Copy.IndicatorWidth = 35;
		this.gv_Copy.IsApzTable = false;
		this.gv_Copy.IsInsertNewRow = false;
		this.gv_Copy.IsLoadingData = false;
		this.gv_Copy.IsMappingContextAction = false;
		this.gv_Copy.ItemId = "OUSER";
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
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 311);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 10;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 311);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 11;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btnSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(434, 340);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Copy);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oReportFormSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReportFormSetting_Copy";
		base.ShowInTaskbar = false;
		this.Text = "Copy Form Setting";
		base.Load += new System.EventHandler(this.frm_FormSetting_Copy_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).EndInit();
		base.ResumeLayout(false);
	}
}
