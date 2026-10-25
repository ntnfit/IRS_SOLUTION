using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFormFilterSettingClient
{
	DataTable GetDataFilterField(ApzObjectTypes objectType);

	Task<DataTable> GetDataFilterSetting(ApzObjectTypes objectType);

	Task<HttpResult> SaveFilterSetting(Tuple<DataSet, ApzObjectTypes> tuple);

	Task<HttpResult> DeleteFilterSetting(int docEntry);

	Task<HttpResult> SaveFilterSettingAddNew(Tuple<DataTable, ApzObjectTypes, DataRow> tuple);

	Task<HttpResult> ChangeSourceType(DataTable dt);

	Task<DataTable> GetFilterStructure();

	Task<DataTable> GetDataFilterValue(int docEntry);

	Task<DataTable> GetDataFilterSettingLine(int docEntry);

	Task<HttpResult> SaveFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple);

	Task<HttpResult> SaveFilterSettingLineAddNew(Tuple<DataTable, ApzObjectTypes> tuple);

	Task<HttpResult> DeleteFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple);

	Task<DataTable> GetFilterLineStructure();

	Task<DataTable> GetDataFilterSettingTable(int docEntry);

	Task<DataTable> GetDataTableByTableName(string tableName);

	Task<HttpResult> SaveFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple);

	Task<HttpResult> DeleteFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple);

	Task<DataTable> GetFilterTableStructure();
}
