namespace Apzon.Entities.Models.Common;

public class Numbering
{
	public string ObjectCode { get; set; }

	public string Series { get; set; }

	public string SeriesName { get; set; }

	public int InitialNum { get; set; }

	public int NextNumber { get; set; }

	public int LastNum { get; set; }

	public string BeginStr { get; set; }

	public string EndStr { get; set; }

	public string Remark { get; set; }

	public short GroupCode { get; set; }

	public string Locked { get; set; }

	public string YearTransf { get; set; }

	public string Indicator { get; set; }

	public string Template { get; set; }

	public int NumSize { get; set; }

	public string FolioPref { get; set; }

	public int NextFolio { get; set; }

	public string DocSubType { get; set; }

	public short DefESeries { get; set; }

	public string IsDigSerie { get; set; }

	public string SeriesType { get; set; }

	public string IsManual { get; set; }

	public int BPLid { get; set; }
}
