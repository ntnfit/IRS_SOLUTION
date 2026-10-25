using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.HttpClientHelper;

public interface IBusinessMasterClient
{
	Task<DataTable> GetDataQuickBPMasterData(string cardCode);

	Task<DataTable> GetContactPersons(string cardType);

	DataTable GetBPGroup();

	Task<DataTable> GetADoc(string cardcode);

	Task<DataTable> ExistsStockTransaction(string cardCode);

	Task<DataTable> ExistsLinked(string cardCode);

	Task<DataTable> ExistsLinkedAll(string cardCode);

	Task<string> ValidateChangedValue(DataSet dsValidate);

	DataTable GetPaymentTerm(int groupNum);

	DataTable ShowPaymentTerm();

	string ExistLinkedContact(int cntctCode);

	string DeleteDoc(string cardCode);

	string DeleteMulti(List<string> lstCardCode);

	DataTable GetRecord(string p, string c, string u, int b);

	DataTable Search(DataTable dtSearch);

	string SaveDoc(BPImpls doc);

	string SaveDocDataSet(DataSet ds);

	string SaveDocDataSetUpdate(DataSet ds);

	Task<DataTable> GetBpBranchAssignment(string cardCode);

	bool CheckDoc(string cardCode);

	DataTable LoadComboPaymentTermPriority();

	DataTable LoadComboIndustry();

	DataTable LoadComboTechnician();

	DataTable LoadComboLanguage();

	DataTable LoadComboTerritory();

	DataTable ShowCardGroup(string groupType);

	DataTable LoadComboCountry();

	DataTable ShowCreditCardType();

	DataTable ShowShippingTypes();

	DataTable ShowPriceList();

	DataTable LoadPaymentDates(string cardCode);

	DataTable GetFactoryIndicatorName(string code);

	DataTable LoadBPBankAccount(string cardCode);

	DataTable ShowBankAccount();

	DataTable GetAddressByCardCode(string cardCode);

	DataTable ShowContactPerson(string code);

	DataTable GetSearchTableAddress();

	DataSet GetBPProperties(string cardCode);

	DataTable GetControlAccounts(string cardCode);

	DataTable GetAccountTabName(string fatherCard, string debPayAcct, string dpmClear, string dpmIntAct, string connBP);

	DataTable GetDefaultLanguage();

	DataTable LoadCBPaymentRunBank(string cryCode);

	DataTable LoadCBPaymentRunAccount(string bankCode, string cryCode);

	DataTable LoadCBPaymentRunBCACode();

	string SaveIgnoreEmpty(BPImpls doc);

	Task<DataSet> GetDataQuickBPPager(int pageNumber, int pageSize, DataTable searchData);

	Task<object> GetNumberOfRecordQuickBP();

	Task<DataTable> GetBpListDMS(string userName, string type = "M");
}
