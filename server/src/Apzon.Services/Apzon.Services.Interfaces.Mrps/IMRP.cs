using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Mrps;

public interface IMRP
{
	DataTable SP_APZ_SearchMRP(string mrpNo, DateTime fDate, DateTime tDate, string status);

	DataTable GetPlant();

	DataTable SP_APZ_GetMRP(int idMrp);

	int GetMaxDocEntry(string tableName);

	HttpResult Save(DataRow dataRow, Enums.TransactionType tranType, string objId);

	DataTable GetWarehouse(int branch);

	DataTable GetById(int id);

	string GetEndateForecast(string forecastCode);

	DataTable GetDataDoc(int id, Enums.NavigateType type);

	HttpResult ValidateScheduleForMrp(string lstPsCode);

	HttpResult GetNewIdMrp();

	DataTable GetSourceCellDataDocumentType();

	DataTable LoadDataSourceCell(string demandType, int demandEntry, int demandLine);
}
