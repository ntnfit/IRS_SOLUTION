using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPromotions : XtraUserControl
{
	public Action ForceOk;

	public Action<DataRow[]> AddItems;

	public Action<DataRow[]> AddSoDiscount;

	public Action<DataRow[]> AddItemsDiscount;

	public Action ClearPromotion;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private RepositoryItemTextEdit rtxt;

	private PanelControl panelControl1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private GridControlEx grd_Promotions;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private RepositoryItemTextEdit repositoryItemTextEdit2;

	private LabelControl labelControl1;

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

	private GridColumn gridColumn26;

	private GridColumn gridColumn27;

	private GridColumn gridColumn28;

	private GridColumn gridColumn29;

	private GridColumn gridColumn30;

	private GridColumn gridColumn31;

	private GridColumn gridColumn37;

	private RetailGrid gv_Promotions;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	public string CardCode { get; set; }

	public string FormId
	{
		get
		{
			return gv_Promotions.FormId;
		}
		set
		{
			gv_Promotions.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gv_Promotions.ItemId;
		}
		set
		{
			gv_Promotions.ItemId = value;
		}
	}

	public ucPromotions()
	{
		InitializeComponent();
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			ClearPromotion?.Invoke();
			if (gv_Promotions.GetSelectedRows() != null && gv_Promotions.GetSelectedRows().Length != 0)
			{
				DataRow[] obj = (from t in gv_Promotions.GetListDataSourceSelectedRow()
					where Function.ParseInt(t["BenefitType"]) == 3
					select t).ToArray();
				DataRow[] obj2 = (from t in gv_Promotions.GetListDataSourceSelectedRow()
					where Function.ParseInt(t["BenefitType"]) == 2
					select t).ToArray();
				DataRow[] obj3 = (from t in gv_Promotions.GetListDataSourceSelectedRow()
					where Function.ParseInt(t["BenefitType"]) == 1
					select t).ToArray();
				AddItems?.Invoke(obj);
				AddItemsDiscount?.Invoke(obj2);
				AddSoDiscount?.Invoke(obj3);
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
			foreach (GridColumn column in gv_Promotions.Columns)
			{
				if (column.Visible)
				{
					text += $"OR {column.FieldName} LIKE '%{txt_Search.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_Promotions.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gridControl1_DoubleClick(object sender, EventArgs e)
	{
	}

	public void SUBMIT()
	{
		try
		{
			AddChooseItemsGrid();
			ForceOk?.Invoke();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetDataSource(DataTable dt)
	{
		try
		{
			gv_Promotions.ShowLoadingPanel();
			try
			{
				grd_Promotions.DataSource = dt;
				grd_Promotions.RefreshDataSource();
				gv_Promotions.BestFitColumns();
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
			gv_Promotions.HideLoadingPanel();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
	}

	private void txt_Search_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
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
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rtxt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Search = new DevExpress.XtraEditors.TextEdit();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.grd_Promotions = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Promotions = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn21 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn23 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn24 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn26 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn27 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn28 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn30 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn31 = new DevExpress.XtraGrid.Columns.GridColumn();
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
		((System.ComponentModel.ISupportInitialize)this.grd_Promotions).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Promotions).BeginInit();
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
		this.panelControl1.Controls.Add(this.labelControl1);
		this.panelControl1.Controls.Add(this.grd_Promotions);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1076, 571);
		this.panelControl1.TabIndex = 0;
		this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Search.Location = new System.Drawing.Point(86, 5);
		this.txt_Search.Name = "txt_Search";
		this.txt_Search.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.txt_Search.Properties.Appearance.Options.UseFont = true;
		this.txt_Search.Size = new System.Drawing.Size(985, 36);
		this.txt_Search.TabIndex = 4;
		this.txt_Search.EditValueChanged += new System.EventHandler(this.textEdit1_EditValueChanged);
		this.txt_Search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Search_KeyDown);
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(5, 8);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(75, 30);
		this.labelControl1.TabIndex = 1;
		this.labelControl1.Text = "Search";
		this.grd_Promotions.AllowPasteExcel = false;
		this.grd_Promotions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Promotions.ClipboardData = null;
		this.grd_Promotions.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.grd_Promotions.Location = new System.Drawing.Point(3, 44);
		this.grd_Promotions.MainView = this.gv_Promotions;
		this.grd_Promotions.Name = "grd_Promotions";
		this.grd_Promotions.Size = new System.Drawing.Size(1068, 527);
		this.grd_Promotions.TabIndex = 0;
		this.grd_Promotions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Promotions });
		this.grd_Promotions.DoubleClick += new System.EventHandler(this.gridControl1_DoubleClick);
		this.gv_Promotions.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.gv_Promotions.Appearance.HeaderPanel.Options.UseFont = true;
		this.gv_Promotions.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Promotions.ColumnKey = null;
		this.gv_Promotions.ColumnPanelRowHeight = 25;
		this.gv_Promotions.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumn19, this.gridColumnEx6, this.gridColumn20, this.gridColumnEx5, this.gridColumnEx1, this.gridColumn29, this.gridColumn21,
			this.gridColumn23, this.gridColumn24, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn30, this.gridColumn31, this.gridColumn37
		});
		this.gv_Promotions.DisableFormSetting = false;
		this.gv_Promotions.DisplayException = false;
		this.gv_Promotions.EditColumn = null;
		this.gv_Promotions.FieldCurrency = null;
		this.gv_Promotions.FieldRate = null;
		this.gv_Promotions.FormId = "";
		this.gv_Promotions.GridControl = this.grd_Promotions;
		this.gv_Promotions.GroupCount = 2;
		this.gv_Promotions.IndexEditRow = -1;
		this.gv_Promotions.IndicatorWidth = 60;
		this.gv_Promotions.IsApzTable = false;
		this.gv_Promotions.IsInsertNewRow = false;
		this.gv_Promotions.IsLoadingData = false;
		this.gv_Promotions.ItemId = "ODATA";
		this.gv_Promotions.LstColumnAllowBatch = null;
		this.gv_Promotions.LstColumnNotAllowBatch = null;
		this.gv_Promotions.LstColumNotSetting = null;
		this.gv_Promotions.Name = "gv_Promotions";
		this.gv_Promotions.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Promotions.OptionsSelection.MultiSelect = true;
		this.gv_Promotions.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Promotions.OptionsView.ShowGroupPanel = false;
		this.gv_Promotions.RowHeight = 60;
		this.gv_Promotions.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx2, DevExpress.Data.ColumnSortOrder.Ascending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx3, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Promotions.UseBatchMod = false;
		this.gridColumnEx2.Caption = "CT Khuyến mãi";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "TenCT";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "Group";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Grp";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Nhóm";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "GroupCT";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 72;
		this.gridColumn19.Caption = "Mã hàng";
		this.gridColumn19.FieldName = "MaHang";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.AllowEdit = false;
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 2;
		this.gridColumn19.Width = 78;
		this.gridColumnEx6.Caption = "Số lượng";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SL";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 71;
		this.gridColumn20.Caption = "Tên hàng";
		this.gridColumn20.FieldName = "TenHang";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.AllowEdit = false;
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 3;
		this.gridColumn20.Width = 163;
		this.gridColumnEx5.Caption = "ĐVT";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DVT";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 79;
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
		this.gridColumn29.Caption = "Đơn giá";
		this.gridColumn29.FieldName = "Price";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.AllowEdit = false;
		this.gridColumn29.OptionsColumn.ReadOnly = true;
		this.gridColumn29.Visible = true;
		this.gridColumn29.VisibleIndex = 6;
		this.gridColumn29.Width = 110;
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
		this.gridColumn28.Caption = "Currency";
		this.gridColumn28.FieldName = "Currency";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.AllowEdit = false;
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn30.Caption = "PriceList";
		this.gridColumn30.FieldName = "PriceList";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.OptionsColumn.AllowEdit = false;
		this.gridColumn30.OptionsColumn.ReadOnly = true;
		this.gridColumn31.Caption = "RateSa";
		this.gridColumn31.FieldName = "RateSa";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.OptionsColumn.AllowEdit = false;
		this.gridColumn31.OptionsColumn.ReadOnly = true;
		this.gridColumn37.Caption = "WarehouseCode";
		this.gridColumn37.FieldName = "WhsCode";
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
		base.Name = "ucPromotions";
		base.Size = new System.Drawing.Size(1076, 571);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.panelControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Promotions).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Promotions).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
