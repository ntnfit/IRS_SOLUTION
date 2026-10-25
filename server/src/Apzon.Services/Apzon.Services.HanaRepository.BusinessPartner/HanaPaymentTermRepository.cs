using System;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaPaymentTermRepository(IDatabaseService databaseService) : PaymentTermRepository(databaseService)
{
	private string[] _arrStr = new string[39]
	{
		"OCPI;GroupNum;A/P Correction Invoice", "OCPV;GroupNum;A/P Correction Invoice Reversal", "ORPC;GroupNum;A/P Credit Memo", "ODPO;GroupNum;A/P Down Payment", "OPCH;GroupNum;A/P Invoice", "ACRD;GroupNum;Business Partners - History", "OCSI;GroupNum;A/R Correction Invoice", "OCIN;GroupNum;A/R Correction Invoice", "OCSV;GroupNum;A/R Correction Invoice Reversal", "ORIN;GroupNum;A/R Credit Memo",
		"ODPI;GroupNum;A/R Down Payment", "OINV;GroupNum;A/R Invoice", "OOAT;GroupNum;Blanket Agreement", "AOAT;GroupNum;Blanket Agreement", "OCRD;GroupNum;Business Partner", "ODLN;GroupNum;Delivery", "ODRF;GroupNum;Drafts", "OIGE;GroupNum;Goods Issue", "OIGN;GroupNum;Goods Receipt", "OPDN;GroupNum;Goods Receipt PO",
		"ORPD;GroupNum;Goods Return", "OIEI;GroupNum;Incoming Excise Invoice", "CTG1;CTGCode;Installment Layout", "OWTQ;GroupNum;Inventory Transfer Request", "ADOC;GroupNum;Invoice - History", "OSAL;GroupNum;Outgoing", "OOEI;GroupNum;Outgoing Excise Invoice", "OPYM;PaymTerms;Payment Methods for Payment Wizard", "PEX1;DocPrmTerm;Payment Results Table - Rows", "OPOR;GroupNum;Purchase Order",
		"OPRQ;GroupNum;Purchase Request", "ORDN;GroupNum;Returns", "ORDR;GroupNum;Sales Order", "OQUT;GroupNum;Sales Quotation", "OENT;GroupNum;Shipping Types", "AWH1;PmntTerms;Tax Definition", "OTRA;GroupNum;Transition", "WHT1;PmntTerms;Withholding Tax Definition", "OPRQ;GroupNum;Purchase Request"
	};

	public override string GetCheckExistString(int groupNum)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("declare groupNum int=" + groupNum + ";" + Environment.NewLine + "declare rowCount int=0;" + Environment.NewLine);
			for (int i = 0; i < _arrStr.Length; i++)
			{
				string[] array = _arrStr[i].Split(';');
				if (array != null && array.Length == 3)
				{
					string text = array[0];
					string text2 = array[1];
					string value = array[2];
					stringBuilder.Append(Environment.NewLine + "SELECT(SELECT 1 FROM ");
					stringBuilder.Append(text);
					stringBuilder.Append(" t WHERE t.");
					stringBuilder.Append("\"" + text2 + "\"");
					stringBuilder.Append("=groupNum) into rowCount from DUMMY;");
					stringBuilder.Append(Environment.NewLine + "IF rowCount > 0 then");
					stringBuilder.Append(Environment.NewLine + " SELECT 'Cannot remove payment term, linked ");
					stringBuilder.Append(value);
					stringBuilder.Append(" exist - ");
					stringBuilder.Append(text + "'");
					stringBuilder.Append(" FROM DUMMY;");
					stringBuilder.Append(Environment.NewLine + "END IF;");
				}
			}
			return stringBuilder.Insert(0, "OK-").ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR-" + ex.Message;
		}
	}
}
