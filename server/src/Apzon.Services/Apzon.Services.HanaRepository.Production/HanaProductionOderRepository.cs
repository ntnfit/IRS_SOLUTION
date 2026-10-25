using Apzon.Commons;
using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaProductionOderRepository : ProductionOderRepository
{
	public HanaProductionOderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxDocEntryQuery()
	{
		return "SELECT ifnull( MAX(\"DocEntry\"),0)+1 as \"DocEntry\", ifnull(MAX(\"DocNum\"),0)+1 as  \"DocNum\" FROM OWOR;";
	}

	protected override string SearchQuery()
	{
		return "a.\"{0}\" LIKE REPLACE(@{0}, '*', '%') ";
	}

	protected override string ShowUsersQuery()
	{
		return "SELECT USERID,U_NAME,USER_CODE FROM OUSR WHERE \"Locked\"='N';";
	}

	protected override string GetOITWQuantityQuery()
	{
		return "SELECT \"IsCommited\", \"OnOrder\", \"OnHand\", (\"OnHand\" + \"OnOrder\" - \"IsCommited\") AS \"Available\" FROM  OITW  \"ItemCode\"=code and \"WhsCode\"=WhsCode;";
	}

	protected override string GetItemNameUoMQuery()
	{
		return "SELECT T1.\"ItemName\", T1.\"InvntryUom\", T1.\"DfltWH\" AS \"WhsCode\" FROM OITT T0 INNER JOIN OITM T1 ON T0.\"Code\" = T1.\"ItemCode\" AND T0.\"Code\" = ItemCode;";
	}

	protected override string ValidateDocDataQuery()
	{
		return "SELECT \"DocEntry\" FROM ORDR WHERE \"DocNum\" = SONum;\r\n            SELECT \"CardCode\" FROM OCRD WHERE \"CardCode\" = CardCode;\r\n            SELECT \"PrjCode\" FROM oprj WHERE \"PrjCode\" = Project AND \"Locked\" = 'N';\r\n            ";
	}

	protected override string ExistsQuery()
	{
		return "SELECT 1 FROM OWOR WHERE \"DocEntry\"=DocEntry;";
	}

	protected override string CheckLineQuery()
	{
		return "SELECT 1 FROM WOR1 WHERE \"DocEntry\"=DocEntry and \"LineNum\"=LineNum;";
	}

	protected override string GetSalesOrderEntryQuery()
	{
		return "Select \"DocEntry\" From ORDR where \"DocNum\"=DocNum;";
	}

	protected override string AddNewQuery()
	{
		return "DELETE FROM OWOR WHERE \"DocEntry\" = DocEntry;\r\n                    DELETE FROM WOR1 WHERE \"DocEntry\" = DocEntry;\r\n                                INSERT INTO OWOR(SELECT * FROM #APZ_DOC );\r\n                    INSERT INTO WOR1(SELECT * FROM #APZ_DOCLINE  WHERE \"ItemCode\" <> '');\r\n                    UPDATE onnm SET \"AutoKey\" = IFNULL((SELECT MAX(\"DocEntry\") FROM owor), 0) + 1 WHERE \"ObjectCode\" = '202';\r\n                    UPDATE nnm1 SET \"NextNumber\" = DocNum + 1 WHERE \"ObjectCode\" = '202' AND \"Series\" = Series; ";
	}

	protected override string BatchAndSerialNumberProcessQuery()
	{
		return "Select top 0 ItemCode, DistNumber, MnfSerial, LotNumber, 0 BaseLineNumber, 0 LogQty, ObjType, AbsEntry, SysNumber INTO #APZ_Batch_Detail\r\n                                        from OBTN\r\n                                    Select top 0 * INTO #APZ_Doc_Batch from OBTN ";
	}

	protected override string GetByKeyQuery()
	{
		return "SELECT * FROM OWOR WHERE \"DocEntry\"=DocEntry SELECT * FROM WOR1 WHERE \"DocEntry\"=DocEntry";
	}

	protected override string GetByBaseEntryQuery(ApzObjectTypes type)
	{
		return string.Concat("SELECT T0.\"ItemCode\", T0.\"DocNum\", -1 AS \"BaseLine\", T0.\"PlannedQty\", T0.\"CmpltQty\", T0.\"Warehouse\", b.\"ItemName\", IFNULL(T2.\"BPLId\", -1) AS \"BplId\", b.\"UgpEntry\", b.\"InvntryUom\", \r\n                                CASE WHEN b.\"ManBtchNum\" = 'Y' THEN 'B' WHEN b.\"ManSerNum\" = 'Y' THEN 'S' ELSE 'N' END AS \"ManBtchSerial\", b.\"OnHand\" AS \"QuantityOnStock\", c.\"OnHand\" AS \"QuantityOnWareHouse\", \r\n                                b.\"SalUnitMsr\" AS \"SalUnitMsr\", IFNULL(b.\"NumInSale\", 0) AS \"NumInSale\", b.\"BuyUnitMsr\", IFNULL(b.\"NumInBuy\", 0) AS \"NumInBuy\", IFNULL(b.\"UgpEntry\", -1) AS \"UgpCode\", b.\"InvntryUom\" AS \"InvUnitMsr\", \r\n                                IFNULL(d.\"AltQty\", 1) / IFNULL(d.\"BaseQty\", 1) AS \"NumInInven\", T0.\"Warehouse\" AS \"WhsCode\", T0.\"Project\", T0.\"OcrCode\", T0.\"OcrCode2\", T0.\"OcrCode3\", T0.\"OcrCode4\", T0.\"OcrCode5\",\r\n                                c.\"AvgPrice\"\r\n\t                        FROM OWOR T0 \r\n\t                        INNER JOIN OITM b ON T0.\"ItemCode\" = b.\"ItemCode\" \r\n\t                        INNER JOIN OITW c ON T0.\"ItemCode\" = c.\"ItemCode\" AND T0.\"Warehouse\" = c.\"WhsCode\" \r\n\t                        LEFT JOIN UGP1 d ON b.\"UgpEntry\" = d.\"UgpEntry\" AND d.\"UomEntry\"=b.\"IUoMEntry\"\r\n\t                        LEFT JOIN NNM1 T2 ON T0.\"Series\" = T2.\"Series\" AND T2.\"ObjectCode\" = '202'\r\n                            WHERE T0.\"DocEntry\"= @DocEntry" + type switch
		{
			ApzObjectTypes.oInventoryGenExit => " AND \"Type\" ='D'", 
			ApzObjectTypes.oInventoryGenEntry => " AND \"Type\" IN ('S','P')", 
			_ => " ", 
		} + " UNION \r\n                        SELECT T0.\"ItemCode\", T1.\"DocNum\", T0.\"LineNum\", -T0.\"PlannedQty\", -T0.\"IssuedQty\", T0.\"wareHouse\", b.\"ItemName\", -1, b.\"UgpEntry\", b.\"InvntryUom\", \r\n                                CASE WHEN b.\"ManBtchNum\" = 'Y' THEN 'B' WHEN b.\"ManSerNum\" = 'Y' THEN 'S' ELSE 'N' END AS \"ManBtchSerial\", b.\"OnHand\" AS \"QuantityOnStock\", c.\"OnHand\" AS \"QuantityOnWareHouse\", \r\n                                b.\"SalUnitMsr\" AS \"SalUnitMsr\", IFNULL(b.\"NumInSale\", 0) AS \"NumInSale\", b.\"BuyUnitMsr\", IFNULL(b.\"NumInBuy\", 0) AS \"NumInBuy\", IFNULL(b.\"UgpEntry\", -1) AS \"UgpCode\", b.\"InvntryUom\" AS \"InvUnitMsr\", \r\n                                IFNULL(d.\"AltQty\", 1) / IFNULL(d.\"BaseQty\", 1) AS \"NumInInven\", T0.\"wareHouse\" AS \"WhsCode\", T0.\"Project\", T0.\"OcrCode\", T0.\"OcrCode2\", T0.\"OcrCode3\", T0.\"OcrCode4\", T0.\"OcrCode5\",\r\n                                c.\"AvgPrice\"\r\n\t                        FROM WOR1 T0 \r\n\t                        INNER JOIN OWOR T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" \r\n\t                        INNER JOIN OITM b ON T0.\"ItemCode\" = b.\"ItemCode\" \r\n\t                        INNER JOIN OITW c ON T0.\"ItemCode\" = c.\"ItemCode\" AND T0.\"wareHouse\" = c.\"WhsCode\" \r\n\t                        LEFT JOIN UGP1 d ON b.\"UgpEntry\" = d.\"UgpEntry\" AND d.\"UomEntry\"=b.\"IUoMEntry\"\r\n                            WHERE T1.\"DocEntry\" = @DocEntry AND T0.\"IssueType\" = 'M' " + type switch
		{
			ApzObjectTypes.oInventoryGenEntry => " AND (T1.\"Type\" ='D' OR T0.\"PlannedQty\" < 0)", 
			ApzObjectTypes.oInventoryGenExit => " AND (T1.\"Type\" IN('S','P') AND T0.\"PlannedQty\" > 0)", 
			_ => "", 
		}, ";");
	}
}
