using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaItemMasterDataRepository : ItemMasterDataRepository
{
	public HanaItemMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetPriceQuery()
	{
		return "SELECT * FROM ITM1 WHERE \"ItemCode\"=ItemCode;";
	}

	protected override string GetItemWarehouseQuery()
	{
		return "SELECT * FROM OITW WHERE \"ItemCode\"=ItemCode;";
	}

	protected override string CheckDocQuery()
	{
		return "SELECT 1 FROM OITM WHERE \"ItemCode\" = itemCode;";
	}

	protected override string GetItemPropertiesQuery2()
	{
		return "SELECT 'N' AS \"QryGroup1\", 'N' AS \"QryGroup2\", 'N' AS \"QryGroup3\", 'N' AS \"QryGroup4\", 'N' AS \"QryGroup5\", 'N' AS \"QryGroup6\", 'N' AS \"QryGroup7\", 'N' AS \"QryGroup8\", 'N' AS \"QryGroup9\", 'N' AS \"QryGroup10\", 'N' AS \"QryGroup11\", 'N' AS \"QryGroup12\", 'N' AS \"QryGroup13\", 'N' AS \"QryGroup14\", 'N' AS \"QryGroup15\", 'N' AS \"QryGroup16\", 'N' AS \"QryGroup17\", 'N' AS \"QryGroup18\", 'N' AS \"QryGroup19\", 'N' AS \"QryGroup20\", 'N' AS \"QryGroup21\", 'N' AS \"QryGroup22\", 'N' AS \"QryGroup23\", 'N' AS \"QryGroup24\", 'N' AS \"QryGroup25\", 'N' AS \"QryGroup26\", 'N' AS \"QryGroup27\", 'N' AS \"QryGroup28\", 'N' AS \"QryGroup29\", 'N' AS \"QryGroup30\", 'N' AS \"QryGroup31\", 'N' AS \"QryGroup32\", 'N' AS \"QryGroup33\", 'N' AS \"QryGroup34\", 'N' AS \"QryGroup35\", 'N' AS \"QryGroup36\", 'N' AS \"QryGroup37\", 'N' AS \"QryGroup38\", 'N' AS \"QryGroup39\", 'N' AS \"QryGroup40\", 'N' AS \"QryGroup41\", 'N' AS \"QryGroup42\", 'N' AS \"QryGroup43\", 'N' AS \"QryGroup44\", 'N' AS \"QryGroup45\", 'N' AS \"QryGroup46\", 'N' AS \"QryGroup47\", 'N' AS \"QryGroup48\", 'N' AS \"QryGroup49\", 'N' AS \"QryGroup50\", 'N' AS \"QryGroup51\", 'N' AS \"QryGroup52\", 'N' AS \"QryGroup53\", 'N' AS \"QryGroup54\", 'N' AS \"QryGroup55\", 'N' AS \"QryGroup56\", 'N' AS \"QryGroup57\", 'N' AS \"QryGroup58\", 'N' AS \"QryGroup59\", 'N' AS \"QryGroup60\", 'N' AS \"QryGroup61\", 'N' AS \"QryGroup62\", 'N' AS \"QryGroup63\", 'N' AS \"QryGroup64\" FROM DUMMY;\r\n                SELECT \"ItmsGrpNam\" FROM OITG ORDER BY \"ItmsTypCod\" ASC; ";
	}

	protected override string CflQuery()
	{
		return "SELECT \"ItemCode\", \"ItemName\", \"DfltWH\", \"OnHand\" FROM OITM;   ";
	}

	protected override string CheckExistItemQuery()
	{
		return "Select \"ItemCode\" from oitm where \"ItemCode\"=ItemCode;";
	}

	protected override string Check_FirstRecordQuery()
	{
		return " SELECT \"ItemCode\" FROM OITM WHERE \"ItemCode\" = (SELECT MIN(\"ItemCode\") FROM OITM); ";
	}

	protected override string Check_LastRecordQuery()
	{
		return "SELECT \"ItemCode\" FROM OITM WHERE \"ItemCode\" = (SELECT MAX(\"ItemCode\") FROM OITM);";
	}

	protected override string DeleteOITWQuery()
	{
		return " DELETE FROM OITW WHERE \"ItemCode\" = itemCode AND \"WhsCode\" = whsCode;";
	}

	protected override string ExistFileInDocQuery()
	{
		return "SELECT \"FileName\" FROM atc1 WHERE \"AbsEntry\" = (SELECT TOP 1 \"AtcEntry\" FROM OITM WHERE \"ItemCode\" = itemCode) AND \"FileExt\" = fileExt AND \"FileName\" = fileName AND \"AbsEntry\" IS NOT NULL; ";
	}

	protected override string GetAPreferredVendorDataQuery()
	{
		return "SELECT a.\"CardName\", a.\"ListNum\", b.\"Price\", b.\"Currency\" AS \"PriceCurr\", c.\"Price\" AS \"LastPurPrc\", c.\"Currency\" AS \"PurchaseCurr\" FROM OCRD a LEFT OUTER JOIN ITM1 b ON b.\"PriceList\" = a.\"ListNum\" AND b.\"ItemCode\" = itemCode LEFT OUTER JOIN ITM1 c ON c.\"ItemCode\" = itemCode AND c.\"PriceList\" = a.\"ListNum\" WHERE a.\"CardCode\" = cardCode;";
	}

	protected override string GetAllDivisionQuery()
	{
		return "SELECT \"Code\", \"Name\", '' AS \"Url\" FROM \"@ARGNS_DIV\" WHERE \"U_type\" = '01';";
	}

	protected override string GetCategoryQuery()
	{
		return "SELECT DISTINCT U_ARGNS_M_GROUP AS \"CatId\", b.\"Name\" AS \"Cat\", '' AS \"Url\" FROM OITM a LEFT OUTER JOIN \"@ARGNS_MODELGRP\" b ON a.U_ARGNS_M_GROUP = b.\"Code\" WHERE U_ARGNS_DIV = DivCode AND a.\"validFor\" = 'Y' OR (a.\"validFor\" = 'N' AND a.\"frozenFor\" = 'N');";
	}

	protected override string GetDefaultLengthCodeQuery()
	{
		return "SELECT TOP 1 \"UnitCode\" FROM OLGT ORDER BY \"UnitName\";";
	}

	protected override string GetItemGroupDataQuery()
	{
		return "SELECT \"PlaningSys\", \"PrcrmntMtd\", \"CompoWH\", \"OrdrIntrvl\", \"OrdrMulti\", \"MinOrdrQty\", \"LeadTime\", \"ToleranDay\", \"UgpEntry\", \"InvntSys\" FROM OITB WHERE \"ItmsGrpCod\" = itmsGrpCod;";
	}

	protected override string GetItemLineTableStructureQuery()
	{
		return "select * from OPLN Where \"ValidFor\"='Y';";
	}

	protected override string GetItemPropertiesQuery1()
	{
		return "SELECT \"QryGroup1\", \"QryGroup2\", \"QryGroup3\", \"QryGroup4\", \"QryGroup5\", \"QryGroup6\", \"QryGroup7\", \"QryGroup8\", \"QryGroup9\", \"QryGroup10\", \"QryGroup11\", \"QryGroup12\", \"QryGroup13\", \"QryGroup14\", \"QryGroup15\", \"QryGroup16\", \"QryGroup17\", \"QryGroup18\", \"QryGroup19\", \"QryGroup20\", \"QryGroup21\", \"QryGroup22\", \"QryGroup23\", \"QryGroup24\", \"QryGroup25\", \"QryGroup26\", \"QryGroup27\", \"QryGroup28\", \"QryGroup29\", \"QryGroup30\", \"QryGroup31\", \"QryGroup32\", \"QryGroup33\", \"QryGroup34\", \"QryGroup35\", \"QryGroup36\", \"QryGroup37\", \"QryGroup38\", \"QryGroup39\", \"QryGroup40\", \"QryGroup41\", \"QryGroup42\", \"QryGroup43\", \"QryGroup44\", \"QryGroup45\", \"QryGroup46\", \"QryGroup47\", \"QryGroup48\", \"QryGroup49\", \"QryGroup50\", \"QryGroup51\", \"QryGroup52\", \"QryGroup53\", \"QryGroup54\", \"QryGroup55\", \"QryGroup56\", \"QryGroup57\", \"QryGroup58\", \"QryGroup59\", \"QryGroup60\", \"QryGroup61\", \"QryGroup62\", \"QryGroup63\", \"QryGroup64\" FROM OITM WHERE \"ItemCode\" = ItemCode;\r\n            SELECT \"ItmsGrpNam\" FROM OITG ORDER BY \"ItmsTypCod\" ASC; ";
	}

	protected override string GetItemTypeQuery()
	{
		return "SELECT \"ItemType\" From OITM Where \"ItemCode\"=ItemCode;";
	}

	protected override string GetItemsPerUnitQuery1()
	{
		return "SELECT \"NumInBuy\" FROM OITM WHERE \"ItemCode\"=ItemCode;";
	}

	protected override string GetItemsPerUnitQuery2()
	{
		return "SELECT \"NumInSale\" FROM OITM WHERE \"ItemCode\"=ItemCode;";
	}

	protected override string GetLghtAndWghtDisplayQuery()
	{
		return "SELECT b.\"UnitDisply\" AS \"UnitDisplyLength\", b.\"UnitCode\" AS \"UnitCodeLength\", c.\"UnitDisply\" AS \"UnitDisplyWeight\", c.\"UnitCode\" AS \"UnitCodeWeight\" FROM OADM a INNER JOIN OLGT b ON a.\"DefLengthU\" = b.\"UnitCode\" INNER JOIN OWGT c ON a.\"DefWeightU\" = c.\"UnitCode\";";
	}

	protected override string GetModelQuery()
	{
		return "SELECT DISTINCT U_ARGNS_MOD AS \"Code\", b.\"U_ModDesc\" AS \"Name\", '' AS \"Url\" FROM OITM a LEFT OUTER JOIN \"@ARGNS_MODEL\" b ON a.U_ARGNS_MOD = b.\"U_ModCode\" WHERE U_ARGNS_DIV = 1 AND U_ARGNS_M_GROUP = Group;";
	}

	protected override string GetOCYCQuery()
	{
		return "SELECT IFNULL(\"Code\", 0) AS \"Code\", IFNULL(\"Name\", '') AS \"Name\" FROM OCYC WHERE \"Type\" = 'M' ORDER BY \"Code\"; ";
	}

	protected override string GetPreferredVendorQuery()
	{
		return "SELECT a.*, b.\"CardName\", b.\"ListNum\", c.\"Price\", c.\"Currency\" AS \"PriceCurr\", d.\"Price\" AS \"LastPurPrc\", d.\"Currency\" AS \"PurchaseCurr\" FROM ITM2 a LEFT OUTER JOIN OCRD b ON a.\"VendorCode\" = b.\"CardCode\" LEFT OUTER JOIN ITM1 c ON a.\"ItemCode\" = c.\"ItemCode\" AND c.\"PriceList\" = b.\"ListNum\" LEFT OUTER JOIN ITM1 d ON a.\"ItemCode\" = d.\"ItemCode\" AND d.\"PriceList\" = b.\"ListNum\" WHERE a.\"ItemCode\" = itemCode;";
	}

	protected override string GetProductionDataQuery()
	{
		return "SELECT (SELECT CASE \"TreeType\" WHEN 'A' THEN 'Assembly' WHEN 'S' THEN 'Sales' WHEN 'P' THEN 'Production' WHEN 'T' THEN 'Template' END FROM OITT WHERE \"Code\" = itemCode) AS \"TreeType\", (SELECT COUNT(1) FROM ITT1 WHERE \"Father\" = itemCode AND \"Type\" = 4) AS \"NoOfItem\", (SELECT COUNT(1) FROM ITT1 WHERE \"Father\" = itemCode AND \"Type\" = 290) AS \"NoOfRes\" FROM DUMMY;";
	}

	protected override string GetTop1Query()
	{
		return "SELECT * FROM oitm  WHERE \"ItemCode\"=ItemCode;";
	}

	protected override string GetUoMGroupQuery()
	{
		return "SELECT \"UgpEntry\" AS \"Code\", \"UgpCode\" AS \"Name\" FROM OUGP;";
	}

	protected override string LoadPriceListQuery()
	{
		return "SELECT \"ListNum\", \"ListName\" FROM OPLN UNION ALL SELECT -1, n'Last Purchase Price' FROM DUMMY UNION ALL SELECT -2, n'Last Evaluated Price' FROM DUMMY;";
	}

	protected override string SearchOITWQuery()
	{
		return "SELECT a.* FROM OITW a INNER JOIN OWHS b ON a.\"WhsCode\" = b.\"WhsCode\" WHERE a.\"ItemCode\" = ItemCode AND IFNULL(b.\"Inactive\", 'N') <> 'Y';";
	}

	protected override string SearchQuery1(DataTable udfDt, ref List<SqlParameter> prms)
	{
		string text = "";
		DataTable dataTable = ExecuteDataTable("SELECT COLUMN_NAME,DATA_TYPE_NAME as DATA_TYPE FROM TABLE_COLUMNS WHERE TABLE_NAME = 'OITM' AND substring(COLUMN_NAME, 1, 2) = 'U_' AND SCHEMA_NAME = CURRENT_SCHEMA;", CommandType.Text);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text2 = Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]);
				string text3 = Function.ToString(dataTable.Rows[i]["DATA_TYPE"]);
				if (!udfDt.Columns.Contains(text2))
				{
					continue;
				}
				string text4 = "@" + text2;
				switch (text3.ToLower())
				{
				case "decimal":
				{
					decimal num2 = Function.ParseDecimal(udfDt.Rows[0][text2]);
					if (num2 != 0m)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, num2));
					}
					continue;
				}
				case "integer":
				{
					int num = Function.ParseInt(udfDt.Rows[0][text2]);
					if (num != 0)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, num));
					}
					continue;
				}
				case "timestamp":
				{
					DateTime? dateTime = Function.ParseDateTime(udfDt.Rows[0][text2]);
					if (dateTime.HasValue)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, dateTime.Value));
					}
					continue;
				}
				}
				string value = Function.ToString(udfDt.Rows[0][text2]);
				if (!string.IsNullOrWhiteSpace(value))
				{
					text = text + " AND \"" + text2 + "\" like '%' || " + text4 + " || '%'";
					prms.Add(new SqlParameter(text4, value));
				}
			}
		}
		return text;
	}

	protected override string FindQuery()
	{
		return "\"{0}\" LIKE N'%'||@{0}||'%' ";
	}

	protected override string SearchQuery2(ItemsImpl doc, ref List<SqlParameter> prms)
	{
		string text = "";
		if (!string.IsNullOrWhiteSpace(doc.ItemCode))
		{
			text += " AND \"ItemCode\" like '%' || @ItemCode || '%'";
			prms.Add(new SqlParameter("@ItemCode", Function.ToString(doc.ItemCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ItemName))
		{
			text += " AND \"ItemName\" like '%' || @ItemName || '%'";
			prms.Add(new SqlParameter("@ItemName", Function.ToString(doc.ItemName)));
		}
		if (!string.IsNullOrWhiteSpace(doc.FrgnName))
		{
			text += " AND \"FrgnName\" like '%' || @FrgnName || '%'";
			prms.Add(new SqlParameter("@FrgnName", Function.ToString(doc.FrgnName)));
		}
		if (doc.ItmsGrpCod != -1 && doc.ItmsGrpCod != 0)
		{
			text += " AND \"ItmsGrpCod\" = @ItmsGrpCod";
			prms.Add(new SqlParameter("@ItmsGrpCod", Function.ToString(doc.ItmsGrpCod)));
		}
		if (doc.UgpEntry != 0)
		{
			text += " AND \"UgpEntry\" = @UgpEntry";
			prms.Add(new SqlParameter("@UgpEntry", Function.ToString(doc.UgpEntry)));
		}
		if (doc.ItemType == "Items")
		{
			doc.ItemType = "I";
		}
		else if (doc.ItemType == "Labor")
		{
			doc.ItemType = "L";
		}
		else if (doc.ItemType == "Travel")
		{
			doc.ItemType = "T";
		}
		if (!string.IsNullOrWhiteSpace(doc.ItemType))
		{
			text += " AND \"ItemType\" like '%' || @ItemType || '%'";
			prms.Add(new SqlParameter("@ItemType", Function.ToString(doc.ItemType)));
		}
		if (doc.ShipType != -1 && doc.ShipType != 0)
		{
			text += " AND \"ShipType\" = @ShipType";
			prms.Add(new SqlParameter("@ShipType", Function.ToString(doc.ShipType)));
		}
		if (doc.FirmCode != -1 && doc.FirmCode != 0)
		{
			text += " AND \"FirmCode\" = @FirmCode";
			prms.Add(new SqlParameter("@FirmCode", Function.ToString(doc.FirmCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.VatGroupPu))
		{
			text += " AND \"VatGroupPu\" like '%' || @VatGroupPu || '%'";
			prms.Add(new SqlParameter("@VatGroupPu", Function.ToString(doc.VatGroupPu)));
		}
		if (!string.IsNullOrWhiteSpace(doc.VatGourpSa))
		{
			text += " AND \"VatGourpSa\" like '%' || @VatGourpSa || '%'";
			prms.Add(new SqlParameter("@VatGourpSa", Function.ToString(doc.VatGourpSa)));
		}
		if (!string.IsNullOrWhiteSpace(doc.CardCode))
		{
			text += " AND \"CardCode\" like '%' || @CardCode || '%'";
			prms.Add(new SqlParameter("@CardCode", Function.ToString(doc.CardCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ManBtchNum))
		{
			text += " AND \"ManBtchNum\" = @ManBtchNum";
			prms.Add(new SqlParameter("@ManBtchNum", Function.ToString(doc.ManBtchNum)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ManSerNum))
		{
			text += " AND \"ManSerNum\" = @ManSerNum";
			prms.Add(new SqlParameter("@ManSerNum", Function.ToString(doc.ManSerNum)));
		}
		return text;
	}
}
