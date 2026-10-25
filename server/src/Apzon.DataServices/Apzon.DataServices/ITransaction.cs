using System;

namespace Apzon.DataServices;

internal interface ITransaction : IDisposable
{
	void Commit();

	void Rollback();
}
