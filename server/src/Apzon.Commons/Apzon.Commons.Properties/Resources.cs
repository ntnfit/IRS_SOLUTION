using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Apzon.Commons.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Apzon.Commons.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string ButtonActivities => ResourceManager.GetString("ButtonActivities", resourceCulture);

	public static string ButtonAdd => ResourceManager.GetString("ButtonAdd", resourceCulture);

	public static string ButtonAssignTo => ResourceManager.GetString("ButtonAssignTo", resourceCulture);

	public static string ButtonBack => ResourceManager.GetString("ButtonBack", resourceCulture);

	public static string ButtonBrowse => ResourceManager.GetString("ButtonBrowse", resourceCulture);

	public static string ButtonCancel => ResourceManager.GetString("ButtonCancel", resourceCulture);

	public static string ButtonCheckCapacity => ResourceManager.GetString("ButtonCheckCapacity", resourceCulture);

	public static string ButtonCheckConnect => ResourceManager.GetString("ButtonCheckConnect", resourceCulture);

	public static string ButtonChoose => ResourceManager.GetString("ButtonChoose", resourceCulture);

	public static string ButtonClose => ResourceManager.GetString("ButtonClose", resourceCulture);

	public static string ButtonCopyFrom => ResourceManager.GetString("ButtonCopyFrom", resourceCulture);

	public static string ButtonCopyTo => ResourceManager.GetString("ButtonCopyTo", resourceCulture);

	public static string ButtonCreatePurchaseRequest => ResourceManager.GetString("ButtonCreatePurchaseRequest", resourceCulture);

	public static string ButtonCreateTransferRequest => ResourceManager.GetString("ButtonCreateTransferRequest", resourceCulture);

	public static string ButtonDelete => ResourceManager.GetString("ButtonDelete", resourceCulture);

	public static string ButtonDisplay => ResourceManager.GetString("ButtonDisplay", resourceCulture);

	public static string ButtonExpandCollapseAll => ResourceManager.GetString("ButtonExpandCollapseAll", resourceCulture);

	public static string ButtonFind => ResourceManager.GetString("ButtonFind", resourceCulture);

	public static string ButtonFollowUp => ResourceManager.GetString("ButtonFollowUp", resourceCulture);

	public static string ButtonLoad => ResourceManager.GetString("ButtonLoad", resourceCulture);

	public static string ButtonMOPlanning => ResourceManager.GetString("ButtonMOPlanning", resourceCulture);

	public static string ButtonNewAssignment => ResourceManager.GetString("ButtonNewAssignment", resourceCulture);

	public static string ButtonOK => ResourceManager.GetString("ButtonOK", resourceCulture);

	public static string ButtonPOScheduling => ResourceManager.GetString("ButtonPOScheduling", resourceCulture);

	public static string ButtonProductionOrder => ResourceManager.GetString("ButtonProductionOrder", resourceCulture);

	public static string ButtonRefresh => ResourceManager.GetString("ButtonRefresh", resourceCulture);

	public static string ButtonRelatedDocuments => ResourceManager.GetString("ButtonRelatedDocuments", resourceCulture);

	public static string ButtonReturnComponents => ResourceManager.GetString("ButtonReturnComponents", resourceCulture);

	public static string ButtonSave => ResourceManager.GetString("ButtonSave", resourceCulture);

	public static string ButtonSearch => ResourceManager.GetString("ButtonSearch", resourceCulture);

	public static string ButtonSetAsDefault => ResourceManager.GetString("ButtonSetAsDefault", resourceCulture);

	public static string ButtonSetting => ResourceManager.GetString("ButtonSetting", resourceCulture);

	public static string ButtonUpdate => ResourceManager.GetString("ButtonUpdate", resourceCulture);

	public static string ButtonUpload => ResourceManager.GetString("ButtonUpload", resourceCulture);

	public static string CDC1_Discount => ResourceManager.GetString("CDC1_Discount", resourceCulture);

	public static string CDC1_LineId => ResourceManager.GetString("CDC1_LineId", resourceCulture);

	public static string CDC1_NumOfDays => ResourceManager.GetString("CDC1_NumOfDays", resourceCulture);

	public static string CFAPInvoice => ResourceManager.GetString("CFAPInvoice", resourceCulture);

	public static string CFBlanketAgreement => ResourceManager.GetString("CFBlanketAgreement", resourceCulture);

	public static string CFGoodRPO => ResourceManager.GetString("CFGoodRPO", resourceCulture);

	public static string CFOrders => ResourceManager.GetString("CFOrders", resourceCulture);

	public static string CFQuotation => ResourceManager.GetString("CFQuotation", resourceCulture);

	public static string CFReturn => ResourceManager.GetString("CFReturn", resourceCulture);

	public static string CTAPCreditMemo => ResourceManager.GetString("CTAPCreditMemo", resourceCulture);

	public static string ctm_Choose => ResourceManager.GetString("ctm_Choose", resourceCulture);

	public static string ctm_ViewDetail => ResourceManager.GetString("ctm_ViewDetail", resourceCulture);

	public static string ctx_Add => ResourceManager.GetString("ctx_Add", resourceCulture);

	public static string ctx_AddRow => ResourceManager.GetString("ctx_AddRow", resourceCulture);

	public static string ctx_Cancel => ResourceManager.GetString("ctx_Cancel", resourceCulture);

	public static string ctx_Close => ResourceManager.GetString("ctx_Close", resourceCulture);

	public static string ctx_CloseRow => ResourceManager.GetString("ctx_CloseRow", resourceCulture);

	public static string ctx_Delete => ResourceManager.GetString("ctx_Delete", resourceCulture);

	public static string ctx_DeleteRow => ResourceManager.GetString("ctx_DeleteRow", resourceCulture);

	public static string ctx_Duplicate => ResourceManager.GetString("ctx_Duplicate", resourceCulture);

	public static string ctx_DuplicateRow => ResourceManager.GetString("ctx_DuplicateRow", resourceCulture);

	public static string ctx_InsertRowBelow => ResourceManager.GetString("ctx_InsertRowBelow", resourceCulture);

	public static string ctx_Remove => ResourceManager.GetString("ctx_Remove", resourceCulture);

	public static string ctx_SaveAsDraft => ResourceManager.GetString("ctx_SaveAsDraft", resourceCulture);

	public static string Doc_gvDoc_CreateDate => ResourceManager.GetString("Doc_gvDoc_CreateDate", resourceCulture);

	public static string Doc_gvDoc_DocDueDate => ResourceManager.GetString("Doc_gvDoc_DocDueDate", resourceCulture);

	public static string Doc_gvDoc_DocStatus => ResourceManager.GetString("Doc_gvDoc_DocStatus", resourceCulture);

	public static string Doc_gvDoc_gcAllVatSum => ResourceManager.GetString("Doc_gvDoc_gcAllVatSum", resourceCulture);

	public static string Doc_gvDoc_gcBPCode => ResourceManager.GetString("Doc_gvDoc_gcBPCode", resourceCulture);

	public static string Doc_gvDoc_gcBranch => ResourceManager.GetString("Doc_gvDoc_gcBranch", resourceCulture);

	public static string Doc_gvDoc_gcContactPersonCode => ResourceManager.GetString("Doc_gvDoc_gcContactPersonCode", resourceCulture);

	public static string Doc_gvDoc_gcCurrencyType => ResourceManager.GetString("Doc_gvDoc_gcCurrencyType", resourceCulture);

	public static string Doc_gvDoc_gcDeliveyDate => ResourceManager.GetString("Doc_gvDoc_gcDeliveyDate", resourceCulture);

	public static string Doc_gvDoc_gcDepartment => ResourceManager.GetString("Doc_gvDoc_gcDepartment", resourceCulture);

	public static string Doc_gvDoc_gcDocAllTotal => ResourceManager.GetString("Doc_gvDoc_gcDocAllTotal", resourceCulture);

	public static string Doc_gvDoc_gcDocAllVatSum => ResourceManager.GetString("Doc_gvDoc_gcDocAllVatSum", resourceCulture);

	public static string Doc_gvDoc_gcDocAtcEntry => ResourceManager.GetString("Doc_gvDoc_gcDocAtcEntry", resourceCulture);

	public static string Doc_gvDoc_gcDocCurrency => ResourceManager.GetString("Doc_gvDoc_gcDocCurrency", resourceCulture);

	public static string Doc_gvDoc_gcDocDis => ResourceManager.GetString("Doc_gvDoc_gcDocDis", resourceCulture);

	public static string Doc_gvDoc_gcDocDueDate => ResourceManager.GetString("Doc_gvDoc_gcDocDueDate", resourceCulture);

	public static string Doc_gvDoc_gcDocNum => ResourceManager.GetString("Doc_gvDoc_gcDocNum", resourceCulture);

	public static string Doc_gvDoc_gcDocRate => ResourceManager.GetString("Doc_gvDoc_gcDocRate", resourceCulture);

	public static string Doc_gvDoc_gcDocTotal => ResourceManager.GetString("Doc_gvDoc_gcDocTotal", resourceCulture);

	public static string Doc_gvDoc_gcDocTotalFc => ResourceManager.GetString("Doc_gvDoc_gcDocTotalFc", resourceCulture);

	public static string Doc_gvDoc_gcDocTotalTax => ResourceManager.GetString("Doc_gvDoc_gcDocTotalTax", resourceCulture);

	public static string Doc_gvDoc_gcDocVatSumFc => ResourceManager.GetString("Doc_gvDoc_gcDocVatSumFc", resourceCulture);

	public static string Doc_gvDoc_gcPostingDate => ResourceManager.GetString("Doc_gvDoc_gcPostingDate", resourceCulture);

	public static string Doc_gvDoc_gcReqTypeName => ResourceManager.GetString("Doc_gvDoc_gcReqTypeName", resourceCulture);

	public static string Doc_gvDoc_gcRequester => ResourceManager.GetString("Doc_gvDoc_gcRequester", resourceCulture);

	public static string Doc_gvDoc_gcTaxDate => ResourceManager.GetString("Doc_gvDoc_gcTaxDate", resourceCulture);

	public static string Doc_gvDoc_gcTotalBeforeDis => ResourceManager.GetString("Doc_gvDoc_gcTotalBeforeDis", resourceCulture);

	public static string Doc_gvDoc_gcValidUntil => ResourceManager.GetString("Doc_gvDoc_gcValidUntil", resourceCulture);

	public static string Doc_gvDoc_gcVatSum => ResourceManager.GetString("Doc_gvDoc_gcVatSum", resourceCulture);

	public static string Doc_gvDoc_gcVatSumFC => ResourceManager.GetString("Doc_gvDoc_gcVatSumFC", resourceCulture);

	public static string Doc_gvDoc_Remark => ResourceManager.GetString("Doc_gvDoc_Remark", resourceCulture);

	public static string Doc_gvDoc_ReqName => ResourceManager.GetString("Doc_gvDoc_ReqName", resourceCulture);

	public static string Doc_gvDoc_Requester => ResourceManager.GetString("Doc_gvDoc_Requester", resourceCulture);

	public static string Doc_gvLogistic_gcAddress => ResourceManager.GetString("Doc_gvLogistic_gcAddress", resourceCulture);

	public static string Doc_gvLogistic_gcAddress2 => ResourceManager.GetString("Doc_gvLogistic_gcAddress2", resourceCulture);

	public static string Doc_gvLogistic_gcBillTo => ResourceManager.GetString("Doc_gvLogistic_gcBillTo", resourceCulture);

	public static string Doc_gvLogistic_gcConfirmed => ResourceManager.GetString("Doc_gvLogistic_gcConfirmed", resourceCulture);

	public static string Doc_gvLogistic_gcShipTo => ResourceManager.GetString("Doc_gvLogistic_gcShipTo", resourceCulture);

	public static string Doc_gvLogistic_gcShipType => ResourceManager.GetString("Doc_gvLogistic_gcShipType", resourceCulture);

	public static string Doc_lb_BusinessPartner => ResourceManager.GetString("Doc_lb_BusinessPartner", resourceCulture);

	public static string Doc_lb_DocStatus => ResourceManager.GetString("Doc_lb_DocStatus", resourceCulture);

	public static string Doc_lb_FromDate => ResourceManager.GetString("Doc_lb_FromDate", resourceCulture);

	public static string Doc_lb_NoOfTrans => ResourceManager.GetString("Doc_lb_NoOfTrans", resourceCulture);

	public static string Doc_lb_Requester => ResourceManager.GetString("Doc_lb_Requester", resourceCulture);

	public static string Doc_lb_ToDate => ResourceManager.GetString("Doc_lb_ToDate", resourceCulture);

	public static string DocPur_gvDoc_gcCardCode => ResourceManager.GetString("DocPur_gvDoc_gcCardCode", resourceCulture);

	public static string DocPur_gvDoc_gcCardName => ResourceManager.GetString("DocPur_gvDoc_gcCardName", resourceCulture);

	public static string DocPur_gvDoc_gcDocTotal => ResourceManager.GetString("DocPur_gvDoc_gcDocTotal", resourceCulture);

	public static string DocPur_gvDoc_gcRequriedDate => ResourceManager.GetString("DocPur_gvDoc_gcRequriedDate", resourceCulture);

	public static string DocSales_gvDoc_gcCardCode => ResourceManager.GetString("DocSales_gvDoc_gcCardCode", resourceCulture);

	public static string DocSales_gvDoc_gcCardName => ResourceManager.GetString("DocSales_gvDoc_gcCardName", resourceCulture);

	public static string frmAtp_tlc_Available => ResourceManager.GetString("frmAtp_tlc_Available", resourceCulture);

	public static string frmAtp_tlc_CardCode => ResourceManager.GetString("frmAtp_tlc_CardCode", resourceCulture);

	public static string frmAtp_tlc_DelDate => ResourceManager.GetString("frmAtp_tlc_DelDate", resourceCulture);

	public static string frmAtp_tlc_Document => ResourceManager.GetString("frmAtp_tlc_Document", resourceCulture);

	public static string frmAtp_tlc_ItemCode => ResourceManager.GetString("frmAtp_tlc_ItemCode", resourceCulture);

	public static string frmAtp_tlc_ItemDescription => ResourceManager.GetString("frmAtp_tlc_ItemDescription", resourceCulture);

	public static string frmAtp_tlc_OnCommitted => ResourceManager.GetString("frmAtp_tlc_OnCommitted", resourceCulture);

	public static string frmAtp_tlc_Orderd => ResourceManager.GetString("frmAtp_tlc_Orderd", resourceCulture);

	public static string frmAtp_tlc_OrderDate => ResourceManager.GetString("frmAtp_tlc_OrderDate", resourceCulture);

	public static string grc_GeneralMOToPlanning => ResourceManager.GetString("grc_GeneralMOToPlanning", resourceCulture);

	public static string grc_ResourceLoad => ResourceManager.GetString("grc_ResourceLoad", resourceCulture);

	public static string ITM1_Price => ResourceManager.GetString("ITM1_Price", resourceCulture);

	public static string ITM1_PriceList => ResourceManager.GetString("ITM1_PriceList", resourceCulture);

	public static string lb_Active => ResourceManager.GetString("lb_Active", resourceCulture);

	public static string lb_ActiveFrom => ResourceManager.GetString("lb_ActiveFrom", resourceCulture);

	public static string lb_ActiveTo => ResourceManager.GetString("lb_ActiveTo", resourceCulture);

	public static string lb_Activity => ResourceManager.GetString("lb_Activity", resourceCulture);

	public static string lb_ActualByProductCost => ResourceManager.GetString("lb_ActualByProductCost", resourceCulture);

	public static string lb_ActualClosingDate => ResourceManager.GetString("lb_ActualClosingDate", resourceCulture);

	public static string lb_ActualItemComponentCost => ResourceManager.GetString("lb_ActualItemComponentCost", resourceCulture);

	public static string lb_ActualProductCost => ResourceManager.GetString("lb_ActualProductCost", resourceCulture);

	public static string lb_ActualResourceComponentCost => ResourceManager.GetString("lb_ActualResourceComponentCost", resourceCulture);

	public static string lb_AddNew => ResourceManager.GetString("lb_AddNew", resourceCulture);

	public static string lb_Address => ResourceManager.GetString("lb_Address", resourceCulture);

	public static string lb_After => ResourceManager.GetString("lb_After", resourceCulture);

	public static string lb_AlternativeOperation => ResourceManager.GetString("lb_AlternativeOperation", resourceCulture);

	public static string lb_Assigned => ResourceManager.GetString("lb_Assigned", resourceCulture);

	public static string lb_AssignedBy => ResourceManager.GetString("lb_AssignedBy", resourceCulture);

	public static string lb_AssignedTo => ResourceManager.GetString("lb_AssignedTo", resourceCulture);

	public static string lb_AtlAdditionalCost => ResourceManager.GetString("lb_AtlAdditionalCost", resourceCulture);

	public static string lb_AttachmentDate => ResourceManager.GetString("lb_AttachmentDate", resourceCulture);

	public static string lb_AttFile => ResourceManager.GetString("lb_AttFile", resourceCulture);

	public static string lb_AttFullPath => ResourceManager.GetString("lb_AttFullPath", resourceCulture);

	public static string lb_Balance => ResourceManager.GetString("lb_Balance", resourceCulture);

	public static string lb_BankCode => ResourceManager.GetString("lb_BankCode", resourceCulture);

	public static string lb_BankName => ResourceManager.GetString("lb_BankName", resourceCulture);

	public static string lb_BarCode => ResourceManager.GetString("lb_BarCode", resourceCulture);

	public static string lb_BaseQty => ResourceManager.GetString("lb_BaseQty", resourceCulture);

	public static string lb_BaseUoM => ResourceManager.GetString("lb_BaseUoM", resourceCulture);

	public static string lb_BatchComplete => ResourceManager.GetString("lb_BatchComplete", resourceCulture);

	public static string lb_BatchToIssue => ResourceManager.GetString("lb_BatchToIssue", resourceCulture);

	public static string lb_Blocks => ResourceManager.GetString("lb_Blocks", resourceCulture);

	public static string lb_BOM => ResourceManager.GetString("lb_BOM", resourceCulture);

	public static string lb_BomType => ResourceManager.GetString("lb_BomType", resourceCulture);

	public static string lb_BPCatalogNo => ResourceManager.GetString("lb_BPCatalogNo", resourceCulture);

	public static string lb_BPCode => ResourceManager.GetString("lb_BPCode", resourceCulture);

	public static string lb_BPCodeAndNameNotNull => ResourceManager.GetString("lb_BPCodeAndNameNotNull", resourceCulture);

	public static string lb_BPName => ResourceManager.GetString("lb_BPName", resourceCulture);

	public static string lb_BPType => ResourceManager.GetString("lb_BPType", resourceCulture);

	public static string lb_Branch => ResourceManager.GetString("lb_Branch", resourceCulture);

	public static string lb_BuildingFloorRoom => ResourceManager.GetString("lb_BuildingFloorRoom", resourceCulture);

	public static string lb_by => ResourceManager.GetString("lb_by", resourceCulture);

	public static string lb_ByProduc => ResourceManager.GetString("lb_ByProduc", resourceCulture);

	public static string lb_Calendar => ResourceManager.GetString("lb_Calendar", resourceCulture);

	public static string lb_Canceled => ResourceManager.GetString("lb_Canceled", resourceCulture);

	public static string lb_CardCode => ResourceManager.GetString("lb_CardCode", resourceCulture);

	public static string lb_CardName => ResourceManager.GetString("lb_CardName", resourceCulture);

	public static string lb_Checked => ResourceManager.GetString("lb_Checked", resourceCulture);

	public static string lb_City => ResourceManager.GetString("lb_City", resourceCulture);

	public static string lb_Closed => ResourceManager.GetString("lb_Closed", resourceCulture);

	public static string lb_ClosePercent => ResourceManager.GetString("lb_ClosePercent", resourceCulture);

	public static string lb_ClosePrcnt => ResourceManager.GetString("lb_ClosePrcnt", resourceCulture);

	public static string lb_Closing => ResourceManager.GetString("lb_Closing", resourceCulture);

	public static string lb_ClosingDate => ResourceManager.GetString("lb_ClosingDate", resourceCulture);

	public static string lb_CntctCode => ResourceManager.GetString("lb_CntctCode", resourceCulture);

	public static string lb_Code => ResourceManager.GetString("lb_Code", resourceCulture);

	public static string lb_Comment => ResourceManager.GetString("lb_Comment", resourceCulture);

	public static string lb_Commission => ResourceManager.GetString("lb_Commission", resourceCulture);

	public static string lb_Completed => ResourceManager.GetString("lb_Completed", resourceCulture);

	public static string lb_CompletedQuantity => ResourceManager.GetString("lb_CompletedQuantity", resourceCulture);

	public static string lb_ComponentCode => ResourceManager.GetString("lb_ComponentCode", resourceCulture);

	public static string lb_ComponentName => ResourceManager.GetString("lb_ComponentName", resourceCulture);

	public static string lb_Confirmations => ResourceManager.GetString("lb_Confirmations", resourceCulture);

	public static string lb_ControlKey => ResourceManager.GetString("lb_ControlKey", resourceCulture);

	public static string lb_CostCenter => ResourceManager.GetString("lb_CostCenter", resourceCulture);

	public static string lb_CostExpenseAct => ResourceManager.GetString("lb_CostExpenseAct", resourceCulture);

	public static string lb_Costs => ResourceManager.GetString("lb_Costs", resourceCulture);

	public static string lb_Count => ResourceManager.GetString("lb_Count", resourceCulture);

	public static string lb_Country => ResourceManager.GetString("lb_Country", resourceCulture);

	public static string lb_CtrlArea => ResourceManager.GetString("lb_CtrlArea", resourceCulture);

	public static string lb_CtrlKey => ResourceManager.GetString("lb_CtrlKey", resourceCulture);

	public static string lb_CurrencyRate => ResourceManager.GetString("lb_CurrencyRate", resourceCulture);

	public static string lb_Customer => ResourceManager.GetString("lb_Customer", resourceCulture);

	public static string lb_CustomerCode => ResourceManager.GetString("lb_CustomerCode", resourceCulture);

	public static string lb_CustomerName => ResourceManager.GetString("lb_CustomerName", resourceCulture);

	public static string lb_CustomerVendorCatNo => ResourceManager.GetString("lb_CustomerVendorCatNo", resourceCulture);

	public static string lb_Dates => ResourceManager.GetString("lb_Dates", resourceCulture);

	public static string lb_DefaultCost => ResourceManager.GetString("lb_DefaultCost", resourceCulture);

	public static string lb_DefaultRelationship => ResourceManager.GetString("lb_DefaultRelationship", resourceCulture);

	public static string lb_DeliveryDate => ResourceManager.GetString("lb_DeliveryDate", resourceCulture);

	public static string lb_Department => ResourceManager.GetString("lb_Department", resourceCulture);

	public static string lb_Description => ResourceManager.GetString("lb_Description", resourceCulture);

	public static string lb_Detail => ResourceManager.GetString("lb_Detail", resourceCulture);

	public static string lb_DiscountPersentage => ResourceManager.GetString("lb_DiscountPersentage", resourceCulture);

	public static string lb_DistrRule => ResourceManager.GetString("lb_DistrRule", resourceCulture);

	public static string lb_DistRule => ResourceManager.GetString("lb_DistRule", resourceCulture);

	public static string lb_DistRule1 => ResourceManager.GetString("lb_DistRule1", resourceCulture);

	public static string lb_DistRule2 => ResourceManager.GetString("lb_DistRule2", resourceCulture);

	public static string lb_DistRule3 => ResourceManager.GetString("lb_DistRule3", resourceCulture);

	public static string lb_DistRule4 => ResourceManager.GetString("lb_DistRule4", resourceCulture);

	public static string lb_DistRule5 => ResourceManager.GetString("lb_DistRule5", resourceCulture);

	public static string lb_DocumentDate => ResourceManager.GetString("lb_DocumentDate", resourceCulture);

	public static string lb_DocumentNo => ResourceManager.GetString("lb_DocumentNo", resourceCulture);

	public static string lb_DocumentNumber => ResourceManager.GetString("lb_DocumentNumber", resourceCulture);

	public static string lb_DocumentType => ResourceManager.GetString("lb_DocumentType", resourceCulture);

	public static string lb_Download => ResourceManager.GetString("lb_Download", resourceCulture);

	public static string lb_DueDate => ResourceManager.GetString("lb_DueDate", resourceCulture);

	public static string lb_Email => ResourceManager.GetString("lb_Email", resourceCulture);

	public static string lb_Employee => ResourceManager.GetString("lb_Employee", resourceCulture);

	public static string lb_EmployeeCode => ResourceManager.GetString("lb_EmployeeCode", resourceCulture);

	public static string lb_EmployeeName => ResourceManager.GetString("lb_EmployeeName", resourceCulture);

	public static string lb_End => ResourceManager.GetString("lb_End", resourceCulture);

	public static string lb_EndTime => ResourceManager.GetString("lb_EndTime", resourceCulture);

	public static string lb_Extention => ResourceManager.GetString("lb_Extention", resourceCulture);

	public static string lb_FAName => ResourceManager.GetString("lb_FAName", resourceCulture);

	public static string lb_Fax => ResourceManager.GetString("lb_Fax", resourceCulture);

	public static string lb_FederalTaxID => ResourceManager.GetString("lb_FederalTaxID", resourceCulture);

	public static string lb_FileName => ResourceManager.GetString("lb_FileName", resourceCulture);

	public static string lb_Find => ResourceManager.GetString("lb_Find", resourceCulture);

	public static string lb_FixedAssetCode => ResourceManager.GetString("lb_FixedAssetCode", resourceCulture);

	public static string lb_FixedBatch => ResourceManager.GetString("lb_FixedBatch", resourceCulture);

	public static string lb_Folder => ResourceManager.GetString("lb_Folder", resourceCulture);

	public static string lb_Fri => ResourceManager.GetString("lb_Fri", resourceCulture);

	public static string lb_GrossProfit => ResourceManager.GetString("lb_GrossProfit", resourceCulture);

	public static string lb_GrossProfitTotal => ResourceManager.GetString("lb_GrossProfitTotal", resourceCulture);

	public static string lb_Group => ResourceManager.GetString("lb_Group", resourceCulture);

	public static string lb_Inactive => ResourceManager.GetString("lb_Inactive", resourceCulture);

	public static string lb_IndicatorName => ResourceManager.GetString("lb_IndicatorName", resourceCulture);

	public static string lb_InternalNumber => ResourceManager.GetString("lb_InternalNumber", resourceCulture);

	public static string lb_IssuedMethod => ResourceManager.GetString("lb_IssuedMethod", resourceCulture);

	public static string lb_IssuedQty => ResourceManager.GetString("lb_IssuedQty", resourceCulture);

	public static string lb_IssueForProduction => ResourceManager.GetString("lb_IssueForProduction", resourceCulture);

	public static string lb_IssueType => ResourceManager.GetString("lb_IssueType", resourceCulture);

	public static string lb_ItemName => ResourceManager.GetString("lb_ItemName", resourceCulture);

	public static string lb_ItemNo => ResourceManager.GetString("lb_ItemNo", resourceCulture);

	public static string lb_ItemType => ResourceManager.GetString("lb_ItemType", resourceCulture);

	public static string lb_JournalRemark => ResourceManager.GetString("lb_JournalRemark", resourceCulture);

	public static string lb_LaborCost => ResourceManager.GetString("lb_LaborCost", resourceCulture);

	public static string lb_LaborTime => ResourceManager.GetString("lb_LaborTime", resourceCulture);

	public static string lb_LaborUoM => ResourceManager.GetString("lb_LaborUoM", resourceCulture);

	public static string lb_LeadTime => ResourceManager.GetString("lb_LeadTime", resourceCulture);

	public static string lb_Length => ResourceManager.GetString("lb_Length", resourceCulture);

	public static string lb_LinkDraft => ResourceManager.GetString("lb_LinkDraft", resourceCulture);

	public static string lb_ListName => ResourceManager.GetString("lb_ListName", resourceCulture);

	public static string lb_Location => ResourceManager.GetString("lb_Location", resourceCulture);

	public static string lb_LocationCode => ResourceManager.GetString("lb_LocationCode", resourceCulture);

	public static string lb_Locked => ResourceManager.GetString("lb_Locked", resourceCulture);

	public static string lb_LotSize => ResourceManager.GetString("lb_LotSize", resourceCulture);

	public static string lb_MachineCost => ResourceManager.GetString("lb_MachineCost", resourceCulture);

	public static string lb_MachineTime => ResourceManager.GetString("lb_MachineTime", resourceCulture);

	public static string lb_MachineUoM => ResourceManager.GetString("lb_MachineUoM", resourceCulture);

	public static string lb_MainSalesEmp => ResourceManager.GetString("lb_MainSalesEmp", resourceCulture);

	public static string lb_MasterOrderCode => ResourceManager.GetString("lb_MasterOrderCode", resourceCulture);

	public static string lb_MasterOrderName => ResourceManager.GetString("lb_MasterOrderName", resourceCulture);

	public static string lb_MasterOrderNo => ResourceManager.GetString("lb_MasterOrderNo", resourceCulture);

	public static string lb_MaterialCode => ResourceManager.GetString("lb_MaterialCode", resourceCulture);

	public static string lb_MaterialName => ResourceManager.GetString("lb_MaterialName", resourceCulture);

	public static string lb_Memo => ResourceManager.GetString("lb_Memo", resourceCulture);

	public static string lb_MobilePhone => ResourceManager.GetString("lb_MobilePhone", resourceCulture);

	public static string lb_MOCode => ResourceManager.GetString("lb_MOCode", resourceCulture);

	public static string lb_ModelCode => ResourceManager.GetString("lb_ModelCode", resourceCulture);

	public static string lb_Mon => ResourceManager.GetString("lb_Mon", resourceCulture);

	public static string lb_MONo => ResourceManager.GetString("lb_MONo", resourceCulture);

	public static string lb_Name => ResourceManager.GetString("lb_Name", resourceCulture);

	public static string lb_NewPassword => ResourceManager.GetString("lb_NewPassword", resourceCulture);

	public static string lb_NextOp => ResourceManager.GetString("lb_NextOp", resourceCulture);

	public static string lb_No => ResourceManager.GetString("lb_No", resourceCulture);

	public static string lb_NoEndDate => ResourceManager.GetString("lb_NoEndDate", resourceCulture);

	public static string lb_Number => ResourceManager.GetString("lb_Number", resourceCulture);

	public static string lb_NumResUnit => ResourceManager.GetString("lb_NumResUnit", resourceCulture);

	public static string lb_ObjectTypeHistory => ResourceManager.GetString("lb_ObjectTypeHistory", resourceCulture);

	public static string lb_Occurrence => ResourceManager.GetString("lb_Occurrence", resourceCulture);

	public static string lb_Of => ResourceManager.GetString("lb_Of", resourceCulture);

	public static string lb_OldPassword => ResourceManager.GetString("lb_OldPassword", resourceCulture);

	public static string lb_OpCode => ResourceManager.GetString("lb_OpCode", resourceCulture);

	public static string lb_OpenQty => ResourceManager.GetString("lb_OpenQty", resourceCulture);

	public static string lb_OperationCode => ResourceManager.GetString("lb_OperationCode", resourceCulture);

	public static string lb_OperationName => ResourceManager.GetString("lb_OperationName", resourceCulture);

	public static string lb_OpporNo => ResourceManager.GetString("lb_OpporNo", resourceCulture);

	public static string lb_OpporStatus => ResourceManager.GetString("lb_OpporStatus", resourceCulture);

	public static string lb_OpportunityName => ResourceManager.GetString("lb_OpportunityName", resourceCulture);

	public static string lb_OpportunityStatus => ResourceManager.GetString("lb_OpportunityStatus", resourceCulture);

	public static string lb_OrderDate => ResourceManager.GetString("lb_OrderDate", resourceCulture);

	public static string lb_OrderNo => ResourceManager.GetString("lb_OrderNo", resourceCulture);

	public static string lb_Origin => ResourceManager.GetString("lb_Origin", resourceCulture);

	public static string lb_OtherCost => ResourceManager.GetString("lb_OtherCost", resourceCulture);

	public static string lb_OverDue => ResourceManager.GetString("lb_OverDue", resourceCulture);

	public static string lb_Owner => ResourceManager.GetString("lb_Owner", resourceCulture);

	public static string lb_Password => ResourceManager.GetString("lb_Password", resourceCulture);

	public static string lb_Path => ResourceManager.GetString("lb_Path", resourceCulture);

	public static string lb_PerInCharge => ResourceManager.GetString("lb_PerInCharge", resourceCulture);

	public static string lb_PickAndPackRemarks => ResourceManager.GetString("lb_PickAndPackRemarks", resourceCulture);

	public static string lb_PlannedQty => ResourceManager.GetString("lb_PlannedQty", resourceCulture);

	public static string lb_PlannedQuantity => ResourceManager.GetString("lb_PlannedQuantity", resourceCulture);

	public static string lb_PlannedTimes => ResourceManager.GetString("lb_PlannedTimes", resourceCulture);

	public static string lb_Plant => ResourceManager.GetString("lb_Plant", resourceCulture);

	public static string lb_PostingDate => ResourceManager.GetString("lb_PostingDate", resourceCulture);

	public static string lb_PotentialAmount => ResourceManager.GetString("lb_PotentialAmount", resourceCulture);

	public static string lb_PredictedClosingDate => ResourceManager.GetString("lb_PredictedClosingDate", resourceCulture);

	public static string lb_PredictedClosingIn => ResourceManager.GetString("lb_PredictedClosingIn", resourceCulture);

	public static string lb_PreviousActivity => ResourceManager.GetString("lb_PreviousActivity", resourceCulture);

	public static string lb_PrevOp => ResourceManager.GetString("lb_PrevOp", resourceCulture);

	public static string lb_Price => ResourceManager.GetString("lb_Price", resourceCulture);

	public static string lb_PriceList => ResourceManager.GetString("lb_PriceList", resourceCulture);

	public static string lb_PriceListName => ResourceManager.GetString("lb_PriceListName", resourceCulture);

	public static string lb_PriceListNo => ResourceManager.GetString("lb_PriceListNo", resourceCulture);

	public static string lb_ProductCode => ResourceManager.GetString("lb_ProductCode", resourceCulture);

	public static string lb_ProductDescription => ResourceManager.GetString("lb_ProductDescription", resourceCulture);

	public static string lb_ProductionOrdersType => ResourceManager.GetString("lb_ProductionOrdersType", resourceCulture);

	public static string lb_ProductName => ResourceManager.GetString("lb_ProductName", resourceCulture);

	public static string lb_ProductNo => ResourceManager.GetString("lb_ProductNo", resourceCulture);

	public static string lb_Project => ResourceManager.GetString("lb_Project", resourceCulture);

	public static string lb_ProjectCode => ResourceManager.GetString("lb_ProjectCode", resourceCulture);

	public static string lb_ProjectName => ResourceManager.GetString("lb_ProjectName", resourceCulture);

	public static string lb_PurchaseOrder => ResourceManager.GetString("lb_PurchaseOrder", resourceCulture);

	public static string lb_PurchaseQuotation => ResourceManager.GetString("lb_PurchaseQuotation", resourceCulture);

	public static string lb_PurchaseRequest => ResourceManager.GetString("lb_PurchaseRequest", resourceCulture);

	public static string lb_PurStage => ResourceManager.GetString("lb_PurStage", resourceCulture);

	public static string lb_Quantities => ResourceManager.GetString("lb_Quantities", resourceCulture);

	public static string lb_Quantity => ResourceManager.GetString("lb_Quantity", resourceCulture);

	public static string lb_Range => ResourceManager.GetString("lb_Range", resourceCulture);

	public static string lb_Rate => ResourceManager.GetString("lb_Rate", resourceCulture);

	public static string lb_Recurrence => ResourceManager.GetString("lb_Recurrence", resourceCulture);

	public static string lb_Ref2 => ResourceManager.GetString("lb_Ref2", resourceCulture);

	public static string lb_RejectedQuantity => ResourceManager.GetString("lb_RejectedQuantity", resourceCulture);

	public static string lb_RelatedBP => ResourceManager.GetString("lb_RelatedBP", resourceCulture);

	public static string lb_RelatedType => ResourceManager.GetString("lb_RelatedType", resourceCulture);

	public static string lb_Relationship => ResourceManager.GetString("lb_Relationship", resourceCulture);

	public static string lb_RelationshipCode => ResourceManager.GetString("lb_RelationshipCode", resourceCulture);

	public static string lb_RelationshipDescription => ResourceManager.GetString("lb_RelationshipDescription", resourceCulture);

	public static string lb_Remark => ResourceManager.GetString("lb_Remark", resourceCulture);

	public static string lb_Reminder => ResourceManager.GetString("lb_Reminder", resourceCulture);

	public static string lb_RePassword => ResourceManager.GetString("lb_RePassword", resourceCulture);

	public static string lb_RepeatEvery => ResourceManager.GetString("lb_RepeatEvery", resourceCulture);

	public static string lb_RepeatOn => ResourceManager.GetString("lb_RepeatOn", resourceCulture);

	public static string lb_ResCode => ResourceManager.GetString("lb_ResCode", resourceCulture);

	public static string lb_ResName => ResourceManager.GetString("lb_ResName", resourceCulture);

	public static string lb_ResourceName => ResourceManager.GetString("lb_ResourceName", resourceCulture);

	public static string lb_ResourceNo => ResourceManager.GetString("lb_ResourceNo", resourceCulture);

	public static string lb_ResourceStandardCostName => ResourceManager.GetString("lb_ResourceStandardCostName", resourceCulture);

	public static string lb_ResourceWIPAct => ResourceManager.GetString("lb_ResourceWIPAct", resourceCulture);

	public static string lb_Room => ResourceManager.GetString("lb_Room", resourceCulture);

	public static string lb_RouCode => ResourceManager.GetString("lb_RouCode", resourceCulture);

	public static string lb_RouName => ResourceManager.GetString("lb_RouName", resourceCulture);

	public static string lb_RoutingCode => ResourceManager.GetString("lb_RoutingCode", resourceCulture);

	public static string lb_RoutingName => ResourceManager.GetString("lb_RoutingName", resourceCulture);

	public static string lb_SalesEmployee => ResourceManager.GetString("lb_SalesEmployee", resourceCulture);

	public static string lb_SalesOrder => ResourceManager.GetString("lb_SalesOrder", resourceCulture);

	public static string lb_SalesQuotation => ResourceManager.GetString("lb_SalesQuotation", resourceCulture);

	public static string lb_SalesStage => ResourceManager.GetString("lb_SalesStage", resourceCulture);

	public static string lb_Sat => ResourceManager.GetString("lb_Sat", resourceCulture);

	public static string lb_SequenceNo => ResourceManager.GetString("lb_SequenceNo", resourceCulture);

	public static string lb_Series => ResourceManager.GetString("lb_Series", resourceCulture);

	public static string lb_SeriesName => ResourceManager.GetString("lb_SeriesName", resourceCulture);

	public static string lb_SeriesNo => ResourceManager.GetString("lb_SeriesNo", resourceCulture);

	public static string lb_SetupCost => ResourceManager.GetString("lb_SetupCost", resourceCulture);

	public static string lb_SetupTime => ResourceManager.GetString("lb_SetupTime", resourceCulture);

	public static string lb_SetupUoM => ResourceManager.GetString("lb_SetupUoM", resourceCulture);

	public static string lb_ShipType => ResourceManager.GetString("lb_ShipType", resourceCulture);

	public static string lb_ShowBPDocs => ResourceManager.GetString("lb_ShowBPDocs", resourceCulture);

	public static string lb_ShowDocumentsRelatedtotheBP => ResourceManager.GetString("lb_ShowDocumentsRelatedtotheBP", resourceCulture);

	public static string lb_Sort => ResourceManager.GetString("lb_Sort", resourceCulture);

	public static string lb_SourceObjectNo => ResourceManager.GetString("lb_SourceObjectNo", resourceCulture);

	public static string lb_SourceObjectType => ResourceManager.GetString("lb_SourceObjectType", resourceCulture);

	public static string lb_Stage => ResourceManager.GetString("lb_Stage", resourceCulture);

	public static string lb_StageNo => ResourceManager.GetString("lb_StageNo", resourceCulture);

	public static string lb_Start => ResourceManager.GetString("lb_Start", resourceCulture);

	public static string lb_StartDate => ResourceManager.GetString("lb_StartDate", resourceCulture);

	public static string lb_State => ResourceManager.GetString("lb_State", resourceCulture);

	public static string lb_Status => ResourceManager.GetString("lb_Status", resourceCulture);

	public static string lb_Street => ResourceManager.GetString("lb_Street", resourceCulture);

	public static string lb_SubOpCode => ResourceManager.GetString("lb_SubOpCode", resourceCulture);

	public static string lb_Sun => ResourceManager.GetString("lb_Sun", resourceCulture);

	public static string lb_TargetPath => ResourceManager.GetString("lb_TargetPath", resourceCulture);

	public static string lb_Technical => ResourceManager.GetString("lb_Technical", resourceCulture);

	public static string lb_TelephoneNo => ResourceManager.GetString("lb_TelephoneNo", resourceCulture);

	public static string lb_Tentative => ResourceManager.GetString("lb_Tentative", resourceCulture);

	public static string lb_ThreatLevel => ResourceManager.GetString("lb_ThreatLevel", resourceCulture);

	public static string lb_Thu => ResourceManager.GetString("lb_Thu", resourceCulture);

	public static string lb_Total => ResourceManager.GetString("lb_Total", resourceCulture);

	public static string lb_TotalAdditionalTime => ResourceManager.GetString("lb_TotalAdditionalTime", resourceCulture);

	public static string lb_TotalDemand => ResourceManager.GetString("lb_TotalDemand", resourceCulture);

	public static string lb_TotalRunTime => ResourceManager.GetString("lb_TotalRunTime", resourceCulture);

	public static string lb_TotalTime => ResourceManager.GetString("lb_TotalTime", resourceCulture);

	public static string lb_TotalVariance => ResourceManager.GetString("lb_TotalVariance", resourceCulture);

	public static string lb_TranType => ResourceManager.GetString("lb_TranType", resourceCulture);

	public static string lb_TreeType => ResourceManager.GetString("lb_TreeType", resourceCulture);

	public static string lb_Tue => ResourceManager.GetString("lb_Tue", resourceCulture);

	public static string lb_Type => ResourceManager.GetString("lb_Type", resourceCulture);

	public static string lb_UoM => ResourceManager.GetString("lb_UoM", resourceCulture);

	public static string lb_UomCode => ResourceManager.GetString("lb_UomCode", resourceCulture);

	public static string lb_UoMName => ResourceManager.GetString("lb_UoMName", resourceCulture);

	public static string lb_User => ResourceManager.GetString("lb_User", resourceCulture);

	public static string lb_UserCode => ResourceManager.GetString("lb_UserCode", resourceCulture);

	public static string lb_UserName => ResourceManager.GetString("lb_UserName", resourceCulture);

	public static string lb_ValidFrom => ResourceManager.GetString("lb_ValidFrom", resourceCulture);

	public static string lb_ValidTo => ResourceManager.GetString("lb_ValidTo", resourceCulture);

	public static string lb_Warehouse => ResourceManager.GetString("lb_Warehouse", resourceCulture);

	public static string lb_WarehouseCode => ResourceManager.GetString("lb_WarehouseCode", resourceCulture);

	public static string lb_WarehouseName => ResourceManager.GetString("lb_WarehouseName", resourceCulture);

	public static string lb_Wed => ResourceManager.GetString("lb_Wed", resourceCulture);

	public static string lb_WeighAmount => ResourceManager.GetString("lb_WeighAmount", resourceCulture);

	public static string lb_Width => ResourceManager.GetString("lb_Width", resourceCulture);

	public static string lb_WipAccount => ResourceManager.GetString("lb_WipAccount", resourceCulture);

	public static string lb_WipActCode => ResourceManager.GetString("lb_WipActCode", resourceCulture);

	public static string lb_Won => ResourceManager.GetString("lb_Won", resourceCulture);

	public static string lb_WorkCenter => ResourceManager.GetString("lb_WorkCenter", resourceCulture);

	public static string lb_ZipCode => ResourceManager.GetString("lb_ZipCode", resourceCulture);

	public static string Line_gvFile_gcDate => ResourceManager.GetString("Line_gvFile_gcDate", resourceCulture);

	public static string Line_gvFile_gcFileName => ResourceManager.GetString("Line_gvFile_gcFileName", resourceCulture);

	public static string Line_gvFile_gcTrgtPath => ResourceManager.GetString("Line_gvFile_gcTrgtPath", resourceCulture);

	public static string Line_gvLine_Discount => ResourceManager.GetString("Line_gvLine_Discount", resourceCulture);

	public static string Line_gvLine_gcBOMTreeType => ResourceManager.GetString("Line_gvLine_gcBOMTreeType", resourceCulture);

	public static string Line_gvLine_gcCurrency => ResourceManager.GetString("Line_gvLine_gcCurrency", resourceCulture);

	public static string Line_gvLine_gcDelDate => ResourceManager.GetString("Line_gvLine_gcDelDate", resourceCulture);

	public static string Line_gvLine_gcFreeTxt => ResourceManager.GetString("Line_gvLine_gcFreeTxt", resourceCulture);

	public static string Line_gvLine_gcFromWhs => ResourceManager.GetString("Line_gvLine_gcFromWhs", resourceCulture);

	public static string Line_gvLine_gcItemCode => ResourceManager.GetString("Line_gvLine_gcItemCode", resourceCulture);

	public static string Line_gvLine_gcItemName => ResourceManager.GetString("Line_gvLine_gcItemName", resourceCulture);

	public static string Line_gvLine_gcLineTotal => ResourceManager.GetString("Line_gvLine_gcLineTotal", resourceCulture);

	public static string Line_gvLine_gcOcrCode => ResourceManager.GetString("Line_gvLine_gcOcrCode", resourceCulture);

	public static string Line_gvLine_gcOcrCode2 => ResourceManager.GetString("Line_gvLine_gcOcrCode2", resourceCulture);

	public static string Line_gvLine_gcOcrCode3 => ResourceManager.GetString("Line_gvLine_gcOcrCode3", resourceCulture);

	public static string Line_gvLine_gcOcrCode4 => ResourceManager.GetString("Line_gvLine_gcOcrCode4", resourceCulture);

	public static string Line_gvLine_gcOcrCode5 => ResourceManager.GetString("Line_gvLine_gcOcrCode5", resourceCulture);

	public static string Line_gvLine_gcOpenQty => ResourceManager.GetString("Line_gvLine_gcOpenQty", resourceCulture);

	public static string Line_gvLine_gcPrice => ResourceManager.GetString("Line_gvLine_gcPrice", resourceCulture);

	public static string Line_gvLine_gcQuantity => ResourceManager.GetString("Line_gvLine_gcQuantity", resourceCulture);

	public static string Line_gvLine_gcRate => ResourceManager.GetString("Line_gvLine_gcRate", resourceCulture);

	public static string Line_gvLine_gcRateCurrency => ResourceManager.GetString("Line_gvLine_gcRateCurrency", resourceCulture);

	public static string Line_gvLine_gcShipDate => ResourceManager.GetString("Line_gvLine_gcShipDate", resourceCulture);

	public static string Line_gvLine_gcTotalFC => ResourceManager.GetString("Line_gvLine_gcTotalFC", resourceCulture);

	public static string Line_gvLine_gcUomCode => ResourceManager.GetString("Line_gvLine_gcUomCode", resourceCulture);

	public static string Line_gvLine_gcVatGroup => ResourceManager.GetString("Line_gvLine_gcVatGroup", resourceCulture);

	public static string Line_gvLine_gcVatSumFrgn => ResourceManager.GetString("Line_gvLine_gcVatSumFrgn", resourceCulture);

	public static string Line_gvLine_gcWhseCode => ResourceManager.GetString("Line_gvLine_gcWhseCode", resourceCulture);

	public static string Line_gvLine_VatAmount => ResourceManager.GetString("Line_gvLine_VatAmount", resourceCulture);

	public static string Line_gvPayTerms_gcAmount => ResourceManager.GetString("Line_gvPayTerms_gcAmount", resourceCulture);

	public static string Line_gvPayTerms_gcBalanceDue => ResourceManager.GetString("Line_gvPayTerms_gcBalanceDue", resourceCulture);

	public static string Line_gvPayTerms_gcDescription => ResourceManager.GetString("Line_gvPayTerms_gcDescription", resourceCulture);

	public static string Line_gvPayTerms_gcDueDate => ResourceManager.GetString("Line_gvPayTerms_gcDueDate", resourceCulture);

	public static string Line_gvPayTerms_gcPaid => ResourceManager.GetString("Line_gvPayTerms_gcPaid", resourceCulture);

	public static string Line_gvPayTerms_gcPayments => ResourceManager.GetString("Line_gvPayTerms_gcPayments", resourceCulture);

	public static string Line_gvPayTerms_gcPercents => ResourceManager.GetString("Line_gvPayTerms_gcPercents", resourceCulture);

	public static string LinePur_gvLine_gcPQTReqdate => ResourceManager.GetString("LinePur_gvLine_gcPQTReqdate", resourceCulture);

	public static string LinePur_gvLine_gcQuantity => ResourceManager.GetString("LinePur_gvLine_gcQuantity", resourceCulture);

	public static string Msg => ResourceManager.GetString("Msg", resourceCulture);

	public static string Msg_BpCodeNotNull => ResourceManager.GetString("Msg_BpCodeNotNull", resourceCulture);

	public static string Msg_CancelSuccess => ResourceManager.GetString("Msg_CancelSuccess", resourceCulture);

	public static string Msg_CannotConnectToServer => ResourceManager.GetString("Msg_CannotConnectToServer", resourceCulture);

	public static string Msg_CardCodeDoesNotExists => ResourceManager.GetString("Msg_CardCodeDoesNotExists", resourceCulture);

	public static string Msg_CloseDateCanNotBeforeStartDate => ResourceManager.GetString("Msg_CloseDateCanNotBeforeStartDate", resourceCulture);

	public static string Msg_CloseDateCanNotNullWhenDocumentIsClosed => ResourceManager.GetString("Msg_CloseDateCanNotNullWhenDocumentIsClosed", resourceCulture);

	public static string Msg_CloseDateIsNull => ResourceManager.GetString("Msg_CloseDateIsNull", resourceCulture);

	public static string Msg_CloseSuccess => ResourceManager.GetString("Msg_CloseSuccess", resourceCulture);

	public static string Msg_ConnectFaile => ResourceManager.GetString("Msg_ConnectFaile", resourceCulture);

	public static string Msg_ConnectSuccess => ResourceManager.GetString("Msg_ConnectSuccess", resourceCulture);

	public static string Msg_DeleteError => ResourceManager.GetString("Msg_DeleteError", resourceCulture);

	public static string Msg_DeleteSuccess => ResourceManager.GetString("Msg_DeleteSuccess", resourceCulture);

	public static string Msg_DoYouWantToChangePaymentCount => ResourceManager.GetString("Msg_DoYouWantToChangePaymentCount", resourceCulture);

	public static string Msg_ExistsCardCode => ResourceManager.GetString("Msg_ExistsCardCode", resourceCulture);

	public static string Msg_FormAccessDenied => ResourceManager.GetString("Msg_FormAccessDenied", resourceCulture);

	public static string Msg_FormChangeDenied => ResourceManager.GetString("Msg_FormChangeDenied", resourceCulture);

	public static string Msg_FunctionNotSupported => ResourceManager.GetString("Msg_FunctionNotSupported", resourceCulture);

	public static string Msg_ItemCodeExists => ResourceManager.GetString("Msg_ItemCodeExists", resourceCulture);

	public static string Msg_LoginInfoNotValid => ResourceManager.GetString("Msg_LoginInfoNotValid", resourceCulture);

	public static string Msg_NameNotNull => ResourceManager.GetString("Msg_NameNotNull", resourceCulture);

	public static string Msg_NoItemChoose => ResourceManager.GetString("Msg_NoItemChoose", resourceCulture);

	public static string Msg_NoRowIsSelected => ResourceManager.GetString("Msg_NoRowIsSelected", resourceCulture);

	public static string Msg_NotChoose => ResourceManager.GetString("Msg_NotChoose", resourceCulture);

	public static string Msg_NotChooseLine => ResourceManager.GetString("Msg_NotChooseLine", resourceCulture);

	public static string Msg_NotChooseRowDuplicate => ResourceManager.GetString("Msg_NotChooseRowDuplicate", resourceCulture);

	public static string Msg_NotExists => ResourceManager.GetString("Msg_NotExists", resourceCulture);

	public static string Msg_NotItem => ResourceManager.GetString("Msg_NotItem", resourceCulture);

	public static string Msg_NotNullBpCode => ResourceManager.GetString("Msg_NotNullBpCode", resourceCulture);

	public static string Msg_NotRecord => ResourceManager.GetString("Msg_NotRecord", resourceCulture);

	public static string Msg_OnlyBeCopiedFromTheSameVendor => ResourceManager.GetString("Msg_OnlyBeCopiedFromTheSameVendor", resourceCulture);

	public static string Msg_ProjectCodeNotNull => ResourceManager.GetString("Msg_ProjectCodeNotNull", resourceCulture);

	public static string Msg_Quantity => ResourceManager.GetString("Msg_Quantity", resourceCulture);

	public static string Msg_SaveError => ResourceManager.GetString("Msg_SaveError", resourceCulture);

	public static string Msg_SaveSuccess => ResourceManager.GetString("Msg_SaveSuccess", resourceCulture);

	public static string Msg_StartDateIsNull => ResourceManager.GetString("Msg_StartDateIsNull", resourceCulture);

	public static string Msg_Success => ResourceManager.GetString("Msg_Success", resourceCulture);

	public static string Msg_UnknownError => ResourceManager.GetString("Msg_UnknownError", resourceCulture);

	public static string Msg_UpdateFailed => ResourceManager.GetString("Msg_UpdateFailed", resourceCulture);

	public static string Msg_UpdateSuccess => ResourceManager.GetString("Msg_UpdateSuccess", resourceCulture);

	public static string Msg_UsersCodeNotNull => ResourceManager.GetString("Msg_UsersCodeNotNull", resourceCulture);

	public static string Msg_UsersNameNotNull => ResourceManager.GetString("Msg_UsersNameNotNull", resourceCulture);

	public static string Msg_WarehouseNotExists => ResourceManager.GetString("Msg_WarehouseNotExists", resourceCulture);

	public static string Msg_Warning => ResourceManager.GetString("Msg_Warning", resourceCulture);

	public static string Msg_YouAreLoggedInWithTheAccount => ResourceManager.GetString("Msg_YouAreLoggedInWithTheAccount", resourceCulture);

	public static string Msg_YouhaveNotchooseVendor => ResourceManager.GetString("Msg_YouhaveNotchooseVendor", resourceCulture);

	public static string Msg_YouHaventSaved => ResourceManager.GetString("Msg_YouHaventSaved", resourceCulture);

	public static string Msg_YouMustInputItemCode => ResourceManager.GetString("Msg_YouMustInputItemCode", resourceCulture);

	public static string nav_Atp => ResourceManager.GetString("nav_Atp", resourceCulture);

	public static string nav_BatchDetail => ResourceManager.GetString("nav_BatchDetail", resourceCulture);

	public static string nav_BusinessCatalog => ResourceManager.GetString("nav_BusinessCatalog", resourceCulture);

	public static string nav_SalesOrder => ResourceManager.GetString("nav_SalesOrder", resourceCulture);

	public static string nav_SalesQuotation => ResourceManager.GetString("nav_SalesQuotation", resourceCulture);

	public static string navAddNewMps => ResourceManager.GetString("navAddNewMps", resourceCulture);

	public static string navApCredit => ResourceManager.GetString("navApCredit", resourceCulture);

	public static string navApInvoice => ResourceManager.GetString("navApInvoice", resourceCulture);

	public static string navApReserve => ResourceManager.GetString("navApReserve", resourceCulture);

	public static string navAuthorization => ResourceManager.GetString("navAuthorization", resourceCulture);

	public static string navBarItem_DocumentDraftsReport => ResourceManager.GetString("navBarItem_DocumentDraftsReport", resourceCulture);

	public static string navBarItem_FtpSetting => ResourceManager.GetString("navBarItem_FtpSetting", resourceCulture);

	public static string navBarItem2_Activity => ResourceManager.GetString("navBarItem2_Activity", resourceCulture);

	public static string navBarItemBoMJob => ResourceManager.GetString("navBarItemBoMJob", resourceCulture);

	public static string navBarItemExRateAndIndexes => ResourceManager.GetString("navBarItemExRateAndIndexes", resourceCulture);

	public static string navBarItemSalesOpport => ResourceManager.GetString("navBarItemSalesOpport", resourceCulture);

	public static string navBOM => ResourceManager.GetString("navBOM", resourceCulture);

	public static string navBp => ResourceManager.GetString("navBp", resourceCulture);

	public static string navChangePwd => ResourceManager.GetString("navChangePwd", resourceCulture);

	public static string navChangeUser => ResourceManager.GetString("navChangeUser", resourceCulture);

	public static string navCredit => ResourceManager.GetString("navCredit", resourceCulture);

	public static string navDelivery => ResourceManager.GetString("navDelivery", resourceCulture);

	public static string navGeneralSetting => ResourceManager.GetString("navGeneralSetting", resourceCulture);

	public static string navGoodsIssue => ResourceManager.GetString("navGoodsIssue", resourceCulture);

	public static string navGoodsReceipt => ResourceManager.GetString("navGoodsReceipt", resourceCulture);

	public static string navGoodsReturn => ResourceManager.GetString("navGoodsReturn", resourceCulture);

	public static string navGroup_Administration => ResourceManager.GetString("navGroup_Administration", resourceCulture);

	public static string navGroup_Banking => ResourceManager.GetString("navGroup_Banking", resourceCulture);

	public static string navGroup_BusinessMaster => ResourceManager.GetString("navGroup_BusinessMaster", resourceCulture);

	public static string navGroup_Inventory => ResourceManager.GetString("navGroup_Inventory", resourceCulture);

	public static string navGroup_Opportunity => ResourceManager.GetString("navGroup_Opportunity", resourceCulture);

	public static string navGroup_PlanningProduction => ResourceManager.GetString("navGroup_PlanningProduction", resourceCulture);

	public static string navGroup_Production => ResourceManager.GetString("navGroup_Production", resourceCulture);

	public static string navGroup_Purchase => ResourceManager.GetString("navGroup_Purchase", resourceCulture);

	public static string navGroup_Sales => ResourceManager.GetString("navGroup_Sales", resourceCulture);

	public static string navGrpo => ResourceManager.GetString("navGrpo", resourceCulture);

	public static string navInComing => ResourceManager.GetString("navInComing", resourceCulture);

	public static string navInvoice => ResourceManager.GetString("navInvoice", resourceCulture);

	public static string navIssueForProduction => ResourceManager.GetString("navIssueForProduction", resourceCulture);

	public static string navItemGroup => ResourceManager.GetString("navItemGroup", resourceCulture);

	public static string navItm => ResourceManager.GetString("navItm", resourceCulture);

	public static string navItmTransfer => ResourceManager.GetString("navItmTransfer", resourceCulture);

	public static string navLendedCost => ResourceManager.GetString("navLendedCost", resourceCulture);

	public static string navListOfMps => ResourceManager.GetString("navListOfMps", resourceCulture);

	public static string navListOfScheduling => ResourceManager.GetString("navListOfScheduling", resourceCulture);

	public static string navMrpList => ResourceManager.GetString("navMrpList", resourceCulture);

	public static string navMrpRunning => ResourceManager.GetString("navMrpRunning", resourceCulture);

	public static string navOutGoing => ResourceManager.GetString("navOutGoing", resourceCulture);

	public static string navProductionOder => ResourceManager.GetString("navProductionOder", resourceCulture);

	public static string navPurchaseOrder => ResourceManager.GetString("navPurchaseOrder", resourceCulture);

	public static string navPurchaseQuotation => ResourceManager.GetString("navPurchaseQuotation", resourceCulture);

	public static string navPurchaseRequest => ResourceManager.GetString("navPurchaseRequest", resourceCulture);

	public static string navReceipt => ResourceManager.GetString("navReceipt", resourceCulture);

	public static string navReserve => ResourceManager.GetString("navReserve", resourceCulture);

	public static string navReturn => ResourceManager.GetString("navReturn", resourceCulture);

	public static string navRevaluation => ResourceManager.GetString("navRevaluation", resourceCulture);

	public static string navScheduling => ResourceManager.GetString("navScheduling", resourceCulture);

	public static string navSettingPassword => ResourceManager.GetString("navSettingPassword", resourceCulture);

	public static string navShift => ResourceManager.GetString("navShift", resourceCulture);

	public static string navSpecialPriceForBusinessPartner => ResourceManager.GetString("navSpecialPriceForBusinessPartner", resourceCulture);

	public static string navTransferRequest => ResourceManager.GetString("navTransferRequest", resourceCulture);

	public static string navUser => ResourceManager.GetString("navUser", resourceCulture);

	public static string navWorkSchedule => ResourceManager.GetString("navWorkSchedule", resourceCulture);

	public static string OBPL_AliasName => ResourceManager.GetString("OBPL_AliasName", resourceCulture);

	public static string OBPL_BPLName => ResourceManager.GetString("OBPL_BPLName", resourceCulture);

	public static string OBPL_Country => ResourceManager.GetString("OBPL_Country", resourceCulture);

	public static string OBPL_County => ResourceManager.GetString("OBPL_County", resourceCulture);

	public static string OBPL_State => ResourceManager.GetString("OBPL_State", resourceCulture);

	public static string OCDC_Title_frm_CashDiscountEdit => ResourceManager.GetString("OCDC_Title_frm_CashDiscountEdit", resourceCulture);

	public static string OCLA_descriptio => ResourceManager.GetString("OCLA_descriptio", resourceCulture);

	public static string OCLA_name => ResourceManager.GetString("OCLA_name", resourceCulture);

	public static string OCLA_statusID => ResourceManager.GetString("OCLA_statusID", resourceCulture);

	public static string OCLG_ActDate => ResourceManager.GetString("OCLG_ActDate", resourceCulture);

	public static string OCLG_Action => ResourceManager.GetString("OCLG_Action", resourceCulture);

	public static string OCLG_AssignedBy => ResourceManager.GetString("OCLG_AssignedBy", resourceCulture);

	public static string OCLG_BeginTime => ResourceManager.GetString("OCLG_BeginTime", resourceCulture);

	public static string OCLG_ClgCode => ResourceManager.GetString("OCLG_ClgCode", resourceCulture);

	public static string OCLG_CntctDate => ResourceManager.GetString("OCLG_CntctDate", resourceCulture);

	public static string OCLG_CntctSbjct => ResourceManager.GetString("OCLG_CntctSbjct", resourceCulture);

	public static string OCLG_CntctType => ResourceManager.GetString("OCLG_CntctType", resourceCulture);

	public static string OCLG_Details => ResourceManager.GetString("OCLG_Details", resourceCulture);

	public static string OCLG_Duration => ResourceManager.GetString("OCLG_Duration", resourceCulture);

	public static string OCLG_HandledBy => ResourceManager.GetString("OCLG_HandledBy", resourceCulture);

	public static string OCLG_inactive => ResourceManager.GetString("OCLG_inactive", resourceCulture);

	public static string OCLG_Notes => ResourceManager.GetString("OCLG_Notes", resourceCulture);

	public static string OCLG_personal => ResourceManager.GetString("OCLG_personal", resourceCulture);

	public static string OCLG_Recontact => ResourceManager.GetString("OCLG_Recontact", resourceCulture);

	public static string OCLG_RecurPat => ResourceManager.GetString("OCLG_RecurPat", resourceCulture);

	public static string OCLG_status => ResourceManager.GetString("OCLG_status", resourceCulture);

	public static string OCLG_Time => ResourceManager.GetString("OCLG_Time", resourceCulture);

	public static string OCPR_Address => ResourceManager.GetString("OCPR_Address", resourceCulture);

	public static string OCPR_BirthCity => ResourceManager.GetString("OCPR_BirthCity", resourceCulture);

	public static string OCPR_BirthDate => ResourceManager.GetString("OCPR_BirthDate", resourceCulture);

	public static string OCPR_BirthPlace => ResourceManager.GetString("OCPR_BirthPlace", resourceCulture);

	public static string OCPR_Cellolar => ResourceManager.GetString("OCPR_Cellolar", resourceCulture);

	public static string OCPR_CntctCode => ResourceManager.GetString("OCPR_CntctCode", resourceCulture);

	public static string OCPR_E_MailL => ResourceManager.GetString("OCPR_E_MailL", resourceCulture);

	public static string OCPR_Fax => ResourceManager.GetString("OCPR_Fax", resourceCulture);

	public static string OCPR_Gender => ResourceManager.GetString("OCPR_Gender", resourceCulture);

	public static string OCPR_Name => ResourceManager.GetString("OCPR_Name", resourceCulture);

	public static string OCPR_Note1 => ResourceManager.GetString("OCPR_Note1", resourceCulture);

	public static string OCPR_Note2 => ResourceManager.GetString("OCPR_Note2", resourceCulture);

	public static string OCPR_Pager => ResourceManager.GetString("OCPR_Pager", resourceCulture);

	public static string OCPR_Password => ResourceManager.GetString("OCPR_Password", resourceCulture);

	public static string OCPR_Position => ResourceManager.GetString("OCPR_Position", resourceCulture);

	public static string OCPR_Profession => ResourceManager.GetString("OCPR_Profession", resourceCulture);

	public static string OCPR_Tel1 => ResourceManager.GetString("OCPR_Tel1", resourceCulture);

	public static string OCPR_Tel2 => ResourceManager.GetString("OCPR_Tel2", resourceCulture);

	public static string OCPR_Title => ResourceManager.GetString("OCPR_Title", resourceCulture);

	public static string OCRC_CardName => ResourceManager.GetString("OCRC_CardName", resourceCulture);

	public static string OCRC_CompanyId => ResourceManager.GetString("OCRC_CompanyId", resourceCulture);

	public static string OCRC_CreditCard => ResourceManager.GetString("OCRC_CreditCard", resourceCulture);

	public static string OCRC_IntTaxCode => ResourceManager.GetString("OCRC_IntTaxCode", resourceCulture);

	public static string OCRC_Phone => ResourceManager.GetString("OCRC_Phone", resourceCulture);

	public static string OCRC_Title_frm_CreditCardType => ResourceManager.GetString("OCRC_Title_frm_CreditCardType", resourceCulture);

	public static string OCRD_AcctName => ResourceManager.GetString("OCRD_AcctName", resourceCulture);

	public static string OCRD_AddID => ResourceManager.GetString("OCRD_AddID", resourceCulture);

	public static string OCRD_AvrageLate => ResourceManager.GetString("OCRD_AvrageLate", resourceCulture);

	public static string OCRD_Balance => ResourceManager.GetString("OCRD_Balance", resourceCulture);

	public static string OCRD_BankCode => ResourceManager.GetString("OCRD_BankCode", resourceCulture);

	public static string OCRD_BankName => ResourceManager.GetString("OCRD_BankName", resourceCulture);

	public static string OCRD_BillToDef => ResourceManager.GetString("OCRD_BillToDef", resourceCulture);

	public static string OCRD_CardCode => ResourceManager.GetString("OCRD_CardCode", resourceCulture);

	public static string OCRD_CardFName => ResourceManager.GetString("OCRD_CardFName", resourceCulture);

	public static string OCRD_CardName => ResourceManager.GetString("OCRD_CardName", resourceCulture);

	public static string OCRD_CardType => ResourceManager.GetString("OCRD_CardType", resourceCulture);

	public static string OCRD_CardValid => ResourceManager.GetString("OCRD_CardValid", resourceCulture);

	public static string OCRD_Cellular => ResourceManager.GetString("OCRD_Cellular", resourceCulture);

	public static string OCRD_CntctPrsn => ResourceManager.GetString("OCRD_CntctPrsn", resourceCulture);

	public static string OCRD_CrCardNum => ResourceManager.GetString("OCRD_CrCardNum", resourceCulture);

	public static string OCRD_CreditCard => ResourceManager.GetString("OCRD_CreditCard", resourceCulture);

	public static string OCRD_CreditLine => ResourceManager.GetString("OCRD_CreditLine", resourceCulture);

	public static string OCRD_Currency => ResourceManager.GetString("OCRD_Currency", resourceCulture);

	public static string OCRD_DebtLine => ResourceManager.GetString("OCRD_DebtLine", resourceCulture);

	public static string OCRD_DflAccount => ResourceManager.GetString("OCRD_DflAccount", resourceCulture);

	public static string OCRD_DflBranch => ResourceManager.GetString("OCRD_DflBranch", resourceCulture);

	public static string OCRD_Discount => ResourceManager.GetString("OCRD_Discount", resourceCulture);

	public static string OCRD_DNotesBal => ResourceManager.GetString("OCRD_DNotesBal", resourceCulture);

	public static string OCRD_E_Mail => ResourceManager.GetString("OCRD_E_Mail", resourceCulture);

	public static string OCRD_Fax => ResourceManager.GetString("OCRD_Fax", resourceCulture);

	public static string OCRD_GroupCode => ResourceManager.GetString("OCRD_GroupCode", resourceCulture);

	public static string OCRD_GroupNum => ResourceManager.GetString("OCRD_GroupNum", resourceCulture);

	public static string OCRD_IntrntSite => ResourceManager.GetString("OCRD_IntrntSite", resourceCulture);

	public static string OCRD_IntrstRate => ResourceManager.GetString("OCRD_IntrstRate", resourceCulture);

	public static string OCRD_LicTradNum => ResourceManager.GetString("OCRD_LicTradNum", resourceCulture);

	public static string OCRD_ListNum => ResourceManager.GetString("OCRD_ListNum", resourceCulture);

	public static string OCRD_Notes => ResourceManager.GetString("OCRD_Notes", resourceCulture);

	public static string OCRD_OrdersBal => ResourceManager.GetString("OCRD_OrdersBal", resourceCulture);

	public static string OCRD_OwnerIdNum => ResourceManager.GetString("OCRD_OwnerIdNum", resourceCulture);

	public static string OCRD_Password => ResourceManager.GetString("OCRD_Password", resourceCulture);

	public static string OCRD_Phone1 => ResourceManager.GetString("OCRD_Phone1", resourceCulture);

	public static string OCRD_Phone2 => ResourceManager.GetString("OCRD_Phone2", resourceCulture);

	public static string OCRD_ShipToDef => ResourceManager.GetString("OCRD_ShipToDef", resourceCulture);

	public static string OCRD_ShipType => ResourceManager.GetString("OCRD_ShipType", resourceCulture);

	public static string OCRD_SlpCode => ResourceManager.GetString("OCRD_SlpCode", resourceCulture);

	public static string OCRD_Title_frm_BusinessMasterDataList => ResourceManager.GetString("OCRD_Title_frm_BusinessMasterDataList", resourceCulture);

	public static string OCRD_validFor => ResourceManager.GetString("OCRD_validFor", resourceCulture);

	public static string OCRD_VatIdUnCmp => ResourceManager.GetString("OCRD_VatIdUnCmp", resourceCulture);

	public static string OCRG_GroupCode => ResourceManager.GetString("OCRG_GroupCode", resourceCulture);

	public static string OCRG_GroupName => ResourceManager.GetString("OCRG_GroupName", resourceCulture);

	public static string OCRG_GroupType => ResourceManager.GetString("OCRG_GroupType", resourceCulture);

	public static string OCRG_Locked => ResourceManager.GetString("OCRG_Locked", resourceCulture);

	public static string OCRG_Title_frm_CardGroup => ResourceManager.GetString("OCRG_Title_frm_CardGroup", resourceCulture);

	public static string OCRG_UserSign => ResourceManager.GetString("OCRG_UserSign", resourceCulture);

	public static string OCRN_Chk100Name => ResourceManager.GetString("OCRN_Chk100Name", resourceCulture);

	public static string OCRN_DocCurrCod => ResourceManager.GetString("OCRN_DocCurrCod", resourceCulture);

	public static string OCRN_ISOCurrCod => ResourceManager.GetString("OCRN_ISOCurrCod", resourceCulture);

	public static string OCRN_Title_frm_CurrencyCode => ResourceManager.GetString("OCRN_Title_frm_CurrencyCode", resourceCulture);

	public static string OCTG_BslineDate => ResourceManager.GetString("OCTG_BslineDate", resourceCulture);

	public static string OCTG_CredLimit => ResourceManager.GetString("OCTG_CredLimit", resourceCulture);

	public static string OCTG_DiscCode => ResourceManager.GetString("OCTG_DiscCode", resourceCulture);

	public static string OCTG_ExtraDays => ResourceManager.GetString("OCTG_ExtraDays", resourceCulture);

	public static string OCTG_LatePyChrg => ResourceManager.GetString("OCTG_LatePyChrg", resourceCulture);

	public static string OCTG_ListNum => ResourceManager.GetString("OCTG_ListNum", resourceCulture);

	public static string OCTG_ObligLimit => ResourceManager.GetString("OCTG_ObligLimit", resourceCulture);

	public static string OCTG_OpenRcpt => ResourceManager.GetString("OCTG_OpenRcpt", resourceCulture);

	public static string OCTG_PymntGroup => ResourceManager.GetString("OCTG_PymntGroup", resourceCulture);

	public static string OCTG_Title_frm_PaymentTerm => ResourceManager.GetString("OCTG_Title_frm_PaymentTerm", resourceCulture);

	public static string OCTG_VolumDscnt => ResourceManager.GetString("OCTG_VolumDscnt", resourceCulture);

	public static string OHEM_EmpID => ResourceManager.GetString("OHEM_EmpID", resourceCulture);

	public static string OHEM_FirstName => ResourceManager.GetString("OHEM_FirstName", resourceCulture);

	public static string OHEM_LastName => ResourceManager.GetString("OHEM_LastName", resourceCulture);

	public static string OITB_ITMSGRPCOD => ResourceManager.GetString("OITB_ITMSGRPCOD", resourceCulture);

	public static string OITB_ITMSGRPNAM => ResourceManager.GetString("OITB_ITMSGRPNAM", resourceCulture);

	public static string OITB_Title_frm_ItemGroup => ResourceManager.GetString("OITB_Title_frm_ItemGroup", resourceCulture);

	public static string OITM_ActiveRangeFrom => ResourceManager.GetString("OITM_ActiveRangeFrom", resourceCulture);

	public static string OITM_Advanced => ResourceManager.GetString("OITM_Advanced", resourceCulture);

	public static string OITM_AvgPrice => ResourceManager.GetString("OITM_AvgPrice", resourceCulture);

	public static string OITM_BuyUnitMsr => ResourceManager.GetString("OITM_BuyUnitMsr", resourceCulture);

	public static string OITM_ByWh => ResourceManager.GetString("OITM_ByWh", resourceCulture);

	public static string OITM_CardCode => ResourceManager.GetString("OITM_CardCode", resourceCulture);

	public static string OITM_CodeBars => ResourceManager.GetString("OITM_CodeBars", resourceCulture);

	public static string OITM_EvalSystem => ResourceManager.GetString("OITM_EvalSystem", resourceCulture);

	public static string OITM_FirmCode => ResourceManager.GetString("OITM_FirmCode", resourceCulture);

	public static string OITM_FrgnName => ResourceManager.GetString("OITM_FrgnName", resourceCulture);

	public static string OITM_FrozenComm => ResourceManager.GetString("OITM_FrozenComm", resourceCulture);

	public static string OITM_FrozenFor => ResourceManager.GetString("OITM_FrozenFor", resourceCulture);

	public static string OITM_FrozenFrom => ResourceManager.GetString("OITM_FrozenFrom", resourceCulture);

	public static string OITM_FrozenTo => ResourceManager.GetString("OITM_FrozenTo", resourceCulture);

	public static string OITM_GLMethods => ResourceManager.GetString("OITM_GLMethods", resourceCulture);

	public static string OITM_InactiveRangeFrom => ResourceManager.GetString("OITM_InactiveRangeFrom", resourceCulture);

	public static string OITM_InventoryLevel => ResourceManager.GetString("OITM_InventoryLevel", resourceCulture);

	public static string OITM_InvntItem => ResourceManager.GetString("OITM_InvntItem", resourceCulture);

	public static string OITM_InvntryUom => ResourceManager.GetString("OITM_InvntryUom", resourceCulture);

	public static string OITM_ItemCode => ResourceManager.GetString("OITM_ItemCode", resourceCulture);

	public static string OITM_ItemName => ResourceManager.GetString("OITM_ItemName", resourceCulture);

	public static string OITM_ItemType => ResourceManager.GetString("OITM_ItemType", resourceCulture);

	public static string OITM_ItmsGrpCod => ResourceManager.GetString("OITM_ItmsGrpCod", resourceCulture);

	public static string OITM_lb_Racti => ResourceManager.GetString("OITM_lb_Racti", resourceCulture);

	public static string OITM_lb_Riacti => ResourceManager.GetString("OITM_lb_Riacti", resourceCulture);

	public static string OITM_lb_ToActi => ResourceManager.GetString("OITM_lb_ToActi", resourceCulture);

	public static string OITM_lb_ToInacti => ResourceManager.GetString("OITM_lb_ToInacti", resourceCulture);

	public static string OITM_ManageItemBy => ResourceManager.GetString("OITM_ManageItemBy", resourceCulture);

	public static string OITM_MaxLevel => ResourceManager.GetString("OITM_MaxLevel", resourceCulture);

	public static string OITM_MinLevel => ResourceManager.GetString("OITM_MinLevel", resourceCulture);

	public static string OITM_MngMethod => ResourceManager.GetString("OITM_MngMethod", resourceCulture);

	public static string OITM_NumInBuy => ResourceManager.GetString("OITM_NumInBuy", resourceCulture);

	public static string OITM_NumInSale => ResourceManager.GetString("OITM_NumInSale", resourceCulture);

	public static string OITM_PrchseItem => ResourceManager.GetString("OITM_PrchseItem", resourceCulture);

	public static string OITM_PurFactor1 => ResourceManager.GetString("OITM_PurFactor1", resourceCulture);

	public static string OITM_PurFactor2 => ResourceManager.GetString("OITM_PurFactor2", resourceCulture);

	public static string OITM_PurFactor3 => ResourceManager.GetString("OITM_PurFactor3", resourceCulture);

	public static string OITM_PurFactor4 => ResourceManager.GetString("OITM_PurFactor4", resourceCulture);

	public static string OITM_PurPackMsr => ResourceManager.GetString("OITM_PurPackMsr", resourceCulture);

	public static string OITM_PurPackUn => ResourceManager.GetString("OITM_PurPackUn", resourceCulture);

	public static string OITM_ReorderQty => ResourceManager.GetString("OITM_ReorderQty", resourceCulture);

	public static string OITM_SalFactor1 => ResourceManager.GetString("OITM_SalFactor1", resourceCulture);

	public static string OITM_SalFactor2 => ResourceManager.GetString("OITM_SalFactor2", resourceCulture);

	public static string OITM_SalFactor3 => ResourceManager.GetString("OITM_SalFactor3", resourceCulture);

	public static string OITM_SalFactor4 => ResourceManager.GetString("OITM_SalFactor4", resourceCulture);

	public static string OITM_SalPackMsr => ResourceManager.GetString("OITM_SalPackMsr", resourceCulture);

	public static string OITM_SalPackUn => ResourceManager.GetString("OITM_SalPackUn", resourceCulture);

	public static string OITM_SalUnitMsr => ResourceManager.GetString("OITM_SalUnitMsr", resourceCulture);

	public static string OITM_SellItem => ResourceManager.GetString("OITM_SellItem", resourceCulture);

	public static string OITM_SerialAndBatchNum => ResourceManager.GetString("OITM_SerialAndBatchNum", resourceCulture);

	public static string OITM_ShipType => ResourceManager.GetString("OITM_ShipType", resourceCulture);

	public static string OITM_SuppCatNum => ResourceManager.GetString("OITM_SuppCatNum", resourceCulture);

	public static string OITM_tab_Inventory => ResourceManager.GetString("OITM_tab_Inventory", resourceCulture);

	public static string OITM_tab_Purchase => ResourceManager.GetString("OITM_tab_Purchase", resourceCulture);

	public static string OITM_tab_Sales => ResourceManager.GetString("OITM_tab_Sales", resourceCulture);

	public static string OITM_Title_frm_FindItemMaster => ResourceManager.GetString("OITM_Title_frm_FindItemMaster", resourceCulture);

	public static string OITM_VailFrom => ResourceManager.GetString("OITM_VailFrom", resourceCulture);

	public static string OITM_VailTo => ResourceManager.GetString("OITM_VailTo", resourceCulture);

	public static string OITM_ValidComm => ResourceManager.GetString("OITM_ValidComm", resourceCulture);

	public static string OITM_validFor => ResourceManager.GetString("OITM_validFor", resourceCulture);

	public static string OITM_VatGourpSa => ResourceManager.GetString("OITM_VatGourpSa", resourceCulture);

	public static string OITM_VatGroupPu => ResourceManager.GetString("OITM_VatGroupPu", resourceCulture);

	public static string OITW_ctx_DeleteWhse => ResourceManager.GetString("OITW_ctx_DeleteWhse", resourceCulture);

	public static string OITW_ctx_SetDefaultWhse => ResourceManager.GetString("OITW_ctx_SetDefaultWhse", resourceCulture);

	public static string OITW_gcAvaiable => ResourceManager.GetString("OITW_gcAvaiable", resourceCulture);

	public static string OITW_gcCommited => ResourceManager.GetString("OITW_gcCommited", resourceCulture);

	public static string OITW_gcLocked => ResourceManager.GetString("OITW_gcLocked", resourceCulture);

	public static string OITW_gcOnHand => ResourceManager.GetString("OITW_gcOnHand", resourceCulture);

	public static string OITW_gcOnOrder => ResourceManager.GetString("OITW_gcOnOrder", resourceCulture);

	public static string OITW_gcPriceInventory => ResourceManager.GetString("OITW_gcPriceInventory", resourceCulture);

	public static string OITW_gcWhseCode => ResourceManager.GetString("OITW_gcWhseCode", resourceCulture);

	public static string OITW_gcWhseName => ResourceManager.GetString("OITW_gcWhseName", resourceCulture);

	public static string OMRC_FIRMCODE => ResourceManager.GetString("OMRC_FIRMCODE", resourceCulture);

	public static string OMRC_FIRMNAME => ResourceManager.GetString("OMRC_FIRMNAME", resourceCulture);

	public static string OMRC_Title_frm_Manufacturer => ResourceManager.GetString("OMRC_Title_frm_Manufacturer", resourceCulture);

	public static string OPLN_PriceListName => ResourceManager.GetString("OPLN_PriceListName", resourceCulture);

	public static string OPLN_PriceListNo => ResourceManager.GetString("OPLN_PriceListNo", resourceCulture);

	public static string OPLN_Title_frm_PriceList => ResourceManager.GetString("OPLN_Title_frm_PriceList", resourceCulture);

	public static string OPRM_authoriza => ResourceManager.GetString("OPRM_authoriza", resourceCulture);

	public static string OPRM_CopyAuth => ResourceManager.GetString("OPRM_CopyAuth", resourceCulture);

	public static string OPRM_FullAuth => ResourceManager.GetString("OPRM_FullAuth", resourceCulture);

	public static string OPRM_MenuName => ResourceManager.GetString("OPRM_MenuName", resourceCulture);

	public static string OPRM_NoAuth => ResourceManager.GetString("OPRM_NoAuth", resourceCulture);

	public static string OPRM_ReadOnly => ResourceManager.GetString("OPRM_ReadOnly", resourceCulture);

	public static string OPRQ_Department => ResourceManager.GetString("OPRQ_Department", resourceCulture);

	public static string OPRQ_DepartmentName => ResourceManager.GetString("OPRQ_DepartmentName", resourceCulture);

	public static string OPRQ_ReqName => ResourceManager.GetString("OPRQ_ReqName", resourceCulture);

	public static string OPRQ_Requester => ResourceManager.GetString("OPRQ_Requester", resourceCulture);

	public static string OSHP_Title_frmShipping => ResourceManager.GetString("OSHP_Title_frmShipping", resourceCulture);

	public static string OSHP_TrnspCode => ResourceManager.GetString("OSHP_TrnspCode", resourceCulture);

	public static string OSHP_TrnspName => ResourceManager.GetString("OSHP_TrnspName", resourceCulture);

	public static string OSHP_WebSite => ResourceManager.GetString("OSHP_WebSite", resourceCulture);

	public static string OSPP_AutoUpdt => ResourceManager.GetString("OSPP_AutoUpdt", resourceCulture);

	public static string OSPP_Discount => ResourceManager.GetString("OSPP_Discount", resourceCulture);

	public static string OSPP_ListNum => ResourceManager.GetString("OSPP_ListNum", resourceCulture);

	public static string OSPP_Price => ResourceManager.GetString("OSPP_Price", resourceCulture);

	public static string OSPP_Titel_frm_SpecialforBP => ResourceManager.GetString("OSPP_Titel_frm_SpecialforBP", resourceCulture);

	public static string OUBR_Branch => ResourceManager.GetString("OUBR_Branch", resourceCulture);

	public static string OUBR_BranchName => ResourceManager.GetString("OUBR_BranchName", resourceCulture);

	public static string OUDP_Code => ResourceManager.GetString("OUDP_Code", resourceCulture);

	public static string OUDP_Name => ResourceManager.GetString("OUDP_Name", resourceCulture);

	public static string OUDP_Remarks => ResourceManager.GetString("OUDP_Remarks", resourceCulture);

	public static string OUSR_U_NAME => ResourceManager.GetString("OUSR_U_NAME", resourceCulture);

	public static string OUSR_USER_CODE => ResourceManager.GetString("OUSR_USER_CODE", resourceCulture);

	public static string OUSR_USERID => ResourceManager.GetString("OUSR_USERID", resourceCulture);

	public static string OUSR_UserSign => ResourceManager.GetString("OUSR_UserSign", resourceCulture);

	public static string OVTG_Category => ResourceManager.GetString("OVTG_Category", resourceCulture);

	public static string OVTG_Code => ResourceManager.GetString("OVTG_Code", resourceCulture);

	public static string OVTG_EffecDate => ResourceManager.GetString("OVTG_EffecDate", resourceCulture);

	public static string OVTG_Name => ResourceManager.GetString("OVTG_Name", resourceCulture);

	public static string OVTG_Rate => ResourceManager.GetString("OVTG_Rate", resourceCulture);

	public static string OVTG_TaxAccount => ResourceManager.GetString("OVTG_TaxAccount", resourceCulture);

	public static string OVTG_Title_frm_TaxDenfine => ResourceManager.GetString("OVTG_Title_frm_TaxDenfine", resourceCulture);

	public static string OWHS_Block => ResourceManager.GetString("OWHS_Block", resourceCulture);

	public static string OWHS_BuildingFloorRoom => ResourceManager.GetString("OWHS_BuildingFloorRoom", resourceCulture);

	public static string OWHS_City => ResourceManager.GetString("OWHS_City", resourceCulture);

	public static string OWHS_County => ResourceManager.GetString("OWHS_County", resourceCulture);

	public static string OWHS_DropShip => ResourceManager.GetString("OWHS_DropShip", resourceCulture);

	public static string OWHS_FederalTaxID => ResourceManager.GetString("OWHS_FederalTaxID", resourceCulture);

	public static string OWHS_Location => ResourceManager.GetString("OWHS_Location", resourceCulture);

	public static string OWHS_Locked => ResourceManager.GetString("OWHS_Locked", resourceCulture);

	public static string OWHS_Nettable => ResourceManager.GetString("OWHS_Nettable", resourceCulture);

	public static string OWHS_ObjTypeHistory => ResourceManager.GetString("OWHS_ObjTypeHistory", resourceCulture);

	public static string OWHS_Street => ResourceManager.GetString("OWHS_Street", resourceCulture);

	public static string OWHS_WhsCode => ResourceManager.GetString("OWHS_WhsCode", resourceCulture);

	public static string OWHS_WhsName => ResourceManager.GetString("OWHS_WhsName", resourceCulture);

	public static string OWHS_ZipCode => ResourceManager.GetString("OWHS_ZipCode", resourceCulture);

	public static string tab_Attachment => ResourceManager.GetString("tab_Attachment", resourceCulture);

	public static string tab_Capacity => ResourceManager.GetString("tab_Capacity", resourceCulture);

	public static string tab_Closed => ResourceManager.GetString("tab_Closed", resourceCulture);

	public static string tab_Competitor => ResourceManager.GetString("tab_Competitor", resourceCulture);

	public static string tab_Components => ResourceManager.GetString("tab_Components", resourceCulture);

	public static string tab_ContactPerson => ResourceManager.GetString("tab_ContactPerson", resourceCulture);

	public static string tab_Contents => ResourceManager.GetString("tab_Contents", resourceCulture);

	public static string tab_Costing => ResourceManager.GetString("tab_Costing", resourceCulture);

	public static string tab_FixedAsset => ResourceManager.GetString("tab_FixedAsset", resourceCulture);

	public static string tab_General => ResourceManager.GetString("tab_General", resourceCulture);

	public static string tab_GeneralMO => ResourceManager.GetString("tab_GeneralMO", resourceCulture);

	public static string tab_Genral => ResourceManager.GetString("tab_Genral", resourceCulture);

	public static string tab_ItemDetail => ResourceManager.GetString("tab_ItemDetail", resourceCulture);

	public static string tab_Partner => ResourceManager.GetString("tab_Partner", resourceCulture);

	public static string tab_PaymentTerms => ResourceManager.GetString("tab_PaymentTerms", resourceCulture);

	public static string tab_Planed => ResourceManager.GetString("tab_Planed", resourceCulture);

	public static string tab_Potential => ResourceManager.GetString("tab_Potential", resourceCulture);

	public static string tab_ReceiptFromProduction => ResourceManager.GetString("tab_ReceiptFromProduction", resourceCulture);

	public static string tab_Released => ResourceManager.GetString("tab_Released", resourceCulture);

	public static string tab_Remark => ResourceManager.GetString("tab_Remark", resourceCulture);

	public static string tab_ResourceLoad => ResourceManager.GetString("tab_ResourceLoad", resourceCulture);

	public static string tab_Summary => ResourceManager.GetString("tab_Summary", resourceCulture);

	public static string tab_UserDefineF => ResourceManager.GetString("tab_UserDefineF", resourceCulture);

	public static string Titel_frm_Atp => ResourceManager.GetString("Titel_frm_Atp", resourceCulture);

	public static string Titel_frm_SalesOpportunityRelationship => ResourceManager.GetString("Titel_frm_SalesOpportunityRelationship", resourceCulture);

	public static string Titel_frmActivitiesOverviewBP => ResourceManager.GetString("Titel_frmActivitiesOverviewBP", resourceCulture);

	public static string Titel_frmActivityListForSalesOpportunity => ResourceManager.GetString("Titel_frmActivityListForSalesOpportunity", resourceCulture);

	public static string Titel_frmActivityMeetingLocation => ResourceManager.GetString("Titel_frmActivityMeetingLocation", resourceCulture);

	public static string Titel_frmActivityStatus => ResourceManager.GetString("Titel_frmActivityStatus", resourceCulture);

	public static string Titel_frmActivitySubjects => ResourceManager.GetString("Titel_frmActivitySubjects", resourceCulture);

	public static string Titel_frmActivityType => ResourceManager.GetString("Titel_frmActivityType", resourceCulture);

	public static string Titel_frmAsignMaterialToOperation => ResourceManager.GetString("Titel_frmAsignMaterialToOperation", resourceCulture);

	public static string Titel_frmAssignComponent => ResourceManager.GetString("Titel_frmAssignComponent", resourceCulture);

	public static string Titel_frmAuthorization => ResourceManager.GetString("Titel_frmAuthorization", resourceCulture);

	public static string Titel_frmBOM => ResourceManager.GetString("Titel_frmBOM", resourceCulture);

	public static string Titel_frmChangePassword => ResourceManager.GetString("Titel_frmChangePassword", resourceCulture);

	public static string Titel_frmChooseDfltBranch => ResourceManager.GetString("Titel_frmChooseDfltBranch", resourceCulture);

	public static string Titel_frmControlKey => ResourceManager.GetString("Titel_frmControlKey", resourceCulture);

	public static string Titel_frmDepartment => ResourceManager.GetString("Titel_frmDepartment", resourceCulture);

	public static string Titel_frmGroupControlKey => ResourceManager.GetString("Titel_frmGroupControlKey", resourceCulture);

	public static string Titel_frmListPlant => ResourceManager.GetString("Titel_frmListPlant", resourceCulture);

	public static string Titel_frmMasterOrderReplan => ResourceManager.GetString("Titel_frmMasterOrderReplan", resourceCulture);

	public static string Titel_frmOperation => ResourceManager.GetString("Titel_frmOperation", resourceCulture);

	public static string Titel_frmReportViewer => ResourceManager.GetString("Titel_frmReportViewer", resourceCulture);

	public static string Titel_frmRouting => ResourceManager.GetString("Titel_frmRouting", resourceCulture);

	public static string Titel_frmSalesOpportunityCompetitor => ResourceManager.GetString("Titel_frmSalesOpportunityCompetitor", resourceCulture);

	public static string Titel_frmSalesOpportunityPartner => ResourceManager.GetString("Titel_frmSalesOpportunityPartner", resourceCulture);

	public static string Titel_frmSalesOpportunityReason => ResourceManager.GetString("Titel_frmSalesOpportunityReason", resourceCulture);

	public static string Titel_frmSalesOpportunityStage => ResourceManager.GetString("Titel_frmSalesOpportunityStage", resourceCulture);

	public static string Titel_frmUsers => ResourceManager.GetString("Titel_frmUsers", resourceCulture);

	public static string Titel_ListOfAccount => ResourceManager.GetString("Titel_ListOfAccount", resourceCulture);

	public static string Titel_ListOfBillOfMaterials => ResourceManager.GetString("Titel_ListOfBillOfMaterials", resourceCulture);

	public static string Titel_ListOfBoMResource => ResourceManager.GetString("Titel_ListOfBoMResource", resourceCulture);

	public static string Titel_ListOfBPCatalogNo => ResourceManager.GetString("Titel_ListOfBPCatalogNo", resourceCulture);

	public static string Titel_ListOfBusinessPartner => ResourceManager.GetString("Titel_ListOfBusinessPartner", resourceCulture);

	public static string Titel_ListOfBusinessVenDor => ResourceManager.GetString("Titel_ListOfBusinessVenDor", resourceCulture);

	public static string Titel_ListOfContactPerson => ResourceManager.GetString("Titel_ListOfContactPerson", resourceCulture);

	public static string Titel_ListOfCustomers => ResourceManager.GetString("Titel_ListOfCustomers", resourceCulture);

	public static string Titel_ListOfDimensionCostCenter => ResourceManager.GetString("Titel_ListOfDimensionCostCenter", resourceCulture);

	public static string Titel_ListOfDisassemblyProductionOrders => ResourceManager.GetString("Titel_ListOfDisassemblyProductionOrders", resourceCulture);

	public static string Titel_ListOfEmployee => ResourceManager.GetString("Titel_ListOfEmployee", resourceCulture);

	public static string Titel_ListOfItem => ResourceManager.GetString("Titel_ListOfItem", resourceCulture);

	public static string Titel_ListOfOperation => ResourceManager.GetString("Titel_ListOfOperation", resourceCulture);

	public static string Titel_ListOfPriceList => ResourceManager.GetString("Titel_ListOfPriceList", resourceCulture);

	public static string Titel_ListOfPurchaseRequest => ResourceManager.GetString("Titel_ListOfPurchaseRequest", resourceCulture);

	public static string Titel_ListOfRequester => ResourceManager.GetString("Titel_ListOfRequester", resourceCulture);

	public static string Titel_ListOfResource => ResourceManager.GetString("Titel_ListOfResource", resourceCulture);

	public static string Titel_ListOfRouting => ResourceManager.GetString("Titel_ListOfRouting", resourceCulture);

	public static string Titel_ListofStandardProductionOrders => ResourceManager.GetString("Titel_ListofStandardProductionOrders", resourceCulture);

	public static string Titel_ListOfUser => ResourceManager.GetString("Titel_ListOfUser", resourceCulture);

	public static string Titel_ListOfVatGroups => ResourceManager.GetString("Titel_ListOfVatGroups", resourceCulture);

	public static string Titel_ListOfWarehouses => ResourceManager.GetString("Titel_ListOfWarehouses", resourceCulture);

	public static string Titel_SalesOpportunity => ResourceManager.GetString("Titel_SalesOpportunity", resourceCulture);

	public static string Titel_SettingPassword => ResourceManager.GetString("Titel_SettingPassword", resourceCulture);

	public static string Title_frmBankAccount => ResourceManager.GetString("Title_frmBankAccount", resourceCulture);

	public static string Title_frmInventoryTransfer => ResourceManager.GetString("Title_frmInventoryTransfer", resourceCulture);

	public static string Title_frmListTranferRequest => ResourceManager.GetString("Title_frmListTranferRequest", resourceCulture);

	public static string Title_frmLocation => ResourceManager.GetString("Title_frmLocation", resourceCulture);

	public static string Title_frmProductionOder => ResourceManager.GetString("Title_frmProductionOder", resourceCulture);

	public static string Title_frmProductionPlanning => ResourceManager.GetString("Title_frmProductionPlanning", resourceCulture);

	public static string Title_frmProductionScheduling => ResourceManager.GetString("Title_frmProductionScheduling", resourceCulture);

	public static string Title_frmProject => ResourceManager.GetString("Title_frmProject", resourceCulture);

	public static string Title_frmReceiptFromProduction => ResourceManager.GetString("Title_frmReceiptFromProduction", resourceCulture);

	public static string Title_frmResources => ResourceManager.GetString("Title_frmResources", resourceCulture);

	public static string Title_frmSaleEmployee => ResourceManager.GetString("Title_frmSaleEmployee", resourceCulture);

	public static string Title_frmWareHourse => ResourceManager.GetString("Title_frmWareHourse", resourceCulture);

	public static string xtra_DoYouWantCreateInventoryTransferRequest => ResourceManager.GetString("xtra_DoYouWantCreateInventoryTransferRequest", resourceCulture);

	public static string xtra_DoYouWantCreatePurchaseRequest => ResourceManager.GetString("xtra_DoYouWantCreatePurchaseRequest", resourceCulture);

	public static string xtra_Question => ResourceManager.GetString("xtra_Question", resourceCulture);

	public static string xtraMsg_AreYouWantToEx => ResourceManager.GetString("xtraMsg_AreYouWantToEx", resourceCulture);

	public static string xtraMsg_Cancel => ResourceManager.GetString("xtraMsg_Cancel", resourceCulture);

	public static string xtraMsg_Close => ResourceManager.GetString("xtraMsg_Close", resourceCulture);

	public static string xtraMsg_Delete => ResourceManager.GetString("xtraMsg_Delete", resourceCulture);

	public static string xtraMsg_Save => ResourceManager.GetString("xtraMsg_Save", resourceCulture);

	public static string xtraMsg_SaveChange => ResourceManager.GetString("xtraMsg_SaveChange", resourceCulture);

	internal Resources()
	{
	}
}
