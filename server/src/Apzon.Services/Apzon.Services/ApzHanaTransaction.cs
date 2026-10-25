using System;
using Apzon.HDBConnection.Interfaces;
using Sap.Data.Hana;

namespace Apzon.Services;

public class ApzHanaTransaction : ITransaction, IDisposable
{
	public HanaTransaction Transaction;

	public ApzHanaTransaction(HanaTransaction transaction)
	{
		Transaction = transaction;
	}

	public void Commit()
	{
		Transaction.Commit();
		Transaction.Dispose();
	}

	public void Rollback()
	{
		Transaction.Rollback();
		Transaction.Dispose();
	}

	public void Dispose()
	{
		Transaction.Dispose();
	}
}
