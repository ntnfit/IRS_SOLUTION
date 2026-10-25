using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Inventory;

public class MaterialRevaluation
{
	public DataBrowser Browser { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string Comments { get; set; }

	public DateTime CreationDate { get; set; }

	public string DataSource { get; set; }

	public DateTime DocDate { get; set; }

	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public DateTime DocTime { get; set; }

	public BoYesNoEnum InflationRevaluation { get; set; }

	public string JournalMemo { get; set; }

	public List<MaterialRevaluation_linesImpl> Lines { get; set; }

	public string Reference1 { get; set; }

	public string Reference2 { get; set; }

	public string RevalType { get; set; }

	public string RevaluationExpenseAccount { get; set; }

	public string RevaluationIncomeAccount { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public int TransNum { get; set; }

	public DateTime UpdateDate { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public int UserSignature { get; set; }

	public string U_Period { get; set; }
}
