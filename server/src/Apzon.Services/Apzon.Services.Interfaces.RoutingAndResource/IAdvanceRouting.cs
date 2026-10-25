using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface IAdvanceRouting
{
	DataTable GetDataDoc(Tuple<DataTable, Enums.NavigateType> tuple);

	DataSet GetDataLine(string routCode);

	HttpResult SaveData(Tuple<DataSet, Enums.TransactionType> tuple);

	HttpResult DeleteDataDoc(string routCode);

	HttpResult ImportFromFile(DataSet dtSet);
}
