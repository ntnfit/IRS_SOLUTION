namespace Apzon.Entities.Models.Administrator;

public class GroupCustomerImpl
{
	public short GroupCode { get; set; }

	public string GroupName { get; set; }

	public string GroupType { get; set; }

	public string Locked { get; set; }

	public string DataSource { get; set; }

	public short? UserSign { get; set; }

	public short? PriceList { get; set; }

	public string DiscRel { get; set; }
}
