using System;
using Apzon.DataServices.Interfaces;

namespace Apzon.DataServices;

public interface IUnitOfWork : IDisposable
{
	IWorkScheduling WorkScheduling { get; }

	IWorkSchedulingLine WorkSchedulingLine { get; }

	IMps Mps { get; }

	IMrp Mrp { get; }

	IPrs Prs { get; }

	IUser User { get; }

	ICommon Common { get; }

	IUserFields UserFields { get; }

	IDocuments Documents { get; }

	IActivities Activities { get; }

	IUDOs UDOs { get; }

	IProject Project { get; }

	IEmployeeMasterData EmployeeMasterData { get; }

	ICreditCard CreditCard { get; }

	ICreditPaymentMethods CreditPaymentMethods { get; }

	IDistributionRule DistributionRule { get; }

	IDimension Dimension { get; }

	ISalesPersons SalesPersons { get; }

	IProfitCenters ProfitCenters { get; }

	ITerritory Territories { get; }

	IShippingType ShippingType { get; }

	IItemGroups ItemGroups { get; }

	IManufacturer Manufacturer { get; }

	IActivitiesMeetingLocation ActivitiesMeetingLocation { get; }

	IMaster Master { get; }

	IOpportunity Opportunity { get; }

	IOpportunityStage OpportunityStage { get; }

	ICompetitor Competitor { get; }

	IRelationship Relationship { get; }

	IInterest Interest { get; }

	IInterestLevel InterestLevel { get; }

	IOpportunityPartners OpportunityPartners { get; }

	IOpportunityReasons OpportunityReasons { get; }

	IItems Items { get; }

	IInventoryGenEntry InventoryGenEntry { get; }

	IInventoryGenExit InventoryGenExit { get; }

	IStockTransferDraft StockTransferDraft { get; }

	IStockTransfer StockTransfer { get; }

	IStockTransferRequest StockTransferRequest { get; }

	ISTDPriceLists STDPriceLists { get; }

	IOCYC OCYC { get; }

	IWarehouses Warehouses { get; }

	IPickLists PickLists { get; }

	ISpecialPrices SpecialPrices { get; }

	IPickingDocument PickingDocument { get; }

	IShipping Shipping { get; }

	IBatchNumberDetails BatchNumberDetails { get; }

	ISerialNumberDetails SerialNumberDetails { get; }

	IBusinessPartnerCatalogNumbers BusinessPartnerCatalogNumbers { get; }

	IAlternativeItem AlternativeItem { get; }

	IDiscountGroup DiscountGroup { get; }

	IProcurementConfirm ProcurementConfirm { get; }

	ILandedCosts LandedCosts { get; }

	IInvoices Invoices { get; }

	IDelivery Delivery { get; }

	ISalesControl SalesControl { get; }

	ISalesBlanketAgreement SalesBlanketAgreement { get; }

	IPickAndPacks PickAndPack { get; }

	IProductionOrderImportTool ProductionOrderImportTool { get; }

	IProductTrees ProductTrees { get; }

	IProductionOrders ProductionOrders { get; }

	IOutsourcing Outsourcing { get; }

	IBusinessPartners BusinessPartners { get; }

	IIndustry Industry { get; }

	IContractTemplate ContractTemplate { get; }

	IGroupCustomerVender GroupCustomerVender { get; }

	IServiceCall ServiceCall { get; }

	IServiceContract ServiceContract { get; }

	ISolution Solution { get; }

	IUsers Users { get; }

	IIntergrated Intergrated { get; }

	IPayments Payments { get; }

	IJournalEntries JournalEntries { get; }

	IChartOfAccount ChartOfAccount { get; }

	IMessage Message { get; }

	IApprovalRequests ApprovalRequests { get; }

	IChangeLog ChangeLog { get; }

	IResourceGroup ResourceGroup { get; }

	IVatGroups VatGroups { get; }

	ICurrencies Currencies { get; }

	ICountries Countries { get; }

	ILengthAndWidth LengthAndWidth { get; }

	IWeight Weight { get; }

	IUserDefinedTexts UserDefinedTexts { get; }

	ICustomGroups CustomGroups { get; }

	IUnitOfMeasureGroups UnitOfMeasureGroups { get; }

	IBinLocMaster BinLocMaster { get; }

	IWarehouseSublevel WarehouseSublevel { get; }

	IBinLocationAttribute BinLocationAttribute { get; }

	IBinLocationField BinLocationField { get; }

	IFreightSetup FreightSetup { get; }

	IBranches Branches { get; }

	IPaymentTermSetUp PaymentTermSetUp { get; }

	IApprovalStages ApprovalStages { get; }

	IApprovalTemplates ApprovalTemplates { get; }

	IUserTablesDefined UserTablesDefined { get; }

	IUserObjectDefined UserObjectDefined { get; }

	IInternalReconciliations InternalReconciliations { get; }

	ITeams Teams { get; }

	IRoles Roles { get; }

	IUnitOfMeasurements UnitOfMeasurements { get; }

	ILocations Locations { get; }

	IInventoryCycles InventoryCycles { get; }

	IActivityStatus ActivityStatus { get; }

	IAttachment2 Attachment2 { get; }

	void BeginTransaction();
}
