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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SetUp.Purchasing;

public class frm_LandedCosts : XtraFormEx
{
	private readonly bool multipleBroker = GlobalData.CompanyDetail != null && Function.ToString(GlobalData.CompanyDetail.Rows[0]["EnMlBrkInv"]) == "Y";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtLandedCosts;

	private DataTable _dtItems;

	private DataTable _dtCost;

	private DataTable _dtDetails;

	private DataTable _dtAttactment;

	private bool _isLoading;

	private double customer;

	private double customerValue;

	private double expnd;

	private double cost;

	private double projectCustomer;

	private double customerFc;

	private double customerValueFc;

	private double expndFc;

	private double costFc;

	private double projectCustomerFc;

	private double sumTotal;

	private double sumTotalFc;

	private double sumCostFc;

	private double sumCostTotalFc;

	private double tax1FC;

	private double tax2FC;

	private double sumCost;

	private double sumCostTotal;

	private double tax1;

	private double tax2;

	private double beforeVat;

	private double docTotal;

	private double costSum;

	private double beforeVatFC;

	private double docTotalFC;

	private double costSumFC;

	private bool flag;

	private string currencyItem = "";

	private string currencyDoc = "";

	private double rateItem;

	private double rateDoc;

	private bool success;

	private bool vwBreak;

	private List<CurrencyCodesImpl> salaryCurrData;

	private string _oldAgent = "";

	private IContainer components;

	private LabelLine labelLine1;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_LandedCost;

	private GridViewEx gv_LandedCost;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn43;

	private GridColumnEx gridColumn45;

	private GridColumnEx gridColumn62;

	private XtraTabControlEx xtra_DetailLandedCosts;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabPageEx xtraTabPage2;

	private GridColumnEx gridColumn64;

	private GridControlEx grd_Item;

	private GridViewEx gv_Item;

	private GridControlEx grd_Costs;

	private GridViewEx gv_Costs;

	private XtraTabPageEx xtraTabPage3;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private XtraTabPageEx xtraTabPage4;

	private GridColumnEx gridColumn65;

	private GridColumnEx gridColumn66;

	private GridColumnEx gridColumn69;

	private GridColumnEx gridColumn70;

	private GridColumnEx gridColumn71;

	private GridColumnEx gridColumn72;

	private GridColumnEx gridColumn73;

	private GridColumnEx gridColumn74;

	private GridColumnEx gridColumn75;

	private GridColumnEx gridColumn76;

	private GridColumnEx gridColumn77;

	private GridColumnEx gridColumn78;

	private GridColumnEx gridColumn79;

	private GridColumnEx gridColumn80;

	private GridColumnEx gridColumn81;

	private GridColumnEx gridColumn82;

	private GridColumnEx gridColumn83;

	private GridColumnEx gridColumn84;

	private GridColumnEx gridColumn85;

	private GridColumnEx gridColumn86;

	private GridColumnEx gridColumn87;

	private GridColumnEx gridColumn88;

	private GridColumnEx gridColumn92;

	private GridColumnEx gridColumn93;

	private GridColumnEx gridColumn94;

	private GridColumnEx gridColumn95;

	private GridColumnEx gridColumn96;

	private GridColumnEx gridColumn97;

	private GridColumnEx gridColumn98;

	private GridColumnEx gridColumn99;

	private GridColumnEx gridColumn100;

	private GridColumnEx gridColumn101;

	private RepositoryItemDateEdit r_datePosting;

	private RepositoryItemDateEdit r_dateDue;

	private RepositoryItemButtonEditCFL r_VendorItem;

	private RepositoryItemLookUpEdit r_lookCurrencyItem;

	private RepositoryItemDateEdit r_dateCus;

	private RepositoryItemButtonEdit r_btnItem;

	private RepositoryItemCalcEdit r_QtyItem;

	private RepositoryItemLookUpEdit r_lookCurrencyIte;

	private RepositoryItemButtonEdit r_btnWhsCodeItem;

	private RepositoryItemLookUpEdit r_lookOhType;

	private RepositoryItemCheckEdit r_checkCost;

	private RepositoryItemButtonEdit r_btnItemDetail;

	private SimpleButtonEx btn_NewLanded;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private TextEditEx txt_Num;

	private RepositoryItemLookUpEdit r_lookSeries;

	private DropDownButton drBtn_CopyFrom;

	private RepositoryItemLookUpEdit r_lookPriceList;

	private RepositoryItemLookUpEdit r_looExpenditure;

	private GridColumnEx gridColumn35;

	private GridColumnEx gridColumn37;

	private GridColumnEx gridColumn38;

	private GridColumnEx gridColumn39;

	private GridColumnEx gridColumn40;

	private GridColumnEx gridColumn41;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemTextEdit r_CostSum;

	private RepositoryItemTextEdit r_ttExpLC;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gridColumnEx30;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private RepositoryItemTextEditExPrice txt_value;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice3;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice4;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice5;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice6;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice7;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice8;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice9;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice10;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice11;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice12;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice13;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice14;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice15;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice16;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice17;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice18;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice19;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice20;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice21;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice22;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice23;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice24;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice25;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice26;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice27;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice28;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice29;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice30;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice31;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice32;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice33;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice34;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice35;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice36;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice37;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice38;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice39;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice40;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice41;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice42;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice43;

	private RepositoryItemButtonEditCFL r_BrockerItem;

	private ButtonEditCFL txt_Vender;

	private ButtonEditCFL txt_Broker;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ucAttachment ucAttachment1;

	private GridColumnEx gridColumnEx36;

	private RepositoryItemButtonEditCFL r_BrokerCost;

	private GridColumnEx gridColumnEx37;

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			btn_Ok.Enabled = true;
			btn_Cancel.Enabled = true;
			btn_Search.Enabled = true;
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			btn_Ok.Enabled = true;
			btn_Cancel.Enabled = true;
			btn_Search.Enabled = true;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_Ok.Enabled = true;
			btn_Cancel.Enabled = true;
			btn_Search.Enabled = true;
			break;
		case Enums.FormViewModeType.Busy:
			btn_Ok.Enabled = false;
			btn_Cancel.Enabled = false;
			btn_Search.Enabled = false;
			break;
		}
	}

	public frm_LandedCosts()
	{
		InitializeComponent();
		if (!multipleBroker)
		{
			if (gv_Costs.LstColumNotSetting == null)
			{
				gv_Costs.LstColumNotSetting = new List<string>();
			}
			gv_Costs.LstColumNotSetting.AddRange(new string[2] { "AgentCode", "AgentName" });
		}
		OnLoadAsync += Frm_LandedCosts_OnLoadAsync;
	}

	private async Task Frm_LandedCosts_OnLoadAsync(object sender, EventArgs e)
	{
		await frm_LandedCosts_Load(sender, e);
	}

	private async Task frm_LandedCosts_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			r_VendorItem.ValueMember = "CardCode";
			r_VendorItem.FieldNameInDataRow("CardCode", "CardName");
			r_VendorItem.FieldNameInGrid("CardCode", "SuppName");
			r_VendorItem.TypeCFL = Enums.TypeCFL.Vendor;
			r_VendorItem.LoadDataSourceWhenEnter = true;
			r_VendorItem.RefreshDataSource();
			r_VendorItem.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			r_BrockerItem.ValueMember = "CardCode";
			r_BrockerItem.FieldNameInDataRow("CardCode", "CardName");
			r_BrockerItem.FieldNameInGrid("AgentCode", "AgentName");
			r_BrockerItem.TypeCFL = Enums.TypeCFL.Vendor;
			r_BrockerItem.LoadDataSourceWhenEnter = true;
			r_BrockerItem.RefreshDataSource();
			r_BrockerItem.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			r_BrokerCost.ValueMember = "CardCode";
			r_BrokerCost.FieldNameInDataRow("CardCode", "CardName");
			r_BrokerCost.FieldNameInGrid("AgentCode", "AgentName");
			r_BrokerCost.TypeCFL = Enums.TypeCFL.Vendor;
			r_BrokerCost.LoadDataSourceWhenEnter = true;
			r_BrokerCost.RefreshDataSource();
			r_BrokerCost.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			txt_Broker.Properties.ValueMember = "CardCode";
			txt_Broker.Properties.TypeCFL = Enums.TypeCFL.Vendor;
			txt_Broker.Properties.LoadDataSourceWhenEnter = true;
			txt_Broker.Properties.RefreshDataSource();
			txt_Broker.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			txt_Vender.Properties.ValueMember = "CardCode";
			txt_Vender.Properties.TypeCFL = Enums.TypeCFL.Vendor;
			txt_Vender.Properties.LoadDataSourceWhenEnter = true;
			txt_Vender.Properties.RefreshDataSource();
			txt_Vender.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			gv_LandedCost.SetFocusedRowCellValue("DocCur", GlobalData.MainCurrency);
			gv_LandedCost.SetFocusedRowCellValue("DocDate", DateTime.Now.Date);
			gv_LandedCost.SetFocusedRowCellValue("DocDueDate", DateTime.Now.Date);
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string fromDate = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
			string toDate = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
			await SetDataNo();
			SetDataCurrency();
			SetDataAllowtionBy();
			SetDataExpenditure();
			LoadCopyMenu();
			await LoadPriceList();
			DataTable dataTable = await Search(fromDate, toDate, "", "", 0);
			grd_LandedCost.DataSource = dataTable;
			if (!dataTable.IsNotNull())
			{
				gv_LandedCost_FocusedRowObjectChanged(null, null);
			}
			GridControlEx gridControlEx = grd_Costs;
			gridControlEx.DataSource = await GetLandedCostName();
			SetVisiable(flag: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			string vendor = Function.ToString(txt_Vender.EditValue);
			string broker = Function.ToString(txt_Broker.EditValue);
			int docNum = Function.ParseInt(txt_Num.EditValue);
			string fromDate = ((date_FromDate.EditValue != null) ? date_FromDate.DateTime.ToString("ddMMyyyy") : "");
			string toDate = ((date_ToDate.EditValue != null) ? date_ToDate.DateTime.ToString("ddMMyyyy") : "");
			if (success)
			{
				return;
			}
			if (formMode != Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					FormMode = Enums.FormViewModeType.View;
					grd_Item.DataSourceTable.Rows.Clear();
					grd_Detail.DataSourceTable.Rows.Clear();
					ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
					GridControlEx gridControlEx = grd_LandedCost;
					gridControlEx.DataSource = await Search(fromDate, toDate, vendor, broker, docNum);
				}
			}
			else
			{
				grd_Item.DataSourceTable.Rows.Clear();
				grd_Detail.DataSourceTable.Rows.Clear();
				ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
				GridControlEx gridControlEx = grd_LandedCost;
				gridControlEx.DataSource = await Search(fromDate, toDate, vendor, broker, docNum);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await Saves();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_CopyFrom_Click(object sender, EventArgs e)
	{
		try
		{
			LoadCopyMenu();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_LandedCost_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				await GetDetails(Function.ParseInt(gv_LandedCost.GetFocusedRowCellValue("DocEntry")));
				txt_Num.EditValue = Function.ParseInt(gv_LandedCost.GetFocusedRowCellValue("DocEntry"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void r_btnItem_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Item.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void r_btnWhsCodeItem_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Item.GetFocusedRowCellValue("WhsCode"));
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void r_btnItemDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Detail.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Costs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "OhType" || e.Column.FieldName == "CostSum")
			{
				ChangingTypeCost();
			}
			if (e.Column.FieldName == "CostSumFC")
			{
				ChangingTypeCostFC();
			}
			if (e.Column.FieldName == "InCustCalc")
			{
				CheckedIncludeCustomer();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LandedCost_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LandedCost_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLandedCosts = grd_LandedCost.DataSourceTable;
			_dtItems = grd_Item.DataSourceTable;
			_dtCost = grd_Costs.DataSourceTable;
			gv_Item.GetFocusedDataSourceRowIndex();
			int dataSourceRowIndex = gv_LandedCost.GetDataSourceRowIndex(e.RowHandle);
			bool flag = false;
			rateDoc = Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocRate"));
			currencyDoc = Function.ToString(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocCur"));
			int focusedDataSourceRowIndex = gv_LandedCost.GetFocusedDataSourceRowIndex();
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (e.Column.FieldName == "DocVat1" || e.Column.FieldName == "DocVat2")
			{
				if (string.Equals(currencyDoc, GlobalData.MainCurrency))
				{
					sumTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
					sumCost = grd_Costs.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSum"]));
					sumCostTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostLC"]));
					tax1 = Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocVat1"));
					tax2 = Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocVat2"));
					beforeVat = sumTotal + sumCost;
					docTotal = sumTotal + sumCost + tax1 + tax2;
					costSum = sumCostTotal;
				}
				else
				{
					sumTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValueFC"]));
					sumCost = grd_Costs.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSumFC"]));
					sumCostTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostFC"]));
					tax1FC = Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocVat1"));
					tax2FC = Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocVat2"));
					beforeVatFC = sumTotal + sumCost;
					docTotalFC = sumTotal + sumCost + tax1 + tax2;
					costSumFC = sumCostTotal;
					_dtLandedCosts.Rows[focusedDataSourceRowIndex]["BeforeVatFC"] = beforeVatFC;
					_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocTotalFC"] = docTotalFC;
					_dtLandedCosts.Rows[focusedDataSourceRowIndex]["CostSumFC"] = costSumFC;
					_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat1FC"] = tax1FC;
					_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat2FC"] = tax2FC;
					beforeVat = Math.Round(beforeVatFC * rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					docTotal = Math.Round(docTotalFC * rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					costSum = Math.Round(costSumFC * rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					tax1 = Math.Round(tax1FC * rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					tax2 = Math.Round(tax2FC * rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				}
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["BeforeVat"] = beforeVat;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocTotal"] = docTotal;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["CostSum"] = costSum;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat1"] = tax1;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat2"] = tax2;
			}
			else if (e.Column.FieldName == "CardCode")
			{
				if (FormMode != Enums.FormViewModeType.Add)
				{
					return;
				}
				DataRow dataRow = (from t in r_VendorItem.DataSource.AsEnumerable()
					where Function.ToString(t["CardCode"]) == Function.ToString(e.Value)
					select t).FirstOrDefault();
				if (dataRow == null)
				{
					return;
				}
				_dtLandedCosts.Rows[0]["SuppName"] = Function.ToString(dataRow["CardName"]);
				currencyDoc = Function.ToString(dataRow["Currency"]);
				if (currencyDoc == "##")
				{
					r_lookCurrencyItem.DataSource = GlobalData.Currencies;
					r_lookCurrencyItem.NullText = "";
					return;
				}
				List<Common.Table_Code> list = new List<Common.Table_Code>();
				if (currencyDoc == GlobalData.MainCurrency)
				{
					list.Add(new Common.Table_Code
					{
						Code = GlobalData.MainCurrency
					});
				}
				else
				{
					list.Add(new Common.Table_Code
					{
						Code = currencyDoc
					});
					list.Add(new Common.Table_Code
					{
						Code = GlobalData.MainCurrency
					});
				}
				r_lookCurrencyItem.DataSource = list;
				r_lookCurrencyItem.NullText = "";
			}
			else if (e.Column.FieldName == "DocCur")
			{
				currencyDoc = Function.ToString(e.Value);
				flag = string.Equals(currencyDoc, GlobalData.MainCurrency);
				SetVisiable(flag);
			}
			else if (e.Column.FieldName == "DocRate")
			{
				flag = string.Equals(currencyDoc, GlobalData.MainCurrency);
				for (int num2 = 0; num2 < _dtItems.Rows.Count; num2++)
				{
					_dtItems.Rows[num2]["DocCur"] = currencyDoc;
					double num3 = Function.ParseDouble(_dtItems.Rows[num2]["FobValue"]);
					if (!flag && num3 > 0.0 && rateDoc > 0.0)
					{
						_dtItems.Rows[num2]["FobValueFC"] = Math.Round(num3 / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["CstmsRate"] = rateDoc;
						_dtItems.Rows[num2]["TtlCustFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["TtlCustLC"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["CustomFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["Custom"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["TtlExpndFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["TtlExpndLC"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["CostFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["Cost"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["PricAtWHFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["PriceAtWH"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						_dtItems.Rows[num2]["TtlCostFC"] = Math.Round(Function.ParseDouble(_dtItems.Rows[num2]["TtlCostLC"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					}
				}
				if (!flag && rateDoc > 0.0)
				{
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocVat1"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "Vat1")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocVat2"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "Vat2")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocBeforeVat"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "BeforeVat")), (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["Total"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "DocTotal")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocCostSum"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "CostSum")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocExpCustom"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "ExpCustom")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					_dtLandedCosts.Rows[dataSourceRowIndex]["DocActCustom"] = Math.Round(Function.ParseDouble(gv_LandedCost.GetRowCellValue(e.RowHandle, "ActCustom")) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				}
			}
			else
			{
				if (!(e.Column.FieldName == "AgentCode"))
				{
					return;
				}
				if (multipleBroker && _oldAgent != Function.ToString(e.Value) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyBroker2Rows, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, Function.ToString(e.Value)) == DialogResult.Yes)
				{
					DataRow dataRow2 = r_BrokerCost.DataSource.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["CardCode"]) == Function.ToString(e.Value));
					for (int num4 = 0; num4 < gv_Costs.RowCount; num4++)
					{
						gv_Costs.SetRowCellValue(num4, "AgentCode", e.Value);
						gv_Costs.SetRowCellValue(num4, "AgentName", dataRow2?["CardName"]);
					}
				}
				_oldAgent = Function.ToString(e.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.RowHandle == -1)
			{
				return;
			}
			double num = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "Quantity"));
			currencyItem = Function.ToString(gv_Item.GetRowCellValue(e.RowHandle, "Currency"));
			currencyDoc = Function.ToString(gv_LandedCost.GetFocusedRowCellValue("DocCur"));
			rateItem = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "Rate"));
			rateDoc = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocRate"));
			int num2 = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num2 = currencyCodesImpl.Decimals;
				}
			}
			Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "FobValue"));
			int dataSourceRowIndex = gv_Item.GetDataSourceRowIndex(e.RowHandle);
			int focusedDataSourceRowIndex = gv_LandedCost.GetFocusedDataSourceRowIndex();
			_dtItems = grd_Item.DataSourceTable;
			_dtLandedCosts = grd_LandedCost.DataSourceTable;
			if (e.Column.FieldName == "CustomFC" || e.Column.FieldName == "CostFC" || e.Column.FieldName == "TtlExpndFC" || e.Column.FieldName == "TtlCustFC" || !string.Equals(currencyDoc, GlobalData.MainCurrency))
			{
				customerFc = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "CustomFC"));
				customerValueFc = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "TtlCustFC"));
				costFc = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "CostFC"));
				expndFc = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "TtlExpndFC"));
				if (num > 0.0)
				{
					if (e.Column.FieldName == "CustomFC")
					{
						customerValueFc = Math.Round(num * customerFc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "TtlCustFC")
					{
						customerFc = Math.Round(customerValueFc / num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "CostFC")
					{
						expndFc = Math.Round(costFc * num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "TtlExpndFC")
					{
						costFc = Math.Round(expndFc / num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
				}
				_dtItems.Rows[dataSourceRowIndex]["TtlCustFC"] = customerValueFc;
				_dtItems.Rows[dataSourceRowIndex]["CustomFC"] = customerFc;
				_dtItems.Rows[dataSourceRowIndex]["TtlExpndFC"] = expndFc;
				_dtItems.Rows[dataSourceRowIndex]["CostFC"] = costFc;
				customer = Math.Round(customerFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				customerValue = Math.Round(customerValueFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				cost = Math.Round(costFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				expnd = Math.Round(expndFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				_dtItems.Rows[dataSourceRowIndex]["TtlCustLC"] = customerValue;
				_dtItems.Rows[dataSourceRowIndex]["Custom"] = customer;
				_dtItems.Rows[dataSourceRowIndex]["TtlExpndLC"] = expnd;
				_dtItems.Rows[dataSourceRowIndex]["Cost"] = cost;
				double whsPrice = customer + Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "PriceFOB")) + cost;
				WhsCostPriceFc(customerFc, customerValueFc, costFc, expndFc, rateDoc, whsPrice);
				projectCustomerFc = grd_Item.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCustFC"]));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["ExCustomFC"] = projectCustomerFc;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["AcCustomFC"] = projectCustomerFc;
				projectCustomer = Math.Round(projectCustomerFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				sumTotalFc = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValueFC"]));
				sumCostFc = grd_Costs.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSumFC"]));
				sumCostTotalFc = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostFC"]));
				tax1FC = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat1FC"));
				tax2FC = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat2FC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["BeforVatFC"] = sumTotalFc + sumCostFc;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocTotalFC"] = sumTotalFc + sumCostFc + tax1FC + tax2FC;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["CostSumFC"] = sumCostTotalFc;
				tax1 = Math.Round(tax1FC * rateDoc, 1);
				tax2 = Math.Round(tax2FC * rateDoc, 1);
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat1"] = tax1;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Vat2"] = tax2;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["BeforeVat"] = Math.Round((sumTotalFc + sumCostFc) * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocTotal"] = Math.Round((sumTotalFc + sumCostFc + tax1FC + tax2FC) * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["CostSum"] = Math.Round(sumCostTotalFc * rateDoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
			}
			else if (e.Column.FieldName == "Custom" || e.Column.FieldName == "Cost" || e.Column.FieldName == "TtlExpndLC" || e.Column.FieldName == "TtlCustLC")
			{
				customer = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "Custom"));
				customerValue = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "TtlCustLC"));
				cost = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "Cost"));
				expnd = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "TtlExpndLC"));
				if (num > 0.0)
				{
					if (e.Column.FieldName == "Custom")
					{
						customerValue = Math.Round(num * customer, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "TtlCustLC")
					{
						customer = Math.Round(customerValue / num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "Cost")
					{
						expnd = Math.Round(cost * num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
					else if (e.Column.FieldName == "TtlExpndLC")
					{
						cost = Math.Round(expnd / num, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					}
				}
				_dtItems.Rows[dataSourceRowIndex]["TtlCustLC"] = customerValue;
				_dtItems.Rows[dataSourceRowIndex]["Custom"] = customer;
				_dtItems.Rows[dataSourceRowIndex]["TtlExpndLC"] = expnd;
				_dtItems.Rows[dataSourceRowIndex]["Cost"] = cost;
				WhsCostPrice(customer, customerValue, cost, expnd);
				sumTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
				sumCost = grd_Costs.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSum"]));
				sumCostTotal = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostLC"]));
				tax1 = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat1"));
				tax2 = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat2"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["BeforeVat"] = sumTotal + sumCost;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocTotal"] = sumTotal + sumCost + tax1 + tax2;
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["CostSum"] = sumCostTotal;
			}
			projectCustomer = grd_Item.DataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCustLC"]));
			_dtLandedCosts.Rows[focusedDataSourceRowIndex]["ExpCustom"] = projectCustomer;
			_dtLandedCosts.Rows[focusedDataSourceRowIndex]["ActCustom"] = projectCustomer;
			if (sumTotalFc > 0.0)
			{
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocVat1"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat1FC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocVat2"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat2FC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocBeforeVat"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("BeforVatFC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Total"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocTotalFC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocCostSum"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("CostSumFC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocExpCustom"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("ExCustomFC"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocActCustom"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("AcCustomFC"));
			}
			else
			{
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocVat1"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat1"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocVat2"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("Vat2"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocBeforeVat"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("BeforeVat"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["Total"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocTotal"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocCostSum"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("CostSum"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocExpCustom"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("ExpCustom"));
				_dtLandedCosts.Rows[focusedDataSourceRowIndex]["DocActCustom"] = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("ActCustom"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LandedCost_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ExpCustom" || e.Column.FieldName == "ActCustom" || e.Column.FieldName == "BeforeVat" || e.Column.FieldName == "DocTotal")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LandedCost_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.HighPriority = true;
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Costs_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AlcName" || e.Column.FieldName == "Factor")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Costs_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.HighPriority = true;
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ItemCode" || e.Column.FieldName == "PriceAtWH")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.HighPriority = true;
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Att_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.HighPriority = true;
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ItemCode" || e.Column.FieldName == "Quantity" || e.Column.FieldName == "Dscription" || e.Column.FieldName == "PriceFOB" || e.Column.FieldName == "FobValue" || e.Column.FieldName == "PriceAtWH" || e.Column.FieldName == "LineTotal" || e.Column.FieldName == "TtlCostLC" || e.Column.FieldName == "WhsCode" || e.Column.FieldName == "ReleaseNum" || e.Column.FieldName == "VarCosts" || e.Column.FieldName == "ConstCosts" || e.Column.FieldName == "UserCustom" || e.Column.FieldName == "FobnLaC" || e.Column.FieldName == "UomCode")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.HighPriority = true;
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			_dtCost = grd_Costs.DataSourceTable;
			_dtItems = grd_Item.DataSourceTable;
			_dtLandedCosts = grd_LandedCost.DatasourceTable();
			if (_dtLandedCosts == null || _dtLandedCosts.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < _dtCost.Rows.Count; i++)
			{
				gv_Costs.FocusedRowHandle = i;
				gv_Costs.SetRowCellValue(i, "CostSum", 0);
				for (int j = 0; j < _dtItems.Rows.Count; j++)
				{
					gv_Item.FocusedRowHandle = j;
					gv_Item.SetRowCellValue(j, "TtlExpndLC", 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NewLanded_Click(object sender, EventArgs e)
	{
		try
		{
			frm_LandedCostsSetup frm_LandedCostsSetup2 = new frm_LandedCostsSetup();
			frm_LandedCostsSetup2.SendDataBackToMain = (ReloadLandedCost)Delegate.Combine(frm_LandedCostsSetup2.SendDataBackToMain, new ReloadLandedCost(ReloadDelegate));
			frm_LandedCostsSetup2.StartPosition = FormStartPosition.CenterScreen;
			frm_LandedCostsSetup2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Item.DataRowCount > 0)
			{
				grd_Item.BeginUpdate();
				gv_Item.DeleteSelectedRows();
				grd_Item.EndUpdate();
				grd_Item.DataSourceTable.AcceptChanges();
				if (_dtLandedCosts != null && _dtLandedCosts.Rows.Count > 0)
				{
					currencyDoc = Function.ToString(_dtLandedCosts.Rows[0]["DocCur"]);
				}
				if (!string.Equals(currencyDoc, GlobalData.MainCurrency))
				{
					ChangingTypeCostFC();
				}
				else
				{
					ChangingTypeCost();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addRowToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private void gv_Detail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CostOH")
			{
				if (!string.Equals(currencyDoc, GlobalData.MainCurrency))
				{
					ChangingTypeCostFC();
				}
				else
				{
					ChangingTypeCost();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ucAttachment1_ButtonBrowse_Click_1(object sender, EventArgs e)
	{
	}

	private void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				grd_LandedCost.DataSourceTable();
				int maxDocEntry = await _httpClient.LandedCosts.GetMaxDocEntry();
				FormMode = Enums.FormViewModeType.Add;
				grd_Item.DataSourceTable.Rows.Clear();
				grd_Detail.DataSourceTable.Rows.Clear();
				DataTable dataTable = await GetLandedCostName();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["LineNum"] = i;
					dataTable.Rows[i]["DocEntry"] = maxDocEntry + 1;
				}
				dataTable.AcceptChanges();
				grd_Costs.DataSource = dataTable;
				_dtLandedCosts = grd_LandedCost.DataSourceTable.Clone();
				if (_dtLandedCosts == null || _dtLandedCosts.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow row = _dtLandedCosts.NewRow();
				if (r_lookSeries.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
				{
					row["Series"] = dataTable2.Rows[0]["Series"];
				}
				int num = await GetNextNumber(Function.ParseInt(row["Series"]));
				row["DocEntry"] = maxDocEntry + 1;
				row["DocNum"] = num;
				row["DocDate"] = DateTime.Now.Date;
				row["DocDueDate"] = DateTime.Now.Date;
				row["DocCur"] = GlobalData.MainCurrency;
				_dtLandedCosts.Rows.Add(row);
				grd_LandedCost.DataSource = _dtLandedCosts;
				_dtItems = grd_Item.DataSourceTable;
				if (_dtItems == null || _dtItems.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow row2 = _dtItems.NewRow();
				_dtItems.Rows.Add(row2);
				grd_Item.DataSource = _dtItems;
				_dtAttactment = ucAttachment1.grd_File.DataSourceTable;
				if (_dtAttactment == null || _dtAttactment.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				ucAttachment1.DataSource = _dtAttactment;
				SetVisiable(flag: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		btn_Search_Click(null, null);
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.LandedCosts.FindDataNextLast(Function.ParseInt(gv_LandedCost.GetFocusedRowCellValue("DocEntry")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_LandedCost.DataSource = dataTable;
				int docId = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
				await GetDetails(docId);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public async Task GetDetails(int docId)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			string doccur = Function.ToString(gv_LandedCost.GetFocusedRowCellValue("DocCur"));
			DataSet dataSet = await _httpClient.LandedCosts.GetDetailDocId(docId);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 2)
			{
				DataTable dataTable = dataSet.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["DocCur"] = doccur;
				}
				grd_Item.DataSource = dataTable;
				grd_Costs.DataSource = dataSet.Tables[1];
				grd_Detail.DataSource = dataSet.Tables[0];
				int absEntry = Function.ParseInt(gv_LandedCost.GetFocusedRowCellValue("AtcEntry"));
				await ucAttachment1.LoadAttachments(absEntry);
			}
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task SetDataNo()
	{
		DataTable dataTable = await _httpClient.LandedCosts.GetNo();
		r_lookSeries.DataSource = dataTable;
		r_lookSeries.DisplayMember = "SeriesName";
		r_lookSeries.ValueMember = "Series";
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			gv_LandedCost.SetFocusedRowCellValue("Series", dataTable.Rows[0]["Series"]);
		}
	}

	public async Task LoadPriceList()
	{
		DataTable dataTable = await _httpClient.LandedCosts.GetPriceList();
		r_lookPriceList.DataSource = dataTable;
		r_lookPriceList.DisplayMember = "ListName";
		r_lookPriceList.ValueMember = "ListNum";
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			gv_Detail.SetFocusedRowCellValue("PriceList", dataTable.Rows[0]["ListNum"]);
		}
	}

	public void SetDataCurrency()
	{
		try
		{
			salaryCurrData = GlobalData.CurrencyCodes;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = salaryCurrDatum.CurrCode;
				dataRow["Name"] = salaryCurrDatum.CurrName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			r_lookCurrencyItem.DataSource = dataTable.Copy();
			r_lookCurrencyItem.NullText = "";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataAllowtionBy()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Cash Value Before Customs",
			Code = "F"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Cash Value After Customs",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Quantity",
			Code = "Q"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Weight",
			Code = "W"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Volume",
			Code = "V"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Equal",
			Code = "A"
		});
		r_lookOhType.DataSource = list;
		r_lookOhType.DisplayMember = "Name";
		r_lookOhType.ValueMember = "Code";
	}

	public void SetDataExpenditure()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Yes",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "No",
			Code = "N"
		});
		r_looExpenditure.DataSource = list;
		r_looExpenditure.DisplayMember = "Name";
		r_looExpenditure.ValueMember = "Code";
	}

	public void LoadCopyMenu()
	{
		try
		{
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseLandedCost,
				ApzObjectTypes.oPurchaseInvoices
			};
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			foreach (ApzObjectTypes item in PopCopyFrom)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				int objType = 0;
				switch (item)
				{
				case ApzObjectTypes.oPurchaseDeliveryNotes:
					dXMenuItem.Caption = MenuCaption.GoodReceiptPO;
					objType = 20;
					break;
				case ApzObjectTypes.oPurchaseLandedCost:
					dXMenuItem.Caption = MenuCaption.LandedCosts;
					objType = 69;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					dXMenuItem.Caption = MenuCaption.APInvoices;
					objType = 18;
					break;
				}
				dXMenuItem.Click += (object s, EventArgs e) =>
				{
					WinCommons._OpenProcessing();
					try
					{
						FindByGoodInvoiceCFL(objType);
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
			drBtn_CopyFrom.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void FindByGoodInvoiceCFL(int objectType)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.LandedCostGodInvoice, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.Vendor = Function.ToString(gv_LandedCost.GetFocusedRowCellValue("CardCode"));
			frm_CFL2.ObjectType = objectType;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			grd_Item.DataSourceTable.Rows.Clear();
			_dtItems = grd_Item.DataSourceTable;
			_dtLandedCosts = grd_LandedCost.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				text = text + Function.ToString(frm_CFL2.ChooseItems[i]["DocEntry"]) + ",";
			}
			if (text.Length > 0)
			{
				DataTable dataTable = await _httpClient.LandedCosts.GetListDetailGLI(objectType, text);
				currencyItem = Function.ToString(dataTable.Rows[0]["Currency"]);
				currencyDoc = Function.ToString(gv_LandedCost.GetFocusedRowCellValue("DocCur"));
				rateItem = Function.ParseDouble(dataTable.Rows[0]["Rate"]);
				rateDoc = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocRate"));
				int num = -1;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
					if (currencyCodesImpl != null)
					{
						num = currencyCodesImpl.Decimals;
					}
				}
				int num2 = Function.ParseInt(Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocEntry")));
				for (int num3 = 0; num3 < dataTable.Rows.Count; num3++)
				{
					DataRow dataRow = _dtItems.NewRow();
					dataRow["DocEntry"] = num2 + 1;
					dataRow["LineNum"] = num3 + 1;
					dataRow["ItemCode"] = Function.ToString(dataTable.Rows[num3]["ItemCode"]);
					dataRow["Dscription"] = Function.ToString(dataTable.Rows[num3]["Dscription"]);
					dataRow["Quantity"] = Function.ParseDouble(dataTable.Rows[num3]["Quantity"]);
					dataRow["UomCode"] = Function.ToString(dataTable.Rows[num3]["UomCode"]);
					dataRow["WhsCode"] = Function.ToString(dataTable.Rows[num3]["WhsCode"]);
					dataRow["LineTotal"] = Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]);
					dataRow["TotalFrgn"] = Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]);
					dataRow["Reference"] = Function.ParseInt(dataTable.Rows[num3]["DocNum"]);
					dataRow["BaseEntry"] = Function.ParseInt(dataTable.Rows[num3]["DocEntry"]);
					dataRow["OrigLine"] = Function.ParseInt(dataTable.Rows[num3]["LineNum"]);
					dataRow["Locked"] = "N";
					if (_dtLandedCosts.IsNotNull())
					{
						dataRow["CstmsRate"] = Function.ParseDouble(_dtLandedCosts.Rows[0]["DocRate"]);
						dataRow["DocCur"] = Function.ToString(_dtLandedCosts.Rows[0]["DocCur"]);
					}
					dataRow["Currency"] = Function.ToString(dataTable.Rows[num3]["Currency"]);
					dataRow["Rate"] = Function.ParseDouble(dataTable.Rows[num3]["Rate"]);
					dataRow["FobValCurr"] = Function.ToString(dataTable.Rows[num3]["Currency"]);
					if (r_lookPriceList.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
					{
						dataRow["PriceList"] = dataTable2.Rows[0]["ListNum"];
					}
					dataRow["CostOH"] = "Y";
					if (objectType == 69)
					{
						dataRow["PriceFOB"] = Function.ParseDouble(dataTable.Rows[num3]["PriceFOB"]);
						dataRow["FobValue"] = Function.ParseDouble(dataTable.Rows[num3]["FobValue"]);
						dataRow["ReleaseNum"] = Function.ParseInt(dataTable.Rows[num3]["ReleaseNum"]);
						dataRow["VarCosts"] = Function.ParseDouble(dataTable.Rows[num3]["VarCosts"]);
						dataRow["ConstCosts"] = Function.ParseDouble(dataTable.Rows[num3]["ConstCosts"]);
						dataRow["UserCustom"] = Function.ParseDouble(dataTable.Rows[num3]["UserCustom"]);
						dataRow["FobnLaC"] = Function.ParseDouble(dataTable.Rows[num3]["FobnLaC"]);
						dataRow["Custom"] = Function.ParseDouble(dataTable.Rows[num3]["Custom"]);
						dataRow["Cost"] = Function.ParseDouble(dataTable.Rows[num3]["Cost"]);
						dataRow["TtlExpndLC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlExpndLC"]);
						dataRow["TtlCustLC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlCustLC"]);
						dataRow["TtlCostLC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlCostLC"]);
						dataRow["CardCode"] = Function.ParseDouble(dataTable.Rows[num3]["CardCode"]);
						dataRow["FobValueFC"] = Function.ParseDouble(dataTable.Rows[num3]["FobValueFC"]);
						dataRow["VarCostsFR"] = Function.ParseDouble(dataTable.Rows[num3]["VarCostsFR"]);
						dataRow["CnstCostFR"] = Function.ParseDouble(dataTable.Rows[num3]["CnstCostFR"]);
						dataRow["UsrCusomFC"] = Function.ParseDouble(dataTable.Rows[num3]["UsrCusomFC"]);
						dataRow["FobnLaCFC"] = Function.ParseDouble(dataTable.Rows[num3]["FobnLaCFC"]);
						dataRow["CustomFC"] = Function.ParseDouble(dataTable.Rows[num3]["CustomFC"]);
						dataRow["CostFC"] = Function.ParseDouble(dataTable.Rows[num3]["CostFC"]);
						dataRow["TtlExpndFC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlExpndFC"]);
						dataRow["TtlCustFC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlCustFC"]);
						dataRow["TtlCostFC"] = Function.ParseDouble(dataTable.Rows[num3]["TtlCostFC"]);
						dataRow["PricAtWHFC"] = Function.ParseDouble(dataTable.Rows[num3]["PricAtWHFC"]);
						dataRow["PriceAtWH"] = Function.ParseDouble(dataTable.Rows[num3]["PriceAtWH"]);
						dataRow["BaseType"] = 69;
					}
					else
					{
						dataRow["CardCode"] = Function.ParseDouble(dataTable.Rows[num3]["BaseCard"]);
						dataRow["PriceFOB"] = Function.ParseDouble(dataTable.Rows[num3]["Price"]);
						if (objectType != 20)
						{
							dataRow["FobValue"] = Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]);
						}
						dataRow["PriceAtWH"] = Function.ParseDouble(dataTable.Rows[num3]["Price"]);
						if (string.Equals(currencyDoc, GlobalData.MainCurrency))
						{
							dataRow["FobnLaC"] = Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]);
							dataRow["FobnLaCFC"] = 0;
						}
						else if (!string.Equals(currencyDoc, GlobalData.MainCurrency))
						{
							double num4 = Math.Round(Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]) * rateItem, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
							dataRow["FobnLaC"] = num4;
							if (!string.Equals(currencyDoc, currencyItem))
							{
								if (rateDoc > 0.0)
								{
									dataRow["FobnLaCFC"] = Math.Round(num4 / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
									dataRow["FobValueFC"] = Math.Round(Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]) / rateDoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
								}
							}
							else
							{
								dataRow["FobValueFC"] = Function.ParseDouble(dataTable.Rows[num3]["LineTotal"]);
								dataRow["FobnLaCFC"] = 0;
							}
						}
						dataRow["ReleaseNum"] = 0;
						dataRow["VarCosts"] = 0;
						dataRow["ConstCosts"] = 0;
						dataRow["UserCustom"] = 0;
						dataRow["Custom"] = 0;
						dataRow["Cost"] = 0;
						dataRow["TtlExpndLC"] = 0;
						dataRow["TtlCustLC"] = 0;
						dataRow["TtlCostLC"] = 0;
						dataRow["VarCostsFR"] = 0;
						dataRow["CnstCostFR"] = 0;
						dataRow["UsrCusomFC"] = 0;
						dataRow["CustomFC"] = 0;
						dataRow["CostFC"] = 0;
						dataRow["TtlExpndFC"] = 0;
						dataRow["TtlCustFC"] = 0;
						dataRow["TtlCostFC"] = 0;
						switch (objectType)
						{
						case 20:
							dataRow["BaseType"] = 20;
							break;
						case 18:
							dataRow["BaseType"] = 18;
							break;
						}
					}
					_dtItems.Rows.Add(dataRow);
				}
			}
			grd_Item.DataSource = _dtItems;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetLandedCostName()
	{
		try
		{
			return await _httpClient.LandedCosts.GetLandedCostName();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task<int> GetNextNumber(int series)
	{
		try
		{
			return await _httpClient.JobVacancy.GetNextNumber(series);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task<DataTable> Search(string fromDate, string toDate, string vendor, string broker, int docNum)
	{
		try
		{
			DataTable dataTable = await _httpClient.LandedCosts.SearchData(fromDate, toDate, vendor, broker, docNum);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (Function.ParseDouble(dataTable.Rows[i]["DocTotalFC"]) > 0.0)
				{
					dataTable.Rows[i]["DocVat1"] = Function.ParseDouble(dataTable.Rows[i]["Vat1FC"]);
					dataTable.Rows[i]["DocVat2"] = Function.ParseDouble(dataTable.Rows[i]["Vat2FC"]);
					dataTable.Rows[i]["DocExpCustom"] = Function.ParseDouble(dataTable.Rows[i]["ExCustomFC"]);
					dataTable.Rows[i]["DocActCustom"] = Function.ParseDouble(dataTable.Rows[i]["AcCustomFC"]);
					dataTable.Rows[i]["DocBeforeVat"] = Function.ParseDouble(dataTable.Rows[i]["BeforVatFC"]);
					dataTable.Rows[i]["Total"] = Function.ParseDouble(dataTable.Rows[i]["DocTotalFC"]);
					dataTable.Rows[i]["DocCostSum"] = Function.ParseDouble(dataTable.Rows[i]["CostSumFC"]);
				}
				else
				{
					dataTable.Rows[i]["DocVat1"] = Function.ParseDouble(dataTable.Rows[i]["Vat1"]);
					dataTable.Rows[i]["DocVat2"] = Function.ParseDouble(dataTable.Rows[i]["Vat2"]);
					dataTable.Rows[i]["DocExpCustom"] = Function.ParseDouble(dataTable.Rows[i]["ExpCustom"]);
					dataTable.Rows[i]["DocActCustom"] = Function.ParseDouble(dataTable.Rows[i]["ActCustom"]);
					dataTable.Rows[i]["DocBeforeVat"] = Function.ParseDouble(dataTable.Rows[i]["BeforeVat"]);
					dataTable.Rows[i]["Total"] = Function.ParseDouble(dataTable.Rows[i]["DocTotal"]);
					dataTable.Rows[i]["DocCostSum"] = Function.ParseDouble(dataTable.Rows[i]["CostSum"]);
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task Saves()
	{
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				DataTable dataTable = grd_LandedCost.DataSourceTable.Copy();
				DataTable dt = grd_Item.DataSourceTable.Copy();
				if (Function.ToString(dataTable.Rows[0]["Descr"]) == "")
				{
					int num = Function.ParseInt(_dtItems.Rows[0]["BaseType"]);
					int num2 = Function.ParseInt(_dtItems.Rows[0]["BaseEntry"]);
					switch (num)
					{
					case 20:
						dataTable.Rows[0]["Descr"] = "Based On Goods Receipt PO_" + num2;
						break;
					case 18:
						dataTable.Rows[0]["Descr"] = "Based On A/P Invoices_" + num2;
						break;
					case 69:
						dataTable.Rows[0]["Descr"] = "Based On Landed Costs_" + num2;
						break;
					}
				}
				DataTable dt2 = grd_Costs.DataSourceTable.Copy();
				if (ucAttachment1 != null && ucAttachment1.DataSource != null)
				{
					_dtAttactment = ucAttachment1.UploadFileNoSAP();
					if (!_dtAttactment.IsNotNull() && ucAttachment1.DataSource.IsNotNull())
					{
						WinCommons._CloseProcessing();
						FormMode = currentMode;
						return;
					}
				}
				if (!dataTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.CouldNotGetDataLandCostHeader, "");
					WinCommons._CloseProcessing();
					FormMode = currentMode;
					return;
				}
				LandedCostImpl landedCostImpl = EntitiesHelper<LandedCostImpl>.ToEntity(dataTable.Rows[0]);
				List<LandedCost_ItemLineImpl> itemLines = EntitiesHelper<LandedCost_ItemLineImpl>.ToEntities(dt);
				List<LandedCost_CostLineImpl> list = EntitiesHelper<LandedCost_CostLineImpl>.ToEntities(dt2);
				if (landedCostImpl == null)
				{
					WinCommons.Error(Enums.MessageCode.CouldNotGenerateDataLandCostHeader, "");
					WinCommons._CloseProcessing();
					FormMode = currentMode;
					return;
				}
				if (currentMode == Enums.FormViewModeType.Add)
				{
					landedCostImpl.DocEntry = 0;
				}
				if (!multipleBroker)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!string.IsNullOrEmpty(list[i].AgentCode))
						{
							list[i].AgentCode = null;
						}
						if (!string.IsNullOrEmpty(list[i].AgentName))
						{
							list[i].AgentName = null;
						}
					}
				}
				landedCostImpl.ItemLines = itemLines;
				landedCostImpl.CostLines = list;
				HttpResult httpResult = await _httpClient.LandedCosts.Save(landedCostImpl);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					success = true;
					GridControlEx gridControlEx = grd_LandedCost;
					gridControlEx.DataSource = await Search("", "", "", "", 0);
					FormMode = Enums.FormViewModeType.View;
					success = false;
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		FormMode = currentMode;
	}

	private double BaseBefore(double priceValue)
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			grd_Item.DataSourceTable.AcceptChanges();
			double result = 0.0;
			double num = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
			if (grd_Item.DataSourceTable.Rows.Count == 1)
			{
				result = 1.0;
			}
			else if (num > 0.0)
			{
				result = Math.Round(priceValue / num, 6);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private double BaseAfter(double priceValue, double ttlCustLC)
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			grd_Item.DataSourceTable.AcceptChanges();
			double num = 0.0;
			double num2 = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
			double num3 = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCustLC"]));
			double num4 = num2 + num3;
			double num5 = priceValue + ttlCustLC;
			if (grd_Item.DataSourceTable.Rows.Count == 1)
			{
				num = 1.0;
			}
			else if (num4 > 0.0)
			{
				num = Math.Round(num5 / num4, 6);
			}
			return Function.ParseDouble(num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private double CostQWV(double columValue, string key)
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			grd_Item.DataSourceTable.AcceptChanges();
			double result = 0.0;
			double num = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t[key]));
			if (grd_Item.DataSourceTable.Rows.Count == 1)
			{
				result = 1.0;
			}
			else if (num > 0.0)
			{
				result = Math.Round(columValue / num, 6);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private void ItemTotal()
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			_dtLandedCosts = grd_LandedCost.DataSourceTable;
			double num = 0.0;
			double num2 = Function.ParseDouble(gv_Item.GetFocusedRowCellValue("PriceAtWH"));
			double num3 = Function.ParseDouble(gv_Item.GetFocusedRowCellValue("Quantity"));
			num = Math.Round(num2 * num3, 1);
			gv_Item.SetFocusedRowCellValue("LineTotal", num);
			double num4 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["DocTotal"] = num4;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangingTypeCost()
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			_dtCost = grd_Costs.DataSourceTable;
			double num = 0.0;
			double num2 = 0.0;
			for (int num3 = 0; num3 < _dtItems.Rows.Count; num3++)
			{
				_dtItems.Rows[num3]["TtlExpndLC"] = 0;
				_dtItems.Rows[num3]["Cost"] = 0;
			}
			for (int num4 = 0; num4 < _dtCost.Rows.Count; num4++)
			{
				string text = Function.ToString(_dtCost.Rows[num4]["OhType"]);
				double num5 = Function.ParseDouble(_dtCost.Rows[num4]["CostSum"]);
				_dtCost.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSum"]));
				double fobValue = 0.0;
				if (_dtItems.IsNotNull())
				{
					fobValue = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
				}
				if (num5 > 0.0)
				{
					for (int num6 = 0; num6 < _dtItems.Rows.Count; num6++)
					{
						double priceValue = Function.ParseDouble(_dtItems.Rows[num6]["FobValue"]);
						double ttlCustLC = Function.ParseDouble(_dtItems.Rows[num6]["TtlCustLC"]);
						double columValue = Function.ParseDouble(_dtItems.Rows[num6]["Quantity"]);
						double num7 = 0.0;
						if (!string.Equals(Function.ToString(_dtItems.Rows[num6]["CostOH"]), "N"))
						{
							switch (text)
							{
							case "F":
								num7 = BaseBefore(priceValue);
								vwBreak = false;
								break;
							case "C":
								num7 = BaseAfter(priceValue, ttlCustLC);
								vwBreak = false;
								break;
							case "Q":
								num7 = CostQWV(columValue, "Quantity");
								vwBreak = false;
								break;
							case "W":
							{
								DataRow[] array = _dtItems.Select($"Weight1 is null or Weight1 = 0");
								double columValue3 = Function.ParseDouble(_dtItems.Rows[num6]["Weight1"]);
								if (array.Length != 0)
								{
									WinCommons.Error(Enums.MessageCode.OnItemTabEnterValueInWeightColumn, "");
									num5 = 0.0;
									vwBreak = true;
								}
								else
								{
									num7 = CostQWV(columValue3, "Weight1");
									vwBreak = false;
								}
								break;
							}
							case "V":
							{
								double columValue2 = Function.ParseDouble(_dtItems.Rows[num6]["Volume"]);
								if (_dtItems.Select($"Volume is null or Volume = 0").Length != 0)
								{
									WinCommons.Error(Enums.MessageCode.OnItemTabEnterValueInVolumeColumn, "");
									num5 = 0.0;
									vwBreak = true;
								}
								else
								{
									num7 = CostQWV(columValue2, "Volume");
									vwBreak = false;
								}
								break;
							}
							}
							num2 = Function.ParseDouble(_dtItems.Rows[num6]["TtlExpndLC"]);
							double num8 = 0.0;
							num8 = ((!(text == "A")) ? Math.Round(num5 * num7, 6) : Math.Round(num5 / (double)_dtItems.Rows.Count, 6));
							num = num8 + num2;
							gv_Item.FocusedRowHandle = num6;
							gv_Item.SetRowCellValue(num6, "TtlExpndLC", num);
						}
						else
						{
							gv_Item.FocusedRowHandle = num6;
							gv_Item.SetRowCellValue(num6, "TtlExpndLC", 0);
							_dtItems.AcceptChanges();
						}
					}
				}
				_dtCost.Rows[num4]["Factor"] = SetValueFactor(num5, fobValue);
			}
			if (vwBreak)
			{
				gv_Item.FocusedRowHandle = _dtItems.Rows.Count - 1;
				gv_Item.SetRowCellValue(_dtItems.Rows.Count - 1, "TtlExpndLC", 0);
			}
			_dtItems.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckedIncludeCustomer()
	{
		try
		{
			_dtItems = grd_Item.DataSourceTable;
			_dtDetails = grd_Detail.DataSourceTable;
			_dtCost = grd_Costs.DataSourceTable;
			_dtLandedCosts = grd_LandedCost.DatasourceTable();
			double num = 0.0;
			double num2 = _dtCost.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSum"]));
			double num3 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValue"]));
			double num4 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Quantity"]));
			double num5 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Volume"]));
			double num6 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Weight1"]));
			if (_dtLandedCosts != null && _dtLandedCosts.Rows.Count > 0)
			{
				currencyDoc = Function.ToString(_dtLandedCosts.Rows[0]["DocCur"]);
				rateDoc = Function.ParseDouble(_dtLandedCosts.Rows[0]["DocRate"]);
			}
			for (int num7 = 0; num7 < _dtCost.Rows.Count; num7++)
			{
				string text = Function.ToString(_dtCost.Rows[num7]["OhType"]);
				if (!(Function.ParseDouble(_dtCost.Rows[num7]["CostSum"]) > 0.0))
				{
					continue;
				}
				for (int num8 = 0; num8 < _dtItems.Rows.Count - 1; num8++)
				{
					double num9 = Function.ParseDouble(_dtItems.Rows[num8]["FobValue"]);
					double num10 = Function.ParseDouble(_dtItems.Rows[num8]["Quantity"]);
					double num11 = Function.ParseDouble(_dtItems.Rows[num8]["Volume"]);
					double num12 = Function.ParseDouble(_dtItems.Rows[num8]["Weight1"]);
					switch (text)
					{
					case "F":
					case "C":
						num = num9 + num2 / num3 * num9;
						break;
					case "Q":
						num = num9 + num2 / num4 * num10;
						break;
					case "W":
						if (_dtItems.Select($"Weight1 is null or Weight1 = 0").Length != 0)
						{
							num = num9 + num2 / num6 * num12;
						}
						break;
					case "V":
						if (_dtItems.Select($"Volume is null or Volume = 0").Length != 0)
						{
							num = num9 + num2 / num5 * num11;
						}
						break;
					}
					_dtItems.Rows[num8]["FobnLaC"] = num;
					if (!string.Equals(currencyDoc, GlobalData.MainCurrency) && rateDoc > 0.0)
					{
						_dtItems.Rows[num8]["FobnLaCFC"] = num / rateDoc;
					}
				}
			}
			_dtItems.AcceptChanges();
			gv_LandedCost.SetFocusedRowCellValue("Custom", 0);
			gv_LandedCost.SetFocusedRowCellValue("CustomFC", 0);
			gv_LandedCost.SetFocusedRowCellValue("TtlCustLC", 0);
			gv_LandedCost.SetFocusedRowCellValue("TtlCustFC", 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void WhsCostPrice(double customer, double customerValue, double cost, double expnd)
	{
		try
		{
			double num = customer + Function.ParseDouble(gv_Item.GetFocusedRowCellValue("PriceFOB")) + cost;
			gv_Item.SetFocusedRowCellValue("PriceAtWH", num);
			double num2 = customerValue + expnd;
			gv_Item.SetFocusedRowCellValue("TtlCostLC", num2);
			ItemTotal();
			double num3 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostLC"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["CostSum"] = num3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private double BaseBeforeFC(double priceValueFC)
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			grd_Item.DataSourceTable.AcceptChanges();
			double result = 0.0;
			double num = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValueFC"]));
			if (grd_Item.DataSourceTable.Rows.Count == 1)
			{
				result = 1.0;
			}
			else if (num > 0.0)
			{
				result = Math.Round(priceValueFC / num, 6);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private double BaseAfterFC(double priceValueFC, double ttlCustFC)
	{
		try
		{
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			grd_Item.DataSourceTable.AcceptChanges();
			double num = 0.0;
			double num2 = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValueFC"]));
			double num3 = grd_Item.DataSourceTable.Select($"CostOH='Y'").AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCustFC"]));
			double num4 = num2 + num3;
			double num5 = priceValueFC + ttlCustFC;
			if (grd_Item.DataSourceTable.Rows.Count == 1)
			{
				num = 1.0;
			}
			else if (num4 > 0.0)
			{
				num = Math.Round(num5 / num4, 6);
			}
			return Function.ParseDouble(num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private void ItemTotalFc(double ratedoc)
	{
		try
		{
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			_dtLandedCosts = grd_LandedCost.DataSourceTable;
			_dtItems = grd_Item.DataSourceTable;
			double num2 = 0.0;
			double num3 = Function.ParseDouble(gv_Item.GetFocusedRowCellValue("PricAtWHFC"));
			double num4 = Function.ParseDouble(gv_Item.GetFocusedRowCellValue("Quantity"));
			num2 = Math.Round(num3 * num4, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["TotalFrgn"] = num2;
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["LineTotal"] = Math.Round(num2 * ratedoc, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
			double num5 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["DocTotal"] = num5;
			double num6 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TotalFrgn"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["DocTotal"] = num6;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangingTypeCostFC()
	{
		try
		{
			_dtCost = grd_Costs.DataSourceTable;
			rateDoc = Function.ParseDouble(gv_LandedCost.GetFocusedRowCellValue("DocRate"));
			currencyItem = Function.ToString(gv_Item.GetFocusedRowCellValue("Currency"));
			currencyDoc = Function.ToString(gv_LandedCost.GetFocusedRowCellValue("DocCur"));
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (_dtItems.IsNotNull())
			{
				num4 = Function.ParseDouble(_dtItems.Rows[0]["CstmsRate"]);
			}
			for (int num5 = 0; num5 < _dtItems.Rows.Count; num5++)
			{
				_dtItems.Rows[num5]["TtlExpndFC"] = 0;
				_dtItems.Rows[num5]["CostFC"] = 0;
				_dtItems.Rows[num5]["TtlExpndLC"] = 0;
				_dtItems.Rows[num5]["Cost"] = 0;
			}
			for (int num6 = 0; num6 < _dtCost.Rows.Count; num6++)
			{
				string text = Function.ToString(_dtCost.Rows[num6]["OhType"]);
				double num7 = Function.ParseDouble(_dtCost.Rows[num6]["CostSumFC"]);
				_dtCost.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["CostSumFC"]));
				double fobValue = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["FobValueFC"]));
				if (num7 > 0.0)
				{
					for (int num8 = 0; num8 < _dtItems.Rows.Count; num8++)
					{
						double priceValueFC = Function.ParseDouble(_dtItems.Rows[num8]["FobValueFC"]);
						double ttlCustFC = Function.ParseDouble(_dtItems.Rows[num8]["TtlCustFC"]);
						double columValue = Function.ParseDouble(_dtItems.Rows[num8]["Quantity"]);
						double num9 = 0.0;
						if (!string.Equals(Function.ToString(_dtItems.Rows[num8]["CostOH"]), "N"))
						{
							switch (text)
							{
							case "F":
								num9 = BaseBeforeFC(priceValueFC);
								vwBreak = false;
								break;
							case "C":
								num9 = BaseAfterFC(priceValueFC, ttlCustFC);
								vwBreak = false;
								break;
							case "Q":
								num9 = CostQWV(columValue, "Quantity");
								vwBreak = false;
								break;
							case "W":
							{
								DataRow[] array = _dtItems.Select($"Weight1 is null or Weight1 = 0");
								double columValue3 = Function.ParseDouble(_dtItems.Rows[num8]["Weight1"]);
								if (array.Length != 0)
								{
									WinCommons.Error(Enums.MessageCode.OnItemTabEnterValueInWeightColumn, "");
									num7 = 0.0;
									vwBreak = true;
								}
								else
								{
									num9 = CostQWV(columValue3, "Weight1");
									vwBreak = false;
								}
								break;
							}
							case "V":
							{
								double columValue2 = Function.ParseDouble(_dtItems.Rows[num8]["Volume"]);
								if (_dtItems.Select($"Volume is null or Volume = 0").Length != 0)
								{
									WinCommons.Error(Enums.MessageCode.OnItemTabEnterValueInVolumeColumn, "");
									num7 = 0.0;
									vwBreak = true;
								}
								else
								{
									num9 = CostQWV(columValue2, "Volume");
									vwBreak = false;
								}
								break;
							}
							}
							num3 = Function.ParseDouble(_dtItems.Rows[num8]["TtlExpndFC"]);
							double num10 = 0.0;
							num10 = ((!(text == "A")) ? Math.Round(num7 * num9, 6) : Math.Round(num7 / (double)_dtItems.Rows.Count, 6));
							num2 = num10 + num3;
							gv_Item.FocusedRowHandle = num8;
							gv_Item.SetRowCellValue(num8, "TtlExpndFC", num2);
						}
						else
						{
							gv_Item.FocusedRowHandle = num8;
							gv_Item.SetRowCellValue(num8, "TtlExpndLC", 0);
						}
					}
				}
				_dtCost.Rows[num6]["CostSum"] = Math.Round(num7 * num4, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				_dtCost.Rows[num6]["Factor"] = SetValueFactor(num7, fobValue);
			}
			if (vwBreak)
			{
				gv_Item.FocusedRowHandle = _dtItems.Rows.Count - 1;
				gv_Item.SetRowCellValue(_dtItems.Rows.Count - 1, "TtlExpndFC", 0);
			}
			_dtItems.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void WhsCostPriceFc(double customerFc, double customerValueFc, double costFc, double expndFc, double ratedoc, double whsPrice)
	{
		try
		{
			double num = 0.0;
			int num2 = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num2 = currencyCodesImpl.Decimals;
				}
			}
			if (ratedoc > 0.0)
			{
				num = Math.Round(Function.ParseDouble(gv_Item.GetFocusedRowCellValue("PriceFOB")) / ratedoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
			}
			double num3 = customerFc + num + costFc;
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["PricAtWHFC"] = num3;
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["PriceAtWH"] = whsPrice;
			double num4 = customerValueFc + expndFc;
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["TtlCostFC"] = num4;
			_dtItems.Rows[gv_Item.GetFocusedDataSourceRowIndex()]["TtlCostLC"] = Math.Round(num4 * ratedoc, (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
			ItemTotalFc(ratedoc);
			double num5 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostFC"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["CostSumFC"] = num5;
			double num6 = _dtItems.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TtlCostLC"]));
			_dtLandedCosts.Rows[gv_LandedCost.GetFocusedDataSourceRowIndex()]["CostSum"] = num6;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ReloadDelegate()
	{
		try
		{
			DataTable dataTable = await GetLandedCostName();
			DataTable dataSourceTable = grd_Costs.DataSourceTable;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				for (int j = 0; j < dataSourceTable.Rows.Count; j++)
				{
					string text = Function.ToString(dataTable.Rows[i]["AlcCode"]);
					string value = Function.ToString(dataTable.Rows[j]["AlcCode"]);
					double num = Function.ParseDouble(dataTable.Rows[j]["CostSum"]);
					string value2 = Function.ToString(dataTable.Rows[j]["OhType"]);
					if (text.Equals(value))
					{
						dataTable.Rows[i]["CostSum"] = num;
						dataTable.Rows[i]["OhType"] = value2;
					}
					else
					{
						dataTable.Rows[i]["CostSum"] = 0;
					}
				}
			}
			grd_Costs.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetVisiable(bool flag)
	{
		try
		{
			GridColumn gridColumn = gv_Item.Columns["CustomFC"];
			GridColumn gridColumn2 = gv_Item.Columns["CostFC"];
			GridColumn gridColumn3 = gv_Item.Columns["PricAtWHFC"];
			GridColumn gridColumn4 = gv_Item.Columns["TotalFrgn"];
			GridColumn gridColumn5 = gv_Item.Columns["VarCostsFR"];
			GridColumn gridColumn6 = gv_Item.Columns["CnstCostFR"];
			GridColumn gridColumn7 = gv_Item.Columns["UsrCusomFC"];
			GridColumn gridColumn8 = gv_Item.Columns["FobValueFC"];
			GridColumn gridColumn9 = gv_Item.Columns["TtlExpndFC"];
			GridColumn gridColumn10 = gv_Item.Columns["TtlCustFC"];
			GridColumn gridColumn11 = gv_Item.Columns["TtlCostFC"];
			GridColumn gridColumn12 = gv_Item.Columns["FobnLaCFC"];
			bool flag2 = (gv_Item.Columns["CstmsRate"].Visible = !flag);
			bool flag4 = (gridColumn12.Visible = flag2);
			bool flag6 = (gridColumn11.Visible = flag4);
			bool flag8 = (gridColumn10.Visible = flag6);
			bool flag10 = (gridColumn9.Visible = flag8);
			bool flag12 = (gridColumn8.Visible = flag10);
			bool flag14 = (gridColumn7.Visible = flag12);
			bool flag16 = (gridColumn6.Visible = flag14);
			bool flag18 = (gridColumn5.Visible = flag16);
			bool flag20 = (gridColumn4.Visible = flag18);
			bool flag22 = (gridColumn3.Visible = flag20);
			bool visible = (gridColumn2.Visible = flag22);
			gridColumn.Visible = visible;
			GridColumn gridColumn13 = gv_Item.Columns["Custom"];
			GridColumn gridColumn14 = gv_Item.Columns["Cost"];
			GridColumn gridColumn15 = gv_Item.Columns["PriceAtWH"];
			GridColumn gridColumn16 = gv_Item.Columns["LineTotal"];
			GridColumn gridColumn17 = gv_Item.Columns["VarCosts"];
			GridColumn gridColumn18 = gv_Item.Columns["ConstCosts"];
			GridColumn gridColumn19 = gv_Item.Columns["UserCustom"];
			GridColumn gridColumn20 = gv_Item.Columns["FobValue"];
			GridColumn gridColumn21 = gv_Item.Columns["TtlExpndLC"];
			GridColumn gridColumn22 = gv_Item.Columns["TtlCustLC"];
			GridColumn gridColumn23 = gv_Item.Columns["TtlCostLC"];
			flag4 = (gv_Item.Columns["FobnLaC"].Visible = flag);
			flag6 = (gridColumn23.Visible = flag4);
			flag8 = (gridColumn22.Visible = flag6);
			flag10 = (gridColumn21.Visible = flag8);
			flag12 = (gridColumn20.Visible = flag10);
			flag14 = (gridColumn19.Visible = flag12);
			flag16 = (gridColumn18.Visible = flag14);
			flag18 = (gridColumn17.Visible = flag16);
			flag20 = (gridColumn16.Visible = flag18);
			flag22 = (gridColumn15.Visible = flag20);
			visible = (gridColumn14.Visible = flag22);
			gridColumn13.Visible = visible;
			gv_Costs.Columns["CostSumFC"].Visible = !flag;
			gv_Costs.Columns["CostSum"].Visible = flag;
			gv_Detail.Columns["PricAtWHFC"].Visible = !flag;
			gv_Detail.Columns["PriceAtWH"].Visible = flag;
			for (int i = 0; i < gv_Item.Columns.Count; i++)
			{
				if (gv_Item.Columns[i].Visible)
				{
					gv_Item.Columns[i].VisibleIndex = i + 100;
				}
			}
			for (int j = 0; j < gv_Costs.Columns.Count; j++)
			{
				if (gv_Costs.Columns[j].Visible)
				{
					gv_Costs.Columns[j].VisibleIndex = j + 100;
				}
			}
			for (int k = 0; k < gv_Detail.Columns.Count; k++)
			{
				if (gv_Detail.Columns[k].Visible)
				{
					gv_Detail.Columns[k].VisibleIndex = k + 100;
				}
			}
			for (int l = 0; l < _dtCost.Rows.Count; l++)
			{
				_dtCost.Rows[l]["DocCur"] = _dtLandedCosts.Rows[0]["DocCur"];
				_dtCost.Rows[l]["DocRate"] = _dtLandedCosts.Rows[0]["DocRate"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private double SetValueFactor(double costSum, double fobValue)
	{
		try
		{
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			double result = 0.0;
			if (fobValue > 0.0)
			{
				result = Math.Round(costSum / fobValue * 100.0, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseDocumentByDocNum, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Function.ToString(gv_LandedCost.GetFocusedRowCellValue("DocNum"))) != DialogResult.Yes)
			{
				return;
			}
			int num = Function.ParseInt(gv_LandedCost.GetFocusedRowCellValue("DocEntry"));
			if (num == 0)
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataLandCost, "");
				return;
			}
			HttpResult httpResult = await _httpClient.LandedCosts.Close(num);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_LandedCosts_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Purchasing.frm_LandedCosts));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice3 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice4 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice5 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice6 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice7 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice8 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice9 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice10 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice11 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice12 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice13 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice14 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice15 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice16 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice17 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice18 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice19 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice20 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice21 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice22 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice23 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice24 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice25 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice26 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice27 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice28 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice29 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice30 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice31 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice32 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice33 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice34 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice35 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice36 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice37 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice38 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice39 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice40 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice41 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice42 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice43 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_LandedCost = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_LandedCost = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn43 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_lookSeries = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_datePosting = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_dateDue = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_VendorItem = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn64 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_BrockerItem = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_lookCurrencyItem = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn62 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_dateCus = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_DetailLandedCosts = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Item = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Item = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn65 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn66 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn69 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_btnItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn70 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn71 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_QtyItem = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn72 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn76 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_value = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn73 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_lookCurrencyIte = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn74 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn75 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn77 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn78 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn79 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_ttExpLC = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn80 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn81 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn82 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn83 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_btnWhsCodeItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn84 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn85 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn86 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn87 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn88 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn92 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_NewLanded = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Costs = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Costs = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn93 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn94 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_lookOhType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn95 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_CostSum = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn96 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn97 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_checkCost = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn98 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_btnItemDetail = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn99 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn100 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_lookPriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn101 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_looExpenditure = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn39 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Num = new Apzon.Controls.Editor.TextEditEx();
		this.drBtn_CopyFrom = new DevExpress.XtraEditors.DropDownButton();
		this.txt_Vender = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_Broker = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_BrokerCost = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice39).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice41).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice42).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_LandedCost).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_LandedCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookSeries).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_datePosting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_datePosting.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateDue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateDue.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_VendorItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_BrockerItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookCurrencyItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateCus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateCus.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_DetailLandedCosts).BeginInit();
		this.xtra_DetailLandedCosts.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Item).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_QtyItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_value).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookCurrencyIte).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_ttExpLC).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnWhsCodeItem).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Costs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Costs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookOhType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_CostSum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_checkCost).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnItemDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookPriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_looExpenditure).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Num.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Vender.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Broker.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_BrokerCost).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemTextEditExPrice3.Name = "repositoryItemTextEditExPrice3";
		this.repositoryItemTextEditExPrice4.Name = "repositoryItemTextEditExPrice4";
		this.repositoryItemTextEditExPrice5.Name = "repositoryItemTextEditExPrice5";
		this.repositoryItemTextEditExPrice6.Name = "repositoryItemTextEditExPrice6";
		this.repositoryItemTextEditExPrice7.Name = "repositoryItemTextEditExPrice7";
		this.repositoryItemTextEditExPrice8.Name = "repositoryItemTextEditExPrice8";
		this.repositoryItemTextEditExPrice9.Name = "repositoryItemTextEditExPrice9";
		this.repositoryItemTextEditExPrice10.Name = "repositoryItemTextEditExPrice10";
		this.repositoryItemTextEditExPrice11.Name = "repositoryItemTextEditExPrice11";
		this.repositoryItemTextEditExPrice12.Name = "repositoryItemTextEditExPrice12";
		this.repositoryItemTextEditExPrice13.Name = "repositoryItemTextEditExPrice13";
		this.repositoryItemTextEditExPrice14.Name = "repositoryItemTextEditExPrice14";
		this.repositoryItemTextEditExPrice15.Name = "repositoryItemTextEditExPrice15";
		this.repositoryItemTextEditExPrice16.Name = "repositoryItemTextEditExPrice16";
		this.repositoryItemTextEditExPrice17.Name = "repositoryItemTextEditExPrice17";
		this.repositoryItemTextEditExPrice18.Name = "repositoryItemTextEditExPrice18";
		this.repositoryItemTextEditExPrice19.Name = "repositoryItemTextEditExPrice19";
		this.repositoryItemTextEditExPrice20.Name = "repositoryItemTextEditExPrice20";
		this.repositoryItemTextEditExPrice21.Name = "repositoryItemTextEditExPrice21";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemTextEditExPrice22.Name = "repositoryItemTextEditExPrice22";
		this.repositoryItemTextEditExPrice23.Name = "repositoryItemTextEditExPrice23";
		this.repositoryItemTextEditExPrice24.Name = "repositoryItemTextEditExPrice24";
		this.repositoryItemTextEditExPrice25.Name = "repositoryItemTextEditExPrice25";
		this.repositoryItemTextEditExPrice26.Name = "repositoryItemTextEditExPrice26";
		this.repositoryItemTextEditExPrice27.Name = "repositoryItemTextEditExPrice27";
		this.repositoryItemTextEditExPrice28.Name = "repositoryItemTextEditExPrice28";
		this.repositoryItemTextEditExPrice29.Name = "repositoryItemTextEditExPrice29";
		this.repositoryItemTextEditExPrice30.Name = "repositoryItemTextEditExPrice30";
		this.repositoryItemTextEditExPrice31.Name = "repositoryItemTextEditExPrice31";
		this.repositoryItemTextEditExPrice32.Name = "repositoryItemTextEditExPrice32";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemTextEditExPrice33.Name = "repositoryItemTextEditExPrice33";
		this.repositoryItemTextEditExPrice34.Name = "repositoryItemTextEditExPrice34";
		this.repositoryItemTextEditExPrice35.Name = "repositoryItemTextEditExPrice35";
		this.repositoryItemTextEditExPrice36.Name = "repositoryItemTextEditExPrice36";
		this.repositoryItemTextEditExPrice37.Name = "repositoryItemTextEditExPrice37";
		this.repositoryItemTextEditExPrice38.Name = "repositoryItemTextEditExPrice38";
		this.repositoryItemTextEditExPrice39.Name = "repositoryItemTextEditExPrice39";
		this.repositoryItemTextEditExPrice40.Name = "repositoryItemTextEditExPrice40";
		this.repositoryItemTextEditExPrice41.Name = "repositoryItemTextEditExPrice41";
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemTextEditExPrice42.Name = "repositoryItemTextEditExPrice42";
		this.repositoryItemTextEditExPrice43.Name = "repositoryItemTextEditExPrice43";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 9);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "From Date";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(78, 6);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 1;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(78, 42);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 3;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(2, 45);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "To Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Vendor";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(197, 9);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Vendor";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Broker";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(197, 45);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "Broker";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Number";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(390, 9);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Number";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(466, 40);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(90, 23);
		this.btn_Search.TabIndex = 10;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 71);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_LandedCost);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtra_DetailLandedCosts);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1052, 492);
		this.splitContainerControl1.SplitterPosition = 137;
		this.splitContainerControl1.TabIndex = 11;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_LandedCost.AllowPasteExcel = true;
		this.grd_LandedCost.ClipboardData = null;
		this.grd_LandedCost.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_LandedCost.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LandedCost.Location = new System.Drawing.Point(0, 0);
		this.grd_LandedCost.MainView = this.gv_LandedCost;
		this.grd_LandedCost.Name = "grd_LandedCost";
		this.grd_LandedCost.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.r_datePosting, this.r_dateDue, this.r_VendorItem, this.r_lookCurrencyItem, this.r_dateCus, this.r_lookSeries, this.r_BrockerItem });
		this.grd_LandedCost.Size = new System.Drawing.Size(1052, 137);
		this.grd_LandedCost.TabIndex = 0;
		this.grd_LandedCost.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LandedCost });
		this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.closeToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(123, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click_1);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.gv_LandedCost.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LandedCost.ColumnKey = null;
		this.gv_LandedCost.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[38]
		{
			this.gridColumn1, this.gridColumn43, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn64, this.gridColumn7, this.gridColumn8, this.gridColumn9,
			this.gridColumn14, this.gridColumn15, this.gridColumn62, this.gridColumn12, this.gridColumn10, this.gridColumn45, this.gridColumn11, this.gridColumn16, this.gridColumn17, this.gridColumn18,
			this.gridColumn6, this.gridColumn20, this.gridColumn21, this.gridColumn19, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24,
			this.gridColumnEx25, this.gridColumnEx29, this.gridColumnEx30, this.gridColumnEx31, this.gridColumnEx32, this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35
		});
		this.gv_LandedCost.DisableFormSetting = false;
		this.gv_LandedCost.DisplayException = false;
		this.gv_LandedCost.FieldCurrency = null;
		this.gv_LandedCost.FieldRate = null;
		this.gv_LandedCost.FormId = "oPurchaseLandedCost";
		this.gv_LandedCost.GridControl = this.grd_LandedCost;
		this.gv_LandedCost.IndicatorWidth = 30;
		this.gv_LandedCost.IsApzTable = false;
		this.gv_LandedCost.IsInsertNewRow = false;
		this.gv_LandedCost.IsLoadingData = false;
		this.gv_LandedCost.ItemId = "OIPF";
		this.gv_LandedCost.LstColumnAllowBatch = null;
		this.gv_LandedCost.LstColumnNotAllowBatch = null;
		this.gv_LandedCost.LstColumnNotEditable = null;
		this.gv_LandedCost.LstColumNotSetting = null;
		this.gv_LandedCost.Name = "gv_LandedCost";
		this.gv_LandedCost.OptionsView.ColumnAutoWidth = false;
		this.gv_LandedCost.OptionsView.ShowGroupPanel = false;
		this.gv_LandedCost.UseBatchMod = false;
		this.gv_LandedCost.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_LandedCost_RowCellStyle);
		this.gv_LandedCost.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_LandedCost_RowStyle);
		this.gv_LandedCost.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_LandedCost_FocusedRowObjectChanged);
		this.gv_LandedCost.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LandedCost_CellValueChanged);
		this.gv_LandedCost.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LandedCost_CellValueChanging);
		this.gridColumn1.Caption = "DocEntry";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "DocEntry";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Width = 150;
		this.gridColumn43.Caption = "Series";
		this.gridColumn43.ColumnEdit = this.r_lookSeries;
		this.gridColumn43.CurrencyDateField = null;
		this.gridColumn43.CurrencyField = null;
		this.gridColumn43.CurrencyFixed = null;
		this.gridColumn43.CurrencyFixedRate = 0.0;
		this.gridColumn43.CurrencyRateField = null;
		this.gridColumn43.FieldName = "Series";
		this.gridColumn43.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn43.Name = "gridColumn43";
		this.gridColumn43.PrimaryKey = false;
		this.gridColumn43.UnitField = null;
		this.gridColumn43.Visible = true;
		this.gridColumn43.VisibleIndex = 0;
		this.gridColumn43.Width = 61;
		this.r_lookSeries.AutoHeight = false;
		this.r_lookSeries.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_lookSeries.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Series", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_lookSeries.DisplayMember = "SeriesName";
		this.r_lookSeries.Name = "r_lookSeries";
		this.r_lookSeries.NullText = "";
		this.r_lookSeries.ValueMember = "Series";
		this.gridColumn2.Caption = "Number";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DocNum";
		this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 88;
		this.gridColumn3.Caption = "Posting Date";
		this.gridColumn3.ColumnEdit = this.r_datePosting;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn3.FieldName = "DocDate";
		this.gridColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 150;
		this.r_datePosting.AutoHeight = false;
		this.r_datePosting.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_datePosting.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_datePosting.Name = "r_datePosting";
		this.gridColumn4.Caption = "Due Date";
		this.gridColumn4.ColumnEdit = this.r_dateDue;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn4.FieldName = "DocDueDate";
		this.gridColumn4.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 150;
		this.r_dateDue.AutoHeight = false;
		this.r_dateDue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_dateDue.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_dateDue.Name = "r_dateDue";
		this.gridColumn5.Caption = "Vendor";
		this.gridColumn5.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.gridColumn5.ColumnEdit = this.r_VendorItem;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CardCode";
		this.gridColumn5.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 150;
		this.r_VendorItem.AutoHeight = false;
		this.r_VendorItem.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.r_VendorItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_VendorItem.ContainerControl = this;
		this.r_VendorItem.DisplayMember = "";
		this.r_VendorItem.DisplayMember2 = "";
		this.r_VendorItem.ListMandatoryField = null;
		this.r_VendorItem.LoadDataSourceWhenEnter = false;
		this.r_VendorItem.Name = "r_VendorItem";
		this.r_VendorItem.SeparateCharacterForMulti = ';';
		this.r_VendorItem.SeparateString = ", ";
		this.r_VendorItem.UsingAutoCompleteBox = false;
		this.r_VendorItem.UsingDataSource = true;
		this.r_VendorItem.ValidateWhenNoDataSource = false;
		this.r_VendorItem.ValueMember = "";
		this.gridColumn64.Caption = "Vendor Name";
		this.gridColumn64.CurrencyDateField = null;
		this.gridColumn64.CurrencyField = null;
		this.gridColumn64.CurrencyFixed = null;
		this.gridColumn64.CurrencyFixedRate = 0.0;
		this.gridColumn64.CurrencyRateField = null;
		this.gridColumn64.FieldName = "SuppName";
		this.gridColumn64.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn64.Name = "gridColumn64";
		this.gridColumn64.PrimaryKey = false;
		this.gridColumn64.UnitField = null;
		this.gridColumn64.Visible = true;
		this.gridColumn64.VisibleIndex = 5;
		this.gridColumn64.Width = 150;
		this.gridColumn7.Caption = "Broker";
		this.gridColumn7.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.gridColumn7.ColumnEdit = this.r_BrockerItem;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "AgentCode";
		this.gridColumn7.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn7.Width = 150;
		this.r_BrockerItem.AutoHeight = false;
		this.r_BrockerItem.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.r_BrockerItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_BrockerItem.ContainerControl = this;
		this.r_BrockerItem.DisplayMember = "";
		this.r_BrockerItem.DisplayMember2 = "";
		this.r_BrockerItem.ListMandatoryField = null;
		this.r_BrockerItem.LoadDataSourceWhenEnter = false;
		this.r_BrockerItem.Name = "r_BrockerItem";
		this.r_BrockerItem.SeparateCharacterForMulti = ';';
		this.r_BrockerItem.SeparateString = ", ";
		this.r_BrockerItem.UsingAutoCompleteBox = false;
		this.r_BrockerItem.UsingDataSource = true;
		this.r_BrockerItem.ValidateWhenNoDataSource = false;
		this.r_BrockerItem.ValueMember = "";
		this.gridColumn8.Caption = "Broker Name";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "AgentName";
		this.gridColumn8.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn8.Width = 150;
		this.gridColumn9.Caption = "DocStatus";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "DocStatus";
		this.gridColumn9.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Width = 150;
		this.gridColumn14.Caption = "Currency";
		this.gridColumn14.ColumnEdit = this.r_lookCurrencyItem;
		this.gridColumn14.CurrencyDateField = "";
		this.gridColumn14.CurrencyField = "";
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = "";
		this.gridColumn14.FieldName = "DocCur";
		this.gridColumn14.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 8;
		this.gridColumn14.Width = 150;
		this.r_lookCurrencyItem.AutoHeight = false;
		this.r_lookCurrencyItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_lookCurrencyItem.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value")
		});
		this.r_lookCurrencyItem.DisplayMember = "Code";
		this.r_lookCurrencyItem.Name = "r_lookCurrencyItem";
		this.r_lookCurrencyItem.NullText = "";
		this.r_lookCurrencyItem.ValueMember = "Code";
		this.gridColumn15.Caption = "Currency Rate";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "DocRate";
		this.gridColumn15.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 9;
		this.gridColumn15.Width = 150;
		this.gridColumn62.Caption = "Customs Date";
		this.gridColumn62.ColumnEdit = this.r_dateCus;
		this.gridColumn62.CurrencyDateField = null;
		this.gridColumn62.CurrencyField = null;
		this.gridColumn62.CurrencyFixed = null;
		this.gridColumn62.CurrencyFixedRate = 0.0;
		this.gridColumn62.CurrencyRateField = null;
		this.gridColumn62.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn62.FieldName = "CustDate";
		this.gridColumn62.Name = "gridColumn62";
		this.gridColumn62.PrimaryKey = false;
		this.gridColumn62.UnitField = null;
		this.gridColumn62.Visible = true;
		this.gridColumn62.VisibleIndex = 13;
		this.gridColumn62.Width = 150;
		this.r_dateCus.AutoHeight = false;
		this.r_dateCus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_dateCus.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_dateCus.Name = "r_dateCus";
		this.gridColumn12.Caption = "Ref.";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Ref1";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 10;
		this.gridColumn12.Width = 150;
		this.gridColumn10.Caption = "File No.";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "AgentNum";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 11;
		this.gridColumn10.Width = 150;
		this.gridColumn45.Caption = "Journal Remark";
		this.gridColumn45.CurrencyDateField = null;
		this.gridColumn45.CurrencyField = null;
		this.gridColumn45.CurrencyFixed = null;
		this.gridColumn45.CurrencyFixedRate = 0.0;
		this.gridColumn45.CurrencyRateField = null;
		this.gridColumn45.FieldName = "JdtMemo";
		this.gridColumn45.Name = "gridColumn45";
		this.gridColumn45.PrimaryKey = false;
		this.gridColumn45.UnitField = null;
		this.gridColumn45.Visible = true;
		this.gridColumn45.VisibleIndex = 12;
		this.gridColumn45.Width = 150;
		this.gridColumn11.Caption = "Description";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Descr";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 14;
		this.gridColumn11.Width = 150;
		this.gridColumn16.Caption = "Projected Customs";
		this.gridColumn16.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gridColumn16.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = "DocCur";
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = "DocRate";
		this.gridColumn16.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn16.FieldName = "ExpCustom";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Width = 150;
		this.gridColumn17.Caption = "Actual Customs";
		this.gridColumn17.ColumnEdit = this.repositoryItemTextEditExPrice3;
		this.gridColumn17.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = "DocCur";
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = "DocRate";
		this.gridColumn17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn17.FieldName = "ActCustom";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Width = 150;
		this.gridColumn18.Caption = "Total Freight Charges";
		this.gridColumn18.ColumnEdit = this.repositoryItemTextEditExPrice4;
		this.gridColumn18.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = "DocCur";
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = "DocRate";
		this.gridColumn18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn18.FieldName = "CostSum";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Width = 150;
		this.gridColumn6.Caption = "Before Tax";
		this.gridColumn6.ColumnEdit = this.repositoryItemTextEditExPrice5;
		this.gridColumn6.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = "DocCur";
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = "DocRate";
		this.gridColumn6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn6.FieldName = "BeforeVat";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Width = 150;
		this.gridColumn20.Caption = "Tax1";
		this.gridColumn20.ColumnEdit = this.repositoryItemTextEditExPrice6;
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = "DocCur";
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = "DocRate";
		this.gridColumn20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn20.FieldName = "Vat1";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Width = 150;
		this.gridColumn21.Caption = "Tax2";
		this.gridColumn21.ColumnEdit = this.repositoryItemTextEditExPrice7;
		this.gridColumn21.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = "DocCur";
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn21.FieldName = "Vat2";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Width = 150;
		this.gridColumn19.Caption = "Total";
		this.gridColumn19.ColumnEdit = this.repositoryItemTextEditExPrice8;
		this.gridColumn19.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = "DocCur";
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = "DocRate";
		this.gridColumn19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn19.FieldName = "DocTotal";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Width = 150;
		this.gridColumnEx19.Caption = "Projected Customs";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemTextEditExPrice9;
		this.gridColumnEx19.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = "DocCur";
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = "DocRate";
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx19.FieldName = "ExCustomFC";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Width = 150;
		this.gridColumnEx20.Caption = "Actual Customs";
		this.gridColumnEx20.ColumnEdit = this.repositoryItemTextEditExPrice10;
		this.gridColumnEx20.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = "DocCur";
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = "DocRate";
		this.gridColumnEx20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx20.FieldName = "AcCustomFC";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Width = 150;
		this.gridColumnEx21.Caption = "Tax 1";
		this.gridColumnEx21.ColumnEdit = this.repositoryItemTextEditExPrice11;
		this.gridColumnEx21.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = "DocCur";
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = "DocRate";
		this.gridColumnEx21.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx21.FieldName = "Vat1FC";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Width = 150;
		this.gridColumnEx22.Caption = "Tax 2";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemTextEditExPrice12;
		this.gridColumnEx22.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = "DocCur";
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = "DocRate";
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx22.FieldName = "Vat2FC";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Width = 150;
		this.gridColumnEx23.Caption = "Before Tax";
		this.gridColumnEx23.ColumnEdit = this.repositoryItemTextEditExPrice13;
		this.gridColumnEx23.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = "DocCur";
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = "DocRate";
		this.gridColumnEx23.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx23.FieldName = "BeforVatFC";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Width = 150;
		this.gridColumnEx24.Caption = "Total";
		this.gridColumnEx24.ColumnEdit = this.repositoryItemTextEditExPrice14;
		this.gridColumnEx24.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = "DocCur";
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = "DocRate";
		this.gridColumnEx24.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx24.FieldName = "DocTotalFC";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Width = 150;
		this.gridColumnEx25.Caption = "Total Freight Charges";
		this.gridColumnEx25.ColumnEdit = this.repositoryItemTextEditExPrice15;
		this.gridColumnEx25.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = "DocCur";
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = "DocRate";
		this.gridColumnEx25.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx25.FieldName = "CostSumFC";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Width = 150;
		this.gridColumnEx29.Caption = "Projected Customs";
		this.gridColumnEx29.ColumnEdit = this.repositoryItemTextEditExPrice16;
		this.gridColumnEx29.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = "DocCur";
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = "DocRate";
		this.gridColumnEx29.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx29.FieldName = "DocExpCustom";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 15;
		this.gridColumnEx29.Width = 150;
		this.gridColumnEx30.Caption = "Actual Customs";
		this.gridColumnEx30.ColumnEdit = this.repositoryItemTextEditExPrice17;
		this.gridColumnEx30.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = "DocCur";
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = "DocRate";
		this.gridColumnEx30.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx30.FieldName = "DocActCustom";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 16;
		this.gridColumnEx30.Width = 150;
		this.gridColumnEx31.Caption = "Total Freight Charges";
		this.gridColumnEx31.ColumnEdit = this.repositoryItemTextEditExPrice18;
		this.gridColumnEx31.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = "DocCur";
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = "DocRate";
		this.gridColumnEx31.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx31.FieldName = "DocCostSum";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 17;
		this.gridColumnEx31.Width = 150;
		this.gridColumnEx32.Caption = "Before Tax";
		this.gridColumnEx32.ColumnEdit = this.repositoryItemTextEditExPrice19;
		this.gridColumnEx32.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = "DocCur";
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = "DocRate";
		this.gridColumnEx32.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx32.FieldName = "DocBeforeVat";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 18;
		this.gridColumnEx32.Width = 150;
		this.gridColumnEx33.Caption = "Tax1";
		this.gridColumnEx33.ColumnEdit = this.repositoryItemTextEditExPrice20;
		this.gridColumnEx33.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = "DocCur";
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = "DocRate";
		this.gridColumnEx33.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx33.FieldName = "DocVat1";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 19;
		this.gridColumnEx33.Width = 150;
		this.gridColumnEx34.Caption = "Tax2";
		this.gridColumnEx34.ColumnEdit = this.repositoryItemTextEditExPrice21;
		this.gridColumnEx34.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = "DocCur";
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = "DocRate";
		this.gridColumnEx34.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx34.FieldName = "DocVat2";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 20;
		this.gridColumnEx34.Width = 150;
		this.gridColumnEx35.Caption = "Total";
		this.gridColumnEx35.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx35.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = "DocCur";
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = "DocRate";
		this.gridColumnEx35.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx35.FieldName = "Total";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Visible = true;
		this.gridColumnEx35.VisibleIndex = 21;
		this.gridColumnEx35.Width = 150;
		this.xtra_DetailLandedCosts.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtra_DetailLandedCosts.IsSearchControl = false;
		this.xtra_DetailLandedCosts.Location = new System.Drawing.Point(0, 0);
		this.xtra_DetailLandedCosts.Name = "xtra_DetailLandedCosts";
		this.xtra_DetailLandedCosts.SelectedTabPage = this.xtraTabPage1;
		this.xtra_DetailLandedCosts.Size = new System.Drawing.Size(1052, 345);
		this.xtra_DetailLandedCosts.TabIndex = 0;
		this.xtra_DetailLandedCosts.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3, this.xtraTabPage4 });
		this.xtraTabPage1.Controls.Add(this.grd_Item);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Items";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1050, 320);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Items";
		this.grd_Item.AllowPasteExcel = true;
		this.grd_Item.ClipboardData = null;
		this.grd_Item.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Item.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Item.Location = new System.Drawing.Point(0, 0);
		this.grd_Item.MainView = this.gv_Item;
		this.grd_Item.Name = "grd_Item";
		this.grd_Item.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.r_btnItem, this.r_QtyItem, this.r_lookCurrencyIte, this.r_btnWhsCodeItem, this.r_ttExpLC, this.txt_value, this.repositoryItemTextEditExPrice1 });
		this.grd_Item.Size = new System.Drawing.Size(1050, 320);
		this.grd_Item.TabIndex = 0;
		this.grd_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Item });
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Item.ColumnKey = null;
		this.gv_Item.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[42]
		{
			this.gridColumn65, this.gridColumn66, this.gridColumn69, this.gridColumn70, this.gridColumn71, this.gridColumn72, this.gridColumn76, this.gridColumnEx8, this.gridColumnEx13, this.gridColumn73,
			this.gridColumn74, this.gridColumn75, this.gridColumnEx1, this.gridColumn77, this.gridColumnEx11, this.gridColumn78, this.gridColumnEx2, this.gridColumn79, this.gridColumnEx9, this.gridColumn80,
			this.gridColumnEx3, this.gridColumn81, this.gridColumnEx4, this.gridColumn82, this.gridColumnEx12, this.gridColumn83, this.gridColumn84, this.gridColumn85, this.gridColumnEx5, this.gridColumn86,
			this.gridColumnEx6, this.gridColumn87, this.gridColumnEx7, this.gridColumn88, this.gridColumnEx10, this.gridColumn92, this.gridColumn40, this.gridColumn41, this.gridColumnEx14, this.gridColumnEx15,
			this.gridColumnEx16, this.gridColumnEx26
		});
		this.gv_Item.DisableFormSetting = false;
		this.gv_Item.DisplayException = false;
		this.gv_Item.FieldCurrency = null;
		this.gv_Item.FieldRate = null;
		this.gv_Item.FormId = "oPurchaseLandedCost";
		this.gv_Item.GridControl = this.grd_Item;
		this.gv_Item.IndicatorWidth = 30;
		this.gv_Item.IsApzTable = false;
		this.gv_Item.IsInsertNewRow = false;
		this.gv_Item.IsLoadingData = false;
		this.gv_Item.ItemId = "IPF1";
		this.gv_Item.LstColumnAllowBatch = null;
		this.gv_Item.LstColumnNotAllowBatch = null;
		this.gv_Item.LstColumnNotEditable = null;
		this.gv_Item.LstColumNotSetting = null;
		this.gv_Item.Name = "gv_Item";
		this.gv_Item.OptionsView.ColumnAutoWidth = false;
		this.gv_Item.OptionsView.ShowGroupPanel = false;
		this.gv_Item.UseBatchMod = false;
		this.gv_Item.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Item_RowCellStyle);
		this.gv_Item.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Item_RowStyle);
		this.gv_Item.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Item_CellValueChanged);
		this.gridColumn65.Caption = "DocEntry";
		this.gridColumn65.CurrencyDateField = null;
		this.gridColumn65.CurrencyField = null;
		this.gridColumn65.CurrencyFixed = null;
		this.gridColumn65.CurrencyFixedRate = 0.0;
		this.gridColumn65.CurrencyRateField = null;
		this.gridColumn65.FieldName = "DocEntry";
		this.gridColumn65.Name = "gridColumn65";
		this.gridColumn65.OptionsColumn.ReadOnly = true;
		this.gridColumn65.PrimaryKey = true;
		this.gridColumn65.UnitField = null;
		this.gridColumn66.Caption = "LineNum";
		this.gridColumn66.CurrencyDateField = null;
		this.gridColumn66.CurrencyField = null;
		this.gridColumn66.CurrencyFixed = null;
		this.gridColumn66.CurrencyFixedRate = 0.0;
		this.gridColumn66.CurrencyRateField = null;
		this.gridColumn66.FieldName = "LineNum";
		this.gridColumn66.Name = "gridColumn66";
		this.gridColumn66.OptionsColumn.ReadOnly = true;
		this.gridColumn66.PrimaryKey = false;
		this.gridColumn66.UnitField = null;
		this.gridColumn69.Caption = "Item No.";
		this.gridColumn69.ColumnEdit = this.r_btnItem;
		this.gridColumn69.CurrencyDateField = null;
		this.gridColumn69.CurrencyField = null;
		this.gridColumn69.CurrencyFixed = null;
		this.gridColumn69.CurrencyFixedRate = 0.0;
		this.gridColumn69.CurrencyRateField = null;
		this.gridColumn69.FieldName = "ItemCode";
		this.gridColumn69.Name = "gridColumn69";
		this.gridColumn69.OptionsColumn.ReadOnly = true;
		this.gridColumn69.PrimaryKey = false;
		this.gridColumn69.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn69.UnitField = null;
		this.gridColumn69.Visible = true;
		this.gridColumn69.VisibleIndex = 0;
		this.r_btnItem.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.r_btnItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_btnItem.Name = "r_btnItem";
		this.r_btnItem.ReadOnly = true;
		this.r_btnItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.r_btnItem_ButtonClick);
		this.gridColumn70.Caption = "Description";
		this.gridColumn70.CurrencyDateField = null;
		this.gridColumn70.CurrencyField = null;
		this.gridColumn70.CurrencyFixed = null;
		this.gridColumn70.CurrencyFixedRate = 0.0;
		this.gridColumn70.CurrencyRateField = null;
		this.gridColumn70.FieldName = "Dscription";
		this.gridColumn70.Name = "gridColumn70";
		this.gridColumn70.OptionsColumn.ReadOnly = true;
		this.gridColumn70.PrimaryKey = false;
		this.gridColumn70.UnitField = null;
		this.gridColumn70.Visible = true;
		this.gridColumn70.VisibleIndex = 1;
		this.gridColumn71.Caption = "Qty";
		this.gridColumn71.ColumnEdit = this.r_QtyItem;
		this.gridColumn71.CurrencyDateField = null;
		this.gridColumn71.CurrencyField = null;
		this.gridColumn71.CurrencyFixed = null;
		this.gridColumn71.CurrencyFixedRate = 0.0;
		this.gridColumn71.CurrencyRateField = null;
		this.gridColumn71.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumn71.FieldName = "Quantity";
		this.gridColumn71.Name = "gridColumn71";
		this.gridColumn71.OptionsColumn.ReadOnly = true;
		this.gridColumn71.PrimaryKey = false;
		this.gridColumn71.UnitField = null;
		this.gridColumn71.Visible = true;
		this.gridColumn71.VisibleIndex = 2;
		this.r_QtyItem.AutoHeight = false;
		this.r_QtyItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_QtyItem.Name = "r_QtyItem";
		this.gridColumn72.Caption = "Base Doc. Price";
		this.gridColumn72.ColumnEdit = this.repositoryItemTextEditExPrice22;
		this.gridColumn72.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn72.CurrencyDateField = null;
		this.gridColumn72.CurrencyField = "Currency";
		this.gridColumn72.CurrencyFixed = null;
		this.gridColumn72.CurrencyFixedRate = 0.0;
		this.gridColumn72.CurrencyRateField = "Rate";
		this.gridColumn72.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn72.FieldName = "PriceFOB";
		this.gridColumn72.Name = "gridColumn72";
		this.gridColumn72.OptionsColumn.ReadOnly = true;
		this.gridColumn72.PrimaryKey = false;
		this.gridColumn72.UnitField = null;
		this.gridColumn72.Visible = true;
		this.gridColumn72.VisibleIndex = 3;
		this.gridColumn72.Width = 96;
		this.gridColumn76.Caption = "Base Doc. Value";
		this.gridColumn76.ColumnEdit = this.txt_value;
		this.gridColumn76.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn76.CurrencyDateField = null;
		this.gridColumn76.CurrencyField = "DocCur";
		this.gridColumn76.CurrencyFixed = null;
		this.gridColumn76.CurrencyFixedRate = 0.0;
		this.gridColumn76.CurrencyRateField = "CstmsRate";
		this.gridColumn76.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn76.FieldName = "FobValue";
		this.gridColumn76.Name = "gridColumn76";
		this.gridColumn76.OptionsColumn.ReadOnly = true;
		this.gridColumn76.PrimaryKey = false;
		this.gridColumn76.UnitField = null;
		this.gridColumn76.Visible = true;
		this.gridColumn76.VisibleIndex = 4;
		this.gridColumn76.Width = 95;
		this.txt_value.AutoHeight = false;
		this.txt_value.Name = "txt_value";
		this.gridColumnEx8.Caption = "Base Doc Val. (FC)";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemTextEditExPrice23;
		this.gridColumnEx8.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = "DocCur";
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = "CstmsRate";
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx8.FieldName = "FobValueFC";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx13.Caption = "Customs Rate";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "CstmsRate";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumn73.Caption = "Currency";
		this.gridColumn73.ColumnEdit = this.r_lookCurrencyIte;
		this.gridColumn73.CurrencyDateField = null;
		this.gridColumn73.CurrencyField = "";
		this.gridColumn73.CurrencyFixed = null;
		this.gridColumn73.CurrencyFixedRate = 0.0;
		this.gridColumn73.CurrencyRateField = "";
		this.gridColumn73.FieldName = "Currency";
		this.gridColumn73.Name = "gridColumn73";
		this.gridColumn73.PrimaryKey = false;
		this.gridColumn73.UnitField = null;
		this.r_lookCurrencyIte.AutoHeight = false;
		this.r_lookCurrencyIte.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_lookCurrencyIte.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_lookCurrencyIte.DisplayMember = "CurrName";
		this.r_lookCurrencyIte.Name = "r_lookCurrencyIte";
		this.r_lookCurrencyIte.ValueMember = "CurrCode";
		this.gridColumn74.Caption = "Custome Rate";
		this.gridColumn74.CurrencyDateField = null;
		this.gridColumn74.CurrencyField = null;
		this.gridColumn74.CurrencyFixed = null;
		this.gridColumn74.CurrencyFixedRate = 0.0;
		this.gridColumn74.CurrencyRateField = null;
		this.gridColumn74.FieldName = "Rate";
		this.gridColumn74.Name = "gridColumn74";
		this.gridColumn74.PrimaryKey = false;
		this.gridColumn74.UnitField = null;
		this.gridColumn75.Caption = "Proj. Cust.";
		this.gridColumn75.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gridColumn75.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn75.CurrencyDateField = null;
		this.gridColumn75.CurrencyField = "DocCur";
		this.gridColumn75.CurrencyFixed = null;
		this.gridColumn75.CurrencyFixedRate = 0.0;
		this.gridColumn75.CurrencyRateField = "CstmsRate";
		this.gridColumn75.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn75.FieldName = "Custom";
		this.gridColumn75.Name = "gridColumn75";
		this.gridColumn75.PrimaryKey = false;
		this.gridColumn75.UnitField = null;
		this.gridColumn75.Visible = true;
		this.gridColumn75.VisibleIndex = 5;
		this.gridColumn75.Width = 108;
		this.repositoryItemTextEditExPrice1.AutoHeight = false;
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.gridColumnEx1.Caption = "Proj. Cust. (FC)";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemTextEditExPrice24;
		this.gridColumnEx1.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = "DocCur";
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = "CstmsRate";
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx1.FieldName = "CustomFC";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumn77.Caption = "Custom Value";
		this.gridColumn77.ColumnEdit = this.repositoryItemTextEditExPrice25;
		this.gridColumn77.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn77.CurrencyDateField = null;
		this.gridColumn77.CurrencyField = "DocCur";
		this.gridColumn77.CurrencyFixed = null;
		this.gridColumn77.CurrencyFixedRate = 0.0;
		this.gridColumn77.CurrencyRateField = "CstmsRate";
		this.gridColumn77.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn77.FieldName = "TtlCustLC";
		this.gridColumn77.Name = "gridColumn77";
		this.gridColumn77.PrimaryKey = false;
		this.gridColumn77.UnitField = null;
		this.gridColumn77.Visible = true;
		this.gridColumn77.VisibleIndex = 6;
		this.gridColumn77.Width = 92;
		this.gridColumnEx11.Caption = "Customs Val. (FC)";
		this.gridColumnEx11.ColumnEdit = this.repositoryItemTextEditExPrice26;
		this.gridColumnEx11.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = "DocCur";
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = "CstmsRate";
		this.gridColumnEx11.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx11.FieldName = "TtlCustFC";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumn78.Caption = "Expenditure";
		this.gridColumn78.ColumnEdit = this.repositoryItemTextEditExPrice27;
		this.gridColumn78.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn78.CurrencyDateField = null;
		this.gridColumn78.CurrencyField = "DocCur";
		this.gridColumn78.CurrencyFixed = null;
		this.gridColumn78.CurrencyFixedRate = 0.0;
		this.gridColumn78.CurrencyRateField = "CstmsRate";
		this.gridColumn78.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn78.FieldName = "Cost";
		this.gridColumn78.Name = "gridColumn78";
		this.gridColumn78.PrimaryKey = false;
		this.gridColumn78.UnitField = null;
		this.gridColumn78.Visible = true;
		this.gridColumn78.VisibleIndex = 7;
		this.gridColumnEx2.Caption = "Freight (FC)";
		this.gridColumnEx2.ColumnEdit = this.repositoryItemTextEditExPrice28;
		this.gridColumnEx2.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = "DocCur";
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = "CstmsRate";
		this.gridColumnEx2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx2.FieldName = "CostFC";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumn79.Caption = "Alloc. Costs Val.";
		this.gridColumn79.ColumnEdit = this.r_ttExpLC;
		this.gridColumn79.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn79.CurrencyDateField = null;
		this.gridColumn79.CurrencyField = "DocCur";
		this.gridColumn79.CurrencyFixed = null;
		this.gridColumn79.CurrencyFixedRate = 0.0;
		this.gridColumn79.CurrencyRateField = "CstmsRate";
		this.gridColumn79.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn79.FieldName = "TtlExpndLC";
		this.gridColumn79.Name = "gridColumn79";
		this.gridColumn79.PrimaryKey = false;
		this.gridColumn79.UnitField = null;
		this.gridColumn79.Visible = true;
		this.gridColumn79.VisibleIndex = 8;
		this.r_ttExpLC.AutoHeight = false;
		this.r_ttExpLC.Name = "r_ttExpLC";
		this.gridColumnEx9.Caption = "Alloc. Costs Val. (FC)";
		this.gridColumnEx9.ColumnEdit = this.repositoryItemTextEditExPrice29;
		this.gridColumnEx9.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = "DocCur";
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = "CstmsRate";
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx9.FieldName = "TtlExpndFC";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumn80.Caption = "Whse Price";
		this.gridColumn80.ColumnEdit = this.repositoryItemTextEditExPrice30;
		this.gridColumn80.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn80.CurrencyDateField = null;
		this.gridColumn80.CurrencyField = "DocCur";
		this.gridColumn80.CurrencyFixed = null;
		this.gridColumn80.CurrencyFixedRate = 0.0;
		this.gridColumn80.CurrencyRateField = "CstmsRate";
		this.gridColumn80.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn80.FieldName = "PriceAtWH";
		this.gridColumn80.Name = "gridColumn80";
		this.gridColumn80.OptionsColumn.ReadOnly = true;
		this.gridColumn80.PrimaryKey = false;
		this.gridColumn80.UnboundExpression = "[PriceFOB] + [UserCustom] + [Cost]";
		this.gridColumn80.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.gridColumn80.UnitField = null;
		this.gridColumn80.Visible = true;
		this.gridColumn80.VisibleIndex = 9;
		this.gridColumnEx3.Caption = "Whse Price (FC)";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemTextEditExPrice31;
		this.gridColumnEx3.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = "DocCur";
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = "CstmsRate";
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx3.FieldName = "PricAtWHFC";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumn81.Caption = "Total";
		this.gridColumn81.ColumnEdit = this.repositoryItemTextEditExPrice32;
		this.gridColumn81.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn81.CurrencyDateField = null;
		this.gridColumn81.CurrencyField = "DocCur";
		this.gridColumn81.CurrencyFixed = null;
		this.gridColumn81.CurrencyFixedRate = 0.0;
		this.gridColumn81.CurrencyRateField = "CstmsRate";
		this.gridColumn81.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn81.FieldName = "LineTotal";
		this.gridColumn81.Name = "gridColumn81";
		this.gridColumn81.OptionsColumn.ReadOnly = true;
		this.gridColumn81.PrimaryKey = false;
		this.gridColumn81.UnitField = null;
		this.gridColumn81.Visible = true;
		this.gridColumn81.VisibleIndex = 10;
		this.gridColumnEx4.Caption = "Total (FC)";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx4.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = "DocCur";
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = "CstmsRate";
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx4.FieldName = "TotalFrgn";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumn82.Caption = "Total Cost";
		this.gridColumn82.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumn82.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn82.CurrencyDateField = null;
		this.gridColumn82.CurrencyField = "DocCur";
		this.gridColumn82.CurrencyFixed = null;
		this.gridColumn82.CurrencyFixedRate = 0.0;
		this.gridColumn82.CurrencyRateField = "CstmsRate";
		this.gridColumn82.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumn82.FieldName = "TtlCostLC";
		this.gridColumn82.Name = "gridColumn82";
		this.gridColumn82.OptionsColumn.ReadOnly = true;
		this.gridColumn82.PrimaryKey = false;
		this.gridColumn82.UnitField = null;
		this.gridColumn82.Visible = true;
		this.gridColumn82.VisibleIndex = 11;
		this.gridColumnEx12.Caption = "Total Costs (FC)";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx12.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = "DocCur";
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = "CstmsRate";
		this.gridColumnEx12.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx12.FieldName = "TtlCostFC";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumn83.Caption = "Warehouse";
		this.gridColumn83.ColumnEdit = this.r_btnWhsCodeItem;
		this.gridColumn83.CurrencyDateField = null;
		this.gridColumn83.CurrencyField = null;
		this.gridColumn83.CurrencyFixed = null;
		this.gridColumn83.CurrencyFixedRate = 0.0;
		this.gridColumn83.CurrencyRateField = null;
		this.gridColumn83.FieldName = "WhsCode";
		this.gridColumn83.Name = "gridColumn83";
		this.gridColumn83.OptionsColumn.ReadOnly = true;
		this.gridColumn83.PrimaryKey = false;
		this.gridColumn83.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn83.UnitField = null;
		this.gridColumn83.Visible = true;
		this.gridColumn83.VisibleIndex = 12;
		this.r_btnWhsCodeItem.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.r_btnWhsCodeItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_btnWhsCodeItem.Name = "r_btnWhsCodeItem";
		this.r_btnWhsCodeItem.ReadOnly = true;
		this.r_btnWhsCodeItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.r_btnWhsCodeItem_ButtonClick);
		this.gridColumn84.Caption = "Release No.";
		this.gridColumn84.ColumnEdit = this.repositoryItemTextEditExPrice33;
		this.gridColumn84.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn84.CurrencyDateField = null;
		this.gridColumn84.CurrencyField = "DocCur";
		this.gridColumn84.CurrencyFixed = null;
		this.gridColumn84.CurrencyFixedRate = 0.0;
		this.gridColumn84.CurrencyRateField = "CstmsRate";
		this.gridColumn84.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn84.FieldName = "ReleaseNum";
		this.gridColumn84.Name = "gridColumn84";
		this.gridColumn84.OptionsColumn.ReadOnly = true;
		this.gridColumn84.PrimaryKey = false;
		this.gridColumn84.UnitField = null;
		this.gridColumn84.Visible = true;
		this.gridColumn84.VisibleIndex = 13;
		this.gridColumn85.Caption = "Var. Costs";
		this.gridColumn85.ColumnEdit = this.repositoryItemTextEditExPrice34;
		this.gridColumn85.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn85.CurrencyDateField = null;
		this.gridColumn85.CurrencyField = "DocCur";
		this.gridColumn85.CurrencyFixed = null;
		this.gridColumn85.CurrencyFixedRate = 0.0;
		this.gridColumn85.CurrencyRateField = "CstmsRate";
		this.gridColumn85.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn85.FieldName = "VarCosts";
		this.gridColumn85.Name = "gridColumn85";
		this.gridColumn85.OptionsColumn.ReadOnly = true;
		this.gridColumn85.PrimaryKey = false;
		this.gridColumn85.UnitField = null;
		this.gridColumn85.Visible = true;
		this.gridColumn85.VisibleIndex = 14;
		this.gridColumnEx5.Caption = "Var. Costs (FC)";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemTextEditExPrice35;
		this.gridColumnEx5.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = "DocCur";
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = "CstmsRate";
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx5.FieldName = "VarCostsFR";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumn86.Caption = "Const. Costs";
		this.gridColumn86.ColumnEdit = this.repositoryItemTextEditExPrice36;
		this.gridColumn86.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn86.CurrencyDateField = null;
		this.gridColumn86.CurrencyField = "DocCur";
		this.gridColumn86.CurrencyFixed = null;
		this.gridColumn86.CurrencyFixedRate = 0.0;
		this.gridColumn86.CurrencyRateField = "CstmsRate";
		this.gridColumn86.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn86.FieldName = "ConstCosts";
		this.gridColumn86.Name = "gridColumn86";
		this.gridColumn86.OptionsColumn.ReadOnly = true;
		this.gridColumn86.PrimaryKey = false;
		this.gridColumn86.UnitField = null;
		this.gridColumn86.Visible = true;
		this.gridColumn86.VisibleIndex = 15;
		this.gridColumnEx6.Caption = "Const. Costs (FC)";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemTextEditExPrice37;
		this.gridColumnEx6.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = "DocCur";
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = "CstmsRate";
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx6.FieldName = "CnstCostFR";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumn87.Caption = "Expected Customs";
		this.gridColumn87.ColumnEdit = this.repositoryItemTextEditExPrice38;
		this.gridColumn87.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn87.CurrencyDateField = null;
		this.gridColumn87.CurrencyField = "DocCur";
		this.gridColumn87.CurrencyFixed = null;
		this.gridColumn87.CurrencyFixedRate = 0.0;
		this.gridColumn87.CurrencyRateField = "CstmsRate";
		this.gridColumn87.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn87.FieldName = "UserCustom";
		this.gridColumn87.Name = "gridColumn87";
		this.gridColumn87.OptionsColumn.ReadOnly = true;
		this.gridColumn87.PrimaryKey = false;
		this.gridColumn87.UnitField = null;
		this.gridColumn87.Visible = true;
		this.gridColumn87.VisibleIndex = 16;
		this.gridColumnEx7.Caption = "Expected Customs (FC)";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemTextEditExPrice39;
		this.gridColumnEx7.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = "DocCur";
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = "CstmsRate";
		this.gridColumnEx7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx7.FieldName = "UsrCusomFC";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumn88.Caption = "FOB and Included Costs";
		this.gridColumn88.ColumnEdit = this.repositoryItemTextEditExPrice40;
		this.gridColumn88.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn88.CurrencyDateField = null;
		this.gridColumn88.CurrencyField = "DocCur";
		this.gridColumn88.CurrencyFixed = null;
		this.gridColumn88.CurrencyFixedRate = 0.0;
		this.gridColumn88.CurrencyRateField = "CstmsRate";
		this.gridColumn88.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn88.FieldName = "FobnLaC";
		this.gridColumn88.Name = "gridColumn88";
		this.gridColumn88.OptionsColumn.ReadOnly = true;
		this.gridColumn88.PrimaryKey = false;
		this.gridColumn88.UnitField = null;
		this.gridColumn88.Visible = true;
		this.gridColumn88.VisibleIndex = 17;
		this.gridColumnEx10.Caption = "FOB and Included Costs (FC)";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemTextEditExPrice41;
		this.gridColumnEx10.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = "DocCur";
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = "CstmsRate";
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx10.FieldName = "FobnLaCFC";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumn92.Caption = "Uom Code";
		this.gridColumn92.CurrencyDateField = null;
		this.gridColumn92.CurrencyField = null;
		this.gridColumn92.CurrencyFixed = null;
		this.gridColumn92.CurrencyFixedRate = 0.0;
		this.gridColumn92.CurrencyRateField = null;
		this.gridColumn92.FieldName = "UomCode";
		this.gridColumn92.Name = "gridColumn92";
		this.gridColumn92.OptionsColumn.ReadOnly = true;
		this.gridColumn92.PrimaryKey = false;
		this.gridColumn92.UnitField = null;
		this.gridColumn92.Visible = true;
		this.gridColumn92.VisibleIndex = 18;
		this.gridColumn40.Caption = "Volume";
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "Volume";
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.UnitField = null;
		this.gridColumn41.Caption = "Weight1";
		this.gridColumn41.CurrencyDateField = null;
		this.gridColumn41.CurrencyField = null;
		this.gridColumn41.CurrencyFixed = null;
		this.gridColumn41.CurrencyFixedRate = 0.0;
		this.gridColumn41.CurrencyRateField = null;
		this.gridColumn41.FieldName = "Weight1";
		this.gridColumn41.Name = "gridColumn41";
		this.gridColumn41.PrimaryKey = false;
		this.gridColumn41.UnitField = null;
		this.gridColumnEx14.Caption = "Base Document";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "BaseEntry";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx15.Caption = "Base Doc. Type";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "BaseType";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx16.Caption = "Base Doc. Ref.";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Reference";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx26.Caption = "DocCur";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = "";
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = "";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.xtraTabPage2.Controls.Add(this.btn_NewLanded);
		this.xtraTabPage2.Controls.Add(this.simpleButton1);
		this.xtraTabPage2.Controls.Add(this.grd_Costs);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Costs";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(1050, 320);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Costs";
		this.btn_NewLanded.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewLanded.EditControl = null;
		this.btn_NewLanded.EditValue = "New Landed Costs";
		this.btn_NewLanded.FieldName = null;
		this.btn_NewLanded.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewLanded.ImageOptions.Image");
		this.btn_NewLanded.IsSearchControl = false;
		this.btn_NewLanded.Location = new System.Drawing.Point(927, 291);
		this.btn_NewLanded.Name = "btn_NewLanded";
		this.btn_NewLanded.ReadOnly = false;
		this.btn_NewLanded.SettingInVisible = false;
		this.btn_NewLanded.SettingReadonly = false;
		this.btn_NewLanded.Size = new System.Drawing.Size(116, 23);
		this.btn_NewLanded.TabIndex = 13;
		this.btn_NewLanded.Text = "New Landed Costs";
		this.btn_NewLanded.Click += new System.EventHandler(this.btn_NewLanded_Click);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Clear Table";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(960, 262);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(83, 23);
		this.simpleButton1.TabIndex = 1;
		this.simpleButton1.Text = "Clear Table";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.grd_Costs.AllowPasteExcel = false;
		this.grd_Costs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Costs.ClipboardData = null;
		this.grd_Costs.Location = new System.Drawing.Point(-1, 0);
		this.grd_Costs.MainView = this.gv_Costs;
		this.grd_Costs.Name = "grd_Costs";
		this.grd_Costs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.r_lookOhType, this.r_checkCost, this.r_CostSum, this.r_BrokerCost });
		this.grd_Costs.Size = new System.Drawing.Size(922, 317);
		this.grd_Costs.TabIndex = 0;
		this.grd_Costs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Costs });
		this.gv_Costs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Costs.ColumnKey = null;
		this.gv_Costs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumn35, this.gridColumn93, this.gridColumn94, this.gridColumn95, this.gridColumnEx17, this.gridColumn96, this.gridColumn97, this.gridColumn37, this.gridColumnEx27, this.gridColumnEx28,
			this.gridColumnEx36, this.gridColumnEx37
		});
		this.gv_Costs.DisableFormSetting = false;
		this.gv_Costs.DisplayException = false;
		this.gv_Costs.FieldCurrency = null;
		this.gv_Costs.FieldRate = null;
		this.gv_Costs.FormId = "oPurchaseLandedCost";
		this.gv_Costs.GridControl = this.grd_Costs;
		this.gv_Costs.IndicatorWidth = 30;
		this.gv_Costs.IsApzTable = false;
		this.gv_Costs.IsInsertNewRow = false;
		this.gv_Costs.IsLoadingData = false;
		this.gv_Costs.ItemId = "IPF2";
		this.gv_Costs.LstColumnAllowBatch = null;
		this.gv_Costs.LstColumnNotAllowBatch = null;
		this.gv_Costs.LstColumnNotEditable = null;
		this.gv_Costs.LstColumNotSetting = null;
		this.gv_Costs.Name = "gv_Costs";
		this.gv_Costs.OptionsView.ShowGroupPanel = false;
		this.gv_Costs.UseBatchMod = false;
		this.gv_Costs.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Costs_RowCellStyle);
		this.gv_Costs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Costs_RowStyle);
		this.gv_Costs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Costs_CellValueChanged);
		this.gridColumn35.Caption = "DocEntry";
		this.gridColumn35.CurrencyDateField = null;
		this.gridColumn35.CurrencyField = null;
		this.gridColumn35.CurrencyFixed = null;
		this.gridColumn35.CurrencyFixedRate = 0.0;
		this.gridColumn35.CurrencyRateField = null;
		this.gridColumn35.FieldName = "DocEntry";
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.OptionsColumn.ReadOnly = true;
		this.gridColumn35.PrimaryKey = false;
		this.gridColumn35.UnitField = null;
		this.gridColumn93.Caption = "Landed Costs";
		this.gridColumn93.CurrencyDateField = null;
		this.gridColumn93.CurrencyField = null;
		this.gridColumn93.CurrencyFixed = null;
		this.gridColumn93.CurrencyFixedRate = 0.0;
		this.gridColumn93.CurrencyRateField = null;
		this.gridColumn93.FieldName = "AlcName";
		this.gridColumn93.Name = "gridColumn93";
		this.gridColumn93.OptionsColumn.ReadOnly = true;
		this.gridColumn93.PrimaryKey = false;
		this.gridColumn93.UnitField = null;
		this.gridColumn93.Visible = true;
		this.gridColumn93.VisibleIndex = 0;
		this.gridColumn93.Width = 260;
		this.gridColumn94.Caption = "Allocation By";
		this.gridColumn94.ColumnEdit = this.r_lookOhType;
		this.gridColumn94.CurrencyDateField = null;
		this.gridColumn94.CurrencyField = null;
		this.gridColumn94.CurrencyFixed = null;
		this.gridColumn94.CurrencyFixedRate = 0.0;
		this.gridColumn94.CurrencyRateField = null;
		this.gridColumn94.FieldName = "OhType";
		this.gridColumn94.Name = "gridColumn94";
		this.gridColumn94.PrimaryKey = false;
		this.gridColumn94.UnitField = null;
		this.gridColumn94.Visible = true;
		this.gridColumn94.VisibleIndex = 1;
		this.gridColumn94.Width = 405;
		this.r_lookOhType.AutoHeight = false;
		this.r_lookOhType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_lookOhType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_lookOhType.DisplayMember = "Name";
		this.r_lookOhType.Name = "r_lookOhType";
		this.r_lookOhType.NullText = "";
		this.r_lookOhType.ValueMember = "Code";
		this.gridColumn95.Caption = "Amount";
		this.gridColumn95.ColumnEdit = this.r_CostSum;
		this.gridColumn95.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn95.CurrencyDateField = null;
		this.gridColumn95.CurrencyField = "Currency";
		this.gridColumn95.CurrencyFixed = null;
		this.gridColumn95.CurrencyFixedRate = 0.0;
		this.gridColumn95.CurrencyRateField = "Rate";
		this.gridColumn95.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumn95.FieldName = "CostSum";
		this.gridColumn95.Name = "gridColumn95";
		this.gridColumn95.PrimaryKey = false;
		this.gridColumn95.UnitField = null;
		this.gridColumn95.Visible = true;
		this.gridColumn95.VisibleIndex = 2;
		this.gridColumn95.Width = 223;
		this.r_CostSum.AutoHeight = false;
		this.r_CostSum.Name = "r_CostSum";
		this.gridColumnEx17.Caption = "Amount";
		this.gridColumnEx17.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx17.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = "DocCur";
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = "DocRate";
		this.gridColumnEx17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx17.FieldName = "CostSumFC";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 3;
		this.gridColumnEx17.Width = 223;
		this.gridColumn96.Caption = "Factor";
		this.gridColumn96.CurrencyDateField = null;
		this.gridColumn96.CurrencyField = null;
		this.gridColumn96.CurrencyFixed = null;
		this.gridColumn96.CurrencyFixedRate = 0.0;
		this.gridColumn96.CurrencyRateField = null;
		this.gridColumn96.FieldName = "Factor";
		this.gridColumn96.Name = "gridColumn96";
		this.gridColumn96.OptionsColumn.ReadOnly = true;
		this.gridColumn96.PrimaryKey = false;
		this.gridColumn96.UnitField = null;
		this.gridColumn96.Visible = true;
		this.gridColumn96.VisibleIndex = 4;
		this.gridColumn96.Width = 223;
		this.gridColumn97.Caption = "Include for Customs";
		this.gridColumn97.ColumnEdit = this.r_checkCost;
		this.gridColumn97.CurrencyDateField = null;
		this.gridColumn97.CurrencyField = null;
		this.gridColumn97.CurrencyFixed = null;
		this.gridColumn97.CurrencyFixedRate = 0.0;
		this.gridColumn97.CurrencyRateField = null;
		this.gridColumn97.FieldName = "InCustCalc";
		this.gridColumn97.Name = "gridColumn97";
		this.gridColumn97.PrimaryKey = false;
		this.gridColumn97.UnitField = null;
		this.gridColumn97.Visible = true;
		this.gridColumn97.VisibleIndex = 5;
		this.gridColumn97.Width = 229;
		this.r_checkCost.AutoHeight = false;
		this.r_checkCost.Name = "r_checkCost";
		this.r_checkCost.ValueChecked = "Y";
		this.r_checkCost.ValueGrayed = "N";
		this.r_checkCost.ValueUnchecked = "N";
		this.gridColumn37.Caption = "AlcCode";
		this.gridColumn37.CurrencyDateField = null;
		this.gridColumn37.CurrencyField = null;
		this.gridColumn37.CurrencyFixed = null;
		this.gridColumn37.CurrencyFixedRate = 0.0;
		this.gridColumn37.CurrencyRateField = null;
		this.gridColumn37.FieldName = "AlcCode";
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.PrimaryKey = false;
		this.gridColumn37.UnitField = null;
		this.gridColumnEx27.Caption = "DocCur";
		this.gridColumnEx27.CurrencyDateField = "DocCur";
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = "DocRate";
		this.gridColumnEx27.FieldName = "DocCur";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx28.Caption = "DocRate";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "DocRate";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.xtraTabPage3.Controls.Add(this.grd_Detail);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "Details";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(1050, 320);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Details";
		this.grd_Detail.AllowPasteExcel = false;
		this.grd_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.Location = new System.Drawing.Point(-1, 0);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.r_btnItemDetail, this.r_lookPriceList, this.r_looExpenditure });
		this.grd_Detail.Size = new System.Drawing.Size(1066, 317);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn98, this.gridColumn99, this.gridColumnEx18, this.gridColumn100, this.gridColumn101, this.gridColumn38, this.gridColumn39 });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oPurchaseLandedCost";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IndicatorWidth = 30;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.ItemId = "IPF1";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Detail_RowCellStyle);
		this.gv_Detail.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Detail_RowStyle);
		this.gv_Detail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanged);
		this.gridColumn98.Caption = "Item";
		this.gridColumn98.ColumnEdit = this.r_btnItemDetail;
		this.gridColumn98.CurrencyDateField = null;
		this.gridColumn98.CurrencyField = null;
		this.gridColumn98.CurrencyFixed = null;
		this.gridColumn98.CurrencyFixedRate = 0.0;
		this.gridColumn98.CurrencyRateField = null;
		this.gridColumn98.FieldName = "ItemCode";
		this.gridColumn98.Name = "gridColumn98";
		this.gridColumn98.OptionsColumn.ReadOnly = true;
		this.gridColumn98.PrimaryKey = false;
		this.gridColumn98.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn98.UnitField = null;
		this.gridColumn98.Visible = true;
		this.gridColumn98.VisibleIndex = 0;
		this.r_btnItemDetail.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.r_btnItemDetail.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_btnItemDetail.Name = "r_btnItemDetail";
		this.r_btnItemDetail.ReadOnly = true;
		this.r_btnItemDetail.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.r_btnItemDetail_ButtonClick);
		this.gridColumn99.Caption = "Whse Price";
		this.gridColumn99.ColumnEdit = this.repositoryItemTextEditExPrice42;
		this.gridColumn99.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn99.CurrencyDateField = null;
		this.gridColumn99.CurrencyField = "DocCur";
		this.gridColumn99.CurrencyFixed = null;
		this.gridColumn99.CurrencyFixedRate = 0.0;
		this.gridColumn99.CurrencyRateField = "CstmsRate";
		this.gridColumn99.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn99.FieldName = "PriceAtWH";
		this.gridColumn99.Name = "gridColumn99";
		this.gridColumn99.OptionsColumn.ReadOnly = true;
		this.gridColumn99.PrimaryKey = false;
		this.gridColumn99.UnitField = null;
		this.gridColumn99.Visible = true;
		this.gridColumn99.VisibleIndex = 1;
		this.gridColumnEx18.Caption = "Whse Price";
		this.gridColumnEx18.ColumnEdit = this.repositoryItemTextEditExPrice43;
		this.gridColumnEx18.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = "DocCur";
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = "CstmsRate";
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx18.FieldName = "PricAtWHFC";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 2;
		this.gridColumn100.Caption = "Price List";
		this.gridColumn100.ColumnEdit = this.r_lookPriceList;
		this.gridColumn100.CurrencyDateField = null;
		this.gridColumn100.CurrencyField = null;
		this.gridColumn100.CurrencyFixed = null;
		this.gridColumn100.CurrencyFixedRate = 0.0;
		this.gridColumn100.CurrencyRateField = "";
		this.gridColumn100.FieldName = "PriceList";
		this.gridColumn100.Name = "gridColumn100";
		this.gridColumn100.PrimaryKey = false;
		this.gridColumn100.UnitField = null;
		this.gridColumn100.Visible = true;
		this.gridColumn100.VisibleIndex = 3;
		this.r_lookPriceList.AutoHeight = false;
		this.r_lookPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_lookPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListNum", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_lookPriceList.DisplayMember = "ListName";
		this.r_lookPriceList.Name = "r_lookPriceList";
		this.r_lookPriceList.NullText = "";
		this.r_lookPriceList.ValueMember = "ListNum";
		this.gridColumn101.Caption = "Expenditure";
		this.gridColumn101.ColumnEdit = this.r_looExpenditure;
		this.gridColumn101.CurrencyDateField = null;
		this.gridColumn101.CurrencyField = null;
		this.gridColumn101.CurrencyFixed = null;
		this.gridColumn101.CurrencyFixedRate = 0.0;
		this.gridColumn101.CurrencyRateField = null;
		this.gridColumn101.FieldName = "CostOH";
		this.gridColumn101.Name = "gridColumn101";
		this.gridColumn101.PrimaryKey = false;
		this.gridColumn101.UnitField = null;
		this.gridColumn101.Visible = true;
		this.gridColumn101.VisibleIndex = 4;
		this.r_looExpenditure.AutoHeight = false;
		this.r_looExpenditure.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_looExpenditure.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_looExpenditure.DisplayMember = "Name";
		this.r_looExpenditure.Name = "r_looExpenditure";
		this.r_looExpenditure.NullText = "";
		this.r_looExpenditure.ValueMember = "Code";
		this.gridColumn38.Caption = "DocEntry";
		this.gridColumn38.CurrencyDateField = null;
		this.gridColumn38.CurrencyField = null;
		this.gridColumn38.CurrencyFixed = null;
		this.gridColumn38.CurrencyFixedRate = 0.0;
		this.gridColumn38.CurrencyRateField = null;
		this.gridColumn38.FieldName = "DocEntry";
		this.gridColumn38.Name = "gridColumn38";
		this.gridColumn38.OptionsColumn.ReadOnly = true;
		this.gridColumn38.PrimaryKey = false;
		this.gridColumn38.UnitField = null;
		this.gridColumn39.Caption = "LineNum";
		this.gridColumn39.CurrencyDateField = null;
		this.gridColumn39.CurrencyField = null;
		this.gridColumn39.CurrencyFixed = null;
		this.gridColumn39.CurrencyFixedRate = 0.0;
		this.gridColumn39.CurrencyRateField = null;
		this.gridColumn39.FieldName = "LineNum";
		this.gridColumn39.Name = "gridColumn39";
		this.gridColumn39.OptionsColumn.ReadOnly = true;
		this.gridColumn39.PrimaryKey = false;
		this.gridColumn39.UnitField = null;
		this.xtraTabPage4.Controls.Add(this.ucAttachment1);
		this.xtraTabPage4.EditControl = null;
		this.xtraTabPage4.EditValue = "Attachment";
		this.xtraTabPage4.FieldName = null;
		this.xtraTabPage4.IsSearchControl = false;
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.ReadOnly = false;
		this.xtraTabPage4.SettingInVisible = false;
		this.xtraTabPage4.SettingReadonly = false;
		this.xtraTabPage4.Size = new System.Drawing.Size(1050, 320);
		this.xtraTabPage4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage4.Text = "Attachment";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oPurchaseLandedCost";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(1050, 320);
		this.ucAttachment1.TabIndex = 0;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 569);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 12;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 569);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_Num.EditControl = null;
		this.txt_Num.FieldName = null;
		this.txt_Num.IsSearchControl = false;
		this.txt_Num.Location = new System.Drawing.Point(466, 6);
		this.txt_Num.Name = "txt_Num";
		this.txt_Num.SettingInVisible = false;
		this.txt_Num.SettingReadonly = false;
		this.txt_Num.Size = new System.Drawing.Size(100, 20);
		this.txt_Num.TabIndex = 15;
		this.drBtn_CopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drBtn_CopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drBtn_CopyFrom.ImageOptions.Image");
		this.drBtn_CopyFrom.Location = new System.Drawing.Point(951, 569);
		this.drBtn_CopyFrom.Name = "drBtn_CopyFrom";
		this.drBtn_CopyFrom.Size = new System.Drawing.Size(98, 23);
		this.drBtn_CopyFrom.TabIndex = 18;
		this.drBtn_CopyFrom.Text = "Copy From";
		this.txt_Vender.EditControl = null;
		this.txt_Vender.FieldName = null;
		this.txt_Vender.IsSearchControl = false;
		this.txt_Vender.Location = new System.Drawing.Point(273, 6);
		this.txt_Vender.Name = "txt_Vender";
		this.txt_Vender.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.txt_Vender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Vender.Properties.ContainerControl = null;
		this.txt_Vender.Properties.DisplayMember = "";
		this.txt_Vender.Properties.DisplayMember2 = "";
		this.txt_Vender.Properties.ListMandatoryField = null;
		this.txt_Vender.Properties.LoadDataSourceWhenEnter = false;
		this.txt_Vender.Properties.SeparateCharacterForMulti = ';';
		this.txt_Vender.Properties.SeparateString = ", ";
		this.txt_Vender.Properties.UsingAutoCompleteBox = false;
		this.txt_Vender.Properties.UsingDataSource = true;
		this.txt_Vender.Properties.ValidateWhenNoDataSource = false;
		this.txt_Vender.Properties.ValueMember = "";
		this.txt_Vender.SettingInVisible = false;
		this.txt_Vender.SettingReadonly = false;
		this.txt_Vender.Size = new System.Drawing.Size(100, 20);
		this.txt_Vender.TabIndex = 19;
		this.txt_Broker.EditControl = null;
		this.txt_Broker.FieldName = null;
		this.txt_Broker.IsSearchControl = false;
		this.txt_Broker.Location = new System.Drawing.Point(273, 42);
		this.txt_Broker.Name = "txt_Broker";
		this.txt_Broker.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.txt_Broker.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Broker.Properties.ContainerControl = null;
		this.txt_Broker.Properties.DisplayMember = "";
		this.txt_Broker.Properties.DisplayMember2 = "";
		this.txt_Broker.Properties.ListMandatoryField = null;
		this.txt_Broker.Properties.LoadDataSourceWhenEnter = false;
		this.txt_Broker.Properties.SeparateCharacterForMulti = ';';
		this.txt_Broker.Properties.SeparateString = ", ";
		this.txt_Broker.Properties.UsingAutoCompleteBox = false;
		this.txt_Broker.Properties.UsingDataSource = true;
		this.txt_Broker.Properties.ValidateWhenNoDataSource = false;
		this.txt_Broker.Properties.ValueMember = "";
		this.txt_Broker.SettingInVisible = false;
		this.txt_Broker.SettingReadonly = false;
		this.txt_Broker.Size = new System.Drawing.Size(100, 20);
		this.txt_Broker.TabIndex = 20;
		this.gridColumnEx36.Caption = "Broker Code";
		this.gridColumnEx36.ColumnEdit = this.r_BrokerCost;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "AgentCode";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx37.Caption = "Broker Name";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "AgentName";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.r_BrokerCost.AutoHeight = false;
		this.r_BrokerCost.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions11.Image");
		this.r_BrokerCost.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_BrokerCost.ContainerControl = this;
		this.r_BrokerCost.DisplayMember = "";
		this.r_BrokerCost.DisplayMember2 = "";
		this.r_BrokerCost.ListMandatoryField = null;
		this.r_BrokerCost.LoadDataSourceWhenEnter = false;
		this.r_BrokerCost.Name = "r_BrokerCost";
		this.r_BrokerCost.SeparateCharacterForMulti = ';';
		this.r_BrokerCost.SeparateString = ", ";
		this.r_BrokerCost.UsingAutoCompleteBox = false;
		this.r_BrokerCost.UsingDataSource = true;
		this.r_BrokerCost.ValidateWhenNoDataSource = false;
		this.r_BrokerCost.ValueMember = "";
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1055, 594);
		base.Controls.Add(this.txt_Broker);
		base.Controls.Add(this.txt_Vender);
		base.Controls.Add(this.drBtn_CopyFrom);
		base.Controls.Add(this.txt_Num);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oPurchaseLandedCost";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_LandedCosts.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LandedCosts";
		this.Text = "Landed Costs";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_LandedCosts_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice39).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice41).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice42).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_LandedCost).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_LandedCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookSeries).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_datePosting.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_datePosting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateDue.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateDue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_VendorItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_BrockerItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookCurrencyItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateCus.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_dateCus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_DetailLandedCosts).EndInit();
		this.xtra_DetailLandedCosts.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Item).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_QtyItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_value).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookCurrencyIte).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_ttExpLC).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnWhsCodeItem).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Costs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Costs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookOhType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_CostSum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_checkCost).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_btnItemDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_lookPriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_looExpenditure).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Num.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Vender.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Broker.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_BrokerCost).EndInit();
		base.ResumeLayout(false);
	}
}
