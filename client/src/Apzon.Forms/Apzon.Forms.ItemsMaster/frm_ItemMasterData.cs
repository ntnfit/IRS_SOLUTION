using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.SetUp.Tax;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryReport;
using Apzon.Forms.Production;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Data;
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

namespace Apzon.Forms.ItemsMaster;

public class frm_ItemMasterData : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public delegate void CloseEvent(ItemsImpl objItems);

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oItems);

	private string _itemCode = string.Empty;

	private string _oldItemCode;

	private string _curPictureName = "";

	private string _dfltWh = "";

	private string _picFilePath;

	private string _cloudImage = "";

	private bool _picFocus;

	private bool _isProcessing;

	private bool _isLoading;

	private bool _isChangingLength;

	private bool _isChangingVolume;

	private bool _isSettingItemGroup;

	private bool flag_SearchByItemCodfrmSpecialBP;

	private bool flag_SearchByItemCode;

	private bool _isChangingMode;

	private bool _isChangingchkInvntItem;

	private bool _isChangingChkPhantom;

	private bool _changeLength;

	private List<UserFieldsImpl> _lstUDFFields;

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private DataTable _dtEvalSystem = new DataTable();

	private DataTable _dtPUom = new DataTable();

	private DataTable _dtSUom = new DataTable();

	private DataTable _dtPriceList = new DataTable();

	private DataTable _dtPreferredVendor = new DataTable();

	private List<DataTable> _lstStructDt = new List<DataTable>();

	private Dictionary<string, Control> _dicControls;

	private DataSet _dsAllBinLocationCode;

	public ItemsImpl _objItems = new ItemsImpl();

	public DataTable _dtLine = new DataTable();

	public DataTable _dtCurrency = new DataTable();

	public bool flagUDF;

	public frm_ChooseFromList frmChooseFromList;

	public string itmType;

	public string itmGroup;

	public string idManufacturer;

	public string idShippingType;

	public string idVatPu;

	public string idVatSa;

	public string idPrList;

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	public frm_ItemMasterData frmItemMasterData;

	public ShowForms frmShowChildrenForm;

	public CloseEvent reloadParent;

	public double price;

	private IContainer components;

	private LabelLine lb_PriceList;

	private LabelLine lb_UnitPrice;

	private TextEditEx txtDescription;

	private TextEditEx txtBarCode;

	private LabelLine lb_BarCode;

	private LabelLine lb_ItemGroup;

	private LabelLine lb_Description;

	private TextEditEx txtItemCode;

	private LabelLine lb_ItemCode;

	private CheckEditEx chkSellItem;

	private CheckEditEx chkPrchseItem;

	private CheckEditEx chkInvntItem;

	private TextEditEx txtFrgnName;

	private LabelLine lb_DescForLang;

	private XtraTabControlEx tabControl_Detail;

	private XtraTabPageEx tab_General;

	private LabelLine lb_ShippingType;

	private LabelLine lb_Manufacture;

	private XtraTabPageEx tab_Purchase;

	private XtraTabPageEx tab_Sales;

	private XtraTabPageEx tab_Inventory;

	private XtraTabPageEx tab_Remark;

	private LabelLine lb_ItemType;

	private Label label1;

	private CalcEditEx txtPurFactor4;

	private CalcEditEx txtPurFactor3;

	private CalcEditEx txtPurFactor2;

	private CalcEditEx txtPurFactor1;

	private LabelLine lb_Factor4;

	private LabelLine lb_Factor3;

	private LabelLine lb_Factor2;

	private LabelLine lb_Factor1;

	private TextEditEx txtPurPackMsr;

	private TextEditEx txtSuppCatNum;

	private LabelLine lb_TaxGroup;

	private LabelLine lb_PackagingUoM;

	private LabelLine lb_Purchasing;

	private LabelLine lb_ItemsPerPurchaseUnit;

	private LabelLine lb_PurchasingUoM;

	private LabelLine lb_CatalogNo;

	private LabelLine lb_PreferredVendor;

	private CalcEditEx txtNumInBuy;

	private CalcEditEx txtPurPackUn;

	private Label label2;

	private CalcEditEx txtSalFactor4;

	private CalcEditEx txtSalFactor3;

	private CalcEditEx txtSalFactor2;

	private CalcEditEx txtSalFactor1;

	private LabelLine lb_Factor4S;

	private LabelLine lb_Factor3S;

	private LabelLine lb_Factor2S;

	private LabelLine lb_Factor1S;

	private TextEditEx txtSalPackMsr;

	private LabelLine lb_TaxGroupS;

	private LabelLine lb_PackagingUoMS;

	private LabelLine lb_Sales;

	private LabelLine lb_SalesUoM;

	private CalcEditEx txtSalPackUn;

	private LabelLine lb_Maximum;

	private LabelLine lb_Minimum;

	private LabelLine lb_RequiredPrUOM;

	private LabelLine lbItemCost;

	private LabelLine lb_ValuationMethod;

	private LabelLine lb_InventoryUom;

	private LabelLine lb_AccBy;

	private CalcEditEx txtAvgPrice;

	private CheckEditEx chkByWh;

	private GridControlEx grd_WareHouse;

	private GridColumnEx gcWhseCode;

	private GridColumnEx gcWhseName;

	private GridColumnEx gcLocked;

	private GridColumnEx gcPriceInventory;

	private GridColumnEx gcOnHand;

	private GridColumnEx gcCommited;

	private GridColumnEx gcOnOrder;

	private GridColumnEx gcAvaiable;

	private ButtonEditEx txtCardCode;

	private GridColumnEx gcMinInventory;

	private GridColumnEx gcMaxInventory;

	private GridColumnEx gcReqInventory;

	private PanelControl panelControl9;

	private PictureEdit picRemarks;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcCreateDate;

	private GridColumnEx gcUserSign2;

	private GridColumnEx gcUpdateDate;

	private RepositoryItemCheckEdit rChk_Lock;

	private LabelLine lb_SerialAndBatchNum;

	private LabelLine lb_ManageItemBy;

	private LabelLine lb_ManagementMethod;

	private LabelLine lbVailFrom;

	private DateEditEx dateValidFrom;

	private DateEditEx dateValidTo;

	private LabelLine lbVailTo;

	private TextEditEx txtValidComm;

	private LabelLine lbRemarks;

	private Panel panel_Inactive;

	private TextEditEx txtFrozenComm;

	private LabelLine lb_T;

	private LabelLine lb_R;

	private LabelLine lb_F;

	private DateEditEx dateFrozenTo;

	private DateEditEx dateFrozenFrom;

	private Panel panel_Active;

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

	private XtraTabPageEx tab_UserDefineF;

	private GridControlEx grd_Udf;

	private XtraTabPageEx tab_PlanData;

	private LabelLine lb_Days1;

	private LabelLine lb_Days;

	private CalcEditEx txt_MinimumOrderQty;

	private CalcEditEx txt_OrderMultiple;

	private LabelLine lb_ToleranceDays;

	private LabelLine lb_MiniumOrderQty;

	private LabelLine lb_LeadTime;

	private LabelLine lb_OrderMultiple;

	private LabelLine lb_ProcurementMethod;

	private LabelLine lb_OrderInterval;

	private LabelLine lb_PlanningMethod;

	private LabelLine lb_ComponentWarehouse;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btn_SetDfltWH;

	private CalcEditEx txt_ToleranceDays;

	private CalcEditEx txt_LeadTime;

	private LookUpEditCombo cbVatGroupPu;

	private LookUpEditCombo cbVatGourpSa;

	private CalcEditEx calc_VatPercentPurchase;

	private CalcEditEx calc_VatPercentSales;

	private XtraTabPageEx tab_Properties;

	private SimpleButtonEx btn_PropertiesClearSelect;

	private SimpleButtonEx btn_PropertiesSellectAll;

	private GridControlEx grd_Properties;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemCheckEdit rep_PropertiesCheck;

	private XtraTabPageEx tab_Attachments;

	private MemoEditEx txtUserText;

	private RepositoryItemCalcEdit rep_CalcInventory_Price;

	private RepositoryItemCalcEdit rep_CalcInventory_Quantity;

	private LookUpEditCombo cb_Code;

	private LookUpEditCombo cbItemType;

	private LookUpEditCombo cbItemGroup;

	private LookUpEditCombo cbPriceList;

	private LookUpEditCombo cbManagementMethod;

	private LookUpEditCombo cbManageItemBy;

	private LookUpEditCombo cbShippingType;

	private LookUpEditCombo cbManufacturer;

	private LookUpEditCombo cbEvalSystem;

	private LookUpEditCombo cbGLMethod;

	private LookUpEditCombo cb_OrderInterval;

	private LookUpEditCombo cb_ComponentWarehouse;

	private LookUpEditCombo cb_ProcurementMethod;

	private LookUpEditCombo cb_PlanningMethod;

	private LookUpEditCombo cb_PriceType;

	private RadioGroupEx rdg_Active;

	private LabelLine lb_InventoryLevel;

	private CalcEditEx txtMaxLevel;

	private CalcEditEx txtMinLevel;

	private CalcEditEx txtReorderQty;

	private LookUpEditCombo luk_UoMGroup_UgpEntry;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_CopyToSalesData;

	private LabelLine lb_BLength1;

	private CalcEditEx calc_BVolume;

	private LabelLine lb_BWeight1;

	private LabelLine lb_BVolume;

	private LabelLine lb_BHeight1;

	private LabelLine lb_BWidth1;

	private LabelLine lbl_PUoMEntry;

	private LookUpEditCombo luk_BVolUnit;

	private LabelLine lbl_SUoMEntry;

	private LookUpEditCombo luk_SVolUnit;

	private LabelLine lb_SLength1;

	private CalcEditEx calc_SVolume;

	private LabelLine lb_SWeight1;

	private LabelLine lb_SVolume;

	private LabelLine lb_SHeight1;

	private LabelLine lb_SWidth1;

	private CalcEditEx txtNumInSale;

	private LabelLine lb_ItemsPerSalesUnit;

	private LabelLine lbl_IUoMEntry;

	private CalcEditEx calc_NumInCnt;

	private LabelLine lbl_NumInCnt1;

	private LabelLine lbl_INUoMEntry;

	private TextEditEx txt_CntUnitMsr;

	private LabelLine lbl_CntUnitMsr;

	private LabelLine Weight;

	private LabelLine lbl_NumInCnt;

	private LookUpEditCombo bte_PUoMEntry;

	private LookUpEditCombo bte_SUoMEntry;

	private LookUpEditCombo bte_INUoMEntry;

	private LookUpEditCombo bte_IUoMEntry;

	private LabelLine lbl_NumInBuy;

	private LabelLine lbl_NumInSale;

	private LabelLine lbl_PriceListUoM;

	private ButtonEditEx txtBuyUnitMsr;

	private ButtonEditEx txtSalUnitMsr;

	private ButtonEditEx txtInvntUom;

	private SimpleButtonEx btn_PUoMForm;

	private SimpleButtonEx btn_SUoMForm;

	private CalcEditEx calc_CustomsGroup;

	private LookUpEditCombo luk_CstGrpCode;

	private Label label3;

	private LabelLine lb_CstGrpCode;

	private SimpleButtonEx btn_PicDelete;

	private SimpleButtonEx btBrower;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem businessPartnerCatalogNumbersToolStripMenuItem;

	private ToolStripMenuItem billOfMaterialsToolStripMenuItem;

	private ToolStripMenuItem inventoryPostingListToolStripMenuItem;

	private ToolStripMenuItem inventoryAuditReportToolStripMenuItem;

	private ToolStripMenuItem avaiableToPromiseToolStripMenuItem;

	private ContextMenuStrip ctmenu_Warehouse;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemButtonEdit rep_bte_WhsCode;

	private TextEditExPrice txtUnitPrice;

	private XtraTabPageEx tab_Production;

	private CheckEditEx chk_Phantom;

	private LookUpEditCombo luk_IssueMthd;

	private LabelLine lb_IssueMthd;

	private LabelLine lb_TreeType;

	private CalcEditEx calc_NoOfRes;

	private CalcEditEx calc_NoOfItem;

	private LabelLine lb_NoOfItem;

	private LabelLine lb_PrdStdCst;

	private LabelLine lb_NoOfRes;

	private CheckEditEx chk_InCostRoll;

	private ButtonEditEx txt_TreeType;

	private TextEditExPrice calc_PrdStdCst;

	private LabelLine lbl_ServiceAttr;

	private CheckEditEx chk_NoDiscount;

	private LabelLine lbl_WarrntyTpl;

	private ButtonEditCFL bte_WarrntTmpl;

	private SimpleButtonEx btn_PreferredVendor;

	private CheckEditEx chk_ManOutOnly;

	private LookUpEditCombo luk_IssuePriBy;

	private LabelLine lbl_IssuePriBy;

	private CheckEditEx chk_OneBOneRec;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemButtonEditCFL rep_DefaultBinLocation;

	private MeasureUnitEditEx calc_BLength1;

	private MeasureUnitEditEx calc_BWeight1;

	private MeasureUnitEditEx calc_BHeight1;

	private MeasureUnitEditEx calc_BWidth1;

	private MeasureUnitEditEx calc_SWeight1;

	private MeasureUnitEditEx calc_SHeight1;

	private MeasureUnitEditEx calc_SWidth1;

	private MeasureUnitEditEx calc_SLength1;

	private MeasureUnitEditEx calc_IWeight1;

	protected GridViewEx gv_WareHouse;

	protected GridViewEx gv_Udf;

	protected GridViewEx gv_Properties;

	protected ucAttachment ucAttachment1;

	public bool IsAssetMasterData { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btOK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.Find:
			btOK.Text = Resources.ButtonFind;
			break;
		case Enums.FormViewModeType.View:
			Controls.EnableDisableControl(FormAuth == Enums.FormAuthorities.Full);
			btCancel.Enabled = true;
			btOK.Enabled = true;
			btOK.Text = Resources.ButtonOK;
			break;
		default:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public async Task MODE_FIND()
	{
		_isChangingMode = true;
		bool flag = true;
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			flag = false;
		}
		if (flag)
		{
			btOK.Enabled = true;
			_oldItemCode = "";
			flag_SearchByItemCodfrmSpecialBP = false;
			flag_SearchByItemCode = false;
			_itemCode = "";
			this.SetFindMode();
			FormMode = Enums.FormViewModeType.Find;
			Reset();
			if (FormMode == Enums.FormViewModeType.Find)
			{
				Controls.EnableDisableControl(isEnable: true);
			}
			if (IsAssetMasterData)
			{
				cbItemType.EditValue = "F";
			}
			else
			{
				cbItemType.EditValue = "";
			}
			_isSettingItemGroup = true;
			cbItemGroup.EditValue = Function.ParseShort(-1);
			_isSettingItemGroup = false;
			LoadRelatedItemGroupData();
			cbShippingType.EditValue = null;
			cbManufacturer.EditValue = Function.ParseShort(-1);
			txtItemCode.Focus();
			cbPriceList.Enabled = false;
			cbPriceList.EditValue = Function.ParseShort(-1);
			panel_Active.Visible = false;
			txtItemCode.ReadOnly = false;
			cb_Code.EditValue = Function.ParseShort(0);
			DataTable dt = new DataTable();
			if (WinFuntions.GetTableStruct(ref dt, "ATC1", _lstStructDt))
			{
				ucAttachment1.grd_File.DataSource = dt;
			}
			else
			{
				ucAttachment1.grd_File.DataSource = null;
			}
			ucAttachment1.grd_File.RefreshDataSource();
			picRemarks.EditValue = null;
			btn_PicDelete.Visible = false;
			_dtPUom.Rows.Clear();
			_dtPUom.AcceptChanges();
			_dtSUom.Rows.Clear();
			_dtSUom.AcceptChanges();
			txtUnitPrice.EditValue = null;
			txtUnitPrice.Currency = null;
			SetControlsUoM(Function.ParseInt(-1));
			luk_UoMGroup_UgpEntry.EditValue = null;
			txt_TreeType.Properties.Buttons[0].Visible = false;
			_dtPreferredVendor = _httpClient.Items.GetPreferredVendor("-1");
			DataTable tableStructure = WinFuntions.GetTableStructure("OITM");
			tableStructure.TableName = "OITM";
			DataTable dataTableNewRow = WinFuntions.GetDataTableNewRow(tableStructure, dfltValue: false);
			grd_Udf.DataSource = dataTableNewRow;
			grd_Udf.RefreshDataSource();
		}
		_isChangingMode = false;
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ToString(_oldItemCode);
				Enums.ChangeLog changeLog = Enums.ChangeLog.clItems;
				Apzon.Entities.Models.Common.GetChangeLogParams getChangeLogParams = new Apzon.Entities.Models.Common.GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
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

	public async Task REFRESH()
	{
		_isChangingMode = true;
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				_isChangingMode = false;
				MODE_FIND();
			}
			else
			{
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				if (FormAuth == Enums.FormAuthorities.Full)
				{
					FormMode = Enums.FormViewModeType.Add;
					this.ResetData();
				}
				else
				{
					FormMode = Enums.FormViewModeType.Find;
				}
				DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITM");
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
						WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				}
				_oldItemCode = "";
				flag_SearchByItemCodfrmSpecialBP = false;
				flag_SearchByItemCode = false;
				_itemCode = "";
				txtUnitPrice.Text = "0";
				txtUnitPrice.Enabled = true;
				Reset();
				LoadCombo();
				LoadCheckBox();
				LoadGroupPurchasing();
				LoadGroupSales();
				LoadGroupInventory();
				cbPriceList.Enabled = true;
				if (IsAssetMasterData)
				{
					cbItemType.EditValue = "F";
				}
				else
				{
					cbItemType.EditValue = "I";
				}
				ChangePropetiesItemWhenChangeItemType();
				DataTable dataTable2 = cbItemGroup.Properties.DataSource as DataTable;
				_isSettingItemGroup = true;
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					cbItemGroup.EditValue = Function.ParseShort(dataTable2.Rows[0]["ITMSGRPCOD"]);
				}
				else
				{
					cbItemGroup.EditValue = -1;
				}
				_isSettingItemGroup = false;
				LoadRelatedItemGroupData();
				if (cbPriceList.Properties.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
				{
					cbPriceList.EditValue = Function.ParseShort(dataTable3.Rows[0]["ListNum"]);
				}
				else
				{
					cbPriceList.EditValue = -1;
				}
				cb_PriceType.EditValue = "0";
				cbManageItemBy.EditValue = "0";
				ShowManagementMethod();
				ShowData_CbEvalSystem();
				if (cbManufacturer.Properties.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
				{
					cbManufacturer.EditValue = Function.ParseShort(dataTable4.Rows[0]["FIRMCODE"]);
				}
				if (cbShippingType.Properties.DataSource is DataTable dataTable5 && dataTable5.Rows.Count > 0)
				{
					if (Function.ParseShort(dataTable5.Rows[0]["TrnspCode"]) != -10)
					{
						cbShippingType.EditValue = Function.ParseShort(dataTable5.Rows[0]["TrnspCode"]);
					}
					else
					{
						cbShippingType.EditValue = null;
					}
				}
				List<Tuple<string, string>> itemProperties = GetItemProperties("-1");
				LoadItemProperties(itemProperties);
				grd_Udf.DataSource = DataSource;
				_dfltWh = "";
				rdg_Active.SelectedIndex = 0;
				DataTable dt = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt, "ATC1", _lstStructDt))
				{
					ucAttachment1.grd_File.DataSource = dt;
				}
				else
				{
					ucAttachment1.grd_File.DataSource = null;
				}
				ucAttachment1.grd_File.RefreshDataSource();
				picRemarks.EditValue = null;
				btn_PicDelete.Visible = false;
				_dtPUom.Rows.Clear();
				_dtPUom.AcceptChanges();
				_dtSUom.Rows.Clear();
				_dtSUom.AcceptChanges();
				txtItemCode.ReadOnly = false;
				cb_Code.ReadOnly = false;
				luk_CstGrpCode.EditValue = -1;
				cbVatGroupPu.EditValue = Function.ToString(GlobalData.GeneralSetting.DfPVatItem);
				cbVatGourpSa.EditValue = Function.ToString(GlobalData.GeneralSetting.DfSVatItem);
				txtUnitPrice.EditValue = null;
				txtUnitPrice.Currency = null;
				luk_UoMGroup_UgpEntry.EditValue = -1;
				SetControlsUoM(Function.ParseInt(-1));
				DataTable defaultSeries = _httpClient.Commons.GetDefaultSeries("4", "--", GlobalData.UserID);
				if (defaultSeries.IsNotNull())
				{
					cb_Code.EditValue = Function.ParseShort(defaultSeries.Rows[0][0]);
				}
				if (cb_Code.Text != "Manual")
				{
					if (FormMode == Enums.FormViewModeType.Add)
					{
						DataTable codeBySeries = _httpClient.Commons.GetCodeBySeries(cb_Code.Text, "4", GlobalData.DefaultBranch);
						if (codeBySeries != null && codeBySeries.Rows.Count > 0)
						{
							txtItemCode.Text = ((codeBySeries.Rows[0][0] == DBNull.Value || codeBySeries.Rows[0][0] == null) ? "" : codeBySeries.Rows[0][0].ToString());
						}
						txtItemCode.ReadOnly = true;
					}
					else
					{
						txtItemCode.ReadOnly = false;
					}
				}
				txt_TreeType.Properties.Buttons[0].Visible = false;
				_dtPreferredVendor = _httpClient.Items.GetPreferredVendor("-1");
				luk_SVolUnit.EditValue = GlobalData.GeneralSetting.DefLengthU;
				luk_BVolUnit.EditValue = GlobalData.GeneralSetting.DefLengthU;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isChangingMode = false;
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			GetRecord("F", string.Empty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(txtItemCode.Text) && ExistsItem(txtItemCode.Text))
			{
				GetRecord("N", txtItemCode.Text);
			}
			else
			{
				GetRecord("F", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			GetRecord("L", string.Empty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(txtItemCode.Text) && ExistsItem(txtItemCode.Text))
			{
				GetRecord("P", txtItemCode.Text);
			}
			else
			{
				GetRecord("L", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			GetADoc(txtItemCode.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ItemMasterData()
	{
		InitializeComponent();
		IsScrollable = true;
		OnLoadAsync += frm_ItemMasterData_Load;
	}

	public frm_ItemMasterData(bool flag, bool flagSearchFrm, string itemCode)
	{
		InitializeComponent();
		IsScrollable = true;
		OnLoadAsync += frm_ItemMasterData_Load;
		SettingLanguage();
		flag_SearchByItemCodfrmSpecialBP = flag;
		flag_SearchByItemCode = flagSearchFrm;
		_itemCode = itemCode;
	}

	private async Task frm_ItemMasterData_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			AcceptButton = btOK;
			CancelButton = btCancel;
			LoadDefaultBinLocation();
			ContextMenuStrip.Opening += ContextMenuStrip_Opening;
			_lstStructDt = new List<DataTable>
			{
				new DataTable("OITM"),
				new DataTable("OITW"),
				new DataTable("ITM2"),
				new DataTable("ITM12"),
				new DataTable("ATC1")
			};
			for (int i = 0; i < _lstStructDt.Count; i++)
			{
				string tableName = _lstStructDt[i].TableName;
				_lstStructDt[i] = WinFuntions.GetTableStructure(_lstStructDt[i].TableName);
				_lstStructDt[i].TableName = tableName;
			}
			await SetDictionaryControls();
			gv_WareHouse.FormId = FormId;
			gv_WareHouse.ItemId = "OITW";
			gv_Udf.FormId = FormId;
			gv_Udf.ItemId = "OITW_Udf";
			gv_Properties.FormId = FormId;
			gv_Properties.ItemId = "OITW_Prop";
			await RefreshData();
			SetFormatControlValue();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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

	private void LoadDefaultBinLocation()
	{
		try
		{
			rep_DefaultBinLocation.TypeCFL = Enums.TypeCFL.UserDefinedData;
			rep_DefaultBinLocation.ValueMember = "AbsEntry";
			rep_DefaultBinLocation.DisplayMember = "BinCode";
			_dsAllBinLocationCode = new DataSet();
			_dsAllBinLocationCode.Tables.Add(_httpClient.BinLocMasterDataClient.GetDataBinLocation(0).Copy());
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_DefaultBinLocation;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					frm.DataSource = _dsAllBinLocationCode.Copy();
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_DefaultBinLocation;
			repositoryItemButtonEditCFL2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					frm.gv_CFL.CustomRowFilter += (object s, RowFilterEventArgs ev) =>
					{
						try
						{
							string text = Function.ToString(gv_WareHouse.GetFocusedRowCellValue("WhsCode"));
							if (Function.ToString(frm.gv_CFL.GetListSourceRowCellValue(ev.ListSourceRow, "WhsCode")) != text)
							{
								ev.Visible = false;
								ev.Handled = true;
							}
						}
						catch (Exception ex3)
						{
							Logging.Write("ERROR", ex3);
						}
					};
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
				}
			}));
			rep_DefaultBinLocation.RefreshDataSource();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_DefaultBinLocation;
			repositoryItemButtonEditCFL3.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL3.ItemSelected, new Action<DataRow, GridView>(BinItemSelected));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void BinItemSelected(DataRow arg1, GridView arg2)
	{
		try
		{
			if (_dsAllBinLocationCode.IsNotNull())
			{
				rep_DefaultBinLocation.DataSource = _dsAllBinLocationCode.Tables[0];
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void inventoryPostingListToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txtItemCode.EditValue);
			frm_InventoryPostingList frm_InventoryPostingList2 = new frm_InventoryPostingList();
			frm_InventoryPostingList2.ItemFrom = text;
			frm_InventoryPostingList2.ItemTo = text;
			frm_InventoryPostingList2.GroupByItem = true;
			frm_InventoryPostingList2.MdiParent = MdiParent;
			frm_InventoryPostingList2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			string itemCode = Function.ToString(txtItemCode.Text);
			if (!Task.Run(async () => await ExtenseionMethods.CheckExistItem(itemCode)).Result)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			cb_Code.ReadOnly = false;
			cb_Code.BackColor = Color.White;
			if (cb_Code.Text == "Manual")
			{
				txtItemCode.ReadOnly = false;
				txtItemCode.BackColor = Color.White;
			}
			else if (cb_Code.Text != "Manual")
			{
				DataTable codeBySeries = _httpClient.Commons.GetCodeBySeries(cb_Code.Text, "4", GlobalData.DefaultBranch);
				if (codeBySeries != null && codeBySeries.Rows.Count > 0)
				{
					txtItemCode.Text = ((codeBySeries.Rows[0][0] == DBNull.Value || codeBySeries.Rows[0][0] == null) ? "" : codeBySeries.Rows[0][0].ToString());
				}
				txtItemCode.ReadOnly = true;
				txtItemCode.BackColor = Color.FromArgb(231, 231, 231);
			}
			if (grd_WareHouse.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					dataTable.Rows[num]["OnHand"] = 0;
					dataTable.Rows[num]["IsCommited"] = 0;
					dataTable.Rows[num]["OnOrder"] = 0;
					dataTable.Rows[num]["AvgPrice"] = 0;
					dataTable.Rows[num]["Locked"] = "N";
				}
				dataTable.AcceptChanges();
				grd_WareHouse.DataSource = dataTable;
				grd_WareHouse.RefreshDataSource();
			}
			txt_TreeType.EditValue = "";
			calc_NoOfItem.EditValue = 0;
			calc_NoOfRes.EditValue = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void billOfMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string searchCode = Function.ToString(txtItemCode.EditValue);
			Function.ToString(cbPriceList.EditValue);
			Function.ToString(grd_WareHouse.DataSourceTable.Rows[0]["WhsCode"]);
			frm_BillOfMaterials frm_BillOfMaterials2 = new frm_BillOfMaterials();
			frm_BillOfMaterials2._searchCode = searchCode;
			frm_BillOfMaterials2._isAddNew = true;
			frm_BillOfMaterials2.MdiParent = MdiParent;
			frm_BillOfMaterials2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtCardCode_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vendor, false, txtCardCode.Text.Trim(), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txtCardCode.Text = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
				}
			}
			if (e.Button.Index == 1)
			{
				if (!string.IsNullOrEmpty(txtCardCode.Text))
				{
					frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
					frm_BusinessMasterData2._copyF_CodeCustomer = txtCardCode.Text;
					frm_BusinessMasterData2.MdiParent = MdiParent;
					frm_BusinessMasterData2.Show();
				}
				txtSuppCatNum.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btBrower_Click(object sender, EventArgs e)
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
				}
				btn_PicDelete.Visible = true;
				_picFilePath = openFileDialog.FileName;
				if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		if (_isProcessing)
		{
			return;
		}
		_isProcessing = true;
		btOK.Enabled = false;
		try
		{
			gv_Udf.PostEditor();
			if (FormMode == Enums.FormViewModeType.Find)
			{
				await Search();
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				if (GlobalData.LoginSap)
				{
					if (ValidateSave("A"))
					{
						await SaveData("A");
					}
				}
				else if (ValidateSave("A"))
				{
					SaveDataNoSAP("A");
				}
				FormMode = Enums.FormViewModeType.Add;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				if (GlobalData.LoginSap)
				{
					if (ValidateSave("U"))
					{
						await SaveData("U");
					}
				}
				else if (ValidateSave("U"))
				{
					SaveDataNoSAP("U");
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			if (FormAuth == Enums.FormAuthorities.Full)
			{
				btOK.Enabled = true;
			}
			_isProcessing = false;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (reloadParent != null)
			{
				reloadParent(_objItems);
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PUoMForm_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UoMInItem frm = new frm_UoMInItem();
			frm._dtSource = _dtPUom.Copy();
			frm._uomType = "P";
			frm._itemCode = txtItemCode.Text;
			frm._ugpCode = Function.ToString(luk_UoMGroup_UgpEntry.Properties.GetDisplayText(luk_UoMGroup_UgpEntry.EditValue));
			frm._ugpEntry = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			if (bte_IUoMEntry.Properties.DataSource is DataTable source)
			{
				DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == Function.ParseInt(bte_IUoMEntry.EditValue));
				if (dataRow != null)
				{
					frm._baseUomCode = Function.ToString(dataRow["UomCode"]);
				}
			}
			if (Function.ParseInt(bte_PUoMEntry.EditValue) > 0)
			{
				frm._dfltEntry = Function.ParseInt(bte_PUoMEntry.EditValue);
			}
			frm.FormClosing += (object s, FormClosingEventArgs e2) =>
			{
				try
				{
					if (frm._btnOKClick && frm._changeData)
					{
						_dtPUom = frm._dtReturn.Copy();
						if (frm._dfltEntry > 0)
						{
							ReloadDataUoMSource(bte_PUoMEntry, _dtPUom);
							bte_PUoMEntry.EditValue = frm._dfltEntry;
							_isLoading = true;
							DataRow dataRow2 = _dtPUom.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["UomEntry"]) == frm._dfltEntry);
							if (dataRow2 != null)
							{
								calc_BHeight1.EditValue = Function.ParseDouble(dataRow2["Height1"]);
								calc_BHeight1.UnitCode = Function.ParseShort(dataRow2["Hght1Unit"]);
								calc_BWidth1.EditValue = Function.ParseDouble(dataRow2["Width1"]);
								calc_BWidth1.UnitCode = Function.ParseShort(dataRow2["Wdth1Unit"]);
								calc_BLength1.EditValue = Function.ParseDouble(dataRow2["Length1"]);
								calc_BLength1.UnitCode = Function.ParseShort(dataRow2["Len1Unit"]);
								calc_BVolume.EditValue = Function.ParseDouble(dataRow2["Volume"]);
								luk_BVolUnit.EditValue = Function.ParseShort(dataRow2["VolUnit"]);
								calc_BWeight1.EditValue = Function.ParseDouble(dataRow2["Weight1"]);
								calc_BWeight1.UnitCode = Function.ParseShort(dataRow2["Wght1Unit"]);
								calc_BHeight1.Enabled = true;
								calc_BWidth1.Enabled = true;
								calc_BLength1.Enabled = true;
								calc_BVolume.Enabled = true;
								luk_BVolUnit.Enabled = true;
								calc_BWeight1.Enabled = true;
							}
							_isLoading = false;
						}
						ChangeButtonOKText(sender);
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
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

	private void btn_SUoMForm_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UoMInItem frm = new frm_UoMInItem();
			frm._dtSource = _dtSUom.Copy();
			frm._uomType = "S";
			frm._itemCode = txtItemCode.Text;
			frm._ugpCode = Function.ToString(luk_UoMGroup_UgpEntry.Properties.GetDisplayText(luk_UoMGroup_UgpEntry.EditValue));
			frm._ugpEntry = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			if (bte_IUoMEntry.Properties.DataSource is DataTable source)
			{
				DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == Function.ParseInt(bte_IUoMEntry.EditValue));
				if (dataRow != null)
				{
					frm._baseUomCode = Function.ToString(dataRow["UomCode"]);
				}
			}
			if (Function.ParseInt(bte_SUoMEntry.EditValue) > 0)
			{
				frm._dfltEntry = Function.ParseInt(bte_SUoMEntry.EditValue);
			}
			frm.FormClosing += (object s, FormClosingEventArgs e2) =>
			{
				try
				{
					if (frm._btnOKClick && frm._changeData)
					{
						_dtSUom = frm._dtReturn.Copy();
						if (frm._dfltEntry > 0)
						{
							ReloadDataUoMSource(bte_SUoMEntry, _dtSUom);
							bte_SUoMEntry.EditValue = frm._dfltEntry;
							_isLoading = true;
							DataRow dataRow2 = _dtSUom.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["UomEntry"]) == frm._dfltEntry);
							if (dataRow2 != null)
							{
								calc_SHeight1.EditValue = Function.ParseDouble(dataRow2["Height1"]);
								calc_SHeight1.UnitCode = Function.ParseShort(dataRow2["Hght1Unit"]);
								calc_SWidth1.EditValue = Function.ParseDouble(dataRow2["Width1"]);
								calc_SWidth1.UnitCode = Function.ParseShort(dataRow2["Wdth1Unit"]);
								calc_SLength1.EditValue = Function.ParseDouble(dataRow2["Length1"]);
								calc_SLength1.UnitCode = Function.ParseShort(dataRow2["Len1Unit"]);
								calc_SVolume.EditValue = Function.ParseDouble(dataRow2["Volume"]);
								luk_SVolUnit.EditValue = Function.ParseShort(dataRow2["VolUnit"]);
								calc_SWeight1.EditValue = Function.ParseDouble(dataRow2["Weight1"]);
								calc_SWeight1.UnitCode = Function.ParseShort(dataRow2["Wght1Unit"]);
								calc_SHeight1.Enabled = true;
								calc_SWidth1.Enabled = true;
								calc_SLength1.Enabled = true;
								calc_SVolume.Enabled = true;
								luk_SVolUnit.Enabled = true;
								calc_SWeight1.Enabled = true;
							}
							_isLoading = false;
						}
						ChangeButtonOKText(sender);
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
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

	private void gvWareHouse_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvWareHouse_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			double num = Function.ParseDouble(gv_WareHouse.GetRowCellValue(gv_WareHouse.FocusedRowHandle, gcOnOrder));
			double num2 = Function.ParseDouble(gv_WareHouse.GetRowCellValue(gv_WareHouse.FocusedRowHandle, gcOnHand));
			double num3 = Function.ParseDouble(gv_WareHouse.GetRowCellValue(gv_WareHouse.FocusedRowHandle, gcCommited));
			if (e.Column == gcOnOrder || e.Column == gcOnHand || e.Column == gcCommited)
			{
				double num4 = num2 + num - num3;
				gv_WareHouse.SetRowCellValue(gv_WareHouse.FocusedRowHandle, gcAvaiable, num4);
			}
			ChangeButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvWareHouse_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(_dfltWh))
			{
				string strA = Function.ToString(gv_WareHouse.GetRowCellValue(e.RowHandle, "WhsCode"));
				if (!string.IsNullOrEmpty(_dfltWh) && string.Compare(strA, _dfltWh, ignoreCase: true) == 0)
				{
					e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_SetDfltWH_Click(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		try
		{
			_dfltWh = Function.ToString(gv_WareHouse.GetFocusedRowCellValue("WhsCode"));
			gv_WareHouse.RefreshData();
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
			ChangeButtonOKText(sender);
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
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PropertiesClearSelect_Click(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
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
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Properties_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PicDelete_Click(object sender, EventArgs e)
	{
		try
		{
			picRemarks.Image = null;
			_picFilePath = "";
			btn_PicDelete.Visible = false;
			if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rdg_Active_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = DataSource as DataTable;
			dynamic val = null;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				val = dataTable.Rows[0];
			}
			ChangeButtonOKText(sender);
			switch (rdg_Active.SelectedIndex)
			{
			case 0:
				panel_Active.Visible = true;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = false;
				if (val != null)
				{
					val["validFor"] = "Y";
					val["frozenFor"] = "N";
					val["frozenFrom"] = DBNull.Value;
					val["frozenTo"] = DBNull.Value;
					val.Table.AcceptChanges();
				}
				break;
			case 1:
				panel_Active.Visible = false;
				panel_Inactive.Visible = true;
				panel_Advanced.Visible = false;
				if (val != null)
				{
					val["validFor"] = "N";
					val["frozenFor"] = "Y";
					val["validFrom"] = DBNull.Value;
					val["validTo"] = DBNull.Value;
					val.Table.AcceptChanges();
				}
				break;
			case 2:
				panel_Active.Visible = false;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = true;
				if (val != null)
				{
					val["validFor"] = "Y";
					val["frozenFor"] = "Y";
					val.Table.AcceptChanges();
				}
				break;
			default:
				panel_Active.Visible = false;
				panel_Inactive.Visible = false;
				panel_Advanced.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbManageItemBy_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOKText(sender);
			ShowManagementMethod();
			ShowData_CbEvalSystem();
			string text = Function.ToString(cbManageItemBy.EditValue);
			if (!_isLoading)
			{
				if (text == "1" || text == "2")
				{
					luk_IssueMthd.EditValue = "M";
					if (GlobalData.GeneralSetting.SnBDfltSB == "Y" && FormMode == Enums.FormViewModeType.Add)
					{
						cbEvalSystem.EditValue = "B";
					}
				}
				else
				{
					luk_IssueMthd.EditValue = "B";
				}
			}
			SetVisibleMultipleRecSameBatch();
			if (!(DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if (!(text == "1"))
			{
				if (text == "2")
				{
					dataTable.Rows[0]["ManSerNum"] = "N";
					dataTable.Rows[0]["ManBtchNum"] = "Y";
				}
				else
				{
					dataTable.Rows[0]["ManSerNum"] = "N";
					dataTable.Rows[0]["ManBtchNum"] = "N";
				}
			}
			else
			{
				dataTable.Rows[0]["ManSerNum"] = "Y";
				dataTable.Rows[0]["ManBtchNum"] = "N";
			}
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cb_PriceType_EditValueChanged(object sender, EventArgs e)
	{
		if (_isLoading)
		{
			return;
		}
		try
		{
			_isLoading = true;
			if (FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			short? i = Function.ParseShort(cbPriceList.EditValue);
			if (i > 0)
			{
				DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["PriceList"]) == i);
				DataRow dataRow2 = _dtPriceList.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["ListNum"]) == i);
				if (dataRow != null && dataRow2 != null)
				{
					switch (Function.ToString(cb_PriceType.EditValue))
					{
					case "0":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency"]);
						}
						else
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["PrimCurr"]);
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["Price"]);
						break;
					case "1":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency1"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency1"]);
						}
						else
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr1"]);
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["AddPrice1"]);
						break;
					case "2":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency2"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency2"]);
						}
						else
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr2"]);
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["AddPrice2"]);
						break;
					}
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private void cbPriceList_EditValueChanged(object sender, EventArgs e)
	{
		if (_isLoading)
		{
			return;
		}
		try
		{
			_isLoading = true;
			if (FormMode == Enums.FormViewModeType.Update)
			{
				txtItemCode.ReadOnly = true;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				txtItemCode.ReadOnly = false;
			}
			short? i = Function.ParseShort(cbPriceList.EditValue);
			if (i > 0)
			{
				DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["PriceList"]) == i);
				DataRow dataRow2 = _dtPriceList.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["ListNum"]) == i);
				if (dataRow != null)
				{
					switch (Function.ToString(cb_PriceType.EditValue))
					{
					case "0":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency"]);
						}
						else if (dataRow2 != null)
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["PrimCurr"]).ToUpper();
						}
						else
						{
							txtUnitPrice.Currency = GlobalData.MainCurrency;
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["Price"]);
						break;
					case "1":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency1"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency1"]);
						}
						else if (dataRow2 != null)
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr1"]).ToUpper();
						}
						else
						{
							txtUnitPrice.Currency = GlobalData.MainCurrency;
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["AddPrice1"]);
						break;
					case "2":
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency2"])))
						{
							txtUnitPrice.Currency = Function.ToString(dataRow["Currency2"]);
						}
						else if (dataRow2 != null)
						{
							txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr2"]).ToUpper();
						}
						else
						{
							txtUnitPrice.Currency = GlobalData.MainCurrency;
						}
						txtUnitPrice.EditValue = Function.ParseDouble(dataRow["AddPrice2"]);
						break;
					}
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private void gv_WareHouse_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if ((e.Column.FieldName == "OnHand" || e.Column.FieldName == "IsCommited" || e.Column.FieldName == "OnOrder" || e.Column.FieldName == "Avaiable" || e.Column.FieldName == "AvgPrice" || e.Column.FieldName == "MinStock" || e.Column.FieldName == "MaxStock" || e.Column.FieldName == "MinOrder") && Function.ParseDouble(e.Value) == 0.0)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_IUoMEntry_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int dataInt = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUoMByUoMGroupItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataInt1 = dataInt;
			frm_CFL2.DataString1 = txtItemCode.Text;
			frm_CFL2.DataString2 = "I";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_IUoMEntry.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["UomEntry"]);
				txtInvntUom.Text = Function.ToString(frm_CFL2.ChooseItem["UomName"]);
				UpdateNumInCount("A");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_INUoMEntry_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int dataInt = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUoMByUoMGroupItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataInt1 = dataInt;
			frm_CFL2.DataString1 = txtItemCode.Text;
			frm_CFL2.DataString2 = "I";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_INUoMEntry.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["UomEntry"]);
				txt_CntUnitMsr.Text = Function.ToString(frm_CFL2.ChooseItem["UomName"]);
				UpdateNumInCount("C");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_PUoMEntry_QueryPopUp(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void InventoryUomEntryChanging(int uomEntry)
	{
		try
		{
			if (bte_IUoMEntry.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == uomEntry);
				if (dataRow != null)
				{
					string utext = Function.ToString(dataRow["UomCode"]);
					lbl_NumInBuy.Utext = utext;
					lbl_NumInSale.Utext = utext;
					lbl_PriceListUoM.Utext = utext;
					lbl_NumInCnt.Utext = utext;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbManageItemBy_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string text = Function.ToString(txtItemCode.EditValue);
			if (FormMode != Enums.FormViewModeType.Find && !string.IsNullOrEmpty(text) && !_isLoading && !_isChangingMode)
			{
				if (Function.ToString(e.NewValue) == "0" && Function.ToString(cbEvalSystem.EditValue) == "B")
				{
					e.Cancel = true;
					WinCommons.Error(Enums.MessageCode.ItemValuationMethodIsSerialBatchItMustBeManagedByBatchOrSerialNumber, "");
				}
				else if (_httpClient.Items.ItemHaveQuantity(text))
				{
					WinCommons.Error(Enums.MessageCode.YouCanModifyManageItemBySettingOnlyAvailableQtyIs0, "");
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidFrom_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (Function.ParseDateTime(e.Value).HasValue)
			{
				_ = Function.ParseDateTime(e.Value).Value;
				if (Function.ParseDateTime(e.Value).Value.Year > 1900)
				{
					return;
				}
			}
			e.DisplayText = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_WhsCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_WareHouse.GetFocusedRowCellValue("WhsCode"));
			if (e.Button.Index == 0)
			{
				frm_CFL frm = new frm_CFL(Enums.TypeCFL.GetListWareHouse, true, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm.ShowDialog(this);
				if (frm.ChooseItems == null || frm.ChooseItems.Count <= 0)
				{
					return;
				}
				for (int i = 0; i < frm.ChooseItems.Count; i++)
				{
					DataRow dataRow = frm.ChooseItems[i];
					if (Function.ToString(dataRow["Inactive"]) == "Y")
					{
						frm.ChooseItems.Remove(dataRow);
					}
				}
				if (!(grd_WareHouse.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || frm.ChooseItems.Count == 0)
				{
					return;
				}
				int num = gv_WareHouse.GetFocusedDataSourceRowIndex();
				for (int j = 0; j < frm.ChooseItems.Count; j++)
				{
					string text2 = Function.ToString(frm.ChooseItems[j]["WhsCode"]);
					for (int k = 0; k < dataTable.Rows.Count; k++)
					{
						if (k != num && string.Compare(Function.ToString(dataTable.Rows[k]["WhsCode"]), text2, ignoreCase: true) == 0 && !string.IsNullOrWhiteSpace(text2))
						{
							WinCommons.Error(Enums.MessageCode.TheSelectedWarehouseAlreadyExists, "");
							return;
						}
					}
				}
				string itemCode = Function.ToString(txtItemCode.Text);
				string text3 = Function.ToString(dataTable.Rows[num]["WhsCode"]);
				if (!string.IsNullOrWhiteSpace(text3) && IsExistsTransItemWarehouse(itemCode, text3))
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteRowThisWhsHasExistsingInventoryTransaction, "");
					return;
				}
				if (!string.IsNullOrWhiteSpace(text3) && IsStockInItemWarehouse(itemCode, text3))
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteRowThereIsStockInThisWhs, "");
					return;
				}
				dataTable.Rows[num]["WhsCode"] = Function.ToString(frm.ChooseItems[0]["WhsCode"]);
				dataTable.Rows[num]["WhsName"] = Function.ToString(frm.ChooseItems[0]["WhsName"]);
				dataTable.Rows[num]["Locked"] = "N";
				dataTable.Rows[num]["AvgPrice"] = 0;
				dataTable.Rows[num]["OnHand"] = 0;
				dataTable.Rows[num]["IsCommited"] = 0;
				dataTable.Rows[num]["OnOrder"] = 0;
				int l;
				for (l = 1; l < frm.ChooseItems.Count; l++)
				{
					if (!dataTable.AsEnumerable().Any((DataRow t) => string.Compare(Function.ToString(t["WhsCode"]), Function.ToString(frm.ChooseItems[l]["WhsCode"]), ignoreCase: true) == 0))
					{
						_ = frm.ChooseItems[l];
						num++;
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["WhsCode"] = Function.ToString(frm.ChooseItems[l]["WhsCode"]);
						dataRow2["WhsName"] = Function.ToString(frm.ChooseItems[l]["WhsName"]);
						dataRow2["Locked"] = "N";
						dataRow2["AvgPrice"] = 0;
						dataRow2["OnHand"] = 0;
						dataRow2["IsCommited"] = 0;
						dataRow2["OnOrder"] = 0;
						dataTable.Rows.InsertAt(dataRow2, num);
						dataTable.AcceptChanges();
					}
				}
				grd_WareHouse.DataSource = dataTable;
				grd_WareHouse.RefreshDataSource();
			}
			else if (!string.IsNullOrWhiteSpace(text))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			if (!(grd_WareHouse.DataSource is DataTable dataTable) || dataTable.Columns.Count == 0)
			{
				DataTable dt = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt, "OITW", _lstStructDt))
				{
					if (!dt.Columns.Contains("WhsName"))
					{
						dt.Columns.Add(new DataColumn("WhsName", typeof(string)));
						dt.AcceptChanges();
					}
					DataRow dataRow = dt.NewRow();
					dataRow["WhsCode"] = "";
					dataRow["WhsName"] = "";
					dataRow["CreateDate"] = DateTime.Today;
					dataRow["UpdateDate"] = DateTime.Today;
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["UserSign2"] = GlobalData.UserID;
					dataRow["Locked"] = "N";
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
					grd_WareHouse.DataSource = dt;
					grd_WareHouse.RefreshDataSource();
					ChangeButtonOKText(sender);
				}
			}
			else if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["WhsCode"]))))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["WhsCode"] = "";
				dataRow2["WhsName"] = "";
				dataRow2["CreateDate"] = DateTime.Today;
				dataRow2["UpdateDate"] = DateTime.Today;
				dataRow2["UserSign"] = GlobalData.UserID;
				dataRow2["UserSign2"] = GlobalData.UserID;
				dataRow2["Locked"] = "N";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grd_WareHouse.DataSource = dataTable;
				grd_WareHouse.RefreshDataSource();
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			int[] selectedRows = gv_WareHouse.GetSelectedRows();
			if (selectedRows == null || selectedRows.Length == 0)
			{
				return;
			}
			string text = Function.ToString(gv_WareHouse.GetFocusedRowCellValue("WhsCode"));
			string itemCode = txtItemCode.Text;
			if (IsStockInItemWarehouse(itemCode, text))
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteRowThereIsStockInThisWhs, "");
				return;
			}
			if (IsExistsTransItemWarehouse(itemCode, text))
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteRowThisWhsHasExistsingInventoryTransaction, "");
				return;
			}
			DataTable dataSource = grd_WareHouse.DataSource as DataTable;
			gv_WareHouse.DeleteSelectedRows();
			grd_WareHouse.DataSource = dataSource;
			grd_WareHouse.RefreshDataSource();
			ChangeButtonOKText(sender);
			if (string.Compare(text, _dfltWh, ignoreCase: true) == 0)
			{
				_dfltWh = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool IsExistsTransItemWarehouse(string itemCode, string whsCode)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(_httpClient.Items.IsExistsTransItemWarehouse(itemCode, whsCode)))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return true;
		}
	}

	private bool IsStockInItemWarehouse(string itemCode, string whsCode)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(_httpClient.Items.IsStockInItemWarehouse(itemCode, whsCode)))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return true;
		}
	}

	private void txtReorderQty_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		if (Function.ParseDouble(e.Value) == 0.0)
		{
			e.Value = 0;
			e.Handled = true;
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string itemCode = txtItemCode.Text;
			if (string.IsNullOrWhiteSpace(itemCode))
			{
				return;
			}
			bool result = Task.Run(async () => await ExtenseionMethods.CheckExistItem(itemCode)).Result;
			if (((FormMode == Enums.FormViewModeType.View) & result) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItemReserve, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = _httpClient.Items.RemoveItem(itemCode);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_TreeType_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (_httpClient.BillOfMasterials.IsABOM(txtItemCode.Text))
			{
				this.DrillDown(ApzObjectTypes.oProductTrees, txtItemCode.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_PreferredVendor_Click(object sender, EventArgs e)
	{
		try
		{
			string dflVendor = "";
			DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(Function.ToString(txtCardCode.Text));
			if (dataTable != null && dataTable.Rows.Count > 0 && Function.ToString(dataTable.Rows[0]["CardType"]).ToLower() == "s")
			{
				dflVendor = Function.ToString(txtCardCode.Text);
			}
			frm_ItemPreferredVendor frm = new frm_ItemPreferredVendor
			{
				DTSource = _dtPreferredVendor.Copy(),
				DfltVendor = dflVendor,
				ItemCode = Function.ToString(txtItemCode.Text)
			};
			frm.FormClosing += (object s1, FormClosingEventArgs sender2) =>
			{
				if (frm.IsOKClick && frm.IsChanged)
				{
					_dtPreferredVendor = frm.DTSource.Copy();
					txtCardCode.Text = frm.DfltVendor;
					ChangeButtonOKText(sender2);
				}
			};
			frm.MdiParent = MdiParent;
			frm.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtCardCode_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			string code = Function.ToString(txtCardCode.Text);
			if (!string.IsNullOrEmpty(code))
			{
				DataTable result = Task.Run(async () => await _httpClient.BusinessMaster.GetADoc(code)).Result;
				if (result == null || result.Rows.Count == 0)
				{
					e.Cancel = true;
				}
				else if (string.Compare(Function.ToString(result.Rows[0]["CardType"]), "s", ignoreCase: true) != 0)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtCardCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ErrorText = "Invalid Vendor code";
		e.ExceptionMode = ExceptionMode.DisplayError;
	}

	private void txtUnitPrice_Validated(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			short? i = Function.ParseShort(cbPriceList.EditValue);
			if (!(i > 0))
			{
				return;
			}
			DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["PriceList"]) == i);
			if (dataRow != null)
			{
				switch (Function.ToString(cb_PriceType.EditValue))
				{
				case "0":
					dataRow["Price"] = Function.ParseDouble(txtUnitPrice.EditValue);
					dataRow["Currency"] = txtUnitPrice.Currency;
					break;
				case "1":
					dataRow["AddPrice1"] = Function.ParseDouble(txtUnitPrice.EditValue);
					dataRow["Currency1"] = txtUnitPrice.Currency;
					break;
				case "2":
					dataRow["AddPrice2"] = Function.ParseDouble(txtUnitPrice.EditValue);
					dataRow["Currency2"] = txtUnitPrice.Currency;
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

	private void cbItemGroup_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == Convert.ToInt16(-1) && !_isSettingItemGroup)
			{
				frm_ItemGroups frm_ItemGroups2 = new frm_ItemGroups
				{
					IsOpenFromBP = true
				};
				frm_ItemGroups2.ShowDialog(this);
				LoadComboOther("ItemGroup", frm_ItemGroups2.ChangedId.ToString());
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbManagementMethod_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find && !_isLoading && !_isChangingMode && Function.ToString(cbEvalSystem.EditValue) == "B")
			{
				e.Cancel = true;
				WinCommons.Error(Enums.MessageCode.SetInventoryMethodBy, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbManufacturer_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				new frm_Manufacturer().ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbShippingType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				new frm_Shipping().ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_CstGrpCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				new frm_CustomsGroup().ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbVatGroupPu_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.NewValue) == "Define New")
			{
				frm_TaxGroups frm_TaxGroups2 = new frm_TaxGroups();
				frm_TaxGroups2.VatType = "I";
				frm_TaxGroups2.ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbVatGourpSa_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.NewValue) == "Define New")
			{
				frm_TaxGroups frm_TaxGroups2 = new frm_TaxGroups();
				frm_TaxGroups2.VatType = "O";
				frm_TaxGroups2.ShowDialog(this);
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cb_OrderInterval_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void bte_PUoMEntry_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int dataInt = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUoMByUoMGroupItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataInt1 = dataInt;
			frm_CFL2.DataString1 = txtItemCode.Text;
			frm_CFL2.DataString2 = "P";
			frm_CFL2.DataTable1 = _dtPUom;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_PUoMEntry.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["UomEntry"]);
				txtBuyUnitMsr.Text = Function.ToString(frm_CFL2.ChooseItem["UomName"]);
				UpdateNumInCount("B");
				calc_BWidth1.Enabled = true;
				calc_BLength1.Enabled = true;
				calc_BWeight1.Enabled = true;
				calc_BHeight1.Enabled = true;
				calc_BVolume.Enabled = true;
				luk_BVolUnit.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_SUoMEntry_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int dataInt = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUoMByUoMGroupItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataInt1 = dataInt;
			frm_CFL2.DataString2 = "S";
			frm_CFL2.DataTable1 = _dtSUom;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_SUoMEntry.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["UomEntry"]);
				txtSalUnitMsr.Text = Function.ToString(frm_CFL2.ChooseItem["UomName"]);
				UpdateNumInCount("S");
				calc_SWidth1.Enabled = true;
				calc_SLength1.Enabled = true;
				calc_SWeight1.Enabled = true;
				calc_SHeight1.Enabled = true;
				calc_SVolume.Enabled = true;
				luk_SVolUnit.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_UoMGroup_UgpEntry_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void btn_CopyToSalesData_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue) == -1)
			{
				calc_SWidth1.EditValue = calc_BWidth1.EditValue;
				calc_SWidth1.UnitCode = calc_BWidth1.UnitCode;
				calc_SLength1.EditValue = calc_BLength1.EditValue;
				calc_SLength1.UnitCode = calc_BLength1.UnitCode;
				calc_SWeight1.EditValue = calc_BWeight1.EditValue;
				calc_SWeight1.UnitCode = calc_BWeight1.UnitCode;
				calc_SHeight1.EditValue = calc_BHeight1.EditValue;
				calc_SHeight1.UnitCode = calc_BHeight1.UnitCode;
				calc_SVolume.EditValue = calc_BVolume.EditValue;
				luk_SVolUnit.EditValue = luk_BVolUnit.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_PUoMEntry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(bte_PUoMEntry.EditValue) <= 0 && !_isLoading)
			{
				txtBuyUnitMsr.Text = "";
				txtNumInBuy.EditValue = 1;
			}
			else
			{
				if (bte_PUoMEntry.Properties.DataSource is DataTable source)
				{
					DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == Function.ParseInt(bte_PUoMEntry.EditValue));
					if (dataRow != null)
					{
						txtBuyUnitMsr.Text = Function.ToString(dataRow["UomName"]);
					}
				}
				UpdateNumInCount("P");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeButtonOKText(sender);
	}

	private void luk_SVolUnit_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		UpdateUoMDatatable("S");
	}

	private void UomNameButtonEdit_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		CFLUomName((ButtonEdit)sender);
	}

	private void frm_ItemMasterData_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isLoading || _isChangingMode)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (new string[3] { calc_SHeight1.FieldName, calc_SWidth1.FieldName, calc_SLength1.FieldName }.Contains(e.FieldName))
			{
				double num = Function.ParseDouble(calc_SHeight1.EditValue);
				double num2 = Function.ParseDouble(calc_SLength1.EditValue);
				double num3 = Function.ParseDouble(calc_SWidth1.EditValue);
				if (Math.Abs(num) > 1E-07 && Math.Abs(num2) > 1E-07 && Math.Abs(num3) > 1E-07)
				{
					short? toUnit = Function.ParseShort(luk_SVolUnit.EditValue);
					double value = num * num2 * num3 * toUnit.GetExchangeData(calc_SHeight1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit.GetExchangeData(calc_SLength1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit.GetExchangeData(calc_SWidth1.UnitCode, Enums.MeasureType.LengthAndWidth);
					_changeLength = true;
					calc_SVolume.EditValue = Math.Round(value, GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
					_changeLength = false;
				}
				UpdateUoMDatatable("S");
			}
			else if (e.FieldName.Equals(luk_SVolUnit.FieldName))
			{
				UpdateUoMDatatable("S");
			}
			else if (e.FieldName.Equals(lb_SVolume.FieldName))
			{
				if (!_changeLength)
				{
					calc_SHeight1.EditValue = 0.0;
					calc_SLength1.EditValue = 0.0;
					calc_SWidth1.EditValue = 0.0;
					UpdateUoMDatatable("S");
				}
			}
			else if (new string[3] { calc_BHeight1.FieldName, calc_BWidth1.FieldName, calc_BLength1.FieldName }.Contains(e.FieldName))
			{
				double num4 = Function.ParseDouble(calc_BHeight1.EditValue);
				double num5 = Function.ParseDouble(calc_BLength1.EditValue);
				double num6 = Function.ParseDouble(calc_BWidth1.EditValue);
				if (Math.Abs(num4) > 1E-07 && Math.Abs(num5) > 1E-07 && Math.Abs(num6) > 1E-07)
				{
					short? toUnit2 = Function.ParseShort(luk_BVolUnit.EditValue);
					double value2 = num4 * num5 * num6 * toUnit2.GetExchangeData(calc_BHeight1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit2.GetExchangeData(calc_BLength1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit2.GetExchangeData(calc_BWidth1.UnitCode, Enums.MeasureType.LengthAndWidth);
					_changeLength = true;
					calc_BVolume.EditValue = Math.Round(value2, GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
					_changeLength = false;
				}
				UpdateUoMDatatable("P");
			}
			else if (e.FieldName.Equals(luk_BVolUnit.FieldName))
			{
				UpdateUoMDatatable("P");
			}
			else if (e.FieldName.Equals(lb_BVolume.FieldName))
			{
				if (!_changeLength)
				{
					calc_BHeight1.EditValue = 0.0;
					calc_BLength1.EditValue = 0.0;
					calc_BWidth1.EditValue = 0.0;
					UpdateUoMDatatable("P");
				}
			}
			else if (e.FieldName.Equals(calc_BWeight1.FieldName))
			{
				UpdateUoMDatatable("P");
			}
			else if (e.FieldName.Equals(calc_SWeight1.FieldName))
			{
				UpdateUoMDatatable("S");
			}
			else if (e.FieldName.Equals(luk_CstGrpCode.FieldName))
			{
				if (luk_CstGrpCode.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["Code"]) == Function.ParseInt(luk_CstGrpCode.EditValue));
					if (dataRow != null)
					{
						calc_CustomsGroup.EditValue = Function.ParseDouble(dataRow["Rate"]);
					}
				}
			}
			else if (e.FieldName.Equals(cbVatGroupPu.FieldName))
			{
				string text = Function.ToString(cbVatGroupPu.EditValue);
				if (string.IsNullOrEmpty(text))
				{
					calc_VatPercentPurchase.EditValue = 0;
					return;
				}
				DataTable dataTable2 = (DataTable)cbVatGroupPu.Properties.DataSource;
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					for (int num7 = 0; num7 < dataTable2.Rows.Count; num7++)
					{
						if (text == Function.ToString(dataTable2.Rows[num7]["Code"]))
						{
							calc_VatPercentPurchase.EditValue = Function.ParseDouble(dataTable2.Rows[num7]["Rate"]);
							break;
						}
					}
				}
				else
				{
					calc_VatPercentPurchase.EditValue = 0;
				}
			}
			else if (e.FieldName.Equals(cbVatGourpSa.FieldName))
			{
				string text2 = Function.ToString(cbVatGourpSa.EditValue);
				if (string.IsNullOrEmpty(text2))
				{
					calc_VatPercentSales.EditValue = 0;
					return;
				}
				DataTable dataTable3 = (DataTable)cbVatGourpSa.Properties.DataSource;
				if (dataTable3 != null && dataTable3.Rows.Count > 0)
				{
					for (int num8 = 0; num8 < dataTable3.Rows.Count; num8++)
					{
						if (text2 == Function.ToString(dataTable3.Rows[num8]["Code"]))
						{
							calc_VatPercentSales.EditValue = Function.ParseDouble(dataTable3.Rows[num8]["Rate"]);
							break;
						}
					}
				}
				else
				{
					calc_VatPercentSales.EditValue = 0;
				}
			}
			else if (e.FieldName.Equals(chkByWh.FieldName))
			{
				EnventChkByWh();
			}
			else if (e.FieldName.Equals(cb_Code.FieldName))
			{
				if (cb_Code.Text == "Manual")
				{
					txtItemCode.Text = "";
					txtItemCode.ReadOnly = false;
				}
				else
				{
					if (!(cb_Code.Text != "Manual"))
					{
						return;
					}
					if (FormMode == Enums.FormViewModeType.Add)
					{
						DataTable codeBySeries = _httpClient.Commons.GetCodeBySeries(cb_Code.Text, "4", GlobalData.DefaultBranch);
						if (codeBySeries != null && codeBySeries.Rows.Count > 0)
						{
							txtItemCode.Text = ((codeBySeries.Rows[0][0] == DBNull.Value || codeBySeries.Rows[0][0] == null) ? "" : codeBySeries.Rows[0][0].ToString());
						}
						txtItemCode.ReadOnly = true;
					}
					else
					{
						txtItemCode.ReadOnly = false;
					}
				}
			}
			else if (e.FieldName.Equals(cbItemType.FieldName))
			{
				ChangePropetiesItemWhenChangeItemType();
			}
			else if (e.FieldName.Equals(cbGLMethod.FieldName))
			{
				EnventCbGLMethod();
			}
			else if (e.FieldName.Equals(cbEvalSystem.FieldName))
			{
				EventCbEvalSystem();
				SetVisibleMultipleRecSameBatch();
			}
			else if (e.FieldName.Equals(cb_ProcurementMethod.FieldName))
			{
				if (Function.ToString(cb_ProcurementMethod.EditValue) == "B")
				{
					lb_ComponentWarehouse.Visible = false;
					cb_ComponentWarehouse.Visible = false;
				}
				else
				{
					lb_ComponentWarehouse.Visible = true;
					cb_ComponentWarehouse.Visible = true;
				}
			}
			else if (e.FieldName.Equals(cbItemGroup.FieldName))
			{
				if (!_isSettingItemGroup && !_isChangingMode && FormMode != Enums.FormViewModeType.Find)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeItemGroup, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						LoadRelatedItemGroupData();
					}
					_dtPUom.Rows.Clear();
					_dtPUom.AcceptChanges();
					_dtSUom.Rows.Clear();
					_dtSUom.AcceptChanges();
				}
			}
			else if (e.FieldName.Equals(bte_SUoMEntry.FieldName))
			{
				if (Function.ParseInt(bte_SUoMEntry.EditValue) <= 0)
				{
					txtSalUnitMsr.Text = "";
					txtNumInSale.EditValue = 1;
					return;
				}
				if (bte_SUoMEntry.Properties.DataSource is DataTable source)
				{
					DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == Function.ParseInt(bte_SUoMEntry.EditValue));
					if (dataRow2 != null)
					{
						txtSalUnitMsr.Text = Function.ToString(dataRow2["UomName"]);
					}
				}
				UpdateNumInCount("S");
			}
			else if (e.FieldName.Equals(chk_Phantom.FieldName))
			{
				_isChangingChkPhantom = true;
				try
				{
					chk_InCostRoll.Enabled = !chk_Phantom.Checked;
					if (_isChangingchkInvntItem)
					{
						_isChangingChkPhantom = false;
						return;
					}
					chkInvntItem.Checked = false;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
				_isChangingChkPhantom = false;
			}
			else if (e.FieldName.Equals(cbManagementMethod.FieldName))
			{
				string text3 = Function.ToString(cbManagementMethod.EditValue);
				if (Function.ToString(cbManageItemBy.EditValue) == "1" && text3 == "R")
				{
					chk_ManOutOnly.Visible = true;
				}
				else
				{
					chk_ManOutOnly.Visible = false;
				}
			}
			else if (e.FieldName.Equals(chkInvntItem.FieldName))
			{
				_isChangingchkInvntItem = true;
				try
				{
					if (_isChangingChkPhantom)
					{
						_isChangingchkInvntItem = false;
						return;
					}
					chk_Phantom.Checked = false;
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
				}
				_isChangingchkInvntItem = false;
			}
			else if (e.FieldName.Equals(bte_IUoMEntry.FieldName))
			{
				InventoryUomEntryChanging(Function.ParseInt(e.Value));
			}
			else if (e.FieldName.Equals(luk_UoMGroup_UgpEntry.FieldName))
			{
				ResetDataUom(Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue), Function.ToString(txtItemCode.EditValue));
				bte_PUoMEntry.EditValue = 0;
				txtBuyUnitMsr.EditValue = "";
				txtNumInBuy.EditValue = 1;
				bte_SUoMEntry.EditValue = 0;
				txtSalUnitMsr.EditValue = "";
				txtNumInSale.EditValue = 1;
				calc_BWidth1.EditValue = 0;
				calc_BLength1.EditValue = 0;
				calc_BWeight1.EditValue = 0;
				calc_BHeight1.EditValue = 0;
				calc_BVolume.EditValue = 0;
				calc_SWidth1.EditValue = 0;
				calc_SLength1.EditValue = 0;
				calc_SWeight1.EditValue = 0;
				calc_SHeight1.EditValue = 0;
				calc_SVolume.EditValue = 0;
				bte_INUoMEntry.EditValue = 0;
				txt_CntUnitMsr.EditValue = "";
				calc_NumInCnt.EditValue = 0;
				luk_SVolUnit.EditValue = GlobalData.GeneralSetting.DefLengthU;
				luk_BVolUnit.EditValue = GlobalData.GeneralSetting.DefLengthU;
				SetControlsUoM(Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue));
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex3.Message);
		}
	}

	private void frm_ItemMasterData_OnControlValueChanging(object sender, ControlChangingEventArgs e)
	{
		try
		{
			if (_isLoading || _isChangingMode || FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			if (e.FieldName.Equals(bte_IUoMEntry.FieldName))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeInventoryUomItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Cancel = true;
				}
			}
			else if (e.FieldName.Equals(luk_IssueMthd.FieldName))
			{
				if (string.Compare(Function.ToString(e.NewValue), "b", StringComparison.OrdinalIgnoreCase) == 0 && (Function.ToString(cbManageItemBy.EditValue) == "1" || Function.ToString(cbManageItemBy.EditValue) == "2"))
				{
					WinCommons.Error(Enums.MessageCode.SerialOrBatchManagedItemCannotDefinedBackflush, "");
					e.Cancel = true;
				}
			}
			else if (e.FieldName.Equals(luk_SVolUnit.FieldName))
			{
				short? toUnit = Function.ParseShort(e.NewValue);
				short? fromUnit = Function.ParseShort(e.OldValue);
				double exchangeData = toUnit.GetExchangeData(fromUnit, Enums.MeasureType.LengthAndWidth);
				_changeLength = true;
				calc_SVolume.EditValue = Function.ParseDouble(calc_SVolume.EditValue) * Math.Pow(exchangeData, 3.0);
				_changeLength = false;
			}
			else if (e.FieldName.Equals(luk_BVolUnit.FieldName))
			{
				short? toUnit2 = Function.ParseShort(e.NewValue);
				short? fromUnit2 = Function.ParseShort(e.OldValue);
				double exchangeData2 = toUnit2.GetExchangeData(fromUnit2, Enums.MeasureType.LengthAndWidth);
				_changeLength = true;
				calc_BVolume.EditValue = Function.ParseDouble(calc_BVolume.EditValue) * Math.Pow(exchangeData2, 3.0);
				_changeLength = false;
			}
			else if (e.FieldName.Equals(luk_UoMGroup_UgpEntry.FieldName) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeUomGroupItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateValidFrom_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateValidFrom2.EditValue = dateValidFrom.EditValue;
	}

	private void dateValidTo_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateValidTo2.EditValue = dateValidTo.EditValue;
	}

	private void dateFrozenFrom_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateFrozenFrom2.EditValue = dateFrozenFrom.EditValue;
	}

	private void dateFrozenTo_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateFrozenTo2.EditValue = dateFrozenTo.EditValue;
	}

	private void dateValidFrom2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateValidFrom.EditValue = dateValidFrom2.EditValue;
	}

	private void dateValidTo2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateValidTo.EditValue = dateValidTo2.EditValue;
	}

	private void dateFrozenFrom2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateFrozenFrom.EditValue = dateFrozenFrom2.EditValue;
	}

	private void dateFrozenTo2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		dateFrozenTo.EditValue = dateFrozenTo2.EditValue;
	}

	private void txtValidComm_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		txtValidComm2.EditValue = txtValidComm.EditValue;
	}

	private void txtFrozenComm_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		txtFrozenComm2.EditValue = txtFrozenComm.EditValue;
	}

	private void txtValidComm2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		txtValidComm.EditValue = txtValidComm2.EditValue;
	}

	private void txtFrozenComm2_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
		txtFrozenComm.EditValue = txtFrozenComm2.EditValue;
	}

	public override void CustomFormSetting()
	{
		AuthsImpl authsImpl = GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl t) => t.menucode == "frm_STDPriceLists");
		if (!authsImpl.authoriza.Equals("N"))
		{
			if (!authsImpl.authoriza.Equals("F"))
			{
				txtUnitPrice.Properties.ReadOnly = true;
			}
		}
		else
		{
			txtUnitPrice.Visible = false;
			lbl_PriceListUoM.Visible = false;
		}
	}

	private void SetVisibleMultipleRecSameBatch()
	{
		try
		{
			chk_OneBOneRec.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnventCbGLMethod()
	{
	}

	private void UpdateUoMDatatable(string p)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (p == "P")
			{
				DataRow dataRow = _dtPUom.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["UomEntry"]) == Function.ParseInt(bte_PUoMEntry.EditValue));
				if (dataRow != null)
				{
					dataRow["Length1"] = Function.ParseDouble(calc_BLength1.EditValue);
					dataRow["Len1Unit"] = Function.ParseInt(calc_BLength1.UnitCode);
					dataRow["Width1"] = Function.ParseDouble(calc_BWidth1.EditValue);
					dataRow["Wdth1Unit"] = Function.ParseInt(calc_BWidth1.UnitCode);
					dataRow["Height1"] = Function.ParseDouble(calc_BHeight1.EditValue);
					dataRow["Hght1Unit"] = Function.ParseInt(calc_BHeight1.UnitCode);
					dataRow["Volume"] = Function.ParseDouble(calc_BVolume.EditValue);
					dataRow["VolUnit"] = Function.ParseInt(luk_BVolUnit.EditValue);
					dataRow["Weight1"] = Function.ParseDouble(calc_BWeight1.EditValue);
					dataRow["Wght1Unit"] = Function.ParseInt(calc_BWeight1.UnitCode);
				}
				_dtPUom.AcceptChanges();
			}
			else if (p == "S")
			{
				DataRow dataRow2 = _dtSUom.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["UomEntry"]) == Function.ParseInt(bte_SUoMEntry.EditValue));
				if (dataRow2 != null)
				{
					dataRow2["Length1"] = Function.ParseDouble(calc_SLength1.EditValue);
					dataRow2["Len1Unit"] = Function.ParseInt(calc_SLength1.UnitCode);
					dataRow2["Width1"] = Function.ParseDouble(calc_SWidth1.EditValue);
					dataRow2["Wdth1Unit"] = Function.ParseInt(calc_SWidth1.UnitCode);
					dataRow2["Height1"] = Function.ParseDouble(calc_SHeight1.EditValue);
					dataRow2["Hght1Unit"] = Function.ParseInt(calc_SHeight1.UnitCode);
					dataRow2["Volume"] = Function.ParseDouble(calc_SVolume.EditValue);
					dataRow2["VolUnit"] = Function.ParseInt(luk_SVolUnit.EditValue);
					dataRow2["Weight1"] = Function.ParseDouble(calc_SWeight1.EditValue);
					dataRow2["Wght1Unit"] = Function.ParseInt(calc_SWeight1.UnitCode);
				}
				_dtSUom.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData(string action)
	{
		_ = 2;
		try
		{
			ItemsImpl doc = GetValueFromDatasource();
			if (doc.CstGrpCode <= 0)
			{
				doc.CstGrpCode = -1;
			}
			doc.Series = Function.ParseInt(cb_Code.EditValue);
			if (FormMode == Enums.FormViewModeType.Add)
			{
				doc.CreateDate = DateTime.Now.Date;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				doc.UserSign = GlobalData.UserID;
				doc.UserSign2 = GlobalData.UserID;
			}
			else
			{
				doc.UserSign2 = GlobalData.UserID;
			}
			doc.ObjType = "4";
			doc.Deleted = "";
			doc.DocEntry = 0;
			doc.DfltWH = _dfltWh;
			if (doc.ManBtchNum == "N" && doc.ManSerNum == "N")
			{
				doc.IssueMthd = Function.ToString(luk_IssueMthd.EditValue);
			}
			DataTable dataTable = grd_WareHouse.DataSource as DataTable;
			if (dataTable != null || dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.Columns.Contains("WhsName"))
				{
					dataTable2.Columns.Remove("WhsName");
					dataTable2.AcceptChanges();
				}
				List<OITW_Impl> list = (List<OITW_Impl>)TableToEntity("OITW", dataTable2);
				list.ForEach((OITW_Impl x) =>
				{
					x.ItemCode = txtItemCode.Text;
				});
				doc.OitwLines = list;
			}
			string pictureName = "";
			if (picRemarks.EditValue != null && _picFilePath != "" && !WinFuntions.UploadPictureFile(_picFilePath, ref pictureName))
			{
				btOK.Enabled = true;
				return;
			}
			doc.PicturName = pictureName;
			if (grd_Properties.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
			{
				List<string> list2 = new List<string>();
				for (int num = 0; num < dataTable3.Rows.Count; num++)
				{
					list2.Add(Function.ToString(dataTable3.Rows[num]["Check"]));
				}
				doc.ItemProperties = list2;
			}
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				List<Items_LineImpl> listITM = (List<Items_LineImpl>)TableToEntity("ITM1", _dtLine);
				doc.ListITM1 = listITM;
			}
			DataTable dt = new DataTable();
			if (WinFuntions.GetTableStruct(ref dt, "ITM12", _lstStructDt))
			{
				dt.Rows.Clear();
				dt.AcceptChanges();
				if (_dtPUom != null && _dtPUom.Rows.Count > 0)
				{
					for (int num2 = 0; num2 < _dtPUom.Rows.Count; num2++)
					{
						dt.ImportRow(_dtPUom.Rows[num2]);
					}
					dt.AcceptChanges();
				}
				if (_dtSUom != null && _dtSUom.Rows.Count > 0)
				{
					for (int num3 = 0; num3 < _dtSUom.Rows.Count; num3++)
					{
						dt.ImportRow(_dtSUom.Rows[num3]);
					}
					dt.AcceptChanges();
				}
			}
			if (dt != null && dt.Rows.Count > 0 && TableToEntity("ITM12", dt) is List<Items_UoMImpl> list3)
			{
				Items_UoMImpl item = new Items_UoMImpl
				{
					UomType = "I",
					UomEntry = Function.ParseInt(bte_IUoMEntry.EditValue)
				};
				list3.Insert(0, item);
				doc.ListITM12 = list3;
			}
			DataTable sourcePreferredVendor = GetSourcePreferredVendor();
			if (sourcePreferredVendor != null && sourcePreferredVendor.Rows.Count > 0 && TableToEntity("ITM2", sourcePreferredVendor) is List<ItemPreferredVendorsImpl> listItemPreferredVendors)
			{
				doc.ListItemPreferredVendors = listItemPreferredVendors;
			}
			List<AttachmentFile_Impl> list4 = ucAttachment1.UploadFile();
			if (ucAttachment1.DataSource.IsNotNull() && list4.Count == 0)
			{
				return;
			}
			doc.AttachmentFiles = list4;
			_lstUDFFields = await WinFuntions.GetUdfFields("OITM");
			if (_lstUDFFields != null && _lstUDFFields.Count > 0 && grd_Udf.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
			{
				DataRow dataRow = dataTable4.Rows[0];
				doc.ListUserFields = new List<UserFieldsImpl>();
				for (int num4 = 0; num4 < _lstUDFFields.Count; num4++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = _lstUDFFields[num4].Name;
					userFieldsImpl.Value = dataRow[_lstUDFFields[num4].Name];
					userFieldsImpl.Type = _lstUDFFields[num4].Type;
					doc.ListUserFields.Add(userFieldsImpl);
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			doc.OldItemCode = _oldItemCode;
			_objItems = doc;
			doc.APZ_SAVE_ACTION = action;
			string text = await Save(doc);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				if (action == "A")
				{
					await REFRESH();
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveDataNoSAP(string action)
	{
		try
		{
			if (!(DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (Function.ParseInt(dataRow["CstGrpCode"]) <= 0)
			{
				dataRow["CstGrpCode"] = -1;
			}
			if (string.Compare(action, "A", ignoreCase: true) == 0)
			{
				dataRow["CreateDate"] = DateTime.Now;
			}
			else
			{
				dataRow["UpdateDate"] = DateTime.Now;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["UserSign2"] = GlobalData.UserID;
			}
			else
			{
				dataRow["UserSign2"] = GlobalData.UserID;
			}
			dataRow["ObjType"] = "4";
			dataRow["Deleted"] = "";
			dataRow["DocEntry"] = 0;
			dataRow["DfltWH"] = _dfltWh;
			dataRow["TreeQty"] = 1;
			if (Function.ParseInt(dataRow["CstGrpCode"]) <= 0)
			{
				dataRow["CstGrpCode"] = -1;
			}
			else
			{
				dataRow["CstGrpCode"] = Function.ParseInt(dataRow["CstGrpCode"]);
			}
			dataRow["CommisGrp"] = 0;
			if (Function.ParseInt(dataRow["OrdrIntrvl"]) <= 0)
			{
				dataRow["OrdrIntrvl"] = DBNull.Value;
			}
			DataTable dataTable2 = grd_WareHouse.DataSource as DataTable;
			DataTable dt = new DataTable();
			if (dataTable2 != null || dataTable2.Rows.Count > 0)
			{
				dt = dataTable2.Copy();
				if (dt.Columns.Contains("WhsName"))
				{
					dt.Columns.Remove("WhsName");
					dt.AcceptChanges();
				}
			}
			else
			{
				WinFuntions.GetTableStruct(ref dt, "OITW", _lstStructDt);
			}
			if (dt != null)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DataRow dataRow2 = dt.Rows[i];
					dataRow2["OnHand"] = ((dataRow2["OnHand"] == null || dataRow2["OnHand"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["OnHand"]));
					dataRow2["IsCommited"] = ((dataRow2["IsCommited"] == null || dataRow2["IsCommited"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["IsCommited"]));
					dataRow2["OnOrder"] = ((dataRow2["OnOrder"] == null || dataRow2["OnOrder"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["OnOrder"]));
					dataRow2["Consig"] = ((dataRow2["Consig"] == null || dataRow2["Consig"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["Consig"]));
					dataRow2["Counted"] = ((dataRow2["Counted"] == null || dataRow2["Counted"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["Counted"]));
					dataRow2["MinStock"] = ((dataRow2["MinStock"] == null || dataRow2["MinStock"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["MinStock"]));
					dataRow2["MaxStock"] = ((dataRow2["MaxStock"] == null || dataRow2["MaxStock"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["MaxStock"]));
					dataRow2["MinOrder"] = ((dataRow2["MinOrder"] == null || dataRow2["MinOrder"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["MinOrder"]));
					dataRow2["AvgPrice"] = ((dataRow2["AvgPrice"] == null || dataRow2["AvgPrice"] == DBNull.Value) ? 0.0 : Function.ParseDouble(dataRow2["AvgPrice"]));
					dataRow2["Locked"] = ((dataRow2["Locked"] == null || dataRow2["Locked"] == DBNull.Value) ? "N" : Function.ToString(dataRow2["Locked"]));
				}
			}
			for (int num = dt.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dt.Rows[num]["WhsCode"])))
				{
					dt.Rows.Remove(dt.Rows[num]);
				}
			}
			dt.AcceptChanges();
			string pictureName = "";
			if (picRemarks.EditValue != null && _picFilePath != "" && !WinFuntions.UploadPictureFile(_picFilePath, ref pictureName))
			{
				btOK.Enabled = true;
				return;
			}
			dataRow["PicturName"] = pictureName;
			if (grd_Properties.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
			{
				for (int j = 0; j < dataTable3.Rows.Count; j++)
				{
					dataRow["QryGroup" + (j + 1)] = Function.ToString(dataTable3.Rows[j]["Check"]);
				}
				dataTable.AcceptChanges();
			}
			DataTable dataTable4 = ((_dtLine == null || _dtLine.Rows.Count <= 0) ? GetItemLineTableStructure() : _dtLine.Copy());
			DataTable dt2 = new DataTable();
			if (!WinFuntions.GetTableStruct(ref dt2, "ITM12", _lstStructDt))
			{
				return;
			}
			dt2.Rows.Clear();
			dt2.AcceptChanges();
			if (_dtPUom != null && _dtPUom.Rows.Count > 0)
			{
				for (int k = 0; k < _dtPUom.Rows.Count; k++)
				{
					dt2.ImportRow(_dtPUom.Rows[k]);
				}
				dt2.AcceptChanges();
			}
			if (_dtSUom != null && _dtSUom.Rows.Count > 0)
			{
				for (int l = 0; l < _dtSUom.Rows.Count; l++)
				{
					dt2.ImportRow(_dtSUom.Rows[l]);
				}
				dt2.AcceptChanges();
			}
			DataTable sourcePreferredVendor = GetSourcePreferredVendor();
			if (grd_Udf.DataSource is DataTable dataTable5 && dataTable5.Rows.Count > 0)
			{
				DataTable dataTable6 = dataTable5.Copy();
				DataRow dataRow3 = dataTable6.Rows[0];
				for (int m = 0; m < dataTable6.Columns.Count; m++)
				{
					if (dataTable6.Columns[m].ColumnName.ToUpper().StartsWith("U_"))
					{
						dataRow[dataTable6.Columns[m].ColumnName] = dataRow3[dataTable6.Columns[m]];
					}
				}
				dataTable.AcceptChanges();
			}
			if (!ucAttachment1.GetAttachmentNoSAP(out var dt3))
			{
				return;
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dataTable.TableName = "OITM";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			dt.TableName = "OITW";
			dt.DataSet?.Tables.Remove(dt.TableName);
			dataSet.Tables.Add(dt);
			dataTable4.TableName = "ITM1";
			dataTable4.DataSet?.Tables.Remove(dataTable4.TableName);
			dataSet.Tables.Add(dataTable4);
			dt2.TableName = "ITM12";
			dt2.DataSet?.Tables.Remove(dt2.TableName);
			dataSet.Tables.Add(dt2);
			dt3.TableName = "ATC1";
			dt3.DataSet?.Tables.Remove(dt3.TableName);
			dataSet.Tables.Add(dt3);
			DataTable dataTable7 = new DataTable();
			dataTable7.Columns.Add(new DataColumn("OldCode", typeof(string)));
			if (FormMode == Enums.FormViewModeType.Update && txtItemCode.Text.Trim() != _oldItemCode.Trim())
			{
				DataRow dataRow4 = dataTable7.NewRow();
				dataRow4[0] = _oldItemCode.Trim();
				dataTable7.Rows.Add(dataRow4);
				dataTable7.AcceptChanges();
			}
			dataTable7.TableName = "OtherTable";
			dataTable7.DataSet?.Tables.Remove(dataTable7.TableName);
			dataSet.Tables.Add(dataTable7);
			sourcePreferredVendor.TableName = "PrefVendor";
			DataSet dataSet2 = sourcePreferredVendor.DataSet;
			if (dataSet2 != null)
			{
				dataSet2.Tables.Remove(sourcePreferredVendor.TableName);
				dataSet2.AcceptChanges();
			}
			dataSet.Tables.Add(sourcePreferredVendor);
			string text = SaveDocDataSet(dataSet);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				if (action == "A")
				{
					REFRESH();
				}
				else
				{
					REFRESH_DATA();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, Resources.Msg_SaveError + ":" + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> Save(ItemsImpl doc)
	{
		try
		{
			return await _httpClient.Items.SaveDoc(doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private string SaveDocDataSet(DataSet ds)
	{
		try
		{
			return _httpClient.Items.SaveDocDataSet(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private void LoadDefaultWareHouse()
	{
		try
		{
			DataTable wareHouseList = GetWareHouseList();
			if (wareHouseList == null || wareHouseList.Rows.Count <= 0)
			{
				return;
			}
			DataTable dt1 = new DataTable();
			if (!WinFuntions.GetTableStruct(ref dt1, "OITW", _lstStructDt))
			{
				return;
			}
			if (!dt1.Columns.Contains("WhsName"))
			{
				dt1.Columns.Add(new DataColumn("WhsName", typeof(string)));
				dt1.AcceptChanges();
			}
			if (GlobalData.GeneralSetting.AutoITW == "Y")
			{
				for (int i = 0; i < wareHouseList.Rows.Count; i++)
				{
					DataRow dataRow = dt1.NewRow();
					dataRow["WhsCode"] = Function.ToString(wareHouseList.Rows[i]["WhsCode"]);
					dataRow["WhsName"] = Function.ToString(wareHouseList.Rows[i]["WhsName"]);
					dataRow["CreateDate"] = DateTime.Today;
					dataRow["UpdateDate"] = DateTime.Today;
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["UserSign2"] = GlobalData.UserID;
					dataRow["Locked"] = "N";
					dt1.Rows.Add(dataRow);
				}
				if (!string.IsNullOrEmpty(GlobalData.GeneralSetting.DfltWhs))
				{
					_dfltWh = GlobalData.GeneralSetting.DfltWhs;
				}
			}
			else
			{
				DataRow dataRow2 = dt1.NewRow();
				dataRow2["WhsCode"] = "";
				dataRow2["WhsName"] = "";
				dataRow2["CreateDate"] = DateTime.Today;
				dataRow2["UpdateDate"] = DateTime.Today;
				dataRow2["UserSign"] = GlobalData.UserID;
				dataRow2["UserSign2"] = GlobalData.UserID;
				dataRow2["Locked"] = "N";
				dt1.Rows.Add(dataRow2);
			}
			dt1.AcceptChanges();
			if (grd_WareHouse.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grd_WareHouse.DataSource = dt1;
					grd_WareHouse.RefreshDataSource();
				}));
			}
			else
			{
				grd_WareHouse.DataSource = dt1;
				grd_WareHouse.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetWareHouseList()
	{
		try
		{
			return _httpClient.Items.ShowWareHouse();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public void LoadGroupPurchasing()
	{
		txtPurFactor1.Text = Function.ParseDouble(1).ToString();
		txtPurFactor2.Text = Function.ParseDouble(1).ToString();
		txtPurFactor3.Text = Function.ParseDouble(1).ToString();
		txtPurFactor4.Text = Function.ParseDouble(1).ToString();
		txtNumInBuy.Text = Function.ParseDouble(1).ToString();
		txtPurPackUn.Text = Function.ParseDouble(1).ToString();
	}

	public void LoadGroupSales()
	{
		txtSalFactor1.Text = Function.ParseDouble(1).ToString();
		txtSalFactor2.Text = Function.ParseDouble(1).ToString();
		txtSalFactor3.Text = Function.ParseDouble(1).ToString();
		txtSalFactor4.Text = Function.ParseDouble(1).ToString();
		txtSalPackUn.Text = Function.ParseDouble(1).ToString();
		txtNumInSale.Text = Function.ParseDouble(1).ToString();
	}

	private void LoadGroupInventory()
	{
		try
		{
			cbGLMethod.EditValue = GlobalData.GeneralSetting.GLMethod;
			cbEvalSystem.EditValue = "A";
			EnventCbGLMethod();
			txtAvgPrice.Text = Convert.ToDouble(0).ToString();
			txtReorderQty.Text = Convert.ToDouble(0).ToString();
			txtMinLevel.Text = Convert.ToDouble(0).ToString();
			txtMaxLevel.Text = Convert.ToDouble(0).ToString();
			EnventChkByWh();
			EventCbEvalSystem();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				LoadDefaultWareHouse();
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				DataTable dt = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt, "OITW", _lstStructDt))
				{
					grd_WareHouse.DataSource = dt;
				}
				else
				{
					grd_WareHouse.DataSource = null;
				}
				grd_WareHouse.RefreshDataSource();
				gv_WareHouse.HideLoadingPanel();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataRecord(DataTable dt)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			WinCommons._OpenProcessing();
			_isLoading = true;
			this.ResetData();
			string itemCode = (_oldItemCode = Function.ToString(dt.Rows[0]["ItemCode"]));
			if (dt.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
			{
				grd_Udf.DataSource = dt;
				grd_Udf.RefreshDataSource();
			}
			DataSource = dt;
			ChangeEnableCheckItem();
			EventCbEvalSystem();
			calc_BLength1.UnitCode = Function.ParseShort(dt.Rows[0]["BLen1Unit"]);
			calc_BWidth1.UnitCode = Function.ParseShort(dt.Rows[0]["BWdth1Unit"]);
			calc_BHeight1.UnitCode = Function.ParseShort(dt.Rows[0]["BHght1Unit"]);
			calc_BWeight1.UnitCode = Function.ParseShort(dt.Rows[0]["BWght1Unit"]);
			calc_SLength1.UnitCode = Function.ParseShort(dt.Rows[0]["SLen1Unit"]);
			calc_SWidth1.UnitCode = Function.ParseShort(dt.Rows[0]["SWdth1Unit"]);
			calc_SHeight1.UnitCode = Function.ParseShort(dt.Rows[0]["SHght1Unit"]);
			calc_SWeight1.UnitCode = Function.ParseShort(dt.Rows[0]["SWght1Unit"]);
			calc_IWeight1.UnitCode = Function.ParseShort(dt.Rows[0]["IWght1Unit"]);
			LoadUoMEntryLookUpEdits(Function.ParseInt(dt.Rows[0]["UgpEntry"]), itemCode);
			cbPriceList.Enabled = true;
			string text = Function.ToString(dt.Rows[0]["ManBtchNum"]);
			string text2 = Function.ToString(dt.Rows[0]["ManSerNum"]);
			if (text == "Y")
			{
				cbManageItemBy.EditValue = "2";
			}
			else if (text2 == "Y")
			{
				cbManageItemBy.EditValue = "1";
			}
			else
			{
				cbManageItemBy.EditValue = "0";
			}
			luk_IssuePriBy.EditValue = Function.ParseShort(dt.Rows[0]["IssuePriBy"]);
			string text3 = Function.ToString(dt.Rows[0]["validFor"]);
			string text4 = Function.ToString(dt.Rows[0]["frozenFor"]);
			DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["validTo"]);
			DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[0]["validFrom"]);
			DateTime? dateTime3 = Function.ParseDateTime(dt.Rows[0]["frozenTo"]);
			DateTime? dateTime4 = Function.ParseDateTime(dt.Rows[0]["frozenFrom"]);
			if (text3 == "Y" && text4 == "Y")
			{
				rdg_Active.SelectedIndex = 2;
			}
			else if ((text3 == "Y" && text4 == "N") || string.IsNullOrEmpty(text4))
			{
				rdg_Active.SelectedIndex = 0;
			}
			else if ((text4 == "Y" && text3 == "N") || string.IsNullOrEmpty(text3))
			{
				rdg_Active.SelectedIndex = 1;
			}
			dateValidTo2.EditValue = dateTime;
			dateValidFrom2.EditValue = dateTime2;
			txtValidComm2.Text = Function.ToString(dt.Rows[0]["ValidComm"]);
			dateFrozenTo2.EditValue = dateTime3;
			dateFrozenFrom2.EditValue = dateTime4;
			txtFrozenComm2.Text = Function.ToString(dt.Rows[0]["FrozenComm"]);
			_dfltWh = Function.ToString(dt.Rows[0]["DfltWH"]);
			_curPictureName = Function.ToString(dt.Rows[0]["PicturName"]);
			_cloudImage = Function.ToString(dt.Rows[0]["CloudImage"]);
			if (tabControl_Detail.SelectedTabPage == tab_Remark)
			{
				LoadRemarkTab();
			}
			List<Tuple<string, string>> itemProperties = GetItemProperties(itemCode);
			LoadItemProperties(itemProperties);
			int absEntry = Function.ParseInt(dt.Rows[0]["AtcEntry"]);
			ucAttachment1.LoadAttachments(absEntry);
			SearchLine(itemCode);
			SearchOITW(itemCode);
			_dtPUom.Rows.Clear();
			_dtPUom.AcceptChanges();
			_dtSUom.Rows.Clear();
			_dtSUom.AcceptChanges();
			SearchITM12(itemCode);
			if (Function.ParseInt(dt.Rows[0]["PUoMEntry"]) > 0)
			{
				calc_BWidth1.Enabled = true;
				calc_BLength1.Enabled = true;
				calc_BWeight1.Enabled = true;
				calc_BHeight1.Enabled = true;
				calc_BVolume.Enabled = true;
				luk_BVolUnit.Enabled = true;
			}
			if (Function.ParseInt(dt.Rows[0]["SUoMEntry"]) > 0)
			{
				calc_SWidth1.Enabled = true;
				calc_SLength1.Enabled = true;
				calc_SWeight1.Enabled = true;
				calc_SHeight1.Enabled = true;
				calc_SVolume.Enabled = true;
				luk_SVolUnit.Enabled = true;
			}
			InventoryUomEntryChanging(Function.ParseInt(bte_IUoMEntry.EditValue));
			SetControlsUoM(Function.ParseInt(dt.Rows[0]["UgpEntry"]));
			if (_httpClient.Items.IsLinkedTrans(itemCode) == "Y")
			{
				cbEvalSystem.ReadOnly = true;
				cbEvalSystem.BackColor = Color.FromArgb(231, 231, 231);
				txtItemCode.ReadOnly = true;
				txtItemCode.BackColor = Color.FromArgb(231, 231, 231);
				cb_Code.ReadOnly = true;
				cb_Code.BackColor = Color.FromArgb(231, 231, 231);
			}
			else if (FormAuth == Enums.FormAuthorities.Full)
			{
				cbEvalSystem.ReadOnly = false;
				cbEvalSystem.BackColor = Color.White;
				txtItemCode.ReadOnly = false;
				txtItemCode.BackColor = Color.White;
				cb_Code.ReadOnly = false;
				cb_Code.BackColor = Color.White;
			}
			chk_Phantom.Checked = Function.ToString(dt.Rows[0]["Phantom"]) == "Y";
			chk_InCostRoll.Checked = Function.ToString(dt.Rows[0]["InCostRoll"]) == "Y";
			DataTable productionData = _httpClient.Items.GetProductionData(Function.ToString(dt.Rows[0]["ItemCode"]));
			if (productionData.IsNotNull())
			{
				string value = Function.ToString(productionData.Rows[0]["TreeType"]);
				if (string.IsNullOrEmpty(value))
				{
					txt_TreeType.Properties.Buttons[0].Visible = false;
				}
				else
				{
					txt_TreeType.Properties.Buttons[0].Visible = true;
				}
				txt_TreeType.Text = value;
				calc_NoOfItem.EditValue = Function.ParseInt(productionData.Rows[0]["NoOfItem"]);
				calc_NoOfRes.EditValue = Function.ParseInt(productionData.Rows[0]["NoOfRes"]);
			}
			else
			{
				txt_TreeType.EditValue = "";
				calc_NoOfItem.EditValue = 0;
				calc_NoOfRes.EditValue = 0;
			}
			_dtPreferredVendor = _httpClient.Items.GetPreferredVendor(itemCode);
			_isLoading = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private void LoadRemarkTab()
	{
		bool flag = false;
		try
		{
			if (!_isLoading)
			{
				flag = true;
			}
			_isLoading = true;
			string text = CopyFileShareHelper.GetUpLoadPath("jpg") + _curPictureName;
			_picFilePath = "";
			btn_PicDelete.Visible = false;
			if (!string.IsNullOrEmpty(_cloudImage))
			{
				picRemarks.LoadAsync(_cloudImage);
			}
			else if (!string.IsNullOrWhiteSpace(_curPictureName) && File.Exists(text))
			{
				try
				{
					picRemarks.Image = null;
					string tempPath = Path.GetTempPath();
					tempPath = (_curPictureName.Contains("APZ-Items-") ? (tempPath + _curPictureName) : (tempPath + "APZ-Items-" + _curPictureName));
					string text2 = "";
					if (File.Exists(tempPath))
					{
						text2 = _ftpHelper.DeleteFileSharing(CopyFileSharingCfg.Domain, tempPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password);
					}
					if (text2 != "")
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Cannot delete file [" + tempPath + "] for copy Items master picture from server");
					}
					else
					{
						_ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, text, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, tempPath, copyForOpen: true);
						if (File.Exists(tempPath))
						{
							using (FileStream stream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
							{
								picRemarks.Image = Image.FromStream(stream);
							}
							_picFilePath = tempPath;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.CannotCopyPictureFromServer2TempFolder4Preview, "");
						}
					}
				}
				catch
				{
					picRemarks.Image = null;
				}
			}
			else
			{
				picRemarks.Image = null;
			}
			if (flag)
			{
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			if (flag)
			{
				_isLoading = false;
			}
		}
	}

	private void LoadCheckBox()
	{
		chkInvntItem.Checked = true;
		chkPrchseItem.Checked = true;
		chkSellItem.Checked = true;
	}

	private void EnventChkByWh()
	{
		if (chkByWh.Checked)
		{
			txtReorderQty.Enabled = false;
			txtMinLevel.Enabled = false;
			txtMaxLevel.Enabled = false;
			gcMinInventory.Visible = true;
			gcMaxInventory.Visible = true;
			gcReqInventory.Visible = true;
		}
		else
		{
			txtReorderQty.Enabled = true;
			txtMinLevel.Enabled = true;
			txtMaxLevel.Enabled = true;
			gcMinInventory.Visible = false;
			gcMaxInventory.Visible = false;
			gcReqInventory.Visible = false;
		}
	}

	private bool ValidateSave(string action)
	{
		try
		{
			ItemsImpl doc = new ItemsImpl();
			doc.ItemCode = txtItemCode.Text;
			doc.ItemName = txtDescription.Text;
			if (doc.ItemCode == string.Empty)
			{
				WinCommons.writeStatus(Enums.MessageCode.YouMustInputItemCode, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				txtItemCode.Focus();
				return false;
			}
			if (doc.ItemCode.Length > 50)
			{
				WinCommons.writeStatus(Enums.MessageCode.InputLength50, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				txtItemCode.Focus();
				return false;
			}
			if (doc.ItemName == string.Empty || doc.ItemName == string.Empty)
			{
				WinCommons.writeStatus(Enums.MessageCode.YouMustInputItemName, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				txtDescription.Focus();
				return false;
			}
			if (action == "A" && !GlobalData.LoginSap && Task.Run(async () => await ExtenseionMethods.CheckExistItem(doc.ItemCode)).Result)
			{
				WinCommons.writeStatus(Enums.MessageCode.ItemCodeExists, Enums.MessageType.Error, Msg_Time._TimeMedium, "", doc.ItemCode);
				txtItemCode.Focus();
				return false;
			}
			if (Function.ParseDouble(txtNumInBuy.EditValue) <= 0.0)
			{
				WinCommons.writeStatus(Enums.MessageCode.EnterQtyGreater0, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				tabControl_Detail.SelectedTabPage = tab_Purchase;
				txtNumInBuy.Focus();
				return false;
			}
			if (Function.ParseDouble(txtNumInSale.EditValue) <= 0.0)
			{
				WinCommons.writeStatus(Enums.MessageCode.EnterQtyGreater0, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				tabControl_Detail.SelectedTabPage = tab_Sales;
				txtNumInSale.Focus();
				return false;
			}
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
					if (dateTime2.Value.Year > 2000 && dateTime.Value.CompareTo(dateTime2.Value) > 0)
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
					if (dateTime4.Value.Year > 2000 && dateTime3.Value.CompareTo(dateTime4.Value) > 0)
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
						if (dateTime2.Value.Year > 2000 && dateTime.Value.CompareTo(dateTime2.Value) > 0)
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
						if (dateTime4.Value.Year > 2000 && dateTime3.Value.CompareTo(dateTime4.Value) > 0)
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
			if (rdg_Active.SelectedIndex == 2)
			{
				return true;
			}
			double num = Function.ParseDouble(txtReorderQty.EditValue);
			double num2 = Function.ParseDouble(txtMinLevel.EditValue);
			double num3 = Function.ParseDouble(txtMaxLevel.EditValue);
			if (num > 0.0 && num2 > 0.0 && num <= num2)
			{
				WinCommons.Error(Enums.MessageCode.TheMinimumInventoryMustLessThanRequireInventoryWithCode, "", doc.ItemCode);
				return false;
			}
			if (num > 0.0 && num3 > 0.0 && num3 <= num)
			{
				WinCommons.Error(Enums.MessageCode.TheMaximunStockLevelMustGreaterThanRequiredStockLevelWithCode, "", doc.ItemCode);
				return false;
			}
			if (num2 > 0.0 && num3 > 0.0 && num3 <= num2)
			{
				WinCommons.Error(Enums.MessageCode.TheMinimumInventoryLevelMustLessThanMaximumInventoryLevelWithCode, "", doc.ItemCode);
				return false;
			}
			if (chk_Phantom.Checked && chkInvntItem.Checked)
			{
				WinCommons.Error(Enums.MessageCode.PhantomItemCannotBeInventoryItem, "");
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private void ChooseFrmListItemCode(string itemCode)
	{
		try
		{
			new List<ItemsImpl>();
			ItemsImpl itemsImpl = new ItemsImpl();
			new Items_LineImpl();
			string txtText = Function.ToString(itemsImpl.ItemCode);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: false, txtText, null);
			frm_ChooseFromList2.ShowDialog(this);
			itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItem;
			if (itemsImpl != null && !string.IsNullOrEmpty(itemsImpl.ItemCode))
			{
				GetADoc(itemsImpl.ItemCode);
			}
			else if (string.IsNullOrEmpty(itemCode))
			{
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SearchLine(string itemCode)
	{
		try
		{
			_dtLine = _httpClient.Items.SearchLine(itemCode);
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				cb_PriceType.EditValue = "0";
				cbPriceList.EditValue = Function.ParseShort(_dtLine.Rows[0]["PriceList"]);
				ShowPriceAndCurrency();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				txtUnitPrice.Focus();
				txtItemCode.ReadOnly = false;
				txtUnitPrice.Enabled = true;
				FormMode = Enums.FormViewModeType.View;
				txtUnitPrice.EditValue = null;
				txtUnitPrice.Currency = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SearchOITW(string itemCode)
	{
		FormMode = Enums.FormViewModeType.View;
		try
		{
			DataTable dataTable = _httpClient.Items.SearchOITW(itemCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!dataTable.Columns.Contains("WhsName"))
				{
					dataTable.Columns.Add(new DataColumn("WhsName", typeof(string)));
				}
				DataTable dtWhs = GetWareHouseList();
				if (dtWhs != null && dtWhs.Rows.Count > 0)
				{
					int i;
					for (i = 0; i < dtWhs.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.Compare(Function.ToString(t["WhsCode"]), Function.ToString(dtWhs.Rows[i]["WhsCode"]), ignoreCase: true) == 0);
						if (dataRow != null)
						{
							dataRow["WhsName"] = Function.ToString(dtWhs.Rows[i]["WhsName"]);
						}
					}
				}
				grd_WareHouse.DataSource = dataTable;
				grd_WareHouse.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				DataTable dt = new DataTable();
				if (WinFuntions.GetTableStruct(ref dt, "OITW", _lstStructDt))
				{
					grd_WareHouse.DataSource = dt;
				}
				else
				{
					grd_WareHouse.DataSource = null;
				}
				if (!dt.Columns.Contains("WhsName"))
				{
					dt.Columns.Add(new DataColumn("WhsName", typeof(string)));
				}
				grd_WareHouse.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SearchITM12(string itemCode)
	{
		try
		{
			_dtPUom.Rows.Clear();
			_dtPUom.AcceptChanges();
			_dtSUom.Rows.Clear();
			_dtSUom.AcceptChanges();
			DataTable dataTable = _httpClient.Items.SearchITM12(itemCode);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			_dtPUom = dataTable.Clone();
			_dtSUom = dataTable.Clone();
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ToString(row["UomType"]) == "P")
				{
					_dtPUom.ImportRow(row);
					_dtPUom.AcceptChanges();
				}
				else if (Function.ToString(row["UomType"]) == "S")
				{
					_dtSUom.ImportRow(row);
					_dtSUom.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetDataUom(int ugpCode, string itemCode)
	{
		try
		{
			_dtPUom.Rows.Clear();
			_dtPUom.AcceptChanges();
			_dtSUom.Rows.Clear();
			_dtSUom.AcceptChanges();
			DataTable uomInItemByUomGroup = _httpClient.Items.GetUomInItemByUomGroup(ugpCode, itemCode);
			if (uomInItemByUomGroup == null || uomInItemByUomGroup.Rows.Count <= 0)
			{
				return;
			}
			_dtPUom = uomInItemByUomGroup.Clone();
			_dtSUom = uomInItemByUomGroup.Clone();
			foreach (DataRow row in uomInItemByUomGroup.Rows)
			{
				if (Function.ToString(row["UomType"]) == "P")
				{
					_dtPUom.ImportRow(row);
					_dtPUom.AcceptChanges();
				}
				else if (Function.ToString(row["UomType"]) == "S")
				{
					_dtSUom.ImportRow(row);
					_dtSUom.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		try
		{
			ItemsImpl itemsImpl = new ItemsImpl();
			Items_LineImpl items_LineImpl = new Items_LineImpl();
			string itemCode = string.Empty;
			if (flag_SearchByItemCodfrmSpecialBP)
			{
				itemCode = _itemCode;
			}
			else if (flag_SearchByItemCode)
			{
				GetADoc(_itemCode);
			}
			else
			{
				itemCode = txtItemCode.Text.Trim();
			}
			string itemType = (itemsImpl.ItemType = Function.ToString(cbItemType.EditValue));
			int num = Function.ParseInt(cbItemGroup.EditValue);
			if (num > 0)
			{
				itemsImpl.ItmsGrpCod = num;
			}
			int num2 = Function.ParseInt(cbShippingType.EditValue);
			if (num2 > 0)
			{
				itemsImpl.ShipType = num2;
			}
			int num3 = Function.ParseInt(cbManufacturer.EditValue);
			if (num3 > 0)
			{
				itemsImpl.FirmCode = num3;
			}
			if (!string.IsNullOrEmpty(Function.ToString(cbVatGroupPu.EditValue)))
			{
				itemsImpl.VatGroupPu = Function.ToString(cbVatGroupPu.EditValue);
			}
			if (!string.IsNullOrEmpty(Function.ToString(cbVatGourpSa.EditValue)))
			{
				itemsImpl.VatGourpSa = Function.ToString(cbVatGourpSa.EditValue);
			}
			switch (Function.ParseInt(cbManageItemBy.EditValue))
			{
			case 2:
				itemsImpl.ManBtchNum = "Y";
				break;
			case 1:
				itemsImpl.ManSerNum = "Y";
				break;
			}
			itemsImpl.ItemCode = itemCode;
			items_LineImpl.ItemCode = itemCode;
			itemsImpl.ItemName = Function.ToString(txtDescription.Text);
			itemsImpl.FrgnName = Function.ToString(txtFrgnName.Text);
			itemsImpl.ItemType = itemType;
			items_LineImpl.PriceList = Function.ParseInt(idPrList);
			itemsImpl.Lines = items_LineImpl;
			itemsImpl.BPLId = GlobalData.DefaultBranch;
			if (Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue) != 0)
			{
				itemsImpl.UgpEntry = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			}
			DataTable dataTable = grd_Udf.DataSource as DataTable;
			if (dataTable == null)
			{
				dataTable = new DataTable();
			}
			WinCommons._OpenProcessing();
			Tuple<ItemsImpl, DataTable> doc = new Tuple<ItemsImpl, DataTable>(itemsImpl, dataTable);
			DataTable dataTable2 = await _httpClient.Items.Search(doc);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (dataTable2.Rows.Count > 1)
				{
					frm_SearchDataList frm_SearchDataList2 = new frm_SearchDataList
					{
						SourceDt = dataTable2,
						XFormId = "OITM_SR",
						ItemId = "GV_CFL",
						Text = "Item Master Data List",
						ObjType = ApzObjectTypes.oItems
					};
					frm_SearchDataList2.ShowDialog(this);
					if (frm_SearchDataList2.ChooseItem != null)
					{
						GetADoc(Function.ToString(frm_SearchDataList2.ChooseItem["ItemCode"]));
					}
					else
					{
						btOK.Enabled = true;
					}
				}
				else
				{
					GetADoc(Function.ToString(dataTable2.Rows[0]["ItemCode"]));
				}
			}
			else
			{
				btOK.Enabled = true;
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			btOK.Enabled = true;
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void GetADoc(string itemCode)
	{
		try
		{
			DataTable aDoc = _httpClient.Items.GetADoc(itemCode);
			if (aDoc != null && aDoc.Rows.Count > 0)
			{
				LoadDataRecord(aDoc);
				return;
			}
			REFRESH();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		try
		{
			lb_ItemCode.Utext = Resources.OITM_ItemCode;
			lb_Description.Utext = Resources.OITM_ItemName;
			lb_BarCode.Utext = Resources.OITM_CodeBars;
			lb_DescForLang.Utext = Resources.OITM_FrgnName;
			lb_ItemType.Utext = Resources.OITM_ItemType;
			lb_ItemGroup.Utext = Resources.OITM_ItmsGrpCod;
			lb_PriceList.Utext = Resources.ITM1_PriceList;
			lb_UnitPrice.Utext = Resources.ITM1_Price;
			chkInvntItem.Text = Resources.OITM_InvntItem;
			chkPrchseItem.Text = Resources.OITM_PrchseItem;
			chkSellItem.Text = Resources.OITM_SellItem;
			lb_Manufacture.Utext = Resources.OITM_FirmCode;
			lb_ShippingType.Utext = Resources.OITM_ShipType;
			lb_SerialAndBatchNum.Utext = Resources.OITM_SerialAndBatchNum;
			lb_ManageItemBy.Utext = Resources.OITM_ManageItemBy;
			lb_ManagementMethod.Utext = Resources.OITM_MngMethod;
			lbVailFrom.Utext = Resources.OITM_VailFrom;
			lbVailTo.Utext = Resources.OITM_VailTo;
			lbRemarks.Utext = Resources.OITM_ValidComm;
			lb_F.Utext = Resources.OITM_FrozenFrom;
			lb_T.Utext = Resources.OITM_FrozenTo;
			lb_R.Utext = Resources.OITM_FrozenComm;
			lb_ActiveRangeFrom.Utext = Resources.OITM_ActiveRangeFrom;
			lb_InactiveRangeFrom.Utext = Resources.OITM_InactiveRangeFrom;
			lb_ToActi.Utext = Resources.OITM_lb_ToActi;
			lb_RActi.Utext = Resources.OITM_lb_Racti;
			lb_ToInacti.Utext = Resources.OITM_lb_ToInacti;
			lb_RIacti.Utext = Resources.OITM_lb_Riacti;
			lb_PreferredVendor.Utext = Resources.OITM_CardCode;
			lb_CatalogNo.Utext = Resources.OITM_SuppCatNum;
			lb_PurchasingUoM.Utext = Resources.OITM_BuyUnitMsr;
			lb_ItemsPerPurchaseUnit.Utext = Resources.OITM_NumInBuy;
			lb_Purchasing.Utext = Resources.OITM_PurPackMsr;
			lb_PackagingUoM.Utext = Resources.OITM_PurPackUn;
			lb_TaxGroup.Utext = Resources.OITM_VatGroupPu;
			lb_Factor1.Utext = Resources.OITM_PurFactor1;
			lb_Factor2.Utext = Resources.OITM_PurFactor2;
			lb_Factor3.Utext = Resources.OITM_PurFactor3;
			lb_Factor4.Utext = Resources.OITM_PurFactor4;
			lb_ItemsPerSalesUnit.Utext = Resources.OITM_NumInSale;
			lb_Sales.Utext = Resources.OITM_SalPackMsr;
			lb_PackagingUoMS.Utext = Resources.OITM_SalPackUn;
			lb_TaxGroupS.Utext = Resources.OITM_VatGourpSa;
			lb_Factor1S.Utext = Resources.OITM_SalFactor1;
			lb_Factor2S.Utext = Resources.OITM_SalFactor2;
			lb_Factor3S.Utext = Resources.OITM_SalFactor3;
			lb_Factor4S.Utext = Resources.OITM_SalFactor4;
			lb_AccBy.Utext = Resources.OITM_GLMethods;
			lb_ValuationMethod.Utext = Resources.OITM_EvalSystem;
			lbItemCost.Utext = Resources.OITM_AvgPrice;
			chkByWh.Text = Resources.OITM_ByWh;
			lb_Minimum.Utext = Resources.OITM_MinLevel;
			lb_Maximum.Utext = Resources.OITM_MaxLevel;
			gcWhseCode.Caption = Resources.OITW_gcWhseCode;
			gcWhseName.Caption = Resources.OITW_gcWhseName;
			gcLocked.Caption = Resources.OITW_gcLocked;
			gcPriceInventory.Caption = Resources.OITW_gcPriceInventory;
			gcOnHand.Caption = Resources.OITW_gcOnHand;
			gcCommited.Caption = Resources.OITW_gcCommited;
			gcOnOrder.Caption = Resources.OITW_gcOnOrder;
			gcAvaiable.Caption = Resources.OITW_gcAvaiable;
			tab_General.Text = Resources.tab_General;
			tab_Purchase.Text = Resources.OITM_tab_Purchase;
			tab_Sales.Text = Resources.OITM_tab_Sales;
			tab_Inventory.Text = Resources.OITM_tab_Inventory;
			tab_Remark.Text = Resources.tab_Remark;
			tab_UserDefineF.Text = Resources.tab_UserDefineF;
			btCancel.Text = Resources.ButtonCancel;
			btOK.Text = Resources.ButtonAdd;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Reset()
	{
		cb_Code.EditValue = 0;
		txtInvntUom.Text = string.Empty;
		txtItemCode.Text = string.Empty;
		txtItemCode.ReadOnly = false;
		txtDescription.Text = string.Empty;
		txtFrgnName.Text = string.Empty;
		cbItemType.EditValue = "";
		cbItemType.EditValue = string.Empty;
		cbPriceList.Enabled = false;
		txtBarCode.Text = string.Empty;
		chkInvntItem.Checked = false;
		chkInvntItem.Enabled = false;
		chkPrchseItem.Checked = false;
		chkPrchseItem.Enabled = false;
		chkSellItem.Checked = false;
		chkSellItem.Enabled = false;
		cbPriceList.Enabled = true;
		cbPriceList.EditValue = -1;
		txtUnitPrice.Enabled = true;
		txtUnitPrice.Text = Function.ParseDouble(price).ToString();
		cbManufacturer.EditValue = -1;
		cbManufacturer.EditValue = string.Empty;
		cbShippingType.EditValue = null;
		dateValidFrom.EditValue = string.Empty;
		dateValidFrom2.EditValue = string.Empty;
		dateValidTo.EditValue = string.Empty;
		dateValidTo2.EditValue = string.Empty;
		txtValidComm.EditValue = string.Empty;
		txtValidComm2.EditValue = string.Empty;
		dateFrozenFrom.EditValue = string.Empty;
		dateFrozenFrom2.EditValue = string.Empty;
		dateFrozenTo.EditValue = string.Empty;
		dateFrozenTo2.EditValue = string.Empty;
		txtFrozenComm.EditValue = string.Empty;
		txtFrozenComm2.EditValue = string.Empty;
		cbManageItemBy.EditValue = "0";
		txtNumInBuy.Text = string.Empty;
		txtPurPackUn.Text = string.Empty;
		txtPurFactor1.Text = string.Empty;
		txtPurFactor2.Text = string.Empty;
		txtPurFactor3.Text = string.Empty;
		txtPurFactor4.Text = string.Empty;
		cbVatGroupPu.EditValue = "";
		calc_VatPercentPurchase.EditValue = 0;
		txtCardCode.Text = string.Empty;
		txtSuppCatNum.Text = string.Empty;
		txtBuyUnitMsr.Text = string.Empty;
		txtPurPackMsr.Text = string.Empty;
		txtNumInSale.Text = string.Empty;
		txtSalPackUn.Text = string.Empty;
		txtSalFactor1.Text = string.Empty;
		txtSalFactor2.Text = string.Empty;
		txtSalFactor3.Text = string.Empty;
		txtSalFactor4.Text = string.Empty;
		cbVatGourpSa.EditValue = "";
		calc_VatPercentSales.EditValue = 0;
		txtSalUnitMsr.Text = string.Empty;
		txtSalPackMsr.Text = string.Empty;
		txtUserText.Text = string.Empty;
		txtReorderQty.EditValue = 0;
		txtMinLevel.EditValue = 0;
		txtMaxLevel.EditValue = 0;
		DataTable dt = new DataTable();
		if (WinFuntions.GetTableStruct(ref dt, "OITW", _lstStructDt))
		{
			grd_WareHouse.DataSource = dt;
		}
		else
		{
			grd_WareHouse.DataSource = null;
		}
		grd_WareHouse.RefreshDataSource();
		grd_Properties.DataSource = null;
		grd_Properties.RefreshDataSource();
		luk_UoMGroup_UgpEntry.EditValue = Function.ParseShort(-1);
		grd_Udf.DataSource = null;
		grd_Udf.RefreshDataSource();
		if (GlobalData.IsMultiBranch)
		{
			chkByWh.Checked = true;
			chkByWh.Enabled = false;
		}
	}

	private async Task RefreshData()
	{
		Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["OITM"] = gv_Udf };
		await WinFuntions.LoadUdfColumns(gv_Udf, "OITM", tblFormular);
		List<Formular> formulars = await WinFuntions.GetFormulas(4);
		gv_Udf.InitFormular(formulars, tblFormular);
		_isLoading = true;
		lbl_PriceListUoM.Utext = "";
		if (IsAssetMasterData)
		{
			chkInvntItem.Enabled = false;
			chkInvntItem.Checked = false;
		}
		lb_ComponentWarehouse.Visible = false;
		cb_ComponentWarehouse.Visible = false;
		cb_Code.EditValue = 0;
		cbManageItemBy.EditValue = "0";
		if (IsAssetMasterData)
		{
			cbItemType.EditValue = "F";
		}
		else
		{
			cbItemType.EditValue = "I";
		}
		ChangePropetiesItemWhenChangeItemType();
		txtUnitPrice.EditValue = Function.ParseDouble(price);
		txtUnitPrice.Enabled = true;
		cbPriceList.Enabled = true;
		LoadCombo();
		LoadCheckBox();
		LoadGroupPurchasing();
		LoadGroupSales();
		LoadGroupInventory();
		if (flag_SearchByItemCodfrmSpecialBP)
		{
			await Search();
			_isLoading = true;
		}
		else if (flag_SearchByItemCode)
		{
			GetADoc(_itemCode);
			_isLoading = true;
		}
		else
		{
			await REFRESH();
			_isLoading = true;
			rdg_Active.SelectedIndex = 0;
			panel_Inactive.Visible = false;
			panel_Advanced.Visible = false;
		}
		if (GlobalData.IsMultiBranch)
		{
			chkByWh.Checked = true;
			chkByWh.Enabled = false;
		}
		if (FormAuth == Enums.FormAuthorities.Full)
		{
			if (flag_SearchByItemCode || flag_SearchByItemCodfrmSpecialBP)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		else if (flag_SearchByItemCode || flag_SearchByItemCodfrmSpecialBP)
		{
			FormMode = Enums.FormViewModeType.View;
		}
		else
		{
			FormMode = Enums.FormViewModeType.Find;
		}
		if (!GlobalData.LoginSap)
		{
			lb_ItemType.Visible = false;
			cbItemType.Visible = false;
			lbl_PUoMEntry.Visible = false;
			bte_PUoMEntry.Visible = false;
			btn_PUoMForm.Visible = false;
			lbl_NumInBuy.Visible = false;
			lb_Purchasing.Visible = false;
			txtPurPackMsr.Visible = false;
			lb_PackagingUoM.Visible = false;
			txtPurPackUn.Visible = false;
			lb_Factor1.Visible = false;
			txtPurFactor1.Visible = false;
			lb_Factor2.Visible = false;
			txtPurFactor2.Visible = false;
			lb_Factor3.Visible = false;
			txtPurFactor3.Visible = false;
			lb_Factor4.Visible = false;
			txtPurFactor4.Visible = false;
			lbl_SUoMEntry.Visible = false;
			bte_SUoMEntry.Visible = false;
			btn_SUoMForm.Visible = false;
			lbl_NumInSale.Visible = false;
			lb_Sales.Visible = false;
			txtSalPackMsr.Visible = false;
			lb_PackagingUoMS.Visible = false;
			txtSalPackUn.Visible = false;
			lb_Factor1S.Visible = false;
			txtSalFactor1.Visible = false;
			lb_Factor2S.Visible = false;
			txtSalFactor2.Visible = false;
			lb_Factor3S.Visible = false;
			txtSalFactor3.Visible = false;
			lb_Factor4S.Visible = false;
			txtSalFactor4.Visible = false;
			lb_AccBy.Visible = false;
			cbGLMethod.Visible = false;
			lbl_IUoMEntry.Visible = false;
			bte_IUoMEntry.Visible = false;
			lbl_INUoMEntry.Visible = false;
			bte_INUoMEntry.Visible = false;
			lbl_CntUnitMsr.Visible = false;
			txt_CntUnitMsr.Visible = false;
			lbl_NumInCnt1.Visible = false;
			calc_NumInCnt.Visible = false;
			lbl_NumInCnt.Visible = false;
			luk_UoMGroup_UgpEntry.Enabled = false;
		}
		else
		{
			lb_ItemType.Visible = true;
			cbItemType.Visible = true;
			lbl_PUoMEntry.Visible = true;
			bte_PUoMEntry.Visible = true;
			btn_PUoMForm.Visible = true;
			lbl_NumInBuy.Visible = true;
			lb_Purchasing.Visible = true;
			txtPurPackMsr.Visible = true;
			lb_PackagingUoM.Visible = true;
			txtPurPackUn.Visible = true;
			lb_Factor1.Visible = true;
			txtPurFactor1.Visible = true;
			lb_Factor2.Visible = true;
			txtPurFactor2.Visible = true;
			lb_Factor3.Visible = true;
			txtPurFactor3.Visible = true;
			lb_Factor4.Visible = true;
			txtPurFactor4.Visible = true;
			lbl_SUoMEntry.Visible = true;
			bte_SUoMEntry.Visible = true;
			btn_SUoMForm.Visible = true;
			lbl_NumInSale.Visible = true;
			lb_Sales.Visible = true;
			txtSalPackMsr.Visible = true;
			lb_PackagingUoMS.Visible = true;
			txtSalPackUn.Visible = true;
			lb_Factor1S.Visible = true;
			txtSalFactor1.Visible = true;
			lb_Factor2S.Visible = true;
			txtSalFactor2.Visible = true;
			lb_Factor3S.Visible = true;
			txtSalFactor3.Visible = true;
			lb_Factor4S.Visible = true;
			txtSalFactor4.Visible = true;
			lb_AccBy.Visible = true;
			cbGLMethod.Visible = true;
			lbl_IUoMEntry.Visible = true;
			bte_IUoMEntry.Visible = true;
			lbl_INUoMEntry.Visible = true;
			bte_INUoMEntry.Visible = true;
			lbl_CntUnitMsr.Visible = true;
			txt_CntUnitMsr.Visible = true;
			lbl_NumInCnt1.Visible = true;
			calc_NumInCnt.Visible = true;
			lbl_NumInCnt.Visible = true;
			luk_UoMGroup_UgpEntry.Enabled = true;
			SetControlsUoM(Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue));
		}
		_dtPriceList = _httpClient.Items.GetPriceList();
		_isLoading = false;
	}

	private void EventCbEvalSystem()
	{
		try
		{
			switch (Function.ToString(cbEvalSystem.EditValue))
			{
			case "A":
				txtAvgPrice.Visible = false;
				lbItemCost.Visible = false;
				txtAvgPrice.Enabled = false;
				break;
			case "S":
				txtAvgPrice.Visible = true;
				lbItemCost.Visible = true;
				txtAvgPrice.Enabled = true;
				break;
			case "F":
				txtAvgPrice.Visible = false;
				lbItemCost.Visible = false;
				break;
			case "B":
			{
				txtAvgPrice.Visible = false;
				lbItemCost.Visible = false;
				bool isLoading = _isLoading;
				_isLoading = true;
				cbManagementMethod.EditValue = "A";
				_isLoading = isLoading;
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

	private void ShowData_CbEvalSystem()
	{
		try
		{
			string text = Function.ToString(cbManageItemBy.EditValue);
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			if (text == "0")
			{
				if (_dtEvalSystem == null)
				{
					return;
				}
				DataTable dataTable = _dtEvalSystem.Copy();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == "B");
					if (dataRow != null)
					{
						dataTable.Rows.Remove(dataRow);
						dataTable.AcceptChanges();
						cbEvalSystem.Properties.DataSource = dataTable;
					}
				}
			}
			else
			{
				cbEvalSystem.Properties.DataSource = _dtEvalSystem.Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowManagementMethod()
	{
		string text = Function.ToString(cbManageItemBy.EditValue);
		if (text == "1" || text == "2")
		{
			lb_ManagementMethod.Visible = true;
			cbManagementMethod.Visible = true;
			luk_IssuePriBy.Visible = true;
			lbl_IssuePriBy.Visible = true;
			string text2 = Function.ToString(cbManagementMethod.EditValue);
			if (text == "1")
			{
				lbl_ServiceAttr.Visible = true;
				lbl_WarrntyTpl.Visible = true;
				bte_WarrntTmpl.Visible = true;
				if (text2 == "R")
				{
					chk_ManOutOnly.Visible = true;
				}
				else
				{
					chk_ManOutOnly.Visible = false;
				}
			}
			else
			{
				lbl_ServiceAttr.Visible = false;
				lbl_WarrntyTpl.Visible = false;
				bte_WarrntTmpl.Visible = false;
				chk_ManOutOnly.Visible = false;
			}
		}
		else
		{
			lb_ManagementMethod.Visible = false;
			cbManagementMethod.Visible = false;
			chk_ManOutOnly.Visible = false;
			luk_IssuePriBy.Visible = false;
			lbl_IssuePriBy.Visible = false;
			lbl_ServiceAttr.Visible = false;
			lbl_WarrntyTpl.Visible = false;
			bte_WarrntTmpl.Visible = false;
		}
	}

	private void GetRecord(string typeRecord, string itmCode)
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			_dtPUom.Rows.Clear();
			_dtPUom.AcceptChanges();
			_dtSUom.Rows.Clear();
			_dtSUom.AcceptChanges();
			DataTable dt = _httpClient.Items.GetRecord(itmCode, typeRecord, GlobalData.DefaultBranch, IsAssetMasterData);
			if (dt != null && dt.Rows.Count > 0)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					LoadDataRecord(dt);
				}));
				return;
			}
			BeginInvoke((MethodInvoker)(() =>
			{
				FormMode = Enums.FormViewModeType.Find;
				txtItemCode.Enabled = true;
				txtItemCode.ReadOnly = false;
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeButtonOKText(object sender)
	{
		try
		{
			if (!_isLoading && !_isChangingMode && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		txtNumInBuy.Properties.Mask.MaskType = MaskType.Numeric;
		txtNumInBuy.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtNumInBuy.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtPurPackUn.Properties.Mask.MaskType = MaskType.Numeric;
		txtPurPackUn.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtPurPackUn.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtPurFactor1.Properties.Mask.MaskType = MaskType.Numeric;
		txtPurFactor1.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtPurFactor1.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtPurFactor2.Properties.Mask.MaskType = MaskType.Numeric;
		txtPurFactor2.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtPurFactor2.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtPurFactor3.Properties.Mask.MaskType = MaskType.Numeric;
		txtPurFactor3.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtPurFactor3.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtPurFactor4.Properties.Mask.MaskType = MaskType.Numeric;
		txtPurFactor4.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtPurFactor4.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtNumInSale.Properties.Mask.MaskType = MaskType.Numeric;
		txtNumInSale.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtNumInSale.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtSalPackUn.Properties.Mask.MaskType = MaskType.Numeric;
		txtSalPackUn.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtSalPackUn.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtSalFactor1.Properties.Mask.MaskType = MaskType.Numeric;
		txtSalFactor1.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtSalFactor1.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtSalFactor2.Properties.Mask.MaskType = MaskType.Numeric;
		txtSalFactor2.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtSalFactor2.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtSalFactor3.Properties.Mask.MaskType = MaskType.Numeric;
		txtSalFactor3.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtSalFactor3.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtSalFactor4.Properties.Mask.MaskType = MaskType.Numeric;
		txtSalFactor4.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txtSalFactor4.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtAvgPrice.Properties.Mask.MaskType = MaskType.Numeric;
		txtAvgPrice.Properties.Mask.EditMask = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
		txtAvgPrice.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtMinLevel.Properties.Mask.MaskType = MaskType.Numeric;
		txtMinLevel.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtMinLevel.Properties.Mask.UseMaskAsDisplayFormat = true;
		txtMaxLevel.Properties.Mask.MaskType = MaskType.Numeric;
		txtMaxLevel.Properties.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		txtMaxLevel.Properties.Mask.UseMaskAsDisplayFormat = true;
		gv_WareHouse.Columns["AvgPrice"].SummaryItem.DisplayFormat = "{0:n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits + "}";
		gv_WareHouse.Columns["OnHand"].SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		gv_WareHouse.Columns["IsCommited"].SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		gv_WareHouse.Columns["OnOrder"].SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		gv_WareHouse.Columns["Avaiable"].SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		txt_OrderMultiple.Properties.Mask.MaskType = MaskType.Numeric;
		txt_OrderMultiple.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txt_OrderMultiple.Properties.Mask.UseMaskAsDisplayFormat = true;
		txt_MinimumOrderQty.Properties.Mask.MaskType = MaskType.Numeric;
		txt_MinimumOrderQty.Properties.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
		txt_MinimumOrderQty.Properties.Mask.UseMaskAsDisplayFormat = true;
		txt_LeadTime.Properties.Mask.MaskType = MaskType.Numeric;
		txt_LeadTime.Properties.Mask.EditMask = "n0";
		txt_LeadTime.Properties.Mask.UseMaskAsDisplayFormat = true;
		txt_ToleranceDays.Properties.Mask.MaskType = MaskType.Numeric;
		txt_ToleranceDays.Properties.Mask.EditMask = "n0";
		txt_ToleranceDays.Properties.Mask.UseMaskAsDisplayFormat = true;
		string dateTimeFormat = GlobalData.DateTimeFormat;
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
		rep_CalcInventory_Price.Mask.MaskType = MaskType.Numeric;
		rep_CalcInventory_Price.Mask.EditMask = "n" + GlobalData.UnitPriceFormat.NumberDecimalDigits;
		rep_CalcInventory_Price.Mask.UseMaskAsDisplayFormat = true;
		rep_CalcInventory_Quantity.Mask.MaskType = MaskType.Numeric;
		rep_CalcInventory_Quantity.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		rep_CalcInventory_Quantity.Mask.UseMaskAsDisplayFormat = true;
		bte_WarrntTmpl.Properties.RefreshDataSource();
	}

	private void gv_Udf_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!_isLoading && !_isChangingMode && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private List<Tuple<string, string>> GetItemProperties(string itemCode)
	{
		List<Tuple<string, string>> list = new List<Tuple<string, string>>();
		try
		{
			DataSet itemProperties = _httpClient.Items.GetItemProperties(itemCode);
			if (itemProperties != null && itemProperties.Tables.Count == 2 && itemProperties.Tables[0] != null && itemProperties.Tables[1] != null && itemProperties.Tables[0].Rows.Count > 0 && itemProperties.Tables[0].Columns.Count == itemProperties.Tables[1].Rows.Count)
			{
				DataTable dataTable = itemProperties.Tables[0];
				DataTable dataTable2 = itemProperties.Tables[1];
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					Tuple<string, string> item = new Tuple<string, string>(Function.ToString(dataTable2.Rows[i][0]), Function.ToString(dataTable.Rows[0][i]));
					list.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	private void tabControl_Detail_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
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

	private void rep_PropertiesCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update && !_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
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

	private void cb_Code_EditValueChanged(object sender, EventArgs e)
	{
		txtDescription.Focus();
	}

	private void LoadItemProperties(List<Tuple<string, string>> lstProperties)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ItmsTypCod", typeof(int));
			dataTable.Columns.Add("ItmsGrpNam", typeof(string));
			dataTable.Columns.Add("Check", typeof(string));
			dataTable.AcceptChanges();
			for (int i = 0; i < lstProperties.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ItmsTypCod"] = i + 1;
				dataRow["ItmsGrpNam"] = lstProperties[i].Item1;
				dataRow["Check"] = lstProperties[i].Item2;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			grd_Properties.DataSource = dataTable;
			grd_Properties.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ExistsItem(string itemCode)
	{
		bool result = false;
		try
		{
			DataTable dataTable = _httpClient.Items.CheckExistItem(itemCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void UpdateNumInCount(string type)
	{
		try
		{
			int invMEntry = Function.ParseInt(bte_IUoMEntry.EditValue);
			int cntEntry = Function.ParseInt(bte_INUoMEntry.EditValue);
			int pCntEntry = Function.ParseInt(bte_PUoMEntry.EditValue);
			int sCntEntry = Function.ParseInt(bte_SUoMEntry.EditValue);
			int ugpEntry = Function.ParseInt(luk_UoMGroup_UgpEntry.EditValue);
			DataTable uoMByUomGroup = GetUoMByUomGroup(ugpEntry);
			if (uoMByUomGroup == null || uoMByUomGroup.Rows.Count <= 0)
			{
				return;
			}
			DataRow dataRow = uoMByUomGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == invMEntry);
			if (type == "A" || type == "C")
			{
				DataRow dataRow2 = uoMByUomGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == cntEntry);
				if (dataRow != null && dataRow2 != null)
				{
					double num = Function.ParseDouble(dataRow["AltQty"]);
					double num2 = Function.ParseDouble(dataRow["BaseQty"]);
					double num3 = Function.ParseDouble(dataRow2["AltQty"]);
					double num4 = Function.ParseDouble(dataRow2["BaseQty"]);
					calc_NumInCnt.EditValue = num4 * num / (num3 * num2);
				}
			}
			if (type == "A" || type == "B")
			{
				DataRow dataRow3 = uoMByUomGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == pCntEntry);
				if (dataRow != null && dataRow3 != null)
				{
					double num5 = Function.ParseDouble(dataRow["AltQty"]);
					double num6 = Function.ParseDouble(dataRow["BaseQty"]);
					double num7 = Function.ParseDouble(dataRow3["AltQty"]);
					double num8 = Function.ParseDouble(dataRow3["BaseQty"]);
					txtNumInBuy.EditValue = num8 * num5 / (num7 * num6);
				}
			}
			if (type == "A" || type == "S")
			{
				DataRow dataRow4 = uoMByUomGroup.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["UomEntry"]) == sCntEntry);
				if (dataRow != null && dataRow4 != null)
				{
					double num9 = Function.ParseDouble(dataRow["AltQty"]);
					double num10 = Function.ParseDouble(dataRow["BaseQty"]);
					double num11 = Function.ParseDouble(dataRow4["AltQty"]);
					double num12 = Function.ParseDouble(dataRow4["BaseQty"]);
					txtNumInSale.EditValue = num12 * num9 / (num11 * num10);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetUoMByUomGroup(int ugpEntry)
	{
		try
		{
			DataTable uoMByUomGroup = _httpClient.Items.GetUoMByUomGroup(ugpEntry);
			if (uoMByUomGroup != null && uoMByUomGroup.Rows.Count > 0)
			{
				return uoMByUomGroup;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void LoadBaseUoM(int ugrpEntry)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (ugrpEntry <= 0)
			{
				bte_IUoMEntry.EditValue = 0;
				txtInvntUom.Text = "";
				return;
			}
			DataTable dataTable = _httpClient.Items.LoadBaseUoM(ugrpEntry);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				bte_IUoMEntry.EditValue = Function.ParseInt(dataTable.Rows[0]["UomEntry"]);
				txtInvntUom.Text = Function.ToString(dataTable.Rows[0]["UomName"]);
			}
			else
			{
				bte_IUoMEntry.EditValue = 0;
				txtInvntUom.Text = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetControlsUoM(int val)
	{
		try
		{
			if (val <= 0)
			{
				btn_CopyToSalesData.Visible = true;
				bte_PUoMEntry.Visible = false;
				bte_SUoMEntry.Visible = false;
				txtBuyUnitMsr.Enabled = true;
				txtNumInBuy.Enabled = true;
				txtSalUnitMsr.Enabled = true;
				txtNumInSale.Enabled = true;
				if (!GlobalData.LoginSap)
				{
					lb_Factor1S.Visible = false;
					lb_Factor2S.Visible = false;
					lb_Factor3S.Visible = false;
					lb_Factor4S.Visible = false;
					lb_Factor1.Visible = false;
					lb_Factor2.Visible = false;
					lb_Factor3.Visible = false;
					lb_Factor4.Visible = false;
					txtPurFactor1.Visible = false;
					txtPurFactor2.Visible = false;
					txtPurFactor3.Visible = false;
					txtPurFactor4.Visible = false;
					txtSalFactor1.Visible = false;
					txtSalFactor2.Visible = false;
					txtSalFactor3.Visible = false;
					txtSalFactor4.Visible = false;
				}
				else
				{
					lb_Factor1S.Visible = true;
					lb_Factor2S.Visible = true;
					lb_Factor3S.Visible = true;
					lb_Factor4S.Visible = true;
					lb_Factor1.Visible = true;
					lb_Factor2.Visible = true;
					lb_Factor3.Visible = true;
					lb_Factor4.Visible = true;
					txtPurFactor1.Visible = true;
					txtPurFactor2.Visible = true;
					txtPurFactor3.Visible = true;
					txtPurFactor4.Visible = true;
					txtSalFactor1.Visible = true;
					txtSalFactor2.Visible = true;
					txtSalFactor3.Visible = true;
					txtSalFactor4.Visible = true;
				}
				calc_BWidth1.Enabled = true;
				calc_BLength1.Enabled = true;
				calc_BWeight1.Enabled = true;
				calc_BHeight1.Enabled = true;
				calc_BVolume.Enabled = true;
				luk_BVolUnit.Enabled = true;
				calc_SWidth1.Enabled = true;
				calc_SLength1.Enabled = true;
				calc_SWeight1.Enabled = true;
				calc_SHeight1.Enabled = true;
				calc_SVolume.Enabled = true;
				luk_SVolUnit.Enabled = true;
				bte_IUoMEntry.Visible = false;
				bte_INUoMEntry.Visible = false;
				txt_CntUnitMsr.Visible = false;
				calc_NumInCnt.Visible = false;
				lbl_NumInCnt.Visible = false;
				lbl_NumInCnt1.Visible = false;
				lbl_CntUnitMsr.Visible = false;
				lbl_INUoMEntry.Visible = false;
				lbl_IUoMEntry.Visible = false;
				lbl_SUoMEntry.Visible = false;
				lbl_NumInSale.Visible = false;
				lbl_NumInCnt.Visible = false;
				lbl_NumInBuy.Visible = false;
				lbl_PUoMEntry.Visible = false;
				lbl_PriceListUoM.Visible = false;
				btn_PUoMForm.Visible = false;
				btn_SUoMForm.Visible = false;
				txtInvntUom.Enabled = true;
			}
			else
			{
				btn_CopyToSalesData.Visible = false;
				bte_PUoMEntry.Visible = true;
				bte_SUoMEntry.Visible = true;
				txtBuyUnitMsr.Enabled = false;
				txtNumInBuy.Enabled = false;
				txtSalUnitMsr.Enabled = false;
				txtNumInSale.Enabled = false;
				lb_Factor1S.Visible = false;
				lb_Factor2S.Visible = false;
				lb_Factor3S.Visible = false;
				lb_Factor4S.Visible = false;
				lb_Factor1.Visible = false;
				lb_Factor2.Visible = false;
				lb_Factor3.Visible = false;
				lb_Factor4.Visible = false;
				txtPurFactor1.Visible = false;
				txtPurFactor2.Visible = false;
				txtPurFactor3.Visible = false;
				txtPurFactor4.Visible = false;
				txtSalFactor1.Visible = false;
				txtSalFactor2.Visible = false;
				txtSalFactor3.Visible = false;
				txtSalFactor4.Visible = false;
				if (Function.ParseInt(bte_PUoMEntry.EditValue) <= 0)
				{
					calc_BWidth1.Enabled = false;
					calc_BLength1.Enabled = false;
					calc_BWeight1.Enabled = false;
					calc_BHeight1.Enabled = false;
					calc_BVolume.Enabled = false;
					luk_BVolUnit.Enabled = false;
				}
				else
				{
					calc_BWidth1.Enabled = true;
					calc_BLength1.Enabled = true;
					calc_BWeight1.Enabled = true;
					calc_BHeight1.Enabled = true;
					calc_BVolume.Enabled = true;
					luk_BVolUnit.Enabled = true;
				}
				if (Function.ParseInt(bte_SUoMEntry.EditValue) <= 0)
				{
					calc_SWidth1.Enabled = false;
					calc_SLength1.Enabled = false;
					calc_SWeight1.Enabled = false;
					calc_SHeight1.Enabled = false;
					calc_SVolume.Enabled = false;
					luk_SVolUnit.Enabled = false;
				}
				else
				{
					calc_SWidth1.Enabled = true;
					calc_SLength1.Enabled = true;
					calc_SWeight1.Enabled = true;
					calc_SHeight1.Enabled = true;
					calc_SVolume.Enabled = true;
					luk_SVolUnit.Enabled = true;
				}
				bte_IUoMEntry.Visible = true;
				bte_INUoMEntry.Visible = true;
				txt_CntUnitMsr.Visible = true;
				calc_NumInCnt.Visible = true;
				lbl_NumInCnt.Visible = true;
				txtInvntUom.Enabled = false;
				lbl_NumInCnt1.Visible = true;
				lbl_CntUnitMsr.Visible = true;
				lbl_INUoMEntry.Visible = true;
				lbl_IUoMEntry.Visible = true;
				lbl_SUoMEntry.Visible = true;
				lbl_NumInSale.Visible = true;
				lbl_NumInCnt.Visible = true;
				lbl_NumInBuy.Visible = true;
				lbl_PUoMEntry.Visible = true;
				lbl_PriceListUoM.Visible = true;
				btn_PUoMForm.Visible = true;
				btn_SUoMForm.Visible = true;
			}
			LoadUoMEntryLookUpEdits(val, txtItemCode.Text);
			LoadBaseUoM(val);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CFLUomName(ButtonEdit sender)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUoMNameItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				sender.EditValue = Function.ToString(frm_CFL2.ChooseItem["UomName"]);
				ChangeButtonOKText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReloadDataUoMSource(LookUpEdit luk, DataTable dt)
	{
		try
		{
			luk.Properties.Columns.Clear();
			luk.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			luk.Properties.DisplayMember = "UomCode";
			luk.Properties.ValueMember = "UomEntry";
			luk.Properties.NullText = "";
			luk.Properties.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetSourcePreferredVendor()
	{
		DataTable dt = new DataTable();
		if (!WinFuntions.GetTableStruct(ref dt, "ITM2", _lstStructDt))
		{
			return new DataTable();
		}
		dt.Rows.Clear();
		dt.AcceptChanges();
		try
		{
			string dflVendor = txtCardCode.Text.Trim();
			if (!string.IsNullOrEmpty(dflVendor))
			{
				if (_dtPreferredVendor == null || _dtPreferredVendor.Rows.Count == 0)
				{
					_dtPreferredVendor = _httpClient.Items.GetPreferredVendor("-1");
				}
				if (!_dtPreferredVendor.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["VendorCode"]), dflVendor, ignoreCase: true) == 0))
				{
					DataRow dataRow = _dtPreferredVendor.NewRow();
					dataRow["ItemCode"] = Function.ToString(txtItemCode.Text);
					dataRow["VendorCode"] = dflVendor;
					dataRow["LogInstanc"] = 0;
					dataRow["ObjType"] = "4";
					_dtPreferredVendor.Rows.Add(dataRow);
					_dtPreferredVendor.AcceptChanges();
				}
			}
			if (_dtPreferredVendor != null && _dtPreferredVendor.Rows.Count > 0)
			{
				for (int num = 0; num < _dtPreferredVendor.Rows.Count; num++)
				{
					string value = Function.ToString(_dtPreferredVendor.Rows[num]["VendorCode"]);
					if (!string.IsNullOrEmpty(value))
					{
						DataRow dataRow2 = dt.NewRow();
						dataRow2["ItemCode"] = Function.ToString(txtItemCode.Text);
						dataRow2["VendorCode"] = value;
						dataRow2["LogInstanc"] = 0;
						dataRow2["ObjType"] = "4";
						dt.Rows.Add(dataRow2);
					}
				}
				dt.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private async Task SetDictionaryControls()
	{
		_dicControls = new Dictionary<string, Control>();
		_dicControls.Add("OITM.Series", cb_Code);
		_dicControls.Add("OITM.ItemCode", txtItemCode);
		_dicControls.Add("OITM.CodeBars", txtBarCode);
		_dicControls.Add("OITM.ItemName", txtDescription);
		_dicControls.Add("OITM.FrgnName", txtFrgnName);
		_dicControls.Add("OITM.ItemType", cbItemType);
		_dicControls.Add("OITM.ItmsGrpCod", cbItemGroup);
		_dicControls.Add("OITM.UserText", txtUserText);
		_dicControls.Add("OITM.UgpEntry", luk_UoMGroup_UgpEntry);
		_dicControls.Add("OITM.FirmCode", cbManufacturer);
		_dicControls.Add("OITM.ShipType", cbShippingType);
		_dicControls.Add("OITM.MngMethod", cbManagementMethod);
		_dicControls.Add("OITM.WarrntTmpl", bte_WarrntTmpl);
		_dicControls.Add("OITM.IssuePriBy", luk_IssuePriBy);
		_dicControls.Add("OITM.ManOutOnly", chk_ManOutOnly);
		_dicControls.Add("OITM.OneBOneRec", chk_OneBOneRec);
		_dicControls.Add("OITM.validTo", dateValidTo);
		_dicControls.Add("OITM.validFrom", dateValidFrom);
		_dicControls.Add("OITM.frozenTo", dateFrozenTo);
		_dicControls.Add("OITM.frozenFrom", dateFrozenFrom);
		_dicControls.Add("OITM.ValidComm", txtValidComm);
		_dicControls.Add("OITM.FrozenComm", txtFrozenComm);
		_dicControls.Add("OITM.CardCode", txtCardCode);
		_dicControls.Add("OITM.SuppCatNum", txtSuppCatNum);
		_dicControls.Add("OITM.BuyUnitMsr", txtBuyUnitMsr);
		_dicControls.Add("OITM.NumInBuy", txtNumInBuy);
		_dicControls.Add("OITM.PurPackMsr", txtPurPackMsr);
		_dicControls.Add("OITM.PurPackUn", txtPurPackUn);
		_dicControls.Add("OITM.VatGroupPu", cbVatGroupPu);
		_dicControls.Add("OITM.PurFactor1", txtPurFactor1);
		_dicControls.Add("OITM.PurFactor2", txtPurFactor2);
		_dicControls.Add("OITM.PurFactor3", txtPurFactor3);
		_dicControls.Add("OITM.PurFactor4", txtPurFactor4);
		_dicControls.Add("OITM.PUoMEntry", bte_PUoMEntry);
		_dicControls.Add("OITM.BLength1", calc_BLength1);
		_dicControls.Add("OITM.BWidth1", calc_BWidth1);
		_dicControls.Add("OITM.BHeight1", calc_BHeight1);
		_dicControls.Add("OITM.BVolume", calc_BVolume);
		_dicControls.Add("OITM.BVolUnit", luk_BVolUnit);
		_dicControls.Add("OITM.BWeight1", calc_BWeight1);
		_dicControls.Add("OITM.CstGrpCode", luk_CstGrpCode);
		_dicControls.Add("OITM.SalUnitMsr", txtSalUnitMsr);
		_dicControls.Add("OITM.NumInSale", txtNumInSale);
		_dicControls.Add("OITM.SalPackMsr", txtSalPackMsr);
		_dicControls.Add("OITM.SalPackUn", txtSalPackUn);
		_dicControls.Add("OITM.VatGourpSa", cbVatGourpSa);
		_dicControls.Add("OITM.SalFactor1", txtSalFactor1);
		_dicControls.Add("OITM.SalFactor2", txtSalFactor2);
		_dicControls.Add("OITM.SalFactor3", txtSalFactor3);
		_dicControls.Add("OITM.SalFactor4", txtSalFactor4);
		_dicControls.Add("OITM.SUoMEntry", bte_SUoMEntry);
		_dicControls.Add("OITM.SLength1", calc_SLength1);
		_dicControls.Add("OITM.SWidth1", calc_SWidth1);
		_dicControls.Add("OITM.SHeight1", calc_SHeight1);
		_dicControls.Add("OITM.SVolume", calc_SVolume);
		_dicControls.Add("OITM.SVolUnit", luk_SVolUnit);
		_dicControls.Add("OITM.SWeight1", calc_SWeight1);
		_dicControls.Add("OITM.GLMethod", cbGLMethod);
		_dicControls.Add("OITM.IUoMEntry", bte_IUoMEntry);
		_dicControls.Add("OITM.InvntryUom", txtInvntUom);
		_dicControls.Add("OITM.IWeight1", calc_IWeight1);
		_dicControls.Add("OITM.INUoMEntry", bte_INUoMEntry);
		_dicControls.Add("OITM.CntUnitMsr", txt_CntUnitMsr);
		_dicControls.Add("OITM.NumInCnt", calc_NumInCnt);
		_dicControls.Add("OITM.EvalSystem", cbEvalSystem);
		_dicControls.Add("OITM.AvgPrice", txtAvgPrice);
		_dicControls.Add("OITM.ReorderQty", txtReorderQty);
		_dicControls.Add("OITM.MinLevel", txtMinLevel);
		_dicControls.Add("OITM.MaxLevel", txtMaxLevel);
		_dicControls.Add("OITM.PlaningSys", cb_PlanningMethod);
		_dicControls.Add("OITM.PrcrmntMtd", cb_ProcurementMethod);
		_dicControls.Add("OITM.CompoWH", cb_ComponentWarehouse);
		_dicControls.Add("OITM.OrdrIntrvl", cb_OrderInterval);
		_dicControls.Add("OITM.OrdrMulti", txt_OrderMultiple);
		_dicControls.Add("OITM.MinOrdrQty", txt_MinimumOrderQty);
		_dicControls.Add("OITM.LeadTime", txt_LeadTime);
		_dicControls.Add("OITM.ToleranDay", txt_ToleranceDays);
		_dicControls.Add("OITM.IssueMthd", luk_IssueMthd);
		_dicControls.Add("OITM.PrdStdCst", calc_PrdStdCst);
		await this.SetControlMaxLength(_dicControls);
		foreach (KeyValuePair<string, Control> dicControl in _dicControls)
		{
			dicControl.Value.Tag = dicControl.Key.Replace(".", ", ");
		}
		ucAttachment1.ButtonBrowse_Click += attachment_EditValueChanged;
		ucAttachment1.ButtonDelete_Click += attachment_EditValueChanged;
		ucAttachment1.gv_File.CellValueChanged += (object sender, CellValueChangedEventArgs e) =>
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		};
	}

	private void attachment_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private object TableToEntity(string table, DataTable dt)
	{
		object result = new object();
		try
		{
			new List<PropertyInfo>();
			switch (table)
			{
			case "OITM":
				result = Function.DataRowToEntity<ItemsImpl>(dt.Rows[0]);
				break;
			case "ITM1":
			{
				List<Items_LineImpl> list3 = new List<Items_LineImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int k = 0; k < dt.Rows.Count; k++)
					{
						Items_LineImpl item3 = Function.DataRowToEntity<Items_LineImpl>(dt.Rows[k]);
						list3.Add(item3);
					}
				}
				result = list3;
				break;
			}
			case "ATC1":
			{
				List<AttachmentFile_Impl> list2 = new List<AttachmentFile_Impl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int j = 0; j < dt.Rows.Count; j++)
					{
						AttachmentFile_Impl item2 = Function.DataRowToEntity<AttachmentFile_Impl>(dt.Rows[j]);
						list2.Add(item2);
					}
				}
				result = list2;
				break;
			}
			case "OITW":
			{
				List<OITW_Impl> list4 = new List<OITW_Impl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					List<string> list5 = (from DataColumn t in dt.Columns
						where t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList();
					for (int num = 0; num < dt.Rows.Count; num++)
					{
						OITW_Impl oITW_Impl = Function.DataRowToEntity<OITW_Impl>(dt.Rows[num]);
						oITW_Impl.UserFields = new List<UserFieldsImpl>();
						for (int num2 = 0; num2 < list5.Count; num2++)
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
							userFieldsImpl.Name = list5[num2];
							userFieldsImpl.Value = dt.Rows[num][list5[num2]];
							Type dataType = dt.Columns[list5[num2]].DataType;
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
							oITW_Impl.UserFields.Add(userFieldsImpl);
						}
						list4.Add(oITW_Impl);
					}
				}
				result = list4;
				break;
			}
			case "ITM12":
			{
				List<Items_UoMImpl> list6 = new List<Items_UoMImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int num3 = 0; num3 < dt.Rows.Count; num3++)
					{
						Items_UoMImpl item4 = Function.DataRowToEntity<Items_UoMImpl>(dt.Rows[num3]);
						list6.Add(item4);
					}
				}
				result = list6;
				break;
			}
			case "ITM2":
			{
				List<ItemPreferredVendorsImpl> list = new List<ItemPreferredVendorsImpl>();
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						ItemPreferredVendorsImpl item = Function.DataRowToEntity<ItemPreferredVendorsImpl>(dt.Rows[i]);
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
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private ItemsImpl GetValueFromDatasource()
	{
		ItemsImpl result = new ItemsImpl();
		try
		{
			if (DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				return (ItemsImpl)TableToEntity("OITM", dataTable.Copy());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private DataTable GetItemLineTableStructure()
	{
		DataTable result = new DataTable();
		try
		{
			result = _httpClient.Items.GetItemLineTableStructure();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void LoadCombo()
	{
		LoadComboCode();
		LoadComboOther("");
	}

	private void LoadComboOther(string controls, string id = "")
	{
		try
		{
			if (string.IsNullOrEmpty(controls))
			{
				LoadStaticCombo();
			}
			if (controls == string.Empty || controls == "ItemGroup")
			{
				short? num = Function.ParseShort(cbItemGroup.EditValue);
				DataTable dataTable = _httpClient.Items.GetItemGroup();
				cbItemGroup.Properties.Columns.Clear();
				cbItemGroup.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "ITMSGRPNAM"
				});
				cbItemGroup.Properties.DisplayMember = "ITMSGRPNAM";
				cbItemGroup.Properties.ValueMember = "ITMSGRPCOD";
				cbItemGroup.Properties.NullText = "";
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					dataTable = new DataTable();
					dataTable.Columns.AddRange(new DataColumn[2]
					{
						new DataColumn("ITMSGRPCOD", typeof(short)),
						new DataColumn("ITMSGRPNAM", typeof(string))
					});
				}
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["ITMSGRPCOD"]) == -1))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ITMSGRPCOD"] = Convert.ToInt16(-1);
					dataRow["ITMSGRPNAM"] = "Define New";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				cbItemGroup.Properties.DataSource = dataTable;
				if (Function.ParseShort(id) > 0)
				{
					num = Function.ParseShort(id);
				}
				_isSettingItemGroup = true;
				cbItemGroup.EditValue = num;
				_isSettingItemGroup = false;
				LoadRelatedItemGroupData();
			}
			if (controls == string.Empty || controls == "UoMGroup")
			{
				DataTable dataTable2 = new DataTable();
				if (GlobalData.LoginSap)
				{
					dataTable2 = _httpClient.Items.GetUoMGroup();
				}
				else
				{
					dataTable2.Columns.Add(new DataColumn("Code", typeof(short)));
					dataTable2.Columns.Add(new DataColumn("Name", typeof(string)));
				}
				if (!dataTable2.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -1))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2[0] = Function.ParseShort(-1);
					dataRow2[1] = "Manual";
					dataTable2.Rows.Add(dataRow2);
					dataTable2.AcceptChanges();
				}
				luk_UoMGroup_UgpEntry.Properties.Columns.Clear();
				luk_UoMGroup_UgpEntry.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "Name"
				});
				luk_UoMGroup_UgpEntry.Properties.DisplayMember = "Name";
				luk_UoMGroup_UgpEntry.Properties.ValueMember = "Code";
				luk_UoMGroup_UgpEntry.Properties.NullText = "";
				luk_UoMGroup_UgpEntry.Properties.DataSource = dataTable2;
				luk_UoMGroup_UgpEntry.EditValue = Function.ParseShort(-1);
			}
			DataTable groupNumPriceList = _httpClient.Items.GetGroupNumPriceList();
			cbPriceList.Properties.Columns.Clear();
			cbPriceList.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "ListName"
			});
			cbPriceList.Properties.DisplayMember = "ListName";
			cbPriceList.Properties.ValueMember = "ListNum";
			cbPriceList.Properties.NullText = "";
			cbPriceList.Properties.DataSource = groupNumPriceList;
			_dtLine = GetItemLineTableStructure();
			DataTable dataTable3 = _httpClient.Items.GetShippingTypes();
			cbShippingType.Properties.Columns.Clear();
			cbShippingType.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "TrnspName"
			});
			cbShippingType.Properties.DisplayMember = "TrnspName";
			cbShippingType.Properties.ValueMember = "TrnspCode";
			cbShippingType.Properties.NullText = "";
			if (dataTable3 == null || dataTable3.Rows.Count == 0)
			{
				dataTable3 = new DataTable();
				dataTable3.Columns.AddRange(new DataColumn[5]
				{
					new DataColumn("TrnspCode", typeof(short)),
					new DataColumn("TrnspName", typeof(string)),
					new DataColumn("WebSite", typeof(string)),
					new DataColumn("UserSign", typeof(short)),
					new DataColumn("DataSource", typeof(string))
				});
				dataTable3.AcceptChanges();
			}
			if (!dataTable3.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["TrnspCode"]) == -10))
			{
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3["TrnspCode"] = Convert.ToInt16(-10);
				dataRow3["TrnspName"] = "Define New";
				dataTable3.Rows.Add(dataRow3);
				dataTable3.AcceptChanges();
			}
			cbShippingType.Properties.DataSource = dataTable3;
			DataTable dataTable4 = _httpClient.Items.GetManufacturers();
			cbManufacturer.Properties.Columns.Clear();
			cbManufacturer.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "FirmName"
			});
			cbManufacturer.Properties.DisplayMember = "FirmName";
			cbManufacturer.Properties.ValueMember = "FirmCode";
			if (dataTable4 == null || dataTable4.Rows.Count == 0)
			{
				dataTable4 = new DataTable();
				dataTable4.Columns.AddRange(new DataColumn[2]
				{
					new DataColumn("FirmCode", typeof(short)),
					new DataColumn("FirmName", typeof(string))
				});
			}
			if (!dataTable4.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["FirmCode"]) == -10))
			{
				DataRow dataRow4 = dataTable4.NewRow();
				dataRow4["FirmCode"] = Convert.ToInt16(-10);
				dataRow4["FirmName"] = "Define New";
				dataTable4.Rows.Add(dataRow4);
				dataTable4.AcceptChanges();
			}
			cbManufacturer.Properties.DataSource = dataTable4;
			DataTable dataTable5 = _httpClient.Items.ItemsMasterGetListCustomsGroup();
			luk_CstGrpCode.Properties.Columns.Clear();
			luk_CstGrpCode.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			luk_CstGrpCode.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Rate",
				FieldName = "Rate"
			});
			luk_CstGrpCode.Properties.DisplayMember = "Name";
			luk_CstGrpCode.Properties.ValueMember = "Code";
			luk_CstGrpCode.Properties.NullText = "";
			if (dataTable5 == null || dataTable5.Rows.Count == 0)
			{
				dataTable5 = new DataTable();
				dataTable5.Columns.AddRange(new DataColumn[3]
				{
					new DataColumn("Code", typeof(short)),
					new DataColumn("Name", typeof(string)),
					new DataColumn("Rate", typeof(double))
				});
				dataTable5.AcceptChanges();
			}
			if (!dataTable5.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -10))
			{
				DataRow dataRow5 = dataTable5.NewRow();
				dataRow5["Code"] = Convert.ToInt16(-10);
				dataRow5["Name"] = "Define New";
				dataTable5.Rows.Add(dataRow5);
				dataTable5.AcceptChanges();
			}
			luk_CstGrpCode.Properties.DataSource = dataTable5;
			DataTable dataTable6 = _httpClient.Items.GetVatGroupO();
			cbVatGourpSa.Properties.Columns.Clear();
			cbVatGourpSa.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "Code"
			});
			cbVatGourpSa.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			cbVatGourpSa.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Rate",
				FieldName = "Rate"
			});
			cbVatGourpSa.Properties.DisplayMember = "Name";
			cbVatGourpSa.Properties.ValueMember = "Code";
			if (dataTable6 == null || dataTable6.Rows.Count == 0)
			{
				dataTable6 = new DataTable();
				dataTable6.Columns.AddRange(new DataColumn[3]
				{
					new DataColumn("Code", typeof(string)),
					new DataColumn("Name", typeof(string)),
					new DataColumn("Rate", typeof(double))
				});
				dataTable6.AcceptChanges();
			}
			if (!dataTable6.AsEnumerable().Any((DataRow r) => Function.ToString(r["Code"]) == "Define New"))
			{
				DataRow dataRow6 = dataTable6.NewRow();
				dataRow6["Code"] = "Define New";
				dataRow6["Name"] = "Define New";
				dataTable6.Rows.Add(dataRow6);
				dataTable6.AcceptChanges();
			}
			cbVatGourpSa.Properties.DataSource = dataTable6;
			DataTable dataTable7 = _httpClient.Items.GetVatGroupI();
			cbVatGroupPu.Properties.Columns.Clear();
			cbVatGroupPu.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "Code"
			});
			cbVatGroupPu.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			cbVatGroupPu.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Rate",
				FieldName = "Rate"
			});
			cbVatGroupPu.Properties.DisplayMember = "Name";
			cbVatGroupPu.Properties.ValueMember = "Code";
			if (dataTable7 == null || dataTable7.Rows.Count == 0)
			{
				dataTable7 = new DataTable();
				dataTable7.Columns.AddRange(new DataColumn[3]
				{
					new DataColumn("Code", typeof(string)),
					new DataColumn("Name", typeof(string)),
					new DataColumn("Rate", typeof(double))
				});
				dataTable7.AcceptChanges();
			}
			if (!dataTable7.AsEnumerable().Any((DataRow r) => Function.ToString(r["Code"]) == "Define New"))
			{
				DataRow dataRow7 = dataTable7.NewRow();
				dataRow7["Code"] = "Define New";
				dataRow7["Name"] = "Define New";
				dataTable7.Rows.Add(dataRow7);
				dataTable7.AcceptChanges();
			}
			cbVatGroupPu.Properties.DataSource = dataTable7;
			if (controls == string.Empty || controls == "OrderInterval")
			{
				DataTable dataTable8 = _httpClient.Items.GetShowOCYC();
				cb_OrderInterval.Properties.Columns.Clear();
				cb_OrderInterval.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "Name"
				});
				cb_OrderInterval.Properties.DisplayMember = "Name";
				cb_OrderInterval.Properties.ValueMember = "Code";
				if (dataTable8 == null || dataTable8.Rows.Count == 0)
				{
					dataTable8 = new DataTable();
					dataTable8.Columns.AddRange(new DataColumn[2]
					{
						new DataColumn("Code", typeof(short)),
						new DataColumn("Name", typeof(string))
					});
				}
				if (!dataTable8.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -1))
				{
					DataRow dataRow8 = dataTable8.NewRow();
					dataRow8["Code"] = Convert.ToInt16(-1);
					dataRow8["Name"] = "";
					dataTable8.Rows.InsertAt(dataRow8, 0);
					dataTable8.AcceptChanges();
				}
				cb_OrderInterval.Properties.DataSource = dataTable8;
			}
			if (controls == string.Empty || controls == "VolumeUnit")
			{
				DataTable volumeUnit = _httpClient.Items.GetVolumeUnit();
				luk_BVolUnit.Properties.Columns.Clear();
				luk_BVolUnit.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "Name"
				});
				luk_BVolUnit.Properties.DisplayMember = "Name";
				luk_BVolUnit.Properties.ValueMember = "Code";
				luk_BVolUnit.Properties.NullText = "";
				luk_BVolUnit.Properties.DataSource = volumeUnit;
				luk_SVolUnit.Properties.Columns.Clear();
				luk_SVolUnit.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "Name"
				});
				luk_SVolUnit.Properties.DisplayMember = "Name";
				luk_SVolUnit.Properties.ValueMember = "Code";
				luk_SVolUnit.Properties.NullText = "";
				luk_SVolUnit.Properties.DataSource = volumeUnit;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadStaticCombo()
	{
		try
		{
			cbItemType.Properties.Columns.Clear();
			if (IsAssetMasterData)
			{
				cbItemType.SetDataSource(new string[1, 2] { { "F", "Fixed Assets" } }, isStringCode: true, "F");
				cbItemType.Enabled = false;
			}
			else
			{
				cbItemType.SetDataSource(new string[3, 2]
				{
					{ "I", "Items" },
					{ "L", "Labor" },
					{ "T", "Travel" }
				}, isStringCode: true, "I");
			}
			cb_PriceType.Properties.Columns.Clear();
			cb_PriceType.SetDataSource(new string[3, 2]
			{
				{ "0", "Primary currency" },
				{ "1", "Additional currency 1" },
				{ "2", "Additional currency 2" }
			}, isStringCode: true, "0");
			cbManageItemBy.Properties.Columns.Clear();
			cbManageItemBy.SetDataSource(new string[3, 2]
			{
				{ "0", "None" },
				{ "1", "Serial Numbers" },
				{ "2", "Batches" }
			}, isStringCode: true, "0");
			cbManagementMethod.Properties.Columns.Clear();
			cbManagementMethod.SetDataSource(new string[2, 2]
			{
				{ "A", "On Every Transaction" },
				{ "R", "On Release Only" }
			}, isStringCode: true, "A");
			luk_IssuePriBy.Properties.Columns.Clear();
			luk_IssuePriBy.SetDataSource(new string[2, 2]
			{
				{ "0", "Serial and Batch Numbers" },
				{ "1", "Bin Locations" }
			}, isStringCode: false, "0", isShort: true);
			cb_PlanningMethod.Properties.Columns.Clear();
			cb_PlanningMethod.SetDataSource(new string[2, 2]
			{
				{ "M", "MRP" },
				{ "N", "None" }
			}, isStringCode: true, "N");
			cbGLMethod.Properties.Columns.Clear();
			cbGLMethod.SetDataSource(new string[3, 2]
			{
				{ "W", "Warehouse" },
				{ "C", "Item Group" },
				{ "L", "Item Level" }
			}, isStringCode: true, "A");
			cbEvalSystem.Properties.Columns.Clear();
			cbEvalSystem.SetDataSource(new string[4, 2]
			{
				{ "A", "Moving Average" },
				{ "S", "Standard" },
				{ "F", "FIFO" },
				{ "B", "Serial/Batch" }
			}, isStringCode: true, "A");
			_dtEvalSystem = cbEvalSystem.Properties.DataSource as DataTable;
			cb_ProcurementMethod.Properties.Columns.Clear();
			cb_ProcurementMethod.SetDataSource(new string[2, 2]
			{
				{ "B", "Buy" },
				{ "M", "Make" }
			}, isStringCode: true, "B");
			cb_ComponentWarehouse.Properties.Columns.Clear();
			cb_ComponentWarehouse.SetDataSource(new string[2, 2]
			{
				{ "B", "From Bill of Materials Line" },
				{ "P", "From Parent Item Document Line" }
			}, isStringCode: true, "B");
			luk_IssueMthd.Properties.Columns.Clear();
			luk_IssueMthd.SetDataSource(new string[2, 2]
			{
				{ "B", "Backflush" },
				{ "M", "Manual" }
			}, isStringCode: true, "B");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboCode()
	{
		try
		{
			cb_Code.Properties.Columns.Clear();
			cb_Code.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "SeriesName"
			});
			cb_Code.Properties.DisplayMember = "SeriesName";
			cb_Code.Properties.ValueMember = "Series";
			cb_Code.Properties.NullText = "";
			string docSubType = "--";
			DataTable listSeries = _httpClient.Commons.GetListSeries("4", GlobalData.DefaultBranch, docSubType);
			if (listSeries != null && listSeries.Rows.Count > 0)
			{
				cb_Code.Properties.DataSource = listSeries;
				DataRow dataRow = listSeries.AsEnumerable().FirstOrDefault((DataRow t) => string.Compare(Function.ToString(t["SeriesName"]), "MANUAL", ignoreCase: true) == 0);
				if (dataRow != null)
				{
					cb_Code.EditValue = Function.ParseShort(dataRow["Series"]);
				}
			}
			else
			{
				cb_Code.EditValue = -1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadUoMEntryLookUpEdits(int ugpEntry, string itemCode)
	{
		try
		{
			DataTable uoMCombo = _httpClient.Items.GetUoMCombo(ugpEntry, itemCode, "P");
			bte_PUoMEntry.Properties.Columns.Clear();
			bte_PUoMEntry.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			bte_PUoMEntry.Properties.DisplayMember = "UomCode";
			bte_PUoMEntry.Properties.ValueMember = "UomEntry";
			bte_PUoMEntry.Properties.NullText = "";
			bte_PUoMEntry.Properties.DataSource = uoMCombo.Copy();
			bte_SUoMEntry.Properties.Columns.Clear();
			bte_SUoMEntry.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			bte_SUoMEntry.Properties.DisplayMember = "UomCode";
			bte_SUoMEntry.Properties.ValueMember = "UomEntry";
			bte_SUoMEntry.Properties.NullText = "";
			bte_SUoMEntry.Properties.DataSource = uoMCombo.Copy();
			bte_IUoMEntry.Properties.Columns.Clear();
			bte_IUoMEntry.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			bte_IUoMEntry.Properties.DisplayMember = "UomCode";
			bte_IUoMEntry.Properties.ValueMember = "UomEntry";
			bte_IUoMEntry.Properties.NullText = "";
			bte_IUoMEntry.Properties.DataSource = uoMCombo.Copy();
			bte_INUoMEntry.Properties.Columns.Clear();
			bte_INUoMEntry.Properties.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			bte_INUoMEntry.Properties.DisplayMember = "UomCode";
			bte_INUoMEntry.Properties.ValueMember = "UomEntry";
			bte_INUoMEntry.Properties.NullText = "";
			bte_INUoMEntry.Properties.DataSource = uoMCombo.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadRelatedItemGroupData()
	{
		try
		{
			int num = Function.ParseInt(cbItemGroup.EditValue);
			if (num <= 0)
			{
				cb_PlanningMethod.EditValue = "M";
				cb_ProcurementMethod.EditValue = "B";
				cb_ComponentWarehouse.Text = "B";
				cb_OrderInterval.EditValue = -1;
				txt_OrderMultiple.EditValue = 0.0;
				txt_MinimumOrderQty.EditValue = 0.0;
				txt_LeadTime.EditValue = 0;
				txt_ToleranceDays.EditValue = 0;
				cbEvalSystem.EditValue = "A";
				return;
			}
			DataTable itemGroupData = _httpClient.Items.GetItemGroupData(num);
			if (itemGroupData.IsNotNull())
			{
				cb_PlanningMethod.EditValue = Function.ToString(itemGroupData.Rows[0]["PlaningSys"]);
				cb_ProcurementMethod.EditValue = Function.ToString(itemGroupData.Rows[0]["PrcrmntMtd"]);
				cb_ComponentWarehouse.Text = Function.ToString(itemGroupData.Rows[0]["CompoWH"]);
				cb_OrderInterval.EditValue = Function.ParseShort(itemGroupData.Rows[0]["OrdrIntrvl"]);
				txt_OrderMultiple.EditValue = Function.ParseDouble(itemGroupData.Rows[0]["OrdrMulti"]);
				txt_MinimumOrderQty.EditValue = Function.ToString(itemGroupData.Rows[0]["MinOrdrQty"]);
				txt_LeadTime.EditValue = Function.ParseInt(itemGroupData.Rows[0]["LeadTime"]);
				txt_ToleranceDays.EditValue = Function.ParseInt(itemGroupData.Rows[0]["ToleranDay"]);
				if (_httpClient.Items.IsLinkedTrans(Function.ToString(txtItemCode.Text)) != "Y")
				{
					cbEvalSystem.EditValue = Function.ToString(itemGroupData.Rows[0]["InvntSys"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowPriceAndCurrency()
	{
		try
		{
			short? i = Function.ParseShort(cbPriceList.EditValue);
			if (!(i > 0))
			{
				return;
			}
			DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["PriceList"]) == i);
			DataRow dataRow2 = _dtPriceList.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseShort(t["ListNum"]) == i);
			if (dataRow == null || dataRow2 == null)
			{
				return;
			}
			switch (Function.ToString(cb_PriceType.EditValue))
			{
			case "0":
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency"])))
				{
					dataRow["Currency"] = Function.ToString(dataRow2["PrimCurr"]);
				}
				txtUnitPrice.Currency = Function.ToString(dataRow["Currency"]);
				txtUnitPrice.EditValue = dataRow["Price"];
				break;
			case "1":
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency1"])))
				{
					dataRow["Currency1"] = Function.ToString(dataRow2["AddCurr1"]);
				}
				txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr1"]);
				txtUnitPrice.EditValue = dataRow["AddPrice1"];
				break;
			case "2":
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["Currency2"])))
				{
					dataRow["Currency2"] = Function.ToString(dataRow2["AddCurr2"]);
				}
				txtUnitPrice.Currency = Function.ToString(dataRow2["AddCurr2"]);
				txtUnitPrice.EditValue = dataRow["AddPrice2"];
				break;
			}
			_dtLine.AcceptChanges();
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

	protected override void NoAuthSetControlSearch()
	{
		if (FormMode == Enums.FormViewModeType.Find)
		{
			Controls.EnableDisableControl(isEnable: true);
		}
		else
		{
			Controls.EnableDisableControl(isEnable: false);
		}
	}

	private void ChangePropetiesItemWhenChangeItemType()
	{
		switch (Function.ToString(cbItemType.EditValue))
		{
		case "I":
			chkInvntItem.Checked = true;
			chkPrchseItem.Checked = true;
			chkSellItem.Checked = true;
			break;
		case "L":
		case "T":
			chkInvntItem.Checked = false;
			chkPrchseItem.Checked = false;
			chkSellItem.Checked = true;
			break;
		case "F":
			chkInvntItem.Checked = false;
			chkPrchseItem.Checked = true;
			chkSellItem.Checked = true;
			break;
		}
		ChangeEnableCheckItem();
	}

	private void ChangeEnableCheckItem()
	{
		switch (Function.ToString(cbItemType.EditValue))
		{
		case "I":
			chkInvntItem.Enabled = true;
			chkPrchseItem.Enabled = true;
			chkSellItem.Enabled = true;
			break;
		case "L":
		case "T":
			chkInvntItem.Enabled = false;
			chkPrchseItem.Enabled = false;
			chkSellItem.Enabled = true;
			break;
		case "F":
			chkInvntItem.Enabled = false;
			chkPrchseItem.Enabled = true;
			chkSellItem.Enabled = true;
			break;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_ItemMasterData));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		this.chkSellItem = new Apzon.Controls.Editor.CheckEditEx();
		this.chkPrchseItem = new Apzon.Controls.Editor.CheckEditEx();
		this.chkInvntItem = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_PriceList = new Apzon.Controls.Controls.LabelLine();
		this.cbPriceList = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_UnitPrice = new Apzon.Controls.Controls.LabelLine();
		this.cb_PriceType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtFrgnName = new Apzon.Controls.Editor.TextEditEx();
		this.txtDescription = new Apzon.Controls.Editor.TextEditEx();
		this.txtBarCode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_ItemType = new Apzon.Controls.Controls.LabelLine();
		this.cbItemType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_ItemGroup = new Apzon.Controls.Controls.LabelLine();
		this.cbItemGroup = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_DescForLang = new Apzon.Controls.Controls.LabelLine();
		this.lb_Description = new Apzon.Controls.Controls.LabelLine();
		this.txtItemCode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_ItemCode = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_Detail = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_General = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_OneBOneRec = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_IssuePriBy = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbl_IssuePriBy = new Apzon.Controls.Controls.LabelLine();
		this.chk_ManOutOnly = new Apzon.Controls.Editor.CheckEditEx();
		this.bte_WarrntTmpl = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lbl_WarrntyTpl = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ServiceAttr = new Apzon.Controls.Controls.LabelLine();
		this.chk_NoDiscount = new Apzon.Controls.Editor.CheckEditEx();
		this.cbManagementMethod = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbManageItemBy = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbShippingType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbManufacturer = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.panel_Advanced = new System.Windows.Forms.Panel();
		this.dateFrozenTo2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateFrozenFrom2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateValidFrom2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateValidTo2 = new Apzon.Controls.Editor.DateEditEx();
		this.txtFrozenComm2 = new Apzon.Controls.Editor.TextEditEx();
		this.txtValidComm2 = new Apzon.Controls.Editor.TextEditEx();
		this.lb_ToInacti = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToActi = new Apzon.Controls.Controls.LabelLine();
		this.lb_RIacti = new Apzon.Controls.Controls.LabelLine();
		this.lb_RActi = new Apzon.Controls.Controls.LabelLine();
		this.lb_InactiveRangeFrom = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActiveRangeFrom = new Apzon.Controls.Controls.LabelLine();
		this.panel_Inactive = new System.Windows.Forms.Panel();
		this.txtFrozenComm = new Apzon.Controls.Editor.TextEditEx();
		this.lb_T = new Apzon.Controls.Controls.LabelLine();
		this.dateFrozenTo = new Apzon.Controls.Editor.DateEditEx();
		this.lb_R = new Apzon.Controls.Controls.LabelLine();
		this.lb_F = new Apzon.Controls.Controls.LabelLine();
		this.dateFrozenFrom = new Apzon.Controls.Editor.DateEditEx();
		this.lb_Manufacture = new Apzon.Controls.Controls.LabelLine();
		this.panel_Active = new System.Windows.Forms.Panel();
		this.lbVailFrom = new Apzon.Controls.Controls.LabelLine();
		this.dateValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.lbRemarks = new Apzon.Controls.Controls.LabelLine();
		this.txtValidComm = new Apzon.Controls.Editor.TextEditEx();
		this.dateValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.lbVailTo = new Apzon.Controls.Controls.LabelLine();
		this.lb_ManagementMethod = new Apzon.Controls.Controls.LabelLine();
		this.lb_ShippingType = new Apzon.Controls.Controls.LabelLine();
		this.lb_ManageItemBy = new Apzon.Controls.Controls.LabelLine();
		this.lb_SerialAndBatchNum = new Apzon.Controls.Controls.LabelLine();
		this.rdg_Active = new Apzon.Controls.Editor.RadioGroupEx();
		this.tab_Purchase = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calc_BWeight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_BHeight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_BWidth1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_BLength1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.btn_PreferredVendor = new Apzon.Controls.Controls.SimpleButtonEx();
		this.calc_CustomsGroup = new Apzon.Controls.Editor.CalcEditEx();
		this.luk_CstGrpCode = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.label3 = new System.Windows.Forms.Label();
		this.lb_CstGrpCode = new Apzon.Controls.Controls.LabelLine();
		this.btn_PUoMForm = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtBuyUnitMsr = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_NumInBuy = new Apzon.Controls.Controls.LabelLine();
		this.bte_PUoMEntry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtPurFactor2 = new Apzon.Controls.Editor.CalcEditEx();
		this.luk_BVolUnit = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btn_CopyToSalesData = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_BLength1 = new Apzon.Controls.Controls.LabelLine();
		this.calc_BVolume = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_BWeight1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_BVolume = new Apzon.Controls.Controls.LabelLine();
		this.lb_BHeight1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_BWidth1 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PUoMEntry = new Apzon.Controls.Controls.LabelLine();
		this.calc_VatPercentPurchase = new Apzon.Controls.Editor.CalcEditEx();
		this.cbVatGroupPu = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtPurPackUn = new Apzon.Controls.Editor.CalcEditEx();
		this.txtNumInBuy = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPurPackMsr = new Apzon.Controls.Editor.TextEditEx();
		this.txtSuppCatNum = new Apzon.Controls.Editor.TextEditEx();
		this.label1 = new System.Windows.Forms.Label();
		this.lb_PreferredVendor = new Apzon.Controls.Controls.LabelLine();
		this.txtPurFactor4 = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPurFactor3 = new Apzon.Controls.Editor.CalcEditEx();
		this.txtPurFactor1 = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_CatalogNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor4 = new Apzon.Controls.Controls.LabelLine();
		this.lb_PurchasingUoM = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor3 = new Apzon.Controls.Controls.LabelLine();
		this.lb_ItemsPerPurchaseUnit = new Apzon.Controls.Controls.LabelLine();
		this.lb_Purchasing = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_PackagingUoM = new Apzon.Controls.Controls.LabelLine();
		this.lb_TaxGroup = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor2 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Sales = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calc_SWeight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_SHeight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_SWidth1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_SLength1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.btn_SUoMForm = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtSalUnitMsr = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_NumInSale = new Apzon.Controls.Controls.LabelLine();
		this.bte_SUoMEntry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbl_SUoMEntry = new Apzon.Controls.Controls.LabelLine();
		this.luk_SVolUnit = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_SLength1 = new Apzon.Controls.Controls.LabelLine();
		this.calc_SVolume = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_SWeight1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_SVolume = new Apzon.Controls.Controls.LabelLine();
		this.lb_SHeight1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_SWidth1 = new Apzon.Controls.Controls.LabelLine();
		this.calc_VatPercentSales = new Apzon.Controls.Editor.CalcEditEx();
		this.cbVatGourpSa = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.label2 = new System.Windows.Forms.Label();
		this.txtSalPackUn = new Apzon.Controls.Editor.CalcEditEx();
		this.txtNumInSale = new Apzon.Controls.Editor.CalcEditEx();
		this.txtSalPackMsr = new Apzon.Controls.Editor.TextEditEx();
		this.lb_SalesUoM = new Apzon.Controls.Controls.LabelLine();
		this.txtSalFactor4 = new Apzon.Controls.Editor.CalcEditEx();
		this.txtSalFactor3 = new Apzon.Controls.Editor.CalcEditEx();
		this.txtSalFactor2 = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_ItemsPerSalesUnit = new Apzon.Controls.Controls.LabelLine();
		this.txtSalFactor1 = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_Sales = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor4S = new Apzon.Controls.Controls.LabelLine();
		this.lb_PackagingUoMS = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor3S = new Apzon.Controls.Controls.LabelLine();
		this.lb_TaxGroupS = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor2S = new Apzon.Controls.Controls.LabelLine();
		this.lb_Factor1S = new Apzon.Controls.Controls.LabelLine();
		this.tab_Inventory = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calc_IWeight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.txtInvntUom = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_INUoMEntry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.bte_IUoMEntry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.calc_NumInCnt = new Apzon.Controls.Editor.CalcEditEx();
		this.txt_CntUnitMsr = new Apzon.Controls.Editor.TextEditEx();
		this.cbEvalSystem = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtAvgPrice = new Apzon.Controls.Editor.CalcEditEx();
		this.lbl_NumInCnt = new Apzon.Controls.Controls.LabelLine();
		this.lbl_NumInCnt1 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_INUoMEntry = new Apzon.Controls.Controls.LabelLine();
		this.lbl_CntUnitMsr = new Apzon.Controls.Controls.LabelLine();
		this.Weight = new Apzon.Controls.Controls.LabelLine();
		this.lbl_IUoMEntry = new Apzon.Controls.Controls.LabelLine();
		this.txtMaxLevel = new Apzon.Controls.Editor.CalcEditEx();
		this.txtMinLevel = new Apzon.Controls.Editor.CalcEditEx();
		this.txtReorderQty = new Apzon.Controls.Editor.CalcEditEx();
		this.cbGLMethod = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_InventoryLevel = new Apzon.Controls.Controls.LabelLine();
		this.btn_SetDfltWH = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chkByWh = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_WareHouse = new Apzon.Controls.Grid.GridControlEx();
		this.ctmenu_Warehouse = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_WareHouse = new Apzon.Controls.Grid.GridViewEx();
		this.gcWhseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_WhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcWhseName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.rChk_Lock = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcPriceInventory = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CalcInventory_Price = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcOnHand = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CalcInventory_Quantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcCommited = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOnOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAvaiable = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMinInventory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMaxInventory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqInventory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DefaultBinLocation = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.lb_AccBy = new Apzon.Controls.Controls.LabelLine();
		this.lb_InventoryUom = new Apzon.Controls.Controls.LabelLine();
		this.lb_ValuationMethod = new Apzon.Controls.Controls.LabelLine();
		this.lbItemCost = new Apzon.Controls.Controls.LabelLine();
		this.lb_RequiredPrUOM = new Apzon.Controls.Controls.LabelLine();
		this.lb_Maximum = new Apzon.Controls.Controls.LabelLine();
		this.lb_Minimum = new Apzon.Controls.Controls.LabelLine();
		this.tab_PlanData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.cb_OrderInterval = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cb_ComponentWarehouse = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cb_ProcurementMethod = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cb_PlanningMethod = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_ToleranceDays = new Apzon.Controls.Editor.CalcEditEx();
		this.txt_LeadTime = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_ComponentWarehouse = new Apzon.Controls.Controls.LabelLine();
		this.lb_Days1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Days = new Apzon.Controls.Controls.LabelLine();
		this.txt_MinimumOrderQty = new Apzon.Controls.Editor.CalcEditEx();
		this.txt_OrderMultiple = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_ToleranceDays = new Apzon.Controls.Controls.LabelLine();
		this.lb_MiniumOrderQty = new Apzon.Controls.Controls.LabelLine();
		this.lb_LeadTime = new Apzon.Controls.Controls.LabelLine();
		this.lb_OrderMultiple = new Apzon.Controls.Controls.LabelLine();
		this.lb_ProcurementMethod = new Apzon.Controls.Controls.LabelLine();
		this.lb_OrderInterval = new Apzon.Controls.Controls.LabelLine();
		this.lb_PlanningMethod = new Apzon.Controls.Controls.LabelLine();
		this.tab_Production = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calc_PrdStdCst = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.txt_TreeType = new Apzon.Controls.Editor.ButtonEditEx();
		this.chk_InCostRoll = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_TreeType = new Apzon.Controls.Controls.LabelLine();
		this.calc_NoOfRes = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_NoOfItem = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_NoOfItem = new Apzon.Controls.Controls.LabelLine();
		this.lb_PrdStdCst = new Apzon.Controls.Controls.LabelLine();
		this.lb_NoOfRes = new Apzon.Controls.Controls.LabelLine();
		this.luk_IssueMthd = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_IssueMthd = new Apzon.Controls.Controls.LabelLine();
		this.chk_Phantom = new Apzon.Controls.Editor.CheckEditEx();
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
		this.panelControl9 = new DevExpress.XtraEditors.PanelControl();
		this.btn_PicDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btBrower = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtUserText = new Apzon.Controls.Editor.MemoEditEx();
		this.picRemarks = new DevExpress.XtraEditors.PictureEdit();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_UserDefineF = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Udf = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Udf = new Apzon.Controls.Grid.GridViewEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cb_Code = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_UoMGroup_UgpEntry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PriceListUoM = new Apzon.Controls.Controls.LabelLine();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.businessPartnerCatalogNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.billOfMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.inventoryPostingListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.inventoryAuditReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.avaiableToPromiseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txtUnitPrice = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		((System.ComponentModel.ISupportInitialize)this.chkSellItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkPrchseItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkInvntItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbPriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_PriceType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFrgnName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescription.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Detail).BeginInit();
		this.tabControl_Detail.SuspendLayout();
		this.tab_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_OneBOneRec.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssuePriBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ManOutOnly.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WarrntTmpl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NoDiscount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbManagementMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbManageItemBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbShippingType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbManufacturer.Properties).BeginInit();
		this.panel_Advanced.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm2.Properties).BeginInit();
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
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).BeginInit();
		this.tab_Purchase.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_BWeight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BHeight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BWidth1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BLength1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_CustomsGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CstGrpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBuyUnitMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PUoMEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BVolUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BVolume.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatPercentPurchase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbVatGroupPu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurPackUn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumInBuy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurPackMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSuppCatNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor4.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor1.Properties).BeginInit();
		this.tab_Sales.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_SWeight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SHeight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SWidth1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SLength1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalUnitMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SUoMEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SVolUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SVolume.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatPercentSales.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbVatGourpSa.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalPackUn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumInSale.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalPackMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor4.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor1.Properties).BeginInit();
		this.tab_Inventory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_IWeight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInvntUom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_INUoMEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_IUoMEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NumInCnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CntUnitMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbEvalSystem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtAvgPrice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtMaxLevel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtMinLevel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReorderQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbGLMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkByWh.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WareHouse).BeginInit();
		this.ctmenu_Warehouse.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_WareHouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rChk_Lock).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CalcInventory_Price).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CalcInventory_Quantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DefaultBinLocation).BeginInit();
		this.tab_PlanData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cb_OrderInterval.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ComponentWarehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ProcurementMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_PlanningMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToleranceDays.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LeadTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MinimumOrderQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OrderMultiple.Properties).BeginInit();
		this.tab_Production.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_PrdStdCst.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TreeType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InCostRoll.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfRes.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssueMthd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Phantom.Properties).BeginInit();
		this.tab_Properties.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PropertiesCheck).BeginInit();
		this.tab_Remark.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl9).BeginInit();
		this.panelControl9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtUserText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.picRemarks.Properties).BeginInit();
		this.tab_Attachments.SuspendLayout();
		this.tab_UserDefineF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UoMGroup_UgpEntry.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtUnitPrice.Properties).BeginInit();
		base.SuspendLayout();
		this.chkSellItem.EditControl = this.chkSellItem;
		this.chkSellItem.FieldName = "SellItem";
		this.chkSellItem.IsSearchControl = false;
		this.chkSellItem.Location = new System.Drawing.Point(611, 56);
		this.chkSellItem.Name = "chkSellItem";
		this.chkSellItem.Properties.Caption = "Sales Item";
		this.chkSellItem.Properties.ValueChecked = "Y";
		this.chkSellItem.Properties.ValueGrayed = "N";
		this.chkSellItem.Properties.ValueUnchecked = "N";
		this.chkSellItem.SettingInVisible = false;
		this.chkSellItem.SettingReadonly = false;
		this.chkSellItem.Size = new System.Drawing.Size(128, 20);
		this.chkSellItem.TabIndex = 12;
		this.chkPrchseItem.EditControl = this.chkPrchseItem;
		this.chkPrchseItem.FieldName = "PrchseItem";
		this.chkPrchseItem.IsSearchControl = false;
		this.chkPrchseItem.Location = new System.Drawing.Point(611, 35);
		this.chkPrchseItem.Name = "chkPrchseItem";
		this.chkPrchseItem.Properties.Caption = "Purchased Item";
		this.chkPrchseItem.Properties.DisplayValueChecked = "Y";
		this.chkPrchseItem.Properties.DisplayValueGrayed = "N";
		this.chkPrchseItem.Properties.DisplayValueUnchecked = "N";
		this.chkPrchseItem.Properties.ValueChecked = "Y";
		this.chkPrchseItem.Properties.ValueGrayed = "N";
		this.chkPrchseItem.Properties.ValueUnchecked = "N";
		this.chkPrchseItem.SettingInVisible = false;
		this.chkPrchseItem.SettingReadonly = false;
		this.chkPrchseItem.Size = new System.Drawing.Size(122, 20);
		this.chkPrchseItem.TabIndex = 11;
		this.chkInvntItem.EditControl = this.chkInvntItem;
		this.chkInvntItem.FieldName = "InvntItem";
		this.chkInvntItem.IsSearchControl = false;
		this.chkInvntItem.Location = new System.Drawing.Point(611, 12);
		this.chkInvntItem.Name = "chkInvntItem";
		this.chkInvntItem.Properties.Caption = "Inventory Item";
		this.chkInvntItem.Properties.DisplayValueChecked = "Y";
		this.chkInvntItem.Properties.DisplayValueGrayed = "N";
		this.chkInvntItem.Properties.DisplayValueUnchecked = "N";
		this.chkInvntItem.Properties.ValueChecked = "Y";
		this.chkInvntItem.Properties.ValueGrayed = "N";
		this.chkInvntItem.Properties.ValueUnchecked = "N";
		this.chkInvntItem.SettingInVisible = false;
		this.chkInvntItem.SettingReadonly = false;
		this.chkInvntItem.Size = new System.Drawing.Size(122, 20);
		this.chkInvntItem.TabIndex = 10;
		this.lb_PriceList.EditControl = this.cbPriceList;
		this.lb_PriceList.EditValue = "Price list";
		this.lb_PriceList.FieldName = "lb_PriceList";
		this.lb_PriceList.HideWhenEmpty = false;
		this.lb_PriceList.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PriceList.Location = new System.Drawing.Point(7, 107);
		this.lb_PriceList.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_PriceList.Name = "lb_PriceList";
		this.lb_PriceList.ReadOnly = false;
		this.lb_PriceList.SettingInVisible = false;
		this.lb_PriceList.SettingReadonly = false;
		this.lb_PriceList.ShowUnderLine = true;
		this.lb_PriceList.Size = new System.Drawing.Size(146, 21);
		this.lb_PriceList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PriceList.TabIndex = 0;
		this.lb_PriceList.TabStop = false;
		this.lb_PriceList.Utext = "Price list";
		this.cbPriceList.EditControl = null;
		this.cbPriceList.FieldName = null;
		this.cbPriceList.IsSearchControl = false;
		this.cbPriceList.Location = new System.Drawing.Point(153, 106);
		this.cbPriceList.Name = "cbPriceList";
		this.cbPriceList.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbPriceList.Properties.Appearance.Options.UseFont = true;
		this.cbPriceList.Properties.AutoHeight = false;
		this.cbPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbPriceList.Properties.IsSearchControl = false;
		this.cbPriceList.Properties.NullText = "";
		this.cbPriceList.SettingInVisible = false;
		this.cbPriceList.SettingReadonly = false;
		this.cbPriceList.Size = new System.Drawing.Size(132, 18);
		this.cbPriceList.TabIndex = 9;
		this.cbPriceList.Tag = "OPLN,ListName";
		this.cbPriceList.EditValueChanged += new System.EventHandler(this.cbPriceList_EditValueChanged);
		this.lb_UnitPrice.EditControl = this.cb_PriceType;
		this.lb_UnitPrice.EditValue = "Unit price";
		this.lb_UnitPrice.FieldName = "lb_UnitPrice";
		this.lb_UnitPrice.HideWhenEmpty = false;
		this.lb_UnitPrice.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UnitPrice.Location = new System.Drawing.Point(307, 107);
		this.lb_UnitPrice.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_UnitPrice.Name = "lb_UnitPrice";
		this.lb_UnitPrice.ReadOnly = false;
		this.lb_UnitPrice.SettingInVisible = false;
		this.lb_UnitPrice.SettingReadonly = false;
		this.lb_UnitPrice.ShowUnderLine = true;
		this.lb_UnitPrice.Size = new System.Drawing.Size(104, 23);
		this.lb_UnitPrice.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UnitPrice.TabIndex = 0;
		this.lb_UnitPrice.TabStop = false;
		this.lb_UnitPrice.Utext = "Unit price";
		this.cb_PriceType.EditControl = null;
		this.cb_PriceType.FieldName = null;
		this.cb_PriceType.IsSearchControl = false;
		this.cb_PriceType.Location = new System.Drawing.Point(405, 106);
		this.cb_PriceType.Name = "cb_PriceType";
		this.cb_PriceType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_PriceType.Properties.Appearance.Options.UseFont = true;
		this.cb_PriceType.Properties.AutoHeight = false;
		this.cb_PriceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_PriceType.Properties.IsSearchControl = false;
		this.cb_PriceType.Properties.NullText = "";
		this.cb_PriceType.SettingInVisible = false;
		this.cb_PriceType.SettingReadonly = false;
		this.cb_PriceType.Size = new System.Drawing.Size(116, 18);
		this.cb_PriceType.TabIndex = 13;
		this.cb_PriceType.EditValueChanged += new System.EventHandler(this.cb_PriceType_EditValueChanged);
		this.txtFrgnName.EditControl = null;
		this.txtFrgnName.FieldName = "FrgnName";
		this.txtFrgnName.IsSearchControl = false;
		this.txtFrgnName.Location = new System.Drawing.Point(153, 49);
		this.txtFrgnName.Name = "txtFrgnName";
		this.txtFrgnName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFrgnName.Properties.Appearance.Options.UseFont = true;
		this.txtFrgnName.Properties.AutoHeight = false;
		this.txtFrgnName.SettingInVisible = false;
		this.txtFrgnName.SettingReadonly = false;
		this.txtFrgnName.Size = new System.Drawing.Size(369, 18);
		this.txtFrgnName.TabIndex = 5;
		this.txtFrgnName.Tag = "OITM,FrgnName";
		this.txtDescription.EditControl = null;
		this.txtDescription.FieldName = "ItemName";
		this.txtDescription.IsSearchControl = false;
		this.txtDescription.Location = new System.Drawing.Point(153, 30);
		this.txtDescription.Name = "txtDescription";
		this.txtDescription.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtDescription.Properties.Appearance.Options.UseFont = true;
		this.txtDescription.Properties.AutoHeight = false;
		this.txtDescription.SettingInVisible = false;
		this.txtDescription.SettingReadonly = false;
		this.txtDescription.Size = new System.Drawing.Size(369, 18);
		this.txtDescription.TabIndex = 4;
		this.txtDescription.Tag = "OITM,ItemName";
		this.txtBarCode.EditControl = null;
		this.txtBarCode.FieldName = "CodeBars";
		this.txtBarCode.IsSearchControl = false;
		this.txtBarCode.Location = new System.Drawing.Point(371, 10);
		this.txtBarCode.Name = "txtBarCode";
		this.txtBarCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBarCode.Properties.Appearance.Options.UseFont = true;
		this.txtBarCode.Properties.AutoHeight = false;
		this.txtBarCode.SettingInVisible = false;
		this.txtBarCode.SettingReadonly = false;
		this.txtBarCode.Size = new System.Drawing.Size(151, 18);
		this.txtBarCode.TabIndex = 3;
		this.txtBarCode.Tag = "OITM,CodeBars";
		this.lb_BarCode.EditControl = this.txtBarCode;
		this.lb_BarCode.EditValue = "Bar code";
		this.lb_BarCode.FieldName = "lb_BarCode";
		this.lb_BarCode.HideWhenEmpty = false;
		this.lb_BarCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BarCode.Location = new System.Drawing.Point(287, 11);
		this.lb_BarCode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_BarCode.Name = "lb_BarCode";
		this.lb_BarCode.ReadOnly = false;
		this.lb_BarCode.SettingInVisible = false;
		this.lb_BarCode.SettingReadonly = false;
		this.lb_BarCode.ShowUnderLine = true;
		this.lb_BarCode.Size = new System.Drawing.Size(84, 20);
		this.lb_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BarCode.TabIndex = 0;
		this.lb_BarCode.TabStop = false;
		this.lb_BarCode.Utext = "Bar code";
		this.lb_ItemType.EditControl = this.cbItemType;
		this.lb_ItemType.EditValue = "Item Type";
		this.lb_ItemType.FieldName = "lb_ItemType";
		this.lb_ItemType.HideWhenEmpty = false;
		this.lb_ItemType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemType.Location = new System.Drawing.Point(7, 69);
		this.lb_ItemType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_ItemType.Name = "lb_ItemType";
		this.lb_ItemType.ReadOnly = false;
		this.lb_ItemType.SettingInVisible = false;
		this.lb_ItemType.SettingReadonly = false;
		this.lb_ItemType.ShowUnderLine = true;
		this.lb_ItemType.Size = new System.Drawing.Size(146, 21);
		this.lb_ItemType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemType.TabIndex = 0;
		this.lb_ItemType.TabStop = false;
		this.lb_ItemType.Utext = "Item Type";
		this.cbItemType.EditControl = null;
		this.cbItemType.FieldName = "ItemType";
		this.cbItemType.IsSearchControl = false;
		this.cbItemType.Location = new System.Drawing.Point(153, 68);
		this.cbItemType.Name = "cbItemType";
		this.cbItemType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbItemType.Properties.Appearance.Options.UseFont = true;
		this.cbItemType.Properties.AutoHeight = false;
		this.cbItemType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbItemType.Properties.IsSearchControl = false;
		this.cbItemType.Properties.NullText = "";
		this.cbItemType.SettingInVisible = false;
		this.cbItemType.SettingReadonly = false;
		this.cbItemType.Size = new System.Drawing.Size(132, 18);
		this.cbItemType.TabIndex = 6;
		this.cbItemType.Tag = "OITM,ItemType";
		this.lb_ItemGroup.EditControl = this.cbItemGroup;
		this.lb_ItemGroup.EditValue = "Item Group";
		this.lb_ItemGroup.FieldName = "lb_ItemGroup";
		this.lb_ItemGroup.HideWhenEmpty = false;
		this.lb_ItemGroup.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemGroup.Location = new System.Drawing.Point(7, 88);
		this.lb_ItemGroup.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_ItemGroup.Name = "lb_ItemGroup";
		this.lb_ItemGroup.ReadOnly = false;
		this.lb_ItemGroup.SettingInVisible = false;
		this.lb_ItemGroup.SettingReadonly = false;
		this.lb_ItemGroup.ShowUnderLine = true;
		this.lb_ItemGroup.Size = new System.Drawing.Size(146, 21);
		this.lb_ItemGroup.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemGroup.TabIndex = 0;
		this.lb_ItemGroup.TabStop = false;
		this.lb_ItemGroup.Utext = "Item Group";
		this.cbItemGroup.EditControl = null;
		this.cbItemGroup.FieldName = "ItmsGrpCod";
		this.cbItemGroup.IsSearchControl = false;
		this.cbItemGroup.Location = new System.Drawing.Point(153, 87);
		this.cbItemGroup.Name = "cbItemGroup";
		this.cbItemGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbItemGroup.Properties.Appearance.Options.UseFont = true;
		this.cbItemGroup.Properties.AutoHeight = false;
		this.cbItemGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbItemGroup.Properties.IsSearchControl = false;
		this.cbItemGroup.Properties.NullText = "";
		this.cbItemGroup.SettingInVisible = false;
		this.cbItemGroup.SettingReadonly = false;
		this.cbItemGroup.Size = new System.Drawing.Size(132, 18);
		this.cbItemGroup.TabIndex = 8;
		this.cbItemGroup.Tag = "OITM,ItmsGrpCod";
		this.cbItemGroup.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbItemGroup_EditValueChanging);
		this.lb_DescForLang.EditControl = this.txtFrgnName;
		this.lb_DescForLang.EditValue = "Desc. in Foreign Lang.";
		this.lb_DescForLang.FieldName = "lb_DescForLang";
		this.lb_DescForLang.HideWhenEmpty = false;
		this.lb_DescForLang.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DescForLang.Location = new System.Drawing.Point(7, 50);
		this.lb_DescForLang.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_DescForLang.Name = "lb_DescForLang";
		this.lb_DescForLang.ReadOnly = false;
		this.lb_DescForLang.SettingInVisible = false;
		this.lb_DescForLang.SettingReadonly = false;
		this.lb_DescForLang.ShowUnderLine = true;
		this.lb_DescForLang.Size = new System.Drawing.Size(146, 21);
		this.lb_DescForLang.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DescForLang.TabIndex = 0;
		this.lb_DescForLang.TabStop = false;
		this.lb_DescForLang.Utext = "Desc. in Foreign Lang.";
		this.lb_Description.EditControl = this.txtDescription;
		this.lb_Description.EditValue = "Description";
		this.lb_Description.FieldName = "lb_Description";
		this.lb_Description.HideWhenEmpty = false;
		this.lb_Description.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Description.Location = new System.Drawing.Point(7, 31);
		this.lb_Description.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Description.Name = "lb_Description";
		this.lb_Description.ReadOnly = false;
		this.lb_Description.SettingInVisible = false;
		this.lb_Description.SettingReadonly = false;
		this.lb_Description.ShowUnderLine = true;
		this.lb_Description.Size = new System.Drawing.Size(146, 21);
		this.lb_Description.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Description.TabIndex = 0;
		this.lb_Description.TabStop = false;
		this.lb_Description.Utext = "Description";
		this.txtItemCode.EditControl = null;
		this.txtItemCode.FieldName = "ItemCode";
		this.txtItemCode.IsSearchControl = false;
		this.txtItemCode.Location = new System.Drawing.Point(153, 10);
		this.txtItemCode.Name = "txtItemCode";
		this.txtItemCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtItemCode.Properties.Appearance.Options.UseFont = true;
		this.txtItemCode.Properties.AutoHeight = false;
		this.txtItemCode.SettingInVisible = false;
		this.txtItemCode.SettingReadonly = false;
		this.txtItemCode.Size = new System.Drawing.Size(113, 18);
		this.txtItemCode.TabIndex = 2;
		this.txtItemCode.Tag = "OITM,ItemCode";
		this.lb_ItemCode.EditControl = this.txtItemCode;
		this.lb_ItemCode.EditValue = "Item Code";
		this.lb_ItemCode.FieldName = "lb_ItemCode";
		this.lb_ItemCode.HideWhenEmpty = false;
		this.lb_ItemCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemCode.Location = new System.Drawing.Point(7, 10);
		this.lb_ItemCode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_ItemCode.Name = "lb_ItemCode";
		this.lb_ItemCode.ReadOnly = false;
		this.lb_ItemCode.SettingInVisible = false;
		this.lb_ItemCode.SettingReadonly = false;
		this.lb_ItemCode.ShowUnderLine = true;
		this.lb_ItemCode.Size = new System.Drawing.Size(82, 22);
		this.lb_ItemCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemCode.TabIndex = 0;
		this.lb_ItemCode.TabStop = false;
		this.lb_ItemCode.Utext = "Item Code";
		this.tabControl_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_Detail.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.tabControl_Detail.Appearance.Options.UseBackColor = true;
		this.tabControl_Detail.IsSearchControl = false;
		this.tabControl_Detail.Location = new System.Drawing.Point(2, 130);
		this.tabControl_Detail.Name = "tabControl_Detail";
		this.tabControl_Detail.SelectedTabPage = this.tab_General;
		this.tabControl_Detail.Size = new System.Drawing.Size(821, 447);
		this.tabControl_Detail.TabIndex = 15;
		this.tabControl_Detail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[10] { this.tab_General, this.tab_Purchase, this.tab_Sales, this.tab_Inventory, this.tab_PlanData, this.tab_Production, this.tab_Properties, this.tab_Remark, this.tab_Attachments, this.tab_UserDefineF });
		this.tabControl_Detail.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_Detail_SelectedPageChanged);
		this.tab_General.Controls.Add(this.chk_OneBOneRec);
		this.tab_General.Controls.Add(this.luk_IssuePriBy);
		this.tab_General.Controls.Add(this.lbl_IssuePriBy);
		this.tab_General.Controls.Add(this.chk_ManOutOnly);
		this.tab_General.Controls.Add(this.bte_WarrntTmpl);
		this.tab_General.Controls.Add(this.lbl_WarrntyTpl);
		this.tab_General.Controls.Add(this.lbl_ServiceAttr);
		this.tab_General.Controls.Add(this.chk_NoDiscount);
		this.tab_General.Controls.Add(this.cbManagementMethod);
		this.tab_General.Controls.Add(this.cbManageItemBy);
		this.tab_General.Controls.Add(this.cbShippingType);
		this.tab_General.Controls.Add(this.cbManufacturer);
		this.tab_General.Controls.Add(this.panel_Advanced);
		this.tab_General.Controls.Add(this.panel_Inactive);
		this.tab_General.Controls.Add(this.lb_Manufacture);
		this.tab_General.Controls.Add(this.panel_Active);
		this.tab_General.Controls.Add(this.lb_ManagementMethod);
		this.tab_General.Controls.Add(this.lb_ShippingType);
		this.tab_General.Controls.Add(this.lb_ManageItemBy);
		this.tab_General.Controls.Add(this.lb_SerialAndBatchNum);
		this.tab_General.Controls.Add(this.rdg_Active);
		this.tab_General.EditControl = null;
		this.tab_General.EditValue = "General";
		this.tab_General.FieldName = "tab_General";
		this.tab_General.IsSearchControl = false;
		this.tab_General.Name = "tab_General";
		this.tab_General.ReadOnly = true;
		this.tab_General.SettingInVisible = false;
		this.tab_General.SettingReadonly = false;
		this.tab_General.Size = new System.Drawing.Size(819, 422);
		this.tab_General.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_General.Text = "General";
		this.chk_OneBOneRec.EditControl = this.chk_OneBOneRec;
		this.chk_OneBOneRec.FieldName = "OneBOneRec";
		this.chk_OneBOneRec.IsSearchControl = false;
		this.chk_OneBOneRec.Location = new System.Drawing.Point(12, 223);
		this.chk_OneBOneRec.Name = "chk_OneBOneRec";
		this.chk_OneBOneRec.Properties.Caption = "Block Multiple Receipts for Same Batch";
		this.chk_OneBOneRec.Properties.DisplayValueChecked = "Y";
		this.chk_OneBOneRec.Properties.DisplayValueGrayed = "N";
		this.chk_OneBOneRec.Properties.DisplayValueUnchecked = "N";
		this.chk_OneBOneRec.Properties.ValueChecked = "Y";
		this.chk_OneBOneRec.Properties.ValueGrayed = "N";
		this.chk_OneBOneRec.Properties.ValueUnchecked = "N";
		this.chk_OneBOneRec.SettingInVisible = false;
		this.chk_OneBOneRec.SettingReadonly = false;
		this.chk_OneBOneRec.Size = new System.Drawing.Size(210, 20);
		this.chk_OneBOneRec.TabIndex = 19;
		this.luk_IssuePriBy.EditControl = null;
		this.luk_IssuePriBy.FieldName = "IssuePriBy";
		this.luk_IssuePriBy.IsSearchControl = false;
		this.luk_IssuePriBy.Location = new System.Drawing.Point(178, 244);
		this.luk_IssuePriBy.Name = "luk_IssuePriBy";
		this.luk_IssuePriBy.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_IssuePriBy.Properties.Appearance.Options.UseFont = true;
		this.luk_IssuePriBy.Properties.AutoHeight = false;
		this.luk_IssuePriBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_IssuePriBy.Properties.IsSearchControl = false;
		this.luk_IssuePriBy.Properties.NullText = "";
		this.luk_IssuePriBy.SettingInVisible = false;
		this.luk_IssuePriBy.SettingReadonly = false;
		this.luk_IssuePriBy.Size = new System.Drawing.Size(163, 18);
		this.luk_IssuePriBy.TabIndex = 18;
		this.luk_IssuePriBy.Tag = "OITM,MngMethod";
		this.lbl_IssuePriBy.EditControl = this.luk_IssuePriBy;
		this.lbl_IssuePriBy.EditValue = "Issue Primarily By";
		this.lbl_IssuePriBy.FieldName = "lbl_IssuePriBy";
		this.lbl_IssuePriBy.HideWhenEmpty = false;
		this.lbl_IssuePriBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_IssuePriBy.Location = new System.Drawing.Point(9, 245);
		this.lbl_IssuePriBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_IssuePriBy.Name = "lbl_IssuePriBy";
		this.lbl_IssuePriBy.ReadOnly = false;
		this.lbl_IssuePriBy.SettingInVisible = false;
		this.lbl_IssuePriBy.SettingReadonly = false;
		this.lbl_IssuePriBy.ShowUnderLine = true;
		this.lbl_IssuePriBy.Size = new System.Drawing.Size(170, 20);
		this.lbl_IssuePriBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_IssuePriBy.TabIndex = 17;
		this.lbl_IssuePriBy.TabStop = false;
		this.lbl_IssuePriBy.Utext = "Issue Primarily By";
		this.chk_ManOutOnly.EditControl = this.chk_ManOutOnly;
		this.chk_ManOutOnly.FieldName = "ManOutOnly";
		this.chk_ManOutOnly.IsSearchControl = false;
		this.chk_ManOutOnly.Location = new System.Drawing.Point(12, 200);
		this.chk_ManOutOnly.Name = "chk_ManOutOnly";
		this.chk_ManOutOnly.Properties.Caption = "Automatic Serial Number Creation on Receipt";
		this.chk_ManOutOnly.Properties.DisplayValueChecked = "Y";
		this.chk_ManOutOnly.Properties.DisplayValueGrayed = "N";
		this.chk_ManOutOnly.Properties.DisplayValueUnchecked = "N";
		this.chk_ManOutOnly.Properties.ValueChecked = "Y";
		this.chk_ManOutOnly.Properties.ValueGrayed = "N";
		this.chk_ManOutOnly.Properties.ValueUnchecked = "N";
		this.chk_ManOutOnly.SettingInVisible = false;
		this.chk_ManOutOnly.SettingReadonly = false;
		this.chk_ManOutOnly.Size = new System.Drawing.Size(239, 20);
		this.chk_ManOutOnly.TabIndex = 16;
		this.bte_WarrntTmpl.EditControl = null;
		this.bte_WarrntTmpl.FieldName = "WarrntTmpl";
		this.bte_WarrntTmpl.IsSearchControl = false;
		this.bte_WarrntTmpl.Location = new System.Drawing.Point(534, 57);
		this.bte_WarrntTmpl.Name = "bte_WarrntTmpl";
		this.bte_WarrntTmpl.Properties.AutoHeight = false;
		this.bte_WarrntTmpl.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.bte_WarrntTmpl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_WarrntTmpl.Properties.ContainerControl = null;
		this.bte_WarrntTmpl.Properties.DisplayMember = "TmpltName";
		this.bte_WarrntTmpl.Properties.DisplayMember2 = "";
		this.bte_WarrntTmpl.Properties.LoadDataSourceWhenEnter = false;
		this.bte_WarrntTmpl.Properties.SeparateCharacterForMulti = ';';
		this.bte_WarrntTmpl.Properties.SeparateString = ", ";
		this.bte_WarrntTmpl.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ItemWarrantyTemplate;
		this.bte_WarrntTmpl.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oContractTemplates;
		this.bte_WarrntTmpl.Properties.UsingAutoCompleteBox = false;
		this.bte_WarrntTmpl.Properties.UsingDataSource = true;
		this.bte_WarrntTmpl.Properties.ValidateWhenNoDataSource = false;
		this.bte_WarrntTmpl.Properties.ValueMember = "TmpltName";
		this.bte_WarrntTmpl.SettingInVisible = false;
		this.bte_WarrntTmpl.SettingReadonly = false;
		this.bte_WarrntTmpl.Size = new System.Drawing.Size(161, 18);
		this.bte_WarrntTmpl.TabIndex = 15;
		this.lbl_WarrntyTpl.EditControl = this.bte_WarrntTmpl;
		this.lbl_WarrntyTpl.EditValue = "Warranty Template";
		this.lbl_WarrntyTpl.FieldName = "lbl_WarrntyTpl";
		this.lbl_WarrntyTpl.HideWhenEmpty = false;
		this.lbl_WarrntyTpl.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_WarrntyTpl.Location = new System.Drawing.Point(372, 58);
		this.lbl_WarrntyTpl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_WarrntyTpl.Name = "lbl_WarrntyTpl";
		this.lbl_WarrntyTpl.ReadOnly = false;
		this.lbl_WarrntyTpl.SettingInVisible = false;
		this.lbl_WarrntyTpl.SettingReadonly = false;
		this.lbl_WarrntyTpl.ShowUnderLine = true;
		this.lbl_WarrntyTpl.Size = new System.Drawing.Size(170, 20);
		this.lbl_WarrntyTpl.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_WarrntyTpl.TabIndex = 13;
		this.lbl_WarrntyTpl.TabStop = false;
		this.lbl_WarrntyTpl.Utext = "Warranty Template";
		this.lbl_ServiceAttr.EditControl = null;
		this.lbl_ServiceAttr.EditValue = "Service Attributes";
		this.lbl_ServiceAttr.FieldName = "lbl_ServiceAttr";
		this.lbl_ServiceAttr.HideWhenEmpty = false;
		this.lbl_ServiceAttr.LineColor = System.Drawing.Color.Black;
		this.lbl_ServiceAttr.Location = new System.Drawing.Point(370, 32);
		this.lbl_ServiceAttr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_ServiceAttr.Name = "lbl_ServiceAttr";
		this.lbl_ServiceAttr.ReadOnly = false;
		this.lbl_ServiceAttr.SettingInVisible = false;
		this.lbl_ServiceAttr.SettingReadonly = false;
		this.lbl_ServiceAttr.ShowUnderLine = true;
		this.lbl_ServiceAttr.Size = new System.Drawing.Size(94, 20);
		this.lbl_ServiceAttr.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ServiceAttr.TabIndex = 12;
		this.lbl_ServiceAttr.TabStop = false;
		this.lbl_ServiceAttr.Utext = "Service Attributes";
		this.chk_NoDiscount.EditControl = this.chk_NoDiscount;
		this.chk_NoDiscount.FieldName = "NoDiscount";
		this.chk_NoDiscount.IsSearchControl = false;
		this.chk_NoDiscount.Location = new System.Drawing.Point(12, 29);
		this.chk_NoDiscount.Name = "chk_NoDiscount";
		this.chk_NoDiscount.Properties.Caption = "Do Not Appl&y Discount Groups";
		this.chk_NoDiscount.Properties.DisplayValueChecked = "Y";
		this.chk_NoDiscount.Properties.DisplayValueGrayed = "N";
		this.chk_NoDiscount.Properties.DisplayValueUnchecked = "N";
		this.chk_NoDiscount.Properties.ValueChecked = "Y";
		this.chk_NoDiscount.Properties.ValueGrayed = "N";
		this.chk_NoDiscount.Properties.ValueUnchecked = "N";
		this.chk_NoDiscount.SettingInVisible = false;
		this.chk_NoDiscount.SettingReadonly = false;
		this.chk_NoDiscount.Size = new System.Drawing.Size(167, 20);
		this.chk_NoDiscount.TabIndex = 11;
		this.cbManagementMethod.EditControl = null;
		this.cbManagementMethod.FieldName = "MngMethod";
		this.cbManagementMethod.IsSearchControl = false;
		this.cbManagementMethod.Location = new System.Drawing.Point(178, 179);
		this.cbManagementMethod.Name = "cbManagementMethod";
		this.cbManagementMethod.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbManagementMethod.Properties.Appearance.Options.UseFont = true;
		this.cbManagementMethod.Properties.AutoHeight = false;
		this.cbManagementMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbManagementMethod.Properties.IsSearchControl = false;
		this.cbManagementMethod.Properties.NullText = "";
		this.cbManagementMethod.SettingInVisible = false;
		this.cbManagementMethod.SettingReadonly = false;
		this.cbManagementMethod.Size = new System.Drawing.Size(163, 18);
		this.cbManagementMethod.TabIndex = 3;
		this.cbManagementMethod.Tag = "OITM,MngMethod";
		this.cbManagementMethod.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbManagementMethod_EditValueChanging);
		this.cbManageItemBy.EditControl = null;
		this.cbManageItemBy.FieldName = null;
		this.cbManageItemBy.IsSearchControl = false;
		this.cbManageItemBy.Location = new System.Drawing.Point(178, 126);
		this.cbManageItemBy.Name = "cbManageItemBy";
		this.cbManageItemBy.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbManageItemBy.Properties.Appearance.Options.UseFont = true;
		this.cbManageItemBy.Properties.AutoHeight = false;
		this.cbManageItemBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbManageItemBy.Properties.IsSearchControl = false;
		this.cbManageItemBy.Properties.NullText = "";
		this.cbManageItemBy.SettingInVisible = false;
		this.cbManageItemBy.SettingReadonly = false;
		this.cbManageItemBy.Size = new System.Drawing.Size(163, 18);
		this.cbManageItemBy.TabIndex = 2;
		this.cbManageItemBy.Tag = "";
		this.cbManageItemBy.EditValueChanged += new System.EventHandler(this.cbManageItemBy_EditValueChanged);
		this.cbManageItemBy.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbManageItemBy_EditValueChanging);
		this.cbShippingType.EditControl = null;
		this.cbShippingType.FieldName = "ShipType";
		this.cbShippingType.IsSearchControl = false;
		this.cbShippingType.Location = new System.Drawing.Point(178, 76);
		this.cbShippingType.Name = "cbShippingType";
		this.cbShippingType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbShippingType.Properties.Appearance.Options.UseFont = true;
		this.cbShippingType.Properties.AutoHeight = false;
		this.cbShippingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbShippingType.Properties.IsSearchControl = false;
		this.cbShippingType.Properties.NullText = "";
		this.cbShippingType.SettingInVisible = false;
		this.cbShippingType.SettingReadonly = false;
		this.cbShippingType.Size = new System.Drawing.Size(163, 18);
		this.cbShippingType.TabIndex = 1;
		this.cbShippingType.Tag = "OITM,ShipType";
		this.cbShippingType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbShippingType_EditValueChanging);
		this.cbManufacturer.EditControl = null;
		this.cbManufacturer.FieldName = "FirmCode";
		this.cbManufacturer.IsSearchControl = false;
		this.cbManufacturer.Location = new System.Drawing.Point(178, 57);
		this.cbManufacturer.Name = "cbManufacturer";
		this.cbManufacturer.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbManufacturer.Properties.Appearance.Options.UseFont = true;
		this.cbManufacturer.Properties.AutoHeight = false;
		this.cbManufacturer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbManufacturer.Properties.IsSearchControl = false;
		this.cbManufacturer.Properties.NullText = "";
		this.cbManufacturer.SettingInVisible = false;
		this.cbManufacturer.SettingReadonly = false;
		this.cbManufacturer.Size = new System.Drawing.Size(163, 18);
		this.cbManufacturer.TabIndex = 0;
		this.cbManufacturer.Tag = "OITM,FirmCode";
		this.cbManufacturer.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbManufacturer_EditValueChanging);
		this.panel_Advanced.Controls.Add(this.dateFrozenTo2);
		this.panel_Advanced.Controls.Add(this.dateFrozenFrom2);
		this.panel_Advanced.Controls.Add(this.dateValidFrom2);
		this.panel_Advanced.Controls.Add(this.dateValidTo2);
		this.panel_Advanced.Controls.Add(this.txtFrozenComm2);
		this.panel_Advanced.Controls.Add(this.txtValidComm2);
		this.panel_Advanced.Controls.Add(this.lb_ToInacti);
		this.panel_Advanced.Controls.Add(this.lb_ToActi);
		this.panel_Advanced.Controls.Add(this.lb_RIacti);
		this.panel_Advanced.Controls.Add(this.lb_RActi);
		this.panel_Advanced.Controls.Add(this.lb_InactiveRangeFrom);
		this.panel_Advanced.Controls.Add(this.lb_ActiveRangeFrom);
		this.panel_Advanced.Location = new System.Drawing.Point(9, 354);
		this.panel_Advanced.Name = "panel_Advanced";
		this.panel_Advanced.Size = new System.Drawing.Size(713, 64);
		this.panel_Advanced.TabIndex = 7;
		this.dateFrozenTo2.EditControl = null;
		this.dateFrozenTo2.EditValue = null;
		this.dateFrozenTo2.FieldName = null;
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
		this.dateFrozenTo2.Size = new System.Drawing.Size(100, 18);
		this.dateFrozenTo2.TabIndex = 4;
		this.dateFrozenTo2.Tag = "OITM,frozenTo";
		this.dateFrozenTo2.EditValueChanged += new System.EventHandler(this.dateFrozenTo2_EditValueChanged);
		this.dateFrozenTo2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.dateFrozenFrom2.EditControl = null;
		this.dateFrozenFrom2.EditValue = null;
		this.dateFrozenFrom2.FieldName = null;
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
		this.dateFrozenFrom2.Size = new System.Drawing.Size(100, 18);
		this.dateFrozenFrom2.TabIndex = 3;
		this.dateFrozenFrom2.Tag = "OITM,frozenFrom";
		this.dateFrozenFrom2.EditValueChanged += new System.EventHandler(this.dateFrozenFrom2_EditValueChanged);
		this.dateFrozenFrom2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.dateValidFrom2.EditControl = null;
		this.dateValidFrom2.EditValue = null;
		this.dateValidFrom2.FieldName = null;
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
		this.dateValidFrom2.Size = new System.Drawing.Size(100, 18);
		this.dateValidFrom2.TabIndex = 0;
		this.dateValidFrom2.Tag = "OITM,validFrom";
		this.dateValidFrom2.EditValueChanged += new System.EventHandler(this.dateValidFrom2_EditValueChanged);
		this.dateValidFrom2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.dateValidTo2.EditControl = null;
		this.dateValidTo2.EditValue = null;
		this.dateValidTo2.FieldName = null;
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
		this.dateValidTo2.Tag = "OITM,validTo";
		this.dateValidTo2.EditValueChanged += new System.EventHandler(this.dateValidTo2_EditValueChanged);
		this.dateValidTo2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.txtFrozenComm2.EditControl = null;
		this.txtFrozenComm2.FieldName = null;
		this.txtFrozenComm2.IsSearchControl = false;
		this.txtFrozenComm2.Location = new System.Drawing.Point(584, 30);
		this.txtFrozenComm2.Name = "txtFrozenComm2";
		this.txtFrozenComm2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFrozenComm2.Properties.Appearance.Options.UseFont = true;
		this.txtFrozenComm2.Properties.AutoHeight = false;
		this.txtFrozenComm2.SettingInVisible = false;
		this.txtFrozenComm2.SettingReadonly = false;
		this.txtFrozenComm2.Size = new System.Drawing.Size(125, 18);
		this.txtFrozenComm2.TabIndex = 5;
		this.txtFrozenComm2.Tag = "OITM,FrozenComm";
		this.txtFrozenComm2.EditValueChanged += new System.EventHandler(this.txtFrozenComm2_EditValueChanged);
		this.txtValidComm2.EditControl = null;
		this.txtValidComm2.FieldName = null;
		this.txtValidComm2.IsSearchControl = false;
		this.txtValidComm2.Location = new System.Drawing.Point(584, 5);
		this.txtValidComm2.Name = "txtValidComm2";
		this.txtValidComm2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtValidComm2.Properties.Appearance.Options.UseFont = true;
		this.txtValidComm2.Properties.AutoHeight = false;
		this.txtValidComm2.SettingInVisible = false;
		this.txtValidComm2.SettingReadonly = false;
		this.txtValidComm2.Size = new System.Drawing.Size(125, 20);
		this.txtValidComm2.TabIndex = 2;
		this.txtValidComm2.Tag = "OITM,ValidComm";
		this.txtValidComm2.EditValueChanged += new System.EventHandler(this.txtValidComm2_EditValueChanged);
		this.lb_ToInacti.EditControl = this.dateFrozenTo2;
		this.lb_ToInacti.EditValue = "To";
		this.lb_ToInacti.FieldName = "lb_ToInacti";
		this.lb_ToInacti.HideWhenEmpty = false;
		this.lb_ToInacti.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToInacti.Location = new System.Drawing.Point(325, 32);
		this.lb_ToInacti.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToInacti.Name = "lb_ToInacti";
		this.lb_ToInacti.ReadOnly = false;
		this.lb_ToInacti.SettingInVisible = false;
		this.lb_ToInacti.SettingReadonly = false;
		this.lb_ToInacti.ShowUnderLine = true;
		this.lb_ToInacti.Size = new System.Drawing.Size(80, 20);
		this.lb_ToInacti.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToInacti.TabIndex = 0;
		this.lb_ToInacti.TabStop = false;
		this.lb_ToInacti.Utext = "To";
		this.lb_ToActi.EditControl = this.dateValidTo2;
		this.lb_ToActi.EditValue = "To";
		this.lb_ToActi.FieldName = "lb_ToActi";
		this.lb_ToActi.HideWhenEmpty = false;
		this.lb_ToActi.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToActi.Location = new System.Drawing.Point(325, 9);
		this.lb_ToActi.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToActi.Name = "lb_ToActi";
		this.lb_ToActi.ReadOnly = false;
		this.lb_ToActi.SettingInVisible = false;
		this.lb_ToActi.SettingReadonly = false;
		this.lb_ToActi.ShowUnderLine = true;
		this.lb_ToActi.Size = new System.Drawing.Size(80, 20);
		this.lb_ToActi.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToActi.TabIndex = 0;
		this.lb_ToActi.TabStop = false;
		this.lb_ToActi.Utext = "To";
		this.lb_RIacti.EditControl = this.txtFrozenComm2;
		this.lb_RIacti.EditValue = "Remarks";
		this.lb_RIacti.FieldName = "lb_RIacti";
		this.lb_RIacti.HideWhenEmpty = false;
		this.lb_RIacti.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RIacti.Location = new System.Drawing.Point(505, 31);
		this.lb_RIacti.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_RIacti.Name = "lb_RIacti";
		this.lb_RIacti.ReadOnly = false;
		this.lb_RIacti.SettingInVisible = false;
		this.lb_RIacti.SettingReadonly = false;
		this.lb_RIacti.ShowUnderLine = true;
		this.lb_RIacti.Size = new System.Drawing.Size(80, 20);
		this.lb_RIacti.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RIacti.TabIndex = 0;
		this.lb_RIacti.TabStop = false;
		this.lb_RIacti.Utext = "Remarks";
		this.lb_RActi.EditControl = this.txtValidComm2;
		this.lb_RActi.EditValue = "Remarks";
		this.lb_RActi.FieldName = "lb_RActi";
		this.lb_RActi.HideWhenEmpty = false;
		this.lb_RActi.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RActi.Location = new System.Drawing.Point(505, 8);
		this.lb_RActi.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_RActi.Name = "lb_RActi";
		this.lb_RActi.ReadOnly = false;
		this.lb_RActi.SettingInVisible = false;
		this.lb_RActi.SettingReadonly = false;
		this.lb_RActi.ShowUnderLine = true;
		this.lb_RActi.Size = new System.Drawing.Size(80, 20);
		this.lb_RActi.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RActi.TabIndex = 0;
		this.lb_RActi.TabStop = false;
		this.lb_RActi.Utext = "Remarks";
		this.lb_InactiveRangeFrom.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_InactiveRangeFrom.Appearance.Options.UseFont = true;
		this.lb_InactiveRangeFrom.EditControl = this.dateFrozenFrom2;
		this.lb_InactiveRangeFrom.EditValue = "Inactive Range            From";
		this.lb_InactiveRangeFrom.FieldName = "lb_InactiveRangeFrom";
		this.lb_InactiveRangeFrom.HideWhenEmpty = false;
		this.lb_InactiveRangeFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_InactiveRangeFrom.Location = new System.Drawing.Point(3, 32);
		this.lb_InactiveRangeFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_InactiveRangeFrom.Name = "lb_InactiveRangeFrom";
		this.lb_InactiveRangeFrom.ReadOnly = false;
		this.lb_InactiveRangeFrom.SettingInVisible = false;
		this.lb_InactiveRangeFrom.SettingReadonly = false;
		this.lb_InactiveRangeFrom.ShowUnderLine = true;
		this.lb_InactiveRangeFrom.Size = new System.Drawing.Size(220, 20);
		this.lb_InactiveRangeFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_InactiveRangeFrom.TabIndex = 0;
		this.lb_InactiveRangeFrom.TabStop = false;
		this.lb_InactiveRangeFrom.Utext = "Inactive Range            From";
		this.lb_ActiveRangeFrom.EditControl = this.dateValidFrom2;
		this.lb_ActiveRangeFrom.EditValue = "Active Range               From";
		this.lb_ActiveRangeFrom.FieldName = "lb_ActiveRangeFrom";
		this.lb_ActiveRangeFrom.HideWhenEmpty = false;
		this.lb_ActiveRangeFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActiveRangeFrom.Location = new System.Drawing.Point(3, 7);
		this.lb_ActiveRangeFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ActiveRangeFrom.Name = "lb_ActiveRangeFrom";
		this.lb_ActiveRangeFrom.ReadOnly = false;
		this.lb_ActiveRangeFrom.SettingInVisible = false;
		this.lb_ActiveRangeFrom.SettingReadonly = false;
		this.lb_ActiveRangeFrom.ShowUnderLine = true;
		this.lb_ActiveRangeFrom.Size = new System.Drawing.Size(220, 20);
		this.lb_ActiveRangeFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActiveRangeFrom.TabIndex = 0;
		this.lb_ActiveRangeFrom.TabStop = false;
		this.lb_ActiveRangeFrom.Utext = "Active Range               From";
		this.panel_Inactive.Controls.Add(this.txtFrozenComm);
		this.panel_Inactive.Controls.Add(this.lb_T);
		this.panel_Inactive.Controls.Add(this.lb_R);
		this.panel_Inactive.Controls.Add(this.lb_F);
		this.panel_Inactive.Controls.Add(this.dateFrozenTo);
		this.panel_Inactive.Controls.Add(this.dateFrozenFrom);
		this.panel_Inactive.Location = new System.Drawing.Point(133, 296);
		this.panel_Inactive.Name = "panel_Inactive";
		this.panel_Inactive.Size = new System.Drawing.Size(589, 31);
		this.panel_Inactive.TabIndex = 6;
		this.txtFrozenComm.EditControl = null;
		this.txtFrozenComm.FieldName = "FrozenComm";
		this.txtFrozenComm.IsSearchControl = false;
		this.txtFrozenComm.Location = new System.Drawing.Point(461, 6);
		this.txtFrozenComm.Name = "txtFrozenComm";
		this.txtFrozenComm.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtFrozenComm.Properties.Appearance.Options.UseFont = true;
		this.txtFrozenComm.Properties.AutoHeight = false;
		this.txtFrozenComm.SettingInVisible = false;
		this.txtFrozenComm.SettingReadonly = false;
		this.txtFrozenComm.Size = new System.Drawing.Size(125, 18);
		this.txtFrozenComm.TabIndex = 2;
		this.txtFrozenComm.Tag = "OITM,FrozenComm";
		this.txtFrozenComm.EditValueChanged += new System.EventHandler(this.txtFrozenComm_EditValueChanged);
		this.lb_T.EditControl = this.dateFrozenTo;
		this.lb_T.EditValue = "To";
		this.lb_T.FieldName = "lb_T";
		this.lb_T.HideWhenEmpty = false;
		this.lb_T.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_T.Location = new System.Drawing.Point(201, 7);
		this.lb_T.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_T.Name = "lb_T";
		this.lb_T.ReadOnly = false;
		this.lb_T.SettingInVisible = false;
		this.lb_T.SettingReadonly = false;
		this.lb_T.ShowUnderLine = true;
		this.lb_T.Size = new System.Drawing.Size(80, 20);
		this.lb_T.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_T.TabIndex = 0;
		this.lb_T.TabStop = false;
		this.lb_T.Utext = "To";
		this.dateFrozenTo.EditControl = null;
		this.dateFrozenTo.EditValue = null;
		this.dateFrozenTo.FieldName = "frozenTo";
		this.dateFrozenTo.Location = new System.Drawing.Point(281, 6);
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
		this.dateFrozenTo.Tag = "OITM,frozenTo";
		this.dateFrozenTo.EditValueChanged += new System.EventHandler(this.dateFrozenTo_EditValueChanged);
		this.dateFrozenTo.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.lb_R.EditControl = this.txtFrozenComm;
		this.lb_R.EditValue = "Remarks";
		this.lb_R.FieldName = "lb_R";
		this.lb_R.HideWhenEmpty = false;
		this.lb_R.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_R.Location = new System.Drawing.Point(381, 7);
		this.lb_R.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_R.Name = "lb_R";
		this.lb_R.ReadOnly = false;
		this.lb_R.SettingInVisible = false;
		this.lb_R.SettingReadonly = false;
		this.lb_R.ShowUnderLine = true;
		this.lb_R.Size = new System.Drawing.Size(80, 20);
		this.lb_R.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_R.TabIndex = 0;
		this.lb_R.TabStop = false;
		this.lb_R.Utext = "Remarks";
		this.lb_F.EditControl = this.dateFrozenFrom;
		this.lb_F.EditValue = "From";
		this.lb_F.FieldName = "lb_F";
		this.lb_F.HideWhenEmpty = false;
		this.lb_F.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_F.Location = new System.Drawing.Point(19, 7);
		this.lb_F.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_F.Name = "lb_F";
		this.lb_F.ReadOnly = false;
		this.lb_F.SettingInVisible = false;
		this.lb_F.SettingReadonly = false;
		this.lb_F.ShowUnderLine = true;
		this.lb_F.Size = new System.Drawing.Size(80, 20);
		this.lb_F.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_F.TabIndex = 0;
		this.lb_F.TabStop = false;
		this.lb_F.Utext = "From";
		this.dateFrozenFrom.EditControl = null;
		this.dateFrozenFrom.EditValue = null;
		this.dateFrozenFrom.FieldName = "frozenFrom";
		this.dateFrozenFrom.Location = new System.Drawing.Point(99, 6);
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
		this.dateFrozenFrom.Tag = "OITM,frozenFrom";
		this.dateFrozenFrom.EditValueChanged += new System.EventHandler(this.dateFrozenFrom_EditValueChanged);
		this.dateFrozenFrom.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.lb_Manufacture.EditControl = this.cbManufacturer;
		this.lb_Manufacture.EditValue = "Manufacturer";
		this.lb_Manufacture.FieldName = "lb_Manufacture";
		this.lb_Manufacture.HideWhenEmpty = false;
		this.lb_Manufacture.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Manufacture.Location = new System.Drawing.Point(9, 58);
		this.lb_Manufacture.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Manufacture.Name = "lb_Manufacture";
		this.lb_Manufacture.ReadOnly = false;
		this.lb_Manufacture.SettingInVisible = false;
		this.lb_Manufacture.SettingReadonly = false;
		this.lb_Manufacture.ShowUnderLine = true;
		this.lb_Manufacture.Size = new System.Drawing.Size(170, 20);
		this.lb_Manufacture.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Manufacture.TabIndex = 0;
		this.lb_Manufacture.TabStop = false;
		this.lb_Manufacture.Utext = "Manufacturer";
		this.panel_Active.Controls.Add(this.lbVailFrom);
		this.panel_Active.Controls.Add(this.lbRemarks);
		this.panel_Active.Controls.Add(this.dateValidFrom);
		this.panel_Active.Controls.Add(this.txtValidComm);
		this.panel_Active.Controls.Add(this.dateValidTo);
		this.panel_Active.Controls.Add(this.lbVailTo);
		this.panel_Active.Location = new System.Drawing.Point(133, 269);
		this.panel_Active.Name = "panel_Active";
		this.panel_Active.Size = new System.Drawing.Size(589, 32);
		this.panel_Active.TabIndex = 5;
		this.lbVailFrom.EditControl = this.dateValidFrom;
		this.lbVailFrom.EditValue = "From";
		this.lbVailFrom.FieldName = "lbVailFrom";
		this.lbVailFrom.HideWhenEmpty = false;
		this.lbVailFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbVailFrom.Location = new System.Drawing.Point(19, 7);
		this.lbVailFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbVailFrom.Name = "lbVailFrom";
		this.lbVailFrom.ReadOnly = false;
		this.lbVailFrom.SettingInVisible = false;
		this.lbVailFrom.SettingReadonly = false;
		this.lbVailFrom.ShowUnderLine = true;
		this.lbVailFrom.Size = new System.Drawing.Size(80, 20);
		this.lbVailFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbVailFrom.TabIndex = 0;
		this.lbVailFrom.TabStop = false;
		this.lbVailFrom.Utext = "From";
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
		this.dateValidFrom.Tag = "OITM,validFrom";
		this.dateValidFrom.EditValueChanged += new System.EventHandler(this.dateValidFrom_EditValueChanged);
		this.dateValidFrom.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.lbRemarks.EditControl = this.txtValidComm;
		this.lbRemarks.EditValue = "Remarks";
		this.lbRemarks.FieldName = "lbRemarks";
		this.lbRemarks.HideWhenEmpty = false;
		this.lbRemarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRemarks.Location = new System.Drawing.Point(381, 7);
		this.lbRemarks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbRemarks.Name = "lbRemarks";
		this.lbRemarks.ReadOnly = false;
		this.lbRemarks.SettingInVisible = false;
		this.lbRemarks.SettingReadonly = false;
		this.lbRemarks.ShowUnderLine = true;
		this.lbRemarks.Size = new System.Drawing.Size(80, 20);
		this.lbRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRemarks.TabIndex = 0;
		this.lbRemarks.TabStop = false;
		this.lbRemarks.Utext = "Remarks";
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
		this.txtValidComm.Size = new System.Drawing.Size(125, 18);
		this.txtValidComm.TabIndex = 2;
		this.txtValidComm.Tag = "OITM,ValidComm";
		this.txtValidComm.EditValueChanged += new System.EventHandler(this.txtValidComm_EditValueChanged);
		this.dateValidTo.EditControl = null;
		this.dateValidTo.EditValue = null;
		this.dateValidTo.FieldName = null;
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
		this.dateValidTo.Tag = "OITM,validTo";
		this.dateValidTo.EditValueChanged += new System.EventHandler(this.dateValidTo_EditValueChanged);
		this.dateValidTo.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.dateValidFrom_CustomDisplayText);
		this.lbVailTo.EditControl = this.dateValidTo;
		this.lbVailTo.EditValue = "To";
		this.lbVailTo.FieldName = "lbVailTo";
		this.lbVailTo.HideWhenEmpty = false;
		this.lbVailTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbVailTo.Location = new System.Drawing.Point(201, 6);
		this.lbVailTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbVailTo.Name = "lbVailTo";
		this.lbVailTo.ReadOnly = false;
		this.lbVailTo.SettingInVisible = false;
		this.lbVailTo.SettingReadonly = false;
		this.lbVailTo.ShowUnderLine = true;
		this.lbVailTo.Size = new System.Drawing.Size(80, 20);
		this.lbVailTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbVailTo.TabIndex = 0;
		this.lbVailTo.TabStop = false;
		this.lbVailTo.Utext = "To";
		this.lb_ManagementMethod.EditControl = this.cbManagementMethod;
		this.lb_ManagementMethod.EditValue = "Management Method";
		this.lb_ManagementMethod.FieldName = "lb_ManagementMethod";
		this.lb_ManagementMethod.HideWhenEmpty = false;
		this.lb_ManagementMethod.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ManagementMethod.Location = new System.Drawing.Point(9, 180);
		this.lb_ManagementMethod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ManagementMethod.Name = "lb_ManagementMethod";
		this.lb_ManagementMethod.ReadOnly = false;
		this.lb_ManagementMethod.SettingInVisible = false;
		this.lb_ManagementMethod.SettingReadonly = false;
		this.lb_ManagementMethod.ShowUnderLine = true;
		this.lb_ManagementMethod.Size = new System.Drawing.Size(170, 20);
		this.lb_ManagementMethod.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ManagementMethod.TabIndex = 0;
		this.lb_ManagementMethod.TabStop = false;
		this.lb_ManagementMethod.Utext = "Management Method";
		this.lb_ShippingType.EditControl = this.cbShippingType;
		this.lb_ShippingType.EditValue = "Shipping Type";
		this.lb_ShippingType.FieldName = "lb_ShippingType";
		this.lb_ShippingType.HideWhenEmpty = false;
		this.lb_ShippingType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ShippingType.Location = new System.Drawing.Point(9, 77);
		this.lb_ShippingType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ShippingType.Name = "lb_ShippingType";
		this.lb_ShippingType.ReadOnly = false;
		this.lb_ShippingType.SettingInVisible = false;
		this.lb_ShippingType.SettingReadonly = false;
		this.lb_ShippingType.ShowUnderLine = true;
		this.lb_ShippingType.Size = new System.Drawing.Size(170, 20);
		this.lb_ShippingType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ShippingType.TabIndex = 0;
		this.lb_ShippingType.TabStop = false;
		this.lb_ShippingType.Utext = "Shipping Type";
		this.lb_ManageItemBy.EditControl = this.cbManageItemBy;
		this.lb_ManageItemBy.EditValue = "Manage Item by";
		this.lb_ManageItemBy.FieldName = "lb_ManageItemBy";
		this.lb_ManageItemBy.HideWhenEmpty = false;
		this.lb_ManageItemBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ManageItemBy.Location = new System.Drawing.Point(9, 127);
		this.lb_ManageItemBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ManageItemBy.Name = "lb_ManageItemBy";
		this.lb_ManageItemBy.ReadOnly = false;
		this.lb_ManageItemBy.SettingInVisible = false;
		this.lb_ManageItemBy.SettingReadonly = false;
		this.lb_ManageItemBy.ShowUnderLine = true;
		this.lb_ManageItemBy.Size = new System.Drawing.Size(170, 20);
		this.lb_ManageItemBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ManageItemBy.TabIndex = 0;
		this.lb_ManageItemBy.TabStop = false;
		this.lb_ManageItemBy.Utext = "Manage Item by";
		this.lb_SerialAndBatchNum.EditControl = null;
		this.lb_SerialAndBatchNum.EditValue = "Serial and Batch Numbers";
		this.lb_SerialAndBatchNum.FieldName = "lb_SerialAndBatchNum";
		this.lb_SerialAndBatchNum.HideWhenEmpty = false;
		this.lb_SerialAndBatchNum.LineColor = System.Drawing.Color.Black;
		this.lb_SerialAndBatchNum.Location = new System.Drawing.Point(9, 103);
		this.lb_SerialAndBatchNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SerialAndBatchNum.Name = "lb_SerialAndBatchNum";
		this.lb_SerialAndBatchNum.ReadOnly = false;
		this.lb_SerialAndBatchNum.SettingInVisible = false;
		this.lb_SerialAndBatchNum.SettingReadonly = false;
		this.lb_SerialAndBatchNum.ShowUnderLine = true;
		this.lb_SerialAndBatchNum.Size = new System.Drawing.Size(128, 20);
		this.lb_SerialAndBatchNum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SerialAndBatchNum.TabIndex = 0;
		this.lb_SerialAndBatchNum.TabStop = false;
		this.lb_SerialAndBatchNum.Utext = "Serial and Batch Numbers";
		this.rdg_Active.EditControl = this.rdg_Active;
		this.rdg_Active.FieldName = null;
		this.rdg_Active.IsSearchControl = false;
		this.rdg_Active.Location = new System.Drawing.Point(11, 275);
		this.rdg_Active.Name = "rdg_Active";
		this.rdg_Active.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_Active.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_Active.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_Active.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Active", true, "lbActive"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Inactive", true, "lbInactive"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Advance", true, "lbAdvance")
		});
		this.rdg_Active.SettingInVisible = false;
		this.rdg_Active.SettingReadonly = false;
		this.rdg_Active.Size = new System.Drawing.Size(116, 82);
		this.rdg_Active.TabIndex = 4;
		this.rdg_Active.SelectedIndexChanged += new System.EventHandler(this.rdg_Active_SelectedIndexChanged);
		this.tab_Purchase.Controls.Add(this.calc_BWeight1);
		this.tab_Purchase.Controls.Add(this.calc_BHeight1);
		this.tab_Purchase.Controls.Add(this.calc_BWidth1);
		this.tab_Purchase.Controls.Add(this.calc_BLength1);
		this.tab_Purchase.Controls.Add(this.btn_PreferredVendor);
		this.tab_Purchase.Controls.Add(this.calc_CustomsGroup);
		this.tab_Purchase.Controls.Add(this.luk_CstGrpCode);
		this.tab_Purchase.Controls.Add(this.label3);
		this.tab_Purchase.Controls.Add(this.lb_CstGrpCode);
		this.tab_Purchase.Controls.Add(this.btn_PUoMForm);
		this.tab_Purchase.Controls.Add(this.txtBuyUnitMsr);
		this.tab_Purchase.Controls.Add(this.lbl_NumInBuy);
		this.tab_Purchase.Controls.Add(this.bte_PUoMEntry);
		this.tab_Purchase.Controls.Add(this.txtPurFactor2);
		this.tab_Purchase.Controls.Add(this.luk_BVolUnit);
		this.tab_Purchase.Controls.Add(this.btn_CopyToSalesData);
		this.tab_Purchase.Controls.Add(this.lb_BLength1);
		this.tab_Purchase.Controls.Add(this.calc_BVolume);
		this.tab_Purchase.Controls.Add(this.lb_BWeight1);
		this.tab_Purchase.Controls.Add(this.lb_BVolume);
		this.tab_Purchase.Controls.Add(this.lb_BHeight1);
		this.tab_Purchase.Controls.Add(this.lb_BWidth1);
		this.tab_Purchase.Controls.Add(this.lbl_PUoMEntry);
		this.tab_Purchase.Controls.Add(this.calc_VatPercentPurchase);
		this.tab_Purchase.Controls.Add(this.cbVatGroupPu);
		this.tab_Purchase.Controls.Add(this.txtCardCode);
		this.tab_Purchase.Controls.Add(this.txtPurPackUn);
		this.tab_Purchase.Controls.Add(this.txtNumInBuy);
		this.tab_Purchase.Controls.Add(this.txtPurPackMsr);
		this.tab_Purchase.Controls.Add(this.txtSuppCatNum);
		this.tab_Purchase.Controls.Add(this.label1);
		this.tab_Purchase.Controls.Add(this.lb_PreferredVendor);
		this.tab_Purchase.Controls.Add(this.txtPurFactor4);
		this.tab_Purchase.Controls.Add(this.txtPurFactor3);
		this.tab_Purchase.Controls.Add(this.txtPurFactor1);
		this.tab_Purchase.Controls.Add(this.lb_CatalogNo);
		this.tab_Purchase.Controls.Add(this.lb_Factor4);
		this.tab_Purchase.Controls.Add(this.lb_PurchasingUoM);
		this.tab_Purchase.Controls.Add(this.lb_Factor3);
		this.tab_Purchase.Controls.Add(this.lb_ItemsPerPurchaseUnit);
		this.tab_Purchase.Controls.Add(this.lb_Purchasing);
		this.tab_Purchase.Controls.Add(this.lb_Factor1);
		this.tab_Purchase.Controls.Add(this.lb_PackagingUoM);
		this.tab_Purchase.Controls.Add(this.lb_TaxGroup);
		this.tab_Purchase.Controls.Add(this.lb_Factor2);
		this.tab_Purchase.EditControl = null;
		this.tab_Purchase.EditValue = "Purchasing Data";
		this.tab_Purchase.FieldName = "tab_Purchase";
		this.tab_Purchase.IsSearchControl = false;
		this.tab_Purchase.Name = "tab_Purchase";
		this.tab_Purchase.ReadOnly = true;
		this.tab_Purchase.SettingInVisible = false;
		this.tab_Purchase.SettingReadonly = false;
		this.tab_Purchase.Size = new System.Drawing.Size(819, 422);
		this.tab_Purchase.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Purchase.Text = "Purchasing Data";
		this.calc_BWeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_BWeight1.EditControl = null;
		this.calc_BWeight1.FieldName = "BWeight1";
		this.calc_BWeight1.IsSearchControl = false;
		this.calc_BWeight1.Location = new System.Drawing.Point(594, 96);
		this.calc_BWeight1.Name = "calc_BWeight1";
		this.calc_BWeight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_BWeight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_BWeight1.SettingInVisible = false;
		this.calc_BWeight1.SettingReadonly = false;
		this.calc_BWeight1.Size = new System.Drawing.Size(142, 20);
		this.calc_BWeight1.TabIndex = 10;
		this.calc_BHeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_BHeight1.EditControl = null;
		this.calc_BHeight1.FieldName = "BHeight1";
		this.calc_BHeight1.IsSearchControl = false;
		this.calc_BHeight1.Location = new System.Drawing.Point(594, 59);
		this.calc_BHeight1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_BHeight1.Name = "calc_BHeight1";
		this.calc_BHeight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_BHeight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_BHeight1.SettingInVisible = false;
		this.calc_BHeight1.SettingReadonly = false;
		this.calc_BHeight1.Size = new System.Drawing.Size(142, 20);
		this.calc_BHeight1.TabIndex = 8;
		this.calc_BWidth1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_BWidth1.EditControl = null;
		this.calc_BWidth1.FieldName = "BWidth1";
		this.calc_BWidth1.IsSearchControl = false;
		this.calc_BWidth1.Location = new System.Drawing.Point(594, 39);
		this.calc_BWidth1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_BWidth1.Name = "calc_BWidth1";
		this.calc_BWidth1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_BWidth1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_BWidth1.SettingInVisible = false;
		this.calc_BWidth1.SettingReadonly = false;
		this.calc_BWidth1.Size = new System.Drawing.Size(142, 20);
		this.calc_BWidth1.TabIndex = 7;
		this.calc_BLength1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_BLength1.EditControl = null;
		this.calc_BLength1.FieldName = "BLength1";
		this.calc_BLength1.IsSearchControl = false;
		this.calc_BLength1.Location = new System.Drawing.Point(594, 19);
		this.calc_BLength1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_BLength1.Name = "calc_BLength1";
		this.calc_BLength1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_BLength1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_BLength1.SettingInVisible = false;
		this.calc_BLength1.SettingReadonly = false;
		this.calc_BLength1.Size = new System.Drawing.Size(142, 20);
		this.calc_BLength1.TabIndex = 6;
		this.btn_PreferredVendor.EditControl = null;
		this.btn_PreferredVendor.EditValue = "...";
		this.btn_PreferredVendor.FieldName = null;
		this.btn_PreferredVendor.IsSearchControl = false;
		this.btn_PreferredVendor.Location = new System.Drawing.Point(345, 9);
		this.btn_PreferredVendor.Name = "btn_PreferredVendor";
		this.btn_PreferredVendor.ReadOnly = false;
		this.btn_PreferredVendor.SettingInVisible = false;
		this.btn_PreferredVendor.SettingReadonly = false;
		this.btn_PreferredVendor.Size = new System.Drawing.Size(25, 23);
		this.btn_PreferredVendor.TabIndex = 99;
		this.btn_PreferredVendor.Text = "...";
		this.btn_PreferredVendor.Click += new System.EventHandler(this.btn_PreferredVendor_Click);
		this.calc_CustomsGroup.EditControl = null;
		this.calc_CustomsGroup.Enabled = false;
		this.calc_CustomsGroup.FieldName = null;
		this.calc_CustomsGroup.IsSearchControl = false;
		this.calc_CustomsGroup.Location = new System.Drawing.Point(348, 254);
		this.calc_CustomsGroup.Name = "calc_CustomsGroup";
		this.calc_CustomsGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_CustomsGroup.Properties.Appearance.Options.UseFont = true;
		this.calc_CustomsGroup.Properties.AutoHeight = false;
		this.calc_CustomsGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_CustomsGroup.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_CustomsGroup.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_CustomsGroup.Properties.IsSearchControl = false;
		this.calc_CustomsGroup.SettingInVisible = false;
		this.calc_CustomsGroup.SettingReadonly = false;
		this.calc_CustomsGroup.Size = new System.Drawing.Size(61, 18);
		this.calc_CustomsGroup.TabIndex = 17;
		this.calc_CustomsGroup.TabStop = false;
		this.calc_CustomsGroup.Tag = "OARG,TotalTax";
		this.luk_CstGrpCode.EditControl = null;
		this.luk_CstGrpCode.FieldName = "CstGrpCode";
		this.luk_CstGrpCode.IsSearchControl = false;
		this.luk_CstGrpCode.Location = new System.Drawing.Point(181, 254);
		this.luk_CstGrpCode.Name = "luk_CstGrpCode";
		this.luk_CstGrpCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_CstGrpCode.Properties.Appearance.Options.UseFont = true;
		this.luk_CstGrpCode.Properties.AutoHeight = false;
		this.luk_CstGrpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CstGrpCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Rate", "Rate")
		});
		this.luk_CstGrpCode.Properties.DisplayMember = "Name";
		this.luk_CstGrpCode.Properties.IsSearchControl = false;
		this.luk_CstGrpCode.Properties.NullText = "";
		this.luk_CstGrpCode.Properties.ValueMember = "Code";
		this.luk_CstGrpCode.SettingInVisible = false;
		this.luk_CstGrpCode.SettingReadonly = false;
		this.luk_CstGrpCode.Size = new System.Drawing.Size(161, 18);
		this.luk_CstGrpCode.TabIndex = 15;
		this.luk_CstGrpCode.Tag = "OITM,CstGrpCode";
		this.luk_CstGrpCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_CstGrpCode_EditValueChanging);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(411, 257);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(18, 13);
		this.label3.TabIndex = 98;
		this.label3.Text = "%";
		this.lb_CstGrpCode.EditControl = this.luk_CstGrpCode;
		this.lb_CstGrpCode.EditValue = "Customs Group";
		this.lb_CstGrpCode.FieldName = "lb_CstGrpCode";
		this.lb_CstGrpCode.HideWhenEmpty = false;
		this.lb_CstGrpCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CstGrpCode.Location = new System.Drawing.Point(8, 254);
		this.lb_CstGrpCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CstGrpCode.Name = "lb_CstGrpCode";
		this.lb_CstGrpCode.ReadOnly = false;
		this.lb_CstGrpCode.SettingInVisible = false;
		this.lb_CstGrpCode.SettingReadonly = false;
		this.lb_CstGrpCode.ShowUnderLine = true;
		this.lb_CstGrpCode.Size = new System.Drawing.Size(174, 20);
		this.lb_CstGrpCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CstGrpCode.TabIndex = 96;
		this.lb_CstGrpCode.TabStop = false;
		this.lb_CstGrpCode.Utext = "Customs Group";
		this.btn_PUoMForm.EditControl = null;
		this.btn_PUoMForm.EditValue = "...";
		this.btn_PUoMForm.FieldName = null;
		this.btn_PUoMForm.IsSearchControl = false;
		this.btn_PUoMForm.Location = new System.Drawing.Point(347, 81);
		this.btn_PUoMForm.Name = "btn_PUoMForm";
		this.btn_PUoMForm.ReadOnly = false;
		this.btn_PUoMForm.SettingInVisible = false;
		this.btn_PUoMForm.SettingReadonly = false;
		this.btn_PUoMForm.Size = new System.Drawing.Size(25, 23);
		this.btn_PUoMForm.TabIndex = 5;
		this.btn_PUoMForm.Text = "...";
		this.btn_PUoMForm.Click += new System.EventHandler(this.btn_PUoMForm_Click);
		this.txtBuyUnitMsr.EditControl = null;
		this.txtBuyUnitMsr.FieldName = "BuyUnitMsr";
		this.txtBuyUnitMsr.IsSearchControl = false;
		this.txtBuyUnitMsr.Location = new System.Drawing.Point(181, 103);
		this.txtBuyUnitMsr.Name = "txtBuyUnitMsr";
		this.txtBuyUnitMsr.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBuyUnitMsr.Properties.Appearance.Options.UseFont = true;
		this.txtBuyUnitMsr.Properties.AutoHeight = false;
		this.txtBuyUnitMsr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBuyUnitMsr.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.UomNameButtonEdit_Properties_ButtonPressed);
		this.txtBuyUnitMsr.SettingInVisible = false;
		this.txtBuyUnitMsr.SettingReadonly = false;
		this.txtBuyUnitMsr.Size = new System.Drawing.Size(161, 18);
		this.txtBuyUnitMsr.TabIndex = 3;
		this.txtBuyUnitMsr.Tag = "OITM,BuyUnitMsr";
		this.lbl_NumInBuy.EditControl = null;
		this.lbl_NumInBuy.EditValue = "1";
		this.lbl_NumInBuy.FieldName = null;
		this.lbl_NumInBuy.HideWhenEmpty = false;
		this.lbl_NumInBuy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_NumInBuy.Location = new System.Drawing.Point(345, 123);
		this.lbl_NumInBuy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_NumInBuy.Name = "lbl_NumInBuy";
		this.lbl_NumInBuy.ReadOnly = false;
		this.lbl_NumInBuy.SettingInVisible = false;
		this.lbl_NumInBuy.SettingReadonly = false;
		this.lbl_NumInBuy.ShowUnderLine = true;
		this.lbl_NumInBuy.Size = new System.Drawing.Size(63, 20);
		this.lbl_NumInBuy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_NumInBuy.TabIndex = 7;
		this.lbl_NumInBuy.TabStop = false;
		this.lbl_NumInBuy.Utext = "1";
		this.bte_PUoMEntry.EditControl = null;
		this.bte_PUoMEntry.FieldName = "PUoMEntry";
		this.bte_PUoMEntry.IsSearchControl = false;
		this.bte_PUoMEntry.Location = new System.Drawing.Point(181, 84);
		this.bte_PUoMEntry.Name = "bte_PUoMEntry";
		this.bte_PUoMEntry.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.bte_PUoMEntry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_PUoMEntry.Properties.Appearance.Options.UseFont = true;
		this.bte_PUoMEntry.Properties.AutoHeight = false;
		this.bte_PUoMEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_PUoMEntry.Properties.IsSearchControl = false;
		this.bte_PUoMEntry.Properties.NullText = "";
		this.bte_PUoMEntry.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_PUoMEntry_Properties_ButtonPressed);
		this.bte_PUoMEntry.SettingInVisible = false;
		this.bte_PUoMEntry.SettingReadonly = false;
		this.bte_PUoMEntry.Size = new System.Drawing.Size(161, 18);
		this.bte_PUoMEntry.TabIndex = 2;
		this.bte_PUoMEntry.Tag = "";
		this.bte_PUoMEntry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.bte_PUoMEntry_QueryPopUp);
		this.bte_PUoMEntry.EditValueChanged += new System.EventHandler(this.bte_PUoMEntry_EditValueChanged);
		this.txtPurFactor2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPurFactor2.EditControl = null;
		this.txtPurFactor2.FieldName = "PurFactor2";
		this.txtPurFactor2.IsSearchControl = false;
		this.txtPurFactor2.Location = new System.Drawing.Point(593, 251);
		this.txtPurFactor2.Name = "txtPurFactor2";
		this.txtPurFactor2.Properties.AutoHeight = false;
		this.txtPurFactor2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPurFactor2.Properties.DisplayFormat.FormatString = "n3";
		this.txtPurFactor2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor2.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtPurFactor2.Properties.EditFormat.FormatString = "n3";
		this.txtPurFactor2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor2.Properties.IsSearchControl = false;
		this.txtPurFactor2.SettingInVisible = false;
		this.txtPurFactor2.SettingReadonly = false;
		this.txtPurFactor2.Size = new System.Drawing.Size(143, 18);
		this.txtPurFactor2.TabIndex = 18;
		this.luk_BVolUnit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_BVolUnit.EditControl = null;
		this.luk_BVolUnit.FieldName = "BVolUnit";
		this.luk_BVolUnit.IsSearchControl = false;
		this.luk_BVolUnit.Location = new System.Drawing.Point(743, 79);
		this.luk_BVolUnit.Name = "luk_BVolUnit";
		this.luk_BVolUnit.Properties.AutoHeight = false;
		this.luk_BVolUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BVolUnit.Properties.IsSearchControl = false;
		this.luk_BVolUnit.Properties.NullText = "";
		this.luk_BVolUnit.Properties.PopupFormMinSize = new System.Drawing.Size(35, 150);
		this.luk_BVolUnit.Properties.PopupWidth = 50;
		this.luk_BVolUnit.SettingInVisible = false;
		this.luk_BVolUnit.SettingReadonly = false;
		this.luk_BVolUnit.Size = new System.Drawing.Size(44, 18);
		this.luk_BVolUnit.TabIndex = 12;
		this.btn_CopyToSalesData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyToSalesData.EditControl = null;
		this.btn_CopyToSalesData.EditValue = ">>";
		this.btn_CopyToSalesData.FieldName = null;
		this.btn_CopyToSalesData.IsSearchControl = false;
		this.btn_CopyToSalesData.Location = new System.Drawing.Point(743, 20);
		this.btn_CopyToSalesData.Name = "btn_CopyToSalesData";
		this.btn_CopyToSalesData.ReadOnly = false;
		this.btn_CopyToSalesData.SettingInVisible = false;
		this.btn_CopyToSalesData.SettingReadonly = false;
		this.btn_CopyToSalesData.Size = new System.Drawing.Size(44, 23);
		this.btn_CopyToSalesData.TabIndex = 11;
		this.btn_CopyToSalesData.Text = ">>";
		this.btn_CopyToSalesData.Click += new System.EventHandler(this.btn_CopyToSalesData_Click);
		this.lb_BLength1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BLength1.EditControl = this.calc_BLength1;
		this.lb_BLength1.EditValue = "Length";
		this.lb_BLength1.FieldName = "lb_BLength1";
		this.lb_BLength1.HideWhenEmpty = false;
		this.lb_BLength1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BLength1.Location = new System.Drawing.Point(477, 23);
		this.lb_BLength1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_BLength1.Name = "lb_BLength1";
		this.lb_BLength1.ReadOnly = false;
		this.lb_BLength1.SettingInVisible = false;
		this.lb_BLength1.SettingReadonly = false;
		this.lb_BLength1.ShowUnderLine = true;
		this.lb_BLength1.Size = new System.Drawing.Size(117, 20);
		this.lb_BLength1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BLength1.TabIndex = 73;
		this.lb_BLength1.TabStop = false;
		this.lb_BLength1.Utext = "Length";
		this.calc_BVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_BVolume.EditControl = null;
		this.calc_BVolume.FieldName = "BVolume";
		this.calc_BVolume.IsSearchControl = false;
		this.calc_BVolume.Location = new System.Drawing.Point(594, 79);
		this.calc_BVolume.Name = "calc_BVolume";
		this.calc_BVolume.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_BVolume.Properties.Appearance.Options.UseFont = true;
		this.calc_BVolume.Properties.AutoHeight = false;
		this.calc_BVolume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_BVolume.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_BVolume.Properties.IsSearchControl = false;
		this.calc_BVolume.SettingInVisible = false;
		this.calc_BVolume.SettingReadonly = false;
		this.calc_BVolume.Size = new System.Drawing.Size(142, 18);
		this.calc_BVolume.TabIndex = 9;
		this.lb_BWeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BWeight1.EditControl = this.calc_BWeight1;
		this.lb_BWeight1.EditValue = "Weight";
		this.lb_BWeight1.FieldName = "lb_BWeight1";
		this.lb_BWeight1.HideWhenEmpty = false;
		this.lb_BWeight1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BWeight1.Location = new System.Drawing.Point(477, 99);
		this.lb_BWeight1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_BWeight1.Name = "lb_BWeight1";
		this.lb_BWeight1.ReadOnly = false;
		this.lb_BWeight1.SettingInVisible = false;
		this.lb_BWeight1.SettingReadonly = false;
		this.lb_BWeight1.ShowUnderLine = true;
		this.lb_BWeight1.Size = new System.Drawing.Size(117, 20);
		this.lb_BWeight1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BWeight1.TabIndex = 71;
		this.lb_BWeight1.TabStop = false;
		this.lb_BWeight1.Utext = "Weight";
		this.lb_BVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BVolume.EditControl = this.calc_BVolume;
		this.lb_BVolume.EditValue = "Volume";
		this.lb_BVolume.FieldName = "BVolume";
		this.lb_BVolume.HideWhenEmpty = false;
		this.lb_BVolume.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BVolume.Location = new System.Drawing.Point(477, 80);
		this.lb_BVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_BVolume.Name = "lb_BVolume";
		this.lb_BVolume.ReadOnly = false;
		this.lb_BVolume.SettingInVisible = false;
		this.lb_BVolume.SettingReadonly = false;
		this.lb_BVolume.ShowUnderLine = true;
		this.lb_BVolume.Size = new System.Drawing.Size(117, 20);
		this.lb_BVolume.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BVolume.TabIndex = 70;
		this.lb_BVolume.TabStop = false;
		this.lb_BVolume.Utext = "Volume";
		this.lb_BHeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BHeight1.EditControl = this.calc_BHeight1;
		this.lb_BHeight1.EditValue = "Height";
		this.lb_BHeight1.FieldName = "lb_BHeight1";
		this.lb_BHeight1.HideWhenEmpty = false;
		this.lb_BHeight1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BHeight1.Location = new System.Drawing.Point(477, 61);
		this.lb_BHeight1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_BHeight1.Name = "lb_BHeight1";
		this.lb_BHeight1.ReadOnly = false;
		this.lb_BHeight1.SettingInVisible = false;
		this.lb_BHeight1.SettingReadonly = false;
		this.lb_BHeight1.ShowUnderLine = true;
		this.lb_BHeight1.Size = new System.Drawing.Size(117, 20);
		this.lb_BHeight1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BHeight1.TabIndex = 69;
		this.lb_BHeight1.TabStop = false;
		this.lb_BHeight1.Utext = "Height";
		this.lb_BWidth1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BWidth1.EditControl = this.calc_BWidth1;
		this.lb_BWidth1.EditValue = "Width";
		this.lb_BWidth1.FieldName = "lb_BWidth1";
		this.lb_BWidth1.HideWhenEmpty = false;
		this.lb_BWidth1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BWidth1.Location = new System.Drawing.Point(477, 42);
		this.lb_BWidth1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_BWidth1.Name = "lb_BWidth1";
		this.lb_BWidth1.ReadOnly = false;
		this.lb_BWidth1.SettingInVisible = false;
		this.lb_BWidth1.SettingReadonly = false;
		this.lb_BWidth1.ShowUnderLine = true;
		this.lb_BWidth1.Size = new System.Drawing.Size(117, 20);
		this.lb_BWidth1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BWidth1.TabIndex = 68;
		this.lb_BWidth1.TabStop = false;
		this.lb_BWidth1.Utext = "Width";
		this.lbl_PUoMEntry.EditControl = this.bte_PUoMEntry;
		this.lbl_PUoMEntry.EditValue = "Purchasing UoM Code";
		this.lbl_PUoMEntry.FieldName = "lbl_PUoMEntry";
		this.lbl_PUoMEntry.HideWhenEmpty = false;
		this.lbl_PUoMEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PUoMEntry.Location = new System.Drawing.Point(8, 85);
		this.lbl_PUoMEntry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_PUoMEntry.Name = "lbl_PUoMEntry";
		this.lbl_PUoMEntry.ReadOnly = false;
		this.lbl_PUoMEntry.SettingInVisible = false;
		this.lbl_PUoMEntry.SettingReadonly = false;
		this.lbl_PUoMEntry.ShowUnderLine = true;
		this.lbl_PUoMEntry.Size = new System.Drawing.Size(174, 20);
		this.lbl_PUoMEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PUoMEntry.TabIndex = 63;
		this.lbl_PUoMEntry.TabStop = false;
		this.lbl_PUoMEntry.Utext = "Purchasing UoM Code";
		this.calc_VatPercentPurchase.EditControl = null;
		this.calc_VatPercentPurchase.Enabled = false;
		this.calc_VatPercentPurchase.FieldName = null;
		this.calc_VatPercentPurchase.IsSearchControl = false;
		this.calc_VatPercentPurchase.Location = new System.Drawing.Point(348, 273);
		this.calc_VatPercentPurchase.Name = "calc_VatPercentPurchase";
		this.calc_VatPercentPurchase.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_VatPercentPurchase.Properties.Appearance.Options.UseFont = true;
		this.calc_VatPercentPurchase.Properties.AutoHeight = false;
		this.calc_VatPercentPurchase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_VatPercentPurchase.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_VatPercentPurchase.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_VatPercentPurchase.Properties.IsSearchControl = false;
		this.calc_VatPercentPurchase.SettingInVisible = false;
		this.calc_VatPercentPurchase.SettingReadonly = false;
		this.calc_VatPercentPurchase.Size = new System.Drawing.Size(61, 18);
		this.calc_VatPercentPurchase.TabIndex = 18;
		this.calc_VatPercentPurchase.TabStop = false;
		this.calc_VatPercentPurchase.Tag = "OVTG,Rate";
		this.cbVatGroupPu.EditControl = null;
		this.cbVatGroupPu.FieldName = "VatGroupPu";
		this.cbVatGroupPu.IsSearchControl = false;
		this.cbVatGroupPu.Location = new System.Drawing.Point(181, 273);
		this.cbVatGroupPu.Name = "cbVatGroupPu";
		this.cbVatGroupPu.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbVatGroupPu.Properties.Appearance.Options.UseFont = true;
		this.cbVatGroupPu.Properties.AutoHeight = false;
		this.cbVatGroupPu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbVatGroupPu.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Rate", "Rate")
		});
		this.cbVatGroupPu.Properties.DisplayMember = "Name";
		this.cbVatGroupPu.Properties.IsSearchControl = false;
		this.cbVatGroupPu.Properties.NullText = "";
		this.cbVatGroupPu.Properties.ValueMember = "Code";
		this.cbVatGroupPu.SettingInVisible = false;
		this.cbVatGroupPu.SettingReadonly = false;
		this.cbVatGroupPu.Size = new System.Drawing.Size(161, 18);
		this.cbVatGroupPu.TabIndex = 16;
		this.cbVatGroupPu.Tag = "OITM,VatGroupPu";
		this.cbVatGroupPu.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbVatGroupPu_EditValueChanging);
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = "CardCode";
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(181, 12);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtCardCode.Properties.Appearance.Options.UseFont = true;
		this.txtCardCode.Properties.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCardCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardCode_Properties_ButtonPressed);
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(161, 18);
		this.txtCardCode.TabIndex = 0;
		this.txtCardCode.Tag = "OITM,CardCode";
		this.txtCardCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtCardCode_InvalidValue);
		this.txtCardCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtCardCode_Validating);
		this.txtPurPackUn.EditControl = null;
		this.txtPurPackUn.FieldName = "PurPackUn";
		this.txtPurPackUn.IsSearchControl = false;
		this.txtPurPackUn.Location = new System.Drawing.Point(181, 195);
		this.txtPurPackUn.Name = "txtPurPackUn";
		this.txtPurPackUn.Properties.AutoHeight = false;
		this.txtPurPackUn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPurPackUn.Properties.DisplayFormat.FormatString = "n3";
		this.txtPurPackUn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurPackUn.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtPurPackUn.Properties.EditFormat.FormatString = "n3";
		this.txtPurPackUn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurPackUn.Properties.IsSearchControl = false;
		this.txtPurPackUn.SettingInVisible = false;
		this.txtPurPackUn.SettingReadonly = false;
		this.txtPurPackUn.Size = new System.Drawing.Size(161, 18);
		this.txtPurPackUn.TabIndex = 14;
		this.txtPurPackUn.Tag = "OITM,PurPackUn";
		this.txtNumInBuy.EditControl = null;
		this.txtNumInBuy.FieldName = "NumInBuy";
		this.txtNumInBuy.IsSearchControl = false;
		this.txtNumInBuy.Location = new System.Drawing.Point(181, 122);
		this.txtNumInBuy.Name = "txtNumInBuy";
		this.txtNumInBuy.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtNumInBuy.Properties.Appearance.Options.UseFont = true;
		this.txtNumInBuy.Properties.AutoHeight = false;
		this.txtNumInBuy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtNumInBuy.Properties.DisplayFormat.FormatString = "n3";
		this.txtNumInBuy.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtNumInBuy.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.txtNumInBuy.Properties.EditFormat.FormatString = "n3";
		this.txtNumInBuy.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtNumInBuy.Properties.IsSearchControl = false;
		this.txtNumInBuy.SettingInVisible = false;
		this.txtNumInBuy.SettingReadonly = false;
		this.txtNumInBuy.Size = new System.Drawing.Size(161, 18);
		this.txtNumInBuy.TabIndex = 4;
		this.txtNumInBuy.Tag = "OITM,NumInBuy";
		this.txtPurPackMsr.EditControl = null;
		this.txtPurPackMsr.FieldName = "PurPackMsr";
		this.txtPurPackMsr.IsSearchControl = false;
		this.txtPurPackMsr.Location = new System.Drawing.Point(181, 176);
		this.txtPurPackMsr.Name = "txtPurPackMsr";
		this.txtPurPackMsr.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPurPackMsr.Properties.Appearance.Options.UseFont = true;
		this.txtPurPackMsr.Properties.AutoHeight = false;
		this.txtPurPackMsr.SettingInVisible = false;
		this.txtPurPackMsr.SettingReadonly = false;
		this.txtPurPackMsr.Size = new System.Drawing.Size(161, 18);
		this.txtPurPackMsr.TabIndex = 13;
		this.txtSuppCatNum.EditControl = null;
		this.txtSuppCatNum.FieldName = "SuppCatNum";
		this.txtSuppCatNum.IsSearchControl = false;
		this.txtSuppCatNum.Location = new System.Drawing.Point(181, 51);
		this.txtSuppCatNum.Name = "txtSuppCatNum";
		this.txtSuppCatNum.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSuppCatNum.Properties.Appearance.Options.UseFont = true;
		this.txtSuppCatNum.Properties.AutoHeight = false;
		this.txtSuppCatNum.SettingInVisible = false;
		this.txtSuppCatNum.SettingReadonly = false;
		this.txtSuppCatNum.Size = new System.Drawing.Size(161, 18);
		this.txtSuppCatNum.TabIndex = 1;
		this.txtSuppCatNum.Tag = "OITM,SuppCatNum";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(411, 276);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(18, 13);
		this.label1.TabIndex = 37;
		this.label1.Text = "%";
		this.lb_PreferredVendor.EditControl = this.txtCardCode;
		this.lb_PreferredVendor.EditValue = "Preferred Vendor";
		this.lb_PreferredVendor.FieldName = "lb_PreferredVendor";
		this.lb_PreferredVendor.HideWhenEmpty = false;
		this.lb_PreferredVendor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PreferredVendor.Location = new System.Drawing.Point(8, 15);
		this.lb_PreferredVendor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PreferredVendor.Name = "lb_PreferredVendor";
		this.lb_PreferredVendor.ReadOnly = false;
		this.lb_PreferredVendor.SettingInVisible = false;
		this.lb_PreferredVendor.SettingReadonly = false;
		this.lb_PreferredVendor.ShowUnderLine = true;
		this.lb_PreferredVendor.Size = new System.Drawing.Size(174, 20);
		this.lb_PreferredVendor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PreferredVendor.TabIndex = 17;
		this.lb_PreferredVendor.TabStop = false;
		this.lb_PreferredVendor.Utext = "Preferred Vendor";
		this.txtPurFactor4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPurFactor4.EditControl = null;
		this.txtPurFactor4.FieldName = "PurFactor4";
		this.txtPurFactor4.IsSearchControl = false;
		this.txtPurFactor4.Location = new System.Drawing.Point(593, 291);
		this.txtPurFactor4.Name = "txtPurFactor4";
		this.txtPurFactor4.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPurFactor4.Properties.Appearance.Options.UseFont = true;
		this.txtPurFactor4.Properties.AutoHeight = false;
		this.txtPurFactor4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPurFactor4.Properties.DisplayFormat.FormatString = "n3";
		this.txtPurFactor4.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor4.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtPurFactor4.Properties.EditFormat.FormatString = "n3";
		this.txtPurFactor4.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor4.Properties.IsSearchControl = false;
		this.txtPurFactor4.SettingInVisible = false;
		this.txtPurFactor4.SettingReadonly = false;
		this.txtPurFactor4.Size = new System.Drawing.Size(143, 18);
		this.txtPurFactor4.TabIndex = 20;
		this.txtPurFactor3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPurFactor3.EditControl = null;
		this.txtPurFactor3.FieldName = "PurFactor3";
		this.txtPurFactor3.IsSearchControl = false;
		this.txtPurFactor3.Location = new System.Drawing.Point(593, 271);
		this.txtPurFactor3.Name = "txtPurFactor3";
		this.txtPurFactor3.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPurFactor3.Properties.Appearance.Options.UseFont = true;
		this.txtPurFactor3.Properties.AutoHeight = false;
		this.txtPurFactor3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPurFactor3.Properties.DisplayFormat.FormatString = "n3";
		this.txtPurFactor3.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor3.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtPurFactor3.Properties.EditFormat.FormatString = "n3";
		this.txtPurFactor3.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor3.Properties.IsSearchControl = false;
		this.txtPurFactor3.SettingInVisible = false;
		this.txtPurFactor3.SettingReadonly = false;
		this.txtPurFactor3.Size = new System.Drawing.Size(143, 18);
		this.txtPurFactor3.TabIndex = 19;
		this.txtPurFactor1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtPurFactor1.EditControl = null;
		this.txtPurFactor1.FieldName = "PurFactor1";
		this.txtPurFactor1.IsSearchControl = false;
		this.txtPurFactor1.Location = new System.Drawing.Point(593, 231);
		this.txtPurFactor1.Name = "txtPurFactor1";
		this.txtPurFactor1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPurFactor1.Properties.Appearance.Options.UseFont = true;
		this.txtPurFactor1.Properties.AutoHeight = false;
		this.txtPurFactor1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPurFactor1.Properties.DisplayFormat.FormatString = "n3";
		this.txtPurFactor1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor1.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtPurFactor1.Properties.EditFormat.FormatString = "n3";
		this.txtPurFactor1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPurFactor1.Properties.IsSearchControl = false;
		this.txtPurFactor1.SettingInVisible = false;
		this.txtPurFactor1.SettingReadonly = false;
		this.txtPurFactor1.Size = new System.Drawing.Size(143, 18);
		this.txtPurFactor1.TabIndex = 17;
		this.lb_CatalogNo.EditControl = this.txtSuppCatNum;
		this.lb_CatalogNo.EditValue = "Mfr Catalog No.";
		this.lb_CatalogNo.FieldName = "lb_CatalogNo";
		this.lb_CatalogNo.HideWhenEmpty = false;
		this.lb_CatalogNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CatalogNo.Location = new System.Drawing.Point(8, 52);
		this.lb_CatalogNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CatalogNo.Name = "lb_CatalogNo";
		this.lb_CatalogNo.ReadOnly = false;
		this.lb_CatalogNo.SettingInVisible = false;
		this.lb_CatalogNo.SettingReadonly = false;
		this.lb_CatalogNo.ShowUnderLine = true;
		this.lb_CatalogNo.Size = new System.Drawing.Size(174, 20);
		this.lb_CatalogNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CatalogNo.TabIndex = 14;
		this.lb_CatalogNo.TabStop = false;
		this.lb_CatalogNo.Utext = "Mfr Catalog No.";
		this.lb_Factor4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor4.EditControl = this.txtPurFactor4;
		this.lb_Factor4.EditValue = "Factor 4";
		this.lb_Factor4.FieldName = "lb_Factor4";
		this.lb_Factor4.HideWhenEmpty = false;
		this.lb_Factor4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor4.Location = new System.Drawing.Point(477, 292);
		this.lb_Factor4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor4.Name = "lb_Factor4";
		this.lb_Factor4.ReadOnly = false;
		this.lb_Factor4.SettingInVisible = false;
		this.lb_Factor4.SettingReadonly = false;
		this.lb_Factor4.ShowUnderLine = true;
		this.lb_Factor4.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor4.TabIndex = 31;
		this.lb_Factor4.TabStop = false;
		this.lb_Factor4.Utext = "Factor 4";
		this.lb_PurchasingUoM.EditControl = this.txtBuyUnitMsr;
		this.lb_PurchasingUoM.EditValue = "Purchasing UoM Name";
		this.lb_PurchasingUoM.FieldName = "lb_PurchasingUoM";
		this.lb_PurchasingUoM.HideWhenEmpty = false;
		this.lb_PurchasingUoM.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PurchasingUoM.Location = new System.Drawing.Point(8, 104);
		this.lb_PurchasingUoM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PurchasingUoM.Name = "lb_PurchasingUoM";
		this.lb_PurchasingUoM.ReadOnly = false;
		this.lb_PurchasingUoM.SettingInVisible = false;
		this.lb_PurchasingUoM.SettingReadonly = false;
		this.lb_PurchasingUoM.ShowUnderLine = true;
		this.lb_PurchasingUoM.Size = new System.Drawing.Size(174, 20);
		this.lb_PurchasingUoM.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PurchasingUoM.TabIndex = 18;
		this.lb_PurchasingUoM.TabStop = false;
		this.lb_PurchasingUoM.Utext = "Purchasing UoM Name";
		this.lb_Factor3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor3.EditControl = this.txtPurFactor3;
		this.lb_Factor3.EditValue = "Factor 3";
		this.lb_Factor3.FieldName = "lb_Factor3";
		this.lb_Factor3.HideWhenEmpty = false;
		this.lb_Factor3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor3.Location = new System.Drawing.Point(477, 272);
		this.lb_Factor3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor3.Name = "lb_Factor3";
		this.lb_Factor3.ReadOnly = false;
		this.lb_Factor3.SettingInVisible = false;
		this.lb_Factor3.SettingReadonly = false;
		this.lb_Factor3.ShowUnderLine = true;
		this.lb_Factor3.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor3.TabIndex = 30;
		this.lb_Factor3.TabStop = false;
		this.lb_Factor3.Utext = "Factor 3";
		this.lb_ItemsPerPurchaseUnit.EditControl = this.txtNumInBuy;
		this.lb_ItemsPerPurchaseUnit.EditValue = "Items per Purchase Unit";
		this.lb_ItemsPerPurchaseUnit.FieldName = "lb_ItemsPerPurchaseUnit";
		this.lb_ItemsPerPurchaseUnit.HideWhenEmpty = false;
		this.lb_ItemsPerPurchaseUnit.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemsPerPurchaseUnit.Location = new System.Drawing.Point(8, 123);
		this.lb_ItemsPerPurchaseUnit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ItemsPerPurchaseUnit.Name = "lb_ItemsPerPurchaseUnit";
		this.lb_ItemsPerPurchaseUnit.ReadOnly = false;
		this.lb_ItemsPerPurchaseUnit.SettingInVisible = false;
		this.lb_ItemsPerPurchaseUnit.SettingReadonly = false;
		this.lb_ItemsPerPurchaseUnit.ShowUnderLine = true;
		this.lb_ItemsPerPurchaseUnit.Size = new System.Drawing.Size(174, 20);
		this.lb_ItemsPerPurchaseUnit.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemsPerPurchaseUnit.TabIndex = 15;
		this.lb_ItemsPerPurchaseUnit.TabStop = false;
		this.lb_ItemsPerPurchaseUnit.Utext = "Items per Purchase Unit";
		this.lb_Purchasing.EditControl = this.txtPurPackMsr;
		this.lb_Purchasing.EditValue = "Packaging UoM Name";
		this.lb_Purchasing.FieldName = "lb_Purchasing";
		this.lb_Purchasing.HideWhenEmpty = false;
		this.lb_Purchasing.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Purchasing.Location = new System.Drawing.Point(8, 177);
		this.lb_Purchasing.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Purchasing.Name = "lb_Purchasing";
		this.lb_Purchasing.ReadOnly = false;
		this.lb_Purchasing.SettingInVisible = false;
		this.lb_Purchasing.SettingReadonly = false;
		this.lb_Purchasing.ShowUnderLine = true;
		this.lb_Purchasing.Size = new System.Drawing.Size(174, 20);
		this.lb_Purchasing.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Purchasing.TabIndex = 19;
		this.lb_Purchasing.TabStop = false;
		this.lb_Purchasing.Utext = "Packaging UoM Name";
		this.lb_Factor1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor1.EditControl = this.txtPurFactor1;
		this.lb_Factor1.EditValue = "Factor 1";
		this.lb_Factor1.FieldName = "lb_Factor1";
		this.lb_Factor1.HideWhenEmpty = false;
		this.lb_Factor1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor1.Location = new System.Drawing.Point(477, 232);
		this.lb_Factor1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor1.Name = "lb_Factor1";
		this.lb_Factor1.ReadOnly = false;
		this.lb_Factor1.SettingInVisible = false;
		this.lb_Factor1.SettingReadonly = false;
		this.lb_Factor1.ShowUnderLine = true;
		this.lb_Factor1.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor1.TabIndex = 28;
		this.lb_Factor1.TabStop = false;
		this.lb_Factor1.Utext = "Factor 1";
		this.lb_PackagingUoM.EditControl = this.txtPurPackUn;
		this.lb_PackagingUoM.EditValue = "Quantity per Packaging UoM";
		this.lb_PackagingUoM.FieldName = "lb_PackagingUoM";
		this.lb_PackagingUoM.HideWhenEmpty = false;
		this.lb_PackagingUoM.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PackagingUoM.Location = new System.Drawing.Point(8, 196);
		this.lb_PackagingUoM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PackagingUoM.Name = "lb_PackagingUoM";
		this.lb_PackagingUoM.ReadOnly = false;
		this.lb_PackagingUoM.SettingInVisible = false;
		this.lb_PackagingUoM.SettingReadonly = false;
		this.lb_PackagingUoM.ShowUnderLine = true;
		this.lb_PackagingUoM.Size = new System.Drawing.Size(174, 20);
		this.lb_PackagingUoM.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PackagingUoM.TabIndex = 16;
		this.lb_PackagingUoM.TabStop = false;
		this.lb_PackagingUoM.Utext = "Quantity per Packaging UoM";
		this.lb_TaxGroup.EditControl = this.cbVatGroupPu;
		this.lb_TaxGroup.EditValue = "Tax Group";
		this.lb_TaxGroup.FieldName = "lb_TaxGroup";
		this.lb_TaxGroup.HideWhenEmpty = false;
		this.lb_TaxGroup.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TaxGroup.Location = new System.Drawing.Point(8, 274);
		this.lb_TaxGroup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TaxGroup.Name = "lb_TaxGroup";
		this.lb_TaxGroup.ReadOnly = false;
		this.lb_TaxGroup.SettingInVisible = false;
		this.lb_TaxGroup.SettingReadonly = false;
		this.lb_TaxGroup.ShowUnderLine = true;
		this.lb_TaxGroup.Size = new System.Drawing.Size(174, 20);
		this.lb_TaxGroup.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TaxGroup.TabIndex = 20;
		this.lb_TaxGroup.TabStop = false;
		this.lb_TaxGroup.Utext = "Tax Group";
		this.lb_Factor2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor2.EditControl = this.txtPurFactor2;
		this.lb_Factor2.EditValue = "Factor 2";
		this.lb_Factor2.FieldName = "lb_Factor2";
		this.lb_Factor2.HideWhenEmpty = false;
		this.lb_Factor2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor2.Location = new System.Drawing.Point(477, 252);
		this.lb_Factor2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor2.Name = "lb_Factor2";
		this.lb_Factor2.ReadOnly = false;
		this.lb_Factor2.SettingInVisible = false;
		this.lb_Factor2.SettingReadonly = false;
		this.lb_Factor2.ShowUnderLine = true;
		this.lb_Factor2.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor2.TabIndex = 29;
		this.lb_Factor2.TabStop = false;
		this.lb_Factor2.Utext = "Factor 2";
		this.tab_Sales.Controls.Add(this.calc_SWeight1);
		this.tab_Sales.Controls.Add(this.calc_SHeight1);
		this.tab_Sales.Controls.Add(this.calc_SWidth1);
		this.tab_Sales.Controls.Add(this.calc_SLength1);
		this.tab_Sales.Controls.Add(this.btn_SUoMForm);
		this.tab_Sales.Controls.Add(this.txtSalUnitMsr);
		this.tab_Sales.Controls.Add(this.lbl_NumInSale);
		this.tab_Sales.Controls.Add(this.bte_SUoMEntry);
		this.tab_Sales.Controls.Add(this.lbl_SUoMEntry);
		this.tab_Sales.Controls.Add(this.luk_SVolUnit);
		this.tab_Sales.Controls.Add(this.lb_SLength1);
		this.tab_Sales.Controls.Add(this.calc_SVolume);
		this.tab_Sales.Controls.Add(this.lb_SWeight1);
		this.tab_Sales.Controls.Add(this.lb_SVolume);
		this.tab_Sales.Controls.Add(this.lb_SHeight1);
		this.tab_Sales.Controls.Add(this.lb_SWidth1);
		this.tab_Sales.Controls.Add(this.calc_VatPercentSales);
		this.tab_Sales.Controls.Add(this.cbVatGourpSa);
		this.tab_Sales.Controls.Add(this.label2);
		this.tab_Sales.Controls.Add(this.txtSalPackUn);
		this.tab_Sales.Controls.Add(this.txtNumInSale);
		this.tab_Sales.Controls.Add(this.txtSalPackMsr);
		this.tab_Sales.Controls.Add(this.lb_SalesUoM);
		this.tab_Sales.Controls.Add(this.txtSalFactor4);
		this.tab_Sales.Controls.Add(this.txtSalFactor3);
		this.tab_Sales.Controls.Add(this.txtSalFactor2);
		this.tab_Sales.Controls.Add(this.lb_ItemsPerSalesUnit);
		this.tab_Sales.Controls.Add(this.txtSalFactor1);
		this.tab_Sales.Controls.Add(this.lb_Sales);
		this.tab_Sales.Controls.Add(this.lb_Factor4S);
		this.tab_Sales.Controls.Add(this.lb_PackagingUoMS);
		this.tab_Sales.Controls.Add(this.lb_Factor3S);
		this.tab_Sales.Controls.Add(this.lb_TaxGroupS);
		this.tab_Sales.Controls.Add(this.lb_Factor2S);
		this.tab_Sales.Controls.Add(this.lb_Factor1S);
		this.tab_Sales.EditControl = null;
		this.tab_Sales.EditValue = "Sales Data";
		this.tab_Sales.FieldName = "tab_Sales";
		this.tab_Sales.IsSearchControl = false;
		this.tab_Sales.Name = "tab_Sales";
		this.tab_Sales.ReadOnly = true;
		this.tab_Sales.SettingInVisible = false;
		this.tab_Sales.SettingReadonly = false;
		this.tab_Sales.Size = new System.Drawing.Size(819, 422);
		this.tab_Sales.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Sales.Text = "Sales Data";
		this.calc_SWeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_SWeight1.EditControl = null;
		this.calc_SWeight1.FieldName = "SWeight1";
		this.calc_SWeight1.IsSearchControl = false;
		this.calc_SWeight1.Location = new System.Drawing.Point(594, 100);
		this.calc_SWeight1.Name = "calc_SWeight1";
		this.calc_SWeight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_SWeight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_SWeight1.SettingInVisible = false;
		this.calc_SWeight1.SettingReadonly = false;
		this.calc_SWeight1.Size = new System.Drawing.Size(143, 20);
		this.calc_SWeight1.TabIndex = 12;
		this.calc_SHeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_SHeight1.EditControl = null;
		this.calc_SHeight1.FieldName = "SHeight1";
		this.calc_SHeight1.IsSearchControl = false;
		this.calc_SHeight1.Location = new System.Drawing.Point(594, 60);
		this.calc_SHeight1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_SHeight1.Name = "calc_SHeight1";
		this.calc_SHeight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_SHeight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_SHeight1.SettingInVisible = false;
		this.calc_SHeight1.SettingReadonly = false;
		this.calc_SHeight1.Size = new System.Drawing.Size(143, 20);
		this.calc_SHeight1.TabIndex = 9;
		this.calc_SWidth1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_SWidth1.EditControl = null;
		this.calc_SWidth1.FieldName = "SWidth1";
		this.calc_SWidth1.IsSearchControl = false;
		this.calc_SWidth1.Location = new System.Drawing.Point(594, 40);
		this.calc_SWidth1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_SWidth1.Name = "calc_SWidth1";
		this.calc_SWidth1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_SWidth1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_SWidth1.SettingInVisible = false;
		this.calc_SWidth1.SettingReadonly = false;
		this.calc_SWidth1.Size = new System.Drawing.Size(143, 20);
		this.calc_SWidth1.TabIndex = 8;
		this.calc_SLength1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_SLength1.EditControl = null;
		this.calc_SLength1.FieldName = "SLength1";
		this.calc_SLength1.IsSearchControl = false;
		this.calc_SLength1.Location = new System.Drawing.Point(594, 20);
		this.calc_SLength1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_SLength1.Name = "calc_SLength1";
		this.calc_SLength1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_SLength1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_SLength1.SettingInVisible = false;
		this.calc_SLength1.SettingReadonly = false;
		this.calc_SLength1.Size = new System.Drawing.Size(143, 20);
		this.calc_SLength1.TabIndex = 7;
		this.btn_SUoMForm.EditControl = null;
		this.btn_SUoMForm.EditValue = "...";
		this.btn_SUoMForm.FieldName = null;
		this.btn_SUoMForm.IsSearchControl = false;
		this.btn_SUoMForm.Location = new System.Drawing.Point(348, 68);
		this.btn_SUoMForm.Name = "btn_SUoMForm";
		this.btn_SUoMForm.ReadOnly = false;
		this.btn_SUoMForm.SettingInVisible = false;
		this.btn_SUoMForm.SettingReadonly = false;
		this.btn_SUoMForm.Size = new System.Drawing.Size(25, 23);
		this.btn_SUoMForm.TabIndex = 2;
		this.btn_SUoMForm.Text = "...";
		this.btn_SUoMForm.Click += new System.EventHandler(this.btn_SUoMForm_Click);
		this.txtSalUnitMsr.EditControl = null;
		this.txtSalUnitMsr.FieldName = "SalUnitMsr";
		this.txtSalUnitMsr.IsSearchControl = false;
		this.txtSalUnitMsr.Location = new System.Drawing.Point(181, 91);
		this.txtSalUnitMsr.Name = "txtSalUnitMsr";
		this.txtSalUnitMsr.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalUnitMsr.Properties.Appearance.Options.UseFont = true;
		this.txtSalUnitMsr.Properties.AutoHeight = false;
		this.txtSalUnitMsr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtSalUnitMsr.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.UomNameButtonEdit_Properties_ButtonPressed);
		this.txtSalUnitMsr.SettingInVisible = false;
		this.txtSalUnitMsr.SettingReadonly = false;
		this.txtSalUnitMsr.Size = new System.Drawing.Size(161, 18);
		this.txtSalUnitMsr.TabIndex = 3;
		this.txtSalUnitMsr.Tag = "OITM,SalUnitMsr";
		this.lbl_NumInSale.EditControl = null;
		this.lbl_NumInSale.EditValue = "1";
		this.lbl_NumInSale.FieldName = null;
		this.lbl_NumInSale.HideWhenEmpty = false;
		this.lbl_NumInSale.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_NumInSale.Location = new System.Drawing.Point(347, 112);
		this.lbl_NumInSale.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_NumInSale.Name = "lbl_NumInSale";
		this.lbl_NumInSale.ReadOnly = false;
		this.lbl_NumInSale.SettingInVisible = false;
		this.lbl_NumInSale.SettingReadonly = false;
		this.lbl_NumInSale.ShowUnderLine = true;
		this.lbl_NumInSale.Size = new System.Drawing.Size(63, 20);
		this.lbl_NumInSale.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_NumInSale.TabIndex = 93;
		this.lbl_NumInSale.TabStop = false;
		this.lbl_NumInSale.Utext = "1";
		this.bte_SUoMEntry.EditControl = null;
		this.bte_SUoMEntry.FieldName = "SUoMEntry";
		this.bte_SUoMEntry.IsSearchControl = false;
		this.bte_SUoMEntry.Location = new System.Drawing.Point(181, 71);
		this.bte_SUoMEntry.Name = "bte_SUoMEntry";
		this.bte_SUoMEntry.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.bte_SUoMEntry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_SUoMEntry.Properties.Appearance.Options.UseFont = true;
		this.bte_SUoMEntry.Properties.AutoHeight = false;
		this.bte_SUoMEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_SUoMEntry.Properties.IsSearchControl = false;
		this.bte_SUoMEntry.Properties.NullText = "";
		this.bte_SUoMEntry.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_SUoMEntry_Properties_ButtonPressed);
		this.bte_SUoMEntry.SettingInVisible = false;
		this.bte_SUoMEntry.SettingReadonly = false;
		this.bte_SUoMEntry.Size = new System.Drawing.Size(161, 18);
		this.bte_SUoMEntry.TabIndex = 1;
		this.bte_SUoMEntry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.bte_PUoMEntry_QueryPopUp);
		this.lbl_SUoMEntry.EditControl = this.bte_SUoMEntry;
		this.lbl_SUoMEntry.EditValue = "Sales UoM Code";
		this.lbl_SUoMEntry.FieldName = "lbl_SUoMEntry";
		this.lbl_SUoMEntry.HideWhenEmpty = false;
		this.lbl_SUoMEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_SUoMEntry.Location = new System.Drawing.Point(8, 72);
		this.lbl_SUoMEntry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_SUoMEntry.Name = "lbl_SUoMEntry";
		this.lbl_SUoMEntry.ReadOnly = false;
		this.lbl_SUoMEntry.SettingInVisible = false;
		this.lbl_SUoMEntry.SettingReadonly = false;
		this.lbl_SUoMEntry.ShowUnderLine = true;
		this.lbl_SUoMEntry.Size = new System.Drawing.Size(174, 20);
		this.lbl_SUoMEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_SUoMEntry.TabIndex = 89;
		this.lbl_SUoMEntry.TabStop = false;
		this.lbl_SUoMEntry.Utext = "Sales UoM Code";
		this.luk_SVolUnit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_SVolUnit.EditControl = null;
		this.luk_SVolUnit.FieldName = "SVolUnit";
		this.luk_SVolUnit.IsSearchControl = false;
		this.luk_SVolUnit.Location = new System.Drawing.Point(743, 81);
		this.luk_SVolUnit.Name = "luk_SVolUnit";
		this.luk_SVolUnit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_SVolUnit.Properties.Appearance.Options.UseFont = true;
		this.luk_SVolUnit.Properties.AutoHeight = false;
		this.luk_SVolUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SVolUnit.Properties.IsSearchControl = false;
		this.luk_SVolUnit.Properties.NullText = "";
		this.luk_SVolUnit.Properties.PopupFormMinSize = new System.Drawing.Size(35, 150);
		this.luk_SVolUnit.Properties.PopupWidth = 50;
		this.luk_SVolUnit.SettingInVisible = false;
		this.luk_SVolUnit.SettingReadonly = false;
		this.luk_SVolUnit.Size = new System.Drawing.Size(44, 18);
		this.luk_SVolUnit.TabIndex = 11;
		this.luk_SVolUnit.EditValueChanged += new System.EventHandler(this.luk_SVolUnit_EditValueChanged);
		this.lb_SLength1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SLength1.EditControl = this.calc_SLength1;
		this.lb_SLength1.EditValue = "Length";
		this.lb_SLength1.FieldName = "lb_SLength1";
		this.lb_SLength1.HideWhenEmpty = false;
		this.lb_SLength1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SLength1.Location = new System.Drawing.Point(477, 23);
		this.lb_SLength1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SLength1.Name = "lb_SLength1";
		this.lb_SLength1.ReadOnly = false;
		this.lb_SLength1.SettingInVisible = false;
		this.lb_SLength1.SettingReadonly = false;
		this.lb_SLength1.ShowUnderLine = true;
		this.lb_SLength1.Size = new System.Drawing.Size(117, 20);
		this.lb_SLength1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SLength1.TabIndex = 85;
		this.lb_SLength1.TabStop = false;
		this.lb_SLength1.Utext = "Length";
		this.calc_SVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_SVolume.EditControl = null;
		this.calc_SVolume.FieldName = "SVolume";
		this.calc_SVolume.IsSearchControl = false;
		this.calc_SVolume.Location = new System.Drawing.Point(594, 81);
		this.calc_SVolume.Name = "calc_SVolume";
		this.calc_SVolume.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_SVolume.Properties.Appearance.Options.UseFont = true;
		this.calc_SVolume.Properties.AutoHeight = false;
		this.calc_SVolume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_SVolume.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_SVolume.Properties.IsSearchControl = false;
		this.calc_SVolume.SettingInVisible = false;
		this.calc_SVolume.SettingReadonly = false;
		this.calc_SVolume.Size = new System.Drawing.Size(143, 18);
		this.calc_SVolume.TabIndex = 10;
		this.lb_SWeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SWeight1.EditControl = this.calc_SWeight1;
		this.lb_SWeight1.EditValue = "Weight";
		this.lb_SWeight1.FieldName = null;
		this.lb_SWeight1.HideWhenEmpty = false;
		this.lb_SWeight1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SWeight1.Location = new System.Drawing.Point(477, 103);
		this.lb_SWeight1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SWeight1.Name = "lb_SWeight1";
		this.lb_SWeight1.ReadOnly = false;
		this.lb_SWeight1.SettingInVisible = false;
		this.lb_SWeight1.SettingReadonly = false;
		this.lb_SWeight1.ShowUnderLine = true;
		this.lb_SWeight1.Size = new System.Drawing.Size(117, 20);
		this.lb_SWeight1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SWeight1.TabIndex = 83;
		this.lb_SWeight1.TabStop = false;
		this.lb_SWeight1.Utext = "Weight";
		this.lb_SVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SVolume.EditControl = this.calc_SVolume;
		this.lb_SVolume.EditValue = "Volume";
		this.lb_SVolume.FieldName = "lb_SVolume";
		this.lb_SVolume.HideWhenEmpty = false;
		this.lb_SVolume.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SVolume.Location = new System.Drawing.Point(477, 82);
		this.lb_SVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SVolume.Name = "lb_SVolume";
		this.lb_SVolume.ReadOnly = false;
		this.lb_SVolume.SettingInVisible = false;
		this.lb_SVolume.SettingReadonly = false;
		this.lb_SVolume.ShowUnderLine = true;
		this.lb_SVolume.Size = new System.Drawing.Size(117, 20);
		this.lb_SVolume.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SVolume.TabIndex = 82;
		this.lb_SVolume.TabStop = false;
		this.lb_SVolume.Utext = "Volume";
		this.lb_SHeight1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SHeight1.EditControl = this.calc_SHeight1;
		this.lb_SHeight1.EditValue = "Height";
		this.lb_SHeight1.FieldName = "lb_SHeight1";
		this.lb_SHeight1.HideWhenEmpty = false;
		this.lb_SHeight1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SHeight1.Location = new System.Drawing.Point(477, 63);
		this.lb_SHeight1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SHeight1.Name = "lb_SHeight1";
		this.lb_SHeight1.ReadOnly = false;
		this.lb_SHeight1.SettingInVisible = false;
		this.lb_SHeight1.SettingReadonly = false;
		this.lb_SHeight1.ShowUnderLine = true;
		this.lb_SHeight1.Size = new System.Drawing.Size(117, 20);
		this.lb_SHeight1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SHeight1.TabIndex = 81;
		this.lb_SHeight1.TabStop = false;
		this.lb_SHeight1.Utext = "Height";
		this.lb_SWidth1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SWidth1.EditControl = this.calc_SWidth1;
		this.lb_SWidth1.EditValue = "Width";
		this.lb_SWidth1.FieldName = "lb_SWidth1";
		this.lb_SWidth1.HideWhenEmpty = false;
		this.lb_SWidth1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SWidth1.Location = new System.Drawing.Point(477, 43);
		this.lb_SWidth1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SWidth1.Name = "lb_SWidth1";
		this.lb_SWidth1.ReadOnly = false;
		this.lb_SWidth1.SettingInVisible = false;
		this.lb_SWidth1.SettingReadonly = false;
		this.lb_SWidth1.ShowUnderLine = true;
		this.lb_SWidth1.Size = new System.Drawing.Size(117, 20);
		this.lb_SWidth1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SWidth1.TabIndex = 80;
		this.lb_SWidth1.TabStop = false;
		this.lb_SWidth1.Utext = "Width";
		this.calc_VatPercentSales.EditControl = null;
		this.calc_VatPercentSales.Enabled = false;
		this.calc_VatPercentSales.FieldName = null;
		this.calc_VatPercentSales.IsSearchControl = false;
		this.calc_VatPercentSales.Location = new System.Drawing.Point(348, 12);
		this.calc_VatPercentSales.Name = "calc_VatPercentSales";
		this.calc_VatPercentSales.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_VatPercentSales.Properties.Appearance.Options.UseFont = true;
		this.calc_VatPercentSales.Properties.AutoHeight = false;
		this.calc_VatPercentSales.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_VatPercentSales.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_VatPercentSales.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_VatPercentSales.Properties.IsSearchControl = false;
		this.calc_VatPercentSales.SettingInVisible = false;
		this.calc_VatPercentSales.SettingReadonly = false;
		this.calc_VatPercentSales.Size = new System.Drawing.Size(50, 18);
		this.calc_VatPercentSales.TabIndex = 60;
		this.calc_VatPercentSales.TabStop = false;
		this.calc_VatPercentSales.Tag = "OITM,Rate";
		this.cbVatGourpSa.EditControl = null;
		this.cbVatGourpSa.FieldName = "VatGourpSa";
		this.cbVatGourpSa.IsSearchControl = false;
		this.cbVatGourpSa.Location = new System.Drawing.Point(181, 12);
		this.cbVatGourpSa.Name = "cbVatGourpSa";
		this.cbVatGourpSa.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbVatGourpSa.Properties.Appearance.Options.UseFont = true;
		this.cbVatGourpSa.Properties.AutoHeight = false;
		this.cbVatGourpSa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbVatGourpSa.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Rate", "Rate")
		});
		this.cbVatGourpSa.Properties.DisplayMember = "Name";
		this.cbVatGourpSa.Properties.IsSearchControl = false;
		this.cbVatGourpSa.Properties.NullText = "";
		this.cbVatGourpSa.Properties.ValueMember = "Code";
		this.cbVatGourpSa.SettingInVisible = false;
		this.cbVatGourpSa.SettingReadonly = false;
		this.cbVatGourpSa.Size = new System.Drawing.Size(161, 18);
		this.cbVatGourpSa.TabIndex = 0;
		this.cbVatGourpSa.Tag = "OITM,VatGourpSa";
		this.cbVatGourpSa.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbVatGourpSa_EditValueChanging);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(399, 15);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(18, 13);
		this.label2.TabIndex = 57;
		this.label2.Text = "%";
		this.txtSalPackUn.EditControl = null;
		this.txtSalPackUn.FieldName = "SalPackUn";
		this.txtSalPackUn.IsSearchControl = false;
		this.txtSalPackUn.Location = new System.Drawing.Point(181, 183);
		this.txtSalPackUn.Name = "txtSalPackUn";
		this.txtSalPackUn.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalPackUn.Properties.Appearance.Options.UseFont = true;
		this.txtSalPackUn.Properties.AutoHeight = false;
		this.txtSalPackUn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSalPackUn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalPackUn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalPackUn.Properties.IsSearchControl = false;
		this.txtSalPackUn.SettingInVisible = false;
		this.txtSalPackUn.SettingReadonly = false;
		this.txtSalPackUn.Size = new System.Drawing.Size(161, 18);
		this.txtSalPackUn.TabIndex = 6;
		this.txtSalPackUn.Tag = "OITM,SalPackUn";
		this.txtNumInSale.EditControl = null;
		this.txtNumInSale.FieldName = "NumInSale";
		this.txtNumInSale.IsSearchControl = false;
		this.txtNumInSale.Location = new System.Drawing.Point(181, 111);
		this.txtNumInSale.Name = "txtNumInSale";
		this.txtNumInSale.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtNumInSale.Properties.Appearance.Options.UseFont = true;
		this.txtNumInSale.Properties.AutoHeight = false;
		this.txtNumInSale.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtNumInSale.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtNumInSale.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.txtNumInSale.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtNumInSale.Properties.IsSearchControl = false;
		this.txtNumInSale.SettingInVisible = false;
		this.txtNumInSale.SettingReadonly = false;
		this.txtNumInSale.Size = new System.Drawing.Size(161, 18);
		this.txtNumInSale.TabIndex = 4;
		this.txtNumInSale.Tag = "OITM,NumInSale";
		this.txtSalPackMsr.EditControl = null;
		this.txtSalPackMsr.FieldName = "SalPackMsr";
		this.txtSalPackMsr.IsSearchControl = false;
		this.txtSalPackMsr.Location = new System.Drawing.Point(181, 163);
		this.txtSalPackMsr.Name = "txtSalPackMsr";
		this.txtSalPackMsr.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalPackMsr.Properties.Appearance.Options.UseFont = true;
		this.txtSalPackMsr.Properties.AutoHeight = false;
		this.txtSalPackMsr.SettingInVisible = false;
		this.txtSalPackMsr.SettingReadonly = false;
		this.txtSalPackMsr.Size = new System.Drawing.Size(161, 18);
		this.txtSalPackMsr.TabIndex = 5;
		this.lb_SalesUoM.EditControl = this.txtSalUnitMsr;
		this.lb_SalesUoM.EditValue = "Sales UoM Name";
		this.lb_SalesUoM.FieldName = "lb_SalesUoM";
		this.lb_SalesUoM.HideWhenEmpty = false;
		this.lb_SalesUoM.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SalesUoM.Location = new System.Drawing.Point(8, 92);
		this.lb_SalesUoM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SalesUoM.Name = "lb_SalesUoM";
		this.lb_SalesUoM.ReadOnly = false;
		this.lb_SalesUoM.SettingInVisible = false;
		this.lb_SalesUoM.SettingReadonly = false;
		this.lb_SalesUoM.ShowUnderLine = true;
		this.lb_SalesUoM.Size = new System.Drawing.Size(174, 20);
		this.lb_SalesUoM.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SalesUoM.TabIndex = 40;
		this.lb_SalesUoM.TabStop = false;
		this.lb_SalesUoM.Utext = "Sales UoM Name";
		this.txtSalFactor4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtSalFactor4.EditControl = null;
		this.txtSalFactor4.FieldName = "SalFactor4";
		this.txtSalFactor4.IsSearchControl = false;
		this.txtSalFactor4.Location = new System.Drawing.Point(593, 291);
		this.txtSalFactor4.Name = "txtSalFactor4";
		this.txtSalFactor4.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalFactor4.Properties.Appearance.Options.UseFont = true;
		this.txtSalFactor4.Properties.AutoHeight = false;
		this.txtSalFactor4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSalFactor4.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor4.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtSalFactor4.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor4.Properties.IsSearchControl = false;
		this.txtSalFactor4.SettingInVisible = false;
		this.txtSalFactor4.SettingReadonly = false;
		this.txtSalFactor4.Size = new System.Drawing.Size(143, 18);
		this.txtSalFactor4.TabIndex = 16;
		this.txtSalFactor3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtSalFactor3.EditControl = null;
		this.txtSalFactor3.FieldName = "SalFactor3";
		this.txtSalFactor3.IsSearchControl = false;
		this.txtSalFactor3.Location = new System.Drawing.Point(593, 271);
		this.txtSalFactor3.Name = "txtSalFactor3";
		this.txtSalFactor3.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalFactor3.Properties.Appearance.Options.UseFont = true;
		this.txtSalFactor3.Properties.AutoHeight = false;
		this.txtSalFactor3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSalFactor3.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor3.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtSalFactor3.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor3.Properties.IsSearchControl = false;
		this.txtSalFactor3.SettingInVisible = false;
		this.txtSalFactor3.SettingReadonly = false;
		this.txtSalFactor3.Size = new System.Drawing.Size(143, 18);
		this.txtSalFactor3.TabIndex = 15;
		this.txtSalFactor2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtSalFactor2.EditControl = null;
		this.txtSalFactor2.FieldName = "SalFactor2";
		this.txtSalFactor2.IsSearchControl = false;
		this.txtSalFactor2.Location = new System.Drawing.Point(593, 251);
		this.txtSalFactor2.Name = "txtSalFactor2";
		this.txtSalFactor2.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalFactor2.Properties.Appearance.Options.UseFont = true;
		this.txtSalFactor2.Properties.AutoHeight = false;
		this.txtSalFactor2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSalFactor2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor2.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtSalFactor2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor2.Properties.IsSearchControl = false;
		this.txtSalFactor2.SettingInVisible = false;
		this.txtSalFactor2.SettingReadonly = false;
		this.txtSalFactor2.Size = new System.Drawing.Size(143, 18);
		this.txtSalFactor2.TabIndex = 14;
		this.lb_ItemsPerSalesUnit.EditControl = this.txtNumInSale;
		this.lb_ItemsPerSalesUnit.EditValue = "Items per Sales Unit";
		this.lb_ItemsPerSalesUnit.FieldName = "lb_ItemsPerSalesUnit";
		this.lb_ItemsPerSalesUnit.HideWhenEmpty = false;
		this.lb_ItemsPerSalesUnit.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemsPerSalesUnit.Location = new System.Drawing.Point(8, 112);
		this.lb_ItemsPerSalesUnit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ItemsPerSalesUnit.Name = "lb_ItemsPerSalesUnit";
		this.lb_ItemsPerSalesUnit.ReadOnly = false;
		this.lb_ItemsPerSalesUnit.SettingInVisible = false;
		this.lb_ItemsPerSalesUnit.SettingReadonly = false;
		this.lb_ItemsPerSalesUnit.ShowUnderLine = true;
		this.lb_ItemsPerSalesUnit.Size = new System.Drawing.Size(174, 20);
		this.lb_ItemsPerSalesUnit.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemsPerSalesUnit.TabIndex = 38;
		this.lb_ItemsPerSalesUnit.TabStop = false;
		this.lb_ItemsPerSalesUnit.Utext = "Items per Sales Unit";
		this.txtSalFactor1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtSalFactor1.EditControl = null;
		this.txtSalFactor1.FieldName = "SalFactor1";
		this.txtSalFactor1.IsSearchControl = false;
		this.txtSalFactor1.Location = new System.Drawing.Point(593, 231);
		this.txtSalFactor1.Name = "txtSalFactor1";
		this.txtSalFactor1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtSalFactor1.Properties.Appearance.Options.UseFont = true;
		this.txtSalFactor1.Properties.AutoHeight = false;
		this.txtSalFactor1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSalFactor1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor1.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtSalFactor1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtSalFactor1.Properties.IsSearchControl = false;
		this.txtSalFactor1.SettingInVisible = false;
		this.txtSalFactor1.SettingReadonly = false;
		this.txtSalFactor1.Size = new System.Drawing.Size(143, 18);
		this.txtSalFactor1.TabIndex = 13;
		this.lb_Sales.EditControl = this.txtSalPackMsr;
		this.lb_Sales.EditValue = "Packaging UoM (Sales)";
		this.lb_Sales.FieldName = "lb_Sales";
		this.lb_Sales.HideWhenEmpty = false;
		this.lb_Sales.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Sales.Location = new System.Drawing.Point(10, 164);
		this.lb_Sales.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Sales.Name = "lb_Sales";
		this.lb_Sales.ReadOnly = false;
		this.lb_Sales.SettingInVisible = false;
		this.lb_Sales.SettingReadonly = false;
		this.lb_Sales.ShowUnderLine = true;
		this.lb_Sales.Size = new System.Drawing.Size(174, 20);
		this.lb_Sales.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Sales.TabIndex = 41;
		this.lb_Sales.TabStop = false;
		this.lb_Sales.Utext = "Packaging UoM (Sales)";
		this.lb_Factor4S.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor4S.EditControl = this.txtSalFactor4;
		this.lb_Factor4S.EditValue = "Factor 4";
		this.lb_Factor4S.FieldName = "lb_Factor4S";
		this.lb_Factor4S.HideWhenEmpty = false;
		this.lb_Factor4S.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor4S.Location = new System.Drawing.Point(477, 292);
		this.lb_Factor4S.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor4S.Name = "lb_Factor4S";
		this.lb_Factor4S.ReadOnly = false;
		this.lb_Factor4S.SettingInVisible = false;
		this.lb_Factor4S.SettingReadonly = false;
		this.lb_Factor4S.ShowUnderLine = true;
		this.lb_Factor4S.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor4S.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor4S.TabIndex = 51;
		this.lb_Factor4S.TabStop = false;
		this.lb_Factor4S.Utext = "Factor 4";
		this.lb_PackagingUoMS.EditControl = this.txtSalPackUn;
		this.lb_PackagingUoMS.EditValue = "Quantity per Packaging UoM";
		this.lb_PackagingUoMS.FieldName = "lb_PackagingUoMS";
		this.lb_PackagingUoMS.HideWhenEmpty = false;
		this.lb_PackagingUoMS.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PackagingUoMS.Location = new System.Drawing.Point(10, 183);
		this.lb_PackagingUoMS.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PackagingUoMS.Name = "lb_PackagingUoMS";
		this.lb_PackagingUoMS.ReadOnly = false;
		this.lb_PackagingUoMS.SettingInVisible = false;
		this.lb_PackagingUoMS.SettingReadonly = false;
		this.lb_PackagingUoMS.ShowUnderLine = true;
		this.lb_PackagingUoMS.Size = new System.Drawing.Size(174, 20);
		this.lb_PackagingUoMS.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PackagingUoMS.TabIndex = 39;
		this.lb_PackagingUoMS.TabStop = false;
		this.lb_PackagingUoMS.Utext = "Quantity per Packaging UoM";
		this.lb_Factor3S.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor3S.EditControl = this.txtSalFactor3;
		this.lb_Factor3S.EditValue = "Factor 3";
		this.lb_Factor3S.FieldName = "lb_Factor3S";
		this.lb_Factor3S.HideWhenEmpty = false;
		this.lb_Factor3S.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor3S.Location = new System.Drawing.Point(477, 272);
		this.lb_Factor3S.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor3S.Name = "lb_Factor3S";
		this.lb_Factor3S.ReadOnly = false;
		this.lb_Factor3S.SettingInVisible = false;
		this.lb_Factor3S.SettingReadonly = false;
		this.lb_Factor3S.ShowUnderLine = true;
		this.lb_Factor3S.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor3S.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor3S.TabIndex = 50;
		this.lb_Factor3S.TabStop = false;
		this.lb_Factor3S.Utext = "Factor 3";
		this.lb_TaxGroupS.EditControl = this.cbVatGourpSa;
		this.lb_TaxGroupS.EditValue = "Tax Group";
		this.lb_TaxGroupS.FieldName = "lb_TaxGroupS";
		this.lb_TaxGroupS.HideWhenEmpty = false;
		this.lb_TaxGroupS.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TaxGroupS.Location = new System.Drawing.Point(8, 13);
		this.lb_TaxGroupS.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TaxGroupS.Name = "lb_TaxGroupS";
		this.lb_TaxGroupS.ReadOnly = false;
		this.lb_TaxGroupS.SettingInVisible = false;
		this.lb_TaxGroupS.SettingReadonly = false;
		this.lb_TaxGroupS.ShowUnderLine = true;
		this.lb_TaxGroupS.Size = new System.Drawing.Size(174, 20);
		this.lb_TaxGroupS.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TaxGroupS.TabIndex = 42;
		this.lb_TaxGroupS.TabStop = false;
		this.lb_TaxGroupS.Utext = "Tax Group";
		this.lb_Factor2S.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor2S.EditControl = this.txtSalFactor2;
		this.lb_Factor2S.EditValue = "Factor 2";
		this.lb_Factor2S.FieldName = "lb_Factor2S";
		this.lb_Factor2S.HideWhenEmpty = false;
		this.lb_Factor2S.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor2S.Location = new System.Drawing.Point(477, 252);
		this.lb_Factor2S.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor2S.Name = "lb_Factor2S";
		this.lb_Factor2S.ReadOnly = false;
		this.lb_Factor2S.SettingInVisible = false;
		this.lb_Factor2S.SettingReadonly = false;
		this.lb_Factor2S.ShowUnderLine = true;
		this.lb_Factor2S.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor2S.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor2S.TabIndex = 49;
		this.lb_Factor2S.TabStop = false;
		this.lb_Factor2S.Utext = "Factor 2";
		this.lb_Factor1S.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Factor1S.EditControl = this.txtSalFactor1;
		this.lb_Factor1S.EditValue = "Factor 1";
		this.lb_Factor1S.FieldName = "lb_Factor1S";
		this.lb_Factor1S.HideWhenEmpty = false;
		this.lb_Factor1S.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Factor1S.Location = new System.Drawing.Point(477, 232);
		this.lb_Factor1S.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Factor1S.Name = "lb_Factor1S";
		this.lb_Factor1S.ReadOnly = false;
		this.lb_Factor1S.SettingInVisible = false;
		this.lb_Factor1S.SettingReadonly = false;
		this.lb_Factor1S.ShowUnderLine = true;
		this.lb_Factor1S.Size = new System.Drawing.Size(117, 20);
		this.lb_Factor1S.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Factor1S.TabIndex = 48;
		this.lb_Factor1S.TabStop = false;
		this.lb_Factor1S.Utext = "Factor 1";
		this.tab_Inventory.Controls.Add(this.calc_IWeight1);
		this.tab_Inventory.Controls.Add(this.txtInvntUom);
		this.tab_Inventory.Controls.Add(this.bte_INUoMEntry);
		this.tab_Inventory.Controls.Add(this.bte_IUoMEntry);
		this.tab_Inventory.Controls.Add(this.calc_NumInCnt);
		this.tab_Inventory.Controls.Add(this.txt_CntUnitMsr);
		this.tab_Inventory.Controls.Add(this.cbEvalSystem);
		this.tab_Inventory.Controls.Add(this.txtAvgPrice);
		this.tab_Inventory.Controls.Add(this.lbl_NumInCnt);
		this.tab_Inventory.Controls.Add(this.lbl_NumInCnt1);
		this.tab_Inventory.Controls.Add(this.lbl_INUoMEntry);
		this.tab_Inventory.Controls.Add(this.lbl_CntUnitMsr);
		this.tab_Inventory.Controls.Add(this.Weight);
		this.tab_Inventory.Controls.Add(this.lbl_IUoMEntry);
		this.tab_Inventory.Controls.Add(this.txtMaxLevel);
		this.tab_Inventory.Controls.Add(this.txtMinLevel);
		this.tab_Inventory.Controls.Add(this.txtReorderQty);
		this.tab_Inventory.Controls.Add(this.cbGLMethod);
		this.tab_Inventory.Controls.Add(this.lb_InventoryLevel);
		this.tab_Inventory.Controls.Add(this.btn_SetDfltWH);
		this.tab_Inventory.Controls.Add(this.chkByWh);
		this.tab_Inventory.Controls.Add(this.grd_WareHouse);
		this.tab_Inventory.Controls.Add(this.lb_AccBy);
		this.tab_Inventory.Controls.Add(this.lb_InventoryUom);
		this.tab_Inventory.Controls.Add(this.lb_ValuationMethod);
		this.tab_Inventory.Controls.Add(this.lbItemCost);
		this.tab_Inventory.Controls.Add(this.lb_RequiredPrUOM);
		this.tab_Inventory.Controls.Add(this.lb_Maximum);
		this.tab_Inventory.Controls.Add(this.lb_Minimum);
		this.tab_Inventory.EditControl = null;
		this.tab_Inventory.EditValue = "Inventory Data";
		this.tab_Inventory.FieldName = "tab_Inventory";
		this.tab_Inventory.IsSearchControl = false;
		this.tab_Inventory.Name = "tab_Inventory";
		this.tab_Inventory.ReadOnly = true;
		this.tab_Inventory.SettingInVisible = false;
		this.tab_Inventory.SettingReadonly = false;
		this.tab_Inventory.Size = new System.Drawing.Size(819, 422);
		this.tab_Inventory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Inventory.Text = "Inventory Data";
		this.calc_IWeight1.EditControl = null;
		this.calc_IWeight1.FieldName = "IWeight1";
		this.calc_IWeight1.IsSearchControl = false;
		this.calc_IWeight1.Location = new System.Drawing.Point(159, 62);
		this.calc_IWeight1.Name = "calc_IWeight1";
		this.calc_IWeight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_IWeight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_IWeight1.SettingInVisible = false;
		this.calc_IWeight1.SettingReadonly = false;
		this.calc_IWeight1.Size = new System.Drawing.Size(143, 20);
		this.calc_IWeight1.TabIndex = 3;
		this.txtInvntUom.EditControl = null;
		this.txtInvntUom.FieldName = "InvntryUom";
		this.txtInvntUom.IsSearchControl = false;
		this.txtInvntUom.Location = new System.Drawing.Point(159, 43);
		this.txtInvntUom.Name = "txtInvntUom";
		this.txtInvntUom.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtInvntUom.Properties.Appearance.Options.UseFont = true;
		this.txtInvntUom.Properties.AutoHeight = false;
		this.txtInvntUom.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.UomNameButtonEdit_Properties_ButtonPressed);
		this.txtInvntUom.SettingInVisible = false;
		this.txtInvntUom.SettingReadonly = false;
		this.txtInvntUom.Size = new System.Drawing.Size(143, 18);
		this.txtInvntUom.TabIndex = 2;
		this.txtInvntUom.Tag = "OITM,InvntryUom";
		this.bte_INUoMEntry.EditControl = null;
		this.bte_INUoMEntry.FieldName = "INUoMEntry";
		this.bte_INUoMEntry.IsSearchControl = false;
		this.bte_INUoMEntry.Location = new System.Drawing.Point(175, 88);
		this.bte_INUoMEntry.Name = "bte_INUoMEntry";
		this.bte_INUoMEntry.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.bte_INUoMEntry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_INUoMEntry.Properties.Appearance.Options.UseFont = true;
		this.bte_INUoMEntry.Properties.AutoHeight = false;
		this.bte_INUoMEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_INUoMEntry.Properties.IsSearchControl = false;
		this.bte_INUoMEntry.Properties.NullText = "";
		this.bte_INUoMEntry.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_INUoMEntry_Properties_ButtonPressed);
		this.bte_INUoMEntry.SettingInVisible = false;
		this.bte_INUoMEntry.SettingReadonly = false;
		this.bte_INUoMEntry.Size = new System.Drawing.Size(126, 18);
		this.bte_INUoMEntry.TabIndex = 4;
		this.bte_INUoMEntry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.bte_PUoMEntry_QueryPopUp);
		this.bte_IUoMEntry.EditControl = null;
		this.bte_IUoMEntry.FieldName = "IUoMEntry";
		this.bte_IUoMEntry.IsSearchControl = false;
		this.bte_IUoMEntry.Location = new System.Drawing.Point(159, 24);
		this.bte_IUoMEntry.Name = "bte_IUoMEntry";
		this.bte_IUoMEntry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.bte_IUoMEntry.Properties.Appearance.Options.UseFont = true;
		this.bte_IUoMEntry.Properties.AutoHeight = false;
		this.bte_IUoMEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_IUoMEntry.Properties.IsSearchControl = false;
		this.bte_IUoMEntry.Properties.NullText = "";
		this.bte_IUoMEntry.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_IUoMEntry_Properties_ButtonPressed);
		this.bte_IUoMEntry.SettingInVisible = false;
		this.bte_IUoMEntry.SettingReadonly = false;
		this.bte_IUoMEntry.Size = new System.Drawing.Size(143, 18);
		this.bte_IUoMEntry.TabIndex = 1;
		this.bte_IUoMEntry.Tag = "";
		this.bte_IUoMEntry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.bte_PUoMEntry_QueryPopUp);
		this.calc_NumInCnt.EditControl = null;
		this.calc_NumInCnt.Enabled = false;
		this.calc_NumInCnt.FieldName = "NumInCnt";
		this.calc_NumInCnt.IsSearchControl = false;
		this.calc_NumInCnt.Location = new System.Drawing.Point(175, 126);
		this.calc_NumInCnt.Name = "calc_NumInCnt";
		this.calc_NumInCnt.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_NumInCnt.Properties.Appearance.Options.UseFont = true;
		this.calc_NumInCnt.Properties.AutoHeight = false;
		this.calc_NumInCnt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NumInCnt.Properties.IsSearchControl = false;
		this.calc_NumInCnt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.calc_NumInCnt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_NumInCnt.SettingInVisible = false;
		this.calc_NumInCnt.SettingReadonly = false;
		this.calc_NumInCnt.Size = new System.Drawing.Size(126, 18);
		this.calc_NumInCnt.TabIndex = 6;
		this.calc_NumInCnt.Tag = "OITM,NumInCnt";
		this.txt_CntUnitMsr.EditControl = null;
		this.txt_CntUnitMsr.Enabled = false;
		this.txt_CntUnitMsr.FieldName = "CntUnitMsr";
		this.txt_CntUnitMsr.IsSearchControl = false;
		this.txt_CntUnitMsr.Location = new System.Drawing.Point(175, 107);
		this.txt_CntUnitMsr.Name = "txt_CntUnitMsr";
		this.txt_CntUnitMsr.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_CntUnitMsr.Properties.Appearance.Options.UseFont = true;
		this.txt_CntUnitMsr.Properties.AutoHeight = false;
		this.txt_CntUnitMsr.SettingInVisible = false;
		this.txt_CntUnitMsr.SettingReadonly = false;
		this.txt_CntUnitMsr.Size = new System.Drawing.Size(126, 18);
		this.txt_CntUnitMsr.TabIndex = 5;
		this.txt_CntUnitMsr.Tag = "OITM,CntUnitMsr";
		this.cbEvalSystem.EditControl = null;
		this.cbEvalSystem.FieldName = "EvalSystem";
		this.cbEvalSystem.IsSearchControl = false;
		this.cbEvalSystem.Location = new System.Drawing.Point(175, 151);
		this.cbEvalSystem.Name = "cbEvalSystem";
		this.cbEvalSystem.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbEvalSystem.Properties.Appearance.Options.UseFont = true;
		this.cbEvalSystem.Properties.AutoHeight = false;
		this.cbEvalSystem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbEvalSystem.Properties.IsSearchControl = false;
		this.cbEvalSystem.Properties.NullText = "";
		this.cbEvalSystem.SettingInVisible = false;
		this.cbEvalSystem.SettingReadonly = false;
		this.cbEvalSystem.Size = new System.Drawing.Size(126, 18);
		this.cbEvalSystem.TabIndex = 7;
		this.cbEvalSystem.Tag = "OITM,EvalSystem";
		this.txtAvgPrice.EditControl = null;
		this.txtAvgPrice.FieldName = "AvgPrice";
		this.txtAvgPrice.IsSearchControl = false;
		this.txtAvgPrice.Location = new System.Drawing.Point(175, 170);
		this.txtAvgPrice.Name = "txtAvgPrice";
		this.txtAvgPrice.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtAvgPrice.Properties.Appearance.Options.UseFont = true;
		this.txtAvgPrice.Properties.AutoHeight = false;
		this.txtAvgPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtAvgPrice.Properties.IsSearchControl = false;
		this.txtAvgPrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtAvgPrice.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txtAvgPrice.SettingInVisible = false;
		this.txtAvgPrice.SettingReadonly = false;
		this.txtAvgPrice.Size = new System.Drawing.Size(126, 18);
		this.txtAvgPrice.TabIndex = 8;
		this.txtAvgPrice.Tag = "OITM,AvgPrice";
		this.lbl_NumInCnt.EditControl = null;
		this.lbl_NumInCnt.EditValue = "1";
		this.lbl_NumInCnt.FieldName = null;
		this.lbl_NumInCnt.HideWhenEmpty = false;
		this.lbl_NumInCnt.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_NumInCnt.Location = new System.Drawing.Point(307, 129);
		this.lbl_NumInCnt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_NumInCnt.Name = "lbl_NumInCnt";
		this.lbl_NumInCnt.ReadOnly = false;
		this.lbl_NumInCnt.SettingInVisible = false;
		this.lbl_NumInCnt.SettingReadonly = false;
		this.lbl_NumInCnt.ShowUnderLine = true;
		this.lbl_NumInCnt.Size = new System.Drawing.Size(63, 20);
		this.lbl_NumInCnt.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_NumInCnt.TabIndex = 92;
		this.lbl_NumInCnt.TabStop = false;
		this.lbl_NumInCnt.Utext = "1";
		this.lbl_NumInCnt1.EditControl = this.calc_NumInCnt;
		this.lbl_NumInCnt1.EditValue = "Items per Counting Unit";
		this.lbl_NumInCnt1.FieldName = "lbl_NumInCnt1";
		this.lbl_NumInCnt1.HideWhenEmpty = false;
		this.lbl_NumInCnt1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_NumInCnt1.Location = new System.Drawing.Point(7, 127);
		this.lbl_NumInCnt1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_NumInCnt1.Name = "lbl_NumInCnt1";
		this.lbl_NumInCnt1.ReadOnly = false;
		this.lbl_NumInCnt1.SettingInVisible = false;
		this.lbl_NumInCnt1.SettingReadonly = false;
		this.lbl_NumInCnt1.ShowUnderLine = true;
		this.lbl_NumInCnt1.Size = new System.Drawing.Size(169, 20);
		this.lbl_NumInCnt1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_NumInCnt1.TabIndex = 91;
		this.lbl_NumInCnt1.TabStop = false;
		this.lbl_NumInCnt1.Utext = "Items per Counting Unit";
		this.lbl_INUoMEntry.EditControl = this.bte_INUoMEntry;
		this.lbl_INUoMEntry.EditValue = "Inventory Counting UoM Code";
		this.lbl_INUoMEntry.FieldName = "lbl_INUoMEntry";
		this.lbl_INUoMEntry.HideWhenEmpty = false;
		this.lbl_INUoMEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_INUoMEntry.Location = new System.Drawing.Point(7, 90);
		this.lbl_INUoMEntry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_INUoMEntry.Name = "lbl_INUoMEntry";
		this.lbl_INUoMEntry.ReadOnly = false;
		this.lbl_INUoMEntry.SettingInVisible = false;
		this.lbl_INUoMEntry.SettingReadonly = false;
		this.lbl_INUoMEntry.ShowUnderLine = true;
		this.lbl_INUoMEntry.Size = new System.Drawing.Size(169, 20);
		this.lbl_INUoMEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_INUoMEntry.TabIndex = 89;
		this.lbl_INUoMEntry.TabStop = false;
		this.lbl_INUoMEntry.Utext = "Inventory Counting UoM Code";
		this.lbl_CntUnitMsr.EditControl = this.txt_CntUnitMsr;
		this.lbl_CntUnitMsr.EditValue = "Inventory Counting UoM Name";
		this.lbl_CntUnitMsr.FieldName = "lbl_CntUnitMsr";
		this.lbl_CntUnitMsr.HideWhenEmpty = false;
		this.lbl_CntUnitMsr.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CntUnitMsr.Location = new System.Drawing.Point(7, 108);
		this.lbl_CntUnitMsr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_CntUnitMsr.Name = "lbl_CntUnitMsr";
		this.lbl_CntUnitMsr.ReadOnly = false;
		this.lbl_CntUnitMsr.SettingInVisible = false;
		this.lbl_CntUnitMsr.SettingReadonly = false;
		this.lbl_CntUnitMsr.ShowUnderLine = true;
		this.lbl_CntUnitMsr.Size = new System.Drawing.Size(169, 20);
		this.lbl_CntUnitMsr.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CntUnitMsr.TabIndex = 87;
		this.lbl_CntUnitMsr.TabStop = false;
		this.lbl_CntUnitMsr.Utext = "Inventory Counting UoM Name";
		this.Weight.EditControl = this.calc_IWeight1;
		this.Weight.EditValue = "Weight";
		this.Weight.FieldName = "lb_Weight";
		this.Weight.HideWhenEmpty = false;
		this.Weight.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.Weight.Location = new System.Drawing.Point(7, 65);
		this.Weight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.Weight.Name = "Weight";
		this.Weight.ReadOnly = false;
		this.Weight.SettingInVisible = false;
		this.Weight.SettingReadonly = false;
		this.Weight.ShowUnderLine = true;
		this.Weight.Size = new System.Drawing.Size(152, 20);
		this.Weight.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Weight.TabIndex = 85;
		this.Weight.TabStop = false;
		this.Weight.Utext = "Weight";
		this.lbl_IUoMEntry.EditControl = this.bte_IUoMEntry;
		this.lbl_IUoMEntry.EditValue = "UoM Code";
		this.lbl_IUoMEntry.FieldName = "lbl_IUoMEntry";
		this.lbl_IUoMEntry.HideWhenEmpty = false;
		this.lbl_IUoMEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_IUoMEntry.Location = new System.Drawing.Point(7, 27);
		this.lbl_IUoMEntry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_IUoMEntry.Name = "lbl_IUoMEntry";
		this.lbl_IUoMEntry.ReadOnly = false;
		this.lbl_IUoMEntry.SettingInVisible = false;
		this.lbl_IUoMEntry.SettingReadonly = false;
		this.lbl_IUoMEntry.ShowUnderLine = true;
		this.lbl_IUoMEntry.Size = new System.Drawing.Size(152, 20);
		this.lbl_IUoMEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_IUoMEntry.TabIndex = 83;
		this.lbl_IUoMEntry.TabStop = false;
		this.lbl_IUoMEntry.Utext = "UoM Code";
		this.txtMaxLevel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtMaxLevel.EditControl = null;
		this.txtMaxLevel.FieldName = "MaxLevel";
		this.txtMaxLevel.IsSearchControl = false;
		this.txtMaxLevel.Location = new System.Drawing.Point(619, 99);
		this.txtMaxLevel.Name = "txtMaxLevel";
		this.txtMaxLevel.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtMaxLevel.Properties.Appearance.Options.UseFont = true;
		this.txtMaxLevel.Properties.AutoHeight = false;
		this.txtMaxLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtMaxLevel.Properties.DisplayFormat.FormatString = "n3";
		this.txtMaxLevel.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtMaxLevel.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtMaxLevel.Properties.EditFormat.FormatString = "n3";
		this.txtMaxLevel.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtMaxLevel.Properties.IsSearchControl = false;
		this.txtMaxLevel.SettingInVisible = false;
		this.txtMaxLevel.SettingReadonly = false;
		this.txtMaxLevel.Size = new System.Drawing.Size(143, 18);
		this.txtMaxLevel.TabIndex = 12;
		this.txtMaxLevel.Tag = "OITM,MaxLevel";
		this.txtMinLevel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtMinLevel.EditControl = null;
		this.txtMinLevel.FieldName = "MinLevel";
		this.txtMinLevel.IsSearchControl = false;
		this.txtMinLevel.Location = new System.Drawing.Point(619, 78);
		this.txtMinLevel.Name = "txtMinLevel";
		this.txtMinLevel.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtMinLevel.Properties.Appearance.Options.UseFont = true;
		this.txtMinLevel.Properties.AutoHeight = false;
		this.txtMinLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtMinLevel.Properties.DisplayFormat.FormatString = "n3";
		this.txtMinLevel.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtMinLevel.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtMinLevel.Properties.EditFormat.FormatString = "n3";
		this.txtMinLevel.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtMinLevel.Properties.IsSearchControl = false;
		this.txtMinLevel.SettingInVisible = false;
		this.txtMinLevel.SettingReadonly = false;
		this.txtMinLevel.Size = new System.Drawing.Size(143, 18);
		this.txtMinLevel.TabIndex = 11;
		this.txtMinLevel.Tag = "OITM,MinLevel";
		this.txtReorderQty.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtReorderQty.EditControl = null;
		this.txtReorderQty.FieldName = "ReorderQty";
		this.txtReorderQty.IsSearchControl = false;
		this.txtReorderQty.Location = new System.Drawing.Point(619, 57);
		this.txtReorderQty.Name = "txtReorderQty";
		this.txtReorderQty.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtReorderQty.Properties.Appearance.Options.UseFont = true;
		this.txtReorderQty.Properties.AutoHeight = false;
		this.txtReorderQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtReorderQty.Properties.DisplayFormat.FormatString = "n3";
		this.txtReorderQty.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtReorderQty.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txtReorderQty.Properties.EditFormat.FormatString = "n3";
		this.txtReorderQty.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtReorderQty.Properties.IsSearchControl = false;
		this.txtReorderQty.SettingInVisible = false;
		this.txtReorderQty.SettingReadonly = false;
		this.txtReorderQty.Size = new System.Drawing.Size(143, 18);
		this.txtReorderQty.TabIndex = 10;
		this.txtReorderQty.Tag = "OITM,ReorderQty";
		this.txtReorderQty.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.txtReorderQty_ParseEditValue);
		this.cbGLMethod.EditControl = null;
		this.cbGLMethod.FieldName = "GLMethod";
		this.cbGLMethod.IsSearchControl = false;
		this.cbGLMethod.Location = new System.Drawing.Point(159, 5);
		this.cbGLMethod.Name = "cbGLMethod";
		this.cbGLMethod.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbGLMethod.Properties.Appearance.Options.UseFont = true;
		this.cbGLMethod.Properties.AutoHeight = false;
		this.cbGLMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbGLMethod.Properties.IsSearchControl = false;
		this.cbGLMethod.Properties.NullText = "";
		this.cbGLMethod.SettingInVisible = false;
		this.cbGLMethod.SettingReadonly = false;
		this.cbGLMethod.Size = new System.Drawing.Size(143, 18);
		this.cbGLMethod.TabIndex = 0;
		this.cbGLMethod.Tag = "OITM,GLMethod";
		this.lb_InventoryLevel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_InventoryLevel.EditControl = null;
		this.lb_InventoryLevel.EditValue = "Inventory Level";
		this.lb_InventoryLevel.FieldName = "lb_InventoryLevel";
		this.lb_InventoryLevel.HideWhenEmpty = false;
		this.lb_InventoryLevel.LineColor = System.Drawing.Color.Black;
		this.lb_InventoryLevel.Location = new System.Drawing.Point(450, 30);
		this.lb_InventoryLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_InventoryLevel.Name = "lb_InventoryLevel";
		this.lb_InventoryLevel.ReadOnly = false;
		this.lb_InventoryLevel.SettingInVisible = false;
		this.lb_InventoryLevel.SettingReadonly = false;
		this.lb_InventoryLevel.ShowUnderLine = true;
		this.lb_InventoryLevel.Size = new System.Drawing.Size(97, 20);
		this.lb_InventoryLevel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_InventoryLevel.TabIndex = 81;
		this.lb_InventoryLevel.TabStop = false;
		this.lb_InventoryLevel.Utext = "Inventory Level";
		this.btn_SetDfltWH.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SetDfltWH.EditControl = null;
		this.btn_SetDfltWH.EditValue = "Set Default Warehouse";
		this.btn_SetDfltWH.FieldName = null;
		this.btn_SetDfltWH.IsSearchControl = false;
		this.btn_SetDfltWH.Location = new System.Drawing.Point(642, 394);
		this.btn_SetDfltWH.Name = "btn_SetDfltWH";
		this.btn_SetDfltWH.ReadOnly = false;
		this.btn_SetDfltWH.SettingInVisible = false;
		this.btn_SetDfltWH.SettingReadonly = false;
		this.btn_SetDfltWH.Size = new System.Drawing.Size(125, 23);
		this.btn_SetDfltWH.TabIndex = 14;
		this.btn_SetDfltWH.Text = "Set Default Warehouse";
		this.btn_SetDfltWH.Click += new System.EventHandler(this.btn_SetDfltWH_Click);
		this.chkByWh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chkByWh.EditControl = this.chkByWh;
		this.chkByWh.FieldName = "ByWh";
		this.chkByWh.IsSearchControl = false;
		this.chkByWh.Location = new System.Drawing.Point(450, 4);
		this.chkByWh.Name = "chkByWh";
		this.chkByWh.Properties.Caption = "Manage Inventory by Warehouse";
		this.chkByWh.Properties.DisplayValueChecked = "Y";
		this.chkByWh.Properties.DisplayValueGrayed = "N";
		this.chkByWh.Properties.DisplayValueUnchecked = "N";
		this.chkByWh.Properties.ValueChecked = "Y";
		this.chkByWh.Properties.ValueGrayed = "N";
		this.chkByWh.Properties.ValueUnchecked = "N";
		this.chkByWh.SettingInVisible = false;
		this.chkByWh.SettingReadonly = false;
		this.chkByWh.Size = new System.Drawing.Size(184, 20);
		this.chkByWh.TabIndex = 9;
		this.chkByWh.Tag = "OITM,ByWH";
		this.grd_WareHouse.AllowPasteExcel = false;
		this.grd_WareHouse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_WareHouse.ClipboardData = null;
		this.grd_WareHouse.ContextMenuStrip = this.ctmenu_Warehouse;
		this.grd_WareHouse.Location = new System.Drawing.Point(2, 194);
		this.grd_WareHouse.MainView = this.gv_WareHouse;
		this.grd_WareHouse.Name = "grd_WareHouse";
		this.grd_WareHouse.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rChk_Lock, this.rep_CalcInventory_Price, this.rep_CalcInventory_Quantity, this.rep_bte_WhsCode, this.rep_DefaultBinLocation });
		this.grd_WareHouse.Size = new System.Drawing.Size(812, 194);
		this.grd_WareHouse.TabIndex = 13;
		this.grd_WareHouse.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_WareHouse });
		this.ctmenu_Warehouse.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctmenu_Warehouse.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.ctmenu_Warehouse.Name = "ctmenu_Warehouse";
		this.ctmenu_Warehouse.Size = new System.Drawing.Size(141, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.addRowToolStripMenuItem.Text = "Add row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.removeRowToolStripMenuItem.Text = "Remove row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_WareHouse.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_WareHouse.ColumnKey = null;
		this.gv_WareHouse.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gcWhseCode, this.gcWhseName, this.gcLocked, this.gcPriceInventory, this.gcOnHand, this.gcCommited, this.gcOnOrder, this.gcAvaiable, this.gcMinInventory, this.gcMaxInventory,
			this.gcReqInventory, this.gcUserSign, this.gcCreateDate, this.gcUserSign2, this.gcUpdateDate, this.gridColumnEx1
		});
		this.gv_WareHouse.DisableFormSetting = false;
		this.gv_WareHouse.DisplayException = false;
		this.gv_WareHouse.EditColumn = null;
		this.gv_WareHouse.FieldCurrency = null;
		this.gv_WareHouse.FieldRate = null;
		this.gv_WareHouse.FormId = "oItem";
		this.gv_WareHouse.GridControl = this.grd_WareHouse;
		this.gv_WareHouse.HasFormSetting = false;
		this.gv_WareHouse.IgnoreFilter = false;
		this.gv_WareHouse.IndexEditRow = -1;
		this.gv_WareHouse.IndicatorWidth = 35;
		this.gv_WareHouse.IsApzTable = false;
		this.gv_WareHouse.IsInsertNewRow = false;
		this.gv_WareHouse.IsLoadingData = false;
		this.gv_WareHouse.IsMappingContextAction = false;
		this.gv_WareHouse.ItemId = "OWHS";
		this.gv_WareHouse.LstColumnAllowBatch = null;
		this.gv_WareHouse.LstColumnNotAllowBatch = null;
		this.gv_WareHouse.LstColumnNotEditable = null;
		this.gv_WareHouse.LstColumNotSetting = null;
		this.gv_WareHouse.LstOldUDF = null;
		this.gv_WareHouse.Name = "gv_WareHouse";
		this.gv_WareHouse.OptionsFind.AllowFindPanel = false;
		this.gv_WareHouse.OptionsView.ShowFooter = true;
		this.gv_WareHouse.OptionsView.ShowGroupPanel = false;
		this.gv_WareHouse.UseBatchMod = false;
		this.gv_WareHouse.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvWareHouse_CustomDrawRowIndicator);
		this.gv_WareHouse.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvWareHouse_RowStyle);
		this.gv_WareHouse.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvWareHouse_CellValueChanged);
		this.gv_WareHouse.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_WareHouse_CustomColumnDisplayText);
		this.gcWhseCode.Caption = "Whse Code";
		this.gcWhseCode.ColumnEdit = this.rep_bte_WhsCode;
		this.gcWhseCode.CurrencyDateField = null;
		this.gcWhseCode.CurrencyField = null;
		this.gcWhseCode.CurrencyFixed = null;
		this.gcWhseCode.CurrencyFixedRate = 0.0;
		this.gcWhseCode.CurrencyRateField = null;
		this.gcWhseCode.FieldName = "WhsCode";
		this.gcWhseCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcWhseCode.Name = "gcWhseCode";
		this.gcWhseCode.OptionsColumn.ReadOnly = true;
		this.gcWhseCode.PrimaryKey = false;
		this.gcWhseCode.UnitField = null;
		this.gcWhseCode.Visible = true;
		this.gcWhseCode.VisibleIndex = 0;
		this.rep_bte_WhsCode.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_bte_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_WhsCode.Name = "rep_bte_WhsCode";
		this.rep_bte_WhsCode.ReadOnly = true;
		this.rep_bte_WhsCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_WhsCode_ButtonPressed);
		this.gcWhseName.Caption = "Whse Name";
		this.gcWhseName.CurrencyDateField = null;
		this.gcWhseName.CurrencyField = null;
		this.gcWhseName.CurrencyFixed = null;
		this.gcWhseName.CurrencyFixedRate = 0.0;
		this.gcWhseName.CurrencyRateField = null;
		this.gcWhseName.FieldName = "WhsName";
		this.gcWhseName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcWhseName.Name = "gcWhseName";
		this.gcWhseName.OptionsColumn.AllowEdit = false;
		this.gcWhseName.OptionsColumn.ReadOnly = true;
		this.gcWhseName.PrimaryKey = false;
		this.gcWhseName.UnitField = null;
		this.gcWhseName.Visible = true;
		this.gcWhseName.VisibleIndex = 1;
		this.gcLocked.Caption = "Locked";
		this.gcLocked.ColumnEdit = this.rChk_Lock;
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcLocked.Visible = true;
		this.gcLocked.VisibleIndex = 2;
		this.rChk_Lock.AutoHeight = false;
		this.rChk_Lock.Name = "rChk_Lock";
		this.rChk_Lock.ValueChecked = "Y";
		this.rChk_Lock.ValueGrayed = "N";
		this.rChk_Lock.ValueUnchecked = "N";
		this.gcPriceInventory.Caption = "Price.Inventory";
		this.gcPriceInventory.ColumnEdit = this.rep_CalcInventory_Price;
		this.gcPriceInventory.CurrencyDateField = null;
		this.gcPriceInventory.CurrencyField = null;
		this.gcPriceInventory.CurrencyFixed = null;
		this.gcPriceInventory.CurrencyFixedRate = 0.0;
		this.gcPriceInventory.CurrencyRateField = null;
		this.gcPriceInventory.FieldName = "AvgPrice";
		this.gcPriceInventory.Name = "gcPriceInventory";
		this.gcPriceInventory.OptionsColumn.AllowEdit = false;
		this.gcPriceInventory.OptionsColumn.ReadOnly = true;
		this.gcPriceInventory.PrimaryKey = false;
		this.gcPriceInventory.UnitField = null;
		this.gcPriceInventory.Visible = true;
		this.gcPriceInventory.VisibleIndex = 3;
		this.rep_CalcInventory_Price.AutoHeight = false;
		this.rep_CalcInventory_Price.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CalcInventory_Price.Name = "rep_CalcInventory_Price";
		this.gcOnHand.Caption = "In Stock";
		this.gcOnHand.ColumnEdit = this.rep_CalcInventory_Quantity;
		this.gcOnHand.CurrencyDateField = null;
		this.gcOnHand.CurrencyField = null;
		this.gcOnHand.CurrencyFixed = null;
		this.gcOnHand.CurrencyFixedRate = 0.0;
		this.gcOnHand.CurrencyRateField = null;
		this.gcOnHand.FieldName = "OnHand";
		this.gcOnHand.Name = "gcOnHand";
		this.gcOnHand.OptionsColumn.AllowEdit = false;
		this.gcOnHand.OptionsColumn.ReadOnly = true;
		this.gcOnHand.PrimaryKey = false;
		this.gcOnHand.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcOnHand.UnitField = null;
		this.gcOnHand.Visible = true;
		this.gcOnHand.VisibleIndex = 4;
		this.rep_CalcInventory_Quantity.AutoHeight = false;
		this.rep_CalcInventory_Quantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CalcInventory_Quantity.Name = "rep_CalcInventory_Quantity";
		this.gcCommited.Caption = "Commited";
		this.gcCommited.ColumnEdit = this.rep_CalcInventory_Quantity;
		this.gcCommited.CurrencyDateField = null;
		this.gcCommited.CurrencyField = null;
		this.gcCommited.CurrencyFixed = null;
		this.gcCommited.CurrencyFixedRate = 0.0;
		this.gcCommited.CurrencyRateField = null;
		this.gcCommited.FieldName = "IsCommited";
		this.gcCommited.Name = "gcCommited";
		this.gcCommited.OptionsColumn.AllowEdit = false;
		this.gcCommited.OptionsColumn.ReadOnly = true;
		this.gcCommited.PrimaryKey = false;
		this.gcCommited.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcCommited.UnitField = null;
		this.gcCommited.Visible = true;
		this.gcCommited.VisibleIndex = 5;
		this.gcOnOrder.Caption = "Ordered";
		this.gcOnOrder.ColumnEdit = this.rep_CalcInventory_Quantity;
		this.gcOnOrder.CurrencyDateField = null;
		this.gcOnOrder.CurrencyField = null;
		this.gcOnOrder.CurrencyFixed = null;
		this.gcOnOrder.CurrencyFixedRate = 0.0;
		this.gcOnOrder.CurrencyRateField = null;
		this.gcOnOrder.FieldName = "OnOrder";
		this.gcOnOrder.Name = "gcOnOrder";
		this.gcOnOrder.OptionsColumn.AllowEdit = false;
		this.gcOnOrder.OptionsColumn.ReadOnly = true;
		this.gcOnOrder.PrimaryKey = false;
		this.gcOnOrder.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcOnOrder.UnitField = null;
		this.gcOnOrder.Visible = true;
		this.gcOnOrder.VisibleIndex = 6;
		this.gcAvaiable.Caption = "Avaiable";
		this.gcAvaiable.ColumnEdit = this.rep_CalcInventory_Quantity;
		this.gcAvaiable.CurrencyDateField = null;
		this.gcAvaiable.CurrencyField = null;
		this.gcAvaiable.CurrencyFixed = null;
		this.gcAvaiable.CurrencyFixedRate = 0.0;
		this.gcAvaiable.CurrencyRateField = null;
		this.gcAvaiable.FieldName = "Avaiable";
		this.gcAvaiable.Name = "gcAvaiable";
		this.gcAvaiable.OptionsColumn.AllowEdit = false;
		this.gcAvaiable.OptionsColumn.ReadOnly = true;
		this.gcAvaiable.PrimaryKey = false;
		this.gcAvaiable.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcAvaiable.UnboundExpression = "IsNull([OnHand],0) - IsNull([IsCommited],0) + IsNull([OnOrder],0)";
		this.gcAvaiable.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.gcAvaiable.UnitField = null;
		this.gcAvaiable.Visible = true;
		this.gcAvaiable.VisibleIndex = 7;
		this.gcMinInventory.Caption = "Min.Inventory";
		this.gcMinInventory.CurrencyDateField = null;
		this.gcMinInventory.CurrencyField = null;
		this.gcMinInventory.CurrencyFixed = null;
		this.gcMinInventory.CurrencyFixedRate = 0.0;
		this.gcMinInventory.CurrencyRateField = null;
		this.gcMinInventory.FieldName = "MinStock";
		this.gcMinInventory.Name = "gcMinInventory";
		this.gcMinInventory.PrimaryKey = false;
		this.gcMinInventory.UnitField = null;
		this.gcMaxInventory.Caption = "Max.Inventory";
		this.gcMaxInventory.CurrencyDateField = null;
		this.gcMaxInventory.CurrencyField = null;
		this.gcMaxInventory.CurrencyFixed = null;
		this.gcMaxInventory.CurrencyFixedRate = 0.0;
		this.gcMaxInventory.CurrencyRateField = null;
		this.gcMaxInventory.FieldName = "MaxStock";
		this.gcMaxInventory.Name = "gcMaxInventory";
		this.gcMaxInventory.PrimaryKey = false;
		this.gcMaxInventory.UnitField = null;
		this.gcReqInventory.Caption = "Req.Inventory";
		this.gcReqInventory.CurrencyDateField = null;
		this.gcReqInventory.CurrencyField = null;
		this.gcReqInventory.CurrencyFixed = null;
		this.gcReqInventory.CurrencyFixedRate = 0.0;
		this.gcReqInventory.CurrencyRateField = null;
		this.gcReqInventory.FieldName = "MinOrder";
		this.gcReqInventory.Name = "gcReqInventory";
		this.gcReqInventory.PrimaryKey = false;
		this.gcReqInventory.UnitField = null;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.OptionsColumn.AllowEdit = false;
		this.gcUserSign.OptionsColumn.ReadOnly = true;
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gcCreateDate.Caption = "CreateDate";
		this.gcCreateDate.CurrencyDateField = null;
		this.gcCreateDate.CurrencyField = null;
		this.gcCreateDate.CurrencyFixed = null;
		this.gcCreateDate.CurrencyFixedRate = 0.0;
		this.gcCreateDate.CurrencyRateField = null;
		this.gcCreateDate.FieldName = "CreateDate";
		this.gcCreateDate.Name = "gcCreateDate";
		this.gcCreateDate.OptionsColumn.AllowEdit = false;
		this.gcCreateDate.OptionsColumn.ReadOnly = true;
		this.gcCreateDate.PrimaryKey = false;
		this.gcCreateDate.UnitField = null;
		this.gcUserSign2.Caption = "UserSign2";
		this.gcUserSign2.CurrencyDateField = null;
		this.gcUserSign2.CurrencyField = null;
		this.gcUserSign2.CurrencyFixed = null;
		this.gcUserSign2.CurrencyFixedRate = 0.0;
		this.gcUserSign2.CurrencyRateField = null;
		this.gcUserSign2.FieldName = "UserSign2";
		this.gcUserSign2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUserSign2.Name = "gcUserSign2";
		this.gcUserSign2.OptionsColumn.AllowEdit = false;
		this.gcUserSign2.OptionsColumn.ReadOnly = true;
		this.gcUserSign2.PrimaryKey = false;
		this.gcUserSign2.UnitField = null;
		this.gcUpdateDate.Caption = "UpdateDate";
		this.gcUpdateDate.CurrencyDateField = null;
		this.gcUpdateDate.CurrencyField = null;
		this.gcUpdateDate.CurrencyFixed = null;
		this.gcUpdateDate.CurrencyFixedRate = 0.0;
		this.gcUpdateDate.CurrencyRateField = null;
		this.gcUpdateDate.FieldName = "UpdateDate";
		this.gcUpdateDate.Name = "gcUpdateDate";
		this.gcUpdateDate.OptionsColumn.AllowEdit = false;
		this.gcUpdateDate.OptionsColumn.ReadOnly = true;
		this.gcUpdateDate.PrimaryKey = false;
		this.gcUpdateDate.UnitField = null;
		this.gridColumnEx1.Caption = "Default Bin Location";
		this.gridColumnEx1.ColumnEdit = this.rep_DefaultBinLocation;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DftBinAbs";
		this.gridColumnEx1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 8;
		this.rep_DefaultBinLocation.AutoHeight = false;
		this.rep_DefaultBinLocation.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rep_DefaultBinLocation.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DefaultBinLocation.ContainerControl = this;
		this.rep_DefaultBinLocation.DisplayMember = "";
		this.rep_DefaultBinLocation.DisplayMember2 = "";
		this.rep_DefaultBinLocation.LoadDataSourceWhenEnter = false;
		this.rep_DefaultBinLocation.Name = "rep_DefaultBinLocation";
		this.rep_DefaultBinLocation.SeparateCharacterForMulti = ';';
		this.rep_DefaultBinLocation.SeparateString = ", ";
		this.rep_DefaultBinLocation.UsingAutoCompleteBox = false;
		this.rep_DefaultBinLocation.UsingDataSource = true;
		this.rep_DefaultBinLocation.ValidateWhenNoDataSource = false;
		this.rep_DefaultBinLocation.ValueMember = "";
		this.lb_AccBy.EditControl = this.cbGLMethod;
		this.lb_AccBy.EditValue = "Set G/L Accounts By";
		this.lb_AccBy.FieldName = "lb_AccBy";
		this.lb_AccBy.HideWhenEmpty = false;
		this.lb_AccBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AccBy.Location = new System.Drawing.Point(7, 6);
		this.lb_AccBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_AccBy.Name = "lb_AccBy";
		this.lb_AccBy.ReadOnly = false;
		this.lb_AccBy.SettingInVisible = false;
		this.lb_AccBy.SettingReadonly = false;
		this.lb_AccBy.ShowUnderLine = true;
		this.lb_AccBy.Size = new System.Drawing.Size(152, 20);
		this.lb_AccBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AccBy.TabIndex = 58;
		this.lb_AccBy.TabStop = false;
		this.lb_AccBy.Utext = "Set G/L Accounts By";
		this.lb_InventoryUom.EditControl = this.txtInvntUom;
		this.lb_InventoryUom.EditValue = "UoM Name";
		this.lb_InventoryUom.FieldName = "lb_InventoryUom";
		this.lb_InventoryUom.HideWhenEmpty = false;
		this.lb_InventoryUom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_InventoryUom.Location = new System.Drawing.Point(7, 46);
		this.lb_InventoryUom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_InventoryUom.Name = "lb_InventoryUom";
		this.lb_InventoryUom.ReadOnly = false;
		this.lb_InventoryUom.SettingInVisible = false;
		this.lb_InventoryUom.SettingReadonly = false;
		this.lb_InventoryUom.ShowUnderLine = true;
		this.lb_InventoryUom.Size = new System.Drawing.Size(152, 20);
		this.lb_InventoryUom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_InventoryUom.TabIndex = 56;
		this.lb_InventoryUom.TabStop = false;
		this.lb_InventoryUom.Utext = "UoM Name";
		this.lb_ValuationMethod.EditControl = this.cbEvalSystem;
		this.lb_ValuationMethod.EditValue = "Valuation Method";
		this.lb_ValuationMethod.FieldName = "lb_ValuationMethod";
		this.lb_ValuationMethod.HideWhenEmpty = false;
		this.lb_ValuationMethod.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ValuationMethod.Location = new System.Drawing.Point(7, 152);
		this.lb_ValuationMethod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ValuationMethod.Name = "lb_ValuationMethod";
		this.lb_ValuationMethod.ReadOnly = false;
		this.lb_ValuationMethod.SettingInVisible = false;
		this.lb_ValuationMethod.SettingReadonly = false;
		this.lb_ValuationMethod.ShowUnderLine = true;
		this.lb_ValuationMethod.Size = new System.Drawing.Size(169, 20);
		this.lb_ValuationMethod.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ValuationMethod.TabIndex = 59;
		this.lb_ValuationMethod.TabStop = false;
		this.lb_ValuationMethod.Utext = "Valuation Method";
		this.lbItemCost.EditControl = this.txtAvgPrice;
		this.lbItemCost.EditValue = "Item Cost";
		this.lbItemCost.FieldName = "lbItemCost";
		this.lbItemCost.HideWhenEmpty = false;
		this.lbItemCost.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbItemCost.Location = new System.Drawing.Point(7, 171);
		this.lbItemCost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbItemCost.Name = "lbItemCost";
		this.lbItemCost.ReadOnly = false;
		this.lbItemCost.SettingInVisible = false;
		this.lbItemCost.SettingReadonly = false;
		this.lbItemCost.ShowUnderLine = true;
		this.lbItemCost.Size = new System.Drawing.Size(169, 20);
		this.lbItemCost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbItemCost.TabIndex = 57;
		this.lbItemCost.TabStop = false;
		this.lbItemCost.Utext = "Item Cost";
		this.lb_RequiredPrUOM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_RequiredPrUOM.EditControl = this.txtReorderQty;
		this.lb_RequiredPrUOM.EditValue = "Required (Purchasing UoM)";
		this.lb_RequiredPrUOM.FieldName = "lb_RequiredPrUOM";
		this.lb_RequiredPrUOM.HideWhenEmpty = false;
		this.lb_RequiredPrUOM.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RequiredPrUOM.Location = new System.Drawing.Point(450, 58);
		this.lb_RequiredPrUOM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_RequiredPrUOM.Name = "lb_RequiredPrUOM";
		this.lb_RequiredPrUOM.ReadOnly = false;
		this.lb_RequiredPrUOM.SettingInVisible = false;
		this.lb_RequiredPrUOM.SettingReadonly = false;
		this.lb_RequiredPrUOM.ShowUnderLine = true;
		this.lb_RequiredPrUOM.Size = new System.Drawing.Size(170, 20);
		this.lb_RequiredPrUOM.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RequiredPrUOM.TabIndex = 64;
		this.lb_RequiredPrUOM.TabStop = false;
		this.lb_RequiredPrUOM.Utext = "Required (Purchasing UoM)";
		this.lb_Maximum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Maximum.EditControl = this.txtMaxLevel;
		this.lb_Maximum.EditValue = "Maximum";
		this.lb_Maximum.FieldName = "lb_Maximum";
		this.lb_Maximum.HideWhenEmpty = false;
		this.lb_Maximum.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Maximum.Location = new System.Drawing.Point(450, 100);
		this.lb_Maximum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Maximum.Name = "lb_Maximum";
		this.lb_Maximum.ReadOnly = false;
		this.lb_Maximum.SettingInVisible = false;
		this.lb_Maximum.SettingReadonly = false;
		this.lb_Maximum.ShowUnderLine = true;
		this.lb_Maximum.Size = new System.Drawing.Size(170, 20);
		this.lb_Maximum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Maximum.TabIndex = 66;
		this.lb_Maximum.TabStop = false;
		this.lb_Maximum.Utext = "Maximum";
		this.lb_Minimum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Minimum.EditControl = this.txtMinLevel;
		this.lb_Minimum.EditValue = "Minimum";
		this.lb_Minimum.FieldName = "lb_Minimum";
		this.lb_Minimum.HideWhenEmpty = false;
		this.lb_Minimum.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Minimum.Location = new System.Drawing.Point(450, 79);
		this.lb_Minimum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Minimum.Name = "lb_Minimum";
		this.lb_Minimum.ReadOnly = false;
		this.lb_Minimum.SettingInVisible = false;
		this.lb_Minimum.SettingReadonly = false;
		this.lb_Minimum.ShowUnderLine = true;
		this.lb_Minimum.Size = new System.Drawing.Size(171, 20);
		this.lb_Minimum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Minimum.TabIndex = 65;
		this.lb_Minimum.TabStop = false;
		this.lb_Minimum.Utext = "Minimum";
		this.tab_PlanData.Controls.Add(this.cb_OrderInterval);
		this.tab_PlanData.Controls.Add(this.cb_ComponentWarehouse);
		this.tab_PlanData.Controls.Add(this.cb_ProcurementMethod);
		this.tab_PlanData.Controls.Add(this.cb_PlanningMethod);
		this.tab_PlanData.Controls.Add(this.txt_ToleranceDays);
		this.tab_PlanData.Controls.Add(this.txt_LeadTime);
		this.tab_PlanData.Controls.Add(this.lb_ComponentWarehouse);
		this.tab_PlanData.Controls.Add(this.lb_Days1);
		this.tab_PlanData.Controls.Add(this.lb_Days);
		this.tab_PlanData.Controls.Add(this.txt_MinimumOrderQty);
		this.tab_PlanData.Controls.Add(this.txt_OrderMultiple);
		this.tab_PlanData.Controls.Add(this.lb_ToleranceDays);
		this.tab_PlanData.Controls.Add(this.lb_MiniumOrderQty);
		this.tab_PlanData.Controls.Add(this.lb_LeadTime);
		this.tab_PlanData.Controls.Add(this.lb_OrderMultiple);
		this.tab_PlanData.Controls.Add(this.lb_ProcurementMethod);
		this.tab_PlanData.Controls.Add(this.lb_OrderInterval);
		this.tab_PlanData.Controls.Add(this.lb_PlanningMethod);
		this.tab_PlanData.EditControl = null;
		this.tab_PlanData.EditValue = "Planning Data";
		this.tab_PlanData.FieldName = "tab_PlanData";
		this.tab_PlanData.IsSearchControl = false;
		this.tab_PlanData.Name = "tab_PlanData";
		this.tab_PlanData.ReadOnly = true;
		this.tab_PlanData.SettingInVisible = false;
		this.tab_PlanData.SettingReadonly = false;
		this.tab_PlanData.Size = new System.Drawing.Size(819, 422);
		this.tab_PlanData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_PlanData.Text = "Planning Data";
		this.cb_OrderInterval.EditControl = null;
		this.cb_OrderInterval.FieldName = "OrdrIntrvl";
		this.cb_OrderInterval.IsSearchControl = false;
		this.cb_OrderInterval.Location = new System.Drawing.Point(199, 77);
		this.cb_OrderInterval.Name = "cb_OrderInterval";
		this.cb_OrderInterval.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_OrderInterval.Properties.Appearance.Options.UseFont = true;
		this.cb_OrderInterval.Properties.AutoHeight = false;
		this.cb_OrderInterval.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_OrderInterval.Properties.IsSearchControl = false;
		this.cb_OrderInterval.Properties.NullText = "";
		this.cb_OrderInterval.SettingInVisible = false;
		this.cb_OrderInterval.SettingReadonly = false;
		this.cb_OrderInterval.Size = new System.Drawing.Size(273, 18);
		this.cb_OrderInterval.TabIndex = 3;
		this.cb_OrderInterval.Tag = "OITM,OrdrIntrvl";
		this.cb_OrderInterval.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cb_OrderInterval_EditValueChanging);
		this.cb_ComponentWarehouse.EditControl = null;
		this.cb_ComponentWarehouse.FieldName = "CompoWH";
		this.cb_ComponentWarehouse.IsSearchControl = false;
		this.cb_ComponentWarehouse.Location = new System.Drawing.Point(198, 57);
		this.cb_ComponentWarehouse.Name = "cb_ComponentWarehouse";
		this.cb_ComponentWarehouse.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_ComponentWarehouse.Properties.Appearance.Options.UseFont = true;
		this.cb_ComponentWarehouse.Properties.AutoHeight = false;
		this.cb_ComponentWarehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_ComponentWarehouse.Properties.IsSearchControl = false;
		this.cb_ComponentWarehouse.Properties.NullText = "";
		this.cb_ComponentWarehouse.SettingInVisible = false;
		this.cb_ComponentWarehouse.SettingReadonly = false;
		this.cb_ComponentWarehouse.Size = new System.Drawing.Size(273, 18);
		this.cb_ComponentWarehouse.TabIndex = 2;
		this.cb_ComponentWarehouse.Tag = "OITM,CompoWH";
		this.cb_ProcurementMethod.EditControl = null;
		this.cb_ProcurementMethod.FieldName = "PrcrmntMtd";
		this.cb_ProcurementMethod.IsSearchControl = false;
		this.cb_ProcurementMethod.Location = new System.Drawing.Point(198, 37);
		this.cb_ProcurementMethod.Name = "cb_ProcurementMethod";
		this.cb_ProcurementMethod.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_ProcurementMethod.Properties.Appearance.Options.UseFont = true;
		this.cb_ProcurementMethod.Properties.AutoHeight = false;
		this.cb_ProcurementMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_ProcurementMethod.Properties.IsSearchControl = false;
		this.cb_ProcurementMethod.Properties.NullText = "";
		this.cb_ProcurementMethod.SettingInVisible = false;
		this.cb_ProcurementMethod.SettingReadonly = false;
		this.cb_ProcurementMethod.Size = new System.Drawing.Size(273, 18);
		this.cb_ProcurementMethod.TabIndex = 1;
		this.cb_ProcurementMethod.Tag = "OITM,PrcrmntMtd";
		this.cb_PlanningMethod.EditControl = null;
		this.cb_PlanningMethod.FieldName = "PlaningSys";
		this.cb_PlanningMethod.IsSearchControl = false;
		this.cb_PlanningMethod.Location = new System.Drawing.Point(199, 17);
		this.cb_PlanningMethod.Name = "cb_PlanningMethod";
		this.cb_PlanningMethod.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_PlanningMethod.Properties.Appearance.Options.UseFont = true;
		this.cb_PlanningMethod.Properties.AutoHeight = false;
		this.cb_PlanningMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_PlanningMethod.Properties.IsSearchControl = false;
		this.cb_PlanningMethod.Properties.NullText = "";
		this.cb_PlanningMethod.SettingInVisible = false;
		this.cb_PlanningMethod.SettingReadonly = false;
		this.cb_PlanningMethod.Size = new System.Drawing.Size(273, 18);
		this.cb_PlanningMethod.TabIndex = 0;
		this.cb_PlanningMethod.Tag = "OITM,PlaningSys";
		this.txt_ToleranceDays.EditControl = null;
		this.txt_ToleranceDays.FieldName = "ToleranDay";
		this.txt_ToleranceDays.IsSearchControl = false;
		this.txt_ToleranceDays.Location = new System.Drawing.Point(198, 210);
		this.txt_ToleranceDays.Name = "txt_ToleranceDays";
		this.txt_ToleranceDays.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_ToleranceDays.Properties.Appearance.Options.UseFont = true;
		this.txt_ToleranceDays.Properties.AutoHeight = false;
		this.txt_ToleranceDays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToleranceDays.Properties.DisplayFormat.FormatString = "n3";
		this.txt_ToleranceDays.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_ToleranceDays.Properties.EditFormat.FormatString = "n3";
		this.txt_ToleranceDays.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_ToleranceDays.Properties.IsSearchControl = false;
		this.txt_ToleranceDays.SettingInVisible = false;
		this.txt_ToleranceDays.SettingReadonly = false;
		this.txt_ToleranceDays.Size = new System.Drawing.Size(274, 18);
		this.txt_ToleranceDays.TabIndex = 7;
		this.txt_ToleranceDays.Tag = "OITM,ToleranDay";
		this.txt_LeadTime.EditControl = null;
		this.txt_LeadTime.FieldName = "LeadTime";
		this.txt_LeadTime.IsSearchControl = false;
		this.txt_LeadTime.Location = new System.Drawing.Point(198, 191);
		this.txt_LeadTime.Name = "txt_LeadTime";
		this.txt_LeadTime.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_LeadTime.Properties.Appearance.Options.UseFont = true;
		this.txt_LeadTime.Properties.AutoHeight = false;
		this.txt_LeadTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_LeadTime.Properties.DisplayFormat.FormatString = "n3";
		this.txt_LeadTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_LeadTime.Properties.EditFormat.FormatString = "n3";
		this.txt_LeadTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_LeadTime.Properties.IsSearchControl = false;
		this.txt_LeadTime.SettingInVisible = false;
		this.txt_LeadTime.SettingReadonly = false;
		this.txt_LeadTime.Size = new System.Drawing.Size(274, 18);
		this.txt_LeadTime.TabIndex = 6;
		this.txt_LeadTime.Tag = "OITM,LeadTime";
		this.lb_ComponentWarehouse.EditControl = this.cb_ComponentWarehouse;
		this.lb_ComponentWarehouse.EditValue = "Component Warehouse";
		this.lb_ComponentWarehouse.FieldName = "lb_ComponentWarehouse";
		this.lb_ComponentWarehouse.HideWhenEmpty = false;
		this.lb_ComponentWarehouse.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ComponentWarehouse.Location = new System.Drawing.Point(20, 58);
		this.lb_ComponentWarehouse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ComponentWarehouse.Name = "lb_ComponentWarehouse";
		this.lb_ComponentWarehouse.ReadOnly = false;
		this.lb_ComponentWarehouse.SettingInVisible = false;
		this.lb_ComponentWarehouse.SettingReadonly = false;
		this.lb_ComponentWarehouse.ShowUnderLine = true;
		this.lb_ComponentWarehouse.Size = new System.Drawing.Size(180, 20);
		this.lb_ComponentWarehouse.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ComponentWarehouse.TabIndex = 4;
		this.lb_ComponentWarehouse.TabStop = false;
		this.lb_ComponentWarehouse.Utext = "Component Warehouse";
		this.lb_Days1.EditControl = null;
		this.lb_Days1.EditValue = "Days";
		this.lb_Days1.FieldName = null;
		this.lb_Days1.HideWhenEmpty = false;
		this.lb_Days1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Days1.Location = new System.Drawing.Point(478, 209);
		this.lb_Days1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Days1.Name = "lb_Days1";
		this.lb_Days1.ReadOnly = false;
		this.lb_Days1.SettingInVisible = false;
		this.lb_Days1.SettingReadonly = false;
		this.lb_Days1.ShowUnderLine = true;
		this.lb_Days1.Size = new System.Drawing.Size(44, 20);
		this.lb_Days1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Days1.TabIndex = 2;
		this.lb_Days1.TabStop = false;
		this.lb_Days1.Utext = "Days";
		this.lb_Days.EditControl = null;
		this.lb_Days.EditValue = "Days";
		this.lb_Days.FieldName = null;
		this.lb_Days.HideWhenEmpty = false;
		this.lb_Days.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Days.Location = new System.Drawing.Point(478, 192);
		this.lb_Days.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Days.Name = "lb_Days";
		this.lb_Days.ReadOnly = false;
		this.lb_Days.SettingInVisible = false;
		this.lb_Days.SettingReadonly = false;
		this.lb_Days.ShowUnderLine = true;
		this.lb_Days.Size = new System.Drawing.Size(44, 20);
		this.lb_Days.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Days.TabIndex = 2;
		this.lb_Days.TabStop = false;
		this.lb_Days.Utext = "Days";
		this.txt_MinimumOrderQty.EditControl = null;
		this.txt_MinimumOrderQty.FieldName = "MinOrdrQty";
		this.txt_MinimumOrderQty.IsSearchControl = false;
		this.txt_MinimumOrderQty.Location = new System.Drawing.Point(198, 117);
		this.txt_MinimumOrderQty.Name = "txt_MinimumOrderQty";
		this.txt_MinimumOrderQty.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_MinimumOrderQty.Properties.Appearance.Options.UseFont = true;
		this.txt_MinimumOrderQty.Properties.AutoHeight = false;
		this.txt_MinimumOrderQty.Properties.IsSearchControl = false;
		this.txt_MinimumOrderQty.SettingInVisible = false;
		this.txt_MinimumOrderQty.SettingReadonly = false;
		this.txt_MinimumOrderQty.Size = new System.Drawing.Size(274, 18);
		this.txt_MinimumOrderQty.TabIndex = 5;
		this.txt_MinimumOrderQty.Tag = "OITM,MinOrdrQty";
		this.txt_OrderMultiple.EditControl = null;
		this.txt_OrderMultiple.FieldName = "OrdrMulti";
		this.txt_OrderMultiple.IsSearchControl = false;
		this.txt_OrderMultiple.Location = new System.Drawing.Point(198, 97);
		this.txt_OrderMultiple.Name = "txt_OrderMultiple";
		this.txt_OrderMultiple.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_OrderMultiple.Properties.Appearance.Options.UseFont = true;
		this.txt_OrderMultiple.Properties.AutoHeight = false;
		this.txt_OrderMultiple.Properties.IsSearchControl = false;
		this.txt_OrderMultiple.SettingInVisible = false;
		this.txt_OrderMultiple.SettingReadonly = false;
		this.txt_OrderMultiple.Size = new System.Drawing.Size(274, 18);
		this.txt_OrderMultiple.TabIndex = 4;
		this.txt_OrderMultiple.Tag = "OITM,OrdrMulti";
		this.lb_ToleranceDays.EditControl = this.txt_ToleranceDays;
		this.lb_ToleranceDays.EditValue = "Tolerance Days";
		this.lb_ToleranceDays.FieldName = "lb_ToleranceDays";
		this.lb_ToleranceDays.HideWhenEmpty = false;
		this.lb_ToleranceDays.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToleranceDays.Location = new System.Drawing.Point(20, 211);
		this.lb_ToleranceDays.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToleranceDays.Name = "lb_ToleranceDays";
		this.lb_ToleranceDays.ReadOnly = false;
		this.lb_ToleranceDays.SettingInVisible = false;
		this.lb_ToleranceDays.SettingReadonly = false;
		this.lb_ToleranceDays.ShowUnderLine = true;
		this.lb_ToleranceDays.Size = new System.Drawing.Size(185, 20);
		this.lb_ToleranceDays.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToleranceDays.TabIndex = 0;
		this.lb_ToleranceDays.TabStop = false;
		this.lb_ToleranceDays.Utext = "Tolerance Days";
		this.lb_MiniumOrderQty.EditControl = this.txt_MinimumOrderQty;
		this.lb_MiniumOrderQty.EditValue = "Minimum Order Qty";
		this.lb_MiniumOrderQty.FieldName = "lb_MiniumOrderQty";
		this.lb_MiniumOrderQty.HideWhenEmpty = false;
		this.lb_MiniumOrderQty.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MiniumOrderQty.Location = new System.Drawing.Point(20, 118);
		this.lb_MiniumOrderQty.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_MiniumOrderQty.Name = "lb_MiniumOrderQty";
		this.lb_MiniumOrderQty.ReadOnly = false;
		this.lb_MiniumOrderQty.SettingInVisible = false;
		this.lb_MiniumOrderQty.SettingReadonly = false;
		this.lb_MiniumOrderQty.ShowUnderLine = true;
		this.lb_MiniumOrderQty.Size = new System.Drawing.Size(185, 20);
		this.lb_MiniumOrderQty.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MiniumOrderQty.TabIndex = 0;
		this.lb_MiniumOrderQty.TabStop = false;
		this.lb_MiniumOrderQty.Utext = "Minimum Order Qty";
		this.lb_LeadTime.EditControl = this.txt_LeadTime;
		this.lb_LeadTime.EditValue = "Lead Time";
		this.lb_LeadTime.FieldName = "lb_LeadTime";
		this.lb_LeadTime.HideWhenEmpty = false;
		this.lb_LeadTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_LeadTime.Location = new System.Drawing.Point(20, 192);
		this.lb_LeadTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_LeadTime.Name = "lb_LeadTime";
		this.lb_LeadTime.ReadOnly = false;
		this.lb_LeadTime.SettingInVisible = false;
		this.lb_LeadTime.SettingReadonly = false;
		this.lb_LeadTime.ShowUnderLine = true;
		this.lb_LeadTime.Size = new System.Drawing.Size(185, 20);
		this.lb_LeadTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_LeadTime.TabIndex = 0;
		this.lb_LeadTime.TabStop = false;
		this.lb_LeadTime.Utext = "Lead Time";
		this.lb_OrderMultiple.EditControl = this.txt_OrderMultiple;
		this.lb_OrderMultiple.EditValue = "Order Multiple";
		this.lb_OrderMultiple.FieldName = "lb_OrderMultiple";
		this.lb_OrderMultiple.HideWhenEmpty = false;
		this.lb_OrderMultiple.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OrderMultiple.Location = new System.Drawing.Point(20, 98);
		this.lb_OrderMultiple.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_OrderMultiple.Name = "lb_OrderMultiple";
		this.lb_OrderMultiple.ReadOnly = false;
		this.lb_OrderMultiple.SettingInVisible = false;
		this.lb_OrderMultiple.SettingReadonly = false;
		this.lb_OrderMultiple.ShowUnderLine = true;
		this.lb_OrderMultiple.Size = new System.Drawing.Size(185, 20);
		this.lb_OrderMultiple.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OrderMultiple.TabIndex = 0;
		this.lb_OrderMultiple.TabStop = false;
		this.lb_OrderMultiple.Utext = "Order Multiple";
		this.lb_ProcurementMethod.EditControl = this.cb_ProcurementMethod;
		this.lb_ProcurementMethod.EditValue = "Procurement Method";
		this.lb_ProcurementMethod.FieldName = "lb_ProcurementMethod";
		this.lb_ProcurementMethod.HideWhenEmpty = false;
		this.lb_ProcurementMethod.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProcurementMethod.Location = new System.Drawing.Point(20, 38);
		this.lb_ProcurementMethod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ProcurementMethod.Name = "lb_ProcurementMethod";
		this.lb_ProcurementMethod.ReadOnly = false;
		this.lb_ProcurementMethod.SettingInVisible = false;
		this.lb_ProcurementMethod.SettingReadonly = false;
		this.lb_ProcurementMethod.ShowUnderLine = true;
		this.lb_ProcurementMethod.Size = new System.Drawing.Size(185, 20);
		this.lb_ProcurementMethod.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProcurementMethod.TabIndex = 0;
		this.lb_ProcurementMethod.TabStop = false;
		this.lb_ProcurementMethod.Utext = "Procurement Method";
		this.lb_OrderInterval.EditControl = this.cb_OrderInterval;
		this.lb_OrderInterval.EditValue = "Order Interval";
		this.lb_OrderInterval.FieldName = "lb_OrderInterval";
		this.lb_OrderInterval.HideWhenEmpty = false;
		this.lb_OrderInterval.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OrderInterval.Location = new System.Drawing.Point(20, 78);
		this.lb_OrderInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_OrderInterval.Name = "lb_OrderInterval";
		this.lb_OrderInterval.ReadOnly = false;
		this.lb_OrderInterval.SettingInVisible = false;
		this.lb_OrderInterval.SettingReadonly = false;
		this.lb_OrderInterval.ShowUnderLine = true;
		this.lb_OrderInterval.Size = new System.Drawing.Size(185, 20);
		this.lb_OrderInterval.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OrderInterval.TabIndex = 0;
		this.lb_OrderInterval.TabStop = false;
		this.lb_OrderInterval.Utext = "Order Interval";
		this.lb_PlanningMethod.EditControl = this.cb_PlanningMethod;
		this.lb_PlanningMethod.EditValue = "Planning Method";
		this.lb_PlanningMethod.FieldName = "lb_PlanningMethod";
		this.lb_PlanningMethod.HideWhenEmpty = false;
		this.lb_PlanningMethod.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PlanningMethod.Location = new System.Drawing.Point(20, 18);
		this.lb_PlanningMethod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PlanningMethod.Name = "lb_PlanningMethod";
		this.lb_PlanningMethod.ReadOnly = false;
		this.lb_PlanningMethod.SettingInVisible = false;
		this.lb_PlanningMethod.SettingReadonly = false;
		this.lb_PlanningMethod.ShowUnderLine = true;
		this.lb_PlanningMethod.Size = new System.Drawing.Size(185, 20);
		this.lb_PlanningMethod.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PlanningMethod.TabIndex = 0;
		this.lb_PlanningMethod.TabStop = false;
		this.lb_PlanningMethod.Utext = "Planning Method";
		this.tab_Production.Controls.Add(this.calc_PrdStdCst);
		this.tab_Production.Controls.Add(this.txt_TreeType);
		this.tab_Production.Controls.Add(this.chk_InCostRoll);
		this.tab_Production.Controls.Add(this.lb_TreeType);
		this.tab_Production.Controls.Add(this.calc_NoOfRes);
		this.tab_Production.Controls.Add(this.calc_NoOfItem);
		this.tab_Production.Controls.Add(this.lb_NoOfItem);
		this.tab_Production.Controls.Add(this.lb_PrdStdCst);
		this.tab_Production.Controls.Add(this.lb_NoOfRes);
		this.tab_Production.Controls.Add(this.luk_IssueMthd);
		this.tab_Production.Controls.Add(this.lb_IssueMthd);
		this.tab_Production.Controls.Add(this.chk_Phantom);
		this.tab_Production.EditControl = null;
		this.tab_Production.EditValue = "Production Data";
		this.tab_Production.FieldName = "tab_Production";
		this.tab_Production.IsSearchControl = false;
		this.tab_Production.Name = "tab_Production";
		this.tab_Production.ReadOnly = true;
		this.tab_Production.SettingInVisible = false;
		this.tab_Production.SettingReadonly = false;
		this.tab_Production.Size = new System.Drawing.Size(819, 422);
		this.tab_Production.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Production.Text = "Production Data";
		this.calc_PrdStdCst.Currency = null;
		this.calc_PrdStdCst.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.None;
		this.calc_PrdStdCst.EditControl = null;
		this.calc_PrdStdCst.FieldName = "PrdStdCst";
		this.calc_PrdStdCst.IsSearchControl = false;
		this.calc_PrdStdCst.Location = new System.Drawing.Point(180, 156);
		this.calc_PrdStdCst.Name = "calc_PrdStdCst";
		this.calc_PrdStdCst.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_PrdStdCst.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_PrdStdCst.Properties.AutoHeight = false;
		this.calc_PrdStdCst.SettingInVisible = false;
		this.calc_PrdStdCst.SettingReadonly = false;
		this.calc_PrdStdCst.Size = new System.Drawing.Size(163, 18);
		this.calc_PrdStdCst.TabIndex = 2;
		this.calc_PrdStdCst.Tag = "OITM,PrdStdCst";
		this.txt_TreeType.EditControl = null;
		this.txt_TreeType.FieldName = "TreeType";
		this.txt_TreeType.IsSearchControl = false;
		this.txt_TreeType.Location = new System.Drawing.Point(180, 97);
		this.txt_TreeType.Name = "txt_TreeType";
		this.txt_TreeType.Properties.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.txt_TreeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_TreeType.Properties.ReadOnly = true;
		this.txt_TreeType.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_TreeType_Properties_ButtonPressed);
		this.txt_TreeType.SettingInVisible = false;
		this.txt_TreeType.SettingReadonly = false;
		this.txt_TreeType.Size = new System.Drawing.Size(163, 20);
		this.txt_TreeType.TabIndex = 77;
		this.txt_TreeType.TabStop = false;
		this.txt_TreeType.Tag = "OITT,TreeType";
		this.chk_InCostRoll.EditControl = this.chk_InCostRoll;
		this.chk_InCostRoll.FieldName = "InCostRoll";
		this.chk_InCostRoll.IsSearchControl = false;
		this.chk_InCostRoll.Location = new System.Drawing.Point(11, 178);
		this.chk_InCostRoll.Name = "chk_InCostRoll";
		this.chk_InCostRoll.Properties.Caption = "Include in Production Std Cost Rollup";
		this.chk_InCostRoll.Properties.DisplayValueChecked = "Y";
		this.chk_InCostRoll.Properties.DisplayValueGrayed = "N";
		this.chk_InCostRoll.Properties.DisplayValueUnchecked = "N";
		this.chk_InCostRoll.Properties.ValueChecked = "Y";
		this.chk_InCostRoll.Properties.ValueGrayed = "N";
		this.chk_InCostRoll.Properties.ValueUnchecked = "N";
		this.chk_InCostRoll.SettingInVisible = false;
		this.chk_InCostRoll.SettingReadonly = false;
		this.chk_InCostRoll.Size = new System.Drawing.Size(202, 20);
		this.chk_InCostRoll.TabIndex = 3;
		this.chk_InCostRoll.Tag = "OITM,InCostRoll";
		this.lb_TreeType.EditControl = this.txt_TreeType;
		this.lb_TreeType.EditValue = "BOM Type";
		this.lb_TreeType.FieldName = "lb_TreeType";
		this.lb_TreeType.HideWhenEmpty = false;
		this.lb_TreeType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TreeType.Location = new System.Drawing.Point(11, 100);
		this.lb_TreeType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TreeType.Name = "lb_TreeType";
		this.lb_TreeType.ReadOnly = true;
		this.lb_TreeType.SettingInVisible = false;
		this.lb_TreeType.SettingReadonly = false;
		this.lb_TreeType.ShowUnderLine = true;
		this.lb_TreeType.Size = new System.Drawing.Size(170, 20);
		this.lb_TreeType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TreeType.TabIndex = 73;
		this.lb_TreeType.TabStop = false;
		this.lb_TreeType.Utext = "BOM Type";
		this.calc_NoOfRes.EditControl = null;
		this.calc_NoOfRes.FieldName = null;
		this.calc_NoOfRes.IsSearchControl = false;
		this.calc_NoOfRes.Location = new System.Drawing.Point(180, 137);
		this.calc_NoOfRes.Name = "calc_NoOfRes";
		this.calc_NoOfRes.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_NoOfRes.Properties.Appearance.Options.UseFont = true;
		this.calc_NoOfRes.Properties.AutoHeight = false;
		this.calc_NoOfRes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NoOfRes.Properties.DisplayFormat.FormatString = "n0";
		this.calc_NoOfRes.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_NoOfRes.Properties.EditFormat.FormatString = "n0";
		this.calc_NoOfRes.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_NoOfRes.Properties.IsSearchControl = false;
		this.calc_NoOfRes.Properties.ReadOnly = true;
		this.calc_NoOfRes.SettingInVisible = false;
		this.calc_NoOfRes.SettingReadonly = false;
		this.calc_NoOfRes.Size = new System.Drawing.Size(163, 18);
		this.calc_NoOfRes.TabIndex = 68;
		this.calc_NoOfRes.TabStop = false;
		this.calc_NoOfItem.EditControl = null;
		this.calc_NoOfItem.FieldName = null;
		this.calc_NoOfItem.IsSearchControl = false;
		this.calc_NoOfItem.Location = new System.Drawing.Point(180, 118);
		this.calc_NoOfItem.Name = "calc_NoOfItem";
		this.calc_NoOfItem.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.calc_NoOfItem.Properties.Appearance.Options.UseFont = true;
		this.calc_NoOfItem.Properties.AutoHeight = false;
		this.calc_NoOfItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NoOfItem.Properties.DisplayFormat.FormatString = "n0";
		this.calc_NoOfItem.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_NoOfItem.Properties.EditFormat.FormatString = "n0";
		this.calc_NoOfItem.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_NoOfItem.Properties.IsSearchControl = false;
		this.calc_NoOfItem.Properties.ReadOnly = true;
		this.calc_NoOfItem.SettingInVisible = false;
		this.calc_NoOfItem.SettingReadonly = false;
		this.calc_NoOfItem.Size = new System.Drawing.Size(163, 18);
		this.calc_NoOfItem.TabIndex = 67;
		this.calc_NoOfItem.TabStop = false;
		this.lb_NoOfItem.EditControl = this.calc_NoOfItem;
		this.lb_NoOfItem.EditValue = "No. of Item Components";
		this.lb_NoOfItem.FieldName = "lb_NoOfItem";
		this.lb_NoOfItem.HideWhenEmpty = false;
		this.lb_NoOfItem.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NoOfItem.Location = new System.Drawing.Point(11, 119);
		this.lb_NoOfItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_NoOfItem.Name = "lb_NoOfItem";
		this.lb_NoOfItem.ReadOnly = true;
		this.lb_NoOfItem.SettingInVisible = false;
		this.lb_NoOfItem.SettingReadonly = false;
		this.lb_NoOfItem.ShowUnderLine = true;
		this.lb_NoOfItem.Size = new System.Drawing.Size(170, 20);
		this.lb_NoOfItem.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NoOfItem.TabIndex = 70;
		this.lb_NoOfItem.TabStop = false;
		this.lb_NoOfItem.Utext = "No. of Item Components";
		this.lb_PrdStdCst.EditControl = this.calc_PrdStdCst;
		this.lb_PrdStdCst.EditValue = "Production Std Cost";
		this.lb_PrdStdCst.FieldName = "lb_PrdStdCst";
		this.lb_PrdStdCst.HideWhenEmpty = false;
		this.lb_PrdStdCst.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PrdStdCst.Location = new System.Drawing.Point(11, 157);
		this.lb_PrdStdCst.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PrdStdCst.Name = "lb_PrdStdCst";
		this.lb_PrdStdCst.ReadOnly = false;
		this.lb_PrdStdCst.SettingInVisible = false;
		this.lb_PrdStdCst.SettingReadonly = false;
		this.lb_PrdStdCst.ShowUnderLine = true;
		this.lb_PrdStdCst.Size = new System.Drawing.Size(170, 20);
		this.lb_PrdStdCst.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PrdStdCst.TabIndex = 72;
		this.lb_PrdStdCst.TabStop = false;
		this.lb_PrdStdCst.Utext = "Production Std Cost";
		this.lb_NoOfRes.EditControl = this.calc_NoOfRes;
		this.lb_NoOfRes.EditValue = "No. of Resource Components";
		this.lb_NoOfRes.FieldName = "lb_NoOfRes";
		this.lb_NoOfRes.HideWhenEmpty = false;
		this.lb_NoOfRes.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NoOfRes.Location = new System.Drawing.Point(11, 138);
		this.lb_NoOfRes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_NoOfRes.Name = "lb_NoOfRes";
		this.lb_NoOfRes.ReadOnly = true;
		this.lb_NoOfRes.SettingInVisible = false;
		this.lb_NoOfRes.SettingReadonly = false;
		this.lb_NoOfRes.ShowUnderLine = true;
		this.lb_NoOfRes.Size = new System.Drawing.Size(171, 20);
		this.lb_NoOfRes.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NoOfRes.TabIndex = 71;
		this.lb_NoOfRes.TabStop = false;
		this.lb_NoOfRes.Utext = "No. of Resource Components";
		this.luk_IssueMthd.EditControl = null;
		this.luk_IssueMthd.FieldName = "IssueMthd";
		this.luk_IssueMthd.IsSearchControl = false;
		this.luk_IssueMthd.Location = new System.Drawing.Point(180, 40);
		this.luk_IssueMthd.Name = "luk_IssueMthd";
		this.luk_IssueMthd.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_IssueMthd.Properties.Appearance.Options.UseFont = true;
		this.luk_IssueMthd.Properties.AutoHeight = false;
		this.luk_IssueMthd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_IssueMthd.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_IssueMthd.Properties.DisplayMember = "Name";
		this.luk_IssueMthd.Properties.IsSearchControl = false;
		this.luk_IssueMthd.Properties.NullText = "";
		this.luk_IssueMthd.Properties.ValueMember = "Code";
		this.luk_IssueMthd.SettingInVisible = false;
		this.luk_IssueMthd.SettingReadonly = false;
		this.luk_IssueMthd.Size = new System.Drawing.Size(163, 18);
		this.luk_IssueMthd.TabIndex = 1;
		this.luk_IssueMthd.Tag = "OITM,IssueMthd";
		this.lb_IssueMthd.EditControl = this.luk_IssueMthd;
		this.lb_IssueMthd.EditValue = "Issue Method";
		this.lb_IssueMthd.FieldName = "lb_IssueMthd";
		this.lb_IssueMthd.HideWhenEmpty = false;
		this.lb_IssueMthd.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_IssueMthd.Location = new System.Drawing.Point(11, 41);
		this.lb_IssueMthd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_IssueMthd.Name = "lb_IssueMthd";
		this.lb_IssueMthd.ReadOnly = false;
		this.lb_IssueMthd.SettingInVisible = false;
		this.lb_IssueMthd.SettingReadonly = false;
		this.lb_IssueMthd.ShowUnderLine = true;
		this.lb_IssueMthd.Size = new System.Drawing.Size(170, 20);
		this.lb_IssueMthd.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_IssueMthd.TabIndex = 11;
		this.lb_IssueMthd.TabStop = false;
		this.lb_IssueMthd.Utext = "Issue Method";
		this.chk_Phantom.EditControl = this.chk_Phantom;
		this.chk_Phantom.FieldName = "Phantom";
		this.chk_Phantom.IsSearchControl = false;
		this.chk_Phantom.Location = new System.Drawing.Point(11, 21);
		this.chk_Phantom.Name = "chk_Phantom";
		this.chk_Phantom.Properties.Caption = "Phantom Item";
		this.chk_Phantom.Properties.DisplayValueChecked = "Y";
		this.chk_Phantom.Properties.DisplayValueGrayed = "N";
		this.chk_Phantom.Properties.DisplayValueUnchecked = "N";
		this.chk_Phantom.Properties.ValueChecked = "Y";
		this.chk_Phantom.Properties.ValueGrayed = "N";
		this.chk_Phantom.Properties.ValueUnchecked = "N";
		this.chk_Phantom.SettingInVisible = false;
		this.chk_Phantom.SettingReadonly = false;
		this.chk_Phantom.Size = new System.Drawing.Size(90, 20);
		this.chk_Phantom.TabIndex = 0;
		this.chk_Phantom.Tag = "OITM,Phantom";
		this.tab_Properties.Controls.Add(this.btn_PropertiesClearSelect);
		this.tab_Properties.Controls.Add(this.btn_PropertiesSellectAll);
		this.tab_Properties.Controls.Add(this.grd_Properties);
		this.tab_Properties.EditControl = null;
		this.tab_Properties.EditValue = "Properties";
		this.tab_Properties.FieldName = "tab_Properties";
		this.tab_Properties.IsSearchControl = false;
		this.tab_Properties.Name = "tab_Properties";
		this.tab_Properties.ReadOnly = true;
		this.tab_Properties.SettingInVisible = false;
		this.tab_Properties.SettingReadonly = false;
		this.tab_Properties.Size = new System.Drawing.Size(819, 422);
		this.tab_Properties.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Properties.Text = "Properties";
		this.btn_PropertiesClearSelect.EditControl = null;
		this.btn_PropertiesClearSelect.EditValue = "Clear Selection";
		this.btn_PropertiesClearSelect.FieldName = null;
		this.btn_PropertiesClearSelect.IsSearchControl = false;
		this.btn_PropertiesClearSelect.Location = new System.Drawing.Point(446, 36);
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
		this.btn_PropertiesSellectAll.Location = new System.Drawing.Point(446, 7);
		this.btn_PropertiesSellectAll.Name = "btn_PropertiesSellectAll";
		this.btn_PropertiesSellectAll.ReadOnly = false;
		this.btn_PropertiesSellectAll.SettingInVisible = false;
		this.btn_PropertiesSellectAll.SettingReadonly = false;
		this.btn_PropertiesSellectAll.Size = new System.Drawing.Size(124, 23);
		this.btn_PropertiesSellectAll.TabIndex = 1;
		this.btn_PropertiesSellectAll.Text = "Select All";
		this.btn_PropertiesSellectAll.Click += new System.EventHandler(this.btn_PropertiesSellectAll_Click);
		this.grd_Properties.AllowPasteExcel = false;
		this.grd_Properties.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grd_Properties.ClipboardData = null;
		this.grd_Properties.Location = new System.Drawing.Point(5, 7);
		this.grd_Properties.MainView = this.gv_Properties;
		this.grd_Properties.Name = "grd_Properties";
		this.grd_Properties.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_PropertiesCheck });
		this.grd_Properties.Size = new System.Drawing.Size(400, 306);
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
		this.gv_Properties.FormId = "oItem";
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
		this.gv_Properties.OptionsFind.AllowFindPanel = false;
		this.gv_Properties.OptionsNavigation.UseTabKey = false;
		this.gv_Properties.OptionsView.ColumnAutoWidth = false;
		this.gv_Properties.OptionsView.ShowGroupPanel = false;
		this.gv_Properties.UseBatchMod = false;
		this.gv_Properties.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Properties_CustomDrawRowIndicator);
		this.gridColumn1.Caption = "Number";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItmsTypCod";
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
		this.gridColumn2.FieldName = "ItmsGrpNam";
		this.gridColumn2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
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
		this.rep_PropertiesCheck.CheckedChanged += new System.EventHandler(this.rep_PropertiesCheck_CheckedChanged);
		this.tab_Remark.Controls.Add(this.panelControl9);
		this.tab_Remark.EditControl = null;
		this.tab_Remark.EditValue = "Remark";
		this.tab_Remark.FieldName = "tab_Remark";
		this.tab_Remark.IsSearchControl = false;
		this.tab_Remark.Name = "tab_Remark";
		this.tab_Remark.ReadOnly = true;
		this.tab_Remark.SettingInVisible = false;
		this.tab_Remark.SettingReadonly = false;
		this.tab_Remark.Size = new System.Drawing.Size(819, 422);
		this.tab_Remark.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Remark.Text = "Remark";
		this.panelControl9.Controls.Add(this.btn_PicDelete);
		this.panelControl9.Controls.Add(this.btBrower);
		this.panelControl9.Controls.Add(this.txtUserText);
		this.panelControl9.Controls.Add(this.picRemarks);
		this.panelControl9.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl9.Location = new System.Drawing.Point(0, 0);
		this.panelControl9.Name = "panelControl9";
		this.panelControl9.Size = new System.Drawing.Size(819, 422);
		this.panelControl9.TabIndex = 0;
		this.btn_PicDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_PicDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_PicDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_PicDelete.EditControl = null;
		this.btn_PicDelete.EditValue = "";
		this.btn_PicDelete.FieldName = null;
		this.btn_PicDelete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PicDelete.ImageOptions.Image");
		this.btn_PicDelete.IsSearchControl = false;
		this.btn_PicDelete.Location = new System.Drawing.Point(284, 378);
		this.btn_PicDelete.Name = "btn_PicDelete";
		this.btn_PicDelete.ReadOnly = false;
		this.btn_PicDelete.SettingInVisible = false;
		this.btn_PicDelete.SettingReadonly = false;
		this.btn_PicDelete.Size = new System.Drawing.Size(23, 25);
		this.btn_PicDelete.TabIndex = 3;
		this.btn_PicDelete.Click += new System.EventHandler(this.btn_PicDelete_Click);
		this.btBrower.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btBrower.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btBrower.EditControl = null;
		this.btBrower.EditValue = "...";
		this.btBrower.FieldName = null;
		this.btBrower.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btBrower.ImageOptions.Image");
		this.btBrower.IsSearchControl = false;
		this.btBrower.Location = new System.Drawing.Point(284, 347);
		this.btBrower.Name = "btBrower";
		this.btBrower.ReadOnly = false;
		this.btBrower.SettingInVisible = false;
		this.btBrower.SettingReadonly = false;
		this.btBrower.Size = new System.Drawing.Size(23, 25);
		this.btBrower.TabIndex = 2;
		this.btBrower.Text = "...";
		this.btBrower.Click += new System.EventHandler(this.btBrower_Click);
		this.txtUserText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtUserText.EditControl = null;
		this.txtUserText.FieldName = "UserText";
		this.txtUserText.IsSearchControl = false;
		this.txtUserText.Location = new System.Drawing.Point(281, 5);
		this.txtUserText.Name = "txtUserText";
		this.txtUserText.SettingInVisible = false;
		this.txtUserText.SettingReadonly = false;
		this.txtUserText.Size = new System.Drawing.Size(533, 336);
		this.txtUserText.TabIndex = 1;
		this.picRemarks.Cursor = System.Windows.Forms.Cursors.Default;
		this.picRemarks.Dock = System.Windows.Forms.DockStyle.Left;
		this.picRemarks.Location = new System.Drawing.Point(2, 2);
		this.picRemarks.Name = "picRemarks";
		this.picRemarks.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
		this.picRemarks.Size = new System.Drawing.Size(276, 418);
		this.picRemarks.TabIndex = 0;
		this.picRemarks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRemarks_MouseDown);
		this.picRemarks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picRemarks_MouseUp);
		this.tab_Attachments.Controls.Add(this.ucAttachment1);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = "tab_Attachments";
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = true;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(819, 422);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oItem";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(819, 422);
		this.ucAttachment1.TabIndex = 0;
		this.tab_UserDefineF.Controls.Add(this.grd_Udf);
		this.tab_UserDefineF.EditControl = null;
		this.tab_UserDefineF.EditValue = "User Defined Fields";
		this.tab_UserDefineF.FieldName = "tab_UserDefineF";
		this.tab_UserDefineF.IsSearchControl = false;
		this.tab_UserDefineF.Name = "tab_UserDefineF";
		this.tab_UserDefineF.ReadOnly = true;
		this.tab_UserDefineF.SettingInVisible = false;
		this.tab_UserDefineF.SettingReadonly = false;
		this.tab_UserDefineF.Size = new System.Drawing.Size(819, 422);
		this.tab_UserDefineF.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_UserDefineF.Text = "User Defined Fields";
		this.grd_Udf.AllowPasteExcel = false;
		this.grd_Udf.ClipboardData = null;
		this.grd_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Udf.Location = new System.Drawing.Point(0, 0);
		this.grd_Udf.MainView = this.gv_Udf;
		this.grd_Udf.Name = "grd_Udf";
		this.grd_Udf.Size = new System.Drawing.Size(819, 422);
		this.grd_Udf.TabIndex = 0;
		this.grd_Udf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Udf });
		this.gv_Udf.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Udf.ColumnKey = null;
		this.gv_Udf.DisableFormSetting = false;
		this.gv_Udf.DisplayException = false;
		this.gv_Udf.EditColumn = null;
		this.gv_Udf.FieldCurrency = null;
		this.gv_Udf.FieldRate = null;
		this.gv_Udf.FormId = "oItem";
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
		this.gv_Udf.OptionsView.ColumnAutoWidth = false;
		this.gv_Udf.OptionsView.ShowGroupPanel = false;
		this.gv_Udf.UseBatchMod = false;
		this.gv_Udf.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Udf_CellValueChanged);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btCancel.Appearance.Options.UseBackColor = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = "BtnCancel";
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(82, 581);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 17;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btOK.Appearance.Options.UseBackColor = true;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = "BtnSave";
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(5, 581);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 16;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.cb_Code.EditControl = null;
		this.cb_Code.FieldName = "Series";
		this.cb_Code.IsSearchControl = false;
		this.cb_Code.Location = new System.Drawing.Point(85, 9);
		this.cb_Code.Name = "cb_Code";
		this.cb_Code.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_Code.Properties.Appearance.Options.UseFont = true;
		this.cb_Code.Properties.AutoHeight = false;
		this.cb_Code.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Code.Properties.IsSearchControl = false;
		this.cb_Code.Properties.NullText = "";
		this.cb_Code.SettingInVisible = false;
		this.cb_Code.SettingReadonly = false;
		this.cb_Code.Size = new System.Drawing.Size(62, 18);
		this.cb_Code.TabIndex = 1;
		this.cb_Code.Tag = "OITM,Series";
		this.cb_Code.EditValueChanged += new System.EventHandler(this.cb_Code_EditValueChanged);
		this.luk_UoMGroup_UgpEntry.EditControl = null;
		this.luk_UoMGroup_UgpEntry.FieldName = "UgpEntry";
		this.luk_UoMGroup_UgpEntry.IsSearchControl = false;
		this.luk_UoMGroup_UgpEntry.Location = new System.Drawing.Point(405, 69);
		this.luk_UoMGroup_UgpEntry.Name = "luk_UoMGroup_UgpEntry";
		this.luk_UoMGroup_UgpEntry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_UoMGroup_UgpEntry.Properties.Appearance.Options.UseFont = true;
		this.luk_UoMGroup_UgpEntry.Properties.AutoHeight = false;
		this.luk_UoMGroup_UgpEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_UoMGroup_UgpEntry.Properties.IsSearchControl = false;
		this.luk_UoMGroup_UgpEntry.Properties.NullText = "";
		this.luk_UoMGroup_UgpEntry.SettingInVisible = false;
		this.luk_UoMGroup_UgpEntry.SettingReadonly = false;
		this.luk_UoMGroup_UgpEntry.Size = new System.Drawing.Size(116, 18);
		this.luk_UoMGroup_UgpEntry.TabIndex = 7;
		this.luk_UoMGroup_UgpEntry.Tag = "OITM,UgpEntry";
		this.luk_UoMGroup_UgpEntry.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_UoMGroup_UgpEntry_EditValueChanging);
		this.labelLine2.EditControl = this.luk_UoMGroup_UgpEntry;
		this.labelLine2.EditValue = "UoM Group";
		this.labelLine2.FieldName = "labelLine2";
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(307, 70);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(104, 21);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.TabStop = false;
		this.labelLine2.Utext = "UoM Group";
		this.lbl_PriceListUoM.EditControl = null;
		this.lbl_PriceListUoM.EditValue = "1";
		this.lbl_PriceListUoM.FieldName = null;
		this.lbl_PriceListUoM.HideWhenEmpty = false;
		this.lbl_PriceListUoM.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PriceListUoM.Location = new System.Drawing.Point(635, 107);
		this.lbl_PriceListUoM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_PriceListUoM.Name = "lbl_PriceListUoM";
		this.lbl_PriceListUoM.ReadOnly = false;
		this.lbl_PriceListUoM.SettingInVisible = false;
		this.lbl_PriceListUoM.SettingReadonly = false;
		this.lbl_PriceListUoM.ShowUnderLine = true;
		this.lbl_PriceListUoM.Size = new System.Drawing.Size(63, 20);
		this.lbl_PriceListUoM.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.SourceValue;
		this.lbl_PriceListUoM.TabIndex = 0;
		this.lbl_PriceListUoM.TabStop = false;
		this.lbl_PriceListUoM.Utext = "1";
		this.lbl_PriceListUoM.Visible = false;
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.removeToolStripMenuItem, this.duplicateToolStripMenuItem, this.toolStripSeparator1, this.businessPartnerCatalogNumbersToolStripMenuItem, this.billOfMaterialsToolStripMenuItem, this.inventoryPostingListToolStripMenuItem, this.inventoryAuditReportToolStripMenuItem, this.avaiableToPromiseToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(257, 186);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(253, 6);
		this.businessPartnerCatalogNumbersToolStripMenuItem.Name = "businessPartnerCatalogNumbersToolStripMenuItem";
		this.businessPartnerCatalogNumbersToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.businessPartnerCatalogNumbersToolStripMenuItem.Text = "Business Partner Catalog Numbers";
		this.billOfMaterialsToolStripMenuItem.Name = "billOfMaterialsToolStripMenuItem";
		this.billOfMaterialsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.billOfMaterialsToolStripMenuItem.Text = "Bill of Materials";
		this.billOfMaterialsToolStripMenuItem.Click += new System.EventHandler(this.billOfMaterialsToolStripMenuItem_Click);
		this.inventoryPostingListToolStripMenuItem.Name = "inventoryPostingListToolStripMenuItem";
		this.inventoryPostingListToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.inventoryPostingListToolStripMenuItem.Text = "Inventory Posting List";
		this.inventoryPostingListToolStripMenuItem.Click += new System.EventHandler(this.inventoryPostingListToolStripMenuItem_Click);
		this.inventoryAuditReportToolStripMenuItem.Name = "inventoryAuditReportToolStripMenuItem";
		this.inventoryAuditReportToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.inventoryAuditReportToolStripMenuItem.Text = "Inventory Audit Report";
		this.inventoryAuditReportToolStripMenuItem.Visible = false;
		this.avaiableToPromiseToolStripMenuItem.Name = "avaiableToPromiseToolStripMenuItem";
		this.avaiableToPromiseToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
		this.avaiableToPromiseToolStripMenuItem.Text = "Avaiable to Promise";
		this.txtUnitPrice.CheckExChangeRate = false;
		this.txtUnitPrice.Currency = null;
		this.txtUnitPrice.EditControl = null;
		this.txtUnitPrice.FieldName = null;
		this.txtUnitPrice.IsSearchControl = false;
		this.txtUnitPrice.Location = new System.Drawing.Point(533, 106);
		this.txtUnitPrice.Name = "txtUnitPrice";
		this.txtUnitPrice.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtUnitPrice.Properties.Appearance.Options.UseFont = true;
		this.txtUnitPrice.Properties.AutoHeight = false;
		this.txtUnitPrice.SettingInVisible = false;
		this.txtUnitPrice.SettingReadonly = false;
		this.txtUnitPrice.Size = new System.Drawing.Size(100, 18);
		this.txtUnitPrice.TabIndex = 14;
		this.txtUnitPrice.Validated += new System.EventHandler(this.txtUnitPrice_Validated);
		base.AcceptButton = this.btOK;
		base.Appearance.BackColor = System.Drawing.SystemColors.Control;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(832, 607);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.txtUnitPrice);
		base.Controls.Add(this.lbl_PriceListUoM);
		base.Controls.Add(this.luk_UoMGroup_UgpEntry);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.cb_PriceType);
		base.Controls.Add(this.cbPriceList);
		base.Controls.Add(this.cbItemGroup);
		base.Controls.Add(this.cbItemType);
		base.Controls.Add(this.cb_Code);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.tabControl_Detail);
		base.Controls.Add(this.lb_ItemCode);
		base.Controls.Add(this.chkSellItem);
		base.Controls.Add(this.chkPrchseItem);
		base.Controls.Add(this.txtItemCode);
		base.Controls.Add(this.chkInvntItem);
		base.Controls.Add(this.lb_Description);
		base.Controls.Add(this.lb_PriceList);
		base.Controls.Add(this.lb_DescForLang);
		base.Controls.Add(this.lb_UnitPrice);
		base.Controls.Add(this.lb_ItemGroup);
		base.Controls.Add(this.txtFrgnName);
		base.Controls.Add(this.lb_ItemType);
		base.Controls.Add(this.txtDescription);
		base.Controls.Add(this.lb_BarCode);
		base.Controls.Add(this.txtBarCode);
		base.FormId = "oItem";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ItemMasterData.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemMasterData";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Item Master Data";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_ItemMasterData_OnControlValueChanged);
		base.OnControlValueChanging += new Apzon.Controls.Form.ControlValueChanging(this.frm_ItemMasterData_OnControlValueChanging);
		((System.ComponentModel.ISupportInitialize)this.chkSellItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkPrchseItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkInvntItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbPriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_PriceType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFrgnName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescription.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Detail).EndInit();
		this.tabControl_Detail.ResumeLayout(false);
		this.tab_General.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_OneBOneRec.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssuePriBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ManOutOnly.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WarrntTmpl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NoDiscount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbManagementMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbManageItemBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbShippingType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbManufacturer.Properties).EndInit();
		this.panel_Advanced.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenTo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrozenFrom2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidFrom2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateValidTo2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFrozenComm2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtValidComm2.Properties).EndInit();
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
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).EndInit();
		this.tab_Purchase.ResumeLayout(false);
		this.tab_Purchase.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_BWeight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BHeight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BWidth1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BLength1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_CustomsGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CstGrpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBuyUnitMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PUoMEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BVolUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BVolume.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatPercentPurchase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbVatGroupPu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurPackUn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumInBuy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurPackMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSuppCatNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor4.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPurFactor1.Properties).EndInit();
		this.tab_Sales.ResumeLayout(false);
		this.tab_Sales.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_SWeight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SHeight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SWidth1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SLength1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalUnitMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SUoMEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SVolUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_SVolume.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatPercentSales.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbVatGourpSa.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalPackUn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumInSale.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalPackMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor4.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalFactor1.Properties).EndInit();
		this.tab_Inventory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calc_IWeight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInvntUom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_INUoMEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_IUoMEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NumInCnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CntUnitMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbEvalSystem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtAvgPrice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtMaxLevel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtMinLevel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReorderQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbGLMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkByWh.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WareHouse).EndInit();
		this.ctmenu_Warehouse.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_WareHouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rChk_Lock).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CalcInventory_Price).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CalcInventory_Quantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DefaultBinLocation).EndInit();
		this.tab_PlanData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cb_OrderInterval.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ComponentWarehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ProcurementMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_PlanningMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToleranceDays.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LeadTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MinimumOrderQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OrderMultiple.Properties).EndInit();
		this.tab_Production.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calc_PrdStdCst.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TreeType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InCostRoll.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfRes.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssueMthd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Phantom.Properties).EndInit();
		this.tab_Properties.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PropertiesCheck).EndInit();
		this.tab_Remark.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl9).EndInit();
		this.panelControl9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtUserText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.picRemarks.Properties).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		this.tab_UserDefineF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UoMGroup_UgpEntry.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtUnitPrice.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
