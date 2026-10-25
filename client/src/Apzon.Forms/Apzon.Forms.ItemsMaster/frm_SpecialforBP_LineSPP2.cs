using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_SpecialforBP_LineSPP2 : XtraFormEx
{
	public string _ItemCode;

	public string _CardCode;

	public string _Currency;

	public double _Discount;

	public double priceItem;

	private string _formController = "SpecialBp_LineSPP2";

	public int _Spp1LNum;

	public int PriceList;

	public DataTable dtConfig = new DataTable();

	private bool IgnoreEvent;

	private int _baseUom = -1;

	private IContainer components;

	private GridControlEx grdDocLineSPP2;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcLINENUM;

	private GridColumnEx gcPrice;

	private GridColumnEx gcDiscount;

	private GridColumnEx gcAmount;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridColumnEx gcSPP2LNum;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1_Amount;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2_Discount;

	private GridViewEx gvDocLineSPP2;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcCurrency;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private GridColumnEx gcUomCode;

	private RepositoryItemLookUpEdit rUom;

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

	public frm_SpecialforBP_LineSPP2()
	{
		InitializeComponent();
	}

	private void frm_SpecialforBP_LineSPP2_Load(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		try
		{
			DataTable uomItem = HttpClient.UnitOfMeasureGroupsClient.GetUomItem(_ItemCode);
			rUom.DataSource = uomItem;
			if (uomItem.IsNotNull())
			{
				DataRow dataRow = uomItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["BaseUom"]).Equals("Y"));
				if (dataRow != null)
				{
					_baseUom = Function.ParseInt(dataRow["UomEntry"]);
				}
			}
			grdDocLineSPP2.DataSource = dtConfig;
			grdDocLineSPP2.RefreshDataSource();
			gvDocLineSPP2.HideLoadingPanel();
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocLineSPP2_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDocLineSPP2, e.Point);
	}

	private void gvDocLineSPP2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvDocLineSPP2.DeleteDataSourceRowSelected();
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private void gvDocLineSPP2_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (priceItem == 0.0 && gvDocLineSPP2.FocusedColumn == gcPrice)
			{
				WinCommons.Error(Enums.MessageCode.NoPriceDefined4PriceList, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocLineSPP2_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (IgnoreEvent)
		{
			return;
		}
		FormMode = Enums.FormViewModeType.Update;
		try
		{
			if (e.Column == gcAmount)
			{
				if (Function.ParseDecimal(e.Value) != 0m)
				{
					gvDocLineSPP2.SetRowCellValue(e.RowHandle, gcDiscount, _Discount);
					double num = priceItem * (1.0 - _Discount / 100.0);
					gvDocLineSPP2.SetRowCellValue(e.RowHandle, gcPrice, num);
					gvDocLineSPP2.SetRowCellValue(e.RowHandle, gcCurrency, _Currency);
					DataRow dataRow = gvDocLineSPP2.GetDataRow(e.RowHandle);
					dataRow["CardCode"] = _CardCode;
					if (Function.ParseInt(dataRow["UomEntry"]) <= 0)
					{
						dataRow["UomEntry"] = _baseUom;
					}
				}
				AddNewRow();
			}
			else if (e.Column == gcDiscount)
			{
				IgnoreEvent = true;
				double num2 = Function.ParseDouble(e.Value);
				gvDocLineSPP2.SetRowCellValue(e.RowHandle, gcPrice, priceItem * (1.0 - num2 / 100.0));
				IgnoreEvent = false;
			}
			else if (e.Column == gcPrice)
			{
				IgnoreEvent = true;
				double num3 = Function.ParseDouble(e.Value);
				double num4 = ((priceItem != 0.0) ? ((1.0 - num3 / priceItem) * 100.0) : 0.0);
				gvDocLineSPP2.SetRowCellValue(e.RowHandle, gcDiscount, num4);
				IgnoreEvent = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		DialogResult = ((FormMode != Enums.FormViewModeType.View) ? DialogResult.OK : DialogResult.Cancel);
		Close();
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grdDocLineSPP2.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || dataSourceTable.AsEnumerable().All((DataRow t) => Function.ParseDecimal(t["Amount"]) != 0m))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				int num = (dataSourceTable.IsNotNull() ? (dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["SPP2LNum"])) + 1) : 0);
				dataRow["SPP2LNum"] = num;
				dataRow["SPP1LNum"] = _Spp1LNum;
				dataRow["ItemCode"] = _ItemCode;
				dataRow["CardCode"] = _CardCode;
				dataRow["Currency"] = _Currency;
				dataRow["UomEntry"] = _baseUom;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				grdDocLineSPP2.DataSource = dataSourceTable;
				grdDocLineSPP2.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void CustomFormSetting()
	{
		if ((rUom.DataSource as DataTable).IsNotNull())
		{
			gcUomCode.Visible = true;
			gcUomCode.OptionsColumn.ReadOnly = false;
		}
		else
		{
			gcUomCode.Visible = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_SpecialforBP_LineSPP2));
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.grdDocLineSPP2 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocLineSPP2 = new Apzon.Controls.Grid.GridViewEx();
		this.gcUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rUom = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLINENUM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSPP2LNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1_Amount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2_Discount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP2).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1_Amount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.grdDocLineSPP2.AllowPasteExcel = true;
		this.grdDocLineSPP2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDocLineSPP2.ClipboardData = null;
		this.grdDocLineSPP2.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDocLineSPP2.Location = new System.Drawing.Point(2, 0);
		this.grdDocLineSPP2.MainView = this.gvDocLineSPP2;
		this.grdDocLineSPP2.Name = "grdDocLineSPP2";
		this.grdDocLineSPP2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCalcEdit1_Amount, this.repositoryItemCalcEdit2_Discount, this.rUom });
		this.grdDocLineSPP2.Size = new System.Drawing.Size(517, 253);
		this.grdDocLineSPP2.TabIndex = 1;
		this.grdDocLineSPP2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocLineSPP2 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDocLineSPP2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocLineSPP2.ColumnKey = null;
		this.gvDocLineSPP2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcUomCode, this.gcItemCode, this.gcCardCode, this.gcLINENUM, this.gcSPP2LNum, this.gcAmount, this.gcDiscount, this.gcPrice });
		this.gvDocLineSPP2.DisableFormSetting = false;
		this.gvDocLineSPP2.DisplayException = false;
		this.gvDocLineSPP2.EditColumn = null;
		this.gvDocLineSPP2.FieldCurrency = null;
		this.gvDocLineSPP2.FieldRate = null;
		this.gvDocLineSPP2.FormId = "oSpecialPriceVolumeDis";
		this.gvDocLineSPP2.GridControl = this.grdDocLineSPP2;
		this.gvDocLineSPP2.IgnoreFilter = false;
		this.gvDocLineSPP2.IndexEditRow = -1;
		this.gvDocLineSPP2.IndicatorWidth = 35;
		this.gvDocLineSPP2.IsApzTable = false;
		this.gvDocLineSPP2.IsInsertNewRow = false;
		this.gvDocLineSPP2.IsLoadingData = false;
		this.gvDocLineSPP2.IsMappingContextAction = false;
		this.gvDocLineSPP2.ItemId = "SPP2";
		this.gvDocLineSPP2.LstColumnAllowBatch = null;
		this.gvDocLineSPP2.LstColumnNotAllowBatch = null;
		this.gvDocLineSPP2.LstColumnNotEditable = null;
		this.gvDocLineSPP2.LstColumNotSetting = null;
		this.gvDocLineSPP2.Name = "gvDocLineSPP2";
		this.gvDocLineSPP2.OptionsView.ColumnAutoWidth = false;
		this.gvDocLineSPP2.OptionsView.ShowFooter = true;
		this.gvDocLineSPP2.OptionsView.ShowGroupPanel = false;
		this.gvDocLineSPP2.UseBatchMod = false;
		this.gvDocLineSPP2.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocLineSPP2_CustomDrawRowIndicator);
		this.gvDocLineSPP2.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDocLineSPP2_PopupMenuShowing);
		this.gvDocLineSPP2.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDocLineSPP2_ShowingEditor);
		this.gvDocLineSPP2.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocLineSPP2_CellValueChanged);
		this.gcUomCode.Caption = "Uom Code";
		this.gcUomCode.ColumnEdit = this.rUom;
		this.gcUomCode.CurrencyDateField = null;
		this.gcUomCode.CurrencyField = null;
		this.gcUomCode.CurrencyFixed = null;
		this.gcUomCode.CurrencyFixedRate = 0.0;
		this.gcUomCode.CurrencyRateField = null;
		this.gcUomCode.FieldName = "UomEntry";
		this.gcUomCode.Name = "gcUomCode";
		this.gcUomCode.PrimaryKey = true;
		this.gcUomCode.UnitField = null;
		this.gcUomCode.Visible = true;
		this.gcUomCode.VisibleIndex = 0;
		this.rUom.AutoHeight = false;
		this.rUom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUom.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("UomCode", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("UomName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("UomEntry", "UomEntry")
		});
		this.rUom.DisplayMember = "UomName";
		this.rUom.Name = "rUom";
		this.rUom.NullText = "";
		this.rUom.ShowHeader = false;
		this.rUom.ValueMember = "UomEntry";
		this.gcItemCode.Caption = "ItemCode";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.gcLINENUM.Caption = "SPP1LNum";
		this.gcLINENUM.CurrencyDateField = null;
		this.gcLINENUM.CurrencyField = null;
		this.gcLINENUM.CurrencyFixed = null;
		this.gcLINENUM.CurrencyFixedRate = 0.0;
		this.gcLINENUM.CurrencyRateField = null;
		this.gcLINENUM.FieldName = "LINENUM";
		this.gcLINENUM.Name = "gcLINENUM";
		this.gcLINENUM.PrimaryKey = false;
		this.gcLINENUM.UnitField = null;
		this.gcSPP2LNum.Caption = "gcSPP2LNum";
		this.gcSPP2LNum.CurrencyDateField = null;
		this.gcSPP2LNum.CurrencyField = null;
		this.gcSPP2LNum.CurrencyFixed = null;
		this.gcSPP2LNum.CurrencyFixedRate = 0.0;
		this.gcSPP2LNum.CurrencyRateField = null;
		this.gcSPP2LNum.FieldName = "SPP2LNum";
		this.gcSPP2LNum.Name = "gcSPP2LNum";
		this.gcSPP2LNum.PrimaryKey = false;
		this.gcSPP2LNum.UnitField = null;
		this.gcAmount.Caption = "Quantity";
		this.gcAmount.ColumnEdit = this.repositoryItemCalcEdit1_Amount;
		this.gcAmount.CurrencyDateField = null;
		this.gcAmount.CurrencyField = null;
		this.gcAmount.CurrencyFixed = null;
		this.gcAmount.CurrencyFixedRate = 0.0;
		this.gcAmount.CurrencyRateField = null;
		this.gcAmount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcAmount.FieldName = "Amount";
		this.gcAmount.Name = "gcAmount";
		this.gcAmount.PrimaryKey = false;
		this.gcAmount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcAmount.UnitField = null;
		this.gcAmount.Visible = true;
		this.gcAmount.VisibleIndex = 1;
		this.gcAmount.Width = 109;
		this.repositoryItemCalcEdit1_Amount.AutoHeight = false;
		this.repositoryItemCalcEdit1_Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1_Amount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1_Amount.Name = "repositoryItemCalcEdit1_Amount";
		this.gcDiscount.Caption = "Disc . in %";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEdit2_Discount;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 2;
		this.gcDiscount.Width = 132;
		this.repositoryItemCalcEdit2_Discount.AutoHeight = false;
		this.repositoryItemCalcEdit2_Discount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.Name = "repositoryItemCalcEdit2_Discount";
		this.gcPrice.Caption = "Special price";
		this.gcPrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = "Currency";
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 3;
		this.gcPrice.Width = 126;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(2, 256);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 40;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(105, 256);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 41;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.ReadOnly = true;
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		this.gcCurrency.Width = 116;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(520, 279);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.grdDocLineSPP2);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.FormId = "oSpecialPriceVolumeDis";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SpecialforBP_LineSPP2";
		this.Text = "Special prices - Volume discount";
		base.Load += new System.EventHandler(this.frm_SpecialforBP_LineSPP2_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP2).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1_Amount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).EndInit();
		base.ResumeLayout(false);
	}
}
