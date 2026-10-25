using System;
using Apzon.HDBConnection.Interfaces;
using Apzon.Services.Interfaces;
using Apzon.Services.Interfaces.Administrator;
using Apzon.Services.Interfaces.Administrator.ApprovalProcess;
using Apzon.Services.Interfaces.Administrator.Banking;
using Apzon.Services.Interfaces.Administrator.Business_Partners;
using Apzon.Services.Interfaces.Administrator.Plugin;
using Apzon.Services.Interfaces.Administrator.Setup;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;
using Apzon.Services.Interfaces.Administrator.Setup.Purchasing;
using Apzon.Services.Interfaces.Administrator.Setup.Service;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;
using Apzon.Services.Interfaces.BusinessPartner;
using Apzon.Services.Interfaces.Commons;
using Apzon.Services.Interfaces.DMS;
using Apzon.Services.Interfaces.DashBoard;
using Apzon.Services.Interfaces.Dashboard;
using Apzon.Services.Interfaces.FacebookCRM;
using Apzon.Services.Interfaces.Financials;
using Apzon.Services.Interfaces.HumanResources;
using Apzon.Services.Interfaces.HumanResources.Salary;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;
using Apzon.Services.Interfaces.ImportData;
using Apzon.Services.Interfaces.Integrated;
using Apzon.Services.Interfaces.Inventory;
using Apzon.Services.Interfaces.Inventory.Batches;
using Apzon.Services.Interfaces.Inventory.InventoryReport;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;
using Apzon.Services.Interfaces.Inventory.ItemManagement;
using Apzon.Services.Interfaces.Inventory.PickAndpack;
using Apzon.Services.Interfaces.Inventory.PriceLists;
using Apzon.Services.Interfaces.KPIs;
using Apzon.Services.Interfaces.Mrps;
using Apzon.Services.Interfaces.Opportunity;
using Apzon.Services.Interfaces.PM;
using Apzon.Services.Interfaces.PackingManagement;
using Apzon.Services.Interfaces.Payment;
using Apzon.Services.Interfaces.PlanningProduction;
using Apzon.Services.Interfaces.Production;
using Apzon.Services.Interfaces.ProductionAddon;
using Apzon.Services.Interfaces.Promotion;
using Apzon.Services.Interfaces.Purchase;
using Apzon.Services.Interfaces.Retail;
using Apzon.Services.Interfaces.RoutingAndResource;
using Apzon.Services.Interfaces.Sales;
using Apzon.Services.Interfaces.Service;
using Apzon.Services.Interfaces.Service.MarketingCampaign;
using Apzon.Services.Interfaces.Util;
using Apzon.Services.Interfaces.Widget;

namespace Apzon.Services;

public interface IUnitOfWork : IDisposable
{
	string ConnectionString { get; set; }

	IChartOfAccount ChartOfAccount { get; }

	IDocumentConfirm DocumentConfirm { get; }

	IMaterialRevaluation MaterialRevaluation { get; }

	IJournalEntries JournalEntries { get; }

	IDimension Dimension { get; }

	IDistributeRule DistributeRule { get; }

	ICostCenters CostCenters { get; }

	IMps Mps { get; }

	IMrpPlan MrpPlan { get; }

	INotificationSetting NotificationSetting { get; }

	ISalesPersons SalesPersons { get; }

	IShippingTypes ShippingTypes { get; }

	IManufacturers Manufacturers { get; }

	IItemGroups ItemGroups { get; }

	IItemProperties ItemProperties { get; }

	IVatGroups VatGroups { get; }

	IPriceLists PriceLists { get; }

	IWarehouse Warehouse { get; }

	IRequester Requester { get; }

	IAcctCode AcctCode { get; }

	ILocation Location { get; }

	IDepartment Department { get; }

	IUsers Users { get; }

	IGeneralSetting GeneralSetting { get; }

	IHoliday Holiday { get; }

	IAuths Auths { get; }

	IPrintLayout PrintLayout { get; }

	IProject Project { get; }

	IFilterSetting FilterSetting { get; }

	IWorkingTable WorkingTable { get; }

	IFreight Freight { get; }

	IGroupCustomerVender GroupCustomerVender { get; }

	IBusinessPartnerProperties BusinessPartnerProperties { get; }

	IBusinessPrioritiesSetup BusinessPrioritiesSetup { get; }

	ICreditCard CreditCard { get; }

	ILengthAndWidth LengthAndWidth { get; }

	IState State { get; }

	IAdministrator Administrator { get; }

	IWeight Weight { get; }

	ICurrencies Currencies { get; }

	ISalesEmployeeBuyer SalesEmployeeBuyer { get; }

	ILoginIRS LoginIRS { get; }

	IUsersIRS UsersIRS { get; }

	IUserDefault UserDefault { get; }

	IBinFieldActivation BinFieldActivation { get; }

	IWHSubSetup WHSubSetup { get; }

	IAttrCodeSetup AttrCodeSetup { get; }

	IBinLocList BinLocList { get; }

	IBinLocMaster BinLocMaster { get; }

	IWHSubManager WHSubManager { get; }

	IWHSubSetGenPre WHSubSetGenPre { get; }

	ICreditCardPaymentMethods CreditCardPaymentMethods { get; }

	IBranches Branches { get; }

	IXtraReportManager XtraReportManager { get; }

	IInventoryCycles InventoryCycles { get; }

	ICycleCountDetermination CycleCountDetermination { get; }

	ICycleCountDeterminationSubTable CycleCountDeterminationSubTable { get; }

	IVehicle Vehicle { get; }

	IVehicleType VehicleType { get; }

	ILandedCosts LandedCosts { get; }

	ITerritory Territory { get; }

	IPluginManager PluginManager { get; }

	ILicenseManager LicenseManager { get; }

	IBarConfig BarConfig { get; }

	IAlertOverview AlertOverview { get; }

	ILandedCostsSetup LandedCostsSetup { get; }

	ICompanyDetails CompanyDetails { get; }

	IDocumentNumbering DocumentNumbering { get; }

	ISeries Series { get; }

	IDocumentSettings DocumentSettings { get; }

	IMenuStructure MenuStructure { get; }

	IUnitOfMeasureGroups UnitOfMeasureGroups { get; }

	IUoMGroupDefinition UoMGroupDefinition { get; }

	IUnitOfMeasure UnitOfMeasure { get; }

	IFormWizard FormWizard { get; }

	IExternalRouting ExternalRouting { get; }

	IJobsManagement JobsManagement { get; }

	IBusinessPartnerss BP { get; }

	ICardGroups CardGroup { get; }

	ISaleEmployees SaleEmployee { get; }

	ICashDiscounts CashDiscount { get; }

	IOCashDiscounts OCashDiscount { get; }

	IPaymentTerms PaymentTerm { get; }

	ICreditCardTypes CreditCardType { get; }

	IBusinessMasterDatas BusinessMasterData { get; }

	IContactPersons ContactPerson { get; }

	ICountries Countries { get; }

	IAddresses Addresses { get; }

	IIndustry Industry { get; }

	IContractTemplate ContractTemplate { get; }

	ICustomsGroup CustomsGroup { get; }

	IItemMasterData Items { get; }

	ISpecialBP SpecialBP { get; }

	ISpecialBP_LineSPP1 SpecialBP_LineSPP1 { get; }

	ISpecialBP_LineSPP2 SpecialBP_LineSPP2 { get; }

	IOCYC OCYC { get; }

	IDocumentDraft DocumentDraft { get; }

	IAtp Atp { get; }

	IDeliveryControl DeliveryControl { get; }

	ISalesControl SalesControl { get; }

	ISalesBlanketAgreement SalesBlanketAgreement { get; }

	IDeliveryConfirm DeliveryConfirm { get; }

	IHandoverDocument HandoverDocument { get; }

	IEmpToken EmpToken { get; }

	IAuthorToken AuthorToken { get; }

	IPurchaseRequest PurchaseRequest { get; }

	IILandedCost PLandedCost { get; }

	IPurchaseRequestControl PurchaseRequestControl { get; }

	IFacebookCRM FacebookCRM { get; }

	IFacebookManager FacebookManager { get; }

	ICommons Commons { get; }

	IUDFs UDFs { get; }

	IUDTs UDTs { get; }

	IExRateAndIndex ExRateAndIndex { get; }

	IChangeLog ChangeLog { get; }

	IFormSetting FormSetting { get; }

	IUDOs UDOs { get; }

	IUDRs UDRs { get; }

	ICFL CFL { get; }

	IQueries Query { get; }

	IFormula Formula { get; }

	ICreateFormula CreateFormula { get; }

	IDiagram Diagram { get; }

	IORW ORW { get; }

	IDefineNew DefineNew { get; }

	IUserDefinedTexts UserDefinedTexts { get; }

	IContextAction ContextAction { get; }

	ISearchSetting SearchSetting { get; }

	IFormFilterSetting FormFilterSetting { get; }

	IExternalRequest ExternalRequest { get; }

	IApprovalDecision ApprovalDecision { get; }

	IPublishService PublishService { get; }

	IDashBoard DashBoard { get; }

	IDashboardCategories DashboardCategories { get; }

	IDashBoardTabs DashBoardTabs { get; }

	IDashboardAuthorization DashboardAuthorization { get; }

	ICategoriesDAB CategoriesDAB { get; }

	IDashboardDetail DashboardDetail { get; }

	IMobileDashboard MobileDashboard { get; }

	IGoodReceiptIt GoodsReceipt { get; }

	IGoodsIssueIt GoodsIssueIt { get; }

	ITranferRequest TranferRequest { get; }

	IInventoryTranfer InventoryTranfer { get; }

	IInventoryRevaluation InventoryRevaluation { get; }

	IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers { get; }

	IBatchDetail BatchDetail { get; }

	IBatchTransaction BatchTransaction { get; }

	IInventoryConfirm InventoryConfirm { get; }

	IPickAndPack PickAndPack { get; }

	IInventoryCoutingBin InventoryCoutingBin { get; }

	IStockCounting StockCounting { get; }

	IInventoryCountingRequest InventoryCountingRequest { get; }

	IInventoryCounting InventoryCounting { get; }

	ISTDPriceLists STDPriceLists { get; }

	IInventoryInWareHouse InventoryInWareHouse { get; }

	IPeriodAndVolumeDiscounts PeriodAndVolumeDiscounts { get; }

	IStockRequest StockRequest { get; }

	IDiscountGroup DiscountGroup { get; }

	IInventoryPostingList InventoryPostingList { get; }

	IInventoryReport InventoryReport { get; }

	ISerialNumberDetails SerialNumberDetails { get; }

	ISerialNumberTransactions SerialNumberTransactions { get; }

	IBatchesControl BatchesControl { get; }

	ICycleCountRecommendation CycleCountRecommendation { get; }

	IPicking Picking { get; }

	IPacking Packing { get; }

	IShipping Shipping { get; }

	IAlternativeItems AlternativeItems { get; }

	IIncomingPayment IncomingPayment { get; }

	IOutgoingPayment OutgoingPayment { get; }

	ICashItemMasterData CashItemMasterData { get; }

	IVoucherMasterData VoucherMasterData { get; }

	ICouponMasterData CouponMasterData { get; }

	IInPayment InPayment { get; }

	IOutPayment OutPayment { get; }

	IIncomingPayments IncomingPayments { get; }

	IOutgoingPayments OutgoingPayments { get; }

	IPaymentRecon PaymentRecon { get; }

	IPaymentRequest PaymentRequest { get; }

	IMKPurchaseOrder MKPurchaseOrder { get; }

	IPaymentWizard PaymentWizard { get; }

	IPayments Payments { get; }

	ITrackingIssue TrackingIssue { get; }

	IProjectManager ProjectManager { get; }

	IProjectReportCompletion ReportCompletion { get; }

	IProjectConfirm ProjectConfirm { get; }

	IFormatRule FormatRule { get; }

	IWBSTemplate WBSTemplate { get; }

	IGanttControl GanttControl { get; }

	IBillOfMasterials BillOfMasterials { get; }

	IBoMJob BoMJob { get; }

	IProductionOder ProductionOrder { get; }

	IIssueForProduction IssueForProduction { get; }

	IReceiptProduction ReceiptProduction { get; }

	IProductionForBatch ProductionForBatch { get; }

	IProductionConfirm ProductionConfirm { get; }

	IBomVersion BomVersion { get; }

	IMaterialShortageReport MaterialShortageReport { get; }

	IProductionOrderImportTool ProductionOrderImportTool { get; }

	IOutsourcing Outsourcing { get; }

	IApzBillOfMaterials ApzBillOfMaterials { get; }

	IMasterOrderReplan MasterOrderReplan { get; }

	IResources Resources { get; }

	IControlKey ControlKey { get; }

	IOperation Operation { get; }

	IRouting Routing { get; }

	ITinhGiay TinhGiay { get; }

	IGroupControlKey GroupControlKey { get; }

	IListPlant ListPlant { get; }

	IProductionControl ProductionControl { get; }

	IProductionPlanning ProductionPlanning { get; }

	ISpecialIssueForBatch SpecialIssueForBatch { get; }

	IReportCompleteForProduction ReportCompleteForProduction { get; }

	IResourceWorkCenter ResourceWorkCenter { get; }

	IOpportunity Opportunity { get; }

	IOpportunityStage OpportunityStage { get; }

	IOpportunityPartner OpportunityPartner { get; }

	IOpportunityRelationship OpportunityRelationship { get; }

	IOpportunityCompetitor OpportunityCompetitor { get; }

	IOpportunityReason OpportunityReason { get; }

	IActivitiesForOpportunity ActivitiesForOpportunity { get; }

	IActivityType ActivityType { get; }

	IActivitySubjects ActivitySubjects { get; }

	IActivityMeetingLocation ActivityMeetingLocation { get; }

	IActivityStatus ActivityStatus { get; }

	IActivityCalendar ActivityCalendar { get; }

	ICalendarSetting CalendarSetting { get; }

	IMailSetting MailSetting { get; }

	IInterest Interest { get; }

	IInterestLevel InterestLevel { get; }

	IEmpMasterData EmpMasterData { get; }

	IHREducation HREducation { get; }

	IEmpContract EmpContract { get; }

	IEmpRelatives EmpRelatives { get; }

	IJobVacancy JobVacancy { get; }

	IJVApplicants JVApplicants { get; }

	IHRMQualifications HRMQualifications { get; }

	IJVInterview JVInterview { get; }

	IJVQuestion JVQuestion { get; }

	IJVAnswer JVAnswer { get; }

	IAllowanceType AllowanceType { get; }

	IPerIncomeTax PerIncomeTax { get; }

	ISalaryInfo SalaryInfo { get; }

	ISalaryInfoMainByEmp SalaryInfoMainByEmp { get; }

	ISalaryAllowanceByEmp SalaryAllowanceByEmp { get; }

	ISalaryInfoAllowance SalaryInfoAllowance { get; }

	ISalaryInfoAdvance SalaryInfoAdvance { get; }

	ISalaryCalculation SalaryCalculation { get; }

	ISalaryFormula SalaryFormula { get; }

	IPenalType PenalType { get; }

	IEmpPenalization EmpPenalization { get; }

	ILeaveParameter LeaveParameter { get; }

	IStates States { get; }

	ITimeSheeting TimeSheeting { get; }

	IWorkPeriod WorkPeriod { get; }

	ITimeSheetingHour TimeSheetingHour { get; }

	ILeaveInformation LeaveInformation { get; }

	IHolidaySetup HolidaySetup { get; }

	IInOutMethod InOutMethod { get; }

	IWorkShift WorkShift { get; }

	IWorkScheduleSetup WorkScheduleSetup { get; }

	IWorkScheduleRegister WorkScheduleRegister { get; }

	IDeductionType DeductionType { get; }

	ISalaryParameters SalaryParameters { get; }

	IMRP MRP { get; }

	ISchedule Schedule { get; }

	ISalesForeCast SalesForeCast { get; }

	IForecast Forecast { get; }

	IAdvanceForecast AdvanceForecast { get; }

	IShift Shift { get; }

	IWorkScheduling WorkScheduling { get; }

	ILines Lines { get; }

	IItemQuotaArrangement ItemQuotaArrangement { get; }

	IItemQuotaArrangementDate ItemQuotaArrangementDate { get; }

	IJetstarPreOrder JetstarPreOrder { get; }

	IServiceCall ServiceCall { get; }

	Isolution Solution { get; }

	IServiceContract ServiceContract { get; }

	IServiceCallSeting ServiceCallSetting { get; }

	ISubActivities SubActivities { get; }

	IWarrantyCard WarrantyCard { get; }

	IServiceLevelAgreement ServiceLevelAgreement { get; }

	IServiceCalls ServiceCalls { get; }

	IServiceContractTemplate ServiceContractTemplate { get; }

	IMarketingCampaign MarketingCampaign { get; }

	IIrsConfig IrsConfig { get; }

	IIntegrated Integrated { get; }

	IIntegratedInventoryTransfer IntegratedInventoryTransfer { get; }

	IIntegratedBP IntegratedBP { get; }

	IIntegratedWarehouse IntegratedWarehouse { get; }

	IPackingBatch PackingBatch { get; }

	IIntegratedUdo IntegratedUdo { get; }

	IIntegratedItem IntegratedItem { get; }

	IImportDataBP ImportDataBP { get; }

	IImportItemMasterData ImportItemMasterData { get; }

	IImportBOM ImportBom { get; }

	IImportData ImportData { get; }

	IAdvancePeriodClosing AdvancePeriodClosing { get; }

	IDocument Documents { get; }

	IRetail Retail { get; }

	IRetailStore RetailStore { get; }

	IPosMaster PosMaster { get; }

	IRetailItemSearch ItemSearch { get; }

	IRetailCustomerSearch CustomerSearch { get; }

	IRetailChooseFromList RetailChooseFromList { get; }

	IRetailCmdConfig RetailCmdConfig { get; }

	IWidget Widget { get; }

	ICapacity Capacity { get; }

	IOperations Operations { get; }

	IAdvanceRouting AdvanceRouting { get; }

	IResourceWorkSchedule ResourceWorkSchedule { get; }

	IAdditionalGeneralSettings AdditionalGeneralSettings { get; }

	IFile Files { get; }

	IResourceGroup ResourceGroup { get; }

	ISMSConfig SmsConfig { get; }

	ICapacitySetting CapacitySetting { get; }

	IBarcodeInventoryCounting BarcodeInventoryCounting { get; }

	ICommentCustomer CommentCustomer { get; }

	IContactUs ContactUs { get; }

	IGeography Geography { get; }

	IStore Store { get; }

	ISaleTerritory SaleTerritory { get; }

	IVisitStore VisitStore { get; }

	ISurvey Survey { get; }

	IQuestionMasterData QuestionMasterData { get; }

	IUploadImage UploadImage { get; }

	ISettingReport SettingReport { get; }

	IOutletVisitingFrequency OutletVisitingFrequency { get; }

	IPromotionSetup PromotionSetup { get; }

	IChannel Channel { get; }

	ISalesRoute SalesRoute { get; }

	IPromotionCondition PromotionCondition { get; }

	ITrackingUserLocation TrackingUserLocation { get; }

	IPromotion Promotion { get; }

	IDMSReport DMSReport { get; }

	IPromotionRegister PromotionRegister { get; }

	IPromotionBenefit PromotionBenefit { get; }

	ICheckPromotion CheckPromotion { get; }

	IPromotionProgram PromotionProgram { get; }

	IImageAlbum ImageAlbum { get; }

	IMarker Marker { get; }

	IDMSDelivery DMSDelivery { get; }

	IAlertTemplate AlertTemplate { get; }

	IDMSApproved DMSApproved { get; }

	IDMSSettingUDF DMSSettingUDF { get; }

	IPackingOrder PackingOrder { get; }

	ICreateTypeCase CreateTypeCase { get; }

	ITraceability Traceability { get; }

	IPrintBillWrap PrintBillWrap { get; }

	IApprovalStages ApprovalStages { get; }

	IApprovalTemplates ApprovalTemplates { get; }

	IWorkingProcess WorkingProcess { get; }

	INotification Notification { get; }

	IOutlet Outlet { get; }

	IKPIsMaterData KPIsMaterData { get; }

	IKPIsSetting KPIsSetting { get; }

	IListPointKPIs ListPointKPIs { get; }

	ISetIndicatorKPIs SetIndicatorKPIs { get; }

	IKPIPlan KPIPlan { get; }

	IKPIScope KPIScope { get; }

	IKPIIndicator KPIIndicator { get; }

	IKPIGroup KPIGroup { get; }

	IKPISetup KPISetup { get; }

	IKPIScore KPIScore { get; }

	IThirdPartyToken ThirdPartyToken { get; }

	IAboutRangeTime AboutRangeTime { get; }

	ISettingApprovalDetail SettingApprovalDetail { get; }

	IComments Comments { get; }

	ITransaction BeginTransaction();

	void EndTransaction(ITransaction transaction);

	string ConvertErrorMessage(string error);
}
