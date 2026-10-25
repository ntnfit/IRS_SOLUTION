using System;
using System.Transactions;

namespace Apzon.DataServices;

public sealed class Transaction : ITransaction, IDisposable
{
	private TransactionScope _transactionScope;

	public Transaction()
	{
		try
		{
			_transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
			{
				IsolationLevel = IsolationLevel.ReadCommitted,
				Timeout = new TimeSpan(0, 15, 0)
			});
		}
		catch (Exception)
		{
			if (_transactionScope != null)
			{
				_transactionScope.Complete();
				_transactionScope.Dispose();
				_transactionScope = null;
			}
		}
	}

	public void Commit()
	{
		if (_transactionScope != null)
		{
			_transactionScope.Complete();
			_transactionScope.Dispose();
		}
	}

	public void Commplete()
	{
		if (_transactionScope != null)
		{
			_transactionScope.Complete();
		}
	}

	public void CompleteDispose()
	{
		if (_transactionScope != null)
		{
			_transactionScope.Complete();
			_transactionScope.Dispose();
			_transactionScope = null;
		}
	}

	public void Rollback()
	{
		if (_transactionScope != null)
		{
			_transactionScope.Dispose();
		}
	}

	public void Dispose()
	{
		if (_transactionScope != null)
		{
			_transactionScope.Dispose();
			_transactionScope = null;
		}
	}
}
