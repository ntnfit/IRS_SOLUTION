using System;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IFormFilterSetting
{
	DataTable GetDataFilterField(ApzObjectTypes objectType);

	DataTable GetDataFilterSetting(ApzObjectTypes objectType);

	HttpResult SaveFilterSetting(Tuple<DataSet, ApzObjectTypes> tuple);

	HttpResult DeleteFilterSetting(int docEntry);

	HttpResult ChangeSourceType(DataTable dt);

	HttpResult SaveFilterSettingAddNew(Tuple<DataTable, ApzObjectTypes, DataRow> tuple);

	DataTable GetFilterStructure();

	DataTable GetDataFilterValue(int docEntry);

	DataTable GetDataFilterSettingLine(int docEntry);

	HttpResult SaveFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple);

	HttpResult SaveFilterSettingLineAddNew(Tuple<DataTable, ApzObjectTypes> tuple);

	HttpResult DeleteFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple);

	DataTable GetFilterLineStructure();

	DataTable GetDataFilterSettingTable(int docEntry);

	DataTable GetDataTableByTableName(string tableName);

	HttpResult SaveFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple);

	HttpResult DeleteFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple);

	DataTable GetFilterTableStructure();
}
