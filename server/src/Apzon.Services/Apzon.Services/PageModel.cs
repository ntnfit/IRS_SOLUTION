using System;

namespace Apzon.Services;

public class PageModel
{
	public int PgIndex { get; set; }

	public int PgCount
	{
		get
		{
			if (!TotalItems.Equals(0))
			{
				return (int)Math.Ceiling((decimal)TotalItems / (decimal)PgSize);
			}
			return 1;
		}
	}

	public int TotalItems { get; set; }

	public int ItemCounts { get; set; }

	public int PgSize { get; set; }

	public int ItemStart => (PgIndex - 1) * PgSize + 1;

	public int ItemEnd => ItemStart + ItemCounts;

	public int PgShow { get; set; }

	public bool IsPagin { get; set; }

	public PageModel()
	{
		PgIndex = 1;
		PgSize = 10;
		PgShow = 10;
		IsPagin = true;
	}
}
