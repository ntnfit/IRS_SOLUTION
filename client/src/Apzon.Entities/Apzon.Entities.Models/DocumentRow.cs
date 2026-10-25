using System;
using System.Data;
using Apzon.Commons;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public class DocumentRow : DataRow
{
	public string ATDocumentType
	{
		get
		{
			return Function.ToString(base["ATDocumentType"]);
		}
		set
		{
			base["ATDocumentType"] = value;
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
			base["CardCode"] = value;
		}
	}

	public int SalesPersonCode
	{
		get
		{
			if (!Table.Columns.Contains("SlpCode"))
			{
				Table.Columns.Add("SlpCode", typeof(int));
			}
			return Function.ParseInt(base["SlpCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("SlpCode"))
			{
				Table.Columns.Add("SlpCode", typeof(int));
			}
			base["SlpCode"] = value;
		}
	}

	public int TransportationCode
	{
		get
		{
			if (!Table.Columns.Contains("TrnspCode"))
			{
				Table.Columns.Add("TrnspCode", typeof(int));
			}
			return Function.ParseInt(base["TrnspCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("TrnspCode"))
			{
				Table.Columns.Add("TrnspCode", typeof(int));
			}
			base["TrnspCode"] = value;
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

	public int DocNum
	{
		get
		{
			if (!Table.Columns.Contains("DocNum"))
			{
				Table.Columns.Add("DocNum", typeof(int));
			}
			return Function.ParseInt(base["DocNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocNum"))
			{
				Table.Columns.Add("DocNum", typeof(int));
			}
			base["DocNum"] = value;
		}
	}

	public string LineNum
	{
		get
		{
			return Function.ToString(base["LineNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("LineNum"))
			{
				Table.Columns.Add("LineNum", typeof(string));
			}
			base["LineNum"] = value;
		}
	}

	public string ExpenseCode
	{
		get
		{
			return Function.ToString(base["ExpenseCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExpenseCode"))
			{
				Table.Columns.Add("ExpenseCode", typeof(string));
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

	public string VatGroup
	{
		get
		{
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

	public string VatPrcnt
	{
		get
		{
			return Function.ToString(base["VatPrcnt"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatPrcnt"))
			{
				Table.Columns.Add("VatPrcnt", typeof(string));
			}
			base["VatPrcnt"] = value;
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

	public string TotalSumSy
	{
		get
		{
			return Function.ToString(base["DocTotalSy"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocTotalSy"))
			{
				Table.Columns.Add("DocTotalSy", typeof(string));
			}
			base["DocTotalSy"] = value;
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

	public double TotalExpns
	{
		get
		{
			return Function.ParseDouble(base["TotalExpns"]);
		}
		set
		{
			if (!Table.Columns.Contains("TotalExpns"))
			{
				Table.Columns.Add("TotalExpns", typeof(double));
			}
			base["TotalExpns"] = value;
		}
	}

	public double TotalExpFC
	{
		get
		{
			return Function.ParseDouble(base["TotalExpFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("TotalExpFC"))
			{
				Table.Columns.Add("TotalExpFC", typeof(double));
			}
			base["TotalExpFC"] = value;
		}
	}

	public double TotalFreight
	{
		get
		{
			return Function.ParseDouble(base["TotalFreight"]);
		}
		set
		{
			if (!Table.Columns.Contains("TotalFreight"))
			{
				Table.Columns.Add("TotalFreight", typeof(double));
			}
			base["TotalFreight"] = value;
		}
	}

	public string DocumentStatus
	{
		get
		{
			return Function.ToString(base["DocStatus"]);
		}
		set
		{
			base["DocStatus"] = value;
		}
	}

	public string DocCurrency
	{
		get
		{
			return Function.ToString(base["DocCur"]);
		}
		set
		{
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
			base["DocRate"] = value;
		}
	}

	public string DocStatusName
	{
		get
		{
			if (!Table.Columns.Contains("DocStatusName"))
			{
				Table.Columns.Add("DocStatusName", typeof(string));
			}
			return Function.ToString(base["DocStatusName"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocStatusName"))
			{
				Table.Columns.Add("DocStatusName", typeof(string));
			}
			base["DocStatusName"] = value;
		}
	}

	public string NumAtCard
	{
		get
		{
			if (!Table.Columns.Contains("NumAtCard"))
			{
				Table.Columns.Add("NumAtCard", typeof(string));
			}
			return Function.ToString(base["NumAtCard"]);
		}
		set
		{
			if (!Table.Columns.Contains("NumAtCard"))
			{
				Table.Columns.Add("NumAtCard", typeof(string));
			}
			base["NumAtCard"] = value;
		}
	}

	public BoYesNoEnum Cancelled
	{
		get
		{
			if (!Table.Columns.Contains("Cancelled"))
			{
				Table.Columns.Add("Cancelled", typeof(BoYesNoEnum));
			}
			return (BoYesNoEnum)base["Cancelled"];
		}
		set
		{
			if (!Table.Columns.Contains("Cancelled"))
			{
				Table.Columns.Add("Cancelled", typeof(BoYesNoEnum));
			}
			base["Cancelled"] = value;
		}
	}

	public DateTime? DocDate
	{
		get
		{
			if (!Table.Columns.Contains("DocDate"))
			{
				Table.Columns.Add("DocDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["DocDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocDate"))
			{
				Table.Columns.Add("DocDate", typeof(DateTime));
			}
			base["DocDate"] = value;
		}
	}

	public string PayDuMonth
	{
		get
		{
			if (!Table.Columns.Contains("PayDuMonth"))
			{
				Table.Columns.Add("PayDuMonth", typeof(string));
			}
			return Function.ToString(base["PayDuMonth"]);
		}
		set
		{
			if (!Table.Columns.Contains("PayDuMonth"))
			{
				Table.Columns.Add("PayDuMonth", typeof(string));
			}
			base["PayDuMonth"] = value;
		}
	}

	public int ExtraMonth
	{
		get
		{
			if (!Table.Columns.Contains("ExtraMonth"))
			{
				Table.Columns.Add("ExtraMonth", typeof(int));
			}
			return Function.ParseInt(base["ExtraMonth"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExtraMonth"))
			{
				Table.Columns.Add("ExtraMonth", typeof(int));
			}
			base["ExtraMonth"] = value;
		}
	}

	public string BslineDate
	{
		get
		{
			if (!Table.Columns.Contains("BslineDate"))
			{
				Table.Columns.Add("BslineDate", typeof(string));
			}
			return Function.ToString(base["BslineDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("BslineDate"))
			{
				Table.Columns.Add("BslineDate", typeof(string));
			}
			base["BslineDate"] = value;
		}
	}

	public int ExtraDays
	{
		get
		{
			if (!Table.Columns.Contains("ExtraDays"))
			{
				Table.Columns.Add("ExtraDays", typeof(int));
			}
			return Function.ParseInt(base["ExtraDays"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExtraDays"))
			{
				Table.Columns.Add("ExtraDays", typeof(int));
			}
			base["ExtraDays"] = value;
		}
	}

	public DateTime? DocDueDate
	{
		get
		{
			return Function.ParseDateTime(base["DocDueDate"]);
		}
		set
		{
			base["DocDueDate"] = value;
		}
	}

	public DateTime? TaxDate
	{
		get
		{
			return Function.ParseDateTime(base["TaxDate"]);
		}
		set
		{
			base["TaxDate"] = value;
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
			base["CardName"] = value;
		}
	}

	public int ContactPersonCode
	{
		get
		{
			if (!Table.Columns.Contains("CntctCode"))
			{
				Table.Columns.Add("CntctCode", typeof(int));
			}
			return Function.ParseInt(base["CntctCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("CntctCode"))
			{
				Table.Columns.Add("CntctCode", typeof(int));
			}
			base["CntctCode"] = value;
		}
	}

	public string ContactPersonName
	{
		get
		{
			if (!Table.Columns.Contains("ContactPersonName"))
			{
				Table.Columns.Add("ContactPersonCode", typeof(int));
			}
			return Function.ToString(base["ContactPersonName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ContactPersonName"))
			{
				Table.Columns.Add("ContactPersonName", typeof(string));
			}
			base["ContactPersonName"] = value;
		}
	}

	public string ShipToCode
	{
		get
		{
			if (!Table.Columns.Contains("ShipToCode"))
			{
				Table.Columns.Add("ShipToCode", typeof(string));
			}
			return Function.ToString(base["ShipToCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("ShipToCode"))
			{
				Table.Columns.Add("ShipToCode", typeof(string));
			}
			base["ShipToCode"] = value;
		}
	}

	public string PayToCode
	{
		get
		{
			if (!Table.Columns.Contains("PayToCode"))
			{
				Table.Columns.Add("PayToCode", typeof(string));
			}
			return Function.ToString(base["PayToCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("PayToCode"))
			{
				Table.Columns.Add("ShipToCode", typeof(string));
			}
			base["PayToCode"] = value;
		}
	}

	public string Address
	{
		get
		{
			if (!Table.Columns.Contains("Address"))
			{
				Table.Columns.Add("Address", typeof(string));
			}
			return Function.ToString(base["Address"]);
		}
		set
		{
			if (!Table.Columns.Contains("Address"))
			{
				Table.Columns.Add("Address", typeof(string));
			}
			base["Address"] = value;
		}
	}

	public string Address2
	{
		get
		{
			if (!Table.Columns.Contains("Address2"))
			{
				Table.Columns.Add("Address2", typeof(string));
			}
			return Function.ToString(base["Address2"]);
		}
		set
		{
			if (!Table.Columns.Contains("Address2"))
			{
				Table.Columns.Add("Address2", typeof(string));
			}
			base["Address2"] = value;
		}
	}

	public string Rounding
	{
		get
		{
			if (!Table.Columns.Contains("Rounding"))
			{
				Table.Columns.Add("Rounding", typeof(string));
			}
			return Function.ToString(base["Rounding"]);
		}
		set
		{
			if (!Table.Columns.Contains("Address2"))
			{
				Table.Columns.Add("Rounding", typeof(string));
			}
			base["Rounding"] = value;
		}
	}

	public string Confirmed
	{
		get
		{
			if (!Table.Columns.Contains("Confirmed"))
			{
				Table.Columns.Add("Confirmed", typeof(string));
			}
			return Function.ToString(base["Confirmed"]);
		}
		set
		{
			if (!Table.Columns.Contains("Confirmed"))
			{
				Table.Columns.Add("Confirmed", typeof(bool));
			}
			base["Confirmed"] = value;
		}
	}

	public string PartSupply
	{
		get
		{
			if (!Table.Columns.Contains("PartSupply"))
			{
				Table.Columns.Add("PartSupply", typeof(string));
			}
			return Function.ToString(base["PartSupply"]);
		}
		set
		{
			if (!Table.Columns.Contains("PartSupply"))
			{
				Table.Columns.Add("PartSupply", typeof(bool));
			}
			base["PartSupply"] = value;
		}
	}

	public double TotalBeforeDis
	{
		get
		{
			return Function.ParseDouble(base["TotalBeforeDis"]);
		}
		set
		{
			base["TotalBeforeDis"] = value;
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
			base["VatSum"] = value;
		}
	}

	public double VatSumFc
	{
		get
		{
			if (!Table.Columns.Contains("VatSumFc"))
			{
				Table.Columns.Add("VatSumFc", typeof(double));
			}
			return Function.ParseDouble(base["VatSumFc"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatSumFc"))
			{
				Table.Columns.Add("VatSumFc", typeof(double));
			}
			base["VatSumFc"] = value;
		}
	}

	public double VatSumSys
	{
		get
		{
			if (!Table.Columns.Contains("VatSumSy"))
			{
				Table.Columns.Add("VatSumSy", typeof(double));
			}
			return Function.ParseDouble(base["VatSumSy"]);
		}
		set
		{
			if (!Table.Columns.Contains("VatSumSy"))
			{
				Table.Columns.Add("VatSumSy", typeof(double));
			}
			base["VatSumSy"] = value;
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
			base["DocTotal"] = value;
		}
	}

	public int ObjType
	{
		get
		{
			if (!Table.Columns.Contains("ObjType"))
			{
				Table.Columns.Add("ObjType", typeof(int));
			}
			return Function.ParseInt(base["ObjType"]);
		}
		set
		{
			if (!Table.Columns.Contains("OBJTYPE"))
			{
				Table.Columns.Add("ObjType", typeof(int));
			}
			base["ObjType"] = value;
		}
	}

	public int ObjTypeFrom
	{
		get
		{
			if (!Table.Columns.Contains("ObjTypeFrom"))
			{
				Table.Columns.Add("ObjTypeFrom", typeof(int));
			}
			return Function.ParseInt(base["ObjTypeFrom"]);
		}
		set
		{
			if (!Table.Columns.Contains("ObjTypeFrom"))
			{
				Table.Columns.Add("ObjTypeFrom", typeof(int));
			}
			base["ObjTypeFrom"] = value;
		}
	}

	public int TypeFrom
	{
		get
		{
			if (!Table.Columns.Contains("TypeFrom"))
			{
				Table.Columns.Add("TypeFrom", typeof(int));
			}
			return Function.ParseInt(base["TypeFrom"]);
		}
		set
		{
			if (!Table.Columns.Contains("TypeFrom"))
			{
				Table.Columns.Add("TypeFrom", typeof(int));
			}
			base["TypeFrom"] = value;
		}
	}

	public int TypeTo
	{
		get
		{
			if (!Table.Columns.Contains("TypeTo"))
			{
				Table.Columns.Add("TypeTo", typeof(int));
			}
			return Function.ParseInt(base["TypeTo"]);
		}
		set
		{
			if (!Table.Columns.Contains("TypeTo"))
			{
				Table.Columns.Add("TypeTo", typeof(int));
			}
			base["TypeTo"] = value;
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

	public string Comments
	{
		get
		{
			return Function.ToString(base["Comments"]);
		}
		set
		{
			if (!Table.Columns.Contains("Comments"))
			{
				Table.Columns.Add("Comments", typeof(string));
			}
			base["Comments"] = value;
		}
	}

	public string Canceled
	{
		get
		{
			if (!Table.Columns.Contains("Canceled"))
			{
				Table.Columns.Add("Canceled", typeof(string));
			}
			return Function.ToString(base["Canceled"]);
		}
		set
		{
			if (!Table.Columns.Contains("Canceled"))
			{
				Table.Columns.Add("Canceled", typeof(string));
			}
			base["Canceled"] = value;
		}
	}

	public DateTime? ClosingDate
	{
		get
		{
			if (!Table.Columns.Contains("ClsDate"))
			{
				Table.Columns.Add("ClsDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["ClsDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("ClsDate"))
			{
				Table.Columns.Add("ClsDate", typeof(DateTime));
			}
			base["ClsDate"] = value;
		}
	}

	public DateTime? CreationDate
	{
		get
		{
			if (!Table.Columns.Contains("CreateDate"))
			{
				Table.Columns.Add("CreateDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["CreateDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("CreateDate"))
			{
				Table.Columns.Add("CreateDate", typeof(DateTime));
			}
			base["CreateDate"] = value;
		}
	}

	public double DocTotalFc
	{
		get
		{
			if (!Table.Columns.Contains("DocTotalFC"))
			{
				Table.Columns.Add("DocTotalFC", typeof(double));
			}
			return Function.ParseDouble(base["DocTotalFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocTotalFC"))
			{
				Table.Columns.Add("DocTotalFC", typeof(double));
			}
			base["DocTotalFC"] = value;
		}
	}

	public double DocTotalSys
	{
		get
		{
			if (!Table.Columns.Contains("DocTotalSy"))
			{
				Table.Columns.Add("DocTotalSy", typeof(double));
			}
			return Function.ParseDouble(base["DocTotalSy"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocTotalSy"))
			{
				Table.Columns.Add("DocTotalSy", typeof(double));
			}
			base["DocTotalSy"] = value;
		}
	}

	public string U_DocType
	{
		get
		{
			if (!Table.Columns.Contains("U_DocType"))
			{
				Table.Columns.Add("U_DocType", typeof(string));
			}
			return Function.ToString(base["U_DocType"]);
		}
		set
		{
			if (!Table.Columns.Contains("U_DocType"))
			{
				Table.Columns.Add("U_DocType", typeof(string));
			}
			base["U_DocType"] = value;
		}
	}

	public int ReqType
	{
		get
		{
			if (!Table.Columns.Contains("ReqType"))
			{
				Table.Columns.Add("ReqType", typeof(int));
			}
			return Function.ParseInt(base["ReqType"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqType"))
			{
				Table.Columns.Add("ReqType", typeof(int));
			}
			base["ReqType"] = value;
		}
	}

	public string ReqTypeName
	{
		get
		{
			if (!Table.Columns.Contains("ReqTypeName"))
			{
				Table.Columns.Add("ReqTypeName", typeof(string));
			}
			return Function.ToString(base["ReqTypeName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqTypeName"))
			{
				Table.Columns.Add("ReqTypeName", typeof(string));
			}
			base["ReqTypeName"] = value;
		}
	}

	public string Requester
	{
		get
		{
			if (!Table.Columns.Contains("Requester"))
			{
				Table.Columns.Add("Requester", typeof(string));
			}
			return Function.ToString(base["Requester"]);
		}
		set
		{
			if (!Table.Columns.Contains("Requester"))
			{
				Table.Columns.Add("Requester", typeof(string));
			}
			base["Requester"] = value;
		}
	}

	public int RequesterBranch
	{
		get
		{
			if (!Table.Columns.Contains("Branch"))
			{
				Table.Columns.Add("Branch", typeof(int));
			}
			return Function.ParseInt(base["Branch"]);
		}
		set
		{
			if (!Table.Columns.Contains("Branch"))
			{
				Table.Columns.Add("Branch", typeof(int));
			}
			base["Branch"] = value;
		}
	}

	public string RequesterBranchName
	{
		get
		{
			if (!Table.Columns.Contains("RequesterBranchName"))
			{
				Table.Columns.Add("RequesterBranchName", typeof(string));
			}
			return Function.ToString(base["RequesterBranchName"]);
		}
		set
		{
			if (!Table.Columns.Contains("RequesterBranchName"))
			{
				Table.Columns.Add("RequesterBranchName", typeof(string));
			}
			base["RequesterBranchName"] = value;
		}
	}

	public int Branch
	{
		get
		{
			if (!Table.Columns.Contains("Branch"))
			{
				Table.Columns.Add("Branch", typeof(int));
			}
			return Function.ParseInt(base["Branch"]);
		}
		set
		{
			if (!Table.Columns.Contains("Branch"))
			{
				Table.Columns.Add("Branch", typeof(int));
			}
			base["Branch"] = value;
		}
	}

	public int RequesterDepartment
	{
		get
		{
			if (!Table.Columns.Contains("Department"))
			{
				Table.Columns.Add("Department", typeof(int));
			}
			return Function.ParseInt(base["Department"]);
		}
		set
		{
			if (!Table.Columns.Contains("Department"))
			{
				Table.Columns.Add("Department", typeof(int));
			}
			base["Department"] = value;
		}
	}

	public string RequesterDepartmentName
	{
		get
		{
			if (!Table.Columns.Contains("RequesterDepartmentName"))
			{
				Table.Columns.Add("RequesterDepartmentName", typeof(string));
			}
			return Function.ToString(base["RequesterDepartmentName"]);
		}
		set
		{
			if (!Table.Columns.Contains("RequesterDepartmentName"))
			{
				Table.Columns.Add("RequesterDepartmentName", typeof(string));
			}
			base["RequesterDepartmentName"] = value;
		}
	}

	public int Department
	{
		get
		{
			if (!Table.Columns.Contains("Department"))
			{
				Table.Columns.Add("Department", typeof(int));
			}
			return Function.ParseInt(base["Department"]);
		}
		set
		{
			if (!Table.Columns.Contains("Department"))
			{
				Table.Columns.Add("Department", typeof(int));
			}
			base["Department"] = value;
		}
	}

	public string RequesterEmail
	{
		get
		{
			if (!Table.Columns.Contains("Email"))
			{
				Table.Columns.Add("Email", typeof(string));
			}
			return Function.ToString(base["Email"]);
		}
		set
		{
			if (!Table.Columns.Contains("Email"))
			{
				Table.Columns.Add("Email", typeof(string));
			}
			base["Email"] = value;
		}
	}

	public string RequesterName
	{
		get
		{
			if (!Table.Columns.Contains("ReqName"))
			{
				Table.Columns.Add("ReqName", typeof(string));
			}
			return Function.ToString(base["ReqName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqName"))
			{
				Table.Columns.Add("ReqName", typeof(string));
			}
			base["ReqName"] = value;
		}
	}

	public string ReqName
	{
		get
		{
			if (!Table.Columns.Contains("ReqName"))
			{
				Table.Columns.Add("ReqName", typeof(string));
			}
			return Function.ToString(base["ReqName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqName"))
			{
				Table.Columns.Add("ReqName", typeof(string));
			}
			base["ReqName"] = value;
		}
	}

	public DateTime? RequriedDate
	{
		get
		{
			if (!Table.Columns.Contains("ReqDate"))
			{
				Table.Columns.Add("ReqDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["ReqDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqDate"))
			{
				Table.Columns.Add("ReqDate", typeof(DateTime));
			}
			base["ReqDate"] = value;
		}
	}

	public DateTime? UpdateDate
	{
		get
		{
			if (!Table.Columns.Contains("UpdateDate"))
			{
				Table.Columns.Add("UpdateDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["UpdateDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("UpdateDate"))
			{
				Table.Columns.Add("UpdateDate", typeof(DateTime));
			}
			base["UpdateDate"] = value;
		}
	}

	public string ShipTypeName
	{
		get
		{
			if (!Table.Columns.Contains("ShipTypeName"))
			{
				Table.Columns.Add("ShipTypeName", typeof(string));
			}
			return Function.ToString(base["ShipTypeName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ShipTypeName"))
			{
				Table.Columns.Add("ShipTypeName", typeof(string));
			}
			base["ShipTypeName"] = value;
		}
	}

	public string TrnspCode
	{
		get
		{
			if (!Table.Columns.Contains("TrnspCode"))
			{
				Table.Columns.Add("TrnspCode", typeof(string));
			}
			return Function.ToString(base["TrnspCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("TrnspCode"))
			{
				Table.Columns.Add("TrnspCode", typeof(string));
			}
			base["TrnspCode"] = value;
		}
	}

	public int GroupNum
	{
		get
		{
			if (!Table.Columns.Contains("GroupNum"))
			{
				Table.Columns.Add("GroupNum", typeof(int));
			}
			return Function.ParseInt(base["GroupNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("GroupNum"))
			{
				Table.Columns.Add("GroupNum", typeof(int));
			}
			base["GroupNum"] = value;
		}
	}

	public int ListNum
	{
		get
		{
			if (!Table.Columns.Contains("ListNum"))
			{
				Table.Columns.Add("ListNum", typeof(int));
			}
			return Function.ParseInt(base["ListNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("ListNum"))
			{
				Table.Columns.Add("ListNum", typeof(int));
			}
			base["ListNum"] = value;
		}
	}

	public string ListName
	{
		get
		{
			if (!Table.Columns.Contains("ListName"))
			{
				Table.Columns.Add("ListName", typeof(string));
			}
			return Function.ToString(base["ListName"]);
		}
		set
		{
			if (!Table.Columns.Contains("ListName"))
			{
				Table.Columns.Add("ListName", typeof(string));
			}
			base["ListName"] = value;
		}
	}

	public int PriceListNo
	{
		get
		{
			if (!Table.Columns.Contains("PriceListNo"))
			{
				Table.Columns.Add("PriceListNo", typeof(int));
			}
			return Function.ParseInt(base["PriceListNo"]);
		}
		set
		{
			if (!Table.Columns.Contains("PriceListNo"))
			{
				Table.Columns.Add("PriceListNo", typeof(int));
			}
			base["PriceListNo"] = value;
		}
	}

	public string PriceListName
	{
		get
		{
			if (!Table.Columns.Contains("PriceListName"))
			{
				Table.Columns.Add("PriceListName", typeof(string));
			}
			return Function.ToString(base["PriceListName"]);
		}
		set
		{
			if (!Table.Columns.Contains("PriceListName"))
			{
				Table.Columns.Add("PriceListName", typeof(string));
			}
			base["PriceListName"] = value;
		}
	}

	public string SeriesName
	{
		get
		{
			if (!Table.Columns.Contains("SeriesName"))
			{
				Table.Columns.Add("SeriesName", typeof(string));
			}
			return Function.ToString(base["SeriesName"]);
		}
		set
		{
			if (!Table.Columns.Contains("SeriesName"))
			{
				Table.Columns.Add("SeriesName", typeof(string));
			}
			base["SeriesName"] = value;
		}
	}

	public string isIns
	{
		get
		{
			if (!Table.Columns.Contains("isIns"))
			{
				Table.Columns.Add("isIns", typeof(string));
			}
			return Function.ToString(base["isIns"]);
		}
		set
		{
			if (!Table.Columns.Contains("isIns"))
			{
				Table.Columns.Add("isIns", typeof(string));
			}
			base["isIns"] = value;
		}
	}

	public string JrnlMemo
	{
		get
		{
			if (!Table.Columns.Contains("JrnlMemo"))
			{
				Table.Columns.Add("JrnlMemo", typeof(string));
			}
			return Function.ToString(base["JrnlMemo"]);
		}
		set
		{
			if (!Table.Columns.Contains("JrnlMemo"))
			{
				Table.Columns.Add("JrnlMemo", typeof(string));
			}
			base["JrnlMemo"] = value;
		}
	}

	public double Total
	{
		get
		{
			if (!Table.Columns.Contains("Total"))
			{
				Table.Columns.Add("Total", typeof(double));
			}
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

	public DateTime? ReqDate
	{
		get
		{
			if (!Table.Columns.Contains("ReqDate"))
			{
				Table.Columns.Add("ReqDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["ReqDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("ReqDate"))
			{
				Table.Columns.Add("ReqDate", typeof(DateTime));
			}
			base["ReqDate"] = value;
		}
	}

	public int Series
	{
		get
		{
			if (!Table.Columns.Contains("Series"))
			{
				Table.Columns.Add("Series", typeof(int));
			}
			return Function.ParseInt(base["Series"]);
		}
		set
		{
			if (!Table.Columns.Contains("Series"))
			{
				Table.Columns.Add("Series", typeof(int));
			}
			base["Series"] = value;
		}
	}

	public double Discount
	{
		get
		{
			if (!Table.Columns.Contains("Discount"))
			{
				Table.Columns.Add("Discount", typeof(double));
			}
			return Function.ParseDouble(base["Discount"]);
		}
		set
		{
			if (!Table.Columns.Contains("Discount"))
			{
				Table.Columns.Add("Discount", typeof(double));
			}
			base["Discount"] = value;
		}
	}

	public string SuppName
	{
		get
		{
			if (!Table.Columns.Contains("SuppName"))
			{
				Table.Columns.Add("SuppName", typeof(string));
			}
			return Function.ToString(base["SuppName"]);
		}
		set
		{
			if (!Table.Columns.Contains("SuppName"))
			{
				Table.Columns.Add("SuppName", typeof(string));
			}
			base["SuppName"] = value;
		}
	}

	public string AgentName
	{
		get
		{
			if (!Table.Columns.Contains("AgentName"))
			{
				Table.Columns.Add("AgentName", typeof(string));
			}
			return Function.ToString(base["AgentName"]);
		}
		set
		{
			if (!Table.Columns.Contains("AgentName"))
			{
				Table.Columns.Add("AgentName", typeof(string));
			}
			base["AgentName"] = value;
		}
	}

	public string DocStatus
	{
		get
		{
			if (!Table.Columns.Contains("DocStatus"))
			{
				Table.Columns.Add("DocStatus", typeof(string));
			}
			return Function.ToString(base["DocStatus"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocStatus"))
			{
				Table.Columns.Add("DocStatus", typeof(string));
			}
			base["DocStatus"] = value;
		}
	}

	public string AgentNum
	{
		get
		{
			if (!Table.Columns.Contains("AgentNum"))
			{
				Table.Columns.Add("AgentNum", typeof(string));
			}
			return Function.ToString(base["AgentNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("AgentNum"))
			{
				Table.Columns.Add("AgentNum", typeof(string));
			}
			base["AgentNum"] = value;
		}
	}

	public string Descr
	{
		get
		{
			if (!Table.Columns.Contains("Descr"))
			{
				Table.Columns.Add("Descr", typeof(string));
			}
			return Function.ToString(base["Descr"]);
		}
		set
		{
			if (!Table.Columns.Contains("Descr"))
			{
				Table.Columns.Add("Descr", typeof(string));
			}
			base["Descr"] = value;
		}
	}

	public string Ref1
	{
		get
		{
			if (!Table.Columns.Contains("Ref1"))
			{
				Table.Columns.Add("Ref1", typeof(string));
			}
			return Function.ToString(base["Ref1"]);
		}
		set
		{
			if (!Table.Columns.Contains("Ref1"))
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
			if (!Table.Columns.Contains("Ref2"))
			{
				Table.Columns.Add("Ref2", typeof(string));
			}
			return Function.ToString(base["Ref2"]);
		}
		set
		{
			if (!Table.Columns.Contains("Ref2"))
			{
				Table.Columns.Add("Ref2", typeof(string));
			}
			base["Ref2"] = value;
		}
	}

	public string DocCur
	{
		get
		{
			if (!Table.Columns.Contains("DocCur"))
			{
				Table.Columns.Add("DocCur", typeof(string));
			}
			return Function.ToString(base["DocCur"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocCur"))
			{
				Table.Columns.Add("DocCur", typeof(string));
			}
			base["DocCur"] = value;
		}
	}

	public double OIPFDocRate
	{
		get
		{
			if (!Table.Columns.Contains("OIPFDocRate"))
			{
				Table.Columns.Add("OIPFDocRate", typeof(double));
			}
			return Function.ParseDouble(base["OIPFDocRate"]);
		}
		set
		{
			if (!Table.Columns.Contains("OIPFDocRate"))
			{
				Table.Columns.Add("OIPFDocRate", typeof(double));
			}
			base["OIPFDocRate"] = value;
		}
	}

	public double ExpCustom
	{
		get
		{
			if (!Table.Columns.Contains("ExpCustom"))
			{
				Table.Columns.Add("ExpCustom", typeof(double));
			}
			return Function.ParseDouble(base["ExpCustom"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExpCustom"))
			{
				Table.Columns.Add("ExpCustom", typeof(double));
			}
			base["ExpCustom"] = value;
		}
	}

	public double ActCustom
	{
		get
		{
			if (!Table.Columns.Contains("ActCustom"))
			{
				Table.Columns.Add("ActCustom", typeof(double));
			}
			return Function.ParseDouble(base["ActCustom"]);
		}
		set
		{
			if (!Table.Columns.Contains("ActCustom"))
			{
				Table.Columns.Add("ActCustom", typeof(double));
			}
			base["ActCustom"] = value;
		}
	}

	public double Vat1
	{
		get
		{
			if (!Table.Columns.Contains("Vat1"))
			{
				Table.Columns.Add("Vat1", typeof(double));
			}
			return Function.ParseDouble(base["Vat1"]);
		}
		set
		{
			if (!Table.Columns.Contains("Vat1"))
			{
				Table.Columns.Add("Vat1", typeof(double));
			}
			base["Vat1"] = value;
		}
	}

	public double Vat2
	{
		get
		{
			if (!Table.Columns.Contains("Vat2"))
			{
				Table.Columns.Add("Vat2", typeof(double));
			}
			return Function.ParseDouble(base["Vat2"]);
		}
		set
		{
			if (!Table.Columns.Contains("Vat2"))
			{
				Table.Columns.Add("Vat2", typeof(double));
			}
			base["Vat2"] = value;
		}
	}

	public double BeforeVat
	{
		get
		{
			if (!Table.Columns.Contains("BeforeVat"))
			{
				Table.Columns.Add("BeforeVat", typeof(double));
			}
			return Function.ParseDouble(base["BeforeVat"]);
		}
		set
		{
			if (!Table.Columns.Contains("BeforeVat"))
			{
				Table.Columns.Add("BeforeVat", typeof(double));
			}
			base["BeforeVat"] = value;
		}
	}

	public double OIPFDocTotal
	{
		get
		{
			if (!Table.Columns.Contains("OIPFDocTotal"))
			{
				Table.Columns.Add("OIPFDocTotal", typeof(double));
			}
			return Function.ParseDouble(base["OIPFDocTotal"]);
		}
		set
		{
			if (!Table.Columns.Contains("OIPFDocTotal"))
			{
				Table.Columns.Add("OIPFDocTotal", typeof(double));
			}
			base["OIPFDocTotal"] = value;
		}
	}

	public double CostSum
	{
		get
		{
			if (!Table.Columns.Contains("CostSum"))
			{
				Table.Columns.Add("CostSum", typeof(double));
			}
			return Function.ParseDouble(base["CostSum"]);
		}
		set
		{
			if (!Table.Columns.Contains("CostSum"))
			{
				Table.Columns.Add("CostSum", typeof(double));
			}
			base["CostSum"] = value;
		}
	}

	public TimeSpan OIPFDocTime
	{
		get
		{
			TimeSpan result = default;
			if (!Table.Columns.Contains("OIPFDocTime"))
			{
				Table.Columns.Add("OIPFDocTime", typeof(TimeSpan));
				TimeSpan.TryParse(base["OIPFDocTime"].ToString(), out result);
			}
			return result;
		}
		set
		{
			if (!Table.Columns.Contains("OIPFDocTime"))
			{
				Table.Columns.Add("OIPFDocTime", typeof(TimeSpan));
			}
			base["OIPFDocTime"] = value;
		}
	}

	public string OIPFCanceled
	{
		get
		{
			if (!Table.Columns.Contains("OIPFCanceled"))
			{
				Table.Columns.Add("OIPFCanceled", typeof(string));
			}
			return Function.ToString(base["OIPFCanceled"]);
		}
		set
		{
			if (!Table.Columns.Contains("OIPFCanceled"))
			{
				Table.Columns.Add("OIPFCanceled", typeof(string));
			}
			base["OIPFCanceled"] = value;
		}
	}

	public double ExCustomFC
	{
		get
		{
			if (!Table.Columns.Contains("ExCustomFC"))
			{
				Table.Columns.Add("ExCustomFC", typeof(double));
			}
			return Function.ParseDouble(base["ExCustomFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExCustomFC"))
			{
				Table.Columns.Add("ExCustomFC", typeof(double));
			}
			base["ExCustomFC"] = value;
		}
	}

	public double AcCustomFC
	{
		get
		{
			if (!Table.Columns.Contains("AcCustomFC"))
			{
				Table.Columns.Add("AcCustomFC", typeof(double));
			}
			return Function.ParseDouble(base["AcCustomFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("AcCustomFC"))
			{
				Table.Columns.Add("AcCustomFC", typeof(double));
			}
			base["AcCustomFC"] = value;
		}
	}

	public double Vat1FC
	{
		get
		{
			if (!Table.Columns.Contains("Vat1FC"))
			{
				Table.Columns.Add("Vat1FC", typeof(double));
			}
			return Function.ParseDouble(base["Vat1FC"]);
		}
		set
		{
			if (!Table.Columns.Contains("Vat1FC"))
			{
				Table.Columns.Add("Vat1FC", typeof(double));
			}
			base["Vat1FC"] = value;
		}
	}

	public double Vat2FC
	{
		get
		{
			if (!Table.Columns.Contains("Vat2FC"))
			{
				Table.Columns.Add("Vat2FC", typeof(double));
			}
			return Function.ParseDouble(base["Vat2FC"]);
		}
		set
		{
			if (!Table.Columns.Contains("Vat2FC"))
			{
				Table.Columns.Add("Vat2FC", typeof(double));
			}
			base["Vat2FC"] = value;
		}
	}

	public double BeforVatFC
	{
		get
		{
			if (!Table.Columns.Contains("BeforVatFC"))
			{
				Table.Columns.Add("BeforVatFC", typeof(double));
			}
			return Function.ParseDouble(base["BeforVatFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("BeforVatFC"))
			{
				Table.Columns.Add("BeforVatFC", typeof(double));
			}
			base["BeforVatFC"] = value;
		}
	}

	public double DocTotalFC
	{
		get
		{
			if (!Table.Columns.Contains("DocTotalFC"))
			{
				Table.Columns.Add("DocTotalFC", typeof(double));
			}
			return Function.ParseDouble(base["DocTotalFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("DocTotalFC"))
			{
				Table.Columns.Add("DocTotalFC", typeof(double));
			}
			base["DocTotalFC"] = value;
		}
	}

	public double CostSumFC
	{
		get
		{
			if (!Table.Columns.Contains("CostSumFC"))
			{
				Table.Columns.Add("CostSumFC", typeof(double));
			}
			return Function.ParseDouble(base["CostSumFC"]);
		}
		set
		{
			if (!Table.Columns.Contains("CostSumFC"))
			{
				Table.Columns.Add("CostSumFC", typeof(double));
			}
			base["CostSumFC"] = value;
		}
	}

	public DateTime? CloseDate
	{
		get
		{
			if (!Table.Columns.Contains("ClsDate"))
			{
				Table.Columns.Add("ClsDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["ClsDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("ClsDate"))
			{
				Table.Columns.Add("ClsDate", typeof(DateTime));
			}
			base["ClsDate"] = value;
		}
	}

	public double Cost_Match
	{
		get
		{
			if (!Table.Columns.Contains("Cost_Match"))
			{
				Table.Columns.Add("Cost_Match", typeof(double));
			}
			return Function.ParseDouble(base["Cost_Match"]);
		}
		set
		{
			if (!Table.Columns.Contains("Cost_Match"))
			{
				Table.Columns.Add("Cost_Match", typeof(double));
			}
			base["Cost_Match"] = value;
		}
	}

	public double C_Match_FC
	{
		get
		{
			if (!Table.Columns.Contains("C_Match_FC"))
			{
				Table.Columns.Add("C_Match_FC", typeof(double));
			}
			return Function.ParseDouble(base["C_Match_FC"]);
		}
		set
		{
			if (!Table.Columns.Contains("C_Match_FC"))
			{
				Table.Columns.Add("C_Match_FC", typeof(double));
			}
			base["C_Match_FC"] = value;
		}
	}

	public string BillOfLad
	{
		get
		{
			if (!Table.Columns.Contains("BillOfLad"))
			{
				Table.Columns.Add("BillOfLad", typeof(string));
			}
			return Function.ToString(base["BillOfLad"]);
		}
		set
		{
			if (!Table.Columns.Contains("BillOfLad"))
			{
				Table.Columns.Add("BillOfLad", typeof(string));
			}
			base["BillOfLad"] = value;
		}
	}

	public int OIPFTrnspCode
	{
		get
		{
			if (!Table.Columns.Contains("OIPFTrnspCode"))
			{
				Table.Columns.Add("OIPFTrnspCode", typeof(int));
			}
			return Function.ParseInt(base["OIPFTrnspCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("OIPFTrnspCode"))
			{
				Table.Columns.Add("OIPFTrnspCode", typeof(int));
			}
			base["OIPFTrnspCode"] = value;
		}
	}

	public double CostFactor
	{
		get
		{
			if (!Table.Columns.Contains("CostFactor"))
			{
				Table.Columns.Add("CostFactor", typeof(double));
			}
			return Function.ParseDouble(base["CostFactor"]);
		}
		set
		{
			if (!Table.Columns.Contains("CostFactor"))
			{
				Table.Columns.Add("CostFactor", typeof(double));
			}
			base["CostFactor"] = value;
		}
	}

	public DateTime? CreateDate
	{
		get
		{
			if (!Table.Columns.Contains("CreateDate"))
			{
				Table.Columns.Add("CreateDate", typeof(DateTime));
			}
			return Function.ParseDateTime(base["CreateDate"]);
		}
		set
		{
			if (!Table.Columns.Contains("CreateDate"))
			{
				Table.Columns.Add("CreateDate", typeof(DateTime));
			}
			base["CreateDate"] = value;
		}
	}

	public int JdtNum
	{
		get
		{
			if (!Table.Columns.Contains("JdtNum"))
			{
				Table.Columns.Add("JdtNum", typeof(int));
			}
			return Function.ParseInt(base["JdtNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("JdtNum"))
			{
				Table.Columns.Add("JdtNum", typeof(int));
			}
			base["JdtNum"] = value;
		}
	}

	public string JdtMemo
	{
		get
		{
			if (!Table.Columns.Contains("JdtMemo"))
			{
				Table.Columns.Add("JdtMemo", typeof(string));
			}
			return Function.ToString(base["JdtMemo"]);
		}
		set
		{
			if (!Table.Columns.Contains("JdtMemo"))
			{
				Table.Columns.Add("JdtMemo", typeof(string));
			}
			base["JdtMemo"] = value;
		}
	}

	public int UserSign
	{
		get
		{
			if (!Table.Columns.Contains("UserSign"))
			{
				Table.Columns.Add("UserSign", typeof(int));
			}
			return Function.ParseInt(base["UserSign"]);
		}
		set
		{
			if (!Table.Columns.Contains("UserSign"))
			{
				Table.Columns.Add("UserSign", typeof(int));
			}
			base["UserSign"] = value;
		}
	}

	public string OIPFObjType
	{
		get
		{
			if (!Table.Columns.Contains("OIPFObjType"))
			{
				Table.Columns.Add("OIPFObjType", typeof(string));
			}
			return Function.ToString(base["OIPFObjType"]);
		}
		set
		{
			if (!Table.Columns.Contains("OIPFObjType"))
			{
				Table.Columns.Add("OIPFObjType", typeof(string));
			}
			base["OIPFObjType"] = value;
		}
	}

	public double ExCustomSC
	{
		get
		{
			if (!Table.Columns.Contains("ExCustomSC"))
			{
				Table.Columns.Add("ExCustomSC", typeof(double));
			}
			return Function.ParseDouble(base["ExCustomSC"]);
		}
		set
		{
			if (!Table.Columns.Contains("ExCustomSC"))
			{
				Table.Columns.Add("ExCustomSC", typeof(double));
			}
			base["ExCustomSC"] = value;
		}
	}

	public double ActCustSC
	{
		get
		{
			if (!Table.Columns.Contains("ActCustSC"))
			{
				Table.Columns.Add("ActCustSC", typeof(double));
			}
			return Function.ParseDouble(base["ActCustSC"]);
		}
		set
		{
			if (!Table.Columns.Contains("ActCustSC"))
			{
				Table.Columns.Add("ActCustSC", typeof(double));
			}
			base["ActCustSC"] = value;
		}
	}

	public double TtlCostSC
	{
		get
		{
			if (!Table.Columns.Contains("TtlCostSC"))
			{
				Table.Columns.Add("TtlCostSC", typeof(double));
			}
			return Function.ParseDouble(base["TtlCostSC"]);
		}
		set
		{
			if (!Table.Columns.Contains("TtlCostSC"))
			{
				Table.Columns.Add("TtlCostSC", typeof(double));
			}
			base["TtlCostSC"] = value;
		}
	}

	public string OpenForLaC
	{
		get
		{
			if (!Table.Columns.Contains("OpenForLaC"))
			{
				Table.Columns.Add("OpenForLaC", typeof(string));
			}
			return Function.ToString(base["OpenForLaC"]);
		}
		set
		{
			if (!Table.Columns.Contains("OpenForLaC"))
			{
				Table.Columns.Add("OpenForLaC", typeof(string));
			}
			base["OpenForLaC"] = value;
		}
	}

	public string incCustom
	{
		get
		{
			if (!Table.Columns.Contains("incCustom"))
			{
				Table.Columns.Add("incCustom", typeof(string));
			}
			return Function.ToString(base["incCustom"]);
		}
		set
		{
			if (!Table.Columns.Contains("incCustom"))
			{
				Table.Columns.Add("incCustom", typeof(string));
			}
			base["incCustom"] = value;
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

	public DocumentLineTable Lines { get; set; }

	public string Reference2
	{
		get
		{
			if (!Table.Columns.Contains("Ref2"))
			{
				Table.Columns.Add("Ref2", typeof(string));
			}
			return Function.ToString(base["Ref2"]);
		}
		set
		{
			if (!Table.Columns.Contains("Ref2"))
			{
				Table.Columns.Add("Ref2", typeof(string));
			}
			base["Ref2"] = value;
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

	public int AtcEntry
	{
		get
		{
			if (!Table.Columns.Contains("AtcEntry"))
			{
				Table.Columns.Add("AtcEntry", typeof(int));
			}
			return Function.ParseInt(base["AtcEntry"]);
		}
		set
		{
			if (!Table.Columns.Contains("AtcEntry"))
			{
				Table.Columns.Add("AtcEntry", typeof(int));
			}
			base["AtcEntry"] = value;
		}
	}

	public int BPLId
	{
		get
		{
			if (!Table.Columns.Contains("BPLId"))
			{
				Table.Columns.Add("BPLId", typeof(int));
			}
			return Function.ParseInt(base["BPLId"]);
		}
		set
		{
			if (!Table.Columns.Contains("BPLId"))
			{
				Table.Columns.Add("BPLId", typeof(int));
			}
			base["BPLId"] = value;
		}
	}

	public string VATRegNum
	{
		get
		{
			if (!Table.Columns.Contains("VATRegNum"))
			{
				Table.Columns.Add("VATRegNum", typeof(string));
			}
			return Function.ToString(base["VATRegNum"]);
		}
		set
		{
			if (!Table.Columns.Contains("VATRegNum"))
			{
				Table.Columns.Add("VATRegNum", typeof(string));
			}
			base["VATRegNum"] = value;
		}
	}

	public int OwnerCode
	{
		get
		{
			if (!Table.Columns.Contains("OwnerCode"))
			{
				Table.Columns.Add("OwnerCode", typeof(int));
			}
			return Function.ParseInt(base["OwnerCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("OwnerCode"))
			{
				Table.Columns.Add("OwnerCode", typeof(int));
			}
			base["OwnerCode"] = value;
		}
	}

	public string OwnerName
	{
		get
		{
			if (!Table.Columns.Contains("OwnerName"))
			{
				Table.Columns.Add("OwnerName", typeof(string));
			}
			return Function.ToString(base["OwnerName"]);
		}
		set
		{
			if (!Table.Columns.Contains("OwnerName"))
			{
				Table.Columns.Add("OwnerName", typeof(string));
			}
			base["OwnerName"] = value;
		}
	}

	public int SlpCode
	{
		get
		{
			if (!Table.Columns.Contains("SlpCode"))
			{
				Table.Columns.Add("SlpCode", typeof(int));
			}
			return Function.ParseInt(base["SlpCode"]);
		}
		set
		{
			if (!Table.Columns.Contains("SlpCode"))
			{
				Table.Columns.Add("SlpCode", typeof(int));
			}
			base["SlpCode"] = value;
		}
	}

	public string JournalMemo
	{
		get
		{
			if (!Table.Columns.Contains("JrnlMemo"))
			{
				Table.Columns.Add("JrnlMemo", typeof(string));
			}
			return Function.ToString(base["JrnlMemo"]);
		}
		set
		{
			if (!Table.Columns.Contains("JrnlMemo"))
			{
				Table.Columns.Add("JrnlMemo", typeof(string));
			}
			base["JrnlMemo"] = value;
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

	public string CtlAccount
	{
		get
		{
			if (!Table.Columns.Contains("CtlAccount"))
			{
				Table.Columns.Add("CtlAccount", typeof(string));
			}
			return Function.ToString(base["CtlAccount"]);
		}
		set
		{
			if (!Table.Columns.Contains("CtlAccount"))
			{
				Table.Columns.Add("CtlAccount", typeof(string));
			}
			base["CtlAccount"] = value;
		}
	}

	public string PickRmrk
	{
		get
		{
			if (!Table.Columns.Contains("PickRmrk"))
			{
				Table.Columns.Add("PickRmrk", typeof(string));
			}
			return Function.ToString(base["PickRmrk"]);
		}
		set
		{
			if (!Table.Columns.Contains("PickRmrk"))
			{
				Table.Columns.Add("PickRmrk", typeof(string));
			}
			base["PickRmrk"] = value;
		}
	}

	internal DocumentRow(DataRowBuilder builder)
		: base(builder)
	{
	}
}
