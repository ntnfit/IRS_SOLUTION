using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Administrator.Business_Partners;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_UserDefault : XtraFormEx
{
	private readonly string _privilege = "";

	private DataTable _dtOudg;

	private DataTable _dtUdg1;

	private DataTable _dtUdg2;

	private DataTable _dtOrinUdg1;

	private int _salesEmployeeCode = -1;

	private bool _edit;

	private string _printRcpt = "N";

	public static Dictionary<string, string> _lstPrintReceipt = new Dictionary<string, string>
	{
		{ "N", "No" },
		{ "A", "Only When Adding" },
		{ "Y", "Always" }
	};

	public static Dictionary<int, string> _lstColour = new Dictionary<int, string>
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
		{ 9, "Brown " }
	};

	public static Dictionary<int, string> _lstLanguage = new Dictionary<int, string>
	{
		{ 32, "Arabic" },
		{ 26, "Czech(Czech)" },
		{ 11, "Danish(Denmark)" },
		{ 16, "Dutch(Netherlands" },
		{ 8, "English(United Kingdom)" },
		{ 3, "English(United States" },
		{ 17, "Finnish(Finland)" },
		{ 22, "French" },
		{ 9, "German" },
		{ 18, "Greek(Greece)" },
		{ 1, "Hebrew(Israel)" },
		{ 14, "Hungarian(Hungary)" },
		{ 13, "Italian(Italy)" },
		{ 30, "Japanese(Japan)" },
		{ 28, "Korean(Korea)" },
		{ 12, "Norwegian(Norway)" },
		{ 5, "Polish(Poland)" },
		{ 29, "Portuguese(Brazil)" },
		{ 19, "Portuguese" },
		{ 24, "Russian(Russia)" },
		{ 15, "Simplified(P.R.Chin)" },
		{ 27, "Slovak(Slovakia" },
		{ 25, "Spanish(Latin America)" },
		{ 23, "Spanish(Spain)" },
		{ 20, "Swedish(Sweden)" },
		{ 35, "Traditional Chinese" },
		{ 31, "Turkish" }
	};

	public static Dictionary<int, string> _lstFontSize = new Dictionary<int, string>
	{
		{ 0, "0" },
		{ 9, "9" },
		{ 10, "10" },
		{ 11, "11" },
		{ 12, "12" },
		{ 14, "14" },
		{ 16, "16" },
		{ 18, "18" },
		{ 20, "20" },
		{ 22, "22" },
		{ 24, "24" },
		{ 26, "26" },
		{ 28, "28" },
		{ 36, "36" }
	};

	private bool flag_next = true;

	private bool flag_prev = true;

	private IContainer components;

	private TextEditEx txt_Code;

	private TextEditEx txt_Name;

	private XtraTabControlEx Tab_Parent;

	private XtraTabPageEx tabPage_General;

	private XtraTabControlEx tabControl_2;

	private XtraTabPageEx tabPage_LocalLanguage;

	private XtraTabPageEx tabPage_ForeignLanguage;

	private XtraTabPageEx tabPage_Defaults;

	private XtraTabPageEx tabPage_Print;

	private XtraTabPageEx tabPage_CreditCards;

	private GridControlEx grd_CreditCards;

	private GridViewEx gv_CreditCards;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private XtraTabPageEx tabPage_Path;

	private LabelLine lb_AttachmentsFolder;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private SeparatorComboBox cbDocType;

	private LabelLine lb_Document;

	private CheckEditEx chk_ExportToPDF;

	private CheckEditEx chk_EMailDocument;

	private CheckEditEx chk_PrintDocument;

	private CheckEditEx chk_ExportToMSWord;

	private MemoEditEx memoEdit_Remark;

	private TextEditEx txt_CopiesforManualSeries;

	private TextEditEx txt_Copies;

	private LabelLine lb_CopiesforManualSeries;

	private LabelLine lb_CopiesInclOriginal;

	private CheckEditEx chk_ItemNo;

	private CheckEditEx chk_PrintDiscountData;

	private ButtonEditEx btE_Warehouse;

	private LookUpEdit lookUpEdit_FontSize;

	private LookUpEdit lookUpEdit_Language;

	private LookUpEdit lookUpEdit_Color;

	private LabelLine lb_FontSize;

	private LabelLine lb_Font;

	private LabelLine lb_Language;

	private LabelLine lb_CashOnHand;

	private LabelLine lb_Warehouse;

	private LabelLine lb_Color;

	private TextEditEx txt_Email;

	private TextEditEx txt_Fax;

	private TextEditEx txt_TelePhone;

	private TextEditEx txt_Tel1;

	private TextEditEx txt_PrintingHeader;

	private TextEditEx txt_AdditionalID;

	private LookUpEdit lookUpEdit_Country;

	private LabelLine lb_EMail;

	private LabelLine lb_Fax;

	private LabelLine lb_Telephone2;

	private LabelLine lb_Tel1;

	private LabelLine lb_PrintingHeader;

	private LabelLine lb_AdditionalID;

	private LabelLine lb_Country;

	private MemoEditEx memoEdit_Address1;

	private TextEditEx txt_EMail2;

	private TextEditEx txt_Fax2;

	private TextEditEx txt_Phone22;

	private TextEditEx txtTel12;

	private TextEditEx txt_PrintingHeader2;

	private TextEditEx txt_AdditionalID2;

	private LookUpEdit lookUpEdit_Country2;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private MemoEditEx memoEdit_Address2;

	private CheckEditEx chk_UserWarehouse;

	private CheckEditEx chk_AllowCreating;

	private Label label7;

	private LookUpEdit lookUpEdit_Branch;

	private LabelLine lb_Branch;

	private LabelLine lb_ChecksReceived;

	private CheckEditEx checkEdit2;

	private ButtonEditEx buttonEdit_CardCode;

	private ButtonEditEx buttonEdit_ChecksReceived;

	private ButtonEditEx buttonEdit_CashOnHand;

	private ButtonEditEx txt_WarehouseName;

	private ButtonEditEx txt_CashOnHand;

	private ButtonEditEx txt_ChecksReceived;

	private ButtonEditEx txt_CardName;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private CheckEditEx chk_PrintAmount;

	private LookUpEdit lookUE_PrintRcpt;

	private LabelLine lb_PrintPaymentWithInvoice;

	private CheckEditEx chk_PrintPaymentInvoiceInSuccession;

	private GridColumnEx gridColumn4;

	private ButtonEditEx buttonEdit_SalesEmployee;

	private TextBoxEx txt_Preview;

	private FontEdit lookUpEdit_Font;

	private LabelLine labelLine8;

	private SimpleButtonEx btn_BrowerFolder;

	private TextEditEx btE_AttachmentsFolder;

	private LabelLine labelLine9;

	private LabelLine labelLine10;

	private LabelLine labelLine12;

	private LabelLine labelLine11;

	private LabelLine labelLine13;

	private LabelLine labelLine16;

	private LabelLine labelLine15;

	private LabelLine labelLine14;

	private LabelLine labelLine18;

	private async Task Search()
	{
		_ = 3;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = txt_Code.Text;
			dataRow["Name"] = txt_Name.Text;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			WinCommons._OpenProcessing();
			DataTable dataTable2 = await _hp.PostDataTableAsync("Search", dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				await GetOUDG();
				await GetUDG1();
				await GetUDG2();
				bt_OK.Text = Resources.ButtonOK;
				FormMode = Enums.FormViewModeType.Find;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 4;
		try
		{
			DataSet ds = new DataSet();
			DataTable dtSaveOudg = await GetColumnsTable_OUDG();
			dtSaveOudg.Rows[0]["Code"] = Function.ToString(txt_Code.Text);
			dtSaveOudg.Rows[0]["Name"] = Function.ToString(txt_Name.Text);
			dtSaveOudg.Rows[0]["Address"] = Function.ToString(memoEdit_Address1.Text);
			dtSaveOudg.Rows[0]["Country"] = Function.ToString(lookUpEdit_Country.EditValue);
			dtSaveOudg.Rows[0]["FreeZoneNo"] = Function.ToString(txt_AdditionalID.Text);
			dtSaveOudg.Rows[0]["PrintHeadr"] = Function.ToString(txt_PrintingHeader.Text);
			dtSaveOudg.Rows[0]["Phone1"] = Function.ToString(txt_Tel1.Text);
			dtSaveOudg.Rows[0]["Phone2"] = Function.ToString(txt_TelePhone.Text);
			dtSaveOudg.Rows[0]["Fax"] = Function.ToString(txt_Fax.Text);
			dtSaveOudg.Rows[0]["E_Mail"] = Function.ToString(txt_Email.Text);
			dtSaveOudg.Rows[0]["FrgnAddr"] = Function.ToString(memoEdit_Address2.Text);
			dtSaveOudg.Rows[0]["Country"] = Function.ToString(lookUpEdit_Country2.EditValue);
			dtSaveOudg.Rows[0]["FreeZoneNo"] = Function.ToString(txt_AdditionalID2.Text);
			dtSaveOudg.Rows[0]["FrnPrntHdr"] = Function.ToString(txt_PrintingHeader2.Text);
			dtSaveOudg.Rows[0]["FrgnPhone1"] = Function.ToString(txtTel12.Text);
			dtSaveOudg.Rows[0]["FrgnPhone2"] = Function.ToString(txt_Phone22.Text);
			dtSaveOudg.Rows[0]["FrgnFax"] = Function.ToString(txt_Fax2.Text);
			dtSaveOudg.Rows[0]["E_Mail"] = Function.ToString(txt_EMail2.Text);
			dtSaveOudg.Rows[0]["Color"] = Function.ToString(lookUpEdit_Color.EditValue);
			dtSaveOudg.Rows[0]["Language"] = Function.ToString(lookUpEdit_Language.EditValue);
			dtSaveOudg.Rows[0]["Font"] = Function.ToString(lookUpEdit_Font.EditValue);
			dtSaveOudg.Rows[0]["FontSize"] = Function.ToString(lookUpEdit_FontSize.EditValue);
			dtSaveOudg.Rows[0]["SalePerson"] = _salesEmployeeCode;
			dtSaveOudg.Rows[0]["Warehouse"] = Function.ToString(btE_Warehouse.EditValue);
			dtSaveOudg.Rows[0]["CashAcct"] = Function.ToString(buttonEdit_CashOnHand.Text);
			dtSaveOudg.Rows[0]["CheckAcct"] = Function.ToString(buttonEdit_ChecksReceived.Text);
			dtSaveOudg.Rows[0]["ICTCard"] = Function.ToString(buttonEdit_CardCode.Text);
			dtSaveOudg.Rows[0]["BPLId"] = lookUpEdit_Branch.EditValue;
			dtSaveOudg.Rows[0]["AdrsFromWh"] = ((chk_UserWarehouse.EditValue == null) ? "N" : chk_UserWarehouse.EditValue);
			dtSaveOudg.Rows[0]["AssetInDoc"] = ((chk_AllowCreating.EditValue == null) ? "N" : chk_AllowCreating.EditValue);
			dtSaveOudg.Rows[0]["PrintRcpt"] = _printRcpt;
			dtSaveOudg.Rows[0]["ShortRcpt"] = ((chk_PrintPaymentInvoiceInSuccession.EditValue == null) ? "N" : chk_PrintPaymentInvoiceInSuccession.EditValue);
			dtSaveOudg.AcceptChanges();
			DataTable dataTable = await GetColumnsTable_UDG1();
			DataTable dtSaveLine_UDG1 = new DataTable();
			if (dataTable != null)
			{
				dtSaveLine_UDG1 = dataTable.Clone();
				for (int i = 0; i < cbDocType.Items.Count; i++)
				{
					DataRow dataRow = dtSaveLine_UDG1.NewRow();
					dataRow["Code"] = txt_Code.Text;
					int objTypeID = GetObjTypeID(cbDocType.Items[i].ToString());
					if (objTypeID >= 0)
					{
						dataRow["ObjType"] = objTypeID;
						dataRow["Copies"] = txt_Copies.Text;
						dataRow["PrintOnAdd"] = ((chk_PrintDocument.EditValue == null) ? "N" : chk_PrintDocument.EditValue);
						dataRow["ExprtOnAdd"] = ((chk_ExportToMSWord.EditValue == null) ? "N" : chk_ExportToMSWord.EditValue);
						dataRow["EmailOnAdd"] = ((chk_EMailDocument.EditValue == null) ? "N" : chk_EMailDocument.EditValue);
						dataRow["PDFOnAdd"] = ((chk_ExportToPDF.EditValue == null) ? "N" : chk_ExportToPDF.EditValue);
						dataRow["HandCopies"] = Function.ToString(txt_CopiesforManualSeries.Text);
						dataRow["Remark"] = Function.ToString(memoEdit_Remark.Text);
						dataRow["PrnDscnt"] = ((chk_PrintDiscountData.EditValue == null) ? "N" : chk_PrintDiscountData.EditValue);
						dataRow["VndrNum"] = ((chk_ItemNo.EditValue == null) ? "N" : chk_ItemNo.EditValue);
						dataRow["PrintSums"] = ((chk_PrintAmount.EditValue == null) ? "N" : chk_PrintAmount.EditValue);
						dtSaveLine_UDG1.Rows.Add(dataRow);
					}
				}
				dtSaveLine_UDG1.AcceptChanges();
			}
			await GetColumnsTable_UDG2();
			DataTable dataTable2 = grd_CreditCards.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
				{
					if (dataTable2.Rows[num][1] == DBNull.Value)
					{
						dataTable2.Rows[num].Delete();
					}
				}
				dataTable2.AcceptChanges();
			}
			ds.Tables.Add(dtSaveOudg);
			ds.AcceptChanges();
			ds.Tables.Add(dtSaveLine_UDG1);
			ds.AcceptChanges();
			ds.Tables.Add(dataTable2);
			ds.AcceptChanges();
			using HttpClientHelper<DataSet> _hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UserDefault");
			WinCommons._OpenProcessing();
			string text = await _hp.PostAsJsonAsync("Save", ds);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await RefreshData();
				ResetControl();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetObjTypeID(string p)
	{
		int result = -1;
		try
		{
			result = p switch
			{
				"" => -1, 
				"Sales Quotation" => 23, 
				"Sales Order" => 17, 
				"Delivery" => 15, 
				"Returns" => 16, 
				"A/R Down Payment" => 203, 
				"A/R Invoice" => 13, 
				"A/R Credit Memo" => 14, 
				"Purchase Request" => 1470000113, 
				"Purchase Quotation" => 540000006, 
				"Purchase Order" => 22, 
				"Goods Receipt PO" => 20, 
				"Goods Return" => 21, 
				"A/P Down Payment" => 204, 
				"A/P Invoice" => 18, 
				"A/P Credit Memo" => 19, 
				"Landed Costs" => 69, 
				"Incoming Payment" => 24, 
				"Deposit" => 25, 
				"Outgoing Payment" => 46, 
				"Postdated Deposit" => 76, 
				"Check for Payment" => 57, 
				"Journal Entry" => 30, 
				"Journal Voucher" => 28, 
				"Goods Receipt" => 59, 
				"Goods Issue" => 60, 
				"Inventory Transfer Request" => 1250000001, 
				"Inventory Transfer" => 67, 
				"Production Order" => 202, 
				"Inventory Revaluation" => 162, 
				"Pick List" => 156, 
				"Inventory Counting" => 1470000065, 
				"Inventory Posting" => 10000071, 
				"Inventory Opening Balances" => 310000001, 
				_ => -1, 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async Task GetOUDG()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			string code = txt_Code.Text;
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			WinCommons._OpenProcessing();
			_dtOudg = await _hp.GetAsync("GetOUDG", new { code });
			if (_dtOudg != null && _dtOudg.Rows.Count > 0)
			{
				txt_Code.Text = Function.ToString(_dtOudg.Rows[0]["Code"]);
				txt_Name.Text = Function.ToString(_dtOudg.Rows[0]["Name"]);
				memoEdit_Address1.Text = Function.ToString(_dtOudg.Rows[0]["Address"]);
				lookUpEdit_Country.EditValue = Function.ToString(_dtOudg.Rows[0]["Country"]);
				txt_AdditionalID.Text = Function.ToString(_dtOudg.Rows[0]["FreeZoneNo"]);
				txt_PrintingHeader.Text = Function.ToString(_dtOudg.Rows[0]["PrintHeadr"]);
				txt_Tel1.Text = Function.ToString(_dtOudg.Rows[0]["Phone1"]);
				txt_TelePhone.Text = Function.ToString(_dtOudg.Rows[0]["Phone2"]);
				txt_Fax.Text = Function.ToString(_dtOudg.Rows[0]["Fax"]);
				txt_Email.Text = Function.ToString(_dtOudg.Rows[0]["E_Mail"]);
				memoEdit_Address2.Text = Function.ToString(_dtOudg.Rows[0]["FrgnAddr"]);
				lookUpEdit_Country2.EditValue = Function.ToString(_dtOudg.Rows[0]["Country"]);
				txt_AdditionalID2.Text = Function.ToString(_dtOudg.Rows[0]["FreeZoneNo"]);
				txt_PrintingHeader2.Text = Function.ToString(_dtOudg.Rows[0]["FrnPrntHdr"]);
				txtTel12.Text = Function.ToString(_dtOudg.Rows[0]["FrgnPhone1"]);
				txt_Phone22.Text = Function.ToString(_dtOudg.Rows[0]["FrgnPhone2"]);
				txt_Fax2.Text = Function.ToString(_dtOudg.Rows[0]["FrgnFax"]);
				txt_EMail2.Text = Function.ToString(_dtOudg.Rows[0]["E_Mail"]);
				lookUpEdit_Color.EditValue = Function.ToString(_dtOudg.Rows[0]["Color"]);
				lookUpEdit_Language.EditValue = Function.ToString(_dtOudg.Rows[0]["Language"]);
				lookUpEdit_Font.EditValue = Function.ToString(_dtOudg.Rows[0]["Font"]);
				lookUpEdit_FontSize.EditValue = Function.ToString(_dtOudg.Rows[0]["FontSize"]);
				buttonEdit_SalesEmployee.EditValue = Function.ToString(_dtOudg.Rows[0]["SalePerson"]);
				btE_Warehouse.EditValue = Function.ToString(_dtOudg.Rows[0]["Warehouse"]);
				txt_WarehouseName.Text = Function.ToString(_dtOudg.Rows[0]["WarehouseName"]);
				buttonEdit_CashOnHand.Text = Function.ToString(_dtOudg.Rows[0]["CashAcct"]);
				txt_CashOnHand.Text = Function.ToString(_dtOudg.Rows[0]["AcctNameY"]);
				buttonEdit_ChecksReceived.Text = Function.ToString(_dtOudg.Rows[0]["CheckAcct"]);
				txt_ChecksReceived.Text = Function.ToString(_dtOudg.Rows[0]["AcctNameN"]);
				buttonEdit_CardCode.Text = Function.ToString(_dtOudg.Rows[0]["ICTCard"]);
				txt_CardName.Text = Function.ToString(_dtOudg.Rows[0]["CardName"]);
				lookUpEdit_Branch.EditValue = Function.ToString(_dtOudg.Rows[0]["BPLId"]);
				chk_UserWarehouse.EditValue = Function.ToString(_dtOudg.Rows[0]["AdrsFromWh"]);
				chk_AllowCreating.EditValue = Function.ToString(_dtOudg.Rows[0]["AssetInDoc"]);
				lookUE_PrintRcpt.EditValue = Function.ToString(_dtOudg.Rows[0]["PrintRcpt"]);
				chk_PrintPaymentInvoiceInSuccession.EditValue = Function.ToString(_dtOudg.Rows[0]["ShortRcpt"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetUDG1()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			string code = txt_Code.Text;
			string type = Function.ToString(GetObjTypeID(cbDocType.SelectedItem.ToString()));
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			_dtUdg1 = await hp.GetAsync("GetUDG1", new { code, type });
			if (_dtUdg1 != null && _dtUdg1.Rows.Count > 0)
			{
				int dType = Function.ParseInt(_dtUdg1.Rows[0]["ObjType"]);
				cbDocType.SelectedItem = GetObjTypeName(dType);
				chk_ExportToMSWord.EditValue = _dtUdg1.Rows[0]["ExprtOnAdd"];
				chk_PrintDocument.EditValue = _dtUdg1.Rows[0]["PrintOnAdd"];
				chk_EMailDocument.EditValue = _dtUdg1.Rows[0]["EmailOnAdd"];
				chk_ExportToPDF.EditValue = _dtUdg1.Rows[0]["PDFOnAdd"];
				txt_Copies.Text = Function.ToString(_dtUdg1.Rows[0]["Copies"]);
				txt_CopiesforManualSeries.Text = Function.ToString(_dtUdg1.Rows[0]["HandCopies"]);
				memoEdit_Remark.Text = Function.ToString(_dtUdg1.Rows[0]["Remark"]);
				chk_PrintDiscountData.EditValue = Function.ToString(_dtUdg1.Rows[0]["PrnDscnt"]);
				chk_ItemNo.EditValue = Function.ToString(_dtUdg1.Rows[0]["VndrNum"]);
				chk_PrintAmount.EditValue = Function.ToString(_dtUdg1.Rows[0]["PrintSums"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetUDG2()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			string code = txt_Code.Text;
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			WinCommons._OpenProcessing();
			_dtUdg2 = await _hp.GetAsync("GetUDG2", new { code });
			grd_CreditCards.DataSource = _dtUdg2;
			grd_CreditCards.RefreshDataSource();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		FormMode = Enums.FormViewModeType.Add;
		LoadValueCheckBoxDefault(Tab_Parent);
		bt_OK.Text = Resources.ButtonAdd;
		await GetUDG2();
	}

	public async Task MODE_FIND()
	{
		bt_OK.Text = Resources.ButtonOK;
		ResetControl();
		if (_privilege == "F")
		{
			bt_OK.Enabled = true;
		}
	}

	public async Task REFRESH()
	{
		bt_OK.Text = Resources.ButtonAdd;
		await RefreshData();
		ResetControl();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			flag_next = true;
			flag_prev = false;
			await GetRecord("OUDG", "", "F");
			ChangeFocusControl(Controls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			if (flag_next)
			{
				string text = Function.ToString(txt_Code.Text);
				if (!string.IsNullOrEmpty(text))
				{
					await GetRecord("OUDG", text, "N");
				}
				else
				{
					await GetRecord("OUDG", text, "F");
				}
				flag_prev = true;
			}
			ChangeFocusControl(Controls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			flag_next = false;
			flag_prev = true;
			await GetRecord("OUDG", "", "L");
			ChangeFocusControl(Controls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.Find;
			if (flag_prev)
			{
				string text = Function.ToString(txt_Code.Text);
				if (!string.IsNullOrEmpty(text))
				{
					await GetRecord("OUDG", text, "P");
				}
				else
				{
					await GetRecord("OUDG", text, "L");
				}
				flag_next = true;
			}
			ChangeFocusControl(Controls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetRecord(string tbl, string key, string TypeRecord)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			DataTable dataTable = await hp.GetAsync("GetRecord", new { tbl, key, TypeRecord });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				key = dataTable.Rows[0]["Code"].ToString();
				txt_Code.Text = key;
				txt_Name.Text = string.Empty;
				await Search();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string getControlType(Control ctl)
	{
		if (ctl is TextEdit)
		{
			return "TextEdit";
		}
		if (ctl is ComboBoxEdit)
		{
			return "ComboBoxEdit";
		}
		if (ctl is CalcEdit)
		{
			return "CalcEdit";
		}
		if (ctl is DateEdit)
		{
			return "DateEdit";
		}
		if (ctl is CheckEdit)
		{
			return "CheckEdit";
		}
		if (ctl is SimpleButton)
		{
			return "SimpleButton";
		}
		if (ctl is GridControl)
		{
			return "GridControl";
		}
		if (ctl is CheckEdit)
		{
			return "CheckEdit";
		}
		if (ctl is RadioGroup)
		{
			return "RadioGroup";
		}
		if (ctl is ContextMenuStrip)
		{
			return "ContextMenuStrip";
		}
		return "";
	}

	private void ChangeFocusControl(Control.ControlCollection ctrControl)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find)
			{
				return;
			}
			foreach (Control item in ctrControl)
			{
				switch (getControlType(item))
				{
				case "ComboBoxEdit":
					((ComboBoxEdit)item).SelectedIndexChanged += OnContentChanged;
					break;
				case "CheckEdit":
					((CheckEdit)item).CheckedChanged += OnContentChanged;
					break;
				case "RadioGroup":
					((RadioButton)item).CheckedChanged += OnContentChanged;
					break;
				case "TextEdit":
					((TextEdit)item).TextChanged += OnContentChanged;
					break;
				case "ButtonEdit":
					((ButtonEdit)item).TextChanged += OnContentChanged;
					break;
				case "MemoEdit":
					((MemoEdit)item).TextChanged += OnContentChanged;
					break;
				case "DateEdit":
					((DateEdit)item).EditValueChanged += OnContentChanged;
					break;
				case "LookUpEdit":
					((LookUpEdit)item).EditValueChanged += OnContentChanged;
					break;
				}
				if (item.Controls.Count > 0)
				{
					ChangeFocusControl(item.Controls);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				FormMode = Enums.FormViewModeType.Update;
				bt_OK.Text = Resources.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetControl()
	{
		try
		{
			WinFuntions.ResetControl(Controls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCombo()
	{
		_ = 2;
		try
		{
			await GetUserSignByUserCode_tableOUSR();
			await GetBranch();
			GetFontSize();
			GetLanguage();
			GetColor();
			await GetCountry();
			GetListPrintPaymentInvoice();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetListPrintPaymentInvoice()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstPrintReceipt.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			lookUE_PrintRcpt.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetColor()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstColour.Select((KeyValuePair<int, string> item) => new Common.Table_Code
			{
				Code = Function.ToString(item.Key),
				Name = item.Value
			}).ToList();
			lookUpEdit_Color.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetLanguage()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstLanguage.Select((KeyValuePair<int, string> item) => new Common.Table_Code
			{
				Code = Function.ToString(item.Key),
				Name = item.Value
			}).ToList();
			lookUpEdit_Language.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetFontSize()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstFontSize.Select((KeyValuePair<int, string> item) => new Common.Table_Code
			{
				Code = Function.ToString(item.Key),
				Name = item.Value
			}).ToList();
			lookUpEdit_FontSize.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private async Task GetBranch()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await hp.GetAsync("GetBranchList", new
			{
				userCode = GlobalData.UserCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				lookUpEdit_Branch.Properties.DataSource = dataTable;
			}
			else
			{
				dataTable.Columns.Add("BPLId", typeof(string));
				dataTable.Columns.Add("BPLName", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["BPLId"] = -1;
				dataRow["BPLName"] = string.Empty;
				dataTable.Rows.Add(dataRow);
				lookUpEdit_Branch.Properties.DataSource = dataTable;
			}
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["BPLId"] = -1;
			dataRow2["BPLName"] = string.Empty;
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
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Countries");
			DataTable dataTable = await hp.GetAsync("GetOCRY");
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = string.Empty;
				dataRow["Name"] = string.Empty;
				dataTable.Rows.InsertAt(dataRow, 0);
				lookUpEdit_Country.Properties.DataSource = dataTable;
				lookUpEdit_Country2.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string GetObjTypeName(int dType)
	{
		string result = "";
		try
		{
			result = dType switch
			{
				-1 => "", 
				23 => "Sales Quotation", 
				17 => "Sales Order", 
				15 => "Delivery", 
				16 => "Returns", 
				203 => "A/R Down Payment", 
				13 => "A/R Invoice", 
				14 => "A/R Credit Memo", 
				1470000113 => "Purchase Request", 
				540000006 => "Purchase Quotation", 
				22 => "Purchase Order", 
				20 => "Goods Receipt PO", 
				21 => "Goods Return", 
				204 => "A/P Down Payment", 
				18 => "A/P Invoice", 
				19 => "A/P Credit Memo", 
				69 => "Landed Costs", 
				24 => "Incoming Payment", 
				25 => "Deposit", 
				46 => "Outgoing Payment", 
				76 => "Postdated Deposit", 
				57 => "Check for Payment", 
				30 => "Journal Entry", 
				28 => "Journal Voucher", 
				59 => "Goods Receipt", 
				60 => "Goods Issue", 
				1250000001 => "Inventory Transfer Request", 
				67 => "Inventory Transfer", 
				202 => "Production Order", 
				162 => "Inventory Revaluation", 
				156 => "Pick List", 
				1470000065 => "Inventory Counting", 
				10000071 => "Inventory Posting", 
				310000001 => "Inventory Opening Balances", 
				_ => "", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void gv_CreditCards_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btE_Warehouse_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				GetWarehouseCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_WarehouseName_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				GetWarehouseCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetWarehouseCFL()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
			if (dataRow != null)
			{
				txt_WarehouseName.Text = Function.ToString(dataRow["WhsName"]);
				btE_Warehouse.Text = Function.ToString(dataRow["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonEdit_CashOnHand_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				GetAccountCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_CashOnHand_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				GetAccountCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAccountCFL()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, string.Empty);
			frm_ChooseFromList2._AccountType = "Y";
			frm_ChooseFromList2.ShowDialog(this);
			AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
			if (acctCodeImpl != null)
			{
				buttonEdit_CashOnHand.Text = acctCodeImpl.AcctCode;
				txt_CashOnHand.Text = acctCodeImpl.AcctName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonEdit_ChecksReceived_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, string.Empty);
			frm_ChooseFromList2._AccountType = "N";
			frm_ChooseFromList2.ShowDialog(this);
			AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
			if (acctCodeImpl != null)
			{
				buttonEdit_ChecksReceived.Text = acctCodeImpl.AcctCode;
				txt_ChecksReceived.Text = acctCodeImpl.AcctName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_ChecksReceived_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, string.Empty);
			frm_ChooseFromList2._AccountType = "N";
			frm_ChooseFromList2.ShowDialog(this);
			AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
			if (acctCodeImpl != null)
			{
				buttonEdit_ChecksReceived.Text = acctCodeImpl.AcctCode;
				txt_ChecksReceived.Text = acctCodeImpl.AcctName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonEdit4_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GetCardCodeCFL();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetCardCodeCFL()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				DataRow chooseItem = frm_CFL2.ChooseItem;
				buttonEdit_CardCode.Text = Function.ToString(chooseItem["CardCode"]);
				txt_CardName.Text = Function.ToString(chooseItem["CardName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_CardName_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		GetCardCodeCFL();
	}

	private void LoadValueCheckBoxDefault(Control ctr)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (control is CheckEdit)
				{
					((CheckEdit)control).EditValue = "N";
				}
				else
				{
					LoadValueCheckBoxDefault(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetColumnsTable_UDG2()
	{
		try
		{
			string tableName = "UDG2";
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			return await hp.GetAsync("GetTableColumns", new { tableName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetColumnsTable_UDG1()
	{
		try
		{
			string tableName = "UDG1";
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			return await hp.GetAsync("GetTableColumns", new { tableName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetColumnsTable_OUDG()
	{
		try
		{
			string tableName = "OUDG";
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			return await hp.GetAsync("GetTableColumns", new { tableName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void lookUpEdit_Country_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_Countries frm_Countries2 = new frm_Countries();
				frm_Countries2.Show();
				frm_Countries2.FormClosing += async (object s, FormClosingEventArgs e2) =>
				{
					await GetCountry();
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lookUpEdit_Country2_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_Countries frm_Countries2 = new frm_Countries();
				frm_Countries2.Show();
				frm_Countries2.FormClosing += async (object s, FormClosingEventArgs e2) =>
				{
					await GetCountry();
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void labelLine11_Load(object sender, EventArgs e)
	{
	}

	private void labelLine16_Load(object sender, EventArgs e)
	{
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.A | Keys.Control:
			ResetControl();
			bt_OK.Text = Resources.ButtonAdd;
			break;
		case Keys.F | Keys.Control:
			ResetControl();
			bt_OK.Text = Resources.ButtonFind;
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void lookUpEdit_Font_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = lookUpEdit_Font.EditValue.ToString();
			Font font = new Font(text, 9f);
			if (!string.IsNullOrEmpty(text))
			{
				txt_Preview.Font = font;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbDocType_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(cbDocType.SelectedItem))
			{
			case "Sales Quotation":
			case "Sales Order":
			case "Delivery":
			case "Returns":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintAmount.Visible = true;
				lb_CopiesforManualSeries.Visible = true;
				txt_CopiesforManualSeries.Visible = true;
				break;
			case "A/R Down Payment":
				chk_ExportToMSWord.Visible = true;
				lb_PrintPaymentWithInvoice.Visible = true;
				lookUE_PrintRcpt.Visible = true;
				chk_PrintPaymentInvoiceInSuccession.Visible = true;
				chk_PrintAmount.Visible = false;
				lb_CopiesforManualSeries.Visible = true;
				txt_CopiesforManualSeries.Visible = true;
				break;
			case "A/R Credit Memo":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = true;
				chk_ItemNo.Visible = true;
				chk_PrintAmount.Visible = false;
				lb_CopiesforManualSeries.Visible = true;
				txt_CopiesforManualSeries.Visible = true;
				break;
			case "Purchase Request":
			case "Purchase Quotation":
			case "Purchase Oder":
			case "Goods Receipt PO":
			case "Goods Return":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = true;
				chk_ItemNo.Visible = true;
				chk_PrintAmount.Visible = true;
				break;
			case "A/P Invoice":
			case "A/P Credit Memo":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = true;
				chk_ItemNo.Visible = true;
				chk_PrintAmount.Visible = false;
				break;
			case "Landed Costs":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = false;
				break;
			case "Incoming Payment":
			case "Outgoing Payment":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = true;
				txt_CopiesforManualSeries.Visible = true;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = false;
				break;
			case "Deposit":
			case "Postdated Deposit":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = false;
				break;
			case "Check for Payment":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = false;
				chk_ExportToPDF.Visible = false;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = true;
				break;
			case "Journal Entry":
			case "Journal Voucher":
			case "Goods Receipt":
			case "Goods Issue":
			case "Inventory Transfer":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = true;
				chk_ExportToPDF.Visible = true;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = false;
				break;
			case "Inventory Transfer Request":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = true;
				chk_ExportToPDF.Visible = true;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = true;
				break;
			case "Production Order":
			case "Inventory Revaluation":
			case "Pick List":
			case "Inventory Counting":
				chk_ExportToMSWord.Visible = false;
				chk_PrintDocument.Visible = true;
				chk_EMailDocument.Visible = true;
				chk_ExportToPDF.Visible = true;
				lb_CopiesforManualSeries.Visible = false;
				txt_CopiesforManualSeries.Visible = false;
				lb_PrintPaymentWithInvoice.Visible = false;
				lookUE_PrintRcpt.Visible = false;
				chk_PrintPaymentInvoiceInSuccession.Visible = false;
				chk_PrintDiscountData.Visible = false;
				chk_ItemNo.Visible = false;
				chk_PrintAmount.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lookUpEdit_Country2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = lookUpEdit_Country2.EditValue;
			lookUpEdit_Country.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lookUpEdit_Country_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = lookUpEdit_Country.EditValue;
			lookUpEdit_Country2.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_AdditionalID_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = txt_AdditionalID.EditValue;
			txt_AdditionalID2.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_AdditionalID2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = txt_AdditionalID2.EditValue;
			txt_AdditionalID.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Email_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = txt_Email.EditValue;
			txt_EMail2.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_EMail2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = txt_EMail2.EditValue;
			txt_Email.EditValue = editValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lookUE_PrintRcpt_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string printRcpt = Function.ToString(lookUE_PrintRcpt.EditValue);
			_printRcpt = printRcpt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_BrowerFolder_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (string.IsNullOrEmpty(btE_AttachmentsFolder.Text))
			{
				folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					btE_AttachmentsFolder.Text = folderBrowserDialog.SelectedPath;
				}
			}
			else
			{
				folderBrowserDialog.SelectedPath = btE_AttachmentsFolder.Text;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					btE_AttachmentsFolder.Text = folderBrowserDialog.SelectedPath;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			string pr = txt_Code.Text;
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefault");
			string text = await hp.PostParamsAsJsonAsync("Remove", pr);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
				ResetControl();
				await RefreshData();
				bt_OK.Text = Resources.ButtonOK;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	public frm_UserDefault()
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UserDefault"))
			{
				InitializeComponent();
				cbDocType.SeparatorColor = Color.Black;
				cbDocType.SeparatorWidth = 1;
				cbDocType.AutoAdjustItemHeight = true;
				cbDocType.Items.Clear();
				cbDocType.AddString(string.Empty);
				cbDocType.AddString("Sales Quotation");
				cbDocType.AddString("Sales Order");
				cbDocType.AddString("Delivery");
				cbDocType.AddString("Returns");
				cbDocType.AddString("A/R Down Payment");
				cbDocType.AddString("A/R Invoice");
				cbDocType.AddString("A/R Credit Memo");
				cbDocType.SetSeparator(8);
				cbDocType.AddString(string.Empty);
				cbDocType.AddString("Purchase Request");
				cbDocType.AddString("Purchase Quotation");
				cbDocType.AddString("Purchase Order");
				cbDocType.AddString("Goods Receipt PO");
				cbDocType.AddString("Goods Return");
				cbDocType.AddString("A/P Down Payment");
				cbDocType.AddString("A/P Invoice");
				cbDocType.AddString("A/P Credit Memo");
				cbDocType.AddString("Landed Costs");
				cbDocType.SetSeparator(18);
				cbDocType.AddString(string.Empty);
				cbDocType.AddString("Incoming Payment");
				cbDocType.AddString("Deposit");
				cbDocType.AddString("Outgoing Payment");
				cbDocType.AddString("Postdated Deposit");
				cbDocType.AddString("Check for Payment");
				cbDocType.AddString("Journal Entry");
				cbDocType.AddString("Journal Voucher");
				cbDocType.SetSeparator(26);
				cbDocType.AddString(string.Empty);
				cbDocType.AddString("Goods Receipt");
				cbDocType.AddString("Goods Issue");
				cbDocType.AddString("Inventory Transfer Request");
				cbDocType.AddString("Inventory Transfer");
				cbDocType.AddString("Production Order");
				cbDocType.AddString("Inventory Revaluation");
				cbDocType.AddString("Pick List");
				cbDocType.AddString("Inventory Counting");
				cbDocType.AddString("Inventory Posting");
				cbDocType.AddString("Inventory Opening Balances");
				cbDocType.SelectedIndex = 6;
				if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UserDefault" && x.authoriza == "F"))
				{
					WinFuntions.DisableAllControl(Controls);
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_UserDefault_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadCombo();
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonEdit2_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.SaleEmployee, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				SalesPersonsImpl salesPersonsImpl = (SalesPersonsImpl)frm_ChooseFromList2.ChooseItem;
				buttonEdit_SalesEmployee.Text = salesPersonsImpl.SalesEmployeeName;
				_salesEmployeeCode = salesPersonsImpl.SalesEmployeeCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (bt_OK.Text == Resources.ButtonFind)
			{
				await Search();
			}
			else if (bt_OK.Text == Resources.ButtonOK)
			{
				Close();
			}
			else
			{
				await Save();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_UserDefault));
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.Tab_Parent = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPage_General = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tabControl_2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPage_LocalLanguage = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Email = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.txt_TelePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Tel1 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_PrintingHeader = new Apzon.Controls.Editor.TextEditEx();
		this.txt_AdditionalID = new Apzon.Controls.Editor.TextEditEx();
		this.lookUpEdit_Country = new DevExpress.XtraEditors.LookUpEdit();
		this.lb_EMail = new Apzon.Controls.Controls.LabelLine();
		this.lb_Fax = new Apzon.Controls.Controls.LabelLine();
		this.lb_Telephone2 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Tel1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_PrintingHeader = new Apzon.Controls.Controls.LabelLine();
		this.lb_AdditionalID = new Apzon.Controls.Controls.LabelLine();
		this.lb_Country = new Apzon.Controls.Controls.LabelLine();
		this.memoEdit_Address1 = new Apzon.Controls.Editor.MemoEditEx();
		this.tabPage_ForeignLanguage = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EMail2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Fax2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Phone22 = new Apzon.Controls.Editor.TextEditEx();
		this.txtTel12 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_PrintingHeader2 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_AdditionalID2 = new Apzon.Controls.Editor.TextEditEx();
		this.lookUpEdit_Country2 = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.memoEdit_Address2 = new Apzon.Controls.Editor.MemoEditEx();
		this.tabPage_Defaults = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.lookUpEdit_Font = new DevExpress.XtraEditors.FontEdit();
		this.txt_Preview = new Apzon.Controls.Editor.TextBoxEx();
		this.buttonEdit_SalesEmployee = new Apzon.Controls.Editor.ButtonEditEx();
		this.checkEdit2 = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UserWarehouse = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowCreating = new Apzon.Controls.Editor.CheckEditEx();
		this.buttonEdit_CardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.buttonEdit_ChecksReceived = new Apzon.Controls.Editor.ButtonEditEx();
		this.buttonEdit_CashOnHand = new Apzon.Controls.Editor.ButtonEditEx();
		this.btE_Warehouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.label7 = new System.Windows.Forms.Label();
		this.lookUpEdit_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUpEdit_FontSize = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUpEdit_Language = new DevExpress.XtraEditors.LookUpEdit();
		this.lookUpEdit_Color = new DevExpress.XtraEditors.LookUpEdit();
		this.lb_FontSize = new Apzon.Controls.Controls.LabelLine();
		this.lb_Font = new Apzon.Controls.Controls.LabelLine();
		this.lb_Language = new Apzon.Controls.Controls.LabelLine();
		this.lb_Branch = new Apzon.Controls.Controls.LabelLine();
		this.lb_ChecksReceived = new Apzon.Controls.Controls.LabelLine();
		this.lb_CashOnHand = new Apzon.Controls.Controls.LabelLine();
		this.lb_Warehouse = new Apzon.Controls.Controls.LabelLine();
		this.lb_Color = new Apzon.Controls.Controls.LabelLine();
		this.txt_WarehouseName = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_CashOnHand = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_ChecksReceived = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_CardName = new Apzon.Controls.Editor.ButtonEditEx();
		this.tabPage_Print = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.lookUE_PrintRcpt = new DevExpress.XtraEditors.LookUpEdit();
		this.memoEdit_Remark = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_CopiesforManualSeries = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Copies = new Apzon.Controls.Editor.TextEditEx();
		this.lb_PrintPaymentWithInvoice = new Apzon.Controls.Controls.LabelLine();
		this.lb_CopiesforManualSeries = new Apzon.Controls.Controls.LabelLine();
		this.lb_CopiesInclOriginal = new Apzon.Controls.Controls.LabelLine();
		this.chk_PrintAmount = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PrintPaymentInvoiceInSuccession = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ItemNo = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PrintDiscountData = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ExportToPDF = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_EMailDocument = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PrintDocument = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ExportToMSWord = new Apzon.Controls.Editor.CheckEditEx();
		this.cbDocType = new Apzon.Forms.Commons.SeparatorComboBox();
		this.lb_Document = new Apzon.Controls.Controls.LabelLine();
		this.tabPage_CreditCards = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_CreditCards = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CreditCards = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabPage_Path = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_BrowerFolder = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_AttachmentsFolder = new Apzon.Controls.Controls.LabelLine();
		this.btE_AttachmentsFolder = new Apzon.Controls.Editor.TextEditEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Tab_Parent).BeginInit();
		this.Tab_Parent.SuspendLayout();
		this.tabPage_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tabControl_2).BeginInit();
		this.tabControl_2.SuspendLayout();
		this.tabPage_LocalLanguage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TelePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AdditionalID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address1.Properties).BeginInit();
		this.tabPage_ForeignLanguage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_EMail2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Phone22.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTel12.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AdditionalID2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Country2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address2.Properties).BeginInit();
		this.tabPage_Defaults.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Font.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_SalesEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UserWarehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowCreating.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_ChecksReceived.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_CashOnHand.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btE_Warehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_FontSize.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Language.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Color.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CashOnHand.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChecksReceived.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).BeginInit();
		this.tabPage_Print.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUE_PrintRcpt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Remark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CopiesforManualSeries.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Copies.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintAmount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintPaymentInvoiceInSuccession.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ItemNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintDiscountData.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExportToPDF.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EMailDocument.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintDocument.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExportToMSWord.Properties).BeginInit();
		this.tabPage_CreditCards.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_CreditCards).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CreditCards).BeginInit();
		this.tabPage_Path.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btE_AttachmentsFolder.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.txt_Code.Location = new System.Drawing.Point(75, 9);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Size = new System.Drawing.Size(158, 20);
		this.txt_Code.TabIndex = 1;
		this.txt_Name.Location = new System.Drawing.Point(331, 12);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Size = new System.Drawing.Size(274, 20);
		this.txt_Name.TabIndex = 2;
		this.Tab_Parent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.Tab_Parent.Location = new System.Drawing.Point(3, 52);
		this.Tab_Parent.Name = "Tab_Parent";
		this.Tab_Parent.SelectedTabPage = this.tabPage_General;
		this.Tab_Parent.Size = new System.Drawing.Size(618, 497);
		this.Tab_Parent.TabIndex = 2;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.Tab_Parent.TabPages;
		DevExpress.XtraTab.XtraTabPage[] pages = new Apzon.Controls.Controls.XtraTabPageEx[5] { this.tabPage_General, this.tabPage_Defaults, this.tabPage_Print, this.tabPage_CreditCards, this.tabPage_Path };
		tabPages.AddRange(pages);
		this.tabPage_General.Controls.Add(this.tabControl_2);
		this.tabPage_General.Name = "tabPage_General";
		this.tabPage_General.Size = new System.Drawing.Size(612, 469);
		this.tabPage_General.Text = "General";
		this.tabControl_2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl_2.Location = new System.Drawing.Point(0, 0);
		this.tabControl_2.Name = "tabControl_2";
		this.tabControl_2.SelectedTabPage = this.tabPage_LocalLanguage;
		this.tabControl_2.Size = new System.Drawing.Size(612, 469);
		this.tabControl_2.TabIndex = 1;
		DevExpress.XtraTab.XtraTabPageCollection tabPages2 = this.tabControl_2.TabPages;
		pages = new Apzon.Controls.Controls.XtraTabPageEx[2] { this.tabPage_LocalLanguage, this.tabPage_ForeignLanguage };
		tabPages2.AddRange(pages);
		this.tabPage_LocalLanguage.Controls.Add(this.labelLine8);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_Email);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_Fax);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_TelePhone);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_Tel1);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_PrintingHeader);
		this.tabPage_LocalLanguage.Controls.Add(this.txt_AdditionalID);
		this.tabPage_LocalLanguage.Controls.Add(this.lookUpEdit_Country);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_EMail);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_Fax);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_Telephone2);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_Tel1);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_PrintingHeader);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_AdditionalID);
		this.tabPage_LocalLanguage.Controls.Add(this.lb_Country);
		this.tabPage_LocalLanguage.Controls.Add(this.memoEdit_Address1);
		this.tabPage_LocalLanguage.Name = "tabPage_LocalLanguage";
		this.tabPage_LocalLanguage.Size = new System.Drawing.Size(606, 441);
		this.tabPage_LocalLanguage.Text = "Local Language";
		this.labelLine8.Location = new System.Drawing.Point(16, 11);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.Size = new System.Drawing.Size(207, 20);
		this.labelLine8.TabIndex = 5;
		this.labelLine8.Utext = "Address";
		this.txt_Email.Location = new System.Drawing.Point(224, 235);
		this.txt_Email.Name = "txt_Email";
		this.txt_Email.Size = new System.Drawing.Size(358, 20);
		this.txt_Email.TabIndex = 10;
		this.txt_Email.EditValueChanged += new System.EventHandler(this.txt_Email_EditValueChanged);
		this.txt_Fax.Location = new System.Drawing.Point(224, 209);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.Size = new System.Drawing.Size(358, 20);
		this.txt_Fax.TabIndex = 9;
		this.txt_TelePhone.Location = new System.Drawing.Point(224, 183);
		this.txt_TelePhone.Name = "txt_TelePhone";
		this.txt_TelePhone.Size = new System.Drawing.Size(358, 20);
		this.txt_TelePhone.TabIndex = 8;
		this.txt_Tel1.Location = new System.Drawing.Point(224, 157);
		this.txt_Tel1.Name = "txt_Tel1";
		this.txt_Tel1.Size = new System.Drawing.Size(358, 20);
		this.txt_Tel1.TabIndex = 7;
		this.txt_PrintingHeader.Location = new System.Drawing.Point(224, 131);
		this.txt_PrintingHeader.Name = "txt_PrintingHeader";
		this.txt_PrintingHeader.Size = new System.Drawing.Size(358, 20);
		this.txt_PrintingHeader.TabIndex = 6;
		this.txt_AdditionalID.Location = new System.Drawing.Point(224, 105);
		this.txt_AdditionalID.Name = "txt_AdditionalID";
		this.txt_AdditionalID.Size = new System.Drawing.Size(358, 20);
		this.txt_AdditionalID.TabIndex = 5;
		this.txt_AdditionalID.EditValueChanged += new System.EventHandler(this.txt_AdditionalID_EditValueChanged);
		this.lookUpEdit_Country.Location = new System.Drawing.Point(224, 79);
		this.lookUpEdit_Country.Name = "lookUpEdit_Country";
		this.lookUpEdit_Country.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.lookUpEdit_Country.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUpEdit_Country.Properties.DisplayMember = "Name";
		this.lookUpEdit_Country.Properties.NullText = "";
		this.lookUpEdit_Country.Properties.ValueMember = "Code";
		this.lookUpEdit_Country.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lookUpEdit_Country_Properties_ButtonClick);
		this.lookUpEdit_Country.Size = new System.Drawing.Size(358, 20);
		this.lookUpEdit_Country.TabIndex = 4;
		this.lookUpEdit_Country.EditValueChanged += new System.EventHandler(this.lookUpEdit_Country_EditValueChanged);
		this.lb_EMail.Location = new System.Drawing.Point(16, 238);
		this.lb_EMail.Name = "lb_EMail";
		this.lb_EMail.Size = new System.Drawing.Size(213, 20);
		this.lb_EMail.TabIndex = 2;
		this.lb_EMail.Utext = "E-Mail";
		this.lb_Fax.Location = new System.Drawing.Point(16, 212);
		this.lb_Fax.Name = "lb_Fax";
		this.lb_Fax.Size = new System.Drawing.Size(213, 20);
		this.lb_Fax.TabIndex = 2;
		this.lb_Fax.Utext = "Fax";
		this.lb_Telephone2.Location = new System.Drawing.Point(16, 186);
		this.lb_Telephone2.Name = "lb_Telephone2";
		this.lb_Telephone2.Size = new System.Drawing.Size(213, 20);
		this.lb_Telephone2.TabIndex = 2;
		this.lb_Telephone2.Utext = "Telephone 2";
		this.lb_Tel1.Location = new System.Drawing.Point(16, 160);
		this.lb_Tel1.Name = "lb_Tel1";
		this.lb_Tel1.Size = new System.Drawing.Size(213, 20);
		this.lb_Tel1.TabIndex = 2;
		this.lb_Tel1.Utext = "Tel. 1";
		this.lb_PrintingHeader.Location = new System.Drawing.Point(16, 134);
		this.lb_PrintingHeader.Name = "lb_PrintingHeader";
		this.lb_PrintingHeader.Size = new System.Drawing.Size(213, 20);
		this.lb_PrintingHeader.TabIndex = 2;
		this.lb_PrintingHeader.Utext = "Printing Header";
		this.lb_AdditionalID.Location = new System.Drawing.Point(16, 108);
		this.lb_AdditionalID.Name = "lb_AdditionalID";
		this.lb_AdditionalID.Size = new System.Drawing.Size(213, 20);
		this.lb_AdditionalID.TabIndex = 2;
		this.lb_AdditionalID.Utext = "Additional ID";
		this.lb_Country.Location = new System.Drawing.Point(16, 82);
		this.lb_Country.Name = "lb_Country";
		this.lb_Country.Size = new System.Drawing.Size(213, 20);
		this.lb_Country.TabIndex = 2;
		this.lb_Country.Utext = "Country";
		this.memoEdit_Address1.Location = new System.Drawing.Point(224, 11);
		this.memoEdit_Address1.Name = "memoEdit_Address1";
		this.memoEdit_Address1.Size = new System.Drawing.Size(358, 59);
		this.memoEdit_Address1.TabIndex = 3;
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine13);
		this.tabPage_ForeignLanguage.Controls.Add(this.txt_EMail2);
		this.tabPage_ForeignLanguage.Controls.Add(this.txt_Fax2);
		this.tabPage_ForeignLanguage.Controls.Add(this.txt_Phone22);
		this.tabPage_ForeignLanguage.Controls.Add(this.txtTel12);
		this.tabPage_ForeignLanguage.Controls.Add(this.txt_PrintingHeader2);
		this.tabPage_ForeignLanguage.Controls.Add(this.txt_AdditionalID2);
		this.tabPage_ForeignLanguage.Controls.Add(this.lookUpEdit_Country2);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine1);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine2);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine3);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine4);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine5);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine6);
		this.tabPage_ForeignLanguage.Controls.Add(this.labelLine7);
		this.tabPage_ForeignLanguage.Controls.Add(this.memoEdit_Address2);
		this.tabPage_ForeignLanguage.Name = "tabPage_ForeignLanguage";
		this.tabPage_ForeignLanguage.Size = new System.Drawing.Size(606, 441);
		this.tabPage_ForeignLanguage.Text = "Foreign Language";
		this.labelLine13.Location = new System.Drawing.Point(20, 10);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.Size = new System.Drawing.Size(202, 20);
		this.labelLine13.TabIndex = 21;
		this.labelLine13.Utext = "Address";
		this.txt_EMail2.Location = new System.Drawing.Point(228, 232);
		this.txt_EMail2.Name = "txt_EMail2";
		this.txt_EMail2.Size = new System.Drawing.Size(358, 20);
		this.txt_EMail2.TabIndex = 15;
		this.txt_EMail2.EditValueChanged += new System.EventHandler(this.txt_EMail2_EditValueChanged);
		this.txt_Fax2.Location = new System.Drawing.Point(228, 206);
		this.txt_Fax2.Name = "txt_Fax2";
		this.txt_Fax2.Size = new System.Drawing.Size(358, 20);
		this.txt_Fax2.TabIndex = 16;
		this.txt_Phone22.Location = new System.Drawing.Point(228, 180);
		this.txt_Phone22.Name = "txt_Phone22";
		this.txt_Phone22.Size = new System.Drawing.Size(358, 20);
		this.txt_Phone22.TabIndex = 17;
		this.txtTel12.Location = new System.Drawing.Point(228, 154);
		this.txtTel12.Name = "txtTel12";
		this.txtTel12.Size = new System.Drawing.Size(358, 20);
		this.txtTel12.TabIndex = 18;
		this.txt_PrintingHeader2.Location = new System.Drawing.Point(228, 128);
		this.txt_PrintingHeader2.Name = "txt_PrintingHeader2";
		this.txt_PrintingHeader2.Size = new System.Drawing.Size(358, 20);
		this.txt_PrintingHeader2.TabIndex = 19;
		this.txt_AdditionalID2.Location = new System.Drawing.Point(228, 102);
		this.txt_AdditionalID2.Name = "txt_AdditionalID2";
		this.txt_AdditionalID2.Size = new System.Drawing.Size(358, 20);
		this.txt_AdditionalID2.TabIndex = 20;
		this.txt_AdditionalID2.EditValueChanged += new System.EventHandler(this.txt_AdditionalID2_EditValueChanged);
		this.lookUpEdit_Country2.Location = new System.Drawing.Point(228, 76);
		this.lookUpEdit_Country2.Name = "lookUpEdit_Country2";
		this.lookUpEdit_Country2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.lookUpEdit_Country2.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lookUpEdit_Country2.Properties.DisplayMember = "Name";
		this.lookUpEdit_Country2.Properties.NullText = "";
		this.lookUpEdit_Country2.Properties.ValueMember = "Code";
		this.lookUpEdit_Country2.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lookUpEdit_Country2_Properties_ButtonClick);
		this.lookUpEdit_Country2.Size = new System.Drawing.Size(358, 20);
		this.lookUpEdit_Country2.TabIndex = 14;
		this.lookUpEdit_Country2.EditValueChanged += new System.EventHandler(this.lookUpEdit_Country2_EditValueChanged);
		this.labelLine1.Location = new System.Drawing.Point(20, 235);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(202, 20);
		this.labelLine1.TabIndex = 7;
		this.labelLine1.Utext = "E-Mail";
		this.labelLine2.Location = new System.Drawing.Point(20, 209);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(202, 20);
		this.labelLine2.TabIndex = 8;
		this.labelLine2.Utext = "Fax";
		this.labelLine3.Location = new System.Drawing.Point(20, 183);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(202, 20);
		this.labelLine3.TabIndex = 9;
		this.labelLine3.Utext = "Telephone 2";
		this.labelLine4.Location = new System.Drawing.Point(20, 157);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(202, 20);
		this.labelLine4.TabIndex = 10;
		this.labelLine4.Utext = "Tel. 1";
		this.labelLine5.Location = new System.Drawing.Point(20, 131);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.Size = new System.Drawing.Size(202, 20);
		this.labelLine5.TabIndex = 11;
		this.labelLine5.Utext = "Printing Header";
		this.labelLine6.Location = new System.Drawing.Point(20, 105);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.Size = new System.Drawing.Size(202, 20);
		this.labelLine6.TabIndex = 12;
		this.labelLine6.Utext = "Additional ID";
		this.labelLine7.Location = new System.Drawing.Point(20, 79);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.Size = new System.Drawing.Size(202, 20);
		this.labelLine7.TabIndex = 13;
		this.labelLine7.Utext = "Country";
		this.memoEdit_Address2.Location = new System.Drawing.Point(228, 8);
		this.memoEdit_Address2.Name = "memoEdit_Address2";
		this.memoEdit_Address2.Size = new System.Drawing.Size(358, 59);
		this.memoEdit_Address2.TabIndex = 6;
		this.tabPage_Defaults.Controls.Add(this.labelLine12);
		this.tabPage_Defaults.Controls.Add(this.labelLine11);
		this.tabPage_Defaults.Controls.Add(this.lookUpEdit_Font);
		this.tabPage_Defaults.Controls.Add(this.txt_Preview);
		this.tabPage_Defaults.Controls.Add(this.buttonEdit_SalesEmployee);
		this.tabPage_Defaults.Controls.Add(this.checkEdit2);
		this.tabPage_Defaults.Controls.Add(this.chk_UserWarehouse);
		this.tabPage_Defaults.Controls.Add(this.chk_AllowCreating);
		this.tabPage_Defaults.Controls.Add(this.buttonEdit_CardCode);
		this.tabPage_Defaults.Controls.Add(this.buttonEdit_ChecksReceived);
		this.tabPage_Defaults.Controls.Add(this.buttonEdit_CashOnHand);
		this.tabPage_Defaults.Controls.Add(this.btE_Warehouse);
		this.tabPage_Defaults.Controls.Add(this.label7);
		this.tabPage_Defaults.Controls.Add(this.lookUpEdit_Branch);
		this.tabPage_Defaults.Controls.Add(this.lookUpEdit_FontSize);
		this.tabPage_Defaults.Controls.Add(this.lookUpEdit_Language);
		this.tabPage_Defaults.Controls.Add(this.lookUpEdit_Color);
		this.tabPage_Defaults.Controls.Add(this.lb_FontSize);
		this.tabPage_Defaults.Controls.Add(this.lb_Font);
		this.tabPage_Defaults.Controls.Add(this.lb_Language);
		this.tabPage_Defaults.Controls.Add(this.lb_Branch);
		this.tabPage_Defaults.Controls.Add(this.lb_ChecksReceived);
		this.tabPage_Defaults.Controls.Add(this.lb_CashOnHand);
		this.tabPage_Defaults.Controls.Add(this.lb_Warehouse);
		this.tabPage_Defaults.Controls.Add(this.lb_Color);
		this.tabPage_Defaults.Controls.Add(this.txt_WarehouseName);
		this.tabPage_Defaults.Controls.Add(this.txt_CashOnHand);
		this.tabPage_Defaults.Controls.Add(this.txt_ChecksReceived);
		this.tabPage_Defaults.Controls.Add(this.txt_CardName);
		this.tabPage_Defaults.Name = "tabPage_Defaults";
		this.tabPage_Defaults.Size = new System.Drawing.Size(612, 469);
		this.tabPage_Defaults.Text = "Defaults";
		this.labelLine12.Location = new System.Drawing.Point(8, 199);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.Size = new System.Drawing.Size(156, 20);
		this.labelLine12.TabIndex = 13;
		this.labelLine12.Utext = "Sales Employee";
		this.labelLine11.Location = new System.Drawing.Point(7, 113);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.Size = new System.Drawing.Size(156, 20);
		this.labelLine11.TabIndex = 12;
		this.labelLine11.Utext = "Preview";
		this.labelLine11.Load += new System.EventHandler(this.labelLine11_Load);
		this.lookUpEdit_Font.Location = new System.Drawing.Point(169, 61);
		this.lookUpEdit_Font.Name = "lookUpEdit_Font";
		this.lookUpEdit_Font.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_Font.Size = new System.Drawing.Size(317, 20);
		this.lookUpEdit_Font.TabIndex = 11;
		this.lookUpEdit_Font.EditValueChanged += new System.EventHandler(this.lookUpEdit_Font_EditValueChanged);
		this.txt_Preview.Location = new System.Drawing.Point(170, 113);
		this.txt_Preview.Multiline = true;
		this.txt_Preview.Name = "txt_Preview";
		this.txt_Preview.ReadOnly = true;
		this.txt_Preview.Size = new System.Drawing.Size(317, 79);
		this.txt_Preview.TabIndex = 10;
		this.txt_Preview.Text = "\r\n\r\nAaBcYyZz - abcd";
		this.txt_Preview.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.buttonEdit_SalesEmployee.Location = new System.Drawing.Point(170, 198);
		this.buttonEdit_SalesEmployee.Name = "buttonEdit_SalesEmployee";
		this.buttonEdit_SalesEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.buttonEdit_SalesEmployee.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit2_Properties_ButtonClick);
		this.buttonEdit_SalesEmployee.Size = new System.Drawing.Size(317, 20);
		this.buttonEdit_SalesEmployee.TabIndex = 9;
		this.checkEdit2.Location = new System.Drawing.Point(16, 437);
		this.checkEdit2.Name = "checkEdit2";
		this.checkEdit2.Properties.Caption = "Use Warehouse Address in A/P Documents";
		this.checkEdit2.Properties.ValueChecked = "Y";
		this.checkEdit2.Properties.ValueUnchecked = "N";
		this.checkEdit2.Size = new System.Drawing.Size(230, 19);
		this.checkEdit2.TabIndex = 8;
		this.chk_UserWarehouse.Location = new System.Drawing.Point(16, 398);
		this.chk_UserWarehouse.Name = "chk_UserWarehouse";
		this.chk_UserWarehouse.Properties.Caption = "Use Warehouse Address in A/P Documents";
		this.chk_UserWarehouse.Properties.ValueChecked = "Y";
		this.chk_UserWarehouse.Properties.ValueUnchecked = "N";
		this.chk_UserWarehouse.Size = new System.Drawing.Size(230, 19);
		this.chk_UserWarehouse.TabIndex = 8;
		this.chk_AllowCreating.Location = new System.Drawing.Point(16, 418);
		this.chk_AllowCreating.Name = "chk_AllowCreating";
		this.chk_AllowCreating.Properties.Caption = "Allow Creating Fixed Assets in Marketing Documents";
		this.chk_AllowCreating.Properties.ValueChecked = "Y";
		this.chk_AllowCreating.Properties.ValueUnchecked = "N";
		this.chk_AllowCreating.Size = new System.Drawing.Size(273, 19);
		this.chk_AllowCreating.TabIndex = 8;
		this.buttonEdit_CardCode.Location = new System.Drawing.Point(170, 328);
		this.buttonEdit_CardCode.Name = "buttonEdit_CardCode";
		this.buttonEdit_CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.buttonEdit_CardCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit4_Properties_ButtonClick);
		this.buttonEdit_CardCode.Size = new System.Drawing.Size(100, 20);
		this.buttonEdit_CardCode.TabIndex = 5;
		this.buttonEdit_ChecksReceived.Location = new System.Drawing.Point(170, 274);
		this.buttonEdit_ChecksReceived.Name = "buttonEdit_ChecksReceived";
		this.buttonEdit_ChecksReceived.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.buttonEdit_ChecksReceived.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit_ChecksReceived_Properties_ButtonClick);
		this.buttonEdit_ChecksReceived.Size = new System.Drawing.Size(100, 20);
		this.buttonEdit_ChecksReceived.TabIndex = 5;
		this.buttonEdit_CashOnHand.Location = new System.Drawing.Point(170, 248);
		this.buttonEdit_CashOnHand.Name = "buttonEdit_CashOnHand";
		this.buttonEdit_CashOnHand.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.buttonEdit_CashOnHand.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit_CashOnHand_Properties_ButtonClick);
		this.buttonEdit_CashOnHand.Size = new System.Drawing.Size(100, 20);
		this.buttonEdit_CashOnHand.TabIndex = 5;
		this.btE_Warehouse.Location = new System.Drawing.Point(170, 222);
		this.btE_Warehouse.Name = "btE_Warehouse";
		this.btE_Warehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btE_Warehouse.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btE_Warehouse_Properties_ButtonClick);
		this.btE_Warehouse.Size = new System.Drawing.Size(100, 20);
		this.btE_Warehouse.TabIndex = 5;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(8, 309);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(233, 13);
		this.label7.TabIndex = 0;
		this.label7.Text = "Default Customer for A/R Invoice and Payment";
		this.lookUpEdit_Branch.Location = new System.Drawing.Point(169, 359);
		this.lookUpEdit_Branch.Name = "lookUpEdit_Branch";
		this.lookUpEdit_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "BPLName"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLid", "BPLid", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.lookUpEdit_Branch.Properties.DisplayMember = "BPLName";
		this.lookUpEdit_Branch.Properties.NullText = "";
		this.lookUpEdit_Branch.Properties.ValueMember = "BPLId";
		this.lookUpEdit_Branch.Size = new System.Drawing.Size(317, 20);
		this.lookUpEdit_Branch.TabIndex = 4;
		this.lookUpEdit_FontSize.Location = new System.Drawing.Point(170, 87);
		this.lookUpEdit_FontSize.Name = "lookUpEdit_FontSize";
		this.lookUpEdit_FontSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_FontSize.Properties.DisplayMember = "Name";
		this.lookUpEdit_FontSize.Properties.NullText = "";
		this.lookUpEdit_FontSize.Properties.ValueMember = "Code";
		this.lookUpEdit_FontSize.Size = new System.Drawing.Size(317, 20);
		this.lookUpEdit_FontSize.TabIndex = 1;
		this.lookUpEdit_Language.Location = new System.Drawing.Point(170, 35);
		this.lookUpEdit_Language.Name = "lookUpEdit_Language";
		this.lookUpEdit_Language.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_Language.Properties.DisplayMember = "Name";
		this.lookUpEdit_Language.Properties.NullText = "";
		this.lookUpEdit_Language.Properties.ValueMember = "Code";
		this.lookUpEdit_Language.Size = new System.Drawing.Size(317, 20);
		this.lookUpEdit_Language.TabIndex = 1;
		this.lookUpEdit_Color.Location = new System.Drawing.Point(169, 9);
		this.lookUpEdit_Color.Name = "lookUpEdit_Color";
		this.lookUpEdit_Color.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_Color.Properties.DisplayMember = "Name";
		this.lookUpEdit_Color.Properties.NullText = "";
		this.lookUpEdit_Color.Properties.ValueMember = "Code";
		this.lookUpEdit_Color.Size = new System.Drawing.Size(317, 20);
		this.lookUpEdit_Color.TabIndex = 1;
		this.lb_FontSize.Location = new System.Drawing.Point(8, 90);
		this.lb_FontSize.Name = "lb_FontSize";
		this.lb_FontSize.Size = new System.Drawing.Size(156, 20);
		this.lb_FontSize.TabIndex = 0;
		this.lb_FontSize.Utext = "Font Size";
		this.lb_Font.Location = new System.Drawing.Point(8, 64);
		this.lb_Font.Name = "lb_Font";
		this.lb_Font.Size = new System.Drawing.Size(156, 20);
		this.lb_Font.TabIndex = 0;
		this.lb_Font.Utext = "Font";
		this.lb_Language.Location = new System.Drawing.Point(8, 38);
		this.lb_Language.Name = "lb_Language";
		this.lb_Language.Size = new System.Drawing.Size(156, 20);
		this.lb_Language.TabIndex = 0;
		this.lb_Language.Utext = "Language";
		this.lb_Branch.Location = new System.Drawing.Point(8, 362);
		this.lb_Branch.Name = "lb_Branch";
		this.lb_Branch.Size = new System.Drawing.Size(156, 20);
		this.lb_Branch.TabIndex = 0;
		this.lb_Branch.Utext = "Branch";
		this.lb_ChecksReceived.Location = new System.Drawing.Point(8, 277);
		this.lb_ChecksReceived.Name = "lb_ChecksReceived";
		this.lb_ChecksReceived.Size = new System.Drawing.Size(156, 20);
		this.lb_ChecksReceived.TabIndex = 0;
		this.lb_ChecksReceived.Utext = "Checks Received";
		this.lb_CashOnHand.Location = new System.Drawing.Point(8, 251);
		this.lb_CashOnHand.Name = "lb_CashOnHand";
		this.lb_CashOnHand.Size = new System.Drawing.Size(156, 20);
		this.lb_CashOnHand.TabIndex = 0;
		this.lb_CashOnHand.Utext = "Cash on Hand";
		this.lb_Warehouse.Location = new System.Drawing.Point(8, 224);
		this.lb_Warehouse.Name = "lb_Warehouse";
		this.lb_Warehouse.Size = new System.Drawing.Size(156, 20);
		this.lb_Warehouse.TabIndex = 0;
		this.lb_Warehouse.Utext = "Warehouse";
		this.lb_Color.Location = new System.Drawing.Point(8, 12);
		this.lb_Color.Name = "lb_Color";
		this.lb_Color.Size = new System.Drawing.Size(156, 20);
		this.lb_Color.TabIndex = 0;
		this.lb_Color.Utext = "Color";
		this.txt_WarehouseName.Location = new System.Drawing.Point(276, 223);
		this.txt_WarehouseName.Name = "txt_WarehouseName";
		this.txt_WarehouseName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_WarehouseName.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_WarehouseName_Properties_ButtonPressed);
		this.txt_WarehouseName.Size = new System.Drawing.Size(211, 20);
		this.txt_WarehouseName.TabIndex = 6;
		this.txt_CashOnHand.Location = new System.Drawing.Point(276, 248);
		this.txt_CashOnHand.Name = "txt_CashOnHand";
		this.txt_CashOnHand.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_CashOnHand.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_CashOnHand_Properties_ButtonClick);
		this.txt_CashOnHand.Size = new System.Drawing.Size(211, 20);
		this.txt_CashOnHand.TabIndex = 6;
		this.txt_ChecksReceived.Location = new System.Drawing.Point(276, 274);
		this.txt_ChecksReceived.Name = "txt_ChecksReceived";
		this.txt_ChecksReceived.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ChecksReceived.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ChecksReceived_Properties_ButtonClick);
		this.txt_ChecksReceived.Size = new System.Drawing.Size(211, 20);
		this.txt_ChecksReceived.TabIndex = 6;
		this.txt_CardName.Location = new System.Drawing.Point(276, 328);
		this.txt_CardName.Name = "txt_CardName";
		this.txt_CardName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_CardName.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_CardName_Properties_ButtonClick);
		this.txt_CardName.Size = new System.Drawing.Size(211, 20);
		this.txt_CardName.TabIndex = 6;
		this.tabPage_Print.Controls.Add(this.labelLine18);
		this.tabPage_Print.Controls.Add(this.labelLine16);
		this.tabPage_Print.Controls.Add(this.labelLine15);
		this.tabPage_Print.Controls.Add(this.labelLine14);
		this.tabPage_Print.Controls.Add(this.lookUE_PrintRcpt);
		this.tabPage_Print.Controls.Add(this.memoEdit_Remark);
		this.tabPage_Print.Controls.Add(this.txt_CopiesforManualSeries);
		this.tabPage_Print.Controls.Add(this.txt_Copies);
		this.tabPage_Print.Controls.Add(this.lb_PrintPaymentWithInvoice);
		this.tabPage_Print.Controls.Add(this.lb_CopiesforManualSeries);
		this.tabPage_Print.Controls.Add(this.lb_CopiesInclOriginal);
		this.tabPage_Print.Controls.Add(this.chk_PrintAmount);
		this.tabPage_Print.Controls.Add(this.chk_PrintPaymentInvoiceInSuccession);
		this.tabPage_Print.Controls.Add(this.chk_ItemNo);
		this.tabPage_Print.Controls.Add(this.chk_PrintDiscountData);
		this.tabPage_Print.Controls.Add(this.chk_ExportToPDF);
		this.tabPage_Print.Controls.Add(this.chk_EMailDocument);
		this.tabPage_Print.Controls.Add(this.chk_PrintDocument);
		this.tabPage_Print.Controls.Add(this.chk_ExportToMSWord);
		this.tabPage_Print.Controls.Add(this.cbDocType);
		this.tabPage_Print.Controls.Add(this.lb_Document);
		this.tabPage_Print.Name = "tabPage_Print";
		this.tabPage_Print.Size = new System.Drawing.Size(612, 469);
		this.tabPage_Print.Text = "Print";
		this.labelLine18.Location = new System.Drawing.Point(11, 309);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.Size = new System.Drawing.Size(230, 20);
		this.labelLine18.TabIndex = 12;
		this.labelLine18.Utext = "PLD Only Properties :";
		this.labelLine16.Location = new System.Drawing.Point(11, 243);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.Size = new System.Drawing.Size(230, 20);
		this.labelLine16.TabIndex = 10;
		this.labelLine16.Utext = "Permanent Remarks for Printing";
		this.labelLine16.Load += new System.EventHandler(this.labelLine16_Load);
		this.labelLine15.Location = new System.Drawing.Point(10, 69);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.Size = new System.Drawing.Size(230, 20);
		this.labelLine15.TabIndex = 9;
		this.labelLine15.Utext = "When Adding Document";
		this.labelLine14.Location = new System.Drawing.Point(11, 42);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.Size = new System.Drawing.Size(230, 20);
		this.labelLine14.TabIndex = 8;
		this.labelLine14.Utext = "PLD and CR Properties";
		this.lookUE_PrintRcpt.Location = new System.Drawing.Point(244, 337);
		this.lookUE_PrintRcpt.Name = "lookUE_PrintRcpt";
		this.lookUE_PrintRcpt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_PrintRcpt.Properties.DisplayMember = "Name";
		this.lookUE_PrintRcpt.Properties.NullText = "";
		this.lookUE_PrintRcpt.Properties.ValueMember = "Code";
		this.lookUE_PrintRcpt.Size = new System.Drawing.Size(321, 20);
		this.lookUE_PrintRcpt.TabIndex = 7;
		this.lookUE_PrintRcpt.EditValueChanged += new System.EventHandler(this.lookUE_PrintRcpt_EditValueChanged);
		this.memoEdit_Remark.Location = new System.Drawing.Point(244, 243);
		this.memoEdit_Remark.Name = "memoEdit_Remark";
		this.memoEdit_Remark.Size = new System.Drawing.Size(321, 54);
		this.memoEdit_Remark.TabIndex = 6;
		this.txt_CopiesforManualSeries.EditValue = "1";
		this.txt_CopiesforManualSeries.Location = new System.Drawing.Point(244, 212);
		this.txt_CopiesforManualSeries.Name = "txt_CopiesforManualSeries";
		this.txt_CopiesforManualSeries.Properties.Appearance.Options.UseTextOptions = true;
		this.txt_CopiesforManualSeries.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txt_CopiesforManualSeries.Size = new System.Drawing.Size(321, 20);
		this.txt_CopiesforManualSeries.TabIndex = 5;
		this.txt_Copies.EditValue = "1";
		this.txt_Copies.Location = new System.Drawing.Point(244, 186);
		this.txt_Copies.Name = "txt_Copies";
		this.txt_Copies.Properties.Appearance.Options.UseTextOptions = true;
		this.txt_Copies.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txt_Copies.Size = new System.Drawing.Size(321, 20);
		this.txt_Copies.TabIndex = 5;
		this.lb_PrintPaymentWithInvoice.Location = new System.Drawing.Point(11, 340);
		this.lb_PrintPaymentWithInvoice.Name = "lb_PrintPaymentWithInvoice";
		this.lb_PrintPaymentWithInvoice.Size = new System.Drawing.Size(230, 20);
		this.lb_PrintPaymentWithInvoice.TabIndex = 4;
		this.lb_PrintPaymentWithInvoice.Utext = "Print Payment with Invoice";
		this.lb_CopiesforManualSeries.Location = new System.Drawing.Point(11, 215);
		this.lb_CopiesforManualSeries.Name = "lb_CopiesforManualSeries";
		this.lb_CopiesforManualSeries.Size = new System.Drawing.Size(230, 20);
		this.lb_CopiesforManualSeries.TabIndex = 4;
		this.lb_CopiesforManualSeries.Utext = "Copies for Manual Series";
		this.lb_CopiesInclOriginal.Location = new System.Drawing.Point(11, 189);
		this.lb_CopiesInclOriginal.Name = "lb_CopiesInclOriginal";
		this.lb_CopiesInclOriginal.Size = new System.Drawing.Size(230, 20);
		this.lb_CopiesInclOriginal.TabIndex = 4;
		this.lb_CopiesInclOriginal.Utext = "Copies (Incl. Original)";
		this.chk_PrintAmount.EditValue = "Y";
		this.chk_PrintAmount.Location = new System.Drawing.Point(33, 438);
		this.chk_PrintAmount.Name = "chk_PrintAmount";
		this.chk_PrintAmount.Properties.Caption = "Print Amounts";
		this.chk_PrintAmount.Properties.ValueChecked = "Y";
		this.chk_PrintAmount.Properties.ValueUnchecked = "N";
		this.chk_PrintAmount.Size = new System.Drawing.Size(236, 19);
		this.chk_PrintAmount.TabIndex = 3;
		this.chk_PrintPaymentInvoiceInSuccession.Location = new System.Drawing.Point(33, 378);
		this.chk_PrintPaymentInvoiceInSuccession.Name = "chk_PrintPaymentInvoiceInSuccession";
		this.chk_PrintPaymentInvoiceInSuccession.Properties.Caption = "Print Payment & Invoice in Succession";
		this.chk_PrintPaymentInvoiceInSuccession.Properties.ValueChecked = "Y";
		this.chk_PrintPaymentInvoiceInSuccession.Properties.ValueUnchecked = "N";
		this.chk_PrintPaymentInvoiceInSuccession.Size = new System.Drawing.Size(236, 19);
		this.chk_PrintPaymentInvoiceInSuccession.TabIndex = 3;
		this.chk_ItemNo.Location = new System.Drawing.Point(33, 417);
		this.chk_ItemNo.Name = "chk_ItemNo";
		this.chk_ItemNo.Properties.Caption = "Print Mfr Catalog No. Instead of Item No";
		this.chk_ItemNo.Properties.ValueChecked = "Y";
		this.chk_ItemNo.Properties.ValueUnchecked = "N";
		this.chk_ItemNo.Size = new System.Drawing.Size(236, 19);
		this.chk_ItemNo.TabIndex = 3;
		this.chk_PrintDiscountData.Location = new System.Drawing.Point(33, 397);
		this.chk_PrintDiscountData.Name = "chk_PrintDiscountData";
		this.chk_PrintDiscountData.Properties.Caption = "Print Discount Data";
		this.chk_PrintDiscountData.Properties.ValueChecked = "Y";
		this.chk_PrintDiscountData.Properties.ValueUnchecked = "N";
		this.chk_PrintDiscountData.Size = new System.Drawing.Size(142, 19);
		this.chk_PrintDiscountData.TabIndex = 3;
		this.chk_ExportToPDF.Location = new System.Drawing.Point(33, 153);
		this.chk_ExportToPDF.Name = "chk_ExportToPDF";
		this.chk_ExportToPDF.Properties.Caption = "Export to PDF";
		this.chk_ExportToPDF.Properties.ValueChecked = "Y";
		this.chk_ExportToPDF.Properties.ValueUnchecked = "N";
		this.chk_ExportToPDF.Size = new System.Drawing.Size(142, 19);
		this.chk_ExportToPDF.TabIndex = 3;
		this.chk_EMailDocument.Location = new System.Drawing.Point(33, 133);
		this.chk_EMailDocument.Name = "chk_EMailDocument";
		this.chk_EMailDocument.Properties.Caption = "E-Mail Document";
		this.chk_EMailDocument.Properties.ValueChecked = "Y";
		this.chk_EMailDocument.Properties.ValueUnchecked = "N";
		this.chk_EMailDocument.Size = new System.Drawing.Size(142, 19);
		this.chk_EMailDocument.TabIndex = 3;
		this.chk_PrintDocument.Location = new System.Drawing.Point(33, 113);
		this.chk_PrintDocument.Name = "chk_PrintDocument";
		this.chk_PrintDocument.Properties.Caption = "Print Document";
		this.chk_PrintDocument.Properties.ValueChecked = "Y";
		this.chk_PrintDocument.Properties.ValueUnchecked = "N";
		this.chk_PrintDocument.Size = new System.Drawing.Size(142, 19);
		this.chk_PrintDocument.TabIndex = 3;
		this.chk_ExportToMSWord.Location = new System.Drawing.Point(33, 93);
		this.chk_ExportToMSWord.Name = "chk_ExportToMSWord";
		this.chk_ExportToMSWord.Properties.Caption = "Export to MS-Word";
		this.chk_ExportToMSWord.Properties.ValueChecked = "Y";
		this.chk_ExportToMSWord.Properties.ValueUnchecked = "N";
		this.chk_ExportToMSWord.Size = new System.Drawing.Size(142, 19);
		this.chk_ExportToMSWord.TabIndex = 3;
		this.cbDocType.AutoAdjustItemHeight = false;
		this.cbDocType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
		this.cbDocType.FormattingEnabled = true;
		this.cbDocType.Location = new System.Drawing.Point(244, 15);
		this.cbDocType.Name = "cbDocType";
		this.cbDocType.SeparatorColor = System.Drawing.Color.Black;
		this.cbDocType.SeparatorMargin = 1;
		this.cbDocType.SeparatorStyle = System.Drawing.Drawing2D.DashStyle.Solid;
		this.cbDocType.SeparatorWidth = 1;
		this.cbDocType.Size = new System.Drawing.Size(321, 22);
		this.cbDocType.TabIndex = 1;
		this.cbDocType.SelectedIndexChanged += new System.EventHandler(this.cbDocType_SelectedIndexChanged);
		this.lb_Document.Location = new System.Drawing.Point(8, 17);
		this.lb_Document.Name = "lb_Document";
		this.lb_Document.Size = new System.Drawing.Size(230, 20);
		this.lb_Document.TabIndex = 0;
		this.lb_Document.Utext = "Document";
		this.tabPage_CreditCards.Controls.Add(this.grd_CreditCards);
		this.tabPage_CreditCards.Name = "tabPage_CreditCards";
		this.tabPage_CreditCards.Size = new System.Drawing.Size(612, 469);
		this.tabPage_CreditCards.Text = "Credit Cards";
		this.grd_CreditCards.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_CreditCards.Location = new System.Drawing.Point(0, 0);
		this.grd_CreditCards.MainView = this.gv_CreditCards;
		this.grd_CreditCards.Name = "grd_CreditCards";
		this.grd_CreditCards.Size = new System.Drawing.Size(612, 469);
		this.grd_CreditCards.TabIndex = 0;
		this.grd_CreditCards.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CreditCards });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gv_CreditCards.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new Apzon.Controls.Grid.GridColumnEx[4] { this.gridColumn4, this.gridColumn1, this.gridColumn2, this.gridColumn3 };
		columns.AddRange(columns2);
		this.gv_CreditCards.GridControl = this.grd_CreditCards;
		this.gv_CreditCards.IndicatorWidth = 35;
		this.gv_CreditCards.Name = "gv_CreditCards";
		this.gv_CreditCards.OptionsView.ShowGroupPanel = false;
		this.gv_CreditCards.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CreditCards_CustomDrawRowIndicator);
		this.gridColumn4.Caption = "Code";
		this.gridColumn4.FieldName = "Code";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn1.Caption = "Credit Card";
		this.gridColumn1.FieldName = "CardName";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "G/L Account";
		this.gridColumn2.FieldName = "AcctCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Account Name";
		this.gridColumn3.FieldName = "AcctName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.tabPage_Path.Controls.Add(this.btn_BrowerFolder);
		this.tabPage_Path.Controls.Add(this.lb_AttachmentsFolder);
		this.tabPage_Path.Controls.Add(this.btE_AttachmentsFolder);
		this.tabPage_Path.Name = "tabPage_Path";
		this.tabPage_Path.Size = new System.Drawing.Size(612, 469);
		this.tabPage_Path.Text = "Path";
		this.btn_BrowerFolder.Location = new System.Drawing.Point(536, 28);
		this.btn_BrowerFolder.Name = "btn_BrowerFolder";
		this.btn_BrowerFolder.Size = new System.Drawing.Size(23, 20);
		this.btn_BrowerFolder.TabIndex = 3;
		this.btn_BrowerFolder.Text = "...";
		this.btn_BrowerFolder.Click += new System.EventHandler(this.btn_BrowerFolder_Click);
		this.lb_AttachmentsFolder.Location = new System.Drawing.Point(8, 31);
		this.lb_AttachmentsFolder.Name = "lb_AttachmentsFolder";
		this.lb_AttachmentsFolder.Size = new System.Drawing.Size(140, 20);
		this.lb_AttachmentsFolder.TabIndex = 0;
		this.lb_AttachmentsFolder.Utext = "Attachments Folder";
		this.btE_AttachmentsFolder.Location = new System.Drawing.Point(148, 28);
		this.btE_AttachmentsFolder.Name = "btE_AttachmentsFolder";
		this.btE_AttachmentsFolder.Size = new System.Drawing.Size(383, 20);
		this.btE_AttachmentsFolder.TabIndex = 2;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.Image = (System.Drawing.Image)resources.GetObject("bt_OK.Image");
		this.bt_OK.Location = new System.Drawing.Point(12, 555);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 11;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.Image");
		this.bt_Cancel.Location = new System.Drawing.Point(93, 555);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 12;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.labelLine9.Location = new System.Drawing.Point(3, 10);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.Size = new System.Drawing.Size(70, 20);
		this.labelLine9.TabIndex = 4;
		this.labelLine9.Utext = "Code";
		this.labelLine10.Location = new System.Drawing.Point(249, 12);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.Size = new System.Drawing.Size(80, 20);
		this.labelLine10.TabIndex = 5;
		this.labelLine10.Utext = "Description";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(622, 583);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.Tab_Parent);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.txt_Code);
		base.Name = "frm_UserDefault";
		base.ShowIcon = false;
		this.Text = "User Default";
		base.Load += new System.EventHandler(this.frm_UserDefault_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Tab_Parent).EndInit();
		this.Tab_Parent.ResumeLayout(false);
		this.tabPage_General.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tabControl_2).EndInit();
		this.tabControl_2.ResumeLayout(false);
		this.tabPage_LocalLanguage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TelePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tel1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AdditionalID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address1.Properties).EndInit();
		this.tabPage_ForeignLanguage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_EMail2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Phone22.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTel12.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PrintingHeader2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AdditionalID2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Country2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Address2.Properties).EndInit();
		this.tabPage_Defaults.ResumeLayout(false);
		this.tabPage_Defaults.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Font.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_SalesEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UserWarehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowCreating.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_ChecksReceived.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit_CashOnHand.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btE_Warehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_FontSize.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Language.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Color.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CashOnHand.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChecksReceived.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).EndInit();
		this.tabPage_Print.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lookUE_PrintRcpt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Remark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CopiesforManualSeries.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Copies.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintAmount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintPaymentInvoiceInSuccession.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ItemNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintDiscountData.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExportToPDF.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EMailDocument.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PrintDocument.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ExportToMSWord.Properties).EndInit();
		this.tabPage_CreditCards.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_CreditCards).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CreditCards).EndInit();
		this.tabPage_Path.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btE_AttachmentsFolder.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
