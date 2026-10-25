using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface IDMSDelivery
{
	DataTable GetListDeliveryHistory(string userCode, string lstCardCode, string lstConfirmStatus, string fromDate, string toDate, int pageIndex);

	DataSet SearchDeliveryOrder(DataTable dt);

	DataSet GetLines(int docentry);

	HttpResult Save(DataSet ds, string action);

	DataTable GetListDeliveryOrder(string userCode, DateTime date, string status);

	DataSet GetDeliveryOrderDetails(string docentry, string cardcode);

	DataTable GetLocationNote(string cardcode, string shipCode);

	DataTable GetBPNote(string cardCode);

	ResponseResult SaveLocationNote(DataTable dt, Enums.TransactionType tranType = Enums.TransactionType.A);

	ResponseResult SaveBPNote(DataTable dt, Enums.TransactionType tranType = Enums.TransactionType.A);

	ResponseResult ConfirmLateTime(string docentry, string cardcode, DateTime newTime, string reason);

	ResponseResult ConfirmDelivery(DataTable dt);
}
