using System.Collections.Generic;

namespace Apzon.Entities.Models.Documents;

public class Series_ImplONNM
{
	public string ObjectCode { get; set; }

	public int? AutoKey { get; set; }

	public int? DfltSeries { get; set; }

	public int? UpdCounter { get; set; }

	public int? UserSign { get; set; }

	public string DocSubType { get; set; }

	public string DocAlias { get; set; }

	public string PeriodTyp { get; set; }

	public List<Series_LineImplNNM1> lineNNM1 { get; set; }
}
