using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.ApprovalProcess;
using Apzon.Forms.Administrator.Banking;
using Apzon.Forms.Administrator.Business_Partners;
using Apzon.Forms.Administrator.Plugin;
using Apzon.Forms.Administrator.SetUp;
using Apzon.Forms.Administrator.SetUp.Inventory;
using Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;
using Apzon.Forms.Administrator.SetUp.Purchasing;
using Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers;
using Apzon.Forms.Administrator.SetUp.Service;
using Apzon.Forms.Administrator.SetUp.Tax;
using Apzon.Forms.Administrator.SystemInitialization;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons.UDF;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Commons.UDT;
using Apzon.Forms.DMS;
using Apzon.Forms.Dashboard;
using Apzon.Forms.FacebookCRM;
using Apzon.Forms.Facebook_CRM;
using Apzon.Forms.Financials;
using Apzon.Forms.FormTemplate;
using Apzon.Forms.Helper;
using Apzon.Forms.Helper.HubHelpers;
using Apzon.Forms.HumanResources;
using Apzon.Forms.HumanResources.Salary;
using Apzon.Forms.HumanResources.TimeSheeting;
using Apzon.Forms.ImportData;
using Apzon.Forms.Integrated;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Inventory.InventoryReport;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Inventory.Item_Management;
using Apzon.Forms.Inventory.Item_Management.Batches;
using Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers;
using Apzon.Forms.Inventory.Pick_and_Pack;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.KPIs;
using Apzon.Forms.MTO_Manufacturing;
using Apzon.Forms.MTS_Manufacturing;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Packing_Management;
using Apzon.Forms.Payment;
using Apzon.Forms.Production;
using Apzon.Forms.Promotions;
using Apzon.Forms.Purchase;
using Apzon.Forms.Reports;
using Apzon.Forms.Retail;
using Apzon.Forms.RoutingAndResource;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.SalesOppotunity.SetUp.Opportunity;
using Apzon.Forms.Service;
using Apzon.Forms.Service.MarketingCampaign;
using Apzon.Forms.UserControl;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.Notification;
using Apzon.Forms.UserControls.POS_UI;
using Apzon.Forms.Widget;
using Apzon.Forms.integrated;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Apzon.Plugin;
using Commons.Helper;
using DevExpress.Data;
using DevExpress.Images;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Svg;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace Apzon.Forms.Commons;

public class Main : XtraFormEx
{
	public class KeystrokMessageFilter : IMessageFilter
	{
		private readonly Main _frmMain;

		private const int WM_LBUTTONDOWN = 513;

		public KeystrokMessageFilter(Main frmMain)
		{
			_frmMain = frmMain;
		}

		public bool PreFilterMessage(ref System.Windows.Forms.Message m)
		{
			try
			{
				if (m.Msg == 256)
				{
					_frmMain.LastActiveTime = DateTime.Now;
					if ((int)m.WParam == 27)
					{
						_frmMain.CloseChildForm();
					}
					switch ((int)((uint)(int)m.WParam | (uint)Control.ModifierKeys))
					{
					case 131137:
						_frmMain.AddNew();
						break;
					case 131142:
						_frmMain.Find();
						break;
					case 131148:
						_frmMain.bar_PreviewLayouts_ItemClick(null, null);
						break;
					}
				}
				else if (m.Msg == 260)
				{
					if (((int)m.LParam & 0x20000000) != 0 && m.WParam == new IntPtr(115) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmExit, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return true;
					}
				}
				else if (m.Msg == 513)
				{
					_frmMain.LastActiveTime = DateTime.Now;
					_frmMain.HideNotificationContainerControl();
				}
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}
	}

	public static bool IsImportLicense;

	private bool _relogin;

	private const int MsgInterval = 100;

	public static string UserName = string.Empty;

	internal static bool Exit = false;

	public static bool IsRestart = false;

	private const int delta = 25;

	private Color backColor;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oOrders);

	private static HubClientHelper CheckOnlineHub;

	private static HubClientHelper UpdateNotiHub;

	private Form _activeForm = new Form();

	public static int WidScreen = Screen.PrimaryScreen.WorkingArea.Width;

	public static int HeightScreen = Screen.PrimaryScreen.WorkingArea.Height;

	public DateTime LastActiveTime;

	private bool _getAlterMessage;

	private readonly Panel _lockPanel = new Panel();

	private readonly System.Windows.Forms.Timer timer_AlertEvent;

	public static Dictionary<string, string> FormType = new Dictionary<string, string>
	{
		{ "frm_JobsManagement", "Apzon.Forms.frm_JobsManagement, Apzon.Forms" },
		{ "frm_Retail", "Apzon.Forms.frm_Retail, Apzon.Forms" },
		{ "frm_LogViewer", "Apzon.Forms.frm_LogViewer, Apzon.Forms" },
		{ "frm_ServerAdress", "Apzon.Forms.frm_ServerAdress, Apzon.Forms" },
		{ "frm_Test", "Apzon.Forms.frm_Test, Apzon.Forms" },
		{ "frm_LoginSap", "Apzon.Forms.frm_LoginSap, Apzon.Forms" },
		{ "frm_AddGroup", "Apzon.Forms.Widget.frm_AddGroup, Apzon.Forms" },
		{ "frm_HomePage", "Apzon.Forms.Widget.frm_HomePage, Apzon.Forms" },
		{ "frm_Widget", "Apzon.Forms.Widget.frm_Widget, Apzon.Forms" },
		{ "frm_WidgetAddItem", "Apzon.Forms.Widget.frm_WidgetAddItem, Apzon.Forms" },
		{ "frm_WidgetManagement", "Apzon.Forms.Widget.frm_WidgetManagement, Apzon.Forms" },
		{ "frm_ActivitiesListForOpportunity", "Apzon.Forms.SalesOppotunity.frm_ActivitiesListForOpportunity, Apzon.Forms" },
		{ "frm_ActivitiesOverviewBP", "Apzon.Forms.SalesOppotunity.frm_ActivitiesOverviewBP, Apzon.Forms" },
		{ "frm_ActivityListForSalesOpportunity", "Apzon.Forms.SalesOppotunity.frm_ActivityListForSalesOpportunity, Apzon.Forms" },
		{ "frm_ActivitiesForOpportunity", "Apzon.Forms.SalesOppotunity.frm_ActivitiesForOpportunity, Apzon.Forms" },
		{ "frm_OpportunityLinkedDocument", "Apzon.Forms.SalesOppotunity.frm_OpportunityLinkedDocument, Apzon.Forms" },
		{ "frm_SalesOpportunity", "Apzon.Forms.SalesOppotunity.frm_SalesOpportunity, Apzon.Forms" },
		{ "frm_TaskManagement", "Apzon.Forms.SalesOppotunity.frm_TaskManagement, Apzon.Forms" },
		{ "frm_Interest", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_Interest, Apzon.Forms" },
		{ "frm_InterestLevel", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_InterestLevel, Apzon.Forms" },
		{ "frm_SalesOpportunityCompetitor", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_SalesOpportunityCompetitor, Apzon.Forms" },
		{ "frm_SalesOpportunityPartner", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_SalesOpportunityPartner, Apzon.Forms" },
		{ "frm_SalesOpportunityReason", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_SalesOpportunityReason, Apzon.Forms" },
		{ "frm_SalesOpportunityRelationship", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_SalesOpportunityRelationship, Apzon.Forms" },
		{ "frm_SalesOpportunityStage", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.frm_SalesOpportunityStage, Apzon.Forms" },
		{ "VirSubFormOpportunitySetup", "Apzon.Forms.SalesOppotunity.SetUp.Opportunity.VirSubFormOpportunitySetup, Apzon.Forms" },
		{ "frm_ActivityMeetingLocation", "Apzon.Forms.SalesOppotunity.SetUp.Activity.frm_ActivityMeetingLocation, Apzon.Forms" },
		{ "frm_ActivityStatus", "Apzon.Forms.SalesOppotunity.SetUp.Activity.frm_ActivityStatus, Apzon.Forms" },
		{ "frm_ActivitySubjects", "Apzon.Forms.SalesOppotunity.SetUp.Activity.frm_ActivitySubjects, Apzon.Forms" },
		{ "frm_ActivityType", "Apzon.Forms.SalesOppotunity.SetUp.Activity.frm_ActivityType, Apzon.Forms" },
		{ "VirSubFormActivitySetup", "Apzon.Forms.SalesOppotunity.SetUp.Activity.VirSubFormActivitySetup, Apzon.Forms" },
		{ "frm_PaymentForInvoice", "Apzon.Forms.Banking.frm_PaymentForInvoice, Apzon.Forms" },
		{ "FrmCflMps", "Apzon.Forms.MTS_Manufacturing.FrmCflMps, Apzon.Forms" },
		{ "FrmCflMrp", "Apzon.Forms.MTS_Manufacturing.FrmCflMrp, Apzon.Forms" },
		{ "FrmCflProductionOrder", "Apzon.Forms.MTS_Manufacturing.FrmCflProductionOrder, Apzon.Forms" },
		{ "FrmDefinedShift", "Apzon.Forms.MTS_Manufacturing.FrmDefinedShift, Apzon.Forms" },
		{ "FrmListMps", "Apzon.Forms.MTS_Manufacturing.FrmListMps, Apzon.Forms" },
		{ "FrmMps", "Apzon.Forms.MTS_Manufacturing.FrmMps, Apzon.Forms" },
		{ "FrmMrp", "Apzon.Forms.MTS_Manufacturing.FrmMrp, Apzon.Forms" },
		{ "FrmMrpFuture", "Apzon.Forms.MTS_Manufacturing.FrmMrpFuture, Apzon.Forms" },
		{ "FrmMrpList", "Apzon.Forms.MTS_Manufacturing.FrmMrpList, Apzon.Forms" },
		{ "FrmWorkScheduling", "Apzon.Forms.MTS_Manufacturing.FrmWorkScheduling, Apzon.Forms" },
		{ "frm_AsignMaterialToOperation", "Apzon.Forms.MTO_Manufacturing.frm_AsignMaterialToOperation, Apzon.Forms" },
		{ "frm_AssignComponent", "Apzon.Forms.MTO_Manufacturing.frm_AssignComponent, Apzon.Forms" },
		{ "frm_BOM", "Apzon.Forms.MTO_Manufacturing.frm_BOM, Apzon.Forms" },
		{ "frm_ControlKey", "Apzon.Forms.MTO_Manufacturing.frm_ControlKey, Apzon.Forms" },
		{ "frm_GroupControlKey", "Apzon.Forms.MTO_Manufacturing.frm_GroupControlKey, Apzon.Forms" },
		{ "frm_ListPlant", "Apzon.Forms.MTO_Manufacturing.frm_ListPlant, Apzon.Forms" },
		{ "frm_MasterOrderReplan", "Apzon.Forms.MTO_Manufacturing.frm_MasterOrderReplan, Apzon.Forms" },
		{ "frm_PackingBatch", "Apzon.Forms.MTO_Manufacturing.frm_PackingBatch, Apzon.Forms" },
		{ "frm_ProductionConfirm", "Apzon.Forms.MTO_Manufacturing.frm_ProductionConfirm, Apzon.Forms" },
		{ "frm_ProductionControl", "Apzon.Forms.MTO_Manufacturing.frm_ProductionControl, Apzon.Forms" },
		{ "frm_ProductionForBatch", "Apzon.Forms.MTO_Manufacturing.frm_ProductionForBatch, Apzon.Forms" },
		{ "frm_ProductionPlanning", "Apzon.Forms.MTO_Manufacturing.frm_ProductionPlanning, Apzon.Forms" },
		{ "frm_ProductionReqForBatch", "Apzon.Forms.MTO_Manufacturing.frm_ProductionReqForBatch, Apzon.Forms" },
		{ "frm_ProductionScheduling", "Apzon.Forms.MTO_Manufacturing.frm_ProductionScheduling, Apzon.Forms" },
		{ "frm_ReportCompleteForProduction", "Apzon.Forms.MTO_Manufacturing.frm_ReportCompleteForProduction, Apzon.Forms" },
		{ "frm_SpecialIssueForProductionByBatch", "Apzon.Forms.MTO_Manufacturing.frm_SpecialIssueForProductionByBatch, Apzon.Forms" },
		{ "frm_Shipping", "Apzon.Forms.ItemsMaster.frm_Shipping, Apzon.Forms" },
		{ "frm_FindItem", "Apzon.Forms.ItemsMaster.frm_FindItem, Apzon.Forms" },
		{ "frm_ItemMasterData", "Apzon.Forms.ItemsMaster.frm_ItemMasterData, Apzon.Forms" },
		{ "frm_ItemPreferredVendor", "Apzon.Forms.ItemsMaster.frm_ItemPreferredVendor, Apzon.Forms" },
		{ "frm_Items", "Apzon.Forms.ItemsMaster.frm_Items, Apzon.Forms" },
		{ "frm_Manufacturer", "Apzon.Forms.ItemsMaster.frm_Manufacturer, Apzon.Forms" },
		{ "frm_OrderInterval", "Apzon.Forms.ItemsMaster.frm_OrderInterval, Apzon.Forms" },
		{ "frm_SpecialforBP", "Apzon.Forms.ItemsMaster.frm_SpecialforBP, Apzon.Forms" },
		{ "frm_SpecialforBP_LineSPP1", "Apzon.Forms.ItemsMaster.frm_SpecialforBP_LineSPP1, Apzon.Forms" },
		{ "frm_SpecialforBP_LineSPP2", "Apzon.Forms.ItemsMaster.frm_SpecialforBP_LineSPP2, Apzon.Forms" },
		{ "frm_UoMInItem", "Apzon.Forms.ItemsMaster.frm_UoMInItem, Apzon.Forms" },
		{ "LookUpEditComboPopupForm", "Apzon.Forms.UserControls.LookUpEditComboPopupForm, Apzon.Forms" },
		{ "LookUpEditDefineNewPopupForm", "Apzon.Forms.UserControls.LookUpEditDefineNewPopupForm, Apzon.Forms" },
		{ "LookUpEditUDFPopupForm", "Apzon.Forms.UserControls.LookUpEditUDFPopupForm, Apzon.Forms" },
		{ "frm_Dialog", "Apzon.Forms.UserControls.POS_UI.frm_Dialog, Apzon.Forms" },
		{ "frm_PaymentDialog", "Apzon.Forms.UserControls.POS_UI.frm_PaymentDialog, Apzon.Forms" },
		{ "frm_PosProcessBar", "Apzon.Forms.UserControls.POS_UI.frm_PosProcessBar, Apzon.Forms" },
		{ "RetailMessageBoxContent", "Apzon.Forms.UserControls.POS_UI.RetailMessageBoxContent, Apzon.Forms" },
		{ "frm_AutoCreationBatchSerial", "Apzon.Forms.UserControls.BatchAndSerial.frm_AutoCreationBatchSerial, Apzon.Forms" },
		{ "frm_AutomaticStringCreation", "Apzon.Forms.UserControls.BatchAndSerial.frm_AutomaticStringCreation, Apzon.Forms" },
		{ "frm_AllowcateSerialBatch", "Apzon.Forms.UserControls.BatchAndSerial.frm_AllowcateSerialBatch, Apzon.Forms" },
		{ "frm_ServiceCalls", "Apzon.Forms.Service.frm_ServiceCalls, Apzon.Forms" },
		{ "frm_ServiceContractTemplate", "Apzon.Forms.Service.frm_ServiceContractTemplate, Apzon.Forms" },
		{ "frm_ServiceLevelAgreement", "Apzon.Forms.Service.frm_ServiceLevelAgreement, Apzon.Forms" },
		{ "frm_WarrantyCard", "Apzon.Forms.Service.frm_WarrantyCard, Apzon.Forms" },
		{ "frm_ConfirmDialog", "Apzon.Forms.Service.frm_ConfirmDialog, Apzon.Forms" },
		{ "frm_Contract", "Apzon.Forms.Service.frm_Contract, Apzon.Forms" },
		{ "frm_CopyTaskSetting", "Apzon.Forms.Service.frm_CopyTaskSetting, Apzon.Forms" },
		{ "frm_EventManagement", "Apzon.Forms.Service.frm_EventManagement, Apzon.Forms" },
		{ "Frm_ServiceCall", "Apzon.Forms.Service.Frm_ServiceCall, Apzon.Forms" },
		{ "frm_ServiceCallSetting", "Apzon.Forms.Service.frm_ServiceCallSetting, Apzon.Forms" },
		{ "frm_Solution", "Apzon.Forms.Service.frm_Solution, Apzon.Forms" },
		{ "frm_Task", "Apzon.Forms.Service.frm_Task, Apzon.Forms" },
		{ "frm_MarketingCampaign", "Apzon.Forms.Service.MarketingCampaign.frm_MarketingCampaign, Apzon.Forms" },
		{ "frm_DownPaymentRequest", "Apzon.Forms.Sales.frm_DownPaymentRequest, Apzon.Forms" },
		{ "frm_GoodsReturnRequest", "Apzon.Forms.Sales.frm_GoodsReturnRequest, Apzon.Forms" },
		{ "frm_ReturnRequest", "Apzon.Forms.Sales.frm_ReturnRequest, Apzon.Forms" },
		{ "frm_ARCreditMemos", "Apzon.Forms.Sales.frm_ARCreditMemos, Apzon.Forms" },
		{ "frm_ARInvoices", "Apzon.Forms.Sales.frm_ARInvoices, Apzon.Forms" },
		{ "frm_AR_ReserveInvoices", "Apzon.Forms.Sales.frm_AR_ReserveInvoices, Apzon.Forms" },
		{ "frm_Atp", "Apzon.Forms.Sales.frm_Atp, Apzon.Forms" },
		{ "frm_AvailableToPromise", "Apzon.Forms.Sales.frm_AvailableToPromise, Apzon.Forms" },
		{ "frm_ChooseBatchForQuotation", "Apzon.Forms.Sales.frm_ChooseBatchForQuotation, Apzon.Forms" },
		{ "frm_Delivery", "Apzon.Forms.Sales.frm_Delivery, Apzon.Forms" },
		{ "frm_DeliveryConfirm", "Apzon.Forms.Sales.frm_DeliveryConfirm, Apzon.Forms" },
		{ "frm_DeliveryControl", "Apzon.Forms.Sales.frm_DeliveryControl, Apzon.Forms" },
		{ "frm_DeliveryOrder", "Apzon.Forms.Sales.frm_DeliveryOrder, Apzon.Forms" },
		{ "frm_DocumentDraftsRP", "Apzon.Forms.Sales.frm_DocumentDraftsRP, Apzon.Forms" },
		{ "frm_GenProductionFormAtp", "Apzon.Forms.Sales.frm_GenProductionFormAtp, Apzon.Forms" },
		{ "frm_GenPurchaseFromAtp", "Apzon.Forms.Sales.frm_GenPurchaseFromAtp, Apzon.Forms" },
		{ "frm_HandoverDocument", "Apzon.Forms.Sales.frm_HandoverDocument, Apzon.Forms" },
		{ "frm_ListDocumentDraft", "Apzon.Forms.Sales.frm_ListDocumentDraft, Apzon.Forms" },
		{ "frm_Return", "Apzon.Forms.Sales.frm_Return, Apzon.Forms" },
		{ "frm_SalesBlanketAgreement", "Apzon.Forms.Sales.frm_SalesBlanketAgreement, Apzon.Forms" },
		{ "frm_SalesBlanketAgreement_RowDetails", "Apzon.Forms.Sales.frm_SalesBlanketAgreement_RowDetails, Apzon.Forms" },
		{ "frm_SalesControl", "Apzon.Forms.Sales.frm_SalesControl, Apzon.Forms" },
		{ "frm_SalesDraftsReport", "Apzon.Forms.Sales.frm_SalesDraftsReport, Apzon.Forms" },
		{ "frm_SalesOrder", "Apzon.Forms.Sales.frm_SalesOrder, Apzon.Forms" },
		{ "frm_SalesQuotationForBatch", "Apzon.Forms.Sales.frm_SalesQuotationForBatch, Apzon.Forms" },
		{ "frm_ScheduleForAtp", "Apzon.Forms.Sales.frm_ScheduleForAtp, Apzon.Forms" },
		{ "frm_ShippingDocumentLoaded", "Apzon.Forms.Sales.frm_ShippingDocumentLoaded, Apzon.Forms" },
		{ "frm_ShippingDocumentOutboundRoute", "Apzon.Forms.Sales.frm_ShippingDocumentOutboundRoute, Apzon.Forms" },
		{ "frm_ShippingDocumentPicked", "Apzon.Forms.Sales.frm_ShippingDocumentPicked, Apzon.Forms" },
		{ "frm_AdvanceRouting", "Apzon.Forms.RoutingAndResource.frm_AdvanceRouting, Apzon.Forms" },
		{ "frm_AppointmentEditting", "Apzon.Forms.RoutingAndResource.frm_AppointmentEditting, Apzon.Forms" },
		{ "frm_CapacityGantt", "Apzon.Forms.RoutingAndResource.frm_CapacityGantt, Apzon.Forms" },
		{ "frm_CapacitySetting", "Apzon.Forms.RoutingAndResource.frm_CapacitySetting, Apzon.Forms" },
		{ "frm_ImportRouting", "Apzon.Forms.RoutingAndResource.frm_ImportRouting, Apzon.Forms" },
		{ "frm_Operations", "Apzon.Forms.RoutingAndResource.frm_Operations, Apzon.Forms" },
		{ "frm_ResourceGroup", "Apzon.Forms.RoutingAndResource.frm_ResourceGroup, Apzon.Forms" },
		{ "frm_ResourceWorkCenter", "Apzon.Forms.RoutingAndResource.frm_ResourceWorkCenter, Apzon.Forms" },
		{ "frm_ResourceWorkScheduling", "Apzon.Forms.RoutingAndResource.frm_ResourceWorkScheduling, Apzon.Forms" },
		{ "frm_Operation", "Apzon.Forms.RoutingAndResource.frm_Operation, Apzon.Forms" },
		{ "frm_Resources", "Apzon.Forms.RoutingAndResource.frm_Resources, Apzon.Forms" },
		{ "frm_Routing", "Apzon.Forms.RoutingAndResource.frm_Routing, Apzon.Forms" },
		{ "frm_CmdConfig", "Apzon.Forms.Retail.frm_CmdConfig, Apzon.Forms" },
		{ "frm_LayoutSetup", "Apzon.Forms.Retail.frm_LayoutSetup, Apzon.Forms" },
		{ "frm_PointOfSalesBAK", "Apzon.Forms.Retail.frm_PointOfSalesBAK, Apzon.Forms" },
		{ "frm_PointOfSales", "Apzon.Forms.Retail.frm_PointOfSales, Apzon.Forms" },
		{ "frm_PosAssignment", "Apzon.Forms.Retail.frm_PosAssignment, Apzon.Forms" },
		{ "frm_PosMaster", "Apzon.Forms.Retail.frm_PosMaster, Apzon.Forms" },
		{ "PosTest", "Apzon.Forms.Retail.PosTest, Apzon.Forms" },
		{ "frm_RetailStore", "Apzon.Forms.Retail.frm_RetailStore, Apzon.Forms" },
		{ "frm_ChromeWebBrowser", "Apzon.Forms.Reports.frm_ChromeWebBrowser, Apzon.Forms" },
		{ "frm_Speadsheet", "Apzon.Forms.Reports.frm_Speadsheet, Apzon.Forms" },
		{ "frm_ValidValue", "Apzon.Forms.Reports.frm_ValidValue, Apzon.Forms" },
		{ "dlg_ImportReport", "Apzon.Forms.Reports.dlg_ImportReport, Apzon.Forms" },
		{ "frm_ExtractQueryReport", "Apzon.Forms.Reports.frm_ExtractQueryReport, Apzon.Forms" },
		{ "frm_ExtractQueryReportManger", "Apzon.Forms.Reports.frm_ExtractQueryReportManger, Apzon.Forms" },
		{ "frm_ExtractQueryReportResult", "Apzon.Forms.Reports.frm_ExtractQueryReportResult, Apzon.Forms" },
		{ "frm_ReportDesigner", "Apzon.Forms.Reports.frm_ReportDesigner, Apzon.Forms" },
		{ "frm_ReportForm", "Apzon.Forms.Reports.frm_ReportForm, Apzon.Forms" },
		{ "frm_ReportFormSettings", "Apzon.Forms.Reports.frm_ReportFormSettings, Apzon.Forms" },
		{ "frm_ReportFormSetting_Copy", "Apzon.Forms.Reports.frm_ReportFormSetting_Copy, Apzon.Forms" },
		{ "frm_ReportManager", "Apzon.Forms.Reports.frm_ReportManager, Apzon.Forms" },
		{ "frm_SelectionCriteria", "Apzon.Forms.Reports.frm_SelectionCriteria, Apzon.Forms" },
		{ "frm_SubReportForm", "Apzon.Forms.Reports.frm_SubReportForm, Apzon.Forms" },
		{ "frm_UDR", "Apzon.Forms.Reports.frm_UDR, Apzon.Forms" },
		{ "frm_XtraParameterSubmit", "Apzon.Forms.Reports.frm_XtraParameterSubmit, Apzon.Forms" },
		{ "frm_ReportViewer", "Apzon.Forms.Reports.frm_ReportViewer, Apzon.Forms" },
		{ "frm_GoodsReceiptPOByBarcode", "Apzon.Forms.Purchase.frm_GoodsReceiptPOByBarcode, Apzon.Forms" },
		{ "frm_PurchaseBlanketAgreement", "Apzon.Forms.Purchase.frm_PurchaseBlanketAgreement, Apzon.Forms" },
		{ "frm_PurchaseDownPaymentRequest", "Apzon.Forms.Purchase.frm_PurchaseDownPaymentRequest, Apzon.Forms" },
		{ "frm_PurchaseRequestControl", "Apzon.Forms.Purchase.frm_PurchaseRequestControl, Apzon.Forms" },
		{ "frm_APCreditMemos", "Apzon.Forms.Purchase.frm_APCreditMemos, Apzon.Forms" },
		{ "frm_APInvoices", "Apzon.Forms.Purchase.frm_APInvoices, Apzon.Forms" },
		{ "frm_APReserveInvoices", "Apzon.Forms.Purchase.frm_APReserveInvoices, Apzon.Forms" },
		{ "frm_GoodReceiptPO", "Apzon.Forms.Purchase.frm_GoodReceiptPO, Apzon.Forms" },
		{ "frm_PLandedCostAddNew", "Apzon.Forms.Purchase.frm_PLandedCostAddNew, Apzon.Forms" },
		{ "frm_PLandedCostsList", "Apzon.Forms.Purchase.frm_PLandedCostsList, Apzon.Forms" },
		{ "frm_ProcurementConfirm", "Apzon.Forms.Purchase.frm_ProcurementConfirm, Apzon.Forms" },
		{ "frm_PurchaseDraftReport", "Apzon.Forms.Purchase.frm_PurchaseDraftReport, Apzon.Forms" },
		{ "frm_PurchaseLandedCost", "Apzon.Forms.Purchase.frm_PurchaseLandedCost, Apzon.Forms" },
		{ "frm_GoodsReturn", "Apzon.Forms.Purchase.frm_GoodsReturn, Apzon.Forms" },
		{ "frm_PurchaseQuotation", "Apzon.Forms.Purchase.frm_PurchaseQuotation, Apzon.Forms" },
		{ "frm_PurchaseRequest", "Apzon.Forms.Purchase.frm_PurchaseRequest, Apzon.Forms" },
		{ "frm_PurchaseOder", "Apzon.Forms.Purchase.frm_PurchaseOder, Apzon.Forms" },
		{ "frm_PromotionBenefit", "Apzon.Forms.Promotions.frm_PromotionBenefit, Apzon.Forms" },
		{ "frm_PromotionCondition", "Apzon.Forms.Promotions.frm_PromotionCondition, Apzon.Forms" },
		{ "frm_PromotionDefinition", "Apzon.Forms.Promotions.frm_PromotionDefinition, Apzon.Forms" },
		{ "frm_PromotionList", "Apzon.Forms.Promotions.frm_PromotionList, Apzon.Forms" },
		{ "frm_PromotionProgram", "Apzon.Forms.Promotions.frm_PromotionProgram, Apzon.Forms" },
		{ "frm_ComponentTransfer", "Apzon.Forms.Production.frm_ComponentTransfer, Apzon.Forms" },
		{ "frm_IssueAndReceiptLackQtyForProduction", "Apzon.Forms.Production.frm_IssueAndReceiptLackQtyForProduction, Apzon.Forms" },
		{ "frm_ReceiptFromPOByBarcode", "Apzon.Forms.Production.frm_ReceiptFromPOByBarcode, Apzon.Forms" },
		{ "frm_AdvanceBillOfMaterials", "Apzon.Forms.Production.frm_AdvanceBillOfMaterials, Apzon.Forms" },
		{ "frm_BillOfMaterials", "Apzon.Forms.Production.frm_BillOfMaterials, Apzon.Forms" },
		{ "frm_GenPurchaseFromMaterialShortage", "Apzon.Forms.Production.frm_GenPurchaseFromMaterialShortage, Apzon.Forms" },
		{ "frm_IssueForProduction", "Apzon.Forms.Production.frm_IssueForProduction, Apzon.Forms" },
		{ "frm_ManufacturingSummary", "Apzon.Forms.Production.frm_ManufacturingSummary, Apzon.Forms" },
		{ "frm_MaterialShortageReport", "Apzon.Forms.Production.frm_MaterialShortageReport, Apzon.Forms" },
		{ "frm_OutSourcing", "Apzon.Forms.Production.frm_OutSourcing, Apzon.Forms" },
		{ "frm_OutsourcingSearchCFL", "Apzon.Forms.Production.frm_OutsourcingSearchCFL, Apzon.Forms" },
		{ "frm_ProdOrderImpToolRCBarcode", "Apzon.Forms.Production.frm_ProdOrderImpToolRCBarcode, Apzon.Forms" },
		{ "frm_ProdOrderImpToolSetTime", "Apzon.Forms.Production.frm_ProdOrderImpToolSetTime, Apzon.Forms" },
		{ "frm_ProductionOrder", "Apzon.Forms.Production.frm_ProductionOrder, Apzon.Forms" },
		{ "frm_ProductionOrderImportTool", "Apzon.Forms.Production.frm_ProductionOrderImportTool, Apzon.Forms" },
		{ "frm_ReceiptFromProduction", "Apzon.Forms.Production.frm_ReceiptFromProduction, Apzon.Forms" },
		{ "frm_Comments", "Apzon.Forms.PM.frm_Comments, Apzon.Forms" },
		{ "frm_GanttControl", "Apzon.Forms.PM.frm_GanttControl, Apzon.Forms" },
		{ "frm_ProjectConfirm", "Apzon.Forms.PM.frm_ProjectConfirm, Apzon.Forms" },
		{ "frm_ProjectReportCompletion", "Apzon.Forms.PM.frm_ProjectReportCompletion, Apzon.Forms" },
		{ "frm_TrackingKanban", "Apzon.Forms.PM.frm_TrackingKanban, Apzon.Forms" },
		{ "frm_WBSTemplate", "Apzon.Forms.PM.frm_WBSTemplate, Apzon.Forms" },
		{ "frm_ChangeStatus", "Apzon.Forms.PM.frm_ChangeStatus, Apzon.Forms" },
		{ "frm_ProjectManager", "Apzon.Forms.PM.frm_ProjectManager, Apzon.Forms" },
		{ "frm_ProjectManagerOrgChart", "Apzon.Forms.PM.frm_ProjectManagerOrgChart, Apzon.Forms" },
		{ "frm_RelatedActivites", "Apzon.Forms.PM.frm_RelatedActivites, Apzon.Forms" },
		{ "frm_TrackingIssue", "Apzon.Forms.PM.frm_TrackingIssue, Apzon.Forms" },
		{ "frm_AdvanceInPayment", "Apzon.Forms.Payment.frm_AdvanceInPayment, Apzon.Forms" },
		{ "frm_AdvanceOutPayment", "Apzon.Forms.Payment.frm_AdvanceOutPayment, Apzon.Forms" },
		{ "frm_AdvancePayment", "Apzon.Forms.Payment.frm_AdvancePayment, Apzon.Forms" },
		{ "frm_AdvancePaymentWizard", "Apzon.Forms.Payment.frm_AdvancePaymentWizard, Apzon.Forms" },
		{ "frm_IncomingPayments", "Apzon.Forms.Payment.frm_IncomingPayments, Apzon.Forms" },
		{ "frm_IncomingPaymentWizard", "Apzon.Forms.Payment.frm_IncomingPaymentWizard, Apzon.Forms" },
		{ "frm_OutgoingPayments", "Apzon.Forms.Payment.frm_OutgoingPayments, Apzon.Forms" },
		{ "frm_VirPayments", "Apzon.Forms.Payment.frm_VirPayments, Apzon.Forms" },
		{ "frm_CashItemMasterData", "Apzon.Forms.Payment.frm_CashItemMasterData, Apzon.Forms" },
		{ "frm_CouponMasterData", "Apzon.Forms.Payment.frm_CouponMasterData, Apzon.Forms" },
		{ "frm_PaymentOutgoing", "Apzon.Forms.Payment.frm_PaymentOutgoing, Apzon.Forms" },
		{ "frm_PaymentIncoming", "Apzon.Forms.Payment.frm_PaymentIncoming, Apzon.Forms" },
		{ "frm_VoucherMasterData", "Apzon.Forms.Payment.frm_VoucherMasterData, Apzon.Forms" },
		{ "frm_CreateTypeCase", "Apzon.Forms.Packing_Management.frm_CreateTypeCase, Apzon.Forms" },
		{ "frm_PackingOrder", "Apzon.Forms.Packing_Management.frm_PackingOrder, Apzon.Forms" },
		{ "frm_PrintBillWrap", "Apzon.Forms.Packing_Management.frm_PrintBillWrap, Apzon.Forms" },
		{ "frm_Traceability", "Apzon.Forms.Packing_Management.frm_Traceability, Apzon.Forms" },
		{ "frm_DrillDownSourceCellMrp", "Apzon.Forms.Mrps.frm_DrillDownSourceCellMrp, Apzon.Forms" },
		{ "frm_FilterSettingLine", "Apzon.Forms.Mrps.frm_FilterSettingLine, Apzon.Forms" },
		{ "frm_FilterSettingTable", "Apzon.Forms.Mrps.frm_FilterSettingTable, Apzon.Forms" },
		{ "frm_FormFilterSetting", "Apzon.Forms.Mrps.frm_FormFilterSetting, Apzon.Forms" },
		{ "frm_JetstarPreOrder", "Apzon.Forms.Mrps.frm_JetstarPreOrder, Apzon.Forms" },
		{ "frm_ScheduleDetailRequirement", "Apzon.Forms.Mrps.frm_ScheduleDetailRequirement, Apzon.Forms" },
		{ "frm_AdvanceForecast", "Apzon.Forms.Mrps.frm_AdvanceForecast, Apzon.Forms" },
		{ "frm_AutoReportComplete", "Apzon.Forms.Mrps.frm_AutoReportComplete, Apzon.Forms" },
		{ "frm_ForeCast", "Apzon.Forms.Mrps.frm_ForeCast, Apzon.Forms" },
		{ "frm_GenPOFromMRP", "Apzon.Forms.Mrps.frm_GenPOFromMRP, Apzon.Forms" },
		{ "frm_GenPOFromSchedule", "Apzon.Forms.Mrps.frm_GenPOFromSchedule, Apzon.Forms" },
		{ "frm_GenPurchaseFromSchedule", "Apzon.Forms.Mrps.frm_GenPurchaseFromSchedule, Apzon.Forms" },
		{ "frm_GenTranferRequestFromMrp", "Apzon.Forms.Mrps.frm_GenTranferRequestFromMrp, Apzon.Forms" },
		{ "frm_InputGenerateForecast", "Apzon.Forms.Mrps.frm_InputGenerateForecast, Apzon.Forms" },
		{ "frm_InputGenerateForecastItemPropeties", "Apzon.Forms.Mrps.frm_InputGenerateForecastItemPropeties, Apzon.Forms" },
		{ "frm_ItemQuotaArrangement", "Apzon.Forms.Mrps.frm_ItemQuotaArrangement, Apzon.Forms" },
		{ "frm_ItemQuotaArrangementDate", "Apzon.Forms.Mrps.frm_ItemQuotaArrangementDate, Apzon.Forms" },
		{ "frm_Lines", "Apzon.Forms.Mrps.frm_Lines, Apzon.Forms" },
		{ "frm_MRP", "Apzon.Forms.Mrps.frm_MRP, Apzon.Forms" },
		{ "frm_MrpPlan", "Apzon.Forms.Mrps.frm_MrpPlan, Apzon.Forms" },
		{ "frm_PrintBarCode", "Apzon.Forms.Mrps.frm_PrintBarCode, Apzon.Forms" },
		{ "frm_Schedule", "Apzon.Forms.Mrps.frm_Schedule, Apzon.Forms" },
		{ "frm_SalesForecast", "Apzon.Forms.Mrps.frm_SalesForecast, Apzon.Forms" },
		{ "frm_SalesForeCastQuickAnalys", "Apzon.Forms.Mrps.frm_SalesForeCastQuickAnalys, Apzon.Forms" },
		{ "frm_KPIGroup", "Apzon.Forms.KPIs.frm_KPIGroup, Apzon.Forms" },
		{ "frm_KPIIndicator", "Apzon.Forms.KPIs.frm_KPIIndicator, Apzon.Forms" },
		{ "frm_KPIPlan", "Apzon.Forms.KPIs.frm_KPIPlan, Apzon.Forms" },
		{ "frm_KPIScope", "Apzon.Forms.KPIs.frm_KPIScope, Apzon.Forms" },
		{ "frm_KPIScore", "Apzon.Forms.KPIs.frm_KPIScore, Apzon.Forms" },
		{ "frm_KPISetup", "Apzon.Forms.KPIs.frm_KPISetup, Apzon.Forms" },
		{ "frm_KPIsMasterData", "Apzon.Forms.KPIs.frm_KPIsMasterData, Apzon.Forms" },
		{ "frm_KPIsScoringDetail", "Apzon.Forms.KPIs.frm_KPIsScoringDetail, Apzon.Forms" },
		{ "frm_KPIsSetting", "Apzon.Forms.KPIs.frm_KPIsSetting, Apzon.Forms" },
		{ "frm_KPIsSetting_Copy", "Apzon.Forms.KPIs.frm_KPIsSetting_Copy, Apzon.Forms" },
		{ "frm_ListPointKPIs", "Apzon.Forms.KPIs.frm_ListPointKPIs, Apzon.Forms" },
		{ "frm_SetIndicatorKPIs", "Apzon.Forms.KPIs.frm_SetIndicatorKPIs, Apzon.Forms" },
		{ "frm_InventoryDraftsReport", "Apzon.Forms.Inventory.InventoryReport.frm_InventoryDraftsReport, Apzon.Forms" },
		{ "frm_InventoryPostingList", "Apzon.Forms.Inventory.InventoryReport.frm_InventoryPostingList, Apzon.Forms" },
		{ "frm_ItemSummary", "Apzon.Forms.Inventory.InventoryReport.frm_ItemSummary, Apzon.Forms" },
		{ "frm_StockMovementSummary", "Apzon.Forms.Inventory.InventoryReport.frm_StockMovementSummary, Apzon.Forms" },
		{ "frm_SystemPriceListDetail", "Apzon.Forms.Inventory.PriceLists.frm_SystemPriceListDetail, Apzon.Forms" },
		{ "frm_DiscountGroup", "Apzon.Forms.Inventory.PriceLists.frm_DiscountGroup, Apzon.Forms" },
		{ "frm_PeriodAndVolumeDiscounts", "Apzon.Forms.Inventory.PriceLists.frm_PeriodAndVolumeDiscounts, Apzon.Forms" },
		{ "frm_PeriodDiscounts", "Apzon.Forms.Inventory.PriceLists.frm_PeriodDiscounts, Apzon.Forms" },
		{ "frm_STDPriceListDetails", "Apzon.Forms.Inventory.PriceLists.frm_STDPriceListDetails, Apzon.Forms" },
		{ "frm_STDPriceListDetailsCriterial", "Apzon.Forms.Inventory.PriceLists.frm_STDPriceListDetailsCriterial, Apzon.Forms" },
		{ "frm_STDPriceLists", "Apzon.Forms.Inventory.PriceLists.frm_STDPriceLists, Apzon.Forms" },
		{ "frm_UoMPrices", "Apzon.Forms.Inventory.PriceLists.frm_UoMPrices, Apzon.Forms" },
		{ "frm_VolumDiscountForPriceList", "Apzon.Forms.Inventory.PriceLists.frm_VolumDiscountForPriceList, Apzon.Forms" },
		{ "frm_PackingByBarcode", "Apzon.Forms.Inventory.Pick_and_Pack.frm_PackingByBarcode, Apzon.Forms" },
		{ "frm_Packing", "Apzon.Forms.Inventory.Pick_and_Pack.frm_Packing, Apzon.Forms" },
		{ "frm_Picking", "Apzon.Forms.Inventory.Pick_and_Pack.frm_Picking, Apzon.Forms" },
		{ "frm_PickingDocument", "Apzon.Forms.Inventory.Pick_and_Pack.frm_PickingDocument, Apzon.Forms" },
		{ "frm_PickingRequest", "Apzon.Forms.Inventory.Pick_and_Pack.frm_PickingRequest, Apzon.Forms" },
		{ "frm_ShipDoc", "Apzon.Forms.Inventory.Pick_and_Pack.frm_ShipDoc, Apzon.Forms" },
		{ "frm_AlternativeItems", "Apzon.Forms.Inventory.Item_Management.frm_AlternativeItems, Apzon.Forms" },
		{ "frm_BatchesControl", "Apzon.Forms.Inventory.Item_Management.frm_BatchesControl, Apzon.Forms" },
		{ "frm_BusinessPartnerCatalogNumbers", "Apzon.Forms.Inventory.Item_Management.frm_BusinessPartnerCatalogNumbers, Apzon.Forms" },
		{ "frm_SerialNumberDetailManagement", "Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers.frm_SerialNumberDetailManagement, Apzon.Forms" },
		{ "frm_SerialNumberDetail", "Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers.frm_SerialNumberDetail, Apzon.Forms" },
		{ "frm_SerialNumberTransaction", "Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers.frm_SerialNumberTransaction, Apzon.Forms" },
		{ "frm_BatchDetailManagement", "Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchDetailManagement, Apzon.Forms" },
		{ "frm_BatchDetail", "Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchDetail, Apzon.Forms" },
		{ "frm_BatchTransaction", "Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchTransaction, Apzon.Forms" },
		{ "frm_BarcodeInventoryCounting", "Apzon.Forms.Inventory.InventoryTransaction.frm_BarcodeInventoryCounting, Apzon.Forms" },
		{ "frm_InventoryCountingRequest", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingRequest, Apzon.Forms" },
		{ "frm_InventoryTransferByBarcode", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryTransferByBarcode, Apzon.Forms" },
		{ "frm_StockCounting", "Apzon.Forms.Inventory.InventoryTransaction.frm_StockCounting, Apzon.Forms" },
		{ "frm_CycleCountRecommendations", "Apzon.Forms.Inventory.InventoryTransaction.frm_CycleCountRecommendations, Apzon.Forms" },
		{ "frm_GoodsIssue", "Apzon.Forms.Inventory.InventoryTransaction.frm_GoodsIssue, Apzon.Forms" },
		{ "frm_GoodsReceipt", "Apzon.Forms.Inventory.InventoryTransaction.frm_GoodsReceipt, Apzon.Forms" },
		{ "frm_InventoryCounting", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCounting, Apzon.Forms" },
		{ "frm_InventoryCountingBin", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingBin, Apzon.Forms" },
		{ "frm_InventoryCountingReport", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingReport, Apzon.Forms" },
		{ "frm_InventoryTranferRequest", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryTranferRequest, Apzon.Forms" },
		{ "frm_InventoryRevaluation", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryRevaluation, Apzon.Forms" },
		{ "frm_InventoryTranfer", "Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryTranfer, Apzon.Forms" },
		{ "frm_Location", "Apzon.Forms.Inventory.InventoryTransaction.frm_Location, Apzon.Forms" },
		{ "frm_PickAndPack", "Apzon.Forms.Inventory.InventoryTransaction.frm_PickAndPack, Apzon.Forms" },
		{ "frm_ReceiptBin", "Apzon.Forms.Inventory.InventoryTransaction.frm_ReceiptBin, Apzon.Forms" },
		{ "frm_StockRequest", "Apzon.Forms.Inventory.InventoryTransaction.frm_StockRequest, Apzon.Forms" },
		{ "VirGenProduction", "Apzon.Forms.Interfaces.Mrp.VirGenProduction, Apzon.Forms" },
		{ "VirSchedule", "Apzon.Forms.Interfaces.Mrp.VirSchedule, Apzon.Forms" },
		{ "VirGenPurchase", "Apzon.Forms.Interfaces.Mrp.VirGenPurchase, Apzon.Forms" },
		{ "VirMrpGenDocument", "Apzon.Forms.Interfaces.Mrp.VirMrpGenDocument, Apzon.Forms" },
		{ "VirFormInventoryTranfer", "Apzon.Forms.Interfaces.FormInventory.VirFormInventoryTranfer, Apzon.Forms" },
		{ "VirFormReceiptIssue", "Apzon.Forms.Interfaces.FormInventory.VirFormReceiptIssue, Apzon.Forms" },
		{ "VirReceiptIssueBin", "Apzon.Forms.Interfaces.FormInventory.VirReceiptIssueBin, Apzon.Forms" },
		{ "VirShipping", "Apzon.Forms.Interfaces.FormInventory.VirShipping, Apzon.Forms" },
		{ "frm_CheckPromotion", "Apzon.Forms.Interfaces.FormDocument.frm_CheckPromotion, Apzon.Forms" },
		{ "frm_ChooseDocuments", "Apzon.Forms.Interfaces.FormDocument.frm_ChooseDocuments, Apzon.Forms" },
		{ "frm_DownPaymentsToDraw", "Apzon.Forms.Interfaces.FormDocument.frm_DownPaymentsToDraw, Apzon.Forms" },
		{ "frm_ReferencedDocument", "Apzon.Forms.Interfaces.FormDocument.frm_ReferencedDocument, Apzon.Forms" },
		{ "frm_OpeningClosingRemarks", "Apzon.Forms.Interfaces.FormDocument.frm_OpeningClosingRemarks, Apzon.Forms" },
		{ "frm_RowTextDetails", "Apzon.Forms.Interfaces.FormDocument.frm_RowTextDetails, Apzon.Forms" },
		{ "frm_UserDefinedTexts", "Apzon.Forms.Interfaces.FormDocument.frm_UserDefinedTexts, Apzon.Forms" },
		{ "VirFormDocument", "Apzon.Forms.Interfaces.FormDocument.VirFormDocument, Apzon.Forms" },
		{ "VirFormPurchase", "Apzon.Forms.Interfaces.FormDocument.VirFormPurchase, Apzon.Forms" },
		{ "VirFormSales", "Apzon.Forms.Interfaces.FormDocument.VirFormSales, Apzon.Forms" },
		{ "VirFormCustomerVendorGroup", "Apzon.Forms.Interfaces.BusinessPartner.VirFormCustomerVendorGroup, Apzon.Forms" },
		{ "frm_Integrate_BusinessPartner", "Apzon.Forms.integrated.frm_Integrate_BusinessPartner, Apzon.Forms" },
		{ "frm_Integrate_Document", "Apzon.Forms.integrated.frm_Integrate_Document, Apzon.Forms" },
		{ "frm_Integrate_InventoryTransfer", "Apzon.Forms.integrated.frm_Integrate_InventoryTransfer, Apzon.Forms" },
		{ "frm_Integrate_WareHouse", "Apzon.Forms.integrated.frm_Integrate_WareHouse, Apzon.Forms" },
		{ "frm_IntegratedUdo", "Apzon.Forms.Integrated.frm_IntegratedUdo, Apzon.Forms" },
		{ "frm_Integrated_MapCode", "Apzon.Forms.Integrated.frm_Integrated_MapCode, Apzon.Forms" },
		{ "frm_Integrate_Item", "Apzon.Forms.Integrated.frm_Integrate_Item, Apzon.Forms" },
		{ "frm_ImportDataProcess", "Apzon.Forms.ImportData.frm_ImportDataProcess, Apzon.Forms" },
		{ "frm_ImportDataWizard", "Apzon.Forms.ImportData.frm_ImportDataWizard, Apzon.Forms" },
		{ "frm_Absence", "Apzon.Forms.HumanResources.frm_Absence, Apzon.Forms" },
		{ "frm_AdvertisementSource", "Apzon.Forms.HumanResources.frm_AdvertisementSource, Apzon.Forms" },
		{ "frm_EmpContract", "Apzon.Forms.HumanResources.frm_EmpContract, Apzon.Forms" },
		{ "frm_EmpContractTypes", "Apzon.Forms.HumanResources.frm_EmpContractTypes, Apzon.Forms" },
		{ "frm_EmpPenalization", "Apzon.Forms.HumanResources.frm_EmpPenalization, Apzon.Forms" },
		{ "frm_EmpRelatives", "Apzon.Forms.HumanResources.frm_EmpRelatives, Apzon.Forms" },
		{ "frm_HRBranches", "Apzon.Forms.HumanResources.frm_HRBranches, Apzon.Forms" },
		{ "frm_HRDepartment", "Apzon.Forms.HumanResources.frm_HRDepartment, Apzon.Forms" },
		{ "frm_HREducation", "Apzon.Forms.HumanResources.frm_HREducation, Apzon.Forms" },
		{ "frm_HREducationTypes", "Apzon.Forms.HumanResources.frm_HREducationTypes, Apzon.Forms" },
		{ "frm_HREmpPosition", "Apzon.Forms.HumanResources.frm_HREmpPosition, Apzon.Forms" },
		{ "frm_HREmpStatus", "Apzon.Forms.HumanResources.frm_HREmpStatus, Apzon.Forms" },
		{ "frm_HRHealth", "Apzon.Forms.HumanResources.frm_HRHealth, Apzon.Forms" },
		{ "frm_HRRolesSetup", "Apzon.Forms.HumanResources.frm_HRRolesSetup, Apzon.Forms" },
		{ "frm_HRStates", "Apzon.Forms.HumanResources.frm_HRStates, Apzon.Forms" },
		{ "frm_HRTeamsSetup", "Apzon.Forms.HumanResources.frm_HRTeamsSetup, Apzon.Forms" },
		{ "frm_HRTerminationReasons", "Apzon.Forms.HumanResources.frm_HRTerminationReasons, Apzon.Forms" },
		{ "frm_HumanResources", "Apzon.Forms.HumanResources.frm_HumanResources, Apzon.Forms" },
		{ "frm_JobType", "Apzon.Forms.HumanResources.frm_JobType, Apzon.Forms" },
		{ "frm_JobVacancy", "Apzon.Forms.HumanResources.frm_JobVacancy, Apzon.Forms" },
		{ "frm_JVApplicants", "Apzon.Forms.HumanResources.frm_JVApplicants, Apzon.Forms" },
		{ "frm_JVBenefits", "Apzon.Forms.HumanResources.frm_JVBenefits, Apzon.Forms" },
		{ "frm_JVInterviewStatus", "Apzon.Forms.HumanResources.frm_JVInterviewStatus, Apzon.Forms" },
		{ "frm_JVInterviewTypes", "Apzon.Forms.HumanResources.frm_JVInterviewTypes, Apzon.Forms" },
		{ "frm_JVQuestion", "Apzon.Forms.HumanResources.frm_JVQuestion, Apzon.Forms" },
		{ "frm_JVQuestionGroup", "Apzon.Forms.HumanResources.frm_JVQuestionGroup, Apzon.Forms" },
		{ "frm_PenalType", "Apzon.Forms.HumanResources.frm_PenalType, Apzon.Forms" },
		{ "frm_PrevEmp", "Apzon.Forms.HumanResources.frm_PrevEmp, Apzon.Forms" },
		{ "frm_Qualifications", "Apzon.Forms.HumanResources.frm_Qualifications, Apzon.Forms" },
		{ "frm_QualificationsGroup", "Apzon.Forms.HumanResources.frm_QualificationsGroup, Apzon.Forms" },
		{ "frm_Reviews", "Apzon.Forms.HumanResources.frm_Reviews, Apzon.Forms" },
		{ "frm_AboutRangeTime", "Apzon.Forms.HumanResources.TimeSheeting.frm_AboutRangeTime, Apzon.Forms" },
		{ "frm_BiometricConfiguration", "Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricConfiguration, Apzon.Forms" },
		{ "frm_BiometricData", "Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricData, Apzon.Forms" },
		{ "frm_BiometricDataProcess", "Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricDataProcess, Apzon.Forms" },
		{ "frm_BiometricEmpMapping", "Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricEmpMapping, Apzon.Forms" },
		{ "frm_EmployeeScheduleEdit", "Apzon.Forms.HumanResources.TimeSheeting.frm_EmployeeScheduleEdit, Apzon.Forms" },
		{ "frm_HolidaySetup", "Apzon.Forms.HumanResources.TimeSheeting.frm_HolidaySetup, Apzon.Forms" },
		{ "frm_ImportTimeSheeting", "Apzon.Forms.HumanResources.TimeSheeting.frm_ImportTimeSheeting, Apzon.Forms" },
		{ "frm_InOutMethod", "Apzon.Forms.HumanResources.TimeSheeting.frm_InOutMethod, Apzon.Forms" },
		{ "frm_LeaveCalculation", "Apzon.Forms.HumanResources.TimeSheeting.frm_LeaveCalculation, Apzon.Forms" },
		{ "frm_LeaveInformation", "Apzon.Forms.HumanResources.TimeSheeting.frm_LeaveInformation, Apzon.Forms" },
		{ "frm_NewPeriod", "Apzon.Forms.HumanResources.TimeSheeting.frm_NewPeriod, Apzon.Forms" },
		{ "frm_OvertimeType", "Apzon.Forms.HumanResources.TimeSheeting.frm_OvertimeType, Apzon.Forms" },
		{ "frm_TimeSheeting", "Apzon.Forms.HumanResources.TimeSheeting.frm_TimeSheeting, Apzon.Forms" },
		{ "frm_TimeSheetingHour", "Apzon.Forms.HumanResources.TimeSheeting.frm_TimeSheetingHour, Apzon.Forms" },
		{ "frm_TimeSheetInOut", "Apzon.Forms.HumanResources.TimeSheeting.frm_TimeSheetInOut, Apzon.Forms" },
		{ "frm_WorkCode", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkCode, Apzon.Forms" },
		{ "frm_WorkPeriod", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkPeriod, Apzon.Forms" },
		{ "frm_WorkScheduleRegister", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkScheduleRegister, Apzon.Forms" },
		{ "frm_WorkScheduleSetup", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkScheduleSetup, Apzon.Forms" },
		{ "frm_WorkShift", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkShift, Apzon.Forms" },
		{ "frm_WorkType", "Apzon.Forms.HumanResources.TimeSheeting.frm_WorkType, Apzon.Forms" },
		{ "frm_AllowanceType", "Apzon.Forms.HumanResources.Salary.frm_AllowanceType, Apzon.Forms" },
		{ "frm_DeductionType", "Apzon.Forms.HumanResources.Salary.frm_DeductionType, Apzon.Forms" },
		{ "frm_PersonalIncomeTax", "Apzon.Forms.HumanResources.Salary.frm_PersonalIncomeTax, Apzon.Forms" },
		{ "frm_SalAdvanced", "Apzon.Forms.HumanResources.Salary.frm_SalAdvanced, Apzon.Forms" },
		{ "frm_SalAllowance", "Apzon.Forms.HumanResources.Salary.frm_SalAllowance, Apzon.Forms" },
		{ "frm_SalAllowanceByEmp", "Apzon.Forms.HumanResources.Salary.frm_SalAllowanceByEmp, Apzon.Forms" },
		{ "frm_SalaryCalculation", "Apzon.Forms.HumanResources.Salary.frm_SalaryCalculation, Apzon.Forms" },
		{ "frm_SalaryInfo", "Apzon.Forms.HumanResources.Salary.frm_SalaryInfo, Apzon.Forms" },
		{ "frm_SalaryParameters", "Apzon.Forms.HumanResources.Salary.frm_SalaryParameters, Apzon.Forms" },
		{ "frm_SalaryResult", "Apzon.Forms.HumanResources.Salary.frm_SalaryResult, Apzon.Forms" },
		{ "frm_SalaryViewResult", "Apzon.Forms.HumanResources.Salary.frm_SalaryViewResult, Apzon.Forms" },
		{ "frm_SalInfoByEmp", "Apzon.Forms.HumanResources.Salary.frm_SalInfoByEmp, Apzon.Forms" },
		{ "frm_SalInfoMain", "Apzon.Forms.HumanResources.Salary.frm_SalInfoMain, Apzon.Forms" },
		{ "frm_LeaveParameter", "Apzon.Forms.HumanResources.Salary.frm_LeaveParameter, Apzon.Forms" },
		{ "F_ThongSoKetNoi", "Apzon.Forms.UserControl.F_ThongSoKetNoi, Apzon.Forms" },
		{ "frm_TempateControlGrid", "Apzon.Forms.FormTemplate.frm_TempateControlGrid, Apzon.Forms" },
		{ "frm_TemplateControlGridGrid", "Apzon.Forms.FormTemplate.frm_TemplateControlGridGrid, Apzon.Forms" },
		{ "frm_TemplateInputBox", "Apzon.Forms.FormTemplate.frm_TemplateInputBox, Apzon.Forms" },
		{ "frm_TemplateWizard", "Apzon.Forms.FormTemplate.frm_TemplateWizard, Apzon.Forms" },
		{ "frm_TemplateWizardDesigner", "Apzon.Forms.FormTemplate.frm_TemplateWizardDesigner, Apzon.Forms" },
		{ "frm_AssetMasterData", "Apzon.Forms.Financials.frm_AssetMasterData, Apzon.Forms" },
		{ "frm_CostCenters", "Apzon.Forms.Financials.frm_CostCenters, Apzon.Forms" },
		{ "frm_Costing", "Apzon.Forms.Financials.frm_Costing, Apzon.Forms" },
		{ "frm_Currencies", "Apzon.Forms.Financials.frm_Currencies, Apzon.Forms" },
		{ "frm_Dimension", "Apzon.Forms.Financials.frm_Dimension, Apzon.Forms" },
		{ "frm_DistributeRule", "Apzon.Forms.Financials.frm_DistributeRule, Apzon.Forms" },
		{ "frm_ListDitributeRule", "Apzon.Forms.Financials.frm_ListDitributeRule, Apzon.Forms" },
		{ "frm_FacebookManager", "Apzon.Forms.FacebookCRM.frm_FacebookManager, Apzon.Forms" },
		{ "frm_FacebookCRM", "Apzon.Forms.Facebook_CRM.frm_FacebookCRM, Apzon.Forms" },
		{ "frm_FacebookLoginDialog", "Apzon.Forms.Facebook_CRM.frm_FacebookLoginDialog, Apzon.Forms" },
		{ "frm_AdminUploadAttachment", "Apzon.Forms.DMS.frm_AdminUploadAttachment, Apzon.Forms" },
		{ "frm_AlertTemplate", "Apzon.Forms.DMS.frm_AlertTemplate, Apzon.Forms" },
		{ "frm_Channel", "Apzon.Forms.DMS.frm_Channel, Apzon.Forms" },
		{ "frm_DMSAuth", "Apzon.Forms.DMS.frm_DMSAuth, Apzon.Forms" },
		{ "frm_DMSDelivery", "Apzon.Forms.DMS.frm_DMSDelivery, Apzon.Forms" },
		{ "frm_DMSSettingUDF", "Apzon.Forms.DMS.frm_DMSSettingUDF, Apzon.Forms" },
		{ "frm_Geography", "Apzon.Forms.DMS.frm_Geography, Apzon.Forms" },
		{ "frm_ImageAlbum", "Apzon.Forms.DMS.frm_ImageAlbum, Apzon.Forms" },
		{ "frm_ImageViewer", "Apzon.Forms.DMS.frm_ImageViewer, Apzon.Forms" },
		{ "frm_ItemImageViewer", "Apzon.Forms.DMS.frm_ItemImageViewer, Apzon.Forms" },
		{ "frm_ListOfDistributors", "Apzon.Forms.DMS.frm_ListOfDistributors, Apzon.Forms" },
		{ "frm_Marker", "Apzon.Forms.DMS.frm_Marker, Apzon.Forms" },
		{ "frm_NotificationEventManager", "Apzon.Forms.DMS.frm_NotificationEventManager, Apzon.Forms" },
		{ "frm_Outlet", "Apzon.Forms.DMS.frm_Outlet, Apzon.Forms" },
		{ "frm_OutletVisitingFrequency", "Apzon.Forms.DMS.frm_OutletVisitingFrequency, Apzon.Forms" },
		{ "frm_PromotionSetup", "Apzon.Forms.DMS.frm_PromotionSetup, Apzon.Forms" },
		{ "frm_QuestionMasterData", "Apzon.Forms.DMS.frm_QuestionMasterData, Apzon.Forms" },
		{ "frm_Region", "Apzon.Forms.DMS.frm_Region, Apzon.Forms" },
		{ "frm_SalesRoute", "Apzon.Forms.DMS.frm_SalesRoute, Apzon.Forms" },
		{ "frm_SaleTerritory", "Apzon.Forms.DMS.frm_SaleTerritory, Apzon.Forms" },
		{ "frm_SettingApprovalDetail", "Apzon.Forms.DMS.frm_SettingApprovalDetail, Apzon.Forms" },
		{ "frm_SettingReport", "Apzon.Forms.DMS.frm_SettingReport, Apzon.Forms" },
		{ "frm_Store", "Apzon.Forms.DMS.frm_Store, Apzon.Forms" },
		{ "frm_UploadAttachment", "Apzon.Forms.DMS.frm_UploadAttachment, Apzon.Forms" },
		{ "frm_UploadItemImage", "Apzon.Forms.DMS.frm_UploadItemImage, Apzon.Forms" },
		{ "frm_UserOnlineDashboard", "Apzon.Forms.DMS.frm_UserOnlineDashboard, Apzon.Forms" },
		{ "frm_ReportSalesCoverage", "Apzon.Forms.DMS.Report.frm_ReportSalesCoverage, Apzon.Forms" },
		{ "frm_SalesCoverageGoogleMap", "Apzon.Forms.DMS.Report.frm_SalesCoverageGoogleMap, Apzon.Forms" },
		{ "frm_CategoriesDAB", "Apzon.Forms.Dashboard.frm_CategoriesDAB, Apzon.Forms" },
		{ "frm_Dashboard", "Apzon.Forms.Dashboard.frm_Dashboard, Apzon.Forms" },
		{ "frm_DashBoardAuthorization", "Apzon.Forms.Dashboard.frm_DashBoardAuthorization, Apzon.Forms" },
		{ "frm_DashboardCategories", "Apzon.Forms.Dashboard.frm_DashboardCategories, Apzon.Forms" },
		{ "frm_DashboardDesigner", "Apzon.Forms.Dashboard.frm_DashboardDesigner, Apzon.Forms" },
		{ "frm_DashboardManagement", "Apzon.Forms.Dashboard.frm_DashboardManagement, Apzon.Forms" },
		{ "frm_DashboardManager", "Apzon.Forms.Dashboard.frm_DashboardManager, Apzon.Forms" },
		{ "frm_DashboardTab", "Apzon.Forms.Dashboard.frm_DashboardTab, Apzon.Forms" },
		{ "frm_DashboardViewer", "Apzon.Forms.Dashboard.frm_DashboardViewer, Apzon.Forms" },
		{ "frm_DataSubController", "Apzon.Forms.Dashboard.frm_DataSubController, Apzon.Forms" },
		{ "frm_DBAuthorizationCopy", "Apzon.Forms.Dashboard.frm_DBAuthorizationCopy, Apzon.Forms" },
		{ "frm_DashboardEdit", "Apzon.Forms.Dashboard.DashboardViewer.frm_DashboardEdit, Apzon.Forms" },
		{ "FrmApprovalRequest", "Apzon.Forms.Commons.FrmApprovalRequest, Apzon.Forms" },
		{ "frm_ChangeLog", "Apzon.Forms.Commons.frm_ChangeLog, Apzon.Forms" },
		{ "frm_ChangeLogDiff", "Apzon.Forms.Commons.frm_ChangeLogDiff, Apzon.Forms" },
		{ "frm_ChangeLogList", "Apzon.Forms.Commons.frm_ChangeLogList, Apzon.Forms" },
		{ "frm_ChangeLogRow", "Apzon.Forms.Commons.frm_ChangeLogRow, Apzon.Forms" },
		{ "frm_ContextAction", "Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms" },
		{ "frm_ContextActionPublishService", "Apzon.Forms.Commons.frm_ContextActionPublishService, Apzon.Forms" },
		{ "frm_ContextActionPublishServiceListOf", "Apzon.Forms.Commons.frm_ContextActionPublishServiceListOf, Apzon.Forms" },
		{ "frm_CopyWizardPage", "Apzon.Forms.Commons.frm_CopyWizardPage, Apzon.Forms" },
		{ "frm_DefineNew", "Apzon.Forms.Commons.frm_DefineNew, Apzon.Forms" },
		{ "frm_Diagram", "Apzon.Forms.Commons.frm_Diagram, Apzon.Forms" },
		{ "frm_ImportFiles", "Apzon.Forms.Commons.frm_ImportFiles, Apzon.Forms" },
		{ "frm_OrganizationalChart", "Apzon.Forms.Commons.frm_OrganizationalChart, Apzon.Forms" },
		{ "frm_ProcessDIServer", "Apzon.Forms.Commons.frm_ProcessDIServer, Apzon.Forms" },
		{ "frm_ReadExcelToGrid", "Apzon.Forms.Commons.frm_ReadExcelToGrid, Apzon.Forms" },
		{ "frm_ReportWriter", "Apzon.Forms.Commons.frm_ReportWriter, Apzon.Forms" },
		{ "frm_SearchDataList", "Apzon.Forms.Commons.frm_SearchDataList, Apzon.Forms" },
		{ "frm_SMSConfig", "Apzon.Forms.Commons.frm_SMSConfig, Apzon.Forms" },
		{ "frm_UserDefinedProcedure", "Apzon.Forms.Commons.frm_UserDefinedProcedure, Apzon.Forms" },
		{ "frm_UserDefinedProcedureCreate", "Apzon.Forms.Commons.frm_UserDefinedProcedureCreate, Apzon.Forms" },
		{ "CFLParamaterSql", "Apzon.Forms.Commons.CFLParamaterSql, Apzon.Forms" },
		{ "frm_ManagerQuery", "Apzon.Forms.Commons.frm_ManagerQuery, Apzon.Forms" },
		{ "frm_SalaryFormula", "Apzon.Forms.Commons.frm_SalaryFormula, Apzon.Forms" },
		{ "frm_SetUpUDT", "Apzon.Forms.Commons.UDT.frm_SetUpUDT, Apzon.Forms" },
		{ "frm_UDTManager", "Apzon.Forms.Commons.UDT.frm_UDTManager, Apzon.Forms" },
		{ "frm_ORW", "Apzon.Forms.Commons.UDO.frm_ORW, Apzon.Forms" },
		{ "frm_UDO", "Apzon.Forms.Commons.UDO.frm_UDO, Apzon.Forms" },
		{ "frm_UserDefineFieldsManagement", "Apzon.Forms.Commons.UDF.frm_UserDefineFieldsManagement, Apzon.Forms" },
		{ "frm_UserTableKeys", "Apzon.Forms.Commons.UDF.frm_UserTableKeys, Apzon.Forms" },
		{ "frm_ManagerCategories", "Apzon.Forms.Commons.Queries.frm_ManagerCategories, Apzon.Forms" },
		{ "frm_SetParamater", "Apzon.Forms.Commons.Queries.frm_SetParamater, Apzon.Forms" },
		{ "frm_FileDialog", "Apzon.Forms.Commons.FileDialog.frm_FileDialog, Apzon.Forms" },
		{ "frm_CFL", "Apzon.Forms.Commons.CFL.frm_CFL, Apzon.Forms" },
		{ "frm_ChooseFromList", "Apzon.Forms.Commons.CFL.frm_ChooseFromList, Apzon.Forms" },
		{ "frm_DemoCfl", "Apzon.Forms.Commons.CFL.frm_DemoCfl, Apzon.Forms" },
		{ "frm_List_Document", "Apzon.Forms.Commons.CFL.frm_List_Document, Apzon.Forms" },
		{ "frm_BPBankAccountSetup", "Apzon.Forms.BusinessMaster.frm_BPBankAccountSetup, Apzon.Forms" },
		{ "frm_BPControlAccounts", "Apzon.Forms.BusinessMaster.frm_BPControlAccounts, Apzon.Forms" },
		{ "frm_BPPaymentDates", "Apzon.Forms.BusinessMaster.frm_BPPaymentDates, Apzon.Forms" },
		{ "frm_BusinessMasterData", "Apzon.Forms.BusinessMaster.frm_BusinessMasterData, Apzon.Forms" },
		{ "frm_CreditCardType", "Apzon.Forms.BusinessMaster.frm_CreditCardType, Apzon.Forms" },
		{ "frm_CashDiscountEdit", "Apzon.Forms.BusinessMaster.frm_CashDiscountEdit, Apzon.Forms" },
		{ "frm_IndustrySetup", "Apzon.Forms.BusinessMaster.frm_IndustrySetup, Apzon.Forms" },
		{ "frm_QuickCreationBPMasterData", "Apzon.Forms.BusinessMaster.frm_QuickCreationBPMasterData, Apzon.Forms" },
		{ "frm_AccessLog", "Apzon.Forms.Administrator.frm_AccessLog, Apzon.Forms" },
		{ "frm_ActivityCalendar", "Apzon.Forms.Administrator.frm_ActivityCalendar, Apzon.Forms" },
		{ "frm_AlertOverview", "Apzon.Forms.Administrator.frm_AlertOverview, Apzon.Forms" },
		{ "frm_AuthorizationCopy", "Apzon.Forms.Administrator.frm_AuthorizationCopy, Apzon.Forms" },
		{ "frm_AuthorToken", "Apzon.Forms.Administrator.frm_AuthorToken, Apzon.Forms" },
		{ "frm_BranchAssignment", "Apzon.Forms.Administrator.frm_BranchAssignment, Apzon.Forms" },
		{ "frm_Braches", "Apzon.Forms.Administrator.frm_Braches, Apzon.Forms" },
		{ "frm_CalendarSetting", "Apzon.Forms.Administrator.frm_CalendarSetting, Apzon.Forms" },
		{ "frm_ChooseDfltBranch", "Apzon.Forms.Administrator.frm_ChooseDfltBranch, Apzon.Forms" },
		{ "frm_CustomsGroup", "Apzon.Forms.Administrator.frm_CustomsGroup, Apzon.Forms" },
		{ "frm_DataOwnerShipAuthorization", "Apzon.Forms.Administrator.frm_DataOwnerShipAuthorization, Apzon.Forms" },
		{ "frm_DataOwnerShipAuthorizationCopy", "Apzon.Forms.Administrator.frm_DataOwnerShipAuthorizationCopy, Apzon.Forms" },
		{ "frm_EmpToken", "Apzon.Forms.Administrator.frm_EmpToken, Apzon.Forms" },
		{ "frm_ExternalRouting", "Apzon.Forms.Administrator.frm_ExternalRouting, Apzon.Forms" },
		{ "frm_FileCopySharingSettings", "Apzon.Forms.Administrator.frm_FileCopySharingSettings, Apzon.Forms" },
		{ "frm_Authorization", "Apzon.Forms.Administrator.frm_Authorization, Apzon.Forms" },
		{ "frm_ChangePassword", "Apzon.Forms.Administrator.frm_ChangePassword, Apzon.Forms" },
		{ "frm_Department", "Apzon.Forms.Administrator.frm_Department, Apzon.Forms" },
		{ "frm_FilterSelect", "Apzon.Forms.Administrator.frm_FilterSelect, Apzon.Forms" },
		{ "frm_FilterSetting", "Apzon.Forms.Administrator.frm_FilterSetting, Apzon.Forms" },
		{ "frm_Freight", "Apzon.Forms.Administrator.frm_Freight, Apzon.Forms" },
		{ "frm_FtpServerSetting", "Apzon.Forms.Administrator.frm_FtpServerSetting, Apzon.Forms" },
		{ "frm_GeneSetting", "Apzon.Forms.Administrator.frm_GeneSetting, Apzon.Forms" },
		{ "frm_Holiday", "Apzon.Forms.Administrator.frm_Holiday, Apzon.Forms" },
		{ "frm_ItemGroups", "Apzon.Forms.Administrator.frm_ItemGroups, Apzon.Forms" },
		{ "frm_ItemProperties", "Apzon.Forms.Administrator.frm_ItemProperties, Apzon.Forms" },
		{ "frm_LengthAndWidth", "Apzon.Forms.Administrator.frm_LengthAndWidth, Apzon.Forms" },
		{ "frm_LicenseManager", "Apzon.Forms.Administrator.frm_LicenseManager, Apzon.Forms" },
		{ "frm_ListQueries", "Apzon.Forms.Administrator.frm_ListQueries, Apzon.Forms" },
		{ "frm_ListTemplateQueries", "Apzon.Forms.Administrator.frm_ListTemplateQueries, Apzon.Forms" },
		{ "frm_Locations", "Apzon.Forms.Administrator.frm_Locations, Apzon.Forms" },
		{ "frm_MailSetting", "Apzon.Forms.Administrator.frm_MailSetting, Apzon.Forms" },
		{ "frm_PreviewLayouts", "Apzon.Forms.Administrator.frm_PreviewLayouts, Apzon.Forms" },
		{ "frm_PrintLayout", "Apzon.Forms.Administrator.frm_PrintLayout, Apzon.Forms" },
		{ "frm_Project", "Apzon.Forms.Administrator.frm_Project, Apzon.Forms" },
		{ "frm_QueryGenerator", "Apzon.Forms.Administrator.frm_QueryGenerator, Apzon.Forms" },
		{ "frm_QueryPreview", "Apzon.Forms.Administrator.frm_QueryPreview, Apzon.Forms" },
		{ "frm_QueryTemplateRpt", "Apzon.Forms.Administrator.frm_QueryTemplateRpt, Apzon.Forms" },
		{ "frm_SettingPassword", "Apzon.Forms.Administrator.frm_SettingPassword, Apzon.Forms" },
		{ "frm_State", "Apzon.Forms.Administrator.frm_State, Apzon.Forms" },
		{ "frm_ThirdPartyToken", "Apzon.Forms.Administrator.frm_ThirdPartyToken, Apzon.Forms" },
		{ "frm_UnitOfMeasureGroups", "Apzon.Forms.Administrator.frm_UnitOfMeasureGroups, Apzon.Forms" },
		{ "frm_UnitOfMeasure", "Apzon.Forms.Administrator.frm_UnitOfMeasure, Apzon.Forms" },
		{ "frm_UoMGroupDefinition", "Apzon.Forms.Administrator.frm_UoMGroupDefinition, Apzon.Forms" },
		{ "frm_UpdateFilter", "Apzon.Forms.Administrator.frm_UpdateFilter, Apzon.Forms" },
		{ "frm_Users", "Apzon.Forms.Administrator.frm_Users, Apzon.Forms" },
		{ "frm_UsersChangePass", "Apzon.Forms.Administrator.frm_UsersChangePass, Apzon.Forms" },
		{ "frm_WareHouses", "Apzon.Forms.Administrator.frm_WareHouses, Apzon.Forms" },
		{ "frm_Weight", "Apzon.Forms.Administrator.frm_Weight, Apzon.Forms" },
		{ "frm_WorkingTable", "Apzon.Forms.Administrator.frm_WorkingTable, Apzon.Forms" },
		{ "frm_POS", "Apzon.Forms.Administrator.frm_POS, Apzon.Forms" },
		{ "frm_NoteManagement", "Apzon.Forms.Administrator.frm_NoteManagement, Apzon.Forms" },
		{ "frm_CompanyDetails", "Apzon.Forms.Administrator.SystemInitialization.frm_CompanyDetails, Apzon.Forms" },
		{ "frm_DocumentNumbering", "Apzon.Forms.Administrator.SystemInitialization.frm_DocumentNumbering, Apzon.Forms" },
		{ "frm_DocumentSettings", "Apzon.Forms.Administrator.SystemInitialization.frm_DocumentSettings, Apzon.Forms" },
		{ "frm_MenuStructure", "Apzon.Forms.Administrator.SystemInitialization.frm_MenuStructure, Apzon.Forms" },
		{ "frm_Series", "Apzon.Forms.Administrator.SystemInitialization.frm_Series, Apzon.Forms" },
		{ "frm_AdditionalGeneralSettings", "Apzon.Forms.Administrator.SetUp.frm_AdditionalGeneralSettings, Apzon.Forms" },
		{ "frm_AutoSelectBatchSerialConfig", "Apzon.Forms.Administrator.SetUp.frm_AutoSelectBatchSerialConfig, Apzon.Forms" },
		{ "frm_BarcodeConfig", "Apzon.Forms.Administrator.SetUp.frm_BarcodeConfig, Apzon.Forms" },
		{ "frm_NotificationSetting", "Apzon.Forms.Administrator.SetUp.frm_NotificationSetting, Apzon.Forms" },
		{ "frm_SetupAdmin", "Apzon.Forms.Administrator.SetUp.frm_SetupAdmin, Apzon.Forms" },
		{ "frm_Territory", "Apzon.Forms.Administrator.SetUp.frm_Territory, Apzon.Forms" },
		{ "frm_UserDefault", "Apzon.Forms.Administrator.SetUp.frm_UserDefault, Apzon.Forms" },
		{ "frm_Vehicle", "Apzon.Forms.Administrator.SetUp.frm_Vehicle, Apzon.Forms" },
		{ "frm_VehicleType", "Apzon.Forms.Administrator.SetUp.frm_VehicleType, Apzon.Forms" },
		{ "frm_TaxDefinitionSetup", "Apzon.Forms.Administrator.SetUp.Tax.frm_TaxDefinitionSetup, Apzon.Forms" },
		{ "frm_TaxGroups", "Apzon.Forms.Administrator.SetUp.Tax.frm_TaxGroups, Apzon.Forms" },
		{ "frm_ContractTemplate", "Apzon.Forms.Administrator.SetUp.Service.frm_ContractTemplate, Apzon.Forms" },
		{ "frm_SalesEmployeeBuyers", "Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers.frm_SalesEmployeeBuyers, Apzon.Forms" },
		{ "frm_SalesGroupcs", "Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers.frm_SalesGroupcs, Apzon.Forms" },
		{ "frm_LandedCosts", "Apzon.Forms.Administrator.SetUp.Purchasing.frm_LandedCosts, Apzon.Forms" },
		{ "frm_LandedCostsSetup", "Apzon.Forms.Administrator.SetUp.Purchasing.frm_LandedCostsSetup, Apzon.Forms" },
		{ "frm_CycleCountDetermination", "Apzon.Forms.Administrator.SetUp.Inventory.frm_CycleCountDetermination, Apzon.Forms" },
		{ "frm_CycleCountDeterminationSubtable", "Apzon.Forms.Administrator.SetUp.Inventory.frm_CycleCountDeterminationSubtable, Apzon.Forms" },
		{ "frm_InventoryCycles", "Apzon.Forms.Administrator.SetUp.Inventory.frm_InventoryCycles, Apzon.Forms" },
		{ "frm_AttrCodeSetup", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_AttrCodeSetup, Apzon.Forms" },
		{ "frm_BinLocList", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_BinLocList, Apzon.Forms" },
		{ "frm_BinLocMasterData", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_BinLocMasterData, Apzon.Forms" },
		{ "frm_FieldActivation", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_FieldActivation, Apzon.Forms" },
		{ "frm_WHSubManager", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_WHSubManager, Apzon.Forms" },
		{ "frm_WHSubSetup", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_WHSubSetup, Apzon.Forms" },
		{ "frm_WHSubSetupPreview", "Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_WHSubSetupPreview, Apzon.Forms" },
		{ "frm_PluginManager", "Apzon.Forms.Administrator.Plugin.frm_PluginManager, Apzon.Forms" },
		{ "frm_RegisterPlugin", "Apzon.Forms.Administrator.Plugin.frm_RegisterPlugin, Apzon.Forms" },
		{ "frm_BusinessPartnerPriorities", "Apzon.Forms.Administrator.Business_Partners.frm_BusinessPartnerPriorities, Apzon.Forms" },
		{ "frm_BusinessPartnerProperties", "Apzon.Forms.Administrator.Business_Partners.frm_BusinessPartnerProperties, Apzon.Forms" },
		{ "frm_Countries", "Apzon.Forms.Administrator.Business_Partners.frm_Countries, Apzon.Forms" },
		{ "frm_CustomerGroup", "Apzon.Forms.Administrator.Business_Partners.frm_CustomerGroup, Apzon.Forms" },
		{ "frm_PaymentTerms", "Apzon.Forms.Administrator.Business_Partners.frm_PaymentTerms, Apzon.Forms" },
		{ "frm_VendorGroup", "Apzon.Forms.Administrator.Business_Partners.frm_VendorGroup, Apzon.Forms" },
		{ "frm_CreditCardSetup", "Apzon.Forms.Administrator.Banking.frm_CreditCardSetup, Apzon.Forms" },
		{ "frm_CreditPaymentMethods", "Apzon.Forms.Administrator.Banking.frm_CreditPaymentMethods, Apzon.Forms" },
		{ "frm_ApprovalStages", "Apzon.Forms.Administrator.ApprovalProcess.frm_ApprovalStages, Apzon.Forms" },
		{ "frm_ApprovalTemplates", "Apzon.Forms.Administrator.ApprovalProcess.frm_ApprovalTemplates, Apzon.Forms" },
		{ "frm_WorkingProcess", "Apzon.Forms.Administrator.ApprovalProcess.frm_WorkingProcess, Apzon.Forms" },
		{ "frm_WorkingProcessSetting", "Apzon.Forms.Administrator.ApprovalProcess.frm_WorkingProcessSetting, Apzon.Forms" },
		{ "frm_FormSetting_Copy", "Apzon.Forms.Commons.frm_FormSetting_Copy, Apzon.Controls" },
		{ "frm_BatchModification", "Apzon.Controls.Form.frm_BatchModification, Apzon.Controls" },
		{ "frm_FormatRule", "Apzon.Controls.Form.frm_FormatRule, Apzon.Controls" },
		{ "frm_FormatRule_Copy", "Apzon.Controls.Form.frm_FormatRule_Copy, Apzon.Controls" },
		{ "frm_FormSetting", "Apzon.Controls.Form.frm_FormSetting, Apzon.Controls" },
		{ "frm_ExchangeRatesAndIndexs", "Apzon.Controls.Form.ExchangeRateAndIndex.frm_ExchangeRatesAndIndexs, Apzon.Controls" },
		{ "frm_SetRateForSelectionCriteria", "Apzon.Controls.Form.ExchangeRateAndIndex.frm_SetRateForSelectionCriteria, Apzon.Controls" }
	};

	private bool _activeFormMaximize;

	private readonly Dictionary<string, BarButtonItem> _integratedBarbutton = new Dictionary<string, BarButtonItem>();

	private bool _previewLayoutAuth;

	private bool _diyAuth;

	private static readonly object _messageLog = new object();

	private string _skinName;

	private bool _userRaise = true;

	private TreeListNode hotTrackNode;

	private const int WM_NCLBUTTONDOWN = 161;

	private const int HTCLOSE = 20;

	private BarManager BarManager;

	private Bar ToolBar;

	private Bar MenuBar;

	private Bar bar3;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem barButtonItem1;

	private BarSubItem barSub_System;

	private BarSubItem barSubItem3;

	private BarSubItem barSub_Module;

	private BarSubItem barSub_Tool;

	private BarSubItem barSub_Help;

	private BarButtonItem barButtonItem2;

	private BarButtonItem barButtonItem3;

	private BarStaticItem barStaticItem1;

	private BarButtonItem bar_PrintLayout;

	private BarButtonItem barButtonItem5;

	private BarButtonItem barPrint;

	private BarStaticItem barStaticItem2;

	private BarButtonItem bar_Find;

	private BarButtonItem bar_NewDoc;

	private BarButtonItem barButtonModeFirst;

	private BarButtonItem barButtonModePrev;

	private BarButtonItem barButtonModeNext;

	private BarButtonItem barButtonLast;

	private BarButtonItem barButtonItem14;

	private BarStaticItem txt_MsgBar;

	private SkinBarSubItem skinBarSubItem1;

	public System.Windows.Forms.Timer TimerStaticStatusBar;

	private BarSubItem barSubItem7;

	private BarButtonItem barButtonItem15;

	private Bar bar4;

	private BarSubItem barSubItem11;

	private BarButtonItem barFormSetting;

	private BarButtonItem barButtonItem7;

	private BarSubItem barSub_Administration;

	private BarSubItem barSub_SalesAR;

	private BarSubItem barSub_PurchaseAP;

	private BarSubItem barSub_BP;

	private BarSubItem barSub_Inventory;

	private BarSubItem barSub_Reports;

	private BarButtonItem barButtonItem11;

	private BarButtonItem barButtonItem10;

	private BarButtonItem barButtonItem8;

	private BarButtonItem barButtonItem9;

	private BarButtonItem barButtonItem12;

	private BarButtonItem barButtonItem16;

	private BarSubItem barSubItem14;

	private BarSubItem barSubItem21;

	private BarButtonItem barButtonItem17;

	private BarButtonItem barButtonItem18;

	private BarButtonItem barButtonItem19;

	private SkinBarSubItem skinBarSubItem2;

	private BarToolbarsListItem barToolbars_CheckForUpdate;

	private BarSubItem barSubItem22;

	private BarButtonItem barButtonItem_CheckForUpdate;

	private System.Windows.Forms.Timer timerLogin;

	private NavBarGroup navBarGroup13;

	private BarSubItem barSub_Language;

	private BarButtonItem barSub_LanguageEn;

	private BarButtonItem barSub_LanguageVn;

	private BarButtonItem barSub_Preview;

	private BarButtonItem barSub_PreviewLayout;

	private BarButtonItem barSub_Print;

	private BarButtonItem barSub_Exit;

	private BarSubItem barSubItem8;

	private BarStaticItem bar_User;

	private BarSubItem barSub_Queries;

	private BarButtonItem menu_frm_QueryGenerator;

	private BarButtonItem menu_frm_BusinessMasterData;

	private BarButtonItem menu_frm_ActivitiesForOpportunity;

	private BarButtonItem menu_frm_SalesQuotationForBatch;

	private BarButtonItem menu_frm_SalesQuotation;

	private BarButtonItem menu_frm_SalesOrder;

	private BarButtonItem menu_frm_Delivery;

	private BarButtonItem menu_frm_Return;

	private BarButtonItem menu_frm_ARInvoices;

	private BarButtonItem menu_frm_ARCreditMemos;

	private BarButtonItem menu_AR_ReserveInvoices;

	private BarButtonItem menu_frm_Atp;

	private BarButtonItem bar_ExportExcel;

	private BarButtonItem bar_ExportWord;

	private BarButtonItem bar_ExportPdf;

	private BarSubItem barSubItem23;

	private BarStaticItem barStaticItem7;

	private BarButtonItem barButtonItem4;

	private BarEditItem barEditItem1;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private BarHeaderItem barHeaderItem1;

	private BarEditItem barEditItem2;

	private RepositoryItemTextEdit repositoryItemTextEdit2;

	private BarDockingMenuItem barDockingMenuItem1;

	private BarEditItem barEditItem3;

	private RepositoryItemTextEdit repositoryItemTextEdit3;

	private BarButtonItem barSub_Export;

	private BarButtonItem barSub_PreviewGridview;

	private BarButtonItem barButtonItem_QueryTemplateRpt;

	private BarButtonItem barButtonItem_QueryManagerRpt;

	private BarSubItem barSubItem_QueryTemplateReport;

	private BarSubItem barSubItem5;

	private BarSubItem barSubItem_QueryManagerReport;

	private NavBarItem nav_frm_BarCodeStandard;

	private NavBarItem nav_frm_ListScheduling;

	private NavBarItem nav_frm_BarCode;

	private NavBarItem nav_frm_ReportBarCode;

	private NavBarItem nav_frm_POProcess;

	private BarButtonItem menu_frm_ManagerQuery;

	private AlertControlEx alertMain;

	private System.Windows.Forms.Timer timer_Alert;

	private BarButtonItem bar_AutoFilterRow;

	private BarButtonItem bar_BestFitColumn;

	private BarStaticItem barStaticItem8;

	private BarStaticItem barStaticItem9;

	private BarStaticItem barStaticItem10;

	private BarStaticItem barStaticItem12;

	private BarEditItem barEditItem4;

	private RepositoryItemTextEdit repositoryItemTextEdit4;

	private BarButtonItem bar_DefaultBranch;

	private BarSubItem barSub_Integrated;

	private BarSubItem barSubItem24;

	private BarButtonItem bar_Expand;

	private BarButtonItem btn_ExportClipboard;

	private RepositoryItemSearchControl repositoryItemSearchControl1;

	private BarEditItem barEditItem5;

	private RepositoryItemTextEdit repositoryItemTextEdit5;

	private BarSubItem barSub_Setup;

	private BarSubItem barSub_SetupGeneral;

	private BarButtonItem barButtonItem6;

	private BarButtonItem barI_frm_LoginSap;

	private BarButtonItem menu_frm_ExchangeRatesAndIndexs;

	private BarButtonItem menu_frm_Users;

	private BarButtonItem menu_frm_SalesEmployeeBuyers;

	private BarButtonItem menu_frm_Freight;

	private BarButtonItem menu_frm_GeneSetting;

	private BarButtonItem menu_frm_FtpServerSetting;

	private BarSubItem barSub_Authorization;

	private BarButtonItem menu_frm_Authorization;

	private BarButtonItem menu_frm_DataOwnerShipAuthorization;

	private BarButtonItem barButtonItem28;

	private BarButtonItem menu_frm_FilterSetting;

	private BarButtonItem barButtonItem34;

	private BarSubItem barSub_SetupBp;

	private BarButtonItem menu_frm_CustomerGroup;

	private BarButtonItem menu_frm_VendorGroup;

	private BarButtonItem menu_frm_BusinessPartnerProperties;

	private BarButtonItem menu_frm_BusinessPartnerPriorities;

	private BarSubItem barSub_SetupInventory;

	private BarButtonItem menu_frm_ItemGroups;

	private BarSubItem barSub_SetupReport;

	private BarButtonItem menu_frm_PrintLayout;

	private BarButtonItem barButtonItem38;

	private BarButtonItem barButtonItem39;

	private BarButtonItem barButtonItem40;

	private BarButtonItem menu_frm_ItemProperties;

	private BarButtonItem menu_frm_WareHouses;

	private BarButtonItem menu_frm_UnitOfMeasure;

	private BarButtonItem menu_frm_UnitOfMeasureGroups;

	private BarButtonItem menu_frm_Shipping;

	private BarButtonItem menu_frm_LengthAndWidth;

	private BarButtonItem menu_frm_Weight;

	private BarButtonItem menu_frm_CustomsGroup;

	private BarButtonItem menu_frm_Manufacturer;

	private BarButtonItem barI_frm_Shipping;

	private BarButtonItem menu_frm_Locations;

	private BarSubItem barSub_Financial;

	private BarButtonItem menu_frm_Braches;

	private BarButtonItem menu_frm_Currencies;

	private BarButtonItem menu_frm_TaxGroups;

	private BarButtonItem barButtonItem56;

	private BarSubItem barSubItem29;

	private BarSubItem barSub_CustomizationTools;

	private BarButtonItem menu_frm_UserDefineFieldsManagement;

	private BarLinkContainerItem bsi_Cus;

	private BarButtonItem menu_frm_UDTManager;

	private BarSubItem barSub_UserDefineWindow;

	private BarSubItem barSub_ImportData;

	private BarButtonItem barButtonItem62;

	private BarButtonItem barButtonItem61;

	private BarButtonItem bar_ItemMasterData;

	private BarButtonItem barButtonItem63;

	private BarButtonItem menu_frm_PurchaseRequest;

	private BarButtonItem menu_frm_PurchaseQuotation;

	private BarButtonItem menu_frm_PurchaseOder;

	private BarButtonItem menu_frm_GoodReceiptPO;

	private BarButtonItem menu_frm_GoodsReturn;

	private BarButtonItem menu_frm_APInvoices;

	private BarButtonItem menu_frm_APCreditMemos;

	private BarButtonItem menu_frm_APReserveInvoices;

	private BarButtonItem menu_frm_LandedCosts;

	private BarButtonItem menu_frm_ProcurementConfirm;

	private BarButtonItem menu_frm_ItemMasterData;

	private BarSubItem barSub_InventoryTransaction;

	private BarButtonItem menu_frm_GoodsReceipt;

	private BarButtonItem menu_frm_GoodsIssue;

	private BarButtonItem menu_frm_InventoryTranferRequest;

	private BarButtonItem menu_frm_InventoryTranfer;

	private BarButtonItem menu_frm_InventoryRevaluation;

	private BarSubItem barSub_ItemManagement;

	private BarSubItem barSub_MRP;

	private BarButtonItem menu_frm_MRP;

	private BarButtonItem menu_frm_Schedule;

	private BarButtonItem menu_frm_GenPOFromSchedule;

	private BarButtonItem menu_frm_PrintBarCode;

	private BarButtonItem menu_frm_AutoReportComplete;

	private BarSubItem barSub_Service;

	private BarButtonItem menu_frm_Solution;

	private BarButtonItem menu_frm_ServiceCalls;

	private BarButtonItem menu_frm_Contract;

	private BarButtonItem menu_frm_ServiceCallSetting;

	private BarSubItem barSub_HRM;

	private BarSubItem barSub_RoutingResources;

	private BarButtonItem menu_frm_Operation;

	private BarButtonItem menu_frm_Resources;

	private BarSubItem barSub_Configuration;

	private BarButtonItem menu_frm_CompanyDetails;

	private BarButtonItem menu_frm_LoginSap;

	private BarButtonItem menu_frm_HumanResources;

	private BarSubItem barSub_Window;

	private BarSubItem barSub_SystemInitialization;

	private BarButtonItem menu_frm_DocumentNumbering;

	private BarSubItem barSub_BankingPayment;

	private BarButtonItem menu_frm_PaymentForInvoice;

	private BarButtonItem barSub_Banking;

	private BarButtonItem barButtonItem22;

	private BarButtonItem barButtonItem23;

	private BarButtonItem barButtonItem24;

	private BarSubItem barSub_Production;

	private BarButtonItem menu_frm_BillOfMaterials;

	private BarButtonItem menu_frm_ProductionOrder;

	private BarSubItem barSub_MTO;

	private BarButtonItem menu_frm_ProductionReqForBatch;

	private BarButtonItem menu_frm_ProductionForBatch;

	private BarButtonItem menu_frm_ProductionConfirm;

	private BarButtonItem menu_frm_PackingBatch;

	private BarButtonItem menu_frm_StockRequest;

	private BarButtonItem menu_frm_Project;

	private BarSubItem barSub_InventoryReport;

	private BarButtonItem menu_frm_BusinessPartnerCatalogNumbers;

	private BarSubItem barSub_Formula;

	private BarButtonItem menu_frm_AdvanceForecast;

	private BarButtonItem menu_FrmDefinedShift;

	private BarButtonItem menu_FrmWorkScheduling;

	private BarButtonItem menu_frm_Lines;

	private BarButtonItem menu_frm_ItemQuotaArrangement;

	private BarButtonItem menu_frm_BarcodeConfig;

	private BarButtonItem barButtonRefreshData;

	private BarButtonItem menu_frm_AdvanceBillOfMaterials;

	private BarButtonItem menu_frm_GenPurchaseFromSchedule;

	private BarSubItem barSub_Dashboard;

	private BarButtonItem bar_Schedule;

	private BarButtonItem menu_frm_SetupAdmin;

	private BarButtonItem menu_frm_DeliveryOrder;

	private BarButtonItem menu_frm_DeliveryConfirm;

	private BarButtonItem menu_frm_HandoverDocument;

	private BarButtonItem menu_frm_EmpToken;

	private BarButtonItem menu_frm_SalesControl;

	private BarButtonItem menu_frm_DeliveryControl;

	private BarButtonItem menu_frm_ReceiptBin;

	private BarButtonItem menu_frm_PickAndPack;

	private BarButtonItem barButtonItem_UpdateReport;

	private BarButtonItem bar_Alert;

	private IContainer components;

	private BarSubItem barSub_PriceLists;

	private BarButtonItem menu_frm_STDPriceLists;

	private BarButtonItem menu_frm_PeriodAndVolumeDiscounts;

	private BarSubItem barSubItem13;

	private BarButtonItem menu_frm_SpecialforBP;

	private BarButtonItem menu_frm_DiscountGroup;

	private DockManager dockMain;

	private DockPanel dockMessageLog;

	private ControlContainer dockPanel1_Container;

	private GridControlEx grd_SystemMessageLog;

	private BarCheckItem barSub_SystemMessageLog;

	private ImageList iml_Column;

	private ImageCollection imc_Data;

	private GridColumnEx gridColumn1;

	private RepositoryItemImageComboBox rep_Type;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	protected internal GridViewEx gv_SystemMessageLog;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private BarButtonItem menu_frm_Countries;

	private BarButtonItem barButtonItem25;

	private BarButtonItem barButtonItem26;

	private BarButtonItem barButtonItem27;

	private BarButtonItem barButtonItem29;

	private BarSubItem barSub_SetupBanking;

	private BarButtonItem menu_frm_CreditCardSetup;

	private BarButtonItem menu_frm_CreditPaymentMethods;

	private BarButtonItem menu_frm_ReceiptFromProduction;

	private BarButtonItem menu_frm_IssueForProduction;

	private BarButtonItem barButtonItem30;

	private BarSubItem barSub_Financials;

	private BarButtonItem menu_frm_DistributeRule;

	private AutoCompleteTextBox txt_MenuSearch;

	private BarButtonItem menu_frm_Dimension;

	private NavigationPane npane_Main;

	private NavigationPage npage_Operations;

	private NavBarGroup navGroupReport_Administration;

	private NavBarGroup navGroupReport_Sales;

	private NavBarGroup navGroupReport_Purchase;

	private NavBarGroup navGroupReport_BusinessPartner;

	private NavBarGroup navGroupReport_Inventory;

	private NavBarGroup navGroupReport_Production;

	private NavBarGroup navGroupReport_MRP;

	private NavBarGroup navGroupReport_HRM;

	private NavBarItem nav_frm_InventoryPostingList;

	private BarButtonItem menu_frm_MrpPlan;

	private BarButtonItem menu_frm_LicenseManager;

	private BarButtonItem menu_frm_CostCenters;

	private BarButtonItem menu_frm_ReportManager;

	private BarButtonItem menu_frm_BatchDetail;

	private BarButtonItem menu_frm_GenTranferRequestFromMrp;

	private NavBarItem nav_frm_StockMovementSummary;

	private BarButtonItem menu_frm_SerialNumberDetail;

	private BarButtonItem menu_frm_AR_ReserveInvoices;

	private BarButtonItem menu_frm_PaymentTerms;

	private BarButtonItem menu_frm_DocumentSettings;

	private BarButtonItem menu_frm_CycleCountDetermination;

	private BarButtonItem menu_frm_InventoryCycles;

	private BarButtonItem menu_frm_BatchesControl;

	private BarButtonItem menu_frm_JobVacancy;

	private BarSubItem barSub_SetupHumanResouces;

	private BarSubItem barSubItem2;

	private BarButtonItem menu_frm_QualificationsGroup;

	private BarButtonItem menu_frm_Qualifications;

	private BarButtonItem menu_frm_JobType;

	private BarButtonItem menu_frm_AdvertisementSource;

	private BarButtonItem menu_frm_JVApplicants;

	private BarButtonItem menu_frm_JVInterviewStatus;

	private BarButtonItem menu_frm_JVInterviewTypes;

	private BarButtonItem menu_frm_JVQuestion;

	private BarButtonItem menu_frm_JVBenefits;

	private XtraTabControlEx tab_MdiForm;

	private BarSubItem barSubItem4;

	private BarButtonItem menu_frm_WorkType;

	private BarButtonItem menu_frm_WorkCode;

	private BarButtonItem menu_frm_AvailableToPromise;

	private BarButtonItem menu_frm_ImportDataWizard;

	private BarButtonItem menu_frm_WorkPeriod;

	private BarButtonItem menu_frm_OvertimeType;

	private BarButtonItem menu_frm_GenPurchaseFromAtp;

	private BarSubItem barSubItem6;

	private BarButtonItem menu_frm_AllowanceType;

	private BarButtonItem menu_frm_DeductionType;

	private BarButtonItem menu_frm_PersonalIncomeTax;

	private BarButtonItem menu_frm_SalaryParameters;

	private BarButtonItem menu_frm_SalaryInfo;

	private BarButtonItem menu_frm_ScheduleForAtp;

	private BarButtonItem menu_frm_GenProductionFormAtp;

	private BarButtonItem menu_frm_TimeSheeting;

	private BarButtonItem menu_frm_MaterialShortageReport;

	private BarSubItem barSub_Sales;

	private BarButtonItem menu_frm_Vehicle;

	private BarButtonItem menu_frm_VehicleType;

	private BarButtonItem menu_frm_PickingRequest;

	private BarButtonItem menu_frm_TimeSheetingHour;

	private BarSubItem barSub_Purcharsing;

	private BarButtonItem menu_frm_LandedCostsSetup;

	private BarButtonItem barButtonItem31;

	private BarSubItem barSubItem18;

	private BarButtonItem menu_frm_ShippingDocumentPicked;

	private BarButtonItem menu_frm_ShippingDocumentLoaded;

	private BarButtonItem menu_frm_ShippingDocumentOutboundRoute;

	private BarButtonItem menu_frm_GenPurchaseFromMaterialShortage;

	private BarButtonItem menu_frm_QueryPreview;

	private BarButtonItem menu_frm_SalaryCalculation;

	private BarCheckItem bar_LogViewer;

	private BarButtonItem menu_frm_LogViewer;

	private BarButtonItem barButtonItem32;

	private BarButtonItem menu_frm_ProductionOrderImportTool;

	private BarButtonItem menu_frm_UserDefinedProcedure;

	private BarButtonItem menu_frm_ORW;

	private BarButtonItem barProcedure;

	private BarSubItem barSub_PlugIn;

	private BarButtonItem menu_frm_PluginManager;

	private BarSubItem barSub_Opportunity;

	private BarButtonItem menu_frm_SalesOpportunity;

	private BarCheckItem barButtonItem_ViewSystemInformation;

	private BarButtonItem barButtonItem21;

	private BarSubItem barSub_PickAndPack;

	private BarButtonItem menu_frm_PickingDocument;

	private BarButtonItem menu_frm_Packing;

	private BarButtonItem menu_frm_ShipDoc;

	private BarSubItem barSub_Opportunities;

	private BarButtonItem menu_frm_SalesOpportunityStage;

	private BarButtonItem menu_frm_SalesOpportunityPartner;

	private BarButtonItem menu_frm_SalesOpportunityCompetitor;

	private BarButtonItem menu_frm_SalesOpportunityRelationship;

	private BarButtonItem barButtonItem33;

	private BarButtonItem menu_frm_SalesOpportunityReason;

	private BarButtonItem menu_frm_PenalType;

	private BarButtonItem menu_frm_EmpPenalization;

	private BarButtonItem menu_frm_OutSourcing;

	private BarButtonItem menu_frm_DashboardManager;

	private BarButtonItem menu_frm_TaskManagement;

	private TransitionManager transitionManager1;

	private BarButtonItem menu_frm_DashboardDesigner;

	private BarButtonItem menu_frm_LeaveParameter;

	private BarButtonItem menu_frm_CashItemMasterData;

	private BarButtonItem menu_frm_CouponMasterData;

	private BarButtonItem menu_frm_VoucherMasterData;

	private BarButtonItem menu_frm_FacebookCRM;

	private BarButtonItem menu_frm_NoteManagement;

	private BarButtonItem menu_frm_Territory;

	private BarButtonItem menu_frm_SalesBlanketAgreement;

	private BarButtonItem menu_frm_EventManagement;

	private BarSubItem barSub_PM;

	private BarButtonItem menu_frm_TrackingIssue;

	private NavBarItem nav_frm_DocumentDraftsRP1;

	private NavBarItem nav_frm_DocumentDraftsRP2;

	private NavBarItem nav_frm_DocumentDraftsRP3;

	private NavBarItem nav_frm_BatchTransaction;

	private BarButtonItem menu_frm_UserDefinedTexts;

	private ImageCollection imc_Menu;

	private TreeList treeMenu;

	private TreeListColumnEx treeListColumn1;

	private BarSubItem barSub_UDO;

	private BarButtonItem menu_frm_ProjectManager;

	private BarButtonItem menu_frm_InventoryCountingBin;

	private BarSubItem barSubItem1;

	private BarButtonItem menu_frm_ContractTemplate;

	private BarButtonItem menu_frm_UDR;

	public BarEditItem bar_ProgessControl;

	public RepositoryItemProgressBar repositoryItemProgressBar1;

	private BarButtonItem menu_frm_MailSetting;

	private BarSubItem barSub_OpportunitiesReport;

	private BarSubItem barSub_SalesReports;

	private BarSubItem barSub_PurchasingReports;

	private BarSubItem barSub_BPReports;

	private BarSubItem barSub_ProductionReports;

	private BarSubItem barSub_MRPReports;

	private BarSubItem barSub_ServicesReports;

	private BarSubItem barSub_BankingReports;

	private BarSubItem barSub_ResourcesReports;

	private BarSubItem barSub_HRMReports;

	private BarSubItem barSub_ProjectReports;

	private BarButtonItem menu_frm_SalesDraftsReport;

	private BarButtonItem menu_frm_InventoryPostingList;

	private BarButtonItem menu_frm_StockMovementSummary;

	private BarButtonItem menu_frm_BatchTransaction;

	private BarButtonItem menu_frm_SerialNumberTransaction;

	private BarButtonItem menu_frm_PurchaseDraftReport;

	private BarButtonItem menu_frm_InventoryDraftsReport;

	private NotificationContrainer notificationContrainerMain;

	private BarButtonItem barBtnNotification;

	private BarButtonItem menu_frm_LeaveInformation;

	private BarButtonItem bar_EditWidget;

	private RepositoryItemTextEdit repositoryItemTextEdit6;

	private BarButtonItem bar_TableLayout;

	private BarButtonItem bar_StackLayout;

	private BarButtonItem bar_FlowLayout;

	private BarButtonItem bar_FreeLayout;

	private BarButtonItem menu_frm_LeaveCalculation;

	private BarButtonItem bar_SaveWidget;

	private AdornerUIManager adornerUIManager1;

	private Guide guide1;

	private Badge badge1;

	private BarButtonItem menu_frm_NotificationSetting;

	private BarButtonItem menu_frm_WidgetManagement;

	private BarButtonItem menu_frm_ProjectReportCompletion;

	private BarButtonItem menu_frm_ProjectConfirm;

	private BarButtonItem menu_frm_JetstarPreOrder;

	private BarButtonItem menu_frm_AdvanceInPayment;

	private BarButtonItem menu_frm_AdvanceOutPayment;

	private BarButtonItem menu_frm_AutoSelectBatchSerialConfig;

	private BarButtonItem menu_frm_InventoryCountingRequest;

	private BarButtonItem menu_frm_InventoryCounting;

	private BarSubItem barSub_Retail;

	private BarButtonItem menu_frm_PosProcessBar;

	private BarButtonItem menu_frm_LayoutSetup;

	private BarButtonItem menu_frm_CmdConfig;

	private BarButtonItem menu_frm_RetailStore;

	private BarButtonItem menu_frm_IssueAndReceiptLackQtyForProduction;

	private BarButtonItem barButtonItem20;

	private BarButtonItem menu_frm_WarrantyCard;

	private BarButtonItem menu_frm_ResourceWorkCenter;

	private BarButtonItem menu_frm_ServiceLevelAgreement;

	private BarButtonItem menu_frm_ServiceContractTemplate;

	private BarButtonItem menu_frm_BatchDetailManagement;

	private BarButtonItem menu_frm_Operations;

	private BarButtonItem menu_frm_PurchaseBlanketAgreement;

	private BarButtonItem menu_frm_AdvanceRouting;

	private BarButtonItem menu_frm_PosMaster;

	private BarButtonItem menu_frm_PosAssignment;

	private BarButtonItem menu_frm_MarketingCampaign;

	private BarButtonItem menu_frm_AlternativeItems;

	private BarButtonItem menu_frm_TemplateWizard;

	private BarButtonItem menu_frm_SerialNumberDetailManagement;

	private BarButtonItem menu_frm_DownPaymentRequest;

	private BarButtonItem menu_frm_DownPaymentInvoice;

	private BarButtonItem menu_frm_PurchaseDownPaymentRequest;

	private BarButtonItem menu_frm_PurchaseDownPaymentInvoice;

	private BarButtonItem menu_frm_ChangeLogList;

	private BarButtonItem menu_frm_ChangePassword;

	private BarButtonItem menu_frm_AssetMasterData;

	private BarButtonItem menu_frm_ExternalRouting;

	private BarButtonItem menu_frm_ResourceWorkScheduling;

	private BarButtonItem barButton_ChangeServerAddress;

	private BarSubItem barSubItem9;

	private BarButtonItem menu_frm_BiometricData;

	private BarButtonItem menu_frm_TimeSheetInOut;

	private BarButtonItem menu_frm_BiometricEmpMapping;

	private BarButtonItem menu_frm_BiometricConfiguration;

	private BarSubItem barSub_ProjectManagement;

	private BarButtonItem menu_frm_WBSTemplate;

	private BarButtonItem menu_frm_ContextActionPublishService;

	private BarButtonItem menu_frm_MenuStructure;

	private BarButtonItem bar_ExportXlsx;

	private BarButtonItem menu_frm_HolidaySetup;

	private BarButtonItem menu_frm_InOutMethod;

	private BarButtonItem menu_frm_WorkShift;

	private BarButtonItem menu_frm_WorkScheduleSetup;

	private BarButtonItem menu_frm_WorkScheduleRegister;

	private BarButtonItem menu_frm_BarcodeInventoryCounting;

	private BarButtonItem menu_frm_AdditionalGeneralSettings;

	private BarButtonItem menu_frm_ImportFiles;

	private BarButtonItem menu_frm_QuickCreationBPMasterData;

	private BarButtonItem menu_frm_JobsManagement;

	private BarSubItem barSub_DMS;

	private BarButtonItem menu_frm_Geography;

	private BarButtonItem menu_frm_SalesRoute;

	private BarSubItem barSub_PackingManagement;

	private BarButtonItem menu_frm_PackingManagement;

	private BarButtonItem menu;

	private BarButtonItem menu_frm_CreateTypeCase;

	private BarButtonItem barButtonItem35;

	private BarButtonItem barButtonItem36;

	private BarButtonItem barButtonItem37;

	private BarButtonItem menu_frm_PackingOrder;

	private BarButtonItem menu_frm_PrintBillWrap;

	private BarButtonItem menu_frm_Traceability;

	private BarButtonItem menu_frm_WorkingProcess;

	private BarButtonItem menu_frm_FacebookManager;

	private BarButtonItem menu_frm_IncomingPaymentWizard;

	private BarButtonItem menu_frm_GoodsReceiptPOByBarcode;

	private BarButtonItem menu_frm_InventoryTransferByBarcode;

	private BarButtonItem menu_frm_ReceiptFromPOByBarcode;

	private BarButtonItem menu_frm_PackingByBarcode;

	private BarButtonItem menu_frm_QuestionMasterData;

	private BarSubItem barSub_BinLocationSetup;

	private BarButtonItem menu_frm_FieldActivation;

	private BarButtonItem menu_frm_AttrCodeSetup;

	private BarButtonItem menu_frm_WHSubSetup;

	private BarButtonItem menu_frm_WHSubManager;

	private BarSubItem barSub_BinLocation;

	private BarButtonItem menu_frm_BinLocMasterData;

	private BarButtonItem barSub_PreviewExternalReport;

	private BarButtonItem menu_frm_OutletVisitingFrequency;

	private BarButtonItem menu_frm_PromotionSetup;

	private BarButtonItem menu_frm_SettingReport;

	private BarButtonItem menu_frm_Channel;

	private BarButtonItem menu_frm_PromotionCondition;

	private BarSubItem barSubItem10;

	private BarSubItem barSub_Danhmuc;

	private BarSubItem barSubItem15;

	private BarButtonItem menu_frm_PromotionBenefit;

	private BarButtonItem menu_frm_PromotionProgram;

	private BarSubItem barSubItem16;

	private BarButtonItem menu_frm_AdvancePaymentWizard;

	private BarSubItem barSubItem17;

	private BarButtonItem menu_frm_SalesCoverageGoogleMap;

	private BarSubItem barSub_KPI;

	private BarButtonItem barButtonItem42;

	private BarButtonItem menu_frm_KPIsSetting;

	private BarButtonItem menu_frm_KPIsMasterData;

	private BarButtonItem menu_frm_SetIndicatorKPIs;

	private BarSubItem barSub_ApprovalProcess;

	private BarButtonItem menu_frm_ApprovalStages;

	private BarButtonItem menu_frm_ApprovalTemplates;

	private BarButtonItem menu_frm_ListPointKPIs;

	private BarButtonItem menu_frm_UploadItemImage;

	private BarButtonItem menu_frm_ImageAlbum;

	private BarButtonItem barButtonItem41;

	private BarButtonItem menu_frm_ImageViewer;

	private BarButtonItem menu_frm_Marker;

	private BarButtonItem menu_frm_DMSAuth;

	private BarButtonItem menu_frm_AlertTemplate;

	private BarButtonItem menu_frm_DMSDelivery;

	private BarButtonItem menu_frm_ReturnRequest;

	private BarButtonItem menu_frm_ThirdPartyToken;

	private BarButtonItem menu_frm_UserOnlineDashboard;

	private BarButtonItem menu_frm_KPIPlan;

	private BarButtonItem menu_frm_KPIScope;

	private BarButtonItem menu_frm_KPIIndicator;

	private BarButtonItem menu_frm_KPIGroup;

	private BarButtonItem menu_frm_KPISetup;

	private BarButtonItem menu_frm_KPIScore;

	private BarButtonItem barButtonItem50;

	private BarButtonItem menu_frm_ItemImageViewer;

	private BarButtonItem menu_frm_NotificationEventManager;

	private BarButtonItem menu_frm_SettingApprovalDetail;

	private BarButtonItem menu_frm_AboutRangeTime;

	private BarButtonItem menu_frm_AccessLog;

	private BarButtonItem menu_frm_GoodsReturnRequest;

	private BarButtonItem menu_frm_DMSSettingUDF;

	private BarButtonItem bar_ShowFooter;

	private BarButtonItem bar_ExportXlsx2;

	private BarButtonItem bar_ExportXlsx3;

	private BarButtonItem bar_ExportXlsx4;

	private BarButtonItem barSub_RefreshUDF;

	private BarButtonItem menu_frm_AdminUploadAttachment;

	private BarToggleSwitchItem barToggle_Fontsize;

	private BarButtonItem menu_frm_ListDashboard;

	private BarButtonItem menu_frm_BiometricDataProcess;

	private TreeListNode HotTrackNode
	{
		get
		{
			return hotTrackNode;
		}
		set
		{
			if (hotTrackNode != value)
			{
				TreeListNode node = hotTrackNode;
				hotTrackNode = value;
				if (treeMenu.ActiveEditor != null)
				{
					treeMenu.PostEditor();
				}
				treeMenu.RefreshNode(node);
				treeMenu.RefreshNode(hotTrackNode);
			}
		}
	}

	public Main()
	{
		InitializeComponent();
		LoadConfigSkins();
		LoadSystemMessageLog();
		timer_AlertEvent = new System.Windows.Forms.Timer();
		timer_AlertEvent.Tick += TimerAlertEventOnTick;
	}

	private void TimerAlertEventOnTick(object sender, EventArgs e)
	{
		try
		{
			if (CheckShowAlert())
			{
				notificationContrainerMain.LoadAlertOverDueForEvent(alertMain, timer_AlertEvent.Interval);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadSystemMessageLog()
	{
		try
		{
			GlobalData.SystemMessageLog = new DataTable();
			GlobalData.SystemMessageLog.Columns.Add("Type", typeof(string));
			GlobalData.SystemMessageLog.Columns.Add("Message", typeof(string));
			GlobalData.SystemMessageLog.Columns.Add("Received", typeof(DateTime));
			GlobalData.SystemMessageLog.Columns.Add("HelpCode", typeof(string));
			grd_SystemMessageLog.DataSource = GlobalData.SystemMessageLog;
		}
		catch (Exception)
		{
		}
	}

	public void ShowForm(XtraFormEx frm)
	{
		frm.MdiParent = this;
		frm.Show();
	}

	public async void Find()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null && !popupForm.IsLoadForm)
			{
				await popupForm.MODE_FIND();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async void AddNew()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				if ((!(frmActive.FormAuth == Enums.FormAuthorities.NoAuthorities || frmActive.FormAuth == Enums.FormAuthorities.Read)))
				{
					await frmActive.REFRESH();
					if (frmActive is XtraFormEx xtraFormEx)
					{
						if (xtraFormEx.SyntaxOnAddMode == null)
						{
							string formulaText = _httpClient.Formula.GetFormulaText(xtraFormEx.FormId, Enums.EventId.OnAfterAddMode, xtraFormEx.FormId);
							xtraFormEx.SyntaxOnAddMode = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
						}
						if (xtraFormEx.SyntaxOnAddMode.Count != 0)
						{
							CtdMessage ctdMessage = new Compiler(Apzon.HttpClientHelper.Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(xtraFormEx, xtraFormEx.SyntaxOnAddMode, out var _);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
							}
						}
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public void CloseChildForm()
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				if (popupForm is XtraFormEx xtraFormEx)
				{
					xtraFormEx.IsFormClosing = true;
				}
				(popupForm as Form).Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void barButtonRefreshData_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				await frmActive.REFRESH_DATA();
				frmActive.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task First()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				await frmActive.MOVE_FIRST();
				frmActive.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task Previous()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				await frmActive.MOVE_PREVIOUS();
				frmActive.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task Next()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				await frmActive.MOVE_NEXT();
				frmActive.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task Last()
	{
		WinCommons._OpenProcessing();
		try
		{
			dynamic frmActive = ApzonCommon.GetPopupForm();
			if (frmActive != null && !frmActive.IsLoadForm)
			{
				await frmActive.MOVE_LAST();
				frmActive.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public Color GetSeparatorColor(NavBarControl navBar)
	{
		return CommonSkins.GetSkin(navBar.LookAndFeel).TranslateColor(SystemColors.ControlLightLight);
	}

	public void EnableDirectionalLayoutControl(Enums.DirectionalControl diectioControl, bool isEnable)
	{
		switch (diectioControl)
		{
		case Enums.DirectionalControl.PreviewLayout:
			barSub_PreviewLayout.Enabled = _previewLayoutAuth & isEnable;
			break;
		case Enums.DirectionalControl.PrintLayout:
		{
			BarButtonItem barButtonItem4 = barSub_Preview;
			bool enabled = (bar_PrintLayout.Enabled = isEnable);
			barButtonItem4.Enabled = enabled;
			break;
		}
		case Enums.DirectionalControl.Print:
		{
			BarButtonItem barButtonItem3 = barSub_Print;
			bool enabled = (barPrint.Enabled = isEnable);
			barButtonItem3.Enabled = enabled;
			break;
		}
		case Enums.DirectionalControl.ExportExcel:
		{
			BarButtonItem barButtonItem = barSub_Export;
			BarButtonItem barButtonItem2 = barSub_PreviewGridview;
			bool flag = (bar_ExportExcel.Enabled = isEnable);
			bool enabled = (barButtonItem2.Enabled = flag);
			barButtonItem.Enabled = enabled;
			bar_ExportXlsx.Enabled = isEnable;
			bar_ExportXlsx2.Enabled = isEnable;
			bar_ExportXlsx3.Enabled = isEnable;
			bar_ExportXlsx4.Enabled = isEnable;
			break;
		}
		case Enums.DirectionalControl.ExportWord:
			bar_ExportWord.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ExportPdf:
			bar_ExportPdf.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ExportClipboard:
			btn_ExportClipboard.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ChangeLog:
			menu_frm_ChangeLogList.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.Find:
			bar_Find.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.NewDoc:
			bar_NewDoc.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ButtonModeFirst:
			barButtonModeFirst.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ButtonModePrev:
			barButtonModePrev.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ButtonModeNext:
			barButtonModeNext.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ButtonLast:
			barButtonLast.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ButtonRefreshData:
			barButtonRefreshData.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.Procedure:
			barProcedure.Enabled = _diyAuth & isEnable;
			break;
		case Enums.DirectionalControl.AutoFilterRow:
			bar_AutoFilterRow.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.ShowFooter:
			bar_ShowFooter.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.BestFitColumn:
			bar_BestFitColumn.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.Expand:
			bar_Expand.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetEdit:
			bar_EditWidget.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetTable:
			bar_TableLayout.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetStack:
			bar_StackLayout.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetFlow:
			bar_FlowLayout.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetFree:
			bar_FreeLayout.Enabled = isEnable;
			break;
		case Enums.DirectionalControl.WidgetSave:
			bar_SaveWidget.Enabled = isEnable;
			break;
		}
	}

	private void Main_ShowProgressBar(int maximum, int minimum = 0, int step = 1, bool percentview = true)
	{
		try
		{
			if (!InvokeRequired)
			{
				bar_ProgessControl.Visibility = BarItemVisibility.Always;
				bar_ProgessControl.Reset();
				repositoryItemProgressBar1.Maximum = maximum;
				repositoryItemProgressBar1.Minimum = minimum;
				repositoryItemProgressBar1.Step = step;
				repositoryItemProgressBar1.PercentView = percentview;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Main_SetValueProgessBar(int value)
	{
		try
		{
			if (!InvokeRequired)
			{
				bar_ProgessControl.Reset();
				bar_ProgessControl.EditValue = value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Main_HideProgressBar(int sleep = 0)
	{
		try
		{
			if (!InvokeRequired)
			{
				Thread.Sleep(sleep);
				bar_ProgessControl.Reset();
				bar_ProgessControl.Visibility = BarItemVisibility.Never;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void SaveFontSizeConfig(string type)
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings["FontSize"] == null)
				{
					configuration.AppSettings.Settings.Add("FontSize", type);
				}
				else
				{
					configuration.AppSettings.Settings["FontSize"].Value = type;
				}
				configuration.Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", "Save font size config", ex.Message);
		}
	}

	public IEnumerable<Type> GetInheritanceHierarchy(Type type)
	{
		Type current = type;
		while (current != null)
		{
			yield return current;
			current = current.BaseType;
		}
	}

	private async void Main_Load(object sender, EventArgs e)
	{
		try
		{
			try
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
				if (configuration.AppSettings.Settings["EPPlus:ExcelPackage.LicenseContext"] == null)
				{
					configuration.AppSettings.Settings.Add("EPPlus:ExcelPackage.LicenseContext", "Commercial");
					configuration.Save();
					Application.Restart();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", "Set EPPlus config", ex.Message);
			}
			try
			{
				string startupFolderName = Function.GetStartupFolderName();
				string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
				if (File.Exists(text))
				{
					System.Configuration.Configuration configuration2 = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
					{
						ExeConfigFilename = text
					}, ConfigurationUserLevel.None);
					if (configuration2.AppSettings.Settings["FontSize"] != null && configuration2.AppSettings.Settings["FontSize"].Value == "L")
					{
						barToggle_Fontsize.Checked = true;
						AppearanceObject.DefaultFont = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 0.5f, AppearanceObject.DefaultFont.Unit);
						AppearanceObject.DefaultMenuFont = new Font(AppearanceObject.DefaultMenuFont.FontFamily, AppearanceObject.DefaultMenuFont.Size + 1.5f, AppearanceObject.DefaultMenuFont.Unit);
						treeMenu.RowHeight++;
					}
					if (configuration2.AppSettings.Settings["ColumnDashboard"] != null)
					{
						GlobalData.ColumnCountDashboard = Function.ParseInt(configuration2.AppSettings.Settings["ColumnDashboard"].Value);
					}
				}
				if (GlobalData.ColumnCountDashboard < 2 || GlobalData.ColumnCountDashboard > 10)
				{
					GlobalData.ColumnCountDashboard = 4;
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", "Get font size", ex2.Message);
			}
			barToggle_Fontsize.CheckedChanged += barToggle_Fontsize_CheckedChanged;
			AlertControlEx alertControlEx = alertMain;
			alertControlEx.ClearNotificationForm = (ClearNotification)Delegate.Combine(alertControlEx.ClearNotificationForm, new ClearNotification(alertMain_ClearNotification));
			GlobalData.ScreenLock = 30;
			_lockPanel.BackColor = Color.Transparent;
			tab_MdiForm.Visible = false;
			notificationContrainerMain.Visible = false;
			barBtnNotification.Visibility = BarItemVisibility.Never;
			txt_MsgBar.Appearance.ForeColor = Color.White;
			txt_MenuSearch.Size = new Size(txt_MenuSearch.Size.Width * WidScreen / 1920, txt_MenuSearch.Size.Height * HeightScreen / 1080);
			txt_MenuSearch.Location = new Point(Size.Width - txt_MenuSearch.Size.Width - 20, txt_MenuSearch.Location.Y * HeightScreen / 1080 + 5);
			XtraFormEx.EnableDirectionalControl = (EnableDirectionalControl)Delegate.Combine(XtraFormEx.EnableDirectionalControl, new EnableDirectionalControl(EnableDirectionalLayoutControl));
			XtraFormEx.ShowProgressBar = (ShowProgressBar)Delegate.Combine(XtraFormEx.ShowProgressBar, new ShowProgressBar(Main_ShowProgressBar));
			XtraFormEx.HideProgressBar = (HideProgressBar)Delegate.Combine(XtraFormEx.HideProgressBar, new HideProgressBar(Main_HideProgressBar));
			XtraFormEx.SetValueProgressBar = (SetValueProgressBar)Delegate.Combine(XtraFormEx.SetValueProgressBar, new SetValueProgressBar(Main_SetValueProgessBar));
			backColor = txt_MsgBar.Appearance.BackColor;
			EventStaticStatusBar();
			Application.AddMessageFilter(new KeystrokMessageFilter(this));
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object s, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
			int num;
			try
			{
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Apzon.HttpClientHelper.Configuration.ApiServer, "Commons");
				http.HttpClient.Timeout = new TimeSpan(0, 1, 0);
				num = await http.GetIntAsync("CheckConnectServer");
			}
			catch (Exception ex3)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
				num = 0;
			}
			if (num != 1)
			{
				using frm_ServerAdress frm_ServerAdress2 = new frm_ServerAdress(Apzon.HttpClientHelper.Configuration.ApiServer);
				frm_ServerAdress2.ShowDialog();
				if (frm_ServerAdress2.DialogResult != DialogResult.OK)
				{
					Exit = true;
					CloseAllChildForm();
					DisConnectSocket();
					Close();
					return;
				}
				Apzon.HttpClientHelper.Configuration.ApiServer = frm_ServerAdress2.ApiServer;
				string startupFolderName2 = Function.GetStartupFolderName();
				_ = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				string pathRoot2 = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				string text2 = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot2, Environment.UserName, startupFolderName2);
				if (File.Exists(text2))
				{
					System.Configuration.Configuration configuration3 = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
					{
						ExeConfigFilename = text2
					}, ConfigurationUserLevel.None);
					if (configuration3.AppSettings.Settings.AllKeys.Contains("ApiServer"))
					{
						configuration3.AppSettings.Settings["ApiServer"].Value = frm_ServerAdress2.ApiServer;
						configuration3.Save();
					}
				}
			}
			using (frm_LoginSap frm_LoginSap2 = new frm_LoginSap())
			{
				if (frm_LoginSap2.ShowDialog(this) == DialogResult.Cancel)
				{
					CloseAllChildForm();
					DisConnectSocket();
					Exit = true;
					Close();
					return;
				}
			}
			Task.Run(() =>
			{
				AddSkinsMenu();
			});
			_relogin = false;
			VisibleAndInvisubleMenubar(isVisible: true);
			XtraFormEx.DialogFormLocation = new Point(npane_Main.Location.X + npane_Main.ClientSize.Width + 5, npane_Main.Location.Y);
			XtraFormEx.DialogFormSize = new Size(Size.Width - npane_Main.ClientSize.Width - 30, ClientSize.Height - npane_Main.Location.Y - 15);
		}
		catch (Exception ex4)
		{
			Logging.Write("ERROR", ex4.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex4.Message);
		}
	}

	private async Task LoadPlugin()
	{
		_ = 1;
		try
		{
			DataTable data = await _httpClient.PluginManagerClient.GetData(GlobalData.DatabaseName);
			string[] source = (from t in data.Select()
				select Function.ToString(t["Name"])).ToArray();
			List<string> list = new List<string>();
			if (Directory.Exists(string.Format("{0}\\{2}\\{1}", Environment.CurrentDirectory, GlobalData.DatabaseName, "Extensions")))
			{
				list = Directory.GetDirectories(string.Format("{0}\\{2}\\{1}", Environment.CurrentDirectory, GlobalData.DatabaseName, "Extensions")).ToList();
			}
			for (int num = 0; num < list.Count; num++)
			{
				string text = Function.ToString(Path.GetFileName(list[num]));
				if (!source.Contains(text))
				{
					WinCommons.Info(Enums.MessageCode.UninstallPluginWithName, "", text);
					Function.DeleteFolder(list[num]);
					WinCommons.Success(Enums.MessageCode.UninstallPluginSuccess, "", text);
				}
			}
			for (int i = 0; i < data.Rows.Count; i++)
			{
				string plugInName = Function.ToString(data.Rows[i]["Name"]);
				if (Directory.Exists(string.Format("{0}\\{3}\\{2}\\{1}", Environment.CurrentDirectory, plugInName, GlobalData.DatabaseName, "Extensions")))
				{
					continue;
				}
				try
				{
					WinCommons.Info(Enums.MessageCode.UninstallPluginPleaseWait, "", plugInName);
					string text2 = await _httpClient.FileNetworkManagerClient.DownLoadFolder(string.Format("{0}\\{1}", "Extensions", plugInName), string.Format("{0}\\{1}\\{2}", Environment.CurrentDirectory, "Extensions", GlobalData.DatabaseName), isOverride: false);
					if (!string.IsNullOrEmpty(text2))
					{
						WinCommons.Error(Enums.MessageCode.InstallPlugInFailed, "", plugInName, text2);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.InstallPluginSuccess, "", plugInName);
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.InstallPlugInFailed, "", plugInName, ex.Message);
				}
			}
			for (int num2 = 0; num2 < data.Rows.Count; num2++)
			{
				if (!Function.ToString(data.Rows[num2]["Group"]).Equals("A"))
				{
					continue;
				}
				try
				{
					PluginManagement pluginManagement = new PluginManagement
					{
						CreateMainMenu = CreateMainMenu
					};
					new PluginLoader().LoadPlugins(pluginManagement, string.Format("{0}\\{1}", GlobalData.DatabaseName, Function.ToString(data.Rows[num2]["Name"])));
					foreach (IPluginApplication pluginApplication in pluginManagement.PluginApplications)
					{
						pluginApplication.MainForm = this;
						pluginApplication.HttpClient = _httpClient;
						pluginApplication.Initialize(Function.ParseInt(data.Rows[num2]["PlugId"]));
					}
				}
				catch (Exception)
				{
				}
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.ErrorLineNumber(ex3);
		}
	}

	public void CheckOnline(bool isNotifi = true, bool isChangeLicene = false)
	{
		try
		{
			if (Program.IsLoginOk)
			{
				Program.IsLoginOk = false;
				GlobalData.SessionId = "";
				if (isNotifi)
				{
					WinCommons.Error(Enums.MessageCode.YourSessionHasBeenLoackedOtherSessionStarted, "");
				}
				else if (isChangeLicene)
				{
					WinCommons.Error(Enums.MessageCode.TheLicenseHasBeenChangedToContinueExitOrLockTheSession, "");
				}
				_relogin = true;
				VisibleAndInvisubleMenubar(isVisible: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void SetChildFormForceClose(bool forceClose)
	{
		XtraFormEx.ExitApplication = forceClose;
	}

	private async void VisibleAndInvisubleMenubar(bool isVisible, bool changeUser = false)
	{
		try
		{
			GlobalData.ListAccessForm = null;
			GlobalData.FormSettings = new Dictionary<string, DataSet>();
			GlobalData.ListDocUDFNameAndType = null;
			GlobalData.ListLineUDFNameAndType = null;
			BeginInvoke((MethodInvoker)(() =>
			{
				tab_MdiForm.Visible = isVisible;
				tab_MdiForm.Visible = isVisible && MdiChildren.Any();
				MenuBar.Visible = isVisible;
				ToolBar.Visible = isVisible;
				txt_MenuSearch.Visible = isVisible;
				npane_Main.Visible = isVisible;
				barBtnNotification.Visibility = BarItemVisibility.Always;
				if (!isVisible)
				{
					barBtnNotification.Visibility = BarItemVisibility.Never;
					notificationContrainerMain.Visible = false;
					timer_Alert.Stop();
					timer_AlertEvent.Stop();
				}
			}));
			List<Form> openForms = ApzonCommon.GetListChildForm();
			if (isVisible)
			{
				if (changeUser)
				{
					treeMenu.DataSource = null;
				}
				Text = GlobalData.CompanyName;
				if (!(await CreateSocketCheckOnline()))
				{
					CheckOnline();
					return;
				}
				Task userLogin = GetUserLogin(GlobalData.UserName);
				if (changeUser)
				{
					SetChildFormForceClose(forceClose: true);
					foreach (Form item in openForms)
					{
						if (!item.IsDisposed && !item.Disposing)
						{
							item.Close();
						}
					}
					barButtonItem_ViewSystemInformation.Checked = false;
					if (dockMessageLog.Visibility == DockVisibility.Hidden)
					{
						barSub_SystemMessageLog.Checked = false;
					}
				}
				else
				{
					if (_activeFormMaximize && ActiveMdiChild != null)
					{
						ActiveMdiChild.WindowState = FormWindowState.Maximized;
					}
					for (int num = 0; num < openForms.Count; num++)
					{
						if (openForms[num].MdiParent == null && !openForms[num].IsDisposed && !openForms[num].Disposing)
						{
							openForms[num].Visible = true;
						}
					}
					foreach (Form item2 in openForms)
					{
						if (item2 == _activeForm && !item2.IsDisposed && !item2.Disposing)
						{
							item2.Activate();
							break;
						}
					}
					if (barSub_SystemMessageLog.Checked)
					{
						dockMessageLog.Visibility = DockVisibility.Visible;
					}
					if (barButtonItem_ViewSystemInformation.Checked)
					{
						XtraFormEx.ViewSystemInformation = true;
					}
				}
				SetChildFormForceClose(forceClose: false);
				if (Controls.Contains(_lockPanel))
				{
					Controls.Remove(_lockPanel);
				}
				LoadNotification();
				LoadPlugin();
				await userLogin;
				await LoadStaticData();
				_activeForm = new Form();
				return;
			}
			DisConnectSocket();
			if (!Program.IsLoginOk && _relogin)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					_relogin = false;
					_activeForm = ApzonCommon.GetActiveForm();
					if (_activeForm != null && _activeForm.WindowState == FormWindowState.Maximized)
					{
						_activeForm.WindowState = FormWindowState.Normal;
						_activeFormMaximize = true;
					}
					txt_MenuSearch.CloseAutoCompeteBox();
					dockMessageLog.Visibility = DockVisibility.Hidden;
					XtraFormEx.ViewSystemInformation = false;
					if (!Controls.Contains(_lockPanel))
					{
						Controls.Add(_lockPanel);
					}
					_lockPanel.Dock = DockStyle.Fill;
					_lockPanel.BringToFront();
					bar_User.Caption = "";
					for (int i = 0; i < openForms.Count; i++)
					{
						if (openForms[i].MdiParent == null)
						{
							if (string.IsNullOrEmpty(openForms[i].Text))
							{
								openForms[i].Close();
							}
							else
							{
								openForms[i].Hide();
							}
						}
					}
					WindowState = FormWindowState.Maximized;
					Activate();
					using frm_LoginSap frm_LoginSap2 = new frm_LoginSap();
					if (MdiChildren.Any((Form f) => f is XtraFormEx xtraFormEx && (xtraFormEx.FormMode == Enums.FormViewModeType.Add || xtraFormEx.FormMode == Enums.FormViewModeType.Update)))
					{
						frm_LoginSap2.HasActiveForm = true;
					}
					SetChildFormForceClose(forceClose: true);
					frm_LoginSap2.ShowDialog(this);
					if (frm_LoginSap2.DialogResult == DialogResult.Cancel)
					{
						_httpClient.LoginClient.LogOut(GlobalData.SessionId);
						Exit = true;
						Close();
					}
					else
					{
						VisibleAndInvisubleMenubar(isVisible: true, frm_LoginSap2.ChangeUser);
					}
				}));
			}
			await GetUserLogin("");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	private async Task LoadStaticData()
	{
		_ = 9;
		try
		{
			bar_User.Caption = "User: " + GlobalData.UserCodeLogin + " (" + GlobalData.UserNameLogin + ") | Integrated Resources System © " + DateTime.Today.Year + " by APZON v" + WinFuntions.GetVersionString();
			_getAlterMessage = true;
			LastActiveTime = DateTime.Now;
			Task<string> task1 = HttpClient.AdditionalGeneralSettingsClient.GetDefaultLng();
			Task<DataSet> task2 = HttpClient.AdditionalGeneralSettingsClient.GetData();
			Task task3 = Task.Run(() =>
			{
				GlobalData.CountryOrg = HttpClient.BusinessMaster.LoadComboCountry();
			});
			await Task.WhenAll(task1, task2, task3);
			Task<List<int>> dashboard = _httpClient.DashboardClient.GetListDashboardByUser(GlobalData.UserName);
			string text = await task1;
			if (!string.IsNullOrEmpty(text))
			{
				GlobalData.DEFAULT_LANGUAGE = text;
			}
			GlobalData.AdditionalGeneralSettings = await task2;
			string text2 = ResourcesHelper.LoadCurrentLanguage();
			GlobalData.CURRENTLANGUAGE = (string.IsNullOrEmpty(text2) ? GlobalData.DEFAULT_LANGUAGE : text2);
			LoadMessageDataCenter();
			Task t0 = LoadListAccessForm();
			List<Task> lstTask = new List<Task> { LoadDefaultData() };
			GlobalData.FormWizard = await _httpClient.FormWizard.GetValid();
			await CreateMenuUdt();
			lstTask.AddRange(new Task[7]
			{
				LoadGeneralSetting(),
				GetADM1(),
				LoadCompanyInfo(),
				GetDimensions(),
				LoadConfigFileSharingUpload(),
				LoadIRSConfig(),
				GetUserID()
			});
			await t0;
			List<int> list = await dashboard;
			if (list != null && list.Count != 0)
			{
				frm_ListDashboard frm_ListDashboard2 = frm_ListDashboard.MyDashboard();
				frm_ListDashboard2.ListDashboardId = list;
				frm_ListDashboard2.WindowState = FormWindowState.Maximized;
				frm_ListDashboard2.MdiParent = this;
				frm_ListDashboard2.Show();
			}
			barSub_RefreshUDF.Visibility = ((!GlobalData.ListAccessForm.Any((AuthsImpl a) => a.menucode == "frm_UserDefineFieldsManagement" && a.authoriza == "F")) ? BarItemVisibility.Never : BarItemVisibility.Always);
			Task task4 = Task.Run(() =>
			{
				HideMainBarSubItem(barSub_Module);
			});
			Task task5 = Task.Run(() =>
			{
				HideMainBarSubItem(barSub_Tool);
			});
			Task task6 = SettingLanguage();
			lstTask.AddRange(new Task[5]
			{
				CreateIntegratedMenu(),
				InsertQueryFilter(),
				LoadNotificationSetting(),
				LoadUserDefaultGroup(),
				CreateNavigateQueryReport()
			});
			await Task.WhenAll(task4, task5, task6);
			lstTask.AddRange(new Task[6]
			{
				CreateMenuQuery(),
				CreateMenuReport(),
				LoadDocumentStatic(),
				LoadAlertTime(),
				LoadModulesSettingLanguage(),
				GetSystemDate()
			});
			gv_SystemMessageLog.Columns["Received"].DisplayFormat.FormatType = FormatType.DateTime;
			gv_SystemMessageLog.Columns["Received"].DisplayFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm:ss";
			Task<DataTable> t4 = Task.Run(async () =>
			{
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Auth");
				return await hp.GetAsync("GetAllAuth", new
				{
					user = GlobalData.UserCodeLogin
				});
			});
			Task.Run(async () =>
			{
				GlobalData.ListCFLItemField = await _httpClient.Cfl.GetListCFLItemField();
			});
			await Task.WhenAll(lstTask);
			Task.Run(() =>
			{
				ReloadAllModule();
			});
			DataTable dataTable = await t4;
			if (dataTable.IsNotNull())
			{
				LoadFormDefault(dataTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	private async Task GetDimensions()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Sales");
		GlobalData.Dimensions = await hp.GetAsync("GetDimensions");
	}

	private void LoadFormDefault(DataTable _dtAuth)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataRow[] array = (from t in _dtAuth.AsEnumerable()
				where !CommonList.HiddenFunction.ContainsKey(Function.ToString(t["menucode"])) || (CommonList.HiddenFunction.ContainsKey(Function.ToString(t["menucode"])) && !CommonList.HiddenFunction[Function.ToString(t["menucode"])])
				select t).ToArray();
			if (array.Length != 0)
			{
				_dtAuth = array.CopyToDataTable();
				DataRow[] array2 = _dtAuth.Select($"isDefault='Y'");
				if (array2 != null)
				{
					DataRow[] array3 = array2;
					foreach (DataRow obj in array3)
					{
						string text = Function.ToString(obj["menucode"]);
						Function.ToString(obj["menucode"]);
						string text2 = Function.ToString(obj["MenuType"]);
						string caption = Function.ToString(obj["MenuName"]);
						switch (text2)
						{
						case "F":
						case "W":
						case "D":
							ShowForm(text);
							break;
						case "O":
							ShowUdo(text.Replace("frm_", ""), caption);
							break;
						case "T":
							ShowUdt(text.Replace("UDT_", ""), caption);
							break;
						case "R":
							ShowReportCrytal(text.Replace("frm_", ""));
							break;
						case "C":
							ShowReportForm(text.Replace("frm_", ""), caption);
							break;
						case "X":
							ShowReportXtra(text.Replace("frm_", ""), "");
							break;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task GetSystemDate()
	{
		try
		{
			GlobalData.SystemDate = await _httpClient.Commons.GetServerDate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResizeHomePage(XtraFormEx home)
	{
		try
		{
			home.Width = ClientSize.Width - npane_Main.ClientSize.Width - 6;
			home.Height = ClientSize.Height - 113;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> CreateSocketCheckOnline()
	{
		try
		{
			HubConnection checkConnection = CheckOnlineHub?.HubConnection;
			HubConnection notiConnection = UpdateNotiHub?.HubConnection;
			Task.Run(() =>
			{
				try
				{
					checkConnection?.Stop();
					notiConnection?.Stop();
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", "Stop Hub Connection", ex2.ToString(), ex2.InnerException?.ToString());
				}
			});
			CheckOnlineHub = new HubClientHelper(this);
			UpdateNotiHub = new HubClientHelper(this);
			Task task = CheckOnlineHub.CreateSocket<bool, bool>("NotificationHub", new Dictionary<string, string> { { "UpdateSession", "CheckOnline" } });
			Task task2 = UpdateNotiHub.CreateSocket<NotifiedObj>("NotificationClient", new Dictionary<string, string> { { "UpdateNotification", "UpdateNotification" } });
			await Task.WhenAll(task, task2);
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			return false;
		}
	}

	public async Task LoadNotificationSetting()
	{
		try
		{
			GlobalData.GlobalNotiSetting = new GlobalNotiSettingImpl();
			DataTable dataTable = await _httpClient.NotificationSettingClient.GetData(GlobalData.UserCodeLogin);
			if (dataTable.IsNotNull())
			{
				Function.ToString(dataTable.Rows[0][0]);
				string text = Function.ToString(dataTable.Rows[0][1]);
				int num = Function.ParseInt(dataTable.Rows[0][2]);
				int num2 = Function.ParseInt(dataTable.Rows[0][3]);
				string text2 = Function.ToString(dataTable.Rows[0][4]);
				string text3 = Function.ToString(dataTable.Rows[0][5]);
				int num3 = Function.ParseInt(dataTable.Rows[0][6]);
				int num4 = Function.ParseInt(dataTable.Rows[0][7]);
				GlobalData.GlobalNotiSetting.ListTimeReceive = new List<PeriodTime>();
				GlobalData.GlobalNotiSetting.checkReceiveNoti = false;
				switch (text)
				{
				case "R":
				{
					for (int k = num; k <= num2; k++)
					{
						if (DateTime.Now.DayOfWeek == (DayOfWeek)k)
						{
							GlobalData.GlobalNotiSetting.checkReceiveNoti = true;
							break;
						}
					}
					break;
				}
				case "AR":
					GlobalData.GlobalNotiSetting.checkReceiveNoti = true;
					break;
				case "P":
				{
					string[] array = Function.ToString(text2).Split(",".ToArray());
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] != "" && DateTime.Now.DayOfWeek == (DayOfWeek)Function.ParseInt(array[j]))
						{
							GlobalData.GlobalNotiSetting.checkReceiveNoti = true;
							break;
						}
					}
					break;
				}
				case "N":
				{
					for (int i = 0; i < 7; i++)
					{
						if ((i < num || i > num2) && DateTime.Now.DayOfWeek == (DayOfWeek)i)
						{
							GlobalData.GlobalNotiSetting.checkReceiveNoti = true;
							break;
						}
					}
					break;
				}
				}
				switch (text3)
				{
				case "R":
				{
					PeriodTime periodTime4 = new PeriodTime();
					periodTime4.startTime = num3;
					periodTime4.endTime = num4;
					GlobalData.GlobalNotiSetting.ListTimeReceive.Add(periodTime4);
					break;
				}
				case "N":
				{
					PeriodTime periodTime2 = new PeriodTime();
					periodTime2.startTime = 0;
					periodTime2.endTime = num3;
					PeriodTime periodTime3 = new PeriodTime();
					periodTime3.startTime = num4;
					periodTime3.endTime = 2359;
					GlobalData.GlobalNotiSetting.ListTimeReceive.Add(periodTime2);
					GlobalData.GlobalNotiSetting.ListTimeReceive.Add(periodTime3);
					break;
				}
				case "AR":
				{
					PeriodTime periodTime = new PeriodTime();
					periodTime.startTime = 0;
					periodTime.endTime = 2359;
					GlobalData.GlobalNotiSetting.ListTimeReceive.Add(periodTime);
					break;
				}
				}
			}
			else
			{
				GlobalData.GlobalNotiSetting.ListTimeReceive = new List<PeriodTime>();
				GlobalData.GlobalNotiSetting.checkReceiveNoti = true;
				PeriodTime periodTime5 = new PeriodTime();
				periodTime5.startTime = 0;
				periodTime5.endTime = 2359;
				GlobalData.GlobalNotiSetting.ListTimeReceive.Add(periodTime5);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public bool CheckShowAlert()
	{
		bool result = false;
		int num = Function.ParseInt(DateTime.Now.TimeOfDay.Hours) * 100 + Function.ParseInt(DateTime.Now.TimeOfDay.Minutes);
		List<PeriodTime> listTimeReceive = GlobalData.GlobalNotiSetting.ListTimeReceive;
		if (GlobalData.GlobalNotiSetting.checkReceiveNoti && ((listTimeReceive.Count == 1 && num >= listTimeReceive[0].startTime && num <= listTimeReceive[0].endTime) || (listTimeReceive.Count == 2 && ((num >= listTimeReceive[0].startTime && num < listTimeReceive[0].endTime) || (num > listTimeReceive[1].startTime && num <= listTimeReceive[1].endTime)))))
		{
			result = true;
		}
		return result;
	}

	public void UpdateNotification(NotifiedObj obj)
	{
		try
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				switch (obj.TranType)
				{
				case Enums.TransactionType.A:
				case Enums.TransactionType.U:
				case Enums.TransactionType.M:
					if ((obj.TranType == Enums.TransactionType.A || obj.TranType == Enums.TransactionType.U) && notificationContrainerMain.CheckExistNotified(obj.ObjectTypes, obj.NotifiedId))
					{
						notificationContrainerMain.UpdateNotification(obj);
					}
					else
					{
						obj.DateTimeFormat = GlobalData.DateTimeFormat + " hh:mm";
						notificationContrainerMain.AddNotified(obj);
						if (!obj.CreateUserCode.Equals(GlobalData.UserName) && CheckShowAlert())
						{
							AlertInfo info = new AlertInfo(obj.NotifiedTitle, obj.NotifiedBody, obj.NotifiedBody, null, obj);
							alertMain.ShowAlert(this, info, obj.TranType);
						}
					}
					break;
				case Enums.TransactionType.D:
				case Enums.TransactionType.C:
					notificationContrainerMain.ClearNotified(obj.ObjectTypes, obj.NotifiedId, obj.TranType);
					break;
				case Enums.TransactionType.L:
				case Enums.TransactionType.R:
					break;
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void alertMain_ClearNotification(ApzObjectTypes objType, string notifiId, Enums.TransactionType transactionType)
	{
		try
		{
			notificationContrainerMain.ClearNotified(objType, notifiId, transactionType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task CreateNavigateQueryReport()
	{
		try
		{
			DataTable dataTable = await _httpClient.Queries.ListOfQueryReport();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int id = Function.ParseInt(dataTable.Rows[0]["Id"]);
			if (id <= 0)
			{
				return;
			}
			NavBarItem navBarItem = new NavBarItem(Function.ToString(dataTable.Rows[0]["Name"]));
			navBarItem.Name = "nav_frm_" + id + Function.ToString(dataTable.Rows[0]["Name"]);
			navBarItem.Hint = Function.ToString(dataTable.Rows[0]["Dscription"]);
			navBarItem.Tag = "QueryReport";
			navBarItem.LinkClicked += (object s, NavBarLinkEventArgs v) =>
			{
				try
				{
					frm_ExtractQueryReportResult frm_ExtractQueryReportResult2 = new frm_ExtractQueryReportResult();
					frm_ExtractQueryReportResult2.ReportId = id;
					frm_ExtractQueryReportResult2.MdiParent = this;
					frm_ExtractQueryReportResult2.Show();
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			Enum.TryParse<Enums.ReportCategory>(Function.ToString(dataTable.Rows[0]["Category"]), out var result);
			switch (result)
			{
			case Enums.ReportCategory.Administration:
				navGroupReport_Administration.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Sales:
				navGroupReport_Sales.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Purchase:
				navGroupReport_Purchase.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.BusinessPartner:
				navGroupReport_BusinessPartner.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Inventory:
				navGroupReport_Inventory.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Production:
				navGroupReport_Production.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Mrp:
				navGroupReport_MRP.ItemLinks.Add(navBarItem);
				break;
			case Enums.ReportCategory.Hrm:
				navGroupReport_HRM.ItemLinks.Add(navBarItem);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAllMenu(BarSubItem item, int fatherId, string menutype, ref DataTable dt)
	{
		foreach (object itemLink in item.ItemLinks)
		{
			if (itemLink.GetType() == typeof(BarSubItemLink))
			{
				if (itemLink is BarSubItemLink barSubItemLink && !string.IsNullOrEmpty(barSubItemLink.Item.Name) && (!CommonList.HiddenFunction.ContainsKey(barSubItemLink.Item.Name.Replace("menu_", "").Replace("barSub_", "")) || !CommonList.HiddenFunction[barSubItemLink.Item.Name.Replace("menu_", "").Replace("barSub_", "")]) && !new string[7] { barSub_Reports.Name, barSub_Configuration.Name, barSub_Language.Name, barI_frm_LoginSap.Name, barSub_UserDefineWindow.Name, barSub_UDO.Name, "barSub_UserReport" }.Contains(barSubItemLink.Item.Name))
				{
					DataRow dataRow = dt.NewRow();
					dataRow["FatherId"] = fatherId;
					dataRow["Code"] = barSubItemLink.Item.Name;
					dataRow["Name"] = barSubItemLink.Caption;
					dataRow["LanguageKey"] = (string.IsNullOrEmpty(Function.ToString(barSubItemLink.Item.Tag)) ? barSubItemLink.Item.Name.Replace("frm_", "").Replace("barSub_", "") : Function.ToString(barSubItemLink.Item.Tag));
					dataRow["MenuType"] = "N";
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
					GetAllMenu(barSubItemLink.Item as BarSubItem, Function.ParseInt(dataRow["Id"]), menutype, ref dt);
				}
			}
			else if (itemLink is BarItemLink barItemLink && !string.IsNullOrEmpty(barItemLink.Item.Name) && (!CommonList.HiddenFunction.ContainsKey(barItemLink.Item.Name.Replace("menu_", "").Replace("barSub_", "")) || !CommonList.HiddenFunction[barItemLink.Item.Name.Replace("menu_", "").Replace("barSub_", "")]) && !new string[8] { barSub_Reports.Name, barSub_Configuration.Name, barSub_Language.Name, barI_frm_LoginSap.Name, barSub_UserDefineWindow.Name, barSub_UDO.Name, "barSub_UserReport", "menu_frm_ListDashboard" }.Contains(barItemLink.Item.Name))
			{
				DataRow dataRow2 = dt.NewRow();
				dataRow2["FatherId"] = fatherId;
				dataRow2["Code"] = barItemLink.Item.Name.Replace("menu_", "");
				dataRow2["LanguageKey"] = (string.IsNullOrEmpty(Function.ToString(barItemLink.Item.Tag)) ? barItemLink.Item.Name.Replace("menu_frm_", "").Replace("barSub_", "") : Function.ToString(barItemLink.Item.Tag));
				dataRow2["Name"] = barItemLink.Caption;
				dataRow2["MenuType"] = menutype;
				dt.Rows.Add(dataRow2);
				dt.AcceptChanges();
			}
		}
	}

	private async Task LoadDocumentStatic()
	{
		try
		{
			Logging.Write("TRACE", "System initialization starting...");
			List<Task> list = new List<Task>();
			list.AddRange(new Task[47]
			{
				Task.Run(async () =>
				{
					DocumentsHelper.Document = await _httpClient.Documents.Search(new DocumentsImpl
					{
						DocEntry = -1
					});
				}),
				Task.Run(async () =>
				{
					DocumentsHelper.Lines = await _httpClient.Documents.GetLine(-1);
					if (DocumentsHelper.Lines.ExistsColumn("LineNum"))
					{
						DocumentsHelper.Lines.Columns["LineNum"].DefaultValue = -1;
					}
				}),
				Task.Run(async () =>
				{
					DocumentsHelper.BatchSerialNumbersStruct = await _httpClient.Documents.LoadBatchNumberStruct();
				}),
				Task.Run(async () =>
				{
					ProductionOrderTableHelper.ProductionHeader = await _httpClient.ProductionOrders.GetADoc(0);
				}),
				Task.Run(async () =>
				{
					DataSet dataSet = await _httpClient.ProductionOrders.SearchLine(new ProductionOder_LineImpl_WOR1
					{
						DocEntry = 0
					});
					if (dataSet != null && dataSet.Tables.Count >= 1)
					{
						ProductionOrderTableHelper.ProductionLines = dataSet.Tables[0];
					}
				}),
				Task.Run(async () =>
				{
					ForecastTableHelper.Header = await _httpClient.AdvanceForecastClient.GetColumnDoc();
				}),
				Task.Run(() =>
				{
					ForecastTableHelper.Items = _httpClient.AdvanceForecastClient.GetColumnLine();
				}),
				Task.Run(async () =>
				{
					if (BussinessPartnerTableHelper.BusinessPartner == null)
					{
						BussinessPartnerTableHelper.BusinessPartner = await _httpClient.BusinessMaster.GetADoc("");
					}
				}),
				Task.Run(() =>
				{
					if (BussinessPartnerTableHelper.BPAddresses == null)
					{
						BussinessPartnerTableHelper.BPAddresses = _httpClient.BusinessMaster.GetAddressByCardCode("");
					}
				}),
				Task.Run(() =>
				{
					if (BussinessPartnerTableHelper.ContactEmployees == null)
					{
						BussinessPartnerTableHelper.ContactEmployees = _httpClient.BusinessMaster.ShowContactPerson("");
					}
				}),
				Task.Run(() =>
				{
					if (ItemsTableHelper.Items == null)
					{
						ItemsTableHelper.Items = _httpClient.Items.GetADoc("");
					}
				}),
				Task.Run(() =>
				{
					if (ItemsTableHelper.ItemPrices == null)
					{
						ItemsTableHelper.ItemPrices = _httpClient.Items.GetPrice("");
					}
				}),
				Task.Run(() =>
				{
					if (ItemsTableHelper.ItemWarehouses == null)
					{
						ItemsTableHelper.ItemWarehouses = _httpClient.Items.GetItemWarehouse("");
					}
				}),
				Task.Run(() =>
				{
					ProductTreeTableHelper.ProductTreeHeader = _httpClient.BillOfMasterials.SearchADoc("");
				}),
				Task.Run(() =>
				{
					ProductTreeTableHelper.ProductTreeLines = _httpClient.BillOfMasterials.ShowITT1("");
				}),
				Task.Run(async () =>
				{
					JournalEntryTableHelper.JournalEntry = await _httpClient.JournalEntry.GetDoc(0);
				}),
				Task.Run(async () =>
				{
					JournalEntryTableHelper.JournalEntryRows = await _httpClient.JournalEntry.GetLines(0);
				}),
				Task.Run(async () =>
				{
					ActivitiesTableHelper.Acivity = await _httpClient.SubActivities.GetStruct();
				}),
				Task.Run(async () =>
				{
					ProjectManagementTableHelper.Project = await _httpClient.ProjectManager.GetStruct();
				}),
				Task.Run(async () =>
				{
					ProjectManagementTableHelper.ProjectDataLines = await _httpClient.ProjectManager.GetDataById(-1);
					if (ProjectManagementTableHelper.ProjectDataLines.Tables.Count > 0)
					{
						ProjectManagementTableHelper.ProjectWorkBreakdownStructure = ProjectManagementTableHelper.ProjectDataLines.Tables[0];
					}
					if (ProjectManagementTableHelper.ProjectDataLines.Tables.Count > 2)
					{
						ProjectManagementTableHelper.ProjectBudget = ProjectManagementTableHelper.ProjectDataLines.Tables[2];
					}
				}),
				Task.Run(async () =>
				{
					OpportunityTableHelper.Opportunity = await _httpClient.OpportunityClient.GetDoc(0, GlobalData.UserCodeLogin);
				}),
				Task.Run(async () =>
				{
					OpportunityTableHelper.OpportunityStages = await _httpClient.OpportunityClient.GetStage(0);
				}),
				Task.Run(async () =>
				{
					OpportunityTableHelper.OpportunityItems = await _httpClient.OpportunityClient.GetItemDetail(0);
				}),
				Task.Run(async () =>
				{
					OpportunityTableHelper.Partner = await _httpClient.OpportunityClient.GetPartner(0);
				}),
				Task.Run(async () =>
				{
					OpportunityTableHelper.Competitors = await _httpClient.OpportunityClient.GetCompetitor(0);
				}),
				Task.Run(() =>
				{
					if (OpportunityTableHelper.LinkActivity == null)
					{
						OpportunityTableHelper.LinkActivity = new DataTable();
						OpportunityTableHelper.LinkActivity.Columns.Add("ClgCode", typeof(string));
						OpportunityTableHelper.LinkActivity.Columns.Add("OprId", typeof(string));
						OpportunityTableHelper.LinkActivity.Columns.Add("OprLine", typeof(int));
					}
					OpportunityTableHelper.Attachment = WinFuntions.GetTableStructure("ATC1");
					OpportunityTableHelper.Interest = WinFuntions.GetTableStructure("OPR4");
					if (OpportunityTableHelper.Reason == null)
					{
						OpportunityTableHelper.Reason = new DataTable();
						OpportunityTableHelper.Reason.Columns.Add(new DataColumn("OpportId", typeof(int)));
						OpportunityTableHelper.Reason.Columns.Add(new DataColumn("Line", typeof(int)));
						OpportunityTableHelper.Reason.Columns.Add(new DataColumn("ReasondId", typeof(int)));
					}
				}),
				Task.Run(async () =>
				{
					DataSet dataSet = await _httpClient.TrackingIssue.GetStructure();
					if (dataSet.Tables.Count > 1)
					{
						TrackingIssueTableHelper.TrackingIssues = dataSet.Tables[0];
						TrackingIssueTableHelper.TrackingHistory = dataSet.Tables[1];
					}
					else
					{
						Logging.Write("TRACE", "Could not get structure of tracking issue");
					}
				}),
				Task.Run(async () =>
				{
					if (ChartOfAccountTableHelper.Account == null)
					{
						ChartOfAccountTableHelper.Account = await _httpClient.ChartOfAccountClient.GetStruct();
					}
				}),
				Task.Run(async () =>
				{
					if (DistributeRuleTableHelper.DistributeRule == null)
					{
						DistributeRuleTableHelper.DistributeRule = await _httpClient.DistributeRule.GetStruct();
					}
				}),
				Task.Run(async () =>
				{
					if (DistributeRuleTableHelper.DistributionRuleRows == null)
					{
						DistributeRuleTableHelper.DistributionRuleRows = await _httpClient.DistributeRule.GetStructRow();
					}
				}),
				Task.Run(async () =>
				{
					if (VatGroupTableHelper.VatGroup == null)
					{
						VatGroupTableHelper.VatGroup = await _httpClient.TaxGroupsClient.GetStructHeader();
					}
				}),
				Task.Run(async () =>
				{
					if (VatGroupTableHelper.VatGroupLines == null)
					{
						VatGroupTableHelper.VatGroupLines = await _httpClient.TaxGroupsClient.GetStructLines();
					}
				}),
				Task.Run(async () =>
				{
					BlanketTableHelper.Blanket = await _httpClient.BlanketAgreement.GetDoc(new DocumentSBAImpl());
				}),
				Task.Run(async () =>
				{
					BlanketTableHelper.BlanketRows = await _httpClient.BlanketAgreement.GetLine(0);
				}),
				Task.Run(() =>
				{
					BlanketTableHelper.BlanketDetails = _httpClient.BlanketAgreement.GetBlanketAgreementDetails(new Tuple<int, int>(0, 0));
				}),
				Task.Run(() =>
				{
					IncommingPaymentTableHelper.Payment = WinFuntions.GetTableStructure("ORCT");
				}),
				Task.Run(() =>
				{
					IncommingPaymentTableHelper.PaymentInvoices = WinFuntions.GetTableStructure("RCT2");
				}),
				Task.Run(() =>
				{
					IncommingPaymentTableHelper.PaymentAccounts = WinFuntions.GetTableStructure("RCT4");
				}),
				Task.Run(() =>
				{
					IncommingPaymentTableHelper.PaymentChecks = WinFuntions.GetTableStructure("RCT1");
				}),
				Task.Run(() =>
				{
					JournalEntryTableHelper.JournalVoucher = WinFuntions.GetTableStructure("OBTF");
				}),
				Task.Run(() =>
				{
					JournalEntryTableHelper.JournalVoucherRows = WinFuntions.GetTableStructure("BTF1");
				}),
				Task.Run(() =>
				{
					CtxTableHelper.PurchaseLandedCosts = WinFuntions.GetTableStructure("OIPF");
				}),
				Task.Run(() =>
				{
					CtxTableHelper.PurchaseLandedCosts_Costs = WinFuntions.GetTableStructure("IPF2");
				}),
				Task.Run(() =>
				{
					CtxTableHelper.PurchaseLandedCosts_Customs = WinFuntions.GetTableStructure("IPF3");
				}),
				Task.Run(() =>
				{
					CtxTableHelper.PurchaseLandedCosts_Rows = WinFuntions.GetTableStructure("IPF1");
				}),
				Task.Run(async () =>
				{
					DataTable dataTable = await _httpClient.Commons.GetEInvoiceToken(GlobalData.UserCodeLogin);
					if (dataTable.IsNotNull())
					{
						GlobalData.ApiToken = Function.ToString(dataTable.Rows[0]["U_TOKEN"]);
					}
				}),
				Task.Run(() =>
				{
					WarehousesTableHelper.Warehouse = WinFuntions.GetTableStructure("OWHS");
				})
			});
			await Task.WhenAll(list);
			Logging.Write("TRACE", "System initialization success");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("TRACE", "System initialization failure");
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadBarUdt()
	{
		try
		{
			using HttpClientHelper<UDTsImpl> helper = new HttpClientHelper<UDTsImpl>(Apzon.HttpClientHelper.Configuration.ApiServer, "UDT");
			List<UDTsImpl> list = await helper.GetListAsync("LoadBarUdt");
			barSub_UserDefineWindow.ItemLinks.Clear();
			barSub_UserDefineWindow.ClearLinks();
			for (int i = 0; i < list.Count; i++)
			{
				BarButtonItem barButton = new BarButtonItem
				{
					Name = list[i].TableName,
					Caption = list[i].TableName + " - " + list[i].Description
				};
				string tx = list[i].TableName + "  " + list[i].Description;
				barButton.ItemClick += (object s, ItemClickEventArgs e) =>
				{
					frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(barButton.Name);
					frm_SetUpUDT2.Text = tx;
					frm_SetUpUDT2.MdiParent = this;
					frm_SetUpUDT2.Show();
				};
				barSub_UserDefineWindow.AddItem(barButton);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadUserDefaultGroup()
	{
		try
		{
			GlobalData.UserDefaultGroup = await _httpClient.User.GetUserDefaultGroup(GlobalData.UserName);
			if (GlobalData.UserDefaultGroup == null)
			{
				GlobalData.UserDefaultGroup = new UserDefaultGroupImpl();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task CreateIntegratedMenu()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<string> hp = new HttpClientHelper<string>(Apzon.HttpClientHelper.Configuration.ApiServer, "Integrated"))
			{
				string text = await hp.GetAsync("CheckIRSAuthorizations");
				if (!string.IsNullOrEmpty(text))
				{
					Logging.Write("Check IRS integrated Authorizations failed: [" + text + "]");
				}
			}
			using HttpClientHelper<DataTable> hp2 = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Integrated");
			DataTable dataTable = await hp2.GetAsync("GetListConfig");
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			barSub_Integrated.ItemLinks.Clear();
			foreach (DataRow row in dataTable.Rows)
			{
				BarButtonItem barBtn = new BarButtonItem();
				barBtn.Caption = "";
				switch (Function.ToString(row["ObjType"]))
				{
				case "4":
					CreateIntegratedBarButtonItem(ref barBtn, row, "menu_frm_Integrate_Item", ApzObjectTypes.oItems);
					break;
				case "17":
					CreateIntegratedBarButtonItem(ref barBtn, row, "menu_frm_Integrate_Document_SaleOrders", ApzObjectTypes.oOrders);
					break;
				case "206":
					CreateIntegratedBarButtonItem(ref barBtn, row, "menu_frm_IntegratedUdo", ApzObjectTypes.oUserDefinedObject);
					break;
				}
				if (barBtn.Caption != "")
				{
					barSub_Integrated.ItemLinks.Add(barBtn);
					BarManager.Items.Add(barBtn);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateIntegratedBarButtonItem(ref BarButtonItem barBtn, DataRow row, string menuName, ApzObjectTypes objType, bool isReserve = false)
	{
		try
		{
			string caption = Function.ToString(row["TypeName"]);
			barBtn.Caption = caption;
			barBtn.Name = menuName;
			barBtn.ItemClick += (object s, ItemClickEventArgs e) =>
			{
				try
				{
					dynamic val = null;
					switch (objType)
					{
					case ApzObjectTypes.oInvoices:
					case ApzObjectTypes.oDeliveryNotes:
					case ApzObjectTypes.oReturns:
					case ApzObjectTypes.oOrders:
					case ApzObjectTypes.oPurchaseInvoices:
					case ApzObjectTypes.oPurchaseDeliveryNotes:
					case ApzObjectTypes.oPurchaseReturns:
					case ApzObjectTypes.oPurchaseOrders:
					case ApzObjectTypes.oQuotations:
					case ApzObjectTypes.oPurchaseQuotations:
					case ApzObjectTypes.oPurchaseRequest:
						val = new frm_Integrate_Document
						{
							Text = "Integrated - " + caption,
							_isIns = (isReserve ? "Y" : "N"),
							_formObjType = objType
						};
						break;
					case ApzObjectTypes.oUserDefinedObject:
						val = new frm_IntegratedUdo
						{
							Text = "Integrated - " + caption
						};
						break;
					case ApzObjectTypes.oItems:
						val = new frm_Integrate_Item
						{
							Text = "Integrated - " + caption
						};
						break;
					}
					if (val != null)
					{
						val.MdiParent = this;
						val.Show();
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			if (!_integratedBarbutton.ContainsKey(menuName.Substring(5)))
			{
				_integratedBarbutton.Add(menuName.Substring(5), barBtn);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task InsertQueryFilter()
	{
		try
		{
			await _httpClient.FilterSetting.InsertQueryFilter(GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadAlertTime()
	{
		try
		{
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "WorkingTable"))
			{
				DataTable dataTable = await hp.GetAsync("GetUserSettings");
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					GlobalData.AlertTime = Function.ParseInt(dataTable.Rows[0][0]) * 60000;
					GlobalData.ScreenLock = Function.ParseInt(dataTable.Rows[0][1]);
				}
				else
				{
					GlobalData.AlertTime = 0;
					GlobalData.ScreenLock = 30;
				}
			}
			alertMain.AutoFormDelay = 20000;
			if (GlobalData.AlertTime > 0)
			{
				if (GlobalData.AlertTime < 60000)
				{
					GlobalData.AlertTime = 300000;
				}
				timer_Alert.Interval = GlobalData.AlertTime;
				timer_Alert.Start();
			}
			else
			{
				timer_Alert.Stop();
			}
			timer_AlertEvent.Start();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetUserID()
	{
		try
		{
			string userCodeLogin = GlobalData.UserCodeLogin;
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Users");
			DataTable dataTable = await hp.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				GlobalData.UserID = Function.ParseInt(dataTable.Rows[0]["USERID"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetStoreCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "RetailStore");
			DataTable dataTable = await helper.GetAsync("LoadAllDoc");
			if (!dataTable.IsNotNull())
			{
				Logging.Write("ERROR", PoinOfSale.msg_GetStoreErr);
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault();
			if (dataRow != null)
			{
				GlobalData.StoreCode = Function.ToString(dataRow["Code"]);
				GlobalData.POSID = 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadIRSConfig()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Commons");
			DataTable dataTable = await hp.GetAsync("GetIRSConfig");
			if (dataTable.Rows.Count > 0)
			{
				if (dataTable.Rows[0] != null)
				{
					GlobalData.ICfg_OppByUser = ((dataTable.Rows[0]["OppByUser"] == DBNull.Value || dataTable.Rows[0]["OppByUser"] == null) ? "N" : dataTable.Rows[0]["OppByUser"].ToString());
					GlobalData.ICfg_OppForecast = ((dataTable.Rows[0]["OppForecast"] == DBNull.Value || dataTable.Rows[0]["OppForecast"] == null) ? "N" : dataTable.Rows[0]["OppForecast"].ToString());
					GlobalData.ICfg_OppPotential = ((dataTable.Rows[0]["OppPotential"] == DBNull.Value || dataTable.Rows[0]["OppPotential"] == null) ? "N" : dataTable.Rows[0]["OppPotential"].ToString());
				}
				else
				{
					GlobalData.ICfg_OppByUser = "N";
					GlobalData.ICfg_OppForecast = "N";
					GlobalData.ICfg_OppPotential = "N";
				}
			}
			else
			{
				GlobalData.ICfg_OppByUser = "N";
				GlobalData.ICfg_OppForecast = "N";
				GlobalData.ICfg_OppPotential = "N";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDefaultData(bool clickChooseBranch = false)
	{
		_ = 1;
		try
		{
			bar_DefaultBranch.Enabled = false;
			using (HttpClientHelper<DataTable> hp_1 = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Commons"))
			{
				DataTable dataTable = await hp_1.GetAsync("GetBranchList", new
				{
					userCode = GlobalData.UserCodeLogin
				});
				if (dataTable.Rows.Count > 1)
				{
					string text = "";
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["BPLId"]) == GlobalData.DefaultBranch);
					if (dataRow != null)
					{
						text = Function.ToString(dataRow["BPLName"]);
					}
					if (clickChooseBranch || dataRow == null)
					{
						frm_ChooseDfltBranch frm_ChooseDfltBranch2 = new frm_ChooseDfltBranch();
						frm_ChooseDfltBranch2._dt = dataTable;
						frm_ChooseDfltBranch2.TopMost = true;
						frm_ChooseDfltBranch2.ShowDialog();
						if (frm_ChooseDfltBranch2.branchName != "")
						{
							text = frm_ChooseDfltBranch2.branchName;
						}
					}
					string text2 = Text.GetCharsBefore(", Branch: ");
					if (text2 == "")
					{
						text2 = GlobalData.CompanyName;
					}
					Text = text2 + ", Branch: " + text;
					bar_DefaultBranch.Enabled = true;
				}
				else
				{
					if (dataTable.Rows.Count == 1)
					{
						GlobalData.DefaultBranch = ((dataTable.Rows[0]["BPLId"] == DBNull.Value || dataTable.Rows[0]["BPLId"] == null) ? (-1) : Function.ParseInt(dataTable.Rows[0]["BPLId"]));
						string text3 = Text.GetCharsBefore(", Branch: ");
						if (text3 == "")
						{
							text3 = GlobalData.CompanyName;
						}
						Text = text3 + ", Branch: " + Function.ToString(dataTable.Rows[0]["BPLName"]);
					}
					else
					{
						GlobalData.DefaultBranch = -1;
						Text = GlobalData.CompanyName;
					}
					bar_DefaultBranch.Enabled = false;
				}
				GlobalData.BranchList = dataTable;
			}
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Commons");
			DataTable dataTable2 = await hp.GetAsync("GetDefaultData", new
			{
				userCode = GlobalData.UserCodeLogin,
				branch = GlobalData.DefaultBranch
			});
			if (dataTable2.Rows.Count > 0)
			{
				if (dataTable2.Rows[0][0] != null && dataTable2.Rows[0][0].ToString() == "Y")
				{
					GlobalData.IsMultiBranch = true;
				}
				else
				{
					GlobalData.IsMultiBranch = false;
					bar_DefaultBranch.Enabled = false;
				}
				GlobalData.DefaultWhs = Function.ToString(dataTable2.Rows[0][1]);
			}
			else
			{
				GlobalData.IsMultiBranch = false;
				bar_DefaultBranch.Enabled = false;
				GlobalData.DefaultWhs = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadListAccessForm()
	{
		treeMenu.ShowLoadingPanel();
		try
		{
			string user = ((!GlobalData.LoginSap) ? GlobalData.UserCodeLogin : GlobalData.UserName);
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Auth"))
			{
				DataTable dataTable = await hp.GetAsync("GetAllAuthForm", new
				{
					user = user,
					lngCode = GlobalData.CURRENTLANGUAGE
				});
				GlobalData.DtAccessForm = dataTable.Clone();
				string[] formTypeCheckLicense = new string[2] { "F", "L" };
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !GlobalData.InvisibleOnSideMenu.Contains(Function.ToString(t["Code"])) && ((Function.ToString(t["MenuType"]) != "N" && Function.ToString(t["MenuType"]) != "L" && Function.ToString(t["MenuType"]) != "A" && Function.ToString(t["authoriza"]) != "N") || Function.ToString(t["MenuType"]) == "N" || Function.ToString(t["MenuType"]) == "A") && !new string[3] { barSub_Queries.Name, barSub_CustomizationTools.Name, barSub_ImportData.Name }.Contains(Function.ToString(t["Code"]))
					select t;
				if (source.Any())
				{
					if (!GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser))
					{
						source = source.Where((DataRow t) => GlobalData.UserLicense != null && GlobalData.UserLicense.UserRule != LicenseRule.Service && (!formTypeCheckLicense.Contains(Function.ToString(t["MenuType"])) || GlobalData.UserLicense.UserRule == LicenseRule.Super || GlobalData.UserLicense.LstAccessFunctionOfUser.Contains(Function.ToString(t["menucode"])))).Select((DataRow r) =>
						{
							r["Name"] = Function.ToString(r["Name"]).Replace(" && ", " & ");
							return r;
						});
					}
					if (source.Any())
					{
						GlobalData.DtAccessForm = source.CopyToDataTable();
						ReloadFatherMenu();
					}
				}
				if (GlobalData.ListAccessForm == null)
				{
					GlobalData.ListAccessForm = new List<AuthsImpl>();
				}
				GlobalData.ListAccessForm.Clear();
				foreach (DataRow row in dataTable.Rows)
				{
					if (!string.IsNullOrEmpty(Function.ToString(row["menucode"])))
					{
						AuthsImpl authsImpl = new AuthsImpl();
						authsImpl.authoriza = Function.ToString(row["authoriza"]);
						authsImpl.MenuType = Function.ToString(row["MenuType"]);
						if (!GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser) && (GlobalData.UserLicense == null || GlobalData.UserLicense.UserRule == LicenseRule.Service || (formTypeCheckLicense.Contains(authsImpl.MenuType) && GlobalData.UserLicense.UserRule == LicenseRule.Normal && !GlobalData.UserLicense.LstAccessFunctionOfUser.Contains(Function.ToString(row["menucode"])))))
						{
							authsImpl.authoriza = "N";
						}
						authsImpl.menucode = Function.ToString(row["menucode"]);
						authsImpl.menuName = Function.ToString(row["MenuName"]);
						authsImpl.LanguageKey = Function.ToString(row["LanguageKey"]);
						GlobalData.ListAccessForm.Add(authsImpl);
					}
				}
				GlobalData.ListAccessForm.AddRange(GlobalData.PublicAccessForm);
			}
			GlobalData.ListAccessForm.Add(new AuthsImpl
			{
				menucode = "frm_ListDashboard",
				authoriza = "F"
			});
			_previewLayoutAuth = GlobalData.ListAccessForm.Any((AuthsImpl f) => f.menucode == "barSub_PreviewLayout" && f.authoriza == "F");
			_diyAuth = GlobalData.ListAccessForm.Any((AuthsImpl f) => f.menucode == "frm_UserDefinedProcedure" && f.authoriza == "F");
			barFormSetting.Enabled = GlobalData.ListAccessForm.Any((AuthsImpl f) => f.menucode == "frm_FormSetting" && f.authoriza != "N");
			SetAutoDataSearchMenu();
			treeMenu.DataSource = GlobalData.DtAccessForm;
			treeMenu.RefreshDataSource();
		}
		catch (Exception ex)
		{
			_previewLayoutAuth = false;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		treeMenu.HideLoadingPanel();
	}

	private void ReloadFatherMenu()
	{
		if (!GlobalData.DtAccessForm.IsNotNull())
		{
			return;
		}
		List<DataRow> list = (from t in GlobalData.DtAccessForm.AsEnumerable()
			where Function.ToString(t["MenuType"]).Equals("N") || Function.ToString(t["MenuType"]).Equals("A")
			select t).ToList();
		List<DataRow> list2 = list.Where((DataRow t) => GlobalData.DtAccessForm.AsEnumerable().All((DataRow x) => Function.ParseInt(x["FatherId"]) != Function.ParseInt(t["Id"]))).ToList();
		while (list2.Any())
		{
			for (int num = 0; num < list2.Count; num++)
			{
				list.Remove(list2[num]);
				GlobalData.DtAccessForm.Rows.Remove(list2[num]);
			}
			list2 = list.Where((DataRow t) => GlobalData.DtAccessForm.AsEnumerable().All((DataRow x) => Function.ParseInt(x["FatherId"]) != Function.ParseInt(t["Id"]))).ToList();
		}
		GlobalData.DtAccessForm.AcceptChanges();
	}

	private void ReloadSideMenu(List<DataRow> lstLanguage)
	{
		if (GlobalData.DtAccessForm != null)
		{
			foreach (DataRow row in GlobalData.DtAccessForm.Rows)
			{
				string text = Function.ToString(row["MenuType"]);
				if (!(text != "F") || !(text != "N") || !(text != "L"))
				{
					DataRow dataRow = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(Function.ToString(row["LanguageKey"])));
					if (dataRow != null)
					{
						row["Name"] = Function.ToString(dataRow["ColumnName"]).Replace(" && ", " & ");
					}
				}
			}
			GlobalData.DtAccessForm.AcceptChanges();
		}
		if (GlobalData.ListAccessForm != null)
		{
			foreach (AuthsImpl form in GlobalData.ListAccessForm)
			{
				DataRow dataRow2 = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(form.LanguageKey));
				if (dataRow2 != null)
				{
					form.menuName = Function.ToString(dataRow2["ColumnName"]).Replace(" && ", " & ");
				}
			}
		}
		SetAutoDataSearchMenu();
	}

	private void HideMainBarSubItem(BarSubItem items)
	{
		try
		{
			foreach (object itemLink in items.ItemLinks)
			{
				if (itemLink.GetType() == typeof(BarSubItemLink))
				{
					if (!((BarSubItemLink)itemLink).Caption.Equals(barSub_Language.Caption) && !((BarSubItemLink)itemLink).Caption.Equals(barSub_Configuration.Caption) && !(((BarSubItemLink)itemLink).Caption == barSub_Integrated.Caption))
					{
						if (HideMainBarSubItemChild((BarSubItemLink)itemLink) == 0)
						{
							((BarSubItemLink)itemLink).Visible = false;
						}
						else
						{
							((BarSubItemLink)itemLink).Visible = true;
						}
					}
					continue;
				}
				BarItemLink barItemLink = (BarItemLink)itemLink;
				if (barItemLink.Item.Name.Equals(barSub_Configuration.Name) || barItemLink.Item.Name.Equals(barSub_Language.Name) || (barItemLink.Item.Name.Length <= 5 && !barItemLink.Item.Name.Contains("@")))
				{
					continue;
				}
				string frm = (barItemLink.Item.Name.Contains("@") ? barItemLink.Item.Name : barItemLink.Item.Name.Remove(0, 5));
				if (CommonList.HiddenFunction.ContainsKey(frm) && CommonList.HiddenFunction[frm])
				{
					barItemLink.Visible = false;
					continue;
				}
				if (!barItemLink.Item.Name.StartsWith("barSub") && (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode.Trim() == frm.Trim()) || GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.authoriza.Trim() == "N" && x.menucode.Trim() == frm.Trim())))
				{
					barItemLink.Visible = false;
				}
				else
				{
					barItemLink.Visible = true;
				}
				if (GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser) && (frm == "frm_Users" || frm == "frm_LicenseManager" || frm == "frm_Authorization" || frm == "frm_SetupAdmin"))
				{
					barItemLink.Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int HideMainBarSubItemChild(BarSubItemLink items)
	{
		int num = 0;
		try
		{
			foreach (object itemLink in items.Item.ItemLinks)
			{
				if (itemLink.GetType() == typeof(BarSubItemLink))
				{
					if (HideMainBarSubItemChild((BarSubItemLink)itemLink) == 0 && ((BarSubItemLink)itemLink).Caption != "Integrated")
					{
						((BarSubItemLink)itemLink).Visible = false;
						continue;
					}
					((BarSubItemLink)itemLink).Visible = true;
					num++;
					continue;
				}
				BarItemLink barItemLink = (BarItemLink)itemLink;
				if (barItemLink.Item.Name.Length <= 5 && !barItemLink.Item.Name.Contains("@"))
				{
					continue;
				}
				string frm = (barItemLink.Item.Name.Contains("@") ? barItemLink.Item.Name : barItemLink.Item.Name.Remove(0, 5));
				if (CommonList.HiddenFunction.ContainsKey(frm) && CommonList.HiddenFunction[frm])
				{
					barItemLink.Visible = false;
					continue;
				}
				if ((!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode.Trim() == frm.Trim()) || GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.authoriza.Trim() == "N" && x.menucode.Trim() == frm.Trim())) && !barItemLink.Item.Name.StartsWith("barSub"))
				{
					barItemLink.Visible = false;
				}
				else
				{
					barItemLink.Visible = true;
					num++;
				}
				if (GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser) && (frm == "frm_Users" || frm == "frm_LicenseManager" || frm == "frm_Authorization" || frm == "frm_SetupAdmin"))
				{
					barItemLink.Visible = true;
					num++;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return num;
	}

	public static async Task LoadConfigFileSharingUpload()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "GeneralSetting");
			DataTable dataTable = await hp.GetAsync("GetFileSharingUploadPath");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				CopyFileSharingCfg.WordPath = ((dataTable.Rows[0]["WordPath"] == DBNull.Value || dataTable.Rows[0]["WordPath"] == null) ? "" : dataTable.Rows[0]["WordPath"].ToString());
				CopyFileSharingCfg.ExcelPath = ((dataTable.Rows[0]["ExcelPath"] == DBNull.Value || dataTable.Rows[0]["ExcelPath"] == null) ? "" : dataTable.Rows[0]["ExcelPath"].ToString());
				CopyFileSharingCfg.BitmapPath = ((dataTable.Rows[0]["BitmapPath"] == DBNull.Value || dataTable.Rows[0]["BitmapPath"] == null) ? "" : dataTable.Rows[0]["BitmapPath"].ToString());
				CopyFileSharingCfg.AttachPath = ((dataTable.Rows[0]["AttachPath"] == DBNull.Value || dataTable.Rows[0]["AttachPath"] == null) ? "" : dataTable.Rows[0]["AttachPath"].ToString());
				CopyFileSharingCfg.XmlPath = ((dataTable.Rows[0]["XmlPath"] == DBNull.Value || dataTable.Rows[0]["XmlPath"] == null) ? "" : dataTable.Rows[0]["XmlPath"].ToString());
				CopyFileShareStruct configData = CopyFileShareHelper.GetConfigData();
				CopyFileSharingCfg.Domain = configData.Domain;
				CopyFileSharingCfg.UserName = configData.User;
				CopyFileSharingCfg.Password = configData.Pass;
				CopyFileSharingCfg.DisableSplitFolder = Function.ToString(dataTable.Rows[0]["DisableSplitFolder"]) == "Y";
			}
			DataTable dataTable2 = await hp.GetAsync("GetObjectTypePath");
			if (!dataTable2.HasColumn())
			{
				return;
			}
			CopyFileSharingCfg.ObjectPath.Clear();
			foreach (DataRow item in dataTable2.AsEnumerable())
			{
				int key = Function.ParseInt(item["ObjType"]);
				string value = Function.ToString(item["Path"]);
				if (!CopyFileSharingCfg.ObjectPath.ContainsKey(key))
				{
					CopyFileSharingCfg.ObjectPath.Add(key, value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetUserLogin(string madk)
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrEmpty(madk))
			{
				GlobalData.SuperUser = await HttpClient.Commons.GetSystemSuperUser();
			}
			if (madk.ToLower().Equals(GlobalData.SuperUser))
			{
				GlobalData.UserCodeLogin = GlobalData.SuperUser;
				GlobalData.UserNameLogin = GlobalData.SuperUser;
				return;
			}
			List<UsersImpl> list = new List<UsersImpl>
			{
				new UsersImpl()
			};
			if (!string.IsNullOrEmpty(madk))
			{
				using HttpClientHelper<UsersImpl> helper = new HttpClientHelper<UsersImpl>(Apzon.HttpClientHelper.Configuration.ApiServer, "Users");
				list = await helper.GetListAsyncByPr("GetUserLogin", new { madk });
			}
			if (list == null || list.Count == 0)
			{
				list = new List<UsersImpl>
				{
					new UsersImpl()
				};
			}
			UsersImpl usersImpl = list[0];
			GlobalData.UserCodeLogin = usersImpl.USER_CODE;
			GlobalData.UserNameLogin = usersImpl.U_NAME;
			GlobalData.BranchLogin = Function.ToString(usersImpl.Branch);
			GlobalData.DeparmentLogin = Function.ToString(usersImpl.Department);
			GlobalData.UserSignLogin = Function.ToString(usersImpl.userSign);
			if (!string.IsNullOrEmpty(usersImpl.WallPaper) && FileIO.FileExist(usersImpl.WallPaper) && !string.IsNullOrEmpty(usersImpl.WllPprDsp))
			{
				BackgroundImage = Image.FromFile(usersImpl.WallPaper);
				switch (usersImpl.WllPprDsp)
				{
				case "1":
					BackgroundImageLayout = ImageLayout.Center;
					break;
				case "2":
					BackgroundImageLayout = ImageLayout.Stretch;
					break;
				case "3":
					BackgroundImageLayout = ImageLayout.Tile;
					break;
				default:
					BackgroundImageLayout = ImageLayout.Center;
					break;
				}
			}
			else
			{
				BackgroundImage = null;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetFormatString()
	{
		try
		{
			await _httpClient.GeneralSetting.GetFormatString();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCurrency()
	{
		try
		{
			using HttpClientHelper<CurrencyCodesImpl> helper = new HttpClientHelper<CurrencyCodesImpl>(Apzon.HttpClientHelper.Configuration.ApiServer, "Manufacturers");
			DataTable dataTable = await helper.GetDataTableAsync("ShowCurrency");
			if (GlobalData.Currencies != null)
			{
				GlobalData.Currencies.Clear();
				GlobalData.CurrencyCodes.Clear();
			}
			else
			{
				GlobalData.Currencies = new List<Common.Table_Code>();
				GlobalData.CurrencyCodes = new List<CurrencyCodesImpl>();
			}
			foreach (DataRow row in dataTable.Rows)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = Function.ToString(row["CurrCode"]);
				table_Code.Name = Function.ToString(row["CurrName"]);
				GlobalData.Currencies.Add(table_Code);
				CurrencyCodesImpl item = new CurrencyCodesImpl
				{
					CurrCode = Function.ToString(row["CurrCode"]),
					CurrName = Function.ToString(row["CurrName"]),
					Decimals = Function.ParseInt(row["Decimals"]),
					RoundSys = Function.ParseInt(row["RoundSys"])
				};
				GlobalData.CurrencyCodes.Add(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadWeightUnits()
	{
		try
		{
			using HttpClientHelper<List<WeightImpl>> helper = new HttpClientHelper<List<WeightImpl>>(Apzon.HttpClientHelper.Configuration.ApiServer, "Weight");
			GlobalData.WeightUnits = await helper.GetAsync("GetData");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadLengthAndWidthUnits()
	{
		try
		{
			using HttpClientHelper<List<LengthAndWidthImpl>> helper = new HttpClientHelper<List<LengthAndWidthImpl>>(Apzon.HttpClientHelper.Configuration.ApiServer, "LengthAndWidth");
			GlobalData.LengthWidthUnits = await helper.GetAsync("GetData");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EventStaticStatusBar()
	{
		TimerStaticStatusBar.Interval = 100;
		TimerStaticStatusBar.Enabled = true;
		TimerStaticStatusBar.Start();
	}

	private void TimerStaticStatusBar_Tick_1(object sender, EventArgs e)
	{
		if ((DateTime.Now - LastActiveTime).TotalMinutes > (double)GlobalData.ScreenLock && Program.IsLoginOk)
		{
			Logging.Write("Client logout", "Process timeout");
			CheckOnline(isNotifi: false);
		}
		if (!MainMessage.MsgEnable)
		{
			return;
		}
		if (MainMessage.MsgCount == 0)
		{
			txt_MsgBar.Caption = ((!string.IsNullOrEmpty(MainMessage.MsgMessage) && MainMessage.MsgMessage.Length > 254) ? (MainMessage.MsgMessage.Substring(0, 254) + "...") : MainMessage.MsgMessage);
			if (MainMessage.MsgColor == Enums.MessageType.Success)
			{
				txt_MsgBar.Appearance.BackColor = Color.DarkGreen;
				txt_MsgBar.Appearance.ForeColor = Color.White;
			}
			else if (MainMessage.MsgColor == Enums.MessageType.Warning)
			{
				txt_MsgBar.Appearance.BackColor = Color.FromArgb(255, 204, 100);
				txt_MsgBar.Appearance.ForeColor = Color.Black;
			}
			else if (MainMessage.MsgColor == Enums.MessageType.Error)
			{
				txt_MsgBar.Appearance.BackColor = Color.DarkRed;
				txt_MsgBar.Appearance.ForeColor = Color.White;
			}
			else if (MainMessage.MsgColor == Enums.MessageType.Info)
			{
				txt_MsgBar.Appearance.BackColor = Color.SteelBlue;
				txt_MsgBar.Appearance.ForeColor = Color.White;
			}
			else if (MainMessage.MsgColor == Enums.MessageType.Transparent)
			{
				txt_MsgBar.Appearance.BackColor = Color.Transparent;
				txt_MsgBar.Appearance.ForeColor = Color.Transparent;
			}
			MainMessage.MsgCount = 1;
			while (!MainMessage.ListMessage.IsEmpty)
			{
				MainMessage.ListMessage.TryTake(out var result);
				GlobalData.SystemMessageLog.Rows.Add(Function.ToString(result.MsgColor), result.Message, result.Time);
				grd_SystemMessageLog.RefreshDataSource();
			}
		}
		else if (MainMessage.MsgCount == MainMessage.MsgCountTimer && !GlobalData.Updating)
		{
			MainMessage.MsgCount = -1;
			MainMessage.MsgMessage = "";
			txt_MsgBar.Caption = MainMessage.MsgMessage;
			MainMessage.MsgEnable = false;
			txt_MsgBar.Appearance.BackColor = backColor;
		}
		else
		{
			MainMessage.MsgCount++;
		}
	}

	private void barButtonModeFirst_ItemClick(object sender, ItemClickEventArgs e)
	{
		First();
	}

	private void barButtonModePrev_ItemClick(object sender, ItemClickEventArgs e)
	{
		Previous();
	}

	private void barButtonLast_ItemClick(object sender, ItemClickEventArgs e)
	{
		Last();
	}

	private void barButtonModeNext_ItemClick(object sender, ItemClickEventArgs e)
	{
		Next();
	}

	private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddNew();
	}

	private void barButtonItemFind_ItemClick(object sender, ItemClickEventArgs e)
	{
		Find();
	}

	private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (!((popupForm == null) ? true : false))
			{
				if (popupForm.frmUDF != null)
				{
					popupForm.frmUDF.Dispose();
					popupForm.frmUDF = null;
				}
				else
				{
					popupForm.OPENFORMUDF();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (!((popupForm == null) ? true : false) && !((popupForm.tableName != null) ? true : false))
			{
				popupForm.tableName();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void barFormSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		_ = 3;
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				if (!GlobalData.ListAccessForm.Any((AuthsImpl f) => f.menucode == "frm_FormSetting" && f.authoriza == "F"))
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReloadMainMenu, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
				{
					barFormSetting.Enabled = false;
					barSub_System.Enabled = false;
					barSub_Module.Enabled = false;
					barSub_Tool.Enabled = false;
					barSub_Window.Enabled = false;
					barSub_Help.Enabled = false;
					treeMenu.DataSource = null;
					treeMenu.ShowLoadingPanel();
					await InsertFormsToAuthorizationTables();
					await LoadListAccessForm();
					await CreateMenuUdt();
					await CreateMenuQuery();
					HideMainBarSubItem(barSub_Module);
					HideMainBarSubItem(barSub_UserDefineWindow);
					SetAutoDataSearchMenu();
					treeMenu.HideLoadingPanel();
					barFormSetting.Enabled = true;
					barSub_System.Enabled = true;
					barSub_Module.Enabled = true;
					barSub_Tool.Enabled = true;
					barSub_Window.Enabled = true;
					barSub_Help.Enabled = true;
					barFormSetting.Enabled = true;
				}
			}
			else
			{
				popupForm.FORMSETTING();
				popupForm.SetControlFormReadOnly();
			}
		}
		catch (Exception)
		{
		}
	}

	private void ReloadAllModule()
	{
		try
		{
			DataTable dt = new DataTable();
			DataColumn column = new DataColumn
			{
				ColumnName = "Id",
				DataType = typeof(int),
				AutoIncrement = true,
				AutoIncrementSeed = 1L,
				AutoIncrementStep = 1L
			};
			dt.Columns.Add(column);
			dt.Columns.Add("FatherId", typeof(int));
			dt.Columns.Add("Code", typeof(string));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("LanguageKey", typeof(string));
			dt.Columns.Add("MenuType", typeof(string));
			GetAllMenu(barSub_Module, -1, "F", ref dt);
			GetAllMenu(barSub_Tool, -1, "L", ref dt);
			if (dt.IsNotNull())
			{
				GlobalData.AllModule = dt;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task InsertFormsToAuthorizationTables()
	{
		_ = 7;
		try
		{
			DataTable dtMenu = new DataTable();
			DataColumn column = new DataColumn
			{
				ColumnName = "Id",
				DataType = typeof(int),
				AutoIncrement = true,
				AutoIncrementSeed = 1L,
				AutoIncrementStep = 1L
			};
			dtMenu.Columns.Add(column);
			dtMenu.Columns.Add("FatherId", typeof(int));
			dtMenu.Columns.Add("Code", typeof(string));
			dtMenu.Columns.Add("Name", typeof(string));
			dtMenu.Columns.Add("LanguageKey", typeof(string));
			dtMenu.Columns.Add("MenuType", typeof(string));
			GetAllMenu(barSub_Module, -1, "F", ref dtMenu);
			GetAllMenu(barSub_Tool, -1, "L", ref dtMenu);
			DataTable dtCus = await _httpClient.MenuStructure.GetListCustomizeMenu();
			if (dtCus == null || dtCus.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Cannot get customize menu info");
				return;
			}
			if (dtCus.Rows.Count != 0)
			{
				foreach (DataRow row2 in dtCus.Rows)
				{
					string fatherMenu = Function.ToString(row2["LanguageKey"]);
					DataRow dataRow2 = (from m in dtMenu.AsEnumerable()
						where m.Field<string>("Code").Equals(fatherMenu) && !string.IsNullOrEmpty(fatherMenu)
						select m).FirstOrDefault();
					if (dataRow2 != null)
					{
						DataRow dataRow3 = dtMenu.NewRow();
						dataRow3["Code"] = row2["Code"];
						dataRow3["Name"] = row2["Name"];
						dataRow3["MenuType"] = row2["MenuType"];
						dataRow3["LanguageKey"] = row2["LanguageKey"];
						dataRow3["FatherId"] = dataRow2["Id"];
						dtMenu.Rows.Add(dataRow3);
					}
				}
			}
			DataTable dt = await GetListUdo();
			if (dt.IsNotNull())
			{
				DataRow[] array = dt.Select("MenuItem = 'Y'");
				if (array.Length != 0)
				{
					dt = array.CopyToDataTable();
				}
				if (dt.IsNotNull())
				{
					DataRow dataRow4 = dtMenu.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]).Equals(barSub_UDO.Name));
					int num;
					if (dataRow4 == null)
					{
						DataRow dataRow5 = dtMenu.NewRow();
						dataRow5["Code"] = barSub_UDO.Name;
						dataRow5["Name"] = barSub_UDO.Caption;
						dataRow5["MenuType"] = "N";
						dataRow5["LanguageKey"] = "UDO";
						dataRow5["FatherId"] = -1;
						dtMenu.Rows.Add(dataRow5);
						num = Function.ParseInt(dataRow5["Id"]);
					}
					else
					{
						num = Function.ParseInt(dataRow4["Id"]);
					}
					int i;
					for (i = 0; i < dt.Rows.Count; i++)
					{
						if (!dtCus.AsEnumerable().Any((DataRow m) => m.Field<string>("Code").Equals("UDO_" + dt.Rows[i]["Code"])))
						{
							DataRow dataRow6 = dtMenu.NewRow();
							dataRow6["Code"] = "UDO_" + dt.Rows[i]["Code"];
							dataRow6["MenuType"] = "O";
							dataRow6["Name"] = Function.ToString(dt.Rows[i]["MenuCapt"]);
							dataRow6["FatherId"] = num;
							dataRow6["LanguageKey"] = "";
							dtMenu.Rows.Add(dataRow6);
						}
					}
				}
			}
			DataTable dtUdt = await GetListUdt("", "");
			if (dtUdt.IsNotNull())
			{
				DataRow dataRow7 = dtMenu.NewRow();
				dataRow7["Code"] = barSub_UserDefineWindow.Name;
				dataRow7["Name"] = barSub_UserDefineWindow.Caption;
				dataRow7["MenuType"] = "N";
				dataRow7["LanguageKey"] = "UserDefineWindow";
				dataRow7["FatherId"] = -1;
				dtMenu.Rows.Add(dataRow7);
				int num2 = Function.ParseInt(dataRow7["Id"]);
				int i2;
				for (i2 = 0; i2 < dtUdt.Rows.Count; i2++)
				{
					if (!dtCus.AsEnumerable().Any((DataRow m) => m.Field<string>("Code").Equals("UDT_" + dtUdt.Rows[i2]["TableName"])))
					{
						string text = Function.ToString(dtUdt.Rows[i2]["TableName"]);
						string arg = Function.ToString(dtUdt.Rows[i2]["Descr"]);
						DataRow dataRow8 = dtMenu.NewRow();
						dataRow8["Code"] = "UDT_" + text;
						dataRow8["MenuType"] = "T";
						dataRow8["LanguageKey"] = "";
						dataRow8["Name"] = $"{text} - {arg}";
						dataRow8["FatherId"] = num2;
						dtMenu.Rows.Add(dataRow8);
					}
				}
			}
			DataTable dataTable = await GetListUserReport();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow9 = dtMenu.NewRow();
				dataRow9["Code"] = "barSub_UserReport";
				dataRow9["Name"] = "Report";
				dataRow9["FatherId"] = -1;
				dataRow9["LanguageKey"] = "UserReport";
				dataRow9["MenuType"] = "N";
				dtMenu.Rows.Add(dataRow9);
				int num3 = Function.ParseInt(dataRow9["Id"]);
				DataRow[] array2 = dataTable.Select("TypeName = 'User Reports' and TypeCode = 'RCRI' and StoreName is null");
				DataRow[] array3;
				if (array2.IsNotNull())
				{
					DataRow dataRow10 = dtMenu.NewRow();
					dataRow10["Code"] = "barSub_CrystalReport";
					dataRow10["Name"] = "Crystal Report";
					dataRow10["FatherId"] = num3;
					dataRow10["LanguageKey"] = "CrystalReport";
					dataRow10["MenuType"] = "N";
					dtMenu.Rows.Add(dataRow10);
					int num4 = Function.ParseInt(dataRow10["Id"]);
					array3 = array2;
					foreach (DataRow dataRow11 in array3)
					{
						DataRow dataRow12 = dtMenu.NewRow();
						dataRow12["Code"] = "frm_" + Function.ToString(dataRow11["DocCode"]);
						dataRow12["Name"] = Function.ToString(dataRow11["DocName"]);
						dataRow12["FatherId"] = num4;
						dataRow12["LanguageKey"] = "";
						dataRow12["MenuType"] = "R";
						dtMenu.Rows.Add(dataRow12);
					}
				}
				array2 = dataTable.Select("TypeName = 'User Reports' and TypeCode = 'RCRI' and StoreName <> ''");
				array3 = array2;
				foreach (DataRow row in array3)
				{
					int num6 = (from r in dtMenu.AsEnumerable()
						where Function.ToString(r["Code"]) == Function.ToString(row["StoreName"])
						select Function.ParseInt(r["Id"])).FirstOrDefault();
					if (num6 != 0)
					{
						DataRow dataRow13 = dtMenu.NewRow();
						dataRow13["Code"] = "frm_" + Function.ToString(row["DocCode"]);
						dataRow13["Name"] = Function.ToString(row["DocName"]);
						dataRow13["FatherId"] = num6;
						dataRow13["LanguageKey"] = "";
						dataRow13["MenuType"] = "R";
						dtMenu.Rows.Add(dataRow13);
					}
				}
				DataRow[] array4 = dataTable.Select("TypeName = 'Form Reports'");
				if (array4.IsNotNull())
				{
					DataRow dataRow14 = dtMenu.NewRow();
					dataRow14["Code"] = "barSub_FormReport";
					dataRow14["Name"] = "Form Report";
					dataRow14["FatherId"] = num3;
					dataRow14["LanguageKey"] = "FormReport";
					dataRow14["MenuType"] = "N";
					dtMenu.Rows.Add(dataRow14);
					int num7 = Function.ParseInt(dataRow14["Id"]);
					array3 = array4;
					foreach (DataRow dataRow15 in array3)
					{
						int num8 = Function.ParseInt(dataRow15["FatherId"]);
						DataRow dataRow16 = dtMenu.NewRow();
						dataRow16["Code"] = "frm_" + Function.ToString(dataRow15["DocCode"]);
						dataRow16["Name"] = Function.ToString(dataRow15["DocName"]);
						dataRow16["FatherId"] = ((num8 == 0) ? num7 : num8);
						dataRow16["LanguageKey"] = "";
						dataRow16["MenuType"] = "C";
						dtMenu.Rows.Add(dataRow16);
					}
				}
				DataRow[] array5 = dataTable.Select("TypeName = 'XtraReports'");
				if (array5.IsNotNull())
				{
					DataRow dataRow17 = dtMenu.NewRow();
					dataRow17["Code"] = "barSub_XtraReport";
					dataRow17["Name"] = "XtraReports";
					dataRow17["FatherId"] = num3;
					dataRow17["LanguageKey"] = "XtraReport";
					dataRow17["MenuType"] = "N";
					dtMenu.Rows.Add(dataRow17);
					int num9 = Function.ParseInt(dataRow17["Id"]);
					array3 = array5;
					foreach (DataRow dataRow18 in array3)
					{
						int num10 = Function.ParseInt(dataRow18["FatherId"]);
						DataRow dataRow19 = dtMenu.NewRow();
						dataRow19["Code"] = "frm_" + Function.ToString(dataRow18["DocName"]);
						dataRow19["Name"] = Function.ToString(dataRow18["DocName"]);
						dataRow19["FatherId"] = ((num10 == 0) ? num9 : num10);
						dataRow19["LanguageKey"] = "";
						dataRow19["MenuType"] = "X";
						dtMenu.Rows.Add(dataRow19);
					}
				}
			}
			DataTable dataTable2 = await _httpClient.DashboardClient.GetListDashboard();
			if (dataTable2.IsNotNull())
			{
				DataRow dataRow20 = dtMenu.NewRow();
				dataRow20["Code"] = "barSub_Analytics";
				dataRow20["Name"] = "Analytics";
				dataRow20["FatherId"] = -1;
				dataRow20["LanguageKey"] = "Analytics";
				dataRow20["MenuType"] = "L";
				dtMenu.Rows.Add(dataRow20);
				int num11 = Function.ParseInt(dataRow20["Id"]);
				DataRow dataRow21 = dtMenu.NewRow();
				dataRow21["Code"] = "barSub_DashboardAuth";
				dataRow21["Name"] = "Dashboards";
				dataRow21["FatherId"] = num11;
				dataRow21["LanguageKey"] = "Dashboards";
				dataRow21["MenuType"] = "L";
				dtMenu.Rows.Add(dataRow21);
				num11 = Function.ParseInt(dataRow21["Id"]);
				foreach (DataRow row3 in dataTable2.Rows)
				{
					DataRow dataRow23 = dtMenu.NewRow();
					dataRow23["Code"] = "barSub_Dashboard" + row3["Id"];
					dataRow23["Name"] = row3["Name"];
					dataRow23["MenuType"] = "L";
					dataRow23["FatherId"] = num11;
					dtMenu.Rows.Add(dataRow23);
				}
			}
			if (barSub_System.ItemLinks.Count > 0)
			{
				DataRow dataRow24 = dtMenu.NewRow();
				dataRow24["Code"] = barSub_System.Name;
				dataRow24["Name"] = barSub_System.Caption;
				dataRow24["MenuType"] = "N";
				dataRow24["LanguageKey"] = "System";
				dataRow24["FatherId"] = -1;
				dtMenu.Rows.Add(dataRow24);
				DataRow dataRow25 = dtMenu.NewRow();
				dataRow25["Code"] = barSub_PreviewLayout.Name;
				dataRow25["Name"] = barSub_PreviewLayout.Caption;
				dataRow25["MenuType"] = "L";
				dataRow25["FatherId"] = dataRow24["Id"];
				dataRow25["LanguageKey"] = "System";
				dtMenu.Rows.Add(dataRow25);
			}
			DataRow dataRow26 = dtMenu.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]) == barSub_CustomizationTools.Name);
			if (!dataRow26.IsNull("Id"))
			{
				DataRow dataRow27 = dtMenu.NewRow();
				dataRow27["Code"] = "barSub_ContextAction";
				dataRow27["Name"] = "Context Action";
				dataRow27["MenuType"] = "L";
				dataRow27["LanguageKey"] = "ContextAction";
				dataRow27["FatherId"] = Function.ParseInt(dataRow26["Id"]);
				dtMenu.Rows.Add(dataRow27);
			}
			DataRow dataRow28 = dtMenu.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]) == "barSub_SystemInitialization");
			DataRow dataRow29 = dtMenu.NewRow();
			dataRow29["Code"] = "barSub_NumberingSeries";
			dataRow29["Name"] = "Numbering Series";
			dataRow29["MenuType"] = "N";
			dataRow29["FatherId"] = dataRow28["Id"];
			dtMenu.Rows.Add(dataRow29);
			for (int num12 = 1; num12 <= 10; num12++)
			{
				DataRow dataRow30 = dtMenu.NewRow();
				dataRow30["Code"] = "barSub_SeriesGroup" + num12;
				dataRow30["Name"] = "Series - Group No. " + num12;
				dataRow30["MenuType"] = "L";
				dataRow30["FatherId"] = dataRow29["Id"];
				dtMenu.Rows.Add(dataRow30);
			}
			DataTable dataTable3 = await _httpClient.Queries.GetAuthGroup();
			DataRow dataRow31 = dtMenu.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]) == "barSub_Queries");
			if (dataRow31 != null)
			{
				DataRow dataRow32 = dtMenu.NewRow();
				dataRow32["Code"] = "barSub_QueryAuthGroup";
				dataRow32["Name"] = "Query Authorization Groups";
				dataRow32["MenuType"] = "N";
				dataRow32["FatherId"] = dataRow31["Id"];
				dtMenu.Rows.Add(dataRow32);
				for (int num13 = 0; num13 < dataTable3.Rows.Count; num13++)
				{
					DataRow dataRow33 = dtMenu.NewRow();
					dataRow33["Code"] = "barSub_QueryAuthGrp" + dataTable3.Rows[num13]["AUTHGRPID"];
					dataRow33["Name"] = dataTable3.Rows[num13]["AUTHGRPN"];
					dataRow33["MenuType"] = "L";
					dataRow33["FatherId"] = dataRow32["Id"];
					dtMenu.Rows.Add(dataRow33);
				}
			}
			DataRow dataRow34 = dtMenu.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]) == "barSub_Administration");
			DataRow dataRow35 = dtMenu.NewRow();
			dataRow35["Code"] = "barSub_CancelDocGenCancelation";
			dataRow35["Name"] = "Cancel Marketing Documents by Generating Cancelation Documents";
			dataRow35["MenuType"] = "L";
			dataRow35["FatherId"] = dataRow34["Id"];
			dtMenu.Rows.Add(dataRow35);
			dataRow35 = dtMenu.NewRow();
			dataRow35["Code"] = "barSub_CancelOtherDoc";
			dataRow35["Name"] = "Cancel Other Documents";
			dataRow35["MenuType"] = "L";
			dataRow35["FatherId"] = dataRow34["Id"];
			dtMenu.Rows.Add(dataRow35);
			dataRow35 = dtMenu.NewRow();
			dataRow35["Code"] = "barSub_CloseDocument";
			dataRow35["Name"] = "Close Document";
			dataRow35["MenuType"] = "L";
			dataRow35["FatherId"] = dataRow34["Id"];
			dtMenu.Rows.Add(dataRow35);
			dtMenu.AcceptChanges();
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Auth");
			string text2 = await hp.PostAsJsonAsync("InsertFormsToAuthTbl", dtMenu);
			if (string.IsNullOrWhiteSpace(text2))
			{
				WinCommons.Success(Enums.MessageCode.ReloadAuthorizationModuleSuccess, "");
				GlobalData.FormWizard = await _httpClient.FormWizard.GetValid();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, Resources.Msg_SaveError + ":" + text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Main_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (e.Cancel)
			{
				return;
			}
			if (!Exit)
			{
				if (!IsImportLicense)
				{
					Program.IsLoginOk = false;
					_httpClient.LoginClient.LogOut(GlobalData.SessionId);
					DisConnectSocket();
				}
				else
				{
					e.Cancel = true;
					if (!Program.IsLoginOk)
					{
						IsImportLicense = false;
						_relogin = false;
						try
						{
							_activeForm = ApzonCommon.GetActiveForm();
							using frm_LoginSap frm_LoginSap2 = new frm_LoginSap();
							if (MdiChildren.Any((Form f) => f is XtraFormEx xtraFormEx && (xtraFormEx.FormMode == Enums.FormViewModeType.Add || xtraFormEx.FormMode == Enums.FormViewModeType.Update)))
							{
								frm_LoginSap2.HasActiveForm = true;
							}
							frm_LoginSap2.ShowDialog(this);
							if (frm_LoginSap2.DialogResult != DialogResult.Cancel)
							{
								VisibleAndInvisubleMenubar(isVisible: true, frm_LoginSap2.ChangeUser);
								return;
							}
							_httpClient.LoginClient.LogOut(GlobalData.SessionId);
							IsRestart = false;
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
						}
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		if (IsRestart)
		{
			IsRestart = false;
		}
	}

	public void navChangeUser_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		_activeForm = ApzonCommon.GetActiveForm();
		using frm_LoginSap frm_LoginSap2 = new frm_LoginSap(changeUser: true);
		if (MdiChildren.Any((Form f) => f is XtraFormEx xtraFormEx && (xtraFormEx.FormMode == Enums.FormViewModeType.Add || xtraFormEx.FormMode == Enums.FormViewModeType.Update)))
		{
			frm_LoginSap2.HasActiveForm = true;
		}
		frm_LoginSap2.ChangeUser = true;
		frm_LoginSap2.ShowDialog();
		if (!Program.IsLoginOk)
		{
			Close();
		}
		else if (frm_LoginSap2.DialogResult == DialogResult.OK)
		{
			VisibleAndInvisubleMenubar(isVisible: true, frm_LoginSap2.ChangeUser);
		}
	}

	private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
	{
		Form[] mdiChildren = MdiChildren;
		for (int i = 0; i < mdiChildren.Length; i++)
		{
			mdiChildren[i].Close();
		}
		F_ThongSoKetNoi f_ThongSoKetNoi = new F_ThongSoKetNoi();
		f_ThongSoKetNoi.MdiParent = this;
		f_ThongSoKetNoi.Show();
	}

	private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
	{
		Form[] mdiChildren = MdiChildren;
		for (int i = 0; i < mdiChildren.Length; i++)
		{
			mdiChildren[i].Close();
		}
	}

	private void navBarItem_DocumentDraftsReport_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_DocumentDraftsRP frm_DocumentDraftsRP2 = new frm_DocumentDraftsRP();
			frm_DocumentDraftsRP2.MdiParent = this;
			frm_DocumentDraftsRP2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void barButtonItem_CheckForUpdate_ItemClick(object sender, ItemClickEventArgs e)
	{
		await CheckForUpdate();
	}

	private void barButtonItem_UpdateReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			WinFuntions.UpdateReportFile();
			WinFuntions.UpdateReportFile("Action");
			WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task CheckForUpdate()
	{
		try
		{
			await WinFuntions.CheckForUpdateClientHttp(GlobalData.DatabaseName, isOpenAplication: false);
		}
		catch (WebException ex)
		{
			WinCommons._CloseProcessing();
			string statusDescription = ((FtpWebResponse)ex.Response).StatusDescription;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, statusDescription);
		}
	}

	private void timerLogin_Tick(object sender, EventArgs e)
	{
		try
		{
			if (frm_LoginSap.Loaded)
			{
				timerLogin.Enabled = false;
			}
			FormCollection openForms = Application.OpenForms;
			for (int i = 0; i < openForms.Count; i++)
			{
				if (openForms[i] != null)
				{
					Form form = openForms[i];
					if (form.Name == "LoginSap")
					{
						form.Focus();
						timerLogin.Enabled = false;
					}
					else if (form.Name == "frm_LogIn")
					{
						form.Focus();
						timerLogin.Enabled = false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadModulesSettingLanguage()
	{
		try
		{
			DataTable dataTable = await HttpClient.FormSettingClient.GetDataSettingLanguage(GlobalData.CURRENTLANGUAGE);
			if (dataTable.IsNotNull())
			{
				barSub_Language.ItemLinks.Clear();
				barSub_Language.ClearLinks();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					BarCheckItem barCheckItem = new BarCheckItem();
					barCheckItem.Caption = Function.ToString(dataTable.Rows[i]["LngName"]);
					barCheckItem.Name = Function.ToString(dataTable.Rows[i]["LngCode"]);
					barCheckItem.Checked = barCheckItem.Name.Equals(GlobalData.CURRENTLANGUAGE);
					barCheckItem.CheckedChanged += barButtonItem_Language_CheckedChanged;
					barSub_Language.AddItem(barCheckItem);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void barButtonItem_Language_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		_ = 1;
		try
		{
			string name = e.Item.Name;
			Resources.Culture = CultureInfo.CreateSpecificCulture(name);
			GlobalData.CURRENTLANGUAGE = name;
			ResourcesHelper.SaveCurrentLanguage();
			await SettingLanguage();
			await LoadModulesSettingLanguage();
			LoadMessageDataCenter();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void barButtonItem_LanguageE_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Resources.Culture = CultureInfo.CreateSpecificCulture("en-US");
			GlobalData.CURRENTLANGUAGE = "en-US";
			ResourcesHelper.SaveCurrentLanguage();
			await SettingLanguage();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void barButtonItem_LanguageVN_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Resources.Culture = CultureInfo.CreateSpecificCulture("vi-VN");
			GlobalData.CURRENTLANGUAGE = "vi-VN";
			ResourcesHelper.SaveCurrentLanguage();
			await SettingLanguage();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadMessageDataCenter()
	{
		try
		{
			DataTable dataTable = await HttpClient.FormSettingClient.GetMessageDataCenter(GlobalData.CURRENTLANGUAGE);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Enums.MessageCode key = (Enums.MessageCode)Function.ParseInt(dataTable.Rows[i]["MsgCode"]);
				if (GlobalData.MessageDataCenter.ContainsKey(key))
				{
					GlobalData.MessageDataCenter[key] = Function.ToString(dataTable.Rows[i]["MsgName"]);
				}
				else
				{
					GlobalData.MessageDataCenter.Add(key, Function.ToString(dataTable.Rows[i]["MsgName"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task SettingLanguage()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await HttpClient.FormSettingClient.GetFormSetting("oMain", "oMain", "-", GlobalData.CURRENTLANGUAGE);
			if (!dataTable.IsNotNull())
			{
				Logging.Write("ERROR", "Cannot load language: " + GlobalData.CURRENTLANGUAGE);
				return;
			}
			List<DataRow> lstLanguage = dataTable.AsEnumerable().ToList();
			await Task.WhenAll(SettingLangeuageMenu(barSub_System, lstLanguage), SettingLangeuageMenu(barSub_Help, lstLanguage), SettingLangeuageMenu(barSub_Module, lstLanguage), SettingLangeuageMenu(barSub_Window, lstLanguage), SettingLangeuageMenu(barSub_Tool, lstLanguage));
			ReloadSideMenu(lstLanguage);
			int i;
			for (i = 0; i < ToolBar.ItemLinks.Count; i++)
			{
				DataRow dataRow = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(ToolBar.ItemLinks[i].Item.Tag));
				if (dataRow != null)
				{
					SuperToolTip superToolTip = new SuperToolTip();
					SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
					superToolTipSetupArgs.Title.Font = new Font("Tahoma", Function.ParseFloat(8.25), FontStyle.Bold);
					superToolTipSetupArgs.ShowFooterSeparator = true;
					superToolTipSetupArgs.Contents.Text += Function.ToString(dataRow["ColumnName"]);
					superToolTip.Setup(superToolTipSetupArgs);
					superToolTip.AllowHtmlText = DefaultBoolean.True;
					ToolBar.ItemLinks[i].Item.SuperTip = superToolTip;
				}
			}
			DataRow dataRow2 = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(txt_MenuSearch.Tag));
			if (dataRow2 != null)
			{
				txt_MenuSearch.ToolTip = Function.ToString(dataRow2["ColumnName"]);
				txt_MenuSearch.Properties.NullText = txt_MenuSearch.ToolTip;
			}
			DataRow dataRow3 = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(npage_Operations.Tag));
			if (dataRow3 != null)
			{
				npage_Operations.Caption = Function.ToString(dataRow3["ColumnName"]);
				MenuBar.Text = Function.ToString(dataRow3["ColumnName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SettingLangeuageMenu(BarSubItem item, List<DataRow> lstLanguage)
	{
		string lngKey = (string.IsNullOrEmpty(Function.ToString(item.Tag)) ? item.Name.Replace("menu_frm_", "").Replace("barSub_", "") : Function.ToString(item.Tag));
		DataRow dataRow = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(lngKey));
		if (dataRow != null)
		{
			item.Caption = Function.ToString(dataRow["ColumnName"]).Replace(" & ", " && ");
		}
		foreach (object itemLink in item.ItemLinks)
		{
			if (itemLink.GetType() == typeof(BarSubItemLink))
			{
				if (!(itemLink is BarSubItemLink barSubItemLink))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(barSubItemLink.Item.Name))
				{
					string barsubLngKey = (string.IsNullOrEmpty(Function.ToString(barSubItemLink.Item.Tag)) ? barSubItemLink.Item.Name.Replace("menu_frm_", "").Replace("barSub_", "") : Function.ToString(barSubItemLink.Item.Tag));
					DataRow dataRow2 = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(barsubLngKey));
					if (dataRow2 != null)
					{
						barSubItemLink.Caption = Function.ToString(dataRow2["ColumnName"]).Replace(" & ", " && ");
					}
					await SettingLangeuageMenu(barSubItemLink.Item as BarSubItem, lstLanguage);
				}
			}
			else
			{
				if (!(itemLink is BarItemLink barItemLink))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(barItemLink.Item.Name))
				{
					string lngKeyItem = (string.IsNullOrEmpty(Function.ToString(barItemLink.Item.Tag)) ? barItemLink.Item.Name.Replace("menu_frm_", "").Replace("barSub_", "") : Function.ToString(barItemLink.Item.Tag));
					DataRow dataRow3 = lstLanguage.FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(lngKeyItem));
					if (dataRow3 != null)
					{
						barItemLink.Caption = Function.ToString(dataRow3["ColumnName"]).Replace(" & ", " && ");
					}
				}
			}
		}
	}

	private void bar_Exit_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmExit, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void bar_PrintLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		PreviewLayout();
	}

	private static void PreviewLayout()
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (!((popupForm == null) ? true : false))
			{
				popupForm.PRINTLAYOUT();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataTable> GetListUdo()
	{
		DataTable dt = new DataTable();
		try
		{
			dt = await _httpClient.Udo.GetListUdo();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	public async Task CreateMenuUdt()
	{
		_ = 1;
		try
		{
			barSub_UserDefineWindow.ItemLinks.Clear();
			barSub_UserDefineWindow.ClearLinks();
			DataTable dtAuthUDT = await GetListUdt(GlobalData.UserName, GlobalData.DefaultBranch.ToString());
			DataTable dataTable = await GetListUdt("", "");
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				string name = Function.ToString(row["TableName"]);
				string des = Function.ToString(row["Descr"]);
				BarButtonItem barButton = new BarButtonItem
				{
					Name = "menu_UDT_" + name,
					Hint = name,
					Caption = $"{name} - {des}".Replace(" & ", " && ")
				};
				barButton.ItemClick += (object s, ItemClickEventArgs e) =>
				{
					frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(name);
					frm_SetUpUDT2.Text = des;
					frm_SetUpUDT2.MdiParent = this;
					frm_SetUpUDT2.Show();
				};
				barSub_UserDefineWindow.AddItem(barButton);
				BarItemLink barItemLink = barSub_UserDefineWindow.ItemLinks.FirstOrDefault((BarItemLink l) => l.Caption == barButton.Caption);
				if (barItemLink != null && (GlobalData.UserLicense == null || GlobalData.UserLicense.UserRule == LicenseRule.Service || !dtAuthUDT.AsEnumerable().Any((DataRow r) => Function.ToString(r["TableName"]) == name)))
				{
					barItemLink.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetListUdt(string userName, string branch)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "UDT");
			dt = await _helper.GetDataTableAsync("GetListUDT", new { userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private void AddSkinsMenu()
	{
		try
		{
			SkinContainerCollection skins = SkinManager.Default.Skins;
			BarSubItem barSubItem = new BarSubItem
			{
				Name = "MenuChangeSkins",
				MenuCaption = "Change Skins",
				Caption = "Change Skins",
				Tag = "ChangeSkins"
			};
			barSub_Configuration.AddItem(barSubItem);
			foreach (SkinContainer item in skins)
			{
				BarCheckItem barCheckItem = new BarCheckItem
				{
					Caption = item.SkinName
				};
				barCheckItem.ItemClick += skin_ItemClick;
				barCheckItem.GroupIndex = 1;
				if (item.SkinName == _skinName)
				{
					barCheckItem.Checked = true;
				}
				barSubItem.AddItem(barCheckItem);
			}
			BarSubItem barSubItem2 = new BarSubItem();
			barSubItem2.Name = "MenuChangeStyles";
			barSubItem2.MenuCaption = "Change Styles";
			barSubItem2.Caption = "Change Styles";
			barSubItem2.Tag = "ChangeStyles";
			barSub_Configuration.AddItem(barSubItem2);
			string[] names = Enum.GetNames(typeof(LookAndFeelStyle));
			foreach (string text in names)
			{
				BarCheckItem barCheckItem2 = new BarCheckItem
				{
					Caption = text
				};
				barCheckItem2.ItemClick += style_ItemClick;
				barCheckItem2.GroupIndex = 1;
				if (text == Enum.GetName(typeof(LookAndFeelStyle), UserLookAndFeel.Default.Style))
				{
					barCheckItem2.Checked = true;
				}
				barSubItem2.AddItem(barCheckItem2);
			}
			BarCheckItem barCheckItem3 = new BarCheckItem
			{
				Caption = "Skin XtraFormEx"
			};
			barCheckItem3.ItemClick += xsitem_ItemClick;
			barCheckItem3.Checked = false;
			barSubItem.AddItem(barCheckItem3).BeginGroup = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void style_ItemClick(object sender, ItemClickEventArgs e)
	{
		LookAndFeelStyle style = (LookAndFeelStyle)Enum.Parse(typeof(LookAndFeelStyle), e.Item.Caption);
		UserLookAndFeel.Default.SetStyle(style, useWindowsXPTheme: false, useDefaultLookAndFeel: false);
		((BarCheckItem)e.Item).Checked = true;
	}

	private void skin_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			UserLookAndFeel.Default.SetSkinStyle(e.Item.Caption);
			((BarCheckItem)e.Item).Checked = true;
			WinFuntions.setConfigSkins(new ConfigSkins
			{
				Caption = e.Item.Caption
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xsitem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (SkinManager.AllowFormSkins)
		{
			SkinManager.DisableFormSkins();
		}
		else
		{
			SkinManager.EnableFormSkins();
		}
		((BarCheckItem)e.Item).Checked = SkinManager.AllowFormSkins;
		LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
	}

	private void LoadConfigSkins()
	{
		try
		{
			string path = WinFuntions._StartupPath + "DevExpress.BonusSkins.v20.1.dll";
			if (File.Exists(path))
			{
				Assembly assembly = Assembly.LoadFile(Path.GetFullPath(path));
				SkinManager.Default.RegisterAssembly(assembly);
				Application.EnableVisualStyles();
			}
			ConfigSkins configSkins = WinFuntions.getConfigSkins();
			if (configSkins != null && !string.IsNullOrEmpty(configSkins.Caption))
			{
				_skinName = configSkins.Caption;
				UserLookAndFeel.Default.SetSkinStyle(configSkins.Caption);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Export(Enums.ExportExt exten)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			if (popupForm.ActiveControl.GetActiveGridControl() is GridControlEx gridControlEx)
			{
				if (gridControlEx.MainView is GridViewEx)
				{
					(gridControlEx.MainView as GridViewEx).Export(exten);
				}
				else if (gridControlEx.MainView is BandedGridViewEx)
				{
					(gridControlEx.MainView as BandedGridViewEx).Export(exten);
				}
			}
			else
			{
				TreeEx activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
				if (activeTreeList != null)
				{
					activeTreeList.Export(exten);
				}
				else
				{
					WinFuntions.GetActivePivotGrid(popupForm.ActiveControl)?.Export(exten);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_Export_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControlEx activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					(activeGridControl.MainView as GridViewEx).Export();
				}
				else if (activeGridControl.MainView is BandedGridViewEx)
				{
					(activeGridControl.MainView as BandedGridViewEx).Export();
				}
			}
			else
			{
				TreeEx activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
				if (activeTreeList != null)
				{
					activeTreeList.Export();
				}
				else
				{
					WinFuntions.GetActivePivotGrid(popupForm.ActiveControl)?.Export();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_ExportExcel_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Xls);
	}

	private void bar_ExportXlsx_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Xlsx);
	}

	private void bar_ExportWord_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Rtf);
	}

	private void bar_ExportPdf_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Pdf);
	}

	private void btn_ExportClipboard_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	private void bar_PreviewGrid_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					((GridViewEx)activeGridControl.MainView)?.ShowRibbonPrintPreview();
				}
				if (activeGridControl.MainView is BandedGridViewEx)
				{
					((BandedGridViewEx)activeGridControl.MainView)?.ShowRibbonPrintPreview();
				}
			}
			else
			{
				WinFuntions.GetActiveTreeList(popupForm.ActiveControl)?.ShowRibbonPrintPreview();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barAutoFilterRow_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					GridViewEx gridViewEx = (GridViewEx)activeGridControl.MainView;
					if (gridViewEx != null)
					{
						if (!gridViewEx.OptionsView.ShowAutoFilterRow)
						{
							gridViewEx.OptionsView.ShowAutoFilterRow = true;
						}
						else
						{
							gridViewEx.OptionsView.ShowAutoFilterRow = false;
						}
					}
				}
				if (!(activeGridControl.MainView is BandedGridViewEx))
				{
					return;
				}
				BandedGridViewEx bandedGridViewEx = (BandedGridViewEx)activeGridControl.MainView;
				if (bandedGridViewEx != null)
				{
					if (!bandedGridViewEx.OptionsView.ShowAutoFilterRow)
					{
						bandedGridViewEx.OptionsView.ShowAutoFilterRow = true;
					}
					else
					{
						bandedGridViewEx.OptionsView.ShowAutoFilterRow = false;
					}
				}
				return;
			}
			TreeList activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
			if (activeTreeList != null)
			{
				if (!activeTreeList.OptionsView.ShowAutoFilterRow)
				{
					activeTreeList.OptionsView.ShowAutoFilterRow = true;
				}
				else
				{
					activeTreeList.OptionsView.ShowAutoFilterRow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_BestFitColumn_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					((GridViewEx)activeGridControl.MainView)?.BestFitColumns();
				}
				if (activeGridControl.MainView is BandedGridViewEx)
				{
					((BandedGridViewEx)activeGridControl.MainView)?.BestFitColumns();
				}
			}
			else
			{
				TreeList activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
				if (activeTreeList != null)
				{
					activeTreeList.BestFitColumns();
				}
				else
				{
					WinFuntions.GetActivePivotGrid(popupForm.ActiveControl)?.BestFit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_BestFitColumn_ItemDoubleClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					GridViewEx gridViewEx = (GridViewEx)activeGridControl.MainView;
					if (gridViewEx != null)
					{
						gridViewEx.OptionsView.ColumnAutoWidth = !gridViewEx.OptionsView.ColumnAutoWidth;
					}
				}
				if (activeGridControl.MainView is BandedGridViewEx)
				{
					BandedGridViewEx bandedGridViewEx = (BandedGridViewEx)activeGridControl.MainView;
					if (bandedGridViewEx != null)
					{
						bandedGridViewEx.OptionsView.ColumnAutoWidth = !bandedGridViewEx.OptionsView.ColumnAutoWidth;
					}
				}
			}
			else
			{
				TreeList activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
				if (activeTreeList != null)
				{
					activeTreeList.OptionsView.AutoWidth = !activeTreeList.OptionsView.AutoWidth;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_Expand_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx)
				{
					GridViewEx gridViewEx = (GridViewEx)activeGridControl.MainView;
					if (gridViewEx != null)
					{
						if (!gridViewEx.GetRowExpanded(gridViewEx.FocusedRowHandle))
						{
							gridViewEx.ExpandAllGroups();
						}
						else
						{
							gridViewEx.CollapseAllGroups();
						}
					}
				}
				if (!(activeGridControl.MainView is BandedGridViewEx))
				{
					return;
				}
				BandedGridViewEx bandedGridViewEx = (BandedGridViewEx)activeGridControl.MainView;
				if (bandedGridViewEx != null)
				{
					if (!bandedGridViewEx.GetRowExpanded(bandedGridViewEx.FocusedRowHandle))
					{
						bandedGridViewEx.ExpandAllGroups();
					}
					else
					{
						bandedGridViewEx.CollapseAllGroups();
					}
				}
				return;
			}
			TreeList activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
			if (activeTreeList != null)
			{
				if (TreeListExpanded(activeTreeList))
				{
					activeTreeList.CollapseAll();
				}
				else
				{
					activeTreeList.ExpandAll();
				}
				return;
			}
			PivotGridControlEx activePivotGrid = WinFuntions.GetActivePivotGrid(popupForm.ActiveControl);
			if (activePivotGrid != null)
			{
				if (PivotExpanded(activePivotGrid))
				{
					activePivotGrid.CollapseAll();
				}
				else
				{
					activePivotGrid.ExpandAll();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bar_DefaultBranch_ItemClick(object sender, ItemClickEventArgs e)
	{
		await LoadDefaultData(clickChooseBranch: true);
	}

	private bool TreeListExpanded(TreeList aTreeList)
	{
		foreach (TreeListNode node in aTreeList.Nodes)
		{
			if (node.Expanded)
			{
				return true;
			}
		}
		return false;
	}

	private bool PivotExpanded(PivotGridControlEx pivot)
	{
		foreach (PivotGridField field in pivot.Fields)
		{
			if (pivot.IsObjectCollapsed(field, 0))
			{
				return false;
			}
		}
		return true;
	}

	private async Task CreateMenuQuery()
	{
		_ = 1;
		try
		{
			string filter = await WinFuntions.ShowFilterSetting("OUQR");
			new DataSet();
			DataSet dataSet = await _httpClient.Queries.GetListQueries(GlobalData.UserName, GlobalData.BranchLogin, filter);
			if (dataSet.IsHasStructure())
			{
				List<BarItemLink> list = new List<BarItemLink>();
				foreach (BarItemLink itemLink in barSub_Queries.ItemLinks)
				{
					if (itemLink.Caption == "User Queries" || itemLink.Caption == "System Queries")
					{
						list.Add(itemLink);
					}
				}
				foreach (BarItemLink item3 in list)
				{
					barSub_Queries.RemoveLink(item3);
				}
				if (GlobalData.UserLicense == null || GlobalData.UserLicense.UserRule == LicenseRule.Service)
				{
					return;
				}
				BarSubItem barSubItem = new BarSubItem();
				barSubItem.Name = "barSub_QueriesSystem";
				barSubItem.MenuCaption = "System Queries";
				barSubItem.Caption = "System Queries";
				barSub_Queries.AddItem(barSubItem);
				BarSubItem barSubItem2 = new BarSubItem();
				barSubItem2.Name = "barSub_QueriesUser";
				barSubItem2.MenuCaption = "User Queries";
				barSubItem2.Caption = "User Queries";
				barSub_Queries.AddItem(barSubItem2);
				DataRow[] array = dataSet.Tables["Data"].Select("QCategory = -2");
				if (array != null && array.Length != 0)
				{
					DataRow[] array2 = array;
					foreach (DataRow item in array2)
					{
						BarButtonItem barButtonItem = new BarButtonItem();
						barButtonItem.Caption = Function.ToString(item["QName"]).Replace(" & ", " && ");
						barButtonItem.Hint = Function.ToString(item["QString"]);
						barButtonItem.Tag = Function.ToString(item["IntrnalKey"]);
						barButtonItem.Description = Function.ToString(item["IntrnalKey"]);
						barButtonItem.ItemClick += (object s, ItemClickEventArgs v) =>
						{
							try
							{
								WinCommons._OpenProcessing();
								BarManager barManager = (BarManager)s;
								string displayHint = barManager.PressedLink.DisplayHint;
								frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
								frm_QueryPreview2.Text = barManager.PressedLink.Caption;
								frm_QueryPreview2.ManagerQ = true;
								frm_QueryPreview2.QueryId = Function.ParseInt(item["IntrnalKey"]);
								frm_QueryPreview2.Sql = displayHint;
								frm_QueryPreview2.MdiParent = this;
								frm_QueryPreview2.Show();
								WinCommons._CloseProcessing();
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.ErrorLineNumber(ex2);
								WinCommons._CloseProcessing();
							}
						};
						barSubItem.AddItem(barButtonItem);
					}
				}
				else
				{
					barSubItem.Visibility = BarItemVisibility.Never;
				}
				DataRow[] array3 = dataSet.Tables["Data"].Select("QCategory > -2");
				if (array3 != null && array3.Length != 0)
				{
					string text = "";
					BarSubItem barSubItem3 = new BarSubItem();
					DataRow[] array2 = array3;
					foreach (DataRow item2 in array2)
					{
						if (!text.Equals(Function.ToString(item2["QCategory"])))
						{
							barSubItem3 = new BarSubItem();
							barSubItem3.Caption = Function.ToString(item2["CatName"]).Replace(" & ", " && ");
							barSubItem2.AddItem(barSubItem3);
							text = Function.ToString(item2["QCategory"]);
						}
						BarButtonItem barButtonItem2 = new BarButtonItem();
						barButtonItem2.Caption = Function.ToString(item2["QName"]).Replace(" & ", " && ");
						barButtonItem2.Tag = Function.ToString(item2["IntrnalKey"]);
						barButtonItem2.Description = Function.ToString(item2["IntrnalKey"]);
						barButtonItem2.Hint = Function.ToString(item2["QString"]);
						barButtonItem2.ItemClick += (object s, ItemClickEventArgs v) =>
						{
							try
							{
								WinCommons._OpenProcessing();
								BarManager barManager = (BarManager)s;
								string displayHint = barManager.PressedLink.DisplayHint;
								frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
								frm_QueryPreview2.Text = barManager.PressedLink.Caption;
								frm_QueryPreview2.ManagerQ = true;
								frm_QueryPreview2.QueryId = Function.ParseInt(item2["IntrnalKey"]);
								frm_QueryPreview2.Sql = displayHint;
								frm_QueryPreview2.MdiParent = this;
								frm_QueryPreview2.Show();
								WinCommons._CloseProcessing();
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.ErrorLineNumber(ex2);
								WinCommons._CloseProcessing();
							}
						};
						barSubItem3.AddItem(barButtonItem2);
					}
				}
			}
			dataSet.Tables.Clear();
			dataSet.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bbi_QueryManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ManagerQuery frm_ManagerQuery2 = new frm_ManagerQuery();
			frm_ManagerQuery2.MdiParent = this;
			frm_ManagerQuery2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
	}

	private void bar_PreviewExternalReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			string text = WinFuntions.PathOpenFile("Crystall Report File (.rpt)", "rpt");
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons._OpenProcessing();
				frm_SelectionCriteria frm_SelectionCriteria2 = new frm_SelectionCriteria();
				frm_SelectionCriteria2.pathFile = text;
				frm_SelectionCriteria2.MdiParent = this;
				frm_SelectionCriteria2.Show();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CreateMenuReport()
	{
		try
		{
			new ComponentResourceManager(typeof(Main));
			BarItemLink barItemLink = barSub_Module.ItemLinks.FirstOrDefault((BarItemLink m) => m.Item.Name.Contains("barSub_UserReport"));
			if (barItemLink != null)
			{
				barSub_Module.ItemLinks.Remove(barItemLink);
			}
			DataTable dataTable = await GetListUserReport();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			BarSubItem barSubItem = new BarSubItem();
			barSubItem.Name = "barSub_UserReport";
			barSubItem.Caption = "Report";
			barSubItem.MenuCaption = "Report";
			barSubItem.ImageOptions.ImageIndex = 25;
			barSub_Module.AddItem(barSubItem);
			int id = (from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ToString(r["menucode"]) == "barSub_CrystalReport"
				select Function.ParseInt(r["Id"])).FirstOrDefault();
			EnumerableRowCollection<string> lstItem = from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ParseInt(r["FatherId"]) == id && Function.ToString(r["authoriza"]) == "F"
				select Function.ToString(r["menucode"]);
			if (lstItem.Any())
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from r in dataTable.AsEnumerable()
					where lstItem.Contains("frm_" + Function.ToString(r["DocCode"]))
					select r;
				if (enumerableRowCollection.Any())
				{
					BarSubItem barSubItem2 = new BarSubItem();
					barSubItem2.Name = "barSub_CrystalReport";
					barSubItem2.MenuCaption = "Crystal Reports";
					barSubItem2.Caption = "Crystal Reports";
					barSubItem2.Tag = "FCrystalReport";
					barSubItem.AddItem(barSubItem2);
					new BarSubItem();
					foreach (DataRow row in enumerableRowCollection)
					{
						BarButtonItem barButtonItem = new BarButtonItem();
						barButtonItem.Name = "menu_frm_" + Function.ToString(row["DocCode"]);
						barButtonItem.Caption = Function.ToString(row["DocName"]).Replace(" & ", " && ");
						barButtonItem.Tag = Function.ToString(row["DocCode"]);
						barButtonItem.Description = Function.ToString(row["TypeCode"]);
						barButtonItem.Hint = Function.ToString(row["FileName"]);
						barButtonItem.ItemClick += (object s, ItemClickEventArgs v) =>
						{
							ShowReportCrytal(Function.ToString(row["DocCode"]));
						};
						barSubItem2.AddItem(barButtonItem);
					}
				}
			}
			id = (from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ToString(r["menucode"]) == "barSub_FormReport"
				select Function.ParseInt(r["Id"])).FirstOrDefault();
			lstItem = from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ParseInt(r["FatherId"]) == id && Function.ToString(r["authoriza"]) == "F"
				select Function.ToString(r["menucode"]);
			if (lstItem.Any())
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection2 = from r in dataTable.AsEnumerable()
					where lstItem.Contains("frm_" + Function.ToString(r["DocCode"]))
					select r;
				if (enumerableRowCollection2.Any())
				{
					BarSubItem barSubItem3 = new BarSubItem();
					barSubItem3.Name = "barSub_FormReport";
					barSubItem3.MenuCaption = "Form Reports";
					barSubItem3.Caption = "Form Reports";
					barSubItem3.Tag = "FFormReport";
					barSubItem.AddItem(barSubItem3);
					new BarSubItem();
					foreach (DataRow row2 in enumerableRowCollection2)
					{
						BarButtonItem barButtonItem2 = new BarButtonItem();
						barButtonItem2.Name = "menu_frm_" + Function.ToString(row2["DocCode"]);
						barButtonItem2.Caption = Function.ToString(row2["DocName"]).Replace(" & ", " && ");
						barButtonItem2.Tag = Function.ToString(row2["DocCode"]);
						barButtonItem2.Description = Function.ToString(row2["TypeCode"]);
						barButtonItem2.Hint = Function.ToString(row2["FileName"]);
						barButtonItem2.ItemClick += (object s, ItemClickEventArgs v) =>
						{
							ShowReportForm(Function.ToString(row2["DocCode"]), Function.ToString(row2["DocName"]));
						};
						barSubItem3.AddItem(barButtonItem2);
					}
				}
			}
			id = (from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ToString(r["menucode"]) == "barSub_XtraReport"
				select Function.ParseInt(r["Id"])).FirstOrDefault();
			lstItem = from r in GlobalData.DtAccessForm.AsEnumerable()
				where Function.ParseInt(r["FatherId"]) == id && Function.ToString(r["authoriza"]) == "F"
				select Function.ToString(r["menucode"]);
			if (lstItem.Any())
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection3 = from r in dataTable.AsEnumerable()
					where lstItem.Contains("frm_" + Function.ToString(r["DocCode"]))
					select r;
				if (enumerableRowCollection3.Any())
				{
					BarSubItem barSubItem4 = new BarSubItem();
					barSubItem4.Name = "barSub_XtraReport";
					barSubItem4.MenuCaption = "Xtra Reports";
					barSubItem4.Caption = "Xtra Reports";
					barSubItem.AddItem(barSubItem4);
					new BarSubItem();
					foreach (DataRow row3 in enumerableRowCollection3)
					{
						BarButtonItem barButtonItem3 = new BarButtonItem();
						barButtonItem3.Name = "menu_frm_" + Function.ToString(row3["DocName"]);
						barButtonItem3.Caption = Function.ToString(row3["DocName"]).Replace(" & ", " && ");
						barButtonItem3.Tag = Function.ToString(row3["DocCode"]);
						barButtonItem3.Description = Function.ToString(row3["TypeCode"]);
						barButtonItem3.Hint = Function.ToString(row3["FileName"]);
						barButtonItem3.ItemClick += (object s, ItemClickEventArgs v) =>
						{
							ShowReportXtra(Function.ToString(row3["DocName"]), Function.ToString(row3["StoreName"]));
						};
						barSubItem4.AddItem(barButtonItem3);
					}
				}
			}
			if (barSubItem.ItemLinks.Count == 0)
			{
				barSubItem.Visibility = BarItemVisibility.Never;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowReportCrytal(string code)
	{
		try
		{
			string fileNameReport = _httpClient.Commons.GetFileNameReport(code);
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_" + code))
			{
				string text = $"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{fileNameReport}";
				if (!FileIO.FileExist(text))
				{
					WinCommons.Error(Enums.MessageCode.ReportFileNotFoundWithPath, "", Path.GetFileName(fileNameReport));
					return;
				}
				WinCommons._OpenProcessing();
				frm_SelectionCriteria frm_SelectionCriteria2 = new frm_SelectionCriteria();
				frm_SelectionCriteria2.pathFile = text;
				frm_SelectionCriteria2.MdiParent = this;
				frm_SelectionCriteria2.Show();
				WinCommons._CloseProcessing();
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

	private void ShowReportForm(string code, string caption)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_ReportForm) && form.Text == caption)
				{
					form.Activate();
					return;
				}
			}
			frm_ReportForm frm_ReportForm2 = new frm_ReportForm(code);
			frm_ReportForm2.Name = "frm_" + code;
			frm_ReportForm2.Text = caption;
			frm_ReportForm2.MdiParent = this;
			frm_ReportForm2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ShowReportXtra(string code, string rpText)
	{
		try
		{
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_" + code))
			{
				throw new Exception(Resources.Msg_FormAccessDenied);
			}
			WinCommons._OpenProcessing();
			if (string.IsNullOrEmpty(rpText))
			{
				DataTable dataTable = await GetListUserReport();
				if (!dataTable.IsNotNull())
				{
					return;
				}
				DataRow[] array = dataTable.Select("TypeName = 'XtraReports' AND DocName = '" + code + "'");
				if (array.IsNotNull())
				{
					rpText = Function.ToString(array[0]["StoreName"]);
				}
			}
			string text = WinFuntions.DecryptStringAES(rpText, "APZ_RDOC");
			XtraReport rp;
			using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
			{
				streamWriter.Write(text);
				streamWriter.Flush();
				rp = XtraReport.FromStream(streamWriter.BaseStream);
			}
			string text2 = WinFuntions.XtraReportViewerManualParam(rp, text);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeShort, text2);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetListUserReport()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "Commons");
			return await _helper.GetDataTableAsync("GetListUserReports", new
			{
				userName = GlobalData.UserName,
				branch = GlobalData.DefaultBranch
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task CreateNavigateMenuReport()
	{
		try
		{
			foreach (LinkPersistInfo item in barSub_Module.LinksPersistInfo)
			{
				BarSubItem barSubItem = (BarSubItem)item.Item;
				List<BarItemLink> list = barSubItem.ItemLinks.Where((BarItemLink m) => m.DisplayHint.Contains("Reports")).ToList();
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				foreach (BarItemLink item2 in list)
				{
					barSubItem.ItemLinks.Remove(item2);
				}
			}
			ComponentResourceManager resources = new ComponentResourceManager(typeof(Main));
			DataSet dataSet = new DataSet();
			string userName = GlobalData.UserName;
			int defaultBranch = GlobalData.DefaultBranch;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Apzon.HttpClientHelper.Configuration.ApiServer, "XtraReportManager"))
			{
				dataSet = await helper.GetAsync("LoadReport", new
				{
					userName = userName,
					branch = defaultBranch
				});
			}
			if (!dataSet.IsHasStructure())
			{
				dataSet.Tables.Clear();
				dataSet.Dispose();
				return;
			}
			DataTable dataTable = dataSet.Tables[1];
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow[] array = dataTable.Select();
			foreach (DataRow row in array)
			{
				if (string.IsNullOrEmpty(Function.ToString(row["Text"])))
				{
					continue;
				}
				NavBarItem navBarItem = new NavBarItem(Function.ToString(row["DocName"]));
				navBarItem.Name = "nav_frm_" + Function.ToString(row["DocName"]);
				navBarItem.Hint = Function.ToString(row["Description"]);
				navBarItem.Tag = "Reports";
				navBarItem.SmallImage = (Image)resources.GetObject("nav_frm_HandoverDocument.SmallImage");
				navBarItem.LinkClicked += async (object s, NavBarLinkEventArgs v) =>
				{
					try
					{
						if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_" + Function.ToString(row["DocName"])))
						{
							throw new Exception(Resources.Msg_FormAccessDenied);
						}
						WinCommons._OpenProcessing();
						string text = WinFuntions.DecryptStringAES(Function.ToString(row["Text"]), "APZ_RDOC");
						XtraReport rp;
						using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
						{
							streamWriter.Write(text);
							streamWriter.Flush();
							rp = XtraReport.FromStream(streamWriter.BaseStream);
						}
						string text2 = WinFuntions.XtraReportViewerManualParam(rp, text);
						if (!string.IsNullOrEmpty(text2))
						{
							WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeShort, text2);
						}
						WinCommons._CloseProcessing();
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				BarButtonItem barButtonItem = new BarButtonItem();
				barButtonItem.Name = "menu_frm_" + Function.ToString(row["DocName"]);
				barButtonItem.Caption = Function.ToString(row["DocName"]).Replace(" & ", " && ");
				barButtonItem.Description = Function.ToString(row["Description"]);
				barButtonItem.Hint = "Reports";
				barButtonItem.ItemClick += async (object s, ItemClickEventArgs v) =>
				{
					try
					{
						if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_" + Function.ToString(row["DocName"])))
						{
							throw new Exception(Resources.Msg_FormAccessDenied);
						}
						WinCommons._OpenProcessing();
						string text = WinFuntions.DecryptStringAES(Function.ToString(row["Text"]), "APZ_RDOC");
						XtraReport rp;
						using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
						{
							streamWriter.Write(text);
							streamWriter.Flush();
							rp = XtraReport.FromStream(streamWriter.BaseStream);
						}
						string text2 = WinFuntions.XtraReportViewerManualParam(rp, text);
						if (!string.IsNullOrEmpty(text2))
						{
							WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeShort, text2);
						}
						WinCommons._CloseProcessing();
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				switch (Function.ToString(row["Menu"]))
				{
				case "Administrator":
					navGroupReport_Administration.ItemLinks.Add(navBarItem);
					barSub_Administration.AddItem(barButtonItem);
					break;
				case "Sales Order":
					navGroupReport_Sales.ItemLinks.Add(navBarItem);
					barSub_SalesAR.AddItem(barButtonItem);
					break;
				case "Purchase Order":
					navGroupReport_Purchase.ItemLinks.Add(navBarItem);
					barSub_PurchaseAP.AddItem(barButtonItem);
					break;
				case "Business Master Data":
					navGroupReport_BusinessPartner.ItemLinks.Add(navBarItem);
					barSub_BP.AddItem(barButtonItem);
					break;
				case "Inventory":
					navGroupReport_Inventory.ItemLinks.Add(navBarItem);
					barSub_Inventory.AddItem(barButtonItem);
					break;
				case "Production Order":
					navGroupReport_Production.ItemLinks.Add(navBarItem);
					barSub_Production.AddItem(barButtonItem);
					break;
				case "MRP":
					navGroupReport_MRP.ItemLinks.Add(navBarItem);
					barSub_MRP.AddItem(barButtonItem);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_Preview_ItemClick(object sender, ItemClickEventArgs e)
	{
		PreviewLayout();
	}

	private void bar_ItemClick(object sender, ItemClickEventArgs e)
	{
		PrintReport();
	}

	private void barSub_Print_ItemClick(object sender, ItemClickEventArgs e)
	{
		PrintReport();
	}

	private void PrintReport()
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (!((popupForm == null) ? true : false))
			{
				popupForm.PRINT();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void bar_PreviewLayouts_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (!((popupForm == null) ? true : false))
			{
				popupForm.PREVIEWLAYOUTS();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void barButtonItem_QueryGenerator_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_QueryGenerator frm_QueryGenerator2 = new frm_QueryGenerator();
			frm_QueryGenerator2.MdiParent = this;
			frm_QueryGenerator2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_BP_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
			frm_BusinessMasterData2.MdiParent = this;
			frm_BusinessMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_Activity_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ActivitiesForOpportunity frm_ActivitiesForOpportunity2 = new frm_ActivitiesForOpportunity();
			frm_ActivitiesForOpportunity2.MdiParent = this;
			frm_ActivitiesForOpportunity2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_SQ_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch();
			frm_SalesQuotationForBatch2.MdiParent = this;
			frm_SalesQuotationForBatch2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_BQ_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch();
			frm_SalesQuotationForBatch2.MdiParent = this;
			frm_SalesQuotationForBatch2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_SO_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder();
			frm_SalesOrder2.MdiParent = this;
			frm_SalesOrder2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_Delivery_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Delivery frm_Delivery2 = new frm_Delivery();
			frm_Delivery2.MdiParent = this;
			frm_Delivery2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_Return_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Return frm_Return2 = new frm_Return();
			frm_Return2.MdiParent = this;
			frm_Return2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_ARI_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ARInvoices frm_ARInvoices2 = new frm_ARInvoices();
			frm_ARInvoices2.MdiParent = this;
			frm_ARInvoices2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_ARCM_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ARCreditMemos frm_ARCreditMemos2 = new frm_ARCreditMemos();
			frm_ARCreditMemos2.MdiParent = this;
			frm_ARCreditMemos2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_ARRI_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AR_ReserveInvoices frm_AR_ReserveInvoices2 = new frm_AR_ReserveInvoices();
			frm_AR_ReserveInvoices2.MdiParent = this;
			frm_AR_ReserveInvoices2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_DDR_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_DocumentDraftsRP frm_DocumentDraftsRP2 = new frm_DocumentDraftsRP();
			frm_DocumentDraftsRP2.MdiParent = this;
			frm_DocumentDraftsRP2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem_AToPromise_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_Atp frm_Atp2 = new frm_Atp();
		frm_Atp2.MdiParent = this;
		frm_Atp2.Show();
	}

	private void menu_frm_DeliveryControl_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_DeliveryControl))
				{
					form.Focus();
					return;
				}
			}
			frm_DeliveryControl frm_DeliveryControl2 = new frm_DeliveryControl();
			frm_DeliveryControl2.MdiParent = this;
			frm_DeliveryControl2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesControl_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_SalesControl))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_SalesControl frm_SalesControl2 = new frm_SalesControl();
			frm_SalesControl2.MdiParent = this;
			frm_SalesControl2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void nav_frm_WorkingTable_LinkPressed(object sender, NavBarLinkEventArgs e)
	{
	}

	private async void timer_Alert_Tick(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (GlobalData.AlertTime == 0)
			{
				timer_Alert.Stop();
				return;
			}
			if (CheckShowAlert())
			{
				notificationContrainerMain.LoadAlertOverDue(alertMain);
			}
			if (!_getAlterMessage)
			{
				return;
			}
			HttpResult httpResult = await LoadAlertMessage();
			if (httpResult.Message.StartsWith("ERR:"))
			{
				_getAlterMessage = false;
				return;
			}
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(httpResult.Content.ToString());
			if (dataTable.IsNotNull())
			{
				foreach (DataRow row in dataTable.Rows)
				{
					AlertInfo info = new AlertInfo("Approval alert from " + Function.ToString(row["FromUser"]), Function.ToString(row["Subject"]), Function.ToString(row["Subject"]), null, "W");
					alertMain.ShowAlert(this, info);
				}
			}
			await LoadAlertOverview();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadAlertOverview()
	{
		try
		{
			DataTable dataTable = await GetAlert();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				AlertInfo info = new AlertInfo("Alert from " + Function.ToString(row["FromUser"]), Function.ToString(row["Name"]), Function.ToString(row["Name"]), null, "A");
				alertMain.ShowAlert(this, info);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetAlert()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "AlertOverview");
			return await hp.GetAsync("GetAlert", new
			{
				username = GlobalData.UserName,
				mailtype = 0
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<HttpResult> LoadAlertMessage()
	{
		try
		{
			using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Apzon.HttpClientHelper.Configuration.ApiServer, "WorkingTable");
			return await hp.GetAsync("GetTopDraft");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult();
		}
	}

	private void alertMain_AlertClick(object sender, AlertClickEventArgs e)
	{
		try
		{
			Activate();
			if (Function.ToString(e.Info.Tag) == "A")
			{
				FormCollection openForms = Application.OpenForms;
				for (int i = 0; i < openForms.Count; i++)
				{
					Form form = openForms[i];
					if (form.Name == "Main")
					{
						if (form.ActiveMdiChild == null || !(form.ActiveMdiChild.Name == frm_AlertOverview.FormName))
						{
							break;
						}
						return;
					}
				}
				bool flag = false;
				for (int j = 0; j < openForms.Count; j++)
				{
					if (openForms[j].Name == frm_AlertOverview.FormName)
					{
						Form form2 = Application.OpenForms[frm_AlertOverview.FormName];
						if (form2 != null && !form2.IsDisposed && !form2.Disposing)
						{
							form2.Focus();
							flag = true;
						}
						break;
					}
				}
				if (!flag)
				{
					frm_AlertOverview frm_AlertOverview2 = new frm_AlertOverview();
					frm_AlertOverview2.MdiParent = this;
					frm_AlertOverview2.Show();
				}
			}
			else if (Function.ToString(e.Info.Tag) == "W")
			{
				frm_WorkingProcess frm_WorkingProcess2 = new frm_WorkingProcess();
				frm_WorkingProcess2.MdiParent = this;
				frm_WorkingProcess2.Show();
			}
			else if (e.Info.Tag is NotifiedObj { ObjectTypes: var objectTypes } notifiedObj)
			{
				switch (objectTypes)
				{
				case ApzObjectTypes.oNoteManagement:
				case ApzObjectTypes.oTaskManagement:
				case ApzObjectTypes.oEventManagement:
					new XtraFormEx
					{
						MdiParent = FindForm()
					}.DrillDown(notifiedObj.ObjectTypes, notifiedObj.NotifiedId);
					break;
				case ApzObjectTypes.oTrackingIssue:
				{
					frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue(notifiedObj.TranType == Enums.TransactionType.M, Function.ParseInt(notifiedObj.NotifiedId));
					frm_TrackingIssue2.MdiParent = this;
					frm_TrackingIssue2.Show();
					break;
				}
				case ApzObjectTypes.oAlertOverview:
				{
					frm_AlertOverview frm_AlertOverview3 = new frm_AlertOverview();
					frm_AlertOverview3.MdiParent = this;
					frm_AlertOverview3.Show();
					break;
				}
				}
				notificationContrainerMain.ClearNotified(notifiedObj.ObjectTypes, notifiedObj.NotifiedId, notifiedObj.TranType);
				if (notifiedObj.TranType == Enums.TransactionType.M)
				{
					_httpClient.SubActivities.MarkAsRead(notifiedObj.CmtCode);
				}
			}
			for (int k = 0; k < alertMain.AlertFormList.Count; k++)
			{
				alertMain.AlertFormList[k].Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_UserDefineFields_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UserDefineFieldsManagement frm_UserDefineFieldsManagement2 = new frm_UserDefineFieldsManagement();
			frm_UserDefineFieldsManagement2.MdiParent = this;
			frm_UserDefineFieldsManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_LicenseManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			if (frm_LicenseManager.IsOpening())
			{
				if (!(ActiveMdiChild is frm_LicenseManager))
				{
					frm_LicenseManager instance = frm_LicenseManager.Instance;
					bool flag = ActiveMdiChild.WindowState == FormWindowState.Maximized;
					if (flag)
					{
						XtraFormEx.LockWindowUpdate(instance.Handle);
					}
					instance.Activate();
					if (flag)
					{
						XtraFormEx.LockWindowUpdate(IntPtr.Zero);
					}
				}
			}
			else
			{
				frm_LicenseManager instance2 = frm_LicenseManager.Instance;
				instance2.MdiParent = this;
				instance2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_UDT_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UDTManager frm_UDTManager2 = new frm_UDTManager();
			frm_UDTManager2.MdiParent = this;
			frm_UDTManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem25_ItemClick(object sender, ItemClickEventArgs e)
	{
		navChangeUser_LinkClicked(null, null);
	}

	private void menu_frm_Authorization_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			if (frm_Authorization.IsOpening())
			{
				if (!(ActiveMdiChild is frm_Authorization))
				{
					frm_Authorization instance = frm_Authorization.Instance;
					bool flag = ActiveMdiChild.WindowState == FormWindowState.Maximized;
					if (flag)
					{
						XtraFormEx.LockWindowUpdate(instance.Handle);
					}
					instance.Activate();
					if (flag)
					{
						XtraFormEx.LockWindowUpdate(IntPtr.Zero);
					}
				}
			}
			else
			{
				frm_Authorization instance2 = frm_Authorization.Instance;
				instance2.RefreshMain = (RefreshMain)Delegate.Combine(instance2.RefreshMain, new RefreshMain(ReloadAuthorization));
				instance2.MdiParent = this;
				instance2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkingTable_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_WorkingTable frm_WorkingTable2 = new frm_WorkingTable();
			frm_WorkingTable2.MdiParent = this;
			frm_WorkingTable2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_FilterSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FilterSetting frm_FilterSetting2 = new frm_FilterSetting();
			frm_FilterSetting2.MdiParent = this;
			frm_FilterSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Freight_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Freight frm_Freight2 = new frm_Freight();
			frm_Freight2.MdiParent = this;
			frm_Freight2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_LengthAndWidth_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_LengthAndWidth frm_LengthAndWidth2 = new frm_LengthAndWidth();
			frm_LengthAndWidth2.MdiParent = this;
			frm_LengthAndWidth2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CustomerGroup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CustomerGroup frm_CustomerGroup2 = new frm_CustomerGroup();
			frm_CustomerGroup2.MdiParent = this;
			frm_CustomerGroup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_VenderGroup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_VendorGroup frm_VendorGroup2 = new frm_VendorGroup();
			frm_VendorGroup2.MdiParent = this;
			frm_VendorGroup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Manufacturer_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Manufacturer frm_Manufacturer2 = new frm_Manufacturer();
			frm_Manufacturer2.MdiParent = this;
			frm_Manufacturer2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WareHourse_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
			frm_WareHouses2.MdiParent = this;
			frm_WareHouses2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BusinessPartnerProperties_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BusinessPartnerProperties frm_BusinessPartnerProperties2 = new frm_BusinessPartnerProperties();
			frm_BusinessPartnerProperties2.MdiParent = this;
			frm_BusinessPartnerProperties2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BusinessPartnerPriorities_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BusinessPartnerPriorities frm_BusinessPartnerPriorities2 = new frm_BusinessPartnerPriorities();
			frm_BusinessPartnerPriorities2.MdiParent = this;
			frm_BusinessPartnerPriorities2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem29_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_GeneSetting frm_GeneSetting2 = new frm_GeneSetting
			{
				MdiParent = this
			};
			frm_GeneSetting2.RefreshMain = (RefreshMain)Delegate.Combine(frm_GeneSetting2.RefreshMain, new RefreshMain(RefreshGeneralSetting));
			frm_GeneSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Shipping_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Shipping frm_Shipping2 = new frm_Shipping();
			frm_Shipping2.MdiParent = this;
			frm_Shipping2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_TaxGroups_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_TaxGroups frm_TaxGroups2 = new frm_TaxGroups();
			frm_TaxGroups2.MdiParent = this;
			frm_TaxGroups2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Braches_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Braches frm_Braches2 = new frm_Braches();
			frm_Braches2.MdiParent = this;
			frm_Braches2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Locations_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Locations frm_Locations2 = new frm_Locations();
			frm_Locations2.MdiParent = this;
			frm_Locations2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ItemProperties_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ItemProperties frm_ItemProperties2 = new frm_ItemProperties();
			frm_ItemProperties2.MdiParent = this;
			frm_ItemProperties2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ItemGroups_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ItemGroups frm_ItemGroups2 = new frm_ItemGroups();
			frm_ItemGroups2.MdiParent = this;
			frm_ItemGroups2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_UnitOfMeasure_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UnitOfMeasure frm_UnitOfMeasure2 = new frm_UnitOfMeasure();
			frm_UnitOfMeasure2.MdiParent = this;
			frm_UnitOfMeasure2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_UnitOfMeasureGroups_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UnitOfMeasureGroups frm_UnitOfMeasureGroups2 = new frm_UnitOfMeasureGroups();
			frm_UnitOfMeasureGroups2.MdiParent = this;
			frm_UnitOfMeasureGroups2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Weight_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Weight frm_Weight2 = new frm_Weight();
			frm_Weight2.MdiParent = this;
			frm_Weight2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Currencies_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Currencies frm_Currencies2 = new frm_Currencies();
			frm_Currencies2.MdiParent = this;
			frm_Currencies2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesEmployeeBuyers_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesEmployeeBuyers frm_SalesEmployeeBuyers2 = new frm_SalesEmployeeBuyers();
			frm_SalesEmployeeBuyers2.MdiParent = this;
			frm_SalesEmployeeBuyers2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CompanyDetails_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CompanyDetails frm_CompanyDetails2 = new frm_CompanyDetails();
			frm_CompanyDetails2.MdiParent = this;
			frm_CompanyDetails2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_FtpServerSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FtpServerSetting frm_FtpServerSetting2 = new frm_FtpServerSetting();
			frm_FtpServerSetting2.MdiParent = this;
			frm_FtpServerSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_LoginSap_ItemClick(object sender, ItemClickEventArgs e)
	{
		navChangeUser_LinkClicked(null, null);
	}

	private void menu_frm_DataOwnerShipAuthorization_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DataOwnerShipAuthorization frm_DataOwnerShipAuthorization2 = new frm_DataOwnerShipAuthorization();
			frm_DataOwnerShipAuthorization2.MdiParent = this;
			frm_DataOwnerShipAuthorization2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_PurchaseRequest frm_PurchaseRequest2 = new frm_PurchaseRequest();
			frm_PurchaseRequest2.MdiParent = this;
			frm_PurchaseRequest2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseQuotation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_PurchaseQuotation frm_PurchaseQuotation2 = new frm_PurchaseQuotation();
			frm_PurchaseQuotation2.MdiParent = this;
			frm_PurchaseQuotation2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseOder_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_PurchaseOder frm_PurchaseOder2 = new frm_PurchaseOder();
			frm_PurchaseOder2.MdiParent = this;
			frm_PurchaseOder2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GoodReceiptPO_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GoodReceiptPO frm_GoodReceiptPO2 = new frm_GoodReceiptPO();
			frm_GoodReceiptPO2.MdiParent = this;
			frm_GoodReceiptPO2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GoodsReturn_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GoodsReturn frm_GoodsReturn2 = new frm_GoodsReturn();
			frm_GoodsReturn2.MdiParent = this;
			frm_GoodsReturn2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_APInvoices_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_APInvoices frm_APInvoices2 = new frm_APInvoices();
			frm_APInvoices2.MdiParent = this;
			frm_APInvoices2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_APCreditMemos_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_APCreditMemos frm_APCreditMemos2 = new frm_APCreditMemos();
			frm_APCreditMemos2.MdiParent = this;
			frm_APCreditMemos2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_APReserveInvoice_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_APReserveInvoices frm_APReserveInvoices2 = new frm_APReserveInvoices();
			frm_APReserveInvoices2.MdiParent = this;
			frm_APReserveInvoices2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProcurementConfirm_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ProcurementConfirm frm_ProcurementConfirm2 = new frm_ProcurementConfirm();
			frm_ProcurementConfirm2.MdiParent = this;
			frm_ProcurementConfirm2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ItemMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: false, string.Empty);
			frm_ItemMasterData2.MdiParent = this;
			frm_ItemMasterData2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GoodsReceiptIT_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GoodsReceipt frm_GoodsReceipt2 = new frm_GoodsReceipt();
			frm_GoodsReceipt2.MdiParent = this;
			frm_GoodsReceipt2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GoodsIssueIT_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GoodsIssue frm_GoodsIssue2 = new frm_GoodsIssue();
			frm_GoodsIssue2.MdiParent = this;
			frm_GoodsIssue2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryTranferRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest();
			frm_InventoryTranferRequest2.MdiParent = this;
			frm_InventoryTranferRequest2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryTranfer_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer();
			frm_InventoryTranfer2.MdiParent = this;
			frm_InventoryTranfer2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryRevaluation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_InventoryRevaluation frm_InventoryRevaluation2 = new frm_InventoryRevaluation();
			frm_InventoryRevaluation2.MdiParent = this;
			frm_InventoryRevaluation2.frmShowChildrenForm = ShowForm;
			frm_InventoryRevaluation2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PickAndPack_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_PickAndPack))
				{
					form.Focus();
					return;
				}
			}
			frm_PickAndPack frm_PickAndPack2 = new frm_PickAndPack();
			frm_PickAndPack2.MdiParent = this;
			frm_PickAndPack2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PickingRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_PickingRequest frm_PickingRequest2 = new frm_PickingRequest();
			frm_PickingRequest2.MdiParent = this;
			frm_PickingRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_MRP_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_MRP frm_MRP2 = new frm_MRP();
			frm_MRP2.MdiParent = this;
			frm_MRP2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProductionSchedule_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Schedule frm_Schedule2 = new frm_Schedule();
			frm_Schedule2.MdiParent = this;
			frm_Schedule2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GenPOFromSchedule_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_GenPOFromSchedule frm_GenPOFromSchedule2 = new frm_GenPOFromSchedule();
			frm_GenPOFromSchedule2.MdiParent = this;
			frm_GenPOFromSchedule2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PrintBarCode_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PrintBarCode frm_PrintBarCode2 = new frm_PrintBarCode();
			frm_PrintBarCode2.MdiParent = this;
			frm_PrintBarCode2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AutoReportComplete_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AutoReportComplete frm_AutoReportComplete2 = new frm_AutoReportComplete();
			frm_AutoReportComplete2.MdiParent = this;
			frm_AutoReportComplete2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Solution_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Solution frm_Solution2 = new frm_Solution();
			frm_Solution2.MdiParent = this;
			frm_Solution2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ServiceCall_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ServiceCalls frm_ServiceCalls2 = new frm_ServiceCalls();
			frm_ServiceCalls2.MdiParent = this;
			frm_ServiceCalls2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Contract_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Contract frm_Contract2 = new frm_Contract();
			frm_Contract2.MdiParent = this;
			frm_Contract2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ServiceCallSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ServiceCallSetting frm_ServiceCallSetting2 = new frm_ServiceCallSetting();
			frm_ServiceCallSetting2.MdiParent = this;
			frm_ServiceCallSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_HumanResources_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_HumanResources frm_HumanResources2 = new frm_HumanResources();
			frm_HumanResources2.MdiParent = this;
			frm_HumanResources2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JobVacancy_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_JobVacancy frm_JobVacancy2 = new frm_JobVacancy();
			frm_JobVacancy2.MdiParent = this;
			frm_JobVacancy2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Operation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Operation frm_Operation2 = new frm_Operation();
			frm_Operation2.MdiParent = this;
			frm_Operation2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Resources_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Resources frm_Resources2 = new frm_Resources();
			frm_Resources2.MdiParent = this;
			frm_Resources2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_IssueForProduction_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_IssueForProduction frm_IssueForProduction2 = new frm_IssueForProduction();
			frm_IssueForProduction2.MdiParent = this;
			frm_IssueForProduction2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetAutoDataSearchMenu()
	{
		try
		{
			if (GlobalData.ListAccessForm == null || GlobalData.ListAccessForm.Count == 0)
			{
				return;
			}
			List<string> list = (from d in GlobalData.ListAccessForm
				where d.authoriza != "N" && d.MenuType != "L"
				select d.menuName).ToList();
			foreach (object itemLink in barSub_Tool.ItemLinks)
			{
				list.AddRange(GetAllChildItemLink(itemLink));
			}
			txt_MenuSearch.Values = list.Distinct().ToArray();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadMenu, "");
		}
	}

	private List<string> GetAllChildItemLink(object item)
	{
		List<string> list = new List<string>();
		try
		{
			if (item is BarSubItemLink barSubItemLink)
			{
				if (barSubItemLink.Item.Name != barSub_Language.Name && barSubItemLink.Item.Name != barSub_Configuration.Name && barSubItemLink.Item.Name != barSub_Integrated.Name)
				{
					foreach (object itemLink in barSubItemLink.Item.ItemLinks)
					{
						list.AddRange(GetAllChildItemLink(itemLink));
					}
				}
			}
			else if (item is BarItemLink barItemLink && (barItemLink.Item.Name.Length > 5 || barItemLink.Item.Name.Contains("@")))
			{
				string frm = (barItemLink.Item.Name.Contains("@") ? barItemLink.Item.Name : barItemLink.Item.Name.Remove(0, 5));
				if (!CommonList.HiddenFunction.ContainsKey(frm) || !CommonList.HiddenFunction[frm])
				{
					AuthsImpl authsImpl = GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl x) => x.authoriza.Trim() != "N" && x.menucode.Trim() == frm.Trim());
					if (authsImpl != null)
					{
						list.Add(authsImpl.menuName);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	private void Main_MdiChildActivate(object sender, EventArgs e)
	{
		try
		{
			Form activeMdiChild = ActiveMdiChild;
			XtraFormEx f = activeMdiChild as XtraFormEx;
			if (f == null)
			{
				EnableAllDirectionalControl(isEnable: false);
			}
			else
			{
				Task.Run(() =>
				{
					try
					{
						f.EnableAllDirectionalControl(isEnable: true);
					}
					catch (Exception)
					{
					}
				});
			}
			if (ActiveMdiChild == null)
			{
				return;
			}
			Form form = ActiveMdiChild;
			if (!barSub_Window.ItemLinks.Any((BarItemLink t) => t.Item.Name == form.Name))
			{
				int num = MdiChildren.Max((Form t) => Function.ParseInt(t.Name));
				form.Name = Function.ToString(num + 1);
				form.FormClosed += mdiChildForm_FormClosed;
				form.TextChanged += Form_TextChanged;
				XtraTabPage page = new XtraTabPage
				{
					Name = form.Name,
					Text = form.Text,
					BorderStyle = BorderStyle.None,
					BackColor = Color.Transparent
				};
				tab_MdiForm.TabPages.Add(page);
				if (!tab_MdiForm.Visible)
				{
					tab_MdiForm.Visible = true;
				}
				BarCheckItem barCheckItem = new BarCheckItem();
				barCheckItem.Caption = form.Text;
				barCheckItem.Name = form.Name;
				barCheckItem.CheckedChanged += barWindowItem_CheckedChanged;
				barSub_Window.ItemLinks.Add(barCheckItem);
			}
			else if (form.WindowState == FormWindowState.Minimized)
			{
				form.WindowState = FormWindowState.Normal;
			}
			XtraTabPage xtraTabPage = tab_MdiForm.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == form.Name);
			if (xtraTabPage != null)
			{
				tab_MdiForm.SelectedTabPage = xtraTabPage;
			}
			_userRaise = false;
			for (int num2 = 0; num2 < barSub_Window.ItemLinks.Count; num2++)
			{
				if (Function.ParseInt(barSub_Window.ItemLinks[num2].Item.Name) > 0)
				{
					((BarCheckItem)barSub_Window.ItemLinks[num2].Item).Checked = barSub_Window.ItemLinks[num2].Item.Name == form.Name;
				}
			}
			_userRaise = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Form_TextChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is XtraForm) && !(sender is RibbonForm))
			{
				return;
			}
			XtraForm form = sender as XtraForm;
			if (!string.IsNullOrWhiteSpace(form.Text))
			{
				XtraTabPage xtraTabPage = tab_MdiForm.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == form.Name);
				if (xtraTabPage != null)
				{
					xtraTabPage.Text = form.Text;
				}
				BarItemLink barItemLink = barSub_Window.ItemLinks.FirstOrDefault((BarItemLink t) => t.Item.Name.Equals(form.Name));
				if (barItemLink != null)
				{
					barItemLink.Item.Caption = form.Text;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", "Form_TextChanged", ex.ToString());
		}
	}

	private void mdiChildForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		try
		{
			if (sender is XtraForm || sender is RibbonForm)
			{
				XtraForm form = sender as XtraForm;
				tab_MdiForm.TabPages.Remove(tab_MdiForm.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == form.Name), destroyPage: true);
				if (tab_MdiForm.TabPages.Count == 0)
				{
					tab_MdiForm.Visible = false;
				}
				barSub_Window.ItemLinks.Remove(barSub_Window.ItemLinks.FirstOrDefault((BarItemLink t) => t.Item.Name == form.Name));
				if (!MdiChildren.Any())
				{
					tab_MdiForm.TabPages.Clear(destroyPages: true);
				}
				tab_MdiForm.Visible = tab_MdiForm.TabPages.Count > 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barWindowItem_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (!_userRaise)
		{
			return;
		}
		try
		{
			if (!((BarCheckItem)sender).Checked)
			{
				((BarCheckItem)sender).Checked = true;
				return;
			}
			string nameformActive = ((BarCheckItem)sender).Name;
			Form form = MdiChildren.FirstOrDefault((Form f) => f.Name == nameformActive);
			if (form == null)
			{
				return;
			}
			Form activeMdiChild = ActiveMdiChild;
			int num;
			if (activeMdiChild == null)
			{
				num = 0;
			}
			else
			{
				num = ((activeMdiChild.WindowState == FormWindowState.Maximized) ? 1 : 0);
				if (num != 0)
				{
					XtraFormEx.LockWindowUpdate(form.Handle);
				}
			}
			form.Activate();
			if (num != 0)
			{
				XtraFormEx.LockWindowUpdate(IntPtr.Zero);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem20_ItemClick_1(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DocumentNumbering frm_DocumentNumbering2 = new frm_DocumentNumbering();
			frm_DocumentNumbering2.MdiParent = this;
			frm_DocumentNumbering2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BillOfMaterials_ItemClick_1(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_BillOfMaterials frm_BillOfMaterials2 = new frm_BillOfMaterials();
			frm_BillOfMaterials2.MdiParent = this;
			frm_BillOfMaterials2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProductionOrder_ItemClick_1(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder();
			frm_ProductionOrder2.MdiParent = this;
			frm_ProductionOrder2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProductionReqForBatch_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_ProductionReqForBatch))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_ProductionReqForBatch frm_ProductionReqForBatch2 = new frm_ProductionReqForBatch();
			frm_ProductionReqForBatch2.MdiParent = this;
			frm_ProductionReqForBatch2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProductionForBatch_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_ProductionForBatch))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_ProductionForBatch frm_ProductionForBatch2 = new frm_ProductionForBatch();
			frm_ProductionForBatch2.MdiParent = this;
			frm_ProductionForBatch2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProductionConfirm_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_ProductionConfirm))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_ProductionConfirm frm_ProductionConfirm2 = new frm_ProductionConfirm();
			frm_ProductionConfirm2.MdiParent = this;
			frm_ProductionConfirm2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PackingBatch_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_PackingBatch))
				{
					form.Focus();
					return;
				}
			}
			frm_PackingBatch frm_PackingBatch2 = new frm_PackingBatch();
			frm_PackingBatch2.MdiParent = this;
			frm_PackingBatch2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_StockRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_StockRequest))
				{
					form.Focus();
					return;
				}
			}
			frm_StockRequest frm_StockRequest2 = new frm_StockRequest();
			frm_StockRequest2.MdiParent = this;
			frm_StockRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_nav_frm_FieldActivation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FieldActivation frm_FieldActivation2 = new frm_FieldActivation();
			frm_FieldActivation2.MdiParent = this;
			frm_FieldActivation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_nav_frm_AttrCodes_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AttrCodeSetup frm_AttrCodeSetup2 = new frm_AttrCodeSetup();
			frm_AttrCodeSetup2.MdiParent = this;
			frm_AttrCodeSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_nav_frm_WHSub_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WHSubSetup frm_WHSubSetup2 = new frm_WHSubSetup();
			frm_WHSubSetup2.MdiParent = this;
			frm_WHSubSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_nav_frm_WHSubMan_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WHSubManager frm_WHSubManager2 = new frm_WHSubManager();
			frm_WHSubManager2.MdiParent = this;
			frm_WHSubManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PrintLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PrintLayout frm_PrintLayout2 = new frm_PrintLayout();
			frm_PrintLayout2.MdiParent = this;
			frm_PrintLayout2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Project_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Project frm_Project2 = new frm_Project();
			frm_Project2.MdiParent = this;
			frm_Project2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BusinessPartnerCatalogNumbers_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_BusinessPartnerCatalogNumbers frm_BusinessPartnerCatalogNumbers2 = new frm_BusinessPartnerCatalogNumbers();
			frm_BusinessPartnerCatalogNumbers2.MdiParent = this;
			frm_BusinessPartnerCatalogNumbers2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdvanceForecast_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AdvanceForecast frm_AdvanceForecast2 = new frm_AdvanceForecast();
			frm_AdvanceForecast2.MdiParent = this;
			frm_AdvanceForecast2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_FrmDefinedShift_ItemPress(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			FrmDefinedShift frmDefinedShift = new FrmDefinedShift();
			frmDefinedShift.MdiParent = this;
			frmDefinedShift.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_FrmWorkScheduling_ItemPress(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			FrmWorkScheduling frmWorkScheduling = new FrmWorkScheduling();
			frmWorkScheduling.MdiParent = this;
			frmWorkScheduling.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_Lines_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Lines frm_Lines2 = new frm_Lines();
			frm_Lines2.MdiParent = this;
			frm_Lines2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ItemQuotaArrangement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ItemQuotaArrangement frm_ItemQuotaArrangement2 = new frm_ItemQuotaArrangement();
			frm_ItemQuotaArrangement2.MdiParent = this;
			frm_ItemQuotaArrangement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BarcodeConfig_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BarcodeConfig frm_BarcodeConfig2 = new frm_BarcodeConfig();
			frm_BarcodeConfig2.MdiParent = this;
			frm_BarcodeConfig2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdvanceBillOfMaterials_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AdvanceBillOfMaterials frm_AdvanceBillOfMaterials2 = new frm_AdvanceBillOfMaterials();
			frm_AdvanceBillOfMaterials2.MdiParent = this;
			frm_AdvanceBillOfMaterials2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GenPurchaseFromSchedule_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_GenPurchaseFromSchedule frm_GenPurchaseFromSchedule2 = new frm_GenPurchaseFromSchedule();
			frm_GenPurchaseFromSchedule2.MdiParent = this;
			frm_GenPurchaseFromSchedule2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem21_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Users frm_Users2 = new frm_Users
			{
				MdiParent = this
			};
			frm_Users2.RefreshMain = (RefreshMain)Delegate.Combine(frm_Users2.RefreshMain, new RefreshMain(ReloadAuthorization));
			frm_Users2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem26_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
			frm_ExchangeRatesAndIndexs2.MdiParent = this;
			frm_ExchangeRatesAndIndexs2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void barI_frm_CustomerGroup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CustomsGroup frm_CustomsGroup2 = new frm_CustomsGroup();
			frm_CustomsGroup2.MdiParent = this;
			frm_CustomsGroup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_Schedule_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ActivityCalendar frm_ActivityCalendar2 = new frm_ActivityCalendar();
			frm_ActivityCalendar2.MdiParent = this;
			frm_ActivityCalendar2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SetupAdmin_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SetupAdmin frm_SetupAdmin2 = new frm_SetupAdmin();
			frm_SetupAdmin2.MdiParent = this;
			frm_SetupAdmin2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ReceiptBin_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_ReceiptBin))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_ReceiptBin frm_ReceiptBin2 = new frm_ReceiptBin();
			frm_ReceiptBin2.MdiParent = this;
			frm_ReceiptBin2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_EmpToken_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_EmpToken))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_EmpToken frm_EmpToken2 = new frm_EmpToken();
			frm_EmpToken2.MdiParent = this;
			frm_EmpToken2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_Alert_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AlertOverview frm_AlertOverview2 = new frm_AlertOverview();
			frm_AlertOverview2.MdiParent = this;
			frm_AlertOverview2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_SystemMessageLog_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dockMessageLog.Visibility = ((!barSub_SystemMessageLog.Checked) ? DockVisibility.Hidden : DockVisibility.Visible);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dockMessageLog_ClosedPanel(object sender, DockPanelEventArgs e)
	{
		barSub_SystemMessageLog.Checked = false;
	}

	private void menu_frm_Countries_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Countries frm_Countries2 = new frm_Countries();
			frm_Countries2.MdiParent = this;
			frm_Countries2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CreditCardSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_CreditCardSetup frm_CreditCardSetup2 = new frm_CreditCardSetup();
			frm_CreditCardSetup2.MdiParent = this;
			frm_CreditCardSetup2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_CreditPaymentMethods_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_CreditPaymentMethods frm_CreditPaymentMethods2 = new frm_CreditPaymentMethods();
			frm_CreditPaymentMethods2.MdiParent = this;
			frm_CreditPaymentMethods2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_RetailStore_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_RetailStore frm_RetailStore2 = new frm_RetailStore();
			frm_RetailStore2.MdiParent = this;
			frm_RetailStore2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_DistributeRule_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DistributeRule frm_DistributeRule2 = new frm_DistributeRule();
			frm_DistributeRule2.MdiParent = this;
			frm_DistributeRule2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_MenuSearch_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				if (string.IsNullOrEmpty(Function.ToString(txt_MenuSearch.EditValue)))
				{
					txt_MenuSearch.Text = null;
					return;
				}
				WinCommons._OpenProcessing();
				ShowForm(Function.ToString(txt_MenuSearch.EditValue));
				txt_MenuSearch.Text = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadFormWithMenu, "", txt_MenuSearch.Text);
		}
		WinCommons._CloseProcessing();
	}

	private void ShowForm(string name, string formId = "", string caption = "")
	{
		AuthsImpl formName = GlobalData.ListAccessForm.Find((AuthsImpl x) => x.MenuType != "N" && (x.menuName.ToLower().Trim() == name.ToLower().Trim() || x.menucode.ToLower().Trim() == name.ToLower().Trim()));
		if (formName != null)
		{
			if (formName.menucode.ToLower().Equals("frm_genesetting"))
			{
				menu_frm_GeneSetting.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_pluginmanager"))
			{
				menu_frm_PluginManager.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_authorization"))
			{
				menu_frm_Authorization.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_licensemanager"))
			{
				menu_frm_LicenseManager.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_users"))
			{
				menu_frm_Users.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_dashboarddesigner"))
			{
				menu_frm_DashboardDesigner.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_menustructure"))
			{
				menu_frm_MenuStructure.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().Equals("frm_listdashboard"))
			{
				menu_frm_ListDashboard.PerformClick();
				return;
			}
			if (formName.menucode.ToLower().StartsWith("frm_integrate_document"))
			{
				if (_integratedBarbutton.ContainsKey(formName.menucode))
				{
					_integratedBarbutton[formName.menucode].PerformClick();
				}
				return;
			}
			switch (formName.MenuType)
			{
			case "F":
			case "L":
			{
				Type type = ((!FormType.ContainsKey(formName.menucode)) ? AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetLoadableTypes()).FirstOrDefault((Type x) => x.Name == formName.menucode) : Type.GetType(FormType[formName.menucode]));
				if (!(type != null))
				{
					break;
				}
				XtraFormEx xtraFormEx = (XtraFormEx)Activator.CreateInstance(type);
				if (type.Name.Contains("frm_PosProcessBar"))
				{
					xtraFormEx.MdiParent = this;
					xtraFormEx.Show();
					txt_MenuSearch.EditValue = null;
					break;
				}
				xtraFormEx.MdiParent = this;
				if (!string.IsNullOrEmpty(formId))
				{
					xtraFormEx.FormId = formId;
				}
				if (!string.IsNullOrEmpty(caption))
				{
					xtraFormEx.Text = caption;
				}
				xtraFormEx.Show();
				txt_MenuSearch.EditValue = null;
				break;
			}
			case "O":
				ShowUdo(formName.menucode.Replace("UDO_", ""), string.IsNullOrEmpty(caption) ? formName.menuName : caption, formId);
				break;
			case "T":
				ShowUdt(formName.menucode.Replace("UDT_", ""), string.IsNullOrEmpty(caption) ? formName.menuName : caption, formId);
				break;
			case "R":
				ShowReportCrytal(formName.menucode.Replace("frm_", ""));
				break;
			case "C":
				ShowReportForm(formName.menucode.Replace("frm_", ""), formName.menuName);
				break;
			case "X":
				ShowReportXtra(formName.menucode.Replace("frm_", ""), "");
				break;
			case "D":
			{
				int widgetEntry = Function.ParseInt(formName.menucode);
				frm_Widget frm_Widget2 = new frm_Widget();
				frm_Widget2.Text = formName.menuName;
				frm_Widget2.WidgetEntry = widgetEntry;
				frm_Widget2.ShowProperties = false;
				frm_Widget2.MdiParent = this;
				frm_Widget2.WindowState = FormWindowState.Maximized;
				frm_Widget2.Show();
				break;
			}
			case "W":
			{
				string code = formName.menucode;
				DataRow dataRow = GlobalData.FormWizard?.AsEnumerable().FirstOrDefault((DataRow t) => code == Function.ToString(t["AbsEntry"]));
				if (dataRow == null)
				{
					break;
				}
				string text = Function.ToString(dataRow["BaseForm"]);
				if (text == "W")
				{
					frm_TemplateControlGridGrid frm_TemplateControlGridGrid2 = new frm_TemplateControlGridGrid(Function.ParseInt(dataRow["AbsEntry"]));
					frm_TemplateControlGridGrid2.Text = Function.ToString(dataRow["Caption"]);
					frm_TemplateControlGridGrid2.MdiParent = this;
					frm_TemplateControlGridGrid2.Show();
					break;
				}
				string text2 = Function.ToString(dataRow["BaseType"]);
				if (!(text2 == "O"))
				{
					if (text2 == "T")
					{
						ShowUdt(text.Replace("UDT_", ""), Function.ToString(dataRow["Caption"]), Function.ToString(dataRow["FormId"]));
					}
					else
					{
						ShowForm(text, Function.ToString(dataRow["FormId"]), Function.ToString(dataRow["Caption"]));
					}
				}
				else
				{
					ShowUdo(text.Replace("frm_", ""), Function.ToString(dataRow["Caption"]), Function.ToString(dataRow["FormId"]));
				}
				break;
			}
			}
		}
		else
		{
			WinCommons.Warning(Enums.MessageCode.FormNotFound, "");
		}
	}

	private async void ReloadAuthorization()
	{
		await LoadListAccessForm();
		HideMainBarSubItem(barSub_Module);
		HideMainBarSubItem(barSub_UserDefineWindow);
		HideMainBarSubItem(barSub_Tool);
		await Task.WhenAll(CreateMenuQuery(), CreateMenuUdt());
		SetAutoDataSearchMenu();
	}

	private void menu_frm_Dimension_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Dimension frm_Dimension2 = new frm_Dimension();
			frm_Dimension2.MdiParent = this;
			frm_Dimension2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void nav_frm_InventoryPostingList_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_InventoryPostingList frm_InventoryPostingList2 = new frm_InventoryPostingList();
			frm_InventoryPostingList2.MdiParent = this;
			frm_InventoryPostingList2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_MrpPlan_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_MrpPlan frm_MrpPlan2 = new frm_MrpPlan();
			frm_MrpPlan2.MdiParent = this;
			frm_MrpPlan2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CostCenters_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_CostCenters frm_CostCenters2 = new frm_CostCenters();
			frm_CostCenters2.MdiParent = this;
			frm_CostCenters2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_XtraReportManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ReportManager frm_ReportManager2 = new frm_ReportManager();
			frm_ReportManager2.MdiParent = this;
			frm_ReportManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GenTranferRequestFromMrp_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_GenTranferRequestFromMrp frm_GenTranferRequestFromMrp2 = new frm_GenTranferRequestFromMrp();
			frm_GenTranferRequestFromMrp2.MdiParent = this;
			frm_GenTranferRequestFromMrp2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BatchDetail_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BatchDetail frm_BatchDetail2 = new frm_BatchDetail();
			frm_BatchDetail2.MdiParent = this;
			frm_BatchDetail2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void nav_frm_StockMovementSummary_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_StockMovementSummary frm_StockMovementSummary2 = new frm_StockMovementSummary();
			frm_StockMovementSummary2.MdiParent = this;
			frm_StockMovementSummary2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SerialNumberDetail_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SerialNumberDetail frm_SerialNumberDetail2 = new frm_SerialNumberDetail();
			frm_SerialNumberDetail2.MdiParent = this;
			frm_SerialNumberDetail2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PaymentTerms_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oPaymentTermsTypes, ApzObjectTypes.oPaymentTermsTypes, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RefreshGeneralSetting()
	{
		await LoadGeneralSetting();
	}

	private async Task LoadGeneralSetting()
	{
		_ = 3;
		try
		{
			GlobalData.GeneralSetting = _httpClient.GeneralSetting.GetAllGeneralSetting();
			if (GlobalData.GeneralSetting != null)
			{
				GlobalData.ExchangeRatePow = ((!Function.ToString(GlobalData.GeneralSetting.DirectRate).Equals("N")) ? 1 : (-1));
			}
			await GetFormatString();
			await LoadCurrency();
			await LoadWeightUnits();
			await LoadLengthAndWidthUnits();
			CultureInfo cultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name);
			cultureInfo.NumberFormat.NumberDecimalSeparator = GlobalData.GeneralSetting.DecSep;
			cultureInfo.NumberFormat.NumberGroupSeparator = GlobalData.GeneralSetting.ThousSep;
			cultureInfo.NumberFormat.NumberGroupSizes = new int[1] { 3 };
			cultureInfo.NumberFormat.CurrencyGroupSizes = new int[1] { 3 };
			cultureInfo.DateTimeFormat.ShortDatePattern = GlobalData.DateTimeFormat;
			cultureInfo.DateTimeFormat.DateSeparator = GlobalData.DateSeparatorFormat;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			if (BackgroundImage == null && !string.IsNullOrEmpty(GlobalData.GeneralSetting.WallPaper) && FileIO.FileExist(GlobalData.GeneralSetting.WallPaper) && !string.IsNullOrEmpty(GlobalData.GeneralSetting.WllPprDsp))
			{
				BackgroundImage = Image.FromFile(GlobalData.GeneralSetting.WallPaper);
				switch (GlobalData.GeneralSetting.WllPprDsp)
				{
				case "1":
					BackgroundImageLayout = ImageLayout.Center;
					break;
				case "2":
					BackgroundImageLayout = ImageLayout.Stretch;
					break;
				case "3":
					BackgroundImageLayout = ImageLayout.Tile;
					break;
				default:
					BackgroundImageLayout = ImageLayout.Center;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetADM1()
	{
		try
		{
			GlobalData.CompanyDetail = await _httpClient.Commons.GetADM1();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", "GetAMD1", ex.ToString());
		}
	}

	private async Task LoadCompanyInfo()
	{
		try
		{
			GlobalData.CompanyInfo = await _httpClient.Commons.GetCompanyInfo();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_DocumentSettings_ItemClick_1(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_DocumentSettings frm_DocumentSettings2 = new frm_DocumentSettings();
			frm_DocumentSettings2.MdiParent = this;
			frm_DocumentSettings2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CycleCountDetermination_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_CycleCountDetermination frm_CycleCountDetermination2 = new frm_CycleCountDetermination();
			frm_CycleCountDetermination2.MdiParent = this;
			frm_CycleCountDetermination2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryCycles_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_InventoryCycles frm_InventoryCycles2 = new frm_InventoryCycles();
			frm_InventoryCycles2.MdiParent = this;
			frm_InventoryCycles2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CycleCountRecommendations_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_CycleCountRecommendations frm_CycleCountRecommendations2 = new frm_CycleCountRecommendations();
			frm_CycleCountRecommendations2.MdiParent = this;
			frm_CycleCountRecommendations2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BatchesControl_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == typeof(frm_BatchesControl))
				{
					form.Focus();
					return;
				}
			}
			WinCommons._OpenProcessing();
			frm_BatchesControl frm_BatchesControl2 = new frm_BatchesControl();
			frm_BatchesControl2.MdiParent = this;
			frm_BatchesControl2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frmIdvSource_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AdvertisementSource frm_AdvertisementSource2 = new frm_AdvertisementSource();
			frm_AdvertisementSource2.MdiParent = this;
			frm_AdvertisementSource2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frmJobTypes_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JobType frm_JobType2 = new frm_JobType();
			frm_JobType2.MdiParent = this;
			frm_JobType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frmQualifications_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Qualifications frm_Qualifications2 = new frm_Qualifications();
			frm_Qualifications2.MdiParent = this;
			frm_Qualifications2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frmQualificationsGroup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_QualificationsGroup frm_QualificationsGroup2 = new frm_QualificationsGroup();
			frm_QualificationsGroup2.MdiParent = this;
			frm_QualificationsGroup2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tab_MdiForm_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
	}

	private void tab_MdiForm_CloseButtonClick(object sender, EventArgs e)
	{
		try
		{
			if (e is ClosePageButtonEventArgs { Page: XtraTabPage page } && Function.ParseInt(page.Name) > 0)
			{
				CloseTabPage(page.Name);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CloseTabPage(string tab)
	{
		MdiChildren.FirstOrDefault((Form f) => f.Name == tab)?.Close();
	}

	private void tab_MdiForm_MouseClick(object sender, MouseEventArgs e)
	{
		try
		{
			if (tab_MdiForm.SelectedTabPage == null)
			{
				return;
			}
			if (e.Button.IsMiddle())
			{
				CloseTabPage(tab_MdiForm.SelectedTabPage.Name);
				return;
			}
			string nameformActive = tab_MdiForm.SelectedTabPage.Name;
			Form form = MdiChildren.FirstOrDefault((Form f) => f.Name == nameformActive);
			if (form == null)
			{
				return;
			}
			Form activeMdiChild = ActiveMdiChild;
			int num;
			if (activeMdiChild == null)
			{
				num = 0;
			}
			else
			{
				num = ((activeMdiChild.WindowState == FormWindowState.Maximized) ? 1 : 0);
				if (num != 0)
				{
					XtraFormEx.LockWindowUpdate(form.Handle);
				}
			}
			_userRaise = false;
			form.Activate();
			_userRaise = true;
			if (num != 0)
			{
				XtraFormEx.LockWindowUpdate(IntPtr.Zero);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JVQuestion_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JVQuestion frm_JVQuestion2 = new frm_JVQuestion();
			frm_JVQuestion2.MdiParent = this;
			frm_JVQuestion2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JVBenefits_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JVBenefits frm_JVBenefits2 = new frm_JVBenefits();
			frm_JVBenefits2.MdiParent = this;
			frm_JVBenefits2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JVInterviewTypes_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JVInterviewTypes frm_JVInterviewTypes2 = new frm_JVInterviewTypes();
			frm_JVInterviewTypes2.MdiParent = this;
			frm_JVInterviewTypes2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InterviewStatus_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JVInterviewStatus frm_JVInterviewStatus2 = new frm_JVInterviewStatus();
			frm_JVInterviewStatus2.MdiParent = this;
			frm_JVInterviewStatus2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JVApplicants_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_JVApplicants frm_JVApplicants2 = new frm_JVApplicants();
			frm_JVApplicants2.MdiParent = this;
			frm_JVApplicants2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_WorkType frm_WorkType2 = new frm_WorkType();
			frm_WorkType2.MdiParent = this;
			frm_WorkType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkCode_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_WorkCode frm_WorkCode2 = new frm_WorkCode();
			frm_WorkCode2.MdiParent = this;
			frm_WorkCode2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void nemu_frm_AvailableToPromise_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AvailableToPromise frm_AvailableToPromise2 = new frm_AvailableToPromise();
			frm_AvailableToPromise2.MdiParent = this;
			frm_AvailableToPromise2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButtonItem21_ItemClick_1(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ImportDataWizard frm_ImportDataWizard2 = new frm_ImportDataWizard();
			frm_ImportDataWizard2.MdiParent = this;
			frm_ImportDataWizard2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkPeriod_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_WorkPeriod frm_WorkPeriod2 = new frm_WorkPeriod();
			frm_WorkPeriod2.MdiParent = this;
			frm_WorkPeriod2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_OvertimeType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_OvertimeType frm_OvertimeType2 = new frm_OvertimeType();
			frm_OvertimeType2.MdiParent = this;
			frm_OvertimeType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GenPurchaseFromAtp_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GenPurchaseFromAtp frm_GenPurchaseFromAtp2 = new frm_GenPurchaseFromAtp();
			frm_GenPurchaseFromAtp2.MdiParent = this;
			frm_GenPurchaseFromAtp2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ScheduleForAtp_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ScheduleForAtp frm_ScheduleForAtp2 = new frm_ScheduleForAtp();
			frm_ScheduleForAtp2.MdiParent = this;
			frm_ScheduleForAtp2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_DeductionType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_DeductionType frm_DeductionType2 = new frm_DeductionType();
			frm_DeductionType2.MdiParent = this;
			frm_DeductionType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalaryParameters_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SalaryParameters frm_SalaryParameters2 = new frm_SalaryParameters();
			frm_SalaryParameters2.MdiParent = this;
			frm_SalaryParameters2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PersonalIncomeTax_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_PersonalIncomeTax frm_PersonalIncomeTax2 = new frm_PersonalIncomeTax();
			frm_PersonalIncomeTax2.MdiParent = this;
			frm_PersonalIncomeTax2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AllowanceType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AllowanceType frm_AllowanceType2 = new frm_AllowanceType();
			frm_AllowanceType2.MdiParent = this;
			frm_AllowanceType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalaryInfo_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_SalaryInfo frm_SalaryInfo2 = new frm_SalaryInfo();
			frm_SalaryInfo2.MdiParent = this;
			frm_SalaryInfo2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_GenProductionFormAtp_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_GenProductionFormAtp frm_GenProductionFormAtp2 = new frm_GenProductionFormAtp();
			frm_GenProductionFormAtp2.MdiParent = this;
			frm_GenProductionFormAtp2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_TimeSheeting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_TimeSheeting frm_TimeSheeting2 = new frm_TimeSheeting();
			frm_TimeSheeting2.MdiParent = this;
			frm_TimeSheeting2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Vehicle_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Vehicle frm_Vehicle2 = new frm_Vehicle();
			frm_Vehicle2.MdiParent = this;
			frm_Vehicle2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_VehicleType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_VehicleType frm_VehicleType2 = new frm_VehicleType();
			frm_VehicleType2.MdiParent = this;
			frm_VehicleType2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_MaterialShortageReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_MaterialShortageReport frm_MaterialShortageReport2 = new frm_MaterialShortageReport();
			frm_MaterialShortageReport2.MdiParent = this;
			frm_MaterialShortageReport2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_TimeSheetingHour_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_TimeSheetingHour frm_TimeSheetingHour2 = new frm_TimeSheetingHour();
			frm_TimeSheetingHour2.MdiParent = this;
			frm_TimeSheetingHour2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_LandedCostsSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_LandedCostsSetup frm_LandedCostsSetup2 = new frm_LandedCostsSetup();
			frm_LandedCostsSetup2.MdiParent = this;
			frm_LandedCostsSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_LandedCosts_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_LandedCosts frm_LandedCosts2 = new frm_LandedCosts();
			frm_LandedCosts2.MdiParent = this;
			frm_LandedCosts2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_ShippingDocumentLoaded_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ShippingDocumentLoaded frm_ShippingDocumentLoaded2 = new frm_ShippingDocumentLoaded();
			frm_ShippingDocumentLoaded2.MdiParent = this;
			frm_ShippingDocumentLoaded2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_ShippingDocumentPicked_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ShippingDocumentPicked frm_ShippingDocumentPicked2 = new frm_ShippingDocumentPicked();
			frm_ShippingDocumentPicked2.MdiParent = this;
			frm_ShippingDocumentPicked2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_ShippingDocumentOutboundRoute_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ShippingDocumentOutboundRoute frm_ShippingDocumentOutboundRoute2 = new frm_ShippingDocumentOutboundRoute();
			frm_ShippingDocumentOutboundRoute2.MdiParent = this;
			frm_ShippingDocumentOutboundRoute2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_GenPurchaseFromMaterialShortage_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_GenPurchaseFromMaterialShortage frm_GenPurchaseFromMaterialShortage2 = new frm_GenPurchaseFromMaterialShortage();
			frm_GenPurchaseFromMaterialShortage2.MdiParent = this;
			frm_GenPurchaseFromMaterialShortage2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_QueryPreview_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
			frm_QueryPreview2.MdiParent = this;
			frm_QueryPreview2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalaryCalculation_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_SalaryCalculation frm_SalaryCalculation2 = new frm_SalaryCalculation();
			frm_SalaryCalculation2.MdiParent = this;
			frm_SalaryCalculation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_LogViewer_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_LogViewer frm_LogViewer2 = new frm_LogViewer();
			frm_LogViewer2.MdiParent = this;
			frm_LogViewer2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_ProductionOrderImportTool_ItemPress(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ProductionOrderImportTool frm_ProductionOrderImportTool2 = new frm_ProductionOrderImportTool();
			frm_ProductionOrderImportTool2.MdiParent = this;
			frm_ProductionOrderImportTool2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void frm_UserDefinedProcedure_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_UserDefinedProcedure frm_UserDefinedProcedure2 = new frm_UserDefinedProcedure();
			frm_UserDefinedProcedure2.MdiParent = this;
			frm_UserDefinedProcedure2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void bar_ObjectsRegistrationWizard_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ORW frm_ORW2 = new frm_ORW();
			frm_ORW2.ReloadUdo = (Action)Delegate.Combine(frm_ORW2.ReloadUdo, (Action)(async () =>
			{
				await InsertFormsToAuthorizationTables();
			}));
			frm_ORW2.MdiParent = this;
			frm_ORW2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void barProcedure_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				WinCommons.Info(Enums.MessageCode.PleaseActiveAnyForm, "");
			}
			else if ((Type)popupForm.GetType() == typeof(frm_SalaryResult))
			{
				frm_SalaryFormula frm_SalaryFormula2 = new frm_SalaryFormula(popupForm, ((Type)popupForm.GetType()).Name);
				frm_SalaryFormula2.MdiParent = this;
				frm_SalaryFormula2.Branch = ((frm_SalaryResult)popupForm).Branch;
				frm_SalaryFormula2.Dept = ((frm_SalaryResult)popupForm).Dept;
				((Control)frm_SalaryFormula2).Show();
			}
			else if ((Type)popupForm.GetType() == typeof(frm_SalaryViewResult))
			{
				frm_SalaryFormula frm_SalaryFormula3 = new frm_SalaryFormula(popupForm, ((Type)popupForm.GetType()).Name);
				frm_SalaryFormula3.MdiParent = this;
				frm_SalaryFormula3.Branch = ((frm_SalaryViewResult)popupForm).Branch;
				frm_SalaryFormula3.Dept = ((frm_SalaryViewResult)popupForm).Dept;
				((Control)frm_SalaryFormula3).Show();
			}
			else if ((!popupForm.DisableLoadScript))
			{
				frm_UserDefinedProcedureCreate frm_UserDefinedProcedureCreate2 = new frm_UserDefinedProcedureCreate(popupForm, ((Type)popupForm.GetType()).Name);
				frm_UserDefinedProcedureCreate2.MdiParent = this;
				((Control)frm_UserDefinedProcedureCreate2).Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "DIY Script not available for this form.");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PluginManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PluginManager frm_PluginManager2 = new frm_PluginManager();
			frm_PluginManager2.CreateMainMenu = CreateMainMenu;
			frm_PluginManager2.MenuBar = barSub_Module;
			frm_PluginManager2.MdiParent = this;
			frm_PluginManager2.TreeMenu = treeMenu;
			frm_PluginManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private object CreateMainMenu(IPluginApplication sender, MenuCreation menu)
	{
		try
		{
			if (menu != null)
			{
				if (!string.IsNullOrEmpty(menu.ParentMenuCode))
				{
					BarSubItem parentMenu = WinFuntions.GetMenubyCode(MenuBar, menu.ParentMenuCode);
					if (parentMenu != null)
					{
						DataTable dataTable = treeMenu.DataSourceTable();
						if (!dataTable.Columns.Contains("Plugin"))
						{
							dataTable.Columns.Add("Plugin", typeof(IPluginApplication));
						}
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["MenuName"]).Equals(parentMenu.Caption));
						if (dataRow != null)
						{
							DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]).Equals($"{menu.MenuCode}@{menu.PluginId}"));
							if (dataRow2 != null)
							{
								dataRow2["FatherId"] = dataRow["Id"];
								dataRow2["Plugin"] = sender;
							}
							else
							{
								DataRow dataRow3 = dataTable.NewRow();
								dataRow3["Id"] = 22000 + menu.PluginId + menu.Position;
								dataRow3["FatherId"] = dataRow["Id"];
								dataRow3["Code"] = $"{menu.MenuCode}@{menu.PluginId}";
								dataRow3["Name"] = menu.Caption;
								dataRow3["MenuName"] = menu.Caption;
								dataRow3["MenuCode"] = menu.MenuCode;
								dataRow3["MenuType"] = "P";
								dataRow3["Plugin"] = sender;
								dataRow3["authoriza"] = "F";
								dataTable.Rows.Add(dataRow3);
							}
							dataTable.AcceptChanges();
							treeMenu.DataSource = dataTable;
							treeMenu.RefreshDataSource();
						}
						BarItemLink barItemLink = parentMenu.ItemLinks.FirstOrDefault((BarItemLink t) => t.Item.Name.Equals($"{menu.MenuCode}@{menu.PluginId}"));
						if (barItemLink != null)
						{
							parentMenu.ItemLinks.Remove(barItemLink);
						}
						switch (menu.Type)
						{
						case Enums.MenuCreationType.String:
						{
							BarButtonItem barButtonItem = new BarButtonItem();
							barButtonItem.Name = $"{menu.MenuCode}@{menu.PluginId}";
							barButtonItem.Caption = menu.Caption;
							parentMenu.AddItem(barButtonItem);
							barButtonItem.ItemPress += (object s, ItemClickEventArgs e) =>
							{
								try
								{
									MenuEvent val = new MenuEvent
									{
										MenuUid = menu.MenuCode
									};
									sender.MenuEvent(val, out var _);
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
									WinCommons.ErrorLineNumber(ex2);
								}
							};
							return barButtonItem;
						}
						case Enums.MenuCreationType.Popup:
						{
							BarSubItem barSubItem = new BarSubItem();
							barSubItem.Name = $"{menu.MenuCode}@{menu.PluginId}";
							barSubItem.Caption = menu.Caption;
							barSubItem.MenuCaption = menu.Caption;
							parentMenu.AddItem(barSubItem);
							return barSubItem;
						}
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.ParentMenuDoesNotExistsWithName, "", menu.ParentMenuCode);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ParentMenuCodeDoesNotExists, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private void BarManager_HighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
	{
		try
		{
			if (e.Link != null)
			{
				XtraFormEx.MouseOnComponent = true;
				BarItemLink link = e.Link;
				string componentInfo = ((!string.IsNullOrEmpty(link.Item.Description)) ? ("MenuId= " + link.Item.Description) : "");
				ViewComponentInfo(componentInfo);
			}
			else
			{
				XtraFormEx.MouseOnComponent = false;
				ViewComponentInfo("");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dockMain_ActiveChildChanged(object sender, DockPanelEventArgs e)
	{
	}

	private void npane_Main_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			INavigationPageBase navigationPageBase = npane_Main.CalcHitInfo(e.Location);
			if (navigationPageBase != null)
			{
				XtraFormEx.MouseOnComponent = true;
				string caption = navigationPageBase.Caption;
				ViewComponentInfo(caption);
			}
			else
			{
				XtraFormEx.MouseOnComponent = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barCheckItem1_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		try
		{
			XtraFormEx.ViewSystemInformation = barButtonItem_ViewSystemInformation.Checked;
			Form[] mdiChildren = MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form is XtraFormEx)
				{
					(form as XtraFormEx).AddViewComponentInfoEvents(form);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Packing_ItemClick(object sender, ItemClickEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_Packing frm_Packing2 = new frm_Packing();
			frm_Packing2.MdiParent = this;
			frm_Packing2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_frm_SalesOpportunity_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunity frm_SalesOpportunity2 = new frm_SalesOpportunity();
			frm_SalesOpportunity2.MdiParent = this;
			frm_SalesOpportunity2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ShipDoc_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ShipDoc frm_ShipDoc2 = new frm_ShipDoc();
			frm_ShipDoc2.MdiParent = this;
			frm_ShipDoc2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesOpportunityStage_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunityStage frm_SalesOpportunityStage2 = new frm_SalesOpportunityStage();
			frm_SalesOpportunityStage2.MdiParent = this;
			frm_SalesOpportunityStage2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesOpportunityPartner_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunityPartner frm_SalesOpportunityPartner2 = new frm_SalesOpportunityPartner();
			frm_SalesOpportunityPartner2.MdiParent = this;
			frm_SalesOpportunityPartner2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesOpportunityCompetitor_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunityCompetitor frm_SalesOpportunityCompetitor2 = new frm_SalesOpportunityCompetitor();
			frm_SalesOpportunityCompetitor2.MdiParent = this;
			frm_SalesOpportunityCompetitor2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesOpportunityRelationship_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunityRelationship frm_SalesOpportunityRelationship2 = new frm_SalesOpportunityRelationship();
			frm_SalesOpportunityRelationship2.MdiParent = this;
			frm_SalesOpportunityRelationship2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesOpportunityReason_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesOpportunityReason frm_SalesOpportunityReason2 = new frm_SalesOpportunityReason();
			frm_SalesOpportunityReason2.MdiParent = this;
			frm_SalesOpportunityReason2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Picking_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PickingDocument frm_PickingDocument2 = new frm_PickingDocument();
			frm_PickingDocument2.MdiParent = this;
			frm_PickingDocument2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PenalType_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PenalType frm_PenalType2 = new frm_PenalType();
			frm_PenalType2.MdiParent = this;
			frm_PenalType2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_EmpPenalization_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_EmpPenalization frm_EmpPenalization2 = new frm_EmpPenalization();
			frm_EmpPenalization2.MdiParent = this;
			frm_EmpPenalization2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_OutSourcing_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_OutSourcing frm_OutSourcing2 = new frm_OutSourcing();
			frm_OutSourcing2.MdiParent = this;
			frm_OutSourcing2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Task_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			try
			{
				WinCommons._OpenProcessing();
				frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
				frm_TaskManagement2.MdiParent = this;
				frm_TaskManagement2.Show();
				WinCommons._CloseProcessing();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void menu_frm_LeaveParameter_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_LeaveParameter frm_LeaveParameter2 = new frm_LeaveParameter();
			frm_LeaveParameter2.MdiParent = this;
			frm_LeaveParameter2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_DashboardManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DashboardManager frm_DashboardManager2 = new frm_DashboardManager();
			frm_DashboardManager2.MdiParent = this;
			frm_DashboardManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_DashboardViewer_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DashboardViewer frm_DashboardViewer2 = new frm_DashboardViewer();
			frm_DashboardViewer2.MdiParent = this;
			frm_DashboardViewer2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CategoriesDAB_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CategoriesDAB frm_CategoriesDAB2 = new frm_CategoriesDAB();
			frm_CategoriesDAB2.MdiParent = this;
			frm_CategoriesDAB2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_DashboardDesigner_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DashboardDesigner frm_DashboardDesigner2 = new frm_DashboardDesigner();
			frm_DashboardDesigner2.MdiParent = this;
			frm_DashboardDesigner2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CashItemMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CashItemMasterData frm_CashItemMasterData2 = new frm_CashItemMasterData();
			frm_CashItemMasterData2.MdiParent = this;
			frm_CashItemMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CouponMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CouponMasterData frm_CouponMasterData2 = new frm_CouponMasterData();
			frm_CouponMasterData2.MdiParent = this;
			frm_CouponMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_VoucherMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_VoucherMasterData frm_VoucherMasterData2 = new frm_VoucherMasterData();
			frm_VoucherMasterData2.MdiParent = this;
			frm_VoucherMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_FacebookCRM_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FacebookCRM frm_FacebookCRM2 = new frm_FacebookCRM();
			frm_FacebookCRM2.MdiParent = this;
			frm_FacebookCRM2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_NoteManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement();
			frm_NoteManagement2.MdiParent = this;
			frm_NoteManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Territory_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Territory frm_Territory2 = new frm_Territory();
			frm_Territory2.MdiParent = this;
			frm_Territory2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SalesBlanketAgreement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SalesBlanketAgreement frm_SalesBlanketAgreement2 = new frm_SalesBlanketAgreement();
			frm_SalesBlanketAgreement2.MdiParent = this;
			frm_SalesBlanketAgreement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_EventManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
			frm_EventManagement2.MdiParent = this;
			frm_EventManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_TrackingIssue_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_TrackingKanban frm_TrackingKanban2 = new frm_TrackingKanban();
			frm_TrackingKanban2.MdiParent = this;
			frm_TrackingKanban2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProjectManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ProjectManager frm_ProjectManager2 = new frm_ProjectManager();
			frm_ProjectManager2.MdiParent = this;
			frm_ProjectManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void nav_frm_BatchTransaction_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			frm_BatchTransaction frm_BatchTransaction2 = new frm_BatchTransaction();
			frm_BatchTransaction2.MdiParent = this;
			frm_BatchTransaction2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_UserDefinedTexts_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UserDefinedTexts frm_UserDefinedTexts2 = new frm_UserDefinedTexts();
			frm_UserDefinedTexts2.MdiParent = this;
			frm_UserDefinedTexts2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (e.Node.HasChildren && e.Node.ParentNode == null)
		{
			e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
		}
		if (e.Node == HotTrackNode)
		{
			e.Appearance.BackColor = Color.FromArgb(252, 185, 19);
		}
	}

	private void treeList1_MouseMove(object sender, MouseEventArgs e)
	{
		TreeListHitInfo treeListHitInfo = (sender as TreeList).CalcHitInfo(new Point(e.X, e.Y));
		HotTrackNode = ((treeListHitInfo.HitInfoType == HitInfoType.Cell) ? treeListHitInfo.Node : null);
	}

	private void treeList1_MouseLeave(object sender, EventArgs e)
	{
		HotTrackNode = null;
	}

	private void treeList1_GetStateImage(object sender, GetStateImageEventArgs e)
	{
		try
		{
			if (e.Node.HasChildren && e.Node.ParentNode == null)
			{
				string name = Function.ToString(e.Node.GetValue("Code")).Replace("barSub", "Img");
				e.NodeImageIndex = imc_Menu.Images.IndexOf(imc_Menu.Images[name]);
			}
			else if (!e.Node.HasChildren)
			{
				e.NodeImageIndex = 2;
			}
			else if (e.Node.ParentNode != null)
			{
				if (e.Node.Expanded)
				{
					e.NodeImageIndex = 1;
				}
				else
				{
					e.NodeImageIndex = 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_Click(object sender, EventArgs e)
	{
		try
		{
			TreeListNode focusedNode = treeMenu.FocusedNode;
			if (focusedNode != null && HotTrackNode != null)
			{
				if (focusedNode.Expanded)
				{
					focusedNode.Collapse();
				}
				else
				{
					if (focusedNode.ParentNode == null)
					{
						treeMenu.CollapseAll();
					}
					focusedNode.Expand();
				}
				if (!focusedNode.HasChildren)
				{
					WinCommons._OpenProcessing();
					string text = Function.ToString(focusedNode.GetValue("Code"));
					string code = Function.ToString(focusedNode.GetValue("menucode"));
					switch (Function.ToString(focusedNode.GetValue("MenuType")))
					{
					case "F":
					case "W":
					case "D":
						ShowForm(text);
						break;
					case "O":
						ShowUdo(text.Replace("frm_", ""), Function.ToString(focusedNode.GetValue("Name")));
						break;
					case "T":
						ShowUdt(text.Replace("UDT_", ""), Function.ToString(focusedNode.GetValue("Name")));
						break;
					case "R":
						ShowReportCrytal(text.Replace("frm_", ""));
						break;
					case "C":
						ShowReportForm(text.Replace("frm_", ""), Function.ToString(focusedNode.GetValue("Name")));
						break;
					case "X":
						ShowReportXtra(text.Replace("frm_", ""), "");
						break;
					case "P":
						ShowPlugin(code, focusedNode);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (ex.InnerException == null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message + ex.InnerException.Message);
				WinCommons.ErrorLineNumber(ex.InnerException);
			}
		}
		WinCommons._CloseProcessing();
	}

	private void ShowPlugin(string code, TreeListNode node)
	{
		try
		{
			MenuEvent val = new MenuEvent
			{
				MenuUid = code
			};
			if (node.GetValue("Plugin") is IPluginApplication pluginApplication)
			{
				pluginApplication.MenuEvent(val, out var _);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowUdo(string udoCode, string caption, string formId = "")
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_UDO frm_UDO2 = new frm_UDO(udoCode);
			if (!string.IsNullOrEmpty(formId))
			{
				frm_UDO2.FormId = formId;
			}
			frm_UDO2.Name = "UDO_" + udoCode;
			frm_UDO2.Text = caption;
			frm_UDO2.MdiParent = this;
			frm_UDO2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowUdt(string name, string caption, string formId = "")
	{
		try
		{
			frm_SetUpUDT frm_SetUpUDT2 = new frm_SetUpUDT(name)
			{
				Text = caption,
				MdiParent = this
			};
			if (!string.IsNullOrEmpty(formId))
			{
				frm_SetUpUDT2.FormId = formId;
			}
			frm_SetUpUDT2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryCountingBin_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_InventoryCountingBin frm_InventoryCountingBin2 = new frm_InventoryCountingBin();
			frm_InventoryCountingBin2.MdiParent = this;
			frm_InventoryCountingBin2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ContractTemplate_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ContractTemplate frm_ContractTemplate2 = new frm_ContractTemplate();
			frm_ContractTemplate2.MdiParent = this;
			frm_ContractTemplate2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_UDR_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UDR frm_UDR2 = new frm_UDR();
			frm_UDR2.RefreshMain = (RefreshMain)Delegate.Combine(frm_UDR2.RefreshMain, new RefreshMain(ReloadAuthorization));
			frm_UDR2.MdiParent = this;
			frm_UDR2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_MailSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_MailSetting frm_MailSetting2 = new frm_MailSetting();
			frm_MailSetting2.MdiParent = this;
			frm_MailSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barBtnNotification_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!string.IsNullOrEmpty(barBtnNotification.Caption))
		{
			notificationContrainerMain.Visible = !notificationContrainerMain.Visible;
		}
	}

	public async Task LoadNotification()
	{
		try
		{
			List<NotifiedObj> lstSource = new List<NotifiedObj>();
			DataTable dataTable = await _httpClient.SubActivities.GetAllNotification(GlobalData.UserID);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				NotifiedObj notifiedObj = new NotifiedObj();
				if (Function.ToString(dataTable.Rows[i]["ObjectTypes"]).Equals("1470000128"))
				{
					notifiedObj.ObjectTypes = ApzObjectTypes.oTaskManagement;
				}
				else if (Function.ToString(dataTable.Rows[i]["ObjectTypes"]).Equals("1470000129"))
				{
					notifiedObj.ObjectTypes = ApzObjectTypes.oEventManagement;
				}
				else if (Function.ToString(dataTable.Rows[i]["ObjectTypes"]).Equals("1470000130"))
				{
					notifiedObj.ObjectTypes = ApzObjectTypes.oTrackingIssue;
				}
				else if (Function.ToString(dataTable.Rows[i]["ObjectTypes"]).Equals("1200004"))
				{
					notifiedObj.ObjectTypes = ApzObjectTypes.oAlertOverview;
				}
				else if (Function.ToString(dataTable.Rows[i]["ObjectTypes"]).Equals("1470000164"))
				{
					notifiedObj.ObjectTypes = ApzObjectTypes.oWorkingProcess;
				}
				notifiedObj.DueDate = Function.ParseDateTimeMinToMaxValue(dataTable.Rows[i]["DueDate"], Function.ParseInt(dataTable.Rows[i]["DueTime"]));
				notifiedObj.DateTimeFormat = GlobalData.DateTimeFormat + " hh:mm";
				notifiedObj.Detail = Function.ToString(dataTable.Rows[i]["Details"]);
				notifiedObj.Location = Function.ToString(dataTable.Rows[i]["Location"]);
				notifiedObj.NotifiedId = Function.ToString(dataTable.Rows[i]["NotifiedId"]);
				notifiedObj.CreateUserCode = Function.ToString(dataTable.Rows[i]["CreateUserCode"]);
				notifiedObj.CreateUserName = Function.ToString(dataTable.Rows[i]["CreateUserName"]);
				notifiedObj.SumResult = Function.ParseInt(dataTable.Rows[i]["SumResult"]);
				notifiedObj.CmtCode = Function.ParseInt(dataTable.Rows[i]["CmtCode"]);
				notifiedObj.TranType = (Function.ToString(dataTable.Rows[i]["TranType"]).Equals("M") ? Enums.TransactionType.M : Enums.TransactionType.A);
				notifiedObj.RecurrenceInfo = Function.ToString(dataTable.Rows[i]["RecurrenceInfo"]);
				notifiedObj.ApptType = Function.ParseInt(dataTable.Rows[i]["CntctType"]);
				switch (Function.ToString(dataTable.Rows[i]["RemType"]))
				{
				case "M":
					notifiedObj.TimeBeforeStart = TimeSpan.FromMinutes(Function.ParseInt(dataTable.Rows[i]["RemQty"]));
					break;
				case "D":
					notifiedObj.TimeBeforeStart = TimeSpan.FromDays(Function.ParseInt(dataTable.Rows[i]["RemQty"]));
					break;
				case "H":
					notifiedObj.TimeBeforeStart = TimeSpan.FromHours(Function.ParseInt(dataTable.Rows[i]["RemQty"]));
					break;
				}
				AppointmentType apptType = (AppointmentType)notifiedObj.ApptType;
				if (apptType == AppointmentType.Pattern && !string.IsNullOrWhiteSpace(notifiedObj.RecurrenceInfo))
				{
					Appointment pattern = StaticAppointmentFactory.CreateAppointment(apptType);
					DateTime dueDate = OccurrenceCalculator.CreateInstance(RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(notifiedObj.RecurrenceInfo), TimeZoneInfo.Utc).FindNextOccurrenceTimeAfter(DateTime.Now, pattern);
					notifiedObj.DueDate = dueDate;
				}
				lstSource.Add(notifiedObj);
			}
			notificationContrainerMain.AddNotification(lstSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void HideNotificationContainerControl()
	{
		try
		{
			Point point = notificationContrainerMain.PointToClient(Cursor.Position);
			if (point.X < 0 || point.Y < 0)
			{
				notificationContrainerMain.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryPostingList_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			nav_frm_InventoryPostingList_LinkClicked(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_StockMovementSummary_ItemClick(object sender, ItemClickEventArgs e)
	{
		nav_frm_StockMovementSummary_LinkClicked(null, null);
	}

	private void menu_frm_BatchTransaction_ItemClick(object sender, ItemClickEventArgs e)
	{
		nav_frm_BatchTransaction_LinkClicked(null, null);
	}

	private void menu_frm_InventoryDraftsReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		navBarItem_DocumentDraftsReport_LinkClicked(null, null);
	}

	private void menu_frm_PurchaseDraftReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		navBarItem_DocumentDraftsReport_LinkClicked(null, null);
	}

	private void menu_frm_LeaveInformation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_LeaveInformation frm_LeaveInformation2 = new frm_LeaveInformation();
			frm_LeaveInformation2.MdiParent = this;
			frm_LeaveInformation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_LeaveCalculation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_LeaveCalculation frm_LeaveCalculation2 = new frm_LeaveCalculation();
			frm_LeaveCalculation2.MdiParent = this;
			frm_LeaveCalculation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_SaveWidget_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_SAVE();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_EditWidget_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_EDIT();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_TableLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_TABLE();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_StackLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_STACK();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_FlowLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_FLOW();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_FreeLayout_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.WIDGET_FREE();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AR_ReserveInvoices_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AR_ReserveInvoices frm_AR_ReserveInvoices2 = new frm_AR_ReserveInvoices();
			frm_AR_ReserveInvoices2.MdiParent = this;
			frm_AR_ReserveInvoices2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_NotificationSetting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_NotificationSetting frm_NotificationSetting2 = new frm_NotificationSetting();
			frm_NotificationSetting2.MdiParent = this;
			frm_NotificationSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WidgetManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WidgetManagement frm_WidgetManagement2 = new frm_WidgetManagement();
			frm_WidgetManagement2.MdiParent = this;
			frm_WidgetManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void DisConnectSocket()
	{
		CheckOnlineHub?.Dispose();
		UpdateNotiHub?.Dispose();
	}

	public void CloseAllChildForm()
	{
		try
		{
			List<Form> listChildForm = ApzonCommon.GetListChildForm();
			if (listChildForm != null)
			{
				Form[] array = listChildForm.Where((Form t) => t.MdiParent == null).ToArray();
				for (int num = 0; num < array.Length; num++)
				{
					listChildForm.Remove(array[num]);
					array[num].Close();
				}
				for (int num2 = listChildForm.Count - 1; num2 >= 0; num2--)
				{
					listChildForm[num2].Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProjectReportCompletion_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ProjectReportCompletion frm_ProjectReportCompletion2 = new frm_ProjectReportCompletion();
			frm_ProjectReportCompletion2.MdiParent = this;
			frm_ProjectReportCompletion2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ProjectConfirm_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ProjectConfirm frm_ProjectConfirm2 = new frm_ProjectConfirm();
			frm_ProjectConfirm2.MdiParent = this;
			frm_ProjectConfirm2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_JetstarPreOrder_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_JetstarPreOrder frm_JetstarPreOrder2 = new frm_JetstarPreOrder();
			frm_JetstarPreOrder2.MdiParent = this;
			frm_JetstarPreOrder2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdvanceInPayment_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AdvanceInPayment frm_AdvanceInPayment2 = new frm_AdvanceInPayment();
			frm_AdvanceInPayment2.MdiParent = this;
			frm_AdvanceInPayment2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdvanceOutPayment_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AdvanceOutPayment frm_AdvanceOutPayment2 = new frm_AdvanceOutPayment();
			frm_AdvanceOutPayment2.MdiParent = this;
			frm_AdvanceOutPayment2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AutoSelectBatchSerialConfig_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AutoSelectBatchSerialConfig frm_AutoSelectBatchSerialConfig2 = new frm_AutoSelectBatchSerialConfig();
			frm_AutoSelectBatchSerialConfig2.MdiParent = this;
			frm_AutoSelectBatchSerialConfig2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InventoryCountingRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_InventoryCountingRequest frm_InventoryCountingRequest2 = new frm_InventoryCountingRequest();
			frm_InventoryCountingRequest2.MdiParent = this;
			frm_InventoryCountingRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void npane_Main_Click(object sender, EventArgs e)
	{
	}

	private void menu_frm_InventoryCounting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_InventoryCounting frm_InventoryCounting2 = new frm_InventoryCounting();
			frm_InventoryCounting2.MdiParent = this;
			frm_InventoryCounting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_StockCounting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_StockCounting frm_StockCounting2 = new frm_StockCounting();
			frm_StockCounting2.MdiParent = this;
			frm_StockCounting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PosProcessBar_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PosProcessBar frm_PosProcessBar2 = new frm_PosProcessBar();
			frm_PosProcessBar2.MdiParent = this;
			frm_PosProcessBar2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_LayoutSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_LayoutSetup frm_LayoutSetup2 = new frm_LayoutSetup();
			frm_LayoutSetup2.MdiParent = this;
			frm_LayoutSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_CmdConfig_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CmdConfig frm_CmdConfig2 = new frm_CmdConfig();
			frm_CmdConfig2.MdiParent = this;
			frm_CmdConfig2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_OutSourcing1_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_IssueAndReceiptLackQtyForProduction frm_IssueAndReceiptLackQtyForProduction2 = new frm_IssueAndReceiptLackQtyForProduction();
			frm_IssueAndReceiptLackQtyForProduction2.MdiParent = this;
			frm_IssueAndReceiptLackQtyForProduction2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WarrantyCard_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WarrantyCard frm_WarrantyCard2 = new frm_WarrantyCard();
			frm_WarrantyCard2.MdiParent = this;
			frm_WarrantyCard2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ResourceWorkCenter_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ResourceWorkCenter frm_ResourceWorkCenter2 = new frm_ResourceWorkCenter();
			frm_ResourceWorkCenter2.MdiParent = this;
			frm_ResourceWorkCenter2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ServiceLevelAgreement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ServiceLevelAgreement frm_ServiceLevelAgreement2 = new frm_ServiceLevelAgreement();
			frm_ServiceLevelAgreement2.MdiParent = this;
			frm_ServiceLevelAgreement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ServiceContractTemplate_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ServiceContractTemplate frm_ServiceContractTemplate2 = new frm_ServiceContractTemplate();
			frm_ServiceContractTemplate2.MdiParent = this;
			frm_ServiceContractTemplate2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BatchDetailManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BatchDetailManagement frm_BatchDetailManagement2 = new frm_BatchDetailManagement();
			frm_BatchDetailManagement2.MdiParent = this;
			frm_BatchDetailManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SerialNumberDetailManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SerialNumberDetailManagement frm_SerialNumberDetailManagement2 = new frm_SerialNumberDetailManagement();
			frm_SerialNumberDetailManagement2.MdiParent = this;
			frm_SerialNumberDetailManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_Operations_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Operations frm_Operations2 = new frm_Operations();
			frm_Operations2.MdiParent = this;
			frm_Operations2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseBlanketAgreement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PurchaseBlanketAgreement frm_PurchaseBlanketAgreement2 = new frm_PurchaseBlanketAgreement();
			frm_PurchaseBlanketAgreement2.MdiParent = this;
			frm_PurchaseBlanketAgreement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdvanceRoutings_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AdvanceRouting frm_AdvanceRouting2 = new frm_AdvanceRouting();
			frm_AdvanceRouting2.MdiParent = this;
			frm_AdvanceRouting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PosMaster_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PosMaster frm_PosMaster2 = new frm_PosMaster();
			frm_PosMaster2.MdiParent = this;
			frm_PosMaster2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PosAssignment_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PosAssignment frm_PosAssignment2 = new frm_PosAssignment();
			frm_PosAssignment2.MdiParent = this;
			frm_PosAssignment2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_MarketingCampaign_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_MarketingCampaign frm_MarketingCampaign2 = new frm_MarketingCampaign();
			frm_MarketingCampaign2.MdiParent = this;
			frm_MarketingCampaign2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AlternativeItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AlternativeItems frm_AlternativeItems2 = new frm_AlternativeItems();
			frm_AlternativeItems2.MdiParent = this;
			frm_AlternativeItems2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_TemplateWizard_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_TemplateWizard frm_TemplateWizard2 = new frm_TemplateWizard();
			frm_TemplateWizard2.MdiParent = this;
			frm_TemplateWizard2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_DownPaymentRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DownPaymentRequest frm_DownPaymentRequest2 = new frm_DownPaymentRequest();
			frm_DownPaymentRequest2.MdiParent = this;
			frm_DownPaymentRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_DownPaymentInvoice_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DownPaymentInvoice frm_DownPaymentInvoice2 = new frm_DownPaymentInvoice();
			frm_DownPaymentInvoice2.MdiParent = this;
			frm_DownPaymentInvoice2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseDownPaymentRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PurchaseDownPaymentRequest frm_PurchaseDownPaymentRequest2 = new frm_PurchaseDownPaymentRequest();
			frm_PurchaseDownPaymentRequest2.MdiParent = this;
			frm_PurchaseDownPaymentRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_PurchaseDownPaymentInvoice_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PurchaseDownPaymentInvoice frm_PurchaseDownPaymentInvoice2 = new frm_PurchaseDownPaymentInvoice();
			frm_PurchaseDownPaymentInvoice2.MdiParent = this;
			frm_PurchaseDownPaymentInvoice2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_ChangeLog_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			dynamic popupForm = ApzonCommon.GetPopupForm();
			if (popupForm != null)
			{
				popupForm.CHANGELOG();
				popupForm.SetControlFormReadOnly();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ChangePassword_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ChangePassword frm_ChangePassword2 = new frm_ChangePassword();
			frm_ChangePassword2.MdiParent = this;
			frm_ChangePassword2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AssetMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AssetMasterData frm_AssetMasterData2 = new frm_AssetMasterData();
			frm_AssetMasterData2.MdiParent = this;
			frm_AssetMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ExternalRouting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ExternalRouting frm_ExternalRouting2 = new frm_ExternalRouting();
			frm_ExternalRouting2.MdiParent = this;
			frm_ExternalRouting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barItemResourceWorkScheduling_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ResourceWorkScheduling frm_ResourceWorkScheduling2 = new frm_ResourceWorkScheduling();
			frm_ResourceWorkScheduling2.MdiParent = this;
			frm_ResourceWorkScheduling2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barButton_ChangeServerAddress_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using frm_ServerAdress frm_ServerAdress2 = new frm_ServerAdress(Apzon.HttpClientHelper.Configuration.ApiServer);
			frm_ServerAdress2.ShowDialog();
			if (frm_ServerAdress2.DialogResult != DialogResult.OK)
			{
				return;
			}
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("ApiServer") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["ApiServer"].Value))
				{
					configuration.AppSettings.Settings["ApiServer"].Value = "";
					configuration.Save();
				}
			}
			System.Configuration.Configuration configuration2 = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			configuration2.AppSettings.Settings["ApiServer"].Value = frm_ServerAdress2.ApiServer;
			configuration2.Save();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.InfoRestartSystem, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK) == DialogResult.OK)
			{
				Exit = true;
				IsRestart = true;
				Program.AplicationRestart();
			}
			else
			{
				Program.AplicationExit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_BiometricData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BiometricData frm_BiometricData2 = new frm_BiometricData();
			frm_BiometricData2.MdiParent = this;
			frm_BiometricData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_TimeSheetInOut_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_TimeSheetInOut frm_TimeSheetInOut2 = new frm_TimeSheetInOut();
			frm_TimeSheetInOut2.MdiParent = this;
			frm_TimeSheetInOut2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BiometricEmpMapping_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BiometricEmpMapping frm_BiometricEmpMapping2 = new frm_BiometricEmpMapping();
			frm_BiometricEmpMapping2.MdiParent = this;
			frm_BiometricEmpMapping2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_SMSConfig_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SMSConfig frm_SMSConfig2 = new frm_SMSConfig();
			frm_SMSConfig2.MdiParent = this;
			frm_SMSConfig2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_BiometricConfiguration_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BiometricConfiguration frm_BiometricConfiguration2 = new frm_BiometricConfiguration();
			frm_BiometricConfiguration2.MdiParent = this;
			frm_BiometricConfiguration2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WBSTemplate_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WBSTemplate frm_WBSTemplate2 = new frm_WBSTemplate();
			frm_WBSTemplate2.MdiParent = this;
			frm_WBSTemplate2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ContextActionPublishService_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ContextActionPublishService frm_ContextActionPublishService2 = new frm_ContextActionPublishService();
			frm_ContextActionPublishService2.MdiParent = this;
			frm_ContextActionPublishService2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_MenuStructure_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_MenuStructure frm_MenuStructure2 = new frm_MenuStructure();
			frm_MenuStructure2.RefreshMain = (RefreshMain)Delegate.Combine(frm_MenuStructure2.RefreshMain, new RefreshMain(ReloadAuthorization));
			frm_MenuStructure2.MdiParent = this;
			frm_MenuStructure2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_HolidaySetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_HolidaySetup frm_HolidaySetup2 = new frm_HolidaySetup();
			frm_HolidaySetup2.MdiParent = this;
			frm_HolidaySetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_InOutMethod_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_InOutMethod frm_InOutMethod2 = new frm_InOutMethod();
			frm_InOutMethod2.MdiParent = this;
			frm_InOutMethod2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkShift_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WorkShift frm_WorkShift2 = new frm_WorkShift();
			frm_WorkShift2.MdiParent = this;
			frm_WorkShift2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkScheduleSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WorkScheduleSetup frm_WorkScheduleSetup2 = new frm_WorkScheduleSetup();
			frm_WorkScheduleSetup2.MdiParent = this;
			frm_WorkScheduleSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_WorkScheduleRegister_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WorkScheduleRegister frm_WorkScheduleRegister2 = new frm_WorkScheduleRegister();
			frm_WorkScheduleRegister2.MdiParent = this;
			frm_WorkScheduleRegister2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void barBarcodeInventoryCounting_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BarcodeInventoryCounting frm_BarcodeInventoryCounting2 = new frm_BarcodeInventoryCounting();
			frm_BarcodeInventoryCounting2.MdiParent = this;
			frm_BarcodeInventoryCounting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_AdditionalGeneralSettings_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using frm_AdditionalGeneralSettings frm_AdditionalGeneralSettings2 = new frm_AdditionalGeneralSettings();
			frm_AdditionalGeneralSettings2.MdiParent = this;
			frm_AdditionalGeneralSettings2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ImportFiles_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ImportFiles frm_ImportFiles2 = new frm_ImportFiles();
			frm_ImportFiles2.MdiParent = this;
			frm_ImportFiles2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_QuickCreationBPMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_QuickCreationBPMasterData frm_QuickCreationBPMasterData2 = new frm_QuickCreationBPMasterData();
			frm_QuickCreationBPMasterData2.MdiParent = this;
			frm_QuickCreationBPMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_JobsManagement_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_JobsManagement frm_JobsManagement2 = new frm_JobsManagement();
			frm_JobsManagement2.MdiParent = this;
			frm_JobsManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_Geography_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using frm_Geography frm_Geography2 = new frm_Geography();
			frm_Geography2.MdiParent = this;
			frm_Geography2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_SaleTerritory_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using frm_SalesRoute frm_SalesRoute2 = new frm_SalesRoute();
			frm_SalesRoute2.MdiParent = this;
			frm_SalesRoute2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_Store_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using frm_Store frm_Store2 = new frm_Store();
			frm_Store2.MdiParent = this;
			frm_Store2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_CreateTypeCase_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_CreateTypeCase frm_CreateTypeCase2 = new frm_CreateTypeCase();
			frm_CreateTypeCase2.MdiParent = this;
			frm_CreateTypeCase2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_PackingOrder_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PackingOrder frm_PackingOrder2 = new frm_PackingOrder();
			frm_PackingOrder2.MdiParent = this;
			frm_PackingOrder2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_PrintBillWrap_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PrintBillWrap frm_PrintBillWrap2 = new frm_PrintBillWrap();
			frm_PrintBillWrap2.MdiParent = this;
			frm_PrintBillWrap2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_Traceability_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Traceability frm_Traceability2 = new frm_Traceability();
			frm_Traceability2.MdiParent = this;
			frm_Traceability2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_WorkingProcess_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WorkingProcess frm_WorkingProcess2 = new frm_WorkingProcess();
			frm_WorkingProcess2.MdiParent = this;
			frm_WorkingProcess2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_FacebookManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FacebookManager frm_FacebookManager2 = new frm_FacebookManager();
			frm_FacebookManager2.MdiParent = this;
			frm_FacebookManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_barButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			string name = (e.Item.Caption.StartsWith("menu_") ? e.Item.Caption.Replace("menu_", "") : e.Item.Caption);
			ShowForm(name);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_QuestionMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_QuestionMasterData frm_QuestionMasterData2 = new frm_QuestionMasterData();
			frm_QuestionMasterData2.MdiParent = this;
			frm_QuestionMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_FieldActivation_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_FieldActivation frm_FieldActivation2 = new frm_FieldActivation();
			frm_FieldActivation2.MdiParent = this;
			frm_FieldActivation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_AttrCodeSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AttrCodeSetup frm_AttrCodeSetup2 = new frm_AttrCodeSetup();
			frm_AttrCodeSetup2.MdiParent = this;
			frm_AttrCodeSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_WHSubSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WHSubSetup frm_WHSubSetup2 = new frm_WHSubSetup();
			frm_WHSubSetup2.MdiParent = this;
			frm_WHSubSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_WHSubManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_WHSubManager frm_WHSubManager2 = new frm_WHSubManager();
			frm_WHSubManager2.MdiParent = this;
			frm_WHSubManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_BinLocMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BinLocMasterData frm_BinLocMasterData2 = new frm_BinLocMasterData();
			frm_BinLocMasterData2.MdiParent = this;
			frm_BinLocMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_PromotionSetup_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PromotionSetup frm_PromotionSetup2 = new frm_PromotionSetup();
			frm_PromotionSetup2.MdiParent = this;
			frm_PromotionSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_SettingReport_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SettingReport frm_SettingReport2 = new frm_SettingReport();
			frm_SettingReport2.MdiParent = this;
			frm_SettingReport2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_Channel_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Channel frm_Channel2 = new frm_Channel();
			frm_Channel2.MdiParent = this;
			frm_Channel2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_PromotionDefinition_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_PromotionDefinition frm_PromotionDefinition2 = new frm_PromotionDefinition();
			frm_PromotionDefinition2.MdiParent = this;
			frm_PromotionDefinition2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_Outlet_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_Outlet frm_Outlet2 = new frm_Outlet();
			frm_Outlet2.MdiParent = this;
			frm_Outlet2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Menu_frm_KPIsMasterData_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_KPIsMasterData frm_KPIsMasterData2 = new frm_KPIsMasterData();
			frm_KPIsMasterData2.MdiParent = this;
			frm_KPIsMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Menu_frm_SetIndicatorKPIs_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SetIndicatorKPIs frm_SetIndicatorKPIs2 = new frm_SetIndicatorKPIs();
			frm_SetIndicatorKPIs2.MdiParent = this;
			frm_SetIndicatorKPIs2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_ApprovalStages_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ApprovalStages frm_ApprovalStages2 = new frm_ApprovalStages();
			frm_ApprovalStages2.MdiParent = this;
			frm_ApprovalStages2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_ApprovalTemplates_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ApprovalTemplates frm_ApprovalTemplates2 = new frm_ApprovalTemplates();
			frm_ApprovalTemplates2.MdiParent = this;
			frm_ApprovalTemplates2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_ListPointKPIs_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ListPointKPIs frm_ListPointKPIs2 = new frm_ListPointKPIs();
			frm_ListPointKPIs2.MdiParent = this;
			frm_ListPointKPIs2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_UploadItemImage_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_UploadItemImage frm_UploadItemImage2 = new frm_UploadItemImage();
			frm_UploadItemImage2.MdiParent = this;
			frm_UploadItemImage2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Menu_frm_ImageAlbum_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ImageAlbum frm_ImageAlbum2 = new frm_ImageAlbum();
			frm_ImageAlbum2.MdiParent = this;
			frm_ImageAlbum2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_ImageViewer_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ImageViewer frm_ImageViewer2 = new frm_ImageViewer();
			frm_ImageViewer2.MdiParent = this;
			frm_ImageViewer2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Menu_frm_Marker_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_Marker frm_Marker2 = new frm_Marker();
		frm_Marker2.MdiParent = this;
		frm_Marker2.Show();
	}

	private void menu_frm_DMSAuth_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_DMSAuth frm_DMSAuth2 = new frm_DMSAuth();
		frm_DMSAuth2.RefreshMain = (RefreshMain)Delegate.Combine(frm_DMSAuth2.RefreshMain, new RefreshMain(ReloadAuthorization));
		frm_DMSAuth2.MdiParent = this;
		frm_DMSAuth2.Show();
	}

	private void menu_frm_AlertTemplate_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_AlertTemplate frm_AlertTemplate2 = new frm_AlertTemplate();
		frm_AlertTemplate2.MdiParent = this;
		frm_AlertTemplate2.Show();
	}

	private void Main_SizeChanged(object sender, EventArgs e)
	{
		try
		{
			XtraFormEx.DialogFormLocation = new Point(npane_Main.Location.X + npane_Main.ClientSize.Width + 5, npane_Main.Location.Y);
			XtraFormEx.DialogFormSize = new Size(Size.Width - npane_Main.ClientSize.Width - 30, ClientSize.Height - npane_Main.Location.Y - 15);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ReturnRequest_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_ReturnRequest frm_ReturnRequest2 = new frm_ReturnRequest();
			frm_ReturnRequest2.MdiParent = this;
			frm_ReturnRequest2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_frm_ThirdPartyToken_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_ThirdPartyToken frm_ThirdPartyToken2 = new frm_ThirdPartyToken();
		frm_ThirdPartyToken2.MdiParent = this;
		frm_ThirdPartyToken2.Show();
	}

	private void menu_frm_UserOnlineDashboard_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_UserOnlineDashboard frm_UserOnlineDashboard2 = new frm_UserOnlineDashboard();
		frm_UserOnlineDashboard2.MdiParent = this;
		frm_UserOnlineDashboard2.Show();
	}

	private void menu_frm_ItemImageViewer_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_ItemImageViewer frm_ItemImageViewer2 = new frm_ItemImageViewer();
		frm_ItemImageViewer2.MdiParent = this;
		frm_ItemImageViewer2.Show();
	}

	private void menu_frm_NotificationEventManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_NotificationEventManager frm_NotificationEventManager2 = new frm_NotificationEventManager();
		frm_NotificationEventManager2.MdiParent = this;
		frm_NotificationEventManager2.Show();
	}

	private void menu_frm_SerialNumberTransaction_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_SerialNumberTransaction frm_SerialNumberTransaction2 = new frm_SerialNumberTransaction();
		frm_SerialNumberTransaction2.MdiParent = this;
		frm_SerialNumberTransaction2.Show();
	}

	private void menu_frm_AboutRangeTime_ItemClick(object sender, ItemClickEventArgs e)
	{
		frm_AboutRangeTime frm_AboutRangeTime2 = new frm_AboutRangeTime();
		frm_AboutRangeTime2.MdiParent = this;
		frm_AboutRangeTime2.Show();
	}

	protected override void WndProc(ref System.Windows.Forms.Message m)
	{
		if (m.Msg == 161 && (int)m.WParam == 20 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmExit, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Close();
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	private void menu_frm_SettingApprovalDetail_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_SettingApprovalDetail frm_SettingApprovalDetail2 = new frm_SettingApprovalDetail();
			frm_SettingApprovalDetail2.MdiParent = this;
			frm_SettingApprovalDetail2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bar_AccessLog_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_AccessLog frm_AccessLog2 = new frm_AccessLog();
			frm_AccessLog2.MdiParent = this;
			frm_AccessLog2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void barButtonItem43_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_GoodsReturnRequest frm_GoodsReturnRequest2 = new frm_GoodsReturnRequest();
			frm_GoodsReturnRequest2.MdiParent = this;
			frm_GoodsReturnRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_DMSSettingUDF_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_DMSSettingUDF frm_DMSSettingUDF2 = new frm_DMSSettingUDF();
			frm_DMSSettingUDF2.MdiParent = this;
			frm_DMSSettingUDF2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bar_ShowFooter_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Form popupForm = ApzonCommon.GetPopupForm();
			if (popupForm == null)
			{
				return;
			}
			GridControl activeGridControl = WinFuntions.GetActiveGridControl(popupForm.ActiveControl);
			if (activeGridControl != null)
			{
				if (activeGridControl.MainView is GridViewEx gridViewEx)
				{
					gridViewEx.OptionsView.ShowFooter = !gridViewEx.OptionsView.ShowFooter;
				}
				else if (activeGridControl.MainView is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.OptionsView.ShowFooter = !bandedGridViewEx.OptionsView.ShowFooter;
				}
			}
			else
			{
				TreeList activeTreeList = WinFuntions.GetActiveTreeList(popupForm.ActiveControl);
				if (activeTreeList != null)
				{
					activeTreeList.OptionsView.ShowSummaryFooter = !activeTreeList.OptionsView.ShowSummaryFooter;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bar_ExportXlsx2_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Xlsx2);
	}

	private void bar_ExportXlsx3_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Xlsx3);
	}

	private void bar_ExportXlsx4_ItemClick(object sender, ItemClickEventArgs e)
	{
		Export(Enums.ExportExt.Xlsx4);
	}

	private async void barSub_RefreshUDF_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			if ((await _httpClient.Udf.RefreshUDF()).MessageCode == Enums.MessageCode.Success)
			{
				WinCommons.Success(Enums.MessageCode.None, "Refresh UDF successed.");
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons.Error(Enums.MessageCode.None, "Error occurred. Please try again.");
	}

	private void menu_frm_ListDashboard_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_ListDashboard frm_ListDashboard2 = frm_ListDashboard.MyDashboard();
			frm_ListDashboard2.MdiParent = this;
			frm_ListDashboard2.Show();
			Form activeMdiChild = ActiveMdiChild;
			int num;
			if (activeMdiChild == null)
			{
				num = 0;
			}
			else
			{
				num = ((activeMdiChild.WindowState == FormWindowState.Maximized) ? 1 : 0);
				if (num != 0)
				{
					XtraFormEx.LockWindowUpdate(frm_ListDashboard2.Handle);
				}
			}
			frm_ListDashboard2.Activate();
			if (num != 0)
			{
				XtraFormEx.LockWindowUpdate(IntPtr.Zero);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_frm_BiometricDataProcess_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			frm_BiometricDataProcess frm_BiometricDataProcess2 = new frm_BiometricDataProcess();
			frm_BiometricDataProcess2.MdiParent = this;
			frm_BiometricDataProcess2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void barToggle_Fontsize_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		int num = (barToggle_Fontsize.Checked ? 1 : (-1));
		AppearanceObject.DefaultFont = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + (float)num * 0.5f);
		AppearanceObject.DefaultMenuFont = new Font(AppearanceObject.DefaultMenuFont.FontFamily, AppearanceObject.DefaultMenuFont.Size + (float)num * 1.5f, AppearanceObject.DefaultMenuFont.Unit);
		treeMenu.RowHeight += num;
		SaveFontSizeConfig((num == 1) ? "L" : "N");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.Main));
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip5 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem5 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip6 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem6 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip7 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem7 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip8 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem8 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip9 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem9 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip10 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem10 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip11 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem11 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip12 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem12 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip13 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem13 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip14 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem14 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip15 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem15 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip16 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip17 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem16 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip18 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem17 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip19 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem18 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip20 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem19 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip21 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem20 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip22 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem21 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip23 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem22 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip24 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip25 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem23 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip26 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip27 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem5 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip28 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem6 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip29 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem7 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip30 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem8 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.SuperToolTip superToolTip31 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem9 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.Animation.Transition transition = new DevExpress.Utils.Animation.Transition();
		DevExpress.Utils.Animation.FadeTransition transitionType = new DevExpress.Utils.Animation.FadeTransition();
		this.BarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.ToolBar = new DevExpress.XtraBars.Bar();
		this.bar_PrintLayout = new DevExpress.XtraBars.BarButtonItem();
		this.barPrint = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportExcel = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportXlsx = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportXlsx2 = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportXlsx3 = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportXlsx4 = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportWord = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ExportPdf = new DevExpress.XtraBars.BarButtonItem();
		this.btn_ExportClipboard = new DevExpress.XtraBars.BarButtonItem();
		this.bar_Find = new DevExpress.XtraBars.BarButtonItem();
		this.bar_NewDoc = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonModeFirst = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonModePrev = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonModeNext = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonLast = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonRefreshData = new DevExpress.XtraBars.BarButtonItem();
		this.barFormSetting = new DevExpress.XtraBars.BarButtonItem();
		this.barProcedure = new DevExpress.XtraBars.BarButtonItem();
		this.bar_AutoFilterRow = new DevExpress.XtraBars.BarButtonItem();
		this.bar_BestFitColumn = new DevExpress.XtraBars.BarButtonItem();
		this.bar_Expand = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ShowFooter = new DevExpress.XtraBars.BarButtonItem();
		this.bar_DefaultBranch = new DevExpress.XtraBars.BarButtonItem();
		this.bar_Schedule = new DevExpress.XtraBars.BarButtonItem();
		this.bar_Alert = new DevExpress.XtraBars.BarButtonItem();
		this.bar_EditWidget = new DevExpress.XtraBars.BarButtonItem();
		this.bar_SaveWidget = new DevExpress.XtraBars.BarButtonItem();
		this.bar_TableLayout = new DevExpress.XtraBars.BarButtonItem();
		this.bar_StackLayout = new DevExpress.XtraBars.BarButtonItem();
		this.bar_FlowLayout = new DevExpress.XtraBars.BarButtonItem();
		this.bar_FreeLayout = new DevExpress.XtraBars.BarButtonItem();
		this.MenuBar = new DevExpress.XtraBars.Bar();
		this.barSub_System = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Preview = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PreviewLayout = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Print = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Export = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PreviewGridview = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LoginSap = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Exit = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Module = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Administration = new DevExpress.XtraBars.BarSubItem();
		this.barI_frm_LoginSap = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ExchangeRatesAndIndexs = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AssetMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Setup = new DevExpress.XtraBars.BarSubItem();
		this.barSub_SetupGeneral = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Users = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ChangePassword = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesEmployeeBuyers = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Freight = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UserDefinedTexts = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SetupAdmin = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BarcodeConfig = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_FilterSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_FtpServerSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LicenseManager = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MailSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_NotificationSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AutoSelectBatchSerialConfig = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ExternalRouting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ContextActionPublishService = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JobsManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GeneSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CompanyDetails = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Territory = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SetupBp = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Countries = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CustomerGroup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_VendorGroup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BusinessPartnerProperties = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BusinessPartnerPriorities = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PaymentTerms = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SetupBanking = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_CreditCardSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CreditPaymentMethods = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Financial = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Braches = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CostCenters = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Currencies = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Dimension = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DistributeRule = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TaxGroups = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Project = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Sales = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Purcharsing = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_LandedCostsSetup = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SetupInventory = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ItemGroups = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ItemProperties = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WareHouses = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UnitOfMeasure = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UnitOfMeasureGroups = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Shipping = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LengthAndWidth = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Weight = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CustomsGroup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Manufacturer = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Locations = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CycleCountDetermination = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryCycles = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Vehicle = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_VehicleType = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BinLocationSetup = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_FieldActivation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AttrCodeSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WHSubSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WHSubManager = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SetupHumanResouces = new DevExpress.XtraBars.BarSubItem();
		this.barSubItem2 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_QualificationsGroup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Qualifications = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JobType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvertisementSource = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JVApplicants = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JVInterviewStatus = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JVInterviewTypes = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JVQuestion = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JVBenefits = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem4 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_WorkType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkCode = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkPeriod = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_OvertimeType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_HolidaySetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InOutMethod = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkShift = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkScheduleSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkScheduleRegister = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AboutRangeTime = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem6 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_AllowanceType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DeductionType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PersonalIncomeTax = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalaryParameters = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LeaveParameter = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PenalType = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BiometricConfiguration = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SetupReport = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PrintLayout = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ReportManager = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Opportunities = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesOpportunityStage = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesOpportunityPartner = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesOpportunityCompetitor = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesOpportunityRelationship = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesOpportunityReason = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ContractTemplate = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ProjectManagement = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_WBSTemplate = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdditionalGeneralSettings = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ThirdPartyToken = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Authorization = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Authorization = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DataOwnerShipAuthorization = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SystemInitialization = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_DocumentNumbering = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DocumentSettings = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MenuStructure = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ApprovalProcess = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ApprovalStages = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ApprovalTemplates = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WorkingProcess = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_EmpToken = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PlugIn = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PluginManager = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AccessLog = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Dashboard = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_DashboardManager = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DashboardDesigner = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_WidgetManagement = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SalesAR = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesBlanketAgreement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesQuotationForBatch = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesOrder = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesControl = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DeliveryControl = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Delivery = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ReturnRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Return = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DownPaymentRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DownPaymentInvoice = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AR_ReserveInvoices = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ARInvoices = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ARCreditMemos = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Atp = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DeliveryOrder = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DeliveryConfirm = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_HandoverDocument = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AvailableToPromise = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenPurchaseFromAtp = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ScheduleForAtp = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenProductionFormAtp = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_SalesReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesDraftsReport = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PurchaseAP = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PurchaseBlanketAgreement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PurchaseRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PurchaseQuotation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PurchaseOder = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GoodReceiptPO = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GoodsReceiptPOByBarcode = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GoodsReturnRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GoodsReturn = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PurchaseDownPaymentRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PurchaseDownPaymentInvoice = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_APInvoices = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_APCreditMemos = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_APReserveInvoices = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LandedCosts = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProcurementConfirm = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PurchasingReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PurchaseDraftReport = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BP = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_BusinessMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ActivitiesForOpportunity = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_FacebookCRM = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BPReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_QuickCreationBPMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_FacebookManager = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Inventory = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ItemMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_InventoryTransaction = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_GoodsReceipt = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ReceiptBin = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GoodsIssue = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryTranferRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryTranfer = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryTransferByBarcode = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryCountingBin = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryRevaluation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_StockRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PickAndPack = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryCountingRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryCounting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BarcodeInventoryCounting = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ItemManagement = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_BatchDetail = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SerialNumberDetail = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BatchesControl = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BatchDetailManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SerialNumberDetailManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AlternativeItems = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BusinessPartnerCatalogNumbers = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PriceLists = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_STDPriceLists = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PeriodAndVolumeDiscounts = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DiscountGroup = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem13 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SpecialforBP = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PickAndPack = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PickingRequest = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PickingDocument = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Packing = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ShipDoc = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PackingByBarcode = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_InventoryReport = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_InventoryDraftsReport = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_InventoryPostingList = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_StockMovementSummary = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BatchTransaction = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SerialNumberTransaction = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BinLocation = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_BinLocMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Production = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_BillOfMaterials = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvanceBillOfMaterials = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProductionOrder = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProductionOrderImportTool = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ReceiptFromProduction = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ReceiptFromPOByBarcode = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_IssueForProduction = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MaterialShortageReport = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenPurchaseFromMaterialShortage = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_OutSourcing = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ProductionReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_IssueAndReceiptLackQtyForProduction = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_MTO = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ProductionReqForBatch = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProductionForBatch = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProductionConfirm = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PackingBatch = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_MRP = new DevExpress.XtraBars.BarSubItem();
		this.menu_FrmDefinedShift = new DevExpress.XtraBars.BarButtonItem();
		this.menu_FrmWorkScheduling = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Lines = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ItemQuotaArrangement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvanceForecast = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MRP = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Schedule = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenPOFromSchedule = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PrintBarCode = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AutoReportComplete = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenPurchaseFromSchedule = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MrpPlan = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_GenTranferRequestFromMrp = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_MRPReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_JetstarPreOrder = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_RoutingResources = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Operation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Resources = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ResourcesReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ResourceWorkCenter = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Operations = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvanceRouting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ResourceWorkScheduling = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PackingManagement = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_CreateTypeCase = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PackingOrder = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PrintBillWrap = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Traceability = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Service = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesOpportunity = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Solution = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ServiceCalls = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Contract = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ServiceCallSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TaskManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_NoteManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_EventManagement = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_OpportunitiesReport = new DevExpress.XtraBars.BarSubItem();
		this.barSub_ServicesReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_WarrantyCard = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ServiceLevelAgreement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ServiceContractTemplate = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_MarketingCampaign = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BankingPayment = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PaymentForInvoice = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CashItemMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CouponMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_VoucherMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_BankingReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_AdvanceInPayment = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvanceOutPayment = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_IncomingPaymentWizard = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdvancePaymentWizard = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Retail = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PosProcessBar = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LayoutSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_CmdConfig = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_RetailStore = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PosMaster = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PosAssignment = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_HRM = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_HumanResources = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_JobVacancy = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalaryInfo = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TimeSheeting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TimeSheetingHour = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalaryCalculation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_EmpPenalization = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_LeaveInformation = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_HRMReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_LeaveCalculation = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem9 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_BiometricData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TimeSheetInOut = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BiometricEmpMapping = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_BiometricDataProcess = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PM = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ProjectManager = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TrackingIssue = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_ProjectReports = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ProjectReportCompletion = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ProjectConfirm = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_DMS = new DevExpress.XtraBars.BarSubItem();
		this.barSubItem10 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SettingReport = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SettingApprovalDetail = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DMSSettingUDF = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DMSAuth = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ImageAlbum = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_QuestionMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Marker = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AlertTemplate = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_NotificationEventManager = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Danhmuc = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_Geography = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_Channel = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UploadItemImage = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ImageViewer = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UserOnlineDashboard = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ItemImageViewer = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_AdminUploadAttachment = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SalesRoute = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem15 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_PromotionSetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PromotionCondition = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PromotionBenefit = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PromotionProgram = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem16 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_OutletVisitingFrequency = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem17 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesCoverageGoogleMap = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_DMSDelivery = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_KPI = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_KPIsMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_SetIndicatorKPIs = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIsSetting = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ListPointKPIs = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIPlan = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIScope = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIIndicator = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIGroup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPISetup = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_KPIScore = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_UDO = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Reports = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Integrated = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Configuration = new DevExpress.XtraBars.BarSubItem();
		this.barToggle_Fontsize = new DevExpress.XtraBars.BarToggleSwitchItem();
		this.barSub_Language = new DevExpress.XtraBars.BarSubItem();
		this.barSub_LanguageEn = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_LanguageVn = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Tool = new DevExpress.XtraBars.BarSubItem();
		this.barSub_Queries = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ManagerQuery = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_QueryGenerator = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_QueryPreview = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_CustomizationTools = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_UserDefineFieldsManagement = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UDTManager = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ORW = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UserDefinedProcedure = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_UDR = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_TemplateWizard = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_UserDefineWindow = new DevExpress.XtraBars.BarSubItem();
		this.barSub_ImportData = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ImportDataWizard = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ImportFiles = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ChangeLogList = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Window = new DevExpress.XtraBars.BarSubItem();
		this.barSub_SystemMessageLog = new DevExpress.XtraBars.BarCheckItem();
		this.barSub_Help = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem_CheckForUpdate = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem_UpdateReport = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_RefreshUDF = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem_ViewSystemInformation = new DevExpress.XtraBars.BarCheckItem();
		this.barButton_ChangeServerAddress = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_PreviewExternalReport = new DevExpress.XtraBars.BarButtonItem();
		this.bar3 = new DevExpress.XtraBars.Bar();
		this.txt_MsgBar = new DevExpress.XtraBars.BarStaticItem();
		this.bar_User = new DevExpress.XtraBars.BarStaticItem();
		this.bar_ProgessControl = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.barBtnNotification = new DevExpress.XtraBars.BarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.dockMain = new DevExpress.XtraBars.Docking.DockManager(this.components);
		this.dockMessageLog = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.grd_SystemMessageLog = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SystemMessageLog = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.imc_Data = new DevExpress.Utils.ImageCollection(this.components);
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.iml_Column = new System.Windows.Forms.ImageList(this.components);
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.imc_Menu = new DevExpress.Utils.ImageCollection(this.components);
		this.barSubItem3 = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
		this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
		this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
		this.barButtonItem14 = new DevExpress.XtraBars.BarButtonItem();
		this.skinBarSubItem1 = new DevExpress.XtraBars.SkinBarSubItem();
		this.barButtonItem15 = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem7 = new DevExpress.XtraBars.BarSubItem();
		this.barSubItem11 = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem7 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem8 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem9 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem10 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem11 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem12 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem16 = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem21 = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem17 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem18 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem19 = new DevExpress.XtraBars.BarButtonItem();
		this.skinBarSubItem2 = new DevExpress.XtraBars.SkinBarSubItem();
		this.barSubItem22 = new DevExpress.XtraBars.BarSubItem();
		this.barToolbars_CheckForUpdate = new DevExpress.XtraBars.BarToolbarsListItem();
		this.barSubItem8 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_SalesQuotation = new DevExpress.XtraBars.BarButtonItem();
		this.menu_AR_ReserveInvoices = new DevExpress.XtraBars.BarButtonItem();
		this.barStaticItem7 = new DevExpress.XtraBars.BarStaticItem();
		this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem23 = new DevExpress.XtraBars.BarSubItem();
		this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barHeaderItem1 = new DevExpress.XtraBars.BarHeaderItem();
		this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barDockingMenuItem1 = new DevExpress.XtraBars.BarDockingMenuItem();
		this.barEditItem3 = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barButtonItem_QueryTemplateRpt = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem_QueryManagerRpt = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem5 = new DevExpress.XtraBars.BarSubItem();
		this.barSubItem_QueryTemplateReport = new DevExpress.XtraBars.BarSubItem();
		this.barSubItem_QueryManagerReport = new DevExpress.XtraBars.BarSubItem();
		this.barEditItem4 = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barStaticItem12 = new DevExpress.XtraBars.BarStaticItem();
		this.barSubItem24 = new DevExpress.XtraBars.BarSubItem();
		this.barEditItem5 = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem28 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem34 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem38 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem39 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem40 = new DevExpress.XtraBars.BarButtonItem();
		this.barI_frm_Shipping = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem56 = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem29 = new DevExpress.XtraBars.BarSubItem();
		this.bsi_Cus = new DevExpress.XtraBars.BarLinkContainerItem();
		this.barButtonItem61 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem62 = new DevExpress.XtraBars.BarButtonItem();
		this.bar_ItemMasterData = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem63 = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Banking = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem22 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem23 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem24 = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Formula = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem25 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem26 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem27 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem29 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem30 = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Financials = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem31 = new DevExpress.XtraBars.BarButtonItem();
		this.barSubItem18 = new DevExpress.XtraBars.BarSubItem();
		this.menu_frm_ShippingDocumentPicked = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ShippingDocumentLoaded = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_ShippingDocumentOutboundRoute = new DevExpress.XtraBars.BarButtonItem();
		this.bar_LogViewer = new DevExpress.XtraBars.BarCheckItem();
		this.menu_frm_LogViewer = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem32 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem21 = new DevExpress.XtraBars.BarButtonItem();
		this.barSub_Opportunity = new DevExpress.XtraBars.BarSubItem();
		this.barButtonItem33 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem20 = new DevExpress.XtraBars.BarButtonItem();
		this.menu = new DevExpress.XtraBars.BarButtonItem();
		this.menu_frm_PackingManagement = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem35 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem36 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem37 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem42 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem41 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem50 = new DevExpress.XtraBars.BarButtonItem();
		this.repositoryItemSearchControl1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchControl();
		this.repositoryItemTextEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.nav_frm_DocumentDraftsRP1 = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_DocumentDraftsRP2 = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_DocumentDraftsRP3 = new DevExpress.XtraNavBar.NavBarItem();
		this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
		this.TimerStaticStatusBar = new System.Windows.Forms.Timer(this.components);
		this.bar4 = new DevExpress.XtraBars.Bar();
		this.barSubItem14 = new DevExpress.XtraBars.BarSubItem();
		this.nav_frm_ListScheduling = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_POProcess = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_ReportBarCode = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_BarCode = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_BarCodeStandard = new DevExpress.XtraNavBar.NavBarItem();
		this.timerLogin = new System.Windows.Forms.Timer(this.components);
		this.navBarGroup13 = new DevExpress.XtraNavBar.NavBarGroup();
		this.alertMain = new Apzon.Controls.Controls.AlertControlEx(this.components);
		this.timer_Alert = new System.Windows.Forms.Timer(this.components);
		this.barStaticItem8 = new DevExpress.XtraBars.BarStaticItem();
		this.barStaticItem9 = new DevExpress.XtraBars.BarStaticItem();
		this.barStaticItem10 = new DevExpress.XtraBars.BarStaticItem();
		this.txt_MenuSearch = new Apzon.Forms.UserControls.AutoCompleteTextBox();
		this.npane_Main = new DevExpress.XtraBars.Navigation.NavigationPane();
		this.npage_Operations = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.treeMenu = new DevExpress.XtraTreeList.TreeList();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.navGroupReport_Sales = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_Administration = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_Purchase = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_BusinessPartner = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_Inventory = new DevExpress.XtraNavBar.NavBarGroup();
		this.nav_frm_InventoryPostingList = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_StockMovementSummary = new DevExpress.XtraNavBar.NavBarItem();
		this.nav_frm_BatchTransaction = new DevExpress.XtraNavBar.NavBarItem();
		this.navGroupReport_Production = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_MRP = new DevExpress.XtraNavBar.NavBarGroup();
		this.navGroupReport_HRM = new DevExpress.XtraNavBar.NavBarGroup();
		this.tab_MdiForm = new Apzon.Controls.Controls.XtraTabControlEx();
		this.transitionManager1 = new DevExpress.Utils.Animation.TransitionManager(this.components);
		this.notificationContrainerMain = new Apzon.Forms.UserControls.Notification.NotificationContrainer();
		this.adornerUIManager1 = new DevExpress.Utils.VisualEffects.AdornerUIManager(this.components);
		this.guide1 = new DevExpress.Utils.VisualEffects.Guide();
		this.badge1 = new DevExpress.Utils.VisualEffects.Badge();
		this.menu_frm_ListDashboard = new DevExpress.XtraBars.BarButtonItem();
		((System.ComponentModel.ISupportInitialize)this.BarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemProgressBar1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dockMain).BeginInit();
		this.dockMessageLog.SuspendLayout();
		this.dockPanel1_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SystemMessageLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SystemMessageLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imc_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imc_Menu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.npane_Main).BeginInit();
		this.npane_Main.SuspendLayout();
		this.npage_Operations.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.treeMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_MdiForm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.adornerUIManager1).BeginInit();
		base.SuspendLayout();
		this.BarManager.AllowCustomization = false;
		this.BarManager.AllowQuickCustomization = false;
		this.BarManager.AllowShowToolbarsPopup = false;
		this.BarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[3] { this.ToolBar, this.MenuBar, this.bar3 });
		this.BarManager.DockControls.Add(this.barDockControlTop);
		this.BarManager.DockControls.Add(this.barDockControlBottom);
		this.BarManager.DockControls.Add(this.barDockControlLeft);
		this.BarManager.DockControls.Add(this.barDockControlRight);
		this.BarManager.DockManager = this.dockMain;
		this.BarManager.Form = this;
		this.BarManager.Images = this.imc_Menu;
		this.BarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[536]
		{
			this.barSub_System, this.barSubItem3, this.barSub_Module, this.barSub_Tool, this.barSub_Help, this.barButtonItem1, this.barButtonItem2, this.barButtonItem3, this.barStaticItem1, this.bar_PrintLayout,
			this.barPrint, this.barStaticItem2, this.bar_Find, this.bar_NewDoc, this.barButtonModeFirst, this.barButtonModePrev, this.barButtonModeNext, this.barButtonLast, this.barButtonItem14, this.skinBarSubItem1,
			this.txt_MsgBar, this.barButtonItem15, this.barSubItem7, this.barSubItem11, this.barButtonItem7, this.barFormSetting, this.barButtonItem8, this.barButtonItem9, this.barButtonItem10, this.barButtonItem11,
			this.barButtonItem12, this.barButtonItem16, this.barSub_Administration, this.barSub_SalesAR, this.barSub_PurchaseAP, this.barSub_BP, this.barSub_Inventory, this.barSub_Reports, this.barSubItem21, this.barButtonItem17,
			this.barButtonItem18, this.barButtonItem19, this.skinBarSubItem2, this.barSubItem22, this.barToolbars_CheckForUpdate, this.barButtonItem_CheckForUpdate, this.barSub_Language, this.barSub_LanguageEn, this.barSub_LanguageVn, this.barSubItem8,
			this.barSub_Exit, this.barSub_Preview, this.barSub_PreviewLayout, this.barSub_Print, this.bar_User, this.barSub_Queries, this.menu_frm_QueryGenerator, this.menu_frm_BusinessMasterData, this.menu_frm_ActivitiesForOpportunity, this.menu_frm_SalesQuotationForBatch,
			this.menu_frm_SalesQuotation, this.menu_frm_SalesOrder, this.menu_frm_SalesDraftsReport, this.menu_frm_Delivery, this.menu_frm_Return, this.menu_frm_ARInvoices, this.menu_frm_ARCreditMemos, this.menu_AR_ReserveInvoices, this.menu_frm_Atp, this.bar_ExportExcel,
			this.bar_ExportWord, this.bar_ExportPdf, this.barStaticItem7, this.barButtonItem4, this.barSubItem23, this.barEditItem1, this.barHeaderItem1, this.barEditItem2, this.barDockingMenuItem1, this.barEditItem3,
			this.barSub_Export, this.barSub_PreviewGridview, this.barButtonItem_QueryTemplateRpt, this.barButtonItem_QueryManagerRpt, this.barSubItem5, this.barSubItem_QueryTemplateReport, this.barSubItem_QueryManagerReport, this.menu_frm_ManagerQuery, this.bar_AutoFilterRow, this.bar_BestFitColumn,
			this.barEditItem4, this.barStaticItem12, this.bar_DefaultBranch, this.barSub_Integrated, this.barSubItem24, this.bar_Expand, this.btn_ExportClipboard, this.barEditItem5, this.barButtonItem6, this.barSub_Setup,
			this.barSub_SetupGeneral, this.menu_frm_Users, this.menu_frm_SalesEmployeeBuyers, this.menu_frm_Freight, this.barI_frm_LoginSap, this.menu_frm_ExchangeRatesAndIndexs, this.menu_frm_FtpServerSetting, this.barButtonItem28, this.menu_frm_GeneSetting, this.barSub_Authorization,
			this.menu_frm_Authorization, this.menu_frm_DataOwnerShipAuthorization, this.menu_frm_FilterSetting, this.barButtonItem34, this.barSub_SetupBp, this.barSub_SetupInventory, this.menu_frm_CustomerGroup, this.menu_frm_VendorGroup, this.barButtonItem38, this.barButtonItem39,
			this.barButtonItem40, this.barSub_SetupReport, this.menu_frm_PrintLayout, this.menu_frm_BusinessPartnerProperties, this.menu_frm_BusinessPartnerPriorities, this.menu_frm_ItemGroups, this.menu_frm_ItemProperties, this.menu_frm_WareHouses, this.menu_frm_UnitOfMeasure, this.menu_frm_UnitOfMeasureGroups,
			this.menu_frm_LengthAndWidth, this.menu_frm_Weight, this.menu_frm_CustomsGroup, this.menu_frm_Manufacturer, this.barI_frm_Shipping, this.menu_frm_Locations, this.menu_frm_Shipping, this.barButtonItem56, this.barSubItem29, this.barSub_Financial,
			this.menu_frm_Braches, this.menu_frm_Currencies, this.menu_frm_TaxGroups, this.bsi_Cus, this.barSub_CustomizationTools, this.menu_frm_UserDefineFieldsManagement, this.menu_frm_UDTManager, this.barSub_UserDefineWindow, this.barButtonItem61, this.barSub_ImportData,
			this.barButtonItem62, this.bar_ItemMasterData, this.barButtonItem63, this.menu_frm_PurchaseRequest, this.menu_frm_PurchaseQuotation, this.menu_frm_PurchaseOder, this.menu_frm_GoodReceiptPO, this.menu_frm_GoodsReturn, this.menu_frm_APInvoices, this.menu_frm_APCreditMemos,
			this.menu_frm_APReserveInvoices, this.menu_frm_LandedCosts, this.menu_frm_ProcurementConfirm, this.menu_frm_ItemMasterData, this.barSub_InventoryTransaction, this.menu_frm_GoodsReceipt, this.menu_frm_GoodsIssue, this.menu_frm_InventoryTranferRequest, this.menu_frm_InventoryTranfer, this.menu_frm_InventoryRevaluation,
			this.barSub_ItemManagement, this.barSub_MRP, this.menu_frm_MRP, this.menu_frm_Schedule, this.menu_frm_GenPOFromSchedule, this.menu_frm_PrintBarCode, this.menu_frm_AutoReportComplete, this.barSub_Service, this.menu_frm_Solution, this.menu_frm_ServiceCalls,
			this.menu_frm_Contract, this.menu_frm_ServiceCallSetting, this.barSub_HRM, this.barSub_RoutingResources, this.menu_frm_Operation, this.menu_frm_Resources, this.barSub_Configuration, this.menu_frm_CompanyDetails, this.menu_frm_LoginSap, this.menu_frm_HumanResources,
			this.barSub_Window, this.barSub_SystemInitialization, this.menu_frm_DocumentNumbering, this.barSub_Banking, this.barButtonItem22, this.barSub_BankingPayment, this.menu_frm_PaymentForInvoice, this.barButtonItem23, this.barButtonItem24, this.barSub_Production,
			this.menu_frm_BillOfMaterials, this.menu_frm_ProductionOrder, this.barSub_MTO, this.menu_frm_ProductionReqForBatch, this.menu_frm_ProductionForBatch, this.menu_frm_ProductionConfirm, this.menu_frm_PackingBatch, this.menu_frm_StockRequest, this.menu_frm_Project, this.barSub_InventoryReport,
			this.menu_frm_BusinessPartnerCatalogNumbers, this.barSub_Formula, this.menu_frm_AdvanceForecast, this.menu_FrmDefinedShift, this.menu_FrmWorkScheduling, this.menu_frm_Lines, this.menu_frm_ItemQuotaArrangement, this.barButtonRefreshData, this.menu_frm_BarcodeConfig, this.menu_frm_AdvanceBillOfMaterials,
			this.menu_frm_GenPurchaseFromSchedule, this.barSub_Dashboard, this.bar_Schedule, this.menu_frm_SetupAdmin, this.menu_frm_DeliveryOrder, this.menu_frm_DeliveryConfirm, this.menu_frm_HandoverDocument, this.menu_frm_EmpToken, this.menu_frm_SalesControl, this.menu_frm_DeliveryControl,
			this.menu_frm_ReceiptBin, this.menu_frm_PickAndPack, this.barButtonItem_UpdateReport, this.bar_Alert, this.barSub_PriceLists, this.menu_frm_STDPriceLists, this.menu_frm_PeriodAndVolumeDiscounts, this.barSubItem13, this.menu_frm_SpecialforBP, this.menu_frm_DiscountGroup,
			this.barSub_SystemMessageLog, this.menu_frm_Countries, this.barButtonItem25, this.barButtonItem26, this.barButtonItem27, this.barButtonItem29, this.barSub_SetupBanking, this.menu_frm_CreditCardSetup, this.menu_frm_CreditPaymentMethods, this.menu_frm_ReceiptFromProduction,
			this.menu_frm_IssueForProduction, this.menu_frm_RetailStore, this.barButtonItem30, this.barSub_Financials, this.menu_frm_DistributeRule, this.menu_frm_Dimension, this.menu_frm_MrpPlan, this.menu_frm_LicenseManager, this.menu_frm_CostCenters, this.menu_frm_ReportManager,
			this.menu_frm_BatchDetail, this.menu_frm_GenTranferRequestFromMrp, this.menu_frm_SerialNumberDetail, this.menu_frm_AR_ReserveInvoices, this.menu_frm_PaymentTerms, this.menu_frm_DocumentSettings, this.menu_frm_CycleCountDetermination, this.menu_frm_InventoryCycles, this.menu_frm_BatchesControl, this.menu_frm_JobVacancy,
			this.barSub_SetupHumanResouces, this.barSubItem2, this.menu_frm_QualificationsGroup, this.menu_frm_Qualifications, this.menu_frm_JobType, this.menu_frm_AdvertisementSource, this.menu_frm_JVApplicants, this.menu_frm_JVInterviewStatus, this.menu_frm_JVInterviewTypes, this.menu_frm_JVQuestion,
			this.menu_frm_JVBenefits, this.barSubItem4, this.menu_frm_WorkType, this.menu_frm_WorkCode, this.menu_frm_AvailableToPromise, this.menu_frm_ImportDataWizard, this.menu_frm_WorkPeriod, this.menu_frm_OvertimeType, this.menu_frm_GenPurchaseFromAtp, this.barSubItem6,
			this.menu_frm_AllowanceType, this.menu_frm_DeductionType, this.menu_frm_PersonalIncomeTax, this.menu_frm_SalaryParameters, this.menu_frm_SalaryInfo, this.menu_frm_ScheduleForAtp, this.menu_frm_GenProductionFormAtp, this.menu_frm_TimeSheeting, this.menu_frm_MaterialShortageReport, this.barSub_Sales,
			this.menu_frm_Vehicle, this.menu_frm_VehicleType, this.menu_frm_PickingRequest, this.menu_frm_TimeSheetingHour, this.barButtonItem31, this.barSub_Purcharsing, this.menu_frm_LandedCostsSetup, this.barSubItem18, this.menu_frm_ShippingDocumentPicked, this.menu_frm_ShippingDocumentLoaded,
			this.menu_frm_ShippingDocumentOutboundRoute, this.menu_frm_QueryPreview, this.menu_frm_GenPurchaseFromMaterialShortage, this.menu_frm_SalaryCalculation, this.bar_LogViewer, this.menu_frm_LogViewer, this.barButtonItem32, this.menu_frm_ProductionOrderImportTool, this.menu_frm_UserDefinedProcedure, this.menu_frm_ORW,
			this.barProcedure, this.barSub_PlugIn, this.menu_frm_PluginManager, this.barButtonItem21, this.barButtonItem_ViewSystemInformation, this.menu_frm_SalesOpportunity, this.barSub_Opportunity, this.barSub_PickAndPack, this.menu_frm_Packing, this.menu_frm_PickingDocument,
			this.menu_frm_ShipDoc, this.barButtonItem33, this.barSub_Opportunities, this.menu_frm_SalesOpportunityStage, this.menu_frm_SalesOpportunityPartner, this.menu_frm_SalesOpportunityCompetitor, this.menu_frm_SalesOpportunityRelationship, this.menu_frm_SalesOpportunityReason, this.menu_frm_PenalType, this.menu_frm_EmpPenalization,
			this.menu_frm_OutSourcing, this.menu_frm_DashboardManager, this.menu_frm_TaskManagement, this.menu_frm_DashboardDesigner, this.menu_frm_LeaveParameter, this.menu_frm_CashItemMasterData, this.menu_frm_CouponMasterData, this.menu_frm_VoucherMasterData, this.menu_frm_FacebookCRM, this.menu_frm_NoteManagement,
			this.menu_frm_Territory, this.menu_frm_SalesBlanketAgreement, this.menu_frm_EventManagement, this.barSub_PM, this.menu_frm_TrackingIssue, this.menu_frm_UserDefinedTexts, this.barSub_UDO, this.menu_frm_ProjectManager, this.menu_frm_InventoryCountingBin, this.barSubItem1,
			this.menu_frm_ContractTemplate, this.menu_frm_UDR, this.bar_ProgessControl, this.menu_frm_MailSetting, this.barSub_OpportunitiesReport, this.barSub_SalesReports, this.barSub_PurchasingReports, this.barSub_BPReports, this.barSub_ProductionReports, this.barSub_MRPReports,
			this.barSub_ServicesReports, this.barSub_BankingReports, this.barSub_ResourcesReports, this.barSub_HRMReports, this.barSub_ProjectReports, this.menu_frm_InventoryPostingList, this.menu_frm_StockMovementSummary, this.menu_frm_BatchTransaction, this.menu_frm_SerialNumberTransaction, this.menu_frm_PurchaseDraftReport,
			this.menu_frm_InventoryDraftsReport, this.barBtnNotification, this.bar_EditWidget, this.bar_TableLayout, this.bar_StackLayout, this.bar_FlowLayout, this.bar_FreeLayout, this.menu_frm_LeaveInformation, this.menu_frm_LeaveCalculation, this.bar_SaveWidget,
			this.menu_frm_NotificationSetting, this.menu_frm_WidgetManagement, this.menu_frm_ProjectReportCompletion, this.menu_frm_ProjectConfirm, this.menu_frm_JetstarPreOrder, this.menu_frm_AdvanceInPayment, this.menu_frm_AdvanceOutPayment, this.menu_frm_AutoSelectBatchSerialConfig, this.menu_frm_InventoryCountingRequest, this.menu_frm_InventoryCounting,
			this.menu_frm_PosProcessBar, this.menu_frm_LayoutSetup, this.barSub_Retail, this.menu_frm_CmdConfig, this.menu_frm_IssueAndReceiptLackQtyForProduction, this.barButtonItem20, this.menu_frm_WarrantyCard, this.menu_frm_ResourceWorkCenter, this.menu_frm_ServiceLevelAgreement, this.menu_frm_ServiceContractTemplate,
			this.menu_frm_BatchDetailManagement, this.menu_frm_Operations, this.menu_frm_PurchaseBlanketAgreement, this.menu_frm_AdvanceRouting, this.menu_frm_PosMaster, this.menu_frm_PosAssignment, this.menu_frm_MarketingCampaign, this.menu_frm_AlternativeItems, this.menu_frm_TemplateWizard, this.menu_frm_SerialNumberDetailManagement,
			this.menu_frm_DownPaymentRequest, this.menu_frm_DownPaymentInvoice, this.menu_frm_PurchaseDownPaymentRequest, this.menu_frm_PurchaseDownPaymentInvoice, this.menu_frm_ChangeLogList, this.menu_frm_ChangePassword, this.menu_frm_AssetMasterData, this.menu_frm_ExternalRouting, this.menu_frm_ResourceWorkScheduling, this.barButton_ChangeServerAddress,
			this.barSubItem9, this.menu_frm_BiometricData, this.menu_frm_BiometricDataProcess, this.menu_frm_TimeSheetInOut, this.menu_frm_BiometricEmpMapping, this.menu_frm_BiometricConfiguration, this.barSub_ProjectManagement, this.menu_frm_WBSTemplate, this.menu_frm_ContextActionPublishService, this.menu_frm_MenuStructure,
			this.bar_ExportXlsx, this.menu_frm_HolidaySetup, this.menu_frm_InOutMethod, this.menu_frm_WorkShift, this.menu_frm_WorkScheduleSetup, this.menu_frm_WorkScheduleRegister, this.menu_frm_BarcodeInventoryCounting, this.menu_frm_AdditionalGeneralSettings, this.menu_frm_ImportFiles, this.menu_frm_QuickCreationBPMasterData,
			this.menu_frm_JobsManagement, this.barSub_DMS, this.menu_frm_Geography, this.menu_frm_SalesRoute, this.barSub_PackingManagement, this.menu, this.menu_frm_PackingManagement, this.barButtonItem35, this.barButtonItem36, this.barButtonItem37,
			this.menu_frm_CreateTypeCase, this.menu_frm_PackingOrder, this.menu_frm_PrintBillWrap, this.menu_frm_Traceability, this.menu_frm_WorkingProcess, this.menu_frm_FacebookManager, this.menu_frm_IncomingPaymentWizard, this.menu_frm_GoodsReceiptPOByBarcode, this.menu_frm_InventoryTransferByBarcode, this.menu_frm_ReceiptFromPOByBarcode,
			this.menu_frm_PackingByBarcode, this.menu_frm_QuestionMasterData, this.barSub_BinLocationSetup, this.menu_frm_FieldActivation, this.menu_frm_AttrCodeSetup, this.menu_frm_WHSubSetup, this.menu_frm_WHSubManager, this.barSub_BinLocation, this.menu_frm_BinLocMasterData, this.barSub_PreviewExternalReport,
			this.menu_frm_OutletVisitingFrequency, this.menu_frm_PromotionSetup, this.menu_frm_SettingReport, this.menu_frm_Channel, this.menu_frm_PromotionCondition, this.barSubItem10, this.barSub_Danhmuc, this.barSubItem15, this.barSubItem16, this.menu_frm_PromotionBenefit,
			this.menu_frm_PromotionProgram, this.menu_frm_AdvancePaymentWizard, this.barSubItem17, this.menu_frm_SalesCoverageGoogleMap, this.barSub_KPI, this.barButtonItem42, this.menu_frm_KPIsSetting, this.menu_frm_KPIsMasterData, this.menu_frm_SetIndicatorKPIs, this.barSub_ApprovalProcess,
			this.menu_frm_ApprovalStages, this.menu_frm_ApprovalTemplates, this.menu_frm_ListPointKPIs, this.menu_frm_UploadItemImage, this.menu_frm_ImageAlbum, this.barButtonItem41, this.menu_frm_ImageViewer, this.menu_frm_Marker, this.menu_frm_DMSAuth, this.menu_frm_AlertTemplate,
			this.menu_frm_DMSDelivery, this.menu_frm_ReturnRequest, this.menu_frm_ThirdPartyToken, this.menu_frm_UserOnlineDashboard, this.menu_frm_KPIPlan, this.menu_frm_KPIScope, this.menu_frm_KPIIndicator, this.menu_frm_KPIGroup, this.menu_frm_KPISetup, this.menu_frm_KPIScore,
			this.barButtonItem50, this.menu_frm_ItemImageViewer, this.menu_frm_NotificationEventManager, this.menu_frm_SettingApprovalDetail, this.menu_frm_AboutRangeTime, this.menu_frm_AccessLog, this.menu_frm_GoodsReturnRequest, this.menu_frm_DMSSettingUDF, this.bar_ShowFooter, this.bar_ExportXlsx2,
			this.bar_ExportXlsx3, this.bar_ExportXlsx4, this.barSub_RefreshUDF, this.menu_frm_AdminUploadAttachment, this.barToggle_Fontsize, this.menu_frm_ListDashboard
		});
		this.BarManager.MainMenu = this.MenuBar;
		this.BarManager.MaxItemId = 606;
		this.BarManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemTextEdit1, this.repositoryItemTextEdit2, this.repositoryItemTextEdit3, this.repositoryItemTextEdit4, this.repositoryItemTextEdit5, this.repositoryItemSearchControl1, this.repositoryItemProgressBar1, this.repositoryItemTextEdit6 });
		this.BarManager.StatusBar = this.bar3;
		this.BarManager.HighlightedLinkChanged += new DevExpress.XtraBars.HighlightedLinkChangedEventHandler(this.BarManager_HighlightedLinkChanged);
		this.ToolBar.BarAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.ToolBar.BarAppearance.Normal.Options.UseFont = true;
		this.ToolBar.BarName = "Tools";
		this.ToolBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
		this.ToolBar.DockCol = 0;
		this.ToolBar.DockRow = 1;
		this.ToolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.ToolBar.FloatLocation = new System.Drawing.Point(2408, 192);
		this.ToolBar.FloatSize = new System.Drawing.Size(100, 20);
		this.ToolBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[32]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_PrintLayout),
			new DevExpress.XtraBars.LinkPersistInfo(this.barPrint),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportExcel, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportXlsx),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportXlsx2),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportXlsx3),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportXlsx4),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportWord),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ExportPdf),
			new DevExpress.XtraBars.LinkPersistInfo(this.btn_ExportClipboard),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_Find, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_NewDoc),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonModeFirst),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonModePrev),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonModeNext),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonLast),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonRefreshData),
			new DevExpress.XtraBars.LinkPersistInfo(this.barFormSetting, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barProcedure, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_AutoFilterRow, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_BestFitColumn),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_Expand),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_ShowFooter),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_DefaultBranch, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_Schedule),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_Alert),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_EditWidget, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_SaveWidget, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_TableLayout, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_StackLayout),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_FlowLayout),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_FreeLayout)
		});
		this.ToolBar.OptionsBar.AllowQuickCustomization = false;
		this.ToolBar.OptionsBar.DisableClose = true;
		this.ToolBar.OptionsBar.DisableCustomization = true;
		this.ToolBar.OptionsBar.DrawDragBorder = false;
		this.ToolBar.Text = "Tools";
		this.ToolBar.Visible = false;
		this.bar_PrintLayout.Enabled = false;
		this.bar_PrintLayout.Hint = "Preview layout";
		this.bar_PrintLayout.Id = 10;
		this.bar_PrintLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_PrintLayout.ImageOptions.Image");
		this.bar_PrintLayout.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_PrintLayout.ImageOptions.LargeImage");
		this.bar_PrintLayout.Name = "bar_PrintLayout";
		this.bar_PrintLayout.Tag = "PreviewLayout";
		this.bar_PrintLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_PrintLayout_ItemClick);
		this.barPrint.Enabled = false;
		this.barPrint.Hint = "Print";
		this.barPrint.Id = 11;
		this.barPrint.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barPrint.ImageOptions.Image");
		this.barPrint.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barPrint.ImageOptions.LargeImage");
		this.barPrint.Name = "barPrint";
		toolTipItem.Text = "Print";
		superToolTip.Items.Add(toolTipItem);
		this.barPrint.SuperTip = superToolTip;
		this.barPrint.Tag = "Print";
		this.barPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ItemClick);
		this.bar_ExportExcel.Enabled = false;
		this.bar_ExportExcel.Hint = "Export Excel";
		this.bar_ExportExcel.Id = 91;
		this.bar_ExportExcel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportExcel.ImageOptions.Image");
		this.bar_ExportExcel.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportExcel.ImageOptions.LargeImage");
		this.bar_ExportExcel.Name = "bar_ExportExcel";
		toolTipItem2.Text = "Export to excel 2003 and below";
		superToolTip2.Items.Add(toolTipItem2);
		this.bar_ExportExcel.SuperTip = superToolTip2;
		this.bar_ExportExcel.Tag = "ExprtExcel2003";
		this.bar_ExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportExcel_ItemClick);
		this.bar_ExportXlsx.Enabled = false;
		this.bar_ExportXlsx.Hint = "Export Excel";
		this.bar_ExportXlsx.Id = 510;
		this.bar_ExportXlsx.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx.ImageOptions.Image");
		this.bar_ExportXlsx.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx.ImageOptions.LargeImage");
		this.bar_ExportXlsx.Name = "bar_ExportXlsx";
		toolTipItem3.Text = "Export to excel";
		superToolTip3.Items.Add(toolTipItem3);
		this.bar_ExportXlsx.SuperTip = superToolTip3;
		this.bar_ExportXlsx.Tag = "ExprtExcel";
		this.bar_ExportXlsx.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportXlsx_ItemClick);
		this.bar_ExportXlsx2.Enabled = false;
		this.bar_ExportXlsx2.Id = 600;
		this.bar_ExportXlsx2.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("bar_ExportXlsx2.ImageOptions.SvgImage");
		this.bar_ExportXlsx2.Name = "bar_ExportXlsx2";
		toolTipItem4.Text = "Export to excel 2";
		superToolTip4.Items.Add(toolTipItem4);
		this.bar_ExportXlsx2.SuperTip = superToolTip4;
		this.bar_ExportXlsx2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportXlsx2_ItemClick);
		this.bar_ExportXlsx3.Enabled = false;
		this.bar_ExportXlsx3.Id = 601;
		this.bar_ExportXlsx3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx3.ImageOptions.Image");
		this.bar_ExportXlsx3.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx3.ImageOptions.LargeImage");
		this.bar_ExportXlsx3.Name = "bar_ExportXlsx3";
		toolTipItem5.Text = "Export to excel 3";
		superToolTip5.Items.Add(toolTipItem5);
		this.bar_ExportXlsx3.SuperTip = superToolTip5;
		this.bar_ExportXlsx3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportXlsx3_ItemClick);
		this.bar_ExportXlsx4.Enabled = false;
		this.bar_ExportXlsx4.Id = 602;
		this.bar_ExportXlsx4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx4.ImageOptions.Image");
		this.bar_ExportXlsx4.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportXlsx4.ImageOptions.LargeImage");
		this.bar_ExportXlsx4.Name = "bar_ExportXlsx4";
		toolTipItem6.Text = "Export to excel 4";
		superToolTip6.Items.Add(toolTipItem6);
		this.bar_ExportXlsx4.SuperTip = superToolTip6;
		this.bar_ExportXlsx4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportXlsx4_ItemClick);
		this.bar_ExportWord.Enabled = false;
		this.bar_ExportWord.Hint = "Export Doc";
		this.bar_ExportWord.Id = 94;
		this.bar_ExportWord.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportWord.ImageOptions.Image");
		this.bar_ExportWord.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportWord.ImageOptions.LargeImage");
		this.bar_ExportWord.Name = "bar_ExportWord";
		toolTipItem7.Text = "Export to word";
		superToolTip7.Items.Add(toolTipItem7);
		this.bar_ExportWord.SuperTip = superToolTip7;
		this.bar_ExportWord.Tag = "ExportDoc";
		this.bar_ExportWord.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportWord_ItemClick);
		this.bar_ExportPdf.Enabled = false;
		this.bar_ExportPdf.Hint = "Export PDF";
		this.bar_ExportPdf.Id = 95;
		this.bar_ExportPdf.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ExportPdf.ImageOptions.Image");
		this.bar_ExportPdf.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ExportPdf.ImageOptions.LargeImage");
		this.bar_ExportPdf.Name = "bar_ExportPdf";
		toolTipItem8.Text = "Export to pdf\r\n";
		superToolTip8.Items.Add(toolTipItem8);
		this.bar_ExportPdf.SuperTip = superToolTip8;
		this.bar_ExportPdf.Tag = "ExportPdf";
		this.bar_ExportPdf.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ExportPdf_ItemClick);
		this.btn_ExportClipboard.Enabled = false;
		this.btn_ExportClipboard.Hint = "Copy To Clipboard";
		this.btn_ExportClipboard.Id = 134;
		this.btn_ExportClipboard.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportClipboard.ImageOptions.Image");
		this.btn_ExportClipboard.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btn_ExportClipboard.ImageOptions.LargeImage");
		this.btn_ExportClipboard.Name = "btn_ExportClipboard";
		toolTipItem9.Text = "Copy to clipboard";
		superToolTip9.Items.Add(toolTipItem9);
		this.btn_ExportClipboard.SuperTip = superToolTip9;
		this.btn_ExportClipboard.Tag = "Copy2Clipboard";
		this.btn_ExportClipboard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_ExportClipboard_ItemClick);
		this.bar_Find.Enabled = false;
		this.bar_Find.Id = 13;
		this.bar_Find.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_Find.ImageOptions.Image");
		this.bar_Find.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_Find.ImageOptions.LargeImage");
		this.bar_Find.Name = "bar_Find";
		toolTipItem10.Text = "Find";
		superToolTip10.Items.Add(toolTipItem10);
		this.bar_Find.SuperTip = superToolTip10;
		this.bar_Find.Tag = "BtnFind";
		this.bar_Find.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemFind_ItemClick);
		this.bar_NewDoc.Enabled = false;
		this.bar_NewDoc.Id = 14;
		this.bar_NewDoc.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_NewDoc.ImageOptions.Image");
		this.bar_NewDoc.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_NewDoc.ImageOptions.LargeImage");
		this.bar_NewDoc.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.A | System.Windows.Forms.Keys.Control);
		this.bar_NewDoc.Name = "bar_NewDoc";
		this.bar_NewDoc.ShortcutKeyDisplayString = "Ctrl+A";
		this.bar_NewDoc.ShowItemShortcut = DevExpress.Utils.DefaultBoolean.True;
		toolTipItem11.Text = "Create New";
		superToolTip11.Items.Add(toolTipItem11);
		this.bar_NewDoc.SuperTip = superToolTip11;
		this.bar_NewDoc.Tag = "BtnAdd";
		this.bar_NewDoc.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem8_ItemClick);
		this.barButtonModeFirst.Enabled = false;
		this.barButtonModeFirst.Id = 15;
		this.barButtonModeFirst.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonModeFirst.ImageOptions.Image");
		this.barButtonModeFirst.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonModeFirst.ImageOptions.LargeImage");
		this.barButtonModeFirst.Name = "barButtonModeFirst";
		toolTipItem12.Text = "First Record";
		superToolTip12.Items.Add(toolTipItem12);
		this.barButtonModeFirst.SuperTip = superToolTip12;
		this.barButtonModeFirst.Tag = "BtnFirst";
		this.barButtonModeFirst.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonModeFirst_ItemClick);
		this.barButtonModePrev.Enabled = false;
		this.barButtonModePrev.Id = 16;
		this.barButtonModePrev.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonModePrev.ImageOptions.Image");
		this.barButtonModePrev.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonModePrev.ImageOptions.LargeImage");
		this.barButtonModePrev.Name = "barButtonModePrev";
		toolTipItem13.Text = "Previous";
		superToolTip13.Items.Add(toolTipItem13);
		this.barButtonModePrev.SuperTip = superToolTip13;
		this.barButtonModePrev.Tag = "BtnPrevious";
		this.barButtonModePrev.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonModePrev_ItemClick);
		this.barButtonModeNext.Enabled = false;
		this.barButtonModeNext.Id = 17;
		this.barButtonModeNext.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonModeNext.ImageOptions.Image");
		this.barButtonModeNext.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonModeNext.ImageOptions.LargeImage");
		this.barButtonModeNext.Name = "barButtonModeNext";
		toolTipItem14.Text = "Next";
		superToolTip14.Items.Add(toolTipItem14);
		this.barButtonModeNext.SuperTip = superToolTip14;
		this.barButtonModeNext.Tag = "BtnNext";
		this.barButtonModeNext.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonModeNext_ItemClick);
		this.barButtonLast.Enabled = false;
		this.barButtonLast.Id = 18;
		this.barButtonLast.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonLast.ImageOptions.Image");
		this.barButtonLast.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonLast.ImageOptions.LargeImage");
		this.barButtonLast.Name = "barButtonLast";
		toolTipItem15.Text = "Last Record";
		superToolTip15.Items.Add(toolTipItem15);
		this.barButtonLast.SuperTip = superToolTip15;
		this.barButtonLast.Tag = "BtnLast";
		this.barButtonLast.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonLast_ItemClick);
		this.barButtonRefreshData.Enabled = false;
		this.barButtonRefreshData.Id = 271;
		this.barButtonRefreshData.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonRefreshData.ImageOptions.Image");
		this.barButtonRefreshData.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonRefreshData.ImageOptions.LargeImage");
		this.barButtonRefreshData.Name = "barButtonRefreshData";
		toolTipTitleItem.Text = "Refresh Data";
		superToolTip16.Items.Add(toolTipTitleItem);
		this.barButtonRefreshData.SuperTip = superToolTip16;
		this.barButtonRefreshData.Tag = "BtnRefresh";
		this.barButtonRefreshData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonRefreshData_ItemClick);
		this.barFormSetting.Enabled = false;
		this.barFormSetting.Id = 36;
		this.barFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barFormSetting.ImageOptions.Image");
		this.barFormSetting.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barFormSetting.ImageOptions.LargeImage");
		this.barFormSetting.Name = "barFormSetting";
		toolTipItem16.Text = "Form Setting";
		superToolTip17.Items.Add(toolTipItem16);
		this.barFormSetting.SuperTip = superToolTip17;
		this.barFormSetting.Tag = "BtnFormSetting";
		this.barFormSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barFormSetting_ItemClick);
		this.barProcedure.Enabled = false;
		this.barProcedure.Id = 390;
		this.barProcedure.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barProcedure.ImageOptions.Image");
		this.barProcedure.Name = "barProcedure";
		toolTipTitleItem2.Text = "DIY Script";
		toolTipItem17.LeftIndent = 6;
		toolTipItem17.Text = "Create New or Edit DIY Script";
		superToolTip18.Items.Add(toolTipTitleItem2);
		superToolTip18.Items.Add(toolTipItem17);
		this.barProcedure.SuperTip = superToolTip18;
		this.barProcedure.Tag = "BtnDIY";
		this.barProcedure.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barProcedure_ItemClick);
		this.bar_AutoFilterRow.Enabled = false;
		this.bar_AutoFilterRow.Hint = "Auto Filter Row";
		this.bar_AutoFilterRow.Id = 122;
		this.bar_AutoFilterRow.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_AutoFilterRow.ImageOptions.Image");
		this.bar_AutoFilterRow.Name = "bar_AutoFilterRow";
		toolTipItem18.Text = "Auto filter row";
		superToolTip19.Items.Add(toolTipItem18);
		this.bar_AutoFilterRow.SuperTip = superToolTip19;
		this.bar_AutoFilterRow.Tag = "BtnFilter";
		this.bar_AutoFilterRow.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barAutoFilterRow_ItemClick);
		this.bar_BestFitColumn.Enabled = false;
		this.bar_BestFitColumn.Hint = "Best Fit Columns";
		this.bar_BestFitColumn.Id = 123;
		this.bar_BestFitColumn.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_BestFitColumn.ImageOptions.Image");
		this.bar_BestFitColumn.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_BestFitColumn.ImageOptions.LargeImage");
		this.bar_BestFitColumn.Name = "bar_BestFitColumn";
		toolTipItem19.Text = "Best fit all column";
		superToolTip20.Items.Add(toolTipItem19);
		this.bar_BestFitColumn.SuperTip = superToolTip20;
		this.bar_BestFitColumn.Tag = "BtnBestFit";
		this.bar_BestFitColumn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_BestFitColumn_ItemClick);
		this.bar_BestFitColumn.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_BestFitColumn_ItemDoubleClick);
		this.bar_Expand.Enabled = false;
		this.bar_Expand.Hint = "Expand/Collapse All";
		this.bar_Expand.Id = 129;
		this.bar_Expand.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_Expand.ImageOptions.Image");
		this.bar_Expand.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_Expand.ImageOptions.LargeImage");
		this.bar_Expand.Name = "bar_Expand";
		toolTipItem20.Text = "Expand/Collapse All";
		superToolTip21.Items.Add(toolTipItem20);
		this.bar_Expand.SuperTip = superToolTip21;
		this.bar_Expand.Tag = "BtnExpand";
		this.bar_Expand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Expand_ItemClick);
		this.bar_ShowFooter.Enabled = false;
		this.bar_ShowFooter.Hint = "Show/Hide Footer";
		this.bar_ShowFooter.Id = 600;
		this.bar_ShowFooter.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_ShowFooter.ImageOptions.Image");
		this.bar_ShowFooter.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_ShowFooter.ImageOptions.LargeImage");
		this.bar_ShowFooter.Name = "bar_ShowFooter";
		toolTipItem21.Text = "Show/Hide Footer";
		superToolTip22.Items.Add(toolTipItem21);
		this.bar_ShowFooter.SuperTip = superToolTip22;
		this.bar_ShowFooter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ShowFooter_ItemClick);
		this.bar_DefaultBranch.Enabled = false;
		this.bar_DefaultBranch.Hint = "Default Branch Selection";
		this.bar_DefaultBranch.Id = 128;
		this.bar_DefaultBranch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_DefaultBranch.ImageOptions.Image");
		this.bar_DefaultBranch.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_DefaultBranch.ImageOptions.LargeImage");
		this.bar_DefaultBranch.Name = "bar_DefaultBranch";
		toolTipItem22.Text = "Select default branch";
		superToolTip23.Items.Add(toolTipItem22);
		this.bar_DefaultBranch.SuperTip = superToolTip23;
		this.bar_DefaultBranch.Tag = "BtnDefBranch";
		this.bar_DefaultBranch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_DefaultBranch_ItemClick);
		this.bar_Schedule.Id = 276;
		this.bar_Schedule.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_Schedule.ImageOptions.Image");
		this.bar_Schedule.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_Schedule.ImageOptions.LargeImage");
		this.bar_Schedule.Name = "bar_Schedule";
		toolTipTitleItem3.Text = "Calendars";
		superToolTip24.Items.Add(toolTipTitleItem3);
		this.bar_Schedule.SuperTip = superToolTip24;
		this.bar_Schedule.Tag = "BtnCalendar";
		this.bar_Schedule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Schedule_ItemClick);
		this.bar_Alert.Id = 288;
		this.bar_Alert.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_Alert.ImageOptions.Image");
		this.bar_Alert.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_Alert.ImageOptions.LargeImage");
		this.bar_Alert.Name = "bar_Alert";
		toolTipItem23.Text = "Alert";
		superToolTip25.Items.Add(toolTipItem23);
		this.bar_Alert.SuperTip = superToolTip25;
		this.bar_Alert.Tag = "BtnAlert";
		this.bar_Alert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Alert_ItemClick);
		this.bar_EditWidget.Caption = "Edit Widget";
		this.bar_EditWidget.Enabled = false;
		this.bar_EditWidget.Id = 454;
		this.bar_EditWidget.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_EditWidget.ImageOptions.Image");
		this.bar_EditWidget.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_EditWidget.ImageOptions.LargeImage");
		this.bar_EditWidget.Name = "bar_EditWidget";
		toolTipTitleItem4.Text = "Edit Widget\r\n";
		superToolTip26.Items.Add(toolTipTitleItem4);
		this.bar_EditWidget.SuperTip = superToolTip26;
		this.bar_EditWidget.Tag = "BtnEditWid";
		this.bar_EditWidget.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_EditWidget_ItemClick);
		this.bar_SaveWidget.Caption = "Save Widget";
		this.bar_SaveWidget.Enabled = false;
		this.bar_SaveWidget.Id = 454;
		this.bar_SaveWidget.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_SaveWidget.ImageOptions.Image");
		this.bar_SaveWidget.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_SaveWidget.ImageOptions.LargeImage");
		this.bar_SaveWidget.Name = "bar_SaveWidget";
		toolTipTitleItem5.Text = "Save Widget";
		superToolTip27.Items.Add(toolTipTitleItem5);
		this.bar_SaveWidget.SuperTip = superToolTip27;
		this.bar_SaveWidget.Tag = "BtnSaveWid";
		this.bar_SaveWidget.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_SaveWidget_ItemClick);
		this.bar_TableLayout.Caption = "Table Layout";
		this.bar_TableLayout.Enabled = false;
		this.bar_TableLayout.Id = 455;
		this.bar_TableLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_TableLayout.ImageOptions.Image");
		this.bar_TableLayout.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_TableLayout.ImageOptions.LargeImage");
		this.bar_TableLayout.Name = "bar_TableLayout";
		toolTipTitleItem6.Text = "Table Layout\r\n";
		superToolTip28.Items.Add(toolTipTitleItem6);
		this.bar_TableLayout.SuperTip = superToolTip28;
		this.bar_TableLayout.Tag = "BtnTableLayout";
		this.bar_TableLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_TableLayout_ItemClick);
		this.bar_StackLayout.Caption = "Stack Layout";
		this.bar_StackLayout.Enabled = false;
		this.bar_StackLayout.Id = 456;
		this.bar_StackLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_StackLayout.ImageOptions.Image");
		this.bar_StackLayout.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_StackLayout.ImageOptions.LargeImage");
		this.bar_StackLayout.Name = "bar_StackLayout";
		toolTipTitleItem7.Text = "Stack Layout\r\n";
		superToolTip29.Items.Add(toolTipTitleItem7);
		this.bar_StackLayout.SuperTip = superToolTip29;
		this.bar_StackLayout.Tag = "BtnStackLayout";
		this.bar_StackLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_StackLayout_ItemClick);
		this.bar_FlowLayout.Caption = "Flow Layout";
		this.bar_FlowLayout.Enabled = false;
		this.bar_FlowLayout.Id = 457;
		this.bar_FlowLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_FlowLayout.ImageOptions.Image");
		this.bar_FlowLayout.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_FlowLayout.ImageOptions.LargeImage");
		this.bar_FlowLayout.Name = "bar_FlowLayout";
		toolTipTitleItem8.Text = "Flow Layout";
		superToolTip30.Items.Add(toolTipTitleItem8);
		this.bar_FlowLayout.SuperTip = superToolTip30;
		this.bar_FlowLayout.Tag = "BtnFlowLayout";
		this.bar_FlowLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_FlowLayout_ItemClick);
		this.bar_FreeLayout.Caption = "Free Layout";
		this.bar_FreeLayout.Enabled = false;
		this.bar_FreeLayout.Id = 458;
		this.bar_FreeLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bar_FreeLayout.ImageOptions.Image");
		this.bar_FreeLayout.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bar_FreeLayout.ImageOptions.LargeImage");
		this.bar_FreeLayout.Name = "bar_FreeLayout";
		toolTipTitleItem9.Text = "Free Layout";
		superToolTip31.Items.Add(toolTipTitleItem9);
		this.bar_FreeLayout.SuperTip = superToolTip31;
		this.bar_FreeLayout.Tag = "BtnFreeLayout";
		this.bar_FreeLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_FreeLayout_ItemClick);
		this.MenuBar.BarName = "Main menu";
		this.MenuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
		this.MenuBar.DockCol = 0;
		this.MenuBar.DockRow = 0;
		this.MenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.MenuBar.FloatLocation = new System.Drawing.Point(59, 162);
		this.MenuBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_System),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Module, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Tool),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Window),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Help)
		});
		this.MenuBar.OptionsBar.AllowQuickCustomization = false;
		this.MenuBar.OptionsBar.DisableClose = true;
		this.MenuBar.OptionsBar.DisableCustomization = true;
		this.MenuBar.OptionsBar.DrawDragBorder = false;
		this.MenuBar.OptionsBar.MultiLine = true;
		this.MenuBar.OptionsBar.UseWholeRow = true;
		this.MenuBar.Tag = "MainMenu";
		this.MenuBar.Text = "Main menu";
		this.MenuBar.Visible = false;
		this.barSub_System.Caption = "System";
		this.barSub_System.Description = "M0000";
		this.barSub_System.Id = 0;
		this.barSub_System.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Preview),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PreviewLayout),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Print),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Export),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PreviewGridview),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LoginSap, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Exit, true)
		});
		this.barSub_System.Name = "barSub_System";
		this.barSub_Preview.Caption = "Preview";
		this.barSub_Preview.Enabled = false;
		this.barSub_Preview.Id = 70;
		this.barSub_Preview.Name = "barSub_Preview";
		this.barSub_Preview.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Preview_ItemClick);
		this.barSub_PreviewLayout.Caption = "Preview Layouts";
		this.barSub_PreviewLayout.Enabled = false;
		this.barSub_PreviewLayout.Id = 71;
		this.barSub_PreviewLayout.Name = "barSub_PreviewLayout";
		this.barSub_PreviewLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_PreviewLayouts_ItemClick);
		this.barSub_Print.Caption = "Print";
		this.barSub_Print.Enabled = false;
		this.barSub_Print.Id = 72;
		this.barSub_Print.Name = "barSub_Print";
		this.barSub_Print.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSub_Print_ItemClick);
		this.barSub_Export.Caption = "Export";
		this.barSub_Export.Enabled = false;
		this.barSub_Export.Id = 111;
		this.barSub_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barSub_Export.ImageOptions.Image");
		this.barSub_Export.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barSub_Export.ImageOptions.LargeImage");
		this.barSub_Export.Name = "barSub_Export";
		this.barSub_Export.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Export_ItemClick);
		this.barSub_PreviewGridview.Caption = "Preview Gridview";
		this.barSub_PreviewGridview.Enabled = false;
		this.barSub_PreviewGridview.Id = 112;
		this.barSub_PreviewGridview.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barSub_PreviewGridview.ImageOptions.Image");
		this.barSub_PreviewGridview.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barSub_PreviewGridview.ImageOptions.LargeImage");
		this.barSub_PreviewGridview.Name = "barSub_PreviewGridview";
		this.barSub_PreviewGridview.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_PreviewGrid_ItemClick);
		this.menu_frm_LoginSap.Caption = "Change User";
		this.menu_frm_LoginSap.Id = 236;
		this.menu_frm_LoginSap.Name = "menu_frm_LoginSap";
		this.menu_frm_LoginSap.Tag = "ChangeUser";
		this.menu_frm_LoginSap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LoginSap_ItemClick);
		this.barSub_Exit.Caption = "Exit";
		this.barSub_Exit.Id = 69;
		this.barSub_Exit.Name = "barSub_Exit";
		this.barSub_Exit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_Exit_ItemClick);
		this.barSub_Module.Caption = "Module";
		this.barSub_Module.Description = "M0001";
		this.barSub_Module.Id = 3;
		this.barSub_Module.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[23]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Administration),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Dashboard),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SalesAR),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PurchaseAP),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BP),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Inventory),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Production, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_MTO),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_MRP),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_RoutingResources),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PackingManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Service, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BankingPayment),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Retail),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_HRM),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PM),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.barSub_DMS, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_KPI),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_UDO, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Reports),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Integrated),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Configuration, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Language)
		});
		this.barSub_Module.Name = "barSub_Module";
		this.barSub_Administration.Caption = "Administration";
		this.barSub_Administration.Description = "20000";
		this.barSub_Administration.Id = 44;
		this.barSub_Administration.ImageOptions.ImageIndex = 3;
		this.barSub_Administration.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[10]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barI_frm_LoginSap),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ExchangeRatesAndIndexs),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AssetMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Setup, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Authorization),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SystemInitialization),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ApprovalProcess, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_EmpToken),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PlugIn, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AccessLog)
		});
		this.barSub_Administration.Name = "barSub_Administration";
		this.barSub_Administration.Tag = "Administration";
		this.barI_frm_LoginSap.Caption = "Change User";
		this.barI_frm_LoginSap.Description = "20100";
		this.barI_frm_LoginSap.Id = 144;
		this.barI_frm_LoginSap.Name = "barI_frm_LoginSap";
		this.barI_frm_LoginSap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem25_ItemClick);
		this.menu_frm_ExchangeRatesAndIndexs.Caption = "Exchange Rates And Indexes";
		this.menu_frm_ExchangeRatesAndIndexs.Description = "20200";
		this.menu_frm_ExchangeRatesAndIndexs.Id = 145;
		this.menu_frm_ExchangeRatesAndIndexs.Name = "menu_frm_ExchangeRatesAndIndexs";
		this.menu_frm_ExchangeRatesAndIndexs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem26_ItemClick);
		this.menu_frm_AssetMasterData.Caption = "Asset Master Data";
		this.menu_frm_AssetMasterData.Id = 496;
		this.menu_frm_AssetMasterData.Name = "menu_frm_AssetMasterData";
		this.menu_frm_AssetMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AssetMasterData_ItemClick);
		this.barSub_Setup.Caption = "Setup";
		this.barSub_Setup.Description = "20300";
		this.barSub_Setup.Id = 138;
		this.barSub_Setup.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[17]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupGeneral),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GeneSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CompanyDetails),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Territory),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupBp, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupBanking),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Financial),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Sales),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Purcharsing),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupInventory),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupHumanResouces),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SetupReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Opportunities),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ProjectManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdditionalGeneralSettings),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ThirdPartyToken)
		});
		this.barSub_Setup.Name = "barSub_Setup";
		this.barSub_SetupGeneral.Caption = "General";
		this.barSub_SetupGeneral.Description = "20310";
		this.barSub_SetupGeneral.Id = 139;
		this.barSub_SetupGeneral.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[16]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Users),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ChangePassword),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesEmployeeBuyers),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Freight),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UserDefinedTexts),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SetupAdmin, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BarcodeConfig),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_FilterSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_FtpServerSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LicenseManager),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MailSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_NotificationSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AutoSelectBatchSerialConfig),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ExternalRouting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ContextActionPublishService),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JobsManagement)
		});
		this.barSub_SetupGeneral.Name = "barSub_SetupGeneral";
		this.menu_frm_Users.Caption = "User";
		this.menu_frm_Users.Description = "20311";
		this.menu_frm_Users.Id = 140;
		this.menu_frm_Users.Name = "menu_frm_Users";
		this.menu_frm_Users.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem21_ItemClick);
		this.menu_frm_ChangePassword.Caption = "Change Password";
		this.menu_frm_ChangePassword.Id = 495;
		this.menu_frm_ChangePassword.Name = "menu_frm_ChangePassword";
		this.menu_frm_ChangePassword.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ChangePassword_ItemClick);
		this.menu_frm_SalesEmployeeBuyers.Caption = "Sales Employees/Buyer";
		this.menu_frm_SalesEmployeeBuyers.Description = "20313";
		this.menu_frm_SalesEmployeeBuyers.Id = 142;
		this.menu_frm_SalesEmployeeBuyers.Name = "menu_frm_SalesEmployeeBuyers";
		this.menu_frm_SalesEmployeeBuyers.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesEmployeeBuyers_ItemClick);
		this.menu_frm_Freight.Caption = "Freight";
		this.menu_frm_Freight.Description = "20314";
		this.menu_frm_Freight.Id = 143;
		this.menu_frm_Freight.Name = "menu_frm_Freight";
		this.menu_frm_Freight.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Freight_ItemClick);
		this.menu_frm_UserDefinedTexts.Caption = "Predefined Texts";
		this.menu_frm_UserDefinedTexts.Id = 424;
		this.menu_frm_UserDefinedTexts.Name = "menu_frm_UserDefinedTexts";
		this.menu_frm_UserDefinedTexts.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UserDefinedTexts_ItemClick);
		this.menu_frm_SetupAdmin.Caption = "Admin Password";
		this.menu_frm_SetupAdmin.Description = "20315";
		this.menu_frm_SetupAdmin.Id = 277;
		this.menu_frm_SetupAdmin.Name = "menu_frm_SetupAdmin";
		this.menu_frm_SetupAdmin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SetupAdmin_ItemClick);
		this.menu_frm_BarcodeConfig.Caption = "Barcode Config";
		this.menu_frm_BarcodeConfig.Description = "20316";
		this.menu_frm_BarcodeConfig.Id = 270;
		this.menu_frm_BarcodeConfig.Name = "menu_frm_BarcodeConfig";
		this.menu_frm_BarcodeConfig.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BarcodeConfig_ItemClick);
		this.menu_frm_FilterSetting.Caption = "Filter";
		this.menu_frm_FilterSetting.Description = "20317";
		this.menu_frm_FilterSetting.Id = 153;
		this.menu_frm_FilterSetting.Name = "menu_frm_FilterSetting";
		this.menu_frm_FilterSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_FilterSetting_ItemClick);
		this.menu_frm_FtpServerSetting.Caption = "FTP Server";
		this.menu_frm_FtpServerSetting.Description = "20318";
		this.menu_frm_FtpServerSetting.Id = 146;
		this.menu_frm_FtpServerSetting.Name = "menu_frm_FtpServerSetting";
		this.menu_frm_FtpServerSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_FtpServerSetting_ItemClick);
		this.menu_frm_LicenseManager.Caption = "License Manager";
		this.menu_frm_LicenseManager.Description = "20321";
		this.menu_frm_LicenseManager.Id = 319;
		this.menu_frm_LicenseManager.Name = "menu_frm_LicenseManager";
		this.menu_frm_LicenseManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LicenseManager_ItemClick);
		this.menu_frm_MailSetting.Caption = "E-Mail Setting";
		this.menu_frm_MailSetting.Id = 432;
		this.menu_frm_MailSetting.Name = "menu_frm_MailSetting";
		this.menu_frm_MailSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MailSetting_ItemClick);
		this.menu_frm_NotificationSetting.Caption = "Notification Setting";
		this.menu_frm_NotificationSetting.Id = 455;
		this.menu_frm_NotificationSetting.Name = "menu_frm_NotificationSetting";
		this.menu_frm_NotificationSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_NotificationSetting_ItemClick);
		this.menu_frm_AutoSelectBatchSerialConfig.Caption = "Automated selection setting for Batch and Serial Number";
		this.menu_frm_AutoSelectBatchSerialConfig.Id = 465;
		this.menu_frm_AutoSelectBatchSerialConfig.Name = "menu_frm_AutoSelectBatchSerialConfig";
		this.menu_frm_AutoSelectBatchSerialConfig.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AutoSelectBatchSerialConfig_ItemClick);
		this.menu_frm_ExternalRouting.Caption = "URI for Outgoing Data";
		this.menu_frm_ExternalRouting.Id = 497;
		this.menu_frm_ExternalRouting.Name = "menu_frm_ExternalRouting";
		this.menu_frm_ExternalRouting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ExternalRouting_ItemClick);
		this.menu_frm_ContextActionPublishService.Caption = "URI for Incoming Data";
		this.menu_frm_ContextActionPublishService.Id = 508;
		this.menu_frm_ContextActionPublishService.Name = "menu_frm_ContextActionPublishService";
		this.menu_frm_ContextActionPublishService.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ContextActionPublishService_ItemClick);
		this.menu_frm_JobsManagement.Caption = "Jobs Management";
		this.menu_frm_JobsManagement.Id = 520;
		this.menu_frm_JobsManagement.Name = "menu_frm_JobsManagement";
		this.menu_frm_JobsManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JobsManagement_ItemClick);
		this.menu_frm_GeneSetting.Caption = "General Setting";
		this.menu_frm_GeneSetting.Description = "20320";
		this.menu_frm_GeneSetting.Id = 148;
		this.menu_frm_GeneSetting.Name = "menu_frm_GeneSetting";
		this.menu_frm_GeneSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem29_ItemClick);
		this.menu_frm_CompanyDetails.Caption = "Company Details";
		this.menu_frm_CompanyDetails.Description = "20330";
		this.menu_frm_CompanyDetails.Id = 235;
		this.menu_frm_CompanyDetails.Name = "menu_frm_CompanyDetails";
		this.menu_frm_CompanyDetails.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CompanyDetails_ItemClick);
		this.menu_frm_Territory.Caption = "Territory";
		this.menu_frm_Territory.Id = 419;
		this.menu_frm_Territory.Name = "menu_frm_Territory";
		this.menu_frm_Territory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Territory_ItemClick);
		this.barSub_SetupBp.Caption = "Business Partner";
		this.barSub_SetupBp.Description = "20340";
		this.barSub_SetupBp.Id = 156;
		this.barSub_SetupBp.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Countries),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CustomerGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_VendorGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BusinessPartnerProperties),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BusinessPartnerPriorities),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PaymentTerms)
		});
		this.barSub_SetupBp.Name = "barSub_SetupBp";
		this.menu_frm_Countries.Caption = "Countries";
		this.menu_frm_Countries.Description = "20341";
		this.menu_frm_Countries.Id = 298;
		this.menu_frm_Countries.Name = "menu_frm_Countries";
		this.menu_frm_Countries.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Countries_ItemClick);
		this.menu_frm_CustomerGroup.Caption = "Customer Group";
		this.menu_frm_CustomerGroup.Description = "20342";
		this.menu_frm_CustomerGroup.Id = 158;
		this.menu_frm_CustomerGroup.Name = "menu_frm_CustomerGroup";
		this.menu_frm_CustomerGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CustomerGroup_ItemClick);
		this.menu_frm_VendorGroup.Caption = "Vendor Group";
		this.menu_frm_VendorGroup.Description = "20343";
		this.menu_frm_VendorGroup.Id = 159;
		this.menu_frm_VendorGroup.Name = "menu_frm_VendorGroup";
		this.menu_frm_VendorGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_VenderGroup_ItemClick);
		this.menu_frm_BusinessPartnerProperties.Caption = "Business Properties";
		this.menu_frm_BusinessPartnerProperties.Description = "20344";
		this.menu_frm_BusinessPartnerProperties.Id = 165;
		this.menu_frm_BusinessPartnerProperties.Name = "menu_frm_BusinessPartnerProperties";
		this.menu_frm_BusinessPartnerProperties.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BusinessPartnerProperties_ItemClick);
		this.menu_frm_BusinessPartnerPriorities.Caption = "Business Priorities";
		this.menu_frm_BusinessPartnerPriorities.Description = "20345";
		this.menu_frm_BusinessPartnerPriorities.Id = 166;
		this.menu_frm_BusinessPartnerPriorities.Name = "menu_frm_BusinessPartnerPriorities";
		this.menu_frm_BusinessPartnerPriorities.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BusinessPartnerPriorities_ItemClick);
		this.menu_frm_PaymentTerms.Caption = "Payment Terms";
		this.menu_frm_PaymentTerms.Description = "20346";
		this.menu_frm_PaymentTerms.Id = 326;
		this.menu_frm_PaymentTerms.Name = "menu_frm_PaymentTerms";
		this.menu_frm_PaymentTerms.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PaymentTerms_ItemClick);
		this.barSub_SetupBanking.Caption = "Banking";
		this.barSub_SetupBanking.Description = "20350";
		this.barSub_SetupBanking.Id = 306;
		this.barSub_SetupBanking.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CreditCardSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CreditPaymentMethods)
		});
		this.barSub_SetupBanking.Name = "barSub_SetupBanking";
		this.menu_frm_CreditCardSetup.Caption = "Credit Card";
		this.menu_frm_CreditCardSetup.Description = "20351";
		this.menu_frm_CreditCardSetup.Id = 307;
		this.menu_frm_CreditCardSetup.Name = "menu_frm_CreditCardSetup";
		this.menu_frm_CreditCardSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CreditCardSetup_ItemClick);
		this.menu_frm_CreditPaymentMethods.Caption = "Credit Card Payment Methods";
		this.menu_frm_CreditPaymentMethods.Description = "20352";
		this.menu_frm_CreditPaymentMethods.Id = 308;
		this.menu_frm_CreditPaymentMethods.Name = "menu_frm_CreditPaymentMethods";
		this.menu_frm_CreditPaymentMethods.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.frm_CreditPaymentMethods_ItemClick);
		this.barSub_Financial.Caption = "Financial";
		this.barSub_Financial.Description = "20360";
		this.barSub_Financial.Id = 181;
		this.barSub_Financial.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Braches),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CostCenters),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Currencies),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Dimension),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DistributeRule),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TaxGroups),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Project)
		});
		this.barSub_Financial.Name = "barSub_Financial";
		this.menu_frm_Braches.Caption = "Branches";
		this.menu_frm_Braches.Description = "20361";
		this.menu_frm_Braches.Id = 182;
		this.menu_frm_Braches.Name = "menu_frm_Braches";
		this.menu_frm_Braches.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Braches_ItemClick);
		this.menu_frm_CostCenters.Caption = "Cost Centers";
		this.menu_frm_CostCenters.Description = "20362";
		this.menu_frm_CostCenters.Id = 320;
		this.menu_frm_CostCenters.Name = "menu_frm_CostCenters";
		this.menu_frm_CostCenters.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CostCenters_ItemClick);
		this.menu_frm_Currencies.Caption = "Currencies";
		this.menu_frm_Currencies.Description = "20363";
		this.menu_frm_Currencies.Id = 183;
		this.menu_frm_Currencies.Name = "menu_frm_Currencies";
		this.menu_frm_Currencies.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Currencies_ItemClick);
		this.menu_frm_Dimension.Caption = "Dimensions";
		this.menu_frm_Dimension.Description = "20364";
		this.menu_frm_Dimension.Id = 317;
		this.menu_frm_Dimension.Name = "menu_frm_Dimension";
		this.menu_frm_Dimension.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Dimension_ItemClick);
		this.menu_frm_DistributeRule.Caption = "Distribute Rules";
		this.menu_frm_DistributeRule.Description = "20365";
		this.menu_frm_DistributeRule.Id = 316;
		this.menu_frm_DistributeRule.Name = "menu_frm_DistributeRule";
		this.menu_frm_DistributeRule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DistributeRule_ItemClick);
		this.menu_frm_TaxGroups.Caption = "Tax Group";
		this.menu_frm_TaxGroups.Description = "20366";
		this.menu_frm_TaxGroups.Id = 184;
		this.menu_frm_TaxGroups.Name = "menu_frm_TaxGroups";
		this.menu_frm_TaxGroups.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TaxGroups_ItemClick);
		this.menu_frm_Project.Caption = "Project";
		this.menu_frm_Project.Description = "20367";
		this.menu_frm_Project.Id = 257;
		this.menu_frm_Project.Name = "menu_frm_Project";
		this.menu_frm_Project.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Project_ItemClick);
		this.barSub_Sales.Caption = "Sales";
		this.barSub_Sales.Description = "20370";
		this.barSub_Sales.Id = 364;
		this.barSub_Sales.Name = "barSub_Sales";
		this.barSub_Purcharsing.Caption = "Purchasing";
		this.barSub_Purcharsing.Description = "20391";
		this.barSub_Purcharsing.Id = 370;
		this.barSub_Purcharsing.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LandedCostsSetup)
		});
		this.barSub_Purcharsing.Name = "barSub_Purcharsing";
		this.menu_frm_LandedCostsSetup.Caption = "Landed Costs";
		this.menu_frm_LandedCostsSetup.Description = "20392";
		this.menu_frm_LandedCostsSetup.Id = 371;
		this.menu_frm_LandedCostsSetup.Name = "menu_frm_LandedCostsSetup";
		this.menu_frm_LandedCostsSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LandedCostsSetup_ItemClick);
		this.barSub_SetupInventory.Caption = "Inventory";
		this.barSub_SetupInventory.Description = "20380";
		this.barSub_SetupInventory.Id = 157;
		this.barSub_SetupInventory.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[16]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ItemGroups),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ItemProperties),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WareHouses, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UnitOfMeasure, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UnitOfMeasureGroups),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Shipping, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LengthAndWidth),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Weight),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CustomsGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Manufacturer),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Locations),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CycleCountDetermination),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryCycles),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Vehicle),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_VehicleType),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BinLocationSetup)
		});
		this.barSub_SetupInventory.Name = "barSub_SetupInventory";
		this.menu_frm_ItemGroups.Caption = "Item Group";
		this.menu_frm_ItemGroups.Description = "20381";
		this.menu_frm_ItemGroups.Id = 167;
		this.menu_frm_ItemGroups.Name = "menu_frm_ItemGroups";
		this.menu_frm_ItemGroups.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ItemGroups_ItemClick);
		this.menu_frm_ItemProperties.Caption = "Item Properties";
		this.menu_frm_ItemProperties.Description = "20382";
		this.menu_frm_ItemProperties.Id = 168;
		this.menu_frm_ItemProperties.Name = "menu_frm_ItemProperties";
		this.menu_frm_ItemProperties.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ItemProperties_ItemClick);
		this.menu_frm_WareHouses.Caption = "Warehouse";
		this.menu_frm_WareHouses.Description = "20383";
		this.menu_frm_WareHouses.Id = 169;
		this.menu_frm_WareHouses.Name = "menu_frm_WareHouses";
		this.menu_frm_WareHouses.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WareHourse_ItemClick);
		this.menu_frm_UnitOfMeasure.Caption = "Unit Of Measure";
		this.menu_frm_UnitOfMeasure.Description = "20384";
		this.menu_frm_UnitOfMeasure.Id = 170;
		this.menu_frm_UnitOfMeasure.Name = "menu_frm_UnitOfMeasure";
		this.menu_frm_UnitOfMeasure.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UnitOfMeasure_ItemClick);
		this.menu_frm_UnitOfMeasureGroups.Caption = "Unit Of Measure Group";
		this.menu_frm_UnitOfMeasureGroups.Description = "20385";
		this.menu_frm_UnitOfMeasureGroups.Id = 171;
		this.menu_frm_UnitOfMeasureGroups.Name = "menu_frm_UnitOfMeasureGroups";
		this.menu_frm_UnitOfMeasureGroups.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UnitOfMeasureGroups_ItemClick);
		this.menu_frm_Shipping.Caption = "Shipping Types";
		this.menu_frm_Shipping.Description = "20386";
		this.menu_frm_Shipping.Id = 178;
		this.menu_frm_Shipping.Name = "menu_frm_Shipping";
		this.menu_frm_Shipping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Shipping_ItemClick);
		this.menu_frm_LengthAndWidth.Caption = "Length and Width";
		this.menu_frm_LengthAndWidth.Description = "20387";
		this.menu_frm_LengthAndWidth.Id = 172;
		this.menu_frm_LengthAndWidth.Name = "menu_frm_LengthAndWidth";
		this.menu_frm_LengthAndWidth.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LengthAndWidth_ItemClick);
		this.menu_frm_Weight.Caption = "Weight";
		this.menu_frm_Weight.Description = "20388";
		this.menu_frm_Weight.Id = 173;
		this.menu_frm_Weight.Name = "menu_frm_Weight";
		this.menu_frm_Weight.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Weight_ItemClick);
		this.menu_frm_CustomsGroup.Caption = "Customs Groups";
		this.menu_frm_CustomsGroup.Description = "20389";
		this.menu_frm_CustomsGroup.Id = 174;
		this.menu_frm_CustomsGroup.Name = "menu_frm_CustomsGroup";
		this.menu_frm_CustomsGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barI_frm_CustomerGroup_ItemClick);
		this.menu_frm_Manufacturer.Caption = "Manufacturers";
		this.menu_frm_Manufacturer.Description = "20393";
		this.menu_frm_Manufacturer.Id = 175;
		this.menu_frm_Manufacturer.Name = "menu_frm_Manufacturer";
		this.menu_frm_Manufacturer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Manufacturer_ItemClick);
		this.menu_frm_Locations.Caption = "Locations";
		this.menu_frm_Locations.Description = "20394";
		this.menu_frm_Locations.Id = 177;
		this.menu_frm_Locations.Name = "menu_frm_Locations";
		this.menu_frm_Locations.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Locations_ItemClick);
		this.menu_frm_CycleCountDetermination.Caption = "Cycle Count Determination";
		this.menu_frm_CycleCountDetermination.Description = "20397";
		this.menu_frm_CycleCountDetermination.Id = 329;
		this.menu_frm_CycleCountDetermination.Name = "menu_frm_CycleCountDetermination";
		this.menu_frm_CycleCountDetermination.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CycleCountDetermination_ItemClick);
		this.menu_frm_InventoryCycles.Caption = "Inventory Cycles";
		this.menu_frm_InventoryCycles.Description = "20398";
		this.menu_frm_InventoryCycles.Id = 330;
		this.menu_frm_InventoryCycles.Name = "menu_frm_InventoryCycles";
		this.menu_frm_InventoryCycles.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryCycles_ItemClick);
		this.menu_frm_Vehicle.Caption = "Vehicle";
		this.menu_frm_Vehicle.Description = "20371";
		this.menu_frm_Vehicle.Id = 365;
		this.menu_frm_Vehicle.Name = "menu_frm_Vehicle";
		this.menu_frm_Vehicle.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Vehicle_ItemClick);
		this.menu_frm_VehicleType.Caption = "Vehicle Type";
		this.menu_frm_VehicleType.Description = "20372";
		this.menu_frm_VehicleType.Id = 366;
		this.menu_frm_VehicleType.Name = "menu_frm_VehicleType";
		this.menu_frm_VehicleType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_VehicleType_ItemClick);
		this.barSub_BinLocationSetup.Caption = "Bin Locations";
		this.barSub_BinLocationSetup.Id = 543;
		this.barSub_BinLocationSetup.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_FieldActivation),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AttrCodeSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WHSubSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WHSubManager)
		});
		this.barSub_BinLocationSetup.Name = "barSub_BinLocationSetup";
		this.menu_frm_FieldActivation.Caption = "Bin Location Field Activation";
		this.menu_frm_FieldActivation.Id = 544;
		this.menu_frm_FieldActivation.Name = "menu_frm_FieldActivation";
		this.menu_frm_FieldActivation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_FieldActivation_ItemClick);
		this.menu_frm_AttrCodeSetup.Caption = "Bin Location Attribute Codes";
		this.menu_frm_AttrCodeSetup.Id = 545;
		this.menu_frm_AttrCodeSetup.Name = "menu_frm_AttrCodeSetup";
		this.menu_frm_AttrCodeSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AttrCodeSetup_ItemClick);
		this.menu_frm_WHSubSetup.Caption = "Warehouse Sublevel Codes";
		this.menu_frm_WHSubSetup.Id = 546;
		this.menu_frm_WHSubSetup.Name = "menu_frm_WHSubSetup";
		this.menu_frm_WHSubSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WHSubSetup_ItemClick);
		this.menu_frm_WHSubManager.Caption = "Warehouse Sublevel Code Management";
		this.menu_frm_WHSubManager.Id = 547;
		this.menu_frm_WHSubManager.Name = "menu_frm_WHSubManager";
		this.menu_frm_WHSubManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WHSubManager_ItemClick);
		this.barSub_SetupHumanResouces.Caption = "Human Resources";
		this.barSub_SetupHumanResouces.Description = "20380";
		this.barSub_SetupHumanResouces.Id = 334;
		this.barSub_SetupHumanResouces.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BiometricConfiguration)
		});
		this.barSub_SetupHumanResouces.Name = "barSub_SetupHumanResouces";
		this.barSubItem2.Caption = "Job Vacancy";
		this.barSubItem2.Description = "20381";
		this.barSubItem2.Id = 335;
		this.barSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_QualificationsGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Qualifications),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JobType),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvertisementSource),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JVApplicants),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JVInterviewStatus),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JVInterviewTypes),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JVQuestion),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JVBenefits)
		});
		this.barSubItem2.Name = "barSubItem2";
		this.menu_frm_QualificationsGroup.Caption = "Qualifications Group";
		this.menu_frm_QualificationsGroup.Description = "20382";
		this.menu_frm_QualificationsGroup.Id = 336;
		this.menu_frm_QualificationsGroup.Name = "menu_frm_QualificationsGroup";
		this.menu_frm_QualificationsGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frmQualificationsGroup_ItemClick);
		this.menu_frm_Qualifications.Caption = "Qualifications";
		this.menu_frm_Qualifications.Description = "20383";
		this.menu_frm_Qualifications.Id = 337;
		this.menu_frm_Qualifications.Name = "menu_frm_Qualifications";
		this.menu_frm_Qualifications.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frmQualifications_ItemClick);
		this.menu_frm_JobType.Caption = "Job Types";
		this.menu_frm_JobType.Description = "20384";
		this.menu_frm_JobType.Id = 338;
		this.menu_frm_JobType.Name = "menu_frm_JobType";
		this.menu_frm_JobType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frmJobTypes_ItemClick);
		this.menu_frm_AdvertisementSource.Caption = "Advertisement Source";
		this.menu_frm_AdvertisementSource.Description = "20385";
		this.menu_frm_AdvertisementSource.Id = 339;
		this.menu_frm_AdvertisementSource.Name = "menu_frm_AdvertisementSource";
		this.menu_frm_AdvertisementSource.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frmIdvSource_ItemClick);
		this.menu_frm_JVApplicants.Caption = "Applicants";
		this.menu_frm_JVApplicants.Description = "20386";
		this.menu_frm_JVApplicants.Id = 340;
		this.menu_frm_JVApplicants.Name = "menu_frm_JVApplicants";
		this.menu_frm_JVApplicants.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JVApplicants_ItemClick);
		this.menu_frm_JVInterviewStatus.Caption = "Interview Status";
		this.menu_frm_JVInterviewStatus.Id = 341;
		this.menu_frm_JVInterviewStatus.Name = "menu_frm_JVInterviewStatus";
		this.menu_frm_JVInterviewStatus.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InterviewStatus_ItemClick);
		this.menu_frm_JVInterviewTypes.Caption = "Interview Types";
		this.menu_frm_JVInterviewTypes.Description = "20388";
		this.menu_frm_JVInterviewTypes.Id = 342;
		this.menu_frm_JVInterviewTypes.Name = "menu_frm_JVInterviewTypes";
		this.menu_frm_JVInterviewTypes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JVInterviewTypes_ItemClick);
		this.menu_frm_JVQuestion.Caption = "Question library";
		this.menu_frm_JVQuestion.Description = "20389";
		this.menu_frm_JVQuestion.Id = 343;
		this.menu_frm_JVQuestion.Name = "menu_frm_JVQuestion";
		this.menu_frm_JVQuestion.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JVQuestion_ItemClick);
		this.menu_frm_JVBenefits.Caption = "Benefit";
		this.menu_frm_JVBenefits.Description = "20404";
		this.menu_frm_JVBenefits.Id = 344;
		this.menu_frm_JVBenefits.Name = "menu_frm_JVBenefits";
		this.menu_frm_JVBenefits.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JVBenefits_ItemClick);
		this.barSubItem4.Caption = "Time Sheeting";
		this.barSubItem4.Description = "20405";
		this.barSubItem4.Id = 346;
		this.barSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[10]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkType),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkCode),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkPeriod),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_OvertimeType),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_HolidaySetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InOutMethod),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkShift),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkScheduleSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkScheduleRegister),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AboutRangeTime)
		});
		this.barSubItem4.Name = "barSubItem4";
		this.menu_frm_WorkType.Caption = "Work Type";
		this.menu_frm_WorkType.Description = "20406";
		this.menu_frm_WorkType.Id = 347;
		this.menu_frm_WorkType.Name = "menu_frm_WorkType";
		this.menu_frm_WorkType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkType_ItemClick);
		this.menu_frm_WorkCode.Caption = "Work Code";
		this.menu_frm_WorkCode.Description = "20407";
		this.menu_frm_WorkCode.Id = 348;
		this.menu_frm_WorkCode.Name = "menu_frm_WorkCode";
		this.menu_frm_WorkCode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkCode_ItemClick);
		this.menu_frm_WorkPeriod.Caption = "Work Period";
		this.menu_frm_WorkPeriod.Description = "20408";
		this.menu_frm_WorkPeriod.Id = 352;
		this.menu_frm_WorkPeriod.Name = "menu_frm_WorkPeriod";
		this.menu_frm_WorkPeriod.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkPeriod_ItemClick);
		this.menu_frm_OvertimeType.Caption = "Overtime Type";
		this.menu_frm_OvertimeType.Description = "20409";
		this.menu_frm_OvertimeType.Id = 353;
		this.menu_frm_OvertimeType.Name = "menu_frm_OvertimeType";
		this.menu_frm_OvertimeType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_OvertimeType_ItemClick);
		this.menu_frm_HolidaySetup.Caption = "Holidays";
		this.menu_frm_HolidaySetup.Id = 511;
		this.menu_frm_HolidaySetup.Name = "menu_frm_HolidaySetup";
		this.menu_frm_HolidaySetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_HolidaySetup_ItemClick);
		this.menu_frm_InOutMethod.Caption = "In/Out Pickup Method";
		this.menu_frm_InOutMethod.Id = 512;
		this.menu_frm_InOutMethod.Name = "menu_frm_InOutMethod";
		this.menu_frm_InOutMethod.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InOutMethod_ItemClick);
		this.menu_frm_WorkShift.Caption = "Work Shift";
		this.menu_frm_WorkShift.Id = 513;
		this.menu_frm_WorkShift.Name = "menu_frm_WorkShift";
		this.menu_frm_WorkShift.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkShift_ItemClick);
		this.menu_frm_WorkScheduleSetup.Caption = "Work Schedule Setup";
		this.menu_frm_WorkScheduleSetup.Id = 514;
		this.menu_frm_WorkScheduleSetup.Name = "menu_frm_WorkScheduleSetup";
		this.menu_frm_WorkScheduleSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkScheduleSetup_ItemClick);
		this.menu_frm_WorkScheduleRegister.Caption = "Schedule Register";
		this.menu_frm_WorkScheduleRegister.Id = 515;
		this.menu_frm_WorkScheduleRegister.Name = "menu_frm_WorkScheduleRegister";
		this.menu_frm_WorkScheduleRegister.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkScheduleRegister_ItemClick);
		this.menu_frm_AboutRangeTime.Caption = "About Range Time";
		this.menu_frm_AboutRangeTime.Id = 594;
		this.menu_frm_AboutRangeTime.Name = "menu_frm_AboutRangeTime";
		this.menu_frm_AboutRangeTime.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AboutRangeTime_ItemClick);
		this.barSubItem6.Caption = "Salary";
		this.barSubItem6.Description = "20410";
		this.barSubItem6.Id = 354;
		this.barSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AllowanceType),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DeductionType),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PersonalIncomeTax),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalaryParameters),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LeaveParameter),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PenalType)
		});
		this.barSubItem6.Name = "barSubItem6";
		this.menu_frm_AllowanceType.Caption = "Allowance Type";
		this.menu_frm_AllowanceType.Description = "20411";
		this.menu_frm_AllowanceType.Id = 355;
		this.menu_frm_AllowanceType.Name = "menu_frm_AllowanceType";
		this.menu_frm_AllowanceType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AllowanceType_ItemClick);
		this.menu_frm_DeductionType.Caption = "Deduction Type";
		this.menu_frm_DeductionType.Description = "20412";
		this.menu_frm_DeductionType.Id = 356;
		this.menu_frm_DeductionType.Name = "menu_frm_DeductionType";
		this.menu_frm_DeductionType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DeductionType_ItemClick);
		this.menu_frm_PersonalIncomeTax.Caption = "Personal Income Tax Level";
		this.menu_frm_PersonalIncomeTax.Description = "20413";
		this.menu_frm_PersonalIncomeTax.Id = 357;
		this.menu_frm_PersonalIncomeTax.Name = "menu_frm_PersonalIncomeTax";
		this.menu_frm_PersonalIncomeTax.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PersonalIncomeTax_ItemClick);
		this.menu_frm_SalaryParameters.Caption = "Salary Paramters";
		this.menu_frm_SalaryParameters.Description = "20414";
		this.menu_frm_SalaryParameters.Id = 358;
		this.menu_frm_SalaryParameters.Name = "menu_frm_SalaryParameters";
		this.menu_frm_SalaryParameters.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalaryParameters_ItemClick);
		this.menu_frm_LeaveParameter.Caption = "Leave Parameter";
		this.menu_frm_LeaveParameter.Id = 413;
		this.menu_frm_LeaveParameter.Name = "menu_frm_LeaveParameter";
		this.menu_frm_LeaveParameter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LeaveParameter_ItemClick);
		this.menu_frm_PenalType.Caption = "Penalty/Bonus Type";
		this.menu_frm_PenalType.Id = 406;
		this.menu_frm_PenalType.Name = "menu_frm_PenalType";
		this.menu_frm_PenalType.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PenalType_ItemClick);
		this.menu_frm_BiometricConfiguration.Caption = "Biometric Devices Config";
		this.menu_frm_BiometricConfiguration.Id = 505;
		this.menu_frm_BiometricConfiguration.Name = "menu_frm_BiometricConfiguration";
		this.menu_frm_BiometricConfiguration.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BiometricConfiguration_ItemClick);
		this.barSub_SetupReport.Caption = "Report";
		this.barSub_SetupReport.Description = "20390";
		this.barSub_SetupReport.Id = 163;
		this.barSub_SetupReport.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PrintLayout),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ReportManager)
		});
		this.barSub_SetupReport.Name = "barSub_SetupReport";
		this.menu_frm_PrintLayout.Caption = "Layout";
		this.menu_frm_PrintLayout.Id = 164;
		this.menu_frm_PrintLayout.Name = "menu_frm_PrintLayout";
		this.menu_frm_PrintLayout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PrintLayout_ItemClick);
		this.menu_frm_ReportManager.Caption = "Report Manager";
		this.menu_frm_ReportManager.Id = 321;
		this.menu_frm_ReportManager.Name = "menu_frm_ReportManager";
		this.menu_frm_ReportManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_XtraReportManager_ItemClick);
		this.barSub_Opportunities.Caption = "Opportunities";
		this.barSub_Opportunities.Id = 400;
		this.barSub_Opportunities.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunityStage),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunityPartner),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunityCompetitor),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunityRelationship),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunityReason)
		});
		this.barSub_Opportunities.Name = "barSub_Opportunities";
		this.menu_frm_SalesOpportunityStage.Caption = "Opportunity Stages";
		this.menu_frm_SalesOpportunityStage.Id = 401;
		this.menu_frm_SalesOpportunityStage.Name = "menu_frm_SalesOpportunityStage";
		this.menu_frm_SalesOpportunityStage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunityStage_ItemClick);
		this.menu_frm_SalesOpportunityPartner.Caption = "Partners";
		this.menu_frm_SalesOpportunityPartner.Id = 402;
		this.menu_frm_SalesOpportunityPartner.Name = "menu_frm_SalesOpportunityPartner";
		this.menu_frm_SalesOpportunityPartner.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunityPartner_ItemClick);
		this.menu_frm_SalesOpportunityCompetitor.Caption = "Competitors";
		this.menu_frm_SalesOpportunityCompetitor.Id = 403;
		this.menu_frm_SalesOpportunityCompetitor.Name = "menu_frm_SalesOpportunityCompetitor";
		this.menu_frm_SalesOpportunityCompetitor.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunityCompetitor_ItemClick);
		this.menu_frm_SalesOpportunityRelationship.Caption = "Relationships";
		this.menu_frm_SalesOpportunityRelationship.Id = 404;
		this.menu_frm_SalesOpportunityRelationship.Name = "menu_frm_SalesOpportunityRelationship";
		this.menu_frm_SalesOpportunityRelationship.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunityRelationship_ItemClick);
		this.menu_frm_SalesOpportunityReason.Caption = "Reasons";
		this.menu_frm_SalesOpportunityReason.Id = 405;
		this.menu_frm_SalesOpportunityReason.Name = "menu_frm_SalesOpportunityReason";
		this.menu_frm_SalesOpportunityReason.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunityReason_ItemClick);
		this.barSubItem1.Caption = "Service";
		this.barSubItem1.Id = 428;
		this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ContractTemplate)
		});
		this.barSubItem1.Name = "barSubItem1";
		this.menu_frm_ContractTemplate.Caption = "Contract Templates";
		this.menu_frm_ContractTemplate.Id = 429;
		this.menu_frm_ContractTemplate.Name = "menu_frm_ContractTemplate";
		this.menu_frm_ContractTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ContractTemplate_ItemClick);
		this.barSub_ProjectManagement.Caption = "Project Management";
		this.barSub_ProjectManagement.Id = 506;
		this.barSub_ProjectManagement.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WBSTemplate)
		});
		this.barSub_ProjectManagement.Name = "barSub_ProjectManagement";
		this.menu_frm_WBSTemplate.Caption = "WBSTemplate";
		this.menu_frm_WBSTemplate.Id = 507;
		this.menu_frm_WBSTemplate.Name = "menu_frm_WBSTemplate";
		this.menu_frm_WBSTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WBSTemplate_ItemClick);
		this.menu_frm_AdditionalGeneralSettings.Caption = "Additional General Settings";
		this.menu_frm_AdditionalGeneralSettings.Id = 517;
		this.menu_frm_AdditionalGeneralSettings.Name = "menu_frm_AdditionalGeneralSettings";
		this.menu_frm_AdditionalGeneralSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdditionalGeneralSettings_ItemClick);
		this.menu_frm_ThirdPartyToken.Caption = "Third Party Token";
		this.menu_frm_ThirdPartyToken.Id = 583;
		this.menu_frm_ThirdPartyToken.Name = "menu_frm_ThirdPartyToken";
		this.menu_frm_ThirdPartyToken.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ThirdPartyToken_ItemClick);
		this.barSub_Authorization.Caption = "Authorization";
		this.barSub_Authorization.Description = "20400";
		this.barSub_Authorization.Id = 149;
		this.barSub_Authorization.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Authorization),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DataOwnerShipAuthorization)
		});
		this.barSub_Authorization.Name = "barSub_Authorization";
		this.menu_frm_Authorization.Caption = "General Authorization";
		this.menu_frm_Authorization.Description = "20415";
		this.menu_frm_Authorization.Id = 150;
		this.menu_frm_Authorization.Name = "menu_frm_Authorization";
		this.menu_frm_Authorization.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Authorization_ItemClick);
		this.menu_frm_DataOwnerShipAuthorization.Caption = "Data Ownership Authorization";
		this.menu_frm_DataOwnerShipAuthorization.Description = "20416";
		this.menu_frm_DataOwnerShipAuthorization.Id = 151;
		this.menu_frm_DataOwnerShipAuthorization.Name = "menu_frm_DataOwnerShipAuthorization";
		this.menu_frm_DataOwnerShipAuthorization.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DataOwnerShipAuthorization_ItemClick);
		this.barSub_SystemInitialization.Caption = "System Initialization";
		this.barSub_SystemInitialization.Description = "20600";
		this.barSub_SystemInitialization.Id = 238;
		this.barSub_SystemInitialization.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DocumentNumbering),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DocumentSettings),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MenuStructure)
		});
		this.barSub_SystemInitialization.Name = "barSub_SystemInitialization";
		this.menu_frm_DocumentNumbering.Caption = "Document Numbering";
		this.menu_frm_DocumentNumbering.Description = "20601";
		this.menu_frm_DocumentNumbering.Id = 239;
		this.menu_frm_DocumentNumbering.Name = "menu_frm_DocumentNumbering";
		this.menu_frm_DocumentNumbering.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem20_ItemClick_1);
		this.menu_frm_DocumentSettings.Caption = "Document Settings";
		this.menu_frm_DocumentSettings.Description = "20602";
		this.menu_frm_DocumentSettings.Id = 328;
		this.menu_frm_DocumentSettings.Name = "menu_frm_DocumentSettings";
		this.menu_frm_DocumentSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DocumentSettings_ItemClick_1);
		this.menu_frm_MenuStructure.Caption = "Menu Structure";
		this.menu_frm_MenuStructure.Id = 509;
		this.menu_frm_MenuStructure.Name = "menu_frm_MenuStructure";
		this.menu_frm_MenuStructure.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MenuStructure_ItemClick);
		this.barSub_ApprovalProcess.Caption = "Approval Process";
		this.barSub_ApprovalProcess.Id = 571;
		this.barSub_ApprovalProcess.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ApprovalStages),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ApprovalTemplates),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WorkingProcess)
		});
		this.barSub_ApprovalProcess.Name = "barSub_ApprovalProcess";
		this.menu_frm_ApprovalStages.Caption = "Approval Stages";
		this.menu_frm_ApprovalStages.Id = 572;
		this.menu_frm_ApprovalStages.Name = "menu_frm_ApprovalStages";
		this.menu_frm_ApprovalStages.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ApprovalStages_ItemClick);
		this.menu_frm_ApprovalTemplates.Caption = "Approval Templates";
		this.menu_frm_ApprovalTemplates.Id = 573;
		this.menu_frm_ApprovalTemplates.Name = "menu_frm_ApprovalTemplates";
		this.menu_frm_ApprovalTemplates.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ApprovalTemplates_ItemClick);
		this.menu_frm_WorkingProcess.Caption = "Working Process";
		this.menu_frm_WorkingProcess.Id = 535;
		this.menu_frm_WorkingProcess.Name = "menu_frm_WorkingProcess";
		this.menu_frm_WorkingProcess.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WorkingProcess_ItemClick);
		this.menu_frm_EmpToken.Caption = "Employee Token";
		this.menu_frm_EmpToken.Id = 282;
		this.menu_frm_EmpToken.Name = "menu_frm_EmpToken";
		this.menu_frm_EmpToken.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_EmpToken_ItemClick);
		this.barSub_PlugIn.Caption = "Plugin";
		this.barSub_PlugIn.Description = "20800";
		this.barSub_PlugIn.Id = 391;
		this.barSub_PlugIn.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PluginManager)
		});
		this.barSub_PlugIn.Name = "barSub_PlugIn";
		this.barSub_PlugIn.Tag = "FPlugIn";
		this.menu_frm_PluginManager.Caption = "Plugin Manager";
		this.menu_frm_PluginManager.Description = "20801";
		this.menu_frm_PluginManager.Id = 392;
		this.menu_frm_PluginManager.Name = "menu_frm_PluginManager";
		this.menu_frm_PluginManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PluginManager_ItemClick);
		this.menu_frm_AccessLog.Caption = "Access Log";
		this.menu_frm_AccessLog.Id = 596;
		this.menu_frm_AccessLog.Name = "menu_frm_AccessLog";
		this.menu_frm_AccessLog.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_AccessLog_ItemClick);
		this.barSub_Dashboard.Caption = "Dashboard";
		this.barSub_Dashboard.Id = 271;
		this.barSub_Dashboard.ImageOptions.ImageIndex = 4;
		this.barSub_Dashboard.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DashboardManager),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DashboardDesigner),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WidgetManagement, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ListDashboard)
		});
		this.barSub_Dashboard.Name = "barSub_Dashboard";
		this.menu_frm_DashboardManager.Caption = "Dashboard Management";
		this.menu_frm_DashboardManager.Id = 409;
		this.menu_frm_DashboardManager.Name = "menu_frm_DashboardManager";
		this.menu_frm_DashboardManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.frm_DashboardManagement_ItemClick);
		this.menu_frm_DashboardDesigner.Caption = "Dashboard Designer";
		this.menu_frm_DashboardDesigner.Id = 412;
		this.menu_frm_DashboardDesigner.Name = "menu_frm_DashboardDesigner";
		this.menu_frm_DashboardDesigner.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DashboardDesigner_ItemClick);
		this.menu_frm_WidgetManagement.Caption = "Wiget Management";
		this.menu_frm_WidgetManagement.Id = 456;
		this.menu_frm_WidgetManagement.Name = "menu_frm_WidgetManagement";
		this.menu_frm_WidgetManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WidgetManagement_ItemClick);
		this.barSub_SalesAR.Caption = "Sales";
		this.barSub_SalesAR.Description = "22000";
		this.barSub_SalesAR.Id = 45;
		this.barSub_SalesAR.ImageOptions.ImageIndex = 6;
		this.barSub_SalesAR.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[22]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesBlanketAgreement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesQuotationForBatch),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOrder),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesControl),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DeliveryControl),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Delivery),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ReturnRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Return),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DownPaymentRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DownPaymentInvoice),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AR_ReserveInvoices),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ARInvoices),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ARCreditMemos),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Atp),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DeliveryOrder),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DeliveryConfirm),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_HandoverDocument),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AvailableToPromise),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenPurchaseFromAtp),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ScheduleForAtp),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenProductionFormAtp),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SalesReports)
		});
		this.barSub_SalesAR.Name = "barSub_SalesAR";
		this.menu_frm_SalesBlanketAgreement.Caption = "Sales Blanket Agreement";
		this.menu_frm_SalesBlanketAgreement.Id = 420;
		this.menu_frm_SalesBlanketAgreement.Name = "menu_frm_SalesBlanketAgreement";
		this.menu_frm_SalesBlanketAgreement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesBlanketAgreement_ItemClick);
		this.menu_frm_SalesQuotationForBatch.Caption = "Sales Quotation";
		this.menu_frm_SalesQuotationForBatch.Description = "22001";
		this.menu_frm_SalesQuotationForBatch.Id = 80;
		this.menu_frm_SalesQuotationForBatch.Name = "menu_frm_SalesQuotationForBatch";
		this.menu_frm_SalesQuotationForBatch.Tag = "SalesQuotation";
		this.menu_frm_SalesQuotationForBatch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_SQ_ItemClick);
		this.menu_frm_SalesOrder.Caption = "Sales Order";
		this.menu_frm_SalesOrder.Description = "22002";
		this.menu_frm_SalesOrder.Id = 82;
		this.menu_frm_SalesOrder.Name = "menu_frm_SalesOrder";
		this.menu_frm_SalesOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_SO_ItemClick);
		this.menu_frm_SalesControl.Caption = "Sales Control";
		this.menu_frm_SalesControl.Description = "22003";
		this.menu_frm_SalesControl.Id = 283;
		this.menu_frm_SalesControl.Name = "menu_frm_SalesControl";
		this.menu_frm_SalesControl.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesControl_ItemClick);
		this.menu_frm_DeliveryControl.Caption = "Delivery Control";
		this.menu_frm_DeliveryControl.Description = "22004";
		this.menu_frm_DeliveryControl.Id = 284;
		this.menu_frm_DeliveryControl.Name = "menu_frm_DeliveryControl";
		this.menu_frm_DeliveryControl.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DeliveryControl_ItemClick);
		this.menu_frm_Delivery.Caption = "Delivery";
		this.menu_frm_Delivery.Description = "22005";
		this.menu_frm_Delivery.Id = 83;
		this.menu_frm_Delivery.Name = "menu_frm_Delivery";
		this.menu_frm_Delivery.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Delivery_ItemClick);
		this.menu_frm_ReturnRequest.Caption = "Return Request";
		this.menu_frm_ReturnRequest.Id = 582;
		this.menu_frm_ReturnRequest.Name = "menu_frm_ReturnRequest";
		this.menu_frm_ReturnRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ReturnRequest_ItemClick);
		this.menu_frm_Return.Caption = "Return";
		this.menu_frm_Return.Description = "22007";
		this.menu_frm_Return.Id = 85;
		this.menu_frm_Return.Name = "menu_frm_Return";
		this.menu_frm_Return.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Return_ItemClick);
		this.menu_frm_DownPaymentRequest.Caption = "A/R Down Payment Request";
		this.menu_frm_DownPaymentRequest.Id = 492;
		this.menu_frm_DownPaymentRequest.Name = "menu_frm_DownPaymentRequest";
		this.menu_frm_DownPaymentRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.frm_DownPaymentRequest_ItemClick);
		this.menu_frm_DownPaymentInvoice.Caption = "A/R Down Payment Invoice";
		this.menu_frm_DownPaymentInvoice.Id = 492;
		this.menu_frm_DownPaymentInvoice.Name = "menu_frm_DownPaymentInvoice";
		this.menu_frm_DownPaymentInvoice.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.frm_DownPaymentInvoice_ItemClick);
		this.menu_frm_AR_ReserveInvoices.Caption = "A/R Reserve Invoice";
		this.menu_frm_AR_ReserveInvoices.Description = "22008";
		this.menu_frm_AR_ReserveInvoices.Id = 325;
		this.menu_frm_AR_ReserveInvoices.Name = "menu_frm_AR_ReserveInvoices";
		this.menu_frm_AR_ReserveInvoices.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AR_ReserveInvoices_ItemClick);
		this.menu_frm_ARInvoices.Caption = "A/R Invoice";
		this.menu_frm_ARInvoices.Description = "22009";
		this.menu_frm_ARInvoices.Id = 86;
		this.menu_frm_ARInvoices.Name = "menu_frm_ARInvoices";
		this.menu_frm_ARInvoices.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ARI_ItemClick);
		this.menu_frm_ARCreditMemos.Caption = "A/R Credit Memo";
		this.menu_frm_ARCreditMemos.Description = "22010";
		this.menu_frm_ARCreditMemos.Id = 87;
		this.menu_frm_ARCreditMemos.Name = "menu_frm_ARCreditMemos";
		this.menu_frm_ARCreditMemos.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ARCM_ItemClick);
		this.menu_frm_Atp.Caption = "Available to Promise";
		this.menu_frm_Atp.Description = "22012";
		this.menu_frm_Atp.Id = 90;
		this.menu_frm_Atp.Name = "menu_frm_Atp";
		this.menu_frm_Atp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_AToPromise_ItemClick);
		this.menu_frm_DeliveryOrder.Caption = "Delivery Order";
		this.menu_frm_DeliveryOrder.Description = "22013";
		this.menu_frm_DeliveryOrder.Id = 279;
		this.menu_frm_DeliveryOrder.Name = "menu_frm_DeliveryOrder";
		this.menu_frm_DeliveryConfirm.Caption = "Delivery Confirm";
		this.menu_frm_DeliveryConfirm.Description = "22014";
		this.menu_frm_DeliveryConfirm.Id = 280;
		this.menu_frm_DeliveryConfirm.Name = "menu_frm_DeliveryConfirm";
		this.menu_frm_HandoverDocument.Caption = "Document Transfer";
		this.menu_frm_HandoverDocument.Description = "22015";
		this.menu_frm_HandoverDocument.Hint = "Bàn giao chứng từ";
		this.menu_frm_HandoverDocument.Id = 281;
		this.menu_frm_HandoverDocument.Name = "menu_frm_HandoverDocument";
		this.menu_frm_AvailableToPromise.Caption = "Available To Promise";
		this.menu_frm_AvailableToPromise.Description = "22016";
		this.menu_frm_AvailableToPromise.Id = 350;
		this.menu_frm_AvailableToPromise.Name = "menu_frm_AvailableToPromise";
		this.menu_frm_AvailableToPromise.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.nemu_frm_AvailableToPromise_ItemClick);
		this.menu_frm_GenPurchaseFromAtp.Caption = "Generate Purchase From Atp";
		this.menu_frm_GenPurchaseFromAtp.Description = "22017";
		this.menu_frm_GenPurchaseFromAtp.Id = 354;
		this.menu_frm_GenPurchaseFromAtp.Name = "menu_frm_GenPurchaseFromAtp";
		this.menu_frm_GenPurchaseFromAtp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenPurchaseFromAtp_ItemClick);
		this.menu_frm_ScheduleForAtp.Caption = "Schedule For Atp";
		this.menu_frm_ScheduleForAtp.Description = "22018";
		this.menu_frm_ScheduleForAtp.Id = 360;
		this.menu_frm_ScheduleForAtp.Name = "menu_frm_ScheduleForAtp";
		this.menu_frm_ScheduleForAtp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ScheduleForAtp_ItemClick);
		this.menu_frm_GenProductionFormAtp.Caption = "Generate Production Order From Atp";
		this.menu_frm_GenProductionFormAtp.Description = "22019";
		this.menu_frm_GenProductionFormAtp.Id = 361;
		this.menu_frm_GenProductionFormAtp.Name = "menu_frm_GenProductionFormAtp";
		this.menu_frm_GenProductionFormAtp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenProductionFormAtp_ItemClick);
		this.barSub_SalesReports.Caption = "Sales Reports";
		this.barSub_SalesReports.Id = 434;
		this.barSub_SalesReports.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesDraftsReport)
		});
		this.barSub_SalesReports.Name = "barSub_SalesReports";
		this.menu_frm_SalesDraftsReport.Caption = "Document Drafts Report";
		this.menu_frm_SalesDraftsReport.Description = "22011";
		this.menu_frm_SalesDraftsReport.Id = 89;
		this.menu_frm_SalesDraftsReport.Name = "menu_frm_SalesDraftsReport";
		this.menu_frm_SalesDraftsReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_DDR_ItemClick);
		this.barSub_PurchaseAP.Caption = "Purchasing";
		this.barSub_PurchaseAP.Id = 46;
		this.barSub_PurchaseAP.ImageOptions.ImageIndex = 7;
		this.barSub_PurchaseAP.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[16]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseBlanketAgreement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseQuotation),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseOder),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodReceiptPO),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodsReceiptPOByBarcode),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodsReturnRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodsReturn),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseDownPaymentRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseDownPaymentInvoice),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_APInvoices),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_APCreditMemos),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_APReserveInvoices),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LandedCosts),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProcurementConfirm),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PurchasingReports)
		});
		this.barSub_PurchaseAP.Name = "barSub_PurchaseAP";
		this.menu_frm_PurchaseBlanketAgreement.Caption = "Purchase Blanket Agreement";
		this.menu_frm_PurchaseBlanketAgreement.Id = 483;
		this.menu_frm_PurchaseBlanketAgreement.Name = "menu_frm_PurchaseBlanketAgreement";
		this.menu_frm_PurchaseBlanketAgreement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseBlanketAgreement_ItemClick);
		this.menu_frm_PurchaseRequest.Caption = "Purchase Request";
		this.menu_frm_PurchaseRequest.Id = 196;
		this.menu_frm_PurchaseRequest.Name = "menu_frm_PurchaseRequest";
		this.menu_frm_PurchaseRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseRequest_ItemClick);
		this.menu_frm_PurchaseQuotation.Caption = "Purchase Quotation";
		this.menu_frm_PurchaseQuotation.Id = 197;
		this.menu_frm_PurchaseQuotation.Name = "menu_frm_PurchaseQuotation";
		this.menu_frm_PurchaseQuotation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseQuotation_ItemClick);
		this.menu_frm_PurchaseOder.Caption = "Purchase Order";
		this.menu_frm_PurchaseOder.Id = 198;
		this.menu_frm_PurchaseOder.Name = "menu_frm_PurchaseOder";
		this.menu_frm_PurchaseOder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseOder_ItemClick);
		this.menu_frm_GoodReceiptPO.Caption = "Goods Receipt PO";
		this.menu_frm_GoodReceiptPO.Id = 199;
		this.menu_frm_GoodReceiptPO.Name = "menu_frm_GoodReceiptPO";
		this.menu_frm_GoodReceiptPO.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GoodReceiptPO_ItemClick);
		this.menu_frm_GoodsReceiptPOByBarcode.Caption = "Goods Receipt PO by Barcode";
		this.menu_frm_GoodsReceiptPOByBarcode.Id = 538;
		this.menu_frm_GoodsReceiptPOByBarcode.Name = "menu_frm_GoodsReceiptPOByBarcode";
		this.menu_frm_GoodsReceiptPOByBarcode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_GoodsReturnRequest.Caption = "Goods Return Request";
		this.menu_frm_GoodsReturnRequest.Id = 598;
		this.menu_frm_GoodsReturnRequest.Name = "menu_frm_GoodsReturnRequest";
		this.menu_frm_GoodsReturnRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem43_ItemClick);
		this.menu_frm_GoodsReturn.Caption = "Goods Return";
		this.menu_frm_GoodsReturn.Id = 200;
		this.menu_frm_GoodsReturn.Name = "menu_frm_GoodsReturn";
		this.menu_frm_GoodsReturn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GoodsReturn_ItemClick);
		this.menu_frm_PurchaseDownPaymentRequest.Caption = "A/P Down Payment Request";
		this.menu_frm_PurchaseDownPaymentRequest.Id = 493;
		this.menu_frm_PurchaseDownPaymentRequest.Name = "menu_frm_PurchaseDownPaymentRequest";
		this.menu_frm_PurchaseDownPaymentRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseDownPaymentRequest_ItemClick);
		this.menu_frm_PurchaseDownPaymentInvoice.Caption = "A/P Down Payment Invoice";
		this.menu_frm_PurchaseDownPaymentInvoice.Id = 493;
		this.menu_frm_PurchaseDownPaymentInvoice.Name = "menu_frm_PurchaseDownPaymentInvoice";
		this.menu_frm_PurchaseDownPaymentInvoice.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseDownPaymentInvoice_ItemClick);
		this.menu_frm_APInvoices.Caption = "A/P Invoice";
		this.menu_frm_APInvoices.Id = 201;
		this.menu_frm_APInvoices.Name = "menu_frm_APInvoices";
		this.menu_frm_APInvoices.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_APInvoices_ItemClick);
		this.menu_frm_APCreditMemos.Caption = "A/P Credit Memo";
		this.menu_frm_APCreditMemos.Id = 202;
		this.menu_frm_APCreditMemos.Name = "menu_frm_APCreditMemos";
		this.menu_frm_APCreditMemos.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_APCreditMemos_ItemClick);
		this.menu_frm_APReserveInvoices.Caption = "A/P Reserve Invoice";
		this.menu_frm_APReserveInvoices.Id = 203;
		this.menu_frm_APReserveInvoices.Name = "menu_frm_APReserveInvoices";
		this.menu_frm_APReserveInvoices.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_APReserveInvoice_ItemClick);
		this.menu_frm_LandedCosts.Caption = "Landed Cost";
		this.menu_frm_LandedCosts.Id = 204;
		this.menu_frm_LandedCosts.Name = "menu_frm_LandedCosts";
		this.menu_frm_LandedCosts.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LandedCosts_ItemClick);
		this.menu_frm_ProcurementConfirm.Caption = "Procurement Confirm";
		this.menu_frm_ProcurementConfirm.Id = 205;
		this.menu_frm_ProcurementConfirm.Name = "menu_frm_ProcurementConfirm";
		this.menu_frm_ProcurementConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProcurementConfirm_ItemClick);
		this.barSub_PurchasingReports.Caption = "Purchasing Reports";
		this.barSub_PurchasingReports.Id = 435;
		this.barSub_PurchasingReports.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PurchaseDraftReport)
		});
		this.barSub_PurchasingReports.Name = "barSub_PurchasingReports";
		this.menu_frm_PurchaseDraftReport.Caption = "Document Drafts Report";
		this.menu_frm_PurchaseDraftReport.Id = 449;
		this.menu_frm_PurchaseDraftReport.Name = "menu_frm_PurchaseDraftReport";
		this.menu_frm_PurchaseDraftReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PurchaseDraftReport_ItemClick);
		this.barSub_BP.Caption = "Business Partners";
		this.barSub_BP.Id = 48;
		this.barSub_BP.ImageOptions.ImageIndex = 8;
		this.barSub_BP.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BusinessMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ActivitiesForOpportunity),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_FacebookCRM),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BPReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_QuickCreationBPMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_FacebookManager)
		});
		this.barSub_BP.Name = "barSub_BP";
		this.menu_frm_BusinessMasterData.Caption = "Business Partner Master Data";
		this.menu_frm_BusinessMasterData.Id = 78;
		this.menu_frm_BusinessMasterData.Name = "menu_frm_BusinessMasterData";
		this.menu_frm_BusinessMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_BP_ItemClick);
		this.menu_frm_ActivitiesForOpportunity.Caption = "Activity";
		this.menu_frm_ActivitiesForOpportunity.Id = 79;
		this.menu_frm_ActivitiesForOpportunity.Name = "menu_frm_ActivitiesForOpportunity";
		this.menu_frm_ActivitiesForOpportunity.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Activity_ItemClick);
		this.menu_frm_FacebookCRM.Caption = "Facebook CRM";
		this.menu_frm_FacebookCRM.Id = 417;
		this.menu_frm_FacebookCRM.Name = "menu_frm_FacebookCRM";
		this.menu_frm_FacebookCRM.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_FacebookCRM_ItemClick);
		this.barSub_BPReports.Caption = "Business Partner Reports";
		this.barSub_BPReports.Id = 436;
		this.barSub_BPReports.Name = "barSub_BPReports";
		this.menu_frm_QuickCreationBPMasterData.Caption = "Quick Creation Business Partner Master Data";
		this.menu_frm_QuickCreationBPMasterData.Id = 519;
		this.menu_frm_QuickCreationBPMasterData.Name = "menu_frm_QuickCreationBPMasterData";
		this.menu_frm_QuickCreationBPMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_QuickCreationBPMasterData_ItemClick);
		this.menu_frm_FacebookManager.Caption = "Facebook Chanel Management";
		this.menu_frm_FacebookManager.Id = 536;
		this.menu_frm_FacebookManager.Name = "menu_frm_FacebookManager";
		this.menu_frm_FacebookManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_FacebookManager_ItemClick);
		this.barSub_Inventory.Caption = "Inventory";
		this.barSub_Inventory.Id = 49;
		this.barSub_Inventory.ImageOptions.ImageIndex = 9;
		this.barSub_Inventory.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ItemMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_InventoryTransaction),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ItemManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BusinessPartnerCatalogNumbers),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PriceLists),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PickAndPack),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_InventoryReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BinLocation)
		});
		this.barSub_Inventory.Name = "barSub_Inventory";
		this.menu_frm_ItemMasterData.Caption = "Items Master Data";
		this.menu_frm_ItemMasterData.Id = 206;
		this.menu_frm_ItemMasterData.Name = "menu_frm_ItemMasterData";
		this.menu_frm_ItemMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ItemMasterData_ItemClick);
		this.barSub_InventoryTransaction.Caption = "Inventory Transaction";
		this.barSub_InventoryTransaction.Id = 207;
		this.barSub_InventoryTransaction.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[13]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodsReceipt),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ReceiptBin),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GoodsIssue),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryTranferRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryTranfer),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryTransferByBarcode),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryCountingBin),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryRevaluation),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_StockRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PickAndPack),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryCountingRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryCounting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BarcodeInventoryCounting)
		});
		this.barSub_InventoryTransaction.Name = "barSub_InventoryTransaction";
		this.menu_frm_GoodsReceipt.Caption = "Goods Receipt";
		this.menu_frm_GoodsReceipt.Id = 208;
		this.menu_frm_GoodsReceipt.Name = "menu_frm_GoodsReceipt";
		this.menu_frm_GoodsReceipt.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GoodsReceiptIT_ItemClick);
		this.menu_frm_ReceiptBin.Caption = "Goods Receipt - Bin";
		this.menu_frm_ReceiptBin.Id = 285;
		this.menu_frm_ReceiptBin.Name = "menu_frm_ReceiptBin";
		this.menu_frm_ReceiptBin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ReceiptBin_ItemClick);
		this.menu_frm_GoodsIssue.Caption = "Goods Issue";
		this.menu_frm_GoodsIssue.Id = 209;
		this.menu_frm_GoodsIssue.Name = "menu_frm_GoodsIssue";
		this.menu_frm_GoodsIssue.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GoodsIssueIT_ItemClick);
		this.menu_frm_InventoryTranferRequest.Caption = "Inventory Transfer Request";
		this.menu_frm_InventoryTranferRequest.Id = 210;
		this.menu_frm_InventoryTranferRequest.Name = "menu_frm_InventoryTranferRequest";
		this.menu_frm_InventoryTranferRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryTranferRequest_ItemClick);
		this.menu_frm_InventoryTranfer.Caption = "Inventory Transfer";
		this.menu_frm_InventoryTranfer.Id = 211;
		this.menu_frm_InventoryTranfer.Name = "menu_frm_InventoryTranfer";
		this.menu_frm_InventoryTranfer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryTranfer_ItemClick);
		this.menu_frm_InventoryTransferByBarcode.Caption = "Inventory Transfer by Barcode";
		this.menu_frm_InventoryTransferByBarcode.Id = 539;
		this.menu_frm_InventoryTransferByBarcode.Name = "menu_frm_InventoryTransferByBarcode";
		this.menu_frm_InventoryTransferByBarcode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_InventoryCountingBin.Caption = "Inventory Counting Bin";
		this.menu_frm_InventoryCountingBin.Id = 427;
		this.menu_frm_InventoryCountingBin.Name = "menu_frm_InventoryCountingBin";
		this.menu_frm_InventoryCountingBin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryCountingBin_ItemClick);
		this.menu_frm_InventoryRevaluation.Caption = "Inventory Revaluation";
		this.menu_frm_InventoryRevaluation.Id = 212;
		this.menu_frm_InventoryRevaluation.Name = "menu_frm_InventoryRevaluation";
		this.menu_frm_InventoryRevaluation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryRevaluation_ItemClick);
		this.menu_frm_StockRequest.Caption = "Stock Request";
		this.menu_frm_StockRequest.Id = 256;
		this.menu_frm_StockRequest.Name = "menu_frm_StockRequest";
		this.menu_frm_StockRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_StockRequest_ItemClick);
		this.menu_frm_PickAndPack.Caption = "Pick And Pack";
		this.menu_frm_PickAndPack.Id = 286;
		this.menu_frm_PickAndPack.Name = "menu_frm_PickAndPack";
		this.menu_frm_PickAndPack.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PickAndPack_ItemClick);
		this.menu_frm_InventoryCountingRequest.Caption = "Inventory Counting Request";
		this.menu_frm_InventoryCountingRequest.Id = 466;
		this.menu_frm_InventoryCountingRequest.Name = "menu_frm_InventoryCountingRequest";
		this.menu_frm_InventoryCountingRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryCountingRequest_ItemClick);
		this.menu_frm_InventoryCounting.Caption = "Inventory Counting";
		this.menu_frm_InventoryCounting.Id = 467;
		this.menu_frm_InventoryCounting.Name = "menu_frm_InventoryCounting";
		this.menu_frm_InventoryCounting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryCounting_ItemClick);
		this.menu_frm_BarcodeInventoryCounting.Caption = "Barcode Inventory Counting";
		this.menu_frm_BarcodeInventoryCounting.Id = 516;
		this.menu_frm_BarcodeInventoryCounting.Name = "menu_frm_BarcodeInventoryCounting";
		this.menu_frm_BarcodeInventoryCounting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBarcodeInventoryCounting_ItemClick);
		this.barSub_ItemManagement.Caption = "Item Management";
		this.barSub_ItemManagement.Id = 214;
		this.barSub_ItemManagement.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BatchDetail),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SerialNumberDetail),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BatchesControl),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BatchDetailManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SerialNumberDetailManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AlternativeItems)
		});
		this.barSub_ItemManagement.Name = "barSub_ItemManagement";
		this.menu_frm_BatchDetail.Caption = "Batch Details";
		this.menu_frm_BatchDetail.Id = 322;
		this.menu_frm_BatchDetail.Name = "menu_frm_BatchDetail";
		this.menu_frm_BatchDetail.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BatchDetail_ItemClick);
		this.menu_frm_SerialNumberDetail.Caption = "Serial Number Details";
		this.menu_frm_SerialNumberDetail.Id = 324;
		this.menu_frm_SerialNumberDetail.Name = "menu_frm_SerialNumberDetail";
		this.menu_frm_SerialNumberDetail.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SerialNumberDetail_ItemClick);
		this.menu_frm_BatchesControl.Caption = "Batches Control";
		this.menu_frm_BatchesControl.Id = 332;
		this.menu_frm_BatchesControl.Name = "menu_frm_BatchesControl";
		this.menu_frm_BatchesControl.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BatchesControl_ItemClick);
		this.menu_frm_BatchDetailManagement.Caption = "Batch Details Management";
		this.menu_frm_BatchDetailManagement.Id = 481;
		this.menu_frm_BatchDetailManagement.Name = "menu_frm_BatchDetailManagement";
		this.menu_frm_BatchDetailManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BatchDetailManagement_ItemClick);
		this.menu_frm_SerialNumberDetailManagement.Caption = "Serial Details Management";
		this.menu_frm_SerialNumberDetailManagement.Id = 491;
		this.menu_frm_SerialNumberDetailManagement.Name = "menu_frm_SerialNumberDetailManagement";
		this.menu_frm_SerialNumberDetailManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SerialNumberDetailManagement_ItemClick);
		this.menu_frm_AlternativeItems.Caption = "Alternative Item";
		this.menu_frm_AlternativeItems.Id = 488;
		this.menu_frm_AlternativeItems.Name = "menu_frm_AlternativeItems";
		this.menu_frm_AlternativeItems.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AlternativeItem_ItemClick);
		this.menu_frm_BusinessPartnerCatalogNumbers.Caption = "Business Partner Catalog Numbers";
		this.menu_frm_BusinessPartnerCatalogNumbers.Id = 260;
		this.menu_frm_BusinessPartnerCatalogNumbers.Name = "menu_frm_BusinessPartnerCatalogNumbers";
		this.menu_frm_BusinessPartnerCatalogNumbers.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BusinessPartnerCatalogNumbers_ItemClick);
		this.barSub_PriceLists.Caption = "Price Lists";
		this.barSub_PriceLists.Id = 289;
		this.barSub_PriceLists.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_STDPriceLists),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PeriodAndVolumeDiscounts),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DiscountGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem13)
		});
		this.barSub_PriceLists.Name = "barSub_PriceLists";
		this.menu_frm_STDPriceLists.Caption = "Price List";
		this.menu_frm_STDPriceLists.Id = 290;
		this.menu_frm_STDPriceLists.Name = "menu_frm_STDPriceLists";
		this.menu_frm_PeriodAndVolumeDiscounts.Caption = "Period and Volume Discounts";
		this.menu_frm_PeriodAndVolumeDiscounts.Id = 291;
		this.menu_frm_PeriodAndVolumeDiscounts.Name = "menu_frm_PeriodAndVolumeDiscounts";
		this.menu_frm_DiscountGroup.Caption = "Discount Groups";
		this.menu_frm_DiscountGroup.Id = 294;
		this.menu_frm_DiscountGroup.Name = "menu_frm_DiscountGroup";
		this.barSubItem13.Caption = "Special Prices";
		this.barSubItem13.Id = 292;
		this.barSubItem13.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SpecialforBP)
		});
		this.barSubItem13.Name = "barSubItem13";
		this.menu_frm_SpecialforBP.Caption = "Special Prices for Business Partners";
		this.menu_frm_SpecialforBP.Id = 293;
		this.menu_frm_SpecialforBP.Name = "menu_frm_SpecialforBP";
		this.barSub_PickAndPack.Caption = "Pick anh Pack";
		this.barSub_PickAndPack.Id = 395;
		this.barSub_PickAndPack.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PickingRequest),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PickingDocument),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Packing),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ShipDoc),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PackingByBarcode)
		});
		this.barSub_PickAndPack.Name = "barSub_PickAndPack";
		this.menu_frm_PickingRequest.Caption = "Picking Request";
		this.menu_frm_PickingRequest.Description = "22021";
		this.menu_frm_PickingRequest.Id = 367;
		this.menu_frm_PickingRequest.Name = "menu_frm_PickingRequest";
		this.menu_frm_PickingRequest.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PickingRequest_ItemClick);
		this.menu_frm_PickingDocument.Caption = "Pick List";
		this.menu_frm_PickingDocument.Id = 397;
		this.menu_frm_PickingDocument.Name = "menu_frm_PickingDocument";
		this.menu_frm_PickingDocument.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Picking_ItemClick);
		this.menu_frm_Packing.Caption = "Packing";
		this.menu_frm_Packing.Id = 396;
		this.menu_frm_Packing.Name = "menu_frm_Packing";
		this.menu_frm_Packing.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Packing_ItemClick);
		this.menu_frm_ShipDoc.Caption = "Shipping";
		this.menu_frm_ShipDoc.Id = 398;
		this.menu_frm_ShipDoc.Name = "menu_frm_ShipDoc";
		this.menu_frm_ShipDoc.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ShipDoc_ItemClick);
		this.menu_frm_PackingByBarcode.Caption = "Packing by Barcode";
		this.menu_frm_PackingByBarcode.Id = 541;
		this.menu_frm_PackingByBarcode.Name = "menu_frm_PackingByBarcode";
		this.menu_frm_PackingByBarcode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSub_InventoryReport.Caption = "Inventory Reports";
		this.barSub_InventoryReport.Id = 258;
		this.barSub_InventoryReport.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryDraftsReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_InventoryPostingList, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_StockMovementSummary),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BatchTransaction),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SerialNumberTransaction)
		});
		this.barSub_InventoryReport.Name = "barSub_InventoryReport";
		this.menu_frm_InventoryDraftsReport.Caption = "Document Drafts Reports";
		this.menu_frm_InventoryDraftsReport.Id = 450;
		this.menu_frm_InventoryDraftsReport.Name = "menu_frm_InventoryDraftsReport";
		this.menu_frm_InventoryDraftsReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryDraftsReport_ItemClick);
		this.menu_frm_InventoryPostingList.Caption = "Inventory Posting List";
		this.menu_frm_InventoryPostingList.Id = 446;
		this.menu_frm_InventoryPostingList.Name = "menu_frm_InventoryPostingList";
		this.menu_frm_InventoryPostingList.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_InventoryPostingList_ItemClick);
		this.menu_frm_StockMovementSummary.Caption = "Stock Movement Summary";
		this.menu_frm_StockMovementSummary.Id = 447;
		this.menu_frm_StockMovementSummary.Name = "menu_frm_StockMovementSummary";
		this.menu_frm_StockMovementSummary.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_StockMovementSummary_ItemClick);
		this.menu_frm_BatchTransaction.Caption = "Batch Transaction Report";
		this.menu_frm_BatchTransaction.Id = 448;
		this.menu_frm_BatchTransaction.Name = "menu_frm_BatchTransaction";
		this.menu_frm_BatchTransaction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BatchTransaction_ItemClick);
		this.menu_frm_SerialNumberTransaction.Caption = "Serial Transaction Report";
		this.menu_frm_SerialNumberTransaction.Id = 448;
		this.menu_frm_SerialNumberTransaction.Name = "menu_frm_SerialNumberTransaction";
		this.menu_frm_SerialNumberTransaction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SerialNumberTransaction_ItemClick);
		this.barSub_BinLocation.Caption = "Bin Locations";
		this.barSub_BinLocation.Id = 548;
		this.barSub_BinLocation.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BinLocMasterData)
		});
		this.barSub_BinLocation.Name = "barSub_BinLocation";
		this.menu_frm_BinLocMasterData.Caption = "Bin Location Master Data";
		this.menu_frm_BinLocMasterData.Id = 549;
		this.menu_frm_BinLocMasterData.Name = "menu_frm_BinLocMasterData";
		this.menu_frm_BinLocMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BinLocMasterData_ItemClick);
		this.barSub_Production.Caption = "Production";
		this.barSub_Production.Id = 248;
		this.barSub_Production.ImageOptions.ImageIndex = 10;
		this.barSub_Production.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[12]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BillOfMaterials),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvanceBillOfMaterials),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProductionOrder, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProductionOrderImportTool),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ReceiptFromProduction, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ReceiptFromPOByBarcode),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_IssueForProduction),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MaterialShortageReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenPurchaseFromMaterialShortage),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_OutSourcing),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ProductionReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_IssueAndReceiptLackQtyForProduction)
		});
		this.barSub_Production.Name = "barSub_Production";
		this.menu_frm_BillOfMaterials.Caption = "Bill Of Materials";
		this.menu_frm_BillOfMaterials.Id = 249;
		this.menu_frm_BillOfMaterials.Name = "menu_frm_BillOfMaterials";
		this.menu_frm_BillOfMaterials.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BillOfMaterials_ItemClick_1);
		this.menu_frm_AdvanceBillOfMaterials.Caption = "Advance Bill Of Materials";
		this.menu_frm_AdvanceBillOfMaterials.Id = 271;
		this.menu_frm_AdvanceBillOfMaterials.Name = "menu_frm_AdvanceBillOfMaterials";
		this.menu_frm_AdvanceBillOfMaterials.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdvanceBillOfMaterials_ItemClick);
		this.menu_frm_ProductionOrder.Caption = "Production Order";
		this.menu_frm_ProductionOrder.Id = 250;
		this.menu_frm_ProductionOrder.Name = "menu_frm_ProductionOrder";
		this.menu_frm_ProductionOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionOrder_ItemClick_1);
		this.menu_frm_ProductionOrderImportTool.Caption = "Production Order Import Tool";
		this.menu_frm_ProductionOrderImportTool.Id = 385;
		this.menu_frm_ProductionOrderImportTool.Name = "menu_frm_ProductionOrderImportTool";
		this.menu_frm_ProductionOrderImportTool.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionOrderImportTool_ItemPress);
		this.menu_frm_ReceiptFromProduction.Caption = "Receipt From Production";
		this.menu_frm_ReceiptFromProduction.Id = 309;
		this.menu_frm_ReceiptFromProduction.Name = "menu_frm_ReceiptFromProduction";
		this.menu_frm_ReceiptFromPOByBarcode.Caption = "Receipt From Production by Barcode";
		this.menu_frm_ReceiptFromPOByBarcode.Id = 540;
		this.menu_frm_ReceiptFromPOByBarcode.Name = "menu_frm_ReceiptFromPOByBarcode";
		this.menu_frm_ReceiptFromPOByBarcode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_IssueForProduction.Caption = "Issue For Production";
		this.menu_frm_IssueForProduction.Id = 310;
		this.menu_frm_IssueForProduction.Name = "menu_frm_IssueForProduction";
		this.menu_frm_IssueForProduction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_IssueForProduction_ItemClick);
		this.menu_frm_MaterialShortageReport.Caption = "Material Shortage Report";
		this.menu_frm_MaterialShortageReport.Id = 363;
		this.menu_frm_MaterialShortageReport.Name = "menu_frm_MaterialShortageReport";
		this.menu_frm_MaterialShortageReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MaterialShortageReport_ItemClick);
		this.menu_frm_GenPurchaseFromMaterialShortage.Caption = "Generator Purchase From Material Shortage";
		this.menu_frm_GenPurchaseFromMaterialShortage.Id = 376;
		this.menu_frm_GenPurchaseFromMaterialShortage.Name = "menu_frm_GenPurchaseFromMaterialShortage";
		this.menu_frm_GenPurchaseFromMaterialShortage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenPurchaseFromMaterialShortage_ItemClick);
		this.menu_frm_OutSourcing.Caption = "Outsourcing";
		this.menu_frm_OutSourcing.Id = 408;
		this.menu_frm_OutSourcing.Name = "menu_frm_OutSourcing";
		this.menu_frm_OutSourcing.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_OutSourcing_ItemClick);
		this.barSub_ProductionReports.Caption = "Production Reports";
		this.barSub_ProductionReports.Id = 438;
		this.barSub_ProductionReports.Name = "barSub_ProductionReports";
		this.menu_frm_IssueAndReceiptLackQtyForProduction.Caption = "Issue And Receipt lack Quantity For Production";
		this.menu_frm_IssueAndReceiptLackQtyForProduction.Id = 475;
		this.menu_frm_IssueAndReceiptLackQtyForProduction.Name = "menu_frm_IssueAndReceiptLackQtyForProduction";
		this.menu_frm_IssueAndReceiptLackQtyForProduction.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_OutSourcing1_ItemClick);
		this.barSub_MTO.Caption = "MTO Manufacturing";
		this.barSub_MTO.Id = 251;
		this.barSub_MTO.ImageOptions.ImageIndex = 10;
		this.barSub_MTO.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProductionReqForBatch),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProductionForBatch),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProductionConfirm),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PackingBatch)
		});
		this.barSub_MTO.Name = "barSub_MTO";
		this.menu_frm_ProductionReqForBatch.Caption = "Production Control";
		this.menu_frm_ProductionReqForBatch.Id = 252;
		this.menu_frm_ProductionReqForBatch.Name = "menu_frm_ProductionReqForBatch";
		this.menu_frm_ProductionReqForBatch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionReqForBatch_ItemClick);
		this.menu_frm_ProductionForBatch.Caption = "Production For Batch";
		this.menu_frm_ProductionForBatch.Id = 253;
		this.menu_frm_ProductionForBatch.Name = "menu_frm_ProductionForBatch";
		this.menu_frm_ProductionForBatch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionForBatch_ItemClick);
		this.menu_frm_ProductionConfirm.Caption = "Production Confirm";
		this.menu_frm_ProductionConfirm.Id = 254;
		this.menu_frm_ProductionConfirm.Name = "menu_frm_ProductionConfirm";
		this.menu_frm_ProductionConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionConfirm_ItemClick);
		this.menu_frm_PackingBatch.Caption = "Packing Batch";
		this.menu_frm_PackingBatch.Id = 255;
		this.menu_frm_PackingBatch.Name = "menu_frm_PackingBatch";
		this.menu_frm_PackingBatch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PackingBatch_ItemClick);
		this.barSub_MRP.Caption = "Planning && MRP";
		this.barSub_MRP.Id = 216;
		this.barSub_MRP.ImageOptions.ImageIndex = 11;
		this.barSub_MRP.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[15]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_FrmDefinedShift),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_FrmWorkScheduling),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Lines),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ItemQuotaArrangement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvanceForecast),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MRP),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Schedule),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenPOFromSchedule),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PrintBarCode),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AutoReportComplete),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenPurchaseFromSchedule),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MrpPlan),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_GenTranferRequestFromMrp),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_MRPReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JetstarPreOrder)
		});
		this.barSub_MRP.Name = "barSub_MRP";
		this.menu_FrmDefinedShift.Caption = "Defined Shift";
		this.menu_FrmDefinedShift.Id = 265;
		this.menu_FrmDefinedShift.Name = "menu_FrmDefinedShift";
		this.menu_FrmDefinedShift.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_FrmDefinedShift_ItemPress);
		this.menu_FrmWorkScheduling.Caption = "Work Scheduling";
		this.menu_FrmWorkScheduling.Id = 267;
		this.menu_FrmWorkScheduling.Name = "menu_FrmWorkScheduling";
		this.menu_FrmWorkScheduling.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_FrmWorkScheduling_ItemPress);
		this.menu_frm_Lines.Caption = "Lines";
		this.menu_frm_Lines.Id = 268;
		this.menu_frm_Lines.Name = "menu_frm_Lines";
		this.menu_frm_Lines.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Lines_ItemClick);
		this.menu_frm_ItemQuotaArrangement.Caption = "Item Quota for Purchase";
		this.menu_frm_ItemQuotaArrangement.Id = 269;
		this.menu_frm_ItemQuotaArrangement.Name = "menu_frm_ItemQuotaArrangement";
		this.menu_frm_ItemQuotaArrangement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ItemQuotaArrangement_ItemClick);
		this.menu_frm_AdvanceForecast.Caption = "Forecast";
		this.menu_frm_AdvanceForecast.Id = 263;
		this.menu_frm_AdvanceForecast.Name = "menu_frm_AdvanceForecast";
		this.menu_frm_AdvanceForecast.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdvanceForecast_ItemClick);
		this.menu_frm_MRP.Caption = "MRP";
		this.menu_frm_MRP.Id = 218;
		this.menu_frm_MRP.Name = "menu_frm_MRP";
		this.menu_frm_MRP.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MRP_ItemClick);
		this.menu_frm_Schedule.Caption = "Schedule";
		this.menu_frm_Schedule.Id = 219;
		this.menu_frm_Schedule.Name = "menu_frm_Schedule";
		this.menu_frm_Schedule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProductionSchedule_ItemClick);
		this.menu_frm_GenPOFromSchedule.Caption = "Generate Production Order ";
		this.menu_frm_GenPOFromSchedule.Id = 220;
		this.menu_frm_GenPOFromSchedule.Name = "menu_frm_GenPOFromSchedule";
		this.menu_frm_GenPOFromSchedule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenPOFromSchedule_ItemClick);
		this.menu_frm_PrintBarCode.Caption = "Print BarCode";
		this.menu_frm_PrintBarCode.Id = 221;
		this.menu_frm_PrintBarCode.Name = "menu_frm_PrintBarCode";
		this.menu_frm_PrintBarCode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PrintBarCode_ItemClick);
		this.menu_frm_AutoReportComplete.Caption = "Report Complete by Barcode";
		this.menu_frm_AutoReportComplete.Id = 222;
		this.menu_frm_AutoReportComplete.Name = "menu_frm_AutoReportComplete";
		this.menu_frm_AutoReportComplete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AutoReportComplete_ItemClick);
		this.menu_frm_GenPurchaseFromSchedule.Caption = "Generate Purchase Order";
		this.menu_frm_GenPurchaseFromSchedule.Id = 270;
		this.menu_frm_GenPurchaseFromSchedule.Name = "menu_frm_GenPurchaseFromSchedule";
		this.menu_frm_GenPurchaseFromSchedule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenPurchaseFromSchedule_ItemClick);
		this.menu_frm_MrpPlan.Caption = "Mrp Planning";
		this.menu_frm_MrpPlan.Id = 318;
		this.menu_frm_MrpPlan.Name = "menu_frm_MrpPlan";
		this.menu_frm_MrpPlan.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MrpPlan_ItemClick);
		this.menu_frm_GenTranferRequestFromMrp.Caption = "Generate Inventory Transfer";
		this.menu_frm_GenTranferRequestFromMrp.Id = 322;
		this.menu_frm_GenTranferRequestFromMrp.Name = "menu_frm_GenTranferRequestFromMrp";
		this.menu_frm_GenTranferRequestFromMrp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_GenTranferRequestFromMrp_ItemClick);
		this.barSub_MRPReports.Caption = "Planning && MRP Reports";
		this.barSub_MRPReports.Id = 439;
		this.barSub_MRPReports.Name = "barSub_MRPReports";
		this.menu_frm_JetstarPreOrder.Caption = "DAILY PURCHASE PLAN";
		this.menu_frm_JetstarPreOrder.Id = 462;
		this.menu_frm_JetstarPreOrder.Name = "menu_frm_JetstarPreOrder";
		this.menu_frm_JetstarPreOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JetstarPreOrder_ItemClick);
		this.barSub_RoutingResources.Caption = "Routing && Resources";
		this.barSub_RoutingResources.Id = 230;
		this.barSub_RoutingResources.ImageOptions.ImageIndex = 21;
		this.barSub_RoutingResources.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Operation),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Resources),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ResourcesReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ResourceWorkCenter),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Operations),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvanceRouting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ResourceWorkScheduling)
		});
		this.barSub_RoutingResources.Name = "barSub_RoutingResources";
		this.menu_frm_Operation.Caption = "Operation";
		this.menu_frm_Operation.Id = 231;
		this.menu_frm_Operation.Name = "menu_frm_Operation";
		this.menu_frm_Operation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Operation_ItemClick);
		this.menu_frm_Resources.Caption = "Resources";
		this.menu_frm_Resources.Id = 233;
		this.menu_frm_Resources.Name = "menu_frm_Resources";
		this.menu_frm_Resources.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Resources_ItemClick);
		this.barSub_ResourcesReports.Caption = "Resources Reports";
		this.barSub_ResourcesReports.Id = 442;
		this.barSub_ResourcesReports.Name = "barSub_ResourcesReports";
		this.menu_frm_ResourceWorkCenter.Caption = "Resource Work Center";
		this.menu_frm_ResourceWorkCenter.Id = 478;
		this.menu_frm_ResourceWorkCenter.Name = "menu_frm_ResourceWorkCenter";
		this.menu_frm_ResourceWorkCenter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ResourceWorkCenter_ItemClick);
		this.menu_frm_Operations.Caption = "Operations";
		this.menu_frm_Operations.Id = 482;
		this.menu_frm_Operations.Name = "menu_frm_Operations";
		this.menu_frm_Operations.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Operations_ItemClick);
		this.menu_frm_AdvanceRouting.Caption = "Advance Routing";
		this.menu_frm_AdvanceRouting.Id = 484;
		this.menu_frm_AdvanceRouting.Name = "menu_frm_AdvanceRouting";
		this.menu_frm_AdvanceRouting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdvanceRoutings_ItemClick);
		this.menu_frm_ResourceWorkScheduling.Caption = "Resource Work Scheduling";
		this.menu_frm_ResourceWorkScheduling.Id = 498;
		this.menu_frm_ResourceWorkScheduling.Name = "menu_frm_ResourceWorkScheduling";
		this.menu_frm_ResourceWorkScheduling.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barItemResourceWorkScheduling_ItemClick);
		this.barSub_PackingManagement.Caption = "Packing Management";
		this.barSub_PackingManagement.Id = 525;
		this.barSub_PackingManagement.ImageOptions.ImageIndex = 27;
		this.barSub_PackingManagement.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CreateTypeCase),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PackingOrder),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PrintBillWrap),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Traceability)
		});
		this.barSub_PackingManagement.Name = "barSub_PackingManagement";
		this.menu_frm_CreateTypeCase.Caption = "Create Type Case";
		this.menu_frm_CreateTypeCase.Id = 531;
		this.menu_frm_CreateTypeCase.Name = "menu_frm_CreateTypeCase";
		this.menu_frm_CreateTypeCase.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CreateTypeCase_ItemClick);
		this.menu_frm_PackingOrder.Caption = "Packing Order";
		this.menu_frm_PackingOrder.Id = 532;
		this.menu_frm_PackingOrder.Name = "menu_frm_PackingOrder";
		this.menu_frm_PackingOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PackingOrder_ItemClick);
		this.menu_frm_PrintBillWrap.Caption = "Wrap Case";
		this.menu_frm_PrintBillWrap.Id = 533;
		this.menu_frm_PrintBillWrap.Name = "menu_frm_PrintBillWrap";
		this.menu_frm_PrintBillWrap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PrintBillWrap_ItemClick);
		this.menu_frm_Traceability.Caption = "Traceability";
		this.menu_frm_Traceability.Id = 534;
		this.menu_frm_Traceability.Name = "menu_frm_Traceability";
		this.menu_frm_Traceability.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Traceability_ItemClick);
		this.barSub_Service.Caption = "CRM";
		this.barSub_Service.Id = 223;
		this.barSub_Service.ImageOptions.ImageIndex = 12;
		this.barSub_Service.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[14]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesOpportunity),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Solution),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ServiceCalls),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Contract),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ServiceCallSetting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TaskManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_NoteManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_EventManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_OpportunitiesReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ServicesReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_WarrantyCard),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ServiceLevelAgreement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ServiceContractTemplate),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_MarketingCampaign)
		});
		this.barSub_Service.Name = "barSub_Service";
		this.menu_frm_SalesOpportunity.Caption = "Opportunity";
		this.menu_frm_SalesOpportunity.Description = "23001";
		this.menu_frm_SalesOpportunity.Id = 394;
		this.menu_frm_SalesOpportunity.Name = "menu_frm_SalesOpportunity";
		this.menu_frm_SalesOpportunity.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalesOpportunity_ItemClick);
		this.menu_frm_Solution.Caption = "Solutions";
		this.menu_frm_Solution.Id = 224;
		this.menu_frm_Solution.Name = "menu_frm_Solution";
		this.menu_frm_Solution.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Solution_ItemClick);
		this.menu_frm_ServiceCalls.Caption = "Service Call";
		this.menu_frm_ServiceCalls.Id = 225;
		this.menu_frm_ServiceCalls.Name = "menu_frm_ServiceCalls";
		this.menu_frm_ServiceCalls.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ServiceCall_ItemClick);
		this.menu_frm_Contract.Caption = "Service Contract";
		this.menu_frm_Contract.Id = 226;
		this.menu_frm_Contract.Name = "menu_frm_Contract";
		this.menu_frm_Contract.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Contract_ItemClick);
		this.menu_frm_ServiceCallSetting.Caption = "Service Call Setting";
		this.menu_frm_ServiceCallSetting.Id = 227;
		this.menu_frm_ServiceCallSetting.Name = "menu_frm_ServiceCallSetting";
		this.menu_frm_ServiceCallSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ServiceCallSetting_ItemClick);
		this.menu_frm_TaskManagement.Caption = "Task Management";
		this.menu_frm_TaskManagement.Id = 411;
		this.menu_frm_TaskManagement.Name = "menu_frm_TaskManagement";
		this.menu_frm_TaskManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Task_ItemClick);
		this.menu_frm_NoteManagement.Caption = "Note Management";
		this.menu_frm_NoteManagement.Id = 418;
		this.menu_frm_NoteManagement.Name = "menu_frm_NoteManagement";
		this.menu_frm_NoteManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_NoteManagement_ItemClick);
		this.menu_frm_EventManagement.Caption = "Event Management";
		this.menu_frm_EventManagement.Id = 421;
		this.menu_frm_EventManagement.Name = "menu_frm_EventManagement";
		this.menu_frm_EventManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_EventManagement_ItemClick);
		this.barSub_OpportunitiesReport.Caption = "CRM Reports";
		this.barSub_OpportunitiesReport.Id = 433;
		this.barSub_OpportunitiesReport.Name = "barSub_OpportunitiesReport";
		this.barSub_ServicesReports.Caption = "Services Reports";
		this.barSub_ServicesReports.Id = 440;
		this.barSub_ServicesReports.Name = "barSub_ServicesReports";
		this.menu_frm_WarrantyCard.Caption = "Warranty Card";
		this.menu_frm_WarrantyCard.Id = 477;
		this.menu_frm_WarrantyCard.Name = "menu_frm_WarrantyCard";
		this.menu_frm_WarrantyCard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_WarrantyCard_ItemClick);
		this.menu_frm_ServiceLevelAgreement.Caption = "Service Level Agreement";
		this.menu_frm_ServiceLevelAgreement.Id = 479;
		this.menu_frm_ServiceLevelAgreement.Name = "menu_frm_ServiceLevelAgreement";
		this.menu_frm_ServiceLevelAgreement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ServiceLevelAgreement_ItemClick);
		this.menu_frm_ServiceContractTemplate.Caption = "Service Contract Template";
		this.menu_frm_ServiceContractTemplate.Id = 480;
		this.menu_frm_ServiceContractTemplate.Name = "menu_frm_ServiceContractTemplate";
		this.menu_frm_ServiceContractTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ServiceContractTemplate_ItemClick);
		this.menu_frm_MarketingCampaign.Caption = "Marketing Campaign";
		this.menu_frm_MarketingCampaign.Id = 487;
		this.menu_frm_MarketingCampaign.Name = "menu_frm_MarketingCampaign";
		this.menu_frm_MarketingCampaign.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_MarketingCampaign_ItemClick);
		this.barSub_BankingPayment.Caption = "Banking";
		this.barSub_BankingPayment.Id = 244;
		this.barSub_BankingPayment.ImageOptions.ImageIndex = 15;
		this.barSub_BankingPayment.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PaymentForInvoice),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CashItemMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CouponMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_VoucherMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_BankingReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvanceInPayment),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvanceOutPayment),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_IncomingPaymentWizard),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdvancePaymentWizard)
		});
		this.barSub_BankingPayment.Name = "barSub_BankingPayment";
		this.menu_frm_PaymentForInvoice.Caption = "Payment for the Invoice";
		this.menu_frm_PaymentForInvoice.Id = 245;
		this.menu_frm_PaymentForInvoice.Name = "menu_frm_PaymentForInvoice";
		this.menu_frm_CashItemMasterData.Caption = "Cash Item Master Data";
		this.menu_frm_CashItemMasterData.Id = 414;
		this.menu_frm_CashItemMasterData.Name = "menu_frm_CashItemMasterData";
		this.menu_frm_CashItemMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CashItemMasterData_ItemClick);
		this.menu_frm_CouponMasterData.Caption = "Coupon Master Data";
		this.menu_frm_CouponMasterData.Id = 415;
		this.menu_frm_CouponMasterData.Name = "menu_frm_CouponMasterData";
		this.menu_frm_CouponMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CouponMasterData_ItemClick);
		this.menu_frm_VoucherMasterData.Caption = "Voucher Master Data";
		this.menu_frm_VoucherMasterData.Id = 416;
		this.menu_frm_VoucherMasterData.Name = "menu_frm_VoucherMasterData";
		this.menu_frm_VoucherMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_VoucherMasterData_ItemClick);
		this.barSub_BankingReports.Caption = "Banking Reports";
		this.barSub_BankingReports.Id = 441;
		this.barSub_BankingReports.Name = "barSub_BankingReports";
		this.menu_frm_AdvanceInPayment.Caption = "Advance Incoming Payment";
		this.menu_frm_AdvanceInPayment.Id = 463;
		this.menu_frm_AdvanceInPayment.Name = "menu_frm_AdvanceInPayment";
		this.menu_frm_AdvanceInPayment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdvanceInPayment_ItemClick);
		this.menu_frm_AdvanceOutPayment.Caption = "Advance Outgoing Payment";
		this.menu_frm_AdvanceOutPayment.Id = 464;
		this.menu_frm_AdvanceOutPayment.Name = "menu_frm_AdvanceOutPayment";
		this.menu_frm_AdvanceOutPayment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AdvanceOutPayment_ItemClick);
		this.menu_frm_IncomingPaymentWizard.Caption = "Incoming Payment Wizard";
		this.menu_frm_IncomingPaymentWizard.Id = 537;
		this.menu_frm_IncomingPaymentWizard.Name = "menu_frm_IncomingPaymentWizard";
		this.menu_frm_IncomingPaymentWizard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_AdvancePaymentWizard.Caption = "Advance Payment Wizard";
		this.menu_frm_AdvancePaymentWizard.Id = 563;
		this.menu_frm_AdvancePaymentWizard.Name = "menu_frm_AdvancePaymentWizard";
		this.menu_frm_AdvancePaymentWizard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSub_Retail.Caption = "Retail";
		this.barSub_Retail.Id = 470;
		this.barSub_Retail.ImageOptions.ImageIndex = 22;
		this.barSub_Retail.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PosProcessBar),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LayoutSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_CmdConfig),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_RetailStore),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PosMaster),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PosAssignment)
		});
		this.barSub_Retail.Name = "barSub_Retail";
		this.menu_frm_PosProcessBar.Caption = "Point Of Sales";
		this.menu_frm_PosProcessBar.Id = 471;
		this.menu_frm_PosProcessBar.Name = "menu_frm_PosProcessBar";
		this.menu_frm_PosProcessBar.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PosProcessBar_ItemClick);
		this.menu_frm_LayoutSetup.Caption = "POS Layout Setup";
		this.menu_frm_LayoutSetup.Id = 472;
		this.menu_frm_LayoutSetup.Name = "menu_frm_LayoutSetup";
		this.menu_frm_LayoutSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LayoutSetup_ItemClick);
		this.menu_frm_CmdConfig.Caption = "ButtonCommands Config";
		this.menu_frm_CmdConfig.Id = 473;
		this.menu_frm_CmdConfig.Name = "menu_frm_CmdConfig";
		this.menu_frm_CmdConfig.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_CmdConfig_ItemClick);
		this.menu_frm_RetailStore.Caption = "Retail Stores";
		this.menu_frm_RetailStore.Id = 474;
		this.menu_frm_RetailStore.Name = "menu_frm_RetailStore";
		this.menu_frm_RetailStore.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_RetailStore_ItemClick);
		this.menu_frm_PosMaster.Caption = "POS Master Data";
		this.menu_frm_PosMaster.Id = 485;
		this.menu_frm_PosMaster.Name = "menu_frm_PosMaster";
		this.menu_frm_PosMaster.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PosMaster_ItemClick);
		this.menu_frm_PosAssignment.Caption = "POS Assignment";
		this.menu_frm_PosAssignment.Id = 486;
		this.menu_frm_PosAssignment.Name = "menu_frm_PosAssignment";
		this.menu_frm_PosAssignment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PosAssignment_ItemClick);
		this.barSub_HRM.Caption = "Human Resources";
		this.barSub_HRM.Id = 228;
		this.barSub_HRM.ImageOptions.ImageIndex = 13;
		this.barSub_HRM.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[11]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_HumanResources),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_JobVacancy),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalaryInfo),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TimeSheeting),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TimeSheetingHour),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalaryCalculation),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_EmpPenalization),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LeaveInformation),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_HRMReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_LeaveCalculation),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem9)
		});
		this.barSub_HRM.Name = "barSub_HRM";
		this.menu_frm_HumanResources.Caption = "Employee Master Data";
		this.menu_frm_HumanResources.Id = 237;
		this.menu_frm_HumanResources.Name = "menu_frm_HumanResources";
		this.menu_frm_HumanResources.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_HumanResources_ItemClick);
		this.menu_frm_JobVacancy.Caption = "Job Vacancy";
		this.menu_frm_JobVacancy.Id = 333;
		this.menu_frm_JobVacancy.Name = "menu_frm_JobVacancy";
		this.menu_frm_JobVacancy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_JobVacancy_ItemClick);
		this.menu_frm_SalaryInfo.Caption = "Employee Salary Information";
		this.menu_frm_SalaryInfo.Id = 359;
		this.menu_frm_SalaryInfo.Name = "menu_frm_SalaryInfo";
		this.menu_frm_SalaryInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalaryInfo_ItemClick);
		this.menu_frm_TimeSheeting.Caption = "Time Sheeting";
		this.menu_frm_TimeSheeting.Id = 362;
		this.menu_frm_TimeSheeting.Name = "menu_frm_TimeSheeting";
		this.menu_frm_TimeSheeting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TimeSheeting_ItemClick);
		this.menu_frm_TimeSheetingHour.Caption = "Time Sheeting OT";
		this.menu_frm_TimeSheetingHour.Id = 368;
		this.menu_frm_TimeSheetingHour.Name = "menu_frm_TimeSheetingHour";
		this.menu_frm_TimeSheetingHour.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TimeSheetingHour_ItemClick);
		this.menu_frm_SalaryCalculation.Caption = "Salary Calculation";
		this.menu_frm_SalaryCalculation.Id = 377;
		this.menu_frm_SalaryCalculation.Name = "menu_frm_SalaryCalculation";
		this.menu_frm_SalaryCalculation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SalaryCalculation_ItemClick);
		this.menu_frm_EmpPenalization.Caption = "Employee Penalty/Bonus";
		this.menu_frm_EmpPenalization.Id = 407;
		this.menu_frm_EmpPenalization.Name = "menu_frm_EmpPenalization";
		this.menu_frm_EmpPenalization.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_EmpPenalization_ItemClick);
		this.menu_frm_LeaveInformation.Caption = "Employee Leave Information";
		this.menu_frm_LeaveInformation.Id = 452;
		this.menu_frm_LeaveInformation.Name = "menu_frm_LeaveInformation";
		this.menu_frm_LeaveInformation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LeaveInformation_ItemClick);
		this.barSub_HRMReports.Caption = "Human Resources Reports";
		this.barSub_HRMReports.Id = 443;
		this.barSub_HRMReports.Name = "barSub_HRMReports";
		this.menu_frm_LeaveCalculation.Caption = "Leave Calculation";
		this.menu_frm_LeaveCalculation.Id = 453;
		this.menu_frm_LeaveCalculation.Name = "menu_frm_LeaveCalculation";
		this.menu_frm_LeaveCalculation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LeaveCalculation_ItemClick);
		this.barSubItem9.Caption = "Employee Appearance Tracking";
		this.barSubItem9.Id = 500;
		this.barSubItem9.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BiometricData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BiometricDataProcess),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TimeSheetInOut),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_BiometricEmpMapping)
		});
		this.barSubItem9.Name = "barSubItem9";
		this.menu_frm_BiometricData.Caption = "BioMetric Data";
		this.menu_frm_BiometricData.Id = 501;
		this.menu_frm_BiometricData.Name = "menu_frm_BiometricData";
		this.menu_frm_BiometricData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BiometricData_ItemClick);
		this.menu_frm_BiometricDataProcess.Caption = "Time Attendance Data";
		this.menu_frm_BiometricDataProcess.Id = 606;
		this.menu_frm_BiometricDataProcess.Name = "menu_frm_BiometricDataProcess";
		this.menu_frm_BiometricDataProcess.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BiometricDataProcess_ItemClick);
		this.menu_frm_TimeSheetInOut.Caption = "TimeSheet In/Out";
		this.menu_frm_TimeSheetInOut.Id = 502;
		this.menu_frm_TimeSheetInOut.Name = "menu_frm_TimeSheetInOut";
		this.menu_frm_TimeSheetInOut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TimeSheetInOut_ItemClick);
		this.menu_frm_BiometricEmpMapping.Caption = "Biometric Mapping";
		this.menu_frm_BiometricEmpMapping.Id = 503;
		this.menu_frm_BiometricEmpMapping.Name = "menu_frm_BiometricEmpMapping";
		this.menu_frm_BiometricEmpMapping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_BiometricEmpMapping_ItemClick);
		this.barSub_PM.Caption = "Project Management";
		this.barSub_PM.Id = 422;
		this.barSub_PM.ImageOptions.ImageIndex = 14;
		this.barSub_PM.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProjectManager),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TrackingIssue),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ProjectReports)
		});
		this.barSub_PM.Name = "barSub_PM";
		this.menu_frm_ProjectManager.Caption = "Project Manager";
		this.menu_frm_ProjectManager.Id = 426;
		this.menu_frm_ProjectManager.Name = "menu_frm_ProjectManager";
		this.menu_frm_ProjectManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProjectManager_ItemClick);
		this.menu_frm_TrackingIssue.Caption = "Tracking Issue";
		this.menu_frm_TrackingIssue.Id = 423;
		this.menu_frm_TrackingIssue.Name = "menu_frm_TrackingIssue";
		this.menu_frm_TrackingIssue.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TrackingIssue_ItemClick);
		this.barSub_ProjectReports.Caption = "Project Management Reports";
		this.barSub_ProjectReports.Id = 444;
		this.barSub_ProjectReports.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProjectReportCompletion),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ProjectConfirm)
		});
		this.barSub_ProjectReports.Name = "barSub_ProjectReports";
		this.menu_frm_ProjectReportCompletion.Caption = "Project Report Completion";
		this.menu_frm_ProjectReportCompletion.Id = 460;
		this.menu_frm_ProjectReportCompletion.Name = "menu_frm_ProjectReportCompletion";
		this.menu_frm_ProjectReportCompletion.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProjectReportCompletion_ItemClick);
		this.menu_frm_ProjectConfirm.Caption = "Project Confirm";
		this.menu_frm_ProjectConfirm.Id = 461;
		this.menu_frm_ProjectConfirm.Name = "menu_frm_ProjectConfirm";
		this.menu_frm_ProjectConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ProjectConfirm_ItemClick);
		this.barSub_DMS.Caption = "DMS";
		this.barSub_DMS.Id = 521;
		this.barSub_DMS.ImageOptions.ImageIndex = 26;
		this.barSub_DMS.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem10, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Danhmuc),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesRoute),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DMSDelivery)
		});
		this.barSub_DMS.Name = "barSub_DMS";
		this.barSubItem10.Caption = "Cài đặt";
		this.barSubItem10.Id = 556;
		this.barSubItem10.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SettingReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SettingApprovalDetail),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DMSSettingUDF),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_DMSAuth),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ImageAlbum),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_QuestionMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Marker),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AlertTemplate),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_NotificationEventManager)
		});
		this.barSubItem10.Name = "barSubItem10";
		this.menu_frm_SettingReport.Caption = "Cài đặt báo cáo";
		this.menu_frm_SettingReport.Id = 553;
		this.menu_frm_SettingReport.Name = "menu_frm_SettingReport";
		this.menu_frm_SettingReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SettingReport_ItemClick);
		this.menu_frm_SettingApprovalDetail.Caption = "Cài đặt chi tiết phê duyệt";
		this.menu_frm_SettingApprovalDetail.Id = 594;
		this.menu_frm_SettingApprovalDetail.Name = "menu_frm_SettingApprovalDetail";
		this.menu_frm_SettingApprovalDetail.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SettingApprovalDetail_ItemClick);
		this.menu_frm_DMSSettingUDF.Caption = "Cài đặt UDF";
		this.menu_frm_DMSSettingUDF.Id = 599;
		this.menu_frm_DMSSettingUDF.Name = "menu_frm_DMSSettingUDF";
		this.menu_frm_DMSSettingUDF.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DMSSettingUDF_ItemClick);
		this.menu_frm_DMSAuth.Caption = "Phân quyền";
		this.menu_frm_DMSAuth.Id = 580;
		this.menu_frm_DMSAuth.Name = "menu_frm_DMSAuth";
		this.menu_frm_DMSAuth.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_DMSAuth_ItemClick);
		this.menu_frm_ImageAlbum.Caption = "Album ảnh";
		this.menu_frm_ImageAlbum.Id = 576;
		this.menu_frm_ImageAlbum.Name = "menu_frm_ImageAlbum";
		this.menu_frm_ImageAlbum.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Menu_frm_ImageAlbum_ItemClick);
		this.menu_frm_QuestionMasterData.Caption = "Bộ câu hỏi";
		this.menu_frm_QuestionMasterData.Id = 542;
		this.menu_frm_QuestionMasterData.Name = "menu_frm_QuestionMasterData";
		this.menu_frm_QuestionMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_QuestionMasterData_ItemClick);
		this.menu_frm_Marker.Caption = "Marker";
		this.menu_frm_Marker.Id = 579;
		this.menu_frm_Marker.Name = "menu_frm_Marker";
		this.menu_frm_Marker.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Menu_frm_Marker_ItemClick);
		this.menu_frm_AlertTemplate.Caption = "Mẫu thông báo";
		this.menu_frm_AlertTemplate.Id = 581;
		this.menu_frm_AlertTemplate.Name = "menu_frm_AlertTemplate";
		this.menu_frm_AlertTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_AlertTemplate_ItemClick);
		this.menu_frm_NotificationEventManager.Caption = "Sự kiện thông báo";
		this.menu_frm_NotificationEventManager.Id = 593;
		this.menu_frm_NotificationEventManager.Name = "menu_frm_NotificationEventManager";
		this.menu_frm_NotificationEventManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_NotificationEventManager_ItemClick);
		this.barSub_Danhmuc.Caption = "Danh mục";
		this.barSub_Danhmuc.Id = 558;
		this.barSub_Danhmuc.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Geography),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_Channel),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UploadItemImage),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ImageViewer),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UserOnlineDashboard),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ItemImageViewer),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_AdminUploadAttachment)
		});
		this.barSub_Danhmuc.Name = "barSub_Danhmuc";
		this.menu_frm_Geography.Caption = "Vị trí địa lý";
		this.menu_frm_Geography.Id = 522;
		this.menu_frm_Geography.Name = "menu_frm_Geography";
		this.menu_frm_Geography.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Geography_ItemClick);
		this.menu_frm_Channel.Caption = "Kênh bán hàng";
		this.menu_frm_Channel.Id = 554;
		this.menu_frm_Channel.Name = "menu_frm_Channel";
		this.menu_frm_Channel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_Channel_ItemClick);
		this.menu_frm_UploadItemImage.Caption = "Tải ảnh sản phẩm";
		this.menu_frm_UploadItemImage.Id = 575;
		this.menu_frm_UploadItemImage.Name = "menu_frm_UploadItemImage";
		this.menu_frm_UploadItemImage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UploadItemImage_ItemClick);
		this.menu_frm_ImageViewer.Caption = "Xem ảnh báo cáo";
		this.menu_frm_ImageViewer.Id = 578;
		this.menu_frm_ImageViewer.Name = "menu_frm_ImageViewer";
		this.menu_frm_ImageViewer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ImageViewer_ItemClick);
		this.menu_frm_UserOnlineDashboard.Caption = "Tra\u0323ng tha\u0301i trư\u0323c tuyê\u0301n";
		this.menu_frm_UserOnlineDashboard.Id = 584;
		this.menu_frm_UserOnlineDashboard.Name = "menu_frm_UserOnlineDashboard";
		this.menu_frm_UserOnlineDashboard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UserOnlineDashboard_ItemClick);
		this.menu_frm_ItemImageViewer.Caption = "Xem danh sa\u0301ch sa\u0309n phâ\u0309m";
		this.menu_frm_ItemImageViewer.Id = 592;
		this.menu_frm_ItemImageViewer.Name = "menu_frm_ItemImageViewer";
		this.menu_frm_ItemImageViewer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ItemImageViewer_ItemClick);
		this.menu_frm_AdminUploadAttachment.Caption = "Tải lên tệp đính kèm";
		this.menu_frm_AdminUploadAttachment.Id = 603;
		this.menu_frm_AdminUploadAttachment.Name = "menu_frm_AdminUploadAttachment";
		this.menu_frm_AdminUploadAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_SalesRoute.Caption = "Tuyến bán hàng";
		this.menu_frm_SalesRoute.Id = 524;
		this.menu_frm_SalesRoute.Name = "menu_frm_SalesRoute";
		this.menu_frm_SalesRoute.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_SaleTerritory_ItemClick);
		this.barSubItem15.Caption = "Promotions";
		this.barSubItem15.Id = 559;
		this.barSubItem15.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PromotionSetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PromotionCondition),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PromotionBenefit),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_PromotionProgram, true)
		});
		this.barSubItem15.Name = "barSubItem15";
		this.menu_frm_PromotionSetup.Caption = "Promotion Type";
		this.menu_frm_PromotionSetup.Id = 552;
		this.menu_frm_PromotionSetup.Name = "menu_frm_PromotionSetup";
		this.menu_frm_PromotionSetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_PromotionSetup_ItemClick);
		this.menu_frm_PromotionCondition.Caption = "Promotion Conditions";
		this.menu_frm_PromotionCondition.Id = 555;
		this.menu_frm_PromotionCondition.Name = "menu_frm_PromotionCondition";
		this.menu_frm_PromotionCondition.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_PromotionBenefit.Caption = "Promotion Benefits";
		this.menu_frm_PromotionBenefit.Id = 561;
		this.menu_frm_PromotionBenefit.Name = "menu_frm_PromotionBenefit";
		this.menu_frm_PromotionBenefit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_PromotionProgram.Caption = "Promotion Programs";
		this.menu_frm_PromotionProgram.Id = 562;
		this.menu_frm_PromotionProgram.Name = "menu_frm_PromotionProgram";
		this.menu_frm_PromotionProgram.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSubItem16.Caption = "Ghé thăm";
		this.barSubItem16.Id = 560;
		this.barSubItem16.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_OutletVisitingFrequency)
		});
		this.barSubItem16.Name = "barSubItem16";
		this.menu_frm_OutletVisitingFrequency.Caption = "Lịch ghé thăm cửa hàng";
		this.menu_frm_OutletVisitingFrequency.Id = 551;
		this.menu_frm_OutletVisitingFrequency.Name = "menu_frm_OutletVisitingFrequency";
		this.menu_frm_OutletVisitingFrequency.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSubItem17.Caption = "Báo cáo";
		this.barSubItem17.Id = 564;
		this.barSubItem17.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SalesCoverageGoogleMap)
		});
		this.barSubItem17.Name = "barSubItem17";
		this.menu_frm_SalesCoverageGoogleMap.Caption = "Bản đồ phủ thị trường";
		this.menu_frm_SalesCoverageGoogleMap.Id = 565;
		this.menu_frm_SalesCoverageGoogleMap.Name = "menu_frm_SalesCoverageGoogleMap";
		this.menu_frm_SalesCoverageGoogleMap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_DMSDelivery.Caption = "Quản lý giao hàng";
		this.menu_frm_DMSDelivery.Id = 580;
		this.menu_frm_DMSDelivery.Name = "menu_frm_DMSDelivery";
		this.menu_frm_DMSDelivery.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSub_KPI.Caption = "KPI";
		this.barSub_KPI.Id = 566;
		this.barSub_KPI.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barSub_KPI.ImageOptions.Image");
		this.barSub_KPI.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[10]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIsMasterData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_SetIndicatorKPIs),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIsSetting, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ListPointKPIs),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIPlan),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIScope),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIIndicator),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIGroup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPISetup),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_KPIScore)
		});
		this.barSub_KPI.Name = "barSub_KPI";
		this.menu_frm_KPIsMasterData.Caption = "Khai báo KPI";
		this.menu_frm_KPIsMasterData.Id = 569;
		this.menu_frm_KPIsMasterData.Name = "menu_frm_KPIsMasterData";
		this.menu_frm_KPIsMasterData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Menu_frm_KPIsMasterData_ItemClick);
		this.menu_frm_SetIndicatorKPIs.Caption = "Bộ chỉ tiêu KPIs";
		this.menu_frm_SetIndicatorKPIs.Id = 570;
		this.menu_frm_SetIndicatorKPIs.Name = "menu_frm_SetIndicatorKPIs";
		this.menu_frm_SetIndicatorKPIs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Menu_frm_SetIndicatorKPIs_ItemClick);
		this.menu_frm_KPIsSetting.Caption = "Cài đặt điểm chuẩn KPI";
		this.menu_frm_KPIsSetting.Id = 568;
		this.menu_frm_KPIsSetting.Name = "menu_frm_KPIsSetting";
		this.menu_frm_KPIsSetting.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_ListPointKPIs.Caption = "Chấm điểm KPIs";
		this.menu_frm_ListPointKPIs.Id = 574;
		this.menu_frm_ListPointKPIs.Name = "menu_frm_ListPointKPIs";
		this.menu_frm_ListPointKPIs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ListPointKPIs_ItemClick);
		this.menu_frm_KPIPlan.Caption = "KPI Plans";
		this.menu_frm_KPIPlan.Id = 585;
		this.menu_frm_KPIPlan.Name = "menu_frm_KPIPlan";
		this.menu_frm_KPIPlan.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_KPIScope.Caption = "KPI Scopes";
		this.menu_frm_KPIScope.Id = 586;
		this.menu_frm_KPIScope.Name = "menu_frm_KPIScope";
		this.menu_frm_KPIScope.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_KPIIndicator.Caption = "KPI Indicators";
		this.menu_frm_KPIIndicator.Id = 587;
		this.menu_frm_KPIIndicator.Name = "menu_frm_KPIIndicator";
		this.menu_frm_KPIIndicator.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_KPIGroup.Caption = "KPI Groups";
		this.menu_frm_KPIGroup.Id = 588;
		this.menu_frm_KPIGroup.Name = "menu_frm_KPIGroup";
		this.menu_frm_KPIGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_KPISetup.Caption = "KPI Setups";
		this.menu_frm_KPISetup.Id = 589;
		this.menu_frm_KPISetup.Name = "menu_frm_KPISetup";
		this.menu_frm_KPISetup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.menu_frm_KPIScore.Caption = "KPI Scores (Manual)";
		this.menu_frm_KPIScore.Id = 590;
		this.menu_frm_KPIScore.Name = "menu_frm_KPIScore";
		this.menu_frm_KPIScore.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_barButtonItem_ItemClick);
		this.barSub_UDO.Caption = "User Defined Object";
		this.barSub_UDO.Id = 425;
		this.barSub_UDO.ImageOptions.ImageIndex = 24;
		this.barSub_UDO.Name = "barSub_UDO";
		this.barSub_Reports.Caption = "Reports";
		this.barSub_Reports.Id = 51;
		this.barSub_Reports.ImageOptions.ImageIndex = 25;
		this.barSub_Reports.Name = "barSub_Reports";
		this.barSub_Integrated.Caption = "Integrated";
		this.barSub_Integrated.Id = 129;
		this.barSub_Integrated.ImageOptions.ImageIndex = 20;
		this.barSub_Integrated.Name = "barSub_Integrated";
		this.barSub_Configuration.Caption = "Configuration";
		this.barSub_Configuration.Id = 234;
		this.barSub_Configuration.ImageOptions.ImageIndex = 12;
		this.barSub_Configuration.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barToggle_Fontsize)
		});
		this.barSub_Configuration.Name = "barSub_Configuration";
		this.barToggle_Fontsize.Caption = "Font Size Larger";
		this.barToggle_Fontsize.Id = 604;
		this.barToggle_Fontsize.Name = "barToggle_Fontsize";
		this.barSub_Language.Caption = "Language";
		this.barSub_Language.Id = 63;
		this.barSub_Language.ImageOptions.ImageIndex = 28;
		this.barSub_Language.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_LanguageEn),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_LanguageVn)
		});
		this.barSub_Language.Name = "barSub_Language";
		this.barSub_LanguageEn.Caption = "English";
		this.barSub_LanguageEn.Id = 64;
		this.barSub_LanguageEn.Name = "barSub_LanguageEn";
		this.barSub_LanguageEn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_LanguageE_ItemClick);
		this.barSub_LanguageVn.Caption = "Vietnamese";
		this.barSub_LanguageVn.Id = 65;
		this.barSub_LanguageVn.Name = "barSub_LanguageVn";
		this.barSub_LanguageVn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_LanguageVN_ItemClick);
		this.barSub_Tool.Caption = "Tool";
		this.barSub_Tool.Description = "M0002";
		this.barSub_Tool.Id = 4;
		this.barSub_Tool.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_Queries),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_CustomizationTools),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_UserDefineWindow),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_ImportData),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ChangeLogList, true)
		});
		this.barSub_Tool.Name = "barSub_Tool";
		this.barSub_Queries.Caption = "Queries";
		this.barSub_Queries.Id = 76;
		this.barSub_Queries.ImageOptions.ImageIndex = 16;
		this.barSub_Queries.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ManagerQuery),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_QueryGenerator),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_QueryPreview)
		});
		this.barSub_Queries.Name = "barSub_Queries";
		this.menu_frm_ManagerQuery.Caption = "Query Manager";
		this.menu_frm_ManagerQuery.Id = 120;
		this.menu_frm_ManagerQuery.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("menu_frm_ManagerQuery.ImageOptions.Image");
		this.menu_frm_ManagerQuery.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("menu_frm_ManagerQuery.ImageOptions.LargeImage");
		this.menu_frm_ManagerQuery.Name = "menu_frm_ManagerQuery";
		this.menu_frm_ManagerQuery.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbi_QueryManager_ItemClick);
		this.menu_frm_QueryGenerator.Caption = "Query Generator";
		this.menu_frm_QueryGenerator.Id = 77;
		this.menu_frm_QueryGenerator.Name = "menu_frm_QueryGenerator";
		this.menu_frm_QueryGenerator.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_QueryGenerator_ItemClick);
		this.menu_frm_QueryPreview.Caption = "Query Preview";
		this.menu_frm_QueryPreview.Id = 379;
		this.menu_frm_QueryPreview.Name = "menu_frm_QueryPreview";
		this.menu_frm_QueryPreview.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_QueryPreview_ItemClick);
		this.barSub_CustomizationTools.Caption = "Customization Tools";
		this.barSub_CustomizationTools.Id = 187;
		this.barSub_CustomizationTools.ImageOptions.ImageIndex = 17;
		this.barSub_CustomizationTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UserDefineFieldsManagement),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UDTManager),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ORW),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UserDefinedProcedure, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_UDR, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_TemplateWizard, true)
		});
		this.barSub_CustomizationTools.Name = "barSub_CustomizationTools";
		this.menu_frm_UserDefineFieldsManagement.Caption = "User - Defined Fields Management";
		this.menu_frm_UserDefineFieldsManagement.Id = 188;
		this.menu_frm_UserDefineFieldsManagement.Name = "menu_frm_UserDefineFieldsManagement";
		this.menu_frm_UserDefineFieldsManagement.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_UserDefineFields_ItemClick);
		this.menu_frm_UDTManager.Caption = "User - Defined Tables Management";
		this.menu_frm_UDTManager.Id = 189;
		this.menu_frm_UDTManager.Name = "menu_frm_UDTManager";
		this.menu_frm_UDTManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_UDT_ItemClick);
		this.menu_frm_ORW.Caption = "Objects Registration Wizard";
		this.menu_frm_ORW.Id = 388;
		this.menu_frm_ORW.Name = "menu_frm_ORW";
		this.menu_frm_ORW.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ObjectsRegistrationWizard_ItemClick);
		this.menu_frm_UserDefinedProcedure.Caption = "DIY Script";
		this.menu_frm_UserDefinedProcedure.Enabled = false;
		this.menu_frm_UserDefinedProcedure.Id = 386;
		this.menu_frm_UserDefinedProcedure.Name = "menu_frm_UserDefinedProcedure";
		this.menu_frm_UserDefinedProcedure.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.frm_UserDefinedProcedure_ItemClick);
		this.menu_frm_UDR.Caption = "Reports Registration Wizard";
		this.menu_frm_UDR.Id = 430;
		this.menu_frm_UDR.Name = "menu_frm_UDR";
		this.menu_frm_UDR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_UDR_ItemClick);
		this.menu_frm_TemplateWizard.Caption = "Design Form Wizard";
		this.menu_frm_TemplateWizard.Id = 490;
		this.menu_frm_TemplateWizard.Name = "menu_frm_TemplateWizard";
		this.menu_frm_TemplateWizard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_TemplateWizard_ItemClick);
		this.barSub_UserDefineWindow.Caption = "User-Defined Windows";
		this.barSub_UserDefineWindow.Id = 190;
		this.barSub_UserDefineWindow.ImageOptions.ImageIndex = 18;
		this.barSub_UserDefineWindow.Name = "barSub_UserDefineWindow";
		this.barSub_ImportData.Caption = "Import Data";
		this.barSub_ImportData.Id = 192;
		this.barSub_ImportData.ImageOptions.ImageIndex = 19;
		this.barSub_ImportData.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ImportDataWizard),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ImportFiles)
		});
		this.barSub_ImportData.Name = "barSub_ImportData";
		this.menu_frm_ImportDataWizard.Caption = "Import Data Wizard";
		this.menu_frm_ImportDataWizard.Id = 351;
		this.menu_frm_ImportDataWizard.Name = "menu_frm_ImportDataWizard";
		this.menu_frm_ImportDataWizard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem21_ItemClick_1);
		this.menu_frm_ImportFiles.Caption = "Import Files";
		this.menu_frm_ImportFiles.Id = 518;
		this.menu_frm_ImportFiles.Name = "menu_frm_ImportFiles";
		this.menu_frm_ImportFiles.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ImportFiles_ItemClick);
		this.menu_frm_ChangeLogList.Caption = "Change Log";
		this.menu_frm_ChangeLogList.Enabled = false;
		this.menu_frm_ChangeLogList.Id = 494;
		this.menu_frm_ChangeLogList.Name = "menu_frm_ChangeLogList";
		this.menu_frm_ChangeLogList.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_ChangeLog_ItemClick);
		this.barSub_Window.Caption = "Window";
		this.barSub_Window.Description = "M0003";
		this.barSub_Window.Id = 238;
		this.barSub_Window.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_SystemMessageLog)
		});
		this.barSub_Window.Name = "barSub_Window";
		this.barSub_SystemMessageLog.Caption = "System Message Log";
		this.barSub_SystemMessageLog.Id = 296;
		this.barSub_SystemMessageLog.Name = "barSub_SystemMessageLog";
		this.barSub_SystemMessageLog.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_SystemMessageLog_ItemClick);
		this.barSub_Help.Caption = "Help";
		this.barSub_Help.Description = "M0004";
		this.barSub_Help.Id = 5;
		this.barSub_Help.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_CheckForUpdate),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_UpdateReport),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_RefreshUDF),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_ViewSystemInformation),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButton_ChangeServerAddress),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSub_PreviewExternalReport, true)
		});
		this.barSub_Help.Name = "barSub_Help";
		this.barButtonItem_CheckForUpdate.Caption = "Check for Updates";
		this.barButtonItem_CheckForUpdate.Id = 61;
		this.barButtonItem_CheckForUpdate.Name = "barButtonItem_CheckForUpdate";
		this.barButtonItem_CheckForUpdate.Tag = "CheckForUpdate";
		this.barButtonItem_CheckForUpdate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_CheckForUpdate_ItemClick);
		this.barButtonItem_UpdateReport.Caption = "Update Report";
		this.barButtonItem_UpdateReport.Id = 287;
		this.barButtonItem_UpdateReport.Name = "barButtonItem_UpdateReport";
		this.barButtonItem_UpdateReport.Tag = "UpdateReport";
		this.barButtonItem_UpdateReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_UpdateReport_ItemClick);
		this.barSub_RefreshUDF.Caption = "Refresh UDF";
		this.barSub_RefreshUDF.Id = 603;
		this.barSub_RefreshUDF.Name = "barSub_RefreshUDF";
		this.barSub_RefreshUDF.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.barSub_RefreshUDF.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSub_RefreshUDF_ItemClick);
		this.barButtonItem_ViewSystemInformation.Caption = "View System Information";
		this.barButtonItem_ViewSystemInformation.Id = 394;
		this.barButtonItem_ViewSystemInformation.Name = "barButtonItem_ViewSystemInformation";
		this.barButtonItem_ViewSystemInformation.Tag = "ViewSystemInformation";
		this.barButtonItem_ViewSystemInformation.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
		this.barButton_ChangeServerAddress.Caption = "Change Server Address";
		this.barButton_ChangeServerAddress.Id = 499;
		this.barButton_ChangeServerAddress.Name = "barButton_ChangeServerAddress";
		this.barButton_ChangeServerAddress.Tag = "ChangeServerAddress";
		this.barButton_ChangeServerAddress.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_ChangeServerAddress_ItemClick);
		this.barSub_PreviewExternalReport.Caption = "Preview External Crystal Reports File";
		this.barSub_PreviewExternalReport.Id = 550;
		this.barSub_PreviewExternalReport.Name = "barSub_PreviewExternalReport";
		this.barSub_PreviewExternalReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_PreviewExternalReport_ItemClick);
		this.bar3.BarName = "Status bar";
		this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
		this.bar3.DockCol = 0;
		this.bar3.DockRow = 0;
		this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
		this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.txt_MsgBar),
			new DevExpress.XtraBars.LinkPersistInfo(this.bar_User),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.bar_ProgessControl, "", false, true, true, 284),
			new DevExpress.XtraBars.LinkPersistInfo(this.barBtnNotification)
		});
		this.bar3.OptionsBar.AllowQuickCustomization = false;
		this.bar3.OptionsBar.DrawDragBorder = false;
		this.bar3.OptionsBar.UseWholeRow = true;
		this.bar3.Text = "Status bar";
		this.txt_MsgBar.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_MsgBar.Id = 25;
		this.txt_MsgBar.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Immediate;
		this.txt_MsgBar.Name = "txt_MsgBar";
		this.txt_MsgBar.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bar_SystemMessageLog_ItemClick);
		this.bar_User.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
		this.bar_User.Id = 75;
		this.bar_User.ItemAppearance.Normal.ForeColor = System.Drawing.Color.DarkOrange;
		this.bar_User.ItemAppearance.Normal.Options.UseForeColor = true;
		this.bar_User.Name = "bar_User";
		this.bar_ProgessControl.Edit = this.repositoryItemProgressBar1;
		this.bar_ProgessControl.Id = 431;
		this.bar_ProgessControl.Name = "bar_ProgessControl";
		this.bar_ProgessControl.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
		this.repositoryItemProgressBar1.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.repositoryItemProgressBar1.ShowTitle = true;
		this.barBtnNotification.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
		this.barBtnNotification.Id = 451;
		this.barBtnNotification.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barBtnNotification.ImageOptions.Image");
		this.barBtnNotification.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barBtnNotification.ImageOptions.LargeImage");
		this.barBtnNotification.Name = "barBtnNotification";
		this.barBtnNotification.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this.barBtnNotification.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnNotification_ItemClick);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.BarManager;
		this.barDockControlTop.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlTop.Size = new System.Drawing.Size(949, 50);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 555);
		this.barDockControlBottom.Manager = this.BarManager;
		this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlBottom.Size = new System.Drawing.Size(949, 27);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 50);
		this.barDockControlLeft.Manager = this.BarManager;
		this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 505);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(949, 50);
		this.barDockControlRight.Manager = this.BarManager;
		this.barDockControlRight.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 505);
		this.dockMain.Form = this;
		this.dockMain.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[1] { this.dockMessageLog });
		this.dockMain.MenuManager = this.BarManager;
		this.dockMain.TopZIndexControls.AddRange(new string[9] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane" });
		this.dockMain.ActiveChildChanged += new DevExpress.XtraBars.Docking.DockPanelEventHandler(this.dockMain_ActiveChildChanged);
		this.dockMessageLog.Controls.Add(this.dockPanel1_Container);
		this.dockMessageLog.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
		this.dockMessageLog.ID = new System.Guid("dd515e1a-a6a0-417a-adab-1bcf9e95eb7b");
		this.dockMessageLog.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("dockMessageLog.ImageOptions.Image");
		this.dockMessageLog.Location = new System.Drawing.Point(0, 489);
		this.dockMessageLog.Name = "dockMessageLog";
		this.dockMessageLog.OriginalSize = new System.Drawing.Size(200, 181);
		this.dockMessageLog.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
		this.dockMessageLog.SavedIndex = 0;
		this.dockMessageLog.Size = new System.Drawing.Size(1139, 181);
		this.dockMessageLog.TabsScroll = true;
		this.dockMessageLog.Text = "System Message Log";
		this.dockMessageLog.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
		this.dockMessageLog.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(this.dockMessageLog_ClosedPanel);
		this.dockPanel1_Container.Controls.Add(this.grd_SystemMessageLog);
		this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel1_Container.Name = "dockPanel1_Container";
		this.dockPanel1_Container.Size = new System.Drawing.Size(1131, 154);
		this.dockPanel1_Container.TabIndex = 0;
		this.grd_SystemMessageLog.AllowPasteExcel = false;
		this.grd_SystemMessageLog.ClipboardData = null;
		this.grd_SystemMessageLog.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SystemMessageLog.Location = new System.Drawing.Point(0, 0);
		this.grd_SystemMessageLog.MainView = this.gv_SystemMessageLog;
		this.grd_SystemMessageLog.MenuManager = this.BarManager;
		this.grd_SystemMessageLog.Name = "grd_SystemMessageLog";
		this.grd_SystemMessageLog.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_Type, this.repositoryItemMemoExEdit1, this.repositoryItemMemoEdit1 });
		this.grd_SystemMessageLog.Size = new System.Drawing.Size(1131, 154);
		this.grd_SystemMessageLog.TabIndex = 0;
		this.grd_SystemMessageLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SystemMessageLog });
		this.gv_SystemMessageLog.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SystemMessageLog.ColumnKey = null;
		this.gv_SystemMessageLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4 });
		this.gv_SystemMessageLog.DisableFormSetting = false;
		this.gv_SystemMessageLog.DisplayException = false;
		this.gv_SystemMessageLog.FieldCurrency = null;
		this.gv_SystemMessageLog.FieldRate = null;
		this.gv_SystemMessageLog.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_SystemMessageLog.FormId = null;
		this.gv_SystemMessageLog.GridControl = this.grd_SystemMessageLog;
		this.gv_SystemMessageLog.GroupFormat = "[#image]{1} {2}";
		this.gv_SystemMessageLog.Images = this.iml_Column;
		this.gv_SystemMessageLog.IndicatorWidth = 35;
		this.gv_SystemMessageLog.IsApzTable = false;
		this.gv_SystemMessageLog.IsInsertNewRow = false;
		this.gv_SystemMessageLog.IsLoadingData = false;
		this.gv_SystemMessageLog.ItemId = null;
		this.gv_SystemMessageLog.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_SystemMessageLog.LstColumnAllowBatch = null;
		this.gv_SystemMessageLog.LstColumnNotAllowBatch = null;
		this.gv_SystemMessageLog.LstColumnNotEditable = null;
		this.gv_SystemMessageLog.LstColumNotSetting = null;
		this.gv_SystemMessageLog.Name = "gv_SystemMessageLog";
		this.gv_SystemMessageLog.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_SystemMessageLog.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
		this.gv_SystemMessageLog.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_SystemMessageLog.OptionsDetail.EnableMasterViewMode = false;
		this.gv_SystemMessageLog.OptionsPrint.PrintHorzLines = false;
		this.gv_SystemMessageLog.OptionsPrint.PrintVertLines = false;
		this.gv_SystemMessageLog.OptionsSelection.MultiSelect = true;
		this.gv_SystemMessageLog.OptionsView.ColumnAutoWidth = false;
		this.gv_SystemMessageLog.OptionsView.ShowGroupedColumns = true;
		this.gv_SystemMessageLog.OptionsView.ShowGroupPanel = false;
		this.gv_SystemMessageLog.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_SystemMessageLog.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn4, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.gv_SystemMessageLog.UseBatchMod = false;
		this.gridColumn1.Caption = "Type";
		this.gridColumn1.ColumnEdit = this.rep_Type;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Type";
		this.gridColumn1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn1.ImageOptions.Alignment = System.Drawing.StringAlignment.Center;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.AllowMove = false;
		this.gridColumn1.OptionsColumn.AllowSize = false;
		this.gridColumn1.OptionsColumn.FixedWidth = true;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.OptionsColumn.ShowCaption = false;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 53;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rep_Type.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.rep_Type.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[3]
		{
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Error", "Error", 2),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Warning", "Warning", 1),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Info", "Info", 0)
		});
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.SmallImages = this.imc_Data;
		this.imc_Data.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imc_Data.ImageStream");
		this.imc_Data.Images.SetKeyName(0, "E-commerce_and_Shopping-81-16.png");
		this.imc_Data.Images.SetKeyName(1, "1481729503_danger.png");
		this.imc_Data.Images.SetKeyName(2, "High16x16.png");
		this.gridColumn2.Caption = "Message";
		this.gridColumn2.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Message";
		this.gridColumn2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 684;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.DoubleClick;
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumn3.Caption = "Help Code";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "HelpCode";
		this.gridColumn3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 135;
		this.gridColumn4.Caption = "Received";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Received";
		this.gridColumn4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 179;
		this.iml_Column.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("iml_Column.ImageStream");
		this.iml_Column.TransparentColor = System.Drawing.Color.Transparent;
		this.iml_Column.Images.SetKeyName(0, "E-commerce_and_Shopping-81-16.png");
		this.iml_Column.Images.SetKeyName(1, "1481727584_58.png");
		this.iml_Column.Images.SetKeyName(2, "High16x16.png");
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.imc_Menu.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imc_Menu.ImageStream");
		this.imc_Menu.Images.SetKeyName(0, "Img_FolderClosed");
		this.imc_Menu.Images.SetKeyName(1, "Img_FolderOpened");
		this.imc_Menu.InsertGalleryImage("Img_MenuItem", "office2013/content/textbox_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/content/textbox_16x16.png"), 2);
		this.imc_Menu.Images.SetKeyName(2, "Img_MenuItem");
		this.imc_Menu.Images.SetKeyName(3, "Img_Administration");
		this.imc_Menu.InsertGalleryImage("Img_Dashboard", "office2013/chart/piestylepie_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/chart/piestylepie_16x16.png"), 4);
		this.imc_Menu.Images.SetKeyName(4, "Img_Dashboard");
		this.imc_Menu.Images.SetKeyName(5, "Img_Opportunity");
		this.imc_Menu.Images.SetKeyName(6, "Img_SalesAR");
		this.imc_Menu.Images.SetKeyName(7, "Img_PurchaseAP");
		this.imc_Menu.Images.SetKeyName(8, "Img_BP");
		this.imc_Menu.Images.SetKeyName(9, "Img_Inventory");
		this.imc_Menu.Images.SetKeyName(10, "Img_Production");
		this.imc_Menu.Images.SetKeyName(11, "Img_MRP");
		this.imc_Menu.Images.SetKeyName(12, "Img_Service");
		this.imc_Menu.Images.SetKeyName(13, "Img_HRM");
		this.imc_Menu.Images.SetKeyName(14, "Img_PM");
		this.imc_Menu.Images.SetKeyName(15, "Img_BankingPayment");
		this.imc_Menu.InsertGalleryImage("Img_Queries", "images/alignment/unmergecells_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/alignment/unmergecells_16x16.png"), 16);
		this.imc_Menu.Images.SetKeyName(16, "Img_Queries");
		this.imc_Menu.Images.SetKeyName(17, "Img_CustomizationTools");
		this.imc_Menu.Images.SetKeyName(18, "Img_UserDefineWindow");
		this.imc_Menu.Images.SetKeyName(19, "Img_ImportData");
		this.imc_Menu.Images.SetKeyName(20, "Img_Integrated");
		this.imc_Menu.Images.SetKeyName(21, "Img_RoutingResources");
		this.imc_Menu.Images.SetKeyName(22, "Img_Retail");
		this.imc_Menu.Images.SetKeyName(23, "Img_MTO");
		this.imc_Menu.InsertGalleryImage("Img_UDO", "images/chart/chartsshowlegend_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/chart/chartsshowlegend_16x16.png"), 24);
		this.imc_Menu.Images.SetKeyName(24, "Img_UDO");
		this.imc_Menu.InsertGalleryImage("Img_UserReport", "office2013/chart/bar_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/chart/bar_16x16.png"), 25);
		this.imc_Menu.Images.SetKeyName(25, "Img_UserReport");
		this.imc_Menu.InsertGalleryImage("Img_DMS", "office2013/navigation/documentmap_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/navigation/documentmap_16x16.png"), 26);
		this.imc_Menu.Images.SetKeyName(26, "Img_DMS");
		this.imc_Menu.Images.SetKeyName(27, "Img_PackingManagement");
		this.imc_Menu.Images.SetKeyName(28, "Img_Language");
		this.imc_Menu.Images.SetKeyName(29, "Img_KPI");
		this.barSubItem3.Caption = "Menu";
		this.barSubItem3.Id = 2;
		this.barSubItem3.Name = "barSubItem3";
		this.barButtonItem1.Id = 6;
		this.barButtonItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
		this.barButtonItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.LargeImage");
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
		this.barButtonItem2.Id = 7;
		this.barButtonItem2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem2.ImageOptions.Image");
		this.barButtonItem2.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem2.ImageOptions.LargeImage");
		this.barButtonItem2.Name = "barButtonItem2";
		this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
		this.barButtonItem3.Id = 8;
		this.barButtonItem3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem3.ImageOptions.Image");
		this.barButtonItem3.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem3.ImageOptions.LargeImage");
		this.barButtonItem3.Name = "barButtonItem3";
		this.barStaticItem1.Id = 9;
		this.barStaticItem1.Name = "barStaticItem1";
		this.barStaticItem2.Id = 12;
		this.barStaticItem2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barStaticItem2.ImageOptions.Image");
		this.barStaticItem2.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barStaticItem2.ImageOptions.LargeImage");
		this.barStaticItem2.Name = "barStaticItem2";
		this.barStaticItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this.barButtonItem14.Id = 21;
		this.barButtonItem14.Name = "barButtonItem14";
		this.skinBarSubItem1.Caption = "skinBarSubItem1";
		this.skinBarSubItem1.Id = 24;
		this.skinBarSubItem1.Name = "skinBarSubItem1";
		this.barButtonItem15.Id = 26;
		this.barButtonItem15.Name = "barButtonItem15";
		this.barSubItem7.Caption = "View";
		this.barSubItem7.Id = 27;
		this.barSubItem7.Name = "barSubItem7";
		this.barSubItem11.Caption = "barSubItem11";
		this.barSubItem11.Id = 33;
		this.barSubItem11.Name = "barSubItem11";
		this.barButtonItem7.Id = 34;
		this.barButtonItem7.Name = "barButtonItem7";
		this.barButtonItem8.Caption = "Configuation";
		this.barButtonItem8.Id = 37;
		this.barButtonItem8.Name = "barButtonItem8";
		this.barButtonItem9.Caption = "barButtonItem9";
		this.barButtonItem9.Id = 38;
		this.barButtonItem9.Name = "barButtonItem9";
		this.barButtonItem10.Caption = "Use Defind Field Manager";
		this.barButtonItem10.Id = 40;
		this.barButtonItem10.Name = "barButtonItem10";
		this.barButtonItem10.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem10_ItemClick);
		this.barButtonItem11.Caption = "Database Connection";
		this.barButtonItem11.Id = 41;
		this.barButtonItem11.Name = "barButtonItem11";
		this.barButtonItem11.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem11_ItemClick);
		this.barButtonItem12.Caption = "Administartor";
		this.barButtonItem12.Id = 42;
		this.barButtonItem12.Name = "barButtonItem12";
		this.barButtonItem16.Id = 43;
		this.barButtonItem16.Name = "barButtonItem16";
		this.barSubItem21.Id = 52;
		this.barSubItem21.Name = "barSubItem21";
		this.barButtonItem17.Caption = "Setting Control";
		this.barButtonItem17.Id = 53;
		this.barButtonItem17.Name = "barButtonItem17";
		this.barButtonItem18.Caption = "Change Style";
		this.barButtonItem18.Id = 54;
		this.barButtonItem18.Name = "barButtonItem18";
		this.barButtonItem19.Caption = "BackUp - Restore";
		this.barButtonItem19.Id = 55;
		this.barButtonItem19.Name = "barButtonItem19";
		this.skinBarSubItem2.Caption = "skinBarSubItem2";
		this.skinBarSubItem2.Id = 57;
		this.skinBarSubItem2.Name = "skinBarSubItem2";
		this.barSubItem22.Caption = "Go To Support";
		this.barSubItem22.Id = 59;
		this.barSubItem22.Name = "barSubItem22";
		this.barToolbars_CheckForUpdate.Id = 62;
		this.barToolbars_CheckForUpdate.Name = "barToolbars_CheckForUpdate";
		this.barSubItem8.Caption = "barSubItem8";
		this.barSubItem8.Id = 68;
		this.barSubItem8.Name = "barSubItem8";
		this.menu_frm_SalesQuotation.Caption = "Sales Quotation";
		this.menu_frm_SalesQuotation.Id = 81;
		this.menu_frm_SalesQuotation.Name = "menu_frm_SalesQuotation";
		this.menu_frm_SalesQuotation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_BQ_ItemClick);
		this.menu_AR_ReserveInvoices.Caption = "A/R Reserve Invoice";
		this.menu_AR_ReserveInvoices.Id = 88;
		this.menu_AR_ReserveInvoices.Name = "menu_AR_ReserveInvoices";
		this.menu_AR_ReserveInvoices.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_ARRI_ItemClick);
		this.barStaticItem7.Id = 96;
		this.barStaticItem7.Name = "barStaticItem7";
		this.barButtonItem4.Caption = "barButtonItem4";
		this.barButtonItem4.Id = 97;
		this.barButtonItem4.Name = "barButtonItem4";
		this.barSubItem23.Caption = "Export";
		this.barSubItem23.Id = 98;
		this.barSubItem23.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barSubItem23.ImageOptions.Image");
		this.barSubItem23.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barSubItem23.ImageOptions.LargeImage");
		this.barSubItem23.Name = "barSubItem23";
		this.barEditItem1.Caption = "barEditItem1";
		this.barEditItem1.Edit = this.repositoryItemTextEdit1;
		this.barEditItem1.Id = 100;
		this.barEditItem1.Name = "barEditItem1";
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.barHeaderItem1.Id = 101;
		this.barHeaderItem1.Name = "barHeaderItem1";
		this.barEditItem2.Edit = this.repositoryItemTextEdit2;
		this.barEditItem2.Id = 102;
		this.barEditItem2.Name = "barEditItem2";
		this.repositoryItemTextEdit2.AutoHeight = false;
		this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
		this.barDockingMenuItem1.Caption = "barDockingMenuItem1";
		this.barDockingMenuItem1.Id = 103;
		this.barDockingMenuItem1.Name = "barDockingMenuItem1";
		this.barEditItem3.Caption = "barEditItem3";
		this.barEditItem3.Edit = this.repositoryItemTextEdit3;
		this.barEditItem3.Id = 104;
		this.barEditItem3.Name = "barEditItem3";
		this.repositoryItemTextEdit3.AutoHeight = false;
		this.repositoryItemTextEdit3.Name = "repositoryItemTextEdit3";
		this.barButtonItem_QueryTemplateRpt.Caption = "Query Template Report";
		this.barButtonItem_QueryTemplateRpt.Id = 113;
		this.barButtonItem_QueryTemplateRpt.Name = "barButtonItem_QueryTemplateRpt";
		this.barButtonItem_QueryManagerRpt.Caption = "Query Manager Report";
		this.barButtonItem_QueryManagerRpt.Id = 114;
		this.barButtonItem_QueryManagerRpt.Name = "barButtonItem_QueryManagerRpt";
		this.barSubItem5.Caption = "barSubItem5";
		this.barSubItem5.Id = 115;
		this.barSubItem5.Name = "barSubItem5";
		this.barSubItem_QueryTemplateReport.Caption = "Query Template Report";
		this.barSubItem_QueryTemplateReport.Id = 116;
		this.barSubItem_QueryTemplateReport.Name = "barSubItem_QueryTemplateReport";
		this.barSubItem_QueryManagerReport.Caption = "Query Manager Report";
		this.barSubItem_QueryManagerReport.Id = 117;
		this.barSubItem_QueryManagerReport.Name = "barSubItem_QueryManagerReport";
		this.barEditItem4.Edit = this.repositoryItemTextEdit4;
		this.barEditItem4.EditWidth = 20;
		this.barEditItem4.Id = 125;
		this.barEditItem4.Name = "barEditItem4";
		this.repositoryItemTextEdit4.AutoHeight = false;
		this.repositoryItemTextEdit4.Name = "repositoryItemTextEdit4";
		this.barStaticItem12.Id = 127;
		this.barStaticItem12.Name = "barStaticItem12";
		this.barStaticItem12.TextAlignment = System.Drawing.StringAlignment.Far;
		this.barSubItem24.Caption = "A/R Invoice";
		this.barSubItem24.Id = 130;
		this.barSubItem24.Name = "barSubItem24";
		this.barEditItem5.Edit = this.repositoryItemTextEdit5;
		this.barEditItem5.Id = 135;
		this.barEditItem5.Name = "barEditItem5";
		this.repositoryItemTextEdit5.AutoHeight = false;
		this.repositoryItemTextEdit5.Name = "repositoryItemTextEdit5";
		this.barButtonItem6.Caption = "Setup";
		this.barButtonItem6.Id = 137;
		this.barButtonItem6.Name = "barButtonItem6";
		this.barButtonItem28.Caption = "Authorization";
		this.barButtonItem28.Id = 147;
		this.barButtonItem28.Name = "barButtonItem28";
		this.barButtonItem34.Caption = "Working Table";
		this.barButtonItem34.Id = 154;
		this.barButtonItem34.Name = "barButtonItem34";
		this.barButtonItem38.Caption = "Report";
		this.barButtonItem38.Id = 160;
		this.barButtonItem38.Name = "barButtonItem38";
		this.barButtonItem39.Caption = "Report";
		this.barButtonItem39.Id = 161;
		this.barButtonItem39.Name = "barButtonItem39";
		this.barButtonItem40.Caption = "barButtonItem40";
		this.barButtonItem40.Id = 162;
		this.barButtonItem40.Name = "barButtonItem40";
		this.barI_frm_Shipping.Caption = "Shipping Types";
		this.barI_frm_Shipping.Id = 176;
		this.barI_frm_Shipping.Name = "barI_frm_Shipping";
		this.barButtonItem56.Caption = "Finalcials";
		this.barButtonItem56.Id = 179;
		this.barButtonItem56.Name = "barButtonItem56";
		this.barSubItem29.Caption = "barSubItem29";
		this.barSubItem29.Id = 180;
		this.barSubItem29.Name = "barSubItem29";
		this.bsi_Cus.Caption = "Customization Tools";
		this.bsi_Cus.Id = 186;
		this.bsi_Cus.Name = "bsi_Cus";
		this.barButtonItem61.Caption = "barButtonItem61";
		this.barButtonItem61.Id = 191;
		this.barButtonItem61.Name = "barButtonItem61";
		this.barButtonItem62.Id = 457;
		this.barButtonItem62.Name = "barButtonItem62";
		this.bar_ItemMasterData.Id = 458;
		this.bar_ItemMasterData.Name = "bar_ItemMasterData";
		this.barButtonItem63.Id = 459;
		this.barButtonItem63.Name = "barButtonItem63";
		this.barSub_Banking.Caption = "Banking";
		this.barSub_Banking.Id = 242;
		this.barSub_Banking.Name = "barSub_Banking";
		this.barButtonItem22.Caption = "barButtonItem22";
		this.barButtonItem22.Id = 243;
		this.barButtonItem22.Name = "barButtonItem22";
		this.barButtonItem23.Caption = "Incomming Payment";
		this.barButtonItem23.Id = 246;
		this.barButtonItem23.Name = "barButtonItem23";
		this.barButtonItem24.Caption = "Outgoing Payment";
		this.barButtonItem24.Id = 247;
		this.barButtonItem24.Name = "barButtonItem24";
		this.barSub_Formula.Caption = "Formula";
		this.barSub_Formula.Id = 261;
		this.barSub_Formula.Name = "barSub_Formula";
		this.barButtonItem25.Caption = "Customer Groups";
		this.barButtonItem25.Id = 299;
		this.barButtonItem25.Name = "barButtonItem25";
		this.barButtonItem26.Caption = "Vendor Groups";
		this.barButtonItem26.Id = 300;
		this.barButtonItem26.Name = "barButtonItem26";
		this.barButtonItem27.Caption = "Business Partner Properties";
		this.barButtonItem27.Id = 301;
		this.barButtonItem27.Name = "barButtonItem27";
		this.barButtonItem29.Caption = "Business Partner Priorities";
		this.barButtonItem29.Id = 302;
		this.barButtonItem29.Name = "barButtonItem29";
		this.barButtonItem30.Caption = "POS-LAYOUT";
		this.barButtonItem30.Id = 313;
		this.barButtonItem30.Name = "barButtonItem30";
		this.barSub_Financials.Caption = "Financials";
		this.barSub_Financials.Id = 315;
		this.barSub_Financials.Name = "barSub_Financials";
		this.barButtonItem31.Caption = "Purchasing";
		this.barButtonItem31.Id = 369;
		this.barButtonItem31.Name = "barButtonItem31";
		this.barSubItem18.Caption = "Shipping Document";
		this.barSubItem18.Description = "22020";
		this.barSubItem18.Id = 372;
		this.barSubItem18.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ShippingDocumentPicked),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ShippingDocumentLoaded),
			new DevExpress.XtraBars.LinkPersistInfo(this.menu_frm_ShippingDocumentOutboundRoute)
		});
		this.barSubItem18.Name = "barSubItem18";
		this.menu_frm_ShippingDocumentPicked.Caption = "Picked";
		this.menu_frm_ShippingDocumentPicked.Description = "22022";
		this.menu_frm_ShippingDocumentPicked.Id = 373;
		this.menu_frm_ShippingDocumentPicked.Name = "menu_frm_ShippingDocumentPicked";
		this.menu_frm_ShippingDocumentPicked.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ShippingDocumentPicked_ItemClick);
		this.menu_frm_ShippingDocumentLoaded.Caption = "Loaded";
		this.menu_frm_ShippingDocumentLoaded.Description = "22023";
		this.menu_frm_ShippingDocumentLoaded.Id = 374;
		this.menu_frm_ShippingDocumentLoaded.Name = "menu_frm_ShippingDocumentLoaded";
		this.menu_frm_ShippingDocumentLoaded.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ShippingDocumentLoaded_ItemClick);
		this.menu_frm_ShippingDocumentOutboundRoute.Caption = "Outbound Route";
		this.menu_frm_ShippingDocumentOutboundRoute.Description = "22024";
		this.menu_frm_ShippingDocumentOutboundRoute.Id = 375;
		this.menu_frm_ShippingDocumentOutboundRoute.Name = "menu_frm_ShippingDocumentOutboundRoute";
		this.menu_frm_ShippingDocumentOutboundRoute.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ShippingDocumentOutboundRoute_ItemClick);
		this.bar_LogViewer.Id = 382;
		this.bar_LogViewer.Name = "bar_LogViewer";
		this.menu_frm_LogViewer.Caption = "LogViewer";
		this.menu_frm_LogViewer.Id = 381;
		this.menu_frm_LogViewer.Name = "menu_frm_LogViewer";
		this.menu_frm_LogViewer.Tag = "LogViewer";
		this.menu_frm_LogViewer.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_LogViewer_ItemClick);
		this.barButtonItem32.Id = 389;
		this.barButtonItem32.Name = "barButtonItem32";
		this.barButtonItem21.Caption = "barButtonItem21";
		this.barButtonItem21.Id = 393;
		this.barButtonItem21.Name = "barButtonItem21";
		this.barSub_Opportunity.Caption = "CRM";
		this.barSub_Opportunity.Description = "23000";
		this.barSub_Opportunity.Id = 393;
		this.barSub_Opportunity.ImageOptions.ImageIndex = 5;
		this.barSub_Opportunity.Name = "barSub_Opportunity";
		this.barButtonItem33.Id = 399;
		this.barButtonItem33.Name = "barButtonItem33";
		this.barButtonItem20.Id = 476;
		this.barButtonItem20.Name = "barButtonItem20";
		this.menu.Caption = "barButtonItem35";
		this.menu.Id = 526;
		this.menu.Name = "menu";
		this.menu_frm_PackingManagement.Caption = "Packing Management";
		this.menu_frm_PackingManagement.Id = 527;
		this.menu_frm_PackingManagement.Name = "menu_frm_PackingManagement";
		this.barButtonItem35.Caption = "barButtonItem35";
		this.barButtonItem35.Id = 528;
		this.barButtonItem35.Name = "barButtonItem35";
		this.barButtonItem36.Caption = "barButtonItem36";
		this.barButtonItem36.Id = 529;
		this.barButtonItem36.Name = "barButtonItem36";
		this.barButtonItem37.Caption = "barButtonItem37";
		this.barButtonItem37.Id = 530;
		this.barButtonItem37.Name = "barButtonItem37";
		this.barButtonItem42.Caption = "KPIs Seting";
		this.barButtonItem42.Id = 567;
		this.barButtonItem42.Name = "barButtonItem42";
		this.barButtonItem41.Caption = "Xem anh bao cao";
		this.barButtonItem41.Id = 577;
		this.barButtonItem41.Name = "barButtonItem41";
		this.barButtonItem50.Caption = "barButtonItem50";
		this.barButtonItem50.Id = 591;
		this.barButtonItem50.Name = "barButtonItem50";
		this.repositoryItemSearchControl1.AutoHeight = false;
		this.repositoryItemSearchControl1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.repositoryItemSearchControl1.Items.AddRange(new object[7] { "Sales Order", "Sales Quotation", "Delivery", "Return", "A/R Invoice", "A/R Reserve Invoice", "A/R Credit Memo" });
		this.repositoryItemSearchControl1.Name = "repositoryItemSearchControl1";
		this.repositoryItemSearchControl1.NullValuePrompt = "Enter text to search Menu...";
		this.repositoryItemTextEdit6.AutoHeight = false;
		this.repositoryItemTextEdit6.Name = "repositoryItemTextEdit6";
		this.nav_frm_DocumentDraftsRP1.Caption = "Document Drafts Report";
		this.nav_frm_DocumentDraftsRP1.Hint = "navItemGroup_SalesReport";
		this.nav_frm_DocumentDraftsRP1.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_DocumentDraftsRP1.ImageOptions.SmallImage");
		this.nav_frm_DocumentDraftsRP1.Name = "nav_frm_DocumentDraftsRP1";
		this.nav_frm_DocumentDraftsRP1.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarItem_DocumentDraftsReport_LinkClicked);
		this.nav_frm_DocumentDraftsRP2.Caption = "Document Drafts Report";
		this.nav_frm_DocumentDraftsRP2.Hint = "navItemGroup_SalesReport";
		this.nav_frm_DocumentDraftsRP2.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_DocumentDraftsRP2.ImageOptions.SmallImage");
		this.nav_frm_DocumentDraftsRP2.Name = "nav_frm_DocumentDraftsRP2";
		this.nav_frm_DocumentDraftsRP2.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarItem_DocumentDraftsReport_LinkClicked);
		this.nav_frm_DocumentDraftsRP3.Caption = "Document Drafts Report";
		this.nav_frm_DocumentDraftsRP3.Hint = "navItemGroup_SalesReport";
		this.nav_frm_DocumentDraftsRP3.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_DocumentDraftsRP3.ImageOptions.SmallImage");
		this.nav_frm_DocumentDraftsRP3.Name = "nav_frm_DocumentDraftsRP3";
		this.nav_frm_DocumentDraftsRP3.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarItem_DocumentDraftsReport_LinkClicked);
		this.barButtonItem5.Id = 10;
		this.barButtonItem5.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem5.ImageOptions.Image");
		this.barButtonItem5.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem5.ImageOptions.LargeImage");
		this.barButtonItem5.Name = "barButtonItem5";
		this.TimerStaticStatusBar.Tick += new System.EventHandler(this.TimerStaticStatusBar_Tick_1);
		this.bar4.BarName = "Custom 4";
		this.bar4.DockCol = 0;
		this.bar4.DockRow = 2;
		this.bar4.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.bar4.OptionsBar.AllowRename = true;
		this.bar4.Text = "Custom 4";
		this.barSubItem14.Caption = "barSubItem13";
		this.barSubItem14.Id = 45;
		this.barSubItem14.Name = "barSubItem14";
		this.nav_frm_ListScheduling.Name = "nav_frm_ListScheduling";
		this.nav_frm_POProcess.Name = "nav_frm_POProcess";
		this.nav_frm_ReportBarCode.Name = "nav_frm_ReportBarCode";
		this.nav_frm_BarCode.Name = "nav_frm_BarCode";
		this.nav_frm_BarCodeStandard.Name = "nav_frm_BarCodeStandard";
		this.timerLogin.Enabled = true;
		this.timerLogin.Interval = 1000;
		this.timerLogin.Tick += new System.EventHandler(this.timerLogin_Tick);
		this.navBarGroup13.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarGroup13.Appearance.Options.UseFont = true;
		this.navBarGroup13.AppearanceBackground.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarGroup13.AppearanceBackground.Options.UseFont = true;
		this.navBarGroup13.AppearanceHotTracked.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarGroup13.AppearanceHotTracked.Options.UseFont = true;
		this.navBarGroup13.Caption = "Administration";
		this.navBarGroup13.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText;
		this.navBarGroup13.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("navBarGroup13.ImageOptions.SmallImage");
		this.navBarGroup13.Name = "navBarGroup13";
		this.navBarGroup13.SelectedLinkIndex = 0;
		this.navBarGroup13.TopVisibleLinkIndex = 4;
		this.alertMain.AutoFormDelay = 1000;
		this.alertMain.AlertClick += new DevExpress.XtraBars.Alerter.AlertClickEventHandler(this.alertMain_AlertClick);
		this.timer_Alert.Interval = 60000;
		this.timer_Alert.Tick += new System.EventHandler(this.timer_Alert_Tick);
		this.barStaticItem8.Id = 35;
		this.barStaticItem8.Name = "barStaticItem8";
		this.barStaticItem9.Id = 35;
		this.barStaticItem9.Name = "barStaticItem9";
		this.barStaticItem10.Id = 35;
		this.barStaticItem10.Name = "barStaticItem10";
		this.txt_MenuSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_MenuSearch.EditValue = "";
		this.txt_MenuSearch.IsSearchControl = false;
		this.txt_MenuSearch.Location = new System.Drawing.Point(1281, 26);
		this.txt_MenuSearch.MenuManager = this.BarManager;
		this.txt_MenuSearch.Name = "txt_MenuSearch";
		this.txt_MenuSearch.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_MenuSearch.Properties.Appearance.Options.UseBackColor = true;
		this.txt_MenuSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_MenuSearch.Properties.NullText = "Search Menu";
		this.txt_MenuSearch.Size = new System.Drawing.Size(282, 20);
		this.txt_MenuSearch.TabIndex = 13;
		this.txt_MenuSearch.Tag = "txtSearch";
		this.txt_MenuSearch.ToolTip = "Search Menu";
		this.txt_MenuSearch.Values = null;
		this.txt_MenuSearch.Visible = false;
		this.txt_MenuSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_MenuSearch_KeyDown);
		this.npane_Main.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.npane_Main.Appearance.Options.UseBackColor = true;
		this.npane_Main.Controls.Add(this.npage_Operations);
		this.npane_Main.Dock = System.Windows.Forms.DockStyle.Left;
		this.npane_Main.ItemOrientation = System.Windows.Forms.Orientation.Vertical;
		this.npane_Main.Location = new System.Drawing.Point(0, 50);
		this.npane_Main.Name = "npane_Main";
		this.npane_Main.PageProperties.ShowExpandButton = false;
		this.npane_Main.PageProperties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
		this.npane_Main.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[1] { this.npage_Operations });
		this.npane_Main.RegularSize = new System.Drawing.Size(300, 505);
		this.npane_Main.SelectedPage = this.npage_Operations;
		this.npane_Main.Size = new System.Drawing.Size(300, 505);
		this.npane_Main.TabIndex = 19;
		this.npane_Main.Text = "navigationPane1";
		this.npane_Main.Visible = false;
		this.npane_Main.Click += new System.EventHandler(this.npane_Main_Click);
		this.npane_Main.MouseMove += new System.Windows.Forms.MouseEventHandler(this.npane_Main_MouseMove);
		this.npage_Operations.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.npage_Operations.Appearance.Options.UseBackColor = true;
		this.npage_Operations.Caption = "Main Menu";
		this.npage_Operations.Controls.Add(this.treeMenu);
		this.npage_Operations.Name = "npage_Operations";
		this.npage_Operations.Properties.AppearanceCaption.BackColor = System.Drawing.SystemColors.Control;
		this.npage_Operations.Properties.AppearanceCaption.Options.UseBackColor = true;
		this.npage_Operations.Properties.AppearanceCaption.Options.UseTextOptions = true;
		this.npage_Operations.Properties.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.npage_Operations.Size = new System.Drawing.Size(240, 432);
		this.npage_Operations.Tag = "MainMenu";
		this.treeMenu.Appearance.Empty.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.Empty.Options.UseBackColor = true;
		this.treeMenu.Appearance.EvenRow.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.EvenRow.Options.UseBackColor = true;
		this.treeMenu.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.treeMenu.Appearance.FocusedCell.Options.UseBackColor = true;
		this.treeMenu.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.treeMenu.Appearance.FocusedRow.Options.UseBackColor = true;
		this.treeMenu.Appearance.HeaderPanel.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.HeaderPanel.Options.UseBackColor = true;
		this.treeMenu.Appearance.OddRow.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.OddRow.Options.UseBackColor = true;
		this.treeMenu.Appearance.Preview.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.Preview.Options.UseBackColor = true;
		this.treeMenu.Appearance.Row.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.Row.Options.UseBackColor = true;
		this.treeMenu.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.treeMenu.Appearance.SelectedRow.Options.UseBackColor = true;
		this.treeMenu.Appearance.TreeLine.BackColor = System.Drawing.SystemColors.Control;
		this.treeMenu.Appearance.TreeLine.Options.UseBackColor = true;
		this.treeMenu.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.treeMenu.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.treeListColumn1 });
		this.treeMenu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.treeMenu.KeyFieldName = "Id";
		this.treeMenu.Location = new System.Drawing.Point(0, 0);
		this.treeMenu.Name = "treeMenu";
		this.treeMenu.OptionsBehavior.Editable = false;
		this.treeMenu.OptionsBehavior.ReadOnly = true;
		this.treeMenu.OptionsCustomization.AllowSort = false;
		this.treeMenu.OptionsFind.AllowFindPanel = false;
		this.treeMenu.OptionsFind.FindMode = DevExpress.XtraTreeList.FindMode.Always;
		this.treeMenu.OptionsFind.ShowClearButton = false;
		this.treeMenu.OptionsFind.ShowFindButton = false;
		this.treeMenu.OptionsLayout.AddNewColumns = false;
		this.treeMenu.OptionsMenu.EnableColumnMenu = false;
		this.treeMenu.OptionsMenu.EnableFooterMenu = false;
		this.treeMenu.OptionsMenu.ShowExpandCollapseItems = false;
		this.treeMenu.OptionsSelection.SelectNodesOnRightClick = true;
		this.treeMenu.OptionsSelection.UseIndicatorForSelection = true;
		this.treeMenu.OptionsView.EnableAppearanceEvenRow = true;
		this.treeMenu.OptionsView.EnableAppearanceOddRow = true;
		this.treeMenu.OptionsView.ExpandButtonCentered = false;
		this.treeMenu.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
		this.treeMenu.OptionsView.ShowButtons = false;
		this.treeMenu.OptionsView.ShowColumns = false;
		this.treeMenu.OptionsView.ShowIndicator = false;
		this.treeMenu.OptionsView.ShowVertLines = false;
		this.treeMenu.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Wide;
		this.treeMenu.ParentFieldName = "FatherId";
		this.treeMenu.RowHeight = 25;
		this.treeMenu.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.Default;
		this.treeMenu.Size = new System.Drawing.Size(240, 432);
		this.treeMenu.StateImageList = this.imc_Menu;
		this.treeMenu.TabIndex = 1;
		this.treeMenu.TreeLevelWidth = 12;
		this.treeMenu.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeList1_GetStateImage);
		this.treeMenu.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeList1_NodeCellStyle);
		this.treeMenu.Click += new System.EventHandler(this.treeList1_Click);
		this.treeMenu.MouseLeave += new System.EventHandler(this.treeList1_MouseLeave);
		this.treeMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeList1_MouseMove);
		this.treeListColumn1.Caption = "Name";
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "Name";
		this.treeListColumn1.MinWidth = 34;
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.navGroupReport_Sales.Caption = "Sales A/R";
		this.navGroupReport_Sales.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[1]
		{
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_DocumentDraftsRP1)
		});
		this.navGroupReport_Sales.Name = "navGroupReport_Sales";
		this.navGroupReport_Sales.SelectedLinkIndex = 0;
		this.navGroupReport_Sales.TopVisibleLinkIndex = 9;
		this.navGroupReport_Administration.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navGroupReport_Administration.Appearance.Options.UseFont = true;
		this.navGroupReport_Administration.AppearanceBackground.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navGroupReport_Administration.AppearanceBackground.Options.UseFont = true;
		this.navGroupReport_Administration.AppearanceHotTracked.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navGroupReport_Administration.AppearanceHotTracked.Options.UseFont = true;
		this.navGroupReport_Administration.Caption = "Administration";
		this.navGroupReport_Administration.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText;
		this.navGroupReport_Administration.Name = "navGroupReport_Administration";
		this.navGroupReport_Administration.SelectedLinkIndex = 0;
		this.navGroupReport_Administration.TopVisibleLinkIndex = 4;
		this.navGroupReport_Purchase.Caption = "Purchasing A/P";
		this.navGroupReport_Purchase.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[1]
		{
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_DocumentDraftsRP2)
		});
		this.navGroupReport_Purchase.Name = "navGroupReport_Purchase";
		this.navGroupReport_Purchase.SelectedLinkIndex = 0;
		this.navGroupReport_BusinessPartner.Caption = "Business Partners";
		this.navGroupReport_BusinessPartner.Name = "navGroupReport_BusinessPartner";
		this.navGroupReport_BusinessPartner.SelectedLinkIndex = 0;
		this.navGroupReport_Inventory.Caption = "Inventory";
		this.navGroupReport_Inventory.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText;
		this.navGroupReport_Inventory.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[4]
		{
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_InventoryPostingList),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_StockMovementSummary),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_DocumentDraftsRP3),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nav_frm_BatchTransaction)
		});
		this.navGroupReport_Inventory.Name = "navGroupReport_Inventory";
		this.navGroupReport_Inventory.SelectedLinkIndex = 0;
		this.nav_frm_InventoryPostingList.Caption = "Inventory Posting List";
		this.nav_frm_InventoryPostingList.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_InventoryPostingList.ImageOptions.SmallImage");
		this.nav_frm_InventoryPostingList.Name = "nav_frm_InventoryPostingList";
		this.nav_frm_InventoryPostingList.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nav_frm_InventoryPostingList_LinkClicked);
		this.nav_frm_StockMovementSummary.Caption = "Stock Movement Summary";
		this.nav_frm_StockMovementSummary.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_StockMovementSummary.ImageOptions.SmallImage");
		this.nav_frm_StockMovementSummary.Name = "nav_frm_StockMovementSummary";
		this.nav_frm_StockMovementSummary.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nav_frm_StockMovementSummary_LinkClicked);
		this.nav_frm_BatchTransaction.Caption = "Batch Transaction Report";
		this.nav_frm_BatchTransaction.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nav_frm_BatchTransaction.ImageOptions.SmallImage");
		this.nav_frm_BatchTransaction.Name = "nav_frm_BatchTransaction";
		this.nav_frm_BatchTransaction.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nav_frm_BatchTransaction_LinkClicked);
		this.navGroupReport_Production.Caption = "Production";
		this.navGroupReport_Production.Name = "navGroupReport_Production";
		this.navGroupReport_Production.SelectedLinkIndex = 0;
		this.navGroupReport_MRP.Caption = "Production Planning & MRP";
		this.navGroupReport_MRP.Name = "navGroupReport_MRP";
		this.navGroupReport_MRP.SelectedLinkIndex = 0;
		this.navGroupReport_HRM.Caption = "Human Resources";
		this.navGroupReport_HRM.Name = "navGroupReport_HRM";
		this.navGroupReport_HRM.SelectedLinkIndex = 0;
		this.tab_MdiForm.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.tab_MdiForm.Appearance.Options.UseBackColor = true;
		this.tab_MdiForm.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tab_MdiForm.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover;
		this.tab_MdiForm.Dock = System.Windows.Forms.DockStyle.Top;
		this.tab_MdiForm.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.False;
		this.tab_MdiForm.HeaderButtons = DevExpress.XtraTab.TabButtons.Prev | DevExpress.XtraTab.TabButtons.Next | DevExpress.XtraTab.TabButtons.Close;
		this.tab_MdiForm.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.Always;
		this.tab_MdiForm.IsSearchControl = false;
		this.tab_MdiForm.Location = new System.Drawing.Point(300, 50);
		this.tab_MdiForm.Name = "tab_MdiForm";
		this.tab_MdiForm.ShowHeaderFocus = DevExpress.Utils.DefaultBoolean.False;
		this.tab_MdiForm.Size = new System.Drawing.Size(649, 25);
		this.tab_MdiForm.TabIndex = 31;
		this.tab_MdiForm.Visible = false;
		this.tab_MdiForm.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tab_MdiForm_SelectedPageChanged);
		this.tab_MdiForm.CloseButtonClick += new System.EventHandler(this.tab_MdiForm_CloseButtonClick);
		this.tab_MdiForm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tab_MdiForm_MouseClick);
		this.transitionManager1.FrameCount = 300;
		transition.BarWaitingIndicatorProperties.Caption = "";
		transition.BarWaitingIndicatorProperties.Description = "";
		transition.Control = null;
		transition.LineWaitingIndicatorProperties.AnimationElementCount = 5;
		transition.LineWaitingIndicatorProperties.Caption = "";
		transition.LineWaitingIndicatorProperties.Description = "";
		transition.RingWaitingIndicatorProperties.AnimationElementCount = 5;
		transition.RingWaitingIndicatorProperties.Caption = "";
		transition.RingWaitingIndicatorProperties.Description = "";
		transition.ShowWaitingIndicator = DevExpress.Utils.DefaultBoolean.True;
		transition.TransitionType = transitionType;
		transition.WaitingAnimatorType = DevExpress.Utils.Animation.WaitingAnimatorType.Line;
		transition.WaitingIndicatorProperties.Caption = "Processsing";
		transition.WaitingIndicatorProperties.Description = "Please wait ...";
		this.transitionManager1.Transitions.Add(transition);
		this.notificationContrainerMain.Appearance.BackColor = System.Drawing.Color.Gainsboro;
		this.notificationContrainerMain.Appearance.ForeColor = System.Drawing.Color.Gainsboro;
		this.notificationContrainerMain.Appearance.Options.UseBackColor = true;
		this.notificationContrainerMain.Appearance.Options.UseForeColor = true;
		this.notificationContrainerMain.ButtonControl = this.barBtnNotification;
		this.notificationContrainerMain.Dock = System.Windows.Forms.DockStyle.Right;
		this.notificationContrainerMain.Location = new System.Drawing.Point(749, 75);
		this.notificationContrainerMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.notificationContrainerMain.Name = "notificationContrainerMain";
		this.notificationContrainerMain.Size = new System.Drawing.Size(200, 480);
		this.notificationContrainerMain.TabIndex = 37;
		this.notificationContrainerMain.Visible = false;
		this.adornerUIManager1.Elements.Add(this.guide1);
		this.adornerUIManager1.Elements.Add(this.badge1);
		this.adornerUIManager1.Owner = this;
		this.menu_frm_ListDashboard.Caption = "My Dashboard";
		this.menu_frm_ListDashboard.Id = 605;
		this.menu_frm_ListDashboard.Name = "menu_frm_ListDashboard";
		this.menu_frm_ListDashboard.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menu_frm_ListDashboard_ItemClick);
		base.Appearance.BackColor = System.Drawing.SystemColors.WindowFrame;
		base.Appearance.ForeColor = System.Drawing.SystemColors.WindowFrame;
		base.Appearance.Options.UseBackColor = true;
		base.Appearance.Options.UseFont = true;
		base.Appearance.Options.UseForeColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		base.ClientSize = new System.Drawing.Size(949, 582);
		base.Controls.Add(this.notificationContrainerMain);
		base.Controls.Add(this.tab_MdiForm);
		base.Controls.Add(this.npane_Main);
		base.Controls.Add(this.txt_MenuSearch);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormId = "";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("Main.IconOptions.Icon");
		base.IsMdiContainer = true;
		base.Margin = new System.Windows.Forms.Padding(2);
		this.MdiChildCaptionFormatString = "{1}";
		this.MinimumSize = new System.Drawing.Size(626, 200);
		base.Name = "Main";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "APZON IRS";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
		base.Load += new System.EventHandler(this.Main_Load);
		base.MdiChildActivate += new System.EventHandler(this.Main_MdiChildActivate);
		base.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
		((System.ComponentModel.ISupportInitialize)this.BarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemProgressBar1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dockMain).EndInit();
		this.dockMessageLog.ResumeLayout(false);
		this.dockPanel1_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SystemMessageLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SystemMessageLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imc_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imc_Menu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSearchControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.npane_Main).EndInit();
		this.npane_Main.ResumeLayout(false);
		this.npage_Operations.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.treeMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_MdiForm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.adornerUIManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
