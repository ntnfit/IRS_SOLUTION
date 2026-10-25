using System;
using Apzon.HDBConnection;
using Apzon.HDBConnection.Interfaces;
using Apzon.Services.HanaRepository.Administrator;
using Apzon.Services.HanaRepository.Administrator.ApprovalProcess;
using Apzon.Services.HanaRepository.Administrator.Banking;
using Apzon.Services.HanaRepository.Administrator.BusinessPartner;
using Apzon.Services.HanaRepository.Administrator.Business_Partner;
using Apzon.Services.HanaRepository.Administrator.Plugin;
using Apzon.Services.HanaRepository.Administrator.Setup;
using Apzon.Services.HanaRepository.Administrator.Setup.Inventory;
using Apzon.Services.HanaRepository.Administrator.Setup.Purchasing;
using Apzon.Services.HanaRepository.Administrator.Setup.Service;
using Apzon.Services.HanaRepository.Administrator.SystemInitialization;
using Apzon.Services.HanaRepository.BusinessPartner;
using Apzon.Services.HanaRepository.Commons;
using Apzon.Services.HanaRepository.DMS;
using Apzon.Services.HanaRepository.DashBoard;
using Apzon.Services.HanaRepository.Document;
using Apzon.Services.HanaRepository.FacebookCRM;
using Apzon.Services.HanaRepository.Financials;
using Apzon.Services.HanaRepository.HumanResources;
using Apzon.Services.HanaRepository.HumanResources.Salary;
using Apzon.Services.HanaRepository.HumanResources.TimeSheeting;
using Apzon.Services.HanaRepository.Integrated;
using Apzon.Services.HanaRepository.Inventory;
using Apzon.Services.HanaRepository.Inventory.Batches;
using Apzon.Services.HanaRepository.Inventory.InventoryReport;
using Apzon.Services.HanaRepository.Inventory.InventoryTransaction;
using Apzon.Services.HanaRepository.Inventory.ItemManagement;
using Apzon.Services.HanaRepository.Inventory.PickAndPack;
using Apzon.Services.HanaRepository.Inventory.PriceLists;
using Apzon.Services.HanaRepository.KPIs;
using Apzon.Services.HanaRepository.Mrps;
using Apzon.Services.HanaRepository.Opportunity;
using Apzon.Services.HanaRepository.PM;
using Apzon.Services.HanaRepository.PackingManagement;
using Apzon.Services.HanaRepository.Payment;
using Apzon.Services.HanaRepository.PlanningProduction;
using Apzon.Services.HanaRepository.Production;
using Apzon.Services.HanaRepository.ProdutionAddon;
using Apzon.Services.HanaRepository.Promotion;
using Apzon.Services.HanaRepository.Purchase;
using Apzon.Services.HanaRepository.Retail;
using Apzon.Services.HanaRepository.RoutingAndResource;
using Apzon.Services.HanaRepository.Sales;
using Apzon.Services.HanaRepository.Service;
using Apzon.Services.HanaRepository.Service.MarketingCampaign;
using Apzon.Services.HanaRepository.Util;
using Apzon.Services.HanaRepository.Widget;
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
using Apzon.Services.Repository.Administrator.Setup;
using Apzon.Services.Repository.BusinessPartner;
using Apzon.Services.Repository.Commons;
using Apzon.Services.Repository.DMS;
using Apzon.Services.Repository.ImportData;
using Apzon.Services.Repository.Integrated;
using Apzon.Services.Repository.Inventory.InventoryTransaction;
using Apzon.Services.Repository.PM;
using Apzon.Services.Repository.RoutingAndResource;

namespace Apzon.Services;

public class HanaUnitOfWork : IUnitOfWork, IDisposable
{
	private bool _disposed;

	private string _connectionString;

	public IPackingBatch PackingBatch => new HanaPackingBatchRepository(new DatabaseServiceHana(_connectionString));

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

	public ISalesPersons SalesPersons => new HanaSalesPersonsRepository(new DatabaseServiceHana(_connectionString));

	public IShippingTypes ShippingTypes => new HanaShippingTypesRepository(new DatabaseServiceHana(_connectionString));

	public IManufacturers Manufacturers => new HanaManufacturersRepository(new DatabaseServiceHana(_connectionString));

	public INotificationSetting NotificationSetting => new HanaNotificationSettingRepository(new DatabaseServiceHana(_connectionString));

	public IItemGroups ItemGroups => new HanaItemGroupsRepository(new DatabaseServiceHana(_connectionString));

	public IItemProperties ItemProperties => new HanaItemPropertiesRepository(new DatabaseServiceHana(_connectionString));

	public IVatGroups VatGroups => new HanaVatGroupsRepository(new DatabaseServiceHana(_connectionString));

	public IPriceLists PriceLists => new HanaPriceListsRepository(new DatabaseServiceHana(_connectionString));

	public IWarehouse Warehouse => new HanaWarehouseRepository(new DatabaseServiceHana(_connectionString));

	public IRequester Requester => new HanaRequesterRepository(new DatabaseServiceHana(_connectionString));

	public IAcctCode AcctCode => new HanaAcctCodeRepository(new DatabaseServiceHana(_connectionString));

	public ILocation Location => new HanaLocationRepository(new DatabaseServiceHana(_connectionString));

	public ICustomsGroup CustomsGroup => new HanaCustomsGroupRepository(new DatabaseServiceHana(_connectionString));

	public IDepartment Department => new HanaDepartmentRepository(new DatabaseServiceHana(_connectionString));

	public IUsers Users => new HanaUsersRepository(new DatabaseServiceHana(_connectionString));

	public IGeneralSetting GeneralSetting => new HanaGeneralSettingRepository(new DatabaseServiceHana(_connectionString));

	public IHoliday Holiday => new HanaHolidayRepository(new DatabaseServiceHana(_connectionString));

	public IAuths Auths => new HanaAuthRepository(new DatabaseServiceHana(_connectionString));

	public IProject Project => new HanaProjectRepository(new DatabaseServiceHana(_connectionString));

	public IPrintLayout PrintLayout => new HanaPrintLayoutRepository(new DatabaseServiceHana(_connectionString));

	public IFilterSetting FilterSetting => new HanaFilterSettingRepository(new DatabaseServiceHana(_connectionString));

	public IWorkingTable WorkingTable => new HanaWorkingTableRepository(new DatabaseServiceHana(_connectionString));

	public IFreight Freight => new HanaFreightRepository(new DatabaseServiceHana(_connectionString));

	public ILengthAndWidth LengthAndWidth => new HanaLengthAndWidthRepository(new DatabaseServiceHana(_connectionString));

	public IGroupCustomerVender GroupCustomerVender => new HanaGroupCustomerVenderRepository(new DatabaseServiceHana(_connectionString));

	public IBusinessPartnerProperties BusinessPartnerProperties => new HanaBusinessPartnerPropertiesRepository(new DatabaseServiceHana(_connectionString));

	public IBusinessPrioritiesSetup BusinessPrioritiesSetup => new HanaBusinessPartnerPrioritiesRepository(new DatabaseServiceHana(_connectionString));

	public ICreditCard CreditCard => new HanaCreditCardRepository(new DatabaseServiceHana(_connectionString));

	public ICreditCardPaymentMethods CreditCardPaymentMethods => new HanaCreditCardPaymentMehodsRepository(new DatabaseServiceHana(_connectionString));

	public IAdministrator Administrator => new HanaAdministratorRepository(new DatabaseServiceHana(_connectionString));

	public IState State => new HanaStateRepository(new DatabaseServiceHana(_connectionString));

	public IWeight Weight => new HanaWeightRepository(new DatabaseServiceHana(_connectionString));

	public ILoginIRS LoginIRS => new HanaLoginIRSRepository(new DatabaseServiceHana(_connectionString));

	public IUsersIRS UsersIRS => new HanaUsersIRSRepository(new DatabaseServiceHana(_connectionString));

	public IBinFieldActivation BinFieldActivation => new HanaBinFieldActivationRepository(new DatabaseServiceHana(_connectionString));

	public IWHSubSetup WHSubSetup => new HanaWHSubSetupRepository(new DatabaseServiceHana(_connectionString));

	public IAttrCodeSetup AttrCodeSetup => new HanaAttrCodeSetupRepository(new DatabaseServiceHana(_connectionString));

	public IBinLocList BinLocList => new HanaBinLocListRepository(new DatabaseServiceHana(_connectionString));

	public IBinLocMaster BinLocMaster => new HanaBinLocMasterRepository(new DatabaseServiceHana(_connectionString));

	public IWHSubManager WHSubManager => new HanaWHSubManagerRepository(new DatabaseServiceHana(_connectionString));

	public IWHSubSetGenPre WHSubSetGenPre => new HanaWHSubSetGenPreRepository(new DatabaseServiceHana(_connectionString));

	public IBarConfig BarConfig => new HanaBarConfigRepository(new DatabaseServiceHana(_connectionString));

	public IBranches Branches => new HanaBranchesRepository(new DatabaseServiceHana(_connectionString));

	public IXtraReportManager XtraReportManager => new HanaXtraReportManagerRepository(new DatabaseServiceHana(_connectionString));

	public IInventoryCycles InventoryCycles => new HanaInventoryCyclesRepository(new DatabaseServiceHana(_connectionString));

	public ICycleCountDetermination CycleCountDetermination => new HanaCycleCountDeterminationRepository(new DatabaseServiceHana(_connectionString));

	public ICycleCountDeterminationSubTable CycleCountDeterminationSubTable => new HanaCycleCountDeterminationSubTableRepository(new DatabaseServiceHana(_connectionString));

	public IVehicle Vehicle => new HanaVehicleRepository(new DatabaseServiceHana(_connectionString));

	public IVehicleType VehicleType => new HanaVehicleTypeRepository(new DatabaseServiceHana(_connectionString));

	public ILandedCostsSetup LandedCostsSetup => new HanaLandedCostsSetupRepository(new DatabaseServiceHana(_connectionString));

	public ILandedCosts LandedCosts => new HanaLandedCostsRepository(new DatabaseServiceHana(_connectionString));

	public ITerritory Territory => new HanaTerritoryRepository(new DatabaseServiceHana(_connectionString));

	public IPluginManager PluginManager => new HanaPluginManagerRepository(new DatabaseServiceHana(_connectionString));

	public ILicenseManager LicenseManager => new HanaLicenseManagerRepository(new DatabaseServiceHana(_connectionString));

	public IAlertOverview AlertOverview => new HanaAlertOverviewRepository(new DatabaseServiceHana(_connectionString), this);

	public ICompanyDetails CompanyDetails => new HanaCompanyDetailsRepository(new DatabaseServiceHana(_connectionString));

	public IDocumentNumbering DocumentNumbering => new HanaDocumentNumberingRepository(new DatabaseServiceHana(_connectionString));

	public ISeries Series => new HanaSeriesRepository(new DatabaseServiceHana(_connectionString));

	public IDocumentSettings DocumentSettings => new HanaDocumentSettingsRepository(new DatabaseServiceHana(_connectionString));

	public IMenuStructure MenuStructure => new HanaMenuStructureRepository(new DatabaseServiceHana(_connectionString));

	public IUnitOfMeasureGroups UnitOfMeasureGroups => new HanaUnitOfMeasureGroupsRepository(new DatabaseServiceHana(_connectionString));

	public IUoMGroupDefinition UoMGroupDefinition => new HanaUoMGroupDefinitionRepository(new DatabaseServiceHana(_connectionString));

	public IUnitOfMeasure UnitOfMeasure => new HanaUnitOfMeasureRepository(new DatabaseServiceHana(_connectionString));

	public IFacebookCRM FacebookCRM => new HanaFacebookCRMRepository(new DatabaseServiceHana(_connectionString));

	public IFacebookManager FacebookManager => new HanaFacebookManagerRepository(new DatabaseServiceHana(_connectionString));

	public ICurrencies Currencies => new HanaCurrenciesRepository(new DatabaseServiceHana(_connectionString));

	public ISalesEmployeeBuyer SalesEmployeeBuyer => new HanaSalesEmployeeBuyerRepository(new DatabaseServiceHana(_connectionString));

	public IDimension Dimension => new HanaDimensionRepository(new DatabaseServiceHana(_connectionString));

	public IDistributeRule DistributeRule => new HanaDistributeRuleRepository(new DatabaseServiceHana(_connectionString));

	public ICountries Countries => new HanaCountriesRepository(new DatabaseServiceHana(_connectionString));

	public IUserDefault UserDefault => new HanaUserDefaultRepository(new DatabaseServiceHana(_connectionString));

	public IFormWizard FormWizard => new HanaFormWizardRepository(new DatabaseServiceHana(_connectionString));

	public IExternalRouting ExternalRouting => new HanaExternalRoutingRepository(new DatabaseServiceHana(_connectionString));

	public IEmpMasterData EmpMasterData => new HanaEmpMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IHREducation HREducation => new HanaHREducationRepository(new DatabaseServiceHana(_connectionString));

	public IEmpContract EmpContract => new HanaEmpContractRepository(new DatabaseServiceHana(_connectionString));

	public IEmpRelatives EmpRelatives => new HanaEmpRelativesRepository(new DatabaseServiceHana(_connectionString));

	public IJobVacancy JobVacancy => new HanaJobVacancyRepository(new DatabaseServiceHana(_connectionString));

	public IJVApplicants JVApplicants => new HanaJVApplicantsRepository(new DatabaseServiceHana(_connectionString));

	public IHRMQualifications HRMQualifications => new HanaHRMQualificationsRepository(new DatabaseServiceHana(_connectionString));

	public IJVInterview JVInterview => new HanaJVInterviewRepository(new DatabaseServiceHana(_connectionString));

	public IJVQuestion JVQuestion => new HanaJVQuestionRepository(new DatabaseServiceHana(_connectionString));

	public IJVAnswer JVAnswer => new HanaJVAnswerRepository(new DatabaseServiceHana(_connectionString));

	public ITimeSheeting TimeSheeting => new HanaTimeSheetingRepository(new DatabaseServiceHana(_connectionString));

	public IWorkPeriod WorkPeriod => new HanaWorkPeriodRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryInfo SalaryInfo => new HanaSalaryInfoRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryInfoMainByEmp SalaryInfoMainByEmp => new HanaSalaryInfoMainByEmpRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryAllowanceByEmp SalaryAllowanceByEmp => new HanaSalaryAllowanceByEmpRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryInfoAllowance SalaryInfoAllowance => new HanaSalaryInfoAllowanceRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryInfoAdvance SalaryInfoAdvance => new HanaSalaryInfoAdvanceRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryCalculation SalaryCalculation => new HanaSalaryCalculationRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryFormula SalaryFormula => new HanaSalaryFormulaRepository(new DatabaseServiceHana(_connectionString));

	public IAllowanceType AllowanceType => new HanaAllowanceTypeRepository(new DatabaseServiceHana(_connectionString));

	public IPerIncomeTax PerIncomeTax => new HanaPerIncomeTaxRepository(new DatabaseServiceHana(_connectionString));

	public ITimeSheetingHour TimeSheetingHour => new HanaTimeSheetingHourRepository(new DatabaseServiceHana(_connectionString));

	public IPenalType PenalType => new HanaPenalTypeRepository(new DatabaseServiceHana(_connectionString));

	public IEmpPenalization EmpPenalization => new HanaEmpPenalizationRepository(new DatabaseServiceHana(_connectionString));

	public ILeaveParameter LeaveParameter => new HanaLeaveParameterRepository(new DatabaseServiceHana(_connectionString));

	public IStates States => new HanaStatesRepository(new DatabaseServiceHana(_connectionString));

	public ILeaveInformation LeaveInformation => new HanaLeaveInformationRepository(new DatabaseServiceHana(_connectionString));

	public IHolidaySetup HolidaySetup => new HanaHolidaySetupRepository(new DatabaseServiceHana(_connectionString));

	public IInOutMethod InOutMethod => new HanaInOutMethodRepository(new DatabaseServiceHana(_connectionString));

	public IWorkShift WorkShift => new HanaWorkShiftRepository(new DatabaseServiceHana(_connectionString));

	public IWorkScheduleSetup WorkScheduleSetup => new HanaWorkScheduleSetupRepository(new DatabaseServiceHana(_connectionString));

	public IWorkScheduleRegister WorkScheduleRegister => new HanaWorkScheduleRegisterRepository(new DatabaseServiceHana(_connectionString));

	public IDocumentConfirm DocumentConfirm => new HanaDocumentConfirmRepository(new DatabaseServiceHana(_connectionString));

	public IChartOfAccount ChartOfAccount => new HanaChartOfAccountRepository(new DatabaseServiceHana(_connectionString));

	public IMaterialRevaluation MaterialRevaluation => new HanaMaterialRevaluationRepository(new DatabaseServiceHana(_connectionString));

	public IJournalEntries JournalEntries => new HanaJournalEntriesRepository(new DatabaseServiceHana(_connectionString));

	public ICostCenters CostCenters => new HanaCostCentersRepository(new DatabaseServiceHana(_connectionString));

	public IBusinessPartnerss BP => new HanaBusinessPartnerRepository(new DatabaseServiceHana(_connectionString));

	public ICardGroups CardGroup => new HanaCardGroupRepository(new DatabaseServiceHana(_connectionString));

	public ISaleEmployees SaleEmployee => new HanaEmployeeRepository(new DatabaseServiceHana(_connectionString));

	public ICashDiscounts CashDiscount => new HanaCashDiscountRepository(new DatabaseServiceHana(_connectionString));

	public IOCashDiscounts OCashDiscount => new HanaOCashDiscountRepository(new DatabaseServiceHana(_connectionString));

	public IPaymentTerms PaymentTerm => new HanaPaymentTermRepository(new DatabaseServiceHana(_connectionString));

	public ICreditCardTypes CreditCardType => new HanaCreditCardTypeRepository(new DatabaseServiceHana(_connectionString));

	public IBusinessMasterDatas BusinessMasterData => new HanaBusinessMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IContactPersons ContactPerson => new HanaContactPersonRepository(new DatabaseServiceHana(_connectionString));

	public IAddresses Addresses => new HanaBpAddressRepository(new DatabaseServiceHana(_connectionString));

	public IIndustry Industry => new HanaIndustryRepository(new DatabaseServiceHana(_connectionString));

	public IContractTemplate ContractTemplate => new HanaContractTemplateRepository(new DatabaseServiceHana(_connectionString));

	public IItemMasterData Items => new HanaItemMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IGoodReceiptIt GoodsReceipt => new HanaGoodReceiptItRespository(new DatabaseServiceHana(_connectionString));

	public IGoodsIssueIt GoodsIssueIt => new HanaGoodsIssueItRespository(new DatabaseServiceHana(_connectionString));

	public ITranferRequest TranferRequest => new HanaTranferRequestRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryTranfer InventoryTranfer => new HanaInventoryTranferRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryRevaluation InventoryRevaluation => new HanaInventoryRevaluationRespository(new DatabaseServiceHana(_connectionString));

	public IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers => new HanaBusinessPartnerCatalogNumbersRespository(new DatabaseServiceHana(_connectionString));

	public IBatchDetail BatchDetail => new HanaBatchDetailsRepository(new DatabaseServiceHana(_connectionString));

	public IBatchTransaction BatchTransaction => new HanaBatchTransactionRepository(new DatabaseServiceHana(_connectionString));

	public ISerialNumberDetails SerialNumberDetails => new HanaSerialNumberDetailsRespository(new DatabaseServiceHana(_connectionString));

	public ISerialNumberTransactions SerialNumberTransactions => new HanaSerialNumberTransactionsRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryConfirm InventoryConfirm => new HanaInventoryConfirmRespository(new DatabaseServiceHana(_connectionString));

	public IPickAndPack PickAndPack => new HanaPickAndPackRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryCoutingBin InventoryCoutingBin => new HanaInventoryCoutingBinRespository(new DatabaseServiceHana(_connectionString));

	public IStockCounting StockCounting => new HanaStockCountingRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryCountingRequest InventoryCountingRequest => new HanaInventoryCountingRequestRespository(new DatabaseServiceHana(_connectionString));

	public IInventoryCounting InventoryCounting => new HanaInventoryCountingRespository(new DatabaseServiceHana(_connectionString));

	public ISTDPriceLists STDPriceLists => new HanaSTDPriceListsRepository(new DatabaseServiceHana(_connectionString));

	public IInventoryInWareHouse InventoryInWareHouse => new HanaInventoryInWareHouseRepository(new DatabaseServiceHana(_connectionString));

	public IPeriodAndVolumeDiscounts PeriodAndVolumeDiscounts => new HanaPeriodAndVolumeDiscountsRepository(new DatabaseServiceHana(_connectionString));

	public IStockRequest StockRequest => new HanaStockRequestRepository(new DatabaseServiceHana(_connectionString));

	public IDiscountGroup DiscountGroup => new HanaDiscountGroupRepository(new DatabaseServiceHana(_connectionString));

	public IInventoryPostingList InventoryPostingList => new HanaInventoryPostingListRepository(new DatabaseServiceHana(_connectionString));

	public IInventoryReport InventoryReport => new HanaInventoryReportRepository(new DatabaseServiceHana(_connectionString));

	public IBatchesControl BatchesControl => new HanaBatchesControlRepository(new DatabaseServiceHana(_connectionString));

	public ICycleCountRecommendation CycleCountRecommendation => new HanaCycleCountRecommendationRespository(new DatabaseServiceHana(_connectionString));

	public IPicking Picking => new HanaPickingRepository(new DatabaseServiceHana(_connectionString));

	public IPacking Packing => new HanaPackingRepository(new DatabaseServiceHana(_connectionString));

	public IShipping Shipping => new HanaShippingRepository(new DatabaseServiceHana(_connectionString));

	public IAlternativeItems AlternativeItems => new HanaAlternativeItemsRepository(new DatabaseServiceHana(_connectionString));

	public IDocument Documents => new HanaDocumentRepository(new DatabaseServiceHana(_connectionString));

	public IDocumentDraft DocumentDraft => new HanaDocumentDraftRepository(new DatabaseServiceHana(_connectionString));

	public IAtp Atp => new HanaAtpRepository(new DatabaseServiceHana(_connectionString));

	public IDeliveryControl DeliveryControl => new HanaDeliveryControlRepository(new DatabaseServiceHana(_connectionString));

	public ISalesControl SalesControl => new HanaSalesControlRepository(new DatabaseServiceHana(_connectionString));

	public ISalesBlanketAgreement SalesBlanketAgreement => new HanaSalesBlanketAgreementRepository(new DatabaseServiceHana(_connectionString));

	public IDeliveryConfirm DeliveryConfirm => new HanaDeliveryConfirmRepository(new DatabaseServiceHana(_connectionString));

	public IHandoverDocument HandoverDocument => new HanaHandoverDocumentRepository(new DatabaseServiceHana(_connectionString));

	public IEmpToken EmpToken => new HanaEmpTokenRepository(new DatabaseServiceHana(_connectionString));

	public IAuthorToken AuthorToken => new HanaAuthorTokenRepository(new DatabaseServiceHana(_connectionString));

	public IPurchaseRequest PurchaseRequest => new HanaPurchaseRequestRespository(new DatabaseServiceHana(_connectionString));

	public IILandedCost PLandedCost => new HanaPLandedCostRepository(new DatabaseServiceHana(_connectionString));

	public IPurchaseRequestControl PurchaseRequestControl => new HanaPurchaseRequestControlRespository(new DatabaseServiceHana(_connectionString));

	public ICommons Commons => new HanaCommonsRepository(new DatabaseServiceHana(_connectionString));

	public IUDFs UDFs => new HanaUDFRepository(new DatabaseServiceHana(_connectionString));

	public IUDTs UDTs => new HanaUDTRepository(new DatabaseServiceHana(_connectionString));

	public IExRateAndIndex ExRateAndIndex => new HanaExRateAndIndexRepository(new DatabaseServiceHana(_connectionString));

	public IChangeLog ChangeLog => new HanaChangeLogRepository(new DatabaseServiceHana(_connectionString));

	public IFormSetting FormSetting => new HanaFormSettingRepository(new DatabaseServiceHana(_connectionString));

	public IUDOs UDOs => new HanaUDORepository(new DatabaseServiceHana(_connectionString));

	public IUDRs UDRs => new HanaUDRRepository(new DatabaseServiceHana(_connectionString));

	public ICFL CFL => new HanaCFLRepository(new DatabaseServiceHana(_connectionString));

	public IQueries Query => new HanaQueriesRepository(new DatabaseServiceHana(_connectionString));

	public IFormula Formula => new HanaFormulaRepository(new DatabaseServiceHana(_connectionString));

	public ICreateFormula CreateFormula => new HanaCreateFormulaRepository(new DatabaseServiceHana(_connectionString));

	public IDiagram Diagram => new HanaDiagramRepository(new DatabaseServiceHana(_connectionString));

	public IORW ORW => new HanaORWRepository(new DatabaseServiceHana(_connectionString));

	public IDefineNew DefineNew => new HanaDefineNewRepository(new DatabaseServiceHana(_connectionString));

	public IUserDefinedTexts UserDefinedTexts => new HanaUserDefinedTextsRepository(new DatabaseServiceHana(_connectionString));

	public IContextAction ContextAction => new HanaContextActionRepository(new DatabaseServiceHana(_connectionString));

	public ISearchSetting SearchSetting => new HanaSearchSettingRepository(new DatabaseServiceHana(_connectionString));

	public IFormFilterSetting FormFilterSetting => new HanaFormFilterSettingRepository(new DatabaseServiceHana(_connectionString));

	public IExternalRequest ExternalRequest => new ExternalRequestRepository(new DatabaseServiceHana(_connectionString));

	public IApprovalDecision ApprovalDecision => new ApprovalDecisionRepository(new DatabaseServiceHana(_connectionString));

	public IDashBoard DashBoard => new HanaDashBoardRepository(new DatabaseServiceHana(_connectionString));

	public IDashboardCategories DashboardCategories => new HanaDashboardCategoriesRepository(new DatabaseServiceHana(_connectionString));

	public IDashBoardTabs DashBoardTabs => new HanaDashBoardTabsRepository(new DatabaseServiceHana(_connectionString));

	public IDashboardAuthorization DashboardAuthorization => new HanaDashBoardAuthRepository(new DatabaseServiceHana(_connectionString));

	public ICategoriesDAB CategoriesDAB => new HanaCategoriesDABRepository(new DatabaseServiceHana(_connectionString));

	public IDashboardDetail DashboardDetail => new HanaDashboardDetailRepository(new DatabaseServiceHana(_connectionString));

	public IMobileDashboard MobileDashboard => new HanaMobileDashboardRepository(new DatabaseServiceHana(_connectionString));

	public ISpecialBP SpecialBP => new HanaSpecialBpRespository(new DatabaseServiceHana(_connectionString));

	public ISpecialBP_LineSPP1 SpecialBP_LineSPP1 => new HanaSpecialBp_LineSPP1Respository(new DatabaseServiceHana(_connectionString));

	public ISpecialBP_LineSPP2 SpecialBP_LineSPP2 => new HanaSpecialBp_LineSPP2Respository(new DatabaseServiceHana(_connectionString));

	public IOCYC OCYC => new HanaOCYCRepository(new DatabaseServiceHana(_connectionString));

	public IIncomingPayment IncomingPayment => new HanaIncomingPaymentRepository(new DatabaseServiceHana(_connectionString));

	public IOutgoingPayment OutgoingPayment => new HanaOutgoingPaymentRepository(new DatabaseServiceHana(_connectionString));

	public ICashItemMasterData CashItemMasterData => new HanaCashItemMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IVoucherMasterData VoucherMasterData => new HanaVoucherMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public ICouponMasterData CouponMasterData => new HanaCouponItemMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IInPayment InPayment => new HanaInPaymentRepository(new DatabaseServiceHana(_connectionString));

	public IOutPayment OutPayment => new HanaOutPaymentRepository(new DatabaseServiceHana(_connectionString));

	public IIncomingPayments IncomingPayments => new HanaIncomingPaymentsRepository(new DatabaseServiceHana(_connectionString));

	public IOutgoingPayments OutgoingPayments => new HanaOutgoingPaymentsRepository(new DatabaseServiceHana(_connectionString));

	public IPaymentRecon PaymentRecon => new HanaPaymentReconRepository(new DatabaseServiceHana(_connectionString));

	public IPaymentRequest PaymentRequest => new HanaPaymentRequestRepository(new DatabaseServiceHana(_connectionString));

	public IMKPurchaseOrder MKPurchaseOrder => new HanaMKPurchaseOrderRepository(new DatabaseServiceHana(_connectionString));

	public IPaymentWizard PaymentWizard => new HanaPaymentWizardRepository(new DatabaseServiceHana(_connectionString));

	public IPayments Payments => new HanaPaymentsRepository(new DatabaseServiceHana(_connectionString));

	public ITrackingIssue TrackingIssue => new HanaTrackingIssueRepository(new DatabaseServiceHana(_connectionString));

	public IProjectManager ProjectManager => new HanaProjectManagerRepository(new DatabaseServiceHana(_connectionString));

	public IProjectReportCompletion ReportCompletion => new HanaProjectReportCompletionRepository(new DatabaseServiceHana(_connectionString));

	public IProjectConfirm ProjectConfirm => new HanaProjectConfirmRepository(new DatabaseServiceHana(_connectionString));

	public IFormatRule FormatRule => new HanaFormatRuleRepository(new DatabaseServiceHana(_connectionString));

	public IWBSTemplate WBSTemplate => new HanaWBSTemplateRepository(new DatabaseServiceHana(_connectionString));

	public IGanttControl GanttControl => new HanaGanttControlRepository(new DatabaseServiceHana(_connectionString));

	public IBillOfMasterials BillOfMasterials => new HanaBillOfMasterialsRepository(new DatabaseServiceHana(_connectionString));

	public IBoMJob BoMJob => new HanaBomJobRepository(new DatabaseServiceHana(_connectionString));

	public IProductionOder ProductionOrder => new HanaProductionOderRepository(new DatabaseServiceHana(_connectionString));

	public IIssueForProduction IssueForProduction => new HanaIssueForProductionRepository(new DatabaseServiceHana(_connectionString));

	public IReceiptProduction ReceiptProduction => new HanaReceiptProductionRepository(new DatabaseServiceHana(_connectionString));

	public IProductionForBatch ProductionForBatch => new HanaProductionForBatchRepository(new DatabaseServiceHana(_connectionString));

	public IProductionConfirm ProductionConfirm => new HanaProductionConfirmRepository(new DatabaseServiceHana(_connectionString));

	public IBomVersion BomVersion => new HanaBomVersionRepository(new DatabaseServiceHana(_connectionString));

	public IMaterialShortageReport MaterialShortageReport => new HanaMaterialShortageReportRepository(new DatabaseServiceHana(_connectionString));

	public IProductionOrderImportTool ProductionOrderImportTool => new HanaProductionOrderImportToolRepository(new DatabaseServiceHana(_connectionString));

	public IOutsourcing Outsourcing => new HanaOutsourcingRepository(new DatabaseServiceHana(_connectionString));

	public IMps Mps => new HanaMpsRepository(new DatabaseServiceHana(_connectionString));

	public IMrpPlan MrpPlan => new HanaMrpPlanRepository(new DatabaseServiceHana(_connectionString));

	public IApzBillOfMaterials ApzBillOfMaterials => new HanaApzBillOfMaterialsRepository(new DatabaseServiceHana(_connectionString));

	public IMasterOrderReplan MasterOrderReplan => new HanaMasterOrderReplanRepository(new DatabaseServiceHana(_connectionString));

	public IResources Resources => new HanaResourcesRepository(new DatabaseServiceHana(_connectionString));

	public IControlKey ControlKey => new HanaControlKeyRepository(new DatabaseServiceHana(_connectionString));

	public IOperation Operation => new HanaOperationRepository(new DatabaseServiceHana(_connectionString));

	public IRouting Routing => new HanaRoutingRepository(new DatabaseServiceHana(_connectionString));

	public IGroupControlKey GroupControlKey => new HanaGroupControlKeyRepository(new DatabaseServiceHana(_connectionString));

	public IListPlant ListPlant => new HanaListPlantRepository(new DatabaseServiceHana(_connectionString));

	public IProductionControl ProductionControl => new HanaProductionControlRepository(new DatabaseServiceHana(_connectionString));

	public IProductionPlanning ProductionPlanning => new HanaProductionPlanningRepository(new DatabaseServiceHana(_connectionString));

	public ITinhGiay TinhGiay => new HanaTinhGiayRepository(new DatabaseServiceHana(_connectionString));

	public ISpecialIssueForBatch SpecialIssueForBatch => new HanaSpecialIssueForBatchRepository(new DatabaseServiceHana(_connectionString));

	public IReportCompleteForProduction ReportCompleteForProduction => new HanaReportCompleteForProductionRepository(new DatabaseServiceHana(_connectionString));

	public IResourceWorkCenter ResourceWorkCenter => new HanaResourceWorkCenterRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunity Opportunity => new HanaOpportunityRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunityStage OpportunityStage => new HanaOpportunityStageRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunityPartner OpportunityPartner => new HanaOpportunityPartnerRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunityRelationship OpportunityRelationship => new HanaOpportunityRelationshipRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunityCompetitor OpportunityCompetitor => new HanaOpportunityCompetitorRepository(new DatabaseServiceHana(_connectionString));

	public IOpportunityReason OpportunityReason => new HanaOpportunityReasonRepository(new DatabaseServiceHana(_connectionString));

	public IActivitiesForOpportunity ActivitiesForOpportunity => new HanaActivitiesForOpportunityRepository(new DatabaseServiceHana(_connectionString));

	public IActivityType ActivityType => new HanaActivityTypeRepository(new DatabaseServiceHana(_connectionString));

	public IActivitySubjects ActivitySubjects => new HanaActivitySubjectsRepository(new DatabaseServiceHana(_connectionString));

	public IActivityMeetingLocation ActivityMeetingLocation => new HanaActivityMeetingLocationRepository(new DatabaseServiceHana(_connectionString));

	public IActivityStatus ActivityStatus => new HanaActivityStatusRepository(new DatabaseServiceHana(_connectionString));

	public IActivityCalendar ActivityCalendar => new HanaActivityCalendarRepository(new DatabaseServiceHana(_connectionString));

	public ICalendarSetting CalendarSetting => new HanaCalendarSettingRepository(new DatabaseServiceHana(_connectionString));

	public IMailSetting MailSetting => new HanaMailSettingRepository(new DatabaseServiceHana(_connectionString));

	public IInterest Interest => new HanaInterestRepository(new DatabaseServiceHana(_connectionString));

	public IInterestLevel InterestLevel => new HanaInterestLevelRepository(new DatabaseServiceHana(_connectionString));

	public IServiceCall ServiceCall => new HanaServiceCallRepository(new DatabaseServiceHana(_connectionString));

	public Isolution Solution => new HanaSolutionRepository(new DatabaseServiceHana(_connectionString));

	public IServiceContract ServiceContract => new HanaServiceContractRepository(new DatabaseServiceHana(_connectionString));

	public IServiceCallSeting ServiceCallSetting => new HanaServiceCallSettingRepository(new DatabaseServiceHana(_connectionString));

	public ISubActivities SubActivities => new HanaSubActivitiesRepository(new DatabaseServiceHana(_connectionString));

	public IServiceCalls ServiceCalls => new HanaServiceCallsRepository(new DatabaseServiceHana(_connectionString));

	public IWarrantyCard WarrantyCard => new HanaWarrantyCardRepository(new DatabaseServiceHana(_connectionString));

	public IServiceLevelAgreement ServiceLevelAgreement => new HanaServiceLevelAgreementRepository(new DatabaseServiceHana(_connectionString));

	public IServiceContractTemplate ServiceContractTemplate => new HanaServiceContractTemplateRepository(new DatabaseServiceHana(_connectionString));

	public IMarketingCampaign MarketingCampaign => new HanaMarketingCampaignRepository(new DatabaseServiceHana(_connectionString));

	public IMRP MRP => new HanaMRPRepository(new DatabaseServiceHana(_connectionString));

	public ISchedule Schedule => new HanaScheduleRepository(new DatabaseServiceHana(_connectionString));

	public ISalesForeCast SalesForeCast => new HanaSalesForeCastRepository(new DatabaseServiceHana(_connectionString));

	public IForecast Forecast => new HanaForecastRepository(new DatabaseServiceHana(_connectionString));

	public IShift Shift => new HanaShiftRepository(new DatabaseServiceHana(_connectionString));

	public IWorkScheduling WorkScheduling => new HanaWorkSchedulingRepository(new DatabaseServiceHana(_connectionString));

	public ILines Lines => new HanaLinesRepository(new DatabaseServiceHana(_connectionString));

	public IItemQuotaArrangement ItemQuotaArrangement => new HanaItemQuotaArrangementRepository(new DatabaseServiceHana(_connectionString));

	public IItemQuotaArrangementDate ItemQuotaArrangementDate => new HanaItemQuotaArrangementDateRepository(new DatabaseServiceHana(_connectionString));

	public IJetstarPreOrder JetstarPreOrder => new HanaJetstarPreOrderRepository(new DatabaseServiceHana(_connectionString));

	public IIrsConfig IrsConfig => new HanaIrsConfigRepository(new DatabaseServiceHana(_connectionString));

	public IAdvanceForecast AdvanceForecast => new HanaAdvanceForecastRepository(new DatabaseServiceHana(_connectionString));

	public IIntegrated Integrated => new HanaIntegratedRepository(new DatabaseServiceHana(_connectionString));

	public IIntegratedInventoryTransfer IntegratedInventoryTransfer => new HanaIntegratedInventoryTransferRepository(new DatabaseServiceHana(_connectionString));

	public IIntegratedBP IntegratedBP => new HanaIntegratedBPRepository(new DatabaseServiceHana(_connectionString));

	public IIntegratedWarehouse IntegratedWarehouse => new HanaIntegratedWarehouseRepository(new DatabaseServiceHana(_connectionString));

	public IIntegratedUdo IntegratedUdo => new HanaIntegratedUdoRepository(new DatabaseServiceHana(_connectionString));

	public IIntegratedItem IntegratedItem => new HanaIntegratedItemRepository(new DatabaseServiceHana(_connectionString));

	public IImportDataBP ImportDataBP => new ImportDataBPRepository(new DatabaseServiceHana(_connectionString));

	public IImportItemMasterData ImportItemMasterData => new ImportItemMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IImportBOM ImportBom => new ImportBOMRepository(new DatabaseServiceHana(_connectionString));

	public IImportData ImportData => new ImportDataRepository(new DatabaseServiceHana(_connectionString));

	public IAdvancePeriodClosing AdvancePeriodClosing => new HanaAdvancePeriodClosingRepository(new DatabaseServiceHana(_connectionString));

	public IRetail Retail => new HanaRetailRepository(new DatabaseServiceHana(_connectionString));

	public IRetailStore RetailStore => new HanaRetailStoreRepository(new DatabaseServiceHana(_connectionString));

	public IPosMaster PosMaster => new HanaPosMasterRepository(new DatabaseServiceHana(_connectionString));

	public IRetailItemSearch ItemSearch => new HanaRetailItemSearchRepository(new DatabaseServiceHana(_connectionString));

	public IRetailCustomerSearch CustomerSearch => new HanaRetailCustomerSearchRepository(new DatabaseServiceHana(_connectionString));

	public IRetailChooseFromList RetailChooseFromList => new HanaRetailChooseFromListRepository(new DatabaseServiceHana(_connectionString));

	public IRetailCmdConfig RetailCmdConfig => new HanaRetailCmdConfigRepository(new DatabaseServiceHana(_connectionString));

	public IDeductionType DeductionType => new HanaDeductionTypeRepository(new DatabaseServiceHana(_connectionString));

	public ISalaryParameters SalaryParameters => new HanaSalaryParametersRepository(new DatabaseServiceHana(_connectionString));

	public IWidget Widget => new HanaWidgetRepository(new DatabaseServiceHana(_connectionString));

	public ICapacity Capacity => new CapacityRepository(new DatabaseServiceHana(_connectionString));

	public IOperations Operations => new OperationsRepository(new DatabaseServiceHana(_connectionString));

	public IAdvanceRouting AdvanceRouting => new AdvanceRoutingRepository(new DatabaseServiceHana(_connectionString));

	public IResourceWorkSchedule ResourceWorkSchedule => new HanaResourceWorkScheduleRepository(new DatabaseServiceHana(_connectionString));

	public ISMSConfig SmsConfig => new HanaSMSConfigRepository(new DatabaseServiceHana(_connectionString));

	public IFile Files => new HanaFileRepository(new DatabaseServiceHana(_connectionString));

	public IResourceGroup ResourceGroup => new ResourceGroupRepository(new DatabaseServiceHana(_connectionString));

	public IPublishService PublishService => new HanaPublishServiceRepository(new DatabaseServiceHana(_connectionString));

	public ICapacitySetting CapacitySetting => new CapacitySettingRepository(new DatabaseServiceHana(_connectionString));

	public IAdditionalGeneralSettings AdditionalGeneralSettings => new HanaAdditionalGeneralSettingsRepository(new DatabaseServiceHana(_connectionString));

	public IBarcodeInventoryCounting BarcodeInventoryCounting => new BarcodeInventoryCountingRepository(new DatabaseServiceHana(_connectionString));

	public IJobsManagement JobsManagement => new JobsManagementRepository(new DatabaseServiceHana(_connectionString));

	public IGeography Geography => new HanaGeographyRepository(new DatabaseServiceHana(_connectionString));

	public IStore Store => new HanaStoreRepository(new DatabaseServiceHana(_connectionString));

	public ISaleTerritory SaleTerritory => new HanaSaleTerritoryRepository(new DatabaseServiceHana(_connectionString));

	public ISurvey Survey => new HanaSurveyRepository(new DatabaseServiceHana(_connectionString));

	public IQuestionMasterData QuestionMasterData => new HanaQuestionMasterDataRepository(new DatabaseServiceHana(_connectionString));

	public IUploadImage UploadImage => new HanaUploadImageRepository(new DatabaseServiceHana(_connectionString));

	public ISettingReport SettingReport => new HanaSettingReportRepository(new DatabaseServiceHana(_connectionString));

	public IOutletVisitingFrequency OutletVisitingFrequency => new OutletVisitingFrequencyRepository(new DatabaseServiceHana(_connectionString));

	public IPromotionSetup PromotionSetup => new HanaPromotionSetupRepositoty(new DatabaseServiceHana(_connectionString));

	public IChannel Channel => new HanaChannelRepository(new DatabaseServiceHana(_connectionString));

	public ISalesRoute SalesRoute => new HanaSalesRouteRepository(new DatabaseServiceHana(_connectionString));

	public IImageAlbum ImageAlbum => new HanaImageAlbumRepository(new DatabaseServiceHana(_connectionString));

	public IMarker Marker => new HanaMarkerRepository(new DatabaseServiceHana(_connectionString));

	public IDMSDelivery DMSDelivery => new HanaDMSDeliveryRepository(new DatabaseServiceHana(_connectionString));

	public IAlertTemplate AlertTemplate => new AlertTemplateRepository(new DatabaseServiceHana(_connectionString));

	public IDMSApproved DMSApproved => new HanaDMSApprovedRepository(new DatabaseServiceHana(_connectionString));

	public ICommentCustomer CommentCustomer => new CommentCustomerRepository(new DatabaseServiceHana(_connectionString));

	public IContactUs ContactUs => new ContactUsRepository(new DatabaseServiceHana(_connectionString));

	public IVisitStore VisitStore => new HanaVisitStoreRepository(new DatabaseServiceHana(_connectionString));

	public IPromotionCondition PromotionCondition => new HanaPromotionConditionRepository(new DatabaseServiceHana(_connectionString));

	public ITrackingUserLocation TrackingUserLocation => new TrackingUserLocationRepository(new DatabaseServiceHana(_connectionString));

	public IPackingOrder PackingOrder => new HanaPackingOrderRepository(new DatabaseServiceHana(_connectionString));

	public ICreateTypeCase CreateTypeCase => new HanaCreateTypeCaseRepository(new DatabaseServiceHana(_connectionString));

	public ITraceability Traceability => new HanaTraceabilityRepository(new DatabaseServiceHana(_connectionString));

	public IPrintBillWrap PrintBillWrap => new HanaPrintBillWrapRepository(new DatabaseServiceHana(_connectionString));

	public IApprovalStages ApprovalStages => new HanaApprovalStagesRepository(new DatabaseServiceHana(_connectionString));

	public IApprovalTemplates ApprovalTemplates => new HanaApprovalTemplatesRepository(new DatabaseServiceHana(_connectionString));

	public IWorkingProcess WorkingProcess => new HanaWorkingProcessRepository(new DatabaseServiceHana(_connectionString));

	public INotification Notification => new NotificationRepository(new DatabaseServiceHana(_connectionString));

	public IOutlet Outlet => new HanaOutletRepository(new DatabaseServiceHana(_connectionString), this);

	public IPromotion Promotion => new HanPromotionRepository(new DatabaseServiceHana(_connectionString));

	public IDMSReport DMSReport => new HanaDMSReportRepository(new DatabaseServiceHana(_connectionString));

	public IPromotionRegister PromotionRegister => new HanaPromotionRegisterRepository(new DatabaseServiceHana(_connectionString));

	public IPromotionBenefit PromotionBenefit => new HanaPromotionBenefitRepository(new DatabaseServiceHana(_connectionString));

	public ICheckPromotion CheckPromotion => new HanaCheckPromotionRepository(new DatabaseServiceHana(_connectionString));

	public IDMSSettingUDF DMSSettingUDF => new HanaDMSSettingUDFRepository(new DatabaseServiceHana(_connectionString));

	public IPromotionProgram PromotionProgram => new HanaPromotionProgramRepository(new DatabaseServiceHana(_connectionString));

	public IKPIsMaterData KPIsMaterData => new HanaKPIsMaterDataRepository(new DatabaseServiceHana(_connectionString));

	public ISetIndicatorKPIs SetIndicatorKPIs => new HanaSetIndicatorKPIsRepository(new DatabaseServiceHana(_connectionString));

	public IKPIsSetting KPIsSetting => new HanaKPIsSettingRepository(new DatabaseServiceHana(_connectionString));

	public IListPointKPIs ListPointKPIs => new HanaListPointKPIsRepository(new DatabaseServiceHana(_connectionString));

	public IKPIPlan KPIPlan => new HanaKPIPlanRepository(new DatabaseServiceHana(_connectionString));

	public IKPIScope KPIScope => new HanaKPIScopeRepository(new DatabaseServiceHana(_connectionString));

	public IKPIIndicator KPIIndicator => new HanaKPIIndicatorRepository(new DatabaseServiceHana(_connectionString));

	public IKPIGroup KPIGroup => new HanaKPIGroupRepository(new DatabaseServiceHana(_connectionString));

	public IKPISetup KPISetup => new HanaKPISetupRepository(new DatabaseServiceHana(_connectionString));

	public IKPIScore KPIScore => new HanaKPIScoreRepository(new DatabaseServiceHana(_connectionString));

	public IThirdPartyToken ThirdPartyToken => new HanaThirdPartyTokenRepository(new DatabaseServiceHana(_connectionString));

	public IAboutRangeTime AboutRangeTime => new HanaAboutRangeTimeRepository(new DatabaseServiceHana(_connectionString));

	public ISettingApprovalDetail SettingApprovalDetail => new HanaSettingApprovalDetailRepository(new DatabaseServiceHana(_connectionString));

	public IComments Comments => new CommentsRepository(new DatabaseServiceHana(_connectionString));

	public HanaUnitOfWork(string connectionString)
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
