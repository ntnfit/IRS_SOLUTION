using System;
using Apzon.DataServices.HanaImplements;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices;

public class HanaUnitOfWork : IUnitOfWork, IDisposable
{
	private string _serviceAddress;

	private Transaction _transaction;

	private bool _disposed;

	private Apzon.Services.IUnitOfWork _serviceUnitOfWork { get; set; }

	public IShift Shift => new HanaShiftRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IWorkScheduling WorkScheduling => new HanaWorkSchedulingRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IWorkSchedulingLine WorkSchedulingLine => new HanaWorkSchedulingLineRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IMps Mps => new HanaMpsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IMrp Mrp => new HanaMrpRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IPrs Prs => new HanaPrsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IUser User => new HanaUserRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ICommon Common => new HanaCommonRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUserFields UserFields => new HanaUserFieldsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUDOs UDOs => new HanaUDORepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IItems Items => new HanaItemsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IInventoryGenEntry InventoryGenEntry => new HanaInventoryGenEntryRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IInventoryGenExit InventoryGenExit => new HanaInventoryGenExitRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IStockTransferDraft StockTransferDraft => new HanaStockTransferDraftRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IStockTransfer StockTransfer => new HanaStockTransferRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IStockTransferRequest StockTransferRequest => new HanaStockTransferRequestRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ISTDPriceLists STDPriceLists => new HanaSTDPriceListsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress), this);

	public IWarehouses Warehouses => new HanaWarehousesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IPickLists PickLists => new HanaPickListsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress), this);

	public ISpecialPrices SpecialPrices => new HanaSpecialPricesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IPickingDocument PickingDocument => new HanaPickingDocumentRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IShipping Shipping => new HanaShippingRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBatchNumberDetails BatchNumberDetails => new HanaBatchNumberDetailsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ISerialNumberDetails SerialNumberDetails => new HanaSerialNumberDetailsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers => new HanaBusinessPartnerCatalogNumbersRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IAlternativeItem AlternativeItem => new HanaAlternativeItemRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IDocuments Documents => new HanaDocumentsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IInvoices Invoices => new HanaInvoicesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IDelivery Delivery => new HanaDeliveryRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ISalesControl SalesControl => new HanaSalesControlRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ISalesBlanketAgreement SalesBlanketAgreement => new HanaSalesBlanketAgreementRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IPickAndPacks PickAndPack => new HanaPickAndPackRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IOpportunity Opportunity => new HanaOpportunityReopository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ICompetitor Competitor => new HanaCompetitorRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IRelationship Relationship => new HanaRelationshipRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IOpportunityStage OpportunityStage => new HanaOpportunityStageRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IInterest Interest => new HanaInterestRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IInterestLevel InterestLevel => new HanaInterestLevelRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IOpportunityPartners OpportunityPartners => new HanaOpportunityPartnersRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IOpportunityReasons OpportunityReasons => new HanaOpportunityReasonsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IActivities Activities => new HanaActivitiesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBusinessPartners BusinessPartners => new HanaBusinessPartnersRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IIndustry Industry => new HanaIndustryRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IContractTemplate ContractTemplate => new HanaContractTemplateRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IManufacturer Manufacturer => new HanaManufacturerRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IActivitiesMeetingLocation ActivitiesMeetingLocation => new HanaActivitiesMeetingLocationRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IGroupCustomerVender GroupCustomerVender => new HanaGroupCustomerVenderRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IProcurementConfirm ProcurementConfirm => new HanaProcurementConfirmRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IProductTrees ProductTrees => new HanaProductTreesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IProductionOrders ProductionOrders => new HanaProductionOrdersRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IProductionOrderImportTool ProductionOrderImportTool => new HanaProductionOrderImportToolRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IOutsourcing Outsourcing => new HanaOutsourcingRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IServiceCall ServiceCall => new HanaServiceCallRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IServiceContract ServiceContract => new HanaServiceContractRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUsers Users => new HanaUsersRepository(_serviceUnitOfWork, new HanaService(_serviceAddress), this);

	public IIntergrated Intergrated => new HanaIntergratedRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ISolution Solution => new HanaSolutionRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IPayments Payments => new HanaPaymentsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IMaster Master => new HanaMasterRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IProject Project => new HanaProjectRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IOCYC OCYC => new HanaOCYCRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IEmployeeMasterData EmployeeMasterData => new HanaEmpMasterDataRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ICreditCard CreditCard => new HanaCreditCardRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ICreditPaymentMethods CreditPaymentMethods => new HanaCreditPaymentMethodsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IDistributionRule DistributionRule => new HanaDistributionRuleRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IDimension Dimension => new HanaDimensionRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ISalesPersons SalesPersons => new HanaSalesPersonsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IProfitCenters ProfitCenters => new HanaProfitCentersRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ITerritory Territories => new HanaTerritoryRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IShippingType ShippingType => new HanaShippingTypeRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IItemGroups ItemGroups => new HanaItemGroupsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress), this);

	public IJournalEntries JournalEntries => new HanaJournalEntriesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IMessage Message => new HanaMessagesRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IApprovalRequests ApprovalRequests => new HanaApprovalRequestsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IChangeLog ChangeLog => new HanaChangeLogRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IDiscountGroup DiscountGroup => new HanaDiscountGroupRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IResourceGroup ResourceGroup => null;

	public IChartOfAccount ChartOfAccount => new HanaChartOfAccountRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IVatGroups VatGroups => new HanaVatGroupsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ICurrencies Currencies => new HanaCurrenciesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ICountries Countries => new HanaCountriesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ILengthAndWidth LengthAndWidth => new HanaLengthAndWidthRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IWeight Weight => new HanaWeightRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUserDefinedTexts UserDefinedTexts => new HanaUserDefinedTextsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ICustomGroups CustomGroups => new HanaCustomGroupsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ILandedCosts LandedCosts => new HanaLandedCostsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUnitOfMeasureGroups UnitOfMeasureGroups => new HanaUnitOfMeasureGroupsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBinLocMaster BinLocMaster => new HanaBinLocMasterRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IWarehouseSublevel WarehouseSublevel => new HanaWarehouseSublevelRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBinLocationAttribute BinLocationAttribute => new HanaBinLocationAttributeRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBinLocationField BinLocationField => new HanaBinLocationFieldRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IFreightSetup FreightSetup => new HanaFreightSetupRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IBranches Branches => new HanaBranchesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IPaymentTermSetUp PaymentTermSetUp => new HanaPaymentTermSetUpRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IApprovalStages ApprovalStages => new HanaApprovalStagesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IApprovalTemplates ApprovalTemplates => new HanaApprovalTemplatesRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUserTablesDefined UserTablesDefined => new HanaUserTablesDefinedRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IUserObjectDefined UserObjectDefined => new HanaUserObjectDefinedRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public IInternalReconciliations InternalReconciliations => new HanaInternalReconciliationsRepository(_serviceUnitOfWork, this, new HanaService(_serviceAddress));

	public ITeams Teams => new HanaTeamsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IRoles Roles => new HanaRolesRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IUnitOfMeasurements UnitOfMeasurements => new HanaUnitOfMeasurementsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public ILocations Locations => new HanaLocationsRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IInventoryCycles InventoryCycles => new HanaInventoryCyclesRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IActivityStatus ActivityStatus => new HanaActivityStatusRepository(_serviceUnitOfWork, new HanaService(_serviceAddress));

	public IAttachment2 Attachment2 => new HanaAttachment2(_serviceUnitOfWork, new HanaService(_serviceAddress));

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

	public HanaUnitOfWork(Apzon.Services.IUnitOfWork serviceUnitOfWork, string serviceAddress)
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
