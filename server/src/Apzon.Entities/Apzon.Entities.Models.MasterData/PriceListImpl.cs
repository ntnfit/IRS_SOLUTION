using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.MasterData;

public class PriceListImpl
{
	public bool Active { get; set; }

	public int BasePriceList { get; set; }

	public string DefaultAdditionalCurrency1 { get; set; }

	public string DefaultAdditionalCurrency2 { get; set; }

	public string DefaultPrimeCurrency { get; set; }

	public double Factor { get; set; }

	public double FixedAmount { get; set; }

	public string GroupNum { get; set; }

	public bool IsGrossPrice { get; set; }

	public string PriceListName { get; set; }

	public int PriceListNo { get; set; }

	public string RoundingMethod { get; set; }

	public string RoundingRule { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public DateTime ValidFrom { get; set; }

	public DateTime ValidTo { get; set; }

	public int UserSign { get; set; }
}
