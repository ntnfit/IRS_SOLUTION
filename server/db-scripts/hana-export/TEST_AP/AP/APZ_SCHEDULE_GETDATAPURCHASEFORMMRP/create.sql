

CREATE PROCEDURE APZ_Schedule_GetDataPurchaseFormMrp (IN user nvarchar(25) default '', code int default 0)
AS
BEGIN
	declare zeroSap numeric(19,6);
	zeroSap = ifnull((select power(10.0000000, -1 * "QtyDec") from OADM), 0.0000001);

	SELECT CAST(gg."DocEntry" AS nvarchar) AS "U_SoDocNo", a."CardCode", c."CardName", a."ItemCode", b."ItemName" AS "Dscription", CURRENT_DATE AS "DocDate", a."DueDate" AS "ShipDate", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "InvQty",
							(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) * (IFNULL(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1)) / IFNULL(b."NumInBuy", 1) AS "Quantity", 
							 (IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "MrpOpenQty",
							case 
								when ifnull((SELECT b1."Warehouse" FROM OUSR a1 INNER JOIN OUDG b1 ON a1."DfltsGroup" = b1."Code" WHERE a1.USER_CODE = :user),'') <> ''
									then (SELECT b."Warehouse" FROM OUSR a INNER JOIN OUDG b ON a."DfltsGroup" = b."Code" WHERE USER_CODE = :user)
								when ifnull((SELECT "DfltWH" FROM OBPL WHERE "BPLId" = f."BplID"),'') <> ''
									then (SELECT "DfltWH" FROM OBPL WHERE "BPLId" = f."BplID")
								else b."DfltWH"
							end  "WhsCode", 
						CASE WHEN t1."Price" IS NOT NULL THEN t1."Price" WHEN t."Price" IS NOT NULL THEN t."Price" ELSE IFNULL(t2."Price", 0) END AS "Price",
						CASE WHEN t1."Price" IS NOT NULL THEN t1."Price" WHEN t."Price" IS NOT NULL THEN t."Price" ELSE IFNULL(t2."Price", 0) END AS "PriceBefDi",
						CASE WHEN t1."Price" IS NOT NULL THEN t1."Currency" WHEN t."Price" IS NOT NULL THEN t."Currency" ELSE t2."Currency" END AS "Currency", 
						CASE WHEN t1."Price" IS NOT NULL THEN t1."Discount" WHEN t."Price" IS NOT NULL THEN t."Discount" ELSE 0 END AS "DiscPrcnt" 
						, a."Notes" AS "Remark", '700003' AS "U_OriginType", a."LineNum" AS "U_OriginLine", a."OMrpId" AS "U_OriginCode", IFNULL(g."Series", -1) AS "Series", IFNULL(g1."Series", -1) AS "SeriesRequest", f."BplID",
						g2."Substitute" "SubCatNum",
						CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", 'N' AS "UseBaseUn", IFNULL(b."NumInBuy", 1) AS "NumPerMsr",
				b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 1) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 1) AS "NumInBuy", 
				IFNULL(b."UgpEntry", -1) AS "UgpEntry", b."InvntryUom" AS "InvUnitMsr", IFNULL(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1) AS "NumInInven", b."BuyUnitMsr" AS "unitMsr",
				CASE WHEN IFNULL(b."UgpEntry", -1) = -1 THEN 'manual' 
				ELSE (SELECT "UomCode" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
								INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
								WHERE tt1."UomName" = b."InvntryUom" AND tt3."UgpEntry" = b."UgpEntry") 
				END AS "UomCode",
				CASE WHEN IFNULL(b."UgpEntry", -1) = -1 THEN -1 
				ELSE (SELECT tt1."UomEntry" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
							INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
							WHERE tt1."UomName" = b."BuyUnitMsr" AND tt3."UgpEntry" = b."UgpEntry") 
				END AS "UomEntry",
				 -1 AS "SlpCode", b."VatGroupPu" AS "VatGroup", t3."Rate" AS "VatPrcnt", f."PrjCode" AS "Project", 
				 a."DemandType", a."DemandEntry", a."DemandLine", ADD_DAYS(a."DueDate", IFNULL(a."LTime", 0)) AS "ReqDate"
			FROM APZ_MRP1 a 
			inner join APZ_OMRP f on f."MrpCode" = a."OMrpId"
			--INNER JOIN APZ_MRPS xx ON xx."Code" = f."ScenCode"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
			left join OCRD c on a."CardCode" = c."CardCode"

			--JOIN Purcahse order have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM POR1 
						WHERE ifnull("U_OriginType", '') = '700003' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d on a."OMrpId" = d."U_OriginCode" and a."LineNum" = d."U_OriginLine"

			--JOIN Purcahse request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM PRQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '700003' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM POR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h 
								on a."OMrpId" = h."U_OriginCode" and h."U_OriginLine" = a."LineNum"

			--Join inventory tranfer have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTR1 
						WHERE IFNULL("U_OriginType", '') = '700003' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d1 on a."OMrpId" = d1."U_OriginCode" and a."LineNum" = d1."U_OriginLine"

			--Join inventory tranfer have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '700003' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM WTR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h1 on a."OMrpId" = h1."U_OriginCode" and h1."U_OriginLine" = a."LineNum"

			--JOIN Purachase order  and purcahse request have been created
			LEFT JOIN (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry" 
						WHERE IFNULL("U_OriginType", '') = '700003' AND t2."ObjType" IN ('22','1470000113') AND t2."WddStatus" = 'W' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h2
							on a."OMrpId" = h2."U_OriginCode" and h2."U_OriginLine" = a."LineNum"

			left join (Select max("Series") "Series", "BPLId" from NNM1 where "ObjectCode" = '22' group by "BPLId") g on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join (Select max("Series") "Series", "BPLId" from NNM1 where "ObjectCode" = '1470000113' group by "BPLId") g1 on ifnull(g1."BPLId",-1) = ifnull(f."BplID",-1)
			LEFT JOIN OSCN g2 ON c."CardCode" = g2."CardCode" and g2."ItemCode" = a."ItemCode"
			left join UGP1 g3 on b."UgpEntry" = g3."UgpEntry" and g3."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")
			left join ORDR gg on comparesplit(cast(gg."DocNum" as nvarchar), a.SOID, ',') = 1
			Left Join OSPP t ON t."CardCode" = c."CardCode" and t."ItemCode" = b."ItemCode"
			left join SPP1 t1 on t1."CardCode" = c."CardCode" and t1."ItemCode" = b."ItemCode" and a."DueDate" >= t1."FromDate" and (t1."ToDate" = null or t1."ToDate" = '' or a."DueDate" <= t1."ToDate")
			Left JOIN ITM1 t2 ON t2."PriceList"=c."ListNum" and t2."ItemCode" = a."ItemCode"
			Left JOin OVTG t3 ON ifnull(b."VatGroupPu",'') = t3."Code"
       WHERE a."OMrpId" = :code and b."PrchseItem" = 'Y' and a."Type" between 5100000 and 5999999 and (ifnull(a."Qty",0) - ifnull(d."Quantity",0) - ifnull(h."Quantity",0) - ifnull(d1."Quantity",0) - ifnull(h1."Quantity",0) - ifnull(h2."Quantity",0)) > :zeroSap and a."DocumentType" = 'B'
		order by a."ItemCode", a."DueDate";
END



