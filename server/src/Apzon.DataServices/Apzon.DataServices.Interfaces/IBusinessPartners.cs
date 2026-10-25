using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IBusinessPartners : IRepository<BPImpls>
{
	HttpResult Save(BPImpls item, string sessionId, bool ignoreEmptyValue = false);

	HttpResult AddOrUpdate(BPImpls model, string sesionId, bool ignoreEmptyValue = false);

	string Remove(string itemCode, string sessionId);

	string RemoveMulti(List<string> lstCardCode, string sessionId);

	bool Exists(string itemCode, string sessionId);

	bool CheckExistsPriorities(int prioCode, string sessionId);

	HttpResult SaveBPProperties(DataTable dtDoc, string sessionId);

	HttpResult SaveBPPriorities(DataTable dt, string sessionId);

	HttpResult RemoveBPPriorities(DataTable dt, string sessionId);

	string MappingDis(BPImpls doc, EnumCommandType commandType, string sessionId);
}
