using System.Collections.Generic;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IDelivery : IRepository<DocumentsImpl>
{
	HttpResult CreateDelivery(List<DocumentsImpl> docs, string sessionId);
}
