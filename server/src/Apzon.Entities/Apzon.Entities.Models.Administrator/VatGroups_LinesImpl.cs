using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class VatGroups_LinesImpl
{
	public int Count { get; set; }

	public int DatevCode { get; set; }

	public DateTime EffecDate { get; set; }

	public double EqualizationTax { get; set; }

	public double Rate { get; set; }

	public string Code { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
