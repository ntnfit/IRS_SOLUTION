using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_ReferencedDocument : XtraFormEx
{
	private IContainer components;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx1;

	private XtraTabPageEx xtraTabPageEx2;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Ref;

	private GridViewEx gv_Ref;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_TransType;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemButtonEditCFL rep_DocNum;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridControlEx grd_RefBy;

	private GridViewEx gridViewEx1;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemLookUpEdit rep_RefTrans;

	private RepositoryItemButtonEdit rep_RefDoc;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	public int DocEntry { get; set; }

	public ApzObjectTypes DocumentType { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public bool Changed { get; set; }

	public DataTable Source { get; set; }

	private IHttpClient httpClient { get; set; }

	public frm_ReferencedDocument()
	{
		InitializeComponent();
	}

	private async void frm_ReferencedDocument_Load(object sender, EventArgs e)
	{
		try
		{
			httpClient = HttpClientHelperFactory.GetHttpClient(DocumentType);
			rep_TransType.SetDataSourceObject();
			rep_RefTrans.SetDataSourceObject();
			DataTable dataSource = await httpClient.Documents.GetReferencedDocumentBy(DocEntry);
			grd_RefBy.DataSource = dataSource;
			grd_RefBy.RefreshDataSource();
			grd_Ref.DataSource = Source.Copy();
			grd_Ref.RefreshDataSource();
			AddNewBlank();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddNewBlank()
	{
		try
		{
			DataTable dataSourceTable = grd_Ref.DataSourceTable;
			if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["RefObjType"])) || Function.ParseInt(t["RefDocNum"]) == 0) == null)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["RefObjType"] = (int)DocumentType;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		if (Changed)
		{
			Source = grd_Ref.DataSourceTable.Copy();
			DialogResult = DialogResult.OK;
		}
		else
		{
			DialogResult = DialogResult.Cancel;
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
	}

	private void gv_Ref_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		Changed = true;
		AddNewBlank();
	}

	private void rep_DocNum_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Ref.GetFocusedRowCellValue("RefDocEntr"));
			ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(gv_Ref.GetFocusedRowCellValue("RefObjType"));
			if (e.Button.Index == 0)
			{
				frm_ChooseDocuments frm_ChooseDocuments2 = new frm_ChooseDocuments();
				frm_ChooseDocuments2.IgnoreEntries = new List<int> { DocEntry };
				frm_ChooseDocuments2.DocType = apzObjectTypes;
				frm_ChooseDocuments2.FromDate = FromDate;
				frm_ChooseDocuments2.ToDate = ToDate;
				if (frm_ChooseDocuments2.ShowDialog(this) != DialogResult.OK)
				{
					return;
				}
				{
					foreach (Tuple<int, int> chooseItem in frm_ChooseDocuments2.ChooseItems)
					{
						gv_Ref.SetFocusedRowCellValue("RefDocEntr", chooseItem.Item1);
						gv_Ref.SetFocusedRowCellValue("RefDocNum", chooseItem.Item2);
						gv_Ref.SetFocusedRowCellValue("RefObjType", (int)apzObjectTypes);
						gv_Ref.FocusedRowHandle += 1;
					}
					return;
				}
			}
			this.DrillDown(apzObjectTypes, apzObjectTypes, num.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_RefDoc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Ref.GetFocusedRowCellValue("DocEntry"));
			ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(gv_Ref.GetFocusedRowCellValue("ObjectType"));
			this.DrillDown(apzObjectTypes, apzObjectTypes, num.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Ref.DeleteSelectedRows();
			grd_Ref.DataSourceTable.AcceptChanges();
			Changed = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_ReferencedDocument));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Ref = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Ref = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TransType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocNum = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_RefBy = new Apzon.Controls.Grid.GridControlEx();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_RefTrans = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_RefDoc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Ref).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Ref).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocNum).BeginInit();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_RefBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RefTrans).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RefDoc).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.xtraTabControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(800, 413);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx1, this.xtraTabPageEx2 });
		this.xtraTabPageEx1.Controls.Add(this.grd_Ref);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Document Referenced To";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(798, 388);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Document Referenced To";
		this.grd_Ref.AllowPasteExcel = true;
		this.grd_Ref.ClipboardData = null;
		this.grd_Ref.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Ref.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Ref.Location = new System.Drawing.Point(0, 0);
		this.grd_Ref.MainView = this.gv_Ref;
		this.grd_Ref.Name = "grd_Ref";
		this.grd_Ref.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_TransType, this.rep_DocNum });
		this.grd_Ref.Size = new System.Drawing.Size(798, 388);
		this.grd_Ref.TabIndex = 0;
		this.grd_Ref.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Ref });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(131, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Ref.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Ref.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Ref.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Ref.ColumnKey = null;
		this.gv_Ref.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6 });
		this.gv_Ref.DisableFormSetting = false;
		this.gv_Ref.DisplayException = false;
		this.gv_Ref.EditColumn = null;
		this.gv_Ref.FieldCurrency = null;
		this.gv_Ref.FieldRate = null;
		this.gv_Ref.FormId = null;
		this.gv_Ref.GridControl = this.grd_Ref;
		this.gv_Ref.IgnoreFilter = false;
		this.gv_Ref.IndexEditRow = -1;
		this.gv_Ref.IndicatorWidth = 35;
		this.gv_Ref.IsApzTable = false;
		this.gv_Ref.IsInsertNewRow = false;
		this.gv_Ref.IsLoadingData = false;
		this.gv_Ref.IsMappingContextAction = false;
		this.gv_Ref.ItemId = null;
		this.gv_Ref.LstColumnAllowBatch = null;
		this.gv_Ref.LstColumnNotAllowBatch = null;
		this.gv_Ref.LstColumnNotEditable = null;
		this.gv_Ref.LstColumNotSetting = null;
		this.gv_Ref.Name = "gv_Ref";
		this.gv_Ref.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Ref.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Ref.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Ref.OptionsPrint.AutoWidth = false;
		this.gv_Ref.OptionsPrint.UsePrintStyles = false;
		this.gv_Ref.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Ref.OptionsView.ColumnAutoWidth = false;
		this.gv_Ref.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Ref.OptionsView.ShowGroupedColumns = true;
		this.gv_Ref.OptionsView.ShowGroupPanel = false;
		this.gv_Ref.UseBatchMod = false;
		this.gv_Ref.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Ref_CellValueChanged);
		this.gridColumnEx1.Caption = "Transaction Type";
		this.gridColumnEx1.ColumnEdit = this.rep_TransType;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "RefObjType";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 111;
		this.rep_TransType.AutoHeight = false;
		this.rep_TransType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TransType.Name = "rep_TransType";
		this.rep_TransType.NullText = "";
		this.gridColumnEx2.Caption = "Doc. Number";
		this.gridColumnEx2.ColumnEdit = this.rep_DocNum;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "RefDocNum";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = true;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 118;
		this.rep_DocNum.AutoHeight = false;
		this.rep_DocNum.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_DocNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DocNum.ContainerControl = this;
		this.rep_DocNum.DisplayMember = "";
		this.rep_DocNum.DisplayMember2 = "";
		this.rep_DocNum.LoadDataSourceWhenEnter = false;
		this.rep_DocNum.Name = "rep_DocNum";
		this.rep_DocNum.SeparateCharacterForMulti = ';';
		this.rep_DocNum.SeparateString = ", ";
		this.rep_DocNum.UsingAutoCompleteBox = false;
		this.rep_DocNum.UsingDataSource = true;
		this.rep_DocNum.ValidateWhenNoDataSource = false;
		this.rep_DocNum.ValueMember = "";
		this.rep_DocNum.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_DocNum_ButtonClick);
		this.gridColumnEx3.Caption = "Ext. Doc. Number";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ExtDocNum";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 138;
		this.gridColumnEx4.Caption = "Date";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "IssueDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 111;
		this.gridColumnEx5.Caption = "Remarks";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Remark";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 268;
		this.gridColumnEx6.Caption = "DocEntry";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "RefDocEntr";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.xtraTabPageEx2.Controls.Add(this.grd_RefBy);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Document Referenced By";
		this.xtraTabPageEx2.FieldName = "";
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(798, 388);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Document Referenced By";
		this.grd_RefBy.AllowPasteExcel = false;
		this.grd_RefBy.ClipboardData = null;
		this.grd_RefBy.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_RefBy.Location = new System.Drawing.Point(0, 0);
		this.grd_RefBy.MainView = this.gridViewEx1;
		this.grd_RefBy.Name = "grd_RefBy";
		this.grd_RefBy.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_RefDoc, this.rep_RefTrans });
		this.grd_RefBy.Size = new System.Drawing.Size(798, 388);
		this.grd_RefBy.TabIndex = 0;
		this.grd_RefBy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridViewEx1 });
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11 });
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.grd_RefBy;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsBehavior.ReadOnly = true;
		this.gridViewEx1.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.gridColumnEx7.Caption = "Transact. Type";
		this.gridColumnEx7.ColumnEdit = this.rep_RefTrans;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ObjectType";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 0;
		this.gridColumnEx7.Width = 134;
		this.rep_RefTrans.AutoHeight = false;
		this.rep_RefTrans.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_RefTrans.Name = "rep_RefTrans";
		this.rep_RefTrans.NullText = "";
		this.gridColumnEx8.Caption = "Ref. Document";
		this.gridColumnEx8.ColumnEdit = this.rep_RefDoc;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "DocNum";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 1;
		this.gridColumnEx8.Width = 126;
		this.rep_RefDoc.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_RefDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_RefDoc.Name = "rep_RefDoc";
		this.rep_RefDoc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_RefDoc_ButtonClick);
		this.gridColumnEx9.Caption = "Date";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "IssueDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 118;
		this.gridColumnEx10.Caption = "Ref. Amount";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx10.FieldName = "RefAmount";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 3;
		this.gridColumnEx10.Width = 130;
		this.gridColumnEx11.Caption = "Remark";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 4;
		this.gridColumnEx11.Width = 250;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(13, 419);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(94, 419);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.xtraTabControlEx1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ReferencedDocument.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReferencedDocument";
		base.ShowInTaskbar = false;
		this.Text = "Referenced Document";
		base.Load += new System.EventHandler(this.frm_ReferencedDocument_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Ref).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Ref).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocNum).EndInit();
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_RefBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RefTrans).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RefDoc).EndInit();
		base.ResumeLayout(false);
	}
}
