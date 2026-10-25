using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.ARInvoice;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Services.Interfaces.Payment;

namespace Apzon.Services.Repository.Payment;

public class OutgoingPaymentRepository : BaseService<IncomingPaymentsImpl>, IOutgoingPayment, IService<IncomingPaymentsImpl>
{
	public OutgoingPaymentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowOutgoingPayment()
	{
		try
		{
			string query = "SELECT \r\n                                          isnull(DocEntry,0) as DocEntry\r\n                                          ,isnull(DocNum,0) as  DocNum \r\n                                          ,isnull(DocType,'') as  DocType \r\n                                          ,isnull(Canceled,'') as  Canceled \r\n                                          ,isnull(Printed,'') as  Printed \r\n                                          ,isnull(DocDate,'') as  DocDate \r\n                                          ,isnull(DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(CardCode,'') as  CardCode \r\n                                          ,isnull(CardName,'') as  CardName \r\n                                          ,isnull(Address,'') as  Address \r\n                                          ,isnull(CashAcct,'') as  CashAcct \r\n                                          ,isnull(CashSum,0) as  CashSum \r\n                                          ,isnull(CreditSum,0) as  CreditSum \r\n                                          ,isnull(CouponSum,0) as  CouponSum \r\n                                          ,isnull(TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(DocCurr,'') as  DocCurr \r\n                                          ,isnull(DocRate,0) as  DocRate \r\n                                          ,isnull(DocTotal,0) as  DocTotal \r\n                                          ,isnull(JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(TransId,0) as  TransId \r\n                                          ,isnull(DocTime,'') as  DocTime \r\n                                          ,isnull(ObjType,'') as  ObjType \r\n                                          ,isnull(UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(CreateDate,'') as  CreateDate \r\n                                          ,isnull(UserSign,0) as  UserSign \r\n                                          ,isnull(VatGroup,'') as  VatGroup \r\n                                          ,isnull(VatSum,0) as  VatSum \r\n                                          ,isnull(Dcount,0) as  Dcount \r\n                                          ,isnull(DcntSum,0) as  DcntSum \r\n                                          ,isnull(CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(DocTotalSy,0) as  DocTotalSy \r\n                                    FROM  OVPM";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetOutgoingPayment(string docentry)
	{
		try
		{
			string text = "";
			text = ((!(GetDocType(docentry) == "C") && !(GetDocType(docentry) == "S") && !(GetDocType(docentry) == "L")) ? ("select  isnull(DocNum,0) as  DocNum \r\n                                          ,isnull(DocType,'') as  DocType \r\n                                          ,isnull(Canceled,'') as  Canceled \r\n                                          ,isnull(Printed,'') as  Printed \r\n                                          ,isnull(DocDate,'') as  DocDate \r\n                                          ,isnull(DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(CardCode,'') as  CardCode \r\n                                          ,isnull(CardName,'') as  CardName \r\n                                          ,isnull(Address,'') as  Address \r\n                                          ,isnull(CashAcct,'') as  CashAcct \r\n                                          ,isnull(CashSum,0) as  CashSum \r\n                                          ,isnull(CreditSum,0) as  CreditSum \r\n                                          ,isnull(CouponSum,0) as  CouponSum \r\n                                          ,isnull(TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(DocCurr,'') as  DocCurr \r\n                                          ,isnull(DocRate,0) as  DocRate \r\n                                          ,isnull(DocTotal,0) as  DocTotal \r\n                                          ,isnull(JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(TransId,0) as  TransId \r\n                                          ,isnull(DocTime,'') as  DocTime \r\n                                          ,isnull(ObjType,'') as  ObjType \r\n                                          ,isnull(UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(CreateDate,'') as  CreateDate \r\n                                          ,isnull(UserSign,0) as  UserSign \r\n                                          ,isnull(VatGroup,'') as  VatGroup \r\n                                          ,isnull(VatSum,0) as  VatSum \r\n                                          ,isnull(Dcount,0) as  Dcount \r\n                                          ,isnull(DcntSum,0) as  DcntSum \r\n                                          ,isnull(CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(DocTotalSy,0) as  DocTotalSy\r\n                                    FROM  OVPM\r\n\t\t\t\t\t\t\t\t\twhere docentry=" + docentry) : ("select  isnull(DocNum,0) as  DocNum \r\n                                          ,isnull(DocType,'') as  DocType \r\n                                          ,isnull(Canceled,'') as  Canceled \r\n                                          ,isnull(Printed,'') as  Printed \r\n                                          ,isnull(DocDate,'') as  DocDate \r\n                                          ,isnull(DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(a.CardCode,'') as  CardCode \r\n                                          ,isnull(a.CardName,'') as  CardName \r\n                                          ,isnull(Address,'') as  Address \r\n                                          ,isnull(CashAcct,'') as  CashAcct \r\n                                          ,isnull(CashSum,0) as  CashSum \r\n                                          ,isnull(CreditSum,0) as  CreditSum \r\n                                          ,isnull(CouponSum,0) as  CouponSum \r\n                                          ,isnull(TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(DocCurr,'') as  DocCurr \r\n                                          ,isnull(DocRate,0) as  DocRate \r\n                                          ,isnull(DocTotal,0) as  DocTotal \r\n                                          ,isnull(JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(TransId,0) as  TransId \r\n                                          ,isnull(DocTime,'') as  DocTime \r\n                                          ,isnull(a.ObjType,'') as  ObjType \r\n                                          ,isnull(a.UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(a.CreateDate,'') as  CreateDate \r\n                                          ,isnull(a.UserSign,0) as  UserSign \r\n                                          ,isnull(VatGroup,'') as  VatGroup \r\n                                          ,isnull(VatSum,0) as  VatSum \r\n                                          ,isnull(Dcount,0) as  Dcount \r\n                                          ,isnull(DcntSum,0) as  DcntSum \r\n                                          ,isnull(CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(DocTotalSy,0) as  DocTotalSy ,isnull(b.CardType,'') as CardType\r\n                                          ,isnull(b.Free_Text,'') as Free_Text\r\n                                    FROM  OVPM a inner join Ocrd b on a.CardCode=b.CardCode\r\n\t\t\t\t\t\t\t\t\twhere a.docentry=" + docentry));
			new DataTable();
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry),0) as maxCode\r\n                                    FROM OVPM";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["maxCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public string DeleteDoc(string code)
	{
		try
		{
			string query = " delete OCTG where groupNum = " + code;
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string GetDocType(string docEntry)
	{
		try
		{
			string query = "SELECT DocType \r\n\t\t\t\t\t\t\t        FROM  OVPM\r\n                                    WHERE docEntry = " + docEntry;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable.Rows[0]["DocType"].ToString();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT GroupNum \r\n\t\t\t\t\t\t\t        FROM  OVPM\r\n                                    WHERE GroupNum = " + code;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string Insert(IncomingPaymentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(IncomingPaymentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(IncomingPaymentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public IncomingPaymentsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public string SaveDoc(IncomingPaymentsImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			try
			{
				string empty2 = string.Empty;
				empty2 = $"INSERT INTO [dbo].[OVPM]\r\n                                                   ([DocEntry] ,[DocNum] ,[DocType] ,[Canceled] ,[Printed] ,[DocDate]\r\n                                                   ,[DocDueDate] ,[CardCode] ,[CardName] ,[Address] ,[CashAcct] ,[CashSum]\r\n                                                   ,[CreditSum] ,[CouponSum]  ,[TrsfrAcct] ,[TrsfrSum]   ,[TrsfrDate]\r\n                                                   ,[TrsfrRef]  ,[DocCurr] ,[DocRate]  ,[DocTotal]  ,[JrnlMemo]\r\n                                                   ,[TransId]  ,[DocTime] ,[ObjType] ,[UpdateDate] ,[CreateDate]\r\n                                                   ,[UserSign] ,[VatGroup] ,[VatSum] ,[Dcount] ,[DcntSum] ,[CashSumFC]\r\n                                                   ,[CreditSumFC] ,[TrsfrSumFC] ,[CashSumSy] ,[CreditSumSy] ,[TrsfrSumSy]\r\n                                                   ,[VatSumFC] ,[VatSumSy] ,[DcntSumFC]  ,[DocTotalFC] ,[DocTotalSy])\r\n                                             VALUES\r\n                                                   ({doc.DocEntry} ,{doc.DocNum}  ,N'{doc.DocType}'  ,N'{doc.Canceled}' ,N'{doc.Printed}' ,N'{doc.DocDate}' ,N'{doc.DocDueDate}' ,N'{doc.CardCode}' ,N'{doc.CardName}'\r\n                                                   ,N'{doc.Address}' ,N'{doc.CashAcct}' ,{doc.CashSum} ,{doc.CreditSum} ,{doc.CouponSum} ,N'{doc.TrsfrAcct}' ,{doc.TrsfrSum} ,N'{doc.TrsfrDate}' ,N'{doc.TrsfrRef}'\r\n                                                   ,N'{doc.DocCurr}' ,{doc.DocRate} ,{doc.DocTotal} ,N'{doc.JrnlMemo}' ,{doc.TransId} ,N'{doc.DocTime}' ,N'{doc.ObjType}' ,N'{doc.UpdateDate}' ,N'{doc.CreateDate}'\r\n                                                   ,{doc.UserSign}  ,N'{doc.VatGroup}' ,{doc.VatSum} ,{doc.Dcount} ,{doc.DcntSum} ,{doc.CashSumFC} ,{doc.CreditSumFC} ,{doc.TrsfrSumFC} ,{doc.CashSumSy} ,{doc.CreditSumSy} ,{doc.TrsfrSumSy}\r\n                                                   ,{doc.VatSumFC} ,{doc.VatSumSy} ,{doc.DcntSumFC} ,{doc.DocTotalFC} ,{doc.DocTotalSy})";
				if (ExecuteNonQueryNoParam(empty2) > 0)
				{
					if (doc._isPurpose)
					{
						foreach (Payments_AccountsImpl item in doc._listPurpose)
						{
							if (!string.IsNullOrEmpty(SaveAccountPurpose(item)))
							{
								dbTransaction.Rollback();
								return "ERROR";
							}
						}
					}
					else
					{
						foreach (IncomingPaymentInvoicesImpl item2 in doc._listInvoice)
						{
							if (!string.IsNullOrEmpty(SaveInvoiceDoc(item2)))
							{
								dbTransaction.Rollback();
								return "ERROR";
							}
							if (doc._isCustomerARInvoice)
							{
								ARInvoicesImpl aRInvoicesImpl = new ARInvoicesImpl();
								aRInvoicesImpl.TargetType = Convert.ToInt32(doc.ObjType);
								aRInvoicesImpl.TrgetEntry = doc.DocEntry;
								aRInvoicesImpl.DocEntry = item2.DocEntry;
								if (!string.IsNullOrEmpty(UpdateARInvoiceDetail(aRInvoicesImpl)))
								{
									dbTransaction.Rollback();
									return "ERROR";
								}
								aRInvoicesImpl = new ARInvoicesImpl();
								aRInvoicesImpl.PaidSum = doc.DocTotal;
								aRInvoicesImpl.UpdateDate = DateTime.Today;
								aRInvoicesImpl.DocEntry = item2.DocEntry;
								if (!string.IsNullOrEmpty(UpdateARInvoice(aRInvoicesImpl)))
								{
									dbTransaction.Rollback();
									return "ERROR";
								}
							}
							else
							{
								ARInvoicesImpl aRInvoicesImpl2 = new ARInvoicesImpl();
								aRInvoicesImpl2.TargetType = Convert.ToInt32(doc.ObjType);
								aRInvoicesImpl2.TrgetEntry = doc.DocEntry;
								aRInvoicesImpl2.DocEntry = item2.DocEntry;
								if (!string.IsNullOrEmpty(UpdateAPInvoiceDetail(aRInvoicesImpl2)))
								{
									dbTransaction.Rollback();
									return "ERROR";
								}
								aRInvoicesImpl2 = new ARInvoicesImpl();
								aRInvoicesImpl2.PaidSum = doc.DocTotal;
								aRInvoicesImpl2.UpdateDate = DateTime.Today;
								aRInvoicesImpl2.DocEntry = item2.DocEntry;
								if (!string.IsNullOrEmpty(UpdateAPInvoice(aRInvoicesImpl2)))
								{
									dbTransaction.Rollback();
									return "ERROR";
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(SaveCreditCardDoc(doc._objCreditCard)))
					{
						dbTransaction.Rollback();
						return "ERROR";
					}
					if (!string.IsNullOrEmpty(SaveCouponDoc(doc._objCoupons)))
					{
						dbTransaction.Rollback();
						return "ERROR";
					}
					dbTransaction.Commit();
					return null;
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
				empty = ex.Message;
			}
			return empty;
		}
	}

	public string SaveCreditCardDoc(IncomingPaymentCreditCardsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"INSERT INTO [dbo].[VPM3]\r\n                                           ([DocNum],[DocDate] ,[CreditCard] ,[CrCardNum] ,[CardName]\r\n                                           ,[CardValid] ,[OwnerIdNum] ,[OwnerPhone] ,[OwnerAdd]\r\n                                           ,[CreditSum] ,[CreditCur] ,[ConfNum])\r\n                                     VALUES\r\n                                           ({doc.DocNum}, N'{doc.DocDate}',{doc.CreditCard} ,N'{doc.CrCardNum}' ,N'{doc.CardName}' ,N'{doc.CardValid}' ,N'{doc.OwnerIdNum}' ,N'{doc.OwnerPhone}'\r\n                                           ,N'{doc.OwnerAdd}' ,{doc.CreditSum} ,N'{doc.CreditCur}' ,N'{doc.ConfNum}')";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveInvoiceDoc(IncomingPaymentInvoicesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"INSERT INTO [dbo].[VPM2]\r\n                                                   ([DocNum] ,[InvoiceId] ,[DocEntry] ,[SumApplied] ,[InvType]\r\n                                                   ,[DocRate] ,[DocLine] ,[vatApplied] ,[ObjType] ,[Dcount]\r\n                                                   ,[DcntSum] ,[BfDcntSum] ,[BfNetDcnt])\r\n                                             VALUES\r\n                                                   ({doc.DocNum} ,{doc.InvoiceId}  ,{doc.DocEntry} ,{doc.SumApplied} ,N'{doc.InvType}'  ,{doc.DocRate} ,{doc.DocLine} ,{doc.vatApplied}\r\n                                                   ,N'{doc.ObjType}' ,{doc.Dcount} ,{doc.DcntSum} ,{doc.BfDcntSum} ,{doc.BfNetDcnt})";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveCouponDoc(IncomingPaymentCouponsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"INSERT INTO [dbo].[VPM9]\r\n                                                   ([DocNum] ,[DocDate] ,[CouponNum] ,[CouponName] ,[CardValid]\r\n                                                   ,[OwnerIdNum] ,[OwnerPhone] ,[OwnerAdd] ,[CouponSum] ,[CouponCur])\r\n                                             VALUES\r\n                                                   ({doc.DocNum} ,N'{doc.DocDate}' ,N'{doc.CouponNum}' ,N'{doc.CouponName}' ,N'{doc.CardValid}'\r\n                                                   ,N'{doc.OwnerIdNum}' ,N'{doc.OwnerPhone}' ,N'{doc.OwnerAdd}' ,{doc.CouponSum} ,N'{doc.CouponCur}')";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveAccountPurpose(Payments_AccountsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"  INSERT INTO [dbo].[VPM4]\r\n                                                       ([DocNum]\r\n                                                       ,[LineId]\r\n                                                       ,[AcctCode]\r\n                                                       ,[AcctName]\r\n                                                       ,[Amount]\r\n                                                       ,[SumApplied]\r\n                                                       ,[Descrip]\r\n                                                       ,[VatGroup]\r\n                                                       ,[VatPrcnt]\r\n                                                       ,[ObjType]\r\n                                                       ,[VatAmnt])\r\n                                                 VALUES\r\n                                                       ({doc.DocNum}\r\n                                                       ,{doc.LineId}\r\n                                                       ,N'{doc.AcctCode}'\r\n                                                       ,N'{doc.AcctName}'\r\n                                                       ,{doc.Amount}\r\n                                                       ,{doc.SumApplied}\r\n                                                       ,N'{doc.Descrip}'\r\n                                                       ,N'{doc.VatGroup}'\r\n                                                       ,{doc.VatPrcnt}\r\n                                                       ,N'{doc.ObjType}'\r\n                                                       ,{doc.VatAmnt})";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateARInvoiceDetail(ARInvoicesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"Update INV1 Set LineStatus= 'C', TargetType= '{doc.TargetType}', TrgetEntry= '{doc.TrgetEntry}'\r\n                                                    Where DocEntry = {doc.DocEntry} and TargetType = -1";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateARInvoice(ARInvoicesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"Update OINV Set DocStatus= 'C',\r\n                                        PaidSys += 0,\r\n                                        PaidSum += {doc.PaidSum},\r\n                                        UpdateDate = '{doc.UpdateDate}' \r\n                                        Where DocEntry = {doc.DocEntry}";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateAPInvoiceDetail(ARInvoicesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"Update PCH1 Set LineStatus= 'C', TargetType= '{doc.TargetType}', TrgetEntry= '{doc.TrgetEntry}'\r\n                                                    Where DocEntry = {doc.DocEntry} and TargetType = -1";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateAPInvoice(ARInvoicesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"Update OPCH Set DocStatus= 'C',\r\n                                        PaidSys += 0,\r\n                                        PaidSum += {doc.PaidSum},\r\n                                        UpdateDate = '{doc.UpdateDate}' \r\n                                        Where DocEntry = {doc.DocEntry}";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveUDF(List<Common.Table_ID_Code> lst)
	{
		try
		{
			string text = "";
			string text2 = "";
			text2 = lst[lst.Count - 1].ID.ToString();
			lst.RemoveAt(lst.Count - 1);
			foreach (Common.Table_ID_Code item in lst)
			{
				text = ((!(item.Name == "CheckEdit") && !(item.Name == "CalcEdit")) ? (text + item.ID?.ToString() + "=N'" + item.Code + "',") : (text + item.ID?.ToString() + "=" + item.Code + ","));
			}
			text = text.Remove(text.Length - 1);
			string query = "update ovpm set " + text + " where docEntry=" + text2;
			if (ExecuteNonQueryNoParam(query) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable GetAPInvoice(string CardCode, string Status)
	{
		try
		{
			string query = "select distinct  ISNULL(a.DocEntry,0) as DocEntry ,\r\n                             ISNULL(a.DocNum,0) as DocNum ,a.DocType  ,a.CANCELED  ,a.Handwrtten  ,a.Printed \r\n                             ,a.DocStatus  ,a.ObjType   ,ISNULL(a.DocDate,'') as DocDate  ,ISNULL(a.DocDueDate,'') as DocDueDate  ,a.CardCode ,a.CardName \r\n                             ,a.Address  ,a.NumAtCard , ISNULL(a.VatSum,0) as VatSum , ISNULL(a.VatSumFC,0) as VatSumFC ,\r\n                             ISNULL(a.DiscPrcnt,0) as DiscPrcnt ,  ISNULL(a.DiscSum,0) as DiscSum ,  ISNULL(a.DiscSumFC,0) as DiscSumFC \r\n                             ,a.DocCur , ISNULL(a.DocRate,0) as DocRate , ISNULL(a.DocTotal,0) as DocTotal , ISNULL(a.DocTotalFC,0) as DocTotalFC ,\r\n                             ISNULL(a.PaidToDate,0) as PaidToDate ,  ISNULL(a.PaidFC,0) as PaidFC , ISNULL(a.GrosProfit,0) as GrosProfit, \r\n                             ISNULL(a.GrosProfFC,0) as GrosProfFC  ,a.Ref1  ,a.Ref2   ,a.Comments  ,a.JrnlMemo ,  ISNULL(a.TransId,0) as TransId ,\r\n                             ISNULL(a.ReceiptNum,0) as ReceiptNum ,  ISNULL(a.GroupNum,0) as GroupNum, ISNULL(a.DocTime,'') as DocTime ,  ISNULL(a.SlpCode,0) as SlpCode ,\r\n                             ISNULL(a.TrnspCode,0) as TrnspCode , ISNULL(a.CntctCode,0) as CntctCode ,  ISNULL(a.VatSumSy,0) as VatSumSy ,\r\n                             ISNULL(a.DiscSumSy,0) as DiscSumSy ,  ISNULL(a.DocTotalSy,0) as DocTotalSy ,  ISNULL(a.PaidSys,0) as PaidSys \r\n                             ,ISNULL(a.UpdateDate,'') as UpdateDate  ,IsICT  ,ISNULL(a.CreateDate,'') as CreateDate , ISNULL(Series,0) as Series,ISNULL(a.TaxDate,'') as TaxDate, ISNULL(a.FinncPriod,0) as FinncPriod ,\r\n                             ISNULL(UserSign,0) as UserSign, ISNULL(UserSign2,0) as UserSign2  ,ISNULL(a.VatDate,'') as  VatDate , ISNULL(PaidSum,0) as PaidSum ,\r\n                             ISNULL(PaidSumFc,0) as PaidSumFc , ISNULL(PaidSumSc,0) as PaidSumSc  ,ISNULL(Cashremainder,0) as Cashremainder \r\n                             ,IsRIV   ,CurSource ,ShipToCode ,PayToCode  ,Confirmed  ,Rounding  ,Address2 \r\n                                        from OPCH a\r\n\t\t                            where CardCode='" + CardCode + "' and DocStatus='" + Status + "'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetARInvoiceByOutgoingPayment(string docNum)
	{
		try
		{
			string query = "select ISNULL(a.DocEntry,0) as DocEntry ,\r\n                             ISNULL(a.DocNum,0) as DocNum ,a.DocType  ,a.CANCELED  ,a.Handwrtten  ,a.Printed \r\n                             ,a.DocStatus  ,a.ObjType   ,ISNULL(a.DocDate,'') as DocDate  ,ISNULL(a.DocDueDate,'') as DocDueDate  ,a.CardCode ,a.CardName \r\n                             ,a.Address  ,a.NumAtCard , ISNULL(a.VatSum,0) as VatSum , ISNULL(a.VatSumFC,0) as VatSumFC ,\r\n                             ISNULL(a.DiscPrcnt,0) as DiscPrcnt ,  ISNULL(a.DiscSum,0) as DiscSum ,  ISNULL(a.DiscSumFC,0) as DiscSumFC \r\n                             ,a.DocCur , ISNULL(a.DocRate,0) as DocRate , ISNULL(a.DocTotal,0) as DocTotal , ISNULL(a.DocTotalFC,0) as DocTotalFC ,\r\n                             ISNULL(a.PaidToDate,0) as PaidToDate ,  ISNULL(a.PaidFC,0) as PaidFC , ISNULL(a.GrosProfit,0) as GrosProfit, \r\n                             ISNULL(a.GrosProfFC,0) as GrosProfFC  ,a.Ref1  ,a.Ref2   ,a.Comments  ,a.JrnlMemo ,  ISNULL(a.TransId,0) as TransId ,\r\n                             ISNULL(a.ReceiptNum,0) as ReceiptNum ,  ISNULL(a.GroupNum,0) as GroupNum, ISNULL(a.DocTime,'') as DocTime ,  ISNULL(a.SlpCode,0) as SlpCode ,\r\n                             ISNULL(a.TrnspCode,0) as TrnspCode , ISNULL(a.CntctCode,0) as CntctCode ,  ISNULL(a.VatSumSy,0) as VatSumSy ,\r\n                             ISNULL(a.DiscSumSy,0) as DiscSumSy ,  ISNULL(a.DocTotalSy,0) as DocTotalSy ,  ISNULL(a.PaidSys,0) as PaidSys \r\n                             ,ISNULL(a.UpdateDate,'') as UpdateDate  ,IsICT  ,ISNULL(a.CreateDate,'') as CreateDate , ISNULL(Series,0) as Series,ISNULL(a.TaxDate,'') as TaxDate, ISNULL(a.FinncPriod,0) as FinncPriod ,\r\n                             ISNULL(UserSign,0) as UserSign, ISNULL(UserSign2,0) as UserSign2  ,ISNULL(a.VatDate,'') as  VatDate , ISNULL(PaidSum,0) as PaidSum ,\r\n                             ISNULL(PaidSumFc,0) as PaidSumFc , ISNULL(PaidSumSc,0) as PaidSumSc  ,L_Code  ,L_Name \r\n                             ,CurSource ,TypeDisc  ,ShipToCode ,PayToCode  ,Confirmed  ,Rounding  ,Address2 \r\n                                        from oinv a inner join vpm2 b on a.DocEntry=b.DocEntry \r\n\t\t                          where b.docnum =" + docNum;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAPInvoiceByOutgoingPayment(string docNum)
	{
		try
		{
			string query = "select  ISNULL(a.DocEntry,0) as DocEntry ,\r\n                             ISNULL(a.DocNum,0) as DocNum ,a.DocType  ,a.CANCELED  ,a.Handwrtten  ,a.Printed \r\n                             ,a.DocStatus  ,a.ObjType   ,ISNULL(a.DocDate,'') as DocDate  ,ISNULL(a.DocDueDate,'') as DocDueDate  ,a.CardCode ,a.CardName \r\n                             ,a.Address  ,a.NumAtCard , ISNULL(a.VatSum,0) as VatSum , ISNULL(a.VatSumFC,0) as VatSumFC ,\r\n                             ISNULL(a.DiscPrcnt,0) as DiscPrcnt ,  ISNULL(a.DiscSum,0) as DiscSum ,  ISNULL(a.DiscSumFC,0) as DiscSumFC \r\n                             ,a.DocCur , ISNULL(a.DocRate,0) as DocRate , ISNULL(a.DocTotal,0) as DocTotal , ISNULL(a.DocTotalFC,0) as DocTotalFC ,\r\n                             ISNULL(a.PaidToDate,0) as PaidToDate ,  ISNULL(a.PaidFC,0) as PaidFC , ISNULL(a.GrosProfit,0) as GrosProfit, \r\n                             ISNULL(a.GrosProfFC,0) as GrosProfFC  ,a.Ref1  ,a.Ref2   ,a.Comments  ,a.JrnlMemo ,  ISNULL(a.TransId,0) as TransId ,\r\n                             ISNULL(a.ReceiptNum,0) as ReceiptNum ,  ISNULL(a.GroupNum,0) as GroupNum, ISNULL(a.DocTime,'') as DocTime ,  ISNULL(a.SlpCode,0) as SlpCode ,\r\n                             ISNULL(a.TrnspCode,0) as TrnspCode , ISNULL(a.CntctCode,0) as CntctCode ,  ISNULL(a.VatSumSy,0) as VatSumSy ,\r\n                             ISNULL(a.DiscSumSy,0) as DiscSumSy ,  ISNULL(a.DocTotalSy,0) as DocTotalSy ,  ISNULL(a.PaidSys,0) as PaidSys \r\n                             ,ISNULL(a.UpdateDate,'') as UpdateDate  ,IsICT  ,ISNULL(a.CreateDate,'') as CreateDate , ISNULL(Series,0) as Series,ISNULL(a.TaxDate,'') as TaxDate, ISNULL(a.FinncPriod,0) as FinncPriod ,\r\n                             ISNULL(UserSign,0) as UserSign, ISNULL(UserSign2,0) as UserSign2  ,ISNULL(a.VatDate,'') as  VatDate , ISNULL(PaidSum,0) as PaidSum ,\r\n                             ISNULL(PaidSumFc,0) as PaidSumFc , ISNULL(PaidSumSc,0) as PaidSumSc  , ISNULL(Cashremainder,0) as Cashremainder \r\n                             ,IsRIV   ,CurSource   ,ShipToCode ,PayToCode  ,Confirmed  ,Rounding  ,Address2 \r\n                                        from opch a inner join vpm2 b on a.DocEntry=b.DocEntry \r\n\t\t                          where b.docnum =" + docNum;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetPurposeOutgoingPayment(string docEntry)
	{
		try
		{
			string query = "SELECT [DocNum]\r\n                                      ,[LineId]\r\n                                      ,[AcctCode]\r\n                                      ,[AcctName]\r\n                                      ,[Amount]\r\n                                      ,[SumApplied]\r\n                                      ,[Descrip]\r\n                                      ,[VatGroup]\r\n                                      ,[VatPrcnt]\r\n                                      ,[ObjType]\r\n                                      ,[VatAmnt]\r\n                                  FROM [dbo].[VPM4]\r\n\t\t                            where DocNum=" + docEntry;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ShowPaymentMeansCreditCardType()
	{
		try
		{
			string query = "select * from OCRC where Locked='N'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ShowPaymentMeansCurrency()
	{
		try
		{
			string query = "select * from OCRN where Locked='N'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAccountPayment(string docNums)
	{
		try
		{
			docNums = docNums.Replace(",", "','");
			docNums = "'" + docNums + "'";
			string query = "select \r\n                                      Code as [AcctCode]\r\n                                      ,Name as [AcctName]\r\n                                      ,0 as [Amount]\r\n                                      ,0 as [SumApplied]\r\n                                      ,'' as [Descrip]\r\n                                      ,0 as [VatGroup]\r\n                                      ,0 as [VatPrcnt]\r\n                                      ,24 as [ObjType]\r\n                                      ,0 as [VatAmnt]\r\n                                from OPPU where code in (" + docNums + ")";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable LoadUDFData(string docEntry)
	{
		try
		{
			List<string> uDFField = getUDFField();
			string text = "";
			foreach (string item in uDFField)
			{
				text = text + item + ",";
			}
			text = text.Remove(text.Length - 1);
			string query = "select " + text + " from ovpm where docentry=" + docEntry;
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private List<string> getUDFField()
	{
		try
		{
			string query = "select FieldName from tblUFieldTable where TableName='ovpm'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			List<string> list = new List<string>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(dataTable.Rows[i][0].ToString());
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new List<string>();
		}
	}

	public DataTable LoadPaymentMeansByOutgoing(string docnum)
	{
		try
		{
			string query = "select \r\n                    a.CreditCard as crCreditCard,\r\n                    a.CreditCur as CreditCur,\r\n                    b.CouponNum ,\r\n                    b.OwnerAdd as coOwnerAdd,\r\n                    b.CouponName,\r\n                    b.OwnerIdNum as coOwnerIdNum,\r\n                    b.OwnerPhone as coOwnerPhone,\r\n                    a.OwnerAdd as crOwnerAdd,\r\n                    a.ConfNum,\r\n                    a.OwnerIdNum as crOwnerIdNum,\r\n                    a.OwnerPhone as crOwnerPhone,\r\n                    a.crCardNum,\r\n\t\t\t\t\t\tb.CardValid as coCardValid,\r\n\t\t\t\t\t\ta.CardValid as crCardValid\r\n                from vpm3 a \r\n                    inner join vpm9 b on a.DocNum=b.DocNum where a.DocNum=" + docnum;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowOITT ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNext(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = " WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM OVPM)\r\n                                SELECT \r\n                                          isnull(next.DocEntry,0) as DocEntry\r\n                                          ,isnull(next.DocNum,0) as  DocNum \r\n                                          ,isnull(next.DocType,'') as  DocType \r\n                                          ,isnull(next.Canceled,'') as  Canceled \r\n                                          ,isnull(next.Printed,'') as  Printed \r\n                                          ,isnull(next.DocDate,'') as  DocDate \r\n                                          ,isnull(next.DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(next.CardCode,'') as  CardCode \r\n                                          ,isnull(next.CardName,'') as  CardName \r\n                                          ,isnull(next.Address,'') as  Address \r\n                                          ,isnull(next.CashAcct,'') as  CashAcct \r\n                                          ,isnull(next.CashSum,0) as  CashSum \r\n                                          ,isnull(next.CreditSum,0) as  CreditSum \r\n                                          ,isnull(next.CouponSum,0) as  CouponSum \r\n                                          ,isnull(next.TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(next.TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(next.TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(next.TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(next.DocCurr,'') as  DocCurr \r\n                                          ,isnull(next.DocRate,0) as  DocRate \r\n                                          ,isnull(next.DocTotal,0) as  DocTotal \r\n                                          ,isnull(next.JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(next.TransId,0) as  TransId \r\n                                          ,isnull(next.DocTime,'') as  DocTime \r\n                                          ,isnull(next.ObjType,'') as  ObjType \r\n                                          ,isnull(next.UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(next.CreateDate,'') as  CreateDate \r\n                                          ,isnull(next.UserSign,0) as  UserSign \r\n                                          ,isnull(next.VatGroup,'') as  VatGroup \r\n                                          ,isnull(next.VatSum,0) as  VatSum \r\n                                          ,isnull(next.Dcount,0) as  Dcount \r\n                                          ,isnull(next.DcntSum,0) as  DcntSum \r\n                                          ,isnull(next.CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(next.CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(next.TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(next.CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(next.CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(next.TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(next.VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(next.VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(next.DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(next.DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(next.DocTotalSy,0) as  DocTotalSy\r\n                                FROM CTE \r\n                                LEFT JOIN CTE next ON next.RN = CTE.RN + 1\r\n                     WHERE CTE.[DocEntry] =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetPrevious(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = " WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM OVPM)\r\n                                SELECT \r\n                                          isnull(prev.DocEntry,0) as DocEntry\r\n                                          ,isnull(prev.DocNum,0) as  DocNum \r\n                                          ,isnull(prev.DocType,'') as  DocType \r\n                                          ,isnull(prev.Canceled,'') as  Canceled \r\n                                          ,isnull(prev.Printed,'') as  Printed \r\n                                          ,isnull(prev.DocDate,'') as  DocDate \r\n                                          ,isnull(prev.DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(prev.CardCode,'') as  CardCode \r\n                                          ,isnull(prev.CardName,'') as  CardName \r\n                                          ,isnull(prev.Address,'') as  Address \r\n                                          ,isnull(prev.CashAcct,'') as  CashAcct \r\n                                          ,isnull(prev.CashSum,0) as  CashSum \r\n                                          ,isnull(prev.CreditSum,0) as  CreditSum \r\n                                          ,isnull(prev.CouponSum,0) as  CouponSum \r\n                                          ,isnull(prev.TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(prev.TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(prev.TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(prev.TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(prev.DocCurr,'') as  DocCurr \r\n                                          ,isnull(prev.DocRate,0) as  DocRate \r\n                                          ,isnull(prev.DocTotal,0) as  DocTotal \r\n                                          ,isnull(prev.JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(prev.TransId,0) as  TransId \r\n                                          ,isnull(prev.DocTime,'') as  DocTime \r\n                                          ,isnull(prev.ObjType,'') as  ObjType \r\n                                          ,isnull(prev.UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(prev.CreateDate,'') as  CreateDate \r\n                                          ,isnull(prev.UserSign,0) as  UserSign \r\n                                          ,isnull(prev.VatGroup,'') as  VatGroup \r\n                                          ,isnull(prev.VatSum,0) as  VatSum \r\n                                          ,isnull(prev.Dcount,0) as  Dcount \r\n                                          ,isnull(prev.DcntSum,0) as  DcntSum \r\n                                          ,isnull(prev.CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(prev.CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(prev.TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(prev.CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(prev.CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(prev.TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(prev.VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(prev.VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(prev.DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(prev.DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(prev.DocTotalSy,0) as  DocTotalSy\r\n                                FROM CTE \r\n                                LEFT JOIN CTE prev ON prev.RN = CTE.RN - 1\r\n                     WHERE CTE.[DocEntry] =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetFirst()
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "SELECT top 1      isnull(DocEntry,0) as DocEntry\r\n                                          ,isnull(DocNum,0) as  DocNum \r\n                                          ,isnull(DocType,'') as  DocType \r\n                                          ,isnull(Canceled,'') as  Canceled \r\n                                          ,isnull(Printed,'') as  Printed \r\n                                          ,isnull(DocDate,'') as  DocDate \r\n                                          ,isnull(DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(CardCode,'') as  CardCode \r\n                                          ,isnull(CardName,'') as  CardName \r\n                                          ,isnull(Address,'') as  Address \r\n                                          ,isnull(CashAcct,'') as  CashAcct \r\n                                          ,isnull(CashSum,0) as  CashSum \r\n                                          ,isnull(CreditSum,0) as  CreditSum \r\n                                          ,isnull(CouponSum,0) as  CouponSum \r\n                                          ,isnull(TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(DocCurr,'') as  DocCurr \r\n                                          ,isnull(DocRate,0) as  DocRate \r\n                                          ,isnull(DocTotal,0) as  DocTotal \r\n                                          ,isnull(JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(TransId,0) as  TransId \r\n                                          ,isnull(DocTime,'') as  DocTime \r\n                                          ,isnull(ObjType,'') as  ObjType \r\n                                          ,isnull(UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(CreateDate,'') as  CreateDate \r\n                                          ,isnull(UserSign,0) as  UserSign \r\n                                          ,isnull(VatGroup,'') as  VatGroup \r\n                                          ,isnull(VatSum,0) as  VatSum \r\n                                          ,isnull(Dcount,0) as  Dcount \r\n                                          ,isnull(DcntSum,0) as  DcntSum \r\n                                          ,isnull(CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(DocTotalSy,0) as  DocTotalSy \r\n                                    FROM  OVPM";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetLast()
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "SELECT top 1      isnull(DocEntry,0) as DocEntry\r\n                                          ,isnull(DocNum,0) as  DocNum \r\n                                          ,isnull(DocType,'') as  DocType \r\n                                          ,isnull(Canceled,'') as  Canceled \r\n                                          ,isnull(Printed,'') as  Printed \r\n                                          ,isnull(DocDate,'') as  DocDate \r\n                                          ,isnull(DocDueDate,'') as  DocDueDate \r\n                                          ,isnull(CardCode,'') as  CardCode \r\n                                          ,isnull(CardName,'') as  CardName \r\n                                          ,isnull(Address,'') as  Address \r\n                                          ,isnull(CashAcct,'') as  CashAcct \r\n                                          ,isnull(CashSum,0) as  CashSum \r\n                                          ,isnull(CreditSum,0) as  CreditSum \r\n                                          ,isnull(CouponSum,0) as  CouponSum \r\n                                          ,isnull(TrsfrAcct,'') as  TrsfrAcct \r\n                                          ,isnull(TrsfrSum,0) as  TrsfrSum \r\n                                          ,isnull(TrsfrDate,'') as  TrsfrDate \r\n                                          ,isnull(TrsfrRef,'') as  TrsfrRef \r\n                                          ,isnull(DocCurr,'') as  DocCurr \r\n                                          ,isnull(DocRate,0) as  DocRate \r\n                                          ,isnull(DocTotal,0) as  DocTotal \r\n                                          ,isnull(JrnlMemo,'') as  JrnlMemo \r\n                                          ,isnull(TransId,0) as  TransId \r\n                                          ,isnull(DocTime,'') as  DocTime \r\n                                          ,isnull(ObjType,'') as  ObjType \r\n                                          ,isnull(UpdateDate,'') as  UpdateDate \r\n                                          ,isnull(CreateDate,'') as  CreateDate \r\n                                          ,isnull(UserSign,0) as  UserSign \r\n                                          ,isnull(VatGroup,'') as  VatGroup \r\n                                          ,isnull(VatSum,0) as  VatSum \r\n                                          ,isnull(Dcount,0) as  Dcount \r\n                                          ,isnull(DcntSum,0) as  DcntSum \r\n                                          ,isnull(CashSumFC,0) as  CashSumFC \r\n                                          ,isnull(CreditSumFC,0) as  CreditSumFC \r\n                                          ,isnull(TrsfrSumFC,0) as  TrsfrSumFC \r\n                                          ,isnull(CashSumSy,0) as  CashSumSy \r\n                                          ,isnull(CreditSumSy,0) as  CreditSumSy \r\n                                          ,isnull(TrsfrSumSy,0) as  TrsfrSumSy \r\n                                          ,isnull(VatSumFC,0) as  VatSumFC \r\n                                          ,isnull(VatSumSy,0) as  VatSumSy \r\n                                          ,isnull(DcntSumFC,0) as  DcntSumFC \r\n                                          ,isnull(DocTotalFC,0) as  DocTotalFC \r\n                                          ,isnull(DocTotalSy,0) as  DocTotalSy \r\n                                    FROM  OVPM ORDER BY DocEntry DESC ";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("APZ_CFL_ListOutgoingPayment", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
