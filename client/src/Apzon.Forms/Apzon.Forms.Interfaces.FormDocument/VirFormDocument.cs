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
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.PM;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Data;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Interfaces.FormDocument;

public class VirFormDocument : XtraFormEx
{
	private DataTable _originalDocTable = new DataTable();

	private DataTable _downPaymentData;

	private Enums.SaveAction _saveAction = Enums.SaveAction.SaveAndSearch;

	public int MaxDocEntry;

	public int MinDocEntry;

	public int DocEntrySearch;

	public string FormBpType = "";

	public string FormDocumentTable;

	public string FormLineTable;

	public string FormController;

	public Enums.DownPaymentType DownPaymentType;

	public Enums.ChangeLog ChangeLogType;

	private bool _isLoading;

	private bool _isSearching;

	public bool IsSalesDocument = true;

	public string CheckAri = "N";

	public string codeNameForAttachment = "";

	public bool IsRefreshDataSource;

	protected int LineNumFocus = -1;

	public string DocFreightTable;

	public Enums.FormViewModeType OpenFormViewMode = Enums.FormViewModeType.View;

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private bool _isChangingRow;

	private readonly List<CntPRepControls> lstContactControls = new List<CntPRepControls>();

	private bool _OpenChooseCFL;

	private readonly string[] _arrBomSalesFatherDisableCols = new string[44]
	{
		"Price", "DiscPrcnt", "VatGroup", "LineTotal", "TotalFrgn", "OpenQty", "UomCode", "OpenInvQty", "UseBaseUn", "PackQty",
		"TreeType", "LineStatus", "TargetType", "TrgetEntry", "BaseType", "BaseRef", "BaseEntry", "BaseLine", "SpecPrice", "TaxAmount",
		"LineNum", "PoTrgNum", "OrigItem", "PriceBefDi", "LineVat", "LineTotal", "QuantityOnStock", "QuantityOnWareHouse", "IsCommited", "IsCommitedInWareHouse",
		"OnOrder", "OnOrderInWareHouse", "Rate", "OpenCreQty", "Currency", "Rate", "BOMTreeType", "IsAqcuistn", "DistribExp", "Address",
		"PickStatus", "NumPerMsr", "GrossBase", "DelivrdQty"
	};

	private readonly string[] _arrBomSalesComponentsDisableCols = new string[38]
	{
		"TotalFrgn", "OpenQty", "UomCode", "OpenInvQty", "UseBaseUn", "PackQty", "TreeType", "LineStatus", "TargetType", "TrgetEntry",
		"BaseType", "BaseRef", "Rate", "BaseEntry", "BaseLine", "SpecPrice", "TaxAmount", "LineNum", "PoTrgNum", "OrigItem",
		"LineVat", "QuantityOnStock", "QuantityOnWareHouse", "IsCommited", "IsCommitedInWareHouse", "OnOrder", "OnOrderInWareHouse", "DelivrdQty", "OpenCreQty", "Currency",
		"Rate", "BOMTreeType", "IsAqcuistn", "Address", "unitMsr", "PickStatus", "NumPerMsr", "ItemCode"
	};

	private HttpClientHelper<BusinessPartnersImpl> _bpClientHelper;

	private IHttpClient _httpClient;

	public DocumentLineTable LstLineEdit = new DocumentLineTable();

	public DocumentLineTable LstLineDelete = new DocumentLineTable();

	public DocumentLineTable LstLineCopy = new DocumentLineTable();

	public int NewDocEntry;

	public bool OkToClose;

	public int CurrentEntry;

	public List<Formular> Formulars = new List<Formular>();

	public Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	public ApzObjectTypes Fromtype;

	public string Fromentry = string.Empty;

	public XtraFormEx Formcopy;

	private DataView _clone;

	public int _docEntry;

	public int _currentFileEntry;

	public int _newFileEntry;

	public DataTable _currentRefDoc = new DataTable();

	public List<Common.Table_ID_Code> LstBomChilds = new List<Common.Table_ID_Code>();

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private GridViewExPainter painter;

	public bool IsLoadPrice;

	protected DocumentSettingImpl _documentSetting = new DocumentSettingImpl();

	private bool _showForLandedCost = true;

	public string FilterHeader = "";

	public string FilterLine = "";

	private DataTable _dtTabAccounting = new DataTable();

	private DataTable _dtContact = new DataTable();

	public DataTable PaymentTermInfo;

	private static string _customizePaymentTerms;

	private readonly Image _btnSaveImg;

	private string _originText;

	private static readonly string[] _lstDocFieldNotEditable = new string[25]
	{
		"DocEntry", "DocStatus", "DocStatusName", "UserSign", "UserSign2", "CreateDate", "CreateTS", "UpdateDate", "UpdateTS", "Printed",
		"CANCELED", "InvntSttus", "TransId", "Posted", "WddStatus", "DocTotal", "DocTotalFC", "IsDraft", "draftKey", "AtcEntry",
		"OpenForLaC", "DataSource", "TotalBeforeDis", "TaxTotal", "Handwrtten"
	};

	private static readonly string[] _lstDocFieldEditable = new string[12]
	{
		"CntctCode", "NumAtCard", "DocDueDate", "PayToCode", "Address", "TrnspCode", "TrackNo", "StampNo", "SlpCode", "OwnerCode",
		"Comments", "JrnlMemo"
	};

	private static readonly string[] _lstCheckCancellation = new string[19]
	{
		"Series", "CntctCode", "NumAtCard", "DocDueDate", "SlpCode", "OwnerCode", "Comments", "Address", "PayToCode", "TrackNo",
		"StampNum", "PickRmrk", "JrnlMemo", "GroupNum", "PeyMethod", "Project", "Indicator", "LicTradNum", "ImportEnt"
	};

	private static readonly string[] _lstFieldLineNotEditable = new string[25]
	{
		"LineNum", "BaseType", "BaseLine", "BaseEntry", "BaseRef", "TargetType", "TrgetEntry", "OpenQty", "OpenInvQty", "LineStatus",
		"UomEntry", "InvntSttus", "QuantityOnWareHouse", "ManBtchSerial", "ManBatchSerial", "UgpCode", "QuantityOnStock", "IsCommited", "OnOrder", "IsCommitedInWareHouse",
		"OnOrderInWareHouse", "BOMTreeType", "TreeType", "AvgPrice", "CmpltQty"
	};

	private static readonly string[] _lstPDNField = new string[4] { "PriceBefDi", "Price", "LineTotal", "TotalFrgn" };

	private bool _lookupChanged;

	private DataTable _dtSeries;

	public DateEditEx txtFromDate;

	public DateEditEx txtToDate;

	public TextEditEx txtNoOfTrans;

	public GridControlEx grdDoc;

	public GridColumnEx gcDocNum;

	public GridColumnEx gcDoc_CardCode;

	public GridColumnEx gcDoc_CardName;

	public GridColumnEx gcDoc_CntctCode;

	public GridColumnEx gcStatus;

	public GridColumnEx gcPostingDate;

	public GridColumnEx col_DueDate;

	public GridColumnEx col_DocDate;

	public GridColumnEx gcRemark;

	public GridColumnEx gcTotalBeforeDis;

	public GridColumnEx gcDis;

	public GridColumnEx gcTotalTax;

	public GridColumnEx gcDocTotal;

	public GridColumnEx gcCurrency;

	public GridColumnEx gcRate;

	public XtraTabControlEx xtraTab_Line;

	public XtraTabPageEx tab_Logistic;

	public GridControlEx grdLogistic;

	public GridColumnEx gcShipType;

	public GridColumnEx gcShipTo;

	public GridColumnEx gcBillTo;

	public GridColumnEx gcConfirmed;

	public XtraTabPageEx tbDocumentLine;

	public GridControlEx grdLine;

	public GridColumnEx gcItemCode;

	public GridColumnEx gcItemName;

	public GridColumnEx gcQuantity;

	public GridColumnEx gcUnitPrice;

	public GridColumnEx gcDisPer;

	public GridColumnEx gcVatGroup;

	public GridColumnEx gcTotalLine;

	public GridColumnEx gcWhseCode;

	public GridColumnEx gcLine_DelDate;

	public GridColumnEx gcRemarkLine;

	public GridColumnEx gcOpenQty;

	public GridColumnEx gcCureencyLine;

	public GridColumnEx gcVatPrcnt;

	public LabelLine lb_FromDate;

	public LabelLine lb_ToDate;

	public LabelLine lb_DocNumber;

	public LabelLine lb_BusinessPartner;

	public LabelLine lb_DocStatus;

	public ContextMenuStrip ctxDoc;

	public ToolStripMenuItem ctm_AddRow;

	public ButtonEditEx txtBusinessPartner;

	public RepositoryItemButtonEdit rtxtCardCode;

	public RepositoryItemButtonEdit rtxtItemcode;

	public GridColumnEx gcLineNum;

	public RepositoryItemComboBox cbxTaxCode;

	public RepositoryItemLookUpEdit rLookUpVatGroup;

	public ToolStripMenuItem ctm_Duplicate;

	public RepositoryItemButtonEdit rtxtWhseCode;

	public System.Windows.Forms.ComboBox cbxStatus;

	public GridColumnEx gridColumn2;

	public ContextMenuStrip ctxLine;

	public ToolStripMenuItem ctm_addRowLine;

	public ToolStripMenuItem ctm_delRowLine;

	public RepositoryItemCalcEdit repositoryItemCalcEdit1;

	public GridColumnEx gridColumn5;

	public GridColumnEx gridColumn6;

	public ToolStripMenuItem ctm_Cancel;

	public ToolStripMenuItem ctm_Close;

	public GridColumnEx gridColumn7;

	public BarManager barManager1;

	public BarDockControl barDockControlTop;

	public BarDockControl barDockControlBottom;

	public BarDockControl barDockControlLeft;

	public BarDockControl barDockControlRight;

	public BarButtonItem ctmCopyToArInvoice;

	public BarButtonItem ctmFromQuotation;

	public GridColumnEx gridColumn8;

	public GridColumnEx gridColumn9;

	public GridColumnEx gcLineVat;

	public RepositoryItemLookUpEdit rlpShipType;

	public RepositoryItemLookUpEdit rShipTo;

	public RepositoryItemLookUpEdit rBillTo;

	public ToolStripMenuItem ctm_LineClose;

	public ToolStripMenuItem ctm_LineDuplicate;

	public RepositoryItemLookUpEdit rlupContactPersonCode;

	public BarButtonItem barButtonItem1;

	public BarButtonItem bar_SaveAsDraft;

	public ToolStripSeparator toolStripSeparator1;

	public ToolStripSeparator toolStripSeparator2;

	public ToolStripMenuItem ctm_SaveAsDraft;

	public RepositoryItemCalcEdit repositoryItemCalcEdit4;

	public RepositoryItemCalcEdit repositoryItemCalcEdit3;

	public RepositoryItemCalcEdit repositoryItemCalcEdit2;

	public GridColumnEx gcVatSumFC;

	public GridColumnEx gcDocTotalFC;

	public RepositoryItemLookUpEdit rILCurrency;

	public RepositoryItemCalcEdit repositoryItemCalcEdit5;

	public RepositoryItemCalcEdit repositoryItemCalcEdit6;

	public GridColumnEx gcLineTotalFC;

	public GridColumnEx gcLineVatSumFrgn;

	public XtraTabPageEx tab_PaymentTerms;

	public GridControlEx grdPaymentTerms;

	public SpinEdit spin_Count;

	public LabelLine lb_Count;

	public CalcEditEx cal_Total;

	public LabelLine lb_Total;

	public GridColumnEx gcTotal;

	public GridColumnEx gcVatAmount;

	public RepositoryItemDateEdit repositoryItemDateEdit1;

	public GridViewEx gvDoc;

	public GridViewEx gv_Logistic;

	public GridViewEx gvLine;

	public GridViewEx gvPayTerms;

	protected GridColumnEx gcBOMTreeType;

	protected RepositoryItemDateEdit repositoryItemDateEdit2;

	protected XtraTabPageEx tab_Attachments;

	protected GridColumnEx gcDocAtcEntry;

	protected GridColumnEx gc_Doc_Branch;

	protected GridColumnEx gc_Doc_BranchVATRegNum;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit8;

	protected GridColumnEx gcStageSalesEmp;

	protected RepositoryItemDateEdit repositoryItemDateEdit3;

	protected ToolStripMenuItem ctm_targetDocument;

	protected ToolStripMenuItem ctm_baseDocument;

	protected ToolStripMenuItem ctm_targetDocumentLine;

	protected ToolStripMenuItem ctm_baseDocumentLine;

	protected GridColumnEx gcTrgetEntry;

	protected GridColumnEx gcBaseType;

	protected GridColumnEx gcBaseRef;

	protected GridColumnEx gcTargetType;

	protected GridColumnEx gcBaseEntry;

	protected GridColumnEx gcBaseLine;

	public SimpleButtonEx btnSearch;

	public SimpleButtonEx btnCancel;

	public DropDownButtonEx drlCopyFrom;

	public DropDownButtonEx drlCopyTo;

	protected GridColumnEx gcLineTreeType;

	protected GridColumnEx gcDeliveryQty;

	public LabelLine labelLine1;

	protected GridColumnEx gcApprovalStatus;

	protected RepositoryItemLookUpEdit rep_ApprovalStatus;

	protected GridColumnEx gcLineCtlAccount;

	protected RepositoryItemButtonEdit rep_bte_CtlAccount;

	protected GridColumnEx gcReqDate;

	protected GridColumnEx gcLinePQTReqdate;

	protected RepositoryItemDateEdit repositoryItemDateEdit4;

	protected XtraTabPageEx tab_Freight;

	protected GridControlEx grd_Freight;

	protected GridViewEx gv_Freight;

	protected GridColumnEx gridColumn4;

	protected GridColumnEx gridColumn1;

	protected GridColumnEx gridColumn18;

	protected RepositoryItemLookUpEdit rep_DistriMethod;

	protected RepositoryItemLookUpEdit rep_StatusFreight;

	protected RepositoryItemButtonEdit rep_Project;

	public RepositoryItemLookUpEdit rep_GroupTax;

	protected RepositoryItemLookUpEdit rep_LstPrice;

	public GridColumnEx gridColumn24;

	public GridColumnEx gridColumn11;

	public GridColumnEx gridColumn13;

	public GridColumnEx gridColumn14;

	public GridColumnEx gridColumn15;

	public GridColumnEx gridColumn16;

	public GridColumnEx gridColumn17;

	public GridColumnEx gridColumn19;

	public GridColumnEx gridColumn20;

	public GridColumnEx gridColumn21;

	public GridColumnEx gridColumn22;

	public GridColumnEx gridColumn23;

	public GridColumnEx gridColumn25;

	public GridColumnEx gridColumn26;

	public GridColumnEx gridColumn27;

	public GridColumnEx gridColumn28;

	public GridColumnEx gridColumn29;

	public GridColumnEx gridColumn30;

	public GridColumnEx gridColumn31;

	public GridColumnEx gridColumn32;

	public GridColumnEx gridColumn33;

	public GridColumnEx gridColumn34;

	public GridColumnEx gridColumn35;

	public GridColumnEx gridColumn36;

	public GridColumnEx gridColumn37;

	public GridColumnEx gridColumn38;

	public GridColumnEx gridColumn39;

	public GridColumnEx gridColumn40;

	public GridColumnEx gridColumn41;

	public GridColumnEx gridColumn42;

	public GridColumnEx gridColumn43;

	public GridColumnEx gridColumn44;

	public GridColumnEx gridColumn45;

	public GridColumnEx gridColumn46;

	public GridColumnEx gridColumn47;

	public GridColumnEx gridColumn48;

	public RepositoryItemLookUpEdit rep_FreightCode;

	public RepositoryItemLookUpEdit rep_FreightTaxGroup;

	protected GridColumnEx gridColumn49;

	protected GridColumnEx gcLinePackQty;

	protected GridColumnEx gcLineUoMName;

	protected GridColumnEx gc_Doc_CurrencyType;

	protected XtraTabPageEx tabP_Accounting;

	public GridControlEx grd_Accounting;

	public GridViewEx gv_Accounting;

	protected GridColumnEx gc_Accounting_JrnlMemo;

	protected GridColumnEx gc_Accounting_Project;

	protected RepositoryItemButtonEdit bte_Accounting_Project;

	protected GridColumnEx gc_Accounting_GroupNum;

	protected RepositoryItemLookUpEdit luk_Accounting_GroupNum;

	protected GridColumnEx gc_Accounting_PeyMethod;

	protected RepositoryItemLookUpEdit luk_Accounting_PaymentMethod;

	protected GridColumnEx gc_Accounting_Indicator;

	protected RepositoryItemLookUpEdit luk_Accounting_Indicator;

	protected GridColumnEx gc_Accounting_LicTradNum;

	protected GridColumnEx gc_Accounting_ImportEnt;

	protected GridColumnEx gc_Accounting_PayDuMonth;

	protected RepositoryItemLookUpEdit luk_Accounting_PayDuMonth;

	protected GridColumnEx gc_Accounting_ExtraMonth;

	protected GridColumnEx gc_Accounting_ExtraDays;

	protected GridColumnEx gc_Accounting_CdcOffset;

	protected GridColumnEx gc_Line_CogsOcrCo2;

	protected GridColumnEx gcLineInventoryUom;

	protected GridColumnEx gcLineItemPerUnit;

	protected RepositoryItemCalcEdit rep_UnitsOfMeasurment;

	protected GridColumnEx gcLineIvenQty;

	protected RepositoryItemCalcEdit rep_InventoryQuantity;

	protected RepositoryItemLookUpEdit rLineUseBaseUnits;

	protected GridColumnEx gc_line_QuantityOnStock;

	protected GridColumnEx gc_line_QuantityOnWareHouse;

	protected GridColumnEx gcBPCatalog;

	protected RepositoryItemButtonEdit rBpCatalog;

	protected GridColumnEx gridColumn57;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit9;

	protected ToolStripMenuItem ctm_BatchSerialNumbers;

	protected SplitContainerControl split_Main;

	public GridColumnEx gcLine_SlpCode;

	protected RepositoryItemLookUpEdit rep_LineSlpCode;

	protected GridColumnEx gcDoc_Series;

	protected RepositoryItemLookUpEdit rep_Series;

	protected GridColumnEx gcLineUomGroup;

	protected GridColumnEx gcLineUomCode;

	protected RepositoryItemLookUpEdit rLineUomGroup;

	protected RepositoryItemButtonEdit rLineUomCode;

	protected GridColumnEx gcLineOpenIvnQty;

	protected ToolStripSeparator toolStripSeparator4;

	protected ToolStripMenuItem viewDiagramToolStripMenuItem;

	protected GridColumnEx gridColumn51;

	protected GridColumnEx gridColumn52;

	protected GridColumnEx gridColumn68;

	protected GridColumnEx gridColumn67;

	protected GridColumnEx gridColumn66;

	protected GridColumnEx gridColumn12;

	protected GridColumnEx gridColumn53;

	protected GridColumnEx gridColumn54;

	protected GridColumnEx gcDoc_ReqType;

	protected RepositoryItemLookUpEdit rep_ReqType;

	protected GridColumnEx gcDoc_Requester;

	protected RepositoryItemButtonEdit rep_Requester;

	protected GridColumnEx gcDoc_ReqName;

	protected GridColumnEx gcDoc_Department;

	protected RepositoryItemLookUpEdit rep_Department;

	protected GridColumnEx gcDoc_Branch;

	protected RepositoryItemLookUpEdit rep_Branch;

	protected LabelLine lb_Requester;

	private IContainer components;

	private RepositoryItemMemoExEdit rep_Address2;

	private RepositoryItemMemoExEdit rep_Address;

	private RepositoryItemLookUpEdit rep_FreightStock;

	private RepositoryItemLookUpEdit rep_ExpenseCode;

	private GridColumnEx gcDoc_NumAtCard;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemCheckEdit rep_chk_UseShpdGd;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemButtonEditCFL rep_LineProject;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemImageComboBox rep_LineType;

	private ImageCollection imageCollection1;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private ToolStripMenuItem toolStripMenuItemOpeningClosing;

	private ToolStripSeparator toolStripSeparator5;

	protected GridColumnEx gcLine_VendorNum;

	private RepositoryItemButtonEditCFL rep_VendorNum;

	private ToolStripMenuItem ctm_RemoveDoc;

	private SplitContainerControl split_LineBatchAndSerial;

	protected ucBatchAndSerial ucBatchAndSerial1;

	private GridColumnEx gcDoc_OwnerCode;

	private RepositoryItemButtonEditCFL rep_doc_OwnerCode;

	private ToolStripMenuItem ctm_NewTask;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private ToolStripMenuItem ctm_NewEvent;

	private ToolStripMenuItem ctm_NewNote;

	private ToolStripMenuItem ctm_ForLandedCost;

	private RepositoryItemTextEditExPrice rPriceEdit;

	private RepositoryItemLookUpEdit rep_BaseType;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemCheckEdit rep_NoInvtryMv;

	private GridColumnEx gridColumnEx12;

	public CalcEditEx cal_TotalFC;

	protected ButtonEditEx txtRequester;

	private GridColumnEx gridColumnEx13;

	private RepositoryItemLookUpEdit rep_Stock;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemLookUpEdit rep_LstPchPrce;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx9;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx10;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx11;

	private RepositoryItemCalcEditEx rLinePackQty;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx13;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx14;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx15;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx16;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx17;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx18;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx19;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx20;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gcDocProject;

	private RepositoryItemButtonEditCFL rDocProject;

	protected LookUpEditEx look_ReqType;

	private GridColumnEx gcDpmPrcnt;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx21;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx22;

	private RepositoryItemCheckEdit rEnSetCost;

	protected GridColumnEx gcLineEnSetCost;

	protected GridColumnEx gcLineRetCost;

	private GridColumnEx gcAgrNo;

	private GridColumnEx gcAgrLnNum;

	private GridColumnEx gcInfoPriceList;

	private RepositoryItemLookUpEditSimple rInfoPriceList;

	private GridColumnEx gcLineSpecPrice;

	private RepositoryItemLookUpEditSimple rLineSpecPrice;

	private RepositoryItemCheckEdit rConfirmed;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx12;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx23;

	private GridColumnEx gcLogistics_BPChCode;

	private RepositoryItemButtonEditCFL rLogisticsBpChannel;

	private GridColumnEx gcLogisticsBpChannelCntc;

	private RepositoryItemLookUpEdit rLogisticBpChannelCntc;

	protected ucAttachment ucAttachment1;

	private GridColumnEx gridColumnEx21;

	private RepositoryItemCheckEdit rep_PartSupply;

	private ToolStripMenuItem alternativeItemsToolStripMenuItem;

	public SimpleButtonEx btn_CheckPromotion;

	private GridColumnEx gcLineTaxOnly;

	private ToolStripMenuItem ctm_RelatedActivities;

	public SimpleButtonEx btn_ClearPromotion;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit3;

	protected DropDownButtonEx btn_Save;

	private GridColumnEx gc_CountryOrg;

	private RepositoryItemLookUpEdit rep_CountryOrg;

	private GridColumnEx gc_StdItemId;

	protected GridColumnEx gcDpmTotal;

	protected RepositoryItemButtonEdit rep_PmtToDraw;

	private GridColumnEx gcDpmVat;

	private GridColumnEx gcDpmVatFC;

	private GridColumnEx gridColumnEx22;

	private RepositoryItemButtonEdit rep_ReferencedDocument;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gcCtlAccount;

	private RepositoryItemButtonEdit rep_CtlAccount;

	private GridColumnEx gcDiscount;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx24;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx25;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx26;

	private GridColumnEx gcPriceMode;

	private RepositoryItemLookUpEdit rep_PriceMode;

	private GridColumnEx gcUomEntry;

	protected RepositoryItemGridLookUpEdit rILSalesEmp;

	private GridView repositoryItemGridLookUpEdit1View;

	public int OpporId { get; set; }

	public DataTable OpenFormDocument { get; set; }

	public DataTable OpenFormLines { get; set; }

	public DataTable OpenFormBatchSerial { get; set; }

	public ContextActionMappingImpl[] MappingDocument { get; set; }

	public ContextActionMappingImpl[] MappingLines { get; set; }

	public ContextActionMappingImpl[] MappingBatchSerial { get; set; }

	public bool Cancellation { get; set; }

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	public List<ApzObjectTypes> PopCopyTo { get; set; }

	public List<ApzObjectTypes> PopBaseDocument { get; set; }

	public List<ApzObjectTypes> PopTargetDocument { get; set; }

	public GridColumn LineLastColumn { get; set; }

	public Enums.BatchSerialTransType BatchSerialTransType { get; set; }

	public string FormLayoutCode { get; set; }

	public string FormLayoutCodeService { get; set; }

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full)
		{
			_formMode = Enums.FormViewModeType.View;
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_Save.ImageOptions.Image = null;
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
			ucBatchAndSerial1.EnableDisableControls(isEnable: false);
		}
		else
		{
			if (_dtSeries != null && _formMode == Enums.FormViewModeType.Add && value != Enums.FormViewModeType.Add)
			{
				rep_Series.DataSource = _dtSeries;
			}
			_formMode = value;
			EnableDisableControls();
		}
	}

	public VirFormDocument()
	{
		InitializeComponent();
		_btnSaveImg = btn_Save.ImageOptions.Image;
		OnLoadAsync += VirFormDocument_Load;
	}

	protected virtual void SettingLanguage()
	{
		ctm_Cancel.Text = Apzon.Commons.Properties.Resources.ctx_Cancel;
		ctm_AddRow.Text = Apzon.Commons.Properties.Resources.ctx_Add;
		ctm_Duplicate.Text = Apzon.Commons.Properties.Resources.ctx_Duplicate;
		ctm_Close.Text = Apzon.Commons.Properties.Resources.ctx_Close;
		ctm_SaveAsDraft.Text = Apzon.Commons.Properties.Resources.ctx_SaveAsDraft;
		ctm_addRowLine.Text = Apzon.Commons.Properties.Resources.ctx_AddRow;
		ctm_delRowLine.Text = Apzon.Commons.Properties.Resources.ctx_DeleteRow;
		ctm_LineClose.Text = Apzon.Commons.Properties.Resources.ctx_CloseRow;
		ctm_LineDuplicate.Text = Apzon.Commons.Properties.Resources.ctx_DuplicateRow;
		btnSearch.Text = Apzon.Commons.Properties.Resources.ButtonSearch;
		btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
		btnCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
		drlCopyTo.Text = Apzon.Commons.Properties.Resources.ButtonCopyTo;
		drlCopyFrom.Text = Apzon.Commons.Properties.Resources.ButtonCopyFrom;
		lb_FromDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_FromDate;
		lb_ToDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_ToDate;
		lb_DocStatus.Utext = Apzon.Commons.Properties.Resources.Doc_lb_DocStatus;
		btnSearch.Text = Apzon.Commons.Properties.Resources.ButtonSearch;
		if (Type.GetType("Apzon.Commons.Properties.Documents, Apzon.Commons") != null)
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.Documents, Apzon.Commons"));
			foreach (GridColumnEx column in gvDoc.Columns)
			{
				column.Caption = componentResourceManager.GetString(column.FieldName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? column.Caption;
			}
		}
		if (Type.GetType("Apzon.Commons.Properties.DocumentsLines, Apzon.Commons") != null)
		{
			ComponentResourceManager componentResourceManager2 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.DocumentsLines, Apzon.Commons"));
			foreach (GridColumnEx column2 in gvLine.Columns)
			{
				column2.Caption = componentResourceManager2.GetString(column2.FieldName) ?? column2.Caption;
			}
		}
		gcShipType.Caption = Apzon.Commons.Properties.Resources.Doc_gvLogistic_gcShipType;
		gcShipTo.Caption = Apzon.Commons.Properties.Resources.Doc_gvLogistic_gcShipTo;
		gcConfirmed.Caption = Apzon.Commons.Properties.Resources.Doc_gvLogistic_gcConfirmed;
		lb_Count.Utext = Apzon.Commons.Properties.Resources.lb_Count;
		lb_Total.Utext = Apzon.Commons.Properties.Resources.lb_Total;
	}

	protected virtual async Task VirFormDocument_Load(object sender, EventArgs e)
	{
		_ = 13;
		try
		{
			_originText = Text;
			_httpClient = HttpClientHelperFactory.GetHttpClient(FormObjectType);
			if (!string.IsNullOrEmpty(Fromentry) || OpporId != 0 || OpenFormViewMode == Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Add;
				if (!string.IsNullOrEmpty(Fromentry) && Fromtype == ApzObjectTypes.oDrafts)
				{
					Text += " - Draft";
				}
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
			_bpClientHelper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "Commons");
			if (FormBpType.Equals("S"))
			{
				gcLogistics_BPChCode.AllowChSet = false;
				gcLogistics_BPChCode.Visible = false;
				gcLogisticsBpChannelCntc.AllowChSet = false;
				gcLogisticsBpChannelCntc.Visible = false;
			}
			ucAttachment1.FormController = FormController;
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, new Action<List<DataRow>>(Attachment_BrowseFile));
			int num = 0;
			DataTable dataTable = GlobalData.AdditionalGeneralSettings.Tables[0];
			if (dataTable.IsNotNull() && dataTable.Columns.Contains("SearchDate"))
			{
				num = Function.ParseInt(dataTable.Rows[0]["SearchDate"]);
			}
			if (num < 0)
			{
				num = ((Configuration.SearchDate > 0) ? Configuration.SearchDate : 15);
			}
			txtFromDate.EditValue = DateTime.Today.AddDays(-num);
			txtToDate.EditValue = DateTime.Today.AddDays(num);
			if (painter == null)
			{
				painter = new GridViewExPainter(gvLine);
			}
			rLineSpecPrice.DataSource = WinFuntions.GetDataPriceSource();
			LoadCopyMenu();
			LoadTableFormular();
			Task tf1 = WinFuntions.LoadUdfColumns(gvDoc, FormDocumentTable, TblFormular);
			Task tf2 = WinFuntions.LoadUdfColumns(gvLine, FormLineTable, TblFormular);
			Task tf3 = WinFuntions.LoadUdfColumns(gvPayTerms, "@PMT1", TblFormular);
			gvDoc.ShowLoadingPanel();
			List<Task> list = new List<Task>();
			list.AddRange(new Task[13]
			{
				InitCustomize(),
				LoadSeries(),
				LoadRequestType(),
				GetDepartment(),
				GetBranch(),
				LoadTabAccountingRepLookUpEdit(),
				CreateColumnsDistRule(gvLine),
				WinFuntions.CreateColumnsDistRule(gv_Freight),
				BindDataToVatGroup("O"),
				LoadUomGroup(),
				LoadComboFreight(),
				WinFuntions.SetBranchListControls(gvDoc),
				LoadDataPriceList()
			});
			LoadInventoryUoM();
			LoadCurrency();
			WinFuntions.LoadApprovalStatus(rep_ApprovalStatus);
			list.Add(CreateFreightGridDatasource());
			if (GlobalData.GeneralSetting.EnPriceMod == "Y")
			{
				using DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("Code");
				dataTable2.Columns.Add("Name");
				dataTable2.Rows.Add("N", "Net");
				dataTable2.Rows.Add("G", "Gross");
				dataTable2.AcceptChanges();
				rep_PriceMode.DataSource = dataTable2;
			}
			list.Add(Task.Run(async () =>
			{
				if (string.IsNullOrEmpty(_customizePaymentTerms))
				{
					_customizePaymentTerms = await _httpClient.Commons.CheckExistColumn("ADOC", "U_paymentEntry");
				}
				if (_customizePaymentTerms == "Y" && (DocumentsHelper.TablePayment == null || DocumentsHelper.TablePayment.Columns.Count == 0))
				{
					DocumentsHelper.TablePayment = (await WinFuntions.CreateCloneTabl("@PMT1")).Clone();
				}
				if (_dtTabAccounting == null || _dtTabAccounting.Columns.Count == 0)
				{
					_dtTabAccounting = (await WinFuntions.DocumentLoadTabAccountingColumns()).Clone();
				}
			}));
			rep_CountryOrg.DataSource = GlobalData.CountryOrg;
			await Task.WhenAll(list);
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "Name",
				ValueMember = "CntctCode"
			};
			if (_dtContact.IsNotNull())
			{
				repositoryItemLookUpEdit.DataSource = _dtContact.Copy();
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", 20, "Name"));
			}
			gcDoc_CntctCode.ColumnEdit = repositoryItemLookUpEdit;
			ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			cbxStatus.SelectedIndex = 1;
			if (!string.IsNullOrEmpty(Fromentry))
			{
				SetWddStatusText();
				Task task = CopyFromEntry();
				Task t2 = LoadDataControl();
				Task t3 = LoadTabAccounting(Function.ParseInt(Fromentry), isCopyFrom: true, Fromtype);
				Task t4 = GetPaymentTerm(Fromtype, Function.ParseInt(Fromentry));
				Task t5 = GetFreight(GetFromFreightTableName(), Function.ParseInt(Fromentry));
				await task;
				if (Fromtype == ApzObjectTypes.oDrafts)
				{
					ucAttachment1.LoadAttachments(Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry")));
				}
				if (Fromtype == ApzObjectTypes.oPurchaseRequest || Fromtype == ApzObjectTypes.oPurchaseQuotations)
				{
					gcDoc_CardCode.OptionsColumn.ReadOnly = false;
				}
				await t3;
				if (FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oReserveInvoices || FormObjectType == ApzObjectTypes.oAPReserveInvoice || FormObjectType == ApzObjectTypes.oCreditNotes || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes)
				{
					DataTable dataTable3 = grd_Accounting.DataSource as DataTable;
					if (dataTable3.IsNotNull())
					{
						PaymentTermInfo = _httpClient.PaymentTermSetup.GetPaymentTermInfo(Function.ParseInt(dataTable3.Rows[0]["GroupNum"]));
						if (PaymentTermInfo.IsNotNull())
						{
							PaymentTermInfo.Rows[0]["PayDuMonth"] = dataTable3.Rows[0]["PayDuMonth"];
							PaymentTermInfo.Rows[0]["ExtraMonth"] = dataTable3.Rows[0]["ExtraMonth"];
							PaymentTermInfo.Rows[0]["ExtraDays"] = dataTable3.Rows[0]["ExtraDays"];
							CalculateDueDate();
						}
					}
				}
				await Task.WhenAll(t2, t4, t5);
				if (Fromtype == ApzObjectTypes.oDrafts)
				{
					_onlyChangeRemark = true;
				}
			}
			else if (OpporId != 0)
			{
				DocumentTable documentTable;
				using (HttpClientHelper<DocumentTable> docTableHelper = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, FormController))
				{
					documentTable = await docTableHelper.GetAsync("GetOpportunity", new
					{
						id = OpporId
					});
				}
				if (documentTable != null && documentTable.Rows.Count > 0)
				{
					documentTable[0].DocEntry = NewDocEntry;
				}
				grdDoc.DataSource = documentTable;
				grdDoc.RefreshDataSource();
				_originalDocTable = documentTable.Copy();
				DocumentLineTable docLine;
				using (HttpClientHelper<DocumentLineTable> docLineTableHelper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, FormController))
				{
					docLine = await docLineTableHelper.GetAsync("GetOpportunityDetail", new
					{
						id = OpporId
					});
				}
				await WinFuntions.ChangeTypeUdf(docLine, "QUT1");
				docLine.ChangeColumnDataType("Quantity", typeof(double));
				docLine.ChangeColumnDataType("LineNum", typeof(int));
				docLine.ChangeColumnDataType("Price", typeof(double));
				docLine.ChangeColumnDataType("DiscPrcnt", typeof(double));
				docLine.ChangeColumnDataType("Rate", typeof(double));
				docLine.ChangeColumnDataType("LineTotal", typeof(double));
				docLine.ChangeColumnDataType("TotalFrgn", typeof(double));
				docLine.ChangeColumnDataType("LineVat", typeof(double));
				docLine.ChangeColumnDataType("PriceBefDi", typeof(double));
				docLine.ChangeColumnDataType("ShipDate", typeof(DateTime));
				docLine.ChangeColumnDataType("OpenQty", typeof(double));
				docLine.ChangeColumnDataType("BaseOpnQty", typeof(double));
				docLine.ChangeColumnDataType("VatSumFrgn", typeof(double));
				grdLine.DataSource = docLine;
				grdLine.RefreshDataSource();
				AddNewBlankLine();
			}
			else
			{
				switch (OpenFormViewMode)
				{
				case Enums.FormViewModeType.Add:
					await AddDocClick();
					if (OpenFormDocument.IsNotNull() || OpenFormLines.IsNotNull())
					{
						await Task.WhenAll(tf1, tf2, tf3);
						gvDoc.MappingSource(OpenFormDocument, MappingDocument, null, null, null, multirow: false);
						gvDoc.HideLoadingPanel();
						gvLine.ShowLoadingPanel();
						gvLine.MappingSource(OpenFormLines, MappingLines, OpenFormBatchSerial, MappingBatchSerial, ucBatchAndSerial1);
						gvLine.HideLoadingPanel();
						ucBatchAndSerial1.SetVisibleAfterContextAction();
						ExtenseionMethods.ShowHideSumColumn(gvLine, Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")), gv_Freight);
					}
					break;
				case Enums.FormViewModeType.View:
					FilterHeader = await WinFuntions.ShowFilterSetting(FormDocumentTable);
					FilterLine = await WinFuntions.ShowFilterSetting(FormLineTable);
					if (DocEntrySearch > 0)
					{
						await GetDoc();
					}
					else
					{
						cbxStatus.SelectedIndex = 1;
						await RefreshData();
					}
					_onlyChangeRemark = true;
					break;
				}
			}
			drlCopyFrom.Enabled = false;
			_OpenChooseCFL = true;
			await Task.WhenAll(tf1, tf2, tf3);
			CustomFormSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvDoc.HideLoadingPanel();
		}
	}

	private async Task CopyHeader()
	{
		DocumentTable documentTable = await _httpClient.Documents.CopyHeader(Fromtype, FormObjectType, Fromentry);
		if (documentTable.IsNotNull())
		{
			int bplId = Function.ParseInt(documentTable.Rows[0]["BPLId"]);
			int defautSeries = GetDefautSeries(bplId);
			documentTable.Rows[0]["Series"] = defautSeries;
		}
		_isLoading = true;
		grdDoc.DataSource = documentTable;
		grdDoc.RefreshDataSource();
		_isLoading = false;
		_originalDocTable = documentTable.Copy();
		grdLogistic.DataSource = documentTable;
		grdLogistic.RefreshDataSource();
		if (Fromtype == ApzObjectTypes.oDrafts)
		{
			try
			{
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oDrafts);
				_currentRefDoc = httpClient.Documents.GetReferencedDocument(Function.ParseInt(Fromentry));
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
		}
	}

	private async Task CopyBatchSerial()
	{
		ucBatchAndSerial1.FromReserve = Fromtype == ApzObjectTypes.oAPReserveInvoice || Fromtype == ApzObjectTypes.oReserveInvoices;
		if (!ucBatchAndSerial1.CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) || !ucBatchAndSerial1.CopyObjectNotBatchAndSerial[FormObjectType].Contains(Fromtype))
		{
			DataTable dataTable = await Task.Run(() => _httpClient.Documents.CopyBatchSerial(Fromentry, Fromtype));
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				string[] columnNames = (from DataColumn t in dataTable2.Columns
					select t.ColumnName).ToArray();
				dataTable = dataTable.DefaultView.ToTable("Source", distinct: false, columnNames);
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
				if (dataRow != null)
				{
					dataTable2.Rows.Remove(dataRow);
					dataTable2.AcceptChanges();
				}
				foreach (DataRow row in dataTable.Rows)
				{
					DataRow dataRow3 = dataTable2.NewRow();
					dataRow3.ItemArray = row.ItemArray;
					dataTable2.Rows.Add(dataRow3);
				}
				ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
			}
			DataTable dataTable3 = await Task.Run(() => _httpClient.BinLocMasterDataClient.GetDataCopyBinAllowcationLine(Fromentry, Fromtype));
			if (dataTable3.IsNotNull())
			{
				ucBatchAndSerial1.DataSourceBinAllowLine = dataTable3;
			}
		}
		ucBatchAndSerial1.SetFormViewMode((Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")) > 0) ? Enums.FormViewModeType.Update : Enums.FormViewModeType.Add);
	}

	private async Task CopyFromEntry()
	{
		ApzObjectTypes toType = FormObjectType;
		if (DownPaymentType == Enums.DownPaymentType.dptInvoice)
		{
			if (FormObjectType == ApzObjectTypes.oDownPayments)
			{
				toType = ApzObjectTypes.oDownPaymentInvoice;
			}
			else if (FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
			{
				toType = ApzObjectTypes.oPurchaseDownPaymentInvoice;
			}
		}
		Task<DocumentLineTable> task = _httpClient.Documents.Copy(Fromtype, toType, Fromentry);
		Task<DocumentTable> t2 = _httpClient.Documents.CopyHeader(Fromtype, toType, Fromentry);
		Task t3 = CopyBatchSerial();
		Task t4 = Task.Run(() =>
		{
			if (Fromtype == ApzObjectTypes.oDrafts)
			{
				try
				{
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oDrafts);
					_currentRefDoc = httpClient.Documents.GetReferencedDocument(Function.ParseInt(Fromentry));
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
		});
		DocumentLineTable docLine = await task;
		if (Fromtype == ApzObjectTypes.oBlanketAgreement)
		{
			foreach (DataRow row in docLine.Rows)
			{
				row["VatGroup"] = row["VatGourpSa"];
				if (Function.ParseInt(row["UomEntry"]) < 0)
				{
					row["UomCode"] = row["SUoMCode"];
					row["unitMsr"] = row["SalUnitMsr"];
					row["NumPerMsr"] = row["NumInSale"];
					row["InvQty"] = Function.ParseDouble(row["Quantity"]) * Function.ParseDouble(row["NumPerMsr"]);
					row["PriceBefDi"] = Function.ParseDouble(row["PriceBefDi"]) * Function.ParseDouble(row["NumPerMsr"]);
				}
				else
				{
					row["unitMsr"] = row["InvntryUom"];
				}
			}
		}
		else if (Fromtype == ApzObjectTypes.oPurchaseBlanketAgreement)
		{
			foreach (DataRow row2 in docLine.Rows)
			{
				row2["VatGroup"] = row2["VatGroupPu"];
				if (Function.ParseInt(row2["UomEntry"]) < 0)
				{
					row2["UomCode"] = row2["PUoMCode"];
					row2["unitMsr"] = row2["BuyUnitMsr"];
					row2["NumPerMsr"] = row2["NumInBuy"];
					row2["InvQty"] = Function.ParseDouble(row2["Quantity"]) * Function.ParseDouble(row2["NumPerMsr"]);
					row2["PriceBefDi"] = Function.ParseDouble(row2["PriceBefDi"]) * Function.ParseDouble(row2["NumPerMsr"]);
				}
				else
				{
					row2["unitMsr"] = row2["InvntryUom"];
				}
			}
		}
		WinFuntions.GenerateBOMItems(ref docLine);
		WinFuntions.SetBOMTreeType(ref docLine, ref LstBomChilds);
		DocumentTable documentTable = await t2;
		_isLoading = true;
		grdDoc.DataSource = documentTable;
		grdDoc.RefreshDataSource();
		LoadSeriesByPostingDate();
		_isLoading = false;
		_originalDocTable = documentTable.Copy();
		ExtenseionMethods.ShowHideSumColumn(gvLine, Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")), gv_Freight);
		grdLogistic.DataSource = documentTable;
		grdLogistic.RefreshDataSource();
		gvDoc.HideLoadingPanel();
		gvLine.ShowLoadingPanel();
		await t3;
		grdLine.DataSource = docLine;
		grdLine.RefreshDataSource();
		if (Fromtype != ApzObjectTypes.oDrafts)
		{
			SetTotalAfterCopy(Fromtype);
		}
		LstLineEdit = grdLine.DataSource as DocumentLineTable;
		gvLine.HideLoadingPanel();
		await t4;
	}

	private void SetTotalAfterCopy(ApzObjectTypes fromType)
	{
		try
		{
			bool flag = true;
			int num = -1;
			ucBatchAndSerial1.RaiseLineItemChanged = false;
			for (int i = 0; i < gvLine.RowCount; i++)
			{
				if (Function.ToString(gvLine.GetRowCellValue(i, "ItemCode")) != "" && Function.ToString(gvLine.GetRowCellValue(i, "LineType")) == "R")
				{
					num = i;
					if (Function.ParseDecimal(gvLine.GetRowCellValue(i, "LineTotal")) == 0m)
					{
						gvLine.SetRowCellValue(i, "PriceBefDi", gvLine.GetRowCellValue(i, "PriceBefDi"));
						flag = false;
					}
					if (fromType == ApzObjectTypes.oBlanketAgreement || fromType == ApzObjectTypes.oPurchaseBlanketAgreement)
					{
						gvLine.SetRowCellValue(i, "VatGroup", gvLine.GetRowCellValue(i, "VatGroup"));
					}
				}
			}
			if (flag && num >= 0)
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
				gvLine.SetRowCellValue(num, (text == GlobalData.MainCurrency) ? "LineTotal" : "TotalFrgn", gvLine.GetRowCellValue(num, (text == GlobalData.MainCurrency) ? "LineTotal" : "TotalFrgn"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		ucBatchAndSerial1.RaiseLineItemChanged = true;
	}

	protected override void XtraFormEx_Shown(object sender, EventArgs e)
	{
		base.XtraFormEx_Shown(sender, e);
		try
		{
			if (!OpenFormLines.IsNotNull() || !MappingLines.Any((ContextActionMappingImpl m) => m.TargetField == "LineVat" || m.TargetField == "VatSumFrgn"))
			{
				return;
			}
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
			string targetField = ((text == GlobalData.MainCurrency) ? "LineVat" : "VatSumFrgn");
			string sourceField = MappingLines.FirstOrDefault((ContextActionMappingImpl m) => m.TargetField == targetField).SourceField;
			for (int num = 0; num < OpenFormLines.Rows.Count; num++)
			{
				if (gvLine.GetRowHandle(num) >= 0)
				{
					gvLine.SetRowCellValue(num, targetField, OpenFormLines.Rows[num][sourceField]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void SetWddStatusText()
	{
		string text;
		switch (Function.ToString(gvDoc.GetFocusedRowCellValue("WddStatus")))
		{
		case "W":
			text = " [Pending]";
			break;
		case "Y":
			text = " [Approved]";
			break;
		case "N":
			text = " [Rejected]";
			break;
		case "P":
			text = " [Generated]";
			break;
		case "A":
			text = " [Generated by Authorizer]";
			break;
		case "C":
			text = " [Canceled]";
			break;
		default:
			text = "";
			if (Cancellation)
			{
				text += " - Cancellation";
			}
			break;
		}
		Text = $"{Text}{text}";
	}

	protected virtual async Task GetBranch()
	{
	}

	protected virtual async Task GetDepartment()
	{
	}

	protected virtual async Task LoadRequestType()
	{
	}

	protected virtual void targetDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				TargetDocument();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void baseDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				BaseDocument();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void baseDocumentToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			BaseDocument();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gvDoc_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				ctxDoc.Visible = FormMode != Enums.FormViewModeType.Busy;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gvLine_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				ctxLine.Visible = FormMode != Enums.FormViewModeType.Busy;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void newToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				NewActivity();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (sender is GridControl { FocusedView: GridViewEx focusedView } && e.KeyData == Keys.Tab && focusedView.FocusedColumn.FieldName.ToUpper() == "OWNERNAME")
			{
				WinFuntions.LoadOwnerCode(gvDoc);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grdLine_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (sender as GridControl).FocusedView as GridViewEx;
			if (e.KeyData != Keys.Tab || !(gridViewEx.FocusedColumn.FieldName.ToUpper() != "LINETYPE"))
			{
				return;
			}
			if (Function.ToString(gvLine.GetFocusedRowCellValue("LineType")) == "T")
			{
				gvLine.FocusedRowHandle++;
				gvLine.FocusedColumn = gvLine.Columns[0];
			}
			else
			{
				if (!(gridViewEx.FocusedColumn.FieldName.ToUpper() == "ITEMCODE") || string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"))) || !_OpenChooseCFL)
				{
					return;
				}
				int focusedDataSourceRowIndex = gvLine.GetFocusedDataSourceRowIndex();
				if (focusedDataSourceRowIndex > -1)
				{
					string text = Function.ToString(gvLine.EditingValue);
					if (string.IsNullOrEmpty(text))
					{
						ChooseFormListItemCode(text, focusedDataSourceRowIndex);
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

	private void ctm_BatchSerialNumbers_Click(object sender, EventArgs e)
	{
		try
		{
			split_LineBatchAndSerial.PanelVisibility = ((split_LineBatchAndSerial.PanelVisibility == SplitPanelVisibility.Both) ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void btnAdd_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				else
				{
					await SaveData();
				}
			}
			else
			{
				OkToClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void btRefresh_Click()
	{
		await ClickToRefresh();
	}

	private async Task ClickToRefresh(bool reloadADoc = true)
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				txtNoOfTrans.Text = string.Empty;
				txtBusinessPartner.Text = string.Empty;
				ucBatchAndSerial1.ClearLineDataSource();
				split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Panel1;
				if (FormMode != Enums.FormViewModeType.Add && Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum")) > 0)
				{
					txtNoOfTrans.Text = (reloadADoc ? Function.ToString(gvDoc.GetFocusedRowCellValue("DocNum")) : "");
					DocEntrySearch = (reloadADoc ? Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")) : 0);
				}
				FormMode = Enums.FormViewModeType.View;
				DocumentTable clone = DocumentsHelper.Document.GetClone();
				grdDoc.DataSource = clone;
				grdDoc.RefreshDataSource();
				LstLineEdit = new DocumentLineTable();
				grdLine.DataSource = LstLineEdit;
				grdLine.RefreshDataSource();
				await RefreshData();
				drlCopyFrom.Enabled = false;
				CustomFormSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void SetEnableControlSearch(bool isEnable)
	{
		DateEditEx dateEditEx = txtFromDate;
		DateEditEx dateEditEx2 = txtToDate;
		ButtonEditEx buttonEditEx = txtBusinessPartner;
		TextEditEx textEditEx = txtNoOfTrans;
		SimpleButtonEx simpleButtonEx = btnSearch;
		System.Windows.Forms.ComboBox comboBox = cbxStatus;
		LookUpEditEx lookUpEditEx = look_ReqType;
		bool flag = (txtRequester.Enabled = isEnable);
		bool flag3 = (lookUpEditEx.Enabled = flag);
		bool flag5 = (comboBox.Enabled = flag3);
		bool flag7 = (simpleButtonEx.Enabled = flag5);
		bool flag9 = (textEditEx.Enabled = flag7);
		bool flag11 = (buttonEditEx.Enabled = flag9);
		bool enabled = (dateEditEx2.Enabled = flag11);
		dateEditEx.Enabled = enabled;
	}

	protected virtual async void btnSearch_Click(object sender, EventArgs e)
	{
		if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
		{
			FormMode = Enums.FormViewModeType.View;
			SetEnableControlSearch(isEnable: false);
			try
			{
				ResetControl();
				await GetDoc();
				NewDocEntry = 0;
				gcDoc_CardCode.OptionsColumn.ReadOnly = true;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
			SetEnableControlSearch(isEnable: true);
		}
	}

	private void cal_TotalFC_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(spin_Count.EditValue);
			double num2 = Function.ParseDouble(cal_TotalFC.EditValue);
			if (num > 0)
			{
				for (int i = 0; i < gvPayTerms.RowCount; i++)
				{
					gvPayTerms.SetRowCellValue(i, "U_TotalFC", num2);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rILSalesEmp_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(Function.ToString(gvDoc.GetFocusedRowCellValue("OwnerName")).Trim() != "") && !_isLoading && sender is GridLookUpEdit gridLookUpEdit)
			{
				DataTable dataTable = await ExtenseionMethods.GetEmployeeCodeBySlpCode(Function.ParseInt(((DataRowView)gridLookUpEdit.GetSelectedDataRow()).Row["SlpCode"]));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					gvDoc.SetFocusedRowCellValue("OwnerCode", Function.ParseInt(dataTable.Rows[0]["OwnerCode"]));
					gvDoc.SetFocusedRowCellValue("OwnerName", Function.ToString(dataTable.Rows[0]["OwnerName"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void txtBusinessPartner_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataRow choosenCardCode = GetChoosenCardCode(txtBusinessPartner.Text);
			if (choosenCardCode != null && choosenCardCode.IsExistsColumn("CardCode"))
			{
				txtBusinessPartner.EditValue = Function.ToString(choosenCardCode["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rlpBillTo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int dataSourceRowIndex = rBillTo.GetDataSourceRowIndex("Address", gv_Logistic.EditingValue);
			string value = Function.ToString(rBillTo.GetDataSourceValue("Street", dataSourceRowIndex)).Replace("\r", "\n");
			gv_Logistic.SetFocusedRowCellValue(gv_Logistic.Columns.ColumnByFieldName("Address"), value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rlpShipTo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int dataSourceRowIndex = rShipTo.GetDataSourceRowIndex("Address", gv_Logistic.EditingValue);
			string value = Function.ToString(rShipTo.GetDataSourceValue("Street", dataSourceRowIndex)).Replace("\r", "\n");
			gv_Logistic.SetFocusedRowCellValue(gv_Logistic.Columns.ColumnByFieldName("Address2"), value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rlpShipType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Logistic.SetFocusedRowCellValue(gv_Logistic.Columns.ColumnByFieldName("TransportationCode"), gv_Logistic.EditingValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected virtual void gv_Logistic_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (_clone != null)
			{
				_clone.Dispose();
				_clone = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void spin_Count_Validating(object sender, CancelEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (Function.ParseInt(spin_Count.EditValue) < 0)
			{
				spin_Count.EditValue = 0;
			}
			if (grdPaymentTerms.DataSource != null && grdPaymentTerms.DataSourceTable.Rows.Count > 0)
			{
				if (Function.ParseInt(spin_Count.EditValue) < grdPaymentTerms.DataSourceTable.Rows.Count)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangePaymentCount, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						LoadPaymentTerm();
					}
					else
					{
						e.Cancel = true;
					}
				}
				else
				{
					LoadPaymentTerm();
				}
			}
			else
			{
				LoadPaymentTerm();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual void spin_Count_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(spin_Count.EditValue);
			double num2 = Function.ParseDouble(cal_Total.EditValue);
			if (num > 0)
			{
				for (int i = 0; i < gvPayTerms.RowCount; i++)
				{
					gvPayTerms.SetRowCellValue(i, "Total", num2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
	{
	}

	protected virtual void cal_Total_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(spin_Count.EditValue);
			double num2 = Function.ParseDouble(cal_Total.EditValue);
			if (num > 0)
			{
				for (int i = 0; i < gvPayTerms.RowCount; i++)
				{
					gvPayTerms.SetRowCellValue(i, "U_Total", num2);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rtxtWhseCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvLine.GetFocusedRowCellValue("LineStatus"));
			string text2 = Function.ToString(gvLine.GetFocusedRowCellValue("WhsCode"));
			if (e.Button.Index == 0)
			{
				if (gvLine.Columns["WhsCode"].OptionsColumn.ReadOnly)
				{
					return;
				}
				if (gvLine.ActiveEditor != null)
				{
					text2 = Function.ToString(gvLine.ActiveEditor.EditValue);
				}
				if (text.ToUpper() != "C")
				{
					ChooseFrmListWhseCode(text2);
				}
			}
			if (e.Button.Index == 1 && !string.IsNullOrEmpty(text2))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rtxtItemcode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvLine.GetFocusedRowCellValue("LineType"));
			if (text == "R")
			{
				if (e.Button.Index == 0)
				{
					if (gvLine.Columns["ItemCode"].OptionsColumn.ReadOnly)
					{
						return;
					}
					_OpenChooseCFL = false;
					string value = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
					int focusedDataSourceRowIndex = gvLine.GetFocusedDataSourceRowIndex();
					if (focusedDataSourceRowIndex > -1)
					{
						string text2 = Function.ToString(gvLine.GetFocusedRowCellValue("LineStatus"));
						int num = Function.ParseInt(gvLine.GetFocusedRowCellValue("TargetType"));
						if (!_OpenChooseCFL && !text2.Equals("C") && num <= 0)
						{
							if (!FormObjectType.Equals(ApzObjectTypes.oPurchaseRequest) && string.IsNullOrEmpty(value))
							{
								WinCommons.Warning(Enums.MessageCode.YouHaveChooseWithColumnCaption, "", gvDoc.Columns["CardCode"].Caption);
							}
							string itemCode = Function.ToString(gvLine.GetRowCellValue(focusedDataSourceRowIndex, "ItemCode"));
							if (gvLine.ActiveEditor != null)
							{
								itemCode = Function.ToString(gvLine.ActiveEditor.EditValue);
							}
							ChooseFormListItemCode(itemCode, focusedDataSourceRowIndex);
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.YouHaveNotChooseDocument, "");
					}
				}
				if (e.Button.Index == 1)
				{
					ExtenseionMethods.LoadBoMFromGridItem(Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode")), MdiParent);
				}
			}
			else if (text == "T")
			{
				frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
				frm_RowTextDetails2.RowText = Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode"));
				if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
				{
					gvLine.SetRowCellValue(gvLine.FocusedRowHandle, "ItemCode", frm_RowTextDetails2.RowText);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Line_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			if (sender is GridViewEx gridViewEx && gridViewEx.FocusedColumn.FieldName == "CntctCode" && gridViewEx.ActiveEditor is LookUpEdit)
			{
				LookUpEdit lookUpEdit = (LookUpEdit)gridViewEx.ActiveEditor;
				if (lookUpEdit != null)
				{
					DataTable table = lookUpEdit.Properties.DataSource as DataTable;
					_clone = new DataView(table);
					_clone.RowFilter = "[CardCode] = '" + Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode")) + "'";
					lookUpEdit.Properties.DataSource = _clone;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Line_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (_clone != null)
			{
				_clone.Dispose();
				_clone = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Line_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus")) != "C" && e.RowHandle == gvLine.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvLine_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "LineStatus")) == "C" && (!e.Column.FieldName.StartsWith("U_") || !(_documentSetting.EnblUpdUDF == "Y")))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
				return;
			}
			if (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "TargetType")) > 0 && e.Column.FieldName != "Quantity" && e.Column.FieldName != "InvQty" && (!e.Column.FieldName.StartsWith("U_") || !(_documentSetting.EnblUpdUDF == "Y")))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
				return;
			}
			string text = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "LineType"));
			if (text != "R")
			{
				e.Appearance.BackColor = StyleColor.GridSubtotal;
				if (text == "S")
				{
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
			}
			if (!(e.Column.FieldName != "ItemCode"))
			{
				string text2 = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "TreeType"));
				string text3 = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "BOMTreeType"));
				int num = Function.ParseInt(text3);
				if (text3.Trim() != "0" && num == 0)
				{
					num = -1;
				}
				if (text2 == "I" && !string.IsNullOrWhiteSpace(text3) && num > -1)
				{
					e.Appearance.ForeColor = GridColor.GridReadOnly;
				}
				else if (text2 == "S")
				{
					e.Appearance.ForeColor = Color.Brown;
				}
				if (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "U_PromotionType")) > 0)
				{
					e.Appearance.ForeColor = Color.Blue;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void cmtLineDuplicate_Click(object sender, EventArgs e)
	{
		try
		{
			LstLineEdit = grdLine.DataSource as DocumentLineTable;
			int focusedRowHandle = gvLine.FocusedRowHandle;
			int focusedDataSourceRowIndex = gvLine.GetFocusedDataSourceRowIndex();
			if (focusedRowHandle < 0 || string.IsNullOrEmpty(Function.ToString(gvLine.GetRowCellValue(focusedRowHandle, "ItemCode"))) || LstLineEdit == null)
			{
				return;
			}
			DocumentLineRow newRow = LstLineEdit.GetNewRow();
			newRow.ItemArray = LstLineEdit[focusedDataSourceRowIndex].ItemArray;
			newRow.LineNum = LstLineEdit.GetNewLineNum(-1);
			newRow.TargetType = -1;
			newRow.TrgetEntry = 0;
			newRow.BaseType = -1;
			newRow.BaseEntry = 0;
			newRow.BaseLine = 0;
			newRow.LineStatusName = "O";
			newRow.RemainingOpenQuantity = newRow.Quantity;
			newRow.OpenInventoryQuantity = newRow.InventoryQuantity;
			newRow.DelivrdQty = 0.0;
			LstLineEdit.Insert(focusedDataSourceRowIndex + 1, newRow);
			AddNewBlankLine();
			LstLineEdit.AcceptChanges();
			ExtenseionMethods.ReloadSubTotal(gvLine, focusedRowHandle);
			int num = GlobalData.GeneralSetting.SumDec.Value;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")).ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = ((currencyCodesImpl.Decimals == -1) ? num : currencyCodesImpl.Decimals);
				}
			}
			ExtenseionMethods.SetDocTotal(gvDoc, grdLine, num, FormObjectType, gv_Freight);
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected virtual void cmtLineClose_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gvLine.FocusedRowHandle;
			if (gvLine.IsRowSelected(focusedRowHandle))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				int focusedDataSourceRowIndex = gvLine.GetFocusedDataSourceRowIndex();
				LstLineEdit = grdLine.DataSource as DocumentLineTable;
				if (LstLineEdit != null && LstLineEdit[focusedDataSourceRowIndex].LineNum != -1)
				{
					LstLineEdit[focusedDataSourceRowIndex].LineStatusName = "C";
					LstLineEdit[focusedDataSourceRowIndex].RemainingOpenQuantity = 0.0;
					LstLineEdit[focusedDataSourceRowIndex]["OpenInvQty"] = 0;
					if (!LstLineEdit.GetEnumerable().Any((DocumentLineRow x) => x.LineStatusName != "C" && x.LineNum != -1))
					{
						gvDoc.SetFocusedRowCellValue("DocStatus", "C");
						gvDoc.SetFocusedRowCellValue("DocStatusName", "Closed");
					}
				}
				grdLine.DataSource = LstLineEdit;
				grdLine.RefreshDataSource();
				if (FormMode != Enums.FormViewModeType.Add)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.ChooseDocument, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected virtual void cmt_delRowLine_Click(object sender, EventArgs e)
	{
		try
		{
			ucBatchAndSerial1.DeleteBatchAndSerialByLineNum();
			ucBatchAndSerial1.BeginUpdateLine();
			if (WinFuntions.DeleteBomChildsItem(gvLine, gvLine.FocusedRowHandle, ucBatchAndSerial1))
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				_onlyChangeRemark = false;
				ExtenseionMethods.ReloadSubTotal(gvLine, gvLine.FocusedRowHandle);
				int num = GlobalData.GeneralSetting.SumDec.Value;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")).ToUpper()));
					if (currencyCodesImpl != null)
					{
						num = ((currencyCodesImpl.Decimals == -1) ? num : currencyCodesImpl.Decimals);
					}
				}
				ExtenseionMethods.SetDocTotal(gvDoc, grdLine, num, FormObjectType, gv_Freight);
				ExtenseionMethods.ReloadDocumentVisOrderAndLineNum(grdLine.DataSource as DocumentLineTable);
			}
			ucBatchAndSerial1.EndUpdateLine();
			ucBatchAndSerial1.SetFocusLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected virtual void cmt_addRowLine_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() == null)
			{
				WinCommons.Warning(Enums.MessageCode.None, "Choose a header record first!");
				return;
			}
			int focusedRowHandle = gvLine.FocusedRowHandle;
			string value = Function.ToString(gvLine.GetRowCellValue(focusedRowHandle, "ItemCode"));
			string text = Function.ToString(gvLine.GetRowCellValue(focusedRowHandle, "LineType"));
			if (!string.IsNullOrEmpty(value) && text == "R")
			{
				AddNewBlankLine(focusedRowHandle);
			}
			else
			{
				AddNewBlankLine();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void rtxtCardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (_formMode != Enums.FormViewModeType.Add)
				{
					return;
				}
				string cardcode = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "CardCode"));
				if (gvDoc.ActiveEditor != null)
				{
					cardcode = Function.ToString(gvDoc.ActiveEditor.EditValue);
				}
				ChooseFormListCardCode(cardcode);
			}
			if (e.Button.Index == 1)
			{
				int focusedRowHandle2 = gvDoc.FocusedRowHandle;
				string text = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle2, "CardCode"));
				if (!string.IsNullOrEmpty(text))
				{
					frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
					frm_BusinessMasterData2._copyF_CodeCustomer = text;
					frm_BusinessMasterData2.MdiParent = MdiParent;
					frm_BusinessMasterData2.Show();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!_isSearching && !_isLoading && FormMode != Enums.FormViewModeType.Add)
			{
				disableCopyTobyRejected();
				_onlyChangeRemark = true;
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("CANCELED"));
				if (text == "C" || text == "Y")
				{
					Text = Text.Replace(" - Cancellation", "") + " - Cancellation";
				}
				else
				{
					Text = Text.Replace(" - Cancellation", "");
				}
				FormMode = Enums.FormViewModeType.Busy;
				await LoadChildData();
				FormMode = Enums.FormViewModeType.View;
				if (drlCopyTo.Enabled && (FormObjectType == ApzObjectTypes.oOrders || FormObjectType == ApzObjectTypes.oPurchaseOrders) && (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update))
				{
					string text2 = Function.ToString(gvDoc.GetFocusedRowCellValue("Confirmed"));
					drlCopyTo.Enabled = text2.Equals("Y");
				}
				if (FormMode != Enums.FormViewModeType.Add)
				{
					_showForLandedCost = true;
				}
				IsRefreshDataSource = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Doc_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (_clone != null)
			{
				_clone.Dispose();
				_clone = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, gvDoc.Columns.ColumnByFieldName("DocStatus"))).Equals("C") && !e.Column.FieldName.StartsWith("U_"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.RowHandle == gvDoc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "CntctCode")
			{
				return;
			}
			string arg = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "CardCode"));
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "Name",
				ValueMember = "CntctCode"
			};
			if (_dtContact.IsNotNull())
			{
				DataRow[] array = _dtContact.Select($"CardCode='{arg}'");
				DataTable dataTable;
				if (array.Length != 0)
				{
					dataTable = array.CopyToDataTable();
				}
				else
				{
					dataTable = new DataTable();
					dataTable.Columns.Add("CntctCode");
					dataTable.Columns.Add("Name");
				}
				dataTable.Rows.InsertAt(dataTable.NewRow(), 0);
				dataTable.AcceptChanges();
				repositoryItemLookUpEdit.DataSource = dataTable;
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", 20, "Name"));
				repositoryItemLookUpEdit.Name = "thanhnv" + e.RowHandle;
				e.RepositoryItem = repositoryItemLookUpEdit;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctxDoc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus")).ToUpper() == "O")
			{
				if (FormMode != Enums.FormViewModeType.Add)
				{
					if (FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes || FormObjectType == ApzObjectTypes.oCreditNotes)
					{
						ctm_Close.Enabled = false;
					}
					else
					{
						ctm_Close.Enabled = true;
					}
					ctm_AddRow.Enabled = true;
					ctm_baseDocument.Enabled = true;
					ctm_Cancel.Enabled = true;
					ctm_Duplicate.Enabled = true;
					ctm_SaveAsDraft.Enabled = false;
					ctm_RelatedActivities.Enabled = true;
					if ((FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes || FormObjectType == ApzObjectTypes.oPurchaseInvoices) && _showForLandedCost)
					{
						if (CheckOpenForLandedCost())
						{
							ctm_ForLandedCost.Text = "Open for Landed Costs";
							ctm_ForLandedCost.Enabled = true;
							ctm_ForLandedCost.Visible = true;
						}
						else
						{
							ctm_ForLandedCost.Text = "Close for Landed Costs";
							ctm_ForLandedCost.Enabled = true;
							ctm_ForLandedCost.Visible = true;
						}
					}
					else
					{
						ctm_ForLandedCost.Enabled = false;
						ctm_ForLandedCost.Visible = false;
					}
				}
				else
				{
					ctm_AddRow.Enabled = false;
					ctm_Cancel.Enabled = false;
					ctm_Close.Enabled = false;
					ctm_Duplicate.Enabled = false;
					ctm_SaveAsDraft.Enabled = true;
					ctm_RelatedActivities.Enabled = false;
					ctm_ForLandedCost.Visible = false;
				}
			}
			else
			{
				ctm_Close.Enabled = false;
				ctm_Cancel.Enabled = Function.ToString(gvDoc.GetFocusedRowCellValue("CANCELED")) == "N";
				ctm_AddRow.Enabled = true;
				ctm_SaveAsDraft.Enabled = false;
				if (gvDoc.GetFocusedDataRow() != null && (FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes || FormObjectType == ApzObjectTypes.oPurchaseInvoices) && _showForLandedCost)
				{
					if (CheckOpenForLandedCost())
					{
						ctm_ForLandedCost.Text = "Open for Landed Costs";
						ctm_ForLandedCost.Enabled = true;
						ctm_ForLandedCost.Visible = true;
					}
					else
					{
						ctm_ForLandedCost.Text = "Close for Landed Costs";
						ctm_ForLandedCost.Enabled = true;
						ctm_ForLandedCost.Visible = true;
					}
				}
				else
				{
					ctm_ForLandedCost.Enabled = false;
					ctm_ForLandedCost.Visible = false;
				}
			}
			ctm_RemoveDoc.Visible = gvDoc.GetFocusedDataRow() != null && "Y" == Function.ToString(gvDoc.GetFocusedRowCellValue("CANCELED")) && (FormObjectType == ApzObjectTypes.oQuotations || FormObjectType == ApzObjectTypes.oPurchaseRequest || FormObjectType == ApzObjectTypes.oPurchaseQuotations);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctxLine_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvLine.GetFocusedRowCellValue("ManBatchSerial"));
			ctm_BatchSerialNumbers.Enabled = !Function.ToString(gvLine.GetFocusedRowCellValue("NoInvtryMv")).Equals("Y") && (text.Equals("S") || text.Equals("B")) && (ucBatchAndSerial1.TransType != Enums.BatchSerialTransType.OutAlocate || Function.ParseDouble(gvLine.GetFocusedRowCellValue("Quantity")) > 0.0);
			ctm_BatchSerialNumbers.Text = ((split_LineBatchAndSerial.PanelVisibility == SplitPanelVisibility.Both) ? "Hide Batch/Serial Numbers" : "Batch/Serial Numbers");
			string text2 = Function.ToString(gvLine.GetFocusedRowCellValue("BaseType"));
			ctm_baseDocumentLine.Visible = !string.IsNullOrEmpty(text2) && !text2.Equals("-1") && !text2.Equals("0");
			string text3 = Function.ToString(gvLine.GetFocusedRowCellValue("TargetType"));
			if (!string.IsNullOrEmpty(text3) && !text3.Equals("-1") && !text3.Equals("0"))
			{
				ctm_targetDocumentLine.Visible = true;
				ctm_delRowLine.Visible = false;
			}
			else
			{
				ctm_targetDocumentLine.Visible = false;
			}
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				return;
			}
			string text4 = Function.ToString(gvLine.GetFocusedRowCellValue("LineStatus")).ToUpper();
			string value = Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode"));
			if (text4 == "O" || text4 == "")
			{
				if (FormMode != Enums.FormViewModeType.Add)
				{
					if (new ApzObjectTypes[6]
					{
						ApzObjectTypes.oInvoices,
						ApzObjectTypes.oPurchaseInvoices,
						ApzObjectTypes.oPurchaseCreditNotes,
						ApzObjectTypes.oDeliveryNotes,
						ApzObjectTypes.oReserveInvoices,
						ApzObjectTypes.oAPReserveInvoice
					}.Contains(FormObjectType))
					{
						ctm_LineClose.Visible = false;
					}
					else if (!string.IsNullOrWhiteSpace(value) && new ApzObjectTypes[6]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oDrafts
					}.Contains(FormObjectType))
					{
						ctm_LineClose.Visible = true;
					}
					else
					{
						ctm_LineClose.Visible = false;
					}
					if (new ApzObjectTypes[7]
					{
						ApzObjectTypes.oQuotations,
						ApzObjectTypes.oOrders,
						ApzObjectTypes.oPurchaseOrders,
						ApzObjectTypes.oPurchaseQuotations,
						ApzObjectTypes.oPurchaseRequest,
						ApzObjectTypes.oDrafts,
						ApzObjectTypes.oReturnRequest
					}.Contains(FormObjectType))
					{
						ctm_LineDuplicate.Visible = true;
						ctm_delRowLine.Visible = true;
						ctm_addRowLine.Visible = true;
					}
					else
					{
						ctm_LineDuplicate.Visible = false;
						ctm_delRowLine.Visible = false;
						ctm_addRowLine.Visible = false;
					}
				}
				else
				{
					ctm_LineDuplicate.Visible = true;
					ctm_addRowLine.Visible = true;
					ctm_delRowLine.Visible = true;
					ctm_LineClose.Visible = false;
				}
			}
			else
			{
				ctm_LineClose.Visible = false;
				ctm_LineDuplicate.Visible = false;
				ctm_addRowLine.Visible = false;
				ctm_delRowLine.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_bte_CtlAccount_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
					gvLine.SetFocusedRowCellValue("AcctCode", acctCodeImpl.AcctCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || gvDoc.IsSorting || gvDoc.IsFiltering)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Busy || (FormMode == Enums.FormViewModeType.Add && gvDoc.DataRowCount > 1) || _isChangingRow)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
				{
					e.Allow = false;
					return;
				}
				gvDoc.IgnoreFilter = false;
				_isLoading = true;
				IsRefreshDataSource = true;
				grdDoc.DataSource = _originalDocTable.Copy();
				grdDoc.RefreshDataSource();
				_isLoading = false;
				e.Allow = true;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void txtToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = txtFromDate.DateTime;
			DateTime dateTime2 = txtToDate.DateTime;
			if (dateTime2 > default(DateTime) && dateTime2 < dateTime)
			{
				WinCommons.writeStatus(Enums.MessageCode.ToDateMustGreaterFromDate, Enums.MessageType.Warning, Msg_Time._TimeShort, "");
				txtToDate.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_Accounting_Project_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_Project frm = new frm_Project();
			frm.FormClosing += (object se, FormClosingEventArgs evs) =>
			{
				if (!string.IsNullOrWhiteSpace(frm.ProjectCode))
				{
					gv_Accounting.SetFocusedRowCellValue("Project", frm.ProjectCode);
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rBpCatalog_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (gvLine.Columns["SubCatNum"].OptionsColumn.ReadOnly)
			{
				return;
			}
			int focusedRowHandle = gvLine.FocusedRowHandle;
			((DocumentTable)grdDoc.DataSource).GetByEntry(Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			string text = Function.ToString(gvLine.GetRowCellValue(focusedRowHandle, "ItemCode"));
			string text2 = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			DocumentLineTable documentLineTable = grdLine.DataSource as DocumentLineTable;
			if (!string.IsNullOrEmpty(text2))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.BpCatalog, true, Function.ToString(gvLine.GetFocusedRowCellValue("SubCatNum")), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					BpCode = text2,
					ItemCode = text
				};
				frm_CFL2.ShowDialog(this);
				WinCommons._OpenProcessing();
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					int num = focusedRowHandle;
					foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
					{
						DocumentLineRow newRow = documentLineTable.GetNewRow();
						if (num == focusedRowHandle)
						{
							WinFuntions.DeleteBomChildsItem(gvLine, focusedRowHandle, ucBatchAndSerial1, isChooseItem: true);
						}
						newRow.SubCatNum = Function.ToString(chooseItem["SubCatNum"]);
						documentLineTable.Insert(num, newRow);
						WinFuntions.DataRowToGridView(chooseItem, gvLine, num);
						num++;
					}
				}
				else if (!string.IsNullOrEmpty(text))
				{
					gvLine.FocusedRowHandle = focusedRowHandle;
					gvLine.FocusedColumn = gvLine.VisibleColumns[0];
					gvLine.ShowEditor();
				}
				LstLineEdit = documentLineTable;
				grdLine.RefreshDataSource();
				AddNewBlankLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async void ctmSaveAsDraft_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currMode = FormMode;
		try
		{
			if (!GlobalData.LoginSap && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DocumentTable documentTable = grdDoc.DataSource as DocumentTable;
				_ = gv_Logistic.DataSource;
				if (!documentTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
					return;
				}
				DataSet dataSet = new DataSet();
				int focusedDataSourceRowIndex = gvDoc.GetFocusedDataSourceRowIndex();
				DataRow dataRow = documentTable[focusedDataSourceRowIndex];
				if (dataRow != null && Function.ParseInt(dataRow["DocEntry"]) == NewDocEntry)
				{
					dataRow["DocEntry"] = 0;
				}
				DocumentTable clone = documentTable.GetClone();
				DataRow dataRow2 = clone.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				clone.Rows.Add(dataRow2);
				clone.TableName = "DOC";
				dataSet.Tables.Add(clone.Copy());
				DocumentLineTable documentLineTable = grdLine.DataSource as DocumentLineTable;
				if (!documentLineTable.IsNotNull())
				{
					return;
				}
				DataTable dataTable = documentLineTable.Copy();
				dataTable.TableName = "LINES";
				DataRow[] array = (from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dataTable = array.CopyToDataTable();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ContentIsEmpty, "");
					FormMode = currMode;
				}
				dataSet.Tables.Add(dataTable);
				DataTable dataTable2 = ucBatchAndSerial1.CheckDataBatch(documentLineTable, isDraft: true);
				if (dataTable2 == null)
				{
					FormMode = currMode;
					WinCommons._CloseProcessing();
					return;
				}
				dataTable2.TableName = "BatchAndSerialNumer";
				dataSet.Tables.Add(dataTable2.Copy());
				if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
				{
					FormMode = currMode;
					WinCommons._CloseProcessing();
					return;
				}
				dt.TableName = "ATTFILE";
				DataSet dataSet2 = dt.DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove("ATTFILE");
					dataSet2.AcceptChanges();
				}
				dataSet.Tables.Add(dt.Copy());
				FormMode = Enums.FormViewModeType.Busy;
				HttpResult httpResult = await _httpClient.Documents.SaveDraft(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					FormMode = currMode;
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					btnSearch.PerformClick();
				}
				return;
			}
			await SaveData(isDraft: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async void cmtClose_Click(object sender, EventArgs e)
	{
		try
		{
			await CancelCloseDelete("Close", Apzon.Commons.Properties.Resources.Msg_Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void cmtCancel_Click(object sender, EventArgs e)
	{
		try
		{
			await CancelCloseDelete("Cancel", Apzon.Commons.Properties.Resources.Msg_Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void ctmDuplicate_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			int focusedRowHandle = gvDoc.FocusedRowHandle;
			if (gvDoc.IsRowSelected(focusedRowHandle))
			{
				int focusedEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
				DataRow row = gvDoc.GetFocusedDataRow();
				await AddDocClick();
				DataTable list = grdDoc.DataSourceTable;
				string[] array = row?.Table.Columns.Cast<DataColumn>().Where((DataColumn t) => t.ColumnName.StartsWith("U_") && list.Columns.Contains(t.ColumnName)).Select((DataColumn t) => t.ColumnName)
					.ToArray();
				for (int num = 0; num < array.Length; num++)
				{
					grdDoc.DataSourceTable.Rows[0][array[num]] = row[array[num]];
				}
				grdDoc.DataSourceTable.AcceptChanges();
				DocumentLineTable documentLineTable = await _httpClient.Documents.Duplicate(focusedEntry);
				foreach (DataRow row2 in documentLineTable.Rows)
				{
					row2["DocEntry"] = NewDocEntry;
					row2["ShipDate"] = DateTime.Now;
					row2["DocDate"] = DateTime.Now;
				}
				grdLine.DataSource = documentLineTable;
				grdLine.RefreshDataSource();
				gvLine.SetRowCellValue(0, "PriceBefDi", gvLine.GetRowCellValue(0, "PriceBefDi"));
				AddNewBlankLine();
				_onlyChangeRemark = false;
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.ChooseDocument, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async void ctmAddRow_Click(object sender, EventArgs e)
	{
		await AddDocClick();
	}

	private async Task AddDocClick()
	{
		FormMode = Enums.FormViewModeType.Add;
		_onlyChangeRemark = true;
		WinCommons._OpenProcessing();
		try
		{
			if (DocumentsHelper.Document.Columns.Count < 20)
			{
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.DocNum = -1;
				DocumentTable document;
				using (HttpClientHelper<DocumentTable> docTableHelper = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, FormController))
				{
					document = await docTableHelper.PostAsync("Search", documentsImpl);
				}
				DocumentsHelper.Document = document;
			}
			DocumentTable clone = DocumentsHelper.Document.GetClone();
			DocumentRow doc = (DocumentRow)clone.NewRow();
			doc.BPLId = GlobalData.DefaultBranch;
			doc.AtcEntry = _newFileEntry;
			doc.DocumentStatus = "O";
			doc.DocCurrency = GlobalData.MainCurrency;
			doc.DocRate = 1.0;
			doc.DocStatusName = "Open";
			DocumentRow documentRow = doc;
			DocumentRow documentRow2 = doc;
			DateTime? dateTime = (doc.TaxDate = DateTime.Today);
			DateTime? docDate = (documentRow2.DocDueDate = dateTime);
			documentRow.DocDate = docDate;
			doc.GroupNum = -1;
			_ = rInfoPriceList.DataSource;
			if (FormObjectType == ApzObjectTypes.oPurchaseRequest)
			{
				doc.ReqType = 12;
				doc.Requester = GlobalData.UserCodeLogin;
				doc.ReqName = GlobalData.UserNameLogin;
				doc.Department = Function.ParseInt(GlobalData.DeparmentLogin);
				doc.Branch = Function.ParseInt(GlobalData.BranchLogin);
				doc.ReqDate = DateTime.Today;
			}
			else if (FormObjectType == ApzObjectTypes.oOrders)
			{
				doc.Confirmed = GlobalData.GeneralSetting.RdrConfrmd;
			}
			else if (FormObjectType == ApzObjectTypes.oPurchaseOrders)
			{
				doc.Confirmed = GlobalData.GeneralSetting.PorConfrmd;
			}
			SetDefaultHeaderValueToAddNew(ref doc);
			clone.Rows.InsertAt(doc, 0);
			clone.AcceptChanges();
			grdDoc.DataSource = clone;
			grdDoc.RefreshDataSource();
			LoadSeriesByPostingDate();
			ucBatchAndSerial1.ClearDataSource();
			_currentRefDoc = new DataTable();
			if (DocumentsHelper.Lines == null || DocumentsHelper.Lines.Columns.Count < 20)
			{
				using HttpClientHelper<DocumentLineTable> docLineTableHelper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, FormController);
				DocumentsHelper.Lines = await docLineTableHelper.GetAsync("GetLine", new
				{
					id = -1
				});
			}
			LstLineEdit = DocumentsHelper.Lines.GetClone();
			Task t3 = BindDataGrdLine(LstLineEdit);
			DocumentTable clone2 = DocumentsHelper.Document.GetClone();
			clone2.Rows.Add(clone2.NewRow());
			clone2.Rows[0]["PartSupply"] = "Y";
			if (FormObjectType == ApzObjectTypes.oOrders)
			{
				clone2.Rows[0]["Confirmed"] = GlobalData.GeneralSetting.RdrConfrmd;
			}
			else if (FormObjectType == ApzObjectTypes.oPurchaseOrders)
			{
				clone2.Rows[0]["Confirmed"] = GlobalData.GeneralSetting.PorConfrmd;
			}
			grdLogistic.DataSource = clone2;
			grdLogistic.RefreshDataSource();
			gcDoc_CardCode.OptionsColumn.ReadOnly = false;
			gcItemCode.OptionsColumn.ReadOnly = false;
			CurrentEntry = NewDocEntry;
			if (DocumentsHelper.DocPayments != null)
			{
				DataTable dataSource = DocumentsHelper.TablePayment.Clone();
				grdPaymentTerms.DataSource = dataSource;
				grdPaymentTerms.RefreshDataSource();
			}
			ucAttachment1.grd_File.DataSource = WinFuntions.GetTableStructure("ATC1");
			ucAttachment1.grd_File.RefreshDataSource();
			drlCopyFrom.Enabled = true;
			drlCopyTo.Enabled = false;
			Task t4 = CreateFreightGridDatasource();
			if (_dtTabAccounting == null || (_dtTabAccounting != null && _dtTabAccounting.Columns.Count == 0))
			{
				_dtTabAccounting = (await WinFuntions.DocumentLoadTabAccountingColumns()).Clone();
			}
			NewRowAccounting();
			DataTable dataSource2 = _dtTabAccounting.Copy();
			grd_Accounting.DataSource = dataSource2;
			grd_Accounting.RefreshDataSource();
			CustomFormSetting();
			await Task.WhenAll(t3, t4);
			_downPaymentData = null;
			gvDoc.Columns["CardCode"].OptionsColumn.AllowEdit = true;
			gvDoc.FocusedColumn = gvDoc.Columns["CardCode"];
			gvDoc.ShowEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual void SetDefaultHeaderValueToAddNew(ref DocumentRow doc)
	{
	}

	private void NewRowAccounting()
	{
		if (_dtTabAccounting.Rows.Count == 0)
		{
			DataRow dataRow = _dtTabAccounting.NewRow();
			dataRow["JrnlMemo"] = "";
			dataRow["Project"] = "";
			dataRow["GroupNum"] = -1;
			dataRow["PeyMethod"] = "";
			dataRow["Indicator"] = "";
			dataRow["LicTradNum"] = "";
			dataRow["ImportEnt"] = 0;
			dataRow["PayDuMonth"] = "N";
			dataRow["ExtraMonth"] = 0;
			dataRow["ExtraDays"] = 0;
			dataRow["CdcOffset"] = 0;
			_dtTabAccounting.Rows.Add(dataRow);
			_dtTabAccounting.AcceptChanges();
		}
	}

	private int GetDefautSeries(int bplId)
	{
		try
		{
			DataTable dataTable = ((rep_Series.DataSource == null) ? new DataTable() : (rep_Series.DataSource as DataTable));
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => (Function.ParseInt(t["BPLId"]) == -1 || Function.ParseInt(t["BPLId"]) == bplId) && Function.ToString(t["IsDefault"]) == "Y" && Function.ParseInt(t["Series"]) != -1);
				if (dataRow != null)
				{
					return Function.ParseInt(dataRow["Series"]);
				}
				dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => (Function.ParseInt(t["BPLId"]) == -1 || Function.ParseInt(t["BPLId"]) == bplId) && Function.ParseInt(t["Series"]) != -1);
				if (dataRow != null)
				{
					return Function.ParseInt(dataRow["Series"]);
				}
				return Function.ParseInt(dataTable.Rows[0]["Series"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return -1;
	}

	private void gv_Freight_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
			int dataSourceRowIndex = gv_Freight.GetDataSourceRowIndex(e.RowHandle);
			DataTable dataSourceTable = grd_Freight.DataSourceTable;
			string text2;
			string text3;
			string text4;
			if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
			{
				text2 = "LineTotal";
				text3 = "VatSum";
				text4 = "GrsAmount";
			}
			else
			{
				text2 = "TotalFrgn";
				text3 = "VatSumFrgn";
				text4 = "GrsFC";
			}
			if (new string[2] { text2, "VatPrcnt" }.Contains(e.Column.FieldName))
			{
				double num = Function.ParseDouble(gv_Freight.GetRowCellValue(e.RowHandle, text2));
				double num2 = Function.ParseDouble(gv_Freight.GetRowCellValue(e.RowHandle, "VatPrcnt"));
				double num3 = num * num2 / 100.0;
				dataSourceTable.Rows[dataSourceRowIndex][text3] = num3;
				dataSourceTable.Rows[dataSourceRowIndex][text4] = num3 + num;
			}
			if (e.Column.FieldName == text3)
			{
				double num4 = Function.ParseDouble(gv_Freight.GetRowCellValue(e.RowHandle, text2));
				double num5 = Function.ParseDouble(e.Value);
				dataSourceTable.Rows[dataSourceRowIndex][text4] = num5 + num4;
			}
			if (e.Column.FieldName == text4)
			{
				double num6 = Function.ParseDouble(e.Value);
				double num7 = Function.ParseDouble(gv_Freight.GetRowCellValue(e.RowHandle, "VatPrcnt"));
				double num8 = num6 / (1.0 + num7 / 100.0);
				double num9 = num8 * num7 / 100.0;
				dataSourceTable.Rows[dataSourceRowIndex][text2] = num8;
				dataSourceTable.Rows[dataSourceRowIndex][text3] = num9;
			}
			if (new string[4] { text2, "VatPrcnt", text3, text4 }.Contains(e.Column.FieldName))
			{
				dataSourceTable.AcceptChanges();
				int num10 = GlobalData.GeneralSetting.SumDec.Value;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")).ToUpper()));
					if (currencyCodesImpl != null)
					{
						num10 = ((currencyCodesImpl.Decimals == -1) ? num10 : currencyCodesImpl.Decimals);
					}
				}
				ExtenseionMethods.SetDocTotal(gvDoc, grdLine, num10, FormObjectType, gv_Freight);
			}
			if (e.Column.FieldName == "VatGroup")
			{
				List<VatGroupsImpl> source = (List<VatGroupsImpl>)rLookUpVatGroup.DataSource;
				string vatCode = Function.ToString(e.Value);
				VatGroupsImpl vatGroupsImpl = source.FirstOrDefault((VatGroupsImpl t) => t.Code == vatCode);
				if (vatGroupsImpl != null)
				{
					gv_Freight.SetRowCellValue(e.RowHandle, "VatPrcnt", vatGroupsImpl.Rate.GetValueOrDefault());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Logistic_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if ((!(gridViewEx.FocusedColumn.FieldName == "ShipToCode") && !(gridViewEx.FocusedColumn.FieldName == "PayToCode")) || !(gridViewEx.ActiveEditor is LookUpEdit))
			{
				return;
			}
			LookUpEdit lookUpEdit = (LookUpEdit)gridViewEx.ActiveEditor;
			if (lookUpEdit != null)
			{
				DataTable dataTable = lookUpEdit.Properties.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					_clone = new DataView(dataTable)
					{
						RowFilter = "[CardCode] = '" + Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode")) + "'"
					};
					lookUpEdit.Properties.DataSource = _clone;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
			}
			else if (_lstDocFieldNotEditable.Contains(gvDoc.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
			else if (gvDoc.FocusedColumn == gc_Doc_Branch && FormMode != Enums.FormViewModeType.Add)
			{
				e.Cancel = true;
			}
			else
			{
				if (gvDoc.FocusedColumn == gcDpmTotal && new ApzObjectTypes[4]
				{
					ApzObjectTypes.oInvoices,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oAPReserveInvoice,
					ApzObjectTypes.oPurchaseInvoices
				}.Contains(FormObjectType))
				{
					return;
				}
				if (FormMode != Enums.FormViewModeType.Add && FormObjectType != ApzObjectTypes.oQuotations && FormObjectType != ApzObjectTypes.oOrders && FormObjectType != ApzObjectTypes.oPurchaseOrders && FormObjectType != ApzObjectTypes.oPurchaseQuotations && FormObjectType != ApzObjectTypes.oPurchaseRequest && FormObjectType != ApzObjectTypes.oDrafts && (FormObjectType != ApzObjectTypes.oPurchaseDownPayments || DownPaymentType != Enums.DownPaymentType.dptRequest) && (FormObjectType != ApzObjectTypes.oDownPayments || DownPaymentType != Enums.DownPaymentType.dptRequest) && !_lstDocFieldEditable.Contains(gvDoc.FocusedColumn.FieldName) && !gvDoc.FocusedColumn.FieldName.StartsWith("U_"))
				{
					e.Cancel = true;
					return;
				}
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
				string text2 = Function.ToString(gvDoc.GetFocusedRowCellValue("CANCELED"));
				if ((text.ToUpper().Equals("C") && gvDoc.FocusedColumn.FieldName != "Comments" && !gvDoc.FocusedColumn.FieldName.StartsWith("U_") && gvDoc.FocusedColumn.FieldName != "CntctCode" && gvDoc.FocusedColumn.FieldName != "SlpCode" && gvDoc.FocusedColumn.FieldName != "TaxDate") || ((gvDoc.FocusedColumn.FieldName == "CardCode" || gvDoc.FocusedColumn.FieldName == "CardName" || gvDoc.FocusedColumn.FieldName == "DocNum" || gvDoc.FocusedColumn.FieldName == "Series") && Fromtype != ApzObjectTypes.oDrafts && FormMode != Enums.FormViewModeType.Add))
				{
					e.Cancel = true;
				}
				else
				{
					gvDoc.Columns["TaxDate"].OptionsColumn.AllowEdit = true;
					gvDoc.Columns["TaxDate"].OptionsColumn.ReadOnly = false;
					gvDoc.Columns["CardCode"].OptionsColumn.AllowEdit = true;
					e.Cancel = false;
				}
				if (gvDoc.FocusedColumn.FieldName.Equals("DocCur"))
				{
					string text3 = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Currency"));
					if (text3 != null && text3 != "##")
					{
						e.Cancel = true;
					}
				}
				else if (gvDoc.FocusedColumn.FieldName.ToUpper().Equals("DOCNUM"))
				{
					if (!Function.ParseInt(gvDoc.GetFocusedRowCellValue("Series")).Equals(-1))
					{
						e.Cancel = true;
					}
				}
				else if (gvDoc.FocusedColumn.FieldName.ToUpper().Equals("NUMATCARD") && text2.Equals("Y"))
				{
					e.Cancel = false;
				}
				else if (gvDoc.FocusedColumn == gcPriceMode)
				{
					if (grdLine.DataSourceTable.AsEnumerable().Any((DataRow r) => Function.ToString(r["ItemCode"]) != ""))
					{
						e.Cancel = true;
						return;
					}
				}
				else if (gvDoc.FocusedColumn.FieldName == "DocRate")
				{
					string text4 = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
					if (text4 == GlobalData.MainCurrency || text4 == "")
					{
						e.Cancel = true;
						return;
					}
				}
				if (Cancellation)
				{
					e.Cancel = !_lstCheckCancellation.Contains(gvDoc.FocusedColumn.FieldName) && !gvDoc.FocusedColumn.FieldName.StartsWith("U_");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
				return;
			}
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
			string text2 = Function.ToString(gvLine.GetFocusedRowCellValue("LineStatus"));
			string fieldName = gvLine.FocusedColumn.FieldName;
			int num = Function.ParseInt(gvLine.GetFocusedRowCellValue("TargetType"));
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			if (text.ToUpper().Equals("C") || text2.Equals("C") || ((new ApzObjectTypes[10]
			{
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oCreditNotes,
				ApzObjectTypes.oReturns,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oPurchaseCreditNotes,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oAPReserveInvoice
			}.Contains(FormObjectType) || ((FormObjectType == ApzObjectTypes.oDownPayments || FormObjectType == ApzObjectTypes.oPurchaseDownPayments) && DownPaymentType == Enums.DownPaymentType.dptInvoice)) && FormMode != Enums.FormViewModeType.Add && !flag))
			{
				if (fieldName != "Text" && fieldName != "FreeTxt" && (!fieldName.StartsWith("U_") || !(_documentSetting.EnblUpdUDF == "Y")))
				{
					e.Cancel = true;
				}
				return;
			}
			if (num > 0)
			{
				if (fieldName != "Text" && fieldName != "FreeTxt" && fieldName != "Quantity" && fieldName != "InvQty" && (!fieldName.StartsWith("U_") || !(_documentSetting.EnblUpdUDF == "Y")))
				{
					e.Cancel = true;
				}
				return;
			}
			if (_lstFieldLineNotEditable.Contains(fieldName))
			{
				e.Cancel = true;
				return;
			}
			string text3 = Function.ToString(gvDoc.GetFocusedRowCellValue("PriceMode"));
			if (text3 == "G")
			{
				switch (fieldName)
				{
				case "PriceBefDi":
				case "LineTotal":
				case "TotalFrgn":
					e.Cancel = true;
					return;
				}
			}
			if (text3 != "G")
			{
				switch (fieldName)
				{
				case "PriceAfVAT":
				case "GTotal":
				case "GTotalFC":
					e.Cancel = true;
					return;
				}
			}
			string text4 = Function.ToString(gvLine.GetFocusedRowCellValue("LineType"));
			if (!string.IsNullOrEmpty(text4) && text4 == "S" && fieldName != "LineType")
			{
				e.Cancel = true;
				return;
			}
			string text5 = Function.ToString(gvLine.GetFocusedRowCellValue("TreeType"));
			string obj = Function.ToString(gvLine.GetFocusedRowCellValue("BOMTreeType"));
			if (text5 == "I" && Function.ParseInt(obj) >= 0 && (_arrBomSalesComponentsDisableCols.Contains(fieldName) || (GlobalData.GeneralSetting.TreePricOn == "Y" && fieldName == "PriceBefDi")))
			{
				e.Cancel = true;
			}
			else if (text5 == "S")
			{
				if (_arrBomSalesFatherDisableCols.Contains(fieldName))
				{
					if (GlobalData.GeneralSetting.TreePricOn == "Y" && fieldName == "PriceBefDi")
					{
						e.Cancel = false;
					}
					else
					{
						e.Cancel = true;
					}
				}
			}
			else if (Cancellation)
			{
				e.Cancel = true;
			}
			else if (fieldName == gcLineUomCode.FieldName)
			{
				if (Function.ToString(gvLine.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("Y") || Function.ParseInt(gvLine.GetFocusedRowCellValue(gcLineUomGroup)) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == gcLineUoMName.FieldName)
			{
				if (!Function.ToString(gvLine.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("N") || Function.ParseInt(gvLine.GetFocusedRowCellValue(gcLineUomGroup)) != -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName.Equals(gcLineEnSetCost.FieldName))
			{
				e.Cancel = !_documentSetting.Enitemcost.Equals("Y");
			}
			else if (fieldName.Equals(gcLineRetCost.FieldName))
			{
				e.Cancel = !Function.ToString(gvLine.GetFocusedRowCellValue(gcLineEnSetCost)).Equals("Y");
			}
			else if (fieldName == gcLineItemPerUnit.FieldName)
			{
				if (Function.ParseInt(gvLine.GetFocusedRowCellValue(gcLineUomGroup)) > 0 || Function.ParseInt(gvLine.GetFocusedRowCellValue(gcTargetType)) > 0 || Function.ToString(gvLine.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("Y"))
				{
					e.Cancel = true;
				}
			}
			else if (FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes)
			{
				if (Function.ParseInt(gvLine.GetFocusedRowCellValue(gcBaseType)) == 18 && _lstPDNField.Contains(fieldName))
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == "Rate")
			{
				string text6 = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
				if (text6 == "")
				{
					text6 = GlobalData.MainCurrency;
				}
				string text7 = Function.ToString(gvLine.GetFocusedRowCellValue("Currency"));
				if (text7 == "")
				{
					text7 = text6;
				}
				if (text6 == text7 || text7 == GlobalData.MainCurrency)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Accounting_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Accounting.FocusedColumn.FieldName == "JrnlMemo" || gv_Accounting.FocusedColumn.FieldName == "REFDOC")
			{
				return;
			}
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus")) == "C")
			{
				if (gv_Accounting.FocusedColumn.FieldName != "Project" && FormObjectType != ApzObjectTypes.oDrafts)
				{
					e.Cancel = true;
				}
			}
			else if (FormMode != Enums.FormViewModeType.Add && FormObjectType != ApzObjectTypes.oQuotations && FormObjectType != ApzObjectTypes.oOrders && FormObjectType != ApzObjectTypes.oPurchaseOrders && FormObjectType != ApzObjectTypes.oPurchaseQuotations && FormObjectType != ApzObjectTypes.oPurchaseRequest)
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

	private void gvLine_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!gvLine.IsInsertNewRow)
			{
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Accounting_GroupNum_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			_lookupChanged = true;
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			PaymentTermInfo = _httpClient.PaymentTermSetup.GetPaymentTermInfo(Function.ParseInt(lookUpEdit.EditValue));
			if (PaymentTermInfo.IsNotNull())
			{
				DataRow dataRow = PaymentTermInfo.Rows[0];
				DataTable dataTable = grd_Accounting.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					dataTable.Rows[0]["PayDuMonth"] = dataRow["PayDuMonth"];
					dataTable.Rows[0]["ExtraMonth"] = dataRow["ExtraMonth"];
					dataTable.Rows[0]["ExtraDays"] = dataRow["ExtraDays"];
					dataTable.AcceptChanges();
				}
				CalculateDueDate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rLineUomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gvLine.GetFocusedRowCellValue(gcLineUomGroup))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvLine.SetFocusedRowCellValue(gcLineUomCode, frm_CFL2.ChooseItem["UomCode"]);
				grdLine.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				frm_Diagram frm_Diagram2 = new frm_Diagram();
				frm_Diagram2.ObjectType = FormObjectType;
				frm_Diagram2.ObjectId = Function.ToString(gvDoc.GetFocusedRowCellValue("DocEntry"));
				frm_Diagram2.MdiParent = MdiParent;
				frm_Diagram2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Requester_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!gvDoc.Columns["Requester"].OptionsColumn.ReadOnly)
			{
				string requester = Function.ToString(gvDoc.GetFocusedRowCellValue("Requester"));
				ChooseFromListRequester(requester);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ChooseFromListRequester(string requester)
	{
	}

	protected virtual void ChooseFromListRequesterSearch()
	{
	}

	private void txtRequester_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseFromListRequesterSearch();
	}

	private void txtRequester_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				ChooseFromListRequesterSearch();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if (keyData == Keys.Tab && ActiveControl.Parent == txtBusinessPartner)
		{
			DataRow choosenCardCode = GetChoosenCardCode(txtBusinessPartner.Text);
			if (choosenCardCode != null && choosenCardCode.IsExistsColumn("CardCode"))
			{
				txtBusinessPartner.EditValue = Function.ToString(choosenCardCode["CardCode"]);
			}
		}
		return base.ProcessDialogKey(keyData);
	}

	private void Attachment_BrowseFile(List<DataRow> lst)
	{
		if (lst.Any() && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void Rep_gvLine_ShipToCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (sender is LookUpEditSimple lookUpEditSimple && lookUpEditSimple.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && lookUpEditSimple.Properties.GetDataSourceRowByKeyValue(lookUpEditSimple.EditValue) is DataRowView dataRowView)
			{
				gvLine.SetFocusedRowCellValue("ShipToDesc", Function.ToString(dataRowView.Row["FullAddress"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void CalculateDueDate()
	{
		if ((FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oReserveInvoices || FormObjectType == ApzObjectTypes.oAPReserveInvoice || FormObjectType == ApzObjectTypes.oCreditNotes || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes) && PaymentTermInfo.IsNotNull())
		{
			DataRow dataRow = PaymentTermInfo.Rows[0];
			DateTime dateTime = Function.ToString(dataRow["BslineDate"]) switch
			{
				"P" => Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("DocDate")), 
				"S" => DateTime.Today, 
				"T" => Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("TaxDate")), 
				_ => Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("DocDate")), 
			};
			switch (Function.ToString(dataRow["PayDuMonth"]))
			{
			case "E":
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1.0);
				break;
			case "H":
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 15);
				break;
			case "Y":
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
				break;
			}
			int months = Function.ParseInt(dataRow["ExtraMonth"]) + Function.ParseInt(dataRow["InstMonth"]);
			int num = Function.ParseInt(dataRow["ExtraDays"]) - Function.ParseInt(dataRow["TolDays"]) + Function.ParseInt(dataRow["InstDays"]);
			gvDoc.SetFocusedRowCellValue("DocDueDate", dateTime.AddMonths(months).AddDays(num));
		}
	}

	private void gv_Accounting_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string fieldName = e.Column.FieldName;
			switch (fieldName)
			{
			case "GroupNum":
				if (_lookupChanged)
				{
					_lookupChanged = false;
					break;
				}
				PaymentTermInfo = _httpClient.PaymentTermSetup.GetPaymentTermInfo(Function.ParseInt(e.Value));
				if (PaymentTermInfo.IsNotNull())
				{
					DataRow dataRow = PaymentTermInfo.Rows[0];
					DataTable dataTable = grd_Accounting.DataSource as DataTable;
					if (dataTable.IsNotNull())
					{
						dataTable.Rows[0]["PayDuMonth"] = dataRow["PayDuMonth"];
						dataTable.Rows[0]["ExtraMonth"] = dataRow["ExtraMonth"];
						dataTable.Rows[0]["ExtraDays"] = dataRow["ExtraDays"];
						dataTable.AcceptChanges();
					}
					CalculateDueDate();
				}
				break;
			case "PayDuMonth":
			case "ExtraMonth":
			case "ExtraDays":
				if (PaymentTermInfo.IsNotNull())
				{
					PaymentTermInfo.Rows[0][fieldName] = gv_Accounting.GetRowCellValue(e.RowHandle, fieldName);
					CalculateDueDate();
				}
				break;
			default:
				if (e.Column.FieldName.Equals("Project") && !Function.ToString(gvDoc.GetFocusedRowCellValue("Project")).Equals(Function.ToString(e.Value)))
				{
					gvDoc.SetFocusedRowCellValue("Project", e.Value);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetMapping(DataTable doc, DataTable lines, ContextActionMappingImpl[] mappingDoc, ContextActionMappingImpl[] mappingLines, DataTable batch, ContextActionMappingImpl[] mappingbatch, Enums.FormViewModeType viewmode = Enums.FormViewModeType.Add)
	{
		OpenFormDocument = doc;
		OpenFormLines = lines;
		MappingDocument = mappingDoc;
		MappingLines = mappingLines;
		OpenFormBatchSerial = batch;
		MappingBatchSerial = mappingbatch;
		OpenFormViewMode = viewmode;
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocType"));
			ApzObjectTypes apzObjectTypes = ((!flag) ? FormObjectType : ApzObjectTypes.oDrafts);
			int num = Function.ParseInt((!flag) ? gvDoc.GetFocusedRowCellValue("DocEntry") : gvDoc.GetFocusedRowCellValue("draftKey"));
			object docEntry = num;
			string formLayoutCode = (text.Equals("S") ? FormLayoutCodeService : FormLayoutCode);
			int num2 = (int)apzObjectTypes;
			await WinFuntions.PrintLayout(cardCode, docEntry, formLayoutCode, num2.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocType"));
			ApzObjectTypes apzObjectTypes = ((!flag) ? FormObjectType : ApzObjectTypes.oDrafts);
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oReserveInvoices:
				apzObjectTypes = ApzObjectTypes.oInvoices;
				break;
			case ApzObjectTypes.oAPReserveInvoice:
				apzObjectTypes = ApzObjectTypes.oPurchaseInvoices;
				break;
			}
			int num = Function.ParseInt((!flag) ? gvDoc.GetFocusedRowCellValue("DocEntry") : gvDoc.GetFocusedRowCellValue("draftKey"));
			object docEntry = num;
			string formLayoutCode = (text.Equals("S") ? FormLayoutCodeService : FormLayoutCode);
			VirFormDocument form = this;
			int num2 = (int)apzObjectTypes;
			await WinFuntions.PreviewLayout(cardCode, docEntry, formLayoutCode, form, num2.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINT()
	{
		try
		{
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocType"));
			ApzObjectTypes apzObjectTypes = ((!flag) ? FormObjectType : ApzObjectTypes.oDrafts);
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oReserveInvoices:
				apzObjectTypes = ApzObjectTypes.oInvoices;
				break;
			case ApzObjectTypes.oAPReserveInvoice:
				apzObjectTypes = ApzObjectTypes.oPurchaseInvoices;
				break;
			}
			int docEntry = Function.ParseInt((!flag) ? gvDoc.GetFocusedRowCellValue("DocEntry") : gvDoc.GetFocusedRowCellValue("draftKey"));
			string formLayoutCode = (text.Equals("S") ? FormLayoutCodeService : FormLayoutCode);
			int num = (int)apzObjectTypes;
			await WinFuntions.Print(cardCode, docEntry, formLayoutCode, num.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task CopyFromDrawAll(ApzObjectTypes type, List<DocumentsImpl> lst, int setRateType)
	{
		try
		{
			WinCommons._OpenProcessing();
			await CopyFrom(type, lst, setRateType);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task CopyFromCustomize(DataTable dt, DataTable dtFreight, List<DocumentsImpl> lst, int setRateType, string Fromentry, ApzObjectTypes formObjectType, ApzObjectTypes fromtype)
	{
		_ = 2;
		try
		{
			gvLine.ShowLoadingPanel();
			string card = lst[0].CardCode;
			if (lst.Exists((DocumentsImpl x) => x.CardCode != card))
			{
				WinCommons.Error(Enums.MessageCode.CannotCopyFromManyBp, "");
				return;
			}
			if (!string.IsNullOrEmpty(card))
			{
				await LoadDataCombobox(card);
				cbxStatus.SelectedIndex = 1;
				DocumentTable doc = await _httpClient.Documents.CopyHeader(fromtype, FormObjectType, Fromentry);
				FormMode = Enums.FormViewModeType.Add;
				if (doc.IsNotNull())
				{
					int bplId = Function.ParseInt(doc.Rows[0]["BPLId"]);
					int defautSeries = GetDefautSeries(bplId);
					doc.Rows[0]["Series"] = defautSeries;
					doc.Rows[0]["DocStatus"] = "O";
				}
				_isLoading = true;
				if (Function.ToString(doc.Rows[0]["CardCode"]) != "")
				{
					grdDoc.DataSource = doc;
					grdDoc.RefreshDataSource();
					_originalDocTable = doc.Copy();
				}
				_isLoading = false;
				ExtenseionMethods.ShowHideSumColumn(gvLine, Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur")), gv_Freight);
				ucBatchAndSerial1.FromReserve = Fromtype == ApzObjectTypes.oAPReserveInvoice || Fromtype == ApzObjectTypes.oReserveInvoices;
				if (ucBatchAndSerial1.CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && ucBatchAndSerial1.CopyObjectNotBatchAndSerial[FormObjectType].Contains(fromtype))
				{
					ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
					ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				}
				else
				{
					DataTable dataTable = _httpClient.Documents.CopyBatchSerial(Fromentry, fromtype);
					if (dataTable.IsNotNull())
					{
						DataTable dataTable2 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
						string[] columnNames = (from DataColumn t in dataTable2.Columns
							select t.ColumnName).ToArray();
						dataTable = dataTable.DefaultView.ToTable("Source", distinct: false, columnNames);
						DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
						if (dataRow != null)
						{
							dataTable2.Rows.Remove(dataRow);
							dataTable2.AcceptChanges();
						}
						foreach (DataRow row in dataTable.Rows)
						{
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3.ItemArray = row.ItemArray;
							dataTable2.Rows.Add(dataRow3);
						}
						ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
					}
					else
					{
						ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
					}
					DataTable batchBinAllowcateLineByDocEntry = _httpClient.BinLocMasterDataClient.GetBatchBinAllowcateLineByDocEntry(Fromentry, fromtype);
					if (batchBinAllowcateLineByDocEntry.IsNotNull())
					{
						foreach (DataRow r in batchBinAllowcateLineByDocEntry.Rows)
						{
							DataRow dataRow4 = dt.AsEnumerable().FirstOrDefault((DataRow n) => Function.ParseInt(n["BaseEntry"]) == Function.ParseInt(r["BaseDocEntry"]) && Function.ParseInt(n["BaseLine"]) == Function.ParseInt(r["BaseLineNumber"]));
							if (dataRow4 != null)
							{
								r["BaseLineNumber"] = dataRow4["LineNum"];
							}
						}
					}
					ucBatchAndSerial1.DataSourceBinAllowLine = (batchBinAllowcateLineByDocEntry.IsNotNull() ? batchBinAllowcateLineByDocEntry : DocumentsHelper.BatchSerialNumbersStruct.Copy());
				}
				if (Cancellation)
				{
					Text += " - Cancellation";
				}
				await LoadTabAccounting(Function.ParseInt(Fromentry), isCopyFrom: true, fromtype);
				if (FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oReserveInvoices || FormObjectType == ApzObjectTypes.oAPReserveInvoice || FormObjectType == ApzObjectTypes.oCreditNotes || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes)
				{
					PaymentTermInfo = _httpClient.PaymentTermSetup.GetPaymentTermInfo(Function.ParseInt(gv_Accounting.GetFocusedRowCellValue("GroupNum")));
					if (PaymentTermInfo.IsNotNull())
					{
						PaymentTermInfo.Rows[0]["PayDuMonth"] = gv_Accounting.GetFocusedRowCellValue("PayDuMonth");
						PaymentTermInfo.Rows[0]["ExtraMonth"] = gv_Accounting.GetFocusedRowCellValue("ExtraMonth");
						PaymentTermInfo.Rows[0]["ExtraDays"] = gv_Accounting.GetFocusedRowCellValue("ExtraDays");
						CalculateDueDate();
					}
				}
				if (FormObjectType != ApzObjectTypes.oPurchaseRequest && fromtype != ApzObjectTypes.oBlanketAgreement)
				{
					DataTable paymentTerms = HttpClientHelperFactory.GetHttpClient(fromtype).Documents.GetPaymentTerms(Function.ParseInt(Fromentry), fromtype);
					if (paymentTerms != null && paymentTerms.Rows.Count > 0)
					{
						spin_Count.EditValue = paymentTerms.Rows.Count;
					}
					grdPaymentTerms.DataSource = paymentTerms;
					grdPaymentTerms.RefreshDataSource();
					cal_Total.Value = Function.ParseDecimal(gvDoc.GetFocusedRowCellValue("DocTotal"));
				}
				if (fromtype != ApzObjectTypes.oBlanketAgreement && fromtype != ApzObjectTypes.oPurchaseBlanketAgreement)
				{
					grdLogistic.DataSource = doc;
					grdLogistic.RefreshDataSource();
				}
				CreateFreightGridDatasource();
				DataTable dataSourceTable = grd_Freight.DataSourceTable;
				if (dt.IsNotNull())
				{
					foreach (DataRow row2 in dtFreight.Rows)
					{
						if (Function.ParseDouble(row2["VatPrcnt"]) != 0.0)
						{
							row2["VatSum"] = Math.Round(Function.ParseDouble(row2["LineTotal"]) / Function.ParseDouble(row2["VatPrcnt"]));
						}
						row2["BaseType"] = (int)fromtype;
						row2["BaseAbsEnt"] = row2["DocEntry"];
						row2["BaseLnNum"] = row2["LineNum"];
						dataSourceTable.ImportRow(row2);
					}
					dataSourceTable.AcceptChanges();
					grd_Freight.DataSource = dataSourceTable;
					grd_Freight.RefreshDataSource();
				}
				ucAttachment1.LoadAttachments(Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry")));
			}
			if (FormObjectType == ApzObjectTypes.oPurchaseQuotations)
			{
				for (int num = 0; num < dt.Rows.Count; num++)
				{
					dt.Rows[num]["PQTReqQty"] = dt.Rows[num]["Quantity"];
					dt.Rows[num]["Quantity"] = 0;
				}
			}
			dt.AcceptChanges();
			WinFuntions.SetBOMTreeType(ref dt, ref LstBomChilds);
			switch (fromtype)
			{
			case ApzObjectTypes.oBlanketAgreement:
				foreach (DataRow row3 in dt.Rows)
				{
					row3["VatGroup"] = row3["VatGourpSa"];
					if (Function.ParseInt(row3["UomEntry"]) < 0)
					{
						row3["UomCode"] = row3["SUoMCode"];
						row3["unitMsr"] = row3["SalUnitMsr"];
						row3["NumPerMsr"] = row3["NumInSale"];
						row3["InvQty"] = Function.ParseDouble(row3["Quantity"]) * Function.ParseDouble(row3["NumPerMsr"]);
						row3["PriceBefDi"] = Function.ParseDouble(row3["PriceBefDi"]) * Function.ParseDouble(row3["NumPerMsr"]);
					}
					else
					{
						row3["unitMsr"] = row3["InvntryUom"];
					}
				}
				break;
			case ApzObjectTypes.oPurchaseBlanketAgreement:
				foreach (DataRow row4 in dt.Rows)
				{
					row4["VatGroup"] = row4["VatGroupPu"];
					if (Function.ParseInt(row4["UomEntry"]) < 0)
					{
						row4["UomCode"] = row4["PUoMCode"];
						row4["unitMsr"] = row4["BuyUnitMsr"];
						row4["NumPerMsr"] = row4["NumInBuy"];
						row4["InvQty"] = Function.ParseDouble(row4["Quantity"]) * Function.ParseDouble(row4["NumPerMsr"]);
						row4["PriceBefDi"] = Function.ParseDouble(row4["PriceBefDi"]) * Function.ParseDouble(row4["NumPerMsr"]);
					}
					else
					{
						row4["unitMsr"] = row4["InvntryUom"];
					}
				}
				break;
			}
			grdLine.DataSource = dt;
			UpdatePacQtyToDocumentCopy(dt);
			grdLine.RefreshDataSource();
			SetTotalAfterCopy(fromtype);
			SetExchangeRateCopy(setRateType);
			AddNewBlankLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvLine.HideLoadingPanel();
		}
	}

	private void SetExchangeRateCopy(int setRateType)
	{
		if (setRateType != 0 && setRateType != 2)
		{
			return;
		}
		string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
		if (text != GlobalData.MainCurrency)
		{
			DateTime date = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("DocDate")) ?? DateTime.Today;
			double exchangeRate = ExtenseionMethods.GetExchangeRate(text, date);
			if (exchangeRate > 0.0)
			{
				gvDoc.SetFocusedRowCellValue("DocRate", exchangeRate);
			}
			else
			{
				frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs
				{
					Curr = text,
					_date = date
				};
				frm.FormClosing += (object s, FormClosingEventArgs e1) =>
				{
					try
					{
						gvDoc.SetFocusedRowCellValue("DocRate", frm.rate);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
						WinCommons.ErrorLineNumber(ex);
					}
				};
				frm.ShowDialog(this);
			}
			for (int num = 0; num < gvLine.RowCount; num++)
			{
				if (Function.ToString(gvLine.GetRowCellValue(num, "ItemCode")) != "" && Function.ToString(gvLine.GetRowCellValue(num, "Currency")) == text)
				{
					gvLine.SetRowCellValue(num, "Rate", gvDoc.GetFocusedRowCellValue("DocRate"));
				}
			}
		}
		if (setRateType != 2)
		{
			return;
		}
		DateTime date2 = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("DocDate")) ?? DateTime.Today;
		for (int i = 0; i < gvLine.RowCount; i++)
		{
			string text2 = Function.ToString(gvLine.GetRowCellValue(i, "Currency"));
			if (Function.ToString(gvLine.GetRowCellValue(i, "ItemCode")) == "" || text2 == "" || text2 == text)
			{
				continue;
			}
			double exchangeRate2 = ExtenseionMethods.GetExchangeRate(text2, date2);
			if (exchangeRate2 > 0.0)
			{
				gvLine.SetRowCellValue(i, "Rate", exchangeRate2);
				continue;
			}
			frm_ExchangeRatesAndIndexs frm2 = new frm_ExchangeRatesAndIndexs
			{
				Curr = text2,
				_date = date2
			};
			frm2.FormClosing += (object s, FormClosingEventArgs e1) =>
			{
				try
				{
					gvLine.SetRowCellValue(i, "Rate", Function.ParseDouble(frm2.rate));
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			frm2.ShowDialog(this);
		}
	}

	private string GetFromFreightTableName(ApzObjectTypes type = ApzObjectTypes.oNone)
	{
		string result = "";
		try
		{
			if (type == ApzObjectTypes.oNone)
			{
				type = Fromtype;
			}
			switch (type)
			{
			case ApzObjectTypes.oInvoices:
				result = "INV3";
				break;
			case ApzObjectTypes.oCreditNotes:
				result = "RIN3";
				break;
			case ApzObjectTypes.oDeliveryNotes:
				result = "DLN3";
				break;
			case ApzObjectTypes.oReturns:
				result = "RDN3";
				break;
			case ApzObjectTypes.oOrders:
				result = "RDR3";
				break;
			case ApzObjectTypes.oQuotations:
				result = "QUT3";
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				result = "PCH3";
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				result = "RPC3";
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				result = "PDN3";
				break;
			case ApzObjectTypes.oPurchaseReturns:
				result = "RPD3";
				break;
			case ApzObjectTypes.oPurchaseOrders:
				result = "POR3";
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				result = "PQT3";
				break;
			case ApzObjectTypes.oPurchaseRequest:
				result = "PRQ3";
				break;
			case ApzObjectTypes.oDrafts:
				result = "DRF3";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private void RefreshFreightGrid(DataTable dtFr)
	{
		try
		{
			if (!(grd_Freight.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || dtFr == null || dtFr.Rows.Count <= 0)
			{
				return;
			}
			if ((Fromtype == ApzObjectTypes.oNone || Function.ParseInt(Fromentry) <= 0) && !new ApzObjectTypes[7]
			{
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oReturnRequest,
				ApzObjectTypes.oGoodsReturnRequest,
				ApzObjectTypes.oPurchaseRequest
			}.Contains(FormObjectType))
			{
				grd_Freight.DataSource = dtFr;
				grd_Freight.RefreshDataSource();
			}
			else
			{
				bool flag = Fromtype != ApzObjectTypes.oNone && Function.ParseInt(Fromentry) > 0 && DocEntrySearch <= 0;
				foreach (DataRow row in dtFr.Rows)
				{
					Function.ParseInt(row["BaseType"]);
					Function.ParseInt(row["BaseAbsEnt"]);
					DataRow dataRow;
					if (flag)
					{
						dataRow = dataTable.NewRow();
						dataTable.Rows.Add(dataRow);
					}
					else
					{
						dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ExpnsCode"]) == Function.ParseInt(row["ExpnsCode"]));
					}
					if (dataRow != null)
					{
						dataRow["ExpnsCode"] = Function.ToString(row["ExpnsCode"]);
						dataRow["LineTotal"] = Function.ParseDouble(row["LineTotal"]);
						dataRow["TotalFrgn"] = Function.ParseDouble(row["TotalFrgn"]);
						dataRow["Comments"] = Function.ToString(row["Comments"]);
						dataRow["VatGroup"] = Function.ToString(row["VatGroup"]);
						dataRow["VatPrcnt"] = Function.ParseDouble(row["VatPrcnt"]);
						dataRow["VatSum"] = Function.ParseDouble(row["VatSum"]);
						dataRow["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
						dataRow["GrsAmount"] = Function.ParseDouble(row["GrsAmount"]);
						dataRow["OcrCode"] = Function.ToString(row["OcrCode"]);
						dataRow["OcrCode2"] = Function.ToString(row["OcrCode2"]);
						dataRow["OcrCode3"] = Function.ToString(row["OcrCode3"]);
						dataRow["OcrCode4"] = Function.ToString(row["OcrCode4"]);
						dataRow["OcrCode5"] = Function.ToString(row["OcrCode5"]);
						dataRow["Project"] = Function.ToString(row["Project"]);
						dataRow["BaseType"] = (flag ? row["ObjType"] : ((object)Function.ParseInt(row["BaseType"])));
						dataRow["BaseAbsEnt"] = (flag ? row["DocEntry"] : ((object)Function.ParseInt(row["BaseAbsEnt"])));
						dataRow["BaseLnNum"] = (flag ? row["LineNum"] : ((object)Function.ParseInt(row["BaseLnNum"])));
						dataRow["Status"] = "O";
					}
				}
			}
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CreateFreightGridDatasource()
	{
		try
		{
			string vatType = "";
			if (FormObjectType == ApzObjectTypes.oOrders || FormObjectType == ApzObjectTypes.oQuotations || FormObjectType == ApzObjectTypes.oCreditNotes || FormObjectType == ApzObjectTypes.oDeliveryNotes || FormObjectType == ApzObjectTypes.oReturns || FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oReserveInvoices)
			{
				vatType = "I";
			}
			else if (FormObjectType == ApzObjectTypes.oPurchaseOrders || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes || FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes || FormObjectType == ApzObjectTypes.oPurchaseReturns || FormObjectType == ApzObjectTypes.oPurchaseQuotations || FormObjectType == ApzObjectTypes.oPurchaseRequest || FormObjectType == ApzObjectTypes.oAPReserveInvoice || FormObjectType == ApzObjectTypes.oPurchaseInvoices)
			{
				vatType = "O";
			}
			if (vatType == "I" || vatType == "O")
			{
				DataTable dataSource = await Task.Run(() => _httpClient.Commons.CreateFreightGridDatasource(vatType));
				grd_Freight.DataSource = dataSource;
				grd_Freight.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSeries()
	{
		DataTable dataTable = await _httpClient.Documents.LoadSeries(GlobalData.UserName);
		rep_Series.DataSource = dataTable;
		_dtSeries = dataTable.Copy();
	}

	private async Task LoadSeriesByPostingDate()
	{
		string postingDate = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("DocDate")).ToString("yyyy-MM-dd");
		DataTable dataTable = await _httpClient.Commons.LoadSeriesByPostingDate(GlobalData.UserName, FormObjectType, "--", postingDate);
		if (dataTable.Columns.Count > 0)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Series"] = -1;
			dataRow["SeriesName"] = "Manual";
			dataRow["BPLId"] = 0;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		rep_Series.DataSource = dataTable;
		if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Series"]) == Function.ParseInt(gvDoc.GetFocusedRowCellValue("Series"))))
		{
			DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["IsDefault"]) == "Y");
			if (dataRow2 != null)
			{
				gvDoc.SetFocusedRowCellValue("Series", dataRow2["Series"]);
			}
			else if (dataTable.Rows.Count > 0)
			{
				gvDoc.SetFocusedRowCellValue("Series", dataTable.Rows[0][0]);
			}
		}
	}

	private async Task GetDocumentSetting()
	{
		try
		{
			DataTable dataTable = await _httpClient.DocumentSettingClient.GetDataSettingByObjectType(FormObjectType);
			if (dataTable.IsNotNull())
			{
				_documentSetting.ObjType = Function.ToString(dataTable.Rows[0]["ObjType"]);
				_documentSetting.ChkDupRef = Function.ToString(dataTable.Rows[0]["ChkDupRef"]);
				_documentSetting.CpyCVRef = Function.ToString(dataTable.Rows[0]["CpyCVRef"]);
				_documentSetting.ReopOrder = Function.ToString(dataTable.Rows[0]["ReopOrder"]);
				_documentSetting.EnSetCost = Function.ToString(dataTable.Rows[0]["EnSetCost"]);
				_documentSetting.Enitemcost = Function.ToString(dataTable.Rows[0]["EnitemCost"]);
				_documentSetting.EnblUpdUDF = Function.ToString(dataTable.Rows[0]["EnblUpdUDF"]);
			}
			else
			{
				_documentSetting.ObjType = "";
				_documentSetting.ChkDupRef = "";
				_documentSetting.CpyCVRef = "";
				_documentSetting.ReopOrder = "";
				_documentSetting.EnSetCost = "";
				_documentSetting.Enitemcost = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task InitCellValueChanged()
	{
		gvDoc.InitCellValueChanged(gvLine, grdLogistic, rBillTo, rShipTo, rlpShipType, rlupContactPersonCode, cal_Total, FormObjectType, cal_TotalFC, gv_Freight, gv_Accounting);
		string vatType = "";
		string formBpType = FormBpType;
		if (!(formBpType == "C"))
		{
			if (formBpType == "S")
			{
				vatType = "I";
			}
		}
		else
		{
			vatType = "O";
		}
		Formulars = await WinFuntions.GetFormulas((int)FormObjectType);
		gvLine.InitLineCellValueChanged(this, gvDoc, ucBatchAndSerial1, rLookUpVatGroup, grdLogistic, Formulars, TblFormular, vatType, FormObjectType, LstBomChilds, gv_Freight);
	}

	protected virtual async Task InitCustomize()
	{
		if (!GlobalData.LoginSap)
		{
			tab_PaymentTerms.PageVisible = false;
		}
		Task t1 = GetDocumentSetting();
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rLogisticsBpChannel;
		repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
		{
			if (chooseItem != null)
			{
				gv_Logistic.SetFocusedRowCellValue("BPChCode", chooseItem["CardCode"]);
				grdLogistic.DataSourceTable.AcceptChanges();
				gv_Logistic.RefreshData();
			}
		}));
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_VendorNum;
		repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
		{
			if (chooseItem != null)
			{
				gvLine.SetFocusedRowCellValue("LineVendor", chooseItem["CardCode"]);
				grdLine.DataSourceTable.AcceptChanges();
				gvLine.RefreshData();
			}
		}));
		rLogisticsBpChannel.RefreshDataSource();
		Task t2 = GetContactPersons();
		rep_BaseType.SetDataSourceObject();
		gvLine.InitPasteFromExcel();
		rILCurrency.ShowExchangeRate(gvDoc);
		gvLine.HandleInvalidValueException();
		Task t5 = CreateShippingColumns();
		Task t6 = InitCellValueChanged();
		Task t7 = LoadComboSalesEmp();
		gvPayTerms.InitFormular(Formulars, TblFormular);
		if (!DocumentsHelper.BatchSerialNumbersStruct.IsNotNull())
		{
			await LoadBatchNumberStruct();
		}
		gvDoc.CellValueChanged += DocCellValueChanged;
		gvLine.CellValueChanged += ChangeViewMode;
		ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
		ucBatchAndSerial2.DBatchSerialChanged = (DataBatchSerialChanged)Delegate.Combine(ucBatchAndSerial2.DBatchSerialChanged, new DataBatchSerialChanged(BatchSerialChangeViewMode));
		gv_Logistic.CellValueChanged += ChangeViewMode;
		gvPayTerms.CellValueChanged += ChangeViewMode;
		gv_Freight.CellValueChanged += ChangeViewMode;
		gv_Accounting.CellValueChanged += ChangeViewMode;
		ucAttachment1.gv_File.CellValueChanged += ChangeViewMode;
		rep_doc_OwnerCode.RefreshDataSource();
		GridColumn gridColumn = gvDoc.Columns["U_RelatedTyp"];
		if (gridColumn != null)
		{
			RepositoryItemLookUpEditUDF repositoryItemLookUpEditUDF = new RepositoryItemLookUpEditUDF();
			repositoryItemLookUpEditUDF.Name = "luk_typeUDF";
			repositoryItemLookUpEditUDF.ShowFooter = false;
			repositoryItemLookUpEditUDF.ShowHeader = false;
			gridColumn.ColumnEdit = repositoryItemLookUpEditUDF;
		}
		GridColumn gridColumn2 = gvDoc.Columns["U_RelatedEnt"];
		if (gridColumn2 != null)
		{
			RegisterChooseFormListUDF registerChooseFormListUDF = new RegisterChooseFormListUDF();
			registerChooseFormListUDF.fileName = "U_RelatedEnt";
			registerChooseFormListUDF.GridView = gvDoc;
			gridColumn2.ColumnEdit = registerChooseFormListUDF;
		}
		if (!new ApzObjectTypes[6]
		{
			ApzObjectTypes.oDownPayments,
			ApzObjectTypes.oPurchaseDownPayments,
			ApzObjectTypes.oInvoices,
			ApzObjectTypes.oPurchaseInvoices,
			ApzObjectTypes.oReserveInvoices,
			ApzObjectTypes.oAPReserveInvoice
		}.Contains(FormObjectType))
		{
			gcDpmTotal.Visible = false;
		}
		else if (FormObjectType != ApzObjectTypes.oDownPayments && FormObjectType != ApzObjectTypes.oPurchaseDownPayments)
		{
			gcDpmPrcnt.Visible = false;
		}
		await Task.WhenAll(t1, t2, t7, t6, t5);
	}

	private void BatchSerialChangeViewMode()
	{
		if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		_onlyChangeRemark = false;
	}

	private async void DocCellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_onlyChangeRemark && !new string[5] { "Comments", "JrnlMemo", "Header", "Footer", "REFDOC" }.Contains(e.Column.FieldName))
			{
				_onlyChangeRemark = false;
			}
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			else if (FormMode == Enums.FormViewModeType.Add && e.Column == gcPostingDate)
			{
				gvDoc.SetFocusedRowCellValue("TaxDate", Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcPostingDate)));
				await LoadSeriesByPostingDate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeViewMode(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (_onlyChangeRemark && !new string[5] { "Comments", "JrnlMemo", "Header", "Footer", "REFDOC" }.Contains(e.Column.FieldName))
			{
				_onlyChangeRemark = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadBatchNumberStruct()
	{
		try
		{
			DocumentsHelper.BatchSerialNumbersStruct = await _httpClient.Documents.LoadBatchNumberStruct();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void CustomFormSetting()
	{
		try
		{
			if (!GlobalData.LoginSap)
			{
				foreach (GridColumn item in gv_Accounting.Columns.Where((GridColumn t) => !new string[3] { "GroupNum", "Project", "JrnlMemo" }.Contains(t.FieldName)))
				{
					item.Visible = false;
				}
			}
			else if (gv_Accounting.Columns["UseShpdGd"] != null && !IsSalesDocument)
			{
				gv_Accounting.Columns["UseShpdGd"].Visible = false;
			}
			LineLastColumn = gvLine.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvLine.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void LoadTableFormular()
	{
		TblFormular[FormDocumentTable] = gvDoc;
		TblFormular[FormLineTable] = gvLine;
		TblFormular["@OPMT"] = gvPayTerms;
	}

	protected virtual void LoadCurrency()
	{
		try
		{
			rILCurrency.DataSource = GlobalData.Currencies;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task LoadComboSalesEmp()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			DataTable dataTable = await helper.GetDataTableAsync("ShowSaleEmployee");
			if (dataTable.IsNotNull())
			{
				rILSalesEmp.PopupView.Columns.AddVisible("SlpName", "Name");
				rILSalesEmp.DataSource = dataTable;
				rep_LineSlpCode.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task BindDataToVatGroup(string category)
	{
		try
		{
			List<VatGroupsImpl> dataSource = await LoadVatGroup(category);
			rLookUpVatGroup.DataSource = dataSource;
			rep_GroupTax.DataSource = dataSource;
			rep_FreightTaxGroup.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task<List<VatGroupsImpl>> LoadVatGroup(string category)
	{
		try
		{
			using HttpClientHelper<VatGroupsImpl> hp = new HttpClientHelper<VatGroupsImpl>(Configuration.ApiServer, "Commons");
			return await hp.GetListAsync("GetVatGroup", new { category });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<VatGroupsImpl>();
	}

	protected virtual async Task LoadDataCombobox(string cardCode)
	{
		try
		{
			DataSet dataSet = await Task.Run(() => _httpClient.Commons.GetDataCombobox(cardCode));
			if (dataSet != null && dataSet.Tables.Count > 3)
			{
				rBillTo.DataSource = dataSet.Tables[0];
				rShipTo.DataSource = dataSet.Tables[1];
				rlpShipType.DataSource = dataSet.Tables[2];
				rlupContactPersonCode.DataSource = dataSet.Tables[3];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task LoadDataControl()
	{
		try
		{
			if (xtraTab_Line != null)
			{
				if (xtraTab_Line.SelectedTabPage != null)
				{
					xtraTab_Line.SelectedTabPage = tbDocumentLine;
				}
				await LoadDataCombobox(string.Empty);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task RefreshData()
	{
		try
		{
			LstLineDelete = new DocumentLineTable();
			ResetControl();
			await LoadData();
			if (gvLine.RowCount == 0)
			{
				grdLine.DataSource = LstLineEdit;
				grdLine.RefreshDataSource();
			}
			gcDoc_CardCode.OptionsColumn.ReadOnly = FormMode == Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetContactPersons()
	{
		IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
		string cardType = (FormBpType.Equals("S") ? "S" : "C;L");
		_dtContact = await httpClient.BusinessMaster.GetContactPersons(cardType);
	}

	protected virtual async Task LoadData()
	{
		try
		{
			await Task.WhenAll(GetDoc(), LoadDataControl());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task GetNewDocEntry()
	{
		try
		{
			using (HttpClientHelper<DocumentsImpl> documentClientHelper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, FormController))
			{
				NewDocEntry = await documentClientHelper.GetIntAsync("GetMaxDocEntry") + 1;
			}
			MaxDocEntry = NewDocEntry - 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task GetMinDocEntry()
	{
		try
		{
			using HttpClientHelper<DocumentsImpl> documentClientHelper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, FormController);
			MinDocEntry = await documentClientHelper.GetIntAsync("GetMinDocEntry");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ClearGrids()
	{
		DocumentTable clone = DocumentsHelper.Document.GetClone();
		grdDoc.DataSource = clone;
		grdDoc.RefreshDataSource();
		LstLineCopy = new DocumentLineTable();
		ucBatchAndSerial1.ClearLineDataSource();
		split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Panel1;
		LstLineEdit = new DocumentLineTable();
		ClearGridLine();
	}

	protected virtual async Task GetDoc()
	{
		_ = 3;
		try
		{
			ClearGrids();
			gvDoc.ShowLoadingPanel();
			_isSearching = true;
			DocumentsImpl documentsImpl = new DocumentsImpl();
			if (!string.IsNullOrEmpty(txtNoOfTrans.Text))
			{
				documentsImpl.DocNum = Function.ParseInt(txtNoOfTrans.Text);
			}
			if (DocEntrySearch > 0)
			{
				documentsImpl.DocEntry = DocEntrySearch;
				DocEntrySearch = 0;
			}
			documentsImpl.UserName = GlobalData.UserName;
			if (FormObjectType == ApzObjectTypes.oDownPayments || FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
			{
				documentsImpl.isIns = ((DownPaymentType == Enums.DownPaymentType.dptInvoice) ? "Y" : "N");
			}
			else
			{
				documentsImpl.isIns = CheckAri;
			}
			if (documentsImpl.DocNum == 0 && documentsImpl.DocEntry == 0)
			{
				if (!string.IsNullOrEmpty(txtFromDate.Text))
				{
					documentsImpl.DocDate = txtFromDate.DateTime;
				}
				if (!string.IsNullOrEmpty(txtToDate.Text))
				{
					documentsImpl.DocDueDate = txtToDate.DateTime;
				}
				if (documentsImpl.DocDate > default(DateTime) && documentsImpl.DocDueDate > default(DateTime) && documentsImpl.DocDueDate < documentsImpl.DocDate)
				{
					WinCommons.writeStatus(Enums.MessageCode.ToDateMustGreaterFromDate, Enums.MessageType.Error, Msg_Time._TimeShort, "");
					txtToDate.Focus();
					return;
				}
				documentsImpl.BPLId = GlobalData.DefaultBranch;
				DocumentsImpl documentsImpl2 = documentsImpl;
				string docStatus;
				if (cbxStatus.Text == "Open")
				{
					docStatus = "O";
				}
				else if (cbxStatus.Text == "" || cbxStatus.Text == "All")
				{
					docStatus = "A";
				}
				else
				{
					docStatus = ((cbxStatus.Text == "Delivered") ? "D" : "C");
				}
				documentsImpl2.DocStatus = docStatus;
				documentsImpl.Cancelled = ((cbxStatus.Text == "Canceled") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				documentsImpl.CardCode = txtBusinessPartner.Text;
				documentsImpl.Requester = Function.ToString(txtRequester.Text);
				documentsImpl.ReqType = Function.ParseInt(look_ReqType.EditValue);
			}
			DocumentTable documentTable = await _httpClient.Documents.Search(documentsImpl);
			if (documentTable != null)
			{
				if (documentTable.IsNotNull())
				{
					documentTable.PrimaryKey = new DataColumn[1] { documentTable.Columns["DocEntry"] };
					string text = Function.ToString(documentTable.Rows[0]["CANCELED"]);
					if (text == "C" || text == "Y")
					{
						Text = Text.Replace(" - Cancellation", "") + " - Cancellation";
					}
					else
					{
						Text = Text.Replace(" - Cancellation", "");
					}
				}
				grdDoc.DataSource = documentTable;
				_originalDocTable = documentTable.Copy();
				grdDoc.RefreshDataSource();
				disableCopyTobyRejected();
				gvDoc.HideLoadingPanel();
				if (documentTable.Count > 0)
				{
					gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.False;
					await LoadChildData();
					gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
				}
			}
			if (_customizePaymentTerms == "Y" && (DocumentsHelper.TablePayment == null || DocumentsHelper.TablePayment.Columns.Count == 0))
			{
				DocumentsHelper.TablePayment = (await WinFuntions.CreateCloneTabl("@PMT1")).Clone();
			}
			if (_dtTabAccounting == null || _dtTabAccounting.Columns.Count == 0)
			{
				_dtTabAccounting = (await WinFuntions.DocumentLoadTabAccountingColumns()).Clone();
			}
			FormMode = Enums.FormViewModeType.View;
			_isSearching = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvDoc.HideLoadingPanel();
		}
	}

	protected virtual void ResetControl()
	{
		if (FormAuth != Enums.FormAuthorities.Read)
		{
			drlCopyFrom.Enabled = false;
			drlCopyTo.Enabled = false;
			ctm_addRowLine.Visible = true;
			ctm_delRowLine.Visible = true;
			ctm_Cancel.Visible = true;
			ctm_Close.Visible = true;
			ctm_LineClose.Visible = true;
			ctm_LineDuplicate.Visible = true;
		}
	}

	protected virtual async Task SaveData(bool isDraft = false)
	{
		gvDoc.PostEditor();
		gvLine.PostEditor();
		gv_Accounting.PostEditor();
		gv_Logistic.PostEditor();
		Enums.FormViewModeType currMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DocumentLineTable documentLineTable = grdLine.DataSource as DocumentLineTable;
			if (documentLineTable != null && documentLineTable.GetEnumerable().All((DocumentLineRow x) => string.IsNullOrEmpty(x.ItemCode)))
			{
				WinCommons.Error(Enums.MessageCode.NoItemChoosen, "");
				FormMode = currMode;
				return;
			}
			if (currMode == Enums.FormViewModeType.Update)
			{
				int docEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
				if (documentLineTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DocEntry"]) != 0 && Function.ParseInt(r["DocEntry"]) != docEntry))
				{
					WinCommons.Error(Enums.MessageCode.DocumentHeaderAndDetailNotMatch, "");
					FormMode = currMode;
					return;
				}
			}
			gvDoc.MakeRowVisible(gvDoc.FocusedRowHandle);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				if (_documentSetting.ChkDupRef.Equals("W"))
				{
					string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gcDoc_NumAtCard));
					if (!string.IsNullOrEmpty(text))
					{
						HttpResult httpResult = await HttpClient.Commons.CheckDocumentDuplicateBpRefNo(FormObjectType, Function.ToString(gvDoc.GetFocusedRowCellValue(gcDoc_CardCode)), text);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							return;
						}
						if (Function.ParseBool(httpResult.Content))
						{
							if (XtraMessageBoxEx.Show(FormBpType.Equals("C") ? Enums.MessageCode.CustomerRefNoAlreadyExists : Enums.MessageCode.VendorRefNoAlreadyExists, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
							{
								return;
							}
							WinCommons._OpenProcessing();
						}
					}
				}
				DocumentTable documentTable = grdDoc.DataSource as DocumentTable;
				if (!GlobalData.LoginSap)
				{
					if (!documentTable.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
						return;
					}
					DataSet dataSet = new DataSet();
					int focusedDataSourceRowIndex = gvDoc.GetFocusedDataSourceRowIndex();
					DataRow dataRow = documentTable[focusedDataSourceRowIndex];
					if (dataRow != null && Function.ParseInt(dataRow["DocEntry"]) == NewDocEntry)
					{
						dataRow["DocEntry"] = 0;
					}
					DocumentTable clone = documentTable.GetClone();
					DataRow dataRow2 = clone.NewRow();
					dataRow2.ItemArray = dataRow.ItemArray;
					clone.Rows.Add(dataRow2);
					clone.TableName = "DOC";
					clone.Rows[0]["IsIns"] = CheckAri;
					dataSet.Tables.Add(clone.Copy());
					DocumentLineTable documentLineTable2 = grdLine.DataSource as DocumentLineTable;
					if (!documentLineTable2.IsNotNull())
					{
						return;
					}
					DataTable dataTable = documentLineTable2.Copy();
					dataTable.TableName = "LINES";
					DataRow[] array = (from dataRow3 in dataTable.AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(dataRow3["ItemCode"]))
						select dataRow3).ToArray();
					if (array.Length != 0)
					{
						dataTable = array.CopyToDataTable();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.ContentIsEmpty, "");
						FormMode = currMode;
					}
					dataSet.Tables.Add(dataTable);
					DataTable dataTable2 = ucBatchAndSerial1.CheckDataBatch(documentLineTable2, isDraft: false);
					if (dataTable2 == null)
					{
						FormMode = currMode;
						WinCommons._CloseProcessing();
						return;
					}
					dataTable2.TableName = "BatchAndSerialNumer";
					dataSet.Tables.Add(dataTable2.Copy());
					if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
					{
						return;
					}
					dt.TableName = "ATTFILE";
					DataSet dataSet2 = dt.DataSet;
					if (dataSet2 != null)
					{
						dataSet2.Tables.Remove("ATTFILE");
						dataSet2.AcceptChanges();
					}
					dataSet.Tables.Add(dt.Copy());
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
					FormMode = Enums.FormViewModeType.Busy;
					HttpResult httpResult2 = await helper.PostAsync("SaveDataSet", dataSet);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
						FormMode = currMode;
						return;
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
					await ClickToRefresh();
					if (Formcopy is frm_ServiceCalls && OpenFormViewMode == Enums.FormViewModeType.Add)
					{
						OkToClose = true;
						Close();
					}
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					if (documentTable == null)
					{
						return;
					}
					int focusedDataSourceRowIndex2 = gvDoc.GetFocusedDataSourceRowIndex();
					if (GlobalData.ListDocUDFNameAndType == null)
					{
						GlobalData.ListDocUDFNameAndType = WinFuntions.GetDocUDFNameAndType();
					}
					DocumentsImpl value = documentTable[focusedDataSourceRowIndex2].ToDocument(GlobalData.ListDocUDFNameAndType);
					if (currMode == Enums.FormViewModeType.Add)
					{
						value.DocEntry = 0;
					}
					if (grdLogistic.DataSource is DocumentTable documentTable2)
					{
						value.TrnspCode = documentTable2[0].TransportationCode;
						value.Address = documentTable2[0].Address;
						value.Address2 = documentTable2[0].Address2;
						value.ShipToCode = documentTable2[0].ShipToCode;
						value.PayToCode = documentTable2[0].PayToCode;
						value.Confirmed = documentTable2[0].Confirmed;
						value.PartSupply = documentTable2[0].PartSupply;
						value.PickRmrk = documentTable2[0].PickRmrk;
					}
					Task t1 = Task.Run(() =>
					{
						if (_currentRefDoc.IsNotNull())
						{
							value.DocumentRef = new List<Document_ReferencedImpl>();
							foreach (DataRow row2 in _currentRefDoc.Rows)
							{
								Document_ReferencedImpl document_ReferencedImpl = new Document_ReferencedImpl();
								int num5 = Function.ParseInt(row2["RefDocNum"]);
								if (num5 != 0)
								{
									document_ReferencedImpl.RefDocEntr = Function.ParseInt(row2["RefDocEntr"]);
									document_ReferencedImpl.RefDocNum = num5;
									document_ReferencedImpl.RefObjType = Function.ParseInt(row2["RefObjType"]);
									document_ReferencedImpl.Remark = Function.ToString(row2["Remark"]);
									value.DocumentRef.Add(document_ReferencedImpl);
								}
							}
						}
					});
					if (FormObjectType == ApzObjectTypes.oDownPayments || FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
					{
						value.isIns = ((DownPaymentType == Enums.DownPaymentType.dptInvoice) ? "Y" : "N");
					}
					else
					{
						value.isIns = CheckAri;
					}
					DocumentsImpl documentsImpl = value;
					int downPaymentType = (int)DownPaymentType;
					documentsImpl.Posted = downPaymentType.ToString();
					if (GlobalData.ListLineUDFNameAndType == null)
					{
						GlobalData.ListLineUDFNameAndType = WinFuntions.GetDocLineUdfNameAndTypeDictionary();
					}
					DocumentLineTable documentLineTable3 = grdLine.DataSource as DocumentLineTable;
					List<Document_LinesImpl> lstLine = documentLineTable3.ToDocumentLine(GlobalData.ListLineUDFNameAndType, priceSpecified: true);
					lstLine = lstLine.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode) && (string.IsNullOrEmpty(x.LineType) || x.LineType == "R")).ToList();
					if (lstLine.Count > 0)
					{
						List<Document_LinesImpl> list = new List<Document_LinesImpl>();
						int num = lstLine.Max((Document_LinesImpl x) => x.LineNum);
						for (int num2 = 0; num2 < lstLine.Count; num2++)
						{
							if (lstLine[num2].DocEntry == NewDocEntry)
							{
								lstLine[num2].RemainingOpenQuantity = lstLine[num2].Quantity;
							}
							if (lstLine[num2].LineNum == -1)
							{
								downPaymentType = (lstLine[num2].LineNum = num + 1);
								num = downPaymentType;
							}
							if (lstLine[num2].BOMTreeType == "-1")
							{
								list.Add(lstLine[num2]);
							}
							if (!Function.ToString(lstLine[num2].BOMTreeType).Equals("T"))
							{
								continue;
							}
							lstLine[num2].BOMTreeType = "0";
							if (lstLine[num2].UserFields != null)
							{
								UserFieldsImpl userFieldsImpl = lstLine[num2].UserFields.FirstOrDefault((UserFieldsImpl userFieldsImpl2) => userFieldsImpl2.Name.ToUpper().Equals("U_BOMTemp".ToUpper()));
								if (userFieldsImpl != null)
								{
									userFieldsImpl.Value = "T";
								}
							}
						}
						for (int num4 = 0; num4 < list.Count; num4++)
						{
							lstLine.Remove(list[num4]);
						}
						DataTable dtBatchSerial = ucBatchAndSerial1.CheckDataBatch(documentLineTable3, isDraft);
						if (dtBatchSerial == null)
						{
							FormMode = currMode;
							WinCommons._CloseProcessing();
							return;
						}
						Task t2 = Task.Run(() =>
						{
							lstLine.LoadDataBatch(dtBatchSerial, FormObjectType);
						});
						DataTable dtBinAllowcation = ucBatchAndSerial1.CheckDataBinAllowcationLine(documentLineTable3, isDraft);
						if (dtBinAllowcation == null)
						{
							FormMode = currMode;
							WinCommons._CloseProcessing();
							return;
						}
						Task t3 = Task.Run(() =>
						{
							lstLine.LoadBinAllowcation(dtBinAllowcation, FormObjectType);
						});
						value.Lines = lstLine;
						List<DataRow> specialLines = (from dataRow3 in documentLineTable3.AsEnumerable()
							where new string[2] { "T", "S" }.Contains(Function.ToString(dataRow3["LineType"]))
							select dataRow3).ToList();
						Task t4 = Task.Run(() =>
						{
							if (specialLines != null && specialLines.Count > 0)
							{
								value.SpecialLines = new List<Document_SpecialLinesImpl>();
								for (int i = 0; i < specialLines.Count; i++)
								{
									Document_SpecialLinesImpl document_SpecialLinesImpl = new Document_SpecialLinesImpl
									{
										AfterLineNumber = Function.ParseInt(specialLines[i]["LineNum"])
									};
									string text3 = Function.ToString(specialLines[i]["LineType"]);
									document_SpecialLinesImpl.LineNum = i;
									if (text3 == "T")
									{
										document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Text;
										document_SpecialLinesImpl.LineText = Function.ToString(specialLines[i]["ItemCode"]);
									}
									else if (text3 == "S")
									{
										document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Subtotal;
										document_SpecialLinesImpl.Subtotal = Function.ParseDouble(specialLines[i]["LineTotal"]);
										document_SpecialLinesImpl.SubtotalFC = Function.ParseDouble(specialLines[i]["TotalFrgn"]);
									}
									value.SpecialLines.Add(document_SpecialLinesImpl);
								}
							}
						});
						Task t5 = Task.Run(() =>
						{
							if (grdPaymentTerms.DataSource != null)
							{
								value.Udos = new List<DocumentUdo>();
								DocumentUdo documentUdo = new DocumentUdo("OPMT", "DocumentPaymentTerms");
								documentUdo.dtFather = new DataTable();
								documentUdo.dtFather.Columns.Add("DocEntry", typeof(int));
								DataRow dataRow3 = documentUdo.dtFather.NewRow();
								UserFieldsImpl userFieldsImpl2 = value.UserFields.FirstOrDefault((UserFieldsImpl userFieldsImpl3) => userFieldsImpl3.Name.Equals("U_paymentEntry"));
								if (userFieldsImpl2 != null)
								{
									dataRow3["DocEntry"] = Function.ParseInt(userFieldsImpl2.Value);
								}
								documentUdo.dtFather.Rows.Add(dataRow3);
								documentUdo.dsChild = new DataSet();
								if (grdPaymentTerms.DataSourceTable.IsNotNull() && grdPaymentTerms.DataSourceTable.Rows.Count > 0)
								{
									DataTable dataTable4 = grdPaymentTerms.DataSourceTable.Copy();
									dataTable4.TableName = "PMT1";
									documentUdo.dsChild.Tables.Add(dataTable4);
									value.Udos.Add(documentUdo);
								}
							}
						});
						string wddStatus = Function.ToString(gvDoc.GetFocusedRowCellValue("WddStatus"));
						if (GlobalData.GeneralSetting.useDocWrf == "Y" && !isDraft && (!_onlyChangeRemark || (value.IsDraft && (wddStatus == "" || wddStatus == "-"))))
						{
							using (HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
							{
								DocumentsImpl doc = value;
								downPaymentType = (int)currMode;
								if (!(await doc.GetApprovalTemplate(dtClientHelper, downPaymentType.ToString(), this)))
								{
									WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
									FormMode = currMode;
									return;
								}
							}
							if (value.Document_ApprovalRequests != null)
							{
								WinCommons._OpenProcessing();
							}
						}
						Task<List<AttachmentFile_Impl>> t7 = Task.Run(() => ucAttachment1.UploadFile());
						Task task = Task.Run(() =>
						{
							if (new ApzObjectTypes[4]
							{
								ApzObjectTypes.oInvoices,
								ApzObjectTypes.oReserveInvoices,
								ApzObjectTypes.oAPReserveInvoice,
								ApzObjectTypes.oPurchaseInvoices
							}.Contains(FormObjectType) && _downPaymentData.IsNotNull())
							{
								EnumerableRowCollection<DataRow> enumerableRowCollection = from r in _downPaymentData.AsEnumerable()
									where Function.ToString(r["Selected"]) == "Y"
									select r;
								if (enumerableRowCollection.Any())
								{
									List<DownPaymentsToDrawImpl> list3 = new List<DownPaymentsToDrawImpl>();
									foreach (DataRow row in enumerableRowCollection)
									{
										DownPaymentsToDrawImpl downPaymentsToDrawImpl = new DownPaymentsToDrawImpl
										{
											DocEntry = Function.ParseInt(row["Id"])
										};
										if (string.IsNullOrEmpty(value.DocCur) || value.DocCur == GlobalData.MainCurrency)
										{
											downPaymentsToDrawImpl.AmountToDraw = Function.ParseDouble(row["NetAmount"]);
											downPaymentsToDrawImpl.GrossAmountToDraw = Function.ParseDouble(row["GrossAmount"]);
											downPaymentsToDrawImpl.Tax = Function.ParseDouble(row["TaxAmount"]);
										}
										else
										{
											downPaymentsToDrawImpl.AmountToDrawFC = Function.ParseDouble(row["NetAmount"]);
											downPaymentsToDrawImpl.GrossAmountToDrawFC = Function.ParseDouble(row["GrossAmount"]);
											downPaymentsToDrawImpl.TaxFC = Function.ParseDouble(row["TaxAmount"]);
										}
										EnumerableRowCollection<DataRow> enumerableRowCollection2 = from r in _downPaymentData.AsEnumerable()
											where Function.ToString(r["FatherId"]) == Function.ToString(row["Id"])
											select r;
										List<DownPaymentToDrawDetailImpl> list4 = new List<DownPaymentToDrawDetailImpl>();
										foreach (DataRow item in enumerableRowCollection2)
										{
											DownPaymentToDrawDetailImpl downPaymentToDrawDetailImpl = new DownPaymentToDrawDetailImpl
											{
												VatGroupCode = Function.ToString(item["VatGroup"])
											};
											if (string.IsNullOrEmpty(value.DocCur) || value.DocCur == GlobalData.MainCurrency)
											{
												downPaymentToDrawDetailImpl.AmountToDraw = Function.ParseDouble(item["NetAmount"]);
												downPaymentToDrawDetailImpl.GrossAmountToDraw = Function.ParseDouble(item["GrossAmount"]);
												downPaymentToDrawDetailImpl.Tax = Function.ParseDouble(item["TaxAmount"]);
											}
											else
											{
												downPaymentToDrawDetailImpl.AmountToDrawFC = Function.ParseDouble(item["NetAmount"]);
												downPaymentToDrawDetailImpl.GrossAmountToDrawFC = Function.ParseDouble(item["GrossAmount"]);
												downPaymentToDrawDetailImpl.TaxFC = Function.ParseDouble(item["TaxAmount"]);
											}
											list4.Add(downPaymentToDrawDetailImpl);
										}
										downPaymentsToDrawImpl.DownPaymentsToDrawDetails = list4;
										list3.Add(downPaymentsToDrawImpl);
									}
									value.DownPaymentsToDraw = list3;
								}
							}
						});
						await Task.WhenAll(t1, t2, t3, t4, t5, task, t7);
						List<AttachmentFile_Impl> list2 = await t7;
						if (ucAttachment1.DataSource.IsNotNull() && list2.Count == 0)
						{
							WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "");
							FormMode = currMode;
							return;
						}
						value.AttachmentFiles = list2;
						DataRow[] array2 = (from dataRow3 in grd_Freight.DataSourceTable.Copy().AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(dataRow3["ExpnsCode"])) && (Function.ParseDouble(dataRow3["LineTotal"]) != 0.0 || Function.ParseDouble(dataRow3["TotalFrgn"]) != 0.0 || (value.DocStatus == "O" && value.DocEntry != 0 && new ApzObjectTypes[3]
							{
								ApzObjectTypes.oOrders,
								ApzObjectTypes.oPurchaseOrders,
								ApzObjectTypes.oDrafts
							}.Contains(FormObjectType)))
							select dataRow3).ToArray();
						if (array2.Length != 0)
						{
							value.Expenses = array2.CopyToDataTable().ToDocumentsAdditional();
						}
						DataTable dataTable3 = (DataTable)grd_Accounting.DataSource;
						if (dataTable3.IsNotNull())
						{
							value.JrnlMemo = Function.ToString(dataTable3.Rows[0]["JrnlMemo"]);
							value.Project = Function.ToString(dataTable3.Rows[0]["Project"]);
							value.GroupNum = Function.ParseInt(dataTable3.Rows[0]["GroupNum"]);
							value.PeyMethod = Function.ToString(dataTable3.Rows[0]["PeyMethod"]);
							value.Indicator = Function.ToString(dataTable3.Rows[0]["Indicator"]);
							value.LicTradNum = Function.ToString(dataTable3.Rows[0]["LicTradNum"]);
							value.ImportEnt = Function.ParseInt(dataTable3.Rows[0]["ImportEnt"]);
							string text2 = Function.ToString(dataTable3.Rows[0]["PayDuMonth"]);
							if (!string.IsNullOrWhiteSpace(text2))
							{
								switch (text2.ToUpper())
								{
								case "E":
									value.StartFrom = BoPayTermDueTypes.pdt_MonthEnd;
									break;
								case "H":
									value.StartFrom = BoPayTermDueTypes.pdt_HalfMonth;
									break;
								case "Y":
									value.StartFrom = BoPayTermDueTypes.pdt_MonthStart;
									break;
								case "N":
									value.StartFrom = BoPayTermDueTypes.pdt_None;
									break;
								default:
									value.StartFrom = BoPayTermDueTypes.pdt_None;
									break;
								}
							}
							else
							{
								value.StartFrom = BoPayTermDueTypes.pdt_None;
							}
							value.ExtraMonth = Function.ParseInt(dataTable3.Rows[0]["ExtraMonth"]);
							value.ExtraDays = Function.ParseInt(dataTable3.Rows[0]["ExtraDays"]);
							value.CdcOffset = Function.ParseInt(dataTable3.Rows[0]["CdcOffset"]);
							value.UseShpdGd = Function.ToString(dataTable3.Rows[0]["UseShpdGd"]);
						}
						if (FormObjectType == ApzObjectTypes.oReserveInvoices)
						{
							value.ObjType = 13;
						}
						else if (FormObjectType == ApzObjectTypes.oAPReserveInvoice)
						{
							value.ObjType = 18;
						}
						else
						{
							value.ObjType = (int)FormObjectType;
						}
						value.SaveFromForm = true;
						HttpResult httpResult3;
						if (!isDraft && (!value.IsDraft || !(wddStatus == "W") || value.UserSign != GlobalData.UserID))
						{
							httpResult3 = await PostDataSave(value);
						}
						else
						{
							if (value.IsDraft && (wddStatus == "-" || (_onlyChangeRemark && wddStatus == "W" && value.UserSign == GlobalData.UserID)))
							{
								value.DocEntry = value.draftKey;
							}
							if (currMode == Enums.FormViewModeType.Add && wddStatus == "-" && value.Series > 0)
							{
								value.DocNum = -1;
							}
							httpResult3 = await PostDataSaveAsDraft(value);
						}
						if (httpResult3.MessageCode == Enums.MessageCode.Success)
						{
							WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
							int docEntry2 = Function.ParseInt(httpResult3.Content);
							if (docEntry2 <= 0)
							{
								await WinFuntions.PrintPreferences(FormObjectType, docEntry2, FormLayoutCode, ((int)FormObjectType).ToString());
							}
							if (Formcopy is frm_ServiceCalls && OpenFormViewMode == Enums.FormViewModeType.Add)
							{
								OkToClose = true;
								Close();
							}
							Fromentry = "";
							_showForLandedCost = true;
							switch (_saveAction)
							{
							case Enums.SaveAction.SaveAndSearch:
								await ClickToRefresh(reloadADoc: false);
								FormMode = Enums.FormViewModeType.View;
								break;
							case Enums.SaveAction.SaveAndClose:
								FormMode = Enums.FormViewModeType.View;
								Close();
								break;
							case Enums.SaveAction.SaveAndNew:
								await AddDocClick();
								break;
							case Enums.SaveAction.SaveAndView:
								DocEntrySearch = docEntry2;
								FormMode = Enums.FormViewModeType.View;
								btnSearch.PerformClick();
								break;
							}
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult3.Message);
							FormMode = currMode;
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.ItemIsMissing, "");
						FormMode = currMode;
					}
				}
			}
			else
			{
				FormMode = currMode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			FormMode = currMode;
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	protected virtual async Task<HttpResult> PostDataSaveAsDraft(DocumentsImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> documentClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await documentClientHelper.PostAsync("SaveAsDraft", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task<HttpResult> PostDataSave(DocumentsImpl doc)
	{
		_ = 1;
		try
		{
			if (doc.IsDraft && _onlyChangeRemark && (doc.Document_ApprovalRequests == null || doc.Document_ApprovalRequests.Count == 0))
			{
				using (HttpClientHelper<HttpResult> helper_1 = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingProcess"))
				{
					return await helper_1.PostAsync("SaveDraftToDocument", doc);
				}
			}
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("SaveDoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual frm_ChooseFromList ShowFormChooseFromList(Enums.TypeCFL type, bool multiselect, string text)
	{
		try
		{
			return new frm_ChooseFromList(type, multiselect, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, "");
	}

	protected virtual frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		try
		{
			return new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, "");
	}

	protected virtual void LoadPaymentTerm()
	{
		try
		{
			int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			DocumentRow byNum = ((DocumentTable)grdDoc.DataSource).GetByNum(docNum);
			if (byNum == null)
			{
				return;
			}
			int num = Function.ParseInt(spin_Count.EditValue);
			cal_Total.EditValue = byNum.DocTotal;
			DataTable dataTable = ((grdPaymentTerms.DataSourceTable.Rows.Count <= 0 || grdPaymentTerms.DataSourceTable.Rows.Count > num) ? DocumentsHelper.TablePayment.Clone() : grdPaymentTerms.DataSourceTable);
			for (int i = ((grdPaymentTerms.DataSourceTable.Rows.Count > 0 && grdPaymentTerms.DataSourceTable.Rows.Count <= num) ? grdPaymentTerms.DataSourceTable.Rows.Count : 0) + 1; i <= num; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				if (dataRow.IsExistsColumn("U_DueDate"))
				{
					dataRow["U_DueDate"] = byNum.DocDueDate;
				}
				if (dataRow.IsExistsColumn("U_DocNum"))
				{
					dataRow["U_DocNum"] = byNum.DocNum;
				}
				if (dataRow.IsExistsColumn("U_Currency"))
				{
					dataRow["U_Currency"] = byNum.DocCurrency;
				}
				if (dataRow.IsExistsColumn("U_Payments"))
				{
					dataRow["U_Payments"] = i;
				}
				if (dataRow.IsExistsColumn("U_Total"))
				{
					dataRow["U_Total"] = byNum.DocTotal;
				}
				if (dataRow.IsExistsColumn("U_TotalFC"))
				{
					dataRow["U_TotalFC"] = byNum.DocTotalFC;
				}
				dataTable.Rows.Add(dataRow);
			}
			grdPaymentTerms.DataSource = dataTable;
			grdPaymentTerms.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual int CheckEditLine()
	{
		int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
		int result = 0;
		if (num != NewDocEntry)
		{
			if (LstLineEdit != null && LstLineEdit.Count() > 1)
			{
				result = ((LstLineEdit[0].DocEntry != 0 && LstLineEdit[0].DocEntry != num) ? 1 : 2);
			}
		}
		else
		{
			result = 3;
		}
		return result;
	}

	protected virtual async void BindDataCheckWhseCode(string whseCode)
	{
		try
		{
			List<WarehouseImpl> list = await GetWarehouseByCode(whseCode);
			if (list.Count == 1)
			{
				gvLine.SetFocusedRowCellValue(gvLine.Columns.ColumnByFieldName("WhsCode"), list[0].WhsCode);
			}
			else
			{
				ChooseFrmListWhseCode(whseCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ChooseFrmListWhseCode(string whseCode)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.WarehouseFromDocuments, multiSelect: false, whseCode);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				gvLine.SetFocusedRowCellValue("WhsCode", frm_ChooseFromList2.ChooseItem["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task<List<WarehouseImpl>> GetWarehouseByCode(string whsCode)
	{
		try
		{
			if (!string.IsNullOrEmpty(whsCode))
			{
				using (HttpClientHelper<WarehouseImpl> hp = new HttpClientHelper<WarehouseImpl>(Configuration.ApiServer, "Warehouse"))
				{
					List<WarehouseImpl> list = await hp.GetListAsync("CflByWhseCode", new
					{
						whseCode = whsCode
					});
					if (list != null)
					{
						return list;
					}
					return new List<WarehouseImpl>();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<WarehouseImpl>();
	}

	private async Task BindDataGrdLine(DocumentLineTable lst)
	{
		try
		{
			if (gvLine.Columns["ShipToCode"].ColumnEdit is RepositoryItemLookUpEditSimple repShipToCode && IsSalesDocument)
			{
				DataTable dataTable = await HttpClientHelperFactory.GetHttpClient().Documents.GetLineShipTo(Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode")));
				if (dataTable != null && dataTable.Columns.Count > 0 && !dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["Address"]))))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Address"] = "";
					dataTable.Rows.InsertAt(dataRow, 0);
					dataTable.AcceptChanges();
				}
				repShipToCode.DataSource = dataTable;
			}
			DocumentRow byNum = (grdDoc.DataSource as DocumentTable).GetByNum(Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum")));
			string text = ((byNum != null) ? Function.ToString(byNum.DocumentStatus) : "O");
			if (lst.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from t in lst.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"])) && Function.ToString(t["LineType"]) == "R"
					select t;
				int maxDatasourceIndex = (source.Any() ? source.Max((DataRow t) => Function.ParseInt(t["LineNum"])) : (-1));
				lst.MaxDatasourceIndex = maxDatasourceIndex;
			}
			grdLine.DataSource = lst;
			grdLine.RefreshDataSource();
			ExtenseionMethods.ReloadSubTotal(gvLine, 0);
			if ((((FormObjectType == ApzObjectTypes.oQuotations || FormObjectType == ApzObjectTypes.oOrders || FormObjectType == ApzObjectTypes.oPurchaseOrders || FormObjectType == ApzObjectTypes.oPurchaseQuotations || FormObjectType == ApzObjectTypes.oPurchaseRequest) && (text.ToUpper() == "OPEN" || text.ToUpper() == "O")) || ((FormObjectType == ApzObjectTypes.oPurchaseDownPayments || FormObjectType == ApzObjectTypes.oDownPayments) && text == "O" && Function.ToString(gvDoc.GetFocusedRowCellValue("Posted")) == "N") || FormMode == Enums.FormViewModeType.Add) && byNum != null)
			{
				AddNewBlankLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ChooseFormListItemCode(string itemCode, int rowHandle)
	{
		try
		{
			DocumentRow byEntry = ((DocumentTable)grdDoc.DataSource).GetByEntry(Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			if (byEntry == null)
			{
				return;
			}
			if (gvLine.FocusedColumn == gcItemCode)
			{
				DocumentLineTable documentLineTable = grdLine.DataSource as DocumentLineTable;
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, itemCode, "O", (FormBpType == "C") ? Enums.ItemObjectType.SellItem : Enums.ItemObjectType.PurchaseItem, null, "", Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode")));
				frm_CFL2.ShowDialog(this);
				WinCommons._OpenProcessing();
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					gvLine.BeginInsertRow();
					int num = rowHandle;
					int num2 = -1;
					List<DataRow> chooseItems = frm_CFL2.ChooseItems;
					ucBatchAndSerial1.ClearLineDataSource();
					foreach (DataRow item in chooseItems)
					{
						if (documentLineTable != null)
						{
							DocumentLineRow newRow = documentLineTable.GetNewRow();
							if (num == rowHandle)
							{
								newRow.LineNum = Function.ParseInt(gvLine.GetRowCellValue(rowHandle, "LineNum"));
								ucBatchAndSerial1.BeginUpdateLine();
								WinFuntions.DeleteBomChildsItem(gvLine, gvLine.FocusedRowHandle, ucBatchAndSerial1, isChooseItem: true);
								ucBatchAndSerial1.EndUpdateLine();
							}
							documentLineTable.Insert(num, newRow);
							documentLineTable.AcceptChanges();
							grdLine.DataSource = documentLineTable;
							grdLine.RefreshDataSource();
							num2 = gvLine.FindRowHandleByDataRow(newRow);
							if (num2 < 0)
							{
								continue;
							}
							gvLine.FocusedRowHandle = num2;
							if (num == rowHandle)
							{
								int num3 = Function.ParseInt(documentLineTable[num]["LineNum"]);
								gvLine.SetRowCellValue(num2, "LineNum", (num3 >= 0) ? num3 : (-1));
							}
						}
						gvLine.SetRowCellValue(num2, "ItemCode", Function.ToString(item["ItemCode"]));
						gvLine.SetRowCellValue(num2, "DocEntry", byEntry.DocEntry);
						gvLine.SetRowCellValue(num2, "LineStatus", "O");
						gvLine.SetRowCellValue(num2, "BaseEntry", -1);
						gvLine.SetRowCellValue(num2, "BaseLine", -1);
						gvLine.SetRowCellValue(num2, "BaseType", -1);
						num++;
					}
					gvLine.FocusedColumn = gvLine.Columns.FirstOrDefault((GridColumn t) => t.FieldName.Equals("Quantity"));
				}
				else if (!string.IsNullOrEmpty(itemCode))
				{
					gvLine.FocusedColumn = gvLine.VisibleColumns[0];
					gvLine.ShowEditor();
				}
				_OpenChooseCFL = true;
				gvLine.EndInsertRow();
				if (documentLineTable != null)
				{
					documentLineTable.AcceptChanges();
					LstLineEdit = documentLineTable;
				}
				grdLine.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gvLine.EndInsertRow();
		WinCommons._CloseProcessing();
	}

	protected virtual void AddNewBlankLine(int rowhandle = -1)
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				return;
			}
			int docEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
			if (DocumentsHelper.Lines != null)
			{
				LstLineEdit = DocumentsHelper.Lines.GetClone();
			}
			if (grdLine.DataSource != null)
			{
				LstLineEdit = grdLine.DataSource as DocumentLineTable;
			}
			if (LstLineEdit != null && LstLineEdit.GetEnumerable().Any((DocumentLineRow x) => string.IsNullOrEmpty(x.ItemCode) && x.LineType == "R") && rowhandle == -1)
			{
				return;
			}
			if (LstLineEdit != null)
			{
				DocumentLineRow line = LstLineEdit.GetNewRow();
				line.DocEntry = docEntry;
				line.LineNum = -1;
				line.ItemCode = "";
				line.LineType = "R";
				line.SubCatNum = "";
				line.ItemDescription = "";
				line.ShipDate = DateTime.Today;
				line.Rate = 1.0;
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
				line.Currency = (string.IsNullOrWhiteSpace(text) ? GlobalData.MainCurrency : text);
				int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocRate"));
				line.RateCurrency = ((num <= 0) ? 1 : num);
				line.Quantity = 0.0;
				line.BOMTreeType = "";
				line.TreeType = "N";
				line.BaseEntry = -1;
				line.BaseLine = -1;
				line.BaseType = -1;
				line.LineStatusName = "O";
				line.PQTReqDate = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("ReqDate")) ?? DateTime.Today;
				LoadDefaultValuePerDocument(ref line);
				if (rowhandle >= 0 && gvLine.GetDataSourceRowIndex(rowhandle) + 1 <= LstLineEdit.Count - 1)
				{
					LstLineEdit.Insert(gvLine.GetDataSourceRowIndex(rowhandle) + 1, line);
				}
				else
				{
					LstLineEdit.Add(line);
				}
			}
			grdLine.DataSource = LstLineEdit;
			grdLine.RefreshDataSource();
			gcDoc_CardCode.OptionsColumn.ReadOnly = FormMode == Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void LoadDefaultValuePerDocument(ref DocumentLineRow line)
	{
	}

	protected virtual void SetDataByCardCode(DataRow dr)
	{
		try
		{
			WinCommons._OpenProcessing();
			string value = Function.ToString(dr["CardCode"]);
			gvDoc.SetFocusedRowCellValue("CardCode", value);
			_downPaymentData = null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async void SetDataByCardCode(BusinessPartnersImpl buss)
	{
		try
		{
			int rowHandle = gvDoc.FocusedRowHandle;
			DocumentTable lst = (DocumentTable)grdDoc.DataSource;
			DocumentRow doc = lst.GetByNum(Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum")));
			doc.CardCode = buss.CardCode;
			doc.CardName = buss.CardName;
			doc.ContactPersonCode = Function.ParseInt(buss.CntctCode);
			doc.SlpCode = Function.ParseInt(buss.SalesPersonCode);
			doc.SalesPersonCode = Function.ParseInt(buss.SalesPersonCode);
			doc.TransportationCode = buss.ShippingType;
			doc.ShipToCode = buss.ShipToDefault;
			doc.PayToCode = buss.BilltoDefault;
			doc.DocDate = DateTime.Today;
			DataTable dataTable = await ExtenseionMethods.GetEmployeeCodeBySlpCode(buss.SalesPersonCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				doc.OwnerCode = Function.ParseInt(dataTable.Rows[0]["OwnerCode"]);
				doc.OwnerName = Function.ToString(dataTable.Rows[0]["OwnerName"]);
			}
			lst.AcceptChanges();
			grdLogistic.RefreshDataSource();
			grdDoc.Refresh();
			gvDoc.FocusedColumn = gcDoc_CardName;
			_ = string.Empty;
			string value = ((!string.IsNullOrEmpty(buss.Currency) && !buss.Currency.Equals("##")) ? buss.Currency : GlobalData.MainCurrency);
			gvDoc.SetRowCellValue(rowHandle, "DocCur", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ChooseFormListCardCode(string cardcode)
	{
		try
		{
			DataRow choosenCardCode = GetChoosenCardCode(cardcode);
			if (choosenCardCode != null)
			{
				SetDataByCardCode(choosenCardCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataRow GetChoosenCardCode(string cardcode)
	{
		Enums.TypeCFL type = Enums.TypeCFL.Customer;
		if (FormBpType == "S")
		{
			type = Enums.TypeCFL.Vendor;
		}
		else if (new ApzObjectTypes[2]
		{
			ApzObjectTypes.oOrders,
			ApzObjectTypes.oQuotations
		}.Contains(FormObjectType))
		{
			type = Enums.TypeCFL.CustomerLead;
		}
		else if (FormBpType == "L")
		{
			type = Enums.TypeCFL.Lead;
		}
		frm_CFL frm_CFL2 = new frm_CFL(type, false, cardcode, "", Enums.ItemObjectType.NoItemType, null, "");
		frm_CFL2.ShowDialog(this);
		return frm_CFL2.ChooseItem;
	}

	private async Task CancelCloseDelete(string function, string result)
	{
		int docEntry = CheckSelectedRow(gvDoc);
		if (docEntry > 0)
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				return;
			}
			if (function == "Cancel")
			{
				bool flag = true;
				if (new ApzObjectTypes[5]
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oPurchaseQuotations,
					ApzObjectTypes.oPurchaseOrders,
					ApzObjectTypes.oPurchaseRequest
				}.Contains(FormObjectType) || ((FormObjectType == ApzObjectTypes.oDownPayments || FormObjectType == ApzObjectTypes.oPurchaseDownPayments) && Function.ToString(gvDoc.GetFocusedRowCellValue("Posted")) == "N"))
				{
					if (GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CancelOtherDoc" && f.authoriza == "F").FirstOrDefault() == null)
					{
						flag = false;
					}
				}
				else if (GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CancelDocGenCancelation" && f.authoriza == "F").FirstOrDefault() == null)
				{
					flag = false;
				}
				if (!flag)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}
			else if (function == "Close" && GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CloseDocument" && f.authoriza == "F").FirstOrDefault() == null)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			Enums.MessageCode mesageCode;
			if (function == "Delete")
			{
				mesageCode = Enums.MessageCode.ConfirmRemoveByDocNum;
			}
			else
			{
				mesageCode = ((function == "Cancel") ? Enums.MessageCode.ConfirmCancelDocumentByDocNum : Enums.MessageCode.ConfirmCloseDocumentByDocNum);
			}
			if (XtraMessageBoxEx.Show(mesageCode, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Function.ToString(num)) == DialogResult.Yes)
			{
				Enums.FormViewModeType oldViewMode = FormMode;
				FormMode = Enums.FormViewModeType.Busy;
				HttpResult httpResult;
				using (HttpClientHelper<HttpResult> documentClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController))
				{
					WinCommons._OpenProcessing();
					httpResult = await documentClientHelper.PostAsync(function, docEntry);
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Success, Msg_Time._TimeMedium, result);
					DocEntrySearch = docEntry;
					FormMode = Enums.FormViewModeType.View;
					btnSearch.PerformClick();
				}
				else
				{
					FormMode = oldViewMode;
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				WinCommons._CloseProcessing();
			}
		}
		else
		{
			Enums.MessageCode mesgCode;
			if (function == "Delete")
			{
				mesgCode = Enums.MessageCode.ChooseItemDelete;
			}
			else
			{
				mesgCode = ((function == "Cancel") ? Enums.MessageCode.ChooseItemCancel : Enums.MessageCode.ChooseItemClose);
			}
			WinCommons.writeStatus(mesgCode, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
		}
	}

	private int CheckSelectedRow(GridViewEx gv)
	{
		try
		{
			int result = 0;
			int[] selectedRows = gv.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				result = Function.ParseInt(gv.GetRowCellValue(selectedRows[0], "DocEntry").ToString());
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	protected virtual async Task GetChildDataByID(int docEntry)
	{
		try
		{
			await Task.WhenAll(GetDocLine(docEntry), GetPaymentTerm(FormObjectType, docEntry), GetCombobox(docEntry, Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"))), GetFreight(DocFreightTable, docEntry), Task.Run(() =>
			{
				_currentRefDoc = _httpClient.Documents.GetReferencedDocument(docEntry);
			}), LoadTabAccounting(docEntry), ucAttachment1.LoadAttachments(Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry"))));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetBatchSerial(int docEntry)
	{
		DataTable dataTable = await Task.Run(() => _httpClient.Documents.GetBatchSerialByDocEntry(docEntry));
		if (dataTable.IsNotNull())
		{
			DataTable dataTable2 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			string[] columnNames = (from DataColumn t in dataTable2.Columns
				select t.ColumnName).ToArray();
			dataTable = dataTable.DefaultView.ToTable("Source", distinct: false, columnNames);
			DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
			if (dataRow != null)
			{
				dataTable2.Rows.Remove(dataRow);
				dataTable2.AcceptChanges();
			}
			foreach (DataRow row in dataTable.Rows)
			{
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3.ItemArray = row.ItemArray;
				dataTable2.Rows.Add(dataRow3);
			}
			ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
		}
		else
		{
			ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
		}
	}

	private async Task GetBinAllocate(int docEntry)
	{
		DataTable dataTable = await Task.Run(() => _httpClient.BinLocMasterDataClient.GetBatchBinAllowcateLineByDocEntry(Function.ToString(docEntry), FormObjectType));
		ucBatchAndSerial1.DataSourceBinAllowLine = (dataTable.IsNotNull() ? dataTable : DocumentsHelper.BatchSerialNumbersStruct.Copy());
	}

	private async Task GetDocLine(int docEntry)
	{
		gvLine.ShowLoadingPanel();
		try
		{
			Task<DocumentLineTable> line = _httpClient.Documents.GetLine(docEntry, FilterLine);
			Task t1 = GetBatchSerial(docEntry);
			Task t2 = GetBinAllocate(docEntry);
			DocumentLineTable lst = await line;
			WinFuntions.SetBOMTreeType(ref lst, ref LstBomChilds);
			await Task.WhenAll(t1, t2);
			await BindDataGrdLine(lst);
			if (LineNumFocus >= 0)
			{
				DataRow dataRow = grdLine.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow dataRow2) => Function.ParseInt(dataRow2["LineNum"]) == LineNumFocus);
				if (dataRow != null)
				{
					gvLine.FocusedRowHandle = gvLine.FindRowHandleByDataRow(dataRow);
				}
				LineNumFocus = -1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			gvLine.HideLoadingPanel();
		}
	}

	private async Task GetPaymentTerm(ApzObjectTypes objType, int docEntry)
	{
		if (string.IsNullOrEmpty(_customizePaymentTerms))
		{
			_customizePaymentTerms = await _httpClient.Commons.CheckExistColumn("ADOC", "U_paymentEntry");
		}
		if (_customizePaymentTerms == "N")
		{
			return;
		}
		gvPayTerms.ShowLoadingPanel();
		try
		{
			DataTable dataSource = await Task.Run(() => _httpClient.Documents.GetPaymentTerms(docEntry, objType));
			grdPaymentTerms.DataSource = dataSource;
			grdPaymentTerms.RefreshDataSource();
			cal_Total.Value = Function.ParseDecimal(gvDoc.GetFocusedRowCellValue("Total"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			gvPayTerms.HideLoadingPanel();
		}
	}

	private async Task GetFreight(string tblName, int docEntry)
	{
		gv_Freight.ShowLoadingPanel();
		try
		{
			await CreateFreightGridDatasource();
			RefreshFreightGrid(await Task.Run(() => _httpClient.Commons.GetFreight(tblName, docEntry.ToString())));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			gv_Freight.HideLoadingPanel();
		}
	}

	private async Task GetCombobox(int docEntry, string cardCode)
	{
		gv_Logistic.ShowLoadingPanel();
		try
		{
			DataSet dataSet = await Task.Run(() => _httpClient.Commons.GetDataCombobox(cardCode));
			if (dataSet != null && dataSet.Tables.Count > 3)
			{
				rBillTo.DataSource = dataSet.Tables[0];
				rShipTo.DataSource = dataSet.Tables[1];
				rlpShipType.DataSource = dataSet.Tables[2];
				rlupContactPersonCode.DataSource = dataSet.Tables[3];
			}
			GetLogistic(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			gv_Logistic.HideLoadingPanel();
		}
	}

	private void GetLogistic(int docEntry)
	{
		try
		{
			DocumentTable documentTable = grdDoc.DataSource as DocumentTable;
			if (!documentTable.IsNotNull())
			{
				return;
			}
			DocumentTable clone = documentTable.GetClone();
			if (clone != null && documentTable.ExistsColumn("DocEntry"))
			{
				DataRow[] array = documentTable.Select("DocEntry='" + docEntry + "'");
				for (int i = 0; i < array.Length; i++)
				{
					DocumentRow documentRow = (DocumentRow)array[i];
					DataRow dataRow = clone.NewRow();
					dataRow.ItemArray = documentRow.ItemArray;
					clone.Rows.Add(dataRow);
				}
				grdLogistic.DataSource = clone;
				grdLogistic.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void LoadCopyMenu()
	{
		Task.Run(() =>
		{
			try
			{
				if (PopCopyFrom != null && PopCopyFrom.Count > 0)
				{
					DXPopupMenu dXPopupMenu = new DXPopupMenu();
					foreach (ApzObjectTypes item in PopCopyFrom)
					{
						DXMenuItem dXMenuItem = new DXMenuItem();
						switch (item)
						{
						case ApzObjectTypes.oQuotations:
							dXMenuItem.Caption = MenuCaption.SalesQuotation;
							break;
						case ApzObjectTypes.oOrders:
							dXMenuItem.Caption = MenuCaption.SalesOrder;
							break;
						case ApzObjectTypes.oDeliveryNotes:
							dXMenuItem.Caption = MenuCaption.Delivery;
							break;
						case ApzObjectTypes.oReturns:
							dXMenuItem.Caption = MenuCaption.Return;
							break;
						case ApzObjectTypes.oInvoices:
							dXMenuItem.Caption = MenuCaption.ARInvoices;
							break;
						case ApzObjectTypes.oCreditNotes:
							dXMenuItem.Caption = MenuCaption.ARCreditMemos;
							break;
						case ApzObjectTypes.oPurchaseQuotations:
							dXMenuItem.Caption = MenuCaption.PurchaseQuotation;
							break;
						case ApzObjectTypes.oPurchaseRequest:
							dXMenuItem.Caption = MenuCaption.PurchaseRequest;
							break;
						case ApzObjectTypes.oPurchaseOrders:
							dXMenuItem.Caption = MenuCaption.PurchaseOder;
							break;
						case ApzObjectTypes.oPurchaseDeliveryNotes:
							dXMenuItem.Caption = MenuCaption.GoodReceiptPO;
							break;
						case ApzObjectTypes.oPurchaseReturns:
							dXMenuItem.Caption = MenuCaption.GoodsReturn;
							break;
						case ApzObjectTypes.oPurchaseInvoices:
							dXMenuItem.Caption = ((CheckAri == "Y") ? MenuCaption.APReserveInvoice : MenuCaption.APInvoices);
							break;
						case ApzObjectTypes.oPurchaseCreditNotes:
							dXMenuItem.Caption = MenuCaption.APCreditMemos;
							break;
						case ApzObjectTypes.oShipping:
							dXMenuItem.Caption = ((MenuCaption.Shipping == "") ? MenuCaption.Shipping : "Shipping");
							break;
						case ApzObjectTypes.oAPReserveInvoice:
							dXMenuItem.Caption = MenuCaption.AR_ReserveInvoices;
							break;
						case ApzObjectTypes.oReserveInvoices:
							dXMenuItem.Caption = MenuCaption.AR_ReserveInvoices;
							break;
						case ApzObjectTypes.oBlanketAgreement:
							dXMenuItem.Caption = MenuCaption.BlanketAgreement;
							break;
						case ApzObjectTypes.oReturnRequest:
							dXMenuItem.Caption = MenuCaption.ReturnRequest;
							break;
						case ApzObjectTypes.oGoodsReturnRequest:
							dXMenuItem.Caption = MenuCaption.GoodsReturnRequest;
							break;
						}
						ApzObjectTypes fromObjType = item;
						dXMenuItem.Click += async (object s, EventArgs e) =>
						{
							WinCommons._OpenProcessing();
							try
							{
								string text = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
								if (string.IsNullOrEmpty(text))
								{
									WinCommons.Error(Enums.MessageCode.YouHaveChooseWithColumnCaption, "", gvDoc.Columns["CardCode"].Caption);
									return;
								}
								if (fromObjType == ApzObjectTypes.oShipping)
								{
									frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Shipping, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL2.BpCode = text;
									frm_CFL2.ShowDialog(this);
									if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
									{
										List<DocumentsImpl> list = new List<DocumentsImpl>();
										foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
										{
											list.Add(new DocumentsImpl
											{
												CardCode = text,
												DocEntry = Function.ParseInt(chooseItem["DocEntry"])
											});
										}
										await CopyFrom(fromObjType, list, 0);
									}
								}
								else
								{
									if (fromObjType == ApzObjectTypes.oBlanketAgreement && (FormObjectType == ApzObjectTypes.oPurchaseQuotations || FormObjectType == ApzObjectTypes.oPurchaseOrders || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes))
									{
										fromObjType = ApzObjectTypes.oPurchaseBlanketAgreement;
									}
									ApzObjectTypes toType = FormObjectType;
									if (DownPaymentType == Enums.DownPaymentType.dptInvoice)
									{
										if (FormObjectType == ApzObjectTypes.oDownPayments)
										{
											toType = ApzObjectTypes.oDownPaymentInvoice;
										}
										else if (FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
										{
											toType = ApzObjectTypes.oPurchaseDownPaymentInvoice;
										}
									}
									using HttpClientHelper<DocumentLineTable> docLineTableHelper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, FormController);
									frm_CopyWizardPage frm_CopyWizardPage2 = new frm_CopyWizardPage(fromObjType, toType, text, Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("DocDate")), docLineTableHelper);
									frm_CopyWizardPage2.TableFreight = GetFromFreightTableName(fromObjType);
									frm_CopyWizardPage2.FillDrawAll = CopyFromDrawAll;
									frm_CopyWizardPage2.FillCustomize = CopyFromCustomize;
									frm_CopyWizardPage2.ShowDialog(this);
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
								WinCommons.ErrorLineNumber(ex2);
							}
							WinCommons._CloseProcessing();
						};
						dXPopupMenu.Items.Add(dXMenuItem);
					}
					drlCopyFrom.DropDownControl = dXPopupMenu;
				}
				if (PopCopyTo != null && PopCopyTo.Count > 0)
				{
					DXPopupMenu dXPopupMenu2 = new DXPopupMenu();
					foreach (ApzObjectTypes item2 in PopCopyTo)
					{
						DXMenuItem dXMenuItem2 = new DXMenuItem();
						switch (item2)
						{
						case ApzObjectTypes.oQuotations:
							dXMenuItem2.Caption = MenuCaption.SalesQuotation;
							break;
						case ApzObjectTypes.oOrders:
							dXMenuItem2.Caption = MenuCaption.SalesOrder;
							break;
						case ApzObjectTypes.oDeliveryNotes:
							dXMenuItem2.Caption = MenuCaption.Delivery;
							break;
						case ApzObjectTypes.oReturns:
							dXMenuItem2.Caption = MenuCaption.Return;
							break;
						case ApzObjectTypes.oInvoices:
							dXMenuItem2.Caption = MenuCaption.ARInvoices;
							break;
						case ApzObjectTypes.oReserveInvoices:
							dXMenuItem2.Caption = MenuCaption.AR_ReserveInvoices;
							break;
						case ApzObjectTypes.oCreditNotes:
							dXMenuItem2.Caption = MenuCaption.ARCreditMemos;
							break;
						case ApzObjectTypes.oPurchaseQuotations:
							dXMenuItem2.Caption = MenuCaption.PurchaseQuotation;
							break;
						case ApzObjectTypes.oPurchaseRequest:
							dXMenuItem2.Caption = MenuCaption.PurchaseRequest;
							break;
						case ApzObjectTypes.oPurchaseOrders:
							dXMenuItem2.Caption = MenuCaption.PurchaseOder;
							break;
						case ApzObjectTypes.oPurchaseDeliveryNotes:
							dXMenuItem2.Caption = MenuCaption.GoodReceiptPO;
							break;
						case ApzObjectTypes.oPurchaseReturns:
							dXMenuItem2.Caption = MenuCaption.GoodsReturn;
							break;
						case ApzObjectTypes.oPurchaseInvoices:
							if (CheckAri == "Y")
							{
								dXMenuItem2.Caption = MenuCaption.APReserveInvoice;
							}
							else
							{
								dXMenuItem2.Caption = MenuCaption.APInvoices;
							}
							break;
						case ApzObjectTypes.oPurchaseCreditNotes:
							dXMenuItem2.Caption = MenuCaption.APCreditMemos;
							break;
						case ApzObjectTypes.oAPReserveInvoice:
							dXMenuItem2.Caption = MenuCaption.APReserveInvoice;
							break;
						case ApzObjectTypes.oReturnRequest:
							dXMenuItem2.Caption = MenuCaption.ReturnRequest;
							break;
						case ApzObjectTypes.oGoodsReturnRequest:
							dXMenuItem2.Caption = MenuCaption.GoodsReturnRequest;
							break;
						case ApzObjectTypes.oDownPayments:
							dXMenuItem2.Caption = MenuCaption.ARDownpaymentRequest;
							break;
						case ApzObjectTypes.oDownPaymentInvoice:
							dXMenuItem2.Caption = MenuCaption.ARDownpaymentInvoice;
							break;
						case ApzObjectTypes.oPurchaseDownPayments:
							dXMenuItem2.Caption = MenuCaption.APDownpaymentRequest;
							break;
						case ApzObjectTypes.oPurchaseDownPaymentInvoice:
							dXMenuItem2.Caption = MenuCaption.APDownpaymentInvoice;
							break;
						}
						ApzObjectTypes objType = item2;
						dXMenuItem2.Click += (object s, EventArgs e) =>
						{
							try
							{
								ApzObjectTypes totype = ((objType == ApzObjectTypes.oReserveInvoices) ? ApzObjectTypes.oInvoices : objType);
								WinFuntions.CopyToDocument(FormObjectType, totype, this, gvDoc, objType == ApzObjectTypes.oReserveInvoices || CheckAri == "Y");
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
							}
						};
						dXPopupMenu2.Items.Add(dXMenuItem2);
					}
					drlCopyTo.DropDownControl = dXPopupMenu2;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		});
	}

	protected virtual void LoadDropDownSave(Enums.FormViewModeType formView)
	{
		switch (formView)
		{
		case Enums.FormViewModeType.View:
			btn_Save.DropDownControl = null;
			break;
		case Enums.FormViewModeType.Update:
		case Enums.FormViewModeType.Add:
		{
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
			string text = ((formView == Enums.FormViewModeType.Update) ? "Update && " : "Add && ");
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			DXMenuItem barNew = new DXMenuItem();
			barNew.Caption = text + "New";
			barNew.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barNew.Caption;
					_saveAction = Enums.SaveAction.SaveAndNew;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barNew);
			DXMenuItem barView = new DXMenuItem();
			barView.Caption = text + "View";
			barView.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barView.Caption;
					_saveAction = Enums.SaveAction.SaveAndView;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barView);
			DXMenuItem barSearch = new DXMenuItem();
			barSearch.Caption = text + "Search";
			barSearch.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barSearch.Caption;
					_saveAction = Enums.SaveAction.SaveAndSearch;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barSearch);
			DXMenuItem barClose = new DXMenuItem();
			barClose.Caption = text + "Close";
			barClose.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barClose.Caption;
					_saveAction = Enums.SaveAction.SaveAndClose;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barClose);
			btn_Save.DropDownControl = dXPopupMenu;
			break;
		}
		}
	}

	private async Task CopyFrom(ApzObjectTypes fromObjectType, List<DocumentsImpl> lst, int setRateType)
	{
		_ = 2;
		try
		{
			gvLine.ShowLoadingPanel();
			string card = lst[0].CardCode;
			if (lst.Exists((DocumentsImpl x) => x.CardCode != card))
			{
				WinCommons.Error(Enums.MessageCode.DocumentMustCopyFromSameCustomer, "");
				return;
			}
			if (!string.IsNullOrEmpty(card))
			{
				gvDoc.SetFocusedRowCellValue("CardCode", card);
			}
			ApzObjectTypes toType = FormObjectType;
			if (DownPaymentType == Enums.DownPaymentType.dptInvoice)
			{
				if (FormObjectType == ApzObjectTypes.oDownPayments)
				{
					toType = ApzObjectTypes.oDownPaymentInvoice;
				}
				else if (FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
				{
					toType = ApzObjectTypes.oPurchaseDownPaymentInvoice;
				}
			}
			string entry = string.Join(",", lst.Select((DocumentsImpl t) => t.DocEntry.ToString()).ToArray());
			DocumentTable documentTable = await _httpClient.Documents.CopyHeader(fromObjectType, toType, entry);
			if (documentTable.IsNotNull())
			{
				int bplId = Function.ParseInt(documentTable.Rows[0]["BPLId"]);
				int defautSeries = GetDefautSeries(bplId);
				documentTable.Rows[0]["Series"] = defautSeries;
			}
			if (!string.IsNullOrEmpty(card))
			{
				grdDoc.DataSource = documentTable;
				grdDoc.RefreshDataSource();
			}
			if (_documentSetting.CpyCVRef.Equals("Y"))
			{
				string[] source = (from t in lst
					where !string.IsNullOrEmpty(t.NumAtCard)
					select t.NumAtCard).ToArray();
				if (source.Any())
				{
					gvDoc.SetFocusedRowCellValue("NumAtCard", string.Join(";", source.Where((string t) => !string.IsNullOrEmpty(Function.ToString(t)))));
				}
			}
			DocumentLineTable dt = await _httpClient.Documents.Copy(fromObjectType, toType, entry);
			ucBatchAndSerial1.FromReserve = Fromtype == ApzObjectTypes.oAPReserveInvoice || Fromtype == ApzObjectTypes.oReserveInvoices;
			if (ucBatchAndSerial1.CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && ucBatchAndSerial1.CopyObjectNotBatchAndSerial[FormObjectType].Contains(fromObjectType))
			{
				ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			}
			else
			{
				DataTable dataTable = _httpClient.Documents.CopyBatchSerial(entry, fromObjectType);
				if (dataTable.IsNotNull())
				{
					DataTable dataTable2 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
					string[] columnNames = (from DataColumn t in dataTable2.Columns
						select t.ColumnName).ToArray();
					dataTable = dataTable.DefaultView.ToTable("Source", distinct: false, columnNames);
					DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
					if (dataRow != null)
					{
						dataTable2.Rows.Remove(dataRow);
						dataTable2.AcceptChanges();
					}
					foreach (DataRow row in dataTable.Rows)
					{
						DataRow dataRow3 = dataTable2.NewRow();
						dataRow3.ItemArray = row.ItemArray;
						dataTable2.Rows.Add(dataRow3);
					}
					dataTable2.Rows.Add(dataTable2.NewRow());
					ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
				}
				else
				{
					ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				}
				DataTable batchBinAllowcateLineByDocEntry = _httpClient.BinLocMasterDataClient.GetBatchBinAllowcateLineByDocEntry(entry, fromObjectType);
				if (batchBinAllowcateLineByDocEntry.IsNotNull())
				{
					foreach (DataRow r in batchBinAllowcateLineByDocEntry.Rows)
					{
						DataRow dataRow4 = dt.AsEnumerable().FirstOrDefault((DataRow n) => Function.ParseInt(n["BaseEntry"]) == Function.ParseInt(r["BaseDocEntry"]) && Function.ParseInt(n["BaseLine"]) == Function.ParseInt(r["BaseLineNumber"]));
						if (dataRow4 != null)
						{
							r["BaseLineNumber"] = dataRow4["LineNum"];
						}
					}
				}
				ucBatchAndSerial1.DataSourceBinAllowLine = (batchBinAllowcateLineByDocEntry.IsNotNull() ? batchBinAllowcateLineByDocEntry : DocumentsHelper.BatchSerialNumbersStruct.Copy());
			}
			WinFuntions.SetBOMTreeType(ref dt, ref LstBomChilds);
			UpdatePacQtyToDocumentCopy(dt);
			switch (fromObjectType)
			{
			case ApzObjectTypes.oBlanketAgreement:
				foreach (DataRow row2 in dt.Rows)
				{
					row2["VatGroup"] = row2["VatGourpSa"];
					if (Function.ParseInt(row2["UomEntry"]) < 0)
					{
						row2["UomCode"] = row2["SUoMCode"];
						row2["unitMsr"] = row2["SalUnitMsr"];
						row2["NumPerMsr"] = row2["NumInSale"];
						row2["InvQty"] = Function.ParseDouble(row2["Quantity"]) * Function.ParseDouble(row2["NumPerMsr"]);
						row2["PriceBefDi"] = Function.ParseDouble(row2["PriceBefDi"]) * Function.ParseDouble(row2["NumPerMsr"]);
					}
					else
					{
						row2["unitMsr"] = row2["InvntryUom"];
					}
				}
				break;
			case ApzObjectTypes.oPurchaseBlanketAgreement:
				foreach (DataRow row3 in dt.Rows)
				{
					row3["VatGroup"] = row3["VatGroupPu"];
					if (Function.ParseInt(row3["UomEntry"]) < 0)
					{
						row3["UomCode"] = row3["PUoMCode"];
						row3["unitMsr"] = row3["BuyUnitMsr"];
						row3["NumPerMsr"] = row3["NumInBuy"];
						row3["InvQty"] = Function.ParseDouble(row3["Quantity"]) * Function.ParseDouble(row3["NumPerMsr"]);
						row3["PriceBefDi"] = Function.ParseDouble(row3["PriceBefDi"]) * Function.ParseDouble(row3["NumPerMsr"]);
					}
					else
					{
						row3["unitMsr"] = row3["InvntryUom"];
					}
				}
				break;
			}
			grdLine.DataSource = dt;
			grdLine.RefreshDataSource();
			SetTotalAfterCopy(fromObjectType);
			LstLineEdit = dt;
			SetExchangeRateCopy(setRateType);
			await LoadTabAccounting(Function.ParseInt(entry), isCopyFrom: true, fromObjectType);
			if (FormObjectType == ApzObjectTypes.oInvoices || FormObjectType == ApzObjectTypes.oPurchaseInvoices || FormObjectType == ApzObjectTypes.oReserveInvoices || FormObjectType == ApzObjectTypes.oAPReserveInvoice || FormObjectType == ApzObjectTypes.oCreditNotes || FormObjectType == ApzObjectTypes.oPurchaseCreditNotes)
			{
				PaymentTermInfo = _httpClient.PaymentTermSetup.GetPaymentTermInfo(Function.ParseInt(gv_Accounting.GetFocusedRowCellValue("GroupNum")));
				if (PaymentTermInfo.IsNotNull())
				{
					PaymentTermInfo.Rows[0]["PayDuMonth"] = gv_Accounting.GetFocusedRowCellValue("PayDuMonth");
					PaymentTermInfo.Rows[0]["ExtraMonth"] = gv_Accounting.GetFocusedRowCellValue("ExtraMonth");
					PaymentTermInfo.Rows[0]["ExtraDays"] = gv_Accounting.GetFocusedRowCellValue("ExtraDays");
					CalculateDueDate();
				}
			}
			DataRow[] array = (from t in _httpClient.Commons.GetFreight(GetFromFreightTableName(fromObjectType), entry).AsEnumerable()
				where Function.ToString(t["Status"]) != "C"
				select t).ToArray();
			if (array.Length != 0)
			{
				if (fromObjectType != ApzObjectTypes.oDrafts)
				{
					DataRow[] array2 = array;
					foreach (DataRow dataRow7 in array2)
					{
						dataRow7["BaseType"] = (int)fromObjectType;
						dataRow7["BaseAbsEnt"] = dataRow7["DocEntry"];
						dataRow7["BaseLnNum"] = dataRow7["LineNum"];
					}
				}
				RefreshFreightGrid(array.CopyToDataTable());
			}
			AddNewBlankLine();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvLine.HideLoadingPanel();
		}
	}

	private void UpdatePacQtyToDocumentCopy(DataTable dtLine)
	{
		if (FormBpType == "C")
		{
			for (int i = 0; i < dtLine.Rows.Count; i++)
			{
				double num = Function.ParseDouble(dtLine.Rows[i]["SalPackUn"]);
				if (Math.Abs(num) < 1E-07)
				{
					num = 1.0;
				}
				double num2 = Function.ParseDouble(dtLine.Rows[i]["Quantity"]) / num;
				double num3 = Math.Truncate(num2);
				dtLine.Rows[i]["PackQty"] = ((num2 - num3 > 1E-07) ? (num3 + 1.0) : num3);
			}
		}
		else if (FormBpType == "S")
		{
			for (int j = 0; j < dtLine.Rows.Count; j++)
			{
				double num4 = Function.ParseDouble(dtLine.Rows[j]["PurPackUn"]);
				if (Math.Abs(num4) < 1E-07)
				{
					num4 = 1.0;
				}
				double num5 = Function.ParseDouble(dtLine.Rows[j]["Quantity"]) / num4;
				double num6 = Math.Truncate(num5);
				dtLine.Rows[j]["PackQty"] = ((num5 - num6 > 1E-07) ? (num6 + 1.0) : num6);
			}
		}
		dtLine.AcceptChanges();
	}

	private int GetObjTypeGetRecord()
	{
		if (DownPaymentType == Enums.DownPaymentType.dptInvoice)
		{
			if (FormObjectType == ApzObjectTypes.oDownPayments)
			{
				return 203000;
			}
			if (FormObjectType == ApzObjectTypes.oPurchaseDownPayments)
			{
				return 204000;
			}
		}
		return (int)FormObjectType;
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			await GetRecord(GetObjTypeGetRecord(), FormDocumentTable, 0, "F");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			await GetRecord(GetObjTypeGetRecord(), FormDocumentTable, 0, "L");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			await GetRecord(GetObjTypeGetRecord(), FormDocumentTable, docNum, "N");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			await GetRecord(GetObjTypeGetRecord(), FormDocumentTable, docNum, "P");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (FormAuth == Enums.FormAuthorities.Full)
			{
				if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
				{
					await AddDocClick();
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					await AddDocClick();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")).ToString();
				Enums.ChangeLog changeLogType = ChangeLogType;
				Apzon.Entities.Models.Common.GetChangeLogParams getChangeLogParams = new Apzon.Entities.Models.Common.GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLogType;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		await ClickToRefresh();
	}

	public async Task LoadComboFreight()
	{
		try
		{
			rep_DistriMethod.SetDataSource(new Dictionary<string, string>
			{
				{ "N", "None" },
				{ "Q", "Quantity" },
				{ "V", "Volume" },
				{ "W", "Weight" },
				{ "E", "Equally" },
				{ "T", "Row Total" }
			});
			rep_StatusFreight.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Closed" }
			});
			rep_LstPrice.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			rep_FreightStock.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			rep_Stock.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			rep_LstPchPrce.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			DataTable dataSource = await _httpClient.Documents.GetFreightCode();
			rep_FreightCode.DataSource = dataSource;
			rep_ExpenseCode.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRule(gv, row);
			}
			for (int i = 1; i < 4; i++)
			{
				foreach (DataRow row2 in GlobalData.Dimensions.Rows)
				{
					AddDistRule(gv, row2, "_F" + i, "Freight " + i);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task CreateShippingColumns()
	{
		try
		{
			if (gvLine.Columns["ShipToDesc"] == null)
			{
				GridColumnEx gridColumnEx = new GridColumnEx();
				gridColumnEx.FieldName = "ShipToDesc";
				gridColumnEx.Name = "gvLine_colShipToDesc";
				gvLine.Columns.Add(gridColumnEx);
			}
			if (gvLine.Columns["ShipToCode"] == null)
			{
				GridColumnEx gridColumnEx2 = new GridColumnEx();
				gridColumnEx2.FieldName = "ShipToCode";
				gridColumnEx2.Name = "gvLine_ShipToCode";
				RepositoryItemLookUpEditSimple repositoryItemLookUpEditSimple = new RepositoryItemLookUpEditSimple();
				repositoryItemLookUpEditSimple.Columns.Clear();
				repositoryItemLookUpEditSimple.Columns.Add(new LookUpColumnInfo("Address", ""));
				repositoryItemLookUpEditSimple.DisplayMember = "Address";
				repositoryItemLookUpEditSimple.ValueMember = "Address";
				repositoryItemLookUpEditSimple.EditValueChanged += Rep_gvLine_ShipToCode_EditValueChanged;
				gridColumnEx2.ColumnEdit = repositoryItemLookUpEditSimple;
				gvLine.Columns.Add(gridColumnEx2);
			}
			if (gvLine.Columns["TrnsCode"] == null)
			{
				GridColumnEx colShipType = new GridColumnEx
				{
					FieldName = "TrnsCode",
					Name = "gvLine_TrnsCode"
				};
				DataTable dataTable = await Task.Run(() => _httpClient.BusinessMaster.ShowShippingTypes());
				if (dataTable != null && !dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["TrnspCode"]) == Function.ParseShort(-1)))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["TrnspName"] = "";
					dataRow["TrnspCode"] = Function.ParseShort(-1);
					dataTable.Rows.InsertAt(dataRow, 0);
					dataTable.AcceptChanges();
				}
				RepositoryItemLookUpEditSimple repositoryItemLookUpEditSimple2 = new RepositoryItemLookUpEditSimple();
				repositoryItemLookUpEditSimple2.Columns.Clear();
				repositoryItemLookUpEditSimple2.Columns.Add(new LookUpColumnInfo("TrnspName", ""));
				repositoryItemLookUpEditSimple2.DisplayMember = "TrnspName";
				repositoryItemLookUpEditSimple2.ValueMember = "TrnspCode";
				repositoryItemLookUpEditSimple2.DataSource = dataTable;
				colShipType.ColumnEdit = repositoryItemLookUpEditSimple2;
				gvLine.Columns.Add(colShipType);
			}
			if (!IsSalesDocument)
			{
				gvLine.Columns["ShipToCode"].Visible = false;
				gvLine.Columns["ShipToDesc"].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void AddDistRule(GridViewEx gv, DataRow r, string fld = "", string caption = "")
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = "OcrCode" + code.ToString();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL typeCFL = Enums.TypeCFL.OrcCode;
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(code switch
					{
						"2" => Enums.TypeCFL.OrcCode2, 
						"3" => Enums.TypeCFL.OrcCode3, 
						"4" => Enums.TypeCFL.OrcCode4, 
						"5" => Enums.TypeCFL.OrcCode5, 
						_ => Enums.TypeCFL.OrcCode, 
					}, multiSelect: false, "");
					frm_ChooseFromList2.PostingDate = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("DocDate"));
					frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, row[0]);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumn gridColumn = gv.Columns[fieldName];
			if (gridColumn == null)
			{
				gridColumn = new GridColumnEx();
				gridColumn.FieldName = fieldName;
				gridColumn.Visible = false;
				gridColumn.Width = 100;
				gv.Columns.Add(gridColumn);
			}
			gridColumn.Caption = ((r["DimDesc"] == null || r["DimDesc"] == DBNull.Value) ? "" : r["DimDesc"].ToString());
			gv.GridControl.RepositoryItems.Add(repositoryItemButtonEdit);
			gridColumn.ColumnEdit = repositoryItemButtonEdit;
			if (!string.IsNullOrEmpty(fld))
			{
				return;
			}
			string fieldNameCog = string.Format("{0}{1}", "CogsOcrCo", string.IsNullOrEmpty(code) ? "d" : code);
			RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit2.Name = "rep_line_" + fieldNameCog;
			repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL typeCFL = Enums.TypeCFL.OrcCode;
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(code switch
					{
						"2" => Enums.TypeCFL.OrcCode2, 
						"3" => Enums.TypeCFL.OrcCode3, 
						"4" => Enums.TypeCFL.OrcCode4, 
						"5" => Enums.TypeCFL.OrcCode5, 
						_ => Enums.TypeCFL.OrcCode, 
					}, multiSelect: false, "");
					frm_ChooseFromList2.PostingDate = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("DocDate"));
					frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldNameCog, row[0]);
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, row[0]);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumn gridColumn2 = gv.Columns[fieldNameCog];
			if (gridColumn2 == null)
			{
				gridColumn2 = new GridColumnEx();
				gridColumn2.FieldName = fieldNameCog;
				gridColumn2.Caption = string.Format("COGS {0} {1}", caption, Function.ToString(r["DimDesc"]));
				gridColumn2.Visible = false;
				gridColumn2.Width = 100;
				gv.Columns.Add(gridColumn2);
			}
			gv.GridControl.RepositoryItems.Add(repositoryItemButtonEdit2);
			gridColumn2.ColumnEdit = repositoryItemButtonEdit2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void LoadInventoryUoM()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Yes",
					Code = "Y"
				},
				new Common.Table_Code
				{
					Name = "No",
					Code = "N"
				}
			};
			rLineUseBaseUnits.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadUomGroup()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rLineUomGroup;
			repositoryItemLookUpEdit.DataSource = await HttpClientHelperFactory.GetHttpClient().Commons.GetDataUomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataPriceList()
	{
		try
		{
			DataTable dataSource = await _httpClient.Commons.GetPriceList();
			rInfoPriceList.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task GetRecord(int ObjType, string tbl, int DocNum, string TypeRecord)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await _help.GetDataTableAsync("GetRecord", new
			{
				ObjType = ObjType,
				tbl = tbl,
				DocNum = DocNum,
				TypeRecord = TypeRecord,
				userCode = GlobalData.UserName
			});
			if (dataTable.IsNotNull())
			{
				if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
				{
					return;
				}
				DocEntrySearch = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
				txtNoOfTrans.Text = Function.ToString(dataTable.Rows[0]["DocNum"]);
				FormMode = Enums.FormViewModeType.View;
				btnSearch.PerformClick();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void EnableDisableControls()
	{
		try
		{
			if (GlobalData.AdditionalGeneralSettings.IsNotNull())
			{
				DataTable dataTable = GlobalData.AdditionalGeneralSettings.Tables[0];
				if (dataTable.IsNotNull() && dataTable.ExistsColumn("CheckPromotion"))
				{
					bool flag = Function.ToString(dataTable.Rows[0]["CheckPromotion"]) == "Y";
					btn_ClearPromotion.Visible = false;
					btn_CheckPromotion.Visible = flag && FormObjectType == ApzObjectTypes.oOrders;
				}
			}
			ucBatchAndSerial1.SetFormViewMode(FormMode);
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
				LoadSeries();
				btnCancel.Enabled = true;
				btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
				btn_Save.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnSearch.Enabled = true;
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				gvDoc.OptionsBehavior.Editable = false;
				gvLine.OptionsBehavior.Editable = false;
				break;
			case Enums.FormViewModeType.Update:
			{
				gvDoc.IgnoreFilter = true;
				btnCancel.Enabled = true;
				if (btn_Save.ImageOptions.Image == null)
				{
					btn_Save.ImageOptions.Image = _btnSaveImg;
				}
				btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
				btn_Save.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnSearch.Enabled = true;
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = gvDoc.GetFocusedRow() != null;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				if (!gvDoc.OptionsBehavior.Editable)
				{
					gvDoc.OptionsBehavior.Editable = true;
				}
				if (!gvLine.OptionsBehavior.Editable)
				{
					gvLine.OptionsBehavior.Editable = true;
				}
				string text2 = "Search";
				text2 = _saveAction switch
				{
					Enums.SaveAction.SaveAndNew => "New", 
					Enums.SaveAction.SaveAndView => "View", 
					Enums.SaveAction.SaveAndClose => "Close", 
					_ => "Search", 
				};
				btn_Save.Text = "Update && " + text2;
				break;
			}
			case Enums.FormViewModeType.Add:
			{
				btnCancel.Enabled = true;
				if (btn_Save.ImageOptions.Image == null)
				{
					btn_Save.ImageOptions.Image = _btnSaveImg;
				}
				btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
				btn_Save.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnSearch.Enabled = true;
				drlCopyFrom.Enabled = true;
				drlCopyTo.Enabled = false;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				if (!gvDoc.OptionsBehavior.Editable)
				{
					gvDoc.OptionsBehavior.Editable = true;
				}
				if (!gvLine.OptionsBehavior.Editable)
				{
					gvLine.OptionsBehavior.Editable = true;
				}
				string text = "Search";
				text = _saveAction switch
				{
					Enums.SaveAction.SaveAndNew => "New", 
					Enums.SaveAction.SaveAndView => "View", 
					Enums.SaveAction.SaveAndClose => "Close", 
					_ => "Search", 
				};
				btn_Save.Text = "Add && " + text;
				break;
			}
			case Enums.FormViewModeType.View:
				gvDoc.IgnoreFilter = false;
				LoadSeries();
				btnCancel.Enabled = true;
				btn_Save.ImageOptions.Image = null;
				btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
				btn_Save.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnSearch.Enabled = true;
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = gvDoc.GetFocusedRow() != null;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				if (!gvDoc.OptionsBehavior.Editable)
				{
					gvDoc.OptionsBehavior.Editable = true;
				}
				if (!gvLine.OptionsBehavior.Editable)
				{
					gvLine.OptionsBehavior.Editable = true;
				}
				gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			case Enums.FormViewModeType.Busy:
				btnCancel.Enabled = false;
				btn_Save.Enabled = false;
				ucAttachment1.EnableButtons(isEnabled: false);
				btnSearch.Enabled = false;
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = false;
				ctxDoc.Enabled = false;
				ctxLine.Enabled = false;
				if (gvDoc.OptionsBehavior.Editable)
				{
					gvDoc.OptionsBehavior.Editable = false;
				}
				if (gvLine.OptionsBehavior.Editable)
				{
					gvLine.OptionsBehavior.Editable = false;
				}
				gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.False;
				break;
			default:
				btnCancel.Enabled = true;
				btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
				btn_Save.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnSearch.Enabled = true;
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				if (!gvDoc.OptionsBehavior.Editable)
				{
					gvDoc.OptionsBehavior.Editable = true;
				}
				if (!gvLine.OptionsBehavior.Editable)
				{
					gvLine.OptionsBehavior.Editable = true;
				}
				gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			}
			LoadDropDownSave(FormMode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTabAccounting(int entry, bool isCopyFrom = false, ApzObjectTypes objT = ApzObjectTypes.oDrafts)
	{
		gv_Accounting.ShowLoadingPanel();
		try
		{
			ApzObjectTypes objType = FormObjectType;
			if (isCopyFrom)
			{
				objType = objT;
			}
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await hp.GetAsync("DocumentLoadTabAccounting", new { entry, objType });
			if (!dataTable.IsNotNull())
			{
				NewRowAccounting();
				dataTable = _dtTabAccounting.Copy();
			}
			grd_Accounting.DataSource = dataTable;
			grd_Accounting.RefreshDataSource();
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
			if (_formMode == Enums.FormViewModeType.Add || (_formMode != Enums.FormViewModeType.Add && (text.ToUpper() == "O" || text.ToUpper() == "OPEN" || string.IsNullOrEmpty(text))))
			{
				for (int i = 0; i < gv_Accounting.Columns.Count; i++)
				{
					gv_Accounting.Columns[i].OptionsColumn.ReadOnly = false;
					gv_Accounting.Columns[i].OptionsColumn.AllowEdit = true;
				}
				return;
			}
			gv_Accounting.Columns["GroupNum"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["GroupNum"].OptionsColumn.AllowEdit = false;
			gv_Accounting.Columns["PeyMethod"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["PeyMethod"].OptionsColumn.AllowEdit = false;
			gv_Accounting.Columns["PayDuMonth"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["PayDuMonth"].OptionsColumn.AllowEdit = false;
			gv_Accounting.Columns["ExtraMonth"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["ExtraMonth"].OptionsColumn.AllowEdit = false;
			gv_Accounting.Columns["ExtraDays"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["ExtraDays"].OptionsColumn.AllowEdit = false;
			gv_Accounting.Columns["CdcOffset"].OptionsColumn.ReadOnly = true;
			gv_Accounting.Columns["CdcOffset"].OptionsColumn.AllowEdit = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Accounting.HideLoadingPanel();
		}
	}

	private async Task LoadTabAccountingRepLookUpEdit()
	{
		try
		{
			using (HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Commons"))
			{
				string condition = "";
				DataSet dataSet = await hp.GetAsync("DocumentGetComBoTabAccounting", new { condition });
				if (dataSet != null)
				{
					if (dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Columns.Count > 0)
					{
						luk_Accounting_GroupNum.DataSource = dataSet.Tables[0];
					}
					if (dataSet.Tables.Count > 1 && dataSet.Tables[1] != null && dataSet.Tables[1].Columns.Count > 0)
					{
						DataRow dataRow = dataSet.Tables[1].NewRow();
						dataRow["Code"] = "";
						dataRow["Name"] = "";
						dataSet.Tables[1].Rows.InsertAt(dataRow, 0);
						dataSet.Tables[1].AcceptChanges();
						luk_Accounting_PaymentMethod.DataSource = dataSet.Tables[1];
					}
					if (dataSet.Tables.Count > 2 && dataSet.Tables[2] != null && dataSet.Tables[2].Columns.Count > 0)
					{
						DataRow dataRow2 = dataSet.Tables[2].NewRow();
						dataRow2["Code"] = "";
						dataRow2["Name"] = "";
						dataSet.Tables[2].Rows.InsertAt(dataRow2, 0);
						dataSet.Tables[2].AcceptChanges();
						luk_Accounting_Indicator.DataSource = dataSet.Tables[2];
					}
				}
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = "N";
			dataRow3["Name"] = "";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["Code"] = "H";
			dataRow4["Name"] = "Half Month";
			dataTable.Rows.Add(dataRow4);
			DataRow dataRow5 = dataTable.NewRow();
			dataRow5["Code"] = "Y";
			dataRow5["Name"] = "Month Start";
			dataTable.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable.NewRow();
			dataRow6["Code"] = "E";
			dataRow6["Name"] = "Month End";
			dataTable.Rows.Add(dataRow6);
			dataTable.AcceptChanges();
			luk_Accounting_PayDuMonth.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewActivity()
	{
		try
		{
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			int num = (int)(flag ? ApzObjectTypes.oDrafts : FormObjectType);
			int num2 = (flag ? Function.ParseInt(Fromentry) : Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			int num3 = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			string value = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromCal = true
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["GroupUser"] = GlobalData.UserID;
			if (Function.ToString(dataRow["Recontact"]) == "")
			{
				dataRow["Recontact"] = DateTime.Today;
				string text = DateTime.Now.Minute.ToString();
				dataRow["BeginTime"] = DateTime.Now.Hour + ((text.Length == 2) ? text : ("0" + text));
			}
			dataRow["Closed"] = "N";
			dataRow["DocType"] = num;
			if (num2 != 0)
			{
				dataRow["DocEntry"] = num2;
			}
			if (num3 != 0)
			{
				dataRow["DocNum"] = num3;
			}
			dataRow["CardCode"] = value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TaskManagement2.DataRowFromCal = dataRow;
			frm_TaskManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TargetDocument()
	{
		try
		{
			int num = Function.ParseInt(gvLine.GetRowCellValue(gvLine.FocusedRowHandle, "TrgetEntry"));
			ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(gvLine.GetRowCellValue(gvLine.FocusedRowHandle, "TargetType"));
			if (num > 0)
			{
				this.DrillDown(apzObjectTypes, apzObjectTypes, num.ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BaseDocument()
	{
		try
		{
			int num = Function.ParseInt(gvLine.GetRowCellValue(gvLine.FocusedRowHandle, "BaseEntry"));
			ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(gvLine.GetRowCellValue(gvLine.FocusedRowHandle, "BaseType"));
			if (num > 0)
			{
				this.DrillDown(apzObjectTypes, apzObjectTypes, num.ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ClearGridLine()
	{
		if (grdLine.DataSource is DocumentLineTable documentLineTable && documentLineTable.Rows.Count > 0)
		{
			documentLineTable.Rows.Clear();
			documentLineTable.AcceptChanges();
		}
		grdLogistic.DataSource = null;
		grdLogistic.RefreshDataSource();
		grd_Accounting.DataSource = null;
		grd_Accounting.RefreshDataSource();
		ucAttachment1.grd_File.DataSource = null;
	}

	private async Task LoadChildData()
	{
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			ClearGridLine();
			string docCurrency = ((gvDoc.GetFocusedRowCellValue("DocCur") == null || gvDoc.GetFocusedRowCellValue("DocCur") == DBNull.Value) ? "" : gvDoc.GetFocusedRowCellValue("DocCur").ToString());
			Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			ExtenseionMethods.ShowHideSumColumn(gvLine, docCurrency, gv_Freight);
			int currentFileEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry"));
			_currentFileEntry = currentFileEntry;
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
			_showForLandedCost = num > 0 && (FormObjectType == ApzObjectTypes.oPurchaseDeliveryNotes || FormObjectType == ApzObjectTypes.oPurchaseInvoices);
			SetTextDoc();
			_isChangingRow = true;
			await GetChildDataByID(num);
			_isChangingRow = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetTextDoc()
	{
		bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
		Text = (flag ? (_originText + " - Draft") : _originText);
		SetWddStatusText();
	}

	private void VirFormDocument_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (!Focused)
				{
					XtraFormEx.LockWindowUpdate(Handle);
					Focus();
					XtraFormEx.LockWindowUpdate(IntPtr.Zero);
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grdLine_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			DrawMergedCells(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void DrawMergedCells(PaintEventArgs e)
	{
		List<DataRow> list = (from t in grdLine.DataSourceTable.AsEnumerable()
			where Function.ToString(t["LineType"]) == "T"
			select t).ToList();
		if (list == null || list.Count <= 0)
		{
			return;
		}
		if (LineLastColumn == null)
		{
			LineLastColumn = gvLine.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvLine.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		foreach (DataRow item in list)
		{
			int num = gvLine.FindRowHandleByDataRow(item);
			if (num >= 0)
			{
				GridViewExMergeCell cell = new GridViewExMergeCell(num, gcItemCode, LineLastColumn);
				painter.DrawMergedCell(cell, e);
			}
		}
	}

	private void gvLine_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
	}

	private void toolStripMenuItemOpeningClosing_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				frm_OpeningClosingRemarks frm_OpeningClosingRemarks2 = new frm_OpeningClosingRemarks();
				frm_OpeningClosingRemarks2.OpeningRemarks = Function.ToString(gvDoc.GetFocusedRowCellValue("Header"));
				frm_OpeningClosingRemarks2.ClosingRemarks = Function.ToString(gvDoc.GetFocusedRowCellValue("Footer"));
				if (frm_OpeningClosingRemarks2.ShowDialog(this) == DialogResult.OK)
				{
					gvDoc.SetFocusedRowCellValue("Header", frm_OpeningClosingRemarks2.OpeningRemarks);
					gvDoc.SetFocusedRowCellValue("Footer", frm_OpeningClosingRemarks2.ClosingRemarks);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ctm_RemoveDoc_Click(object sender, EventArgs e)
	{
		try
		{
			await CancelCloseDelete("Delete", Apzon.Commons.Properties.Resources.Msg_DeleteSuccess);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_NewEvent_Click(object sender, EventArgs e)
	{
		if (gvDoc.GetFocusedDataRow() != null)
		{
			NewEvent();
		}
	}

	private async Task gv_Logistic_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.Column.FieldName.Equals(gcLogistics_BPChCode.FieldName))
			{
				string cardCode = Function.ToString(e.Value);
				DataTable dtBusiness = await HttpClient.BusinessMaster.GetADoc(cardCode);
				if (dtBusiness.IsNotNull())
				{
					DataTable dataTable = await HttpClient.Commons.GetContactPerson(cardCode);
					if (dataTable.IsNotNull())
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Name"] = "";
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
					}
					rLogisticBpChannelCntc.DataSource = dataTable;
					DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).Equals(Function.ToString(dtBusiness.Rows[0]["CntctPrsn"])));
					if (dataRow2 != null)
					{
						gv_Logistic.SetRowCellValue(e.RowHandle, "BPChCntc", dataRow2["CntctCode"]);
					}
				}
			}
			grdLogistic.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void alternativeItemsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string itemCode = Function.ToString(gvLine.GetFocusedRowCellValue(gcItemCode));
			if (string.IsNullOrEmpty(itemCode))
			{
				return;
			}
			DataSet dataSet = await HttpClient.AlternativeItemsClient.GetDataCfl(itemCode, GlobalData.UserName, Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcDoc_Branch)), Function.ToString(Function.ToString(gvDoc.GetFocusedRowCellValue(gcDoc_CardCode))), Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcPostingDate)));
			if (dataSet == null || dataSet.Tables.Count != 2 || !dataSet.Tables[1].IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.AlternativeItemHaveNotBeenDefined4Item, "", itemCode);
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.AlternativeItems, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataSource = dataSet;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvLine.SetFocusedRowCellValue(gcItemCode, frm_CFL2.ChooseItem["ItemCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool gvLine_CheckValidChanged(int rowHandle, string columnEdit)
	{
		try
		{
			return !columnEdit.Equals("ItemCode") || Function.ToString(gvLine.GetRowCellValue(rowHandle, "LineType")).Equals("R");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return true;
	}

	private void btn_CheckPromotion_Click(object sender, EventArgs e)
	{
		try
		{
			frm_CheckPromotion frm_CheckPromotion2 = new frm_CheckPromotion();
			frm_CheckPromotion2.Input = new DataSet();
			DataTable dataTable = grdLine.DataSourceTable.Copy();
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
				select t).ToArray();
			DataTable dataTable2 = dataTable.Clone();
			if (array.Length != 0)
			{
				dataTable2 = array.CopyToDataTable();
			}
			DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
			DataTable dataTable3 = grdDoc.DataSourceTable.Clone();
			if (focusedDataRow != null)
			{
				dataTable3 = new DataRow[1] { focusedDataRow }.CopyToDataTable();
			}
			frm_CheckPromotion2.Input.Tables.AddRange(new DataTable[2]
			{
				dataTable3.Copy(),
				dataTable2.Copy()
			});
			if (FormMode != Enums.FormViewModeType.Add)
			{
				frm_CheckPromotion2.DocEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
			}
			frm_CheckPromotion2.ChoosePromotion = (Action<List<DataRow>, DataTable>)Delegate.Combine(frm_CheckPromotion2.ChoosePromotion, new Action<List<DataRow>, DataTable>(ChoosePromotion));
			GridColumn gridColumn = gvLine.Columns["U_PromotionType"];
			if (gridColumn != null)
			{
				frm_CheckPromotion2.PromotiontypeEditor = gridColumn.ColumnEdit;
			}
			frm_CheckPromotion2.MdiParent = MdiParent;
			frm_CheckPromotion2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChoosePromotion(List<DataRow> rows, DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				string value = Function.ToString(dt.Rows[0]["GUID"]);
				gvDoc.SetFocusedRowCellValue("U_GUID", value);
			}
			foreach (DataRow item in rows.OrderBy((DataRow t) => Function.ParseInt(t["TypeID"])))
			{
				int num = Function.ParseInt(item["TypeID"]);
				switch (num)
				{
				case 1:
				case 2:
				case 7:
				{
					string itemCode = Function.ToString(item["ItemCode"]);
					for (int num9 = 0; num9 < gvLine.RowCount; num9++)
					{
						int lineNum = Function.ParseInt(gvLine.GetRowCellValue(num9, "LineNum"));
						if (dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == lineNum && Function.ToString(t["ItemCode"]) == itemCode && new int[2] { 3, 4 }.Contains(Function.ParseInt(t["TypeID"]))) == null && Function.ToString(gvLine.GetRowCellValue(num9, "ItemCode")) == itemCode)
						{
							switch (num)
							{
							case 1:
							{
								double num14 = Function.ParseDouble(gvLine.GetRowCellValue(num9, "DiscPrcnt"));
								double num15 = Function.ParseDouble(item["ActualVal"]);
								gvLine.SetRowCellValue(num9, "DiscPrcnt", num14 + num15);
								break;
							}
							case 2:
							{
								double num12 = Function.ParseDouble(gvLine.GetRowCellValue(num9, "LineTotal"));
								double num13 = Function.ParseDouble(item["ActualVal"]);
								gvLine.SetRowCellValue(num9, "LineTotal", num12 - num13);
								break;
							}
							case 7:
							{
								double num10 = Function.ParseDouble(gvLine.GetRowCellValue(num9, "LineTotal"));
								double num11 = Function.ParseDouble(item["ActualVal"]);
								gvLine.SetRowCellValue(num9, "LineTotal", num10 - num11);
								break;
							}
							}
							gvLine.SetRowCellValue(num9, "U_PromotionType", num);
							gvLine.SetRowCellValue(num9, "U_PromotionCode", Function.ToString(item["ProgramCode"]));
							gvLine.SetRowCellValue(num9, "U_PromotionGroup", Function.ToString(item["GroupID"]));
							gvLine.SetRowCellValue(num9, "U_PromotionPack", Function.ToString(item["PackNo"]));
							gvLine.SetRowCellValue(num9, "U_PromotionLine", Function.ParseInt(item["ProfitLineID"]));
							gvLine.SetRowCellValue(num9, "U_PromotionGroupProfit", Function.ParseInt(item["ProfitGroupID"]));
						}
					}
					break;
				}
				case 3:
				case 4:
				{
					string value2 = Function.ToString(item["ItemCode"]);
					double num8 = Function.ParseDouble(item["ActualVal"]);
					AddNewBlankLine();
					int rowHandle = gvLine.RowCount - 1;
					gvLine.SetRowCellValue(rowHandle, "ItemCode", value2);
					gvLine.SetRowCellValue(rowHandle, "Quantity", num8);
					gvLine.SetRowCellValue(rowHandle, "DiscPrcnt", 100);
					gvLine.SetRowCellValue(rowHandle, "U_PromotionType", num);
					gvLine.SetRowCellValue(rowHandle, "U_PromotionCode", Function.ToString(item["ProgramCode"]));
					gvLine.SetRowCellValue(rowHandle, "U_PromotionGroup", Function.ToString(item["GroupID"]));
					gvLine.SetRowCellValue(rowHandle, "U_PromotionPack", Function.ToString(item["PackNo"]));
					gvLine.SetRowCellValue(rowHandle, "U_PromotionLine", Function.ParseInt(item["ProfitLineID"]));
					gvLine.SetRowCellValue(rowHandle, "U_PromotionGroupProfit", Function.ParseInt(item["ProfitGroupID"]));
					break;
				}
				case 5:
				{
					double num6 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscPrcnt"));
					double num7 = Function.ParseDouble(item["ActualVal"]);
					gvDoc.SetFocusedRowCellValue("DiscPrcnt", num6 + num7);
					break;
				}
				case 6:
				{
					double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscSum"));
					double num3 = Function.ParseDouble(item["ActualVal"]);
					double num4 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TotalBeforeDis"));
					double num5 = (num2 + num3) * 100.0 / num4;
					gvDoc.SetFocusedRowCellValue("DiscPrcnt", num5);
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void relatedActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetFocusedDataRow() != null)
			{
				frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(FormObjectType, Function.ToString(gvDoc.GetFocusedRowCellValue("DocEntry")));
				frm_RelatedActivites2.DocNum = Function.ToString(gvDoc.GetFocusedRowCellValue("DocNum"));
				frm_RelatedActivites2.MdiParent = MdiParent;
				frm_RelatedActivites2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ClearPromotion_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("U_GUID"));
			if (string.IsNullOrEmpty(text) || XtraMessageBox.Show("Do you want to clear the promotion?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataTable = await _httpClient.Promotion.GetExistDocPromotion(text);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			gvDoc.SetFocusedRowCellValue("U_GUID", string.Empty);
			foreach (DataRow row in dataTable.Rows)
			{
				int num = Function.ParseInt(row["TypeID"]);
				switch (num)
				{
				case 1:
				case 2:
				case 7:
				{
					string itemCode = Function.ToString(row["ItemCode"]);
					for (int i = 0; i < gvLine.RowCount; i++)
					{
						int lineNum = Function.ParseInt(gvLine.GetRowCellValue(i, "LineNum"));
						if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == lineNum && Function.ToString(t["ItemCode"]) == itemCode && new int[2] { 3, 4 }.Contains(Function.ParseInt(t["TypeID"]))) == null && Function.ToString(gvLine.GetRowCellValue(i, "ItemCode")) == itemCode)
						{
							switch (num)
							{
							case 1:
							{
								double num12 = Function.ParseDouble(gvLine.GetRowCellValue(i, "DiscPrcnt"));
								double num13 = Function.ParseDouble(row["ActualVal"]);
								gvLine.SetRowCellValue(i, "DiscPrcnt", num12 + num13);
								break;
							}
							case 2:
							{
								double num10 = Function.ParseDouble(gvLine.GetRowCellValue(i, "LineTotal"));
								double num11 = Function.ParseDouble(row["ActualVal"]);
								gvLine.SetRowCellValue(i, "LineTotal", num10 + num11);
								break;
							}
							case 7:
							{
								double num8 = Function.ParseDouble(gvLine.GetRowCellValue(i, "LineTotal"));
								double num9 = Function.ParseDouble(row["ActualVal"]);
								gvLine.SetRowCellValue(i, "LineTotal", num8 + num9);
								break;
							}
							}
							gvLine.SetRowCellValue(i, "U_PromotionType", "");
							gvLine.SetRowCellValue(i, "U_PromotionCode", "");
							gvLine.SetRowCellValue(i, "U_PromotionGroup", null);
							gvLine.SetRowCellValue(i, "U_PromotionPack", null);
							gvLine.SetRowCellValue(i, "U_PromotionLine", null);
							gvLine.SetRowCellValue(i, "U_PromotionGroupProfit", null);
						}
					}
					break;
				}
				case 3:
				case 4:
					gvLine.DeleteRow(Function.ParseInt(row["LineNum"]));
					grdLine.DataSourceTable.AcceptChanges();
					break;
				case 5:
				{
					double num6 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscPrcnt"));
					double num7 = Function.ParseDouble(row["ActualVal"]);
					gvDoc.SetFocusedRowCellValue("DiscPrcnt", num6 - num7);
					break;
				}
				case 6:
				{
					double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscSum"));
					double num3 = Function.ParseDouble(row["ActualVal"]);
					double num4 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TotalBeforeDis"));
					double num5 = (num2 - num3) * 100.0 / num4;
					gvDoc.SetFocusedRowCellValue("DiscPrcnt", num5);
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rep_PmtToDraw_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		_ = 2;
		try
		{
			rep_PmtToDraw.Buttons[0].Enabled = false;
			if (FormMode != Enums.FormViewModeType.Add)
			{
				int docEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
				DataTable dataTable = await _httpClient.Documents.GetDownPaymentDrawed(FormObjectType, docEntry);
				if (dataTable.IsNotNull())
				{
					new frm_DownPaymentsToDraw(dataTable, allowEdit: false).ShowDialog(this);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "No down payments have been posted yet for this document");
				}
				return;
			}
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			string docCur = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
			DataTable dt = await _httpClient.Documents.GetDownPaymentToDraw(FormObjectType, cardCode, docCur);
			if (dt.Columns.Count != 0)
			{
				dt.Columns.Add("Selected", typeof(string));
			}
			if (Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft")) && _downPaymentData == null)
			{
				_downPaymentData = await _httpClient.Documents.GetDownPaymentDrawed(ApzObjectTypes.oDrafts, Function.ParseInt(gvDoc.GetFocusedRowCellValue("draftKey")));
				_downPaymentData.Columns.Add("Selected", typeof(string));
				List<DataRow> list = new List<DataRow>();
				foreach (DataRow r in _downPaymentData.Rows)
				{
					DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Id"]) == Function.ToString(r["Id"]));
					if (dataRow != null)
					{
						dataRow["NetAmount"] = r["NetAmount"];
						dataRow["TaxAmount"] = r["TaxAmount"];
						dataRow["GrossAmount"] = r["GrossAmount"];
						if (Function.ToString(r["FatherId"]) == "")
						{
							dataRow["Selected"] = "Y";
						}
						list.Add(dataRow);
					}
				}
				_downPaymentData.Rows.Clear();
				foreach (DataRow item in list)
				{
					_downPaymentData.ImportRow(item);
				}
				_downPaymentData.AcceptChanges();
			}
			if (dt.IsNotNull())
			{
				if (_downPaymentData.IsNotNull())
				{
					EnumerableRowCollection<DataRow> enumerableRowCollection = from dataRow4 in _downPaymentData.AsEnumerable()
						where Function.ToString(dataRow4["Selected"]) == "Y"
						select dataRow4;
					List<DataRow> list2 = new List<DataRow>();
					List<DataRow> list3 = new List<DataRow>();
					foreach (DataRow item2 in enumerableRowCollection)
					{
						string id = Function.ToString(item2["Id"]);
						DataRow dataRow2 = dt.AsEnumerable().FirstOrDefault((DataRow dataRow4) => Function.ToString(dataRow4["Id"]) == id);
						if (dataRow2 == null)
						{
							continue;
						}
						EnumerableRowCollection<DataRow> enumerableRowCollection2 = from dataRow4 in _downPaymentData.AsEnumerable()
							where Function.ToString(dataRow4["FatherId"]) == id
							select dataRow4;
						EnumerableRowCollection<DataRow> enumerableRowCollection3 = from dataRow4 in dt.AsEnumerable()
							where Function.ToString(dataRow4["FatherId"]) == id
							select dataRow4;
						if (enumerableRowCollection2.Count() != enumerableRowCollection3.Count())
						{
							continue;
						}
						bool flag = true;
						foreach (DataRow child in enumerableRowCollection2)
						{
							DataRow dataRow3 = dt.AsEnumerable().FirstOrDefault((DataRow dataRow4) => Function.ToString(dataRow4["Id"]) == Function.ToString(child["Id"]));
							if (dataRow3 == null || Function.ParseDecimal(child["OpenNetAmount"]) != Function.ParseDecimal(dataRow3["OpenNetAmount"]))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							list2.Add(item2);
							list2.AddRange(enumerableRowCollection2);
							list3.Add(dataRow2);
							list3.AddRange(enumerableRowCollection3);
						}
					}
					foreach (DataRow item3 in list3)
					{
						dt.Rows.Remove(item3);
					}
					foreach (DataRow item4 in list2)
					{
						dt.ImportRow(item4);
					}
					dt.AcceptChanges();
				}
				frm_DownPaymentsToDraw frm_DownPaymentsToDraw2 = new frm_DownPaymentsToDraw(dt);
				frm_DownPaymentsToDraw2.ShowDialog(this);
				if (frm_DownPaymentsToDraw2.DialogResult != DialogResult.OK)
				{
					return;
				}
				double num = 0.0;
				if (_downPaymentData.IsNotNull())
				{
					(from dataRow4 in _downPaymentData.AsEnumerable()
						where Function.ToString(dataRow4["Selected"]) == "Y"
						select dataRow4).Sum((DataRow dataRow4) => Function.ParseDouble(dataRow4["NetAmount"]));
					num = (from dataRow4 in _downPaymentData.AsEnumerable()
						where Function.ToString(dataRow4["Selected"]) == "Y"
						select dataRow4).Sum((DataRow dataRow4) => Function.ParseDouble(dataRow4["TaxAmount"]));
				}
				_downPaymentData = frm_DownPaymentsToDraw2.DownPaymentData;
				double num2 = (from dataRow4 in _downPaymentData.AsEnumerable()
					where Function.ToString(dataRow4["Selected"]) == "Y"
					select dataRow4).Sum((DataRow dataRow4) => Function.ParseDouble(dataRow4["NetAmount"]));
				double num3 = (from dataRow4 in _downPaymentData.AsEnumerable()
					where Function.ToString(dataRow4["Selected"]) == "Y"
					select dataRow4).Sum((DataRow dataRow4) => Function.ParseDouble(dataRow4["TaxAmount"]));
				gvDoc.SetFocusedRowCellValue("DPMTotal", num2);
				gvDoc.SetFocusedRowCellValue((docCur == GlobalData.MainCurrency) ? "DpmAmnt" : "DpmAmntFC", num2);
				string fieldName = ((docCur == GlobalData.MainCurrency) ? "DpmVat" : "DpmVatFc");
				gvDoc.SetFocusedRowCellValue(fieldName, num3);
				double num4 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TaxTotal")) + num - num3;
				gvDoc.SetFocusedRowCellValue("TaxTotal", num4);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "You cannot choose the \"Total Down Payment\" \"Browse\" button; first create a down payment");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			rep_PmtToDraw.Buttons[0].Enabled = true;
		}
	}

	private async void rep_ReferencedDocument_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ReferencedDocument frm_ReferencedDocument2 = new frm_ReferencedDocument();
			frm_ReferencedDocument2.DocEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry"));
			frm_ReferencedDocument2.DocumentType = FormObjectType;
			if (_currentRefDoc == null || _currentRefDoc.Columns.Count == 0)
			{
				_currentRefDoc = _httpClient.Documents.GetReferencedDocument(0);
			}
			frm_ReferencedDocument2.Source = _currentRefDoc;
			frm_ReferencedDocument2.FromDate = Function.ParseDateTime(txtFromDate.EditValue);
			frm_ReferencedDocument2.ToDate = Function.ParseDateTime(txtToDate.EditValue);
			if (frm_ReferencedDocument2.ShowDialog(this) == DialogResult.OK)
			{
				_currentRefDoc = frm_ReferencedDocument2.Source;
				int countRef = (from t in _currentRefDoc.AsEnumerable()
					where Function.ParseInt(t["RefObjType"]) != 0 && Function.ParseInt(t["RefDocEntr"]) != 0
					select t).Count();
				if (grd_Accounting.DataSource == null)
				{
					GridControlEx gridControlEx = grd_Accounting;
					gridControlEx.DataSource = (await WinFuntions.DocumentLoadTabAccountingColumns()).Clone();
				}
				gv_Accounting.SetRowCellValue(0, "REFDOC", countRef);
				gv_Accounting.CloseEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewEvent()
	{
		try
		{
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			int num = (int)(flag ? ApzObjectTypes.oDrafts : FormObjectType);
			int num2 = (flag ? Function.ParseInt(Fromentry) : Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			int num3 = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			string value = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			frm_EventManagement frm_EventManagement2 = new frm_EventManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromRelated = true
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["GroupUser"] = GlobalData.UserID;
			dataRow["ClgCode"] = 0;
			dataRow["Recontact"] = DateTime.Today;
			string text = DateTime.Now.ToString("HH:mm");
			dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
			dataRow["EndDate"] = DateTime.Today;
			string text2 = DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
			dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
			if (Function.ParseInt(dataRow["BeginTime"]) == 0 && Function.ParseInt(dataRow["ENDTime"]) == 0)
			{
				dataRow["AllDay"] = "Y";
			}
			else
			{
				dataRow["AllDay"] = "N";
			}
			dataRow["AllDayEvent"] = false;
			dataRow["Closed"] = "N";
			dataRow["DocType"] = num;
			if (num2 != 0)
			{
				dataRow["DocEntry"] = num2;
			}
			if (num3 != 0)
			{
				dataRow["DocNum"] = num3;
			}
			dataRow["CardCode"] = value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_EventManagement2.DataRowFromCal = dataRow;
			frm_EventManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CtlAccount_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.AcctBPDownPayment, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue(gvDoc.FocusedColumn, frm_CFL2.ChooseItem["AcctCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void ctm_NewNote_Click(object sender, EventArgs e)
	{
		if (gvDoc.GetFocusedDataRow() != null)
		{
			NewNote();
		}
	}

	private void NewNote()
	{
		try
		{
			bool flag = Function.ParseBool(gvDoc.GetFocusedRowCellValue("IsDraft"));
			int num = (int)(flag ? ApzObjectTypes.oDrafts : FormObjectType);
			int num2 = (flag ? Function.ParseInt(Fromentry) : Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")));
			int num3 = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			string value = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromRelated = true
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["GroupUser"] = GlobalData.UserID;
			dataRow["ClgCode"] = 0;
			dataRow["Recontact"] = DateTime.Today;
			DateTime.Now.ToString("HH:mm");
			dataRow["EndDate"] = DateTime.Today;
			DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
			dataRow["Closed"] = "N";
			dataRow["AllDayEvent"] = false;
			dataRow["DocType"] = num;
			if (num2 != 0)
			{
				dataRow["DocEntry"] = num2;
			}
			if (num3 != 0)
			{
				dataRow["DocNum"] = num3;
			}
			dataRow["CardCode"] = value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_NoteManagement2.DataRowFromCal = dataRow;
			frm_NoteManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_ForLandedCost_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			int focusedDataSourceRowIndex = gvDoc.GetFocusedDataSourceRowIndex();
			if (focusedDataSourceRowIndex < 0)
			{
				return;
			}
			if (Function.ToString(dataSourceTable.Rows[focusedDataSourceRowIndex]["OpenForLaC"]) == "N")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmOpenForLandedCost, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
				{
					return;
				}
				dataSourceTable.Rows[focusedDataSourceRowIndex]["OpenForLaC"] = "Y";
			}
			else
			{
				dataSourceTable.Rows[focusedDataSourceRowIndex]["OpenForLaC"] = "N";
			}
			dataSourceTable.AcceptChanges();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			_showForLandedCost = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckOpenForLandedCost()
	{
		try
		{
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("OpenForLaC")) == "N")
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	protected virtual void disableCopyTobyRejected()
	{
		string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
		string text2 = Function.ToString(gvDoc.GetFocusedRowCellValue("WddStatus"));
		drlCopyTo.SettingReadonly = false;
		drlCopyTo.Enabled = (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && text.Equals("O") && !text2.Equals("N");
		drlCopyTo.SettingReadonly = !drlCopyTo.Enabled;
	}

	protected override void NoAuthSetControlSearch()
	{
		ToolStripMenuItem toolStripMenuItem = ctm_SaveAsDraft;
		ToolStripMenuItem toolStripMenuItem2 = ctm_BatchSerialNumbers;
		ToolStripMenuItem toolStripMenuItem3 = ctm_targetDocument;
		ToolStripMenuItem toolStripMenuItem4 = ctm_baseDocumentLine;
		ToolStripMenuItem toolStripMenuItem5 = ctm_baseDocument;
		ToolStripMenuItem toolStripMenuItem6 = ctm_targetDocumentLine;
		bool flag = (viewDiagramToolStripMenuItem.Visible = true);
		bool flag3 = (toolStripMenuItem6.Visible = flag);
		bool flag5 = (toolStripMenuItem5.Visible = flag3);
		bool flag7 = (toolStripMenuItem4.Visible = flag5);
		bool flag9 = (toolStripMenuItem3.Visible = flag7);
		bool visible = (toolStripMenuItem2.Visible = flag9);
		toolStripMenuItem.Visible = visible;
		drlCopyTo.SettingReadonly = false;
		drlCopyTo.Enabled = true;
	}

	public void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.VirFormDocument));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
		Apzon.Controls.Editor.RepositoryItemCalcEditEx repositoryItemCalcEditEx = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		this.split_Main = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.ctxDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_AddRow = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewTask = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewEvent = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewNote = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_ForLandedCost = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.ctm_RemoveDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Cancel = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Close = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItemOpeningClosing = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.ctm_SaveAsDraft = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_targetDocument = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_baseDocument = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_RelatedActivities = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDoc_Series = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Series = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rtxtCardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDoc_CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_CntctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_NumAtCard = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_ReqType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ReqType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDoc_Requester = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Requester = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDoc_ReqName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_Department = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Department = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDoc_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPostingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.col_DueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.col_DocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTotalBeforeDis = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDpmPrcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx21 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcDpmTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx22 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcDpmVat = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDpmVatFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDis = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx26 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumn49 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTotalTax = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDocTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcVatAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rILCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcDocTotalFC = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcDocAtcEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Doc_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Doc_BranchVATRegNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStageSalesEmp = new Apzon.Controls.Grid.GridColumnEx();
		this.rILSalesEmp = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gcApprovalStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ApprovalStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Doc_CurrencyType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gcDoc_OwnerCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_OwnerCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcInfoPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rInfoPriceList = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCtlAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CtlAccount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPriceMode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PriceMode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rlupContactPersonCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemDateEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemDateEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_PmtToDraw = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.split_LineBatchAndSerial = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTab_Line = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tbDocumentLine = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.ctxLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_addRowLine = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_delRowLine = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_LineClose = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_LineDuplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_targetDocumentLine = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_baseDocumentLine = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_BatchSerialNumbers = new System.Windows.Forms.ToolStripMenuItem();
		this.alternativeItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineType = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rtxtItemcode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBPCatalog = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpCatalog = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcUnitPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceEdit = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcDisPer = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcVatGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rLookUpVatGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineTaxOnly = new Apzon.Controls.Grid.GridColumnEx();
		this.rEnSetCost = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcTotalLine = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWhseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rtxtWhseCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLinePQTReqdate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcLine_DelDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemarkLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpenQty = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcCureencyLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatPrcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx9 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineVat = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx10 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcLineTotalFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineVatSumFrgn = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx12 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx23 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcBOMTreeType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTrgetEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BaseType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBaseRef = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTargetType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineTreeType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDeliveryQty = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx11 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcLineCtlAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_CtlAccount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLinePackQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rLinePackQty = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_FreightCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LstPrice = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_FreightStock = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn42 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn43 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn46 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn48 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUomGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUomCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineUoMName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Line_CogsOcrCo2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineInventoryUom = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUseBaseUnits = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineItemPerUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UnitsOfMeasurment = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineIvenQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_InventoryQuantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineOpenIvnQty = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx13 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gc_line_QuantityOnStock = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx14 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx15 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx16 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gc_line_QuantityOnWareHouse = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx17 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx18 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx19 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridColumn57 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLine_SlpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineSlpCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcLine_VendorNum = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_VendorNum = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_NoInvtryMv = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx20 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gcLineEnSetCost = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineRetCost = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgrNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgrLnNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSpecPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineSpecPrice = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gc_CountryOrg = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CountryOrg = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_StdItemId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.cbxTaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_FreightTaxGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCalcEdit9 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tab_PaymentTerms = new Apzon.Controls.Controls.XtraTabPageEx();
		this.cal_TotalFC = new Apzon.Controls.Editor.CalcEditEx();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.ctmCopyToArInvoice = new DevExpress.XtraBars.BarButtonItem();
		this.ctmFromQuotation = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.bar_SaveAsDraft = new DevExpress.XtraBars.BarButtonItem();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.cal_Total = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_Total = new Apzon.Controls.Controls.LabelLine();
		this.spin_Count = new DevExpress.XtraEditors.SpinEdit();
		this.lb_Count = new Apzon.Controls.Controls.LabelLine();
		this.grdPaymentTerms = new Apzon.Controls.Grid.GridControlEx();
		this.gvPayTerms = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemCalcEdit8 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tab_Logistic = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdLogistic = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Logistic = new Apzon.Controls.Grid.GridViewEx();
		this.gcShipType = new Apzon.Controls.Grid.GridColumnEx();
		this.rlpShipType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcShipTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rShipTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Address2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gcBillTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rBillTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcConfirmed = new Apzon.Controls.Grid.GridColumnEx();
		this.rConfirmed = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Address = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gcLogistics_BPChCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLogisticsBpChannel = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcLogisticsBpChannelCntc = new Apzon.Controls.Grid.GridColumnEx();
		this.rLogisticBpChannelCntc = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PartSupply = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.tab_Freight = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Freight = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Freight = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ExpenseCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn67 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_GroupTax = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn66 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DistriMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn68 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_StatusFreight = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Project = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn53 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn54 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn52 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Stock = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LstPchPrce = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tabP_Accounting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Accounting = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Accounting = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Accounting_JrnlMemo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Accounting_Project = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_Accounting_Project = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Accounting_GroupNum = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Accounting_GroupNum = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Accounting_PeyMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Accounting_PaymentMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Accounting_Indicator = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Accounting_Indicator = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Accounting_LicTradNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Accounting_ImportEnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Accounting_PayDuMonth = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Accounting_PayDuMonth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Accounting_ExtraMonth = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Accounting_ExtraDays = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Accounting_CdcOffset = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_UseShpdGd = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ReferencedDocument = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.ucBatchAndSerial1 = new Apzon.Forms.UserControls.BatchAndSerial.ucBatchAndSerial();
		this.repositoryItemCalcEditEx24 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx25 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtNoOfTrans = new Apzon.Controls.Editor.TextEditEx();
		this.drlCopyTo = new Apzon.Controls.Controls.DropDownButtonEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.txtBusinessPartner = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbxStatus = new System.Windows.Forms.ComboBox();
		this.drlCopyFrom = new Apzon.Controls.Controls.DropDownButtonEx();
		this.txtRequester = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Requester = new Apzon.Controls.Controls.LabelLine();
		this.look_ReqType = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_CheckPromotion = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ClearPromotion = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.DropDownButtonEx();
		((System.ComponentModel.ISupportInitialize)this.split_Main).BeginInit();
		this.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.ctxDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Series).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtCardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReqType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Requester).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx26).BeginInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_OwnerCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rInfoPriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CtlAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceMode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlupContactPersonCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PmtToDraw).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_LineBatchAndSerial).BeginInit();
		this.split_LineBatchAndSerial.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Line).BeginInit();
		this.xtraTab_Line.SuspendLayout();
		this.tbDocumentLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		this.ctxLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtItemcode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCatalog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLookUpVatGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEnSetCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtWhseCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_CtlAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePackQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LstPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightStock).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUseBaseUnits).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UnitsOfMeasurment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_InventoryQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineSlpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VendorNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_NoInvtryMv).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryOrg).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbxTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightTaxGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit9).BeginInit();
		this.tab_PaymentTerms.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_TotalFC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Count.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdPaymentTerms).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvPayTerms).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).BeginInit();
		this.tab_Logistic.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLogistic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Logistic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rShipTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBillTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rConfirmed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLogisticsBpChannel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLogisticBpChannelCntc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PartSupply).BeginInit();
		this.tab_Freight.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Freight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Freight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ExpenseCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GroupTax).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DistriMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StatusFreight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stock).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LstPchPrce).BeginInit();
		this.tabP_Accounting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Accounting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Accounting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Accounting_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_GroupNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_PaymentMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_Indicator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_PayDuMonth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_UseShpdGd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReferencedDocument).BeginInit();
		this.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRequester.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ReqType.Properties).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.split_Main, "split_Main");
		this.split_Main.Horizontal = false;
		this.split_Main.Name = "split_Main";
		this.split_Main.Panel1.Controls.Add(this.grdDoc);
		resources.ApplyResources(this.split_Main.Panel1, "split_Main.Panel1");
		this.split_Main.Panel2.Controls.Add(this.split_LineBatchAndSerial);
		resources.ApplyResources(this.split_Main.Panel2, "split_Main.Panel2");
		this.split_Main.SplitterPosition = 143;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.ctxDoc;
		resources.ApplyResources(this.grdDoc, "grdDoc");
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[24]
		{
			this.rtxtCardCode, this.rlupContactPersonCode, this.repositoryItemCalcEdit2, this.repositoryItemCalcEdit3, this.repositoryItemCalcEdit4, this.rILCurrency, this.repositoryItemDateEdit3, this.rep_ApprovalStatus, this.repositoryItemDateEdit4, this.rep_Series,
			this.rep_ReqType, this.rep_Requester, this.rep_Department, this.rep_Branch, this.repositoryItemMemoExEdit1, this.repositoryItemMemoExEdit2, this.rep_doc_OwnerCode, this.rDocProject, this.rInfoPriceList, this.repositoryItemMemoExEdit3,
			this.rep_PmtToDraw, this.rep_CtlAccount, this.rep_PriceMode, this.rILSalesEmp
		});
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.ctxDoc.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctxDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[19]
		{
			this.ctm_AddRow, this.ctm_Duplicate, this.ctm_NewTask, this.ctm_NewEvent, this.ctm_NewNote, this.ctm_ForLandedCost, this.toolStripSeparator1, this.ctm_RemoveDoc, this.ctm_Cancel, this.ctm_Close,
			this.toolStripSeparator2, this.toolStripMenuItemOpeningClosing, this.toolStripSeparator5, this.ctm_SaveAsDraft, this.ctm_targetDocument, this.ctm_baseDocument, this.toolStripSeparator4, this.viewDiagramToolStripMenuItem, this.ctm_RelatedActivities
		});
		this.ctxDoc.Name = "contextMenuStrip1";
		resources.ApplyResources(this.ctxDoc, "ctxDoc");
		this.ctxDoc.Opening += new System.ComponentModel.CancelEventHandler(this.ctxDoc_Opening);
		this.ctm_AddRow.Name = "ctm_AddRow";
		resources.ApplyResources(this.ctm_AddRow, "ctm_AddRow");
		this.ctm_AddRow.Click += new System.EventHandler(this.ctmAddRow_Click);
		this.ctm_Duplicate.Name = "ctm_Duplicate";
		resources.ApplyResources(this.ctm_Duplicate, "ctm_Duplicate");
		this.ctm_Duplicate.Click += new System.EventHandler(this.ctmDuplicate_Click);
		this.ctm_NewTask.Name = "ctm_NewTask";
		resources.ApplyResources(this.ctm_NewTask, "ctm_NewTask");
		this.ctm_NewTask.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
		this.ctm_NewEvent.Name = "ctm_NewEvent";
		resources.ApplyResources(this.ctm_NewEvent, "ctm_NewEvent");
		this.ctm_NewEvent.Click += new System.EventHandler(this.ctm_NewEvent_Click);
		this.ctm_NewNote.Name = "ctm_NewNote";
		resources.ApplyResources(this.ctm_NewNote, "ctm_NewNote");
		this.ctm_NewNote.Click += new System.EventHandler(this.ctm_NewNote_Click);
		this.ctm_ForLandedCost.Name = "ctm_ForLandedCost";
		resources.ApplyResources(this.ctm_ForLandedCost, "ctm_ForLandedCost");
		this.ctm_ForLandedCost.Click += new System.EventHandler(this.ctm_ForLandedCost_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.ctm_RemoveDoc.Name = "ctm_RemoveDoc";
		resources.ApplyResources(this.ctm_RemoveDoc, "ctm_RemoveDoc");
		this.ctm_RemoveDoc.Click += new System.EventHandler(this.ctm_RemoveDoc_Click);
		this.ctm_Cancel.Name = "ctm_Cancel";
		resources.ApplyResources(this.ctm_Cancel, "ctm_Cancel");
		this.ctm_Cancel.Click += new System.EventHandler(this.cmtCancel_Click);
		this.ctm_Close.Name = "ctm_Close";
		resources.ApplyResources(this.ctm_Close, "ctm_Close");
		this.ctm_Close.Click += new System.EventHandler(this.cmtClose_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		this.toolStripMenuItemOpeningClosing.Name = "toolStripMenuItemOpeningClosing";
		resources.ApplyResources(this.toolStripMenuItemOpeningClosing, "toolStripMenuItemOpeningClosing");
		this.toolStripMenuItemOpeningClosing.Click += new System.EventHandler(this.toolStripMenuItemOpeningClosing_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
		this.ctm_SaveAsDraft.Name = "ctm_SaveAsDraft";
		resources.ApplyResources(this.ctm_SaveAsDraft, "ctm_SaveAsDraft");
		this.ctm_SaveAsDraft.Click += new System.EventHandler(this.ctmSaveAsDraft_Click);
		this.ctm_targetDocument.Name = "ctm_targetDocument";
		resources.ApplyResources(this.ctm_targetDocument, "ctm_targetDocument");
		this.ctm_targetDocument.Click += new System.EventHandler(this.targetDocumentToolStripMenuItem_Click);
		this.ctm_baseDocument.Name = "ctm_baseDocument";
		resources.ApplyResources(this.ctm_baseDocument, "ctm_baseDocument");
		this.ctm_baseDocument.Click += new System.EventHandler(this.baseDocumentToolStripMenuItem_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		resources.ApplyResources(this.viewDiagramToolStripMenuItem, "viewDiagramToolStripMenuItem");
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.ctm_RelatedActivities.Name = "ctm_RelatedActivities";
		resources.ApplyResources(this.ctm_RelatedActivities, "ctm_RelatedActivities");
		this.ctm_RelatedActivities.Click += new System.EventHandler(this.relatedActivitiesToolStripMenuItem_Click);
		this.gvDoc.Appearance.HotTrackedRow.BackColor = System.Drawing.Color.PaleGoldenrod;
		this.gvDoc.Appearance.HotTrackedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[54]
		{
			this.gcDoc_Series, this.gcDocNum, this.gcDoc_CardCode, this.gcDoc_CardName, this.gcDoc_CntctCode, this.gcDoc_NumAtCard, this.gcStatus, this.gcDoc_ReqType, this.gcDoc_Requester, this.gcDoc_ReqName,
			this.gcDoc_Department, this.gcDoc_Branch, this.gcPostingDate, this.col_DueDate, this.col_DocDate, this.gcReqDate, this.gridColumnEx20, this.gcTotalBeforeDis, this.gcDpmPrcnt, this.gcDpmTotal,
			this.gcDpmVat, this.gcDpmVatFC, this.gcDis, this.gcDiscount, this.gridColumn49, this.gcTotalTax, this.gcDocTotal, this.gcVatAmount, this.gcTotal, this.gcCurrency,
			this.gcRate, this.gridColumn2, this.gridColumn5, this.gridColumn6, this.gcVatSumFC, this.gcDocTotalFC, this.gcDocAtcEntry, this.gc_Doc_Branch, this.gc_Doc_BranchVATRegNum, this.gcStageSalesEmp,
			this.gcApprovalStatus, this.gc_Doc_CurrencyType, this.gcRemark, this.gridColumnEx4, this.gridColumnEx5, this.gcDoc_OwnerCode, this.gridColumnEx15, this.gridColumnEx16, this.gcDocProject, this.gcInfoPriceList,
			this.gridColumnEx23, this.gridColumnEx24, this.gcCtlAccount, this.gcPriceMode
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oDocument";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "ORDR";
		this.gvDoc.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsDetail.EnableMasterViewMode = false;
		this.gvDoc.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gvDoc.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		resources.ApplyResources(this.gvDoc, "gvDoc");
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gvDoc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEdit);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.HiddenEditor += new System.EventHandler(this.gv_Doc_HiddenEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gvDoc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvDoc_MouseUp);
		resources.ApplyResources(this.gcDoc_Series, "gcDoc_Series");
		this.gcDoc_Series.ColumnEdit = this.rep_Series;
		this.gcDoc_Series.CurrencyDateField = null;
		this.gcDoc_Series.CurrencyField = null;
		this.gcDoc_Series.CurrencyFixed = null;
		this.gcDoc_Series.CurrencyFixedRate = 0.0;
		this.gcDoc_Series.CurrencyRateField = null;
		this.gcDoc_Series.FieldName = "Series";
		this.gcDoc_Series.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_Series.Name = "gcDoc_Series";
		this.gcDoc_Series.PrimaryKey = false;
		this.gcDoc_Series.UnitField = null;
		resources.ApplyResources(this.rep_Series, "rep_Series");
		this.rep_Series.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Series.Buttons"))
		});
		this.rep_Series.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[4]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_Series.Columns"), resources.GetString("rep_Series.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_Series.Columns2"), resources.GetString("rep_Series.Columns3")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_Series.Columns4"), resources.GetString("rep_Series.Columns5"), (int)resources.GetObject("rep_Series.Columns6"), (DevExpress.Utils.FormatType)resources.GetObject("rep_Series.Columns7"), resources.GetString("rep_Series.Columns8"), (bool)resources.GetObject("rep_Series.Columns9"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rep_Series.Columns10"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rep_Series.Columns11"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rep_Series.Columns12")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_Series.Columns13"), resources.GetString("rep_Series.Columns14"), (int)resources.GetObject("rep_Series.Columns15"), (DevExpress.Utils.FormatType)resources.GetObject("rep_Series.Columns16"), resources.GetString("rep_Series.Columns17"), (bool)resources.GetObject("rep_Series.Columns18"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rep_Series.Columns19"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rep_Series.Columns20"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rep_Series.Columns21"))
		});
		this.rep_Series.DisplayMember = "SeriesName";
		this.rep_Series.Name = "rep_Series";
		this.rep_Series.ValueMember = "Series";
		resources.ApplyResources(this.gcDocNum, "gcDocNum");
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.OptionsColumn.ReadOnly = true;
		this.gcDocNum.PrimaryKey = true;
		this.gcDocNum.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
		this.gcDocNum.UnitField = null;
		resources.ApplyResources(this.gcDoc_CardCode, "gcDoc_CardCode");
		this.gcDoc_CardCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.gcDoc_CardCode.ColumnEdit = this.rtxtCardCode;
		this.gcDoc_CardCode.CurrencyDateField = null;
		this.gcDoc_CardCode.CurrencyField = null;
		this.gcDoc_CardCode.CurrencyFixed = null;
		this.gcDoc_CardCode.CurrencyFixedRate = 0.0;
		this.gcDoc_CardCode.CurrencyRateField = null;
		this.gcDoc_CardCode.FieldName = "CardCode";
		this.gcDoc_CardCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_CardCode.Name = "gcDoc_CardCode";
		this.gcDoc_CardCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.False;
		this.gcDoc_CardCode.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.False;
		this.gcDoc_CardCode.PrimaryKey = false;
		this.gcDoc_CardCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDoc_CardCode.UnitField = null;
		resources.ApplyResources(this.rtxtCardCode, "rtxtCardCode");
		this.rtxtCardCode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		serializableAppearanceObject.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject.Options.UseBackColor = true;
		serializableAppearanceObject.Options.UseBorderColor = true;
		serializableAppearanceObject2.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject2.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject2.Options.UseBackColor = true;
		serializableAppearanceObject2.Options.UseBorderColor = true;
		serializableAppearanceObject3.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject3.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject3.Options.UseBackColor = true;
		serializableAppearanceObject3.Options.UseBorderColor = true;
		serializableAppearanceObject4.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject4.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject4.Options.UseBackColor = true;
		serializableAppearanceObject4.Options.UseBorderColor = true;
		this.rtxtCardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtCardCode.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtCardCode.Buttons1"), resources.GetString("rtxtCardCode.Buttons2"), (int)resources.GetObject("rtxtCardCode.Buttons3"), (bool)resources.GetObject("rtxtCardCode.Buttons4"), (bool)resources.GetObject("rtxtCardCode.Buttons5"), (bool)resources.GetObject("rtxtCardCode.Buttons6"), editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, resources.GetString("rtxtCardCode.Buttons7"), resources.GetObject("rtxtCardCode.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rtxtCardCode.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rtxtCardCode.Buttons10"))
		});
		this.rtxtCardCode.Name = "rtxtCardCode";
		this.rtxtCardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rtxtCardCode_ButtonClick);
		resources.ApplyResources(this.gcDoc_CardName, "gcDoc_CardName");
		this.gcDoc_CardName.CurrencyDateField = null;
		this.gcDoc_CardName.CurrencyField = null;
		this.gcDoc_CardName.CurrencyFixed = null;
		this.gcDoc_CardName.CurrencyFixedRate = 0.0;
		this.gcDoc_CardName.CurrencyRateField = null;
		this.gcDoc_CardName.FieldName = "CardName";
		this.gcDoc_CardName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_CardName.Name = "gcDoc_CardName";
		this.gcDoc_CardName.PrimaryKey = false;
		this.gcDoc_CardName.UnitField = null;
		resources.ApplyResources(this.gcDoc_CntctCode, "gcDoc_CntctCode");
		this.gcDoc_CntctCode.CurrencyDateField = null;
		this.gcDoc_CntctCode.CurrencyField = null;
		this.gcDoc_CntctCode.CurrencyFixed = null;
		this.gcDoc_CntctCode.CurrencyFixedRate = 0.0;
		this.gcDoc_CntctCode.CurrencyRateField = null;
		this.gcDoc_CntctCode.FieldName = "CntctCode";
		this.gcDoc_CntctCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_CntctCode.Name = "gcDoc_CntctCode";
		this.gcDoc_CntctCode.PrimaryKey = false;
		this.gcDoc_CntctCode.UnitField = null;
		resources.ApplyResources(this.gcDoc_NumAtCard, "gcDoc_NumAtCard");
		this.gcDoc_NumAtCard.CurrencyDateField = null;
		this.gcDoc_NumAtCard.CurrencyField = null;
		this.gcDoc_NumAtCard.CurrencyFixed = null;
		this.gcDoc_NumAtCard.CurrencyFixedRate = 0.0;
		this.gcDoc_NumAtCard.CurrencyRateField = null;
		this.gcDoc_NumAtCard.FieldName = "NumAtCard";
		this.gcDoc_NumAtCard.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_NumAtCard.Name = "gcDoc_NumAtCard";
		this.gcDoc_NumAtCard.PrimaryKey = false;
		this.gcDoc_NumAtCard.UnitField = null;
		resources.ApplyResources(this.gcStatus, "gcStatus");
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "DocStatusName";
		this.gcStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.OptionsColumn.ReadOnly = true;
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		resources.ApplyResources(this.gcDoc_ReqType, "gcDoc_ReqType");
		this.gcDoc_ReqType.ColumnEdit = this.rep_ReqType;
		this.gcDoc_ReqType.CurrencyDateField = null;
		this.gcDoc_ReqType.CurrencyField = null;
		this.gcDoc_ReqType.CurrencyFixed = null;
		this.gcDoc_ReqType.CurrencyFixedRate = 0.0;
		this.gcDoc_ReqType.CurrencyRateField = null;
		this.gcDoc_ReqType.FieldName = "ReqType";
		this.gcDoc_ReqType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_ReqType.Name = "gcDoc_ReqType";
		this.gcDoc_ReqType.PrimaryKey = false;
		this.gcDoc_ReqType.UnitField = null;
		resources.ApplyResources(this.rep_ReqType, "rep_ReqType");
		this.rep_ReqType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_ReqType.Buttons"))
		});
		this.rep_ReqType.Name = "rep_ReqType";
		resources.ApplyResources(this.gcDoc_Requester, "gcDoc_Requester");
		this.gcDoc_Requester.ColumnEdit = this.rep_Requester;
		this.gcDoc_Requester.CurrencyDateField = null;
		this.gcDoc_Requester.CurrencyField = null;
		this.gcDoc_Requester.CurrencyFixed = null;
		this.gcDoc_Requester.CurrencyFixedRate = 0.0;
		this.gcDoc_Requester.CurrencyRateField = null;
		this.gcDoc_Requester.FieldName = "Requester";
		this.gcDoc_Requester.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_Requester.Name = "gcDoc_Requester";
		this.gcDoc_Requester.PrimaryKey = false;
		this.gcDoc_Requester.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDoc_Requester.UnitField = null;
		resources.ApplyResources(this.rep_Requester, "rep_Requester");
		this.rep_Requester.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Requester.Buttons"))
		});
		this.rep_Requester.Name = "rep_Requester";
		this.rep_Requester.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Requester_ButtonClick);
		resources.ApplyResources(this.gcDoc_ReqName, "gcDoc_ReqName");
		this.gcDoc_ReqName.CurrencyDateField = null;
		this.gcDoc_ReqName.CurrencyField = null;
		this.gcDoc_ReqName.CurrencyFixed = null;
		this.gcDoc_ReqName.CurrencyFixedRate = 0.0;
		this.gcDoc_ReqName.CurrencyRateField = null;
		this.gcDoc_ReqName.FieldName = "ReqName";
		this.gcDoc_ReqName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_ReqName.Name = "gcDoc_ReqName";
		this.gcDoc_ReqName.PrimaryKey = false;
		this.gcDoc_ReqName.UnitField = null;
		resources.ApplyResources(this.gcDoc_Department, "gcDoc_Department");
		this.gcDoc_Department.ColumnEdit = this.rep_Department;
		this.gcDoc_Department.CurrencyDateField = null;
		this.gcDoc_Department.CurrencyField = null;
		this.gcDoc_Department.CurrencyFixed = null;
		this.gcDoc_Department.CurrencyFixedRate = 0.0;
		this.gcDoc_Department.CurrencyRateField = null;
		this.gcDoc_Department.FieldName = "Department";
		this.gcDoc_Department.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_Department.Name = "gcDoc_Department";
		this.gcDoc_Department.PrimaryKey = false;
		this.gcDoc_Department.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDoc_Department.UnitField = null;
		resources.ApplyResources(this.rep_Department, "rep_Department");
		this.rep_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Department.Buttons"))
		});
		this.rep_Department.DisplayMember = "Name";
		this.rep_Department.Name = "rep_Department";
		this.rep_Department.ValueMember = "Code";
		resources.ApplyResources(this.gcDoc_Branch, "gcDoc_Branch");
		this.gcDoc_Branch.ColumnEdit = this.rep_Branch;
		this.gcDoc_Branch.CurrencyDateField = null;
		this.gcDoc_Branch.CurrencyField = null;
		this.gcDoc_Branch.CurrencyFixed = null;
		this.gcDoc_Branch.CurrencyFixedRate = 0.0;
		this.gcDoc_Branch.CurrencyRateField = null;
		this.gcDoc_Branch.FieldName = "Branch";
		this.gcDoc_Branch.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_Branch.Name = "gcDoc_Branch";
		this.gcDoc_Branch.PrimaryKey = false;
		this.gcDoc_Branch.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDoc_Branch.UnitField = null;
		resources.ApplyResources(this.rep_Branch, "rep_Branch");
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Branch.Buttons"))
		});
		this.rep_Branch.DisplayMember = "Name";
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.ValueMember = "Code";
		resources.ApplyResources(this.gcPostingDate, "gcPostingDate");
		this.gcPostingDate.CurrencyDateField = null;
		this.gcPostingDate.CurrencyField = null;
		this.gcPostingDate.CurrencyFixed = null;
		this.gcPostingDate.CurrencyFixedRate = 0.0;
		this.gcPostingDate.CurrencyRateField = null;
		this.gcPostingDate.DisplayFormat.FormatString = "d";
		this.gcPostingDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcPostingDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcPostingDate.FieldName = "DocDate";
		this.gcPostingDate.Name = "gcPostingDate";
		this.gcPostingDate.PrimaryKey = false;
		this.gcPostingDate.UnitField = null;
		resources.ApplyResources(this.col_DueDate, "col_DueDate");
		this.col_DueDate.CurrencyDateField = null;
		this.col_DueDate.CurrencyField = null;
		this.col_DueDate.CurrencyFixed = null;
		this.col_DueDate.CurrencyFixedRate = 0.0;
		this.col_DueDate.CurrencyRateField = null;
		this.col_DueDate.DisplayFormat.FormatString = "d";
		this.col_DueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.col_DueDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.col_DueDate.FieldName = "DocDueDate";
		this.col_DueDate.Name = "col_DueDate";
		this.col_DueDate.PrimaryKey = false;
		this.col_DueDate.UnitField = null;
		resources.ApplyResources(this.col_DocDate, "col_DocDate");
		this.col_DocDate.CurrencyDateField = null;
		this.col_DocDate.CurrencyField = null;
		this.col_DocDate.CurrencyFixed = null;
		this.col_DocDate.CurrencyFixedRate = 0.0;
		this.col_DocDate.CurrencyRateField = null;
		this.col_DocDate.DisplayFormat.FormatString = "d";
		this.col_DocDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.col_DocDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.col_DocDate.FieldName = "TaxDate";
		this.col_DocDate.Name = "col_DocDate";
		this.col_DocDate.PrimaryKey = false;
		this.col_DocDate.UnitField = null;
		resources.ApplyResources(this.gcReqDate, "gcReqDate");
		this.gcReqDate.CurrencyDateField = null;
		this.gcReqDate.CurrencyField = null;
		this.gcReqDate.CurrencyFixed = null;
		this.gcReqDate.CurrencyFixedRate = 0.0;
		this.gcReqDate.CurrencyRateField = null;
		this.gcReqDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcReqDate.FieldName = "ReqDate";
		this.gcReqDate.Name = "gcReqDate";
		this.gcReqDate.PrimaryKey = false;
		this.gcReqDate.UnitField = null;
		resources.ApplyResources(this.gridColumnEx20, "gridColumnEx20");
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx20.FieldName = "AssetDate";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		resources.ApplyResources(this.gcTotalBeforeDis, "gcTotalBeforeDis");
		this.gcTotalBeforeDis.ColumnEdit = this.repositoryItemCalcEdit4;
		this.gcTotalBeforeDis.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcTotalBeforeDis.CurrencyDateField = null;
		this.gcTotalBeforeDis.CurrencyField = "DocCur";
		this.gcTotalBeforeDis.CurrencyFixed = null;
		this.gcTotalBeforeDis.CurrencyFixedRate = 0.0;
		this.gcTotalBeforeDis.CurrencyRateField = null;
		this.gcTotalBeforeDis.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcTotalBeforeDis.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcTotalBeforeDis.FieldName = "TotalBeforeDis";
		this.gcTotalBeforeDis.Name = "gcTotalBeforeDis";
		this.gcTotalBeforeDis.OptionsColumn.ReadOnly = true;
		this.gcTotalBeforeDis.PrimaryKey = false;
		this.gcTotalBeforeDis.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit4, "repositoryItemCalcEdit4");
		this.repositoryItemCalcEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit4.Buttons"))
		});
		this.repositoryItemCalcEdit4.Name = "repositoryItemCalcEdit4";
		resources.ApplyResources(this.gcDpmPrcnt, "gcDpmPrcnt");
		this.gcDpmPrcnt.ColumnEdit = this.repositoryItemCalcEditEx21;
		this.gcDpmPrcnt.CurrencyDateField = null;
		this.gcDpmPrcnt.CurrencyField = null;
		this.gcDpmPrcnt.CurrencyFixed = null;
		this.gcDpmPrcnt.CurrencyFixedRate = 0.0;
		this.gcDpmPrcnt.CurrencyRateField = null;
		this.gcDpmPrcnt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDpmPrcnt.FieldName = "DpmPrcnt";
		this.gcDpmPrcnt.Name = "gcDpmPrcnt";
		this.gcDpmPrcnt.PrimaryKey = false;
		this.gcDpmPrcnt.UnitField = null;
		this.repositoryItemCalcEditEx21.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx21.Buttons"))
		});
		this.repositoryItemCalcEditEx21.IsSearchControl = false;
		this.repositoryItemCalcEditEx21.Name = "repositoryItemCalcEditEx21";
		resources.ApplyResources(this.gcDpmTotal, "gcDpmTotal");
		this.gcDpmTotal.ColumnEdit = this.repositoryItemCalcEditEx22;
		this.gcDpmTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcDpmTotal.CurrencyDateField = null;
		this.gcDpmTotal.CurrencyField = "DocCur";
		this.gcDpmTotal.CurrencyFixed = null;
		this.gcDpmTotal.CurrencyFixedRate = 0.0;
		this.gcDpmTotal.CurrencyRateField = null;
		this.gcDpmTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDpmTotal.FieldName = "DPMTotal";
		this.gcDpmTotal.Name = "gcDpmTotal";
		this.gcDpmTotal.PrimaryKey = false;
		this.gcDpmTotal.UnitField = null;
		this.repositoryItemCalcEditEx22.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx22.Buttons"))
		});
		this.repositoryItemCalcEditEx22.IsSearchControl = false;
		this.repositoryItemCalcEditEx22.Name = "repositoryItemCalcEditEx22";
		this.gcDpmVat.CurrencyDateField = null;
		this.gcDpmVat.CurrencyField = null;
		this.gcDpmVat.CurrencyFixed = null;
		this.gcDpmVat.CurrencyFixedRate = 0.0;
		this.gcDpmVat.CurrencyRateField = null;
		this.gcDpmVat.FieldName = "DpmVat";
		this.gcDpmVat.Name = "gcDpmVat";
		this.gcDpmVat.PrimaryKey = false;
		this.gcDpmVat.UnitField = null;
		this.gcDpmVatFC.CurrencyDateField = null;
		this.gcDpmVatFC.CurrencyField = null;
		this.gcDpmVatFC.CurrencyFixed = null;
		this.gcDpmVatFC.CurrencyFixedRate = 0.0;
		this.gcDpmVatFC.CurrencyRateField = null;
		this.gcDpmVatFC.FieldName = "DpmVatFc";
		this.gcDpmVatFC.Name = "gcDpmVatFC";
		this.gcDpmVatFC.PrimaryKey = false;
		this.gcDpmVatFC.UnitField = null;
		resources.ApplyResources(this.gcDis, "gcDis");
		this.gcDis.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcDis.CurrencyDateField = null;
		this.gcDis.CurrencyField = null;
		this.gcDis.CurrencyFixed = null;
		this.gcDis.CurrencyFixedRate = 0.0;
		this.gcDis.CurrencyRateField = null;
		this.gcDis.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDis.FieldName = "DiscPrcnt";
		this.gcDis.Name = "gcDis";
		this.gcDis.PrimaryKey = false;
		this.gcDis.UnitField = null;
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx1.Buttons"))
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		resources.ApplyResources(this.gcDiscount, "gcDiscount");
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEditEx26;
		this.gcDiscount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = "DocCur";
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.UnitField = null;
		this.repositoryItemCalcEditEx26.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx26.Buttons"))
		});
		this.repositoryItemCalcEditEx26.IsSearchControl = false;
		this.repositoryItemCalcEditEx26.Name = "repositoryItemCalcEditEx26";
		resources.ApplyResources(this.gridColumn49, "gridColumn49");
		repositoryItemCalcEditEx.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx27.Buttons"))
		});
		repositoryItemCalcEditEx.IsSearchControl = false;
		this.gridColumn49.ColumnEdit = repositoryItemCalcEditEx;
		this.gridColumn49.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn49.CurrencyDateField = null;
		this.gridColumn49.CurrencyField = "DocCur";
		this.gridColumn49.CurrencyFixed = null;
		this.gridColumn49.CurrencyFixedRate = 0.0;
		this.gridColumn49.CurrencyRateField = null;
		this.gridColumn49.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumn49.FieldName = "TotalFreight";
		this.gridColumn49.Name = "gridColumn49";
		this.gridColumn49.PrimaryKey = false;
		this.gridColumn49.UnitField = null;
		resources.ApplyResources(this.gcTotalTax, "gcTotalTax");
		this.gcTotalTax.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gcTotalTax.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcTotalTax.CurrencyDateField = null;
		this.gcTotalTax.CurrencyField = null;
		this.gcTotalTax.CurrencyFixed = null;
		this.gcTotalTax.CurrencyFixedRate = 0.0;
		this.gcTotalTax.CurrencyRateField = null;
		this.gcTotalTax.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcTotalTax.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcTotalTax.FieldName = "VatSum";
		this.gcTotalTax.Name = "gcTotalTax";
		this.gcTotalTax.OptionsColumn.ReadOnly = true;
		this.gcTotalTax.PrimaryKey = false;
		this.gcTotalTax.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit3, "repositoryItemCalcEdit3");
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit3.Buttons"))
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		resources.ApplyResources(this.gcDocTotal, "gcDocTotal");
		this.gcDocTotal.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gcDocTotal.CurrencyDateField = null;
		this.gcDocTotal.CurrencyField = null;
		this.gcDocTotal.CurrencyFixed = null;
		this.gcDocTotal.CurrencyFixedRate = 0.0;
		this.gcDocTotal.CurrencyRateField = null;
		this.gcDocTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDocTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotal.FieldName = "DocTotal";
		this.gcDocTotal.Name = "gcDocTotal";
		this.gcDocTotal.OptionsColumn.ReadOnly = true;
		this.gcDocTotal.PrimaryKey = false;
		this.gcDocTotal.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit2, "repositoryItemCalcEdit2");
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit2.Buttons"))
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		resources.ApplyResources(this.gcVatAmount, "gcVatAmount");
		this.gcVatAmount.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcVatAmount.CurrencyDateField = null;
		this.gcVatAmount.CurrencyField = null;
		this.gcVatAmount.CurrencyFixed = null;
		this.gcVatAmount.CurrencyFixedRate = 0.0;
		this.gcVatAmount.CurrencyRateField = null;
		this.gcVatAmount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcVatAmount.FieldName = "TaxTotal";
		this.gcVatAmount.Name = "gcVatAmount";
		this.gcVatAmount.OptionsColumn.ReadOnly = true;
		this.gcVatAmount.PrimaryKey = false;
		this.gcVatAmount.UnitField = null;
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx2.Buttons"))
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		resources.ApplyResources(this.gcTotal, "gcTotal");
		this.gcTotal.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gcTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcTotal.CurrencyDateField = null;
		this.gcTotal.CurrencyField = "DocCur";
		this.gcTotal.CurrencyFixed = null;
		this.gcTotal.CurrencyFixedRate = 0.0;
		this.gcTotal.CurrencyRateField = null;
		this.gcTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcTotal.FieldName = "Total";
		this.gcTotal.Name = "gcTotal";
		this.gcTotal.OptionsColumn.ReadOnly = true;
		this.gcTotal.PrimaryKey = false;
		this.gcTotal.UnitField = null;
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx3.Buttons"))
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		resources.ApplyResources(this.gcCurrency, "gcCurrency");
		this.gcCurrency.ColumnEdit = this.rILCurrency;
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "DocCur";
		this.gcCurrency.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		resources.ApplyResources(this.rILCurrency, "rILCurrency");
		this.rILCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rILCurrency.Buttons"))
		});
		this.rILCurrency.DisplayMember = "Code";
		this.rILCurrency.Name = "rILCurrency";
		this.rILCurrency.ValueMember = "Code";
		resources.ApplyResources(this.gcRate, "gcRate");
		this.gcRate.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gcRate.CurrencyDateField = null;
		this.gcRate.CurrencyField = null;
		this.gcRate.CurrencyFixed = null;
		this.gcRate.CurrencyFixedRate = 0.0;
		this.gcRate.CurrencyRateField = null;
		this.gcRate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gcRate.FieldName = "DocRate";
		this.gcRate.Name = "gcRate";
		this.gcRate.PrimaryKey = false;
		this.gcRate.UnitField = null;
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx4.Buttons"))
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		resources.ApplyResources(this.gridColumn2, "gridColumn2");
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DocEntry";
		this.gridColumn2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		resources.ApplyResources(this.gridColumn5, "gridColumn5");
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CANCELED";
		this.gridColumn5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		resources.ApplyResources(this.gridColumn6, "gridColumn6");
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "DocStatus";
		this.gridColumn6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		resources.ApplyResources(this.gcVatSumFC, "gcVatSumFC");
		this.gcVatSumFC.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gcVatSumFC.CurrencyDateField = null;
		this.gcVatSumFC.CurrencyField = null;
		this.gcVatSumFC.CurrencyFixed = null;
		this.gcVatSumFC.CurrencyFixedRate = 0.0;
		this.gcVatSumFC.CurrencyRateField = null;
		this.gcVatSumFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcVatSumFC.FieldName = "VatSumFC";
		this.gcVatSumFC.Name = "gcVatSumFC";
		this.gcVatSumFC.OptionsColumn.ReadOnly = true;
		this.gcVatSumFC.PrimaryKey = false;
		this.gcVatSumFC.UnitField = null;
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx5.Buttons"))
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		resources.ApplyResources(this.gcDocTotalFC, "gcDocTotalFC");
		this.gcDocTotalFC.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gcDocTotalFC.CurrencyDateField = null;
		this.gcDocTotalFC.CurrencyField = null;
		this.gcDocTotalFC.CurrencyFixed = null;
		this.gcDocTotalFC.CurrencyFixedRate = 0.0;
		this.gcDocTotalFC.CurrencyRateField = null;
		this.gcDocTotalFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotalFC.FieldName = "DocTotalFC";
		this.gcDocTotalFC.Name = "gcDocTotalFC";
		this.gcDocTotalFC.OptionsColumn.ReadOnly = true;
		this.gcDocTotalFC.PrimaryKey = false;
		this.gcDocTotalFC.UnitField = null;
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx6.Buttons"))
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		resources.ApplyResources(this.gcDocAtcEntry, "gcDocAtcEntry");
		this.gcDocAtcEntry.CurrencyDateField = null;
		this.gcDocAtcEntry.CurrencyField = null;
		this.gcDocAtcEntry.CurrencyFixed = null;
		this.gcDocAtcEntry.CurrencyFixedRate = 0.0;
		this.gcDocAtcEntry.CurrencyRateField = null;
		this.gcDocAtcEntry.FieldName = "AtcEntry";
		this.gcDocAtcEntry.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocAtcEntry.Name = "gcDocAtcEntry";
		this.gcDocAtcEntry.PrimaryKey = false;
		this.gcDocAtcEntry.UnitField = null;
		resources.ApplyResources(this.gc_Doc_Branch, "gc_Doc_Branch");
		this.gc_Doc_Branch.CurrencyDateField = null;
		this.gc_Doc_Branch.CurrencyField = null;
		this.gc_Doc_Branch.CurrencyFixed = null;
		this.gc_Doc_Branch.CurrencyFixedRate = 0.0;
		this.gc_Doc_Branch.CurrencyRateField = null;
		this.gc_Doc_Branch.FieldName = "BPLId";
		this.gc_Doc_Branch.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Doc_Branch.Name = "gc_Doc_Branch";
		this.gc_Doc_Branch.PrimaryKey = false;
		this.gc_Doc_Branch.UnitField = null;
		resources.ApplyResources(this.gc_Doc_BranchVATRegNum, "gc_Doc_BranchVATRegNum");
		this.gc_Doc_BranchVATRegNum.CurrencyDateField = null;
		this.gc_Doc_BranchVATRegNum.CurrencyField = null;
		this.gc_Doc_BranchVATRegNum.CurrencyFixed = null;
		this.gc_Doc_BranchVATRegNum.CurrencyFixedRate = 0.0;
		this.gc_Doc_BranchVATRegNum.CurrencyRateField = null;
		this.gc_Doc_BranchVATRegNum.FieldName = "VATRegNum";
		this.gc_Doc_BranchVATRegNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Doc_BranchVATRegNum.Name = "gc_Doc_BranchVATRegNum";
		this.gc_Doc_BranchVATRegNum.PrimaryKey = false;
		this.gc_Doc_BranchVATRegNum.UnitField = null;
		resources.ApplyResources(this.gcStageSalesEmp, "gcStageSalesEmp");
		this.gcStageSalesEmp.ColumnEdit = this.rILSalesEmp;
		this.gcStageSalesEmp.CurrencyDateField = null;
		this.gcStageSalesEmp.CurrencyField = null;
		this.gcStageSalesEmp.CurrencyFixed = null;
		this.gcStageSalesEmp.CurrencyFixedRate = 0.0;
		this.gcStageSalesEmp.CurrencyRateField = null;
		this.gcStageSalesEmp.FieldName = "SlpCode";
		this.gcStageSalesEmp.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcStageSalesEmp.Name = "gcStageSalesEmp";
		this.gcStageSalesEmp.PrimaryKey = false;
		this.gcStageSalesEmp.UnitField = null;
		this.rILSalesEmp.AllowMouseWheel = false;
		resources.ApplyResources(this.rILSalesEmp, "rILSalesEmp");
		this.rILSalesEmp.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rILSalesEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rILSalesEmp.Buttons"))
		});
		this.rILSalesEmp.DisplayMember = "SlpName";
		this.rILSalesEmp.Name = "rILSalesEmp";
		this.rILSalesEmp.PopupView = this.repositoryItemGridLookUpEdit1View;
		this.rILSalesEmp.ValueMember = "SlpCode";
		this.rILSalesEmp.EditValueChanged += new System.EventHandler(this.rILSalesEmp_EditValueChanged);
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsBehavior.AutoPopulateColumns = false;
		this.repositoryItemGridLookUpEdit1View.OptionsBehavior.Editable = false;
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowIndicator = false;
		resources.ApplyResources(this.gcApprovalStatus, "gcApprovalStatus");
		this.gcApprovalStatus.ColumnEdit = this.rep_ApprovalStatus;
		this.gcApprovalStatus.CurrencyDateField = null;
		this.gcApprovalStatus.CurrencyField = null;
		this.gcApprovalStatus.CurrencyFixed = null;
		this.gcApprovalStatus.CurrencyFixedRate = 0.0;
		this.gcApprovalStatus.CurrencyRateField = null;
		this.gcApprovalStatus.FieldName = "WddStatus";
		this.gcApprovalStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcApprovalStatus.Name = "gcApprovalStatus";
		this.gcApprovalStatus.OptionsColumn.ReadOnly = true;
		this.gcApprovalStatus.PrimaryKey = false;
		this.gcApprovalStatus.UnitField = null;
		resources.ApplyResources(this.rep_ApprovalStatus, "rep_ApprovalStatus");
		this.rep_ApprovalStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_ApprovalStatus.Buttons"))
		});
		this.rep_ApprovalStatus.Name = "rep_ApprovalStatus";
		resources.ApplyResources(this.gc_Doc_CurrencyType, "gc_Doc_CurrencyType");
		this.gc_Doc_CurrencyType.CurrencyDateField = null;
		this.gc_Doc_CurrencyType.CurrencyField = null;
		this.gc_Doc_CurrencyType.CurrencyFixed = null;
		this.gc_Doc_CurrencyType.CurrencyFixedRate = 0.0;
		this.gc_Doc_CurrencyType.CurrencyRateField = null;
		this.gc_Doc_CurrencyType.FieldName = "Currency";
		this.gc_Doc_CurrencyType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Doc_CurrencyType.Name = "gc_Doc_CurrencyType";
		this.gc_Doc_CurrencyType.PrimaryKey = false;
		this.gc_Doc_CurrencyType.UnitField = null;
		resources.ApplyResources(this.gcRemark, "gcRemark");
		this.gcRemark.ColumnEdit = this.repositoryItemMemoExEdit3;
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Comments";
		this.gcRemark.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		resources.ApplyResources(this.repositoryItemMemoExEdit3, "repositoryItemMemoExEdit3");
		this.repositoryItemMemoExEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemMemoExEdit3.Buttons"))
		});
		this.repositoryItemMemoExEdit3.Name = "repositoryItemMemoExEdit3";
		this.repositoryItemMemoExEdit3.ShowIcon = false;
		resources.ApplyResources(this.gridColumnEx4, "gridColumnEx4");
		this.gridColumnEx4.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Header";
		this.gridColumnEx4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		resources.ApplyResources(this.repositoryItemMemoExEdit1, "repositoryItemMemoExEdit1");
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemMemoExEdit1.Buttons"))
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		resources.ApplyResources(this.gridColumnEx5, "gridColumnEx5");
		this.gridColumnEx5.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Footer";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		resources.ApplyResources(this.repositoryItemMemoExEdit2, "repositoryItemMemoExEdit2");
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemMemoExEdit2.Buttons"))
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		resources.ApplyResources(this.gcDoc_OwnerCode, "gcDoc_OwnerCode");
		this.gcDoc_OwnerCode.ColumnEdit = this.rep_doc_OwnerCode;
		this.gcDoc_OwnerCode.CurrencyDateField = null;
		this.gcDoc_OwnerCode.CurrencyField = null;
		this.gcDoc_OwnerCode.CurrencyFixed = null;
		this.gcDoc_OwnerCode.CurrencyFixedRate = 0.0;
		this.gcDoc_OwnerCode.CurrencyRateField = null;
		this.gcDoc_OwnerCode.FieldName = "OwnerCode";
		this.gcDoc_OwnerCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDoc_OwnerCode.Name = "gcDoc_OwnerCode";
		this.gcDoc_OwnerCode.PrimaryKey = false;
		this.gcDoc_OwnerCode.UnitField = null;
		resources.ApplyResources(this.rep_doc_OwnerCode, "rep_doc_OwnerCode");
		this.rep_doc_OwnerCode.AutoLoadDataSource = true;
		this.rep_doc_OwnerCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_doc_OwnerCode.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_doc_OwnerCode.Buttons1"), resources.GetString("rep_doc_OwnerCode.Buttons2"), (int)resources.GetObject("rep_doc_OwnerCode.Buttons3"), (bool)resources.GetObject("rep_doc_OwnerCode.Buttons4"), (bool)resources.GetObject("rep_doc_OwnerCode.Buttons5"), (bool)resources.GetObject("rep_doc_OwnerCode.Buttons6"), imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, resources.GetString("rep_doc_OwnerCode.Buttons7"), resources.GetObject("rep_doc_OwnerCode.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rep_doc_OwnerCode.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rep_doc_OwnerCode.Buttons10"))
		});
		this.rep_doc_OwnerCode.ContainerControl = this;
		this.rep_doc_OwnerCode.DisplayMember = "firstName";
		this.rep_doc_OwnerCode.DisplayMember2 = "lastName";
		this.rep_doc_OwnerCode.ListMandatoryField = null;
		this.rep_doc_OwnerCode.LoadDataSourceWhenEnter = false;
		this.rep_doc_OwnerCode.Name = "rep_doc_OwnerCode";
		this.rep_doc_OwnerCode.SeparateCharacterForMulti = ';';
		this.rep_doc_OwnerCode.SeparateString = ", ";
		this.rep_doc_OwnerCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_doc_OwnerCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_doc_OwnerCode.UsingAutoCompleteBox = false;
		this.rep_doc_OwnerCode.UsingDataSource = true;
		this.rep_doc_OwnerCode.ValidateWhenNoDataSource = false;
		this.rep_doc_OwnerCode.ValueMember = "EmpId";
		resources.ApplyResources(this.gridColumnEx15, "gridColumnEx15");
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "U_RelatedTyp";
		this.gridColumnEx15.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		resources.ApplyResources(this.gridColumnEx16, "gridColumnEx16");
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "RelatedEnt";
		this.gridColumnEx16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		resources.ApplyResources(this.gcDocProject, "gcDocProject");
		this.gcDocProject.ColumnEdit = this.rDocProject;
		this.gcDocProject.CurrencyDateField = null;
		this.gcDocProject.CurrencyField = null;
		this.gcDocProject.CurrencyFixed = null;
		this.gcDocProject.CurrencyFixedRate = 0.0;
		this.gcDocProject.CurrencyRateField = null;
		this.gcDocProject.FieldName = "Project";
		this.gcDocProject.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocProject.Name = "gcDocProject";
		this.gcDocProject.PrimaryKey = false;
		this.gcDocProject.UnitField = null;
		resources.ApplyResources(this.rDocProject, "rDocProject");
		this.rDocProject.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rDocProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rDocProject.Buttons"), resources.GetString("rDocProject.Buttons1"), (int)resources.GetObject("rDocProject.Buttons2"), (bool)resources.GetObject("rDocProject.Buttons3"), (bool)resources.GetObject("rDocProject.Buttons4"), (bool)resources.GetObject("rDocProject.Buttons5"), imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, resources.GetString("rDocProject.Buttons6"), resources.GetObject("rDocProject.Buttons7"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rDocProject.Buttons8"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rDocProject.Buttons9")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rDocProject.Buttons10"), resources.GetString("rDocProject.Buttons11"), (int)resources.GetObject("rDocProject.Buttons12"), (bool)resources.GetObject("rDocProject.Buttons13"), (bool)resources.GetObject("rDocProject.Buttons14"), (bool)resources.GetObject("rDocProject.Buttons15"), editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, resources.GetString("rDocProject.Buttons16"), resources.GetObject("rDocProject.Buttons17"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rDocProject.Buttons18"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rDocProject.Buttons19"))
		});
		this.rDocProject.ContainerControl = this;
		this.rDocProject.DisplayMember = "";
		this.rDocProject.DisplayMember2 = "";
		this.rDocProject.ListMandatoryField = null;
		this.rDocProject.LoadDataSourceWhenEnter = false;
		this.rDocProject.Name = "rDocProject";
		this.rDocProject.SeparateCharacterForMulti = ';';
		this.rDocProject.SeparateString = ", ";
		this.rDocProject.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rDocProject.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProjectCodes;
		this.rDocProject.UsingAutoCompleteBox = false;
		this.rDocProject.UsingDataSource = true;
		this.rDocProject.ValidateWhenNoDataSource = false;
		this.rDocProject.ValueMember = "PrjCode";
		resources.ApplyResources(this.gcInfoPriceList, "gcInfoPriceList");
		this.gcInfoPriceList.ColumnEdit = this.rInfoPriceList;
		this.gcInfoPriceList.CurrencyDateField = null;
		this.gcInfoPriceList.CurrencyField = null;
		this.gcInfoPriceList.CurrencyFixed = null;
		this.gcInfoPriceList.CurrencyFixedRate = 0.0;
		this.gcInfoPriceList.CurrencyRateField = null;
		this.gcInfoPriceList.FieldName = "InfoPriceList";
		this.gcInfoPriceList.Name = "gcInfoPriceList";
		this.gcInfoPriceList.PrimaryKey = false;
		this.gcInfoPriceList.UnitField = null;
		resources.ApplyResources(this.rInfoPriceList, "rInfoPriceList");
		this.rInfoPriceList.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rInfoPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rInfoPriceList.Buttons"))
		});
		this.rInfoPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rInfoPriceList.Columns"), resources.GetString("rInfoPriceList.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rInfoPriceList.Columns2"), resources.GetString("rInfoPriceList.Columns3"), (int)resources.GetObject("rInfoPriceList.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rInfoPriceList.Columns5"), resources.GetString("rInfoPriceList.Columns6"), (bool)resources.GetObject("rInfoPriceList.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rInfoPriceList.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rInfoPriceList.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rInfoPriceList.Columns10"))
		});
		this.rInfoPriceList.DisplayMember = "Name";
		this.rInfoPriceList.IsSearchControl = false;
		this.rInfoPriceList.Name = "rInfoPriceList";
		this.rInfoPriceList.ShowFooter = false;
		this.rInfoPriceList.ShowHeader = false;
		this.rInfoPriceList.ValueMember = "Code";
		resources.ApplyResources(this.gridColumnEx23, "gridColumnEx23");
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "DpmAmnt";
		this.gridColumnEx23.MinWidth = 25;
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		resources.ApplyResources(this.gridColumnEx24, "gridColumnEx24");
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "DpmAmntFC";
		this.gridColumnEx24.MinWidth = 25;
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		resources.ApplyResources(this.gcCtlAccount, "gcCtlAccount");
		this.gcCtlAccount.ColumnEdit = this.rep_CtlAccount;
		this.gcCtlAccount.CurrencyDateField = null;
		this.gcCtlAccount.CurrencyField = null;
		this.gcCtlAccount.CurrencyFixed = null;
		this.gcCtlAccount.CurrencyFixedRate = 0.0;
		this.gcCtlAccount.CurrencyRateField = null;
		this.gcCtlAccount.FieldName = "CtlAccount";
		this.gcCtlAccount.Name = "gcCtlAccount";
		this.gcCtlAccount.PrimaryKey = false;
		this.gcCtlAccount.UnitField = null;
		resources.ApplyResources(this.rep_CtlAccount, "rep_CtlAccount");
		this.rep_CtlAccount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_CtlAccount.Buttons"))
		});
		this.rep_CtlAccount.Name = "rep_CtlAccount";
		this.rep_CtlAccount.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CtlAccount_ButtonClick);
		resources.ApplyResources(this.gcPriceMode, "gcPriceMode");
		this.gcPriceMode.ColumnEdit = this.rep_PriceMode;
		this.gcPriceMode.CurrencyDateField = null;
		this.gcPriceMode.CurrencyField = null;
		this.gcPriceMode.CurrencyFixed = null;
		this.gcPriceMode.CurrencyFixedRate = 0.0;
		this.gcPriceMode.CurrencyRateField = null;
		this.gcPriceMode.FieldName = "PriceMode";
		this.gcPriceMode.MinWidth = 25;
		this.gcPriceMode.Name = "gcPriceMode";
		this.gcPriceMode.PrimaryKey = false;
		this.gcPriceMode.UnitField = null;
		resources.ApplyResources(this.rep_PriceMode, "rep_PriceMode");
		this.rep_PriceMode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_PriceMode.Buttons"))
		});
		this.rep_PriceMode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_PriceMode.Columns"), resources.GetString("rep_PriceMode.Columns1"), (int)resources.GetObject("rep_PriceMode.Columns2"), (DevExpress.Utils.FormatType)resources.GetObject("rep_PriceMode.Columns3"), resources.GetString("rep_PriceMode.Columns4"), (bool)resources.GetObject("rep_PriceMode.Columns5"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rep_PriceMode.Columns6"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rep_PriceMode.Columns7"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rep_PriceMode.Columns8")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_PriceMode.Columns9"), resources.GetString("rep_PriceMode.Columns10"))
		});
		this.rep_PriceMode.DisplayMember = "Name";
		this.rep_PriceMode.Name = "rep_PriceMode";
		this.rep_PriceMode.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.ContentWidth;
		this.rep_PriceMode.ShowFooter = false;
		this.rep_PriceMode.ShowHeader = false;
		this.rep_PriceMode.ValueMember = "Code";
		resources.ApplyResources(this.rlupContactPersonCode, "rlupContactPersonCode");
		this.rlupContactPersonCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rlupContactPersonCode.Buttons"))
		});
		this.rlupContactPersonCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rlupContactPersonCode.Columns"), resources.GetString("rlupContactPersonCode.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rlupContactPersonCode.Columns2"), resources.GetString("rlupContactPersonCode.Columns3")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rlupContactPersonCode.Columns4"), resources.GetString("rlupContactPersonCode.Columns5"), (int)resources.GetObject("rlupContactPersonCode.Columns6"), (DevExpress.Utils.FormatType)resources.GetObject("rlupContactPersonCode.Columns7"), resources.GetString("rlupContactPersonCode.Columns8"), (bool)resources.GetObject("rlupContactPersonCode.Columns9"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rlupContactPersonCode.Columns10"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rlupContactPersonCode.Columns11"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rlupContactPersonCode.Columns12"))
		});
		this.rlupContactPersonCode.DisplayMember = "Name";
		this.rlupContactPersonCode.Name = "rlupContactPersonCode";
		this.rlupContactPersonCode.ValueMember = "Name";
		resources.ApplyResources(this.repositoryItemDateEdit3, "repositoryItemDateEdit3");
		this.repositoryItemDateEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit3.Buttons"))
		});
		this.repositoryItemDateEdit3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit3.CalendarTimeProperties.Buttons"))
		});
		this.repositoryItemDateEdit3.Name = "repositoryItemDateEdit3";
		resources.ApplyResources(this.repositoryItemDateEdit4, "repositoryItemDateEdit4");
		this.repositoryItemDateEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit4.Buttons"))
		});
		this.repositoryItemDateEdit4.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit4.CalendarTimeProperties.Buttons"))
		});
		this.repositoryItemDateEdit4.Name = "repositoryItemDateEdit4";
		this.rep_PmtToDraw.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_PmtToDraw.Name = "rep_PmtToDraw";
		this.rep_PmtToDraw.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_PmtToDraw.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_PmtToDraw_ButtonClick);
		resources.ApplyResources(this.split_LineBatchAndSerial, "split_LineBatchAndSerial");
		this.split_LineBatchAndSerial.Horizontal = false;
		this.split_LineBatchAndSerial.Name = "split_LineBatchAndSerial";
		this.split_LineBatchAndSerial.Panel1.Controls.Add(this.xtraTab_Line);
		resources.ApplyResources(this.split_LineBatchAndSerial.Panel1, "split_LineBatchAndSerial.Panel1");
		this.split_LineBatchAndSerial.Panel2.Controls.Add(this.ucBatchAndSerial1);
		resources.ApplyResources(this.split_LineBatchAndSerial.Panel2, "split_LineBatchAndSerial.Panel2");
		this.split_LineBatchAndSerial.SplitterPosition = 161;
		this.xtraTab_Line.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
		resources.ApplyResources(this.xtraTab_Line, "xtraTab_Line");
		this.xtraTab_Line.IsSearchControl = false;
		this.xtraTab_Line.Name = "xtraTab_Line";
		this.xtraTab_Line.SelectedTabPage = this.tbDocumentLine;
		this.xtraTab_Line.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[6] { this.tbDocumentLine, this.tab_PaymentTerms, this.tab_Logistic, this.tab_Freight, this.tabP_Accounting, this.tab_Attachments });
		this.tbDocumentLine.Controls.Add(this.grdLine);
		this.tbDocumentLine.EditControl = null;
		this.tbDocumentLine.EditValue = "Contents";
		this.tbDocumentLine.FieldName = null;
		this.tbDocumentLine.IsSearchControl = false;
		this.tbDocumentLine.Name = "tbDocumentLine";
		this.tbDocumentLine.ReadOnly = false;
		this.tbDocumentLine.SettingInVisible = false;
		this.tbDocumentLine.SettingReadonly = false;
		resources.ApplyResources(this.tbDocumentLine, "tbDocumentLine");
		this.tbDocumentLine.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.grdLine.AllowPasteExcel = false;
		this.grdLine.ClipboardData = null;
		this.grdLine.ContextMenuStrip = this.ctxLine;
		resources.ApplyResources(this.grdLine, "grdLine");
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[30]
		{
			this.rtxtItemcode, this.cbxTaxCode, this.rLookUpVatGroup, this.rtxtWhseCode, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit5, this.repositoryItemCalcEdit6, this.repositoryItemDateEdit1, this.rep_bte_CtlAccount, this.rep_FreightCode,
			this.rep_FreightTaxGroup, this.rep_LstPrice, this.rep_UnitsOfMeasurment, this.rep_InventoryQuantity, this.rLineUseBaseUnits, this.rBpCatalog, this.repositoryItemCalcEdit9, this.rep_LineSlpCode, this.rLineUomGroup, this.rLineUomCode,
			this.rep_FreightStock, this.rep_LineProject, this.rep_LineType, this.rep_VendorNum, this.rPriceEdit, this.rep_BaseType, this.rep_NoInvtryMv, this.rEnSetCost, this.rLineSpecPrice, this.rep_CountryOrg
		});
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.grdLine.Paint += new System.Windows.Forms.PaintEventHandler(this.grdLine_Paint);
		this.grdLine.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdLine_EditorKeyDown);
		this.ctxLine.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctxLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.ctm_addRowLine, this.ctm_delRowLine, this.ctm_LineClose, this.ctm_LineDuplicate, this.ctm_targetDocumentLine, this.ctm_baseDocumentLine, this.ctm_BatchSerialNumbers, this.alternativeItemsToolStripMenuItem });
		this.ctxLine.Name = "contextMenuStrip2";
		resources.ApplyResources(this.ctxLine, "ctxLine");
		this.ctxLine.Opening += new System.ComponentModel.CancelEventHandler(this.ctxLine_Opening);
		this.ctm_addRowLine.Name = "ctm_addRowLine";
		resources.ApplyResources(this.ctm_addRowLine, "ctm_addRowLine");
		this.ctm_addRowLine.Click += new System.EventHandler(this.cmt_addRowLine_Click);
		this.ctm_delRowLine.Name = "ctm_delRowLine";
		resources.ApplyResources(this.ctm_delRowLine, "ctm_delRowLine");
		this.ctm_delRowLine.Click += new System.EventHandler(this.cmt_delRowLine_Click);
		this.ctm_LineClose.Name = "ctm_LineClose";
		resources.ApplyResources(this.ctm_LineClose, "ctm_LineClose");
		this.ctm_LineClose.Click += new System.EventHandler(this.cmtLineClose_Click);
		this.ctm_LineDuplicate.Name = "ctm_LineDuplicate";
		resources.ApplyResources(this.ctm_LineDuplicate, "ctm_LineDuplicate");
		this.ctm_LineDuplicate.Click += new System.EventHandler(this.cmtLineDuplicate_Click);
		this.ctm_targetDocumentLine.Name = "ctm_targetDocumentLine";
		resources.ApplyResources(this.ctm_targetDocumentLine, "ctm_targetDocumentLine");
		this.ctm_targetDocumentLine.Click += new System.EventHandler(this.targetDocumentToolStripMenuItem_Click);
		this.ctm_baseDocumentLine.Name = "ctm_baseDocumentLine";
		resources.ApplyResources(this.ctm_baseDocumentLine, "ctm_baseDocumentLine");
		this.ctm_baseDocumentLine.Click += new System.EventHandler(this.baseDocumentToolStripMenuItem1_Click);
		this.ctm_BatchSerialNumbers.Name = "ctm_BatchSerialNumbers";
		resources.ApplyResources(this.ctm_BatchSerialNumbers, "ctm_BatchSerialNumbers");
		this.ctm_BatchSerialNumbers.Click += new System.EventHandler(this.ctm_BatchSerialNumbers_Click);
		this.alternativeItemsToolStripMenuItem.Name = "alternativeItemsToolStripMenuItem";
		resources.ApplyResources(this.alternativeItemsToolStripMenuItem, "alternativeItemsToolStripMenuItem");
		this.alternativeItemsToolStripMenuItem.Click += new System.EventHandler(this.alternativeItemsToolStripMenuItem_Click);
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = "ItemCode";
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[95]
		{
			this.gridColumnEx3, this.gcItemCode, this.gcItemName, this.gcBPCatalog, this.gcQuantity, this.gcUnitPrice, this.gcDisPer, this.gcVatGroup, this.gridColumnEx17, this.gcLineTaxOnly,
			this.gcTotalLine, this.gcWhseCode, this.gcLinePQTReqdate, this.gridColumnEx10, this.gcLine_DelDate, this.gcRemarkLine, this.gcOpenQty, this.gcCureencyLine, this.gcVatPrcnt, this.gcLineNum,
			this.gridColumn7, this.gcLineVat, this.gcLineTotalFC, this.gcLineVatSumFrgn, this.gridColumnEx18, this.gridColumnEx19, this.gcBOMTreeType, this.gcTrgetEntry, this.gcBaseType, this.gcBaseRef,
			this.gcTargetType, this.gcBaseEntry, this.gcBaseLine, this.gcLineTreeType, this.gcDeliveryQty, this.gcLineCtlAccount, this.gcLinePackQty, this.gridColumn19, this.gridColumn20, this.gridColumn21,
			this.gridColumn22, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29, this.gridColumn30, this.gridColumn31,
			this.gridColumn32, this.gridColumn33, this.gridColumn34, this.gridColumn35, this.gridColumn36, this.gridColumn37, this.gridColumn38, this.gridColumn39, this.gridColumn40, this.gridColumn41,
			this.gridColumn42, this.gridColumn43, this.gridColumn44, this.gridColumn45, this.gridColumn46, this.gridColumn47, this.gridColumn48, this.gcLineUomGroup, this.gcLineUomCode, this.gcLineUoMName,
			this.gc_Line_CogsOcrCo2, this.gcLineInventoryUom, this.gcLineItemPerUnit, this.gcLineIvenQty, this.gcLineOpenIvnQty, this.gc_line_QuantityOnStock, this.gridColumnEx6, this.gridColumnEx8, this.gc_line_QuantityOnWareHouse, this.gridColumnEx7,
			this.gridColumnEx9, this.gridColumn57, this.gcLine_SlpCode, this.gridColumnEx2, this.gcLine_VendorNum, this.gridColumnEx11, this.gridColumnEx12, this.gcLineEnSetCost, this.gcLineRetCost, this.gcAgrNo,
			this.gcAgrLnNum, this.gcLineSpecPrice, this.gc_CountryOrg, this.gc_StdItemId, this.gcUomEntry
		});
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "oDocument";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IndicatorWidth = 35;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.ItemId = "RDR1";
		this.gvLine.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsCustomization.AllowSort = false;
		this.gvLine.OptionsDetail.EnableMasterViewMode = false;
		this.gvLine.OptionsSelection.MultiSelect = true;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowFooter = true;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		resources.ApplyResources(this.gvLine, "gvLine");
		this.gvLine.CheckValidChanged += new Apzon.Controls.Grid.CheckValidChanged(this.gvLine_CheckValidChanged);
		this.gvLine.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvLine_CustomDrawCell);
		this.gvLine.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvLine_RowCellStyle);
		this.gvLine.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Line_RowStyle);
		this.gvLine.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Line_ShowingEditor);
		this.gvLine.HiddenEditor += new System.EventHandler(this.gv_Line_HiddenEditor);
		this.gvLine.ShownEditor += new System.EventHandler(this.gv_Line_ShownEditor);
		this.gvLine.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvLine_FocusedRowObjectChanged);
		this.gvLine.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvLine_MouseUp);
		resources.ApplyResources(this.gridColumnEx3, "gridColumnEx3");
		this.gridColumnEx3.ColumnEdit = this.rep_LineType;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "LineType";
		this.gridColumnEx3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		resources.ApplyResources(this.rep_LineType, "rep_LineType");
		this.rep_LineType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LineType.Buttons"))
		});
		this.rep_LineType.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[3]
		{
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("rep_LineType.Items"), resources.GetString("rep_LineType.Items1"), (int)resources.GetObject("rep_LineType.Items2")),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("rep_LineType.Items3"), resources.GetString("rep_LineType.Items4"), (int)resources.GetObject("rep_LineType.Items5")),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("rep_LineType.Items6"), resources.GetString("rep_LineType.Items7"), (int)resources.GetObject("rep_LineType.Items8"))
		});
		this.rep_LineType.Name = "rep_LineType";
		this.rep_LineType.SmallImages = this.imageCollection1;
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.imageCollection1.InsertGalleryImage("new_16x16.png", "office2013/actions/new_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/actions/new_16x16.png"), 0);
		this.imageCollection1.Images.SetKeyName(0, "new_16x16.png");
		this.imageCollection1.InsertGalleryImage("changetextcase_16x16.png", "images/richedit/changetextcase_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/richedit/changetextcase_16x16.png"), 1);
		this.imageCollection1.Images.SetKeyName(1, "changetextcase_16x16.png");
		this.imageCollection1.InsertGalleryImage("summary_16x16.png", "images/data/summary_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/data/summary_16x16.png"), 2);
		this.imageCollection1.Images.SetKeyName(2, "summary_16x16.png");
		resources.ApplyResources(this.gcItemCode, "gcItemCode");
		this.gcItemCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.SellItem;
		this.gcItemCode.ColumnEdit = this.rtxtItemcode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		resources.ApplyResources(this.rtxtItemcode, "rtxtItemcode");
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rtxtItemcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtItemcode.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtItemcode.Buttons1"), resources.GetString("rtxtItemcode.Buttons2"), (int)resources.GetObject("rtxtItemcode.Buttons3"), (bool)resources.GetObject("rtxtItemcode.Buttons4"), (bool)resources.GetObject("rtxtItemcode.Buttons5"), (bool)resources.GetObject("rtxtItemcode.Buttons6"), editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, resources.GetString("rtxtItemcode.Buttons7"), resources.GetObject("rtxtItemcode.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rtxtItemcode.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rtxtItemcode.Buttons10"))
		});
		this.rtxtItemcode.Name = "rtxtItemcode";
		this.rtxtItemcode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rtxtItemcode_ButtonClick);
		resources.ApplyResources(this.gcItemName, "gcItemName");
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "Dscription";
		this.gcItemName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemName.UnitField = null;
		resources.ApplyResources(this.gcBPCatalog, "gcBPCatalog");
		this.gcBPCatalog.ColumnEdit = this.rBpCatalog;
		this.gcBPCatalog.CurrencyDateField = null;
		this.gcBPCatalog.CurrencyField = null;
		this.gcBPCatalog.CurrencyFixed = null;
		this.gcBPCatalog.CurrencyFixedRate = 0.0;
		this.gcBPCatalog.CurrencyRateField = null;
		this.gcBPCatalog.FieldName = "SubCatNum";
		this.gcBPCatalog.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBPCatalog.Name = "gcBPCatalog";
		this.gcBPCatalog.PrimaryKey = false;
		this.gcBPCatalog.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBPCatalog.UnitField = null;
		resources.ApplyResources(this.rBpCatalog, "rBpCatalog");
		this.rBpCatalog.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rBpCatalog.Buttons"))
		});
		this.rBpCatalog.Name = "rBpCatalog";
		this.rBpCatalog.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBpCatalog_ButtonClick);
		resources.ApplyResources(this.gcQuantity, "gcQuantity");
		this.gcQuantity.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit1, "repositoryItemCalcEdit1");
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		resources.ApplyResources(this.gcUnitPrice, "gcUnitPrice");
		this.gcUnitPrice.ColumnEdit = this.rPriceEdit;
		this.gcUnitPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcUnitPrice.CurrencyDateField = "DocDate";
		this.gcUnitPrice.CurrencyField = "Currency";
		this.gcUnitPrice.CurrencyFixed = null;
		this.gcUnitPrice.CurrencyFixedRate = 0.0;
		this.gcUnitPrice.CurrencyRateField = "Rate";
		this.gcUnitPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcUnitPrice.FieldName = "PriceBefDi";
		this.gcUnitPrice.Name = "gcUnitPrice";
		this.gcUnitPrice.PrimaryKey = false;
		this.gcUnitPrice.UnitField = null;
		resources.ApplyResources(this.rPriceEdit, "rPriceEdit");
		this.rPriceEdit.Name = "rPriceEdit";
		resources.ApplyResources(this.gcDisPer, "gcDisPer");
		this.gcDisPer.ColumnEdit = this.repositoryItemCalcEdit5;
		this.gcDisPer.CurrencyDateField = null;
		this.gcDisPer.CurrencyField = null;
		this.gcDisPer.CurrencyFixed = null;
		this.gcDisPer.CurrencyFixedRate = 0.0;
		this.gcDisPer.CurrencyRateField = null;
		this.gcDisPer.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDisPer.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDisPer.FieldName = "DiscPrcnt";
		this.gcDisPer.Name = "gcDisPer";
		this.gcDisPer.PrimaryKey = false;
		this.gcDisPer.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit5, "repositoryItemCalcEdit5");
		this.repositoryItemCalcEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit5.Buttons"))
		});
		this.repositoryItemCalcEdit5.Name = "repositoryItemCalcEdit5";
		resources.ApplyResources(this.gcVatGroup, "gcVatGroup");
		this.gcVatGroup.ColumnEdit = this.rLookUpVatGroup;
		this.gcVatGroup.CurrencyDateField = null;
		this.gcVatGroup.CurrencyField = null;
		this.gcVatGroup.CurrencyFixed = null;
		this.gcVatGroup.CurrencyFixedRate = 0.0;
		this.gcVatGroup.CurrencyRateField = null;
		this.gcVatGroup.FieldName = "VatGroup";
		this.gcVatGroup.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcVatGroup.Name = "gcVatGroup";
		this.gcVatGroup.PrimaryKey = false;
		this.gcVatGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcVatGroup.UnitField = null;
		resources.ApplyResources(this.rLookUpVatGroup, "rLookUpVatGroup");
		this.rLookUpVatGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLookUpVatGroup.Buttons"))
		});
		this.rLookUpVatGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLookUpVatGroup.Columns"), resources.GetString("rLookUpVatGroup.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLookUpVatGroup.Columns2"), resources.GetString("rLookUpVatGroup.Columns3")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLookUpVatGroup.Columns4"), resources.GetString("rLookUpVatGroup.Columns5"), (int)resources.GetObject("rLookUpVatGroup.Columns6"), (DevExpress.Utils.FormatType)resources.GetObject("rLookUpVatGroup.Columns7"), resources.GetString("rLookUpVatGroup.Columns8"), (bool)resources.GetObject("rLookUpVatGroup.Columns9"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rLookUpVatGroup.Columns10"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rLookUpVatGroup.Columns11"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rLookUpVatGroup.Columns12"))
		});
		this.rLookUpVatGroup.DisplayMember = "Code";
		this.rLookUpVatGroup.Name = "rLookUpVatGroup";
		this.rLookUpVatGroup.ValueMember = "Code";
		resources.ApplyResources(this.gridColumnEx17, "gridColumnEx17");
		this.gridColumnEx17.ColumnEdit = this.rPriceEdit;
		this.gridColumnEx17.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx17.CurrencyDateField = "DocDate";
		this.gridColumnEx17.CurrencyField = "Currency";
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = "Rate";
		this.gridColumnEx17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx17.FieldName = "PriceAfVAT";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		resources.ApplyResources(this.gcLineTaxOnly, "gcLineTaxOnly");
		this.gcLineTaxOnly.ColumnEdit = this.rEnSetCost;
		this.gcLineTaxOnly.CurrencyDateField = null;
		this.gcLineTaxOnly.CurrencyField = null;
		this.gcLineTaxOnly.CurrencyFixed = null;
		this.gcLineTaxOnly.CurrencyFixedRate = 0.0;
		this.gcLineTaxOnly.CurrencyRateField = null;
		this.gcLineTaxOnly.FieldName = "TaxOnly";
		this.gcLineTaxOnly.Name = "gcLineTaxOnly";
		this.gcLineTaxOnly.PrimaryKey = false;
		this.gcLineTaxOnly.UnitField = null;
		resources.ApplyResources(this.rEnSetCost, "rEnSetCost");
		this.rEnSetCost.Name = "rEnSetCost";
		this.rEnSetCost.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rEnSetCost.ValueChecked = "Y";
		this.rEnSetCost.ValueUnchecked = "N";
		resources.ApplyResources(this.gcTotalLine, "gcTotalLine");
		this.gcTotalLine.ColumnEdit = this.repositoryItemCalcEdit6;
		this.gcTotalLine.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcTotalLine.CurrencyDateField = null;
		this.gcTotalLine.CurrencyField = "";
		this.gcTotalLine.CurrencyFixed = null;
		this.gcTotalLine.CurrencyFixedRate = 0.0;
		this.gcTotalLine.CurrencyRateField = null;
		this.gcTotalLine.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcTotalLine.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcTotalLine.FieldName = "LineTotal";
		this.gcTotalLine.Name = "gcTotalLine";
		this.gcTotalLine.PrimaryKey = false;
		this.gcTotalLine.UnitField = null;
		resources.ApplyResources(this.repositoryItemCalcEdit6, "repositoryItemCalcEdit6");
		this.repositoryItemCalcEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit6.Buttons"))
		});
		this.repositoryItemCalcEdit6.Name = "repositoryItemCalcEdit6";
		resources.ApplyResources(this.gcWhseCode, "gcWhseCode");
		this.gcWhseCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcWhseCode.ColumnEdit = this.rtxtWhseCode;
		this.gcWhseCode.CurrencyDateField = null;
		this.gcWhseCode.CurrencyField = null;
		this.gcWhseCode.CurrencyFixed = null;
		this.gcWhseCode.CurrencyFixedRate = 0.0;
		this.gcWhseCode.CurrencyRateField = null;
		this.gcWhseCode.FieldName = "WhsCode";
		this.gcWhseCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcWhseCode.Name = "gcWhseCode";
		this.gcWhseCode.PrimaryKey = false;
		this.gcWhseCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWhseCode.UnitField = null;
		resources.ApplyResources(this.rtxtWhseCode, "rtxtWhseCode");
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rtxtWhseCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtWhseCode.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rtxtWhseCode.Buttons1"), resources.GetString("rtxtWhseCode.Buttons2"), (int)resources.GetObject("rtxtWhseCode.Buttons3"), (bool)resources.GetObject("rtxtWhseCode.Buttons4"), (bool)resources.GetObject("rtxtWhseCode.Buttons5"), (bool)resources.GetObject("rtxtWhseCode.Buttons6"), editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, resources.GetString("rtxtWhseCode.Buttons7"), resources.GetObject("rtxtWhseCode.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rtxtWhseCode.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rtxtWhseCode.Buttons10"))
		});
		this.rtxtWhseCode.Name = "rtxtWhseCode";
		this.rtxtWhseCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rtxtWhseCode_ButtonClick);
		resources.ApplyResources(this.gcLinePQTReqdate, "gcLinePQTReqdate");
		this.gcLinePQTReqdate.CurrencyDateField = null;
		this.gcLinePQTReqdate.CurrencyField = null;
		this.gcLinePQTReqdate.CurrencyFixed = null;
		this.gcLinePQTReqdate.CurrencyFixedRate = 0.0;
		this.gcLinePQTReqdate.CurrencyRateField = null;
		this.gcLinePQTReqdate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcLinePQTReqdate.FieldName = "PQTReqDate";
		this.gcLinePQTReqdate.Name = "gcLinePQTReqdate";
		this.gcLinePQTReqdate.PrimaryKey = false;
		this.gcLinePQTReqdate.UnitField = null;
		resources.ApplyResources(this.gridColumnEx10, "gridColumnEx10");
		this.gridColumnEx10.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx10.FieldName = "PQTReqQty";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx7.Buttons"))
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		resources.ApplyResources(this.gcLine_DelDate, "gcLine_DelDate");
		this.gcLine_DelDate.CurrencyDateField = null;
		this.gcLine_DelDate.CurrencyField = null;
		this.gcLine_DelDate.CurrencyFixed = null;
		this.gcLine_DelDate.CurrencyFixedRate = 0.0;
		this.gcLine_DelDate.CurrencyRateField = null;
		this.gcLine_DelDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcLine_DelDate.FieldName = "ShipDate";
		this.gcLine_DelDate.Name = "gcLine_DelDate";
		this.gcLine_DelDate.PrimaryKey = false;
		this.gcLine_DelDate.UnitField = null;
		resources.ApplyResources(this.gcRemarkLine, "gcRemarkLine");
		this.gcRemarkLine.CurrencyDateField = null;
		this.gcRemarkLine.CurrencyField = null;
		this.gcRemarkLine.CurrencyFixed = null;
		this.gcRemarkLine.CurrencyFixedRate = 0.0;
		this.gcRemarkLine.CurrencyRateField = null;
		this.gcRemarkLine.FieldName = "FreeTxt";
		this.gcRemarkLine.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcRemarkLine.Name = "gcRemarkLine";
		this.gcRemarkLine.PrimaryKey = false;
		this.gcRemarkLine.UnitField = null;
		resources.ApplyResources(this.gcOpenQty, "gcOpenQty");
		this.gcOpenQty.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gcOpenQty.CurrencyDateField = null;
		this.gcOpenQty.CurrencyField = null;
		this.gcOpenQty.CurrencyFixed = null;
		this.gcOpenQty.CurrencyFixedRate = 0.0;
		this.gcOpenQty.CurrencyRateField = null;
		this.gcOpenQty.DisplayFormat.FormatString = "n0";
		this.gcOpenQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcOpenQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcOpenQty.FieldName = "OpenQty";
		this.gcOpenQty.Name = "gcOpenQty";
		this.gcOpenQty.OptionsColumn.AllowEdit = false;
		this.gcOpenQty.PrimaryKey = false;
		this.gcOpenQty.UnitField = null;
		this.repositoryItemCalcEditEx8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx8.Buttons"))
		});
		this.repositoryItemCalcEditEx8.IsSearchControl = false;
		this.repositoryItemCalcEditEx8.Name = "repositoryItemCalcEditEx8";
		resources.ApplyResources(this.gcCureencyLine, "gcCureencyLine");
		this.gcCureencyLine.CurrencyDateField = null;
		this.gcCureencyLine.CurrencyField = null;
		this.gcCureencyLine.CurrencyFixed = null;
		this.gcCureencyLine.CurrencyFixedRate = 0.0;
		this.gcCureencyLine.CurrencyRateField = null;
		this.gcCureencyLine.FieldName = "Currency";
		this.gcCureencyLine.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcCureencyLine.Name = "gcCureencyLine";
		this.gcCureencyLine.PrimaryKey = false;
		this.gcCureencyLine.UnitField = null;
		resources.ApplyResources(this.gcVatPrcnt, "gcVatPrcnt");
		this.gcVatPrcnt.ColumnEdit = this.repositoryItemCalcEditEx9;
		this.gcVatPrcnt.CurrencyDateField = null;
		this.gcVatPrcnt.CurrencyField = null;
		this.gcVatPrcnt.CurrencyFixed = null;
		this.gcVatPrcnt.CurrencyFixedRate = 0.0;
		this.gcVatPrcnt.CurrencyRateField = null;
		this.gcVatPrcnt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcVatPrcnt.FieldName = "VatPrcnt";
		this.gcVatPrcnt.Name = "gcVatPrcnt";
		this.gcVatPrcnt.PrimaryKey = false;
		this.gcVatPrcnt.UnitField = null;
		this.repositoryItemCalcEditEx9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx9.Buttons"))
		});
		this.repositoryItemCalcEditEx9.IsSearchControl = false;
		this.repositoryItemCalcEditEx9.Name = "repositoryItemCalcEditEx9";
		resources.ApplyResources(this.gcLineNum, "gcLineNum");
		this.gcLineNum.CurrencyDateField = null;
		this.gcLineNum.CurrencyField = null;
		this.gcLineNum.CurrencyFixed = null;
		this.gcLineNum.CurrencyFixedRate = 0.0;
		this.gcLineNum.CurrencyRateField = null;
		this.gcLineNum.FieldName = "LineNum";
		this.gcLineNum.Name = "gcLineNum";
		this.gcLineNum.PrimaryKey = false;
		this.gcLineNum.UnitField = null;
		resources.ApplyResources(this.gridColumn7, "gridColumn7");
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "LineStatus";
		this.gridColumn7.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		resources.ApplyResources(this.gcLineVat, "gcLineVat");
		this.gcLineVat.ColumnEdit = this.repositoryItemCalcEditEx10;
		this.gcLineVat.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcLineVat.CurrencyDateField = null;
		this.gcLineVat.CurrencyField = null;
		this.gcLineVat.CurrencyFixed = null;
		this.gcLineVat.CurrencyFixedRate = 0.0;
		this.gcLineVat.CurrencyRateField = null;
		this.gcLineVat.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcLineVat.FieldName = "LineVat";
		this.gcLineVat.Name = "gcLineVat";
		this.gcLineVat.PrimaryKey = false;
		this.gcLineVat.UnitField = null;
		this.repositoryItemCalcEditEx10.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx10.Buttons"))
		});
		this.repositoryItemCalcEditEx10.IsSearchControl = false;
		this.repositoryItemCalcEditEx10.Name = "repositoryItemCalcEditEx10";
		resources.ApplyResources(this.gcLineTotalFC, "gcLineTotalFC");
		this.gcLineTotalFC.CurrencyDateField = null;
		this.gcLineTotalFC.CurrencyField = null;
		this.gcLineTotalFC.CurrencyFixed = null;
		this.gcLineTotalFC.CurrencyFixedRate = 0.0;
		this.gcLineTotalFC.CurrencyRateField = null;
		this.gcLineTotalFC.FieldName = "TotalFrgn";
		this.gcLineTotalFC.Name = "gcLineTotalFC";
		this.gcLineTotalFC.PrimaryKey = false;
		this.gcLineTotalFC.UnitField = null;
		resources.ApplyResources(this.gcLineVatSumFrgn, "gcLineVatSumFrgn");
		this.gcLineVatSumFrgn.CurrencyDateField = null;
		this.gcLineVatSumFrgn.CurrencyField = null;
		this.gcLineVatSumFrgn.CurrencyFixed = null;
		this.gcLineVatSumFrgn.CurrencyFixedRate = 0.0;
		this.gcLineVatSumFrgn.CurrencyRateField = null;
		this.gcLineVatSumFrgn.FieldName = "VatSumFrgn";
		this.gcLineVatSumFrgn.Name = "gcLineVatSumFrgn";
		this.gcLineVatSumFrgn.PrimaryKey = false;
		this.gcLineVatSumFrgn.UnitField = null;
		resources.ApplyResources(this.gridColumnEx18, "gridColumnEx18");
		this.gridColumnEx18.ColumnEdit = this.repositoryItemCalcEditEx12;
		this.gridColumnEx18.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx18.FieldName = "GTotal";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.repositoryItemCalcEditEx12.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx12.Buttons"))
		});
		this.repositoryItemCalcEditEx12.IsSearchControl = false;
		this.repositoryItemCalcEditEx12.Name = "repositoryItemCalcEditEx12";
		resources.ApplyResources(this.gridColumnEx19, "gridColumnEx19");
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEditEx23;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx19.FieldName = "GTotalFC";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.repositoryItemCalcEditEx23.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx23.Buttons"))
		});
		this.repositoryItemCalcEditEx23.IsSearchControl = false;
		this.repositoryItemCalcEditEx23.Name = "repositoryItemCalcEditEx23";
		resources.ApplyResources(this.gcBOMTreeType, "gcBOMTreeType");
		this.gcBOMTreeType.CurrencyDateField = null;
		this.gcBOMTreeType.CurrencyField = null;
		this.gcBOMTreeType.CurrencyFixed = null;
		this.gcBOMTreeType.CurrencyFixedRate = 0.0;
		this.gcBOMTreeType.CurrencyRateField = null;
		this.gcBOMTreeType.FieldName = "BOMTreeType";
		this.gcBOMTreeType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBOMTreeType.Name = "gcBOMTreeType";
		this.gcBOMTreeType.PrimaryKey = false;
		this.gcBOMTreeType.UnitField = null;
		resources.ApplyResources(this.gcTrgetEntry, "gcTrgetEntry");
		this.gcTrgetEntry.CurrencyDateField = null;
		this.gcTrgetEntry.CurrencyField = null;
		this.gcTrgetEntry.CurrencyFixed = null;
		this.gcTrgetEntry.CurrencyFixedRate = 0.0;
		this.gcTrgetEntry.CurrencyRateField = null;
		this.gcTrgetEntry.FieldName = "TrgetEntry";
		this.gcTrgetEntry.Name = "gcTrgetEntry";
		this.gcTrgetEntry.PrimaryKey = false;
		this.gcTrgetEntry.UnitField = null;
		resources.ApplyResources(this.gcBaseType, "gcBaseType");
		this.gcBaseType.ColumnEdit = this.rep_BaseType;
		this.gcBaseType.CurrencyDateField = null;
		this.gcBaseType.CurrencyField = null;
		this.gcBaseType.CurrencyFixed = null;
		this.gcBaseType.CurrencyFixedRate = 0.0;
		this.gcBaseType.CurrencyRateField = null;
		this.gcBaseType.FieldName = "BaseType";
		this.gcBaseType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBaseType.Name = "gcBaseType";
		this.gcBaseType.PrimaryKey = false;
		this.gcBaseType.UnitField = null;
		this.rep_BaseType.Name = "rep_BaseType";
		resources.ApplyResources(this.gcBaseRef, "gcBaseRef");
		this.gcBaseRef.CurrencyDateField = null;
		this.gcBaseRef.CurrencyField = null;
		this.gcBaseRef.CurrencyFixed = null;
		this.gcBaseRef.CurrencyFixedRate = 0.0;
		this.gcBaseRef.CurrencyRateField = null;
		this.gcBaseRef.FieldName = "BaseRef";
		this.gcBaseRef.Name = "gcBaseRef";
		this.gcBaseRef.PrimaryKey = false;
		this.gcBaseRef.UnitField = null;
		resources.ApplyResources(this.gcTargetType, "gcTargetType");
		this.gcTargetType.CurrencyDateField = null;
		this.gcTargetType.CurrencyField = null;
		this.gcTargetType.CurrencyFixed = null;
		this.gcTargetType.CurrencyFixedRate = 0.0;
		this.gcTargetType.CurrencyRateField = null;
		this.gcTargetType.FieldName = "TargetType";
		this.gcTargetType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcTargetType.Name = "gcTargetType";
		this.gcTargetType.PrimaryKey = false;
		this.gcTargetType.UnitField = null;
		resources.ApplyResources(this.gcBaseEntry, "gcBaseEntry");
		this.gcBaseEntry.CurrencyDateField = null;
		this.gcBaseEntry.CurrencyField = null;
		this.gcBaseEntry.CurrencyFixed = null;
		this.gcBaseEntry.CurrencyFixedRate = 0.0;
		this.gcBaseEntry.CurrencyRateField = null;
		this.gcBaseEntry.FieldName = "BaseEntry";
		this.gcBaseEntry.Name = "gcBaseEntry";
		this.gcBaseEntry.PrimaryKey = false;
		this.gcBaseEntry.UnitField = null;
		resources.ApplyResources(this.gcBaseLine, "gcBaseLine");
		this.gcBaseLine.CurrencyDateField = null;
		this.gcBaseLine.CurrencyField = null;
		this.gcBaseLine.CurrencyFixed = null;
		this.gcBaseLine.CurrencyFixedRate = 0.0;
		this.gcBaseLine.CurrencyRateField = null;
		this.gcBaseLine.FieldName = "BaseLine";
		this.gcBaseLine.Name = "gcBaseLine";
		this.gcBaseLine.PrimaryKey = false;
		this.gcBaseLine.UnitField = null;
		resources.ApplyResources(this.gcLineTreeType, "gcLineTreeType");
		this.gcLineTreeType.CurrencyDateField = null;
		this.gcLineTreeType.CurrencyField = null;
		this.gcLineTreeType.CurrencyFixed = null;
		this.gcLineTreeType.CurrencyFixedRate = 0.0;
		this.gcLineTreeType.CurrencyRateField = null;
		this.gcLineTreeType.FieldName = "TreeType";
		this.gcLineTreeType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineTreeType.Name = "gcLineTreeType";
		this.gcLineTreeType.PrimaryKey = false;
		this.gcLineTreeType.UnitField = null;
		resources.ApplyResources(this.gcDeliveryQty, "gcDeliveryQty");
		this.gcDeliveryQty.ColumnEdit = this.repositoryItemCalcEditEx11;
		this.gcDeliveryQty.CurrencyDateField = null;
		this.gcDeliveryQty.CurrencyField = null;
		this.gcDeliveryQty.CurrencyFixed = null;
		this.gcDeliveryQty.CurrencyFixedRate = 0.0;
		this.gcDeliveryQty.CurrencyRateField = null;
		this.gcDeliveryQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcDeliveryQty.FieldName = "DelivrdQty";
		this.gcDeliveryQty.Name = "gcDeliveryQty";
		this.gcDeliveryQty.PrimaryKey = false;
		this.gcDeliveryQty.UnitField = null;
		this.repositoryItemCalcEditEx11.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx11.Buttons"))
		});
		this.repositoryItemCalcEditEx11.IsSearchControl = false;
		this.repositoryItemCalcEditEx11.Name = "repositoryItemCalcEditEx11";
		resources.ApplyResources(this.gcLineCtlAccount, "gcLineCtlAccount");
		this.gcLineCtlAccount.CflType = Apzon.Commons.Enums.Enums.TypeCFL.AcctCode;
		this.gcLineCtlAccount.ColumnEdit = this.rep_bte_CtlAccount;
		this.gcLineCtlAccount.CurrencyDateField = null;
		this.gcLineCtlAccount.CurrencyField = null;
		this.gcLineCtlAccount.CurrencyFixed = null;
		this.gcLineCtlAccount.CurrencyFixedRate = 0.0;
		this.gcLineCtlAccount.CurrencyRateField = null;
		this.gcLineCtlAccount.FieldName = "AcctCode";
		this.gcLineCtlAccount.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineCtlAccount.Name = "gcLineCtlAccount";
		this.gcLineCtlAccount.PrimaryKey = false;
		this.gcLineCtlAccount.UnitField = null;
		resources.ApplyResources(this.rep_bte_CtlAccount, "rep_bte_CtlAccount");
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_bte_CtlAccount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_bte_CtlAccount.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_bte_CtlAccount.Buttons1"), resources.GetString("rep_bte_CtlAccount.Buttons2"), (int)resources.GetObject("rep_bte_CtlAccount.Buttons3"), (bool)resources.GetObject("rep_bte_CtlAccount.Buttons4"), (bool)resources.GetObject("rep_bte_CtlAccount.Buttons5"), (bool)resources.GetObject("rep_bte_CtlAccount.Buttons6"), editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, resources.GetString("rep_bte_CtlAccount.Buttons7"), resources.GetObject("rep_bte_CtlAccount.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rep_bte_CtlAccount.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rep_bte_CtlAccount.Buttons10"))
		});
		this.rep_bte_CtlAccount.Name = "rep_bte_CtlAccount";
		this.rep_bte_CtlAccount.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_CtlAccount_ButtonPressed);
		resources.ApplyResources(this.gcLinePackQty, "gcLinePackQty");
		this.gcLinePackQty.ColumnEdit = this.rLinePackQty;
		this.gcLinePackQty.CurrencyDateField = null;
		this.gcLinePackQty.CurrencyField = null;
		this.gcLinePackQty.CurrencyFixed = null;
		this.gcLinePackQty.CurrencyFixedRate = 0.0;
		this.gcLinePackQty.CurrencyRateField = null;
		this.gcLinePackQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLinePackQty.FieldName = "PackQty";
		this.gcLinePackQty.Name = "gcLinePackQty";
		this.gcLinePackQty.PrimaryKey = false;
		this.gcLinePackQty.UnitField = null;
		this.rLinePackQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLinePackQty.Buttons"))
		});
		this.rLinePackQty.IsSearchControl = false;
		this.rLinePackQty.Name = "rLinePackQty";
		resources.ApplyResources(this.gridColumn19, "gridColumn19");
		this.gridColumn19.ColumnEdit = this.rep_FreightCode;
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "ExpnsCode_F1";
		this.gridColumn19.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		resources.ApplyResources(this.rep_FreightCode, "rep_FreightCode");
		this.rep_FreightCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_FreightCode.Buttons"))
		});
		this.rep_FreightCode.DisplayMember = "Name";
		this.rep_FreightCode.Name = "rep_FreightCode";
		this.rep_FreightCode.ValueMember = "Code";
		resources.ApplyResources(this.gridColumn20, "gridColumn20");
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "LineTotal_F1";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		resources.ApplyResources(this.gridColumn21, "gridColumn21");
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "VatSum_F1";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		resources.ApplyResources(this.gridColumn22, "gridColumn22");
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "TotalFrgn_F1";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		resources.ApplyResources(this.gridColumn23, "gridColumn23");
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "VatSumFrgn_F1";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		resources.ApplyResources(this.gridColumn24, "gridColumn24");
		this.gridColumn24.ColumnEdit = this.rLookUpVatGroup;
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "VatGroup_F1";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		resources.ApplyResources(this.gridColumn25, "gridColumn25");
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "VatPrcnt_F1";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		resources.ApplyResources(this.gridColumn26, "gridColumn26");
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "IsAcquistn_F1";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		resources.ApplyResources(this.gridColumn27, "gridColumn27");
		this.gridColumn27.ColumnEdit = this.rep_LstPrice;
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "LstPchPrce_F1";
		this.gridColumn27.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		resources.ApplyResources(this.rep_LstPrice, "rep_LstPrice");
		this.rep_LstPrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LstPrice.Buttons"))
		});
		this.rep_LstPrice.Name = "rep_LstPrice";
		resources.ApplyResources(this.gridColumn28, "gridColumn28");
		this.gridColumn28.ColumnEdit = this.rep_FreightStock;
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "Stock_F1";
		this.gridColumn28.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		resources.ApplyResources(this.rep_FreightStock, "rep_FreightStock");
		this.rep_FreightStock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_FreightStock.Buttons"))
		});
		this.rep_FreightStock.DisplayMember = "Name";
		this.rep_FreightStock.Name = "rep_FreightStock";
		this.rep_FreightStock.ValueMember = "Code";
		resources.ApplyResources(this.gridColumn29, "gridColumn29");
		this.gridColumn29.ColumnEdit = this.rep_FreightCode;
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "ExpnsCode_F2";
		this.gridColumn29.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		resources.ApplyResources(this.gridColumn30, "gridColumn30");
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "LineTotal_F2";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		resources.ApplyResources(this.gridColumn31, "gridColumn31");
		this.gridColumn31.CurrencyDateField = null;
		this.gridColumn31.CurrencyField = null;
		this.gridColumn31.CurrencyFixed = null;
		this.gridColumn31.CurrencyFixedRate = 0.0;
		this.gridColumn31.CurrencyRateField = null;
		this.gridColumn31.FieldName = "VatSum_F2";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.PrimaryKey = false;
		this.gridColumn31.UnitField = null;
		resources.ApplyResources(this.gridColumn32, "gridColumn32");
		this.gridColumn32.CurrencyDateField = null;
		this.gridColumn32.CurrencyField = null;
		this.gridColumn32.CurrencyFixed = null;
		this.gridColumn32.CurrencyFixedRate = 0.0;
		this.gridColumn32.CurrencyRateField = null;
		this.gridColumn32.FieldName = "TotalFrgn_F2";
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.PrimaryKey = false;
		this.gridColumn32.UnitField = null;
		resources.ApplyResources(this.gridColumn33, "gridColumn33");
		this.gridColumn33.CurrencyDateField = null;
		this.gridColumn33.CurrencyField = null;
		this.gridColumn33.CurrencyFixed = null;
		this.gridColumn33.CurrencyFixedRate = 0.0;
		this.gridColumn33.CurrencyRateField = null;
		this.gridColumn33.FieldName = "VatSumFrgn_F2";
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.PrimaryKey = false;
		this.gridColumn33.UnitField = null;
		resources.ApplyResources(this.gridColumn34, "gridColumn34");
		this.gridColumn34.ColumnEdit = this.rLookUpVatGroup;
		this.gridColumn34.CurrencyDateField = null;
		this.gridColumn34.CurrencyField = null;
		this.gridColumn34.CurrencyFixed = null;
		this.gridColumn34.CurrencyFixedRate = 0.0;
		this.gridColumn34.CurrencyRateField = null;
		this.gridColumn34.FieldName = "VatGroup_F2";
		this.gridColumn34.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.PrimaryKey = false;
		this.gridColumn34.UnitField = null;
		resources.ApplyResources(this.gridColumn35, "gridColumn35");
		this.gridColumn35.CurrencyDateField = null;
		this.gridColumn35.CurrencyField = null;
		this.gridColumn35.CurrencyFixed = null;
		this.gridColumn35.CurrencyFixedRate = 0.0;
		this.gridColumn35.CurrencyRateField = null;
		this.gridColumn35.FieldName = "VatPrcnt_F2";
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.PrimaryKey = false;
		this.gridColumn35.UnitField = null;
		resources.ApplyResources(this.gridColumn36, "gridColumn36");
		this.gridColumn36.CurrencyDateField = null;
		this.gridColumn36.CurrencyField = null;
		this.gridColumn36.CurrencyFixed = null;
		this.gridColumn36.CurrencyFixedRate = 0.0;
		this.gridColumn36.CurrencyRateField = null;
		this.gridColumn36.FieldName = "IsAcquistn_F2";
		this.gridColumn36.Name = "gridColumn36";
		this.gridColumn36.PrimaryKey = false;
		this.gridColumn36.UnitField = null;
		resources.ApplyResources(this.gridColumn37, "gridColumn37");
		this.gridColumn37.CurrencyDateField = null;
		this.gridColumn37.CurrencyField = null;
		this.gridColumn37.CurrencyFixed = null;
		this.gridColumn37.CurrencyFixedRate = 0.0;
		this.gridColumn37.CurrencyRateField = null;
		this.gridColumn37.FieldName = "LstPchPrce_F2";
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.PrimaryKey = false;
		this.gridColumn37.UnitField = null;
		resources.ApplyResources(this.gridColumn38, "gridColumn38");
		this.gridColumn38.ColumnEdit = this.rep_FreightStock;
		this.gridColumn38.CurrencyDateField = null;
		this.gridColumn38.CurrencyField = null;
		this.gridColumn38.CurrencyFixed = null;
		this.gridColumn38.CurrencyFixedRate = 0.0;
		this.gridColumn38.CurrencyRateField = null;
		this.gridColumn38.FieldName = "Stock_F2";
		this.gridColumn38.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn38.Name = "gridColumn38";
		this.gridColumn38.PrimaryKey = false;
		this.gridColumn38.UnitField = null;
		resources.ApplyResources(this.gridColumn39, "gridColumn39");
		this.gridColumn39.ColumnEdit = this.rep_FreightCode;
		this.gridColumn39.CurrencyDateField = null;
		this.gridColumn39.CurrencyField = null;
		this.gridColumn39.CurrencyFixed = null;
		this.gridColumn39.CurrencyFixedRate = 0.0;
		this.gridColumn39.CurrencyRateField = null;
		this.gridColumn39.FieldName = "ExpnsCode_F3";
		this.gridColumn39.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn39.Name = "gridColumn39";
		this.gridColumn39.PrimaryKey = false;
		this.gridColumn39.UnitField = null;
		resources.ApplyResources(this.gridColumn40, "gridColumn40");
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "LineTotal_F3";
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.UnitField = null;
		resources.ApplyResources(this.gridColumn41, "gridColumn41");
		this.gridColumn41.CurrencyDateField = null;
		this.gridColumn41.CurrencyField = null;
		this.gridColumn41.CurrencyFixed = null;
		this.gridColumn41.CurrencyFixedRate = 0.0;
		this.gridColumn41.CurrencyRateField = null;
		this.gridColumn41.FieldName = "VatSum_F3";
		this.gridColumn41.Name = "gridColumn41";
		this.gridColumn41.PrimaryKey = false;
		this.gridColumn41.UnitField = null;
		resources.ApplyResources(this.gridColumn42, "gridColumn42");
		this.gridColumn42.CurrencyDateField = null;
		this.gridColumn42.CurrencyField = null;
		this.gridColumn42.CurrencyFixed = null;
		this.gridColumn42.CurrencyFixedRate = 0.0;
		this.gridColumn42.CurrencyRateField = null;
		this.gridColumn42.FieldName = "TotalFrgn_F3";
		this.gridColumn42.Name = "gridColumn42";
		this.gridColumn42.PrimaryKey = false;
		this.gridColumn42.UnitField = null;
		resources.ApplyResources(this.gridColumn43, "gridColumn43");
		this.gridColumn43.CurrencyDateField = null;
		this.gridColumn43.CurrencyField = null;
		this.gridColumn43.CurrencyFixed = null;
		this.gridColumn43.CurrencyFixedRate = 0.0;
		this.gridColumn43.CurrencyRateField = null;
		this.gridColumn43.FieldName = "VatSumFrgn_F3";
		this.gridColumn43.Name = "gridColumn43";
		this.gridColumn43.PrimaryKey = false;
		this.gridColumn43.UnitField = null;
		resources.ApplyResources(this.gridColumn44, "gridColumn44");
		this.gridColumn44.ColumnEdit = this.rLookUpVatGroup;
		this.gridColumn44.CurrencyDateField = null;
		this.gridColumn44.CurrencyField = null;
		this.gridColumn44.CurrencyFixed = null;
		this.gridColumn44.CurrencyFixedRate = 0.0;
		this.gridColumn44.CurrencyRateField = null;
		this.gridColumn44.FieldName = "VatGroup_F3";
		this.gridColumn44.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn44.Name = "gridColumn44";
		this.gridColumn44.PrimaryKey = false;
		this.gridColumn44.UnitField = null;
		resources.ApplyResources(this.gridColumn45, "gridColumn45");
		this.gridColumn45.CurrencyDateField = null;
		this.gridColumn45.CurrencyField = null;
		this.gridColumn45.CurrencyFixed = null;
		this.gridColumn45.CurrencyFixedRate = 0.0;
		this.gridColumn45.CurrencyRateField = null;
		this.gridColumn45.FieldName = "VatPrcnt_F3";
		this.gridColumn45.Name = "gridColumn45";
		this.gridColumn45.PrimaryKey = false;
		this.gridColumn45.UnitField = null;
		resources.ApplyResources(this.gridColumn46, "gridColumn46");
		this.gridColumn46.CurrencyDateField = null;
		this.gridColumn46.CurrencyField = null;
		this.gridColumn46.CurrencyFixed = null;
		this.gridColumn46.CurrencyFixedRate = 0.0;
		this.gridColumn46.CurrencyRateField = null;
		this.gridColumn46.FieldName = "IsAcquistn_F3";
		this.gridColumn46.Name = "gridColumn46";
		this.gridColumn46.PrimaryKey = false;
		this.gridColumn46.UnitField = null;
		resources.ApplyResources(this.gridColumn47, "gridColumn47");
		this.gridColumn47.CurrencyDateField = null;
		this.gridColumn47.CurrencyField = null;
		this.gridColumn47.CurrencyFixed = null;
		this.gridColumn47.CurrencyFixedRate = 0.0;
		this.gridColumn47.CurrencyRateField = null;
		this.gridColumn47.FieldName = "LstPchPrce_F3";
		this.gridColumn47.Name = "gridColumn47";
		this.gridColumn47.PrimaryKey = false;
		this.gridColumn47.UnitField = null;
		resources.ApplyResources(this.gridColumn48, "gridColumn48");
		this.gridColumn48.ColumnEdit = this.rep_FreightStock;
		this.gridColumn48.CurrencyDateField = null;
		this.gridColumn48.CurrencyField = null;
		this.gridColumn48.CurrencyFixed = null;
		this.gridColumn48.CurrencyFixedRate = 0.0;
		this.gridColumn48.CurrencyRateField = null;
		this.gridColumn48.FieldName = "Stock_F3";
		this.gridColumn48.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn48.Name = "gridColumn48";
		this.gridColumn48.PrimaryKey = false;
		this.gridColumn48.UnitField = null;
		resources.ApplyResources(this.gcLineUomGroup, "gcLineUomGroup");
		this.gcLineUomGroup.ColumnEdit = this.rLineUomGroup;
		this.gcLineUomGroup.CurrencyDateField = null;
		this.gcLineUomGroup.CurrencyField = null;
		this.gcLineUomGroup.CurrencyFixed = null;
		this.gcLineUomGroup.CurrencyFixedRate = 0.0;
		this.gcLineUomGroup.CurrencyRateField = null;
		this.gcLineUomGroup.FieldName = "UgpCode";
		this.gcLineUomGroup.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUomGroup.Name = "gcLineUomGroup";
		this.gcLineUomGroup.OptionsColumn.AllowEdit = false;
		this.gcLineUomGroup.PrimaryKey = false;
		this.gcLineUomGroup.UnitField = null;
		resources.ApplyResources(this.rLineUomGroup, "rLineUomGroup");
		this.rLineUomGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLineUomGroup.Buttons"))
		});
		this.rLineUomGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineUomGroup.Columns"), resources.GetString("rLineUomGroup.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineUomGroup.Columns2"), resources.GetString("rLineUomGroup.Columns3"), (int)resources.GetObject("rLineUomGroup.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rLineUomGroup.Columns5"), resources.GetString("rLineUomGroup.Columns6"), (bool)resources.GetObject("rLineUomGroup.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rLineUomGroup.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rLineUomGroup.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rLineUomGroup.Columns10"))
		});
		this.rLineUomGroup.DisplayMember = "Name";
		this.rLineUomGroup.Name = "rLineUomGroup";
		this.rLineUomGroup.ShowFooter = false;
		this.rLineUomGroup.ShowHeader = false;
		this.rLineUomGroup.ValueMember = "Code";
		resources.ApplyResources(this.gcLineUomCode, "gcLineUomCode");
		this.gcLineUomCode.ColumnEdit = this.rLineUomCode;
		this.gcLineUomCode.CurrencyDateField = null;
		this.gcLineUomCode.CurrencyField = null;
		this.gcLineUomCode.CurrencyFixed = null;
		this.gcLineUomCode.CurrencyFixedRate = 0.0;
		this.gcLineUomCode.CurrencyRateField = null;
		this.gcLineUomCode.FieldName = "UomCode";
		this.gcLineUomCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUomCode.Name = "gcLineUomCode";
		this.gcLineUomCode.PrimaryKey = false;
		this.gcLineUomCode.UnitField = null;
		resources.ApplyResources(this.rLineUomCode, "rLineUomCode");
		this.rLineUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rLineUomCode.Name = "rLineUomCode";
		this.rLineUomCode.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rLineUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineUomCode_ButtonClick);
		resources.ApplyResources(this.gcLineUoMName, "gcLineUoMName");
		this.gcLineUoMName.CurrencyDateField = null;
		this.gcLineUoMName.CurrencyField = null;
		this.gcLineUoMName.CurrencyFixed = null;
		this.gcLineUoMName.CurrencyFixedRate = 0.0;
		this.gcLineUoMName.CurrencyRateField = null;
		this.gcLineUoMName.FieldName = "unitMsr";
		this.gcLineUoMName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUoMName.Name = "gcLineUoMName";
		this.gcLineUoMName.PrimaryKey = false;
		this.gcLineUoMName.UnitField = null;
		resources.ApplyResources(this.gc_Line_CogsOcrCo2, "gc_Line_CogsOcrCo2");
		this.gc_Line_CogsOcrCo2.CurrencyDateField = null;
		this.gc_Line_CogsOcrCo2.CurrencyField = null;
		this.gc_Line_CogsOcrCo2.CurrencyFixed = null;
		this.gc_Line_CogsOcrCo2.CurrencyFixedRate = 0.0;
		this.gc_Line_CogsOcrCo2.CurrencyRateField = null;
		this.gc_Line_CogsOcrCo2.FieldName = "CogsOcrCo2";
		this.gc_Line_CogsOcrCo2.Name = "gc_Line_CogsOcrCo2";
		this.gc_Line_CogsOcrCo2.PrimaryKey = false;
		this.gc_Line_CogsOcrCo2.UnitField = null;
		resources.ApplyResources(this.gcLineInventoryUom, "gcLineInventoryUom");
		this.gcLineInventoryUom.ColumnEdit = this.rLineUseBaseUnits;
		this.gcLineInventoryUom.CurrencyDateField = null;
		this.gcLineInventoryUom.CurrencyField = null;
		this.gcLineInventoryUom.CurrencyFixed = null;
		this.gcLineInventoryUom.CurrencyFixedRate = 0.0;
		this.gcLineInventoryUom.CurrencyRateField = null;
		this.gcLineInventoryUom.FieldName = "UseBaseUn";
		this.gcLineInventoryUom.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineInventoryUom.Name = "gcLineInventoryUom";
		this.gcLineInventoryUom.PrimaryKey = false;
		this.gcLineInventoryUom.UnitField = null;
		resources.ApplyResources(this.rLineUseBaseUnits, "rLineUseBaseUnits");
		this.rLineUseBaseUnits.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLineUseBaseUnits.Buttons"))
		});
		this.rLineUseBaseUnits.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineUseBaseUnits.Columns"), resources.GetString("rLineUseBaseUnits.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineUseBaseUnits.Columns2"), resources.GetString("rLineUseBaseUnits.Columns3"), (int)resources.GetObject("rLineUseBaseUnits.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rLineUseBaseUnits.Columns5"), resources.GetString("rLineUseBaseUnits.Columns6"), (bool)resources.GetObject("rLineUseBaseUnits.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rLineUseBaseUnits.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rLineUseBaseUnits.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rLineUseBaseUnits.Columns10"))
		});
		this.rLineUseBaseUnits.DisplayMember = "Name";
		this.rLineUseBaseUnits.Name = "rLineUseBaseUnits";
		this.rLineUseBaseUnits.ShowFooter = false;
		this.rLineUseBaseUnits.ShowHeader = false;
		this.rLineUseBaseUnits.ValueMember = "Code";
		resources.ApplyResources(this.gcLineItemPerUnit, "gcLineItemPerUnit");
		this.gcLineItemPerUnit.ColumnEdit = this.rep_UnitsOfMeasurment;
		this.gcLineItemPerUnit.CurrencyDateField = null;
		this.gcLineItemPerUnit.CurrencyField = null;
		this.gcLineItemPerUnit.CurrencyFixed = null;
		this.gcLineItemPerUnit.CurrencyFixedRate = 0.0;
		this.gcLineItemPerUnit.CurrencyRateField = null;
		this.gcLineItemPerUnit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineItemPerUnit.FieldName = "NumPerMsr";
		this.gcLineItemPerUnit.Name = "gcLineItemPerUnit";
		this.gcLineItemPerUnit.PrimaryKey = false;
		this.gcLineItemPerUnit.UnitField = null;
		resources.ApplyResources(this.rep_UnitsOfMeasurment, "rep_UnitsOfMeasurment");
		this.rep_UnitsOfMeasurment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_UnitsOfMeasurment.Buttons"))
		});
		this.rep_UnitsOfMeasurment.Name = "rep_UnitsOfMeasurment";
		resources.ApplyResources(this.gcLineIvenQty, "gcLineIvenQty");
		this.gcLineIvenQty.ColumnEdit = this.rep_InventoryQuantity;
		this.gcLineIvenQty.CurrencyDateField = null;
		this.gcLineIvenQty.CurrencyField = null;
		this.gcLineIvenQty.CurrencyFixed = null;
		this.gcLineIvenQty.CurrencyFixedRate = 0.0;
		this.gcLineIvenQty.CurrencyRateField = null;
		this.gcLineIvenQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineIvenQty.FieldName = "InvQty";
		this.gcLineIvenQty.Name = "gcLineIvenQty";
		this.gcLineIvenQty.PrimaryKey = false;
		this.gcLineIvenQty.UnitField = null;
		resources.ApplyResources(this.rep_InventoryQuantity, "rep_InventoryQuantity");
		this.rep_InventoryQuantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_InventoryQuantity.Buttons"))
		});
		this.rep_InventoryQuantity.Name = "rep_InventoryQuantity";
		resources.ApplyResources(this.gcLineOpenIvnQty, "gcLineOpenIvnQty");
		this.gcLineOpenIvnQty.ColumnEdit = this.repositoryItemCalcEditEx13;
		this.gcLineOpenIvnQty.CurrencyDateField = null;
		this.gcLineOpenIvnQty.CurrencyField = null;
		this.gcLineOpenIvnQty.CurrencyFixed = null;
		this.gcLineOpenIvnQty.CurrencyFixedRate = 0.0;
		this.gcLineOpenIvnQty.CurrencyRateField = null;
		this.gcLineOpenIvnQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOpenIvnQty.FieldName = "OpenInvQty";
		this.gcLineOpenIvnQty.Name = "gcLineOpenIvnQty";
		this.gcLineOpenIvnQty.PrimaryKey = false;
		this.gcLineOpenIvnQty.UnitField = null;
		this.repositoryItemCalcEditEx13.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx13.Buttons"))
		});
		this.repositoryItemCalcEditEx13.IsSearchControl = false;
		this.repositoryItemCalcEditEx13.Name = "repositoryItemCalcEditEx13";
		resources.ApplyResources(this.gc_line_QuantityOnStock, "gc_line_QuantityOnStock");
		this.gc_line_QuantityOnStock.ColumnEdit = this.repositoryItemCalcEditEx14;
		this.gc_line_QuantityOnStock.CurrencyDateField = null;
		this.gc_line_QuantityOnStock.CurrencyField = null;
		this.gc_line_QuantityOnStock.CurrencyFixed = null;
		this.gc_line_QuantityOnStock.CurrencyFixedRate = 0.0;
		this.gc_line_QuantityOnStock.CurrencyRateField = null;
		this.gc_line_QuantityOnStock.DisplayFormat.FormatString = "n3";
		this.gc_line_QuantityOnStock.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_line_QuantityOnStock.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_line_QuantityOnStock.FieldName = "QuantityOnStock";
		this.gc_line_QuantityOnStock.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_line_QuantityOnStock.Name = "gc_line_QuantityOnStock";
		this.gc_line_QuantityOnStock.PrimaryKey = false;
		this.gc_line_QuantityOnStock.UnitField = null;
		this.repositoryItemCalcEditEx14.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx14.Buttons"))
		});
		this.repositoryItemCalcEditEx14.IsSearchControl = false;
		this.repositoryItemCalcEditEx14.Name = "repositoryItemCalcEditEx14";
		resources.ApplyResources(this.gridColumnEx6, "gridColumnEx6");
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCalcEditEx15;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx6.FieldName = "IsCommited";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.repositoryItemCalcEditEx15.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx15.Buttons"))
		});
		this.repositoryItemCalcEditEx15.IsSearchControl = false;
		this.repositoryItemCalcEditEx15.Name = "repositoryItemCalcEditEx15";
		resources.ApplyResources(this.gridColumnEx8, "gridColumnEx8");
		this.gridColumnEx8.ColumnEdit = this.repositoryItemCalcEditEx16;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx8.FieldName = "OnOrder";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.repositoryItemCalcEditEx16.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx16.Buttons"))
		});
		this.repositoryItemCalcEditEx16.IsSearchControl = false;
		this.repositoryItemCalcEditEx16.Name = "repositoryItemCalcEditEx16";
		resources.ApplyResources(this.gc_line_QuantityOnWareHouse, "gc_line_QuantityOnWareHouse");
		this.gc_line_QuantityOnWareHouse.ColumnEdit = this.repositoryItemCalcEditEx17;
		this.gc_line_QuantityOnWareHouse.CurrencyDateField = null;
		this.gc_line_QuantityOnWareHouse.CurrencyField = null;
		this.gc_line_QuantityOnWareHouse.CurrencyFixed = null;
		this.gc_line_QuantityOnWareHouse.CurrencyFixedRate = 0.0;
		this.gc_line_QuantityOnWareHouse.CurrencyRateField = null;
		this.gc_line_QuantityOnWareHouse.DisplayFormat.FormatString = "n3";
		this.gc_line_QuantityOnWareHouse.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_line_QuantityOnWareHouse.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_line_QuantityOnWareHouse.FieldName = "QuantityOnWareHouse";
		this.gc_line_QuantityOnWareHouse.Name = "gc_line_QuantityOnWareHouse";
		this.gc_line_QuantityOnWareHouse.PrimaryKey = false;
		this.gc_line_QuantityOnWareHouse.UnitField = null;
		this.repositoryItemCalcEditEx17.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx17.Buttons"))
		});
		this.repositoryItemCalcEditEx17.IsSearchControl = false;
		this.repositoryItemCalcEditEx17.Name = "repositoryItemCalcEditEx17";
		resources.ApplyResources(this.gridColumnEx7, "gridColumnEx7");
		this.gridColumnEx7.ColumnEdit = this.repositoryItemCalcEditEx18;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx7.FieldName = "IsCommitedInWareHouse";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.repositoryItemCalcEditEx18.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx18.Buttons"))
		});
		this.repositoryItemCalcEditEx18.IsSearchControl = false;
		this.repositoryItemCalcEditEx18.Name = "repositoryItemCalcEditEx18";
		resources.ApplyResources(this.gridColumnEx9, "gridColumnEx9");
		this.gridColumnEx9.ColumnEdit = this.repositoryItemCalcEditEx19;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx9.FieldName = "OnOrderInWareHouse";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.repositoryItemCalcEditEx19.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx19.Buttons"))
		});
		this.repositoryItemCalcEditEx19.IsSearchControl = false;
		this.repositoryItemCalcEditEx19.Name = "repositoryItemCalcEditEx19";
		resources.ApplyResources(this.gridColumn57, "gridColumn57");
		this.gridColumn57.ColumnEdit = this.rPriceEdit;
		this.gridColumn57.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn57.CurrencyDateField = "DocDate";
		this.gridColumn57.CurrencyField = "Currency";
		this.gridColumn57.CurrencyFixed = null;
		this.gridColumn57.CurrencyFixedRate = 0.0;
		this.gridColumn57.CurrencyRateField = "Rate";
		this.gridColumn57.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn57.FieldName = "Price";
		this.gridColumn57.Name = "gridColumn57";
		this.gridColumn57.OptionsColumn.AllowEdit = false;
		this.gridColumn57.OptionsColumn.ReadOnly = true;
		this.gridColumn57.PrimaryKey = false;
		this.gridColumn57.UnboundExpression = "[Price]*(1-([DiscPrcnt]/100))";
		this.gridColumn57.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.gridColumn57.UnitField = null;
		resources.ApplyResources(this.gcLine_SlpCode, "gcLine_SlpCode");
		this.gcLine_SlpCode.ColumnEdit = this.rep_LineSlpCode;
		this.gcLine_SlpCode.CurrencyDateField = null;
		this.gcLine_SlpCode.CurrencyField = null;
		this.gcLine_SlpCode.CurrencyFixed = null;
		this.gcLine_SlpCode.CurrencyFixedRate = 0.0;
		this.gcLine_SlpCode.CurrencyRateField = null;
		this.gcLine_SlpCode.FieldName = "SlpCode";
		this.gcLine_SlpCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLine_SlpCode.Name = "gcLine_SlpCode";
		this.gcLine_SlpCode.PrimaryKey = false;
		this.gcLine_SlpCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLine_SlpCode.UnitField = null;
		resources.ApplyResources(this.rep_LineSlpCode, "rep_LineSlpCode");
		this.rep_LineSlpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LineSlpCode.Buttons"))
		});
		this.rep_LineSlpCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_LineSlpCode.Columns"), resources.GetString("rep_LineSlpCode.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_LineSlpCode.Columns2"), resources.GetString("rep_LineSlpCode.Columns3"))
		});
		this.rep_LineSlpCode.DisplayMember = "SlpName";
		this.rep_LineSlpCode.Name = "rep_LineSlpCode";
		this.rep_LineSlpCode.ValueMember = "SlpCode";
		resources.ApplyResources(this.gridColumnEx2, "gridColumnEx2");
		this.gridColumnEx2.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.gridColumnEx2.ColumnEdit = this.rep_LineProject;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Project";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx2.UnitField = null;
		resources.ApplyResources(this.rep_LineProject, "rep_LineProject");
		this.rep_LineProject.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_LineProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LineProject.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LineProject.Buttons1"), resources.GetString("rep_LineProject.Buttons2"), (int)resources.GetObject("rep_LineProject.Buttons3"), (bool)resources.GetObject("rep_LineProject.Buttons4"), (bool)resources.GetObject("rep_LineProject.Buttons5"), (bool)resources.GetObject("rep_LineProject.Buttons6"), editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, resources.GetString("rep_LineProject.Buttons7"), resources.GetObject("rep_LineProject.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rep_LineProject.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rep_LineProject.Buttons10"))
		});
		this.rep_LineProject.ContainerControl = this;
		this.rep_LineProject.DisplayMember = "PrjCode";
		this.rep_LineProject.DisplayMember2 = "";
		this.rep_LineProject.ListMandatoryField = null;
		this.rep_LineProject.LoadDataSourceWhenEnter = false;
		this.rep_LineProject.Name = "rep_LineProject";
		this.rep_LineProject.SeparateCharacterForMulti = ';';
		this.rep_LineProject.SeparateString = ", ";
		this.rep_LineProject.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rep_LineProject.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProjectCodes;
		this.rep_LineProject.UsingAutoCompleteBox = false;
		this.rep_LineProject.UsingDataSource = true;
		this.rep_LineProject.ValidateWhenNoDataSource = false;
		this.rep_LineProject.ValueMember = "PrjCode";
		resources.ApplyResources(this.gcLine_VendorNum, "gcLine_VendorNum");
		this.gcLine_VendorNum.ColumnEdit = this.rep_VendorNum;
		this.gcLine_VendorNum.CurrencyDateField = null;
		this.gcLine_VendorNum.CurrencyField = null;
		this.gcLine_VendorNum.CurrencyFixed = null;
		this.gcLine_VendorNum.CurrencyFixedRate = 0.0;
		this.gcLine_VendorNum.CurrencyRateField = null;
		this.gcLine_VendorNum.FieldName = "LineVendor";
		this.gcLine_VendorNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLine_VendorNum.Name = "gcLine_VendorNum";
		this.gcLine_VendorNum.PrimaryKey = false;
		this.gcLine_VendorNum.UnitField = null;
		resources.ApplyResources(this.rep_VendorNum, "rep_VendorNum");
		this.rep_VendorNum.AutoLoadDataSource = true;
		this.rep_VendorNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_VendorNum.Buttons")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_VendorNum.Buttons1"), resources.GetString("rep_VendorNum.Buttons2"), (int)resources.GetObject("rep_VendorNum.Buttons3"), (bool)resources.GetObject("rep_VendorNum.Buttons4"), (bool)resources.GetObject("rep_VendorNum.Buttons5"), (bool)resources.GetObject("rep_VendorNum.Buttons6"), imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, resources.GetString("rep_VendorNum.Buttons7"), resources.GetObject("rep_VendorNum.Buttons8"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rep_VendorNum.Buttons9"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rep_VendorNum.Buttons10"))
		});
		this.rep_VendorNum.ContainerControl = this;
		this.rep_VendorNum.DisplayMember = "CardCode";
		this.rep_VendorNum.DisplayMember2 = "";
		this.rep_VendorNum.ListMandatoryField = null;
		this.rep_VendorNum.LoadDataSourceWhenEnter = false;
		this.rep_VendorNum.Name = "rep_VendorNum";
		this.rep_VendorNum.SeparateCharacterForMulti = ';';
		this.rep_VendorNum.SeparateString = ", ";
		this.rep_VendorNum.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.rep_VendorNum.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_VendorNum.UsingAutoCompleteBox = false;
		this.rep_VendorNum.UsingDataSource = true;
		this.rep_VendorNum.ValidateWhenNoDataSource = false;
		this.rep_VendorNum.ValueMember = "CardCode";
		resources.ApplyResources(this.gridColumnEx11, "gridColumnEx11");
		this.gridColumnEx11.ColumnEdit = this.rep_NoInvtryMv;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "NoInvtryMv";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		resources.ApplyResources(this.rep_NoInvtryMv, "rep_NoInvtryMv");
		this.rep_NoInvtryMv.Name = "rep_NoInvtryMv";
		this.rep_NoInvtryMv.ValueChecked = "Y";
		this.rep_NoInvtryMv.ValueGrayed = "N";
		this.rep_NoInvtryMv.ValueUnchecked = "N";
		resources.ApplyResources(this.gridColumnEx12, "gridColumnEx12");
		this.gridColumnEx12.ColumnEdit = this.repositoryItemCalcEditEx20;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gridColumnEx12.FieldName = "Rate";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.repositoryItemCalcEditEx20.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx20.Buttons"))
		});
		this.repositoryItemCalcEditEx20.IsSearchControl = false;
		this.repositoryItemCalcEditEx20.Name = "repositoryItemCalcEditEx20";
		resources.ApplyResources(this.gcLineEnSetCost, "gcLineEnSetCost");
		this.gcLineEnSetCost.ColumnEdit = this.rEnSetCost;
		this.gcLineEnSetCost.CurrencyDateField = null;
		this.gcLineEnSetCost.CurrencyField = null;
		this.gcLineEnSetCost.CurrencyFixed = null;
		this.gcLineEnSetCost.CurrencyFixedRate = 0.0;
		this.gcLineEnSetCost.CurrencyRateField = null;
		this.gcLineEnSetCost.FieldName = "EnSetCost";
		this.gcLineEnSetCost.Name = "gcLineEnSetCost";
		this.gcLineEnSetCost.PrimaryKey = false;
		this.gcLineEnSetCost.UnitField = null;
		resources.ApplyResources(this.gcLineRetCost, "gcLineRetCost");
		this.gcLineRetCost.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcLineRetCost.CurrencyDateField = null;
		this.gcLineRetCost.CurrencyField = null;
		this.gcLineRetCost.CurrencyFixed = null;
		this.gcLineRetCost.CurrencyFixedRate = 0.0;
		this.gcLineRetCost.CurrencyRateField = null;
		this.gcLineRetCost.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcLineRetCost.FieldName = "RetCost";
		this.gcLineRetCost.Name = "gcLineRetCost";
		this.gcLineRetCost.PrimaryKey = false;
		this.gcLineRetCost.UnitField = null;
		resources.ApplyResources(this.gcAgrNo, "gcAgrNo");
		this.gcAgrNo.CurrencyDateField = null;
		this.gcAgrNo.CurrencyField = null;
		this.gcAgrNo.CurrencyFixed = null;
		this.gcAgrNo.CurrencyFixedRate = 0.0;
		this.gcAgrNo.CurrencyRateField = null;
		this.gcAgrNo.FieldName = "AgrNo";
		this.gcAgrNo.Name = "gcAgrNo";
		this.gcAgrNo.PrimaryKey = false;
		this.gcAgrNo.UnitField = null;
		resources.ApplyResources(this.gcAgrLnNum, "gcAgrLnNum");
		this.gcAgrLnNum.CurrencyDateField = null;
		this.gcAgrLnNum.CurrencyField = null;
		this.gcAgrLnNum.CurrencyFixed = null;
		this.gcAgrLnNum.CurrencyFixedRate = 0.0;
		this.gcAgrLnNum.CurrencyRateField = null;
		this.gcAgrLnNum.FieldName = "AgrLnNum";
		this.gcAgrLnNum.Name = "gcAgrLnNum";
		this.gcAgrLnNum.PrimaryKey = false;
		this.gcAgrLnNum.UnitField = null;
		resources.ApplyResources(this.gcLineSpecPrice, "gcLineSpecPrice");
		this.gcLineSpecPrice.ColumnEdit = this.rLineSpecPrice;
		this.gcLineSpecPrice.CurrencyDateField = null;
		this.gcLineSpecPrice.CurrencyField = null;
		this.gcLineSpecPrice.CurrencyFixed = null;
		this.gcLineSpecPrice.CurrencyFixedRate = 0.0;
		this.gcLineSpecPrice.CurrencyRateField = null;
		this.gcLineSpecPrice.FieldName = "SpecPrice";
		this.gcLineSpecPrice.Name = "gcLineSpecPrice";
		this.gcLineSpecPrice.PrimaryKey = false;
		this.gcLineSpecPrice.UnitField = null;
		resources.ApplyResources(this.rLineSpecPrice, "rLineSpecPrice");
		this.rLineSpecPrice.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rLineSpecPrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLineSpecPrice.Buttons"))
		});
		this.rLineSpecPrice.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineSpecPrice.Columns"), resources.GetString("rLineSpecPrice.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLineSpecPrice.Columns2"), resources.GetString("rLineSpecPrice.Columns3"), (int)resources.GetObject("rLineSpecPrice.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rLineSpecPrice.Columns5"), resources.GetString("rLineSpecPrice.Columns6"), (bool)resources.GetObject("rLineSpecPrice.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rLineSpecPrice.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rLineSpecPrice.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rLineSpecPrice.Columns10"))
		});
		this.rLineSpecPrice.DisplayMember = "Name";
		this.rLineSpecPrice.IsSearchControl = false;
		this.rLineSpecPrice.Name = "rLineSpecPrice";
		this.rLineSpecPrice.ShowFooter = false;
		this.rLineSpecPrice.ShowHeader = false;
		this.rLineSpecPrice.ValueMember = "Code";
		resources.ApplyResources(this.gc_CountryOrg, "gc_CountryOrg");
		this.gc_CountryOrg.ColumnEdit = this.rep_CountryOrg;
		this.gc_CountryOrg.CurrencyDateField = null;
		this.gc_CountryOrg.CurrencyField = null;
		this.gc_CountryOrg.CurrencyFixed = null;
		this.gc_CountryOrg.CurrencyFixedRate = 0.0;
		this.gc_CountryOrg.CurrencyRateField = null;
		this.gc_CountryOrg.FieldName = "CountryOrg";
		this.gc_CountryOrg.Name = "gc_CountryOrg";
		this.gc_CountryOrg.PrimaryKey = false;
		this.gc_CountryOrg.UnitField = null;
		resources.ApplyResources(this.rep_CountryOrg, "rep_CountryOrg");
		this.rep_CountryOrg.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_CountryOrg.Buttons"))
		});
		this.rep_CountryOrg.DisplayMember = "Name";
		this.rep_CountryOrg.Name = "rep_CountryOrg";
		this.rep_CountryOrg.ValueMember = "Code";
		resources.ApplyResources(this.gc_StdItemId, "gc_StdItemId");
		this.gc_StdItemId.CurrencyDateField = null;
		this.gc_StdItemId.CurrencyField = null;
		this.gc_StdItemId.CurrencyFixed = null;
		this.gc_StdItemId.CurrencyFixedRate = 0.0;
		this.gc_StdItemId.CurrencyRateField = null;
		this.gc_StdItemId.FieldName = "StdItemId";
		this.gc_StdItemId.Name = "gc_StdItemId";
		this.gc_StdItemId.PrimaryKey = false;
		this.gc_StdItemId.UnitField = null;
		this.gcUomEntry.CurrencyDateField = null;
		this.gcUomEntry.CurrencyField = null;
		this.gcUomEntry.CurrencyFixed = null;
		this.gcUomEntry.CurrencyFixedRate = 0.0;
		this.gcUomEntry.CurrencyRateField = null;
		this.gcUomEntry.FieldName = "UomEntry";
		this.gcUomEntry.Name = "gcUomEntry";
		this.gcUomEntry.PrimaryKey = false;
		this.gcUomEntry.UnitField = null;
		resources.ApplyResources(this.cbxTaxCode, "cbxTaxCode");
		this.cbxTaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("cbxTaxCode.Buttons"))
		});
		this.cbxTaxCode.Name = "cbxTaxCode";
		resources.ApplyResources(this.repositoryItemDateEdit1, "repositoryItemDateEdit1");
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit1.Buttons"))
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit1.CalendarTimeProperties.Buttons"))
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		resources.ApplyResources(this.rep_FreightTaxGroup, "rep_FreightTaxGroup");
		this.rep_FreightTaxGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_FreightTaxGroup.Buttons"))
		});
		this.rep_FreightTaxGroup.DisplayMember = "Code";
		this.rep_FreightTaxGroup.Name = "rep_FreightTaxGroup";
		this.rep_FreightTaxGroup.ValueMember = "Code";
		resources.ApplyResources(this.repositoryItemCalcEdit9, "repositoryItemCalcEdit9");
		this.repositoryItemCalcEdit9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit9.Buttons"))
		});
		this.repositoryItemCalcEdit9.Name = "repositoryItemCalcEdit9";
		this.tab_PaymentTerms.AllowTouchScroll = true;
		resources.ApplyResources(this.tab_PaymentTerms, "tab_PaymentTerms");
		this.tab_PaymentTerms.Controls.Add(this.cal_TotalFC);
		this.tab_PaymentTerms.Controls.Add(this.labelLine1);
		this.tab_PaymentTerms.Controls.Add(this.cal_Total);
		this.tab_PaymentTerms.Controls.Add(this.lb_Total);
		this.tab_PaymentTerms.Controls.Add(this.spin_Count);
		this.tab_PaymentTerms.Controls.Add(this.lb_Count);
		this.tab_PaymentTerms.Controls.Add(this.grdPaymentTerms);
		this.tab_PaymentTerms.EditControl = null;
		this.tab_PaymentTerms.EditValue = "Payment Terms";
		this.tab_PaymentTerms.FieldName = null;
		this.tab_PaymentTerms.IsSearchControl = false;
		this.tab_PaymentTerms.Name = "tab_PaymentTerms";
		this.tab_PaymentTerms.ReadOnly = false;
		this.tab_PaymentTerms.SettingInVisible = false;
		this.tab_PaymentTerms.SettingReadonly = false;
		this.tab_PaymentTerms.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.cal_TotalFC.EditControl = null;
		this.cal_TotalFC.FieldName = null;
		this.cal_TotalFC.IsSearchControl = false;
		resources.ApplyResources(this.cal_TotalFC, "cal_TotalFC");
		this.cal_TotalFC.MenuManager = this.barManager1;
		this.cal_TotalFC.Name = "cal_TotalFC";
		this.cal_TotalFC.Properties.IsSearchControl = false;
		this.cal_TotalFC.Properties.ReadOnly = true;
		this.cal_TotalFC.SettingInVisible = false;
		this.cal_TotalFC.SettingReadonly = false;
		this.cal_TotalFC.EditValueChanged += new System.EventHandler(this.cal_TotalFC_EditValueChanged);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.ctmCopyToArInvoice, this.ctmFromQuotation, this.barButtonItem1, this.bar_SaveAsDraft });
		this.barManager1.MaxItemId = 15;
		this.barDockControlTop.CausesValidation = false;
		resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlBottom.CausesValidation = false;
		resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlLeft.CausesValidation = false;
		resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlRight.CausesValidation = false;
		resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
		this.barDockControlRight.Manager = this.barManager1;
		resources.ApplyResources(this.ctmCopyToArInvoice, "ctmCopyToArInvoice");
		this.ctmCopyToArInvoice.Id = 1;
		this.ctmCopyToArInvoice.Name = "ctmCopyToArInvoice";
		resources.ApplyResources(this.ctmFromQuotation, "ctmFromQuotation");
		this.ctmFromQuotation.Id = 2;
		this.ctmFromQuotation.Name = "ctmFromQuotation";
		resources.ApplyResources(this.barButtonItem1, "barButtonItem1");
		this.barButtonItem1.Id = 3;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
		resources.ApplyResources(this.bar_SaveAsDraft, "bar_SaveAsDraft");
		this.bar_SaveAsDraft.Id = 4;
		this.bar_SaveAsDraft.Name = "bar_SaveAsDraft";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "TotalFC";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.labelLine1, "labelLine1");
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = false;
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.Utext = "TotalFC";
		this.cal_Total.EditControl = null;
		this.cal_Total.FieldName = null;
		this.cal_Total.IsSearchControl = false;
		resources.ApplyResources(this.cal_Total, "cal_Total");
		this.cal_Total.MenuManager = this.barManager1;
		this.cal_Total.Name = "cal_Total";
		this.cal_Total.Properties.IsSearchControl = false;
		this.cal_Total.Properties.ReadOnly = true;
		this.cal_Total.SettingInVisible = false;
		this.cal_Total.SettingReadonly = false;
		this.cal_Total.EditValueChanged += new System.EventHandler(this.cal_Total_EditValueChanged);
		this.lb_Total.EditControl = null;
		this.lb_Total.EditValue = "Total";
		this.lb_Total.FieldName = null;
		this.lb_Total.HideWhenEmpty = false;
		this.lb_Total.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_Total, "lb_Total");
		this.lb_Total.Name = "lb_Total";
		this.lb_Total.ReadOnly = false;
		this.lb_Total.SettingInVisible = false;
		this.lb_Total.SettingReadonly = false;
		this.lb_Total.ShowUnderLine = false;
		this.lb_Total.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Total.Utext = "Total";
		resources.ApplyResources(this.spin_Count, "spin_Count");
		this.spin_Count.MenuManager = this.barManager1;
		this.spin_Count.Name = "spin_Count";
		this.spin_Count.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("spin_Count.Properties.Buttons"))
		});
		this.spin_Count.EditValueChanged += new System.EventHandler(this.spin_Count_EditValueChanged);
		this.spin_Count.Validating += new System.ComponentModel.CancelEventHandler(this.spin_Count_Validating);
		this.lb_Count.EditControl = null;
		this.lb_Count.EditValue = "Count";
		this.lb_Count.FieldName = null;
		this.lb_Count.HideWhenEmpty = false;
		this.lb_Count.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_Count, "lb_Count");
		this.lb_Count.Name = "lb_Count";
		this.lb_Count.ReadOnly = false;
		this.lb_Count.SettingInVisible = false;
		this.lb_Count.SettingReadonly = false;
		this.lb_Count.ShowUnderLine = false;
		this.lb_Count.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Count.Utext = "Count";
		this.grdPaymentTerms.AllowPasteExcel = false;
		resources.ApplyResources(this.grdPaymentTerms, "grdPaymentTerms");
		this.grdPaymentTerms.ClipboardData = null;
		this.grdPaymentTerms.MainView = this.gvPayTerms;
		this.grdPaymentTerms.MenuManager = this.barManager1;
		this.grdPaymentTerms.Name = "grdPaymentTerms";
		this.grdPaymentTerms.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemDateEdit2, this.repositoryItemCalcEdit8 });
		this.grdPaymentTerms.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvPayTerms });
		this.gvPayTerms.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvPayTerms.ColumnKey = null;
		this.gvPayTerms.DisableFormSetting = false;
		this.gvPayTerms.DisplayException = false;
		this.gvPayTerms.FieldCurrency = null;
		this.gvPayTerms.FieldRate = null;
		this.gvPayTerms.FormId = "oDocument";
		this.gvPayTerms.GridControl = this.grdPaymentTerms;
		this.gvPayTerms.IndicatorWidth = 35;
		this.gvPayTerms.IsApzTable = false;
		this.gvPayTerms.IsInsertNewRow = false;
		this.gvPayTerms.IsLoadingData = false;
		this.gvPayTerms.ItemId = "OPMT";
		this.gvPayTerms.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gvPayTerms.LstColumnAllowBatch = null;
		this.gvPayTerms.LstColumnNotAllowBatch = null;
		this.gvPayTerms.LstColumnNotEditable = null;
		this.gvPayTerms.LstColumNotSetting = null;
		this.gvPayTerms.Name = "gvPayTerms";
		this.gvPayTerms.OptionsView.ColumnAutoWidth = false;
		this.gvPayTerms.OptionsView.ShowFooter = true;
		this.gvPayTerms.OptionsView.ShowGroupPanel = false;
		this.gvPayTerms.Tag = "@OPMT";
		this.gvPayTerms.UseBatchMod = false;
		resources.ApplyResources(this.gvPayTerms, "gvPayTerms");
		resources.ApplyResources(this.repositoryItemDateEdit2, "repositoryItemDateEdit2");
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit2.Buttons"))
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemDateEdit2.CalendarTimeProperties.Buttons"))
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		resources.ApplyResources(this.repositoryItemCalcEdit8, "repositoryItemCalcEdit8");
		this.repositoryItemCalcEdit8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEdit8.Buttons"))
		});
		this.repositoryItemCalcEdit8.Name = "repositoryItemCalcEdit8";
		this.tab_Logistic.Controls.Add(this.grdLogistic);
		this.tab_Logistic.EditControl = null;
		this.tab_Logistic.EditValue = "Logistic";
		this.tab_Logistic.FieldName = null;
		this.tab_Logistic.IsSearchControl = false;
		this.tab_Logistic.Name = "tab_Logistic";
		this.tab_Logistic.ReadOnly = false;
		this.tab_Logistic.SettingInVisible = false;
		this.tab_Logistic.SettingReadonly = false;
		resources.ApplyResources(this.tab_Logistic, "tab_Logistic");
		this.tab_Logistic.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.grdLogistic.AllowPasteExcel = false;
		this.grdLogistic.ClipboardData = null;
		resources.ApplyResources(this.grdLogistic, "grdLogistic");
		this.grdLogistic.MainView = this.gv_Logistic;
		this.grdLogistic.Name = "grdLogistic";
		this.grdLogistic.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rlpShipType, this.rShipTo, this.rBillTo, this.rep_Address2, this.rep_Address, this.rConfirmed, this.rLogisticsBpChannel, this.rLogisticBpChannelCntc, this.rep_PartSupply });
		this.grdLogistic.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Logistic });
		this.gv_Logistic.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Logistic.ColumnKey = null;
		this.gv_Logistic.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcShipType, this.gcShipTo, this.gridColumn9, this.gcBillTo, this.gcConfirmed, this.gridColumn8, this.gcLogistics_BPChCode, this.gcLogisticsBpChannelCntc, this.gridColumnEx21 });
		this.gv_Logistic.DisableFormSetting = false;
		this.gv_Logistic.DisplayException = false;
		this.gv_Logistic.FieldCurrency = null;
		this.gv_Logistic.FieldRate = null;
		this.gv_Logistic.FormId = "oDocument";
		this.gv_Logistic.GridControl = this.grdLogistic;
		this.gv_Logistic.IndicatorWidth = 35;
		this.gv_Logistic.IsApzTable = false;
		this.gv_Logistic.IsInsertNewRow = false;
		this.gv_Logistic.IsLoadingData = false;
		this.gv_Logistic.ItemId = "OLGT";
		this.gv_Logistic.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Logistic.LstColumnAllowBatch = null;
		this.gv_Logistic.LstColumnNotAllowBatch = null;
		this.gv_Logistic.LstColumnNotEditable = null;
		this.gv_Logistic.LstColumNotSetting = null;
		this.gv_Logistic.Name = "gv_Logistic";
		this.gv_Logistic.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Logistic.OptionsView.ShowGroupPanel = false;
		this.gv_Logistic.UseBatchMod = false;
		resources.ApplyResources(this.gv_Logistic, "gv_Logistic");
		this.gv_Logistic.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Logistic_CellValueChangedAsync);
		this.gv_Logistic.HiddenEditor += new System.EventHandler(this.gv_Logistic_HiddenEditor);
		this.gv_Logistic.ShownEditor += new System.EventHandler(this.gv_Logistic_ShownEditor);
		resources.ApplyResources(this.gcShipType, "gcShipType");
		this.gcShipType.ColumnEdit = this.rlpShipType;
		this.gcShipType.CurrencyDateField = null;
		this.gcShipType.CurrencyField = null;
		this.gcShipType.CurrencyFixed = null;
		this.gcShipType.CurrencyFixedRate = 0.0;
		this.gcShipType.CurrencyRateField = null;
		this.gcShipType.FieldName = "TrnspCode";
		this.gcShipType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcShipType.Name = "gcShipType";
		this.gcShipType.PrimaryKey = false;
		this.gcShipType.UnitField = null;
		resources.ApplyResources(this.rlpShipType, "rlpShipType");
		this.rlpShipType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rlpShipType.Buttons"))
		});
		this.rlpShipType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rlpShipType.Columns"), resources.GetString("rlpShipType.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rlpShipType.Columns2"), resources.GetString("rlpShipType.Columns3"), (int)resources.GetObject("rlpShipType.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rlpShipType.Columns5"), resources.GetString("rlpShipType.Columns6"), (bool)resources.GetObject("rlpShipType.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rlpShipType.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rlpShipType.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rlpShipType.Columns10"))
		});
		this.rlpShipType.DisplayMember = "Name";
		this.rlpShipType.Name = "rlpShipType";
		this.rlpShipType.ValueMember = "Code";
		this.rlpShipType.EditValueChanged += new System.EventHandler(this.rlpShipType_EditValueChanged);
		resources.ApplyResources(this.gcShipTo, "gcShipTo");
		this.gcShipTo.ColumnEdit = this.rShipTo;
		this.gcShipTo.CurrencyDateField = null;
		this.gcShipTo.CurrencyField = null;
		this.gcShipTo.CurrencyFixed = null;
		this.gcShipTo.CurrencyFixedRate = 0.0;
		this.gcShipTo.CurrencyRateField = null;
		this.gcShipTo.FieldName = "ShipToCode";
		this.gcShipTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcShipTo.Name = "gcShipTo";
		this.gcShipTo.PrimaryKey = false;
		this.gcShipTo.UnitField = null;
		resources.ApplyResources(this.rShipTo, "rShipTo");
		this.rShipTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rShipTo.Buttons"))
		});
		this.rShipTo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rShipTo.Columns"), resources.GetString("rShipTo.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rShipTo.Columns2"), resources.GetString("rShipTo.Columns3"), (int)resources.GetObject("rShipTo.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rShipTo.Columns5"), resources.GetString("rShipTo.Columns6"), (bool)resources.GetObject("rShipTo.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rShipTo.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rShipTo.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rShipTo.Columns10")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rShipTo.Columns11"), resources.GetString("rShipTo.Columns12"), (int)resources.GetObject("rShipTo.Columns13"), (DevExpress.Utils.FormatType)resources.GetObject("rShipTo.Columns14"), resources.GetString("rShipTo.Columns15"), (bool)resources.GetObject("rShipTo.Columns16"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rShipTo.Columns17"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rShipTo.Columns18"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rShipTo.Columns19"))
		});
		this.rShipTo.DisplayMember = "Address";
		this.rShipTo.Name = "rShipTo";
		this.rShipTo.ValueMember = "Address";
		this.rShipTo.EditValueChanged += new System.EventHandler(this.rlpShipTo_EditValueChanged);
		resources.ApplyResources(this.gridColumn9, "gridColumn9");
		this.gridColumn9.ColumnEdit = this.rep_Address2;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Address2";
		this.gridColumn9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		resources.ApplyResources(this.rep_Address2, "rep_Address2");
		this.rep_Address2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Address2.Buttons"))
		});
		this.rep_Address2.Name = "rep_Address2";
		this.rep_Address2.ShowIcon = false;
		resources.ApplyResources(this.gcBillTo, "gcBillTo");
		this.gcBillTo.ColumnEdit = this.rBillTo;
		this.gcBillTo.CurrencyDateField = null;
		this.gcBillTo.CurrencyField = null;
		this.gcBillTo.CurrencyFixed = null;
		this.gcBillTo.CurrencyFixedRate = 0.0;
		this.gcBillTo.CurrencyRateField = null;
		this.gcBillTo.FieldName = "PayToCode";
		this.gcBillTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBillTo.Name = "gcBillTo";
		this.gcBillTo.PrimaryKey = false;
		this.gcBillTo.UnitField = null;
		resources.ApplyResources(this.rBillTo, "rBillTo");
		this.rBillTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rBillTo.Buttons"))
		});
		this.rBillTo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rBillTo.Columns"), resources.GetString("rBillTo.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rBillTo.Columns2"), resources.GetString("rBillTo.Columns3"), (int)resources.GetObject("rBillTo.Columns4"), (DevExpress.Utils.FormatType)resources.GetObject("rBillTo.Columns5"), resources.GetString("rBillTo.Columns6"), (bool)resources.GetObject("rBillTo.Columns7"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rBillTo.Columns8"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rBillTo.Columns9"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rBillTo.Columns10")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rBillTo.Columns11"), resources.GetString("rBillTo.Columns12"), (int)resources.GetObject("rBillTo.Columns13"), (DevExpress.Utils.FormatType)resources.GetObject("rBillTo.Columns14"), resources.GetString("rBillTo.Columns15"), (bool)resources.GetObject("rBillTo.Columns16"), (DevExpress.Utils.HorzAlignment)resources.GetObject("rBillTo.Columns17"), (DevExpress.Data.ColumnSortOrder)resources.GetObject("rBillTo.Columns18"), (DevExpress.Utils.DefaultBoolean)resources.GetObject("rBillTo.Columns19"))
		});
		this.rBillTo.DisplayMember = "Address";
		this.rBillTo.Name = "rBillTo";
		this.rBillTo.ValueMember = "Address";
		this.rBillTo.EditValueChanged += new System.EventHandler(this.rlpBillTo_EditValueChanged);
		resources.ApplyResources(this.gcConfirmed, "gcConfirmed");
		this.gcConfirmed.ColumnEdit = this.rConfirmed;
		this.gcConfirmed.CurrencyDateField = null;
		this.gcConfirmed.CurrencyField = null;
		this.gcConfirmed.CurrencyFixed = null;
		this.gcConfirmed.CurrencyFixedRate = 0.0;
		this.gcConfirmed.CurrencyRateField = null;
		this.gcConfirmed.FieldName = "Confirmed";
		this.gcConfirmed.Name = "gcConfirmed";
		this.gcConfirmed.PrimaryKey = false;
		this.gcConfirmed.UnitField = null;
		resources.ApplyResources(this.rConfirmed, "rConfirmed");
		this.rConfirmed.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.rConfirmed.Name = "rConfirmed";
		this.rConfirmed.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rConfirmed.ValueChecked = "Y";
		this.rConfirmed.ValueUnchecked = "N";
		resources.ApplyResources(this.gridColumn8, "gridColumn8");
		this.gridColumn8.ColumnEdit = this.rep_Address;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Address";
		this.gridColumn8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		resources.ApplyResources(this.rep_Address, "rep_Address");
		this.rep_Address.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Address.Buttons"))
		});
		this.rep_Address.Name = "rep_Address";
		this.rep_Address.ShowIcon = false;
		resources.ApplyResources(this.gcLogistics_BPChCode, "gcLogistics_BPChCode");
		this.gcLogistics_BPChCode.ColumnEdit = this.rLogisticsBpChannel;
		this.gcLogistics_BPChCode.CurrencyDateField = null;
		this.gcLogistics_BPChCode.CurrencyField = null;
		this.gcLogistics_BPChCode.CurrencyFixed = null;
		this.gcLogistics_BPChCode.CurrencyFixedRate = 0.0;
		this.gcLogistics_BPChCode.CurrencyRateField = null;
		this.gcLogistics_BPChCode.FieldName = "BPChCode";
		this.gcLogistics_BPChCode.Name = "gcLogistics_BPChCode";
		this.gcLogistics_BPChCode.PrimaryKey = false;
		this.gcLogistics_BPChCode.UnitField = null;
		resources.ApplyResources(this.rLogisticsBpChannel, "rLogisticsBpChannel");
		this.rLogisticsBpChannel.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.rLogisticsBpChannel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLogisticsBpChannel.Buttons"), resources.GetString("rLogisticsBpChannel.Buttons1"), (int)resources.GetObject("rLogisticsBpChannel.Buttons2"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons3"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons4"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons5"), imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, resources.GetString("rLogisticsBpChannel.Buttons6"), resources.GetObject("rLogisticsBpChannel.Buttons7"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rLogisticsBpChannel.Buttons8"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rLogisticsBpChannel.Buttons9")),
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLogisticsBpChannel.Buttons10"), resources.GetString("rLogisticsBpChannel.Buttons11"), (int)resources.GetObject("rLogisticsBpChannel.Buttons12"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons13"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons14"), (bool)resources.GetObject("rLogisticsBpChannel.Buttons15"), editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, resources.GetString("rLogisticsBpChannel.Buttons16"), resources.GetObject("rLogisticsBpChannel.Buttons17"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rLogisticsBpChannel.Buttons18"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rLogisticsBpChannel.Buttons19"))
		});
		this.rLogisticsBpChannel.ContainerControl = this;
		this.rLogisticsBpChannel.DisplayMember = "CardCode";
		this.rLogisticsBpChannel.DisplayMember2 = "";
		this.rLogisticsBpChannel.ListMandatoryField = null;
		this.rLogisticsBpChannel.LoadDataSourceWhenEnter = false;
		this.rLogisticsBpChannel.Name = "rLogisticsBpChannel";
		this.rLogisticsBpChannel.SeparateCharacterForMulti = ';';
		this.rLogisticsBpChannel.SeparateString = ", ";
		this.rLogisticsBpChannel.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Customer;
		this.rLogisticsBpChannel.UsingAutoCompleteBox = false;
		this.rLogisticsBpChannel.UsingDataSource = true;
		this.rLogisticsBpChannel.ValidateWhenNoDataSource = false;
		this.rLogisticsBpChannel.ValueMember = "CardCode";
		resources.ApplyResources(this.gcLogisticsBpChannelCntc, "gcLogisticsBpChannelCntc");
		this.gcLogisticsBpChannelCntc.ColumnEdit = this.rLogisticBpChannelCntc;
		this.gcLogisticsBpChannelCntc.CurrencyDateField = null;
		this.gcLogisticsBpChannelCntc.CurrencyField = null;
		this.gcLogisticsBpChannelCntc.CurrencyFixed = null;
		this.gcLogisticsBpChannelCntc.CurrencyFixedRate = 0.0;
		this.gcLogisticsBpChannelCntc.CurrencyRateField = null;
		this.gcLogisticsBpChannelCntc.FieldName = "BPChCntc";
		this.gcLogisticsBpChannelCntc.Name = "gcLogisticsBpChannelCntc";
		this.gcLogisticsBpChannelCntc.PrimaryKey = false;
		this.gcLogisticsBpChannelCntc.UnitField = null;
		resources.ApplyResources(this.rLogisticBpChannelCntc, "rLogisticBpChannelCntc");
		this.rLogisticBpChannelCntc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rLogisticBpChannelCntc.Buttons"))
		});
		this.rLogisticBpChannelCntc.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLogisticBpChannelCntc.Columns"), resources.GetString("rLogisticBpChannelCntc.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rLogisticBpChannelCntc.Columns2"), resources.GetString("rLogisticBpChannelCntc.Columns3"))
		});
		this.rLogisticBpChannelCntc.DisplayMember = "Name";
		this.rLogisticBpChannelCntc.Name = "rLogisticBpChannelCntc";
		this.rLogisticBpChannelCntc.ShowHeader = false;
		this.rLogisticBpChannelCntc.ValueMember = "CntctCode";
		resources.ApplyResources(this.gridColumnEx21, "gridColumnEx21");
		this.gridColumnEx21.ColumnEdit = this.rep_PartSupply;
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "PartSupply";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		resources.ApplyResources(this.rep_PartSupply, "rep_PartSupply");
		this.rep_PartSupply.Name = "rep_PartSupply";
		this.rep_PartSupply.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_PartSupply.ValueChecked = "Y";
		this.rep_PartSupply.ValueGrayed = "N";
		this.rep_PartSupply.ValueUnchecked = "N";
		this.tab_Freight.Controls.Add(this.grd_Freight);
		this.tab_Freight.EditControl = null;
		this.tab_Freight.EditValue = "Freight Charges";
		this.tab_Freight.FieldName = null;
		this.tab_Freight.IsSearchControl = false;
		this.tab_Freight.Name = "tab_Freight";
		this.tab_Freight.ReadOnly = false;
		this.tab_Freight.SettingInVisible = false;
		this.tab_Freight.SettingReadonly = false;
		resources.ApplyResources(this.tab_Freight, "tab_Freight");
		this.tab_Freight.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.grd_Freight.AllowPasteExcel = false;
		this.grd_Freight.ClipboardData = null;
		resources.ApplyResources(this.grd_Freight, "grd_Freight");
		this.grd_Freight.MainView = this.gv_Freight;
		this.grd_Freight.MenuManager = this.barManager1;
		this.grd_Freight.Name = "grd_Freight";
		this.grd_Freight.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_DistriMethod, this.rep_Project, this.rep_StatusFreight, this.rep_GroupTax, this.rep_ExpenseCode, this.rep_Stock, this.rep_LstPchPrce });
		this.grd_Freight.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Freight });
		this.gv_Freight.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Freight.ColumnKey = null;
		this.gv_Freight.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[19]
		{
			this.gridColumn4, this.gridColumn1, this.gridColumn18, this.gridColumn67, this.gridColumn11, this.gridColumn66, this.gridColumn13, this.gridColumn14, this.gridColumn68, this.gridColumn16,
			this.gridColumn15, this.gridColumn17, this.gridColumn12, this.gridColumn53, this.gridColumn54, this.gridColumn51, this.gridColumn52, this.gridColumnEx13, this.gridColumnEx14
		});
		this.gv_Freight.DisableFormSetting = false;
		this.gv_Freight.DisplayException = false;
		this.gv_Freight.FieldCurrency = null;
		this.gv_Freight.FieldRate = null;
		this.gv_Freight.FormId = "oDocument";
		this.gv_Freight.GridControl = this.grd_Freight;
		this.gv_Freight.IndicatorWidth = 40;
		this.gv_Freight.IsApzTable = false;
		this.gv_Freight.IsInsertNewRow = false;
		this.gv_Freight.IsLoadingData = false;
		this.gv_Freight.ItemId = "OFRT";
		this.gv_Freight.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Freight.LstColumnAllowBatch = null;
		this.gv_Freight.LstColumnNotAllowBatch = null;
		this.gv_Freight.LstColumnNotEditable = null;
		this.gv_Freight.LstColumNotSetting = null;
		this.gv_Freight.Name = "gv_Freight";
		this.gv_Freight.OptionsView.ColumnAutoWidth = false;
		this.gv_Freight.OptionsView.ShowGroupedColumns = true;
		this.gv_Freight.OptionsView.ShowGroupPanel = false;
		this.gv_Freight.UseBatchMod = false;
		resources.ApplyResources(this.gv_Freight, "gv_Freight");
		this.gv_Freight.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Freight_CellValueChanged);
		this.gridColumn4.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn4.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn4, "gridColumn4");
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DocEntry";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn1.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn1, "gridColumn1");
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "LineNum";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn18.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn18.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn18, "gridColumn18");
		this.gridColumn18.ColumnEdit = this.rep_ExpenseCode;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "ExpnsCode";
		this.gridColumn18.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowEdit = false;
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		resources.ApplyResources(this.rep_ExpenseCode, "rep_ExpenseCode");
		this.rep_ExpenseCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_ExpenseCode.Buttons"))
		});
		this.rep_ExpenseCode.DisplayMember = "Name";
		this.rep_ExpenseCode.Name = "rep_ExpenseCode";
		this.rep_ExpenseCode.ValueMember = "Code";
		resources.ApplyResources(this.gridColumn67, "gridColumn67");
		this.gridColumn67.CurrencyDateField = null;
		this.gridColumn67.CurrencyField = null;
		this.gridColumn67.CurrencyFixed = null;
		this.gridColumn67.CurrencyFixedRate = 0.0;
		this.gridColumn67.CurrencyRateField = null;
		this.gridColumn67.FieldName = "Comments";
		this.gridColumn67.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn67.Name = "gridColumn67";
		this.gridColumn67.PrimaryKey = false;
		this.gridColumn67.UnitField = null;
		resources.ApplyResources(this.gridColumn11, "gridColumn11");
		this.gridColumn11.ColumnEdit = this.rep_GroupTax;
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "VatGroup";
		this.gridColumn11.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		resources.ApplyResources(this.rep_GroupTax, "rep_GroupTax");
		this.rep_GroupTax.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_GroupTax.Buttons"))
		});
		this.rep_GroupTax.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_GroupTax.Columns"), resources.GetString("rep_GroupTax.Columns1")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_GroupTax.Columns2"), resources.GetString("rep_GroupTax.Columns3")),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("rep_GroupTax.Columns4"), resources.GetString("rep_GroupTax.Columns5"))
		});
		this.rep_GroupTax.DisplayMember = "Code";
		this.rep_GroupTax.Name = "rep_GroupTax";
		this.rep_GroupTax.ValueMember = "Code";
		this.gridColumn66.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn66.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn66, "gridColumn66");
		this.gridColumn66.CurrencyDateField = null;
		this.gridColumn66.CurrencyField = null;
		this.gridColumn66.CurrencyFixed = null;
		this.gridColumn66.CurrencyFixedRate = 0.0;
		this.gridColumn66.CurrencyRateField = null;
		this.gridColumn66.FieldName = "VatPrcnt";
		this.gridColumn66.Name = "gridColumn66";
		this.gridColumn66.OptionsColumn.AllowEdit = false;
		this.gridColumn66.OptionsColumn.ReadOnly = true;
		this.gridColumn66.PrimaryKey = false;
		this.gridColumn66.UnitField = null;
		this.gridColumn13.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn13.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn13, "gridColumn13");
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn13.FieldName = "VatSum";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowEdit = false;
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		resources.ApplyResources(this.gridColumn14, "gridColumn14");
		this.gridColumn14.ColumnEdit = this.rep_DistriMethod;
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "DistrbMthd";
		this.gridColumn14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		resources.ApplyResources(this.rep_DistriMethod, "rep_DistriMethod");
		this.rep_DistriMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_DistriMethod.Buttons"))
		});
		this.rep_DistriMethod.DisplayMember = "Name";
		this.rep_DistriMethod.Name = "rep_DistriMethod";
		this.rep_DistriMethod.ValueMember = "ID";
		resources.ApplyResources(this.gridColumn68, "gridColumn68");
		this.gridColumn68.CurrencyDateField = null;
		this.gridColumn68.CurrencyField = null;
		this.gridColumn68.CurrencyFixed = null;
		this.gridColumn68.CurrencyFixedRate = 0.0;
		this.gridColumn68.CurrencyRateField = null;
		this.gridColumn68.FieldName = "LineTotal";
		this.gridColumn68.Name = "gridColumn68";
		this.gridColumn68.PrimaryKey = false;
		this.gridColumn68.UnitField = null;
		this.gridColumn16.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn16.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn16, "gridColumn16");
		this.gridColumn16.ColumnEdit = this.rep_StatusFreight;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Status";
		this.gridColumn16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowEdit = false;
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		resources.ApplyResources(this.rep_StatusFreight, "rep_StatusFreight");
		this.rep_StatusFreight.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_StatusFreight.Buttons"))
		});
		this.rep_StatusFreight.DisplayMember = "Name";
		this.rep_StatusFreight.Name = "rep_StatusFreight";
		this.rep_StatusFreight.ValueMember = "ID";
		resources.ApplyResources(this.gridColumn15, "gridColumn15");
		this.gridColumn15.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn15.FieldName = "GrsAmount";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		resources.ApplyResources(this.gridColumn17, "gridColumn17");
		this.gridColumn17.ColumnEdit = this.rep_Project;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Project";
		this.gridColumn17.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		resources.ApplyResources(this.rep_Project, "rep_Project");
		this.rep_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Project.Buttons"))
		});
		this.rep_Project.Name = "rep_Project";
		resources.ApplyResources(this.gridColumn12, "gridColumn12");
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "TotalFrgn";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		resources.ApplyResources(this.gridColumn53, "gridColumn53");
		this.gridColumn53.CurrencyDateField = null;
		this.gridColumn53.CurrencyField = null;
		this.gridColumn53.CurrencyFixed = null;
		this.gridColumn53.CurrencyFixedRate = 0.0;
		this.gridColumn53.CurrencyRateField = null;
		this.gridColumn53.FieldName = "VatSumFrgn";
		this.gridColumn53.Name = "gridColumn53";
		this.gridColumn53.PrimaryKey = false;
		this.gridColumn53.UnitField = null;
		resources.ApplyResources(this.gridColumn54, "gridColumn54");
		this.gridColumn54.CurrencyDateField = null;
		this.gridColumn54.CurrencyField = null;
		this.gridColumn54.CurrencyFixed = null;
		this.gridColumn54.CurrencyFixedRate = 0.0;
		this.gridColumn54.CurrencyRateField = null;
		this.gridColumn54.FieldName = "GrsFC";
		this.gridColumn54.Name = "gridColumn54";
		this.gridColumn54.PrimaryKey = false;
		this.gridColumn54.UnitField = null;
		this.gridColumn51.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn51.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn51, "gridColumn51");
		this.gridColumn51.CurrencyDateField = null;
		this.gridColumn51.CurrencyField = null;
		this.gridColumn51.CurrencyFixed = null;
		this.gridColumn51.CurrencyFixedRate = 0.0;
		this.gridColumn51.CurrencyRateField = null;
		this.gridColumn51.FieldName = "BaseRef";
		this.gridColumn51.Name = "gridColumn51";
		this.gridColumn51.OptionsColumn.AllowEdit = false;
		this.gridColumn51.OptionsColumn.ReadOnly = true;
		this.gridColumn51.PrimaryKey = false;
		this.gridColumn51.UnitField = null;
		this.gridColumn52.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gridColumn52.AppearanceCell.Options.UseBackColor = true;
		resources.ApplyResources(this.gridColumn52, "gridColumn52");
		this.gridColumn52.CurrencyDateField = null;
		this.gridColumn52.CurrencyField = null;
		this.gridColumn52.CurrencyFixed = null;
		this.gridColumn52.CurrencyFixedRate = 0.0;
		this.gridColumn52.CurrencyRateField = null;
		this.gridColumn52.FieldName = "BaseLnNum";
		this.gridColumn52.Name = "gridColumn52";
		this.gridColumn52.OptionsColumn.AllowEdit = false;
		this.gridColumn52.OptionsColumn.ReadOnly = true;
		this.gridColumn52.PrimaryKey = false;
		this.gridColumn52.UnitField = null;
		resources.ApplyResources(this.gridColumnEx13, "gridColumnEx13");
		this.gridColumnEx13.ColumnEdit = this.rep_Stock;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Stock";
		this.gridColumnEx13.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		resources.ApplyResources(this.rep_Stock, "rep_Stock");
		this.rep_Stock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_Stock.Buttons"))
		});
		this.rep_Stock.Name = "rep_Stock";
		resources.ApplyResources(this.gridColumnEx14, "gridColumnEx14");
		this.gridColumnEx14.ColumnEdit = this.rep_LstPchPrce;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "LstPchPrce";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		resources.ApplyResources(this.rep_LstPchPrce, "rep_LstPchPrce");
		this.rep_LstPchPrce.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_LstPchPrce.Buttons"))
		});
		this.rep_LstPchPrce.Name = "rep_LstPchPrce";
		this.tabP_Accounting.Controls.Add(this.grd_Accounting);
		this.tabP_Accounting.EditControl = null;
		this.tabP_Accounting.EditValue = "Accounting";
		this.tabP_Accounting.FieldName = null;
		this.tabP_Accounting.IsSearchControl = false;
		this.tabP_Accounting.Name = "tabP_Accounting";
		this.tabP_Accounting.ReadOnly = false;
		this.tabP_Accounting.SettingInVisible = false;
		this.tabP_Accounting.SettingReadonly = false;
		resources.ApplyResources(this.tabP_Accounting, "tabP_Accounting");
		this.tabP_Accounting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.grd_Accounting.AllowPasteExcel = false;
		this.grd_Accounting.ClipboardData = null;
		resources.ApplyResources(this.grd_Accounting, "grd_Accounting");
		this.grd_Accounting.MainView = this.gv_Accounting;
		this.grd_Accounting.Name = "grd_Accounting";
		this.grd_Accounting.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.luk_Accounting_GroupNum, this.bte_Accounting_Project, this.luk_Accounting_PaymentMethod, this.luk_Accounting_PayDuMonth, this.luk_Accounting_Indicator, this.rep_chk_UseShpdGd, this.rep_ReferencedDocument });
		this.grd_Accounting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Accounting });
		this.gv_Accounting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Accounting.ColumnKey = null;
		this.gv_Accounting.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gc_Accounting_JrnlMemo, this.gc_Accounting_Project, this.gc_Accounting_GroupNum, this.gc_Accounting_PeyMethod, this.gc_Accounting_Indicator, this.gc_Accounting_LicTradNum, this.gc_Accounting_ImportEnt, this.gc_Accounting_PayDuMonth, this.gc_Accounting_ExtraMonth, this.gc_Accounting_ExtraDays,
			this.gc_Accounting_CdcOffset, this.gridColumnEx1, this.gridColumnEx22
		});
		this.gv_Accounting.DisableFormSetting = false;
		this.gv_Accounting.DisplayException = false;
		this.gv_Accounting.FieldCurrency = null;
		this.gv_Accounting.FieldRate = null;
		this.gv_Accounting.FormId = "oDocument";
		this.gv_Accounting.GridControl = this.grd_Accounting;
		this.gv_Accounting.IndicatorWidth = 35;
		this.gv_Accounting.IsApzTable = false;
		this.gv_Accounting.IsInsertNewRow = false;
		this.gv_Accounting.IsLoadingData = false;
		this.gv_Accounting.ItemId = "OACT";
		this.gv_Accounting.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Accounting.LstColumnAllowBatch = null;
		this.gv_Accounting.LstColumnNotAllowBatch = null;
		this.gv_Accounting.LstColumnNotEditable = null;
		this.gv_Accounting.LstColumNotSetting = null;
		this.gv_Accounting.Name = "gv_Accounting";
		this.gv_Accounting.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Accounting.OptionsView.ColumnAutoWidth = false;
		this.gv_Accounting.OptionsView.ShowGroupPanel = false;
		this.gv_Accounting.UseBatchMod = false;
		resources.ApplyResources(this.gv_Accounting, "gv_Accounting");
		this.gv_Accounting.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Accounting_ShowingEditor);
		this.gv_Accounting.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Accounting_CellValueChanged);
		resources.ApplyResources(this.gc_Accounting_JrnlMemo, "gc_Accounting_JrnlMemo");
		this.gc_Accounting_JrnlMemo.CurrencyDateField = null;
		this.gc_Accounting_JrnlMemo.CurrencyField = null;
		this.gc_Accounting_JrnlMemo.CurrencyFixed = null;
		this.gc_Accounting_JrnlMemo.CurrencyFixedRate = 0.0;
		this.gc_Accounting_JrnlMemo.CurrencyRateField = null;
		this.gc_Accounting_JrnlMemo.FieldName = "JrnlMemo";
		this.gc_Accounting_JrnlMemo.Name = "gc_Accounting_JrnlMemo";
		this.gc_Accounting_JrnlMemo.PrimaryKey = false;
		this.gc_Accounting_JrnlMemo.UnitField = null;
		resources.ApplyResources(this.gc_Accounting_Project, "gc_Accounting_Project");
		this.gc_Accounting_Project.ColumnEdit = this.bte_Accounting_Project;
		this.gc_Accounting_Project.CurrencyDateField = null;
		this.gc_Accounting_Project.CurrencyField = null;
		this.gc_Accounting_Project.CurrencyFixed = null;
		this.gc_Accounting_Project.CurrencyFixedRate = 0.0;
		this.gc_Accounting_Project.CurrencyRateField = null;
		this.gc_Accounting_Project.FieldName = "Project";
		this.gc_Accounting_Project.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Accounting_Project.Name = "gc_Accounting_Project";
		this.gc_Accounting_Project.PrimaryKey = false;
		this.gc_Accounting_Project.UnitField = null;
		resources.ApplyResources(this.bte_Accounting_Project, "bte_Accounting_Project");
		this.bte_Accounting_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("bte_Accounting_Project.Buttons"))
		});
		this.bte_Accounting_Project.Name = "bte_Accounting_Project";
		this.bte_Accounting_Project.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Accounting_Project_ButtonPressed);
		resources.ApplyResources(this.gc_Accounting_GroupNum, "gc_Accounting_GroupNum");
		this.gc_Accounting_GroupNum.ColumnEdit = this.luk_Accounting_GroupNum;
		this.gc_Accounting_GroupNum.CurrencyDateField = null;
		this.gc_Accounting_GroupNum.CurrencyField = null;
		this.gc_Accounting_GroupNum.CurrencyFixed = null;
		this.gc_Accounting_GroupNum.CurrencyFixedRate = 0.0;
		this.gc_Accounting_GroupNum.CurrencyRateField = null;
		this.gc_Accounting_GroupNum.FieldName = "GroupNum";
		this.gc_Accounting_GroupNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Accounting_GroupNum.Name = "gc_Accounting_GroupNum";
		this.gc_Accounting_GroupNum.PrimaryKey = false;
		this.gc_Accounting_GroupNum.UnitField = null;
		resources.ApplyResources(this.luk_Accounting_GroupNum, "luk_Accounting_GroupNum");
		this.luk_Accounting_GroupNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("luk_Accounting_GroupNum.Buttons"))
		});
		this.luk_Accounting_GroupNum.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luk_Accounting_GroupNum.Columns"), resources.GetString("luk_Accounting_GroupNum.Columns1"))
		});
		this.luk_Accounting_GroupNum.DisplayMember = "Name";
		this.luk_Accounting_GroupNum.Name = "luk_Accounting_GroupNum";
		this.luk_Accounting_GroupNum.ValueMember = "Code";
		this.luk_Accounting_GroupNum.EditValueChanged += new System.EventHandler(this.luk_Accounting_GroupNum_EditValueChanged);
		resources.ApplyResources(this.gc_Accounting_PeyMethod, "gc_Accounting_PeyMethod");
		this.gc_Accounting_PeyMethod.ColumnEdit = this.luk_Accounting_PaymentMethod;
		this.gc_Accounting_PeyMethod.CurrencyDateField = null;
		this.gc_Accounting_PeyMethod.CurrencyField = null;
		this.gc_Accounting_PeyMethod.CurrencyFixed = null;
		this.gc_Accounting_PeyMethod.CurrencyFixedRate = 0.0;
		this.gc_Accounting_PeyMethod.CurrencyRateField = null;
		this.gc_Accounting_PeyMethod.FieldName = "PeyMethod";
		this.gc_Accounting_PeyMethod.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Accounting_PeyMethod.Name = "gc_Accounting_PeyMethod";
		this.gc_Accounting_PeyMethod.PrimaryKey = false;
		this.gc_Accounting_PeyMethod.UnitField = null;
		resources.ApplyResources(this.luk_Accounting_PaymentMethod, "luk_Accounting_PaymentMethod");
		this.luk_Accounting_PaymentMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("luk_Accounting_PaymentMethod.Buttons"))
		});
		this.luk_Accounting_PaymentMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luk_Accounting_PaymentMethod.Columns"), resources.GetString("luk_Accounting_PaymentMethod.Columns1"))
		});
		this.luk_Accounting_PaymentMethod.DisplayMember = "Name";
		this.luk_Accounting_PaymentMethod.Name = "luk_Accounting_PaymentMethod";
		this.luk_Accounting_PaymentMethod.ValueMember = "Code";
		resources.ApplyResources(this.gc_Accounting_Indicator, "gc_Accounting_Indicator");
		this.gc_Accounting_Indicator.ColumnEdit = this.luk_Accounting_Indicator;
		this.gc_Accounting_Indicator.CurrencyDateField = null;
		this.gc_Accounting_Indicator.CurrencyField = null;
		this.gc_Accounting_Indicator.CurrencyFixed = null;
		this.gc_Accounting_Indicator.CurrencyFixedRate = 0.0;
		this.gc_Accounting_Indicator.CurrencyRateField = null;
		this.gc_Accounting_Indicator.FieldName = "Indicator";
		this.gc_Accounting_Indicator.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Accounting_Indicator.Name = "gc_Accounting_Indicator";
		this.gc_Accounting_Indicator.PrimaryKey = false;
		this.gc_Accounting_Indicator.UnitField = null;
		resources.ApplyResources(this.luk_Accounting_Indicator, "luk_Accounting_Indicator");
		this.luk_Accounting_Indicator.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("luk_Accounting_Indicator.Buttons"))
		});
		this.luk_Accounting_Indicator.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luk_Accounting_Indicator.Columns"), resources.GetString("luk_Accounting_Indicator.Columns1"))
		});
		this.luk_Accounting_Indicator.DisplayMember = "Name";
		this.luk_Accounting_Indicator.Name = "luk_Accounting_Indicator";
		this.luk_Accounting_Indicator.ValueMember = "Code";
		resources.ApplyResources(this.gc_Accounting_LicTradNum, "gc_Accounting_LicTradNum");
		this.gc_Accounting_LicTradNum.CurrencyDateField = null;
		this.gc_Accounting_LicTradNum.CurrencyField = null;
		this.gc_Accounting_LicTradNum.CurrencyFixed = null;
		this.gc_Accounting_LicTradNum.CurrencyFixedRate = 0.0;
		this.gc_Accounting_LicTradNum.CurrencyRateField = null;
		this.gc_Accounting_LicTradNum.FieldName = "LicTradNum";
		this.gc_Accounting_LicTradNum.Name = "gc_Accounting_LicTradNum";
		this.gc_Accounting_LicTradNum.PrimaryKey = false;
		this.gc_Accounting_LicTradNum.UnitField = null;
		resources.ApplyResources(this.gc_Accounting_ImportEnt, "gc_Accounting_ImportEnt");
		this.gc_Accounting_ImportEnt.CurrencyDateField = null;
		this.gc_Accounting_ImportEnt.CurrencyField = null;
		this.gc_Accounting_ImportEnt.CurrencyFixed = null;
		this.gc_Accounting_ImportEnt.CurrencyFixedRate = 0.0;
		this.gc_Accounting_ImportEnt.CurrencyRateField = null;
		this.gc_Accounting_ImportEnt.FieldName = "ImportEnt";
		this.gc_Accounting_ImportEnt.Name = "gc_Accounting_ImportEnt";
		this.gc_Accounting_ImportEnt.PrimaryKey = false;
		this.gc_Accounting_ImportEnt.UnitField = null;
		resources.ApplyResources(this.gc_Accounting_PayDuMonth, "gc_Accounting_PayDuMonth");
		this.gc_Accounting_PayDuMonth.ColumnEdit = this.luk_Accounting_PayDuMonth;
		this.gc_Accounting_PayDuMonth.CurrencyDateField = null;
		this.gc_Accounting_PayDuMonth.CurrencyField = null;
		this.gc_Accounting_PayDuMonth.CurrencyFixed = null;
		this.gc_Accounting_PayDuMonth.CurrencyFixedRate = 0.0;
		this.gc_Accounting_PayDuMonth.CurrencyRateField = null;
		this.gc_Accounting_PayDuMonth.FieldName = "PayDuMonth";
		this.gc_Accounting_PayDuMonth.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Accounting_PayDuMonth.Name = "gc_Accounting_PayDuMonth";
		this.gc_Accounting_PayDuMonth.PrimaryKey = false;
		this.gc_Accounting_PayDuMonth.UnitField = null;
		resources.ApplyResources(this.luk_Accounting_PayDuMonth, "luk_Accounting_PayDuMonth");
		this.luk_Accounting_PayDuMonth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("luk_Accounting_PayDuMonth.Buttons"))
		});
		this.luk_Accounting_PayDuMonth.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luk_Accounting_PayDuMonth.Columns"), resources.GetString("luk_Accounting_PayDuMonth.Columns1"))
		});
		this.luk_Accounting_PayDuMonth.DisplayMember = "Name";
		this.luk_Accounting_PayDuMonth.Name = "luk_Accounting_PayDuMonth";
		this.luk_Accounting_PayDuMonth.ValueMember = "Code";
		resources.ApplyResources(this.gc_Accounting_ExtraMonth, "gc_Accounting_ExtraMonth");
		this.gc_Accounting_ExtraMonth.CurrencyDateField = null;
		this.gc_Accounting_ExtraMonth.CurrencyField = null;
		this.gc_Accounting_ExtraMonth.CurrencyFixed = null;
		this.gc_Accounting_ExtraMonth.CurrencyFixedRate = 0.0;
		this.gc_Accounting_ExtraMonth.CurrencyRateField = null;
		this.gc_Accounting_ExtraMonth.FieldName = "ExtraMonth";
		this.gc_Accounting_ExtraMonth.Name = "gc_Accounting_ExtraMonth";
		this.gc_Accounting_ExtraMonth.PrimaryKey = false;
		this.gc_Accounting_ExtraMonth.UnitField = null;
		resources.ApplyResources(this.gc_Accounting_ExtraDays, "gc_Accounting_ExtraDays");
		this.gc_Accounting_ExtraDays.CurrencyDateField = null;
		this.gc_Accounting_ExtraDays.CurrencyField = null;
		this.gc_Accounting_ExtraDays.CurrencyFixed = null;
		this.gc_Accounting_ExtraDays.CurrencyFixedRate = 0.0;
		this.gc_Accounting_ExtraDays.CurrencyRateField = null;
		this.gc_Accounting_ExtraDays.FieldName = "ExtraDays";
		this.gc_Accounting_ExtraDays.Name = "gc_Accounting_ExtraDays";
		this.gc_Accounting_ExtraDays.PrimaryKey = false;
		this.gc_Accounting_ExtraDays.UnitField = null;
		resources.ApplyResources(this.gc_Accounting_CdcOffset, "gc_Accounting_CdcOffset");
		this.gc_Accounting_CdcOffset.CurrencyDateField = null;
		this.gc_Accounting_CdcOffset.CurrencyField = null;
		this.gc_Accounting_CdcOffset.CurrencyFixed = null;
		this.gc_Accounting_CdcOffset.CurrencyFixedRate = 0.0;
		this.gc_Accounting_CdcOffset.CurrencyRateField = null;
		this.gc_Accounting_CdcOffset.FieldName = "CdcOffset";
		this.gc_Accounting_CdcOffset.Name = "gc_Accounting_CdcOffset";
		this.gc_Accounting_CdcOffset.PrimaryKey = false;
		this.gc_Accounting_CdcOffset.UnitField = null;
		resources.ApplyResources(this.gridColumnEx1, "gridColumnEx1");
		this.gridColumnEx1.ColumnEdit = this.rep_chk_UseShpdGd;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "UseShpdGd";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		resources.ApplyResources(this.rep_chk_UseShpdGd, "rep_chk_UseShpdGd");
		this.rep_chk_UseShpdGd.Name = "rep_chk_UseShpdGd";
		this.rep_chk_UseShpdGd.ValueChecked = "Y";
		this.rep_chk_UseShpdGd.ValueGrayed = "N";
		this.rep_chk_UseShpdGd.ValueUnchecked = "N";
		resources.ApplyResources(this.gridColumnEx22, "gridColumnEx22");
		this.gridColumnEx22.ColumnEdit = this.rep_ReferencedDocument;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "REFDOC";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		resources.ApplyResources(this.rep_ReferencedDocument, "rep_ReferencedDocument");
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ReferencedDocument.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("rep_ReferencedDocument.Buttons"), resources.GetString("rep_ReferencedDocument.Buttons1"), (int)resources.GetObject("rep_ReferencedDocument.Buttons2"), (bool)resources.GetObject("rep_ReferencedDocument.Buttons3"), (bool)resources.GetObject("rep_ReferencedDocument.Buttons4"), (bool)resources.GetObject("rep_ReferencedDocument.Buttons5"), editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, resources.GetString("rep_ReferencedDocument.Buttons6"), resources.GetObject("rep_ReferencedDocument.Buttons7"), (DevExpress.Utils.SuperToolTip)resources.GetObject("rep_ReferencedDocument.Buttons8"), (DevExpress.Utils.ToolTipAnchor)resources.GetObject("rep_ReferencedDocument.Buttons9"))
		});
		this.rep_ReferencedDocument.Name = "rep_ReferencedDocument";
		this.rep_ReferencedDocument.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_ReferencedDocument.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ReferencedDocument_ButtonClick);
		this.tab_Attachments.Controls.Add(this.ucAttachment1);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = null;
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		resources.ApplyResources(this.tab_Attachments, "tab_Attachments");
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.ucAttachment1.DataSource = null;
		resources.ApplyResources(this.ucAttachment1, "ucAttachment1");
		this.ucAttachment1.FormId = "oDocument";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.ButtonDelete_Click += new System.EventHandler(this.ucAttachment1_ButtonDelete_Click);
		this.ucBatchAndSerial1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.ucBatchAndSerial1, "ucBatchAndSerial1");
		this.ucBatchAndSerial1.FieldLineType = "LineType";
		this.ucBatchAndSerial1.FieldQuantity = "InvQty";
		this.ucBatchAndSerial1.FieldType = "ManBatchSerial";
		this.ucBatchAndSerial1.FormId = "oDocument";
		this.ucBatchAndSerial1.GvHeader = this.gvDoc;
		this.ucBatchAndSerial1.GvLineItem = this.gvLine;
		this.ucBatchAndSerial1.IsAllowcateBinLocationToWhs = false;
		this.ucBatchAndSerial1.ItemId = "OBTN";
		this.ucBatchAndSerial1.ItemIdBin = "oBinAllowcation";
		this.ucBatchAndSerial1.Name = "ucBatchAndSerial1";
		this.repositoryItemCalcEditEx24.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx24.Buttons"))
		});
		this.repositoryItemCalcEditEx24.IsSearchControl = false;
		this.repositoryItemCalcEditEx24.Name = "repositoryItemCalcEditEx24";
		this.repositoryItemCalcEditEx25.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("repositoryItemCalcEditEx25.Buttons"))
		});
		this.repositoryItemCalcEditEx25.IsSearchControl = false;
		this.repositoryItemCalcEditEx25.Name = "repositoryItemCalcEditEx25";
		this.txtFromDate.EditControl = null;
		resources.ApplyResources(this.txtFromDate, "txtFromDate");
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Appearance.Font = (System.Drawing.Font)resources.GetObject("txtFromDate.Properties.Appearance.Font");
		this.txtFromDate.Properties.Appearance.Options.UseFont = true;
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtFromDate.Properties.Buttons"))
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtFromDate.Properties.CalendarTimeProperties.Buttons"))
		});
		this.txtFromDate.Properties.IsSearchControl = true;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtToDate.EditControl = null;
		resources.ApplyResources(this.txtToDate, "txtToDate");
		this.txtToDate.FieldName = null;
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Appearance.Font = (System.Drawing.Font)resources.GetObject("txtToDate.Properties.Appearance.Font");
		this.txtToDate.Properties.Appearance.Options.UseFont = true;
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtToDate.Properties.Buttons"))
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtToDate.Properties.CalendarTimeProperties.Buttons"))
		});
		this.txtToDate.Properties.IsSearchControl = true;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.EditValueChanged += new System.EventHandler(this.txtToDate_EditValueChanged);
		this.txtNoOfTrans.EditControl = null;
		this.txtNoOfTrans.FieldName = null;
		this.txtNoOfTrans.IsSearchControl = true;
		resources.ApplyResources(this.txtNoOfTrans, "txtNoOfTrans");
		this.txtNoOfTrans.Name = "txtNoOfTrans";
		this.txtNoOfTrans.Properties.Appearance.Font = (System.Drawing.Font)resources.GetObject("txtNoOfTrans.Properties.Appearance.Font");
		this.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		this.txtNoOfTrans.SettingInVisible = false;
		this.txtNoOfTrans.SettingReadonly = false;
		resources.ApplyResources(this.drlCopyTo, "drlCopyTo");
		this.drlCopyTo.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.drlCopyTo.Appearance.Options.UseBackColor = true;
		this.drlCopyTo.EditControl = null;
		this.drlCopyTo.EditValue = "Copy To";
		this.drlCopyTo.FieldName = null;
		this.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		this.drlCopyTo.IsSearchControl = false;
		this.drlCopyTo.ItemId = null;
		this.drlCopyTo.Name = "drlCopyTo";
		this.drlCopyTo.ReadOnly = false;
		this.drlCopyTo.SettingInVisible = false;
		this.drlCopyTo.SettingReadonly = false;
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		resources.ApplyResources(this.btnSearch, "btnSearch");
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		resources.ApplyResources(this.btnCancel, "btnCancel");
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_FromDate, "lb_FromDate");
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.Utext = "From Date";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_ToDate, "lb_ToDate");
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.Utext = "To Date";
		this.lb_DocNumber.EditControl = null;
		this.lb_DocNumber.EditValue = "Doc. Number";
		this.lb_DocNumber.FieldName = null;
		this.lb_DocNumber.HideWhenEmpty = false;
		this.lb_DocNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_DocNumber, "lb_DocNumber");
		this.lb_DocNumber.Name = "lb_DocNumber";
		this.lb_DocNumber.ReadOnly = false;
		this.lb_DocNumber.SettingInVisible = false;
		this.lb_DocNumber.SettingReadonly = false;
		this.lb_DocNumber.ShowUnderLine = true;
		this.lb_DocNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNumber.Utext = "Doc. Number";
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Business Partner";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_BusinessPartner, "lb_BusinessPartner");
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.Utext = "Business Partner";
		this.lb_DocStatus.EditControl = null;
		this.lb_DocStatus.EditValue = "Document Status";
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_DocStatus, "lb_DocStatus");
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ReadOnly = false;
		this.lb_DocStatus.SettingInVisible = false;
		this.lb_DocStatus.SettingReadonly = false;
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.Utext = "Document Status";
		this.txtBusinessPartner.EditControl = null;
		this.txtBusinessPartner.FieldName = null;
		this.txtBusinessPartner.IsSearchControl = true;
		resources.ApplyResources(this.txtBusinessPartner, "txtBusinessPartner");
		this.txtBusinessPartner.Name = "txtBusinessPartner";
		this.txtBusinessPartner.Properties.Appearance.Font = (System.Drawing.Font)resources.GetObject("txtBusinessPartner.Properties.Appearance.Font");
		this.txtBusinessPartner.Properties.Appearance.Options.UseFont = true;
		this.txtBusinessPartner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtBusinessPartner.Properties.Buttons"))
		});
		this.txtBusinessPartner.SettingInVisible = false;
		this.txtBusinessPartner.SettingReadonly = false;
		this.txtBusinessPartner.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBusinessPartner_ButtonClick);
		this.cbxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		resources.ApplyResources(this.cbxStatus, "cbxStatus");
		this.cbxStatus.FormattingEnabled = true;
		this.cbxStatus.Items.AddRange(new object[5]
		{
			resources.GetString("cbxStatus.Items"),
			resources.GetString("cbxStatus.Items1"),
			resources.GetString("cbxStatus.Items2"),
			resources.GetString("cbxStatus.Items3"),
			resources.GetString("cbxStatus.Items4")
		});
		this.cbxStatus.Name = "cbxStatus";
		resources.ApplyResources(this.drlCopyFrom, "drlCopyFrom");
		this.drlCopyFrom.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.drlCopyFrom.Appearance.Options.UseBackColor = true;
		this.drlCopyFrom.EditControl = null;
		this.drlCopyFrom.EditValue = "Copy From";
		this.drlCopyFrom.FieldName = null;
		this.drlCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyFrom.ImageOptions.Image");
		this.drlCopyFrom.IsSearchControl = false;
		this.drlCopyFrom.ItemId = null;
		this.drlCopyFrom.Name = "drlCopyFrom";
		this.drlCopyFrom.ReadOnly = false;
		this.drlCopyFrom.SettingInVisible = false;
		this.drlCopyFrom.SettingReadonly = false;
		this.txtRequester.EditControl = null;
		this.txtRequester.FieldName = null;
		this.txtRequester.IsSearchControl = true;
		resources.ApplyResources(this.txtRequester, "txtRequester");
		this.txtRequester.MenuManager = this.barManager1;
		this.txtRequester.Name = "txtRequester";
		this.txtRequester.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("txtRequester.Properties.Buttons"))
		});
		this.txtRequester.SettingInVisible = false;
		this.txtRequester.SettingReadonly = false;
		this.txtRequester.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtRequester_ButtonClick);
		this.txtRequester.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRequester_KeyDown);
		this.lb_Requester.EditControl = null;
		this.lb_Requester.EditValue = "Requester";
		this.lb_Requester.FieldName = null;
		this.lb_Requester.HideWhenEmpty = false;
		this.lb_Requester.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		resources.ApplyResources(this.lb_Requester, "lb_Requester");
		this.lb_Requester.Name = "lb_Requester";
		this.lb_Requester.ReadOnly = false;
		this.lb_Requester.SettingInVisible = false;
		this.lb_Requester.SettingReadonly = false;
		this.lb_Requester.ShowUnderLine = true;
		this.lb_Requester.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Requester.Utext = "Requester";
		this.look_ReqType.EditControl = null;
		this.look_ReqType.FieldName = null;
		this.look_ReqType.IsSearchControl = true;
		resources.ApplyResources(this.look_ReqType, "look_ReqType");
		this.look_ReqType.Name = "look_ReqType";
		this.look_ReqType.Properties.Appearance.Font = (System.Drawing.Font)resources.GetObject("look_ReqType.Properties.Appearance.Font");
		this.look_ReqType.Properties.Appearance.Options.UseFont = true;
		this.look_ReqType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("look_ReqType.Properties.Buttons"))
		});
		this.look_ReqType.Properties.NullText = resources.GetString("look_ReqType.Properties.NullText");
		this.look_ReqType.Properties.PopupSizeable = false;
		this.look_ReqType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.look_ReqType.SettingInVisible = false;
		this.look_ReqType.SettingReadonly = false;
		resources.ApplyResources(this.btn_CheckPromotion, "btn_CheckPromotion");
		this.btn_CheckPromotion.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_CheckPromotion.Appearance.Options.UseBackColor = true;
		this.btn_CheckPromotion.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_CheckPromotion.EditControl = null;
		this.btn_CheckPromotion.EditValue = "Check promotion";
		this.btn_CheckPromotion.FieldName = null;
		this.btn_CheckPromotion.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CheckPromotion.ImageOptions.Image");
		this.btn_CheckPromotion.IsSearchControl = false;
		this.btn_CheckPromotion.Name = "btn_CheckPromotion";
		this.btn_CheckPromotion.ReadOnly = false;
		this.btn_CheckPromotion.SettingInVisible = false;
		this.btn_CheckPromotion.SettingReadonly = false;
		this.btn_CheckPromotion.Click += new System.EventHandler(this.btn_CheckPromotion_Click);
		resources.ApplyResources(this.btn_ClearPromotion, "btn_ClearPromotion");
		this.btn_ClearPromotion.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_ClearPromotion.Appearance.Options.UseBackColor = true;
		this.btn_ClearPromotion.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_ClearPromotion.EditControl = null;
		this.btn_ClearPromotion.EditValue = "Clear promotion";
		this.btn_ClearPromotion.FieldName = null;
		this.btn_ClearPromotion.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ClearPromotion.ImageOptions.Image");
		this.btn_ClearPromotion.IsSearchControl = false;
		this.btn_ClearPromotion.Name = "btn_ClearPromotion";
		this.btn_ClearPromotion.ReadOnly = false;
		this.btn_ClearPromotion.SettingInVisible = false;
		this.btn_ClearPromotion.SettingReadonly = false;
		this.btn_ClearPromotion.Click += new System.EventHandler(this.btn_ClearPromotion_Click);
		resources.ApplyResources(this.btn_Save, "btn_Save");
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "_ _ _";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.ItemId = null;
		this.btn_Save.MenuManager = this.barManager1;
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Click += new System.EventHandler(this.btnAdd_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btn_ClearPromotion);
		base.Controls.Add(this.btn_CheckPromotion);
		base.Controls.Add(this.txtRequester);
		base.Controls.Add(this.lb_Requester);
		base.Controls.Add(this.split_Main);
		base.Controls.Add(this.drlCopyTo);
		base.Controls.Add(this.drlCopyFrom);
		base.Controls.Add(this.cbxStatus);
		base.Controls.Add(this.txtBusinessPartner);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_BusinessPartner);
		base.Controls.Add(this.lb_DocNumber);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtNoOfTrans);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.look_ReqType);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormId = "oDocument";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("VirFormDocument.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "VirFormDocument";
		base.ShowInTaskbar = false;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirFormDocument_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.split_Main).EndInit();
		this.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.ctxDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Series).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtCardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReqType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Requester).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx26).EndInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_OwnerCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rInfoPriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CtlAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceMode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlupContactPersonCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PmtToDraw).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_LineBatchAndSerial).EndInit();
		this.split_LineBatchAndSerial.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Line).EndInit();
		this.xtraTab_Line.ResumeLayout(false);
		this.tbDocumentLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		this.ctxLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtItemcode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCatalog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLookUpVatGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEnSetCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxtWhseCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_CtlAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePackQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LstPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightStock).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUseBaseUnits).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UnitsOfMeasurment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_InventoryQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineSlpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VendorNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_NoInvtryMv).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryOrg).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbxTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FreightTaxGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit9).EndInit();
		this.tab_PaymentTerms.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cal_TotalFC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Count.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdPaymentTerms).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvPayTerms).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).EndInit();
		this.tab_Logistic.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLogistic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Logistic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rShipTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBillTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rConfirmed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLogisticsBpChannel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLogisticBpChannelCntc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PartSupply).EndInit();
		this.tab_Freight.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Freight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Freight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ExpenseCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GroupTax).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DistriMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StatusFreight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stock).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LstPchPrce).EndInit();
		this.tabP_Accounting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Accounting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Accounting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Accounting_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_GroupNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_PaymentMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_Indicator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Accounting_PayDuMonth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_UseShpdGd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReferencedDocument).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRequester.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ReqType.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
