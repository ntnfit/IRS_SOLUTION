using System;
using System.Transactions;
using Apzon.HDBConnection.Interfaces;

namespace Apzon.HDBConnection;

public class Transaction : ITransaction, IDisposable
{
	private TransactionScope _transactionScope;

	public Transaction(IsolationLevel level = IsolationLevel.ReadCommitted, double timeout = 15.0)
	{
		try
		{
			TransactionOptions transactionOptions = new TransactionOptions
			{
				IsolationLevel = level,
				Timeout = TimeSpan.FromMinutes(timeout)
			};
			_transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions);
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
		_transactionScope.Complete();
		_transactionScope.Dispose();
	}

	public void Commplete()
	{
		_transactionScope.Complete();
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
		_transactionScope.Dispose();
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
