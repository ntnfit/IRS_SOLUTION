using System.Collections.Generic;

namespace Apzon.Forms.DMS;

public class Level2s
{
	public string level2_id { get; set; }

	public string name { get; set; }

	public string type { get; set; }

	public List<Level3s> level3s { get; set; }
}
