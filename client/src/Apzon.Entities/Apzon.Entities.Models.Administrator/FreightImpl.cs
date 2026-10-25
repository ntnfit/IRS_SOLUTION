namespace Apzon.Entities.Models.Administrator;

public class FreightImpl
{
	public int ExpnsCode { get; set; }

	public string ExpnsName { get; set; }

	public string RevAcct { get; set; }

	public string ExpnsAcct { get; set; }

	public string VatGroupO { get; set; }

	public string VatGroupI { get; set; }

	public long RevFixSum { get; set; }

	public long ExpFixSum { get; set; }

	public char? WTLiable { get; set; }

	public char? DistrbMthd { get; set; }

	public char? BaseMethod { get; set; }

	public char? Stock { get; set; }

	public char? LstPchPrce { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string Project { get; set; }
}
