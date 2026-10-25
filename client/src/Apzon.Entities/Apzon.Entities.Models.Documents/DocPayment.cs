using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class DocPayment
{
	public string Code { get; set; }

	public string Name { get; set; }

	public int Payments { get; set; }

	public string Description { get; set; }

	public DateTime DueDate { get; set; }

	public double Amount { get; set; }

	public double Percents { get; set; }

	public double Total { get; set; }

	public double Paid { get; set; }

	public double BalanceDue { get; set; }

	public int ObjType { get; set; }

	public int DocEntry { get; set; }

	public string Currency { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
