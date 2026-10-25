using System.Collections.Generic;
using Apzon.Commons.Enums;

namespace Apzon.Commons.Constants;

public class FormMessage
{
	public static string MsgSaveChange = "Do you want to save the changes?";

	public static string MsgTitleSystemMessage = "System Message";

	public static Dictionary<Apzon.Commons.Enums.Enums.MessageCode, string> DefaultMessageCenter = new Dictionary<Apzon.Commons.Enums.Enums.MessageCode, string>
	{
		{
			Apzon.Commons.Enums.Enums.MessageCode.Add,
			"Add"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Error,
			"Error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Warning,
			"Warning"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Info,
			"Information"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.System,
			"System"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Yes,
			"Yes"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.No,
			"No"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Cancel,
			"Cancel"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Success,
			"Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Update,
			"Update"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Ok,
			"Ok"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Save,
			"Save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSave,
			"Do you want to save?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdate,
			"Do you want to save change?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancel,
			"Do you want to Cancel?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SystemMessage,
			"System message"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Abort,
			"Abort"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Ignore,
			"Ignore"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Retry,
			"Retry"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoRestartSystem,
			"You need to restart System"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmExit,
			"Do you want to exit?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmClose,
			"Do you want to Close?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDelete,
			"Do you want to Delete?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmContinueNotSave,
			"Unsaved data will be lost. Do you want to continue without saving?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveItemReserve,
			"Removal of an item cannot be reversed. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeInventoryUomItem,
			"Changing inventory UoM removes all UoM prices defined for the item. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeUomGroupItem,
			"Changing UoM group removes all associated sales UoM, purchasing UoM, package\r\ntypes, and UoM prices for the item. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeItemGroup,
			"Do you want to update the item information according to the selected item group?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCloseForm,
			"Do you want to close this form?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmApplyAllFormatConditionForUser,
			"Do you want to apply ALL format condition rule expression for selected users?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmApplyAllFormsettingForUser,
			"Do you want to apply ALL form setting for selected users?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmThisFormsetingForUser,
			"Do you want to apply this form setting for selected users?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSaveBeforeClose,
			"Unsaved data will be lost. Do you want to save changes before close this form?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteActivityWhenExistsOp,
			"This activity is connected to an opportunity or service call; if deleted, it cannot be \r\nrestored. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteActivity,
			"If you remove an Activity, it cannot be restored. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotOpenFile,
			"The file could not be opened.\r\nPath: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotSaveFile,
			"The file could not be saved.\r\nPath: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRestoreFormsettingAllUser,
			"Do you want to restore form setting for all user?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRestoreFormsettingCurrentUser,
			"Do you want to restore form setting for current user?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveLinkDocument,
			"The link to the document will be removed. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveLinkDocuemntDraft,
			"The link to the document draft will be removed. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRenameDestFileName,
			"Destination file exists, do you want to rename upload file?\r\nPath: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyAppointment,
			"Do you want to copy this Appointment?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyAppointmentFrom,
			"Do you want to copy this Appointment from {0} to {1}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoSubjectCanNotEmpty,
			"Subject cannot be empty!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChange,
			"Do you want to change?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoCannotLoadData,
			"Cannot load data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmContinuteBatchExists,
			"The Batch {0} already exists. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteSelectedRow,
			"Do you want to delete selected row?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveCampaign,
			"Do you want to remove - Campaign .No {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveWarranty,
			"Do you want to remove - Warranty .No: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveByNo,
			"Do you want to remove - No: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveSla,
			"Do you want to remove - SLA ID: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeStatusTerminated,
			"Do you want to change status to terminated. All related warranty cards will be updated to inactive status?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventManagementConfirmSave,
			"All events in the recurrence will be changed; modified events will be disassociated from this recurrence. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventManagementConfirmDeleteWithRecurrences,
			"Do you want to delete this record with its all recurrences?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoSendMailFailedWithReason,
			"Send mail failed! Reason: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoFillEmailForReceieve,
			"You must fill email Address for receivers!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventManagementConfirmReceiveNotEmail,
			"Some receivers don't have email address. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSendMail,
			"Are you sure you want to send this mail?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoSendMailSuccess,
			"Send mail successfully!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoNotSelectedTemplateSendMailSelectTemp,
			"You have not selected any templates. Send mail failed!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ContractConfirmChangeService,
			"Changing the service contract type may cause data loss. Do you still want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ContractInfoChangingService,
			"Changing the service contract type may cause data loss"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoWaitApproval,
			"This document was wait for approval!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveItemById,
			"Do you want to remove this item: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateBpProject2Line,
			"Do you want to update the BP project in the lines accordingly?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SalesBlanketConfirmChangeMethod,
			"Data in the Details tab and Row Details will be deleted. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCloseItem,
			"Do you want to close this item?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveItem,
			"Do you want to remove this item?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoDocumentNotSelect,
			"One or more documents have not been selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoConfirmPassToHandOver,
			"Need to confirm the password to hand over!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoIncorrectEmployeeCode,
			"Incorrect employee code!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoBranchNotSelect,
			"One or more batches have not been selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoTicketPrinted,
			"The ticket has been printed!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRun,
			"Do you want to Run?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreatePurchaseRepuest,
			"Do you want create purchase request?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateTransferRequest,
			"Do you want create inventory transfer request?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveActivitySubject,
			"Removing an Activity Subject is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateAmountAndProfit,
			"Do you want update Amount and Gross Profit?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveSalesOpportunity,
			"Removing an opportunity is irreversible. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteThisRow,
			"Do you want to delete this row?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRefreshData,
			"Do you want to refresh data?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteChildrenItem,
			"Do you want to delete children item?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeCalendar,
			"Do you want to change the calendar?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeFromDate,
			"Do you want to update the From Date according to the selected Line Selected?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeToDate,
			"Do you want to update the To Date according to the selected Line Selected?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PointOfSalesConfirmDeleteItem,
			"This action need Manage Overide! Are you sure you want to delete this item?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoNoRowSelected,
			"No row is selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductionOrderConfirmUpdateEndDate,
			"Do you want to update the existing table rows with a new end date?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductionOrderConfirmUpdateComponent,
			"Components will be updated accordingly; continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductionOrderConfirmUpdateStartAndEndDate,
			"Do you want to update existing table rows with a new start date and end date?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdatePriceListInRow,
			"Do you want to update existing rows with a Price List?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateProjectInRow,
			"Do you want to update existing rows with a Project Code?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveByCode,
			"Do you want to remove - Code: {0}? After removing will not undo "
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WbsTemplateConfirmUpdateDataForTask,
			"Do you want to update the {0} value for existing Tasks with this new?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveByDocEntry,
			"Do you want to remove - DocEntry: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProjectManagerConfirmDelete,
			"{0} is associated with the Budget Item you want to delete?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProjectManagerConfirmChange,
			"Task Template Code : {0} have gen task.Do you want change?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProjectManagerConfirmGenTaskWhenGenerated,
			"Task Template Code : {0} have generate task. Would you like to continue generate task?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmGenTask,
			"Do you want to generate task?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PaymentForInvoiceConfirmPostAmount,
			"Do you want to post this amount: VND {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateProductionPlan,
			"Do you want to save and creat production plan?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRefreshDataReceiptTab,
			"Do you want to refresh data in Tab Receipt?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUnReleaseProductionOrder,
			"Do you want to UnRelease this production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelProductionOrder,
			"Do you want to Cancel this production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateDocument,
			"Do you want to create document?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateInventoryTransfer,
			"Do you want to create Inventory Transfer?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeSourceType,
			"Do you want to change source type?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ForecastConfirmChanging,
			"Changing the forecast view will erase quantities for the current forecast; continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReportCompletePo,
			"Do you want to report complete these production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmApplyToAllLine,
			"Do you want to apply to all lines?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateNewVersion,
			"There's a new version avaiable. Do you want to update now?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoDownloadFail,
			"Downloaded Fail"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoDownloadSuccess,
			"Downloaded Success. Exit to install new version"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeUser,
			"Any unsaved changes will be lost. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PriceListConfirmInitForPriceList,
			"Do you want to initialize currency (correct invalid currency to local currency) for price list?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PriceListConfirmUpdate,
			"Selected items prices in price list {0} will be updated according to the base price \r\nlist {1} and factor {2}. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReplaceBatchSerial,
			"Batch/serial numbers already choose. Do you want to replace all?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelItem,
			"Do you want to cancel this item?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoEnterBpBeforeEnterItem,
			"You must enter BP before choose item!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoEnterItemBeforeEnterBp,
			"You must choose item before choose Bussiness Partners"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateDataToExistsLine,
			"Do you want to update the {0} value for existing lines with this new?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateProductionOrder,
			"Do you want to create these production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReleaseProductionOrder,
			"Do you want to realese these production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelSelectProductionOrder,
			"Do you want to cancel these production order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSaveDraft,
			"Do you want to save as draft?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSaveDocument,
			"Do you want to save this document?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCustomDataLine,
			"Do you want to custom data line?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateFromWhsToLine,
			"Do you want to update the \"From Warehouse\" value for existing lines with this new?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateToWhsToLine,
			"Do you want to update the \"To Warehouse\" value for existing lines with this new?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelDocument,
			"Do you want to cancel this document?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangePaymentCount,
			"Do you want to change payment's count?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCloseDocumentByDocNum,
			"Do you want to Close - DocNum: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelDocumentByDocNum,
			"Do you want to Cancel - DocNum: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveByDocNum,
			"Do you want to Remove - DocNum: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreatePurchaseOrder,
			"Do you want to create Purchase Order?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmStartImportData,
			"Do you want to start import data?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelImportProcess,
			"Cancel the import process?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmClosePeriod,
			"Do you want to close period [{0}] from branch [{1}]?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCancelTimeSheeting,
			"Do you want to cancel timesheeting(include Overtime TimeSheet) of period [{0}]? This action is irreversible"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteRecordWithIrrever,
			"This action is irreversible! Do you really want to delete these records?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSenSalarySlipViaEmail,
			"Employee's Slip will be sent via Email. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoSendEmailProcessSuccess,
			"Send mails process completed!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyFormSettingBetweenFunction,
			"Do you want to copy formsetting from {0} to {1}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeApplicant,
			"ApplicantNo: {0} exist in tab InterView.Do you want to change?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteEmployee,
			"Do you want to delete this employee?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateDeliveryDate2Line,
			"Do you want to update the existing table rows with the new delivery Date?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateSalesEmployee2Line,
			"Do you want to update the existing table rows with the new sales employee?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUpdateBpData2Line,
			"Update document rows according to new BP's data?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmAddInventoryRevaluation,
			"Do you want to Add Inventory Revaluation?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveCostCenter,
			"Do you want to remove this Cost Center?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReActivePage,
			"This page is already actived, do you want to re-active?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveMettingLocation,
			"Removing an Meeting Location is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveActivitySataus,
			"Removing an Activity Status is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveDashboard,
			"Do you want to remove this dashboard?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveCatalogAndDashboard,
			"Do you want to remove this category and all of its dashboard?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteItemWhenExistsChild,
			"This item has child items do you want to delete its child items?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteObjectAndTable,
			"Do you want to delete the object and remove its tables?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmUnregisterObject,
			"Do you want to unregister the object?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmAddUdf,
			"The database structure has been modified. In order to resume the process, all open windows will be closed. Do you want to continue adding the user-defined field?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemovePublishService,
			"Removal of a published service cannot be reversed. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveActivityType,
			"Removing an Activity Type is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteCatalogAndQuery,
			"Delete a category will delete all its queries. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReplacePriceListAndEffective,
			"Do you want to replace the price list and effective discount according to the selected business partner group? Field: Group"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmApplyPaymentTerm2Bp,
			"Do you want to apply the associated payment terms to the business partner?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeCurrencyForBpLinkDocument,
			"You cannot change the currency for a customer-type business partner linked to Sales Order"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveContactPerson,
			"Removing a contact person is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveBp,
			"Removal of a business partner cannot be reversed. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteFolder,
			"Do you want to delete this folder?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmEnableNegativeInventoryUsage,
			"Negative inventory usage is not in accordance with accounting rules. Are you sure you want to enable negative inventory usage?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSettingTerritory,
			"Setting territory to \"inactive\" will also set all its sub-territories to \"inactive\".Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteTerritory,
			"Deleting territory will also delete all its sub-territories.Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmLogoutAllUser,
			"All sessions will be logout. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveWhs,
			"Removing a warehouse is irreversible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteGroup,
			"Do you want to delete this Group?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoQueryCondition,
			"Paramaters of the form [%0], [%1]......[%n]....\r\nif paramater is current user is form of that paramater be @CURRENTUSER \r\nif paramater is current branch is form of that paramater be @CURRENTBRANCH\r\nExample: userid = [%1]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Conditions,
			"Conditions"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InfoConnectSmtpSuccess,
			"Connect to SMTP server successfully!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSaveMailSetting,
			"This setting will be applied to whole system. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveProject,
			"Removal of a project cannot be reversed.Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyTaskWithSubTask,
			"Do you want to copy this Task (Will copy this with its sub Tasks to TODAY View)?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyThisTask,
			"Do you want to copy this Task (with its sub Tasks)?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyThisTaskWithTime,
			"Do you want to copy this Task (with its sub Tasks) from {0} to {1}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmChangeOccurence,
			"Do you want to change this occurence?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteUser,
			"You are about to delete this user; adding a new user with the same user code is not possible. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmSaveJobManagement,
			"Do you want to save (that will update and run the job)?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRestoreThisRecord,
			"Do you want to restore this record?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ExceptionMessage,
			"Exception"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UnknownMessage,
			"Unknown message"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EndDateGreaterStartDate,
			"End Date must be lagrer than Start Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouHaveNotChooseDocument,
			"You have not choose document"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustInputItemCode,
			"You must input Item Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InputLength50,
			"Input characters not exceeding 50"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustInputItemName,
			"You must input Item Name"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeExists,
			"Item code '{0}' already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterQtyGreater0,
			"Enter quantity greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToDateMustGreaterFromDate,
			"To Date must be greater than From Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveSuccess,
			"Save success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CancelSoSuccess,
			"{0} Cancel SO success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CloseSoSuccess,
			"{0} Close SO success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CancelSuccess,
			"Cancel success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CloseSuccess,
			"Close success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DeleteSuccess,
			"Delete success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AfterNotNull,
			"After not null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidNumericValue,
			"Invalid numeric value"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TelephoneNotExceeding20,
			"TelePhone not exceeding 20 numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TelephoneLeast10,
			"Telephone must be at least 10 numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AddCurrentActivityBeforeAddFollowActivity,
			"Add the current activity before adding a follow-up activity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RemoveSuccess,
			"Remove success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UpdateSuccess,
			"Update Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MrpCodeNotNull,
			"MpsCode is not null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CreateDateGreaterNow,
			"CreateDate must be greater than or equal  Now"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateGreaterToday,
			"From Date must be greater than ToDay"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FixDateGreaterFromDate,
			"Fix date must be greater than From Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveSuccessWithDocEntry,
			"Save success  - DocEntry: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseItemCodeAndCardCodeInGeneral,
			"Please choose ItemCode and CardCode in General table"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotConnect2Server,
			"Cannot connect to server"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NeedChoseDocumentDuplicate,
			"You need choose document to duplicate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseDocument,
			"Please choose document"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseItemDelete,
			"Choose item: Delete"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseItemCancel,
			"Choose item: Cancel"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseItemClose,
			"Choose item: Close"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartDateBeforeBirthDate,
			"Start Date is before the Birth Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TermDateNotBePriorToStartDate,
			"Term Date cannot be prior to Start Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseRow,
			"Choose row"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CardCodeBeSame,
			"CardCode must be the same"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UpdateFailed,
			"Update data failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotSupportForTree,
			"Not support for tree"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TableDoesNotExitsWithTableName,
			"Table {0} does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowIsEmpty,
			"Row is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoRecord,
			"No Record"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowSelectedResCodeEmpty,
			"Error. Row is selected have ResCode Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidUnit,
			"Invalid Unit"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChangeLogNotSupportObject,
			"ChangeLog not support for this object"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateBeweent,
			"Date must between: {0} - {1}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ColumnFieldNotBelongTable,
			"Column field could not belong table"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotLoadLayoutXml,
			"Cannot load layout xml"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotGetDataHeader,
			"Cannot get data - header"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CaptionIsMissing,
			"Caption is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PageIsNotMatch,
			"Page is not matched"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.IssueDateIsMissing,
			"Issue date  is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProjectCodeIsMissing,
			"Project Code is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidDocNumber,
			"Invalid Document Number"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UploadAttachmentFailed,
			"Upload Attachment failed: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AttachmentChangedOrDelete,
			"Attachment has been changed or deleted"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AttachmentPathNotAccess,
			"Attachment Path: {0} is not accessible"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DatasourceIsNull,
			"Data Source is Null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataMapping,
			"Could not get data mapping"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RemoveError,
			"Remove error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoRelatedControlSet,
			"No related control is set"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotGetDataBinAllocation,
			"Cannot get data bin allocation"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DecreaseBatchQty,
			"Decrease batch quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotGetDataBatchNumber,
			"Cannot get data batch number"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchSerialNumberInfomationDefinedUpdate,
			"Batch/serial number information can be defined/updated either in the Batch Management or Serial Numbers Management window"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InQtyValueGreater0,
			"In \"Qty\" (Quantity) column, enter value greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustBeChooseBatchSerialBeforeAllocateBin,
			"You must be choose batch/serial before allocate bin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocateBatchSerialMustBeGreater0,
			"Allocate batch/serial must be greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocateQtyGreaterLineQty,
			"Allocate quantity greater than line quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateRowWithDisNumberAndBinCode,
			"Duplicate row DisNumber = '{0}', BinCode = '{1}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateRowWithBinCode,
			"Duplicate row  BinCode = '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinAllocationCanBeDefinedUpdate,
			"Bin allocation can be defined/updated either in the Bin Management window"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InSerialFieldNotExists,
			"In \"Serial Number\" field, enter number does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InBatchNumberFieldNotExists,
			"In \"Batch Number\" field, enter number does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocateQtyGreaterThanBatchQty,
			"Allocate quantity greater than batch quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocateBinLocationGreater0,
			"Allocate bin location must be greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataBinAllocate,
			"Could not get data bin allocate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinLocationExistsOnGrid,
			"Bin location exists on grid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinDoesNotExists,
			"Bin does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ASerialNumberOnlyAllocate2ABin,
			"A Serial number only allocate to a bin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartDateShouldNotGreaterThanEndDate,
			"Start date should not be greater than End date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoMatchingRecordFound,
			"No Matching record found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskDoesNotExistsOrNotAccess,
			"Task \"{0}\" does not exist or is not accessible"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartDateIsRequired,
			"Start Date is required"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DueDateMustBeAfterStartDate,
			"Due Date must be after Start Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskNameIsRequired,
			"Task Name is required"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskNameIsRequired4Copy,
			"Task Name is required for copying"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResolutionTimeMustBeInterger,
			"Resolution Time  must be an integer"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResponseTimeMustBeInterger,
			"Response Time must be an integer"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EndTimeMustBeGreaterThanStartTime,
			"End Time must be greater than Start time"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterName,
			"Enter name"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterEmailAndPassword,
			"Enter email and password"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateMustBeBefore2Date,
			"From date must be before To date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SubjectCustomerIsNull,
			"Subject or Customer is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotClose,
			"Cannot close"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCanNotSchedule2InstanceOfSaveRecurrenceOnSameDay,
			"You cannot schedule two instances of the same recurrence on the same day"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCanNotScheduleAnEventSkipOverOtherEventSameRecurrence,
			"You cannot schedule an event that skips over another event of the same recurrence"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeIsNotValid,
			"Item code is not valid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateDataIsNull,
			"Date data is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveDataCanNotBeNull,
			"Save data cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotGetHeaderStruc2Save,
			"Cannot get header structure to save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateCanNotBeNull,
			"From date cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToDateCanNotBeNull,
			"To date cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseApprovalAtp,
			"You must choose an approved Atp"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CanNotGetDataAtp,
			"Cannot get data Atp"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseApprovalMrp,
			"You must choose an approved MRP"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseMrp134,
			"You must choose mrp with type 1,3,4"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseMrp3,
			"You must choose mrp with type 3"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseMrp234,
			"You must choose mrp with type 2,3,4"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FixDateCanNotBeNull,
			"Fixed date cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UdoTableIsInvalid,
			"UDO Table is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocEntryNotFound,
			"Docentry {0} not found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyQuantityItemIsSmallerOrEqualPlannedQuantity,
			"Specify a quantity of the item that is smaller than or equal to the planned quantity in the item line of the agreement"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyQuantityMustGreater0,
			"Specify quantity must be greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyToDate,
			"Specify To date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyFromDate,
			"Specify From date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyFromDateEarlierOrEqualEndDate,
			"Specify a From date that is earlier than or equal to the agreement's end date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyFromDateLaterOrEqualStartDate,
			"Specify a From date that is later than or equal to the agreement's start date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyToDateEarlierOrEqualEndDate,
			"Specify a To date that is earlier than or equal to the agreement's end date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyToDateLaterOrEqualStartDate,
			"Specify a To date that is later than or equal to the agreement's start date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyQuantityGreater0BlanketRow,
			"Specify a quantity greater than 0.  [Blanket Agreement - Rows - Planned Quantity]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SalesBlanketIsNull,
			"Sales Blanket Agreement is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidCurrency,
			"Invalid currency"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyEndDateBlanket,
			"Specify end date.  [Blanket Agreement - End Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TerminationDateBlanket,
			"Termination date.  [Blanket Agreement - Termination Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifySignDateBlanket,
			"Specify sign date.  [Blanket Agreement - Sign Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyStartDateBlanket,
			"Specify start date.  [Blanket Agreement - Start Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyEndDateLaterOrEqualStartDateBlanket,
			"Specify an end date that is later than or equal to the start date.  [Blanket Agreement - End Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidBpCodeBlanket,
			"Invalid BP code [Blanket Agreement - BP Code]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyPlanAmountLcGreaterThan0,
			"Specify a planned amount (LC) greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyPlanAmountFcGreaterThan0,
			"Specify a planned amount (FC) greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotSelectSameItemTwice,
			"You cannot select the same item twice; select different items in different rows"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidDate,
			"Invalid date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouDoNotEnterFromDate,
			"You do not enter From date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouDoNotEnterToDate,
			"You do not enter To date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ShipdateMustBetweenFromDateAndToDate,
			"Shipdate must be between fromdate and todate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseFromDateBeforeEditInput,
			"You must choose from date before edit input"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseToDateBeforeEditInput,
			"You must choose to date before edit input"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterItemCodeBefore,
			"You must enter ItemCode before"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataResultIsNullCannotSave,
			"Data result is null. You cannot save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseAtpType,
			"You must choose Atp type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterAtpStatus,
			"You must enter Atp status"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToDateMustGreaterOrEqualFromDate,
			"To date must be greater than or equal to from date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDataToAddNewInputTab,
			"Cannot get data to add new row input tab"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterWhsCode,
			"You must enter whs code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetApprovalTemplate,
			"Cannot get approval template"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseFromWhs,
			"Choose from warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDataToSave,
			"No data to save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateName,
			"Duplicate name '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnStagesTabStartDateMustLaterOrEqualStartDateInGeneral,
			"On \"Stages\" tab, start date must be later than or equal to start date in general area"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OpenDateCanNotAfterClosingDate,
			"Open date cannot after closing date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NextStageOpenDateCanNotBeforePreviousStage,
			"Next stage Open date cannot before previous stage closing date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChangeLogIsNotAvailable,
			"Change log is not available"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoBusinessPartnerChoose,
			"No Business partner choosen"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BusinessPartnerCanotBeNull,
			"Business partner cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BusinessPartnerDoesNotExistsOopr,
			"Business Partner code does not exist. [OOPR.CardCode]: '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeDoesNotExistsOopr,
			"Item code does not exist. [OOPR.CardCode]: '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CloseDateCannotNullWhenDocumentClose,
			"Close date cannot be null when document is closed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CloseDateCannotBeBeforeStartDate,
			"Close date cannot be before start date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PredicClosingDateMustLaterOrEqualStageClosingDate,
			"Predicted closing date must be later than or equals stage closing date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowIsSelectedHaveRoutCodeEmpty,
			"Row is selected have RoutCode is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseRoutingCodeBeforeChooseOpCode,
			"You must choose Routing Code before choose Opcode"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CardNameCannotBeEmpty,
			"Card Name cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CardNameCannotBeDuplicate,
			"Card Name cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetStructureToAddNew,
			"Cannot get stucture to add new"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterCodeBeforeSave,
			"You must enter code before save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowIsEmptyCannotDelete,
			"Row is empty, cannot delete"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetStructureToImportData,
			"Cannot get structure to import data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FileImportDoNotEnoughData,
			"File import do not enough data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadDataSetting,
			"Cannot load Data setting"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetRoutingLines,
			"Cannot get routing lines"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterRoutNo,
			"You must enter 'Route No'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterProductCode,
			"You must enter 'Product Code'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataLinesIsEmpty,
			"Data lines is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInDatabase,
			"This entry has already existed in the database"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInTable,
			"This entry has already existed in the table"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeNameCannotEmpty,
			"Type Name cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeNameCannotDuplicate,
			"Type Name cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HotKeyIsDuplicate,
			"Hot key [{0}] is duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FormAccessDenied,
			"User does not have permission to access this function"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.VendorDoesNotExistsChooseOtherVendor,
			"Vendor does not exist. Choose another vendor"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouDoNotChooseVendor,
			"You do not choose vendor"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StoreUpdateProductionFailed,
			"Store update production order executed failed: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDatasourceTableStructWithTableName,
			"Cannot get DataSource table [{0}] structure"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotReportCompletionChangeProductionToRelease,
			"Cannot report completion. Please change Production order status to Released"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PlannedQtyLesserIssueQty,
			"Planned quantity is lesser than issued QTY"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OrderDateIsMissingProductionOrder,
			"Order date is missing.  [Production Order - Order Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryIsNull,
			"This entry is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInBpPriorities,
			"This entry has already existed in the following tables 'BP Priorities'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteThisPriorityUsed,
			"Cannot delete, this priority used"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidDocObjectCode,
			"Invalid Doc. Object Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterDiscountPercentageLessThan100,
			"Enter a discount percentage of less than 100"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InDpmDownPaymentPerFieldEnterGreaterThan0,
			"In \"DPM %\" (down payment percentage) field, enter percentage value greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InDpmDownPaymentFieldEnterGreaterThan0,
			"In \"DPM\" (down payment) amount field, enter amount greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InQuantityColumnEnterGreaterThan0,
			"In \"Quantity\" column, enter whole number greater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveError,
			"Save Error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetValueUdfField,
			"Could not get value of UDF - {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartDateIsMissingProductionOrder,
			"Start date is missing.  [Production Order - Start Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DueDateIsMissingProductionOrder,
			"Due date is missing.  [Production Order - Due Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemIsMissing,
			"Item is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotIssueComponentChangeProductionToRelease,
			"Could not issue components. Please change Production order status to Released"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductNumberIsMissingProductionOrder,
			"Product Number is missing[Production Order - Product No.]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidItemNumber,
			"Invalid item number"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MustHaveComponentInProductionRow,
			"Must have components in production order rows"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HaveNoIssueData,
			"Have no issue data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HaveNoReceiptData,
			"Have no receipt data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemAlreadyChooseWithItemCode,
			"Item '{0}' has already chosen"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotFindMaterialShortageReport,
			"Cannot find Material Shortage Report"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoProductGiven,
			"No product given"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductQuantityMustGreaterThan0,
			"Product quantity must greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoItemInBom,
			"No item in BOM"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetLineStructure,
			"Cannot get line structure"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseBomBeforeChooseVersion,
			"You must choose Bom before choose Bom version"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnlyProductionBomOrTemplateBomCanDefinedAsPurchaseItem,
			"Only a production BOM or a template BOM can be defined as a purchase item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnlyProductionBomCanDefinedAnInventoryPurchaseItem,
			"Only a production BOM can be defined as an Inventory/Purchase item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ASalesBomMustDefinedASalesItem,
			"A sales BOM must be defined as a sales item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductNoDoesNotExists,
			"Product no does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemAlreadyDefinedAtHigherLevelInProductTreeBillOfMaterialWithLine,
			"Item already defined at a higher level in the product tree [Bill of Materials - Components - Component Code][line: {0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotMakeDependentOfItself,
			"You cannot make a  Dependent of itself"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DependentOnlyInputNumberFromColumnLineNum,
			"Dependent only input number from column LineNum"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeOnlyPositiveWholeNumber4SimpleRelationshipBetweenDependentInSameProject,
			"Type only positive, whole numbers for simple relationships between Dependent in the same project"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StatusIsNullOrIdIsNull,
			"Status is null or Id is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DeleteError,
			"Delete Error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InQuantityColumnEnterWholeNumberGreaterThan0QuantityMandatory,
			"In 'Quantity' column, enter whole number greater than 0 => Quantity mandatory !=0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetData,
			"Cannot get data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDataPredecessor,
			"Cannot get data - Predecessors"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeOnlyPositiveWholeNumber4SimpleRelationshipBetweenWbs,
			"Type only positive, whole numbers for simple relationships between WBS in the same project"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotMakeWbsPredecessorOfItself,
			"You cannot make a WBS a predecessor of itself"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProjectCannotLinkSummaryWbs2ItsSub,
			"Project can't link a summary WBS to one of its sub"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PredecessorOnlyInputFromLineNum,
			"Predecessor only input number from column LineNum"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PredecessorOnlyGetSmallerSelection,
			"Predecessor only get smaller selections"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WbsFromDateShouldNotGreaterThanProjectDueDate,
			"WBS's From date should not be greater than Project's Due date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WbsFromDateShouldNotGreaterThanWbsDueDate,
			"WBS's From date should not be greater than WBS's Due date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WbsDueDateShouldNotLessWbsFromDate,
			"WBS's Due date should not be less than WBS's From date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotMakeSubProjectOfItself,
			"You cannot make a Sub-project of itself"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskCodeExistsInTableWithId,
			"Task Code has already existed in the table: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeCannotBeNull,
			"Item Code cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvoiceMustPaidOrInvoiceHaveZeroPrice,
			"Invoice must be paid or Invoices have zero price"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoRow2Paid,
			"No row to paid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterNameBeforeEnterToDate,
			"You must enter name before you enter to date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseDayInYearSelected,
			"You must choose day in selected year"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterNameBeforeEnterRemarks,
			"You must enter name before you can enter Remarks"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterNameBeforeEnterYear,
			"You must enter name before you can enter Year"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterNameBeforeEnterFromDate,
			"You must enter name before you can enter from date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoesNotExists,
			"does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BardCodeIsUsed,
			"Bar Code is used"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryExistsInFollowingTableCountries,
			"This entry has already existed in the following tables 'Countries'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MasterOrderNameCannotBeEmpty,
			"Master order name can't be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CreateProductionPlanError,
			"Create Production Plan Error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TableIsEmptyCheckAgainInSaleOrder,
			"Table is empty. Check again in Sale Order, please"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DeleteErrorCannotDeleteAllRows,
			"Delete Error. Can't delete all rows"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RemoveRowErrorTableIsNull,
			"Remove Row Error. Table is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseRowBeforePressThisButton,
			"Please choose row before press this Button"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OrderLineInTabReceiptIsEmpty,
			"Order Line in Tab Recepit is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OrderNoInTabReceiptIsEmpty,
			"Order No in Tab Recepit is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchCompleteInTabReceiptIsEmpty,
			"BatchComplete in Tab Recepit is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchIssueInTabReceiptIsEmpty,
			"BatchIssue in Tab Recepit is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WarehousrInTabRoutingIsEmpty,
			"WareHouse in tab Routing  is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QuantityInTabRoutingIsEmpty,
			"Quantity in tab Routing is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchInTabRoutingIsEmpty,
			"Batch in tab Routing is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseItem,
			"Please choose item(s)"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoMatchingRecordFoundWithDocEntry,
			"No matching record found - DocEntry: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisControlKeyCodeExistedInRoutingOrPlanning,
			"This Control Key Code existed in Routing Or Planning"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ErrorRowIsSelectedHaveKeyEmpty,
			"Error. Row is selected have Key is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotCompleteBecauseNotEnoughInfomation,
			"NOT COMPLETE BECAUSE NOT ENOUGH INFORMATION"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDataReport,
			"Cannot get data report"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseBranchAfterChooseWhs,
			"You must choose branch after choose whs"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MustFocusedOnChildNode,
			"Must focus on child node"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineCodeLineNameCannotBeEmptyTotalMachineMustGreaterThan0,
			"LineCode, LineName cannot be empty; Total of Machine must greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineCodeExisted,
			"LineCode existed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FatherLineDoesNotExisted,
			"Father Line does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemsDataIsEmpty,
			"Items Data is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddItemsWhenLineNotActive,
			"Cannot add Items when line is not active"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UsersDataIsEmpty,
			"Users data is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidUserCode,
			"Invalid User code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MrpDataIsEmpty,
			"Mrp data is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoQuantityData,
			"No quantity data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoQuantityDataToCreatePurchaseOrder,
			"No quantity data to create purchase order"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetIdToGeneratePo,
			"Cannot get id to generate PO"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateVendorCodeWithCode,
			"Duplicate Vendor Code - [{0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseItemFirst,
			"Please choose item first"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemExistedWithItemCode,
			"Item - [{0}] existed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidItemCodeWithCode,
			"Invalid ItemCode - [{0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PeriodCannotBeNull,
			"Periods cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateItemWithCode,
			"Duplicate Item - [{0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateFromDateWithDate,
			"Duplicate From Date - [{0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateTimeOverlappedWithValue,
			"Date time is overlapped. From Date -[{0}] and -[{1}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDocumentSelected,
			"No documents selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MissingData,
			"Missing Data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DetailIsEmpty,
			"Detail is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TitleDataIsEmpty,
			"Title data is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCannotAppearTwiceInForecast,
			"Item cannot appear twice in a forecast"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TableNotFound,
			"Table Not Found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MappingItemCodeIsMissing,
			"Mapping Item code is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeDoesNotExists,
			"Item does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WarehouseCannotBeNull,
			"Warehouse cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDuplicateKeyItemCodeWhsCodeWithValue,
			"Cannot duplicate key (ItemCode, WhsCode, BPCode) = ({0},{1},{2})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotProcessDateData,
			"Cannot process Date data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseDomain,
			"Please choose domain"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseDatabase,
			"Please choose database"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseEnterUserName,
			"Please enter user name"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseEnterPassword,
			"Please enter password"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ClientDoesNotAuthenticated,
			"Client does not authenticate. Contact admin to get file certificate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ClientDoesNotAuthenticatedWithDomain,
			"Client does not authenticated with this domain"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotReadFileToUpdate,
			"Cannot read file to update"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisItemDoesNotExistsInList,
			"This item does exists in list"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ManufacturerIsAssignedToItems,
			"Manufacturer is assigned to items"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryExistsInFollowingTableManufacturers,
			"This entry has already existed in the following tables 'Manufacturers'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadIssueMethod,
			"Cannot load \"Issue Method\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadPropeties,
			"Cannot load \"Properties\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadItemType,
			"Cannot load \"Item Type\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadUomGroup,
			"Cannot load \"UoM Group\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadPriceList,
			"Cannot load \"Price List\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadPriceType,
			"Cannot load \"Price Type\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadMenufactures,
			"Cannot load \"Manufactures\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadShippingType,
			"Cannot load \"Shipping Type\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadManageItemBy,
			"Cannot load \"Manage Item by\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadManagementMethod,
			"Cannot load \"Management Method\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadCustomGroup,
			"Cannot load \"Custom Group\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadVatGroupPurchase,
			"Cannot load \"Vat Group Purchase\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadVatGroupSales,
			"Cannot load \"Vat Group Sales\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadGlMethod,
			"Cannot load \"GL Method\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadPlanningMethod,
			"Cannot load \"Planning Method\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadProcurementMethod,
			"Cannot load \"Procurement Method\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.VendorAlreadyExists,
			"Vendor has already existed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateRangeOverlapChangeAvtiveOrInactiveDateRange,
			"Date ranges overlap; change the active or inactive date range"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidDateSpecifyToDateNoEarlierThanFromDate,
			"Invalid date; specify a \"To\" date no earlier than the \"From\" date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheMinimumInventoryLevelMustLessThanMaximumInventoryLevelWithCode,
			"The minimum inventory level must be less than maximum inventory level[Items - Maximum Inventory Level], '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheMaximunStockLevelMustGreaterThanRequiredStockLevelWithCode,
			"The maximum stock level must be greater than the required stock level[Items - Maximum Inventory Level], '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheMinimumInventoryMustLessThanRequireInventoryWithCode,
			"The minimum inventory must be less than the required inventory [Items - Minimum Inventory Level], '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PhantomItemCannotBeInventoryItem,
			"Phantom item cannot be inventory item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCopyPictureFromServer2TempFolder4Preview,
			"Cannot copy picture from server to temp folder for preview"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SerialOrBatchManagedItemCannotDefinedBackflush,
			"Serial or batch managed items cannot be defined as backflush"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SetInventoryMethodBy,
			"Set Inventory Method By"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteRowThereIsStockInThisWhs,
			"Cannot delete row - there is stock in this warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteRowThisWhsHasExistsingInventoryTransaction,
			"Cannot delete row; this warehouse has existing inventory transactions"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemValuationMethodIsSerialBatchItMustBeManagedByBatchOrSerialNumber,
			"Item's valuation method is Serial/Batch; it must be managed by batch or serial numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCanModifyManageItemBySettingOnlyAvailableQtyIs0,
			"You can modify the 'Manage Item by' setting only when the available quantity is zero and there are no serial or batch numbers available"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheSelectedWarehouseAlreadyExists,
			"The selected warehouse already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollwingTableDeliveryType,
			"This entry has already existed in the following tables 'Delivery Types'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataName,
			"Not enough data, 'Name'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRemoveObjectInUseInDocuments,
			"Cannot remove, object in use in documents"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoRowToInsert,
			"No rows to insert"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CurrencyDoesNotMatchPriceList,
			"Currency does not match price list"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidFactor,
			"Invalid factor"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.APriceListWithNamAlreadyExists,
			"A price list with this name already existed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PriceListInfiniteLoopInPriceListDependenciesWithCode,
			"Price List {0} - Infinite Loop in Price List Dependencies"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoRowToUpdate,
			"No rows to update"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoBussinessSpecify,
			"No business specify"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoBpGroupSpecify,
			"No BP Group specify"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDocumentToSave,
			"No Document to save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllBpRecordExistsCannotInsertAgain,
			"All BP record exists, cannot insert again"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ContentIsEmpty,
			"Content is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SelectRowToRelease,
			"Select row to Release"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnlySelect1Order2Release,
			"Only select one Order to release"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchSerialNumberAlreadyExistsWithCodeItemAndLine,
			"Batch/Serial number '{0}' for item '{1}' in row {2} already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRemoveConfirmedLine,
			"Cannot remove the confirmed line"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCheckBatchSerialResult,
			"Cannot check batch/serial result"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseItemToRemove,
			"you must choose Item to remove"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseBpCodeToRemove,
			"you must choose Bpcode to remove"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseBpBeforeSave,
			"You must choose BP before save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BpCatalogNumberIsMissing,
			"BP Catalog Number is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseItemBeforeSave,
			"You must choose item before save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BpIsMissing,
			"BP is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableCardProperties,
			"This entry already exists in the following tables 'Card Properties'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCopyFromManyBp,
			"Cannot copy from many business partner"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoItemChoosen,
			"No item was chosen"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentMustCopyFromSameCustomer,
			"Documents must copy from the same customer"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PaymentTermCodeCannotBeEmpty,
			"Payment Terms Code can't be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentCurrencyIsNull,
			"Document currency is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseOriginItemBeforeAddNew,
			"Choose Origin Item Before Add New"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoOriginItem,
			"No Origin Item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoAlternativeItem,
			"No Alternative Item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemIsIdenticalToItemOrogin,
			"Item Is Identical To Item Origin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisDocumentIsClose,
			"This Document is closed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDocumentToCopyTo,
			"No document to Copy to"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterDueDateInventoryTransferRequest,
			"Enter due date [Inventory Transfer Request - Due Date]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateFieldCannotBeNull,
			"Date field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TimeFieldCannotBeNull,
			"Time field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidCountingType,
			"Invalid counting type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadStructure,
			"Cannot load structure"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BarcodeDoesNotExists,
			"Barcode does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TakerIdCannotBeNull,
			"Taker Id cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataCountingCannotBenull,
			"Data counting cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouOnlySelectedOneWhs,
			"You only select one WhsCode"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ServiceErrorReloadForm,
			"Service error. Please Reload form again"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseItem,
			"You must choose item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseCustommer,
			"Please choose Customer"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotBeAddedAddingContentWouldCauseCirularDependency,
			"Could not be added. Adding this \"{0}\" as content would cause a circular dependency"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocDateCannotBeNull,
			"Doc date cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetIssueComponent,
			"Cannot get issue components"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FactorMustBeGreaterThan0,
			"Factor must be grater than 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisFunctionIsNotSupported,
			"This function is not supported"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromWhsInHeaderCannotBeEmpty,
			"From Warehouse in  header cant not be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToWhsInHeaderCannotBeNull,
			"To Warehouse in header cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PostingDateCannotBeNull,
			"Posting date cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateDeviatesFromPermissibleRange,
			"Date deviates from permissible range"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoReleaseWhsHasBeenDefined,
			"No release warehouse has been defined"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowWithoutReceiptWhs,
			"Row without receipt warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineWhsConnotBeNull,
			"Line warehouse cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReceiptWhsCannotBeIndenticalToReleaseWhs,
			"Receipt warehouse cannot be identical to the release warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadDataCopy,
			"Cannot Load data copy"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemAlreadyImported,
			"Items already imported"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDataToImport,
			"No data to import"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisActivityDoesNotExistAnyMore,
			"This activity does not exist anymore"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SomethingWrong,
			"Something wrong"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetInfo,
			"Cannot get info"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotChangeActivityAction,
			"You cannot change the activity's action"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotRemoveContactPersonInUseWithCode,
			"You cannot remove contact person '{0}'; this contact person is used in '{1}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotChangeBpTypeLickedObjectExists,
			"Cannot change BP type, linked object exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheNameFieldMustNotEmptyEnterNameForContactPerson,
			"The \"Name\" field must not be empty; enter a name for the contact person"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidIndicatorCode,
			"Invalid indicator code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BusinessPartnerCodeAlreadyExistsAssignedToBpEnterUniqurBpCode,
			"Business partner code '{0}' already assigned to a business partner; enter a unique business partner code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BpCodeCannotBeEmpty,
			"Business Partner code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRenameBpWhenTransactionExists,
			"Cannot rename Business Partner's Code when transaction exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BpAlreadyImported,
			"BPs already imported"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AddOrUpdateIsNotValidActionForBOMObject,
			"'Add or Update' is not a valid action for Bill of materials object"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoSourceFielSelected,
			"No source file selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FileInvalidFormat,
			"File {0} is invalid format"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThereIsNoObjectOrImportCodeGenerated,
			"There is no object or import code generated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FileLineNumberMustGreaterThan2,
			"File lines number must greater than 2"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BranchCannotBeEmpty,
			"Branch cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartAndEndOutTimeCannotbeEmpty,
			"Start and End Out Time cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartAndEndInTimeCannotBeEmpty,
			"Start and End In Time cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartAndEndWorkingTimeCannotBeEmpty,
			"Start and End Working Time cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartAndEndBreakTimeMustBeAllEmptyOrAllHaveValue,
			"Start and End Breaking Time must be all empty or all have values"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemDoesNotExistsOrCannotGetItemDetailTable,
			"Item does not exist or cannot get item details table"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RoundUnitCannotBeEmpty,
			"Round Unit cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WOrkTypeCannotBeEmpty,
			"Worktype cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NameCannotBeEmpty,
			"Name cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CodeCannotBeEmpty,
			"Code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UnableToCalculateLeaveNoData4PeriodWasFound,
			"Unable to calculate leave, no data for period [{0}] was found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResourceIdCannotBeEmpty,
			"ResourceId cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResourceIdCannotBeDuplicate,
			"ResourceId cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectAPeriod,
			"Please select a Period"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectOTType,
			"Please select an OT Type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseEnterValidEmployeeNo,
			"Please enter a valid Employee No"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PeriodCannotBeCloseHere,
			"Period cannot be closed in here"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseABranch,
			"Please choose a Branch"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LeaveDayHasExeededLimit,
			"Leave days({0}) of {1} has exceeded the limit({2})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OTHourHasExeededLimit,
			"OT hours({0}) of {1} has exceeded the limit({2})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidWorkCodeInRow,
			"Invalid WorkCode in row {0}, column {1}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WorkCodeIsInvalidWithCode,
			"Work code [{0}] is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ExportFailedNoDataWereFound,
			"Export failed. No data found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDataFound,
			"No data found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Unable2ClosePeriodNoInfomationAboutCurrentPeriodFound,
			"Unable to close period, no information about current period was found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DescriptionCannotBeNull,
			"Description cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FileIsInvalidFormatNumberOfRequiredFieldNotMetWIthPath,
			"File {0} is in invalid format, the number of required fields is not met"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FileIsInvalidFormatHavingNoLine,
			"File {0} is in invalid format for having no lines"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseMapSourceFieldToThisTargetField,
			"Please map a Source Field to this Target Field ({0})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ATargetFieldCanOnlyBeMapTo1SourceField,
			"A Target Field can only be mapped to one Source Field"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NullValueDetected,
			"Null value detected in {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidDateFormat,
			"Invalid Date format: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EmployeeIsInvalid,
			"{0}  is an invalid Employee No"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WorkCodeIsInvalid,
			"{0} is an invalid Work Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateIsNotInSelectedPeriod,
			"{0} is not in selected Period ({1})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoTemplateWereFound,
			"No templates were found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoEntryCatalogName,
			"No entry Category name"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotDeleteSystemQueriesCatalog,
			"You cannot delete system queries category"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReportFileNotFoundWithPath,
			"Report file not found: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouDoNotPermisionModifiedForm,
			"You are not permitted to perform this action"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InstallPlugInFailed,
			"Install plug in {0} failed. {1}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YourSessionHasBeenLoackedOtherSessionStarted,
			"Your session has been locked because another session was started using the same user ID.\r\nTo continue, exit or lock the other session"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateCannotBeEmptyWithCode,
			"From Date cannot be empty! [Code = {0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateCannotBeLargerThanToDateWithCode,
			"FromDate cannot be larger than ToDate! [Code = {0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentIsEmpty,
			"Document is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentLineIsEmpty,
			"Document's lines is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HolidayCannotBeEmpty,
			"Holiday Code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BranchOrPeriodIsEmpty,
			"Branch or Period is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig,
			"Can not connect to device! Please recheck configuration"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValidFromDateCannotBeNull,
			"ValidFrom Date cannot be NULL"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValidFromDateCannotBeLargeThanValidToDate,
			"ValidFrom Date cannot be larger than ValidTo Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EmpIdCannotBeEmpty,
			"EmpId cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateCannotBeEmptyWithEmpId,
			"From Date cannot be empty! [EmpNo ={0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValidFromCannotbeLargeThanValidToDateWithEmpId,
			"ValidFrom Date cannot be larger than ValidTo Date! [EmpNo = {0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouDoNorHavePermisionToModifiedForm,
			"You do not have permision to modify this form"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoMatchingRecordFoundPleaseCalculateBeforeViewSalary,
			"No matching records found! Please calculate before view Salary"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCalculateSalaryBeforeMissingTimeSheeting,
			"Cannot calculate salary because of missing TimeSheeting"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValidFromFieldCannotBeNull,
			"ValidFrom field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AmountCannotBeEmpty,
			"Amount cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetPeriodInfomation,
			"Can not get Period Information"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ImportRptFileFailed,
			"Import Rpt File failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaxLevelCannotBeEmpty,
			"TaxLevel cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaxLevelCannotBeDuplicated,
			"TaxLevel cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MinAmountCannotBeEmpty,
			"Min Amount cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MaxAmountCannotBeEmpty,
			"Max Amount cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RatioCannotBeEmpty,
			"Ratio cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MinAmountIsNull,
			"Min amount is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseEventType,
			"Please choose one of event type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseWriteFomularBeforeSaving,
			"Please write formula before saving"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseCategory,
			"Please choose category"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FormIdCannotBeNull,
			"Form Id cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventIdCannotBeNull,
			"Event Id cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableBpPlace,
			"This entry already existed in the following tables 'Business Place'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseCountryBefore,
			"Please choose country before"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteTeritoryReferencedByOtherObject,
			"Cannot delete territory referenced by other objetcs"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotSetTerritoryWhoseParentTerritoryIsInactiveToActive,
			"Cannot set territory whose parent territory is \"inactive\" to \"active\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteThisGroupUsed,
			"Can't delete,this group used"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateCannotBeEmptyWithEmpNo,
			"Date cannot be empty! [EmpNo = {0}]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseFromOneBp,
			"Choose from one bussiness partner"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouAreNotAssignedToBranchAndCannotCopyBaseDocumentToTargetDocument,
			"You are not assigned to branch {0} and cannot copy base document to target document"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LayoutNotFound,
			"Layout not found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotConfigFtpYet,
			"Not config ftp yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UploadPictureFailed,
			"Upload Picture File failed: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UploadPictureFileFailedSourceFileNotFoundWithPath,
			"Upload Picture File failed: Source file [{0}] not found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HolidayCodeCannotbeEmpty,
			"Holiday Code cannot empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed,
			"Can't delete,this record is being used"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ApplicantNoOrApplicantNameOrQualificationFieldCannotBeNull,
			"Applicant No or Applicant Name or Qualifications field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataParamater,
			"Could not get data parameter"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataReportParamater,
			"Could not get data report's parameters"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataIsEmpty,
			"Data is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotReadDataFromThisFileTagAPZ_OCTANotAvailable,
			"Could not read data from this file. Tag APZ_OCTA was not available"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotReadDataFormThisFile,
			"Could not read data from this file"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataHttpRequestConfig,
			"Could not get data http request Config"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseCheckCronExpressionBeforeSaving,
			"Please check cron expression before saving"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartJobFailedWitId,
			"Start Job Failed: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSaveDataFirst,
			"Please save data first"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StopJobFailedWithId,
			"Stop Job Failed: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteUnitCodeUsed,
			"Cannot delete,this UnitCode used"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataLength,
			"Not enough data, 'Length (mm)'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyInFollowingTableLengthUnits,
			"This entry already exists in the following tables  'Length Units'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FieldCannotBeNullWithFieldName,
			"{0} field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AObjectTypeCanOnlyHaveOneCustomAttachmentPath,
			"An Object Type can only have one custom attachment path"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidFolderPath,
			"Invalid folder path"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SalesEmployeeAlreadyAssociatedWithAnotherEmployeeOrAnotherSalesEmployee,
			"Sales employee already associated with another employee, or employee already associated with another sales employee"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAssociateWithEmployeeUpdateSalesEmployeeFirst,
			"Cannot associate with employee; update the sales employee first"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRemoveARecordCreatedBySystem,
			"Cannot remove a record created by the system"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataSalesEmployeeName,
			"Not enough data, 'Sales Employee Name'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoChangedRowToSavefrm_SalesEmployeeBuyers,
			"No changed rows to save (frm_SalesEmployeeBuyers - 471)"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RptPathDoesNotExists,
			"Rpt file path does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseImportRptFileBeforeSendMail,
			"Please Import Rpt File before send mail"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetTableStructureWithTableName,
			"Cannot get table structure '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnteredIpServerYet,
			"Not entered Ip server yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnteredServerPortYet,
			"Not entered Server Port yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnteredHardwareKeyYet,
			"Not entered Hardware key yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoNotChooseLicenseFile,
			"Do not choose license file"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadLicenseFile,
			"can't load license file"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ApiAddressIsInvalid,
			"Api address is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouHaveChangedDatabasePleaseReloginAndMapLicense,
			"You have changed database! please relogin and map license again"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PasswordIncorrect,
			"Password Incorrect"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NameCannotBeDuplicate,
			"Name cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateOrToDateCannotBeNull,
			"FromDate or ToDate  field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.VisOrderMustNotBeIdentical,
			"VisOrder must not be identical"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouHaveNoApplicant,
			"You have no Applicants"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EmployeeCannotBeHisOwnManager,
			"Employee can't be his own manager"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LoadDataError,
			"Load data ERROR"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSaveThisEmployeeBeforeCreatingNewRelatives,
			"Please save this Employee before creating new his/her Relatives"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UploadPictureFailedFilePathDidNotExists,
			"Upload Picture failed: File Path did not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseSignerFromEmployeeList,
			"Please choose a Signer from Employee List ({0})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QuestionContentOrQuestionGroupFieldCannotBeNull,
			"Question Content or Question Group field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AnswerContentFieldCannotBeNull,
			"Answer Content field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRemoveSystemData,
			"Can't remove system data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FailedToReply,
			"Failed to reply"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectAPage,
			"Please select a page"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ActiveFailed,
			"Active failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidPageToken,
			"Invalid Page Token"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValueCannotBeEmptyTaxDefinitionCode,
			"Value cannot be empty [Tax Definition - Code]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExsitsInFollowingTableTaxDefinition,
			"This entry already exists in the following tables 'Tax Definition'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DefineEffectiveDate,
			"Define effective date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheTargetTableCannotAddNewRowAutomaticallyContentBelowRowNotCopied,
			"The target table cannot add new rows automatically. Contents below row '1' were not copied"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TemplateNameIsMissingContractTemplate,
			"Template name is missing [Contract Template - Template Name]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableCustomerGroup,
			"This entry already exists in the following tables 'Customs Groups'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateOrDerscriptionFieldCannotBeNull,
			"Date or Description field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeFieldCannotBeNull,
			"Type field can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UomGroupCannotBeDeleteBecauseItIsCurrentlyReferencedWithCode,
			"UoM group {0} cannot be deleted because it is currently referenced"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GroupCannotBeDuplicated,
			"Group cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataGroupCode,
			"Not enough data , 'Group Code'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataGroupDers,
			"Not enough data , 'Group Description'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UomGroupCanOnlyDefinedIfBaseUomDefinedForItWithCode,
			"UoM group {0} can only be defined if a base UoM has been defined for it"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterGroupCodeBeforeCreateDefinetionForNewGroup,
			"Enter the Group Code before create definition for new Group"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UriIsMissing,
			"uri is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetApprovalStatusAndDocumentStatusWithDraftKey,
			"Could not get approval status and document status - draft key: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCreateDocumentThatHasNotApprovedWithDraftKey,
			"Cannot create document that has not Approved - draft key: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DraftIsNotOpenCouldNotGenerateDocumnetWithDraftKey,
			"Draft is not open, could not generate document - draft key: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocEntryIsMissing,
			"DocEntry is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataMappingIsMissing,
			"Data mapping is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DatasourceIsMissing,
			"Data source is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.HeaderDataMappingIsMissing,
			"Header's data mapping is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartDateIsMissing,
			"Start Date is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EndDateIsMissing,
			"End Date is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SourceDataDoesNotContainColumn,
			"Source data does not contain column - {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetSourceDataDataSourceStringIsEmpty,
			"Could not get source data - Data String is Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetSourceData,
			"Could not get source data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataProductionHeader,
			"Could not get data - production header"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataItemGeneral,
			"Could not get data - items - general"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UserCodeCannotBeNull,
			"User Code cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UserNameCannotBeNull,
			"User Name cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WarehouseCodeAlreadyExistsEnterAnotherWhsCode,
			"Warehouse Code already exists! Enter anthor Warehouse Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidWhsCode,
			"Invalid warehouse code.  [Warehouses - Warehouse Code]"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateGroupName,
			"Duplicated group name '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteFolderItContainSubitem,
			"Cannot delete the folder; it contains subitems"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ErrorConnect2Server,
			"Error connect to server"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadFormWithMenu,
			"Can not load form {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ParentMenuDoesNotExistsWithName,
			"Parent menu \"{0}\" doesn't exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ParentMenuCodeDoesNotExists,
			"Parent menu code doesn't exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadMenu,
			"Can not load menu"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheLicenseHasBeenChangedToContinueExitOrLockTheSession,
			"The license has been changed. To continue, exit or lock the other session."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateOrKindFieldCannotBeNull,
			"Date or Kind field cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ContractCodeCannotBeNull,
			"Contract Code can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SignDateCannotBeNull,
			"Sign Date can't be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseConfirmerFromEmployeeList,
			"Please choose a Confirmer from Employee List ({0})"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ClientScretIsMissing,
			"ClientSecret is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ClientIdIsMissing,
			"ClientId is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MailAddressIsMissing,
			"Mail Address is missing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustSetupSmtpCredential,
			"You must setup Smtp Credentials"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotUploadTheTokenFilePleaseContactAdministratorForMoreDetail,
			"Can not upload the Token file! Please contact Administrator for more details"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DateCannotBeOutOfPeriod,
			"Date cannot be out of Period! EmpId ={0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TypeCannotBeEmpty,
			"Type cannot be empty! EmpId = {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouHaveToEnterType,
			"You have to enter type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotConnect2ServerPleaseTryAgain,
			"Can not connect to server. Please try again.."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDataConfig,
			"Cannot get data config"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FatherMenuIsMissing,
			"Father menu is misssing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDistributeRuleDataToSave,
			"No Distribute rule data to save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDistributeRuleLineToSave,
			"No Distribute rule line data to save"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterValidAmountInTotal,
			"Enter valid amount in \"Total\""
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PrcCodeExistsInList,
			"PrcCode exists in list"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateDistributeRuleCode,
			"Duplicate Distribution Rule Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDistributeLineData,
			"No Distribute Line Data"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyValidateToDateThatIsSameOrLaterThanValidFromDate,
			"Specify \"Valid To\" date that is the same as or later than \"Valid From\" date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CharacterNumberIsGreaterThanAllowed,
			"Character number is greater than allowed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableCurrencyCode,
			"This entry already exists in the following tables 'Currency Codes'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterValidCodeInternationalCode,
			"Enter valid code, 'International Code'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThatOperationHasCanceled,
			"That operation has canceled"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CostCenterAlreadyExists,
			"Cost center already exists.  Field: Cost Center"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterCostCenterField,
			"In \"Cost Center\" field, enter cost center code.  Field: Cost Center"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifyEffectiveToDateThatIsSameOrLaterThanEffectiveFrom,
			"Specify \"Effective To\" date that is the same as or later than \"Effective From\" date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotGetDataContainer,
			"Can not get data container"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConnectSuccessfully,
			"Connect successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConnectFailed,
			"Connect failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RegionOfStructureCannotBeEmpty,
			"Region of structure cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisGeographyIsInvalid,
			"This Geography is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CountryIsInvalid,
			"Country is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RegionDoesNotExists,
			"Region does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GeographyDoesNotExists,
			"Geography does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisItemAlreadyExistsInDetail,
			"This item already exists in Detail"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisStoreAlreadyExists,
			"This store already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GeographyCodeCannotBeEmpty,
			"Geography code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CountryCannotBeEmpty,
			"Country cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisRegionIsInvalid,
			"This region is invalid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisCountryDoesNotExists,
			"This Country does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseRegionOfStructure,
			"Please choose region of structure"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoEntryCategory,
			"no entry category"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QueryIsNull,
			"Query is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheFileCouldNotBeOpened,
			"The file could not be opened.{0}{0}Path: {1}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDisconnectPlugin,
			"Can not disconnect plugin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataLandCostHeader,
			"Could not get data - Landed costs - header"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGenerateDataLandCostHeader,
			"Could not generate data - Landed costs - header"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnItemTabEnterValueInWeightColumn,
			"On Items tab, enter value in Weight column"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnItemTabEnterValueInVolumeColumn,
			"On Items tab, enter value in Volume column"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataLandCost,
			"Could not get landed cost -0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AccountCannotBeEmpty,
			"Account cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidBankCode,
			"Invalid BankCode"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidUomWithCode,
			"Invalid UoM: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidBasicUnitParam,
			"Invalid basic unit param : {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteDefaultRecord,
			"Can not delete default records"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteUomAsignedToUomGroup,
			"Cannot delete UoM {0};  UoM is assigned to a UoM group"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidUomInDataPleaseCheckBeforeSaving,
			"Invalid Uom in Data! Please check before saving"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UomCodeCannotBeEmpty,
			"UomCode cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UomCodeCannotBeDuplicated,
			"UomCode cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseOneOfItemProcessEvent,
			"Please choose one of item process event"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisFormIsNotAvaiable,
			"This form is not avaiable"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventCannotProcessInForm,
			"{0} event cannot process in Form"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EventCannotProcessInView,
			"{0} event cannot process in View"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemIdCannotBeNull,
			"Item Id cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataBusinessPartnerGeneral,
			"Could not get data - business Partner - general"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ControllerCodeCannotBeDuplicate,
			"ControllerCode cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TabNameInACatagoryCannotBeBuplicated,
			"Tab Name in a Category cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FatherControllerCannotBeEmpty,
			"Father Controller Code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CategoryCannotBeEmpty,
			"Category cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TabNameCannotBeEmpty,
			"Tab Name cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UserMustNotBeNull,
			"User must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OldPassMustNotBeNull,
			"Old Password must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NewPassMustNotBeNull,
			"New Password must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MinLengthPass,
			"Min length Pass is 6"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmPassMustnotBeNull,
			"Confirm Password must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmPassIsNotTheSameNewPass,
			"Confirm Password is not the same New Password"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FieldCannotBeUpdate,
			"Field cannot be updated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TableIsInUseInOneOfUserObject,
			"The table is in use in one of the user objects"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyInFollowingTableOutb,
			"This entry already exists in the following tables  'User Tables'(OUTB)"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterValidTableName,
			"Enter valid TableName"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ObjectCodeIsEmpty,
			"Object code is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustToFillInfomation,
			"You must to fill information"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InvalidTimeString,
			"Invalid time string"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InFieldNameColumnYouCannotEnterDuplicateValue,
			"In \"Field Name\" column, you cannot enter duplicate values"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InValueColumnYouCannotEnterDuplicateValues,
			"In \"Value\" column, you cannot enter duplicate values"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.KeyNameIsNotEmpty,
			"Key Name is not empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.KeyMustHaveAtLeast1Field,
			"Keys must have at least 1 field"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotLoadPlugInFile,
			"Can not load plugin file"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FolderPluginRegisterDoesNotExists,
			"Folder plugin register does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseFolderPluginRegister,
			"You must choose folder plugin register"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoNotHavePermissionAcessFolder,
			"Do not have permision access folder"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotRegisterPluginUploadFileToServerFailed,
			"Could not register plugin. Upload file to server failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotFindTable,
			"Can not find table"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAccess,
			"Can not access"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateDate,
			"Duplicates dates"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AlcCodeCannotBeEmpty,
			"AlcCode cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RecordAlreadyExists,
			"Record already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotReadFileToUpdate,
			"Could not read file to update"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentHeaderNotFocus,
			"Document header not focus"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentHeaderNotFound,
			"Document header not found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartCountDataIsNotNull,
			"Start count date is not null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TimeMustNotBeNull,
			"Time must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterValidCode,
			"Enter valid code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CodeCannotBeDuplicate,
			"Code cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotInsertNullValueIntoColumName,
			"Cannot insert the value NULL into column 'Name', table: @{0}; column does not allow nulls. UPDATE fails."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataPropetyName,
			"Not enough data, 'Property Name'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableItemPropeties,
			"This entry already exists in the following tables 'Item Properties'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableLocation,
			"This entry already exists in the following tables 'Locations'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NotEnoughDataWeight,
			"Not enough data  , 'Weight (mg)'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableWeightUnit,
			"This entry already exists in the following tables  'Weight Units'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ExecuteQueryError,
			"Execute query error"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmationFailedTypeAgain,
			"Confirmation failed.Type again"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PasswordMustBeDifferentFromPasswordExample,
			"Password must be different from password example"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableVTG1,
			"This entry already exists in the following tables '(VTG1)'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseCetificateFile2Import,
			"You must choose certificate file to import"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataJornalEntryList,
			"Could not get data - journal entry - list"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetJornalEntryGeneral,
			"Could not get data - journal entry - general"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterAPositiveValue,
			"Enter a positive value"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataBillOfMaterialGeneral,
			"Could not get data - Bill of materials - general"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ColumnDoesNotBelongItem,
			"Colum {0} does not belong item {1}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RestoreSuccess,
			"Restore success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CopyFormatRuleSuccess,
			"Copy format rule success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CopyFormSettingSuccess,
			"Copy Form Setting success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AddedSuccessfully,
			"Added successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ImportSuccess,
			"Import success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SuccessWithContent,
			"Success - {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SuccessWithDocNo,
			"Success - Doc No. {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GenTaskSuccessfully,
			"Gen Task successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ImportFinished,
			"Import finished"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StoreUpdateProductionOrderExecuteSuccess,
			"Store update production order executed successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Import2TempTableFinishSuccess,
			"Import to temp table finished success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OperationCompletedSuccess,
			"Operation completed successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RunSuccess,
			"Run success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CreatePurchaseOrderSuccess,
			"Create Purchase order success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReleaseSuccess,
			"Release success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CreateSuccess,
			"Create success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReportSuccess,
			"Report success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.IssueAndReceiptCompleted,
			"Issue and receipt completed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ClosePeriodSuccess,
			"Close period successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ExportSuccess,
			"Export Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ActiveSuccess,
			"Active success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InstallPluginSuccess,
			"Install plug in {0} success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReloadAuthorizationModuleSuccess,
			"Reload Authorization Module Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UninstallPluginSuccess,
			"Uninstall plug in {0} success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AuthozationCopySuccess,
			"Authorization copy success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataCopiedToClipboad,
			"Data copied to clipboard"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RegisterPluginSuccess,
			"Register Plugin success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PingDeviceSuccess,
			"Ping device successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LeavehaveBeenCalculateSuccess,
			"Leaves have been calculated successfully"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ImportLicenseSuccess,
			"Import license success. Please login with administrator to granted license for user"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StartJobSuccess,
			"Start Job Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StopJobSuccess,
			"Stop Job Success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PluginDisconnected,
			"Plugin disconnected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GenerateJournalEntryCopleted,
			"Generate Journal Entry Completed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.RowEffected,
			"{0} row(s) affected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveDraftSuccess,
			"Save Draft success"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.GenerateItemSuccess,
			"Generate items completed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotParseDataOnThisControl,
			"Could not parse data on this control"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddBinAllocationToRowQuantity0,
			"Cannot add bin allocation to row {0} with quantity equal 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddRowWithoutCopleteSelectionBinAllocation,
			"Cannot add row {0} without complete selection of bin allocation"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddBinAllocationRowWithoutInventoryMovement,
			"Cannot add bin allocation to row {0} Without Inventory Movement"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterBatchNo,
			"You must enter Batch no"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustEnterSerialNo,
			"You must enter serial no"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocationBatchSerialQtyGreaterThanItemQty,
			"Allocate batch/serial quantity greater than item quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WarehouseNotSetupFollowBin,
			"Warehouse {0} not setup follow bin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinDidNotExists,
			"Bin did not exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchSerialDidNotExistsInBin,
			"Batch/Serial did not exist in bin"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseItemCodeBeforeChooseBatch,
			"You must choose itemcode before choose batch"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseWhsBeforeChooseBatch,
			"You must choose warehouse before choose batch"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.InSerialNumberFieldEnterAlreadyExists,
			"In \"Serial Number\" field, enter already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinAllocateQtyGreaterThanItemQuantity,
			"Bin Allocate quantity greater than item quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataRowIsEmpty,
			"Data row is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineNumberIsDuplicate,
			"LineNumber is dupplicate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineDoesNotExists,
			"Line does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QuantityIsGreaterThanRequired,
			"Quantity is greater than required"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QuantityIsSmallerThanRequired,
			"Quantity is smaller than required"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddBatchSerialNumberRowWithQuantity0,
			"Cannot add batch/serial number to row {0} with quantity equal 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddRowWithoutCompleteSelectionBatchSerialNumber,
			"Cannot add row {0} without complete selection of batch/serial numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddBatchSerialRowWithoutInventoryMovement,
			"Cannot add batch/serial to row {0} Without Inventory Movement"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddRowQuantityMustGreaterThanOrEqualRequiredBatchSerialNumber,
			"Cannot add row {0}. Quantity must be greater than or equals required of batch/serial numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationWhs,
			"Item '{0}' Line {1} must be fully allocated to bin locations warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationToWhs,
			"Item '{0}' line {1} must be fully allocated to bin locations to warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddRowQuantityMustBeGreaterThanOrEqualRequiredBinAllocation,
			"Cannot add row {0}. Quantity must be greater than or equals required of bin allocation"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinLocationDoesNotExistsInWhs,
			"Bin Location does not exist in the warehouse"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouCannotChangeScheduleOfPastDay,
			"You cannot change schedule of the past days"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResourceDoesNotExists,
			"Resource does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDuplicateResource,
			"Can not duplicate resource"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterRouteNo,
			"Enter Route No"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductionDoesNotExitst,
			"Production does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterRouteNoAndProductCode,
			"Enter Route No and Product Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ProductCodeIsNull,
			"Product Code is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OperationDoesNotExists,
			"Operation does not exist"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.QuantityOfParentItemMustBeGreaterThan0,
			"Quantity of parent item must be greater than zero"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BpCodeAlreadyExistsInTableWithCode,
			"BpCode already exist in the table. {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataImportFromFileDoesNotEnough,
			"Data import from file does not enough"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotAddBatchSerialQuantityMustBeGreaterOrEqualRequiredBatchSerialNumber,
			"Cannot add batch/serial {0}. Quantity must be greater than or equals required of batch/serial numbers"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheBatchSerialNumberAlreadyExists,
			"The batch/serial number {0}  already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoDataBatchSerialFoundWithLine,
			"No data batch/serial found. Line {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CheckBeforeSetDefault,
			"Check before set default"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotUncheckDefault,
			"Can not uncheck the default"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectItemToRemove,
			"Please select item to remove"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotEditCompleteAppointment,
			"Cannot edit complete appointment"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemLink2ProductionOrderCannotDelete,
			"Item link to production order, cannot delete"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotUpdateCompleteApointment,
			"Cannot update to complete appointment"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ResourceDoesNotAssign2ProduceThisItem,
			"Resource '{0}' does not assign to produce this item"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouNeedToSaveRowInfomationBeforeCreatingTheDocument,
			"You need to save row ItemCode = {0} information before creating the document"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.VersionUpdateCannotBeNull,
			"Version update cant not null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NewVersionUpdateCannotBeNull,
			"New Version update cannot null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotConnect2FtpServer,
			"Cannot connect to FTP server"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotCheck4UpdateClient,
			"Can not check for update client"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemInWhsExistsInGrid,
			"Item {0} in warehouse {1} exist in grid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemBinCodeWhsExistsInGrid,
			"Item {0}, BinCode {1} in warehouse {2} exist in grid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheBatchAlreadyExists,
			"The Batch {0} already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheSerialAlreadyExists,
			"The Serial {0} already exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheBatchDoesNotBeNull,
			"The Batch does not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheSerialDoesNotBeNull,
			"The Serial does not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BinCodeCannotBeNull,
			"BinCode cannot be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisItemCodeInWhsAlreadyExistsInFollowingGrid,
			"This ItemCode in WhsCode already exists in the following grid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AllocateBatchSerialQtyEquaItemQuantity,
			"Allocate batch/serial quantity equal item quantity"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteCloseRecord,
			"Can not delete Closed Record"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseSearchDate,
			"You must choose search date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeDoesNotExistsWithCode,
			"Item Code \"{0}\" does not exist or inactive"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveDataBeforeChanging,
			"Save data before changing"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SaveDataBeforeCopy,
			"Save data before copy"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BarcodeExists,
			"Barcode exists"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PeriodCodeMustNotBeNull,
			"Period Code must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PeriodNameMustNotBeNull,
			"Period Name must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NumberOfPeriodMustNotBeNull,
			"Number of period must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YearMustNotBeNull,
			"Year must not be null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoSalesEmployeeIsSelected,
			"No Employees is selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisPeriodHasBeenClosed,
			"This period has been closed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectedABranch,
			"Please select a branch"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectPeriod2Continue,
			"Please select Period to continue"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouHaveChooseWithColumnCaption,
			"You have not choose {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoSerialNumberTransactionFound,
			"No serial number transactions were found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskIsOnlyGenFor1Employee,
			"Task is only gen for 1 employee"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EmployeeDoesNotHaveAWorkSchedule,
			"Employee: {0} does not have a work schedule"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateIsNull,
			"FromDate is null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateDoesNotHaveWorkScheduleYet,
			"FromDate does not have a Work shedule yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ItemCodeIsBeingLinked2Document,
			"ItemCode: {0} is being linked to the document"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TaskCodeIsNullAtWbs,
			"TaskCode is null at WBS {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisDocumentHadBeenClose,
			"This document had been closed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LineHadBeenClose,
			"Line had been closed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisDocumentHadBeenCanceled,
			"This document had been canceled"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified,
			"The length of the string entered is larger than specified"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FirstYouShouldEditValueOfBpCodeAddFunctionNotSaveRowNullBpCode,
			"First you should edit the value of the BP Code. Add function will not save row with null value BP Code"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.OnlyTaskCanBeCopied,
			"Only TASK can be copied"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.LicenseIsZero,
			"License {0} is : 0"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.MaxLiceseNumber,
			"The max License is: "
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustMakeWorkScheduleBeforeMakingSchedule,
			"You must make work schedule before making schedule"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseSelectPranchBeforeContinue,
			"Please select branch before continue"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.WarningValueOnlyBetween0and1,
			"Warning! Value only between 0 and 1"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CaptionIsNotEmpty,
			"Caption is not Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.StoreIsNotEmpty,
			"Store is not Empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToPeriodIsEmpty,
			"To Period is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromPeriodIsEmpty,
			"From Period is empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PicturePathDoesNotExistsOnServerOrBadAuth,
			"Picture path does not exist on server or bad authentication"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseChooseItemForThisAction,
			"Please choose item for this aciton"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FormNotFound,
			"Form not found"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ColumnHaveBeenSelected,
			"Column have been selected"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotDownloadFile2Update,
			"Could not download file to update"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotExecuteXtraReportQuery,
			"Cannot Execute XtraReport Query"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseNumberOfCandidate,
			"You must choose the number of candidates"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ApplicantNoExistsInGrid,
			"ApplicantNo: {0} exist in grid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ApplicantNoExistsInTabInterviewHouHaveRemoveApplicentNoBeforeChangeStatus,
			"ApplicantNo: {0} exist in tab InterView.You have remove ApplicantNo before change status"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThereAreNoHiringYet,
			"There are no Hirring yet"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotReselectEnRollNoForExistedRecord,
			"Can not reselect EnrollNo for existed record"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotReselectedEmployee4ExistedRecord,
			"Can not reselect Employee for existed record"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseWarehouse2Counting,
			"Choose warehouse to counting"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BatchNumberHaveBeenCounting,
			"Batch number have been counting"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UrlNotValid,
			"url not valid"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotRemoveThisParamater,
			"Can not remove this parameter"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataMustBeSaveBeforeDuplicate,
			"Data must be saved before duplicate"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ContractNumAlreadyExistsInTableWithCode,
			"Contract num already exist in the table. {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UninstallPluginWithName,
			"Uninstall plugin {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PleaseActiveAnyForm,
			"Please active any form"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UninstallPluginPleaseWait,
			"Install plug in {0}. Please wait..."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YourClientIsLatest,
			"Your APZON-IRS client is the latest"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DownloadFileUpdatePleaseWait,
			"Download file update. Please wait..."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Failed,
			"Failed"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotHideThisComment,
			"Cannot hide this comment"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotUnhideThisCommnet,
			"Cannot unhide this comment"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisCommentAlreadyShow,
			"This comment is already shown"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ThisCommentAlreadyHide,
			"This comment is already hidden"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PostAdded,
			"{0} post(s) added."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Hidden,
			"Hidden"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Shown,
			"Shown"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Copied,
			"Copied"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.Assigned,
			"Assigned"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CustomerRefNoAlreadyExists,
			"Customer Ref. No. already exists in a document of this type. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.VendorRefNoAlreadyExists,
			"Vendor Ref. No. already exists in a document of this type. Continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AlternativeItemHaveNotBeenDefined4Item,
			"Alternative items have not been defined for item '{0}'"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoYouWantToApprovalThisDocument,
			"Do you want to approval this document?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoYouWantToApprovalTheSelectedDocuments,
			"Do you want to approval the selected documents?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoYouWantToRejectThisDocument,
			"Do you want to reject this document?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DoYouWantToRejectTheSelectedDocuments,
			"Do you want to reject the selected documents?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.EnterBaseUoM,
			"Enter Base UoM"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.AlternateUoMCannotEqualBaseUoM,
			"Alternate UoM cannot equal BaseUoM"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DataIsNotAvailable,
			"Data is not available"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CannotDeleteWHSublevelCode,
			"You cannot change or delete warehouse sublevel code; warehouse sublevel code is used in bin location codes"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.PushNotificationConfirm,
			"Are you sure you want to send this note as app notification?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseSaleEmployee,
			"You must choose Sales Employee"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ReportCannotBeNull,
			"Report field can't be null!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.YouMustChooseType,
			"You must choose Type"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CardCodeCannotBeEmpty,
			"Card Code cannot be empty"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CardCodeCannotBeDulplicated,
			"Card Code cannot be duplicated"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DuplicateItemSubstitute,
			"Item already defined as the original item's substitute"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCreateMultipleDocument,
			"Do you want to create document selected?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CodeNotNull,
			"Code is not null"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ValueExistsInTableWithId,
			"Value has already existed in the table: {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmCopyKpiSetting,
			"Do you want to apply this setting for selected employees?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmRemoveUdf,
			"Field removal is irreversible. Do you want to continue removing the field?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmClearHeaderFooter,
			"Document's body is empty. Do you want to clear document's header and document's footer?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateNotInPeriodRange,
			"From Date does not in period range"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ToDateNotInPeriodRange,
			"To Date does not in period range"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FromDateCanNotLargerThanToDate,
			"From Date cannot larger than To Date"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ChooseASettingBeforeCopy,
			"Please select a setting before copy"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoPriceDefined4PriceList,
			"No price defined for item in selected price list"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.SpecifiedNewTargetGroup,
			"You have specified a new Target Group. Do you want to replace all existing BPs in \"Target BP\" tab with the new Target Group?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.NoPermissionPerformAction,
			"You are not permitted to perform this action"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.BaseProjectPending,
			"System exists baseline project pending code is {0}"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmDeleteByLineNum,
			"Do you want to delete - LineNum: {0}?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.UpdateValueLineByValueHeader,
			"Do you want to update the value for the one route or all routes? YES - for one routes and NO - for all routes"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.DocumentHeaderAndDetailNotMatch,
			"Document header and detail do not match. Please reload the document!"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmOpenForLandedCost,
			"Landed Costs document has already been created for the items specified in this document. Do you want to continue?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmReloadMainMenu,
			"Do you really want to reload main menu?"
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.FormsettingNotAvailable,
			"Form setting is not available for this object."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.CouldNotGetDataPayment,
			"Could not get data payment."
		},
		{
			Apzon.Commons.Enums.Enums.MessageCode.ConfirmApplyBroker2Rows,
			"Do you want to copy selected broker {0} to all of the costs rows?"
		}
	};
}
