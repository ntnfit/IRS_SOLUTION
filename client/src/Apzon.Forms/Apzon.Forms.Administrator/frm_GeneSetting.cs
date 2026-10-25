using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_GeneSetting : XtraFormEx
{
	private const string FormController = "GeneralSetting";

	private DataSet dts;

	private bool _isLoading;

	public RefreshMain RefreshMain;

	private IContainer components;

	private XtraTabControlEx xtraTab_Inventory;

	private XtraTabPageEx Tab_BP;

	private XtraTabPageEx Tab_Budget;

	private XtraTabPageEx Tab_Services;

	private XtraTabPageEx Tab_Display;

	private XtraTabPageEx Tab_Font;

	private XtraTabPageEx Tab_Path;

	private XtraTabPageEx Tab_Inventory;

	private XtraTabPageEx Tab_Resources;

	private XtraTabPageEx Tab_CashFlow;

	private XtraTabPageEx Tab_Cockpit;

	private XtraTabPageEx Tab_CostAccounting;

	private XtraTabPageEx Tab_Pricing;

	private XtraTabPageEx Tab_HideFunctions;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private LayoutControl layoutControl1;

	private CheckEditEx CrdCommUse;

	private CheckEditEx ItmCommUse;

	private CheckEditEx SlpCommUse;

	private CheckEditEx PickLimit;

	private CheckEditEx OrderLimit;

	private CheckEditEx DlnLimit;

	private CheckEditEx SalesLimit;

	private CheckEditEx CreditLimt;

	private CheckEditEx ObligLimit;

	private CheckEditEx AddDlnBlnc;

	private LayoutControlGroup Root;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem6;

	private LayoutControlItemEx layoutControlItem3;

	private LayoutControlItemEx layoutControlItem4;

	private LayoutControlItemEx layoutControlItem5;

	private LayoutControlItemEx layoutControlItem6;

	private LayoutControlItemEx layoutControlItem7;

	private LayoutControlItemEx layoutControlItem8;

	private LayoutControlItemEx layoutControlItem9;

	private LayoutControlItemEx layoutControlItem10;

	private LayoutControlItemEx layoutControlItem11;

	private LayoutControlItemEx layoutControlItem12;

	private LayoutControlItemEx layoutControlItem13;

	private CheckEditEx MBAOnPer;

	private CheckEditEx DspIBPDoc;

	private CheckEditEx DspFrznBP;

	private CheckEditEx EnbApprDI;

	private CheckEditEx useDocWrf;

	private LayoutControlItemEx layoutControlItem14;

	private LayoutControlItemEx layoutControlItem15;

	private LayoutControlItemEx layoutControlItem16;

	private LayoutControlItemEx layoutControlItem17;

	private LayoutControlItemEx layoutControlItem19;

	private LayoutControlItemEx layoutControlItem20;

	private LayoutControlItemEx layoutControlItem21;

	private LayoutControlItemEx layoutControlItem22;

	private LayoutControlItemEx layoutControlItem23;

	private EmptySpaceItem emptySpaceItem3;

	private SimpleLabelItem simpleLabelItem2;

	private SimpleLabelItem simpleLabelItem1;

	private EmptySpaceItem emptySpaceItem8;

	private LayoutControl layoutControl2;

	private LayoutControlGroup layoutControlGroup1;

	private EmptySpaceItem emptySpaceItem9;

	private CheckEditEx DoBudget;

	private LayoutControlItemEx layoutControlItem1;

	private SimpleLabelItem simpleLabelItem3;

	private RadioGroupEx BgtBlock;

	private LayoutControlItemEx layoutControlItem2;

	private EmptySpaceItem emptySpaceItem7;

	private EmptySpaceItem emptySpaceItem10;

	private EmptySpaceItem emptySpaceItem11;

	private CheckEditEx BdgtAcctng;

	private CheckEditEx BdgtPDNDoc;

	private CheckEditEx BdgtPORDoc;

	private CheckEditEx BdgtPRQDOC;

	private RadioGroupEx BgtWarning;

	private EmptySpaceItem emptySpaceItem12;

	private LayoutControlItemEx layoutControlItem24;

	private EmptySpaceItem emptySpaceItem13;

	private EmptySpaceItem emptySpaceItem14;

	private EmptySpaceItem emptySpaceItem15;

	private EmptySpaceItem emptySpaceItem16;

	private LayoutControlItemEx layoutControlItem25;

	private LayoutControlItemEx layoutControlItem26;

	private LayoutControlItemEx layoutControlItem27;

	private LayoutControlItemEx layoutControlItem28;

	private LayoutControl layoutControl3;

	private CheckEditEx SPNEnabled;

	private CheckEditEx IMCEEnable;

	private TextEditEx MaxHistory;

	private TextEditEx ScreenLock;

	private TextEditEx AlertPolFr;

	private RadioGroupEx DfltByEml;

	private CheckEditEx ck_UsedProxy;

	private CheckEditEx ShowNewTsk;

	private CheckEditEx OpenDps;

	private CheckEditEx OpenCdt;

	private CheckEditEx ShowNewMsg;

	private CheckEditEx ContactLog;

	private CheckEditEx RclFlag;

	private CheckEditEx RcrFlag;

	private CheckEditEx DsplyRates;

	private CheckEditEx CheckFiles;

	private LookUpEditCombo OpenCredit;

	private LookUpEditCombo MapService;

	private LayoutControlGroup layoutControlGroup2;

	private EmptySpaceItem emptySpaceItem17;

	private EmptySpaceItem emptySpaceItem18;

	private EmptySpaceItem emptySpaceItem19;

	private EmptySpaceItem emptySpaceItem20;

	private EmptySpaceItem emptySpaceItem21;

	private EmptySpaceItem emptySpaceItem22;

	private SimpleLabelItem simpleLabelItem4;

	private LayoutControlItemEx layoutControlItem29;

	private LayoutControlItemEx layoutControlItem30;

	private LayoutControlItemEx layoutControlItem31;

	private LayoutControlItemEx layoutControlItem32;

	private LayoutControlItemEx layoutControlItem33;

	private LayoutControlItemEx layoutControlItem34;

	private LayoutControlItemEx layoutControlItem35;

	private LayoutControlItemEx layoutControlItem36;

	private LayoutControlItemEx layoutControlItem37;

	private LayoutControlItemEx layoutControlItem38;

	private LayoutControlItemEx layoutControlItem39;

	private LayoutControlItemEx layoutControlItem40;

	private EmptySpaceItem emptySpaceItem23;

	private LayoutControlItemEx layoutControlItem41;

	private EmptySpaceItem emptySpaceItem24;

	private LayoutControlItemEx layoutControlItem42;

	private LayoutControlItemEx layoutControlItem43;

	private LayoutControlItemEx layoutControlItem44;

	private LayoutControlItemEx layoutControlItem45;

	private LayoutControlItemEx layoutControlItem46;

	private LayoutControlItemEx layoutControlItem47;

	private LayoutControlItemEx layoutControlItem48;

	private LayoutControlItemEx layoutControlItem49;

	private EmptySpaceItem emptySpaceItem25;

	private LayoutControlItemEx layoutControlItem50;

	private EmptySpaceItem emptySpaceItem26;

	private MemoExEdit memo_Address;

	private MemoExEdit memo_Port;

	private LayoutControl layoutControl4;

	private CheckEditEx ck_ManagerCompany;

	private RadioGroupEx DirectRate;

	private CheckEditEx CurOnRight;

	private TextEditEx ThousSep;

	private TextEditEx DecSep;

	private TextEditEx QueryDec;

	private TextEditEx MeasureDec;

	private TextEditEx PercentDec;

	private TextEditEx QtyDec;

	private TextEditEx RateDec;

	private TextEditEx SumDec;

	private TextEditEx MaxChoose;

	private TextEditEx AdvImagePr;

	private TextEditEx DateSep;

	private LayoutControlGroup layoutControlGroup3;

	private EmptySpaceItem emptySpaceItem27;

	private EmptySpaceItem emptySpaceItem28;

	private EmptySpaceItem emptySpaceItem29;

	private SimpleLabelItem simpleLabelItem8;

	private LayoutControlItemEx layoutControlItem53;

	private LayoutControlItemEx layoutControlItem54;

	private LayoutControlItemEx layoutControlItem55;

	private LayoutControlItemEx layoutControlItem56;

	private LayoutControlItemEx layoutControlItem57;

	private LayoutControlItemEx layoutControlItem58;

	private EmptySpaceItem emptySpaceItem32;

	private LayoutControlItemEx layoutControlItem59;

	private LayoutControlItemEx layoutControlItem60;

	private LayoutControlItemEx layoutControlItem61;

	private LayoutControlItemEx layoutControlItem62;

	private LayoutControlItemEx layoutControlItem63;

	private LayoutControlItemEx layoutControlItem64;

	private LayoutControlItemEx layoutControlItem65;

	private LayoutControlItemEx layoutControlItem66;

	private LayoutControlItemEx layoutControlItem67;

	private LayoutControlItemEx layoutControlItem68;

	private LayoutControlItemEx layoutControlItem69;

	private LayoutControlItemEx layoutControlItem70;

	private EmptySpaceItem emptySpaceItem33;

	private SimpleLabelItem simpleLabelItem9;

	private LayoutControlItemEx layoutControlItem71;

	private EmptySpaceItem emptySpaceItem34;

	private LayoutControlItemEx layoutControlItem72;

	private CheckEditEx TxtSrch;

	private CheckEditEx B1SgtEb;

	private SimpleLabelItem simpleLabelItem10;

	private LayoutControlItemEx layoutControlItem73;

	private LayoutControlItemEx layoutControlItem74;

	private LayoutControlItemEx layoutControlItem75;

	private EmptySpaceItem emptySpaceItem35;

	private LayoutControl layoutControl5;

	private PictureBox txt_PicBox;

	private SimpleButtonEx simpleButton1;

	private LookUpEditCombo WllPprDsp;

	private LayoutControlGroup layoutControlGroup4;

	private EmptySpaceItem emptySpaceItem38;

	private LayoutControlItemEx layoutControlItem76;

	private LayoutControlItemEx layoutControlItem77;

	private LayoutControlItemEx layoutControlItem78;

	private LayoutControlItemEx layoutControlItem79;

	private EmptySpaceItem emptySpaceItem40;

	private SimpleLabelItem simpleLabelItem11;

	private SimpleLabelItem simpleLabelItem12;

	private LayoutControlItemEx layoutControlItem80;

	private LayoutControlItemEx layoutControlItem81;

	private LayoutControlItemEx layoutControlItem82;

	private LayoutControl layoutControl6;

	private SimpleButtonEx btn_XML;

	private SimpleButtonEx btn_Current;

	private SimpleButtonEx btn_Extension;

	private SimpleButtonEx btn_Att;

	private SimpleButtonEx btn_Pic;

	private SimpleButtonEx btn_Excel;

	private SimpleButtonEx btn_MicWord;

	private TextEditEx XmlPath;

	private TextEditEx txt_CurrentScander;

	private TextEditEx ExtPath;

	private TextEditEx AttachPath;

	private TextEditEx BitmapPath;

	private TextEditEx ExcelPath;

	private TextEditEx WordPath;

	private LayoutControlGroup layoutControlGroup5;

	private EmptySpaceItem emptySpaceItem44;

	private LayoutControlItemEx layoutControlItem83;

	private LayoutControlItemEx layoutControlItem84;

	private LayoutControlItemEx layoutControlItem85;

	private LayoutControlItemEx layoutControlItem86;

	private LayoutControlItemEx layoutControlItem87;

	private LayoutControlItemEx layoutControlItem88;

	private LayoutControlItemEx layoutControlItem89;

	private LayoutControlItemEx layoutControlItem90;

	private LayoutControlItemEx layoutControlItem91;

	private LayoutControlItemEx layoutControlItem92;

	private LayoutControlItemEx layoutControlItem93;

	private LayoutControlItemEx layoutControlItem94;

	private LayoutControlItemEx layoutControlItem95;

	private LayoutControlItemEx layoutControlItem96;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx Tab_Item;

	private LayoutControl layoutControl7;

	private CheckEditEx DspFrznITM;

	private CheckEditEx DspIITMDoc;

	private CheckEditEx AutoAddPkg;

	private CheckEditEx AutoAddUoM;

	private CheckEditEx WTLiable;

	private CheckEditEx AutoITW;

	private CheckEditEx AutoCrIns;

	private LayoutControlGroup layoutControlGroup6;

	private EmptySpaceItem emptySpaceItem48;

	private EmptySpaceItem emptySpaceItem50;

	private EmptySpaceItem emptySpaceItem51;

	private SimpleLabelItem simpleLabelItem13;

	private LayoutControlItemEx layoutControlItem98;

	private LayoutControlItemEx layoutControlItem99;

	private LayoutControlItemEx layoutControlItem100;

	private LayoutControlItemEx layoutControlItem101;

	private LayoutControlItemEx layoutControlItem102;

	private LayoutControlItemEx layoutControlItem103;

	private EmptySpaceItem emptySpaceItem53;

	private LayoutControlItemEx layoutControlItem105;

	private LayoutControlItemEx layoutControlItem106;

	private EmptySpaceItem emptySpaceItem54;

	private LayoutControlItemEx layoutControlItem107;

	private SimpleLabelItem simpleLabelItem14;

	private EmptySpaceItem emptySpaceItem55;

	private LayoutControlItemEx layoutControlItem108;

	private LayoutControlItemEx layoutControlItem109;

	private XtraTabPageEx Tab_Plan;

	private XtraTabPageEx Tab_Report;

	private LayoutControl layoutControl8;

	private TextEditEx Days;

	private TextEditEx Months;

	private CheckEditEx AutoResWhs;

	private LayoutControlGroup layoutControlGroup7;

	private EmptySpaceItem emptySpaceItem56;

	private EmptySpaceItem emptySpaceItem57;

	private LayoutControlItemEx layoutControlItem110;

	private LayoutControlItemEx layoutControlItem111;

	private SimpleLabelItem simpleLabelItem15;

	private LayoutControlItemEx layoutControlItem112;

	private LayoutControlItemEx layoutControlItem113;

	private LayoutControlItemEx layoutControlItem114;

	private EmptySpaceItem emptySpaceItem58;

	private LayoutControl layoutControl9;

	private RadioGroupEx IgrAllCash;

	private RadioGroupEx CfwAsnMust;

	private LayoutControlGroup layoutControlGroup8;

	private EmptySpaceItem emptySpaceItem59;

	private EmptySpaceItem emptySpaceItem60;

	private SimpleLabelItem simpleLabelItem16;

	private LayoutControlItemEx layoutControlItem115;

	private EmptySpaceItem emptySpaceItem61;

	private SimpleLabelItem simpleLabelItem17;

	private LayoutControlItemEx layoutControlItem116;

	private LayoutControlItemEx layoutControlItem117;

	private SimpleLabelItem simpleLabelItem18;

	private EmptySpaceItem emptySpaceItem62;

	private EmptySpaceItem emptySpaceItem63;

	private LayoutControlItemEx layoutControlItem118;

	private LayoutControl layoutControl10;

	private SimpleButtonEx btn_More;

	private RadioGroupEx PrsWkCntEb;

	private LayoutControlGroup layoutControlGroup9;

	private EmptySpaceItem emptySpaceItem64;

	private EmptySpaceItem emptySpaceItem65;

	private LayoutControlItemEx layoutControlItem119;

	private EmptySpaceItem emptySpaceItem66;

	private LayoutControlItemEx layoutControlItem120;

	private EmptySpaceItem emptySpaceItem67;

	private EmptySpaceItem emptySpaceItem68;

	private LayoutControl layoutControl12;

	private CheckEditEx SIPLSeting;

	private CheckEditEx SIPLDoc;

	private CheckEditEx SIPLReport;

	private CheckEditEx IsUpdNstdB;

	private CheckEditEx ClnZeroPln;

	private CheckEditEx ApyBsActPL;

	private CheckEditEx ApyBsActSP;

	private CheckEditEx ApyBsActPV;

	private LayoutControlGroup layoutControlGroup11;

	private EmptySpaceItem emptySpaceItem75;

	private EmptySpaceItem emptySpaceItem76;

	private LayoutControlItemEx layoutControlItem125;

	private LayoutControlItemEx layoutControlItem126;

	private LayoutControlItemEx layoutControlItem127;

	private EmptySpaceItem emptySpaceItem77;

	private LayoutControlItemEx layoutControlItem128;

	private EmptySpaceItem emptySpaceItem78;

	private LayoutControlItemEx layoutControlItem129;

	private LayoutControlItemEx layoutControlItem130;

	private LayoutControlItemEx layoutControlItem131;

	private LayoutControlItemEx layoutControlItem132;

	private LayoutControlItemEx ck_Setting;

	private LayoutControl layoutControl13;

	private CheckEditEx ck_MRP;

	private CheckEditEx ck_;

	private CheckEditEx ck_SerialNum;

	private CheckEditEx ck_CostCounting;

	private CheckEditEx ck_DunningWizard;

	private CheckEditEx ck_PaymentWizard;

	private CheckEditEx ck_Budget;

	private LayoutControlGroup layoutControlGroup12;

	private SimpleLabelItem simpleLabelItem23;

	private LayoutControlItemEx layoutControlItem134;

	private LayoutControlItemEx layoutControlItem135;

	private LayoutControlItemEx layoutControlItem136;

	private LayoutControlItemEx layoutControlItem137;

	private LayoutControlItemEx layoutControlItem138;

	private LayoutControlItemEx layoutControlItem139;

	private LayoutControlItemEx layoutControlItem140;

	private FolderBrowserDialog folderBrowserDialog1;

	private EmptySpaceItem emptySpaceItem37;

	private CheckEditEx SriCreatIn;

	private LayoutControlItemEx layoutControlItem142;

	private CheckEditEx OneBOneRec;

	private EmptySpaceItem emptySpaceItem36;

	private LayoutControlItemEx layoutControlItem143;

	private LayoutControlItemEx layoutControlItem144;

	private LayoutControl layoutControl14;

	private LayoutControlGroup layoutControlGroup13;

	private EmptySpaceItem emptySpaceItem39;

	private LayoutControl layoutControl15;

	private LayoutControlGroup layoutControlGroup14;

	private TextEditEx DaysBack;

	private TextEditEx DaysFwrd;

	private CheckEditEx ConsumeFCT;

	private LayoutControlItemEx layoutControlItem104;

	private LayoutControlItemEx layoutControlItem145;

	private LayoutControlItemEx layoutControlItem146;

	private EmptySpaceItem emptySpaceItem52;

	private LayoutControlItemEx layoutControlItem141;

	private EmptySpaceItem emptySpaceItem79;

	private EmptySpaceItem emptySpaceItem82;

	private EmptySpaceItem emptySpaceItem49;

	private RadioGroupEx SimReport;

	private SimpleLabelItem simpleLabelItem24;

	private LayoutControlItemEx layoutControlItem147;

	private EmptySpaceItem emptySpaceItem83;

	private LayoutControl layoutControl11;

	private CheckEditEx UseMltDims;

	private LayoutControlGroup layoutControlGroup10;

	private LayoutControlItemEx layoutControlItem121;

	private EmptySpaceItem emptySpaceItem70;

	private SimpleLabelItem simpleLabelItem19;

	private LayoutControlItemEx layoutControlItem124;

	private EmptySpaceItem emptySpaceItem72;

	private LabelLine label3;

	private RadioGroupEx MDStyle;

	private EmptySpaceItem emptySpaceItem73;

	private EmptySpaceItem emptySpaceItem85;

	private EmptySpaceItem emptySpaceItem86;

	private EmptySpaceItem emptySpaceItem87;

	private LayoutControlItemEx layoutControlItem122;

	private LayoutControlItemEx layoutControlItem148;

	private EmptySpaceItem emptySpaceItem74;

	private EmptySpaceItem emptySpaceItem84;

	private LabelLine label6;

	private LabelLine label5;

	private LayoutControlItemEx layoutControlItem150;

	private LayoutControlItemEx layoutControlItem151;

	private CheckEditEx ck_UnitOfMeasure;

	private LayoutControlItemEx layoutControlItem149;

	private LabelLine label4;

	private LabelLine label2;

	private LabelLine label1;

	private LayoutControlItemEx layoutControlItem152;

	private LayoutControlItemEx layoutControlItem153;

	private LayoutControlItemEx layoutControlItem154;

	private MemoExEdit memo_Area;

	private MemoExEdit txt_ForEx;

	private LookUpEditCombo Color;

	private LookUpEditCombo DefLengthU;

	private LookUpEditCombo DefWeightU;

	private LookUpEditCombo TimeFormat;

	private LookUpEditCombo DateFormat;

	private CheckEditEx ApyPRinPEC;

	private CheckEditEx ApyDRinPEC;

	private LayoutControlItemEx layoutControlItem133;

	private LayoutControlItemEx layoutControlItem155;

	private EmptySpaceItem emptySpaceItem71;

	private LookUpEditCombo DfCustTerm;

	private LookUpEditCombo DfVendTerm;

	private LookUpEditCombo DfltCustPM;

	private LookUpEditCombo DfltVendPM;

	private LookUpEditCombo CreditDpst;

	private FontEdit lok_Fonts;

	private ComboBoxEditEx lok_fontSize;

	private MemoEditEx txt_PreviewFont;

	private ContextMenuStrip contextMenuStrip1;

	private LookUpEditCombo SriMngSys;

	private LookUpEditCombo SriUniqFld;

	private LookUpEditCombo BtchStatus;

	private LookUpEditCombo DfltWhs;

	private LookUpEditCombo GLMethod;

	private LookUpEditCombo ConsumeMtd;

	private TextEditEx WallPaper;

	private EmptySpaceItem emptySpaceItem42;

	private LookUpEditCombo TPLId;

	private LookUpEditCombo DftResWhs;

	private SimpleButtonEx simpleButton2;

	private SimpleLabelItem simpleLabelItem5;

	private LayoutControlItemEx layoutControlItem18;

	private LookUpEditCombo PrjBlock;

	private LookUpEditCombo PriceProcM;

	private EmptySpaceItem emptySpaceItem30;

	private EmptySpaceItem emptySpaceItem31;

	private LookUpEditCombo StartFrom;

	private LookUpEditCombo CfwInDflt;

	private LookUpEditCombo CfwOutDflt;

	private OpenFileDialog openFileDialog1;

	private LayoutControlItemEx layoutControlItem51;

	private EmptySpaceItem emptySpaceItem43;

	private CalcEditEx PriceDec;

	private XtraTabPageEx Tab_Sales;

	private LabelLine labelLine1;

	private LookUpEditCombo luk_DfltCard;

	private XtraTabPageEx Tab_Purchasing;

	private LookUpEditCombo DfSVatServ;

	private LookUpEditCombo DfSVatItem;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LookUpEditCombo DfPVatServ;

	private LookUpEditCombo DfPVatItem;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private GridControlEx grd_ObjAtt;

	private GridViewEx gv_ObjAtt;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemButtonEdit bte_Path;

	private LayoutControlItemEx layoutControlItem52;

	private RepositoryItemComboBox cbo_ObjType;

	private RepositoryItemLookUpEditSimple luk_ObjType;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem toolStripMenuItem1;

	private CheckEditEx DisableSplitFolder;

	private LayoutControlItemEx layoutControlItem97;

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

	public frm_GeneSetting()
	{
		InitializeComponent();
		FormId = "oGeneSetting";
		OnLoadAsync += frm_GeneSetting_Load;
		gv_ObjAtt.CellValueChanged += Gv_ObjAtt_CellValueChanged;
	}

	public async Task frm_GeneSetting_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			IMCEEnable.Enabled = false;
			btn_More.Enabled = false;
			gv_ObjAtt.OptionsView.ColumnAutoWidth = true;
			await LoadInItControl();
			await LoadData();
			if (!GlobalData.LoginSap)
			{
				Tab_Budget.PageVisible = false;
				simpleLabelItem5.Visibility = LayoutVisibility.Never;
				Tab_Font.PageVisible = false;
				layoutControlItem15.Visibility = LayoutVisibility.Never;
				layoutControlItem16.Visibility = LayoutVisibility.Never;
				layoutControlItem17.Visibility = LayoutVisibility.Never;
				MBAOnPer.Visible = false;
				WTLiable.Visible = false;
				layoutControlItem105.Visibility = LayoutVisibility.Never;
				layoutControlItem144.Visibility = LayoutVisibility.Never;
				layoutControlItem148.Visibility = LayoutVisibility.Never;
				layoutControlItem133.Visibility = LayoutVisibility.Never;
				layoutControlItem155.Visibility = LayoutVisibility.Never;
				layoutControlItem18.Visibility = LayoutVisibility.Never;
				layoutControlItem124.Visibility = LayoutVisibility.Never;
			}
			else
			{
				Tab_Budget.PageVisible = true;
				simpleLabelItem5.Visibility = LayoutVisibility.Always;
				Tab_Font.PageVisible = true;
				layoutControlItem15.Visibility = LayoutVisibility.Always;
				layoutControlItem16.Visibility = LayoutVisibility.Always;
				layoutControlItem17.Visibility = LayoutVisibility.Always;
				MBAOnPer.Visible = true;
				WTLiable.Visible = true;
				layoutControlItem105.Visibility = LayoutVisibility.Always;
				layoutControlItem144.Visibility = LayoutVisibility.Always;
				layoutControlItem148.Visibility = LayoutVisibility.Always;
				layoutControlItem133.Visibility = LayoutVisibility.Always;
				layoutControlItem155.Visibility = LayoutVisibility.Always;
				layoutControlItem18.Visibility = LayoutVisibility.Always;
				layoutControlItem124.Visibility = LayoutVisibility.Always;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void ReloadPaymentTerm()
	{
		try
		{
			DataTable dataTable = await GetDefaultPaymentTermforCustomer();
			if (dataTable.IsNotNull())
			{
				DfCustTerm.Properties.DataSource = dataTable;
				DfCustTerm.Properties.DisplayMember = "Name";
				DfCustTerm.Properties.ValueMember = "Code";
				DfCustTerm.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DfVendTerm.Properties.DataSource = dataTable;
				DfVendTerm.Properties.DisplayMember = "Name";
				DfVendTerm.Properties.ValueMember = "Code";
				DfVendTerm.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInItControl()
	{
		_ = 9;
		try
		{
			DataTable dataTable = await ExistsTransaction();
			if (dataTable.IsNotNull())
			{
				DirectRate.ReadOnly = Function.ToString(dataTable.Rows[0][0]).Equals("Y");
			}
			DataTable dataTable2 = await GetDefaultPaymentTermforCustomer();
			if (dataTable2.IsNotNull())
			{
				DfCustTerm.Properties.DataSource = dataTable2;
				DfCustTerm.Properties.DisplayMember = "Name";
				DfCustTerm.Properties.ValueMember = "Code";
				DfCustTerm.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DfVendTerm.Properties.DataSource = dataTable2;
				DfVendTerm.Properties.DisplayMember = "Name";
				DfVendTerm.Properties.ValueMember = "Code";
				DfVendTerm.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			DataTable dataTable3 = await GetInputTax();
			if (dataTable3.IsNotNull())
			{
				DfPVatItem.Properties.DataSource = dataTable3;
				DfPVatItem.Properties.DisplayMember = "Name";
				DfPVatItem.Properties.ValueMember = "Code";
				DfPVatItem.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DfPVatServ.Properties.DataSource = dataTable3;
				DfPVatServ.Properties.DisplayMember = "Name";
				DfPVatServ.Properties.ValueMember = "Code";
				DfPVatServ.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			DataTable dataTable4 = await GetOutputTax();
			if (dataTable4.IsNotNull())
			{
				DfSVatItem.Properties.DataSource = dataTable4;
				DfSVatItem.Properties.DisplayMember = "Name";
				DfSVatItem.Properties.ValueMember = "Code";
				DfSVatItem.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DfSVatServ.Properties.DataSource = dataTable4;
				DfSVatServ.Properties.DisplayMember = "Name";
				DfSVatServ.Properties.ValueMember = "Code";
				DfSVatServ.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			DataTable dataTable5 = await LoadDataCashFlow();
			if (dataTable5.IsNotNull())
			{
				CfwInDflt.Properties.DataSource = dataTable5;
				CfwInDflt.Properties.DisplayMember = "Name";
				CfwInDflt.Properties.ValueMember = "Code";
				CfwInDflt.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				CfwOutDflt.Properties.DataSource = dataTable5;
				CfwOutDflt.Properties.DisplayMember = "Name";
				CfwOutDflt.Properties.ValueMember = "Code";
				CfwOutDflt.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			DataTable dataTable6 = await GetDefaultPaymentMethodforCustomer();
			if (dataTable6.IsNotNull())
			{
				DataRow row = dataTable6.NewRow();
				dataTable6.Rows.InsertAt(row, 0);
				dataTable6.AcceptChanges();
				DfltCustPM.Properties.DataSource = dataTable6;
				DfltCustPM.Properties.DisplayMember = "Name";
				DfltCustPM.Properties.ValueMember = "Code";
				DfltCustPM.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DfltVendPM.Properties.DataSource = dataTable6;
				DfltVendPM.Properties.DisplayMember = "Name";
				DfltVendPM.Properties.ValueMember = "Code";
				DfltVendPM.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			List<Common.Table_Code> dataSource = new Dictionary<string, string>
			{
				{ "Y", "Manually" },
				{ "N", "Automatically" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			CreditDpst.Properties.DataSource = dataSource;
			CreditDpst.Properties.DisplayMember = "Name";
			CreditDpst.Properties.ValueMember = "Code";
			CreditDpst.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> dataSource2 = new Dictionary<string, string>
			{
				{ "0", "24H" },
				{ "1", "12H" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_ID
			{
				ID = item.Key,
				Name = item.Value
			}).ToList();
			TimeFormat.Properties.DataSource = dataSource2;
			TimeFormat.Properties.DisplayMember = "Name";
			TimeFormat.Properties.ValueMember = "ID";
			TimeFormat.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> dataSource3 = new Dictionary<string, string>
			{
				{ "0", "DD/MM/YY" },
				{ "1", "DD/MM/CCYY" },
				{ "2", "MM/DD/YY" },
				{ "3", "MM/DD/CCYY" },
				{ "4", "CCYY/MM/DD" },
				{ "5", "DD/Month/YYYY" },
				{ "6", "YY/MM/DD" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_ID
			{
				ID = item.Key,
				Name = item.Value
			}).ToList();
			DateFormat.Properties.DataSource = dataSource3;
			DateFormat.Properties.DisplayMember = "Name";
			DateFormat.Properties.ValueMember = "ID";
			DateFormat.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			RepositoryItemLookUpEditCombo properties = DefLengthU.Properties;
			properties.DataSource = await httpClient.GeneralSetting.GetLengthUnits();
			DefLengthU.Properties.DisplayMember = "Name";
			DefLengthU.Properties.ValueMember = "ID";
			DefLengthU.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			properties = DefWeightU.Properties;
			properties.DataSource = await httpClient.GeneralSetting.GetWeightUnits();
			DefWeightU.Properties.DisplayMember = "Name";
			DefWeightU.Properties.ValueMember = "ID";
			DefWeightU.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource4 = new Dictionary<string, string>
			{
				{ "Y", "Always" },
				{ "N", "No" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			OpenCredit.Properties.DataSource = dataSource4;
			OpenCredit.Properties.DisplayMember = "Name";
			OpenCredit.Properties.ValueMember = "Code";
			OpenCredit.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			DataTable dataTable7 = await GetMapService();
			if (dataTable7.IsNotNull())
			{
				DataRow row2 = dataTable7.NewRow();
				dataTable7.Rows.InsertAt(row2, 0);
				dataTable7.AcceptChanges();
				MapService.Properties.DataSource = dataTable7;
				MapService.Properties.DisplayMember = "Name";
				MapService.Properties.ValueMember = "Code";
				MapService.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			Dictionary<int, string> dictionary = new Dictionary<int, string>
			{
				{ 0, "Combined" },
				{ 1, "Classic" },
				{ 2, "Gray" },
				{ 3, "Violet" },
				{ 4, "Blue" },
				{ 5, "Green" },
				{ 6, "Yellow" },
				{ 7, "Orange" },
				{ 8, "Red" },
				{ 9, "Brown" }
			};
			DataTable dataTable8 = new DataTable
			{
				Columns = 
				{
					{
						"ID",
						typeof(int)
					},
					{
						"Name",
						typeof(string)
					}
				}
			};
			foreach (KeyValuePair<int, string> item in dictionary)
			{
				DataRow dataRow = dataTable8.NewRow();
				dataRow["ID"] = item.Key;
				dataRow["Name"] = item.Value;
				dataTable8.Rows.Add(dataRow);
			}
			Color.Properties.DataSource = dataTable8;
			Color.Properties.DisplayMember = "Name";
			Color.Properties.ValueMember = "ID";
			Color.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource5 = new Dictionary<string, string>
			{
				{ "A", "On Every Transaction" },
				{ "R", "On Release Only" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			SriMngSys.Properties.DataSource = dataSource5;
			SriMngSys.Properties.DisplayMember = "Name";
			SriMngSys.Properties.ValueMember = "Code";
			SriMngSys.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> dataSource6 = new Dictionary<string, string>
			{
				{ "0", "None" },
				{ "2", "Mfr Serial No." },
				{ "3", "Serial Number" },
				{ "4", "Lot Number" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_ID
			{
				ID = item.Key,
				Name = item.Value
			}).ToList();
			SriUniqFld.Properties.DataSource = dataSource6;
			SriUniqFld.Properties.DisplayMember = "Name";
			SriUniqFld.Properties.ValueMember = "ID";
			SriUniqFld.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			Dictionary<int, string> dictionary2 = new Dictionary<int, string>
			{
				{ 1, "Centralized" },
				{ 2, "Full Screen" },
				{ 3, "Tile" }
			};
			DataTable dataTable9 = new DataTable
			{
				Columns = 
				{
					{
						"ID",
						typeof(int)
					},
					{
						"Name",
						typeof(string)
					}
				}
			};
			foreach (KeyValuePair<int, string> item2 in dictionary2)
			{
				DataRow dataRow2 = dataTable9.NewRow();
				dataRow2["ID"] = item2.Key;
				dataRow2["Name"] = item2.Value;
				dataTable9.Rows.Add(dataRow2);
			}
			WllPprDsp.Properties.DataSource = dataTable9;
			WllPprDsp.Properties.DisplayMember = "Name";
			WllPprDsp.Properties.ValueMember = "ID";
			WllPprDsp.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> dataSource7 = new Dictionary<string, string>
			{
				{ "1", "Today" },
				{ "2", "Month Start" },
				{ "3", "Month End" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_ID
			{
				ID = item.Key,
				Name = item.Value
			}).ToList();
			StartFrom.Properties.DataSource = dataSource7;
			StartFrom.Properties.DisplayMember = "Name";
			StartFrom.Properties.ValueMember = "ID";
			StartFrom.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> dataSource8 = new Dictionary<string, string>
			{
				{ "0", "Released" },
				{ "1", "Not Accessible" },
				{ "2", "Locked" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_ID
			{
				ID = item.Key,
				Name = item.Value
			}).ToList();
			BtchStatus.Properties.DataSource = dataSource8;
			BtchStatus.Properties.DisplayMember = "Name";
			BtchStatus.Properties.ValueMember = "ID";
			BtchStatus.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			DataTable dataTable10 = await GetDfltWhs();
			if (dataTable10.IsNotNull())
			{
				DataRow row3 = dataTable10.NewRow();
				dataTable10.Rows.InsertAt(row3, 0);
				dataTable10.AcceptChanges();
				DfltWhs.Properties.DataSource = dataTable10;
				DfltWhs.Properties.DisplayMember = "Name";
				DfltWhs.Properties.ValueMember = "Code";
				DfltWhs.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				DftResWhs.Properties.DataSource = dataTable10;
				DftResWhs.Properties.DisplayMember = "Name";
				DftResWhs.Properties.ValueMember = "Code";
				DftResWhs.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
			List<Common.Table_Code> dataSource9 = new Dictionary<string, string>
			{
				{ "B", "Backward-Forward" },
				{ "F", "Forward-Backward" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			ConsumeMtd.Properties.DataSource = dataSource9;
			ConsumeMtd.Properties.DisplayMember = "Name";
			ConsumeMtd.Properties.ValueMember = "Code";
			ConsumeMtd.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource10 = new Dictionary<string, string>
			{
				{ "N", "Without Warning" },
				{ "B", "Block Posting" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			PrjBlock.Properties.DataSource = dataSource10;
			PrjBlock.Properties.DisplayMember = "Name";
			PrjBlock.Properties.ValueMember = "Code";
			PrjBlock.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource11 = new Dictionary<string, string>
			{
				{ "R", "Remove UoM Prices" },
				{ "U", "Update UoM Prices Accordingly" },
				{ "L", "Keep Corresponding UoM Prices Unchanged" },
				{ "K", "Keep All UoM Prices Unchanged" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			PriceProcM.Properties.DataSource = dataSource11;
			PriceProcM.Properties.DisplayMember = "Name";
			PriceProcM.Properties.ValueMember = "Code";
			PriceProcM.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource12 = new Dictionary<string, string>
			{
				{ "W", "Warehouse" },
				{ "C", "Item Group" },
				{ "L", "Item Level" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			GLMethod.Properties.DataSource = dataSource12;
			GLMethod.Properties.DisplayMember = "Name";
			GLMethod.Properties.ValueMember = "Code";
			GLMethod.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			if (ck_UsedProxy.Checked)
			{
				layoutControlItem39.Visibility = LayoutVisibility.Always;
				layoutControlItem40.Visibility = LayoutVisibility.Always;
			}
			else
			{
				layoutControlItem39.Visibility = LayoutVisibility.Never;
				layoutControlItem40.Visibility = LayoutVisibility.Never;
			}
			TPLId.EditValue = DBNull.Value;
			SetLookUpObjectType();
			Tab_Budget.PageVisible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetLookUpObjectType()
	{
		try
		{
			luk_ObjType.TextEditStyle = TextEditStyles.Standard;
			luk_ObjType.SetDataSourceObject();
			luk_ObjType.PopulateColumns();
			if (luk_ObjType.Columns.Count > 0)
			{
				luk_ObjType.Columns[0].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetControl(Control ctr, DataRow row)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (row.IsExistsColumn(control.Name))
				{
					if (control is CheckEdit)
					{
						((CheckEdit)control).EditValue = row[control.Name];
					}
					if (control is LookUpEdit)
					{
						((LookUpEdit)control).EditValue = row[control.Name];
					}
					if (control is TextEdit)
					{
						((TextEdit)control).EditValue = row[control.Name];
					}
					if (control is MemoExEdit)
					{
						((MemoExEdit)control).EditValue = row[control.Name];
					}
					if (control is RadioGroup)
					{
						((RadioGroup)control).EditValue = row[control.Name];
					}
					if (control is CalcEditEx)
					{
						((CalcEdit)control).EditValue = row[control.Name];
					}
				}
				else
				{
					GetControl(control, row);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataRow GetDataRow(Control ctr, DataRow row)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (row.IsExistsColumn(control.Name))
				{
					if (control is CheckEdit)
					{
						row[control.Name] = ((CheckEdit)control).EditValue;
					}
					if (control is LookUpEdit)
					{
						row[control.Name] = ((LookUpEdit)control).EditValue;
					}
					if (control is TextEdit)
					{
						row[control.Name] = ((TextEdit)control).EditValue;
					}
					if (control is MemoExEdit)
					{
						row[control.Name] = ((MemoExEdit)control).EditValue;
					}
					if (control is RadioGroup)
					{
						row[control.Name] = ((RadioGroup)control).EditValue;
					}
					if (control is CalcEditEx)
					{
						row[control.Name] = ((CalcEdit)control).EditValue;
					}
				}
				else
				{
					GetDataRow(control, row);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return row;
	}

	private async Task LoadData()
	{
		_isLoading = true;
		try
		{
			dts = await GetData();
			if (dts.IsNotNull())
			{
				for (int i = 0; i < dts.Tables.Count; i++)
				{
					DataTable dataTable = dts.Tables[i];
					if (dataTable.IsNotNull())
					{
						DataRow row = dataTable.Rows[0];
						GetControl(xtraTab_Inventory, row);
					}
				}
			}
			DataTable dataTable2 = await GetObjectTypePath();
			if (dataTable2.IsNotNull())
			{
				grd_ObjAtt.DataSource = dataTable2;
			}
			else
			{
				grd_ObjAtt.DataSource = WinFuntions.GetTableStructure("APZ_ATC2");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async Task<DataSet> GetData()
	{
		try
		{
			int userID = GlobalData.UserID;
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetData", new
			{
				userId = userID
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataTable> GetDefaultPaymentTermforCustomer()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetDefaultPaymentTermforCustomer");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetInputTax()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
			return await helper.GetAsync("GetVatGroupI");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetOutputTax()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
			return await helper.GetAsync("GetVatGroupO");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadDataCashFlow()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await helper.GetAsync("LoadDataCashFlow");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetDefaultPaymentMethodforCustomer()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetDefaultPaymentMethodforCustomer");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetMapService()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetMapService");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetDfltWhs()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetDfltWhs");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetObjectTypePath()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			return await helper.GetAsync("GetObjectTypePath");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> ExistsTransaction()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await helper.GetAsync("ExistsTransaction");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void ChangeCustomerLimit()
	{
		try
		{
			if (Function.ToString(CreditLimt.EditValue) != "Y" && Function.ToString(ObligLimit.EditValue) != "Y")
			{
				SalesLimit.EditValue = "N";
				DlnLimit.EditValue = "N";
				OrderLimit.EditValue = "N";
				PickLimit.EditValue = "N";
				SalesLimit.Enabled = false;
				DlnLimit.Enabled = false;
				OrderLimit.Enabled = false;
				PickLimit.Enabled = false;
			}
			else
			{
				SalesLimit.Enabled = true;
				DlnLimit.Enabled = true;
				OrderLimit.Enabled = true;
				PickLimit.Enabled = true;
			}
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetUpdateText()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (grd_ObjAtt.DataSourceTable.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from dataRow in grd_ObjAtt.DataSourceTable.AsEnumerable()
					where Function.ParseInt(dataRow["ObjType"]) != 0 && Function.ToString(dataRow["Path"]) != ""
					select dataRow;
				if (source.Count() > 0)
				{
					grd_ObjAtt.DataSource = source.CopyToDataTable();
				}
				else
				{
					grd_ObjAtt.DataSourceTable.Rows.Clear();
				}
				grd_ObjAtt.DataSourceTable.AcceptChanges();
				foreach (DataRow dr in grd_ObjAtt.DataSourceTable.AsEnumerable())
				{
					int type = Function.ParseInt(dr["ObjType"]);
					if (type != 0 && grd_ObjAtt.DataSourceTable.AsEnumerable().Any((DataRow t) => t != dr && Function.ParseInt(t["ObjType"]) == type))
					{
						WinCommons.Error(Enums.MessageCode.AObjectTypeCanOnlyHaveOneCustomAttachmentPath, "");
						return;
					}
				}
			}
			WinCommons._OpenProcessing();
			if (dts.IsNotNull())
			{
				for (int num = 0; num < dts.Tables.Count; num++)
				{
					DataTable dataTable = dts.Tables[num];
					if (dataTable.IsNotNull())
					{
						DataRow row = dataTable.NewRow();
						row = GetDataRow(xtraTab_Inventory, row);
						dataTable.Rows.Clear();
						dataTable.AcceptChanges();
						dataTable.Rows.Add(row);
						dataTable.AcceptChanges();
					}
				}
			}
			dts.AcceptChanges();
			DataSet dataSet = dts.Copy();
			if (grd_ObjAtt.DataSourceTable.HasColumn())
			{
				dataSet.Tables.Add(grd_ObjAtt.DataSourceTable.Copy());
			}
			Tuple<DataSet, int> tup = new Tuple<DataSet, int>(dataSet, GlobalData.UserID);
			btn_OK.Enabled = false;
			btn_Cancel.Enabled = false;
			string text = await SaveData(tup);
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				if (RefreshMain != null && !((XtraForm)RefreshMain.Target).IsDisposed)
				{
					RefreshMain();
				}
				await Main.LoadConfigFileSharingUpload();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData(Tuple<DataSet, int> tup)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "GeneralSetting");
			return await helper.PostAsync("SaveData", tup);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private void DfVendTerm_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oPaymentTermsTypes, ApzObjectTypes.oPaymentTermsTypes, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DfCustTerm_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oPaymentTermsTypes, ApzObjectTypes.oPaymentTermsTypes, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pictureBox1_Click(object sender, EventArgs e)
	{
	}

	private void simpleButton9_Click(object sender, EventArgs e)
	{
	}

	private void btn_MicWord_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					WordPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Excel_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					ExcelPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Pic_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					BitmapPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Att_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					AttachPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Extension_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					ExtPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Current_Click(object sender, EventArgs e)
	{
	}

	private void btn_XML_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			if (string.IsNullOrEmpty(XmlPath.Text))
			{
				folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
				if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
				{
					XmlPath.Text = folderBrowserDialogEx.SelectedPath;
				}
				return;
			}
			folderBrowserDialogEx.SelectedPath = XmlPath.Text;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					XmlPath.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_IncomingPayment_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txt_PicBox.Image = new Bitmap(openFileDialog.FileName);
				WallPaper.Text = openFileDialog.FileName;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void textEdit31_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(lok_fontSize.EditValue)))
			{
				string familyName = lok_Fonts.EditValue.ToString();
				int num = Function.ParseInt(lok_fontSize.EditValue);
				Font font = new Font(familyName, num);
				txt_PreviewFont.Font = font;
				AppearanceObject.DefaultFont = new Font(familyName, num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void textEdit33_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void memoExEdit1_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit5_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void gridLookUpEdit3_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit16_CheckedChanged_1(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void textEdit30_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateText();
			string familyName = lok_Fonts.EditValue.ToString();
			if (!string.IsNullOrEmpty(Function.ToString(lok_fontSize.EditValue)))
			{
				int num = Function.ParseInt(lok_fontSize.EditValue);
				Font font = new Font(familyName, num);
				txt_PreviewFont.Font = font;
				AppearanceObject.DefaultFont = new Font(familyName, num);
			}
			else
			{
				Font font2 = new Font(familyName, 12f);
				txt_PreviewFont.Font = font2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkEdit43_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit63_CheckedChanged_1(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void radioGroup5_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit46_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit48_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void textEdit3_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void textEdit6_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void lok_Color_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void textEdit53_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void textEdit49_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit55_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PickLimit_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void txt_CurrentScander_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CreditLimt_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreditLimt_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeCustomerLimit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ObligLimit_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeCustomerLimit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void useDocWrf_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateText();
			if (useDocWrf.EditValue == "N")
			{
				EnbApprDI.EditValue = "N";
				EnbApprDI.Enabled = false;
			}
			else
			{
				EnbApprDI.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BgtBlock_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (BgtBlock.SelectedIndex == 2)
			{
				layoutControlItem24.ContentVisible = false;
				layoutControlItem25.ContentVisible = false;
				layoutControlItem26.ContentVisible = false;
				layoutControlItem27.ContentVisible = false;
				layoutControlItem28.ContentVisible = false;
			}
			else
			{
				layoutControlItem24.ContentVisible = true;
				layoutControlItem25.ContentVisible = true;
				layoutControlItem26.ContentVisible = true;
				layoutControlItem27.ContentVisible = true;
				layoutControlItem28.ContentVisible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DoBudget_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateText();
			if (DoBudget.Checked)
			{
				simpleLabelItem3.Visibility = LayoutVisibility.Always;
				layoutControlItem2.ContentVisible = true;
				if (BgtBlock.SelectedIndex == 2)
				{
					layoutControlItem24.ContentVisible = false;
					layoutControlItem25.ContentVisible = false;
					layoutControlItem26.ContentVisible = false;
					layoutControlItem27.ContentVisible = false;
					layoutControlItem28.ContentVisible = false;
				}
				else
				{
					layoutControlItem24.ContentVisible = true;
					layoutControlItem25.ContentVisible = true;
					layoutControlItem26.ContentVisible = true;
					layoutControlItem27.ContentVisible = true;
					layoutControlItem28.ContentVisible = true;
				}
			}
			else
			{
				simpleLabelItem3.Visibility = LayoutVisibility.Never;
				layoutControlItem2.ContentVisible = false;
				layoutControlItem24.ContentVisible = false;
				layoutControlItem25.ContentVisible = false;
				layoutControlItem26.ContentVisible = false;
				layoutControlItem27.ContentVisible = false;
				layoutControlItem28.ContentVisible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddDlnBlnc_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CreditLimt_CheckedChanged_1(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ObligLimit_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SalesLimit_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void OrderLimit_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SlpCommUse_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ItmCommUse_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CrdCommUse_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void useDocWrf_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void EnbApprDI_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DspFrznBP_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DspIBPDoc_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void MBAOnPer_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfCustTerm_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfVendTerm_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfltVendPM_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CreditDpst_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BgtBlock_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BdgtPRQDOC_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BdgtPORDoc_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BdgtPDNDoc_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BdgtAcctng_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CheckFiles_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DsplyRates_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void RcrFlag_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void RclFlag_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ContactLog_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ShowNewMsg_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void OpenCdt_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void OpenDps_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ShowNewTsk_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ScreenLock_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void OpenCredit_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_UsedProxy_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
		if (ck_UsedProxy.Checked)
		{
			layoutControlItem39.Visibility = LayoutVisibility.Always;
			layoutControlItem40.Visibility = LayoutVisibility.Always;
		}
		else
		{
			layoutControlItem39.Visibility = LayoutVisibility.Never;
			layoutControlItem40.Visibility = LayoutVisibility.Never;
		}
	}

	private void memo_Address_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void memo_Port_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfltByEml_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void memo_Area_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void MapService_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void MaxHistory_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void IMCEEnable_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SPNEnabled_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DefLengthU_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DefWeightU_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void TimeFormat_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DateFormat_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DateSep_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_ManagerCompany_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AdvImagePr_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void MaxChoose_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void TPLId_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void B1SgtEb_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void TxtSrch_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SumDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PriceDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void RateDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void QtyDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PercentDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void MeasureDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void QueryDec_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DecSep_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ThousSep_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CurOnRight_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DirectRate_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void lok_Fonts_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void lok_fontSize_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void WallPaper_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void WordPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ExcelPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BitmapPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AttachPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ExtPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void XmlPath_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SriMngSys_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
		SriCreatIn.Enabled = Function.ToString(SriMngSys.EditValue).Equals("R");
	}

	private void SriUniqFld_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void checkEdit37_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void BtchStatus_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void OneBOneRec_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfltWhs_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AutoITW_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void WTLiable_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AutoAddUoM_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AutoAddPkg_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DspFrznITM_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ConsumeFCT_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ConsumeMtd_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DaysBack_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DaysFwrd_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SimReport_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DftResWhs_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void AutoResWhs_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void StartFrom_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void Months_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void Days_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void CfwOutDflt_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void IgrAllCash_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PrsWkCntEb_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (PrsWkCntEb.SelectedIndex == 0)
			{
				btn_More.Enabled = true;
			}
			else
			{
				btn_More.Enabled = false;
			}
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void MDStyle_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PrjBlock_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(PrjBlock.EditValue) == "B")
			{
				ApyDRinPEC.Visible = true;
				ApyPRinPEC.Visible = true;
			}
			else
			{
				ApyDRinPEC.Visible = false;
				ApyPRinPEC.Visible = false;
			}
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ApyDRinPEC_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(PrjBlock.EditValue) == "B")
			{
				SetUpdateText();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ApyPRinPEC_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(PrjBlock.EditValue) == "B")
			{
				SetUpdateText();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
		}
	}

	private void ApyBsActPV_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ApyBsActPL_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ClnZeroPln_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void PriceProcM_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void IsUpdNstdB_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SIPLReport_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SIPLDoc_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void SIPLSeting_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_PaymentWizard_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_DunningWizard_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_CostCounting_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_SerialNum_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck__CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_MRP_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void ck_UnitOfMeasure_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfSVatItem_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfSVatServ_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfPVatItem_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void DfPVatServ_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateText();
	}

	private void bte_Path_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
			{
				string text = folderBrowserDialogEx.SelectedPath;
				if (!text[text.Length - 1].Equals('\\'))
				{
					text += "\\";
				}
				(sender as ButtonEdit).EditValue = text;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_ObjAtt_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetUpdateText();
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_ObjAtt.FocusedRowHandle != -2147483647)
			{
				gv_ObjAtt.BeginUpdate();
				gv_ObjAtt.DeleteSelectedRows();
				gv_ObjAtt.EndUpdate();
				SetUpdateText();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DisableSplitFolder_CheckedChanged(object sender, EventArgs e)
	{
		SetUpdateText();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_GeneSetting));
		this.xtraTab_Inventory = new Apzon.Controls.Controls.XtraTabControlEx();
		this.Tab_BP = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.MBAOnPer = new Apzon.Controls.Editor.CheckEditEx();
		this.DspIBPDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.DspFrznBP = new Apzon.Controls.Editor.CheckEditEx();
		this.EnbApprDI = new Apzon.Controls.Editor.CheckEditEx();
		this.useDocWrf = new Apzon.Controls.Editor.CheckEditEx();
		this.CrdCommUse = new Apzon.Controls.Editor.CheckEditEx();
		this.ItmCommUse = new Apzon.Controls.Editor.CheckEditEx();
		this.SlpCommUse = new Apzon.Controls.Editor.CheckEditEx();
		this.PickLimit = new Apzon.Controls.Editor.CheckEditEx();
		this.OrderLimit = new Apzon.Controls.Editor.CheckEditEx();
		this.DlnLimit = new Apzon.Controls.Editor.CheckEditEx();
		this.SalesLimit = new Apzon.Controls.Editor.CheckEditEx();
		this.CreditLimt = new Apzon.Controls.Editor.CheckEditEx();
		this.ObligLimit = new Apzon.Controls.Editor.CheckEditEx();
		this.AddDlnBlnc = new Apzon.Controls.Editor.CheckEditEx();
		this.DfCustTerm = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfVendTerm = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfltCustPM = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfltVendPM = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.CreditDpst = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem4 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem5 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem6 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem7 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem8 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem9 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem10 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem11 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem13 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem14 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem15 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem16 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem17 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem19 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem20 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem21 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem22 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem23 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem2 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem12 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Budget = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
		this.BdgtAcctng = new Apzon.Controls.Editor.CheckEditEx();
		this.BdgtPDNDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.BdgtPORDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.BdgtPRQDOC = new Apzon.Controls.Editor.CheckEditEx();
		this.BgtWarning = new Apzon.Controls.Editor.RadioGroupEx();
		this.BgtBlock = new Apzon.Controls.Editor.RadioGroupEx();
		this.DoBudget = new Apzon.Controls.Editor.CheckEditEx();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem3 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem2 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem11 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem12 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem24 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem13 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem14 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem15 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem16 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem25 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem26 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem27 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem28 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Services = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
		this.label4 = new Apzon.Controls.Controls.LabelLine();
		this.label2 = new Apzon.Controls.Controls.LabelLine();
		this.label1 = new Apzon.Controls.Controls.LabelLine();
		this.SPNEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.IMCEEnable = new Apzon.Controls.Editor.CheckEditEx();
		this.MaxHistory = new Apzon.Controls.Editor.TextEditEx();
		this.ScreenLock = new Apzon.Controls.Editor.TextEditEx();
		this.AlertPolFr = new Apzon.Controls.Editor.TextEditEx();
		this.DfltByEml = new Apzon.Controls.Editor.RadioGroupEx();
		this.ck_UsedProxy = new Apzon.Controls.Editor.CheckEditEx();
		this.ShowNewTsk = new Apzon.Controls.Editor.CheckEditEx();
		this.OpenDps = new Apzon.Controls.Editor.CheckEditEx();
		this.OpenCdt = new Apzon.Controls.Editor.CheckEditEx();
		this.ShowNewMsg = new Apzon.Controls.Editor.CheckEditEx();
		this.ContactLog = new Apzon.Controls.Editor.CheckEditEx();
		this.RclFlag = new Apzon.Controls.Editor.CheckEditEx();
		this.RcrFlag = new Apzon.Controls.Editor.CheckEditEx();
		this.DsplyRates = new Apzon.Controls.Editor.CheckEditEx();
		this.CheckFiles = new Apzon.Controls.Editor.CheckEditEx();
		this.OpenCredit = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.MapService = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.memo_Address = new DevExpress.XtraEditors.MemoExEdit();
		this.memo_Port = new DevExpress.XtraEditors.MemoExEdit();
		this.memo_Area = new DevExpress.XtraEditors.MemoExEdit();
		this.txt_ForEx = new DevExpress.XtraEditors.MemoExEdit();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem17 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem18 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem19 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem20 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem21 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem22 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem4 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem29 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem30 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem31 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem32 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem33 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem34 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem35 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem36 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem37 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem38 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem39 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem40 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem23 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem41 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem24 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem42 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem43 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem44 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem45 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem46 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem47 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem48 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem49 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem25 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem50 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem26 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem152 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem153 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem154 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Display = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
		this.TxtSrch = new Apzon.Controls.Editor.CheckEditEx();
		this.B1SgtEb = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_ManagerCompany = new Apzon.Controls.Editor.CheckEditEx();
		this.DirectRate = new Apzon.Controls.Editor.RadioGroupEx();
		this.CurOnRight = new Apzon.Controls.Editor.CheckEditEx();
		this.ThousSep = new Apzon.Controls.Editor.TextEditEx();
		this.DecSep = new Apzon.Controls.Editor.TextEditEx();
		this.QueryDec = new Apzon.Controls.Editor.TextEditEx();
		this.MeasureDec = new Apzon.Controls.Editor.TextEditEx();
		this.PercentDec = new Apzon.Controls.Editor.TextEditEx();
		this.QtyDec = new Apzon.Controls.Editor.TextEditEx();
		this.RateDec = new Apzon.Controls.Editor.TextEditEx();
		this.SumDec = new Apzon.Controls.Editor.TextEditEx();
		this.MaxChoose = new Apzon.Controls.Editor.TextEditEx();
		this.AdvImagePr = new Apzon.Controls.Editor.TextEditEx();
		this.DateSep = new Apzon.Controls.Editor.TextEditEx();
		this.Color = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DefLengthU = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DefWeightU = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.TimeFormat = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DateFormat = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.TPLId = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.PriceDec = new Apzon.Controls.Editor.CalcEditEx();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem27 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem28 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem29 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem8 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem53 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem54 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem55 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem56 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem57 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem58 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem32 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem59 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem60 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem61 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem62 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem63 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem64 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem65 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem66 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem67 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem68 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem69 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem70 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem33 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem9 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem71 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem72 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem10 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem73 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem74 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem75 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem35 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem71 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_Font = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
		this.txt_PicBox = new System.Windows.Forms.PictureBox();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.WllPprDsp = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lok_Fonts = new DevExpress.XtraEditors.FontEdit();
		this.lok_fontSize = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_PreviewFont = new Apzon.Controls.Editor.MemoEditEx();
		this.WallPaper = new Apzon.Controls.Editor.TextEditEx();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem76 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem77 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem78 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem79 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem40 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem11 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.simpleLabelItem12 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem80 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem81 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem38 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem37 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem82 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem42 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_Path = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
		this.DisableSplitFolder = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_ObjAtt = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ObjAtt = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ObjType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_Path = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cbo_ObjType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.btn_XML = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Current = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Extension = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Att = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Pic = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Excel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_MicWord = new Apzon.Controls.Controls.SimpleButtonEx();
		this.XmlPath = new Apzon.Controls.Editor.TextEditEx();
		this.txt_CurrentScander = new Apzon.Controls.Editor.TextEditEx();
		this.ExtPath = new Apzon.Controls.Editor.TextEditEx();
		this.AttachPath = new Apzon.Controls.Editor.TextEditEx();
		this.BitmapPath = new Apzon.Controls.Editor.TextEditEx();
		this.ExcelPath = new Apzon.Controls.Editor.TextEditEx();
		this.WordPath = new Apzon.Controls.Editor.TextEditEx();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem44 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem84 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem85 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem89 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem90 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem93 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem94 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem95 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem96 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem83 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem91 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem92 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem86 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem87 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem88 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem43 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem52 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem97 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Inventory = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.Tab_Item = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
		this.OneBOneRec = new Apzon.Controls.Editor.CheckEditEx();
		this.SriCreatIn = new Apzon.Controls.Editor.CheckEditEx();
		this.DspFrznITM = new Apzon.Controls.Editor.CheckEditEx();
		this.DspIITMDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.AutoAddPkg = new Apzon.Controls.Editor.CheckEditEx();
		this.AutoAddUoM = new Apzon.Controls.Editor.CheckEditEx();
		this.WTLiable = new Apzon.Controls.Editor.CheckEditEx();
		this.AutoITW = new Apzon.Controls.Editor.CheckEditEx();
		this.AutoCrIns = new Apzon.Controls.Editor.CheckEditEx();
		this.SriMngSys = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.SriUniqFld = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.BtchStatus = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfltWhs = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.GLMethod = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem48 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem50 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem51 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem13 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem99 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem100 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem101 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem102 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem103 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem53 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem105 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem106 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem54 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem107 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem14 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.emptySpaceItem55 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem108 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem109 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem98 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem142 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem36 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem143 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem144 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Plan = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl14 = new DevExpress.XtraLayout.LayoutControl();
		this.DaysBack = new Apzon.Controls.Editor.TextEditEx();
		this.DaysFwrd = new Apzon.Controls.Editor.TextEditEx();
		this.ConsumeFCT = new Apzon.Controls.Editor.CheckEditEx();
		this.ConsumeMtd = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup13 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem39 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem104 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem145 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem146 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem52 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem141 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem79 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem82 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl15 = new DevExpress.XtraLayout.LayoutControl();
		this.SimReport = new Apzon.Controls.Editor.RadioGroupEx();
		this.layoutControlGroup14 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem49 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem24 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem147 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem83 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_Resources = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl8 = new DevExpress.XtraLayout.LayoutControl();
		this.Days = new Apzon.Controls.Editor.TextEditEx();
		this.Months = new Apzon.Controls.Editor.TextEditEx();
		this.AutoResWhs = new Apzon.Controls.Editor.CheckEditEx();
		this.DftResWhs = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.StartFrom = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem56 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem57 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem110 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem111 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem15 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem112 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem113 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem114 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem58 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_CashFlow = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl9 = new DevExpress.XtraLayout.LayoutControl();
		this.IgrAllCash = new Apzon.Controls.Editor.RadioGroupEx();
		this.CfwAsnMust = new Apzon.Controls.Editor.RadioGroupEx();
		this.CfwInDflt = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.CfwOutDflt = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem59 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem60 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem16 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem115 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem61 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem17 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem116 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem117 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem18 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.emptySpaceItem62 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem63 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem118 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Cockpit = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl10 = new DevExpress.XtraLayout.LayoutControl();
		this.btn_More = new Apzon.Controls.Controls.SimpleButtonEx();
		this.PrsWkCntEb = new Apzon.Controls.Editor.RadioGroupEx();
		this.layoutControlGroup9 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem64 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem65 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem119 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem66 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem120 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem67 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem68 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_CostAccounting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl11 = new DevExpress.XtraLayout.LayoutControl();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ApyPRinPEC = new Apzon.Controls.Editor.CheckEditEx();
		this.ApyDRinPEC = new Apzon.Controls.Editor.CheckEditEx();
		this.label3 = new Apzon.Controls.Controls.LabelLine();
		this.MDStyle = new Apzon.Controls.Editor.RadioGroupEx();
		this.UseMltDims = new Apzon.Controls.Editor.CheckEditEx();
		this.PrjBlock = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem121 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem70 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleLabelItem19 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem124 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem72 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem73 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem85 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem86 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem87 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem122 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem148 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem133 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem155 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.simpleLabelItem5 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem18 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem30 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem31 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.Tab_Pricing = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl12 = new DevExpress.XtraLayout.LayoutControl();
		this.label6 = new Apzon.Controls.Controls.LabelLine();
		this.label5 = new Apzon.Controls.Controls.LabelLine();
		this.SIPLSeting = new Apzon.Controls.Editor.CheckEditEx();
		this.SIPLDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.SIPLReport = new Apzon.Controls.Editor.CheckEditEx();
		this.IsUpdNstdB = new Apzon.Controls.Editor.CheckEditEx();
		this.ClnZeroPln = new Apzon.Controls.Editor.CheckEditEx();
		this.ApyBsActPL = new Apzon.Controls.Editor.CheckEditEx();
		this.ApyBsActSP = new Apzon.Controls.Editor.CheckEditEx();
		this.ApyBsActPV = new Apzon.Controls.Editor.CheckEditEx();
		this.PriceProcM = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.layoutControlGroup11 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem75 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem76 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem125 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem126 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem127 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem77 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem128 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem78 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem129 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem130 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem131 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem132 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.ck_Setting = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem150 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem151 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_HideFunctions = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl13 = new DevExpress.XtraLayout.LayoutControl();
		this.ck_UnitOfMeasure = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_MRP = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_ = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_SerialNum = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_CostCounting = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_DunningWizard = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_PaymentWizard = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Budget = new Apzon.Controls.Editor.CheckEditEx();
		this.layoutControlGroup12 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.simpleLabelItem23 = new DevExpress.XtraLayout.SimpleLabelItem();
		this.layoutControlItem134 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem135 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem136 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem137 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem138 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem139 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem140 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem149 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.Tab_Sales = new Apzon.Controls.Controls.XtraTabPageEx();
		this.DfSVatServ = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfSVatItem = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.luk_DfltCard = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.Tab_Purchasing = new Apzon.Controls.Controls.XtraTabPageEx();
		this.DfPVatServ = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.DfPVatItem = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
		this.emptySpaceItem74 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem84 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
		this.layoutControlItem51 = new Apzon.Controls.Controls.LayoutControlItemEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Inventory).BeginInit();
		this.xtraTab_Inventory.SuspendLayout();
		this.Tab_BP.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.MBAOnPer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DspIBPDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DspFrznBP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.EnbApprDI.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.useDocWrf.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CrdCommUse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItmCommUse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SlpCommUse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PickLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OrderLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DlnLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalesLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CreditLimt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ObligLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AddDlnBlnc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfCustTerm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfVendTerm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfltCustPM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfltVendPM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CreditDpst.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).BeginInit();
		this.Tab_Budget.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.BdgtAcctng.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPDNDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPORDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPRQDOC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BgtWarning.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BgtBlock.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DoBudget.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem28).BeginInit();
		this.Tab_Services.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).BeginInit();
		this.layoutControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SPNEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.IMCEEnable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MaxHistory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ScreenLock.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AlertPolFr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfltByEml.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_UsedProxy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ShowNewTsk.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OpenDps.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OpenCdt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ShowNewMsg.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ContactLog.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RclFlag.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RcrFlag.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DsplyRates.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CheckFiles.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OpenCredit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MapService.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Address.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Port.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Area.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ForEx.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem39).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem41).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem42).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem46).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem47).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem49).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem50).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem152).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem153).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem154).BeginInit();
		this.Tab_Display.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).BeginInit();
		this.layoutControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.TxtSrch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.B1SgtEb.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_ManagerCompany.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DirectRate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CurOnRight.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ThousSep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DecSep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.QueryDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MeasureDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PercentDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.QtyDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RateDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SumDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MaxChoose.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AdvImagePr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DateSep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Color.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DefLengthU.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DefWeightU.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.TimeFormat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DateFormat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.TPLId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PriceDec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem53).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem54).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem55).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem56).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem58).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem59).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem60).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem61).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem62).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem63).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem64).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem65).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem66).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem67).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem68).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem69).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem71).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem72).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem73).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem74).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem75).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).BeginInit();
		this.Tab_Font.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl5).BeginInit();
		this.layoutControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_PicBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WllPprDsp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lok_Fonts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lok_fontSize.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PreviewFont.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WallPaper.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem76).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem77).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem78).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem79).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem80).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem81).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem82).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).BeginInit();
		this.Tab_Path.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl6).BeginInit();
		this.layoutControl6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.DisableSplitFolder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ObjAtt).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ObjAtt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ObjType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Path).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbo_ObjType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.XmlPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CurrentScander.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ExtPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AttachPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BitmapPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ExcelPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WordPath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem84).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem85).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem89).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem90).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem93).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem94).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem95).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem96).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem83).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem91).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem92).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem86).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem87).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem88).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem52).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem97).BeginInit();
		this.Tab_Inventory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.Tab_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl7).BeginInit();
		this.layoutControl7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.OneBOneRec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SriCreatIn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DspFrznITM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DspIITMDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutoAddPkg.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutoAddUoM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WTLiable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutoITW.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutoCrIns.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SriMngSys.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SriUniqFld.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BtchStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfltWhs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.GLMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem99).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem100).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem101).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem102).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem103).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem105).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem106).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem107).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem108).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem109).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem98).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem142).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem143).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem144).BeginInit();
		this.Tab_Plan.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl14).BeginInit();
		this.layoutControl14.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.DaysBack.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DaysFwrd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ConsumeFCT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ConsumeMtd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem104).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem145).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem146).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem141).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem79).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem82).BeginInit();
		this.Tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl15).BeginInit();
		this.layoutControl15.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SimReport.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem147).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem83).BeginInit();
		this.Tab_Resources.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl8).BeginInit();
		this.layoutControl8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Days.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Months.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutoResWhs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DftResWhs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.StartFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem110).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem111).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem112).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem113).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem114).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).BeginInit();
		this.Tab_CashFlow.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl9).BeginInit();
		this.layoutControl9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.IgrAllCash.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CfwAsnMust.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CfwInDflt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CfwOutDflt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem59).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem115).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem61).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem116).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem117).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem63).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem118).BeginInit();
		this.Tab_Cockpit.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl10).BeginInit();
		this.layoutControl10.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PrsWkCntEb.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem119).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem120).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem67).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).BeginInit();
		this.Tab_CostAccounting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).BeginInit();
		this.layoutControl11.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ApyPRinPEC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ApyDRinPEC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MDStyle.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UseMltDims.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PrjBlock.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem121).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem124).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem72).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem85).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem86).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem87).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem122).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem148).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem133).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem155).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).BeginInit();
		this.Tab_Pricing.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl12).BeginInit();
		this.layoutControl12.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SIPLSeting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SIPLDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SIPLReport.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.IsUpdNstdB.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ClnZeroPln.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActPL.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActSP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActPV.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PriceProcM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem75).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem125).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem126).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem127).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem128).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem78).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem129).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem130).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem131).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem132).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Setting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem150).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem151).BeginInit();
		this.Tab_HideFunctions.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl13).BeginInit();
		this.layoutControl13.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_UnitOfMeasure.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_MRP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_SerialNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_CostCounting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_DunningWizard.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_PaymentWizard.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Budget.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem134).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem135).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem136).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem137).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem138).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem139).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem140).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem149).BeginInit();
		this.Tab_Sales.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.DfSVatServ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfSVatItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DfltCard.Properties).BeginInit();
		this.Tab_Purchasing.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.DfPVatServ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DfPVatItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem84).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem51).BeginInit();
		base.SuspendLayout();
		this.xtraTab_Inventory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTab_Inventory.IsSearchControl = false;
		this.xtraTab_Inventory.Location = new System.Drawing.Point(2, -2);
		this.xtraTab_Inventory.Name = "xtraTab_Inventory";
		this.xtraTab_Inventory.SelectedTabPage = this.Tab_BP;
		this.xtraTab_Inventory.Size = new System.Drawing.Size(878, 477);
		this.xtraTab_Inventory.TabIndex = 3;
		this.xtraTab_Inventory.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[15]
		{
			this.Tab_BP, this.Tab_Budget, this.Tab_Services, this.Tab_Display, this.Tab_Font, this.Tab_Path, this.Tab_Inventory, this.Tab_Resources, this.Tab_CashFlow, this.Tab_Cockpit,
			this.Tab_CostAccounting, this.Tab_Pricing, this.Tab_HideFunctions, this.Tab_Sales, this.Tab_Purchasing
		});
		this.Tab_BP.Controls.Add(this.layoutControl1);
		this.Tab_BP.EditControl = null;
		this.Tab_BP.EditValue = "BP";
		this.Tab_BP.FieldName = "Tab_BP";
		this.Tab_BP.IsSearchControl = false;
		this.Tab_BP.Name = "Tab_BP";
		this.Tab_BP.ReadOnly = false;
		this.Tab_BP.SettingInVisible = false;
		this.Tab_BP.SettingReadonly = false;
		this.Tab_BP.Size = new System.Drawing.Size(876, 452);
		this.Tab_BP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_BP.Text = "BP";
		this.layoutControl1.Controls.Add(this.MBAOnPer);
		this.layoutControl1.Controls.Add(this.DspIBPDoc);
		this.layoutControl1.Controls.Add(this.DspFrznBP);
		this.layoutControl1.Controls.Add(this.EnbApprDI);
		this.layoutControl1.Controls.Add(this.useDocWrf);
		this.layoutControl1.Controls.Add(this.CrdCommUse);
		this.layoutControl1.Controls.Add(this.ItmCommUse);
		this.layoutControl1.Controls.Add(this.SlpCommUse);
		this.layoutControl1.Controls.Add(this.PickLimit);
		this.layoutControl1.Controls.Add(this.OrderLimit);
		this.layoutControl1.Controls.Add(this.DlnLimit);
		this.layoutControl1.Controls.Add(this.SalesLimit);
		this.layoutControl1.Controls.Add(this.CreditLimt);
		this.layoutControl1.Controls.Add(this.ObligLimit);
		this.layoutControl1.Controls.Add(this.AddDlnBlnc);
		this.layoutControl1.Controls.Add(this.DfCustTerm);
		this.layoutControl1.Controls.Add(this.DfVendTerm);
		this.layoutControl1.Controls.Add(this.DfltCustPM);
		this.layoutControl1.Controls.Add(this.DfltVendPM);
		this.layoutControl1.Controls.Add(this.CreditDpst);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(564, 262, 251, 347);
		this.layoutControl1.Root = this.Root;
		this.layoutControl1.Size = new System.Drawing.Size(876, 452);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.MBAOnPer.EditControl = null;
		this.MBAOnPer.FieldName = null;
		this.MBAOnPer.IsSearchControl = false;
		this.MBAOnPer.Location = new System.Drawing.Point(200, 420);
		this.MBAOnPer.Name = "MBAOnPer";
		this.MBAOnPer.Properties.Caption = "Allow Multiple Blanket Agreements for Same Period";
		this.MBAOnPer.Properties.DisplayValueChecked = "Y";
		this.MBAOnPer.Properties.DisplayValueUnchecked = "N";
		this.MBAOnPer.Properties.ValueChecked = "Y";
		this.MBAOnPer.Properties.ValueGrayed = "";
		this.MBAOnPer.Properties.ValueUnchecked = "N";
		this.MBAOnPer.SettingInVisible = false;
		this.MBAOnPer.SettingReadonly = false;
		this.MBAOnPer.Size = new System.Drawing.Size(266, 20);
		this.MBAOnPer.StyleController = this.layoutControl1;
		this.MBAOnPer.TabIndex = 26;
		this.MBAOnPer.Visible = false;
		this.MBAOnPer.CheckedChanged += new System.EventHandler(this.MBAOnPer_CheckedChanged);
		this.DspIBPDoc.EditControl = null;
		this.DspIBPDoc.FieldName = null;
		this.DspIBPDoc.IsSearchControl = false;
		this.DspIBPDoc.Location = new System.Drawing.Point(12, 327);
		this.DspIBPDoc.Name = "DspIBPDoc";
		this.DspIBPDoc.Properties.Caption = "Display Inactive Business Partners in Marketing Documents";
		this.DspIBPDoc.Properties.DisplayValueChecked = "Y";
		this.DspIBPDoc.Properties.DisplayValueUnchecked = "N";
		this.DspIBPDoc.Properties.ValueChecked = "Y";
		this.DspIBPDoc.Properties.ValueGrayed = "";
		this.DspIBPDoc.Properties.ValueUnchecked = "N";
		this.DspIBPDoc.SettingInVisible = false;
		this.DspIBPDoc.SettingReadonly = false;
		this.DspIBPDoc.Size = new System.Drawing.Size(454, 20);
		this.DspIBPDoc.StyleController = this.layoutControl1;
		this.DspIBPDoc.TabIndex = 25;
		this.DspIBPDoc.CheckedChanged += new System.EventHandler(this.DspIBPDoc_CheckedChanged);
		this.DspFrznBP.EditControl = null;
		this.DspFrznBP.FieldName = null;
		this.DspFrznBP.IsSearchControl = false;
		this.DspFrznBP.Location = new System.Drawing.Point(12, 303);
		this.DspFrznBP.Name = "DspFrznBP";
		this.DspFrznBP.Properties.Caption = "Display Inactive Business Partners in Reports";
		this.DspFrznBP.Properties.DisplayValueChecked = "Y";
		this.DspFrznBP.Properties.DisplayValueUnchecked = "N";
		this.DspFrznBP.Properties.ValueChecked = "Y";
		this.DspFrznBP.Properties.ValueGrayed = "";
		this.DspFrznBP.Properties.ValueUnchecked = "N";
		this.DspFrznBP.SettingInVisible = false;
		this.DspFrznBP.SettingReadonly = false;
		this.DspFrznBP.Size = new System.Drawing.Size(454, 20);
		this.DspFrznBP.StyleController = this.layoutControl1;
		this.DspFrznBP.TabIndex = 24;
		this.DspFrznBP.CheckedChanged += new System.EventHandler(this.DspFrznBP_CheckedChanged);
		this.EnbApprDI.EditControl = null;
		this.EnbApprDI.FieldName = null;
		this.EnbApprDI.IsSearchControl = false;
		this.EnbApprDI.Location = new System.Drawing.Point(36, 279);
		this.EnbApprDI.Name = "EnbApprDI";
		this.EnbApprDI.Properties.Caption = "Activate Approval Procedures in DI";
		this.EnbApprDI.Properties.DisplayValueChecked = "Y";
		this.EnbApprDI.Properties.DisplayValueUnchecked = "N";
		this.EnbApprDI.Properties.ValueChecked = "Y";
		this.EnbApprDI.Properties.ValueGrayed = "";
		this.EnbApprDI.Properties.ValueUnchecked = "N";
		this.EnbApprDI.SettingInVisible = false;
		this.EnbApprDI.SettingReadonly = false;
		this.EnbApprDI.Size = new System.Drawing.Size(430, 20);
		this.EnbApprDI.StyleController = this.layoutControl1;
		this.EnbApprDI.TabIndex = 23;
		this.EnbApprDI.CheckedChanged += new System.EventHandler(this.EnbApprDI_CheckedChanged);
		this.useDocWrf.EditControl = null;
		this.useDocWrf.FieldName = null;
		this.useDocWrf.IsSearchControl = false;
		this.useDocWrf.Location = new System.Drawing.Point(12, 255);
		this.useDocWrf.Name = "useDocWrf";
		this.useDocWrf.Properties.Caption = "Activate Approval Procedures";
		this.useDocWrf.Properties.DisplayValueChecked = "Y";
		this.useDocWrf.Properties.DisplayValueUnchecked = "N";
		this.useDocWrf.Properties.ValueChecked = "Y";
		this.useDocWrf.Properties.ValueGrayed = "";
		this.useDocWrf.Properties.ValueUnchecked = "N";
		this.useDocWrf.SettingInVisible = false;
		this.useDocWrf.SettingReadonly = false;
		this.useDocWrf.Size = new System.Drawing.Size(454, 20);
		this.useDocWrf.StyleController = this.layoutControl1;
		this.useDocWrf.TabIndex = 22;
		this.useDocWrf.CheckedChanged += new System.EventHandler(this.useDocWrf_CheckedChanged);
		this.useDocWrf.EditValueChanged += new System.EventHandler(this.useDocWrf_EditValueChanged);
		this.CrdCommUse.EditControl = null;
		this.CrdCommUse.FieldName = null;
		this.CrdCommUse.IsSearchControl = false;
		this.CrdCommUse.Location = new System.Drawing.Point(461, 77);
		this.CrdCommUse.Name = "CrdCommUse";
		this.CrdCommUse.Properties.Caption = "Customers";
		this.CrdCommUse.Properties.DisplayValueChecked = "Y";
		this.CrdCommUse.Properties.DisplayValueUnchecked = "N";
		this.CrdCommUse.Properties.ValueChecked = "Y";
		this.CrdCommUse.Properties.ValueGrayed = "";
		this.CrdCommUse.Properties.ValueUnchecked = "N";
		this.CrdCommUse.SettingInVisible = false;
		this.CrdCommUse.SettingReadonly = false;
		this.CrdCommUse.Size = new System.Drawing.Size(205, 20);
		this.CrdCommUse.StyleController = this.layoutControl1;
		this.CrdCommUse.TabIndex = 15;
		this.CrdCommUse.CheckedChanged += new System.EventHandler(this.CrdCommUse_CheckedChanged);
		this.ItmCommUse.EditControl = null;
		this.ItmCommUse.FieldName = null;
		this.ItmCommUse.IsSearchControl = false;
		this.ItmCommUse.Location = new System.Drawing.Point(461, 53);
		this.ItmCommUse.Name = "ItmCommUse";
		this.ItmCommUse.Properties.Caption = "Items";
		this.ItmCommUse.Properties.DisplayValueChecked = "Y";
		this.ItmCommUse.Properties.DisplayValueUnchecked = "N";
		this.ItmCommUse.Properties.ValueChecked = "Y";
		this.ItmCommUse.Properties.ValueGrayed = "";
		this.ItmCommUse.Properties.ValueUnchecked = "N";
		this.ItmCommUse.SettingInVisible = false;
		this.ItmCommUse.SettingReadonly = false;
		this.ItmCommUse.Size = new System.Drawing.Size(205, 20);
		this.ItmCommUse.StyleController = this.layoutControl1;
		this.ItmCommUse.TabIndex = 14;
		this.ItmCommUse.CheckedChanged += new System.EventHandler(this.ItmCommUse_CheckedChanged);
		this.SlpCommUse.EditControl = null;
		this.SlpCommUse.FieldName = null;
		this.SlpCommUse.IsSearchControl = false;
		this.SlpCommUse.Location = new System.Drawing.Point(461, 29);
		this.SlpCommUse.Name = "SlpCommUse";
		this.SlpCommUse.Properties.Caption = "Sales Emplo&yees";
		this.SlpCommUse.Properties.DisplayValueChecked = "Y";
		this.SlpCommUse.Properties.DisplayValueUnchecked = "N";
		this.SlpCommUse.Properties.ValueChecked = "Y";
		this.SlpCommUse.Properties.ValueGrayed = "";
		this.SlpCommUse.Properties.ValueUnchecked = "N";
		this.SlpCommUse.SettingInVisible = false;
		this.SlpCommUse.SettingReadonly = false;
		this.SlpCommUse.Size = new System.Drawing.Size(205, 20);
		this.SlpCommUse.StyleController = this.layoutControl1;
		this.SlpCommUse.TabIndex = 13;
		this.SlpCommUse.CheckedChanged += new System.EventHandler(this.SlpCommUse_CheckedChanged);
		this.PickLimit.EditControl = null;
		this.PickLimit.FieldName = null;
		this.PickLimit.IsSearchControl = false;
		this.PickLimit.Location = new System.Drawing.Point(252, 101);
		this.PickLimit.Name = "PickLimit";
		this.PickLimit.Properties.Caption = "Pick List";
		this.PickLimit.Properties.DisplayValueChecked = "Y";
		this.PickLimit.Properties.DisplayValueUnchecked = "N";
		this.PickLimit.Properties.ValueChecked = "Y";
		this.PickLimit.Properties.ValueGrayed = "";
		this.PickLimit.Properties.ValueUnchecked = "N";
		this.PickLimit.SettingInVisible = false;
		this.PickLimit.SettingReadonly = false;
		this.PickLimit.Size = new System.Drawing.Size(205, 20);
		this.PickLimit.StyleController = this.layoutControl1;
		this.PickLimit.TabIndex = 12;
		this.PickLimit.CheckedChanged += new System.EventHandler(this.PickLimit_CheckedChanged);
		this.OrderLimit.EditControl = null;
		this.OrderLimit.FieldName = null;
		this.OrderLimit.IsSearchControl = false;
		this.OrderLimit.Location = new System.Drawing.Point(252, 77);
		this.OrderLimit.Name = "OrderLimit";
		this.OrderLimit.Properties.Caption = "Sales Order";
		this.OrderLimit.Properties.DisplayValueChecked = "Y";
		this.OrderLimit.Properties.DisplayValueUnchecked = "N";
		this.OrderLimit.Properties.ValueChecked = "Y";
		this.OrderLimit.Properties.ValueGrayed = "";
		this.OrderLimit.Properties.ValueUnchecked = "N";
		this.OrderLimit.SettingInVisible = false;
		this.OrderLimit.SettingReadonly = false;
		this.OrderLimit.Size = new System.Drawing.Size(205, 20);
		this.OrderLimit.StyleController = this.layoutControl1;
		this.OrderLimit.TabIndex = 11;
		this.OrderLimit.CheckedChanged += new System.EventHandler(this.OrderLimit_CheckedChanged);
		this.DlnLimit.EditControl = null;
		this.DlnLimit.FieldName = null;
		this.DlnLimit.IsSearchControl = false;
		this.DlnLimit.Location = new System.Drawing.Point(252, 53);
		this.DlnLimit.Name = "DlnLimit";
		this.DlnLimit.Properties.Caption = "De&livery";
		this.DlnLimit.Properties.DisplayValueChecked = "Y";
		this.DlnLimit.Properties.DisplayValueUnchecked = "N";
		this.DlnLimit.Properties.ValueChecked = "Y";
		this.DlnLimit.Properties.ValueGrayed = "";
		this.DlnLimit.Properties.ValueUnchecked = "N";
		this.DlnLimit.SettingInVisible = false;
		this.DlnLimit.SettingReadonly = false;
		this.DlnLimit.Size = new System.Drawing.Size(205, 20);
		this.DlnLimit.StyleController = this.layoutControl1;
		this.DlnLimit.TabIndex = 10;
		this.DlnLimit.CheckedChanged += new System.EventHandler(this.checkEdit5_CheckedChanged);
		this.SalesLimit.EditControl = null;
		this.SalesLimit.FieldName = null;
		this.SalesLimit.IsSearchControl = false;
		this.SalesLimit.Location = new System.Drawing.Point(252, 29);
		this.SalesLimit.Name = "SalesLimit";
		this.SalesLimit.Properties.Caption = "A/R Invoice";
		this.SalesLimit.Properties.DisplayValueChecked = "Y";
		this.SalesLimit.Properties.DisplayValueUnchecked = "N";
		this.SalesLimit.Properties.ValueChecked = "Y";
		this.SalesLimit.Properties.ValueGrayed = "";
		this.SalesLimit.Properties.ValueUnchecked = "N";
		this.SalesLimit.SettingInVisible = false;
		this.SalesLimit.SettingReadonly = false;
		this.SalesLimit.Size = new System.Drawing.Size(205, 20);
		this.SalesLimit.StyleController = this.layoutControl1;
		this.SalesLimit.TabIndex = 9;
		this.SalesLimit.CheckedChanged += new System.EventHandler(this.SalesLimit_CheckedChanged);
		this.CreditLimt.EditControl = null;
		this.CreditLimt.FieldName = null;
		this.CreditLimt.IsSearchControl = false;
		this.CreditLimt.Location = new System.Drawing.Point(12, 29);
		this.CreditLimt.Name = "CreditLimt";
		this.CreditLimt.Properties.Caption = "Credit Limit";
		this.CreditLimt.Properties.DisplayValueChecked = "Y";
		this.CreditLimt.Properties.DisplayValueUnchecked = "N";
		this.CreditLimt.Properties.ValueChecked = "Y";
		this.CreditLimt.Properties.ValueGrayed = "";
		this.CreditLimt.Properties.ValueUnchecked = "N";
		this.CreditLimt.SettingInVisible = false;
		this.CreditLimt.SettingReadonly = false;
		this.CreditLimt.Size = new System.Drawing.Size(236, 20);
		this.CreditLimt.StyleController = this.layoutControl1;
		this.CreditLimt.TabIndex = 8;
		this.CreditLimt.CheckedChanged += new System.EventHandler(this.CreditLimt_CheckedChanged_1);
		this.CreditLimt.EditValueChanged += new System.EventHandler(this.CreditLimt_EditValueChanged);
		this.ObligLimit.EditControl = null;
		this.ObligLimit.FieldName = null;
		this.ObligLimit.IsSearchControl = false;
		this.ObligLimit.Location = new System.Drawing.Point(12, 53);
		this.ObligLimit.Name = "ObligLimit";
		this.ObligLimit.Properties.Caption = "Commitment Limit";
		this.ObligLimit.Properties.DisplayValueChecked = "Y";
		this.ObligLimit.Properties.DisplayValueUnchecked = "N";
		this.ObligLimit.Properties.ValueChecked = "Y";
		this.ObligLimit.Properties.ValueGrayed = "";
		this.ObligLimit.Properties.ValueUnchecked = "N";
		this.ObligLimit.SettingInVisible = false;
		this.ObligLimit.SettingReadonly = false;
		this.ObligLimit.Size = new System.Drawing.Size(236, 20);
		this.ObligLimit.StyleController = this.layoutControl1;
		this.ObligLimit.TabIndex = 7;
		this.ObligLimit.CheckedChanged += new System.EventHandler(this.ObligLimit_CheckedChanged);
		this.ObligLimit.EditValueChanged += new System.EventHandler(this.ObligLimit_EditValueChanged);
		this.AddDlnBlnc.EditControl = null;
		this.AddDlnBlnc.FieldName = null;
		this.AddDlnBlnc.IsSearchControl = false;
		this.AddDlnBlnc.Location = new System.Drawing.Point(12, 77);
		this.AddDlnBlnc.Name = "AddDlnBlnc";
		this.AddDlnBlnc.Properties.Caption = "Consider Deliveries Balance";
		this.AddDlnBlnc.Properties.DisplayValueChecked = "Y";
		this.AddDlnBlnc.Properties.DisplayValueGrayed = "N";
		this.AddDlnBlnc.Properties.DisplayValueUnchecked = "N";
		this.AddDlnBlnc.Properties.ValueChecked = "Y";
		this.AddDlnBlnc.Properties.ValueGrayed = "";
		this.AddDlnBlnc.Properties.ValueUnchecked = "N";
		this.AddDlnBlnc.SettingInVisible = false;
		this.AddDlnBlnc.SettingReadonly = false;
		this.AddDlnBlnc.Size = new System.Drawing.Size(236, 20);
		this.AddDlnBlnc.StyleController = this.layoutControl1;
		this.AddDlnBlnc.TabIndex = 6;
		this.AddDlnBlnc.CheckedChanged += new System.EventHandler(this.AddDlnBlnc_CheckedChanged);
		this.DfCustTerm.EditControl = null;
		this.DfCustTerm.FieldName = null;
		this.DfCustTerm.IsSearchControl = false;
		this.DfCustTerm.Location = new System.Drawing.Point(200, 135);
		this.DfCustTerm.Name = "DfCustTerm";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.DfCustTerm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.DfCustTerm.Properties.IsSearchControl = false;
		this.DfCustTerm.Properties.NullText = "";
		this.DfCustTerm.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.DfCustTerm_Properties_ButtonPressed);
		this.DfCustTerm.SettingInVisible = false;
		this.DfCustTerm.SettingReadonly = false;
		this.DfCustTerm.Size = new System.Drawing.Size(266, 20);
		this.DfCustTerm.StyleController = this.layoutControl1;
		this.DfCustTerm.TabIndex = 16;
		this.DfCustTerm.EditValueChanged += new System.EventHandler(this.DfCustTerm_EditValueChanged);
		this.DfVendTerm.EditControl = null;
		this.DfVendTerm.FieldName = null;
		this.DfVendTerm.IsSearchControl = false;
		this.DfVendTerm.Location = new System.Drawing.Point(200, 159);
		this.DfVendTerm.Name = "DfVendTerm";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.DfVendTerm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.DfVendTerm.Properties.IsSearchControl = false;
		this.DfVendTerm.Properties.NullText = "";
		this.DfVendTerm.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.DfVendTerm_Properties_ButtonPressed);
		this.DfVendTerm.SettingInVisible = false;
		this.DfVendTerm.SettingReadonly = false;
		this.DfVendTerm.Size = new System.Drawing.Size(266, 20);
		this.DfVendTerm.StyleController = this.layoutControl1;
		this.DfVendTerm.TabIndex = 17;
		this.DfVendTerm.EditValueChanged += new System.EventHandler(this.DfVendTerm_EditValueChanged);
		this.DfltCustPM.EditControl = null;
		this.DfltCustPM.FieldName = null;
		this.DfltCustPM.IsSearchControl = false;
		this.DfltCustPM.Location = new System.Drawing.Point(200, 183);
		this.DfltCustPM.Name = "DfltCustPM";
		this.DfltCustPM.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfltCustPM.Properties.IsSearchControl = false;
		this.DfltCustPM.Properties.NullText = "";
		this.DfltCustPM.SettingInVisible = false;
		this.DfltCustPM.SettingReadonly = false;
		this.DfltCustPM.Size = new System.Drawing.Size(266, 20);
		this.DfltCustPM.StyleController = this.layoutControl1;
		this.DfltCustPM.TabIndex = 18;
		this.DfltCustPM.EditValueChanged += new System.EventHandler(this.gridLookUpEdit3_EditValueChanged);
		this.DfltVendPM.EditControl = null;
		this.DfltVendPM.FieldName = null;
		this.DfltVendPM.IsSearchControl = false;
		this.DfltVendPM.Location = new System.Drawing.Point(200, 207);
		this.DfltVendPM.Name = "DfltVendPM";
		this.DfltVendPM.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfltVendPM.Properties.IsSearchControl = false;
		this.DfltVendPM.Properties.NullText = "";
		this.DfltVendPM.SettingInVisible = false;
		this.DfltVendPM.SettingReadonly = false;
		this.DfltVendPM.Size = new System.Drawing.Size(266, 20);
		this.DfltVendPM.StyleController = this.layoutControl1;
		this.DfltVendPM.TabIndex = 19;
		this.DfltVendPM.EditValueChanged += new System.EventHandler(this.DfltVendPM_EditValueChanged);
		this.CreditDpst.EditControl = null;
		this.CreditDpst.FieldName = null;
		this.CreditDpst.IsSearchControl = false;
		this.CreditDpst.Location = new System.Drawing.Point(200, 231);
		this.CreditDpst.Name = "CreditDpst";
		this.CreditDpst.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CreditDpst.Properties.IsSearchControl = false;
		this.CreditDpst.Properties.NullText = "";
		this.CreditDpst.SettingInVisible = false;
		this.CreditDpst.SettingReadonly = false;
		this.CreditDpst.Size = new System.Drawing.Size(266, 20);
		this.CreditDpst.StyleController = this.layoutControl1;
		this.CreditDpst.TabIndex = 20;
		this.CreditDpst.EditValueChanged += new System.EventHandler(this.CreditDpst_EditValueChanged);
		this.Root.CustomizationFormText = "Root";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[29]
		{
			this.emptySpaceItem1, this.emptySpaceItem2, this.emptySpaceItem4, this.emptySpaceItem5, this.emptySpaceItem6, this.layoutControlItem3, this.layoutControlItem4, this.layoutControlItem5, this.layoutControlItem6, this.layoutControlItem7,
			this.layoutControlItem8, this.layoutControlItem9, this.layoutControlItem10, this.layoutControlItem11, this.layoutControlItem13, this.layoutControlItem14, this.layoutControlItem15, this.layoutControlItem16, this.layoutControlItem17, this.layoutControlItem19,
			this.layoutControlItem20, this.layoutControlItem21, this.layoutControlItem22, this.layoutControlItem23, this.emptySpaceItem3, this.simpleLabelItem2, this.simpleLabelItem1, this.emptySpaceItem8, this.layoutControlItem12
		});
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(876, 452);
		this.Root.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(449, 89);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(209, 34);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(240, 113);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(209, 10);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem4";
		this.emptySpaceItem4.Location = new System.Drawing.Point(458, 123);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(398, 309);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem5";
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 339);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(458, 69);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem6.Location = new System.Drawing.Point(658, 0);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(198, 123);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem3.Control = this.AddDlnBlnc;
		this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
		this.layoutControlItem3.EditControl = null;
		this.layoutControlItem3.EditValue = "layoutControlItem3";
		this.layoutControlItem3.FieldName = null;
		this.layoutControlItem3.IsSearchControl = false;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 65);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.ReadOnly = false;
		this.layoutControlItem3.SettingInVisible = false;
		this.layoutControlItem3.SettingReadonly = false;
		this.layoutControlItem3.Size = new System.Drawing.Size(240, 58);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem4.Control = this.ObligLimit;
		this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
		this.layoutControlItem4.EditControl = null;
		this.layoutControlItem4.EditValue = "layoutControlItem4";
		this.layoutControlItem4.FieldName = null;
		this.layoutControlItem4.IsSearchControl = false;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 41);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.ReadOnly = false;
		this.layoutControlItem4.SettingInVisible = false;
		this.layoutControlItem4.SettingReadonly = false;
		this.layoutControlItem4.Size = new System.Drawing.Size(240, 24);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem5.Control = this.CreditLimt;
		this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
		this.layoutControlItem5.EditControl = null;
		this.layoutControlItem5.EditValue = "layoutControlItem5";
		this.layoutControlItem5.FieldName = null;
		this.layoutControlItem5.IsSearchControl = false;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.ReadOnly = false;
		this.layoutControlItem5.SettingInVisible = false;
		this.layoutControlItem5.SettingReadonly = false;
		this.layoutControlItem5.Size = new System.Drawing.Size(240, 24);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem6.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem6.Control = this.SalesLimit;
		this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
		this.layoutControlItem6.EditControl = null;
		this.layoutControlItem6.EditValue = "layoutControlItem6";
		this.layoutControlItem6.FieldName = null;
		this.layoutControlItem6.IsSearchControl = false;
		this.layoutControlItem6.Location = new System.Drawing.Point(240, 17);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.ReadOnly = false;
		this.layoutControlItem6.SettingInVisible = false;
		this.layoutControlItem6.SettingReadonly = false;
		this.layoutControlItem6.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem7.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem7.Control = this.DlnLimit;
		this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
		this.layoutControlItem7.EditControl = null;
		this.layoutControlItem7.EditValue = "layoutControlItem7";
		this.layoutControlItem7.FieldName = null;
		this.layoutControlItem7.IsSearchControl = false;
		this.layoutControlItem7.Location = new System.Drawing.Point(240, 41);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.ReadOnly = false;
		this.layoutControlItem7.SettingInVisible = false;
		this.layoutControlItem7.SettingReadonly = false;
		this.layoutControlItem7.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem8.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem8.Control = this.OrderLimit;
		this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
		this.layoutControlItem8.EditControl = null;
		this.layoutControlItem8.EditValue = "layoutControlItem8";
		this.layoutControlItem8.FieldName = null;
		this.layoutControlItem8.IsSearchControl = false;
		this.layoutControlItem8.Location = new System.Drawing.Point(240, 65);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.ReadOnly = false;
		this.layoutControlItem8.SettingInVisible = false;
		this.layoutControlItem8.SettingReadonly = false;
		this.layoutControlItem8.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem9.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem9.Control = this.PickLimit;
		this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
		this.layoutControlItem9.EditControl = null;
		this.layoutControlItem9.EditValue = "layoutControlItem9";
		this.layoutControlItem9.FieldName = null;
		this.layoutControlItem9.IsSearchControl = false;
		this.layoutControlItem9.Location = new System.Drawing.Point(240, 89);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.ReadOnly = false;
		this.layoutControlItem9.SettingInVisible = false;
		this.layoutControlItem9.SettingReadonly = false;
		this.layoutControlItem9.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem10.Control = this.SlpCommUse;
		this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
		this.layoutControlItem10.EditControl = null;
		this.layoutControlItem10.EditValue = "layoutControlItem10";
		this.layoutControlItem10.FieldName = null;
		this.layoutControlItem10.IsSearchControl = false;
		this.layoutControlItem10.Location = new System.Drawing.Point(449, 17);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.ReadOnly = false;
		this.layoutControlItem10.SettingInVisible = false;
		this.layoutControlItem10.SettingReadonly = false;
		this.layoutControlItem10.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.layoutControlItem11.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem11.Control = this.ItmCommUse;
		this.layoutControlItem11.CustomizationFormText = "layoutControlItem11";
		this.layoutControlItem11.EditControl = null;
		this.layoutControlItem11.EditValue = "layoutControlItem11";
		this.layoutControlItem11.FieldName = null;
		this.layoutControlItem11.IsSearchControl = false;
		this.layoutControlItem11.Location = new System.Drawing.Point(449, 41);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.ReadOnly = false;
		this.layoutControlItem11.SettingInVisible = false;
		this.layoutControlItem11.SettingReadonly = false;
		this.layoutControlItem11.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.TextVisible = false;
		this.layoutControlItem13.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem13.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem13.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem13.Control = this.DfCustTerm;
		this.layoutControlItem13.CustomizationFormText = "Default Payment Term for Customer";
		this.layoutControlItem13.EditControl = null;
		this.layoutControlItem13.EditValue = "Default Payment Term for Customer";
		this.layoutControlItem13.FieldName = null;
		this.layoutControlItem13.IsSearchControl = false;
		this.layoutControlItem13.Location = new System.Drawing.Point(0, 123);
		this.layoutControlItem13.Name = "layoutControlItem13";
		this.layoutControlItem13.ReadOnly = false;
		this.layoutControlItem13.SettingInVisible = false;
		this.layoutControlItem13.SettingReadonly = false;
		this.layoutControlItem13.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem13.Text = "Default Payment Term for Customer";
		this.layoutControlItem13.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem14.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem14.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem14.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem14.Control = this.DfVendTerm;
		this.layoutControlItem14.CustomizationFormText = "layoutControlItem14";
		this.layoutControlItem14.EditControl = null;
		this.layoutControlItem14.EditValue = "Default Payment Term for Vendor";
		this.layoutControlItem14.FieldName = null;
		this.layoutControlItem14.IsSearchControl = false;
		this.layoutControlItem14.Location = new System.Drawing.Point(0, 147);
		this.layoutControlItem14.Name = "layoutControlItem14";
		this.layoutControlItem14.ReadOnly = false;
		this.layoutControlItem14.SettingInVisible = false;
		this.layoutControlItem14.SettingReadonly = false;
		this.layoutControlItem14.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem14.Text = "Default Payment Term for Vendor";
		this.layoutControlItem14.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem15.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem15.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem15.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem15.Control = this.DfltCustPM;
		this.layoutControlItem15.CustomizationFormText = "layoutControlItem15";
		this.layoutControlItem15.EditControl = null;
		this.layoutControlItem15.EditValue = "Default Payment Method for Customer";
		this.layoutControlItem15.FieldName = null;
		this.layoutControlItem15.IsSearchControl = false;
		this.layoutControlItem15.Location = new System.Drawing.Point(0, 171);
		this.layoutControlItem15.Name = "layoutControlItem15";
		this.layoutControlItem15.ReadOnly = false;
		this.layoutControlItem15.SettingInVisible = false;
		this.layoutControlItem15.SettingReadonly = false;
		this.layoutControlItem15.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem15.Text = "Default Payment Method for Customer";
		this.layoutControlItem15.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem16.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem16.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem16.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem16.Control = this.DfltVendPM;
		this.layoutControlItem16.CustomizationFormText = "layoutControlItem16";
		this.layoutControlItem16.EditControl = null;
		this.layoutControlItem16.EditValue = "Default Payment Method for Vendor";
		this.layoutControlItem16.FieldName = null;
		this.layoutControlItem16.IsSearchControl = false;
		this.layoutControlItem16.Location = new System.Drawing.Point(0, 195);
		this.layoutControlItem16.Name = "layoutControlItem16";
		this.layoutControlItem16.ReadOnly = false;
		this.layoutControlItem16.SettingInVisible = false;
		this.layoutControlItem16.SettingReadonly = false;
		this.layoutControlItem16.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem16.Text = "Default Payment Method for Vendor";
		this.layoutControlItem16.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem17.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem17.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem17.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem17.Control = this.CreditDpst;
		this.layoutControlItem17.CustomizationFormText = "layoutControlItem17";
		this.layoutControlItem17.EditControl = null;
		this.layoutControlItem17.EditValue = "Submit Credit Vouchers";
		this.layoutControlItem17.FieldName = null;
		this.layoutControlItem17.IsSearchControl = false;
		this.layoutControlItem17.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem17.Name = "layoutControlItem17";
		this.layoutControlItem17.ReadOnly = false;
		this.layoutControlItem17.SettingInVisible = false;
		this.layoutControlItem17.SettingReadonly = false;
		this.layoutControlItem17.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem17.Text = "Submit Credit Vouchers";
		this.layoutControlItem17.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem19.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem19.Control = this.useDocWrf;
		this.layoutControlItem19.CustomizationFormText = "layoutControlItem19";
		this.layoutControlItem19.EditControl = null;
		this.layoutControlItem19.EditValue = "layoutControlItem19";
		this.layoutControlItem19.FieldName = null;
		this.layoutControlItem19.IsSearchControl = false;
		this.layoutControlItem19.Location = new System.Drawing.Point(0, 243);
		this.layoutControlItem19.Name = "layoutControlItem19";
		this.layoutControlItem19.ReadOnly = false;
		this.layoutControlItem19.SettingInVisible = false;
		this.layoutControlItem19.SettingReadonly = false;
		this.layoutControlItem19.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem19.TextVisible = false;
		this.layoutControlItem20.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem20.Control = this.EnbApprDI;
		this.layoutControlItem20.CustomizationFormText = "layoutControlItem20";
		this.layoutControlItem20.EditControl = null;
		this.layoutControlItem20.EditValue = "layoutControlItem20";
		this.layoutControlItem20.FieldName = null;
		this.layoutControlItem20.IsSearchControl = false;
		this.layoutControlItem20.Location = new System.Drawing.Point(24, 267);
		this.layoutControlItem20.Name = "layoutControlItem20";
		this.layoutControlItem20.ReadOnly = false;
		this.layoutControlItem20.SettingInVisible = false;
		this.layoutControlItem20.SettingReadonly = false;
		this.layoutControlItem20.Size = new System.Drawing.Size(434, 24);
		this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem20.TextVisible = false;
		this.layoutControlItem21.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem21.Control = this.DspFrznBP;
		this.layoutControlItem21.CustomizationFormText = "layoutControlItem21";
		this.layoutControlItem21.EditControl = null;
		this.layoutControlItem21.EditValue = "layoutControlItem21";
		this.layoutControlItem21.FieldName = null;
		this.layoutControlItem21.IsSearchControl = false;
		this.layoutControlItem21.Location = new System.Drawing.Point(0, 291);
		this.layoutControlItem21.Name = "layoutControlItem21";
		this.layoutControlItem21.ReadOnly = false;
		this.layoutControlItem21.SettingInVisible = false;
		this.layoutControlItem21.SettingReadonly = false;
		this.layoutControlItem21.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem21.TextVisible = false;
		this.layoutControlItem22.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem22.Control = this.DspIBPDoc;
		this.layoutControlItem22.CustomizationFormText = "layoutControlItem22";
		this.layoutControlItem22.EditControl = null;
		this.layoutControlItem22.EditValue = "layoutControlItem22";
		this.layoutControlItem22.FieldName = null;
		this.layoutControlItem22.IsSearchControl = false;
		this.layoutControlItem22.Location = new System.Drawing.Point(0, 315);
		this.layoutControlItem22.Name = "layoutControlItem22";
		this.layoutControlItem22.ReadOnly = false;
		this.layoutControlItem22.SettingInVisible = false;
		this.layoutControlItem22.SettingReadonly = false;
		this.layoutControlItem22.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem22.TextVisible = false;
		this.layoutControlItem23.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem23.Control = this.MBAOnPer;
		this.layoutControlItem23.CustomizationFormText = "layoutControlItem23";
		this.layoutControlItem23.EditControl = null;
		this.layoutControlItem23.EditValue = "layoutControlItem23";
		this.layoutControlItem23.FieldName = null;
		this.layoutControlItem23.IsSearchControl = false;
		this.layoutControlItem23.Location = new System.Drawing.Point(0, 408);
		this.layoutControlItem23.Name = "layoutControlItem23";
		this.layoutControlItem23.ReadOnly = false;
		this.layoutControlItem23.SettingInVisible = false;
		this.layoutControlItem23.SettingReadonly = false;
		this.layoutControlItem23.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem23.TextSize = new System.Drawing.Size(185, 13);
		this.layoutControlItem23.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 267);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(24, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem2.AllowHotTrack = false;
		this.simpleLabelItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem2.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem2.CustomizationFormText = "Set Commission by";
		this.simpleLabelItem2.Location = new System.Drawing.Point(449, 0);
		this.simpleLabelItem2.Name = "simpleLabelItem2";
		this.simpleLabelItem2.Size = new System.Drawing.Size(209, 17);
		this.simpleLabelItem2.Text = "Set Commission by";
		this.simpleLabelItem2.TextSize = new System.Drawing.Size(185, 13);
		this.simpleLabelItem1.AllowHotTrack = false;
		this.simpleLabelItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem1.CustomizationFormText = "Customer Activity Restrictions";
		this.simpleLabelItem1.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem1.Name = "simpleLabelItem1";
		this.simpleLabelItem1.Size = new System.Drawing.Size(221, 17);
		this.simpleLabelItem1.Text = "Customer Activity Restrictions";
		this.simpleLabelItem1.TextSize = new System.Drawing.Size(185, 13);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem8";
		this.emptySpaceItem8.Location = new System.Drawing.Point(221, 0);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(228, 17);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem12.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem12.Control = this.CrdCommUse;
		this.layoutControlItem12.CustomizationFormText = "layoutControlItem12";
		this.layoutControlItem12.EditControl = null;
		this.layoutControlItem12.EditValue = "layoutControlItem12";
		this.layoutControlItem12.FieldName = null;
		this.layoutControlItem12.IsSearchControl = false;
		this.layoutControlItem12.Location = new System.Drawing.Point(449, 65);
		this.layoutControlItem12.Name = "layoutControlItem12";
		this.layoutControlItem12.ReadOnly = false;
		this.layoutControlItem12.SettingInVisible = false;
		this.layoutControlItem12.SettingReadonly = false;
		this.layoutControlItem12.Size = new System.Drawing.Size(209, 24);
		this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem12.TextVisible = false;
		this.Tab_Budget.Controls.Add(this.layoutControl2);
		this.Tab_Budget.EditControl = null;
		this.Tab_Budget.EditValue = "Budget";
		this.Tab_Budget.FieldName = "Tab_Budget";
		this.Tab_Budget.IsSearchControl = false;
		this.Tab_Budget.Name = "Tab_Budget";
		this.Tab_Budget.ReadOnly = false;
		this.Tab_Budget.SettingInVisible = false;
		this.Tab_Budget.SettingReadonly = false;
		this.Tab_Budget.Size = new System.Drawing.Size(876, 452);
		this.Tab_Budget.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Budget.Text = "Budget";
		this.layoutControl2.Controls.Add(this.BdgtAcctng);
		this.layoutControl2.Controls.Add(this.BdgtPDNDoc);
		this.layoutControl2.Controls.Add(this.BdgtPORDoc);
		this.layoutControl2.Controls.Add(this.BdgtPRQDOC);
		this.layoutControl2.Controls.Add(this.BgtWarning);
		this.layoutControl2.Controls.Add(this.BgtBlock);
		this.layoutControl2.Controls.Add(this.DoBudget);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(564, 402, 250, 350);
		this.layoutControl2.Root = this.layoutControlGroup1;
		this.layoutControl2.Size = new System.Drawing.Size(876, 311);
		this.layoutControl2.TabIndex = 0;
		this.layoutControl2.Text = "layoutControl2";
		this.BdgtAcctng.EditControl = null;
		this.BdgtAcctng.FieldName = null;
		this.BdgtAcctng.IsSearchControl = false;
		this.BdgtAcctng.Location = new System.Drawing.Point(156, 279);
		this.BdgtAcctng.Name = "BdgtAcctng";
		this.BdgtAcctng.Properties.Caption = "Accounting";
		this.BdgtAcctng.Properties.DisplayValueChecked = "Y";
		this.BdgtAcctng.Properties.DisplayValueUnchecked = "N";
		this.BdgtAcctng.Properties.ValueChecked = "Y";
		this.BdgtAcctng.Properties.ValueGrayed = "";
		this.BdgtAcctng.Properties.ValueUnchecked = "N";
		this.BdgtAcctng.SettingInVisible = false;
		this.BdgtAcctng.SettingReadonly = false;
		this.BdgtAcctng.Size = new System.Drawing.Size(133, 20);
		this.BdgtAcctng.StyleController = this.layoutControl2;
		this.BdgtAcctng.TabIndex = 10;
		this.BdgtAcctng.CheckedChanged += new System.EventHandler(this.BdgtAcctng_CheckedChanged);
		this.BdgtPDNDoc.EditControl = null;
		this.BdgtPDNDoc.FieldName = null;
		this.BdgtPDNDoc.IsSearchControl = false;
		this.BdgtPDNDoc.Location = new System.Drawing.Point(156, 255);
		this.BdgtPDNDoc.Name = "BdgtPDNDoc";
		this.BdgtPDNDoc.Properties.Caption = "Goods Receipt POs";
		this.BdgtPDNDoc.Properties.DisplayValueChecked = "Y";
		this.BdgtPDNDoc.Properties.DisplayValueUnchecked = "N";
		this.BdgtPDNDoc.Properties.ValueChecked = "Y";
		this.BdgtPDNDoc.Properties.ValueGrayed = "";
		this.BdgtPDNDoc.Properties.ValueUnchecked = "N";
		this.BdgtPDNDoc.SettingInVisible = false;
		this.BdgtPDNDoc.SettingReadonly = false;
		this.BdgtPDNDoc.Size = new System.Drawing.Size(133, 20);
		this.BdgtPDNDoc.StyleController = this.layoutControl2;
		this.BdgtPDNDoc.TabIndex = 9;
		this.BdgtPDNDoc.CheckedChanged += new System.EventHandler(this.BdgtPDNDoc_CheckedChanged);
		this.BdgtPORDoc.EditControl = null;
		this.BdgtPORDoc.FieldName = null;
		this.BdgtPORDoc.IsSearchControl = false;
		this.BdgtPORDoc.Location = new System.Drawing.Point(156, 231);
		this.BdgtPORDoc.Name = "BdgtPORDoc";
		this.BdgtPORDoc.Properties.Caption = "Purchase Orders";
		this.BdgtPORDoc.Properties.DisplayValueChecked = "Y";
		this.BdgtPORDoc.Properties.DisplayValueUnchecked = "N";
		this.BdgtPORDoc.Properties.ValueChecked = "Y";
		this.BdgtPORDoc.Properties.ValueGrayed = "";
		this.BdgtPORDoc.Properties.ValueUnchecked = "N";
		this.BdgtPORDoc.SettingInVisible = false;
		this.BdgtPORDoc.SettingReadonly = false;
		this.BdgtPORDoc.Size = new System.Drawing.Size(133, 20);
		this.BdgtPORDoc.StyleController = this.layoutControl2;
		this.BdgtPORDoc.TabIndex = 8;
		this.BdgtPORDoc.CheckedChanged += new System.EventHandler(this.BdgtPORDoc_CheckedChanged);
		this.BdgtPRQDOC.EditControl = null;
		this.BdgtPRQDOC.FieldName = null;
		this.BdgtPRQDOC.IsSearchControl = false;
		this.BdgtPRQDOC.Location = new System.Drawing.Point(156, 207);
		this.BdgtPRQDOC.Name = "BdgtPRQDOC";
		this.BdgtPRQDOC.Properties.Caption = "Purchase Re&quest";
		this.BdgtPRQDOC.Properties.DisplayValueChecked = "Y";
		this.BdgtPRQDOC.Properties.DisplayValueUnchecked = "N";
		this.BdgtPRQDOC.Properties.ValueChecked = "Y";
		this.BdgtPRQDOC.Properties.ValueGrayed = "";
		this.BdgtPRQDOC.Properties.ValueUnchecked = "N";
		this.BdgtPRQDOC.SettingInVisible = false;
		this.BdgtPRQDOC.SettingReadonly = false;
		this.BdgtPRQDOC.Size = new System.Drawing.Size(133, 20);
		this.BdgtPRQDOC.StyleController = this.layoutControl2;
		this.BdgtPRQDOC.TabIndex = 7;
		this.BdgtPRQDOC.CheckedChanged += new System.EventHandler(this.BdgtPRQDOC_CheckedChanged);
		this.BgtWarning.EditControl = null;
		this.BgtWarning.FieldName = null;
		this.BgtWarning.IsSearchControl = false;
		this.BgtWarning.Location = new System.Drawing.Point(156, 137);
		this.BgtWarning.Name = "BgtWarning";
		this.BgtWarning.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.BgtWarning.Properties.Columns = 1;
		this.BgtWarning.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("A", "For Annual Budget"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("M", "For Month&ly Budget")
		});
		this.BgtWarning.SettingInVisible = false;
		this.BgtWarning.SettingReadonly = false;
		this.BgtWarning.Size = new System.Drawing.Size(708, 56);
		this.BgtWarning.StyleController = this.layoutControl2;
		this.BgtWarning.TabIndex = 6;
		this.BgtWarning.SelectedIndexChanged += new System.EventHandler(this.radioGroup2_SelectedIndexChanged);
		this.BgtBlock.EditControl = null;
		this.BgtBlock.FieldName = null;
		this.BgtBlock.IsSearchControl = false;
		this.BgtBlock.Location = new System.Drawing.Point(48, 78);
		this.BgtBlock.Name = "BgtBlock";
		this.BgtBlock.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.BgtBlock.Properties.Columns = 3;
		this.BgtBlock.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("B", "Block Deviation from Budget"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("W", "Warning"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Without Warning")
		});
		this.BgtBlock.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.BgtBlock.SettingInVisible = false;
		this.BgtBlock.SettingReadonly = false;
		this.BgtBlock.Size = new System.Drawing.Size(655, 32);
		this.BgtBlock.StyleController = this.layoutControl2;
		this.BgtBlock.TabIndex = 5;
		this.BgtBlock.SelectedIndexChanged += new System.EventHandler(this.BgtBlock_SelectedIndexChanged);
		this.BgtBlock.EditValueChanged += new System.EventHandler(this.BgtBlock_EditValueChanged);
		this.DoBudget.EditControl = null;
		this.DoBudget.FieldName = null;
		this.DoBudget.IsSearchControl = false;
		this.DoBudget.Location = new System.Drawing.Point(12, 12);
		this.DoBudget.Name = "DoBudget";
		this.DoBudget.Properties.Caption = "Budget Initialization";
		this.DoBudget.Properties.DisplayValueChecked = "Y";
		this.DoBudget.Properties.DisplayValueUnchecked = "N";
		this.DoBudget.Properties.ValueChecked = "Y";
		this.DoBudget.Properties.ValueGrayed = "";
		this.DoBudget.Properties.ValueUnchecked = "N";
		this.DoBudget.SettingInVisible = false;
		this.DoBudget.SettingReadonly = false;
		this.DoBudget.Size = new System.Drawing.Size(852, 20);
		this.DoBudget.StyleController = this.layoutControl2;
		this.DoBudget.TabIndex = 4;
		this.DoBudget.CheckedChanged += new System.EventHandler(this.checkEdit16_CheckedChanged_1);
		this.DoBudget.EditValueChanged += new System.EventHandler(this.DoBudget_EditValueChanged);
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
		{
			this.emptySpaceItem9, this.layoutControlItem1, this.simpleLabelItem3, this.layoutControlItem2, this.emptySpaceItem7, this.emptySpaceItem10, this.emptySpaceItem11, this.emptySpaceItem12, this.layoutControlItem24, this.emptySpaceItem13,
			this.emptySpaceItem14, this.emptySpaceItem15, this.emptySpaceItem16, this.layoutControlItem25, this.layoutControlItem26, this.layoutControlItem27, this.layoutControlItem28
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(876, 311);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem9";
		this.emptySpaceItem9.Location = new System.Drawing.Point(24, 24);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(832, 12);
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem1.Control = this.DoBudget;
		this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
		this.layoutControlItem1.EditControl = null;
		this.layoutControlItem1.EditValue = "layoutControlItem1";
		this.layoutControlItem1.FieldName = null;
		this.layoutControlItem1.IsSearchControl = false;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.ReadOnly = false;
		this.layoutControlItem1.SettingInVisible = false;
		this.layoutControlItem1.SettingReadonly = false;
		this.layoutControlItem1.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.simpleLabelItem3.AllowHotTrack = false;
		this.simpleLabelItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem3.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem3.CustomizationFormText = "LabelsimpleLabelItem3";
		this.simpleLabelItem3.Location = new System.Drawing.Point(24, 36);
		this.simpleLabelItem3.Name = "simpleLabelItem3";
		this.simpleLabelItem3.Size = new System.Drawing.Size(832, 17);
		this.simpleLabelItem3.Text = "For a Document that Deviates from the Budget:";
		this.simpleLabelItem3.TextSize = new System.Drawing.Size(229, 13);
		this.layoutControlItem2.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem2.Control = this.BgtBlock;
		this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
		this.layoutControlItem2.EditControl = null;
		this.layoutControlItem2.EditValue = "layoutControlItem2";
		this.layoutControlItem2.FieldName = null;
		this.layoutControlItem2.IsSearchControl = false;
		this.layoutControlItem2.Location = new System.Drawing.Point(36, 66);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.ReadOnly = false;
		this.layoutControlItem2.SettingInVisible = false;
		this.layoutControlItem2.SettingReadonly = false;
		this.layoutControlItem2.Size = new System.Drawing.Size(659, 36);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.CustomizationFormText = "emptySpaceItem7";
		this.emptySpaceItem7.Location = new System.Drawing.Point(36, 53);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(820, 13);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.CustomizationFormText = "emptySpaceItem10";
		this.emptySpaceItem10.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem10.Name = "emptySpaceItem10";
		this.emptySpaceItem10.Size = new System.Drawing.Size(24, 78);
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem11.AllowHotTrack = false;
		this.emptySpaceItem11.CustomizationFormText = "emptySpaceItem11";
		this.emptySpaceItem11.Location = new System.Drawing.Point(24, 53);
		this.emptySpaceItem11.Name = "emptySpaceItem11";
		this.emptySpaceItem11.Size = new System.Drawing.Size(12, 49);
		this.emptySpaceItem11.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem12.AllowHotTrack = false;
		this.emptySpaceItem12.CustomizationFormText = "emptySpaceItem12";
		this.emptySpaceItem12.Location = new System.Drawing.Point(695, 66);
		this.emptySpaceItem12.Name = "emptySpaceItem12";
		this.emptySpaceItem12.Size = new System.Drawing.Size(161, 36);
		this.emptySpaceItem12.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem24.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem24.Control = this.BgtWarning;
		this.layoutControlItem24.CustomizationFormText = "layoutControlItem24";
		this.layoutControlItem24.EditControl = null;
		this.layoutControlItem24.EditValue = "layoutControlItem24";
		this.layoutControlItem24.FieldName = null;
		this.layoutControlItem24.IsSearchControl = false;
		this.layoutControlItem24.Location = new System.Drawing.Point(144, 125);
		this.layoutControlItem24.Name = "layoutControlItem24";
		this.layoutControlItem24.ReadOnly = false;
		this.layoutControlItem24.SettingInVisible = false;
		this.layoutControlItem24.SettingReadonly = false;
		this.layoutControlItem24.Size = new System.Drawing.Size(712, 60);
		this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem24.TextVisible = false;
		this.emptySpaceItem13.AllowHotTrack = false;
		this.emptySpaceItem13.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem13.Location = new System.Drawing.Point(0, 125);
		this.emptySpaceItem13.Name = "emptySpaceItem13";
		this.emptySpaceItem13.Size = new System.Drawing.Size(144, 166);
		this.emptySpaceItem13.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem14.AllowHotTrack = false;
		this.emptySpaceItem14.CustomizationFormText = "emptySpaceItem14";
		this.emptySpaceItem14.Location = new System.Drawing.Point(0, 102);
		this.emptySpaceItem14.Name = "emptySpaceItem14";
		this.emptySpaceItem14.Size = new System.Drawing.Size(856, 23);
		this.emptySpaceItem14.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem15.AllowHotTrack = false;
		this.emptySpaceItem15.CustomizationFormText = "emptySpaceItem15";
		this.emptySpaceItem15.Location = new System.Drawing.Point(144, 185);
		this.emptySpaceItem15.Name = "emptySpaceItem15";
		this.emptySpaceItem15.Size = new System.Drawing.Size(712, 10);
		this.emptySpaceItem15.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem16.AllowHotTrack = false;
		this.emptySpaceItem16.CustomizationFormText = "emptySpaceItem16";
		this.emptySpaceItem16.Location = new System.Drawing.Point(281, 195);
		this.emptySpaceItem16.Name = "emptySpaceItem16";
		this.emptySpaceItem16.Size = new System.Drawing.Size(575, 96);
		this.emptySpaceItem16.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem25.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem25.Control = this.BdgtPRQDOC;
		this.layoutControlItem25.CustomizationFormText = "layoutControlItem25";
		this.layoutControlItem25.EditControl = null;
		this.layoutControlItem25.EditValue = "layoutControlItem25";
		this.layoutControlItem25.FieldName = null;
		this.layoutControlItem25.IsSearchControl = false;
		this.layoutControlItem25.Location = new System.Drawing.Point(144, 195);
		this.layoutControlItem25.Name = "layoutControlItem25";
		this.layoutControlItem25.ReadOnly = false;
		this.layoutControlItem25.SettingInVisible = false;
		this.layoutControlItem25.SettingReadonly = false;
		this.layoutControlItem25.Size = new System.Drawing.Size(137, 24);
		this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem25.TextVisible = false;
		this.layoutControlItem26.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem26.Control = this.BdgtPORDoc;
		this.layoutControlItem26.CustomizationFormText = "layoutControlItem26";
		this.layoutControlItem26.EditControl = null;
		this.layoutControlItem26.EditValue = "layoutControlItem26";
		this.layoutControlItem26.FieldName = null;
		this.layoutControlItem26.IsSearchControl = false;
		this.layoutControlItem26.Location = new System.Drawing.Point(144, 219);
		this.layoutControlItem26.Name = "layoutControlItem26";
		this.layoutControlItem26.ReadOnly = false;
		this.layoutControlItem26.SettingInVisible = false;
		this.layoutControlItem26.SettingReadonly = false;
		this.layoutControlItem26.Size = new System.Drawing.Size(137, 24);
		this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem26.TextVisible = false;
		this.layoutControlItem27.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem27.Control = this.BdgtPDNDoc;
		this.layoutControlItem27.CustomizationFormText = "layoutControlItem27";
		this.layoutControlItem27.EditControl = null;
		this.layoutControlItem27.EditValue = "layoutControlItem27";
		this.layoutControlItem27.FieldName = null;
		this.layoutControlItem27.IsSearchControl = false;
		this.layoutControlItem27.Location = new System.Drawing.Point(144, 243);
		this.layoutControlItem27.Name = "layoutControlItem27";
		this.layoutControlItem27.ReadOnly = false;
		this.layoutControlItem27.SettingInVisible = false;
		this.layoutControlItem27.SettingReadonly = false;
		this.layoutControlItem27.Size = new System.Drawing.Size(137, 24);
		this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem27.TextVisible = false;
		this.layoutControlItem28.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem28.Control = this.BdgtAcctng;
		this.layoutControlItem28.CustomizationFormText = "layoutControlItem28";
		this.layoutControlItem28.EditControl = null;
		this.layoutControlItem28.EditValue = "layoutControlItem28";
		this.layoutControlItem28.FieldName = null;
		this.layoutControlItem28.IsSearchControl = false;
		this.layoutControlItem28.Location = new System.Drawing.Point(144, 267);
		this.layoutControlItem28.Name = "layoutControlItem28";
		this.layoutControlItem28.ReadOnly = false;
		this.layoutControlItem28.SettingInVisible = false;
		this.layoutControlItem28.SettingReadonly = false;
		this.layoutControlItem28.Size = new System.Drawing.Size(137, 24);
		this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem28.TextVisible = false;
		this.Tab_Services.Controls.Add(this.layoutControl3);
		this.Tab_Services.EditControl = null;
		this.Tab_Services.EditValue = "Services";
		this.Tab_Services.FieldName = "Tab_Services";
		this.Tab_Services.IsSearchControl = false;
		this.Tab_Services.Name = "Tab_Services";
		this.Tab_Services.ReadOnly = false;
		this.Tab_Services.SettingInVisible = false;
		this.Tab_Services.SettingReadonly = false;
		this.Tab_Services.Size = new System.Drawing.Size(876, 452);
		this.Tab_Services.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Services.Text = "Services";
		this.layoutControl3.Controls.Add(this.label4);
		this.layoutControl3.Controls.Add(this.label2);
		this.layoutControl3.Controls.Add(this.label1);
		this.layoutControl3.Controls.Add(this.SPNEnabled);
		this.layoutControl3.Controls.Add(this.IMCEEnable);
		this.layoutControl3.Controls.Add(this.MaxHistory);
		this.layoutControl3.Controls.Add(this.ScreenLock);
		this.layoutControl3.Controls.Add(this.AlertPolFr);
		this.layoutControl3.Controls.Add(this.DfltByEml);
		this.layoutControl3.Controls.Add(this.ck_UsedProxy);
		this.layoutControl3.Controls.Add(this.ShowNewTsk);
		this.layoutControl3.Controls.Add(this.OpenDps);
		this.layoutControl3.Controls.Add(this.OpenCdt);
		this.layoutControl3.Controls.Add(this.ShowNewMsg);
		this.layoutControl3.Controls.Add(this.ContactLog);
		this.layoutControl3.Controls.Add(this.RclFlag);
		this.layoutControl3.Controls.Add(this.RcrFlag);
		this.layoutControl3.Controls.Add(this.DsplyRates);
		this.layoutControl3.Controls.Add(this.CheckFiles);
		this.layoutControl3.Controls.Add(this.OpenCredit);
		this.layoutControl3.Controls.Add(this.MapService);
		this.layoutControl3.Controls.Add(this.memo_Address);
		this.layoutControl3.Controls.Add(this.memo_Port);
		this.layoutControl3.Controls.Add(this.memo_Area);
		this.layoutControl3.Controls.Add(this.txt_ForEx);
		this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl3.Location = new System.Drawing.Point(0, 0);
		this.layoutControl3.Name = "layoutControl3";
		this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(698, 344, 250, 350);
		this.layoutControl3.Root = this.layoutControlGroup2;
		this.layoutControl3.Size = new System.Drawing.Size(876, 452);
		this.layoutControl3.TabIndex = 0;
		this.layoutControl3.Text = "layoutControl3";
		this.label4.EditControl = null;
		this.label4.EditValue = "Default E-Mail Method";
		this.label4.FieldName = null;
		this.label4.HideWhenEmpty = false;
		this.label4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label4.Location = new System.Drawing.Point(333, 169);
		this.label4.Name = "label4";
		this.label4.ReadOnly = false;
		this.label4.SettingInVisible = false;
		this.label4.SettingReadonly = false;
		this.label4.ShowUnderLine = true;
		this.label4.Size = new System.Drawing.Size(291, 19);
		this.label4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label4.TabIndex = 28;
		this.label4.Utext = "Default E-Mail Method";
		this.label2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Appearance.Options.UseFont = true;
		this.label2.EditControl = null;
		this.label2.EditValue = "Internet Definitions";
		this.label2.FieldName = null;
		this.label2.HideWhenEmpty = false;
		this.label2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label2.Location = new System.Drawing.Point(333, 12);
		this.label2.Name = "label2";
		this.label2.ReadOnly = false;
		this.label2.SettingInVisible = false;
		this.label2.SettingReadonly = false;
		this.label2.ShowUnderLine = true;
		this.label2.Size = new System.Drawing.Size(291, 16);
		this.label2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label2.TabIndex = 27;
		this.label2.Utext = "Internet Definitions";
		this.label1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Appearance.Options.UseFont = true;
		this.label1.EditControl = null;
		this.label1.EditValue = "Telephone No.";
		this.label1.FieldName = null;
		this.label1.HideWhenEmpty = false;
		this.label1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label1.Location = new System.Drawing.Point(333, 263);
		this.label1.Name = "label1";
		this.label1.ReadOnly = false;
		this.label1.SettingInVisible = false;
		this.label1.SettingReadonly = false;
		this.label1.ShowUnderLine = true;
		this.label1.Size = new System.Drawing.Size(291, 39);
		this.label1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label1.TabIndex = 26;
		this.label1.Utext = "Telephone No.";
		this.SPNEnabled.EditControl = null;
		this.SPNEnabled.FieldName = null;
		this.SPNEnabled.IsSearchControl = false;
		this.SPNEnabled.Location = new System.Drawing.Point(628, 56);
		this.SPNEnabled.Name = "SPNEnabled";
		this.SPNEnabled.Properties.Caption = "Enable Transaction Notification";
		this.SPNEnabled.Properties.DisplayValueChecked = "Y";
		this.SPNEnabled.Properties.DisplayValueUnchecked = "N";
		this.SPNEnabled.Properties.ValueChecked = "Y";
		this.SPNEnabled.Properties.ValueGrayed = "";
		this.SPNEnabled.Properties.ValueUnchecked = "N";
		this.SPNEnabled.SettingInVisible = false;
		this.SPNEnabled.SettingReadonly = false;
		this.SPNEnabled.Size = new System.Drawing.Size(236, 20);
		this.SPNEnabled.StyleController = this.layoutControl3;
		this.SPNEnabled.TabIndex = 25;
		this.SPNEnabled.CheckedChanged += new System.EventHandler(this.SPNEnabled_CheckedChanged);
		this.IMCEEnable.EditControl = null;
		this.IMCEEnable.FieldName = null;
		this.IMCEEnable.IsSearchControl = false;
		this.IMCEEnable.Location = new System.Drawing.Point(628, 12);
		this.IMCEEnable.Name = "IMCEEnable";
		this.IMCEEnable.Properties.Caption = "Enable Analytics Powered by SAP HANA";
		this.IMCEEnable.Properties.DisplayValueChecked = "Y";
		this.IMCEEnable.Properties.DisplayValueUnchecked = "N";
		this.IMCEEnable.Properties.ValueChecked = "Y";
		this.IMCEEnable.Properties.ValueGrayed = "";
		this.IMCEEnable.Properties.ValueUnchecked = "N";
		this.IMCEEnable.SettingInVisible = false;
		this.IMCEEnable.SettingReadonly = false;
		this.IMCEEnable.Size = new System.Drawing.Size(236, 20);
		this.IMCEEnable.StyleController = this.layoutControl3;
		this.IMCEEnable.TabIndex = 24;
		this.IMCEEnable.CheckedChanged += new System.EventHandler(this.IMCEEnable_CheckedChanged);
		this.MaxHistory.EditControl = null;
		this.MaxHistory.FieldName = null;
		this.MaxHistory.IsSearchControl = false;
		this.MaxHistory.Location = new System.Drawing.Point(534, 378);
		this.MaxHistory.Name = "MaxHistory";
		this.MaxHistory.SettingInVisible = false;
		this.MaxHistory.SettingReadonly = false;
		this.MaxHistory.Size = new System.Drawing.Size(90, 20);
		this.MaxHistory.StyleController = this.layoutControl3;
		this.MaxHistory.TabIndex = 23;
		this.MaxHistory.EditValueChanged += new System.EventHandler(this.MaxHistory_EditValueChanged);
		this.ScreenLock.EditControl = null;
		this.ScreenLock.FieldName = null;
		this.ScreenLock.IsSearchControl = false;
		this.ScreenLock.Location = new System.Drawing.Point(213, 330);
		this.ScreenLock.Name = "ScreenLock";
		this.ScreenLock.SettingInVisible = false;
		this.ScreenLock.SettingReadonly = false;
		this.ScreenLock.Size = new System.Drawing.Size(116, 20);
		this.ScreenLock.StyleController = this.layoutControl3;
		this.ScreenLock.TabIndex = 18;
		this.ScreenLock.EditValueChanged += new System.EventHandler(this.ScreenLock_EditValueChanged);
		this.AlertPolFr.EditControl = null;
		this.AlertPolFr.FieldName = null;
		this.AlertPolFr.IsSearchControl = false;
		this.AlertPolFr.Location = new System.Drawing.Point(213, 306);
		this.AlertPolFr.Name = "AlertPolFr";
		this.AlertPolFr.SettingInVisible = false;
		this.AlertPolFr.SettingReadonly = false;
		this.AlertPolFr.Size = new System.Drawing.Size(116, 20);
		this.AlertPolFr.StyleController = this.layoutControl3;
		this.AlertPolFr.TabIndex = 17;
		this.AlertPolFr.EditValueChanged += new System.EventHandler(this.textEdit3_EditValueChanged);
		this.DfltByEml.EditControl = null;
		this.DfltByEml.FieldName = null;
		this.DfltByEml.IsSearchControl = false;
		this.DfltByEml.Location = new System.Drawing.Point(333, 192);
		this.DfltByEml.Name = "DfltByEml";
		this.DfltByEml.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.DfltByEml.Properties.Columns = 1;
		this.DfltByEml.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("E", "SAP Business One Mailer"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Outlook E-Mail")
		});
		this.DfltByEml.SettingInVisible = false;
		this.DfltByEml.SettingReadonly = false;
		this.DfltByEml.Size = new System.Drawing.Size(291, 57);
		this.DfltByEml.StyleController = this.layoutControl3;
		this.DfltByEml.TabIndex = 16;
		this.DfltByEml.SelectedIndexChanged += new System.EventHandler(this.DfltByEml_SelectedIndexChanged);
		this.ck_UsedProxy.EditControl = null;
		this.ck_UsedProxy.FieldName = null;
		this.ck_UsedProxy.IsSearchControl = false;
		this.ck_UsedProxy.Location = new System.Drawing.Point(333, 32);
		this.ck_UsedProxy.Name = "ck_UsedProxy";
		this.ck_UsedProxy.Properties.Caption = "Use Proxy Server for Web Connection";
		this.ck_UsedProxy.Properties.DisplayValueChecked = "Y";
		this.ck_UsedProxy.Properties.DisplayValueUnchecked = "N";
		this.ck_UsedProxy.Properties.ValueChecked = "Y";
		this.ck_UsedProxy.Properties.ValueGrayed = "";
		this.ck_UsedProxy.Properties.ValueUnchecked = "N";
		this.ck_UsedProxy.SettingInVisible = false;
		this.ck_UsedProxy.SettingReadonly = false;
		this.ck_UsedProxy.Size = new System.Drawing.Size(291, 20);
		this.ck_UsedProxy.StyleController = this.layoutControl3;
		this.ck_UsedProxy.TabIndex = 13;
		this.ck_UsedProxy.CheckedChanged += new System.EventHandler(this.ck_UsedProxy_CheckedChanged);
		this.ShowNewTsk.EditControl = null;
		this.ShowNewTsk.FieldName = null;
		this.ShowNewTsk.IsSearchControl = false;
		this.ShowNewTsk.Location = new System.Drawing.Point(12, 224);
		this.ShowNewTsk.Name = "ShowNewTsk";
		this.ShowNewTsk.Properties.Caption = "Display Worklist When New Task Arrives";
		this.ShowNewTsk.Properties.DisplayValueChecked = "Y";
		this.ShowNewTsk.Properties.DisplayValueUnchecked = "N";
		this.ShowNewTsk.Properties.ValueChecked = "Y";
		this.ShowNewTsk.Properties.ValueGrayed = "";
		this.ShowNewTsk.Properties.ValueUnchecked = "N";
		this.ShowNewTsk.SettingInVisible = false;
		this.ShowNewTsk.SettingReadonly = false;
		this.ShowNewTsk.Size = new System.Drawing.Size(317, 20);
		this.ShowNewTsk.StyleController = this.layoutControl3;
		this.ShowNewTsk.TabIndex = 12;
		this.ShowNewTsk.CheckedChanged += new System.EventHandler(this.ShowNewTsk_CheckedChanged);
		this.OpenDps.EditControl = null;
		this.OpenDps.FieldName = null;
		this.OpenDps.IsSearchControl = false;
		this.OpenDps.Location = new System.Drawing.Point(12, 200);
		this.OpenDps.Name = "OpenDps";
		this.OpenDps.Properties.Caption = "Open Postdated Checks Window";
		this.OpenDps.Properties.DisplayValueChecked = "Y";
		this.OpenDps.Properties.DisplayValueUnchecked = "N";
		this.OpenDps.Properties.ValueChecked = "Y";
		this.OpenDps.Properties.ValueGrayed = "";
		this.OpenDps.Properties.ValueUnchecked = "N";
		this.OpenDps.SettingInVisible = false;
		this.OpenDps.SettingReadonly = false;
		this.OpenDps.Size = new System.Drawing.Size(317, 20);
		this.OpenDps.StyleController = this.layoutControl3;
		this.OpenDps.TabIndex = 11;
		this.OpenDps.CheckedChanged += new System.EventHandler(this.OpenDps_CheckedChanged);
		this.OpenCdt.EditControl = null;
		this.OpenCdt.FieldName = null;
		this.OpenCdt.IsSearchControl = false;
		this.OpenCdt.Location = new System.Drawing.Point(12, 176);
		this.OpenCdt.Name = "OpenCdt";
		this.OpenCdt.Properties.Caption = "Open Window for Credit Voucher Ref. Update";
		this.OpenCdt.Properties.DisplayValueChecked = "Y";
		this.OpenCdt.Properties.DisplayValueUnchecked = "N";
		this.OpenCdt.Properties.ValueChecked = "Y";
		this.OpenCdt.Properties.ValueGrayed = "";
		this.OpenCdt.Properties.ValueUnchecked = "N";
		this.OpenCdt.SettingInVisible = false;
		this.OpenCdt.SettingReadonly = false;
		this.OpenCdt.Size = new System.Drawing.Size(317, 20);
		this.OpenCdt.StyleController = this.layoutControl3;
		this.OpenCdt.TabIndex = 10;
		this.OpenCdt.CheckedChanged += new System.EventHandler(this.OpenCdt_CheckedChanged);
		this.ShowNewMsg.EditControl = null;
		this.ShowNewMsg.FieldName = null;
		this.ShowNewMsg.IsSearchControl = false;
		this.ShowNewMsg.Location = new System.Drawing.Point(12, 152);
		this.ShowNewMsg.Name = "ShowNewMsg";
		this.ShowNewMsg.Properties.Caption = "Disp&lay Inbox When New Message Arrives";
		this.ShowNewMsg.Properties.DisplayValueChecked = "Y";
		this.ShowNewMsg.Properties.DisplayValueUnchecked = "N";
		this.ShowNewMsg.Properties.ValueChecked = "Y";
		this.ShowNewMsg.Properties.ValueGrayed = "";
		this.ShowNewMsg.Properties.ValueUnchecked = "N";
		this.ShowNewMsg.SettingInVisible = false;
		this.ShowNewMsg.SettingReadonly = false;
		this.ShowNewMsg.Size = new System.Drawing.Size(317, 20);
		this.ShowNewMsg.StyleController = this.layoutControl3;
		this.ShowNewMsg.TabIndex = 9;
		this.ShowNewMsg.CheckedChanged += new System.EventHandler(this.ShowNewMsg_CheckedChanged);
		this.ContactLog.EditControl = null;
		this.ContactLog.FieldName = null;
		this.ContactLog.IsSearchControl = false;
		this.ContactLog.Location = new System.Drawing.Point(12, 128);
		this.ContactLog.Name = "ContactLog";
		this.ContactLog.Properties.Caption = "Send Alert for Activities Scheduled for Today";
		this.ContactLog.Properties.DisplayValueChecked = "Y";
		this.ContactLog.Properties.DisplayValueUnchecked = "N";
		this.ContactLog.Properties.ValueChecked = "Y";
		this.ContactLog.Properties.ValueGrayed = "";
		this.ContactLog.Properties.ValueUnchecked = "N";
		this.ContactLog.SettingInVisible = false;
		this.ContactLog.SettingReadonly = false;
		this.ContactLog.Size = new System.Drawing.Size(317, 20);
		this.ContactLog.StyleController = this.layoutControl3;
		this.ContactLog.TabIndex = 8;
		this.ContactLog.CheckedChanged += new System.EventHandler(this.ContactLog_CheckedChanged);
		this.RclFlag.EditControl = null;
		this.RclFlag.FieldName = null;
		this.RclFlag.IsSearchControl = false;
		this.RclFlag.Location = new System.Drawing.Point(12, 104);
		this.RclFlag.Name = "RclFlag";
		this.RclFlag.Properties.Caption = "Display Recurring Transactions on Execution";
		this.RclFlag.Properties.DisplayValueChecked = "Y";
		this.RclFlag.Properties.DisplayValueUnchecked = "N";
		this.RclFlag.Properties.ValueChecked = "Y";
		this.RclFlag.Properties.ValueGrayed = "";
		this.RclFlag.Properties.ValueUnchecked = "N";
		this.RclFlag.SettingInVisible = false;
		this.RclFlag.SettingReadonly = false;
		this.RclFlag.Size = new System.Drawing.Size(317, 20);
		this.RclFlag.StyleController = this.layoutControl3;
		this.RclFlag.TabIndex = 7;
		this.RclFlag.CheckedChanged += new System.EventHandler(this.RclFlag_CheckedChanged);
		this.RcrFlag.EditControl = null;
		this.RcrFlag.FieldName = null;
		this.RcrFlag.IsSearchControl = false;
		this.RcrFlag.Location = new System.Drawing.Point(12, 80);
		this.RcrFlag.Name = "RcrFlag";
		this.RcrFlag.Properties.Caption = "Displa&y Recurring Postings on Execution";
		this.RcrFlag.Properties.DisplayValueChecked = "Y";
		this.RcrFlag.Properties.DisplayValueUnchecked = "N";
		this.RcrFlag.Properties.ValueChecked = "Y";
		this.RcrFlag.Properties.ValueGrayed = "";
		this.RcrFlag.Properties.ValueUnchecked = "N";
		this.RcrFlag.SettingInVisible = false;
		this.RcrFlag.SettingReadonly = false;
		this.RcrFlag.Size = new System.Drawing.Size(317, 20);
		this.RcrFlag.StyleController = this.layoutControl3;
		this.RcrFlag.TabIndex = 6;
		this.RcrFlag.CheckedChanged += new System.EventHandler(this.RcrFlag_CheckedChanged);
		this.DsplyRates.EditControl = null;
		this.DsplyRates.FieldName = null;
		this.DsplyRates.IsSearchControl = false;
		this.DsplyRates.Location = new System.Drawing.Point(12, 56);
		this.DsplyRates.Name = "DsplyRates";
		this.DsplyRates.Properties.Caption = "Open Exchange Rates Table";
		this.DsplyRates.Properties.DisplayValueChecked = "Y";
		this.DsplyRates.Properties.DisplayValueUnchecked = "N";
		this.DsplyRates.Properties.ValueChecked = "Y";
		this.DsplyRates.Properties.ValueGrayed = "";
		this.DsplyRates.Properties.ValueUnchecked = "N";
		this.DsplyRates.SettingInVisible = false;
		this.DsplyRates.SettingReadonly = false;
		this.DsplyRates.Size = new System.Drawing.Size(317, 20);
		this.DsplyRates.StyleController = this.layoutControl3;
		this.DsplyRates.TabIndex = 5;
		this.DsplyRates.CheckedChanged += new System.EventHandler(this.DsplyRates_CheckedChanged);
		this.CheckFiles.EditControl = null;
		this.CheckFiles.FieldName = null;
		this.CheckFiles.IsSearchControl = false;
		this.CheckFiles.Location = new System.Drawing.Point(12, 29);
		this.CheckFiles.Name = "CheckFiles";
		this.CheckFiles.Properties.Caption = "Perform Data Check";
		this.CheckFiles.Properties.DisplayValueChecked = "Y";
		this.CheckFiles.Properties.DisplayValueUnchecked = "N";
		this.CheckFiles.Properties.ValueChecked = "Y";
		this.CheckFiles.Properties.ValueGrayed = "";
		this.CheckFiles.Properties.ValueUnchecked = "N";
		this.CheckFiles.SettingInVisible = false;
		this.CheckFiles.SettingReadonly = false;
		this.CheckFiles.Size = new System.Drawing.Size(317, 20);
		this.CheckFiles.StyleController = this.layoutControl3;
		this.CheckFiles.TabIndex = 4;
		this.CheckFiles.CheckedChanged += new System.EventHandler(this.CheckFiles_CheckedChanged);
		this.OpenCredit.EditControl = null;
		this.OpenCredit.FieldName = null;
		this.OpenCredit.IsSearchControl = false;
		this.OpenCredit.Location = new System.Drawing.Point(213, 354);
		this.OpenCredit.Name = "OpenCredit";
		this.OpenCredit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.OpenCredit.Properties.IsSearchControl = false;
		this.OpenCredit.Properties.NullText = "";
		this.OpenCredit.SettingInVisible = false;
		this.OpenCredit.SettingReadonly = false;
		this.OpenCredit.Size = new System.Drawing.Size(116, 20);
		this.OpenCredit.StyleController = this.layoutControl3;
		this.OpenCredit.TabIndex = 21;
		this.OpenCredit.EditValueChanged += new System.EventHandler(this.OpenCredit_EditValueChanged);
		this.MapService.EditControl = null;
		this.MapService.FieldName = null;
		this.MapService.IsSearchControl = false;
		this.MapService.Location = new System.Drawing.Point(534, 354);
		this.MapService.Name = "MapService";
		this.MapService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.MapService.Properties.IsSearchControl = false;
		this.MapService.Properties.NullText = "";
		this.MapService.SettingInVisible = false;
		this.MapService.SettingReadonly = false;
		this.MapService.Size = new System.Drawing.Size(90, 20);
		this.MapService.StyleController = this.layoutControl3;
		this.MapService.TabIndex = 22;
		this.MapService.EditValueChanged += new System.EventHandler(this.MapService_EditValueChanged);
		this.memo_Address.Location = new System.Drawing.Point(534, 56);
		this.memo_Address.Name = "memo_Address";
		this.memo_Address.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Address.Properties.ShowIcon = false;
		this.memo_Address.Size = new System.Drawing.Size(90, 20);
		this.memo_Address.StyleController = this.layoutControl3;
		this.memo_Address.TabIndex = 14;
		this.memo_Address.EditValueChanged += new System.EventHandler(this.memo_Address_EditValueChanged);
		this.memo_Port.Location = new System.Drawing.Point(534, 80);
		this.memo_Port.Name = "memo_Port";
		this.memo_Port.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Port.Properties.ShowIcon = false;
		this.memo_Port.Size = new System.Drawing.Size(90, 20);
		this.memo_Port.StyleController = this.layoutControl3;
		this.memo_Port.TabIndex = 15;
		this.memo_Port.EditValueChanged += new System.EventHandler(this.memo_Port_EditValueChanged);
		this.memo_Area.Location = new System.Drawing.Point(534, 306);
		this.memo_Area.Name = "memo_Area";
		this.memo_Area.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Area.Properties.ShowIcon = false;
		this.memo_Area.Size = new System.Drawing.Size(90, 20);
		this.memo_Area.StyleController = this.layoutControl3;
		this.memo_Area.TabIndex = 19;
		this.memo_Area.EditValueChanged += new System.EventHandler(this.memo_Area_EditValueChanged);
		this.txt_ForEx.Location = new System.Drawing.Point(534, 330);
		this.txt_ForEx.Name = "txt_ForEx";
		this.txt_ForEx.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ForEx.Properties.ShowIcon = false;
		this.txt_ForEx.Size = new System.Drawing.Size(90, 20);
		this.txt_ForEx.StyleController = this.layoutControl3;
		this.txt_ForEx.TabIndex = 20;
		this.txt_ForEx.EditValueChanged += new System.EventHandler(this.textEdit6_EditValueChanged);
		this.layoutControlGroup2.CustomizationFormText = "Root";
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[36]
		{
			this.emptySpaceItem17, this.emptySpaceItem18, this.emptySpaceItem19, this.emptySpaceItem20, this.emptySpaceItem21, this.emptySpaceItem22, this.simpleLabelItem4, this.layoutControlItem29, this.layoutControlItem30, this.layoutControlItem31,
			this.layoutControlItem32, this.layoutControlItem33, this.layoutControlItem34, this.layoutControlItem35, this.layoutControlItem36, this.layoutControlItem37, this.layoutControlItem38, this.layoutControlItem39, this.layoutControlItem40, this.emptySpaceItem23,
			this.layoutControlItem41, this.emptySpaceItem24, this.layoutControlItem42, this.layoutControlItem43, this.layoutControlItem44, this.layoutControlItem45, this.layoutControlItem46, this.layoutControlItem47, this.layoutControlItem48, this.layoutControlItem49,
			this.emptySpaceItem25, this.layoutControlItem50, this.emptySpaceItem26, this.layoutControlItem152, this.layoutControlItem153, this.layoutControlItem154
		});
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup2.TextVisible = false;
		this.emptySpaceItem17.AllowHotTrack = false;
		this.emptySpaceItem17.CustomizationFormText = "emptySpaceItem17";
		this.emptySpaceItem17.Location = new System.Drawing.Point(616, 251);
		this.emptySpaceItem17.Name = "emptySpaceItem17";
		this.emptySpaceItem17.Size = new System.Drawing.Size(240, 181);
		this.emptySpaceItem17.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem18.AllowHotTrack = false;
		this.emptySpaceItem18.CustomizationFormText = "emptySpaceItem18";
		this.emptySpaceItem18.Location = new System.Drawing.Point(0, 366);
		this.emptySpaceItem18.Name = "emptySpaceItem18";
		this.emptySpaceItem18.Size = new System.Drawing.Size(321, 66);
		this.emptySpaceItem18.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem19.AllowHotTrack = false;
		this.emptySpaceItem19.CustomizationFormText = "emptySpaceItem19";
		this.emptySpaceItem19.Location = new System.Drawing.Point(321, 390);
		this.emptySpaceItem19.Name = "emptySpaceItem19";
		this.emptySpaceItem19.Size = new System.Drawing.Size(295, 42);
		this.emptySpaceItem19.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem20.AllowHotTrack = false;
		this.emptySpaceItem20.CustomizationFormText = "emptySpaceItem20";
		this.emptySpaceItem20.Location = new System.Drawing.Point(0, 236);
		this.emptySpaceItem20.Name = "emptySpaceItem20";
		this.emptySpaceItem20.Size = new System.Drawing.Size(321, 15);
		this.emptySpaceItem20.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem21.AllowHotTrack = false;
		this.emptySpaceItem21.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem21.Location = new System.Drawing.Point(321, 241);
		this.emptySpaceItem21.Name = "emptySpaceItem21";
		this.emptySpaceItem21.Size = new System.Drawing.Size(295, 10);
		this.emptySpaceItem21.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem22.AllowHotTrack = false;
		this.emptySpaceItem22.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem22.Location = new System.Drawing.Point(616, 68);
		this.emptySpaceItem22.Name = "emptySpaceItem22";
		this.emptySpaceItem22.Size = new System.Drawing.Size(240, 183);
		this.emptySpaceItem22.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem4.AllowHotTrack = false;
		this.simpleLabelItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem4.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem4.CustomizationFormText = "At the Beginning of Each Session";
		this.simpleLabelItem4.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem4.Name = "simpleLabelItem4";
		this.simpleLabelItem4.Size = new System.Drawing.Size(321, 17);
		this.simpleLabelItem4.Text = "At the Beginning of Each Session";
		this.simpleLabelItem4.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem29.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem29.Control = this.CheckFiles;
		this.layoutControlItem29.CustomizationFormText = "layoutControlItem29";
		this.layoutControlItem29.EditControl = null;
		this.layoutControlItem29.EditValue = "layoutControlItem29";
		this.layoutControlItem29.FieldName = null;
		this.layoutControlItem29.IsSearchControl = false;
		this.layoutControlItem29.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem29.Name = "layoutControlItem29";
		this.layoutControlItem29.ReadOnly = false;
		this.layoutControlItem29.SettingInVisible = false;
		this.layoutControlItem29.SettingReadonly = false;
		this.layoutControlItem29.Size = new System.Drawing.Size(321, 27);
		this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem29.TextVisible = false;
		this.layoutControlItem30.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem30.Control = this.DsplyRates;
		this.layoutControlItem30.CustomizationFormText = "layoutControlItem30";
		this.layoutControlItem30.EditControl = null;
		this.layoutControlItem30.EditValue = "layoutControlItem30";
		this.layoutControlItem30.FieldName = null;
		this.layoutControlItem30.IsSearchControl = false;
		this.layoutControlItem30.Location = new System.Drawing.Point(0, 44);
		this.layoutControlItem30.Name = "layoutControlItem30";
		this.layoutControlItem30.ReadOnly = false;
		this.layoutControlItem30.SettingInVisible = false;
		this.layoutControlItem30.SettingReadonly = false;
		this.layoutControlItem30.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem30.TextVisible = false;
		this.layoutControlItem31.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem31.Control = this.RcrFlag;
		this.layoutControlItem31.CustomizationFormText = "layoutControlItem31";
		this.layoutControlItem31.EditControl = null;
		this.layoutControlItem31.EditValue = "layoutControlItem31";
		this.layoutControlItem31.FieldName = null;
		this.layoutControlItem31.IsSearchControl = false;
		this.layoutControlItem31.Location = new System.Drawing.Point(0, 68);
		this.layoutControlItem31.Name = "layoutControlItem31";
		this.layoutControlItem31.ReadOnly = false;
		this.layoutControlItem31.SettingInVisible = false;
		this.layoutControlItem31.SettingReadonly = false;
		this.layoutControlItem31.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem31.TextVisible = false;
		this.layoutControlItem32.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem32.Control = this.RclFlag;
		this.layoutControlItem32.CustomizationFormText = "layoutControlItem32";
		this.layoutControlItem32.EditControl = null;
		this.layoutControlItem32.EditValue = "layoutControlItem32";
		this.layoutControlItem32.FieldName = null;
		this.layoutControlItem32.IsSearchControl = false;
		this.layoutControlItem32.Location = new System.Drawing.Point(0, 92);
		this.layoutControlItem32.Name = "layoutControlItem32";
		this.layoutControlItem32.ReadOnly = false;
		this.layoutControlItem32.SettingInVisible = false;
		this.layoutControlItem32.SettingReadonly = false;
		this.layoutControlItem32.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem32.TextVisible = false;
		this.layoutControlItem33.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem33.Control = this.ContactLog;
		this.layoutControlItem33.CustomizationFormText = "layoutControlItem33";
		this.layoutControlItem33.EditControl = null;
		this.layoutControlItem33.EditValue = "layoutControlItem33";
		this.layoutControlItem33.FieldName = null;
		this.layoutControlItem33.IsSearchControl = false;
		this.layoutControlItem33.Location = new System.Drawing.Point(0, 116);
		this.layoutControlItem33.Name = "layoutControlItem33";
		this.layoutControlItem33.ReadOnly = false;
		this.layoutControlItem33.SettingInVisible = false;
		this.layoutControlItem33.SettingReadonly = false;
		this.layoutControlItem33.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem33.TextVisible = false;
		this.layoutControlItem34.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem34.Control = this.ShowNewMsg;
		this.layoutControlItem34.CustomizationFormText = "layoutControlItem34";
		this.layoutControlItem34.EditControl = null;
		this.layoutControlItem34.EditValue = "layoutControlItem34";
		this.layoutControlItem34.FieldName = null;
		this.layoutControlItem34.IsSearchControl = false;
		this.layoutControlItem34.Location = new System.Drawing.Point(0, 140);
		this.layoutControlItem34.Name = "layoutControlItem34";
		this.layoutControlItem34.ReadOnly = false;
		this.layoutControlItem34.SettingInVisible = false;
		this.layoutControlItem34.SettingReadonly = false;
		this.layoutControlItem34.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem34.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem34.TextVisible = false;
		this.layoutControlItem35.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem35.Control = this.OpenCdt;
		this.layoutControlItem35.CustomizationFormText = "layoutControlItem35";
		this.layoutControlItem35.EditControl = null;
		this.layoutControlItem35.EditValue = "layoutControlItem35";
		this.layoutControlItem35.FieldName = null;
		this.layoutControlItem35.IsSearchControl = false;
		this.layoutControlItem35.Location = new System.Drawing.Point(0, 164);
		this.layoutControlItem35.Name = "layoutControlItem35";
		this.layoutControlItem35.ReadOnly = false;
		this.layoutControlItem35.SettingInVisible = false;
		this.layoutControlItem35.SettingReadonly = false;
		this.layoutControlItem35.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem35.TextVisible = false;
		this.layoutControlItem36.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem36.Control = this.OpenDps;
		this.layoutControlItem36.CustomizationFormText = "layoutControlItem36";
		this.layoutControlItem36.EditControl = null;
		this.layoutControlItem36.EditValue = "layoutControlItem36";
		this.layoutControlItem36.FieldName = null;
		this.layoutControlItem36.IsSearchControl = false;
		this.layoutControlItem36.Location = new System.Drawing.Point(0, 188);
		this.layoutControlItem36.Name = "layoutControlItem36";
		this.layoutControlItem36.ReadOnly = false;
		this.layoutControlItem36.SettingInVisible = false;
		this.layoutControlItem36.SettingReadonly = false;
		this.layoutControlItem36.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem36.TextVisible = false;
		this.layoutControlItem37.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem37.Control = this.ShowNewTsk;
		this.layoutControlItem37.CustomizationFormText = "layoutControlItem37";
		this.layoutControlItem37.EditControl = null;
		this.layoutControlItem37.EditValue = "layoutControlItem37";
		this.layoutControlItem37.FieldName = null;
		this.layoutControlItem37.IsSearchControl = false;
		this.layoutControlItem37.Location = new System.Drawing.Point(0, 212);
		this.layoutControlItem37.Name = "layoutControlItem37";
		this.layoutControlItem37.ReadOnly = false;
		this.layoutControlItem37.SettingInVisible = false;
		this.layoutControlItem37.SettingReadonly = false;
		this.layoutControlItem37.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem37.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem37.TextVisible = false;
		this.layoutControlItem38.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem38.Control = this.ck_UsedProxy;
		this.layoutControlItem38.CustomizationFormText = "layoutControlItem38";
		this.layoutControlItem38.EditControl = null;
		this.layoutControlItem38.EditValue = "layoutControlItem38";
		this.layoutControlItem38.FieldName = null;
		this.layoutControlItem38.IsSearchControl = false;
		this.layoutControlItem38.Location = new System.Drawing.Point(321, 20);
		this.layoutControlItem38.Name = "layoutControlItem38";
		this.layoutControlItem38.ReadOnly = false;
		this.layoutControlItem38.SettingInVisible = false;
		this.layoutControlItem38.SettingReadonly = false;
		this.layoutControlItem38.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem38.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem38.TextVisible = false;
		this.layoutControlItem39.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem39.Control = this.memo_Address;
		this.layoutControlItem39.CustomizationFormText = "Address";
		this.layoutControlItem39.EditControl = null;
		this.layoutControlItem39.EditValue = "Address";
		this.layoutControlItem39.FieldName = null;
		this.layoutControlItem39.IsSearchControl = false;
		this.layoutControlItem39.Location = new System.Drawing.Point(321, 44);
		this.layoutControlItem39.Name = "layoutControlItem39";
		this.layoutControlItem39.ReadOnly = false;
		this.layoutControlItem39.SettingInVisible = false;
		this.layoutControlItem39.SettingReadonly = false;
		this.layoutControlItem39.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem39.Text = "Address";
		this.layoutControlItem39.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem40.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem40.Control = this.memo_Port;
		this.layoutControlItem40.CustomizationFormText = "Port";
		this.layoutControlItem40.EditControl = null;
		this.layoutControlItem40.EditValue = "Port";
		this.layoutControlItem40.FieldName = null;
		this.layoutControlItem40.IsSearchControl = false;
		this.layoutControlItem40.Location = new System.Drawing.Point(321, 68);
		this.layoutControlItem40.Name = "layoutControlItem40";
		this.layoutControlItem40.ReadOnly = false;
		this.layoutControlItem40.SettingInVisible = false;
		this.layoutControlItem40.SettingReadonly = false;
		this.layoutControlItem40.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem40.Text = "Port";
		this.layoutControlItem40.TextSize = new System.Drawing.Size(198, 13);
		this.emptySpaceItem23.AllowHotTrack = false;
		this.emptySpaceItem23.CustomizationFormText = "emptySpaceItem23";
		this.emptySpaceItem23.Location = new System.Drawing.Point(321, 92);
		this.emptySpaceItem23.Name = "emptySpaceItem23";
		this.emptySpaceItem23.Size = new System.Drawing.Size(295, 65);
		this.emptySpaceItem23.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem41.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem41.Control = this.DfltByEml;
		this.layoutControlItem41.CustomizationFormText = "layoutControlItem41";
		this.layoutControlItem41.EditControl = null;
		this.layoutControlItem41.EditValue = "layoutControlItem41";
		this.layoutControlItem41.FieldName = null;
		this.layoutControlItem41.IsSearchControl = false;
		this.layoutControlItem41.Location = new System.Drawing.Point(321, 180);
		this.layoutControlItem41.Name = "layoutControlItem41";
		this.layoutControlItem41.ReadOnly = false;
		this.layoutControlItem41.SettingInVisible = false;
		this.layoutControlItem41.SettingReadonly = false;
		this.layoutControlItem41.Size = new System.Drawing.Size(295, 61);
		this.layoutControlItem41.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem41.TextVisible = false;
		this.emptySpaceItem24.AllowHotTrack = false;
		this.emptySpaceItem24.CustomizationFormText = "emptySpaceItem24";
		this.emptySpaceItem24.Location = new System.Drawing.Point(0, 251);
		this.emptySpaceItem24.Name = "emptySpaceItem24";
		this.emptySpaceItem24.Size = new System.Drawing.Size(321, 43);
		this.emptySpaceItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem42.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem42.Control = this.AlertPolFr;
		this.layoutControlItem42.CustomizationFormText = "Update Messages (Min.)";
		this.layoutControlItem42.EditControl = null;
		this.layoutControlItem42.EditValue = "Update Messages (Min.)";
		this.layoutControlItem42.FieldName = null;
		this.layoutControlItem42.IsSearchControl = false;
		this.layoutControlItem42.Location = new System.Drawing.Point(0, 294);
		this.layoutControlItem42.Name = "layoutControlItem42";
		this.layoutControlItem42.ReadOnly = false;
		this.layoutControlItem42.SettingInVisible = false;
		this.layoutControlItem42.SettingReadonly = false;
		this.layoutControlItem42.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem42.Text = "Update Messages (Min.)";
		this.layoutControlItem42.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem43.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem43.Control = this.ScreenLock;
		this.layoutControlItem43.CustomizationFormText = "Screen Locking Time (Min.)";
		this.layoutControlItem43.EditControl = null;
		this.layoutControlItem43.EditValue = "Screen Locking Time (Min.)";
		this.layoutControlItem43.FieldName = null;
		this.layoutControlItem43.IsSearchControl = false;
		this.layoutControlItem43.Location = new System.Drawing.Point(0, 318);
		this.layoutControlItem43.Name = "layoutControlItem43";
		this.layoutControlItem43.ReadOnly = false;
		this.layoutControlItem43.SettingInVisible = false;
		this.layoutControlItem43.SettingReadonly = false;
		this.layoutControlItem43.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem43.Text = "Screen Locking Time (Min.)";
		this.layoutControlItem43.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem44.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem44.Control = this.memo_Area;
		this.layoutControlItem44.CustomizationFormText = "Area Code";
		this.layoutControlItem44.EditControl = null;
		this.layoutControlItem44.EditValue = "Area Code";
		this.layoutControlItem44.FieldName = null;
		this.layoutControlItem44.IsSearchControl = false;
		this.layoutControlItem44.Location = new System.Drawing.Point(321, 294);
		this.layoutControlItem44.Name = "layoutControlItem44";
		this.layoutControlItem44.ReadOnly = false;
		this.layoutControlItem44.SettingInVisible = false;
		this.layoutControlItem44.SettingReadonly = false;
		this.layoutControlItem44.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem44.Text = "Area Code";
		this.layoutControlItem44.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem45.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem45.Control = this.txt_ForEx;
		this.layoutControlItem45.CustomizationFormText = "For External Line";
		this.layoutControlItem45.EditControl = null;
		this.layoutControlItem45.EditValue = "For External Line";
		this.layoutControlItem45.FieldName = null;
		this.layoutControlItem45.IsSearchControl = false;
		this.layoutControlItem45.Location = new System.Drawing.Point(321, 318);
		this.layoutControlItem45.Name = "layoutControlItem45";
		this.layoutControlItem45.ReadOnly = false;
		this.layoutControlItem45.SettingInVisible = false;
		this.layoutControlItem45.SettingReadonly = false;
		this.layoutControlItem45.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem45.Text = "For External Line";
		this.layoutControlItem45.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem46.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem46.Control = this.OpenCredit;
		this.layoutControlItem46.CustomizationFormText = "Open Postdated Credit Vouchers Window";
		this.layoutControlItem46.EditControl = null;
		this.layoutControlItem46.EditValue = "Open Postdated Credit Vouchers Window";
		this.layoutControlItem46.FieldName = null;
		this.layoutControlItem46.IsSearchControl = false;
		this.layoutControlItem46.Location = new System.Drawing.Point(0, 342);
		this.layoutControlItem46.Name = "layoutControlItem46";
		this.layoutControlItem46.ReadOnly = false;
		this.layoutControlItem46.SettingInVisible = false;
		this.layoutControlItem46.SettingReadonly = false;
		this.layoutControlItem46.Size = new System.Drawing.Size(321, 24);
		this.layoutControlItem46.Text = "Open Postdated Credit Vouchers Window";
		this.layoutControlItem46.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem47.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem47.Control = this.MapService;
		this.layoutControlItem47.CustomizationFormText = "Map Service";
		this.layoutControlItem47.EditControl = null;
		this.layoutControlItem47.EditValue = "Map Service";
		this.layoutControlItem47.FieldName = null;
		this.layoutControlItem47.IsSearchControl = false;
		this.layoutControlItem47.Location = new System.Drawing.Point(321, 342);
		this.layoutControlItem47.Name = "layoutControlItem47";
		this.layoutControlItem47.ReadOnly = false;
		this.layoutControlItem47.SettingInVisible = false;
		this.layoutControlItem47.SettingReadonly = false;
		this.layoutControlItem47.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem47.Text = "Map Service";
		this.layoutControlItem47.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem48.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem48.Control = this.MaxHistory;
		this.layoutControlItem48.CustomizationFormText = "History / Log";
		this.layoutControlItem48.EditControl = null;
		this.layoutControlItem48.EditValue = "History / Log";
		this.layoutControlItem48.FieldName = null;
		this.layoutControlItem48.IsSearchControl = false;
		this.layoutControlItem48.Location = new System.Drawing.Point(321, 366);
		this.layoutControlItem48.Name = "layoutControlItem48";
		this.layoutControlItem48.ReadOnly = false;
		this.layoutControlItem48.SettingInVisible = false;
		this.layoutControlItem48.SettingReadonly = false;
		this.layoutControlItem48.Size = new System.Drawing.Size(295, 24);
		this.layoutControlItem48.Text = "History / Log";
		this.layoutControlItem48.TextSize = new System.Drawing.Size(198, 13);
		this.layoutControlItem49.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem49.Control = this.IMCEEnable;
		this.layoutControlItem49.CustomizationFormText = "layoutControlItem49";
		this.layoutControlItem49.EditControl = null;
		this.layoutControlItem49.EditValue = "layoutControlItem49";
		this.layoutControlItem49.FieldName = null;
		this.layoutControlItem49.IsSearchControl = false;
		this.layoutControlItem49.Location = new System.Drawing.Point(616, 0);
		this.layoutControlItem49.Name = "layoutControlItem49";
		this.layoutControlItem49.ReadOnly = false;
		this.layoutControlItem49.SettingInVisible = false;
		this.layoutControlItem49.SettingReadonly = false;
		this.layoutControlItem49.Size = new System.Drawing.Size(240, 24);
		this.layoutControlItem49.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem49.TextVisible = false;
		this.emptySpaceItem25.AllowHotTrack = false;
		this.emptySpaceItem25.CustomizationFormText = "emptySpaceItem25";
		this.emptySpaceItem25.Location = new System.Drawing.Point(616, 34);
		this.emptySpaceItem25.Name = "emptySpaceItem25";
		this.emptySpaceItem25.Size = new System.Drawing.Size(240, 10);
		this.emptySpaceItem25.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem50.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem50.Control = this.SPNEnabled;
		this.layoutControlItem50.CustomizationFormText = "layoutControlItem50";
		this.layoutControlItem50.EditControl = null;
		this.layoutControlItem50.EditValue = "layoutControlItem50";
		this.layoutControlItem50.FieldName = null;
		this.layoutControlItem50.IsSearchControl = false;
		this.layoutControlItem50.Location = new System.Drawing.Point(616, 44);
		this.layoutControlItem50.Name = "layoutControlItem50";
		this.layoutControlItem50.ReadOnly = false;
		this.layoutControlItem50.SettingInVisible = false;
		this.layoutControlItem50.SettingReadonly = false;
		this.layoutControlItem50.Size = new System.Drawing.Size(240, 24);
		this.layoutControlItem50.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem50.TextVisible = false;
		this.emptySpaceItem26.AllowHotTrack = false;
		this.emptySpaceItem26.CustomizationFormText = "emptySpaceItem26";
		this.emptySpaceItem26.Location = new System.Drawing.Point(616, 24);
		this.emptySpaceItem26.Name = "emptySpaceItem26";
		this.emptySpaceItem26.Size = new System.Drawing.Size(240, 10);
		this.emptySpaceItem26.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem152.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem152.Control = this.label1;
		this.layoutControlItem152.CustomizationFormText = "layoutControlItem152";
		this.layoutControlItem152.EditControl = null;
		this.layoutControlItem152.EditValue = "layoutControlItem152";
		this.layoutControlItem152.FieldName = null;
		this.layoutControlItem152.IsSearchControl = false;
		this.layoutControlItem152.Location = new System.Drawing.Point(321, 251);
		this.layoutControlItem152.Name = "layoutControlItem152";
		this.layoutControlItem152.ReadOnly = false;
		this.layoutControlItem152.SettingInVisible = false;
		this.layoutControlItem152.SettingReadonly = false;
		this.layoutControlItem152.Size = new System.Drawing.Size(295, 43);
		this.layoutControlItem152.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem152.TextVisible = false;
		this.layoutControlItem153.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem153.Control = this.label2;
		this.layoutControlItem153.CustomizationFormText = "layoutControlItem153";
		this.layoutControlItem153.EditControl = null;
		this.layoutControlItem153.EditValue = "layoutControlItem153";
		this.layoutControlItem153.FieldName = null;
		this.layoutControlItem153.IsSearchControl = false;
		this.layoutControlItem153.Location = new System.Drawing.Point(321, 0);
		this.layoutControlItem153.Name = "layoutControlItem153";
		this.layoutControlItem153.ReadOnly = false;
		this.layoutControlItem153.SettingInVisible = false;
		this.layoutControlItem153.SettingReadonly = false;
		this.layoutControlItem153.Size = new System.Drawing.Size(295, 20);
		this.layoutControlItem153.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem153.TextVisible = false;
		this.layoutControlItem154.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem154.Control = this.label4;
		this.layoutControlItem154.CustomizationFormText = "layoutControlItem154";
		this.layoutControlItem154.EditControl = null;
		this.layoutControlItem154.EditValue = "layoutControlItem154";
		this.layoutControlItem154.FieldName = null;
		this.layoutControlItem154.IsSearchControl = false;
		this.layoutControlItem154.Location = new System.Drawing.Point(321, 157);
		this.layoutControlItem154.Name = "layoutControlItem154";
		this.layoutControlItem154.ReadOnly = false;
		this.layoutControlItem154.SettingInVisible = false;
		this.layoutControlItem154.SettingReadonly = false;
		this.layoutControlItem154.Size = new System.Drawing.Size(295, 23);
		this.layoutControlItem154.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem154.TextVisible = false;
		this.Tab_Display.Controls.Add(this.layoutControl4);
		this.Tab_Display.EditControl = null;
		this.Tab_Display.EditValue = "Display";
		this.Tab_Display.FieldName = "Tab_Display";
		this.Tab_Display.IsSearchControl = false;
		this.Tab_Display.Name = "Tab_Display";
		this.Tab_Display.ReadOnly = false;
		this.Tab_Display.SettingInVisible = false;
		this.Tab_Display.SettingReadonly = false;
		this.Tab_Display.Size = new System.Drawing.Size(876, 452);
		this.Tab_Display.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Display.Text = "Display";
		this.layoutControl4.Controls.Add(this.TxtSrch);
		this.layoutControl4.Controls.Add(this.B1SgtEb);
		this.layoutControl4.Controls.Add(this.ck_ManagerCompany);
		this.layoutControl4.Controls.Add(this.DirectRate);
		this.layoutControl4.Controls.Add(this.CurOnRight);
		this.layoutControl4.Controls.Add(this.ThousSep);
		this.layoutControl4.Controls.Add(this.DecSep);
		this.layoutControl4.Controls.Add(this.QueryDec);
		this.layoutControl4.Controls.Add(this.MeasureDec);
		this.layoutControl4.Controls.Add(this.PercentDec);
		this.layoutControl4.Controls.Add(this.QtyDec);
		this.layoutControl4.Controls.Add(this.RateDec);
		this.layoutControl4.Controls.Add(this.SumDec);
		this.layoutControl4.Controls.Add(this.MaxChoose);
		this.layoutControl4.Controls.Add(this.AdvImagePr);
		this.layoutControl4.Controls.Add(this.DateSep);
		this.layoutControl4.Controls.Add(this.Color);
		this.layoutControl4.Controls.Add(this.DefLengthU);
		this.layoutControl4.Controls.Add(this.DefWeightU);
		this.layoutControl4.Controls.Add(this.TimeFormat);
		this.layoutControl4.Controls.Add(this.DateFormat);
		this.layoutControl4.Controls.Add(this.TPLId);
		this.layoutControl4.Controls.Add(this.PriceDec);
		this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl4.Location = new System.Drawing.Point(0, 0);
		this.layoutControl4.Name = "layoutControl4";
		this.layoutControl4.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(564, -7, 250, 350);
		this.layoutControl4.Root = this.layoutControlGroup3;
		this.layoutControl4.Size = new System.Drawing.Size(876, 452);
		this.layoutControl4.TabIndex = 0;
		this.layoutControl4.Text = "layoutControl4";
		this.TxtSrch.EditControl = null;
		this.TxtSrch.FieldName = null;
		this.TxtSrch.IsSearchControl = false;
		this.TxtSrch.Location = new System.Drawing.Point(12, 324);
		this.TxtSrch.Name = "TxtSrch";
		this.TxtSrch.Properties.Caption = "Te&xt Search";
		this.TxtSrch.Properties.DisplayValueChecked = "Y";
		this.TxtSrch.Properties.DisplayValueUnchecked = "N";
		this.TxtSrch.Properties.ValueChecked = "Y";
		this.TxtSrch.Properties.ValueGrayed = "";
		this.TxtSrch.Properties.ValueUnchecked = "N";
		this.TxtSrch.SettingInVisible = false;
		this.TxtSrch.SettingReadonly = false;
		this.TxtSrch.Size = new System.Drawing.Size(350, 20);
		this.TxtSrch.StyleController = this.layoutControl4;
		this.TxtSrch.TabIndex = 27;
		this.TxtSrch.CheckedChanged += new System.EventHandler(this.TxtSrch_CheckedChanged);
		this.B1SgtEb.EditControl = null;
		this.B1SgtEb.FieldName = null;
		this.B1SgtEb.IsSearchControl = false;
		this.B1SgtEb.Location = new System.Drawing.Point(12, 300);
		this.B1SgtEb.Name = "B1SgtEb";
		this.B1SgtEb.Properties.Caption = "Enable SAP Business One Suggest";
		this.B1SgtEb.Properties.DisplayValueChecked = "Y";
		this.B1SgtEb.Properties.DisplayValueUnchecked = "N";
		this.B1SgtEb.Properties.ValueChecked = "Y";
		this.B1SgtEb.Properties.ValueGrayed = "";
		this.B1SgtEb.Properties.ValueUnchecked = "N";
		this.B1SgtEb.SettingInVisible = false;
		this.B1SgtEb.SettingReadonly = false;
		this.B1SgtEb.Size = new System.Drawing.Size(350, 20);
		this.B1SgtEb.StyleController = this.layoutControl4;
		this.B1SgtEb.TabIndex = 26;
		this.B1SgtEb.CheckedChanged += new System.EventHandler(this.B1SgtEb_CheckedChanged);
		this.ck_ManagerCompany.EditControl = null;
		this.ck_ManagerCompany.FieldName = null;
		this.ck_ManagerCompany.IsSearchControl = false;
		this.ck_ManagerCompany.Location = new System.Drawing.Point(12, 170);
		this.ck_ManagerCompany.Name = "ck_ManagerCompany";
		this.ck_ManagerCompany.Properties.Caption = "Manage Company Time";
		this.ck_ManagerCompany.Properties.DisplayValueChecked = "Y";
		this.ck_ManagerCompany.Properties.DisplayValueUnchecked = "N";
		this.ck_ManagerCompany.Properties.ValueChecked = "Y";
		this.ck_ManagerCompany.Properties.ValueGrayed = "";
		this.ck_ManagerCompany.Properties.ValueUnchecked = "N";
		this.ck_ManagerCompany.SettingInVisible = false;
		this.ck_ManagerCompany.SettingReadonly = false;
		this.ck_ManagerCompany.Size = new System.Drawing.Size(350, 20);
		this.ck_ManagerCompany.StyleController = this.layoutControl4;
		this.ck_ManagerCompany.TabIndex = 25;
		this.ck_ManagerCompany.CheckedChanged += new System.EventHandler(this.ck_ManagerCompany_CheckedChanged);
		this.DirectRate.EditControl = null;
		this.DirectRate.FieldName = null;
		this.DirectRate.IsSearchControl = false;
		this.DirectRate.Location = new System.Drawing.Point(404, 306);
		this.DirectRate.Name = "DirectRate";
		this.DirectRate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.DirectRate.Properties.Columns = 1;
		this.DirectRate.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Y", "Direct"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Indirect")
		});
		this.DirectRate.SettingInVisible = false;
		this.DirectRate.SettingReadonly = false;
		this.DirectRate.Size = new System.Drawing.Size(361, 58);
		this.DirectRate.StyleController = this.layoutControl4;
		this.DirectRate.TabIndex = 24;
		this.DirectRate.SelectedIndexChanged += new System.EventHandler(this.DirectRate_SelectedIndexChanged);
		this.CurOnRight.EditControl = null;
		this.CurOnRight.FieldName = null;
		this.CurOnRight.IsSearchControl = false;
		this.CurOnRight.Location = new System.Drawing.Point(404, 255);
		this.CurOnRight.Name = "CurOnRight";
		this.CurOnRight.Properties.Caption = "Disp&lay Currency on the Right";
		this.CurOnRight.Properties.DisplayValueChecked = "Y";
		this.CurOnRight.Properties.DisplayValueUnchecked = "N";
		this.CurOnRight.Properties.ValueChecked = "Y";
		this.CurOnRight.Properties.ValueGrayed = "";
		this.CurOnRight.Properties.ValueUnchecked = "N";
		this.CurOnRight.SettingInVisible = false;
		this.CurOnRight.SettingReadonly = false;
		this.CurOnRight.Size = new System.Drawing.Size(361, 20);
		this.CurOnRight.StyleController = this.layoutControl4;
		this.CurOnRight.TabIndex = 23;
		this.CurOnRight.CheckedChanged += new System.EventHandler(this.CurOnRight_CheckedChanged);
		this.ThousSep.EditControl = null;
		this.ThousSep.FieldName = null;
		this.ThousSep.IsSearchControl = false;
		this.ThousSep.Location = new System.Drawing.Point(559, 221);
		this.ThousSep.Name = "ThousSep";
		this.ThousSep.SettingInVisible = false;
		this.ThousSep.SettingReadonly = false;
		this.ThousSep.Size = new System.Drawing.Size(206, 20);
		this.ThousSep.StyleController = this.layoutControl4;
		this.ThousSep.TabIndex = 22;
		this.ThousSep.EditValueChanged += new System.EventHandler(this.ThousSep_EditValueChanged);
		this.DecSep.EditControl = null;
		this.DecSep.FieldName = null;
		this.DecSep.IsSearchControl = false;
		this.DecSep.Location = new System.Drawing.Point(559, 197);
		this.DecSep.Name = "DecSep";
		this.DecSep.SettingInVisible = false;
		this.DecSep.SettingReadonly = false;
		this.DecSep.Size = new System.Drawing.Size(206, 20);
		this.DecSep.StyleController = this.layoutControl4;
		this.DecSep.TabIndex = 21;
		this.DecSep.EditValueChanged += new System.EventHandler(this.DecSep_EditValueChanged);
		this.QueryDec.EditControl = null;
		this.QueryDec.FieldName = null;
		this.QueryDec.IsSearchControl = false;
		this.QueryDec.Location = new System.Drawing.Point(559, 173);
		this.QueryDec.Name = "QueryDec";
		this.QueryDec.SettingInVisible = false;
		this.QueryDec.SettingReadonly = false;
		this.QueryDec.Size = new System.Drawing.Size(206, 20);
		this.QueryDec.StyleController = this.layoutControl4;
		this.QueryDec.TabIndex = 20;
		this.QueryDec.EditValueChanged += new System.EventHandler(this.QueryDec_EditValueChanged);
		this.MeasureDec.EditControl = null;
		this.MeasureDec.FieldName = null;
		this.MeasureDec.IsSearchControl = false;
		this.MeasureDec.Location = new System.Drawing.Point(559, 149);
		this.MeasureDec.Name = "MeasureDec";
		this.MeasureDec.SettingInVisible = false;
		this.MeasureDec.SettingReadonly = false;
		this.MeasureDec.Size = new System.Drawing.Size(206, 20);
		this.MeasureDec.StyleController = this.layoutControl4;
		this.MeasureDec.TabIndex = 19;
		this.MeasureDec.EditValueChanged += new System.EventHandler(this.MeasureDec_EditValueChanged);
		this.PercentDec.EditControl = null;
		this.PercentDec.FieldName = null;
		this.PercentDec.IsSearchControl = false;
		this.PercentDec.Location = new System.Drawing.Point(559, 125);
		this.PercentDec.Name = "PercentDec";
		this.PercentDec.SettingInVisible = false;
		this.PercentDec.SettingReadonly = false;
		this.PercentDec.Size = new System.Drawing.Size(206, 20);
		this.PercentDec.StyleController = this.layoutControl4;
		this.PercentDec.TabIndex = 18;
		this.PercentDec.EditValueChanged += new System.EventHandler(this.PercentDec_EditValueChanged);
		this.QtyDec.EditControl = null;
		this.QtyDec.FieldName = null;
		this.QtyDec.IsSearchControl = false;
		this.QtyDec.Location = new System.Drawing.Point(559, 101);
		this.QtyDec.Name = "QtyDec";
		this.QtyDec.SettingInVisible = false;
		this.QtyDec.SettingReadonly = false;
		this.QtyDec.Size = new System.Drawing.Size(206, 20);
		this.QtyDec.StyleController = this.layoutControl4;
		this.QtyDec.TabIndex = 17;
		this.QtyDec.EditValueChanged += new System.EventHandler(this.QtyDec_EditValueChanged);
		this.RateDec.EditControl = null;
		this.RateDec.FieldName = null;
		this.RateDec.IsSearchControl = false;
		this.RateDec.Location = new System.Drawing.Point(559, 77);
		this.RateDec.Name = "RateDec";
		this.RateDec.SettingInVisible = false;
		this.RateDec.SettingReadonly = false;
		this.RateDec.Size = new System.Drawing.Size(206, 20);
		this.RateDec.StyleController = this.layoutControl4;
		this.RateDec.TabIndex = 16;
		this.RateDec.EditValueChanged += new System.EventHandler(this.RateDec_EditValueChanged);
		this.SumDec.EditControl = null;
		this.SumDec.FieldName = null;
		this.SumDec.IsSearchControl = false;
		this.SumDec.Location = new System.Drawing.Point(559, 29);
		this.SumDec.Name = "SumDec";
		this.SumDec.SettingInVisible = false;
		this.SumDec.SettingReadonly = false;
		this.SumDec.Size = new System.Drawing.Size(206, 20);
		this.SumDec.StyleController = this.layoutControl4;
		this.SumDec.TabIndex = 14;
		this.SumDec.EditValueChanged += new System.EventHandler(this.SumDec_EditValueChanged);
		this.MaxChoose.EditControl = null;
		this.MaxChoose.FieldName = null;
		this.MaxChoose.IsSearchControl = false;
		this.MaxChoose.Location = new System.Drawing.Point(167, 218);
		this.MaxChoose.Name = "MaxChoose";
		this.MaxChoose.SettingInVisible = false;
		this.MaxChoose.SettingReadonly = false;
		this.MaxChoose.Size = new System.Drawing.Size(195, 20);
		this.MaxChoose.StyleController = this.layoutControl4;
		this.MaxChoose.TabIndex = 13;
		this.MaxChoose.EditValueChanged += new System.EventHandler(this.MaxChoose_EditValueChanged);
		this.AdvImagePr.EditControl = null;
		this.AdvImagePr.FieldName = null;
		this.AdvImagePr.IsSearchControl = false;
		this.AdvImagePr.Location = new System.Drawing.Point(167, 194);
		this.AdvImagePr.Name = "AdvImagePr";
		this.AdvImagePr.SettingInVisible = false;
		this.AdvImagePr.SettingReadonly = false;
		this.AdvImagePr.Size = new System.Drawing.Size(195, 20);
		this.AdvImagePr.StyleController = this.layoutControl4;
		this.AdvImagePr.TabIndex = 12;
		this.AdvImagePr.EditValueChanged += new System.EventHandler(this.AdvImagePr_EditValueChanged);
		this.DateSep.EditControl = null;
		this.DateSep.FieldName = null;
		this.DateSep.IsSearchControl = false;
		this.DateSep.Location = new System.Drawing.Point(167, 132);
		this.DateSep.Name = "DateSep";
		this.DateSep.SettingInVisible = false;
		this.DateSep.SettingReadonly = false;
		this.DateSep.Size = new System.Drawing.Size(195, 20);
		this.DateSep.StyleController = this.layoutControl4;
		this.DateSep.TabIndex = 11;
		this.DateSep.EditValueChanged += new System.EventHandler(this.DateSep_EditValueChanged);
		this.Color.EditControl = null;
		this.Color.FieldName = null;
		this.Color.IsSearchControl = false;
		this.Color.Location = new System.Drawing.Point(167, 12);
		this.Color.Name = "Color";
		this.Color.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.Color.Properties.IsSearchControl = false;
		this.Color.Properties.NullText = "";
		this.Color.SettingInVisible = false;
		this.Color.SettingReadonly = false;
		this.Color.Size = new System.Drawing.Size(195, 20);
		this.Color.StyleController = this.layoutControl4;
		this.Color.TabIndex = 6;
		this.Color.EditValueChanged += new System.EventHandler(this.lok_Color_EditValueChanged);
		this.DefLengthU.EditControl = null;
		this.DefLengthU.FieldName = null;
		this.DefLengthU.IsSearchControl = false;
		this.DefLengthU.Location = new System.Drawing.Point(167, 36);
		this.DefLengthU.Name = "DefLengthU";
		this.DefLengthU.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DefLengthU.Properties.IsSearchControl = false;
		this.DefLengthU.Properties.NullText = "";
		this.DefLengthU.SettingInVisible = false;
		this.DefLengthU.SettingReadonly = false;
		this.DefLengthU.Size = new System.Drawing.Size(195, 20);
		this.DefLengthU.StyleController = this.layoutControl4;
		this.DefLengthU.TabIndex = 7;
		this.DefLengthU.EditValueChanged += new System.EventHandler(this.DefLengthU_EditValueChanged);
		this.DefWeightU.EditControl = null;
		this.DefWeightU.FieldName = null;
		this.DefWeightU.IsSearchControl = false;
		this.DefWeightU.Location = new System.Drawing.Point(167, 60);
		this.DefWeightU.Name = "DefWeightU";
		this.DefWeightU.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DefWeightU.Properties.IsSearchControl = false;
		this.DefWeightU.Properties.NullText = "";
		this.DefWeightU.SettingInVisible = false;
		this.DefWeightU.SettingReadonly = false;
		this.DefWeightU.Size = new System.Drawing.Size(195, 20);
		this.DefWeightU.StyleController = this.layoutControl4;
		this.DefWeightU.TabIndex = 8;
		this.DefWeightU.EditValueChanged += new System.EventHandler(this.DefWeightU_EditValueChanged);
		this.TimeFormat.EditControl = null;
		this.TimeFormat.FieldName = null;
		this.TimeFormat.IsSearchControl = false;
		this.TimeFormat.Location = new System.Drawing.Point(167, 84);
		this.TimeFormat.Name = "TimeFormat";
		this.TimeFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.TimeFormat.Properties.IsSearchControl = false;
		this.TimeFormat.Properties.NullText = "";
		this.TimeFormat.SettingInVisible = false;
		this.TimeFormat.SettingReadonly = false;
		this.TimeFormat.Size = new System.Drawing.Size(195, 20);
		this.TimeFormat.StyleController = this.layoutControl4;
		this.TimeFormat.TabIndex = 9;
		this.TimeFormat.EditValueChanged += new System.EventHandler(this.TimeFormat_EditValueChanged);
		this.DateFormat.EditControl = null;
		this.DateFormat.FieldName = null;
		this.DateFormat.IsSearchControl = false;
		this.DateFormat.Location = new System.Drawing.Point(167, 108);
		this.DateFormat.Name = "DateFormat";
		this.DateFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DateFormat.Properties.IsSearchControl = false;
		this.DateFormat.Properties.NullText = "";
		this.DateFormat.SettingInVisible = false;
		this.DateFormat.SettingReadonly = false;
		this.DateFormat.Size = new System.Drawing.Size(195, 20);
		this.DateFormat.StyleController = this.layoutControl4;
		this.DateFormat.TabIndex = 10;
		this.DateFormat.EditValueChanged += new System.EventHandler(this.DateFormat_EditValueChanged);
		this.TPLId.EditControl = null;
		this.TPLId.FieldName = null;
		this.TPLId.IsSearchControl = false;
		this.TPLId.Location = new System.Drawing.Point(167, 259);
		this.TPLId.Name = "TPLId";
		this.TPLId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.TPLId.Properties.IsSearchControl = false;
		this.TPLId.Properties.NullText = "";
		this.TPLId.SettingInVisible = false;
		this.TPLId.SettingReadonly = false;
		this.TPLId.Size = new System.Drawing.Size(195, 20);
		this.TPLId.StyleController = this.layoutControl4;
		this.TPLId.TabIndex = 28;
		this.TPLId.EditValueChanged += new System.EventHandler(this.TPLId_EditValueChanged);
		this.PriceDec.EditControl = null;
		this.PriceDec.EditValue = new decimal(new int[4] { 6, 0, 0, 0 });
		this.PriceDec.FieldName = "PriceDec";
		this.PriceDec.IsSearchControl = false;
		this.PriceDec.Location = new System.Drawing.Point(559, 53);
		this.PriceDec.Name = "PriceDec";
		this.PriceDec.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PriceDec.Properties.IsSearchControl = false;
		this.PriceDec.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PriceDec.SettingInVisible = false;
		this.PriceDec.SettingReadonly = false;
		this.PriceDec.Size = new System.Drawing.Size(206, 20);
		this.PriceDec.StyleController = this.layoutControl4;
		this.PriceDec.TabIndex = 15;
		this.PriceDec.EditValueChanged += new System.EventHandler(this.PriceDec_EditValueChanged);
		this.layoutControlGroup3.CustomizationFormText = "Root";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[34]
		{
			this.emptySpaceItem27, this.emptySpaceItem28, this.emptySpaceItem29, this.simpleLabelItem8, this.layoutControlItem53, this.layoutControlItem54, this.layoutControlItem55, this.layoutControlItem56, this.layoutControlItem57, this.layoutControlItem58,
			this.emptySpaceItem32, this.layoutControlItem59, this.layoutControlItem60, this.layoutControlItem61, this.layoutControlItem62, this.layoutControlItem63, this.layoutControlItem64, this.layoutControlItem65, this.layoutControlItem66, this.layoutControlItem67,
			this.layoutControlItem68, this.layoutControlItem69, this.layoutControlItem70, this.emptySpaceItem33, this.simpleLabelItem9, this.layoutControlItem71, this.emptySpaceItem34, this.layoutControlItem72, this.simpleLabelItem10, this.layoutControlItem73,
			this.layoutControlItem74, this.layoutControlItem75, this.emptySpaceItem35, this.emptySpaceItem71
		});
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup3.TextVisible = false;
		this.emptySpaceItem27.AllowHotTrack = false;
		this.emptySpaceItem27.CustomizationFormText = "emptySpaceItem27";
		this.emptySpaceItem27.Location = new System.Drawing.Point(392, 356);
		this.emptySpaceItem27.Name = "emptySpaceItem27";
		this.emptySpaceItem27.Size = new System.Drawing.Size(365, 76);
		this.emptySpaceItem27.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem28.AllowHotTrack = false;
		this.emptySpaceItem28.CustomizationFormText = "emptySpaceItem28";
		this.emptySpaceItem28.Location = new System.Drawing.Point(0, 336);
		this.emptySpaceItem28.Name = "emptySpaceItem28";
		this.emptySpaceItem28.Size = new System.Drawing.Size(354, 96);
		this.emptySpaceItem28.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem29.AllowHotTrack = false;
		this.emptySpaceItem29.CustomizationFormText = "emptySpaceItem29";
		this.emptySpaceItem29.Location = new System.Drawing.Point(757, 0);
		this.emptySpaceItem29.Name = "emptySpaceItem29";
		this.emptySpaceItem29.Size = new System.Drawing.Size(99, 432);
		this.emptySpaceItem29.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem8.AllowHotTrack = false;
		this.simpleLabelItem8.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem8.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem8.CustomizationFormText = "LabelsimpleLabelItem8";
		this.simpleLabelItem8.Location = new System.Drawing.Point(392, 0);
		this.simpleLabelItem8.Name = "simpleLabelItem8";
		this.simpleLabelItem8.Size = new System.Drawing.Size(365, 17);
		this.simpleLabelItem8.Text = "Decimal Places  (0..6)";
		this.simpleLabelItem8.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem53.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem53.Control = this.Color;
		this.layoutControlItem53.CustomizationFormText = "layoutControlItem53";
		this.layoutControlItem53.EditControl = null;
		this.layoutControlItem53.EditValue = "Color";
		this.layoutControlItem53.FieldName = null;
		this.layoutControlItem53.IsSearchControl = false;
		this.layoutControlItem53.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem53.Name = "layoutControlItem53";
		this.layoutControlItem53.ReadOnly = false;
		this.layoutControlItem53.SettingInVisible = false;
		this.layoutControlItem53.SettingReadonly = false;
		this.layoutControlItem53.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem53.Text = "Color";
		this.layoutControlItem53.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem53.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem54.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem54.Control = this.DefLengthU;
		this.layoutControlItem54.CustomizationFormText = "layoutControlItem54";
		this.layoutControlItem54.EditControl = null;
		this.layoutControlItem54.EditValue = "Default Length UoM";
		this.layoutControlItem54.FieldName = null;
		this.layoutControlItem54.IsSearchControl = false;
		this.layoutControlItem54.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem54.Name = "layoutControlItem54";
		this.layoutControlItem54.ReadOnly = false;
		this.layoutControlItem54.SettingInVisible = false;
		this.layoutControlItem54.SettingReadonly = false;
		this.layoutControlItem54.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem54.Text = "Default Length UoM";
		this.layoutControlItem54.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem55.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem55.Control = this.DefWeightU;
		this.layoutControlItem55.CustomizationFormText = "layoutControlItem55";
		this.layoutControlItem55.EditControl = null;
		this.layoutControlItem55.EditValue = "Default Weight UoM";
		this.layoutControlItem55.FieldName = null;
		this.layoutControlItem55.IsSearchControl = false;
		this.layoutControlItem55.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem55.Name = "layoutControlItem55";
		this.layoutControlItem55.ReadOnly = false;
		this.layoutControlItem55.SettingInVisible = false;
		this.layoutControlItem55.SettingReadonly = false;
		this.layoutControlItem55.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem55.Text = "Default Weight UoM";
		this.layoutControlItem55.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem56.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem56.Control = this.TimeFormat;
		this.layoutControlItem56.CustomizationFormText = "layoutControlItem56";
		this.layoutControlItem56.EditControl = null;
		this.layoutControlItem56.EditValue = "Time Format";
		this.layoutControlItem56.FieldName = null;
		this.layoutControlItem56.IsSearchControl = false;
		this.layoutControlItem56.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem56.Name = "layoutControlItem56";
		this.layoutControlItem56.ReadOnly = false;
		this.layoutControlItem56.SettingInVisible = false;
		this.layoutControlItem56.SettingReadonly = false;
		this.layoutControlItem56.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem56.Text = "Time Format";
		this.layoutControlItem56.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem57.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem57.Control = this.DateFormat;
		this.layoutControlItem57.CustomizationFormText = "layoutControlItem57";
		this.layoutControlItem57.EditControl = null;
		this.layoutControlItem57.EditValue = "Date Format";
		this.layoutControlItem57.FieldName = null;
		this.layoutControlItem57.IsSearchControl = false;
		this.layoutControlItem57.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem57.Name = "layoutControlItem57";
		this.layoutControlItem57.ReadOnly = false;
		this.layoutControlItem57.SettingInVisible = false;
		this.layoutControlItem57.SettingReadonly = false;
		this.layoutControlItem57.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem57.Text = "Date Format";
		this.layoutControlItem57.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem58.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem58.Control = this.DateSep;
		this.layoutControlItem58.CustomizationFormText = "layoutControlItem58";
		this.layoutControlItem58.EditControl = null;
		this.layoutControlItem58.EditValue = "Separator";
		this.layoutControlItem58.FieldName = null;
		this.layoutControlItem58.IsSearchControl = false;
		this.layoutControlItem58.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem58.Name = "layoutControlItem58";
		this.layoutControlItem58.ReadOnly = false;
		this.layoutControlItem58.SettingInVisible = false;
		this.layoutControlItem58.SettingReadonly = false;
		this.layoutControlItem58.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem58.Text = "Separator";
		this.layoutControlItem58.TextSize = new System.Drawing.Size(152, 13);
		this.emptySpaceItem32.AllowHotTrack = false;
		this.emptySpaceItem32.CustomizationFormText = "emptySpaceItem32";
		this.emptySpaceItem32.Location = new System.Drawing.Point(0, 144);
		this.emptySpaceItem32.Name = "emptySpaceItem32";
		this.emptySpaceItem32.Size = new System.Drawing.Size(354, 14);
		this.emptySpaceItem32.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem59.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem59.Control = this.AdvImagePr;
		this.layoutControlItem59.CustomizationFormText = "layoutControlItem59";
		this.layoutControlItem59.EditControl = null;
		this.layoutControlItem59.EditValue = "Ext. Image Processing";
		this.layoutControlItem59.FieldName = null;
		this.layoutControlItem59.IsSearchControl = false;
		this.layoutControlItem59.Location = new System.Drawing.Point(0, 182);
		this.layoutControlItem59.Name = "layoutControlItem59";
		this.layoutControlItem59.ReadOnly = false;
		this.layoutControlItem59.SettingInVisible = false;
		this.layoutControlItem59.SettingReadonly = false;
		this.layoutControlItem59.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem59.Text = "Ext. Image Processing";
		this.layoutControlItem59.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem60.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem60.Control = this.MaxChoose;
		this.layoutControlItem60.CustomizationFormText = "layoutControlItem60";
		this.layoutControlItem60.EditControl = null;
		this.layoutControlItem60.EditValue = "No. of Rows in 'List of' Windows";
		this.layoutControlItem60.FieldName = null;
		this.layoutControlItem60.IsSearchControl = false;
		this.layoutControlItem60.Location = new System.Drawing.Point(0, 206);
		this.layoutControlItem60.Name = "layoutControlItem60";
		this.layoutControlItem60.ReadOnly = false;
		this.layoutControlItem60.SettingInVisible = false;
		this.layoutControlItem60.SettingReadonly = false;
		this.layoutControlItem60.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem60.Text = "No. of Rows in 'List of' Windows";
		this.layoutControlItem60.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem61.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem61.Control = this.SumDec;
		this.layoutControlItem61.CustomizationFormText = "layoutControlItem61";
		this.layoutControlItem61.EditControl = null;
		this.layoutControlItem61.EditValue = "Amounts";
		this.layoutControlItem61.FieldName = null;
		this.layoutControlItem61.IsSearchControl = false;
		this.layoutControlItem61.Location = new System.Drawing.Point(392, 17);
		this.layoutControlItem61.Name = "layoutControlItem61";
		this.layoutControlItem61.ReadOnly = false;
		this.layoutControlItem61.SettingInVisible = false;
		this.layoutControlItem61.SettingReadonly = false;
		this.layoutControlItem61.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem61.Text = "Amounts";
		this.layoutControlItem61.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem62.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem62.Control = this.PriceDec;
		this.layoutControlItem62.CustomizationFormText = "layoutControlItem62";
		this.layoutControlItem62.EditControl = null;
		this.layoutControlItem62.EditValue = "Prices";
		this.layoutControlItem62.FieldName = null;
		this.layoutControlItem62.IsSearchControl = false;
		this.layoutControlItem62.Location = new System.Drawing.Point(392, 41);
		this.layoutControlItem62.Name = "layoutControlItem62";
		this.layoutControlItem62.ReadOnly = false;
		this.layoutControlItem62.SettingInVisible = false;
		this.layoutControlItem62.SettingReadonly = false;
		this.layoutControlItem62.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem62.Text = "Prices";
		this.layoutControlItem62.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem63.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem63.Control = this.RateDec;
		this.layoutControlItem63.CustomizationFormText = "layoutControlItem63";
		this.layoutControlItem63.EditControl = null;
		this.layoutControlItem63.EditValue = "Rates";
		this.layoutControlItem63.FieldName = null;
		this.layoutControlItem63.IsSearchControl = false;
		this.layoutControlItem63.Location = new System.Drawing.Point(392, 65);
		this.layoutControlItem63.Name = "layoutControlItem63";
		this.layoutControlItem63.ReadOnly = false;
		this.layoutControlItem63.SettingInVisible = false;
		this.layoutControlItem63.SettingReadonly = false;
		this.layoutControlItem63.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem63.Text = "Rates";
		this.layoutControlItem63.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem64.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem64.Control = this.QtyDec;
		this.layoutControlItem64.CustomizationFormText = "layoutControlItem64";
		this.layoutControlItem64.EditControl = null;
		this.layoutControlItem64.EditValue = "Quantities";
		this.layoutControlItem64.FieldName = null;
		this.layoutControlItem64.IsSearchControl = false;
		this.layoutControlItem64.Location = new System.Drawing.Point(392, 89);
		this.layoutControlItem64.Name = "layoutControlItem64";
		this.layoutControlItem64.ReadOnly = false;
		this.layoutControlItem64.SettingInVisible = false;
		this.layoutControlItem64.SettingReadonly = false;
		this.layoutControlItem64.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem64.Text = "Quantities";
		this.layoutControlItem64.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem65.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem65.Control = this.PercentDec;
		this.layoutControlItem65.CustomizationFormText = "layoutControlItem65";
		this.layoutControlItem65.EditControl = null;
		this.layoutControlItem65.EditValue = "Percent";
		this.layoutControlItem65.FieldName = null;
		this.layoutControlItem65.IsSearchControl = false;
		this.layoutControlItem65.Location = new System.Drawing.Point(392, 113);
		this.layoutControlItem65.Name = "layoutControlItem65";
		this.layoutControlItem65.ReadOnly = false;
		this.layoutControlItem65.SettingInVisible = false;
		this.layoutControlItem65.SettingReadonly = false;
		this.layoutControlItem65.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem65.Text = "Percent";
		this.layoutControlItem65.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem66.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem66.Control = this.MeasureDec;
		this.layoutControlItem66.CustomizationFormText = "layoutControlItem66";
		this.layoutControlItem66.EditControl = null;
		this.layoutControlItem66.EditValue = "Units";
		this.layoutControlItem66.FieldName = null;
		this.layoutControlItem66.IsSearchControl = false;
		this.layoutControlItem66.Location = new System.Drawing.Point(392, 137);
		this.layoutControlItem66.Name = "layoutControlItem66";
		this.layoutControlItem66.ReadOnly = false;
		this.layoutControlItem66.SettingInVisible = false;
		this.layoutControlItem66.SettingReadonly = false;
		this.layoutControlItem66.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem66.Text = "Units";
		this.layoutControlItem66.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem67.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem67.Control = this.QueryDec;
		this.layoutControlItem67.CustomizationFormText = "layoutControlItem67";
		this.layoutControlItem67.EditControl = null;
		this.layoutControlItem67.EditValue = "Decimals in Query";
		this.layoutControlItem67.FieldName = null;
		this.layoutControlItem67.IsSearchControl = false;
		this.layoutControlItem67.Location = new System.Drawing.Point(392, 161);
		this.layoutControlItem67.Name = "layoutControlItem67";
		this.layoutControlItem67.ReadOnly = false;
		this.layoutControlItem67.SettingInVisible = false;
		this.layoutControlItem67.SettingReadonly = false;
		this.layoutControlItem67.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem67.Text = "Decimals in Query";
		this.layoutControlItem67.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem68.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem68.Control = this.DecSep;
		this.layoutControlItem68.CustomizationFormText = "layoutControlItem68";
		this.layoutControlItem68.EditControl = null;
		this.layoutControlItem68.EditValue = "Separator";
		this.layoutControlItem68.FieldName = null;
		this.layoutControlItem68.IsSearchControl = false;
		this.layoutControlItem68.Location = new System.Drawing.Point(392, 185);
		this.layoutControlItem68.Name = "layoutControlItem68";
		this.layoutControlItem68.ReadOnly = false;
		this.layoutControlItem68.SettingInVisible = false;
		this.layoutControlItem68.SettingReadonly = false;
		this.layoutControlItem68.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem68.Text = "Separator";
		this.layoutControlItem68.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem69.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem69.Control = this.ThousSep;
		this.layoutControlItem69.CustomizationFormText = "layoutControlItem69";
		this.layoutControlItem69.EditControl = null;
		this.layoutControlItem69.EditValue = "Thousands Sep.";
		this.layoutControlItem69.FieldName = null;
		this.layoutControlItem69.IsSearchControl = false;
		this.layoutControlItem69.Location = new System.Drawing.Point(392, 209);
		this.layoutControlItem69.Name = "layoutControlItem69";
		this.layoutControlItem69.ReadOnly = false;
		this.layoutControlItem69.SettingInVisible = false;
		this.layoutControlItem69.SettingReadonly = false;
		this.layoutControlItem69.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem69.Text = "Thousands Sep.";
		this.layoutControlItem69.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem70.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem70.Control = this.CurOnRight;
		this.layoutControlItem70.CustomizationFormText = "layoutControlItem70";
		this.layoutControlItem70.EditControl = null;
		this.layoutControlItem70.EditValue = "layoutControlItem70";
		this.layoutControlItem70.FieldName = null;
		this.layoutControlItem70.IsSearchControl = false;
		this.layoutControlItem70.Location = new System.Drawing.Point(392, 243);
		this.layoutControlItem70.Name = "layoutControlItem70";
		this.layoutControlItem70.ReadOnly = false;
		this.layoutControlItem70.SettingInVisible = false;
		this.layoutControlItem70.SettingReadonly = false;
		this.layoutControlItem70.Size = new System.Drawing.Size(365, 24);
		this.layoutControlItem70.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem70.TextVisible = false;
		this.emptySpaceItem33.AllowHotTrack = false;
		this.emptySpaceItem33.CustomizationFormText = "emptySpaceItem33";
		this.emptySpaceItem33.Location = new System.Drawing.Point(392, 267);
		this.emptySpaceItem33.Name = "emptySpaceItem33";
		this.emptySpaceItem33.Size = new System.Drawing.Size(365, 10);
		this.emptySpaceItem33.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem9.AllowHotTrack = false;
		this.simpleLabelItem9.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem9.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem9.CustomizationFormText = "LabelsimpleLabelItem9";
		this.simpleLabelItem9.Location = new System.Drawing.Point(392, 277);
		this.simpleLabelItem9.Name = "simpleLabelItem9";
		this.simpleLabelItem9.Size = new System.Drawing.Size(365, 17);
		this.simpleLabelItem9.Text = "Exchange Rate Posting";
		this.simpleLabelItem9.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem71.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem71.Control = this.DirectRate;
		this.layoutControlItem71.CustomizationFormText = "layoutControlItem71";
		this.layoutControlItem71.EditControl = null;
		this.layoutControlItem71.EditValue = "layoutControlItem71";
		this.layoutControlItem71.FieldName = null;
		this.layoutControlItem71.IsSearchControl = false;
		this.layoutControlItem71.Location = new System.Drawing.Point(392, 294);
		this.layoutControlItem71.Name = "layoutControlItem71";
		this.layoutControlItem71.ReadOnly = false;
		this.layoutControlItem71.SettingInVisible = false;
		this.layoutControlItem71.SettingReadonly = false;
		this.layoutControlItem71.Size = new System.Drawing.Size(365, 62);
		this.layoutControlItem71.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem71.TextVisible = false;
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.CustomizationFormText = "emptySpaceItem34";
		this.emptySpaceItem34.Location = new System.Drawing.Point(392, 233);
		this.emptySpaceItem34.Name = "emptySpaceItem34";
		this.emptySpaceItem34.Size = new System.Drawing.Size(365, 10);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem72.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem72.Control = this.ck_ManagerCompany;
		this.layoutControlItem72.CustomizationFormText = "layoutControlItem72";
		this.layoutControlItem72.EditControl = null;
		this.layoutControlItem72.EditValue = "layoutControlItem72";
		this.layoutControlItem72.FieldName = null;
		this.layoutControlItem72.IsSearchControl = false;
		this.layoutControlItem72.Location = new System.Drawing.Point(0, 158);
		this.layoutControlItem72.Name = "layoutControlItem72";
		this.layoutControlItem72.ReadOnly = false;
		this.layoutControlItem72.SettingInVisible = false;
		this.layoutControlItem72.SettingReadonly = false;
		this.layoutControlItem72.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem72.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem72.TextVisible = false;
		this.simpleLabelItem10.AllowHotTrack = false;
		this.simpleLabelItem10.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem10.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem10.CustomizationFormText = "Choose from List Preferences";
		this.simpleLabelItem10.Location = new System.Drawing.Point(0, 271);
		this.simpleLabelItem10.Name = "simpleLabelItem10";
		this.simpleLabelItem10.Size = new System.Drawing.Size(354, 17);
		this.simpleLabelItem10.Text = "Choose from List Preferences";
		this.simpleLabelItem10.TextSize = new System.Drawing.Size(152, 13);
		this.layoutControlItem73.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem73.Control = this.B1SgtEb;
		this.layoutControlItem73.CustomizationFormText = "layoutControlItem73";
		this.layoutControlItem73.EditControl = null;
		this.layoutControlItem73.EditValue = "layoutControlItem73";
		this.layoutControlItem73.FieldName = null;
		this.layoutControlItem73.IsSearchControl = false;
		this.layoutControlItem73.Location = new System.Drawing.Point(0, 288);
		this.layoutControlItem73.Name = "layoutControlItem73";
		this.layoutControlItem73.ReadOnly = false;
		this.layoutControlItem73.SettingInVisible = false;
		this.layoutControlItem73.SettingReadonly = false;
		this.layoutControlItem73.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem73.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem73.TextVisible = false;
		this.layoutControlItem74.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem74.Control = this.TxtSrch;
		this.layoutControlItem74.CustomizationFormText = "layoutControlItem74";
		this.layoutControlItem74.EditControl = null;
		this.layoutControlItem74.EditValue = "layoutControlItem74";
		this.layoutControlItem74.FieldName = null;
		this.layoutControlItem74.IsSearchControl = false;
		this.layoutControlItem74.Location = new System.Drawing.Point(0, 312);
		this.layoutControlItem74.Name = "layoutControlItem74";
		this.layoutControlItem74.ReadOnly = false;
		this.layoutControlItem74.SettingInVisible = false;
		this.layoutControlItem74.SettingReadonly = false;
		this.layoutControlItem74.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem74.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem74.TextVisible = false;
		this.layoutControlItem75.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem75.Control = this.TPLId;
		this.layoutControlItem75.CustomizationFormText = "Default UI Template";
		this.layoutControlItem75.EditControl = null;
		this.layoutControlItem75.EditValue = "Default UI Template";
		this.layoutControlItem75.FieldName = null;
		this.layoutControlItem75.IsSearchControl = false;
		this.layoutControlItem75.Location = new System.Drawing.Point(0, 247);
		this.layoutControlItem75.Name = "layoutControlItem75";
		this.layoutControlItem75.ReadOnly = false;
		this.layoutControlItem75.SettingInVisible = false;
		this.layoutControlItem75.SettingReadonly = false;
		this.layoutControlItem75.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem75.Text = "Default UI Template";
		this.layoutControlItem75.TextSize = new System.Drawing.Size(152, 13);
		this.emptySpaceItem35.AllowHotTrack = false;
		this.emptySpaceItem35.CustomizationFormText = "emptySpaceItem35";
		this.emptySpaceItem35.Location = new System.Drawing.Point(0, 230);
		this.emptySpaceItem35.Name = "emptySpaceItem35";
		this.emptySpaceItem35.Size = new System.Drawing.Size(354, 17);
		this.emptySpaceItem35.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem71.AllowHotTrack = false;
		this.emptySpaceItem71.CustomizationFormText = "emptySpaceItem71";
		this.emptySpaceItem71.Location = new System.Drawing.Point(354, 0);
		this.emptySpaceItem71.Name = "emptySpaceItem71";
		this.emptySpaceItem71.Size = new System.Drawing.Size(38, 432);
		this.emptySpaceItem71.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_Font.Controls.Add(this.layoutControl5);
		this.Tab_Font.EditControl = null;
		this.Tab_Font.EditValue = "Font & Bkgd";
		this.Tab_Font.FieldName = "Tab_Font";
		this.Tab_Font.IsSearchControl = false;
		this.Tab_Font.Name = "Tab_Font";
		this.Tab_Font.ReadOnly = false;
		this.Tab_Font.SettingInVisible = false;
		this.Tab_Font.SettingReadonly = false;
		this.Tab_Font.Size = new System.Drawing.Size(876, 452);
		this.Tab_Font.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Font.Text = "Font & Bkgd";
		this.layoutControl5.Controls.Add(this.txt_PicBox);
		this.layoutControl5.Controls.Add(this.simpleButton1);
		this.layoutControl5.Controls.Add(this.WllPprDsp);
		this.layoutControl5.Controls.Add(this.lok_Fonts);
		this.layoutControl5.Controls.Add(this.lok_fontSize);
		this.layoutControl5.Controls.Add(this.txt_PreviewFont);
		this.layoutControl5.Controls.Add(this.WallPaper);
		this.layoutControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl5.Location = new System.Drawing.Point(0, 0);
		this.layoutControl5.Name = "layoutControl5";
		this.layoutControl5.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(728, 296, 250, 350);
		this.layoutControl5.Root = this.layoutControlGroup4;
		this.layoutControl5.Size = new System.Drawing.Size(876, 452);
		this.layoutControl5.TabIndex = 0;
		this.layoutControl5.Text = "layoutControl5";
		this.txt_PicBox.Location = new System.Drawing.Point(328, 79);
		this.txt_PicBox.Name = "txt_PicBox";
		this.txt_PicBox.Size = new System.Drawing.Size(283, 83);
		this.txt_PicBox.TabIndex = 8;
		this.txt_PicBox.TabStop = false;
		this.txt_PicBox.Click += new System.EventHandler(this.pictureBox1_Click);
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Browse...";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(533, 36);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(78, 22);
		this.simpleButton1.StyleController = this.layoutControl5;
		this.simpleButton1.TabIndex = 7;
		this.simpleButton1.Text = "Browse...";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.WllPprDsp.EditControl = null;
		this.WllPprDsp.FieldName = null;
		this.WllPprDsp.IsSearchControl = false;
		this.WllPprDsp.Location = new System.Drawing.Point(398, 166);
		this.WllPprDsp.Name = "WllPprDsp";
		this.WllPprDsp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.WllPprDsp.Properties.IsSearchControl = false;
		this.WllPprDsp.Properties.NullText = "";
		this.WllPprDsp.SettingInVisible = false;
		this.WllPprDsp.SettingReadonly = false;
		this.WllPprDsp.Size = new System.Drawing.Size(213, 20);
		this.WllPprDsp.StyleController = this.layoutControl5;
		this.WllPprDsp.TabIndex = 10;
		this.WllPprDsp.EditValueChanged += new System.EventHandler(this.textEdit33_EditValueChanged);
		this.lok_Fonts.Location = new System.Drawing.Point(82, 12);
		this.lok_Fonts.Name = "lok_Fonts";
		this.lok_Fonts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lok_Fonts.Properties.PopupSizeable = true;
		this.lok_Fonts.Size = new System.Drawing.Size(242, 20);
		this.lok_Fonts.StyleController = this.layoutControl5;
		this.lok_Fonts.TabIndex = 4;
		this.lok_Fonts.SelectedIndexChanged += new System.EventHandler(this.lok_Fonts_SelectedIndexChanged);
		this.lok_Fonts.EditValueChanged += new System.EventHandler(this.textEdit30_EditValueChanged);
		this.lok_fontSize.EditControl = null;
		this.lok_fontSize.FieldName = null;
		this.lok_fontSize.IsSearchControl = false;
		this.lok_fontSize.Location = new System.Drawing.Point(82, 36);
		this.lok_fontSize.Name = "lok_fontSize";
		this.lok_fontSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lok_fontSize.Properties.Items.AddRange(new object[14]
		{
			"\t", "9\t", "10\t", "11\t", "12\t", "14\t", "16\t", "18\t", "20\t", "22\t",
			"24\t", "26\t", "28\t", "36\t"
		});
		this.lok_fontSize.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lok_fontSize.SettingInVisible = false;
		this.lok_fontSize.SettingReadonly = false;
		this.lok_fontSize.Size = new System.Drawing.Size(242, 20);
		this.lok_fontSize.StyleController = this.layoutControl5;
		this.lok_fontSize.TabIndex = 5;
		this.lok_fontSize.SelectedIndexChanged += new System.EventHandler(this.lok_fontSize_SelectedIndexChanged);
		this.lok_fontSize.EditValueChanged += new System.EventHandler(this.textEdit31_EditValueChanged);
		this.txt_PreviewFont.EditControl = null;
		this.txt_PreviewFont.EditValue = "AABBCCXXYYZZ";
		this.txt_PreviewFont.FieldName = null;
		this.txt_PreviewFont.IsSearchControl = false;
		this.txt_PreviewFont.Location = new System.Drawing.Point(12, 77);
		this.txt_PreviewFont.Name = "txt_PreviewFont";
		this.txt_PreviewFont.Properties.Appearance.Options.UseTextOptions = true;
		this.txt_PreviewFont.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.txt_PreviewFont.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.txt_PreviewFont.SettingInVisible = false;
		this.txt_PreviewFont.SettingReadonly = false;
		this.txt_PreviewFont.Size = new System.Drawing.Size(312, 85);
		this.txt_PreviewFont.StyleController = this.layoutControl5;
		this.txt_PreviewFont.TabIndex = 9;
		this.txt_PreviewFont.EditValueChanged += new System.EventHandler(this.memoExEdit1_EditValueChanged);
		this.WallPaper.EditControl = null;
		this.WallPaper.FieldName = null;
		this.WallPaper.IsSearchControl = false;
		this.WallPaper.Location = new System.Drawing.Point(398, 12);
		this.WallPaper.Name = "WallPaper";
		this.WallPaper.SettingInVisible = false;
		this.WallPaper.SettingReadonly = false;
		this.WallPaper.Size = new System.Drawing.Size(213, 20);
		this.WallPaper.StyleController = this.layoutControl5;
		this.WallPaper.TabIndex = 6;
		this.WallPaper.EditValueChanged += new System.EventHandler(this.WallPaper_EditValueChanged);
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.layoutControlItem76, this.layoutControlItem77, this.layoutControlItem78, this.layoutControlItem79, this.emptySpaceItem40, this.simpleLabelItem11, this.simpleLabelItem12, this.layoutControlItem80, this.layoutControlItem81, this.emptySpaceItem38,
			this.emptySpaceItem37, this.layoutControlItem82, this.emptySpaceItem42
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup4.TextVisible = false;
		this.layoutControlItem76.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem76.Control = this.lok_Fonts;
		this.layoutControlItem76.CustomizationFormText = "layoutControlItem76";
		this.layoutControlItem76.EditControl = null;
		this.layoutControlItem76.EditValue = "Font";
		this.layoutControlItem76.FieldName = null;
		this.layoutControlItem76.IsSearchControl = false;
		this.layoutControlItem76.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem76.Name = "layoutControlItem76";
		this.layoutControlItem76.ReadOnly = false;
		this.layoutControlItem76.SettingInVisible = false;
		this.layoutControlItem76.SettingReadonly = false;
		this.layoutControlItem76.Size = new System.Drawing.Size(316, 24);
		this.layoutControlItem76.Text = "Font";
		this.layoutControlItem76.TextSize = new System.Drawing.Size(67, 13);
		this.layoutControlItem77.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem77.Control = this.lok_fontSize;
		this.layoutControlItem77.CustomizationFormText = "layoutControlItem77";
		this.layoutControlItem77.EditControl = null;
		this.layoutControlItem77.EditValue = "Font Size";
		this.layoutControlItem77.FieldName = null;
		this.layoutControlItem77.IsSearchControl = false;
		this.layoutControlItem77.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem77.Name = "layoutControlItem77";
		this.layoutControlItem77.ReadOnly = false;
		this.layoutControlItem77.SettingInVisible = false;
		this.layoutControlItem77.SettingReadonly = false;
		this.layoutControlItem77.Size = new System.Drawing.Size(316, 24);
		this.layoutControlItem77.Text = "Font Size";
		this.layoutControlItem77.TextSize = new System.Drawing.Size(67, 13);
		this.layoutControlItem78.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem78.Control = this.WallPaper;
		this.layoutControlItem78.CustomizationFormText = "layoutControlItem78";
		this.layoutControlItem78.EditControl = null;
		this.layoutControlItem78.EditValue = "Background";
		this.layoutControlItem78.FieldName = null;
		this.layoutControlItem78.IsSearchControl = false;
		this.layoutControlItem78.Location = new System.Drawing.Point(316, 0);
		this.layoutControlItem78.Name = "layoutControlItem78";
		this.layoutControlItem78.ReadOnly = false;
		this.layoutControlItem78.SettingInVisible = false;
		this.layoutControlItem78.SettingReadonly = false;
		this.layoutControlItem78.Size = new System.Drawing.Size(287, 24);
		this.layoutControlItem78.Text = "Background";
		this.layoutControlItem78.TextSize = new System.Drawing.Size(67, 13);
		this.layoutControlItem79.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem79.Control = this.simpleButton1;
		this.layoutControlItem79.CustomizationFormText = "layoutControlItem79";
		this.layoutControlItem79.EditControl = null;
		this.layoutControlItem79.EditValue = "layoutControlItem79";
		this.layoutControlItem79.FieldName = null;
		this.layoutControlItem79.IsSearchControl = false;
		this.layoutControlItem79.Location = new System.Drawing.Point(521, 24);
		this.layoutControlItem79.Name = "layoutControlItem79";
		this.layoutControlItem79.ReadOnly = false;
		this.layoutControlItem79.SettingInVisible = false;
		this.layoutControlItem79.SettingReadonly = false;
		this.layoutControlItem79.Size = new System.Drawing.Size(82, 26);
		this.layoutControlItem79.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem79.TextVisible = false;
		this.emptySpaceItem40.AllowHotTrack = false;
		this.emptySpaceItem40.CustomizationFormText = "emptySpaceItem40";
		this.emptySpaceItem40.Location = new System.Drawing.Point(316, 24);
		this.emptySpaceItem40.Name = "emptySpaceItem40";
		this.emptySpaceItem40.Size = new System.Drawing.Size(205, 26);
		this.emptySpaceItem40.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem11.AllowHotTrack = false;
		this.simpleLabelItem11.CustomizationFormText = "LabelsimpleLabelItem11";
		this.simpleLabelItem11.Location = new System.Drawing.Point(0, 48);
		this.simpleLabelItem11.Name = "simpleLabelItem11";
		this.simpleLabelItem11.Size = new System.Drawing.Size(316, 17);
		this.simpleLabelItem11.Text = "Preview";
		this.simpleLabelItem11.TextSize = new System.Drawing.Size(67, 13);
		this.simpleLabelItem12.AllowHotTrack = false;
		this.simpleLabelItem12.CustomizationFormText = "LabelsimpleLabelItem12";
		this.simpleLabelItem12.Location = new System.Drawing.Point(316, 50);
		this.simpleLabelItem12.Name = "simpleLabelItem12";
		this.simpleLabelItem12.Size = new System.Drawing.Size(287, 17);
		this.simpleLabelItem12.Text = "Preview";
		this.simpleLabelItem12.TextSize = new System.Drawing.Size(67, 13);
		this.layoutControlItem80.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem80.Control = this.txt_PicBox;
		this.layoutControlItem80.CustomizationFormText = "layoutControlItem80";
		this.layoutControlItem80.EditControl = null;
		this.layoutControlItem80.EditValue = "layoutControlItem80";
		this.layoutControlItem80.FieldName = null;
		this.layoutControlItem80.IsSearchControl = false;
		this.layoutControlItem80.Location = new System.Drawing.Point(316, 67);
		this.layoutControlItem80.Name = "layoutControlItem80";
		this.layoutControlItem80.ReadOnly = false;
		this.layoutControlItem80.SettingInVisible = false;
		this.layoutControlItem80.SettingReadonly = false;
		this.layoutControlItem80.Size = new System.Drawing.Size(287, 87);
		this.layoutControlItem80.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem80.TextVisible = false;
		this.layoutControlItem81.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem81.Control = this.txt_PreviewFont;
		this.layoutControlItem81.CustomizationFormText = "layoutControlItem81";
		this.layoutControlItem81.EditControl = null;
		this.layoutControlItem81.EditValue = "layoutControlItem81";
		this.layoutControlItem81.FieldName = null;
		this.layoutControlItem81.IsSearchControl = false;
		this.layoutControlItem81.Location = new System.Drawing.Point(0, 65);
		this.layoutControlItem81.Name = "layoutControlItem81";
		this.layoutControlItem81.ReadOnly = false;
		this.layoutControlItem81.SettingInVisible = false;
		this.layoutControlItem81.SettingReadonly = false;
		this.layoutControlItem81.Size = new System.Drawing.Size(316, 89);
		this.layoutControlItem81.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem81.TextVisible = false;
		this.emptySpaceItem38.AllowHotTrack = false;
		this.emptySpaceItem38.CustomizationFormText = "emptySpaceItem38";
		this.emptySpaceItem38.Location = new System.Drawing.Point(603, 0);
		this.emptySpaceItem38.Name = "emptySpaceItem38";
		this.emptySpaceItem38.Size = new System.Drawing.Size(253, 432);
		this.emptySpaceItem38.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem37.AllowHotTrack = false;
		this.emptySpaceItem37.CustomizationFormText = "emptySpaceItem37";
		this.emptySpaceItem37.Location = new System.Drawing.Point(0, 178);
		this.emptySpaceItem37.Name = "emptySpaceItem37";
		this.emptySpaceItem37.Size = new System.Drawing.Size(603, 254);
		this.emptySpaceItem37.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem82.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem82.Control = this.WllPprDsp;
		this.layoutControlItem82.CustomizationFormText = "layoutControlItem82";
		this.layoutControlItem82.EditControl = null;
		this.layoutControlItem82.EditValue = "Image Display";
		this.layoutControlItem82.FieldName = null;
		this.layoutControlItem82.IsSearchControl = false;
		this.layoutControlItem82.Location = new System.Drawing.Point(316, 154);
		this.layoutControlItem82.Name = "layoutControlItem82";
		this.layoutControlItem82.ReadOnly = false;
		this.layoutControlItem82.SettingInVisible = false;
		this.layoutControlItem82.SettingReadonly = false;
		this.layoutControlItem82.Size = new System.Drawing.Size(287, 24);
		this.layoutControlItem82.Text = "Image Display";
		this.layoutControlItem82.TextSize = new System.Drawing.Size(67, 13);
		this.emptySpaceItem42.AllowHotTrack = false;
		this.emptySpaceItem42.CustomizationFormText = "emptySpaceItem42";
		this.emptySpaceItem42.Location = new System.Drawing.Point(0, 154);
		this.emptySpaceItem42.Name = "emptySpaceItem42";
		this.emptySpaceItem42.Size = new System.Drawing.Size(316, 24);
		this.emptySpaceItem42.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_Path.Controls.Add(this.layoutControl6);
		this.Tab_Path.EditControl = null;
		this.Tab_Path.EditValue = "Path";
		this.Tab_Path.FieldName = "Tab_Path";
		this.Tab_Path.IsSearchControl = false;
		this.Tab_Path.Name = "Tab_Path";
		this.Tab_Path.ReadOnly = false;
		this.Tab_Path.SettingInVisible = false;
		this.Tab_Path.SettingReadonly = false;
		this.Tab_Path.Size = new System.Drawing.Size(876, 452);
		this.Tab_Path.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Path.Text = "Path";
		this.layoutControl6.Controls.Add(this.DisableSplitFolder);
		this.layoutControl6.Controls.Add(this.grd_ObjAtt);
		this.layoutControl6.Controls.Add(this.btn_XML);
		this.layoutControl6.Controls.Add(this.btn_Current);
		this.layoutControl6.Controls.Add(this.btn_Extension);
		this.layoutControl6.Controls.Add(this.btn_Att);
		this.layoutControl6.Controls.Add(this.btn_Pic);
		this.layoutControl6.Controls.Add(this.btn_Excel);
		this.layoutControl6.Controls.Add(this.btn_MicWord);
		this.layoutControl6.Controls.Add(this.XmlPath);
		this.layoutControl6.Controls.Add(this.txt_CurrentScander);
		this.layoutControl6.Controls.Add(this.ExtPath);
		this.layoutControl6.Controls.Add(this.AttachPath);
		this.layoutControl6.Controls.Add(this.BitmapPath);
		this.layoutControl6.Controls.Add(this.ExcelPath);
		this.layoutControl6.Controls.Add(this.WordPath);
		this.layoutControl6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl6.Location = new System.Drawing.Point(0, 0);
		this.layoutControl6.Name = "layoutControl6";
		this.layoutControl6.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1116, 315, 435, 350);
		this.layoutControl6.Root = this.layoutControlGroup5;
		this.layoutControl6.Size = new System.Drawing.Size(876, 452);
		this.layoutControl6.TabIndex = 0;
		this.layoutControl6.Text = "layoutControl6";
		this.DisableSplitFolder.EditControl = null;
		this.DisableSplitFolder.FieldName = null;
		this.DisableSplitFolder.IsSearchControl = false;
		this.DisableSplitFolder.Location = new System.Drawing.Point(12, 194);
		this.DisableSplitFolder.Name = "DisableSplitFolder";
		this.DisableSplitFolder.Properties.Caption = "Disable split files into sub folder by date";
		this.DisableSplitFolder.Properties.DisplayValueChecked = "Y";
		this.DisableSplitFolder.Properties.DisplayValueUnchecked = "N";
		this.DisableSplitFolder.Properties.ValueChecked = "Y";
		this.DisableSplitFolder.Properties.ValueGrayed = "";
		this.DisableSplitFolder.Properties.ValueUnchecked = "N";
		this.DisableSplitFolder.SettingInVisible = false;
		this.DisableSplitFolder.SettingReadonly = false;
		this.DisableSplitFolder.Size = new System.Drawing.Size(497, 20);
		this.DisableSplitFolder.StyleController = this.layoutControl6;
		this.DisableSplitFolder.TabIndex = 24;
		this.DisableSplitFolder.CheckedChanged += new System.EventHandler(this.DisableSplitFolder_CheckedChanged);
		this.grd_ObjAtt.AllowPasteExcel = false;
		this.grd_ObjAtt.ClipboardData = null;
		this.grd_ObjAtt.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_ObjAtt.Location = new System.Drawing.Point(12, 218);
		this.grd_ObjAtt.MainView = this.gv_ObjAtt;
		this.grd_ObjAtt.Name = "grd_ObjAtt";
		this.grd_ObjAtt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.bte_Path, this.cbo_ObjType, this.luk_ObjType });
		this.grd_ObjAtt.Size = new System.Drawing.Size(497, 189);
		this.grd_ObjAtt.TabIndex = 18;
		this.grd_ObjAtt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ObjAtt });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem1.Text = "Remove";
		this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
		this.gv_ObjAtt.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_ObjAtt.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ObjAtt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ObjAtt.ColumnKey = null;
		this.gv_ObjAtt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx1, this.gridColumnEx2 });
		this.gv_ObjAtt.DisableFormSetting = false;
		this.gv_ObjAtt.DisplayException = false;
		this.gv_ObjAtt.EditColumn = null;
		this.gv_ObjAtt.FieldCurrency = null;
		this.gv_ObjAtt.FieldRate = null;
		this.gv_ObjAtt.FormId = "oGeneralSeting";
		this.gv_ObjAtt.GridControl = this.grd_ObjAtt;
		this.gv_ObjAtt.IgnoreFilter = false;
		this.gv_ObjAtt.IndexEditRow = -1;
		this.gv_ObjAtt.IndicatorWidth = 35;
		this.gv_ObjAtt.IsApzTable = false;
		this.gv_ObjAtt.IsInsertNewRow = false;
		this.gv_ObjAtt.IsLoadingData = false;
		this.gv_ObjAtt.IsMappingContextAction = false;
		this.gv_ObjAtt.ItemId = "OABJATT";
		this.gv_ObjAtt.LstColumnAllowBatch = null;
		this.gv_ObjAtt.LstColumnNotAllowBatch = null;
		this.gv_ObjAtt.LstColumnNotEditable = null;
		this.gv_ObjAtt.LstColumNotSetting = null;
		this.gv_ObjAtt.Name = "gv_ObjAtt";
		this.gv_ObjAtt.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ObjAtt.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ObjAtt.OptionsView.ColumnAutoWidth = false;
		this.gv_ObjAtt.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_ObjAtt.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ObjAtt.OptionsView.ShowGroupPanel = false;
		this.gv_ObjAtt.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Object Type";
		this.gridColumnEx1.ColumnEdit = this.luk_ObjType;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ObjType";
		this.gridColumnEx1.MinWidth = 180;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 193;
		this.luk_ObjType.AutoHeight = false;
		this.luk_ObjType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ObjType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ObjType.IsSearchControl = false;
		this.luk_ObjType.Name = "luk_ObjType";
		this.luk_ObjType.NullText = "";
		this.luk_ObjType.ShowFooter = false;
		this.luk_ObjType.ShowHeader = false;
		this.gridColumnEx2.Caption = "Path";
		this.gridColumnEx2.ColumnEdit = this.bte_Path;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Path";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 252;
		this.bte_Path.AutoHeight = false;
		this.bte_Path.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_Path.Name = "bte_Path";
		this.bte_Path.ReadOnly = true;
		this.bte_Path.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Path_ButtonPressed);
		this.cbo_ObjType.AutoHeight = false;
		this.cbo_ObjType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbo_ObjType.Name = "cbo_ObjType";
		this.btn_XML.EditControl = null;
		this.btn_XML.EditValue = "...";
		this.btn_XML.FieldName = null;
		this.btn_XML.IsSearchControl = false;
		this.btn_XML.Location = new System.Drawing.Point(466, 168);
		this.btn_XML.Name = "btn_XML";
		this.btn_XML.ReadOnly = false;
		this.btn_XML.SettingInVisible = false;
		this.btn_XML.SettingReadonly = false;
		this.btn_XML.Size = new System.Drawing.Size(43, 22);
		this.btn_XML.StyleController = this.layoutControl6;
		this.btn_XML.TabIndex = 17;
		this.btn_XML.Text = "...";
		this.btn_XML.Click += new System.EventHandler(this.btn_XML_Click);
		this.btn_Current.EditControl = null;
		this.btn_Current.EditValue = "...";
		this.btn_Current.FieldName = null;
		this.btn_Current.IsSearchControl = false;
		this.btn_Current.Location = new System.Drawing.Point(466, 142);
		this.btn_Current.Name = "btn_Current";
		this.btn_Current.ReadOnly = false;
		this.btn_Current.SettingInVisible = false;
		this.btn_Current.SettingReadonly = false;
		this.btn_Current.Size = new System.Drawing.Size(43, 22);
		this.btn_Current.StyleController = this.layoutControl6;
		this.btn_Current.TabIndex = 16;
		this.btn_Current.Text = "...";
		this.btn_Current.Click += new System.EventHandler(this.btn_Current_Click);
		this.btn_Extension.EditControl = null;
		this.btn_Extension.EditValue = "...";
		this.btn_Extension.FieldName = null;
		this.btn_Extension.IsSearchControl = false;
		this.btn_Extension.Location = new System.Drawing.Point(466, 116);
		this.btn_Extension.Name = "btn_Extension";
		this.btn_Extension.ReadOnly = false;
		this.btn_Extension.SettingInVisible = false;
		this.btn_Extension.SettingReadonly = false;
		this.btn_Extension.Size = new System.Drawing.Size(43, 22);
		this.btn_Extension.StyleController = this.layoutControl6;
		this.btn_Extension.TabIndex = 15;
		this.btn_Extension.Text = "...";
		this.btn_Extension.Click += new System.EventHandler(this.btn_Extension_Click);
		this.btn_Att.EditControl = null;
		this.btn_Att.EditValue = "...";
		this.btn_Att.FieldName = null;
		this.btn_Att.IsSearchControl = false;
		this.btn_Att.Location = new System.Drawing.Point(466, 90);
		this.btn_Att.Name = "btn_Att";
		this.btn_Att.ReadOnly = false;
		this.btn_Att.SettingInVisible = false;
		this.btn_Att.SettingReadonly = false;
		this.btn_Att.Size = new System.Drawing.Size(43, 22);
		this.btn_Att.StyleController = this.layoutControl6;
		this.btn_Att.TabIndex = 14;
		this.btn_Att.Text = "...";
		this.btn_Att.Click += new System.EventHandler(this.btn_Att_Click);
		this.btn_Pic.EditControl = null;
		this.btn_Pic.EditValue = "...";
		this.btn_Pic.FieldName = null;
		this.btn_Pic.IsSearchControl = false;
		this.btn_Pic.Location = new System.Drawing.Point(466, 64);
		this.btn_Pic.Name = "btn_Pic";
		this.btn_Pic.ReadOnly = false;
		this.btn_Pic.SettingInVisible = false;
		this.btn_Pic.SettingReadonly = false;
		this.btn_Pic.Size = new System.Drawing.Size(43, 22);
		this.btn_Pic.StyleController = this.layoutControl6;
		this.btn_Pic.TabIndex = 13;
		this.btn_Pic.Text = "...";
		this.btn_Pic.Click += new System.EventHandler(this.btn_Pic_Click);
		this.btn_Excel.EditControl = null;
		this.btn_Excel.EditValue = "...";
		this.btn_Excel.FieldName = null;
		this.btn_Excel.IsSearchControl = false;
		this.btn_Excel.Location = new System.Drawing.Point(466, 38);
		this.btn_Excel.Name = "btn_Excel";
		this.btn_Excel.ReadOnly = false;
		this.btn_Excel.SettingInVisible = false;
		this.btn_Excel.SettingReadonly = false;
		this.btn_Excel.Size = new System.Drawing.Size(43, 22);
		this.btn_Excel.StyleController = this.layoutControl6;
		this.btn_Excel.TabIndex = 12;
		this.btn_Excel.Text = "...";
		this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
		this.btn_MicWord.EditControl = null;
		this.btn_MicWord.EditValue = "...";
		this.btn_MicWord.FieldName = null;
		this.btn_MicWord.IsSearchControl = false;
		this.btn_MicWord.Location = new System.Drawing.Point(466, 12);
		this.btn_MicWord.Name = "btn_MicWord";
		this.btn_MicWord.ReadOnly = false;
		this.btn_MicWord.SettingInVisible = false;
		this.btn_MicWord.SettingReadonly = false;
		this.btn_MicWord.Size = new System.Drawing.Size(43, 22);
		this.btn_MicWord.StyleController = this.layoutControl6;
		this.btn_MicWord.TabIndex = 11;
		this.btn_MicWord.Text = "...";
		this.btn_MicWord.Click += new System.EventHandler(this.btn_MicWord_Click);
		this.XmlPath.EditControl = null;
		this.XmlPath.FieldName = null;
		this.XmlPath.IsSearchControl = false;
		this.XmlPath.Location = new System.Drawing.Point(173, 168);
		this.XmlPath.Name = "XmlPath";
		this.XmlPath.Properties.ReadOnly = true;
		this.XmlPath.SettingInVisible = false;
		this.XmlPath.SettingReadonly = false;
		this.XmlPath.Size = new System.Drawing.Size(289, 20);
		this.XmlPath.StyleController = this.layoutControl6;
		this.XmlPath.TabIndex = 10;
		this.XmlPath.EditValueChanged += new System.EventHandler(this.XmlPath_EditValueChanged);
		this.txt_CurrentScander.EditControl = null;
		this.txt_CurrentScander.FieldName = null;
		this.txt_CurrentScander.IsSearchControl = false;
		this.txt_CurrentScander.Location = new System.Drawing.Point(173, 142);
		this.txt_CurrentScander.Name = "txt_CurrentScander";
		this.txt_CurrentScander.Properties.ReadOnly = true;
		this.txt_CurrentScander.SettingInVisible = false;
		this.txt_CurrentScander.SettingReadonly = false;
		this.txt_CurrentScander.Size = new System.Drawing.Size(289, 20);
		this.txt_CurrentScander.StyleController = this.layoutControl6;
		this.txt_CurrentScander.TabIndex = 9;
		this.txt_CurrentScander.EditValueChanged += new System.EventHandler(this.txt_CurrentScander_EditValueChanged);
		this.ExtPath.EditControl = null;
		this.ExtPath.FieldName = null;
		this.ExtPath.IsSearchControl = false;
		this.ExtPath.Location = new System.Drawing.Point(173, 116);
		this.ExtPath.Name = "ExtPath";
		this.ExtPath.Properties.ReadOnly = true;
		this.ExtPath.SettingInVisible = false;
		this.ExtPath.SettingReadonly = false;
		this.ExtPath.Size = new System.Drawing.Size(289, 20);
		this.ExtPath.StyleController = this.layoutControl6;
		this.ExtPath.TabIndex = 8;
		this.ExtPath.EditValueChanged += new System.EventHandler(this.ExtPath_EditValueChanged);
		this.AttachPath.EditControl = null;
		this.AttachPath.FieldName = null;
		this.AttachPath.IsSearchControl = false;
		this.AttachPath.Location = new System.Drawing.Point(173, 90);
		this.AttachPath.Name = "AttachPath";
		this.AttachPath.Properties.ReadOnly = true;
		this.AttachPath.SettingInVisible = false;
		this.AttachPath.SettingReadonly = false;
		this.AttachPath.Size = new System.Drawing.Size(289, 20);
		this.AttachPath.StyleController = this.layoutControl6;
		this.AttachPath.TabIndex = 7;
		this.AttachPath.EditValueChanged += new System.EventHandler(this.AttachPath_EditValueChanged);
		this.BitmapPath.EditControl = null;
		this.BitmapPath.FieldName = null;
		this.BitmapPath.IsSearchControl = false;
		this.BitmapPath.Location = new System.Drawing.Point(173, 64);
		this.BitmapPath.Name = "BitmapPath";
		this.BitmapPath.Properties.ReadOnly = true;
		this.BitmapPath.SettingInVisible = false;
		this.BitmapPath.SettingReadonly = false;
		this.BitmapPath.Size = new System.Drawing.Size(289, 20);
		this.BitmapPath.StyleController = this.layoutControl6;
		this.BitmapPath.TabIndex = 6;
		this.BitmapPath.EditValueChanged += new System.EventHandler(this.BitmapPath_EditValueChanged);
		this.ExcelPath.EditControl = null;
		this.ExcelPath.FieldName = null;
		this.ExcelPath.IsSearchControl = false;
		this.ExcelPath.Location = new System.Drawing.Point(173, 38);
		this.ExcelPath.Name = "ExcelPath";
		this.ExcelPath.Properties.ReadOnly = true;
		this.ExcelPath.SettingInVisible = false;
		this.ExcelPath.SettingReadonly = false;
		this.ExcelPath.Size = new System.Drawing.Size(289, 20);
		this.ExcelPath.StyleController = this.layoutControl6;
		this.ExcelPath.TabIndex = 5;
		this.ExcelPath.EditValueChanged += new System.EventHandler(this.ExcelPath_EditValueChanged);
		this.WordPath.EditControl = null;
		this.WordPath.FieldName = null;
		this.WordPath.IsSearchControl = false;
		this.WordPath.Location = new System.Drawing.Point(173, 12);
		this.WordPath.Name = "WordPath";
		this.WordPath.Properties.ReadOnly = true;
		this.WordPath.SettingInVisible = false;
		this.WordPath.SettingReadonly = false;
		this.WordPath.Size = new System.Drawing.Size(289, 20);
		this.WordPath.StyleController = this.layoutControl6;
		this.WordPath.TabIndex = 4;
		this.WordPath.EditValueChanged += new System.EventHandler(this.WordPath_EditValueChanged);
		this.layoutControlGroup5.CustomizationFormText = "Root";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[18]
		{
			this.emptySpaceItem44, this.layoutControlItem84, this.layoutControlItem85, this.layoutControlItem89, this.layoutControlItem90, this.layoutControlItem93, this.layoutControlItem94, this.layoutControlItem95, this.layoutControlItem96, this.layoutControlItem83,
			this.layoutControlItem91, this.layoutControlItem92, this.layoutControlItem86, this.layoutControlItem87, this.layoutControlItem88, this.emptySpaceItem43, this.layoutControlItem52, this.layoutControlItem97
		});
		this.layoutControlGroup5.Name = "Root";
		this.layoutControlGroup5.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup5.TextVisible = false;
		this.emptySpaceItem44.AllowHotTrack = false;
		this.emptySpaceItem44.CustomizationFormText = "emptySpaceItem44";
		this.emptySpaceItem44.Location = new System.Drawing.Point(0, 399);
		this.emptySpaceItem44.Name = "emptySpaceItem44";
		this.emptySpaceItem44.Size = new System.Drawing.Size(501, 33);
		this.emptySpaceItem44.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem84.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem84.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem84.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem84.Control = this.ExcelPath;
		this.layoutControlItem84.CustomizationFormText = "layoutControlItem84";
		this.layoutControlItem84.EditControl = null;
		this.layoutControlItem84.EditValue = "Microsoft Excel Folder";
		this.layoutControlItem84.FieldName = null;
		this.layoutControlItem84.IsSearchControl = false;
		this.layoutControlItem84.Location = new System.Drawing.Point(0, 26);
		this.layoutControlItem84.Name = "layoutControlItem84";
		this.layoutControlItem84.ReadOnly = false;
		this.layoutControlItem84.SettingInVisible = false;
		this.layoutControlItem84.SettingReadonly = false;
		this.layoutControlItem84.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem84.Text = "Microsoft Excel Folder";
		this.layoutControlItem84.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem85.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem85.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem85.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem85.Control = this.BitmapPath;
		this.layoutControlItem85.CustomizationFormText = "layoutControlItem85";
		this.layoutControlItem85.EditControl = null;
		this.layoutControlItem85.EditValue = "Pictures Folder";
		this.layoutControlItem85.FieldName = null;
		this.layoutControlItem85.IsSearchControl = false;
		this.layoutControlItem85.Location = new System.Drawing.Point(0, 52);
		this.layoutControlItem85.Name = "layoutControlItem85";
		this.layoutControlItem85.ReadOnly = false;
		this.layoutControlItem85.SettingInVisible = false;
		this.layoutControlItem85.SettingReadonly = false;
		this.layoutControlItem85.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem85.Text = "Pictures Folder";
		this.layoutControlItem85.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem89.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem89.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem89.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem89.Control = this.XmlPath;
		this.layoutControlItem89.CustomizationFormText = "layoutControlItem89";
		this.layoutControlItem89.EditControl = null;
		this.layoutControlItem89.EditValue = "XML File Folder";
		this.layoutControlItem89.FieldName = null;
		this.layoutControlItem89.IsSearchControl = false;
		this.layoutControlItem89.Location = new System.Drawing.Point(0, 156);
		this.layoutControlItem89.Name = "layoutControlItem89";
		this.layoutControlItem89.ReadOnly = false;
		this.layoutControlItem89.SettingInVisible = false;
		this.layoutControlItem89.SettingReadonly = false;
		this.layoutControlItem89.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem89.Text = "XML File Folder";
		this.layoutControlItem89.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem90.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem90.Control = this.btn_MicWord;
		this.layoutControlItem90.CustomizationFormText = "layoutControlItem90";
		this.layoutControlItem90.EditControl = null;
		this.layoutControlItem90.EditValue = "layoutControlItem90";
		this.layoutControlItem90.FieldName = null;
		this.layoutControlItem90.IsSearchControl = false;
		this.layoutControlItem90.Location = new System.Drawing.Point(454, 0);
		this.layoutControlItem90.Name = "layoutControlItem90";
		this.layoutControlItem90.ReadOnly = false;
		this.layoutControlItem90.SettingInVisible = false;
		this.layoutControlItem90.SettingReadonly = false;
		this.layoutControlItem90.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem90.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem90.TextVisible = false;
		this.layoutControlItem93.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem93.Control = this.btn_Att;
		this.layoutControlItem93.CustomizationFormText = "layoutControlItem93";
		this.layoutControlItem93.EditControl = null;
		this.layoutControlItem93.EditValue = "layoutControlItem93";
		this.layoutControlItem93.FieldName = null;
		this.layoutControlItem93.IsSearchControl = false;
		this.layoutControlItem93.Location = new System.Drawing.Point(454, 78);
		this.layoutControlItem93.Name = "layoutControlItem93";
		this.layoutControlItem93.ReadOnly = false;
		this.layoutControlItem93.SettingInVisible = false;
		this.layoutControlItem93.SettingReadonly = false;
		this.layoutControlItem93.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem93.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem93.TextVisible = false;
		this.layoutControlItem94.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem94.Control = this.btn_Extension;
		this.layoutControlItem94.CustomizationFormText = "layoutControlItem94";
		this.layoutControlItem94.EditControl = null;
		this.layoutControlItem94.EditValue = "layoutControlItem94";
		this.layoutControlItem94.FieldName = null;
		this.layoutControlItem94.IsSearchControl = false;
		this.layoutControlItem94.Location = new System.Drawing.Point(454, 104);
		this.layoutControlItem94.Name = "layoutControlItem94";
		this.layoutControlItem94.ReadOnly = false;
		this.layoutControlItem94.SettingInVisible = false;
		this.layoutControlItem94.SettingReadonly = false;
		this.layoutControlItem94.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem94.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem94.TextVisible = false;
		this.layoutControlItem95.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem95.Control = this.btn_Current;
		this.layoutControlItem95.CustomizationFormText = "layoutControlItem95";
		this.layoutControlItem95.EditControl = null;
		this.layoutControlItem95.EditValue = "layoutControlItem95";
		this.layoutControlItem95.FieldName = null;
		this.layoutControlItem95.IsSearchControl = false;
		this.layoutControlItem95.Location = new System.Drawing.Point(454, 130);
		this.layoutControlItem95.Name = "layoutControlItem95";
		this.layoutControlItem95.ReadOnly = false;
		this.layoutControlItem95.SettingInVisible = false;
		this.layoutControlItem95.SettingReadonly = false;
		this.layoutControlItem95.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem95.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem95.TextVisible = false;
		this.layoutControlItem96.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem96.Control = this.btn_XML;
		this.layoutControlItem96.CustomizationFormText = "layoutControlItem96";
		this.layoutControlItem96.EditControl = null;
		this.layoutControlItem96.EditValue = "layoutControlItem96";
		this.layoutControlItem96.FieldName = null;
		this.layoutControlItem96.IsSearchControl = false;
		this.layoutControlItem96.Location = new System.Drawing.Point(454, 156);
		this.layoutControlItem96.Name = "layoutControlItem96";
		this.layoutControlItem96.ReadOnly = false;
		this.layoutControlItem96.SettingInVisible = false;
		this.layoutControlItem96.SettingReadonly = false;
		this.layoutControlItem96.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem96.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem96.TextVisible = false;
		this.layoutControlItem83.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem83.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem83.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem83.Control = this.WordPath;
		this.layoutControlItem83.CustomizationFormText = "layoutControlItem83";
		this.layoutControlItem83.EditControl = null;
		this.layoutControlItem83.EditValue = "Microsoft Word Templates Folder";
		this.layoutControlItem83.FieldName = null;
		this.layoutControlItem83.IsSearchControl = false;
		this.layoutControlItem83.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem83.Name = "layoutControlItem83";
		this.layoutControlItem83.ReadOnly = false;
		this.layoutControlItem83.SettingInVisible = false;
		this.layoutControlItem83.SettingReadonly = false;
		this.layoutControlItem83.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem83.Text = "Microsoft Word Templates Folder";
		this.layoutControlItem83.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem91.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem91.Control = this.btn_Excel;
		this.layoutControlItem91.CustomizationFormText = "layoutControlItem91";
		this.layoutControlItem91.EditControl = null;
		this.layoutControlItem91.EditValue = "layoutControlItem91";
		this.layoutControlItem91.FieldName = null;
		this.layoutControlItem91.IsSearchControl = false;
		this.layoutControlItem91.Location = new System.Drawing.Point(454, 26);
		this.layoutControlItem91.Name = "layoutControlItem91";
		this.layoutControlItem91.ReadOnly = false;
		this.layoutControlItem91.SettingInVisible = false;
		this.layoutControlItem91.SettingReadonly = false;
		this.layoutControlItem91.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem91.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem91.TextVisible = false;
		this.layoutControlItem92.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem92.Control = this.btn_Pic;
		this.layoutControlItem92.CustomizationFormText = "layoutControlItem92";
		this.layoutControlItem92.EditControl = null;
		this.layoutControlItem92.EditValue = "layoutControlItem92";
		this.layoutControlItem92.FieldName = null;
		this.layoutControlItem92.IsSearchControl = false;
		this.layoutControlItem92.Location = new System.Drawing.Point(454, 52);
		this.layoutControlItem92.Name = "layoutControlItem92";
		this.layoutControlItem92.ReadOnly = false;
		this.layoutControlItem92.SettingInVisible = false;
		this.layoutControlItem92.SettingReadonly = false;
		this.layoutControlItem92.Size = new System.Drawing.Size(47, 26);
		this.layoutControlItem92.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem92.TextVisible = false;
		this.layoutControlItem86.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem86.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem86.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem86.Control = this.AttachPath;
		this.layoutControlItem86.CustomizationFormText = "layoutControlItem86";
		this.layoutControlItem86.EditControl = null;
		this.layoutControlItem86.EditValue = "Attachments Folder";
		this.layoutControlItem86.FieldName = null;
		this.layoutControlItem86.IsSearchControl = false;
		this.layoutControlItem86.Location = new System.Drawing.Point(0, 78);
		this.layoutControlItem86.Name = "layoutControlItem86";
		this.layoutControlItem86.ReadOnly = false;
		this.layoutControlItem86.SettingInVisible = false;
		this.layoutControlItem86.SettingReadonly = false;
		this.layoutControlItem86.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem86.Text = "Attachments Folder";
		this.layoutControlItem86.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem87.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem87.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem87.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem87.Control = this.ExtPath;
		this.layoutControlItem87.CustomizationFormText = "layoutControlItem87";
		this.layoutControlItem87.EditControl = null;
		this.layoutControlItem87.EditValue = "Extensions Folder";
		this.layoutControlItem87.FieldName = null;
		this.layoutControlItem87.IsSearchControl = false;
		this.layoutControlItem87.Location = new System.Drawing.Point(0, 104);
		this.layoutControlItem87.Name = "layoutControlItem87";
		this.layoutControlItem87.ReadOnly = false;
		this.layoutControlItem87.SettingInVisible = false;
		this.layoutControlItem87.SettingReadonly = false;
		this.layoutControlItem87.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem87.Text = "Extensions Folder";
		this.layoutControlItem87.TextSize = new System.Drawing.Size(158, 13);
		this.layoutControlItem88.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem88.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem88.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem88.Control = this.txt_CurrentScander;
		this.layoutControlItem88.CustomizationFormText = "layoutControlItem88";
		this.layoutControlItem88.EditControl = null;
		this.layoutControlItem88.EditValue = "Current Scanner";
		this.layoutControlItem88.FieldName = null;
		this.layoutControlItem88.IsSearchControl = false;
		this.layoutControlItem88.Location = new System.Drawing.Point(0, 130);
		this.layoutControlItem88.Name = "layoutControlItem88";
		this.layoutControlItem88.ReadOnly = false;
		this.layoutControlItem88.SettingInVisible = false;
		this.layoutControlItem88.SettingReadonly = false;
		this.layoutControlItem88.Size = new System.Drawing.Size(454, 26);
		this.layoutControlItem88.Text = "Current Scanner";
		this.layoutControlItem88.TextSize = new System.Drawing.Size(158, 13);
		this.emptySpaceItem43.AllowHotTrack = false;
		this.emptySpaceItem43.CustomizationFormText = "emptySpaceItem43";
		this.emptySpaceItem43.Location = new System.Drawing.Point(501, 0);
		this.emptySpaceItem43.Name = "emptySpaceItem43";
		this.emptySpaceItem43.Size = new System.Drawing.Size(355, 432);
		this.emptySpaceItem43.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem52.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem52.Control = this.grd_ObjAtt;
		this.layoutControlItem52.EditControl = null;
		this.layoutControlItem52.EditValue = "layoutControlItem52";
		this.layoutControlItem52.FieldName = null;
		this.layoutControlItem52.IsSearchControl = false;
		this.layoutControlItem52.Location = new System.Drawing.Point(0, 206);
		this.layoutControlItem52.Name = "layoutControlItem52";
		this.layoutControlItem52.ReadOnly = false;
		this.layoutControlItem52.SettingInVisible = false;
		this.layoutControlItem52.SettingReadonly = false;
		this.layoutControlItem52.Size = new System.Drawing.Size(501, 193);
		this.layoutControlItem52.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem52.TextVisible = false;
		this.layoutControlItem97.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem97.Control = this.DisableSplitFolder;
		this.layoutControlItem97.EditControl = null;
		this.layoutControlItem97.EditValue = "layoutControlItem97";
		this.layoutControlItem97.FieldName = null;
		this.layoutControlItem97.IsSearchControl = false;
		this.layoutControlItem97.Location = new System.Drawing.Point(0, 182);
		this.layoutControlItem97.Name = "layoutControlItem97";
		this.layoutControlItem97.ReadOnly = false;
		this.layoutControlItem97.SettingInVisible = false;
		this.layoutControlItem97.SettingReadonly = false;
		this.layoutControlItem97.Size = new System.Drawing.Size(501, 24);
		this.layoutControlItem97.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem97.TextVisible = false;
		this.Tab_Inventory.Controls.Add(this.xtraTabControl2);
		this.Tab_Inventory.EditControl = null;
		this.Tab_Inventory.EditValue = "Inventory";
		this.Tab_Inventory.FieldName = "Tab_Inventory";
		this.Tab_Inventory.IsSearchControl = false;
		this.Tab_Inventory.Name = "Tab_Inventory";
		this.Tab_Inventory.ReadOnly = false;
		this.Tab_Inventory.SettingInVisible = false;
		this.Tab_Inventory.SettingReadonly = false;
		this.Tab_Inventory.Size = new System.Drawing.Size(876, 452);
		this.Tab_Inventory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Inventory.Text = "Inventory";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.Tab_Item;
		this.xtraTabControl2.Size = new System.Drawing.Size(876, 452);
		this.xtraTabControl2.TabIndex = 0;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.Tab_Item, this.Tab_Plan, this.Tab_Report });
		this.Tab_Item.Controls.Add(this.layoutControl7);
		this.Tab_Item.EditControl = null;
		this.Tab_Item.EditValue = "Items";
		this.Tab_Item.FieldName = "Tab_Item";
		this.Tab_Item.IsSearchControl = false;
		this.Tab_Item.Name = "Tab_Item";
		this.Tab_Item.ReadOnly = false;
		this.Tab_Item.SettingInVisible = false;
		this.Tab_Item.SettingReadonly = false;
		this.Tab_Item.Size = new System.Drawing.Size(874, 427);
		this.Tab_Item.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Item.Text = "Items";
		this.layoutControl7.Controls.Add(this.OneBOneRec);
		this.layoutControl7.Controls.Add(this.SriCreatIn);
		this.layoutControl7.Controls.Add(this.DspFrznITM);
		this.layoutControl7.Controls.Add(this.DspIITMDoc);
		this.layoutControl7.Controls.Add(this.AutoAddPkg);
		this.layoutControl7.Controls.Add(this.AutoAddUoM);
		this.layoutControl7.Controls.Add(this.WTLiable);
		this.layoutControl7.Controls.Add(this.AutoITW);
		this.layoutControl7.Controls.Add(this.AutoCrIns);
		this.layoutControl7.Controls.Add(this.SriMngSys);
		this.layoutControl7.Controls.Add(this.SriUniqFld);
		this.layoutControl7.Controls.Add(this.BtchStatus);
		this.layoutControl7.Controls.Add(this.DfltWhs);
		this.layoutControl7.Controls.Add(this.GLMethod);
		this.layoutControl7.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl7.Location = new System.Drawing.Point(0, 0);
		this.layoutControl7.Name = "layoutControl7";
		this.layoutControl7.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(570, 346, 250, 350);
		this.layoutControl7.Root = this.layoutControlGroup6;
		this.layoutControl7.Size = new System.Drawing.Size(874, 427);
		this.layoutControl7.TabIndex = 0;
		this.layoutControl7.Text = "layoutControl7";
		this.OneBOneRec.EditControl = null;
		this.OneBOneRec.FieldName = null;
		this.OneBOneRec.IsSearchControl = false;
		this.OneBOneRec.Location = new System.Drawing.Point(12, 159);
		this.OneBOneRec.Name = "OneBOneRec";
		this.OneBOneRec.Properties.Caption = "Block Multiple Receipts for Same Batch with Serial/Batch Valuation Method";
		this.OneBOneRec.Properties.DisplayValueChecked = "Y";
		this.OneBOneRec.Properties.DisplayValueUnchecked = "N";
		this.OneBOneRec.Properties.ValueChecked = "Y";
		this.OneBOneRec.Properties.ValueGrayed = "";
		this.OneBOneRec.Properties.ValueUnchecked = "N";
		this.OneBOneRec.SettingInVisible = false;
		this.OneBOneRec.SettingReadonly = false;
		this.OneBOneRec.Size = new System.Drawing.Size(454, 20);
		this.OneBOneRec.StyleController = this.layoutControl7;
		this.OneBOneRec.TabIndex = 17;
		this.OneBOneRec.CheckedChanged += new System.EventHandler(this.OneBOneRec_CheckedChanged);
		this.SriCreatIn.EditControl = null;
		this.SriCreatIn.FieldName = null;
		this.SriCreatIn.IsSearchControl = false;
		this.SriCreatIn.Location = new System.Drawing.Point(12, 53);
		this.SriCreatIn.Name = "SriCreatIn";
		this.SriCreatIn.Properties.Caption = "Automatic Serial Number Creation on Receipt";
		this.SriCreatIn.Properties.DisplayValueChecked = "Y";
		this.SriCreatIn.Properties.DisplayValueUnchecked = "N";
		this.SriCreatIn.Properties.ValueChecked = "Y";
		this.SriCreatIn.Properties.ValueGrayed = "";
		this.SriCreatIn.Properties.ValueUnchecked = "N";
		this.SriCreatIn.SettingInVisible = false;
		this.SriCreatIn.SettingReadonly = false;
		this.SriCreatIn.Size = new System.Drawing.Size(454, 20);
		this.SriCreatIn.StyleController = this.layoutControl7;
		this.SriCreatIn.TabIndex = 6;
		this.SriCreatIn.Visible = false;
		this.SriCreatIn.CheckedChanged += new System.EventHandler(this.checkEdit63_CheckedChanged_1);
		this.DspFrznITM.EditControl = null;
		this.DspFrznITM.FieldName = null;
		this.DspFrznITM.IsSearchControl = false;
		this.DspFrznITM.Location = new System.Drawing.Point(482, 29);
		this.DspFrznITM.Name = "DspFrznITM";
		this.DspFrznITM.Properties.Caption = "Reports";
		this.DspFrznITM.Properties.DisplayValueChecked = "Y";
		this.DspFrznITM.Properties.DisplayValueUnchecked = "N";
		this.DspFrznITM.Properties.ValueChecked = "Y";
		this.DspFrznITM.Properties.ValueGrayed = "";
		this.DspFrznITM.Properties.ValueUnchecked = "N";
		this.DspFrznITM.SettingInVisible = false;
		this.DspFrznITM.SettingReadonly = false;
		this.DspFrznITM.Size = new System.Drawing.Size(213, 20);
		this.DspFrznITM.StyleController = this.layoutControl7;
		this.DspFrznITM.TabIndex = 15;
		this.DspFrznITM.CheckedChanged += new System.EventHandler(this.DspFrznITM_CheckedChanged);
		this.DspIITMDoc.EditControl = null;
		this.DspIITMDoc.FieldName = null;
		this.DspIITMDoc.IsSearchControl = false;
		this.DspIITMDoc.Location = new System.Drawing.Point(482, 53);
		this.DspIITMDoc.Name = "DspIITMDoc";
		this.DspIITMDoc.Properties.Caption = "Marketing Documents";
		this.DspIITMDoc.Properties.DisplayValueChecked = "Y";
		this.DspIITMDoc.Properties.DisplayValueUnchecked = "N";
		this.DspIITMDoc.Properties.ValueChecked = "Y";
		this.DspIITMDoc.Properties.ValueGrayed = "";
		this.DspIITMDoc.Properties.ValueUnchecked = "N";
		this.DspIITMDoc.SettingInVisible = false;
		this.DspIITMDoc.SettingReadonly = false;
		this.DspIITMDoc.Size = new System.Drawing.Size(213, 20);
		this.DspIITMDoc.StyleController = this.layoutControl7;
		this.DspIITMDoc.TabIndex = 14;
		this.DspIITMDoc.CheckedChanged += new System.EventHandler(this.checkEdit43_CheckedChanged);
		this.AutoAddPkg.EditControl = null;
		this.AutoAddPkg.FieldName = null;
		this.AutoAddPkg.IsSearchControl = false;
		this.AutoAddPkg.Location = new System.Drawing.Point(12, 360);
		this.AutoAddPkg.Name = "AutoAddPkg";
		this.AutoAddPkg.Properties.Caption = "Auto. Add All Package Definitions to New and Existing Items";
		this.AutoAddPkg.Properties.DisplayValueChecked = "Y";
		this.AutoAddPkg.Properties.DisplayValueUnchecked = "N";
		this.AutoAddPkg.Properties.ValueChecked = "Y";
		this.AutoAddPkg.Properties.ValueGrayed = "";
		this.AutoAddPkg.Properties.ValueUnchecked = "N";
		this.AutoAddPkg.SettingInVisible = false;
		this.AutoAddPkg.SettingReadonly = false;
		this.AutoAddPkg.Size = new System.Drawing.Size(850, 20);
		this.AutoAddPkg.StyleController = this.layoutControl7;
		this.AutoAddPkg.TabIndex = 13;
		this.AutoAddPkg.CheckedChanged += new System.EventHandler(this.AutoAddPkg_CheckedChanged);
		this.AutoAddUoM.EditControl = null;
		this.AutoAddUoM.FieldName = null;
		this.AutoAddUoM.IsSearchControl = false;
		this.AutoAddUoM.Location = new System.Drawing.Point(12, 336);
		this.AutoAddUoM.Name = "AutoAddUoM";
		this.AutoAddUoM.Properties.Caption = "Auto. Add All UoM Group Definitions to New and E&xisting Items";
		this.AutoAddUoM.Properties.DisplayValueChecked = "Y";
		this.AutoAddUoM.Properties.DisplayValueUnchecked = "N";
		this.AutoAddUoM.Properties.ValueChecked = "Y";
		this.AutoAddUoM.Properties.ValueGrayed = "";
		this.AutoAddUoM.Properties.ValueUnchecked = "N";
		this.AutoAddUoM.SettingInVisible = false;
		this.AutoAddUoM.SettingReadonly = false;
		this.AutoAddUoM.Size = new System.Drawing.Size(850, 20);
		this.AutoAddUoM.StyleController = this.layoutControl7;
		this.AutoAddUoM.TabIndex = 12;
		this.AutoAddUoM.CheckedChanged += new System.EventHandler(this.AutoAddUoM_CheckedChanged);
		this.WTLiable.EditControl = null;
		this.WTLiable.FieldName = null;
		this.WTLiable.IsSearchControl = false;
		this.WTLiable.Location = new System.Drawing.Point(12, 312);
		this.WTLiable.Name = "WTLiable";
		this.WTLiable.Properties.Caption = "Withholding Tax";
		this.WTLiable.Properties.DisplayValueChecked = "Y";
		this.WTLiable.Properties.DisplayValueUnchecked = "N";
		this.WTLiable.Properties.ValueChecked = "Y";
		this.WTLiable.Properties.ValueGrayed = "";
		this.WTLiable.Properties.ValueUnchecked = "N";
		this.WTLiable.SettingInVisible = false;
		this.WTLiable.SettingReadonly = false;
		this.WTLiable.Size = new System.Drawing.Size(850, 20);
		this.WTLiable.StyleController = this.layoutControl7;
		this.WTLiable.TabIndex = 11;
		this.WTLiable.Visible = false;
		this.WTLiable.CheckedChanged += new System.EventHandler(this.WTLiable_CheckedChanged);
		this.AutoITW.EditControl = null;
		this.AutoITW.FieldName = null;
		this.AutoITW.IsSearchControl = false;
		this.AutoITW.Location = new System.Drawing.Point(12, 231);
		this.AutoITW.Name = "AutoITW";
		this.AutoITW.Properties.Caption = "Auto. Add All Warehouses to New and Existing Items";
		this.AutoITW.Properties.DisplayValueChecked = "Y";
		this.AutoITW.Properties.DisplayValueUnchecked = "N";
		this.AutoITW.Properties.ValueChecked = "Y";
		this.AutoITW.Properties.ValueGrayed = "";
		this.AutoITW.Properties.ValueUnchecked = "N";
		this.AutoITW.SettingInVisible = false;
		this.AutoITW.SettingReadonly = false;
		this.AutoITW.Size = new System.Drawing.Size(454, 20);
		this.AutoITW.StyleController = this.layoutControl7;
		this.AutoITW.TabIndex = 9;
		this.AutoITW.CheckedChanged += new System.EventHandler(this.AutoITW_CheckedChanged);
		this.AutoCrIns.EditControl = null;
		this.AutoCrIns.FieldName = null;
		this.AutoCrIns.IsSearchControl = false;
		this.AutoCrIns.Location = new System.Drawing.Point(12, 111);
		this.AutoCrIns.Name = "AutoCrIns";
		this.AutoCrIns.Properties.Caption = "Auto. Create Customer E&quipment Card";
		this.AutoCrIns.Properties.DisplayValueChecked = "Y";
		this.AutoCrIns.Properties.DisplayValueUnchecked = "N";
		this.AutoCrIns.Properties.ValueChecked = "Y";
		this.AutoCrIns.Properties.ValueGrayed = "";
		this.AutoCrIns.Properties.ValueUnchecked = "N";
		this.AutoCrIns.SettingInVisible = false;
		this.AutoCrIns.SettingReadonly = false;
		this.AutoCrIns.Size = new System.Drawing.Size(454, 20);
		this.AutoCrIns.StyleController = this.layoutControl7;
		this.AutoCrIns.TabIndex = 6;
		this.AutoCrIns.CheckedChanged += new System.EventHandler(this.checkEdit37_CheckedChanged);
		this.SriMngSys.EditControl = null;
		this.SriMngSys.FieldName = null;
		this.SriMngSys.IsSearchControl = false;
		this.SriMngSys.Location = new System.Drawing.Point(157, 29);
		this.SriMngSys.Name = "SriMngSys";
		this.SriMngSys.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SriMngSys.Properties.IsSearchControl = false;
		this.SriMngSys.Properties.NullText = "";
		this.SriMngSys.SettingInVisible = false;
		this.SriMngSys.SettingReadonly = false;
		this.SriMngSys.Size = new System.Drawing.Size(309, 20);
		this.SriMngSys.StyleController = this.layoutControl7;
		this.SriMngSys.TabIndex = 4;
		this.SriMngSys.EditValueChanged += new System.EventHandler(this.SriMngSys_EditValueChanged);
		this.SriUniqFld.EditControl = null;
		this.SriUniqFld.FieldName = null;
		this.SriUniqFld.IsSearchControl = false;
		this.SriUniqFld.Location = new System.Drawing.Point(157, 87);
		this.SriUniqFld.Name = "SriUniqFld";
		this.SriUniqFld.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SriUniqFld.Properties.IsSearchControl = false;
		this.SriUniqFld.Properties.NullText = "";
		this.SriUniqFld.SettingInVisible = false;
		this.SriUniqFld.SettingReadonly = false;
		this.SriUniqFld.Size = new System.Drawing.Size(309, 20);
		this.SriUniqFld.StyleController = this.layoutControl7;
		this.SriUniqFld.TabIndex = 5;
		this.SriUniqFld.EditValueChanged += new System.EventHandler(this.SriUniqFld_EditValueChanged);
		this.BtchStatus.EditControl = null;
		this.BtchStatus.FieldName = null;
		this.BtchStatus.IsSearchControl = false;
		this.BtchStatus.Location = new System.Drawing.Point(157, 135);
		this.BtchStatus.Name = "BtchStatus";
		this.BtchStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.BtchStatus.Properties.IsSearchControl = false;
		this.BtchStatus.Properties.NullText = "";
		this.BtchStatus.SettingInVisible = false;
		this.BtchStatus.SettingReadonly = false;
		this.BtchStatus.Size = new System.Drawing.Size(309, 20);
		this.BtchStatus.StyleController = this.layoutControl7;
		this.BtchStatus.TabIndex = 7;
		this.BtchStatus.EditValueChanged += new System.EventHandler(this.BtchStatus_EditValueChanged);
		this.DfltWhs.EditControl = null;
		this.DfltWhs.FieldName = null;
		this.DfltWhs.IsSearchControl = false;
		this.DfltWhs.Location = new System.Drawing.Point(157, 183);
		this.DfltWhs.Name = "DfltWhs";
		this.DfltWhs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfltWhs.Properties.IsSearchControl = false;
		this.DfltWhs.Properties.NullText = "";
		this.DfltWhs.SettingInVisible = false;
		this.DfltWhs.SettingReadonly = false;
		this.DfltWhs.Size = new System.Drawing.Size(309, 20);
		this.DfltWhs.StyleController = this.layoutControl7;
		this.DfltWhs.TabIndex = 8;
		this.DfltWhs.EditValueChanged += new System.EventHandler(this.DfltWhs_EditValueChanged);
		this.GLMethod.EditControl = null;
		this.GLMethod.FieldName = null;
		this.GLMethod.IsSearchControl = false;
		this.GLMethod.Location = new System.Drawing.Point(157, 207);
		this.GLMethod.Name = "GLMethod";
		this.GLMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.GLMethod.Properties.IsSearchControl = false;
		this.GLMethod.Properties.NullText = "";
		this.GLMethod.SettingInVisible = false;
		this.GLMethod.SettingReadonly = false;
		this.GLMethod.Size = new System.Drawing.Size(309, 20);
		this.GLMethod.StyleController = this.layoutControl7;
		this.GLMethod.TabIndex = 18;
		this.GLMethod.EditValueChanged += new System.EventHandler(this.textEdit53_EditValueChanged);
		this.layoutControlGroup6.CustomizationFormText = "Root";
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[23]
		{
			this.emptySpaceItem48, this.emptySpaceItem50, this.emptySpaceItem51, this.simpleLabelItem13, this.layoutControlItem99, this.layoutControlItem100, this.layoutControlItem101, this.layoutControlItem102, this.layoutControlItem103, this.emptySpaceItem53,
			this.layoutControlItem105, this.layoutControlItem106, this.emptySpaceItem54, this.layoutControlItem107, this.simpleLabelItem14, this.emptySpaceItem55, this.layoutControlItem108, this.layoutControlItem109, this.layoutControlItem98, this.layoutControlItem142,
			this.emptySpaceItem36, this.layoutControlItem143, this.layoutControlItem144
		});
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Size = new System.Drawing.Size(874, 427);
		this.layoutControlGroup6.TextVisible = false;
		this.emptySpaceItem48.AllowHotTrack = false;
		this.emptySpaceItem48.CustomizationFormText = "emptySpaceItem48";
		this.emptySpaceItem48.Location = new System.Drawing.Point(0, 372);
		this.emptySpaceItem48.Name = "emptySpaceItem48";
		this.emptySpaceItem48.Size = new System.Drawing.Size(854, 35);
		this.emptySpaceItem48.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem50.AllowHotTrack = false;
		this.emptySpaceItem50.CustomizationFormText = "emptySpaceItem50";
		this.emptySpaceItem50.Location = new System.Drawing.Point(0, 253);
		this.emptySpaceItem50.Name = "emptySpaceItem50";
		this.emptySpaceItem50.Size = new System.Drawing.Size(458, 10);
		this.emptySpaceItem50.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem51.AllowHotTrack = false;
		this.emptySpaceItem51.CustomizationFormText = "emptySpaceItem51";
		this.emptySpaceItem51.Location = new System.Drawing.Point(687, 0);
		this.emptySpaceItem51.Name = "emptySpaceItem51";
		this.emptySpaceItem51.Size = new System.Drawing.Size(167, 263);
		this.emptySpaceItem51.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem13.AllowHotTrack = false;
		this.simpleLabelItem13.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem13.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem13.CustomizationFormText = "LabelsimpleLabelItem13";
		this.simpleLabelItem13.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem13.Name = "simpleLabelItem13";
		this.simpleLabelItem13.Size = new System.Drawing.Size(458, 17);
		this.simpleLabelItem13.Text = "Serial Numbers and Batches";
		this.simpleLabelItem13.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem99.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem99.Control = this.SriUniqFld;
		this.layoutControlItem99.CustomizationFormText = "layoutControlItem99";
		this.layoutControlItem99.EditControl = null;
		this.layoutControlItem99.EditValue = "Unique Serial Numbers by";
		this.layoutControlItem99.FieldName = null;
		this.layoutControlItem99.IsSearchControl = false;
		this.layoutControlItem99.Location = new System.Drawing.Point(0, 75);
		this.layoutControlItem99.Name = "layoutControlItem99";
		this.layoutControlItem99.ReadOnly = false;
		this.layoutControlItem99.SettingInVisible = false;
		this.layoutControlItem99.SettingReadonly = false;
		this.layoutControlItem99.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem99.Text = "Unique Serial Numbers by";
		this.layoutControlItem99.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem100.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem100.Control = this.AutoCrIns;
		this.layoutControlItem100.CustomizationFormText = "layoutControlItem100";
		this.layoutControlItem100.EditControl = null;
		this.layoutControlItem100.EditValue = "layoutControlItem100";
		this.layoutControlItem100.FieldName = null;
		this.layoutControlItem100.IsSearchControl = false;
		this.layoutControlItem100.Location = new System.Drawing.Point(0, 99);
		this.layoutControlItem100.Name = "layoutControlItem100";
		this.layoutControlItem100.ReadOnly = false;
		this.layoutControlItem100.SettingInVisible = false;
		this.layoutControlItem100.SettingReadonly = false;
		this.layoutControlItem100.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem100.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem100.TextVisible = false;
		this.layoutControlItem101.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem101.Control = this.BtchStatus;
		this.layoutControlItem101.CustomizationFormText = "layoutControlItem101";
		this.layoutControlItem101.EditControl = null;
		this.layoutControlItem101.EditValue = "Basic Setting for Batch Status";
		this.layoutControlItem101.FieldName = null;
		this.layoutControlItem101.IsSearchControl = false;
		this.layoutControlItem101.Location = new System.Drawing.Point(0, 123);
		this.layoutControlItem101.Name = "layoutControlItem101";
		this.layoutControlItem101.ReadOnly = false;
		this.layoutControlItem101.SettingInVisible = false;
		this.layoutControlItem101.SettingReadonly = false;
		this.layoutControlItem101.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem101.Text = "Basic Setting for Batch Status";
		this.layoutControlItem101.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem102.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem102.Control = this.DfltWhs;
		this.layoutControlItem102.CustomizationFormText = "layoutControlItem102";
		this.layoutControlItem102.EditControl = null;
		this.layoutControlItem102.EditValue = "Default Warehouse";
		this.layoutControlItem102.FieldName = null;
		this.layoutControlItem102.IsSearchControl = false;
		this.layoutControlItem102.Location = new System.Drawing.Point(0, 171);
		this.layoutControlItem102.Name = "layoutControlItem102";
		this.layoutControlItem102.ReadOnly = false;
		this.layoutControlItem102.SettingInVisible = false;
		this.layoutControlItem102.SettingReadonly = false;
		this.layoutControlItem102.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem102.Text = "Default Warehouse";
		this.layoutControlItem102.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem103.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem103.Control = this.AutoITW;
		this.layoutControlItem103.CustomizationFormText = "layoutControlItem103";
		this.layoutControlItem103.EditControl = null;
		this.layoutControlItem103.EditValue = "layoutControlItem103";
		this.layoutControlItem103.FieldName = null;
		this.layoutControlItem103.IsSearchControl = false;
		this.layoutControlItem103.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem103.Name = "layoutControlItem103";
		this.layoutControlItem103.ReadOnly = false;
		this.layoutControlItem103.SettingInVisible = false;
		this.layoutControlItem103.SettingReadonly = false;
		this.layoutControlItem103.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem103.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem103.TextVisible = false;
		this.emptySpaceItem53.AllowHotTrack = false;
		this.emptySpaceItem53.CustomizationFormText = "emptySpaceItem53";
		this.emptySpaceItem53.Location = new System.Drawing.Point(0, 243);
		this.emptySpaceItem53.Name = "emptySpaceItem53";
		this.emptySpaceItem53.Size = new System.Drawing.Size(458, 10);
		this.emptySpaceItem53.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem105.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem105.Control = this.WTLiable;
		this.layoutControlItem105.CustomizationFormText = "layoutControlItem105";
		this.layoutControlItem105.EditControl = null;
		this.layoutControlItem105.EditValue = "layoutControlItem105";
		this.layoutControlItem105.FieldName = null;
		this.layoutControlItem105.IsSearchControl = false;
		this.layoutControlItem105.Location = new System.Drawing.Point(0, 300);
		this.layoutControlItem105.Name = "layoutControlItem105";
		this.layoutControlItem105.ReadOnly = false;
		this.layoutControlItem105.SettingInVisible = false;
		this.layoutControlItem105.SettingReadonly = false;
		this.layoutControlItem105.Size = new System.Drawing.Size(854, 24);
		this.layoutControlItem105.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem105.TextVisible = false;
		this.layoutControlItem105.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem106.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem106.Control = this.AutoAddUoM;
		this.layoutControlItem106.CustomizationFormText = "layoutControlItem106";
		this.layoutControlItem106.EditControl = null;
		this.layoutControlItem106.EditValue = "layoutControlItem106";
		this.layoutControlItem106.FieldName = null;
		this.layoutControlItem106.IsSearchControl = false;
		this.layoutControlItem106.Location = new System.Drawing.Point(0, 324);
		this.layoutControlItem106.Name = "layoutControlItem106";
		this.layoutControlItem106.ReadOnly = false;
		this.layoutControlItem106.SettingInVisible = false;
		this.layoutControlItem106.SettingReadonly = false;
		this.layoutControlItem106.Size = new System.Drawing.Size(854, 24);
		this.layoutControlItem106.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem106.TextVisible = false;
		this.emptySpaceItem54.AllowHotTrack = false;
		this.emptySpaceItem54.CustomizationFormText = "emptySpaceItem54";
		this.emptySpaceItem54.Location = new System.Drawing.Point(0, 263);
		this.emptySpaceItem54.Name = "emptySpaceItem54";
		this.emptySpaceItem54.Size = new System.Drawing.Size(854, 37);
		this.emptySpaceItem54.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem107.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem107.Control = this.AutoAddPkg;
		this.layoutControlItem107.CustomizationFormText = "layoutControlItem107";
		this.layoutControlItem107.EditControl = null;
		this.layoutControlItem107.EditValue = "layoutControlItem107";
		this.layoutControlItem107.FieldName = null;
		this.layoutControlItem107.IsSearchControl = false;
		this.layoutControlItem107.Location = new System.Drawing.Point(0, 348);
		this.layoutControlItem107.Name = "layoutControlItem107";
		this.layoutControlItem107.ReadOnly = false;
		this.layoutControlItem107.SettingInVisible = false;
		this.layoutControlItem107.SettingReadonly = false;
		this.layoutControlItem107.Size = new System.Drawing.Size(854, 24);
		this.layoutControlItem107.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem107.TextVisible = false;
		this.simpleLabelItem14.AllowHotTrack = false;
		this.simpleLabelItem14.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem14.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem14.CustomizationFormText = "LabelsimpleLabelItem14";
		this.simpleLabelItem14.Location = new System.Drawing.Point(470, 0);
		this.simpleLabelItem14.Name = "simpleLabelItem14";
		this.simpleLabelItem14.Size = new System.Drawing.Size(217, 17);
		this.simpleLabelItem14.Text = "Display Inactive Items In";
		this.simpleLabelItem14.TextSize = new System.Drawing.Size(142, 13);
		this.emptySpaceItem55.AllowHotTrack = false;
		this.emptySpaceItem55.CustomizationFormText = "emptySpaceItem55";
		this.emptySpaceItem55.Location = new System.Drawing.Point(458, 0);
		this.emptySpaceItem55.Name = "emptySpaceItem55";
		this.emptySpaceItem55.Size = new System.Drawing.Size(12, 263);
		this.emptySpaceItem55.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem108.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem108.Control = this.DspIITMDoc;
		this.layoutControlItem108.CustomizationFormText = "layoutControlItem108";
		this.layoutControlItem108.EditControl = null;
		this.layoutControlItem108.EditValue = "layoutControlItem108";
		this.layoutControlItem108.FieldName = null;
		this.layoutControlItem108.IsSearchControl = false;
		this.layoutControlItem108.Location = new System.Drawing.Point(470, 41);
		this.layoutControlItem108.Name = "layoutControlItem108";
		this.layoutControlItem108.ReadOnly = false;
		this.layoutControlItem108.SettingInVisible = false;
		this.layoutControlItem108.SettingReadonly = false;
		this.layoutControlItem108.Size = new System.Drawing.Size(217, 222);
		this.layoutControlItem108.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem108.TextVisible = false;
		this.layoutControlItem109.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem109.Control = this.DspFrznITM;
		this.layoutControlItem109.CustomizationFormText = "layoutControlItem109";
		this.layoutControlItem109.EditControl = null;
		this.layoutControlItem109.EditValue = "layoutControlItem109";
		this.layoutControlItem109.FieldName = null;
		this.layoutControlItem109.IsSearchControl = false;
		this.layoutControlItem109.Location = new System.Drawing.Point(470, 17);
		this.layoutControlItem109.Name = "layoutControlItem109";
		this.layoutControlItem109.ReadOnly = false;
		this.layoutControlItem109.SettingInVisible = false;
		this.layoutControlItem109.SettingReadonly = false;
		this.layoutControlItem109.Size = new System.Drawing.Size(217, 24);
		this.layoutControlItem109.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem109.TextVisible = false;
		this.layoutControlItem98.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem98.Control = this.SriMngSys;
		this.layoutControlItem98.CustomizationFormText = "layoutControlItem98";
		this.layoutControlItem98.EditControl = null;
		this.layoutControlItem98.EditValue = "Management Method";
		this.layoutControlItem98.FieldName = null;
		this.layoutControlItem98.IsSearchControl = false;
		this.layoutControlItem98.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem98.Name = "layoutControlItem98";
		this.layoutControlItem98.ReadOnly = false;
		this.layoutControlItem98.SettingInVisible = false;
		this.layoutControlItem98.SettingReadonly = false;
		this.layoutControlItem98.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem98.Text = "Management Method";
		this.layoutControlItem98.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem142.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem142.Control = this.SriCreatIn;
		this.layoutControlItem142.CustomizationFormText = "layoutControlItem142";
		this.layoutControlItem142.EditControl = null;
		this.layoutControlItem142.EditValue = "layoutControlItem142";
		this.layoutControlItem142.FieldName = null;
		this.layoutControlItem142.IsSearchControl = false;
		this.layoutControlItem142.Location = new System.Drawing.Point(0, 41);
		this.layoutControlItem142.Name = "layoutControlItem142";
		this.layoutControlItem142.ReadOnly = false;
		this.layoutControlItem142.SettingInVisible = false;
		this.layoutControlItem142.SettingReadonly = false;
		this.layoutControlItem142.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem142.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem142.TextVisible = false;
		this.emptySpaceItem36.AllowHotTrack = false;
		this.emptySpaceItem36.CustomizationFormText = "emptySpaceItem36";
		this.emptySpaceItem36.Location = new System.Drawing.Point(0, 65);
		this.emptySpaceItem36.Name = "emptySpaceItem36";
		this.emptySpaceItem36.Size = new System.Drawing.Size(458, 10);
		this.emptySpaceItem36.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem143.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem143.Control = this.OneBOneRec;
		this.layoutControlItem143.CustomizationFormText = "layoutControlItem143";
		this.layoutControlItem143.EditControl = null;
		this.layoutControlItem143.EditValue = "layoutControlItem143";
		this.layoutControlItem143.FieldName = null;
		this.layoutControlItem143.IsSearchControl = false;
		this.layoutControlItem143.Location = new System.Drawing.Point(0, 147);
		this.layoutControlItem143.Name = "layoutControlItem143";
		this.layoutControlItem143.ReadOnly = false;
		this.layoutControlItem143.SettingInVisible = false;
		this.layoutControlItem143.SettingReadonly = false;
		this.layoutControlItem143.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem143.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem143.TextVisible = false;
		this.layoutControlItem144.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem144.Control = this.GLMethod;
		this.layoutControlItem144.CustomizationFormText = "Set G/L Accounts By";
		this.layoutControlItem144.EditControl = null;
		this.layoutControlItem144.EditValue = "Set G/L Accounts By";
		this.layoutControlItem144.FieldName = null;
		this.layoutControlItem144.IsSearchControl = false;
		this.layoutControlItem144.Location = new System.Drawing.Point(0, 195);
		this.layoutControlItem144.Name = "layoutControlItem144";
		this.layoutControlItem144.ReadOnly = false;
		this.layoutControlItem144.SettingInVisible = false;
		this.layoutControlItem144.SettingReadonly = false;
		this.layoutControlItem144.Size = new System.Drawing.Size(458, 24);
		this.layoutControlItem144.Text = "Set G/L Accounts By";
		this.layoutControlItem144.TextSize = new System.Drawing.Size(142, 13);
		this.layoutControlItem144.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.Tab_Plan.Controls.Add(this.layoutControl14);
		this.Tab_Plan.EditControl = null;
		this.Tab_Plan.EditValue = "P&lanning";
		this.Tab_Plan.FieldName = "Tab_Plan";
		this.Tab_Plan.IsSearchControl = false;
		this.Tab_Plan.Name = "Tab_Plan";
		this.Tab_Plan.ReadOnly = false;
		this.Tab_Plan.SettingInVisible = false;
		this.Tab_Plan.SettingReadonly = false;
		this.Tab_Plan.Size = new System.Drawing.Size(874, 427);
		this.Tab_Plan.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Plan.Text = "P&lanning";
		this.layoutControl14.Controls.Add(this.DaysBack);
		this.layoutControl14.Controls.Add(this.DaysFwrd);
		this.layoutControl14.Controls.Add(this.ConsumeFCT);
		this.layoutControl14.Controls.Add(this.ConsumeMtd);
		this.layoutControl14.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl14.Location = new System.Drawing.Point(0, 0);
		this.layoutControl14.Name = "layoutControl14";
		this.layoutControl14.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(490, 352, 250, 350);
		this.layoutControl14.Root = this.layoutControlGroup13;
		this.layoutControl14.Size = new System.Drawing.Size(874, 427);
		this.layoutControl14.TabIndex = 0;
		this.layoutControl14.Text = "layoutControl14";
		this.DaysBack.EditControl = null;
		this.DaysBack.FieldName = null;
		this.DaysBack.IsSearchControl = false;
		this.DaysBack.Location = new System.Drawing.Point(116, 78);
		this.DaysBack.Name = "DaysBack";
		this.DaysBack.SettingInVisible = false;
		this.DaysBack.SettingReadonly = false;
		this.DaysBack.Size = new System.Drawing.Size(198, 20);
		this.DaysBack.StyleController = this.layoutControl14;
		this.DaysBack.TabIndex = 7;
		this.DaysBack.EditValueChanged += new System.EventHandler(this.DaysBack_EditValueChanged);
		this.DaysFwrd.EditControl = null;
		this.DaysFwrd.FieldName = null;
		this.DaysFwrd.IsSearchControl = false;
		this.DaysFwrd.Location = new System.Drawing.Point(116, 102);
		this.DaysFwrd.Name = "DaysFwrd";
		this.DaysFwrd.SettingInVisible = false;
		this.DaysFwrd.SettingReadonly = false;
		this.DaysFwrd.Size = new System.Drawing.Size(198, 20);
		this.DaysFwrd.StyleController = this.layoutControl14;
		this.DaysFwrd.TabIndex = 5;
		this.DaysFwrd.EditValueChanged += new System.EventHandler(this.DaysFwrd_EditValueChanged);
		this.ConsumeFCT.EditControl = null;
		this.ConsumeFCT.EditValue = true;
		this.ConsumeFCT.FieldName = null;
		this.ConsumeFCT.IsSearchControl = false;
		this.ConsumeFCT.Location = new System.Drawing.Point(12, 12);
		this.ConsumeFCT.Name = "ConsumeFCT";
		this.ConsumeFCT.Properties.Caption = "Consume Forecast";
		this.ConsumeFCT.Properties.ValueChecked = "Y";
		this.ConsumeFCT.Properties.ValueUnchecked = "N";
		this.ConsumeFCT.SettingInVisible = false;
		this.ConsumeFCT.SettingReadonly = false;
		this.ConsumeFCT.Size = new System.Drawing.Size(302, 20);
		this.ConsumeFCT.StyleController = this.layoutControl14;
		this.ConsumeFCT.TabIndex = 4;
		this.ConsumeFCT.CheckedChanged += new System.EventHandler(this.ConsumeFCT_CheckedChanged);
		this.ConsumeMtd.EditControl = null;
		this.ConsumeMtd.FieldName = null;
		this.ConsumeMtd.IsSearchControl = false;
		this.ConsumeMtd.Location = new System.Drawing.Point(116, 54);
		this.ConsumeMtd.Name = "ConsumeMtd";
		this.ConsumeMtd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ConsumeMtd.Properties.IsSearchControl = false;
		this.ConsumeMtd.Properties.NullText = "";
		this.ConsumeMtd.SettingInVisible = false;
		this.ConsumeMtd.SettingReadonly = false;
		this.ConsumeMtd.Size = new System.Drawing.Size(198, 20);
		this.ConsumeMtd.StyleController = this.layoutControl14;
		this.ConsumeMtd.TabIndex = 6;
		this.ConsumeMtd.EditValueChanged += new System.EventHandler(this.ConsumeMtd_EditValueChanged);
		this.layoutControlGroup13.CustomizationFormText = "Root";
		this.layoutControlGroup13.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup13.GroupBordersVisible = false;
		this.layoutControlGroup13.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.emptySpaceItem39, this.layoutControlItem104, this.layoutControlItem145, this.layoutControlItem146, this.emptySpaceItem52, this.layoutControlItem141, this.emptySpaceItem79, this.emptySpaceItem82 });
		this.layoutControlGroup13.Name = "Root";
		this.layoutControlGroup13.Size = new System.Drawing.Size(874, 427);
		this.layoutControlGroup13.TextVisible = false;
		this.emptySpaceItem39.AllowHotTrack = false;
		this.emptySpaceItem39.CustomizationFormText = "emptySpaceItem39";
		this.emptySpaceItem39.Location = new System.Drawing.Point(0, 114);
		this.emptySpaceItem39.Name = "emptySpaceItem39";
		this.emptySpaceItem39.Size = new System.Drawing.Size(306, 246);
		this.emptySpaceItem39.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem104.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem104.Control = this.ConsumeFCT;
		this.layoutControlItem104.CustomizationFormText = "layoutControlItem104";
		this.layoutControlItem104.EditControl = null;
		this.layoutControlItem104.EditValue = "layoutControlItem104";
		this.layoutControlItem104.FieldName = null;
		this.layoutControlItem104.IsSearchControl = false;
		this.layoutControlItem104.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem104.Name = "layoutControlItem104";
		this.layoutControlItem104.ReadOnly = false;
		this.layoutControlItem104.SettingInVisible = false;
		this.layoutControlItem104.SettingReadonly = false;
		this.layoutControlItem104.Size = new System.Drawing.Size(306, 24);
		this.layoutControlItem104.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem104.TextVisible = false;
		this.layoutControlItem145.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem145.Control = this.ConsumeMtd;
		this.layoutControlItem145.CustomizationFormText = "Days Backward";
		this.layoutControlItem145.EditControl = null;
		this.layoutControlItem145.EditValue = "Days Backward";
		this.layoutControlItem145.FieldName = null;
		this.layoutControlItem145.IsSearchControl = false;
		this.layoutControlItem145.Location = new System.Drawing.Point(0, 42);
		this.layoutControlItem145.Name = "layoutControlItem145";
		this.layoutControlItem145.ReadOnly = false;
		this.layoutControlItem145.SettingInVisible = false;
		this.layoutControlItem145.SettingReadonly = false;
		this.layoutControlItem145.Size = new System.Drawing.Size(306, 24);
		this.layoutControlItem145.Text = "Days Backward";
		this.layoutControlItem145.TextSize = new System.Drawing.Size(101, 13);
		this.layoutControlItem146.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem146.Control = this.DaysBack;
		this.layoutControlItem146.CustomizationFormText = "Days Forward";
		this.layoutControlItem146.EditControl = null;
		this.layoutControlItem146.EditValue = "Days Forward";
		this.layoutControlItem146.FieldName = null;
		this.layoutControlItem146.IsSearchControl = false;
		this.layoutControlItem146.Location = new System.Drawing.Point(0, 66);
		this.layoutControlItem146.Name = "layoutControlItem146";
		this.layoutControlItem146.ReadOnly = false;
		this.layoutControlItem146.SettingInVisible = false;
		this.layoutControlItem146.SettingReadonly = false;
		this.layoutControlItem146.Size = new System.Drawing.Size(306, 24);
		this.layoutControlItem146.Text = "Days Forward";
		this.layoutControlItem146.TextSize = new System.Drawing.Size(101, 13);
		this.emptySpaceItem52.AllowHotTrack = false;
		this.emptySpaceItem52.CustomizationFormText = "emptySpaceItem52";
		this.emptySpaceItem52.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem52.Name = "emptySpaceItem52";
		this.emptySpaceItem52.Size = new System.Drawing.Size(306, 18);
		this.emptySpaceItem52.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem141.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem141.Control = this.DaysFwrd;
		this.layoutControlItem141.CustomizationFormText = "Consumption Method";
		this.layoutControlItem141.EditControl = null;
		this.layoutControlItem141.EditValue = "Consumption Method";
		this.layoutControlItem141.FieldName = null;
		this.layoutControlItem141.IsSearchControl = false;
		this.layoutControlItem141.Location = new System.Drawing.Point(0, 90);
		this.layoutControlItem141.Name = "layoutControlItem141";
		this.layoutControlItem141.ReadOnly = false;
		this.layoutControlItem141.SettingInVisible = false;
		this.layoutControlItem141.SettingReadonly = false;
		this.layoutControlItem141.Size = new System.Drawing.Size(306, 24);
		this.layoutControlItem141.Text = "Consumption Method";
		this.layoutControlItem141.TextSize = new System.Drawing.Size(101, 13);
		this.emptySpaceItem79.AllowHotTrack = false;
		this.emptySpaceItem79.CustomizationFormText = "emptySpaceItem79";
		this.emptySpaceItem79.Location = new System.Drawing.Point(0, 360);
		this.emptySpaceItem79.Name = "emptySpaceItem79";
		this.emptySpaceItem79.Size = new System.Drawing.Size(306, 47);
		this.emptySpaceItem79.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem82.AllowHotTrack = false;
		this.emptySpaceItem82.CustomizationFormText = "emptySpaceItem82";
		this.emptySpaceItem82.Location = new System.Drawing.Point(306, 0);
		this.emptySpaceItem82.Name = "emptySpaceItem82";
		this.emptySpaceItem82.Size = new System.Drawing.Size(548, 407);
		this.emptySpaceItem82.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_Report.Controls.Add(this.layoutControl15);
		this.Tab_Report.EditControl = null;
		this.Tab_Report.EditValue = "Reporting";
		this.Tab_Report.FieldName = "Tab_Report";
		this.Tab_Report.IsSearchControl = false;
		this.Tab_Report.Name = "Tab_Report";
		this.Tab_Report.ReadOnly = false;
		this.Tab_Report.SettingInVisible = false;
		this.Tab_Report.SettingReadonly = false;
		this.Tab_Report.Size = new System.Drawing.Size(874, 427);
		this.Tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Report.Text = "Reporting";
		this.layoutControl15.Controls.Add(this.SimReport);
		this.layoutControl15.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl15.Location = new System.Drawing.Point(0, 0);
		this.layoutControl15.Name = "layoutControl15";
		this.layoutControl15.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(759, 249, 250, 350);
		this.layoutControl15.Root = this.layoutControlGroup14;
		this.layoutControl15.Size = new System.Drawing.Size(874, 427);
		this.layoutControl15.TabIndex = 3;
		this.layoutControl15.Text = "layoutControl15";
		this.SimReport.EditControl = null;
		this.SimReport.FieldName = null;
		this.SimReport.IsSearchControl = false;
		this.SimReport.Location = new System.Drawing.Point(12, 39);
		this.SimReport.Name = "SimReport";
		this.SimReport.Properties.Columns = 1;
		this.SimReport.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Classic Valuation Report, E&xcluding Item Master Valuation"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Y", "Enhanced Valuation Report, Including All Valuation Methods")
		});
		this.SimReport.SettingInVisible = false;
		this.SimReport.SettingReadonly = false;
		this.SimReport.Size = new System.Drawing.Size(850, 59);
		this.SimReport.StyleController = this.layoutControl15;
		this.SimReport.TabIndex = 4;
		this.SimReport.SelectedIndexChanged += new System.EventHandler(this.SimReport_SelectedIndexChanged);
		this.layoutControlGroup14.CustomizationFormText = "Root";
		this.layoutControlGroup14.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup14.GroupBordersVisible = false;
		this.layoutControlGroup14.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.emptySpaceItem49, this.simpleLabelItem24, this.layoutControlItem147, this.emptySpaceItem83 });
		this.layoutControlGroup14.Name = "Root";
		this.layoutControlGroup14.Size = new System.Drawing.Size(874, 427);
		this.layoutControlGroup14.TextVisible = false;
		this.emptySpaceItem49.AllowHotTrack = false;
		this.emptySpaceItem49.CustomizationFormText = "emptySpaceItem49";
		this.emptySpaceItem49.Location = new System.Drawing.Point(0, 90);
		this.emptySpaceItem49.Name = "emptySpaceItem49";
		this.emptySpaceItem49.Size = new System.Drawing.Size(854, 317);
		this.emptySpaceItem49.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem24.AllowHotTrack = false;
		this.simpleLabelItem24.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem24.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem24.CustomizationFormText = "Inventory Valuation Simulation Report";
		this.simpleLabelItem24.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem24.Name = "simpleLabelItem24";
		this.simpleLabelItem24.Size = new System.Drawing.Size(854, 17);
		this.simpleLabelItem24.Text = "Inventory Valuation Simulation Report";
		this.simpleLabelItem24.TextSize = new System.Drawing.Size(182, 13);
		this.layoutControlItem147.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem147.Control = this.SimReport;
		this.layoutControlItem147.CustomizationFormText = "layoutControlItem147";
		this.layoutControlItem147.EditControl = null;
		this.layoutControlItem147.EditValue = "layoutControlItem147";
		this.layoutControlItem147.FieldName = null;
		this.layoutControlItem147.IsSearchControl = false;
		this.layoutControlItem147.Location = new System.Drawing.Point(0, 27);
		this.layoutControlItem147.Name = "layoutControlItem147";
		this.layoutControlItem147.ReadOnly = false;
		this.layoutControlItem147.SettingInVisible = false;
		this.layoutControlItem147.SettingReadonly = false;
		this.layoutControlItem147.Size = new System.Drawing.Size(854, 63);
		this.layoutControlItem147.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem147.TextVisible = false;
		this.emptySpaceItem83.AllowHotTrack = false;
		this.emptySpaceItem83.CustomizationFormText = "emptySpaceItem83";
		this.emptySpaceItem83.Location = new System.Drawing.Point(0, 17);
		this.emptySpaceItem83.Name = "emptySpaceItem83";
		this.emptySpaceItem83.Size = new System.Drawing.Size(854, 10);
		this.emptySpaceItem83.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_Resources.Controls.Add(this.layoutControl8);
		this.Tab_Resources.EditControl = null;
		this.Tab_Resources.EditValue = "Resources";
		this.Tab_Resources.FieldName = "Tab_Resources";
		this.Tab_Resources.IsSearchControl = false;
		this.Tab_Resources.Name = "Tab_Resources";
		this.Tab_Resources.ReadOnly = false;
		this.Tab_Resources.SettingInVisible = false;
		this.Tab_Resources.SettingReadonly = false;
		this.Tab_Resources.Size = new System.Drawing.Size(876, 452);
		this.Tab_Resources.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Resources.Text = "Resources";
		this.layoutControl8.Controls.Add(this.Days);
		this.layoutControl8.Controls.Add(this.Months);
		this.layoutControl8.Controls.Add(this.AutoResWhs);
		this.layoutControl8.Controls.Add(this.DftResWhs);
		this.layoutControl8.Controls.Add(this.StartFrom);
		this.layoutControl8.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl8.Location = new System.Drawing.Point(0, 0);
		this.layoutControl8.Name = "layoutControl8";
		this.layoutControl8.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(854, 270, 250, 350);
		this.layoutControl8.Root = this.layoutControlGroup7;
		this.layoutControl8.Size = new System.Drawing.Size(876, 452);
		this.layoutControl8.TabIndex = 0;
		this.layoutControl8.Text = "layoutControl8";
		this.Days.EditControl = null;
		this.Days.FieldName = null;
		this.Days.IsSearchControl = false;
		this.Days.Location = new System.Drawing.Point(517, 77);
		this.Days.Name = "Days";
		this.Days.SettingInVisible = false;
		this.Days.SettingReadonly = false;
		this.Days.Size = new System.Drawing.Size(68, 20);
		this.Days.StyleController = this.layoutControl8;
		this.Days.TabIndex = 8;
		this.Days.EditValueChanged += new System.EventHandler(this.Days_EditValueChanged);
		this.Months.EditControl = null;
		this.Months.FieldName = null;
		this.Months.IsSearchControl = false;
		this.Months.Location = new System.Drawing.Point(324, 77);
		this.Months.Name = "Months";
		this.Months.SettingInVisible = false;
		this.Months.SettingReadonly = false;
		this.Months.Size = new System.Drawing.Size(69, 20);
		this.Months.StyleController = this.layoutControl8;
		this.Months.TabIndex = 7;
		this.Months.EditValueChanged += new System.EventHandler(this.Months_EditValueChanged);
		this.AutoResWhs.EditControl = null;
		this.AutoResWhs.EditValue = true;
		this.AutoResWhs.FieldName = null;
		this.AutoResWhs.IsSearchControl = false;
		this.AutoResWhs.Location = new System.Drawing.Point(12, 36);
		this.AutoResWhs.Name = "AutoResWhs";
		this.AutoResWhs.Properties.Caption = "Auto Add All Warehouses to New Resources";
		this.AutoResWhs.Properties.DisplayValueChecked = "Y";
		this.AutoResWhs.Properties.DisplayValueUnchecked = "N";
		this.AutoResWhs.Properties.ValueChecked = "Y";
		this.AutoResWhs.Properties.ValueGrayed = "";
		this.AutoResWhs.Properties.ValueUnchecked = "N";
		this.AutoResWhs.SettingInVisible = false;
		this.AutoResWhs.SettingReadonly = false;
		this.AutoResWhs.Size = new System.Drawing.Size(573, 20);
		this.AutoResWhs.StyleController = this.layoutControl8;
		this.AutoResWhs.TabIndex = 5;
		this.AutoResWhs.CheckedChanged += new System.EventHandler(this.AutoResWhs_CheckedChanged);
		this.DftResWhs.EditControl = null;
		this.DftResWhs.FieldName = null;
		this.DftResWhs.IsSearchControl = false;
		this.DftResWhs.Location = new System.Drawing.Point(132, 12);
		this.DftResWhs.Name = "DftResWhs";
		this.DftResWhs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DftResWhs.Properties.IsSearchControl = false;
		this.DftResWhs.Properties.NullText = "";
		this.DftResWhs.SettingInVisible = false;
		this.DftResWhs.SettingReadonly = false;
		this.DftResWhs.Size = new System.Drawing.Size(233, 20);
		this.DftResWhs.StyleController = this.layoutControl8;
		this.DftResWhs.TabIndex = 4;
		this.DftResWhs.EditValueChanged += new System.EventHandler(this.DftResWhs_EditValueChanged);
		this.StartFrom.EditControl = null;
		this.StartFrom.FieldName = null;
		this.StartFrom.IsSearchControl = false;
		this.StartFrom.Location = new System.Drawing.Point(132, 77);
		this.StartFrom.Name = "StartFrom";
		this.StartFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.StartFrom.Properties.IsSearchControl = false;
		this.StartFrom.Properties.NullText = "";
		this.StartFrom.SettingInVisible = false;
		this.StartFrom.SettingReadonly = false;
		this.StartFrom.Size = new System.Drawing.Size(68, 20);
		this.StartFrom.StyleController = this.layoutControl8;
		this.StartFrom.TabIndex = 6;
		this.StartFrom.EditValueChanged += new System.EventHandler(this.StartFrom_EditValueChanged);
		this.layoutControlGroup7.CustomizationFormText = "Root";
		this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup7.GroupBordersVisible = false;
		this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.emptySpaceItem56, this.emptySpaceItem57, this.layoutControlItem110, this.layoutControlItem111, this.simpleLabelItem15, this.layoutControlItem112, this.layoutControlItem113, this.layoutControlItem114, this.emptySpaceItem58 });
		this.layoutControlGroup7.Name = "Root";
		this.layoutControlGroup7.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup7.TextVisible = false;
		this.emptySpaceItem56.AllowHotTrack = false;
		this.emptySpaceItem56.CustomizationFormText = "emptySpaceItem56";
		this.emptySpaceItem56.Location = new System.Drawing.Point(0, 89);
		this.emptySpaceItem56.Name = "emptySpaceItem56";
		this.emptySpaceItem56.Size = new System.Drawing.Size(577, 343);
		this.emptySpaceItem56.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem57.AllowHotTrack = false;
		this.emptySpaceItem57.CustomizationFormText = "emptySpaceItem57";
		this.emptySpaceItem57.Location = new System.Drawing.Point(577, 0);
		this.emptySpaceItem57.Name = "emptySpaceItem57";
		this.emptySpaceItem57.Size = new System.Drawing.Size(279, 432);
		this.emptySpaceItem57.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem110.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem110.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem110.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem110.Control = this.DftResWhs;
		this.layoutControlItem110.CustomizationFormText = "layoutControlItem110";
		this.layoutControlItem110.EditControl = null;
		this.layoutControlItem110.EditValue = "Default Warehouse";
		this.layoutControlItem110.FieldName = null;
		this.layoutControlItem110.IsSearchControl = false;
		this.layoutControlItem110.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem110.Name = "layoutControlItem110";
		this.layoutControlItem110.ReadOnly = false;
		this.layoutControlItem110.SettingInVisible = false;
		this.layoutControlItem110.SettingReadonly = false;
		this.layoutControlItem110.Size = new System.Drawing.Size(357, 24);
		this.layoutControlItem110.Text = "Default Warehouse";
		this.layoutControlItem110.TextSize = new System.Drawing.Size(117, 13);
		this.layoutControlItem111.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem111.Control = this.AutoResWhs;
		this.layoutControlItem111.CustomizationFormText = "layoutControlItem111";
		this.layoutControlItem111.EditControl = null;
		this.layoutControlItem111.EditValue = "layoutControlItem111";
		this.layoutControlItem111.FieldName = null;
		this.layoutControlItem111.IsSearchControl = false;
		this.layoutControlItem111.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem111.Name = "layoutControlItem111";
		this.layoutControlItem111.ReadOnly = false;
		this.layoutControlItem111.SettingInVisible = false;
		this.layoutControlItem111.SettingReadonly = false;
		this.layoutControlItem111.Size = new System.Drawing.Size(577, 24);
		this.layoutControlItem111.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem111.TextVisible = false;
		this.simpleLabelItem15.AllowHotTrack = false;
		this.simpleLabelItem15.CustomizationFormText = "LabelsimpleLabelItem15";
		this.simpleLabelItem15.Location = new System.Drawing.Point(0, 48);
		this.simpleLabelItem15.Name = "simpleLabelItem15";
		this.simpleLabelItem15.Size = new System.Drawing.Size(577, 17);
		this.simpleLabelItem15.Text = "Default Capacity Period:";
		this.simpleLabelItem15.TextSize = new System.Drawing.Size(117, 13);
		this.layoutControlItem112.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem112.Control = this.StartFrom;
		this.layoutControlItem112.CustomizationFormText = "layoutControlItem112";
		this.layoutControlItem112.EditControl = null;
		this.layoutControlItem112.EditValue = "Start From Today Until";
		this.layoutControlItem112.FieldName = null;
		this.layoutControlItem112.IsSearchControl = false;
		this.layoutControlItem112.Location = new System.Drawing.Point(0, 65);
		this.layoutControlItem112.Name = "layoutControlItem112";
		this.layoutControlItem112.ReadOnly = false;
		this.layoutControlItem112.SettingInVisible = false;
		this.layoutControlItem112.SettingReadonly = false;
		this.layoutControlItem112.Size = new System.Drawing.Size(192, 24);
		this.layoutControlItem112.Text = "Start From Today Until";
		this.layoutControlItem112.TextSize = new System.Drawing.Size(117, 13);
		this.layoutControlItem113.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem113.Control = this.Months;
		this.layoutControlItem113.CustomizationFormText = "layoutControlItem113";
		this.layoutControlItem113.EditControl = null;
		this.layoutControlItem113.EditValue = "              +";
		this.layoutControlItem113.FieldName = null;
		this.layoutControlItem113.IsSearchControl = false;
		this.layoutControlItem113.Location = new System.Drawing.Point(192, 65);
		this.layoutControlItem113.Name = "layoutControlItem113";
		this.layoutControlItem113.ReadOnly = false;
		this.layoutControlItem113.SettingInVisible = false;
		this.layoutControlItem113.SettingReadonly = false;
		this.layoutControlItem113.Size = new System.Drawing.Size(193, 24);
		this.layoutControlItem113.Text = "              +";
		this.layoutControlItem113.TextSize = new System.Drawing.Size(117, 13);
		this.layoutControlItem114.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem114.Control = this.Days;
		this.layoutControlItem114.CustomizationFormText = "layoutControlItem114";
		this.layoutControlItem114.EditControl = null;
		this.layoutControlItem114.EditValue = "    Months +";
		this.layoutControlItem114.FieldName = null;
		this.layoutControlItem114.IsSearchControl = false;
		this.layoutControlItem114.Location = new System.Drawing.Point(385, 65);
		this.layoutControlItem114.Name = "layoutControlItem114";
		this.layoutControlItem114.ReadOnly = false;
		this.layoutControlItem114.SettingInVisible = false;
		this.layoutControlItem114.SettingReadonly = false;
		this.layoutControlItem114.Size = new System.Drawing.Size(192, 24);
		this.layoutControlItem114.Text = "    Months +";
		this.layoutControlItem114.TextSize = new System.Drawing.Size(117, 13);
		this.emptySpaceItem58.AllowHotTrack = false;
		this.emptySpaceItem58.CustomizationFormText = "emptySpaceItem58";
		this.emptySpaceItem58.Location = new System.Drawing.Point(357, 0);
		this.emptySpaceItem58.Name = "emptySpaceItem58";
		this.emptySpaceItem58.Size = new System.Drawing.Size(220, 24);
		this.emptySpaceItem58.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_CashFlow.Controls.Add(this.layoutControl9);
		this.Tab_CashFlow.EditControl = null;
		this.Tab_CashFlow.EditValue = "Cash Flow";
		this.Tab_CashFlow.FieldName = "Tab_CashFlow";
		this.Tab_CashFlow.IsSearchControl = false;
		this.Tab_CashFlow.Name = "Tab_CashFlow";
		this.Tab_CashFlow.PageVisible = false;
		this.Tab_CashFlow.ReadOnly = false;
		this.Tab_CashFlow.SettingInVisible = false;
		this.Tab_CashFlow.SettingReadonly = false;
		this.Tab_CashFlow.Size = new System.Drawing.Size(876, 452);
		this.Tab_CashFlow.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_CashFlow.Text = "Cash Flow";
		this.layoutControl9.Controls.Add(this.IgrAllCash);
		this.layoutControl9.Controls.Add(this.CfwAsnMust);
		this.layoutControl9.Controls.Add(this.CfwInDflt);
		this.layoutControl9.Controls.Add(this.CfwOutDflt);
		this.layoutControl9.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl9.Location = new System.Drawing.Point(0, 0);
		this.layoutControl9.Name = "layoutControl9";
		this.layoutControl9.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(707, 309, 250, 350);
		this.layoutControl9.Root = this.layoutControlGroup8;
		this.layoutControl9.Size = new System.Drawing.Size(876, 452);
		this.layoutControl9.TabIndex = 0;
		this.layoutControl9.Text = "layoutControl9";
		this.IgrAllCash.EditControl = null;
		this.IgrAllCash.FieldName = null;
		this.IgrAllCash.IsSearchControl = false;
		this.IgrAllCash.Location = new System.Drawing.Point(12, 253);
		this.IgrAllCash.Name = "IgrAllCash";
		this.IgrAllCash.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.IgrAllCash.Properties.Columns = 1;
		this.IgrAllCash.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Y", "Ignore Without Warning"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Warning Only")
		});
		this.IgrAllCash.SettingInVisible = false;
		this.IgrAllCash.SettingReadonly = false;
		this.IgrAllCash.Size = new System.Drawing.Size(585, 57);
		this.IgrAllCash.StyleController = this.layoutControl9;
		this.IgrAllCash.TabIndex = 7;
		this.IgrAllCash.SelectedIndexChanged += new System.EventHandler(this.IgrAllCash_SelectedIndexChanged);
		this.CfwAsnMust.EditControl = null;
		this.CfwAsnMust.FieldName = null;
		this.CfwAsnMust.IsSearchControl = false;
		this.CfwAsnMust.Location = new System.Drawing.Point(12, 29);
		this.CfwAsnMust.Name = "CfwAsnMust";
		this.CfwAsnMust.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.CfwAsnMust.Properties.Columns = 1;
		this.CfwAsnMust.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Y", "Mandator&y"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Optional")
		});
		this.CfwAsnMust.SettingInVisible = false;
		this.CfwAsnMust.SettingReadonly = false;
		this.CfwAsnMust.Size = new System.Drawing.Size(585, 57);
		this.CfwAsnMust.StyleController = this.layoutControl9;
		this.CfwAsnMust.TabIndex = 4;
		this.CfwAsnMust.SelectedIndexChanged += new System.EventHandler(this.radioGroup5_SelectedIndexChanged);
		this.CfwInDflt.EditControl = null;
		this.CfwInDflt.FieldName = null;
		this.CfwInDflt.IsSearchControl = false;
		this.CfwInDflt.Location = new System.Drawing.Point(292, 188);
		this.CfwInDflt.Name = "CfwInDflt";
		this.CfwInDflt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CfwInDflt.Properties.DisplayMember = "Name";
		this.CfwInDflt.Properties.IsSearchControl = false;
		this.CfwInDflt.Properties.NullText = "";
		this.CfwInDflt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.CfwInDflt.Properties.ValueMember = "Code";
		this.CfwInDflt.SettingInVisible = false;
		this.CfwInDflt.SettingReadonly = false;
		this.CfwInDflt.Size = new System.Drawing.Size(305, 20);
		this.CfwInDflt.StyleController = this.layoutControl9;
		this.CfwInDflt.TabIndex = 5;
		this.CfwInDflt.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_IncomingPayment_ButtonClick);
		this.CfwInDflt.EditValueChanged += new System.EventHandler(this.textEdit49_EditValueChanged);
		this.CfwOutDflt.EditControl = null;
		this.CfwOutDflt.FieldName = null;
		this.CfwOutDflt.IsSearchControl = false;
		this.CfwOutDflt.Location = new System.Drawing.Point(292, 212);
		this.CfwOutDflt.Name = "CfwOutDflt";
		this.CfwOutDflt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CfwOutDflt.Properties.DisplayMember = "Name";
		this.CfwOutDflt.Properties.IsSearchControl = false;
		this.CfwOutDflt.Properties.NullText = "";
		this.CfwOutDflt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.CfwOutDflt.Properties.ValueMember = "Code";
		this.CfwOutDflt.SettingInVisible = false;
		this.CfwOutDflt.SettingReadonly = false;
		this.CfwOutDflt.Size = new System.Drawing.Size(305, 20);
		this.CfwOutDflt.StyleController = this.layoutControl9;
		this.CfwOutDflt.TabIndex = 6;
		this.CfwOutDflt.EditValueChanged += new System.EventHandler(this.CfwOutDflt_EditValueChanged);
		this.layoutControlGroup8.CustomizationFormText = "Root";
		this.layoutControlGroup8.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup8.GroupBordersVisible = false;
		this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.emptySpaceItem59, this.emptySpaceItem60, this.simpleLabelItem16, this.layoutControlItem115, this.emptySpaceItem61, this.simpleLabelItem17, this.layoutControlItem116, this.layoutControlItem117, this.simpleLabelItem18, this.emptySpaceItem62,
			this.emptySpaceItem63, this.layoutControlItem118
		});
		this.layoutControlGroup8.Name = "Root";
		this.layoutControlGroup8.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup8.TextVisible = false;
		this.emptySpaceItem59.AllowHotTrack = false;
		this.emptySpaceItem59.CustomizationFormText = "emptySpaceItem59";
		this.emptySpaceItem59.Location = new System.Drawing.Point(0, 302);
		this.emptySpaceItem59.Name = "emptySpaceItem59";
		this.emptySpaceItem59.Size = new System.Drawing.Size(589, 130);
		this.emptySpaceItem59.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem60.AllowHotTrack = false;
		this.emptySpaceItem60.CustomizationFormText = "emptySpaceItem60";
		this.emptySpaceItem60.Location = new System.Drawing.Point(589, 0);
		this.emptySpaceItem60.Name = "emptySpaceItem60";
		this.emptySpaceItem60.Size = new System.Drawing.Size(267, 432);
		this.emptySpaceItem60.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem16.AllowHotTrack = false;
		this.simpleLabelItem16.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem16.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem16.CustomizationFormText = "LabelsimpleLabelItem16";
		this.simpleLabelItem16.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem16.Name = "simpleLabelItem16";
		this.simpleLabelItem16.Size = new System.Drawing.Size(589, 17);
		this.simpleLabelItem16.Text = "Assignment of Cash Flow Line Item";
		this.simpleLabelItem16.TextSize = new System.Drawing.Size(260, 13);
		this.layoutControlItem115.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem115.Control = this.CfwAsnMust;
		this.layoutControlItem115.CustomizationFormText = "layoutControlItem115";
		this.layoutControlItem115.EditControl = null;
		this.layoutControlItem115.EditValue = "layoutControlItem115";
		this.layoutControlItem115.FieldName = null;
		this.layoutControlItem115.IsSearchControl = false;
		this.layoutControlItem115.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem115.Name = "layoutControlItem115";
		this.layoutControlItem115.ReadOnly = false;
		this.layoutControlItem115.SettingInVisible = false;
		this.layoutControlItem115.SettingReadonly = false;
		this.layoutControlItem115.Size = new System.Drawing.Size(589, 61);
		this.layoutControlItem115.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem115.TextVisible = false;
		this.emptySpaceItem61.AllowHotTrack = false;
		this.emptySpaceItem61.CustomizationFormText = "emptySpaceItem61";
		this.emptySpaceItem61.Location = new System.Drawing.Point(0, 78);
		this.emptySpaceItem61.Name = "emptySpaceItem61";
		this.emptySpaceItem61.Size = new System.Drawing.Size(589, 81);
		this.emptySpaceItem61.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem17.AllowHotTrack = false;
		this.simpleLabelItem17.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem17.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem17.CustomizationFormText = "LabelsimpleLabelItem17";
		this.simpleLabelItem17.Location = new System.Drawing.Point(0, 159);
		this.simpleLabelItem17.Name = "simpleLabelItem17";
		this.simpleLabelItem17.Size = new System.Drawing.Size(589, 17);
		this.simpleLabelItem17.Text = "Default Primary Form Cash Flow Line Item";
		this.simpleLabelItem17.TextSize = new System.Drawing.Size(260, 13);
		this.layoutControlItem116.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem116.Control = this.CfwInDflt;
		this.layoutControlItem116.CustomizationFormText = "layoutControlItem116";
		this.layoutControlItem116.EditControl = null;
		this.layoutControlItem116.EditValue = "Incoming Payment";
		this.layoutControlItem116.FieldName = null;
		this.layoutControlItem116.IsSearchControl = false;
		this.layoutControlItem116.Location = new System.Drawing.Point(17, 176);
		this.layoutControlItem116.Name = "layoutControlItem116";
		this.layoutControlItem116.ReadOnly = false;
		this.layoutControlItem116.SettingInVisible = false;
		this.layoutControlItem116.SettingReadonly = false;
		this.layoutControlItem116.Size = new System.Drawing.Size(572, 24);
		this.layoutControlItem116.Text = "Incoming Payment";
		this.layoutControlItem116.TextSize = new System.Drawing.Size(260, 13);
		this.layoutControlItem117.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem117.Control = this.CfwOutDflt;
		this.layoutControlItem117.CustomizationFormText = "layoutControlItem117";
		this.layoutControlItem117.EditControl = null;
		this.layoutControlItem117.EditValue = "Outgoing Payment";
		this.layoutControlItem117.FieldName = null;
		this.layoutControlItem117.IsSearchControl = false;
		this.layoutControlItem117.Location = new System.Drawing.Point(17, 200);
		this.layoutControlItem117.Name = "layoutControlItem117";
		this.layoutControlItem117.ReadOnly = false;
		this.layoutControlItem117.SettingInVisible = false;
		this.layoutControlItem117.SettingReadonly = false;
		this.layoutControlItem117.Size = new System.Drawing.Size(572, 24);
		this.layoutControlItem117.Text = "Outgoing Payment";
		this.layoutControlItem117.TextSize = new System.Drawing.Size(260, 13);
		this.simpleLabelItem18.AllowHotTrack = false;
		this.simpleLabelItem18.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleLabelItem18.AppearanceItemCaption.Options.UseFont = true;
		this.simpleLabelItem18.CustomizationFormText = "LabelsimpleLabelItem18";
		this.simpleLabelItem18.Location = new System.Drawing.Point(0, 224);
		this.simpleLabelItem18.Name = "simpleLabelItem18";
		this.simpleLabelItem18.Size = new System.Drawing.Size(589, 17);
		this.simpleLabelItem18.Text = "Assignment in Transaction with All Relevant Cash Flow";
		this.simpleLabelItem18.TextSize = new System.Drawing.Size(260, 13);
		this.emptySpaceItem62.AllowHotTrack = false;
		this.emptySpaceItem62.CustomizationFormText = "emptySpaceItem62";
		this.emptySpaceItem62.Location = new System.Drawing.Point(0, 176);
		this.emptySpaceItem62.Name = "emptySpaceItem62";
		this.emptySpaceItem62.Size = new System.Drawing.Size(17, 24);
		this.emptySpaceItem62.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem63.AllowHotTrack = false;
		this.emptySpaceItem63.CustomizationFormText = "emptySpaceItem63";
		this.emptySpaceItem63.Location = new System.Drawing.Point(0, 200);
		this.emptySpaceItem63.Name = "emptySpaceItem63";
		this.emptySpaceItem63.Size = new System.Drawing.Size(17, 24);
		this.emptySpaceItem63.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem118.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem118.Control = this.IgrAllCash;
		this.layoutControlItem118.CustomizationFormText = "layoutControlItem118";
		this.layoutControlItem118.EditControl = null;
		this.layoutControlItem118.EditValue = "layoutControlItem118";
		this.layoutControlItem118.FieldName = null;
		this.layoutControlItem118.IsSearchControl = false;
		this.layoutControlItem118.Location = new System.Drawing.Point(0, 241);
		this.layoutControlItem118.Name = "layoutControlItem118";
		this.layoutControlItem118.ReadOnly = false;
		this.layoutControlItem118.SettingInVisible = false;
		this.layoutControlItem118.SettingReadonly = false;
		this.layoutControlItem118.Size = new System.Drawing.Size(589, 61);
		this.layoutControlItem118.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem118.TextVisible = false;
		this.Tab_Cockpit.Controls.Add(this.layoutControl10);
		this.Tab_Cockpit.EditControl = null;
		this.Tab_Cockpit.EditValue = "Cockpit";
		this.Tab_Cockpit.FieldName = "Tab_Cockpit";
		this.Tab_Cockpit.IsSearchControl = false;
		this.Tab_Cockpit.Name = "Tab_Cockpit";
		this.Tab_Cockpit.PageVisible = false;
		this.Tab_Cockpit.ReadOnly = false;
		this.Tab_Cockpit.SettingInVisible = false;
		this.Tab_Cockpit.SettingReadonly = false;
		this.Tab_Cockpit.Size = new System.Drawing.Size(876, 452);
		this.Tab_Cockpit.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Cockpit.Text = "Cockpit";
		this.layoutControl10.Controls.Add(this.btn_More);
		this.layoutControl10.Controls.Add(this.PrsWkCntEb);
		this.layoutControl10.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl10.Location = new System.Drawing.Point(0, 0);
		this.layoutControl10.Name = "layoutControl10";
		this.layoutControl10.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(745, 369, 250, 350);
		this.layoutControl10.Root = this.layoutControlGroup9;
		this.layoutControl10.Size = new System.Drawing.Size(876, 452);
		this.layoutControl10.TabIndex = 0;
		this.layoutControl10.Text = "layoutControl10";
		this.btn_More.EditControl = null;
		this.btn_More.EditValue = "More ...";
		this.btn_More.FieldName = null;
		this.btn_More.IsSearchControl = false;
		this.btn_More.Location = new System.Drawing.Point(12, 129);
		this.btn_More.Name = "btn_More";
		this.btn_More.ReadOnly = false;
		this.btn_More.SettingInVisible = false;
		this.btn_More.SettingReadonly = false;
		this.btn_More.Size = new System.Drawing.Size(86, 22);
		this.btn_More.StyleController = this.layoutControl10;
		this.btn_More.TabIndex = 5;
		this.btn_More.Text = "More ...";
		this.PrsWkCntEb.EditControl = null;
		this.PrsWkCntEb.FieldName = null;
		this.PrsWkCntEb.IsSearchControl = false;
		this.PrsWkCntEb.Location = new System.Drawing.Point(12, 47);
		this.PrsWkCntEb.Name = "PrsWkCntEb";
		this.PrsWkCntEb.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.PrsWkCntEb.Properties.Columns = 1;
		this.PrsWkCntEb.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Y", "Cookpit"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "None")
		});
		this.PrsWkCntEb.SettingInVisible = false;
		this.PrsWkCntEb.SettingReadonly = false;
		this.PrsWkCntEb.Size = new System.Drawing.Size(425, 57);
		this.PrsWkCntEb.StyleController = this.layoutControl10;
		this.PrsWkCntEb.TabIndex = 4;
		this.PrsWkCntEb.SelectedIndexChanged += new System.EventHandler(this.PrsWkCntEb_SelectedIndexChanged);
		this.layoutControlGroup9.CustomizationFormText = "Root";
		this.layoutControlGroup9.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup9.GroupBordersVisible = false;
		this.layoutControlGroup9.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.emptySpaceItem64, this.emptySpaceItem65, this.layoutControlItem119, this.emptySpaceItem66, this.layoutControlItem120, this.emptySpaceItem67, this.emptySpaceItem68 });
		this.layoutControlGroup9.Name = "Root";
		this.layoutControlGroup9.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup9.TextVisible = false;
		this.emptySpaceItem64.AllowHotTrack = false;
		this.emptySpaceItem64.CustomizationFormText = "emptySpaceItem64";
		this.emptySpaceItem64.Location = new System.Drawing.Point(429, 0);
		this.emptySpaceItem64.Name = "emptySpaceItem64";
		this.emptySpaceItem64.Size = new System.Drawing.Size(427, 432);
		this.emptySpaceItem64.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem65.AllowHotTrack = false;
		this.emptySpaceItem65.CustomizationFormText = "emptySpaceItem65";
		this.emptySpaceItem65.Location = new System.Drawing.Point(0, 143);
		this.emptySpaceItem65.Name = "emptySpaceItem65";
		this.emptySpaceItem65.Size = new System.Drawing.Size(429, 289);
		this.emptySpaceItem65.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem119.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem119.Control = this.PrsWkCntEb;
		this.layoutControlItem119.CustomizationFormText = "layoutControlItem119";
		this.layoutControlItem119.EditControl = null;
		this.layoutControlItem119.EditValue = "layoutControlItem119";
		this.layoutControlItem119.FieldName = null;
		this.layoutControlItem119.IsSearchControl = false;
		this.layoutControlItem119.Location = new System.Drawing.Point(0, 35);
		this.layoutControlItem119.Name = "layoutControlItem119";
		this.layoutControlItem119.ReadOnly = false;
		this.layoutControlItem119.SettingInVisible = false;
		this.layoutControlItem119.SettingReadonly = false;
		this.layoutControlItem119.Size = new System.Drawing.Size(429, 61);
		this.layoutControlItem119.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem119.TextVisible = false;
		this.emptySpaceItem66.AllowHotTrack = false;
		this.emptySpaceItem66.CustomizationFormText = "emptySpaceItem66";
		this.emptySpaceItem66.Location = new System.Drawing.Point(0, 96);
		this.emptySpaceItem66.Name = "emptySpaceItem66";
		this.emptySpaceItem66.Size = new System.Drawing.Size(429, 21);
		this.emptySpaceItem66.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem120.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem120.Control = this.btn_More;
		this.layoutControlItem120.CustomizationFormText = "layoutControlItem120";
		this.layoutControlItem120.EditControl = null;
		this.layoutControlItem120.EditValue = "layoutControlItem120";
		this.layoutControlItem120.FieldName = null;
		this.layoutControlItem120.IsSearchControl = false;
		this.layoutControlItem120.Location = new System.Drawing.Point(0, 117);
		this.layoutControlItem120.Name = "layoutControlItem120";
		this.layoutControlItem120.ReadOnly = false;
		this.layoutControlItem120.SettingInVisible = false;
		this.layoutControlItem120.SettingReadonly = false;
		this.layoutControlItem120.Size = new System.Drawing.Size(90, 26);
		this.layoutControlItem120.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem120.TextVisible = false;
		this.emptySpaceItem67.AllowHotTrack = false;
		this.emptySpaceItem67.CustomizationFormText = "emptySpaceItem67";
		this.emptySpaceItem67.Location = new System.Drawing.Point(90, 117);
		this.emptySpaceItem67.Name = "emptySpaceItem67";
		this.emptySpaceItem67.Size = new System.Drawing.Size(339, 26);
		this.emptySpaceItem67.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem68.AllowHotTrack = false;
		this.emptySpaceItem68.CustomizationFormText = "emptySpaceItem68";
		this.emptySpaceItem68.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem68.Name = "emptySpaceItem68";
		this.emptySpaceItem68.Size = new System.Drawing.Size(429, 35);
		this.emptySpaceItem68.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_CostAccounting.Controls.Add(this.layoutControl11);
		this.Tab_CostAccounting.EditControl = null;
		this.Tab_CostAccounting.EditValue = "Cost Accounting";
		this.Tab_CostAccounting.FieldName = "Tab_CostAccounting";
		this.Tab_CostAccounting.IsSearchControl = false;
		this.Tab_CostAccounting.Name = "Tab_CostAccounting";
		this.Tab_CostAccounting.ReadOnly = false;
		this.Tab_CostAccounting.SettingInVisible = false;
		this.Tab_CostAccounting.SettingReadonly = false;
		this.Tab_CostAccounting.Size = new System.Drawing.Size(876, 452);
		this.Tab_CostAccounting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_CostAccounting.Text = "Cost Accounting";
		this.layoutControl11.Controls.Add(this.simpleButton2);
		this.layoutControl11.Controls.Add(this.ApyPRinPEC);
		this.layoutControl11.Controls.Add(this.ApyDRinPEC);
		this.layoutControl11.Controls.Add(this.label3);
		this.layoutControl11.Controls.Add(this.MDStyle);
		this.layoutControl11.Controls.Add(this.UseMltDims);
		this.layoutControl11.Controls.Add(this.PrjBlock);
		this.layoutControl11.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl11.Location = new System.Drawing.Point(0, 0);
		this.layoutControl11.Name = "layoutControl11";
		this.layoutControl11.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(728, 322, 250, 350);
		this.layoutControl11.Root = this.layoutControlGroup10;
		this.layoutControl11.Size = new System.Drawing.Size(876, 452);
		this.layoutControl11.TabIndex = 0;
		this.layoutControl11.Text = "layoutControl11";
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "...";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(174, 179);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(156, 22);
		this.simpleButton2.StyleController = this.layoutControl11;
		this.simpleButton2.TabIndex = 12;
		this.simpleButton2.Text = "...";
		this.ApyPRinPEC.EditControl = null;
		this.ApyPRinPEC.FieldName = null;
		this.ApyPRinPEC.IsSearchControl = false;
		this.ApyPRinPEC.Location = new System.Drawing.Point(349, 203);
		this.ApyPRinPEC.Name = "ApyPRinPEC";
		this.ApyPRinPEC.Properties.Caption = "Appl&y to Period-End Closing";
		this.ApyPRinPEC.Properties.DisplayValueChecked = "Y";
		this.ApyPRinPEC.Properties.DisplayValueUnchecked = "N";
		this.ApyPRinPEC.Properties.ValueChecked = "Y";
		this.ApyPRinPEC.Properties.ValueGrayed = "";
		this.ApyPRinPEC.Properties.ValueUnchecked = "N";
		this.ApyPRinPEC.SettingInVisible = false;
		this.ApyPRinPEC.SettingReadonly = false;
		this.ApyPRinPEC.Size = new System.Drawing.Size(343, 20);
		this.ApyPRinPEC.StyleController = this.layoutControl11;
		this.ApyPRinPEC.TabIndex = 11;
		this.ApyPRinPEC.CheckedChanged += new System.EventHandler(this.ApyPRinPEC_CheckedChanged);
		this.ApyDRinPEC.EditControl = null;
		this.ApyDRinPEC.FieldName = null;
		this.ApyDRinPEC.IsSearchControl = false;
		this.ApyDRinPEC.Location = new System.Drawing.Point(42, 205);
		this.ApyDRinPEC.Name = "ApyDRinPEC";
		this.ApyDRinPEC.Properties.Caption = "Apply to Period-End Closing";
		this.ApyDRinPEC.Properties.DisplayValueChecked = "Y";
		this.ApyDRinPEC.Properties.DisplayValueUnchecked = "N";
		this.ApyDRinPEC.Properties.ValueChecked = "Y";
		this.ApyDRinPEC.Properties.ValueGrayed = "";
		this.ApyDRinPEC.Properties.ValueUnchecked = "N";
		this.ApyDRinPEC.SettingInVisible = false;
		this.ApyDRinPEC.SettingReadonly = false;
		this.ApyDRinPEC.Size = new System.Drawing.Size(288, 20);
		this.ApyDRinPEC.StyleController = this.layoutControl11;
		this.ApyDRinPEC.TabIndex = 10;
		this.ApyDRinPEC.CheckedChanged += new System.EventHandler(this.ApyDRinPEC_CheckedChanged);
		this.label3.EditControl = null;
		this.label3.EditValue = "Specify how to post journal entry line without a distribution rule or project when G/L account is set up for cost accounting";
		this.label3.FieldName = null;
		this.label3.HideWhenEmpty = false;
		this.label3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label3.Location = new System.Drawing.Point(42, 130);
		this.label3.Name = "label3";
		this.label3.ReadOnly = false;
		this.label3.SettingInVisible = false;
		this.label3.SettingReadonly = false;
		this.label3.ShowUnderLine = true;
		this.label3.Size = new System.Drawing.Size(650, 45);
		this.label3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label3.TabIndex = 9;
		this.label3.Utext = "Specify how to post journal entry line without a distribution rule or project when G/L account is set up for cost accounting";
		this.label3.Visible = false;
		this.MDStyle.EditControl = null;
		this.MDStyle.FieldName = null;
		this.MDStyle.IsSearchControl = false;
		this.MDStyle.Location = new System.Drawing.Point(42, 63);
		this.MDStyle.Name = "MDStyle";
		this.MDStyle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.MDStyle.Properties.Columns = 1;
		this.MDStyle.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("U", "In a Unified Column"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("S", "In Separate Columns")
		});
		this.MDStyle.SettingInVisible = false;
		this.MDStyle.SettingReadonly = false;
		this.MDStyle.Size = new System.Drawing.Size(650, 63);
		this.MDStyle.StyleController = this.layoutControl11;
		this.MDStyle.TabIndex = 8;
		this.MDStyle.SelectedIndexChanged += new System.EventHandler(this.MDStyle_SelectedIndexChanged);
		this.UseMltDims.EditControl = null;
		this.UseMltDims.EditValue = true;
		this.UseMltDims.FieldName = null;
		this.UseMltDims.IsSearchControl = false;
		this.UseMltDims.Location = new System.Drawing.Point(12, 12);
		this.UseMltDims.Name = "UseMltDims";
		this.UseMltDims.Properties.Caption = "Use Multidimensions";
		this.UseMltDims.Properties.DisplayValueChecked = "Y";
		this.UseMltDims.Properties.DisplayValueUnchecked = "N";
		this.UseMltDims.Properties.ValueChecked = "Y";
		this.UseMltDims.Properties.ValueGrayed = "";
		this.UseMltDims.Properties.ValueUnchecked = "N";
		this.UseMltDims.SettingInVisible = false;
		this.UseMltDims.SettingReadonly = false;
		this.UseMltDims.Size = new System.Drawing.Size(680, 20);
		this.UseMltDims.StyleController = this.layoutControl11;
		this.UseMltDims.TabIndex = 4;
		this.UseMltDims.CheckedChanged += new System.EventHandler(this.checkEdit46_CheckedChanged);
		this.PrjBlock.EditControl = null;
		this.PrjBlock.FieldName = null;
		this.PrjBlock.IsSearchControl = false;
		this.PrjBlock.Location = new System.Drawing.Point(472, 179);
		this.PrjBlock.Name = "PrjBlock";
		this.PrjBlock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PrjBlock.Properties.IsSearchControl = false;
		this.PrjBlock.Properties.NullText = "";
		this.PrjBlock.SettingInVisible = false;
		this.PrjBlock.SettingReadonly = false;
		this.PrjBlock.Size = new System.Drawing.Size(220, 20);
		this.PrjBlock.StyleController = this.layoutControl11;
		this.PrjBlock.TabIndex = 7;
		this.PrjBlock.EditValueChanged += new System.EventHandler(this.PrjBlock_EditValueChanged);
		this.layoutControlGroup10.CustomizationFormText = "Root";
		this.layoutControlGroup10.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup10.GroupBordersVisible = false;
		this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
		{
			this.layoutControlItem121, this.emptySpaceItem70, this.simpleLabelItem19, this.layoutControlItem124, this.emptySpaceItem72, this.emptySpaceItem73, this.emptySpaceItem85, this.emptySpaceItem86, this.emptySpaceItem87, this.layoutControlItem122,
			this.layoutControlItem148, this.layoutControlItem133, this.layoutControlItem155, this.simpleLabelItem5, this.layoutControlItem18, this.emptySpaceItem30, this.emptySpaceItem31
		});
		this.layoutControlGroup10.Name = "Root";
		this.layoutControlGroup10.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup10.TextVisible = false;
		this.layoutControlItem121.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem121.Control = this.UseMltDims;
		this.layoutControlItem121.CustomizationFormText = "layoutControlItem121";
		this.layoutControlItem121.EditControl = null;
		this.layoutControlItem121.EditValue = "layoutControlItem121";
		this.layoutControlItem121.FieldName = null;
		this.layoutControlItem121.IsSearchControl = false;
		this.layoutControlItem121.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem121.Name = "layoutControlItem121";
		this.layoutControlItem121.ReadOnly = false;
		this.layoutControlItem121.SettingInVisible = false;
		this.layoutControlItem121.SettingReadonly = false;
		this.layoutControlItem121.Size = new System.Drawing.Size(684, 24);
		this.layoutControlItem121.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem121.TextVisible = false;
		this.emptySpaceItem70.AllowHotTrack = false;
		this.emptySpaceItem70.CustomizationFormText = "emptySpaceItem70";
		this.emptySpaceItem70.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem70.Name = "emptySpaceItem70";
		this.emptySpaceItem70.Size = new System.Drawing.Size(684, 10);
		this.emptySpaceItem70.TextSize = new System.Drawing.Size(0, 0);
		this.simpleLabelItem19.AllowHotTrack = false;
		this.simpleLabelItem19.CustomizationFormText = "Display Distribution Rules";
		this.simpleLabelItem19.Location = new System.Drawing.Point(30, 34);
		this.simpleLabelItem19.Name = "simpleLabelItem19";
		this.simpleLabelItem19.Size = new System.Drawing.Size(654, 17);
		this.simpleLabelItem19.Text = "Display Distribution Rules";
		this.simpleLabelItem19.TextSize = new System.Drawing.Size(120, 13);
		this.layoutControlItem124.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem124.Control = this.PrjBlock;
		this.layoutControlItem124.CustomizationFormText = "layoutControlItem124";
		this.layoutControlItem124.EditControl = null;
		this.layoutControlItem124.EditValue = "Project Code";
		this.layoutControlItem124.FieldName = null;
		this.layoutControlItem124.IsSearchControl = false;
		this.layoutControlItem124.Location = new System.Drawing.Point(337, 167);
		this.layoutControlItem124.Name = "layoutControlItem124";
		this.layoutControlItem124.ReadOnly = false;
		this.layoutControlItem124.SettingInVisible = false;
		this.layoutControlItem124.SettingReadonly = false;
		this.layoutControlItem124.Size = new System.Drawing.Size(347, 24);
		this.layoutControlItem124.Text = "Project Code";
		this.layoutControlItem124.TextSize = new System.Drawing.Size(120, 13);
		this.emptySpaceItem72.AllowHotTrack = false;
		this.emptySpaceItem72.CustomizationFormText = "emptySpaceItem72";
		this.emptySpaceItem72.Location = new System.Drawing.Point(322, 167);
		this.emptySpaceItem72.Name = "emptySpaceItem72";
		this.emptySpaceItem72.Size = new System.Drawing.Size(15, 50);
		this.emptySpaceItem72.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem73.AllowHotTrack = false;
		this.emptySpaceItem73.AllowHtmlStringInCaption = true;
		this.emptySpaceItem73.CustomizationFormText = "emptySpaceItem73";
		this.emptySpaceItem73.Location = new System.Drawing.Point(684, 0);
		this.emptySpaceItem73.Name = "emptySpaceItem73";
		this.emptySpaceItem73.Size = new System.Drawing.Size(172, 217);
		this.emptySpaceItem73.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem85.AllowHotTrack = false;
		this.emptySpaceItem85.CustomizationFormText = "emptySpaceItem85";
		this.emptySpaceItem85.Location = new System.Drawing.Point(0, 34);
		this.emptySpaceItem85.Name = "emptySpaceItem85";
		this.emptySpaceItem85.Size = new System.Drawing.Size(30, 17);
		this.emptySpaceItem85.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem86.AllowHotTrack = false;
		this.emptySpaceItem86.CustomizationFormText = "emptySpaceItem86";
		this.emptySpaceItem86.Location = new System.Drawing.Point(0, 51);
		this.emptySpaceItem86.Name = "emptySpaceItem86";
		this.emptySpaceItem86.Size = new System.Drawing.Size(30, 116);
		this.emptySpaceItem86.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem87.AllowHotTrack = false;
		this.emptySpaceItem87.CustomizationFormText = "emptySpaceItem87";
		this.emptySpaceItem87.Location = new System.Drawing.Point(0, 167);
		this.emptySpaceItem87.Name = "emptySpaceItem87";
		this.emptySpaceItem87.Size = new System.Drawing.Size(30, 50);
		this.emptySpaceItem87.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem122.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem122.Control = this.MDStyle;
		this.layoutControlItem122.CustomizationFormText = "layoutControlItem122";
		this.layoutControlItem122.EditControl = null;
		this.layoutControlItem122.EditValue = "layoutControlItem122";
		this.layoutControlItem122.FieldName = null;
		this.layoutControlItem122.IsSearchControl = false;
		this.layoutControlItem122.Location = new System.Drawing.Point(30, 51);
		this.layoutControlItem122.Name = "layoutControlItem122";
		this.layoutControlItem122.ReadOnly = false;
		this.layoutControlItem122.SettingInVisible = false;
		this.layoutControlItem122.SettingReadonly = false;
		this.layoutControlItem122.Size = new System.Drawing.Size(654, 67);
		this.layoutControlItem122.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem122.TextVisible = false;
		this.layoutControlItem148.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem148.Control = this.label3;
		this.layoutControlItem148.CustomizationFormText = "layoutControlItem148";
		this.layoutControlItem148.EditControl = null;
		this.layoutControlItem148.EditValue = "layoutControlItem148";
		this.layoutControlItem148.FieldName = null;
		this.layoutControlItem148.IsSearchControl = false;
		this.layoutControlItem148.Location = new System.Drawing.Point(30, 118);
		this.layoutControlItem148.Name = "layoutControlItem148";
		this.layoutControlItem148.ReadOnly = false;
		this.layoutControlItem148.SettingInVisible = false;
		this.layoutControlItem148.SettingReadonly = false;
		this.layoutControlItem148.Size = new System.Drawing.Size(654, 49);
		this.layoutControlItem148.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem148.TextVisible = false;
		this.layoutControlItem148.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem133.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem133.Control = this.ApyDRinPEC;
		this.layoutControlItem133.CustomizationFormText = "layoutControlItem133";
		this.layoutControlItem133.EditControl = null;
		this.layoutControlItem133.EditValue = "layoutControlItem133";
		this.layoutControlItem133.FieldName = null;
		this.layoutControlItem133.IsSearchControl = false;
		this.layoutControlItem133.Location = new System.Drawing.Point(30, 193);
		this.layoutControlItem133.Name = "layoutControlItem133";
		this.layoutControlItem133.ReadOnly = false;
		this.layoutControlItem133.SettingInVisible = false;
		this.layoutControlItem133.SettingReadonly = false;
		this.layoutControlItem133.Size = new System.Drawing.Size(292, 24);
		this.layoutControlItem133.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem133.TextVisible = false;
		this.layoutControlItem133.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem155.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem155.Control = this.ApyPRinPEC;
		this.layoutControlItem155.CustomizationFormText = "layoutControlItem155";
		this.layoutControlItem155.EditControl = null;
		this.layoutControlItem155.EditValue = "layoutControlItem155";
		this.layoutControlItem155.FieldName = null;
		this.layoutControlItem155.IsSearchControl = false;
		this.layoutControlItem155.Location = new System.Drawing.Point(337, 191);
		this.layoutControlItem155.Name = "layoutControlItem155";
		this.layoutControlItem155.ReadOnly = false;
		this.layoutControlItem155.SettingInVisible = false;
		this.layoutControlItem155.SettingReadonly = false;
		this.layoutControlItem155.Size = new System.Drawing.Size(347, 26);
		this.layoutControlItem155.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem155.TextVisible = false;
		this.layoutControlItem155.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.simpleLabelItem5.AllowHotTrack = false;
		this.simpleLabelItem5.CustomizationFormText = "Distribution Rule";
		this.simpleLabelItem5.Location = new System.Drawing.Point(30, 167);
		this.simpleLabelItem5.Name = "simpleLabelItem5";
		this.simpleLabelItem5.Size = new System.Drawing.Size(132, 26);
		this.simpleLabelItem5.Text = "Distribution Rule";
		this.simpleLabelItem5.TextSize = new System.Drawing.Size(120, 13);
		this.layoutControlItem18.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem18.Control = this.simpleButton2;
		this.layoutControlItem18.CustomizationFormText = "layoutControlItem18";
		this.layoutControlItem18.EditControl = null;
		this.layoutControlItem18.EditValue = "layoutControlItem18";
		this.layoutControlItem18.FieldName = null;
		this.layoutControlItem18.IsSearchControl = false;
		this.layoutControlItem18.Location = new System.Drawing.Point(162, 167);
		this.layoutControlItem18.Name = "layoutControlItem18";
		this.layoutControlItem18.ReadOnly = false;
		this.layoutControlItem18.SettingInVisible = false;
		this.layoutControlItem18.SettingReadonly = false;
		this.layoutControlItem18.Size = new System.Drawing.Size(160, 26);
		this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.TextVisible = false;
		this.emptySpaceItem30.AllowHotTrack = false;
		this.emptySpaceItem30.CustomizationFormText = "emptySpaceItem30";
		this.emptySpaceItem30.Location = new System.Drawing.Point(0, 399);
		this.emptySpaceItem30.Name = "emptySpaceItem30";
		this.emptySpaceItem30.Size = new System.Drawing.Size(856, 33);
		this.emptySpaceItem30.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem31.AllowHotTrack = false;
		this.emptySpaceItem31.CustomizationFormText = "emptySpaceItem31";
		this.emptySpaceItem31.Location = new System.Drawing.Point(0, 217);
		this.emptySpaceItem31.Name = "emptySpaceItem31";
		this.emptySpaceItem31.Size = new System.Drawing.Size(856, 182);
		this.emptySpaceItem31.TextSize = new System.Drawing.Size(0, 0);
		this.Tab_Pricing.Controls.Add(this.layoutControl12);
		this.Tab_Pricing.EditControl = null;
		this.Tab_Pricing.EditValue = "Pricing";
		this.Tab_Pricing.FieldName = "Tab_Pricing";
		this.Tab_Pricing.IsSearchControl = false;
		this.Tab_Pricing.Name = "Tab_Pricing";
		this.Tab_Pricing.ReadOnly = false;
		this.Tab_Pricing.SettingInVisible = false;
		this.Tab_Pricing.SettingReadonly = false;
		this.Tab_Pricing.Size = new System.Drawing.Size(876, 452);
		this.Tab_Pricing.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Pricing.Text = "Pricing";
		this.layoutControl12.Controls.Add(this.label6);
		this.layoutControl12.Controls.Add(this.label5);
		this.layoutControl12.Controls.Add(this.SIPLSeting);
		this.layoutControl12.Controls.Add(this.SIPLDoc);
		this.layoutControl12.Controls.Add(this.SIPLReport);
		this.layoutControl12.Controls.Add(this.IsUpdNstdB);
		this.layoutControl12.Controls.Add(this.ClnZeroPln);
		this.layoutControl12.Controls.Add(this.ApyBsActPL);
		this.layoutControl12.Controls.Add(this.ApyBsActSP);
		this.layoutControl12.Controls.Add(this.ApyBsActPV);
		this.layoutControl12.Controls.Add(this.PriceProcM);
		this.layoutControl12.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl12.Location = new System.Drawing.Point(0, 0);
		this.layoutControl12.Name = "layoutControl12";
		this.layoutControl12.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(805, 346, 250, 350);
		this.layoutControl12.Root = this.layoutControlGroup11;
		this.layoutControl12.Size = new System.Drawing.Size(876, 452);
		this.layoutControl12.TabIndex = 0;
		this.layoutControl12.Text = "layoutControl12";
		this.label6.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label6.Appearance.Options.UseFont = true;
		this.label6.EditControl = null;
		this.label6.EditValue = "Display Inactive Price Lists In";
		this.label6.FieldName = null;
		this.label6.HideWhenEmpty = false;
		this.label6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label6.Location = new System.Drawing.Point(468, 12);
		this.label6.Name = "label6";
		this.label6.ReadOnly = false;
		this.label6.SettingInVisible = false;
		this.label6.SettingReadonly = false;
		this.label6.ShowUnderLine = true;
		this.label6.Size = new System.Drawing.Size(396, 21);
		this.label6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label6.TabIndex = 15;
		this.label6.Utext = "Display Inactive Price Lists In";
		this.label5.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Appearance.Options.UseFont = true;
		this.label5.EditControl = null;
		this.label5.EditValue = "Display Zero Price in Documents if Price Source is Based on Inactive Price Lists";
		this.label5.FieldName = null;
		this.label5.HideWhenEmpty = false;
		this.label5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label5.Location = new System.Drawing.Point(12, 12);
		this.label5.Name = "label5";
		this.label5.ReadOnly = false;
		this.label5.SettingInVisible = false;
		this.label5.SettingReadonly = false;
		this.label5.ShowUnderLine = true;
		this.label5.Size = new System.Drawing.Size(452, 21);
		this.label5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label5.TabIndex = 14;
		this.label5.Utext = "Display Zero Price in Documents if Price Source is Based on Inactive Price Lists";
		this.SIPLSeting.EditControl = null;
		this.SIPLSeting.FieldName = null;
		this.SIPLSeting.IsSearchControl = false;
		this.SIPLSeting.Location = new System.Drawing.Point(468, 85);
		this.SIPLSeting.Name = "SIPLSeting";
		this.SIPLSeting.Properties.Caption = "Settings";
		this.SIPLSeting.Properties.DisplayValueChecked = "Y";
		this.SIPLSeting.Properties.DisplayValueUnchecked = "N";
		this.SIPLSeting.Properties.ValueChecked = "Y";
		this.SIPLSeting.Properties.ValueGrayed = "";
		this.SIPLSeting.Properties.ValueUnchecked = "N";
		this.SIPLSeting.SettingInVisible = false;
		this.SIPLSeting.SettingReadonly = false;
		this.SIPLSeting.Size = new System.Drawing.Size(396, 20);
		this.SIPLSeting.StyleController = this.layoutControl12;
		this.SIPLSeting.TabIndex = 12;
		this.SIPLSeting.CheckedChanged += new System.EventHandler(this.SIPLSeting_CheckedChanged);
		this.SIPLDoc.EditControl = null;
		this.SIPLDoc.FieldName = null;
		this.SIPLDoc.IsSearchControl = false;
		this.SIPLDoc.Location = new System.Drawing.Point(468, 61);
		this.SIPLDoc.Name = "SIPLDoc";
		this.SIPLDoc.Properties.Caption = "Sales, Purchasing and Inventor&y Documents";
		this.SIPLDoc.Properties.DisplayValueChecked = "Y";
		this.SIPLDoc.Properties.DisplayValueUnchecked = "N";
		this.SIPLDoc.Properties.ValueChecked = "Y";
		this.SIPLDoc.Properties.ValueGrayed = "";
		this.SIPLDoc.Properties.ValueUnchecked = "N";
		this.SIPLDoc.SettingInVisible = false;
		this.SIPLDoc.SettingReadonly = false;
		this.SIPLDoc.Size = new System.Drawing.Size(396, 20);
		this.SIPLDoc.StyleController = this.layoutControl12;
		this.SIPLDoc.TabIndex = 11;
		this.SIPLDoc.CheckedChanged += new System.EventHandler(this.SIPLDoc_CheckedChanged);
		this.SIPLReport.EditControl = null;
		this.SIPLReport.FieldName = null;
		this.SIPLReport.IsSearchControl = false;
		this.SIPLReport.Location = new System.Drawing.Point(468, 37);
		this.SIPLReport.Name = "SIPLReport";
		this.SIPLReport.Properties.Caption = "Reports";
		this.SIPLReport.Properties.DisplayValueChecked = "Y";
		this.SIPLReport.Properties.DisplayValueUnchecked = "N";
		this.SIPLReport.Properties.ValueChecked = "Y";
		this.SIPLReport.Properties.ValueGrayed = "";
		this.SIPLReport.Properties.ValueUnchecked = "N";
		this.SIPLReport.SettingInVisible = false;
		this.SIPLReport.SettingReadonly = false;
		this.SIPLReport.Size = new System.Drawing.Size(396, 20);
		this.SIPLReport.StyleController = this.layoutControl12;
		this.SIPLReport.TabIndex = 10;
		this.SIPLReport.CheckedChanged += new System.EventHandler(this.SIPLReport_CheckedChanged);
		this.IsUpdNstdB.EditControl = null;
		this.IsUpdNstdB.FieldName = null;
		this.IsUpdNstdB.IsSearchControl = false;
		this.IsUpdNstdB.Location = new System.Drawing.Point(25, 198);
		this.IsUpdNstdB.Name = "IsUpdNstdB";
		this.IsUpdNstdB.Properties.Caption = "Remove or Update UoM Prices Based on Last Purchase or Evaluated Price Lists";
		this.IsUpdNstdB.Properties.DisplayValueChecked = "Y";
		this.IsUpdNstdB.Properties.DisplayValueUnchecked = "N";
		this.IsUpdNstdB.Properties.ValueChecked = "Y";
		this.IsUpdNstdB.Properties.ValueGrayed = "";
		this.IsUpdNstdB.Properties.ValueUnchecked = "N";
		this.IsUpdNstdB.SettingInVisible = false;
		this.IsUpdNstdB.SettingReadonly = false;
		this.IsUpdNstdB.Size = new System.Drawing.Size(439, 20);
		this.IsUpdNstdB.StyleController = this.layoutControl12;
		this.IsUpdNstdB.TabIndex = 9;
		this.IsUpdNstdB.CheckedChanged += new System.EventHandler(this.IsUpdNstdB_CheckedChanged);
		this.ClnZeroPln.EditControl = null;
		this.ClnZeroPln.FieldName = null;
		this.ClnZeroPln.IsSearchControl = false;
		this.ClnZeroPln.Location = new System.Drawing.Point(12, 150);
		this.ClnZeroPln.Name = "ClnZeroPln";
		this.ClnZeroPln.Properties.Caption = "Remove Unpriced Items from Price List in Database";
		this.ClnZeroPln.Properties.DisplayValueChecked = "Y";
		this.ClnZeroPln.Properties.DisplayValueUnchecked = "N";
		this.ClnZeroPln.Properties.ValueChecked = "Y";
		this.ClnZeroPln.Properties.ValueGrayed = "";
		this.ClnZeroPln.Properties.ValueUnchecked = "N";
		this.ClnZeroPln.SettingInVisible = false;
		this.ClnZeroPln.SettingReadonly = false;
		this.ClnZeroPln.Size = new System.Drawing.Size(452, 20);
		this.ClnZeroPln.StyleController = this.layoutControl12;
		this.ClnZeroPln.TabIndex = 7;
		this.ClnZeroPln.CheckedChanged += new System.EventHandler(this.ClnZeroPln_CheckedChanged);
		this.ApyBsActPL.EditControl = null;
		this.ApyBsActPL.FieldName = null;
		this.ApyBsActPL.IsSearchControl = false;
		this.ApyBsActPL.Location = new System.Drawing.Point(12, 85);
		this.ApyBsActPL.Name = "ApyBsActPL";
		this.ApyBsActPL.Properties.Caption = "Price Lists";
		this.ApyBsActPL.Properties.DisplayValueChecked = "Y";
		this.ApyBsActPL.Properties.DisplayValueUnchecked = "N";
		this.ApyBsActPL.Properties.ValueChecked = "Y";
		this.ApyBsActPL.Properties.ValueGrayed = "";
		this.ApyBsActPL.Properties.ValueUnchecked = "N";
		this.ApyBsActPL.SettingInVisible = false;
		this.ApyBsActPL.SettingReadonly = false;
		this.ApyBsActPL.Size = new System.Drawing.Size(452, 20);
		this.ApyBsActPL.StyleController = this.layoutControl12;
		this.ApyBsActPL.TabIndex = 6;
		this.ApyBsActPL.CheckedChanged += new System.EventHandler(this.ApyBsActPL_CheckedChanged);
		this.ApyBsActSP.EditControl = null;
		this.ApyBsActSP.FieldName = null;
		this.ApyBsActSP.IsSearchControl = false;
		this.ApyBsActSP.Location = new System.Drawing.Point(12, 61);
		this.ApyBsActSP.Name = "ApyBsActSP";
		this.ApyBsActSP.Properties.Caption = "Special Prices for Business Partners";
		this.ApyBsActSP.Properties.DisplayValueChecked = "Y";
		this.ApyBsActSP.Properties.DisplayValueUnchecked = "N";
		this.ApyBsActSP.Properties.ValueChecked = "Y";
		this.ApyBsActSP.Properties.ValueGrayed = "";
		this.ApyBsActSP.Properties.ValueUnchecked = "N";
		this.ApyBsActSP.SettingInVisible = false;
		this.ApyBsActSP.SettingReadonly = false;
		this.ApyBsActSP.Size = new System.Drawing.Size(452, 20);
		this.ApyBsActSP.StyleController = this.layoutControl12;
		this.ApyBsActSP.TabIndex = 5;
		this.ApyBsActSP.CheckedChanged += new System.EventHandler(this.checkEdit48_CheckedChanged);
		this.ApyBsActPV.EditControl = null;
		this.ApyBsActPV.FieldName = null;
		this.ApyBsActPV.IsSearchControl = false;
		this.ApyBsActPV.Location = new System.Drawing.Point(12, 37);
		this.ApyBsActPV.Name = "ApyBsActPV";
		this.ApyBsActPV.Properties.Caption = "Period and Volume Discounts";
		this.ApyBsActPV.Properties.DisplayValueChecked = "Y";
		this.ApyBsActPV.Properties.DisplayValueUnchecked = "N";
		this.ApyBsActPV.Properties.ValueChecked = "Y";
		this.ApyBsActPV.Properties.ValueGrayed = "";
		this.ApyBsActPV.Properties.ValueUnchecked = "N";
		this.ApyBsActPV.SettingInVisible = false;
		this.ApyBsActPV.SettingReadonly = false;
		this.ApyBsActPV.Size = new System.Drawing.Size(452, 20);
		this.ApyBsActPV.StyleController = this.layoutControl12;
		this.ApyBsActPV.TabIndex = 4;
		this.ApyBsActPV.CheckedChanged += new System.EventHandler(this.ApyBsActPV_CheckedChanged);
		this.PriceProcM.EditControl = null;
		this.PriceProcM.FieldName = null;
		this.PriceProcM.IsSearchControl = false;
		this.PriceProcM.Location = new System.Drawing.Point(193, 174);
		this.PriceProcM.Name = "PriceProcM";
		this.PriceProcM.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PriceProcM.Properties.IsSearchControl = false;
		this.PriceProcM.Properties.NullText = "";
		this.PriceProcM.SettingInVisible = false;
		this.PriceProcM.SettingReadonly = false;
		this.PriceProcM.Size = new System.Drawing.Size(271, 20);
		this.PriceProcM.StyleController = this.layoutControl12;
		this.PriceProcM.TabIndex = 8;
		this.PriceProcM.EditValueChanged += new System.EventHandler(this.PriceProcM_EditValueChanged);
		this.layoutControlGroup11.CustomizationFormText = "Root";
		this.layoutControlGroup11.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup11.GroupBordersVisible = false;
		this.layoutControlGroup11.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.emptySpaceItem75, this.emptySpaceItem76, this.layoutControlItem125, this.layoutControlItem126, this.layoutControlItem127, this.emptySpaceItem77, this.layoutControlItem128, this.emptySpaceItem78, this.layoutControlItem129, this.layoutControlItem130,
			this.layoutControlItem131, this.layoutControlItem132, this.ck_Setting, this.layoutControlItem150, this.layoutControlItem151
		});
		this.layoutControlGroup11.Name = "Root";
		this.layoutControlGroup11.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup11.TextVisible = false;
		this.emptySpaceItem75.AllowHotTrack = false;
		this.emptySpaceItem75.CustomizationFormText = "emptySpaceItem75";
		this.emptySpaceItem75.Location = new System.Drawing.Point(13, 210);
		this.emptySpaceItem75.Name = "emptySpaceItem75";
		this.emptySpaceItem75.Size = new System.Drawing.Size(443, 222);
		this.emptySpaceItem75.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem76.AllowHotTrack = false;
		this.emptySpaceItem76.CustomizationFormText = "emptySpaceItem76";
		this.emptySpaceItem76.Location = new System.Drawing.Point(456, 97);
		this.emptySpaceItem76.Name = "emptySpaceItem76";
		this.emptySpaceItem76.Size = new System.Drawing.Size(400, 335);
		this.emptySpaceItem76.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem125.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem125.Control = this.ApyBsActPV;
		this.layoutControlItem125.CustomizationFormText = "layoutControlItem125";
		this.layoutControlItem125.EditControl = null;
		this.layoutControlItem125.EditValue = "layoutControlItem125";
		this.layoutControlItem125.FieldName = null;
		this.layoutControlItem125.IsSearchControl = false;
		this.layoutControlItem125.Location = new System.Drawing.Point(0, 25);
		this.layoutControlItem125.Name = "layoutControlItem125";
		this.layoutControlItem125.ReadOnly = false;
		this.layoutControlItem125.SettingInVisible = false;
		this.layoutControlItem125.SettingReadonly = false;
		this.layoutControlItem125.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem125.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem125.TextVisible = false;
		this.layoutControlItem126.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem126.Control = this.ApyBsActSP;
		this.layoutControlItem126.CustomizationFormText = "layoutControlItem126";
		this.layoutControlItem126.EditControl = null;
		this.layoutControlItem126.EditValue = "layoutControlItem126";
		this.layoutControlItem126.FieldName = null;
		this.layoutControlItem126.IsSearchControl = false;
		this.layoutControlItem126.Location = new System.Drawing.Point(0, 49);
		this.layoutControlItem126.Name = "layoutControlItem126";
		this.layoutControlItem126.ReadOnly = false;
		this.layoutControlItem126.SettingInVisible = false;
		this.layoutControlItem126.SettingReadonly = false;
		this.layoutControlItem126.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem126.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem126.TextVisible = false;
		this.layoutControlItem127.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem127.Control = this.ApyBsActPL;
		this.layoutControlItem127.CustomizationFormText = "layoutControlItem127";
		this.layoutControlItem127.EditControl = null;
		this.layoutControlItem127.EditValue = "layoutControlItem127";
		this.layoutControlItem127.FieldName = null;
		this.layoutControlItem127.IsSearchControl = false;
		this.layoutControlItem127.Location = new System.Drawing.Point(0, 73);
		this.layoutControlItem127.Name = "layoutControlItem127";
		this.layoutControlItem127.ReadOnly = false;
		this.layoutControlItem127.SettingInVisible = false;
		this.layoutControlItem127.SettingReadonly = false;
		this.layoutControlItem127.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem127.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem127.TextVisible = false;
		this.emptySpaceItem77.AllowHotTrack = false;
		this.emptySpaceItem77.CustomizationFormText = "emptySpaceItem77";
		this.emptySpaceItem77.Location = new System.Drawing.Point(0, 97);
		this.emptySpaceItem77.Name = "emptySpaceItem77";
		this.emptySpaceItem77.Size = new System.Drawing.Size(456, 41);
		this.emptySpaceItem77.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem128.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem128.Control = this.ClnZeroPln;
		this.layoutControlItem128.CustomizationFormText = "layoutControlItem128";
		this.layoutControlItem128.EditControl = null;
		this.layoutControlItem128.EditValue = "layoutControlItem128";
		this.layoutControlItem128.FieldName = null;
		this.layoutControlItem128.IsSearchControl = false;
		this.layoutControlItem128.Location = new System.Drawing.Point(0, 138);
		this.layoutControlItem128.Name = "layoutControlItem128";
		this.layoutControlItem128.ReadOnly = false;
		this.layoutControlItem128.SettingInVisible = false;
		this.layoutControlItem128.SettingReadonly = false;
		this.layoutControlItem128.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem128.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem128.TextVisible = false;
		this.emptySpaceItem78.AllowHotTrack = false;
		this.emptySpaceItem78.CustomizationFormText = "emptySpaceItem78";
		this.emptySpaceItem78.Location = new System.Drawing.Point(0, 186);
		this.emptySpaceItem78.Name = "emptySpaceItem78";
		this.emptySpaceItem78.Size = new System.Drawing.Size(13, 246);
		this.emptySpaceItem78.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem129.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem129.Control = this.PriceProcM;
		this.layoutControlItem129.CustomizationFormText = "layoutControlItem129";
		this.layoutControlItem129.EditControl = null;
		this.layoutControlItem129.EditValue = "When UoM Conversion Rules Change";
		this.layoutControlItem129.FieldName = null;
		this.layoutControlItem129.IsSearchControl = false;
		this.layoutControlItem129.Location = new System.Drawing.Point(0, 162);
		this.layoutControlItem129.Name = "layoutControlItem129";
		this.layoutControlItem129.ReadOnly = false;
		this.layoutControlItem129.SettingInVisible = false;
		this.layoutControlItem129.SettingReadonly = false;
		this.layoutControlItem129.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem129.Text = "When UoM Conversion Rules Change";
		this.layoutControlItem129.TextSize = new System.Drawing.Size(178, 13);
		this.layoutControlItem130.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem130.Control = this.IsUpdNstdB;
		this.layoutControlItem130.CustomizationFormText = "layoutControlItem130";
		this.layoutControlItem130.EditControl = null;
		this.layoutControlItem130.EditValue = "layoutControlItem130";
		this.layoutControlItem130.FieldName = null;
		this.layoutControlItem130.IsSearchControl = false;
		this.layoutControlItem130.Location = new System.Drawing.Point(13, 186);
		this.layoutControlItem130.Name = "layoutControlItem130";
		this.layoutControlItem130.ReadOnly = false;
		this.layoutControlItem130.SettingInVisible = false;
		this.layoutControlItem130.SettingReadonly = false;
		this.layoutControlItem130.Size = new System.Drawing.Size(443, 24);
		this.layoutControlItem130.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem130.TextVisible = false;
		this.layoutControlItem131.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem131.Control = this.SIPLReport;
		this.layoutControlItem131.CustomizationFormText = "layoutControlItem131";
		this.layoutControlItem131.EditControl = null;
		this.layoutControlItem131.EditValue = "layoutControlItem131";
		this.layoutControlItem131.FieldName = null;
		this.layoutControlItem131.IsSearchControl = false;
		this.layoutControlItem131.Location = new System.Drawing.Point(456, 25);
		this.layoutControlItem131.Name = "layoutControlItem131";
		this.layoutControlItem131.ReadOnly = false;
		this.layoutControlItem131.SettingInVisible = false;
		this.layoutControlItem131.SettingReadonly = false;
		this.layoutControlItem131.Size = new System.Drawing.Size(400, 24);
		this.layoutControlItem131.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem131.TextVisible = false;
		this.layoutControlItem132.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem132.Control = this.SIPLDoc;
		this.layoutControlItem132.CustomizationFormText = "layoutControlItem132";
		this.layoutControlItem132.EditControl = null;
		this.layoutControlItem132.EditValue = "layoutControlItem132";
		this.layoutControlItem132.FieldName = null;
		this.layoutControlItem132.IsSearchControl = false;
		this.layoutControlItem132.Location = new System.Drawing.Point(456, 49);
		this.layoutControlItem132.Name = "layoutControlItem132";
		this.layoutControlItem132.ReadOnly = false;
		this.layoutControlItem132.SettingInVisible = false;
		this.layoutControlItem132.SettingReadonly = false;
		this.layoutControlItem132.Size = new System.Drawing.Size(400, 24);
		this.layoutControlItem132.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem132.TextVisible = false;
		this.ck_Setting.BackColor = System.Drawing.Color.Empty;
		this.ck_Setting.Control = this.SIPLSeting;
		this.ck_Setting.CustomizationFormText = "layoutControlItem133";
		this.ck_Setting.EditControl = null;
		this.ck_Setting.EditValue = "ck_Setting";
		this.ck_Setting.FieldName = null;
		this.ck_Setting.IsSearchControl = false;
		this.ck_Setting.Location = new System.Drawing.Point(456, 73);
		this.ck_Setting.Name = "ck_Setting";
		this.ck_Setting.ReadOnly = false;
		this.ck_Setting.SettingInVisible = false;
		this.ck_Setting.SettingReadonly = false;
		this.ck_Setting.Size = new System.Drawing.Size(400, 24);
		this.ck_Setting.TextSize = new System.Drawing.Size(0, 0);
		this.ck_Setting.TextVisible = false;
		this.layoutControlItem150.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem150.Control = this.label5;
		this.layoutControlItem150.CustomizationFormText = "layoutControlItem150";
		this.layoutControlItem150.EditControl = null;
		this.layoutControlItem150.EditValue = "layoutControlItem150";
		this.layoutControlItem150.FieldName = null;
		this.layoutControlItem150.IsSearchControl = false;
		this.layoutControlItem150.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem150.Name = "layoutControlItem150";
		this.layoutControlItem150.ReadOnly = false;
		this.layoutControlItem150.SettingInVisible = false;
		this.layoutControlItem150.SettingReadonly = false;
		this.layoutControlItem150.Size = new System.Drawing.Size(456, 25);
		this.layoutControlItem150.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem150.TextVisible = false;
		this.layoutControlItem151.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem151.Control = this.label6;
		this.layoutControlItem151.CustomizationFormText = "layoutControlItem151";
		this.layoutControlItem151.EditControl = null;
		this.layoutControlItem151.EditValue = "layoutControlItem151";
		this.layoutControlItem151.FieldName = null;
		this.layoutControlItem151.IsSearchControl = false;
		this.layoutControlItem151.Location = new System.Drawing.Point(456, 0);
		this.layoutControlItem151.Name = "layoutControlItem151";
		this.layoutControlItem151.ReadOnly = false;
		this.layoutControlItem151.SettingInVisible = false;
		this.layoutControlItem151.SettingReadonly = false;
		this.layoutControlItem151.Size = new System.Drawing.Size(400, 25);
		this.layoutControlItem151.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem151.TextVisible = false;
		this.Tab_HideFunctions.Controls.Add(this.layoutControl13);
		this.Tab_HideFunctions.EditControl = null;
		this.Tab_HideFunctions.EditValue = "Hide Functions";
		this.Tab_HideFunctions.FieldName = "Tab_HideFunctions";
		this.Tab_HideFunctions.IsSearchControl = false;
		this.Tab_HideFunctions.Name = "Tab_HideFunctions";
		this.Tab_HideFunctions.PageVisible = false;
		this.Tab_HideFunctions.ReadOnly = false;
		this.Tab_HideFunctions.SettingInVisible = false;
		this.Tab_HideFunctions.SettingReadonly = false;
		this.Tab_HideFunctions.Size = new System.Drawing.Size(876, 452);
		this.Tab_HideFunctions.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_HideFunctions.Text = "Hide Functions";
		this.layoutControl13.Controls.Add(this.ck_UnitOfMeasure);
		this.layoutControl13.Controls.Add(this.ck_MRP);
		this.layoutControl13.Controls.Add(this.ck_);
		this.layoutControl13.Controls.Add(this.ck_SerialNum);
		this.layoutControl13.Controls.Add(this.ck_CostCounting);
		this.layoutControl13.Controls.Add(this.ck_DunningWizard);
		this.layoutControl13.Controls.Add(this.ck_PaymentWizard);
		this.layoutControl13.Controls.Add(this.ck_Budget);
		this.layoutControl13.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl13.Location = new System.Drawing.Point(0, 0);
		this.layoutControl13.Name = "layoutControl13";
		this.layoutControl13.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(564, 253, 250, 350);
		this.layoutControl13.Root = this.layoutControlGroup12;
		this.layoutControl13.Size = new System.Drawing.Size(876, 452);
		this.layoutControl13.TabIndex = 0;
		this.layoutControl13.Text = "layoutControl13";
		this.ck_UnitOfMeasure.EditControl = null;
		this.ck_UnitOfMeasure.FieldName = null;
		this.ck_UnitOfMeasure.IsSearchControl = false;
		this.ck_UnitOfMeasure.Location = new System.Drawing.Point(12, 197);
		this.ck_UnitOfMeasure.Name = "ck_UnitOfMeasure";
		this.ck_UnitOfMeasure.Properties.Caption = "Units of Measure";
		this.ck_UnitOfMeasure.Properties.DisplayValueChecked = "Y";
		this.ck_UnitOfMeasure.Properties.DisplayValueUnchecked = "N";
		this.ck_UnitOfMeasure.Properties.ValueChecked = "Y";
		this.ck_UnitOfMeasure.Properties.ValueGrayed = "";
		this.ck_UnitOfMeasure.Properties.ValueUnchecked = "N";
		this.ck_UnitOfMeasure.SettingInVisible = false;
		this.ck_UnitOfMeasure.SettingReadonly = false;
		this.ck_UnitOfMeasure.Size = new System.Drawing.Size(852, 20);
		this.ck_UnitOfMeasure.StyleController = this.layoutControl13;
		this.ck_UnitOfMeasure.TabIndex = 11;
		this.ck_UnitOfMeasure.CheckedChanged += new System.EventHandler(this.ck_UnitOfMeasure_CheckedChanged);
		this.ck_MRP.EditControl = null;
		this.ck_MRP.FieldName = null;
		this.ck_MRP.IsSearchControl = false;
		this.ck_MRP.Location = new System.Drawing.Point(12, 173);
		this.ck_MRP.Name = "ck_MRP";
		this.ck_MRP.Properties.Caption = "MRP";
		this.ck_MRP.Properties.DisplayValueChecked = "Y";
		this.ck_MRP.Properties.DisplayValueUnchecked = "N";
		this.ck_MRP.Properties.ValueChecked = "Y";
		this.ck_MRP.Properties.ValueGrayed = "";
		this.ck_MRP.Properties.ValueUnchecked = "N";
		this.ck_MRP.SettingInVisible = false;
		this.ck_MRP.SettingReadonly = false;
		this.ck_MRP.Size = new System.Drawing.Size(852, 20);
		this.ck_MRP.StyleController = this.layoutControl13;
		this.ck_MRP.TabIndex = 10;
		this.ck_MRP.CheckedChanged += new System.EventHandler(this.ck_MRP_CheckedChanged);
		this.ck_.EditControl = null;
		this.ck_.FieldName = null;
		this.ck_.IsSearchControl = false;
		this.ck_.Location = new System.Drawing.Point(12, 149);
		this.ck_.Name = "ck_";
		this.ck_.Properties.Caption = "Production";
		this.ck_.Properties.DisplayValueChecked = "Y";
		this.ck_.Properties.DisplayValueUnchecked = "N";
		this.ck_.Properties.ValueChecked = "Y";
		this.ck_.Properties.ValueGrayed = "";
		this.ck_.Properties.ValueUnchecked = "N";
		this.ck_.SettingInVisible = false;
		this.ck_.SettingReadonly = false;
		this.ck_.Size = new System.Drawing.Size(852, 20);
		this.ck_.StyleController = this.layoutControl13;
		this.ck_.TabIndex = 9;
		this.ck_.CheckedChanged += new System.EventHandler(this.ck__CheckedChanged);
		this.ck_SerialNum.EditControl = null;
		this.ck_SerialNum.FieldName = null;
		this.ck_SerialNum.IsSearchControl = false;
		this.ck_SerialNum.Location = new System.Drawing.Point(12, 125);
		this.ck_SerialNum.Name = "ck_SerialNum";
		this.ck_SerialNum.Properties.Caption = "Serial Numbers and Batches";
		this.ck_SerialNum.Properties.DisplayValueChecked = "Y";
		this.ck_SerialNum.Properties.DisplayValueUnchecked = "N";
		this.ck_SerialNum.Properties.ValueChecked = "Y";
		this.ck_SerialNum.Properties.ValueGrayed = "";
		this.ck_SerialNum.Properties.ValueUnchecked = "N";
		this.ck_SerialNum.SettingInVisible = false;
		this.ck_SerialNum.SettingReadonly = false;
		this.ck_SerialNum.Size = new System.Drawing.Size(852, 20);
		this.ck_SerialNum.StyleController = this.layoutControl13;
		this.ck_SerialNum.TabIndex = 8;
		this.ck_SerialNum.CheckedChanged += new System.EventHandler(this.ck_SerialNum_CheckedChanged);
		this.ck_CostCounting.EditControl = null;
		this.ck_CostCounting.FieldName = null;
		this.ck_CostCounting.IsSearchControl = false;
		this.ck_CostCounting.Location = new System.Drawing.Point(12, 101);
		this.ck_CostCounting.Name = "ck_CostCounting";
		this.ck_CostCounting.Properties.Caption = "Cost Accounting";
		this.ck_CostCounting.Properties.DisplayValueChecked = "Y";
		this.ck_CostCounting.Properties.DisplayValueUnchecked = "N";
		this.ck_CostCounting.Properties.ValueChecked = "Y";
		this.ck_CostCounting.Properties.ValueGrayed = "";
		this.ck_CostCounting.Properties.ValueUnchecked = "N";
		this.ck_CostCounting.SettingInVisible = false;
		this.ck_CostCounting.SettingReadonly = false;
		this.ck_CostCounting.Size = new System.Drawing.Size(852, 20);
		this.ck_CostCounting.StyleController = this.layoutControl13;
		this.ck_CostCounting.TabIndex = 7;
		this.ck_CostCounting.CheckedChanged += new System.EventHandler(this.ck_CostCounting_CheckedChanged);
		this.ck_DunningWizard.EditControl = null;
		this.ck_DunningWizard.FieldName = null;
		this.ck_DunningWizard.IsSearchControl = false;
		this.ck_DunningWizard.Location = new System.Drawing.Point(12, 77);
		this.ck_DunningWizard.Name = "ck_DunningWizard";
		this.ck_DunningWizard.Properties.Caption = "Dunning Wizard";
		this.ck_DunningWizard.Properties.DisplayValueChecked = "Y";
		this.ck_DunningWizard.Properties.DisplayValueUnchecked = "N";
		this.ck_DunningWizard.Properties.ValueChecked = "Y";
		this.ck_DunningWizard.Properties.ValueGrayed = "";
		this.ck_DunningWizard.Properties.ValueUnchecked = "N";
		this.ck_DunningWizard.SettingInVisible = false;
		this.ck_DunningWizard.SettingReadonly = false;
		this.ck_DunningWizard.Size = new System.Drawing.Size(852, 20);
		this.ck_DunningWizard.StyleController = this.layoutControl13;
		this.ck_DunningWizard.TabIndex = 6;
		this.ck_DunningWizard.CheckedChanged += new System.EventHandler(this.ck_DunningWizard_CheckedChanged);
		this.ck_PaymentWizard.EditControl = null;
		this.ck_PaymentWizard.FieldName = null;
		this.ck_PaymentWizard.IsSearchControl = false;
		this.ck_PaymentWizard.Location = new System.Drawing.Point(12, 53);
		this.ck_PaymentWizard.Name = "ck_PaymentWizard";
		this.ck_PaymentWizard.Properties.Caption = "Pa&yment Wizard";
		this.ck_PaymentWizard.Properties.DisplayValueChecked = "Y";
		this.ck_PaymentWizard.Properties.DisplayValueUnchecked = "N";
		this.ck_PaymentWizard.Properties.ValueChecked = "Y";
		this.ck_PaymentWizard.Properties.ValueGrayed = "";
		this.ck_PaymentWizard.Properties.ValueUnchecked = "N";
		this.ck_PaymentWizard.SettingInVisible = false;
		this.ck_PaymentWizard.SettingReadonly = false;
		this.ck_PaymentWizard.Size = new System.Drawing.Size(852, 20);
		this.ck_PaymentWizard.StyleController = this.layoutControl13;
		this.ck_PaymentWizard.TabIndex = 5;
		this.ck_PaymentWizard.CheckedChanged += new System.EventHandler(this.ck_PaymentWizard_CheckedChanged);
		this.ck_Budget.EditControl = null;
		this.ck_Budget.FieldName = null;
		this.ck_Budget.IsSearchControl = false;
		this.ck_Budget.Location = new System.Drawing.Point(12, 29);
		this.ck_Budget.Name = "ck_Budget";
		this.ck_Budget.Properties.Caption = "Budget";
		this.ck_Budget.Properties.DisplayValueChecked = "Y";
		this.ck_Budget.Properties.DisplayValueUnchecked = "N";
		this.ck_Budget.Properties.ValueChecked = "Y";
		this.ck_Budget.Properties.ValueGrayed = "";
		this.ck_Budget.Properties.ValueUnchecked = "N";
		this.ck_Budget.SettingInVisible = false;
		this.ck_Budget.SettingReadonly = false;
		this.ck_Budget.Size = new System.Drawing.Size(852, 20);
		this.ck_Budget.StyleController = this.layoutControl13;
		this.ck_Budget.TabIndex = 4;
		this.ck_Budget.CheckedChanged += new System.EventHandler(this.checkEdit55_CheckedChanged);
		this.layoutControlGroup12.CustomizationFormText = "Root";
		this.layoutControlGroup12.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup12.GroupBordersVisible = false;
		this.layoutControlGroup12.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.simpleLabelItem23, this.layoutControlItem134, this.layoutControlItem135, this.layoutControlItem136, this.layoutControlItem137, this.layoutControlItem138, this.layoutControlItem139, this.layoutControlItem140, this.layoutControlItem149 });
		this.layoutControlGroup12.Name = "Root";
		this.layoutControlGroup12.Size = new System.Drawing.Size(876, 452);
		this.layoutControlGroup12.TextVisible = false;
		this.simpleLabelItem23.AllowHotTrack = false;
		this.simpleLabelItem23.CustomizationFormText = "LabelsimpleLabelItem23";
		this.simpleLabelItem23.Location = new System.Drawing.Point(0, 0);
		this.simpleLabelItem23.Name = "simpleLabelItem23";
		this.simpleLabelItem23.Size = new System.Drawing.Size(856, 17);
		this.simpleLabelItem23.Text = "If your company does not use one or more of the following functions, you can hide them by selecting the respective option(s):";
		this.simpleLabelItem23.TextSize = new System.Drawing.Size(606, 13);
		this.layoutControlItem134.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem134.Control = this.ck_Budget;
		this.layoutControlItem134.CustomizationFormText = "layoutControlItem134";
		this.layoutControlItem134.EditControl = null;
		this.layoutControlItem134.EditValue = "layoutControlItem134";
		this.layoutControlItem134.FieldName = null;
		this.layoutControlItem134.IsSearchControl = false;
		this.layoutControlItem134.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem134.Name = "layoutControlItem134";
		this.layoutControlItem134.ReadOnly = false;
		this.layoutControlItem134.SettingInVisible = false;
		this.layoutControlItem134.SettingReadonly = false;
		this.layoutControlItem134.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem134.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem134.TextVisible = false;
		this.layoutControlItem135.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem135.Control = this.ck_PaymentWizard;
		this.layoutControlItem135.CustomizationFormText = "layoutControlItem135";
		this.layoutControlItem135.EditControl = null;
		this.layoutControlItem135.EditValue = "layoutControlItem135";
		this.layoutControlItem135.FieldName = null;
		this.layoutControlItem135.IsSearchControl = false;
		this.layoutControlItem135.Location = new System.Drawing.Point(0, 41);
		this.layoutControlItem135.Name = "layoutControlItem135";
		this.layoutControlItem135.ReadOnly = false;
		this.layoutControlItem135.SettingInVisible = false;
		this.layoutControlItem135.SettingReadonly = false;
		this.layoutControlItem135.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem135.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem135.TextVisible = false;
		this.layoutControlItem136.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem136.Control = this.ck_DunningWizard;
		this.layoutControlItem136.CustomizationFormText = "layoutControlItem136";
		this.layoutControlItem136.EditControl = null;
		this.layoutControlItem136.EditValue = "layoutControlItem136";
		this.layoutControlItem136.FieldName = null;
		this.layoutControlItem136.IsSearchControl = false;
		this.layoutControlItem136.Location = new System.Drawing.Point(0, 65);
		this.layoutControlItem136.Name = "layoutControlItem136";
		this.layoutControlItem136.ReadOnly = false;
		this.layoutControlItem136.SettingInVisible = false;
		this.layoutControlItem136.SettingReadonly = false;
		this.layoutControlItem136.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem136.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem136.TextVisible = false;
		this.layoutControlItem137.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem137.Control = this.ck_CostCounting;
		this.layoutControlItem137.CustomizationFormText = "layoutControlItem137";
		this.layoutControlItem137.EditControl = null;
		this.layoutControlItem137.EditValue = "layoutControlItem137";
		this.layoutControlItem137.FieldName = null;
		this.layoutControlItem137.IsSearchControl = false;
		this.layoutControlItem137.Location = new System.Drawing.Point(0, 89);
		this.layoutControlItem137.Name = "layoutControlItem137";
		this.layoutControlItem137.ReadOnly = false;
		this.layoutControlItem137.SettingInVisible = false;
		this.layoutControlItem137.SettingReadonly = false;
		this.layoutControlItem137.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem137.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem137.TextVisible = false;
		this.layoutControlItem138.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem138.Control = this.ck_SerialNum;
		this.layoutControlItem138.CustomizationFormText = "layoutControlItem138";
		this.layoutControlItem138.EditControl = null;
		this.layoutControlItem138.EditValue = "layoutControlItem138";
		this.layoutControlItem138.FieldName = null;
		this.layoutControlItem138.IsSearchControl = false;
		this.layoutControlItem138.Location = new System.Drawing.Point(0, 113);
		this.layoutControlItem138.Name = "layoutControlItem138";
		this.layoutControlItem138.ReadOnly = false;
		this.layoutControlItem138.SettingInVisible = false;
		this.layoutControlItem138.SettingReadonly = false;
		this.layoutControlItem138.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem138.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem138.TextVisible = false;
		this.layoutControlItem139.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem139.Control = this.ck_;
		this.layoutControlItem139.CustomizationFormText = "layoutControlItem139";
		this.layoutControlItem139.EditControl = null;
		this.layoutControlItem139.EditValue = "layoutControlItem139";
		this.layoutControlItem139.FieldName = null;
		this.layoutControlItem139.IsSearchControl = false;
		this.layoutControlItem139.Location = new System.Drawing.Point(0, 137);
		this.layoutControlItem139.Name = "layoutControlItem139";
		this.layoutControlItem139.ReadOnly = false;
		this.layoutControlItem139.SettingInVisible = false;
		this.layoutControlItem139.SettingReadonly = false;
		this.layoutControlItem139.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem139.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem139.TextVisible = false;
		this.layoutControlItem140.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem140.Control = this.ck_MRP;
		this.layoutControlItem140.CustomizationFormText = "layoutControlItem140";
		this.layoutControlItem140.EditControl = null;
		this.layoutControlItem140.EditValue = "layoutControlItem140";
		this.layoutControlItem140.FieldName = null;
		this.layoutControlItem140.IsSearchControl = false;
		this.layoutControlItem140.Location = new System.Drawing.Point(0, 161);
		this.layoutControlItem140.Name = "layoutControlItem140";
		this.layoutControlItem140.ReadOnly = false;
		this.layoutControlItem140.SettingInVisible = false;
		this.layoutControlItem140.SettingReadonly = false;
		this.layoutControlItem140.Size = new System.Drawing.Size(856, 24);
		this.layoutControlItem140.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem140.TextVisible = false;
		this.layoutControlItem149.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem149.Control = this.ck_UnitOfMeasure;
		this.layoutControlItem149.CustomizationFormText = "layoutControlItem149";
		this.layoutControlItem149.EditControl = null;
		this.layoutControlItem149.EditValue = "layoutControlItem149";
		this.layoutControlItem149.FieldName = null;
		this.layoutControlItem149.IsSearchControl = false;
		this.layoutControlItem149.Location = new System.Drawing.Point(0, 185);
		this.layoutControlItem149.Name = "layoutControlItem149";
		this.layoutControlItem149.ReadOnly = false;
		this.layoutControlItem149.SettingInVisible = false;
		this.layoutControlItem149.SettingReadonly = false;
		this.layoutControlItem149.Size = new System.Drawing.Size(856, 247);
		this.layoutControlItem149.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem149.TextVisible = false;
		this.Tab_Sales.Controls.Add(this.DfSVatServ);
		this.Tab_Sales.Controls.Add(this.DfSVatItem);
		this.Tab_Sales.Controls.Add(this.labelLine3);
		this.Tab_Sales.Controls.Add(this.labelLine2);
		this.Tab_Sales.Controls.Add(this.labelLine1);
		this.Tab_Sales.Controls.Add(this.luk_DfltCard);
		this.Tab_Sales.EditControl = null;
		this.Tab_Sales.EditValue = "Sales";
		this.Tab_Sales.FieldName = "Tab_Sales";
		this.Tab_Sales.IsSearchControl = false;
		this.Tab_Sales.Name = "Tab_Sales";
		this.Tab_Sales.ReadOnly = false;
		this.Tab_Sales.SettingInVisible = false;
		this.Tab_Sales.SettingReadonly = false;
		this.Tab_Sales.Size = new System.Drawing.Size(876, 452);
		this.Tab_Sales.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Sales.Text = "Sales";
		this.DfSVatServ.EditControl = null;
		this.DfSVatServ.FieldName = null;
		this.DfSVatServ.IsSearchControl = false;
		this.DfSVatServ.Location = new System.Drawing.Point(164, 41);
		this.DfSVatServ.Name = "DfSVatServ";
		this.DfSVatServ.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfSVatServ.Properties.IsSearchControl = false;
		this.DfSVatServ.Properties.NullText = "";
		this.DfSVatServ.SettingInVisible = false;
		this.DfSVatServ.SettingReadonly = false;
		this.DfSVatServ.Size = new System.Drawing.Size(150, 20);
		this.DfSVatServ.TabIndex = 5;
		this.DfSVatServ.EditValueChanged += new System.EventHandler(this.DfSVatServ_EditValueChanged);
		this.DfSVatItem.EditControl = null;
		this.DfSVatItem.FieldName = null;
		this.DfSVatItem.IsSearchControl = false;
		this.DfSVatItem.Location = new System.Drawing.Point(164, 12);
		this.DfSVatItem.Name = "DfSVatItem";
		this.DfSVatItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfSVatItem.Properties.IsSearchControl = false;
		this.DfSVatItem.Properties.NullText = "";
		this.DfSVatItem.SettingInVisible = false;
		this.DfSVatItem.SettingReadonly = false;
		this.DfSVatItem.Size = new System.Drawing.Size(150, 20);
		this.DfSVatItem.TabIndex = 4;
		this.DfSVatItem.EditValueChanged += new System.EventHandler(this.DfSVatItem_EditValueChanged);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Sales Tax Group (Service)";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(11, 44);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(160, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 3;
		this.labelLine3.Utext = "Sales Tax Group (Service)";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Sales Tax Group (Item)";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(11, 15);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(160, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Sales Tax Group (Item)";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Default Customer for A/R Invoice and Payment";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 108);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(234, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Default Customer for A/R Invoice and Payment";
		this.labelLine1.Visible = false;
		this.luk_DfltCard.EditControl = null;
		this.luk_DfltCard.FieldName = null;
		this.luk_DfltCard.IsSearchControl = false;
		this.luk_DfltCard.Location = new System.Drawing.Point(417, 108);
		this.luk_DfltCard.Name = "luk_DfltCard";
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.luk_DfltCard.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_DfltCard.Properties.IsSearchControl = false;
		this.luk_DfltCard.Properties.NullText = "";
		this.luk_DfltCard.SettingInVisible = false;
		this.luk_DfltCard.SettingReadonly = false;
		this.luk_DfltCard.Size = new System.Drawing.Size(100, 20);
		this.luk_DfltCard.TabIndex = 0;
		this.luk_DfltCard.Visible = false;
		this.Tab_Purchasing.Controls.Add(this.DfPVatServ);
		this.Tab_Purchasing.Controls.Add(this.DfPVatItem);
		this.Tab_Purchasing.Controls.Add(this.labelLine4);
		this.Tab_Purchasing.Controls.Add(this.labelLine5);
		this.Tab_Purchasing.EditControl = null;
		this.Tab_Purchasing.EditValue = "Purchasing";
		this.Tab_Purchasing.FieldName = "Tab_Purchasing";
		this.Tab_Purchasing.IsSearchControl = false;
		this.Tab_Purchasing.Name = "Tab_Purchasing";
		this.Tab_Purchasing.ReadOnly = false;
		this.Tab_Purchasing.SettingInVisible = false;
		this.Tab_Purchasing.SettingReadonly = false;
		this.Tab_Purchasing.Size = new System.Drawing.Size(876, 452);
		this.Tab_Purchasing.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Purchasing.Text = "Purchasing";
		this.DfPVatServ.EditControl = null;
		this.DfPVatServ.FieldName = null;
		this.DfPVatServ.IsSearchControl = false;
		this.DfPVatServ.Location = new System.Drawing.Point(171, 41);
		this.DfPVatServ.Name = "DfPVatServ";
		this.DfPVatServ.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfPVatServ.Properties.IsSearchControl = false;
		this.DfPVatServ.Properties.NullText = "";
		this.DfPVatServ.SettingInVisible = false;
		this.DfPVatServ.SettingReadonly = false;
		this.DfPVatServ.Size = new System.Drawing.Size(150, 20);
		this.DfPVatServ.TabIndex = 9;
		this.DfPVatServ.EditValueChanged += new System.EventHandler(this.DfPVatServ_EditValueChanged);
		this.DfPVatItem.EditControl = null;
		this.DfPVatItem.FieldName = null;
		this.DfPVatItem.IsSearchControl = false;
		this.DfPVatItem.Location = new System.Drawing.Point(171, 14);
		this.DfPVatItem.Name = "DfPVatItem";
		this.DfPVatItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DfPVatItem.Properties.IsSearchControl = false;
		this.DfPVatItem.Properties.NullText = "";
		this.DfPVatItem.SettingInVisible = false;
		this.DfPVatItem.SettingReadonly = false;
		this.DfPVatItem.Size = new System.Drawing.Size(150, 20);
		this.DfPVatItem.TabIndex = 8;
		this.DfPVatItem.EditValueChanged += new System.EventHandler(this.DfPVatItem_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Purchase Tax Group (Service)";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(9, 43);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(163, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 7;
		this.labelLine4.Utext = "Purchase Tax Group (Service)";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Purchase Tax Group (Item)";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(9, 17);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(163, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 6;
		this.labelLine5.Utext = "Purchase Tax Group (Item)";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 481);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(78, 23);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 481);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(78, 23);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.emptySpaceItem74.AllowHotTrack = false;
		this.emptySpaceItem74.CustomizationFormText = "emptySpaceItem74";
		this.emptySpaceItem74.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem74.Name = "emptySpaceItem74";
		this.emptySpaceItem74.Size = new System.Drawing.Size(10, 171);
		this.emptySpaceItem74.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem84.AllowHotTrack = false;
		this.emptySpaceItem84.CustomizationFormText = "emptySpaceItem84";
		this.emptySpaceItem84.Location = new System.Drawing.Point(0, 23);
		this.emptySpaceItem84.Name = "emptySpaceItem84";
		this.emptySpaceItem84.Size = new System.Drawing.Size(10, 27);
		this.emptySpaceItem84.TextSize = new System.Drawing.Size(0, 0);
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
		this.openFileDialog1.FileName = "openFileDialog1";
		this.layoutControlItem51.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem51.CustomizationFormText = "layoutControlItem51";
		this.layoutControlItem51.EditControl = null;
		this.layoutControlItem51.EditValue = "layoutControlItem51";
		this.layoutControlItem51.FieldName = null;
		this.layoutControlItem51.IsSearchControl = false;
		this.layoutControlItem51.Location = new System.Drawing.Point(712, 0);
		this.layoutControlItem51.Name = "layoutControlItem51";
		this.layoutControlItem51.ReadOnly = false;
		this.layoutControlItem51.SettingInVisible = false;
		this.layoutControlItem51.SettingReadonly = false;
		this.layoutControlItem51.Size = new System.Drawing.Size(70, 429);
		this.layoutControlItem51.TextSize = new System.Drawing.Size(50, 20);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(877, 510);
		base.Controls.Add(this.xtraTab_Inventory);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oGeneralSeting";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_GeneSetting.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_GeneSetting";
		base.ShowInTaskbar = false;
		this.Text = "General Setting";
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Inventory).EndInit();
		this.xtraTab_Inventory.ResumeLayout(false);
		this.Tab_BP.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.MBAOnPer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DspIBPDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DspFrznBP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.EnbApprDI.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.useDocWrf.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CrdCommUse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItmCommUse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SlpCommUse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PickLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OrderLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DlnLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalesLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CreditLimt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ObligLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AddDlnBlnc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfCustTerm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfVendTerm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfltCustPM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfltVendPM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CreditDpst.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).EndInit();
		this.Tab_Budget.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.BdgtAcctng.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPDNDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPORDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BdgtPRQDOC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BgtWarning.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BgtBlock.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DoBudget.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem28).EndInit();
		this.Tab_Services.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).EndInit();
		this.layoutControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SPNEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.IMCEEnable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MaxHistory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ScreenLock.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AlertPolFr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfltByEml.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_UsedProxy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ShowNewTsk.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OpenDps.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OpenCdt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ShowNewMsg.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ContactLog.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RclFlag.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RcrFlag.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DsplyRates.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CheckFiles.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OpenCredit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MapService.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Address.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Port.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Area.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ForEx.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem39).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem41).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem42).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem46).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem47).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem49).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem50).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem152).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem153).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem154).EndInit();
		this.Tab_Display.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).EndInit();
		this.layoutControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.TxtSrch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.B1SgtEb.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_ManagerCompany.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DirectRate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CurOnRight.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ThousSep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DecSep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.QueryDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MeasureDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PercentDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.QtyDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RateDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SumDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MaxChoose.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AdvImagePr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DateSep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Color.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DefLengthU.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DefWeightU.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.TimeFormat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DateFormat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.TPLId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PriceDec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem53).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem54).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem55).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem56).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem58).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem59).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem60).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem61).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem62).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem63).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem64).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem65).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem66).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem67).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem68).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem69).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem71).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem72).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem73).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem74).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem75).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).EndInit();
		this.Tab_Font.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl5).EndInit();
		this.layoutControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_PicBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WllPprDsp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lok_Fonts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lok_fontSize.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PreviewFont.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WallPaper.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem76).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem77).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem78).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem79).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem80).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem81).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem82).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).EndInit();
		this.Tab_Path.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl6).EndInit();
		this.layoutControl6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.DisableSplitFolder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ObjAtt).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ObjAtt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ObjType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Path).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbo_ObjType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.XmlPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CurrentScander.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ExtPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AttachPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BitmapPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ExcelPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WordPath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem84).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem85).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem89).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem90).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem93).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem94).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem95).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem96).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem83).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem91).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem92).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem86).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem87).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem88).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem52).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem97).EndInit();
		this.Tab_Inventory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.Tab_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl7).EndInit();
		this.layoutControl7.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.OneBOneRec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SriCreatIn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DspFrznITM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DspIITMDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutoAddPkg.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutoAddUoM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WTLiable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutoITW.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutoCrIns.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SriMngSys.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SriUniqFld.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BtchStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfltWhs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.GLMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem99).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem100).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem101).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem102).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem103).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem105).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem106).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem107).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem108).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem109).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem98).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem142).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem143).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem144).EndInit();
		this.Tab_Plan.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl14).EndInit();
		this.layoutControl14.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.DaysBack.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DaysFwrd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ConsumeFCT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ConsumeMtd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem104).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem145).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem146).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem141).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem79).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem82).EndInit();
		this.Tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl15).EndInit();
		this.layoutControl15.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SimReport.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem147).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem83).EndInit();
		this.Tab_Resources.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl8).EndInit();
		this.layoutControl8.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Days.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Months.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutoResWhs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DftResWhs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.StartFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem110).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem111).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem112).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem113).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem114).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).EndInit();
		this.Tab_CashFlow.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl9).EndInit();
		this.layoutControl9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.IgrAllCash.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CfwAsnMust.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CfwInDflt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CfwOutDflt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem59).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem115).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem61).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem116).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem117).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem63).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem118).EndInit();
		this.Tab_Cockpit.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl10).EndInit();
		this.layoutControl10.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PrsWkCntEb.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem119).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem120).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem67).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).EndInit();
		this.Tab_CostAccounting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).EndInit();
		this.layoutControl11.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ApyPRinPEC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ApyDRinPEC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MDStyle.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UseMltDims.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PrjBlock.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem121).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem124).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem72).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem85).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem86).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem87).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem122).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem148).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem133).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem155).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).EndInit();
		this.Tab_Pricing.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl12).EndInit();
		this.layoutControl12.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SIPLSeting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SIPLDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SIPLReport.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.IsUpdNstdB.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ClnZeroPln.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActPL.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActSP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ApyBsActPV.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PriceProcM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem75).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem125).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem126).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem127).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem128).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem78).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem129).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem130).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem131).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem132).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Setting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem150).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem151).EndInit();
		this.Tab_HideFunctions.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl13).EndInit();
		this.layoutControl13.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_UnitOfMeasure.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_MRP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_SerialNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_CostCounting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_DunningWizard.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_PaymentWizard.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Budget.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleLabelItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem134).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem135).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem136).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem137).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem138).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem139).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem140).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem149).EndInit();
		this.Tab_Sales.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.DfSVatServ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfSVatItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DfltCard.Properties).EndInit();
		this.Tab_Purchasing.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.DfPVatServ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DfPVatItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem84).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem51).EndInit();
		base.ResumeLayout(false);
	}
}
