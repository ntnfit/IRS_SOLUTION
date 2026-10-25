using System;
using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IGeneralSetting : IService<GeneralSettingImpl>
{
	DataTable ShowGeneralSetting();

	bool Check(string code, string codeP);

	DataTable GetFormatString();

	DataTable GetFileSharingUploadPath();

	DataSet GetData(int userId);

	DataTable GetDefaultPaymentTermforCustomer();

	DataTable GetDefaultPaymentMethodforCustomer();

	DataTable GetMapService();

	DataTable GetDfltWhs();

	string SaveData(Tuple<DataSet, int> tuple);

	DataTable GetAllGeneralSetting();

	DataTable GetObjectTypePath();

	string GetMainCurrency();

	DataTable GetLengthUnits();

	DataTable GetWeightUnits();
}
