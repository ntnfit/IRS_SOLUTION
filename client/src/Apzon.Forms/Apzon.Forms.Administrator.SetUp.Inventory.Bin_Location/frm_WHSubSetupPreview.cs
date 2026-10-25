using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_WHSubSetupPreview : XtraFormEx
{
	public string WhSubName;

	public DataTable DtSource;

	private IContainer components;

	private GridControlEx grdWHSubPre;

	private GridViewEx gvWHSubPre;

	private GridColumnEx colCode1;

	private GridColumnEx colDes;

	private GridColumnEx colErrDetails;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private TextEditEx txtWHSub;

	private LabelControl lblWHSub;

	private LabelLine labelLine1;

	public frm_WHSubSetupPreview()
	{
		InitializeComponent();
	}

	private void EnableDisableButton(bool isEnable)
	{
		SimpleButtonEx simpleButtonEx = btnOk;
		bool enabled = (btnCancel.Enabled = isEnable);
		simpleButtonEx.Enabled = enabled;
	}

	private void frm_WHSubSetupPreview_Load(object sender, EventArgs e)
	{
		txtWHSub.Text = WhSubName;
		string text = Function.ToString(DtSource.Rows[0]["Type"]);
		if (!(text == "A"))
		{
			if (text == "U")
			{
				Text = "Warehouse Sublevel Codes Management - Update Preview";
				btnOk.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
			else
			{
				Text = "Warehouse Sublevel Codes Management - Deletion Preview";
				btnOk.Text = Apzon.Commons.Properties.Resources.ButtonDelete;
			}
		}
		else
		{
			Text = "Warehouse Sublevel Codes Management - Generation Preview";
			btnOk.Text = "Generate";
		}
		grdWHSubPre.DataSource = DtSource;
		grdWHSubPre.RefreshDataSource();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		EnableDisableButton(isEnable: false);
		try
		{
			if (DtSource.Rows.Count == 0)
			{
				WinCommons.Info(Enums.MessageCode.NoQuantityData, "");
				EnableDisableButton(isEnable: true);
				return;
			}
			gvWHSubPre.ShowLoadingPanel();
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WHSubSetup"))
			{
				HttpResult httpResult = await hp.PostAsync("SaveGen", DtSource);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
					return;
				}
				dataTable = JsonConvert.DeserializeObject<DataTable>(httpResult.Content.ToString());
			}
			gvWHSubPre.HideLoadingPanel();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Info(Enums.MessageCode.SaveError, "");
				EnableDisableButton(isEnable: true);
				return;
			}
			int count = dataTable.Rows.Count;
			int num = 0;
			DataTable dataTable2 = new DataTable();
			if (dataTable.Select("Error <> ''").Length != 0)
			{
				dataTable2 = dataTable.Select("Error <> ''").CopyToDataTable();
				num = dataTable2.Rows.Count;
			}
			string text = Function.ToString(DtSource.Rows[0]["Type"]);
			if (text == "U" && num == 0)
			{
				WinCommons.Info(Enums.MessageCode.OperationCompletedSuccess, "");
				Close();
			}
			else
			{
				grdWHSubPre.DataSource = dataTable2;
				grdWHSubPre.RefreshDataSource();
				DtSource = dataTable2;
				if (text != "U")
				{
					WinCommons.Info(Enums.MessageCode.None, string.Format("{0} out of {1} warehouse sublevel codes have been {2}", count - num, count, (text == "A") ? "generated" : "deleted"));
				}
			}
		}
		catch (Exception ex)
		{
			gvWHSubPre.HideLoadingPanel();
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		EnableDisableButton(isEnable: true);
	}

	private void gvWHSub_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvWHSubPre_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (!string.IsNullOrEmpty(Function.ToString(gvWHSubPre.GetRowCellValue(e.RowHandle, "Error"))))
		{
			e.Appearance.ForeColor = Color.Red;
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
		this.grdWHSubPre = new Apzon.Controls.Grid.GridControlEx();
		this.gvWHSubPre = new Apzon.Controls.Grid.GridViewEx();
		this.colCode1 = new Apzon.Controls.Grid.GridColumnEx();
		this.colDes = new Apzon.Controls.Grid.GridColumnEx();
		this.colErrDetails = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtWHSub = new Apzon.Controls.Editor.TextEditEx();
		this.lblWHSub = new DevExpress.XtraEditors.LabelControl();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grdWHSubPre).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvWHSubPre).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtWHSub.Properties).BeginInit();
		base.SuspendLayout();
		this.grdWHSubPre.AllowPasteExcel = true;
		this.grdWHSubPre.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdWHSubPre.ClipboardData = null;
		this.grdWHSubPre.Location = new System.Drawing.Point(0, 35);
		this.grdWHSubPre.MainView = this.gvWHSubPre;
		this.grdWHSubPre.Name = "grdWHSubPre";
		this.grdWHSubPre.Size = new System.Drawing.Size(650, 281);
		this.grdWHSubPre.TabIndex = 0;
		this.grdWHSubPre.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvWHSubPre });
		this.gvWHSubPre.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvWHSubPre.ColumnKey = null;
		this.gvWHSubPre.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.colCode1, this.colDes, this.colErrDetails });
		this.gvWHSubPre.DisableFormSetting = false;
		this.gvWHSubPre.DisplayException = false;
		this.gvWHSubPre.EditColumn = null;
		this.gvWHSubPre.FieldCurrency = null;
		this.gvWHSubPre.FieldRate = null;
		this.gvWHSubPre.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.gvWHSubPre.FormId = null;
		this.gvWHSubPre.GridControl = this.grdWHSubPre;
		this.gvWHSubPre.IgnoreFilter = false;
		this.gvWHSubPre.IndexEditRow = -1;
		this.gvWHSubPre.IndicatorWidth = 40;
		this.gvWHSubPre.IsApzTable = false;
		this.gvWHSubPre.IsInsertNewRow = false;
		this.gvWHSubPre.IsLoadingData = false;
		this.gvWHSubPre.IsMappingContextAction = false;
		this.gvWHSubPre.ItemId = null;
		this.gvWHSubPre.LstColumnAllowBatch = null;
		this.gvWHSubPre.LstColumnNotAllowBatch = null;
		this.gvWHSubPre.LstColumnNotEditable = null;
		this.gvWHSubPre.LstColumNotSetting = null;
		this.gvWHSubPre.Name = "gvWHSubPre";
		this.gvWHSubPre.OptionsBehavior.ReadOnly = true;
		this.gvWHSubPre.OptionsCustomization.AllowSort = false;
		this.gvWHSubPre.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gvWHSubPre.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gvWHSubPre.OptionsView.ShowGroupPanel = false;
		this.gvWHSubPre.UseBatchMod = false;
		this.gvWHSubPre.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvWHSub_CustomDrawRowIndicator);
		this.gvWHSubPre.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvWHSubPre_RowStyle);
		this.colCode1.Caption = "Code";
		this.colCode1.CurrencyDateField = null;
		this.colCode1.CurrencyField = null;
		this.colCode1.CurrencyFixed = null;
		this.colCode1.CurrencyFixedRate = 0.0;
		this.colCode1.CurrencyRateField = null;
		this.colCode1.FieldName = "SLCode";
		this.colCode1.Name = "colCode1";
		this.colCode1.OptionsColumn.AllowFocus = false;
		this.colCode1.OptionsColumn.ReadOnly = true;
		this.colCode1.PrimaryKey = true;
		this.colCode1.UnitField = null;
		this.colCode1.Visible = true;
		this.colCode1.VisibleIndex = 0;
		this.colCode1.Width = 194;
		this.colDes.Caption = "Description";
		this.colDes.CurrencyDateField = null;
		this.colDes.CurrencyField = null;
		this.colDes.CurrencyFixed = null;
		this.colDes.CurrencyFixedRate = 0.0;
		this.colDes.CurrencyRateField = null;
		this.colDes.FieldName = "Descr";
		this.colDes.Name = "colDes";
		this.colDes.OptionsColumn.AllowFocus = false;
		this.colDes.OptionsColumn.ReadOnly = true;
		this.colDes.PrimaryKey = false;
		this.colDes.UnitField = null;
		this.colDes.Visible = true;
		this.colDes.VisibleIndex = 1;
		this.colDes.Width = 194;
		this.colErrDetails.AppearanceCell.Options.UseTextOptions = true;
		this.colErrDetails.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colErrDetails.Caption = "Error Details";
		this.colErrDetails.CurrencyDateField = null;
		this.colErrDetails.CurrencyField = null;
		this.colErrDetails.CurrencyFixed = null;
		this.colErrDetails.CurrencyFixedRate = 0.0;
		this.colErrDetails.CurrencyRateField = null;
		this.colErrDetails.FieldName = "Error";
		this.colErrDetails.Name = "colErrDetails";
		this.colErrDetails.OptionsColumn.AllowFocus = false;
		this.colErrDetails.OptionsColumn.ReadOnly = true;
		this.colErrDetails.PrimaryKey = false;
		this.colErrDetails.UnitField = null;
		this.colErrDetails.Visible = true;
		this.colErrDetails.VisibleIndex = 2;
		this.colErrDetails.Width = 198;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(94, 322);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "Generate";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(13, 322);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "Generate";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.txtWHSub.EditControl = null;
		this.txtWHSub.FieldName = null;
		this.txtWHSub.IsSearchControl = false;
		this.txtWHSub.Location = new System.Drawing.Point(155, 9);
		this.txtWHSub.Name = "txtWHSub";
		this.txtWHSub.Properties.AllowFocused = false;
		this.txtWHSub.Properties.ReadOnly = true;
		this.txtWHSub.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.txtWHSub.SettingInVisible = false;
		this.txtWHSub.SettingReadonly = false;
		this.txtWHSub.Size = new System.Drawing.Size(224, 20);
		this.txtWHSub.TabIndex = 3;
		this.lblWHSub.Location = new System.Drawing.Point(13, 16);
		this.lblWHSub.Name = "lblWHSub";
		this.lblWHSub.Size = new System.Drawing.Size(0, 13);
		this.lblWHSub.TabIndex = 2;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Warehouse Sublevel";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 9);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(136, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 7;
		this.labelLine1.Utext = "Warehouse Sublevel";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(650, 357);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.lblWHSub);
		base.Controls.Add(this.grdWHSubPre);
		base.Controls.Add(this.txtWHSub);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WHSubSetupPreview";
		this.Text = "Warehouse Sublevel Codes Management - Generation Preview";
		base.Load += new System.EventHandler(this.frm_WHSubSetupPreview_Load);
		((System.ComponentModel.ISupportInitialize)this.grdWHSubPre).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvWHSubPre).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtWHSub.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
