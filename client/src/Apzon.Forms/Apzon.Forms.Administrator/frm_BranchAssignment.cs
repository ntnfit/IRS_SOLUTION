using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_BranchAssignment : XtraFormEx
{
	private DataTable _dtSource = new DataTable();

	private string _code = "";

	private bool _changed;

	private ApzObjectTypes _objType;

	private IHttpClient _unitOfWork = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Add;

	private GridControlEx grd_Assignment;

	private GridViewEx gv_Assignment;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemCheckEdit rep_Checked;

	private RepositoryItemCheckEdit rep_Disabled;

	public DataTable DtSource => _dtSource;

	public string Code => _code;

	public bool Changed => _changed;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Add.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Add.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_BranchAssignment(DataTable dtSource, string code, ApzObjectTypes objType)
	{
		try
		{
			_dtSource = dtSource.Copy();
			_code = code;
			_objType = objType;
			InitializeComponent();
			OnLoadAsync += frm_BranchAssignment_Load;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_BranchAssignment_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (_dtSource == null || _dtSource.Rows.Count == 0)
			{
				if (_objType == ApzObjectTypes.oUsers)
				{
					_dtSource = await _unitOfWork.User.GetUserBranchAssignment(_code);
				}
				else if (_objType == ApzObjectTypes.oBusinessPartners)
				{
					_dtSource = await _unitOfWork.BusinessMaster.GetBpBranchAssignment(_code);
				}
			}
			grd_Assignment.DataSource = _dtSource.Copy();
			grd_Assignment.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Add_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			_changed = true;
			_dtSource = grd_Assignment.DataSource as DataTable;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Assignment_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = StyleColor.GridDisable1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Assignment_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_BranchAssignment));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Assignment = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Assignment = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Checked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Disabled = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Assignment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Assignment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Disabled).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 255);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(3, 255);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(75, 20);
		this.btn_Add.TabIndex = 13;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.grd_Assignment.AllowPasteExcel = true;
		this.grd_Assignment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Assignment.ClipboardData = null;
		this.grd_Assignment.Location = new System.Drawing.Point(3, 3);
		this.grd_Assignment.MainView = this.gv_Assignment;
		this.grd_Assignment.Name = "grd_Assignment";
		this.grd_Assignment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Checked, this.rep_Disabled });
		this.grd_Assignment.Size = new System.Drawing.Size(799, 248);
		this.grd_Assignment.TabIndex = 15;
		this.grd_Assignment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Assignment });
		this.gv_Assignment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Assignment.ColumnKey = null;
		this.gv_Assignment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8 });
		this.gv_Assignment.DisableFormSetting = false;
		this.gv_Assignment.DisplayException = false;
		this.gv_Assignment.EditColumn = null;
		this.gv_Assignment.FieldCurrency = null;
		this.gv_Assignment.FieldRate = null;
		this.gv_Assignment.FormId = "oBranchAssignment";
		this.gv_Assignment.GridControl = this.grd_Assignment;
		this.gv_Assignment.IgnoreFilter = false;
		this.gv_Assignment.IndexEditRow = -1;
		this.gv_Assignment.IndicatorWidth = 35;
		this.gv_Assignment.IsApzTable = false;
		this.gv_Assignment.IsInsertNewRow = false;
		this.gv_Assignment.IsLoadingData = false;
		this.gv_Assignment.IsMappingContextAction = false;
		this.gv_Assignment.ItemId = "CRD8";
		this.gv_Assignment.LstColumnAllowBatch = null;
		this.gv_Assignment.LstColumnNotAllowBatch = null;
		this.gv_Assignment.LstColumnNotEditable = null;
		this.gv_Assignment.LstColumNotSetting = null;
		this.gv_Assignment.Name = "gv_Assignment";
		this.gv_Assignment.OptionsCustomization.AllowSort = false;
		this.gv_Assignment.OptionsNavigation.UseTabKey = false;
		this.gv_Assignment.OptionsView.ColumnAutoWidth = false;
		this.gv_Assignment.OptionsView.ShowGroupPanel = false;
		this.gv_Assignment.UseBatchMod = false;
		this.gv_Assignment.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Assignment_RowCellStyle);
		this.gv_Assignment.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Assignment_CellValueChanging);
		this.gridColumnEx1.Caption = "Branch Name";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "BPLName";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Alias Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "AliasName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "County";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "County";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx4.Caption = "State";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "State";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx5.Caption = "Country";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Country";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx6.Caption = "Disabled";
		this.gridColumnEx6.ColumnEdit = this.rep_Checked;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Disabled";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.rep_Checked.AutoHeight = false;
		this.rep_Checked.DisplayValueChecked = "Y";
		this.rep_Checked.DisplayValueGrayed = "N";
		this.rep_Checked.DisplayValueUnchecked = "N";
		this.rep_Checked.Name = "rep_Checked";
		this.rep_Checked.ValueChecked = "Y";
		this.rep_Checked.ValueGrayed = "N";
		this.rep_Checked.ValueUnchecked = "N";
		this.gridColumnEx7.Caption = "Assigned";
		this.gridColumnEx7.ColumnEdit = this.rep_Disabled;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Checked";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.rep_Disabled.AutoHeight = false;
		this.rep_Disabled.DisplayValueChecked = "Y";
		this.rep_Disabled.DisplayValueGrayed = "N";
		this.rep_Disabled.DisplayValueUnchecked = "N";
		this.rep_Disabled.Name = "rep_Disabled";
		this.rep_Disabled.ValueChecked = "Y";
		this.rep_Disabled.ValueGrayed = "N";
		this.rep_Disabled.ValueUnchecked = "N";
		this.gridColumnEx8.Caption = "gridColumnEx8";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "BPLId";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(805, 279);
		base.Controls.Add(this.grd_Assignment);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.FormId = "oBranchAssignment";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BranchAssignment";
		this.Text = "Branch Assignment";
		((System.ComponentModel.ISupportInitialize)this.grd_Assignment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Assignment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Disabled).EndInit();
		base.ResumeLayout(false);
	}
}
