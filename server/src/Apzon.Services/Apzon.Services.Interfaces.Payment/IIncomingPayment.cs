using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.Services.Interfaces.Payment;

public interface IIncomingPayment : IService<IncomingPaymentsImpl>
{
	DataTable ShowIncomingPayment();

	DataTable GetIncomingPayment(string docentry);

	DataTable LoadUDFData(string docentry);

	string SaveDoc(IncomingPaymentsImpl doc);

	string SaveUDF(List<Common.Table_ID_Code> lst);

	DataTable GetARInvoice(string CardCode, string Status);

	DataTable GetARInvoiceByIncomingPayment(string docNum);

	DataTable GetAPInvoiceByIncomingPayment(string docNum);

	DataTable GetAccountPayment(string docNums);

	int GetMaxCode();

	DataTable GetPurposeIncomingPayment(string docEntry);

	DataTable GetNext(string docEntry);

	DataTable GetPrevious(string docEntry);

	DataTable GetFirst();

	DataTable GetLast();

	string DeleteDoc(string code);

	DataTable ShowPaymentMeansCurrency();

	DataTable ShowPaymentMeansCreditCardType();

	DataTable Cfl();

	DataTable LoadPaymentMeansByIncoming(string docNum);

	DataTable GetByInvoiceNo(string barcode);

	DataSet GetListInvoice(DataTable dtSearch);

	DataSet GetListPayment(DataTable dtSearch);

	DataTable GetAll();
}
