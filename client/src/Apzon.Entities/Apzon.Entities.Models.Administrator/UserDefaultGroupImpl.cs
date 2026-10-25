using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Administrator;

public class UserDefaultGroupImpl
{
	public string AdditionalIdNumber { get; set; }

	public string Address { get; set; }

	public string AddressinForeignLanguage { get; set; }

	public bool AssetInDoc { get; set; }

	public int BPLID { get; set; }

	public string BPforInvoicePayment { get; set; }

	public string CashAccount { get; set; }

	public string CheckingAcct { get; set; }

	public string Code { get; set; }

	public string Country { get; set; }

	public List<DefaultCreditCardsImpl> DefaultCreditCards { get; set; }

	public List<DefaultDocumentsImpl> DefaultDocuments { get; set; }

	public string DefaultTaxCode { get; set; }

	public string FaxNumber { get; set; }

	public string FaxNumberForeignLang { get; set; }

	public string Name { get; set; }

	public string PhoneNumber1 { get; set; }

	public string PhoneNumber1ForeignLang { get; set; }

	public string PhoneNumber2 { get; set; }

	public string PhoneNumber2ForeignLang { get; set; }

	public bool PrintInvoiceandPaymentinS { get; set; }

	public BoPrintReceiptEnum PrintReceipt { get; set; }

	public string PrintingHeader { get; set; }

	public string PrintingHeaderInForeignLangu { get; set; }

	public int SalesEmployee { get; set; }

	public bool UseTax { get; set; }

	public bool UseWarehouseAddressinAPD { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public int UserSignature { get; set; }

	public string Warehouse { get; set; }

	public int WindowsColor { get; set; }

	public string eMail { get; set; }
}
