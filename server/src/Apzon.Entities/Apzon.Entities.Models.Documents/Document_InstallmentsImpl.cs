using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class Document_InstallmentsImpl
{
	public int Count { get; set; }

	public DateTime DueDate { get; set; }

	public int DunningLevel { get; set; }

	public int InstallmentId { get; set; }

	public DateTime LastDunningDate { get; set; }

	public BoYesNoEnum PaymentOrdered { get; set; }

	public double Percentage { get; set; }

	public double Total { get; set; }

	public double TotalFC { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
