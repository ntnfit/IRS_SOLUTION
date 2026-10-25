using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class HttpClientRepository : IHttpClient
{
	private readonly ApzObjectTypes _type;

	public ICommonClient Commons => new CommonsClient();

	public IUdoClient Udo => new UdoClient();

	public IInventoryCountingBinClient InventoryCountingBin => new InventoryCountingBinClient();

	public IStockCountingClient StockCounting => new StockCountingClient();

	public IInventoryCountingRequestClient InventoryCountingRequest => new InventoryCountingRequestClient();

	public IInventoryCountingClient InventoryCounting => new InventoryCountingClient();

	public IStockTransferClient StockTransfer => new StockTransferClient();

	public IIrsConfigClient IrsConfig => new IrsConfigClient();

	public IUdfClient Udf => new UdfClient();

	public IOwnerAuthorizeClient OwnerAuthorize => new OwnerAuthorizeClient();

	public ICflClient Cfl => new CflClient();

	public IProductionOrdersClient ProductionOrders => new ProductionOrdersClient();

	public IBillOfMasterialsClient BillOfMasterials => new BillOfMasterialsClient();

	public IDocumentsClient Documents => new DocumentsClient(_type);

	public IFilterSettingClient FilterSetting => new FilterSettingClient();

	public IPackingBatch PackingBatch => new PackingBatch();

	public IUserClient User => new UserClientRepository();

	public IItems Items => new ItemsClient();

	public IIncommingPayments IncommingPayments => new IncommingPaymentsClient();

	public IMrpClient Mrp => new MrpClient();

	public IProductionScheduleClient ProductionSchedule => new ProductionScheduleClient();

	public IBusinessMasterClient BusinessMaster => new BusinessMasterClient();

	public IReceiptIssueBin ReceiptIssueBin => new ReceiptIssueBinClient();

	public IDiagram Diagram => new DiagramClient();

	public IFormula Formula => new FormulaClient();

	public IWarehouseClient Warehouse => new WarehouseClient();

	public IInventoryPostingListClient InventoryPostingList => new InventoryPostingListClient();

	public ISalesEmployeeBuyerClient SalesEmployeeBuyerClient => new SalesEmployeeBuyerClient();

	public ICostCenters CostCentersClient => new CostCentersClient();

	public ILoginClient LoginClient => new LoginClient();

	public IRetailChooseFromList RetailChooseFromList => new RetailChooseFromListClient();

	public IRetailClient Retail => new RetailClientHelper();

	public IRetailStoreClient RetailStore => new RetailStoreClient();

	public IPosMasterClient PosMasterClient => new PosMasterClient();

	public IGeneralSetting GeneralSetting => new GeneralSettingClient();

	public IBatchDetails BatchDetails => new BatchDetailsClient();

	public ISerialDetails SerialDetails => new SerialDetailsClient();

	public IAdvanceForecastClient AdvanceForecastClient => new AdvanceForecastClient();

	public IScheduleClient ScheduleClient => new ScheduleClient();

	public IInventoryTransferClient InventoryTransferClient => new InventoryTransferClient();

	public IInventoryCyclesClient InventoryCycles => new InventoryCyclesClient();

	public ICycleCountDeterminationClient CycleCountDetermination => new CycleCountDeterminationClient();

	public ICycleCountRecommendationClient CycleCountRecommendation => new CycleCountRecommendationClient();

	public ICycleCDSubTableClient CycleCDSubTable => new CycleCDSubTableClient();

	public IEmployeeMasterClient EmployeeMasterClient => new EmployeeMasterClient();

	public IXtraReportManagerClient XtraReportManagerClient => new XtraReportManagerClient();

	public IBusinessPartnerCatalogNumbersClient BusinessPartnerCatalogNumbersClient => new BusinessPartnerCatalogNumbersClient();

	public IEmpRelativesClient EmpRelativesClient => new EmpRelativesClient();

	public IJobVacancyClient JobVacancy => new JobVacancyClient();

	public IPaymentTermSetup PaymentTermSetup => new PaymentTermSetupClient();

	public IPriceListSetup PriceListSetup => new PriceListSetupClient();

	public IAtpClient AtpClient => new AtpClient();

	public IJVQuestionClient JVQuestion => new JVQuestionClient();

	public ITimeSheetingClient TimeSheetingClient => new TimeSheetingClient();

	public IWorkPeriodClient WorkPeriod => new WorkPeriodClient();

	public IImportData ImportDataClient => new ImportDataClient();

	public ISalaryInfoClient SalaryInfoClient => new SalaryInfoClient();

	public IPenalType PenalType => new PenalType();

	public IEmpPenalization EmpPenalization => new EmpPenalization();

	public ILeaveParameterClient LeaveParameterClient => new LeaveParameterClient();

	public IDeductionTypeClient DeductionType => new DeductionTypeClient();

	public ISalaryParametersClient SalaryParameters => new SalaryParametersClient();

	public IAllowanceType AllowanceType => new AllowanceType();

	public IVehicleClient Vehicle => new VehicleClient();

	public IVehicleTypeClient VehicleType => new VehicleTypeClient();

	public IMaterialShortageReportClient MaterialShortageReportClient => new MaterialShortageReportClient();

	public IQueriesClient Queries => new QueriesClient();

	public ISalaryInfoAllowanceClient SalaryInfoAllowanceClient => new SalaryInfoAllowanceClient();

	public ISalaryInfoAdvanceClient SalaryInfoAdvanceClient => new SalaryInfoAdvanceClient();

	public IPickingClient Picking => new PickingClient();

	public ISalaryInfoMainByEmpClient SalaryInfoMainByEmpClient => new SalaryInfoMainByEmpClient();

	public ISalaryAllowanceByEmpClient SalaryAllowanceByEmpClient => new SalaryAllowanceByEmpClient();

	public ILandedCostsSetupCLient LandedCostsSetupCLient => new LandedCostsSetupCLient();

	public ILandedCostsClient LandedCosts => new LandedCostsClient();

	public IPickListsClient PickLists => new PickListsClient();

	public ISalaryCalculationClient SalaryCalculationClient => new SalaryCalculationClient();

	public ISalaryFormula SalaryFormula => new SalaryFormulaClient();

	public IExRateAndIndexClient ExRateAndIndex => new ExRateAndIndexClient();

	public IORWClient ORW => new ORWClient();

	public IProductionOrderImportTool ProductionOrderImportTool => new ProductionOrderImportToolClient();

	public ISolutionClient Solution => new SolutionClient();

	public IPackingClient Packing => new PackingClient();

	public IOpportunityClient OpportunityClient => new OpportunityClientHelper();

	public IShippingClient Shipping => new ShippingClient();

	public IActivityClient ActivityClient => new ActivityClient();

	public IPluginManagerClient PluginManagerClient => new PluginManagerClient();

	public IOutsourcingClient OutsourcingClient => new OutsourcingClient();

	public IFileNetworkManagerClient FileNetworkManagerClient => new FileNetworkManagerClient();

	public IDefineNewClient DefineNewClient => new DefineNewClient();

	public ICategoriesDABClient CategoriesDABClient => new CategoriesDABClient();

	public IDashboardClient DashboardClient => new DashboardClient();

	public IDashboardDetailClient DashboardDetail => new DashboardDetailClient();

	public IActivityCalendarClient ActivityCalendarClient => new ActivityCalendarClient();

	public ICashItemMasterDataClient CashItemMasterData => new CashItemMasterDataClient();

	public IVoucherMasterDataClient VoucherMasterData => new VoucherMasterDataClient();

	public ICouponMasterDataClient CouponMasterData => new CouponMasterDataClient();

	public ITerritoryClient Territory => new TerritoryClient();

	public ICalendarSettingClient CalendarSettingClient => new CalendarSettingClient();

	public ISubActivitiesClient SubActivities => new SubActivitiesClient();

	public ITrackingIssueClient TrackingIssue => new TrackingIssueClient();

	public IUserDefinedTextsClient UserDefinedTexts => new UserDefinedTextsClient();

	public IUdrClient Udr => new UdrClient();

	public IIndustryClient Industry => new IndustryClient();

	public IFormSettingClient FormSettingClient => new FormSettingClient();

	public IContractTemplateClient ContractTemplateClient => new ContractTemplateClient();

	public IProjectManagerClient ProjectManager => new ProjectManagerClient();

	public IProjectReportCompletionClient ReportCompletion => new ProjectReportCompletionClient();

	public IProjectConfirmClient ProjectConfirm => new ProjectConfirmClient();

	public IDocumentNumberingClient DocumentNumering => new DocumentNumberingClient();

	public ITaxGroupsClient TaxGroupsClient => new TaxGroupsClient();

	public IContextActionClient ContextAction => new ContextActionClient();

	public IInventoryTransferRequestClient InventoryTransferRequest => new InventoryTransferRequestClient();

	public IGoodsReceiptClient GoodsReceipt => new GoodsReceiptClient();

	public IGoodsIssueClient GoodsIssue => new GoodsIssueClient();

	public IWidgetClient Widget => new WidgetClient();

	public IMailSettingClient MailSettingClient => new MailSettingClient();

	public INotificationSettingClient NotificationSettingClient => new NotificationSettingClient();

	public ILeaveInformationClient LeaveInformationClient => new LeaveInformationClient();

	public ISearchSettingClient SearchSetting => new SearchSettingClient();

	public IBlanketAgreement BlanketAgreement => new BlanketAgreementClient();

	public IInPaymentClient InPaymentClient => new InPaymentClient();

	public IOutPaymentClient OutPaymentClient => new OutPaymentClient();

	public IIncomingPaymentsClient IncomingPaymentsClient => new IncomingPaymentsClient();

	public IOutgoingPaymentsClient OutgoingPaymentsClient => new OutgoingPaymentsClient();

	public IPaymentReconClient PaymentReconClient => new PaymentReconClient();

	public IUdtClient UdtClient => new UdtClient();

	public IWarrantyCardClient WarrantyCard => new WarrantyCardClient();

	public IServiceLevelAgreementClient ServiceLevelAgreement => new ServiceLevelAgreementClient();

	public IFormFilterSettingClient FormFilterSettingClient => new FormFilterSettingClient();

	public IResourceWorkCenterClient ResourceWorkCenter => new ResourceWorkCenterClient();

	public IFormWizardClient FormWizard => new FormWizardClient();

	public ICapacityClient Capacity => new CapacityClient();

	public IServiceCallsClient ServiceCalls => new ServiceCallsClient();

	public IServiceContractTemplateClient ServiceContractTemplateClient => new ServiceContractTemplateClient();

	public IOperationsClient OpearationsClient => new OperationsClient();

	public IBinLocMasterDataClient BinLocMasterDataClient => new BinLocMasterDataClient();

	public IPurchaseRequestControlClient PurchaseRequestControl => new PurchaseRequestControlClient();

	public IAdvanceRoutingClient AdvanceRoutingClient => new AdvanceRoutingClient();

	public IMarketingCampaignClient MarketingCampaign => new MarketingCampaignClient();

	public IAlternativeItemsClient AlternativeItemsClient => new AlternativeItemsClient();

	public IJournalEntryClient JournalEntry => new JournalEntryClient();

	public IWorkingTableClient WorkingTable => new WorkingTableClient();

	public IChangeLogClient ChangeLogClient => new ChangeLogClient();

	public IResourceGroupClient ResourceGroupClient => new ResourceGroupClient();

	public IExternalRoutingClient ExternalRoutingClient => new ExternalRoutingClient();

	public IResourceWorkScheduleClient ResourceWorkScheduleClient => new ResourceWorkScheduleClient();

	public ISMSClient SmsClient => new SMSClient();

	public IWBSTemplateClient WBSTemplate => new WBSTemplateClient();

	public IPublishServiceClient PublishService => new PublishServiceClient();

	public IChartOfAccountClient ChartOfAccountClient => new ChartOfAccountClient();

	public IDistributeRuleClient DistributeRule => new DistributeRuleClient();

	public IMenuStructureClient MenuStructure => new MenuStructureClient();

	public ICapacitySettingClient CapacitySettingClient => new CapacitySettingClient();

	public IHolidaySetupClient HolidaySetupClient => new HolidaySetupClient();

	public IInOutMethodClient InOutMethodClient => new InOutMethodClient();

	public IWorkShiftClient WorkShiftClient => new WorkShiftClient();

	public IWorkScheduleSetupClient WorkScheduleSetupClient => new WorkScheduleSetupClient();

	public IWorkScheduleRegisterClient WorkScheduleRegisterClient => new WorkScheduleRegisterClient();

	public IDocumentSettingClient DocumentSettingClient => new DocumentSettingClient();

	public IBarcodeInventoryCountingClient BarcodeInventoryCountingClient => new BarcodeInventoryCountingClient();

	public IDocumentDraftClient DocumentDraftClient => new DocumentDraftClient();

	public IJobsManagementClient JobsManagementClient => new JobsManagementClient();

	public IUnitOfMeasureGroupsClient UnitOfMeasureGroupsClient => new UnitOfMeasureGroupsClient();

	public IAdditionalGeneralSettingsClient AdditionalGeneralSettingsClient => new AdditionalGeneralSettingsClient();

	public IGeographyClient GeographyClient => new GeographyClient();

	public IStoreClient StoreClient => new StoreClient();

	public ISaleTerritoryClient SaleTerritoryClient => new SaleTerritoryClient();

	public IWorkingProcessClient WorkingProcessClient => new WorkingProcessClient();

	public IPaymentWizardClient PaymentWizardClient => new PaymentWizardClient();

	public IFacebookManagerClient FacebookManagerClient => new FacebookManagerClient();

	public IGoodsReceiptPOByBarcodeClient GoodsReceiptPOByBarcodeClient => new GoodsReceiptPOByBarcodeClient();

	public IQuestionMasterDataClient QuestionMasterDataClient => new QuestionMasterDataClient();

	public IBinLocationFieldClient BinLocationFieldClient => new BinLocationFieldClient();

	public IBinLocationAttributeClient BinLocationAttributeClient => new BinLocationAttributeClient();

	public IWarehouseSublevelClient WarehouseSublevelClient => new WarehouseSublevelClient();

	public IOutletVisitingFrequencyClient OutletVisitingFrequencyClient => new OutletVisitingFrequencyClient();

	public IPromotionSetupClient PromotionSetupClient => new PromotionSetupClient();

	public ISettingReportClient SettingReportClient => new SettingReportClient();

	public IChannelClient ChannelClient => new ChannelClient();

	public ISalesRouteClient SalesRouteClient => new SalesRouteClient();

	public IPromotionConditionClient PromotionCondition => new PromotionConditionClient();

	public IOutletClient Outlet => new OutletClient();

	public IPromotionClient Promotion => new PromotionClient();

	public IDMSReportClient DMSReport => new DMSReportClient();

	public IKPIsMaterDataClient KPIsMaterDataClient => new KPIsMaterDataClient();

	public ISetIndicatorKPIsClient SetIndicatorKPIsClient => new SetIndicatorKPIsClient();

	public IKPIsSettingClient KPIsSettingClient => new KPIsSettingClient();

	public IListPointKPIsClient ListPointKPIsClient => new ListPointKPIsClient();

	public IPromotionBenefitClient PromotionBenefit => new PromotionBenefitClient();

	public IApprovalStages ApprovalStages => new ApprovalStagesClient();

	public IApprovalTemplates ApprovalTemplates => new ApprovalTemplatesClient();

	public IImageAlbumClient ImageAlbumClient => new ImageAlbumClient();

	public IUploadImageClient UploadImageClient => new UploadImageClient();

	public IMarkerClient MarkerClient => new MarkerClient();

	public IAlertTemplate AlertTemplate => new AlertTemplateClient();

	public IDMSDeliveryClient DMSDeliveryClient => new DMSDeliveryClient();

	public IGanttControlClient GanttControl => new GanttControlClient();

	public IPaymentsClient Payments => new PaymentsClient();

	public IThirdPartyTokenClient ThirdPartyToken => new ThirdPartyTokenClient();

	public IKPIPlanClient KPIPlan => new KPIPlanClient();

	public IKPIScopeClient KPIScope => new KPIScopeClient();

	public IKPIIndicatorClient KPIIndicator => new KPIIndicatorClient();

	public IKPIGroupClient KPIGroup => new KPIGroupClient();

	public IKPISetupClient KPISetup => new KPISetupClient();

	public IKPIScoreClient KPIScore => new KPIScoreClient();

	public IInternalReconciliationsClient InternalReconciliationsClient => new InternalReconciliationsClient();

	public IPromotionProgramClient PromotionProgram => new PromotionProgramClient();

	public ISettingApprovalDetailClient SettingApprovalDetail => new SettingApprovalDetailClient();

	public IAboutRangeTimeClient AboutRangeTimeClient => new AboutRangeTimeClient();

	public ICommentsClient Comments => new CommentsClient();

	public IDMSSettingUDFClient DMSSettingUDF => new DMSSettingUDFClient();

	public HttpClientRepository()
	{
	}

	public HttpClientRepository(ApzObjectTypes type)
	{
		_type = type;
	}
}
