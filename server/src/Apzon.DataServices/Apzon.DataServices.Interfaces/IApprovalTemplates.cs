using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IApprovalTemplates
{
	string Mapping(DataSet ds, EnumCommandType commandType, string sessionId);

	HttpResult Save(Tuple<DataSet, string> tuple, string sessionId);

	HttpResult Delete(int templateCode, string sessionId);
}
