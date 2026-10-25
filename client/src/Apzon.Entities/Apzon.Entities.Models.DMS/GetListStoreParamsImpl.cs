namespace Apzon.Entities.Models.DMS;

public class GetListStoreParamsImpl
{
	public string cardType { get; set; }

	public string cardName { get; set; }

	public string subType { get; set; }

	public string saleUser { get; set; }

	public string type { get; set; }

	public string filterType { get; set; }

	public string searchText { get; set; }

	public int pageIndex { get; set; }

	public string fromDate { get; set; }

	public string toDate { get; set; }

	public string u_LevelDL { get; set; }

	public string checkApi { get; set; }

	public string filterProperties { get; set; }

	public float latitude { get; set; }

	public float longitude { get; set; }
}
