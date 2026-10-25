using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class FormFilterSettingController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataFilterField(ApzObjectTypes objectType)
	{
		return UnitOfWork.FormFilterSetting.GetDataFilterField(objectType);
	}

	[HttpGet]
	public DataTable GetDataFilterSetting(ApzObjectTypes objectType)
	{
		return UnitOfWork.FormFilterSetting.GetDataFilterSetting(objectType);
	}

	[HttpPost]
	public HttpResult SaveFilterSetting([FromBody] Tuple<DataSet, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.SaveFilterSetting(tuple);
	}

	[HttpPost]
	public HttpResult DeleteFilterSetting([FromBody] int docEntry)
	{
		return UnitOfWork.FormFilterSetting.DeleteFilterSetting(docEntry);
	}

	[HttpPost]
	public HttpResult ChangeSourceType(DataTable dt)
	{
		return UnitOfWork.FormFilterSetting.ChangeSourceType(dt);
	}

	[HttpPost]
	public HttpResult SaveFilterSettingAddNew([FromBody] Tuple<DataTable, ApzObjectTypes, DataRow> tuple)
	{
		return UnitOfWork.FormFilterSetting.SaveFilterSettingAddNew(tuple);
	}

	[HttpGet]
	public DataTable GetFilterStructure()
	{
		return UnitOfWork.FormFilterSetting.GetFilterStructure();
	}

	[HttpGet]
	public DataTable GetDataFilterValue(int docEntry)
	{
		return UnitOfWork.FormFilterSetting.GetDataFilterValue(docEntry);
	}

	[HttpGet]
	public DataTable GetDataFilterSettingLine(int docEntry)
	{
		return UnitOfWork.FormFilterSetting.GetDataFilterSettingLine(docEntry);
	}

	[HttpPost]
	public HttpResult SaveFilterSettingLine([FromBody] Tuple<DataTable, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.SaveFilterSettingLine(tuple);
	}

	[HttpPost]
	public HttpResult SaveFilterSettingLineAddNew([FromBody] Tuple<DataTable, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.SaveFilterSettingLineAddNew(tuple);
	}

	[HttpPost]
	public HttpResult DeleteFilterSettingLine([FromBody] Tuple<DataTable, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.DeleteFilterSettingLine(tuple);
	}

	[HttpGet]
	public DataTable GetFilterLineStructure()
	{
		return UnitOfWork.FormFilterSetting.GetFilterLineStructure();
	}

	[HttpGet]
	public DataTable GetDataFilterSettingTable(int docEntry)
	{
		return UnitOfWork.FormFilterSetting.GetDataFilterSettingTable(docEntry);
	}

	[HttpGet]
	public DataTable GetDataTableByTableName(string tableName)
	{
		return UnitOfWork.FormFilterSetting.GetDataTableByTableName(tableName);
	}

	[HttpPost]
	public HttpResult SaveFilterSettingTable([FromBody] Tuple<DataTable, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.SaveFilterSettingTable(tuple);
	}

	[HttpPost]
	public HttpResult DeleteFilterSettingTable([FromBody] Tuple<DataTable, ApzObjectTypes> tuple)
	{
		return UnitOfWork.FormFilterSetting.DeleteFilterSettingTable(tuple);
	}

	[HttpGet]
	public DataTable GetFilterTableStructure()
	{
		return UnitOfWork.FormFilterSetting.GetFilterTableStructure();
	}
}
