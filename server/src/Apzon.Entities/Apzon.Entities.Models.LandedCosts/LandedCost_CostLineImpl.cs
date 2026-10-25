using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.LandedCosts;

public class LandedCost_CostLineImpl
{
	public int DocEntry { get; set; }

	public string AlcCode { get; set; }

	public string OhType { get; set; }

	public double CostSum { get; set; }

	public double CostSumFC { get; set; }

	public double Factor { get; set; }

	public string CostType { get; set; }

	public string InCustCalc { get; set; }

	public double OpenCost { get; set; }

	public double OpenCostFC { get; set; }

	public string AgentCode { get; set; }

	public string AgentName { get; set; }

	public string CostCateg { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
