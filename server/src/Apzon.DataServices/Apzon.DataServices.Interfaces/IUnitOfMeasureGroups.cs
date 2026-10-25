using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IUnitOfMeasureGroups
{
	HttpResult SaveData(Tuple<DataTable, DataTable, Enums.HttpAction> tupData, string sessionId);
}
