using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;

namespace Apzon.DataServices.Interfaces;

public interface IProductionOrders : IRepository<ProductionOder_Impl_OWOR>
{
	HttpResult Save(ProductionOder_Impl_OWOR po, string sessionId);

	string Remove(int entry, string sessionId);

	bool Exists(int entry, string sessionId);

	Task<HttpResult> SaveList(List<ProductionOder_Impl_OWOR> oDocs, string sessionId);

	Task<HttpResult> SaveList(List<Tuple<DataTable, DataTable>> lstDocs, string sessionId);

	HttpResult UpdateStatus(List<ProductionOder_Impl_OWOR> oDocs, string sessionId);

	Task<HttpResult> ReceiptForBatch(Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable> model, string sessionId);

	Task<HttpResult> ConfirmProduction(Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable, string> model, string sessionId);

	HttpResult SaveProPlanning(List<ProductionOder_Impl_OWOR> po, string sessionId);

	string UpdateStatusByDI(int docEntry, string status, string sessionId);

	Task<HttpResult> PackingBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId);

	Task<HttpResult> ConfirmBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model, string sessionId);

	string MappingXml(ProductionOder_Impl_OWOR model, EnumCommandType commandType, bool mapAttachment = false);

	HttpResult SaveProductionPlan(Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable> model, string sessionId);

	HttpResult Adjust(DocumentsImpl[] docs, string sessionId);

	Task<HttpResult> SaveDocsPrdImportTools(List<Tuple<DataTable, DataTable>> lstDocs, string sessionId);

	HttpResult CancelDoc(int docEntry, string sessionId);
}
