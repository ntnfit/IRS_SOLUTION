using System.Collections.Generic;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IInvoices : IRepository<DocumentsImpl>
{
	HttpResult CreateInvoices(List<DocumentsImpl> docs, string sessionId);
}
