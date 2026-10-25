using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IBusinessMasterDatas
{
	DataTable Table();

	List<ItemsImpl> Cfl();

	DataTable Search(DataTable doc, string userCode);

	DataTable ShowCreditCardType();

	DataTable LoadUDFData(string cardCode);

	bool CheckDoc(string itemCode);

	string DeleteDoc(string itemCode);

	DataTable GetRecord(string position, string cardCode, string userCode, int BPLId);

	DataTable GetDataQuickBPMasterData(string cardCode);

	DataTable GetBPByCardCode(string CardCode, string userCode, int BPLId);

	DataTable GetBPByCardCodeWithAddress(string CardCode, string userCode, int BPLId);

	DataTable ShowCardGroup(string groupType);

	DataTable LoadUDFDataSAP(string cardCode);

	DataTable CheckExistCardCode(string CardCode);

	DataTable GetEmployeeCodeBySlpCode(int slpCode);

	DataTable GetVatByCardCode(string cardCode);

	DataTable GetAddressByCardCode(string cardCode);

	DataTable GetListAddressByCardCode(string cardCode);

	DataTable GetSearchTableAddress();

	DataTable GetADoc(string CardCode, string userName);

	DataTable GetContactPersons(string cardType);

	DataSet GetBPProperties(string cardCode);

	DataTable ExistFileInDoc(string cardCode, string fileName, string fileExt);

	DataTable GetBPTableStructure(string table);

	string SaveDocDataSet(DataSet ds, string action);

	DataTable LoadComboCountry();

	DataTable ShowBankAccount();

	DataTable IsValidBankCode(string bankCode);

	DataTable LoadBPBankAccount(string cardCode);

	DataTable LoadComboIndustry();

	DataTable LoadComboTerritory();

	DataTable LoadComboLanguage();

	DataTable LoadComboTechnician();

	DataTable GetFactoryIndicatorName(string Code);

	DataTable LoadComboPaymentTermPriority();

	DataTable LoadComboPaymentTermHolidays();

	DataTable LoadPaymentDates(string cardCode);

	DataTable GetBPGroup();

	DataTable ExistsStockTransaction(string cardCode);

	DataTable ExistsLinked(string cardCode);

	string ValidateChangedValue(DataSet ds);

	DataTable ExistsLinkedAll(string cardCode);

	DataTable GetPaymentTerm(int groupNum);

	DataTable ShowPaymentTerm();

	string ExistLinkedContact(int cntctCode);

	DataTable GetControlAccounts(string cardCode);

	DataTable GetAccountTabName(string fatherCard, string debPayAcct, string dpmClear, string dpmIntAct, string connBP);

	DataTable GetDefaultLanguage();

	DataTable LoadCBPaymentRunBank(string cryCode);

	DataTable LoadCBPaymentRunAccount(string bankCode, string cryCode);

	DataTable LoadCBPaymentRunBCACode();

	DataTable GetBpBranchAssignment(string cardCode);

	ResponseResult GetListStore(GetListStoreParamsImpl data);

	DataTable GetBpList(string cardType, string cardName, string subType, string saleUser, string type = "");

	DataTable GetDetailBP(string cardCode, string userCode);

	DataTable GetDetailBPRevenue(string cardCode, string userCode);

	DataSet GetDataQuickBPPager(int pageNumber, int pageSize, DataTable searchData);

	object GetNumberOfRecordQuickBP();

	DataTable GetActivityByCardCode(string cardCode);

	DataTable DMSGetDataCardType();

	DataTable DMSGetDataPropertiesBusinessType();

	DataSet DMSGetDetailAllStore(string userName);

	DataTable CheckPotential(string cardCode);
}
