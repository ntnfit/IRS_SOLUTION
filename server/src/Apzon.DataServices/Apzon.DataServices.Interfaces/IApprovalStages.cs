using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IApprovalStages
{
	string Mapping(DataRow document, DataTable documentLine, EnumCommandType commandType, string sessionId);

	HttpResult Save(Tuple<DataTable, DataTable, string> tuple, string sessionId);

	HttpResult Delete(int stageCode, string sessionId);
}
