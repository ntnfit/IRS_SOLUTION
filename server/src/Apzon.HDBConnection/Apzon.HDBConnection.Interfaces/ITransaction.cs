using System;

namespace Apzon.HDBConnection.Interfaces;

public interface ITransaction : IDisposable
{
	void Commit();

	void Rollback();
}
