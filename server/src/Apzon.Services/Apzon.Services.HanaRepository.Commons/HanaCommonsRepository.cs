using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaCommonsRepository : CommonsRepository
{
	public HanaCommonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string GetDatabaseListQuery(bool loginSap)
	{
		string result = "select a.\"CmpDB\" as \"dbName\", a.\"CmpName\" as \"cmpName\" from APZ_SLSP a INNER JOIN \r\n\t\t\t\t\t\t\t(SELECT t1.\"Database\" from APZ_OLDD t1 INNER JOIN \r\n\t\t\t\t\t\t\t(Select * from APZ_OCDB WHERE \"Domain\" = domain) t2 ON t1.\"Domain\" = t2.\"Domain\") b ON b.\"Database\" = a.\"CmpDB\";";
		if (loginSap)
		{
			result = "select a.\"CmpDB\" as \"dbName\", T3.\"cmpName\" as \"cmpName\" from APZ_SLSP a INNER JOIN \r\n                        (SELECT t1.\"Database\" from APZ_OLDD t1 INNER JOIN\r\n                        (Select * from APZ_OCDB WHERE \"Domain\" = $domain) t2 ON t1.\"Domain\" = t2.\"Domain\") b ON b.\"Database\" = a.\"CmpDB\"\r\n\r\n                        INNER JOIN SBOCOMMON.SRGC T3 ON A.\"CmpDB\" = T3.\"dbName\"\r\n                        ORDER BY a.\"CmpDB\";";
		}
		return result;
	}

	protected override string GetUserCodeQuery()
	{
		return "select USERID,USER_CODE from ousr;";
	}

	public override string GetInfoCompanyDbServerQuery()
	{
		return "select \"CpnDBServer\", \"CpnDBUser\", \"CpnDBPass\", \"Status\" from APZ_SLSP where \"CmpDB\" = cmpDb;";
	}

	protected override string GetCheckExitsDatabaseQuery()
	{
		return "SELECT \"Version\" FROM APZ_CINF;";
	}

	protected override string GetDatabaseVersionQuery()
	{
		return "SELECT \"Version\" VERSION FROM APZ_CINF;";
	}

	protected override string GetServerDateQuery()
	{
		return "SELECT TO_VARCHAR(current_date, 'yyyyMMdd') from DUMMY;";
	}

	protected override string GetListUserReportsQueryInsertLayout()
	{
		return "INSERT INTO APZ_PRINTLAYOUT (\"TypeCode\", \"TypeName\", \"DocCode\", \"DocName\",\"FileName\") \r\n\t\t\t\t\t\t\t    SELECT \"TypeCode\", N'User Reports', \"DocCode\", \"DocName\", N'' FROM RDOC WHERE \"Author\" != 'System'\r\n                                AND NOT EXISTS \r\n                                                (SELECT 1 FROM APZ_PRINTLAYOUT \r\n                                                 WHERE RDOC.\"DocCode\" = APZ_PRINTLAYOUT.\"DocCode\");";
	}

	protected override string GetRoleQuery()
	{
		return "select \"typeID\",\"name\" from ohty;";
	}

	protected override string GetHardwarekeyQuery()
	{
		return "SELECT \"HardwareKey\" FROM APZ_OLDD WHERE \"Database\" = database";
	}

	protected override string GetSearchTableQuery()
	{
		return "SELECT string_agg( COLUMN_NAME || ' ' || DATA_TYPE_NAME || ',')\r\n\t                        FROM TABLE_COLUMNS\r\n\r\n                            WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME =tableName;";
	}

	protected override string ExistsTableQuery()
	{
		return "\r\n                        DECLARE TableName VARCHAR(100);\r\n                        DECLARE isExists bigint ;\r\n                        select ifnull(TablePara,'') into TableName from dummy;\r\n                        SELECT COUNT(1)  into isExists FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = TableName;\r\n                        select isExists from dummy;\r\n                  ";
	}

	protected override string GetAllFieldTableQuery()
	{
		return "SELECT \"COLUMN_NAME\", ROW_ORDER_POSITION ORDINAL_POSITION,DATA_TYPE_NAME DATA_TYPE,\"LENGTH\" CHARACTER_MAXIMUM_LENGTH FROM TABLE_COLUMNS WHERE SCHEMA_NAME = current_schema and TABLE_NAME=$apz;";
	}

	protected override string GetBillToQuerry()
	{
		return "SELECT \"Address\",IFNULL(\"Street\",'') || CHAR(13) || IFNULL(\"ZipCode\",'') || ' '|| IFNULL(\"City\",'') || CHAR(13) || IFNULL(\"County\",'') AS \"Street\",\"CardCode\" FROM CRD1 \r\n                                WHERE \"AdresType\"='B' AND (ifnull(@CardCode,'')='' OR \"CardCode\" =@CardCode);";
	}

	protected override string GetShipToQuery()
	{
		return "SELECT \"Address\",IFNULL(\"Street\",'') || CHAR(13) || IFNULL(\"ZipCode\",'') || ' ' || IFNULL(\"City\",'') || CHAR(13) || IFNULL(\"County\",'') AS \"Street\",\"CardCode\" FROM CRD1 \r\n                                WHERE \"AdresType\"='S' AND (ifnull(@CardCode,'')='' OR \"CardCode\" =@CardCode);";
	}

	protected override string GetProcIsExistQuery()
	{
		return "SELECT * FROM sys.procedures WHERE SCHEMA_NAME = current_schema and PROCEDURE_NAME = procName;";
	}

	protected override string LoadSeriesByPostingDateQuery()
	{
		return "SELECT \"Series\",\"SeriesName\",IFNULL(\"BPLId\",-1) \"BPLId\" FROM NNM1 WHERE \"ObjectCode\"=@ObjectCode AND (\"BPLId\" IN (SELECT \"BPLId\" FROM USR6 WHERE \"UserCode\"=@UserCode) OR IFNULL(\"BPLId\",-1)=-1) and \"Indicator\" in (select \"Indicator\" from OFPR where \"F_RefDate\" <= @refDate and \"T_RefDate\" >= @refDate);";
	}

	protected override string ShowSeriesQuery()
	{
		return "SELECT \"Series\",\"SeriesName\" from NNM1;";
	}

	protected override string GetSeriesQuery()
	{
		return "select \"Series\" as \"Value\", \"SeriesName\" as \"Name\" from nnm1 where \"ObjectCode\" = ObjectCode;";
	}

	protected override string GetNextNumberSeriesQuery()
	{
		return "select  \"NextNumber\" from nnm1 where \"Series\" = series;";
	}

	protected override string GetDefaultSeriesQuery()
	{
		return "if (select count(1) from NNM2 where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType and \"UserSign\" = @userId and \"Series\" is not null) > 0 then\r\n                        select top 1 \"Series\" from NNM2 where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType and \"UserSign\" = @userId;\r\n                     else\r\n                        select top 1 \"DfltSeries\" from ONNM where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType;\r\n                     end if;";
	}

	protected override string ContactPersonQuery()
	{
		return "SELECT  ifnull(\"CntctCode\",0) as \"CntctCode\",\r\n\t\t                             ifnull(\"Name\",'') as \"Name\" , \"CardCode\"\r\n                             FROM OCPR WHERE \"CardCode\" = @cardCode;";
	}

	protected override string GetDataStructureQuery()
	{
		return "SELECT\r\n                         COLUMN_NAME,\r\n                         CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n                         ELSE '(('||DEFAULT_VALUE||'))' \r\n                         END AS COLUMN_DEFAULT,\r\n                         CASE IS_NULLABLE WHEN 'TRUE' \r\n                            THEN 'YES' \r\n                            ELSE 'NO' \r\n                            END AS IS_NULLABLE,\r\n                         LOWER(CASE DATA_TYPE_NAME \r\n                             WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                         WHEN 'INTEGER' THEN 'INT' \r\n\t                         WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                         WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                         ELSE DATA_TYPE_NAME \r\n\t                         END) AS DATA_TYPE,\r\n                         CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN LENGTH\r\n     \t                    ELSE NULL \r\n     \t                    END AS CHARACTER_MAXIMUM_LENGTH,\r\n                         CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN NULL\r\n                             ELSE LENGTH \r\n                             END AS NUMERIC_PRECISION,\r\n                         SCALE AS NUMERIC_SCALE \r\n                    FROM TABLE_COLUMNS \r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA \r\n                        AND TABLE_NAME = TblName\r\n                    ORDER BY POSITION;";
	}

	protected override string GetDataTableNewRowQuery()
	{
		return "SELECT\r\n\t                     COLUMN_NAME,\r\n\t                     CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n                         ELSE '(('||DEFAULT_VALUE||'))' \r\n                         END AS COLUMN_DEFAULT,\r\n\t                     CASE IS_NULLABLE WHEN 'TRUE' \r\n\t                        THEN 'YES' \r\n\t                        ELSE 'NO' \r\n\t                        END AS IS_NULLABLE,\r\n\t                     CASE DATA_TYPE_NAME WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END AS DATA_TYPE\r\n                    FROM TABLE_COLUMNS\r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME=@TblName and (IS_NULLABLE = 'NO' or DEFAULT_Value is not null);";
	}

	protected override string GetStructureTableQuery()
	{
		return "SELECT\r\n\t                     COLUMN_NAME,\r\n\t                     CASE WHEN DATA_TYPE_NAME LIKE '%CHAR' OR DATA_TYPE_NAME LIKE '%TEXT' THEN '('''||DEFAULT_VALUE||''')'\r\n                         ELSE '(('||DEFAULT_VALUE||'))' \r\n                         END AS COLUMN_DEFAULT,\r\n\t                     CASE IS_NULLABLE WHEN 'TRUE' \r\n\t                        THEN 'YES' \r\n\t                        ELSE 'NO' \r\n\t                        END AS IS_NULLABLE,\r\n\t                     CASE DATA_TYPE_NAME WHEN 'NCLOB' THEN 'NTEXT' \r\n\t                     WHEN 'INTEGER' THEN 'INT' \r\n\t                     WHEN 'TIMESTAMP' THEN 'DATETIME' \r\n\t                     WHEN 'DECIMAL' THEN 'NUMERIC' \r\n\t                     ELSE DATA_TYPE_NAME \r\n\t                     END AS DATA_TYPE\r\n                    FROM TABLE_COLUMNS\r\n                    WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME=@TblName;";
	}

	protected override string GetSql(DataTable dt, string tableName)
	{
		try
		{
			string text = "SELECT ";
			for (int i = 0; i < Function.ParseInt(dt.Rows.Count); i++)
			{
				dt.Rows[i]["COLUMN_NAME"] = "\"" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "\"";
				switch (Function.ToString(dt.Rows[i]["DATA_TYPE"]).ToUpper())
				{
				case "NVARCHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text6 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text6 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NTEXT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text11 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text11 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DATETIME":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",NOW()) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text10 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text10 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "CHAR":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",'') AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text3 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text3 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "BIT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text8 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text8 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "INT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text7 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text7 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "SMALLINT":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text12 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 2);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text12 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "NUMERIC":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text9 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text9 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "DOUBLE":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text4 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text4 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				case "MONEY":
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text5 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(1, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 1);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text5 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				default:
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"])))
					{
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",0) AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					else
					{
						string text2 = Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Substring(2, Function.ToString(dt.Rows[i]["COLUMN_DEFAULT"]).Length - 4);
						text = text + Environment.NewLine + "IFNULL(" + dt.Rows[i]["COLUMN_NAME"]?.ToString() + "," + text2 + ") AS " + dt.Rows[i]["COLUMN_NAME"]?.ToString() + ",";
					}
					break;
				}
			}
			text = text.Substring(0, text.Length - 1);
			return text + Environment.NewLine + "FROM " + tableName + ";";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Function get SQL ERROR ";
		}
	}

	protected override string GetPriceItemQuery()
	{
		return "SELECT IFNULL(\"Price\", 0) AS \"Price\", IFNULL(\"Currency\", '') AS \"Currency\" FROM ITM1 WHERE \"ItemCode\" = itemcode AND \"PriceList\" = pricelist;";
	}

	protected override string GetCurrencyByListNum()
	{
		return "SELECT IFNULL(\"PrimCurr\", '') FROM OPLN WHERE \"ListNum\" = ListNum;";
	}

	public override string ListDocByCardCodeQuery(ApzObjectTypes type, ApzObjectTypes toType, string cardCode, string postingDate, ref List<SqlParameter> prms)
	{
		List<string> tableName = Function.GetTableName(type);
		string text = tableName[0];
		string arg = tableName[1];
		prms = new List<SqlParameter>();
		string text2;
		if (type == ApzObjectTypes.oBlanketAgreement)
		{
			text2 = $"SELECT \r\n                                                \"AbsID\" AS  \"DocEntry\",\r\n                                                \"StartDate\" AS \"DocDate\",\r\n                                                \"EndDate\" AS \"DocDueDate\",\r\n                                                \"BpCode\" AS \"CardCode\",\r\n                                                \"BpName\" AS \"CardName\",\r\n                                                \"CntctCode\",\r\n                                                '' AS \"Comments\" ,\r\n                                                '0' AS \"DocTotal\" ,\r\n                                                '0' AS \"VatSum\",\r\n                                                '0' AS \"DiscPrcnt\" ,\r\n                                                '0' AS \"DocRate\",\r\n                                                '' \"NumAtCard\",\r\n                                                *\r\n                                                                        \r\n                                        FROM {text}\r\n                                        WHERE \"Status\" = 'A' AND \"BpType\" = 'C' AND \"Cancelled\" = 'N'";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text2 += " AND \"BpCode\" =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			if (!string.IsNullOrEmpty(postingDate))
			{
				text2 += " AND \"StartDate\" <= @postingDate AND \"EndDate\" >= @postingDate";
				prms.Add(new SqlParameter("@postingDate", Function.ParseDateTime(postingDate, "yyyyMMdd")));
			}
		}
		else if (type == ApzObjectTypes.oPurchaseBlanketAgreement)
		{
			text2 = $"SELECT \r\n                                                \"AbsID\" AS  \"DocEntry\",\r\n                                                \"StartDate\" AS \"DocDate\",\r\n                                                \"EndDate\" AS \"DocDueDate\",\r\n                                                \"BpCode\" AS \"CardCode\",\r\n                                                \"BpName\" AS \"CardName\",\r\n                                                \"CntctCode\",\r\n                                                '' AS \"Comments\" ,\r\n                                                '0' AS \"DocTotal\" ,\r\n                                                '0' AS \"VatSum\",\r\n                                                '0' AS \"DiscPrcnt\" ,\r\n                                                '0' AS \"DocRate\",\r\n                                                '' \"NumAtCard\",\r\n                                                *\r\n                                                                        \r\n                                        FROM {text}\r\n                                        WHERE \"Status\" = 'A' AND \"BpType\" = 'S' AND \"Cancelled\" = 'N'";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text2 += " AND \"BpCode\" =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
			if (!string.IsNullOrEmpty(postingDate))
			{
				text2 += " AND \"StartDate\" <= @postingDate AND \"EndDate\" >= @postingDate";
				prms.Add(new SqlParameter("@postingDate", Function.ParseDateTime(postingDate, "yyyyMMdd")));
			}
		}
		else if (type == ApzObjectTypes.oPurchaseRequest)
		{
			text2 = $"select *,Case When \"ReqType\" = 12 then 'User' When  \"ReqType\" = 171 then 'Employee' END AS \"ReqTypeName\"       \r\n                                            FROM {text}\r\n                                            WHERE \"DocStatus\"='O' ";
		}
		else if (type == ApzObjectTypes.oQuotations)
		{
			text2 = $"DECLARE isCopyClosedQuo NVARCHAR(1);\r\n                                    select ifNULL((SELECT IFNULL(\"ClosedQuot\",'') FROM oadm),'') into isCopyClosedQuo from DUMMY;\r\n\r\n                                    SELECT * FROM {text}\r\n                                        WHERE (\"DocStatus\"='O' AND isCopyClosedQuo<>'Y') \r\n\t                                        OR \r\n\t\t                                        (isCopyClosedQuo = 'Y' AND (\"DocStatus\"='C' OR \"DocStatus\"='O'))\r\n\t                                        ";
			if (!string.IsNullOrEmpty(cardCode))
			{
				text2 += " AND \"CardCode\" =@CardCode";
				prms.Add(new SqlParameter("@CardCode", cardCode));
			}
		}
		else
		{
			if ((type == ApzObjectTypes.oReserveInvoices && toType == ApzObjectTypes.oDeliveryNotes) || (type == ApzObjectTypes.oAPReserveInvoice && toType == ApzObjectTypes.oPurchaseDeliveryNotes))
			{
				text2 = "select * from " + text + " where \"isIns\" = 'Y' and \"InvntSttus\" = 'O'";
				if (!string.IsNullOrEmpty(cardCode))
				{
					text2 += " AND \"CardCode\" = @CardCode";
					prms.Add(new SqlParameter("@CardCode", cardCode));
				}
			}
			else
			{
				text2 = $"SELECT * FROM {text} a\r\n                                            WHERE a.\"DocStatus\"='O' and exists(select 1 from {arg} where \"DocEntry\" = a.\"DocEntry\" and  \"OpenQty\" <> 0) ";
				if (!string.IsNullOrEmpty(cardCode) && type != ApzObjectTypes.oInventoryGenExit && type != ApzObjectTypes.oInventoryGenEntry)
				{
					text2 = ((type != ApzObjectTypes.oStockTransfer && type != ApzObjectTypes.oInventoryTransferRequest) ? (text2 + " AND \"CardCode\" = @CardCode") : (text2 + " AND ifnull(\"CardCode\", @CardCode) = @CardCode"));
					prms.Add(new SqlParameter("@CardCode", cardCode));
				}
				switch (type)
				{
				case ApzObjectTypes.oAPReserveInvoice:
				case ApzObjectTypes.oReserveInvoices:
					text2 += " and \"isIns\" ='Y'";
					break;
				case ApzObjectTypes.oInvoices:
				case ApzObjectTypes.oPurchaseInvoices:
					text2 += " and \"isIns\" ='N'";
					break;
				}
			}
			if (type == ApzObjectTypes.oOrders || type == ApzObjectTypes.oPurchaseOrders)
			{
				text2 += " and \"Confirmed\" = 'Y'";
			}
		}
		return text2 + " order by \"DocDate\";";
	}

	protected override string DocumentPriceQuery()
	{
		return "call apz_document_price(@CardCode,@DocDate,@ItemCode,@Quantity, @uomCode, @infoPriceList, PriceView);\r\n                    select * from $PriceView;";
	}

	protected override string GetQueryListDocByCardCode_SalesOpportunity(string tableName)
	{
		return $"SELECT \r\n                                            iFnull(\"DocEntry\",0) \"DocEntry\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"DocNum\",0) \"DocNum\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"DocDate\",cast('1900-01-01' as timestamp)) \"DocDate\"\r\n                                            ,ifnull(\"DocDueDate\",cast('1900-01-01' as timestamp)) \"DocDueDate\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"CardCode\",'') \"CardCode\"\r\n                                            ,ifnull(\"CardName\",'') \"CardName\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"Comments\",'') \"Comments\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"CntctCode\",-1) \"CntctCode\"\r\n                                            ,ifnull(\"VatSum\",0) \"VatSum\"\r\n\t\t\t\t\t\t\t\t\t\t\t, ifnull(\"DocTotal\",0) \"DocTotal\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"DiscPrcnt\",0) \"DiscPrcnt\"\r\n\t\t\t\t\t\t\t\t\t\t\t,ifnull(\"DocRate\",0) \"DocRate\"\r\n                                            FROM {tableName} WHERE 1=1 ";
	}

	protected override string GetItemOnwhQuery()
	{
		return "select ifnull(\"OnHand\",0) \"InWhs\",ifnull(\"IsCommited\",0) \"IsCommitedInWhs\",ifnull(\"OnOrder\",0) \"OnOrderInWhs\", \"AvgPrice\" from oitw where \"ItemCode\" = @itemcode and \"WhsCode\" = @whscode;";
	}

	protected override string GetTableColumnQuery()
	{
		return "select *, DATA_TYPE_NAME DATA_TYPE from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME=@tableName;";
	}

	protected override string GetDataItemOnhandQuery()
	{
		return "select ifnull(b.\"OnHand\",0) as \"OnHandItem\", ifnull(b.\"IsCommited\",0) as \"IsCommittedItem\", ifnull(b.\"OnOrder\",0) as \"OnOrderedItem\", \r\n                ifnull(c.\"OnHand\",0) as \"OnHandWhs\", ifnull(c.\"IsCommited\",0) as \"IsCommittedWhs\", ifnull(c.\"OnOrder\",0) as \"OnOrderedWhs\", c.\"AvgPrice\" \r\n                from OITM b left join OITW c on b.\"ItemCode\" = c.\"ItemCode\" and c.\"WhsCode\" = @whsCode where b.\"ItemCode\" = @itemCode;";
	}

	public override DataTable GetFirstRecord(string tableName, string fieldKey)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName))
			{
				string query = "select top 1 ROW_NUMBER() OVER (ORDER BY \"" + fieldKey + "\") RN, * FROM " + tableName + ";";
				return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFirstRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public override DataTable GetLastRecord(string tableName, string columnId)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string query = "select * from " + tableName + " where \"" + columnId + "\"=(select max(\"" + columnId + "\") from " + tableName + ");";
				return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLastRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public override DataTable GetNextRecord(string tableName, string columnId, string value)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string text = null;
				text += " CTE =";
				text = text + " select ROW_NUMBER() OVER (ORDER BY \"" + columnId + "\") RN, * FROM " + tableName + ";\r\n";
				text += " SELECT \"Next Row\".* FROM CTE \"Current Row\"";
				text += " LEFT JOIN CTE \"Next Row\" ON ";
				text += " \"Next Row\".RN = \"Current Row\".RN + 1";
				text = text + " WHERE \"Current Row\".\"" + columnId + "\" = '" + value + "';";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPreviousRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public override DataTable GetPreviousRecord(string tableName, string columnId, string value)
	{
		try
		{
			if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnId))
			{
				string text = null;
				text += " CTE =";
				text = text + " select ROW_NUMBER() OVER (ORDER BY \"" + columnId + "\") RN, * FROM " + tableName + ";\r\n";
				text += " SELECT \"Previous Row\".* FROM CTE \"Current Row\"";
				text += " LEFT JOIN CTE \"Previous Row\" ON ";
				text += " \"Previous Row\".RN = \"Current Row\".RN - 1";
				text = text + " WHERE \"Current Row\".\"" + columnId + "\" = '" + value + "';";
				return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPreviousRecord   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public override DataTable GetAllContactPerson()
	{
		try
		{
			new DataTable();
			return ExecuteDataTable("select ifnull(A.\"CntctCode\",0) as \"CntctCode\",ifnull(A.\"Cellolar\",\"Phone1\") as \"Telephone\",B.\"CardCode\",ifnull(a.\"Name\",'') as \"Name\" from OCPR A left join ocrd B on A.\"CardCode\"=B.\"CardCode\";", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ContactPerson   FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected override string ValidateDocNumByDocTypeQuery(string tableName)
	{
		return string.Format("declare checkCount int = 0;\r\n                            select ifnull((select top 1 1 from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = N'{0}' and COLUMN_NAME = N'CardCode'), 0) into checkCount from DUMMY;\r\n                            IF $checkCount = 1\r\n                            THEN\r\n                              SELECT \"DocNum\", \"DocEntry\", \"CardCode\" FROM {0} WHERE \"DocNum\" LIKE @DocNum;\r\n                            \r\n                            ELSE\r\n                            \r\n                                SELECT \"DocNum\", \"DocEntry\", '' as \"CardCode\" FROM {0} WHERE \"DocNum\" LIKE @DocNum;\r\n                            END IF;", tableName);
	}

	protected override string ExecuteSqlStandarQuery(string queryText, List<QueryParamater> paramater, ref List<SqlParameter> lstPara)
	{
		for (int i = 0; i < paramater.Count; i++)
		{
			string text = "";
			if (paramater[i].Value == null)
			{
				text = "null";
			}
			else if (!(paramater[i].Value is DateTime))
			{
				text = ((!string.IsNullOrEmpty(Function.ToString(paramater[i].Value)) || !new string[9] { "1", "2", "3", "4", "5", "6", "9", "10", "11" }.Contains(Function.ToString(paramater[i].DataType).ToLower())) ? ("'" + Function.ToString(paramater[i].Value) + "'") : "null");
			}
			else
			{
				text = "'" + Function.ParseDateTimes(paramater[i].Value).ToString("yyyy-MM-dd") + "'";
			}
			queryText = queryText.Replace("N'" + paramater[i].ParaName + "'", text);
			queryText = queryText.Replace("'" + paramater[i].ParaName + "'", text);
			queryText = queryText.Replace(paramater[i].ParaName, text);
		}
		return queryText;
	}

	protected override string GetDatabaseFollowDomain()
	{
		return "select distinct a.BASE_SCHEMA_NAME as \"Name\", b.\"HardwareKey\" from SYS.OBJECT_DEPENDENCIES a\r\n\t                    left join (select \"Domain\", \"HardwareKey\", \"Database\" from APZ_OLDD where \"Domain\" = @domain) b on b.\"Database\" = a.BASE_SCHEMA_NAME\r\n\t                    where a.BASE_SCHEMA_NAME not in ('SAP_XS_USAGE', 'SAP_REST_API', 'SAP_XS_LM', 'SAP_XS_LM_PE', 'HANA_XS_BASE', 'SYS', 'SYSTEM', 'UIS')\r\n\t                    and a.BASE_SCHEMA_NAME not like '_SYS%'\r\n\t                    and (@cloudSystem = 100 or b.\"Domain\" = @domain);";
	}

	protected override string GetCompanyInfoQuery(string name)
	{
		return string.Format("if exists(select 1 from SYS.TABLES where SCHEMA_NAME = CURRENT_SCHEMA)\r\n\t                 then\r\n\t\t                    select A.\"Code\", B.\"CompnyName\" as \"CmpName\", B.\"CompnyAddr\" as \"CmpAdress\", 1 as \"CmpType\", B.\"Phone1\" as \"CmpTel\", A.\"Version\", A.\"TextCode\"  \r\n                            from \"{0}\".APZ_CINF A, \"{0}\".OADM B;\r\n\t                end if;", name);
	}

	public override string CheckExistColumn(string table, string column)
	{
		try
		{
			string query = "SELECT TOP 1 'Y' FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = @table AND COLUMN_NAME = @column;";
			if (Function.ToString(ExecuteScalar(query, CommandType.Text, new SqlParameter("@table", table), new SqlParameter("@column", column))) == "Y")
			{
				return "Y";
			}
			return "N";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "";
		}
	}

	protected override string GetAccessLogQuery()
	{
		return "select ID, \"User\", \"LoginTime\",to_varchar(\"LoginTime\",'HH24:MI:SS') \"LoginHour\",\"LogoffTime\", to_varchar(\"LogoffTime\",'HH24:MI:SS') \"LogoffHour\", \"ClientLocalIpAddress\",\"ClientMachineName\",\"ClientWindowsLogonName\" \r\n                from apz_loglogin\r\n                where \"DataBase\" = @database\r\n                and(cast(\"LoginTime\" as date) between cast(@fromDate as date) and cast(@toDate as date)\r\n                  or cast(\"LogoffTime\" as date)  between cast(@fromDate as date) and cast(@toDate as date))\r\n                and \"User\" = @user  order by ID desc;";
	}

	protected override string GetAccessLogLastLoginQuery()
	{
		return " select y.\"User\",'' \"Name\",'' \"Branch\",'' \"Department\",y.\"LoginTime\",to_varchar(y.\"LoginTime\",'HH24:MI:SS') \"LoginHour\",y.\"LogoffTime\", to_varchar(y.\"LogoffTime\",'HH24:MI:SS') \"LogoffHour\",y.\"status\"\r\n ,y.\"ClientLocalIpAddress\",y.\"ClientMachineName\",y.\"ClientWindowsLogonName\",case when y.\"status\"='Y' then 1 else 0 end is_online \r\n from (select \"User\",Max(ID) ID from apz_loglogin  where \"DataBase\"=@database group by \"User\") x inner join apz_loglogin y on x.ID=y.ID ;";
	}
}
