using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_ItemPreferredVendor : XtraFormEx
{
	public DataTable DTSource = new DataTable();

	public string DfltVendor = "";

	public string ItemCode = "";

	public bool IsOKClick;

	public bool IsChanged;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oItems);

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcVendorCode;

	private GridColumnEx gcObject;

	private GridColumnEx gcBpName;

	private GridColumnEx gcPriceList;

	private GridColumnEx gcItemPrice;

	private GridColumnEx gcLastPrice;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btn_SetDefault;

	private RepositoryItemButtonEditCFL rep_VendorCode;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gcLogInstance;

	private RepositoryItemLookUpEdit rPriceList;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ItemPreferredVendor()
	{
		InitializeComponent();
	}

	private void frm_ItemPreferredVendor_Load(object sender, EventArgs e)
	{
		try
		{
			DataTable groupNumPriceList = _httpClient.Items.GetGroupNumPriceList();
			rPriceList.DataSource = groupNumPriceList;
			grd_Doc.DataSource = DTSource;
			grd_Doc.RefreshDataSource();
			AddNewRow();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_VendorCode;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(CFLVendor));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "VendorCode")
			{
				if (_httpClient.BusinessMaster.CheckDoc(Function.ToString(e.Value)))
				{
					DataTable aPreferredVendorData = _httpClient.Items.GetAPreferredVendorData(ItemCode, Function.ToString(e.Value));
					if (aPreferredVendorData != null && aPreferredVendorData.Rows.Count > 0)
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "CardName", Function.ToString(aPreferredVendorData.Rows[0]["CardName"]));
						gv_Doc.SetRowCellValue(e.RowHandle, "ListNum", Function.ParseInt(aPreferredVendorData.Rows[0]["ListNum"]));
						gv_Doc.SetRowCellValue(e.RowHandle, "Price", Function.ParseDouble(aPreferredVendorData.Rows[0]["Price"]));
						gv_Doc.SetRowCellValue(e.RowHandle, "LastPurPrc", Function.ParseDouble(aPreferredVendorData.Rows[0]["LastPurPrc"]));
					}
				}
				AddNewRow();
			}
			FormMode = Enums.FormViewModeType.Update;
			IsChanged = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.FieldName != "VendorCode")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "VendorCode")
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "VendorCode"));
			if (!string.IsNullOrEmpty(text) && string.Compare(text, DfltVendor, ignoreCase: true) == 0)
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			IsChanged = false;
			IsOKClick = true;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("VendorCode"));
			if (!string.IsNullOrEmpty(Function.ToString(text)) && string.Compare(text, DfltVendor, ignoreCase: true) == 0)
			{
				DfltVendor = "";
			}
			gv_Doc.DeleteSelectedRows();
			if (dataTable != null)
			{
				FormMode = Enums.FormViewModeType.Update;
				IsChanged = true;
				dataTable.AcceptChanges();
			}
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SetDefault_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("VendorCode"));
			if (!string.IsNullOrEmpty(text))
			{
				DfltVendor = text;
				gv_Doc.RefreshData();
				IsChanged = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				IsChanged = true;
				FormMode = Enums.FormViewModeType.View;
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				gv_Doc.PostEditor();
				DTSource = grd_Doc.DataSource as DataTable;
				DTSource.AcceptChanges();
				IsOKClick = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!gv_Doc.FocusedColumn.FieldName.Equals(gcVendorCode.FieldName))
			{
				return;
			}
			string code = Function.ToString(e.Value);
			if (grd_Doc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["VendorCode"]).Equals(code));
				if (dataTable.AsEnumerable().Count((DataRow r) => Function.ToString(r["VendorCode"]).Equals(code)) > 1 || (dataRow != null && dataTable.AsEnumerable().IndexOf(dataRow) != gv_Doc.GetFocusedDataSourceRowIndex()))
				{
					WinCommons.Error(Enums.MessageCode.VendorAlreadyExists, "");
					e.Valid = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _httpClient.Items.GetPreferredVendor("-1");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["VendorCode"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ItemCode"] = ItemCode;
				dataRow["VendorCode"] = "";
				dataRow["LogInstanc"] = 0;
				dataRow["ObjType"] = "4";
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CFLVendor(DataRow row)
	{
		try
		{
			string code = Function.ToString(row["CardCode"]);
			if (grd_Doc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["VendorCode"]).Equals(code));
				if (dataTable.AsEnumerable().Count((DataRow r) => Function.ToString(r["VendorCode"]).Equals(code)) > 1 || (dataRow != null && dataTable.AsEnumerable().IndexOf(dataRow) != gv_Doc.GetFocusedDataSourceRowIndex()))
				{
					WinCommons.Error(Enums.MessageCode.VendorAlreadyExists, "");
					return;
				}
			}
			gv_Doc.SetFocusedRowCellValue("VendorCode", code);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_ItemPreferredVendor));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVendorCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_VendorCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcLogInstance = new Apzon.Controls.Grid.GridColumnEx();
		this.gcObject = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBpName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcItemPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLastPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SetDefault = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VendorCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Location = new System.Drawing.Point(9, 7);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_VendorCode, this.rPriceList });
		this.grd_Doc.Size = new System.Drawing.Size(620, 293);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcItemCode, this.gcVendorCode, this.gcLogInstance, this.gcObject, this.gcBpName, this.gcPriceList, this.gcItemPrice, this.gcLastPrice });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPreferedVendor";
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
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcVendorCode.Caption = "BP Code";
		this.gcVendorCode.ColumnEdit = this.rep_VendorCode;
		this.gcVendorCode.CurrencyDateField = null;
		this.gcVendorCode.CurrencyField = null;
		this.gcVendorCode.CurrencyFixed = null;
		this.gcVendorCode.CurrencyFixedRate = 0.0;
		this.gcVendorCode.CurrencyRateField = null;
		this.gcVendorCode.FieldName = "VendorCode";
		this.gcVendorCode.Name = "gcVendorCode";
		this.gcVendorCode.PrimaryKey = false;
		this.gcVendorCode.UnitField = null;
		this.gcVendorCode.Visible = true;
		this.gcVendorCode.VisibleIndex = 0;
		this.gcVendorCode.Width = 74;
		this.rep_VendorCode.AutoHeight = false;
		this.rep_VendorCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_VendorCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_VendorCode.ContainerControl = this;
		this.rep_VendorCode.DisplayMember = "CardCode";
		this.rep_VendorCode.DisplayMember2 = "";
		this.rep_VendorCode.LoadDataSourceWhenEnter = false;
		this.rep_VendorCode.Name = "rep_VendorCode";
		this.rep_VendorCode.SeparateCharacterForMulti = ';';
		this.rep_VendorCode.SeparateString = ", ";
		this.rep_VendorCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.rep_VendorCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_VendorCode.UsingAutoCompleteBox = false;
		this.rep_VendorCode.UsingDataSource = true;
		this.rep_VendorCode.ValidateWhenNoDataSource = false;
		this.rep_VendorCode.ValueMember = "CardCode";
		this.gcLogInstance.Caption = "Log Instance";
		this.gcLogInstance.CurrencyDateField = null;
		this.gcLogInstance.CurrencyField = null;
		this.gcLogInstance.CurrencyFixed = null;
		this.gcLogInstance.CurrencyFixedRate = 0.0;
		this.gcLogInstance.CurrencyRateField = null;
		this.gcLogInstance.FieldName = "LogInstanc";
		this.gcLogInstance.Name = "gcLogInstance";
		this.gcLogInstance.PrimaryKey = false;
		this.gcLogInstance.UnitField = null;
		this.gcObject.Caption = "Object";
		this.gcObject.CurrencyDateField = null;
		this.gcObject.CurrencyField = null;
		this.gcObject.CurrencyFixed = null;
		this.gcObject.CurrencyFixedRate = 0.0;
		this.gcObject.CurrencyRateField = null;
		this.gcObject.FieldName = "ObjType";
		this.gcObject.Name = "gcObject";
		this.gcObject.PrimaryKey = false;
		this.gcObject.UnitField = null;
		this.gcBpName.Caption = "BP Name";
		this.gcBpName.CurrencyDateField = null;
		this.gcBpName.CurrencyField = null;
		this.gcBpName.CurrencyFixed = null;
		this.gcBpName.CurrencyFixedRate = 0.0;
		this.gcBpName.CurrencyRateField = null;
		this.gcBpName.FieldName = "CardName";
		this.gcBpName.Name = "gcBpName";
		this.gcBpName.PrimaryKey = false;
		this.gcBpName.UnitField = null;
		this.gcBpName.Visible = true;
		this.gcBpName.VisibleIndex = 1;
		this.gcBpName.Width = 223;
		this.gcPriceList.Caption = "Price List";
		this.gcPriceList.ColumnEdit = this.rPriceList;
		this.gcPriceList.CurrencyDateField = null;
		this.gcPriceList.CurrencyField = null;
		this.gcPriceList.CurrencyFixed = null;
		this.gcPriceList.CurrencyFixedRate = 0.0;
		this.gcPriceList.CurrencyRateField = null;
		this.gcPriceList.FieldName = "ListNum";
		this.gcPriceList.Name = "gcPriceList";
		this.gcPriceList.PrimaryKey = false;
		this.gcPriceList.UnitField = null;
		this.gcPriceList.Visible = true;
		this.gcPriceList.VisibleIndex = 2;
		this.gcPriceList.Width = 137;
		this.rPriceList.AutoHeight = false;
		this.rPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListNum", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rPriceList.DisplayMember = "ListName";
		this.rPriceList.Name = "rPriceList";
		this.rPriceList.NullText = "";
		this.rPriceList.ShowHeader = false;
		this.rPriceList.ValueMember = "ListNum";
		this.gcItemPrice.Caption = "Item Price";
		this.gcItemPrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcItemPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcItemPrice.CurrencyDateField = null;
		this.gcItemPrice.CurrencyField = "PriceCurr";
		this.gcItemPrice.CurrencyFixed = null;
		this.gcItemPrice.CurrencyFixedRate = 0.0;
		this.gcItemPrice.CurrencyRateField = null;
		this.gcItemPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcItemPrice.FieldName = "Price";
		this.gcItemPrice.Name = "gcItemPrice";
		this.gcItemPrice.PrimaryKey = false;
		this.gcItemPrice.UnitField = null;
		this.gcItemPrice.Visible = true;
		this.gcItemPrice.VisibleIndex = 3;
		this.gcItemPrice.Width = 77;
		this.gcLastPrice.Caption = "Last Price";
		this.gcLastPrice.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcLastPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcLastPrice.CurrencyDateField = null;
		this.gcLastPrice.CurrencyField = "PurchaseCurr";
		this.gcLastPrice.CurrencyFixed = null;
		this.gcLastPrice.CurrencyFixedRate = 0.0;
		this.gcLastPrice.CurrencyRateField = null;
		this.gcLastPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLastPrice.FieldName = "LastPurPrc";
		this.gcLastPrice.Name = "gcLastPrice";
		this.gcLastPrice.PrimaryKey = false;
		this.gcLastPrice.UnitField = null;
		this.gcLastPrice.Visible = true;
		this.gcLastPrice.VisibleIndex = 4;
		this.gcLastPrice.Width = 70;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(86, 306);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 19;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(9, 306);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 18;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btn_SetDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SetDefault.EditControl = null;
		this.btn_SetDefault.EditValue = "&Set as Default";
		this.btn_SetDefault.FieldName = null;
		this.btn_SetDefault.IsSearchControl = false;
		this.btn_SetDefault.Location = new System.Drawing.Point(532, 306);
		this.btn_SetDefault.Name = "btn_SetDefault";
		this.btn_SetDefault.ReadOnly = false;
		this.btn_SetDefault.SettingInVisible = false;
		this.btn_SetDefault.SettingReadonly = false;
		this.btn_SetDefault.Size = new System.Drawing.Size(97, 23);
		this.btn_SetDefault.TabIndex = 20;
		this.btn_SetDefault.Text = "&Set as Default";
		this.btn_SetDefault.Click += new System.EventHandler(this.btn_SetDefault_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(641, 336);
		base.Controls.Add(this.btn_SetDefault);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oPreferedVendor";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemPreferredVendor";
		this.Text = "Preferred Vendor";
		base.Load += new System.EventHandler(this.frm_ItemPreferredVendor_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VendorCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).EndInit();
		base.ResumeLayout(false);
	}
}
