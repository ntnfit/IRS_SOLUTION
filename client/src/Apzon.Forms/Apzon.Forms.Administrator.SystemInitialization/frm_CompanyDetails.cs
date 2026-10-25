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
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SystemInitialization;

public class frm_CompanyDetails : XtraFormEx
{
	private const string FormController = "CompanyDetails";

	private string _privilege = "";

	private string _CodeCountry = "";

	private List<Tuple<string, string>> _lstTaxRateDetermination = new List<Tuple<string, string>>();

	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tp_Generel;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx tp_LocalLang;

	private XtraTabPageEx tp_FrgnLang;

	private XtraTabPageEx tp_AccountingData;

	private XtraTabPageEx tp_BasicInit;

	private LabelLine labelLine5;

	private TextEditEx txt_County;

	private LabelLine labelLine6;

	private TextEditEx txt_ZipCode;

	private LabelLine labelLine7;

	private TextEditEx txt_City;

	private LabelLine labelLine8;

	private TextEditEx txt_BuildingFloorRoom;

	private LabelLine labelLine9;

	private TextEditEx txt_Block;

	private LabelLine labelLine4;

	private TextEditEx txt_StreetNo;

	private LabelLine labelLine3;

	private TextEditEx txt_StreetPObox;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private TextEditEx txt_CompanyName;

	private LabelLine lb_Name;

	private TextEditEx txt_GLN;

	private LabelLine labelLine10;

	private TextEditEx txt_Email;

	private LabelLine labelLine11;

	private TextEditEx txt_Fax;

	private LabelLine labelLine12;

	private TextEditEx txt_Telephone2;

	private LabelLine labelLine13;

	private TextEditEx txt_Telephone1;

	private LabelLine labelLine14;

	private TextEditEx txt_AliasName;

	private LabelLine labelLine15;

	private TextEditEx txt_ActiveManager;

	private LabelLine labelLine16;

	private TextEditEx txt_PrintingHeader;

	private LabelLine labelLine17;

	private TextEditEx txt_InternetAddress;

	private LabelLine labelLine18;

	private LabelLine labelLine19;

	private MemoEditEx memoEdit_Address;

	private MemoEditEx memoEdit_Address2;

	private TextEditEx txt_GLN2;

	private LabelLine labelLine20;

	private TextEditEx txt_Email2;

	private LabelLine labelLine21;

	private TextEditEx txt_Fax2;

	private LabelLine labelLine22;

	private TextEditEx txt_Telephone22;

	private LabelLine labelLine23;

	private TextEditEx txt_Telephone12;

	private LabelLine labelLine24;

	private TextEditEx txt_AliasName2;

	private LabelLine labelLine25;

	private TextEditEx txt_ActiveManager2;

	private LabelLine labelLine26;

	private TextEditEx txt_PrintingHeader2;

	private LabelLine labelLine27;

	private TextEditEx txt_InternetAddress2;

	private LabelLine labelLine28;

	private LabelLine labelLine29;

	private LabelLine labelLine30;

	private TextEditEx txt_County2;

	private LabelLine labelLine31;

	private TextEditEx txt_ZipCode2;

	private LabelLine labelLine32;

	private TextEditEx txt_City2;

	private LabelLine labelLine33;

	private TextEditEx txt_BuildingFloorRoom2;

	private LabelLine labelLine34;

	private TextEditEx txt_Block2;

	private LabelLine labelLine35;

	private TextEditEx txt_StreetNo2;

	private LabelLine labelLine36;

	private TextEditEx txt_StreetPOBox2;

	private LabelLine labelLine37;

	private LabelLine labelLine38;

	private TextEditEx txt_CompanyName2;

	private LabelLine labelLine39;

	private LabelLine labelLine41;

	private LabelLine labelLine42;

	private TextEditEx txt_TaxOfficial2;

	private LabelLine labelLine43;

	private TextEditEx txt_TaxDeductionNumber;

	private LabelLine labelLine44;

	private TextEditEx txt_ExemptionNumber;

	private LabelLine labelLine45;

	private LabelLine labelLine46;

	private TextEditEx txt_FreeZoneNo;

	private LabelLine labelLine47;

	private TextEditEx txt_FederalTaxID3;

	private LabelLine labelLine48;

	private TextEditEx txt_FederalTaxID2;

	private LabelLine labelLine49;

	private TextEditEx txt_FederalTaxID1;

	private LabelLine labelLine50;

	private TextEditEx txt_TaxOffice;

	private LabelLine labelLine51;

	private CheckEditEx chk_ApplyExchangeRateOnDeferredTax;

	private CheckEditEx chk_UseDeferredTax;

	private CheckEditEx chk_ExtendedTaxReporting;

	private LabelLine lb_DefaultAccountCurrency;

	private LabelLine lb_SystemCurrency;

	private LabelLine lb_LocalCurrency;

	private TextEditEx txt_ChartOfAccountsTemplate;

	private LabelLine lb_ChartOfAccountsTemplate;

	private LabelLine labelLine55;

	private CheckEditEx chk_DispCreditBalance;

	private GroupBox groupBox1;

	private TextEditEx txt_DefaultBankBranch;

	private LabelLine labelLine58;

	private LabelLine labelLine59;

	private LabelLine labelLine60;

	private LabelLine labelLine61;

	private TextEditEx txt_OrderingParty;

	private LabelLine labelLine57;

	private LookUpEdit lookUpEdit_Holiday;

	private CheckEditEx chk_MultiLangauge;

	private CheckEditEx chk_PermitMore;

	private CheckEditEx chk_AllowNegativeAmounts;

	private CheckEditEx chk_UseSegmentationAccounts;

	private CheckEditEx chk_UsePerpetalInventory;

	private Label label1;

	private CheckEditEx chk_UsePurchaseAccountPostingSystem;

	private CheckEditEx chk_AllowStockRelease;

	private CheckEditEx chk_ManageItemCost;

	private LookUpEdit lookUE_DefaulAccountNo;

	private LookUpEdit lookUE_DefaultBank;

	private LookUpEdit lookUE_DefaultBankCountry;

	private LabelLine labelLine40;

	private CheckEditEx chk_InstallBankStatementPr;

	private CheckEditEx chk_EnableFixedAssets;

	private LookUpEdit lookUE_Country;

	private LookUpEdit lookUE_State;

	private LookUpEdit lookUE_Country2;

	private LookUpEdit lookUE_State2;

	private CalcEditEx txt_CompanyTaxRate;

	private CheckEditEx chk_AllowSelection;

	private CheckEditEx chk_MaskCreditCardNumber;

	private CheckEditEx chk_EnableAdvanced;

	private CheckEditEx chk_EnableMultiBranch;

	private LookUpEdit luk_CalcDepreciationBy;

	private LookUpEdit luk_ItmGrpValuationMthd;

	private LookUpEdit luk_TaxRDetermination;

	private RadioGroupEx rdG_MngSerialNBatchCostBy;

	private LookUpEdit luk_DefaultAccountCurrency;

	private LookUpEdit look_SystemCurrency;

	private LookUpEdit look_LocalCurrency;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CompanyDetails()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CompanyDetails"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CompanyDetails" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_CompanyDetails_Load(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		btn_Cancel.Text = Resources.ButtonCancel;
		await ReLoadData();
		if (!GlobalData.LoginSap)
		{
			lb_DefaultAccountCurrency.Visible = false;
			luk_DefaultAccountCurrency.Visible = false;
			chk_DispCreditBalance.Visible = false;
			chk_UseSegmentationAccounts.Visible = false;
			chk_AllowNegativeAmounts.Visible = false;
			chk_UsePurchaseAccountPostingSystem.Visible = false;
			label1.Visible = false;
			rdG_MngSerialNBatchCostBy.Visible = false;
			chk_EnableFixedAssets.Visible = false;
			labelLine40.Visible = false;
			luk_CalcDepreciationBy.Visible = false;
			chk_EnableAdvanced.Visible = false;
			chk_MaskCreditCardNumber.Visible = false;
			chk_AllowSelection.Visible = false;
		}
		else
		{
			lb_DefaultAccountCurrency.Visible = true;
			luk_DefaultAccountCurrency.Visible = true;
			chk_DispCreditBalance.Visible = true;
			chk_UseSegmentationAccounts.Visible = true;
			chk_AllowNegativeAmounts.Visible = true;
			chk_UsePurchaseAccountPostingSystem.Visible = true;
			label1.Visible = true;
			rdG_MngSerialNBatchCostBy.Visible = true;
			chk_EnableFixedAssets.Visible = true;
			labelLine40.Visible = true;
			luk_CalcDepreciationBy.Visible = true;
			chk_EnableAdvanced.Visible = true;
			chk_MaskCreditCardNumber.Visible = true;
			chk_AllowSelection.Visible = true;
		}
	}

	private void lookUpEdit_Holiday_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_Holiday frm_Holiday2 = new frm_Holiday();
				frm_Holiday2.MdiParent = MdiParent;
				frm_Holiday2.Show();
				frm_Holiday2.FormClosing += async (object obj, FormClosingEventArgs e2) =>
				{
					await GetHoliDay();
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		btn_OK.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				DataSet ds = new DataSet();
				DataTable dtDoc = await GetTableColumns("OADM");
				DataTable dataTable = await GetTableColumns("ADM1");
				if (dtDoc == null || dtDoc.Columns.Count < 10 || dataTable == null || dataTable.Columns.Count < 10 || dtDoc.Rows.Count == 0 || dataTable.Rows.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
					btn_OK.Enabled = true;
					return;
				}
				dtDoc.Rows[0]["CompnyName"] = txt_CompanyName.Text;
				dtDoc.Rows[0]["CompnyAddr"] = memoEdit_Address.Text;
				dtDoc.Rows[0]["State"] = Function.ToString(lookUE_State.EditValue);
				dtDoc.Rows[0]["Country"] = Function.ToString(lookUE_Country.EditValue);
				dtDoc.Rows[0]["PrintHeadr"] = txt_PrintingHeader.Text;
				dtDoc.Rows[0]["Manager"] = txt_ActiveManager.Text;
				dtDoc.Rows[0]["AliasName"] = txt_AliasName.Text;
				dtDoc.Rows[0]["Phone1"] = txt_Telephone1.Text;
				dtDoc.Rows[0]["Phone2"] = txt_Telephone12.Text;
				dtDoc.Rows[0]["Fax"] = txt_Fax.Text;
				dtDoc.Rows[0]["E_Mail"] = txt_Email.Text;
				dtDoc.Rows[0]["CmpnyAddrF"] = memoEdit_Address2.Text;
				dtDoc.Rows[0]["PrintHdrF"] = txt_PrintingHeader2.Text;
				dtDoc.Rows[0]["ManagerF"] = txt_ActiveManager2.Text;
				dtDoc.Rows[0]["Phone1F"] = txt_Telephone2.Text;
				dtDoc.Rows[0]["Phone2F"] = txt_Telephone22.Text;
				dtDoc.Rows[0]["FaxF"] = txt_Fax2.Text;
				dtDoc.Rows[0]["RevOffice"] = txt_TaxOffice.Text;
				dtDoc.Rows[0]["TaxIdNum"] = txt_FederalTaxID1.Text;
				dtDoc.Rows[0]["TaxIdNum2"] = txt_FederalTaxID2.Text;
				dtDoc.Rows[0]["TaxIdNum3"] = txt_FederalTaxID3.Text;
				dtDoc.Rows[0]["FreeZoneNo"] = txt_FreeZoneNo.Text;
				dtDoc.Rows[0]["IncomeTax"] = Function.ParseDouble(txt_CompanyTaxRate.Text);
				dtDoc.Rows[0]["DdctFileNo"] = txt_ExemptionNumber.Text;
				dtDoc.Rows[0]["IRSFileNo"] = txt_TaxDeductionNumber.Text;
				dtDoc.Rows[0]["DdctOffice"] = txt_TaxOfficial2.Text;
				dtDoc.Rows[0]["DeferrTax"] = (chk_UseDeferredTax.Checked ? "Y" : "N");
				dtDoc.Rows[0]["ExRtDefTax"] = Function.ToString(chk_ApplyExchangeRateOnDeferredTax.EditValue);
				dtDoc.Rows[0]["TaxRateDet"] = Function.ToString(luk_TaxRDetermination.EditValue);
				dtDoc.Rows[0]["HldCode"] = Function.ToString(lookUpEdit_Holiday.EditValue);
				dtDoc.Rows[0]["UseExtRpt"] = (chk_ExtendedTaxReporting.Checked ? "Y" : "N");
				dtDoc.Rows[0]["CompType"] = txt_ChartOfAccountsTemplate.Text;
				dtDoc.Rows[0]["MainCurncy"] = Function.ToString(look_LocalCurrency.EditValue);
				dtDoc.Rows[0]["SysCurrncy"] = Function.ToString(look_SystemCurrency.EditValue);
				dtDoc.Rows[0]["DfActCurr"] = Function.ToString(luk_DefaultAccountCurrency.EditValue);
				dtDoc.Rows[0]["DispPosDeb"] = (chk_DispCreditBalance.Checked ? "Y" : "N");
				dtDoc.Rows[0]["EnbSgmnAct"] = (chk_UseSegmentationAccounts.Checked ? "Y" : "N");
				dtDoc.Rows[0]["NegAmount"] = (chk_AllowNegativeAmounts.Checked ? "Y" : "N");
				dtDoc.Rows[0]["DocNmMtd"] = (chk_PermitMore.Checked ? "Y" : "N");
				dtDoc.Rows[0]["MultiLang"] = (chk_MultiLangauge.Checked ? "Y" : "N");
				dtDoc.Rows[0]["ContInvnt"] = (chk_UsePerpetalInventory.Checked ? "Y" : "N");
				dtDoc.Rows[0]["InvntSystm"] = Function.ToString(luk_ItmGrpValuationMthd.EditValue);
				dtDoc.Rows[0]["PriceSys"] = (chk_ManageItemCost.Checked ? "Y" : "N");
				dtDoc.Rows[0]["UsePaSys"] = (chk_UsePurchaseAccountPostingSystem.Checked ? "Y" : "N");
				dtDoc.Rows[0]["RelStkNoPr"] = (chk_AllowStockRelease.Checked ? "Y" : "N");
				dtDoc.Rows[0]["SnBDfltSB"] = ((rdG_MngSerialNBatchCostBy.SelectedIndex == 0) ? "Y" : "N");
				dtDoc.Rows[0]["OrderParty"] = txt_OrderingParty.Text;
				dtDoc.Rows[0]["BankCountr"] = Function.ToString(lookUE_DefaultBankCountry.EditValue);
				dtDoc.Rows[0]["DflBnkCode"] = Function.ToString(lookUE_DefaultBank.EditValue);
				dtDoc.Rows[0]["DflBnkAcct"] = Function.ToString(lookUE_DefaulAccountNo.EditValue);
				dtDoc.Rows[0]["DflBranch"] = txt_DefaultBankBranch.Text;
				dtDoc.Rows[0]["InstFixAst"] = (chk_InstallBankStatementPr.Checked ? "Y" : "N");
				dtDoc.Rows[0]["DeprecCalc"] = Function.ToString(luk_CalcDepreciationBy.EditValue);
				dtDoc.Rows[0]["MltpBrnchs"] = Function.ToString(chk_EnableMultiBranch.EditValue);
				dtDoc.Rows[0]["CCMask"] = (chk_MaskCreditCardNumber.Checked ? "Y" : "N");
				dataTable.Rows[0]["Street"] = txt_StreetPObox.Text;
				dataTable.Rows[0]["StreetNo"] = txt_StreetNo.Text;
				dataTable.Rows[0]["Block"] = txt_Block.Text;
				dataTable.Rows[0]["Building"] = txt_BuildingFloorRoom.Text;
				dataTable.Rows[0]["City"] = txt_City.Text;
				dataTable.Rows[0]["ZipCode"] = txt_ZipCode.Text;
				dataTable.Rows[0]["County"] = txt_County.Text;
				dataTable.Rows[0]["GlblLocNum"] = txt_InternetAddress.Text;
				dataTable.Rows[0]["IntrntAdrs"] = txt_GLN.Text;
				dataTable.Rows[0]["StreetF"] = txt_StreetPOBox2.Text;
				dataTable.Rows[0]["StreetNoF"] = txt_StreetNo2.Text;
				dataTable.Rows[0]["BlockF"] = txt_Block2.Text;
				dataTable.Rows[0]["BuildingF"] = txt_BuildingFloorRoom2.Text;
				dataTable.Rows[0]["CityF"] = txt_City2.Text;
				dataTable.Rows[0]["ZipCodeF"] = txt_ZipCode2.Text;
				dataTable.Rows[0]["CountyF"] = txt_County2.Text;
				ds.Tables.Clear();
				ds.Tables.Add(dtDoc);
				ds.Tables.Add(dataTable);
				string text = await SaveData(ds);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					GlobalData.GeneralSetting = _httpClient.GeneralSetting.GetAllGeneralSetting();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Resources.Msg_SaveError + " " + text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
	}

	private void luk_DefaultAccountCurrency_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_CompanyName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_CompanyName2.EditValue = txt_CompanyName.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lookUE_Country_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			string editValue = (_CodeCountry = Function.ToString(lookUE_Country.EditValue));
			lookUE_Country2.EditValue = editValue;
			await GetState(_CodeCountry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lookUE_Country2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			string editValue = (_CodeCountry = Function.ToString(lookUE_Country2.EditValue));
			lookUE_Country.EditValue = editValue;
			await GetState(_CodeCountry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void memoEdit_Address_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_StreetPObox_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_StreetNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Block_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BuildingFloorRoom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_City_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ZipCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_County_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lookUE_State_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				lookUE_State2.EditValue = lookUE_State.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_InternetAddress_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_InternetAddress2.Text = txt_InternetAddress.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_PrintingHeader_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ActiveManager_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_AliasName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_AliasName2.Text = txt_AliasName.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Telephone1_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Telephone2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Fax_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Email_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_Email2.Text = txt_Email.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_GLN_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_CompanyName2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_CompanyName.EditValue = txt_CompanyName2.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void memoEdit_Address2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_StreetPOBox2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_StreetNo2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Block2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BuildingFloorRoom2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_City2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ZipCode2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_County2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lookUE_State2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				lookUE_State.EditValue = lookUE_State2.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_InternetAddress2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_InternetAddress.Text = txt_InternetAddress2.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_PrintingHeader2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ActiveManager2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_AliasName2_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_AliasName.Text = txt_AliasName2.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Telephone12_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Telephone22_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Fax2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Email2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				txt_Email.Text = txt_Email2.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_GLN2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_TaxOffice_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_FederalTaxID1_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_FederalTaxID2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_FederalTaxID3_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_FreeZoneNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_CompanyTaxRate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ExemptionNumber_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_TaxDeductionNumber_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_TaxOfficial2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_UseDeferredTax_Properties_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_ApplyExchangeRateOnDeferredTax_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_ExtendedTaxReporting_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_DispCreditBalance_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_UseSegmentationAccounts_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_AllowNegativeAmounts_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_PermitMore_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_MultiLangauge_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_UsePerpetalInventory_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_ManageItemCost_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_UsePurchaseAccountPostingSystem_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_AllowStockRelease_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rdG_MngSerialNBatchCostBy_Properties_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_OrderingParty_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lookUE_DefaultBankCountry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lookUE_DefaultBank_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lookUE_DefaulAccountNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_DefaultBankBranch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_InstallBankStatementPr_Properties_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_EnableFixedAssets_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_EnableMultiBranch_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_EnableAdvanced_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_MaskCreditCardNumber_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_AllowSelection_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_CalcDepreciationBy_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ReLoadData()
	{
		_ = 3;
		try
		{
			_isLoading = true;
			await LoadCombo();
			await GetOADM();
			await GetADM1();
			DataTable dataTable = await _httpClient.Commons.ExistsTransaction();
			if (dataTable.IsNotNull())
			{
				bool flag = Function.ToString(dataTable.Rows[0][0]).ToUpper().Equals("Y");
				LookUpEdit lookUpEdit = look_LocalCurrency;
				bool enabled = (look_SystemCurrency.Enabled = !flag);
				lookUpEdit.Enabled = enabled;
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetOADM()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			DataTable dataTable = await hp.GetAsync("GetOADM");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txt_CompanyName.Text = Function.ToString(dataTable.Rows[0]["CompnyName"]);
				memoEdit_Address.Text = Function.ToString(dataTable.Rows[0]["CompnyAddr"]);
				lookUE_State.EditValue = Function.ToString(dataTable.Rows[0]["State"]);
				lookUE_Country.EditValue = Function.ToString(dataTable.Rows[0]["Country"]);
				txt_PrintingHeader.Text = Function.ToString(dataTable.Rows[0]["PrintHeadr"]);
				txt_ActiveManager.Text = Function.ToString(dataTable.Rows[0]["Manager"]);
				txt_AliasName.Text = Function.ToString(dataTable.Rows[0]["AliasName"]);
				txt_Telephone1.Text = Function.ToString(dataTable.Rows[0]["Phone1"]);
				txt_Telephone12.Text = Function.ToString(dataTable.Rows[0]["Phone2"]);
				txt_Fax.Text = Function.ToString(dataTable.Rows[0]["Fax"]);
				txt_Email.Text = Function.ToString(dataTable.Rows[0]["E_Mail"]);
				txt_CompanyName2.Text = Function.ToString(dataTable.Rows[0]["CompnyName"]);
				memoEdit_Address2.Text = Function.ToString(dataTable.Rows[0]["CmpnyAddrF"]);
				lookUE_State2.EditValue = Function.ToString(dataTable.Rows[0]["State"]);
				lookUE_Country2.EditValue = Function.ToString(dataTable.Rows[0]["Country"]);
				txt_PrintingHeader2.Text = Function.ToString(dataTable.Rows[0]["PrintHdrF"]);
				txt_ActiveManager2.Text = Function.ToString(dataTable.Rows[0]["ManagerF"]);
				txt_AliasName2.Text = Function.ToString(dataTable.Rows[0]["AliasName"]);
				txt_Telephone2.Text = Function.ToString(dataTable.Rows[0]["Phone1F"]);
				txt_Telephone22.Text = Function.ToString(dataTable.Rows[0]["Phone2F"]);
				txt_Fax2.Text = Function.ToString(dataTable.Rows[0]["FaxF"]);
				txt_Email2.Text = Function.ToString(dataTable.Rows[0]["E_Mail"]);
				txt_TaxOffice.Text = Function.ToString(dataTable.Rows[0]["RevOffice"]);
				txt_FederalTaxID1.Text = Function.ToString(dataTable.Rows[0]["TaxIdNum"]);
				txt_FederalTaxID2.Text = Function.ToString(dataTable.Rows[0]["TaxIdNum2"]);
				txt_FederalTaxID3.Text = Function.ToString(dataTable.Rows[0]["TaxIdNum3"]);
				txt_FreeZoneNo.Text = Function.ToString(dataTable.Rows[0]["FreeZoneNo"]);
				txt_CompanyTaxRate.Text = Function.ToString(dataTable.Rows[0]["IncomeTax"]);
				txt_ExemptionNumber.Text = Function.ToString(dataTable.Rows[0]["DdctFileNo"]);
				txt_TaxDeductionNumber.Text = Function.ToString(dataTable.Rows[0]["IRSFileNo"]);
				txt_TaxOfficial2.Text = Function.ToString(dataTable.Rows[0]["DdctOffice"]);
				chk_UseDeferredTax.Checked = Function.ToString(dataTable.Rows[0]["DeferrTax"]) == "Y";
				chk_ApplyExchangeRateOnDeferredTax.Checked = Function.ToString(dataTable.Rows[0]["ExRtDefTax"]) == "Y";
				luk_TaxRDetermination.EditValue = Function.ToString(dataTable.Rows[0]["TaxRateDet"]);
				lookUpEdit_Holiday.EditValue = Function.ToString(dataTable.Rows[0]["HldCode"]);
				chk_ExtendedTaxReporting.Checked = Function.ToString(dataTable.Rows[0]["UseExtRpt"]) == "Y";
				txt_ChartOfAccountsTemplate.Text = ((Function.ToString(dataTable.Rows[0]["CompType"]).ToUpper() == "U") ? "User Define" : Function.ToString(dataTable.Rows[0]["CompType"]));
				look_LocalCurrency.EditValue = Function.ToString(dataTable.Rows[0]["MainCurncy"]);
				look_SystemCurrency.EditValue = Function.ToString(dataTable.Rows[0]["SysCurrncy"]);
				luk_DefaultAccountCurrency.EditValue = Function.ToString(dataTable.Rows[0]["DfActCurr"]);
				chk_DispCreditBalance.Checked = Function.ParseBool(dataTable.Rows[0]["DispPosDeb"]);
				chk_UseSegmentationAccounts.Checked = Function.ToString(dataTable.Rows[0]["EnbSgmnAct"]) == "Y";
				chk_AllowNegativeAmounts.Checked = Function.ToString(dataTable.Rows[0]["NegAmount"]) == "Y";
				chk_PermitMore.Checked = Function.ToString(dataTable.Rows[0]["DocNmMtd"]) == "Y";
				chk_MultiLangauge.Checked = Function.ToString(dataTable.Rows[0]["MultiLang"]) == "Y";
				chk_UsePerpetalInventory.Checked = Function.ToString(dataTable.Rows[0]["ContInvnt"]) == "Y";
				luk_ItmGrpValuationMthd.EditValue = Function.ToString(dataTable.Rows[0]["InvntSystm"]);
				chk_ManageItemCost.Checked = Function.ToString(dataTable.Rows[0]["PriceSys"]) == "Y";
				chk_UsePurchaseAccountPostingSystem.Checked = Function.ToString(dataTable.Rows[0]["UsePaSys"]) == "Y";
				chk_AllowStockRelease.Checked = Function.ToString(dataTable.Rows[0]["RelStkNoPr"]) == "Y";
				rdG_MngSerialNBatchCostBy.SelectedIndex = Function.ParseInt(dataTable.Rows[0]["SnBDfltSB"]);
				txt_OrderingParty.Text = Function.ToString(dataTable.Rows[0]["OrderParty"]);
				lookUE_DefaultBankCountry.EditValue = Function.ToString(dataTable.Rows[0]["BankCountr"]);
				lookUE_DefaultBank.EditValue = Function.ToString(dataTable.Rows[0]["DflBnkCode"]);
				lookUE_DefaulAccountNo.EditValue = Function.ToString(dataTable.Rows[0]["DflBnkAcct"]);
				txt_DefaultBankBranch.Text = Function.ToString(dataTable.Rows[0]["DflBranch"]);
				chk_InstallBankStatementPr.Checked = Function.ToString(dataTable.Rows[0]["InstFixAst"]) == "Y";
				chk_EnableFixedAssets.Checked = false;
				luk_CalcDepreciationBy.EditValue = Function.ToString(dataTable.Rows[0]["DeprecCalc"]);
				chk_EnableMultiBranch.Checked = Function.ToString(dataTable.Rows[0]["MltpBrnchs"]) == "Y";
				chk_MaskCreditCardNumber.Checked = Function.ToString(dataTable.Rows[0]["CCMask"]) == "Y";
				chk_EnableAdvanced.Checked = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetADM1()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			DataTable dataTable = await hp.GetAsync("GetADM1");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txt_StreetPObox.Text = Function.ToString(dataTable.Rows[0]["Street"]);
				txt_StreetNo.Text = Function.ToString(dataTable.Rows[0]["StreetNo"]);
				txt_Block.Text = Function.ToString(dataTable.Rows[0]["Block"]);
				txt_BuildingFloorRoom.Text = Function.ToString(dataTable.Rows[0]["Building"]);
				txt_City.Text = Function.ToString(dataTable.Rows[0]["City"]);
				txt_ZipCode.Text = Function.ToString(dataTable.Rows[0]["ZipCode"]);
				txt_County.Text = Function.ToString(dataTable.Rows[0]["County"]);
				txt_InternetAddress.Text = Function.ToString(dataTable.Rows[0]["IntrntAdrs"]);
				txt_GLN.Text = Function.ToString(dataTable.Rows[0]["GlblLocNum"]);
				txt_StreetPOBox2.Text = Function.ToString(dataTable.Rows[0]["StreetF"]);
				txt_StreetNo2.Text = Function.ToString(dataTable.Rows[0]["StreetNoF"]);
				txt_Block2.Text = Function.ToString(dataTable.Rows[0]["BlockF"]);
				txt_BuildingFloorRoom2.Text = Function.ToString(dataTable.Rows[0]["BuildingF"]);
				txt_City2.Text = Function.ToString(dataTable.Rows[0]["CityF"]);
				txt_ZipCode2.Text = Function.ToString(dataTable.Rows[0]["ZipCodeF"]);
				txt_County2.Text = Function.ToString(dataTable.Rows[0]["CountyF"]);
				txt_InternetAddress2.Text = Function.ToString(dataTable.Rows[0]["IntrntAdrs"]);
				txt_GLN2.Text = Function.ToString(dataTable.Rows[0]["GlblLocNum"]);
				chk_AllowSelection.Checked = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCombo()
	{
		_ = 1;
		try
		{
			Dictionary<string, string> dic = GlobalData.CurrencyCodes.ToDictionary((CurrencyCodesImpl item) => item.CurrCode, (CurrencyCodesImpl item) => item.CurrName);
			look_LocalCurrency.SetDataSource(dic);
			look_SystemCurrency.SetDataSource(dic);
			await GetHoliDay();
			GetTaxRateDetermination();
			GetDefaultAccountCurrency();
			GetItemGroupsValuationMethod();
			GetCalculateDepreciationBy();
			GetDefaultAccountCurrency();
			await GetCountry();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData(DataSet ds)
	{
		string result = "";
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CompanyDetails");
			result = await hp.PostAsJsonAsync("Save", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private async Task<DataTable> GetTableColumns(string tableName)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			dt = await hp.GetAsync("GetTableColumns", new { tableName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private void GetCalculateDepreciationBy()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "D";
			dataRow["Name"] = "Day";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "M";
			dataRow2["Name"] = "Month";
			dataTable.Rows.Add(dataRow2);
			luk_CalcDepreciationBy.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetItemGroupsValuationMethod()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "A";
			dataRow["Name"] = "Moving Average";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "S";
			dataRow2["Name"] = "Standard";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = "F";
			dataRow3["Name"] = "FIFO";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			luk_ItmGrpValuationMthd.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDefaultAccountCurrency()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "Y";
			dataRow["Name"] = "All Currencies";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "N";
			dataRow2["Name"] = "Local Currency";
			dataTable.Rows.Add(dataRow2);
			luk_DefaultAccountCurrency.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetTaxRateDetermination()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "P";
			dataRow["Name"] = "Posting Date";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "D";
			dataRow2["Name"] = "Document Date";
			dataTable.Rows.Add(dataRow2);
			luk_TaxRDetermination.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetHoliDay()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			DataTable dataTable = await hp.GetAsync("GetHoliday");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				lookUpEdit_Holiday.Properties.DataSource = dataTable;
			}
			else
			{
				dataTable.Columns.Add("HldCode", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["HldCode"] = string.Empty;
				dataTable.Rows.Add(dataRow);
				lookUpEdit_Holiday.Properties.DataSource = dataTable;
			}
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["HldCode"] = string.Empty;
			dataTable.Rows.InsertAt(dataRow2, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetCountry()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			DataTable dt = await hp.GetAsync("GetCountry");
			if (dt != null && dt.Rows.Count > 0)
			{
				lookUE_Country.Properties.DataSource = dt;
				lookUE_Country2.Properties.DataSource = dt;
				await GetState(_CodeCountry);
			}
			else
			{
				dt.Columns.Add("Code", typeof(string));
				dt.Columns.Add("Name", typeof(string));
				DataRow dataRow = dt.NewRow();
				dataRow["Code"] = string.Empty;
				dataRow["Name"] = string.Empty;
				dt.Rows.Add(dataRow);
				lookUE_Country.Properties.DataSource = dt;
				lookUE_Country2.Properties.DataSource = dt;
			}
			DataRow dataRow2 = dt.NewRow();
			dataRow2["Code"] = string.Empty;
			dataRow2["Name"] = string.Empty;
			dt.Rows.InsertAt(dataRow2, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetState(string _CodeCountry)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			DataTable dataTable = await hp.GetAsync("GetState", new
			{
				codeCountry = _CodeCountry
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				lookUE_State.Properties.DataSource = dataTable;
				lookUE_State2.Properties.DataSource = dataTable;
			}
			else
			{
				dataTable.Columns.Add("Code", typeof(string));
				dataTable.Columns.Add("Name", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = string.Empty;
				dataRow["Name"] = string.Empty;
				dataTable.Rows.Add(dataRow);
				lookUE_State.Properties.DataSource = dataTable;
				lookUE_State2.Properties.DataSource = dataTable;
			}
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = string.Empty;
			dataRow2["Name"] = string.Empty;
			dataTable.Rows.InsertAt(dataRow2, 0);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SystemInitialization.frm_CompanyDetails));
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tp_Generel = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tp_LocalLang = new Apzon.Controls.Controls.XtraTabPageEx();
		this.lookUE_Country = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUE_State = new DevExpress.XtraEditors.LookUpEdit();
		this.memoEdit_Address = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_GLN = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Email = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Telephone2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Telephone1 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.txt_AliasName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ActiveManager = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.txt_PrintingHeader = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.txt_InternetAddress = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_County = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ZipCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.txt_City = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BuildingFloorRoom = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Block = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_StreetNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_StreetPObox = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CompanyName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.tp_FrgnLang = new Apzon.Controls.Controls.XtraTabPageEx();
		this.lookUE_Country2 = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUE_State2 = new DevExpress.XtraEditors.LookUpEdit();
		this.memoEdit_Address2 = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_GLN2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Email2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Fax2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Telephone22 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Telephone12 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_AliasName2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ActiveManager2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.txt_PrintingHeader2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine27 = new Apzon.Controls.Controls.LabelLine();
		this.txt_InternetAddress2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine28 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine29 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine30 = new Apzon.Controls.Controls.LabelLine();
		this.txt_County2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine31 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ZipCode2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine32 = new Apzon.Controls.Controls.LabelLine();
		this.txt_City2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine33 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BuildingFloorRoom2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine34 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Block2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine35 = new Apzon.Controls.Controls.LabelLine();
		this.txt_StreetNo2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine36 = new Apzon.Controls.Controls.LabelLine();
		this.txt_StreetPOBox2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine37 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine38 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CompanyName2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine39 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine22 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine23 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.tp_AccountingData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_CompanyTaxRate = new Apzon.Controls.Editor.CalcEditEx();
		this.luk_TaxRDetermination = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUpEdit_Holiday = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_ExtendedTaxReporting = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ApplyExchangeRateOnDeferredTax = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UseDeferredTax = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine41 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine42 = new Apzon.Controls.Controls.LabelLine();
		this.txt_TaxOfficial2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine43 = new Apzon.Controls.Controls.LabelLine();
		this.txt_TaxDeductionNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine44 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ExemptionNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine45 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine46 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FreeZoneNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine47 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FederalTaxID3 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine48 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FederalTaxID2 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine49 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FederalTaxID1 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine50 = new Apzon.Controls.Controls.LabelLine();
		this.txt_TaxOffice = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine51 = new Apzon.Controls.Controls.LabelLine();
		this.tp_BasicInit = new Apzon.Controls.Controls.XtraTabPageEx();
		this.look_SystemCurrency = new DevExpress.XtraEditors.LookUpEdit();
		this.look_LocalCurrency = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_DefaultAccountCurrency = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_ItmGrpValuationMthd = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_CalcDepreciationBy = new DevExpress.XtraEditors.LookUpEdit();
		this.label1 = new System.Windows.Forms.Label();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.lookUE_DefaulAccountNo = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUE_DefaultBank = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUE_DefaultBankCountry = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_DefaultBankBranch = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine58 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine59 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine60 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine61 = new Apzon.Controls.Controls.LabelLine();
		this.txt_OrderingParty = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine40 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine57 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine55 = new Apzon.Controls.Controls.LabelLine();
		this.chk_InstallBankStatementPr = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UsePurchaseAccountPostingSystem = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowSelection = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_MaskCreditCardNumber = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_EnableAdvanced = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_EnableMultiBranch = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_EnableFixedAssets = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowStockRelease = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ManageItemCost = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UsePerpetalInventory = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_MultiLangauge = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PermitMore = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowNegativeAmounts = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UseSegmentationAccounts = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_DispCreditBalance = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_DefaultAccountCurrency = new Apzon.Controls.Controls.LabelLine();
		this.lb_SystemCurrency = new Apzon.Controls.Controls.LabelLine();
		this.lb_LocalCurrency = new Apzon.Controls.Controls.LabelLine();
		this.txt_ChartOfAccountsTemplate = new Apzon.Controls.Editor.TextEditEx();
		this.lb_ChartOfAccountsTemplate = new Apzon.Controls.Controls.LabelLine();
		this.rdG_MngSerialNBatchCostBy = new Apzon.Controls.Editor.RadioGroupEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tp_Generel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.tp_LocalLang.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUE_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_State.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AliasName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveManager.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InternetAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_County.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ZipCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_City.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BuildingFloorRoom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Block.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetPObox.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyName.Properties).BeginInit();
		this.tp_FrgnLang.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUE_Country2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_State2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone22.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone12.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AliasName2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveManager2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InternetAddress2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_County2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ZipCode2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_City2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BuildingFloorRoom2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Block2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetNo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetPOBox2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyName2.Properties).BeginInit();
		this.tp_AccountingData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyTaxRate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TaxRDetermination.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Holiday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExtendedTaxReporting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ApplyExchangeRateOnDeferredTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseDeferredTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxOfficial2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxDeductionNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExemptionNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FreeZoneNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxOffice.Properties).BeginInit();
		this.tp_BasicInit.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_SystemCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_LocalCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DefaultAccountCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmGrpValuationMthd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CalcDepreciationBy.Properties).BeginInit();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaulAccountNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaultBank.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaultBankCountry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DefaultBankBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OrderingParty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InstallBankStatementPr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UsePurchaseAccountPostingSystem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowSelection.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MaskCreditCardNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableAdvanced.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableMultiBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableFixedAssets.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowStockRelease.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ManageItemCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UsePerpetalInventory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MultiLangauge.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PermitMore.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowNegativeAmounts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseSegmentationAccounts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DispCreditBalance.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChartOfAccountsTemplate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdG_MngSerialNBatchCostBy.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 543);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 0;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 543);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 1;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(5, 3);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.tp_Generel;
		this.xtraTabControl1.Size = new System.Drawing.Size(537, 534);
		this.xtraTabControl1.TabIndex = 2;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tp_Generel, this.tp_AccountingData, this.tp_BasicInit });
		this.tp_Generel.Controls.Add(this.xtraTabControl2);
		this.tp_Generel.FieldName = "";
		this.tp_Generel.IsSearchControl = false;
		this.tp_Generel.Name = "tp_Generel";
		this.tp_Generel.Size = new System.Drawing.Size(533, 510);
		this.tp_Generel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tp_Generel.Text = "General";
		this.xtraTabControl2.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xtraTabControl2.Appearance.Options.UseBackColor = true;
		this.xtraTabControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(2, 2);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.PaintStyleName = "Standard";
		this.xtraTabControl2.SelectedTabPage = this.tp_LocalLang;
		this.xtraTabControl2.Size = new System.Drawing.Size(530, 507);
		this.xtraTabControl2.TabIndex = 0;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tp_LocalLang, this.tp_FrgnLang });
		this.tp_LocalLang.Controls.Add(this.lookUE_Country);
		this.tp_LocalLang.Controls.Add(this.lookUE_State);
		this.tp_LocalLang.Controls.Add(this.memoEdit_Address);
		this.tp_LocalLang.Controls.Add(this.txt_GLN);
		this.tp_LocalLang.Controls.Add(this.labelLine10);
		this.tp_LocalLang.Controls.Add(this.txt_Email);
		this.tp_LocalLang.Controls.Add(this.labelLine11);
		this.tp_LocalLang.Controls.Add(this.txt_Fax);
		this.tp_LocalLang.Controls.Add(this.labelLine12);
		this.tp_LocalLang.Controls.Add(this.txt_Telephone2);
		this.tp_LocalLang.Controls.Add(this.labelLine13);
		this.tp_LocalLang.Controls.Add(this.txt_Telephone1);
		this.tp_LocalLang.Controls.Add(this.labelLine14);
		this.tp_LocalLang.Controls.Add(this.txt_AliasName);
		this.tp_LocalLang.Controls.Add(this.labelLine15);
		this.tp_LocalLang.Controls.Add(this.txt_ActiveManager);
		this.tp_LocalLang.Controls.Add(this.labelLine16);
		this.tp_LocalLang.Controls.Add(this.txt_PrintingHeader);
		this.tp_LocalLang.Controls.Add(this.labelLine17);
		this.tp_LocalLang.Controls.Add(this.txt_InternetAddress);
		this.tp_LocalLang.Controls.Add(this.labelLine18);
		this.tp_LocalLang.Controls.Add(this.labelLine19);
		this.tp_LocalLang.Controls.Add(this.labelLine5);
		this.tp_LocalLang.Controls.Add(this.txt_County);
		this.tp_LocalLang.Controls.Add(this.labelLine6);
		this.tp_LocalLang.Controls.Add(this.txt_ZipCode);
		this.tp_LocalLang.Controls.Add(this.labelLine7);
		this.tp_LocalLang.Controls.Add(this.txt_City);
		this.tp_LocalLang.Controls.Add(this.labelLine8);
		this.tp_LocalLang.Controls.Add(this.txt_BuildingFloorRoom);
		this.tp_LocalLang.Controls.Add(this.labelLine9);
		this.tp_LocalLang.Controls.Add(this.txt_Block);
		this.tp_LocalLang.Controls.Add(this.labelLine4);
		this.tp_LocalLang.Controls.Add(this.txt_StreetNo);
		this.tp_LocalLang.Controls.Add(this.labelLine3);
		this.tp_LocalLang.Controls.Add(this.txt_StreetPObox);
		this.tp_LocalLang.Controls.Add(this.labelLine2);
		this.tp_LocalLang.Controls.Add(this.labelLine1);
		this.tp_LocalLang.Controls.Add(this.txt_CompanyName);
		this.tp_LocalLang.Controls.Add(this.lb_Name);
		this.tp_LocalLang.FieldName = "";
		this.tp_LocalLang.IsSearchControl = false;
		this.tp_LocalLang.Name = "tp_LocalLang";
		this.tp_LocalLang.Size = new System.Drawing.Size(526, 483);
		this.tp_LocalLang.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tp_LocalLang.Text = "Local Language";
		this.lookUE_Country.Location = new System.Drawing.Point(157, 251);
		this.lookUE_Country.Name = "lookUE_Country";
		this.lookUE_Country.Properties.AutoHeight = false;
		this.lookUE_Country.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_Country.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUE_Country.Properties.DisplayMember = "Name";
		this.lookUE_Country.Properties.NullText = "";
		this.lookUE_Country.Properties.ValueMember = "Code";
		this.lookUE_Country.Size = new System.Drawing.Size(345, 18);
		this.lookUE_Country.TabIndex = 45;
		this.lookUE_Country.EditValueChanged += new System.EventHandler(this.lookUE_Country_EditValueChanged);
		this.lookUE_State.Location = new System.Drawing.Point(157, 232);
		this.lookUE_State.Name = "lookUE_State";
		this.lookUE_State.Properties.AutoHeight = false;
		this.lookUE_State.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_State.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUE_State.Properties.DisplayMember = "Name";
		this.lookUE_State.Properties.NullText = "";
		this.lookUE_State.Properties.ValueMember = "Code";
		this.lookUE_State.Size = new System.Drawing.Size(345, 18);
		this.lookUE_State.TabIndex = 44;
		this.lookUE_State.EditValueChanged += new System.EventHandler(this.lookUE_State_EditValueChanged);
		this.memoEdit_Address.FieldName = null;
		this.memoEdit_Address.IsSearchControl = false;
		this.memoEdit_Address.Location = new System.Drawing.Point(157, 26);
		this.memoEdit_Address.Name = "memoEdit_Address";
		this.memoEdit_Address.Properties.ReadOnly = true;
		this.memoEdit_Address.Size = new System.Drawing.Size(345, 56);
		this.memoEdit_Address.TabIndex = 43;
		this.memoEdit_Address.EditValueChanged += new System.EventHandler(this.memoEdit_Address_EditValueChanged);
		this.txt_GLN.FieldName = null;
		this.txt_GLN.IsSearchControl = false;
		this.txt_GLN.Location = new System.Drawing.Point(157, 440);
		this.txt_GLN.Name = "txt_GLN";
		this.txt_GLN.Properties.AutoHeight = false;
		this.txt_GLN.Size = new System.Drawing.Size(345, 18);
		this.txt_GLN.TabIndex = 41;
		this.txt_GLN.EditValueChanged += new System.EventHandler(this.txt_GLN_EditValueChanged);
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(16, 441);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(143, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 42;
		this.labelLine10.Utext = "GLN";
		this.txt_Email.FieldName = null;
		this.txt_Email.IsSearchControl = false;
		this.txt_Email.Location = new System.Drawing.Point(157, 421);
		this.txt_Email.Name = "txt_Email";
		this.txt_Email.Properties.AutoHeight = false;
		this.txt_Email.Size = new System.Drawing.Size(345, 18);
		this.txt_Email.TabIndex = 39;
		this.txt_Email.EditValueChanged += new System.EventHandler(this.txt_Email_EditValueChanged);
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(16, 422);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(143, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 40;
		this.labelLine11.Utext = "E-Mail";
		this.txt_Fax.FieldName = null;
		this.txt_Fax.IsSearchControl = false;
		this.txt_Fax.Location = new System.Drawing.Point(157, 402);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.Properties.AutoHeight = false;
		this.txt_Fax.Size = new System.Drawing.Size(345, 18);
		this.txt_Fax.TabIndex = 37;
		this.txt_Fax.EditValueChanged += new System.EventHandler(this.txt_Fax_EditValueChanged);
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(16, 403);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(143, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 38;
		this.labelLine12.Utext = "Fax";
		this.txt_Telephone2.FieldName = null;
		this.txt_Telephone2.IsSearchControl = false;
		this.txt_Telephone2.Location = new System.Drawing.Point(157, 383);
		this.txt_Telephone2.Name = "txt_Telephone2";
		this.txt_Telephone2.Properties.AutoHeight = false;
		this.txt_Telephone2.Size = new System.Drawing.Size(345, 18);
		this.txt_Telephone2.TabIndex = 35;
		this.txt_Telephone2.EditValueChanged += new System.EventHandler(this.txt_Telephone2_EditValueChanged);
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(16, 384);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(143, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 36;
		this.labelLine13.Utext = "Telephone 2";
		this.txt_Telephone1.FieldName = null;
		this.txt_Telephone1.IsSearchControl = false;
		this.txt_Telephone1.Location = new System.Drawing.Point(157, 364);
		this.txt_Telephone1.Name = "txt_Telephone1";
		this.txt_Telephone1.Properties.AutoHeight = false;
		this.txt_Telephone1.Size = new System.Drawing.Size(345, 18);
		this.txt_Telephone1.TabIndex = 33;
		this.txt_Telephone1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Telephone1_EditValueChanging);
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(16, 365);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(143, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 34;
		this.labelLine14.Utext = "Telephone 1";
		this.txt_AliasName.FieldName = null;
		this.txt_AliasName.IsSearchControl = false;
		this.txt_AliasName.Location = new System.Drawing.Point(157, 345);
		this.txt_AliasName.Name = "txt_AliasName";
		this.txt_AliasName.Properties.AutoHeight = false;
		this.txt_AliasName.Size = new System.Drawing.Size(345, 18);
		this.txt_AliasName.TabIndex = 31;
		this.txt_AliasName.EditValueChanged += new System.EventHandler(this.txt_AliasName_EditValueChanged);
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(16, 346);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(143, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 32;
		this.labelLine15.Utext = "Alias Name";
		this.txt_ActiveManager.FieldName = null;
		this.txt_ActiveManager.IsSearchControl = false;
		this.txt_ActiveManager.Location = new System.Drawing.Point(157, 308);
		this.txt_ActiveManager.Name = "txt_ActiveManager";
		this.txt_ActiveManager.Properties.AutoHeight = false;
		this.txt_ActiveManager.Size = new System.Drawing.Size(345, 18);
		this.txt_ActiveManager.TabIndex = 29;
		this.txt_ActiveManager.EditValueChanged += new System.EventHandler(this.txt_ActiveManager_EditValueChanged);
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(16, 309);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(143, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 30;
		this.labelLine16.Utext = "Active Manager";
		this.txt_PrintingHeader.FieldName = null;
		this.txt_PrintingHeader.IsSearchControl = false;
		this.txt_PrintingHeader.Location = new System.Drawing.Point(157, 289);
		this.txt_PrintingHeader.Name = "txt_PrintingHeader";
		this.txt_PrintingHeader.Properties.AutoHeight = false;
		this.txt_PrintingHeader.Size = new System.Drawing.Size(345, 18);
		this.txt_PrintingHeader.TabIndex = 27;
		this.txt_PrintingHeader.EditValueChanged += new System.EventHandler(this.txt_PrintingHeader_EditValueChanged);
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(16, 290);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(143, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 28;
		this.labelLine17.Utext = "Printing Header";
		this.txt_InternetAddress.FieldName = null;
		this.txt_InternetAddress.IsSearchControl = false;
		this.txt_InternetAddress.Location = new System.Drawing.Point(157, 270);
		this.txt_InternetAddress.Name = "txt_InternetAddress";
		this.txt_InternetAddress.Properties.AutoHeight = false;
		this.txt_InternetAddress.Size = new System.Drawing.Size(345, 18);
		this.txt_InternetAddress.TabIndex = 25;
		this.txt_InternetAddress.EditValueChanged += new System.EventHandler(this.txt_InternetAddress_EditValueChanged);
		this.labelLine18.FieldName = null;
		this.labelLine18.HideWhenEmpty = false;
		this.labelLine18.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine18.Location = new System.Drawing.Point(16, 271);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.ShowUnderLine = true;
		this.labelLine18.Size = new System.Drawing.Size(143, 20);
		this.labelLine18.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine18.TabIndex = 26;
		this.labelLine18.Utext = "Internet Address";
		this.labelLine19.FieldName = null;
		this.labelLine19.HideWhenEmpty = false;
		this.labelLine19.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine19.Location = new System.Drawing.Point(16, 253);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.ShowUnderLine = true;
		this.labelLine19.Size = new System.Drawing.Size(143, 20);
		this.labelLine19.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine19.TabIndex = 24;
		this.labelLine19.Utext = "Country";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(16, 233);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(143, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 22;
		this.labelLine5.Utext = "State";
		this.txt_County.FieldName = null;
		this.txt_County.IsSearchControl = false;
		this.txt_County.Location = new System.Drawing.Point(157, 213);
		this.txt_County.Name = "txt_County";
		this.txt_County.Properties.AutoHeight = false;
		this.txt_County.Size = new System.Drawing.Size(345, 18);
		this.txt_County.TabIndex = 19;
		this.txt_County.EditValueChanged += new System.EventHandler(this.txt_County_EditValueChanged);
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(16, 214);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(143, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 20;
		this.labelLine6.Utext = "County";
		this.txt_ZipCode.FieldName = null;
		this.txt_ZipCode.IsSearchControl = false;
		this.txt_ZipCode.Location = new System.Drawing.Point(157, 194);
		this.txt_ZipCode.Name = "txt_ZipCode";
		this.txt_ZipCode.Properties.AutoHeight = false;
		this.txt_ZipCode.Size = new System.Drawing.Size(345, 18);
		this.txt_ZipCode.TabIndex = 17;
		this.txt_ZipCode.EditValueChanged += new System.EventHandler(this.txt_ZipCode_EditValueChanged);
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(16, 195);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(143, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 18;
		this.labelLine7.Utext = "Zip Code";
		this.txt_City.FieldName = null;
		this.txt_City.IsSearchControl = false;
		this.txt_City.Location = new System.Drawing.Point(157, 175);
		this.txt_City.Name = "txt_City";
		this.txt_City.Properties.AutoHeight = false;
		this.txt_City.Size = new System.Drawing.Size(345, 18);
		this.txt_City.TabIndex = 15;
		this.txt_City.EditValueChanged += new System.EventHandler(this.txt_City_EditValueChanged);
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(16, 176);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(143, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 16;
		this.labelLine8.Utext = "City";
		this.txt_BuildingFloorRoom.FieldName = null;
		this.txt_BuildingFloorRoom.IsSearchControl = false;
		this.txt_BuildingFloorRoom.Location = new System.Drawing.Point(157, 156);
		this.txt_BuildingFloorRoom.Name = "txt_BuildingFloorRoom";
		this.txt_BuildingFloorRoom.Properties.AutoHeight = false;
		this.txt_BuildingFloorRoom.Size = new System.Drawing.Size(345, 18);
		this.txt_BuildingFloorRoom.TabIndex = 13;
		this.txt_BuildingFloorRoom.EditValueChanged += new System.EventHandler(this.txt_BuildingFloorRoom_EditValueChanged);
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(16, 157);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(143, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 14;
		this.labelLine9.Utext = "Building/Floor/Room";
		this.txt_Block.FieldName = null;
		this.txt_Block.IsSearchControl = false;
		this.txt_Block.Location = new System.Drawing.Point(157, 137);
		this.txt_Block.Name = "txt_Block";
		this.txt_Block.Properties.AutoHeight = false;
		this.txt_Block.Size = new System.Drawing.Size(345, 18);
		this.txt_Block.TabIndex = 11;
		this.txt_Block.EditValueChanged += new System.EventHandler(this.txt_Block_EditValueChanged);
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(16, 138);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(143, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Block";
		this.txt_StreetNo.FieldName = null;
		this.txt_StreetNo.IsSearchControl = false;
		this.txt_StreetNo.Location = new System.Drawing.Point(157, 118);
		this.txt_StreetNo.Name = "txt_StreetNo";
		this.txt_StreetNo.Properties.AutoHeight = false;
		this.txt_StreetNo.Size = new System.Drawing.Size(345, 18);
		this.txt_StreetNo.TabIndex = 9;
		this.txt_StreetNo.EditValueChanged += new System.EventHandler(this.txt_StreetNo_EditValueChanged);
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(16, 119);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(143, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 10;
		this.labelLine3.Utext = "Street No.";
		this.txt_StreetPObox.FieldName = null;
		this.txt_StreetPObox.IsSearchControl = false;
		this.txt_StreetPObox.Location = new System.Drawing.Point(157, 99);
		this.txt_StreetPObox.Name = "txt_StreetPObox";
		this.txt_StreetPObox.Properties.AutoHeight = false;
		this.txt_StreetPObox.Size = new System.Drawing.Size(345, 18);
		this.txt_StreetPObox.TabIndex = 7;
		this.txt_StreetPObox.EditValueChanged += new System.EventHandler(this.txt_StreetPObox_EditValueChanged);
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(16, 100);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(143, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 8;
		this.labelLine2.Utext = "Street /PO box";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(16, 26);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(143, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Address";
		this.txt_CompanyName.FieldName = null;
		this.txt_CompanyName.IsSearchControl = false;
		this.txt_CompanyName.Location = new System.Drawing.Point(157, 7);
		this.txt_CompanyName.Name = "txt_CompanyName";
		this.txt_CompanyName.Properties.AutoHeight = false;
		this.txt_CompanyName.Size = new System.Drawing.Size(345, 18);
		this.txt_CompanyName.TabIndex = 3;
		this.txt_CompanyName.EditValueChanged += new System.EventHandler(this.txt_CompanyName_EditValueChanged);
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(16, 8);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(143, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 4;
		this.lb_Name.Utext = "Company Name";
		this.tp_FrgnLang.Controls.Add(this.lookUE_Country2);
		this.tp_FrgnLang.Controls.Add(this.lookUE_State2);
		this.tp_FrgnLang.Controls.Add(this.memoEdit_Address2);
		this.tp_FrgnLang.Controls.Add(this.txt_GLN2);
		this.tp_FrgnLang.Controls.Add(this.txt_Email2);
		this.tp_FrgnLang.Controls.Add(this.txt_Fax2);
		this.tp_FrgnLang.Controls.Add(this.txt_Telephone22);
		this.tp_FrgnLang.Controls.Add(this.txt_Telephone12);
		this.tp_FrgnLang.Controls.Add(this.txt_AliasName2);
		this.tp_FrgnLang.Controls.Add(this.txt_ActiveManager2);
		this.tp_FrgnLang.Controls.Add(this.labelLine26);
		this.tp_FrgnLang.Controls.Add(this.txt_PrintingHeader2);
		this.tp_FrgnLang.Controls.Add(this.labelLine27);
		this.tp_FrgnLang.Controls.Add(this.txt_InternetAddress2);
		this.tp_FrgnLang.Controls.Add(this.labelLine28);
		this.tp_FrgnLang.Controls.Add(this.labelLine29);
		this.tp_FrgnLang.Controls.Add(this.labelLine30);
		this.tp_FrgnLang.Controls.Add(this.txt_County2);
		this.tp_FrgnLang.Controls.Add(this.labelLine31);
		this.tp_FrgnLang.Controls.Add(this.txt_ZipCode2);
		this.tp_FrgnLang.Controls.Add(this.labelLine32);
		this.tp_FrgnLang.Controls.Add(this.txt_City2);
		this.tp_FrgnLang.Controls.Add(this.labelLine33);
		this.tp_FrgnLang.Controls.Add(this.txt_BuildingFloorRoom2);
		this.tp_FrgnLang.Controls.Add(this.labelLine34);
		this.tp_FrgnLang.Controls.Add(this.txt_Block2);
		this.tp_FrgnLang.Controls.Add(this.labelLine35);
		this.tp_FrgnLang.Controls.Add(this.txt_StreetNo2);
		this.tp_FrgnLang.Controls.Add(this.labelLine36);
		this.tp_FrgnLang.Controls.Add(this.txt_StreetPOBox2);
		this.tp_FrgnLang.Controls.Add(this.labelLine37);
		this.tp_FrgnLang.Controls.Add(this.labelLine38);
		this.tp_FrgnLang.Controls.Add(this.txt_CompanyName2);
		this.tp_FrgnLang.Controls.Add(this.labelLine39);
		this.tp_FrgnLang.Controls.Add(this.labelLine20);
		this.tp_FrgnLang.Controls.Add(this.labelLine21);
		this.tp_FrgnLang.Controls.Add(this.labelLine22);
		this.tp_FrgnLang.Controls.Add(this.labelLine23);
		this.tp_FrgnLang.Controls.Add(this.labelLine24);
		this.tp_FrgnLang.Controls.Add(this.labelLine25);
		this.tp_FrgnLang.FieldName = "";
		this.tp_FrgnLang.IsSearchControl = false;
		this.tp_FrgnLang.Name = "tp_FrgnLang";
		this.tp_FrgnLang.Size = new System.Drawing.Size(526, 483);
		this.tp_FrgnLang.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tp_FrgnLang.Text = "Foreign Language";
		this.lookUE_Country2.Location = new System.Drawing.Point(157, 251);
		this.lookUE_Country2.Name = "lookUE_Country2";
		this.lookUE_Country2.Properties.AutoHeight = false;
		this.lookUE_Country2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_Country2.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUE_Country2.Properties.DisplayMember = "Name";
		this.lookUE_Country2.Properties.NullText = "";
		this.lookUE_Country2.Properties.ValueMember = "Code";
		this.lookUE_Country2.Size = new System.Drawing.Size(345, 18);
		this.lookUE_Country2.TabIndex = 85;
		this.lookUE_Country2.EditValueChanged += new System.EventHandler(this.lookUE_Country2_EditValueChanged);
		this.lookUE_State2.Location = new System.Drawing.Point(157, 232);
		this.lookUE_State2.Name = "lookUE_State2";
		this.lookUE_State2.Properties.AutoHeight = false;
		this.lookUE_State2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_State2.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUE_State2.Properties.DisplayMember = "Name";
		this.lookUE_State2.Properties.NullText = "";
		this.lookUE_State2.Properties.ValueMember = "Code";
		this.lookUE_State2.Size = new System.Drawing.Size(345, 18);
		this.lookUE_State2.TabIndex = 84;
		this.lookUE_State2.EditValueChanged += new System.EventHandler(this.lookUE_State2_EditValueChanged);
		this.memoEdit_Address2.FieldName = null;
		this.memoEdit_Address2.IsSearchControl = false;
		this.memoEdit_Address2.Location = new System.Drawing.Point(157, 26);
		this.memoEdit_Address2.Name = "memoEdit_Address2";
		this.memoEdit_Address2.Properties.ReadOnly = true;
		this.memoEdit_Address2.Size = new System.Drawing.Size(345, 56);
		this.memoEdit_Address2.TabIndex = 83;
		this.memoEdit_Address2.EditValueChanged += new System.EventHandler(this.memoEdit_Address2_EditValueChanged);
		this.txt_GLN2.FieldName = null;
		this.txt_GLN2.IsSearchControl = false;
		this.txt_GLN2.Location = new System.Drawing.Point(157, 440);
		this.txt_GLN2.Name = "txt_GLN2";
		this.txt_GLN2.Properties.AutoHeight = false;
		this.txt_GLN2.Size = new System.Drawing.Size(345, 18);
		this.txt_GLN2.TabIndex = 81;
		this.txt_GLN2.EditValueChanged += new System.EventHandler(this.txt_GLN2_EditValueChanged);
		this.txt_Email2.FieldName = null;
		this.txt_Email2.IsSearchControl = false;
		this.txt_Email2.Location = new System.Drawing.Point(157, 421);
		this.txt_Email2.Name = "txt_Email2";
		this.txt_Email2.Properties.AutoHeight = false;
		this.txt_Email2.Size = new System.Drawing.Size(345, 18);
		this.txt_Email2.TabIndex = 79;
		this.txt_Email2.EditValueChanged += new System.EventHandler(this.txt_Email2_EditValueChanged);
		this.txt_Fax2.FieldName = null;
		this.txt_Fax2.IsSearchControl = false;
		this.txt_Fax2.Location = new System.Drawing.Point(157, 402);
		this.txt_Fax2.Name = "txt_Fax2";
		this.txt_Fax2.Properties.AutoHeight = false;
		this.txt_Fax2.Size = new System.Drawing.Size(345, 18);
		this.txt_Fax2.TabIndex = 77;
		this.txt_Fax2.EditValueChanged += new System.EventHandler(this.txt_Fax2_EditValueChanged);
		this.txt_Telephone22.FieldName = null;
		this.txt_Telephone22.IsSearchControl = false;
		this.txt_Telephone22.Location = new System.Drawing.Point(157, 383);
		this.txt_Telephone22.Name = "txt_Telephone22";
		this.txt_Telephone22.Properties.AutoHeight = false;
		this.txt_Telephone22.Size = new System.Drawing.Size(345, 18);
		this.txt_Telephone22.TabIndex = 75;
		this.txt_Telephone22.EditValueChanged += new System.EventHandler(this.txt_Telephone22_EditValueChanged);
		this.txt_Telephone12.FieldName = null;
		this.txt_Telephone12.IsSearchControl = false;
		this.txt_Telephone12.Location = new System.Drawing.Point(157, 364);
		this.txt_Telephone12.Name = "txt_Telephone12";
		this.txt_Telephone12.Properties.AutoHeight = false;
		this.txt_Telephone12.Size = new System.Drawing.Size(345, 18);
		this.txt_Telephone12.TabIndex = 73;
		this.txt_Telephone12.EditValueChanged += new System.EventHandler(this.txt_Telephone12_EditValueChanged);
		this.txt_AliasName2.FieldName = null;
		this.txt_AliasName2.IsSearchControl = false;
		this.txt_AliasName2.Location = new System.Drawing.Point(157, 345);
		this.txt_AliasName2.Name = "txt_AliasName2";
		this.txt_AliasName2.Properties.AutoHeight = false;
		this.txt_AliasName2.Size = new System.Drawing.Size(345, 18);
		this.txt_AliasName2.TabIndex = 71;
		this.txt_AliasName2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_AliasName2_EditValueChanging);
		this.txt_ActiveManager2.FieldName = null;
		this.txt_ActiveManager2.IsSearchControl = false;
		this.txt_ActiveManager2.Location = new System.Drawing.Point(157, 308);
		this.txt_ActiveManager2.Name = "txt_ActiveManager2";
		this.txt_ActiveManager2.Properties.AutoHeight = false;
		this.txt_ActiveManager2.Size = new System.Drawing.Size(345, 18);
		this.txt_ActiveManager2.TabIndex = 69;
		this.txt_ActiveManager2.EditValueChanged += new System.EventHandler(this.txt_ActiveManager2_EditValueChanged);
		this.labelLine26.FieldName = null;
		this.labelLine26.HideWhenEmpty = false;
		this.labelLine26.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine26.Location = new System.Drawing.Point(16, 309);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.ShowUnderLine = true;
		this.labelLine26.Size = new System.Drawing.Size(143, 20);
		this.labelLine26.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine26.TabIndex = 70;
		this.labelLine26.Utext = "Active Manager";
		this.txt_PrintingHeader2.FieldName = null;
		this.txt_PrintingHeader2.IsSearchControl = false;
		this.txt_PrintingHeader2.Location = new System.Drawing.Point(157, 289);
		this.txt_PrintingHeader2.Name = "txt_PrintingHeader2";
		this.txt_PrintingHeader2.Properties.AutoHeight = false;
		this.txt_PrintingHeader2.Size = new System.Drawing.Size(345, 18);
		this.txt_PrintingHeader2.TabIndex = 67;
		this.txt_PrintingHeader2.EditValueChanged += new System.EventHandler(this.txt_PrintingHeader2_EditValueChanged);
		this.labelLine27.FieldName = null;
		this.labelLine27.HideWhenEmpty = false;
		this.labelLine27.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine27.Location = new System.Drawing.Point(16, 290);
		this.labelLine27.Name = "labelLine27";
		this.labelLine27.ShowUnderLine = true;
		this.labelLine27.Size = new System.Drawing.Size(143, 20);
		this.labelLine27.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine27.TabIndex = 68;
		this.labelLine27.Utext = "Printing Header";
		this.txt_InternetAddress2.FieldName = null;
		this.txt_InternetAddress2.IsSearchControl = false;
		this.txt_InternetAddress2.Location = new System.Drawing.Point(157, 270);
		this.txt_InternetAddress2.Name = "txt_InternetAddress2";
		this.txt_InternetAddress2.Properties.AutoHeight = false;
		this.txt_InternetAddress2.Size = new System.Drawing.Size(345, 18);
		this.txt_InternetAddress2.TabIndex = 65;
		this.txt_InternetAddress2.EditValueChanged += new System.EventHandler(this.txt_InternetAddress2_EditValueChanged);
		this.labelLine28.FieldName = null;
		this.labelLine28.HideWhenEmpty = false;
		this.labelLine28.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine28.Location = new System.Drawing.Point(16, 271);
		this.labelLine28.Name = "labelLine28";
		this.labelLine28.ShowUnderLine = true;
		this.labelLine28.Size = new System.Drawing.Size(143, 20);
		this.labelLine28.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine28.TabIndex = 66;
		this.labelLine28.Utext = "Internet Address";
		this.labelLine29.FieldName = null;
		this.labelLine29.HideWhenEmpty = false;
		this.labelLine29.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine29.Location = new System.Drawing.Point(16, 253);
		this.labelLine29.Name = "labelLine29";
		this.labelLine29.ShowUnderLine = true;
		this.labelLine29.Size = new System.Drawing.Size(143, 20);
		this.labelLine29.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine29.TabIndex = 64;
		this.labelLine29.Utext = "Country";
		this.labelLine30.FieldName = null;
		this.labelLine30.HideWhenEmpty = false;
		this.labelLine30.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine30.Location = new System.Drawing.Point(16, 233);
		this.labelLine30.Name = "labelLine30";
		this.labelLine30.ShowUnderLine = true;
		this.labelLine30.Size = new System.Drawing.Size(143, 20);
		this.labelLine30.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine30.TabIndex = 62;
		this.labelLine30.Utext = "State";
		this.txt_County2.FieldName = null;
		this.txt_County2.IsSearchControl = false;
		this.txt_County2.Location = new System.Drawing.Point(157, 213);
		this.txt_County2.Name = "txt_County2";
		this.txt_County2.Properties.AutoHeight = false;
		this.txt_County2.Size = new System.Drawing.Size(345, 18);
		this.txt_County2.TabIndex = 59;
		this.txt_County2.EditValueChanged += new System.EventHandler(this.txt_County2_EditValueChanged);
		this.labelLine31.FieldName = null;
		this.labelLine31.HideWhenEmpty = false;
		this.labelLine31.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine31.Location = new System.Drawing.Point(16, 214);
		this.labelLine31.Name = "labelLine31";
		this.labelLine31.ShowUnderLine = true;
		this.labelLine31.Size = new System.Drawing.Size(143, 20);
		this.labelLine31.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine31.TabIndex = 60;
		this.labelLine31.Utext = "County";
		this.txt_ZipCode2.FieldName = null;
		this.txt_ZipCode2.IsSearchControl = false;
		this.txt_ZipCode2.Location = new System.Drawing.Point(157, 194);
		this.txt_ZipCode2.Name = "txt_ZipCode2";
		this.txt_ZipCode2.Properties.AutoHeight = false;
		this.txt_ZipCode2.Size = new System.Drawing.Size(345, 18);
		this.txt_ZipCode2.TabIndex = 57;
		this.txt_ZipCode2.EditValueChanged += new System.EventHandler(this.txt_ZipCode2_EditValueChanged);
		this.labelLine32.FieldName = null;
		this.labelLine32.HideWhenEmpty = false;
		this.labelLine32.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine32.Location = new System.Drawing.Point(16, 195);
		this.labelLine32.Name = "labelLine32";
		this.labelLine32.ShowUnderLine = true;
		this.labelLine32.Size = new System.Drawing.Size(143, 20);
		this.labelLine32.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine32.TabIndex = 58;
		this.labelLine32.Utext = "Zip Code";
		this.txt_City2.FieldName = null;
		this.txt_City2.IsSearchControl = false;
		this.txt_City2.Location = new System.Drawing.Point(157, 175);
		this.txt_City2.Name = "txt_City2";
		this.txt_City2.Properties.AutoHeight = false;
		this.txt_City2.Size = new System.Drawing.Size(345, 18);
		this.txt_City2.TabIndex = 55;
		this.txt_City2.EditValueChanged += new System.EventHandler(this.txt_City2_EditValueChanged);
		this.labelLine33.FieldName = null;
		this.labelLine33.HideWhenEmpty = false;
		this.labelLine33.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine33.Location = new System.Drawing.Point(16, 176);
		this.labelLine33.Name = "labelLine33";
		this.labelLine33.ShowUnderLine = true;
		this.labelLine33.Size = new System.Drawing.Size(143, 20);
		this.labelLine33.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine33.TabIndex = 56;
		this.labelLine33.Utext = "City";
		this.txt_BuildingFloorRoom2.FieldName = null;
		this.txt_BuildingFloorRoom2.IsSearchControl = false;
		this.txt_BuildingFloorRoom2.Location = new System.Drawing.Point(157, 156);
		this.txt_BuildingFloorRoom2.Name = "txt_BuildingFloorRoom2";
		this.txt_BuildingFloorRoom2.Properties.AutoHeight = false;
		this.txt_BuildingFloorRoom2.Size = new System.Drawing.Size(345, 18);
		this.txt_BuildingFloorRoom2.TabIndex = 53;
		this.txt_BuildingFloorRoom2.EditValueChanged += new System.EventHandler(this.txt_BuildingFloorRoom2_EditValueChanged);
		this.labelLine34.FieldName = null;
		this.labelLine34.HideWhenEmpty = false;
		this.labelLine34.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine34.Location = new System.Drawing.Point(16, 157);
		this.labelLine34.Name = "labelLine34";
		this.labelLine34.ShowUnderLine = true;
		this.labelLine34.Size = new System.Drawing.Size(143, 20);
		this.labelLine34.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine34.TabIndex = 54;
		this.labelLine34.Utext = "Building/Floor/Room";
		this.txt_Block2.FieldName = null;
		this.txt_Block2.IsSearchControl = false;
		this.txt_Block2.Location = new System.Drawing.Point(157, 137);
		this.txt_Block2.Name = "txt_Block2";
		this.txt_Block2.Properties.AutoHeight = false;
		this.txt_Block2.Size = new System.Drawing.Size(345, 18);
		this.txt_Block2.TabIndex = 51;
		this.txt_Block2.EditValueChanged += new System.EventHandler(this.txt_Block2_EditValueChanged);
		this.labelLine35.FieldName = null;
		this.labelLine35.HideWhenEmpty = false;
		this.labelLine35.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine35.Location = new System.Drawing.Point(16, 138);
		this.labelLine35.Name = "labelLine35";
		this.labelLine35.ShowUnderLine = true;
		this.labelLine35.Size = new System.Drawing.Size(143, 20);
		this.labelLine35.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine35.TabIndex = 52;
		this.labelLine35.Utext = "Block";
		this.txt_StreetNo2.FieldName = null;
		this.txt_StreetNo2.IsSearchControl = false;
		this.txt_StreetNo2.Location = new System.Drawing.Point(157, 118);
		this.txt_StreetNo2.Name = "txt_StreetNo2";
		this.txt_StreetNo2.Properties.AutoHeight = false;
		this.txt_StreetNo2.Size = new System.Drawing.Size(345, 18);
		this.txt_StreetNo2.TabIndex = 49;
		this.txt_StreetNo2.EditValueChanged += new System.EventHandler(this.txt_StreetNo2_EditValueChanged);
		this.labelLine36.FieldName = null;
		this.labelLine36.HideWhenEmpty = false;
		this.labelLine36.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine36.Location = new System.Drawing.Point(16, 119);
		this.labelLine36.Name = "labelLine36";
		this.labelLine36.ShowUnderLine = true;
		this.labelLine36.Size = new System.Drawing.Size(143, 20);
		this.labelLine36.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine36.TabIndex = 50;
		this.labelLine36.Utext = "Street No.";
		this.txt_StreetPOBox2.FieldName = null;
		this.txt_StreetPOBox2.IsSearchControl = false;
		this.txt_StreetPOBox2.Location = new System.Drawing.Point(157, 99);
		this.txt_StreetPOBox2.Name = "txt_StreetPOBox2";
		this.txt_StreetPOBox2.Properties.AutoHeight = false;
		this.txt_StreetPOBox2.Size = new System.Drawing.Size(345, 18);
		this.txt_StreetPOBox2.TabIndex = 47;
		this.txt_StreetPOBox2.EditValueChanged += new System.EventHandler(this.txt_StreetPOBox2_EditValueChanged);
		this.labelLine37.FieldName = null;
		this.labelLine37.HideWhenEmpty = false;
		this.labelLine37.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine37.Location = new System.Drawing.Point(16, 100);
		this.labelLine37.Name = "labelLine37";
		this.labelLine37.ShowUnderLine = true;
		this.labelLine37.Size = new System.Drawing.Size(143, 20);
		this.labelLine37.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine37.TabIndex = 48;
		this.labelLine37.Utext = "Street /PO box";
		this.labelLine38.FieldName = null;
		this.labelLine38.HideWhenEmpty = false;
		this.labelLine38.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine38.Location = new System.Drawing.Point(16, 26);
		this.labelLine38.Name = "labelLine38";
		this.labelLine38.ShowUnderLine = true;
		this.labelLine38.Size = new System.Drawing.Size(143, 20);
		this.labelLine38.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine38.TabIndex = 46;
		this.labelLine38.Utext = "Address";
		this.txt_CompanyName2.FieldName = null;
		this.txt_CompanyName2.IsSearchControl = false;
		this.txt_CompanyName2.Location = new System.Drawing.Point(157, 7);
		this.txt_CompanyName2.Name = "txt_CompanyName2";
		this.txt_CompanyName2.Properties.AutoHeight = false;
		this.txt_CompanyName2.Size = new System.Drawing.Size(345, 18);
		this.txt_CompanyName2.TabIndex = 44;
		this.txt_CompanyName2.EditValueChanged += new System.EventHandler(this.txt_CompanyName2_EditValueChanged);
		this.labelLine39.FieldName = null;
		this.labelLine39.HideWhenEmpty = false;
		this.labelLine39.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine39.Location = new System.Drawing.Point(16, 8);
		this.labelLine39.Name = "labelLine39";
		this.labelLine39.ShowUnderLine = true;
		this.labelLine39.Size = new System.Drawing.Size(143, 20);
		this.labelLine39.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine39.TabIndex = 45;
		this.labelLine39.Utext = "Company Name";
		this.labelLine20.FieldName = null;
		this.labelLine20.HideWhenEmpty = false;
		this.labelLine20.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine20.Location = new System.Drawing.Point(16, 441);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.ShowUnderLine = true;
		this.labelLine20.Size = new System.Drawing.Size(143, 20);
		this.labelLine20.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine20.TabIndex = 82;
		this.labelLine20.Utext = "GLN";
		this.labelLine21.FieldName = null;
		this.labelLine21.HideWhenEmpty = false;
		this.labelLine21.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine21.Location = new System.Drawing.Point(16, 422);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.ShowUnderLine = true;
		this.labelLine21.Size = new System.Drawing.Size(143, 20);
		this.labelLine21.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine21.TabIndex = 80;
		this.labelLine21.Utext = "E-Mail";
		this.labelLine22.FieldName = null;
		this.labelLine22.HideWhenEmpty = false;
		this.labelLine22.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine22.Location = new System.Drawing.Point(16, 403);
		this.labelLine22.Name = "labelLine22";
		this.labelLine22.ShowUnderLine = true;
		this.labelLine22.Size = new System.Drawing.Size(143, 20);
		this.labelLine22.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine22.TabIndex = 78;
		this.labelLine22.Utext = "Fax";
		this.labelLine23.FieldName = null;
		this.labelLine23.HideWhenEmpty = false;
		this.labelLine23.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine23.Location = new System.Drawing.Point(16, 384);
		this.labelLine23.Name = "labelLine23";
		this.labelLine23.ShowUnderLine = true;
		this.labelLine23.Size = new System.Drawing.Size(143, 20);
		this.labelLine23.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine23.TabIndex = 76;
		this.labelLine23.Utext = "Telephone 2";
		this.labelLine24.FieldName = null;
		this.labelLine24.HideWhenEmpty = false;
		this.labelLine24.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine24.Location = new System.Drawing.Point(16, 365);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.ShowUnderLine = true;
		this.labelLine24.Size = new System.Drawing.Size(143, 20);
		this.labelLine24.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine24.TabIndex = 74;
		this.labelLine24.Utext = "Telephone 1";
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine25.Location = new System.Drawing.Point(16, 346);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(143, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 72;
		this.labelLine25.Utext = "Alias Name";
		this.tp_AccountingData.Controls.Add(this.txt_CompanyTaxRate);
		this.tp_AccountingData.Controls.Add(this.luk_TaxRDetermination);
		this.tp_AccountingData.Controls.Add(this.lookUpEdit_Holiday);
		this.tp_AccountingData.Controls.Add(this.chk_ExtendedTaxReporting);
		this.tp_AccountingData.Controls.Add(this.chk_ApplyExchangeRateOnDeferredTax);
		this.tp_AccountingData.Controls.Add(this.chk_UseDeferredTax);
		this.tp_AccountingData.Controls.Add(this.labelLine41);
		this.tp_AccountingData.Controls.Add(this.labelLine42);
		this.tp_AccountingData.Controls.Add(this.txt_TaxOfficial2);
		this.tp_AccountingData.Controls.Add(this.labelLine43);
		this.tp_AccountingData.Controls.Add(this.txt_TaxDeductionNumber);
		this.tp_AccountingData.Controls.Add(this.labelLine44);
		this.tp_AccountingData.Controls.Add(this.txt_ExemptionNumber);
		this.tp_AccountingData.Controls.Add(this.labelLine45);
		this.tp_AccountingData.Controls.Add(this.labelLine46);
		this.tp_AccountingData.Controls.Add(this.txt_FreeZoneNo);
		this.tp_AccountingData.Controls.Add(this.labelLine47);
		this.tp_AccountingData.Controls.Add(this.txt_FederalTaxID3);
		this.tp_AccountingData.Controls.Add(this.labelLine48);
		this.tp_AccountingData.Controls.Add(this.txt_FederalTaxID2);
		this.tp_AccountingData.Controls.Add(this.labelLine49);
		this.tp_AccountingData.Controls.Add(this.txt_FederalTaxID1);
		this.tp_AccountingData.Controls.Add(this.labelLine50);
		this.tp_AccountingData.Controls.Add(this.txt_TaxOffice);
		this.tp_AccountingData.Controls.Add(this.labelLine51);
		this.tp_AccountingData.FieldName = "";
		this.tp_AccountingData.IsSearchControl = false;
		this.tp_AccountingData.Name = "tp_AccountingData";
		this.tp_AccountingData.Size = new System.Drawing.Size(533, 510);
		this.tp_AccountingData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tp_AccountingData.Text = "Accounting Data";
		this.txt_CompanyTaxRate.FieldName = null;
		this.txt_CompanyTaxRate.IsSearchControl = false;
		this.txt_CompanyTaxRate.Location = new System.Drawing.Point(161, 146);
		this.txt_CompanyTaxRate.Name = "txt_CompanyTaxRate";
		this.txt_CompanyTaxRate.Properties.AutoHeight = false;
		this.txt_CompanyTaxRate.Properties.IsSearchControl = false;
		this.txt_CompanyTaxRate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_CompanyTaxRate.Size = new System.Drawing.Size(345, 18);
		this.txt_CompanyTaxRate.TabIndex = 41;
		this.txt_CompanyTaxRate.EditValueChanged += new System.EventHandler(this.txt_CompanyTaxRate_EditValueChanged);
		this.luk_TaxRDetermination.Location = new System.Drawing.Point(161, 315);
		this.luk_TaxRDetermination.Name = "luk_TaxRDetermination";
		this.luk_TaxRDetermination.Properties.AutoHeight = false;
		this.luk_TaxRDetermination.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TaxRDetermination.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_TaxRDetermination.Properties.DisplayMember = "Name";
		this.luk_TaxRDetermination.Properties.NullText = "";
		this.luk_TaxRDetermination.Properties.ValueMember = "Code";
		this.luk_TaxRDetermination.Size = new System.Drawing.Size(344, 18);
		this.luk_TaxRDetermination.TabIndex = 77;
		this.lookUpEdit_Holiday.Location = new System.Drawing.Point(161, 336);
		this.lookUpEdit_Holiday.Name = "lookUpEdit_Holiday";
		this.lookUpEdit_Holiday.Properties.AutoHeight = false;
		this.lookUpEdit_Holiday.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.lookUpEdit_Holiday.Properties.DisplayMember = "HldCode";
		this.lookUpEdit_Holiday.Properties.NullText = "";
		this.lookUpEdit_Holiday.Properties.ValueMember = "HldCode";
		this.lookUpEdit_Holiday.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lookUpEdit_Holiday_Properties_ButtonClick);
		this.lookUpEdit_Holiday.Size = new System.Drawing.Size(345, 18);
		this.lookUpEdit_Holiday.TabIndex = 56;
		this.chk_ExtendedTaxReporting.FieldName = null;
		this.chk_ExtendedTaxReporting.IsSearchControl = false;
		this.chk_ExtendedTaxReporting.Location = new System.Drawing.Point(19, 400);
		this.chk_ExtendedTaxReporting.Name = "chk_ExtendedTaxReporting";
		this.chk_ExtendedTaxReporting.Properties.Caption = "Extended Tax Reporting";
		this.chk_ExtendedTaxReporting.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ExtendedTaxReporting.Properties.ValueChecked = "Y";
		this.chk_ExtendedTaxReporting.Properties.ValueUnchecked = "N";
		this.chk_ExtendedTaxReporting.Size = new System.Drawing.Size(143, 20);
		this.chk_ExtendedTaxReporting.TabIndex = 55;
		this.chk_ExtendedTaxReporting.CheckedChanged += new System.EventHandler(this.chk_ExtendedTaxReporting_CheckedChanged);
		this.chk_ApplyExchangeRateOnDeferredTax.FieldName = null;
		this.chk_ApplyExchangeRateOnDeferredTax.IsSearchControl = false;
		this.chk_ApplyExchangeRateOnDeferredTax.Location = new System.Drawing.Point(19, 283);
		this.chk_ApplyExchangeRateOnDeferredTax.Name = "chk_ApplyExchangeRateOnDeferredTax";
		this.chk_ApplyExchangeRateOnDeferredTax.Properties.Caption = "Apply Exchange Rate on Deferred Tax";
		this.chk_ApplyExchangeRateOnDeferredTax.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ApplyExchangeRateOnDeferredTax.Properties.ValueChecked = "Y";
		this.chk_ApplyExchangeRateOnDeferredTax.Properties.ValueUnchecked = "N";
		this.chk_ApplyExchangeRateOnDeferredTax.Size = new System.Drawing.Size(212, 20);
		this.chk_ApplyExchangeRateOnDeferredTax.TabIndex = 54;
		this.chk_ApplyExchangeRateOnDeferredTax.CheckedChanged += new System.EventHandler(this.chk_ApplyExchangeRateOnDeferredTax_CheckedChanged);
		this.chk_UseDeferredTax.FieldName = null;
		this.chk_UseDeferredTax.IsSearchControl = false;
		this.chk_UseDeferredTax.Location = new System.Drawing.Point(19, 231);
		this.chk_UseDeferredTax.Name = "chk_UseDeferredTax";
		this.chk_UseDeferredTax.Properties.Caption = "Use Deferred Tax";
		this.chk_UseDeferredTax.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_UseDeferredTax.Properties.ValueChecked = "Y";
		this.chk_UseDeferredTax.Properties.ValueUnchecked = "N";
		this.chk_UseDeferredTax.Properties.CheckedChanged += new System.EventHandler(this.chk_UseDeferredTax_Properties_CheckedChanged);
		this.chk_UseDeferredTax.Size = new System.Drawing.Size(112, 20);
		this.chk_UseDeferredTax.TabIndex = 53;
		this.labelLine41.FieldName = null;
		this.labelLine41.HideWhenEmpty = false;
		this.labelLine41.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine41.Location = new System.Drawing.Point(19, 337);
		this.labelLine41.Name = "labelLine41";
		this.labelLine41.ShowUnderLine = true;
		this.labelLine41.Size = new System.Drawing.Size(143, 20);
		this.labelLine41.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine41.TabIndex = 52;
		this.labelLine41.Utext = "Holidays";
		this.labelLine42.FieldName = null;
		this.labelLine42.HideWhenEmpty = false;
		this.labelLine42.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine42.Location = new System.Drawing.Point(19, 316);
		this.labelLine42.Name = "labelLine42";
		this.labelLine42.ShowUnderLine = true;
		this.labelLine42.Size = new System.Drawing.Size(143, 20);
		this.labelLine42.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine42.TabIndex = 50;
		this.labelLine42.Utext = "Tax Rate Determination";
		this.txt_TaxOfficial2.FieldName = null;
		this.txt_TaxOfficial2.IsSearchControl = false;
		this.txt_TaxOfficial2.Location = new System.Drawing.Point(161, 209);
		this.txt_TaxOfficial2.Name = "txt_TaxOfficial2";
		this.txt_TaxOfficial2.Properties.AutoHeight = false;
		this.txt_TaxOfficial2.Size = new System.Drawing.Size(345, 18);
		this.txt_TaxOfficial2.TabIndex = 47;
		this.txt_TaxOfficial2.EditValueChanged += new System.EventHandler(this.txt_TaxOfficial2_EditValueChanged);
		this.labelLine43.FieldName = null;
		this.labelLine43.HideWhenEmpty = false;
		this.labelLine43.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine43.Location = new System.Drawing.Point(19, 210);
		this.labelLine43.Name = "labelLine43";
		this.labelLine43.ShowUnderLine = true;
		this.labelLine43.Size = new System.Drawing.Size(143, 20);
		this.labelLine43.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine43.TabIndex = 48;
		this.labelLine43.Utext = "Tax Official";
		this.txt_TaxDeductionNumber.FieldName = null;
		this.txt_TaxDeductionNumber.IsSearchControl = false;
		this.txt_TaxDeductionNumber.Location = new System.Drawing.Point(161, 188);
		this.txt_TaxDeductionNumber.Name = "txt_TaxDeductionNumber";
		this.txt_TaxDeductionNumber.Properties.AutoHeight = false;
		this.txt_TaxDeductionNumber.Size = new System.Drawing.Size(345, 18);
		this.txt_TaxDeductionNumber.TabIndex = 45;
		this.txt_TaxDeductionNumber.EditValueChanged += new System.EventHandler(this.txt_TaxDeductionNumber_EditValueChanged);
		this.labelLine44.FieldName = null;
		this.labelLine44.HideWhenEmpty = false;
		this.labelLine44.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine44.Location = new System.Drawing.Point(19, 189);
		this.labelLine44.Name = "labelLine44";
		this.labelLine44.ShowUnderLine = true;
		this.labelLine44.Size = new System.Drawing.Size(143, 20);
		this.labelLine44.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine44.TabIndex = 46;
		this.labelLine44.Utext = "Tax Deduction Number";
		this.txt_ExemptionNumber.FieldName = null;
		this.txt_ExemptionNumber.IsSearchControl = false;
		this.txt_ExemptionNumber.Location = new System.Drawing.Point(161, 167);
		this.txt_ExemptionNumber.Name = "txt_ExemptionNumber";
		this.txt_ExemptionNumber.Properties.AutoHeight = false;
		this.txt_ExemptionNumber.Size = new System.Drawing.Size(345, 18);
		this.txt_ExemptionNumber.TabIndex = 43;
		this.txt_ExemptionNumber.EditValueChanged += new System.EventHandler(this.txt_ExemptionNumber_EditValueChanged);
		this.labelLine45.FieldName = null;
		this.labelLine45.HideWhenEmpty = false;
		this.labelLine45.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine45.Location = new System.Drawing.Point(19, 168);
		this.labelLine45.Name = "labelLine45";
		this.labelLine45.ShowUnderLine = true;
		this.labelLine45.Size = new System.Drawing.Size(143, 20);
		this.labelLine45.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine45.TabIndex = 44;
		this.labelLine45.Utext = "Exemption Number";
		this.labelLine46.FieldName = null;
		this.labelLine46.HideWhenEmpty = false;
		this.labelLine46.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine46.Location = new System.Drawing.Point(19, 147);
		this.labelLine46.Name = "labelLine46";
		this.labelLine46.ShowUnderLine = true;
		this.labelLine46.Size = new System.Drawing.Size(143, 20);
		this.labelLine46.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine46.TabIndex = 42;
		this.labelLine46.Utext = "Company Tax Rate";
		this.txt_FreeZoneNo.FieldName = null;
		this.txt_FreeZoneNo.IsSearchControl = false;
		this.txt_FreeZoneNo.Location = new System.Drawing.Point(161, 98);
		this.txt_FreeZoneNo.Name = "txt_FreeZoneNo";
		this.txt_FreeZoneNo.Properties.AutoHeight = false;
		this.txt_FreeZoneNo.Size = new System.Drawing.Size(345, 18);
		this.txt_FreeZoneNo.TabIndex = 39;
		this.txt_FreeZoneNo.EditValueChanged += new System.EventHandler(this.txt_FreeZoneNo_EditValueChanged);
		this.labelLine47.FieldName = null;
		this.labelLine47.HideWhenEmpty = false;
		this.labelLine47.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine47.Location = new System.Drawing.Point(19, 99);
		this.labelLine47.Name = "labelLine47";
		this.labelLine47.ShowUnderLine = true;
		this.labelLine47.Size = new System.Drawing.Size(143, 20);
		this.labelLine47.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine47.TabIndex = 40;
		this.labelLine47.Utext = "Additional Tax ID ";
		this.txt_FederalTaxID3.FieldName = null;
		this.txt_FederalTaxID3.IsSearchControl = false;
		this.txt_FederalTaxID3.Location = new System.Drawing.Point(161, 77);
		this.txt_FederalTaxID3.Name = "txt_FederalTaxID3";
		this.txt_FederalTaxID3.Properties.AutoHeight = false;
		this.txt_FederalTaxID3.Size = new System.Drawing.Size(345, 18);
		this.txt_FederalTaxID3.TabIndex = 37;
		this.txt_FederalTaxID3.EditValueChanged += new System.EventHandler(this.txt_FederalTaxID3_EditValueChanged);
		this.labelLine48.FieldName = null;
		this.labelLine48.HideWhenEmpty = false;
		this.labelLine48.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine48.Location = new System.Drawing.Point(19, 78);
		this.labelLine48.Name = "labelLine48";
		this.labelLine48.ShowUnderLine = true;
		this.labelLine48.Size = new System.Drawing.Size(143, 20);
		this.labelLine48.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine48.TabIndex = 38;
		this.labelLine48.Utext = "Federal Tax ID 3";
		this.txt_FederalTaxID2.FieldName = null;
		this.txt_FederalTaxID2.IsSearchControl = false;
		this.txt_FederalTaxID2.Location = new System.Drawing.Point(161, 56);
		this.txt_FederalTaxID2.Name = "txt_FederalTaxID2";
		this.txt_FederalTaxID2.Properties.AutoHeight = false;
		this.txt_FederalTaxID2.Size = new System.Drawing.Size(345, 18);
		this.txt_FederalTaxID2.TabIndex = 35;
		this.txt_FederalTaxID2.EditValueChanged += new System.EventHandler(this.txt_FederalTaxID2_EditValueChanged);
		this.labelLine49.FieldName = null;
		this.labelLine49.HideWhenEmpty = false;
		this.labelLine49.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine49.Location = new System.Drawing.Point(19, 57);
		this.labelLine49.Name = "labelLine49";
		this.labelLine49.ShowUnderLine = true;
		this.labelLine49.Size = new System.Drawing.Size(143, 20);
		this.labelLine49.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine49.TabIndex = 36;
		this.labelLine49.Utext = "Federal Tax ID 2";
		this.txt_FederalTaxID1.FieldName = null;
		this.txt_FederalTaxID1.IsSearchControl = false;
		this.txt_FederalTaxID1.Location = new System.Drawing.Point(161, 35);
		this.txt_FederalTaxID1.Name = "txt_FederalTaxID1";
		this.txt_FederalTaxID1.Properties.AutoHeight = false;
		this.txt_FederalTaxID1.Size = new System.Drawing.Size(345, 18);
		this.txt_FederalTaxID1.TabIndex = 33;
		this.txt_FederalTaxID1.EditValueChanged += new System.EventHandler(this.txt_FederalTaxID1_EditValueChanged);
		this.labelLine50.FieldName = null;
		this.labelLine50.HideWhenEmpty = false;
		this.labelLine50.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine50.Location = new System.Drawing.Point(19, 36);
		this.labelLine50.Name = "labelLine50";
		this.labelLine50.ShowUnderLine = true;
		this.labelLine50.Size = new System.Drawing.Size(143, 20);
		this.labelLine50.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine50.TabIndex = 34;
		this.labelLine50.Utext = "Federal Tax ID 1";
		this.txt_TaxOffice.FieldName = null;
		this.txt_TaxOffice.IsSearchControl = false;
		this.txt_TaxOffice.Location = new System.Drawing.Point(161, 14);
		this.txt_TaxOffice.Name = "txt_TaxOffice";
		this.txt_TaxOffice.Properties.AutoHeight = false;
		this.txt_TaxOffice.Size = new System.Drawing.Size(345, 18);
		this.txt_TaxOffice.TabIndex = 31;
		this.txt_TaxOffice.EditValueChanged += new System.EventHandler(this.txt_TaxOffice_EditValueChanged);
		this.labelLine51.FieldName = null;
		this.labelLine51.HideWhenEmpty = false;
		this.labelLine51.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine51.Location = new System.Drawing.Point(19, 15);
		this.labelLine51.Name = "labelLine51";
		this.labelLine51.ShowUnderLine = true;
		this.labelLine51.Size = new System.Drawing.Size(143, 20);
		this.labelLine51.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine51.TabIndex = 32;
		this.labelLine51.Utext = "Tax Office";
		this.tp_BasicInit.Controls.Add(this.look_SystemCurrency);
		this.tp_BasicInit.Controls.Add(this.look_LocalCurrency);
		this.tp_BasicInit.Controls.Add(this.luk_DefaultAccountCurrency);
		this.tp_BasicInit.Controls.Add(this.luk_ItmGrpValuationMthd);
		this.tp_BasicInit.Controls.Add(this.luk_CalcDepreciationBy);
		this.tp_BasicInit.Controls.Add(this.label1);
		this.tp_BasicInit.Controls.Add(this.groupBox1);
		this.tp_BasicInit.Controls.Add(this.txt_OrderingParty);
		this.tp_BasicInit.Controls.Add(this.labelLine40);
		this.tp_BasicInit.Controls.Add(this.labelLine57);
		this.tp_BasicInit.Controls.Add(this.labelLine55);
		this.tp_BasicInit.Controls.Add(this.chk_InstallBankStatementPr);
		this.tp_BasicInit.Controls.Add(this.chk_UsePurchaseAccountPostingSystem);
		this.tp_BasicInit.Controls.Add(this.chk_AllowSelection);
		this.tp_BasicInit.Controls.Add(this.chk_MaskCreditCardNumber);
		this.tp_BasicInit.Controls.Add(this.chk_EnableAdvanced);
		this.tp_BasicInit.Controls.Add(this.chk_EnableMultiBranch);
		this.tp_BasicInit.Controls.Add(this.chk_EnableFixedAssets);
		this.tp_BasicInit.Controls.Add(this.chk_AllowStockRelease);
		this.tp_BasicInit.Controls.Add(this.chk_ManageItemCost);
		this.tp_BasicInit.Controls.Add(this.chk_UsePerpetalInventory);
		this.tp_BasicInit.Controls.Add(this.chk_MultiLangauge);
		this.tp_BasicInit.Controls.Add(this.chk_PermitMore);
		this.tp_BasicInit.Controls.Add(this.chk_AllowNegativeAmounts);
		this.tp_BasicInit.Controls.Add(this.chk_UseSegmentationAccounts);
		this.tp_BasicInit.Controls.Add(this.chk_DispCreditBalance);
		this.tp_BasicInit.Controls.Add(this.lb_DefaultAccountCurrency);
		this.tp_BasicInit.Controls.Add(this.lb_SystemCurrency);
		this.tp_BasicInit.Controls.Add(this.lb_LocalCurrency);
		this.tp_BasicInit.Controls.Add(this.txt_ChartOfAccountsTemplate);
		this.tp_BasicInit.Controls.Add(this.lb_ChartOfAccountsTemplate);
		this.tp_BasicInit.Controls.Add(this.rdG_MngSerialNBatchCostBy);
		this.tp_BasicInit.FieldName = "";
		this.tp_BasicInit.IsSearchControl = false;
		this.tp_BasicInit.Name = "tp_BasicInit";
		this.tp_BasicInit.Size = new System.Drawing.Size(533, 510);
		this.tp_BasicInit.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tp_BasicInit.Text = "Basic Initialization";
		this.look_SystemCurrency.Enabled = false;
		this.look_SystemCurrency.Location = new System.Drawing.Point(198, 43);
		this.look_SystemCurrency.Name = "look_SystemCurrency";
		this.look_SystemCurrency.Properties.AutoHeight = false;
		this.look_SystemCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_SystemCurrency.Properties.NullText = "";
		this.look_SystemCurrency.Size = new System.Drawing.Size(307, 18);
		this.look_SystemCurrency.TabIndex = 80;
		this.look_LocalCurrency.Enabled = false;
		this.look_LocalCurrency.Location = new System.Drawing.Point(198, 24);
		this.look_LocalCurrency.Name = "look_LocalCurrency";
		this.look_LocalCurrency.Properties.AutoHeight = false;
		this.look_LocalCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_LocalCurrency.Properties.NullText = "";
		this.look_LocalCurrency.Size = new System.Drawing.Size(307, 18);
		this.look_LocalCurrency.TabIndex = 79;
		this.luk_DefaultAccountCurrency.Location = new System.Drawing.Point(198, 62);
		this.luk_DefaultAccountCurrency.Name = "luk_DefaultAccountCurrency";
		this.luk_DefaultAccountCurrency.Properties.AutoHeight = false;
		this.luk_DefaultAccountCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DefaultAccountCurrency.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_DefaultAccountCurrency.Properties.DisplayMember = "Name";
		this.luk_DefaultAccountCurrency.Properties.NullText = "";
		this.luk_DefaultAccountCurrency.Properties.ValueMember = "Code";
		this.luk_DefaultAccountCurrency.Size = new System.Drawing.Size(307, 18);
		this.luk_DefaultAccountCurrency.TabIndex = 78;
		this.luk_DefaultAccountCurrency.EditValueChanged += new System.EventHandler(this.luk_DefaultAccountCurrency_EditValueChanged);
		this.luk_ItmGrpValuationMthd.Location = new System.Drawing.Point(273, 172);
		this.luk_ItmGrpValuationMthd.Name = "luk_ItmGrpValuationMthd";
		this.luk_ItmGrpValuationMthd.Properties.AutoHeight = false;
		this.luk_ItmGrpValuationMthd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItmGrpValuationMthd.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_ItmGrpValuationMthd.Properties.DisplayMember = "Name";
		this.luk_ItmGrpValuationMthd.Properties.NullText = "";
		this.luk_ItmGrpValuationMthd.Properties.ValueMember = "Code";
		this.luk_ItmGrpValuationMthd.Size = new System.Drawing.Size(232, 18);
		this.luk_ItmGrpValuationMthd.TabIndex = 76;
		this.luk_CalcDepreciationBy.Location = new System.Drawing.Point(160, 441);
		this.luk_CalcDepreciationBy.Name = "luk_CalcDepreciationBy";
		this.luk_CalcDepreciationBy.Properties.AutoHeight = false;
		this.luk_CalcDepreciationBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CalcDepreciationBy.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_CalcDepreciationBy.Properties.DisplayMember = "Name";
		this.luk_CalcDepreciationBy.Properties.NullText = "";
		this.luk_CalcDepreciationBy.Properties.ValueMember = "Code";
		this.luk_CalcDepreciationBy.Size = new System.Drawing.Size(345, 18);
		this.luk_CalcDepreciationBy.TabIndex = 75;
		this.luk_CalcDepreciationBy.EditValueChanged += new System.EventHandler(this.luk_CalcDepreciationBy_EditValueChanged);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(341, 197);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(165, 13);
		this.label1.TabIndex = 73;
		this.label1.Text = "Manage Serial and Batch Cost By";
		this.groupBox1.Controls.Add(this.lookUE_DefaulAccountNo);
		this.groupBox1.Controls.Add(this.lookUE_DefaultBank);
		this.groupBox1.Controls.Add(this.lookUE_DefaultBankCountry);
		this.groupBox1.Controls.Add(this.txt_DefaultBankBranch);
		this.groupBox1.Controls.Add(this.labelLine58);
		this.groupBox1.Controls.Add(this.labelLine59);
		this.groupBox1.Controls.Add(this.labelLine60);
		this.groupBox1.Controls.Add(this.labelLine61);
		this.groupBox1.Location = new System.Drawing.Point(18, 297);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(487, 95);
		this.groupBox1.TabIndex = 71;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "House Bank";
		this.lookUE_DefaulAccountNo.Location = new System.Drawing.Point(167, 52);
		this.lookUE_DefaulAccountNo.Name = "lookUE_DefaulAccountNo";
		this.lookUE_DefaulAccountNo.Properties.AutoHeight = false;
		this.lookUE_DefaulAccountNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_DefaulAccountNo.Properties.NullText = "";
		this.lookUE_DefaulAccountNo.Size = new System.Drawing.Size(314, 18);
		this.lookUE_DefaulAccountNo.TabIndex = 55;
		this.lookUE_DefaulAccountNo.EditValueChanged += new System.EventHandler(this.lookUE_DefaulAccountNo_EditValueChanged);
		this.lookUE_DefaultBank.Location = new System.Drawing.Point(167, 32);
		this.lookUE_DefaultBank.Name = "lookUE_DefaultBank";
		this.lookUE_DefaultBank.Properties.AutoHeight = false;
		this.lookUE_DefaultBank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_DefaultBank.Properties.NullText = "";
		this.lookUE_DefaultBank.Size = new System.Drawing.Size(314, 18);
		this.lookUE_DefaultBank.TabIndex = 55;
		this.lookUE_DefaultBank.EditValueChanged += new System.EventHandler(this.lookUE_DefaultBank_EditValueChanged);
		this.lookUE_DefaultBankCountry.Location = new System.Drawing.Point(167, 12);
		this.lookUE_DefaultBankCountry.Name = "lookUE_DefaultBankCountry";
		this.lookUE_DefaultBankCountry.Properties.AutoHeight = false;
		this.lookUE_DefaultBankCountry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_DefaultBankCountry.Properties.NullText = "";
		this.lookUE_DefaultBankCountry.Size = new System.Drawing.Size(314, 18);
		this.lookUE_DefaultBankCountry.TabIndex = 55;
		this.lookUE_DefaultBankCountry.EditValueChanged += new System.EventHandler(this.lookUE_DefaultBankCountry_EditValueChanged);
		this.txt_DefaultBankBranch.FieldName = null;
		this.txt_DefaultBankBranch.IsSearchControl = false;
		this.txt_DefaultBankBranch.Location = new System.Drawing.Point(167, 72);
		this.txt_DefaultBankBranch.Name = "txt_DefaultBankBranch";
		this.txt_DefaultBankBranch.Properties.AutoHeight = false;
		this.txt_DefaultBankBranch.Size = new System.Drawing.Size(314, 18);
		this.txt_DefaultBankBranch.TabIndex = 53;
		this.txt_DefaultBankBranch.EditValueChanged += new System.EventHandler(this.txt_DefaultBankBranch_EditValueChanged);
		this.labelLine58.FieldName = null;
		this.labelLine58.HideWhenEmpty = false;
		this.labelLine58.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine58.Location = new System.Drawing.Point(0, 73);
		this.labelLine58.Name = "labelLine58";
		this.labelLine58.ShowUnderLine = true;
		this.labelLine58.Size = new System.Drawing.Size(173, 20);
		this.labelLine58.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine58.TabIndex = 54;
		this.labelLine58.Utext = "Default Branch";
		this.labelLine59.FieldName = null;
		this.labelLine59.HideWhenEmpty = false;
		this.labelLine59.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine59.Location = new System.Drawing.Point(0, 53);
		this.labelLine59.Name = "labelLine59";
		this.labelLine59.ShowUnderLine = true;
		this.labelLine59.Size = new System.Drawing.Size(173, 20);
		this.labelLine59.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine59.TabIndex = 52;
		this.labelLine59.Utext = "Default Account No.";
		this.labelLine60.FieldName = null;
		this.labelLine60.HideWhenEmpty = false;
		this.labelLine60.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine60.Location = new System.Drawing.Point(0, 33);
		this.labelLine60.Name = "labelLine60";
		this.labelLine60.ShowUnderLine = true;
		this.labelLine60.Size = new System.Drawing.Size(173, 20);
		this.labelLine60.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine60.TabIndex = 50;
		this.labelLine60.Utext = "Default Bank";
		this.labelLine61.FieldName = null;
		this.labelLine61.HideWhenEmpty = false;
		this.labelLine61.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine61.Location = new System.Drawing.Point(0, 13);
		this.labelLine61.Name = "labelLine61";
		this.labelLine61.ShowUnderLine = true;
		this.labelLine61.Size = new System.Drawing.Size(173, 20);
		this.labelLine61.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine61.TabIndex = 48;
		this.labelLine61.Utext = "Default Bank Country";
		this.txt_OrderingParty.FieldName = null;
		this.txt_OrderingParty.IsSearchControl = false;
		this.txt_OrderingParty.Location = new System.Drawing.Point(185, 275);
		this.txt_OrderingParty.Name = "txt_OrderingParty";
		this.txt_OrderingParty.Properties.AutoHeight = false;
		this.txt_OrderingParty.Size = new System.Drawing.Size(320, 18);
		this.txt_OrderingParty.TabIndex = 69;
		this.txt_OrderingParty.EditValueChanged += new System.EventHandler(this.txt_OrderingParty_EditValueChanged);
		this.labelLine40.FieldName = null;
		this.labelLine40.HideWhenEmpty = false;
		this.labelLine40.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine40.Location = new System.Drawing.Point(18, 442);
		this.labelLine40.Name = "labelLine40";
		this.labelLine40.ShowUnderLine = true;
		this.labelLine40.Size = new System.Drawing.Size(143, 20);
		this.labelLine40.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine40.TabIndex = 70;
		this.labelLine40.Utext = "Calculate Depreciation By";
		this.labelLine57.FieldName = null;
		this.labelLine57.HideWhenEmpty = false;
		this.labelLine57.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine57.Location = new System.Drawing.Point(18, 276);
		this.labelLine57.Name = "labelLine57";
		this.labelLine57.ShowUnderLine = true;
		this.labelLine57.Size = new System.Drawing.Size(173, 20);
		this.labelLine57.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine57.TabIndex = 70;
		this.labelLine57.Utext = "Ordering Party";
		this.labelLine55.FieldName = null;
		this.labelLine55.HideWhenEmpty = false;
		this.labelLine55.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine55.Location = new System.Drawing.Point(18, 173);
		this.labelLine55.Name = "labelLine55";
		this.labelLine55.ShowUnderLine = true;
		this.labelLine55.Size = new System.Drawing.Size(256, 20);
		this.labelLine55.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine55.TabIndex = 63;
		this.labelLine55.Utext = "Item Groups Valuation Method";
		this.chk_InstallBankStatementPr.EditValue = "N";
		this.chk_InstallBankStatementPr.FieldName = null;
		this.chk_InstallBankStatementPr.IsSearchControl = false;
		this.chk_InstallBankStatementPr.Location = new System.Drawing.Point(18, 395);
		this.chk_InstallBankStatementPr.Name = "chk_InstallBankStatementPr";
		this.chk_InstallBankStatementPr.Properties.Caption = "Install Bank Stattement Processing";
		this.chk_InstallBankStatementPr.Properties.ValueChecked = "Y";
		this.chk_InstallBankStatementPr.Properties.ValueUnchecked = "N";
		this.chk_InstallBankStatementPr.Properties.CheckedChanged += new System.EventHandler(this.chk_InstallBankStatementPr_Properties_CheckedChanged);
		this.chk_InstallBankStatementPr.Size = new System.Drawing.Size(230, 20);
		this.chk_InstallBankStatementPr.TabIndex = 56;
		this.chk_UsePurchaseAccountPostingSystem.EditValue = "N";
		this.chk_UsePurchaseAccountPostingSystem.Enabled = false;
		this.chk_UsePurchaseAccountPostingSystem.FieldName = null;
		this.chk_UsePurchaseAccountPostingSystem.IsSearchControl = false;
		this.chk_UsePurchaseAccountPostingSystem.Location = new System.Drawing.Point(18, 216);
		this.chk_UsePurchaseAccountPostingSystem.Name = "chk_UsePurchaseAccountPostingSystem";
		this.chk_UsePurchaseAccountPostingSystem.Properties.Caption = "Use Purchase Account Posting System";
		this.chk_UsePurchaseAccountPostingSystem.Properties.ValueChecked = "Y";
		this.chk_UsePurchaseAccountPostingSystem.Properties.ValueUnchecked = "N";
		this.chk_UsePurchaseAccountPostingSystem.Size = new System.Drawing.Size(230, 20);
		this.chk_UsePurchaseAccountPostingSystem.TabIndex = 56;
		this.chk_UsePurchaseAccountPostingSystem.CheckedChanged += new System.EventHandler(this.chk_UsePurchaseAccountPostingSystem_CheckedChanged);
		this.chk_AllowSelection.EditValue = "N";
		this.chk_AllowSelection.FieldName = null;
		this.chk_AllowSelection.IsSearchControl = false;
		this.chk_AllowSelection.Location = new System.Drawing.Point(208, 487);
		this.chk_AllowSelection.Name = "chk_AllowSelection";
		this.chk_AllowSelection.Properties.Caption = "Allow Selection of Any Account Type for Revenue Accounts";
		this.chk_AllowSelection.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_AllowSelection.Properties.ValueChecked = "Y";
		this.chk_AllowSelection.Properties.ValueUnchecked = "N";
		this.chk_AllowSelection.Size = new System.Drawing.Size(313, 20);
		this.chk_AllowSelection.TabIndex = 56;
		this.chk_AllowSelection.CheckedChanged += new System.EventHandler(this.chk_AllowSelection_CheckedChanged);
		this.chk_MaskCreditCardNumber.EditValue = "Y";
		this.chk_MaskCreditCardNumber.Enabled = false;
		this.chk_MaskCreditCardNumber.FieldName = null;
		this.chk_MaskCreditCardNumber.IsSearchControl = false;
		this.chk_MaskCreditCardNumber.Location = new System.Drawing.Point(18, 487);
		this.chk_MaskCreditCardNumber.Name = "chk_MaskCreditCardNumber";
		this.chk_MaskCreditCardNumber.Properties.Caption = "Mask Credit Card Number";
		this.chk_MaskCreditCardNumber.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_MaskCreditCardNumber.Properties.ValueChecked = "Y";
		this.chk_MaskCreditCardNumber.Properties.ValueUnchecked = "N";
		this.chk_MaskCreditCardNumber.Size = new System.Drawing.Size(162, 20);
		this.chk_MaskCreditCardNumber.TabIndex = 56;
		this.chk_MaskCreditCardNumber.CheckedChanged += new System.EventHandler(this.chk_MaskCreditCardNumber_CheckedChanged);
		this.chk_EnableAdvanced.EditValue = "N";
		this.chk_EnableAdvanced.FieldName = null;
		this.chk_EnableAdvanced.IsSearchControl = false;
		this.chk_EnableAdvanced.Location = new System.Drawing.Point(208, 466);
		this.chk_EnableAdvanced.Name = "chk_EnableAdvanced";
		this.chk_EnableAdvanced.Properties.Caption = "Enable Advanced G/L Account Determination";
		this.chk_EnableAdvanced.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_EnableAdvanced.Properties.ValueChecked = "Y";
		this.chk_EnableAdvanced.Properties.ValueUnchecked = "N";
		this.chk_EnableAdvanced.Size = new System.Drawing.Size(282, 20);
		this.chk_EnableAdvanced.TabIndex = 56;
		this.chk_EnableAdvanced.CheckedChanged += new System.EventHandler(this.chk_EnableAdvanced_CheckedChanged);
		this.chk_EnableMultiBranch.EditValue = "N";
		this.chk_EnableMultiBranch.FieldName = null;
		this.chk_EnableMultiBranch.IsSearchControl = false;
		this.chk_EnableMultiBranch.Location = new System.Drawing.Point(18, 466);
		this.chk_EnableMultiBranch.Name = "chk_EnableMultiBranch";
		this.chk_EnableMultiBranch.Properties.Caption = "Enable Multiple Branches";
		this.chk_EnableMultiBranch.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_EnableMultiBranch.Properties.ValueChecked = "Y";
		this.chk_EnableMultiBranch.Properties.ValueUnchecked = "N";
		this.chk_EnableMultiBranch.Size = new System.Drawing.Size(162, 20);
		this.chk_EnableMultiBranch.TabIndex = 56;
		this.chk_EnableMultiBranch.CheckedChanged += new System.EventHandler(this.chk_EnableMultiBranch_CheckedChanged);
		this.chk_EnableFixedAssets.EditValue = "Y";
		this.chk_EnableFixedAssets.Enabled = false;
		this.chk_EnableFixedAssets.FieldName = null;
		this.chk_EnableFixedAssets.IsSearchControl = false;
		this.chk_EnableFixedAssets.Location = new System.Drawing.Point(18, 418);
		this.chk_EnableFixedAssets.Name = "chk_EnableFixedAssets";
		this.chk_EnableFixedAssets.Properties.Caption = "Enable Fixed Assets";
		this.chk_EnableFixedAssets.Properties.ValueChecked = "Y";
		this.chk_EnableFixedAssets.Properties.ValueUnchecked = "N";
		this.chk_EnableFixedAssets.Size = new System.Drawing.Size(230, 20);
		this.chk_EnableFixedAssets.TabIndex = 56;
		this.chk_EnableFixedAssets.CheckedChanged += new System.EventHandler(this.chk_EnableFixedAssets_CheckedChanged);
		this.chk_AllowStockRelease.EditValue = "Y";
		this.chk_AllowStockRelease.FieldName = null;
		this.chk_AllowStockRelease.IsSearchControl = false;
		this.chk_AllowStockRelease.Location = new System.Drawing.Point(18, 239);
		this.chk_AllowStockRelease.Name = "chk_AllowStockRelease";
		this.chk_AllowStockRelease.Properties.Caption = "Allow Stock Release Without Item Cost";
		this.chk_AllowStockRelease.Properties.ValueChecked = "Y";
		this.chk_AllowStockRelease.Properties.ValueUnchecked = "N";
		this.chk_AllowStockRelease.Size = new System.Drawing.Size(230, 20);
		this.chk_AllowStockRelease.TabIndex = 56;
		this.chk_AllowStockRelease.CheckedChanged += new System.EventHandler(this.chk_AllowStockRelease_CheckedChanged);
		this.chk_ManageItemCost.EditValue = "Y";
		this.chk_ManageItemCost.Enabled = false;
		this.chk_ManageItemCost.FieldName = null;
		this.chk_ManageItemCost.IsSearchControl = false;
		this.chk_ManageItemCost.Location = new System.Drawing.Point(18, 193);
		this.chk_ManageItemCost.Name = "chk_ManageItemCost";
		this.chk_ManageItemCost.Properties.Caption = "Manage Item Cost per Warehouse";
		this.chk_ManageItemCost.Properties.ValueChecked = "Y";
		this.chk_ManageItemCost.Properties.ValueUnchecked = "N";
		this.chk_ManageItemCost.Size = new System.Drawing.Size(230, 20);
		this.chk_ManageItemCost.TabIndex = 56;
		this.chk_ManageItemCost.CheckedChanged += new System.EventHandler(this.chk_ManageItemCost_CheckedChanged);
		this.chk_UsePerpetalInventory.EditValue = "Y";
		this.chk_UsePerpetalInventory.Enabled = false;
		this.chk_UsePerpetalInventory.FieldName = null;
		this.chk_UsePerpetalInventory.IsSearchControl = false;
		this.chk_UsePerpetalInventory.Location = new System.Drawing.Point(337, 109);
		this.chk_UsePerpetalInventory.Name = "chk_UsePerpetalInventory";
		this.chk_UsePerpetalInventory.Properties.Caption = "Use Perpetual Inventory";
		this.chk_UsePerpetalInventory.Properties.ValueChecked = "Y";
		this.chk_UsePerpetalInventory.Properties.ValueUnchecked = "N";
		this.chk_UsePerpetalInventory.Size = new System.Drawing.Size(168, 20);
		this.chk_UsePerpetalInventory.TabIndex = 56;
		this.chk_UsePerpetalInventory.CheckedChanged += new System.EventHandler(this.chk_UsePerpetalInventory_CheckedChanged);
		this.chk_MultiLangauge.EditValue = "Y";
		this.chk_MultiLangauge.FieldName = null;
		this.chk_MultiLangauge.IsSearchControl = false;
		this.chk_MultiLangauge.Location = new System.Drawing.Point(337, 86);
		this.chk_MultiLangauge.Name = "chk_MultiLangauge";
		this.chk_MultiLangauge.Properties.Caption = "Multi-Language Support";
		this.chk_MultiLangauge.Properties.ValueChecked = "Y";
		this.chk_MultiLangauge.Properties.ValueUnchecked = "N";
		this.chk_MultiLangauge.Size = new System.Drawing.Size(168, 20);
		this.chk_MultiLangauge.TabIndex = 56;
		this.chk_MultiLangauge.CheckedChanged += new System.EventHandler(this.chk_MultiLangauge_CheckedChanged);
		this.chk_PermitMore.EditValue = "N";
		this.chk_PermitMore.Enabled = false;
		this.chk_PermitMore.FieldName = null;
		this.chk_PermitMore.IsSearchControl = false;
		this.chk_PermitMore.Location = new System.Drawing.Point(18, 145);
		this.chk_PermitMore.Name = "chk_PermitMore";
		this.chk_PermitMore.Properties.Caption = "Permit More than Once Document Type per Series";
		this.chk_PermitMore.Properties.ValueChecked = "Y";
		this.chk_PermitMore.Properties.ValueUnchecked = "N";
		this.chk_PermitMore.Size = new System.Drawing.Size(292, 20);
		this.chk_PermitMore.TabIndex = 56;
		this.chk_PermitMore.CheckedChanged += new System.EventHandler(this.chk_PermitMore_CheckedChanged);
		this.chk_AllowNegativeAmounts.EditValue = "Y";
		this.chk_AllowNegativeAmounts.FieldName = null;
		this.chk_AllowNegativeAmounts.IsSearchControl = false;
		this.chk_AllowNegativeAmounts.Location = new System.Drawing.Point(18, 124);
		this.chk_AllowNegativeAmounts.Name = "chk_AllowNegativeAmounts";
		this.chk_AllowNegativeAmounts.Properties.Caption = "Allow Negative Amounts for Reversal Transaction Posting";
		this.chk_AllowNegativeAmounts.Properties.ValueChecked = "Y";
		this.chk_AllowNegativeAmounts.Properties.ValueUnchecked = "N";
		this.chk_AllowNegativeAmounts.Size = new System.Drawing.Size(311, 20);
		this.chk_AllowNegativeAmounts.TabIndex = 56;
		this.chk_AllowNegativeAmounts.CheckedChanged += new System.EventHandler(this.chk_AllowNegativeAmounts_CheckedChanged);
		this.chk_UseSegmentationAccounts.EditValue = "N";
		this.chk_UseSegmentationAccounts.Enabled = false;
		this.chk_UseSegmentationAccounts.FieldName = null;
		this.chk_UseSegmentationAccounts.IsSearchControl = false;
		this.chk_UseSegmentationAccounts.Location = new System.Drawing.Point(18, 105);
		this.chk_UseSegmentationAccounts.Name = "chk_UseSegmentationAccounts";
		this.chk_UseSegmentationAccounts.Properties.Caption = "Use Segmentation Accouts";
		this.chk_UseSegmentationAccounts.Properties.ValueChecked = "Y";
		this.chk_UseSegmentationAccounts.Properties.ValueUnchecked = "N";
		this.chk_UseSegmentationAccounts.Size = new System.Drawing.Size(230, 20);
		this.chk_UseSegmentationAccounts.TabIndex = 56;
		this.chk_UseSegmentationAccounts.CheckedChanged += new System.EventHandler(this.chk_UseSegmentationAccounts_CheckedChanged);
		this.chk_DispCreditBalance.EditValue = "Y";
		this.chk_DispCreditBalance.Enabled = false;
		this.chk_DispCreditBalance.FieldName = null;
		this.chk_DispCreditBalance.IsSearchControl = false;
		this.chk_DispCreditBalance.Location = new System.Drawing.Point(18, 84);
		this.chk_DispCreditBalance.Name = "chk_DispCreditBalance";
		this.chk_DispCreditBalance.Properties.Caption = "Display Credit Balance with Negative Sign";
		this.chk_DispCreditBalance.Properties.ValueChecked = "Y";
		this.chk_DispCreditBalance.Properties.ValueUnchecked = "N";
		this.chk_DispCreditBalance.Size = new System.Drawing.Size(230, 20);
		this.chk_DispCreditBalance.TabIndex = 56;
		this.chk_DispCreditBalance.CheckedChanged += new System.EventHandler(this.chk_DispCreditBalance_CheckedChanged);
		this.lb_DefaultAccountCurrency.FieldName = null;
		this.lb_DefaultAccountCurrency.HideWhenEmpty = false;
		this.lb_DefaultAccountCurrency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DefaultAccountCurrency.Location = new System.Drawing.Point(18, 63);
		this.lb_DefaultAccountCurrency.Name = "lb_DefaultAccountCurrency";
		this.lb_DefaultAccountCurrency.ShowUnderLine = true;
		this.lb_DefaultAccountCurrency.Size = new System.Drawing.Size(184, 20);
		this.lb_DefaultAccountCurrency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DefaultAccountCurrency.TabIndex = 46;
		this.lb_DefaultAccountCurrency.Utext = "Default Account Currency";
		this.lb_SystemCurrency.FieldName = null;
		this.lb_SystemCurrency.HideWhenEmpty = false;
		this.lb_SystemCurrency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SystemCurrency.Location = new System.Drawing.Point(18, 44);
		this.lb_SystemCurrency.Name = "lb_SystemCurrency";
		this.lb_SystemCurrency.ShowUnderLine = true;
		this.lb_SystemCurrency.Size = new System.Drawing.Size(184, 20);
		this.lb_SystemCurrency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SystemCurrency.TabIndex = 44;
		this.lb_SystemCurrency.Utext = "System Curency";
		this.lb_LocalCurrency.FieldName = null;
		this.lb_LocalCurrency.HideWhenEmpty = false;
		this.lb_LocalCurrency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_LocalCurrency.Location = new System.Drawing.Point(18, 25);
		this.lb_LocalCurrency.Name = "lb_LocalCurrency";
		this.lb_LocalCurrency.ShowUnderLine = true;
		this.lb_LocalCurrency.Size = new System.Drawing.Size(184, 20);
		this.lb_LocalCurrency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_LocalCurrency.TabIndex = 42;
		this.lb_LocalCurrency.Utext = "Local Currency";
		this.txt_ChartOfAccountsTemplate.FieldName = null;
		this.txt_ChartOfAccountsTemplate.IsSearchControl = false;
		this.txt_ChartOfAccountsTemplate.Location = new System.Drawing.Point(198, 5);
		this.txt_ChartOfAccountsTemplate.Name = "txt_ChartOfAccountsTemplate";
		this.txt_ChartOfAccountsTemplate.Properties.AutoHeight = false;
		this.txt_ChartOfAccountsTemplate.Properties.ReadOnly = true;
		this.txt_ChartOfAccountsTemplate.Size = new System.Drawing.Size(307, 18);
		this.txt_ChartOfAccountsTemplate.TabIndex = 39;
		this.lb_ChartOfAccountsTemplate.FieldName = null;
		this.lb_ChartOfAccountsTemplate.HideWhenEmpty = false;
		this.lb_ChartOfAccountsTemplate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ChartOfAccountsTemplate.Location = new System.Drawing.Point(18, 6);
		this.lb_ChartOfAccountsTemplate.Name = "lb_ChartOfAccountsTemplate";
		this.lb_ChartOfAccountsTemplate.ShowUnderLine = true;
		this.lb_ChartOfAccountsTemplate.Size = new System.Drawing.Size(184, 20);
		this.lb_ChartOfAccountsTemplate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ChartOfAccountsTemplate.TabIndex = 40;
		this.lb_ChartOfAccountsTemplate.Utext = "Chart of Accounts Template";
		this.rdG_MngSerialNBatchCostBy.FieldName = null;
		this.rdG_MngSerialNBatchCostBy.IsSearchControl = false;
		this.rdG_MngSerialNBatchCostBy.Location = new System.Drawing.Point(337, 210);
		this.rdG_MngSerialNBatchCostBy.Name = "rdG_MngSerialNBatchCostBy";
		this.rdG_MngSerialNBatchCostBy.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdG_MngSerialNBatchCostBy.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Items Group Valuation Method"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Serial/Batch Valuation Method")
		});
		this.rdG_MngSerialNBatchCostBy.Properties.SelectedIndexChanged += new System.EventHandler(this.rdG_MngSerialNBatchCostBy_Properties_SelectedIndexChanged);
		this.rdG_MngSerialNBatchCostBy.Size = new System.Drawing.Size(180, 48);
		this.rdG_MngSerialNBatchCostBy.TabIndex = 77;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.ClientSize = new System.Drawing.Size(554, 571);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.FormId = "oCompanyDetail";
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.Name = "frm_CompanyDetails";
		this.Text = "Company Details";
		base.Load += new System.EventHandler(this.frm_CompanyDetails_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tp_Generel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.tp_LocalLang.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lookUE_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_State.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AliasName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveManager.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InternetAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_County.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ZipCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_City.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BuildingFloorRoom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Block.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetPObox.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyName.Properties).EndInit();
		this.tp_FrgnLang.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lookUE_Country2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_State2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_GLN2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone22.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Telephone12.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AliasName2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveManager2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InternetAddress2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_County2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ZipCode2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_City2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BuildingFloorRoom2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Block2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetNo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StreetPOBox2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyName2.Properties).EndInit();
		this.tp_AccountingData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_CompanyTaxRate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TaxRDetermination.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Holiday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExtendedTaxReporting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ApplyExchangeRateOnDeferredTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseDeferredTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxOfficial2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxDeductionNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExemptionNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FreeZoneNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FederalTaxID1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaxOffice.Properties).EndInit();
		this.tp_BasicInit.ResumeLayout(false);
		this.tp_BasicInit.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.look_SystemCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_LocalCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DefaultAccountCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmGrpValuationMthd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CalcDepreciationBy.Properties).EndInit();
		this.groupBox1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaulAccountNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaultBank.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_DefaultBankCountry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DefaultBankBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OrderingParty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InstallBankStatementPr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UsePurchaseAccountPostingSystem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowSelection.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MaskCreditCardNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableAdvanced.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableMultiBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableFixedAssets.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowStockRelease.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ManageItemCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UsePerpetalInventory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MultiLangauge.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PermitMore.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowNegativeAmounts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseSegmentationAccounts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DispCreditBalance.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChartOfAccountsTemplate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdG_MngSerialNBatchCostBy.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
