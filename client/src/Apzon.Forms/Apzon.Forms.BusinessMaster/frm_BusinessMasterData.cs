using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.Banking;
using Apzon.Forms.Administrator.Business_Partners;
using Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Financials;
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources;
using Apzon.Forms.Interfaces.BusinessPartner;
using Apzon.Forms.Inventory.PriceLists;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.Buttons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.DataProcessing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.BusinessMaster;

public class frm_BusinessMasterData : XtraFormEx
{
	public int _cntCode;

	public int _defaultShipAddrHandle;

	public int _defaultBillAddrHandle;

	public string _copyF_CodeCustomer;

	public DataTable dataFromQuickCreationBMD = new DataTable();

	private bool isLoadDataFromQuickCreationBMD;

	public bool isAddBMDComplete;

	public Action<bool, string> RefreshQuickCreation;

	public bool isExistsCardCode;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private Dictionary<string, BaseEdit> _dicContactUdfControl = new Dictionary<string, BaseEdit>();

	private string _oldCardCode = "";

	private string _picFilePath = "";

	private string _defaultBillAddress = "";

	private string _defaultShipAddress = "";

	private string _dfltContact = "";

	private string _oldCardType = "";

	private string _curPictureName = "";

	private string _oldValueSeries = "";

	private string _cloudImage = "";

	private bool _picFocus;

	private string[] _arrCurrencyType = new string[3] { "Local Currency", "System Currency", "BP Currency" };

	private List<UserFieldsImpl> _lstUdfFields;

	private bool _isLoading;

	private bool _isAskPaymentTerm = true;

	private bool _isForeignCurrency;

	private bool _isSettingCardGroup;

	private bool _isSettingHousBnkAct;

	private bool _isSetting_HsBnkIban;

	private bool _isSettingDateValidFrom;

	private bool _isSettingDateFrozenFrom;

	private bool _isSettingDateValidTo;

	private bool _isSettingDateFrozenTo;

	private bool _isSettingValidComments;

	private bool _isSettingFrozenComments;

	private bool _isSettingFormDataSource;

	private List<Common.Table_Code> _listCurrencyCode;

	private List<Common.Table_Code> _listCreditCardType;

	private Dictionary<string, Control> _dicControls;

	private DataTable _dtBpBankAccount = new DataTable();

	private DataTable _dtPaymentDates = new DataTable();

	private DataTable _dtBpGroup = new DataTable();

	private DataTable _dtOriginal = new DataTable();

	private DataTable _dtBpControlAccounts = new DataTable();

	private DataTable _dtBpCtlAccountsStruct = new DataTable();

	private DataTable _dtBranchAssignment = new DataTable();

	private List<DataTable> _lstStructDt = new List<DataTable>();

	private short _oldSeries;

	private List<string> _lstManualBindControl = new List<string>();

	private int _bankKey;

	private int[] _allowChangeBpTypeObjType = new int[6] { 17, 23, 22, 540000006, 1470000113, 97 };

	private DataTable _dtStates;

	private List<RepositoryItemLookUpEdit> _listLukStates = new List<RepositoryItemLookUpEdit>();

	private readonly RepositoryItemLookUpEdit _defaultValueEdit = new RepositoryItemLookUpEdit();

	private CalcEditEx calc_Balance = new CalcEditEx();

	private CalcEditEx calc_DNotesBal = new CalcEditEx();

	private CalcEditEx calc_OrdersBal = new CalcEditEx();

	private CalcEditEx calc_BalanceFC = new CalcEditEx();

	private CalcEditEx calc_DNoteBalFC = new CalcEditEx();

	private CalcEditEx calc_OrderBalFC = new CalcEditEx();

	private CalcEditEx calc_BalanceSY = new CalcEditEx();

	private CalcEditEx calc_DNoteBalSY = new CalcEditEx();

	private CalcEditEx calc_OrderBalSY = new CalcEditEx();

	private CalcEditEx calc_OprCount = new CalcEditEx();

	private LabelLine lbl_AccountBalance = new LabelLine();

	private LabelLine lbl_Deliveries = new LabelLine();

	private LabelLine lbl_Orders = new LabelLine();

	private LabelLine lbl_Opportunities = new LabelLine();

	private Panel pnDisplayBalance = new Panel();

	private bool _cardTypeChanged;

	private bool _isProcessing;

	private IContainer components;

	private LabelLine lb_Code;

	private LabelLine lb_ForeignName;

	private LabelLine lb_Name;

	private LabelLine lb_Group;

	private LabelLine lb_FederalTaxID;

	private LabelLine lb_Currency;

	private TextEditEx txt_CardCode;

	private TextEditEx txtForeignName;

	private TextEditEx txt_CardName;

	private TextEditEx txtFederalTaxID;

	private ContextMenuStrip ctMenu_Contact;

	private ToolStripMenuItem toolStripMenuItemDelete;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btAdd;

	private XtraTabControlEx xTab_BP;

	private XtraTabPageEx tab_General;

	private LabelLine lb_SalesEmpLoyee;

	private LabelLine lb_ShippingType;

	private TextEditEx txtGeneralFederalTaxID;

	private LabelLine lb_Remark;

	private LabelLine lb_UnifiedFederalTaxID;

	private TextEditEx txtGeneralContactPerson;

	private TextEditEx txtGeneralIDNo2;

	private TextEditEx txtGeneralPassword;

	private LabelLine lb_ContactP;

	private LabelLine lb_IDno2;

	private LabelLine lb_Passw;

	private TextEditEx txtGeneralEmail;

	private TextEditEx txtGeneralWebsite;

	private TextEditEx txtGeneralFax;

	private LabelLine lb_Email;

	private LabelLine lb_Website;

	private LabelLine lb_Fax;

	private TextEditEx txtGeneralTel2;

	private TextEditEx txtGeneralMobilePhone;

	private TextEditEx txtGeneralTel1;

	private LabelLine lb_Tel2;

	private LabelLine lb_MobiP;

	private LabelLine lb_Tel1;

	private XtraTabPageEx tab_PaymentTerms;

	private DateEditEx datePaymentExpirationDate;

	private TextEditEx txtPaymentAvarageDelay;

	private LabelLine lb_AvarageDelay;

	private TextEditEx txtPaymentIDNumber;

	private LabelLine lb_IDNumber;

	private LabelLine lb_ExpirationDate;

	private TextEditEx txtPaymentCreditCardNo;

	private LabelLine lb_CreditCardNo;

	private LabelLine lb_CreditCardType;

	private TextEditEx txt_DflSwift;

	private LabelLine lb_DflSwift;

	private TextEditEx txt_BPBankCode;

	private TextEditEx txt_BPBankAccount;

	private LabelLine lb_BankCode;

	private LabelLine lb_Account;

	private LabelLine lb_BankName;

	private LabelLine lb_Commitment;

	private LabelLine lb_BankCountry;

	private LabelLine lb_CreditL;

	private LabelLine lb_Interest;

	private LabelLine lb_TotalDis;

	private LabelLine lb_PriceLst;

	private LabelLine lb_PaymentTerms;

	private CalcEditEx txtPaymentTotalDiscount;

	private CalcEditEx txtPaymentInterestonArrears;

	private CalcEditEx txtPaymentCreditLimit;

	private CalcEditEx txtPaymentCommitmentLimit;

	private XtraTabPageEx tab_ContactPerson;

	private SimpleButtonEx btContactPersonDefault;

	private DateEditEx date_BirthDate;

	private CheckEditEx chk_Active;

	private ComboBoxEditEx cb_Gender;

	private LabelLine lb_CityOfB;

	private TextEditEx txt_Profession;

	private TextEditEx txt_BirthCity;

	private LabelLine lb_Profession;

	private LabelLine lb_Gender;

	private LabelLine lb_DateOfB;

	private TextEditEx txt_Password;

	private LabelLine lb_PlaceOfB;

	private LabelLine lb_PassWord;

	private TextEditEx txt_Notes1;

	private TextEditEx txt_Notes2;

	private TextEditEx txt_Pager;

	private LabelLine lb_Remark1;

	private LabelLine lb_Remark2;

	private LabelLine lb_Pager;

	private TextEditEx txt_Fax;

	private TextEditEx txt_E_MailL;

	private TextEditEx txt_Cellolar;

	private TextEditEx txt_Tel1;

	private TextEditEx txt_Tel2;

	private TextEditEx txt_Address;

	private TextEditEx txt_Title;

	private TextEditEx txt_Position;

	private TextEditEx txt_Name;

	private GridControlEx grd_Contact;

	private GridViewEx gv_Contact;

	private GridColumnEx gcName;

	private GridColumnEx CntctCode;

	private LabelLine lb_EmailCnt;

	private LabelLine lb_FaxCnt;

	private LabelLine lb_MobiPhone;

	private LabelLine lb_TeleP2;

	private LabelLine lb_TeleP1;

	private LabelLine lb_Address;

	private LabelLine lb_Title;

	private LabelLine lb_Position;

	private LabelLine lb_NameCnt;

	private XtraTabPageEx tab_Remark;

	private MemoEditEx memoRemarks;

	private SimpleButtonEx btnRemarkBrowserImage;

	private PictureEdit picRemarks;

	private XtraTabPageEx tab_UserDefineF;

	private GridControlEx grd_Udf;

	private GridViewEx gv_Udf;

	private XtraTabPageEx tab_Address;

	private GridControlEx grd_Address;

	private GridViewEx gv_Address;

	private GridColumnEx gc_Address_Address;

	private GridColumnEx gc_Address_CardCode;

	private GridColumnEx gc_Address_AdresType;

	private RepositoryItemLookUpEdit rep_Address_AdresType;

	private GridColumnEx gc_Address_Street;

	private GridColumnEx gc_Address_StreetNo;

	private GridColumnEx gc_Address_Block;

	private GridColumnEx gc_Address_City;

	private GridColumnEx gc_Address_ZipCode;

	private GridColumnEx gc_Address_County;

	private GridColumnEx gc_Address_State;

	private GridColumnEx gc_Address_Country;

	private GridColumnEx gc_Address_Building;

	private GridColumnEx gc_Address_GlblLocNum;

	private ContextMenuStrip contextMenuStrip_Address;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private XtraTabPageEx tab_Properties;

	private SimpleButtonEx btn_PropertiesClearSelect;

	private SimpleButtonEx btn_PropertiesSellectAll;

	private GridControlEx grd_Properties;

	private GridViewEx gv_Properties;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemCheckEdit rep_PropertiesCheck;

	private XtraTabPageEx tab_AttachmentFiles;

	private SimpleButtonEx btn_PicDelete;

	private TextEditEx txt_BPBankBranch;

	private TextEditEx txt_BPBankAccountName;

	private TextEditEx txt_BPBankIBAN;

	private TextEditEx txt_BPBankControlKey;

	private TextEditEx txt_BPBankMandateID;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine10;

	private TextEditEx txt_PaymentDefaultIBAN;

	private LabelLine labelLine9;

	private LabelLine labelLine4;

	private CheckEditEx chk_AcceptsEndorsedChecks;

	private CheckEditEx chk_EndorsableChecksFrom;

	private CheckEditEx chk_DoNotApplyDiscountGroup;

	private CheckEditEx chk_AllowPartialDlvrPerRow;

	private CheckEditEx chk_AllowPartialDlvrOfSO;

	private SimpleButtonEx btn_PaymentDates;

	private LabelLine labelLine11;

	private RepositoryItemLookUpEdit rep_Address_Country;

	private DateEditEx txt_BPBankDateOfSignature;

	private Panel panel_Advanced;

	private TextEditEx txtFrozenComm2;

	private TextEditEx txtValidComm2;

	private LabelLine lb_ToInacti;

	private LabelLine lb_ToActi;

	private LabelLine lb_RIacti;

	private LabelLine lb_RActi;

	private LabelLine lb_InactiveRangeFrom;

	private DateEditEx dateFrozenTo2;

	private LabelLine lb_ActiveRangeFrom;

	private DateEditEx dateFrozenFrom2;

	private DateEditEx dateValidTo2;

	private DateEditEx dateValidFrom2;

	private Panel panel_Inactive;

	private TextEditEx txtFrozenComm;

	private LabelLine lb_T;

	private LabelLine lb_R;

	private LabelLine lb_F;

	private DateEditEx dateFrozenTo;

	private DateEditEx dateFrozenFrom;

	private Panel panel_Active;

	private LabelLine lbVailFrom;

	private LabelLine lbRemarks;

	private DateEditEx dateValidFrom;

	private TextEditEx txtValidComm;

	private DateEditEx dateValidTo;

	private LabelLine lbVailTo;

	private RadioGroupEx rdg_Active;

	private ButtonEditEx bte_BPProject;

	private TextEditEx txt_FactoringIndicator;

	private ButtonEditEx bte_FactoringIndicator;

	private LabelLine labelLine12;

	private LabelLine labelLine15;

	private LabelLine labelLine14;

	private LabelLine labelLine13;

	private TextEditEx txt_GeneralAliasName;

	private LabelLine labelLine16;

	private TextEditEx txt_GLN;

	private LabelLine labelLine21;

	private LabelLine lbl_Language;

	private LabelLine labelLine19;

	private LabelLine labelLine17;

	private LabelLine labelLine18;

	private ButtonEditEx bte_BPChannelCode;

	private ContextMenuStrip contextMenuStrip_BP;

	private ToolStripMenuItem removeBusinessPartnerToolStripMenuItem;

	private SimpleButtonEx btn_AddressDefault;

	private ComboBoxEditEx cb_CurrencyType;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ucAttachment ucAttachment1;

	private LookUpEditCombo luk_Series;

	private LookUpEditCombo luk_CardType;

	private LookUpEditCombo luk_CardGroup;

	private LookUpEditCombo luk_ShippingType;

	private LookUpEditCombo luk_SalesBuyer;

	private LookUpEditCombo luk_CreditCardType;

	private LookUpEditCombo luk_PaymentTerm;

	private LookUpEditCombo luk_Currency;

	private LookUpEditCombo luk_PaymentPriceList;

	private LookUpEditCombo txt_BirthPlace;

	private LookUpEditCombo luk_PaymentTermPriority;

	private LookUpEditCombo luk_EffectiveDiscount;

	private LookUpEditCombo luk_BPBankName;

	private LookUpEditCombo bte_BPBankCountry;

	private LookUpEditCombo luk_BusinessType;

	private LookUpEditCombo luk_Language;

	private LookUpEditCombo luk_BPIndustry;

	private LookUpEditCombo luk_Territory;

	private LookUpEditCombo luk_Technician;

	private XtraTabPageEx tab_Accounting;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Accounting_general;

	private SimpleButtonEx btEx_ControlAccts;

	private LabelLine labelLine8;

	private XtraTabPageEx tab_Accounting_tax;

	private LabelLine lbl_DebPayAcct;

	private LabelLine labelLine24;

	private LabelLine labelLine23;

	private LabelLine labelLine26;

	private LabelLine lbl_ConnBP;

	private LookUpEditCombo luk_VatStatus;

	private LabelLine labelLine27;

	private LabelLine labelLine28;

	private LabelLine lbl_DunnDate;

	private LabelLine lbl_DunnLevel;

	private DateEditEx dateX_DunnDate;

	private LabelLine lbl_ConnBPName;

	private LabelLine lbl_ConSolidBPName;

	private LabelLine lbl_InterimAcctName;

	private LabelLine lbl_ClearAcctName;

	private LabelLine lbl_AcctPayableName;

	private LabelLine lbl_ControlAccts;

	private ucButtonCFLImage uc_BCFLImg_ShippingType;

	private ucButtonCFLImage uc_BCFLImg_Language;

	private ucButtonCFLImage uc_BCFLImg_SalesBuyer;

	private LabelLine lbOwner;

	private ButtonEditCFL bteCFL_OwnerCode;

	private XtraTabPageEx tab_PaymentRun;

	private LabelLine labelLine25;

	private PanelControl panelControl1;

	private LabelLine labelLine29;

	private PanelControl panelControl2;

	private LabelLine labelLine36;

	private LabelLine labelLine35;

	private LabelLine labelLine34;

	private LabelLine labelLine33;

	private LabelLine labelLine32;

	private LabelLine labelLine31;

	private LabelLine labelLine30;

	private LookUpEditCombo luk_HsBnkIBAN;

	private LookUpEditCombo luk_HousBnkAct;

	private LookUpEditCombo luk_HouseBank;

	private LookUpEditCombo luk_HousBnkCry;

	private LabelLine labelLine37;

	private LabelLine labelLine38;

	private LookUpEditCombo luk_BCACode;

	private RadioGroupEx rdg_FatherType;

	private ButtonEditEx bte_FatherCard;

	private ButtonEditEx bte_DebPayAcct;

	private ButtonEditEx bte_DpmIntAct;

	private ButtonEditEx bte_DpmClear;

	private TextEditEx txt_PlngGroup;

	private ButtonEditEx bte_ConnBP;

	private CheckEditEx chk_Affiliate;

	private CheckEditEx chk_DeferrTax;

	private ButtonEditEx bte_ECVatGroup;

	private TextEditEx txt_ECVatGroupName;

	private CalcEditEx calc_DunnLevel;

	private CheckEditEx chk_UseShpdGd;

	private CheckEditEx chk_BlockDunn;

	private TextEditEx txt_HousBnkBrn;

	private TextEditEx txt_HsBnkSwift;

	private TextEditEx txt_HousCtlKey;

	private TextEditEx txt_RefDetails;

	private CheckEditEx chk_CollecAuth;

	private CheckEditEx chk_AutoCalBCG;

	private MemoEditEx txtGeneralRemark;

	private SimpleButtonEx btn_BranchAssignment;

	private LabelLine lbl_BranchAssignment;

	private RepositoryItemLookUpEdit rep_States;

	private ToolStripMenuItem dupplicateToolStripMenuItem;

	private string _defaultContact
	{
		get
		{
			return _dfltContact;
		}
		set
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				_dfltContact = value;
				txtGeneralContactPerson.Text = value;
				if (FormMode == Enums.FormViewModeType.Find)
				{
					txtGeneralContactPerson.ReadOnly = false;
				}
				else
				{
					txtGeneralContactPerson.ReadOnly = true;
				}
			}));
		}
	}

	public frm_BusinessMasterData()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BusinessMasterData"))
		{
			InitializeComponent();
			IsScrollable = true;
			OnLoadAsync += frm_BusinessMasterData_Load;
			SettingLanguage();
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btAdd.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btAdd.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			Controls.EnableDisableControl(FormAuth == Enums.FormAuthorities.Full);
			btCancel.Enabled = true;
			btAdd.Enabled = true;
			btAdd.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btAdd.Text = Resources.ButtonFind;
			break;
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string oldCardCode = _oldCardCode;
				Enums.ChangeLog changeLog = Enums.ChangeLog.clBusinessPartners;
				Apzon.Entities.Models.Common.GetChangeLogParams getChangeLogParams = new Apzon.Entities.Models.Common.GetChangeLogParams();
				getChangeLogParams.PrimaryKey = oldCardCode;
				getChangeLogParams.Object = changeLog;
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

	private async Task ClearUDF()
	{
		DataTable dataSourceTable = grd_Udf.DataSourceTable;
		if (!dataSourceTable.IsNotNull())
		{
			return;
		}
		IEnumerable<string> enumerable = from DataColumn c in dataSourceTable.Columns
			where c.ColumnName.StartsWith("U_")
			select c.ColumnName;
		foreach (string item in enumerable)
		{
			dataSourceTable.Columns[item].AllowDBNull = true;
		}
		foreach (string item2 in enumerable)
		{
			dataSourceTable.Rows[0][item2] = DBNull.Value;
		}
		dataSourceTable.AcceptChanges();
		grd_Udf.RefreshDataSource();
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) && !_isLoading)
			{
				pnDisplayBalance.Visible = false;
				_dtBpControlAccounts = _dtBpCtlAccountsStruct.Clone();
				FormMode = Enums.FormViewModeType.Find;
				Controls.EnableDisableControl(isEnable: true);
				ResetBmdControls();
				ClearUDF();
				_isAskPaymentTerm = false;
				_isLoading = true;
				txt_CardCode.ReadOnly = false;
				txt_CardCode.Enabled = true;
				txt_CardCode.Focus();
				luk_Series.ReadOnly = true;
				SetControlsBackColor();
				luk_CardType.EditValue = null;
				luk_BPType_EditValueChanged(null, null);
				luk_CardGroup.EditValue = null;
				picRemarks.EditValue = null;
				ChangeAddressType(Function.ToString(luk_CardType.EditValue));
				cb_CurrencyType.Visible = false;
				luk_Currency.Properties.DataSource = _listCurrencyCode.ToList();
				_bankKey = 0;
				_isLoading = false;
				_isAskPaymentTerm = true;
				txtGeneralContactPerson.ReadOnly = false;
				_dtBranchAssignment = new DataTable();
				btn_BranchAssignment.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				MODE_FIND();
			}
			else
			{
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				_isLoading = true;
				_dtBpControlAccounts = GetDefaultControlAccounts();
				_isAskPaymentTerm = false;
				FormMode = Enums.FormViewModeType.Add;
				grd_Properties.DataSource = null;
				grd_Properties.RefreshDataSource();
				SetControlsBackColor();
				ResetBmdControls();
				_isAskPaymentTerm = false;
				pnDisplayBalance.Visible = false;
				_isLoading = true;
				DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OCRD");
				if (dataTable != null)
				{
					dataTable.Rows.Clear();
					dataTable.AcceptChanges();
					DataTable dataTableNewRow = WinFuntions.GetDataTableNewRow(dataTable.Copy());
					if (dataTableNewRow != null && dataTableNewRow.Columns.Count > 0)
					{
						dataTableNewRow.AcceptChanges();
						DataSource = dataTableNewRow.Copy();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				}
				DataTable dt = new DataTable();
				WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt);
				DataRow dataRow = dt.NewRow();
				dataRow["CntctCode"] = -1;
				dataRow["Name"] = "Add new";
				dt.Rows.Add(dataRow);
				dt.AcceptChanges();
				grd_Contact.DataSource = dt;
				grd_Contact.RefreshDataSource();
				List<Tuple<string, string>> bPProperties = GetBPProperties("-1");
				LoadBPProperties(bPProperties);
				_isLoading = true;
				addRowToolStripMenuItem_Click(null, null);
				luk_Currency.EditValue = GlobalData.MainCurrency;
				luk_ShippingType.EditValue = Convert.ToInt16(-1);
				luk_BusinessType.EditValue = "C";
				luk_BPType_EditValueChanged(null, null);
				picRemarks.EditValue = null;
				luk_CardType.EditValue = "C";
				_isAskPaymentTerm = false;
				luk_PaymentTerm.EditValue = GlobalData.GeneralSetting.DfCustTerm;
				_isAskPaymentTerm = true;
				ChangePaymentTermData();
				txtGeneralContactPerson.ReadOnly = true;
				ChangeAddressType(Function.ToString(luk_CardType.EditValue));
				luk_SalesBuyer.EditValue = Function.ParseInt(GlobalData.GeneralSetting.DfltSlp);
				chk_Active.Checked = true;
				_defaultContact = "";
				cb_CurrencyType.Visible = false;
				luk_Currency.Properties.DataSource = _listCurrencyCode.ToList();
				_isLoading = false;
				_isAskPaymentTerm = true;
				DataTable defaultSeries = _httpClient.Commons.GetDefaultSeries("2", "C", GlobalData.UserID);
				if (defaultSeries.IsNotNull())
				{
					luk_Series.EditValue = Function.ParseShort(defaultSeries.Rows[0][0]);
					luk_DocCode_EditValueChanged(null, null);
				}
				DataTable defaultLanguage = _httpClient.BusinessMaster.GetDefaultLanguage();
				if (defaultLanguage.IsNotNull())
				{
					luk_Language.EditValue = Function.ParseInt(defaultLanguage.Rows[0][0]);
				}
				luk_EffectiveDiscount.EditValue = "L";
				_bankKey = 0;
				chk_UseShpdGd.Checked = GlobalData.GeneralSetting.UseShpdGd == "Y";
				_dtBranchAssignment = new DataTable();
				btn_BranchAssignment.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_FIRST()
	{
		if (!_isLoading)
		{
			_isLoading = true;
			try
			{
				await GetRecord("F", "");
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			_isLoading = false;
		}
	}

	public async Task MOVE_NEXT()
	{
		if (_isLoading)
		{
			return;
		}
		_isLoading = true;
		try
		{
			if (string.IsNullOrWhiteSpace(txt_CardCode.Text) || !CheckDoc())
			{
				await GetRecord("F", "");
			}
			else
			{
				await GetRecord("N", txt_CardCode.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	public async Task MOVE_LAST()
	{
		if (!_isLoading)
		{
			_isLoading = true;
			try
			{
				await GetRecord("L", "");
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			_isLoading = false;
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		if (_isLoading)
		{
			return;
		}
		_isLoading = true;
		try
		{
			if (string.IsNullOrWhiteSpace(txt_CardCode.Text) || !CheckDoc())
			{
				await GetRecord("L", "");
			}
			else
			{
				await GetRecord("P", txt_CardCode.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await GetADoc(txt_CardCode.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_BusinessMasterData_Load(object sender, EventArgs e)
	{
		FormMode = ((!string.IsNullOrWhiteSpace(_copyF_CodeCustomer)) ? Enums.FormViewModeType.View : ((FormAuth != Enums.FormAuthorities.Full) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add));
		try
		{
			AcceptButton = btAdd;
			CancelButton = btCancel;
			bte_BPBankCountry.EditValueChanged += BteBpBankCountryOnEditValueChanged;
			ContextMenuStrip.Opening += ContextMenuStrip_Opening;
			SetBalanceDisplayControls(loadForm: true);
			LabelLine labelLine = lbOwner;
			bool visible = (bteCFL_OwnerCode.Visible = GlobalData.CompanyInfo.DoFilter.Equals("Y") && (GlobalData.GeneralSetting.DoMngMth.Equals("B") || GlobalData.GeneralSetting.DoMngMth.Equals("A")));
			labelLine.Visible = visible;
			if (!GlobalData.IsMultiBranch)
			{
				btn_BranchAssignment.Visible = false;
				lbl_BranchAssignment.Visible = false;
			}
			if (GlobalData.GeneralSetting.MultiLang.ToLower() != "y")
			{
				luk_Language.Visible = false;
				lbl_Language.Visible = false;
				uc_BCFLImg_Language.Visible = false;
			}
			FormId = "OCRD";
			gv_Properties.ItemId = "OCRD_Prop";
			gv_Properties.FormId = FormId;
			gv_Udf.ItemId = "OCRD_Udf";
			gv_Udf.FormId = FormId;
			gv_Address.ItemId = "OCRD_Addr";
			gv_Address.FormId = FormId;
			gv_Contact.ItemId = "OCRD_Cnt";
			gv_Contact.FormId = FormId;
			ucAttachment1.FormId = FormId;
			_lstStructDt = new List<DataTable>
			{
				new DataTable("ATC1"),
				new DataTable("CRD3"),
				new DataTable("OCRD"),
				new DataTable("OCPR"),
				new DataTable("OCRB"),
				new DataTable("CRD5"),
				new DataTable("CRD1")
			};
			for (int i = 0; i < _lstStructDt.Count; i++)
			{
				string tableName = _lstStructDt[i].TableName;
				_lstStructDt[i] = _httpClient.Commons.GetTableStructureWithDefaultValue(_lstStructDt[i].TableName);
				_lstStructDt[i].TableName = tableName;
			}
			await SetInitialControls();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["OCRD"] = gv_Udf,
				["CRD1"] = gv_Address
			};
			await WinFuntions.LoadUdfColumns(gv_Udf, "OCRD", tblFormular);
			await WinFuntions.LoadUdfColumns(gv_Address, "CRD1", tblFormular);
			List<Formular> formulars = await WinFuntions.GetFormulas(2);
			gv_Udf.InitFormular(formulars, tblFormular);
			DataTable formulaSearchByTable = _httpClient.Commons.GetFormulaSearchByTable("CRD1");
			if (formulaSearchByTable.IsNotNull())
			{
				await gv_Address.InitChooseFromList(formulaSearchByTable, tblFormular);
			}
			gv_Address.CustomRowCellEdit += GvAddressOnCustomRowCellEdit;
			_defaultValueEdit.DisplayMember = "Name";
			_defaultValueEdit.ValueMember = "Code";
			_defaultValueEdit.BestFitMode = BestFitMode.BestFitResizePopup;
			_defaultValueEdit.NullText = "";
			_defaultValueEdit.ForceInitialize();
			await WaitLoad();
			SetFormatControlValue();
			_lstManualBindControl.Add("BankCode");
			bteCFL_OwnerCode.Properties.CustomButtonSearchClick = true;
			if (dataFromQuickCreationBMD.IsNotNull())
			{
				await LoadDataFromQickCreationBPMasterData(dataFromQuickCreationBMD);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (_picFocus)
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

	private void BteBpBankCountryOnEditValueChanged(object sender, EventArgs e)
	{
		LoadComboBankByCountry();
	}

	private void SetBalanceDisplayControls(bool loadForm = false)
	{
		try
		{
			if (loadForm)
			{
				lbl_AccountBalance.FieldName = null;
				lbl_AccountBalance.HideWhenEmpty = false;
				lbl_AccountBalance.LineColor = Color.FromArgb(213, 218, 223);
				lbl_AccountBalance.Location = new Point(5, 3);
				lbl_AccountBalance.Name = "lbl_AccountBalance";
				lbl_AccountBalance.ShowUnderLine = true;
				lbl_AccountBalance.Size = new Size(150, 20);
				lbl_AccountBalance.SourceType = Enums.DataSourceType.TittleValue;
				lbl_AccountBalance.TabIndex = 1;
				lbl_AccountBalance.TabStop = false;
				lbl_AccountBalance.Utext = "Account Balance";
				lbl_Deliveries.FieldName = null;
				lbl_Deliveries.HideWhenEmpty = false;
				lbl_Deliveries.LineColor = Color.FromArgb(213, 218, 223);
				lbl_Deliveries.Location = new Point(5, 22);
				lbl_Deliveries.Name = "lbl_Deliveries";
				lbl_Deliveries.ShowUnderLine = true;
				lbl_Deliveries.Size = new Size(150, 20);
				lbl_Deliveries.SourceType = Enums.DataSourceType.TittleValue;
				lbl_Deliveries.TabIndex = 1;
				lbl_Deliveries.TabStop = false;
				lbl_Deliveries.Utext = "Deliveries";
				lbl_Orders.FieldName = null;
				lbl_Orders.HideWhenEmpty = false;
				lbl_Orders.LineColor = Color.FromArgb(213, 218, 223);
				lbl_Orders.Location = new Point(5, 41);
				lbl_Orders.Name = "lbl_Orders";
				lbl_Orders.ShowUnderLine = true;
				lbl_Orders.Size = new Size(150, 20);
				lbl_Orders.SourceType = Enums.DataSourceType.TittleValue;
				lbl_Orders.TabIndex = 1;
				lbl_Orders.TabStop = false;
				lbl_Orders.Utext = "Orders";
				lbl_Opportunities.FieldName = null;
				lbl_Opportunities.HideWhenEmpty = false;
				lbl_Opportunities.LineColor = Color.FromArgb(213, 218, 223);
				lbl_Opportunities.Location = new Point(5, 60);
				lbl_Opportunities.Name = "lbl_Opportunities";
				lbl_Opportunities.ShowUnderLine = true;
				lbl_Opportunities.Size = new Size(150, 20);
				lbl_Opportunities.SourceType = Enums.DataSourceType.TittleValue;
				lbl_Opportunities.TabIndex = 1;
				lbl_Opportunities.TabStop = false;
				lbl_Opportunities.Utext = "Opportunities";
				calc_Balance.Properties.Buttons.RemoveAt(0);
				calc_Balance.FieldName = "Balance";
				calc_Balance.Location = new Point(153, 2);
				calc_Balance.Name = "calc_Balance";
				calc_Balance.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_Balance.Properties.Appearance.Options.UseFont = true;
				calc_Balance.Properties.AutoHeight = false;
				calc_Balance.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_Balance.Properties.DisplayFormat.FormatString = "n0";
				calc_Balance.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_Balance.Properties.EditFormat.FormatString = "n0";
				calc_Balance.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_Balance.Properties.Mask.MaskType = MaskType.None;
				calc_Balance.Properties.Precision = 0;
				calc_Balance.Size = new Size(206, 18);
				calc_Balance.TabIndex = 5;
				calc_BalanceFC.Properties.Buttons.RemoveAt(0);
				calc_BalanceFC.FieldName = "BalanceFC";
				calc_BalanceFC.Location = new Point(153, 2);
				calc_BalanceFC.Name = "calc_BalanceFC";
				calc_BalanceFC.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_BalanceFC.Properties.Appearance.Options.UseFont = true;
				calc_BalanceFC.Properties.AutoHeight = false;
				calc_BalanceFC.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_BalanceFC.Properties.DisplayFormat.FormatString = "n0";
				calc_BalanceFC.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_BalanceFC.Properties.EditFormat.FormatString = "n0";
				calc_BalanceFC.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_BalanceFC.Properties.Mask.MaskType = MaskType.None;
				calc_BalanceFC.Properties.Precision = 0;
				calc_BalanceFC.Size = new Size(206, 18);
				calc_BalanceFC.TabIndex = 6;
				calc_BalanceSY.Properties.Buttons.RemoveAt(0);
				calc_BalanceSY.FieldName = "BalanceSY";
				calc_BalanceSY.Location = new Point(153, 2);
				calc_BalanceSY.Name = "calc_BalanceSY";
				calc_BalanceSY.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_BalanceSY.Properties.Appearance.Options.UseFont = true;
				calc_BalanceSY.Properties.AutoHeight = false;
				calc_BalanceSY.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_BalanceSY.Properties.DisplayFormat.FormatString = "n0";
				calc_BalanceSY.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_BalanceSY.Properties.EditFormat.FormatString = "n0";
				calc_BalanceSY.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_BalanceSY.Properties.Mask.MaskType = MaskType.None;
				calc_BalanceSY.Properties.Precision = 0;
				calc_BalanceSY.Size = new Size(206, 18);
				calc_BalanceSY.TabIndex = 6;
				calc_DNotesBal.Properties.Buttons.RemoveAt(0);
				calc_DNotesBal.FieldName = "DNotesBal";
				calc_DNotesBal.Location = new Point(153, 21);
				calc_DNotesBal.Name = "calc_DNotesBal";
				calc_DNotesBal.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_DNotesBal.Properties.Appearance.Options.UseFont = true;
				calc_DNotesBal.Properties.AutoHeight = false;
				calc_DNotesBal.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_DNotesBal.Properties.DisplayFormat.FormatString = "n0";
				calc_DNotesBal.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_DNotesBal.Properties.EditFormat.FormatString = "n0";
				calc_DNotesBal.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_DNotesBal.Properties.Mask.MaskType = MaskType.None;
				calc_DNotesBal.Properties.Precision = 0;
				calc_DNotesBal.Size = new Size(206, 18);
				calc_DNotesBal.TabIndex = 5;
				calc_DNoteBalFC.Properties.Buttons.RemoveAt(0);
				calc_DNoteBalFC.FieldName = "DNoteBalFC";
				calc_DNoteBalFC.Location = new Point(153, 21);
				calc_DNoteBalFC.Name = "calc_DNoteBalFC";
				calc_DNoteBalFC.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_DNoteBalFC.Properties.Appearance.Options.UseFont = true;
				calc_DNoteBalFC.Properties.AutoHeight = false;
				calc_DNoteBalFC.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_DNoteBalFC.Properties.DisplayFormat.FormatString = "n0";
				calc_DNoteBalFC.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_DNoteBalFC.Properties.EditFormat.FormatString = "n0";
				calc_DNoteBalFC.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_DNoteBalFC.Properties.Mask.MaskType = MaskType.None;
				calc_DNoteBalFC.Properties.Precision = 0;
				calc_DNoteBalFC.Size = new Size(206, 18);
				calc_DNoteBalFC.TabIndex = 6;
				calc_DNoteBalSY.Properties.Buttons.RemoveAt(0);
				calc_DNoteBalSY.FieldName = "DNoteBalSY";
				calc_DNoteBalSY.Location = new Point(153, 21);
				calc_DNoteBalSY.Name = "calc_DNoteBalSY";
				calc_DNoteBalSY.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_DNoteBalSY.Properties.Appearance.Options.UseFont = true;
				calc_DNoteBalSY.Properties.AutoHeight = false;
				calc_DNoteBalSY.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_DNoteBalSY.Properties.DisplayFormat.FormatString = "n0";
				calc_DNoteBalSY.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_DNoteBalSY.Properties.EditFormat.FormatString = "n0";
				calc_DNoteBalSY.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_DNoteBalSY.Properties.Mask.MaskType = MaskType.None;
				calc_DNoteBalSY.Properties.Precision = 0;
				calc_DNoteBalSY.Size = new Size(206, 18);
				calc_DNoteBalSY.TabIndex = 6;
				calc_OrdersBal.Properties.Buttons.RemoveAt(0);
				calc_OrdersBal.FieldName = "OrdersBal";
				calc_OrdersBal.Location = new Point(153, 40);
				calc_OrdersBal.Name = "calc_OrdersBal";
				calc_OrdersBal.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_OrdersBal.Properties.Appearance.Options.UseFont = true;
				calc_OrdersBal.Properties.AutoHeight = false;
				calc_OrdersBal.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_OrdersBal.Properties.DisplayFormat.FormatString = "n0";
				calc_OrdersBal.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_OrdersBal.Properties.EditFormat.FormatString = "n0";
				calc_OrdersBal.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_OrdersBal.Properties.Mask.MaskType = MaskType.None;
				calc_OrdersBal.Properties.Precision = 0;
				calc_OrdersBal.Size = new Size(206, 18);
				calc_OrdersBal.TabIndex = 5;
				calc_OrderBalFC.Properties.Buttons.RemoveAt(0);
				calc_OrderBalFC.FieldName = "OrderBalFC";
				calc_OrderBalFC.Location = new Point(153, 40);
				calc_OrderBalFC.Name = "calc_OrderBalFC";
				calc_OrderBalFC.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_OrderBalFC.Properties.Appearance.Options.UseFont = true;
				calc_OrderBalFC.Properties.AutoHeight = false;
				calc_OrderBalFC.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_OrderBalFC.Properties.DisplayFormat.FormatString = "n0";
				calc_OrderBalFC.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_OrderBalFC.Properties.EditFormat.FormatString = "n0";
				calc_OrderBalFC.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_OrderBalFC.Properties.Mask.MaskType = MaskType.None;
				calc_OrderBalFC.Properties.Precision = 0;
				calc_OrderBalFC.Size = new Size(206, 18);
				calc_OrderBalFC.TabIndex = 6;
				calc_OrderBalSY.Properties.Buttons.RemoveAt(0);
				calc_OrderBalSY.FieldName = "OrderBalSY";
				calc_OrderBalSY.Location = new Point(153, 40);
				calc_OrderBalSY.Name = "calc_OrderBalSY";
				calc_OrderBalSY.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_OrderBalSY.Properties.Appearance.Options.UseFont = true;
				calc_OrderBalSY.Properties.AutoHeight = false;
				calc_OrderBalSY.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_OrderBalSY.Properties.DisplayFormat.FormatString = "n0";
				calc_OrderBalSY.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_OrderBalSY.Properties.EditFormat.FormatString = "n0";
				calc_OrderBalSY.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_OrderBalSY.Properties.Mask.MaskType = MaskType.None;
				calc_OrderBalSY.Properties.Precision = 0;
				calc_OrderBalSY.Size = new Size(206, 18);
				calc_OrderBalSY.TabIndex = 6;
				calc_OprCount.Properties.Buttons.RemoveAt(0);
				calc_OprCount.FieldName = "OprCount";
				calc_OprCount.Location = new Point(153, 59);
				calc_OprCount.Name = "calc_OprCount";
				calc_OprCount.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				calc_OprCount.Properties.Appearance.Options.UseFont = true;
				calc_OprCount.Properties.AutoHeight = false;
				calc_OprCount.Properties.Buttons.AddRange(new EditorButton[1]
				{
					new EditorButton(ButtonPredefines.Combo)
				});
				calc_OprCount.Properties.DisplayFormat.FormatString = "n0";
				calc_OprCount.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calc_OprCount.Properties.EditFormat.FormatString = "n0";
				calc_OprCount.Properties.EditFormat.FormatType = FormatType.Numeric;
				calc_OprCount.Properties.Mask.MaskType = MaskType.None;
				calc_OprCount.Properties.Precision = 0;
				calc_OprCount.Size = new Size(206, 18);
				calc_OprCount.TabIndex = 5;
				pnDisplayBalance.Controls.Add(calc_Balance);
				pnDisplayBalance.Controls.Add(calc_DNotesBal);
				pnDisplayBalance.Controls.Add(calc_OrdersBal);
				pnDisplayBalance.Controls.Add(calc_BalanceFC);
				pnDisplayBalance.Controls.Add(calc_DNoteBalFC);
				pnDisplayBalance.Controls.Add(calc_OrderBalFC);
				pnDisplayBalance.Controls.Add(calc_BalanceSY);
				pnDisplayBalance.Controls.Add(calc_DNoteBalSY);
				pnDisplayBalance.Controls.Add(calc_OrderBalSY);
				pnDisplayBalance.Controls.Add(calc_OprCount);
				pnDisplayBalance.Controls.Add(lbl_AccountBalance);
				pnDisplayBalance.Controls.Add(lbl_Deliveries);
				pnDisplayBalance.Controls.Add(lbl_Orders);
				pnDisplayBalance.Controls.Add(lbl_Opportunities);
				pnDisplayBalance.Location = new Point(494, 21);
				pnDisplayBalance.Name = "pnDisplayBalance";
				pnDisplayBalance.Size = new Size(369, 84);
				pnDisplayBalance.TabIndex = 0;
				pnDisplayBalance.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				ScrollEx.Controls.Add(pnDisplayBalance);
			}
			else
			{
				switch (Function.ToString(luk_CardType.EditValue).ToUpper())
				{
				case "C":
					lbl_AccountBalance.Visible = true;
					lbl_Deliveries.Visible = true;
					lbl_Deliveries.Utext = "Deliveries";
					lbl_Orders.Visible = true;
					lbl_Orders.Utext = "Orders";
					lbl_Opportunities.Visible = true;
					calc_OprCount.Visible = true;
					VisibleInvisibleBalanceControl("C");
					break;
				case "L":
					lbl_AccountBalance.Visible = false;
					lbl_Deliveries.Visible = false;
					lbl_Orders.Visible = true;
					lbl_Opportunities.Visible = true;
					calc_OprCount.Visible = true;
					VisibleInvisibleBalanceControl("L");
					break;
				case "S":
					lbl_AccountBalance.Visible = true;
					lbl_Deliveries.Visible = true;
					lbl_Deliveries.Utext = "Goods Receipt POs";
					lbl_Orders.Visible = true;
					lbl_Orders.Utext = "Purchase Orders";
					lbl_Opportunities.Visible = false;
					calc_OprCount.Visible = false;
					VisibleInvisibleBalanceControl("S");
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void VisibleInvisibleBalanceControl(string v)
	{
		try
		{
			switch (v)
			{
			case "C":
			case "S":
				switch (cb_CurrencyType.SelectedIndex)
				{
				case 0:
					calc_Balance.Visible = true;
					calc_DNotesBal.Visible = true;
					calc_OrdersBal.Visible = true;
					calc_BalanceFC.Visible = false;
					calc_DNoteBalFC.Visible = false;
					calc_OrderBalFC.Visible = false;
					calc_BalanceSY.Visible = false;
					calc_DNoteBalSY.Visible = false;
					calc_OrderBalSY.Visible = false;
					break;
				case 1:
					calc_Balance.Visible = false;
					calc_DNotesBal.Visible = false;
					calc_OrdersBal.Visible = false;
					calc_BalanceFC.Visible = false;
					calc_DNoteBalFC.Visible = false;
					calc_OrderBalFC.Visible = false;
					calc_BalanceSY.Visible = true;
					calc_DNoteBalSY.Visible = true;
					calc_OrderBalSY.Visible = true;
					break;
				case 2:
					calc_Balance.Visible = false;
					calc_DNotesBal.Visible = false;
					calc_OrdersBal.Visible = false;
					calc_BalanceFC.Visible = true;
					calc_DNoteBalFC.Visible = true;
					calc_OrderBalFC.Visible = true;
					calc_BalanceSY.Visible = false;
					calc_DNoteBalSY.Visible = false;
					calc_OrderBalSY.Visible = false;
					break;
				}
				break;
			case "L":
				switch (cb_CurrencyType.SelectedIndex)
				{
				case 0:
					calc_Balance.Visible = false;
					calc_DNotesBal.Visible = false;
					calc_OrdersBal.Visible = true;
					calc_BalanceFC.Visible = false;
					calc_DNoteBalFC.Visible = false;
					calc_OrderBalFC.Visible = false;
					calc_BalanceSY.Visible = false;
					calc_DNoteBalSY.Visible = false;
					calc_OrderBalSY.Visible = false;
					break;
				case 1:
					calc_Balance.Visible = false;
					calc_DNotesBal.Visible = false;
					calc_OrdersBal.Visible = false;
					calc_BalanceFC.Visible = false;
					calc_DNoteBalFC.Visible = false;
					calc_OrderBalFC.Visible = false;
					calc_BalanceSY.Visible = false;
					calc_DNoteBalSY.Visible = false;
					calc_OrderBalSY.Visible = true;
					break;
				case 2:
					calc_Balance.Visible = false;
					calc_DNotesBal.Visible = false;
					calc_OrdersBal.Visible = false;
					calc_BalanceFC.Visible = false;
					calc_DNoteBalFC.Visible = false;
					calc_OrderBalFC.Visible = true;
					calc_BalanceSY.Visible = false;
					calc_DNoteBalSY.Visible = false;
					calc_OrderBalSY.Visible = false;
					break;
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadControlToDataSource(dynamic control)
	{
		try
		{
			foreach (dynamic item in control.Controls)
			{
				if (item.GetType().GetProperty("FieldName") != null)
				{
					try
					{
						ExtensionMethods.AddControllProcessSource(item, item.FieldName);
					}
					catch (Exception)
					{
					}
				}
				if (item is XtraTabControl)
				{
					foreach (dynamic item2 in item.TabPages)
					{
						if (item.SelectedTabPage.Name != item2.Name)
						{
							LoadControlToDataSource(item2);
						}
					}
				}
				else if (item is SplitContainer || item is SplitContainerControl)
				{
					LoadControlToDataSource(item.Panel1);
					LoadControlToDataSource(item.Panel2);
				}
				else
				{
					LoadControlToDataSource(item);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void bte_BPBankCountry_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find && !string.IsNullOrWhiteSpace(txt_CardCode.Text) && ((_dtBpBankAccount != null && _dtBpBankAccount.Columns.Count != 0) || WinFuntions.GetTableStruct(ref _dtBpBankAccount, "OCRB", _lstStructDt)))
			{
				string country = Function.ToString(bte_BPBankCountry.EditValue);
				string bankCode = Function.ToString(txt_BPBankCode.EditValue);
				string account = Function.ToString(txt_BPBankAccount.EditValue);
				frm_BPBankAccountSetup frm_BPBankAccountSetup2 = new frm_BPBankAccountSetup
				{
					DtSource = _dtBpBankAccount.Copy(),
					UpdateBankFormAccountMethod = UpdateBpBankAccountFromChild
				};
				DataRow dataRow = _dtBpBankAccount.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Country"]).ToUpper() == country.ToUpper() && Function.ToString(t["BankCode"]).ToUpper() == bankCode.ToUpper() && Function.ToString(t["Account"]).ToUpper() == account.ToUpper());
				if (dataRow != null)
				{
					frm_BPBankAccountSetup2.DflRow = _dtBpBankAccount.Rows.IndexOf(dataRow);
				}
				frm_BPBankAccountSetup2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_BPType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeAddressType(Function.ToString(luk_CardType.EditValue));
			if (FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			bool isLoading = _isLoading;
			_isLoading = true;
			string docSubType = Function.ToString(luk_CardType.EditValue);
			if (_cardTypeChanged)
			{
				LoadCardGroup();
			}
			if (!isLoading)
			{
				_isLoading = false;
			}
			LoadComboCode();
			if (FormMode == Enums.FormViewModeType.Add || _oldCardCode != "")
			{
				luk_Series.ReadOnly = false;
			}
			DataTable defaultSeries = _httpClient.Commons.GetDefaultSeries("2", docSubType, GlobalData.UserID);
			if (defaultSeries.IsNotNull())
			{
				luk_Series.EditValue = Function.ParseShort(defaultSeries.Rows[0][0]);
				luk_DocCode_EditValueChanged(null, null);
			}
			if (Function.ToString(luk_CardType.EditValue) == "C")
			{
				bte_BPChannelCode.Visible = true;
				labelLine18.Visible = true;
				luk_Technician.Visible = true;
				labelLine17.Visible = true;
				lb_SalesEmpLoyee.Utext = "Sales Employee";
				chk_AllowPartialDlvrOfSO.Visible = true;
				chk_AllowPartialDlvrPerRow.Visible = true;
			}
			else if (Function.ToString(luk_CardType.EditValue) == "S")
			{
				bte_BPChannelCode.Visible = false;
				labelLine18.Visible = false;
				luk_Technician.Visible = false;
				labelLine17.Visible = false;
				lb_SalesEmpLoyee.Utext = "Buyer";
				chk_AllowPartialDlvrOfSO.Visible = false;
				chk_AllowPartialDlvrPerRow.Visible = false;
			}
			else
			{
				lb_SalesEmpLoyee.Utext = "Sales Employee";
				bte_BPChannelCode.Visible = true;
				labelLine18.Visible = true;
				luk_Technician.Visible = false;
				labelLine17.Visible = false;
				chk_AllowPartialDlvrOfSO.Visible = true;
				chk_AllowPartialDlvrPerRow.Visible = true;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				lbl_ControlAccts.Visible = false;
				btEx_ControlAccts.Visible = false;
				lbl_ConnBP.Visible = true;
				bte_ConnBP.Visible = true;
				chk_BlockDunn.Visible = false;
				lbl_DunnLevel.Visible = false;
				calc_DunnLevel.Visible = false;
				lbl_DunnDate.Visible = false;
				dateX_DunnDate.Visible = false;
				chk_UseShpdGd.Visible = false;
				if (Function.ToString(luk_CardType.EditValue) == "C")
				{
					lbl_DebPayAcct.Utext = "Accounts Receivable";
					lbl_ConnBP.Utext = "Connected Vendor";
				}
				else if (Function.ToString(luk_CardType.EditValue) == "S")
				{
					lbl_DebPayAcct.Utext = "Accounts Payable";
					lbl_ConnBP.Utext = "Connected Customer";
				}
				else
				{
					lbl_ConnBP.Visible = false;
					bte_ConnBP.Visible = false;
					lbl_DebPayAcct.Utext = "Accounts Receivable";
					lbl_ConnBP.Utext = "Connected Vendor";
				}
			}
			else if (Function.ToString(luk_CardType.EditValue) == "C")
			{
				lbl_ControlAccts.Visible = true;
				btEx_ControlAccts.Visible = true;
				lbl_ConnBP.Visible = true;
				bte_ConnBP.Visible = true;
				chk_BlockDunn.Visible = true;
				lbl_DunnLevel.Visible = true;
				calc_DunnLevel.Visible = true;
				lbl_DunnDate.Visible = true;
				dateX_DunnDate.Visible = true;
				chk_UseShpdGd.Visible = true;
				lbl_DebPayAcct.Utext = "Accounts Receivable";
				lbl_ConnBP.Utext = "Connected Vendor";
			}
			else if (Function.ToString(luk_CardType.EditValue) == "S")
			{
				lbl_ControlAccts.Visible = true;
				btEx_ControlAccts.Visible = true;
				chk_BlockDunn.Visible = false;
				lbl_DunnLevel.Visible = false;
				calc_DunnLevel.Visible = false;
				lbl_DunnDate.Visible = false;
				lbl_ConnBP.Visible = true;
				bte_ConnBP.Visible = true;
				dateX_DunnDate.Visible = false;
				chk_UseShpdGd.Visible = false;
				lbl_DebPayAcct.Utext = "Accounts Payable";
				lbl_ConnBP.Utext = "Connected Customer";
			}
			else
			{
				lbl_ControlAccts.Visible = false;
				btEx_ControlAccts.Visible = false;
				chk_BlockDunn.Visible = true;
				lbl_DunnLevel.Visible = true;
				calc_DunnLevel.Visible = true;
				lbl_DunnDate.Visible = true;
				dateX_DunnDate.Visible = true;
				lbl_ConnBP.Visible = false;
				bte_ConnBP.Visible = false;
				chk_UseShpdGd.Visible = false;
				lbl_DebPayAcct.Utext = "Accounts Receivable";
				lbl_ConnBP.Utext = "Connected Vendor";
			}
			SetBalanceDisplayControls();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				if (Function.ToString(luk_CardType.EditValue) == "C")
				{
					_isAskPaymentTerm = false;
					luk_PaymentTerm.EditValue = GlobalData.GeneralSetting.DfCustTerm;
					_isAskPaymentTerm = true;
				}
				else if (Function.ToString(luk_CardType.EditValue) == "S")
				{
					_isAskPaymentTerm = false;
					luk_PaymentTerm.EditValue = GlobalData.GeneralSetting.DfVendTerm;
					_isAskPaymentTerm = true;
				}
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_DocCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (luk_Series.Text.ToUpper() == "MANUAL" || string.IsNullOrWhiteSpace(luk_Series.Text))
			{
				txt_CardCode.ReadOnly = false;
			}
			else
			{
				txt_CardCode.ReadOnly = true;
			}
			if (_isLoading)
			{
				return;
			}
			if (string.Compare(luk_Series.Text, "MANUAL", ignoreCase: true) == 0)
			{
				if (string.Compare(_oldValueSeries, "MANUAL", ignoreCase: true) != 0 || FormMode == Enums.FormViewModeType.Add)
				{
					txt_CardCode.Text = "";
				}
				_oldValueSeries = "";
			}
			else
			{
				string s = Function.ToString(luk_Series.Text);
				string o = "2";
				int defaultBranch = GlobalData.DefaultBranch;
				DataTable codeBySeries = _httpClient.Commons.GetCodeBySeries(s, o, defaultBranch);
				if (codeBySeries != null && codeBySeries.Rows.Count > 0)
				{
					txt_CardCode.Text = ((codeBySeries.Rows[0][0] == DBNull.Value || codeBySeries.Rows[0][0] == null) ? "" : codeBySeries.Rows[0][0].ToString());
				}
				txt_CardCode.ReadOnly = true;
			}
			if (FormMode == Enums.FormViewModeType.Update && _oldSeries == Function.ParseShort(luk_Series.EditValue))
			{
				txt_CardCode.Text = _oldCardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isLoading = true;
		try
		{
			int code = Function.ParseInt(gv_Contact.GetFocusedRowCellValue("CntctCode"));
			if (grd_Contact.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["CntctCode"]) == code);
				if (dataRow != null)
				{
					SetContactPersonControl(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void txtContactPersonName_Leave(object sender, EventArgs e)
	{
		try
		{
			if (sender is TextEdit textEdit && textEdit.Name == txt_Name.Name)
			{
				AddContactPerson();
			}
			else
			{
				UpdateContactPerson();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btContactPersonDefault_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Contact.GetFocusedRowCellValue("Name"));
			if (!string.IsNullOrEmpty(text) && text.ToUpper() != "ADD NEW")
			{
				txtGeneralContactPerson.Text = text;
				_defaultContact = text;
				gv_Contact.PostEditor();
				gv_Contact.Focus();
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			int[] selectedRows = gv_Contact.GetSelectedRows();
			if (selectedRows == null || selectedRows.Length == 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveContactPerson, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes || !(Function.ToString(gv_Contact.GetRowCellValue(selectedRows[0], "Name")).ToUpper() != "ADD NEW"))
			{
				return;
			}
			int cntctCode = Function.ParseInt(gv_Contact.GetRowCellValue(selectedRows[0], "CntctCode"));
			string text = Function.ToString(gv_Contact.GetRowCellValue(selectedRows[0], "Name"));
			string text2 = _httpClient.BusinessMaster.ExistLinkedContact(cntctCode);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.YouCannotRemoveContactPersonInUseWithCode, "", text, text2);
				return;
			}
			gv_Contact.DeleteSelectedRows();
			if (!(grd_Contact.DataSource is DataTable dataTable))
			{
				return;
			}
			dataTable.AcceptChanges();
			gv_Contact.FocusedRowHandle = 0;
			ChangeButtonOKText(sender);
			if (dataTable.Rows.Count == 2)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).ToUpper() != "ADD NEW");
				if (dataRow != null)
				{
					_defaultContact = Function.ToString(dataRow["Name"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Address.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetSearchTableAddress();
				dataTable.Rows[0]["LineNum"] = -1;
				dataTable.Rows[0]["AdresType"] = "B";
				dataTable.AcceptChanges();
			}
			else if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["Address"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["LineNum"] = -1;
				dataRow["AdresType"] = "B";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			grd_Address.DataSource = dataTable;
			grd_Address.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Address.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 1)
			{
				gv_Address.DeleteSelectedRows();
				dataTable.AcceptChanges();
				ChangeButtonOKText(sender);
			}
			grd_Address.DataSource = dataTable;
			grd_Address.RefreshDataSource();
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if ((from t in dataTable.AsEnumerable()
				where Function.ToString(t["AdresType"]).ToUpper() != "S" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"]))
				select t).Count() == 1)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["AdresType"]).ToUpper() != "S" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"])));
				if (dataRow != null)
				{
					_defaultShipAddress = Function.ToString(dataRow["Address"]);
				}
			}
			if ((from t in dataTable.AsEnumerable()
				where Function.ToString(t["AdresType"]).ToUpper() != "B" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"]))
				select t).Count() == 1)
			{
				DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["AdresType"]).ToUpper() != "B" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"])));
				if (dataRow2 != null)
				{
					_defaultBillAddress = Function.ToString(dataRow2["Address"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PropertiesSellectAll_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Check"] = "Y";
				}
				dataTable.AcceptChanges();
				grd_Properties.DataSource = dataTable;
				grd_Properties.RefreshDataSource();
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PropertiesClearSelect_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Check"] = "N";
				}
				dataTable.AcceptChanges();
				grd_Properties.DataSource = dataTable;
				grd_Properties.RefreshDataSource();
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Properties_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btn_PicDelete_Click(object sender, EventArgs e)
	{
		try
		{
			picRemarks.Image = null;
			_picFilePath = "";
			btn_PicDelete.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_PaymentPriceList_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_STDPriceLists frm_STDPriceLists2 = new frm_STDPriceLists();
				frm_STDPriceLists2.FormClosing += (object s, FormClosingEventArgs e2) =>
				{
					LoadPriceList();
				};
				frm_STDPriceLists2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_BPBankCountry_QueryPopUp(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void rdg_Active_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			switch (rdg_Active.SelectedIndex)
			{
			case 0:
				panel_Active.Visible = true;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = false;
				break;
			case 1:
				panel_Active.Visible = false;
				panel_Inactive.Visible = true;
				panel_Advanced.Visible = false;
				break;
			case 2:
				panel_Active.Visible = false;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = true;
				break;
			default:
				panel_Active.Visible = false;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = false;
				break;
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeBusinessPartnerToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveBp, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string cardCode = txt_CardCode.Text;
				string text = _httpClient.BusinessMaster.DeleteDoc(cardCode);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					MODE_FIND();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip_BP_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				removeBusinessPartnerToolStripMenuItem.Visible = false;
			}
			else
			{
				removeBusinessPartnerToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Controls_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private void luk_CreditCardType_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_CreditCardSetup frm_CreditCardSetup2 = new frm_CreditCardSetup();
				frm_CreditCardSetup2.FormClosing += async (object s, FormClosingEventArgs e2) =>
				{
					await LoadCreditCardType();
				};
				frm_CreditCardSetup2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_CardGroup_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		try
		{
			if (_isLoading || _isSettingCardGroup || FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			if (isLoadDataFromQuickCreationBMD)
			{
				short? grpCode = Function.ParseShort(luk_CardGroup.EditValue);
				string bpType = Function.ToString(luk_CardType.EditValue);
				DataRow dataRow = _dtBpGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["GroupType"]) == bpType && Function.ParseInt(t["GroupCode"]) == grpCode);
				if (dataRow != null)
				{
					luk_EffectiveDiscount.EditValue = Function.ToString(dataRow["DiscRel"]);
					luk_PaymentPriceList.EditValue = Function.ParseShort(dataRow["PriceList"]);
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReplacePriceListAndEffective, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				short? grpCode2 = Function.ParseShort(luk_CardGroup.EditValue);
				string bpType2 = Function.ToString(luk_CardType.EditValue);
				DataRow dataRow2 = _dtBpGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["GroupType"]) == bpType2 && Function.ParseInt(t["GroupCode"]) == grpCode2);
				if (dataRow2 != null)
				{
					luk_EffectiveDiscount.EditValue = Function.ToString(dataRow2["DiscRel"]);
					luk_PaymentPriceList.EditValue = Function.ParseShort(dataRow2["PriceList"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_AddressDefault_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				string text = Function.ToString(gv_Address.GetFocusedRowCellValue("Address"));
				if (Function.ToString(gv_Address.GetFocusedRowCellValue("AdresType")).ToUpper() == "S")
				{
					_defaultShipAddress = text;
					_defaultShipAddrHandle = gv_Address.FocusedRowHandle;
				}
				else
				{
					_defaultBillAddress = text;
					_defaultBillAddrHandle = gv_Address.FocusedRowHandle;
				}
				gv_Address.RefreshData();
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Contact_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && Function.ToString(gv_Contact.GetRowCellValue(e.RowHandle, "Name")).ToUpper() == _defaultContact.ToUpper())
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Address_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				string text = Function.ToString(gv_Address.GetRowCellValue(e.RowHandle, "Address"));
				string text2 = Function.ToString(gv_Address.GetRowCellValue(e.RowHandle, "AdresType"));
				if (text2 == "S" && text.ToUpper() == _defaultShipAddress.ToUpper() && e.RowHandle == _defaultShipAddrHandle)
				{
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
				else if (text2 == "B" && text.ToUpper() == _defaultBillAddress.ToUpper() && e.RowHandle == _defaultBillAddrHandle)
				{
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
				else
				{
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Address_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			if (grd_Address.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				if ((from t in dataTable.AsEnumerable()
					where Function.ToString(t["AdresType"]).ToUpper() == "S" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"]))
					select t).Count() == 1)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["AdresType"]).ToUpper() == "S" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"])));
					if (dataRow != null)
					{
						_defaultShipAddress = Function.ToString(dataRow["Address"]).Trim();
						for (int num = 0; num < gv_Address.DataRowCount; num++)
						{
							if (string.Compare(Function.ToString(gv_Address.GetRowCellValue(num, "Address")).Trim(), _defaultShipAddress, ignoreCase: true) == 0 && string.Compare(Function.ToString(gv_Address.GetRowCellValue(num, "AdresType")).Trim(), "S", ignoreCase: true) == 0)
							{
								_defaultShipAddrHandle = num;
							}
						}
					}
				}
				if ((from t in dataTable.AsEnumerable()
					where Function.ToString(t["AdresType"]).ToUpper() == "B" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"]))
					select t).Count() == 1)
				{
					DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["AdresType"]).ToUpper() == "B" && !string.IsNullOrWhiteSpace(Function.ToString(t["Address"])));
					if (dataRow2 != null)
					{
						_defaultBillAddress = Function.ToString(dataRow2["Address"]).Trim();
						for (int num2 = 0; num2 < gv_Address.DataRowCount; num2++)
						{
							if (string.Compare(Function.ToString(gv_Address.GetRowCellValue(num2, "Address")).Trim(), _defaultBillAddress, ignoreCase: true) == 0 && string.Compare(Function.ToString(gv_Address.GetRowCellValue(num2, "AdresType")).Trim(), "B", ignoreCase: true) == 0)
							{
								_defaultBillAddrHandle = num2;
							}
						}
					}
				}
				if (e.Column.FieldName == "Country")
				{
					gv_Address.SetRowCellValue(e.RowHandle, "State", "");
				}
			}
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				addRowToolStripMenuItem_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Address_ShownEditor(object sender, EventArgs e)
	{
	}

	private void rep_States_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string countryCode = Function.ToString(gv_Address.GetFocusedRowCellValue("Country"));
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRStates frm_HRStates2 = new frm_HRStates
				{
					MdiParent = MdiParent,
					CountryCode = countryCode,
					Type = 1
				};
				frm_HRStates2.RefreshAllStates = (Func<string, Task>)Delegate.Combine(frm_HRStates2.RefreshAllStates, new Func<string, Task>(LoadAllState));
				frm_HRStates2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chkContactPersonActive_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (txt_Name.Text.ToUpper() != "ADD NEW")
			{
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cb_CurrencyType_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			SetBalanceDisplayControls();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_BPType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!GlobalData.LoginSap && !_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				string cardCode = txt_CardCode.Text;
				DataTable result = Task.Run(async () => await _httpClient.BusinessMaster.ExistsLinkedAll(cardCode)).Result;
				if (result != null && result.Rows.Count > 0 && result.AsEnumerable().FirstOrDefault((DataRow t) => !_allowChangeBpTypeObjType.Contains(Function.ParseInt(t[0]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.CannotChangeBpTypeLickedObjectExists, "");
					e.Cancel = true;
				}
			}
			string text = Function.ToString(e.OldValue);
			string text2 = Function.ToString(e.NewValue);
			if (((text == "C" || text == "L") && text2 == "S") || ((text2 == "C" || text2 == "L") && text == "S") || !new string[3] { "S", "L", "C" }.Contains(text) || !new string[3] { "S", "L", "C" }.Contains(text2) || text2 == text)
			{
				_cardTypeChanged = true;
			}
			else
			{
				_cardTypeChanged = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_DocCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			int oldVal = Function.ParseInt(e.NewValue);
			int newVal = Function.ParseInt(e.NewValue);
			if (luk_Series.Properties.DataSource is DataTable source)
			{
				DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Series"]) == oldVal);
				DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Series"]) == newVal);
				if (dataRow != null && dataRow2 != null)
				{
					_oldValueSeries = Function.ToString(dataRow["SeriesName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtContactPersonName_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (sender is TextEdit textEdit && string.IsNullOrWhiteSpace(Function.ToString(textEdit.Text)))
			{
				WinCommons.Error(Enums.MessageCode.TheNameFieldMustNotEmptyEnterNameForContactPerson, "");
				textEdit.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private void bte_FactoringIndicator_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(bte_FactoringIndicator.EditValue)))
			{
				return;
			}
			DataSet dataFactoringIndicator = _httpClient.Cfl.GetDataFactoringIndicator();
			bool flag = true;
			if (dataFactoringIndicator == null || dataFactoringIndicator.Tables.Count < 2 || !dataFactoringIndicator.Tables[1].IsNotNull())
			{
				flag = false;
			}
			else
			{
				DataRow[] array = dataFactoringIndicator.Tables[1].Select("Code like '%" + Function.ToString(bte_FactoringIndicator.EditValue) + "%'");
				if (array.Length != 0)
				{
					bte_FactoringIndicator.EditValue = Function.ToString(array[0]["Code"]);
					txt_FactoringIndicator.Text = Function.ToString(array[0]["Name"]);
				}
				else
				{
					flag = false;
				}
			}
			if (!flag)
			{
				bte_FactoringIndicator_ButtonPressed(bte_FactoringIndicator, new ButtonPressedEventArgs(bte_FactoringIndicator.Properties.Buttons[0]));
				e.Cancel = true;
				WinCommons.Error(Enums.MessageCode.InvalidIndicatorCode, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_FactoringIndicator_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			e.ExceptionMode = ExceptionMode.NoAction;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_DoNotApplyDiscountGroup_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			luk_EffectiveDiscount.Enabled = !chk_DoNotApplyDiscountGroup.Checked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btEx_ControlAccts_Click(object sender, EventArgs e)
	{
		try
		{
			frm_BPControlAccounts frm = new frm_BPControlAccounts();
			frm.DTSource = _dtBpControlAccounts.Copy();
			frm.BPType = Function.ToString(luk_CardType.EditValue);
			frm.FormClosing += (object s1, FormClosingEventArgs e2) =>
			{
				if (frm.IsOKClick && frm.IsChangeValue)
				{
					_dtBpControlAccounts = frm.DTSource.Copy();
					FormMode = Enums.FormViewModeType.Update;
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Controls_EditValueChanged(object sender, CellValueChangedEventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			luk_Series.ReadOnly = false;
			luk_Series.BackColor = Color.White;
			FormMode = Enums.FormViewModeType.Add;
			if (luk_Series.Text.ToUpper() == "MANUAL" || string.IsNullOrWhiteSpace(luk_Series.Text))
			{
				txt_CardCode.ReadOnly = false;
				txt_CardCode.BackColor = Color.White;
			}
			else
			{
				string s = Function.ToString(luk_Series.Text);
				string o = "2";
				int defaultBranch = GlobalData.DefaultBranch;
				DataTable codeBySeries = _httpClient.Commons.GetCodeBySeries(s, o, defaultBranch);
				if (codeBySeries != null && codeBySeries.Rows.Count > 0)
				{
					txt_CardCode.Text = ((codeBySeries.Rows[0][0] == DBNull.Value || codeBySeries.Rows[0][0] == null) ? "" : codeBySeries.Rows[0][0].ToString());
				}
				txt_CardCode.ReadOnly = true;
				txt_CardCode.BackColor = Color.FromArgb(231, 231, 231);
			}
			DataTable dataTable = grd_Contact.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDuplicateBpContactPerson, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					int num = -2;
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						if (Function.ParseInt(dataTable.Rows[i]["CntctCode"]) != -1)
						{
							dataTable.Rows[i]["CntctCode"] = num;
							num--;
						}
					}
					dataTable.AcceptChanges();
					grd_Contact.DataSource = dataTable;
					grd_Contact.RefreshDataSource();
				}
				else
				{
					for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
					{
						if (Function.ParseInt(dataTable.Rows[num2]["CntctCode"]) != -1)
						{
							dataTable.Rows.Remove(dataTable.Rows[num2]);
						}
					}
					dataTable.AcceptChanges();
					grd_Contact.DataSource = dataTable;
					grd_Contact.RefreshDataSource();
				}
			}
			luk_Currency.Properties.DataSource = _listCurrencyCode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_FatherCard_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(luk_CardType.EditValue);
				Enums.TypeCFL cflType = Enums.TypeCFL.None;
				if (text == "C")
				{
					cflType = Enums.TypeCFL.Customer;
				}
				else if (text == "S")
				{
					cflType = Enums.TypeCFL.Vendor;
				}
				CFLTabAccounting(sender, cflType, Function.ToString(bte_FatherCard.EditValue), "CardCode", "CardName", lbl_ConSolidBPName);
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oBusinessPartners, Function.ToString(bte_FatherCard.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_DebPayAcct_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(luk_CardType.EditValue);
				Enums.TypeCFL cflType = Enums.TypeCFL.None;
				if (text == "C")
				{
					cflType = Enums.TypeCFL.AcctCusBPOpenDebts;
				}
				else if (text == "S")
				{
					cflType = Enums.TypeCFL.AcctVenBPOpenDebts;
				}
				CFLTabAccounting(sender, cflType, Function.ToString(bte_DebPayAcct.EditValue), "AcctCode", "AcctName", lbl_AcctPayableName);
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oChartOfAccounts, Function.ToString(bte_DebPayAcct.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_DpmClear_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				CFLTabAccounting(sender, Enums.TypeCFL.ChartOfAccount, Function.ToString(bte_DpmClear.EditValue), "AcctCode", "AcctName", lbl_ClearAcctName);
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oChartOfAccounts, Function.ToString(bte_DebPayAcct.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_DpmIntAct_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				CFLTabAccounting(sender, Enums.TypeCFL.ChartOfAccount, Function.ToString(bte_DpmIntAct.EditValue), "AcctCode", "AcctName", lbl_InterimAcctName);
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oChartOfAccounts, Function.ToString(bte_DpmIntAct.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_ConnBP_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(luk_CardType.EditValue);
			Enums.TypeCFL cflType = Enums.TypeCFL.None;
			if (text == "C")
			{
				cflType = Enums.TypeCFL.Vendor;
			}
			else if (text == "S")
			{
				cflType = Enums.TypeCFL.Customer;
			}
			CFLTabAccounting(sender, cflType, Function.ToString(bte_DebPayAcct.EditValue), "CardCode", "CardName", lbl_ConnBPName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bteCFL_OwnerCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = "";
			if (FormMode == Enums.FormViewModeType.Add && GlobalData.BranchList.IsNotNull())
			{
				foreach (DataRow row in GlobalData.BranchList.Rows)
				{
					text = text + Function.ToString(row[0]) + ",";
				}
			}
			if ((FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View) && _dtBranchAssignment.IsNotNull())
			{
				DataRow[] array = _dtBranchAssignment.Select("Checked = 'Y'");
				foreach (DataRow dataRow2 in array)
				{
					text = text + Function.ToString(dataRow2[0]) + ",";
				}
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("@BPLId", text));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, Function.ToString(bteCFL_OwnerCode.EditValue), "APZ_Cfl_GetEmployeeListForSalesEmpSetup", Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			bteCFL_OwnerCode.Properties.fromCFL = true;
			if (frm_CFL2.ChooseItem != null)
			{
				bteCFL_OwnerCode.Properties.selectedRowFromCFL = frm_CFL2.ChooseItem;
				bteCFL_OwnerCode.EditValue = chooseItem[0];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dupplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Address.DataSourceTable;
			if (dataSourceTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(gv_Address.GetFocusedRowCellValue("Address"))))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow.ItemArray = gv_Address.GetFocusedDataRow().ItemArray;
				dataRow["LineNum"] = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
				DataRow dataRow2 = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Address"])));
				if (dataRow2 != null)
				{
					int pos = dataSourceTable.AsEnumerable().IndexOf(dataRow2);
					dataSourceTable.Rows.InsertAt(dataRow, pos);
				}
				else
				{
					dataSourceTable.Rows.Add(dataRow);
				}
				dataSourceTable.AcceptChanges();
				grd_Address.DataSource = dataSourceTable;
				grd_Address.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Currency_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -1)
			{
				new frm_Currencies().ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_CardGroup_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isSettingCardGroup || Function.ParseShort(e.NewValue) != -1)
			{
				return;
			}
			string text = Function.ToString(luk_CardType.EditValue);
			short? num = Function.ParseShort(luk_CardGroup.EditValue);
			_isSettingCardGroup = true;
			VirFormCustomerVendorGroup virFormCustomerVendorGroup = null;
			if (text.ToUpper() == "S")
			{
				virFormCustomerVendorGroup = new frm_VendorGroup();
			}
			else if (text.ToUpper() == "C" || text.ToUpper() == "L")
			{
				virFormCustomerVendorGroup = new frm_CustomerGroup();
			}
			if (virFormCustomerVendorGroup != null)
			{
				virFormCustomerVendorGroup.IsOpenFromBP = true;
				virFormCustomerVendorGroup.ShowDialog(this);
				LoadCardGroup();
				if (virFormCustomerVendorGroup.NewValue != 0)
				{
					luk_CardGroup.EditValue = virFormCustomerVendorGroup.NewValue;
				}
				else
				{
					luk_CardGroup.EditValue = num;
				}
			}
			_isSettingCardGroup = false;
			e.Cancel = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void luk_ShippingType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				frm_Shipping frm_Shipping2 = new frm_Shipping();
				frm_Shipping2.MaximizeBox = false;
				frm_Shipping2.ShowDialog(this);
				await reloadComboBoxFromChildForm("ShippingType", Function.ToString(luk_ShippingType.EditValue));
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_BPIndustry_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10)
			{
				int num = Function.ParseInt(luk_BPIndustry.EditValue);
				frm_IndustrySetup frm_IndustrySetup2 = new frm_IndustrySetup();
				frm_IndustrySetup2.IsOpenFromBP = true;
				frm_IndustrySetup2.FormClosing += (object s, FormClosingEventArgs e2) =>
				{
					LoadComboIndustry();
				};
				frm_IndustrySetup2.ShowDialog(this);
				if (frm_IndustrySetup2.NewValue != 0)
				{
					luk_BPIndustry.EditValue = frm_IndustrySetup2.NewValue;
				}
				else
				{
					luk_BPIndustry.EditValue = num;
				}
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void luk_SalesBuyer_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10)
			{
				frm_SalesEmployeeBuyers frm_SalesEmployeeBuyers2 = new frm_SalesEmployeeBuyers();
				frm_SalesEmployeeBuyers2.MaximizeBox = false;
				frm_SalesEmployeeBuyers2.ShowDialog(this);
				await reloadComboBoxFromChildForm("SaleEmployee", Function.ToString(luk_SalesBuyer.EditValue));
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_HousBnkCry_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		LoadCBPaymentRunBank();
	}

	private void luk_HouseBank_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			luk_HousBnkAct.EditValue = "";
			luk_HsBnkIBAN.EditValue = "";
			txt_HousBnkBrn.Text = "";
			txt_HsBnkSwift.Text = "";
			txt_HousCtlKey.Text = "";
			LoadCBPaymentRunAccount();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_HousBnkAct_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		_isSettingHousBnkAct = true;
		try
		{
			if (!_isSetting_HsBnkIban)
			{
				string account = Function.ToString(luk_HousBnkAct.EditValue);
				SetPaymentRunBankAccountData(account, luk_HsBnkIBAN);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSettingHousBnkAct = false;
	}

	private void luk_HsBnkIban_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		_isSetting_HsBnkIban = true;
		try
		{
			if (!_isSettingHousBnkAct)
			{
				string account = Function.ToString(luk_HsBnkIBAN.EditValue);
				SetPaymentRunBankAccountData(account, luk_HousBnkAct);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSetting_HsBnkIban = false;
	}

	private void chk_AllowPartialDlvrOfSO_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			if (chk_AllowPartialDlvrOfSO.Checked)
			{
				chk_AllowPartialDlvrPerRow.Visible = true;
			}
			else
			{
				chk_AllowPartialDlvrPerRow.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PaymentDates_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find && !string.IsNullOrWhiteSpace(txt_CardCode.Text) && ((_dtPaymentDates != null && _dtPaymentDates.Columns.Count != 0) || WinFuntions.GetTableStruct(ref _dtPaymentDates, "CRD5", _lstStructDt)))
			{
				frm_BPPaymentDates frm_BPPaymentDates2 = new frm_BPPaymentDates();
				frm_BPPaymentDates2._dtSource = _dtPaymentDates.Copy();
				frm_BPPaymentDates2._cardCode = txt_CardCode.Text;
				frm_BPPaymentDates2.FormClosing += frmPaymentDates_FormClosing;
				frm_BPPaymentDates2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frmPaymentDates_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (!(sender is frm_BPPaymentDates { _isButtonOK: not false } frm_BPPaymentDates2))
			{
				return;
			}
			frm_BPPaymentDates2._isButtonOK = false;
			if (!frm_BPPaymentDates2._isOK)
			{
				e.Cancel = true;
				return;
			}
			_dtPaymentDates = frm_BPPaymentDates2._dtSource;
			if (_dtPaymentDates == null || _dtPaymentDates.Rows.Count <= 0)
			{
				return;
			}
			for (int num = _dtPaymentDates.Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = _dtPaymentDates.Rows[num];
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["PmntDate"])))
				{
					_dtPaymentDates.Rows.Remove(dataRow);
					_dtPaymentDates.AcceptChanges();
				}
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_FactoringIndicator_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.FactoringIndicator, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_FactoringIndicator.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
				txt_FactoringIndicator.Text = Function.ToString(frm_CFL2.ChooseItem["Name"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_BPProject_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_BPProject.EditValue = Function.ToString(frm_CFL2.ChooseItem["PrjCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_BPChannelCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_BPChannelCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_Technician_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeesTechnician, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				luk_Technician.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["empID"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Territory_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Territory, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				luk_Territory.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["territryID"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Technician_QueryPopUp(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void luk_Territory_QueryPopUp(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void txtValidComm_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingValidComments)
			{
				ChangeButtonOKText(sender);
				_isSettingValidComments = true;
				txtValidComm2.EditValue = txtValidComm.EditValue;
				_isSettingValidComments = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtFrozenComm_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingFrozenComments)
			{
				ChangeButtonOKText(sender);
				_isSettingFrozenComments = true;
				txtFrozenComm2.EditValue = txtFrozenComm.EditValue;
				_isSettingFrozenComments = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtValidComm2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingValidComments)
			{
				ChangeButtonOKText(sender);
				_isSettingValidComments = true;
				txtValidComm.EditValue = txtValidComm2.EditValue;
				_isSettingValidComments = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtFrozenComm2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingFrozenComments)
			{
				ChangeButtonOKText(sender);
				_isSettingFrozenComments = true;
				txtFrozenComm.EditValue = txtFrozenComm2.EditValue;
				_isSettingFrozenComments = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateValidFrom)
			{
				ChangeButtonOKText(sender);
				_isSettingDateValidFrom = true;
				dateValidFrom2.EditValue = dateValidFrom.EditValue;
				_isSettingDateValidFrom = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidTo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateValidTo)
			{
				ChangeButtonOKText(sender);
				_isSettingDateValidTo = true;
				dateValidTo2.EditValue = dateValidTo.EditValue;
				_isSettingDateValidTo = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateFrozenFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateFrozenFrom)
			{
				ChangeButtonOKText(sender);
				_isSettingDateFrozenFrom = true;
				dateFrozenFrom2.EditValue = dateFrozenFrom.EditValue;
				_isSettingDateFrozenFrom = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateFrozenTo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateFrozenTo)
			{
				ChangeButtonOKText(sender);
				_isSettingDateFrozenTo = true;
				dateFrozenTo2.EditValue = dateFrozenTo.EditValue;
				_isSettingDateFrozenTo = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidFrom2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateValidFrom)
			{
				ChangeButtonOKText(sender);
				_isSettingDateValidFrom = true;
				dateValidFrom.EditValue = dateValidFrom2.EditValue;
				_isSettingDateValidFrom = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidTo2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateValidTo)
			{
				ChangeButtonOKText(sender);
				_isSettingDateValidTo = true;
				dateValidTo.EditValue = dateValidTo2.EditValue;
				_isSettingDateValidTo = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateFrozenFrom2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateFrozenFrom)
			{
				ChangeButtonOKText(sender);
				_isSettingDateFrozenFrom = true;
				dateFrozenFrom.EditValue = dateFrozenFrom2.EditValue;
				_isSettingDateFrozenFrom = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateFrozenTo2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isSettingDateFrozenTo)
			{
				ChangeButtonOKText(sender);
				_isSettingDateFrozenTo = true;
				dateFrozenTo.EditValue = dateFrozenTo2.EditValue;
				_isSettingDateFrozenTo = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbPaymentPaymentTerm_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			if (_isAskPaymentTerm && !_isLoading)
			{
				if (isLoadDataFromQuickCreationBMD)
				{
					ChangePaymentTermData();
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyPaymentTerm2Bp, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					ChangePaymentTermData();
				}
			}
			else
			{
				ChangePaymentTermData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangePaymentTermData()
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			int num = Function.ParseInt(luk_PaymentTerm.EditValue);
			if (num != 0)
			{
				DataTable paymentTerm = _httpClient.BusinessMaster.GetPaymentTerm(num);
				if (paymentTerm != null && paymentTerm.Rows.Count > 0)
				{
					DataRow dataRow = paymentTerm.Rows[0];
					txtPaymentInterestonArrears.EditValue = Function.ParseDecimal(dataRow["LatePyChrg"]);
					luk_PaymentPriceList.EditValue = Function.ParseShort(dataRow["ListNum"]);
					txtPaymentTotalDiscount.EditValue = Function.ParseDecimal(dataRow["VolumDscnt"]);
					txtPaymentCreditLimit.EditValue = Function.ParseDecimal(dataRow["CredLimit"]);
					txtPaymentCommitmentLimit.EditValue = Function.ParseDecimal(dataRow["ObligLimit"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbPaymentPaymentTerm_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				WinCommons._OpenProcessing();
				string value = Function.ToString(luk_PaymentTerm.EditValue);
				frm_PaymentTerms frm_PaymentTerms2 = new frm_PaymentTerms();
				frm_PaymentTerms2.FormClosing += async (object s, FormClosingEventArgs e2) =>
				{
					await LoadPaymentTerm();
					_isAskPaymentTerm = false;
					luk_PaymentTerm.EditValue = Function.ParseInt(value);
					ChangePaymentTermData();
					_isAskPaymentTerm = true;
				};
				frm_PaymentTerms2.ShowDialog(this);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task reloadComboBoxFromChildForm(string comboBoxName, string value)
	{
		_ = 3;
		try
		{
			string[] array = comboBoxName.Split(',');
			if (array[0] == "BMD")
			{
				_copyF_CodeCustomer = array[1];
				await WaitLoad();
				return;
			}
			switch (comboBoxName)
			{
			case "CardGroup":
				LoadCardGroup();
				luk_CardGroup.EditValue = Function.ParseInt(value);
				break;
			case "Currency":
				LoadCurrency();
				luk_Currency.EditValue = Function.ToString(value);
				break;
			case "SaleEmployee":
				await LoadSaleEmployee();
				luk_SalesBuyer.EditValue = Function.ParseInt(value);
				break;
			case "CreditType":
				await LoadCreditCardType();
				luk_CreditCardType.EditValue = Function.ToString(value);
				break;
			case "ShippingType":
				await LoadShippingType();
				luk_ShippingType.EditValue = Function.ParseInt(value);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btAdd_Click(object sender, EventArgs e)
	{
		if (_isProcessing)
		{
			return;
		}
		_isProcessing = true;
		btAdd.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				await SearchList();
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				if (!ValidateData())
				{
					btAdd.Enabled = true;
					return;
				}
				if (!GlobalData.LoginSap)
				{
					await AddBMDNoSAP("U");
				}
				else
				{
					await AddBMD();
				}
				btAdd.Enabled = true;
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				if (IsManualCode() && CheckDoc())
				{
					WinCommons.Error(Enums.MessageCode.BusinessPartnerCodeAlreadyExistsAssignedToBpEnterUniqurBpCode, "", txt_CardCode.Text);
				}
				else
				{
					if (!ValidateData())
					{
						btAdd.Enabled = true;
						return;
					}
					if (!GlobalData.LoginSap)
					{
						await AddBMDNoSAP("A");
					}
					else
					{
						await AddBMD();
					}
				}
				btAdd.Enabled = true;
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			if (FormAuth == Enums.FormAuthorities.Full)
			{
				btAdd.Enabled = true;
			}
			_isProcessing = false;
		}
	}

	private bool ValidateData()
	{
		try
		{
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void btnRemarkBrowserImage_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png",
				FilterIndex = 2,
				RestoreDirectory = true,
				InitialDirectory = CopyFileShareHelper.GetUpLoadPath("jpg")
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					picRemarks.Image = Image.FromStream(stream);
					btn_PicDelete.Visible = true;
				}
				_picFilePath = openFileDialog.FileName;
				if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
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
		Close();
	}

	private void btn_BranchAssignment_Click(object sender, EventArgs e)
	{
		try
		{
			frm_BranchAssignment frm = new frm_BranchAssignment(_dtBranchAssignment.Copy(), Function.ToString(txt_CardCode.Text), ApzObjectTypes.oBusinessPartners);
			frm.FormClosing += (object se, FormClosingEventArgs ev) =>
			{
				if (frm.Changed)
				{
					_dtBranchAssignment = frm.DtSource.Copy();
					if (FormMode == Enums.FormViewModeType.View && !string.IsNullOrWhiteSpace(txt_CardCode.Text))
					{
						FormMode = Enums.FormViewModeType.Update;
					}
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetBmdControls()
	{
		try
		{
			_isAskPaymentTerm = false;
			_isLoading = true;
			_oldCardCode = "";
			_oldCardType = "";
			_dtOriginal = new DataTable();
			txt_CardCode.Text = "";
			txt_CardName.Text = "";
			txtForeignName.Text = "";
			luk_Currency.ItemIndex = 0;
			txtFederalTaxID.Text = "";
			calc_Balance.Text = "";
			calc_DNotesBal.Text = "";
			calc_Balance.ReadOnly = true;
			calc_DNotesBal.ReadOnly = true;
			calc_OrdersBal.ReadOnly = true;
			calc_BalanceFC.ReadOnly = true;
			calc_DNoteBalFC.ReadOnly = true;
			calc_OrderBalFC.ReadOnly = true;
			calc_BalanceSY.ReadOnly = true;
			calc_DNoteBalSY.ReadOnly = true;
			calc_OrderBalSY.ReadOnly = true;
			calc_OprCount.ReadOnly = true;
			calc_OrdersBal.Text = "";
			txtGeneralTel1.Text = "";
			txtGeneralTel2.Text = "";
			txtGeneralMobilePhone.Text = "";
			txtGeneralFax.Text = "";
			txtGeneralEmail.Text = "";
			txtGeneralWebsite.Text = "";
			luk_ShippingType.ItemIndex = -1;
			txtGeneralPassword.Text = "";
			txtGeneralContactPerson.Text = "";
			txtGeneralContactPerson.ReadOnly = false;
			txtGeneralIDNo2.Text = "";
			txtGeneralFederalTaxID.Text = "";
			txtGeneralRemark.Text = "";
			luk_SalesBuyer.ItemIndex = -1;
			txtPaymentInterestonArrears.Text = "";
			luk_PaymentPriceList.ItemIndex = -1;
			txtPaymentTotalDiscount.Text = "";
			txtPaymentCreditLimit.Text = "";
			txtPaymentCommitmentLimit.Text = "";
			txt_BPBankCode.Text = "";
			txt_BPBankAccount.Text = "";
			txt_DflSwift.Text = "";
			luk_CreditCardType.ItemIndex = -1;
			txtPaymentCreditCardNo.Text = "";
			datePaymentExpirationDate.DateTime = DateTime.Now;
			txtPaymentIDNumber.Text = "";
			txtPaymentAvarageDelay.Text = "";
			try
			{
				picRemarks.Image = null;
				_picFilePath = "";
				btn_PicDelete.Visible = false;
			}
			catch
			{
			}
			memoRemarks.Text = "";
			refreshGridSource();
			luk_Technician.EditValue = -1;
			luk_Territory.EditValue = -1;
			luk_Language.EditValue = -1;
			_isAskPaymentTerm = false;
			luk_SalesBuyer.ItemIndex = 0;
			luk_ShippingType.ItemIndex = 0;
			luk_Currency.ItemIndex = 0;
			luk_CreditCardType.ItemIndex = 0;
			luk_PaymentTerm.ItemIndex = 0;
			WinFuntions.GetTableStruct(ref _dtBpBankAccount, "OCRB", _lstStructDt);
			WinFuntions.GetTableStruct(ref _dtPaymentDates, "CRD5", _lstStructDt);
			rdg_Active.SelectedIndex = 0;
			_isLoading = false;
			_isAskPaymentTerm = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRecord(string position, string cardCode)
	{
		try
		{
			_isLoading = true;
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				int defaultBranch = GlobalData.DefaultBranch;
				DataTable record = _httpClient.BusinessMaster.GetRecord(position, cardCode, userCodeLogin, defaultBranch);
				if (record != null && record.Rows.Count > 0)
				{
					await ShowData(record);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetADoc(string CardCode, int cntCode = 0)
	{
		try
		{
			DataTable dtBP = await _httpClient.BusinessMaster.GetADoc(CardCode);
			BeginInvoke((MethodInvoker)(async () =>
			{
				if (dtBP != null && dtBP.Rows.Count > 0)
				{
					await ShowData(dtBP);
					if (cntCode > 0)
					{
						xTab_BP.SelectedTabPage = tab_ContactPerson;
						for (int i = 0; i < gv_Contact.DataRowCount; i++)
						{
							if (Function.ParseInt(gv_Contact.GetRowCellValue(i, "CntctCode")) == cntCode)
							{
								gv_Contact.FocusedRowHandle = i;
								break;
							}
						}
					}
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SearchList()
	{
		_ = 1;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Series", typeof(int));
			dataTable.Columns.Add("CardCode", typeof(string));
			dataTable.Columns.Add("CardName", typeof(string));
			dataTable.Columns.Add("CardFName", typeof(string));
			dataTable.Columns.Add("ShipType", typeof(int));
			dataTable.Columns.Add("CardType", typeof(string));
			dataTable.Columns.Add("GroupCode", typeof(int));
			dataTable.Columns.Add("LicTradNum", typeof(string));
			dataTable.Columns.Add("Currency", typeof(string));
			dataTable.Columns.Add("Phone1", typeof(string));
			dataTable.Columns.Add("Phone2", typeof(string));
			dataTable.Columns.Add("E_Mail", typeof(string));
			dataTable.Columns.Add("Cellular", typeof(string));
			dataTable.Columns.Add("Fax", typeof(string));
			dataTable.Columns.Add("IntrntSite", typeof(string));
			DataTable dataSourceTable = grd_Udf.DataSourceTable;
			List<DataColumn> list = new List<DataColumn>();
			if (dataSourceTable.IsNotNull())
			{
				list = (from DataColumn t in dataSourceTable.Columns
					where Function.ToString(t.ColumnName).StartsWith("U_")
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					dataTable.Columns.Add(list[num].ColumnName, list[num].DataType);
				}
			}
			DataRow dataRow = dataTable.NewRow();
			if (!string.IsNullOrEmpty(luk_Series.Text))
			{
				dataRow["Series"] = Function.ParseInt(luk_Series.EditValue);
			}
			dataRow["CardCode"] = txt_CardCode.Text.Trim();
			dataRow["CardName"] = txt_CardName.Text.Trim();
			dataRow["CardFName"] = txtForeignName.Text.Trim();
			if (!string.IsNullOrEmpty(luk_ShippingType.Text))
			{
				dataRow["ShipType"] = Function.ParseInt(luk_ShippingType.EditValue);
			}
			dataRow["CardType"] = Function.ToString(luk_CardType.EditValue);
			if (!string.IsNullOrEmpty(luk_CardGroup.Text))
			{
				dataRow["GroupCode"] = Function.ParseInt(luk_CardGroup.EditValue);
			}
			dataRow["LicTradNum"] = Function.ToString(txtFederalTaxID.EditValue);
			dataRow["Currency"] = Function.ToString(luk_Currency.EditValue);
			dataRow["Phone1"] = Function.ToString(txtGeneralTel1.EditValue);
			dataRow["Phone2"] = Function.ToString(txtGeneralTel2.EditValue);
			dataRow["E_Mail"] = Function.ToString(txtGeneralFax.EditValue);
			dataRow["E_Mail"] = Function.ToString(txtGeneralEmail.EditValue);
			dataRow["Cellular"] = Function.ToString(txtGeneralMobilePhone.EditValue);
			dataRow["Fax"] = Function.ToString(txtGeneralFax.EditValue);
			dataRow["IntrntSite"] = Function.ToString(txtGeneralWebsite.EditValue);
			if (dataSourceTable.IsNotNull())
			{
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					dataRow[list[num2].ColumnName] = dataSourceTable.Rows[0][list[num2].ColumnName];
				}
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			WinCommons._OpenProcessing();
			DataTable dataTable2 = _httpClient.BusinessMaster.Search(dataTable);
			if (dataTable2 != null)
			{
				if (dataTable2.Rows.Count > 1)
				{
					frm_SearchDataList frm_SearchDataList2 = new frm_SearchDataList
					{
						SourceDt = dataTable2,
						XFormId = "OCRD_SR",
						ItemId = "GVDOC",
						Text = "Business Master Data List",
						ObjType = ApzObjectTypes.oBusinessPartners
					};
					frm_SearchDataList2.ShowDialog(this);
					if (frm_SearchDataList2.ChooseItem != null)
					{
						await GetADoc(Function.ToString(frm_SearchDataList2.ChooseItem["CardCode"]));
					}
					else
					{
						btAdd.Enabled = true;
					}
				}
				else if (dataTable2.Rows.Count == 1)
				{
					await GetADoc(Function.ToString(dataTable2.Rows[0]["CardCode"]));
				}
				else
				{
					btAdd.Enabled = true;
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			btAdd.Enabled = true;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void AddContactPerson()
	{
		try
		{
			int ctnCode = Function.ParseInt(gv_Contact.GetFocusedRowCellValue("CntctCode"));
			if (txt_Name.Text.Trim() == "" || txt_Name.Text.Trim().ToUpper() == "ADD NEW" || _isLoading)
			{
				return;
			}
			DataTable dt = grd_Contact.DataSource as DataTable;
			if ((dt == null || dt.Columns.Count == 0) && !WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt))
			{
				return;
			}
			int focusedRowHandle = 0;
			if (ctnCode == -1)
			{
				DataRow dataRow = dt.NewRow();
				dataRow["CardCode"] = txt_CardCode.Text;
				dataRow["Name"] = txt_Name.Text;
				dataRow["Title"] = txt_Title.Text;
				dataRow["Position"] = txt_Position.Text;
				dataRow["Address"] = txt_Address.Text;
				dataRow["Tel1"] = txt_Tel1.Text.Replace("_", "");
				dataRow["Tel2"] = txt_Tel2.Text.Replace("_", "");
				dataRow["Cellolar"] = txt_Cellolar.Text.Replace("_", "");
				dataRow["Fax"] = txt_Fax.Text.Replace("_", "");
				dataRow["E_MailL"] = txt_E_MailL.Text;
				dataRow["Pager"] = txt_Pager.Text;
				dataRow["Notes1"] = txt_Notes1.Text;
				dataRow["Notes2"] = txt_Notes2.Text;
				dataRow["Password"] = txt_Password.Text;
				dataRow["BirthPlace"] = Function.ToString(txt_BirthPlace.EditValue);
				DateTime? dateTime = Function.ParseDateTime(date_BirthDate.EditValue);
				if (!dateTime.HasValue)
				{
					dataRow["BirthDate"] = DBNull.Value;
				}
				else
				{
					dataRow["BirthDate"] = dateTime.Value;
				}
				string text = cb_Gender.Text;
				switch (text)
				{
				default:
					if (text.Length == 0)
					{
						dataRow["Gender"] = "E";
					}
					break;
				case "Male":
					dataRow["Gender"] = "M";
					break;
				case "Female":
					dataRow["Gender"] = "F";
					break;
				case null:
					break;
				}
				dataRow["Profession"] = txt_Profession.Text;
				dataRow["BirthCity"] = txt_BirthCity.Text;
				dataRow["Active"] = (chk_Active.Checked ? "Y" : "N");
				int num = ((dt.Rows.Count == 0) ? (-2) : (dt.AsEnumerable().Min((DataRow x) => Function.ParseInt(x["CntctCode"])) - 1));
				dataRow["CntctCode"] = num;
				foreach (DataColumn item in from DataColumn t in dataRow.Table.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					if (_dicContactUdfControl.ContainsKey(item.ColumnName))
					{
						dataRow[item.ColumnName] = _dicContactUdfControl[item.ColumnName].EditValue;
					}
				}
				if (Function.ParseInt(dataRow["CntctCode"]) != -1)
				{
					focusedRowHandle = dt.Rows.Count - 1;
					dt.Rows.InsertAt(dataRow, dt.Rows.Count - 1);
				}
			}
			else if (dt != null && dt.AsEnumerable().Any((DataRow x) => Function.ParseInt(x["CntctCode"]) == ctnCode))
			{
				DataRow dataRow2 = dt.AsEnumerable().FirstOrDefault((DataRow x) => Function.ParseInt(x["CntctCode"]) == ctnCode);
				dataRow2["Name"] = txt_Name.Text;
				dataRow2["Title"] = txt_Title.Text;
				dataRow2["Position"] = txt_Position.Text;
				dataRow2["Address"] = txt_Address.Text;
				dataRow2["Tel1"] = txt_Tel1.Text.Replace("_", "");
				dataRow2["Tel2"] = txt_Tel2.Text.Replace("_", "");
				dataRow2["Cellolar"] = txt_Cellolar.Text.Replace("_", "");
				dataRow2["Fax"] = txt_Fax.Text.Replace("_", "");
				dataRow2["E_MailL"] = txt_E_MailL.Text;
				dataRow2["Pager"] = txt_Pager.Text;
				dataRow2["Notes1"] = txt_Notes1.Text;
				dataRow2["Notes2"] = txt_Notes2.Text;
				dataRow2["Password"] = txt_Password.Text;
				dataRow2["BirthPlace"] = Function.ToString(txt_BirthPlace.EditValue);
				DateTime? dateTime2 = Function.ParseDateTime(date_BirthDate.EditValue);
				if (!dateTime2.HasValue)
				{
					dataRow2["BirthDate"] = DBNull.Value;
				}
				else
				{
					dataRow2["BirthDate"] = dateTime2.Value;
				}
				string text = cb_Gender.Text;
				switch (text)
				{
				default:
					if (text.Length == 0)
					{
						dataRow2["Gender"] = "E";
					}
					break;
				case "Male":
					dataRow2["Gender"] = "M";
					break;
				case "Female":
					dataRow2["Gender"] = "F";
					break;
				case null:
					break;
				}
				dataRow2["Profession"] = txt_Profession.Text;
				dataRow2["BirthCity"] = txt_BirthCity.Text;
				dataRow2["Active"] = (chk_Active.Checked ? "Y" : "N");
				foreach (DataColumn item2 in from DataColumn t in dataRow2.Table.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					if (_dicContactUdfControl.ContainsKey(item2.ColumnName))
					{
						dataRow2[item2.ColumnName] = _dicContactUdfControl[item2.ColumnName].EditValue;
					}
				}
			}
			dt.AcceptChanges();
			grd_Contact.DataSource = dt;
			grd_Contact.RefreshDataSource();
			if (ctnCode == -1)
			{
				gv_Contact.FocusedRowHandle = focusedRowHandle;
			}
			if (dt != null && dt.Rows.Count == 2)
			{
				DataRow dataRow3 = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).ToUpper() != "ADD NEW");
				if (dataRow3 != null)
				{
					_defaultContact = Function.ToString(dataRow3["Name"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UpdateContactPerson()
	{
		try
		{
			int ctnCode = Function.ParseInt(gv_Contact.GetFocusedRowCellValue("CntctCode"));
			if (txt_Name.Text.Trim() == "" || txt_Name.Text.Trim().ToUpper() == "ADD NEW" || _isLoading || ctnCode == -1)
			{
				return;
			}
			DataTable dt = grd_Contact.DataSource as DataTable;
			if ((dt == null || dt.Columns.Count == 0) && !WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt))
			{
				return;
			}
			if (dt != null && dt.AsEnumerable().Any((DataRow x) => Function.ParseInt(x["CntctCode"]) == ctnCode))
			{
				DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow x) => Function.ParseInt(x["CntctCode"]) == ctnCode);
				dataRow["Name"] = txt_Name.Text;
				dataRow["Title"] = txt_Title.Text;
				dataRow["Position"] = txt_Position.Text;
				dataRow["Address"] = txt_Address.Text;
				dataRow["Tel1"] = txt_Tel1.Text.Replace("_", "");
				dataRow["Tel2"] = txt_Tel2.Text.Replace("_", "");
				dataRow["Cellolar"] = txt_Cellolar.Text.Replace("_", "");
				dataRow["Fax"] = txt_Fax.Text.Replace("_", "");
				dataRow["E_MailL"] = txt_E_MailL.Text;
				dataRow["Pager"] = txt_Pager.Text;
				dataRow["Notes1"] = txt_Notes1.Text;
				dataRow["Notes2"] = txt_Notes2.Text;
				dataRow["Password"] = txt_Password.Text;
				dataRow["BirthPlace"] = Function.ToString(txt_BirthPlace.EditValue);
				DateTime? dateTime = Function.ParseDateTime(date_BirthDate.EditValue);
				if (!dateTime.HasValue)
				{
					dataRow["BirthDate"] = DBNull.Value;
				}
				else
				{
					dataRow["BirthDate"] = dateTime.Value;
				}
				string text = cb_Gender.Text;
				switch (text)
				{
				default:
					if (text.Length == 0)
					{
						dataRow["Gender"] = "E";
					}
					break;
				case "Male":
					dataRow["Gender"] = "M";
					break;
				case "Female":
					dataRow["Gender"] = "F";
					break;
				case null:
					break;
				}
				dataRow["Profession"] = txt_Profession.Text;
				dataRow["BirthCity"] = txt_BirthCity.Text;
				dataRow["Active"] = (chk_Active.Checked ? "Y" : "N");
				foreach (DataColumn item in from DataColumn t in dataRow.Table.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					if (_dicContactUdfControl.ContainsKey(item.ColumnName))
					{
						dataRow[item.ColumnName] = _dicContactUdfControl[item.ColumnName].EditValue;
					}
				}
			}
			dt.AcceptChanges();
			grd_Contact.DataSource = dt;
			grd_Contact.RefreshDataSource();
			if (dt != null && dt.Rows.Count == 2)
			{
				DataRow dataRow2 = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).ToUpper() != "ADD NEW");
				if (dataRow2 != null)
				{
					_defaultContact = Function.ToString(dataRow2["Name"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void refreshGridSource()
	{
		if (grd_Contact.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				DataTable dt5 = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt5, "CRD1", _lstStructDt))
				{
					grd_Address.DataSource = dt5;
				}
				else
				{
					grd_Address.DataSource = null;
				}
				grd_Address.RefreshDataSource();
				DataTable dt6 = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt6, "OCRD", _lstStructDt))
				{
					if (dt6.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
					{
						DataRow row2 = dt6.NewRow();
						dt6.Rows.Add(row2);
						dt6.AcceptChanges();
						grd_Udf.DataSource = dt6;
					}
				}
				else
				{
					grd_Udf.DataSource = null;
				}
				grd_Udf.RefreshDataSource();
				DataTable dt7 = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt7, "ATC1", _lstStructDt))
				{
					ucAttachment1.grd_File.DataSource = dt7;
				}
				else
				{
					ucAttachment1.grd_File.DataSource = null;
				}
				ucAttachment1.grd_File.RefreshDataSource();
				DataTable dt8 = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt8, "OCPR", _lstStructDt))
				{
					grd_Contact.DataSource = dt8;
				}
				else
				{
					grd_Contact.DataSource = null;
				}
				grd_Contact.RefreshDataSource();
			}));
			return;
		}
		DataTable dt = new DataTable();
		if (WinFuntions.GetTableStruct(ref dt, "CRD1", _lstStructDt))
		{
			grd_Address.DataSource = dt;
		}
		else
		{
			grd_Address.DataSource = null;
		}
		grd_Address.RefreshDataSource();
		DataTable dt2 = new DataTable();
		if (WinFuntions.GetTableStruct(ref dt2, "OCRD", _lstStructDt))
		{
			if (dt2.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
			{
				DataRow row = dt2.NewRow();
				dt2.Rows.Add(row);
				dt2.AcceptChanges();
				grd_Udf.DataSource = dt2;
			}
		}
		else
		{
			grd_Udf.DataSource = null;
		}
		grd_Udf.RefreshDataSource();
		DataTable dt3 = new DataTable();
		if (WinFuntions.GetTableStruct(ref dt3, "ATC1", _lstStructDt))
		{
			ucAttachment1.grd_File.DataSource = dt3;
		}
		else
		{
			ucAttachment1.grd_File.DataSource = null;
		}
		ucAttachment1.grd_File.RefreshDataSource();
		DataTable dt4 = new DataTable();
		if (WinFuntions.GetTableStruct(ref dt4, "OCPR", _lstStructDt))
		{
			grd_Contact.DataSource = dt4;
		}
		else
		{
			grd_Contact.DataSource = null;
		}
		grd_Contact.RefreshDataSource();
	}

	private void SetContactPersonControl(DataRow r)
	{
		try
		{
			DataTable dt = new DataTable();
			if (!WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt))
			{
				return;
			}
			dt.ImportRow(r);
			dt.AcceptChanges();
			txt_Name.EditValue = Function.ToString(r["Name"]);
			txt_Title.EditValue = Function.ToString(r["Title"]);
			txt_Position.EditValue = Function.ToString(r["Position"]);
			txt_Address.EditValue = Function.ToString(r["Address"]);
			txt_Tel1.EditValue = Function.ToString(r["Tel1"]);
			txt_Tel2.EditValue = Function.ToString(r["Tel2"]);
			txt_Cellolar.EditValue = Function.ToString(r["Cellolar"]);
			txt_Fax.EditValue = Function.ToString(r["Fax"]);
			txt_E_MailL.EditValue = Function.ToString(r["E_MailL"]);
			chk_Active.EditValue = Function.ToString(r["Active"]);
			txt_Pager.EditValue = Function.ToString(r["Pager"]);
			txt_Notes1.EditValue = Function.ToString(r["Notes1"]);
			txt_Notes2.EditValue = Function.ToString(r["Notes2"]);
			txt_Password.EditValue = Function.ToString(r["Password"]);
			txt_BirthPlace.EditValue = Function.ToString(r["BirthPlace"]);
			date_BirthDate.EditValue = Function.ParseDateTime(r["BirthDate"]);
			cb_Gender.EditValue = Function.ToString(r["Gender"]);
			txt_Profession.EditValue = Function.ToString(r["Profession"]);
			txt_BirthCity.EditValue = Function.ToString(r["BirthCity"]);
			switch (Function.ToString(dt.Rows[0]["Gender"]))
			{
			case "M":
				cb_Gender.Text = "Male";
				break;
			case "F":
				cb_Gender.Text = "Female";
				break;
			case "E":
				cb_Gender.Text = "";
				break;
			}
			if (Function.ToString(dt.Rows[0]["Name"]).ToUpper() == "ADD NEW")
			{
				chk_Active.Checked = true;
			}
			else
			{
				chk_Active.Checked = Function.ToString(dt.Rows[0]["Active"]) == "Y";
			}
			foreach (DataColumn item in from DataColumn t in r.Table.Columns
				where t.ColumnName.StartsWith("U_")
				select t)
			{
				if (_dicContactUdfControl.ContainsKey(item.ColumnName))
				{
					_dicContactUdfControl[item.ColumnName].EditValue = r[item.ColumnName];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddBMD()
	{
		if ((txt_CardCode.Text == "" && FormMode == Enums.FormViewModeType.Update) || txt_CardCode.Text.Trim() == "")
		{
			WinCommons.Error(Enums.MessageCode.BpCodeCannotBeEmpty, "");
		}
		else
		{
			if (!ValidateSave() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			try
			{
				BPImpls BMDItem = GetValueFromDatasource();
				BMDItem.OldCardCode = _oldCardCode;
				if (FormMode != Enums.FormViewModeType.Update)
				{
					if (IsManualCode())
					{
						BMDItem.CardCode = txt_CardCode.Text;
					}
					else
					{
						BMDItem.CardCode = string.Empty;
					}
					BMDItem.Balance = 0.0;
					BMDItem.DNotesBal = 0.0;
					BMDItem.OrdersBal = 0.0;
				}
				BMDItem.Series = Function.ParseInt(luk_Series.EditValue);
				DateTime? dateTime = Function.ParseDateTime(dateValidFrom2.EditValue);
				DateTime? dateTime2 = Function.ParseDateTime(dateValidTo2.EditValue);
				DateTime? dateTime3 = Function.ParseDateTime(dateFrozenFrom2.EditValue);
				DateTime? dateTime4 = Function.ParseDateTime(dateFrozenTo2.EditValue);
				DateTime? validFrom2;
				DateTime? frozenFrom2;
				DateTime? validFrom;
				DateTime? frozenFrom;
				DateTime? frozenTo;
				DateTime? frozenTo2;
				DateTime? validTo;
				DateTime? validTo2;
				switch (rdg_Active.SelectedIndex)
				{
				case 0:
					BMDItem.validFor = "Y";
					BMDItem.frozenFor = "N";
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						if (dateTime.Value.Year >= 1990)
						{
							validFrom2 = dateTime;
							goto IL_021d;
						}
					}
					validFrom2 = null;
					goto IL_021d;
				case 1:
					BMDItem.validFor = "N";
					BMDItem.validFrom = null;
					BMDItem.validTo = null;
					BMDItem.frozenFor = "Y";
					if (dateTime3.HasValue)
					{
						_ = dateTime3.Value;
						if (dateTime3.Value.Year >= 1990)
						{
							frozenFrom2 = dateTime3;
							goto IL_0317;
						}
					}
					frozenFrom2 = null;
					goto IL_0317;
				case 2:
					BMDItem.validFor = "Y";
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						if (dateTime.Value.Year >= 1990)
						{
							validFrom = dateTime;
							goto IL_03ad;
						}
					}
					validFrom = null;
					goto IL_03ad;
				default:
					{
						BMDItem.validFor = "N";
						BMDItem.frozenFor = "N";
						break;
					}
					IL_043e:
					BMDItem.frozenFrom = frozenFrom;
					if (dateTime4.HasValue)
					{
						_ = dateTime4.Value;
						if (dateTime4.Value.Year >= 1990)
						{
							frozenTo = dateTime4;
							goto IL_047f;
						}
					}
					frozenTo = null;
					goto IL_047f;
					IL_0317:
					BMDItem.frozenFrom = frozenFrom2;
					if (dateTime4.HasValue)
					{
						_ = dateTime4.Value;
						if (dateTime4.Value.Year >= 1990)
						{
							frozenTo2 = dateTime4;
							goto IL_0358;
						}
					}
					frozenTo2 = null;
					goto IL_0358;
					IL_03ad:
					BMDItem.validFrom = validFrom;
					if (dateTime2.HasValue)
					{
						_ = dateTime2.Value;
						if (dateTime2.Value.Year >= 1990)
						{
							validTo = dateTime2;
							goto IL_03ed;
						}
					}
					validTo = null;
					goto IL_03ed;
					IL_025d:
					BMDItem.validTo = validTo2;
					BMDItem.frozenFrom = null;
					BMDItem.frozenTo = null;
					break;
					IL_047f:
					BMDItem.frozenTo = frozenTo;
					break;
					IL_021d:
					BMDItem.validFrom = validFrom2;
					if (dateTime2.HasValue)
					{
						_ = dateTime2.Value;
						if (dateTime2.Value.Year >= 1990)
						{
							validTo2 = dateTime2;
							goto IL_025d;
						}
					}
					validTo2 = null;
					goto IL_025d;
					IL_03ed:
					BMDItem.validTo = validTo;
					BMDItem.frozenFor = "Y";
					if (dateTime3.HasValue)
					{
						_ = dateTime3.Value;
						if (dateTime3.Value.Year >= 1990)
						{
							frozenFrom = dateTime3;
							goto IL_043e;
						}
					}
					frozenFrom = null;
					goto IL_043e;
					IL_0358:
					BMDItem.frozenTo = frozenTo2;
					break;
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					BMDItem.CreateDate = DateTime.Now.Date;
					BMDItem.OldCardCode = "";
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					BMDItem.UserSign = GlobalData.UserID;
					BMDItem.UserSign2 = GlobalData.UserID;
				}
				else
				{
					BMDItem.UserSign2 = GlobalData.UserID;
				}
				BMDItem.UserSign = 3;
				BMDItem.UserSign2 = 3;
				BMDItem.ObjType = "2";
				BMDItem.Deleted = "";
				BMDItem.DocEntry = 0;
				BMDItem.DflBankKey = _bankKey;
				BMDItem.DeferrTax = (chk_DeferrTax.Checked ? "Y" : "N");
				BMDItem.Affiliate = (chk_Affiliate.Checked ? "Y" : "N");
				BMDItem.FatherType = ((rdg_FatherType.SelectedIndex == 0) ? "P" : "D");
				BMDItem.BlockDunn = (chk_BlockDunn.Checked ? "Y" : "N");
				BMDItem.UseShpdGd = (chk_UseShpdGd.Checked ? "Y" : "N");
				BMDItem.VatStatus = Function.ToString(luk_VatStatus.EditValue);
				BMDItem.VatGroup = Function.ToString(bte_ECVatGroup.EditValue);
				if (_dtBpControlAccounts.IsNotNull())
				{
					List<BPAccountReceivablePaybleImpl> list = new List<BPAccountReceivablePaybleImpl>();
					for (int i = 0; i < _dtBpControlAccounts.Rows.Count; i++)
					{
						BPAccountReceivablePaybleImpl bPAccountReceivablePaybleImpl = new BPAccountReceivablePaybleImpl();
						bPAccountReceivablePaybleImpl.AcctCode = Function.ToString(_dtBpControlAccounts.Rows[i]["AcctCode"]);
						bPAccountReceivablePaybleImpl.AcctType = Function.ToString(_dtBpControlAccounts.Rows[i]["AcctType"]);
						bPAccountReceivablePaybleImpl.CardCode = Function.ToString(_dtBpControlAccounts.Rows[i]["CardCode"]);
						bPAccountReceivablePaybleImpl.LogInstane = Function.ParseInt(_dtBpControlAccounts.Rows[i]["LogInstane"]);
						bPAccountReceivablePaybleImpl.ObjType = Function.ParseShort(_dtBpControlAccounts.Rows[i]["ObjType"]).Value;
						list.Add(bPAccountReceivablePaybleImpl);
					}
					BMDItem.AccountRecivablePayables = list;
				}
				BMDItem.HsBnkIBAN = Function.ToString(luk_HsBnkIBAN.Text);
				BMDItem.CollecAuth = (chk_CollecAuth.Checked ? "Y" : "N");
				BMDItem.AutoCalBCG = (chk_AutoCalBCG.Checked ? "Y" : "N");
				if (grd_Contact.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					List<ContactPersonImpl> list2 = (List<ContactPersonImpl>)TableToEntity("OCPR", dataTable);
					list2.RemoveAll((ContactPersonImpl x) => x.Name.ToUpper() == "ADD NEW");
					if (list2 != null && list2.Count > 0)
					{
						BMDItem.ListContactPerson = list2;
					}
				}
				List<AttachmentFile_Impl> list3 = ucAttachment1.UploadFile();
				if (ucAttachment1.DataSource.IsNotNull() && list3.Count == 0)
				{
					return;
				}
				BMDItem.AttachmentFiles = list3;
				_lstUdfFields = await WinFuntions.GetUdfFields("OCRD");
				if (_lstUdfFields != null && _lstUdfFields.Count > 0 && grd_Udf.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
				{
					DataRow dataRow = dataTable2.Rows[0];
					BMDItem.ListUserFields = new List<UserFieldsImpl>();
					for (int num = 0; num < _lstUdfFields.Count; num++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = _lstUdfFields[num].Name;
						userFieldsImpl.Value = dataRow[_lstUdfFields[num].Name];
						userFieldsImpl.Type = _lstUdfFields[num].Type;
						BMDItem.ListUserFields.Add(userFieldsImpl);
					}
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					BMDItem.Address = string.Empty;
				}
				if (grd_Address.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
				{
					List<AddressesImpl> list4 = (List<AddressesImpl>)TableToEntity("CRD1", dataTable3);
					list4.RemoveAll((AddressesImpl x) => string.IsNullOrWhiteSpace(x.Address));
					BMDItem.Addresses = list4;
				}
				if (_dtBpBankAccount != null && _dtBpBankAccount.Rows.Count > 0)
				{
					for (int num2 = _dtBpBankAccount.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow dataRow2 = _dtBpBankAccount.Rows[num2];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow2["BankCode"])))
						{
							_dtBpBankAccount.Rows.Remove(dataRow2);
							_dtBpBankAccount.AcceptChanges();
						}
					}
					if (_dtBpBankAccount.Rows.Count > 0)
					{
						List<BankAccountsImpl> bPBankAccounts = (List<BankAccountsImpl>)TableToEntity("OCRB", _dtBpBankAccount);
						BMDItem.BPBankAccounts = bPBankAccounts;
					}
				}
				if (grd_Properties.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
				{
					List<string> list5 = new List<string>();
					for (int num3 = 0; num3 < dataTable4.Rows.Count; num3++)
					{
						list5.Add(Function.ToString(dataTable4.Rows[num3]["Check"]));
					}
					BMDItem.ItemProperties = list5;
				}
				string pictureName = "";
				if (picRemarks.EditValue != null && _picFilePath != "" && !WinFuntions.UploadPictureFile(_picFilePath, ref pictureName))
				{
					btAdd.Enabled = true;
					return;
				}
				BMDItem.Picture = pictureName;
				if (_dtPaymentDates != null && _dtPaymentDates.Rows.Count > 0)
				{
					for (int num4 = _dtPaymentDates.Rows.Count - 1; num4 >= 0; num4--)
					{
						DataRow dataRow3 = _dtPaymentDates.Rows[num4];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow3["PmntDate"])))
						{
							_dtPaymentDates.Rows.Remove(dataRow3);
							_dtPaymentDates.AcceptChanges();
						}
					}
					if (_dtPaymentDates.Rows.Count > 0)
					{
						List<PaymentDatesImpl> bPPaymentDates = (List<PaymentDatesImpl>)TableToEntity("CRD5", _dtPaymentDates);
						BMDItem.BPPaymentDates = bPPaymentDates;
					}
				}
				BMDItem.CntctPrsn = _defaultContact;
				BMDItem.BillToDef = _defaultBillAddress;
				BMDItem.ShipToDef = _defaultShipAddress;
				if (_dtBranchAssignment != null && _dtBranchAssignment.Rows.Count > 0)
				{
					BMDItem.BPBranchAssignment = new List<BPBranchAssignmentImpls>();
					for (int num5 = 0; num5 < _dtBranchAssignment.Rows.Count; num5++)
					{
						if (Function.ToString(_dtBranchAssignment.Rows[num5]["Checked"]).ToLower() == "y")
						{
							BPBranchAssignmentImpls bPBranchAssignmentImpls = new BPBranchAssignmentImpls();
							bPBranchAssignmentImpls.BPLId = Function.ParseInt(_dtBranchAssignment.Rows[num5]["BPLId"]);
							BMDItem.BPBranchAssignment.Add(bPBranchAssignmentImpls);
						}
					}
				}
				WinCommons._OpenProcessing();
				string text = _httpClient.BusinessMaster.SaveDoc(BMDItem);
				WinCommons._CloseProcessing();
				if (string.IsNullOrEmpty(text))
				{
					isAddBMDComplete = true;
					if (RefreshQuickCreation != null)
					{
						RefreshQuickCreation(isAddBMDComplete, BMDItem.CardCode);
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
					if (FormMode == Enums.FormViewModeType.Add)
					{
						await REFRESH();
					}
					else if (FormMode == Enums.FormViewModeType.Update)
					{
						FormMode = Enums.FormViewModeType.View;
					}
				}
				else
				{
					isAddBMDComplete = false;
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		}
	}

	private bool ValidateSave()
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(dateValidFrom.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(dateValidTo.EditValue);
			DateTime? dateTime3 = Function.ParseDateTime(dateFrozenFrom.EditValue);
			DateTime? dateTime4 = Function.ParseDateTime(dateFrozenTo.EditValue);
			switch (rdg_Active.SelectedIndex)
			{
			case 0:
				if (!dateTime.HasValue || !dateTime2.HasValue)
				{
					break;
				}
				_ = dateTime.Value;
				if (dateTime.Value.Year > 2000)
				{
					_ = dateTime2.Value;
					if (dateTime2.Value.Year > 2000 && dateTime.Value.CompareTo(dateTime2.Value) >= 0)
					{
						WinCommons.Error(Enums.MessageCode.InvalidDateSpecifyToDateNoEarlierThanFromDate, "");
						return false;
					}
				}
				break;
			case 1:
				if (!dateTime3.HasValue || !dateTime4.HasValue)
				{
					break;
				}
				_ = dateTime3.Value;
				if (dateTime3.Value.Year > 2000)
				{
					_ = dateTime4.Value;
					if (dateTime4.Value.Year > 2000 && dateTime3.Value.CompareTo(dateTime4.Value) >= 0)
					{
						WinCommons.Error(Enums.MessageCode.InvalidDateSpecifyToDateNoEarlierThanFromDate, "");
						return false;
					}
				}
				break;
			case 2:
			{
				dateTime = Function.ParseDateTime(dateValidFrom2.EditValue);
				dateTime2 = Function.ParseDateTime(dateValidTo2.EditValue);
				dateTime3 = Function.ParseDateTime(dateFrozenFrom2.EditValue);
				dateTime4 = Function.ParseDateTime(dateFrozenTo2.EditValue);
				if (dateTime.HasValue && dateTime2.HasValue)
				{
					_ = dateTime.Value;
					if (dateTime.Value.Year > 2000)
					{
						_ = dateTime2.Value;
						if (dateTime2.Value.Year > 2000 && dateTime.Value.CompareTo(dateTime2.Value) >= 0)
						{
							WinCommons.Error(Enums.MessageCode.InvalidDateSpecifyToDateNoEarlierThanFromDate, "");
							return false;
						}
					}
				}
				if (dateTime3.HasValue && dateTime4.HasValue)
				{
					_ = dateTime3.Value;
					if (dateTime3.Value.Year > 2000)
					{
						_ = dateTime4.Value;
						if (dateTime4.Value.Year > 2000 && dateTime3.Value.CompareTo(dateTime4.Value) >= 0)
						{
							WinCommons.Error(Enums.MessageCode.InvalidDateSpecifyToDateNoEarlierThanFromDate, "");
							return false;
						}
					}
				}
				DateTime? dateTime5;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime5 = dateTime;
				}
				else
				{
					dateTime5 = new DateTime(1950, 1, 1);
				}
				dateTime = dateTime5;
				DateTime? dateTime6;
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					dateTime6 = dateTime2;
				}
				else
				{
					dateTime6 = new DateTime(2100, 1, 1);
				}
				dateTime2 = dateTime6;
				DateTime? dateTime7;
				if (dateTime3.HasValue)
				{
					_ = dateTime3.Value;
					dateTime7 = dateTime3;
				}
				else
				{
					dateTime7 = new DateTime(1950, 1, 1);
				}
				dateTime3 = dateTime7;
				DateTime? dateTime8;
				if (dateTime4.HasValue)
				{
					_ = dateTime4.Value;
					dateTime8 = dateTime4;
				}
				else
				{
					dateTime8 = new DateTime(2100, 1, 1);
				}
				dateTime4 = dateTime8;
				if ((dateTime.Value.CompareTo(dateTime3) <= 0 && dateTime3.Value.CompareTo(dateTime2) <= 0) || (dateTime.Value.CompareTo(dateTime4) <= 0 && dateTime4.Value.CompareTo(dateTime2) <= 0) || (dateTime3.Value.CompareTo(dateTime) <= 0 && dateTime.Value.CompareTo(dateTime4) <= 0) || (dateTime3.Value.CompareTo(dateTime2) <= 0 && dateTime2.Value.CompareTo(dateTime4) <= 0))
				{
					WinCommons.Error(Enums.MessageCode.DateRangeOverlapChangeAvtiveOrInactiveDateRange, "");
					return false;
				}
				break;
			}
			}
			_ = rdg_Active.SelectedIndex;
			_ = 2;
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task AddBMDNoSAP(string action)
	{
		if ((txt_CardCode.Text == "" && FormMode == Enums.FormViewModeType.Update) || txt_CardCode.Text.Trim() == "")
		{
			WinCommons.Error(Enums.MessageCode.BpCodeCannotBeEmpty, "");
		}
		else
		{
			if (!ValidateSave() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			try
			{
				DataTable dtBP = DataSource as DataTable;
				if (_oldCardCode.Trim() != txt_CardCode.Text.Trim() && !string.IsNullOrWhiteSpace(_oldCardCode))
				{
					DataTable dataTable = await _httpClient.BusinessMaster.ExistsStockTransaction(_oldCardCode);
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						WinCommons.Error(Enums.MessageCode.CannotRenameBpWhenTransactionExists, "");
						return;
					}
				}
				DataRow BMDItem = dtBP.Rows[0];
				if (FormMode != Enums.FormViewModeType.Update)
				{
					BMDItem["CardCode"] = txt_CardCode.Text;
					BMDItem["Balance"] = 0;
					BMDItem["DNotesBal"] = 0;
					BMDItem["OrdersBal"] = 0;
				}
				BMDItem["Series"] = Function.ParseInt(luk_Series.EditValue);
				switch (rdg_Active.SelectedIndex)
				{
				case 0:
					BMDItem["validFor"] = "Y";
					BMDItem["frozenFor"] = "N";
					break;
				case 1:
					BMDItem["validFor"] = "N";
					BMDItem["frozenFor"] = "Y";
					break;
				case 2:
					BMDItem["validFor"] = "Y";
					BMDItem["frozenFor"] = "Y";
					break;
				default:
					BMDItem["validFor"] = "N";
					BMDItem["frozenFor"] = "N";
					break;
				}
				DateTime? dateTime = Function.ParseDateTime(dateValidFrom.EditValue);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					if (dateTime.Value.Year >= 1990)
					{
						goto IL_034f;
					}
				}
				BMDItem["validFrom"] = new DateTime(1900, 1, 1);
				goto IL_034f;
				IL_0c28:
				DataTable dtPmntDate = new DataTable();
				if (_dtPaymentDates == null || _dtPaymentDates.Rows.Count == 0)
				{
					if (!WinFuntions.GetTableStruct(ref dtPmntDate, "CRD5", _lstStructDt))
					{
						return;
					}
				}
				else
				{
					for (int num = _dtPaymentDates.Rows.Count - 1; num >= 0; num--)
					{
						DataRow dataRow = _dtPaymentDates.Rows[num];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["PmntDate"])))
						{
							_dtPaymentDates.Rows.Remove(dataRow);
							_dtPaymentDates.AcceptChanges();
						}
					}
					if (_dtPaymentDates.Rows.Count > 0)
					{
						dtPmntDate = _dtPaymentDates.Copy();
					}
					else if (!WinFuntions.GetTableStruct(ref dtPmntDate, "CRD5", _lstStructDt))
					{
						return;
					}
				}
				BMDItem["CntctPrsn"] = _defaultContact;
				BMDItem["BillToDef"] = _defaultBillAddress;
				BMDItem["ShipToDef"] = _defaultShipAddress;
				DataTable dtCRD8 = WinFuntions.GetTableStructure("CRD8");
				if (_dtBranchAssignment != null && _dtBranchAssignment.Rows.Count > 0)
				{
					dtCRD8.Rows.Clear();
					dtCRD8.AcceptChanges();
					for (int i = 0; i < _dtBranchAssignment.Rows.Count; i++)
					{
						if (Function.ToString(_dtBranchAssignment.Rows[i]["Checked"]).ToLower() == "y")
						{
							DataRow dataRow2 = dtCRD8.NewRow();
							dataRow2["BPLId"] = Function.ParseInt(_dtBranchAssignment.Rows[i]["BPLId"]);
							dataRow2["CardCode"] = txt_CardCode.Text;
							dtCRD8.Rows.Add(dataRow2);
						}
					}
					dtCRD8.AcceptChanges();
				}
				dtBP.AcceptChanges();
				if (_dtOriginal != null && _dtOriginal.Rows.Count > 0)
				{
					using DataSet dsValidate = new DataSet();
					using DataTable dtOld = _dtOriginal.Copy();
					using DataTable dtNew = dtBP.Copy();
					dsValidate.Tables.Add(dtOld);
					dsValidate.Tables.Add(dtNew);
					string text = await _httpClient.BusinessMaster.ValidateChangedValue(dsValidate);
					if (!text.StartsWith("OK"))
					{
						WinCommons.Error(Enums.MessageCode.None, "Validate Error: " + text);
						return;
					}
					string[] array = text.Split('-');
					if (array != null && array.Length == 2 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeCurrencyForBpLinkDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						BMDItem["Currency"] = array[1];
						luk_Currency.EditValue = array[1];
						dtBP.AcceptChanges();
					}
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				dtBP.TableName = "BP";
				dtBP.DataSet?.Tables.Remove(dtBP.TableName);
				dataSet.Tables.Add(dtBP);
				DataTable dtAttachment;
				dtAttachment.TableName = "AttFile";
				dtAttachment.DataSet?.Tables.Remove(dtAttachment.TableName);
				dataSet.Tables.Add(dtAttachment);
				DataTable dtAddress;
				dtAddress.TableName = "Address";
				dtAddress.DataSet?.Tables.Remove(dtAddress.TableName);
				dataSet.Tables.Add(dtAddress);
				DataTable dtCntPrsn;
				dtCntPrsn.TableName = "CntPrsn";
				dtCntPrsn.DataSet?.Tables.Remove(dtCntPrsn.TableName);
				dataSet.Tables.Add(dtCntPrsn);
				DataTable dtBankAcct;
				dtBankAcct.TableName = "BankAcct";
				dtBankAcct.DataSet?.Tables.Remove(dtBankAcct.TableName);
				dataSet.Tables.Add(dtBankAcct);
				dtPmntDate.TableName = "PmntDate";
				dtPmntDate.DataSet?.Tables.Remove(dtPmntDate.TableName);
				dataSet.Tables.Add(dtPmntDate);
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add(new DataColumn("OldCode", typeof(string)));
				if (FormMode == Enums.FormViewModeType.Update && txt_CardCode.Text.Trim() != _oldCardCode.Trim())
				{
					DataRow dataRow3 = dataTable2.NewRow();
					dataRow3[0] = _oldCardCode.Trim();
					dataTable2.Rows.Add(dataRow3);
					dataTable2.AcceptChanges();
				}
				dataTable2.TableName = "OtherTable";
				dataTable2.DataSet?.Tables.Remove(dataTable2.TableName);
				dataSet.Tables.Add(dataTable2);
				dtCRD8.TableName = "CRD8";
				dtCRD8.DataSet?.Tables.Remove(dtCRD8.TableName);
				dataSet.Tables.Add(dtCRD8);
				dataSet.AcceptChanges();
				WinCommons._OpenProcessing();
				string text2 = "";
				if (action == "U")
				{
					text2 = _httpClient.BusinessMaster.SaveDocDataSetUpdate(dataSet);
				}
				else if (action == "A")
				{
					text2 = _httpClient.BusinessMaster.SaveDocDataSet(dataSet);
				}
				WinCommons._CloseProcessing();
				if (string.IsNullOrEmpty(text2))
				{
					if (FormMode == Enums.FormViewModeType.Add)
					{
						REFRESH();
					}
					if (FormMode == Enums.FormViewModeType.Update)
					{
						REFRESH_DATA();
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
				return;
				IL_046e:
				if (string.Compare(action, "A", ignoreCase: true) == 0)
				{
					BMDItem["CreateDate"] = DateTime.Now;
				}
				else
				{
					BMDItem["UpdateDate"] = DateTime.Now;
				}
				if (string.Compare(action, "A", ignoreCase: true) == 0)
				{
					BMDItem["UserSign"] = GlobalData.UserID;
					BMDItem["UserSign2"] = GlobalData.UserID;
				}
				else
				{
					BMDItem["UserSign2"] = GlobalData.UserID;
				}
				BMDItem["ObjType"] = "2";
				BMDItem["Deleted"] = "";
				BMDItem["DocEntry"] = 0;
				BMDItem["BackOrder"] = (chk_AllowPartialDlvrPerRow.Checked ? "Y" : "N");
				BMDItem["PartDelivr"] = (chk_AllowPartialDlvrOfSO.Checked ? "Y" : "N");
				BMDItem["NoDiscount"] = (chk_DoNotApplyDiscountGroup.Checked ? "Y" : "N");
				BMDItem["EdrsFromBP"] = (chk_EndorsableChecksFrom.Checked ? "Y" : "N");
				BMDItem["EdrsToBP"] = (chk_AcceptsEndorsedChecks.Checked ? "Y" : "N");
				BMDItem["DflBankKey"] = _bankKey;
				BMDItem["DeferrTax"] = (chk_DeferrTax.Checked ? "Y" : "N");
				BMDItem["Affiliate"] = (chk_Affiliate.Checked ? "Y" : "N");
				BMDItem["FatherType"] = ((rdg_FatherType.SelectedIndex == 0) ? "P" : "D");
				BMDItem["HsBnkIBAN"] = Function.ToString(luk_HsBnkIBAN.Text);
				BMDItem["CollecAuth"] = (chk_CollecAuth.Checked ? "Y" : "N");
				BMDItem["AutoCalBCG"] = (chk_AutoCalBCG.Checked ? "Y" : "N");
				dtAttachment = ucAttachment1.UploadFileNoSAP();
				if (!dtAttachment.IsNotNull() && ucAttachment1.DataSource.IsNotNull())
				{
					return;
				}
				dtAddress = grd_Address.DataSource as DataTable;
				if (dtAddress == null)
				{
					if (!WinFuntions.GetTableStruct(ref dtAddress, "CRD1", _lstStructDt))
					{
						return;
					}
				}
				else
				{
					for (int j = 0; j < dtAddress.Rows.Count; j++)
					{
						dtAddress.Rows[j]["CardCode"] = Function.ToString(BMDItem["CardCode"]);
					}
					for (int num2 = dtAddress.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow dataRow4 = dtAddress.Rows[num2];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow4["Address"])))
						{
							dtAddress.Rows.Remove(dataRow4);
							dtAddress.AcceptChanges();
						}
					}
				}
				string pictureName = "";
				if (picRemarks.EditValue != null && _picFilePath != "" && !WinFuntions.UploadPictureFile(_picFilePath, ref pictureName))
				{
					btAdd.Enabled = true;
					return;
				}
				BMDItem["Picture"] = pictureName;
				if (grd_Udf.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
				{
					DataTable dataTable4 = dataTable3.Copy();
					DataRow dataRow5 = dataTable4.Rows[0];
					for (int k = 0; k < dataTable4.Columns.Count; k++)
					{
						if (dataTable4.Columns[k].ColumnName.ToUpper().StartsWith("U_"))
						{
							BMDItem[dataTable4.Columns[k].ColumnName] = dataRow5[dataTable4.Columns[k]];
						}
					}
					dtBP.AcceptChanges();
				}
				if (grd_Properties.DataSource is DataTable dataTable5 && dataTable5.Rows.Count > 0)
				{
					new List<string>();
					for (int l = 0; l < dataTable5.Rows.Count; l++)
					{
						BMDItem["QryGroup" + (l + 1)] = Function.ToString(dataTable5.Rows[l]["Check"]);
					}
					dtBP.AcceptChanges();
				}
				DataTable dt = grd_Contact.DataSource as DataTable;
				dtCntPrsn = new DataTable();
				if (dt == null || dt.Rows.Count == 0)
				{
					if (!WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt))
					{
						return;
					}
				}
				else
				{
					dtCntPrsn = dt.Copy();
					for (int num3 = dtCntPrsn.Rows.Count - 1; num3 >= 0; num3--)
					{
						DataRow dataRow6 = dtCntPrsn.Rows[num3];
						if (Function.ToString(dataRow6["Name"]).ToUpper() == "ADD NEW")
						{
							dtCntPrsn.Rows.Remove(dataRow6);
							dtCntPrsn.AcceptChanges();
						}
					}
				}
				dtBankAcct = new DataTable();
				if (_dtBpBankAccount == null || _dtBpBankAccount.Rows.Count == 0)
				{
					if (!WinFuntions.GetTableStruct(ref dtBankAcct, "OCRB", _lstStructDt))
					{
						return;
					}
				}
				else
				{
					for (int num4 = _dtBpBankAccount.Rows.Count - 1; num4 >= 0; num4--)
					{
						DataRow dataRow7 = _dtBpBankAccount.Rows[num4];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow7["BankCode"])))
						{
							_dtBpBankAccount.Rows.Remove(dataRow7);
							_dtBpBankAccount.AcceptChanges();
						}
					}
					if (_dtBpBankAccount.Rows.Count > 0)
					{
						dtBankAcct = _dtBpBankAccount.Copy();
					}
					else if (!WinFuntions.GetTableStruct(ref dtBankAcct, "OCRB", _lstStructDt))
					{
						return;
					}
				}
				DateTime? dateTime2 = Function.ParseDateTime(txt_BPBankDateOfSignature.EditValue);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if (dateTime2.Value.Year >= 1990)
					{
						goto IL_0c28;
					}
				}
				BMDItem["SignDate"] = new DateTime(1900, 1, 1);
				goto IL_0c28;
				IL_040e:
				DateTime? dateTime3 = Function.ParseDateTime(dateFrozenTo.EditValue);
				if (dateTime3.HasValue)
				{
					_ = dateTime3.Value;
					if (dateTime3.Value.Year >= 1990)
					{
						goto IL_046e;
					}
				}
				BMDItem["frozenTo"] = new DateTime(1900, 1, 1);
				goto IL_046e;
				IL_034f:
				DateTime? dateTime4 = Function.ParseDateTime(dateValidTo.EditValue);
				if (dateTime4.HasValue)
				{
					_ = dateTime4.Value;
					if (dateTime4.Value.Year >= 1990)
					{
						goto IL_03ae;
					}
				}
				BMDItem["validTo"] = new DateTime(1900, 1, 1);
				goto IL_03ae;
				IL_03ae:
				DateTime? dateTime5 = Function.ParseDateTime(dateFrozenFrom.EditValue);
				if (dateTime5.HasValue)
				{
					_ = dateTime5.Value;
					if (dateTime5.Value.Year >= 1990)
					{
						goto IL_040e;
					}
				}
				BMDItem["frozenFrom"] = new DateTime(1900, 1, 1);
				goto IL_040e;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		}
	}

	private async Task LoadDataFromQickCreationBPMasterData(DataTable dt)
	{
		try
		{
			await ShowData(dt);
			if (!isExistsCardCode)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		lb_Code.Utext = Resources.lb_Code;
		lb_Name.Utext = Resources.OCRD_CardName;
		lb_ForeignName.Utext = Resources.OCRD_CardFName;
		lb_Group.Utext = Resources.OCRD_GroupCode;
		lb_Currency.Utext = Resources.OCRD_Currency;
		lb_FederalTaxID.Utext = Resources.OCRD_LicTradNum;
		lb_Tel1.Utext = Resources.OCRD_Phone1;
		lb_Tel2.Utext = Resources.OCRD_Phone2;
		lb_MobiPhone.Utext = Resources.OCRD_Phone1;
		lb_Fax.Utext = Resources.OCRD_Fax;
		lb_Email.Utext = Resources.OCRD_E_Mail;
		lb_Website.Utext = Resources.OCRD_IntrntSite;
		lb_ShippingType.Utext = Resources.OCRD_ShipType;
		lb_Passw.Utext = Resources.OCRD_Password;
		lb_ContactP.Utext = Resources.OCRD_CntctPrsn;
		lb_IDno2.Utext = Resources.OCRD_AddID;
		lb_UnifiedFederalTaxID.Utext = Resources.OCRD_VatIdUnCmp;
		lb_Remark.Utext = Resources.OCRD_Notes;
		lb_SalesEmpLoyee.Utext = Resources.OCRD_SlpCode;
		lb_PaymentTerms.Utext = Resources.OCRD_GroupNum;
		lb_Interest.Utext = Resources.OCRD_IntrstRate;
		lb_PriceLst.Utext = Resources.OCRD_ListNum;
		lb_TotalDis.Utext = Resources.OCRD_Discount;
		lb_CreditL.Utext = Resources.OCRD_CreditLine;
		lb_Commitment.Utext = Resources.OCRD_DebtLine;
		lb_BankCountry.Utext = "Bank Country";
		lb_BankName.Utext = Resources.OCRD_BankName;
		lb_BankCode.Utext = Resources.OCRD_BankCode;
		lb_Account.Utext = Resources.OCRD_DflAccount;
		lb_CreditCardType.Utext = Resources.OCRD_CreditCard;
		lb_CreditCardNo.Utext = Resources.OCRD_CrCardNum;
		lb_ExpirationDate.Utext = Resources.OCRD_CardValid;
		lb_IDNumber.Utext = Resources.OCRD_OwnerIdNum;
		lb_AvarageDelay.Utext = Resources.OCRD_AvrageLate;
		lb_NameCnt.Utext = Resources.OCPR_Name;
		lb_Title.Utext = Resources.OCPR_Title;
		lb_Position.Utext = Resources.OCPR_Position;
		lb_Address.Utext = Resources.OCPR_Address;
		lb_TeleP1.Utext = Resources.OCPR_Tel1;
		lb_TeleP2.Utext = Resources.OCPR_Tel2;
		lb_MobiPhone.Utext = Resources.OCPR_Cellolar;
		lb_FaxCnt.Utext = Resources.OCPR_Fax;
		lb_EmailCnt.Utext = Resources.OCPR_E_MailL;
		lb_Pager.Utext = Resources.OCPR_Pager;
		lb_Remark1.Utext = Resources.OCPR_Note1;
		lb_Remark2.Utext = Resources.OCPR_Note2;
		lb_PassWord.Utext = Resources.OCPR_Password;
		lb_PlaceOfB.Utext = Resources.OCPR_BirthPlace;
		lb_DateOfB.Utext = Resources.OCPR_BirthDate;
		lb_Gender.Utext = Resources.OCPR_Gender;
		lb_Profession.Utext = Resources.OCPR_Profession;
		lb_CityOfB.Utext = Resources.OCPR_BirthCity;
		chk_Active.Text = Resources.OCRD_validFor;
		tab_General.Text = Resources.tab_General;
		tab_PaymentTerms.Text = Resources.tab_PaymentTerms;
		tab_ContactPerson.Text = Resources.tab_ContactPerson;
		tab_Remark.Text = Resources.tab_Remark;
		btCancel.Text = Resources.ButtonCancel;
	}

	private bool CheckDoc()
	{
		try
		{
			string cardCode = txt_CardCode.Text;
			return _httpClient.BusinessMaster.CheckDoc(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task WaitLoad()
	{
		_ = 7;
		try
		{
			_isLoading = true;
			await WinFuntions.LoadUdfColumns(gv_Udf, "OCRD");
			LoadComboAddressType();
			LoadComboBPType();
			LoadComboCountry();
			LoadCBPaymentRunBCACode();
			LoadComboEffectiveDiscount();
			LoadComboTabGeneral();
			LoadComboPaymentTerm();
			LoadBPGroup();
			LoadComboTaxGroup();
			LoadCurrency();
			await LoadPaymentTerm();
			await LoadSaleEmployee();
			await LoadCreditCardType();
			await LoadShippingType();
			await LoadAllState();
			LoadPriceList();
			LoadContactPerson("");
			LoadCardGroup();
			txt_CardCode.Text = _copyF_CodeCustomer;
			string text = txt_CardCode.Text;
			if (!string.IsNullOrEmpty(text))
			{
				await GetADoc(text, _cntCode);
			}
			else
			{
				WinFuntions.GetNewRow_UDF(grd_Udf, "OCRD");
				await REFRESH();
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadAllState(string country = "")
	{
		try
		{
			_dtStates = await _httpClient.EmployeeMasterClient.GetStates();
			if (_listLukStates.Exists((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(country)) && !string.IsNullOrWhiteSpace(country))
			{
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = _listLukStates.Find((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(country));
				EnumerableRowCollection<DataRow> source = from t in _dtStates.AsEnumerable()
					where Function.ToString(t["Country"]) == country
					select t;
				DataTable dataTable = _dtStates.Clone();
				if (source.Any())
				{
					dataTable = source.CopyToDataTable();
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = -1;
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Code"] = -10000;
				dataRow2["Name"] = "-Define New-";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				repositoryItemLookUpEdit.DataSource = dataTable.Copy();
				repositoryItemLookUpEdit.DropDownRows = dataTable.Rows.Count;
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit();
			repositoryItemLookUpEdit2.Assign(_defaultValueEdit);
			DataRow dataRow3 = _dtStates.NewRow();
			dataRow3["Code"] = -1;
			DataRow dataRow4 = _dtStates.NewRow();
			dataRow4["Code"] = -10000;
			dataRow4["Name"] = "-Define New-";
			_dtStates.Rows.InsertAt(dataRow3, 0);
			_dtStates.Rows.Add(dataRow4);
			_dtStates.AcceptChanges();
			repositoryItemLookUpEdit2.DataSource = _dtStates.Copy();
			repositoryItemLookUpEdit2.Name = country;
			repositoryItemLookUpEdit2.DropDownRows = _dtStates.Rows.Count;
			repositoryItemLookUpEdit2.PopulateColumns();
			repositoryItemLookUpEdit2.EditValueChanging += rep_States_EditValueChanging;
			for (int num = 0; num < repositoryItemLookUpEdit2.Columns.Count; num++)
			{
				LookUpColumnInfo lookUpColumnInfo = repositoryItemLookUpEdit2.Columns[num];
				if (lookUpColumnInfo.FieldName != "Name")
				{
					lookUpColumnInfo.Visible = false;
				}
			}
			gc_Address_State.ColumnEdit = repositoryItemLookUpEdit2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboPaymentTerm()
	{
		try
		{
			luk_PaymentTermPriority.Properties.DisplayMember = "PrioDesc";
			luk_PaymentTermPriority.Properties.ValueMember = "PrioCode";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboPaymentTermPriority();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_PaymentTermPriority.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboTabGeneral()
	{
		try
		{
			LoadComboIndustry();
			LoadComboTechnician();
			LoadComboTerritory();
			LoadComboLanguage();
			luk_BusinessType.SetDataSource(new string[3, 2]
			{
				{ "C", "Company" },
				{ "I", "Private" },
				{ "E", "Employee" }
			}, isStringCode: true, "C");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboLanguage()
	{
		try
		{
			luk_Language.Properties.DisplayMember = "Name";
			luk_Language.Properties.ValueMember = "Code";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboLanguage();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_Language.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboTerritory()
	{
		try
		{
			luk_Territory.Properties.DisplayMember = "descript";
			luk_Territory.Properties.ValueMember = "territryID";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboTerritory();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_Territory.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboTechnician()
	{
		try
		{
			luk_Technician.Properties.DisplayMember = "Name";
			luk_Technician.Properties.ValueMember = "empID";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboTechnician();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_Technician.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboIndustry()
	{
		try
		{
			luk_BPIndustry.Properties.DisplayMember = "IndName";
			luk_BPIndustry.Properties.ValueMember = "IndCode";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboIndustry();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.AddRange(new DataColumn[3]
				{
					new DataColumn("IndCode", typeof(int)),
					new DataColumn("IndName", typeof(string)),
					new DataColumn("IndDesc", typeof(string))
				});
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => r["IndCode"] == null || r["IndCode"] == DBNull.Value))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["IndCode"] = DBNull.Value;
				dataRow["IndName"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["IndCode"]) == -10))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["IndCode"] = -10;
				dataRow2["IndName"] = "Define New";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
			}
			luk_BPIndustry.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboEffectiveDiscount()
	{
		try
		{
			luk_EffectiveDiscount.SetDataSource(new string[5, 2]
			{
				{ "L", "Lowest Discount" },
				{ "H", "Highest Discount" },
				{ "A", "Average Disc." },
				{ "S", "Discount Totals" },
				{ "M", "Discount Multiples" }
			}, isStringCode: true, "L");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboCountry()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboCountry();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rep_Address_Country.DataSource = dataTable;
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
				bte_BPBankCountry.Properties.DataSource = dataTable;
				txt_BirthPlace.Properties.DataSource = dataTable;
				luk_HousBnkCry.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void picRemarks_MouseDown(object sender, MouseEventArgs e)
	{
		_picFocus = true;
	}

	private void picRemarks_MouseUp(object sender, MouseEventArgs e)
	{
		_picFocus = false;
	}

	private void xTab_BP_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (e.Page == tab_Remark)
			{
				LoadRemarkTab();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboAddressType()
	{
		try
		{
			rep_Address_AdresType.SetDataSource(new string[2, 2]
			{
				{ "B", "Pay To" },
				{ "S", "Ship To" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboTaxGroup()
	{
		try
		{
			luk_VatStatus.SetDataSource(new string[2, 2]
			{
				{ "Y", "Liable" },
				{ "N", "Exempted" }
			}, isStringCode: true);
			luk_VatStatus.EditValueChanged += Luk_VatStatus_EditValueChanged;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Luk_VatStatus_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(Function.ToString(luk_VatStatus.EditValue)))
			{
				bte_ECVatGroup.EditValue = null;
				txt_ECVatGroupName.EditValue = null;
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataRow GetChoosenTaxGroup(string vatGroup, string cardType)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, lstpr: new List<Common.ParameterValue>
		{
			new Common.ParameterValue("@cardType", cardType),
			new Common.ParameterValue("@vatGroup", vatGroup)
		}, multiSelect: false, txtSeach: Function.ToString(bte_ECVatGroup.EditValue), storeProcedure: "Apz_Cfl_LoadTaxGroup", itemObjectType: Enums.ItemObjectType.NoItemType, itemId: "", prms: Array.Empty<string>());
		frm_CFL2.ShowDialog(this);
		return frm_CFL2.ChooseItem;
	}

	private void bte_ECVatGroup_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				DataRow choosenTaxGroup = GetChoosenTaxGroup(Function.ToString(luk_VatStatus.EditValue), Function.ToString(luk_CardType.EditValue));
				if (choosenTaxGroup != null && choosenTaxGroup.IsExistsColumn("Code"))
				{
					bte_ECVatGroup.EditValue = Function.ToString(choosenTaxGroup["Code"]);
				}
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_ECVatGroup_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(Function.ToString(bte_ECVatGroup.EditValue)))
			{
				List<QueryParamater> list = new List<QueryParamater>();
				list.Add(new QueryParamater
				{
					ParaName = "@code",
					Value = Function.ToString(bte_ECVatGroup.EditValue),
					DataType = "string"
				});
				DataTable executeStoreResult = _httpClient.Commons.GetExecuteStoreResult("Apz_TaxGroup_LoadById", list);
				if (executeStoreResult.IsNotNull())
				{
					txt_ECVatGroupName.EditValue = Function.ToString(executeStoreResult.Rows[0]["Name"]);
				}
			}
			else
			{
				txt_ECVatGroupName.EditValue = string.Empty;
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBPType()
	{
		try
		{
			luk_CardType.SetDataSource(new string[3, 2]
			{
				{ "C", "Customer" },
				{ "S", "Vendor" },
				{ "L", "Lead" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboCode()
	{
		try
		{
			_ = txt_CardCode.Text;
			string text = Function.ToString(luk_CardType.EditValue);
			if (text == "L")
			{
				text = "C";
			}
			string o = "2";
			int defaultBranch = GlobalData.DefaultBranch;
			DataTable dataTable = _httpClient.Commons.GetListSeries(o, defaultBranch, text);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("Series", typeof(short));
				dataTable.Columns.Add("SeriesName", typeof(string));
				dataTable.AcceptChanges();
			}
			luk_Series.Properties.DataSource = dataTable;
			if (dataTable.Rows.Count == 1)
			{
				luk_Series.EditValue = Function.ParseShort(dataTable.Rows[0]["Series"]);
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["SeriesName"]).ToUpper() == "MANUAL");
			luk_Series.EditValue = ((dataRow == null) ? new short?(Convert.ToInt16(0)) : Function.ParseShort(dataRow["Series"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCardGroup()
	{
		bool isSettingCardGroup = _isSettingCardGroup;
		try
		{
			_isSettingCardGroup = true;
			DataTable dataTable = new DataTable();
			if (FormMode == Enums.FormViewModeType.Find)
			{
				dataTable = _httpClient.BusinessMaster.ShowCardGroup("A");
			}
			else
			{
				string text = Function.ToString(luk_CardType.EditValue);
				if (text == "L")
				{
					text = "C";
				}
				dataTable = _httpClient.BusinessMaster.ShowCardGroup(text);
			}
			DataTable dataTable2 = new DataTable();
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				dataTable2 = dataTable.Clone();
				dataTable2.Columns["GroupCode"].DataType = typeof(short);
				dataTable2.AcceptChanges();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable2.NewRow();
					for (int j = 0; j < dataTable.Columns.Count; j++)
					{
						if (dataTable.Columns[j].ColumnName == "GroupCode")
						{
							dataRow[dataTable.Columns[j].ColumnName] = Function.ParseShort(dataTable.Rows[i][j]);
						}
						else
						{
							dataRow[dataTable.Columns[j].ColumnName] = dataTable.Rows[i][j];
						}
					}
					dataTable2.Rows.Add(dataRow);
				}
				dataTable2.AcceptChanges();
			}
			luk_CardGroup.Properties.DataSource = dataTable2;
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (FormMode == Enums.FormViewModeType.Find)
				{
					DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseShort(r["GroupCode"]) == -1);
					if (dataRow2 != null)
					{
						dataTable2.Rows.Remove(dataRow2);
					}
					dataTable2.AcceptChanges();
				}
				else if (!_isSettingFormDataSource)
				{
					luk_CardGroup.EditValue = Function.ParseShort(dataTable2.Rows[0]["GroupCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSettingCardGroup = isSettingCardGroup;
	}

	private void LoadCurrency()
	{
		try
		{
			_listCurrencyCode = GlobalData.Currencies;
			if (!_listCurrencyCode.Exists((Common.Table_Code x) => x.Code == "##"))
			{
				_listCurrencyCode.Add(new Common.Table_Code
				{
					Code = "##",
					Name = "All Currencies"
				});
			}
			if (!_listCurrencyCode.Exists((Common.Table_Code x) => x.Code == "-1"))
			{
				_listCurrencyCode.Add(new Common.Table_Code
				{
					Code = "-1",
					Name = "Define New"
				});
			}
			luk_Currency.Properties.DataSource = _listCurrencyCode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPaymentTerm()
	{
		try
		{
			DataTable dataSource = _httpClient.BusinessMaster.ShowPaymentTerm();
			_isAskPaymentTerm = false;
			luk_PaymentTerm.Properties.DataSource = dataSource;
			_isAskPaymentTerm = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadSaleEmployee()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			DataTable dataTable = await hp.GetAsync("ShowSaleEmployee");
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.AddRange(new DataColumn[5]
				{
					new DataColumn("SlpCode", typeof(int)),
					new DataColumn("SlpName", typeof(string)),
					new DataColumn("Memo", typeof(string)),
					new DataColumn("Commission", typeof(double)),
					new DataColumn("UAddress", typeof(string))
				});
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["SlpCode"]) == -10))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["SlpCode"] = -10;
				dataRow["SlpName"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["SlpCode"]) == -1))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["SlpCode"] = -1;
				dataRow2["SlpName"] = "";
				dataTable.Rows.InsertAt(dataRow2, 0);
				dataTable.AcceptChanges();
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_SalesBuyer.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCreditCardType()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowCreditCardType();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				_listCreditCardType = new List<Common.Table_Code>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Common.Table_Code table_Code = new Common.Table_Code();
					table_Code.Code = dataTable.Rows[i]["CreditCard"].ToString();
					table_Code.Name = dataTable.Rows[i]["CardName"].ToString();
					_listCreditCardType.Add(table_Code);
				}
				luk_CreditCardType.Properties.DataSource = _listCreditCardType;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadShippingType()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowShippingTypes();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.AddRange(new DataColumn[5]
				{
					new DataColumn("TrnspCode", typeof(short)),
					new DataColumn("TrnspName", typeof(string)),
					new DataColumn("WebSite", typeof(string)),
					new DataColumn("UserSign", typeof(short)),
					new DataColumn("DataSource", typeof(string))
				});
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["TrnspCode"]) == -10))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["TrnspCode"] = Convert.ToInt16(-10);
				dataRow["TrnspName"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["TrnspCode"]) == -1))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["TrnspCode"] = Convert.ToInt16(-1);
				dataRow2["TrnspName"] = "";
				dataTable.Rows.InsertAt(dataRow2, 0);
				dataTable.AcceptChanges();
			}
			luk_ShippingType.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadPriceList()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowPriceList();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["PriceListNo"] = -2;
				dataRow["PriceListName"] = "Last Evaluated Price";
				dataTable.Rows.InsertAt(dataRow, 0);
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["PriceListNo"] = -1;
				dataRow2["PriceListName"] = "Last Purchase Price";
				dataTable.Rows.InsertAt(dataRow2, 1);
				dataTable.AcceptChanges();
				luk_PaymentPriceList.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ShowData(DataTable dt)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.View;
		try
		{
			pnDisplayBalance.Visible = true;
			_dtOriginal = dt.Copy();
			_isAskPaymentTerm = false;
			_defaultContact = Function.ToString(dt.Rows[0]["CntctPrsn"]);
			_defaultBillAddress = Function.ToString(dt.Rows[0]["BillToDef"]);
			_defaultShipAddress = Function.ToString(dt.Rows[0]["ShipToDef"]);
			string cardCode = Function.ToString(dt.Rows[0]["CardCode"]);
			_isLoading = true;
			this.ResetData();
			LoadCardGroup();
			LoadComboCode();
			LoadContactPerson(cardCode);
			_isLoading = true;
			calc_Balance.ReadOnly = true;
			calc_DNotesBal.ReadOnly = true;
			calc_OrdersBal.ReadOnly = true;
			calc_BalanceFC.ReadOnly = true;
			calc_DNoteBalFC.ReadOnly = true;
			calc_OrderBalFC.ReadOnly = true;
			calc_BalanceSY.ReadOnly = true;
			calc_DNoteBalSY.ReadOnly = true;
			calc_OrderBalSY.ReadOnly = true;
			calc_OprCount.ReadOnly = true;
			txtGeneralContactPerson.ReadOnly = true;
			_oldCardCode = Function.ToString(cardCode);
			_oldSeries = (short)(Function.ParseShort(dt.Rows[0]["Series"]).HasValue ? Function.ParseShort(dt.Rows[0]["Series"]).Value : 0);
			_oldCardType = Function.ToString(dt.Rows[0]["CardType"]);
			_isAskPaymentTerm = false;
			_isSettingFormDataSource = true;
			DataSource = dt;
			_isSettingFormDataSource = false;
			LoadBPBankAccount(cardCode, Function.ToString(dt.Rows[0]["BankCountr"]), Function.ToString(dt.Rows[0]["BankCode"]), Function.ToString(dt.Rows[0]["DflAccount"]));
			_bankKey = Function.ParseInt(dt.Rows[0]["DflBankKey"]);
			chk_DeferrTax.Checked = Function.ToString(dt.Rows[0]["DeferrTax"]) == "Y";
			chk_Affiliate.Checked = Function.ToString(dt.Rows[0]["Affiliate"]) == "Y";
			chk_BlockDunn.Checked = Function.ToString(dt.Rows[0]["BlockDunn"]) == "Y";
			chk_UseShpdGd.Checked = Function.ToString(dt.Rows[0]["UseShpdGd"]) == "Y";
			if (Function.ToString(dt.Rows[0]["FatherType"]) == "P")
			{
				rdg_FatherType.SelectedIndex = 0;
			}
			else
			{
				rdg_FatherType.SelectedIndex = 1;
			}
			string value = "";
			if (Function.ToString(luk_CardType.EditValue) == "C")
			{
				value = "Down Payment Receivables";
			}
			else if (Function.ToString(luk_CardType.EditValue) == "S")
			{
				value = "Down Payment Payables";
			}
			luk_VatStatus.EditValue = ((Function.ToString(dt.Rows[0]["VatStatus"]) == "") ? "Y" : Function.ToString(dt.Rows[0]["VatStatus"]));
			bte_ECVatGroup.EditValue = Function.ToString(dt.Rows[0]["ECVatGroup"]);
			string fatherCard = Function.ToString(bte_FatherCard.EditValue);
			string debPayAcct = Function.ToString(bte_DebPayAcct.EditValue);
			string dpmClear = Function.ToString(bte_DpmClear.EditValue);
			string dpmIntAct = Function.ToString(bte_DpmIntAct.EditValue);
			string connBP = Function.ToString(bte_ConnBP.EditValue);
			DataTable accountTabName = _httpClient.BusinessMaster.GetAccountTabName(fatherCard, debPayAcct, dpmClear, dpmIntAct, connBP);
			if (accountTabName.IsNotNull())
			{
				lbl_ConSolidBPName.Utext = Function.ToString(accountTabName.Rows[0]["FatherCard"]);
				lbl_AcctPayableName.Utext = Function.ToString(accountTabName.Rows[0]["DebPayAcct"]);
				lbl_ClearAcctName.Utext = Function.ToString(accountTabName.Rows[0]["DpmClear"]);
				lbl_InterimAcctName.Utext = Function.ToString(accountTabName.Rows[0]["DpmIntAct"]);
				lbl_ConnBPName.Utext = Function.ToString(accountTabName.Rows[0]["ConnBP"]);
			}
			DataTable controlAccounts = _httpClient.BusinessMaster.GetControlAccounts(cardCode);
			if (controlAccounts.IsNotNull())
			{
				_dtBpControlAccounts = controlAccounts.Copy();
				for (int i = 0; i < _dtBpControlAccounts.Rows.Count; i++)
				{
					if (Function.ToString(_dtBpControlAccounts.Rows[i]["AcctType"]) == "D")
					{
						_dtBpControlAccounts.Rows[i]["AcctTypeName"] = value;
					}
				}
			}
			else
			{
				_dtBpControlAccounts = GetDefaultControlAccounts();
			}
			calc_Balance.EditValue = Function.ParseDouble(dt.Rows[0]["Balance"]);
			calc_BalanceFC.EditValue = Function.ParseDouble(dt.Rows[0]["BalanceSys"]);
			calc_BalanceSY.EditValue = Function.ParseDouble(dt.Rows[0]["BalanceFC"]);
			calc_DNotesBal.EditValue = Function.ParseDouble(dt.Rows[0]["DNotesBal"]);
			calc_DNoteBalFC.EditValue = Function.ParseDouble(dt.Rows[0]["DNoteBalSy"]);
			calc_DNoteBalSY.EditValue = Function.ParseDouble(dt.Rows[0]["DNoteBalFC"]);
			calc_OrdersBal.EditValue = Function.ParseDouble(dt.Rows[0]["OrdersBal"]);
			calc_OrderBalSY.EditValue = Function.ParseDouble(dt.Rows[0]["OrderBalSy"]);
			calc_OrderBalFC.EditValue = Function.ParseDouble(dt.Rows[0]["OrderBalFC"]);
			calc_OprCount.EditValue = Function.ParseDouble(dt.Rows[0]["OprCount"]);
			if (!string.IsNullOrWhiteSpace(Function.ToString(dt.Rows[0]["Indicator"])))
			{
				txt_FactoringIndicator.Text = GetFactoryIndicatorName(Function.ToString(dt.Rows[0]["Indicator"]));
			}
			LoadPaymentDates(cardCode);
			string text = Function.ToString(dt.Rows[0]["validFor"]);
			string text2 = Function.ToString(dt.Rows[0]["frozenFor"]);
			DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["validTo"]);
			DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[0]["validFrom"]);
			DateTime? dateTime3 = Function.ParseDateTime(dt.Rows[0]["frozenTo"]);
			DateTime? dateTime4 = Function.ParseDateTime(dt.Rows[0]["frozenFrom"]);
			if (text == "Y" && text2 == "Y")
			{
				rdg_Active.SelectedIndex = 2;
			}
			else if ((text == "Y" && text2 == "N") || string.IsNullOrEmpty(text2))
			{
				rdg_Active.SelectedIndex = 0;
			}
			else if ((text2 == "Y" && text == "N") || string.IsNullOrEmpty(text))
			{
				rdg_Active.SelectedIndex = 1;
			}
			else
			{
				rdg_Active.SelectedIndex = 0;
			}
			dateValidTo2.EditValue = dateTime;
			dateValidFrom2.EditValue = dateTime2;
			txtValidComm2.Text = Function.ToString(dt.Rows[0]["ValidComm"]);
			dateFrozenTo2.EditValue = dateTime3;
			dateFrozenFrom2.EditValue = dateTime4;
			txtFrozenComm2.Text = Function.ToString(dt.Rows[0]["FrozenComm"]);
			if (dt.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
			{
				grd_Udf.DataSource = dt;
				grd_Udf.RefreshDataSource();
			}
			int absEntry = Function.ParseInt(dt.Rows[0]["AtcEntry"]);
			Task t0 = ucAttachment1.LoadAttachments(absEntry);
			_curPictureName = Function.ToString(dt.Rows[0]["Picture"]);
			_cloudImage = Function.ToString(dt.Rows[0]["CloudImage"]);
			if (xTab_BP.SelectedTabPage == tab_Remark)
			{
				LoadRemarkTab();
			}
			luk_Series.ReadOnly = false;
			List<Tuple<string, string>> bPProperties = GetBPProperties(cardCode);
			LoadBPProperties(bPProperties);
			LoadAddress(cardCode);
			ChangeAddressType(Function.ToString(dt.Rows[0]["CardType"]));
			DataTable dataTable = await _httpClient.BusinessMaster.ExistsLinkedAll(cardCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.AsEnumerable().FirstOrDefault((DataRow dataRow) => Function.ParseInt(dataRow["ObjType"]) == 13 || Function.ParseInt(dataRow["ObjType"]) == 15 || Function.ParseInt(dataRow["ObjType"]) == 20 || Function.ParseInt(dataRow["ObjType"]) == 18) != null)
				{
					List<Common.Table_Code> list = _listCurrencyCode.ToList();
					Common.Table_Code table_Code = list.FirstOrDefault((Common.Table_Code table_Code3) => table_Code3.Code == "##");
					string bpCurr = Function.ToString(dt.Rows[0]["Currency"]);
					if (bpCurr == "##")
					{
						list.Clear();
						if (table_Code != null)
						{
							list.Add(table_Code);
						}
					}
					else
					{
						Common.Table_Code table_Code2 = list.FirstOrDefault((Common.Table_Code table_Code3) => string.Compare(table_Code3.Code, bpCurr, ignoreCase: true) == 0);
						list.Clear();
						if (table_Code2 != null)
						{
							list.Add(table_Code2);
						}
						if (table_Code != null)
						{
							list.Add(table_Code);
						}
					}
					luk_Currency.Properties.DataSource = list;
					luk_Series.ReadOnly = true;
					luk_Series.BackColor = Color.FromArgb(231, 231, 231);
					txt_CardCode.ReadOnly = true;
					txt_CardCode.BackColor = Color.FromArgb(231, 231, 231);
				}
			}
			else
			{
				luk_Currency.Properties.DataSource = _listCurrencyCode;
				luk_Series.ReadOnly = false;
				txt_CardCode.BackColor = Color.White;
				txt_CardCode.ReadOnly = false;
				txt_CardCode.BackColor = Color.White;
			}
			if (string.Compare(Function.ToString(dt.Rows[0]["Currency"]), GlobalData.MainCurrency, ignoreCase: true) != 0 && Function.ToString(dt.Rows[0]["Currency"]) != "##")
			{
				_isForeignCurrency = true;
			}
			cb_CurrencyType.Visible = true;
			cb_CurrencyType.Properties.Items.Clear();
			if (_isForeignCurrency)
			{
				ComboBoxItemCollection items = cb_CurrencyType.Properties.Items;
				object[] arrCurrencyType = _arrCurrencyType;
				items.AddRange(arrCurrencyType);
				cb_CurrencyType.SelectedIndex = 2;
			}
			else
			{
				cb_CurrencyType.Properties.Items.Add(_arrCurrencyType[0]);
				cb_CurrencyType.Properties.Items.Add(_arrCurrencyType[1]);
				cb_CurrencyType.SelectedIndex = 0;
			}
			if (string.Compare(_oldCardType, "S", ignoreCase: true) == 0)
			{
				chk_AllowPartialDlvrOfSO.Visible = false;
				chk_AllowPartialDlvrPerRow.Visible = false;
			}
			else
			{
				chk_AllowPartialDlvrOfSO.Visible = true;
				chk_AllowPartialDlvrPerRow.Visible = true;
			}
			if (GlobalData.IsMultiBranch)
			{
				btn_BranchAssignment.Enabled = true;
				_dtBranchAssignment = await _httpClient.BusinessMaster.GetBpBranchAssignment(cardCode);
			}
			gv_Address.RefreshData();
			await t0;
			_isAskPaymentTerm = true;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isLoading = false;
			WinCommons._CloseProcessing();
		}
	}

	private void LoadRemarkTab()
	{
		string text = CopyFileShareHelper.GetUpLoadPath("jpg") + _curPictureName;
		_picFilePath = "";
		btn_PicDelete.Visible = false;
		if (!string.IsNullOrEmpty(_cloudImage))
		{
			picRemarks.LoadAsync(_cloudImage);
			return;
		}
		if (!string.IsNullOrWhiteSpace(_curPictureName) && File.Exists(text))
		{
			try
			{
				picRemarks.Image = null;
				string tempPath = Path.GetTempPath();
				tempPath = (_curPictureName.Contains("APZ-BP-") ? (tempPath + _curPictureName) : (tempPath + "APZ-BP-" + _curPictureName));
				string text2 = "";
				if (File.Exists(tempPath))
				{
					text2 = _ftpHelper.DeleteFileSharing(CopyFileSharingCfg.Domain, tempPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password);
				}
				if (text2 != "")
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Cannot delete file [" + tempPath + "] for copy BP picture from server");
				}
				else
				{
					_ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, text, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, tempPath, copyForOpen: true);
					if (File.Exists(tempPath))
					{
						using (FileStream stream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
						{
							picRemarks.Image = Image.FromStream(stream);
							btn_PicDelete.Visible = true;
						}
						_picFilePath = tempPath;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.CannotCopyPictureFromServer2TempFolder4Preview, "");
					}
				}
				return;
			}
			catch
			{
				picRemarks.Image = null;
				return;
			}
		}
		picRemarks.Image = null;
	}

	private void ChangeAddressType(string cardType)
	{
		try
		{
			if (rep_Address_AdresType.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				if (cardType == "C" || cardType == "L")
				{
					dataTable.Rows[0][1] = "Bill To";
				}
				else
				{
					dataTable.Rows[0][1] = "Pay To";
				}
				dataTable.AcceptChanges();
				rep_Address_AdresType.DataSource = dataTable;
				gv_Address.RefreshData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadPaymentDates(string cardCode)
	{
		try
		{
			_dtPaymentDates = _httpClient.BusinessMaster.LoadPaymentDates(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string GetFactoryIndicatorName(string code)
	{
		string result = "";
		try
		{
			DataTable factoryIndicatorName = _httpClient.BusinessMaster.GetFactoryIndicatorName(code);
			if (factoryIndicatorName != null && factoryIndicatorName.Rows.Count > 0)
			{
				result = Function.ToString(factoryIndicatorName.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void LoadBPBankAccount(string cardCode, string country, string bankCode, string account)
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.LoadBPBankAccount(cardCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				_dtBpBankAccount = dataTable;
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Country"]).ToUpper() == country.ToUpper() && Function.ToString(t["BankCode"]).ToUpper() == bankCode.ToUpper() && Function.ToString(t["Account"]).ToUpper() == account.ToUpper());
				if (dataRow != null)
				{
					luk_BPBankName.EditValue = Function.ToString(dataRow["BankCode"]);
					txt_BPBankAccountName.EditValue = Function.ToString(dataRow["AcctName"]);
				}
			}
			else
			{
				WinFuntions.GetTableStruct(ref _dtBpBankAccount, "OCRB", _lstStructDt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBankByCountry()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowBankAccount();
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ToString(t["CountryCod"]) == Function.ToString(bte_BPBankCountry.EditValue)
				select t;
			if (source.Any())
			{
				luk_BPBankName.Properties.DataSource = source.CopyToDataTable();
			}
			else
			{
				luk_BPBankName.Properties.DataSource = dataTable.Clone();
			}
			luk_BPBankName.Properties.ForceInitialize();
			luk_BPBankName.Properties.PopulateColumns();
			for (int num = 0; num < luk_BPBankName.Properties.Columns.Count; num++)
			{
				LookUpColumnInfo lookUpColumnInfo = luk_BPBankName.Properties.Columns[num];
				if (lookUpColumnInfo.FieldName == "BankCode" || lookUpColumnInfo.FieldName == "BankName")
				{
					lookUpColumnInfo.Visible = true;
				}
				else
				{
					lookUpColumnInfo.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadContactPerson(string code)
	{
		try
		{
			DataTable dt = _httpClient.BusinessMaster.ShowContactPerson(code);
			if ((dt == null || dt.Columns.Count == 0) && !WinFuntions.GetTableStruct(ref dt, "OCPR", _lstStructDt))
			{
				return;
			}
			DataRow dataRow = dt.NewRow();
			dataRow["CntctCode"] = -1;
			dataRow["Name"] = "Add new";
			dt.Rows.Add(dataRow);
			dt.AcceptChanges();
			if (grd_Contact.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grd_Contact.DataSource = dt;
					grd_Contact.RefreshDataSource();
				}));
			}
			else
			{
				grd_Contact.DataSource = dt;
				grd_Contact.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBPGroup()
	{
		try
		{
			_dtBpGroup = _httpClient.BusinessMaster.GetBPGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadAddress(string p)
	{
		try
		{
			_listLukStates.Clear();
			DataTable addressByCardCode = _httpClient.BusinessMaster.GetAddressByCardCode(p);
			grd_Address.DataSource = addressByCardCode;
			grd_Address.RefreshDataSource();
			foreach (DataRow row in addressByCardCode.Rows)
			{
				string country = Function.ToString(row["Country"]);
				if (!_listLukStates.Exists((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(country)) && !string.IsNullOrWhiteSpace(country))
				{
					AddNewLukState(country);
				}
			}
			for (int num = 0; num < gv_Address.DataRowCount; num++)
			{
				if (string.Compare(Function.ToString(gv_Address.GetRowCellValue(num, "Address")).Trim(), _defaultShipAddress, ignoreCase: true) == 0 && string.Compare(Function.ToString(gv_Address.GetRowCellValue(num, "AdresType")).Trim(), "S", ignoreCase: true) == 0)
				{
					_defaultShipAddrHandle = num;
				}
			}
			for (int num2 = 0; num2 < gv_Address.DataRowCount; num2++)
			{
				if (string.Compare(Function.ToString(gv_Address.GetRowCellValue(num2, "Address")).Trim(), _defaultBillAddress, ignoreCase: true) == 0 && string.Compare(Function.ToString(gv_Address.GetRowCellValue(num2, "AdresType")).Trim(), "B", ignoreCase: true) == 0)
				{
					_defaultBillAddrHandle = num2;
				}
			}
			addRowToolStripMenuItem_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GvAddressOnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "State"))
			{
				return;
			}
			string country = Function.ToString(gv_Address.GetRowCellValue(e.RowHandle, "Country"));
			if (!string.IsNullOrWhiteSpace(country))
			{
				if (!_listLukStates.Exists((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(country)))
				{
					AddNewLukState(country);
				}
				e.RepositoryItem = _listLukStates.Find((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(country));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewLukState(string country)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(country))
			{
				return;
			}
			EnumerableRowCollection<DataRow> source = from t in _dtStates.AsEnumerable()
				where Function.ToString(t["Country"]) == country
				select t;
			DataTable dataTable = _dtStates.Clone();
			if (source.Any())
			{
				dataTable = source.CopyToDataTable();
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			repositoryItemLookUpEdit.Assign(_defaultValueEdit);
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = -1;
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = -10000;
			dataRow2["Name"] = "-Define New-";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			repositoryItemLookUpEdit.DataSource = dataTable.Copy();
			repositoryItemLookUpEdit.Name = country;
			repositoryItemLookUpEdit.DropDownRows = dataTable.Rows.Count;
			repositoryItemLookUpEdit.PopulateColumns();
			repositoryItemLookUpEdit.EditValueChanging += rep_States_EditValueChanging;
			for (int num = 0; num < repositoryItemLookUpEdit.Columns.Count; num++)
			{
				LookUpColumnInfo lookUpColumnInfo = repositoryItemLookUpEdit.Columns[num];
				if (lookUpColumnInfo.FieldName != "Name")
				{
					lookUpColumnInfo.Visible = false;
				}
			}
			_listLukStates.Add(repositoryItemLookUpEdit);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetSearchTableAddress()
	{
		DataTable ret = new DataTable();
		try
		{
			DataTable searchTableAddress = _httpClient.BusinessMaster.GetSearchTableAddress();
			ProcessGetTable(searchTableAddress, ref ret);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private void ProcessGetTable(DataTable source, ref DataTable ret)
	{
		try
		{
			string text = Function.ToString(source.Rows[0][0]);
			if (text.Length <= 0)
			{
				return;
			}
			text = text.Remove(text.Length - 1);
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Split(' ')[0];
				string text3 = array[i].Split(' ')[1];
				if (!ret.Columns.Contains(text2))
				{
					DataColumn dataColumn = new DataColumn
					{
						ColumnName = text2,
						Caption = text2
					};
					switch (text3)
					{
					case "nvarchar":
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					case "char":
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					case "money":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "numeric":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "float":
						dataColumn.DataType = typeof(double);
						dataColumn.DefaultValue = Function.ParseDouble(0);
						break;
					case "decimal":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "datetime":
						dataColumn.DataType = typeof(DateTime);
						dataColumn.DefaultValue = Function.ParseDateTime(0);
						break;
					case "date":
						dataColumn.DataType = typeof(DateTime);
						dataColumn.DefaultValue = Function.ParseDateTime(0);
						break;
					case "smallint":
						dataColumn.DataType = typeof(short);
						dataColumn.DefaultValue = Function.ParseShort(0);
						break;
					case "tinyint":
						dataColumn.DataType = typeof(byte);
						dataColumn.DefaultValue = Function.ParseByte(0);
						break;
					case "bigint":
						dataColumn.DataType = typeof(int);
						dataColumn.DefaultValue = Function.ParseLong(0);
						break;
					case "int":
						dataColumn.DataType = typeof(int);
						dataColumn.DefaultValue = Function.ParseInt(0);
						break;
					case "bit":
						dataColumn.DataType = typeof(bool);
						dataColumn.DefaultValue = true;
						break;
					default:
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					}
					ret.Columns.Add(dataColumn);
				}
			}
			ret.AcceptChanges();
			ret.Rows.Clear();
			DataRow row = ret.NewRow();
			ret.Rows.Add(row);
			ret.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetControlsBackColor()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				this.SetFindMode();
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				this.ResetData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<Tuple<string, string>> GetBPProperties(string cardCode)
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		try
		{
			DataSet bPProperties = _httpClient.BusinessMaster.GetBPProperties(cardCode);
			if (bPProperties != null && bPProperties.Tables.Count == 2 && bPProperties.Tables[0] != null && bPProperties.Tables[1] != null && bPProperties.Tables[0].Rows.Count > 0 && bPProperties.Tables[0].Columns.Count == bPProperties.Tables[1].Rows.Count)
			{
				DataTable dataTable = bPProperties.Tables[0];
				DataTable dataTable2 = bPProperties.Tables[1];
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					Tuple<string, string> item = new Tuple<string, string>(Function.ToString(dataTable2.Rows[i][0]), Function.ToString(dataTable.Rows[0][i]));
					list.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	private void LoadBPProperties(List<Tuple<string, string>> lstProperties)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("GroupCode", typeof(int));
			dataTable.Columns.Add("GroupName", typeof(string));
			dataTable.Columns.Add("Check", typeof(string));
			dataTable.AcceptChanges();
			for (int i = 0; i < lstProperties.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["GroupCode"] = i + 1;
				dataRow["GroupName"] = lstProperties[i].Item1;
				dataRow["Check"] = lstProperties[i].Item2;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			grd_Properties.DataSource = dataTable;
			grd_Properties.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeButtonOKText(object sender)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
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

	private void SetFormatControlValue()
	{
		try
		{
			calc_Balance.Properties.Mask.MaskType = MaskType.Numeric;
			calc_Balance.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_Balance.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_DNotesBal.Properties.Mask.MaskType = MaskType.Numeric;
			calc_DNotesBal.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_DNotesBal.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_OrdersBal.Properties.Mask.MaskType = MaskType.Numeric;
			calc_OrdersBal.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_OrdersBal.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_OprCount.Properties.Mask.MaskType = MaskType.Numeric;
			calc_OprCount.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_OprCount.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_BalanceFC.Properties.Mask.MaskType = MaskType.Numeric;
			calc_BalanceFC.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_BalanceFC.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_DNoteBalFC.Properties.Mask.MaskType = MaskType.Numeric;
			calc_DNoteBalFC.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_DNoteBalFC.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_OrderBalFC.Properties.Mask.MaskType = MaskType.Numeric;
			calc_OrderBalFC.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_OrderBalFC.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_BalanceSY.Properties.Mask.MaskType = MaskType.Numeric;
			calc_BalanceSY.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_BalanceSY.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_DNoteBalSY.Properties.Mask.MaskType = MaskType.Numeric;
			calc_DNoteBalSY.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_DNoteBalSY.Properties.Mask.UseMaskAsDisplayFormat = true;
			calc_OrderBalSY.Properties.Mask.MaskType = MaskType.Numeric;
			calc_OrderBalSY.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			calc_OrderBalSY.Properties.Mask.UseMaskAsDisplayFormat = true;
			string dateTimeFormat = GlobalData.DateTimeFormat;
			txt_BPBankDateOfSignature.Properties.Mask.MaskType = MaskType.DateTime;
			txt_BPBankDateOfSignature.Properties.Mask.EditMask = dateTimeFormat;
			txt_BPBankDateOfSignature.Properties.Mask.UseMaskAsDisplayFormat = true;
			date_BirthDate.Properties.Mask.MaskType = MaskType.DateTime;
			date_BirthDate.Properties.Mask.EditMask = dateTimeFormat;
			date_BirthDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateValidFrom.Properties.Mask.MaskType = MaskType.DateTime;
			dateValidFrom.Properties.Mask.EditMask = dateTimeFormat;
			dateValidFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateValidTo.Properties.Mask.MaskType = MaskType.DateTime;
			dateValidTo.Properties.Mask.EditMask = dateTimeFormat;
			dateValidTo.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateFrozenFrom.Properties.Mask.MaskType = MaskType.DateTime;
			dateFrozenFrom.Properties.Mask.EditMask = dateTimeFormat;
			dateFrozenFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateFrozenTo.Properties.Mask.MaskType = MaskType.DateTime;
			dateFrozenTo.Properties.Mask.EditMask = dateTimeFormat;
			dateFrozenTo.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateValidFrom2.Properties.Mask.MaskType = MaskType.DateTime;
			dateValidFrom2.Properties.Mask.EditMask = dateTimeFormat;
			dateValidFrom2.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateValidTo2.Properties.Mask.MaskType = MaskType.DateTime;
			dateValidTo2.Properties.Mask.EditMask = dateTimeFormat;
			dateValidTo2.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateFrozenFrom2.Properties.Mask.MaskType = MaskType.DateTime;
			dateFrozenFrom2.Properties.Mask.EditMask = dateTimeFormat;
			dateFrozenFrom2.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateFrozenTo2.Properties.Mask.MaskType = MaskType.DateTime;
			dateFrozenTo2.Properties.Mask.EditMask = dateTimeFormat;
			dateFrozenTo2.Properties.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SetInitialControls()
	{
		_ = 1;
		try
		{
			foreach (IControlEx item in LstControlSource)
			{
				item.Tag = item.FieldName;
			}
			ucAttachment1.ButtonBrowse_Click += ucAttachment1_ButtonBrowse_Click;
			ucAttachment1.ButtonDelete_Click += ucAttachment1_ButtonDelete_Click;
			bte_FactoringIndicator.ErrorText = "";
			if (!GlobalData.LoginSap)
			{
				tab_Accounting.PageVisible = false;
			}
			WinFuntions.GetTableStruct(ref _dtBpCtlAccountsStruct, "CRD3", _lstStructDt);
			if (_dtBpCtlAccountsStruct != null)
			{
				_dtBpCtlAccountsStruct.Columns.Add("AcctName", typeof(string));
				_dtBpCtlAccountsStruct.Columns.Add("AcctTypeName", typeof(string));
				_dtBpControlAccounts = _dtBpCtlAccountsStruct.Clone();
			}
			lbl_ConSolidBPName.Utext = "";
			lbl_AcctPayableName.Utext = "";
			lbl_ClearAcctName.Utext = "";
			lbl_InterimAcctName.Utext = "";
			lbl_ConnBPName.Utext = "";
			uc_BCFLImg_ShippingType.CFLColumnName = "TrnspCode";
			uc_BCFLImg_ShippingType.CFLType = Enums.TypeCFL.DeliveryTypes;
			uc_BCFLImg_ShippingType.CFLBaseEditor = luk_ShippingType;
			uc_BCFLImg_SalesBuyer.CFLColumnName = "SlpCode";
			uc_BCFLImg_SalesBuyer.CFLBaseEditor = luk_SalesBuyer;
			uc_BCFLImg_SalesBuyer.CFLType = Enums.TypeCFL.SaleEmployee;
			uc_BCFLImg_Language.CFLColumnName = "Code";
			uc_BCFLImg_Language.CFLBaseEditor = luk_Language;
			uc_BCFLImg_Language.CFLType = Enums.TypeCFL.Languages;
			bteCFL_OwnerCode.Properties.RefreshDataSource();
			List<UserFieldsImpl> list = await _httpClient.Udf.GetListField("OCPR");
			if (list == null || list.Count <= 0)
			{
				return;
			}
			int i = 1;
			foreach (UserFieldsImpl field in list)
			{
				BaseEdit baseEdit = await WinFuntions.GetUdfEditor(field);
				baseEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				baseEdit.Location = new Point(687, txt_BirthCity.Location.Y + 25 * i);
				baseEdit.Name = "U_" + field.Name;
				baseEdit.Properties.Appearance.Font = new Font("Segoe UI", 7f);
				baseEdit.Properties.Appearance.Options.UseFont = true;
				baseEdit.Properties.AutoHeight = false;
				baseEdit.Size = new Size(170, 18);
				baseEdit.EditValueChanged += Controls_EditValueChanged;
				baseEdit.Leave += txtContactPersonName_Leave;
				tab_ContactPerson.Controls.Add(baseEdit);
				LabelLine labelLine = new LabelLine();
				labelLine.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				labelLine.Appearance.BackColor = Color.FromArgb(235, 236, 239);
				labelLine.Appearance.Options.UseBackColor = true;
				labelLine.HideWhenEmpty = false;
				labelLine.LineColor = Color.FromArgb(213, 218, 223);
				labelLine.Location = new Point(587, lb_CityOfB.Location.Y + 25 * i);
				labelLine.ShowUnderLine = true;
				labelLine.Size = new Size(100, 20);
				labelLine.SourceType = Enums.DataSourceType.TittleValue;
				labelLine.TabIndex = 60;
				labelLine.TabStop = false;
				labelLine.Utext = field.Description;
				tab_ContactPerson.Controls.Add(labelLine);
				_dicContactUdfControl["U_" + field.Name] = baseEdit;
				i++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BindBpBankAccount(DataRow r)
	{
		luk_BPBankName.EditValue = Function.ToString(r["BankCode"]);
		txt_BPBankAccountName.EditValue = Function.ToString(r["AcctName"]);
		bte_BPBankCountry.EditValue = Function.ToString(r["Country"]);
		txt_BPBankCode.EditValue = Function.ToString(r["BankCode"]);
		txt_BPBankAccount.EditValue = Function.ToString(r["Account"]);
		txt_DflSwift.EditValue = Function.ToString(r["SwiftNum"]);
		txt_BPBankBranch.EditValue = Function.ToString(r["Branch"]);
		txt_BPBankControlKey.EditValue = Function.ToString(r["ControlKey"]);
		txt_BPBankIBAN.EditValue = Function.ToString(r["IBAN"]);
		txt_BPBankMandateID.EditValue = Function.ToString(r["MandateID"]);
		txt_BPBankDateOfSignature.EditValue = Function.ParseDateTime(r["SignDate"]);
	}

	private void UpdateBpBankAccountFromChild(dynamic sender)
	{
		try
		{
			if (!(sender is frm_BPBankAccountSetup frm_BPBankAccountSetup2) || (!frm_BPBankAccountSetup2.IsButtonClick && !frm_BPBankAccountSetup2.IsUpdate))
			{
				return;
			}
			_dtBpBankAccount = frm_BPBankAccountSetup2.DtSource;
			if (_dtBpBankAccount != null && _dtBpBankAccount.Rows.Count > 0)
			{
				switch (_dtBpBankAccount.AsEnumerable().Count((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"]))))
				{
				case 0:
				{
					DataRow r = _dtBpBankAccount.NewRow();
					BindBpBankAccount(r);
					break;
				}
				case 1:
				{
					DataRow dataRow3 = _dtBpBankAccount.AsEnumerable().First((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"])));
					_bankKey = Function.ParseInt(dataRow3["BankKey"]);
					BindBpBankAccount(dataRow3);
					break;
				}
				default:
				{
					if (frm_BPBankAccountSetup2.DflRow > -1 && frm_BPBankAccountSetup2.DflRow < _dtBpBankAccount.Rows.Count)
					{
						DataRow dataRow = _dtBpBankAccount.Rows[frm_BPBankAccountSetup2.DflRow];
						_bankKey = Function.ParseInt(dataRow["BankKey"]);
						BindBpBankAccount(dataRow);
						break;
					}
					DataRow dataRow2 = _dtBpBankAccount.AsEnumerable().First((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["BankCode"])));
					_bankKey = Function.ParseInt(dataRow2["BankKey"]);
					BindBpBankAccount(dataRow2);
					break;
				}
				}
			}
			else if (_dtBpBankAccount != null)
			{
				DataRow r2 = _dtBpBankAccount.NewRow();
				BindBpBankAccount(r2);
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private BPImpls GetValueFromDatasource()
	{
		BPImpls result = new BPImpls();
		try
		{
			if (DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				return (BPImpls)TableToEntity("OCRD", dataTable.Copy());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private object TableToEntity(string table, DataTable dt)
	{
		object result = new object();
		try
		{
			switch (table)
			{
			case "OCRD":
				result = Function.DataRowToEntity<BPImpls>(dt.Rows[0]);
				break;
			case "ATC1":
			{
				List<AttachmentFile_Impl> list3 = new List<AttachmentFile_Impl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int num = 0; num < dt.Rows.Count; num++)
					{
						AttachmentFile_Impl item2 = Function.DataRowToEntity<AttachmentFile_Impl>(dt.Rows[num]);
						list3.Add(item2);
					}
				}
				result = list3;
				break;
			}
			case "OCPR":
			{
				List<ContactPersonImpl> list2 = new List<ContactPersonImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int j = 0; j < dt.Rows.Count; j++)
					{
						ContactPersonImpl contactPersonImpl = Function.DataRowToEntity<ContactPersonImpl>(dt.Rows[j]);
						contactPersonImpl.UserFields = new List<UserFieldsImpl>();
						foreach (DataColumn item4 in from DataColumn t in dt.Columns
							where t.ColumnName.StartsWith("U_")
							select t)
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
							userFieldsImpl.Name = item4.ColumnName;
							userFieldsImpl.Value = dt.Rows[j][item4.ColumnName];
							Type dataType = item4.DataType;
							if (dataType == typeof(DateTime))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Date;
							}
							else if (dataType == typeof(float))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Float;
							}
							else if (dataType == typeof(double) || dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(decimal))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							}
							else
							{
								userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							}
							contactPersonImpl.UserFields.Add(userFieldsImpl);
						}
						list2.Add(contactPersonImpl);
					}
				}
				result = list2;
				break;
			}
			case "CRD1":
			{
				List<AddressesImpl> list4 = new List<AddressesImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					List<string> list5 = (from DataColumn t in dt.Columns
						where t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList();
					for (int num2 = 0; num2 < dt.Rows.Count; num2++)
					{
						AddressesImpl addressesImpl = Function.DataRowToEntity<AddressesImpl>(dt.Rows[num2]);
						addressesImpl.UserFields = new List<UserFieldsImpl>();
						if (list5.Count > 0)
						{
							for (int num3 = 0; num3 < list5.Count; num3++)
							{
								UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
								userFieldsImpl2.Name = list5[num3];
								if (dt.Columns[list5[num3]].DataType == typeof(DateTime))
								{
									userFieldsImpl2.Value = Function.ParseDateTime(dt.Rows[num2][list5[num3]]);
									userFieldsImpl2.Type = BoFieldTypes.db_Date;
								}
								else if (dt.Columns[list5[num3]].DataType == typeof(double) || dt.Columns[list5[num3]].DataType == typeof(decimal))
								{
									userFieldsImpl2.Value = Function.ParseDouble(dt.Rows[num2][list5[num3]]);
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl2.Value = Function.ToString(dt.Rows[num2][list5[num3]]);
									userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
								}
								addressesImpl.UserFields.Add(userFieldsImpl2);
							}
						}
						list4.Add(addressesImpl);
					}
				}
				result = list4;
				break;
			}
			case "OCRB":
			{
				List<BankAccountsImpl> list6 = new List<BankAccountsImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int num4 = 0; num4 < dt.Rows.Count; num4++)
					{
						BankAccountsImpl item3 = Function.DataRowToEntity<BankAccountsImpl>(dt.Rows[num4]);
						list6.Add(item3);
					}
				}
				result = list6;
				break;
			}
			case "CRD5":
			{
				List<PaymentDatesImpl> list = new List<PaymentDatesImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						PaymentDatesImpl item = Function.DataRowToEntity<PaymentDatesImpl>(dt.Rows[i]);
						list.Add(item);
					}
				}
				result = list;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private DataTable GetDefaultControlAccounts()
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (_dtBpCtlAccountsStruct != null)
			{
				if (!_dtBpCtlAccountsStruct.Columns.Contains("AcctName"))
				{
					_dtBpCtlAccountsStruct.Columns.Add("AcctName", typeof(string));
				}
				if (!_dtBpCtlAccountsStruct.Columns.Contains("AcctTypeName"))
				{
					_dtBpCtlAccountsStruct.Columns.Add("AcctTypeName", typeof(string));
				}
				_dtBpCtlAccountsStruct.AcceptChanges();
				dataTable = _dtBpCtlAccountsStruct.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["AcctType"] = "D";
				if (Function.ToString(luk_CardType.EditValue) == "C")
				{
					dataRow["AcctTypeName"] = "Down Payment Receivables";
				}
				else if (Function.ToString(luk_CardType.EditValue) == "S")
				{
					dataRow["AcctTypeName"] = "Down Payment Payables";
				}
				dataRow["AcctCode"] = "";
				dataRow["AcctName"] = "";
				dataRow["LogInstane"] = 0;
				dataRow["ObjType"] = (short)2;
				dataTable.Rows.Add(dataRow);
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["AcctType"] = "D";
				dataRow2["AcctTypeName"] = "Open Debts";
				dataRow2["AcctCode"] = "";
				dataRow2["AcctName"] = "";
				dataRow2["LogInstane"] = 0;
				dataRow2["ObjType"] = (short)2;
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private bool IsManualCode()
	{
		try
		{
			DataRow row = (luk_Series.GetSelectedDataRow() as DataRowView).Row;
			return row["IsManual"].Equals("Y") && row["SeriesType"].Equals("B");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private void CFLTabAccounting(object sender, Enums.TypeCFL cflType, string text, string fieldName, string labelField, LabelLine label)
	{
		try
		{
			if (sender is ButtonEdit buttonEdit)
			{
				frm_CFL frm_CFL2 = new frm_CFL(cflType, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					buttonEdit.EditValue = Function.ToString(frm_CFL2.ChooseItem[fieldName]);
					label.Utext = Function.ToString(frm_CFL2.ChooseItem[labelField]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCBPaymentRunBank()
	{
		try
		{
			string cryCode = Function.ToString(luk_HousBnkCry.EditValue);
			DataTable dataSource = _httpClient.BusinessMaster.LoadCBPaymentRunBank(cryCode);
			luk_HouseBank.Properties.DataSource = dataSource;
			LoadCBPaymentRunAccount();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCBPaymentRunAccount()
	{
		try
		{
			string bankCode = Function.ToString(luk_HouseBank.EditValue);
			string cryCode = Function.ToString(luk_HousBnkCry.EditValue);
			DataTable dataTable = _httpClient.BusinessMaster.LoadCBPaymentRunAccount(bankCode, cryCode);
			luk_HousBnkAct.Properties.DataSource = dataTable;
			luk_HsBnkIBAN.Properties.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCBPaymentRunBCACode()
	{
		try
		{
			DataTable dataSource = _httpClient.BusinessMaster.LoadCBPaymentRunBCACode();
			luk_BCACode.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetPaymentRunBankAccountData(string account, LookUpEdit luk)
	{
		try
		{
			if (luk.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(r["Account"]), account, ignoreCase: true) == 0);
				if (dataRow != null)
				{
					luk.EditValue = account;
					txt_HousBnkBrn.Text = Function.ToString(dataRow["Branch"]);
					txt_HsBnkSwift.Text = Function.ToString(dataRow["SwiftNum"]);
					txt_HousCtlKey.Text = Function.ToString(dataRow["ControlKey"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		if (FormMode == Enums.FormViewModeType.Find)
		{
			Controls.EnableDisableControl(isEnable: true);
			return;
		}
		Controls.EnableDisableControl(isEnable: false);
		btAdd.Enabled = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_BusinessMasterData));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lb_Code = new Apzon.Controls.Controls.LabelLine();
		this.lb_ForeignName = new Apzon.Controls.Controls.LabelLine();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.lb_Group = new Apzon.Controls.Controls.LabelLine();
		this.lb_FederalTaxID = new Apzon.Controls.Controls.LabelLine();
		this.lb_Currency = new Apzon.Controls.Controls.LabelLine();
		this.txt_CardCode = new Apzon.Controls.Editor.TextEditEx();
		this.txtForeignName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_CardName = new Apzon.Controls.Editor.TextEditEx();
		this.txtFederalTaxID = new Apzon.Controls.Editor.TextEditEx();
		this.ctMenu_Contact = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xTab_BP = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_General = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_BranchAssignment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_BranchAssignment = new Apzon.Controls.Controls.LabelLine();
		this.txtGeneralRemark = new Apzon.Controls.Editor.MemoEditEx();
		this.uc_BCFLImg_Language = new Apzon.Forms.UserControls.Buttons.ucButtonCFLImage();
		this.uc_BCFLImg_SalesBuyer = new Apzon.Forms.UserControls.Buttons.ucButtonCFLImage();
		this.uc_BCFLImg_ShippingType = new Apzon.Forms.UserControls.Buttons.ucButtonCFLImage();
		this.luk_Technician = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_BPIndustry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_GeneralAliasName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_BusinessType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.bte_BPProject = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_FactoringIndicator = new Apzon.Controls.Editor.TextEditEx();
		this.luk_ShippingType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtGeneralEmail = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralWebsite = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralFax = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralTel2 = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralMobilePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralTel1 = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Territory = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_GLN = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Language = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.bte_BPChannelCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.luk_SalesBuyer = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtGeneralFederalTaxID = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralContactPerson = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralIDNo2 = new Apzon.Controls.Editor.TextEditEx();
		this.txtGeneralPassword = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Language = new Apzon.Controls.Controls.LabelLine();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.bte_FactoringIndicator = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.rdg_Active = new Apzon.Controls.Editor.RadioGroupEx();
		this.panel_Advanced = new System.Windows.Forms.Panel();
		this.txtFrozenComm2 = new Apzon.Controls.Editor.TextEditEx();
		this.txtValidComm2 = new Apzon.Controls.Editor.TextEditEx();
		this.dateFrozenTo2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateFrozenFrom2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateValidTo2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateValidFrom2 = new Apzon.Controls.Editor.DateEditEx();
		this.lb_ToInacti = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToActi = new Apzon.Controls.Controls.LabelLine();
		this.lb_RIacti = new Apzon.Controls.Controls.LabelLine();
		this.lb_RActi = new Apzon.Controls.Controls.LabelLine();
		this.lb_InactiveRangeFrom = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActiveRangeFrom = new Apzon.Controls.Controls.LabelLine();
		this.panel_Inactive = new System.Windows.Forms.Panel();
		this.txtFrozenComm = new Apzon.Controls.Editor.TextEditEx();
		this.dateFrozenTo = new Apzon.Controls.Editor.DateEditEx();
		this.dateFrozenFrom = new Apzon.Controls.Editor.DateEditEx();
		this.lb_T = new Apzon.Controls.Controls.LabelLine();
		this.lb_R = new Apzon.Controls.Controls.LabelLine();
		this.lb_F = new Apzon.Controls.Controls.LabelLine();
		this.panel_Active = new System.Windows.Forms.Panel();
		this.dateValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.txtValidComm = new Apzon.Controls.Editor.TextEditEx();
		this.dateValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.lbVailFrom = new Apzon.Controls.Controls.LabelLine();
		this.lbRemarks = new Apzon.Controls.Controls.LabelLine();
		this.lbVailTo = new Apzon.Controls.Controls.LabelLine();
		this.lb_SalesEmpLoyee = new Apzon.Controls.Controls.LabelLine();
		this.lb_ShippingType = new Apzon.Controls.Controls.LabelLine();
		this.lb_Remark = new Apzon.Controls.Controls.LabelLine();
		this.lb_UnifiedFederalTaxID = new Apzon.Controls.Controls.LabelLine();
		this.lb_ContactP = new Apzon.Controls.Controls.LabelLine();
		this.lb_IDno2 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Passw = new Apzon.Controls.Controls.LabelLine();
		this.lb_Email = new Apzon.Controls.Controls.LabelLine();
		this.lb_Website = new Apzon.Controls.Controls.LabelLine();
		this.lb_Fax = new Apzon.Controls.Controls.LabelLine();
		this.lb_Tel2 = new Apzon.Controls.Controls.LabelLine();
		this.lb_MobiP = new Apzon.Controls.Controls.LabelLine();
		this.lb_Tel1 = new Apzon.Controls.Controls.LabelLine();
		this.tab_ContactPerson = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_BirthPlace = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btContactPersonDefault = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_BirthDate = new Apzon.Controls.Editor.DateEditEx();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Gender = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lb_CityOfB = new Apzon.Controls.Controls.LabelLine();
		this.txt_Profession = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BirthCity = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Profession = new Apzon.Controls.Controls.LabelLine();
		this.lb_Gender = new Apzon.Controls.Controls.LabelLine();
		this.lb_DateOfB = new Apzon.Controls.Controls.LabelLine();
		this.txt_Password = new Apzon.Controls.Editor.TextEditEx();
		this.lb_PlaceOfB = new Apzon.Controls.Controls.LabelLine();
		this.lb_PassWord = new Apzon.Controls.Controls.LabelLine();
		this.txt_Notes1 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Notes2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Pager = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Remark1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Remark2 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Pager = new Apzon.Controls.Controls.LabelLine();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.txt_E_MailL = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Cellolar = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Tel1 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Tel2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Address = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Title = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Position = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Contact = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Contact = new Apzon.Controls.Grid.GridViewEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.CntctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_EmailCnt = new Apzon.Controls.Controls.LabelLine();
		this.lb_FaxCnt = new Apzon.Controls.Controls.LabelLine();
		this.lb_MobiPhone = new Apzon.Controls.Controls.LabelLine();
		this.lb_TeleP2 = new Apzon.Controls.Controls.LabelLine();
		this.lb_TeleP1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Address = new Apzon.Controls.Controls.LabelLine();
		this.lb_Title = new Apzon.Controls.Controls.LabelLine();
		this.lb_Position = new Apzon.Controls.Controls.LabelLine();
		this.lb_NameCnt = new Apzon.Controls.Controls.LabelLine();
		this.tab_Address = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_AddressDefault = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Address = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Address = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.dupplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Address = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Address_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_AdresType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Address_AdresType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Address_Street = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_StreetNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_Block = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_City = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_ZipCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_County = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_State = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_States = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Address_Country = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Address_Country = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Address_Building = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address_GlblLocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_PaymentTerms = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine29 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_DflSwift = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankAccount = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankAccountName = new Apzon.Controls.Editor.TextEditEx();
		this.bte_BPBankCountry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_BPBankBranch = new Apzon.Controls.Editor.TextEditEx();
		this.luk_BPBankName = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_BPBankMandateID = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankControlKey = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankIBAN = new Apzon.Controls.Editor.TextEditEx();
		this.txt_BPBankDateOfSignature = new Apzon.Controls.Editor.DateEditEx();
		this.lb_BankCountry = new Apzon.Controls.Controls.LabelLine();
		this.lb_DflSwift = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.lb_BankCode = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Account = new Apzon.Controls.Controls.LabelLine();
		this.lb_BankName = new Apzon.Controls.Controls.LabelLine();
		this.luk_EffectiveDiscount = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.btn_PaymentDates = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_PaymentDefaultIBAN = new Apzon.Controls.Editor.TextEditEx();
		this.luk_PaymentTermPriority = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_CreditCardType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.datePaymentExpirationDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtPaymentAvarageDelay = new Apzon.Controls.Editor.TextEditEx();
		this.txtPaymentIDNumber = new Apzon.Controls.Editor.TextEditEx();
		this.txtPaymentCreditCardNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.chk_AcceptsEndorsedChecks = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_EndorsableChecksFrom = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_DoNotApplyDiscountGroup = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowPartialDlvrPerRow = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowPartialDlvrOfSO = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_PaymentPriceList = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_PaymentTerm = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtPaymentTotalDiscount = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPaymentInterestonArrears = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPaymentCreditLimit = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPaymentCommitmentLimit = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_Commitment = new Apzon.Controls.Controls.LabelLine();
		this.lb_CreditL = new Apzon.Controls.Controls.LabelLine();
		this.lb_Interest = new Apzon.Controls.Controls.LabelLine();
		this.lb_TotalDis = new Apzon.Controls.Controls.LabelLine();
		this.lb_PriceLst = new Apzon.Controls.Controls.LabelLine();
		this.lb_PaymentTerms = new Apzon.Controls.Controls.LabelLine();
		this.lb_AvarageDelay = new Apzon.Controls.Controls.LabelLine();
		this.lb_IDNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_ExpirationDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_CreditCardNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_CreditCardType = new Apzon.Controls.Controls.LabelLine();
		this.tab_PaymentRun = new Apzon.Controls.Controls.XtraTabPageEx();
		this.luk_BCACode = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.chk_AutoCalBCG = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine38 = new Apzon.Controls.Controls.LabelLine();
		this.chk_CollecAuth = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_RefDetails = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine37 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txt_HousCtlKey = new Apzon.Controls.Editor.TextEditEx();
		this.txt_HsBnkSwift = new Apzon.Controls.Editor.TextEditEx();
		this.txt_HousBnkBrn = new Apzon.Controls.Editor.TextEditEx();
		this.luk_HsBnkIBAN = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_HousBnkAct = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_HouseBank = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_HousBnkCry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine36 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine35 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine34 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine33 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine32 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine31 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine30 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Accounting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Accounting_general = new Apzon.Controls.Controls.XtraTabPageEx();
		this.lbl_ControlAccts = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ConnBPName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ConSolidBPName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_InterimAcctName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ClearAcctName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_AcctPayableName = new Apzon.Controls.Controls.LabelLine();
		this.chk_BlockDunn = new Apzon.Controls.Editor.CheckEditEx();
		this.dateX_DunnDate = new Apzon.Controls.Editor.DateEditEx();
		this.calc_DunnLevel = new Apzon.Controls.Editor.CalcEditEx();
		this.bte_ConnBP = new Apzon.Controls.Editor.ButtonEditEx();
		this.chk_UseShpdGd = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Affiliate = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_PlngGroup = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ConnBP = new Apzon.Controls.Controls.LabelLine();
		this.bte_DpmIntAct = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.bte_DpmClear = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine23 = new Apzon.Controls.Controls.LabelLine();
		this.bte_DebPayAcct = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_DebPayAcct = new Apzon.Controls.Controls.LabelLine();
		this.btEx_ControlAccts = new Apzon.Controls.Controls.SimpleButtonEx();
		this.rdg_FatherType = new Apzon.Controls.Editor.RadioGroupEx();
		this.bte_FatherCard = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DunnDate = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DunnLevel = new Apzon.Controls.Controls.LabelLine();
		this.tab_Accounting_tax = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_DeferrTax = new Apzon.Controls.Editor.CheckEditEx();
		this.bte_ECVatGroup = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine28 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ECVatGroupName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_VatStatus = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine27 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Properties = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PropertiesClearSelect = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_PropertiesSellectAll = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Properties = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Properties = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PropertiesCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.tab_Remark = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PicDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.memoRemarks = new Apzon.Controls.Editor.MemoEditEx();
		this.btnRemarkBrowserImage = new Apzon.Controls.Controls.SimpleButtonEx();
		this.picRemarks = new DevExpress.XtraEditors.PictureEdit();
		this.tab_AttachmentFiles = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_UserDefineF = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Udf = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Udf = new Apzon.Controls.Grid.GridViewEx();
		this.luk_Currency = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.contextMenuStrip_BP = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeBusinessPartnerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cb_CurrencyType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.luk_Series = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_CardType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_CardGroup = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbOwner = new Apzon.Controls.Controls.LabelLine();
		this.bteCFL_OwnerCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtForeignName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFederalTaxID.Properties).BeginInit();
		this.ctMenu_Contact.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xTab_BP).BeginInit();
		this.xTab_BP.SuspendLayout();
		this.tab_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Technician.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BPIndustry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GeneralAliasName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BusinessType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPProject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FactoringIndicator.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ShippingType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralEmail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralWebsite.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralFax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralTel2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralMobilePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralTel1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Territory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Language.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPChannelCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SalesBuyer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralFederalTaxID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralContactPerson.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralIDNo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FactoringIndicator.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).BeginInit();
		this.panel_Advanced.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties).BeginInit();
		this.panel_Inactive.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom.Properties).BeginInit();
		this.panel_Active.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo.Properties).BeginInit();
		this.tab_ContactPerson.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_BirthPlace.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BirthDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BirthDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Gender.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Profession.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BirthCity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Notes1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Notes2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pager.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_E_MailL.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Cellolar.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Address.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Title.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Position.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contact).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contact).BeginInit();
		this.tab_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Address).BeginInit();
		this.contextMenuStrip_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Address).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address_AdresType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_States).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address_Country).BeginInit();
		this.tab_PaymentTerms.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_DflSwift.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankAccount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankAccountName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPBankCountry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BPBankName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankMandateID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankControlKey.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankIBAN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankDateOfSignature.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankDateOfSignature.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_EffectiveDiscount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentDefaultIBAN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTermPriority.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CreditCardType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePaymentExpirationDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePaymentExpirationDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentAvarageDelay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentIDNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCreditCardNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AcceptsEndorsedChecks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EndorsableChecksFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DoNotApplyDiscountGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPartialDlvrPerRow.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPartialDlvrOfSO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentPriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTerm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentTotalDiscount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentInterestonArrears.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCreditLimit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCommitmentLimit.Properties).BeginInit();
		this.tab_PaymentRun.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_BCACode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AutoCalBCG.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CollecAuth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RefDetails.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_HousCtlKey.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HsBnkSwift.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HousBnkBrn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HsBnkIBAN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HousBnkAct.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HouseBank.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HousBnkCry.Properties).BeginInit();
		this.tab_Accounting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Accounting_general.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_BlockDunn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateX_DunnDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateX_DunnDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DunnLevel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ConnBP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseShpdGd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Affiliate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PlngGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DpmIntAct.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DpmClear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DebPayAcct.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_FatherType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FatherCard.Properties).BeginInit();
		this.tab_Accounting_tax.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_DeferrTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ECVatGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ECVatGroupName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_VatStatus.Properties).BeginInit();
		this.tab_Properties.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PropertiesCheck).BeginInit();
		this.tab_Remark.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.picRemarks.Properties).BeginInit();
		this.tab_AttachmentFiles.SuspendLayout();
		this.tab_UserDefineF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency.Properties).BeginInit();
		this.contextMenuStrip_BP.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cb_CurrencyType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Series.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CardType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CardGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_OwnerCode.Properties).BeginInit();
		base.SuspendLayout();
		this.lb_Code.EditControl = null;
		this.lb_Code.EditValue = "Code";
		this.lb_Code.FieldName = null;
		this.lb_Code.HideWhenEmpty = false;
		this.lb_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Code.Location = new System.Drawing.Point(14, 7);
		this.lb_Code.Name = "lb_Code";
		this.lb_Code.ReadOnly = false;
		this.lb_Code.SettingInVisible = false;
		this.lb_Code.SettingReadonly = false;
		this.lb_Code.ShowUnderLine = true;
		this.lb_Code.Size = new System.Drawing.Size(83, 20);
		this.lb_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Code.TabIndex = 0;
		this.lb_Code.TabStop = false;
		this.lb_Code.Utext = "Code";
		this.lb_ForeignName.EditControl = null;
		this.lb_ForeignName.EditValue = "Foreign Name";
		this.lb_ForeignName.FieldName = null;
		this.lb_ForeignName.HideWhenEmpty = false;
		this.lb_ForeignName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ForeignName.Location = new System.Drawing.Point(14, 45);
		this.lb_ForeignName.Name = "lb_ForeignName";
		this.lb_ForeignName.ReadOnly = false;
		this.lb_ForeignName.SettingInVisible = false;
		this.lb_ForeignName.SettingReadonly = false;
		this.lb_ForeignName.ShowUnderLine = true;
		this.lb_ForeignName.Size = new System.Drawing.Size(150, 20);
		this.lb_ForeignName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ForeignName.TabIndex = 0;
		this.lb_ForeignName.TabStop = false;
		this.lb_ForeignName.Utext = "Foreign Name";
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "Name";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(14, 26);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(150, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 0;
		this.lb_Name.TabStop = false;
		this.lb_Name.Utext = "Name";
		this.lb_Group.EditControl = null;
		this.lb_Group.EditValue = "Group";
		this.lb_Group.FieldName = null;
		this.lb_Group.HideWhenEmpty = false;
		this.lb_Group.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Group.Location = new System.Drawing.Point(14, 66);
		this.lb_Group.Name = "lb_Group";
		this.lb_Group.ReadOnly = false;
		this.lb_Group.SettingInVisible = false;
		this.lb_Group.SettingReadonly = false;
		this.lb_Group.ShowUnderLine = true;
		this.lb_Group.Size = new System.Drawing.Size(150, 20);
		this.lb_Group.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Group.TabIndex = 0;
		this.lb_Group.TabStop = false;
		this.lb_Group.Utext = "Group";
		this.lb_FederalTaxID.EditControl = null;
		this.lb_FederalTaxID.EditValue = "Federal Tax ID";
		this.lb_FederalTaxID.FieldName = null;
		this.lb_FederalTaxID.HideWhenEmpty = false;
		this.lb_FederalTaxID.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FederalTaxID.Location = new System.Drawing.Point(14, 105);
		this.lb_FederalTaxID.Name = "lb_FederalTaxID";
		this.lb_FederalTaxID.ReadOnly = false;
		this.lb_FederalTaxID.SettingInVisible = false;
		this.lb_FederalTaxID.SettingReadonly = false;
		this.lb_FederalTaxID.ShowUnderLine = true;
		this.lb_FederalTaxID.Size = new System.Drawing.Size(153, 20);
		this.lb_FederalTaxID.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FederalTaxID.TabIndex = 0;
		this.lb_FederalTaxID.TabStop = false;
		this.lb_FederalTaxID.Utext = "Federal Tax ID";
		this.lb_Currency.EditControl = null;
		this.lb_Currency.EditValue = "Currency";
		this.lb_Currency.FieldName = null;
		this.lb_Currency.HideWhenEmpty = false;
		this.lb_Currency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Currency.Location = new System.Drawing.Point(14, 85);
		this.lb_Currency.Name = "lb_Currency";
		this.lb_Currency.ReadOnly = false;
		this.lb_Currency.SettingInVisible = false;
		this.lb_Currency.SettingReadonly = false;
		this.lb_Currency.ShowUnderLine = true;
		this.lb_Currency.Size = new System.Drawing.Size(152, 20);
		this.lb_Currency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Currency.TabIndex = 0;
		this.lb_Currency.TabStop = false;
		this.lb_Currency.Utext = "Currency";
		this.txt_CardCode.EditControl = null;
		this.txt_CardCode.FieldName = "CardCode";
		this.txt_CardCode.IsSearchControl = false;
		this.txt_CardCode.Location = new System.Drawing.Point(164, 6);
		this.txt_CardCode.Name = "txt_CardCode";
		this.txt_CardCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_CardCode.Properties.Appearance.Options.UseFont = true;
		this.txt_CardCode.Properties.AutoHeight = false;
		this.txt_CardCode.Properties.MaxLength = 15;
		this.txt_CardCode.SettingInVisible = false;
		this.txt_CardCode.SettingReadonly = false;
		this.txt_CardCode.Size = new System.Drawing.Size(117, 18);
		this.txt_CardCode.TabIndex = 2;
		this.txt_CardCode.EditValueChanged += new System.EventHandler(this.txtCode_EditValueChanged);
		this.txtForeignName.EditControl = null;
		this.txtForeignName.FieldName = "CardFName";
		this.txtForeignName.IsSearchControl = false;
		this.txtForeignName.Location = new System.Drawing.Point(164, 44);
		this.txtForeignName.Name = "txtForeignName";
		this.txtForeignName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtForeignName.Properties.Appearance.Options.UseFont = true;
		this.txtForeignName.Properties.AutoHeight = false;
		this.txtForeignName.SettingInVisible = false;
		this.txtForeignName.SettingReadonly = false;
		this.txtForeignName.Size = new System.Drawing.Size(207, 18);
		this.txtForeignName.TabIndex = 5;
		this.txtForeignName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_CardName.EditControl = null;
		this.txt_CardName.FieldName = "CardName";
		this.txt_CardName.IsSearchControl = false;
		this.txt_CardName.Location = new System.Drawing.Point(164, 25);
		this.txt_CardName.Name = "txt_CardName";
		this.txt_CardName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_CardName.Properties.Appearance.Options.UseFont = true;
		this.txt_CardName.Properties.AutoHeight = false;
		this.txt_CardName.SettingInVisible = false;
		this.txt_CardName.SettingReadonly = false;
		this.txt_CardName.Size = new System.Drawing.Size(207, 18);
		this.txt_CardName.TabIndex = 4;
		this.txt_CardName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtFederalTaxID.EditControl = null;
		this.txtFederalTaxID.FieldName = "LicTradNum";
		this.txtFederalTaxID.IsSearchControl = false;
		this.txtFederalTaxID.Location = new System.Drawing.Point(164, 103);
		this.txtFederalTaxID.Name = "txtFederalTaxID";
		this.txtFederalTaxID.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFederalTaxID.Properties.Appearance.Options.UseFont = true;
		this.txtFederalTaxID.Properties.AutoHeight = false;
		this.txtFederalTaxID.SettingInVisible = false;
		this.txtFederalTaxID.SettingReadonly = false;
		this.txtFederalTaxID.Size = new System.Drawing.Size(207, 18);
		this.txtFederalTaxID.TabIndex = 8;
		this.txtFederalTaxID.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.ctMenu_Contact.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemDelete });
		this.ctMenu_Contact.Name = "contextMenuStrip1";
		this.ctMenu_Contact.Size = new System.Drawing.Size(131, 26);
		this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
		this.toolStripMenuItemDelete.Size = new System.Drawing.Size(130, 22);
		this.toolStripMenuItemDelete.Text = "Delete row";
		this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btCancel.Appearance.Options.UseBackColor = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(116, 574);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 12;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btAdd.Appearance.Options.UseBackColor = true;
		this.btAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "Add";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = false;
		this.btAdd.Location = new System.Drawing.Point(14, 574);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(99, 20);
		this.btAdd.TabIndex = 11;
		this.btAdd.Text = "Add";
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.xTab_BP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xTab_BP.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xTab_BP.Appearance.Options.UseBackColor = true;
		this.xTab_BP.IsSearchControl = false;
		this.xTab_BP.Location = new System.Drawing.Point(14, 144);
		this.xTab_BP.Name = "xTab_BP";
		this.xTab_BP.SelectedTabPage = this.tab_General;
		this.xTab_BP.Size = new System.Drawing.Size(873, 424);
		this.xTab_BP.TabIndex = 10;
		this.xTab_BP.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[10] { this.tab_General, this.tab_ContactPerson, this.tab_Address, this.tab_PaymentTerms, this.tab_PaymentRun, this.tab_Accounting, this.tab_Properties, this.tab_Remark, this.tab_AttachmentFiles, this.tab_UserDefineF });
		this.xTab_BP.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xTab_BP_SelectedPageChanged);
		this.tab_General.Appearance.PageClient.BackColor = System.Drawing.SystemColors.Control;
		this.tab_General.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_General.Controls.Add(this.btn_BranchAssignment);
		this.tab_General.Controls.Add(this.lbl_BranchAssignment);
		this.tab_General.Controls.Add(this.txtGeneralRemark);
		this.tab_General.Controls.Add(this.uc_BCFLImg_Language);
		this.tab_General.Controls.Add(this.uc_BCFLImg_SalesBuyer);
		this.tab_General.Controls.Add(this.uc_BCFLImg_ShippingType);
		this.tab_General.Controls.Add(this.luk_Technician);
		this.tab_General.Controls.Add(this.luk_BPIndustry);
		this.tab_General.Controls.Add(this.txt_GeneralAliasName);
		this.tab_General.Controls.Add(this.luk_BusinessType);
		this.tab_General.Controls.Add(this.bte_BPProject);
		this.tab_General.Controls.Add(this.txt_FactoringIndicator);
		this.tab_General.Controls.Add(this.luk_ShippingType);
		this.tab_General.Controls.Add(this.txtGeneralEmail);
		this.tab_General.Controls.Add(this.txtGeneralWebsite);
		this.tab_General.Controls.Add(this.txtGeneralFax);
		this.tab_General.Controls.Add(this.txtGeneralTel2);
		this.tab_General.Controls.Add(this.txtGeneralMobilePhone);
		this.tab_General.Controls.Add(this.txtGeneralTel1);
		this.tab_General.Controls.Add(this.luk_Territory);
		this.tab_General.Controls.Add(this.txt_GLN);
		this.tab_General.Controls.Add(this.luk_Language);
		this.tab_General.Controls.Add(this.bte_BPChannelCode);
		this.tab_General.Controls.Add(this.luk_SalesBuyer);
		this.tab_General.Controls.Add(this.txtGeneralFederalTaxID);
		this.tab_General.Controls.Add(this.txtGeneralContactPerson);
		this.tab_General.Controls.Add(this.txtGeneralIDNo2);
		this.tab_General.Controls.Add(this.txtGeneralPassword);
		this.tab_General.Controls.Add(this.labelLine21);
		this.tab_General.Controls.Add(this.lbl_Language);
		this.tab_General.Controls.Add(this.labelLine19);
		this.tab_General.Controls.Add(this.labelLine17);
		this.tab_General.Controls.Add(this.labelLine18);
		this.tab_General.Controls.Add(this.labelLine16);
		this.tab_General.Controls.Add(this.labelLine15);
		this.tab_General.Controls.Add(this.labelLine14);
		this.tab_General.Controls.Add(this.labelLine13);
		this.tab_General.Controls.Add(this.bte_FactoringIndicator);
		this.tab_General.Controls.Add(this.labelLine12);
		this.tab_General.Controls.Add(this.rdg_Active);
		this.tab_General.Controls.Add(this.panel_Advanced);
		this.tab_General.Controls.Add(this.panel_Inactive);
		this.tab_General.Controls.Add(this.panel_Active);
		this.tab_General.Controls.Add(this.lb_SalesEmpLoyee);
		this.tab_General.Controls.Add(this.lb_ShippingType);
		this.tab_General.Controls.Add(this.lb_Remark);
		this.tab_General.Controls.Add(this.lb_UnifiedFederalTaxID);
		this.tab_General.Controls.Add(this.lb_ContactP);
		this.tab_General.Controls.Add(this.lb_IDno2);
		this.tab_General.Controls.Add(this.lb_Passw);
		this.tab_General.Controls.Add(this.lb_Email);
		this.tab_General.Controls.Add(this.lb_Website);
		this.tab_General.Controls.Add(this.lb_Fax);
		this.tab_General.Controls.Add(this.lb_Tel2);
		this.tab_General.Controls.Add(this.lb_MobiP);
		this.tab_General.Controls.Add(this.lb_Tel1);
		this.tab_General.EditControl = null;
		this.tab_General.EditValue = "&General";
		this.tab_General.FieldName = "tab_General";
		this.tab_General.IsSearchControl = false;
		this.tab_General.Name = "tab_General";
		this.tab_General.ReadOnly = false;
		this.tab_General.SettingInVisible = false;
		this.tab_General.SettingReadonly = false;
		this.tab_General.Size = new System.Drawing.Size(871, 399);
		this.tab_General.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_General.Text = "&General";
		this.btn_BranchAssignment.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_BranchAssignment.EditControl = null;
		this.btn_BranchAssignment.EditValue = "...";
		this.btn_BranchAssignment.FieldName = null;
		this.btn_BranchAssignment.IsSearchControl = false;
		this.btn_BranchAssignment.Location = new System.Drawing.Point(148, 242);
		this.btn_BranchAssignment.Name = "btn_BranchAssignment";
		this.btn_BranchAssignment.ReadOnly = false;
		this.btn_BranchAssignment.SettingInVisible = false;
		this.btn_BranchAssignment.SettingReadonly = false;
		this.btn_BranchAssignment.Size = new System.Drawing.Size(29, 20);
		this.btn_BranchAssignment.TabIndex = 48;
		this.btn_BranchAssignment.Text = "...";
		this.btn_BranchAssignment.Click += new System.EventHandler(this.btn_BranchAssignment_Click);
		this.lbl_BranchAssignment.EditControl = null;
		this.lbl_BranchAssignment.EditValue = "Branch Assignment";
		this.lbl_BranchAssignment.FieldName = null;
		this.lbl_BranchAssignment.HideWhenEmpty = false;
		this.lbl_BranchAssignment.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BranchAssignment.Location = new System.Drawing.Point(11, 244);
		this.lbl_BranchAssignment.Name = "lbl_BranchAssignment";
		this.lbl_BranchAssignment.ReadOnly = false;
		this.lbl_BranchAssignment.SettingInVisible = false;
		this.lbl_BranchAssignment.SettingReadonly = false;
		this.lbl_BranchAssignment.ShowUnderLine = true;
		this.lbl_BranchAssignment.Size = new System.Drawing.Size(140, 20);
		this.lbl_BranchAssignment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BranchAssignment.TabIndex = 47;
		this.lbl_BranchAssignment.TabStop = false;
		this.lbl_BranchAssignment.Utext = "Branch Assignment";
		this.txtGeneralRemark.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtGeneralRemark.EditControl = null;
		this.txtGeneralRemark.FieldName = "Notes";
		this.txtGeneralRemark.IsSearchControl = false;
		this.txtGeneralRemark.Location = new System.Drawing.Point(631, 81);
		this.txtGeneralRemark.Name = "txtGeneralRemark";
		this.txtGeneralRemark.SettingInVisible = false;
		this.txtGeneralRemark.SettingReadonly = false;
		this.txtGeneralRemark.Size = new System.Drawing.Size(207, 40);
		this.txtGeneralRemark.TabIndex = 46;
		this.txtGeneralRemark.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.uc_BCFLImg_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.uc_BCFLImg_Language.CFLColumnName = null;
		this.uc_BCFLImg_Language.CFLDataType = Apzon.Commons.Enums.Enums.CFLButtonImageDataType.IntegerType;
		this.uc_BCFLImg_Language.IsSearchControl = false;
		this.uc_BCFLImg_Language.IsSetCFLText = false;
		this.uc_BCFLImg_Language.Location = new System.Drawing.Point(839, 198);
		this.uc_BCFLImg_Language.Name = "uc_BCFLImg_Language";
		this.uc_BCFLImg_Language.Size = new System.Drawing.Size(18, 18);
		this.uc_BCFLImg_Language.TabIndex = 45;
		this.uc_BCFLImg_SalesBuyer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.uc_BCFLImg_SalesBuyer.CFLColumnName = null;
		this.uc_BCFLImg_SalesBuyer.CFLDataType = Apzon.Commons.Enums.Enums.CFLButtonImageDataType.IntegerType;
		this.uc_BCFLImg_SalesBuyer.IsSearchControl = false;
		this.uc_BCFLImg_SalesBuyer.IsSetCFLText = false;
		this.uc_BCFLImg_SalesBuyer.Location = new System.Drawing.Point(838, 122);
		this.uc_BCFLImg_SalesBuyer.Name = "uc_BCFLImg_SalesBuyer";
		this.uc_BCFLImg_SalesBuyer.Size = new System.Drawing.Size(18, 18);
		this.uc_BCFLImg_SalesBuyer.TabIndex = 44;
		this.uc_BCFLImg_ShippingType.CFLColumnName = null;
		this.uc_BCFLImg_ShippingType.CFLDataType = Apzon.Commons.Enums.Enums.CFLButtonImageDataType.ShortType;
		this.uc_BCFLImg_ShippingType.IsSearchControl = false;
		this.uc_BCFLImg_ShippingType.IsSetCFLText = false;
		this.uc_BCFLImg_ShippingType.Location = new System.Drawing.Point(355, 120);
		this.uc_BCFLImg_ShippingType.Name = "uc_BCFLImg_ShippingType";
		this.uc_BCFLImg_ShippingType.Size = new System.Drawing.Size(18, 18);
		this.uc_BCFLImg_ShippingType.TabIndex = 43;
		this.luk_Technician.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Technician.EditControl = null;
		this.luk_Technician.FieldName = null;
		this.luk_Technician.IsSearchControl = false;
		this.luk_Technician.Location = new System.Drawing.Point(631, 160);
		this.luk_Technician.Name = "luk_Technician";
		this.luk_Technician.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Technician.Properties.Appearance.Options.UseFont = true;
		this.luk_Technician.Properties.AutoHeight = false;
		this.luk_Technician.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.luk_Technician.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Technician.Properties.IsSearchControl = false;
		this.luk_Technician.Properties.NullText = "";
		this.luk_Technician.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Technician_ButtonPressed);
		this.luk_Technician.SettingInVisible = false;
		this.luk_Technician.SettingReadonly = false;
		this.luk_Technician.Size = new System.Drawing.Size(207, 18);
		this.luk_Technician.TabIndex = 21;
		this.luk_Technician.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luk_Technician_QueryPopUp);
		this.luk_Technician.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_BPIndustry.EditControl = null;
		this.luk_BPIndustry.FieldName = "IndustryC";
		this.luk_BPIndustry.IsSearchControl = false;
		this.luk_BPIndustry.Location = new System.Drawing.Point(148, 176);
		this.luk_BPIndustry.Name = "luk_BPIndustry";
		this.luk_BPIndustry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_BPIndustry.Properties.Appearance.Options.UseFont = true;
		this.luk_BPIndustry.Properties.AutoHeight = false;
		this.luk_BPIndustry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BPIndustry.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IndName", "Name")
		});
		this.luk_BPIndustry.Properties.DisplayMember = "SeriesName";
		this.luk_BPIndustry.Properties.IsSearchControl = false;
		this.luk_BPIndustry.Properties.NullText = "";
		this.luk_BPIndustry.Properties.ValueMember = "Series";
		this.luk_BPIndustry.SettingInVisible = false;
		this.luk_BPIndustry.SettingReadonly = false;
		this.luk_BPIndustry.Size = new System.Drawing.Size(207, 18);
		this.luk_BPIndustry.TabIndex = 11;
		this.luk_BPIndustry.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_BPIndustry.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_BPIndustry_EditValueChanging);
		this.txt_GeneralAliasName.EditControl = null;
		this.txt_GeneralAliasName.FieldName = "AliasName";
		this.txt_GeneralAliasName.IsSearchControl = false;
		this.txt_GeneralAliasName.Location = new System.Drawing.Point(148, 214);
		this.txt_GeneralAliasName.Name = "txt_GeneralAliasName";
		this.txt_GeneralAliasName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_GeneralAliasName.Properties.Appearance.Options.UseFont = true;
		this.txt_GeneralAliasName.Properties.AutoHeight = false;
		this.txt_GeneralAliasName.SettingInVisible = false;
		this.txt_GeneralAliasName.SettingReadonly = false;
		this.txt_GeneralAliasName.Size = new System.Drawing.Size(207, 18);
		this.txt_GeneralAliasName.TabIndex = 13;
		this.txt_GeneralAliasName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_BusinessType.EditControl = null;
		this.luk_BusinessType.FieldName = "CmpPrivate";
		this.luk_BusinessType.IsSearchControl = false;
		this.luk_BusinessType.Location = new System.Drawing.Point(148, 195);
		this.luk_BusinessType.Name = "luk_BusinessType";
		this.luk_BusinessType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_BusinessType.Properties.Appearance.Options.UseFont = true;
		this.luk_BusinessType.Properties.AutoHeight = false;
		this.luk_BusinessType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BusinessType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_BusinessType.Properties.DisplayMember = "SeriesName";
		this.luk_BusinessType.Properties.IsSearchControl = false;
		this.luk_BusinessType.Properties.NullText = "";
		this.luk_BusinessType.Properties.ValueMember = "Series";
		this.luk_BusinessType.SettingInVisible = false;
		this.luk_BusinessType.SettingReadonly = false;
		this.luk_BusinessType.Size = new System.Drawing.Size(207, 18);
		this.luk_BusinessType.TabIndex = 12;
		this.luk_BusinessType.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bte_BPProject.EditControl = null;
		this.bte_BPProject.FieldName = "ProjectCod";
		this.bte_BPProject.IsSearchControl = false;
		this.bte_BPProject.Location = new System.Drawing.Point(148, 157);
		this.bte_BPProject.Name = "bte_BPProject";
		this.bte_BPProject.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_BPProject.Properties.Appearance.Options.UseFont = true;
		this.bte_BPProject.Properties.AutoHeight = false;
		this.bte_BPProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPProject.SettingInVisible = false;
		this.bte_BPProject.SettingReadonly = false;
		this.bte_BPProject.Size = new System.Drawing.Size(207, 18);
		this.bte_BPProject.TabIndex = 10;
		this.bte_BPProject.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPProject_ButtonPressed);
		this.bte_BPProject.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_FactoringIndicator.EditControl = null;
		this.txt_FactoringIndicator.FieldName = "FactoringInd";
		this.txt_FactoringIndicator.IsSearchControl = false;
		this.txt_FactoringIndicator.Location = new System.Drawing.Point(195, 138);
		this.txt_FactoringIndicator.Name = "txt_FactoringIndicator";
		this.txt_FactoringIndicator.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_FactoringIndicator.Properties.Appearance.Options.UseFont = true;
		this.txt_FactoringIndicator.Properties.AutoHeight = false;
		this.txt_FactoringIndicator.Properties.MaxLength = 26;
		this.txt_FactoringIndicator.Properties.ReadOnly = true;
		this.txt_FactoringIndicator.SettingInVisible = false;
		this.txt_FactoringIndicator.SettingReadonly = false;
		this.txt_FactoringIndicator.Size = new System.Drawing.Size(160, 18);
		this.txt_FactoringIndicator.TabIndex = 9;
		this.luk_ShippingType.EditControl = null;
		this.luk_ShippingType.FieldName = "ShipType";
		this.luk_ShippingType.IsSearchControl = false;
		this.luk_ShippingType.Location = new System.Drawing.Point(148, 119);
		this.luk_ShippingType.Name = "luk_ShippingType";
		this.luk_ShippingType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_ShippingType.Properties.Appearance.Options.UseFont = true;
		this.luk_ShippingType.Properties.AutoHeight = false;
		this.luk_ShippingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ShippingType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TrnspName", "Name")
		});
		this.luk_ShippingType.Properties.DisplayMember = "TrnspName";
		this.luk_ShippingType.Properties.IsSearchControl = false;
		this.luk_ShippingType.Properties.NullText = "";
		this.luk_ShippingType.Properties.ValueMember = "TrnspCode";
		this.luk_ShippingType.SettingInVisible = false;
		this.luk_ShippingType.SettingReadonly = false;
		this.luk_ShippingType.Size = new System.Drawing.Size(207, 18);
		this.luk_ShippingType.TabIndex = 7;
		this.luk_ShippingType.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_ShippingType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_ShippingType_EditValueChanging);
		this.txtGeneralEmail.EditControl = null;
		this.txtGeneralEmail.FieldName = "E_Mail";
		this.txtGeneralEmail.IsSearchControl = false;
		this.txtGeneralEmail.Location = new System.Drawing.Point(148, 81);
		this.txtGeneralEmail.Name = "txtGeneralEmail";
		this.txtGeneralEmail.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralEmail.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralEmail.Properties.AutoHeight = false;
		this.txtGeneralEmail.SettingInVisible = false;
		this.txtGeneralEmail.SettingReadonly = false;
		this.txtGeneralEmail.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralEmail.TabIndex = 5;
		this.txtGeneralEmail.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralWebsite.EditControl = null;
		this.txtGeneralWebsite.FieldName = "IntrntSite";
		this.txtGeneralWebsite.IsSearchControl = false;
		this.txtGeneralWebsite.Location = new System.Drawing.Point(148, 100);
		this.txtGeneralWebsite.Name = "txtGeneralWebsite";
		this.txtGeneralWebsite.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralWebsite.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralWebsite.Properties.AutoHeight = false;
		this.txtGeneralWebsite.SettingInVisible = false;
		this.txtGeneralWebsite.SettingReadonly = false;
		this.txtGeneralWebsite.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralWebsite.TabIndex = 6;
		this.txtGeneralWebsite.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralFax.EditControl = null;
		this.txtGeneralFax.FieldName = "Fax";
		this.txtGeneralFax.IsSearchControl = false;
		this.txtGeneralFax.Location = new System.Drawing.Point(148, 62);
		this.txtGeneralFax.Name = "txtGeneralFax";
		this.txtGeneralFax.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralFax.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralFax.Properties.AutoHeight = false;
		this.txtGeneralFax.Properties.MaxLength = 26;
		this.txtGeneralFax.SettingInVisible = false;
		this.txtGeneralFax.SettingReadonly = false;
		this.txtGeneralFax.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralFax.TabIndex = 4;
		this.txtGeneralFax.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralTel2.EditControl = null;
		this.txtGeneralTel2.FieldName = "Phone2";
		this.txtGeneralTel2.IsSearchControl = false;
		this.txtGeneralTel2.Location = new System.Drawing.Point(148, 24);
		this.txtGeneralTel2.Name = "txtGeneralTel2";
		this.txtGeneralTel2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralTel2.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralTel2.Properties.AutoHeight = false;
		this.txtGeneralTel2.Properties.MaxLength = 26;
		this.txtGeneralTel2.SettingInVisible = false;
		this.txtGeneralTel2.SettingReadonly = false;
		this.txtGeneralTel2.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralTel2.TabIndex = 2;
		this.txtGeneralTel2.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralMobilePhone.EditControl = null;
		this.txtGeneralMobilePhone.FieldName = "Cellular";
		this.txtGeneralMobilePhone.IsSearchControl = false;
		this.txtGeneralMobilePhone.Location = new System.Drawing.Point(148, 43);
		this.txtGeneralMobilePhone.Name = "txtGeneralMobilePhone";
		this.txtGeneralMobilePhone.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralMobilePhone.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralMobilePhone.Properties.AutoHeight = false;
		this.txtGeneralMobilePhone.Properties.MaxLength = 26;
		this.txtGeneralMobilePhone.SettingInVisible = false;
		this.txtGeneralMobilePhone.SettingReadonly = false;
		this.txtGeneralMobilePhone.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralMobilePhone.TabIndex = 3;
		this.txtGeneralMobilePhone.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralTel1.EditControl = null;
		this.txtGeneralTel1.FieldName = "Phone1";
		this.txtGeneralTel1.IsSearchControl = false;
		this.txtGeneralTel1.Location = new System.Drawing.Point(148, 5);
		this.txtGeneralTel1.Name = "txtGeneralTel1";
		this.txtGeneralTel1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralTel1.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralTel1.Properties.AutoHeight = false;
		this.txtGeneralTel1.Properties.MaxLength = 26;
		this.txtGeneralTel1.SettingInVisible = false;
		this.txtGeneralTel1.SettingReadonly = false;
		this.txtGeneralTel1.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralTel1.TabIndex = 1;
		this.txtGeneralTel1.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_Territory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Territory.EditControl = null;
		this.luk_Territory.FieldName = "Territory";
		this.luk_Territory.IsSearchControl = false;
		this.luk_Territory.Location = new System.Drawing.Point(631, 179);
		this.luk_Territory.Name = "luk_Territory";
		this.luk_Territory.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Territory.Properties.Appearance.Options.UseFont = true;
		this.luk_Territory.Properties.AutoHeight = false;
		this.luk_Territory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.luk_Territory.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("descript", "Name")
		});
		this.luk_Territory.Properties.DisplayMember = "descript";
		this.luk_Territory.Properties.IsSearchControl = false;
		this.luk_Territory.Properties.NullText = "";
		this.luk_Territory.Properties.ValueMember = "territryID";
		this.luk_Territory.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.luk_Territory_Properties_ButtonPressed);
		this.luk_Territory.SettingInVisible = false;
		this.luk_Territory.SettingReadonly = false;
		this.luk_Territory.Size = new System.Drawing.Size(207, 18);
		this.luk_Territory.TabIndex = 22;
		this.luk_Territory.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luk_Territory_QueryPopUp);
		this.luk_Territory.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_GLN.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_GLN.EditControl = null;
		this.txt_GLN.FieldName = "GlblLocNum";
		this.txt_GLN.IsSearchControl = false;
		this.txt_GLN.Location = new System.Drawing.Point(631, 217);
		this.txt_GLN.Name = "txt_GLN";
		this.txt_GLN.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_GLN.Properties.Appearance.Options.UseFont = true;
		this.txt_GLN.Properties.AutoHeight = false;
		this.txt_GLN.SettingInVisible = false;
		this.txt_GLN.SettingReadonly = false;
		this.txt_GLN.Size = new System.Drawing.Size(207, 18);
		this.txt_GLN.TabIndex = 24;
		this.txt_GLN.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Language.EditControl = null;
		this.luk_Language.FieldName = null;
		this.luk_Language.IsSearchControl = false;
		this.luk_Language.Location = new System.Drawing.Point(631, 198);
		this.luk_Language.Name = "luk_Language";
		this.luk_Language.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Language.Properties.Appearance.Options.UseFont = true;
		this.luk_Language.Properties.AutoHeight = false;
		this.luk_Language.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Language.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Language.Properties.DisplayMember = "Name";
		this.luk_Language.Properties.IsSearchControl = false;
		this.luk_Language.Properties.NullText = "";
		this.luk_Language.Properties.ValueMember = "Code";
		this.luk_Language.SettingInVisible = false;
		this.luk_Language.SettingReadonly = false;
		this.luk_Language.Size = new System.Drawing.Size(207, 18);
		this.luk_Language.TabIndex = 23;
		this.luk_Language.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bte_BPChannelCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bte_BPChannelCode.EditControl = null;
		this.bte_BPChannelCode.FieldName = "ChannlBP";
		this.bte_BPChannelCode.IsSearchControl = false;
		this.bte_BPChannelCode.Location = new System.Drawing.Point(631, 141);
		this.bte_BPChannelCode.Name = "bte_BPChannelCode";
		this.bte_BPChannelCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_BPChannelCode.Properties.Appearance.Options.UseFont = true;
		this.bte_BPChannelCode.Properties.AutoHeight = false;
		this.bte_BPChannelCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPChannelCode.SettingInVisible = false;
		this.bte_BPChannelCode.SettingReadonly = false;
		this.bte_BPChannelCode.Size = new System.Drawing.Size(207, 18);
		this.bte_BPChannelCode.TabIndex = 20;
		this.bte_BPChannelCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPChannelCode_ButtonPressed);
		this.bte_BPChannelCode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_SalesBuyer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_SalesBuyer.EditControl = null;
		this.luk_SalesBuyer.FieldName = "SlpCode";
		this.luk_SalesBuyer.IsSearchControl = false;
		this.luk_SalesBuyer.Location = new System.Drawing.Point(631, 122);
		this.luk_SalesBuyer.Name = "luk_SalesBuyer";
		this.luk_SalesBuyer.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_SalesBuyer.Properties.Appearance.Options.UseFont = true;
		this.luk_SalesBuyer.Properties.AutoHeight = false;
		this.luk_SalesBuyer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SalesBuyer.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.luk_SalesBuyer.Properties.DisplayMember = "SlpName";
		this.luk_SalesBuyer.Properties.IsSearchControl = false;
		this.luk_SalesBuyer.Properties.NullText = "";
		this.luk_SalesBuyer.Properties.ValueMember = "SlpCode";
		this.luk_SalesBuyer.SettingInVisible = false;
		this.luk_SalesBuyer.SettingReadonly = false;
		this.luk_SalesBuyer.Size = new System.Drawing.Size(207, 18);
		this.luk_SalesBuyer.TabIndex = 19;
		this.luk_SalesBuyer.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_SalesBuyer.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_SalesBuyer_EditValueChanging);
		this.txtGeneralFederalTaxID.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtGeneralFederalTaxID.EditControl = null;
		this.txtGeneralFederalTaxID.FieldName = "VatIdUnCmp";
		this.txtGeneralFederalTaxID.IsSearchControl = false;
		this.txtGeneralFederalTaxID.Location = new System.Drawing.Point(631, 62);
		this.txtGeneralFederalTaxID.Name = "txtGeneralFederalTaxID";
		this.txtGeneralFederalTaxID.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralFederalTaxID.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralFederalTaxID.Properties.AutoHeight = false;
		this.txtGeneralFederalTaxID.SettingInVisible = false;
		this.txtGeneralFederalTaxID.SettingReadonly = false;
		this.txtGeneralFederalTaxID.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralFederalTaxID.TabIndex = 17;
		this.txtGeneralFederalTaxID.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralContactPerson.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtGeneralContactPerson.EditControl = null;
		this.txtGeneralContactPerson.FieldName = "CntctPrsn";
		this.txtGeneralContactPerson.IsSearchControl = false;
		this.txtGeneralContactPerson.Location = new System.Drawing.Point(631, 24);
		this.txtGeneralContactPerson.Name = "txtGeneralContactPerson";
		this.txtGeneralContactPerson.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralContactPerson.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralContactPerson.Properties.AutoHeight = false;
		this.txtGeneralContactPerson.SettingInVisible = false;
		this.txtGeneralContactPerson.SettingReadonly = false;
		this.txtGeneralContactPerson.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralContactPerson.TabIndex = 15;
		this.txtGeneralIDNo2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtGeneralIDNo2.EditControl = null;
		this.txtGeneralIDNo2.FieldName = "AddID";
		this.txtGeneralIDNo2.IsSearchControl = false;
		this.txtGeneralIDNo2.Location = new System.Drawing.Point(631, 43);
		this.txtGeneralIDNo2.Name = "txtGeneralIDNo2";
		this.txtGeneralIDNo2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralIDNo2.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralIDNo2.Properties.AutoHeight = false;
		this.txtGeneralIDNo2.SettingInVisible = false;
		this.txtGeneralIDNo2.SettingReadonly = false;
		this.txtGeneralIDNo2.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralIDNo2.TabIndex = 16;
		this.txtGeneralIDNo2.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtGeneralPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtGeneralPassword.EditControl = null;
		this.txtGeneralPassword.FieldName = "Password";
		this.txtGeneralPassword.IsSearchControl = false;
		this.txtGeneralPassword.Location = new System.Drawing.Point(631, 5);
		this.txtGeneralPassword.Name = "txtGeneralPassword";
		this.txtGeneralPassword.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtGeneralPassword.Properties.Appearance.Options.UseFont = true;
		this.txtGeneralPassword.Properties.AutoHeight = false;
		this.txtGeneralPassword.SettingInVisible = false;
		this.txtGeneralPassword.SettingReadonly = false;
		this.txtGeneralPassword.Size = new System.Drawing.Size(207, 18);
		this.txtGeneralPassword.TabIndex = 14;
		this.labelLine21.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine21.EditControl = null;
		this.labelLine21.EditValue = "GLN";
		this.labelLine21.FieldName = null;
		this.labelLine21.HideWhenEmpty = false;
		this.labelLine21.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine21.Location = new System.Drawing.Point(493, 218);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.ReadOnly = false;
		this.labelLine21.SettingInVisible = false;
		this.labelLine21.SettingReadonly = false;
		this.labelLine21.ShowUnderLine = true;
		this.labelLine21.Size = new System.Drawing.Size(140, 20);
		this.labelLine21.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine21.TabIndex = 42;
		this.labelLine21.TabStop = false;
		this.labelLine21.Utext = "GLN";
		this.lbl_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Language.EditControl = null;
		this.lbl_Language.EditValue = "Language";
		this.lbl_Language.FieldName = null;
		this.lbl_Language.HideWhenEmpty = false;
		this.lbl_Language.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Language.Location = new System.Drawing.Point(493, 200);
		this.lbl_Language.Name = "lbl_Language";
		this.lbl_Language.ReadOnly = false;
		this.lbl_Language.SettingInVisible = false;
		this.lbl_Language.SettingReadonly = false;
		this.lbl_Language.ShowUnderLine = true;
		this.lbl_Language.Size = new System.Drawing.Size(140, 20);
		this.lbl_Language.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Language.TabIndex = 41;
		this.lbl_Language.TabStop = false;
		this.lbl_Language.Utext = "Language";
		this.labelLine19.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine19.EditControl = null;
		this.labelLine19.EditValue = "Territory";
		this.labelLine19.FieldName = null;
		this.labelLine19.HideWhenEmpty = false;
		this.labelLine19.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine19.Location = new System.Drawing.Point(493, 181);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.ReadOnly = false;
		this.labelLine19.SettingInVisible = false;
		this.labelLine19.SettingReadonly = false;
		this.labelLine19.ShowUnderLine = true;
		this.labelLine19.Size = new System.Drawing.Size(140, 20);
		this.labelLine19.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine19.TabIndex = 40;
		this.labelLine19.TabStop = false;
		this.labelLine19.Utext = "Territory";
		this.labelLine17.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine17.EditControl = null;
		this.labelLine17.EditValue = "Technician";
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(493, 161);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ReadOnly = false;
		this.labelLine17.SettingInVisible = false;
		this.labelLine17.SettingReadonly = false;
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(140, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 39;
		this.labelLine17.TabStop = false;
		this.labelLine17.Utext = "Technician";
		this.labelLine18.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine18.EditControl = null;
		this.labelLine18.EditValue = "BP Channel Code";
		this.labelLine18.FieldName = null;
		this.labelLine18.HideWhenEmpty = false;
		this.labelLine18.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine18.Location = new System.Drawing.Point(493, 142);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.ReadOnly = false;
		this.labelLine18.SettingInVisible = false;
		this.labelLine18.SettingReadonly = false;
		this.labelLine18.ShowUnderLine = true;
		this.labelLine18.Size = new System.Drawing.Size(140, 20);
		this.labelLine18.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine18.TabIndex = 38;
		this.labelLine18.TabStop = false;
		this.labelLine18.Utext = "BP Channel Code";
		this.labelLine16.EditControl = null;
		this.labelLine16.EditValue = "Alias Name";
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(9, 215);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ReadOnly = false;
		this.labelLine16.SettingInVisible = false;
		this.labelLine16.SettingReadonly = false;
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(140, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 31;
		this.labelLine16.TabStop = false;
		this.labelLine16.Utext = "Alias Name";
		this.labelLine15.EditControl = null;
		this.labelLine15.EditValue = "Business Partner Type";
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(9, 196);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ReadOnly = false;
		this.labelLine15.SettingInVisible = false;
		this.labelLine15.SettingReadonly = false;
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(140, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 0;
		this.labelLine15.TabStop = false;
		this.labelLine15.Utext = "Business Partner Type";
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "Industry";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(9, 176);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(140, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 6;
		this.labelLine14.TabStop = false;
		this.labelLine14.Utext = "Industry";
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "BP Project";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(9, 157);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(140, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 0;
		this.labelLine13.TabStop = false;
		this.labelLine13.Utext = "BP Project";
		this.bte_FactoringIndicator.EditControl = null;
		this.bte_FactoringIndicator.FieldName = "Indicator";
		this.bte_FactoringIndicator.IsSearchControl = false;
		this.bte_FactoringIndicator.Location = new System.Drawing.Point(148, 138);
		this.bte_FactoringIndicator.Name = "bte_FactoringIndicator";
		this.bte_FactoringIndicator.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_FactoringIndicator.Properties.Appearance.Options.UseFont = true;
		this.bte_FactoringIndicator.Properties.AutoHeight = false;
		this.bte_FactoringIndicator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_FactoringIndicator.SettingInVisible = false;
		this.bte_FactoringIndicator.SettingReadonly = false;
		this.bte_FactoringIndicator.Size = new System.Drawing.Size(46, 18);
		this.bte_FactoringIndicator.TabIndex = 8;
		this.bte_FactoringIndicator.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_FactoringIndicator_ButtonPressed);
		this.bte_FactoringIndicator.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.bte_FactoringIndicator_InvalidValue);
		this.bte_FactoringIndicator.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bte_FactoringIndicator.Validating += new System.ComponentModel.CancelEventHandler(this.bte_FactoringIndicator_Validating);
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Factoring Indicator";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(9, 139);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(140, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 0;
		this.labelLine12.TabStop = false;
		this.labelLine12.Utext = "Factoring Indicator";
		this.rdg_Active.EditControl = null;
		this.rdg_Active.FieldName = null;
		this.rdg_Active.IsSearchControl = false;
		this.rdg_Active.Location = new System.Drawing.Point(9, 272);
		this.rdg_Active.Name = "rdg_Active";
		this.rdg_Active.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.rdg_Active.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_Active.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_Active.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Active"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Inactive"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Advance")
		});
		this.rdg_Active.SettingInVisible = false;
		this.rdg_Active.SettingReadonly = false;
		this.rdg_Active.Size = new System.Drawing.Size(116, 82);
		this.rdg_Active.TabIndex = 25;
		this.rdg_Active.SelectedIndexChanged += new System.EventHandler(this.rdg_Active_SelectedIndexChanged);
		this.panel_Advanced.Controls.Add(this.txtFrozenComm2);
		this.panel_Advanced.Controls.Add(this.txtValidComm2);
		this.panel_Advanced.Controls.Add(this.dateFrozenTo2);
		this.panel_Advanced.Controls.Add(this.dateFrozenFrom2);
		this.panel_Advanced.Controls.Add(this.dateValidTo2);
		this.panel_Advanced.Controls.Add(this.dateValidFrom2);
		this.panel_Advanced.Controls.Add(this.lb_ToInacti);
		this.panel_Advanced.Controls.Add(this.lb_ToActi);
		this.panel_Advanced.Controls.Add(this.lb_RIacti);
		this.panel_Advanced.Controls.Add(this.lb_RActi);
		this.panel_Advanced.Controls.Add(this.lb_InactiveRangeFrom);
		this.panel_Advanced.Controls.Add(this.lb_ActiveRangeFrom);
		this.panel_Advanced.Location = new System.Drawing.Point(131, 333);
		this.panel_Advanced.Name = "panel_Advanced";
		this.panel_Advanced.Size = new System.Drawing.Size(734, 64);
		this.panel_Advanced.TabIndex = 28;
		this.txtFrozenComm2.EditControl = null;
		this.txtFrozenComm2.FieldName = "FrozenComm";
		this.txtFrozenComm2.IsSearchControl = false;
		this.txtFrozenComm2.Location = new System.Drawing.Point(584, 30);
		this.txtFrozenComm2.Name = "txtFrozenComm2";
		this.txtFrozenComm2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFrozenComm2.Properties.Appearance.Options.UseFont = true;
		this.txtFrozenComm2.Properties.AutoHeight = false;
		this.txtFrozenComm2.SettingInVisible = false;
		this.txtFrozenComm2.SettingReadonly = false;
		this.txtFrozenComm2.Size = new System.Drawing.Size(147, 20);
		this.txtFrozenComm2.TabIndex = 5;
		this.txtFrozenComm2.EditValueChanged += new System.EventHandler(this.txtFrozenComm2_EditValueChanged);
		this.txtValidComm2.EditControl = null;
		this.txtValidComm2.FieldName = "ValidComm";
		this.txtValidComm2.IsSearchControl = false;
		this.txtValidComm2.Location = new System.Drawing.Point(584, 5);
		this.txtValidComm2.Name = "txtValidComm2";
		this.txtValidComm2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtValidComm2.Properties.Appearance.Options.UseFont = true;
		this.txtValidComm2.Properties.AutoHeight = false;
		this.txtValidComm2.SettingInVisible = false;
		this.txtValidComm2.SettingReadonly = false;
		this.txtValidComm2.Size = new System.Drawing.Size(147, 20);
		this.txtValidComm2.TabIndex = 2;
		this.txtValidComm2.EditValueChanged += new System.EventHandler(this.txtValidComm2_EditValueChanged);
		this.dateFrozenTo2.EditControl = null;
		this.dateFrozenTo2.EditValue = null;
		this.dateFrozenTo2.FieldName = "frozenTo";
		this.dateFrozenTo2.Location = new System.Drawing.Point(404, 31);
		this.dateFrozenTo2.Name = "dateFrozenTo2";
		this.dateFrozenTo2.Properties.AutoHeight = false;
		this.dateFrozenTo2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenTo2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenTo2.Properties.IsSearchControl = false;
		this.dateFrozenTo2.SettingInVisible = false;
		this.dateFrozenTo2.SettingReadonly = false;
		this.dateFrozenTo2.Size = new System.Drawing.Size(100, 20);
		this.dateFrozenTo2.TabIndex = 4;
		this.dateFrozenTo2.EditValueChanged += new System.EventHandler(this.dateFrozenTo2_EditValueChanged);
		this.dateFrozenFrom2.EditControl = null;
		this.dateFrozenFrom2.EditValue = null;
		this.dateFrozenFrom2.FieldName = "frozenFrom";
		this.dateFrozenFrom2.Location = new System.Drawing.Point(222, 31);
		this.dateFrozenFrom2.Name = "dateFrozenFrom2";
		this.dateFrozenFrom2.Properties.AutoHeight = false;
		this.dateFrozenFrom2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenFrom2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenFrom2.Properties.IsSearchControl = false;
		this.dateFrozenFrom2.SettingInVisible = false;
		this.dateFrozenFrom2.SettingReadonly = false;
		this.dateFrozenFrom2.Size = new System.Drawing.Size(100, 20);
		this.dateFrozenFrom2.TabIndex = 3;
		this.dateFrozenFrom2.EditValueChanged += new System.EventHandler(this.dateFrozenFrom2_EditValueChanged);
		this.dateValidTo2.EditControl = null;
		this.dateValidTo2.EditValue = null;
		this.dateValidTo2.FieldName = "validTo";
		this.dateValidTo2.Location = new System.Drawing.Point(404, 6);
		this.dateValidTo2.Name = "dateValidTo2";
		this.dateValidTo2.Properties.AutoHeight = false;
		this.dateValidTo2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidTo2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidTo2.Properties.IsSearchControl = false;
		this.dateValidTo2.SettingInVisible = false;
		this.dateValidTo2.SettingReadonly = false;
		this.dateValidTo2.Size = new System.Drawing.Size(100, 20);
		this.dateValidTo2.TabIndex = 1;
		this.dateValidTo2.EditValueChanged += new System.EventHandler(this.dateValidTo2_EditValueChanged);
		this.dateValidFrom2.EditControl = null;
		this.dateValidFrom2.EditValue = null;
		this.dateValidFrom2.FieldName = "validFrom";
		this.dateValidFrom2.Location = new System.Drawing.Point(222, 6);
		this.dateValidFrom2.Name = "dateValidFrom2";
		this.dateValidFrom2.Properties.AutoHeight = false;
		this.dateValidFrom2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidFrom2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidFrom2.Properties.IsSearchControl = false;
		this.dateValidFrom2.SettingInVisible = false;
		this.dateValidFrom2.SettingReadonly = false;
		this.dateValidFrom2.Size = new System.Drawing.Size(100, 20);
		this.dateValidFrom2.TabIndex = 0;
		this.dateValidFrom2.EditValueChanged += new System.EventHandler(this.dateValidFrom2_EditValueChanged);
		this.lb_ToInacti.EditControl = null;
		this.lb_ToInacti.EditValue = "To";
		this.lb_ToInacti.FieldName = null;
		this.lb_ToInacti.HideWhenEmpty = false;
		this.lb_ToInacti.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToInacti.Location = new System.Drawing.Point(326, 34);
		this.lb_ToInacti.Name = "lb_ToInacti";
		this.lb_ToInacti.ReadOnly = false;
		this.lb_ToInacti.SettingInVisible = false;
		this.lb_ToInacti.SettingReadonly = false;
		this.lb_ToInacti.ShowUnderLine = false;
		this.lb_ToInacti.Size = new System.Drawing.Size(80, 20);
		this.lb_ToInacti.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToInacti.TabIndex = 28;
		this.lb_ToInacti.TabStop = false;
		this.lb_ToInacti.Utext = "To";
		this.lb_ToActi.EditControl = null;
		this.lb_ToActi.EditValue = "To";
		this.lb_ToActi.FieldName = null;
		this.lb_ToActi.HideWhenEmpty = false;
		this.lb_ToActi.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToActi.Location = new System.Drawing.Point(326, 8);
		this.lb_ToActi.Name = "lb_ToActi";
		this.lb_ToActi.ReadOnly = false;
		this.lb_ToActi.SettingInVisible = false;
		this.lb_ToActi.SettingReadonly = false;
		this.lb_ToActi.ShowUnderLine = false;
		this.lb_ToActi.Size = new System.Drawing.Size(80, 20);
		this.lb_ToActi.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToActi.TabIndex = 29;
		this.lb_ToActi.TabStop = false;
		this.lb_ToActi.Utext = "To";
		this.lb_RIacti.EditControl = null;
		this.lb_RIacti.EditValue = "Remarks";
		this.lb_RIacti.FieldName = null;
		this.lb_RIacti.HideWhenEmpty = false;
		this.lb_RIacti.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RIacti.Location = new System.Drawing.Point(506, 33);
		this.lb_RIacti.Name = "lb_RIacti";
		this.lb_RIacti.ReadOnly = false;
		this.lb_RIacti.SettingInVisible = false;
		this.lb_RIacti.SettingReadonly = false;
		this.lb_RIacti.ShowUnderLine = false;
		this.lb_RIacti.Size = new System.Drawing.Size(80, 20);
		this.lb_RIacti.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RIacti.TabIndex = 30;
		this.lb_RIacti.TabStop = false;
		this.lb_RIacti.Utext = "Remarks";
		this.lb_RActi.EditControl = null;
		this.lb_RActi.EditValue = "Remarks";
		this.lb_RActi.FieldName = null;
		this.lb_RActi.HideWhenEmpty = false;
		this.lb_RActi.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RActi.Location = new System.Drawing.Point(506, 8);
		this.lb_RActi.Name = "lb_RActi";
		this.lb_RActi.ReadOnly = false;
		this.lb_RActi.SettingInVisible = false;
		this.lb_RActi.SettingReadonly = false;
		this.lb_RActi.ShowUnderLine = false;
		this.lb_RActi.Size = new System.Drawing.Size(80, 20);
		this.lb_RActi.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RActi.TabIndex = 31;
		this.lb_RActi.TabStop = false;
		this.lb_RActi.Utext = "Remarks";
		this.lb_InactiveRangeFrom.EditControl = null;
		this.lb_InactiveRangeFrom.EditValue = "Inactive Range            From";
		this.lb_InactiveRangeFrom.FieldName = null;
		this.lb_InactiveRangeFrom.HideWhenEmpty = false;
		this.lb_InactiveRangeFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_InactiveRangeFrom.Location = new System.Drawing.Point(4, 34);
		this.lb_InactiveRangeFrom.Name = "lb_InactiveRangeFrom";
		this.lb_InactiveRangeFrom.ReadOnly = false;
		this.lb_InactiveRangeFrom.SettingInVisible = false;
		this.lb_InactiveRangeFrom.SettingReadonly = false;
		this.lb_InactiveRangeFrom.ShowUnderLine = false;
		this.lb_InactiveRangeFrom.Size = new System.Drawing.Size(220, 20);
		this.lb_InactiveRangeFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_InactiveRangeFrom.TabIndex = 32;
		this.lb_InactiveRangeFrom.TabStop = false;
		this.lb_InactiveRangeFrom.Utext = "Inactive Range            From";
		this.lb_ActiveRangeFrom.EditControl = null;
		this.lb_ActiveRangeFrom.EditValue = "Active Range               From";
		this.lb_ActiveRangeFrom.FieldName = null;
		this.lb_ActiveRangeFrom.HideWhenEmpty = false;
		this.lb_ActiveRangeFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActiveRangeFrom.Location = new System.Drawing.Point(4, 9);
		this.lb_ActiveRangeFrom.Name = "lb_ActiveRangeFrom";
		this.lb_ActiveRangeFrom.ReadOnly = false;
		this.lb_ActiveRangeFrom.SettingInVisible = false;
		this.lb_ActiveRangeFrom.SettingReadonly = false;
		this.lb_ActiveRangeFrom.ShowUnderLine = false;
		this.lb_ActiveRangeFrom.Size = new System.Drawing.Size(220, 20);
		this.lb_ActiveRangeFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActiveRangeFrom.TabIndex = 33;
		this.lb_ActiveRangeFrom.TabStop = false;
		this.lb_ActiveRangeFrom.Utext = "Active Range               From";
		this.panel_Inactive.Controls.Add(this.txtFrozenComm);
		this.panel_Inactive.Controls.Add(this.dateFrozenTo);
		this.panel_Inactive.Controls.Add(this.dateFrozenFrom);
		this.panel_Inactive.Controls.Add(this.lb_T);
		this.panel_Inactive.Controls.Add(this.lb_R);
		this.panel_Inactive.Controls.Add(this.lb_F);
		this.panel_Inactive.Location = new System.Drawing.Point(148, 296);
		this.panel_Inactive.Name = "panel_Inactive";
		this.panel_Inactive.Size = new System.Drawing.Size(610, 31);
		this.panel_Inactive.TabIndex = 27;
		this.txtFrozenComm.EditControl = null;
		this.txtFrozenComm.FieldName = "FrozenComm";
		this.txtFrozenComm.IsSearchControl = false;
		this.txtFrozenComm.Location = new System.Drawing.Point(461, 5);
		this.txtFrozenComm.Name = "txtFrozenComm";
		this.txtFrozenComm.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFrozenComm.Properties.Appearance.Options.UseFont = true;
		this.txtFrozenComm.Properties.AutoHeight = false;
		this.txtFrozenComm.SettingInVisible = false;
		this.txtFrozenComm.SettingReadonly = false;
		this.txtFrozenComm.Size = new System.Drawing.Size(146, 18);
		this.txtFrozenComm.TabIndex = 2;
		this.txtFrozenComm.EditValueChanged += new System.EventHandler(this.txtFrozenComm_EditValueChanged);
		this.dateFrozenTo.EditControl = null;
		this.dateFrozenTo.EditValue = null;
		this.dateFrozenTo.FieldName = "frozenTo";
		this.dateFrozenTo.Location = new System.Drawing.Point(281, 5);
		this.dateFrozenTo.Name = "dateFrozenTo";
		this.dateFrozenTo.Properties.AutoHeight = false;
		this.dateFrozenTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenTo.Properties.IsSearchControl = false;
		this.dateFrozenTo.SettingInVisible = false;
		this.dateFrozenTo.SettingReadonly = false;
		this.dateFrozenTo.Size = new System.Drawing.Size(100, 18);
		this.dateFrozenTo.TabIndex = 1;
		this.dateFrozenTo.EditValueChanged += new System.EventHandler(this.dateFrozenTo_EditValueChanged);
		this.dateFrozenFrom.EditControl = null;
		this.dateFrozenFrom.EditValue = null;
		this.dateFrozenFrom.FieldName = "frozenFrom";
		this.dateFrozenFrom.Location = new System.Drawing.Point(99, 5);
		this.dateFrozenFrom.Name = "dateFrozenFrom";
		this.dateFrozenFrom.Properties.AutoHeight = false;
		this.dateFrozenFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrozenFrom.Properties.IsSearchControl = false;
		this.dateFrozenFrom.SettingInVisible = false;
		this.dateFrozenFrom.SettingReadonly = false;
		this.dateFrozenFrom.Size = new System.Drawing.Size(100, 18);
		this.dateFrozenFrom.TabIndex = 0;
		this.dateFrozenFrom.EditValueChanged += new System.EventHandler(this.dateFrozenFrom_EditValueChanged);
		this.lb_T.EditControl = null;
		this.lb_T.EditValue = "To";
		this.lb_T.FieldName = null;
		this.lb_T.HideWhenEmpty = false;
		this.lb_T.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_T.Location = new System.Drawing.Point(203, 6);
		this.lb_T.Name = "lb_T";
		this.lb_T.ReadOnly = false;
		this.lb_T.SettingInVisible = false;
		this.lb_T.SettingReadonly = false;
		this.lb_T.ShowUnderLine = false;
		this.lb_T.Size = new System.Drawing.Size(80, 20);
		this.lb_T.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_T.TabIndex = 26;
		this.lb_T.TabStop = false;
		this.lb_T.Utext = "To";
		this.lb_R.EditControl = null;
		this.lb_R.EditValue = "Remarks";
		this.lb_R.FieldName = null;
		this.lb_R.HideWhenEmpty = false;
		this.lb_R.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_R.Location = new System.Drawing.Point(383, 6);
		this.lb_R.Name = "lb_R";
		this.lb_R.ReadOnly = false;
		this.lb_R.SettingInVisible = false;
		this.lb_R.SettingReadonly = false;
		this.lb_R.ShowUnderLine = false;
		this.lb_R.Size = new System.Drawing.Size(80, 20);
		this.lb_R.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_R.TabIndex = 27;
		this.lb_R.TabStop = false;
		this.lb_R.Utext = "Remarks";
		this.lb_F.EditControl = null;
		this.lb_F.EditValue = "From";
		this.lb_F.FieldName = null;
		this.lb_F.HideWhenEmpty = false;
		this.lb_F.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_F.Location = new System.Drawing.Point(21, 6);
		this.lb_F.Name = "lb_F";
		this.lb_F.ReadOnly = false;
		this.lb_F.SettingInVisible = false;
		this.lb_F.SettingReadonly = false;
		this.lb_F.ShowUnderLine = false;
		this.lb_F.Size = new System.Drawing.Size(80, 20);
		this.lb_F.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_F.TabIndex = 28;
		this.lb_F.TabStop = false;
		this.lb_F.Utext = "From";
		this.panel_Active.Controls.Add(this.dateValidFrom);
		this.panel_Active.Controls.Add(this.txtValidComm);
		this.panel_Active.Controls.Add(this.dateValidTo);
		this.panel_Active.Controls.Add(this.lbVailFrom);
		this.panel_Active.Controls.Add(this.lbRemarks);
		this.panel_Active.Controls.Add(this.lbVailTo);
		this.panel_Active.Location = new System.Drawing.Point(148, 268);
		this.panel_Active.Name = "panel_Active";
		this.panel_Active.Size = new System.Drawing.Size(610, 32);
		this.panel_Active.TabIndex = 26;
		this.dateValidFrom.EditControl = null;
		this.dateValidFrom.EditValue = null;
		this.dateValidFrom.FieldName = "validFrom";
		this.dateValidFrom.Location = new System.Drawing.Point(99, 6);
		this.dateValidFrom.Name = "dateValidFrom";
		this.dateValidFrom.Properties.AutoHeight = false;
		this.dateValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidFrom.Properties.IsSearchControl = false;
		this.dateValidFrom.SettingInVisible = false;
		this.dateValidFrom.SettingReadonly = false;
		this.dateValidFrom.Size = new System.Drawing.Size(100, 18);
		this.dateValidFrom.TabIndex = 0;
		this.dateValidFrom.EditValueChanged += new System.EventHandler(this.dateValidFrom_EditValueChanged);
		this.txtValidComm.EditControl = null;
		this.txtValidComm.FieldName = "ValidComm";
		this.txtValidComm.IsSearchControl = false;
		this.txtValidComm.Location = new System.Drawing.Point(461, 6);
		this.txtValidComm.Name = "txtValidComm";
		this.txtValidComm.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtValidComm.Properties.Appearance.Options.UseFont = true;
		this.txtValidComm.Properties.AutoHeight = false;
		this.txtValidComm.SettingInVisible = false;
		this.txtValidComm.SettingReadonly = false;
		this.txtValidComm.Size = new System.Drawing.Size(146, 18);
		this.txtValidComm.TabIndex = 2;
		this.txtValidComm.EditValueChanged += new System.EventHandler(this.txtValidComm_EditValueChanged);
		this.dateValidTo.EditControl = null;
		this.dateValidTo.EditValue = null;
		this.dateValidTo.FieldName = "validTo";
		this.dateValidTo.Location = new System.Drawing.Point(281, 6);
		this.dateValidTo.Name = "dateValidTo";
		this.dateValidTo.Properties.AutoHeight = false;
		this.dateValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateValidTo.Properties.IsSearchControl = false;
		this.dateValidTo.SettingInVisible = false;
		this.dateValidTo.SettingReadonly = false;
		this.dateValidTo.Size = new System.Drawing.Size(100, 18);
		this.dateValidTo.TabIndex = 1;
		this.dateValidTo.EditValueChanged += new System.EventHandler(this.dateValidTo_EditValueChanged);
		this.lbVailFrom.EditControl = null;
		this.lbVailFrom.EditValue = "From";
		this.lbVailFrom.FieldName = null;
		this.lbVailFrom.HideWhenEmpty = false;
		this.lbVailFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbVailFrom.Location = new System.Drawing.Point(21, 7);
		this.lbVailFrom.Name = "lbVailFrom";
		this.lbVailFrom.ReadOnly = false;
		this.lbVailFrom.SettingInVisible = false;
		this.lbVailFrom.SettingReadonly = false;
		this.lbVailFrom.ShowUnderLine = false;
		this.lbVailFrom.Size = new System.Drawing.Size(80, 20);
		this.lbVailFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbVailFrom.TabIndex = 3;
		this.lbVailFrom.TabStop = false;
		this.lbVailFrom.Utext = "From";
		this.lbRemarks.EditControl = null;
		this.lbRemarks.EditValue = "Remarks";
		this.lbRemarks.FieldName = null;
		this.lbRemarks.HideWhenEmpty = false;
		this.lbRemarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRemarks.Location = new System.Drawing.Point(383, 7);
		this.lbRemarks.Name = "lbRemarks";
		this.lbRemarks.ReadOnly = false;
		this.lbRemarks.SettingInVisible = false;
		this.lbRemarks.SettingReadonly = false;
		this.lbRemarks.ShowUnderLine = false;
		this.lbRemarks.Size = new System.Drawing.Size(80, 20);
		this.lbRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRemarks.TabIndex = 28;
		this.lbRemarks.TabStop = false;
		this.lbRemarks.Utext = "Remarks";
		this.lbVailTo.EditControl = null;
		this.lbVailTo.EditValue = "To";
		this.lbVailTo.FieldName = null;
		this.lbVailTo.HideWhenEmpty = false;
		this.lbVailTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbVailTo.Location = new System.Drawing.Point(203, 7);
		this.lbVailTo.Name = "lbVailTo";
		this.lbVailTo.ReadOnly = false;
		this.lbVailTo.SettingInVisible = false;
		this.lbVailTo.SettingReadonly = false;
		this.lbVailTo.ShowUnderLine = false;
		this.lbVailTo.Size = new System.Drawing.Size(80, 20);
		this.lbVailTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbVailTo.TabIndex = 27;
		this.lbVailTo.TabStop = false;
		this.lbVailTo.Utext = "To";
		this.lb_SalesEmpLoyee.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SalesEmpLoyee.EditControl = null;
		this.lb_SalesEmpLoyee.EditValue = "Sales Employee";
		this.lb_SalesEmpLoyee.FieldName = null;
		this.lb_SalesEmpLoyee.HideWhenEmpty = false;
		this.lb_SalesEmpLoyee.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SalesEmpLoyee.Location = new System.Drawing.Point(493, 124);
		this.lb_SalesEmpLoyee.Name = "lb_SalesEmpLoyee";
		this.lb_SalesEmpLoyee.ReadOnly = false;
		this.lb_SalesEmpLoyee.SettingInVisible = false;
		this.lb_SalesEmpLoyee.SettingReadonly = false;
		this.lb_SalesEmpLoyee.ShowUnderLine = true;
		this.lb_SalesEmpLoyee.Size = new System.Drawing.Size(140, 20);
		this.lb_SalesEmpLoyee.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SalesEmpLoyee.TabIndex = 37;
		this.lb_SalesEmpLoyee.TabStop = false;
		this.lb_SalesEmpLoyee.Utext = "Sales Employee";
		this.lb_ShippingType.EditControl = null;
		this.lb_ShippingType.EditValue = "Shipping Type";
		this.lb_ShippingType.FieldName = null;
		this.lb_ShippingType.HideWhenEmpty = false;
		this.lb_ShippingType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ShippingType.Location = new System.Drawing.Point(9, 119);
		this.lb_ShippingType.Name = "lb_ShippingType";
		this.lb_ShippingType.ReadOnly = false;
		this.lb_ShippingType.SettingInVisible = false;
		this.lb_ShippingType.SettingReadonly = false;
		this.lb_ShippingType.ShowUnderLine = true;
		this.lb_ShippingType.Size = new System.Drawing.Size(140, 20);
		this.lb_ShippingType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ShippingType.TabIndex = 0;
		this.lb_ShippingType.TabStop = false;
		this.lb_ShippingType.Utext = "Shipping Type";
		this.lb_Remark.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Remark.EditControl = null;
		this.lb_Remark.EditValue = "Remark";
		this.lb_Remark.FieldName = null;
		this.lb_Remark.HideWhenEmpty = false;
		this.lb_Remark.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remark.Location = new System.Drawing.Point(493, 82);
		this.lb_Remark.Name = "lb_Remark";
		this.lb_Remark.ReadOnly = false;
		this.lb_Remark.SettingInVisible = false;
		this.lb_Remark.SettingReadonly = false;
		this.lb_Remark.ShowUnderLine = true;
		this.lb_Remark.Size = new System.Drawing.Size(140, 20);
		this.lb_Remark.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remark.TabIndex = 36;
		this.lb_Remark.TabStop = false;
		this.lb_Remark.Utext = "Remark";
		this.lb_UnifiedFederalTaxID.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_UnifiedFederalTaxID.EditControl = null;
		this.lb_UnifiedFederalTaxID.EditValue = "Unified Federal Tax ID";
		this.lb_UnifiedFederalTaxID.FieldName = null;
		this.lb_UnifiedFederalTaxID.HideWhenEmpty = false;
		this.lb_UnifiedFederalTaxID.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UnifiedFederalTaxID.Location = new System.Drawing.Point(493, 64);
		this.lb_UnifiedFederalTaxID.Name = "lb_UnifiedFederalTaxID";
		this.lb_UnifiedFederalTaxID.ReadOnly = false;
		this.lb_UnifiedFederalTaxID.SettingInVisible = false;
		this.lb_UnifiedFederalTaxID.SettingReadonly = false;
		this.lb_UnifiedFederalTaxID.ShowUnderLine = true;
		this.lb_UnifiedFederalTaxID.Size = new System.Drawing.Size(140, 20);
		this.lb_UnifiedFederalTaxID.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UnifiedFederalTaxID.TabIndex = 35;
		this.lb_UnifiedFederalTaxID.TabStop = false;
		this.lb_UnifiedFederalTaxID.Utext = "Unified Federal Tax ID";
		this.lb_ContactP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_ContactP.EditControl = null;
		this.lb_ContactP.EditValue = "Contact Person";
		this.lb_ContactP.FieldName = null;
		this.lb_ContactP.HideWhenEmpty = false;
		this.lb_ContactP.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ContactP.Location = new System.Drawing.Point(493, 25);
		this.lb_ContactP.Name = "lb_ContactP";
		this.lb_ContactP.ReadOnly = false;
		this.lb_ContactP.SettingInVisible = false;
		this.lb_ContactP.SettingReadonly = false;
		this.lb_ContactP.ShowUnderLine = true;
		this.lb_ContactP.Size = new System.Drawing.Size(140, 20);
		this.lb_ContactP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ContactP.TabIndex = 33;
		this.lb_ContactP.TabStop = false;
		this.lb_ContactP.Utext = "Contact Person";
		this.lb_IDno2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_IDno2.EditControl = null;
		this.lb_IDno2.EditValue = "ID No. 2";
		this.lb_IDno2.FieldName = null;
		this.lb_IDno2.HideWhenEmpty = false;
		this.lb_IDno2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_IDno2.Location = new System.Drawing.Point(493, 45);
		this.lb_IDno2.Name = "lb_IDno2";
		this.lb_IDno2.ReadOnly = false;
		this.lb_IDno2.SettingInVisible = false;
		this.lb_IDno2.SettingReadonly = false;
		this.lb_IDno2.ShowUnderLine = true;
		this.lb_IDno2.Size = new System.Drawing.Size(140, 20);
		this.lb_IDno2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_IDno2.TabIndex = 34;
		this.lb_IDno2.TabStop = false;
		this.lb_IDno2.Utext = "ID No. 2";
		this.lb_Passw.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Passw.EditControl = null;
		this.lb_Passw.EditValue = "Password";
		this.lb_Passw.FieldName = null;
		this.lb_Passw.HideWhenEmpty = false;
		this.lb_Passw.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Passw.Location = new System.Drawing.Point(493, 6);
		this.lb_Passw.Name = "lb_Passw";
		this.lb_Passw.ReadOnly = false;
		this.lb_Passw.SettingInVisible = false;
		this.lb_Passw.SettingReadonly = false;
		this.lb_Passw.ShowUnderLine = true;
		this.lb_Passw.Size = new System.Drawing.Size(140, 20);
		this.lb_Passw.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Passw.TabIndex = 32;
		this.lb_Passw.TabStop = false;
		this.lb_Passw.Utext = "Password";
		this.lb_Email.EditControl = null;
		this.lb_Email.EditValue = "Email";
		this.lb_Email.FieldName = null;
		this.lb_Email.HideWhenEmpty = false;
		this.lb_Email.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Email.Location = new System.Drawing.Point(9, 82);
		this.lb_Email.Name = "lb_Email";
		this.lb_Email.ReadOnly = false;
		this.lb_Email.SettingInVisible = false;
		this.lb_Email.SettingReadonly = false;
		this.lb_Email.ShowUnderLine = true;
		this.lb_Email.Size = new System.Drawing.Size(140, 20);
		this.lb_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Email.TabIndex = 0;
		this.lb_Email.TabStop = false;
		this.lb_Email.Utext = "Email";
		this.lb_Website.EditControl = null;
		this.lb_Website.EditValue = "Website";
		this.lb_Website.FieldName = null;
		this.lb_Website.HideWhenEmpty = false;
		this.lb_Website.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Website.Location = new System.Drawing.Point(9, 100);
		this.lb_Website.Name = "lb_Website";
		this.lb_Website.ReadOnly = false;
		this.lb_Website.SettingInVisible = false;
		this.lb_Website.SettingReadonly = false;
		this.lb_Website.ShowUnderLine = true;
		this.lb_Website.Size = new System.Drawing.Size(140, 20);
		this.lb_Website.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Website.TabIndex = 0;
		this.lb_Website.TabStop = false;
		this.lb_Website.Utext = "Website";
		this.lb_Fax.EditControl = null;
		this.lb_Fax.EditValue = "Fax";
		this.lb_Fax.FieldName = null;
		this.lb_Fax.HideWhenEmpty = false;
		this.lb_Fax.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Fax.Location = new System.Drawing.Point(9, 63);
		this.lb_Fax.Name = "lb_Fax";
		this.lb_Fax.ReadOnly = false;
		this.lb_Fax.SettingInVisible = false;
		this.lb_Fax.SettingReadonly = false;
		this.lb_Fax.ShowUnderLine = true;
		this.lb_Fax.Size = new System.Drawing.Size(140, 20);
		this.lb_Fax.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Fax.TabIndex = 0;
		this.lb_Fax.TabStop = false;
		this.lb_Fax.Utext = "Fax";
		this.lb_Tel2.EditControl = null;
		this.lb_Tel2.EditValue = "Tel 2";
		this.lb_Tel2.FieldName = null;
		this.lb_Tel2.HideWhenEmpty = false;
		this.lb_Tel2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Tel2.Location = new System.Drawing.Point(9, 25);
		this.lb_Tel2.Name = "lb_Tel2";
		this.lb_Tel2.ReadOnly = false;
		this.lb_Tel2.SettingInVisible = false;
		this.lb_Tel2.SettingReadonly = false;
		this.lb_Tel2.ShowUnderLine = true;
		this.lb_Tel2.Size = new System.Drawing.Size(140, 20);
		this.lb_Tel2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Tel2.TabIndex = 0;
		this.lb_Tel2.TabStop = false;
		this.lb_Tel2.Utext = "Tel 2";
		this.lb_MobiP.EditControl = null;
		this.lb_MobiP.EditValue = "Mobile phone";
		this.lb_MobiP.FieldName = null;
		this.lb_MobiP.HideWhenEmpty = false;
		this.lb_MobiP.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MobiP.Location = new System.Drawing.Point(9, 43);
		this.lb_MobiP.Name = "lb_MobiP";
		this.lb_MobiP.ReadOnly = false;
		this.lb_MobiP.SettingInVisible = false;
		this.lb_MobiP.SettingReadonly = false;
		this.lb_MobiP.ShowUnderLine = true;
		this.lb_MobiP.Size = new System.Drawing.Size(140, 20);
		this.lb_MobiP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MobiP.TabIndex = 0;
		this.lb_MobiP.TabStop = false;
		this.lb_MobiP.Utext = "Mobile phone";
		this.lb_Tel1.EditControl = null;
		this.lb_Tel1.EditValue = "Tel 1";
		this.lb_Tel1.FieldName = null;
		this.lb_Tel1.HideWhenEmpty = false;
		this.lb_Tel1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Tel1.Location = new System.Drawing.Point(9, 6);
		this.lb_Tel1.Name = "lb_Tel1";
		this.lb_Tel1.ReadOnly = false;
		this.lb_Tel1.SettingInVisible = false;
		this.lb_Tel1.SettingReadonly = false;
		this.lb_Tel1.ShowUnderLine = true;
		this.lb_Tel1.Size = new System.Drawing.Size(140, 20);
		this.lb_Tel1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Tel1.TabIndex = 0;
		this.lb_Tel1.TabStop = false;
		this.lb_Tel1.Utext = "Tel 1";
		this.tab_ContactPerson.Controls.Add(this.txt_BirthPlace);
		this.tab_ContactPerson.Controls.Add(this.btContactPersonDefault);
		this.tab_ContactPerson.Controls.Add(this.date_BirthDate);
		this.tab_ContactPerson.Controls.Add(this.chk_Active);
		this.tab_ContactPerson.Controls.Add(this.cb_Gender);
		this.tab_ContactPerson.Controls.Add(this.lb_CityOfB);
		this.tab_ContactPerson.Controls.Add(this.txt_Profession);
		this.tab_ContactPerson.Controls.Add(this.txt_BirthCity);
		this.tab_ContactPerson.Controls.Add(this.lb_Profession);
		this.tab_ContactPerson.Controls.Add(this.lb_Gender);
		this.tab_ContactPerson.Controls.Add(this.lb_DateOfB);
		this.tab_ContactPerson.Controls.Add(this.txt_Password);
		this.tab_ContactPerson.Controls.Add(this.lb_PlaceOfB);
		this.tab_ContactPerson.Controls.Add(this.lb_PassWord);
		this.tab_ContactPerson.Controls.Add(this.txt_Notes1);
		this.tab_ContactPerson.Controls.Add(this.txt_Notes2);
		this.tab_ContactPerson.Controls.Add(this.txt_Pager);
		this.tab_ContactPerson.Controls.Add(this.lb_Remark1);
		this.tab_ContactPerson.Controls.Add(this.lb_Remark2);
		this.tab_ContactPerson.Controls.Add(this.lb_Pager);
		this.tab_ContactPerson.Controls.Add(this.txt_Fax);
		this.tab_ContactPerson.Controls.Add(this.txt_E_MailL);
		this.tab_ContactPerson.Controls.Add(this.txt_Cellolar);
		this.tab_ContactPerson.Controls.Add(this.txt_Tel1);
		this.tab_ContactPerson.Controls.Add(this.txt_Tel2);
		this.tab_ContactPerson.Controls.Add(this.txt_Address);
		this.tab_ContactPerson.Controls.Add(this.txt_Title);
		this.tab_ContactPerson.Controls.Add(this.txt_Position);
		this.tab_ContactPerson.Controls.Add(this.txt_Name);
		this.tab_ContactPerson.Controls.Add(this.grd_Contact);
		this.tab_ContactPerson.Controls.Add(this.lb_EmailCnt);
		this.tab_ContactPerson.Controls.Add(this.lb_FaxCnt);
		this.tab_ContactPerson.Controls.Add(this.lb_MobiPhone);
		this.tab_ContactPerson.Controls.Add(this.lb_TeleP2);
		this.tab_ContactPerson.Controls.Add(this.lb_TeleP1);
		this.tab_ContactPerson.Controls.Add(this.lb_Address);
		this.tab_ContactPerson.Controls.Add(this.lb_Title);
		this.tab_ContactPerson.Controls.Add(this.lb_Position);
		this.tab_ContactPerson.Controls.Add(this.lb_NameCnt);
		this.tab_ContactPerson.EditControl = null;
		this.tab_ContactPerson.EditValue = "&Contact person";
		this.tab_ContactPerson.FieldName = "tab_ContactPerson";
		this.tab_ContactPerson.IsSearchControl = false;
		this.tab_ContactPerson.Name = "tab_ContactPerson";
		this.tab_ContactPerson.ReadOnly = false;
		this.tab_ContactPerson.SettingInVisible = false;
		this.tab_ContactPerson.SettingReadonly = false;
		this.tab_ContactPerson.Size = new System.Drawing.Size(871, 399);
		this.tab_ContactPerson.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ContactPerson.Text = "&Contact person";
		this.txt_BirthPlace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_BirthPlace.EditControl = null;
		this.txt_BirthPlace.FieldName = null;
		this.txt_BirthPlace.IsSearchControl = false;
		this.txt_BirthPlace.Location = new System.Drawing.Point(687, 94);
		this.txt_BirthPlace.Name = "txt_BirthPlace";
		this.txt_BirthPlace.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BirthPlace.Properties.Appearance.Options.UseFont = true;
		this.txt_BirthPlace.Properties.AutoHeight = false;
		this.txt_BirthPlace.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_BirthPlace.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txt_BirthPlace.Properties.DisplayMember = "Name";
		this.txt_BirthPlace.Properties.IsSearchControl = false;
		this.txt_BirthPlace.Properties.NullText = "";
		this.txt_BirthPlace.Properties.ValueMember = "Code";
		this.txt_BirthPlace.SettingInVisible = false;
		this.txt_BirthPlace.SettingReadonly = false;
		this.txt_BirthPlace.Size = new System.Drawing.Size(170, 18);
		this.txt_BirthPlace.TabIndex = 16;
		this.txt_BirthPlace.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BirthPlace.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.btContactPersonDefault.EditControl = null;
		this.btContactPersonDefault.EditValue = "Set as Defa&ult";
		this.btContactPersonDefault.FieldName = null;
		this.btContactPersonDefault.IsSearchControl = false;
		this.btContactPersonDefault.Location = new System.Drawing.Point(90, 351);
		this.btContactPersonDefault.Name = "btContactPersonDefault";
		this.btContactPersonDefault.ReadOnly = false;
		this.btContactPersonDefault.SettingInVisible = false;
		this.btContactPersonDefault.SettingReadonly = false;
		this.btContactPersonDefault.Size = new System.Drawing.Size(127, 23);
		this.btContactPersonDefault.TabIndex = 1;
		this.btContactPersonDefault.Text = "Set as Defa&ult";
		this.btContactPersonDefault.Click += new System.EventHandler(this.btContactPersonDefault_Click);
		this.date_BirthDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_BirthDate.EditControl = null;
		this.date_BirthDate.EditValue = null;
		this.date_BirthDate.FieldName = null;
		this.date_BirthDate.Location = new System.Drawing.Point(687, 113);
		this.date_BirthDate.Name = "date_BirthDate";
		this.date_BirthDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.date_BirthDate.Properties.Appearance.Options.UseFont = true;
		this.date_BirthDate.Properties.AutoHeight = false;
		this.date_BirthDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BirthDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BirthDate.Properties.IsSearchControl = false;
		this.date_BirthDate.Properties.NullDate = new System.DateTime(1900, 1, 1, 11, 27, 31, 0);
		this.date_BirthDate.SettingInVisible = false;
		this.date_BirthDate.SettingReadonly = false;
		this.date_BirthDate.Size = new System.Drawing.Size(170, 18);
		this.date_BirthDate.TabIndex = 17;
		this.date_BirthDate.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.date_BirthDate.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.chk_Active.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Active.EditControl = null;
		this.chk_Active.FieldName = null;
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(406, 194);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.DisplayValueChecked = "Y";
		this.chk_Active.Properties.DisplayValueGrayed = "N";
		this.chk_Active.Properties.DisplayValueUnchecked = "N";
		this.chk_Active.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 11;
		this.chk_Active.CheckedChanged += new System.EventHandler(this.chkContactPersonActive_CheckedChanged);
		this.chk_Active.EditValueChanged += new System.EventHandler(this.txtContactPersonName_Leave);
		this.cb_Gender.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cb_Gender.EditControl = null;
		this.cb_Gender.FieldName = null;
		this.cb_Gender.IsSearchControl = false;
		this.cb_Gender.Location = new System.Drawing.Point(687, 132);
		this.cb_Gender.Name = "cb_Gender";
		this.cb_Gender.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_Gender.Properties.Appearance.Options.UseFont = true;
		this.cb_Gender.Properties.AutoHeight = false;
		this.cb_Gender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Gender.Properties.Items.AddRange(new object[3] { "Male", "Female", "" });
		this.cb_Gender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_Gender.SettingInVisible = false;
		this.cb_Gender.SettingReadonly = false;
		this.cb_Gender.Size = new System.Drawing.Size(170, 18);
		this.cb_Gender.TabIndex = 18;
		this.cb_Gender.SelectedIndexChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cb_Gender.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.lb_CityOfB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_CityOfB.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_CityOfB.Appearance.Options.UseBackColor = true;
		this.lb_CityOfB.EditControl = null;
		this.lb_CityOfB.EditValue = "City of Birth";
		this.lb_CityOfB.FieldName = null;
		this.lb_CityOfB.HideWhenEmpty = false;
		this.lb_CityOfB.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CityOfB.Location = new System.Drawing.Point(587, 172);
		this.lb_CityOfB.Name = "lb_CityOfB";
		this.lb_CityOfB.ReadOnly = false;
		this.lb_CityOfB.SettingInVisible = false;
		this.lb_CityOfB.SettingReadonly = false;
		this.lb_CityOfB.ShowUnderLine = true;
		this.lb_CityOfB.Size = new System.Drawing.Size(100, 20);
		this.lb_CityOfB.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CityOfB.TabIndex = 60;
		this.lb_CityOfB.TabStop = false;
		this.lb_CityOfB.Utext = "City of Birth";
		this.txt_Profession.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Profession.EditControl = null;
		this.txt_Profession.FieldName = null;
		this.txt_Profession.IsSearchControl = false;
		this.txt_Profession.Location = new System.Drawing.Point(687, 151);
		this.txt_Profession.Name = "txt_Profession";
		this.txt_Profession.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Profession.Properties.Appearance.Options.UseFont = true;
		this.txt_Profession.Properties.AutoHeight = false;
		this.txt_Profession.SettingInVisible = false;
		this.txt_Profession.SettingReadonly = false;
		this.txt_Profession.Size = new System.Drawing.Size(170, 18);
		this.txt_Profession.TabIndex = 19;
		this.txt_Profession.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Profession.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_BirthCity.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_BirthCity.EditControl = null;
		this.txt_BirthCity.FieldName = null;
		this.txt_BirthCity.IsSearchControl = false;
		this.txt_BirthCity.Location = new System.Drawing.Point(687, 170);
		this.txt_BirthCity.Name = "txt_BirthCity";
		this.txt_BirthCity.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BirthCity.Properties.Appearance.Options.UseFont = true;
		this.txt_BirthCity.Properties.AutoHeight = false;
		this.txt_BirthCity.SettingInVisible = false;
		this.txt_BirthCity.SettingReadonly = false;
		this.txt_BirthCity.Size = new System.Drawing.Size(170, 18);
		this.txt_BirthCity.TabIndex = 20;
		this.txt_BirthCity.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BirthCity.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.lb_Profession.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Profession.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Profession.Appearance.Options.UseBackColor = true;
		this.lb_Profession.EditControl = null;
		this.lb_Profession.EditValue = "Profession";
		this.lb_Profession.FieldName = null;
		this.lb_Profession.HideWhenEmpty = false;
		this.lb_Profession.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Profession.Location = new System.Drawing.Point(587, 153);
		this.lb_Profession.Name = "lb_Profession";
		this.lb_Profession.ReadOnly = false;
		this.lb_Profession.SettingInVisible = false;
		this.lb_Profession.SettingReadonly = false;
		this.lb_Profession.ShowUnderLine = true;
		this.lb_Profession.Size = new System.Drawing.Size(100, 20);
		this.lb_Profession.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Profession.TabIndex = 56;
		this.lb_Profession.TabStop = false;
		this.lb_Profession.Utext = "Profession";
		this.lb_Gender.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Gender.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Gender.Appearance.Options.UseBackColor = true;
		this.lb_Gender.EditControl = null;
		this.lb_Gender.EditValue = "Gender";
		this.lb_Gender.FieldName = null;
		this.lb_Gender.HideWhenEmpty = false;
		this.lb_Gender.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Gender.Location = new System.Drawing.Point(587, 133);
		this.lb_Gender.Name = "lb_Gender";
		this.lb_Gender.ReadOnly = false;
		this.lb_Gender.SettingInVisible = false;
		this.lb_Gender.SettingReadonly = false;
		this.lb_Gender.ShowUnderLine = true;
		this.lb_Gender.Size = new System.Drawing.Size(100, 20);
		this.lb_Gender.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Gender.TabIndex = 55;
		this.lb_Gender.TabStop = false;
		this.lb_Gender.Utext = "Gender";
		this.lb_DateOfB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_DateOfB.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_DateOfB.Appearance.Options.UseBackColor = true;
		this.lb_DateOfB.EditControl = null;
		this.lb_DateOfB.EditValue = "Date of Birth";
		this.lb_DateOfB.FieldName = null;
		this.lb_DateOfB.HideWhenEmpty = false;
		this.lb_DateOfB.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DateOfB.Location = new System.Drawing.Point(587, 114);
		this.lb_DateOfB.Name = "lb_DateOfB";
		this.lb_DateOfB.ReadOnly = false;
		this.lb_DateOfB.SettingInVisible = false;
		this.lb_DateOfB.SettingReadonly = false;
		this.lb_DateOfB.ShowUnderLine = true;
		this.lb_DateOfB.Size = new System.Drawing.Size(100, 20);
		this.lb_DateOfB.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DateOfB.TabIndex = 54;
		this.lb_DateOfB.TabStop = false;
		this.lb_DateOfB.Utext = "Date of Birth";
		this.txt_Password.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Password.EditControl = null;
		this.txt_Password.FieldName = null;
		this.txt_Password.IsSearchControl = false;
		this.txt_Password.Location = new System.Drawing.Point(687, 75);
		this.txt_Password.Name = "txt_Password";
		this.txt_Password.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Password.Properties.Appearance.Options.UseFont = true;
		this.txt_Password.Properties.AutoHeight = false;
		this.txt_Password.SettingInVisible = false;
		this.txt_Password.SettingReadonly = false;
		this.txt_Password.Size = new System.Drawing.Size(170, 18);
		this.txt_Password.TabIndex = 15;
		this.txt_Password.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Password.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.lb_PlaceOfB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_PlaceOfB.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_PlaceOfB.Appearance.Options.UseBackColor = true;
		this.lb_PlaceOfB.EditControl = null;
		this.lb_PlaceOfB.EditValue = "Country of Birth";
		this.lb_PlaceOfB.FieldName = null;
		this.lb_PlaceOfB.HideWhenEmpty = false;
		this.lb_PlaceOfB.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PlaceOfB.Location = new System.Drawing.Point(587, 97);
		this.lb_PlaceOfB.Name = "lb_PlaceOfB";
		this.lb_PlaceOfB.ReadOnly = false;
		this.lb_PlaceOfB.SettingInVisible = false;
		this.lb_PlaceOfB.SettingReadonly = false;
		this.lb_PlaceOfB.ShowUnderLine = true;
		this.lb_PlaceOfB.Size = new System.Drawing.Size(100, 20);
		this.lb_PlaceOfB.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PlaceOfB.TabIndex = 50;
		this.lb_PlaceOfB.TabStop = false;
		this.lb_PlaceOfB.Utext = "Country of Birth";
		this.lb_PassWord.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_PassWord.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_PassWord.Appearance.Options.UseBackColor = true;
		this.lb_PassWord.EditControl = null;
		this.lb_PassWord.EditValue = "Password";
		this.lb_PassWord.FieldName = null;
		this.lb_PassWord.HideWhenEmpty = false;
		this.lb_PassWord.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PassWord.Location = new System.Drawing.Point(587, 77);
		this.lb_PassWord.Name = "lb_PassWord";
		this.lb_PassWord.ReadOnly = false;
		this.lb_PassWord.SettingInVisible = false;
		this.lb_PassWord.SettingReadonly = false;
		this.lb_PassWord.ShowUnderLine = true;
		this.lb_PassWord.Size = new System.Drawing.Size(100, 20);
		this.lb_PassWord.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PassWord.TabIndex = 49;
		this.lb_PassWord.TabStop = false;
		this.lb_PassWord.Utext = "Password";
		this.txt_Notes1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Notes1.EditControl = null;
		this.txt_Notes1.FieldName = null;
		this.txt_Notes1.IsSearchControl = false;
		this.txt_Notes1.Location = new System.Drawing.Point(687, 37);
		this.txt_Notes1.Name = "txt_Notes1";
		this.txt_Notes1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Notes1.Properties.Appearance.Options.UseFont = true;
		this.txt_Notes1.Properties.AutoHeight = false;
		this.txt_Notes1.SettingInVisible = false;
		this.txt_Notes1.SettingReadonly = false;
		this.txt_Notes1.Size = new System.Drawing.Size(170, 18);
		this.txt_Notes1.TabIndex = 13;
		this.txt_Notes1.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Notes1.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Notes2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Notes2.EditControl = null;
		this.txt_Notes2.FieldName = null;
		this.txt_Notes2.IsSearchControl = false;
		this.txt_Notes2.Location = new System.Drawing.Point(687, 56);
		this.txt_Notes2.Name = "txt_Notes2";
		this.txt_Notes2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Notes2.Properties.Appearance.Options.UseFont = true;
		this.txt_Notes2.Properties.AutoHeight = false;
		this.txt_Notes2.SettingInVisible = false;
		this.txt_Notes2.SettingReadonly = false;
		this.txt_Notes2.Size = new System.Drawing.Size(170, 18);
		this.txt_Notes2.TabIndex = 14;
		this.txt_Notes2.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Notes2.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Pager.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Pager.EditControl = null;
		this.txt_Pager.FieldName = null;
		this.txt_Pager.IsSearchControl = false;
		this.txt_Pager.Location = new System.Drawing.Point(687, 18);
		this.txt_Pager.Name = "txt_Pager";
		this.txt_Pager.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Pager.Properties.Appearance.Options.UseFont = true;
		this.txt_Pager.Properties.AutoHeight = false;
		this.txt_Pager.SettingInVisible = false;
		this.txt_Pager.SettingReadonly = false;
		this.txt_Pager.Size = new System.Drawing.Size(170, 18);
		this.txt_Pager.TabIndex = 12;
		this.txt_Pager.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Pager.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.lb_Remark1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Remark1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Remark1.Appearance.Options.UseBackColor = true;
		this.lb_Remark1.EditControl = null;
		this.lb_Remark1.EditValue = "Remark 1";
		this.lb_Remark1.FieldName = null;
		this.lb_Remark1.HideWhenEmpty = false;
		this.lb_Remark1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remark1.Location = new System.Drawing.Point(587, 39);
		this.lb_Remark1.Name = "lb_Remark1";
		this.lb_Remark1.ReadOnly = false;
		this.lb_Remark1.SettingInVisible = false;
		this.lb_Remark1.SettingReadonly = false;
		this.lb_Remark1.ShowUnderLine = true;
		this.lb_Remark1.Size = new System.Drawing.Size(100, 20);
		this.lb_Remark1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remark1.TabIndex = 45;
		this.lb_Remark1.TabStop = false;
		this.lb_Remark1.Utext = "Remark 1";
		this.lb_Remark2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Remark2.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Remark2.Appearance.Options.UseBackColor = true;
		this.lb_Remark2.EditControl = null;
		this.lb_Remark2.EditValue = "Remark 2";
		this.lb_Remark2.FieldName = null;
		this.lb_Remark2.HideWhenEmpty = false;
		this.lb_Remark2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remark2.Location = new System.Drawing.Point(587, 60);
		this.lb_Remark2.Name = "lb_Remark2";
		this.lb_Remark2.ReadOnly = false;
		this.lb_Remark2.SettingInVisible = false;
		this.lb_Remark2.SettingReadonly = false;
		this.lb_Remark2.ShowUnderLine = true;
		this.lb_Remark2.Size = new System.Drawing.Size(100, 20);
		this.lb_Remark2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remark2.TabIndex = 44;
		this.lb_Remark2.TabStop = false;
		this.lb_Remark2.Utext = "Remark 2";
		this.lb_Pager.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Pager.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Pager.Appearance.Options.UseBackColor = true;
		this.lb_Pager.EditControl = null;
		this.lb_Pager.EditValue = "Pager";
		this.lb_Pager.FieldName = null;
		this.lb_Pager.HideWhenEmpty = false;
		this.lb_Pager.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Pager.Location = new System.Drawing.Point(587, 20);
		this.lb_Pager.Name = "lb_Pager";
		this.lb_Pager.ReadOnly = false;
		this.lb_Pager.SettingInVisible = false;
		this.lb_Pager.SettingReadonly = false;
		this.lb_Pager.ShowUnderLine = true;
		this.lb_Pager.Size = new System.Drawing.Size(100, 20);
		this.lb_Pager.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Pager.TabIndex = 43;
		this.lb_Pager.TabStop = false;
		this.lb_Pager.Utext = "Pager";
		this.txt_Fax.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Fax.EditControl = null;
		this.txt_Fax.FieldName = null;
		this.txt_Fax.IsSearchControl = false;
		this.txt_Fax.Location = new System.Drawing.Point(406, 148);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Fax.Properties.Appearance.Options.UseFont = true;
		this.txt_Fax.Properties.AutoHeight = false;
		this.txt_Fax.Properties.MaxLength = 26;
		this.txt_Fax.SettingInVisible = false;
		this.txt_Fax.SettingReadonly = false;
		this.txt_Fax.Size = new System.Drawing.Size(170, 18);
		this.txt_Fax.TabIndex = 9;
		this.txt_Fax.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Fax.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_E_MailL.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_E_MailL.EditControl = null;
		this.txt_E_MailL.FieldName = null;
		this.txt_E_MailL.IsSearchControl = false;
		this.txt_E_MailL.Location = new System.Drawing.Point(406, 167);
		this.txt_E_MailL.Name = "txt_E_MailL";
		this.txt_E_MailL.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_E_MailL.Properties.Appearance.Options.UseFont = true;
		this.txt_E_MailL.Properties.AutoHeight = false;
		this.txt_E_MailL.SettingInVisible = false;
		this.txt_E_MailL.SettingReadonly = false;
		this.txt_E_MailL.Size = new System.Drawing.Size(170, 18);
		this.txt_E_MailL.TabIndex = 10;
		this.txt_E_MailL.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_E_MailL.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Cellolar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Cellolar.EditControl = null;
		this.txt_Cellolar.FieldName = null;
		this.txt_Cellolar.IsSearchControl = false;
		this.txt_Cellolar.Location = new System.Drawing.Point(406, 129);
		this.txt_Cellolar.Name = "txt_Cellolar";
		this.txt_Cellolar.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Cellolar.Properties.Appearance.Options.UseFont = true;
		this.txt_Cellolar.Properties.AutoHeight = false;
		this.txt_Cellolar.Properties.MaxLength = 26;
		this.txt_Cellolar.SettingInVisible = false;
		this.txt_Cellolar.SettingReadonly = false;
		this.txt_Cellolar.Size = new System.Drawing.Size(170, 18);
		this.txt_Cellolar.TabIndex = 8;
		this.txt_Cellolar.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Cellolar.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Tel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Tel1.EditControl = null;
		this.txt_Tel1.FieldName = null;
		this.txt_Tel1.IsSearchControl = false;
		this.txt_Tel1.Location = new System.Drawing.Point(406, 91);
		this.txt_Tel1.Name = "txt_Tel1";
		this.txt_Tel1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Tel1.Properties.Appearance.Options.UseFont = true;
		this.txt_Tel1.Properties.AutoHeight = false;
		this.txt_Tel1.Properties.MaxLength = 26;
		this.txt_Tel1.SettingInVisible = false;
		this.txt_Tel1.SettingReadonly = false;
		this.txt_Tel1.Size = new System.Drawing.Size(170, 18);
		this.txt_Tel1.TabIndex = 6;
		this.txt_Tel1.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Tel1.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Tel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Tel2.EditControl = null;
		this.txt_Tel2.FieldName = null;
		this.txt_Tel2.IsSearchControl = false;
		this.txt_Tel2.Location = new System.Drawing.Point(406, 110);
		this.txt_Tel2.Name = "txt_Tel2";
		this.txt_Tel2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Tel2.Properties.Appearance.Options.UseFont = true;
		this.txt_Tel2.Properties.AutoHeight = false;
		this.txt_Tel2.Properties.MaxLength = 26;
		this.txt_Tel2.SettingInVisible = false;
		this.txt_Tel2.SettingReadonly = false;
		this.txt_Tel2.Size = new System.Drawing.Size(170, 18);
		this.txt_Tel2.TabIndex = 7;
		this.txt_Tel2.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Tel2.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Address.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Address.EditControl = null;
		this.txt_Address.FieldName = null;
		this.txt_Address.IsSearchControl = false;
		this.txt_Address.Location = new System.Drawing.Point(406, 72);
		this.txt_Address.Name = "txt_Address";
		this.txt_Address.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Address.Properties.Appearance.Options.UseFont = true;
		this.txt_Address.Properties.AutoHeight = false;
		this.txt_Address.SettingInVisible = false;
		this.txt_Address.SettingReadonly = false;
		this.txt_Address.Size = new System.Drawing.Size(170, 18);
		this.txt_Address.TabIndex = 5;
		this.txt_Address.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Address.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Title.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Title.EditControl = null;
		this.txt_Title.FieldName = null;
		this.txt_Title.IsSearchControl = false;
		this.txt_Title.Location = new System.Drawing.Point(406, 34);
		this.txt_Title.Name = "txt_Title";
		this.txt_Title.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Title.Properties.Appearance.Options.UseFont = true;
		this.txt_Title.Properties.AutoHeight = false;
		this.txt_Title.SettingInVisible = false;
		this.txt_Title.SettingReadonly = false;
		this.txt_Title.Size = new System.Drawing.Size(170, 18);
		this.txt_Title.TabIndex = 3;
		this.txt_Title.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Title.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Position.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Position.EditControl = null;
		this.txt_Position.FieldName = null;
		this.txt_Position.IsSearchControl = false;
		this.txt_Position.Location = new System.Drawing.Point(406, 53);
		this.txt_Position.Name = "txt_Position";
		this.txt_Position.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Position.Properties.Appearance.Options.UseFont = true;
		this.txt_Position.Properties.AutoHeight = false;
		this.txt_Position.SettingInVisible = false;
		this.txt_Position.SettingReadonly = false;
		this.txt_Position.Size = new System.Drawing.Size(170, 18);
		this.txt_Position.TabIndex = 4;
		this.txt_Position.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Position.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Name.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = "";
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(406, 15);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Name.Properties.Appearance.Options.UseFont = true;
		this.txt_Name.Properties.AutoHeight = false;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(170, 18);
		this.txt_Name.TabIndex = 2;
		this.txt_Name.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_Name.Leave += new System.EventHandler(this.txtContactPersonName_Leave);
		this.txt_Name.Validating += new System.ComponentModel.CancelEventHandler(this.txtContactPersonName_Validating);
		this.grd_Contact.AllowPasteExcel = false;
		this.grd_Contact.ClipboardData = null;
		this.grd_Contact.ContextMenuStrip = this.ctMenu_Contact;
		this.grd_Contact.Location = new System.Drawing.Point(9, 17);
		this.grd_Contact.MainView = this.gv_Contact;
		this.grd_Contact.Name = "grd_Contact";
		this.grd_Contact.Size = new System.Drawing.Size(291, 330);
		this.grd_Contact.TabIndex = 0;
		this.grd_Contact.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Contact });
		this.gv_Contact.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Contact.ColumnKey = null;
		this.gv_Contact.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcName, this.CntctCode });
		this.gv_Contact.DisableFormSetting = false;
		this.gv_Contact.DisplayException = false;
		this.gv_Contact.EditColumn = null;
		this.gv_Contact.FieldCurrency = null;
		this.gv_Contact.FieldRate = null;
		this.gv_Contact.FormId = "oBussinessPartnerMasterData";
		this.gv_Contact.GridControl = this.grd_Contact;
		this.gv_Contact.HasFormSetting = false;
		this.gv_Contact.IgnoreFilter = false;
		this.gv_Contact.IndexEditRow = -1;
		this.gv_Contact.IndicatorWidth = 35;
		this.gv_Contact.IsApzTable = false;
		this.gv_Contact.IsInsertNewRow = false;
		this.gv_Contact.IsLoadingData = false;
		this.gv_Contact.IsMappingContextAction = false;
		this.gv_Contact.ItemId = "OCONTACT";
		this.gv_Contact.LstColumnAllowBatch = null;
		this.gv_Contact.LstColumnNotAllowBatch = null;
		this.gv_Contact.LstColumnNotEditable = null;
		this.gv_Contact.LstColumNotSetting = null;
		this.gv_Contact.LstOldUDF = null;
		this.gv_Contact.Name = "gv_Contact";
		this.gv_Contact.OptionsFind.AllowFindPanel = false;
		this.gv_Contact.OptionsNavigation.UseTabKey = false;
		this.gv_Contact.OptionsView.ShowGroupPanel = false;
		this.gv_Contact.UseBatchMod = false;
		this.gv_Contact.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Contact_RowStyle);
		this.gv_Contact.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.OptionsColumn.AllowEdit = false;
		this.gcName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcName.OptionsColumn.ReadOnly = true;
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 0;
		this.CntctCode.Caption = "CntctCode";
		this.CntctCode.CurrencyDateField = null;
		this.CntctCode.CurrencyField = null;
		this.CntctCode.CurrencyFixed = null;
		this.CntctCode.CurrencyFixedRate = 0.0;
		this.CntctCode.CurrencyRateField = null;
		this.CntctCode.FieldName = "CntctCode";
		this.CntctCode.Name = "CntctCode";
		this.CntctCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.CntctCode.PrimaryKey = false;
		this.CntctCode.UnitField = null;
		this.lb_EmailCnt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_EmailCnt.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_EmailCnt.Appearance.Options.UseBackColor = true;
		this.lb_EmailCnt.EditControl = null;
		this.lb_EmailCnt.EditValue = "Email";
		this.lb_EmailCnt.FieldName = null;
		this.lb_EmailCnt.HideWhenEmpty = false;
		this.lb_EmailCnt.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_EmailCnt.Location = new System.Drawing.Point(306, 169);
		this.lb_EmailCnt.Name = "lb_EmailCnt";
		this.lb_EmailCnt.ReadOnly = false;
		this.lb_EmailCnt.SettingInVisible = false;
		this.lb_EmailCnt.SettingReadonly = false;
		this.lb_EmailCnt.ShowUnderLine = true;
		this.lb_EmailCnt.Size = new System.Drawing.Size(100, 20);
		this.lb_EmailCnt.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_EmailCnt.TabIndex = 42;
		this.lb_EmailCnt.TabStop = false;
		this.lb_EmailCnt.Utext = "Email";
		this.lb_FaxCnt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_FaxCnt.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_FaxCnt.Appearance.Options.UseBackColor = true;
		this.lb_FaxCnt.EditControl = null;
		this.lb_FaxCnt.EditValue = "Fax";
		this.lb_FaxCnt.FieldName = null;
		this.lb_FaxCnt.HideWhenEmpty = false;
		this.lb_FaxCnt.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FaxCnt.Location = new System.Drawing.Point(306, 150);
		this.lb_FaxCnt.Name = "lb_FaxCnt";
		this.lb_FaxCnt.ReadOnly = false;
		this.lb_FaxCnt.SettingInVisible = false;
		this.lb_FaxCnt.SettingReadonly = false;
		this.lb_FaxCnt.ShowUnderLine = true;
		this.lb_FaxCnt.Size = new System.Drawing.Size(100, 20);
		this.lb_FaxCnt.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FaxCnt.TabIndex = 38;
		this.lb_FaxCnt.TabStop = false;
		this.lb_FaxCnt.Utext = "Fax";
		this.lb_MobiPhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_MobiPhone.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_MobiPhone.Appearance.Options.UseBackColor = true;
		this.lb_MobiPhone.EditControl = null;
		this.lb_MobiPhone.EditValue = "Mobile Phone";
		this.lb_MobiPhone.FieldName = null;
		this.lb_MobiPhone.HideWhenEmpty = false;
		this.lb_MobiPhone.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MobiPhone.Location = new System.Drawing.Point(306, 132);
		this.lb_MobiPhone.Name = "lb_MobiPhone";
		this.lb_MobiPhone.ReadOnly = false;
		this.lb_MobiPhone.SettingInVisible = false;
		this.lb_MobiPhone.SettingReadonly = false;
		this.lb_MobiPhone.ShowUnderLine = true;
		this.lb_MobiPhone.Size = new System.Drawing.Size(100, 20);
		this.lb_MobiPhone.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MobiPhone.TabIndex = 37;
		this.lb_MobiPhone.TabStop = false;
		this.lb_MobiPhone.Utext = "Mobile Phone";
		this.lb_TeleP2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_TeleP2.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_TeleP2.Appearance.Options.UseBackColor = true;
		this.lb_TeleP2.EditControl = null;
		this.lb_TeleP2.EditValue = "Telephone 2";
		this.lb_TeleP2.FieldName = null;
		this.lb_TeleP2.HideWhenEmpty = false;
		this.lb_TeleP2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TeleP2.Location = new System.Drawing.Point(306, 112);
		this.lb_TeleP2.Name = "lb_TeleP2";
		this.lb_TeleP2.ReadOnly = false;
		this.lb_TeleP2.SettingInVisible = false;
		this.lb_TeleP2.SettingReadonly = false;
		this.lb_TeleP2.ShowUnderLine = true;
		this.lb_TeleP2.Size = new System.Drawing.Size(100, 20);
		this.lb_TeleP2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TeleP2.TabIndex = 36;
		this.lb_TeleP2.TabStop = false;
		this.lb_TeleP2.Utext = "Telephone 2";
		this.lb_TeleP1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_TeleP1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_TeleP1.Appearance.Options.UseBackColor = true;
		this.lb_TeleP1.EditControl = null;
		this.lb_TeleP1.EditValue = "Telephone 1";
		this.lb_TeleP1.FieldName = null;
		this.lb_TeleP1.HideWhenEmpty = false;
		this.lb_TeleP1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TeleP1.Location = new System.Drawing.Point(306, 94);
		this.lb_TeleP1.Name = "lb_TeleP1";
		this.lb_TeleP1.ReadOnly = false;
		this.lb_TeleP1.SettingInVisible = false;
		this.lb_TeleP1.SettingReadonly = false;
		this.lb_TeleP1.ShowUnderLine = true;
		this.lb_TeleP1.Size = new System.Drawing.Size(100, 20);
		this.lb_TeleP1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TeleP1.TabIndex = 32;
		this.lb_TeleP1.TabStop = false;
		this.lb_TeleP1.Utext = "Telephone 1";
		this.lb_Address.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Address.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Address.Appearance.Options.UseBackColor = true;
		this.lb_Address.EditControl = null;
		this.lb_Address.EditValue = "Address";
		this.lb_Address.FieldName = null;
		this.lb_Address.HideWhenEmpty = false;
		this.lb_Address.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Address.Location = new System.Drawing.Point(306, 75);
		this.lb_Address.Name = "lb_Address";
		this.lb_Address.ReadOnly = false;
		this.lb_Address.SettingInVisible = false;
		this.lb_Address.SettingReadonly = false;
		this.lb_Address.ShowUnderLine = true;
		this.lb_Address.Size = new System.Drawing.Size(100, 20);
		this.lb_Address.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Address.TabIndex = 31;
		this.lb_Address.TabStop = false;
		this.lb_Address.Utext = "Address";
		this.lb_Title.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Title.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Title.Appearance.Options.UseBackColor = true;
		this.lb_Title.EditControl = null;
		this.lb_Title.EditValue = "Title";
		this.lb_Title.FieldName = null;
		this.lb_Title.HideWhenEmpty = false;
		this.lb_Title.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Title.Location = new System.Drawing.Point(306, 37);
		this.lb_Title.Name = "lb_Title";
		this.lb_Title.ReadOnly = false;
		this.lb_Title.SettingInVisible = false;
		this.lb_Title.SettingReadonly = false;
		this.lb_Title.ShowUnderLine = true;
		this.lb_Title.Size = new System.Drawing.Size(100, 20);
		this.lb_Title.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Title.TabIndex = 27;
		this.lb_Title.TabStop = false;
		this.lb_Title.Utext = "Title";
		this.lb_Position.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Position.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Position.Appearance.Options.UseBackColor = true;
		this.lb_Position.EditControl = null;
		this.lb_Position.EditValue = "Position";
		this.lb_Position.FieldName = null;
		this.lb_Position.HideWhenEmpty = false;
		this.lb_Position.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Position.Location = new System.Drawing.Point(306, 56);
		this.lb_Position.Name = "lb_Position";
		this.lb_Position.ReadOnly = false;
		this.lb_Position.SettingInVisible = false;
		this.lb_Position.SettingReadonly = false;
		this.lb_Position.ShowUnderLine = true;
		this.lb_Position.Size = new System.Drawing.Size(100, 20);
		this.lb_Position.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Position.TabIndex = 26;
		this.lb_Position.TabStop = false;
		this.lb_Position.Utext = "Position";
		this.lb_NameCnt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_NameCnt.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_NameCnt.Appearance.Options.UseBackColor = true;
		this.lb_NameCnt.EditControl = null;
		this.lb_NameCnt.EditValue = "Name";
		this.lb_NameCnt.FieldName = null;
		this.lb_NameCnt.HideWhenEmpty = false;
		this.lb_NameCnt.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NameCnt.Location = new System.Drawing.Point(306, 18);
		this.lb_NameCnt.Name = "lb_NameCnt";
		this.lb_NameCnt.ReadOnly = false;
		this.lb_NameCnt.SettingInVisible = false;
		this.lb_NameCnt.SettingReadonly = false;
		this.lb_NameCnt.ShowUnderLine = true;
		this.lb_NameCnt.Size = new System.Drawing.Size(100, 20);
		this.lb_NameCnt.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NameCnt.TabIndex = 25;
		this.lb_NameCnt.TabStop = false;
		this.lb_NameCnt.Utext = "Name";
		this.tab_Address.Controls.Add(this.btn_AddressDefault);
		this.tab_Address.Controls.Add(this.grd_Address);
		this.tab_Address.EditControl = null;
		this.tab_Address.EditValue = "Address";
		this.tab_Address.FieldName = "tab_Address";
		this.tab_Address.IsSearchControl = false;
		this.tab_Address.Name = "tab_Address";
		this.tab_Address.ReadOnly = false;
		this.tab_Address.SettingInVisible = false;
		this.tab_Address.SettingReadonly = false;
		this.tab_Address.Size = new System.Drawing.Size(871, 399);
		this.tab_Address.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Address.Text = "Address";
		this.btn_AddressDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AddressDefault.EditControl = null;
		this.btn_AddressDefault.EditValue = "Set as Defa&ult";
		this.btn_AddressDefault.FieldName = null;
		this.btn_AddressDefault.IsSearchControl = false;
		this.btn_AddressDefault.Location = new System.Drawing.Point(5, 369);
		this.btn_AddressDefault.Name = "btn_AddressDefault";
		this.btn_AddressDefault.ReadOnly = false;
		this.btn_AddressDefault.SettingInVisible = false;
		this.btn_AddressDefault.SettingReadonly = false;
		this.btn_AddressDefault.Size = new System.Drawing.Size(127, 23);
		this.btn_AddressDefault.TabIndex = 1;
		this.btn_AddressDefault.Text = "Set as Defa&ult";
		this.btn_AddressDefault.Click += new System.EventHandler(this.btn_AddressDefault_Click);
		this.grd_Address.AllowPasteExcel = true;
		this.grd_Address.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Address.ClipboardData = null;
		this.grd_Address.ContextMenuStrip = this.contextMenuStrip_Address;
		this.grd_Address.Location = new System.Drawing.Point(0, 0);
		this.grd_Address.MainView = this.gv_Address;
		this.grd_Address.Name = "grd_Address";
		this.grd_Address.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_Address_AdresType, this.rep_Address_Country, this.rep_States });
		this.grd_Address.Size = new System.Drawing.Size(869, 363);
		this.grd_Address.TabIndex = 0;
		this.grd_Address.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Address });
		this.contextMenuStrip_Address.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.dupplicateToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip_Address.Name = "contextMenuStrip_Address";
		this.contextMenuStrip_Address.Size = new System.Drawing.Size(151, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.dupplicateToolStripMenuItem.Name = "dupplicateToolStripMenuItem";
		this.dupplicateToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.dupplicateToolStripMenuItem.Text = "Duplicate Row";
		this.dupplicateToolStripMenuItem.Click += new System.EventHandler(this.dupplicateToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Address.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Address.ColumnKey = null;
		this.gv_Address.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gc_Address_Address, this.gc_Address_CardCode, this.gc_Address_AdresType, this.gc_Address_Street, this.gc_Address_StreetNo, this.gc_Address_Block, this.gc_Address_City, this.gc_Address_ZipCode, this.gc_Address_County, this.gc_Address_State,
			this.gc_Address_Country, this.gc_Address_Building, this.gc_Address_GlblLocNum
		});
		this.gv_Address.DisableFormSetting = false;
		this.gv_Address.DisplayException = false;
		this.gv_Address.EditColumn = null;
		this.gv_Address.FieldCurrency = null;
		this.gv_Address.FieldRate = null;
		this.gv_Address.FormId = "oBussinessPartnerMasterData";
		this.gv_Address.GridControl = this.grd_Address;
		this.gv_Address.HasFormSetting = false;
		this.gv_Address.IgnoreFilter = false;
		this.gv_Address.IndexEditRow = -1;
		this.gv_Address.IndicatorWidth = 35;
		this.gv_Address.IsApzTable = false;
		this.gv_Address.IsInsertNewRow = false;
		this.gv_Address.IsLoadingData = false;
		this.gv_Address.IsMappingContextAction = false;
		this.gv_Address.ItemId = "OADDRESS";
		this.gv_Address.LstColumnAllowBatch = null;
		this.gv_Address.LstColumnNotAllowBatch = null;
		this.gv_Address.LstColumnNotEditable = null;
		this.gv_Address.LstColumNotSetting = null;
		this.gv_Address.LstOldUDF = null;
		this.gv_Address.Name = "gv_Address";
		this.gv_Address.OptionsCustomization.AllowSort = false;
		this.gv_Address.OptionsFind.AllowFindPanel = false;
		this.gv_Address.OptionsNavigation.UseTabKey = false;
		this.gv_Address.OptionsView.ColumnAutoWidth = false;
		this.gv_Address.OptionsView.ShowGroupPanel = false;
		this.gv_Address.UseBatchMod = false;
		this.gv_Address.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Address_RowStyle);
		this.gv_Address.ShownEditor += new System.EventHandler(this.gv_Address_ShownEditor);
		this.gv_Address.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Address_CellValueChanged);
		this.gc_Address_Address.Caption = "Address";
		this.gc_Address_Address.CurrencyDateField = null;
		this.gc_Address_Address.CurrencyField = null;
		this.gc_Address_Address.CurrencyFixed = null;
		this.gc_Address_Address.CurrencyFixedRate = 0.0;
		this.gc_Address_Address.CurrencyRateField = null;
		this.gc_Address_Address.FieldName = "Address";
		this.gc_Address_Address.Name = "gc_Address_Address";
		this.gc_Address_Address.PrimaryKey = true;
		this.gc_Address_Address.UnitField = null;
		this.gc_Address_Address.Visible = true;
		this.gc_Address_Address.VisibleIndex = 0;
		this.gc_Address_CardCode.Caption = "CardCode";
		this.gc_Address_CardCode.CurrencyDateField = null;
		this.gc_Address_CardCode.CurrencyField = null;
		this.gc_Address_CardCode.CurrencyFixed = null;
		this.gc_Address_CardCode.CurrencyFixedRate = 0.0;
		this.gc_Address_CardCode.CurrencyRateField = null;
		this.gc_Address_CardCode.FieldName = "CardCode";
		this.gc_Address_CardCode.Name = "gc_Address_CardCode";
		this.gc_Address_CardCode.PrimaryKey = false;
		this.gc_Address_CardCode.UnitField = null;
		this.gc_Address_AdresType.Caption = "Adress Type";
		this.gc_Address_AdresType.ColumnEdit = this.rep_Address_AdresType;
		this.gc_Address_AdresType.CurrencyDateField = null;
		this.gc_Address_AdresType.CurrencyField = null;
		this.gc_Address_AdresType.CurrencyFixed = null;
		this.gc_Address_AdresType.CurrencyFixedRate = 0.0;
		this.gc_Address_AdresType.CurrencyRateField = null;
		this.gc_Address_AdresType.FieldName = "AdresType";
		this.gc_Address_AdresType.Name = "gc_Address_AdresType";
		this.gc_Address_AdresType.PrimaryKey = false;
		this.gc_Address_AdresType.UnitField = null;
		this.gc_Address_AdresType.Visible = true;
		this.gc_Address_AdresType.VisibleIndex = 1;
		this.rep_Address_AdresType.AutoHeight = false;
		this.rep_Address_AdresType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Address_AdresType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Address_AdresType.DisplayMember = "Name";
		this.rep_Address_AdresType.Name = "rep_Address_AdresType";
		this.rep_Address_AdresType.NullText = "";
		this.rep_Address_AdresType.ValueMember = "Code";
		this.gc_Address_Street.Caption = "Street";
		this.gc_Address_Street.CurrencyDateField = null;
		this.gc_Address_Street.CurrencyField = null;
		this.gc_Address_Street.CurrencyFixed = null;
		this.gc_Address_Street.CurrencyFixedRate = 0.0;
		this.gc_Address_Street.CurrencyRateField = null;
		this.gc_Address_Street.FieldName = "Street";
		this.gc_Address_Street.Name = "gc_Address_Street";
		this.gc_Address_Street.PrimaryKey = false;
		this.gc_Address_Street.UnitField = null;
		this.gc_Address_Street.Visible = true;
		this.gc_Address_Street.VisibleIndex = 2;
		this.gc_Address_StreetNo.Caption = "Street No.";
		this.gc_Address_StreetNo.CurrencyDateField = null;
		this.gc_Address_StreetNo.CurrencyField = null;
		this.gc_Address_StreetNo.CurrencyFixed = null;
		this.gc_Address_StreetNo.CurrencyFixedRate = 0.0;
		this.gc_Address_StreetNo.CurrencyRateField = null;
		this.gc_Address_StreetNo.FieldName = "StreetNo";
		this.gc_Address_StreetNo.Name = "gc_Address_StreetNo";
		this.gc_Address_StreetNo.PrimaryKey = false;
		this.gc_Address_StreetNo.UnitField = null;
		this.gc_Address_StreetNo.Visible = true;
		this.gc_Address_StreetNo.VisibleIndex = 3;
		this.gc_Address_Block.Caption = "Block";
		this.gc_Address_Block.CurrencyDateField = null;
		this.gc_Address_Block.CurrencyField = null;
		this.gc_Address_Block.CurrencyFixed = null;
		this.gc_Address_Block.CurrencyFixedRate = 0.0;
		this.gc_Address_Block.CurrencyRateField = null;
		this.gc_Address_Block.FieldName = "Block";
		this.gc_Address_Block.Name = "gc_Address_Block";
		this.gc_Address_Block.PrimaryKey = false;
		this.gc_Address_Block.UnitField = null;
		this.gc_Address_Block.Visible = true;
		this.gc_Address_Block.VisibleIndex = 4;
		this.gc_Address_City.Caption = "City";
		this.gc_Address_City.CurrencyDateField = null;
		this.gc_Address_City.CurrencyField = null;
		this.gc_Address_City.CurrencyFixed = null;
		this.gc_Address_City.CurrencyFixedRate = 0.0;
		this.gc_Address_City.CurrencyRateField = null;
		this.gc_Address_City.FieldName = "City";
		this.gc_Address_City.Name = "gc_Address_City";
		this.gc_Address_City.PrimaryKey = false;
		this.gc_Address_City.UnitField = null;
		this.gc_Address_City.Visible = true;
		this.gc_Address_City.VisibleIndex = 5;
		this.gc_Address_ZipCode.Caption = "Zip Code";
		this.gc_Address_ZipCode.CurrencyDateField = null;
		this.gc_Address_ZipCode.CurrencyField = null;
		this.gc_Address_ZipCode.CurrencyFixed = null;
		this.gc_Address_ZipCode.CurrencyFixedRate = 0.0;
		this.gc_Address_ZipCode.CurrencyRateField = null;
		this.gc_Address_ZipCode.FieldName = "ZipCode";
		this.gc_Address_ZipCode.Name = "gc_Address_ZipCode";
		this.gc_Address_ZipCode.PrimaryKey = false;
		this.gc_Address_ZipCode.UnitField = null;
		this.gc_Address_ZipCode.Visible = true;
		this.gc_Address_ZipCode.VisibleIndex = 6;
		this.gc_Address_County.Caption = "County";
		this.gc_Address_County.CurrencyDateField = null;
		this.gc_Address_County.CurrencyField = null;
		this.gc_Address_County.CurrencyFixed = null;
		this.gc_Address_County.CurrencyFixedRate = 0.0;
		this.gc_Address_County.CurrencyRateField = null;
		this.gc_Address_County.FieldName = "County";
		this.gc_Address_County.Name = "gc_Address_County";
		this.gc_Address_County.PrimaryKey = false;
		this.gc_Address_County.UnitField = null;
		this.gc_Address_County.Visible = true;
		this.gc_Address_County.VisibleIndex = 7;
		this.gc_Address_State.Caption = "State";
		this.gc_Address_State.ColumnEdit = this.rep_States;
		this.gc_Address_State.CurrencyDateField = null;
		this.gc_Address_State.CurrencyField = null;
		this.gc_Address_State.CurrencyFixed = null;
		this.gc_Address_State.CurrencyFixedRate = 0.0;
		this.gc_Address_State.CurrencyRateField = null;
		this.gc_Address_State.FieldName = "State";
		this.gc_Address_State.Name = "gc_Address_State";
		this.gc_Address_State.PrimaryKey = false;
		this.gc_Address_State.UnitField = null;
		this.gc_Address_State.Visible = true;
		this.gc_Address_State.VisibleIndex = 8;
		this.rep_States.AutoHeight = false;
		this.rep_States.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_States.Name = "rep_States";
		this.rep_States.NullText = "";
		this.rep_States.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_States_EditValueChanging);
		this.gc_Address_Country.Caption = "Country";
		this.gc_Address_Country.ColumnEdit = this.rep_Address_Country;
		this.gc_Address_Country.CurrencyDateField = null;
		this.gc_Address_Country.CurrencyField = null;
		this.gc_Address_Country.CurrencyFixed = null;
		this.gc_Address_Country.CurrencyFixedRate = 0.0;
		this.gc_Address_Country.CurrencyRateField = null;
		this.gc_Address_Country.FieldName = "Country";
		this.gc_Address_Country.Name = "gc_Address_Country";
		this.gc_Address_Country.PrimaryKey = false;
		this.gc_Address_Country.UnitField = null;
		this.gc_Address_Country.Visible = true;
		this.gc_Address_Country.VisibleIndex = 9;
		this.rep_Address_Country.AutoHeight = false;
		this.rep_Address_Country.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Address_Country.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Address_Country.DisplayMember = "Name";
		this.rep_Address_Country.Name = "rep_Address_Country";
		this.rep_Address_Country.NullText = "";
		this.rep_Address_Country.ValueMember = "Code";
		this.gc_Address_Building.Caption = "Building/Floor/Room";
		this.gc_Address_Building.CurrencyDateField = null;
		this.gc_Address_Building.CurrencyField = null;
		this.gc_Address_Building.CurrencyFixed = null;
		this.gc_Address_Building.CurrencyFixedRate = 0.0;
		this.gc_Address_Building.CurrencyRateField = null;
		this.gc_Address_Building.FieldName = "Building";
		this.gc_Address_Building.Name = "gc_Address_Building";
		this.gc_Address_Building.PrimaryKey = false;
		this.gc_Address_Building.UnitField = null;
		this.gc_Address_Building.Visible = true;
		this.gc_Address_Building.VisibleIndex = 10;
		this.gc_Address_GlblLocNum.Caption = "GLN";
		this.gc_Address_GlblLocNum.CurrencyDateField = null;
		this.gc_Address_GlblLocNum.CurrencyField = null;
		this.gc_Address_GlblLocNum.CurrencyFixed = null;
		this.gc_Address_GlblLocNum.CurrencyFixedRate = 0.0;
		this.gc_Address_GlblLocNum.CurrencyRateField = null;
		this.gc_Address_GlblLocNum.FieldName = "GlblLocNum";
		this.gc_Address_GlblLocNum.Name = "gc_Address_GlblLocNum";
		this.gc_Address_GlblLocNum.PrimaryKey = false;
		this.gc_Address_GlblLocNum.UnitField = null;
		this.gc_Address_GlblLocNum.Visible = true;
		this.gc_Address_GlblLocNum.VisibleIndex = 11;
		this.tab_PaymentTerms.Controls.Add(this.labelLine29);
		this.tab_PaymentTerms.Controls.Add(this.panelControl1);
		this.tab_PaymentTerms.Controls.Add(this.luk_EffectiveDiscount);
		this.tab_PaymentTerms.Controls.Add(this.labelLine11);
		this.tab_PaymentTerms.Controls.Add(this.btn_PaymentDates);
		this.tab_PaymentTerms.Controls.Add(this.txt_PaymentDefaultIBAN);
		this.tab_PaymentTerms.Controls.Add(this.luk_PaymentTermPriority);
		this.tab_PaymentTerms.Controls.Add(this.luk_CreditCardType);
		this.tab_PaymentTerms.Controls.Add(this.datePaymentExpirationDate);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentAvarageDelay);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentIDNumber);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentCreditCardNo);
		this.tab_PaymentTerms.Controls.Add(this.labelLine10);
		this.tab_PaymentTerms.Controls.Add(this.labelLine9);
		this.tab_PaymentTerms.Controls.Add(this.labelLine4);
		this.tab_PaymentTerms.Controls.Add(this.chk_AcceptsEndorsedChecks);
		this.tab_PaymentTerms.Controls.Add(this.chk_EndorsableChecksFrom);
		this.tab_PaymentTerms.Controls.Add(this.chk_DoNotApplyDiscountGroup);
		this.tab_PaymentTerms.Controls.Add(this.chk_AllowPartialDlvrPerRow);
		this.tab_PaymentTerms.Controls.Add(this.chk_AllowPartialDlvrOfSO);
		this.tab_PaymentTerms.Controls.Add(this.luk_PaymentPriceList);
		this.tab_PaymentTerms.Controls.Add(this.luk_PaymentTerm);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentTotalDiscount);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentInterestonArrears);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentCreditLimit);
		this.tab_PaymentTerms.Controls.Add(this.txtPaymentCommitmentLimit);
		this.tab_PaymentTerms.Controls.Add(this.lb_Commitment);
		this.tab_PaymentTerms.Controls.Add(this.lb_CreditL);
		this.tab_PaymentTerms.Controls.Add(this.lb_Interest);
		this.tab_PaymentTerms.Controls.Add(this.lb_TotalDis);
		this.tab_PaymentTerms.Controls.Add(this.lb_PriceLst);
		this.tab_PaymentTerms.Controls.Add(this.lb_PaymentTerms);
		this.tab_PaymentTerms.Controls.Add(this.lb_AvarageDelay);
		this.tab_PaymentTerms.Controls.Add(this.lb_IDNumber);
		this.tab_PaymentTerms.Controls.Add(this.lb_ExpirationDate);
		this.tab_PaymentTerms.Controls.Add(this.lb_CreditCardNo);
		this.tab_PaymentTerms.Controls.Add(this.lb_CreditCardType);
		this.tab_PaymentTerms.EditControl = null;
		this.tab_PaymentTerms.EditValue = "&Payment Terms";
		this.tab_PaymentTerms.FieldName = "tab_PaymentTerms";
		this.tab_PaymentTerms.IsSearchControl = false;
		this.tab_PaymentTerms.Name = "tab_PaymentTerms";
		this.tab_PaymentTerms.ReadOnly = false;
		this.tab_PaymentTerms.SettingInVisible = false;
		this.tab_PaymentTerms.SettingReadonly = false;
		this.tab_PaymentTerms.Size = new System.Drawing.Size(871, 399);
		this.tab_PaymentTerms.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_PaymentTerms.Text = "&Payment Terms";
		this.labelLine29.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine29.Appearance.Options.UseBackColor = true;
		this.labelLine29.EditControl = null;
		this.labelLine29.EditValue = "Business Partner Bank";
		this.labelLine29.FieldName = null;
		this.labelLine29.HideWhenEmpty = false;
		this.labelLine29.LineColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.labelLine29.Location = new System.Drawing.Point(9, 189);
		this.labelLine29.Name = "labelLine29";
		this.labelLine29.ReadOnly = false;
		this.labelLine29.SettingInVisible = false;
		this.labelLine29.SettingReadonly = false;
		this.labelLine29.ShowUnderLine = true;
		this.labelLine29.Size = new System.Drawing.Size(111, 20);
		this.labelLine29.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine29.TabIndex = 94;
		this.labelLine29.TabStop = false;
		this.labelLine29.Utext = "Business Partner Bank";
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.Controls.Add(this.txt_DflSwift);
		this.panelControl1.Controls.Add(this.txt_BPBankCode);
		this.panelControl1.Controls.Add(this.txt_BPBankAccount);
		this.panelControl1.Controls.Add(this.txt_BPBankAccountName);
		this.panelControl1.Controls.Add(this.bte_BPBankCountry);
		this.panelControl1.Controls.Add(this.txt_BPBankBranch);
		this.panelControl1.Controls.Add(this.luk_BPBankName);
		this.panelControl1.Controls.Add(this.txt_BPBankMandateID);
		this.panelControl1.Controls.Add(this.txt_BPBankControlKey);
		this.panelControl1.Controls.Add(this.txt_BPBankIBAN);
		this.panelControl1.Controls.Add(this.txt_BPBankDateOfSignature);
		this.panelControl1.Controls.Add(this.lb_BankCountry);
		this.panelControl1.Controls.Add(this.lb_DflSwift);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.labelLine6);
		this.panelControl1.Controls.Add(this.labelLine7);
		this.panelControl1.Controls.Add(this.lb_BankCode);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Controls.Add(this.lb_Account);
		this.panelControl1.Controls.Add(this.lb_BankName);
		this.panelControl1.Location = new System.Drawing.Point(10, 209);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(521, 145);
		this.panelControl1.TabIndex = 93;
		this.txt_DflSwift.EditControl = null;
		this.txt_DflSwift.FieldName = "DflSwift";
		this.txt_DflSwift.IsSearchControl = false;
		this.txt_DflSwift.Location = new System.Drawing.Point(109, 80);
		this.txt_DflSwift.Name = "txt_DflSwift";
		this.txt_DflSwift.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_DflSwift.Properties.Appearance.Options.UseFont = true;
		this.txt_DflSwift.Properties.AutoHeight = false;
		this.txt_DflSwift.Properties.ReadOnly = true;
		this.txt_DflSwift.SettingInVisible = false;
		this.txt_DflSwift.SettingReadonly = false;
		this.txt_DflSwift.Size = new System.Drawing.Size(141, 18);
		this.txt_DflSwift.TabIndex = 4;
		this.txt_DflSwift.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankCode.EditControl = null;
		this.txt_BPBankCode.FieldName = "BankCode";
		this.txt_BPBankCode.IsSearchControl = false;
		this.txt_BPBankCode.Location = new System.Drawing.Point(109, 42);
		this.txt_BPBankCode.Name = "txt_BPBankCode";
		this.txt_BPBankCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankCode.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankCode.Properties.AutoHeight = false;
		this.txt_BPBankCode.Properties.ReadOnly = true;
		this.txt_BPBankCode.SettingInVisible = false;
		this.txt_BPBankCode.SettingReadonly = false;
		this.txt_BPBankCode.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankCode.TabIndex = 2;
		this.txt_BPBankCode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankAccount.EditControl = null;
		this.txt_BPBankAccount.FieldName = "DflAccount";
		this.txt_BPBankAccount.IsSearchControl = false;
		this.txt_BPBankAccount.Location = new System.Drawing.Point(109, 61);
		this.txt_BPBankAccount.Name = "txt_BPBankAccount";
		this.txt_BPBankAccount.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankAccount.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankAccount.Properties.AutoHeight = false;
		this.txt_BPBankAccount.Properties.ReadOnly = true;
		this.txt_BPBankAccount.SettingInVisible = false;
		this.txt_BPBankAccount.SettingReadonly = false;
		this.txt_BPBankAccount.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankAccount.TabIndex = 3;
		this.txt_BPBankAccount.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankAccountName.EditControl = null;
		this.txt_BPBankAccountName.FieldName = "BankAccountName_D";
		this.txt_BPBankAccountName.IsSearchControl = false;
		this.txt_BPBankAccountName.Location = new System.Drawing.Point(109, 99);
		this.txt_BPBankAccountName.Name = "txt_BPBankAccountName";
		this.txt_BPBankAccountName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankAccountName.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankAccountName.Properties.AutoHeight = false;
		this.txt_BPBankAccountName.Properties.ReadOnly = true;
		this.txt_BPBankAccountName.SettingInVisible = false;
		this.txt_BPBankAccountName.SettingReadonly = false;
		this.txt_BPBankAccountName.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankAccountName.TabIndex = 5;
		this.txt_BPBankAccountName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bte_BPBankCountry.EditControl = null;
		this.bte_BPBankCountry.FieldName = "BankCountr";
		this.bte_BPBankCountry.IsSearchControl = false;
		this.bte_BPBankCountry.Location = new System.Drawing.Point(109, 4);
		this.bte_BPBankCountry.Name = "bte_BPBankCountry";
		this.bte_BPBankCountry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_BPBankCountry.Properties.Appearance.Options.UseFont = true;
		this.bte_BPBankCountry.Properties.AutoHeight = false;
		this.bte_BPBankCountry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPBankCountry.Properties.DisplayMember = "Name";
		this.bte_BPBankCountry.Properties.IsSearchControl = false;
		this.bte_BPBankCountry.Properties.NullText = "";
		this.bte_BPBankCountry.Properties.ValueMember = "Code";
		this.bte_BPBankCountry.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPBankCountry_ButtonPressed);
		this.bte_BPBankCountry.SettingInVisible = false;
		this.bte_BPBankCountry.SettingReadonly = false;
		this.bte_BPBankCountry.Size = new System.Drawing.Size(141, 18);
		this.bte_BPBankCountry.TabIndex = 0;
		this.bte_BPBankCountry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.bte_BPBankCountry_QueryPopUp);
		this.bte_BPBankCountry.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankBranch.EditControl = null;
		this.txt_BPBankBranch.FieldName = "DflBranch";
		this.txt_BPBankBranch.IsSearchControl = false;
		this.txt_BPBankBranch.Location = new System.Drawing.Point(109, 118);
		this.txt_BPBankBranch.Name = "txt_BPBankBranch";
		this.txt_BPBankBranch.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankBranch.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankBranch.Properties.AutoHeight = false;
		this.txt_BPBankBranch.Properties.ReadOnly = true;
		this.txt_BPBankBranch.SettingInVisible = false;
		this.txt_BPBankBranch.SettingReadonly = false;
		this.txt_BPBankBranch.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankBranch.TabIndex = 6;
		this.txt_BPBankBranch.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_BPBankName.EditControl = null;
		this.luk_BPBankName.FieldName = "BankCode_D";
		this.luk_BPBankName.IsSearchControl = false;
		this.luk_BPBankName.Location = new System.Drawing.Point(109, 23);
		this.luk_BPBankName.Name = "luk_BPBankName";
		this.luk_BPBankName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_BPBankName.Properties.Appearance.Options.UseFont = true;
		this.luk_BPBankName.Properties.AutoHeight = false;
		this.luk_BPBankName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BPBankName.Properties.DisplayMember = "BankName";
		this.luk_BPBankName.Properties.IsSearchControl = false;
		this.luk_BPBankName.Properties.NullText = "";
		this.luk_BPBankName.Properties.ReadOnly = true;
		this.luk_BPBankName.Properties.ValueMember = "BankCode";
		this.luk_BPBankName.SettingInVisible = false;
		this.luk_BPBankName.SettingReadonly = false;
		this.luk_BPBankName.Size = new System.Drawing.Size(141, 18);
		this.luk_BPBankName.TabIndex = 1;
		this.luk_BPBankName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankMandateID.EditControl = null;
		this.txt_BPBankMandateID.FieldName = "MandateID";
		this.txt_BPBankMandateID.IsSearchControl = false;
		this.txt_BPBankMandateID.Location = new System.Drawing.Point(369, 42);
		this.txt_BPBankMandateID.Name = "txt_BPBankMandateID";
		this.txt_BPBankMandateID.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankMandateID.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankMandateID.Properties.AutoHeight = false;
		this.txt_BPBankMandateID.Properties.ReadOnly = true;
		this.txt_BPBankMandateID.SettingInVisible = false;
		this.txt_BPBankMandateID.SettingReadonly = false;
		this.txt_BPBankMandateID.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankMandateID.TabIndex = 9;
		this.txt_BPBankMandateID.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankControlKey.EditControl = null;
		this.txt_BPBankControlKey.FieldName = "BankCtlKey";
		this.txt_BPBankControlKey.IsSearchControl = false;
		this.txt_BPBankControlKey.Location = new System.Drawing.Point(369, 4);
		this.txt_BPBankControlKey.Name = "txt_BPBankControlKey";
		this.txt_BPBankControlKey.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankControlKey.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankControlKey.Properties.AutoHeight = false;
		this.txt_BPBankControlKey.Properties.ReadOnly = true;
		this.txt_BPBankControlKey.SettingInVisible = false;
		this.txt_BPBankControlKey.SettingReadonly = false;
		this.txt_BPBankControlKey.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankControlKey.TabIndex = 7;
		this.txt_BPBankControlKey.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankIBAN.EditControl = null;
		this.txt_BPBankIBAN.FieldName = "DflIBAN";
		this.txt_BPBankIBAN.IsSearchControl = false;
		this.txt_BPBankIBAN.Location = new System.Drawing.Point(369, 23);
		this.txt_BPBankIBAN.Name = "txt_BPBankIBAN";
		this.txt_BPBankIBAN.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankIBAN.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankIBAN.Properties.AutoHeight = false;
		this.txt_BPBankIBAN.Properties.ReadOnly = true;
		this.txt_BPBankIBAN.SettingInVisible = false;
		this.txt_BPBankIBAN.SettingReadonly = false;
		this.txt_BPBankIBAN.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankIBAN.TabIndex = 8;
		this.txt_BPBankIBAN.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_BPBankDateOfSignature.EditControl = null;
		this.txt_BPBankDateOfSignature.EditValue = null;
		this.txt_BPBankDateOfSignature.FieldName = "SignDate";
		this.txt_BPBankDateOfSignature.Location = new System.Drawing.Point(369, 61);
		this.txt_BPBankDateOfSignature.Name = "txt_BPBankDateOfSignature";
		this.txt_BPBankDateOfSignature.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_BPBankDateOfSignature.Properties.Appearance.Options.UseFont = true;
		this.txt_BPBankDateOfSignature.Properties.AutoHeight = false;
		this.txt_BPBankDateOfSignature.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_BPBankDateOfSignature.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_BPBankDateOfSignature.Properties.IsSearchControl = false;
		this.txt_BPBankDateOfSignature.Properties.ReadOnly = true;
		this.txt_BPBankDateOfSignature.SettingInVisible = false;
		this.txt_BPBankDateOfSignature.SettingReadonly = false;
		this.txt_BPBankDateOfSignature.Size = new System.Drawing.Size(141, 18);
		this.txt_BPBankDateOfSignature.TabIndex = 10;
		this.txt_BPBankDateOfSignature.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_BankCountry.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_BankCountry.Appearance.Options.UseBackColor = true;
		this.lb_BankCountry.EditControl = null;
		this.lb_BankCountry.EditValue = "Bank Country";
		this.lb_BankCountry.FieldName = null;
		this.lb_BankCountry.HideWhenEmpty = false;
		this.lb_BankCountry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BankCountry.Location = new System.Drawing.Point(5, 5);
		this.lb_BankCountry.Name = "lb_BankCountry";
		this.lb_BankCountry.ReadOnly = false;
		this.lb_BankCountry.SettingInVisible = false;
		this.lb_BankCountry.SettingReadonly = false;
		this.lb_BankCountry.ShowUnderLine = true;
		this.lb_BankCountry.Size = new System.Drawing.Size(105, 20);
		this.lb_BankCountry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BankCountry.TabIndex = 49;
		this.lb_BankCountry.TabStop = false;
		this.lb_BankCountry.Utext = "Bank Country";
		this.lb_DflSwift.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_DflSwift.Appearance.Options.UseBackColor = true;
		this.lb_DflSwift.EditControl = null;
		this.lb_DflSwift.EditValue = "BIC/SWIFT Code";
		this.lb_DflSwift.FieldName = null;
		this.lb_DflSwift.HideWhenEmpty = false;
		this.lb_DflSwift.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DflSwift.Location = new System.Drawing.Point(5, 81);
		this.lb_DflSwift.Name = "lb_DflSwift";
		this.lb_DflSwift.ReadOnly = false;
		this.lb_DflSwift.SettingInVisible = false;
		this.lb_DflSwift.SettingReadonly = false;
		this.lb_DflSwift.ShowUnderLine = true;
		this.lb_DflSwift.Size = new System.Drawing.Size(105, 20);
		this.lb_DflSwift.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DflSwift.TabIndex = 61;
		this.lb_DflSwift.TabStop = false;
		this.lb_DflSwift.Utext = "BIC/SWIFT Code";
		this.labelLine3.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine3.Appearance.Options.UseBackColor = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "IBAN";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(265, 24);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(105, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 79;
		this.labelLine3.TabStop = false;
		this.labelLine3.Utext = "IBAN";
		this.labelLine5.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine5.Appearance.Options.UseBackColor = true;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Ctrl Int. ID";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(265, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(105, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 80;
		this.labelLine5.TabStop = false;
		this.labelLine5.Utext = "Ctrl Int. ID";
		this.labelLine2.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine2.Appearance.Options.UseBackColor = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Bank Account Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(5, 100);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(105, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 74;
		this.labelLine2.TabStop = false;
		this.labelLine2.Utext = "Bank Account Name";
		this.labelLine6.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine6.Appearance.Options.UseBackColor = true;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Date of Signature";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(265, 62);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(105, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 84;
		this.labelLine6.TabStop = false;
		this.labelLine6.Utext = "Date of Signature";
		this.labelLine7.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine7.Appearance.Options.UseBackColor = true;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Mandate ID";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(265, 43);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(105, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 81;
		this.labelLine7.TabStop = false;
		this.labelLine7.Utext = "Mandate ID";
		this.lb_BankCode.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_BankCode.Appearance.Options.UseBackColor = true;
		this.lb_BankCode.EditControl = null;
		this.lb_BankCode.EditValue = "Bank Code";
		this.lb_BankCode.FieldName = null;
		this.lb_BankCode.HideWhenEmpty = false;
		this.lb_BankCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BankCode.Location = new System.Drawing.Point(5, 43);
		this.lb_BankCode.Name = "lb_BankCode";
		this.lb_BankCode.ReadOnly = false;
		this.lb_BankCode.SettingInVisible = false;
		this.lb_BankCode.SettingReadonly = false;
		this.lb_BankCode.ShowUnderLine = true;
		this.lb_BankCode.Size = new System.Drawing.Size(105, 20);
		this.lb_BankCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BankCode.TabIndex = 57;
		this.lb_BankCode.TabStop = false;
		this.lb_BankCode.Utext = "Bank Code";
		this.labelLine1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine1.Appearance.Options.UseBackColor = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Branch";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 119);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(105, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 75;
		this.labelLine1.TabStop = false;
		this.labelLine1.Utext = "Branch";
		this.lb_Account.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Account.Appearance.Options.UseBackColor = true;
		this.lb_Account.EditControl = null;
		this.lb_Account.EditValue = "Account";
		this.lb_Account.FieldName = null;
		this.lb_Account.HideWhenEmpty = false;
		this.lb_Account.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Account.Location = new System.Drawing.Point(5, 62);
		this.lb_Account.Name = "lb_Account";
		this.lb_Account.ReadOnly = false;
		this.lb_Account.SettingInVisible = false;
		this.lb_Account.SettingReadonly = false;
		this.lb_Account.ShowUnderLine = true;
		this.lb_Account.Size = new System.Drawing.Size(105, 20);
		this.lb_Account.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Account.TabIndex = 56;
		this.lb_Account.TabStop = false;
		this.lb_Account.Utext = "Account";
		this.lb_BankName.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_BankName.Appearance.Options.UseBackColor = true;
		this.lb_BankName.EditControl = null;
		this.lb_BankName.EditValue = "Bank Name";
		this.lb_BankName.FieldName = null;
		this.lb_BankName.HideWhenEmpty = false;
		this.lb_BankName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BankName.Location = new System.Drawing.Point(5, 24);
		this.lb_BankName.Name = "lb_BankName";
		this.lb_BankName.ReadOnly = false;
		this.lb_BankName.SettingInVisible = false;
		this.lb_BankName.SettingReadonly = false;
		this.lb_BankName.ShowUnderLine = true;
		this.lb_BankName.Size = new System.Drawing.Size(105, 20);
		this.lb_BankName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BankName.TabIndex = 55;
		this.lb_BankName.TabStop = false;
		this.lb_BankName.Utext = "Bank Name";
		this.luk_EffectiveDiscount.EditControl = null;
		this.luk_EffectiveDiscount.FieldName = "DiscRel";
		this.luk_EffectiveDiscount.IsSearchControl = false;
		this.luk_EffectiveDiscount.Location = new System.Drawing.Point(148, 143);
		this.luk_EffectiveDiscount.Name = "luk_EffectiveDiscount";
		this.luk_EffectiveDiscount.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_EffectiveDiscount.Properties.Appearance.Options.UseFont = true;
		this.luk_EffectiveDiscount.Properties.AutoHeight = false;
		this.luk_EffectiveDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_EffectiveDiscount.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_EffectiveDiscount.Properties.DisplayMember = "Name";
		this.luk_EffectiveDiscount.Properties.IsSearchControl = false;
		this.luk_EffectiveDiscount.Properties.NullText = "";
		this.luk_EffectiveDiscount.Properties.ValueMember = "Code";
		this.luk_EffectiveDiscount.SettingInVisible = false;
		this.luk_EffectiveDiscount.SettingReadonly = false;
		this.luk_EffectiveDiscount.Size = new System.Drawing.Size(207, 18);
		this.luk_EffectiveDiscount.TabIndex = 6;
		this.luk_EffectiveDiscount.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine11.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine11.Appearance.Options.UseBackColor = true;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Effective Discount";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(10, 146);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(140, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 92;
		this.labelLine11.TabStop = false;
		this.labelLine11.Utext = "Effective Discount";
		this.btn_PaymentDates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PaymentDates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_PaymentDates.EditControl = null;
		this.btn_PaymentDates.EditValue = "...";
		this.btn_PaymentDates.FieldName = null;
		this.btn_PaymentDates.IsSearchControl = false;
		this.btn_PaymentDates.Location = new System.Drawing.Point(640, 138);
		this.btn_PaymentDates.Name = "btn_PaymentDates";
		this.btn_PaymentDates.ReadOnly = false;
		this.btn_PaymentDates.SettingInVisible = false;
		this.btn_PaymentDates.SettingReadonly = false;
		this.btn_PaymentDates.Size = new System.Drawing.Size(29, 20);
		this.btn_PaymentDates.TabIndex = 14;
		this.btn_PaymentDates.Text = "...";
		this.btn_PaymentDates.Click += new System.EventHandler(this.btn_PaymentDates_Click);
		this.txt_PaymentDefaultIBAN.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_PaymentDefaultIBAN.EditControl = null;
		this.txt_PaymentDefaultIBAN.FieldName = "IBAN";
		this.txt_PaymentDefaultIBAN.IsSearchControl = false;
		this.txt_PaymentDefaultIBAN.Location = new System.Drawing.Point(640, 116);
		this.txt_PaymentDefaultIBAN.Name = "txt_PaymentDefaultIBAN";
		this.txt_PaymentDefaultIBAN.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_PaymentDefaultIBAN.Properties.Appearance.Options.UseFont = true;
		this.txt_PaymentDefaultIBAN.Properties.AutoHeight = false;
		this.txt_PaymentDefaultIBAN.SettingInVisible = false;
		this.txt_PaymentDefaultIBAN.SettingReadonly = false;
		this.txt_PaymentDefaultIBAN.Size = new System.Drawing.Size(207, 18);
		this.txt_PaymentDefaultIBAN.TabIndex = 13;
		this.txt_PaymentDefaultIBAN.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_PaymentTermPriority.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_PaymentTermPriority.EditControl = null;
		this.luk_PaymentTermPriority.FieldName = "Priority";
		this.luk_PaymentTermPriority.IsSearchControl = false;
		this.luk_PaymentTermPriority.Location = new System.Drawing.Point(640, 97);
		this.luk_PaymentTermPriority.Name = "luk_PaymentTermPriority";
		this.luk_PaymentTermPriority.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_PaymentTermPriority.Properties.Appearance.Options.UseFont = true;
		this.luk_PaymentTermPriority.Properties.AutoHeight = false;
		this.luk_PaymentTermPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PaymentTermPriority.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PrioDesc", "Name")
		});
		this.luk_PaymentTermPriority.Properties.DisplayMember = "PrioDesc";
		this.luk_PaymentTermPriority.Properties.IsSearchControl = false;
		this.luk_PaymentTermPriority.Properties.NullText = "";
		this.luk_PaymentTermPriority.Properties.ValueMember = "PrioCode";
		this.luk_PaymentTermPriority.SettingInVisible = false;
		this.luk_PaymentTermPriority.SettingReadonly = false;
		this.luk_PaymentTermPriority.Size = new System.Drawing.Size(207, 18);
		this.luk_PaymentTermPriority.TabIndex = 12;
		this.luk_PaymentTermPriority.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_CreditCardType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_CreditCardType.EditControl = null;
		this.luk_CreditCardType.FieldName = "CreditCard";
		this.luk_CreditCardType.IsSearchControl = false;
		this.luk_CreditCardType.Location = new System.Drawing.Point(640, 2);
		this.luk_CreditCardType.Name = "luk_CreditCardType";
		this.luk_CreditCardType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_CreditCardType.Properties.Appearance.Options.UseFont = true;
		this.luk_CreditCardType.Properties.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.luk_CreditCardType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_CreditCardType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_CreditCardType.Properties.DisplayMember = "Name";
		this.luk_CreditCardType.Properties.IsSearchControl = false;
		this.luk_CreditCardType.Properties.NullText = "";
		this.luk_CreditCardType.Properties.ValueMember = "Code";
		this.luk_CreditCardType.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.luk_CreditCardType_Properties_ButtonPressed);
		this.luk_CreditCardType.SettingInVisible = false;
		this.luk_CreditCardType.SettingReadonly = false;
		this.luk_CreditCardType.Size = new System.Drawing.Size(207, 18);
		this.luk_CreditCardType.TabIndex = 7;
		this.luk_CreditCardType.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.datePaymentExpirationDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.datePaymentExpirationDate.EditControl = null;
		this.datePaymentExpirationDate.EditValue = null;
		this.datePaymentExpirationDate.FieldName = "CardValid";
		this.datePaymentExpirationDate.Location = new System.Drawing.Point(640, 40);
		this.datePaymentExpirationDate.Name = "datePaymentExpirationDate";
		this.datePaymentExpirationDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.datePaymentExpirationDate.Properties.Appearance.Options.UseFont = true;
		this.datePaymentExpirationDate.Properties.AutoHeight = false;
		this.datePaymentExpirationDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePaymentExpirationDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePaymentExpirationDate.Properties.IsSearchControl = false;
		this.datePaymentExpirationDate.SettingInVisible = false;
		this.datePaymentExpirationDate.SettingReadonly = false;
		this.datePaymentExpirationDate.Size = new System.Drawing.Size(207, 18);
		this.datePaymentExpirationDate.TabIndex = 9;
		this.datePaymentExpirationDate.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentAvarageDelay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPaymentAvarageDelay.EditControl = null;
		this.txtPaymentAvarageDelay.FieldName = "AvrageLate";
		this.txtPaymentAvarageDelay.IsSearchControl = false;
		this.txtPaymentAvarageDelay.Location = new System.Drawing.Point(640, 78);
		this.txtPaymentAvarageDelay.Name = "txtPaymentAvarageDelay";
		this.txtPaymentAvarageDelay.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentAvarageDelay.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentAvarageDelay.Properties.AutoHeight = false;
		this.txtPaymentAvarageDelay.SettingInVisible = false;
		this.txtPaymentAvarageDelay.SettingReadonly = false;
		this.txtPaymentAvarageDelay.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentAvarageDelay.TabIndex = 11;
		this.txtPaymentAvarageDelay.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentIDNumber.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPaymentIDNumber.EditControl = null;
		this.txtPaymentIDNumber.FieldName = "OwnerIdNum";
		this.txtPaymentIDNumber.IsSearchControl = false;
		this.txtPaymentIDNumber.Location = new System.Drawing.Point(640, 59);
		this.txtPaymentIDNumber.Name = "txtPaymentIDNumber";
		this.txtPaymentIDNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentIDNumber.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentIDNumber.Properties.AutoHeight = false;
		this.txtPaymentIDNumber.SettingInVisible = false;
		this.txtPaymentIDNumber.SettingReadonly = false;
		this.txtPaymentIDNumber.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentIDNumber.TabIndex = 10;
		this.txtPaymentIDNumber.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentCreditCardNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPaymentCreditCardNo.EditControl = null;
		this.txtPaymentCreditCardNo.FieldName = "CrCardNum";
		this.txtPaymentCreditCardNo.IsSearchControl = false;
		this.txtPaymentCreditCardNo.Location = new System.Drawing.Point(640, 21);
		this.txtPaymentCreditCardNo.Name = "txtPaymentCreditCardNo";
		this.txtPaymentCreditCardNo.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentCreditCardNo.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentCreditCardNo.Properties.AutoHeight = false;
		this.txtPaymentCreditCardNo.SettingInVisible = false;
		this.txtPaymentCreditCardNo.SettingReadonly = false;
		this.txtPaymentCreditCardNo.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentCreditCardNo.TabIndex = 8;
		this.txtPaymentCreditCardNo.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine10.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine10.Appearance.Options.UseBackColor = true;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Payment Dates";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(503, 141);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(140, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 89;
		this.labelLine10.TabStop = false;
		this.labelLine10.Utext = "Payment Dates";
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine9.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine9.Appearance.Options.UseBackColor = true;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Default IBAN";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(502, 118);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(140, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 88;
		this.labelLine9.TabStop = false;
		this.labelLine9.Utext = "Default IBAN";
		this.labelLine4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine4.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine4.Appearance.Options.UseBackColor = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Priority";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(502, 99);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(140, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 84;
		this.labelLine4.TabStop = false;
		this.labelLine4.Utext = "Priority";
		this.chk_AcceptsEndorsedChecks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_AcceptsEndorsedChecks.EditControl = null;
		this.chk_AcceptsEndorsedChecks.FieldName = "EdrsToBP";
		this.chk_AcceptsEndorsedChecks.IsSearchControl = false;
		this.chk_AcceptsEndorsedChecks.Location = new System.Drawing.Point(600, 250);
		this.chk_AcceptsEndorsedChecks.Name = "chk_AcceptsEndorsedChecks";
		this.chk_AcceptsEndorsedChecks.Properties.Caption = "This &BP Accepts Endorsed Checks";
		this.chk_AcceptsEndorsedChecks.Properties.DisplayValueChecked = "Y";
		this.chk_AcceptsEndorsedChecks.Properties.DisplayValueGrayed = "N";
		this.chk_AcceptsEndorsedChecks.Properties.DisplayValueUnchecked = "N";
		this.chk_AcceptsEndorsedChecks.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_AcceptsEndorsedChecks.Properties.ValueChecked = "Y";
		this.chk_AcceptsEndorsedChecks.Properties.ValueGrayed = "N";
		this.chk_AcceptsEndorsedChecks.Properties.ValueUnchecked = "N";
		this.chk_AcceptsEndorsedChecks.SettingInVisible = false;
		this.chk_AcceptsEndorsedChecks.SettingReadonly = false;
		this.chk_AcceptsEndorsedChecks.Size = new System.Drawing.Size(197, 20);
		this.chk_AcceptsEndorsedChecks.TabIndex = 20;
		this.chk_AcceptsEndorsedChecks.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_EndorsableChecksFrom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_EndorsableChecksFrom.EditControl = null;
		this.chk_EndorsableChecksFrom.FieldName = "EdrsFromBP";
		this.chk_EndorsableChecksFrom.IsSearchControl = false;
		this.chk_EndorsableChecksFrom.Location = new System.Drawing.Point(600, 231);
		this.chk_EndorsableChecksFrom.Name = "chk_EndorsableChecksFrom";
		this.chk_EndorsableChecksFrom.Properties.Caption = "Endorsable Checks from This BP";
		this.chk_EndorsableChecksFrom.Properties.DisplayValueChecked = "Y";
		this.chk_EndorsableChecksFrom.Properties.DisplayValueGrayed = "N";
		this.chk_EndorsableChecksFrom.Properties.DisplayValueUnchecked = "N";
		this.chk_EndorsableChecksFrom.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_EndorsableChecksFrom.Properties.ValueChecked = "Y";
		this.chk_EndorsableChecksFrom.Properties.ValueGrayed = "N";
		this.chk_EndorsableChecksFrom.Properties.ValueUnchecked = "N";
		this.chk_EndorsableChecksFrom.SettingInVisible = false;
		this.chk_EndorsableChecksFrom.SettingReadonly = false;
		this.chk_EndorsableChecksFrom.Size = new System.Drawing.Size(197, 20);
		this.chk_EndorsableChecksFrom.TabIndex = 19;
		this.chk_EndorsableChecksFrom.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_DoNotApplyDiscountGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_DoNotApplyDiscountGroup.EditControl = null;
		this.chk_DoNotApplyDiscountGroup.FieldName = "NoDiscount";
		this.chk_DoNotApplyDiscountGroup.IsSearchControl = false;
		this.chk_DoNotApplyDiscountGroup.Location = new System.Drawing.Point(600, 212);
		this.chk_DoNotApplyDiscountGroup.Name = "chk_DoNotApplyDiscountGroup";
		this.chk_DoNotApplyDiscountGroup.Properties.Caption = "Do Not Apply Discount Groups";
		this.chk_DoNotApplyDiscountGroup.Properties.DisplayValueChecked = "Y";
		this.chk_DoNotApplyDiscountGroup.Properties.DisplayValueGrayed = "N";
		this.chk_DoNotApplyDiscountGroup.Properties.DisplayValueUnchecked = "N";
		this.chk_DoNotApplyDiscountGroup.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_DoNotApplyDiscountGroup.Properties.ValueChecked = "Y";
		this.chk_DoNotApplyDiscountGroup.Properties.ValueGrayed = "N";
		this.chk_DoNotApplyDiscountGroup.Properties.ValueUnchecked = "N";
		this.chk_DoNotApplyDiscountGroup.SettingInVisible = false;
		this.chk_DoNotApplyDiscountGroup.SettingReadonly = false;
		this.chk_DoNotApplyDiscountGroup.Size = new System.Drawing.Size(197, 20);
		this.chk_DoNotApplyDiscountGroup.TabIndex = 18;
		this.chk_DoNotApplyDiscountGroup.CheckedChanged += new System.EventHandler(this.chk_DoNotApplyDiscountGroup_CheckedChanged);
		this.chk_AllowPartialDlvrPerRow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_AllowPartialDlvrPerRow.EditControl = null;
		this.chk_AllowPartialDlvrPerRow.FieldName = "BackOrder";
		this.chk_AllowPartialDlvrPerRow.IsSearchControl = false;
		this.chk_AllowPartialDlvrPerRow.Location = new System.Drawing.Point(600, 194);
		this.chk_AllowPartialDlvrPerRow.Name = "chk_AllowPartialDlvrPerRow";
		this.chk_AllowPartialDlvrPerRow.Properties.Caption = "Allow Partial Delivery per Row";
		this.chk_AllowPartialDlvrPerRow.Properties.DisplayValueChecked = "Y";
		this.chk_AllowPartialDlvrPerRow.Properties.DisplayValueGrayed = "N";
		this.chk_AllowPartialDlvrPerRow.Properties.DisplayValueUnchecked = "N";
		this.chk_AllowPartialDlvrPerRow.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_AllowPartialDlvrPerRow.Properties.ValueChecked = "Y";
		this.chk_AllowPartialDlvrPerRow.Properties.ValueGrayed = "N";
		this.chk_AllowPartialDlvrPerRow.Properties.ValueUnchecked = "N";
		this.chk_AllowPartialDlvrPerRow.SettingInVisible = false;
		this.chk_AllowPartialDlvrPerRow.SettingReadonly = false;
		this.chk_AllowPartialDlvrPerRow.Size = new System.Drawing.Size(197, 20);
		this.chk_AllowPartialDlvrPerRow.TabIndex = 17;
		this.chk_AllowPartialDlvrPerRow.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_AllowPartialDlvrOfSO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_AllowPartialDlvrOfSO.EditControl = null;
		this.chk_AllowPartialDlvrOfSO.FieldName = "PartDelivr";
		this.chk_AllowPartialDlvrOfSO.IsSearchControl = false;
		this.chk_AllowPartialDlvrOfSO.Location = new System.Drawing.Point(600, 175);
		this.chk_AllowPartialDlvrOfSO.Name = "chk_AllowPartialDlvrOfSO";
		this.chk_AllowPartialDlvrOfSO.Properties.Caption = "Allow Partial Delivery of Sales Order";
		this.chk_AllowPartialDlvrOfSO.Properties.DisplayValueChecked = "Y";
		this.chk_AllowPartialDlvrOfSO.Properties.DisplayValueGrayed = "N";
		this.chk_AllowPartialDlvrOfSO.Properties.DisplayValueUnchecked = "N";
		this.chk_AllowPartialDlvrOfSO.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_AllowPartialDlvrOfSO.Properties.ValueChecked = "Y";
		this.chk_AllowPartialDlvrOfSO.Properties.ValueGrayed = "N";
		this.chk_AllowPartialDlvrOfSO.Properties.ValueUnchecked = "N";
		this.chk_AllowPartialDlvrOfSO.SettingInVisible = false;
		this.chk_AllowPartialDlvrOfSO.SettingReadonly = false;
		this.chk_AllowPartialDlvrOfSO.Size = new System.Drawing.Size(220, 20);
		this.chk_AllowPartialDlvrOfSO.TabIndex = 16;
		this.chk_AllowPartialDlvrOfSO.CheckedChanged += new System.EventHandler(this.chk_AllowPartialDlvrOfSO_CheckedChanged);
		this.luk_PaymentPriceList.EditControl = null;
		this.luk_PaymentPriceList.FieldName = "ListNum";
		this.luk_PaymentPriceList.IsSearchControl = false;
		this.luk_PaymentPriceList.Location = new System.Drawing.Point(148, 44);
		this.luk_PaymentPriceList.Name = "luk_PaymentPriceList";
		this.luk_PaymentPriceList.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_PaymentPriceList.Properties.Appearance.Options.UseFont = true;
		this.luk_PaymentPriceList.Properties.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.luk_PaymentPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_PaymentPriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PriceListName", "Name")
		});
		this.luk_PaymentPriceList.Properties.DisplayMember = "PriceListName";
		this.luk_PaymentPriceList.Properties.IsSearchControl = false;
		this.luk_PaymentPriceList.Properties.NullText = "";
		this.luk_PaymentPriceList.Properties.ValueMember = "PriceListNo";
		this.luk_PaymentPriceList.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.luk_PaymentPriceList_Properties_ButtonPressed);
		this.luk_PaymentPriceList.SettingInVisible = false;
		this.luk_PaymentPriceList.SettingReadonly = false;
		this.luk_PaymentPriceList.Size = new System.Drawing.Size(207, 18);
		this.luk_PaymentPriceList.TabIndex = 2;
		this.luk_PaymentPriceList.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_PaymentTerm.EditControl = null;
		this.luk_PaymentTerm.FieldName = "GroupNum";
		this.luk_PaymentTerm.IsSearchControl = false;
		this.luk_PaymentTerm.Location = new System.Drawing.Point(148, 6);
		this.luk_PaymentTerm.Name = "luk_PaymentTerm";
		this.luk_PaymentTerm.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_PaymentTerm.Properties.Appearance.Options.UseFont = true;
		this.luk_PaymentTerm.Properties.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.luk_PaymentTerm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_PaymentTerm.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PymntGroup", "Name")
		});
		this.luk_PaymentTerm.Properties.DisplayMember = "PymntGroup";
		this.luk_PaymentTerm.Properties.IsSearchControl = false;
		this.luk_PaymentTerm.Properties.NullText = "";
		this.luk_PaymentTerm.Properties.ValueMember = "GroupNum";
		this.luk_PaymentTerm.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbPaymentPaymentTerm_Properties_ButtonPressed);
		this.luk_PaymentTerm.SettingInVisible = false;
		this.luk_PaymentTerm.SettingReadonly = false;
		this.luk_PaymentTerm.Size = new System.Drawing.Size(207, 18);
		this.luk_PaymentTerm.TabIndex = 0;
		this.luk_PaymentTerm.EditValueChanged += new System.EventHandler(this.cbPaymentPaymentTerm_SelectedIndexChanged);
		this.txtPaymentTotalDiscount.EditControl = null;
		this.txtPaymentTotalDiscount.FieldName = "Discount";
		this.txtPaymentTotalDiscount.IsSearchControl = false;
		this.txtPaymentTotalDiscount.Location = new System.Drawing.Point(148, 63);
		this.txtPaymentTotalDiscount.Name = "txtPaymentTotalDiscount";
		this.txtPaymentTotalDiscount.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentTotalDiscount.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentTotalDiscount.Properties.AutoHeight = false;
		this.txtPaymentTotalDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPaymentTotalDiscount.Properties.DisplayFormat.FormatString = "n0";
		this.txtPaymentTotalDiscount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentTotalDiscount.Properties.EditFormat.FormatString = "n0";
		this.txtPaymentTotalDiscount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentTotalDiscount.Properties.IsSearchControl = false;
		this.txtPaymentTotalDiscount.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtPaymentTotalDiscount.Properties.Precision = 0;
		this.txtPaymentTotalDiscount.SettingInVisible = false;
		this.txtPaymentTotalDiscount.SettingReadonly = false;
		this.txtPaymentTotalDiscount.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentTotalDiscount.TabIndex = 3;
		this.txtPaymentTotalDiscount.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentInterestonArrears.EditControl = null;
		this.txtPaymentInterestonArrears.FieldName = "IntrstRate";
		this.txtPaymentInterestonArrears.IsSearchControl = false;
		this.txtPaymentInterestonArrears.Location = new System.Drawing.Point(148, 25);
		this.txtPaymentInterestonArrears.Name = "txtPaymentInterestonArrears";
		this.txtPaymentInterestonArrears.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentInterestonArrears.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentInterestonArrears.Properties.AutoHeight = false;
		this.txtPaymentInterestonArrears.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPaymentInterestonArrears.Properties.DisplayFormat.FormatString = "n0";
		this.txtPaymentInterestonArrears.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentInterestonArrears.Properties.EditFormat.FormatString = "n0";
		this.txtPaymentInterestonArrears.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentInterestonArrears.Properties.IsSearchControl = false;
		this.txtPaymentInterestonArrears.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtPaymentInterestonArrears.Properties.Precision = 0;
		this.txtPaymentInterestonArrears.SettingInVisible = false;
		this.txtPaymentInterestonArrears.SettingReadonly = false;
		this.txtPaymentInterestonArrears.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentInterestonArrears.TabIndex = 1;
		this.txtPaymentInterestonArrears.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentCreditLimit.EditControl = null;
		this.txtPaymentCreditLimit.FieldName = "CreditLine";
		this.txtPaymentCreditLimit.IsSearchControl = false;
		this.txtPaymentCreditLimit.Location = new System.Drawing.Point(148, 82);
		this.txtPaymentCreditLimit.Name = "txtPaymentCreditLimit";
		this.txtPaymentCreditLimit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentCreditLimit.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentCreditLimit.Properties.AutoHeight = false;
		this.txtPaymentCreditLimit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPaymentCreditLimit.Properties.DisplayFormat.FormatString = "n0";
		this.txtPaymentCreditLimit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentCreditLimit.Properties.EditFormat.FormatString = "n0";
		this.txtPaymentCreditLimit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentCreditLimit.Properties.IsSearchControl = false;
		this.txtPaymentCreditLimit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtPaymentCreditLimit.Properties.Precision = 0;
		this.txtPaymentCreditLimit.SettingInVisible = false;
		this.txtPaymentCreditLimit.SettingReadonly = false;
		this.txtPaymentCreditLimit.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentCreditLimit.TabIndex = 4;
		this.txtPaymentCreditLimit.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtPaymentCommitmentLimit.EditControl = null;
		this.txtPaymentCommitmentLimit.FieldName = "DebtLine";
		this.txtPaymentCommitmentLimit.IsSearchControl = false;
		this.txtPaymentCommitmentLimit.Location = new System.Drawing.Point(148, 101);
		this.txtPaymentCommitmentLimit.Name = "txtPaymentCommitmentLimit";
		this.txtPaymentCommitmentLimit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPaymentCommitmentLimit.Properties.Appearance.Options.UseFont = true;
		this.txtPaymentCommitmentLimit.Properties.AutoHeight = false;
		this.txtPaymentCommitmentLimit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPaymentCommitmentLimit.Properties.DisplayFormat.FormatString = "n0";
		this.txtPaymentCommitmentLimit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentCommitmentLimit.Properties.EditFormat.FormatString = "n0";
		this.txtPaymentCommitmentLimit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaymentCommitmentLimit.Properties.IsSearchControl = false;
		this.txtPaymentCommitmentLimit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtPaymentCommitmentLimit.Properties.Precision = 0;
		this.txtPaymentCommitmentLimit.SettingInVisible = false;
		this.txtPaymentCommitmentLimit.SettingReadonly = false;
		this.txtPaymentCommitmentLimit.Size = new System.Drawing.Size(207, 18);
		this.txtPaymentCommitmentLimit.TabIndex = 5;
		this.txtPaymentCommitmentLimit.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_Commitment.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Commitment.Appearance.Options.UseBackColor = true;
		this.lb_Commitment.EditControl = null;
		this.lb_Commitment.EditValue = "Commitment Limit";
		this.lb_Commitment.FieldName = null;
		this.lb_Commitment.HideWhenEmpty = false;
		this.lb_Commitment.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Commitment.Location = new System.Drawing.Point(9, 102);
		this.lb_Commitment.Name = "lb_Commitment";
		this.lb_Commitment.ReadOnly = false;
		this.lb_Commitment.SettingInVisible = false;
		this.lb_Commitment.SettingReadonly = false;
		this.lb_Commitment.ShowUnderLine = true;
		this.lb_Commitment.Size = new System.Drawing.Size(140, 20);
		this.lb_Commitment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Commitment.TabIndex = 53;
		this.lb_Commitment.TabStop = false;
		this.lb_Commitment.Utext = "Commitment Limit";
		this.lb_CreditL.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_CreditL.Appearance.Options.UseBackColor = true;
		this.lb_CreditL.EditControl = null;
		this.lb_CreditL.EditValue = "Credit Limit";
		this.lb_CreditL.FieldName = null;
		this.lb_CreditL.HideWhenEmpty = false;
		this.lb_CreditL.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CreditL.Location = new System.Drawing.Point(9, 83);
		this.lb_CreditL.Name = "lb_CreditL";
		this.lb_CreditL.ReadOnly = false;
		this.lb_CreditL.SettingInVisible = false;
		this.lb_CreditL.SettingReadonly = false;
		this.lb_CreditL.ShowUnderLine = true;
		this.lb_CreditL.Size = new System.Drawing.Size(140, 20);
		this.lb_CreditL.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CreditL.TabIndex = 47;
		this.lb_CreditL.TabStop = false;
		this.lb_CreditL.Utext = "Credit Limit";
		this.lb_Interest.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_Interest.Appearance.Options.UseBackColor = true;
		this.lb_Interest.EditControl = null;
		this.lb_Interest.EditValue = "Interest on Arrears %";
		this.lb_Interest.FieldName = null;
		this.lb_Interest.HideWhenEmpty = false;
		this.lb_Interest.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Interest.Location = new System.Drawing.Point(9, 26);
		this.lb_Interest.Name = "lb_Interest";
		this.lb_Interest.ReadOnly = false;
		this.lb_Interest.SettingInVisible = false;
		this.lb_Interest.SettingReadonly = false;
		this.lb_Interest.ShowUnderLine = true;
		this.lb_Interest.Size = new System.Drawing.Size(140, 20);
		this.lb_Interest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Interest.TabIndex = 45;
		this.lb_Interest.TabStop = false;
		this.lb_Interest.Utext = "Interest on Arrears %";
		this.lb_TotalDis.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_TotalDis.Appearance.Options.UseBackColor = true;
		this.lb_TotalDis.EditControl = null;
		this.lb_TotalDis.EditValue = "Total Discount %";
		this.lb_TotalDis.FieldName = null;
		this.lb_TotalDis.HideWhenEmpty = false;
		this.lb_TotalDis.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TotalDis.Location = new System.Drawing.Point(9, 64);
		this.lb_TotalDis.Name = "lb_TotalDis";
		this.lb_TotalDis.ReadOnly = false;
		this.lb_TotalDis.SettingInVisible = false;
		this.lb_TotalDis.SettingReadonly = false;
		this.lb_TotalDis.ShowUnderLine = true;
		this.lb_TotalDis.Size = new System.Drawing.Size(140, 20);
		this.lb_TotalDis.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TotalDis.TabIndex = 43;
		this.lb_TotalDis.TabStop = false;
		this.lb_TotalDis.Utext = "Total Discount %";
		this.lb_PriceLst.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_PriceLst.Appearance.Options.UseBackColor = true;
		this.lb_PriceLst.EditControl = null;
		this.lb_PriceLst.EditValue = "Price List";
		this.lb_PriceLst.FieldName = null;
		this.lb_PriceLst.HideWhenEmpty = false;
		this.lb_PriceLst.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PriceLst.Location = new System.Drawing.Point(9, 45);
		this.lb_PriceLst.Name = "lb_PriceLst";
		this.lb_PriceLst.ReadOnly = false;
		this.lb_PriceLst.SettingInVisible = false;
		this.lb_PriceLst.SettingReadonly = false;
		this.lb_PriceLst.ShowUnderLine = true;
		this.lb_PriceLst.Size = new System.Drawing.Size(140, 20);
		this.lb_PriceLst.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PriceLst.TabIndex = 41;
		this.lb_PriceLst.TabStop = false;
		this.lb_PriceLst.Utext = "Price List";
		this.lb_PaymentTerms.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_PaymentTerms.Appearance.Options.UseBackColor = true;
		this.lb_PaymentTerms.EditControl = null;
		this.lb_PaymentTerms.EditValue = "Payment Terms";
		this.lb_PaymentTerms.FieldName = null;
		this.lb_PaymentTerms.HideWhenEmpty = false;
		this.lb_PaymentTerms.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PaymentTerms.Location = new System.Drawing.Point(9, 7);
		this.lb_PaymentTerms.Name = "lb_PaymentTerms";
		this.lb_PaymentTerms.ReadOnly = false;
		this.lb_PaymentTerms.SettingInVisible = false;
		this.lb_PaymentTerms.SettingReadonly = false;
		this.lb_PaymentTerms.ShowUnderLine = true;
		this.lb_PaymentTerms.Size = new System.Drawing.Size(140, 20);
		this.lb_PaymentTerms.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PaymentTerms.TabIndex = 39;
		this.lb_PaymentTerms.TabStop = false;
		this.lb_PaymentTerms.Utext = "Payment Terms";
		this.lb_AvarageDelay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_AvarageDelay.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_AvarageDelay.Appearance.Options.UseBackColor = true;
		this.lb_AvarageDelay.EditControl = null;
		this.lb_AvarageDelay.EditValue = "Avarage Delay";
		this.lb_AvarageDelay.FieldName = null;
		this.lb_AvarageDelay.HideWhenEmpty = false;
		this.lb_AvarageDelay.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AvarageDelay.Location = new System.Drawing.Point(502, 80);
		this.lb_AvarageDelay.Name = "lb_AvarageDelay";
		this.lb_AvarageDelay.ReadOnly = false;
		this.lb_AvarageDelay.SettingInVisible = false;
		this.lb_AvarageDelay.SettingReadonly = false;
		this.lb_AvarageDelay.ShowUnderLine = true;
		this.lb_AvarageDelay.Size = new System.Drawing.Size(140, 20);
		this.lb_AvarageDelay.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AvarageDelay.TabIndex = 71;
		this.lb_AvarageDelay.TabStop = false;
		this.lb_AvarageDelay.Utext = "Avarage Delay";
		this.lb_IDNumber.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_IDNumber.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_IDNumber.Appearance.Options.UseBackColor = true;
		this.lb_IDNumber.EditControl = null;
		this.lb_IDNumber.EditValue = "ID Number";
		this.lb_IDNumber.FieldName = null;
		this.lb_IDNumber.HideWhenEmpty = false;
		this.lb_IDNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_IDNumber.Location = new System.Drawing.Point(502, 61);
		this.lb_IDNumber.Name = "lb_IDNumber";
		this.lb_IDNumber.ReadOnly = false;
		this.lb_IDNumber.SettingInVisible = false;
		this.lb_IDNumber.SettingReadonly = false;
		this.lb_IDNumber.ShowUnderLine = true;
		this.lb_IDNumber.Size = new System.Drawing.Size(140, 20);
		this.lb_IDNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_IDNumber.TabIndex = 69;
		this.lb_IDNumber.TabStop = false;
		this.lb_IDNumber.Utext = "ID Number";
		this.lb_ExpirationDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_ExpirationDate.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_ExpirationDate.Appearance.Options.UseBackColor = true;
		this.lb_ExpirationDate.EditControl = null;
		this.lb_ExpirationDate.EditValue = "Expiration Date";
		this.lb_ExpirationDate.FieldName = null;
		this.lb_ExpirationDate.HideWhenEmpty = false;
		this.lb_ExpirationDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ExpirationDate.Location = new System.Drawing.Point(502, 42);
		this.lb_ExpirationDate.Name = "lb_ExpirationDate";
		this.lb_ExpirationDate.ReadOnly = false;
		this.lb_ExpirationDate.SettingInVisible = false;
		this.lb_ExpirationDate.SettingReadonly = false;
		this.lb_ExpirationDate.ShowUnderLine = true;
		this.lb_ExpirationDate.Size = new System.Drawing.Size(140, 20);
		this.lb_ExpirationDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ExpirationDate.TabIndex = 67;
		this.lb_ExpirationDate.TabStop = false;
		this.lb_ExpirationDate.Utext = "Expiration Date";
		this.lb_CreditCardNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_CreditCardNo.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_CreditCardNo.Appearance.Options.UseBackColor = true;
		this.lb_CreditCardNo.EditControl = null;
		this.lb_CreditCardNo.EditValue = "Credit Card No.";
		this.lb_CreditCardNo.FieldName = null;
		this.lb_CreditCardNo.HideWhenEmpty = false;
		this.lb_CreditCardNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CreditCardNo.Location = new System.Drawing.Point(502, 22);
		this.lb_CreditCardNo.Name = "lb_CreditCardNo";
		this.lb_CreditCardNo.ReadOnly = false;
		this.lb_CreditCardNo.SettingInVisible = false;
		this.lb_CreditCardNo.SettingReadonly = false;
		this.lb_CreditCardNo.ShowUnderLine = true;
		this.lb_CreditCardNo.Size = new System.Drawing.Size(140, 20);
		this.lb_CreditCardNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CreditCardNo.TabIndex = 65;
		this.lb_CreditCardNo.TabStop = false;
		this.lb_CreditCardNo.Utext = "Credit Card No.";
		this.lb_CreditCardType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_CreditCardType.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.lb_CreditCardType.Appearance.Options.UseBackColor = true;
		this.lb_CreditCardType.EditControl = null;
		this.lb_CreditCardType.EditValue = "Credit Card Type";
		this.lb_CreditCardType.FieldName = null;
		this.lb_CreditCardType.HideWhenEmpty = false;
		this.lb_CreditCardType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CreditCardType.Location = new System.Drawing.Point(502, 3);
		this.lb_CreditCardType.Name = "lb_CreditCardType";
		this.lb_CreditCardType.ReadOnly = false;
		this.lb_CreditCardType.SettingInVisible = false;
		this.lb_CreditCardType.SettingReadonly = false;
		this.lb_CreditCardType.ShowUnderLine = true;
		this.lb_CreditCardType.Size = new System.Drawing.Size(140, 20);
		this.lb_CreditCardType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CreditCardType.TabIndex = 63;
		this.lb_CreditCardType.TabStop = false;
		this.lb_CreditCardType.Utext = "Credit Card Type";
		this.tab_PaymentRun.Controls.Add(this.luk_BCACode);
		this.tab_PaymentRun.Controls.Add(this.chk_AutoCalBCG);
		this.tab_PaymentRun.Controls.Add(this.labelLine38);
		this.tab_PaymentRun.Controls.Add(this.chk_CollecAuth);
		this.tab_PaymentRun.Controls.Add(this.txt_RefDetails);
		this.tab_PaymentRun.Controls.Add(this.labelLine37);
		this.tab_PaymentRun.Controls.Add(this.panelControl2);
		this.tab_PaymentRun.Controls.Add(this.labelLine25);
		this.tab_PaymentRun.EditControl = null;
		this.tab_PaymentRun.EditValue = "Payment Run";
		this.tab_PaymentRun.FieldName = "tab_PaymentRun";
		this.tab_PaymentRun.IsSearchControl = false;
		this.tab_PaymentRun.Name = "tab_PaymentRun";
		this.tab_PaymentRun.ReadOnly = false;
		this.tab_PaymentRun.SettingInVisible = false;
		this.tab_PaymentRun.SettingReadonly = false;
		this.tab_PaymentRun.Size = new System.Drawing.Size(871, 399);
		this.tab_PaymentRun.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_PaymentRun.Text = "Payment Run";
		this.luk_BCACode.EditControl = null;
		this.luk_BCACode.FieldName = "BCACode";
		this.luk_BCACode.IsSearchControl = false;
		this.luk_BCACode.Location = new System.Drawing.Point(240, 273);
		this.luk_BCACode.Name = "luk_BCACode";
		this.luk_BCACode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_BCACode.Properties.Appearance.Options.UseFont = true;
		this.luk_BCACode.Properties.AutoHeight = false;
		this.luk_BCACode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BCACode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_BCACode.Properties.DisplayMember = "Name";
		this.luk_BCACode.Properties.IsSearchControl = false;
		this.luk_BCACode.Properties.NullText = "";
		this.luk_BCACode.Properties.ValueMember = "Code";
		this.luk_BCACode.SettingInVisible = false;
		this.luk_BCACode.SettingReadonly = false;
		this.luk_BCACode.Size = new System.Drawing.Size(97, 18);
		this.luk_BCACode.TabIndex = 18;
		this.luk_BCACode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_AutoCalBCG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_AutoCalBCG.EditControl = null;
		this.chk_AutoCalBCG.FieldName = "AutoCalBCG";
		this.chk_AutoCalBCG.IsSearchControl = false;
		this.chk_AutoCalBCG.Location = new System.Drawing.Point(22, 298);
		this.chk_AutoCalBCG.Name = "chk_AutoCalBCG";
		this.chk_AutoCalBCG.Properties.Caption = "Auto. Cal. Bank Charge for Incoming Payment";
		this.chk_AutoCalBCG.Properties.DisplayValueChecked = "Y";
		this.chk_AutoCalBCG.Properties.DisplayValueGrayed = "N";
		this.chk_AutoCalBCG.Properties.DisplayValueUnchecked = "N";
		this.chk_AutoCalBCG.Properties.ReadOnly = true;
		this.chk_AutoCalBCG.Properties.ValueChecked = "Y";
		this.chk_AutoCalBCG.Properties.ValueGrayed = "N";
		this.chk_AutoCalBCG.Properties.ValueUnchecked = "N";
		this.chk_AutoCalBCG.SettingInVisible = false;
		this.chk_AutoCalBCG.SettingReadonly = false;
		this.chk_AutoCalBCG.Size = new System.Drawing.Size(243, 20);
		this.chk_AutoCalBCG.TabIndex = 20;
		this.labelLine38.EditControl = null;
		this.labelLine38.EditValue = "Bank Charges Allocation Code";
		this.labelLine38.FieldName = null;
		this.labelLine38.HideWhenEmpty = false;
		this.labelLine38.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine38.Location = new System.Drawing.Point(22, 274);
		this.labelLine38.Name = "labelLine38";
		this.labelLine38.ReadOnly = false;
		this.labelLine38.SettingInVisible = false;
		this.labelLine38.SettingReadonly = false;
		this.labelLine38.ShowUnderLine = true;
		this.labelLine38.Size = new System.Drawing.Size(220, 20);
		this.labelLine38.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine38.TabIndex = 19;
		this.labelLine38.TabStop = false;
		this.labelLine38.Utext = "Bank Charges Allocation Code";
		this.chk_CollecAuth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CollecAuth.EditControl = null;
		this.chk_CollecAuth.FieldName = null;
		this.chk_CollecAuth.IsSearchControl = false;
		this.chk_CollecAuth.Location = new System.Drawing.Point(22, 252);
		this.chk_CollecAuth.Name = "chk_CollecAuth";
		this.chk_CollecAuth.Properties.Caption = "Collection Authori&zation";
		this.chk_CollecAuth.Properties.DisplayValueChecked = "Y";
		this.chk_CollecAuth.Properties.DisplayValueGrayed = "N";
		this.chk_CollecAuth.Properties.DisplayValueUnchecked = "N";
		this.chk_CollecAuth.Properties.ValueChecked = "Y";
		this.chk_CollecAuth.Properties.ValueGrayed = "N";
		this.chk_CollecAuth.Properties.ValueUnchecked = "N";
		this.chk_CollecAuth.SettingInVisible = false;
		this.chk_CollecAuth.SettingReadonly = false;
		this.chk_CollecAuth.Size = new System.Drawing.Size(220, 20);
		this.chk_CollecAuth.TabIndex = 17;
		this.chk_CollecAuth.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_RefDetails.EditControl = null;
		this.txt_RefDetails.FieldName = "RefDetails";
		this.txt_RefDetails.IsSearchControl = false;
		this.txt_RefDetails.Location = new System.Drawing.Point(172, 207);
		this.txt_RefDetails.Name = "txt_RefDetails";
		this.txt_RefDetails.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_RefDetails.Properties.Appearance.Options.UseFont = true;
		this.txt_RefDetails.Properties.AutoHeight = false;
		this.txt_RefDetails.SettingInVisible = false;
		this.txt_RefDetails.SettingReadonly = false;
		this.txt_RefDetails.Size = new System.Drawing.Size(165, 18);
		this.txt_RefDetails.TabIndex = 14;
		this.txt_RefDetails.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine37.EditControl = null;
		this.labelLine37.EditValue = "Reference Details";
		this.labelLine37.FieldName = null;
		this.labelLine37.HideWhenEmpty = false;
		this.labelLine37.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine37.Location = new System.Drawing.Point(22, 208);
		this.labelLine37.Name = "labelLine37";
		this.labelLine37.ReadOnly = false;
		this.labelLine37.SettingInVisible = false;
		this.labelLine37.SettingReadonly = false;
		this.labelLine37.ShowUnderLine = true;
		this.labelLine37.Size = new System.Drawing.Size(150, 20);
		this.labelLine37.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine37.TabIndex = 13;
		this.labelLine37.TabStop = false;
		this.labelLine37.Utext = "Reference Details";
		this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.panelControl2.Controls.Add(this.txt_HousCtlKey);
		this.panelControl2.Controls.Add(this.txt_HsBnkSwift);
		this.panelControl2.Controls.Add(this.txt_HousBnkBrn);
		this.panelControl2.Controls.Add(this.luk_HsBnkIBAN);
		this.panelControl2.Controls.Add(this.luk_HousBnkAct);
		this.panelControl2.Controls.Add(this.luk_HouseBank);
		this.panelControl2.Controls.Add(this.luk_HousBnkCry);
		this.panelControl2.Controls.Add(this.labelLine36);
		this.panelControl2.Controls.Add(this.labelLine35);
		this.panelControl2.Controls.Add(this.labelLine34);
		this.panelControl2.Controls.Add(this.labelLine33);
		this.panelControl2.Controls.Add(this.labelLine32);
		this.panelControl2.Controls.Add(this.labelLine31);
		this.panelControl2.Controls.Add(this.labelLine30);
		this.panelControl2.Location = new System.Drawing.Point(16, 23);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(329, 157);
		this.panelControl2.TabIndex = 2;
		this.txt_HousCtlKey.EditControl = null;
		this.txt_HousCtlKey.FieldName = "HousCtlKey";
		this.txt_HousCtlKey.IsSearchControl = false;
		this.txt_HousCtlKey.Location = new System.Drawing.Point(156, 128);
		this.txt_HousCtlKey.Name = "txt_HousCtlKey";
		this.txt_HousCtlKey.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_HousCtlKey.Properties.Appearance.Options.UseFont = true;
		this.txt_HousCtlKey.Properties.AutoHeight = false;
		this.txt_HousCtlKey.Properties.ReadOnly = true;
		this.txt_HousCtlKey.SettingInVisible = false;
		this.txt_HousCtlKey.SettingReadonly = false;
		this.txt_HousCtlKey.Size = new System.Drawing.Size(165, 18);
		this.txt_HousCtlKey.TabIndex = 14;
		this.txt_HsBnkSwift.EditControl = null;
		this.txt_HsBnkSwift.FieldName = "HsBnkSwift";
		this.txt_HsBnkSwift.IsSearchControl = false;
		this.txt_HsBnkSwift.Location = new System.Drawing.Point(156, 108);
		this.txt_HsBnkSwift.Name = "txt_HsBnkSwift";
		this.txt_HsBnkSwift.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_HsBnkSwift.Properties.Appearance.Options.UseFont = true;
		this.txt_HsBnkSwift.Properties.AutoHeight = false;
		this.txt_HsBnkSwift.Properties.ReadOnly = true;
		this.txt_HsBnkSwift.SettingInVisible = false;
		this.txt_HsBnkSwift.SettingReadonly = false;
		this.txt_HsBnkSwift.Size = new System.Drawing.Size(165, 18);
		this.txt_HsBnkSwift.TabIndex = 13;
		this.txt_HousBnkBrn.EditControl = null;
		this.txt_HousBnkBrn.FieldName = "HousBnkBrn";
		this.txt_HousBnkBrn.IsSearchControl = false;
		this.txt_HousBnkBrn.Location = new System.Drawing.Point(156, 68);
		this.txt_HousBnkBrn.Name = "txt_HousBnkBrn";
		this.txt_HousBnkBrn.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_HousBnkBrn.Properties.Appearance.Options.UseFont = true;
		this.txt_HousBnkBrn.Properties.AutoHeight = false;
		this.txt_HousBnkBrn.Properties.ReadOnly = true;
		this.txt_HousBnkBrn.SettingInVisible = false;
		this.txt_HousBnkBrn.SettingReadonly = false;
		this.txt_HousBnkBrn.Size = new System.Drawing.Size(165, 18);
		this.txt_HousBnkBrn.TabIndex = 12;
		this.luk_HsBnkIBAN.EditControl = null;
		this.luk_HsBnkIBAN.FieldName = "HsBnkIBAN";
		this.luk_HsBnkIBAN.IsSearchControl = false;
		this.luk_HsBnkIBAN.Location = new System.Drawing.Point(156, 88);
		this.luk_HsBnkIBAN.Name = "luk_HsBnkIBAN";
		this.luk_HsBnkIBAN.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_HsBnkIBAN.Properties.Appearance.Options.UseFont = true;
		this.luk_HsBnkIBAN.Properties.AutoHeight = false;
		this.luk_HsBnkIBAN.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HsBnkIBAN.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IBAN", "IBAN")
		});
		this.luk_HsBnkIBAN.Properties.DisplayMember = "IBAN";
		this.luk_HsBnkIBAN.Properties.IsSearchControl = false;
		this.luk_HsBnkIBAN.Properties.NullText = "";
		this.luk_HsBnkIBAN.Properties.ValueMember = "Account";
		this.luk_HsBnkIBAN.SettingInVisible = false;
		this.luk_HsBnkIBAN.SettingReadonly = false;
		this.luk_HsBnkIBAN.Size = new System.Drawing.Size(165, 18);
		this.luk_HsBnkIBAN.TabIndex = 11;
		this.luk_HsBnkIBAN.EditValueChanged += new System.EventHandler(this.luk_HsBnkIban_EditValueChanged);
		this.luk_HousBnkAct.EditControl = null;
		this.luk_HousBnkAct.FieldName = "HousBnkAct";
		this.luk_HousBnkAct.IsSearchControl = false;
		this.luk_HousBnkAct.Location = new System.Drawing.Point(156, 48);
		this.luk_HousBnkAct.Name = "luk_HousBnkAct";
		this.luk_HousBnkAct.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_HousBnkAct.Properties.Appearance.Options.UseFont = true;
		this.luk_HousBnkAct.Properties.AutoHeight = false;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.luk_HousBnkAct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_HousBnkAct.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Account", "Account")
		});
		this.luk_HousBnkAct.Properties.DisplayMember = "Account";
		this.luk_HousBnkAct.Properties.IsSearchControl = false;
		this.luk_HousBnkAct.Properties.NullText = "";
		this.luk_HousBnkAct.Properties.ValueMember = "Account";
		this.luk_HousBnkAct.SettingInVisible = false;
		this.luk_HousBnkAct.SettingReadonly = false;
		this.luk_HousBnkAct.Size = new System.Drawing.Size(165, 18);
		this.luk_HousBnkAct.TabIndex = 10;
		this.luk_HousBnkAct.EditValueChanged += new System.EventHandler(this.luk_HousBnkAct_EditValueChanged);
		this.luk_HouseBank.EditControl = null;
		this.luk_HouseBank.FieldName = "HouseBank";
		this.luk_HouseBank.IsSearchControl = false;
		this.luk_HouseBank.Location = new System.Drawing.Point(156, 28);
		this.luk_HouseBank.Name = "luk_HouseBank";
		this.luk_HouseBank.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_HouseBank.Properties.Appearance.Options.UseFont = true;
		this.luk_HouseBank.Properties.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.luk_HouseBank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_HouseBank.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BankName", "BankName")
		});
		this.luk_HouseBank.Properties.DisplayMember = "BankName";
		this.luk_HouseBank.Properties.IsSearchControl = false;
		this.luk_HouseBank.Properties.NullText = "";
		this.luk_HouseBank.Properties.ValueMember = "BankCode";
		this.luk_HouseBank.SettingInVisible = false;
		this.luk_HouseBank.SettingReadonly = false;
		this.luk_HouseBank.Size = new System.Drawing.Size(165, 18);
		this.luk_HouseBank.TabIndex = 9;
		this.luk_HouseBank.EditValueChanged += new System.EventHandler(this.luk_HouseBank_EditValueChanged);
		this.luk_HousBnkCry.EditControl = null;
		this.luk_HousBnkCry.FieldName = "HousBnkCry";
		this.luk_HousBnkCry.IsSearchControl = false;
		this.luk_HousBnkCry.Location = new System.Drawing.Point(156, 8);
		this.luk_HousBnkCry.Name = "luk_HousBnkCry";
		this.luk_HousBnkCry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_HousBnkCry.Properties.Appearance.Options.UseFont = true;
		this.luk_HousBnkCry.Properties.AutoHeight = false;
		this.luk_HousBnkCry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HousBnkCry.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_HousBnkCry.Properties.DisplayMember = "Name";
		this.luk_HousBnkCry.Properties.IsSearchControl = false;
		this.luk_HousBnkCry.Properties.NullText = "";
		this.luk_HousBnkCry.Properties.ValueMember = "Code";
		this.luk_HousBnkCry.SettingInVisible = false;
		this.luk_HousBnkCry.SettingReadonly = false;
		this.luk_HousBnkCry.Size = new System.Drawing.Size(165, 18);
		this.luk_HousBnkCry.TabIndex = 8;
		this.luk_HousBnkCry.EditValueChanged += new System.EventHandler(this.luk_HousBnkCry_EditValueChanged);
		this.labelLine36.EditControl = null;
		this.labelLine36.EditValue = "Control No.";
		this.labelLine36.FieldName = null;
		this.labelLine36.HideWhenEmpty = false;
		this.labelLine36.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine36.Location = new System.Drawing.Point(6, 129);
		this.labelLine36.Name = "labelLine36";
		this.labelLine36.ReadOnly = false;
		this.labelLine36.SettingInVisible = false;
		this.labelLine36.SettingReadonly = false;
		this.labelLine36.ShowUnderLine = true;
		this.labelLine36.Size = new System.Drawing.Size(150, 20);
		this.labelLine36.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine36.TabIndex = 7;
		this.labelLine36.TabStop = false;
		this.labelLine36.Utext = "Control No.";
		this.labelLine35.EditControl = null;
		this.labelLine35.EditValue = "BIC/SWIFT Code";
		this.labelLine35.FieldName = null;
		this.labelLine35.HideWhenEmpty = false;
		this.labelLine35.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine35.Location = new System.Drawing.Point(6, 109);
		this.labelLine35.Name = "labelLine35";
		this.labelLine35.ReadOnly = false;
		this.labelLine35.SettingInVisible = false;
		this.labelLine35.SettingReadonly = false;
		this.labelLine35.ShowUnderLine = true;
		this.labelLine35.Size = new System.Drawing.Size(150, 20);
		this.labelLine35.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine35.TabIndex = 6;
		this.labelLine35.TabStop = false;
		this.labelLine35.Utext = "BIC/SWIFT Code";
		this.labelLine34.EditControl = null;
		this.labelLine34.EditValue = "IBAN";
		this.labelLine34.FieldName = null;
		this.labelLine34.HideWhenEmpty = false;
		this.labelLine34.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine34.Location = new System.Drawing.Point(6, 89);
		this.labelLine34.Name = "labelLine34";
		this.labelLine34.ReadOnly = false;
		this.labelLine34.SettingInVisible = false;
		this.labelLine34.SettingReadonly = false;
		this.labelLine34.ShowUnderLine = true;
		this.labelLine34.Size = new System.Drawing.Size(150, 20);
		this.labelLine34.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine34.TabIndex = 5;
		this.labelLine34.TabStop = false;
		this.labelLine34.Utext = "IBAN";
		this.labelLine33.EditControl = null;
		this.labelLine33.EditValue = "Branch";
		this.labelLine33.FieldName = null;
		this.labelLine33.HideWhenEmpty = false;
		this.labelLine33.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine33.Location = new System.Drawing.Point(6, 69);
		this.labelLine33.Name = "labelLine33";
		this.labelLine33.ReadOnly = false;
		this.labelLine33.SettingInVisible = false;
		this.labelLine33.SettingReadonly = false;
		this.labelLine33.ShowUnderLine = true;
		this.labelLine33.Size = new System.Drawing.Size(150, 20);
		this.labelLine33.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine33.TabIndex = 4;
		this.labelLine33.TabStop = false;
		this.labelLine33.Utext = "Branch";
		this.labelLine32.EditControl = null;
		this.labelLine32.EditValue = "Account";
		this.labelLine32.FieldName = null;
		this.labelLine32.HideWhenEmpty = false;
		this.labelLine32.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine32.Location = new System.Drawing.Point(6, 49);
		this.labelLine32.Name = "labelLine32";
		this.labelLine32.ReadOnly = false;
		this.labelLine32.SettingInVisible = false;
		this.labelLine32.SettingReadonly = false;
		this.labelLine32.ShowUnderLine = true;
		this.labelLine32.Size = new System.Drawing.Size(150, 20);
		this.labelLine32.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine32.TabIndex = 3;
		this.labelLine32.TabStop = false;
		this.labelLine32.Utext = "Account";
		this.labelLine31.EditControl = null;
		this.labelLine31.EditValue = "Bank";
		this.labelLine31.FieldName = null;
		this.labelLine31.HideWhenEmpty = false;
		this.labelLine31.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine31.Location = new System.Drawing.Point(6, 29);
		this.labelLine31.Name = "labelLine31";
		this.labelLine31.ReadOnly = false;
		this.labelLine31.SettingInVisible = false;
		this.labelLine31.SettingReadonly = false;
		this.labelLine31.ShowUnderLine = true;
		this.labelLine31.Size = new System.Drawing.Size(150, 20);
		this.labelLine31.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine31.TabIndex = 2;
		this.labelLine31.TabStop = false;
		this.labelLine31.Utext = "Bank";
		this.labelLine30.EditControl = null;
		this.labelLine30.EditValue = "Country";
		this.labelLine30.FieldName = null;
		this.labelLine30.HideWhenEmpty = false;
		this.labelLine30.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine30.Location = new System.Drawing.Point(6, 9);
		this.labelLine30.Name = "labelLine30";
		this.labelLine30.ReadOnly = false;
		this.labelLine30.SettingInVisible = false;
		this.labelLine30.SettingReadonly = false;
		this.labelLine30.ShowUnderLine = true;
		this.labelLine30.Size = new System.Drawing.Size(150, 20);
		this.labelLine30.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine30.TabIndex = 1;
		this.labelLine30.TabStop = false;
		this.labelLine30.Utext = "Country";
		this.labelLine25.EditControl = null;
		this.labelLine25.EditValue = "House Bank";
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.labelLine25.Location = new System.Drawing.Point(16, 3);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ReadOnly = false;
		this.labelLine25.SettingInVisible = false;
		this.labelLine25.SettingReadonly = false;
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(65, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 1;
		this.labelLine25.TabStop = false;
		this.labelLine25.Utext = "House Bank";
		this.tab_Accounting.Controls.Add(this.xtraTabControl1);
		this.tab_Accounting.EditControl = null;
		this.tab_Accounting.EditValue = "Accounting";
		this.tab_Accounting.FieldName = "tab_Accounting";
		this.tab_Accounting.IsSearchControl = false;
		this.tab_Accounting.Name = "tab_Accounting";
		this.tab_Accounting.ReadOnly = false;
		this.tab_Accounting.SettingInVisible = false;
		this.tab_Accounting.SettingReadonly = false;
		this.tab_Accounting.Size = new System.Drawing.Size(871, 399);
		this.tab_Accounting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Accounting.Text = "Accounting";
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.Header.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.Header.BorderColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.Header.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.Header.Options.UseBorderColor = true;
		this.xtraTabControl1.AppearancePage.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.PageClient.Options.UseBackColor = true;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.tab_Accounting_general;
		this.xtraTabControl1.Size = new System.Drawing.Size(871, 399);
		this.xtraTabControl1.TabIndex = 11;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Accounting_general, this.tab_Accounting_tax });
		this.tab_Accounting_general.Appearance.PageClient.BackColor = System.Drawing.SystemColors.Control;
		this.tab_Accounting_general.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Accounting_general.Controls.Add(this.lbl_ControlAccts);
		this.tab_Accounting_general.Controls.Add(this.lbl_ConnBPName);
		this.tab_Accounting_general.Controls.Add(this.lbl_ConSolidBPName);
		this.tab_Accounting_general.Controls.Add(this.lbl_InterimAcctName);
		this.tab_Accounting_general.Controls.Add(this.lbl_ClearAcctName);
		this.tab_Accounting_general.Controls.Add(this.lbl_AcctPayableName);
		this.tab_Accounting_general.Controls.Add(this.chk_BlockDunn);
		this.tab_Accounting_general.Controls.Add(this.dateX_DunnDate);
		this.tab_Accounting_general.Controls.Add(this.calc_DunnLevel);
		this.tab_Accounting_general.Controls.Add(this.bte_ConnBP);
		this.tab_Accounting_general.Controls.Add(this.chk_UseShpdGd);
		this.tab_Accounting_general.Controls.Add(this.chk_Affiliate);
		this.tab_Accounting_general.Controls.Add(this.txt_PlngGroup);
		this.tab_Accounting_general.Controls.Add(this.labelLine26);
		this.tab_Accounting_general.Controls.Add(this.lbl_ConnBP);
		this.tab_Accounting_general.Controls.Add(this.bte_DpmIntAct);
		this.tab_Accounting_general.Controls.Add(this.labelLine24);
		this.tab_Accounting_general.Controls.Add(this.bte_DpmClear);
		this.tab_Accounting_general.Controls.Add(this.labelLine23);
		this.tab_Accounting_general.Controls.Add(this.bte_DebPayAcct);
		this.tab_Accounting_general.Controls.Add(this.lbl_DebPayAcct);
		this.tab_Accounting_general.Controls.Add(this.btEx_ControlAccts);
		this.tab_Accounting_general.Controls.Add(this.rdg_FatherType);
		this.tab_Accounting_general.Controls.Add(this.bte_FatherCard);
		this.tab_Accounting_general.Controls.Add(this.labelLine8);
		this.tab_Accounting_general.Controls.Add(this.lbl_DunnDate);
		this.tab_Accounting_general.Controls.Add(this.lbl_DunnLevel);
		this.tab_Accounting_general.EditControl = null;
		this.tab_Accounting_general.EditValue = "General";
		this.tab_Accounting_general.FieldName = "tab_Accounting_general";
		this.tab_Accounting_general.IsSearchControl = false;
		this.tab_Accounting_general.Name = "tab_Accounting_general";
		this.tab_Accounting_general.ReadOnly = false;
		this.tab_Accounting_general.SettingInVisible = false;
		this.tab_Accounting_general.SettingReadonly = false;
		this.tab_Accounting_general.Size = new System.Drawing.Size(867, 375);
		this.tab_Accounting_general.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Accounting_general.Text = "General";
		this.lbl_ControlAccts.EditControl = null;
		this.lbl_ControlAccts.EditValue = "Control Accounts";
		this.lbl_ControlAccts.FieldName = null;
		this.lbl_ControlAccts.HideWhenEmpty = true;
		this.lbl_ControlAccts.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ControlAccts.Location = new System.Drawing.Point(9, 82);
		this.lbl_ControlAccts.Name = "lbl_ControlAccts";
		this.lbl_ControlAccts.ReadOnly = false;
		this.lbl_ControlAccts.SettingInVisible = false;
		this.lbl_ControlAccts.SettingReadonly = false;
		this.lbl_ControlAccts.ShowUnderLine = true;
		this.lbl_ControlAccts.Size = new System.Drawing.Size(89, 13);
		this.lbl_ControlAccts.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ControlAccts.TabIndex = 39;
		this.lbl_ControlAccts.TabStop = false;
		this.lbl_ControlAccts.Utext = "Control Accounts";
		this.lbl_ConnBPName.EditControl = null;
		this.lbl_ConnBPName.EditValue = "1";
		this.lbl_ConnBPName.FieldName = null;
		this.lbl_ConnBPName.HideWhenEmpty = true;
		this.lbl_ConnBPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ConnBPName.Location = new System.Drawing.Point(372, 250);
		this.lbl_ConnBPName.Name = "lbl_ConnBPName";
		this.lbl_ConnBPName.ReadOnly = false;
		this.lbl_ConnBPName.SettingInVisible = false;
		this.lbl_ConnBPName.SettingReadonly = false;
		this.lbl_ConnBPName.ShowUnderLine = false;
		this.lbl_ConnBPName.Size = new System.Drawing.Size(302, 20);
		this.lbl_ConnBPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ConnBPName.TabIndex = 38;
		this.lbl_ConnBPName.TabStop = false;
		this.lbl_ConnBPName.Utext = "1";
		this.lbl_ConSolidBPName.EditControl = null;
		this.lbl_ConSolidBPName.EditValue = "1";
		this.lbl_ConSolidBPName.FieldName = null;
		this.lbl_ConSolidBPName.HideWhenEmpty = true;
		this.lbl_ConSolidBPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ConSolidBPName.Location = new System.Drawing.Point(372, 6);
		this.lbl_ConSolidBPName.Name = "lbl_ConSolidBPName";
		this.lbl_ConSolidBPName.ReadOnly = false;
		this.lbl_ConSolidBPName.SettingInVisible = false;
		this.lbl_ConSolidBPName.SettingReadonly = false;
		this.lbl_ConSolidBPName.ShowUnderLine = false;
		this.lbl_ConSolidBPName.Size = new System.Drawing.Size(302, 20);
		this.lbl_ConSolidBPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ConSolidBPName.TabIndex = 37;
		this.lbl_ConSolidBPName.TabStop = false;
		this.lbl_ConSolidBPName.Utext = "1";
		this.lbl_InterimAcctName.EditControl = null;
		this.lbl_InterimAcctName.EditValue = "1";
		this.lbl_InterimAcctName.FieldName = null;
		this.lbl_InterimAcctName.HideWhenEmpty = true;
		this.lbl_InterimAcctName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_InterimAcctName.Location = new System.Drawing.Point(372, 139);
		this.lbl_InterimAcctName.Name = "lbl_InterimAcctName";
		this.lbl_InterimAcctName.ReadOnly = false;
		this.lbl_InterimAcctName.SettingInVisible = false;
		this.lbl_InterimAcctName.SettingReadonly = false;
		this.lbl_InterimAcctName.ShowUnderLine = false;
		this.lbl_InterimAcctName.Size = new System.Drawing.Size(302, 20);
		this.lbl_InterimAcctName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_InterimAcctName.TabIndex = 36;
		this.lbl_InterimAcctName.TabStop = false;
		this.lbl_InterimAcctName.Utext = "1";
		this.lbl_ClearAcctName.EditControl = null;
		this.lbl_ClearAcctName.EditValue = "1";
		this.lbl_ClearAcctName.FieldName = null;
		this.lbl_ClearAcctName.HideWhenEmpty = true;
		this.lbl_ClearAcctName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ClearAcctName.Location = new System.Drawing.Point(372, 119);
		this.lbl_ClearAcctName.Name = "lbl_ClearAcctName";
		this.lbl_ClearAcctName.ReadOnly = false;
		this.lbl_ClearAcctName.SettingInVisible = false;
		this.lbl_ClearAcctName.SettingReadonly = false;
		this.lbl_ClearAcctName.ShowUnderLine = false;
		this.lbl_ClearAcctName.Size = new System.Drawing.Size(302, 20);
		this.lbl_ClearAcctName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ClearAcctName.TabIndex = 35;
		this.lbl_ClearAcctName.TabStop = false;
		this.lbl_ClearAcctName.Utext = "1";
		this.lbl_AcctPayableName.EditControl = null;
		this.lbl_AcctPayableName.EditValue = "1";
		this.lbl_AcctPayableName.FieldName = null;
		this.lbl_AcctPayableName.HideWhenEmpty = true;
		this.lbl_AcctPayableName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_AcctPayableName.Location = new System.Drawing.Point(372, 100);
		this.lbl_AcctPayableName.Name = "lbl_AcctPayableName";
		this.lbl_AcctPayableName.ReadOnly = false;
		this.lbl_AcctPayableName.SettingInVisible = false;
		this.lbl_AcctPayableName.SettingReadonly = false;
		this.lbl_AcctPayableName.ShowUnderLine = false;
		this.lbl_AcctPayableName.Size = new System.Drawing.Size(302, 20);
		this.lbl_AcctPayableName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_AcctPayableName.TabIndex = 34;
		this.lbl_AcctPayableName.TabStop = false;
		this.lbl_AcctPayableName.Utext = "1";
		this.chk_BlockDunn.EditControl = null;
		this.chk_BlockDunn.FieldName = null;
		this.chk_BlockDunn.IsSearchControl = false;
		this.chk_BlockDunn.Location = new System.Drawing.Point(9, 174);
		this.chk_BlockDunn.Name = "chk_BlockDunn";
		this.chk_BlockDunn.Properties.Caption = "Block Dunning Letters";
		this.chk_BlockDunn.Properties.DisplayValueChecked = "Y";
		this.chk_BlockDunn.Properties.DisplayValueGrayed = "N";
		this.chk_BlockDunn.Properties.DisplayValueUnchecked = "N";
		this.chk_BlockDunn.Properties.ValueChecked = "Y";
		this.chk_BlockDunn.Properties.ValueGrayed = "N";
		this.chk_BlockDunn.Properties.ValueUnchecked = "N";
		this.chk_BlockDunn.SettingInVisible = false;
		this.chk_BlockDunn.SettingReadonly = false;
		this.chk_BlockDunn.Size = new System.Drawing.Size(126, 20);
		this.chk_BlockDunn.TabIndex = 6;
		this.chk_BlockDunn.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.dateX_DunnDate.EditControl = null;
		this.dateX_DunnDate.EditValue = null;
		this.dateX_DunnDate.FieldName = "DunnDate";
		this.dateX_DunnDate.Location = new System.Drawing.Point(188, 230);
		this.dateX_DunnDate.Name = "dateX_DunnDate";
		this.dateX_DunnDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateX_DunnDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateX_DunnDate.Properties.IsSearchControl = false;
		this.dateX_DunnDate.Properties.ReadOnly = true;
		this.dateX_DunnDate.SettingInVisible = false;
		this.dateX_DunnDate.SettingReadonly = false;
		this.dateX_DunnDate.Size = new System.Drawing.Size(178, 20);
		this.dateX_DunnDate.TabIndex = 32;
		this.calc_DunnLevel.EditControl = null;
		this.calc_DunnLevel.FieldName = "DunnLevel";
		this.calc_DunnLevel.IsSearchControl = false;
		this.calc_DunnLevel.Location = new System.Drawing.Point(188, 210);
		this.calc_DunnLevel.Name = "calc_DunnLevel";
		this.calc_DunnLevel.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(231, 231, 231);
		this.calc_DunnLevel.Properties.Appearance.Options.UseBackColor = true;
		this.calc_DunnLevel.Properties.AutoHeight = false;
		this.calc_DunnLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_DunnLevel.Properties.IsSearchControl = false;
		this.calc_DunnLevel.Properties.ReadOnly = true;
		this.calc_DunnLevel.SettingInVisible = false;
		this.calc_DunnLevel.SettingReadonly = false;
		this.calc_DunnLevel.Size = new System.Drawing.Size(178, 18);
		this.calc_DunnLevel.TabIndex = 31;
		this.calc_DunnLevel.TabStop = false;
		this.bte_ConnBP.EditControl = null;
		this.bte_ConnBP.FieldName = "ConnBP";
		this.bte_ConnBP.IsSearchControl = false;
		this.bte_ConnBP.Location = new System.Drawing.Point(188, 251);
		this.bte_ConnBP.Name = "bte_ConnBP";
		this.bte_ConnBP.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_ConnBP.Properties.Appearance.Options.UseFont = true;
		this.bte_ConnBP.Properties.AutoHeight = false;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.bte_ConnBP.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_ConnBP.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ConnBP_Properties_ButtonPressed);
		this.bte_ConnBP.SettingInVisible = false;
		this.bte_ConnBP.SettingReadonly = false;
		this.bte_ConnBP.Size = new System.Drawing.Size(178, 18);
		this.bte_ConnBP.TabIndex = 7;
		this.bte_ConnBP.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_UseShpdGd.EditControl = null;
		this.chk_UseShpdGd.FieldName = "UseShpdGd";
		this.chk_UseShpdGd.IsSearchControl = false;
		this.chk_UseShpdGd.Location = new System.Drawing.Point(9, 342);
		this.chk_UseShpdGd.Name = "chk_UseShpdGd";
		this.chk_UseShpdGd.Properties.Caption = "Use Shipped Goods Account";
		this.chk_UseShpdGd.Properties.DisplayValueChecked = "Y";
		this.chk_UseShpdGd.Properties.DisplayValueGrayed = "N";
		this.chk_UseShpdGd.Properties.DisplayValueUnchecked = "N";
		this.chk_UseShpdGd.Properties.ValueChecked = "Y";
		this.chk_UseShpdGd.Properties.ValueGrayed = "N";
		this.chk_UseShpdGd.Properties.ValueUnchecked = "N";
		this.chk_UseShpdGd.SettingInVisible = false;
		this.chk_UseShpdGd.SettingReadonly = false;
		this.chk_UseShpdGd.Size = new System.Drawing.Size(157, 20);
		this.chk_UseShpdGd.TabIndex = 10;
		this.chk_UseShpdGd.Visible = false;
		this.chk_UseShpdGd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_Affiliate.EditControl = null;
		this.chk_Affiliate.FieldName = "Affiliate";
		this.chk_Affiliate.IsSearchControl = false;
		this.chk_Affiliate.Location = new System.Drawing.Point(9, 322);
		this.chk_Affiliate.Name = "chk_Affiliate";
		this.chk_Affiliate.Properties.Caption = "Affiliate";
		this.chk_Affiliate.Properties.DisplayValueChecked = "Y";
		this.chk_Affiliate.Properties.DisplayValueGrayed = "N";
		this.chk_Affiliate.Properties.DisplayValueUnchecked = "N";
		this.chk_Affiliate.Properties.ValueChecked = "Y";
		this.chk_Affiliate.Properties.ValueGrayed = "N";
		this.chk_Affiliate.Properties.ValueUnchecked = "N";
		this.chk_Affiliate.SettingInVisible = false;
		this.chk_Affiliate.SettingReadonly = false;
		this.chk_Affiliate.Size = new System.Drawing.Size(60, 20);
		this.chk_Affiliate.TabIndex = 9;
		this.chk_Affiliate.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_PlngGroup.EditControl = null;
		this.txt_PlngGroup.FieldName = "PlngGroup";
		this.txt_PlngGroup.IsSearchControl = false;
		this.txt_PlngGroup.Location = new System.Drawing.Point(188, 300);
		this.txt_PlngGroup.Name = "txt_PlngGroup";
		this.txt_PlngGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_PlngGroup.Properties.Appearance.Options.UseFont = true;
		this.txt_PlngGroup.Properties.AutoHeight = false;
		this.txt_PlngGroup.SettingInVisible = false;
		this.txt_PlngGroup.SettingReadonly = false;
		this.txt_PlngGroup.Size = new System.Drawing.Size(178, 18);
		this.txt_PlngGroup.TabIndex = 8;
		this.txt_PlngGroup.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine26.EditControl = null;
		this.labelLine26.EditValue = "Planning Group";
		this.labelLine26.FieldName = null;
		this.labelLine26.HideWhenEmpty = false;
		this.labelLine26.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine26.Location = new System.Drawing.Point(9, 301);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.ReadOnly = false;
		this.labelLine26.SettingInVisible = false;
		this.labelLine26.SettingReadonly = false;
		this.labelLine26.ShowUnderLine = true;
		this.labelLine26.Size = new System.Drawing.Size(180, 20);
		this.labelLine26.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine26.TabIndex = 25;
		this.labelLine26.TabStop = false;
		this.labelLine26.Utext = "Planning Group";
		this.lbl_ConnBP.EditControl = null;
		this.lbl_ConnBP.EditValue = "Connected Customer";
		this.lbl_ConnBP.FieldName = null;
		this.lbl_ConnBP.HideWhenEmpty = false;
		this.lbl_ConnBP.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ConnBP.Location = new System.Drawing.Point(9, 253);
		this.lbl_ConnBP.Name = "lbl_ConnBP";
		this.lbl_ConnBP.ReadOnly = false;
		this.lbl_ConnBP.SettingInVisible = false;
		this.lbl_ConnBP.SettingReadonly = false;
		this.lbl_ConnBP.ShowUnderLine = true;
		this.lbl_ConnBP.Size = new System.Drawing.Size(180, 20);
		this.lbl_ConnBP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ConnBP.TabIndex = 23;
		this.lbl_ConnBP.TabStop = false;
		this.lbl_ConnBP.Utext = "Connected Customer";
		this.bte_DpmIntAct.EditControl = null;
		this.bte_DpmIntAct.FieldName = "DpmIntAct";
		this.bte_DpmIntAct.IsSearchControl = false;
		this.bte_DpmIntAct.Location = new System.Drawing.Point(188, 138);
		this.bte_DpmIntAct.Name = "bte_DpmIntAct";
		this.bte_DpmIntAct.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_DpmIntAct.Properties.Appearance.Options.UseFont = true;
		this.bte_DpmIntAct.Properties.AutoHeight = false;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.bte_DpmIntAct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DpmIntAct.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_DpmIntAct_Properties_ButtonPressed);
		this.bte_DpmIntAct.SettingInVisible = false;
		this.bte_DpmIntAct.SettingReadonly = false;
		this.bte_DpmIntAct.Size = new System.Drawing.Size(178, 18);
		this.bte_DpmIntAct.TabIndex = 5;
		this.bte_DpmIntAct.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine24.EditControl = null;
		this.labelLine24.EditValue = "Down Payment Interim Account";
		this.labelLine24.FieldName = null;
		this.labelLine24.HideWhenEmpty = false;
		this.labelLine24.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine24.Location = new System.Drawing.Point(9, 139);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.ReadOnly = false;
		this.labelLine24.SettingInVisible = false;
		this.labelLine24.SettingReadonly = false;
		this.labelLine24.ShowUnderLine = true;
		this.labelLine24.Size = new System.Drawing.Size(180, 20);
		this.labelLine24.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine24.TabIndex = 21;
		this.labelLine24.TabStop = false;
		this.labelLine24.Utext = "Down Payment Interim Account";
		this.bte_DpmClear.EditControl = null;
		this.bte_DpmClear.FieldName = "DpmClear";
		this.bte_DpmClear.IsSearchControl = false;
		this.bte_DpmClear.Location = new System.Drawing.Point(188, 119);
		this.bte_DpmClear.Name = "bte_DpmClear";
		this.bte_DpmClear.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_DpmClear.Properties.Appearance.Options.UseFont = true;
		this.bte_DpmClear.Properties.AutoHeight = false;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.bte_DpmClear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DpmClear.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_DpmClear_Properties_ButtonPressed);
		this.bte_DpmClear.SettingInVisible = false;
		this.bte_DpmClear.SettingReadonly = false;
		this.bte_DpmClear.Size = new System.Drawing.Size(178, 18);
		this.bte_DpmClear.TabIndex = 4;
		this.bte_DpmClear.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine23.EditControl = null;
		this.labelLine23.EditValue = "Down Payment Clearing Account";
		this.labelLine23.FieldName = null;
		this.labelLine23.HideWhenEmpty = false;
		this.labelLine23.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine23.Location = new System.Drawing.Point(9, 120);
		this.labelLine23.Name = "labelLine23";
		this.labelLine23.ReadOnly = false;
		this.labelLine23.SettingInVisible = false;
		this.labelLine23.SettingReadonly = false;
		this.labelLine23.ShowUnderLine = true;
		this.labelLine23.Size = new System.Drawing.Size(180, 20);
		this.labelLine23.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine23.TabIndex = 19;
		this.labelLine23.TabStop = false;
		this.labelLine23.Utext = "Down Payment Clearing Account";
		this.bte_DebPayAcct.EditControl = null;
		this.bte_DebPayAcct.FieldName = "DebPayAcct";
		this.bte_DebPayAcct.IsSearchControl = false;
		this.bte_DebPayAcct.Location = new System.Drawing.Point(188, 100);
		this.bte_DebPayAcct.Name = "bte_DebPayAcct";
		this.bte_DebPayAcct.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_DebPayAcct.Properties.Appearance.Options.UseFont = true;
		this.bte_DebPayAcct.Properties.AutoHeight = false;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.bte_DebPayAcct.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DebPayAcct.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_DebPayAcct_Properties_ButtonPressed);
		this.bte_DebPayAcct.SettingInVisible = false;
		this.bte_DebPayAcct.SettingReadonly = false;
		this.bte_DebPayAcct.Size = new System.Drawing.Size(178, 18);
		this.bte_DebPayAcct.TabIndex = 3;
		this.bte_DebPayAcct.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lbl_DebPayAcct.EditControl = null;
		this.lbl_DebPayAcct.EditValue = "Accounts Payable";
		this.lbl_DebPayAcct.FieldName = null;
		this.lbl_DebPayAcct.HideWhenEmpty = false;
		this.lbl_DebPayAcct.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DebPayAcct.Location = new System.Drawing.Point(9, 101);
		this.lbl_DebPayAcct.Name = "lbl_DebPayAcct";
		this.lbl_DebPayAcct.ReadOnly = false;
		this.lbl_DebPayAcct.SettingInVisible = false;
		this.lbl_DebPayAcct.SettingReadonly = false;
		this.lbl_DebPayAcct.ShowUnderLine = true;
		this.lbl_DebPayAcct.Size = new System.Drawing.Size(180, 20);
		this.lbl_DebPayAcct.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DebPayAcct.TabIndex = 17;
		this.lbl_DebPayAcct.TabStop = false;
		this.lbl_DebPayAcct.Utext = "Accounts Payable";
		this.btEx_ControlAccts.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btEx_ControlAccts.EditControl = null;
		this.btEx_ControlAccts.EditValue = "...";
		this.btEx_ControlAccts.FieldName = null;
		this.btEx_ControlAccts.IsSearchControl = false;
		this.btEx_ControlAccts.Location = new System.Drawing.Point(188, 75);
		this.btEx_ControlAccts.Name = "btEx_ControlAccts";
		this.btEx_ControlAccts.ReadOnly = false;
		this.btEx_ControlAccts.SettingInVisible = false;
		this.btEx_ControlAccts.SettingReadonly = false;
		this.btEx_ControlAccts.Size = new System.Drawing.Size(29, 20);
		this.btEx_ControlAccts.TabIndex = 2;
		this.btEx_ControlAccts.Text = "...";
		this.btEx_ControlAccts.Click += new System.EventHandler(this.btEx_ControlAccts_Click);
		this.rdg_FatherType.EditControl = null;
		this.rdg_FatherType.FieldName = "FatherType";
		this.rdg_FatherType.IsSearchControl = false;
		this.rdg_FatherType.Location = new System.Drawing.Point(9, 28);
		this.rdg_FatherType.Name = "rdg_FatherType";
		this.rdg_FatherType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_FatherType.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_FatherType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_FatherType.Properties.Columns = 2;
		this.rdg_FatherType.Properties.ItemHorzAlignment = DevExpress.XtraEditors.RadioItemHorzAlignment.Near;
		this.rdg_FatherType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("P", "Payment Consolidation"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("D", "Delivery Consolidation")
		});
		this.rdg_FatherType.SettingInVisible = false;
		this.rdg_FatherType.SettingReadonly = false;
		this.rdg_FatherType.Size = new System.Drawing.Size(347, 18);
		this.rdg_FatherType.TabIndex = 1;
		this.rdg_FatherType.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bte_FatherCard.EditControl = null;
		this.bte_FatherCard.FieldName = "FatherCard";
		this.bte_FatherCard.IsSearchControl = false;
		this.bte_FatherCard.Location = new System.Drawing.Point(188, 6);
		this.bte_FatherCard.Name = "bte_FatherCard";
		this.bte_FatherCard.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_FatherCard.Properties.Appearance.Options.UseFont = true;
		this.bte_FatherCard.Properties.AutoHeight = false;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.bte_FatherCard.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_FatherCard.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_FatherCard_Properties_ButtonPressed);
		this.bte_FatherCard.SettingInVisible = false;
		this.bte_FatherCard.SettingReadonly = false;
		this.bte_FatherCard.Size = new System.Drawing.Size(178, 18);
		this.bte_FatherCard.TabIndex = 0;
		this.bte_FatherCard.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Consolidating BP";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(9, 7);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(180, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 9;
		this.labelLine8.TabStop = false;
		this.labelLine8.Utext = "Consolidating BP";
		this.lbl_DunnDate.EditControl = null;
		this.lbl_DunnDate.EditValue = "Dunning Date";
		this.lbl_DunnDate.FieldName = null;
		this.lbl_DunnDate.HideWhenEmpty = false;
		this.lbl_DunnDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DunnDate.Location = new System.Drawing.Point(9, 233);
		this.lbl_DunnDate.Name = "lbl_DunnDate";
		this.lbl_DunnDate.ReadOnly = false;
		this.lbl_DunnDate.SettingInVisible = false;
		this.lbl_DunnDate.SettingReadonly = false;
		this.lbl_DunnDate.ShowUnderLine = true;
		this.lbl_DunnDate.Size = new System.Drawing.Size(180, 20);
		this.lbl_DunnDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DunnDate.TabIndex = 29;
		this.lbl_DunnDate.TabStop = false;
		this.lbl_DunnDate.Utext = "Dunning Date";
		this.lbl_DunnLevel.EditControl = null;
		this.lbl_DunnLevel.EditValue = "Dunning Level";
		this.lbl_DunnLevel.FieldName = null;
		this.lbl_DunnLevel.HideWhenEmpty = false;
		this.lbl_DunnLevel.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DunnLevel.Location = new System.Drawing.Point(9, 211);
		this.lbl_DunnLevel.Name = "lbl_DunnLevel";
		this.lbl_DunnLevel.ReadOnly = false;
		this.lbl_DunnLevel.SettingInVisible = false;
		this.lbl_DunnLevel.SettingReadonly = false;
		this.lbl_DunnLevel.ShowUnderLine = true;
		this.lbl_DunnLevel.Size = new System.Drawing.Size(180, 20);
		this.lbl_DunnLevel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DunnLevel.TabIndex = 30;
		this.lbl_DunnLevel.TabStop = false;
		this.lbl_DunnLevel.Utext = "Dunning Level";
		this.tab_Accounting_tax.Controls.Add(this.chk_DeferrTax);
		this.tab_Accounting_tax.Controls.Add(this.bte_ECVatGroup);
		this.tab_Accounting_tax.Controls.Add(this.labelLine28);
		this.tab_Accounting_tax.Controls.Add(this.txt_ECVatGroupName);
		this.tab_Accounting_tax.Controls.Add(this.luk_VatStatus);
		this.tab_Accounting_tax.Controls.Add(this.labelLine27);
		this.tab_Accounting_tax.EditControl = null;
		this.tab_Accounting_tax.EditValue = "Tax";
		this.tab_Accounting_tax.FieldName = "tab_Accounting_tax";
		this.tab_Accounting_tax.IsSearchControl = false;
		this.tab_Accounting_tax.Name = "tab_Accounting_tax";
		this.tab_Accounting_tax.ReadOnly = false;
		this.tab_Accounting_tax.SettingInVisible = false;
		this.tab_Accounting_tax.SettingReadonly = false;
		this.tab_Accounting_tax.Size = new System.Drawing.Size(867, 375);
		this.tab_Accounting_tax.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Accounting_tax.Text = "Tax";
		this.chk_DeferrTax.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_DeferrTax.EditControl = null;
		this.chk_DeferrTax.FieldName = null;
		this.chk_DeferrTax.IsSearchControl = false;
		this.chk_DeferrTax.Location = new System.Drawing.Point(11, 197);
		this.chk_DeferrTax.Name = "chk_DeferrTax";
		this.chk_DeferrTax.Properties.Caption = "Deferred Tax";
		this.chk_DeferrTax.Properties.ValueChecked = "Y";
		this.chk_DeferrTax.Properties.ValueGrayed = "";
		this.chk_DeferrTax.Properties.ValueUnchecked = "N";
		this.chk_DeferrTax.SettingInVisible = false;
		this.chk_DeferrTax.SettingReadonly = false;
		this.chk_DeferrTax.Size = new System.Drawing.Size(220, 20);
		this.chk_DeferrTax.TabIndex = 28;
		this.bte_ECVatGroup.EditControl = null;
		this.bte_ECVatGroup.FieldName = "ECVatGroup";
		this.bte_ECVatGroup.IsSearchControl = false;
		this.bte_ECVatGroup.Location = new System.Drawing.Point(188, 26);
		this.bte_ECVatGroup.Name = "bte_ECVatGroup";
		this.bte_ECVatGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_ECVatGroup.Properties.Appearance.Options.UseFont = true;
		this.bte_ECVatGroup.Properties.AutoHeight = false;
		this.bte_ECVatGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ECVatGroup.SettingInVisible = false;
		this.bte_ECVatGroup.SettingReadonly = false;
		this.bte_ECVatGroup.Size = new System.Drawing.Size(43, 18);
		this.bte_ECVatGroup.TabIndex = 11;
		this.bte_ECVatGroup.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ECVatGroup_ButtonPressed);
		this.bte_ECVatGroup.EditValueChanged += new System.EventHandler(this.bte_ECVatGroup_EditValueChanged);
		this.labelLine28.EditControl = null;
		this.labelLine28.EditValue = "Tax Group";
		this.labelLine28.FieldName = null;
		this.labelLine28.HideWhenEmpty = false;
		this.labelLine28.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine28.Location = new System.Drawing.Point(9, 27);
		this.labelLine28.Name = "labelLine28";
		this.labelLine28.ReadOnly = false;
		this.labelLine28.SettingInVisible = false;
		this.labelLine28.SettingReadonly = false;
		this.labelLine28.ShowUnderLine = false;
		this.labelLine28.Size = new System.Drawing.Size(180, 20);
		this.labelLine28.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine28.TabIndex = 10;
		this.labelLine28.TabStop = false;
		this.labelLine28.Utext = "Tax Group";
		this.txt_ECVatGroupName.EditControl = null;
		this.txt_ECVatGroupName.FieldName = "ECVatGroupName";
		this.txt_ECVatGroupName.IsSearchControl = false;
		this.txt_ECVatGroupName.Location = new System.Drawing.Point(233, 26);
		this.txt_ECVatGroupName.Name = "txt_ECVatGroupName";
		this.txt_ECVatGroupName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_ECVatGroupName.Properties.Appearance.Options.UseFont = true;
		this.txt_ECVatGroupName.Properties.AutoHeight = false;
		this.txt_ECVatGroupName.SettingInVisible = false;
		this.txt_ECVatGroupName.SettingReadonly = false;
		this.txt_ECVatGroupName.Size = new System.Drawing.Size(185, 18);
		this.txt_ECVatGroupName.TabIndex = 9;
		this.luk_VatStatus.EditControl = null;
		this.luk_VatStatus.FieldName = "VatStatus";
		this.luk_VatStatus.IsSearchControl = false;
		this.luk_VatStatus.Location = new System.Drawing.Point(188, 6);
		this.luk_VatStatus.Name = "luk_VatStatus";
		this.luk_VatStatus.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_VatStatus.Properties.Appearance.Options.UseFont = true;
		this.luk_VatStatus.Properties.AutoHeight = false;
		this.luk_VatStatus.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions11.Image");
		this.luk_VatStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_VatStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "SeriesName")
		});
		this.luk_VatStatus.Properties.DisplayMember = "SeriesName";
		this.luk_VatStatus.Properties.IsSearchControl = false;
		this.luk_VatStatus.Properties.NullText = "";
		this.luk_VatStatus.Properties.ShowFooter = false;
		this.luk_VatStatus.Properties.ShowHeader = false;
		this.luk_VatStatus.Properties.ValueMember = "Series";
		this.luk_VatStatus.SettingInVisible = false;
		this.luk_VatStatus.SettingReadonly = false;
		this.luk_VatStatus.Size = new System.Drawing.Size(231, 18);
		this.luk_VatStatus.TabIndex = 3;
		this.labelLine27.EditControl = null;
		this.labelLine27.EditValue = "Tax Status";
		this.labelLine27.FieldName = null;
		this.labelLine27.HideWhenEmpty = false;
		this.labelLine27.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine27.Location = new System.Drawing.Point(9, 7);
		this.labelLine27.Name = "labelLine27";
		this.labelLine27.ReadOnly = false;
		this.labelLine27.SettingInVisible = false;
		this.labelLine27.SettingReadonly = false;
		this.labelLine27.ShowUnderLine = false;
		this.labelLine27.Size = new System.Drawing.Size(180, 20);
		this.labelLine27.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine27.TabIndex = 2;
		this.labelLine27.TabStop = false;
		this.labelLine27.Utext = "Tax Status";
		this.tab_Properties.Controls.Add(this.btn_PropertiesClearSelect);
		this.tab_Properties.Controls.Add(this.btn_PropertiesSellectAll);
		this.tab_Properties.Controls.Add(this.grd_Properties);
		this.tab_Properties.EditControl = null;
		this.tab_Properties.EditValue = "Properties";
		this.tab_Properties.FieldName = "tab_Properties";
		this.tab_Properties.IsSearchControl = false;
		this.tab_Properties.Name = "tab_Properties";
		this.tab_Properties.ReadOnly = false;
		this.tab_Properties.SettingInVisible = false;
		this.tab_Properties.SettingReadonly = false;
		this.tab_Properties.Size = new System.Drawing.Size(871, 399);
		this.tab_Properties.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Properties.Text = "Properties";
		this.btn_PropertiesClearSelect.EditControl = null;
		this.btn_PropertiesClearSelect.EditValue = "Clear Selection";
		this.btn_PropertiesClearSelect.FieldName = null;
		this.btn_PropertiesClearSelect.IsSearchControl = false;
		this.btn_PropertiesClearSelect.Location = new System.Drawing.Point(448, 35);
		this.btn_PropertiesClearSelect.Name = "btn_PropertiesClearSelect";
		this.btn_PropertiesClearSelect.ReadOnly = false;
		this.btn_PropertiesClearSelect.SettingInVisible = false;
		this.btn_PropertiesClearSelect.SettingReadonly = false;
		this.btn_PropertiesClearSelect.Size = new System.Drawing.Size(124, 23);
		this.btn_PropertiesClearSelect.TabIndex = 2;
		this.btn_PropertiesClearSelect.Text = "Clear Selection";
		this.btn_PropertiesClearSelect.Click += new System.EventHandler(this.btn_PropertiesClearSelect_Click);
		this.btn_PropertiesSellectAll.EditControl = null;
		this.btn_PropertiesSellectAll.EditValue = "Select All";
		this.btn_PropertiesSellectAll.FieldName = null;
		this.btn_PropertiesSellectAll.IsSearchControl = false;
		this.btn_PropertiesSellectAll.Location = new System.Drawing.Point(448, 6);
		this.btn_PropertiesSellectAll.Name = "btn_PropertiesSellectAll";
		this.btn_PropertiesSellectAll.ReadOnly = false;
		this.btn_PropertiesSellectAll.SettingInVisible = false;
		this.btn_PropertiesSellectAll.SettingReadonly = false;
		this.btn_PropertiesSellectAll.Size = new System.Drawing.Size(124, 23);
		this.btn_PropertiesSellectAll.TabIndex = 1;
		this.btn_PropertiesSellectAll.Text = "Select All";
		this.btn_PropertiesSellectAll.Click += new System.EventHandler(this.btn_PropertiesSellectAll_Click);
		this.grd_Properties.AllowPasteExcel = false;
		this.grd_Properties.ClipboardData = null;
		this.grd_Properties.Location = new System.Drawing.Point(9, 6);
		this.grd_Properties.MainView = this.gv_Properties;
		this.grd_Properties.Name = "grd_Properties";
		this.grd_Properties.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_PropertiesCheck });
		this.grd_Properties.Size = new System.Drawing.Size(400, 339);
		this.grd_Properties.TabIndex = 0;
		this.grd_Properties.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Properties });
		this.gv_Properties.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Properties.ColumnKey = null;
		this.gv_Properties.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gv_Properties.DisableFormSetting = false;
		this.gv_Properties.DisplayException = false;
		this.gv_Properties.EditColumn = null;
		this.gv_Properties.FieldCurrency = null;
		this.gv_Properties.FieldRate = null;
		this.gv_Properties.FormId = "oBussinessPartnerMasterData";
		this.gv_Properties.GridControl = this.grd_Properties;
		this.gv_Properties.HasFormSetting = false;
		this.gv_Properties.IgnoreFilter = false;
		this.gv_Properties.IndexEditRow = -1;
		this.gv_Properties.IndicatorWidth = 30;
		this.gv_Properties.IsApzTable = false;
		this.gv_Properties.IsInsertNewRow = false;
		this.gv_Properties.IsLoadingData = false;
		this.gv_Properties.IsMappingContextAction = false;
		this.gv_Properties.ItemId = "OPROPETIES";
		this.gv_Properties.LstColumnAllowBatch = null;
		this.gv_Properties.LstColumnNotAllowBatch = null;
		this.gv_Properties.LstColumnNotEditable = null;
		this.gv_Properties.LstColumNotSetting = null;
		this.gv_Properties.LstOldUDF = null;
		this.gv_Properties.Name = "gv_Properties";
		this.gv_Properties.OptionsNavigation.UseTabKey = false;
		this.gv_Properties.OptionsView.ColumnAutoWidth = false;
		this.gv_Properties.OptionsView.ShowGroupPanel = false;
		this.gv_Properties.UseBatchMod = false;
		this.gv_Properties.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Properties_CustomDrawRowIndicator);
		this.gv_Properties.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Controls_EditValueChanged);
		this.gridColumn1.Caption = "Number";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "GroupCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Property Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "GroupName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 277;
		this.gridColumn3.ColumnEdit = this.rep_PropertiesCheck;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Check";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.rep_PropertiesCheck.AutoHeight = false;
		this.rep_PropertiesCheck.Name = "rep_PropertiesCheck";
		this.rep_PropertiesCheck.ValueChecked = "Y";
		this.rep_PropertiesCheck.ValueUnchecked = "N";
		this.rep_PropertiesCheck.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.tab_Remark.Controls.Add(this.btn_PicDelete);
		this.tab_Remark.Controls.Add(this.memoRemarks);
		this.tab_Remark.Controls.Add(this.btnRemarkBrowserImage);
		this.tab_Remark.Controls.Add(this.picRemarks);
		this.tab_Remark.EditControl = null;
		this.tab_Remark.EditValue = "&Remarks";
		this.tab_Remark.FieldName = "tab_Remark";
		this.tab_Remark.IsSearchControl = false;
		this.tab_Remark.Name = "tab_Remark";
		this.tab_Remark.ReadOnly = false;
		this.tab_Remark.SettingInVisible = false;
		this.tab_Remark.SettingReadonly = false;
		this.tab_Remark.Size = new System.Drawing.Size(871, 399);
		this.tab_Remark.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Remark.Text = "&Remarks";
		this.btn_PicDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_PicDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_PicDelete.EditControl = null;
		this.btn_PicDelete.EditValue = "";
		this.btn_PicDelete.FieldName = null;
		this.btn_PicDelete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PicDelete.ImageOptions.Image");
		this.btn_PicDelete.IsSearchControl = false;
		this.btn_PicDelete.Location = new System.Drawing.Point(322, 265);
		this.btn_PicDelete.Name = "btn_PicDelete";
		this.btn_PicDelete.ReadOnly = false;
		this.btn_PicDelete.SettingInVisible = false;
		this.btn_PicDelete.SettingReadonly = false;
		this.btn_PicDelete.Size = new System.Drawing.Size(23, 25);
		this.btn_PicDelete.TabIndex = 3;
		this.btn_PicDelete.Click += new System.EventHandler(this.btn_PicDelete_Click);
		this.memoRemarks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoRemarks.EditControl = null;
		this.memoRemarks.FieldName = "Free_Text";
		this.memoRemarks.IsSearchControl = false;
		this.memoRemarks.Location = new System.Drawing.Point(322, 3);
		this.memoRemarks.Name = "memoRemarks";
		this.memoRemarks.SettingInVisible = false;
		this.memoRemarks.SettingReadonly = false;
		this.memoRemarks.Size = new System.Drawing.Size(544, 219);
		this.memoRemarks.TabIndex = 1;
		this.memoRemarks.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.btnRemarkBrowserImage.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btnRemarkBrowserImage.EditControl = null;
		this.btnRemarkBrowserImage.EditValue = "...";
		this.btnRemarkBrowserImage.FieldName = null;
		this.btnRemarkBrowserImage.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRemarkBrowserImage.ImageOptions.Image");
		this.btnRemarkBrowserImage.IsSearchControl = false;
		this.btnRemarkBrowserImage.Location = new System.Drawing.Point(322, 228);
		this.btnRemarkBrowserImage.Name = "btnRemarkBrowserImage";
		this.btnRemarkBrowserImage.ReadOnly = false;
		this.btnRemarkBrowserImage.SettingInVisible = false;
		this.btnRemarkBrowserImage.SettingReadonly = false;
		this.btnRemarkBrowserImage.Size = new System.Drawing.Size(23, 25);
		this.btnRemarkBrowserImage.TabIndex = 2;
		this.btnRemarkBrowserImage.Text = "...";
		this.btnRemarkBrowserImage.Click += new System.EventHandler(this.btnRemarkBrowserImage_Click);
		this.picRemarks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.picRemarks.Cursor = System.Windows.Forms.Cursors.Default;
		this.picRemarks.Location = new System.Drawing.Point(3, 3);
		this.picRemarks.Name = "picRemarks";
		this.picRemarks.Properties.PictureStoreMode = DevExpress.XtraEditors.Controls.PictureStoreMode.ByteArray;
		this.picRemarks.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
		this.picRemarks.Size = new System.Drawing.Size(313, 394);
		this.picRemarks.TabIndex = 0;
		this.picRemarks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRemarks_MouseDown);
		this.picRemarks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picRemarks_MouseUp);
		this.tab_AttachmentFiles.Controls.Add(this.ucAttachment1);
		this.tab_AttachmentFiles.EditControl = null;
		this.tab_AttachmentFiles.EditValue = "Attachments";
		this.tab_AttachmentFiles.FieldName = "tab_AttachmentFiles";
		this.tab_AttachmentFiles.IsSearchControl = false;
		this.tab_AttachmentFiles.Name = "tab_AttachmentFiles";
		this.tab_AttachmentFiles.ReadOnly = false;
		this.tab_AttachmentFiles.SettingInVisible = false;
		this.tab_AttachmentFiles.SettingReadonly = false;
		this.tab_AttachmentFiles.Size = new System.Drawing.Size(871, 399);
		this.tab_AttachmentFiles.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_AttachmentFiles.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oBussinessPartnerMasterData";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(871, 399);
		this.ucAttachment1.TabIndex = 0;
		this.tab_UserDefineF.Controls.Add(this.grd_Udf);
		this.tab_UserDefineF.EditControl = null;
		this.tab_UserDefineF.EditValue = "User Defined Fields";
		this.tab_UserDefineF.FieldName = "tab_UserDefineF";
		this.tab_UserDefineF.IsSearchControl = false;
		this.tab_UserDefineF.Name = "tab_UserDefineF";
		this.tab_UserDefineF.ReadOnly = false;
		this.tab_UserDefineF.SettingInVisible = false;
		this.tab_UserDefineF.SettingReadonly = false;
		this.tab_UserDefineF.Size = new System.Drawing.Size(871, 399);
		this.tab_UserDefineF.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_UserDefineF.Text = "User Defined Fields";
		this.grd_Udf.AllowPasteExcel = true;
		this.grd_Udf.ClipboardData = null;
		this.grd_Udf.ContextMenuStrip = this.ctMenu_Contact;
		this.grd_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		gridLevelNode.RelationName = "Level1";
		this.grd_Udf.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.grd_Udf.Location = new System.Drawing.Point(0, 0);
		this.grd_Udf.MainView = this.gv_Udf;
		this.grd_Udf.Name = "grd_Udf";
		this.grd_Udf.Size = new System.Drawing.Size(871, 399);
		this.grd_Udf.TabIndex = 0;
		this.grd_Udf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Udf });
		this.gv_Udf.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Udf.ColumnKey = null;
		this.gv_Udf.DisableFormSetting = false;
		this.gv_Udf.DisplayException = false;
		this.gv_Udf.EditColumn = null;
		this.gv_Udf.FieldCurrency = null;
		this.gv_Udf.FieldRate = null;
		this.gv_Udf.FormId = "oBussinessPartnerMasterData";
		this.gv_Udf.GridControl = this.grd_Udf;
		this.gv_Udf.HasFormSetting = false;
		this.gv_Udf.IgnoreFilter = false;
		this.gv_Udf.IndexEditRow = -1;
		this.gv_Udf.IndicatorWidth = 35;
		this.gv_Udf.IsApzTable = false;
		this.gv_Udf.IsInsertNewRow = false;
		this.gv_Udf.IsLoadingData = false;
		this.gv_Udf.IsMappingContextAction = false;
		this.gv_Udf.ItemId = "OUDF";
		this.gv_Udf.LstColumnAllowBatch = null;
		this.gv_Udf.LstColumnNotAllowBatch = null;
		this.gv_Udf.LstColumnNotEditable = null;
		this.gv_Udf.LstColumNotSetting = null;
		this.gv_Udf.LstOldUDF = null;
		this.gv_Udf.Name = "gv_Udf";
		this.gv_Udf.OptionsNavigation.UseTabKey = false;
		this.gv_Udf.OptionsView.ColumnAutoWidth = false;
		this.gv_Udf.OptionsView.ShowGroupPanel = false;
		this.gv_Udf.UseBatchMod = false;
		this.gv_Udf.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Controls_EditValueChanged);
		this.luk_Currency.EditControl = null;
		this.luk_Currency.FieldName = "Currency";
		this.luk_Currency.IsSearchControl = false;
		this.luk_Currency.Location = new System.Drawing.Point(164, 84);
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Currency.Properties.Appearance.Options.UseFont = true;
		this.luk_Currency.Properties.AutoHeight = false;
		this.luk_Currency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Currency.Properties.DisplayMember = "Name";
		this.luk_Currency.Properties.IsSearchControl = false;
		this.luk_Currency.Properties.NullText = "";
		this.luk_Currency.Properties.ValueMember = "Code";
		this.luk_Currency.SettingInVisible = false;
		this.luk_Currency.SettingReadonly = false;
		this.luk_Currency.Size = new System.Drawing.Size(207, 18);
		this.luk_Currency.TabIndex = 7;
		this.luk_Currency.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_Currency.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Currency_EditValueChanging);
		this.contextMenuStrip_BP.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeBusinessPartnerToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.contextMenuStrip_BP.Name = "contextMenuStrip_BP";
		this.contextMenuStrip_BP.Size = new System.Drawing.Size(207, 48);
		this.contextMenuStrip_BP.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_BP_Opening);
		this.removeBusinessPartnerToolStripMenuItem.Name = "removeBusinessPartnerToolStripMenuItem";
		this.removeBusinessPartnerToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
		this.removeBusinessPartnerToolStripMenuItem.Text = "Remove Business Partner";
		this.removeBusinessPartnerToolStripMenuItem.Click += new System.EventHandler(this.removeBusinessPartnerToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.cb_CurrencyType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cb_CurrencyType.EditControl = null;
		this.cb_CurrencyType.FieldName = null;
		this.cb_CurrencyType.IsSearchControl = false;
		this.cb_CurrencyType.Location = new System.Drawing.Point(647, 2);
		this.cb_CurrencyType.Name = "cb_CurrencyType";
		this.cb_CurrencyType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_CurrencyType.Properties.Appearance.Options.UseFont = true;
		this.cb_CurrencyType.Properties.AutoHeight = false;
		this.cb_CurrencyType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_CurrencyType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_CurrencyType.SettingInVisible = false;
		this.cb_CurrencyType.SettingReadonly = false;
		this.cb_CurrencyType.Size = new System.Drawing.Size(206, 18);
		this.cb_CurrencyType.TabIndex = 9;
		this.cb_CurrencyType.SelectedIndexChanged += new System.EventHandler(this.cb_CurrencyType_SelectedIndexChanged);
		this.luk_Series.EditControl = null;
		this.luk_Series.FieldName = "Series";
		this.luk_Series.IsSearchControl = false;
		this.luk_Series.Location = new System.Drawing.Point(75, 6);
		this.luk_Series.Name = "luk_Series";
		this.luk_Series.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Series.Properties.Appearance.Options.UseFont = true;
		this.luk_Series.Properties.AutoHeight = false;
		this.luk_Series.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.luk_Series.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_Series.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "SeriesName")
		});
		this.luk_Series.Properties.DisplayMember = "SeriesName";
		this.luk_Series.Properties.IsSearchControl = false;
		this.luk_Series.Properties.NullText = "";
		this.luk_Series.Properties.ShowFooter = false;
		this.luk_Series.Properties.ShowHeader = false;
		this.luk_Series.Properties.ValueMember = "Series";
		this.luk_Series.SettingInVisible = false;
		this.luk_Series.SettingReadonly = false;
		this.luk_Series.Size = new System.Drawing.Size(86, 18);
		this.luk_Series.TabIndex = 1;
		this.luk_Series.EditValueChanged += new System.EventHandler(this.luk_DocCode_EditValueChanged);
		this.luk_Series.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_DocCode_EditValueChanging);
		this.luk_CardType.EditControl = null;
		this.luk_CardType.FieldName = "CardType";
		this.luk_CardType.IsSearchControl = false;
		this.luk_CardType.Location = new System.Drawing.Point(286, 6);
		this.luk_CardType.Name = "luk_CardType";
		this.luk_CardType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_CardType.Properties.Appearance.Options.UseFont = true;
		this.luk_CardType.Properties.AutoHeight = false;
		this.luk_CardType.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		editorButtonImageOptions13.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions13.Image");
		this.luk_CardType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_CardType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_CardType.Properties.DisplayMember = "Name";
		this.luk_CardType.Properties.IsSearchControl = false;
		this.luk_CardType.Properties.NullText = "";
		this.luk_CardType.Properties.ShowFooter = false;
		this.luk_CardType.Properties.ShowHeader = false;
		this.luk_CardType.Properties.ValueMember = "Code";
		this.luk_CardType.SettingInVisible = false;
		this.luk_CardType.SettingReadonly = false;
		this.luk_CardType.Size = new System.Drawing.Size(85, 18);
		this.luk_CardType.TabIndex = 3;
		this.luk_CardType.EditValueChanged += new System.EventHandler(this.luk_BPType_EditValueChanged);
		this.luk_CardType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_BPType_EditValueChanging);
		this.luk_CardGroup.EditControl = null;
		this.luk_CardGroup.FieldName = "GroupCode";
		this.luk_CardGroup.IsSearchControl = false;
		this.luk_CardGroup.Location = new System.Drawing.Point(164, 63);
		this.luk_CardGroup.Name = "luk_CardGroup";
		this.luk_CardGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_CardGroup.Properties.Appearance.Options.UseFont = true;
		this.luk_CardGroup.Properties.AutoHeight = false;
		this.luk_CardGroup.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_CardGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CardGroup.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupName", "GroupName")
		});
		this.luk_CardGroup.Properties.DisplayMember = "GroupName";
		this.luk_CardGroup.Properties.IsSearchControl = false;
		this.luk_CardGroup.Properties.NullText = "";
		this.luk_CardGroup.Properties.ShowFooter = false;
		this.luk_CardGroup.Properties.ShowHeader = false;
		this.luk_CardGroup.Properties.ValueMember = "GroupCode";
		this.luk_CardGroup.SettingInVisible = false;
		this.luk_CardGroup.SettingReadonly = false;
		this.luk_CardGroup.Size = new System.Drawing.Size(207, 20);
		this.luk_CardGroup.TabIndex = 6;
		this.luk_CardGroup.EditValueChanged += new System.EventHandler(this.luk_CardGroup_EditValueChanged);
		this.luk_CardGroup.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_CardGroup_EditValueChanging);
		this.lbOwner.EditControl = null;
		this.lbOwner.EditValue = "Owner";
		this.lbOwner.FieldName = null;
		this.lbOwner.HideWhenEmpty = false;
		this.lbOwner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOwner.Location = new System.Drawing.Point(14, 123);
		this.lbOwner.Name = "lbOwner";
		this.lbOwner.ReadOnly = false;
		this.lbOwner.SettingInVisible = false;
		this.lbOwner.SettingReadonly = false;
		this.lbOwner.ShowUnderLine = true;
		this.lbOwner.Size = new System.Drawing.Size(153, 20);
		this.lbOwner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOwner.TabIndex = 75;
		this.lbOwner.TabStop = false;
		this.lbOwner.Utext = "Owner";
		this.bteCFL_OwnerCode.EditControl = null;
		this.bteCFL_OwnerCode.FieldName = "OwnerCode";
		this.bteCFL_OwnerCode.IsSearchControl = false;
		this.bteCFL_OwnerCode.Location = new System.Drawing.Point(164, 122);
		this.bteCFL_OwnerCode.Name = "bteCFL_OwnerCode";
		this.bteCFL_OwnerCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bteCFL_OwnerCode.Properties.Appearance.Options.UseFont = true;
		this.bteCFL_OwnerCode.Properties.AutoHeight = false;
		this.bteCFL_OwnerCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions14.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.bteCFL_OwnerCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteCFL_OwnerCode.Properties.ContainerControl = null;
		this.bteCFL_OwnerCode.Properties.DisplayMember = "lastName";
		this.bteCFL_OwnerCode.Properties.DisplayMember2 = "firstName";
		this.bteCFL_OwnerCode.Properties.LoadDataSourceWhenEnter = false;
		this.bteCFL_OwnerCode.Properties.SeparateCharacterForMulti = ';';
		this.bteCFL_OwnerCode.Properties.SeparateString = ", ";
		this.bteCFL_OwnerCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.EmployeeListForSalesEmpSetup;
		this.bteCFL_OwnerCode.Properties.UsingAutoCompleteBox = false;
		this.bteCFL_OwnerCode.Properties.UsingDataSource = true;
		this.bteCFL_OwnerCode.Properties.ValidateWhenNoDataSource = false;
		this.bteCFL_OwnerCode.Properties.ValueMember = "EmpId";
		this.bteCFL_OwnerCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bteCFL_OwnerCode_Properties_ButtonClick);
		this.bteCFL_OwnerCode.SettingInVisible = false;
		this.bteCFL_OwnerCode.SettingReadonly = false;
		this.bteCFL_OwnerCode.Size = new System.Drawing.Size(207, 18);
		this.bteCFL_OwnerCode.TabIndex = 76;
		this.bteCFL_OwnerCode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(906, 600);
		this.ContextMenuStrip = this.contextMenuStrip_BP;
		base.Controls.Add(this.bteCFL_OwnerCode);
		base.Controls.Add(this.lbOwner);
		base.Controls.Add(this.luk_CardGroup);
		base.Controls.Add(this.luk_CardType);
		base.Controls.Add(this.luk_Series);
		base.Controls.Add(this.cb_CurrencyType);
		base.Controls.Add(this.luk_Currency);
		base.Controls.Add(this.xTab_BP);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.txtFederalTaxID);
		base.Controls.Add(this.txt_CardName);
		base.Controls.Add(this.txtForeignName);
		base.Controls.Add(this.txt_CardCode);
		base.Controls.Add(this.lb_FederalTaxID);
		base.Controls.Add(this.lb_Currency);
		base.Controls.Add(this.lb_Group);
		base.Controls.Add(this.lb_Name);
		base.Controls.Add(this.lb_ForeignName);
		base.Controls.Add(this.lb_Code);
		base.FormId = "oBussinessPartnerMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BusinessMasterData";
		this.Text = "Business Partner Master Data";
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtForeignName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFederalTaxID.Properties).EndInit();
		this.ctMenu_Contact.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xTab_BP).EndInit();
		this.xTab_BP.ResumeLayout(false);
		this.tab_General.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtGeneralRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Technician.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BPIndustry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GeneralAliasName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BusinessType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPProject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FactoringIndicator.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ShippingType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralEmail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralWebsite.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralFax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralTel2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralMobilePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralTel1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Territory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Language.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPChannelCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SalesBuyer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralFederalTaxID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralContactPerson.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralIDNo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtGeneralPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FactoringIndicator.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).EndInit();
		this.panel_Advanced.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties).EndInit();
		this.panel_Inactive.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom.Properties).EndInit();
		this.panel_Active.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo.Properties).EndInit();
		this.tab_ContactPerson.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_BirthPlace.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BirthDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BirthDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Gender.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Profession.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BirthCity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Notes1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Notes2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pager.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_E_MailL.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Cellolar.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Address.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Title.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Position.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contact).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contact).EndInit();
		this.tab_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Address).EndInit();
		this.contextMenuStrip_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Address).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address_AdresType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_States).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Address_Country).EndInit();
		this.tab_PaymentTerms.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_DflSwift.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankAccount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankAccountName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPBankCountry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BPBankName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankMandateID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankControlKey.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankIBAN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankDateOfSignature.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPBankDateOfSignature.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_EffectiveDiscount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentDefaultIBAN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTermPriority.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CreditCardType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePaymentExpirationDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePaymentExpirationDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentAvarageDelay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentIDNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCreditCardNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AcceptsEndorsedChecks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EndorsableChecksFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DoNotApplyDiscountGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPartialDlvrPerRow.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPartialDlvrOfSO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentPriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTerm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentTotalDiscount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentInterestonArrears.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCreditLimit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaymentCommitmentLimit.Properties).EndInit();
		this.tab_PaymentRun.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_BCACode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AutoCalBCG.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CollecAuth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RefDetails.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_HousCtlKey.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HsBnkSwift.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HousBnkBrn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HsBnkIBAN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HousBnkAct.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HouseBank.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HousBnkCry.Properties).EndInit();
		this.tab_Accounting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Accounting_general.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_BlockDunn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateX_DunnDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateX_DunnDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DunnLevel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ConnBP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseShpdGd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Affiliate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PlngGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DpmIntAct.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DpmClear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DebPayAcct.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_FatherType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FatherCard.Properties).EndInit();
		this.tab_Accounting_tax.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_DeferrTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ECVatGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ECVatGroupName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_VatStatus.Properties).EndInit();
		this.tab_Properties.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PropertiesCheck).EndInit();
		this.tab_Remark.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.picRemarks.Properties).EndInit();
		this.tab_AttachmentFiles.ResumeLayout(false);
		this.tab_UserDefineF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency.Properties).EndInit();
		this.contextMenuStrip_BP.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cb_CurrencyType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Series.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CardType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CardGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_OwnerCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
