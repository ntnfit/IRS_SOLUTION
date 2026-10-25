using System;
using System.Collections.Generic;

namespace Apzon.Forms.Commons.Helper;

public class GroupEventArgs<TKey> : EventArgs
{
	private IEnumerable<TKey> keysCore;

	public IEnumerable<TKey> Entities => keysCore;

	public GroupEventArgs(IEnumerable<TKey> keys)
	{
		keysCore = keys;
	}
}
