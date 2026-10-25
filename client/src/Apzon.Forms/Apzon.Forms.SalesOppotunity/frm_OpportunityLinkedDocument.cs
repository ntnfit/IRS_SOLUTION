using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.SalesOppotunity;

public class frm_OpportunityLinkedDocument : XtraFormEx
{
	public DataTable DTDocNumAndType = new DataTable();

	public string CardCode = "";

	public string CardName = "";

	public int OpprId;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btn_Close;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private LabelLine labelLine1;

	private TextEditEx txt_CardCode;

	private TextEditEx txt_CardName;

	private LabelLine labelLine2;

	private TextEditEx txt_OpprId;

	private LabelLine labelLine3;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEdit rep_bte_DocNum;

	private GridColumnEx gridColumnEx5;

	public frm_OpportunityLinkedDocument()
	{
		InitializeComponent();
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void rep_bte_DocNum_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocType"));
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			string[] array = text.Split(' ');
			if (array != null && array.Length != 0)
			{
				switch (array[0])
				{
				case "QU":
					this.DrillDown(ApzObjectTypes.oQuotations, num.ToString());
					break;
				case "OR":
					this.DrillDown(ApzObjectTypes.oOrders, num.ToString());
					break;
				case "DN":
					this.DrillDown(ApzObjectTypes.oDeliveryNotes, num.ToString());
					break;
				case "IN":
					this.DrillDown(ApzObjectTypes.oInvoices, num.ToString());
					break;
				case "PQ":
					this.DrillDown(ApzObjectTypes.oPurchaseQuotations, num.ToString());
					break;
				case "PO":
					this.DrillDown(ApzObjectTypes.oPurchaseOrders, num.ToString());
					break;
				case "PD":
					this.DrillDown(ApzObjectTypes.oPurchaseDeliveryNotes, num.ToString());
					break;
				case "PU":
					this.DrillDown(ApzObjectTypes.oPurchaseInvoices, num.ToString());
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_OpportunityLinkedDocument_Load(object sender, EventArgs e)
	{
		try
		{
			txt_OpprId.Properties.Mask.MaskType = MaskType.Numeric;
			txt_OpprId.Properties.Mask.EditMask = "n0";
			txt_OpprId.Properties.Mask.UseMaskAsDisplayFormat = true;
			txt_CardCode.Text = CardCode;
			txt_CardName.Text = CardName;
			txt_OpprId.Text = OpprId.ToString();
			DataTable linkedDocuments = _httpClient.OpportunityClient.GetLinkedDocuments(DTDocNumAndType);
			grd_Doc.DataSource = linkedDocuments;
			grd_Doc.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_OpportunityLinkedDocument));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_DocNum = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CardCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_CardName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_OpprId = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DocNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OpprId.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(6, 307);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 4;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(6, 57);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_bte_DocNum });
		this.grd_Doc.Size = new System.Drawing.Size(555, 245);
		this.grd_Doc.TabIndex = 3;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oOpportunityLinkedDocument";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gridColumnEx1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumnEx1.AppearanceCell.Options.UseBackColor = true;
		this.gridColumnEx1.Caption = "Date";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocDate";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 85;
		this.gridColumnEx2.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumnEx2.AppearanceCell.Options.UseBackColor = true;
		this.gridColumnEx2.Caption = "Document";
		this.gridColumnEx2.ColumnEdit = this.rep_bte_DocNum;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "DocType";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 95;
		this.rep_bte_DocNum.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_bte_DocNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_DocNum.Name = "rep_bte_DocNum";
		this.rep_bte_DocNum.ReadOnly = true;
		this.rep_bte_DocNum.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_DocNum_ButtonPressed);
		this.gridColumnEx3.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumnEx3.AppearanceCell.Options.UseBackColor = true;
		this.gridColumnEx3.Caption = "Remarks";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Comments";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.AllowEdit = false;
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 259;
		this.gridColumnEx4.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumnEx4.AppearanceCell.Options.UseBackColor = true;
		this.gridColumnEx4.Caption = "Total (LC)";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "DocTotal";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.AllowEdit = false;
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 88;
		this.gridColumnEx5.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumnEx5.AppearanceCell.Options.UseBackColor = true;
		this.gridColumnEx5.Caption = "DocEntry";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DocEntry";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.AllowEdit = false;
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(6, 9);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 22;
		this.labelLine1.Utext = "BP Code";
		this.txt_CardCode.EditControl = null;
		this.txt_CardCode.FieldName = null;
		this.txt_CardCode.IsSearchControl = false;
		this.txt_CardCode.Location = new System.Drawing.Point(118, 6);
		this.txt_CardCode.Name = "txt_CardCode";
		this.txt_CardCode.Properties.ReadOnly = true;
		this.txt_CardCode.SettingInVisible = false;
		this.txt_CardCode.SettingReadonly = false;
		this.txt_CardCode.Size = new System.Drawing.Size(207, 20);
		this.txt_CardCode.TabIndex = 0;
		this.txt_CardName.EditControl = null;
		this.txt_CardName.FieldName = null;
		this.txt_CardName.IsSearchControl = false;
		this.txt_CardName.Location = new System.Drawing.Point(118, 32);
		this.txt_CardName.Name = "txt_CardName";
		this.txt_CardName.Properties.ReadOnly = true;
		this.txt_CardName.SettingInVisible = false;
		this.txt_CardName.SettingReadonly = false;
		this.txt_CardName.Size = new System.Drawing.Size(207, 20);
		this.txt_CardName.TabIndex = 1;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(6, 35);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(117, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 24;
		this.labelLine2.Utext = "BP Name";
		this.txt_OpprId.EditControl = null;
		this.txt_OpprId.FieldName = null;
		this.txt_OpprId.IsSearchControl = false;
		this.txt_OpprId.Location = new System.Drawing.Point(468, 6);
		this.txt_OpprId.Name = "txt_OpprId";
		this.txt_OpprId.Properties.ReadOnly = true;
		this.txt_OpprId.SettingInVisible = false;
		this.txt_OpprId.SettingReadonly = false;
		this.txt_OpprId.Size = new System.Drawing.Size(93, 20);
		this.txt_OpprId.TabIndex = 2;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Opportunity No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(363, 9);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(106, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 26;
		this.labelLine3.Utext = "Opportunity No.";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(566, 334);
		base.Controls.Add(this.txt_OpprId);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_CardName);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_CardCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Close);
		base.FormId = "oOpportunityLinkedDocument";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_OpportunityLinkedDocument";
		this.Text = "Linked Document";
		base.Load += new System.EventHandler(this.frm_OpportunityLinkedDocument_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DocNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OpprId.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
