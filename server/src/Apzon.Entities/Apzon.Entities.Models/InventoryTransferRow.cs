using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Entities.Models;

public class InventoryTransferRow : Row<InventoryTranferImpl>
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

	public int DocNum
	{
		get
		{
			return Function.ParseInt(base["DocNum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocNum".ToUpper())))
			{
				Table.Columns.Add("DocNum", typeof(int));
			}
			base["DocNum"] = value;
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
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ObjType".ToUpper())))
			{
				Table.Columns.Add("ObjType", typeof(int));
			}
			base["ObjType"] = value;
		}
	}

	public string DocType
	{
		get
		{
			return Function.ToString(base["DocType"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocType".ToUpper())))
			{
				Table.Columns.Add("DocType", typeof(string));
			}
			base["DocType"] = value;
		}
	}

	public string CANCELED
	{
		get
		{
			return Function.ToString(base["CANCELED"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CANCELED".ToUpper())))
			{
				Table.Columns.Add("CANCELED", typeof(string));
			}
			base["CANCELED"] = value;
		}
	}

	public string Handwrtten
	{
		get
		{
			return Function.ToString(base["Handwrtten"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Handwrtten".ToUpper())))
			{
				Table.Columns.Add("Handwrtten", typeof(string));
			}
			base["Handwrtten"] = value;
		}
	}

	public string Printed
	{
		get
		{
			return Function.ToString(base["Printed"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Printed".ToUpper())))
			{
				Table.Columns.Add("Printed", typeof(string));
			}
			base["Printed"] = value;
		}
	}

	public string DocStatus
	{
		get
		{
			return Function.ToString(base["DocStatus"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocStatus".ToUpper())))
			{
				Table.Columns.Add("DocStatus", typeof(string));
			}
			base["DocStatus"] = value;
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

	public DateTime DocDueDate
	{
		get
		{
			return Function.ParseDateTime(base["DocDueDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocDueDate".ToUpper())))
			{
				Table.Columns.Add("DocDueDate", typeof(DateTime));
			}
			base["DocDueDate"] = value;
		}
	}

	public string CardCode
	{
		get
		{
			return Function.ToString(base["CardCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CardCode".ToUpper())))
			{
				Table.Columns.Add("CardCode", typeof(string));
			}
			base["CardCode"] = value;
		}
	}

	public string CardName
	{
		get
		{
			return Function.ToString(base["CardName"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CardName".ToUpper())))
			{
				Table.Columns.Add("CardName", typeof(string));
			}
			base["CardName"] = value;
		}
	}

	public string Address
	{
		get
		{
			return Function.ToString(base["Address"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Address".ToUpper())))
			{
				Table.Columns.Add("Address", typeof(string));
			}
			base["Address"] = value;
		}
	}

	public string NumAtCard
	{
		get
		{
			return Function.ToString(base["NumAtCard"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("NumAtCard".ToUpper())))
			{
				Table.Columns.Add("NumAtCard", typeof(string));
			}
			base["NumAtCard"] = value;
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

	public double VatSumFC
	{
		get
		{
			return Function.ParseDouble(base["VatSumFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatSumFC".ToUpper())))
			{
				Table.Columns.Add("VatSumFC", typeof(double));
			}
			base["VatSumFC"] = value;
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

	public double DiscSum
	{
		get
		{
			return Function.ParseDouble(base["DiscSum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DiscSum".ToUpper())))
			{
				Table.Columns.Add("DiscSum", typeof(double));
			}
			base["DiscSum"] = value;
		}
	}

	public double DiscSumFC
	{
		get
		{
			return Function.ParseDouble(base["DiscSumFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DiscSumFC".ToUpper())))
			{
				Table.Columns.Add("DiscSumFC", typeof(double));
			}
			base["DiscSumFC"] = value;
		}
	}

	public string DocCur
	{
		get
		{
			return Function.ToString(base["DocCur"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocCur".ToUpper())))
			{
				Table.Columns.Add("DocCur", typeof(string));
			}
			base["DocCur"] = value;
		}
	}

	public double DocRate
	{
		get
		{
			return Function.ParseDouble(base["DocRate"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocRate".ToUpper())))
			{
				Table.Columns.Add("DocRate", typeof(double));
			}
			base["DocRate"] = value;
		}
	}

	public double DocTotal
	{
		get
		{
			return Function.ParseDouble(base["DocTotal"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocTotal".ToUpper())))
			{
				Table.Columns.Add("DocTotal", typeof(double));
			}
			base["DocTotal"] = value;
		}
	}

	public double DocTotalFC
	{
		get
		{
			return Function.ParseDouble(base["DocTotalFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocTotalFC".ToUpper())))
			{
				Table.Columns.Add("DocTotalFC", typeof(double));
			}
			base["DocTotalFC"] = value;
		}
	}

	public double PaidToDate
	{
		get
		{
			return Function.ParseDouble(base["PaidToDate"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidToDate".ToUpper())))
			{
				Table.Columns.Add("PaidToDate", typeof(double));
			}
			base["PaidToDate"] = value;
		}
	}

	public double PaidFC
	{
		get
		{
			return Function.ParseDouble(base["PaidFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidFC".ToUpper())))
			{
				Table.Columns.Add("PaidFC", typeof(double));
			}
			base["PaidFC"] = value;
		}
	}

	public double GrosProfit
	{
		get
		{
			return Function.ParseDouble(base["GrosProfit"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GrosProfit".ToUpper())))
			{
				Table.Columns.Add("GrosProfit", typeof(double));
			}
			base["GrosProfit"] = value;
		}
	}

	public double GrosProfFC
	{
		get
		{
			return Function.ParseDouble(base["GrosProfFC"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GrosProfFC".ToUpper())))
			{
				Table.Columns.Add("GrosProfFC", typeof(double));
			}
			base["GrosProfFC"] = value;
		}
	}

	public string Ref1
	{
		get
		{
			return Function.ToString(base["Ref1"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Ref1".ToUpper())))
			{
				Table.Columns.Add("Ref1", typeof(string));
			}
			base["Ref1"] = value;
		}
	}

	public string Ref2
	{
		get
		{
			return Function.ToString(base["Ref2"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Ref2".ToUpper())))
			{
				Table.Columns.Add("Ref2", typeof(string));
			}
			base["Ref2"] = value;
		}
	}

	public string Comments
	{
		get
		{
			return Function.ToString(base["Comments"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Comments".ToUpper())))
			{
				Table.Columns.Add("Comments", typeof(string));
			}
			base["Comments"] = value;
		}
	}

	public string JrnlMemo
	{
		get
		{
			return Function.ToString(base["JrnlMemo"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("JrnlMemo".ToUpper())))
			{
				Table.Columns.Add("JrnlMemo", typeof(string));
			}
			base["JrnlMemo"] = value;
		}
	}

	public int TransId
	{
		get
		{
			return Function.ParseInt(base["TransId"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TransId".ToUpper())))
			{
				Table.Columns.Add("TransId", typeof(int));
			}
			base["TransId"] = value;
		}
	}

	public int ReceiptNum
	{
		get
		{
			return Function.ParseInt(base["ReceiptNum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ReceiptNum".ToUpper())))
			{
				Table.Columns.Add("ReceiptNum", typeof(int));
			}
			base["ReceiptNum"] = value;
		}
	}

	public int GroupNum
	{
		get
		{
			return Function.ParseInt(base["GroupNum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("GroupNum".ToUpper())))
			{
				Table.Columns.Add("GroupNum", typeof(int));
			}
			base["GroupNum"] = value;
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

	public int TrnspCode
	{
		get
		{
			return Function.ParseInt(base["TrnspCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TrnspCode".ToUpper())))
			{
				Table.Columns.Add("TrnspCode", typeof(int));
			}
			base["TrnspCode"] = value;
		}
	}

	public int CntctCode
	{
		get
		{
			return Function.ParseInt(base["CntctCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CntctCode".ToUpper())))
			{
				Table.Columns.Add("CntctCode", typeof(int));
			}
			base["CntctCode"] = value;
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

	public double DiscSumSy
	{
		get
		{
			return Function.ParseDouble(base["DiscSumSy"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DiscSumSy".ToUpper())))
			{
				Table.Columns.Add("DiscSumSy", typeof(double));
			}
			base["DiscSumSy"] = value;
		}
	}

	public double DocTotalSy
	{
		get
		{
			return Function.ParseDouble(base["DocTotalSy"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("DocTotalSy".ToUpper())))
			{
				Table.Columns.Add("DocTotalSy", typeof(double));
			}
			base["DocTotalSy"] = value;
		}
	}

	public double PaidSys
	{
		get
		{
			return Function.ParseDouble(base["PaidSys"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidSys".ToUpper())))
			{
				Table.Columns.Add("PaidSys", typeof(double));
			}
			base["PaidSys"] = value;
		}
	}

	public DateTime UpdateDate
	{
		get
		{
			return Function.ParseDateTime(base["UpdateDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UpdateDate".ToUpper())))
			{
				Table.Columns.Add("UpdateDate", typeof(DateTime));
			}
			base["UpdateDate"] = value;
		}
	}

	public string IsICT
	{
		get
		{
			return Function.ToString(base["IsICT"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("IsICT".ToUpper())))
			{
				Table.Columns.Add("IsICT", typeof(string));
			}
			base["IsICT"] = value;
		}
	}

	public DateTime CreateDate
	{
		get
		{
			return Function.ParseDateTime(base["CreateDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CreateDate".ToUpper())))
			{
				Table.Columns.Add("CreateDate", typeof(DateTime));
			}
			base["CreateDate"] = value;
		}
	}

	public int Series
	{
		get
		{
			return Function.ParseInt(base["Series"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Series".ToUpper())))
			{
				Table.Columns.Add("Series", typeof(int));
			}
			base["Series"] = value;
		}
	}

	public DateTime TaxDate
	{
		get
		{
			return Function.ParseDateTime(base["TaxDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TaxDate".ToUpper())))
			{
				Table.Columns.Add("TaxDate", typeof(DateTime));
			}
			base["TaxDate"] = value;
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

	public int UserSign
	{
		get
		{
			return Function.ParseInt(base["UserSign"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UserSign".ToUpper())))
			{
				Table.Columns.Add("UserSign", typeof(int));
			}
			base["UserSign"] = value;
		}
	}

	public int UserSign2
	{
		get
		{
			return Function.ParseInt(base["UserSign2"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("UserSign2".ToUpper())))
			{
				Table.Columns.Add("UserSign2", typeof(int));
			}
			base["UserSign2"] = value;
		}
	}

	public DateTime VatDate
	{
		get
		{
			return Function.ParseDateTime(base["VatDate"]) ?? new DateTime(1900, 1, 1);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("VatDate".ToUpper())))
			{
				Table.Columns.Add("VatDate", typeof(DateTime));
			}
			base["VatDate"] = value;
		}
	}

	public double PaidSum
	{
		get
		{
			return Function.ParseDouble(base["PaidSum"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidSum".ToUpper())))
			{
				Table.Columns.Add("PaidSum", typeof(double));
			}
			base["PaidSum"] = value;
		}
	}

	public double PaidSumFc
	{
		get
		{
			return Function.ParseDouble(base["PaidSumFc"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidSumFc".ToUpper())))
			{
				Table.Columns.Add("PaidSumFc", typeof(double));
			}
			base["PaidSumFc"] = value;
		}
	}

	public double PaidSumSc
	{
		get
		{
			return Function.ParseDouble(base["PaidSumSc"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PaidSumSc".ToUpper())))
			{
				Table.Columns.Add("PaidSumSc", typeof(double));
			}
			base["PaidSumSc"] = value;
		}
	}

	public string Filler
	{
		get
		{
			return Function.ToString(base["Filler"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Filler".ToUpper())))
			{
				Table.Columns.Add("Filler", typeof(string));
			}
			base["Filler"] = value;
		}
	}

	public string CurSource
	{
		get
		{
			return Function.ToString(base["CurSource"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("CurSource".ToUpper())))
			{
				Table.Columns.Add("CurSource", typeof(string));
			}
			base["CurSource"] = value;
		}
	}

	public string isIns
	{
		get
		{
			return Function.ToString(base["isIns"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("isIns".ToUpper())))
			{
				Table.Columns.Add("isIns", typeof(string));
			}
			base["isIns"] = value;
		}
	}

	public string Cancelled
	{
		get
		{
			return Function.ToString(base["Cancelled"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Cancelled".ToUpper())))
			{
				Table.Columns.Add("Cancelled", typeof(string));
			}
			base["Cancelled"] = value;
		}
	}

	public List<InventoryTranfer_LineImpl> Lines
	{
		get
		{
			return (List<InventoryTranfer_LineImpl>)base["Lines"];
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("Lines".ToUpper())))
			{
				Table.Columns.Add("Lines", typeof(List<InventoryTranfer_LineImpl>));
			}
			base["Lines"] = value;
		}
	}

	public int ObjTypes
	{
		get
		{
			return Function.ParseInt(base["ObjTypes"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ObjTypes".ToUpper())))
			{
				Table.Columns.Add("ObjTypes", typeof(int));
			}
			base["ObjTypes"] = value;
		}
	}

	public ApzObjectTypes TypeTo
	{
		get
		{
			return (ApzObjectTypes)base["TypeTo"];
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TypeTo".ToUpper())))
			{
				Table.Columns.Add("TypeTo", typeof(ApzObjectTypes));
			}
			base["TypeTo"] = value;
		}
	}

	public ApzObjectTypes TypeFrom
	{
		get
		{
			return (ApzObjectTypes)base["TypeFrom"];
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("TypeFrom".ToUpper())))
			{
				Table.Columns.Add("TypeFrom", typeof(ApzObjectTypes));
			}
			base["TypeFrom"] = value;
		}
	}

	public int ObjTypeFrom
	{
		get
		{
			return Function.ParseInt(base["ObjTypeFrom"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ObjTypeFrom".ToUpper())))
			{
				Table.Columns.Add("ObjTypeFrom", typeof(int));
			}
			base["ObjTypeFrom"] = value;
		}
	}

	public string ToWhsCode
	{
		get
		{
			return Function.ToString(base["ToWhsCode"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("ToWhsCode".ToUpper())))
			{
				Table.Columns.Add("ToWhsCode", typeof(string));
			}
			base["ToWhsCode"] = value;
		}
	}

	public string PriceList
	{
		get
		{
			return Function.ToString(base["PriceList"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("PriceList".ToUpper())))
			{
				Table.Columns.Add("PriceList", typeof(string));
			}
			base["PriceList"] = value;
		}
	}

	public string SeriesName
	{
		get
		{
			return Function.ToString(base["SeriesName"]);
		}
		set
		{
			if (!Table.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.ToUpper().Equals("SeriesName".ToUpper())))
			{
				Table.Columns.Add("SeriesName", typeof(string));
			}
			base["SeriesName"] = value;
		}
	}

	internal InventoryTransferRow(DataRowBuilder builder)
		: base(builder)
	{
	}
}
