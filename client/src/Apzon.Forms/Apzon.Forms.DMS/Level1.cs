using System.Collections.Generic;

namespace Apzon.Forms.DMS;

public class Level1
{
	public string level1_id { get; set; }

	public string name { get; set; }

	public string type { get; set; }

	public List<Level2s> level2s { get; set; }
}
