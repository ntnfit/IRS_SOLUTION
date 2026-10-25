namespace Apzon.HttpClientHelper;

public interface IHttpClient
{
	ICommonClient Commons { get; }

	IUdoClient Udo { get; }

	IInventoryCountingBinClient InventoryCountingBin { get; }

	IInventoryCountingRequestClient InventoryCountingRequest { get; }

	IInventoryCountingClient InventoryCounting { get; }

	IStockCountingClient StockCounting { get; }

	IStockTransferClient StockTransfer { get; }

	IIrsConfigClient IrsConfig { get; }

	IUdfClient Udf { get; }

	IOwnerAuthorizeClient OwnerAuthorize { get; }

	ICflClient Cfl { get; }

	IProductionOrdersClient ProductionOrders { get; }

	IBillOfMasterialsClient BillOfMasterials { get; }

	IDocumentsClient Documents { get; }

	IFilterSettingClient FilterSetting { get; }

	IPackingBatch PackingBatch { get; }

	IUserClient User { get; }

	IItems Items { get; }

	IIncommingPayments IncommingPayments { get; }

	IMrpClient Mrp { get; }

	IProductionScheduleClient ProductionSchedule { get; }

	IBusinessMasterClient BusinessMaster { get; }

	IReceiptIssueBin ReceiptIssueBin { get; }

	IDiagram Diagram { get; }

	IFormula Formula { get; }

	IWarehouseClient Warehouse { get; }

	IInventoryPostingListClient InventoryPostingList { get; }

	ISalesEmployeeBuyerClient SalesEmployeeBuyerClient { get; }

	ICostCenters CostCentersClient { get; }

	ILoginClient LoginClient { get; }

	IRetailChooseFromList RetailChooseFromList { get; }

	IRetailClient Retail { get; }

	IRetailStoreClient RetailStore { get; }

	IPosMasterClient PosMasterClient { get; }

	IGeneralSetting GeneralSetting { get; }

	IBatchDetails BatchDetails { get; }

	ISerialDetails SerialDetails { get; }

	IAdvanceForecastClient AdvanceForecastClient { get; }

	IScheduleClient ScheduleClient { get; }

	IInventoryTransferClient InventoryTransferClient { get; }

	IInventoryTransferRequestClient InventoryTransferRequest { get; }

	IInventoryCyclesClient InventoryCycles { get; }

	ICycleCountDeterminationClient CycleCountDetermination { get; }

	ICycleCountRecommendationClient CycleCountRecommendation { get; }

	ICycleCDSubTableClient CycleCDSubTable { get; }

	IEmployeeMasterClient EmployeeMasterClient { get; }

	IEmpRelativesClient EmpRelativesClient { get; }

	IJobVacancyClient JobVacancy { get; }

	IXtraReportManagerClient XtraReportManagerClient { get; }

	IBusinessPartnerCatalogNumbersClient BusinessPartnerCatalogNumbersClient { get; }

	IJVQuestionClient JVQuestion { get; }

	IPaymentTermSetup PaymentTermSetup { get; }

	IPriceListSetup PriceListSetup { get; }

	IAtpClient AtpClient { get; }

	ITimeSheetingClient TimeSheetingClient { get; }

	ILeaveInformationClient LeaveInformationClient { get; }

	IWorkPeriodClient WorkPeriod { get; }

	IImportData ImportDataClient { get; }

	ISalaryInfoClient SalaryInfoClient { get; }

	ISalaryInfoMainByEmpClient SalaryInfoMainByEmpClient { get; }

	ISalaryAllowanceByEmpClient SalaryAllowanceByEmpClient { get; }

	ISalaryInfoAllowanceClient SalaryInfoAllowanceClient { get; }

	ISalaryInfoAdvanceClient SalaryInfoAdvanceClient { get; }

	IPenalType PenalType { get; }

	IEmpPenalization EmpPenalization { get; }

	ILeaveParameterClient LeaveParameterClient { get; }

	IDeductionTypeClient DeductionType { get; }

	ISalaryParametersClient SalaryParameters { get; }

	IMaterialShortageReportClient MaterialShortageReportClient { get; }

	IAllowanceType AllowanceType { get; }

	IVehicleClient Vehicle { get; }

	IVehicleTypeClient VehicleType { get; }

	IQueriesClient Queries { get; }

	IPickingClient Picking { get; }

	ILandedCostsSetupCLient LandedCostsSetupCLient { get; }

	ILandedCostsClient LandedCosts { get; }

	IPickListsClient PickLists { get; }

	ISalaryCalculationClient SalaryCalculationClient { get; }

	ISalaryFormula SalaryFormula { get; }

	IExRateAndIndexClient ExRateAndIndex { get; }

	IORWClient ORW { get; }

	IProductionOrderImportTool ProductionOrderImportTool { get; }

	ISolutionClient Solution { get; }

	IPackingClient Packing { get; }

	IShippingClient Shipping { get; }

	IOpportunityClient OpportunityClient { get; }

	IActivityClient ActivityClient { get; }

	IPluginManagerClient PluginManagerClient { get; }

	IOutsourcingClient OutsourcingClient { get; }

	IFileNetworkManagerClient FileNetworkManagerClient { get; }

	IDefineNewClient DefineNewClient { get; }

	ICategoriesDABClient CategoriesDABClient { get; }

	IDashboardClient DashboardClient { get; }

	IDashboardDetailClient DashboardDetail { get; }

	IActivityCalendarClient ActivityCalendarClient { get; }

	ICashItemMasterDataClient CashItemMasterData { get; }

	IVoucherMasterDataClient VoucherMasterData { get; }

	ICouponMasterDataClient CouponMasterData { get; }

	ITerritoryClient Territory { get; }

	INotificationSettingClient NotificationSettingClient { get; }

	ICalendarSettingClient CalendarSettingClient { get; }

	IMailSettingClient MailSettingClient { get; }

	ISubActivitiesClient SubActivities { get; }

	ITrackingIssueClient TrackingIssue { get; }

	IUserDefinedTextsClient UserDefinedTexts { get; }

	IUdrClient Udr { get; }

	IIndustryClient Industry { get; }

	IFormSettingClient FormSettingClient { get; }

	IContractTemplateClient ContractTemplateClient { get; }

	IProjectManagerClient ProjectManager { get; }

	IProjectReportCompletionClient ReportCompletion { get; }

	IProjectConfirmClient ProjectConfirm { get; }

	IDocumentNumberingClient DocumentNumering { get; }

	ITaxGroupsClient TaxGroupsClient { get; }

	IContextActionClient ContextAction { get; }

	IGoodsReceiptClient GoodsReceipt { get; }

	IGoodsIssueClient GoodsIssue { get; }

	IWidgetClient Widget { get; }

	ISearchSettingClient SearchSetting { get; }

	IBlanketAgreement BlanketAgreement { get; }

	IInPaymentClient InPaymentClient { get; }

	IOutPaymentClient OutPaymentClient { get; }

	IIncomingPaymentsClient IncomingPaymentsClient { get; }

	IOutgoingPaymentsClient OutgoingPaymentsClient { get; }

	IPaymentReconClient PaymentReconClient { get; }

	IUdtClient UdtClient { get; }

	IFormFilterSettingClient FormFilterSettingClient { get; }

	IFormWizardClient FormWizard { get; }

	IResourceWorkCenterClient ResourceWorkCenter { get; }

	IWarrantyCardClient WarrantyCard { get; }

	IServiceLevelAgreementClient ServiceLevelAgreement { get; }

	ICapacityClient Capacity { get; }

	IServiceCallsClient ServiceCalls { get; }

	IServiceContractTemplateClient ServiceContractTemplateClient { get; }

	IOperationsClient OpearationsClient { get; }

	IPurchaseRequestControlClient PurchaseRequestControl { get; }

	IAdvanceRoutingClient AdvanceRoutingClient { get; }

	IBinLocMasterDataClient BinLocMasterDataClient { get; }

	IMarketingCampaignClient MarketingCampaign { get; }

	IAlternativeItemsClient AlternativeItemsClient { get; }

	IJournalEntryClient JournalEntry { get; }

	IWorkingTableClient WorkingTable { get; }

	IChangeLogClient ChangeLogClient { get; }

	IResourceGroupClient ResourceGroupClient { get; }

	IExternalRoutingClient ExternalRoutingClient { get; }

	IResourceWorkScheduleClient ResourceWorkScheduleClient { get; }

	ISMSClient SmsClient { get; }

	IWBSTemplateClient WBSTemplate { get; }

	IPublishServiceClient PublishService { get; }

	IChartOfAccountClient ChartOfAccountClient { get; }

	IDistributeRuleClient DistributeRule { get; }

	IMenuStructureClient MenuStructure { get; }

	ICapacitySettingClient CapacitySettingClient { get; }

	IHolidaySetupClient HolidaySetupClient { get; }

	IInOutMethodClient InOutMethodClient { get; }

	IWorkShiftClient WorkShiftClient { get; }

	IWorkScheduleSetupClient WorkScheduleSetupClient { get; }

	IWorkScheduleRegisterClient WorkScheduleRegisterClient { get; }

	IDocumentSettingClient DocumentSettingClient { get; }

	IBarcodeInventoryCountingClient BarcodeInventoryCountingClient { get; }

	IDocumentDraftClient DocumentDraftClient { get; }

	IJobsManagementClient JobsManagementClient { get; }

	IUnitOfMeasureGroupsClient UnitOfMeasureGroupsClient { get; }

	IAdditionalGeneralSettingsClient AdditionalGeneralSettingsClient { get; }

	IGeographyClient GeographyClient { get; }

	IStoreClient StoreClient { get; }

	ISaleTerritoryClient SaleTerritoryClient { get; }

	IWorkingProcessClient WorkingProcessClient { get; }

	IPaymentWizardClient PaymentWizardClient { get; }

	IFacebookManagerClient FacebookManagerClient { get; }

	IGoodsReceiptPOByBarcodeClient GoodsReceiptPOByBarcodeClient { get; }

	IQuestionMasterDataClient QuestionMasterDataClient { get; }

	IOutletVisitingFrequencyClient OutletVisitingFrequencyClient { get; }

	IBinLocationFieldClient BinLocationFieldClient { get; }

	IBinLocationAttributeClient BinLocationAttributeClient { get; }

	IWarehouseSublevelClient WarehouseSublevelClient { get; }

	IPromotionSetupClient PromotionSetupClient { get; }

	ISettingReportClient SettingReportClient { get; }

	IChannelClient ChannelClient { get; }

	ISalesRouteClient SalesRouteClient { get; }

	IPromotionConditionClient PromotionCondition { get; }

	IOutletClient Outlet { get; }

	IPromotionClient Promotion { get; }

	IDMSReportClient DMSReport { get; }

	IPromotionBenefitClient PromotionBenefit { get; }

	IKPIsMaterDataClient KPIsMaterDataClient { get; }

	ISetIndicatorKPIsClient SetIndicatorKPIsClient { get; }

	IApprovalStages ApprovalStages { get; }

	IApprovalTemplates ApprovalTemplates { get; }

	IKPIsSettingClient KPIsSettingClient { get; }

	IListPointKPIsClient ListPointKPIsClient { get; }

	IImageAlbumClient ImageAlbumClient { get; }

	IUploadImageClient UploadImageClient { get; }

	IMarkerClient MarkerClient { get; }

	IAlertTemplate AlertTemplate { get; }

	IDMSDeliveryClient DMSDeliveryClient { get; }

	IGanttControlClient GanttControl { get; }

	IPaymentsClient Payments { get; }

	IThirdPartyTokenClient ThirdPartyToken { get; }

	IKPIPlanClient KPIPlan { get; }

	IKPIScopeClient KPIScope { get; }

	IKPIIndicatorClient KPIIndicator { get; }

	IKPIGroupClient KPIGroup { get; }

	IKPISetupClient KPISetup { get; }

	IKPIScoreClient KPIScore { get; }

	IInternalReconciliationsClient InternalReconciliationsClient { get; }

	IPromotionProgramClient PromotionProgram { get; }

	ISettingApprovalDetailClient SettingApprovalDetail { get; }

	IAboutRangeTimeClient AboutRangeTimeClient { get; }

	ICommentsClient Comments { get; }

	IDMSSettingUDFClient DMSSettingUDF { get; }
}
