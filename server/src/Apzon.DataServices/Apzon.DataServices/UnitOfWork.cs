using System;
using Apzon.DataServices.Implements;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
	private string _serviceAddress;

	private Transaction _transaction;

	private bool _disposed;

	private Apzon.Services.IUnitOfWork _serviceUnitOfWork { get; set; }

	public IShift Shift => new ShiftRepository(_serviceUnitOfWork, new DiServices());

	public IWorkScheduling WorkScheduling => new WorkSchedulingRepository(_serviceUnitOfWork, new DiServices());

	public IWorkSchedulingLine WorkSchedulingLine => new WorkSchedulingLineRepository(_serviceUnitOfWork, new DiServices());

	public IMps Mps => new MpsRepository(_serviceUnitOfWork, new DiServices());

	public IMrp Mrp => new MrpRepository(_serviceUnitOfWork, new DiServices());

	public IPrs Prs => new PrsRepository(_serviceUnitOfWork, new DiServices());

	public IUser User => new UserRepository(_serviceUnitOfWork, new DiServices());

	public ICommon Common => new CommonRepository(_serviceUnitOfWork, this, new DiServices());

	public IUserFields UserFields => new UserFieldsRepository(_serviceUnitOfWork, this, new DiServices());

	public IUDOs UDOs => new UDORepository(_serviceUnitOfWork, new DiServices());

	public IItems Items => new ItemsRepository(_serviceUnitOfWork, this, new DiServices());

	public IInventoryGenEntry InventoryGenEntry => new InventoryGenEntryRepository(_serviceUnitOfWork, this, new DiServices());

	public IInventoryGenExit InventoryGenExit => new InventoryGenExitRepository(_serviceUnitOfWork, new DiServices());

	public IStockTransferDraft StockTransferDraft => new StockTransferDraftRepository(_serviceUnitOfWork, new DiServices());

	public IStockTransfer StockTransfer => new StockTransferRepository(_serviceUnitOfWork, this, new DiServices());

	public IStockTransferRequest StockTransferRequest => new StockTransferRequestRepository(_serviceUnitOfWork, this, new DiServices());

	public ISTDPriceLists STDPriceLists => new STDPriceListsRepository(_serviceUnitOfWork, new DiServices(), this);

	public IWarehouses Warehouses => new WarehousesRepository(_serviceUnitOfWork, this, new DiServices());

	public IPickLists PickLists => new PickListsRepository(_serviceUnitOfWork, new DiServices(), this);

	public ISpecialPrices SpecialPrices => new SpecialPricesRepository(_serviceUnitOfWork, this, new DiServices());

	public IPickingDocument PickingDocument => new PickingDocumentRepository(_serviceUnitOfWork, this, new DiServices());

	public IShipping Shipping => new ShippingRepository(_serviceUnitOfWork, this, new DiServices());

	public IBatchNumberDetails BatchNumberDetails => new BatchNumberDetailsRepository(_serviceUnitOfWork, this, new DiServices());

	public ISerialNumberDetails SerialNumberDetails => new SerialNumberDetailsRepository(_serviceUnitOfWork, this, new DiServices());

	public IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers => new BusinessPartnerCatalogNumbersRepository(_serviceUnitOfWork, this, new DiServices());

	public IAlternativeItem AlternativeItem => new AlternativeItemRepository(_serviceUnitOfWork, new DiServices());

	public IDocuments Documents => new DocumentsRepository(_serviceUnitOfWork, this, new DiServices());

	public IInvoices Invoices => new InvoicesRepository(_serviceUnitOfWork, this, new DiServices());

	public IDelivery Delivery => new DeliveryRepository(_serviceUnitOfWork, this, new DiServices());

	public ISalesControl SalesControl => new SalesControlRepository(_serviceUnitOfWork, new DiServices());

	public ISalesBlanketAgreement SalesBlanketAgreement => new SalesBlanketAgreementRepository(_serviceUnitOfWork, new DiServices());

	public IPickAndPacks PickAndPack => new PickAndPackRepository(_serviceUnitOfWork, this, new DiServices());

	public IOpportunity Opportunity => new OpportunityRepository(_serviceUnitOfWork, this, new DiServices());

	public ICompetitor Competitor => new CompetitorRepository(_serviceUnitOfWork, this, new DiServices());

	public IRelationship Relationship => new RelationshipRepository(_serviceUnitOfWork, this, new DiServices());

	public IOpportunityStage OpportunityStage => new OpportunityStageRepository(_serviceUnitOfWork, new DiServices());

	public IInterest Interest => new InterestRepository(_serviceUnitOfWork, this, new DiServices());

	public IInterestLevel InterestLevel => new InterestLevelRepository(_serviceUnitOfWork, new DiServices());

	public IOpportunityPartners OpportunityPartners => new OpportunityPartnersRepository(_serviceUnitOfWork, this, new DiServices());

	public IOpportunityReasons OpportunityReasons => new OpportunityReasonsRepository(_serviceUnitOfWork, this, new DiServices());

	public IActivities Activities => new ActivitiesRepository(_serviceUnitOfWork, this, new DiServices());

	public IBusinessPartners BusinessPartners => new BusinessPartnersRepository(_serviceUnitOfWork, this, new DiServices());

	public IIndustry Industry => new IndustryRepository(_serviceUnitOfWork, this, new DiServices());

	public IContractTemplate ContractTemplate => new ContractTemplateRepository(_serviceUnitOfWork, this, new DiServices());

	public IManufacturer Manufacturer => new ManufacturerRepository(_serviceUnitOfWork, this, new DiServices());

	public IActivitiesMeetingLocation ActivitiesMeetingLocation => new ActivitiesMeetingLocationRepository(_serviceUnitOfWork, this, new DiServices());

	public IGroupCustomerVender GroupCustomerVender => new GroupCustomerVenderRepository(_serviceUnitOfWork, new DiServices());

	public IProcurementConfirm ProcurementConfirm => new ProcurementConfirmRepository(_serviceUnitOfWork, new DiServices());

	public ILandedCosts LandedCosts => new LandedCostsRepository(_serviceUnitOfWork, this, new DiServices());

	public IProductTrees ProductTrees => new ProductTreesRepository(_serviceUnitOfWork, this, new DiServices());

	public IProductionOrders ProductionOrders => new ProductionOrdersRepository(_serviceUnitOfWork, this, new DiServices());

	public IProductionOrderImportTool ProductionOrderImportTool => new ProductionOrderImportToolRepository(_serviceUnitOfWork, this, new DiServices());

	public IOutsourcing Outsourcing => new OutsourcingRepository(_serviceUnitOfWork, this, new DiServices());

	public IServiceCall ServiceCall => new ServiceCallRepository(_serviceUnitOfWork, new DiServices());

	public IServiceContract ServiceContract => new ServiceContractRepository(_serviceUnitOfWork, this, new DiServices());

	public IUsers Users => new UsersRepository(_serviceUnitOfWork, new DiServices(), this);

	public IIntergrated Intergrated => new IntergratedRepository(_serviceUnitOfWork, this, new DiServices());

	public ISolution Solution => new SolutionRepository(_serviceUnitOfWork, this, new DiServices());

	public IPayments Payments => new PaymentsRepository(_serviceUnitOfWork, new DiServices());

	public IMaster Master => new MasterRepository(_serviceUnitOfWork, new DiServices());

	public IProject Project => new ProjectRepository(_serviceUnitOfWork, new DiServices());

	public IOCYC OCYC => new OCYCRepository(_serviceUnitOfWork, new DiServices());

	public IEmployeeMasterData EmployeeMasterData => new EmpMasterDataRepository(_serviceUnitOfWork, this, new DiServices());

	public ICreditCard CreditCard => new CreditCardRepository(_serviceUnitOfWork, new DiServices());

	public ICreditPaymentMethods CreditPaymentMethods => new CreditPaymentMethodsRepository(_serviceUnitOfWork, new DiServices());

	public IDistributionRule DistributionRule => new DistributionRuleRepository(_serviceUnitOfWork, new DiServices());

	public IDimension Dimension => new DimensionRepository(_serviceUnitOfWork, new DiServices());

	public ISalesPersons SalesPersons => new SalesPersonsRepository(_serviceUnitOfWork, new DiServices());

	public IProfitCenters ProfitCenters => new ProfitCentersRepository(_serviceUnitOfWork, new DiServices());

	public ITerritory Territories => new TerritoryRepository(_serviceUnitOfWork, new DiServices());

	public IShippingType ShippingType => new ShippingTypeRepository(_serviceUnitOfWork, new DiServices());

	public IItemGroups ItemGroups => new ItemGroupsRepository(_serviceUnitOfWork, new DiServices(), this);

	public IDiscountGroup DiscountGroup => new DiscountGroupRepository(_serviceUnitOfWork, new DiServices());

	public IJournalEntries JournalEntries => new JournalEntriesRepository(_serviceUnitOfWork, this, new DiServices());

	public IChartOfAccount ChartOfAccount => new ChartOfAccountRepository(_serviceUnitOfWork, this, new DiServices());

	public ICurrencies Currencies => new CurrenciesRepository(_serviceUnitOfWork, this, new DiServices());

	public IMessage Message => new MessagesRepository(_serviceUnitOfWork, new DiServices());

	public IApprovalRequests ApprovalRequests => new ApprovalRequestsRepository(_serviceUnitOfWork, this, new DiServices());

	public IChangeLog ChangeLog => new ChangeLogRepository(_serviceUnitOfWork, new DiServices());

	public IResourceGroup ResourceGroup => new ResourceGroupRepository(_serviceUnitOfWork, new DiServices());

	public IVatGroups VatGroups => new VatGroupsRepository(_serviceUnitOfWork, this, new DiServices());

	public ICountries Countries => new CountriesRepository(_serviceUnitOfWork, this, new DiServices());

	public ILengthAndWidth LengthAndWidth => new LengthAndWidthRepository(_serviceUnitOfWork, this, new DiServices());

	public IWeight Weight => new WeightRepository(_serviceUnitOfWork, this, new DiServices());

	public IUserDefinedTexts UserDefinedTexts => null;

	public ICustomGroups CustomGroups => new CustomGroupsRepository(_serviceUnitOfWork, this, new DiServices());

	public IUnitOfMeasureGroups UnitOfMeasureGroups => new UnitOfMeasureGroupsRepository(_serviceUnitOfWork, this, new DiServices());

	public IBinLocMaster BinLocMaster => new BinLocMasterRepository(_serviceUnitOfWork, this, new DiServices());

	public IWarehouseSublevel WarehouseSublevel => new WarehouseSublevelRepository(_serviceUnitOfWork, this, new DiServices());

	public IBinLocationAttribute BinLocationAttribute => new BinLocationAttributeRepository(_serviceUnitOfWork, this, new DiServices());

	public IBinLocationField BinLocationField => new BinLocationFieldRepository(_serviceUnitOfWork, this, new DiServices());

	public IFreightSetup FreightSetup => new FreightSetupReopsitory(_serviceUnitOfWork, this, new DiServices());

	public IBranches Branches => new BranchesRepository(_serviceUnitOfWork, this, new DiServices());

	public IPaymentTermSetUp PaymentTermSetUp => new PaymentTermSetupRepository(_serviceUnitOfWork, this, new DiServices());

	public IApprovalStages ApprovalStages => new ApprovalStagesRepository(_serviceUnitOfWork, this, new DiServices());

	public IApprovalTemplates ApprovalTemplates => new ApprovalTemplatesRepository(_serviceUnitOfWork, this, new DiServices());

	public IUserTablesDefined UserTablesDefined => new UserTablesDefinedRepository(_serviceUnitOfWork, this, new DiServices());

	public IUserObjectDefined UserObjectDefined => new UserObjectDefinedRepository(_serviceUnitOfWork, this, new DiServices());

	public IInternalReconciliations InternalReconciliations => new InternalReconciliationsRepository(_serviceUnitOfWork, this, new DiServices());

	public ITeams Teams => new TeamsRepository(_serviceUnitOfWork, new DiServices());

	public IRoles Roles => new RolesRepository(_serviceUnitOfWork, new DiServices());

	public IUnitOfMeasurements UnitOfMeasurements => new UnitOfMeasurementsRepository(_serviceUnitOfWork, new DiServices());

	public ILocations Locations => new LocationsRepository(_serviceUnitOfWork, new DiServices());

	public IInventoryCycles InventoryCycles => new InventoryCyclesRepository(_serviceUnitOfWork, new DiServices());

	public IActivityStatus ActivityStatus => new ActivityStatusRepository(_serviceUnitOfWork, new DiServices());

	public IAttachment2 Attachment2 => new Attachment2(_serviceUnitOfWork, new DiServices());

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

	public UnitOfWork(Apzon.Services.IUnitOfWork serviceUnitOfWork, string serviceAddress)
	{
		_serviceUnitOfWork = serviceUnitOfWork;
		_serviceAddress = serviceAddress;
	}

	public void BeginTransaction()
	{
		if (_transaction == null)
		{
			_transaction = new Transaction();
		}
	}

	public void Commit()
	{
		_transaction.Commit();
	}
}
