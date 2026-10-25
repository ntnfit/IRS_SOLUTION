using System;
using System.Data;
using Apzon.Commons;

namespace Apzon.Entities.Models;

public class DocumentLineRow : DataRow
{
	public bool FromExcel
	{
		get
		{
			if (!Table.Columns.Contains("FromExcel"))
			{
				Table.Columns.Add("FromExcel", typeof(bool));
			}
			return Function.ParseBool(base["FromExcel"]);
		}
		set
		{
			if (!Table.Columns.Contains("FromExcel"))
			{
				Table.Columns.Add("FromExcel", typeof(bool));
			}
			base["FromExcel"] = value;
		}
	}

	public int DocEntry
	{
		get
		{
			if (!Table.Columns.Contains("DocEntry"))
			{
				Table.Columns.Add("DocEntry", typeof(int));
			}
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

	public string AccountCode
	{
		get
		{
			if (!Table.Columns.Contains("AcctCode"))
			{
				Table.Columns.Add("AcctCode", typeof(string));
			}
			return Function.ToString(base["AcctCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("AcctCode"))
			{
				Table.Columns.Add("AcctCode", typeof(string));
			}
			base["AcctCode"] = value;
		}
	}

	public string ItemCode
	{
		get
		{
			if (!Table.Columns.Contains("ItemCode"))
			{
				Table.Columns.Add("ItemCode", typeof(string));
			}
			return Function.ToString(base["ItemCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("ItemCode"))
			{
				Table.Columns.Add("ItemCode", typeof(string));
			}
			base["ItemCode"] = value;
		}
	}

	public string LineType
	{
		get
		{
			if (!Table.Columns.Contains("LineType"))
			{
				Table.Columns.Add("LineType", typeof(string));
			}
			return Function.ToString(base["LineType"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineType"))
			{
				Table.Columns.Add("LineType", typeof(string));
			}
			base["LineType"] = value;
		}
	}

	public string SubCatNum
	{
		get
		{
			if (!Table.Columns.Contains("SubCatNum"))
			{
				Table.Columns.Add("SubCatNum", typeof(string));
			}
			return Function.ToString(base["SubCatNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("SubCatNum"))
			{
				Table.Columns.Add("SubCatNum", typeof(string));
			}
			base["SubCatNum"] = value;
		}
	}

	public string UseBaseUnits
	{
		get
		{
			if (!Table.Columns.Contains("UseBaseUn"))
			{
				Table.Columns.Add("UseBaseUn", typeof(string));
			}
			return Function.ToString(base["UseBaseUn"]);
		}
		set
		{
			if (!Table.Columns.Contains("UseBaseUn"))
			{
				Table.Columns.Add("UseBaseUn", typeof(string));
			}
			base["UseBaseUn"] = value;
		}
	}

	public string ItemDescription
	{
		get
		{
			if (!Table.Columns.Contains("Dscription"))
			{
				Table.Columns.Add("Dscription", typeof(string));
			}
			return Function.ToString(base["Dscription"]);
		}
		set
		{
			if (!Table.Columns.Contains("Dscription"))
			{
				Table.Columns.Add("Dscription", typeof(string));
			}
			base["Dscription"] = value;
		}
	}

	public string VatInActive
	{
		get
		{
			if (!Table.Columns.Contains("VatInActive"))
			{
				Table.Columns.Add("VatInActive", typeof(string));
			}
			return Function.ToString(base["VatInActive"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatInActive"))
			{
				Table.Columns.Add("VatInActive", typeof(string));
			}
			base["VatInActive"] = value;
		}
	}

	public string Currency
	{
		get
		{
			if (!Table.Columns.Contains("Currency"))
			{
				Table.Columns.Add("Currency", typeof(string));
			}
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

	public int LineNum
	{
		get
		{
			if (!Table.Columns.Contains("LineNum"))
			{
				DataColumn dataColumn = new DataColumn("LineNum", typeof(int));
				dataColumn.DefaultValue = -1;
				Table.Columns.Add(dataColumn);
			}
			return Function.ParseInt(base["LineNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineNum"))
			{
				DataColumn dataColumn = new DataColumn("LineNum", typeof(int));
				dataColumn.DefaultValue = -1;
				Table.Columns.Add(dataColumn);
			}
			base["LineNum"] = value;
		}
	}

	public int VisOrder
	{
		get
		{
			if (!Table.Columns.Contains("VisOrder"))
			{
				Table.Columns.Add("LineVisOrderNum", typeof(int));
			}
			return Function.ParseInt(base["VisOrder"]);
		}
		set
		{
			if (!Table.Columns.Contains("VisOrder"))
			{
				Table.Columns.Add("VisOrder", typeof(int));
			}
			base["VisOrder"] = value;
		}
	}

	public int BaseType
	{
		get
		{
			if (!Table.Columns.Contains("BaseType"))
			{
				Table.Columns.Add("BaseType", typeof(int));
			}
			return Function.ParseInt(base["BaseType"]);
		}
		set
		{
			if (!Table.Columns.Contains("BaseType"))
			{
				Table.Columns.Add("BaseType", typeof(int));
			}
			base["BaseType"] = value;
		}
	}

	public double Quantity
	{
		get
		{
			if (!Table.Columns.Contains("Quantity"))
			{
				Table.Columns.Add("Quantity", typeof(double));
			}
			return Function.ParseDouble(base["Quantity"]);
		}
		set
		{
			if (!Table.Columns.Contains("Quantity"))
			{
				Table.Columns.Add("Quantity", typeof(double));
			}
			base["Quantity"] = value;
		}
	}

	public double UnitsOfMeasurment
	{
		get
		{
			if (!Table.Columns.Contains("NumPerMsr"))
			{
				Table.Columns.Add("NumPerMsr", typeof(double));
			}
			return Function.ParseDouble(base["NumPerMsr"]);
		}
		set
		{
			if (!Table.Columns.Contains("NumPerMsr"))
			{
				Table.Columns.Add("NumPerMsr", typeof(double));
			}
			base["NumPerMsr"] = value;
		}
	}

	public double UnitPrice
	{
		get
		{
			if (!Table.Columns.Contains("UnitPrice"))
			{
				Table.Columns.Add("UnitPrice", typeof(double));
			}
			return Function.ParseDouble(base["UnitPrice"]);
		}
		set
		{
			if (!Table.Columns.Contains("UnitPrice"))
			{
				Table.Columns.Add("UnitPrice", typeof(double));
			}
			base["UnitPrice"] = value;
		}
	}

	public double Price
	{
		get
		{
			if (!Table.Columns.Contains("Price"))
			{
				Table.Columns.Add("Price", typeof(double));
			}
			return Function.ParseDouble(base["Price"]);
		}
		set
		{
			if (!Table.Columns.Contains("Price"))
			{
				Table.Columns.Add("Price", typeof(double));
			}
			base["Price"] = value;
		}
	}

	public double PriceAfVAT
	{
		get
		{
			if (!Table.Columns.Contains("PriceAfVAT"))
			{
				Table.Columns.Add("PriceAfVAT", typeof(double));
			}
			return Function.ParseDouble(base["PriceAfVAT"]);
		}
		set
		{
			if (!Table.Columns.Contains("PriceAfVAT"))
			{
				Table.Columns.Add("PriceAfVAT", typeof(double));
			}
			base["PriceAfVAT"] = value;
		}
	}

	public double GTotal
	{
		get
		{
			if (!Table.Columns.Contains("GTotal"))
			{
				Table.Columns.Add("GTotal", typeof(double));
			}
			return Function.ParseDouble(base["GTotal"]);
		}
		set
		{
			if (!Table.Columns.Contains("GTotal"))
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
			if (!Table.Columns.Contains("GTotalFC"))
			{
				Table.Columns.Add("GTotalFC", typeof(double));
			}
			return Function.ParseDouble(base["GTotalFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("GTotalFC"))
			{
				Table.Columns.Add("GTotalFC", typeof(double));
			}
			base["GTotalFC"] = value;
		}
	}

	public DateTime? ShipDate
	{
		get
		{
			if (!Table.Columns.Contains("ShipDate"))
			{
				Table.Columns.Add("ShipDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["ShipDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("ShipDate"))
			{
				Table.Columns.Add("ShipDate", typeof(DateTime));
			}
			base["ShipDate"] = value;
		}
	}

	public string WarehouseCode
	{
		get
		{
			if (!Table.Columns.Contains("WhsCode"))
			{
				Table.Columns.Add("WhsCode", typeof(string));
			}
			return Function.ToString(base["WhsCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("WhsCode"))
			{
				Table.Columns.Add("WhsCode", typeof(string));
			}
			base["WhsCode"] = value;
		}
	}

	public string VatGroup
	{
		get
		{
			if (!Table.Columns.Contains("VatGroup"))
			{
				Table.Columns.Add("VatGroup", typeof(string));
			}
			return Function.ToString(base["VatGroup"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatGroup"))
			{
				Table.Columns.Add("VatGroup", typeof(string));
			}
			base["VatGroup"] = value;
		}
	}

	public double Rate
	{
		get
		{
			if (!Table.Columns.Contains("VatPrcnt"))
			{
				Table.Columns.Add("VatPrcnt", typeof(double));
			}
			return Function.ParseDouble(base["VatPrcnt"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatPrcnt"))
			{
				Table.Columns.Add("VatPrcnt", typeof(double));
			}
			base["VatPrcnt"] = value;
		}
	}

	public double RateCurrency
	{
		get
		{
			if (!Table.Columns.Contains("Rate"))
			{
				Table.Columns.Add("Rate", typeof(double));
			}
			return Function.ParseDouble(base["Rate"]);
		}
		set
		{
			if (!Table.Columns.Contains("Rate"))
			{
				Table.Columns.Add("Rate", typeof(double));
			}
			base["Rate"] = value;
		}
	}

	public double DiscountPercent
	{
		get
		{
			if (!Table.Columns.Contains("DiscPrcnt"))
			{
				Table.Columns.Add("DiscPrcnt", typeof(double));
			}
			return Function.ParseDouble(base["DiscPrcnt"]);
		}
		set
		{
			if (!Table.Columns.Contains("DiscPrcnt"))
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
			if (!Table.Columns.Contains("LineTotal"))
			{
				Table.Columns.Add("LineTotal", typeof(double));
			}
			return Function.ParseDouble(base["LineTotal"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineTotal"))
			{
				Table.Columns.Add("LineTotal", typeof(double));
			}
			base["LineTotal"] = value;
		}
	}

	public double LineVat
	{
		get
		{
			if (!Table.Columns.Contains("LineVat"))
			{
				Table.Columns.Add("LineVat", typeof(double));
			}
			return Function.ParseDouble(base["LineVat"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineVat"))
			{
				Table.Columns.Add("LineVat", typeof(double));
			}
			base["LineVat"] = value;
		}
	}

	public double TotalFrgn
	{
		get
		{
			if (!Table.Columns.Contains("TotalFrgn"))
			{
				Table.Columns.Add("TotalFrgn", typeof(double));
			}
			return Function.ParseDouble(base["TotalFrgn"]);
		}
		set
		{
			if (!Table.Columns.Contains("TotalFrgn"))
			{
				Table.Columns.Add("TotalFrgn", typeof(double));
			}
			base["TotalFrgn"] = value;
		}
	}

	public double VatSumFrgn
	{
		get
		{
			if (!Table.Columns.Contains("VatSumFrgn"))
			{
				Table.Columns.Add("VatSumFrgn", typeof(double));
			}
			return Function.ParseDouble(base["VatSumFrgn"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatSumFrgn"))
			{
				Table.Columns.Add("VatSumFrgn", typeof(double));
			}
			base["VatSumFrgn"] = value;
		}
	}

	public double DelivrdQty
	{
		get
		{
			if (!Table.Columns.Contains("DelivrdQty"))
			{
				Table.Columns.Add("DelivrdQty", typeof(double));
			}
			return Function.ParseDouble(base["DelivrdQty"]);
		}
		set
		{
			if (!Table.Columns.Contains("DelivrdQty"))
			{
				Table.Columns.Add("DelivrdQty", typeof(double));
			}
			base["DelivrdQty"] = value;
		}
	}

	public double RemainingOpenQuantity
	{
		get
		{
			if (!Table.Columns.Contains("OpenQty"))
			{
				Table.Columns.Add("OpenQty", typeof(double));
			}
			return Function.ParseDouble(base["OpenQty"]);
		}
		set
		{
			if (!Table.Columns.Contains("OpenQty"))
			{
				Table.Columns.Add("OpenQty", typeof(double));
			}
			base["OpenQty"] = value;
		}
	}

	public double OpenInventoryQuantity
	{
		get
		{
			if (!Table.Columns.Contains("OpenInvQty"))
			{
				Table.Columns.Add("OpenInvQty", typeof(double));
			}
			return Function.ParseDouble(base["OpenInvQty"]);
		}
		set
		{
			if (!Table.Columns.Contains("OpenInvQty"))
			{
				Table.Columns.Add("OpenInvQty", typeof(double));
			}
			base["OpenInvQty"] = value;
		}
	}

	public double InventoryQuantity
	{
		get
		{
			if (!Table.Columns.Contains("InvQty"))
			{
				Table.Columns.Add("InvQty", typeof(double));
			}
			return Function.ParseDouble(base["InvQty"]);
		}
		set
		{
			if (!Table.Columns.Contains("InvQty"))
			{
				Table.Columns.Add("InvQty", typeof(double));
			}
			base["InvQty"] = value;
		}
	}

	public double PriceBefDi
	{
		get
		{
			if (!Table.Columns.Contains("PriceBefDi"))
			{
				Table.Columns.Add("PriceBefDi", typeof(double));
			}
			return Function.ParseDouble(base["PriceBefDi"]);
		}
		set
		{
			if (!Table.Columns.Contains("PriceBefDi"))
			{
				Table.Columns.Add("PriceBefDi", typeof(double));
			}
			base["PriceBefDi"] = value;
		}
	}

	public double TaxPercentagePerRow
	{
		get
		{
			if (!Table.Columns.Contains("TaxPercentagePerRow"))
			{
				Table.Columns.Add("TaxPercentagePerRow", typeof(double));
			}
			return Function.ParseDouble(base["TaxPercentagePerRow"]);
		}
		set
		{
			if (!Table.Columns.Contains("TaxPercentagePerRow"))
			{
				Table.Columns.Add("TaxPercentagePerRow", typeof(double));
			}
			base["TaxPercentagePerRow"] = value;
		}
	}

	public string LineStatusName
	{
		get
		{
			if (!Table.Columns.Contains("LineStatus"))
			{
				Table.Columns.Add("LineStatus", typeof(string));
			}
			return Function.ToString(base["LineStatus"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineStatus"))
			{
				Table.Columns.Add("LineStatus", typeof(string));
			}
			base["LineStatus"] = value;
		}
	}

	public string TaxCode
	{
		get
		{
			if (!Table.Columns.Contains("TaxCode"))
			{
				Table.Columns.Add("TaxCode", typeof(string));
			}
			return Function.ToString(base["TaxCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("TaxCode"))
			{
				Table.Columns.Add("TaxCode", typeof(string));
			}
			base["TaxCode"] = value;
		}
	}

	public double TaxTotal
	{
		get
		{
			if (!Table.Columns.Contains("TaxTotal"))
			{
				Table.Columns.Add("TaxTotal", typeof(double));
			}
			return Function.ParseDouble(base["TaxTotal"]);
		}
		set
		{
			if (!Table.Columns.Contains("TaxTotal"))
			{
				Table.Columns.Add("TaxTotal", typeof(double));
			}
			base["TaxTotal"] = value;
		}
	}

	public string Text
	{
		get
		{
			if (!Table.Columns.Contains("Text"))
			{
				Table.Columns.Add("Text", typeof(string));
			}
			return Function.ToString(base["Text"]);
		}
		set
		{
			if (!Table.Columns.Contains("Text"))
			{
				Table.Columns.Add("Text", typeof(string));
			}
			base["Text"] = value;
		}
	}

	public int BaseEntry
	{
		get
		{
			if (!Table.Columns.Contains("BaseEntry"))
			{
				Table.Columns.Add("BaseEntry", typeof(int));
			}
			return Function.ParseInt(base["BaseEntry"]);
		}
		set
		{
			if (!Table.Columns.Contains("BaseEntry"))
			{
				Table.Columns.Add("BaseEntry", typeof(int));
			}
			base["BaseEntry"] = value;
		}
	}

	public int BaseLine
	{
		get
		{
			if (!Table.Columns.Contains("BaseLine"))
			{
				Table.Columns.Add("BaseLine", typeof(int));
			}
			return Function.ParseInt(base["BaseLine"]);
		}
		set
		{
			if (!Table.Columns.Contains("BaseLine"))
			{
				Table.Columns.Add("BaseLine", typeof(int));
			}
			base["BaseLine"] = value;
		}
	}

	public DateTime? RequiredDate
	{
		get
		{
			if (!Table.Columns.Contains("PQTReqDate"))
			{
				Table.Columns.Add("PQTReqDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["PQTReqDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("PQTReqDate"))
			{
				Table.Columns.Add("PQTReqDate", typeof(DateTime));
			}
			base["PQTReqDate"] = value;
		}
	}

	public DateTime? PQTReqDate
	{
		get
		{
			if (!Table.Columns.Contains("PQTReqDate"))
			{
				Table.Columns.Add("PQTReqDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["PQTReqDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("PQTReqDate"))
			{
				Table.Columns.Add("PQTReqDate", typeof(DateTime));
			}
			base["PQTReqDate"] = value;
		}
	}

	public double RequiredQuantity
	{
		get
		{
			if (!Table.Columns.Contains("PQTReqQty"))
			{
				Table.Columns.Add("PQTReqQty", typeof(double));
			}
			return Function.ParseDouble(base["PQTReqQty"]);
		}
		set
		{
			if (!Table.Columns.Contains("PQTReqQty"))
			{
				Table.Columns.Add("PQTReqQty", typeof(double));
			}
			base["PQTReqQty"] = value;
		}
	}

	public string UoMCode
	{
		get
		{
			if (!Table.Columns.Contains("UoMCode"))
			{
				Table.Columns.Add("UoMCode", typeof(string));
			}
			return Function.ToString(base["UoMCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("UoMCode"))
			{
				Table.Columns.Add("UoMCode", typeof(string));
			}
			base["UoMCode"] = value;
		}
	}

	public string Remark
	{
		get
		{
			if (!Table.Columns.Contains("Remark"))
			{
				Table.Columns.Add("Remark", typeof(string));
			}
			return Function.ToString(base["Remark"]);
		}
		set
		{
			if (!Table.Columns.Contains("Remark"))
			{
				Table.Columns.Add("Remark", typeof(string));
			}
			base["Remark"] = value;
		}
	}

	public string FreeTxt
	{
		get
		{
			if (!Table.Columns.Contains("FreeTxt"))
			{
				Table.Columns.Add("FreeTxt", typeof(string));
			}
			return Function.ToString(base["FreeTxt"]);
		}
		set
		{
			if (!Table.Columns.Contains("FreeTxt"))
			{
				Table.Columns.Add("FreeTxt", typeof(string));
			}
			base["FreeTxt"] = value;
		}
	}

	public string FreeText
	{
		get
		{
			if (!Table.Columns.Contains("FreeText"))
			{
				Table.Columns.Add("FreeText", typeof(string));
			}
			return Function.ToString(base["FreeText"]);
		}
		set
		{
			if (!Table.Columns.Contains("FreeText"))
			{
				Table.Columns.Add("FreeText", typeof(string));
			}
			base["FreeText"] = value;
		}
	}

	public DateTime? PQTReqdate
	{
		get
		{
			if (!Table.Columns.Contains("PQTReqdate"))
			{
				Table.Columns.Add("PQTReqdate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["PQTReqdate"]);
		}
		set
		{
			if (!Table.Columns.Contains("PQTReqdate"))
			{
				Table.Columns.Add("PQTReqdate", typeof(DateTime));
			}
			base["PQTReqdate"] = value;
		}
	}

	public string UoMName
	{
		get
		{
			if (!Table.Columns.Contains("unitMsr"))
			{
				Table.Columns.Add("unitMsr", typeof(string));
			}
			return Function.ToString(base["unitMsr"]);
		}
		set
		{
			if (!Table.Columns.Contains("unitMsr"))
			{
				Table.Columns.Add("unitMsr", typeof(string));
			}
			base["unitMsr"] = value;
		}
	}

	public string AcctCode
	{
		get
		{
			if (!Table.Columns.Contains("AcctCode"))
			{
				Table.Columns.Add("AcctCode", typeof(string));
			}
			return Function.ToString(base["AcctCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("AcctCode"))
			{
				Table.Columns.Add("AcctCode", typeof(string));
			}
			base["AcctCode"] = value;
		}
	}

	public double QuantityOnStock
	{
		get
		{
			if (!Table.Columns.Contains("QuantityOnStock"))
			{
				Table.Columns.Add("QuantityOnStock", typeof(double));
			}
			return Function.ParseDouble(base["QuantityOnStock"]);
		}
		set
		{
			if (!Table.Columns.Contains("QuantityOnStock"))
			{
				Table.Columns.Add("QuantityOnStock", typeof(double));
			}
			base["QuantityOnStock"] = value;
		}
	}

	public string OcrCode
	{
		get
		{
			if (!Table.Columns.Contains("OcrCode"))
			{
				Table.Columns.Add("OcrCode", typeof(string));
			}
			return Function.ToString(base["OcrCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("OcrCode"))
			{
				Table.Columns.Add("OcrCode", typeof(string));
			}
			base["OcrCode"] = value;
		}
	}

	public string OcrCode2
	{
		get
		{
			if (!Table.Columns.Contains("OcrCode2"))
			{
				Table.Columns.Add("OcrCode2", typeof(string));
			}
			return Function.ToString(base["OcrCode2"]);
		}
		set
		{
			if (!Table.Columns.Contains("OcrCode2"))
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
			if (!Table.Columns.Contains("OcrCode3"))
			{
				Table.Columns.Add("OcrCode3", typeof(string));
			}
			return Function.ToString(base["OcrCode3"]);
		}
		set
		{
			if (!Table.Columns.Contains("OcrCode3"))
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
			if (!Table.Columns.Contains("OcrCode4"))
			{
				Table.Columns.Add("OcrCode4", typeof(string));
			}
			return Function.ToString(base["OcrCode4"]);
		}
		set
		{
			if (!Table.Columns.Contains("OcrCode4"))
			{
				Table.Columns.Add("OcrCode4", typeof(string));
			}
			base["OcrCode4"] = value;
		}
	}

	public string OcrCode5
	{
		get
		{
			if (!Table.Columns.Contains("OcrCode5"))
			{
				Table.Columns.Add("OcrCode5", typeof(string));
			}
			return Function.ToString(base["OcrCode5"]);
		}
		set
		{
			if (!Table.Columns.Contains("OcrCode5"))
			{
				Table.Columns.Add("OcrCode5", typeof(string));
			}
			base["OcrCode5"] = value;
		}
	}

	public int TrgetEntry
	{
		get
		{
			if (!Table.Columns.Contains("TrgetEntry"))
			{
				Table.Columns.Add("TrgetEntry", typeof(int));
			}
			return Function.ParseInt(base["TrgetEntry"]);
		}
		set
		{
			if (!Table.Columns.Contains("TrgetEntry"))
			{
				Table.Columns.Add("TrgetEntry", typeof(int));
			}
			base["TrgetEntry"] = value;
		}
	}

	public int TargetType
	{
		get
		{
			if (!Table.Columns.Contains("TargetType"))
			{
				Table.Columns.Add("TargetType", typeof(int));
			}
			return Function.ParseInt(base["TargetType"]);
		}
		set
		{
			if (!Table.Columns.Contains("TargetType"))
			{
				Table.Columns.Add("TargetType", typeof(int));
			}
			base["TargetType"] = value;
		}
	}

	public string BOMTreeType
	{
		get
		{
			if (!Table.Columns.Contains("BOMTreeType"))
			{
				Table.Columns.Add("BOMTreeType", typeof(string));
			}
			return Function.ToString(base["BOMTreeType"]);
		}
		set
		{
			if (!Table.Columns.Contains("BOMTreeType"))
			{
				Table.Columns.Add("BOMTreeType", typeof(string));
			}
			base["BOMTreeType"] = value;
		}
	}

	public string TreeType
	{
		get
		{
			if (!Table.Columns.Contains("TreeType"))
			{
				Table.Columns.Add("TreeType", typeof(string));
			}
			return Function.ToString(base["TreeType"]);
		}
		set
		{
			if (!Table.Columns.Contains("TreeType"))
			{
				Table.Columns.Add("TreeType", typeof(string));
			}
			base["TreeType"] = value;
		}
	}

	public string LineVendor
	{
		get
		{
			if (!Table.Columns.Contains("LineVendor"))
			{
				Table.Columns.Add("LineVendor", typeof(string));
			}
			return Function.ToString(base["LineVendor"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineVendor"))
			{
				Table.Columns.Add("LineVendor", typeof(string));
			}
			base["LineVendor"] = value;
		}
	}

	public string FrgnName
	{
		get
		{
			if (!Table.Columns.Contains("FrgnName"))
			{
				Table.Columns.Add("FrgnName", typeof(string));
			}
			return Function.ToString(base["FrgnName"]);
		}
		set
		{
			if (!Table.Columns.Contains("FrgnName"))
			{
				Table.Columns.Add("FrgnName", typeof(string));
			}
			base["FrgnName"] = value;
		}
	}

	public double QuantityOnWareHouse
	{
		get
		{
			if (!Table.Columns.Contains("QuantityOnWareHouse"))
			{
				Table.Columns.Add("QuantityOnWareHouse", typeof(double));
			}
			return Function.ParseDouble(base["QuantityOnWareHouse"]);
		}
		set
		{
			if (!Table.Columns.Contains("QuantityOnWareHouse"))
			{
				Table.Columns.Add("QuantityOnWareHouse", typeof(double));
			}
			base["QuantityOnWareHouse"] = value;
		}
	}

	public int ExpenseCode
	{
		get
		{
			return Function.ParseInt(base["ExpenseCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExpenseCode"))
			{
				Table.Columns.Add("ExpenseCode", typeof(int));
			}
			base["ExpenseCode"] = value;
		}
	}

	public string ExpnsName
	{
		get
		{
			return Function.ToString(base["ExpnsName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExpnsName"))
			{
				Table.Columns.Add("ExpnsName", typeof(string));
			}
			base["ExpnsName"] = value;
		}
	}

	public string Remarks
	{
		get
		{
			return Function.ToString(base["Remarks"]);
		}
		set
		{
			if (!Table.Columns.Contains("Remarks"))
			{
				Table.Columns.Add("Remarks", typeof(string));
			}
			base["Remarks"] = value;
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
			if (!Table.Columns.Contains("VatPrcnt"))
			{
				Table.Columns.Add("VatPrcnt", typeof(double));
			}
			base["VatPrcnt"] = value;
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
			if (!Table.Columns.Contains("VatSum"))
			{
				Table.Columns.Add("VatSum", typeof(double));
			}
			base["VatSum"] = value;
		}
	}

	public string DistrbMthd
	{
		get
		{
			return Function.ToString(base["DistrbMthd"]);
		}
		set
		{
			if (!Table.Columns.Contains("DistrbMthd"))
			{
				Table.Columns.Add("DistrbMthd", typeof(string));
			}
			base["DistrbMthd"] = value;
		}
	}

	public int TotalSumSy
	{
		get
		{
			return Function.ParseInt(base["TotalSumSy"]);
		}
		set
		{
			if (!Table.Columns.Contains("TotalSumSy"))
			{
				Table.Columns.Add("TotalSumSy", typeof(int));
			}
			base["TotalSumSy"] = value;
		}
	}

	public string Status
	{
		get
		{
			return Function.ToString(base["Status"]);
		}
		set
		{
			if (!Table.Columns.Contains("Status"))
			{
				Table.Columns.Add("Status", typeof(string));
			}
			base["Status"] = value;
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
			if (!Table.Columns.Contains("Project"))
			{
				Table.Columns.Add("Project", typeof(string));
			}
			base["Project"] = value;
		}
	}

	public bool SpecifiedPriceBefDi
	{
		get
		{
			if (!Table.Columns.Contains("SpecifiedPriceBefDi"))
			{
				Table.Columns.Add("SpecifiedPriceBefDi", typeof(bool));
			}
			return Function.ParseBool(base["SpecifiedPriceBefDi"]);
		}
		set
		{
			if (!Table.Columns.Contains("SpecifiedPriceBefDi"))
			{
				Table.Columns.Add("SpecifiedPriceBefDi", typeof(bool));
			}
			base["SpecifiedPriceBefDi"] = value;
		}
	}

	public bool SpecifiedPriceAfterVat
	{
		get
		{
			if (!Table.Columns.Contains("SpecifiedPriceAfterVat"))
			{
				Table.Columns.Add("SpecifiedPriceAfterVat", typeof(bool));
			}
			return Function.ParseBool(base["SpecifiedPriceAfterVat"]);
		}
		set
		{
			if (!Table.Columns.Contains("SpecifiedPriceAfterVat"))
			{
				Table.Columns.Add("SpecifiedPriceAfterVat", typeof(bool));
			}
			base["SpecifiedPriceAfterVat"] = value;
		}
	}

	public decimal RateBomSales
	{
		get
		{
			if (!Table.Columns.Contains("RateBomSales"))
			{
				Table.Columns.Add("RateBomSales", typeof(decimal));
			}
			return Function.ParseDecimal(base["RateBomSales"]);
		}
		set
		{
			if (!Table.Columns.Contains("RateBomSales"))
			{
				Table.Columns.Add("RateBomSales", typeof(decimal));
			}
			base["RateBomSales"] = value;
		}
	}

	public int ItemType
	{
		get
		{
			if (!Table.Columns.Contains("ItemType"))
			{
				Table.Columns.Add("ItemType", typeof(int));
			}
			return Function.ParseInt(base["ItemType"]);
		}
		set
		{
			if (!Table.Columns.Contains("ItemType"))
			{
				Table.Columns.Add("ItemType", typeof(int));
			}
			base["ItemType"] = value;
		}
	}

	internal DocumentLineRow(DataRowBuilder builder)
		: base(builder)
	{
	}
}
