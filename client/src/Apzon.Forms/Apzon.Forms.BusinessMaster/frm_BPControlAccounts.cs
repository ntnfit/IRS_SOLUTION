using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.BusinessMaster;

public class frm_BPControlAccounts : XtraFormEx
{
	public bool IsOKClick;

	public bool IsChangeValue;

	public DataTable DTSource = new DataTable();

	public string BPType = "";

	private IContainer components;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grdEx_Accounts;

	private GridViewEx gvEx_Accounts;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEdit rep_bte_AcctCode;

	private RepositoryItemButtonEditCFL rep_bte_CO;

	private RepositoryItemButtonEditCFL rep_bte_D;

	private RepositoryItemButtonEditCFL rep_bte_SO;

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_BPControlAccounts()
	{
		InitializeComponent();
	}

	private void frm_BPControlAccounts_Load(object sender, EventArgs e)
	{
		try
		{
			LoadRepostoryCFLDataSource();
			grdEx_Accounts.DataSource = DTSource;
			grdEx_Accounts.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Accounts_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvEx_Accounts.FocusedColumn.FieldName != "AcctCode")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Accounts_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "AcctCode")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Accounts_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "AcctCode")
			{
				return;
			}
			string text = Function.ToString(gvEx_Accounts.GetRowCellValue(e.RowHandle, "AcctType"));
			if (text == "D")
			{
				e.RepositoryItem = rep_bte_D;
			}
			else if (text == "O")
			{
				if (BPType == "C")
				{
					e.RepositoryItem = rep_bte_CO;
				}
				else if (BPType == "S")
				{
					e.RepositoryItem = rep_bte_SO;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Accounts_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			IsOKClick = false;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.View;
				return;
			}
			IsOKClick = true;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_AcctCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			Enums.TypeCFL type = Enums.TypeCFL.AcctCode;
			string text = Function.ToString(gvEx_Accounts.GetFocusedRowCellValue("AcctType"));
			if (text == "D")
			{
				type = Enums.TypeCFL.AcctBPDownPayment;
			}
			else if (text == "O")
			{
				if (BPType == "C")
				{
					type = Enums.TypeCFL.AcctCusBPOpenDebts;
				}
				else if (BPType == "S")
				{
					type = Enums.TypeCFL.AcctVenBPOpenDebts;
				}
			}
			frm_CFL frm_CFL2 = new frm_CFL(type, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvEx_Accounts.SetFocusedRowCellValue("AcctCode", Function.ToString(frm_CFL2.ChooseItem["AcctCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadRepostoryCFLDataSource()
	{
		try
		{
			rep_bte_D.ValueMember = "AcctCode";
			rep_bte_D.TypeCFL = Enums.TypeCFL.AcctBPDownPayment;
			rep_bte_D.FieldNameInDataRow("AcctCode", "AcctName");
			rep_bte_D.RefreshDataSource();
			rep_bte_CO.ValueMember = "AcctCode";
			rep_bte_CO.TypeCFL = Enums.TypeCFL.AcctCusBPOpenDebts;
			rep_bte_CO.FieldNameInDataRow("AcctCode", "AcctName");
			rep_bte_CO.RefreshDataSource();
			rep_bte_SO.ValueMember = "AcctCode";
			rep_bte_SO.TypeCFL = Enums.TypeCFL.AcctVenBPOpenDebts;
			rep_bte_SO.FieldNameInDataRow("AcctCode", "AcctName");
			rep_bte_SO.RefreshDataSource();
			if (!grdEx_Accounts.RepositoryItems.Contains(rep_bte_D))
			{
				grdEx_Accounts.RepositoryItems.Add(rep_bte_D);
			}
			if (!grdEx_Accounts.RepositoryItems.Contains(rep_bte_SO))
			{
				grdEx_Accounts.RepositoryItems.Add(rep_bte_SO);
			}
			if (!grdEx_Accounts.RepositoryItems.Contains(rep_bte_CO))
			{
				grdEx_Accounts.RepositoryItems.Add(rep_bte_CO);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Accounts_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			IsChangeValue = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_BPControlAccounts));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_Accounts = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_Accounts = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_AcctCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_CO = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_bte_D = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_bte_SO = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Accounts).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Accounts).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_AcctCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_CO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_D).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_SO).BeginInit();
		base.SuspendLayout();
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 240);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(70, 20);
		this.btn_OK.TabIndex = 12;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(76, 240);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.grdEx_Accounts.AllowPasteExcel = true;
		this.grdEx_Accounts.ClipboardData = null;
		this.grdEx_Accounts.Location = new System.Drawing.Point(3, 2);
		this.grdEx_Accounts.MainView = this.gvEx_Accounts;
		this.grdEx_Accounts.Name = "grdEx_Accounts";
		this.grdEx_Accounts.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_bte_AcctCode, this.rep_bte_CO, this.rep_bte_D, this.rep_bte_SO });
		this.grdEx_Accounts.Size = new System.Drawing.Size(449, 232);
		this.grdEx_Accounts.TabIndex = 14;
		this.grdEx_Accounts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Accounts });
		this.gvEx_Accounts.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Accounts.ColumnKey = null;
		this.gvEx_Accounts.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4 });
		this.gvEx_Accounts.DisableFormSetting = false;
		this.gvEx_Accounts.DisplayException = false;
		this.gvEx_Accounts.EditColumn = null;
		this.gvEx_Accounts.FieldCurrency = null;
		this.gvEx_Accounts.FieldRate = null;
		this.gvEx_Accounts.FormId = "oControlAccount";
		this.gvEx_Accounts.GridControl = this.grdEx_Accounts;
		this.gvEx_Accounts.IgnoreFilter = false;
		this.gvEx_Accounts.IndexEditRow = -1;
		this.gvEx_Accounts.IndicatorWidth = 35;
		this.gvEx_Accounts.IsApzTable = false;
		this.gvEx_Accounts.IsInsertNewRow = false;
		this.gvEx_Accounts.IsLoadingData = false;
		this.gvEx_Accounts.IsMappingContextAction = false;
		this.gvEx_Accounts.ItemId = "OCAR";
		this.gvEx_Accounts.LstColumnAllowBatch = null;
		this.gvEx_Accounts.LstColumnNotAllowBatch = null;
		this.gvEx_Accounts.LstColumnNotEditable = null;
		this.gvEx_Accounts.LstColumNotSetting = null;
		this.gvEx_Accounts.Name = "gvEx_Accounts";
		this.gvEx_Accounts.OptionsView.BestFitMaxRowCount = 100;
		this.gvEx_Accounts.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Accounts.OptionsView.ShowGroupPanel = false;
		this.gvEx_Accounts.UseBatchMod = false;
		this.gvEx_Accounts.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_Accounts_RowCellStyle);
		this.gvEx_Accounts.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvEx_Accounts_CustomRowCellEdit);
		this.gvEx_Accounts.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_Accounts_ShowingEditor);
		this.gvEx_Accounts.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_Accounts_CellValueChanged);
		this.gridColumnEx1.Caption = "Account Type Name";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AcctTypeName";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 154;
		this.gridColumnEx2.Caption = "Account Code";
		this.gridColumnEx2.ColumnEdit = this.rep_bte_AcctCode;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "AcctCode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 76;
		this.rep_bte_AcctCode.AutoHeight = false;
		this.rep_bte_AcctCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_bte_AcctCode.Name = "rep_bte_AcctCode";
		this.gridColumnEx3.Caption = "Account Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "AcctName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 176;
		this.gridColumnEx4.Caption = "AcctType";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "AcctType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.rep_bte_CO.AutoHeight = false;
		this.rep_bte_CO.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_bte_CO.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_CO.ContainerControl = this;
		this.rep_bte_CO.DisplayMember = "";
		this.rep_bte_CO.DisplayMember2 = "";
		this.rep_bte_CO.LoadDataSourceWhenEnter = false;
		this.rep_bte_CO.Name = "rep_bte_CO";
		this.rep_bte_CO.SeparateCharacterForMulti = ';';
		this.rep_bte_CO.SeparateString = ", ";
		this.rep_bte_CO.UsingAutoCompleteBox = false;
		this.rep_bte_CO.UsingDataSource = true;
		this.rep_bte_CO.ValidateWhenNoDataSource = false;
		this.rep_bte_CO.ValueMember = "";
		this.rep_bte_D.AutoHeight = false;
		this.rep_bte_D.AutoLoadDataSource = true;
		this.rep_bte_D.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_D.ContainerControl = this;
		this.rep_bte_D.DisplayMember = "";
		this.rep_bte_D.DisplayMember2 = "";
		this.rep_bte_D.LoadDataSourceWhenEnter = false;
		this.rep_bte_D.Name = "rep_bte_D";
		this.rep_bte_D.SeparateCharacterForMulti = ';';
		this.rep_bte_D.SeparateString = ", ";
		this.rep_bte_D.UsingAutoCompleteBox = false;
		this.rep_bte_D.UsingDataSource = true;
		this.rep_bte_D.ValidateWhenNoDataSource = false;
		this.rep_bte_D.ValueMember = "";
		this.rep_bte_SO.AutoHeight = false;
		this.rep_bte_SO.AutoLoadDataSource = true;
		this.rep_bte_SO.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_SO.ContainerControl = this;
		this.rep_bte_SO.DisplayMember = "";
		this.rep_bte_SO.DisplayMember2 = "";
		this.rep_bte_SO.LoadDataSourceWhenEnter = false;
		this.rep_bte_SO.Name = "rep_bte_SO";
		this.rep_bte_SO.SeparateCharacterForMulti = ';';
		this.rep_bte_SO.SeparateString = ", ";
		this.rep_bte_SO.UsingAutoCompleteBox = false;
		this.rep_bte_SO.UsingDataSource = true;
		this.rep_bte_SO.ValidateWhenNoDataSource = false;
		this.rep_bte_SO.ValueMember = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(455, 266);
		base.Controls.Add(this.grdEx_Accounts);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oControlAccount";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BPControlAccounts";
		this.Text = "Control Accounts -";
		base.Load += new System.EventHandler(this.frm_BPControlAccounts_Load);
		((System.ComponentModel.ISupportInitialize)this.grdEx_Accounts).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Accounts).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_AcctCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_CO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_D).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_SO).EndInit();
		base.ResumeLayout(false);
	}
}
