using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Kit.ContextAction;

public interface IContextActionHandle
{
	ContextActionQuery GetQuery(DataSet ds, DataTable inputSource);

	DataSet GetSourceTask(DataSet ds, DataTable source, DataTable dtStruct);

	List<DocumentsImpl> GetMappingDocument(DataTable source, DataSet ds);

	List<ItemsImpl> GetMappingItem(DataTable source, DataSet dsAction);

	List<BPImpls> GetMappingBusinesPartner(DataTable source, DataSet dsAction);

	List<ChartOfAccountsImpl> GetMappingChartOfAccount(DataTable source, DataSet dsAction);

	List<DataSet> GetMappingDistributeRule(DataTable source, DataSet dsAction);

	List<VatGroupsImpl> GetMappingVatGroups(DataTable source, DataSet dsAction);

	ApzObjectTypes GetObjectTypesFromContext(Enums.ContextActionDocumentType objtype);

	List<DataSet> GetMappingUserDefinedObject(DataTable source, DataSet dsAction);

	List<JournalEntriesImpl> GetMappingJournalEntry(DataTable source, DataTable dtMapping);

	DataTable GetmappingSourceSaleEmployee(DataTable dtStruct, DataTable source, DataTable dtMapping);
}
