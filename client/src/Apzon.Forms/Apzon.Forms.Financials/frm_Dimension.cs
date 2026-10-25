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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Financials;

public class frm_Dimension : XtraFormEx
{
	private string _privilege = "";

	private string _Controller = "Dimension";

	private string[] _disableDim = new string[5] { "N", "N", "N", "N", "N" };

	private IContainer components;

	private GridControlEx grd_ODIM;

	private GridViewEx gv_ODIM;

	private GridColumnEx gcDimCode;

	private GridColumnEx gcDimName;

	private GridColumnEx gcDimActive;

	private GridColumnEx gcDimDesc;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			bt_Cancel.Enabled = true;
			return;
		}
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

	public frm_Dimension()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Dimension"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Dimension" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void gv_ODIM_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "DimCode")
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void bt_Cancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			await Save();
		}
		Close();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else
		{
			await Save();
		}
	}

	private void gv_ODIM_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_ODIM.GetFocusedRowCellValue("DimCode"));
			if ((num > 0 && _disableDim[num - 1] == "Y" && gv_ODIM.FocusedColumn.FieldName.ToUpper() == "DIMACTIVE") || gv_ODIM.FocusedColumn.FieldName.ToUpper() == "DIMNAME")
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ODIM_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_ODIM.GetRowCellValue(e.RowHandle, "DimCode"));
			if ((num > 0 && _disableDim[num - 1] == "Y" && e.Column.FieldName.ToUpper() == "DIMACTIVE") || e.Column.FieldName.ToUpper() == "DIMNAME")
			{
				e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_Dimensison_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> _hp_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller))
			{
				DataTable dataSource = await _hp_1.GetAsync("GetData");
				grd_ODIM.DataSource = dataSource;
				grd_ODIM.RefreshDataSource();
			}
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable = await _hp.GetAsync("GetDisableDim");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				_disableDim = Function.ToString(dataTable.Rows[0][0]).Split(',');
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable pr = grd_ODIM.DataSource as DataTable;
			using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, _Controller))
			{
				if (string.IsNullOrEmpty(await helper.PostAsync("Save", pr)))
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.SaveError, "");
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_Dimension));
		this.grd_ODIM = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ODIM = new Apzon.Controls.Grid.GridViewEx();
		this.gcDimCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDimName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDimActive = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcDimDesc = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_ODIM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ODIM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_ODIM.AllowPasteExcel = true;
		this.grd_ODIM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ODIM.ClipboardData = null;
		this.grd_ODIM.Location = new System.Drawing.Point(1, 2);
		this.grd_ODIM.MainView = this.gv_ODIM;
		this.grd_ODIM.Name = "grd_ODIM";
		this.grd_ODIM.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCheckEdit1 });
		this.grd_ODIM.Size = new System.Drawing.Size(390, 189);
		this.grd_ODIM.TabIndex = 0;
		this.grd_ODIM.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ODIM });
		this.gv_ODIM.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ODIM.ColumnKey = null;
		this.gv_ODIM.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcDimCode, this.gcDimName, this.gcDimActive, this.gcDimDesc });
		this.gv_ODIM.DisableFormSetting = false;
		this.gv_ODIM.DisplayException = false;
		this.gv_ODIM.EditColumn = null;
		this.gv_ODIM.FieldCurrency = null;
		this.gv_ODIM.FieldRate = null;
		this.gv_ODIM.FormId = "oDimensions";
		this.gv_ODIM.GridControl = this.grd_ODIM;
		this.gv_ODIM.IgnoreFilter = false;
		this.gv_ODIM.IndexEditRow = -1;
		this.gv_ODIM.IndicatorWidth = 35;
		this.gv_ODIM.IsApzTable = false;
		this.gv_ODIM.IsInsertNewRow = false;
		this.gv_ODIM.IsLoadingData = false;
		this.gv_ODIM.IsMappingContextAction = false;
		this.gv_ODIM.ItemId = "ODIM";
		this.gv_ODIM.LstColumnAllowBatch = null;
		this.gv_ODIM.LstColumnNotAllowBatch = null;
		this.gv_ODIM.LstColumnNotEditable = null;
		this.gv_ODIM.LstColumNotSetting = null;
		this.gv_ODIM.Name = "gv_ODIM";
		this.gv_ODIM.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gv_ODIM.OptionsView.ShowGroupPanel = false;
		this.gv_ODIM.UseBatchMod = false;
		this.gv_ODIM.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_ODIM_RowCellStyle);
		this.gv_ODIM.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ODIM_ShowingEditor);
		this.gv_ODIM.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ODIM_CellValueChanging);
		this.gcDimCode.Caption = "DimCode";
		this.gcDimCode.CurrencyDateField = null;
		this.gcDimCode.CurrencyField = null;
		this.gcDimCode.CurrencyFixed = null;
		this.gcDimCode.CurrencyFixedRate = 0.0;
		this.gcDimCode.CurrencyRateField = null;
		this.gcDimCode.FieldName = "DimCode";
		this.gcDimCode.Name = "gcDimCode";
		this.gcDimCode.PrimaryKey = true;
		this.gcDimCode.UnitField = null;
		this.gcDimName.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.gcDimName.AppearanceCell.Options.UseBackColor = true;
		this.gcDimName.Caption = "Name";
		this.gcDimName.CurrencyDateField = null;
		this.gcDimName.CurrencyField = null;
		this.gcDimName.CurrencyFixed = null;
		this.gcDimName.CurrencyFixedRate = 0.0;
		this.gcDimName.CurrencyRateField = null;
		this.gcDimName.FieldName = "DimName";
		this.gcDimName.Name = "gcDimName";
		this.gcDimName.OptionsColumn.AllowEdit = false;
		this.gcDimName.OptionsColumn.AllowFocus = false;
		this.gcDimName.OptionsColumn.ReadOnly = true;
		this.gcDimName.PrimaryKey = false;
		this.gcDimName.UnitField = null;
		this.gcDimName.Visible = true;
		this.gcDimName.VisibleIndex = 0;
		this.gcDimName.Width = 124;
		this.gcDimActive.Caption = "Active";
		this.gcDimActive.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcDimActive.CurrencyDateField = null;
		this.gcDimActive.CurrencyField = null;
		this.gcDimActive.CurrencyFixed = null;
		this.gcDimActive.CurrencyFixedRate = 0.0;
		this.gcDimActive.CurrencyRateField = null;
		this.gcDimActive.FieldName = "DimActive";
		this.gcDimActive.Name = "gcDimActive";
		this.gcDimActive.PrimaryKey = false;
		this.gcDimActive.UnitField = null;
		this.gcDimActive.Visible = true;
		this.gcDimActive.VisibleIndex = 1;
		this.gcDimActive.Width = 65;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit1.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gcDimDesc.Caption = "Description";
		this.gcDimDesc.CurrencyDateField = null;
		this.gcDimDesc.CurrencyField = null;
		this.gcDimDesc.CurrencyFixed = null;
		this.gcDimDesc.CurrencyFixedRate = 0.0;
		this.gcDimDesc.CurrencyRateField = null;
		this.gcDimDesc.FieldName = "DimDesc";
		this.gcDimDesc.Name = "gcDimDesc";
		this.gcDimDesc.PrimaryKey = false;
		this.gcDimDesc.UnitField = null;
		this.gcDimDesc.Visible = true;
		this.gcDimDesc.VisibleIndex = 2;
		this.gcDimDesc.Width = 184;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(1, 195);
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
		this.bt_Cancel.Location = new System.Drawing.Point(79, 195);
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
		base.ClientSize = new System.Drawing.Size(392, 221);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_ODIM);
		base.FormId = "oDimensions";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Dimension";
		this.Text = "Dimensions";
		base.Load += new System.EventHandler(this.frm_Dimensison_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ODIM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ODIM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
