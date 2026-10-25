using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Helper;
using Apzon.Forms.Retail;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class UcChooseFromList : XtraUserControl
{
	public delegate void ChooseFromListClose(params DataRow[] chooseItem);

	public DataRow ChooseItem;

	public List<DataRow> ChooseItems;

	public List<string> Params = new List<string>();

	private readonly Enums.TypeCFL _typeCfl;

	private readonly string _strSearch;

	private readonly bool _multiselect;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private RepositoryItemTextEdit rtxt;

	private PanelControl panelControl1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private GridControlEx grd_CFL;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private RepositoryItemTextEdit repositoryItemTextEdit2;

	private LabelControl labelControl1;

	private SimpleButton btnCancel;

	private SimpleButton btnChoose;

	private TextEdit txt_Search;

	private GridColumn gc_ItemCode;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

	private GridColumn gridColumn4;

	private GridColumn gridColumn5;

	private GridColumn gridColumn6;

	private GridColumn gridColumn7;

	private GridColumn gridColumn8;

	private GridColumn gridColumn9;

	private GridColumn gridColumn10;

	private GridColumn gridColumn11;

	private GridColumn gridColumn12;

	private GridColumn gridColumn13;

	private GridColumn gridColumn14;

	private GridColumn gridColumn15;

	private GridColumn gridColumn16;

	private GridColumn gridColumn17;

	private GridColumn gridColumn18;

	private GridColumn gridColumn19;

	private GridColumn gridColumn20;

	private GridColumn gridColumn21;

	private GridColumn gridColumn23;

	private GridColumn gridColumn24;

	private GridColumn gridColumn25;

	private GridColumn gridColumn26;

	private GridColumn gridColumn27;

	private GridColumn gridColumn28;

	private GridColumn gridColumn29;

	private GridColumn gridColumn30;

	private GridColumn gridColumn31;

	private GridColumn gridColumn32;

	private GridColumn gridColumn33;

	private GridColumn gridColumn34;

	private GridColumn gridColumn35;

	private GridColumn gridColumn36;

	private GridColumn gridColumn37;

	private RetailGrid gv_CFL;

	private GridColumnEx gridColumnEx1;

	public string CardCode { get; set; }

	public string FormId
	{
		get
		{
			return gv_CFL.FormId;
		}
		set
		{
			gv_CFL.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gv_CFL.ItemId;
		}
		set
		{
			gv_CFL.ItemId = value;
		}
	}

	public event ChooseFromListClose OnChooseFromListClose;

	public UcChooseFromList()
	{
		InitializeComponent();
	}

	public UcChooseFromList(Enums.TypeCFL typeCfl, bool multiselect = false, string strSeach = "")
	{
		InitializeComponent();
		_typeCfl = typeCfl;
		_multiselect = multiselect;
		_strSearch = strSeach;
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (gv_CFL.GetSelectedRows() == null || gv_CFL.GetSelectedRows().Length == 0)
			{
				return;
			}
			int[] selectedRows = gv_CFL.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				DataRow dataRow = gv_CFL.GetDataRow(rowHandle);
				if (_multiselect)
				{
					ChooseItems.Add(dataRow);
					if (OnChooseFromListClose != null)
					{
						OnChooseFromListClose(ChooseItems.ToArray());
					}
					continue;
				}
				ChooseItem = dataRow;
				if (OnChooseFromListClose != null)
				{
					OnChooseFromListClose(ChooseItem);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void textEdit1_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			foreach (GridColumn column in gv_CFL.Columns)
			{
				if (column.Visible)
				{
					text += $"OR {column.FieldName} LIKE '%{txt_Search.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_CFL.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gridControl1_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			AddChooseItemsGrid();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void uc_ItemSearch_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			frm_PointOfSales.CloseCFL = true;
			gv_CFL.ShowLoadingPanel();
			try
			{
				if (_multiselect)
				{
					gv_CFL.OptionsSelection.MultiSelect = true;
					gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
				}
				DataSet ds = new DataSet();
				switch (_typeCfl)
				{
				case Enums.TypeCFL.ListItems:
					ds = await _httpClient.RetailChooseFromList.ListItems(GlobalData.UserName, GlobalData.StoreCode, _strSearch, CardCode, _strSearch);
					break;
				case Enums.TypeCFL.Customer:
					ds = await _httpClient.RetailChooseFromList.Customer(GlobalData.UserName, GlobalData.StoreCode, _strSearch);
					break;
				}
				if (ds.IsNotNull())
				{
					await GridViewHelper.GenerateGrid(grd_CFL, gv_CFL, ds);
				}
				ds.Tables.Clear();
				ds.Dispose();
				gv_CFL.BestFitColumns();
				BeginInvoke((MethodInvoker)(() =>
				{
					txt_Search.Focus();
				}));
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
			gv_CFL.HideLoadingPanel();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		try
		{
			AddChooseItemsGrid();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (OnChooseFromListClose != null)
			{
				OnChooseFromListClose(null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Search_MouseDown(object sender, MouseEventArgs e)
	{
	}

	private void txt_Search_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				btnChoose_Click(null, null);
			}
			if (e.KeyCode == Keys.Escape)
			{
				btnCancel_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UcChooseFromList_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Escape)
			{
				btnCancel.PerformClick();
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.UcChooseFromList));
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rtxt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Search = new DevExpress.XtraEditors.TextEdit();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.grd_CFL = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CFL = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn21 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn23 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn24 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn25 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn26 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn27 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn28 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn29 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn30 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn31 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn32 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn33 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn34 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn35 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn36 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn37 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gc_ItemCode = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).BeginInit();
		base.SuspendLayout();
		this.repositoryItemLookUpEdit_Type.AutoHeight = false;
		this.repositoryItemLookUpEdit_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit_Type.Name = "repositoryItemLookUpEdit_Type";
		this.repositoryItemLookUpEdit_Type.NullText = "";
		this.rtxt.AutoHeight = false;
		this.rtxt.Name = "rtxt";
		this.panelControl1.Controls.Add(this.txt_Search);
		this.panelControl1.Controls.Add(this.btnCancel);
		this.panelControl1.Controls.Add(this.btnChoose);
		this.panelControl1.Controls.Add(this.labelControl1);
		this.panelControl1.Controls.Add(this.grd_CFL);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1145, 764);
		this.panelControl1.TabIndex = 0;
		this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Search.Location = new System.Drawing.Point(86, 5);
		this.txt_Search.Name = "txt_Search";
		this.txt_Search.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.txt_Search.Properties.Appearance.Options.UseFont = true;
		this.txt_Search.Size = new System.Drawing.Size(1054, 36);
		this.txt_Search.TabIndex = 4;
		this.txt_Search.EditValueChanged += new System.EventHandler(this.textEdit1_EditValueChanged);
		this.txt_Search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Search_KeyDown);
		this.txt_Search.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_Search_MouseDown);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btnCancel.Appearance.Options.UseFont = true;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(1025, 704);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(115, 55);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnChoose.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btnChoose.Appearance.Options.UseFont = true;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.Location = new System.Drawing.Point(907, 704);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.Size = new System.Drawing.Size(115, 55);
		this.btnChoose.TabIndex = 2;
		this.btnChoose.Text = "OK";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(5, 8);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(75, 30);
		this.labelControl1.TabIndex = 1;
		this.labelControl1.Text = "Search";
		this.grd_CFL.AllowPasteExcel = false;
		this.grd_CFL.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_CFL.ClipboardData = null;
		this.grd_CFL.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.grd_CFL.Location = new System.Drawing.Point(3, 44);
		this.grd_CFL.MainView = this.gv_CFL;
		this.grd_CFL.Name = "grd_CFL";
		this.grd_CFL.Size = new System.Drawing.Size(1137, 654);
		this.grd_CFL.TabIndex = 0;
		this.grd_CFL.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CFL });
		this.grd_CFL.DoubleClick += new System.EventHandler(this.gridControl1_DoubleClick);
		this.gv_CFL.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.gv_CFL.Appearance.HeaderPanel.Options.UseFont = true;
		this.gv_CFL.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CFL.ColumnKey = null;
		this.gv_CFL.ColumnPanelRowHeight = 25;
		this.gv_CFL.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[19]
		{
			this.gridColumnEx1, this.gridColumn19, this.gridColumn20, this.gridColumn21, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28,
			this.gridColumn29, this.gridColumn30, this.gridColumn31, this.gridColumn32, this.gridColumn33, this.gridColumn34, this.gridColumn35, this.gridColumn36, this.gridColumn37
		});
		this.gv_CFL.DisableFormSetting = false;
		this.gv_CFL.DisplayException = false;
		this.gv_CFL.EditColumn = null;
		this.gv_CFL.FieldCurrency = null;
		this.gv_CFL.FieldRate = null;
		this.gv_CFL.FormId = null;
		this.gv_CFL.GridControl = this.grd_CFL;
		this.gv_CFL.IndexEditRow = -1;
		this.gv_CFL.IndicatorWidth = 60;
		this.gv_CFL.IsApzTable = false;
		this.gv_CFL.IsInsertNewRow = false;
		this.gv_CFL.IsLoadingData = false;
		this.gv_CFL.ItemId = "ODATA";
		this.gv_CFL.LstColumnAllowBatch = null;
		this.gv_CFL.LstColumnNotAllowBatch = null;
		this.gv_CFL.LstColumNotSetting = null;
		this.gv_CFL.Name = "gv_CFL";
		this.gv_CFL.RowHeight = 60;
		this.gv_CFL.UseBatchMod = false;
		this.gridColumnEx1.Caption = "BarCode";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "CodeBars";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumn19.Caption = "Item Code";
		this.gridColumn19.FieldName = "ItemCode";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.AllowEdit = false;
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 1;
		this.gridColumn20.Caption = "Item Name";
		this.gridColumn20.FieldName = "ItemDescription";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.AllowEdit = false;
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 2;
		this.gridColumn21.Caption = "FrgnName";
		this.gridColumn21.FieldName = "FrgnName";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.AllowEdit = false;
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn23.Caption = "ItmsGrpCod";
		this.gridColumn23.FieldName = "ItmsGrpCod";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.OptionsColumn.AllowEdit = false;
		this.gridColumn23.OptionsColumn.ReadOnly = true;
		this.gridColumn24.Caption = "SalesVATGroup";
		this.gridColumn24.FieldName = "SalesVATGroup";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.OptionsColumn.AllowEdit = false;
		this.gridColumn24.OptionsColumn.ReadOnly = true;
		this.gridColumn25.Caption = "VatGroupPu";
		this.gridColumn25.FieldName = "VatGroupPu";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.OptionsColumn.AllowEdit = false;
		this.gridColumn25.OptionsColumn.ReadOnly = true;
		this.gridColumn26.Caption = "VatGourpSa";
		this.gridColumn26.FieldName = "VatGourpSa";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.OptionsColumn.AllowEdit = false;
		this.gridColumn26.OptionsColumn.ReadOnly = true;
		this.gridColumn27.Caption = "QuantityOnStock";
		this.gridColumn27.FieldName = "QuantityOnStock";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.OptionsColumn.AllowEdit = false;
		this.gridColumn27.OptionsColumn.ReadOnly = true;
		this.gridColumn27.Visible = true;
		this.gridColumn27.VisibleIndex = 3;
		this.gridColumn28.Caption = "Currency";
		this.gridColumn28.FieldName = "Currency";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.AllowEdit = false;
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 6;
		this.gridColumn29.Caption = "Price";
		this.gridColumn29.FieldName = "Price";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.AllowEdit = false;
		this.gridColumn29.OptionsColumn.ReadOnly = true;
		this.gridColumn29.Visible = true;
		this.gridColumn29.VisibleIndex = 4;
		this.gridColumn30.Caption = "PriceList";
		this.gridColumn30.FieldName = "PriceList";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.OptionsColumn.AllowEdit = false;
		this.gridColumn30.OptionsColumn.ReadOnly = true;
		this.gridColumn30.Visible = true;
		this.gridColumn30.VisibleIndex = 5;
		this.gridColumn31.Caption = "RateSa";
		this.gridColumn31.FieldName = "RateSa";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.OptionsColumn.AllowEdit = false;
		this.gridColumn31.OptionsColumn.ReadOnly = true;
		this.gridColumn32.Caption = "RatePu";
		this.gridColumn32.FieldName = "RatePu";
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.OptionsColumn.AllowEdit = false;
		this.gridColumn32.OptionsColumn.ReadOnly = true;
		this.gridColumn33.Caption = "BOMTreeType";
		this.gridColumn33.FieldName = "BOMTreeType";
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.OptionsColumn.AllowEdit = false;
		this.gridColumn33.OptionsColumn.ReadOnly = true;
		this.gridColumn34.Caption = "VatPuInActive";
		this.gridColumn34.FieldName = "VatPuInActive";
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.OptionsColumn.AllowEdit = false;
		this.gridColumn34.OptionsColumn.ReadOnly = true;
		this.gridColumn35.Caption = "VatSaInActive";
		this.gridColumn35.FieldName = "VatSaInActive";
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.OptionsColumn.AllowEdit = false;
		this.gridColumn35.OptionsColumn.ReadOnly = true;
		this.gridColumn36.Caption = "TreeType";
		this.gridColumn36.FieldName = "TreeType";
		this.gridColumn36.Name = "gridColumn36";
		this.gridColumn36.OptionsColumn.AllowEdit = false;
		this.gridColumn36.OptionsColumn.ReadOnly = true;
		this.gridColumn37.Caption = "WarehouseCode";
		this.gridColumn37.FieldName = "WarehouseCode";
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.OptionsColumn.AllowEdit = false;
		this.gridColumn37.OptionsColumn.ReadOnly = true;
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.NullText = "";
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.repositoryItemLookUpEdit2.AutoHeight = false;
		this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
		this.repositoryItemLookUpEdit2.NullText = "";
		this.repositoryItemTextEdit2.AutoHeight = false;
		this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
		this.gc_ItemCode.Caption = "Item Code";
		this.gc_ItemCode.FieldName = "ItemCode";
		this.gc_ItemCode.Name = "gc_ItemCode";
		this.gc_ItemCode.Visible = true;
		this.gc_ItemCode.VisibleIndex = 0;
		this.gc_ItemCode.Width = 82;
		this.gridColumn1.Caption = "Item Name";
		this.gridColumn1.FieldName = "ItemDescription";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 1;
		this.gridColumn1.Width = 82;
		this.gridColumn2.Caption = "FrgnName";
		this.gridColumn2.FieldName = "FrgnName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn3.Caption = "Old Code";
		this.gridColumn3.FieldName = "U_OldCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.gridColumn4.Caption = "ItmsGrpCod";
		this.gridColumn4.FieldName = "ItmsGrpCod";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.gridColumn5.Caption = "SalesVATGroup";
		this.gridColumn5.FieldName = "SalesVATGroup";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 5;
		this.gridColumn6.Caption = "VatGroupPu";
		this.gridColumn6.FieldName = "VatGroupPu";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 6;
		this.gridColumn7.Caption = "VatGourpSa";
		this.gridColumn7.FieldName = "VatGourpSa";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 7;
		this.gridColumn8.Caption = "QuantityOnStock";
		this.gridColumn8.FieldName = "QuantityOnStock";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 8;
		this.gridColumn9.Caption = "Currency";
		this.gridColumn9.FieldName = "Currency";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 9;
		this.gridColumn10.Caption = "Price";
		this.gridColumn10.DisplayFormat.FormatString = "n2";
		this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn10.FieldName = "Price";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 10;
		this.gridColumn11.Caption = "PriceList";
		this.gridColumn11.FieldName = "PriceList";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 11;
		this.gridColumn12.Caption = "RateSa";
		this.gridColumn12.FieldName = "RateSa";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 12;
		this.gridColumn13.Caption = "RatePu";
		this.gridColumn13.FieldName = "RatePu";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 13;
		this.gridColumn14.Caption = "BOMTreeType";
		this.gridColumn14.FieldName = "BOMTreeType";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 14;
		this.gridColumn15.Caption = "VatPuInActive";
		this.gridColumn15.FieldName = "VatPuInActive";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 15;
		this.gridColumn16.Caption = "VatSaInActive";
		this.gridColumn16.FieldName = "VatSaInActive";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 16;
		this.gridColumn17.Caption = "TreeType";
		this.gridColumn17.FieldName = "TreeType";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 17;
		this.gridColumn18.Caption = "WarehouseCode";
		this.gridColumn18.FieldName = "WarehouseCode";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 18;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.panelControl1);
		base.Name = "UcChooseFromList";
		base.Size = new System.Drawing.Size(1145, 764);
		base.Load += new System.EventHandler(this.uc_ItemSearch_Load);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UcChooseFromList_KeyDown);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.panelControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
