using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Dashboard;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.DashboardCommon;
using DevExpress.Export;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Design;
using DevExpress.XtraSpreadsheet.UI;
using DevExpress.XtraTab;

namespace Apzon.Forms.DMS;

public class frm_SettingReport : XtraFormEx
{
	public enum saveType
	{
		Add = 1,
		Update,
		View
	}

	private DataTable dtSource = new DataTable();

	private DataTable dtProperties = new DataTable();

	private DataTable dtListReport = new DataTable();

	protected bool isAddNew;

	private bool isActiveFromBar;

	private bool isFromLoadColor;

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	protected readonly Dictionary<string, string> _dataTypeHeader = new Dictionary<string, string>
	{
		{ "normal", "Normal" },
		{ "number", "Number" },
		{ "date", "Date" },
		{ "datetime", "DateTime" }
	};

	protected readonly Dictionary<string, string> _columnType = new Dictionary<string, string>
	{
		{ "I", "Info" },
		{ "V", "Value" },
		{ "A", "Argument" },
		{ "S", "Series" }
	};

	private readonly Dictionary<string, string> _justifyContent = new Dictionary<string, string>
	{
		{ "", "" },
		{ "flex-start", "flex-start" },
		{ "flex-end", "flex-end" },
		{ "center", "center" },
		{ "space-between", "space-between" },
		{ "space-around", "space-around" },
		{ "space-evenly", "space-evenly" }
	};

	private readonly Dictionary<string, string> _alignItems = new Dictionary<string, string>
	{
		{ "", "" },
		{ "stretch", "stretch" },
		{ "flex-start", "flex-start" },
		{ "flex-end", "flex-end" },
		{ "center", "center" },
		{ "baseline", "baseline" }
	};

	private readonly Dictionary<string, string> _ItemType = new Dictionary<string, string>
	{
		{ "", "" },
		{ "NumberFormat", "NumberFormat" },
		{ "PriceFormat", "PriceFormat" },
		{ "DateTime", "DateTime" },
		{ "Image", "Image" }
	};

	private readonly Dictionary<string, string> _textAligns = new Dictionary<string, string>
	{
		{ "", "" },
		{ "right", "right" },
		{ "left", "left" },
		{ "center", "center" },
		{ "justify", "justify" },
		{ "string", "string" }
	};

	private readonly Dictionary<string, string> _alignSelf = new Dictionary<string, string>
	{
		{ "", "" },
		{ "auto", "auto" },
		{ "flex-start", "flex-start" },
		{ "flex-end", "flex-end" },
		{ "center", "center" },
		{ "baseline", "baseline" },
		{ "stretch", "stretch" }
	};

	private readonly Dictionary<string, string> _fontWeight = new Dictionary<string, string>
	{
		{ "", "" },
		{ "100", "100" },
		{ "200", "200" },
		{ "300", "300" },
		{ "400", "400" },
		{ "500", "500" },
		{ "600", "600" },
		{ "700", "700" },
		{ "800", "800" },
		{ "900", "900" },
		{ "normal", "normal" },
		{ "bold", "bold" },
		{ "bolder", "bolder" },
		{ "lighter", "lighter" }
	};

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected DataTable dataTable1;

	protected DataTable dataTable2;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown1;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown2;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown3;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown4;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown5;

	protected DataTable dataTable3;

	protected DataTable dataTable4;

	protected SimpleButton simpleButton1;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown6;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown7;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown8;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown9;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown10;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown11;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown12;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown13;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown14;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown15;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown16;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown17;

	protected DataTable dataTable5;

	protected DataTable dataTable6;

	protected SpreadsheetControl sc_Main;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown18;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown19;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown20;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown21;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown22;

	protected SplitContainerControl splitContainerControl1;

	protected SplitContainerControl splitContainerControl2;

	protected GridControlEx grd_listReport;

	protected GridViewEx gv_listReport;

	protected GridColumnEx gc_DocEntry;

	protected GridColumnEx gc_ReportId;

	protected GridColumnEx gc_Active;

	protected RepositoryItemCheckEdit repositoryItemCheckEdit1;

	protected DataTable dataTable7;

	protected DataTable dataTable8;

	protected SimpleButtonEx btn_cancel;

	protected SimpleButtonEx btn_ok;

	protected ContextMenuStrip contextMenuStrip_Doc;

	protected ToolStripMenuItem stripItem_Add;

	protected ToolStripMenuItem stripItem_Remove;

	protected RepositoryItemButtonEditCFL btn_reportGrid;

	protected BarManager barManager1;

	protected FontBar fontBar1;

	protected ChangeFontNameItem changeFontNameItem1;

	protected RepositoryItemFontEdit repositoryItemFontEdit1;

	protected ChangeFontSizeItem changeFontSizeItem1;

	protected RepositoryItemSpreadsheetFontSizeEdit repositoryItemSpreadsheetFontSizeEdit1;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem5;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem6;

	protected SpreadsheetCommandBarCheckItem check_Bold;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem2;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem3;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem4;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem1;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem7;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem8;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem9;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem10;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem11;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem12;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem13;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem14;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem15;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem16;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem17;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem18;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem19;

	protected ChangeBorderLineColorItem changeBorderLineColorItem1;

	protected ChangeBorderLineStyleItem changeBorderLineStyleItem1;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown23;

	protected ChangeCellFillColorItem changeCellFillColorItem1;

	protected ChangeFontColorItem changeFontColorItem1;

	protected AlignmentBar alignmentBar1;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem5;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem6;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem7;

	protected SpreadsheetCommandBarCheckItem check_alignLeft;

	protected SpreadsheetCommandBarCheckItem check_alignCenter;

	protected SpreadsheetCommandBarCheckItem check_alignRight;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem20;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem21;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem11;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem2;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem12;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem22;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem23;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem24;

	protected BarDockControl barDockControlTop;

	protected BarDockControl barDockControlBottom;

	protected BarDockControl barDockControlLeft;

	protected BarDockControl barDockControlRight;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem1;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem2;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem3;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem4;

	protected ChangeNumberFormatItem changeNumberFormatItem1;

	protected RepositoryItemPopupGalleryEdit repositoryItemPopupGalleryEdit1;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem3;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem25;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem26;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem27;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem28;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem29;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem30;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem31;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem32;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem33;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem34;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem7;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem4;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem35;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem36;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem37;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem38;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem39;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem40;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem41;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem5;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem42;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem43;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem44;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem45;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem46;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem47;

	protected SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem1;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown24;

	protected SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem2;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown25;

	protected SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem3;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown26;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem48;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem6;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem49;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem50;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem51;

	protected GalleryFormatAsTableItem galleryFormatAsTableItem1;

	protected CommandBarGalleryDropDown commandBarGalleryDropDown27;

	protected ChangeStyleItem changeStyleItem1;

	protected RepositoryItemSpreadsheetCellStyleEdit repositoryItemSpreadsheetCellStyleEdit1;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem8;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem52;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem53;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem54;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem55;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem56;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem57;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem58;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem59;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem9;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem60;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem61;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem62;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem63;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem64;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem65;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem11;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem66;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem67;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem68;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem69;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem70;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem10;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem71;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem72;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem73;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem74;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem75;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem76;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem77;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem78;

	protected ChangeSheetTabColorItem changeSheetTabColorItem1;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem79;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem13;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem80;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem12;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem81;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem82;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem83;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem84;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem85;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem13;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem86;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem87;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem88;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem89;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem14;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem90;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem91;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem92;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem93;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem94;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem95;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem15;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem96;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem97;

	protected SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem14;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem98;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem99;

	protected SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem16;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem100;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem101;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem102;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem103;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem104;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem105;

	protected SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem106;

	protected ClipboardBar clipboardBar1;

	protected SpreadsheetBarController spreadsheetBarController1;

	protected ToolStripMenuItem stripItem_Refresh;

	protected DataTable dataTable9;

	protected DataTable dataTable10;

	protected DataTable dataTable11;

	protected DataTable dataTable12;

	protected BarEditItem bar_fontColor;

	protected RepositoryItemColorPickEdit repositoryItemColorPickEdit2;

	protected ToolTip toolTip1;

	protected XtraTabPageEx xtraTabPageEx1;

	protected GridViewEx gv_listProperties;

	protected GridColumnEx gc_Type;

	protected GridColumnEx gc_ItemType;

	protected RepositoryItemLookUpEdit luk_ItemType;

	protected GridColumnEx gc_FieldName;

	protected GridColumnEx gc_DemoData;

	protected RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	protected GridColumnEx gc_size;

	protected RepositoryItemCalcEdit txt_Size;

	protected GridColumnEx gc_TextBefore;

	protected GridColumnEx gc_TextAfter;

	protected GridColumnEx gc_Color;

	protected RepositoryItemColorPickEdit repositoryItemColorPickEdit1;

	protected GridColumnEx gc_AlignItems;

	protected RepositoryItemLookUpEdit luk_AlignItem;

	protected GridColumnEx gc_JustifyContent;

	protected RepositoryItemLookUpEdit luk_JustifyContent;

	protected GridColumnEx gc_FontSize;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit1;

	protected GridColumnEx gc_FontWeight;

	protected RepositoryItemLookUpEdit luk_FontWeight;

	protected GridColumnEx gc_PaddingTop;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit2;

	protected GridColumnEx gc_PaddingBottom;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit3;

	protected GridColumnEx gc_PaddingLeft;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit4;

	protected GridColumnEx gc_PaddingRight;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit5;

	protected GridColumnEx gc_TextAlign;

	protected RepositoryItemLookUpEdit luk_TextAlign;

	protected GridColumnEx gc_AlignSelf;

	protected RepositoryItemLookUpEdit luk_AlignSelf;

	protected GridColumnEx gc_Left;

	protected GridColumnEx gc_Top;

	protected GridColumnEx gc_Right;

	protected GridColumnEx gc_Bottom;

	protected XtraTabPageEx xtraTabPageEx2;

	protected DataTable dataTable13;

	protected DataTable dataTable14;

	protected GridControlEx grd_Chart;

	protected GridViewEx gv_Chart;

	protected GridColumnEx gc_OrderId;

	protected GridColumnEx gc_FieldNameChart;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit6;

	protected GridColumnEx gc_DisplayText;

	protected GridColumnEx gc_ChartColor;

	protected RepositoryItemLookUpEdit luk_ChartFieldName;

	protected RepositoryItemLookUpEdit luk_DataType;

	protected RepositoryItemCheckEdit check_ChartVisible;

	protected RepositoryItemColorPickEdit repositoryItemColorPickEdit3;

	protected ContextMenuStrip contextMenuStrip_Chart;

	protected ToolStripMenuItem addNewToolStripMenuItem;

	protected ToolStripMenuItem removeToolStripMenuItem;

	protected ToolStripMenuItem refreshToolStripMenuItem;

	protected DataTable dataTable15;

	protected DataTable dataTable16;

	protected RepositoryItemCheckEdit check_IsKey;

	protected DataTable dataTable17;

	protected DataTable dataTable18;

	protected XtraTabControlEx tab_dashboard;

	protected DataTable dataTable19;

	protected XtraTabPageEx xtraTabPageEx3;

	protected GridViewEx gv_Header;

	protected DataTable dataTable20;

	protected DataTable dataTable21;

	protected DataTable dataTable22;

	protected DataTable dataTable23;

	protected RepositoryItemSearchLookUpEdit luk_FieldName;

	protected GridView repositoryItemSearchLookUpEdit1View;

	protected GridColumnEx gc_HeaderOrderId;

	protected GridColumnEx gc_HeaderFieldName;

	protected RepositoryItemSearchLookUpEdit luk_HeaderFieldName;

	protected GridView gridView1;

	protected GridColumnEx gc_HeaderDataType;

	protected RepositoryItemLookUpEdit luk_HeaderDataType;

	protected GridColumnEx gc_HeaderDisplayText;

	public GridControlEx grd_listProperties;

	private IContainer components;

	public GridControlEx grd_Header;

	private ContextMenuStrip contextMenuStrip_Header;

	private ToolStripMenuItem addNewToolStripMenuItem1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem1;

	private GridColumnEx gc_ColumnName;

	private GridColumnEx gc_TableOrderId;

	private RepositoryItemCalcEdit repositoryItemCalcEdit7;

	private GridColumnEx gc_DateTimeValueFormat;

	private GridColumnEx gc_DateTimeDisplayFormat;

	private RepositoryItemCalcEdit repositoryItemCalcEdit8;

	private GridColumnEx gc_currencyField;

	private GridView gridView2;

	protected RepositoryItemSearchLookUpEdit luk_currencyField;

	protected GridColumnEx gc_ReportName;

	private GridColumnEx gc_ColumnType;

	private RepositoryItemLookUpEdit luk_ColumnType;

	private RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;

	private GridView gridView3;

	private XtraTabPageEx tabPage_dashboard;

	private SimpleButtonEx btn_EditDesign;

	private GridControlEx grd_dashboard;

	private GridViewEx gv_dashboard;

	private GridColumnEx gc_dashboardName;

	private GridColumnEx gc_XMLLayout;

	private TextEditEx txt_DashboardName;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private ContextMenuStrip contextMenuStrip_Dashboard;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemButtonEdit btn_detailDashboard;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_ok.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_SettingReport()
	{
		InitializeComponent();
		OnLoadAsync += frm_SettingReport_Load;
		btn_detailDashboard.ButtonClick += DetailDashboardClick;
		SetDefaultDocumentStyle();
		tab_dashboard.TabPages[0].PageVisible = false;
		tab_dashboard.TabPages[2].PageVisible = false;
		tab_dashboard.TabPages[3].PageVisible = true;
	}

	private void SetDefaultDocumentStyle()
	{
		try
		{
			sc_Main.ActiveWorksheet.DefaultColumnWidth = 230f;
			sc_Main.Document.Styles[BuiltInStyleId.Normal].Alignment.WrapText = true;
			sc_Main.Document.Styles[BuiltInStyleId.Normal].Font.Size = 13.0;
			sc_Main.Document.Styles[BuiltInStyleId.Normal].Alignment.Vertical = SpreadsheetVerticalAlignment.Top;
			sc_Main.Document.Styles[BuiltInStyleId.Normal].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async void SetDataLukField(string code)
	{
		try
		{
			DataTable dataTable = await _httpClient.SettingReportClient.GetColumnStoreReport(code);
			luk_ChartFieldName.DataSource = dataTable;
			dataTable.Rows.Add("", "");
			luk_FieldName.DataSource = dataTable;
			luk_currencyField.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<DataTable> LoadData(int id, string fromDate, string toDate, string report)
	{
		try
		{
			return await _httpClient.SettingReportClient.LoadData(id, fromDate, toDate, report);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual async Task LoadHeader()
	{
		try
		{
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<DataTable> GetLineById(int id)
	{
		try
		{
			return await _httpClient.SettingReportClient.GetAllLineById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetDataChartById(int id)
	{
		try
		{
			return await _httpClient.SettingReportClient.GetDataChartById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual void LoadCFLButtonHeader()
	{
		try
		{
			btn_reportGrid.TypeCFL = Enums.TypeCFL.GetAllReportDMS;
			btn_reportGrid.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void DetailDashboardClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataRow focusedDataRow = ((ButtonEdit)sender).GetGridViewFromEditor().GetFocusedDataRow();
			string caption = Function.ToString(focusedDataRow["DashboardName"]);
			string text = Function.ToString(focusedDataRow["Layout"]);
			Function.ToString(focusedDataRow["DashboardId"]);
			XDocument xDoc = XDocument.Parse(text);
			DashboardObjectDataSource dashboardObjectDataSource = await GetDashboarDataSource();
			DevExpress.DashboardCommon.Dashboard dashboard = new DevExpress.DashboardCommon.Dashboard();
			dashboard.LoadFromXDocument(xDoc);
			foreach (IDashboardDataSource dataSource in dashboard.DataSources)
			{
				if (dataSource is DashboardObjectDataSource)
				{
					((DashboardObjectDataSource)dataSource).DataSource = dashboardObjectDataSource.DataSource;
				}
			}
			frm_DashboardDesigner frm;
			if (dashboard != null)
			{
				dashboard.Title.Text = caption;
				dashboard.Title.Visible = false;
				dashboard.Title.ShowMasterFilterState = true;
				frm = new frm_DashboardDesigner(dashboard);
			}
			else
			{
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(new DataTable());
				frm = new frm_DashboardDesigner(dataSet, caption);
			}
			frm_DashboardDesigner frm_DashboardDesigner2 = frm;
			frm_DashboardDesigner2.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner2.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
			{
				await SaveEditedDashboard(obj, frm, Enums.TransactionType.U);
			}));
			frm.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private new async Task Load()
	{
		try
		{
			dtSource.Clear();
			dtSource.Columns.Clear();
			dtSource.Columns.Add("Id", typeof(int));
			dtSource.Columns.Add("FatherId", typeof(int));
			dtSource.Columns.Add("Value", typeof(string));
			dtSource.Columns.Add("Type", typeof(string));
			dtSource.Columns.Add("Left", typeof(int));
			dtSource.Columns.Add("Top", typeof(int));
			dtSource.Columns.Add("Right", typeof(int));
			dtSource.Columns.Add("Bottom", typeof(int));
			dtSource.AcceptChanges();
			dtProperties.Clear();
			dtProperties.Columns.Clear();
			dtProperties.Columns.Add("DocEntry", typeof(int));
			dtProperties.Columns.Add("FatherNum", typeof(int));
			dtProperties.Columns.Add("Id", typeof(int));
			dtProperties.Columns.Add("ItemType", typeof(string));
			dtProperties.Columns.Add("fieldName", typeof(string));
			dtProperties.Columns.Add("size", typeof(string));
			dtProperties.Columns.Add("textBefore", typeof(string));
			dtProperties.Columns.Add("textAfter", typeof(string));
			dtProperties.Columns.Add("color", typeof(string));
			dtProperties.Columns.Add("alignItems", typeof(string));
			dtProperties.Columns.Add("justifyContent", typeof(string));
			dtProperties.Columns.Add("fontSize", typeof(string));
			dtProperties.Columns.Add("fontWeight", typeof(string));
			dtProperties.Columns.Add("paddingTop", typeof(string));
			dtProperties.Columns.Add("paddingBottom", typeof(string));
			dtProperties.Columns.Add("paddingLeft", typeof(string));
			dtProperties.Columns.Add("paddingRight", typeof(string));
			dtProperties.Columns.Add("textAlign", typeof(string));
			dtProperties.Columns.Add("alignSelf", typeof(string));
			dtProperties.Columns.Add("LeftIndex", typeof(string));
			dtProperties.Columns.Add("TopIndex", typeof(string));
			dtProperties.Columns.Add("RightIndex", typeof(string));
			dtProperties.Columns.Add("BottomIndex", typeof(string));
			dtProperties.Columns.Add("Type", typeof(string));
			dtProperties.Columns.Add("DemoData", typeof(string));
			dtProperties.Columns.Add("ColumnName", typeof(string));
			dtProperties.Columns.Add("TableOrderId", typeof(string));
			dtProperties.Columns.Add("DateTimeDisplayFormat", typeof(string));
			dtProperties.Columns.Add("DateTimeValueFormat", typeof(string));
			dtProperties.Columns.Add("currencyField", typeof(string));
			dtProperties.AcceptChanges();
			luk_AlignItem.DataSource = _alignItems;
			luk_AlignSelf.DataSource = _alignSelf;
			luk_JustifyContent.DataSource = _justifyContent;
			luk_TextAlign.DataSource = _textAligns;
			luk_FontWeight.DataSource = _fontWeight;
			luk_ItemType.DataSource = _ItemType;
			luk_ColumnType.DataSource = _columnType;
			LoadCFLButtonHeader();
			dtListReport = await LoadData(-1, "", "", "");
			grd_listReport.DataSource = dtListReport;
			SetDataLukField(Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportId)));
			LoadDataLine();
			SetStatus(saveType.View);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_SettingReport_Load(object sender, EventArgs e)
	{
		await Load();
	}

	private void MapData(CellRange range, int fatherId)
	{
		List<Tuple<int, int, int, CellRange>> list = new List<Tuple<int, int, int, CellRange>>();
		bool flag = true;
		if (fatherId != -1)
		{
			for (int i = 0; i < range.RowCount; i++)
			{
				for (int j = 0; j < range.ColumnCount; j++)
				{
					IList<CellRange> mergedRanges = range[i, j].GetMergedRanges();
					if (mergedRanges.Count <= 0)
					{
						if (range.ColumnCount != 1)
						{
							flag = false;
							break;
						}
					}
					else if (mergedRanges[0].ColumnCount != range.ColumnCount)
					{
						flag = false;
						break;
					}
				}
			}
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			for (int k = 0; k < range.ExistingCells.ToList().Count; k++)
			{
				DataRow dataRow = dtSource.NewRow();
				int num = 0;
				if (dtSource.IsNotNull())
				{
					num = Function.ParseInt(Function.ToString(dtSource.AsEnumerable().Max((DataRow x) => x["Id"])));
				}
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = fatherId;
				dataRow["Value"] = Function.ToString(range.ExistingCells.ToList()[k].Value);
				dataRow["Type"] = "item";
				dataRow["Left"] = range.ExistingCells.ToList()[k].LeftColumnIndex;
				dataRow["Top"] = range.ExistingCells.ToList()[k].TopRowIndex;
				dataRow["Right"] = range.ExistingCells.ToList()[k].RightColumnIndex;
				dataRow["Bottom"] = range.ExistingCells.ToList()[k].BottomRowIndex;
				dtSource.Rows.Add(dataRow.ItemArray);
				dtSource.AcceptChanges();
			}
			return;
		}
		for (int num2 = 0; num2 < range.RowCount; num2++)
		{
			int num3 = 0;
			if (list.Count > 0 && list[list.Count - 1].Item4.BottomRowIndex >= range[num2, 0].TopRowIndex)
			{
				continue;
			}
			for (int num4 = 0; num4 < range.ColumnCount; num4++)
			{
				IList<CellRange> mergedRanges2 = range[num2, num4].GetMergedRanges();
				if (mergedRanges2.Count <= 0 && num3 < 1)
				{
					num3 = 1;
				}
				else if (mergedRanges2.Count > 0 && num3 < mergedRanges2[0].RowCount)
				{
					num3 = mergedRanges2[0].RowCount;
				}
			}
			int leftColumnIndex = range.LeftColumnIndex;
			int num5 = range.TopRowIndex;
			int rightColumnIndex = range.RightColumnIndex;
			int bottomRowIndex = range.BottomRowIndex;
			if (list.Count == 0)
			{
				bottomRowIndex = num5 + num3 - 1;
			}
			else
			{
				num5 = list[list.Count - 1].Item4.BottomRowIndex + 1;
				bottomRowIndex = list[list.Count - 1].Item4.BottomRowIndex + num3;
			}
			Tuple<int, int, int, CellRange> item = Tuple.Create(num5, bottomRowIndex, num3, sc_Main.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, num5, rightColumnIndex, bottomRowIndex));
			list.Add(item);
		}
		for (int num6 = 0; num6 < list.Count; num6++)
		{
			DataRow dataRow2 = dtSource.NewRow();
			int num7 = 0;
			if (dtSource.IsNotNull())
			{
				num7 = Function.ParseInt(Function.ToString(dtSource.AsEnumerable().Max((DataRow x) => x["Id"])));
			}
			dataRow2["Id"] = num7 + 1;
			dataRow2["FatherId"] = fatherId;
			dataRow2["Value"] = "";
			dataRow2["Type"] = "itemArr";
			dataRow2["Left"] = list[num6].Item4.LeftColumnIndex;
			dataRow2["Top"] = list[num6].Item4.TopRowIndex;
			dataRow2["Right"] = list[num6].Item4.RightColumnIndex;
			dataRow2["Bottom"] = list[num6].Item4.BottomRowIndex;
			dtSource.Rows.Add(dataRow2.ItemArray);
			dtSource.AcceptChanges();
			CellRange item2 = list[num6].Item4;
			List<Tuple<int, int, int, CellRange>> list2 = new List<Tuple<int, int, int, CellRange>>();
			for (int num8 = 0; num8 < item2.ColumnCount; num8++)
			{
				int num9 = 0;
				if (list2.Count > 0 && list2[list2.Count - 1].Item4.RightColumnIndex >= item2[0, num8].LeftColumnIndex)
				{
					continue;
				}
				for (int num10 = 0; num10 < item2.RowCount; num10++)
				{
					IList<CellRange> mergedRanges3 = item2[num10, num8].GetMergedRanges();
					if (mergedRanges3.Count <= 0 && num9 < 1)
					{
						num9 = 1;
					}
					else if (mergedRanges3.Count > 0 && num9 < mergedRanges3[0].ColumnCount)
					{
						num9 = mergedRanges3[0].ColumnCount;
					}
				}
				int num11 = item2.LeftColumnIndex;
				int topRowIndex = item2.TopRowIndex;
				int rightColumnIndex2 = item2.RightColumnIndex;
				int bottomRowIndex2 = item2.BottomRowIndex;
				if (list2.Count == 0)
				{
					rightColumnIndex2 = num11 + num9 - 1;
				}
				else
				{
					num11 = list2[list2.Count - 1].Item4.RightColumnIndex + 1;
					rightColumnIndex2 = list2[list2.Count - 1].Item4.RightColumnIndex + num9;
				}
				Tuple<int, int, int, CellRange> item3 = Tuple.Create(num11, rightColumnIndex2, num9, sc_Main.ActiveWorksheet.Range.FromLTRB(num11, topRowIndex, rightColumnIndex2, bottomRowIndex2));
				list2.Add(item3);
				if (sc_Main.ActiveWorksheet.Range.FromLTRB(num11, topRowIndex, rightColumnIndex2, bottomRowIndex2).ExistingCells.Where((Cell x) => !string.IsNullOrEmpty(Function.ToString(x.Value))).Count() == 1)
				{
					DataRow dataRow3 = dtSource.NewRow();
					int num12 = 0;
					if (dtSource.IsNotNull())
					{
						num12 = Function.ParseInt(Function.ToString(dtSource.AsEnumerable().Max((DataRow x) => x["Id"])));
					}
					dataRow3["Id"] = num12 + 1;
					dataRow3["FatherId"] = num7 + 1;
					dataRow3["Value"] = Function.ToString(sc_Main.ActiveWorksheet.Range.FromLTRB(num11, topRowIndex, rightColumnIndex2, bottomRowIndex2).ExistingCells.Where((Cell x) => !string.IsNullOrEmpty(Function.ToString(x.Value))).FirstOrDefault().Value);
					dataRow3["Type"] = "item";
					dataRow3["Left"] = num11;
					dataRow3["Top"] = topRowIndex;
					dataRow3["Right"] = rightColumnIndex2;
					dataRow3["Bottom"] = bottomRowIndex2;
					dtSource.Rows.Add(dataRow3.ItemArray);
					dtSource.AcceptChanges();
					continue;
				}
				DataRow dataRow4 = dtSource.NewRow();
				int num13 = 0;
				if (dtSource.IsNotNull())
				{
					num13 = Function.ParseInt(Function.ToString(dtSource.AsEnumerable().Max((DataRow x) => x["Id"])));
				}
				dataRow4["Id"] = num13 + 1;
				dataRow4["FatherId"] = num7 + 1;
				dataRow4["Value"] = "";
				dataRow4["Type"] = "rowArr";
				dataRow4["Left"] = num11;
				dataRow4["Top"] = topRowIndex;
				dataRow4["Right"] = rightColumnIndex2;
				dataRow4["Bottom"] = bottomRowIndex2;
				dtSource.Rows.Add(dataRow4.ItemArray);
				dtSource.AcceptChanges();
				MapData(sc_Main.ActiveWorksheet.Range.FromLTRB(num11, topRowIndex, rightColumnIndex2, bottomRowIndex2), num13 + 1);
			}
		}
	}

	private List<Tuple<int, int, int, int>> GetListCell(CellRange range)
	{
		List<Tuple<int, int, int, int>> list = new List<Tuple<int, int, int, int>>();
		try
		{
			for (int i = 0; i < range.RowCount; i++)
			{
				for (int j = 0; j < range.ColumnCount; j++)
				{
					IList<CellRange> mergedRanges = range[i, j].GetMergedRanges();
					if (mergedRanges.Count > 0)
					{
						CellRange cellRange = mergedRanges[0];
						int leftColumnIndex = cellRange.LeftColumnIndex;
						int topRowIndex = cellRange.TopRowIndex;
						int rightColumnIndex = cellRange.RightColumnIndex;
						int bottomRowIndex = cellRange.BottomRowIndex;
						bool flag = false;
						for (int k = 0; k < list.Count; k++)
						{
							if (leftColumnIndex == list[k].Item1 && topRowIndex == list[k].Item2 && rightColumnIndex == list[k].Item3 && bottomRowIndex == list[k].Item4)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list.Add(Tuple.Create(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex));
						}
						continue;
					}
					Cell cell = range[i, j];
					int leftColumnIndex2 = cell.LeftColumnIndex;
					int topRowIndex2 = cell.TopRowIndex;
					int rightColumnIndex2 = cell.RightColumnIndex;
					int bottomRowIndex2 = cell.BottomRowIndex;
					bool flag2 = false;
					for (int l = 0; l < list.Count; l++)
					{
						if (leftColumnIndex2 == list[l].Item1 && topRowIndex2 == list[l].Item2 && rightColumnIndex2 == list[l].Item3 && bottomRowIndex2 == list[l].Item4)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						list.Add(Tuple.Create(leftColumnIndex2, topRowIndex2, rightColumnIndex2, bottomRowIndex2));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return list;
	}

	private void NumberedSelection(List<Tuple<int, int, int, int>> listCell, CellRange range)
	{
		try
		{
			for (int i = 0; i < listCell.Count; i++)
			{
				sc_Main.ActiveWorksheet.Range.FromLTRB(listCell[i].Item1, listCell[i].Item2, listCell[i].Item3, listCell[i].Item4).Value = Function.ToString(i + 1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click_1(object sender, EventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
			dtProperties.Clear();
			CellRange selection = sc_Main.Selection;
			List<Tuple<int, int, int, int>> listCell = GetListCell(selection);
			NumberedSelection(listCell, selection);
			dtSource.Clear();
			MapData(selection, -1);
			int num = 0;
			for (int i = 0; i < dtSource.Rows.Count; i++)
			{
				DataRow dataRow = dtProperties.NewRow();
				dataRow["LeftIndex"] = Function.ToString(dtSource.Rows[i]["Left"]);
				dataRow["TopIndex"] = Function.ToString(dtSource.Rows[i]["Top"]);
				dataRow["RightIndex"] = Function.ToString(dtSource.Rows[i]["Right"]);
				dataRow["BottomIndex"] = Function.ToString(dtSource.Rows[i]["Bottom"]);
				dataRow["Type"] = Function.ToString(dtSource.Rows[i]["Type"]);
				dataRow["FatherNum"] = Function.ParseInt(dtSource.Rows[i]["FatherId"]);
				dataRow["Id"] = Function.ParseInt(dtSource.Rows[i]["Id"]);
				dataRow["size"] = 1;
				if (Function.ToString(dtSource.Rows[i]["Type"]) == "item")
				{
					dataRow["textAlign"] = "left";
					dataRow["fontSize"] = 13;
					dataRow["fontWeight"] = "normal";
					num++;
					dataRow["TableOrderId"] = num;
				}
				dtProperties.Rows.Add(dataRow.ItemArray);
			}
			dtProperties.AcceptChanges();
			grd_listProperties.DataSource = dtProperties;
			grd_listProperties.RefreshDataSource();
			RefreshDataSpreadSheet();
			NumberedSelection(listCell, sc_Main.Document.Range.FromLTRB(selection.LeftColumnIndex, selection.TopRowIndex, selection.RightColumnIndex, selection.BottomRowIndex));
			sc_Main.Document.Range.FromLTRB(selection.LeftColumnIndex, selection.TopRowIndex, selection.RightColumnIndex, selection.BottomRowIndex).Borders.SetAllBorders(Color.FromArgb(254, 185, 47), BorderLineStyle.Dotted);
			sc_Main.Document.Range.FromLTRB(selection.LeftColumnIndex, selection.TopRowIndex, selection.RightColumnIndex, selection.BottomRowIndex).Borders.SetOutsideBorders(Color.FromArgb(254, 185, 47), BorderLineStyle.Medium);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string MappingJson(int fatherId = -1)
	{
		DataTable source = dtSource.Copy();
		string text = "";
		EnumerableRowCollection<DataRow> source2 = from x in source.AsEnumerable()
			where Function.ToString(x["FatherId"]) == Function.ToString(fatherId)
			select x;
		DataTable dataTable = new DataTable();
		if (source2.Count() > 0)
		{
			dataTable = source2.CopyToDataTable();
		}
		for (int num = 0; num < dataTable.Rows.Count; num++)
		{
			EnumerableRowCollection<DataRow> source3 = from x in source.AsEnumerable()
				where Function.ToString(x["Id"]) == Function.ToString(fatherId)
				select x;
			DataTable dataTable2 = new DataTable();
			if (source3.Count() > 0)
			{
				dataTable2 = source3.CopyToDataTable();
			}
			string text2 = "";
			if (dataTable2.IsNotNull())
			{
				text2 = Function.ToString(dataTable2.Rows[0]["Type"]);
			}
			string text3 = Function.ToString(dataTable.Rows[num]["Type"]);
			if (text2 == "rowArr" && text3 != "itemArr")
			{
				text += "{\"itemArr\":[";
			}
			text += "{";
			if (text3 == "itemArr" || text3 == "rowArr")
			{
				if (text3 == "rowArr")
				{
					text += "\"size\":1,";
				}
				text = text + "\"" + text3 + "\":[";
				text += MappingJson(Function.ParseInt(dataTable.Rows[num]["Id"]));
				text += "]";
			}
			else
			{
				text += "\"size\":1,";
				text += "\"fieldName\":\"carcode\"";
			}
			text += "}";
			if (num != dataTable.Rows.Count - 1 && text2 != "rowArr")
			{
				text += ",";
			}
			if (text2 == "rowArr")
			{
				if (text3 != "itemArr")
				{
					text += "]}";
				}
				if (num != dataTable.Rows.Count - 1)
				{
					text += ",";
				}
			}
		}
		return text;
	}

	private void gv_listProperties_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			DataRowView dataRowView = (DataRowView)gv_listProperties.GetFocusedRow();
			if (dataRowView != null)
			{
				int leftColumnIndex = Function.ParseInt(dataRowView["LeftIndex"]);
				int topRowIndex = Function.ParseInt(dataRowView["TopIndex"]);
				int rightColumnIndex = Function.ParseInt(dataRowView["RightIndex"]);
				int bottomRowIndex = Function.ParseInt(dataRowView["BottomIndex"]);
				sc_Main.Selection = sc_Main.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_listProperties_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if ((gv_listProperties.FocusedColumn == gc_FieldName || gv_listProperties.FocusedColumn == gc_ItemType || gv_listProperties.FocusedColumn == gc_TextBefore || gv_listProperties.FocusedColumn == gc_TextAfter || gv_listProperties.FocusedColumn == gc_Color || gv_listProperties.FocusedColumn == gc_FontSize || gv_listProperties.FocusedColumn == gc_FontWeight || gv_listProperties.FocusedColumn == gc_TextAlign || gv_listProperties.FocusedColumn == gc_DemoData || gv_listProperties.FocusedColumn == gc_ColumnName || gv_listProperties.FocusedColumn == gc_TableOrderId) && Function.ToString(gv_listProperties.GetRowCellValue(gv_listProperties.FocusedRowHandle, gc_Type)) != "item")
			{
				e.Cancel = true;
			}
			else if ((gv_listProperties.FocusedColumn == gc_DateTimeDisplayFormat || gv_listProperties.FocusedColumn == gc_DateTimeValueFormat) && Function.ToString(gv_listProperties.GetRowCellValue(gv_listProperties.FocusedRowHandle, gc_ItemType)) != "DateTime")
			{
				e.Cancel = true;
			}
			else if (gv_listProperties.FocusedColumn == gc_currencyField && Function.ToString(gv_listProperties.GetRowCellValue(gv_listProperties.FocusedRowHandle, gc_ItemType)) != "PriceFormat")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string ShowTextValue(string value, string textBefore, string textAfter)
	{
		return textBefore + value + textAfter;
	}

	private void gv_listProperties_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
			if (isActiveFromBar)
			{
				isActiveFromBar = false;
				return;
			}
			int leftColumnIndex = Function.ParseInt(gv_listProperties.GetRowCellValue(e.RowHandle, gc_Left));
			int rightColumnIndex = Function.ParseInt(gv_listProperties.GetRowCellValue(e.RowHandle, gc_Right));
			int topRowIndex = Function.ParseInt(gv_listProperties.GetRowCellValue(e.RowHandle, gc_Top));
			int bottomRowIndex = Function.ParseInt(gv_listProperties.GetRowCellValue(e.RowHandle, gc_Bottom));
			CellRange cellRange = sc_Main.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex);
			if (e.Column == gc_FieldName)
			{
				if (string.IsNullOrEmpty(Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_DemoData))))
				{
					Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_FieldName));
					string textBefore = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextBefore));
					string textAfter = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextAfter));
					cellRange.Value = ShowTextValue(Function.ToString(e.Value), textBefore, textAfter);
				}
			}
			else if (e.Column == gc_ItemType)
			{
				if (Function.ToString(e.Value) != "DateTime")
				{
					gv_listProperties.SetRowCellValue(e.RowHandle, gc_DateTimeDisplayFormat, null);
					gv_listProperties.SetRowCellValue(e.RowHandle, gc_DateTimeValueFormat, null);
				}
				if (Function.ToString(e.Value) != "PriceFormat")
				{
					gv_listProperties.SetRowCellValue(e.RowHandle, gc_currencyField, null);
				}
			}
			else if (e.Column == gc_DemoData)
			{
				string value = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_FieldName));
				string textBefore2 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextBefore));
				string textAfter2 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextAfter));
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					cellRange.Value = ShowTextValue(value, textBefore2, textAfter2);
				}
				else
				{
					cellRange.Value = ShowTextValue(Function.ToString(e.Value), textBefore2, textAfter2);
				}
			}
			else if (e.Column == gc_TextAfter)
			{
				string value2 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_DemoData));
				string value3 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_FieldName));
				string textBefore3 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextBefore));
				string textAfter3 = Function.ToString(e.Value);
				if (string.IsNullOrEmpty(value2))
				{
					cellRange.Value = ShowTextValue(value3, textBefore3, textAfter3);
				}
				else
				{
					cellRange.Value = ShowTextValue(value2, textBefore3, textAfter3);
				}
			}
			else if (e.Column == gc_TextBefore)
			{
				string value4 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_DemoData));
				string value5 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_FieldName));
				string textAfter4 = Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_TextAfter));
				string textBefore4 = Function.ToString(e.Value);
				if (string.IsNullOrEmpty(value4))
				{
					cellRange.Value = ShowTextValue(value5, textBefore4, textAfter4);
				}
				else
				{
					cellRange.Value = ShowTextValue(value4, textBefore4, textAfter4);
				}
			}
			else if (e.Column == gc_FontSize)
			{
				cellRange.Font.Size = Function.ParseDouble(e.Value);
			}
			else if (e.Column == gc_FontWeight)
			{
				if (Function.ToString(e.Value) == "bold" || Function.ToString(e.Value) == "bolder")
				{
					cellRange.Font.Bold = true;
				}
				else
				{
					cellRange.Font.Bold = false;
				}
			}
			else if (e.Column == gc_TextAlign)
			{
				if (Function.ToString(e.Value) == "left")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
				}
				else if (Function.ToString(e.Value) == "right")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;
				}
				else if (Function.ToString(e.Value) == "center")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
				}
				else
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
				}
			}
			else if (e.Column == gc_Color)
			{
				cellRange.Font.Color = Color.FromArgb(Function.ParseInt(e.Value));
			}
			else
			{
				if (e.Column != gc_TableOrderId)
				{
					return;
				}
				string value6 = Function.ToString(Function.ParseInt(e.Value));
				DataTable source = grd_listProperties.DatasourceTable();
				bool flag = source.AsEnumerable().Any((DataRow row) => value6 == Function.ToString(row["TableOrderId"]));
				string value7 = Function.ToString(gv_listProperties.ActiveEditor.OldEditValue);
				if (flag)
				{
					int[] array = (from item in source.AsEnumerable().Select((DataRow row, int index) => new { row, index })
						where Function.ToString(item.row["TableOrderId"]) == value6 && item.index != e.RowHandle
						select item.index).ToArray();
					if (array.Length != 0)
					{
						gv_listProperties.CellValueChanged -= gv_listProperties_CellValueChanged;
						gv_listProperties.SetRowCellValue(array[0], gc_TableOrderId, value7);
						gv_listProperties.CellValueChanged += gv_listProperties_CellValueChanged;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ok_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			HttpResult httpResult;
			if (FormMode == Enums.FormViewModeType.Add)
			{
				WinCommons._OpenProcessing();
				httpResult = await AddNew();
				WinCommons._CloseProcessing();
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				httpResult = await UpdateAction();
				WinCommons._CloseProcessing();
			}
			else
			{
				httpResult = new HttpResult
				{
					Message = "Not found"
				};
			}
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Load();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetStatus(saveType type)
	{
		try
		{
			switch (type)
			{
			case saveType.Add:
				isAddNew = true;
				stripItem_Remove.Enabled = false;
				FormMode = Enums.FormViewModeType.Add;
				break;
			case saveType.Update:
				isAddNew = false;
				stripItem_Remove.Enabled = true;
				FormMode = Enums.FormViewModeType.Update;
				break;
			default:
				isAddNew = false;
				stripItem_Remove.Enabled = true;
				FormMode = Enums.FormViewModeType.View;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			SetStatus(saveType.Add);
			DataRow dataRow = grd_listReport.DatasourceTable().NewRow();
			dataRow["Active"] = "Y";
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["CreateDate"] = DateTime.Now;
			DataTable dataTable = dataRow.Table.Clone();
			dataTable.Rows.Add(dataRow.ItemArray);
			grd_listReport.DataSource = dataTable;
			grd_listReport.RefreshDataSource();
			LoadDataLine();
			SetDataLukField(Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_listReport_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshDataSpreadSheet()
	{
		try
		{
			sc_Main.CreateNewDocument();
			SetDefaultDocumentStyle();
			DataTable dataTable = grd_listProperties.DatasourceTable().Copy();
			if ((from x in dataTable.Copy().AsEnumerable()
				where Function.ToString(x["Type"]) == "item"
				select x).Count() > 0)
			{
				List<DataRow> list = (from x in dataTable.Copy().AsEnumerable()
					where Function.ToString(x["Type"]) == "item"
					select x).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					int leftColumnIndex = Function.ParseInt(list[num]["LeftIndex"]);
					int topRowIndex = Function.ParseInt(list[num]["TopIndex"]);
					int bottomRowIndex = Function.ParseInt(list[num]["BottomIndex"]);
					int rightColumnIndex = Function.ParseInt(list[num]["RightIndex"]);
					CellRange cellRange = sc_Main.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex);
					cellRange.Merge();
					cellRange.Value = Function.ToString(list[num]["fieldName"]);
					cellRange.FillColor = Color.FromArgb(47, 76, 92);
					cellRange.Font.Color = Color.FromArgb(255, 255, 255);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadDataLine()
	{
		_ = 3;
		try
		{
			sc_Main.CreateNewDocument();
			SetDefaultDocumentStyle();
			grd_listProperties.DataSource = new DataTable();
			grd_listProperties.RefreshDataSource();
			await LoadHeader();
			DataTable dataSource = await _httpClient.SettingReportClient.GetDataDashboardById(Function.ParseInt(gv_listReport.GetFocusedRowCellValue(gc_DocEntry)), -1);
			grd_dashboard.DataSource = dataSource;
			grd_dashboard.RefreshDataSource();
			DataTable dataSource2 = await GetDataChartById(Function.ParseInt(gv_listReport.GetFocusedRowCellValue(gc_DocEntry)));
			grd_Chart.DataSource = dataSource2;
			grd_Chart.RefreshDataSource();
			DataTable dataTable = await GetLineById(Function.ParseInt(gv_listReport.GetFocusedRowCellValue(gc_DocEntry)));
			grd_listProperties.DataSource = dataTable;
			grd_listProperties.RefreshDataSource();
			if ((from x in dataTable.Copy().AsEnumerable()
				where Function.ToString(x["Type"]) == "item"
				select x).Count() <= 0)
			{
				return;
			}
			List<DataRow> list = (from x in dataTable.Copy().AsEnumerable()
				where Function.ToString(x["Type"]) == "item"
				select x).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				int leftColumnIndex = Function.ParseInt(list[num]["LeftIndex"]);
				int topRowIndex = Function.ParseInt(list[num]["TopIndex"]);
				int bottomRowIndex = Function.ParseInt(list[num]["BottomIndex"]);
				int rightColumnIndex = Function.ParseInt(list[num]["RightIndex"]);
				CellRange cellRange = sc_Main.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex);
				cellRange.Merge();
				if (!string.IsNullOrEmpty(Function.ToString(list[num]["DemoData"])))
				{
					cellRange.Value = ShowTextValue(Function.ToString(list[num]["DemoData"]), Function.ToString(list[num]["textBefore"]), Function.ToString(list[num]["textAfter"]));
				}
				else
				{
					cellRange.Value = ShowTextValue(Function.ToString(list[num]["fieldName"]), Function.ToString(list[num]["textBefore"]), Function.ToString(list[num]["textAfter"]));
				}
				cellRange.FillColor = Color.FromArgb(47, 76, 92);
				if (!string.IsNullOrEmpty(Function.ToString(list[num]["color"])))
				{
					cellRange.Font.Color = Color.FromArgb(Function.ParseInt(list[num]["color"]));
				}
				else
				{
					cellRange.Font.Color = Color.FromArgb(255, 255, 255);
				}
				if (Function.ToString(list[num]["fontWeight"]) == "bold" || Function.ToString(list[num]["fontWeight"]) == "bolder")
				{
					cellRange.Font.Bold = true;
				}
				else
				{
					cellRange.Font.Bold = false;
				}
				if (Function.ToString(list[num]["textAlign"]) == "left")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
				}
				else if (Function.ToString(list[num]["textAlign"]) == "center")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
				}
				else if (Function.ToString(list[num]["textAlign"]) == "right")
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;
				}
				else
				{
					cellRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
				}
				if (!string.IsNullOrEmpty(Function.ToString(list[num]["fontSize"])))
				{
					cellRange.Font.Size = Function.ParseDouble(list[num]["fontSize"]);
				}
				else
				{
					cellRange.Font.Size = 13.0;
				}
			}
			sc_Main.ActiveWorksheet.GetUsedRange().Borders.SetAllBorders(Color.FromArgb(254, 185, 47), BorderLineStyle.Dotted);
			sc_Main.ActiveWorksheet.GetUsedRange().Borders.SetOutsideBorders(Color.FromArgb(254, 185, 47), BorderLineStyle.Medium);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_listReport_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			SetDataLukField(Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportId)));
			LoadDataLine();
			for (int i = 0; i < dtProperties.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_DocEntry))))
				{
					dtProperties.Rows[i]["DocEntry"] = Function.ParseInt(gv_listReport.GetFocusedRowCellValue(gc_DocEntry));
				}
				else
				{
					dtProperties.Rows[i]["DocEntry"] = DBNull.Value;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void stripItem_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			await Load();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int FindIndexRowPropertiesByPosition()
	{
		int result = -1;
		try
		{
			CellRange selection = sc_Main.Selection;
			int left = selection.LeftColumnIndex;
			int top = selection.TopRowIndex;
			int right = selection.RightColumnIndex;
			int bottom = selection.BottomRowIndex;
			DataTable dataTable = grd_listProperties.DatasourceTable();
			EnumerableRowCollection<DataRow> source = from x in dataTable.AsEnumerable()
				where Function.ToString(x["LeftIndex"]) == Function.ToString(left) && Function.ToString(x["RightIndex"]) == Function.ToString(right) && Function.ToString(x["TopIndex"]) == Function.ToString(top) && Function.ToString(x["BottomIndex"]) == Function.ToString(bottom)
				select x;
			if (source.Count() > 0)
			{
				EnumerableRowCollection<DataRow> source2 = source.Where((DataRow x) => Function.ToString(x["Type"]) == "item");
				int num = ((source.Count() <= 1 || source2.Count() <= 0) ? Function.ParseInt(source.ToList()[0]["Id"]) : Function.ParseInt(source2.ToList()[0]["Id"]));
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					if (Function.ToString(dataTable.Rows[num2]["Id"]) == Function.ToString(num))
					{
						result = num2;
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async void gv_listReport_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				e.Allow = false;
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					SetStatus(saveType.View);
					await Load();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void stripItem_Remove_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_DocEntry));
				string text = await DeleteAction(id);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await Load();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_listProperties_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (((e.Column == gc_FieldName || e.Column == gc_ItemType || e.Column == gc_TextBefore || e.Column == gc_TextAfter || e.Column == gc_Color || e.Column == gc_FontSize || e.Column == gc_FontWeight || e.Column == gc_TextAlign || e.Column == gc_DemoData || e.Column == gc_ColumnName || e.Column == gc_TableOrderId) && Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_Type)) != "item") || e.Column == gc_Type)
			{
				e.Appearance.BackColor = Color.FromArgb(204, 204, 204);
			}
			else if (e.Column == gc_DateTimeDisplayFormat || e.Column == gc_DateTimeValueFormat)
			{
				if (Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_ItemType)) != "DateTime")
				{
					e.Appearance.BackColor = Color.FromArgb(204, 204, 204);
				}
			}
			else if (e.Column == gc_currencyField && Function.ToString(gv_listProperties.GetRowCellValue(e.RowHandle, gc_ItemType)) != "PriceFormat")
			{
				e.Appearance.BackColor = Color.FromArgb(204, 204, 204);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetValueListPropertiesBarItemClick(GridColumnEx col, object value)
	{
		try
		{
			isActiveFromBar = true;
			int num = FindIndexRowPropertiesByPosition();
			if (num >= 0)
			{
				gv_listProperties.SetRowCellValue(num, col, value);
				gv_listProperties.RefreshData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void changeFontSizeItem1_EditValueChanged(object sender, EventArgs e)
	{
		SetValueListPropertiesBarItemClick(gc_FontSize, changeFontSizeItem1.EditValue);
	}

	private void barEditItem1_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (isFromLoadColor)
			{
				isFromLoadColor = false;
				return;
			}
			sc_Main.Selection.Font.Color = Color.FromArgb(Function.ParseInt(bar_fontColor.EditValue));
			SetValueListPropertiesBarItemClick(gc_Color, bar_fontColor.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spreadsheetCommandBarButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			double size = sc_Main.Selection.Font.Size;
			if (size <= 8.0)
			{
				SetValueListPropertiesBarItemClick(gc_FontSize, 8);
			}
			else
			{
				SetValueListPropertiesBarItemClick(gc_FontSize, size + 1.0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void sc_Main_SelectionChanged(object sender, EventArgs e)
	{
		try
		{
			int num = FindIndexRowPropertiesByPosition();
			if (num >= 0)
			{
				gv_listProperties.FocusedRowHandle = num;
			}
			Color color = sc_Main.Selection.Font.Color;
			isFromLoadColor = true;
			bar_fontColor.EditValue = Function.ParseInt(color.ToArgb());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void check_alignLeft_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (check_alignLeft.Checked)
		{
			SetValueListPropertiesBarItemClick(gc_TextAlign, "left");
		}
	}

	private void check_alignCenter_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (check_alignCenter.Checked)
		{
			SetValueListPropertiesBarItemClick(gc_TextAlign, "center");
		}
	}

	private void check_alignRight_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (check_alignRight.Checked)
		{
			SetValueListPropertiesBarItemClick(gc_TextAlign, "right");
		}
	}

	private void check_Bold_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (check_Bold.Checked)
		{
			SetValueListPropertiesBarItemClick(gc_FontWeight, "bold");
		}
		else
		{
			SetValueListPropertiesBarItemClick(gc_FontWeight, "normal");
		}
	}

	private void spreadsheetCommandBarButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			double size = sc_Main.Selection.Font.Size;
			if (size <= 8.0)
			{
				SetValueListPropertiesBarItemClick(gc_FontSize, 8);
			}
			else
			{
				SetValueListPropertiesBarItemClick(gc_FontSize, size - 1.0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (tab_dashboard.SelectedTabPage == xtraTabPageEx1)
			{
				simpleButton1.Visible = true;
				barDockControlTop.Visible = true;
			}
			else
			{
				simpleButton1.Visible = false;
				barDockControlTop.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
			DataTable dataTable = grd_Chart.DatasourceTable();
			string obj = "0";
			if (dataTable.IsNotNull())
			{
				obj = dataTable.AsEnumerable().Max((DataRow r) => r["OrderId"]).ToString();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["OrderId"] = Function.ParseInt(obj) + 1;
			dataRow["ColumnType"] = "I";
			dataTable.Rows.Add(dataRow.ItemArray);
			grd_Chart.DataSource = dataTable;
			grd_Chart.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Chart.DeleteRow(gv_Chart.FocusedRowHandle);
			gv_Chart.RefreshData();
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await Load();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Chart_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
			if (e.Column != gc_OrderId)
			{
				return;
			}
			string value = Function.ToString(Function.ParseInt(e.Value));
			DataTable source = grd_Chart.DatasourceTable();
			bool flag = source.AsEnumerable().Any((DataRow row) => value == Function.ToString(row["OrderId"]));
			string value2 = Function.ToString(gv_Chart.ActiveEditor.OldEditValue);
			if (flag)
			{
				int[] array = (from item in source.AsEnumerable().Select((DataRow row, int index) => new { row, index })
					where Function.ToString(item.row["OrderId"]) == value && item.index != e.RowHandle
					select item.index).ToArray();
				if (array.Length != 0)
				{
					gv_Chart.CellValueChanged -= gv_Chart_CellValueChanged;
					gv_Chart.SetRowCellValue(array[0], gc_OrderId, value2);
					gv_Chart.CellValueChanged += gv_Chart_CellValueChanged;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void HeaderAddNew()
	{
		try
		{
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void HeaderDelete()
	{
		try
		{
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void HeaderRefresh()
	{
		try
		{
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<HttpResult> AddNew()
	{
		try
		{
			DataTable dataTable = ((DataRowView)gv_listReport.GetFocusedRow()).Row.Table.Clone();
			dataTable.Rows.Add(((DataRowView)gv_listReport.GetFocusedRow()).Row.ItemArray);
			dataTable.AcceptChanges();
			DataTable table = ((DataView)gv_listProperties.DataSource).ToTable().Copy();
			DataTable table2 = ((DataView)gv_Chart.DataSource).ToTable().Copy();
			DataTable table3 = ((DataView)gv_dashboard.DataSource).ToTable().Copy();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			dataSet.Tables.Add(table3);
			dataSet.AcceptChanges();
			return await _httpClient.SettingReportClient.Add(dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = ex.Message
			};
		}
	}

	public virtual async Task<HttpResult> UpdateAction()
	{
		try
		{
			DataTable dataTable = ((DataRowView)gv_listReport.GetFocusedRow()).Row.Table.Clone();
			dataTable.Rows.Add(((DataRowView)gv_listReport.GetFocusedRow()).Row.ItemArray);
			dataTable.AcceptChanges();
			DataTable table = ((DataView)gv_listProperties.DataSource).ToTable().Copy();
			DataTable table2 = ((DataView)gv_Chart.DataSource).ToTable().Copy();
			DataTable table3 = ((DataView)gv_dashboard.DataSource).ToTable().Copy();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			dataSet.Tables.Add(table3);
			dataSet.AcceptChanges();
			return await _httpClient.SettingReportClient.Update(dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = ex.Message
			};
		}
	}

	public virtual async Task<string> DeleteAction(string id)
	{
		try
		{
			return await _httpClient.SettingReportClient.DeleteDocs(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		HeaderAddNew();
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		HeaderDelete();
	}

	private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		HeaderRefresh();
	}

	private void gv_Header_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
			if (e.Column != gc_HeaderOrderId)
			{
				return;
			}
			string value = Function.ToString(Function.ParseInt(e.Value));
			DataTable source = grd_Header.DatasourceTable();
			bool flag = source.AsEnumerable().Any((DataRow row) => value == Function.ToString(row["OrderId"]));
			string value2 = Function.ToString(gv_Header.ActiveEditor.OldEditValue);
			if (flag)
			{
				int[] array = (from item in source.AsEnumerable().Select((DataRow row, int index) => new { row, index })
					where Function.ToString(item.row["OrderId"]) == value && item.index != e.RowHandle
					select item.index).ToArray();
				if (array.Length != 0)
				{
					gv_Header.CellValueChanged -= gv_Header_CellValueChanged;
					gv_Header.SetRowCellValue(array[0], gc_HeaderOrderId, value2);
					gv_Header.CellValueChanged += gv_Header_CellValueChanged;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void btn_reportGrid_SelectedFromCFL(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.ChooseItem["Code"]);
			string value = Function.ToString(e.ChooseItem["Name"]);
			SetDataLukField(Function.ToString(text));
			_ = e.View.FocusedRowHandle;
			e.View.SetFocusedRowCellValue("ReportName", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DashboardObjectDataSource> GetDashboarDataSource()
	{
		try
		{
			Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportName));
			_ = gv_listReport.FocusedRowHandle;
			string code = Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportId));
			DataTable dataTable = (await _httpClient.SettingReportClient.GetDefaultDataSourceReport(code)).Clone();
			for (int i = 0; i < 2; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					DataColumn dataColumn = dataTable.Columns[j];
					if (dataColumn.DataType == typeof(short) || dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(long) || dataColumn.DataType == typeof(byte) || dataColumn.DataType == typeof(sbyte) || dataColumn.DataType == typeof(uint) || dataColumn.DataType == typeof(ulong) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(float))
					{
						Random random = new Random();
						dataRow[dataColumn] = random.Next((i + 1) * 10, 200);
					}
					else if (dataColumn.DataType == typeof(DateTime))
					{
						dataRow[dataColumn] = DateTime.Now.AddDays(i + 1);
					}
					else
					{
						dataRow[dataColumn] = dataColumn.ColumnName + " - " + (i + 1);
					}
				}
				dataTable.Rows.Add(dataRow.ItemArray);
			}
			dataTable.AcceptChanges();
			return new DashboardObjectDataSource("ds", dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DashboardObjectDataSource();
		}
	}

	private async Task SaveEditedDashboard(DevExpress.DashboardCommon.Dashboard dashboard, frm_DashboardDesigner frm, Enums.TransactionType transType = Enums.TransactionType.A)
	{
		try
		{
			Enums.FormViewModeType formMode = FormMode;
			try
			{
				DataTable dataTable = grd_dashboard.DatasourceTable().Copy();
				XDocument xDocument = dashboard.SaveToXDocument();
				DataRow dataRow;
				if (transType == Enums.TransactionType.A)
				{
					dataRow = dataTable.NewRow();
					dataRow["DashboardName"] = Function.ToString(dashboard.Title.Text);
				}
				else
				{
					dataRow = gv_dashboard.GetFocusedDataRow();
				}
				dataRow["Layout"] = xDocument.ToString();
				if (transType == Enums.TransactionType.A)
				{
					dataTable.Rows.Add(dataRow);
					txt_DashboardName.Text = "";
				}
				else
				{
					dataTable.Rows[gv_dashboard.FocusedRowHandle].ItemArray = dataRow.ItemArray;
				}
				dataTable.AcceptChanges();
				grd_dashboard.DataSource = dataTable;
				if (!isAddNew)
				{
					SetStatus(saveType.Update);
				}
				frm.Close();
				return;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = formMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async void btn_EditDesign_Click(object sender, EventArgs e)
	{
		try
		{
			string caption = (string.IsNullOrEmpty(txt_DashboardName.Text) ? Function.ToString(gv_listReport.GetFocusedRowCellValue(gc_ReportName)) : txt_DashboardName.Text);
			DashboardObjectDataSource item = await GetDashboarDataSource();
			DevExpress.DashboardCommon.Dashboard dashboard = new DevExpress.DashboardCommon.Dashboard();
			dashboard.DataSources.Add(item);
			frm_DashboardDesigner frm;
			if (dashboard != null)
			{
				dashboard.Title.Text = caption;
				dashboard.Title.Visible = false;
				dashboard.Title.ShowMasterFilterState = true;
				frm = new frm_DashboardDesigner(dashboard);
			}
			else
			{
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(new DataTable());
				frm = new frm_DashboardDesigner(dataSet, caption);
			}
			frm_DashboardDesigner frm_DashboardDesigner2 = frm;
			frm_DashboardDesigner2.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner2.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
			{
				await SaveEditedDashboard(obj, frm);
			}));
			frm.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_dashboard_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			gv_dashboard.DeleteRow(gv_dashboard.FocusedRowHandle);
			gv_dashboard.RefreshData();
			if (!isAddNew)
			{
				SetStatus(saveType.Update);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_SettingReport));
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem107 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem108 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem109 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem110 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem111 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem112 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem113 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem114 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem115 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem116 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem117 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem118 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem119 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem120 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem121 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem122 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem123 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem124 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem125 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem126 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem127 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem128 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem129 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem130 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem131 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem132 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem133 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem134 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem135 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem136 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem137 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem138 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem139 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem140 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem141 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem142 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem143 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem144 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem145 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem146 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem147 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem148 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem149 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem150 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem151 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem152 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem153 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem154 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem155 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem156 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem157 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem158 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem159 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem160 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem161 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem162 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem163 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem164 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem165 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem166 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem167 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem168 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem169 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem170 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem171 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem172 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem173 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem174 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem175 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem176 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem177 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem178 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem179 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem180 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem181 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem182 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem183 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem184 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem185 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem186 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem187 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem188 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem189 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem190 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem191 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem192 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem193 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem194 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem195 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem196 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem197 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem198 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem199 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem200 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem201 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem202 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem203 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem204 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem205 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem206 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem207 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem208 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem209 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem210 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem211 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem212 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem213 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem214 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem215 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem216 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem217 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem218 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem219 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem220 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem221 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem222 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem223 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem224 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem225 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem226 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem227 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem228 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem229 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem230 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem231 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem232 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem233 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem234 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem235 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem236 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem237 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem238 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem239 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem240 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem241 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem242 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem243 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem244 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem245 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem246 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem247 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem248 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem249 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem250 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem251 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem252 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem253 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem254 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
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
		this.commandBarGalleryDropDown1 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.fontBar1 = new DevExpress.XtraSpreadsheet.UI.FontBar();
		this.sc_Main = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
		this.changeFontSizeItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontSizeItem();
		this.repositoryItemSpreadsheetFontSizeEdit1 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit();
		this.spreadsheetCommandBarButtonItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.check_Bold = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.changeBorderLineColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineColorItem();
		this.changeBorderLineStyleItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineStyleItem();
		this.commandBarGalleryDropDown23 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.bar_fontColor = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemColorPickEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.alignmentBar1 = new DevExpress.XtraSpreadsheet.UI.AlignmentBar();
		this.spreadsheetCommandBarCheckItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.check_alignLeft = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.check_alignCenter = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.check_alignRight = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.spreadsheetCommandBarButtonItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.changeFontNameItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontNameItem();
		this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
		this.spreadsheetCommandBarCheckItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.changeCellFillColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeCellFillColorItem();
		this.changeFontColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontColorItem();
		this.changeNumberFormatItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeNumberFormatItem();
		this.repositoryItemPopupGalleryEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupGalleryEdit();
		this.spreadsheetCommandBarSubItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarSubItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown24 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown25 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown26 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.galleryFormatAsTableItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryFormatAsTableItem();
		this.commandBarGalleryDropDown27 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.changeStyleItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeStyleItem();
		this.repositoryItemSpreadsheetCellStyleEdit1 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetCellStyleEdit();
		this.spreadsheetCommandBarSubItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.changeSheetTabColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeSheetTabColorItem();
		this.spreadsheetCommandBarButtonItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.commandBarGalleryDropDown18 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown19 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown20 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown21 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown22 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown6 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown7 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown8 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown9 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown10 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown11 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown12 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown13 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown14 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown15 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown16 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown17 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown2 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown3 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown4 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.commandBarGalleryDropDown5 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_listReport = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.stripItem_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.stripItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.stripItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_listReport = new Apzon.Controls.Grid.GridViewEx();
		this.gc_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ReportId = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_reportGrid = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ReportName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.tab_dashboard = new Apzon.Controls.Controls.XtraTabControlEx();
		this.contextMenuStrip_Chart = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_listProperties = new Apzon.Controls.Grid.GridControlEx();
		this.gv_listProperties = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItemType = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ItemType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_FieldName = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_FieldName = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
		this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gc_ColumnName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_TableOrderId = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit7 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_DemoData = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gc_size = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Size = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_TextBefore = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_TextAfter = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Color = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemColorPickEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.gc_AlignItems = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_AlignItem = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_JustifyContent = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_JustifyContent = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_FontSize = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_FontWeight = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_FontWeight = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_PaddingTop = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_PaddingBottom = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_PaddingLeft = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_PaddingRight = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_TextAlign = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TextAlign = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_AlignSelf = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_AlignSelf = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Left = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Top = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Right = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Bottom = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DateTimeValueFormat = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DateTimeDisplayFormat = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_currencyField = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_currencyField = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
		this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.xtraTabPageEx3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Header = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Header = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Header = new Apzon.Controls.Grid.GridViewEx();
		this.gc_HeaderOrderId = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit8 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_HeaderFieldName = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_HeaderFieldName = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
		this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gc_HeaderDataType = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_HeaderDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_HeaderDisplayText = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Chart = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Chart = new Apzon.Controls.Grid.GridViewEx();
		this.gc_OrderId = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_FieldNameChart = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ChartFieldName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_DisplayText = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ColumnType = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ColumnType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ChartColor = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemColorPickEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.luk_DataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.check_ChartVisible = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.check_IsKey = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
		this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.tabPage_dashboard = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_DashboardName = new Apzon.Controls.Editor.TextEditEx();
		this.grd_dashboard = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Dashboard = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_dashboard = new Apzon.Controls.Grid.GridViewEx();
		this.gc_dashboardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_XMLLayout = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_detailDashboard = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_EditDesign = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.clipboardBar1 = new DevExpress.XtraSpreadsheet.UI.ClipboardBar();
		this.spreadsheetBarController1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController(this.components);
		this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetFontSizeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPopupGalleryEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetCellStyleEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_listReport).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_listReport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_reportGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_dashboard).BeginInit();
		this.tab_dashboard.SuspendLayout();
		this.contextMenuStrip_Chart.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_listProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_listProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FieldName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Size).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AlignItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_JustifyContent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FontWeight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TextAlign).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AlignSelf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_currencyField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		this.xtraTabPageEx3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Header).BeginInit();
		this.contextMenuStrip_Header.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HeaderFieldName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HeaderDataType).BeginInit();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Chart).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Chart).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ChartFieldName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ColumnType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_ChartVisible).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_IsKey).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		this.tabPage_dashboard.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_DashboardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_dashboard).BeginInit();
		this.contextMenuStrip_Dashboard.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_dashboard).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_detailDashboard).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetBarController1).BeginInit();
		base.SuspendLayout();
		this.commandBarGalleryDropDown1.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown1.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown1.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.commandBarGalleryDropDown1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown1.Gallery.ItemSize = new System.Drawing.Size(136, 26);
		this.commandBarGalleryDropDown1.Gallery.RowCount = 14;
		this.commandBarGalleryDropDown1.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown1.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown1.Manager = this.barManager1;
		this.commandBarGalleryDropDown1.Name = "commandBarGalleryDropDown1";
		this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[2] { this.fontBar1, this.alignmentBar1 });
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[150]
		{
			this.spreadsheetCommandBarButtonItem1, this.spreadsheetCommandBarButtonItem2, this.spreadsheetCommandBarButtonItem3, this.spreadsheetCommandBarButtonItem4, this.changeFontNameItem1, this.changeFontSizeItem1, this.spreadsheetCommandBarButtonItem5, this.spreadsheetCommandBarButtonItem6, this.check_Bold, this.spreadsheetCommandBarCheckItem2,
			this.spreadsheetCommandBarCheckItem3, this.spreadsheetCommandBarCheckItem4, this.spreadsheetCommandBarSubItem1, this.spreadsheetCommandBarButtonItem7, this.spreadsheetCommandBarButtonItem8, this.spreadsheetCommandBarButtonItem9, this.spreadsheetCommandBarButtonItem10, this.spreadsheetCommandBarButtonItem11, this.spreadsheetCommandBarButtonItem12, this.spreadsheetCommandBarButtonItem13,
			this.spreadsheetCommandBarButtonItem14, this.spreadsheetCommandBarButtonItem15, this.spreadsheetCommandBarButtonItem16, this.spreadsheetCommandBarButtonItem17, this.spreadsheetCommandBarButtonItem18, this.spreadsheetCommandBarButtonItem19, this.changeBorderLineColorItem1, this.changeBorderLineStyleItem1, this.changeCellFillColorItem1, this.changeFontColorItem1,
			this.spreadsheetCommandBarCheckItem5, this.spreadsheetCommandBarCheckItem6, this.spreadsheetCommandBarCheckItem7, this.check_alignLeft, this.check_alignCenter, this.check_alignRight, this.spreadsheetCommandBarButtonItem20, this.spreadsheetCommandBarButtonItem21, this.spreadsheetCommandBarCheckItem11, this.spreadsheetCommandBarSubItem2,
			this.spreadsheetCommandBarCheckItem12, this.spreadsheetCommandBarButtonItem22, this.spreadsheetCommandBarButtonItem23, this.spreadsheetCommandBarButtonItem24, this.changeNumberFormatItem1, this.spreadsheetCommandBarSubItem3, this.spreadsheetCommandBarButtonItem25, this.spreadsheetCommandBarButtonItem26, this.spreadsheetCommandBarButtonItem27, this.spreadsheetCommandBarButtonItem28,
			this.spreadsheetCommandBarButtonItem29, this.spreadsheetCommandBarButtonItem30, this.spreadsheetCommandBarButtonItem31, this.spreadsheetCommandBarButtonItem32, this.spreadsheetCommandBarButtonItem33, this.spreadsheetCommandBarButtonItem34, this.spreadsheetCommandBarSubItem7, this.spreadsheetCommandBarButtonItem35, this.spreadsheetCommandBarButtonItem36, this.spreadsheetCommandBarButtonItem37,
			this.spreadsheetCommandBarButtonItem38, this.spreadsheetCommandBarButtonItem39, this.spreadsheetCommandBarButtonItem40, this.spreadsheetCommandBarButtonItem41, this.spreadsheetCommandBarSubItem4, this.spreadsheetCommandBarButtonItem42, this.spreadsheetCommandBarButtonItem43, this.spreadsheetCommandBarButtonItem44, this.spreadsheetCommandBarButtonItem45, this.spreadsheetCommandBarButtonItem46,
			this.spreadsheetCommandBarButtonItem47, this.spreadsheetCommandBarSubItem5, this.spreadsheetCommandBarButtonGalleryDropDownItem1, this.spreadsheetCommandBarButtonGalleryDropDownItem2, this.spreadsheetCommandBarButtonGalleryDropDownItem3, this.spreadsheetCommandBarButtonItem48, this.spreadsheetCommandBarButtonItem49, this.spreadsheetCommandBarButtonItem50, this.spreadsheetCommandBarSubItem6, this.spreadsheetCommandBarButtonItem51,
			this.galleryFormatAsTableItem1, this.changeStyleItem1, this.spreadsheetCommandBarSubItem8, this.spreadsheetCommandBarButtonItem52, this.spreadsheetCommandBarButtonItem53, this.spreadsheetCommandBarButtonItem54, this.spreadsheetCommandBarButtonItem55, this.spreadsheetCommandBarButtonItem56, this.spreadsheetCommandBarButtonItem57, this.spreadsheetCommandBarButtonItem58,
			this.spreadsheetCommandBarButtonItem59, this.spreadsheetCommandBarSubItem9, this.spreadsheetCommandBarButtonItem60, this.spreadsheetCommandBarButtonItem61, this.spreadsheetCommandBarButtonItem62, this.spreadsheetCommandBarButtonItem63, this.spreadsheetCommandBarButtonItem64, this.spreadsheetCommandBarButtonItem65, this.spreadsheetCommandBarSubItem11, this.spreadsheetCommandBarButtonItem66,
			this.spreadsheetCommandBarButtonItem67, this.spreadsheetCommandBarButtonItem68, this.spreadsheetCommandBarButtonItem69, this.spreadsheetCommandBarButtonItem70, this.spreadsheetCommandBarButtonItem71, this.spreadsheetCommandBarButtonItem72, this.spreadsheetCommandBarButtonItem73, this.spreadsheetCommandBarButtonItem74, this.spreadsheetCommandBarButtonItem75, this.spreadsheetCommandBarButtonItem76,
			this.spreadsheetCommandBarSubItem10, this.spreadsheetCommandBarButtonItem77, this.spreadsheetCommandBarButtonItem78, this.changeSheetTabColorItem1, this.spreadsheetCommandBarButtonItem79, this.spreadsheetCommandBarCheckItem13, this.spreadsheetCommandBarButtonItem80, this.spreadsheetCommandBarSubItem12, this.spreadsheetCommandBarButtonItem81, this.spreadsheetCommandBarButtonItem82,
			this.spreadsheetCommandBarButtonItem83, this.spreadsheetCommandBarButtonItem84, this.spreadsheetCommandBarButtonItem85, this.spreadsheetCommandBarSubItem13, this.spreadsheetCommandBarButtonItem86, this.spreadsheetCommandBarButtonItem87, this.spreadsheetCommandBarButtonItem88, this.spreadsheetCommandBarButtonItem89, this.spreadsheetCommandBarSubItem14, this.spreadsheetCommandBarButtonItem90,
			this.spreadsheetCommandBarButtonItem91, this.spreadsheetCommandBarButtonItem92, this.spreadsheetCommandBarButtonItem93, this.spreadsheetCommandBarButtonItem94, this.spreadsheetCommandBarButtonItem95, this.spreadsheetCommandBarSubItem15, this.spreadsheetCommandBarButtonItem96, this.spreadsheetCommandBarButtonItem97, this.spreadsheetCommandBarCheckItem14, this.spreadsheetCommandBarButtonItem98,
			this.spreadsheetCommandBarButtonItem99, this.spreadsheetCommandBarSubItem16, this.spreadsheetCommandBarButtonItem100, this.spreadsheetCommandBarButtonItem101, this.spreadsheetCommandBarButtonItem102, this.spreadsheetCommandBarButtonItem103, this.spreadsheetCommandBarButtonItem104, this.spreadsheetCommandBarButtonItem105, this.spreadsheetCommandBarButtonItem106, this.bar_fontColor
		});
		this.barManager1.MaxItemId = 151;
		this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemFontEdit1, this.repositoryItemSpreadsheetFontSizeEdit1, this.repositoryItemPopupGalleryEdit1, this.repositoryItemSpreadsheetCellStyleEdit1, this.repositoryItemColorPickEdit2 });
		this.fontBar1.Control = this.sc_Main;
		this.fontBar1.DockCol = 1;
		this.fontBar1.DockRow = 0;
		this.fontBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.fontBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontSizeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.check_Bold),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_fontColor)
		});
		this.fontBar1.Offset = 361;
		this.sc_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.sc_Main.Location = new System.Drawing.Point(0, 0);
		this.sc_Main.MenuManager = this.barManager1;
		this.sc_Main.Name = "sc_Main";
		this.sc_Main.Options.Culture = new System.Globalization.CultureInfo("en-US");
		this.sc_Main.Size = new System.Drawing.Size(427, 362);
		this.sc_Main.TabIndex = 2;
		this.sc_Main.Text = "spreadsheetControl1";
		this.sc_Main.SelectionChanged += new System.EventHandler(this.sc_Main_SelectionChanged);
		this.changeFontSizeItem1.Edit = this.repositoryItemSpreadsheetFontSizeEdit1;
		this.changeFontSizeItem1.Id = 1;
		this.changeFontSizeItem1.Name = "changeFontSizeItem1";
		this.changeFontSizeItem1.EditValueChanged += new System.EventHandler(this.changeFontSizeItem1_EditValueChanged);
		this.repositoryItemSpreadsheetFontSizeEdit1.AutoHeight = false;
		this.repositoryItemSpreadsheetFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSpreadsheetFontSizeEdit1.Control = this.sc_Main;
		this.repositoryItemSpreadsheetFontSizeEdit1.Name = "repositoryItemSpreadsheetFontSizeEdit1";
		this.spreadsheetCommandBarButtonItem5.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		this.spreadsheetCommandBarButtonItem5.CommandName = "FormatIncreaseFontSize";
		this.spreadsheetCommandBarButtonItem5.Id = 2;
		this.spreadsheetCommandBarButtonItem5.Name = "spreadsheetCommandBarButtonItem5";
		this.spreadsheetCommandBarButtonItem5.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.spreadsheetCommandBarButtonItem5_ItemClick);
		this.spreadsheetCommandBarButtonItem6.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		this.spreadsheetCommandBarButtonItem6.CommandName = "FormatDecreaseFontSize";
		this.spreadsheetCommandBarButtonItem6.Id = 3;
		this.spreadsheetCommandBarButtonItem6.Name = "spreadsheetCommandBarButtonItem6";
		this.spreadsheetCommandBarButtonItem6.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.spreadsheetCommandBarButtonItem6_ItemClick);
		this.check_Bold.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.check_Bold.CommandName = "FormatFontBold";
		this.check_Bold.Id = 4;
		this.check_Bold.Name = "check_Bold";
		this.check_Bold.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.check_Bold_ItemClick);
		this.spreadsheetCommandBarSubItem1.ButtonGroupTag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
		this.spreadsheetCommandBarSubItem1.CommandName = "FormatBordersCommandGroup";
		this.spreadsheetCommandBarSubItem1.Id = 8;
		this.spreadsheetCommandBarSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[15]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem8),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem9),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem10),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem19),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineStyleItem1)
		});
		this.spreadsheetCommandBarSubItem1.Name = "spreadsheetCommandBarSubItem1";
		this.spreadsheetCommandBarSubItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem7.CommandName = "FormatBottomBorder";
		this.spreadsheetCommandBarButtonItem7.Id = 9;
		this.spreadsheetCommandBarButtonItem7.Name = "spreadsheetCommandBarButtonItem7";
		this.spreadsheetCommandBarButtonItem8.CommandName = "FormatTopBorder";
		this.spreadsheetCommandBarButtonItem8.Id = 10;
		this.spreadsheetCommandBarButtonItem8.Name = "spreadsheetCommandBarButtonItem8";
		this.spreadsheetCommandBarButtonItem9.CommandName = "FormatLeftBorder";
		this.spreadsheetCommandBarButtonItem9.Id = 11;
		this.spreadsheetCommandBarButtonItem9.Name = "spreadsheetCommandBarButtonItem9";
		this.spreadsheetCommandBarButtonItem10.CommandName = "FormatRightBorder";
		this.spreadsheetCommandBarButtonItem10.Id = 12;
		this.spreadsheetCommandBarButtonItem10.Name = "spreadsheetCommandBarButtonItem10";
		this.spreadsheetCommandBarButtonItem11.CommandName = "FormatNoBorders";
		this.spreadsheetCommandBarButtonItem11.Id = 13;
		this.spreadsheetCommandBarButtonItem11.Name = "spreadsheetCommandBarButtonItem11";
		this.spreadsheetCommandBarButtonItem12.CommandName = "FormatAllBorders";
		this.spreadsheetCommandBarButtonItem12.Id = 14;
		this.spreadsheetCommandBarButtonItem12.Name = "spreadsheetCommandBarButtonItem12";
		this.spreadsheetCommandBarButtonItem13.CommandName = "FormatOutsideBorders";
		this.spreadsheetCommandBarButtonItem13.Id = 15;
		this.spreadsheetCommandBarButtonItem13.Name = "spreadsheetCommandBarButtonItem13";
		this.spreadsheetCommandBarButtonItem14.CommandName = "FormatThickBorder";
		this.spreadsheetCommandBarButtonItem14.Id = 16;
		this.spreadsheetCommandBarButtonItem14.Name = "spreadsheetCommandBarButtonItem14";
		this.spreadsheetCommandBarButtonItem15.CommandName = "FormatBottomDoubleBorder";
		this.spreadsheetCommandBarButtonItem15.Id = 17;
		this.spreadsheetCommandBarButtonItem15.Name = "spreadsheetCommandBarButtonItem15";
		this.spreadsheetCommandBarButtonItem16.CommandName = "FormatBottomThickBorder";
		this.spreadsheetCommandBarButtonItem16.Id = 18;
		this.spreadsheetCommandBarButtonItem16.Name = "spreadsheetCommandBarButtonItem16";
		this.spreadsheetCommandBarButtonItem17.CommandName = "FormatTopAndBottomBorder";
		this.spreadsheetCommandBarButtonItem17.Id = 19;
		this.spreadsheetCommandBarButtonItem17.Name = "spreadsheetCommandBarButtonItem17";
		this.spreadsheetCommandBarButtonItem18.CommandName = "FormatTopAndThickBottomBorder";
		this.spreadsheetCommandBarButtonItem18.Id = 20;
		this.spreadsheetCommandBarButtonItem18.Name = "spreadsheetCommandBarButtonItem18";
		this.spreadsheetCommandBarButtonItem19.CommandName = "FormatTopAndDoubleBottomBorder";
		this.spreadsheetCommandBarButtonItem19.Id = 21;
		this.spreadsheetCommandBarButtonItem19.Name = "spreadsheetCommandBarButtonItem19";
		this.changeBorderLineColorItem1.ActAsDropDown = true;
		this.changeBorderLineColorItem1.Id = 22;
		this.changeBorderLineColorItem1.Name = "changeBorderLineColorItem1";
		this.changeBorderLineStyleItem1.DropDownControl = this.commandBarGalleryDropDown23;
		this.changeBorderLineStyleItem1.Id = 23;
		this.changeBorderLineStyleItem1.Name = "changeBorderLineStyleItem1";
		this.commandBarGalleryDropDown23.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown23.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown23.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown23.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.commandBarGalleryDropDown23.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown23.Gallery.ItemSize = new System.Drawing.Size(136, 26);
		this.commandBarGalleryDropDown23.Gallery.RowCount = 14;
		this.commandBarGalleryDropDown23.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown23.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown23.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown23.Manager = this.barManager1;
		this.commandBarGalleryDropDown23.Name = "commandBarGalleryDropDown23";
		this.bar_fontColor.Caption = "barEditItem1";
		this.bar_fontColor.Edit = this.repositoryItemColorPickEdit2;
		this.bar_fontColor.Id = 149;
		this.bar_fontColor.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_fontColor.ImageOptions.Image");
		this.bar_fontColor.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_fontColor.ImageOptions.LargeImage");
		this.bar_fontColor.Name = "bar_fontColor";
		this.bar_fontColor.EditValueChanged += new System.EventHandler(this.barEditItem1_EditValueChanged);
		this.repositoryItemColorPickEdit2.AutoHeight = false;
		this.repositoryItemColorPickEdit2.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit2.Name = "repositoryItemColorPickEdit2";
		this.repositoryItemColorPickEdit2.StoreColorAsInteger = true;
		this.alignmentBar1.Control = this.sc_Main;
		this.alignmentBar1.DockCol = 0;
		this.alignmentBar1.DockRow = 0;
		this.alignmentBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.alignmentBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[10]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.check_alignLeft),
			new DevExpress.XtraBars.LinkPersistInfo(this.check_alignCenter),
			new DevExpress.XtraBars.LinkPersistInfo(this.check_alignRight),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem20),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem21),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem2)
		});
		this.alignmentBar1.Offset = 1;
		this.spreadsheetCommandBarCheckItem5.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem5.CommandName = "FormatAlignmentTop";
		this.spreadsheetCommandBarCheckItem5.Id = 26;
		this.spreadsheetCommandBarCheckItem5.Name = "spreadsheetCommandBarCheckItem5";
		this.spreadsheetCommandBarCheckItem5.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem6.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem6.CommandName = "FormatAlignmentMiddle";
		this.spreadsheetCommandBarCheckItem6.Id = 27;
		this.spreadsheetCommandBarCheckItem6.Name = "spreadsheetCommandBarCheckItem6";
		this.spreadsheetCommandBarCheckItem6.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem7.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem7.CommandName = "FormatAlignmentBottom";
		this.spreadsheetCommandBarCheckItem7.Id = 28;
		this.spreadsheetCommandBarCheckItem7.Name = "spreadsheetCommandBarCheckItem7";
		this.spreadsheetCommandBarCheckItem7.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.check_alignLeft.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.check_alignLeft.CommandName = "FormatAlignmentLeft";
		this.check_alignLeft.Id = 29;
		this.check_alignLeft.Name = "check_alignLeft";
		this.check_alignLeft.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.check_alignLeft_ItemClick);
		this.check_alignCenter.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.check_alignCenter.CommandName = "FormatAlignmentCenter";
		this.check_alignCenter.Id = 30;
		this.check_alignCenter.Name = "check_alignCenter";
		this.check_alignCenter.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.check_alignCenter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.check_alignCenter_ItemClick);
		this.check_alignRight.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.check_alignRight.CommandName = "FormatAlignmentRight";
		this.check_alignRight.Id = 31;
		this.check_alignRight.Name = "check_alignRight";
		this.check_alignRight.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.check_alignRight_ItemClick);
		this.spreadsheetCommandBarButtonItem20.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		this.spreadsheetCommandBarButtonItem20.CommandName = "FormatDecreaseIndent";
		this.spreadsheetCommandBarButtonItem20.Id = 32;
		this.spreadsheetCommandBarButtonItem20.Name = "spreadsheetCommandBarButtonItem20";
		this.spreadsheetCommandBarButtonItem20.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem21.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		this.spreadsheetCommandBarButtonItem21.CommandName = "FormatIncreaseIndent";
		this.spreadsheetCommandBarButtonItem21.Id = 33;
		this.spreadsheetCommandBarButtonItem21.Name = "spreadsheetCommandBarButtonItem21";
		this.spreadsheetCommandBarButtonItem21.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem11.CommandName = "FormatWrapText";
		this.spreadsheetCommandBarCheckItem11.Id = 34;
		this.spreadsheetCommandBarCheckItem11.Name = "spreadsheetCommandBarCheckItem11";
		this.spreadsheetCommandBarCheckItem11.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarSubItem2.CommandName = "EditingMergeCellsCommandGroup";
		this.spreadsheetCommandBarSubItem2.Id = 35;
		this.spreadsheetCommandBarSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem22),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem24),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem23)
		});
		this.spreadsheetCommandBarSubItem2.Name = "spreadsheetCommandBarSubItem2";
		this.spreadsheetCommandBarSubItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem12.CommandName = "EditingMergeAndCenterCells";
		this.spreadsheetCommandBarCheckItem12.Id = 36;
		this.spreadsheetCommandBarCheckItem12.Name = "spreadsheetCommandBarCheckItem12";
		this.spreadsheetCommandBarButtonItem22.CommandName = "EditingMergeCellsAcross";
		this.spreadsheetCommandBarButtonItem22.Id = 37;
		this.spreadsheetCommandBarButtonItem22.Name = "spreadsheetCommandBarButtonItem22";
		this.spreadsheetCommandBarButtonItem24.CommandName = "EditingUnmergeCells";
		this.spreadsheetCommandBarButtonItem24.Id = 39;
		this.spreadsheetCommandBarButtonItem24.Name = "spreadsheetCommandBarButtonItem24";
		this.spreadsheetCommandBarButtonItem23.CommandName = "EditingMergeCells";
		this.spreadsheetCommandBarButtonItem23.Id = 38;
		this.spreadsheetCommandBarButtonItem23.Name = "spreadsheetCommandBarButtonItem23";
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1007, 24);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 637);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1007, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 613);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1007, 24);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 613);
		this.spreadsheetCommandBarButtonItem1.CommandName = "PasteSelection";
		this.spreadsheetCommandBarButtonItem1.Id = 40;
		this.spreadsheetCommandBarButtonItem1.Name = "spreadsheetCommandBarButtonItem1";
		this.spreadsheetCommandBarButtonItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem2.CommandName = "CutSelection";
		this.spreadsheetCommandBarButtonItem2.Id = 41;
		this.spreadsheetCommandBarButtonItem2.Name = "spreadsheetCommandBarButtonItem2";
		this.spreadsheetCommandBarButtonItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem3.CommandName = "CopySelection";
		this.spreadsheetCommandBarButtonItem3.Id = 42;
		this.spreadsheetCommandBarButtonItem3.Name = "spreadsheetCommandBarButtonItem3";
		this.spreadsheetCommandBarButtonItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem4.CommandName = "ShowPasteSpecialForm";
		this.spreadsheetCommandBarButtonItem4.Id = 43;
		this.spreadsheetCommandBarButtonItem4.Name = "spreadsheetCommandBarButtonItem4";
		this.spreadsheetCommandBarButtonItem4.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.changeFontNameItem1.Edit = this.repositoryItemFontEdit1;
		this.changeFontNameItem1.Id = 0;
		this.changeFontNameItem1.Name = "changeFontNameItem1";
		this.repositoryItemFontEdit1.AutoHeight = false;
		this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
		this.spreadsheetCommandBarCheckItem2.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem2.CommandName = "FormatFontItalic";
		this.spreadsheetCommandBarCheckItem2.Id = 5;
		this.spreadsheetCommandBarCheckItem2.Name = "spreadsheetCommandBarCheckItem2";
		this.spreadsheetCommandBarCheckItem3.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem3.CommandName = "FormatFontUnderline";
		this.spreadsheetCommandBarCheckItem3.Id = 6;
		this.spreadsheetCommandBarCheckItem3.Name = "spreadsheetCommandBarCheckItem3";
		this.spreadsheetCommandBarCheckItem4.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem4.CommandName = "FormatFontStrikeout";
		this.spreadsheetCommandBarCheckItem4.Id = 7;
		this.spreadsheetCommandBarCheckItem4.Name = "spreadsheetCommandBarCheckItem4";
		this.changeCellFillColorItem1.Id = 24;
		this.changeCellFillColorItem1.Name = "changeCellFillColorItem1";
		this.changeFontColorItem1.Id = 25;
		this.changeFontColorItem1.Name = "changeFontColorItem1";
		this.changeNumberFormatItem1.Edit = this.repositoryItemPopupGalleryEdit1;
		this.changeNumberFormatItem1.Id = 44;
		this.changeNumberFormatItem1.Name = "changeNumberFormatItem1";
		this.repositoryItemPopupGalleryEdit1.AutoHeight = false;
		this.repositoryItemPopupGalleryEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemPopupGalleryEdit1.Gallery.AllowFilter = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.AutoFitColumns = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.ColumnCount = 1;
		this.repositoryItemPopupGalleryEdit1.Gallery.FixedImageSize = false;
		spreadsheetCommandGalleryItem.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem.CaptionAsValue = true;
		spreadsheetCommandGalleryItem.Checked = true;
		spreadsheetCommandGalleryItem.CommandName = "FormatNumberGeneral";
		spreadsheetCommandGalleryItem.IsEmptyHint = true;
		spreadsheetCommandGalleryItem2.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem2.CaptionAsValue = true;
		spreadsheetCommandGalleryItem2.CommandName = "FormatNumberDecimal";
		spreadsheetCommandGalleryItem2.IsEmptyHint = true;
		spreadsheetCommandGalleryItem3.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem3.CaptionAsValue = true;
		spreadsheetCommandGalleryItem3.CommandName = "FormatNumberAccountingCurrency";
		spreadsheetCommandGalleryItem3.IsEmptyHint = true;
		spreadsheetCommandGalleryItem4.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem4.CaptionAsValue = true;
		spreadsheetCommandGalleryItem4.CommandName = "FormatNumberAccountingRegular";
		spreadsheetCommandGalleryItem4.IsEmptyHint = true;
		spreadsheetCommandGalleryItem5.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem5.CaptionAsValue = true;
		spreadsheetCommandGalleryItem5.CommandName = "FormatNumberShortDate";
		spreadsheetCommandGalleryItem5.IsEmptyHint = true;
		spreadsheetCommandGalleryItem6.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem6.CaptionAsValue = true;
		spreadsheetCommandGalleryItem6.CommandName = "FormatNumberLongDate";
		spreadsheetCommandGalleryItem6.IsEmptyHint = true;
		spreadsheetCommandGalleryItem7.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem7.CaptionAsValue = true;
		spreadsheetCommandGalleryItem7.CommandName = "FormatNumberTime";
		spreadsheetCommandGalleryItem7.IsEmptyHint = true;
		spreadsheetCommandGalleryItem8.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem8.CaptionAsValue = true;
		spreadsheetCommandGalleryItem8.CommandName = "FormatNumberPercentage";
		spreadsheetCommandGalleryItem8.IsEmptyHint = true;
		spreadsheetCommandGalleryItem9.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem9.CaptionAsValue = true;
		spreadsheetCommandGalleryItem9.CommandName = "FormatNumberFraction";
		spreadsheetCommandGalleryItem9.IsEmptyHint = true;
		spreadsheetCommandGalleryItem10.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem10.CaptionAsValue = true;
		spreadsheetCommandGalleryItem10.CommandName = "FormatNumberScientific";
		spreadsheetCommandGalleryItem10.IsEmptyHint = true;
		spreadsheetCommandGalleryItem11.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem11.CaptionAsValue = true;
		spreadsheetCommandGalleryItem11.CommandName = "FormatNumberText";
		spreadsheetCommandGalleryItem11.IsEmptyHint = true;
		galleryItemGroup.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[11]
		{
			spreadsheetCommandGalleryItem, spreadsheetCommandGalleryItem2, spreadsheetCommandGalleryItem3, spreadsheetCommandGalleryItem4, spreadsheetCommandGalleryItem5, spreadsheetCommandGalleryItem6, spreadsheetCommandGalleryItem7, spreadsheetCommandGalleryItem8, spreadsheetCommandGalleryItem9, spreadsheetCommandGalleryItem10,
			spreadsheetCommandGalleryItem11
		});
		this.repositoryItemPopupGalleryEdit1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup });
		this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.repositoryItemPopupGalleryEdit1.Gallery.RowCount = 11;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowGroupCaption = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowItemText = true;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
		this.repositoryItemPopupGalleryEdit1.Gallery.StretchItems = true;
		this.repositoryItemPopupGalleryEdit1.Name = "repositoryItemPopupGalleryEdit1";
		this.repositoryItemPopupGalleryEdit1.ShowButtons = false;
		this.repositoryItemPopupGalleryEdit1.ShowPopupCloseButton = false;
		this.repositoryItemPopupGalleryEdit1.ShowSizeGrip = false;
		this.spreadsheetCommandBarSubItem3.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarSubItem3.CommandName = "FormatNumberAccountingCommandGroup";
		this.spreadsheetCommandBarSubItem3.Id = 45;
		this.spreadsheetCommandBarSubItem3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem25),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem26),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem27),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem28),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem29),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem30)
		});
		this.spreadsheetCommandBarSubItem3.Name = "spreadsheetCommandBarSubItem3";
		this.spreadsheetCommandBarSubItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem25.CommandName = "FormatNumberAccountingDefault";
		this.spreadsheetCommandBarButtonItem25.Id = 46;
		this.spreadsheetCommandBarButtonItem25.Name = "spreadsheetCommandBarButtonItem25";
		this.spreadsheetCommandBarButtonItem26.CommandName = "FormatNumberAccountingUS";
		this.spreadsheetCommandBarButtonItem26.Id = 47;
		this.spreadsheetCommandBarButtonItem26.Name = "spreadsheetCommandBarButtonItem26";
		this.spreadsheetCommandBarButtonItem27.CommandName = "FormatNumberAccountingUK";
		this.spreadsheetCommandBarButtonItem27.Id = 48;
		this.spreadsheetCommandBarButtonItem27.Name = "spreadsheetCommandBarButtonItem27";
		this.spreadsheetCommandBarButtonItem28.CommandName = "FormatNumberAccountingEuro";
		this.spreadsheetCommandBarButtonItem28.Id = 49;
		this.spreadsheetCommandBarButtonItem28.Name = "spreadsheetCommandBarButtonItem28";
		this.spreadsheetCommandBarButtonItem29.CommandName = "FormatNumberAccountingPRC";
		this.spreadsheetCommandBarButtonItem29.Id = 50;
		this.spreadsheetCommandBarButtonItem29.Name = "spreadsheetCommandBarButtonItem29";
		this.spreadsheetCommandBarButtonItem30.CommandName = "FormatNumberAccountingSwiss";
		this.spreadsheetCommandBarButtonItem30.Id = 51;
		this.spreadsheetCommandBarButtonItem30.Name = "spreadsheetCommandBarButtonItem30";
		this.spreadsheetCommandBarButtonItem31.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarButtonItem31.CommandName = "FormatNumberPercent";
		this.spreadsheetCommandBarButtonItem31.Id = 52;
		this.spreadsheetCommandBarButtonItem31.Name = "spreadsheetCommandBarButtonItem31";
		this.spreadsheetCommandBarButtonItem31.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem32.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarButtonItem32.CommandName = "FormatNumberAccounting";
		this.spreadsheetCommandBarButtonItem32.Id = 53;
		this.spreadsheetCommandBarButtonItem32.Name = "spreadsheetCommandBarButtonItem32";
		this.spreadsheetCommandBarButtonItem32.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem33.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
		this.spreadsheetCommandBarButtonItem33.CommandName = "FormatNumberIncreaseDecimal";
		this.spreadsheetCommandBarButtonItem33.Id = 54;
		this.spreadsheetCommandBarButtonItem33.Name = "spreadsheetCommandBarButtonItem33";
		this.spreadsheetCommandBarButtonItem33.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem34.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
		this.spreadsheetCommandBarButtonItem34.CommandName = "FormatNumberDecreaseDecimal";
		this.spreadsheetCommandBarButtonItem34.Id = 55;
		this.spreadsheetCommandBarButtonItem34.Name = "spreadsheetCommandBarButtonItem34";
		this.spreadsheetCommandBarButtonItem34.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarSubItem7.CommandName = "ConditionalFormattingCommandGroup";
		this.spreadsheetCommandBarSubItem7.Id = 56;
		this.spreadsheetCommandBarSubItem7.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem48),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem51)
		});
		this.spreadsheetCommandBarSubItem7.Name = "spreadsheetCommandBarSubItem7";
		this.spreadsheetCommandBarSubItem4.CommandName = "ConditionalFormattingHighlightCellsRuleCommandGroup";
		this.spreadsheetCommandBarSubItem4.Id = 57;
		this.spreadsheetCommandBarSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem35),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem36),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem37),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem38),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem39),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem40),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem41)
		});
		this.spreadsheetCommandBarSubItem4.Name = "spreadsheetCommandBarSubItem4";
		this.spreadsheetCommandBarButtonItem35.CommandName = "ConditionalFormattingGreaterThanRuleCommand";
		this.spreadsheetCommandBarButtonItem35.Id = 58;
		this.spreadsheetCommandBarButtonItem35.Name = "spreadsheetCommandBarButtonItem35";
		this.spreadsheetCommandBarButtonItem35.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem36.CommandName = "ConditionalFormattingLessThanRuleCommand";
		this.spreadsheetCommandBarButtonItem36.Id = 59;
		this.spreadsheetCommandBarButtonItem36.Name = "spreadsheetCommandBarButtonItem36";
		this.spreadsheetCommandBarButtonItem36.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem37.CommandName = "ConditionalFormattingBetweenRuleCommand";
		this.spreadsheetCommandBarButtonItem37.Id = 60;
		this.spreadsheetCommandBarButtonItem37.Name = "spreadsheetCommandBarButtonItem37";
		this.spreadsheetCommandBarButtonItem37.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem38.CommandName = "ConditionalFormattingEqualToRuleCommand";
		this.spreadsheetCommandBarButtonItem38.Id = 61;
		this.spreadsheetCommandBarButtonItem38.Name = "spreadsheetCommandBarButtonItem38";
		this.spreadsheetCommandBarButtonItem38.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem39.CommandName = "ConditionalFormattingTextContainsRuleCommand";
		this.spreadsheetCommandBarButtonItem39.Id = 62;
		this.spreadsheetCommandBarButtonItem39.Name = "spreadsheetCommandBarButtonItem39";
		this.spreadsheetCommandBarButtonItem39.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem40.CommandName = "ConditionalFormattingDateOccurringRuleCommand";
		this.spreadsheetCommandBarButtonItem40.Id = 63;
		this.spreadsheetCommandBarButtonItem40.Name = "spreadsheetCommandBarButtonItem40";
		this.spreadsheetCommandBarButtonItem40.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem41.CommandName = "ConditionalFormattingDuplicateValuesRuleCommand";
		this.spreadsheetCommandBarButtonItem41.Id = 64;
		this.spreadsheetCommandBarButtonItem41.Name = "spreadsheetCommandBarButtonItem41";
		this.spreadsheetCommandBarButtonItem41.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem5.CommandName = "ConditionalFormattingTopBottomRuleCommandGroup";
		this.spreadsheetCommandBarSubItem5.Id = 65;
		this.spreadsheetCommandBarSubItem5.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem42),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem43),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem44),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem45),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem46),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem47)
		});
		this.spreadsheetCommandBarSubItem5.Name = "spreadsheetCommandBarSubItem5";
		this.spreadsheetCommandBarButtonItem42.CommandName = "ConditionalFormattingTop10RuleCommand";
		this.spreadsheetCommandBarButtonItem42.Id = 66;
		this.spreadsheetCommandBarButtonItem42.Name = "spreadsheetCommandBarButtonItem42";
		this.spreadsheetCommandBarButtonItem42.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem43.CommandName = "ConditionalFormattingTop10PercentRuleCommand";
		this.spreadsheetCommandBarButtonItem43.Id = 67;
		this.spreadsheetCommandBarButtonItem43.Name = "spreadsheetCommandBarButtonItem43";
		this.spreadsheetCommandBarButtonItem43.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem44.CommandName = "ConditionalFormattingBottom10RuleCommand";
		this.spreadsheetCommandBarButtonItem44.Id = 68;
		this.spreadsheetCommandBarButtonItem44.Name = "spreadsheetCommandBarButtonItem44";
		this.spreadsheetCommandBarButtonItem44.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem45.CommandName = "ConditionalFormattingBottom10PercentRuleCommand";
		this.spreadsheetCommandBarButtonItem45.Id = 69;
		this.spreadsheetCommandBarButtonItem45.Name = "spreadsheetCommandBarButtonItem45";
		this.spreadsheetCommandBarButtonItem45.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem46.CommandName = "ConditionalFormattingAboveAverageRuleCommand";
		this.spreadsheetCommandBarButtonItem46.Id = 70;
		this.spreadsheetCommandBarButtonItem46.Name = "spreadsheetCommandBarButtonItem46";
		this.spreadsheetCommandBarButtonItem46.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem47.CommandName = "ConditionalFormattingBelowAverageRuleCommand";
		this.spreadsheetCommandBarButtonItem47.Id = 71;
		this.spreadsheetCommandBarButtonItem47.Name = "spreadsheetCommandBarButtonItem47";
		this.spreadsheetCommandBarButtonItem47.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.CommandName = "ConditionalFormattingDataBarsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.DropDownControl = this.commandBarGalleryDropDown24;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.Id = 72;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.Name = "spreadsheetCommandBarButtonGalleryDropDownItem1";
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown24.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
		spreadsheetCommandGalleryItem12.CommandName = "ConditionalFormattingDataBarGradientBlue";
		spreadsheetCommandGalleryItem13.CommandName = "ConditionalFormattingDataBarGradientGreen";
		spreadsheetCommandGalleryItem14.CommandName = "ConditionalFormattingDataBarGradientRed";
		spreadsheetCommandGalleryItem15.CommandName = "ConditionalFormattingDataBarGradientOrange";
		spreadsheetCommandGalleryItem16.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
		spreadsheetCommandGalleryItem17.CommandName = "ConditionalFormattingDataBarGradientPurple";
		spreadsheetCommandGalleryItemGroup.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem12, spreadsheetCommandGalleryItem13, spreadsheetCommandGalleryItem14, spreadsheetCommandGalleryItem15, spreadsheetCommandGalleryItem16, spreadsheetCommandGalleryItem17 });
		spreadsheetCommandGalleryItemGroup2.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
		spreadsheetCommandGalleryItem18.CommandName = "ConditionalFormattingDataBarSolidBlue";
		spreadsheetCommandGalleryItem19.CommandName = "ConditionalFormattingDataBarSolidGreen";
		spreadsheetCommandGalleryItem20.CommandName = "ConditionalFormattingDataBarSolidRed";
		spreadsheetCommandGalleryItem21.CommandName = "ConditionalFormattingDataBarSolidOrange";
		spreadsheetCommandGalleryItem22.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
		spreadsheetCommandGalleryItem23.CommandName = "ConditionalFormattingDataBarSolidPurple";
		spreadsheetCommandGalleryItemGroup2.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem18, spreadsheetCommandGalleryItem19, spreadsheetCommandGalleryItem20, spreadsheetCommandGalleryItem21, spreadsheetCommandGalleryItem22, spreadsheetCommandGalleryItem23 });
		this.commandBarGalleryDropDown24.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup, spreadsheetCommandGalleryItemGroup2 });
		this.commandBarGalleryDropDown24.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown24.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown24.Manager = this.barManager1;
		this.commandBarGalleryDropDown24.Name = "commandBarGalleryDropDown24";
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.DropDownControl = this.commandBarGalleryDropDown25;
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.Id = 73;
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.Name = "spreadsheetCommandBarButtonGalleryDropDownItem2";
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown25.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup3.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		spreadsheetCommandGalleryItem24.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
		spreadsheetCommandGalleryItem25.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
		spreadsheetCommandGalleryItem26.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
		spreadsheetCommandGalleryItem27.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
		spreadsheetCommandGalleryItem28.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
		spreadsheetCommandGalleryItem29.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
		spreadsheetCommandGalleryItem30.CommandName = "ConditionalFormattingColorScaleWhiteRed";
		spreadsheetCommandGalleryItem31.CommandName = "ConditionalFormattingColorScaleRedWhite";
		spreadsheetCommandGalleryItem32.CommandName = "ConditionalFormattingColorScaleGreenWhite";
		spreadsheetCommandGalleryItem33.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
		spreadsheetCommandGalleryItem34.CommandName = "ConditionalFormattingColorScaleGreenYellow";
		spreadsheetCommandGalleryItem35.CommandName = "ConditionalFormattingColorScaleYellowGreen";
		spreadsheetCommandGalleryItemGroup3.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[12]
		{
			spreadsheetCommandGalleryItem24, spreadsheetCommandGalleryItem25, spreadsheetCommandGalleryItem26, spreadsheetCommandGalleryItem27, spreadsheetCommandGalleryItem28, spreadsheetCommandGalleryItem29, spreadsheetCommandGalleryItem30, spreadsheetCommandGalleryItem31, spreadsheetCommandGalleryItem32, spreadsheetCommandGalleryItem33,
			spreadsheetCommandGalleryItem34, spreadsheetCommandGalleryItem35
		});
		this.commandBarGalleryDropDown25.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup3 });
		this.commandBarGalleryDropDown25.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown25.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown25.Manager = this.barManager1;
		this.commandBarGalleryDropDown25.Name = "commandBarGalleryDropDown25";
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.CommandName = "ConditionalFormattingIconSetsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.DropDownControl = this.commandBarGalleryDropDown26;
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.Id = 74;
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.Name = "spreadsheetCommandBarButtonGalleryDropDownItem3";
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown26.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup4.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
		spreadsheetCommandGalleryItem36.CommandName = "ConditionalFormattingIconSetArrows3Colored";
		spreadsheetCommandGalleryItem37.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
		spreadsheetCommandGalleryItem38.CommandName = "ConditionalFormattingIconSetArrows4Colored";
		spreadsheetCommandGalleryItem39.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
		spreadsheetCommandGalleryItem40.CommandName = "ConditionalFormattingIconSetArrows5Colored";
		spreadsheetCommandGalleryItem41.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
		spreadsheetCommandGalleryItem42.CommandName = "ConditionalFormattingIconSetTriangles3";
		spreadsheetCommandGalleryItemGroup4.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem36, spreadsheetCommandGalleryItem37, spreadsheetCommandGalleryItem38, spreadsheetCommandGalleryItem39, spreadsheetCommandGalleryItem40, spreadsheetCommandGalleryItem41, spreadsheetCommandGalleryItem42 });
		spreadsheetCommandGalleryItemGroup5.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
		spreadsheetCommandGalleryItem43.CommandName = "ConditionalFormattingIconSetTrafficLights3";
		spreadsheetCommandGalleryItem44.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
		spreadsheetCommandGalleryItem45.CommandName = "ConditionalFormattingIconSetTrafficLights4";
		spreadsheetCommandGalleryItem46.CommandName = "ConditionalFormattingIconSetSigns3";
		spreadsheetCommandGalleryItem47.CommandName = "ConditionalFormattingIconSetRedToBlack";
		spreadsheetCommandGalleryItemGroup5.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem43, spreadsheetCommandGalleryItem44, spreadsheetCommandGalleryItem45, spreadsheetCommandGalleryItem46, spreadsheetCommandGalleryItem47 });
		spreadsheetCommandGalleryItemGroup6.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
		spreadsheetCommandGalleryItem48.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
		spreadsheetCommandGalleryItem49.CommandName = "ConditionalFormattingIconSetSymbols3";
		spreadsheetCommandGalleryItem50.CommandName = "ConditionalFormattingIconSetFlags3";
		spreadsheetCommandGalleryItemGroup6.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem48, spreadsheetCommandGalleryItem49, spreadsheetCommandGalleryItem50 });
		spreadsheetCommandGalleryItemGroup7.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
		spreadsheetCommandGalleryItem51.CommandName = "ConditionalFormattingIconSetStars3";
		spreadsheetCommandGalleryItem52.CommandName = "ConditionalFormattingIconSetRatings4";
		spreadsheetCommandGalleryItem53.CommandName = "ConditionalFormattingIconSetRatings5";
		spreadsheetCommandGalleryItem54.CommandName = "ConditionalFormattingIconSetQuarters5";
		spreadsheetCommandGalleryItem55.CommandName = "ConditionalFormattingIconSetBoxes5";
		spreadsheetCommandGalleryItemGroup7.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem51, spreadsheetCommandGalleryItem52, spreadsheetCommandGalleryItem53, spreadsheetCommandGalleryItem54, spreadsheetCommandGalleryItem55 });
		this.commandBarGalleryDropDown26.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[4] { spreadsheetCommandGalleryItemGroup4, spreadsheetCommandGalleryItemGroup5, spreadsheetCommandGalleryItemGroup6, spreadsheetCommandGalleryItemGroup7 });
		this.commandBarGalleryDropDown26.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown26.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown26.Manager = this.barManager1;
		this.commandBarGalleryDropDown26.Name = "commandBarGalleryDropDown26";
		this.spreadsheetCommandBarButtonItem48.CommandName = "NewConditionalFormattingRule";
		this.spreadsheetCommandBarButtonItem48.Id = 75;
		this.spreadsheetCommandBarButtonItem48.Name = "spreadsheetCommandBarButtonItem48";
		this.spreadsheetCommandBarSubItem6.CommandName = "ConditionalFormattingRemoveCommandGroup";
		this.spreadsheetCommandBarSubItem6.Id = 76;
		this.spreadsheetCommandBarSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem49),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem50)
		});
		this.spreadsheetCommandBarSubItem6.Name = "spreadsheetCommandBarSubItem6";
		this.spreadsheetCommandBarButtonItem49.CommandName = "ConditionalFormattingRemoveFromSheet";
		this.spreadsheetCommandBarButtonItem49.Id = 77;
		this.spreadsheetCommandBarButtonItem49.Name = "spreadsheetCommandBarButtonItem49";
		this.spreadsheetCommandBarButtonItem50.CommandName = "ConditionalFormattingRemove";
		this.spreadsheetCommandBarButtonItem50.Id = 78;
		this.spreadsheetCommandBarButtonItem50.Name = "spreadsheetCommandBarButtonItem50";
		this.spreadsheetCommandBarButtonItem51.CommandName = "ConditionalFormattingRulesManager";
		this.spreadsheetCommandBarButtonItem51.Id = 79;
		this.spreadsheetCommandBarButtonItem51.Name = "spreadsheetCommandBarButtonItem51";
		this.galleryFormatAsTableItem1.DropDownControl = this.commandBarGalleryDropDown27;
		this.galleryFormatAsTableItem1.Id = 80;
		this.galleryFormatAsTableItem1.Name = "galleryFormatAsTableItem1";
		this.commandBarGalleryDropDown27.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown27.Gallery.ColumnCount = 7;
		this.commandBarGalleryDropDown27.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown27.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown27.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.commandBarGalleryDropDown27.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown27.Manager = this.barManager1;
		this.commandBarGalleryDropDown27.Name = "commandBarGalleryDropDown27";
		this.changeStyleItem1.Edit = this.repositoryItemSpreadsheetCellStyleEdit1;
		this.changeStyleItem1.EditWidth = 130;
		this.changeStyleItem1.Id = 81;
		this.changeStyleItem1.Name = "changeStyleItem1";
		this.repositoryItemSpreadsheetCellStyleEdit1.AutoHeight = false;
		this.repositoryItemSpreadsheetCellStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSpreadsheetCellStyleEdit1.Control = this.sc_Main;
		this.repositoryItemSpreadsheetCellStyleEdit1.Name = "repositoryItemSpreadsheetCellStyleEdit1";
		this.spreadsheetCommandBarSubItem8.CommandName = "InsertCellsCommandGroup";
		this.spreadsheetCommandBarSubItem8.Id = 82;
		this.spreadsheetCommandBarSubItem8.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem52),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem53),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem54),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem55),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem56),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem57),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem58),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem59)
		});
		this.spreadsheetCommandBarSubItem8.Name = "spreadsheetCommandBarSubItem8";
		this.spreadsheetCommandBarButtonItem52.CommandName = "InsertCells";
		this.spreadsheetCommandBarButtonItem52.Id = 83;
		this.spreadsheetCommandBarButtonItem52.Name = "spreadsheetCommandBarButtonItem52";
		this.spreadsheetCommandBarButtonItem53.CommandName = "InsertSheetRows";
		this.spreadsheetCommandBarButtonItem53.Id = 84;
		this.spreadsheetCommandBarButtonItem53.Name = "spreadsheetCommandBarButtonItem53";
		this.spreadsheetCommandBarButtonItem54.CommandName = "InsertSheetColumns";
		this.spreadsheetCommandBarButtonItem54.Id = 85;
		this.spreadsheetCommandBarButtonItem54.Name = "spreadsheetCommandBarButtonItem54";
		this.spreadsheetCommandBarButtonItem55.CommandName = "InsertTableRowsAbove";
		this.spreadsheetCommandBarButtonItem55.Id = 86;
		this.spreadsheetCommandBarButtonItem55.Name = "spreadsheetCommandBarButtonItem55";
		this.spreadsheetCommandBarButtonItem56.CommandName = "InsertTableRowBelow";
		this.spreadsheetCommandBarButtonItem56.Id = 87;
		this.spreadsheetCommandBarButtonItem56.Name = "spreadsheetCommandBarButtonItem56";
		this.spreadsheetCommandBarButtonItem57.CommandName = "InsertTableColumnsToTheLeft";
		this.spreadsheetCommandBarButtonItem57.Id = 88;
		this.spreadsheetCommandBarButtonItem57.Name = "spreadsheetCommandBarButtonItem57";
		this.spreadsheetCommandBarButtonItem58.CommandName = "InsertTableColumnToTheRight";
		this.spreadsheetCommandBarButtonItem58.Id = 89;
		this.spreadsheetCommandBarButtonItem58.Name = "spreadsheetCommandBarButtonItem58";
		this.spreadsheetCommandBarButtonItem59.CommandName = "InsertSheet";
		this.spreadsheetCommandBarButtonItem59.Id = 90;
		this.spreadsheetCommandBarButtonItem59.Name = "spreadsheetCommandBarButtonItem59";
		this.spreadsheetCommandBarSubItem9.CommandName = "RemoveCellsCommandGroup";
		this.spreadsheetCommandBarSubItem9.Id = 91;
		this.spreadsheetCommandBarSubItem9.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem60),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem61),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem62),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem63),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem64),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem65)
		});
		this.spreadsheetCommandBarSubItem9.Name = "spreadsheetCommandBarSubItem9";
		this.spreadsheetCommandBarButtonItem60.CommandName = "RemoveCells";
		this.spreadsheetCommandBarButtonItem60.Id = 92;
		this.spreadsheetCommandBarButtonItem60.Name = "spreadsheetCommandBarButtonItem60";
		this.spreadsheetCommandBarButtonItem61.CommandName = "RemoveSheetRows";
		this.spreadsheetCommandBarButtonItem61.Id = 93;
		this.spreadsheetCommandBarButtonItem61.Name = "spreadsheetCommandBarButtonItem61";
		this.spreadsheetCommandBarButtonItem62.CommandName = "RemoveSheetColumns";
		this.spreadsheetCommandBarButtonItem62.Id = 94;
		this.spreadsheetCommandBarButtonItem62.Name = "spreadsheetCommandBarButtonItem62";
		this.spreadsheetCommandBarButtonItem63.CommandName = "RemoveTableRows";
		this.spreadsheetCommandBarButtonItem63.Id = 95;
		this.spreadsheetCommandBarButtonItem63.Name = "spreadsheetCommandBarButtonItem63";
		this.spreadsheetCommandBarButtonItem64.CommandName = "RemoveTableColumns";
		this.spreadsheetCommandBarButtonItem64.Id = 96;
		this.spreadsheetCommandBarButtonItem64.Name = "spreadsheetCommandBarButtonItem64";
		this.spreadsheetCommandBarButtonItem65.CommandName = "RemoveSheet";
		this.spreadsheetCommandBarButtonItem65.Id = 97;
		this.spreadsheetCommandBarButtonItem65.Name = "spreadsheetCommandBarButtonItem65";
		this.spreadsheetCommandBarSubItem11.CommandName = "FormatCommandGroup";
		this.spreadsheetCommandBarSubItem11.Id = 98;
		this.spreadsheetCommandBarSubItem11.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[12]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem66),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem67),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem68),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem69),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem70),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem10),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem77),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem78),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeSheetTabColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem79),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem80)
		});
		this.spreadsheetCommandBarSubItem11.Name = "spreadsheetCommandBarSubItem11";
		this.spreadsheetCommandBarButtonItem66.CommandName = "FormatRowHeight";
		this.spreadsheetCommandBarButtonItem66.Id = 99;
		this.spreadsheetCommandBarButtonItem66.Name = "spreadsheetCommandBarButtonItem66";
		this.spreadsheetCommandBarButtonItem67.CommandName = "FormatAutoFitRowHeight";
		this.spreadsheetCommandBarButtonItem67.Id = 100;
		this.spreadsheetCommandBarButtonItem67.Name = "spreadsheetCommandBarButtonItem67";
		this.spreadsheetCommandBarButtonItem68.CommandName = "FormatColumnWidth";
		this.spreadsheetCommandBarButtonItem68.Id = 101;
		this.spreadsheetCommandBarButtonItem68.Name = "spreadsheetCommandBarButtonItem68";
		this.spreadsheetCommandBarButtonItem69.CommandName = "FormatAutoFitColumnWidth";
		this.spreadsheetCommandBarButtonItem69.Id = 102;
		this.spreadsheetCommandBarButtonItem69.Name = "spreadsheetCommandBarButtonItem69";
		this.spreadsheetCommandBarButtonItem70.CommandName = "FormatDefaultColumnWidth";
		this.spreadsheetCommandBarButtonItem70.Id = 103;
		this.spreadsheetCommandBarButtonItem70.Name = "spreadsheetCommandBarButtonItem70";
		this.spreadsheetCommandBarSubItem10.CommandName = "HideAndUnhideCommandGroup";
		this.spreadsheetCommandBarSubItem10.Id = 104;
		this.spreadsheetCommandBarSubItem10.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem71),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem72),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem73),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem74),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem75),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem76)
		});
		this.spreadsheetCommandBarSubItem10.Name = "spreadsheetCommandBarSubItem10";
		this.spreadsheetCommandBarButtonItem71.CommandName = "HideRows";
		this.spreadsheetCommandBarButtonItem71.Id = 105;
		this.spreadsheetCommandBarButtonItem71.Name = "spreadsheetCommandBarButtonItem71";
		this.spreadsheetCommandBarButtonItem72.CommandName = "HideColumns";
		this.spreadsheetCommandBarButtonItem72.Id = 106;
		this.spreadsheetCommandBarButtonItem72.Name = "spreadsheetCommandBarButtonItem72";
		this.spreadsheetCommandBarButtonItem73.CommandName = "HideSheet";
		this.spreadsheetCommandBarButtonItem73.Id = 107;
		this.spreadsheetCommandBarButtonItem73.Name = "spreadsheetCommandBarButtonItem73";
		this.spreadsheetCommandBarButtonItem74.CommandName = "UnhideRows";
		this.spreadsheetCommandBarButtonItem74.Id = 108;
		this.spreadsheetCommandBarButtonItem74.Name = "spreadsheetCommandBarButtonItem74";
		this.spreadsheetCommandBarButtonItem75.CommandName = "UnhideColumns";
		this.spreadsheetCommandBarButtonItem75.Id = 109;
		this.spreadsheetCommandBarButtonItem75.Name = "spreadsheetCommandBarButtonItem75";
		this.spreadsheetCommandBarButtonItem76.CommandName = "UnhideSheet";
		this.spreadsheetCommandBarButtonItem76.Id = 110;
		this.spreadsheetCommandBarButtonItem76.Name = "spreadsheetCommandBarButtonItem76";
		this.spreadsheetCommandBarButtonItem77.CommandName = "RenameSheet";
		this.spreadsheetCommandBarButtonItem77.Id = 111;
		this.spreadsheetCommandBarButtonItem77.Name = "spreadsheetCommandBarButtonItem77";
		this.spreadsheetCommandBarButtonItem78.CommandName = "MoveOrCopySheet";
		this.spreadsheetCommandBarButtonItem78.Id = 112;
		this.spreadsheetCommandBarButtonItem78.Name = "spreadsheetCommandBarButtonItem78";
		this.changeSheetTabColorItem1.ActAsDropDown = true;
		this.changeSheetTabColorItem1.Id = 113;
		this.changeSheetTabColorItem1.Name = "changeSheetTabColorItem1";
		this.spreadsheetCommandBarButtonItem79.CommandName = "ReviewProtectSheet";
		this.spreadsheetCommandBarButtonItem79.Id = 114;
		this.spreadsheetCommandBarButtonItem79.Name = "spreadsheetCommandBarButtonItem79";
		this.spreadsheetCommandBarCheckItem13.CommandName = "FormatCellLocked";
		this.spreadsheetCommandBarCheckItem13.Id = 115;
		this.spreadsheetCommandBarCheckItem13.Name = "spreadsheetCommandBarCheckItem13";
		this.spreadsheetCommandBarButtonItem80.CommandName = "FormatCellsContextMenuItem";
		this.spreadsheetCommandBarButtonItem80.Id = 116;
		this.spreadsheetCommandBarButtonItem80.Name = "spreadsheetCommandBarButtonItem80";
		this.spreadsheetCommandBarSubItem12.CommandName = "EditingAutoSumCommandGroup";
		this.spreadsheetCommandBarSubItem12.Id = 117;
		this.spreadsheetCommandBarSubItem12.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem81),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem82),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem83),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem84),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem85)
		});
		this.spreadsheetCommandBarSubItem12.Name = "spreadsheetCommandBarSubItem12";
		this.spreadsheetCommandBarSubItem12.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem81.CommandName = "FunctionsInsertSum";
		this.spreadsheetCommandBarButtonItem81.Id = 118;
		this.spreadsheetCommandBarButtonItem81.Name = "spreadsheetCommandBarButtonItem81";
		this.spreadsheetCommandBarButtonItem82.CommandName = "FunctionsInsertAverage";
		this.spreadsheetCommandBarButtonItem82.Id = 119;
		this.spreadsheetCommandBarButtonItem82.Name = "spreadsheetCommandBarButtonItem82";
		this.spreadsheetCommandBarButtonItem83.CommandName = "FunctionsInsertCountNumbers";
		this.spreadsheetCommandBarButtonItem83.Id = 120;
		this.spreadsheetCommandBarButtonItem83.Name = "spreadsheetCommandBarButtonItem83";
		this.spreadsheetCommandBarButtonItem84.CommandName = "FunctionsInsertMax";
		this.spreadsheetCommandBarButtonItem84.Id = 121;
		this.spreadsheetCommandBarButtonItem84.Name = "spreadsheetCommandBarButtonItem84";
		this.spreadsheetCommandBarButtonItem85.CommandName = "FunctionsInsertMin";
		this.spreadsheetCommandBarButtonItem85.Id = 122;
		this.spreadsheetCommandBarButtonItem85.Name = "spreadsheetCommandBarButtonItem85";
		this.spreadsheetCommandBarSubItem13.CommandName = "EditingFillCommandGroup";
		this.spreadsheetCommandBarSubItem13.Id = 123;
		this.spreadsheetCommandBarSubItem13.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem86),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem87),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem88),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem89)
		});
		this.spreadsheetCommandBarSubItem13.Name = "spreadsheetCommandBarSubItem13";
		this.spreadsheetCommandBarSubItem13.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem86.CommandName = "EditingFillDown";
		this.spreadsheetCommandBarButtonItem86.Id = 124;
		this.spreadsheetCommandBarButtonItem86.Name = "spreadsheetCommandBarButtonItem86";
		this.spreadsheetCommandBarButtonItem87.CommandName = "EditingFillRight";
		this.spreadsheetCommandBarButtonItem87.Id = 125;
		this.spreadsheetCommandBarButtonItem87.Name = "spreadsheetCommandBarButtonItem87";
		this.spreadsheetCommandBarButtonItem88.CommandName = "EditingFillUp";
		this.spreadsheetCommandBarButtonItem88.Id = 126;
		this.spreadsheetCommandBarButtonItem88.Name = "spreadsheetCommandBarButtonItem88";
		this.spreadsheetCommandBarButtonItem89.CommandName = "EditingFillLeft";
		this.spreadsheetCommandBarButtonItem89.Id = 127;
		this.spreadsheetCommandBarButtonItem89.Name = "spreadsheetCommandBarButtonItem89";
		this.spreadsheetCommandBarSubItem14.CommandName = "FormatClearCommandGroup";
		this.spreadsheetCommandBarSubItem14.Id = 128;
		this.spreadsheetCommandBarSubItem14.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem90),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem91),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem92),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem93),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem94),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem95)
		});
		this.spreadsheetCommandBarSubItem14.Name = "spreadsheetCommandBarSubItem14";
		this.spreadsheetCommandBarSubItem14.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem90.CommandName = "FormatClearAll";
		this.spreadsheetCommandBarButtonItem90.Id = 129;
		this.spreadsheetCommandBarButtonItem90.Name = "spreadsheetCommandBarButtonItem90";
		this.spreadsheetCommandBarButtonItem91.CommandName = "FormatClearFormats";
		this.spreadsheetCommandBarButtonItem91.Id = 130;
		this.spreadsheetCommandBarButtonItem91.Name = "spreadsheetCommandBarButtonItem91";
		this.spreadsheetCommandBarButtonItem92.CommandName = "FormatClearContents";
		this.spreadsheetCommandBarButtonItem92.Id = 131;
		this.spreadsheetCommandBarButtonItem92.Name = "spreadsheetCommandBarButtonItem92";
		this.spreadsheetCommandBarButtonItem93.CommandName = "FormatClearComments";
		this.spreadsheetCommandBarButtonItem93.Id = 132;
		this.spreadsheetCommandBarButtonItem93.Name = "spreadsheetCommandBarButtonItem93";
		this.spreadsheetCommandBarButtonItem94.CommandName = "FormatClearHyperlinks";
		this.spreadsheetCommandBarButtonItem94.Id = 133;
		this.spreadsheetCommandBarButtonItem94.Name = "spreadsheetCommandBarButtonItem94";
		this.spreadsheetCommandBarButtonItem95.CommandName = "FormatRemoveHyperlinks";
		this.spreadsheetCommandBarButtonItem95.Id = 134;
		this.spreadsheetCommandBarButtonItem95.Name = "spreadsheetCommandBarButtonItem95";
		this.spreadsheetCommandBarSubItem15.CommandName = "EditingSortAndFilterCommandGroup";
		this.spreadsheetCommandBarSubItem15.Id = 135;
		this.spreadsheetCommandBarSubItem15.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem96),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem97),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem98),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem99)
		});
		this.spreadsheetCommandBarSubItem15.Name = "spreadsheetCommandBarSubItem15";
		this.spreadsheetCommandBarButtonItem96.CommandName = "DataSortAscending";
		this.spreadsheetCommandBarButtonItem96.Id = 136;
		this.spreadsheetCommandBarButtonItem96.Name = "spreadsheetCommandBarButtonItem96";
		this.spreadsheetCommandBarButtonItem97.CommandName = "DataSortDescending";
		this.spreadsheetCommandBarButtonItem97.Id = 137;
		this.spreadsheetCommandBarButtonItem97.Name = "spreadsheetCommandBarButtonItem97";
		this.spreadsheetCommandBarCheckItem14.CommandName = "DataFilterToggle";
		this.spreadsheetCommandBarCheckItem14.Id = 138;
		this.spreadsheetCommandBarCheckItem14.Name = "spreadsheetCommandBarCheckItem14";
		this.spreadsheetCommandBarButtonItem98.CommandName = "DataFilterClear";
		this.spreadsheetCommandBarButtonItem98.Id = 139;
		this.spreadsheetCommandBarButtonItem98.Name = "spreadsheetCommandBarButtonItem98";
		this.spreadsheetCommandBarButtonItem99.CommandName = "DataFilterReApply";
		this.spreadsheetCommandBarButtonItem99.Id = 140;
		this.spreadsheetCommandBarButtonItem99.Name = "spreadsheetCommandBarButtonItem99";
		this.spreadsheetCommandBarSubItem16.CommandName = "EditingFindAndSelectCommandGroup";
		this.spreadsheetCommandBarSubItem16.Id = 141;
		this.spreadsheetCommandBarSubItem16.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem100),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem101),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem102),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem103),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem104),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem105),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem106)
		});
		this.spreadsheetCommandBarSubItem16.Name = "spreadsheetCommandBarSubItem16";
		this.spreadsheetCommandBarButtonItem100.CommandName = "EditingFind";
		this.spreadsheetCommandBarButtonItem100.Id = 142;
		this.spreadsheetCommandBarButtonItem100.Name = "spreadsheetCommandBarButtonItem100";
		this.spreadsheetCommandBarButtonItem101.CommandName = "EditingReplace";
		this.spreadsheetCommandBarButtonItem101.Id = 143;
		this.spreadsheetCommandBarButtonItem101.Name = "spreadsheetCommandBarButtonItem101";
		this.spreadsheetCommandBarButtonItem102.CommandName = "EditingSelectFormulas";
		this.spreadsheetCommandBarButtonItem102.Id = 144;
		this.spreadsheetCommandBarButtonItem102.Name = "spreadsheetCommandBarButtonItem102";
		this.spreadsheetCommandBarButtonItem103.CommandName = "EditingSelectComments";
		this.spreadsheetCommandBarButtonItem103.Id = 145;
		this.spreadsheetCommandBarButtonItem103.Name = "spreadsheetCommandBarButtonItem103";
		this.spreadsheetCommandBarButtonItem104.CommandName = "EditingSelectConditionalFormatting";
		this.spreadsheetCommandBarButtonItem104.Id = 146;
		this.spreadsheetCommandBarButtonItem104.Name = "spreadsheetCommandBarButtonItem104";
		this.spreadsheetCommandBarButtonItem105.CommandName = "EditingSelectConstants";
		this.spreadsheetCommandBarButtonItem105.Id = 147;
		this.spreadsheetCommandBarButtonItem105.Name = "spreadsheetCommandBarButtonItem105";
		this.spreadsheetCommandBarButtonItem106.CommandName = "EditingSelectDataValidation";
		this.spreadsheetCommandBarButtonItem106.Id = 148;
		this.spreadsheetCommandBarButtonItem106.Name = "spreadsheetCommandBarButtonItem106";
		this.commandBarGalleryDropDown18.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown18.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown18.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown18.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.commandBarGalleryDropDown18.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown18.Gallery.ItemSize = new System.Drawing.Size(136, 26);
		this.commandBarGalleryDropDown18.Gallery.RowCount = 14;
		this.commandBarGalleryDropDown18.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown18.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown18.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown18.Manager = this.barManager1;
		this.commandBarGalleryDropDown18.Name = "commandBarGalleryDropDown18";
		this.commandBarGalleryDropDown19.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup8.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
		spreadsheetCommandGalleryItem56.Caption = "Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem56.CommandName = "ConditionalFormattingDataBarGradientBlue";
		spreadsheetCommandGalleryItem56.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem56.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem57.Caption = "Green Data Bar (Gradient)";
		spreadsheetCommandGalleryItem57.CommandName = "ConditionalFormattingDataBarGradientGreen";
		spreadsheetCommandGalleryItem57.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem57.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem58.Caption = "Red Data Bar (Gradient)";
		spreadsheetCommandGalleryItem58.CommandName = "ConditionalFormattingDataBarGradientRed";
		spreadsheetCommandGalleryItem58.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem58.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem59.Caption = "Orange Data Bar (Gradient)";
		spreadsheetCommandGalleryItem59.CommandName = "ConditionalFormattingDataBarGradientOrange";
		spreadsheetCommandGalleryItem59.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem59.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem60.Caption = "Light Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem60.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
		spreadsheetCommandGalleryItem60.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem60.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem61.Caption = "Purple Data Bar (Gradient)";
		spreadsheetCommandGalleryItem61.CommandName = "ConditionalFormattingDataBarGradientPurple";
		spreadsheetCommandGalleryItem61.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem61.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup8.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem56, spreadsheetCommandGalleryItem57, spreadsheetCommandGalleryItem58, spreadsheetCommandGalleryItem59, spreadsheetCommandGalleryItem60, spreadsheetCommandGalleryItem61 });
		spreadsheetCommandGalleryItemGroup9.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
		spreadsheetCommandGalleryItem62.Caption = "Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem62.CommandName = "ConditionalFormattingDataBarSolidBlue";
		spreadsheetCommandGalleryItem62.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem62.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem63.Caption = "Green Data Bar (Solid)";
		spreadsheetCommandGalleryItem63.CommandName = "ConditionalFormattingDataBarSolidGreen";
		spreadsheetCommandGalleryItem63.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem63.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem64.Caption = "Red Data Bar (Solid)";
		spreadsheetCommandGalleryItem64.CommandName = "ConditionalFormattingDataBarSolidRed";
		spreadsheetCommandGalleryItem64.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem64.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem65.Caption = "Orange Data Bar (Solid)";
		spreadsheetCommandGalleryItem65.CommandName = "ConditionalFormattingDataBarSolidOrange";
		spreadsheetCommandGalleryItem65.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem65.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem66.Caption = "Light Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem66.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
		spreadsheetCommandGalleryItem66.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem66.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem67.Caption = "Purple Data Bar (Solid)";
		spreadsheetCommandGalleryItem67.CommandName = "ConditionalFormattingDataBarSolidPurple";
		spreadsheetCommandGalleryItem67.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem67.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup9.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem62, spreadsheetCommandGalleryItem63, spreadsheetCommandGalleryItem64, spreadsheetCommandGalleryItem65, spreadsheetCommandGalleryItem66, spreadsheetCommandGalleryItem67 });
		this.commandBarGalleryDropDown19.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup8, spreadsheetCommandGalleryItemGroup9 });
		this.commandBarGalleryDropDown19.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown19.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown19.Manager = this.barManager1;
		this.commandBarGalleryDropDown19.Name = "commandBarGalleryDropDown19";
		this.commandBarGalleryDropDown20.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup10.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		spreadsheetCommandGalleryItem68.Caption = "Green - Yellow - Red Color Scale";
		spreadsheetCommandGalleryItem68.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
		spreadsheetCommandGalleryItem68.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem68.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem69.Caption = "Red - Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem69.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
		spreadsheetCommandGalleryItem69.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem69.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem70.Caption = "Green - White - Red Color Scale";
		spreadsheetCommandGalleryItem70.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
		spreadsheetCommandGalleryItem70.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem70.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem71.Caption = "Red - White - Green Color Scale";
		spreadsheetCommandGalleryItem71.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
		spreadsheetCommandGalleryItem71.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem71.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem72.Caption = "Blue - White - Red Color Scale";
		spreadsheetCommandGalleryItem72.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
		spreadsheetCommandGalleryItem72.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem72.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem73.Caption = "Red - White - Blue Color Scale";
		spreadsheetCommandGalleryItem73.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
		spreadsheetCommandGalleryItem73.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem73.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem74.Caption = "White - Red Color Scale";
		spreadsheetCommandGalleryItem74.CommandName = "ConditionalFormattingColorScaleWhiteRed";
		spreadsheetCommandGalleryItem74.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem74.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem75.Caption = "Red - White Color Scale";
		spreadsheetCommandGalleryItem75.CommandName = "ConditionalFormattingColorScaleRedWhite";
		spreadsheetCommandGalleryItem75.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem75.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem76.Caption = "Green - White Color Scale";
		spreadsheetCommandGalleryItem76.CommandName = "ConditionalFormattingColorScaleGreenWhite";
		spreadsheetCommandGalleryItem76.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem76.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem77.Caption = "White - Green Color Scale";
		spreadsheetCommandGalleryItem77.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
		spreadsheetCommandGalleryItem77.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem77.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem78.Caption = "Green - Yellow Color Scale";
		spreadsheetCommandGalleryItem78.CommandName = "ConditionalFormattingColorScaleGreenYellow";
		spreadsheetCommandGalleryItem78.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem78.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem79.Caption = "Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem79.CommandName = "ConditionalFormattingColorScaleYellowGreen";
		spreadsheetCommandGalleryItem79.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem79.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItemGroup10.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[12]
		{
			spreadsheetCommandGalleryItem68, spreadsheetCommandGalleryItem69, spreadsheetCommandGalleryItem70, spreadsheetCommandGalleryItem71, spreadsheetCommandGalleryItem72, spreadsheetCommandGalleryItem73, spreadsheetCommandGalleryItem74, spreadsheetCommandGalleryItem75, spreadsheetCommandGalleryItem76, spreadsheetCommandGalleryItem77,
			spreadsheetCommandGalleryItem78, spreadsheetCommandGalleryItem79
		});
		this.commandBarGalleryDropDown20.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup10 });
		this.commandBarGalleryDropDown20.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown20.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown20.Manager = this.barManager1;
		this.commandBarGalleryDropDown20.Name = "commandBarGalleryDropDown20";
		this.commandBarGalleryDropDown21.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup11.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
		spreadsheetCommandGalleryItem80.Caption = "3 Arrows (Colored)";
		spreadsheetCommandGalleryItem80.CommandName = "ConditionalFormattingIconSetArrows3Colored";
		spreadsheetCommandGalleryItem80.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem80.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem81.Caption = "3 Arrows (Gray)";
		spreadsheetCommandGalleryItem81.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
		spreadsheetCommandGalleryItem81.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem81.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem82.Caption = "4 Arrows (Colored)";
		spreadsheetCommandGalleryItem82.CommandName = "ConditionalFormattingIconSetArrows4Colored";
		spreadsheetCommandGalleryItem82.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem82.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem83.Caption = "4 Arrows (Gray)";
		spreadsheetCommandGalleryItem83.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
		spreadsheetCommandGalleryItem83.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem83.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem84.Caption = "5 Arrows (Colored)";
		spreadsheetCommandGalleryItem84.CommandName = "ConditionalFormattingIconSetArrows5Colored";
		spreadsheetCommandGalleryItem84.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem84.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem85.Caption = "5 Arrows (Gray)";
		spreadsheetCommandGalleryItem85.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
		spreadsheetCommandGalleryItem85.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem85.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem86.Caption = "3 Triangles";
		spreadsheetCommandGalleryItem86.CommandName = "ConditionalFormattingIconSetTriangles3";
		spreadsheetCommandGalleryItem86.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem86.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup11.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem80, spreadsheetCommandGalleryItem81, spreadsheetCommandGalleryItem82, spreadsheetCommandGalleryItem83, spreadsheetCommandGalleryItem84, spreadsheetCommandGalleryItem85, spreadsheetCommandGalleryItem86 });
		spreadsheetCommandGalleryItemGroup12.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
		spreadsheetCommandGalleryItem87.Caption = "3 Traffic Lights (Unrimmed)";
		spreadsheetCommandGalleryItem87.CommandName = "ConditionalFormattingIconSetTrafficLights3";
		spreadsheetCommandGalleryItem87.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem87.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem88.Caption = "3 Traffic Lights (Rimmed)";
		spreadsheetCommandGalleryItem88.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
		spreadsheetCommandGalleryItem88.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem88.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem89.Caption = "4 Traffic Lights";
		spreadsheetCommandGalleryItem89.CommandName = "ConditionalFormattingIconSetTrafficLights4";
		spreadsheetCommandGalleryItem89.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem89.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem90.Caption = "3 Signs";
		spreadsheetCommandGalleryItem90.CommandName = "ConditionalFormattingIconSetSigns3";
		spreadsheetCommandGalleryItem90.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem90.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem91.Caption = "Red To Black";
		spreadsheetCommandGalleryItem91.CommandName = "ConditionalFormattingIconSetRedToBlack";
		spreadsheetCommandGalleryItem91.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem91.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup12.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem87, spreadsheetCommandGalleryItem88, spreadsheetCommandGalleryItem89, spreadsheetCommandGalleryItem90, spreadsheetCommandGalleryItem91 });
		spreadsheetCommandGalleryItemGroup13.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
		spreadsheetCommandGalleryItem92.Caption = "3 Symbols (Circled)";
		spreadsheetCommandGalleryItem92.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
		spreadsheetCommandGalleryItem92.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem92.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem93.Caption = "3 Symbols (Uncircled)";
		spreadsheetCommandGalleryItem93.CommandName = "ConditionalFormattingIconSetSymbols3";
		spreadsheetCommandGalleryItem93.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem93.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem94.Caption = "3 Flags";
		spreadsheetCommandGalleryItem94.CommandName = "ConditionalFormattingIconSetFlags3";
		spreadsheetCommandGalleryItem94.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem94.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup13.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem92, spreadsheetCommandGalleryItem93, spreadsheetCommandGalleryItem94 });
		spreadsheetCommandGalleryItemGroup14.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
		spreadsheetCommandGalleryItem95.Caption = "3 Stars";
		spreadsheetCommandGalleryItem95.CommandName = "ConditionalFormattingIconSetStars3";
		spreadsheetCommandGalleryItem95.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem95.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem96.Caption = "4 Ratings";
		spreadsheetCommandGalleryItem96.CommandName = "ConditionalFormattingIconSetRatings4";
		spreadsheetCommandGalleryItem96.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem96.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem97.Caption = "5 Ratings";
		spreadsheetCommandGalleryItem97.CommandName = "ConditionalFormattingIconSetRatings5";
		spreadsheetCommandGalleryItem97.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem97.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem98.Caption = "5 Quarters";
		spreadsheetCommandGalleryItem98.CommandName = "ConditionalFormattingIconSetQuarters5";
		spreadsheetCommandGalleryItem98.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem98.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem99.Caption = "5 Boxes";
		spreadsheetCommandGalleryItem99.CommandName = "ConditionalFormattingIconSetBoxes5";
		spreadsheetCommandGalleryItem99.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem99.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup14.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem95, spreadsheetCommandGalleryItem96, spreadsheetCommandGalleryItem97, spreadsheetCommandGalleryItem98, spreadsheetCommandGalleryItem99 });
		this.commandBarGalleryDropDown21.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[4] { spreadsheetCommandGalleryItemGroup11, spreadsheetCommandGalleryItemGroup12, spreadsheetCommandGalleryItemGroup13, spreadsheetCommandGalleryItemGroup14 });
		this.commandBarGalleryDropDown21.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown21.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown21.Manager = this.barManager1;
		this.commandBarGalleryDropDown21.Name = "commandBarGalleryDropDown21";
		this.commandBarGalleryDropDown22.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown22.Gallery.ColumnCount = 7;
		this.commandBarGalleryDropDown22.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown22.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown22.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.commandBarGalleryDropDown22.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown22.Manager = this.barManager1;
		this.commandBarGalleryDropDown22.Name = "commandBarGalleryDropDown22";
		this.commandBarGalleryDropDown6.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown6.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown6.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown6.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.commandBarGalleryDropDown6.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown6.Gallery.ItemSize = new System.Drawing.Size(136, 26);
		this.commandBarGalleryDropDown6.Gallery.RowCount = 14;
		this.commandBarGalleryDropDown6.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown6.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown6.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown6.Manager = this.barManager1;
		this.commandBarGalleryDropDown6.Name = "commandBarGalleryDropDown6";
		this.commandBarGalleryDropDown7.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup15.Caption = "Gradient Fill";
		spreadsheetCommandGalleryItemGroup15.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
		spreadsheetCommandGalleryItem100.Caption = "Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem100.CommandName = "ConditionalFormattingDataBarGradientBlue";
		spreadsheetCommandGalleryItem100.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem100.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem101.Caption = "Green Data Bar (Gradient)";
		spreadsheetCommandGalleryItem101.CommandName = "ConditionalFormattingDataBarGradientGreen";
		spreadsheetCommandGalleryItem101.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem101.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem102.Caption = "Red Data Bar (Gradient)";
		spreadsheetCommandGalleryItem102.CommandName = "ConditionalFormattingDataBarGradientRed";
		spreadsheetCommandGalleryItem102.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem102.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem103.Caption = "Orange Data Bar (Gradient)";
		spreadsheetCommandGalleryItem103.CommandName = "ConditionalFormattingDataBarGradientOrange";
		spreadsheetCommandGalleryItem103.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem103.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem104.Caption = "Light Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem104.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
		spreadsheetCommandGalleryItem104.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem104.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem105.Caption = "Purple Data Bar (Gradient)";
		spreadsheetCommandGalleryItem105.CommandName = "ConditionalFormattingDataBarGradientPurple";
		spreadsheetCommandGalleryItem105.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem105.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup15.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem100, spreadsheetCommandGalleryItem101, spreadsheetCommandGalleryItem102, spreadsheetCommandGalleryItem103, spreadsheetCommandGalleryItem104, spreadsheetCommandGalleryItem105 });
		spreadsheetCommandGalleryItemGroup16.Caption = "Solid Fill";
		spreadsheetCommandGalleryItemGroup16.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
		spreadsheetCommandGalleryItem106.Caption = "Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem106.CommandName = "ConditionalFormattingDataBarSolidBlue";
		spreadsheetCommandGalleryItem106.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem106.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem107.Caption = "Green Data Bar (Solid)";
		spreadsheetCommandGalleryItem107.CommandName = "ConditionalFormattingDataBarSolidGreen";
		spreadsheetCommandGalleryItem107.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem107.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem108.Caption = "Red Data Bar (Solid)";
		spreadsheetCommandGalleryItem108.CommandName = "ConditionalFormattingDataBarSolidRed";
		spreadsheetCommandGalleryItem108.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem108.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem109.Caption = "Orange Data Bar (Solid)";
		spreadsheetCommandGalleryItem109.CommandName = "ConditionalFormattingDataBarSolidOrange";
		spreadsheetCommandGalleryItem109.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem109.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem110.Caption = "Light Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem110.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
		spreadsheetCommandGalleryItem110.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem110.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem111.Caption = "Purple Data Bar (Solid)";
		spreadsheetCommandGalleryItem111.CommandName = "ConditionalFormattingDataBarSolidPurple";
		spreadsheetCommandGalleryItem111.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem111.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup16.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem106, spreadsheetCommandGalleryItem107, spreadsheetCommandGalleryItem108, spreadsheetCommandGalleryItem109, spreadsheetCommandGalleryItem110, spreadsheetCommandGalleryItem111 });
		this.commandBarGalleryDropDown7.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup15, spreadsheetCommandGalleryItemGroup16 });
		this.commandBarGalleryDropDown7.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown7.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown7.Manager = this.barManager1;
		this.commandBarGalleryDropDown7.Name = "commandBarGalleryDropDown7";
		this.commandBarGalleryDropDown8.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup17.Caption = "Color Scales";
		spreadsheetCommandGalleryItemGroup17.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		spreadsheetCommandGalleryItem112.Caption = "Green - Yellow - Red Color Scale";
		spreadsheetCommandGalleryItem112.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
		spreadsheetCommandGalleryItem112.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem112.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem113.Caption = "Red - Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem113.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
		spreadsheetCommandGalleryItem113.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem113.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem114.Caption = "Green - White - Red Color Scale";
		spreadsheetCommandGalleryItem114.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
		spreadsheetCommandGalleryItem114.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem114.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem115.Caption = "Red - White - Green Color Scale";
		spreadsheetCommandGalleryItem115.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
		spreadsheetCommandGalleryItem115.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem115.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem116.Caption = "Blue - White - Red Color Scale";
		spreadsheetCommandGalleryItem116.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
		spreadsheetCommandGalleryItem116.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem116.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem117.Caption = "Red - White - Blue Color Scale";
		spreadsheetCommandGalleryItem117.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
		spreadsheetCommandGalleryItem117.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem117.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem118.Caption = "White - Red Color Scale";
		spreadsheetCommandGalleryItem118.CommandName = "ConditionalFormattingColorScaleWhiteRed";
		spreadsheetCommandGalleryItem118.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem118.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem119.Caption = "Red - White Color Scale";
		spreadsheetCommandGalleryItem119.CommandName = "ConditionalFormattingColorScaleRedWhite";
		spreadsheetCommandGalleryItem119.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem119.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem120.Caption = "Green - White Color Scale";
		spreadsheetCommandGalleryItem120.CommandName = "ConditionalFormattingColorScaleGreenWhite";
		spreadsheetCommandGalleryItem120.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem120.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem121.Caption = "White - Green Color Scale";
		spreadsheetCommandGalleryItem121.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
		spreadsheetCommandGalleryItem121.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem121.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem122.Caption = "Green - Yellow Color Scale";
		spreadsheetCommandGalleryItem122.CommandName = "ConditionalFormattingColorScaleGreenYellow";
		spreadsheetCommandGalleryItem122.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem122.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem123.Caption = "Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem123.CommandName = "ConditionalFormattingColorScaleYellowGreen";
		spreadsheetCommandGalleryItem123.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem123.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItemGroup17.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[12]
		{
			spreadsheetCommandGalleryItem112, spreadsheetCommandGalleryItem113, spreadsheetCommandGalleryItem114, spreadsheetCommandGalleryItem115, spreadsheetCommandGalleryItem116, spreadsheetCommandGalleryItem117, spreadsheetCommandGalleryItem118, spreadsheetCommandGalleryItem119, spreadsheetCommandGalleryItem120, spreadsheetCommandGalleryItem121,
			spreadsheetCommandGalleryItem122, spreadsheetCommandGalleryItem123
		});
		this.commandBarGalleryDropDown8.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup17 });
		this.commandBarGalleryDropDown8.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown8.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown8.Manager = this.barManager1;
		this.commandBarGalleryDropDown8.Name = "commandBarGalleryDropDown8";
		this.commandBarGalleryDropDown9.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup18.Caption = "Directional";
		spreadsheetCommandGalleryItemGroup18.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
		spreadsheetCommandGalleryItem124.Caption = "3 Arrows (Colored)";
		spreadsheetCommandGalleryItem124.CommandName = "ConditionalFormattingIconSetArrows3Colored";
		spreadsheetCommandGalleryItem124.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem124.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem125.Caption = "3 Arrows (Gray)";
		spreadsheetCommandGalleryItem125.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
		spreadsheetCommandGalleryItem125.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem125.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem126.Caption = "4 Arrows (Colored)";
		spreadsheetCommandGalleryItem126.CommandName = "ConditionalFormattingIconSetArrows4Colored";
		spreadsheetCommandGalleryItem126.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem126.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem127.Caption = "4 Arrows (Gray)";
		spreadsheetCommandGalleryItem127.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
		spreadsheetCommandGalleryItem127.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem127.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem128.Caption = "5 Arrows (Colored)";
		spreadsheetCommandGalleryItem128.CommandName = "ConditionalFormattingIconSetArrows5Colored";
		spreadsheetCommandGalleryItem128.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem128.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem129.Caption = "5 Arrows (Gray)";
		spreadsheetCommandGalleryItem129.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
		spreadsheetCommandGalleryItem129.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem129.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem130.Caption = "3 Triangles";
		spreadsheetCommandGalleryItem130.CommandName = "ConditionalFormattingIconSetTriangles3";
		spreadsheetCommandGalleryItem130.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem130.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup18.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem124, spreadsheetCommandGalleryItem125, spreadsheetCommandGalleryItem126, spreadsheetCommandGalleryItem127, spreadsheetCommandGalleryItem128, spreadsheetCommandGalleryItem129, spreadsheetCommandGalleryItem130 });
		spreadsheetCommandGalleryItemGroup19.Caption = "Shapes";
		spreadsheetCommandGalleryItemGroup19.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
		spreadsheetCommandGalleryItem131.Caption = "3 Traffic Lights (Unrimmed)";
		spreadsheetCommandGalleryItem131.CommandName = "ConditionalFormattingIconSetTrafficLights3";
		spreadsheetCommandGalleryItem131.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem131.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem132.Caption = "3 Traffic Lights (Rimmed)";
		spreadsheetCommandGalleryItem132.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
		spreadsheetCommandGalleryItem132.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem132.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem133.Caption = "4 Traffic Lights";
		spreadsheetCommandGalleryItem133.CommandName = "ConditionalFormattingIconSetTrafficLights4";
		spreadsheetCommandGalleryItem133.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem133.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem134.Caption = "3 Signs";
		spreadsheetCommandGalleryItem134.CommandName = "ConditionalFormattingIconSetSigns3";
		spreadsheetCommandGalleryItem134.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem134.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem135.Caption = "Red To Black";
		spreadsheetCommandGalleryItem135.CommandName = "ConditionalFormattingIconSetRedToBlack";
		spreadsheetCommandGalleryItem135.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem135.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup19.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem131, spreadsheetCommandGalleryItem132, spreadsheetCommandGalleryItem133, spreadsheetCommandGalleryItem134, spreadsheetCommandGalleryItem135 });
		spreadsheetCommandGalleryItemGroup20.Caption = "Indicators";
		spreadsheetCommandGalleryItemGroup20.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
		spreadsheetCommandGalleryItem136.Caption = "3 Symbols (Circled)";
		spreadsheetCommandGalleryItem136.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
		spreadsheetCommandGalleryItem136.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem136.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem137.Caption = "3 Symbols (Uncircled)";
		spreadsheetCommandGalleryItem137.CommandName = "ConditionalFormattingIconSetSymbols3";
		spreadsheetCommandGalleryItem137.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem137.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem138.Caption = "3 Flags";
		spreadsheetCommandGalleryItem138.CommandName = "ConditionalFormattingIconSetFlags3";
		spreadsheetCommandGalleryItem138.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem138.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup20.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem136, spreadsheetCommandGalleryItem137, spreadsheetCommandGalleryItem138 });
		spreadsheetCommandGalleryItemGroup21.Caption = "Ratings";
		spreadsheetCommandGalleryItemGroup21.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
		spreadsheetCommandGalleryItem139.Caption = "3 Stars";
		spreadsheetCommandGalleryItem139.CommandName = "ConditionalFormattingIconSetStars3";
		spreadsheetCommandGalleryItem139.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem139.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem140.Caption = "4 Ratings";
		spreadsheetCommandGalleryItem140.CommandName = "ConditionalFormattingIconSetRatings4";
		spreadsheetCommandGalleryItem140.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem140.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem141.Caption = "5 Ratings";
		spreadsheetCommandGalleryItem141.CommandName = "ConditionalFormattingIconSetRatings5";
		spreadsheetCommandGalleryItem141.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem141.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem142.Caption = "5 Quarters";
		spreadsheetCommandGalleryItem142.CommandName = "ConditionalFormattingIconSetQuarters5";
		spreadsheetCommandGalleryItem142.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem142.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem143.Caption = "5 Boxes";
		spreadsheetCommandGalleryItem143.CommandName = "ConditionalFormattingIconSetBoxes5";
		spreadsheetCommandGalleryItem143.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem143.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup21.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem139, spreadsheetCommandGalleryItem140, spreadsheetCommandGalleryItem141, spreadsheetCommandGalleryItem142, spreadsheetCommandGalleryItem143 });
		this.commandBarGalleryDropDown9.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[4] { spreadsheetCommandGalleryItemGroup18, spreadsheetCommandGalleryItemGroup19, spreadsheetCommandGalleryItemGroup20, spreadsheetCommandGalleryItemGroup21 });
		this.commandBarGalleryDropDown9.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown9.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown9.Manager = this.barManager1;
		this.commandBarGalleryDropDown9.Name = "commandBarGalleryDropDown9";
		this.commandBarGalleryDropDown10.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown10.Gallery.ColumnCount = 7;
		this.commandBarGalleryDropDown10.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown10.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown10.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.commandBarGalleryDropDown10.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown10.Manager = this.barManager1;
		this.commandBarGalleryDropDown10.Name = "commandBarGalleryDropDown10";
		this.commandBarGalleryDropDown11.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup22.Caption = "2-D Column";
		spreadsheetCommandGalleryItemGroup22.CommandName = "InsertChartColumn2DCommandGroup";
		spreadsheetCommandGalleryItem144.Caption = "Clustered Column";
		spreadsheetCommandGalleryItem144.CommandName = "InsertChartColumnClustered2D";
		spreadsheetCommandGalleryItem144.Description = "Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the order of categories is not important or for displaying item counts such as a histogram.";
		spreadsheetCommandGalleryItem144.Hint = "Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the order of categories is not important or for displaying item counts such as a histogram.";
		spreadsheetCommandGalleryItem145.Caption = "Stacked Column";
		spreadsheetCommandGalleryItem145.CommandName = "InsertChartColumnStacked2D";
		spreadsheetCommandGalleryItem145.Description = "Compare the contribution of each value to a total across categories by using vertical rectangles.\r\n\r\nUse it to emphasize the total across series for one category.";
		spreadsheetCommandGalleryItem145.Hint = "Compare the contribution of each value to a total across categories by using vertical rectangles.\r\n\r\nUse it to emphasize the total across series for one category.";
		spreadsheetCommandGalleryItem146.Caption = "100% Stacked Column";
		spreadsheetCommandGalleryItem146.CommandName = "InsertChartColumnPercentStacked2D";
		spreadsheetCommandGalleryItem146.Description = "Compare the percentage that each value contributes to a total across categories by using vertical rectangles.\r\n\r\nUse is to emphasize the proportion of each data series.";
		spreadsheetCommandGalleryItem146.Hint = "Compare the percentage that each value contributes to a total across categories by using vertical rectangles.\r\n\r\nUse is to emphasize the proportion of each data series.";
		spreadsheetCommandGalleryItemGroup22.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem144, spreadsheetCommandGalleryItem145, spreadsheetCommandGalleryItem146 });
		spreadsheetCommandGalleryItemGroup23.Caption = "3-D Column";
		spreadsheetCommandGalleryItemGroup23.CommandName = "InsertChartColumn3DCommandGroup";
		spreadsheetCommandGalleryItem147.Caption = "3-D Clustered Column";
		spreadsheetCommandGalleryItem147.CommandName = "InsertChartColumnClustered3D";
		spreadsheetCommandGalleryItem147.Description = "Compare values across categories and display clustered columns in 3-D format.";
		spreadsheetCommandGalleryItem147.Hint = "Compare values across categories and display clustered columns in 3-D format.";
		spreadsheetCommandGalleryItem148.Caption = "Stacked Column in 3-D";
		spreadsheetCommandGalleryItem148.CommandName = "InsertChartColumnStacked3D";
		spreadsheetCommandGalleryItem148.Description = "Compare the contribution of each value to a total across categories and display stacked columns in 3-D format.";
		spreadsheetCommandGalleryItem148.Hint = "Compare the contribution of each value to a total across categories and display stacked columns in 3-D format.";
		spreadsheetCommandGalleryItem149.Caption = "100% Stacked Column in 3-D";
		spreadsheetCommandGalleryItem149.CommandName = "InsertChartColumnPercentStacked3D";
		spreadsheetCommandGalleryItem149.Description = "Compare the percentage that each value contributes to a total across categories and display 100% stacked columns in 3-D format.";
		spreadsheetCommandGalleryItem149.Hint = "Compare the percentage that each value contributes to a total across categories and display 100% stacked columns in 3-D format.";
		spreadsheetCommandGalleryItem150.Caption = "3-D Column";
		spreadsheetCommandGalleryItem150.CommandName = "InsertChartColumn3D";
		spreadsheetCommandGalleryItem150.Description = "Compare values across categories and across series on three axes.\r\n\r\nUse it when the categories and series are equally important.";
		spreadsheetCommandGalleryItem150.Hint = "Compare values across categories and across series on three axes.\r\n\r\nUse it when the categories and series are equally important.";
		spreadsheetCommandGalleryItemGroup23.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem147, spreadsheetCommandGalleryItem148, spreadsheetCommandGalleryItem149, spreadsheetCommandGalleryItem150 });
		spreadsheetCommandGalleryItemGroup24.Caption = "Cylinder";
		spreadsheetCommandGalleryItemGroup24.CommandName = "InsertChartCylinderCommandGroup";
		spreadsheetCommandGalleryItem151.Caption = "Clustered Cylinder";
		spreadsheetCommandGalleryItem151.CommandName = "InsertChartCylinderClustered";
		spreadsheetCommandGalleryItem151.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem151.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem152.Caption = "Stacked Cylinder";
		spreadsheetCommandGalleryItem152.CommandName = "InsertChartCylinderStacked";
		spreadsheetCommandGalleryItem152.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem152.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem153.Caption = "100% Stacked Cylinder";
		spreadsheetCommandGalleryItem153.CommandName = "InsertChartCylinderPercentStacked";
		spreadsheetCommandGalleryItem153.Description = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem153.Hint = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem154.Caption = "3-D Cylinder";
		spreadsheetCommandGalleryItem154.CommandName = "InsertChartCylinder";
		spreadsheetCommandGalleryItem154.Description = "Compare values across categories and across series and display a cylinder chart on three axes.";
		spreadsheetCommandGalleryItem154.Hint = "Compare values across categories and across series and display a cylinder chart on three axes.";
		spreadsheetCommandGalleryItemGroup24.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem151, spreadsheetCommandGalleryItem152, spreadsheetCommandGalleryItem153, spreadsheetCommandGalleryItem154 });
		spreadsheetCommandGalleryItemGroup25.Caption = "Cone";
		spreadsheetCommandGalleryItemGroup25.CommandName = "InsertChartConeCommandGroup";
		spreadsheetCommandGalleryItem155.Caption = "Clustered Cone";
		spreadsheetCommandGalleryItem155.CommandName = "InsertChartConeClustered";
		spreadsheetCommandGalleryItem155.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem155.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem156.Caption = "Stacked Cone";
		spreadsheetCommandGalleryItem156.CommandName = "InsertChartConeStacked";
		spreadsheetCommandGalleryItem156.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem156.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem157.Caption = "100% Stacked Cone";
		spreadsheetCommandGalleryItem157.CommandName = "InsertChartConePercentStacked";
		spreadsheetCommandGalleryItem157.Description = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem157.Hint = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem158.Caption = "3-D Cone";
		spreadsheetCommandGalleryItem158.CommandName = "InsertChartCone";
		spreadsheetCommandGalleryItem158.Description = "Compare values across categories and across series and display a cone chart on three axes.";
		spreadsheetCommandGalleryItem158.Hint = "Compare values across categories and across series and display a cone chart on three axes.";
		spreadsheetCommandGalleryItemGroup25.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem155, spreadsheetCommandGalleryItem156, spreadsheetCommandGalleryItem157, spreadsheetCommandGalleryItem158 });
		spreadsheetCommandGalleryItemGroup26.Caption = "Pyramid";
		spreadsheetCommandGalleryItemGroup26.CommandName = "InsertChartPyramidCommandGroup";
		spreadsheetCommandGalleryItem159.Caption = "Clustered Pyramid";
		spreadsheetCommandGalleryItem159.CommandName = "InsertChartPyramidClustered";
		spreadsheetCommandGalleryItem159.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem159.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem160.Caption = "Stacked Pyramid";
		spreadsheetCommandGalleryItem160.CommandName = "InsertChartPyramidStacked";
		spreadsheetCommandGalleryItem160.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem160.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem161.Caption = "100% Stacked Pyramid";
		spreadsheetCommandGalleryItem161.CommandName = "InsertChartPyramidPercentStacked";
		spreadsheetCommandGalleryItem161.Description = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem161.Hint = "Compare the percentage that each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem162.Caption = "3-D Pyramid";
		spreadsheetCommandGalleryItem162.CommandName = "InsertChartPyramid";
		spreadsheetCommandGalleryItem162.Description = "Compare values across categories and across series and display a pyramid chart on three axes.";
		spreadsheetCommandGalleryItem162.Hint = "Compare values across categories and across series and display a pyramid chart on three axes.";
		spreadsheetCommandGalleryItemGroup26.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem159, spreadsheetCommandGalleryItem160, spreadsheetCommandGalleryItem161, spreadsheetCommandGalleryItem162 });
		this.commandBarGalleryDropDown11.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[5] { spreadsheetCommandGalleryItemGroup22, spreadsheetCommandGalleryItemGroup23, spreadsheetCommandGalleryItemGroup24, spreadsheetCommandGalleryItemGroup25, spreadsheetCommandGalleryItemGroup26 });
		this.commandBarGalleryDropDown11.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown11.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown11.Manager = this.barManager1;
		this.commandBarGalleryDropDown11.Name = "commandBarGalleryDropDown11";
		this.commandBarGalleryDropDown12.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup27.Caption = "2-D Line";
		spreadsheetCommandGalleryItemGroup27.CommandName = "InsertChartLine2DCommandGroup";
		spreadsheetCommandGalleryItem163.Caption = "Line";
		spreadsheetCommandGalleryItem163.CommandName = "InsertChartLine";
		spreadsheetCommandGalleryItem163.Description = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are many data points and the order is important.";
		spreadsheetCommandGalleryItem163.Hint = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are many data points and the order is important.";
		spreadsheetCommandGalleryItem164.Caption = "Stacked Line";
		spreadsheetCommandGalleryItem164.CommandName = "InsertChartStackedLine";
		spreadsheetCommandGalleryItem164.Description = "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.";
		spreadsheetCommandGalleryItem164.Hint = "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.";
		spreadsheetCommandGalleryItem165.Caption = "100% Stacked line";
		spreadsheetCommandGalleryItem165.CommandName = "InsertChartPercentStackedLine";
		spreadsheetCommandGalleryItem165.Description = "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.";
		spreadsheetCommandGalleryItem165.Hint = "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.";
		spreadsheetCommandGalleryItem166.Caption = "Line with Markers";
		spreadsheetCommandGalleryItem166.CommandName = "InsertChartLineWithMarkers";
		spreadsheetCommandGalleryItem166.Description = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are only a few data points.";
		spreadsheetCommandGalleryItem166.Hint = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are only a few data points.";
		spreadsheetCommandGalleryItem167.Caption = "Stacked Line with Markers";
		spreadsheetCommandGalleryItem167.CommandName = "InsertChartStackedLineWithMarkers";
		spreadsheetCommandGalleryItem167.Description = "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.";
		spreadsheetCommandGalleryItem167.Hint = "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.";
		spreadsheetCommandGalleryItem168.Caption = "100% Stacked Line with Markers";
		spreadsheetCommandGalleryItem168.CommandName = "InsertChartPercentStackedLineWithMarkers";
		spreadsheetCommandGalleryItem168.Description = "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.";
		spreadsheetCommandGalleryItem168.Hint = "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.";
		spreadsheetCommandGalleryItemGroup27.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem163, spreadsheetCommandGalleryItem164, spreadsheetCommandGalleryItem165, spreadsheetCommandGalleryItem166, spreadsheetCommandGalleryItem167, spreadsheetCommandGalleryItem168 });
		spreadsheetCommandGalleryItemGroup28.Caption = "3-D Line";
		spreadsheetCommandGalleryItemGroup28.CommandName = "InsertChartLine3DCommandGroup";
		spreadsheetCommandGalleryItem169.Caption = "3-D Line";
		spreadsheetCommandGalleryItem169.CommandName = "InsertChartLine3D";
		spreadsheetCommandGalleryItem169.Description = "Display each row or column of data as a 3-D ribbon on three axes.";
		spreadsheetCommandGalleryItem169.Hint = "Display each row or column of data as a 3-D ribbon on three axes.";
		spreadsheetCommandGalleryItemGroup28.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[1] { spreadsheetCommandGalleryItem169 });
		this.commandBarGalleryDropDown12.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup27, spreadsheetCommandGalleryItemGroup28 });
		this.commandBarGalleryDropDown12.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown12.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown12.Manager = this.barManager1;
		this.commandBarGalleryDropDown12.Name = "commandBarGalleryDropDown12";
		this.commandBarGalleryDropDown13.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup29.Caption = "2-D Pie";
		spreadsheetCommandGalleryItemGroup29.CommandName = "InsertChartPie2DCommandGroup";
		spreadsheetCommandGalleryItem170.Caption = "Pie";
		spreadsheetCommandGalleryItem170.CommandName = "InsertChartPie2D";
		spreadsheetCommandGalleryItem170.Description = "Display the contribution of each value to a total.\r\n\r\nUse it when the values can be added together or when you have only one data series and all values are positive.";
		spreadsheetCommandGalleryItem170.Hint = "Display the contribution of each value to a total.\r\n\r\nUse it when the values can be added together or when you have only one data series and all values are positive.";
		spreadsheetCommandGalleryItem171.Caption = "Exploded Pie";
		spreadsheetCommandGalleryItem171.CommandName = "InsertChartPieExploded2D";
		spreadsheetCommandGalleryItem171.Description = "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a pie chart, and explode individual values instead.";
		spreadsheetCommandGalleryItem171.Hint = "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a pie chart, and explode individual values instead.";
		spreadsheetCommandGalleryItemGroup29.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem170, spreadsheetCommandGalleryItem171 });
		spreadsheetCommandGalleryItemGroup30.Caption = "3-D Pie";
		spreadsheetCommandGalleryItemGroup30.CommandName = "InsertChartPie3DCommandGroup";
		spreadsheetCommandGalleryItem172.Caption = "Pie in 3-D";
		spreadsheetCommandGalleryItem172.CommandName = "InsertChartPie3D";
		spreadsheetCommandGalleryItem172.Description = "Display the contribution of each value to a total.";
		spreadsheetCommandGalleryItem172.Hint = "Display the contribution of each value to a total.";
		spreadsheetCommandGalleryItem173.Caption = "Exploded pie in 3-D";
		spreadsheetCommandGalleryItem173.CommandName = "InsertChartPieExploded3D";
		spreadsheetCommandGalleryItem173.Description = "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a 3-D pie chart, and explode individual values instead.";
		spreadsheetCommandGalleryItem173.Hint = "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a 3-D pie chart, and explode individual values instead.";
		spreadsheetCommandGalleryItemGroup30.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem172, spreadsheetCommandGalleryItem173 });
		spreadsheetCommandGalleryItemGroup31.Caption = "Doughnut";
		spreadsheetCommandGalleryItemGroup31.CommandName = "InsertChartDoughnut2DCommandGroup";
		spreadsheetCommandGalleryItem174.Caption = "Doughnut";
		spreadsheetCommandGalleryItem174.CommandName = "InsertChartDoughnut2D";
		spreadsheetCommandGalleryItem174.Description = "Display the contribution of each value to a total like a pie chart, but it can contain multiple series.";
		spreadsheetCommandGalleryItem174.Hint = "Display the contribution of each value to a total like a pie chart, but it can contain multiple series.";
		spreadsheetCommandGalleryItem175.Caption = "Exploded Doughnut";
		spreadsheetCommandGalleryItem175.CommandName = "InsertChartDoughnutExploded2D";
		spreadsheetCommandGalleryItem175.Description = "Display the contribution of each value to a total while emphasizing individual values like an exploded pie chart, but it can contain multiple series.";
		spreadsheetCommandGalleryItem175.Hint = "Display the contribution of each value to a total while emphasizing individual values like an exploded pie chart, but it can contain multiple series.";
		spreadsheetCommandGalleryItemGroup31.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem174, spreadsheetCommandGalleryItem175 });
		this.commandBarGalleryDropDown13.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[3] { spreadsheetCommandGalleryItemGroup29, spreadsheetCommandGalleryItemGroup30, spreadsheetCommandGalleryItemGroup31 });
		this.commandBarGalleryDropDown13.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown13.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown13.Manager = this.barManager1;
		this.commandBarGalleryDropDown13.Name = "commandBarGalleryDropDown13";
		this.commandBarGalleryDropDown14.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup32.Caption = "2-D Bar";
		spreadsheetCommandGalleryItemGroup32.CommandName = "InsertChartBar2DCommandGroup";
		spreadsheetCommandGalleryItem176.Caption = "Clustered Bar";
		spreadsheetCommandGalleryItem176.CommandName = "InsertChartBarClustered2D";
		spreadsheetCommandGalleryItem176.Description = "Compare values across categories using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.";
		spreadsheetCommandGalleryItem176.Hint = "Compare values across categories using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.";
		spreadsheetCommandGalleryItem177.Caption = "Stacked Bar";
		spreadsheetCommandGalleryItem177.CommandName = "InsertChartBarStacked2D";
		spreadsheetCommandGalleryItem177.Description = "Compare the contribution of each value to a total across categories by using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.";
		spreadsheetCommandGalleryItem177.Hint = "Compare the contribution of each value to a total across categories by using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.";
		spreadsheetCommandGalleryItem178.Caption = "100% Stacked Bar";
		spreadsheetCommandGalleryItem178.CommandName = "InsertChartBarPercentStacked2D";
		spreadsheetCommandGalleryItemGroup32.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem176, spreadsheetCommandGalleryItem177, spreadsheetCommandGalleryItem178 });
		spreadsheetCommandGalleryItemGroup33.Caption = "3-D Bar";
		spreadsheetCommandGalleryItemGroup33.CommandName = "InsertChartBar3DCommandGroup";
		spreadsheetCommandGalleryItem179.Caption = "Clustered Bar in 3-D";
		spreadsheetCommandGalleryItem179.CommandName = "InsertChartBarClustered3D";
		spreadsheetCommandGalleryItem179.Description = "Compare values across categories and display clustered bars in 3-D format.";
		spreadsheetCommandGalleryItem179.Hint = "Compare values across categories and display clustered bars in 3-D format.";
		spreadsheetCommandGalleryItem180.Caption = "Stacked Bar in 3-D";
		spreadsheetCommandGalleryItem180.CommandName = "InsertChartBarStacked3D";
		spreadsheetCommandGalleryItem180.Description = "Compare the contribution of each value to a total across categories and display stacked bars in 3-D format.";
		spreadsheetCommandGalleryItem180.Hint = "Compare the contribution of each value to a total across categories and display stacked bars in 3-D format.";
		spreadsheetCommandGalleryItem181.Caption = "100% Stacked Bar in 3-D";
		spreadsheetCommandGalleryItem181.CommandName = "InsertChartBarPercentStacked3D";
		spreadsheetCommandGalleryItem181.Description = "Compare the percentange each value contributes to a total across categories and display 100% stacked bars in 3-D format.";
		spreadsheetCommandGalleryItem181.Hint = "Compare the percentange each value contributes to a total across categories and display 100% stacked bars in 3-D format.";
		spreadsheetCommandGalleryItemGroup33.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem179, spreadsheetCommandGalleryItem180, spreadsheetCommandGalleryItem181 });
		spreadsheetCommandGalleryItemGroup34.Caption = "Cylinder";
		spreadsheetCommandGalleryItemGroup34.CommandName = "InsertChartHorizontalCylinderCommandGroup";
		spreadsheetCommandGalleryItem182.Caption = "Clustered Horizontal Cylinder";
		spreadsheetCommandGalleryItem182.CommandName = "InsertChartHorizontalCylinderClustered";
		spreadsheetCommandGalleryItem182.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem182.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem183.Caption = "Stacked Horizontal Cylinder";
		spreadsheetCommandGalleryItem183.CommandName = "InsertChartHorizontalCylinderStacked";
		spreadsheetCommandGalleryItem183.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem183.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem184.Caption = "100% Stacked Horizontal Cylinder";
		spreadsheetCommandGalleryItem184.CommandName = "InsertChartHorizontalCylinderPercentStacked";
		spreadsheetCommandGalleryItem184.Description = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem184.Hint = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItemGroup34.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem182, spreadsheetCommandGalleryItem183, spreadsheetCommandGalleryItem184 });
		spreadsheetCommandGalleryItemGroup35.Caption = "Cone";
		spreadsheetCommandGalleryItemGroup35.CommandName = "InsertChartHorizontalConeCommandGroup";
		spreadsheetCommandGalleryItem185.Caption = "Clustered Horizontal Cone";
		spreadsheetCommandGalleryItem185.CommandName = "InsertChartHorizontalConeClustered";
		spreadsheetCommandGalleryItem185.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem185.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem186.Caption = "Stacked Horizontal Cone";
		spreadsheetCommandGalleryItem186.CommandName = "InsertChartHorizontalConeStacked";
		spreadsheetCommandGalleryItem186.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem186.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem187.Caption = "100% Stacked Horizontal Cone";
		spreadsheetCommandGalleryItem187.CommandName = "InsertChartHorizontalConePercentStacked";
		spreadsheetCommandGalleryItem187.Description = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem187.Hint = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItemGroup35.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem185, spreadsheetCommandGalleryItem186, spreadsheetCommandGalleryItem187 });
		spreadsheetCommandGalleryItemGroup36.Caption = "Pyramid";
		spreadsheetCommandGalleryItemGroup36.CommandName = "InsertChartHorizontalPyramidCommandGroup";
		spreadsheetCommandGalleryItem188.Caption = "Clustered Horizontal Pyramid";
		spreadsheetCommandGalleryItem188.CommandName = "InsertChartHorizontalPyramidClustered";
		spreadsheetCommandGalleryItem188.Description = "Compare values across categories.";
		spreadsheetCommandGalleryItem188.Hint = "Compare values across categories.";
		spreadsheetCommandGalleryItem189.Caption = "Stacked Horizontal Pyramid";
		spreadsheetCommandGalleryItem189.CommandName = "InsertChartHorizontalPyramidStacked";
		spreadsheetCommandGalleryItem189.Description = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem189.Hint = "Compare the contribution of each value to a total across categories.";
		spreadsheetCommandGalleryItem190.Caption = "100% Stacked Horizontal Pyramid";
		spreadsheetCommandGalleryItem190.CommandName = "InsertChartHorizontalPyramidPercentStacked";
		spreadsheetCommandGalleryItem190.Description = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItem190.Hint = "Compare the percentange each value contributes to a total across categories.";
		spreadsheetCommandGalleryItemGroup36.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem188, spreadsheetCommandGalleryItem189, spreadsheetCommandGalleryItem190 });
		this.commandBarGalleryDropDown14.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[5] { spreadsheetCommandGalleryItemGroup32, spreadsheetCommandGalleryItemGroup33, spreadsheetCommandGalleryItemGroup34, spreadsheetCommandGalleryItemGroup35, spreadsheetCommandGalleryItemGroup36 });
		this.commandBarGalleryDropDown14.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown14.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown14.Manager = this.barManager1;
		this.commandBarGalleryDropDown14.Name = "commandBarGalleryDropDown14";
		this.commandBarGalleryDropDown15.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup37.Caption = "2-D Area";
		spreadsheetCommandGalleryItemGroup37.CommandName = "InsertChartArea2DCommandGroup";
		spreadsheetCommandGalleryItem191.Caption = "Area";
		spreadsheetCommandGalleryItem191.CommandName = "InsertChartArea";
		spreadsheetCommandGalleryItem191.Description = "Display the trend of values over time or categories.";
		spreadsheetCommandGalleryItem191.Hint = "Display the trend of values over time or categories.";
		spreadsheetCommandGalleryItem192.Caption = "Stacked Area";
		spreadsheetCommandGalleryItem192.CommandName = "InsertChartStackedArea";
		spreadsheetCommandGalleryItem192.Description = "Display the trend of the contribution of each value over time or categories.\r\n\r\nUse it to emphasize the trend in the total across series for one category.";
		spreadsheetCommandGalleryItem192.Hint = "Display the trend of the contribution of each value over time or categories.\r\n\r\nUse it to emphasize the trend in the total across series for one category.";
		spreadsheetCommandGalleryItem193.Caption = "100% Stacked Area";
		spreadsheetCommandGalleryItem193.CommandName = "InsertChartPercentStackedArea";
		spreadsheetCommandGalleryItem193.Description = "Display the trend of the percentage each value contibutes over time or categories.\r\n\r\nUse it to emphasize the trend in the proportion of each series.";
		spreadsheetCommandGalleryItem193.Hint = "Display the trend of the percentage each value contibutes over time or categories.\r\n\r\nUse it to emphasize the trend in the proportion of each series.";
		spreadsheetCommandGalleryItemGroup37.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem191, spreadsheetCommandGalleryItem192, spreadsheetCommandGalleryItem193 });
		spreadsheetCommandGalleryItemGroup38.Caption = "3-D Area";
		spreadsheetCommandGalleryItemGroup38.CommandName = "InsertChartArea3DCommandGroup";
		spreadsheetCommandGalleryItem194.Caption = "3-D Area";
		spreadsheetCommandGalleryItem194.CommandName = "InsertChartArea3D";
		spreadsheetCommandGalleryItem194.Description = "Display the trend of values over time or categories using areas on three axes.";
		spreadsheetCommandGalleryItem194.Hint = "Display the trend of values over time or categories using areas on three axes.";
		spreadsheetCommandGalleryItem195.Caption = "Stacked Area in 3-D";
		spreadsheetCommandGalleryItem195.CommandName = "InsertChartStackedArea3D";
		spreadsheetCommandGalleryItem195.Description = "Display the trend of the contribution of each value over time or categories by using stacked areas in a 3-D format.";
		spreadsheetCommandGalleryItem195.Hint = "Display the trend of the contribution of each value over time or categories by using stacked areas in a 3-D format.";
		spreadsheetCommandGalleryItem196.Caption = "100% Stacked Area in 3-D";
		spreadsheetCommandGalleryItem196.CommandName = "InsertChartPercentStackedArea3D";
		spreadsheetCommandGalleryItem196.Description = "Display the trend of the percentage each value contributes over time or categories by using 100% stacked areas in 3-D format.";
		spreadsheetCommandGalleryItem196.Hint = "Display the trend of the percentage each value contributes over time or categories by using 100% stacked areas in 3-D format.";
		spreadsheetCommandGalleryItemGroup38.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem194, spreadsheetCommandGalleryItem195, spreadsheetCommandGalleryItem196 });
		this.commandBarGalleryDropDown15.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup37, spreadsheetCommandGalleryItemGroup38 });
		this.commandBarGalleryDropDown15.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown15.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown15.Manager = this.barManager1;
		this.commandBarGalleryDropDown15.Name = "commandBarGalleryDropDown15";
		this.commandBarGalleryDropDown16.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup39.Caption = "Scatter";
		spreadsheetCommandGalleryItemGroup39.CommandName = "InsertChartScatterCommandGroup";
		spreadsheetCommandGalleryItem197.Caption = "Scatter with only Markers";
		spreadsheetCommandGalleryItem197.CommandName = "InsertChartScatterMarkers";
		spreadsheetCommandGalleryItem197.Description = "Compare pairs of values.\r\n\r\nUse is when the values are not in X-axis order or when they represent separate measurements.";
		spreadsheetCommandGalleryItem197.Hint = "Compare pairs of values.\r\n\r\nUse is when the values are not in X-axis order or when they represent separate measurements.";
		spreadsheetCommandGalleryItem198.Caption = "Scatter with Smooth Lines and Markers";
		spreadsheetCommandGalleryItem198.CommandName = "InsertChartScatterSmoothLinesAndMarkers";
		spreadsheetCommandGalleryItem198.Description = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents a function.";
		spreadsheetCommandGalleryItem198.Hint = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents a function.";
		spreadsheetCommandGalleryItem199.Caption = "Scatter with Smooth Lines";
		spreadsheetCommandGalleryItem199.CommandName = "InsertChartScatterSmoothLines";
		spreadsheetCommandGalleryItem199.Description = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents a function.";
		spreadsheetCommandGalleryItem199.Hint = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents a function.";
		spreadsheetCommandGalleryItem200.Caption = "Scatter with Straight Lines and Markers";
		spreadsheetCommandGalleryItem200.CommandName = "InsertChartScatterLinesAndMarkers";
		spreadsheetCommandGalleryItem200.Description = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents separate samples.";
		spreadsheetCommandGalleryItem200.Hint = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents separate samples.";
		spreadsheetCommandGalleryItem201.Caption = "Scatter with Straight Lines";
		spreadsheetCommandGalleryItem201.CommandName = "InsertChartScatterLines";
		spreadsheetCommandGalleryItem201.Description = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents separate samples.";
		spreadsheetCommandGalleryItem201.Hint = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents separate samples.";
		spreadsheetCommandGalleryItemGroup39.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem197, spreadsheetCommandGalleryItem198, spreadsheetCommandGalleryItem199, spreadsheetCommandGalleryItem200, spreadsheetCommandGalleryItem201 });
		spreadsheetCommandGalleryItemGroup40.Caption = "Bubble";
		spreadsheetCommandGalleryItemGroup40.CommandName = "InsertChartBubbleCommandGroup";
		spreadsheetCommandGalleryItem202.Caption = "Bubble";
		spreadsheetCommandGalleryItem202.CommandName = "InsertChartBubble";
		spreadsheetCommandGalleryItem202.Description = "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker.";
		spreadsheetCommandGalleryItem202.Hint = "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker.";
		spreadsheetCommandGalleryItem203.Caption = "Bubble with a 3-D effect";
		spreadsheetCommandGalleryItem203.CommandName = "InsertChartBubble3D";
		spreadsheetCommandGalleryItem203.Description = "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker, which is displayed with a 3-D effect.";
		spreadsheetCommandGalleryItem203.Hint = "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker, which is displayed with a 3-D effect.";
		spreadsheetCommandGalleryItemGroup40.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem202, spreadsheetCommandGalleryItem203 });
		this.commandBarGalleryDropDown16.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup39, spreadsheetCommandGalleryItemGroup40 });
		this.commandBarGalleryDropDown16.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown16.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown16.Manager = this.barManager1;
		this.commandBarGalleryDropDown16.Name = "commandBarGalleryDropDown16";
		this.commandBarGalleryDropDown17.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup41.Caption = "Stock";
		spreadsheetCommandGalleryItemGroup41.CommandName = "InsertChartStockCommandGroup";
		spreadsheetCommandGalleryItem204.Caption = "High-Low-Close";
		spreadsheetCommandGalleryItem204.CommandName = "InsertChartStockHighLowClose";
		spreadsheetCommandGalleryItem204.Description = "Requires three series of values in order High, Low and Close.";
		spreadsheetCommandGalleryItem204.Hint = "Requires three series of values in order High, Low and Close.";
		spreadsheetCommandGalleryItem205.Caption = "Open-High-Low-Close";
		spreadsheetCommandGalleryItem205.CommandName = "InsertChartStockOpenHighLowClose";
		spreadsheetCommandGalleryItem205.Description = "Requires four series of values in order Open, High, Low and Close.";
		spreadsheetCommandGalleryItem205.Hint = "Requires four series of values in order Open, High, Low and Close.";
		spreadsheetCommandGalleryItem206.Caption = "Volume-High-Low-Close";
		spreadsheetCommandGalleryItem206.CommandName = "InsertChartStockVolumeHighLowClose";
		spreadsheetCommandGalleryItem206.Description = "Requires four series of values in order Volume, High, Low and Close.";
		spreadsheetCommandGalleryItem206.Hint = "Requires four series of values in order Volume, High, Low and Close.";
		spreadsheetCommandGalleryItem207.Caption = "Volume-Open-High-Low-Close";
		spreadsheetCommandGalleryItem207.CommandName = "InsertChartStockVolumeOpenHighLowClose";
		spreadsheetCommandGalleryItem207.Description = "Requires five series of values in order Volume, Open, High, Low and Close.";
		spreadsheetCommandGalleryItem207.Hint = "Requires five series of values in order Volume, Open, High, Low and Close.";
		spreadsheetCommandGalleryItemGroup41.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem204, spreadsheetCommandGalleryItem205, spreadsheetCommandGalleryItem206, spreadsheetCommandGalleryItem207 });
		spreadsheetCommandGalleryItemGroup42.Caption = "Radar";
		spreadsheetCommandGalleryItemGroup42.CommandName = "InsertChartRadarCommandGroup";
		spreadsheetCommandGalleryItem208.Caption = "Radar";
		spreadsheetCommandGalleryItem208.CommandName = "InsertChartRadar";
		spreadsheetCommandGalleryItem208.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.";
		spreadsheetCommandGalleryItem208.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.";
		spreadsheetCommandGalleryItem209.Caption = "Radar with Markers";
		spreadsheetCommandGalleryItem209.CommandName = "InsertChartRadarWithMarkers";
		spreadsheetCommandGalleryItem209.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.";
		spreadsheetCommandGalleryItem209.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.";
		spreadsheetCommandGalleryItem210.Caption = "Filled Radar";
		spreadsheetCommandGalleryItem210.CommandName = "InsertChartRadarFilled";
		spreadsheetCommandGalleryItem210.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable and there is only one series.";
		spreadsheetCommandGalleryItem210.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable and there is only one series.";
		spreadsheetCommandGalleryItemGroup42.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem208, spreadsheetCommandGalleryItem209, spreadsheetCommandGalleryItem210 });
		this.commandBarGalleryDropDown17.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup41, spreadsheetCommandGalleryItemGroup42 });
		this.commandBarGalleryDropDown17.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown17.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown17.Manager = this.barManager1;
		this.commandBarGalleryDropDown17.Name = "commandBarGalleryDropDown17";
		this.commandBarGalleryDropDown2.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup43.Caption = "Gradient Fill";
		spreadsheetCommandGalleryItemGroup43.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
		spreadsheetCommandGalleryItem211.Caption = "Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem211.CommandName = "ConditionalFormattingDataBarGradientBlue";
		spreadsheetCommandGalleryItem211.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem211.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem212.Caption = "Green Data Bar (Gradient)";
		spreadsheetCommandGalleryItem212.CommandName = "ConditionalFormattingDataBarGradientGreen";
		spreadsheetCommandGalleryItem212.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem212.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem213.Caption = "Red Data Bar (Gradient)";
		spreadsheetCommandGalleryItem213.CommandName = "ConditionalFormattingDataBarGradientRed";
		spreadsheetCommandGalleryItem213.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem213.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem214.Caption = "Orange Data Bar (Gradient)";
		spreadsheetCommandGalleryItem214.CommandName = "ConditionalFormattingDataBarGradientOrange";
		spreadsheetCommandGalleryItem214.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem214.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem215.Caption = "Light Blue Data Bar (Gradient)";
		spreadsheetCommandGalleryItem215.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
		spreadsheetCommandGalleryItem215.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem215.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem216.Caption = "Purple Data Bar (Gradient)";
		spreadsheetCommandGalleryItem216.CommandName = "ConditionalFormattingDataBarGradientPurple";
		spreadsheetCommandGalleryItem216.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem216.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup43.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem211, spreadsheetCommandGalleryItem212, spreadsheetCommandGalleryItem213, spreadsheetCommandGalleryItem214, spreadsheetCommandGalleryItem215, spreadsheetCommandGalleryItem216 });
		spreadsheetCommandGalleryItemGroup44.Caption = "Solid Fill";
		spreadsheetCommandGalleryItemGroup44.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
		spreadsheetCommandGalleryItem217.Caption = "Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem217.CommandName = "ConditionalFormattingDataBarSolidBlue";
		spreadsheetCommandGalleryItem217.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem217.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem218.Caption = "Green Data Bar (Solid)";
		spreadsheetCommandGalleryItem218.CommandName = "ConditionalFormattingDataBarSolidGreen";
		spreadsheetCommandGalleryItem218.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem218.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem219.Caption = "Red Data Bar (Solid)";
		spreadsheetCommandGalleryItem219.CommandName = "ConditionalFormattingDataBarSolidRed";
		spreadsheetCommandGalleryItem219.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem219.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem220.Caption = "Orange Data Bar (Solid)";
		spreadsheetCommandGalleryItem220.CommandName = "ConditionalFormattingDataBarSolidOrange";
		spreadsheetCommandGalleryItem220.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem220.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem221.Caption = "Light Blue Data Bar (Solid)";
		spreadsheetCommandGalleryItem221.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
		spreadsheetCommandGalleryItem221.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem221.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem222.Caption = "Purple Data Bar (Solid)";
		spreadsheetCommandGalleryItem222.CommandName = "ConditionalFormattingDataBarSolidPurple";
		spreadsheetCommandGalleryItem222.Description = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItem222.Hint = "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.";
		spreadsheetCommandGalleryItemGroup44.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem217, spreadsheetCommandGalleryItem218, spreadsheetCommandGalleryItem219, spreadsheetCommandGalleryItem220, spreadsheetCommandGalleryItem221, spreadsheetCommandGalleryItem222 });
		this.commandBarGalleryDropDown2.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup43, spreadsheetCommandGalleryItemGroup44 });
		this.commandBarGalleryDropDown2.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown2.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown2.Manager = this.barManager1;
		this.commandBarGalleryDropDown2.Name = "commandBarGalleryDropDown2";
		this.commandBarGalleryDropDown3.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup45.Caption = "Color Scales";
		spreadsheetCommandGalleryItemGroup45.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		spreadsheetCommandGalleryItem223.Caption = "Green - Yellow - Red Color Scale";
		spreadsheetCommandGalleryItem223.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
		spreadsheetCommandGalleryItem223.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem223.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem224.Caption = "Red - Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem224.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
		spreadsheetCommandGalleryItem224.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem224.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem225.Caption = "Green - White - Red Color Scale";
		spreadsheetCommandGalleryItem225.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
		spreadsheetCommandGalleryItem225.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem225.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem226.Caption = "Red - White - Green Color Scale";
		spreadsheetCommandGalleryItem226.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
		spreadsheetCommandGalleryItem226.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem226.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem227.Caption = "Blue - White - Red Color Scale";
		spreadsheetCommandGalleryItem227.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
		spreadsheetCommandGalleryItem227.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem227.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem228.Caption = "Red - White - Blue Color Scale";
		spreadsheetCommandGalleryItem228.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
		spreadsheetCommandGalleryItem228.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem228.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem229.Caption = "White - Red Color Scale";
		spreadsheetCommandGalleryItem229.CommandName = "ConditionalFormattingColorScaleWhiteRed";
		spreadsheetCommandGalleryItem229.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem229.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem230.Caption = "Red - White Color Scale";
		spreadsheetCommandGalleryItem230.CommandName = "ConditionalFormattingColorScaleRedWhite";
		spreadsheetCommandGalleryItem230.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem230.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem231.Caption = "Green - White Color Scale";
		spreadsheetCommandGalleryItem231.CommandName = "ConditionalFormattingColorScaleGreenWhite";
		spreadsheetCommandGalleryItem231.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem231.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem232.Caption = "White - Green Color Scale";
		spreadsheetCommandGalleryItem232.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
		spreadsheetCommandGalleryItem232.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem232.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem233.Caption = "Green - Yellow Color Scale";
		spreadsheetCommandGalleryItem233.CommandName = "ConditionalFormattingColorScaleGreenYellow";
		spreadsheetCommandGalleryItem233.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem233.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem234.Caption = "Yellow - Green Color Scale";
		spreadsheetCommandGalleryItem234.CommandName = "ConditionalFormattingColorScaleYellowGreen";
		spreadsheetCommandGalleryItem234.Description = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItem234.Hint = "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.";
		spreadsheetCommandGalleryItemGroup45.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[12]
		{
			spreadsheetCommandGalleryItem223, spreadsheetCommandGalleryItem224, spreadsheetCommandGalleryItem225, spreadsheetCommandGalleryItem226, spreadsheetCommandGalleryItem227, spreadsheetCommandGalleryItem228, spreadsheetCommandGalleryItem229, spreadsheetCommandGalleryItem230, spreadsheetCommandGalleryItem231, spreadsheetCommandGalleryItem232,
			spreadsheetCommandGalleryItem233, spreadsheetCommandGalleryItem234
		});
		this.commandBarGalleryDropDown3.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup45 });
		this.commandBarGalleryDropDown3.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown3.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown3.Manager = this.barManager1;
		this.commandBarGalleryDropDown3.Name = "commandBarGalleryDropDown3";
		this.commandBarGalleryDropDown4.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup46.Caption = "Directional";
		spreadsheetCommandGalleryItemGroup46.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
		spreadsheetCommandGalleryItem235.Caption = "3 Arrows (Colored)";
		spreadsheetCommandGalleryItem235.CommandName = "ConditionalFormattingIconSetArrows3Colored";
		spreadsheetCommandGalleryItem235.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem235.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem236.Caption = "3 Arrows (Gray)";
		spreadsheetCommandGalleryItem236.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
		spreadsheetCommandGalleryItem236.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem236.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem237.Caption = "4 Arrows (Colored)";
		spreadsheetCommandGalleryItem237.CommandName = "ConditionalFormattingIconSetArrows4Colored";
		spreadsheetCommandGalleryItem237.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem237.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem238.Caption = "4 Arrows (Gray)";
		spreadsheetCommandGalleryItem238.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
		spreadsheetCommandGalleryItem238.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem238.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem239.Caption = "5 Arrows (Colored)";
		spreadsheetCommandGalleryItem239.CommandName = "ConditionalFormattingIconSetArrows5Colored";
		spreadsheetCommandGalleryItem239.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem239.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem240.Caption = "5 Arrows (Gray)";
		spreadsheetCommandGalleryItem240.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
		spreadsheetCommandGalleryItem240.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem240.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem241.Caption = "3 Triangles";
		spreadsheetCommandGalleryItem241.CommandName = "ConditionalFormattingIconSetTriangles3";
		spreadsheetCommandGalleryItem241.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem241.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup46.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem235, spreadsheetCommandGalleryItem236, spreadsheetCommandGalleryItem237, spreadsheetCommandGalleryItem238, spreadsheetCommandGalleryItem239, spreadsheetCommandGalleryItem240, spreadsheetCommandGalleryItem241 });
		spreadsheetCommandGalleryItemGroup47.Caption = "Shapes";
		spreadsheetCommandGalleryItemGroup47.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
		spreadsheetCommandGalleryItem242.Caption = "3 Traffic Lights (Unrimmed)";
		spreadsheetCommandGalleryItem242.CommandName = "ConditionalFormattingIconSetTrafficLights3";
		spreadsheetCommandGalleryItem242.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem242.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem243.Caption = "3 Traffic Lights (Rimmed)";
		spreadsheetCommandGalleryItem243.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
		spreadsheetCommandGalleryItem243.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem243.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem244.Caption = "4 Traffic Lights";
		spreadsheetCommandGalleryItem244.CommandName = "ConditionalFormattingIconSetTrafficLights4";
		spreadsheetCommandGalleryItem244.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem244.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem245.Caption = "3 Signs";
		spreadsheetCommandGalleryItem245.CommandName = "ConditionalFormattingIconSetSigns3";
		spreadsheetCommandGalleryItem245.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem245.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem246.Caption = "Red To Black";
		spreadsheetCommandGalleryItem246.CommandName = "ConditionalFormattingIconSetRedToBlack";
		spreadsheetCommandGalleryItem246.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem246.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup47.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem242, spreadsheetCommandGalleryItem243, spreadsheetCommandGalleryItem244, spreadsheetCommandGalleryItem245, spreadsheetCommandGalleryItem246 });
		spreadsheetCommandGalleryItemGroup48.Caption = "Indicators";
		spreadsheetCommandGalleryItemGroup48.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
		spreadsheetCommandGalleryItem247.Caption = "3 Symbols (Circled)";
		spreadsheetCommandGalleryItem247.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
		spreadsheetCommandGalleryItem247.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem247.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem248.Caption = "3 Symbols (Uncircled)";
		spreadsheetCommandGalleryItem248.CommandName = "ConditionalFormattingIconSetSymbols3";
		spreadsheetCommandGalleryItem248.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem248.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem249.Caption = "3 Flags";
		spreadsheetCommandGalleryItem249.CommandName = "ConditionalFormattingIconSetFlags3";
		spreadsheetCommandGalleryItem249.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem249.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup48.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem247, spreadsheetCommandGalleryItem248, spreadsheetCommandGalleryItem249 });
		spreadsheetCommandGalleryItemGroup49.Caption = "Ratings";
		spreadsheetCommandGalleryItemGroup49.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
		spreadsheetCommandGalleryItem250.Caption = "3 Stars";
		spreadsheetCommandGalleryItem250.CommandName = "ConditionalFormattingIconSetStars3";
		spreadsheetCommandGalleryItem250.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem250.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem251.Caption = "4 Ratings";
		spreadsheetCommandGalleryItem251.CommandName = "ConditionalFormattingIconSetRatings4";
		spreadsheetCommandGalleryItem251.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem251.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem252.Caption = "5 Ratings";
		spreadsheetCommandGalleryItem252.CommandName = "ConditionalFormattingIconSetRatings5";
		spreadsheetCommandGalleryItem252.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem252.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem253.Caption = "5 Quarters";
		spreadsheetCommandGalleryItem253.CommandName = "ConditionalFormattingIconSetQuarters5";
		spreadsheetCommandGalleryItem253.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem253.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem254.Caption = "5 Boxes";
		spreadsheetCommandGalleryItem254.CommandName = "ConditionalFormattingIconSetBoxes5";
		spreadsheetCommandGalleryItem254.Description = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItem254.Hint = "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.";
		spreadsheetCommandGalleryItemGroup49.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem250, spreadsheetCommandGalleryItem251, spreadsheetCommandGalleryItem252, spreadsheetCommandGalleryItem253, spreadsheetCommandGalleryItem254 });
		this.commandBarGalleryDropDown4.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[4] { spreadsheetCommandGalleryItemGroup46, spreadsheetCommandGalleryItemGroup47, spreadsheetCommandGalleryItemGroup48, spreadsheetCommandGalleryItemGroup49 });
		this.commandBarGalleryDropDown4.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown4.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown4.Manager = this.barManager1;
		this.commandBarGalleryDropDown4.Name = "commandBarGalleryDropDown4";
		this.commandBarGalleryDropDown5.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown5.Gallery.ColumnCount = 7;
		this.commandBarGalleryDropDown5.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown5.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown5.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.commandBarGalleryDropDown5.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown5.Manager = this.barManager1;
		this.commandBarGalleryDropDown5.Name = "commandBarGalleryDropDown5";
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.Location = new System.Drawing.Point(712, 602);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(121, 23);
		this.simpleButton1.TabIndex = 1;
		this.simpleButton1.Text = "Generate Template";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click_1);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 30);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_listReport);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tab_dashboard);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1007, 566);
		this.splitContainerControl1.SplitterPosition = 169;
		this.splitContainerControl1.TabIndex = 7;
		this.grd_listReport.AllowPasteExcel = true;
		this.grd_listReport.ClipboardData = null;
		this.grd_listReport.ContextMenuStrip = this.contextMenuStrip_Doc;
		this.grd_listReport.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_listReport.Location = new System.Drawing.Point(0, 0);
		this.grd_listReport.MainView = this.gv_listReport;
		this.grd_listReport.Name = "grd_listReport";
		this.grd_listReport.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.btn_reportGrid });
		this.grd_listReport.Size = new System.Drawing.Size(1007, 169);
		this.grd_listReport.TabIndex = 0;
		this.grd_listReport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_listReport });
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.stripItem_Add, this.stripItem_Remove, this.stripItem_Refresh });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(118, 70);
		this.stripItem_Add.Name = "stripItem_Add";
		this.stripItem_Add.Size = new System.Drawing.Size(117, 22);
		this.stripItem_Add.Text = "Add";
		this.stripItem_Add.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.stripItem_Remove.Name = "stripItem_Remove";
		this.stripItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.stripItem_Remove.Text = "Remove";
		this.stripItem_Remove.Click += new System.EventHandler(this.stripItem_Remove_Click);
		this.stripItem_Refresh.Name = "stripItem_Refresh";
		this.stripItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.stripItem_Refresh.Text = "Refresh";
		this.stripItem_Refresh.Click += new System.EventHandler(this.stripItem_Refresh_Click);
		this.gv_listReport.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_listReport.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_listReport.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_listReport.ColumnKey = null;
		this.gv_listReport.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_DocEntry, this.gc_ReportId, this.gc_ReportName, this.gc_Active });
		this.gv_listReport.DisableFormSetting = false;
		this.gv_listReport.DisplayException = false;
		this.gv_listReport.EditColumn = null;
		this.gv_listReport.FieldCurrency = null;
		this.gv_listReport.FieldRate = null;
		this.gv_listReport.FormId = "oSettingReport";
		this.gv_listReport.GridControl = this.grd_listReport;
		this.gv_listReport.HasFormSetting = false;
		this.gv_listReport.IgnoreFilter = false;
		this.gv_listReport.IndexEditRow = -1;
		this.gv_listReport.IndicatorWidth = 35;
		this.gv_listReport.IsApzTable = false;
		this.gv_listReport.IsInsertNewRow = false;
		this.gv_listReport.IsLoadingData = false;
		this.gv_listReport.IsMappingContextAction = false;
		this.gv_listReport.ItemId = "APZ_D_OSRT";
		this.gv_listReport.LstColumnAllowBatch = null;
		this.gv_listReport.LstColumnNotAllowBatch = null;
		this.gv_listReport.LstColumnNotEditable = null;
		this.gv_listReport.LstColumNotSetting = null;
		this.gv_listReport.LstOldUDF = null;
		this.gv_listReport.Name = "gv_listReport";
		this.gv_listReport.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_listReport.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_listReport.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_listReport.OptionsPrint.AutoWidth = false;
		this.gv_listReport.OptionsPrint.UsePrintStyles = false;
		this.gv_listReport.OptionsView.BestFitMaxRowCount = 100;
		this.gv_listReport.OptionsView.ColumnAutoWidth = false;
		this.gv_listReport.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_listReport.OptionsView.ShowGroupedColumns = true;
		this.gv_listReport.OptionsView.ShowGroupPanel = false;
		this.gv_listReport.ReadOnly = false;
		this.gv_listReport.SettingReadonly = false;
		this.gv_listReport.UseBatchMod = false;
		this.gv_listReport.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_listReport_FocusedRowChanged);
		this.gv_listReport.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_listReport_CellValueChanged);
		this.gv_listReport.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_listReport_BeforeLeaveRow);
		this.gc_DocEntry.Caption = "Doc Entry";
		this.gc_DocEntry.CurrencyDateField = null;
		this.gc_DocEntry.CurrencyField = null;
		this.gc_DocEntry.CurrencyFixed = null;
		this.gc_DocEntry.CurrencyFixedRate = 0.0;
		this.gc_DocEntry.CurrencyRateField = null;
		this.gc_DocEntry.FieldName = "DocEntry";
		this.gc_DocEntry.Name = "gc_DocEntry";
		this.gc_DocEntry.OptionsColumn.ReadOnly = true;
		this.gc_DocEntry.PrimaryKey = false;
		this.gc_DocEntry.UnitField = null;
		this.gc_DocEntry.Visible = true;
		this.gc_DocEntry.VisibleIndex = 0;
		this.gc_ReportId.Caption = "Report Id";
		this.gc_ReportId.ColumnEdit = this.btn_reportGrid;
		this.gc_ReportId.CurrencyDateField = null;
		this.gc_ReportId.CurrencyField = null;
		this.gc_ReportId.CurrencyFixed = null;
		this.gc_ReportId.CurrencyFixedRate = 0.0;
		this.gc_ReportId.CurrencyRateField = null;
		this.gc_ReportId.FieldName = "ReportId";
		this.gc_ReportId.Name = "gc_ReportId";
		this.gc_ReportId.PrimaryKey = false;
		this.gc_ReportId.UnitField = null;
		this.gc_ReportId.Visible = true;
		this.gc_ReportId.VisibleIndex = 1;
		this.btn_reportGrid.AutoHeight = false;
		this.btn_reportGrid.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_reportGrid.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_reportGrid.ContainerControl = this;
		this.btn_reportGrid.DisplayMember = "Name";
		this.btn_reportGrid.DisplayMember2 = "";
		this.btn_reportGrid.ListMandatoryField = null;
		this.btn_reportGrid.LoadDataSourceWhenEnter = false;
		this.btn_reportGrid.Name = "btn_reportGrid";
		this.btn_reportGrid.SeparateCharacterForMulti = ';';
		this.btn_reportGrid.SeparateString = ", ";
		this.btn_reportGrid.UsingAutoCompleteBox = false;
		this.btn_reportGrid.UsingDataSource = true;
		this.btn_reportGrid.ValidateWhenNoDataSource = false;
		this.btn_reportGrid.ValueMember = "Code";
		this.btn_reportGrid.SelectedFromCFL += new System.EventHandler<Apzon.Forms.UserControls.ButtonEditCFLEventArgs>(this.btn_reportGrid_SelectedFromCFL);
		this.gc_ReportName.Caption = "Report Name";
		this.gc_ReportName.CurrencyDateField = null;
		this.gc_ReportName.CurrencyField = null;
		this.gc_ReportName.CurrencyFixed = null;
		this.gc_ReportName.CurrencyFixedRate = 0.0;
		this.gc_ReportName.CurrencyRateField = null;
		this.gc_ReportName.FieldName = "ReportName";
		this.gc_ReportName.Name = "gc_ReportName";
		this.gc_ReportName.PrimaryKey = false;
		this.gc_ReportName.UnitField = null;
		this.gc_ReportName.Visible = true;
		this.gc_ReportName.VisibleIndex = 2;
		this.gc_Active.Caption = "Active";
		this.gc_Active.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "Active";
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.PrimaryKey = false;
		this.gc_Active.UnitField = null;
		this.gc_Active.Visible = true;
		this.gc_Active.VisibleIndex = 3;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.tab_dashboard.ContextMenuStrip = this.contextMenuStrip_Chart;
		this.tab_dashboard.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tab_dashboard.IsSearchControl = false;
		this.tab_dashboard.Location = new System.Drawing.Point(0, 0);
		this.tab_dashboard.Name = "tab_dashboard";
		this.tab_dashboard.SelectedTabPage = this.xtraTabPageEx1;
		this.tab_dashboard.Size = new System.Drawing.Size(1007, 387);
		this.tab_dashboard.TabIndex = 1;
		this.tab_dashboard.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.xtraTabPageEx3, this.xtraTabPageEx1, this.xtraTabPageEx2, this.tabPage_dashboard });
		this.tab_dashboard.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
		this.contextMenuStrip_Chart.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem, this.refreshToolStripMenuItem });
		this.contextMenuStrip_Chart.Name = "contextMenuStrip_Chart";
		this.contextMenuStrip_Chart.Size = new System.Drawing.Size(118, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addNewToolStripMenuItem.Text = "Add";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.xtraTabPageEx1.Controls.Add(this.splitContainerControl2);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "List Data";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(1005, 362);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "List Data";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.sc_Main);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_listProperties);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1005, 362);
		this.splitContainerControl2.SplitterPosition = 427;
		this.splitContainerControl2.TabIndex = 0;
		this.grd_listProperties.AllowPasteExcel = true;
		this.grd_listProperties.ClipboardData = null;
		this.grd_listProperties.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_listProperties.Location = new System.Drawing.Point(0, 0);
		this.grd_listProperties.MainView = this.gv_listProperties;
		this.grd_listProperties.Name = "grd_listProperties";
		this.grd_listProperties.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[17]
		{
			this.luk_ItemType, this.txt_Size, this.luk_AlignItem, this.luk_JustifyContent, this.repositoryItemCalcEdit1, this.luk_FontWeight, this.repositoryItemCalcEdit2, this.repositoryItemCalcEdit3, this.repositoryItemCalcEdit4, this.repositoryItemCalcEdit5,
			this.luk_TextAlign, this.luk_AlignSelf, this.repositoryItemColorPickEdit1, this.repositoryItemMemoExEdit1, this.luk_FieldName, this.repositoryItemCalcEdit7, this.luk_currencyField
		});
		this.grd_listProperties.Size = new System.Drawing.Size(568, 362);
		this.grd_listProperties.TabIndex = 0;
		this.grd_listProperties.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_listProperties });
		this.gv_listProperties.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_listProperties.ColumnKey = null;
		this.gv_listProperties.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[27]
		{
			this.gc_Type, this.gc_ItemType, this.gc_FieldName, this.gc_ColumnName, this.gc_TableOrderId, this.gc_DemoData, this.gc_size, this.gc_TextBefore, this.gc_TextAfter, this.gc_Color,
			this.gc_AlignItems, this.gc_JustifyContent, this.gc_FontSize, this.gc_FontWeight, this.gc_PaddingTop, this.gc_PaddingBottom, this.gc_PaddingLeft, this.gc_PaddingRight, this.gc_TextAlign, this.gc_AlignSelf,
			this.gc_Left, this.gc_Top, this.gc_Right, this.gc_Bottom, this.gc_DateTimeValueFormat, this.gc_DateTimeDisplayFormat, this.gc_currencyField
		});
		this.gv_listProperties.DisableFormSetting = false;
		this.gv_listProperties.DisplayException = false;
		this.gv_listProperties.EditColumn = null;
		this.gv_listProperties.FieldCurrency = null;
		this.gv_listProperties.FieldRate = null;
		this.gv_listProperties.FormId = "oSettingReport";
		this.gv_listProperties.GridControl = this.grd_listProperties;
		this.gv_listProperties.HasFormSetting = false;
		this.gv_listProperties.IgnoreFilter = false;
		this.gv_listProperties.IndexEditRow = -1;
		this.gv_listProperties.IndicatorWidth = 35;
		this.gv_listProperties.IsApzTable = false;
		this.gv_listProperties.IsInsertNewRow = false;
		this.gv_listProperties.IsLoadingData = false;
		this.gv_listProperties.IsMappingContextAction = false;
		this.gv_listProperties.ItemId = "APZ_D_SRT2";
		this.gv_listProperties.LstColumnAllowBatch = null;
		this.gv_listProperties.LstColumnNotAllowBatch = null;
		this.gv_listProperties.LstColumnNotEditable = null;
		this.gv_listProperties.LstColumNotSetting = null;
		this.gv_listProperties.LstOldUDF = null;
		this.gv_listProperties.Name = "gv_listProperties";
		this.gv_listProperties.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_listProperties.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_listProperties.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_listProperties.OptionsPrint.AutoWidth = false;
		this.gv_listProperties.OptionsPrint.UsePrintStyles = false;
		this.gv_listProperties.OptionsView.BestFitMaxRowCount = 100;
		this.gv_listProperties.OptionsView.ColumnAutoWidth = false;
		this.gv_listProperties.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_listProperties.OptionsView.ShowGroupedColumns = true;
		this.gv_listProperties.OptionsView.ShowGroupPanel = false;
		this.gv_listProperties.ReadOnly = false;
		this.gv_listProperties.SettingReadonly = false;
		this.gv_listProperties.UseBatchMod = false;
		this.gv_listProperties.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_listProperties_RowCellStyle);
		this.gv_listProperties.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_listProperties_ShowingEditor);
		this.gv_listProperties.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_listProperties_FocusedRowChanged);
		this.gv_listProperties.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_listProperties_CellValueChanged);
		this.gc_Type.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(204, 204, 204);
		this.gc_Type.AppearanceCell.BackColor2 = System.Drawing.Color.FromArgb(204, 204, 204);
		this.gc_Type.AppearanceCell.Options.UseBackColor = true;
		this.gc_Type.Caption = "Type";
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.OptionsColumn.ReadOnly = true;
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 0;
		this.gc_ItemType.Caption = "Item Type";
		this.gc_ItemType.ColumnEdit = this.luk_ItemType;
		this.gc_ItemType.CurrencyDateField = null;
		this.gc_ItemType.CurrencyField = null;
		this.gc_ItemType.CurrencyFixed = null;
		this.gc_ItemType.CurrencyFixedRate = 0.0;
		this.gc_ItemType.CurrencyRateField = null;
		this.gc_ItemType.FieldName = "ItemType";
		this.gc_ItemType.Name = "gc_ItemType";
		this.gc_ItemType.PrimaryKey = false;
		this.gc_ItemType.UnitField = null;
		this.gc_ItemType.Visible = true;
		this.gc_ItemType.VisibleIndex = 1;
		this.luk_ItemType.AutoHeight = false;
		this.luk_ItemType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItemType.Name = "luk_ItemType";
		this.luk_ItemType.NullText = "";
		this.gc_FieldName.Caption = "Field Name";
		this.gc_FieldName.ColumnEdit = this.luk_FieldName;
		this.gc_FieldName.CurrencyDateField = null;
		this.gc_FieldName.CurrencyField = null;
		this.gc_FieldName.CurrencyFixed = null;
		this.gc_FieldName.CurrencyFixedRate = 0.0;
		this.gc_FieldName.CurrencyRateField = null;
		this.gc_FieldName.FieldName = "fieldName";
		this.gc_FieldName.Name = "gc_FieldName";
		this.gc_FieldName.PrimaryKey = false;
		this.gc_FieldName.UnitField = null;
		this.gc_FieldName.Visible = true;
		this.gc_FieldName.VisibleIndex = 2;
		this.luk_FieldName.AutoHeight = false;
		this.luk_FieldName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FieldName.DisplayMember = "Name";
		this.luk_FieldName.Name = "luk_FieldName";
		this.luk_FieldName.NullText = "";
		this.luk_FieldName.PopupView = this.repositoryItemSearchLookUpEdit1View;
		this.luk_FieldName.ValueMember = "Code";
		this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
		this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gc_ColumnName.Caption = "Column Name";
		this.gc_ColumnName.CurrencyDateField = null;
		this.gc_ColumnName.CurrencyField = null;
		this.gc_ColumnName.CurrencyFixed = null;
		this.gc_ColumnName.CurrencyFixedRate = 0.0;
		this.gc_ColumnName.CurrencyRateField = null;
		this.gc_ColumnName.FieldName = "ColumnName";
		this.gc_ColumnName.Name = "gc_ColumnName";
		this.gc_ColumnName.PrimaryKey = false;
		this.gc_ColumnName.UnitField = null;
		this.gc_ColumnName.Visible = true;
		this.gc_ColumnName.VisibleIndex = 3;
		this.gc_TableOrderId.Caption = "Table order id";
		this.gc_TableOrderId.ColumnEdit = this.repositoryItemCalcEdit7;
		this.gc_TableOrderId.CurrencyDateField = null;
		this.gc_TableOrderId.CurrencyField = null;
		this.gc_TableOrderId.CurrencyFixed = null;
		this.gc_TableOrderId.CurrencyFixedRate = 0.0;
		this.gc_TableOrderId.CurrencyRateField = null;
		this.gc_TableOrderId.FieldName = "TableOrderId";
		this.gc_TableOrderId.Name = "gc_TableOrderId";
		this.gc_TableOrderId.PrimaryKey = false;
		this.gc_TableOrderId.UnitField = null;
		this.gc_TableOrderId.Visible = true;
		this.gc_TableOrderId.VisibleIndex = 4;
		this.repositoryItemCalcEdit7.AutoHeight = false;
		this.repositoryItemCalcEdit7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit7.Name = "repositoryItemCalcEdit7";
		this.gc_DemoData.Caption = "Demo Data";
		this.gc_DemoData.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gc_DemoData.CurrencyDateField = null;
		this.gc_DemoData.CurrencyField = null;
		this.gc_DemoData.CurrencyFixed = null;
		this.gc_DemoData.CurrencyFixedRate = 0.0;
		this.gc_DemoData.CurrencyRateField = null;
		this.gc_DemoData.FieldName = "DemoData";
		this.gc_DemoData.Name = "gc_DemoData";
		this.gc_DemoData.PrimaryKey = false;
		this.gc_DemoData.UnitField = null;
		this.gc_DemoData.Visible = true;
		this.gc_DemoData.VisibleIndex = 5;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gc_size.Caption = "Size";
		this.gc_size.ColumnEdit = this.txt_Size;
		this.gc_size.CurrencyDateField = null;
		this.gc_size.CurrencyField = null;
		this.gc_size.CurrencyFixed = null;
		this.gc_size.CurrencyFixedRate = 0.0;
		this.gc_size.CurrencyRateField = null;
		this.gc_size.FieldName = "size";
		this.gc_size.Name = "gc_size";
		this.gc_size.PrimaryKey = false;
		this.gc_size.UnitField = null;
		this.gc_size.Visible = true;
		this.gc_size.VisibleIndex = 6;
		this.txt_Size.AutoHeight = false;
		this.txt_Size.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Size.Name = "txt_Size";
		this.gc_TextBefore.Caption = "Text Before";
		this.gc_TextBefore.CurrencyDateField = null;
		this.gc_TextBefore.CurrencyField = null;
		this.gc_TextBefore.CurrencyFixed = null;
		this.gc_TextBefore.CurrencyFixedRate = 0.0;
		this.gc_TextBefore.CurrencyRateField = null;
		this.gc_TextBefore.FieldName = "textBefore";
		this.gc_TextBefore.Name = "gc_TextBefore";
		this.gc_TextBefore.PrimaryKey = false;
		this.gc_TextBefore.UnitField = null;
		this.gc_TextBefore.Visible = true;
		this.gc_TextBefore.VisibleIndex = 7;
		this.gc_TextAfter.Caption = "Text After";
		this.gc_TextAfter.CurrencyDateField = null;
		this.gc_TextAfter.CurrencyField = null;
		this.gc_TextAfter.CurrencyFixed = null;
		this.gc_TextAfter.CurrencyFixedRate = 0.0;
		this.gc_TextAfter.CurrencyRateField = null;
		this.gc_TextAfter.FieldName = "textAfter";
		this.gc_TextAfter.Name = "gc_TextAfter";
		this.gc_TextAfter.PrimaryKey = false;
		this.gc_TextAfter.UnitField = null;
		this.gc_TextAfter.Visible = true;
		this.gc_TextAfter.VisibleIndex = 8;
		this.gc_Color.Caption = "Color";
		this.gc_Color.ColumnEdit = this.repositoryItemColorPickEdit1;
		this.gc_Color.CurrencyDateField = null;
		this.gc_Color.CurrencyField = null;
		this.gc_Color.CurrencyFixed = null;
		this.gc_Color.CurrencyFixedRate = 0.0;
		this.gc_Color.CurrencyRateField = null;
		this.gc_Color.FieldName = "color";
		this.gc_Color.Name = "gc_Color";
		this.gc_Color.PrimaryKey = false;
		this.gc_Color.UnitField = null;
		this.gc_Color.Visible = true;
		this.gc_Color.VisibleIndex = 9;
		this.repositoryItemColorPickEdit1.AutoHeight = false;
		this.repositoryItemColorPickEdit1.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit1.Name = "repositoryItemColorPickEdit1";
		this.repositoryItemColorPickEdit1.StoreColorAsInteger = true;
		this.gc_AlignItems.Caption = "Align Item";
		this.gc_AlignItems.ColumnEdit = this.luk_AlignItem;
		this.gc_AlignItems.CurrencyDateField = null;
		this.gc_AlignItems.CurrencyField = null;
		this.gc_AlignItems.CurrencyFixed = null;
		this.gc_AlignItems.CurrencyFixedRate = 0.0;
		this.gc_AlignItems.CurrencyRateField = null;
		this.gc_AlignItems.FieldName = "alignItems";
		this.gc_AlignItems.Name = "gc_AlignItems";
		this.gc_AlignItems.PrimaryKey = false;
		this.gc_AlignItems.UnitField = null;
		this.gc_AlignItems.Visible = true;
		this.gc_AlignItems.VisibleIndex = 10;
		this.luk_AlignItem.AutoHeight = false;
		this.luk_AlignItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_AlignItem.Name = "luk_AlignItem";
		this.luk_AlignItem.NullText = "";
		this.gc_JustifyContent.Caption = "Justify Content";
		this.gc_JustifyContent.ColumnEdit = this.luk_JustifyContent;
		this.gc_JustifyContent.CurrencyDateField = null;
		this.gc_JustifyContent.CurrencyField = null;
		this.gc_JustifyContent.CurrencyFixed = null;
		this.gc_JustifyContent.CurrencyFixedRate = 0.0;
		this.gc_JustifyContent.CurrencyRateField = null;
		this.gc_JustifyContent.FieldName = "justifyContent";
		this.gc_JustifyContent.Name = "gc_JustifyContent";
		this.gc_JustifyContent.PrimaryKey = false;
		this.gc_JustifyContent.UnitField = null;
		this.gc_JustifyContent.Visible = true;
		this.gc_JustifyContent.VisibleIndex = 11;
		this.luk_JustifyContent.AutoHeight = false;
		this.luk_JustifyContent.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_JustifyContent.Name = "luk_JustifyContent";
		this.luk_JustifyContent.NullText = "";
		this.gc_FontSize.Caption = "Font Size";
		this.gc_FontSize.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_FontSize.CurrencyDateField = null;
		this.gc_FontSize.CurrencyField = null;
		this.gc_FontSize.CurrencyFixed = null;
		this.gc_FontSize.CurrencyFixedRate = 0.0;
		this.gc_FontSize.CurrencyRateField = null;
		this.gc_FontSize.FieldName = "fontSize";
		this.gc_FontSize.Name = "gc_FontSize";
		this.gc_FontSize.PrimaryKey = false;
		this.gc_FontSize.UnitField = null;
		this.gc_FontSize.Visible = true;
		this.gc_FontSize.VisibleIndex = 12;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc_FontWeight.Caption = "Font Weight";
		this.gc_FontWeight.ColumnEdit = this.luk_FontWeight;
		this.gc_FontWeight.CurrencyDateField = null;
		this.gc_FontWeight.CurrencyField = null;
		this.gc_FontWeight.CurrencyFixed = null;
		this.gc_FontWeight.CurrencyFixedRate = 0.0;
		this.gc_FontWeight.CurrencyRateField = null;
		this.gc_FontWeight.FieldName = "fontWeight";
		this.gc_FontWeight.Name = "gc_FontWeight";
		this.gc_FontWeight.PrimaryKey = false;
		this.gc_FontWeight.UnitField = null;
		this.gc_FontWeight.Visible = true;
		this.gc_FontWeight.VisibleIndex = 13;
		this.luk_FontWeight.AutoHeight = false;
		this.luk_FontWeight.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FontWeight.Name = "luk_FontWeight";
		this.luk_FontWeight.NullText = "";
		this.gc_PaddingTop.Caption = "Padding Top";
		this.gc_PaddingTop.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gc_PaddingTop.CurrencyDateField = null;
		this.gc_PaddingTop.CurrencyField = null;
		this.gc_PaddingTop.CurrencyFixed = null;
		this.gc_PaddingTop.CurrencyFixedRate = 0.0;
		this.gc_PaddingTop.CurrencyRateField = null;
		this.gc_PaddingTop.FieldName = "paddingTop";
		this.gc_PaddingTop.Name = "gc_PaddingTop";
		this.gc_PaddingTop.PrimaryKey = false;
		this.gc_PaddingTop.UnitField = null;
		this.gc_PaddingTop.Visible = true;
		this.gc_PaddingTop.VisibleIndex = 14;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.gc_PaddingBottom.Caption = "Padding Bottom";
		this.gc_PaddingBottom.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gc_PaddingBottom.CurrencyDateField = null;
		this.gc_PaddingBottom.CurrencyField = null;
		this.gc_PaddingBottom.CurrencyFixed = null;
		this.gc_PaddingBottom.CurrencyFixedRate = 0.0;
		this.gc_PaddingBottom.CurrencyRateField = null;
		this.gc_PaddingBottom.FieldName = "paddingBottom";
		this.gc_PaddingBottom.Name = "gc_PaddingBottom";
		this.gc_PaddingBottom.PrimaryKey = false;
		this.gc_PaddingBottom.UnitField = null;
		this.gc_PaddingBottom.Visible = true;
		this.gc_PaddingBottom.VisibleIndex = 15;
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.gc_PaddingLeft.Caption = "Padding Left";
		this.gc_PaddingLeft.ColumnEdit = this.repositoryItemCalcEdit4;
		this.gc_PaddingLeft.CurrencyDateField = null;
		this.gc_PaddingLeft.CurrencyField = null;
		this.gc_PaddingLeft.CurrencyFixed = null;
		this.gc_PaddingLeft.CurrencyFixedRate = 0.0;
		this.gc_PaddingLeft.CurrencyRateField = null;
		this.gc_PaddingLeft.FieldName = "paddingLeft";
		this.gc_PaddingLeft.Name = "gc_PaddingLeft";
		this.gc_PaddingLeft.PrimaryKey = false;
		this.gc_PaddingLeft.UnitField = null;
		this.gc_PaddingLeft.Visible = true;
		this.gc_PaddingLeft.VisibleIndex = 16;
		this.repositoryItemCalcEdit4.AutoHeight = false;
		this.repositoryItemCalcEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit4.Name = "repositoryItemCalcEdit4";
		this.gc_PaddingRight.Caption = "Padding Right";
		this.gc_PaddingRight.ColumnEdit = this.repositoryItemCalcEdit5;
		this.gc_PaddingRight.CurrencyDateField = null;
		this.gc_PaddingRight.CurrencyField = null;
		this.gc_PaddingRight.CurrencyFixed = null;
		this.gc_PaddingRight.CurrencyFixedRate = 0.0;
		this.gc_PaddingRight.CurrencyRateField = null;
		this.gc_PaddingRight.FieldName = "paddingRight";
		this.gc_PaddingRight.Name = "gc_PaddingRight";
		this.gc_PaddingRight.PrimaryKey = false;
		this.gc_PaddingRight.UnitField = null;
		this.gc_PaddingRight.Visible = true;
		this.gc_PaddingRight.VisibleIndex = 17;
		this.repositoryItemCalcEdit5.AutoHeight = false;
		this.repositoryItemCalcEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit5.Name = "repositoryItemCalcEdit5";
		this.gc_TextAlign.Caption = "Text Align";
		this.gc_TextAlign.ColumnEdit = this.luk_TextAlign;
		this.gc_TextAlign.CurrencyDateField = null;
		this.gc_TextAlign.CurrencyField = null;
		this.gc_TextAlign.CurrencyFixed = null;
		this.gc_TextAlign.CurrencyFixedRate = 0.0;
		this.gc_TextAlign.CurrencyRateField = null;
		this.gc_TextAlign.FieldName = "textAlign";
		this.gc_TextAlign.Name = "gc_TextAlign";
		this.gc_TextAlign.PrimaryKey = false;
		this.gc_TextAlign.UnitField = null;
		this.gc_TextAlign.Visible = true;
		this.gc_TextAlign.VisibleIndex = 18;
		this.luk_TextAlign.AutoHeight = false;
		this.luk_TextAlign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TextAlign.Name = "luk_TextAlign";
		this.luk_TextAlign.NullText = "";
		this.gc_AlignSelf.Caption = "Align Self";
		this.gc_AlignSelf.ColumnEdit = this.luk_AlignSelf;
		this.gc_AlignSelf.CurrencyDateField = null;
		this.gc_AlignSelf.CurrencyField = null;
		this.gc_AlignSelf.CurrencyFixed = null;
		this.gc_AlignSelf.CurrencyFixedRate = 0.0;
		this.gc_AlignSelf.CurrencyRateField = null;
		this.gc_AlignSelf.FieldName = "alignSelf";
		this.gc_AlignSelf.Name = "gc_AlignSelf";
		this.gc_AlignSelf.PrimaryKey = false;
		this.gc_AlignSelf.UnitField = null;
		this.gc_AlignSelf.Visible = true;
		this.gc_AlignSelf.VisibleIndex = 19;
		this.luk_AlignSelf.AutoHeight = false;
		this.luk_AlignSelf.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_AlignSelf.Name = "luk_AlignSelf";
		this.luk_AlignSelf.NullText = "";
		this.gc_Left.Caption = "Left";
		this.gc_Left.CurrencyDateField = null;
		this.gc_Left.CurrencyField = null;
		this.gc_Left.CurrencyFixed = null;
		this.gc_Left.CurrencyFixedRate = 0.0;
		this.gc_Left.CurrencyRateField = null;
		this.gc_Left.FieldName = "LeftIndex";
		this.gc_Left.Name = "gc_Left";
		this.gc_Left.OptionsColumn.ReadOnly = true;
		this.gc_Left.PrimaryKey = false;
		this.gc_Left.UnitField = null;
		this.gc_Top.Caption = "Top";
		this.gc_Top.CurrencyDateField = null;
		this.gc_Top.CurrencyField = null;
		this.gc_Top.CurrencyFixed = null;
		this.gc_Top.CurrencyFixedRate = 0.0;
		this.gc_Top.CurrencyRateField = null;
		this.gc_Top.FieldName = "TopIndex";
		this.gc_Top.Name = "gc_Top";
		this.gc_Top.OptionsColumn.ReadOnly = true;
		this.gc_Top.PrimaryKey = false;
		this.gc_Top.UnitField = null;
		this.gc_Right.Caption = "Right";
		this.gc_Right.CurrencyDateField = null;
		this.gc_Right.CurrencyField = null;
		this.gc_Right.CurrencyFixed = null;
		this.gc_Right.CurrencyFixedRate = 0.0;
		this.gc_Right.CurrencyRateField = null;
		this.gc_Right.FieldName = "RightIndex";
		this.gc_Right.Name = "gc_Right";
		this.gc_Right.OptionsColumn.ReadOnly = true;
		this.gc_Right.PrimaryKey = false;
		this.gc_Right.UnitField = null;
		this.gc_Bottom.Caption = "Bottom";
		this.gc_Bottom.CurrencyDateField = null;
		this.gc_Bottom.CurrencyField = null;
		this.gc_Bottom.CurrencyFixed = null;
		this.gc_Bottom.CurrencyFixedRate = 0.0;
		this.gc_Bottom.CurrencyRateField = null;
		this.gc_Bottom.FieldName = "BottomIndex";
		this.gc_Bottom.Name = "gc_Bottom";
		this.gc_Bottom.OptionsColumn.ReadOnly = true;
		this.gc_Bottom.PrimaryKey = false;
		this.gc_Bottom.UnitField = null;
		this.gc_DateTimeValueFormat.Caption = "Date time value format";
		this.gc_DateTimeValueFormat.CurrencyDateField = null;
		this.gc_DateTimeValueFormat.CurrencyField = null;
		this.gc_DateTimeValueFormat.CurrencyFixed = null;
		this.gc_DateTimeValueFormat.CurrencyFixedRate = 0.0;
		this.gc_DateTimeValueFormat.CurrencyRateField = null;
		this.gc_DateTimeValueFormat.FieldName = "DateTimeValueFormat";
		this.gc_DateTimeValueFormat.Name = "gc_DateTimeValueFormat";
		this.gc_DateTimeValueFormat.PrimaryKey = false;
		this.gc_DateTimeValueFormat.UnitField = null;
		this.gc_DateTimeValueFormat.Visible = true;
		this.gc_DateTimeValueFormat.VisibleIndex = 20;
		this.gc_DateTimeDisplayFormat.Caption = "Date time display format";
		this.gc_DateTimeDisplayFormat.CurrencyDateField = null;
		this.gc_DateTimeDisplayFormat.CurrencyField = null;
		this.gc_DateTimeDisplayFormat.CurrencyFixed = null;
		this.gc_DateTimeDisplayFormat.CurrencyFixedRate = 0.0;
		this.gc_DateTimeDisplayFormat.CurrencyRateField = null;
		this.gc_DateTimeDisplayFormat.FieldName = "DateTimeDisplayFormat";
		this.gc_DateTimeDisplayFormat.Name = "gc_DateTimeDisplayFormat";
		this.gc_DateTimeDisplayFormat.PrimaryKey = false;
		this.gc_DateTimeDisplayFormat.UnitField = null;
		this.gc_DateTimeDisplayFormat.Visible = true;
		this.gc_DateTimeDisplayFormat.VisibleIndex = 21;
		this.gc_currencyField.Caption = "Currency Field";
		this.gc_currencyField.ColumnEdit = this.luk_currencyField;
		this.gc_currencyField.CurrencyDateField = null;
		this.gc_currencyField.CurrencyField = null;
		this.gc_currencyField.CurrencyFixed = null;
		this.gc_currencyField.CurrencyFixedRate = 0.0;
		this.gc_currencyField.CurrencyRateField = null;
		this.gc_currencyField.FieldName = "currencyField";
		this.gc_currencyField.Name = "gc_currencyField";
		this.gc_currencyField.PrimaryKey = false;
		this.gc_currencyField.UnitField = null;
		this.gc_currencyField.Visible = true;
		this.gc_currencyField.VisibleIndex = 22;
		this.luk_currencyField.AutoHeight = false;
		this.luk_currencyField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_currencyField.DisplayMember = "Name";
		this.luk_currencyField.Name = "luk_currencyField";
		this.luk_currencyField.NullText = "";
		this.luk_currencyField.PopupView = this.gridView2;
		this.luk_currencyField.ValueMember = "Code";
		this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView2.Name = "gridView2";
		this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView2.OptionsView.ShowGroupPanel = false;
		this.xtraTabPageEx3.Controls.Add(this.grd_Header);
		this.xtraTabPageEx3.EditControl = null;
		this.xtraTabPageEx3.EditValue = "Header";
		this.xtraTabPageEx3.FieldName = null;
		this.xtraTabPageEx3.IsSearchControl = false;
		this.xtraTabPageEx3.Name = "xtraTabPageEx3";
		this.xtraTabPageEx3.ReadOnly = false;
		this.xtraTabPageEx3.SettingInVisible = false;
		this.xtraTabPageEx3.SettingReadonly = false;
		this.xtraTabPageEx3.Size = new System.Drawing.Size(1005, 362);
		this.xtraTabPageEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx3.Text = "Header";
		this.grd_Header.AllowPasteExcel = false;
		this.grd_Header.ClipboardData = null;
		this.grd_Header.ContextMenuStrip = this.contextMenuStrip_Header;
		this.grd_Header.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Header.Location = new System.Drawing.Point(0, 0);
		this.grd_Header.MainView = this.gv_Header;
		this.grd_Header.MenuManager = this.barManager1;
		this.grd_Header.Name = "grd_Header";
		this.grd_Header.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.luk_HeaderFieldName, this.luk_HeaderDataType, this.repositoryItemCalcEdit8 });
		this.grd_Header.Size = new System.Drawing.Size(1005, 362);
		this.grd_Header.TabIndex = 0;
		this.grd_Header.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Header });
		this.contextMenuStrip_Header.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem1, this.deleteToolStripMenuItem, this.refreshToolStripMenuItem1 });
		this.contextMenuStrip_Header.Name = "contextMenuStrip_Header";
		this.contextMenuStrip_Header.Size = new System.Drawing.Size(118, 70);
		this.addNewToolStripMenuItem1.Name = "addNewToolStripMenuItem1";
		this.addNewToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.addNewToolStripMenuItem1.Text = "Add";
		this.addNewToolStripMenuItem1.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.refreshToolStripMenuItem1.Name = "refreshToolStripMenuItem1";
		this.refreshToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.refreshToolStripMenuItem1.Text = "Refresh";
		this.refreshToolStripMenuItem1.Click += new System.EventHandler(this.refreshToolStripMenuItem1_Click);
		this.gv_Header.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Header.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Header.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Header.ColumnKey = null;
		this.gv_Header.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_HeaderOrderId, this.gc_HeaderFieldName, this.gc_HeaderDataType, this.gc_HeaderDisplayText });
		this.gv_Header.DisableFormSetting = false;
		this.gv_Header.DisplayException = false;
		this.gv_Header.EditColumn = null;
		this.gv_Header.FieldCurrency = null;
		this.gv_Header.FieldRate = null;
		this.gv_Header.FormId = null;
		this.gv_Header.GridControl = this.grd_Header;
		this.gv_Header.HasFormSetting = false;
		this.gv_Header.IgnoreFilter = false;
		this.gv_Header.IndexEditRow = -1;
		this.gv_Header.IndicatorWidth = 35;
		this.gv_Header.IsApzTable = false;
		this.gv_Header.IsInsertNewRow = false;
		this.gv_Header.IsLoadingData = false;
		this.gv_Header.IsMappingContextAction = false;
		this.gv_Header.ItemId = null;
		this.gv_Header.LstColumnAllowBatch = null;
		this.gv_Header.LstColumnNotAllowBatch = null;
		this.gv_Header.LstColumnNotEditable = null;
		this.gv_Header.LstColumNotSetting = null;
		this.gv_Header.LstOldUDF = null;
		this.gv_Header.Name = "gv_Header";
		this.gv_Header.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Header.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Header.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Header.OptionsPrint.AutoWidth = false;
		this.gv_Header.OptionsPrint.UsePrintStyles = false;
		this.gv_Header.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Header.OptionsView.ColumnAutoWidth = false;
		this.gv_Header.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Header.OptionsView.ShowGroupedColumns = true;
		this.gv_Header.OptionsView.ShowGroupPanel = false;
		this.gv_Header.ReadOnly = false;
		this.gv_Header.SettingReadonly = false;
		this.gv_Header.UseBatchMod = false;
		this.gv_Header.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Header_CellValueChanged);
		this.gc_HeaderOrderId.Caption = "Order Id";
		this.gc_HeaderOrderId.ColumnEdit = this.repositoryItemCalcEdit8;
		this.gc_HeaderOrderId.CurrencyDateField = null;
		this.gc_HeaderOrderId.CurrencyField = null;
		this.gc_HeaderOrderId.CurrencyFixed = null;
		this.gc_HeaderOrderId.CurrencyFixedRate = 0.0;
		this.gc_HeaderOrderId.CurrencyRateField = null;
		this.gc_HeaderOrderId.FieldName = "OrderId";
		this.gc_HeaderOrderId.Name = "gc_HeaderOrderId";
		this.gc_HeaderOrderId.PrimaryKey = false;
		this.gc_HeaderOrderId.UnitField = null;
		this.gc_HeaderOrderId.Visible = true;
		this.gc_HeaderOrderId.VisibleIndex = 0;
		this.repositoryItemCalcEdit8.AutoHeight = false;
		this.repositoryItemCalcEdit8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit8.Name = "repositoryItemCalcEdit8";
		this.gc_HeaderFieldName.Caption = "Field Name";
		this.gc_HeaderFieldName.ColumnEdit = this.luk_HeaderFieldName;
		this.gc_HeaderFieldName.CurrencyDateField = null;
		this.gc_HeaderFieldName.CurrencyField = null;
		this.gc_HeaderFieldName.CurrencyFixed = null;
		this.gc_HeaderFieldName.CurrencyFixedRate = 0.0;
		this.gc_HeaderFieldName.CurrencyRateField = null;
		this.gc_HeaderFieldName.FieldName = "FieldName";
		this.gc_HeaderFieldName.Name = "gc_HeaderFieldName";
		this.gc_HeaderFieldName.PrimaryKey = false;
		this.gc_HeaderFieldName.UnitField = null;
		this.gc_HeaderFieldName.Visible = true;
		this.gc_HeaderFieldName.VisibleIndex = 1;
		this.luk_HeaderFieldName.AutoHeight = false;
		this.luk_HeaderFieldName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HeaderFieldName.Name = "luk_HeaderFieldName";
		this.luk_HeaderFieldName.NullText = "";
		this.luk_HeaderFieldName.PopupView = this.gridView1;
		this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gc_HeaderDataType.Caption = "Data Type";
		this.gc_HeaderDataType.ColumnEdit = this.luk_HeaderDataType;
		this.gc_HeaderDataType.CurrencyDateField = null;
		this.gc_HeaderDataType.CurrencyField = null;
		this.gc_HeaderDataType.CurrencyFixed = null;
		this.gc_HeaderDataType.CurrencyFixedRate = 0.0;
		this.gc_HeaderDataType.CurrencyRateField = null;
		this.gc_HeaderDataType.FieldName = "DataType";
		this.gc_HeaderDataType.Name = "gc_HeaderDataType";
		this.gc_HeaderDataType.PrimaryKey = false;
		this.gc_HeaderDataType.UnitField = null;
		this.gc_HeaderDataType.Visible = true;
		this.gc_HeaderDataType.VisibleIndex = 2;
		this.luk_HeaderDataType.AutoHeight = false;
		this.luk_HeaderDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HeaderDataType.Name = "luk_HeaderDataType";
		this.luk_HeaderDataType.NullText = "";
		this.gc_HeaderDisplayText.Caption = "Display Text";
		this.gc_HeaderDisplayText.CurrencyDateField = null;
		this.gc_HeaderDisplayText.CurrencyField = null;
		this.gc_HeaderDisplayText.CurrencyFixed = null;
		this.gc_HeaderDisplayText.CurrencyFixedRate = 0.0;
		this.gc_HeaderDisplayText.CurrencyRateField = null;
		this.gc_HeaderDisplayText.FieldName = "DisplayText";
		this.gc_HeaderDisplayText.Name = "gc_HeaderDisplayText";
		this.gc_HeaderDisplayText.PrimaryKey = false;
		this.gc_HeaderDisplayText.UnitField = null;
		this.gc_HeaderDisplayText.Visible = true;
		this.gc_HeaderDisplayText.VisibleIndex = 3;
		this.xtraTabPageEx2.Controls.Add(this.grd_Chart);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Chart";
		this.xtraTabPageEx2.FieldName = "";
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(1005, 362);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Chart";
		this.grd_Chart.AllowPasteExcel = false;
		this.grd_Chart.ClipboardData = null;
		this.grd_Chart.ContextMenuStrip = this.contextMenuStrip_Chart;
		this.grd_Chart.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Chart.Location = new System.Drawing.Point(0, 0);
		this.grd_Chart.MainView = this.gv_Chart;
		this.grd_Chart.MenuManager = this.barManager1;
		this.grd_Chart.Name = "grd_Chart";
		this.grd_Chart.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemCalcEdit6, this.luk_ChartFieldName, this.luk_DataType, this.check_ChartVisible, this.repositoryItemColorPickEdit3, this.check_IsKey, this.repositoryItemSearchLookUpEdit1, this.luk_ColumnType });
		this.grd_Chart.Size = new System.Drawing.Size(1005, 362);
		this.grd_Chart.TabIndex = 0;
		this.grd_Chart.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Chart });
		this.gv_Chart.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Chart.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Chart.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Chart.ColumnKey = null;
		this.gv_Chart.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_OrderId, this.gc_FieldNameChart, this.gc_DisplayText, this.gc_ColumnType, this.gc_ChartColor });
		this.gv_Chart.DisableFormSetting = false;
		this.gv_Chart.DisplayException = false;
		this.gv_Chart.EditColumn = null;
		this.gv_Chart.FieldCurrency = null;
		this.gv_Chart.FieldRate = null;
		this.gv_Chart.FormId = null;
		this.gv_Chart.GridControl = this.grd_Chart;
		this.gv_Chart.HasFormSetting = false;
		this.gv_Chart.IgnoreFilter = false;
		this.gv_Chart.IndexEditRow = -1;
		this.gv_Chart.IndicatorWidth = 35;
		this.gv_Chart.IsApzTable = false;
		this.gv_Chart.IsInsertNewRow = false;
		this.gv_Chart.IsLoadingData = false;
		this.gv_Chart.IsMappingContextAction = false;
		this.gv_Chart.ItemId = null;
		this.gv_Chart.LstColumnAllowBatch = null;
		this.gv_Chart.LstColumnNotAllowBatch = null;
		this.gv_Chart.LstColumnNotEditable = null;
		this.gv_Chart.LstColumNotSetting = null;
		this.gv_Chart.LstOldUDF = null;
		this.gv_Chart.Name = "gv_Chart";
		this.gv_Chart.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Chart.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Chart.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Chart.OptionsPrint.AutoWidth = false;
		this.gv_Chart.OptionsPrint.UsePrintStyles = false;
		this.gv_Chart.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Chart.OptionsView.ColumnAutoWidth = false;
		this.gv_Chart.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Chart.OptionsView.ShowGroupedColumns = true;
		this.gv_Chart.OptionsView.ShowGroupPanel = false;
		this.gv_Chart.ReadOnly = false;
		this.gv_Chart.SettingReadonly = false;
		this.gv_Chart.UseBatchMod = false;
		this.gv_Chart.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Chart_CellValueChanged);
		this.gc_OrderId.Caption = "Order Id";
		this.gc_OrderId.ColumnEdit = this.repositoryItemCalcEdit6;
		this.gc_OrderId.CurrencyDateField = null;
		this.gc_OrderId.CurrencyField = null;
		this.gc_OrderId.CurrencyFixed = null;
		this.gc_OrderId.CurrencyFixedRate = 0.0;
		this.gc_OrderId.CurrencyRateField = null;
		this.gc_OrderId.FieldName = "OrderId";
		this.gc_OrderId.Name = "gc_OrderId";
		this.gc_OrderId.PrimaryKey = false;
		this.gc_OrderId.UnitField = null;
		this.gc_OrderId.Visible = true;
		this.gc_OrderId.VisibleIndex = 0;
		this.repositoryItemCalcEdit6.AutoHeight = false;
		this.repositoryItemCalcEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit6.Name = "repositoryItemCalcEdit6";
		this.gc_FieldNameChart.Caption = "Field Name";
		this.gc_FieldNameChart.ColumnEdit = this.luk_ChartFieldName;
		this.gc_FieldNameChart.CurrencyDateField = null;
		this.gc_FieldNameChart.CurrencyField = null;
		this.gc_FieldNameChart.CurrencyFixed = null;
		this.gc_FieldNameChart.CurrencyFixedRate = 0.0;
		this.gc_FieldNameChart.CurrencyRateField = null;
		this.gc_FieldNameChart.FieldName = "FieldName";
		this.gc_FieldNameChart.Name = "gc_FieldNameChart";
		this.gc_FieldNameChart.PrimaryKey = false;
		this.gc_FieldNameChart.UnitField = null;
		this.gc_FieldNameChart.Visible = true;
		this.gc_FieldNameChart.VisibleIndex = 1;
		this.luk_ChartFieldName.AutoHeight = false;
		this.luk_ChartFieldName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ChartFieldName.DisplayMember = "Name";
		this.luk_ChartFieldName.Name = "luk_ChartFieldName";
		this.luk_ChartFieldName.NullText = "";
		this.luk_ChartFieldName.ValueMember = "Code";
		this.gc_DisplayText.Caption = "Display Text";
		this.gc_DisplayText.CurrencyDateField = null;
		this.gc_DisplayText.CurrencyField = null;
		this.gc_DisplayText.CurrencyFixed = null;
		this.gc_DisplayText.CurrencyFixedRate = 0.0;
		this.gc_DisplayText.CurrencyRateField = null;
		this.gc_DisplayText.FieldName = "DisplayText";
		this.gc_DisplayText.Name = "gc_DisplayText";
		this.gc_DisplayText.PrimaryKey = false;
		this.gc_DisplayText.UnitField = null;
		this.gc_DisplayText.Visible = true;
		this.gc_DisplayText.VisibleIndex = 2;
		this.gc_ColumnType.Caption = "Column Type";
		this.gc_ColumnType.ColumnEdit = this.luk_ColumnType;
		this.gc_ColumnType.CurrencyDateField = null;
		this.gc_ColumnType.CurrencyField = null;
		this.gc_ColumnType.CurrencyFixed = null;
		this.gc_ColumnType.CurrencyFixedRate = 0.0;
		this.gc_ColumnType.CurrencyRateField = null;
		this.gc_ColumnType.FieldName = "ColumnType";
		this.gc_ColumnType.Name = "gc_ColumnType";
		this.gc_ColumnType.PrimaryKey = false;
		this.gc_ColumnType.UnitField = null;
		this.gc_ColumnType.Visible = true;
		this.gc_ColumnType.VisibleIndex = 3;
		this.luk_ColumnType.AutoHeight = false;
		this.luk_ColumnType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ColumnType.Name = "luk_ColumnType";
		this.luk_ColumnType.NullText = "";
		this.gc_ChartColor.Caption = "Color";
		this.gc_ChartColor.ColumnEdit = this.repositoryItemColorPickEdit3;
		this.gc_ChartColor.CurrencyDateField = null;
		this.gc_ChartColor.CurrencyField = null;
		this.gc_ChartColor.CurrencyFixed = null;
		this.gc_ChartColor.CurrencyFixedRate = 0.0;
		this.gc_ChartColor.CurrencyRateField = null;
		this.gc_ChartColor.FieldName = "Color";
		this.gc_ChartColor.Name = "gc_ChartColor";
		this.gc_ChartColor.PrimaryKey = false;
		this.gc_ChartColor.UnitField = null;
		this.gc_ChartColor.Visible = true;
		this.gc_ChartColor.VisibleIndex = 4;
		this.repositoryItemColorPickEdit3.AutoHeight = false;
		this.repositoryItemColorPickEdit3.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit3.Name = "repositoryItemColorPickEdit3";
		this.repositoryItemColorPickEdit3.StoreColorAsInteger = true;
		this.luk_DataType.AutoHeight = false;
		this.luk_DataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DataType.Name = "luk_DataType";
		this.luk_DataType.NullText = "";
		this.check_ChartVisible.AutoHeight = false;
		this.check_ChartVisible.Name = "check_ChartVisible";
		this.check_ChartVisible.ValueChecked = "Y";
		this.check_ChartVisible.ValueUnchecked = "N";
		this.check_IsKey.AutoHeight = false;
		this.check_IsKey.Name = "check_IsKey";
		this.check_IsKey.ValueChecked = "Y";
		this.check_IsKey.ValueUnchecked = "N";
		this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
		this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
		this.repositoryItemSearchLookUpEdit1.PopupView = this.gridView3;
		this.gridView3.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView3.Name = "gridView3";
		this.gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView3.OptionsView.ShowGroupPanel = false;
		this.tabPage_dashboard.Controls.Add(this.txt_DashboardName);
		this.tabPage_dashboard.Controls.Add(this.grd_dashboard);
		this.tabPage_dashboard.Controls.Add(this.btn_EditDesign);
		this.tabPage_dashboard.EditControl = null;
		this.tabPage_dashboard.EditValue = "Dashboard";
		this.tabPage_dashboard.FieldName = "";
		this.tabPage_dashboard.IsSearchControl = false;
		this.tabPage_dashboard.Name = "tabPage_dashboard";
		this.tabPage_dashboard.ReadOnly = false;
		this.tabPage_dashboard.SettingInVisible = false;
		this.tabPage_dashboard.SettingReadonly = false;
		this.tabPage_dashboard.Size = new System.Drawing.Size(1005, 362);
		this.tabPage_dashboard.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_dashboard.Text = "Dashboard";
		this.txt_DashboardName.EditControl = null;
		this.txt_DashboardName.FieldName = null;
		this.txt_DashboardName.IsSearchControl = false;
		this.txt_DashboardName.Location = new System.Drawing.Point(11, 5);
		this.txt_DashboardName.MenuManager = this.barManager1;
		this.txt_DashboardName.Name = "txt_DashboardName";
		this.txt_DashboardName.SettingInVisible = false;
		this.txt_DashboardName.SettingReadonly = false;
		this.txt_DashboardName.Size = new System.Drawing.Size(140, 20);
		this.txt_DashboardName.TabIndex = 3;
		this.grd_dashboard.AllowPasteExcel = false;
		this.grd_dashboard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_dashboard.ClipboardData = null;
		this.grd_dashboard.ContextMenuStrip = this.contextMenuStrip_Dashboard;
		this.grd_dashboard.Location = new System.Drawing.Point(-1, 32);
		this.grd_dashboard.MainView = this.gv_dashboard;
		this.grd_dashboard.MenuManager = this.barManager1;
		this.grd_dashboard.Name = "grd_dashboard";
		this.grd_dashboard.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemMemoExEdit2, this.btn_detailDashboard });
		this.grd_dashboard.Size = new System.Drawing.Size(1007, 331);
		this.grd_dashboard.TabIndex = 2;
		this.grd_dashboard.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_dashboard });
		this.contextMenuStrip_Dashboard.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem1 });
		this.contextMenuStrip_Dashboard.Name = "contextMenuStrip_Dashboard";
		this.contextMenuStrip_Dashboard.Size = new System.Drawing.Size(118, 26);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.deleteToolStripMenuItem1.Text = "Remove";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.gv_dashboard.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_dashboard.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_dashboard.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_dashboard.ColumnKey = null;
		this.gv_dashboard.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_dashboardName, this.gc_XMLLayout, this.gridColumnEx1 });
		this.gv_dashboard.DisableFormSetting = false;
		this.gv_dashboard.DisplayException = false;
		this.gv_dashboard.EditColumn = null;
		this.gv_dashboard.FieldCurrency = null;
		this.gv_dashboard.FieldRate = null;
		this.gv_dashboard.FormId = null;
		this.gv_dashboard.GridControl = this.grd_dashboard;
		this.gv_dashboard.HasFormSetting = false;
		this.gv_dashboard.IgnoreFilter = false;
		this.gv_dashboard.IndexEditRow = 0;
		this.gv_dashboard.IndicatorWidth = 35;
		this.gv_dashboard.IsApzTable = false;
		this.gv_dashboard.IsInsertNewRow = false;
		this.gv_dashboard.IsLoadingData = false;
		this.gv_dashboard.IsMappingContextAction = false;
		this.gv_dashboard.ItemId = null;
		this.gv_dashboard.LstColumnAllowBatch = null;
		this.gv_dashboard.LstColumnNotAllowBatch = null;
		this.gv_dashboard.LstColumnNotEditable = null;
		this.gv_dashboard.LstColumNotSetting = null;
		this.gv_dashboard.LstOldUDF = null;
		this.gv_dashboard.Name = "gv_dashboard";
		this.gv_dashboard.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_dashboard.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_dashboard.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_dashboard.OptionsPrint.AutoWidth = false;
		this.gv_dashboard.OptionsPrint.UsePrintStyles = false;
		this.gv_dashboard.OptionsView.BestFitMaxRowCount = 100;
		this.gv_dashboard.OptionsView.ColumnAutoWidth = false;
		this.gv_dashboard.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_dashboard.OptionsView.ShowGroupedColumns = true;
		this.gv_dashboard.OptionsView.ShowGroupPanel = false;
		this.gv_dashboard.ReadOnly = false;
		this.gv_dashboard.SettingReadonly = false;
		this.gv_dashboard.UseBatchMod = false;
		this.gv_dashboard.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_dashboard_CellValueChanged);
		this.gc_dashboardName.Caption = "Dashboard Name";
		this.gc_dashboardName.CurrencyDateField = null;
		this.gc_dashboardName.CurrencyField = null;
		this.gc_dashboardName.CurrencyFixed = null;
		this.gc_dashboardName.CurrencyFixedRate = 0.0;
		this.gc_dashboardName.CurrencyRateField = null;
		this.gc_dashboardName.FieldName = "DashboardName";
		this.gc_dashboardName.Name = "gc_dashboardName";
		this.gc_dashboardName.PrimaryKey = false;
		this.gc_dashboardName.UnitField = null;
		this.gc_dashboardName.Visible = true;
		this.gc_dashboardName.VisibleIndex = 0;
		this.gc_XMLLayout.Caption = "XML Layout";
		this.gc_XMLLayout.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gc_XMLLayout.CurrencyDateField = null;
		this.gc_XMLLayout.CurrencyField = null;
		this.gc_XMLLayout.CurrencyFixed = null;
		this.gc_XMLLayout.CurrencyFixedRate = 0.0;
		this.gc_XMLLayout.CurrencyRateField = null;
		this.gc_XMLLayout.FieldName = "Layout";
		this.gc_XMLLayout.Name = "gc_XMLLayout";
		this.gc_XMLLayout.PrimaryKey = false;
		this.gc_XMLLayout.UnitField = null;
		this.gc_XMLLayout.Visible = true;
		this.gc_XMLLayout.VisibleIndex = 1;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.gridColumnEx1.ColumnEdit = this.btn_detailDashboard;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.btn_detailDashboard.AutoHeight = false;
		this.btn_detailDashboard.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_detailDashboard.Name = "btn_detailDashboard";
		this.btn_detailDashboard.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.btn_EditDesign.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_EditDesign.EditControl = null;
		this.btn_EditDesign.EditValue = "Create New Dashboard";
		this.btn_EditDesign.FieldName = null;
		this.btn_EditDesign.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditDesign.ImageOptions.Image");
		this.btn_EditDesign.IsSearchControl = false;
		this.btn_EditDesign.Location = new System.Drawing.Point(157, 3);
		this.btn_EditDesign.Name = "btn_EditDesign";
		this.btn_EditDesign.ReadOnly = false;
		this.btn_EditDesign.SettingInVisible = false;
		this.btn_EditDesign.SettingReadonly = false;
		this.btn_EditDesign.Size = new System.Drawing.Size(140, 23);
		this.btn_EditDesign.TabIndex = 1;
		this.btn_EditDesign.Text = "Create New Dashboard";
		this.btn_EditDesign.Click += new System.EventHandler(this.btn_EditDesign_Click);
		this.btn_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ok.EditControl = null;
		this.btn_ok.EditValue = "OK";
		this.btn_ok.FieldName = null;
		this.btn_ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ok.ImageOptions.Image");
		this.btn_ok.IsSearchControl = false;
		this.btn_ok.Location = new System.Drawing.Point(839, 602);
		this.btn_ok.Name = "btn_ok";
		this.btn_ok.ReadOnly = false;
		this.btn_ok.SettingInVisible = false;
		this.btn_ok.SettingReadonly = false;
		this.btn_ok.Size = new System.Drawing.Size(75, 23);
		this.btn_ok.TabIndex = 12;
		this.btn_ok.Text = "OK";
		this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
		this.btn_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_cancel.EditControl = null;
		this.btn_cancel.EditValue = "Cancel";
		this.btn_cancel.FieldName = null;
		this.btn_cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_cancel.ImageOptions.Image");
		this.btn_cancel.IsSearchControl = false;
		this.btn_cancel.Location = new System.Drawing.Point(920, 602);
		this.btn_cancel.Name = "btn_cancel";
		this.btn_cancel.ReadOnly = false;
		this.btn_cancel.SettingInVisible = false;
		this.btn_cancel.SettingReadonly = false;
		this.btn_cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_cancel.TabIndex = 12;
		this.btn_cancel.Text = "Cancel";
		this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
		this.clipboardBar1.BarName = "";
		this.clipboardBar1.Control = this.sc_Main;
		this.clipboardBar1.DockCol = 0;
		this.clipboardBar1.DockRow = 0;
		this.clipboardBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.clipboardBar1.FloatLocation = new System.Drawing.Point(77, 176);
		this.clipboardBar1.Text = "";
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem4);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontNameItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontSizeItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem6);
		this.spreadsheetBarController1.BarItems.Add(this.check_Bold);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem7);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem15);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem16);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem17);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem18);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem19);
		this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineStyleItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeCellFillColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem7);
		this.spreadsheetBarController1.BarItems.Add(this.check_alignLeft);
		this.spreadsheetBarController1.BarItems.Add(this.check_alignCenter);
		this.spreadsheetBarController1.BarItems.Add(this.check_alignRight);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem20);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem21);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem22);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem23);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem24);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem2);
		this.spreadsheetBarController1.BarItems.Add(this.changeNumberFormatItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem25);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem26);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem27);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem28);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem29);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem30);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem31);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem32);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem33);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem34);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem35);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem36);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem37);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem38);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem39);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem40);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem41);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem42);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem43);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem44);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem45);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem46);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem47);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem48);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem49);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem50);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem51);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem7);
		this.spreadsheetBarController1.BarItems.Add(this.galleryFormatAsTableItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeStyleItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem52);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem53);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem54);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem55);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem56);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem57);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem58);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem59);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem60);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem61);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem62);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem63);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem64);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem65);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem66);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem67);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem68);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem69);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem70);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem71);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem72);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem73);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem74);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem75);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem76);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem77);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem78);
		this.spreadsheetBarController1.BarItems.Add(this.changeSheetTabColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem79);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem80);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem81);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem82);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem83);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem84);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem85);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem86);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem87);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem88);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem89);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem90);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem91);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem92);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem93);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem94);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem95);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem96);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem97);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem98);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem99);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem15);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem100);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem101);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem102);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem103);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem104);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem105);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem106);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem16);
		this.spreadsheetBarController1.Control = this.sc_Main;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1007, 637);
		base.Controls.Add(this.btn_cancel);
		base.Controls.Add(this.btn_ok);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormId = "oSettingReport";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_SettingReport.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SettingReport";
		this.Text = "Setting Report";
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetFontSizeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPopupGalleryEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetCellStyleEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_listReport).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_listReport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_reportGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_dashboard).EndInit();
		this.tab_dashboard.ResumeLayout(false);
		this.contextMenuStrip_Chart.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_listProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_listProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FieldName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Size).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AlignItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_JustifyContent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FontWeight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TextAlign).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AlignSelf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_currencyField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		this.xtraTabPageEx3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Header).EndInit();
		this.contextMenuStrip_Header.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HeaderFieldName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HeaderDataType).EndInit();
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Chart).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Chart).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ChartFieldName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ColumnType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_ChartVisible).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_IsKey).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		this.tabPage_dashboard.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_DashboardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_dashboard).EndInit();
		this.contextMenuStrip_Dashboard.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_dashboard).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_detailDashboard).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetBarController1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
