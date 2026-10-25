using System;

namespace Apzon.Forms.Commons.Helper;

public class EntitiesCountEventArgs : EventArgs
{
	public int Count { get; private set; }

	public EntitiesCountEventArgs(int count)
	{
		Count = count;
	}
}
