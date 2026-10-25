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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmCflMps : XtraFormEx
{
	private readonly IHttpClientHelper<Mps> _mpsClientHelper = new HttpClientHelper<Mps>(Configuration.ApiServer, "Mps");

	private readonly XtraFormEx frm;

	public string mpsCode = string.Empty;

	private int status;

	private IContainer components;

	private SimpleButtonEx btSearch;

	private ComboBoxEditEx cbStatus;

	private ButtonEditEx txtCode;

	private GroupControl groupControl2;

	private GridControlEx grdControlMPS;

	private GridViewEx grdViewMPS;

	private GridColumnEx gColId;

	private GridColumnEx gColCode;

	private GridColumnEx gColCreateDate;

	private GridColumnEx gColBeginDate;

	private GridColumnEx gColEndDate;

	private GridColumnEx gColStatus;

	private GridColumnEx gColNotes;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	private GroupControl groupControl1;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	public FrmCflMps(XtraFormEx frm)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflMps"))
		{
			this.frm = frm;
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflMps" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void FrmCflMps_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		txtCode.Text = mpsCode;
		List<Common.Table_ID> list = FrmMrp.GetStatus();
		WinFuntions._addComboBox(cbStatus, list, _selectFirstItem: true);
		if (frm.Name == "FrmMps")
		{
			status = 1;
			cbStatus.Enabled = false;
		}
		else
		{
			cbStatus.Enabled = true;
		}
		await RefreshData();
		WinCommons._CloseProcessing();
	}

	private async Task RefreshData()
	{
		mpsCode = txtCode.Text.Trim().ToUpper();
		new DataTable();
		if (frm.Name.Equals("FrmMrp"))
		{
			status = 2;
			WinFuntions._selectItemComboBoxByValue(cbStatus, status.ToString());
			cbStatus.Enabled = false;
		}
		else if (!object.Equals(cbStatus.SelectedItem, string.Empty))
		{
			Common.ListItems listItems = (Common.ListItems)cbStatus.SelectedItem;
			status = Convert.ToInt32(listItems.Value);
		}
		DataTable dataSource = await GetOMPS(mpsCode, status, DateTime.MinValue, DateTime.MinValue);
		grdControlMPS.DataSource = dataSource;
		grdControlMPS.RefreshDataSource();
	}

	private async Task<DataTable> GetOMPS(string mpsCode, int status, DateTime beginDate, DateTime endDate)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsync("GetMps", new { mpsCode, status, beginDate, endDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async void btSearch_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await RefreshData();
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnChoose_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await SetValueToMpsCode();
		WinCommons._CloseProcessing();
	}

	private async Task SetValueToMpsCode()
	{
		try
		{
			if (grdViewMPS.FocusedRowHandle >= 0)
			{
				if (frm.Name == "FrmListMps")
				{
					((FrmListMps)frm).mpsCode = grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "MpsCode").ToString();
					Close();
				}
				if (frm.Name == "FrmMps")
				{
					((FrmMps)frm).MpsCode = grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "MpsCode").ToString();
					((FrmMps)frm).Id = Convert.ToInt32(grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "Id").ToString());
					Close();
				}
				if (frm.Name == "FrmMrp")
				{
					((FrmMrp)frm).obj = new Mrp();
					((FrmMrp)frm).obj.MpsCode = grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "MpsCode").ToString();
					Close();
				}
				if (frm.Name == "FrmMrpList")
				{
					((FrmMrpList)frm).mpsCode = grdViewMPS.GetRowCellValue(grdViewMPS.FocusedRowHandle, "MpsCode").ToString();
					Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewMPS_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.Caption == "Status" && e.Value != null)
		{
			if (e.Value.ToString() == 1.ToString())
			{
				e.DisplayText = "Draft";
			}
			else if (e.Value.ToString() == 2.ToString())
			{
				e.DisplayText = "Aprroved";
			}
			else if (e.Value.ToString() == 3.ToString())
			{
				e.DisplayText = "Open";
			}
			else if (e.Value.ToString() == 4.ToString())
			{
				e.DisplayText = "Close";
			}
			else if (e.Value.ToString() == (-1).ToString())
			{
				e.DisplayText = "Reject";
			}
			else if (e.Value.ToString() == 5.ToString())
			{
				e.DisplayText = "Done";
			}
		}
	}

	private async void grdViewMPS_DoubleClick(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await SetValueToMpsCode();
		WinCommons._CloseProcessing();
	}

	private async void grdViewMPS_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				WinCommons._OpenProcessing();
				await SetValueToMpsCode();
				WinCommons._CloseProcessing();
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmCflMps));
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbStatus = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grdControlMPS = new Apzon.Controls.Grid.GridControlEx();
		this.grdViewMPS = new Apzon.Controls.Grid.GridViewEx();
		this.gColId = new Apzon.Controls.Grid.GridColumnEx();
		this.gColCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gColCreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColBeginDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gColStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gColNotes = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdControlMPS).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMPS).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		base.SuspendLayout();
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(486, 25);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(75, 20);
		this.btSearch.TabIndex = 2;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(365, 25);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(115, 20);
		this.cbStatus.TabIndex = 1;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(98, 25);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(160, 20);
		this.txtCode.TabIndex = 0;
		this.groupControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl2.Controls.Add(this.grdControlMPS);
		this.groupControl2.Location = new System.Drawing.Point(0, 56);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(571, 281);
		this.groupControl2.TabIndex = 5;
		this.groupControl2.Text = "List MPS";
		this.grdControlMPS.AllowPasteExcel = true;
		this.grdControlMPS.ClipboardData = null;
		this.grdControlMPS.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdControlMPS.Location = new System.Drawing.Point(2, 23);
		this.grdControlMPS.MainView = this.grdViewMPS;
		this.grdControlMPS.Name = "grdControlMPS";
		this.grdControlMPS.Size = new System.Drawing.Size(567, 256);
		this.grdControlMPS.TabIndex = 0;
		this.grdControlMPS.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdViewMPS });
		this.grdViewMPS.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.grdViewMPS.ColumnKey = null;
		this.grdViewMPS.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gColId, this.gColCode, this.gColCreateDate, this.gColBeginDate, this.gColEndDate, this.gColStatus, this.gColNotes });
		this.grdViewMPS.DisableFormSetting = false;
		this.grdViewMPS.DisplayException = false;
		this.grdViewMPS.EditColumn = null;
		this.grdViewMPS.FieldCurrency = null;
		this.grdViewMPS.FieldRate = null;
		this.grdViewMPS.FormId = null;
		this.grdViewMPS.GridControl = this.grdControlMPS;
		this.grdViewMPS.IgnoreFilter = false;
		this.grdViewMPS.IndexEditRow = -1;
		this.grdViewMPS.IndicatorWidth = 30;
		this.grdViewMPS.IsApzTable = false;
		this.grdViewMPS.IsInsertNewRow = false;
		this.grdViewMPS.IsLoadingData = false;
		this.grdViewMPS.IsMappingContextAction = false;
		this.grdViewMPS.ItemId = null;
		this.grdViewMPS.LstColumnAllowBatch = null;
		this.grdViewMPS.LstColumnNotAllowBatch = null;
		this.grdViewMPS.LstColumnNotEditable = null;
		this.grdViewMPS.LstColumNotSetting = null;
		this.grdViewMPS.Name = "grdViewMPS";
		this.grdViewMPS.OptionsBehavior.Editable = false;
		this.grdViewMPS.OptionsView.ShowGroupPanel = false;
		this.grdViewMPS.UseBatchMod = false;
		this.grdViewMPS.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.grdViewMPS_CustomColumnDisplayText);
		this.grdViewMPS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdViewMPS_KeyDown);
		this.grdViewMPS.DoubleClick += new System.EventHandler(this.grdViewMPS_DoubleClick);
		this.gColId.Caption = "Id";
		this.gColId.CurrencyDateField = null;
		this.gColId.CurrencyField = null;
		this.gColId.CurrencyFixed = null;
		this.gColId.CurrencyFixedRate = 0.0;
		this.gColId.CurrencyRateField = null;
		this.gColId.FieldName = "Id";
		this.gColId.Name = "gColId";
		this.gColId.OptionsColumn.AllowEdit = false;
		this.gColId.PrimaryKey = true;
		this.gColId.UnitField = null;
		this.gColCode.Caption = "MPS Code";
		this.gColCode.CurrencyDateField = null;
		this.gColCode.CurrencyField = null;
		this.gColCode.CurrencyFixed = null;
		this.gColCode.CurrencyFixedRate = 0.0;
		this.gColCode.CurrencyRateField = null;
		this.gColCode.FieldName = "MpsCode";
		this.gColCode.Name = "gColCode";
		this.gColCode.OptionsColumn.AllowEdit = false;
		this.gColCode.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gColCode.PrimaryKey = false;
		this.gColCode.UnitField = null;
		this.gColCode.Visible = true;
		this.gColCode.VisibleIndex = 0;
		this.gColCode.Width = 110;
		this.gColCreateDate.Caption = "Create Date";
		this.gColCreateDate.CurrencyDateField = null;
		this.gColCreateDate.CurrencyField = null;
		this.gColCreateDate.CurrencyFixed = null;
		this.gColCreateDate.CurrencyFixedRate = 0.0;
		this.gColCreateDate.CurrencyRateField = null;
		this.gColCreateDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gColCreateDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gColCreateDate.FieldName = "CreateDate";
		this.gColCreateDate.Name = "gColCreateDate";
		this.gColCreateDate.OptionsColumn.AllowEdit = false;
		this.gColCreateDate.PrimaryKey = false;
		this.gColCreateDate.UnitField = null;
		this.gColCreateDate.Width = 62;
		this.gColBeginDate.Caption = "From Date";
		this.gColBeginDate.CurrencyDateField = null;
		this.gColBeginDate.CurrencyField = null;
		this.gColBeginDate.CurrencyFixed = null;
		this.gColBeginDate.CurrencyFixedRate = 0.0;
		this.gColBeginDate.CurrencyRateField = null;
		this.gColBeginDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gColBeginDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gColBeginDate.FieldName = "BeginDate";
		this.gColBeginDate.Name = "gColBeginDate";
		this.gColBeginDate.OptionsColumn.AllowEdit = false;
		this.gColBeginDate.PrimaryKey = false;
		this.gColBeginDate.UnitField = null;
		this.gColBeginDate.Visible = true;
		this.gColBeginDate.VisibleIndex = 1;
		this.gColBeginDate.Width = 67;
		this.gColEndDate.Caption = "To Date";
		this.gColEndDate.CurrencyDateField = null;
		this.gColEndDate.CurrencyField = null;
		this.gColEndDate.CurrencyFixed = null;
		this.gColEndDate.CurrencyFixedRate = 0.0;
		this.gColEndDate.CurrencyRateField = null;
		this.gColEndDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gColEndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gColEndDate.FieldName = "EndDate";
		this.gColEndDate.Name = "gColEndDate";
		this.gColEndDate.OptionsColumn.AllowEdit = false;
		this.gColEndDate.PrimaryKey = false;
		this.gColEndDate.UnitField = null;
		this.gColEndDate.Visible = true;
		this.gColEndDate.VisibleIndex = 2;
		this.gColEndDate.Width = 69;
		this.gColStatus.Caption = "Status";
		this.gColStatus.CurrencyDateField = null;
		this.gColStatus.CurrencyField = null;
		this.gColStatus.CurrencyFixed = null;
		this.gColStatus.CurrencyFixedRate = 0.0;
		this.gColStatus.CurrencyRateField = null;
		this.gColStatus.FieldName = "Status";
		this.gColStatus.Name = "gColStatus";
		this.gColStatus.OptionsColumn.AllowEdit = false;
		this.gColStatus.PrimaryKey = false;
		this.gColStatus.UnitField = null;
		this.gColStatus.Visible = true;
		this.gColStatus.VisibleIndex = 3;
		this.gColStatus.Width = 68;
		this.gColNotes.Caption = "Remark";
		this.gColNotes.CurrencyDateField = null;
		this.gColNotes.CurrencyField = null;
		this.gColNotes.CurrencyFixed = null;
		this.gColNotes.CurrencyFixedRate = 0.0;
		this.gColNotes.CurrencyRateField = null;
		this.gColNotes.FieldName = "Notes";
		this.gColNotes.Name = "gColNotes";
		this.gColNotes.OptionsColumn.AllowEdit = false;
		this.gColNotes.PrimaryKey = false;
		this.gColNotes.UnitField = null;
		this.gColNotes.Visible = true;
		this.gColNotes.VisibleIndex = 4;
		this.gColNotes.Width = 159;
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.btSearch);
		this.groupControl1.Controls.Add(this.cbStatus);
		this.groupControl1.Controls.Add(this.txtCode);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(571, 73);
		this.groupControl1.TabIndex = 4;
		this.groupControl1.Text = "MPS Info";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(275, 27);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(90, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Status";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "MPS Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(8, 27);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(90, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "MPS Code";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(87, 343);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(8, 343);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 6;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(571, 368);
		base.Controls.Add(this.groupControl2);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.Controls.Add(this.groupControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmCflMps";
		base.ShowInTaskbar = false;
		this.Text = "List Of MPS";
		base.Load += new System.EventHandler(this.FrmCflMps_Load);
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdControlMPS).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMPS).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
