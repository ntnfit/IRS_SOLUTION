using System;

namespace Apzon.Forms.Commons.Helper;

public class EntityEventArgs<TID> : EventArgs
{
	private TID entityKeyCore;

	public TID Key => entityKeyCore;

	public EntityEventArgs(TID entityKey)
	{
		entityKeyCore = entityKey;
	}
}
