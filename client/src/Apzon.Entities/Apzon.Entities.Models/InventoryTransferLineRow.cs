using System;
using System.Data;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Entities.Models;

public class InventoryTransferLineRow : Row<InventoryTranfer_LineImpl>
{
	public int DocEntry
	{
		get
		{
			return Function.ParseInt(base["DocEntry"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocEntry".ToUpper())))
			{
				Table.Columns.Add("DocEntry", typeof(int));
			}
			base["DocEntry"] = value;
		}
	}

	public int LineNum
	{
		get
		{
			return Function.ParseInt(base["LineNum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("LineNum".ToUpper())))
			{
				Table.Columns.Add("LineNum", typeof(int));
			}
			base["LineNum"] = value;
		}
	}

	public int TargetType
	{
		get
		{
			return Function.ParseInt(base["TargetType"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TargetType".ToUpper())))
			{
				Table.Columns.Add("TargetType", typeof(int));
			}
			base["TargetType"] = value;
		}
	}

	public int TrgetEntry
	{
		get
		{
			return Function.ParseInt(base["TrgetEntry"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TrgetEntry".ToUpper())))
			{
				Table.Columns.Add("TrgetEntry", typeof(int));
			}
			base["TrgetEntry"] = value;
		}
	}

	public string BaseRef
	{
		get
		{
			return Function.ToString(base["BaseRef"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseRef".ToUpper())))
			{
				Table.Columns.Add("BaseRef", typeof(string));
			}
			base["BaseRef"] = value;
		}
	}

	public int BaseType
	{
		get
		{
			return Function.ParseInt(base["BaseType"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseType".ToUpper())))
			{
				Table.Columns.Add("BaseType", typeof(int));
			}
			base["BaseType"] = value;
		}
	}

	public int BaseEntry
	{
		get
		{
			return Function.ParseInt(base["BaseEntry"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseEntry".ToUpper())))
			{
				Table.Columns.Add("BaseEntry", typeof(string));
			}
			base["BaseEntry"] = value;
		}
	}

	public int BaseLine
	{
		get
		{
			return Function.ParseInt(base["BaseLine"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseLine".ToUpper())))
			{
				Table.Columns.Add("BaseLine", typeof(string));
			}
			base["BaseLine"] = value;
		}
	}

	public string LineStatus
	{
		get
		{
			return Function.ToString(base["LineStatus"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("LineStatus".ToUpper())))
			{
				Table.Columns.Add("LineStatus", typeof(string));
			}
			base["LineStatus"] = value;
		}
	}

	public string ItemCode
	{
		get
		{
			return Function.ToString(base["ItemCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ItemCode".ToUpper())))
			{
				Table.Columns.Add("ItemCode", typeof(string));
			}
			base["ItemCode"] = value;
		}
	}

	public string Dscription
	{
		get
		{
			return Function.ToString(base["Dscription"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Dscription".ToUpper())))
			{
				Table.Columns.Add("Dscription", typeof(string));
			}
			base["Dscription"] = value;
		}
	}

	public double Quantity
	{
		get
		{
			return Function.ParseDouble(base["Quantity"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Quantity".ToUpper())))
			{
				Table.Columns.Add("Quantity", typeof(double));
			}
			base["Quantity"] = value;
		}
	}

	public double OpenQty
	{
		get
		{
			return Function.ParseDouble(base["OpenQty"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OpenQty".ToUpper())))
			{
				Table.Columns.Add("OpenQty", typeof(double));
			}
			base["OpenQty"] = value;
		}
	}

	public double Price
	{
		get
		{
			return Function.ParseDouble(base["Price"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Price".ToUpper())))
			{
				Table.Columns.Add("Price", typeof(double));
			}
			base["Price"] = value;
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
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Currency".ToUpper())))
			{
				Table.Columns.Add("Currency", typeof(string));
			}
			base["Currency"] = value;
		}
	}

	public double Rate
	{
		get
		{
			return Function.ParseDouble(base["Rate"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Rate".ToUpper())))
			{
				Table.Columns.Add("Rate", typeof(double));
			}
			base["Rate"] = value;
		}
	}

	public double DiscPrcnt
	{
		get
		{
			return Function.ParseDouble(base["DiscPrcnt"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DiscPrcnt".ToUpper())))
			{
				Table.Columns.Add("DiscPrcnt", typeof(double));
			}
			base["DiscPrcnt"] = value;
		}
	}

	public double LineTotal
	{
		get
		{
			return Function.ParseDouble(base["LineTotal"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("LineTotal".ToUpper())))
			{
				Table.Columns.Add("LineTotal", typeof(double));
			}
			base["LineTotal"] = value;
		}
	}

	public double TotalFrgn
	{
		get
		{
			return Function.ParseDouble(base["TotalFrgn"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TotalFrgn".ToUpper())))
			{
				Table.Columns.Add("TotalFrgn", typeof(double));
			}
			base["TotalFrgn"] = value;
		}
	}

	public double OpenSum
	{
		get
		{
			return Function.ParseDouble(base["OpenSum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OpenSum".ToUpper())))
			{
				Table.Columns.Add("OpenSum", typeof(double));
			}
			base["OpenSum"] = value;
		}
	}

	public double OpenSumFC
	{
		get
		{
			return Function.ParseDouble(base["OpenSumFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OpenSumFC".ToUpper())))
			{
				Table.Columns.Add("OpenSumFC", typeof(double));
			}
			base["OpenSumFC"] = value;
		}
	}

	public string WhsCode
	{
		get
		{
			return Function.ToString(base["WhsCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("WhsCode".ToUpper())))
			{
				Table.Columns.Add("WhsCode", typeof(string));
			}
			base["WhsCode"] = value;
		}
	}

	public int SlpCode
	{
		get
		{
			return Function.ParseInt(base["SlpCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("SlpCode".ToUpper())))
			{
				Table.Columns.Add("SlpCode", typeof(int));
			}
			base["SlpCode"] = value;
		}
	}

	public string AcctCode
	{
		get
		{
			return Function.ToString(base["AcctCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("AcctCode".ToUpper())))
			{
				Table.Columns.Add("AcctCode", typeof(string));
			}
			base["AcctCode"] = value;
		}
	}

	public double PriceBefDi
	{
		get
		{
			return Function.ParseDouble(base["PriceBefDi"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PriceBefDi".ToUpper())))
			{
				Table.Columns.Add("PriceBefDi", typeof(double));
			}
			base["PriceBefDi"] = value;
		}
	}

	public DateTime DocDate
	{
		get
		{
			return Function.ParseDateTime(base["DocDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocDate".ToUpper())))
			{
				Table.Columns.Add("DocDate", typeof(DateTime));
			}
			base["DocDate"] = value;
		}
	}

	public string OcrCode
	{
		get
		{
			return Function.ToString(base["OcrCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OcrCode".ToUpper())))
			{
				Table.Columns.Add("OcrCode", typeof(string));
			}
			base["OcrCode"] = value;
		}
	}

	public string Project
	{
		get
		{
			return Function.ToString(base["Project"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Project".ToUpper())))
			{
				Table.Columns.Add("Project", typeof(string));
			}
			base["Project"] = value;
		}
	}

	public string CodeBars
	{
		get
		{
			return Function.ToString(base["CodeBars"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CodeBars".ToUpper())))
			{
				Table.Columns.Add("CodeBars", typeof(string));
			}
			base["CodeBars"] = value;
		}
	}

	public double VatPrcnt
	{
		get
		{
			return Function.ParseDouble(base["VatPrcnt"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatPrcnt".ToUpper())))
			{
				Table.Columns.Add("VatPrcnt", typeof(double));
			}
			base["VatPrcnt"] = value;
		}
	}

	public string VatGroup
	{
		get
		{
			return Function.ToString(base["VatGroup"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatGroup".ToUpper())))
			{
				Table.Columns.Add("VatGroup", typeof(string));
			}
			base["VatGroup"] = value;
		}
	}

	public double PriceAfVAT
	{
		get
		{
			return Function.ParseDouble(base["PriceAfVAT"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PriceAfVAT".ToUpper())))
			{
				Table.Columns.Add("PriceAfVAT", typeof(double));
			}
			base["PriceAfVAT"] = value;
		}
	}

	public int BaseDocNum
	{
		get
		{
			return Function.ParseInt(base["BaseDocNum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseDocNum".ToUpper())))
			{
				Table.Columns.Add("BaseDocNum", typeof(int));
			}
			base["BaseDocNum"] = value;
		}
	}

	public double VatSum
	{
		get
		{
			return Function.ParseDouble(base["VatSum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatSum".ToUpper())))
			{
				Table.Columns.Add("VatSum", typeof(double));
			}
			base["VatSum"] = value;
		}
	}

	public double VatSumFrgn
	{
		get
		{
			return Function.ParseDouble(base["VatSumFrgn"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatSumFrgn".ToUpper())))
			{
				Table.Columns.Add("VatSumFrgn", typeof(double));
			}
			base["VatSumFrgn"] = value;
		}
	}

	public double VatSumSy
	{
		get
		{
			return Function.ParseDouble(base["VatSumSy"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatSumSy".ToUpper())))
			{
				Table.Columns.Add("VatSumSy", typeof(double));
			}
			base["VatSumSy"] = value;
		}
	}

	public int FinncPriod
	{
		get
		{
			return Function.ParseInt(base["FinncPriod"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("FinncPriod".ToUpper())))
			{
				Table.Columns.Add("FinncPriod", typeof(int));
			}
			base["FinncPriod"] = value;
		}
	}

	public string ObjType
	{
		get
		{
			return Function.ToString(base["ObjType"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ObjType".ToUpper())))
			{
				Table.Columns.Add("ObjType", typeof(string));
			}
			base["ObjType"] = value;
		}
	}

	public double GrssProfit
	{
		get
		{
			return Function.ParseDouble(base["GrssProfit"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GrssProfit".ToUpper())))
			{
				Table.Columns.Add("GrssProfit", typeof(double));
			}
			base["GrssProfit"] = value;
		}
	}

	public double GrssProfFC
	{
		get
		{
			return Function.ParseDouble(base["GrssProfFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GrssProfFC".ToUpper())))
			{
				Table.Columns.Add("GrssProfFC", typeof(double));
			}
			base["GrssProfFC"] = value;
		}
	}

	public string TaxCode
	{
		get
		{
			return Function.ToString(base["TaxCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TaxCode".ToUpper())))
			{
				Table.Columns.Add("TaxCode", typeof(string));
			}
			base["TaxCode"] = value;
		}
	}

	public double BaseQty
	{
		get
		{
			return Function.ParseDouble(base["BaseQty"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseQty".ToUpper())))
			{
				Table.Columns.Add("BaseQty", typeof(double));
			}
			base["BaseQty"] = value;
		}
	}

	public double BaseOpnQty
	{
		get
		{
			return Function.ParseDouble(base["BaseOpnQty"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("BaseOpnQty".ToUpper())))
			{
				Table.Columns.Add("BaseOpnQty", typeof(double));
			}
			base["BaseOpnQty"] = value;
		}
	}

	public double VatDscntPr
	{
		get
		{
			return Function.ParseDouble(base["VatDscntPr"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatDscntPr".ToUpper())))
			{
				Table.Columns.Add("VatDscntPr", typeof(double));
			}
			base["VatDscntPr"] = value;
		}
	}

	public double GTotal
	{
		get
		{
			return Function.ParseDouble(base["GTotal"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GTotal".ToUpper())))
			{
				Table.Columns.Add("GTotal", typeof(double));
			}
			base["GTotal"] = value;
		}
	}

	public double GTotalFC
	{
		get
		{
			return Function.ParseDouble(base["GTotalFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GTotalFC".ToUpper())))
			{
				Table.Columns.Add("GTotalFC", typeof(double));
			}
			base["GTotalFC"] = value;
		}
	}

	public string OcrCode2
	{
		get
		{
			return Function.ToString(base["OcrCode2"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OcrCode2".ToUpper())))
			{
				Table.Columns.Add("OcrCode2", typeof(string));
			}
			base["OcrCode2"] = value;
		}
	}

	public string OcrCode3
	{
		get
		{
			return Function.ToString(base["OcrCode3"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OcrCode3".ToUpper())))
			{
				Table.Columns.Add("OcrCode3", typeof(string));
			}
			base["OcrCode3"] = value;
		}
	}

	public string OcrCode4
	{
		get
		{
			return Function.ToString(base["OcrCode4"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("OcrCode4".ToUpper())))
			{
				Table.Columns.Add("OcrCode4", typeof(string));
			}
			base["OcrCode4"] = value;
		}
	}

	public string CogsOcrCo2
	{
		get
		{
			return Function.ToString(base["CogsOcrCo2"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CogsOcrCo2".ToUpper())))
			{
				Table.Columns.Add("CogsOcrCo2", typeof(string));
			}
			base["CogsOcrCo2"] = value;
		}
	}

	public string CogsOcrCo3
	{
		get
		{
			return Function.ToString(base["CogsOcrCo3"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CogsOcrCo3".ToUpper())))
			{
				Table.Columns.Add("CogsOcrCo3", typeof(string));
			}
			base["CogsOcrCo3"] = value;
		}
	}

	public string CogsOcrCo4
	{
		get
		{
			return Function.ToString(base["CogsOcrCo4"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CogsOcrCo4".ToUpper())))
			{
				Table.Columns.Add("CogsOcrCo4", typeof(string));
			}
			base["CogsOcrCo4"] = value;
		}
	}

	public string CogsOcrCo5
	{
		get
		{
			return Function.ToString(base["CogsOcrCo5"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CogsOcrCo5".ToUpper())))
			{
				Table.Columns.Add("CogsOcrCo5", typeof(string));
			}
			base["CogsOcrCo5"] = value;
		}
	}

	public decimal Factor1
	{
		get
		{
			return Function.ParseDecimal(base["Factor1"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Factor1".ToUpper())))
			{
				Table.Columns.Add("Factor1", typeof(decimal));
			}
			base["Factor1"] = value;
		}
	}

	public decimal Factor2
	{
		get
		{
			return Function.ParseDecimal(base["Factor2"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Factor2".ToUpper())))
			{
				Table.Columns.Add("Factor2", typeof(decimal));
			}
			base["Factor2"] = value;
		}
	}

	public decimal Factor3
	{
		get
		{
			return Function.ParseDecimal(base["Factor3"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Factor3".ToUpper())))
			{
				Table.Columns.Add("Factor3", typeof(decimal));
			}
			base["Factor3"] = value;
		}
	}

	public decimal Factor4
	{
		get
		{
			return Function.ParseDecimal(base["Factor4"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Factor4".ToUpper())))
			{
				Table.Columns.Add("Factor4", typeof(decimal));
			}
			base["Factor4"] = value;
		}
	}

	public string UseBaseUn
	{
		get
		{
			return Function.ToString(base["UseBaseUn"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UseBaseUn".ToUpper())))
			{
				Table.Columns.Add("UseBaseUn", typeof(string));
			}
			base["UseBaseUn"] = value;
		}
	}

	public string unitMsr
	{
		get
		{
			return Function.ToString(base["unitMsr"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("unitMsr".ToUpper())))
			{
				Table.Columns.Add("unitMsr", typeof(string));
			}
			base["unitMsr"] = value;
		}
	}

	public double NumPerMsr
	{
		get
		{
			return Function.ParseDouble(base["NumPerMsr"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("NumPerMsr".ToUpper())))
			{
				Table.Columns.Add("NumPerMsr", typeof(double));
			}
			base["NumPerMsr"] = value;
		}
	}

	public string FromWhsCod
	{
		get
		{
			return Function.ToString(base["FromWhsCod"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("FromWhsCod".ToUpper())))
			{
				Table.Columns.Add("FromWhsCod", typeof(string));
			}
			base["FromWhsCod"] = value;
		}
	}

	public decimal QuantityOnStock
	{
		get
		{
			return Function.ParseDecimal(base["QuantityOnStock"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("QuantityOnStock".ToUpper())))
			{
				Table.Columns.Add("QuantityOnStock", typeof(decimal));
			}
			base["QuantityOnStock"] = value;
		}
	}

	public string UoMName
	{
		get
		{
			return Function.ToString(base["UoMName"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UoMName".ToUpper())))
			{
				Table.Columns.Add("UoMName", typeof(string));
			}
			base["UoMName"] = value;
		}
	}

	public string UoMCode
	{
		get
		{
			return Function.ToString(base["UoMCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UoMCode".ToUpper())))
			{
				Table.Columns.Add("UoMCode", typeof(string));
			}
			base["UoMCode"] = value;
		}
	}

	internal InventoryTransferLineRow(DataRowBuilder builder)
		: base(builder)
	{
	}
}
