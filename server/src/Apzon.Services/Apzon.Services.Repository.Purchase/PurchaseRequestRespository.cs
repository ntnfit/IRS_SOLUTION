using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Purchase;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Purchase;

public class PurchaseRequestRespository : BaseService<DocumentsImpl>, IPurchaseRequest, IService<DocumentsImpl>
{
	public PurchaseRequestRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string SaveAsDraft(DocumentsImpl model, ApzObjectTypes apzObjectTypes)
	{
		string text = "";
		try
		{
			Documents documents = (Documents)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("4AFE074D-4F31-4E7B-87DF-64D55098DA5E")));
			bool byKey = documents.GetByKey(model.DocNum);
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oPurchaseRequest:
				documents.DocObjectCode = BoObjectTypes.oPurchaseRequest;
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				documents.DocObjectCode = BoObjectTypes.oPurchaseQuotations;
				break;
			case ApzObjectTypes.oPurchaseOrders:
				documents.DocObjectCode = BoObjectTypes.oPurchaseOrders;
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				documents.DocObjectCode = BoObjectTypes.oPurchaseDeliveryNotes;
				break;
			case ApzObjectTypes.oPurchaseReturns:
				documents.DocObjectCode = BoObjectTypes.oPurchaseReturns;
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				documents.DocObjectCode = BoObjectTypes.oPurchaseInvoices;
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				documents.DocObjectCode = BoObjectTypes.oPurchaseCreditNotes;
				break;
			}
			if (documents.DocObjectCode != BoObjectTypes.oPurchaseRequest)
			{
				documents.CardCode = model.CardCode;
				documents.CardName = model.CardName;
				if (model.CntctCode > 0)
				{
					documents.ContactPersonCode = model.CntctCode;
				}
				if (!string.IsNullOrEmpty(model.NumAtCard))
				{
					documents.NumAtCard = model.NumAtCard.Trim();
				}
				if (!string.IsNullOrEmpty(model.DocCur))
				{
					documents.DocCurrency = model.DocCur;
					documents.DocRate = (model.DocCur.Equals("VND") ? 1.0 : Function.ParseDouble(documents.DocRate));
				}
			}
			else
			{
				documents.ReqType = model.ReqType;
				documents.Requester = model.Requester;
				if (!string.IsNullOrEmpty(model.RequesterName))
				{
					documents.RequesterName = model.RequesterName;
				}
				documents.RequesterBranch = model.RequesterBranch;
				documents.RequesterDepartment = model.RequesterDepartment;
				if (model.SendNotification == BoYesNoEnum.tYES)
				{
					documents.SendNotification = BoYesNoEnum.tYES;
				}
				else
				{
					documents.SendNotification = BoYesNoEnum.tNO;
				}
				if (!string.IsNullOrEmpty(model.Email))
				{
					documents.RequesterEmail = model.Email;
				}
			}
			documents.Series = model.Series;
			_ = model.DocDate;
			documents.DocDate = Convert.ToDateTime(model.DocDate);
			_ = model.TaxDate;
			documents.TaxDate = Convert.ToDateTime(model.TaxDate);
			_ = model.ReqDate;
			documents.RequriedDate = Convert.ToDateTime(model.ReqDate);
			_ = model.DocDueDate;
			documents.DocDueDate = Convert.ToDateTime(model.DocDueDate);
			documents.SummeryType = model.SummeryType;
			if (model.SlpCode != 0)
			{
				documents.SalesPersonCode = Function.ParseInt(model.SlpCode);
			}
			if (model.DocumentsOwner > 0)
			{
				documents.DocumentsOwner = Function.ParseInt(model.DocumentsOwner);
			}
			documents.Comments = model.Comments;
			documents.DiscountPercent = Function.ParseDouble(model.DiscPrcnt);
			documents.RoundingDiffAmount = Function.ParseDouble(model.RoundingDiffAmount);
			if (documents.DocObjectCode != BoObjectTypes.oPurchaseRequest)
			{
				if (!string.IsNullOrEmpty(model.ShipToCode))
				{
					documents.ShipToCode = model.ShipToCode;
				}
				if (!string.IsNullOrEmpty(model.Address2))
				{
					documents.Address2 = model.Address2;
				}
				if (!string.IsNullOrEmpty(model.PayToCode))
				{
					documents.PayToCode = model.PayToCode;
				}
				if (!string.IsNullOrEmpty(model.Address))
				{
					documents.Address = model.Address;
				}
				documents.TransportationCode = model.TrnspCode;
				documents.LanguageCode = Function.ParseInt(model.LanguageCode);
				if (!string.IsNullOrEmpty(model.TrackingNumber))
				{
					documents.TrackingNumber = model.TrackingNumber;
				}
				if (!string.IsNullOrEmpty(model.PickRmrk))
				{
					documents.PickRemark = model.PickRmrk;
				}
				if (!string.IsNullOrEmpty(model.BPChCode))
				{
					documents.BPChannelCode = model.BPChCode;
				}
				if (model.BPChCntc > 0)
				{
					documents.BPChannelContact = model.BPChCntc;
				}
				documents.JournalMemo = model.JrnlMemo;
				if (!string.IsNullOrEmpty(model.Project))
				{
					documents.Project = model.Project;
				}
				if (model.GroupNumber > 0)
				{
					documents.GroupNumber = model.GroupNumber;
				}
				if (!string.IsNullOrEmpty(model.PeyMethod))
				{
					documents.PaymentMethod = model.PeyMethod;
				}
				if (!string.IsNullOrEmpty(model.CntrlBnk))
				{
					documents.CentralBankIndicator = model.CntrlBnk;
				}
				documents.CashDiscountDateOffset = Function.ParseInt(model.CdcOffset);
				if (!string.IsNullOrEmpty(model.PayToBankAccountNo))
				{
					documents.PayToBankAccountNo = model.PayToBankAccountNo;
				}
				if (!string.IsNullOrEmpty(model.PayToBankBranch))
				{
					documents.PayToBankBranch = model.PayToBankBranch;
				}
				if (!string.IsNullOrEmpty(model.PayToBankCountry))
				{
					documents.PayToBankCountry = model.PayToBankCountry;
				}
				if (!string.IsNullOrEmpty(model.PayToBankCode))
				{
					documents.PayToBankCode = model.PayToBankCode;
				}
				if (!string.IsNullOrEmpty(model.Indicator))
				{
					documents.Indicator = model.Indicator;
				}
				if (!string.IsNullOrEmpty(model.LicTradNum))
				{
					documents.FederalTaxID = model.LicTradNum;
				}
				if (model.ImportFileNum > 0)
				{
					documents.ImportFileNum = model.ImportFileNum;
				}
				if (model.ExtraMonth != 0)
				{
					documents.ExtraMonth = model.ExtraMonth;
				}
				if (model.ExtraDays != 0)
				{
					documents.ExtraDays = model.ExtraDays;
				}
				_ = model.CancelDate;
				documents.CancelDate = Convert.ToDateTime(model.CancelDate);
			}
			if (model.Lines != null && model.Lines.Count > 0)
			{
				for (int i = 0; i < model.Lines.Count; i++)
				{
					Document_LinesImpl document_LinesImpl = model.Lines[i];
					Logging.Write("Log Line Num ", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LineNum= ", model.Lines[i].LineNum.ToString());
					if (document_LinesImpl.LineNum == -1)
					{
						documents.Lines.Add();
						if (i == 0)
						{
							document_LinesImpl.LineNum = 0;
						}
						else
						{
							document_LinesImpl.LineNum = model.Lines[i - 1].LineNum + 1;
						}
					}
					documents.Lines.SetCurrentLine(i);
					if (!string.IsNullOrEmpty(document_LinesImpl.ItemCode))
					{
						documents.Lines.ItemCode = document_LinesImpl.ItemCode;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Dscription))
					{
						documents.Lines.ItemDescription = document_LinesImpl.Dscription;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Text))
					{
						documents.Lines.ItemDetails = document_LinesImpl.Text;
					}
					documents.Lines.Quantity = Function.ParseDouble(document_LinesImpl.Quantity);
					if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
					{
						documents.Lines.WarehouseCode = document_LinesImpl.WhsCode;
					}
					documents.Lines.UnitPrice = Function.ParseDouble(document_LinesImpl.UnitPrice);
					documents.Lines.Price = Function.ParseDouble(document_LinesImpl.Price);
					if (!string.IsNullOrEmpty(document_LinesImpl.Project))
					{
						documents.Lines.ProjectCode = document_LinesImpl.Project;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
					{
						documents.Lines.Currency = document_LinesImpl.Currency;
						documents.Lines.Rate = Function.ParseDouble(document_LinesImpl.Rate);
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.ShipToCode))
					{
						documents.Lines.ShipToCode = document_LinesImpl.ShipToCode;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.ShipToDesc))
					{
						documents.Lines.ShipToDescription = document_LinesImpl.ShipToDesc;
					}
					if (document_LinesImpl.TrnsCode != 0)
					{
						documents.Lines.ShippingMethod = Function.ParseInt(document_LinesImpl.TrnsCode);
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.TaxCode))
					{
						documents.Lines.TaxCode = document_LinesImpl.TaxCode;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.FreeText))
					{
						documents.Lines.FreeText = document_LinesImpl.FreeText;
					}
					documents.Lines.DiscountPercent = Function.ParseDouble(document_LinesImpl.DiscPrcnt);
					if (document_LinesImpl.AgrNo != 0)
					{
						documents.Lines.AgreementNo = document_LinesImpl.AgrNo;
					}
					documents.Lines.LineStatus = BoStatus.bost_Open;
					if (!string.IsNullOrEmpty(document_LinesImpl.unitMsr))
					{
						documents.Lines.MeasureUnit = document_LinesImpl.unitMsr;
					}
					if (document_LinesImpl.UomEntry != 0)
					{
						documents.Lines.UoMEntry = Function.ParseInt(document_LinesImpl.UomEntry);
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Usage))
					{
						documents.Lines.Usage = document_LinesImpl.Usage;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.VatGroup))
					{
						documents.Lines.VatGroup = document_LinesImpl.VatGroup;
					}
					if (document_LinesImpl.TransportMode != 0)
					{
						documents.Lines.TransportMode = Function.ParseInt(document_LinesImpl.TransportMode);
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.SupplierCatNum))
					{
						documents.Lines.SupplierCatNum = document_LinesImpl.SupplierCatNum;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.SerialNum))
					{
						documents.Lines.SerialNum = document_LinesImpl.SerialNum;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.AcctCode))
					{
						documents.Lines.AccountCode = document_LinesImpl.AcctCode;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Address))
					{
						documents.Lines.Address = document_LinesImpl.Address;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CogsAcct))
					{
						documents.Lines.COGSAccountCode = document_LinesImpl.CogsAcct;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CostingCode))
					{
						documents.Lines.CostingCode = document_LinesImpl.CostingCode;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CostingCode2))
					{
						documents.Lines.CostingCode2 = document_LinesImpl.CostingCode2;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CostingCode3))
					{
						documents.Lines.CostingCode3 = document_LinesImpl.CostingCode3;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CostingCode4))
					{
						documents.Lines.CostingCode4 = document_LinesImpl.CostingCode4;
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.CostingCode5))
					{
						documents.Lines.CostingCode5 = document_LinesImpl.CostingCode5;
					}
					if (documents.DocObjectCode == BoObjectTypes.oPurchaseRequest)
					{
						documents.Lines.LineVendor = document_LinesImpl.LineVendor;
					}
					if (documents.DocObjectCode == BoObjectTypes.oPurchaseRequest)
					{
						continue;
					}
					if (document_LinesImpl.BatchNumbers != null && document_LinesImpl.BatchNumbers.Count > 0)
					{
						for (int j = 0; j < document_LinesImpl.BatchNumbers.Count; j++)
						{
							documents.Lines.BatchNumbers.Add();
							documents.Lines.BatchNumbers.SetCurrentLine(j);
							BatchNumbersImpl batchNumbersImpl = document_LinesImpl.BatchNumbers[j];
							documents.Lines.BatchNumbers.BatchNumber = batchNumbersImpl.BatchNumber;
							documents.Lines.BatchNumbers.Quantity = batchNumbersImpl.Quantity;
							if (batchNumbersImpl.ManufacturerSerialNumber != null)
							{
								documents.Lines.BatchNumbers.ManufacturerSerialNumber = batchNumbersImpl.ManufacturerSerialNumber;
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								documents.Lines.BatchNumbers.InternalSerialNumber = batchNumbersImpl.InternalSerialNumber;
							}
							if (batchNumbersImpl.ExpiryDate.HasValue)
							{
								documents.Lines.BatchNumbers.ExpiryDate = batchNumbersImpl.ExpiryDate.Value;
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue)
							{
								documents.Lines.BatchNumbers.ManufacturingDate = batchNumbersImpl.ManufacturingDate.Value;
							}
							if (batchNumbersImpl.AddmisionDate.HasValue)
							{
								documents.Lines.BatchNumbers.AddmisionDate = batchNumbersImpl.AddmisionDate.Value;
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								documents.Lines.BatchNumbers.Location = batchNumbersImpl.Location;
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								documents.Lines.BatchNumbers.Notes = batchNumbersImpl.Notes;
							}
						}
					}
					if (document_LinesImpl.SerialNumbers == null || document_LinesImpl.SerialNumbers.Count <= 0)
					{
						continue;
					}
					for (int k = 0; k < document_LinesImpl.SerialNumbers.Count; k++)
					{
						documents.Lines.SerialNumbers.Add();
						documents.Lines.SerialNumbers.SetCurrentLine(k);
						SerialNumbersImpl serialNumbersImpl = document_LinesImpl.SerialNumbers[k];
						documents.Lines.SerialNumbers.SystemSerialNumber = serialNumbersImpl.SystemSerialNumber;
						if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
						{
							documents.Lines.SerialNumbers.ManufacturerSerialNumber = serialNumbersImpl.ManufacturerSerialNumber;
						}
						documents.Lines.SerialNumbers.BatchID = serialNumbersImpl.BatchID;
						if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
						{
							documents.Lines.SerialNumbers.InternalSerialNumber = serialNumbersImpl.InternalSerialNumber;
						}
						if (serialNumbersImpl.ExpiryDate.HasValue)
						{
							documents.Lines.SerialNumbers.ExpiryDate = serialNumbersImpl.ExpiryDate.Value;
						}
						if (serialNumbersImpl.ManufactureDate.HasValue)
						{
							documents.Lines.SerialNumbers.ManufactureDate = serialNumbersImpl.ManufactureDate.Value;
						}
						if (serialNumbersImpl.ReceptionDate.HasValue)
						{
							documents.Lines.SerialNumbers.ReceptionDate = serialNumbersImpl.ReceptionDate.Value;
						}
						if (serialNumbersImpl.WarrantyStart.HasValue)
						{
							documents.Lines.SerialNumbers.WarrantyStart = serialNumbersImpl.WarrantyStart.Value;
						}
						if (serialNumbersImpl.WarrantyEnd.HasValue)
						{
							documents.Lines.SerialNumbers.WarrantyEnd = serialNumbersImpl.WarrantyEnd.Value;
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
						{
							documents.Lines.SerialNumbers.Location = serialNumbersImpl.Location;
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
						{
							documents.Lines.SerialNumbers.Notes = serialNumbersImpl.Notes;
						}
						documents.Lines.SerialNumbers.Quantity = serialNumbersImpl.Quantity;
					}
				}
			}
			if (model.Expenses != null && model.Expenses.Count > 0)
			{
				int num = 0;
				foreach (DocumentsAdditionalExpensesImpl expense in model.Expenses)
				{
					if (expense.VatSum + expense.LineTotal == 0.0)
					{
						continue;
					}
					documents.Expenses.Add();
					documents.Expenses.SetCurrentLine(num);
					if ((expense.BaseType != 0) & (expense.BaseType != -1))
					{
						documents.Expenses.BaseDocType = expense.BaseType;
						if (expense.DocEntry != 0)
						{
							documents.Expenses.BaseDocEntry = expense.DocEntry;
						}
						if (expense.BaseLnNum != 0)
						{
							documents.Expenses.BaseDocLine = expense.BaseLnNum;
						}
					}
					if (expense.DedVatSum != 0.0)
					{
						documents.Expenses.DeductibleTaxSum = expense.DedVatSum;
					}
					switch (expense.DistrbMthd.ToString())
					{
					case "N":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_None;
						break;
					case "Q":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_Quantity;
						break;
					case "V":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_Volume;
						break;
					case "W":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_Weight;
						break;
					case "E":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_Equally;
						break;
					case "T":
						documents.Expenses.DistributionMethod = BoAdEpnsDistribMethods.aedm_RowTotal;
						break;
					}
					if (!string.IsNullOrEmpty(expense.OcrCode))
					{
						documents.Expenses.DistributionRule = expense.OcrCode;
					}
					if (!string.IsNullOrEmpty(expense.OcrCode2))
					{
						documents.Expenses.DistributionRule2 = expense.OcrCode2;
					}
					if (!string.IsNullOrEmpty(expense.OcrCode3))
					{
						documents.Expenses.DistributionRule3 = expense.OcrCode3;
					}
					if (!string.IsNullOrEmpty(expense.OcrCode4))
					{
						documents.Expenses.DistributionRule4 = expense.OcrCode4;
					}
					if (!string.IsNullOrEmpty(expense.OcrCode5))
					{
						documents.Expenses.DistributionRule5 = expense.OcrCode5;
					}
					if (expense.ExpnsCode != 0)
					{
						documents.Expenses.ExpenseCode = expense.ExpnsCode;
					}
					documents.Expenses.LastPurchasePrice = BoYesNoEnum.tYES;
					documents.Expenses.LineTotal = expense.LineTotal;
					if (!string.IsNullOrEmpty(expense.Project))
					{
						documents.Expenses.Project = expense.Project;
					}
					if (!string.IsNullOrEmpty(expense.Comments))
					{
						documents.Expenses.Remarks = expense.Comments;
					}
					documents.Expenses.Stock = BoYesNoEnum.tNO;
					if (!string.IsNullOrEmpty(expense.TaxCode))
					{
						documents.Expenses.TaxCode = expense.TaxCode;
					}
					documents.Expenses.TaxSum = expense.VatSum;
					if (!string.IsNullOrEmpty(expense.VatGroup))
					{
						documents.Expenses.VatGroup = expense.VatGroup;
					}
					documents.Expenses.WTLiable = ((expense.TaxStatus == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					num++;
				}
			}
			if (!byKey)
			{
				if (documents.Add() != 0)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Insert as draft FAILED", "ERROR");
					return "ERROR";
				}
			}
			else
			{
				Logging.Write("TRACE", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Update as draft TRACE = ", documents.DocNum.ToString());
				if (documents.Update() != 0)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Update as draft FAILED", "ERROR");
					return "ERROR";
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "save as draft Exception", ex.Message.ToString());
			return ex.Message.ToString();
		}
	}

	public DataTable SelectOne(int docEntry, bool isDraft = false)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_PurchaseRequest_SearchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			if (isDraft)
			{
				list.Add(new SqlParameter("@IsDraft", isDraft));
			}
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Purchase - Line ", ex.Message.ToString());
		}
		return result;
	}

	public DataTable SearchDocments(DocumentsImpl doc, string filter = "")
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "APZ_PurchaseRequest_SearchDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			if (doc.DocNum == 0)
			{
				list.Add(new SqlParameter("@docNum", doc.DocNum));
				if (doc.DocDate != DateTime.MinValue)
				{
					list.Add(new SqlParameter("@tDocDate", doc.DocDate.Date));
				}
				if (doc.DocDueDate != DateTime.MinValue)
				{
					list.Add(new SqlParameter("@fDocDate", doc.DocDueDate));
				}
				if (!string.IsNullOrEmpty(doc.DocStatus))
				{
					list.Add(new SqlParameter("@documentStatus", doc.DocStatus));
				}
				if (doc.Canceled == "Y")
				{
					list.Add(new SqlParameter("@cancelled", 'Y'));
				}
				else if (doc.Canceled == "N")
				{
					list.Add(new SqlParameter("@cancelled", 'N'));
				}
				if (!string.IsNullOrEmpty(doc.Requester))
				{
					list.Add(new SqlParameter("@requester", doc.Requester));
				}
				if (!string.IsNullOrEmpty(doc.UserName))
				{
					list.Add(new SqlParameter("@UserName", doc.UserName));
				}
				if (doc.BPLId > 0 || string.IsNullOrEmpty(doc.BPLId.ToString()))
				{
					list.Add(new SqlParameter("@Branch", doc.BPLId));
				}
			}
			else if (doc.DocNum > 0 || doc.DocNum == -1)
			{
				list.Add(new SqlParameter("@docNum", doc.DocNum));
				if (!string.IsNullOrEmpty(doc.UserName))
				{
					list.Add(new SqlParameter("@UserName", doc.UserName));
				}
				if (doc.BPLId > 0 || string.IsNullOrEmpty(doc.BPLId.ToString()))
				{
					list.Add(new SqlParameter("@Branch", doc.BPLId));
				}
			}
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (!string.IsNullOrEmpty(filter))
			{
				string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
				DataRow[] array = dataTable.Select(filterText);
				if (array.IsNotNull())
				{
					return array.CopyToDataTable();
				}
				return new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return dataTable;
	}

	public bool CheckDoc(int docEntry)
	{
		try
		{
			string query = "SELECT   A.DocEntry \r\n                            FROM OPRQ A \r\n                            WHERE A.DocEntry = '" + docEntry + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckDoc  FAILED ", ex.ToString());
			return false;
		}
	}

	public bool CheckDocLine(int docEntry, int lineNum)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT 1\r\n                            FROM PRQ1 \r\n                            WHERE DocEntry = @DocEntry AND LineNum = @LineNum", CommandType.Text, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@LineNum", lineNum));
			return dataTable != null && dataTable.Rows.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckDocLine  FAILED ", ex.ToString());
			return false;
		}
	}

	public DataTable GetDepartment(string requester)
	{
		try
		{
			return ExecuteDataTable("SELECT A.Requester , A.ReqName, b.Name , ISNULL(B.Code,0) AS Code\r\n                            From OPRQ A\r\n                            LEFT JOIN OUDP B\r\n                            ON B.Code = A.Department\r\n                            where (@Requester='' OR (@Requester<>'' and Requester=@Requester))", CommandType.Text, new SqlParameter("@Requester", requester));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetBranch(string requester)
	{
		try
		{
			new DataTable();
			string text = "SELECT A.Requester , A.ReqName, C.Name ,ISNULL(C.Code,0) AS Code\r\n                                From OPRQ A\r\n                                left join OUBR C\r\n                                ON C.Code = A.Branch\r\n                                where 1=1";
			if (!string.IsNullOrEmpty(requester))
			{
				text = text + " AND Requester ='" + requester + "'";
			}
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public int GetQuantityBase(string tbl, string tblLine, int baseEntry, int baseLine)
	{
		try
		{
			string empty = string.Empty;
			empty = "SELECT B.BaseEntry ";
			if (baseLine != int.MinValue)
			{
				empty += " , B.BaseLine";
			}
			empty = empty + ", SUM( B.Quantity) AS Quantity\r\n                    FROM " + tbl + " A";
			empty = empty + " LEFT JOIN  " + tblLine + " B ON A.DocEntry = B.DocEntry";
			empty += " WHERE 1=1 AND B.LineStatus != 'C'";
			if (baseEntry != 0)
			{
				empty = empty + " AND B.BaseEntry='" + baseEntry + "'";
			}
			if (baseLine != int.MinValue)
			{
				empty = empty + " AND  B.BaseLine =N'" + baseLine + "'";
			}
			empty += " GROUP BY B.BaseEntry";
			if (baseLine != int.MinValue)
			{
				empty += " , B.BaseLine";
			}
			return Convert.ToInt32(ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null).Rows[0]["Quantity"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetQuantityBase  FAILED ", ex.Message.ToString());
			return 0;
		}
	}

	public int GetOpenQuantity(string tbl, string tblLine, int docEntry, int lineNum)
	{
		try
		{
			string empty = string.Empty;
			empty = "SELECT a.DocEntry ";
			if (lineNum != int.MinValue)
			{
				empty += " , B.LineNum";
			}
			empty = empty + ", SUM( B.OpenQty) AS OpenQty\r\n                    FROM " + tbl + " A";
			empty = empty + " LEFT JOIN  " + tblLine + " B ON A.DocEntry = B.DocEntry";
			empty += " WHERE 1=1 ";
			empty += " AND B.LineStatus != 'C'";
			if (docEntry != 0)
			{
				empty = empty + " AND A.DocEntry='" + docEntry + "'";
			}
			if (lineNum != int.MinValue)
			{
				empty = empty + " AND  B.LineNum ='" + lineNum + "'";
			}
			empty += " GROUP BY A.DocEntry";
			if (lineNum != int.MinValue)
			{
				empty += " , B.LineNum";
			}
			return Convert.ToInt32(ExecuteDataTable(empty, CommandType.Text, (SqlParameter[])null).Rows[0]["OpenQty"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOpenQuantity  FAILED ", ex.Message.ToString());
			return 0;
		}
	}

	public int DealDocOrgin(ApzObjectTypes typeFrom, int baseEntry)
	{
		try
		{
			List<string> list = new List<string>();
			list = GetTableName(typeFrom);
			int openQuantity = GetOpenQuantity(list[0], list[1], baseEntry, int.MinValue);
			string empty = string.Empty;
			if (openQuantity == 0)
			{
				empty = $" UPDATE {list[0]} \r\n                                        SET DocStatus = 'C'\r\n                                        WHERE DocEntry = '{baseEntry}'\r\n                                        ";
				ExecuteNonQuery(empty, CommandType.Text, (SqlParameter[])null);
			}
			return openQuantity;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CloseDocOrgin  FAILED ", ex.Message.ToString());
			return int.MinValue;
		}
	}

	public int DealLineOrgin(ApzObjectTypes typeTo, ApzObjectTypes typeFrom, int baseEntry, int baseLine)
	{
		try
		{
			string text = string.Empty;
			List<string> list = new List<string>();
			list = GetTableName(typeTo);
			int quantityBase = GetQuantityBase(list[0], list[1], baseEntry, baseLine);
			List<string> list2 = new List<string>();
			list2 = GetTableName(typeFrom);
			int num = GetOpenQuantity(list2[0], list2[1], baseEntry, baseLine) - quantityBase;
			if (num <= 0)
			{
				text = $" UPDATE {list2[1]} \r\n                                        SET LineStatus = 'C'\r\n                                        WHERE DocEntry = '{baseEntry}' AND LineNum ='{baseLine}'\r\n                                        ";
			}
			text += string.Format(" UPDATE {0} \r\n                                SET OpenQty = '" + num + "'\r\n                                WHERE DocEntry = '{1}' AND LineNum ='{2}'\r\n                                ", list2[1], baseEntry, baseLine);
			ExecuteNonQuery(text, CommandType.Text, (SqlParameter[])null);
			return num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CloseLineOrgin  FAILED ", ex.Message.ToString());
			return int.MinValue;
		}
	}

	public List<string> GetTableName(ApzObjectTypes type)
	{
		List<string> list = new List<string>();
		string item = string.Empty;
		string item2 = string.Empty;
		switch (type)
		{
		case ApzObjectTypes.oQuotations:
			item = "OQUT";
			item2 = "QUT1";
			break;
		case ApzObjectTypes.oOrders:
			item = "ORDR";
			item2 = "RDR1";
			break;
		case ApzObjectTypes.oDeliveryNotes:
			item = "ODLN";
			item2 = "DLN1";
			break;
		case ApzObjectTypes.oInvoices:
			item = "OINV";
			item2 = "INV1";
			break;
		case ApzObjectTypes.oCreditNotes:
			item = "ORIN";
			item2 = "RIN1";
			break;
		case ApzObjectTypes.oBlanketAgreement:
			item = "OOAT";
			item2 = "OAT1";
			break;
		case ApzObjectTypes.oReturns:
			item = "ORDN";
			item2 = "RDN1";
			break;
		case ApzObjectTypes.oPurchaseQuotations:
			item = "OPQT";
			item2 = "PQT1";
			break;
		case ApzObjectTypes.oPurchaseOrders:
			item = "OPQT";
			item2 = "PQT1";
			break;
		case ApzObjectTypes.oPurchaseRequest:
			item = "OPRQ";
			item2 = "PRQ1";
			break;
		case ApzObjectTypes.oPurchaseInvoices:
			item = "OPCH";
			item2 = "PCH1";
			break;
		case ApzObjectTypes.oPurchaseDeliveryNotes:
			item = "OPDN";
			item2 = "PDN1";
			break;
		case ApzObjectTypes.oPurchaseCreditNotes:
			item = "ORPC";
			item2 = "RPC1";
			break;
		case ApzObjectTypes.oPurchaseReturns:
			item = "ORPD";
			item2 = "RPD1";
			break;
		case ApzObjectTypes.oDrafts:
			item = "ODRF";
			item2 = "DRF1";
			break;
		}
		list.Add(item);
		list.Add(item2);
		return list;
	}

	public DataTable GetListDepartment()
	{
		try
		{
			return ExecuteDataTable(GetListDepartmentString(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListDepartment ", ex.Message.ToString());
		}
		return new DataTable();
	}

	protected virtual string GetListDepartmentString()
	{
		return "select Code, isnull (Name,'') as Name from OUDP\r\n                               Union all\r\n                               select -1,'' order by code ";
	}

	public DataTable GetListBranch()
	{
		try
		{
			return ExecuteDataTable(GetListBranchString(), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListBranch", ex.Message.ToString());
		}
		return new DataTable();
	}

	protected virtual string GetListBranchString()
	{
		return "select Code, isnull (Name,'') as Name from OUBR\r\n                                Union all\r\n                                select -1,'' order by code";
	}

	public DataTable GetListPurchaseRequest()
	{
		try
		{
			return ExecuteDataTable("APZ_CFL_PurchaseRequest", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListPurchaseRequest", ex.Message.ToString());
		}
		return new DataTable();
	}

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("OPRQ", "DocEntry");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("OPRQ", "DocEntry");
	}

	public DataTable GetNextRecord(string id)
	{
		return GetUnitOfWork().Commons.GetNextRecord("OPRQ", "DocEntry", id);
	}

	public DataTable GetPreviousRecord(string id)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("OPRQ", "DocEntry", id);
	}

	public DataTable GetRecoderDoc(int id)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocEntry", Function.ParseInt(id))
			};
			DataTable dataTable = ExecuteDataTable("APZ_PurchaseRequest_GetRecoderDoc", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecoderDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecoderLine(int id)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@DocEntry", Function.ParseInt(id))
			};
			DataTable dataTable = ExecuteDataTable("APZ_PurchaseRequest_GetRecoderLine", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecoderDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxDocEntry()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OPRQ ", CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["MaxDocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry  FAILED ", ex.ToString());
			return 0;
		}
	}

	public int GetMaxDocNum()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT ISNULL( MAX(DocNum ),0) as MaxDocNum\r\n                            FROM OPRQ ", CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["MaxDocNum"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocNum  FAILED ", ex.ToString());
			return 0;
		}
	}

	public int GetMinDocEntry()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT ISNULL( MIN(DocEntry ),0) as DocEntry\r\n                            FROM OPRQ ", CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMinDocEntry  FAILED ", ex.ToString());
			return 0;
		}
	}

	public DataTable GetDefaultTableDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_PurchaseRequest_GetDefaultTableDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDefaultTableDoc FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDefaultTableLine()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_PurchaseRequest_GetDefaultTableLine", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDefaultTableLine  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType)
	{
		return GetUnitOfWork().Documents.GetBatchSerialByCode(itemcode, whscode, lineNum, barcode, objType);
	}

	public DataTable GetBatchSerialByDocEntry(int docEntry)
	{
		return GetUnitOfWork().Documents.GetBatchSerialByDocEntry(docEntry, ApzObjectTypes.oPurchaseRequest);
	}

	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return GetUnitOfWork().Documents.CopyBatchSerial(fromentry, fromtype);
	}

	public DataTable LoadSeries(string usercode)
	{
		return GetUnitOfWork().Documents.LoadSeries(usercode, ApzObjectTypes.oPurchaseRequest);
	}

	public int GetDocumentNumber(int series)
	{
		return GetUnitOfWork().Documents.GetDocumentNumber(series, ApzObjectTypes.oPurchaseRequest);
	}

	public DocumentsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}
}
