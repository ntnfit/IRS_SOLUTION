using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.Services.Interfaces.Payment;

public interface IOutgoingPayment : IService<IncomingPaymentsImpl>
{
	DataTable ShowOutgoingPayment();

	DataTable GetOutgoingPayment(string docentry);

	DataTable LoadUDFData(string docentry);

	string SaveDoc(IncomingPaymentsImpl doc);

	string SaveUDF(List<Common.Table_ID_Code> lst);

	DataTable GetAPInvoice(string CardCode, string Status);

	DataTable GetARInvoiceByOutgoingPayment(string docNum);

	DataTable GetAPInvoiceByOutgoingPayment(string docNum);

	DataTable GetPurposeOutgoingPayment(string docEntry);

	DataTable GetAccountPayment(string docNums);

	int GetMaxCode();

	DataTable GetNext(string docEntry);

	DataTable GetPrevious(string docEntry);

	DataTable GetFirst();

	DataTable GetLast();

	string DeleteDoc(string code);

	DataTable ShowPaymentMeansCurrency();

	DataTable ShowPaymentMeansCreditCardType();

	DataTable LoadPaymentMeansByOutgoing(string docNum);

	DataTable GetAll();
}
