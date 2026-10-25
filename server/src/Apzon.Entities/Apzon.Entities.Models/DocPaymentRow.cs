using System;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Entities.Models;

public class DocPaymentRow : Row<DocPayment>
{
	public string Code
	{
		get
		{
			return Function.ToString(base["Code"]);
		}
		set
		{
			if (!Table.Columns.Contains("Code"))
			{
				Table.Columns.Add("Code", typeof(string));
			}
			base["Code"] = value;
		}
	}

	public string Name
	{
		get
		{
			return Function.ToString(base["Name"]);
		}
		set
		{
			if (!Table.Columns.Contains("Name"))
			{
				Table.Columns.Add("Name", typeof(string));
			}
			base["Name"] = value;
		}
	}

	public int Payments
	{
		get
		{
			return Function.ParseInt(base["Payments"]);
		}
		set
		{
			if (!Table.Columns.Contains("Payments"))
			{
				Table.Columns.Add("Payments", typeof(int));
			}
			base["Payments"] = value;
		}
	}

	public string Description
	{
		get
		{
			return Function.ToString(base["Description"]);
		}
		set
		{
			if (!Table.Columns.Contains("Description"))
			{
				Table.Columns.Add("Description", typeof(string));
			}
			base["Description"] = value;
		}
	}

	public DateTime? DueDate
	{
		get
		{
			return Function.ParseDateTime(base["DueDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("DueDate"))
			{
				Table.Columns.Add("DueDate", typeof(DateTime));
			}
			base["DueDate"] = value;
		}
	}

	public double Amount
	{
		get
		{
			return Function.ParseDouble(base["Amount"]);
		}
		set
		{
			if (!Table.Columns.Contains("Amount"))
			{
				Table.Columns.Add("Amount", typeof(double));
			}
			base["Amount"] = value;
		}
	}

	public double Percents
	{
		get
		{
			return Function.ParseDouble(base["Percents"]);
		}
		set
		{
			if (!Table.Columns.Contains("Percents"))
			{
				Table.Columns.Add("Percents", typeof(double));
			}
			base["Percents"] = value;
		}
	}

	public double Total
	{
		get
		{
			return Function.ParseDouble(base["Total"]);
		}
		set
		{
			if (!Table.Columns.Contains("Total"))
			{
				Table.Columns.Add("Total", typeof(double));
			}
			base["Total"] = value;
		}
	}

	public double Paid
	{
		get
		{
			return Function.ParseDouble(base["Paid"]);
		}
		set
		{
			if (!Table.Columns.Contains("Paid"))
			{
				Table.Columns.Add("Paid", typeof(double));
			}
			base["Paid"] = value;
		}
	}

	public double BalanceDue
	{
		get
		{
			return Function.ParseDouble(base["BalanceDue"]);
		}
		set
		{
			if (!Table.Columns.Contains("BalanceDue"))
			{
				Table.Columns.Add("BalanceDue", typeof(double));
			}
			base["BalanceDue"] = value;
		}
	}

	public int ObjType
	{
		get
		{
			return Function.ParseInt(base["ObjType"]);
		}
		set
		{
			if (!Table.Columns.Contains("ObjType"))
			{
				Table.Columns.Add("ObjType", typeof(int));
			}
			base["ObjType"] = value;
		}
	}

	public int DocEntry
	{
		get
		{
			return Function.ParseInt(base["DocEntry"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocEntry"))
			{
				Table.Columns.Add("DocEntry", typeof(int));
			}
			base["DocEntry"] = value;
		}
	}

	public string Currency
	{
		get
		{
			return Function.ToString(base["Currency"]);
		}
		set
		{
			if (!Table.Columns.Contains("Currency"))
			{
				Table.Columns.Add("Currency", typeof(string));
			}
			base["Currency"] = value;
		}
	}

	internal DocPaymentRow(DataRowBuilder builder)
		: base(builder)
	{
	}
}
