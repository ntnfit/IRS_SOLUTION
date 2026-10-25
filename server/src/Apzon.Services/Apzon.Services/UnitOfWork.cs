using System;
using Apzon.HDBConnection;
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
using Apzon.Services.Repository.Administrator;
using Apzon.Services.Repository.Administrator.ApprovalProcess;
using Apzon.Services.Repository.Administrator.Setup;
using Apzon.Services.Repository.BusinessPartner;
using Apzon.Services.Repository.Commons;
using Apzon.Services.Repository.DMS;
using Apzon.Services.Repository.FacebookCRM;
using Apzon.Services.Repository.HumanResources.TimeSheeting;
using Apzon.Services.Repository.ImportData;
using Apzon.Services.Repository.Integrated;
using Apzon.Services.Repository.Inventory.InventoryTransaction;
using Apzon.Services.Repository.Inventory.ItemManagement;
using Apzon.Services.Repository.KPIs;
using Apzon.Services.Repository.PM;
using Apzon.Services.Repository.Payment;
using Apzon.Services.Repository.Promotion;
using Apzon.Services.Repository.RoutingAndResource;
using Apzon.Services.SqlRepository.Administrator;
using Apzon.Services.SqlRepository.Administrator.Banking;
using Apzon.Services.SqlRepository.Administrator.Business_Partner;
using Apzon.Services.SqlRepository.Administrator.Plugin;
using Apzon.Services.SqlRepository.Administrator.Setup;
using Apzon.Services.SqlRepository.Administrator.Setup.Inventory;
using Apzon.Services.SqlRepository.Administrator.Setup.Purchasing;
using Apzon.Services.SqlRepository.Administrator.Setup.Service;
using Apzon.Services.SqlRepository.Administrator.SystemInitialization;
using Apzon.Services.SqlRepository.BusinessPartner;
using Apzon.Services.SqlRepository.Commons;
using Apzon.Services.SqlRepository.DashBoard;
using Apzon.Services.SqlRepository.Document;
using Apzon.Services.SqlRepository.FacebookCRM;
using Apzon.Services.SqlRepository.Financials;
using Apzon.Services.SqlRepository.HumanResources;
using Apzon.Services.SqlRepository.HumanResources.Salary;
using Apzon.Services.SqlRepository.HumanResources.TimeSheeting;
using Apzon.Services.SqlRepository.Integrated;
using Apzon.Services.SqlRepository.Inventory;
using Apzon.Services.SqlRepository.Inventory.Batches;
using Apzon.Services.SqlRepository.Inventory.InventoryReport;
using Apzon.Services.SqlRepository.Inventory.InventoryTransaction;
using Apzon.Services.SqlRepository.Inventory.ItemManagement;
using Apzon.Services.SqlRepository.Inventory.PickAndPack;
using Apzon.Services.SqlRepository.Inventory.PickAndPack.PriceLists;
using Apzon.Services.SqlRepository.Inventory.PriceLists;
using Apzon.Services.SqlRepository.Mrps;
using Apzon.Services.SqlRepository.Opportunity;
using Apzon.Services.SqlRepository.PM;
using Apzon.Services.SqlRepository.PackingManagement;
using Apzon.Services.SqlRepository.Payment;
using Apzon.Services.SqlRepository.PlanningProduction;
using Apzon.Services.SqlRepository.Production;
using Apzon.Services.SqlRepository.ProdutionAddon;
using Apzon.Services.SqlRepository.Purchase;
using Apzon.Services.SqlRepository.Retail;
using Apzon.Services.SqlRepository.RoutingAndResource;
using Apzon.Services.SqlRepository.Sales;
using Apzon.Services.SqlRepository.Service;
using Apzon.Services.SqlRepository.Service.MarketingCampaign;
using Apzon.Services.SqlRepository.Util;
using Apzon.Services.SqlRepository.Widget;

namespace Apzon.Services;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
	private string _connectionString;

	private bool _disposed;

	public IPackingBatch PackingBatch => new SqlPackingBatchRepository(new DatabaseServiceSql(_connectionString));

	public string ConnectionString
	{
		get
		{
			return _connectionString;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public ISalesPersons SalesPersons => new SqlSalesPersonsRepository(new DatabaseServiceSql(_connectionString));

	public IShippingTypes ShippingTypes => new SqlShippingTypesRepository(new DatabaseServiceSql(_connectionString));

	public IManufacturers Manufacturers => new SqlManufacturersRepository(new DatabaseServiceSql(_connectionString));

	public INotificationSetting NotificationSetting => new SqlNotificationSettingRepository(new DatabaseServiceSql(_connectionString));

	public IItemGroups ItemGroups => new SqlItemGroupsRepository(new DatabaseServiceSql(_connectionString));

	public IItemProperties ItemProperties => new SqlItemPropertiesRepository(new DatabaseServiceSql(_connectionString));

	public IVatGroups VatGroups => new SqlVatGroupsRepository(new DatabaseServiceSql(_connectionString));

	public IPriceLists PriceLists => new SqlPriceListsRepository(new DatabaseServiceSql(_connectionString));

	public IWarehouse Warehouse => new SqlWarehouseRepository(new DatabaseServiceSql(_connectionString));

	public IRequester Requester => new SqlRequesterRepository(new DatabaseServiceSql(_connectionString));

	public IAcctCode AcctCode => new SqlAcctCodeRepository(new DatabaseServiceSql(_connectionString));

	public ILocation Location => new SqlLocationRepository(new DatabaseServiceSql(_connectionString));

	public ICustomsGroup CustomsGroup => new SqlCustomsGroupRepository(new DatabaseServiceSql(_connectionString));

	public IDepartment Department => new SqlDepartmentRepository(new DatabaseServiceSql(_connectionString));

	public IUsers Users => new SqlUsersRepository(new DatabaseServiceSql(_connectionString));

	public IGeneralSetting GeneralSetting => new SqlGeneralSettingRepository(new DatabaseServiceSql(_connectionString));

	public IHoliday Holiday => new SqlHolidayRepository(new DatabaseServiceSql(_connectionString));

	public IAuths Auths => new SqlAuthRepository(new DatabaseServiceSql(_connectionString));

	public IProject Project => new SqlProjectRepository(new DatabaseServiceSql(_connectionString));

	public IPrintLayout PrintLayout => new SqlPrintLayoutRepository(new DatabaseServiceSql(_connectionString));

	public IFilterSetting FilterSetting => new SqlFilterSettingRepository(new DatabaseServiceSql(_connectionString));

	public IWorkingTable WorkingTable => new SqlWorkingTableRepository(new DatabaseServiceSql(_connectionString));

	public IFreight Freight => new SqlFreightRepository(new DatabaseServiceSql(_connectionString));

	public ILengthAndWidth LengthAndWidth => new SqlLengthAndWidthRepository(new DatabaseServiceSql(_connectionString));

	public IGroupCustomerVender GroupCustomerVender => new SqlGroupCustomerVenderRepository(new DatabaseServiceSql(_connectionString));

	public IBusinessPartnerProperties BusinessPartnerProperties => new SqlBusinessPartnerPropertiesRepository(new DatabaseServiceSql(_connectionString));

	public IBusinessPrioritiesSetup BusinessPrioritiesSetup => new SqlBusinessPartnerPrioritiesRepository(new DatabaseServiceSql(_connectionString));

	public ICreditCard CreditCard => new SqlCreditCardRepository(new DatabaseServiceSql(_connectionString));

	public ICreditCardPaymentMethods CreditCardPaymentMethods => new SqlCreditCardPaymentMehodsRepository(new DatabaseServiceSql(_connectionString));

	public IAdministrator Administrator => new SqlAdministratorRepository(new DatabaseServiceSql(_connectionString));

	public IState State => new SqlStateRepository(new DatabaseServiceSql(_connectionString));

	public IWeight Weight => new SqlWeightRepository(new DatabaseServiceSql(_connectionString));

	public ILoginIRS LoginIRS => new SqlLoginIRSRepository(new DatabaseServiceSql(_connectionString));

	public IUsersIRS UsersIRS => new SqlUsersIRSRepository(new DatabaseServiceSql(_connectionString));

	public IBinFieldActivation BinFieldActivation => new SqlBinFieldActivationRepository(new DatabaseServiceSql(_connectionString));

	public IWHSubSetup WHSubSetup => new SqlWHSubSetupRepository(new DatabaseServiceSql(_connectionString));

	public IAttrCodeSetup AttrCodeSetup => new SqlAttrCodeSetupRepository(new DatabaseServiceSql(_connectionString));

	public IBinLocList BinLocList => new SqlBinLocListRepository(new DatabaseServiceSql(_connectionString));

	public IBinLocMaster BinLocMaster => new SqlBinLocMasterRepository(new DatabaseServiceSql(_connectionString));

	public IWHSubManager WHSubManager => new SqlWHSubManagerRepository(new DatabaseServiceSql(_connectionString));

	public IWHSubSetGenPre WHSubSetGenPre => new SqlWHSubSetGenPreRepository(new DatabaseServiceSql(_connectionString));

	public IBarConfig BarConfig => new SqlBarConfigRepository(new DatabaseServiceSql(_connectionString));

	public IBranches Branches => new SqlBranchesRepository(new DatabaseServiceSql(_connectionString));

	public IXtraReportManager XtraReportManager => new SqlXtraReportManagerRepository(new DatabaseServiceSql(_connectionString));

	public IInventoryCycles InventoryCycles => new SqlInventoryCyclesRepository(new DatabaseServiceSql(_connectionString));

	public ICycleCountDetermination CycleCountDetermination => new SqlCycleCountDeterminationRepository(new DatabaseServiceSql(_connectionString));

	public ICycleCountDeterminationSubTable CycleCountDeterminationSubTable => new SqlCycleCountDeterminationSubTableRepository(new DatabaseServiceSql(_connectionString));

	public IVehicle Vehicle => new SqlVehicleRepository(new DatabaseServiceSql(_connectionString));

	public IVehicleType VehicleType => new SqlVehicleTypeRepository(new DatabaseServiceSql(_connectionString));

	public ILandedCostsSetup LandedCostsSetup => new SqlLandedCostsSetupRepository(new DatabaseServiceSql(_connectionString));

	public ILandedCosts LandedCosts => new SqlLandedCostsRepository(new DatabaseServiceSql(_connectionString));

	public ITerritory Territory => new SqlTerritoryRepository(new DatabaseServiceSql(_connectionString));

	public IPluginManager PluginManager => new SqlPluginManagerRepository(new DatabaseServiceSql(_connectionString));

	public ILicenseManager LicenseManager => new SqlLicenseManagerRepository(new DatabaseServiceSql(_connectionString));

	public IAlertOverview AlertOverview => new SqlAlertOverviewRepository(new DatabaseServiceSql(_connectionString), this);

	public ICompanyDetails CompanyDetails => new SqlCompanyDetailsRepository(new DatabaseServiceSql(_connectionString));

	public IDocumentNumbering DocumentNumbering => new SqlDocumentNumberingRepository(new DatabaseServiceSql(_connectionString));

	public ISeries Series => new SqlSeriesRepository(new DatabaseServiceSql(_connectionString));

	public IDocumentSettings DocumentSettings => new SqlDocumentSettingsRepository(new DatabaseServiceSql(_connectionString));

	public IMenuStructure MenuStructure => new SqlMenuStructureRepository(new DatabaseServiceSql(_connectionString));

	public IUnitOfMeasureGroups UnitOfMeasureGroups => new SqlUnitOfMeasureGroupsRepository(new DatabaseServiceSql(_connectionString));

	public IUoMGroupDefinition UoMGroupDefinition => new SqlUoMGroupDefinitionRepository(new DatabaseServiceSql(_connectionString));

	public IUnitOfMeasure UnitOfMeasure => new SqlUnitOfMeasureRepository(new DatabaseServiceSql(_connectionString));

	public IFacebookCRM FacebookCRM => new SqlFacebookCRMRepository(new DatabaseServiceSql(_connectionString));

	public IFacebookManager FacebookManager => new FacebookManagerRepository(new DatabaseServiceSql(_connectionString));

	public ICurrencies Currencies => new SqlCurrenciesRepository(new DatabaseServiceSql(_connectionString));

	public ISalesEmployeeBuyer SalesEmployeeBuyer => new SqlSalesEmployeeBuyerRepository(new DatabaseServiceSql(_connectionString));

	public IDimension Dimension => new SqlDimensionRepository(new DatabaseServiceSql(_connectionString));

	public IDistributeRule DistributeRule => new SqlDistributeRuleRepository(new DatabaseServiceSql(_connectionString));

	public ICountries Countries => new SqlCountriesRepository(new DatabaseServiceSql(_connectionString));

	public IUserDefault UserDefault => new SqlUserDefaultRepository(new DatabaseServiceSql(_connectionString));

	public IFormWizard FormWizard => new SqlFormWizardRepository(new DatabaseServiceSql(_connectionString));

	public IExternalRouting ExternalRouting => new ExternalRoutingRepository(new DatabaseServiceSql(_connectionString));

	public IJobsManagement JobsManagement => new JobsManagementRepository(new DatabaseServiceSql(_connectionString));

	public IEmpMasterData EmpMasterData => new SqlEmpMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IHREducation HREducation => new SqlHREducationRepository(new DatabaseServiceSql(_connectionString));

	public IEmpContract EmpContract => new SqlEmpContractRepository(new DatabaseServiceSql(_connectionString));

	public IEmpRelatives EmpRelatives => new SqlEmpRelativesRepository(new DatabaseServiceSql(_connectionString));

	public IJobVacancy JobVacancy => new SqlJobVacancyRepository(new DatabaseServiceSql(_connectionString));

	public IJVApplicants JVApplicants => new SqlJVApplicantsRepository(new DatabaseServiceSql(_connectionString));

	public IHRMQualifications HRMQualifications => new SqlHRMQualificationsRepository(new DatabaseServiceSql(_connectionString));

	public IJVInterview JVInterview => new SqlJVInterviewRepository(new DatabaseServiceSql(_connectionString));

	public IJVQuestion JVQuestion => new SqlJVQuestionRepository(new DatabaseServiceSql(_connectionString));

	public IJVAnswer JVAnswer => new SqlJVAnswerRepository(new DatabaseServiceSql(_connectionString));

	public ITimeSheeting TimeSheeting => new SqlTimeSheetingRepository(new DatabaseServiceSql(_connectionString));

	public IWorkPeriod WorkPeriod => new SqlWorkPeriodRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryInfo SalaryInfo => new SqlSalaryInfoRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryInfoMainByEmp SalaryInfoMainByEmp => new SqlSalaryInfoMainByEmpRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryAllowanceByEmp SalaryAllowanceByEmp => new SqlSalaryAllowanceByEmpRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryInfoAllowance SalaryInfoAllowance => new SqlSalaryInfoAllowanceRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryInfoAdvance SalaryInfoAdvance => new SqlSalaryInfoAdvanceRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryCalculation SalaryCalculation => new SqlSalaryCalculationRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryFormula SalaryFormula => new SqlSalaryFormulaRepository(new DatabaseServiceSql(_connectionString));

	public IAllowanceType AllowanceType => new SqlAllowanceTypeRepository(new DatabaseServiceSql(_connectionString));

	public IPerIncomeTax PerIncomeTax => new SqlPerIncomeTaxRepository(new DatabaseServiceSql(_connectionString));

	public ITimeSheetingHour TimeSheetingHour => new SqlTimeSheetingHourRepository(new DatabaseServiceSql(_connectionString));

	public IPenalType PenalType => new SqlPenalTypeRepository(new DatabaseServiceSql(_connectionString));

	public IEmpPenalization EmpPenalization => new SqlEmpPenalizationRepository(new DatabaseServiceSql(_connectionString));

	public ILeaveParameter LeaveParameter => new SqlLeaveParameterRepository(new DatabaseServiceSql(_connectionString));

	public IStates States => new SqlStatesRepository(new DatabaseServiceSql(_connectionString));

	public ILeaveInformation LeaveInformation => new SqlLeaveInformationRepository(new DatabaseServiceSql(_connectionString));

	public IHolidaySetup HolidaySetup => new SqlHolidaySetupRepository(new DatabaseServiceSql(_connectionString));

	public IInOutMethod InOutMethod => new SqlInOutMethodRepository(new DatabaseServiceSql(_connectionString));

	public IWorkShift WorkShift => new SqlWorkShiftRepository(new DatabaseServiceSql(_connectionString));

	public IWorkScheduleSetup WorkScheduleSetup => new SqlWorkScheduleSetupRepository(new DatabaseServiceSql(_connectionString));

	public IWorkScheduleRegister WorkScheduleRegister => new SqlWorkScheduleRegisterRepository(new DatabaseServiceSql(_connectionString));

	public IDocumentConfirm DocumentConfirm => new SqlDocumentConfirmRepository(new DatabaseServiceSql(_connectionString));

	public IChartOfAccount ChartOfAccount => new SqlChartOfAccountRepository(new DatabaseServiceSql(_connectionString));

	public IMaterialRevaluation MaterialRevaluation => new SqlMaterialRevaluationRepository(new DatabaseServiceSql(_connectionString));

	public IJournalEntries JournalEntries => new SqlJournalEntriesRepository(new DatabaseServiceSql(_connectionString));

	public ICostCenters CostCenters => new SqlCostCentersRepository(new DatabaseServiceSql(_connectionString));

	public IBusinessPartnerss BP => new SqlBusinessPartnerRepository(new DatabaseServiceSql(_connectionString));

	public ICardGroups CardGroup => new SqlCardGroupRepository(new DatabaseServiceSql(_connectionString));

	public ISaleEmployees SaleEmployee => new SqlSaleEmployeeRepository(new DatabaseServiceSql(_connectionString));

	public ICashDiscounts CashDiscount => new SqlCashDiscountRepository(new DatabaseServiceSql(_connectionString));

	public IOCashDiscounts OCashDiscount => new SqlOCashDiscountRepository(new DatabaseServiceSql(_connectionString));

	public IPaymentTerms PaymentTerm => new SqlPaymentTermRepository(new DatabaseServiceSql(_connectionString));

	public ICreditCardTypes CreditCardType => new SqlCreditCardTypeRepository(new DatabaseServiceSql(_connectionString));

	public IBusinessMasterDatas BusinessMasterData => new SqlBusinessMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IContactPersons ContactPerson => new SqlContactPersonRepository(new DatabaseServiceSql(_connectionString));

	public IAddresses Addresses => new SqlBpAddressRepository(new DatabaseServiceSql(_connectionString));

	public IIndustry Industry => new SqlIndustryRepository(new DatabaseServiceSql(_connectionString));

	public IContractTemplate ContractTemplate => new SqlContractTemplateRepository(new DatabaseServiceSql(_connectionString));

	public IItemMasterData Items => new SqlItemMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IGoodReceiptIt GoodsReceipt => new SqlGoodReceiptItRespository(new DatabaseServiceSql(_connectionString));

	public IGoodsIssueIt GoodsIssueIt => new SqlGoodsIssueItRespository(new DatabaseServiceSql(_connectionString));

	public ITranferRequest TranferRequest => new SqlTranferRequestRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryTranfer InventoryTranfer => new SqlInventoryTranferRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryRevaluation InventoryRevaluation => new SqlInventoryRevaluationRespository(new DatabaseServiceSql(_connectionString));

	public IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers => new SqlBusinessPartnerCatalogNumbersRespository(new DatabaseServiceSql(_connectionString));

	public IBatchDetail BatchDetail => new SqlBathDetailsRepository(new DatabaseServiceSql(_connectionString));

	public IBatchTransaction BatchTransaction => new SqlBatchTransactionRepository(new DatabaseServiceSql(_connectionString));

	public ISerialNumberDetails SerialNumberDetails => new SqlSerialNumberDetailsRespository(new DatabaseServiceSql(_connectionString));

	public ISerialNumberTransactions SerialNumberTransactions => new SqlSerialNumberTransactionsRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryConfirm InventoryConfirm => new SqlInventoryConfirmRespository(new DatabaseServiceSql(_connectionString));

	public IPickAndPack PickAndPack => new SqlPickAndPackRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryCoutingBin InventoryCoutingBin => new SqlInventoryCoutingBinRespository(new DatabaseServiceSql(_connectionString));

	public IStockCounting StockCounting => new SqlStockCountingRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryCountingRequest InventoryCountingRequest => new SqlInventoryCountingRequestRespository(new DatabaseServiceSql(_connectionString));

	public IInventoryCounting InventoryCounting => new SqlInventoryCountingRespository(new DatabaseServiceSql(_connectionString));

	public ISTDPriceLists STDPriceLists => new SqlSTDPriceListsRepository(new DatabaseServiceSql(_connectionString));

	public IInventoryInWareHouse InventoryInWareHouse => new SqlInventoryInWareHouseRepository(new DatabaseServiceSql(_connectionString));

	public IPeriodAndVolumeDiscounts PeriodAndVolumeDiscounts => new SqlPeriodAndVolumeDiscountsRepository(new DatabaseServiceSql(_connectionString));

	public IStockRequest StockRequest => new SqlStockRequestRepository(new DatabaseServiceSql(_connectionString));

	public IDiscountGroup DiscountGroup => new SqlDiscountGroupRepository(new DatabaseServiceSql(_connectionString));

	public IInventoryPostingList InventoryPostingList => new SqlInventoryPostingListRepository(new DatabaseServiceSql(_connectionString));

	public IInventoryReport InventoryReport => new SqlInventoryReportRepository(new DatabaseServiceSql(_connectionString));

	public IBatchesControl BatchesControl => new SqlBatchesControlRepository(new DatabaseServiceSql(_connectionString));

	public ICycleCountRecommendation CycleCountRecommendation => new SqlCycleCountRecommendationRespository(new DatabaseServiceSql(_connectionString));

	public IPicking Picking => new SqlPickingRepository(new DatabaseServiceSql(_connectionString));

	public IPacking Packing => new SqlPackingRepository(new DatabaseServiceSql(_connectionString));

	public IShipping Shipping => new SqlShippingRepository(new DatabaseServiceSql(_connectionString));

	public IAlternativeItems AlternativeItems => new AlternativeItemsRepository(new DatabaseServiceSql(_connectionString));

	public IDocument Documents => new SqlDocumentRepository(new DatabaseServiceSql(_connectionString));

	public IDocumentDraft DocumentDraft => new SqlDocumentDraftRepository(new DatabaseServiceSql(_connectionString));

	public IAtp Atp => new SqlAtpRepository(new DatabaseServiceSql(_connectionString));

	public IDeliveryControl DeliveryControl => new SqlDeliveryControlRepository(new DatabaseServiceSql(_connectionString));

	public ISalesControl SalesControl => new SqlSalesControlRepository(new DatabaseServiceSql(_connectionString));

	public ISalesBlanketAgreement SalesBlanketAgreement => new SqlSalesBlanketAgreementRepository(new DatabaseServiceSql(_connectionString));

	public IDeliveryConfirm DeliveryConfirm => new SqlDeliveryConfirmRepository(new DatabaseServiceSql(_connectionString));

	public IHandoverDocument HandoverDocument => new SqlHandoverDocumentRepository(new DatabaseServiceSql(_connectionString));

	public IEmpToken EmpToken => new SqlEmpTokenRepository(new DatabaseServiceSql(_connectionString));

	public IAuthorToken AuthorToken => new SqlAuthorTokenRepository(new DatabaseServiceSql(_connectionString));

	public IPurchaseRequest PurchaseRequest => new SqlPurchaseRequestRespository(new DatabaseServiceSql(_connectionString));

	public IILandedCost PLandedCost => new SqlPLandedCostRepository(new DatabaseServiceSql(_connectionString));

	public IPurchaseRequestControl PurchaseRequestControl => new SqlPurchaseRequestControlRespository(new DatabaseServiceSql(_connectionString));

	public ICommons Commons => new SqlCommonsRepository(new DatabaseServiceSql(_connectionString));

	public IUDFs UDFs => new SqlUDFRepository(new DatabaseServiceSql(_connectionString));

	public IUDTs UDTs => new SqlUDTRepository(new DatabaseServiceSql(_connectionString));

	public IExRateAndIndex ExRateAndIndex => new SqlExRateAndIndexRepository(new DatabaseServiceSql(_connectionString));

	public IChangeLog ChangeLog => new SqlChangeLogRepository(new DatabaseServiceSql(_connectionString));

	public IFormSetting FormSetting => new SqlFormSettingRepository(new DatabaseServiceSql(_connectionString));

	public IUDOs UDOs => new SqlUDORepository(new DatabaseServiceSql(_connectionString));

	public IUDRs UDRs => new SqlUDRRepository(new DatabaseServiceSql(_connectionString));

	public ICFL CFL => new SqlCFLRepository(new DatabaseServiceSql(_connectionString));

	public IQueries Query => new SqlQueriesRepository(new DatabaseServiceSql(_connectionString));

	public IFormula Formula => new SqlFormulaRepository(new DatabaseServiceSql(_connectionString));

	public ICreateFormula CreateFormula => new SqlCreateFormulaRepository(new DatabaseServiceSql(_connectionString));

	public IDiagram Diagram => new SqlDiagramRepository(new DatabaseServiceSql(_connectionString));

	public IORW ORW => new SqlORWRepository(new DatabaseServiceSql(_connectionString));

	public IDefineNew DefineNew => new SqlDefineNewRepository(new DatabaseServiceSql(_connectionString));

	public IUserDefinedTexts UserDefinedTexts => new SqlUserDefinedTextsRepository(new DatabaseServiceSql(_connectionString));

	public IContextAction ContextAction => new SqlContextActionRepository(new DatabaseServiceSql(_connectionString));

	public ISearchSetting SearchSetting => new SqlSearchSettingRepository(new DatabaseServiceSql(_connectionString));

	public IFormFilterSetting FormFilterSetting => new SqlFormFilterSettingRepository(new DatabaseServiceSql(_connectionString));

	public IExternalRequest ExternalRequest => new ExternalRequestRepository(new DatabaseServiceSql(_connectionString));

	public IApprovalDecision ApprovalDecision => new ApprovalDecisionRepository(new DatabaseServiceSql(_connectionString));

	public IDashBoard DashBoard => new SqlDashBoardRepository(new DatabaseServiceSql(_connectionString));

	public IDashboardCategories DashboardCategories => new SqlDashboardCategoriesRepository(new DatabaseServiceSql(_connectionString));

	public IDashBoardTabs DashBoardTabs => new SqlDashBoardTabsRepository(new DatabaseServiceSql(_connectionString));

	public IDashboardAuthorization DashboardAuthorization => new SqlDashBoardAuthRepository(new DatabaseServiceSql(_connectionString));

	public ICategoriesDAB CategoriesDAB => new SqlCategoriesDABRepository(new DatabaseServiceSql(_connectionString));

	public IDashboardDetail DashboardDetail => new SqlDashboardDetailRepository(new DatabaseServiceSql(_connectionString));

	public IMobileDashboard MobileDashboard => new SqlMobileDashboardRepository(new DatabaseServiceSql(_connectionString));

	public ISpecialBP SpecialBP => new SqlSpecialBpRespository(new DatabaseServiceSql(_connectionString));

	public ISpecialBP_LineSPP1 SpecialBP_LineSPP1 => new SqlSpecialBp_LineSPP1Respository(new DatabaseServiceSql(_connectionString));

	public ISpecialBP_LineSPP2 SpecialBP_LineSPP2 => new SqlSpecialBp_LineSPP2Respository(new DatabaseServiceSql(_connectionString));

	public IOCYC OCYC => new SqlOCYCRepository(new DatabaseServiceSql(_connectionString));

	public IIncomingPayment IncomingPayment => new SqlIncomingPaymentRepository(new DatabaseServiceSql(_connectionString));

	public IOutgoingPayment OutgoingPayment => new SqlOutgoingPaymentRepository(new DatabaseServiceSql(_connectionString));

	public ICashItemMasterData CashItemMasterData => new SqlCashItemMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IVoucherMasterData VoucherMasterData => new SqlVoucherMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public ICouponMasterData CouponMasterData => new SqlCouponItemMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IInPayment InPayment => new SqlInPaymentRepository(new DatabaseServiceSql(_connectionString));

	public IOutPayment OutPayment => new SqlOutPaymentRepository(new DatabaseServiceSql(_connectionString));

	public IIncomingPayments IncomingPayments => new SqlIncomingPaymentsRepository(new DatabaseServiceSql(_connectionString));

	public IOutgoingPayments OutgoingPayments => new SqlOutgoingPaymentsRepository(new DatabaseServiceSql(_connectionString));

	public IPaymentRecon PaymentRecon => new SqlPaymentReconRepository(new DatabaseServiceSql(_connectionString));

	public IPaymentRequest PaymentRequest => new SqlPaymentRequestRepository(new DatabaseServiceSql(_connectionString));

	public IMKPurchaseOrder MKPurchaseOrder => new SqlMKPurchaseOrderRepository(new DatabaseServiceSql(_connectionString));

	public IPaymentWizard PaymentWizard => new SqlPaymentWizardRepository(new DatabaseServiceSql(_connectionString));

	public IPayments Payments => new PaymentsRepository(new DatabaseServiceSql(_connectionString));

	public ITrackingIssue TrackingIssue => new SqlTrackingIssueRepository(new DatabaseServiceSql(_connectionString));

	public IProjectManager ProjectManager => new SqlProjectManagerRepository(new DatabaseServiceSql(_connectionString));

	public IProjectReportCompletion ReportCompletion => new SqlProjectReportCompletionRepository(new DatabaseServiceSql(_connectionString));

	public IProjectConfirm ProjectConfirm => new SqlProjectConfirmRepository(new DatabaseServiceSql(_connectionString));

	public IFormatRule FormatRule => new SqlFormatRuleRepository(new DatabaseServiceSql(_connectionString));

	public IWBSTemplate WBSTemplate => new SqlWBSTemplateRepository(new DatabaseServiceSql(_connectionString));

	public IGanttControl GanttControl => new GanttControlRepository(new DatabaseServiceSql(_connectionString));

	public IBillOfMasterials BillOfMasterials => new SqlBillOfMasterialsRepository(new DatabaseServiceSql(_connectionString));

	public IBoMJob BoMJob => new SqlBomJobRepository(new DatabaseServiceSql(_connectionString));

	public IProductionOder ProductionOrder => new SqlProductionOderRepository(new DatabaseServiceSql(_connectionString));

	public IIssueForProduction IssueForProduction => new SqlIssueForProductionRepository(new DatabaseServiceSql(_connectionString));

	public IReceiptProduction ReceiptProduction => new SqlReceiptProductionRepository(new DatabaseServiceSql(_connectionString));

	public IProductionForBatch ProductionForBatch => new SqlProductionForBatchRepository(new DatabaseServiceSql(_connectionString));

	public IProductionConfirm ProductionConfirm => new SqlProductionConfirmRepository(new DatabaseServiceSql(_connectionString));

	public IBomVersion BomVersion => new SqlBomVersionRepository(new DatabaseServiceSql(_connectionString));

	public IMaterialShortageReport MaterialShortageReport => new SqlMaterialShortageReportRepository(new DatabaseServiceSql(_connectionString));

	public IProductionOrderImportTool ProductionOrderImportTool => new SqlProductionOrderImportToolRepository(new DatabaseServiceSql(_connectionString));

	public IOutsourcing Outsourcing => new SqlOutsourcingRepository(new DatabaseServiceSql(_connectionString));

	public IMps Mps => new SqlMpsRepository(new DatabaseServiceSql(_connectionString));

	public IMrpPlan MrpPlan => new SqlMrpPlanRepository(new DatabaseServiceSql(_connectionString));

	public IApzBillOfMaterials ApzBillOfMaterials => new SqlApzBillOfMaterialsRepository(new DatabaseServiceSql(_connectionString));

	public IMasterOrderReplan MasterOrderReplan => new SqlMasterOrderReplanRepository(new DatabaseServiceSql(_connectionString));

	public IResources Resources => new SqlResourcesRepository(new DatabaseServiceSql(_connectionString));

	public IControlKey ControlKey => new SqlControlKeyRepository(new DatabaseServiceSql(_connectionString));

	public IOperation Operation => new SqlOperationRepository(new DatabaseServiceSql(_connectionString));

	public IRouting Routing => new SqlRoutingRepository(new DatabaseServiceSql(_connectionString));

	public IGroupControlKey GroupControlKey => new SqlGroupControlKeyRepository(new DatabaseServiceSql(_connectionString));

	public IListPlant ListPlant => new SqlListPlantRepository(new DatabaseServiceSql(_connectionString));

	public IProductionControl ProductionControl => new SqlProductionControlRepository(new DatabaseServiceSql(_connectionString));

	public IProductionPlanning ProductionPlanning => new SqlProductionPlanningRepository(new DatabaseServiceSql(_connectionString));

	public ITinhGiay TinhGiay => new SqlTinhGiayRepository(new DatabaseServiceSql(_connectionString));

	public ISpecialIssueForBatch SpecialIssueForBatch => new SqlSpecialIssueForBatchRepository(new DatabaseServiceSql(_connectionString));

	public IReportCompleteForProduction ReportCompleteForProduction => new SqlReportCompleteForProductionRepository(new DatabaseServiceSql(_connectionString));

	public IResourceWorkCenter ResourceWorkCenter => new SqlResourceWorkCenterRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunity Opportunity => new SqlOpportunityRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunityStage OpportunityStage => new SqlOpportunityStageRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunityPartner OpportunityPartner => new SqlOpportunityPartnerRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunityRelationship OpportunityRelationship => new SqlOpportunityRelationshipRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunityCompetitor OpportunityCompetitor => new SqlOpportunityCompetitorRepository(new DatabaseServiceSql(_connectionString));

	public IOpportunityReason OpportunityReason => new SqlOpportunityReasonRepository(new DatabaseServiceSql(_connectionString));

	public IActivitiesForOpportunity ActivitiesForOpportunity => new SqlActivitiesForOpportunityRepository(new DatabaseServiceSql(_connectionString));

	public IActivityType ActivityType => new SqlActivityTypeRepository(new DatabaseServiceSql(_connectionString));

	public IActivitySubjects ActivitySubjects => new SqlActivitySubjectsRepository(new DatabaseServiceSql(_connectionString));

	public IActivityMeetingLocation ActivityMeetingLocation => new SqlActivityMeetingLocationRepository(new DatabaseServiceSql(_connectionString));

	public IActivityStatus ActivityStatus => new SqlActivityStatusRepository(new DatabaseServiceSql(_connectionString));

	public IActivityCalendar ActivityCalendar => new SqlActivityCalendarRepository(new DatabaseServiceSql(_connectionString));

	public ICalendarSetting CalendarSetting => new SqlCalendarSettingRepository(new DatabaseServiceSql(_connectionString));

	public IMailSetting MailSetting => new SqlMailSettingRepository(new DatabaseServiceSql(_connectionString));

	public IInterest Interest => new SqlInterestRepository(new DatabaseServiceSql(_connectionString));

	public IInterestLevel InterestLevel => new SqlInterestLevelRepository(new DatabaseServiceSql(_connectionString));

	public IServiceCall ServiceCall => new SqlServiceCallRepository(new DatabaseServiceSql(_connectionString));

	public Isolution Solution => new SqlSolutionRepository(new DatabaseServiceSql(_connectionString));

	public IServiceContract ServiceContract => new SqlServiceContractRepository(new DatabaseServiceSql(_connectionString));

	public IServiceCallSeting ServiceCallSetting => new SqlServiceCallSettingRepository(new DatabaseServiceSql(_connectionString));

	public ISubActivities SubActivities => new SqlSubActivitiesRepository(new DatabaseServiceSql(_connectionString));

	public IServiceCalls ServiceCalls => new SqlServiceCallsRepository(new DatabaseServiceSql(_connectionString));

	public IWarrantyCard WarrantyCard => new SqlWarrantyCardRepository(new DatabaseServiceSql(_connectionString));

	public IServiceLevelAgreement ServiceLevelAgreement => new SqlServiceLevelAgreementRepository(new DatabaseServiceSql(_connectionString));

	public IServiceContractTemplate ServiceContractTemplate => new SqlServiceContractTemplateRepository(new DatabaseServiceSql(_connectionString));

	public IMarketingCampaign MarketingCampaign => new SqlMarketingCampaignRepository(new DatabaseServiceSql(_connectionString));

	public IMRP MRP => new SqlMRPRepository(new DatabaseServiceSql(_connectionString));

	public ISchedule Schedule => new SqlScheduleRepository(new DatabaseServiceSql(_connectionString));

	public ISalesForeCast SalesForeCast => new SqlSalesForeCastRepository(new DatabaseServiceSql(_connectionString));

	public IForecast Forecast => new SqlForecastRepository(new DatabaseServiceSql(_connectionString));

	public IShift Shift => new SqlShiftRepository(new DatabaseServiceSql(_connectionString));

	public IWorkScheduling WorkScheduling => new SqlWorkSchedulingRepository(new DatabaseServiceSql(_connectionString));

	public ILines Lines => new SqlLinesRepository(new DatabaseServiceSql(_connectionString));

	public IItemQuotaArrangement ItemQuotaArrangement => new SqlItemQuotaArrangementRepository(new DatabaseServiceSql(_connectionString));

	public IItemQuotaArrangementDate ItemQuotaArrangementDate => new SqlItemQuotaArrangementDateRepository(new DatabaseServiceSql(_connectionString));

	public IJetstarPreOrder JetstarPreOrder => new SqlJetstarPreOrderRepository(new DatabaseServiceSql(_connectionString));

	public IIrsConfig IrsConfig => new SqlIrsConfigRepository(new DatabaseServiceSql(_connectionString));

	public IAdvanceForecast AdvanceForecast => new SqlAdvanceForecastRepository(new DatabaseServiceSql(_connectionString));

	public IIntegrated Integrated => new SqlIntegratedRepository(new DatabaseServiceSql(_connectionString));

	public IIntegratedInventoryTransfer IntegratedInventoryTransfer => new SqlIntegratedInventoryTransferRepository(new DatabaseServiceSql(_connectionString));

	public IIntegratedBP IntegratedBP => new SqlIntegratedBPRepository(new DatabaseServiceSql(_connectionString));

	public IIntegratedWarehouse IntegratedWarehouse => new SqlIntegratedWarehouseRepository(new DatabaseServiceSql(_connectionString));

	public IIntegratedUdo IntegratedUdo => new SqlIntegratedUdoRepository(new DatabaseServiceSql(_connectionString));

	public IIntegratedItem IntegratedItem => new SqlIntegratedItemRepository(new DatabaseServiceSql(_connectionString));

	public IImportDataBP ImportDataBP => new ImportDataBPRepository(new DatabaseServiceSql(_connectionString));

	public IImportItemMasterData ImportItemMasterData => new ImportItemMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IImportBOM ImportBom => new ImportBOMRepository(new DatabaseServiceSql(_connectionString));

	public IImportData ImportData => new ImportDataRepository(new DatabaseServiceSql(_connectionString));

	public IAdvancePeriodClosing AdvancePeriodClosing => new SqlAdvancePeriodClosingRepository(new DatabaseServiceSql(_connectionString));

	public IRetail Retail => new SqlRetailRepository(new DatabaseServiceSql(_connectionString));

	public IRetailStore RetailStore => new SqlRetailStoreRepository(new DatabaseServiceSql(_connectionString));

	public IPosMaster PosMaster => new SqlPosMasterRepository(new DatabaseServiceSql(_connectionString));

	public IRetailItemSearch ItemSearch => new SqlRetailItemSearchRepository(new DatabaseServiceSql(_connectionString));

	public IRetailCustomerSearch CustomerSearch => new SqlRetailCustomerSearchRepository(new DatabaseServiceSql(_connectionString));

	public IRetailChooseFromList RetailChooseFromList => new SqlRetailChooseFromListRepository(new DatabaseServiceSql(_connectionString));

	public IRetailCmdConfig RetailCmdConfig => new SqlRetailCmdConfigRepository(new DatabaseServiceSql(_connectionString));

	public IDeductionType DeductionType => new SqlDeductionTypeRepository(new DatabaseServiceSql(_connectionString));

	public ISalaryParameters SalaryParameters => new SqlSalaryParametersRepository(new DatabaseServiceSql(_connectionString));

	public IWidget Widget => new SqlWidgetRepository(new DatabaseServiceSql(_connectionString));

	public ICapacity Capacity => new CapacityRepository(new DatabaseServiceSql(_connectionString));

	public IOperations Operations => new OperationsRepository(new DatabaseServiceSql(_connectionString));

	public IAdvanceRouting AdvanceRouting => new AdvanceRoutingRepository(new DatabaseServiceSql(_connectionString));

	public IResourceWorkSchedule ResourceWorkSchedule => new ResourceWorkScheduleRepository(new DatabaseServiceSql(_connectionString));

	public ISMSConfig SmsConfig => new SMSConfigRepository(new DatabaseServiceSql(_connectionString));

	public IAdditionalGeneralSettings AdditionalGeneralSettings => new AdditionalGeneralSettingsRepository(new DatabaseServiceSql(_connectionString));

	public IFile Files => new FileRepository(new DatabaseServiceSql(_connectionString));

	public IResourceGroup ResourceGroup => new ResourceGroupRepository(new DatabaseServiceSql(_connectionString));

	public IPublishService PublishService => new PublishServiceRepository(new DatabaseServiceSql(_connectionString));

	public ICapacitySetting CapacitySetting => new CapacitySettingRepository(new DatabaseServiceSql(_connectionString));

	public IBarcodeInventoryCounting BarcodeInventoryCounting => new BarcodeInventoryCountingRepository(new DatabaseServiceSql(_connectionString));

	public IGeography Geography => new GeographyRepository(new DatabaseServiceSql(_connectionString));

	public IStore Store => new StoreRepository(new DatabaseServiceSql(_connectionString));

	public ISaleTerritory SaleTerritory => new SaleTerritoryRepository(new DatabaseServiceSql(_connectionString));

	public IVisitStore VisitStore => new VisitStoreRepository(new DatabaseServiceSql(_connectionString));

	public ISurvey Survey => new SurveyRepository(new DatabaseServiceSql(_connectionString));

	public IQuestionMasterData QuestionMasterData => new QuestionMasterDataRepository(new DatabaseServiceSql(_connectionString));

	public IUploadImage UploadImage => new UploadImageRepository(new DatabaseServiceSql(_connectionString));

	public ISettingReport SettingReport => new SettingReportRepository(new DatabaseServiceSql(_connectionString));

	public IOutletVisitingFrequency OutletVisitingFrequency => new OutletVisitingFrequencyRepository(new DatabaseServiceSql(_connectionString));

	public IPromotionSetup PromotionSetup => new PromotionSetupRepository(new DatabaseServiceSql(_connectionString));

	public IChannel Channel => new ChannelRepository(new DatabaseServiceSql(_connectionString));

	public ISalesRoute SalesRoute => new SalesRouteRepository(new DatabaseServiceSql(_connectionString));

	public IPromotionCondition PromotionCondition => new PromotionConditionRepository(new DatabaseServiceSql(_connectionString));

	public ITrackingUserLocation TrackingUserLocation => new TrackingUserLocationRepository(new DatabaseServiceSql(_connectionString));

	public IImageAlbum ImageAlbum => new ImageAlbumRepository(new DatabaseServiceSql(_connectionString));

	public IMarker Marker => new MarkerRepository(new DatabaseServiceSql(_connectionString));

	public IDMSDelivery DMSDelivery => new DMSDeliveryRepository(new DatabaseServiceSql(_connectionString));

	public IAlertTemplate AlertTemplate => new AlertTemplateRepository(new DatabaseServiceSql(_connectionString));

	public IDMSApproved DMSApproved => new DMSApprovedRepository(new DatabaseServiceSql(_connectionString));

	public IDMSSettingUDF DMSSettingUDF => new DMSSettingUDFRepository(new DatabaseServiceSql(_connectionString));

	public ICommentCustomer CommentCustomer => new CommentCustomerRepository(new DatabaseServiceSql(_connectionString));

	public IContactUs ContactUs => new ContactUsRepository(new DatabaseServiceSql(_connectionString));

	public IPackingOrder PackingOrder => new SqlPackingOrderRepository(new DatabaseServiceSql(_connectionString));

	public ICreateTypeCase CreateTypeCase => new SqlCreateTypeCaseRepository(new DatabaseServiceSql(_connectionString));

	public ITraceability Traceability => new SqlTraceabilityRepository(new DatabaseServiceSql(_connectionString));

	public IPrintBillWrap PrintBillWrap => new SqlPrintBillWrapRepository(new DatabaseServiceSql(_connectionString));

	public IApprovalStages ApprovalStages => new ApprovalStagesRepository(new DatabaseServiceSql(_connectionString));

	public IApprovalTemplates ApprovalTemplates => new ApprovalTemplatesRepository(new DatabaseServiceSql(_connectionString));

	public IWorkingProcess WorkingProcess => new WorkingProcessRepository(new DatabaseServiceSql(_connectionString));

	public INotification Notification => new NotificationRepository(new DatabaseServiceSql(_connectionString));

	public IOutlet Outlet => new OutletRepository(new DatabaseServiceSql(_connectionString), this);

	public IPromotion Promotion => new PromotionRepository(new DatabaseServiceSql(_connectionString));

	public IDMSReport DMSReport => new DMSReportRepository(new DatabaseServiceSql(_connectionString));

	public IPromotionRegister PromotionRegister => new PromotionRegisterRepository(new DatabaseServiceSql(_connectionString));

	public IPromotionBenefit PromotionBenefit => new PromotionBenefitRepository(new DatabaseServiceSql(_connectionString));

	public ICheckPromotion CheckPromotion => new CheckPromotionRepository(new DatabaseServiceSql(_connectionString));

	public IPromotionProgram PromotionProgram => new PromotionProgramRepository(new DatabaseServiceSql(_connectionString));

	public IKPIsMaterData KPIsMaterData => new KPIsMaterDataRepository(new DatabaseServiceSql(_connectionString));

	public ISetIndicatorKPIs SetIndicatorKPIs => new SetIndicatorKPIsRepository(new DatabaseServiceSql(_connectionString));

	public IKPIsSetting KPIsSetting => new KPIsSettingRepository(new DatabaseServiceSql(_connectionString));

	public IListPointKPIs ListPointKPIs => new ListPointKPIsRepository(new DatabaseServiceSql(_connectionString));

	public IKPIPlan KPIPlan => new KPIPlanRepository(new DatabaseServiceSql(_connectionString));

	public IKPIScope KPIScope => new KPIScopeRepository(new DatabaseServiceSql(_connectionString));

	public IKPIIndicator KPIIndicator => new KPIIndicatorRepository(new DatabaseServiceSql(_connectionString));

	public IKPIGroup KPIGroup => new KPIGroupRepository(new DatabaseServiceSql(_connectionString));

	public IKPISetup KPISetup => new KPISetupRepository(new DatabaseServiceSql(_connectionString));

	public IKPIScore KPIScore => new KPIScoreRepository(new DatabaseServiceSql(_connectionString));

	public IThirdPartyToken ThirdPartyToken => new ThirdPartyTokenRepository(new DatabaseServiceSql(_connectionString));

	public ISettingApprovalDetail SettingApprovalDetail => new SettingApprovalDetailRepository(new DatabaseServiceSql(_connectionString));

	public IAboutRangeTime AboutRangeTime => new AboutRangeTimeRepository(new DatabaseServiceSql(_connectionString));

	public IComments Comments => new CommentsRepository(new DatabaseServiceSql(_connectionString));

	public UnitOfWork(string connectionString)
	{
		_connectionString = connectionString;
	}

	public ITransaction BeginTransaction()
	{
		return new Transaction();
	}

	public void EndTransaction(ITransaction transaction)
	{
		transaction?.Dispose();
	}

	public string ConvertErrorMessage(string error)
	{
		return new DatabaseServiceSql(_connectionString).ConvertSqlErrorMessage(error);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!_disposed)
		{
		}
		_disposed = true;
	}
}
