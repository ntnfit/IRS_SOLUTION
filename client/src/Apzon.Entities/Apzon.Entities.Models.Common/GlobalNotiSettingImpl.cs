using System.Collections.Generic;

namespace Apzon.Entities.Models.Common;

public class GlobalNotiSettingImpl
{
	public List<PeriodTime> ListTimeReceive { get; set; }

	public bool checkReceiveNoti { get; set; }
}
