

CREATE PROCEDURE APZ_Schedule_GetDataInventoryTransferFormMrp (IN user nvarchar(25) default '', code int default 0)
AS
BEGIN
	declare zeroSap numeric(19,6);
	zeroSap = ifnull((select power(10.0000000, -1 * "QtyDec") from OADM), 0.0000001);

	SELECT DISTINCT IFNULL(g."Series", -1) AS "Series", a."CardCode", c."CardName", a."ItemCode", b."ItemName" AS "Dscription", a."DueDate" AS "ShipDate", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "InvQty", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "Quantity", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "MrpOpenQty",
						cast('' as nvarchar(8)) "FromWhsCod", 
						cast('' as nvarchar(8)) "WhsCode"
						,a."Notes" AS "Remark", '700003' AS "U_OriginType", a."LineNum" AS "U_OriginLine", a."OMrpId" AS "U_OriginCode", CAST(0 AS decimal(19, 6)) AS "InStoc", CAST(0 AS decimal(19, 6)) AS "InWhs", IFNULL(g1."Series", -1) AS "SeriesRequest", IFNULL(f."BplID", -1) AS "BplID",
						case 

				when b."ManBtchNum" = 'Y' then 'B'
				when b."ManSerNum" = 'Y' then 'S'
				else 'N'

				 End "ManBtchSerial", 'Y' "UseBaseUn", cast(1 as float) "NumPerMsr",
				b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", 
				IFNULL(b."UgpEntry", -1) AS "UgpEntry", b."InvntryUom" AS "InvUnitMsr", IFNULL(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1) AS "NumInInven", b."InvntryUom" AS "unitMsr",
				CASE WHEN IFNULL(b."UgpEntry", -1) = -1 THEN 'manual' 
				ELSE (SELECT "UomCode" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
					 INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
					 WHERE tt1."UomName" = b."InvntryUom" AND tt3."UgpEntry" = b."UgpEntry") 
				END AS "UomCode",
				CASE WHEN IFNULL(b."UgpEntry", -1) = -1 THEN -1 
				ELSE (SELECT tt1."UomEntry" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
					 INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
					 WHERE tt1."UomName" = b."InvntryUom" AND tt3."UgpEntry" = b."UgpEntry") 
				END AS "UomEntry"
				, a."DemandType", a."DemandEntry", a."DemandLine"
			FROM APZ_MRP1 a 
			inner join APZ_OMRP f on f."MrpCode" = a."OMrpId"
			--INNER JOIN APZ_MRPS xx ON f."ScenCode" = xx."Code"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			left join OCRD c on a."CardCode" = c."CardCode"

			--Join purchase order have been created
            left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM POR1 
            			WHERE IFNULL("U_OriginType", '') = '700003' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d on a."OMrpId" = d."U_OriginCode" and a."LineNum" = d."U_OriginLine"

			--Join Purchase request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM PRQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '700003' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM POR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h 
								on a."OMrpId" = h."U_OriginCode" and h."U_OriginLine" = a."LineNum"

			--Join inventory transfer have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTR1 
						WHERE IFNULL("U_OriginType", '') = '700003' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d1 on a."OMrpId" = d1."U_OriginCode" and a."LineNum" = d1."U_OriginLine"

			--JOin incentory transfer request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '700003' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM WTR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h1 on a."OMrpId" = h1."U_OriginCode" and h1."U_OriginLine" = a."LineNum"

			--JOIN Purachase order  and purcahse request have been created
			LEFT JOIN (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry" 
						WHERE ifnull("U_OriginType", '') = '700003' AND t2."ObjType" IN ('22','1470000113') AND t2."WddStatus" = 'W' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h2
							on a."OMrpId" = h2."U_OriginCode" and h2."U_OriginLine" = a."LineNum"

			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '67' GROUP BY "BPLId") g on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '1250000001' GROUP BY "BPLId") g1 on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join ORDR gg on comparesplit(gg."DocNum", a.SOID, ',') = 1
			left join UGP1 g3 on b."UgpEntry" = g3."UgpEntry" and g3."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")
       WHERE a."OMrpId" = :code and b."InvntItem" = 'Y' and a."Type" between 5100000 and 5999999 and (ifnull(a."Qty",0) - ifnull(d."Quantity",0) - ifnull(h."Quantity",0) - ifnull(d1."Quantity",0) - ifnull(h1."Quantity",0) - ifnull(h2."Quantity",0)) > :zeroSap and a."DocumentType" = 'B'
		order by a."ItemCode", a."DueDate";
END




