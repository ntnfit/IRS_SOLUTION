

CREATE PROCEDURE APZ_Schedule_GetDataInventoryTransferFormSchedule (IN user nvarchar(25) default '', code int default 0)
AS
BEGIN
	declare zeroSap numeric(19,6);
	zeroSap = ifnull((SELECT power(10.0000000, -1 * "QtyDec") FROM OADM), 0.0000001);

	SELECT DISTINCT IFNULL(g."Series", -1) AS "Series", a."CardCode", c."CardName", a."ItemCode", b."ItemName" AS "Dscription", a."WorkDay" AS "ShipDate", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "InvQty",
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "Quantity", 
						(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "MrpOpenQty",
						cast('' as nvarchar(8)) "FromWhsCod", 
						cast('' as nvarchar(8)) "WhsCode", f."PrjCode" "Project"
						,a."Notes" AS "Remark", '800004' AS "U_OriginType", a."Line" AS "U_OriginLine", a."PSId" AS "U_OriginCode", CAST(0 AS decimal(19, 6)) AS "InStoc", CAST(0 AS decimal(19, 6)) AS "InWhs", IFNULL(g1."Series", -1) AS "SeriesRequest", IFNULL(f."BplID", -1) AS "BplID",
						case 

				when b."ManBtchNum" = 'Y' then 'B'
				when b."ManSerNum" = 'Y' then 'S'
				else 'N'

				 End "ManBtchSerial", 'Y' "UseBaseUn", cast(1 as float) "NumPerMsr",
				b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", 
				IFNULL(b."UgpEntry", -1) AS "UgpEntry", b."InvntryUom" AS "InvUnitMsr", IFNULL(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1) AS "NumInInven", b."InvntryUom" AS "unitMsr",
				case

					when ifnull(b."UgpEntry",-1) = -1 then 'manual'
					else (SELECT "UomCode" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
										   INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
										   WHERE tt1."UomName" = b."InvntryUom" AND tt3."UgpEntry" = b."UgpEntry")

				end "UomCode",
				case

					when ifnull(b."UgpEntry",-1) = -1 then -1
					else (SELECT tt1."UomEntry" FROM OUOM tt1 INNER JOIN UGP1 tt2 ON tt1."UomEntry" = tt2."UomEntry" 
												INNER JOIN OUGP tt3 ON tt2."UgpEntry" = tt3."UgpEntry" 
												WHERE tt1."UomName" = b."InvntryUom" AND tt3."UgpEntry" = b."UgpEntry")

				end "UomEntry"
				, a."DemandType", a."DemandEntry", a."DemandLine"
			FROM apz_PRS1 a 
			inner join apz_OPRS e on e."PSCode" = a."PSId"
			inner join APZ_OMRP f on f."MrpCode" = e."MrpId"
			--inner join APZ_MRPS xx ON f."ScenCode" = f."Code"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			left join OCRD c on a."CardCode" = c."CardCode"

            --Join purchase order have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM POR1 
					   WHERE IFNULL("U_OriginType", '') = '800004' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d on a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine"

			--Join Purchase request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM PRQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '800004' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM POR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h 
								on a."PSId" = h."U_OriginCode" and h."U_OriginLine" = a."Line"

			--Join inventory transfer have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTR1 
					   WHERE IFNULL("U_OriginType", '') = '800004' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d1 on a."PSId" = d1."U_OriginCode" and a."Line" = d1."U_OriginLine"

			--JOin incentory transfer request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM WTQ1 t1 
						WHERE IFNULL("U_OriginType", '') = '800004' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM WTR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h1 
								on a."PSId" = h1."U_OriginCode" and h1."U_OriginLine" = a."Line"

			--JOIN Purachase order  and purcahse request have been created
			LEFT JOIN (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(IFNULL("InvQty", 0)) AS "Quantity" FROM DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry" 
						WHERE IFNULL("U_OriginType", '') = '800004' AND t2."ObjType" IN ('22','1470000113') AND t2."WddStatus" = 'W' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h2
							on a."PSId" = h2."U_OriginCode" and h2."U_OriginLine" = a."Line"

			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '67' GROUP BY "BPLId") g on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '1250000001' GROUP BY "BPLId") g1 on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join UGP1 g3 on b."UgpEntry" = g3."UgpEntry" and g3."UomEntry" in (SELECT "UomEntry" FROM OUOM WHERE "UomName" = b."InvntryUom")
       WHERE e."PSCode" = :code and b."InvntItem" = 'Y' and "TypePlanning" = 2 and ifnull(a."Shift", '') != '' AND (ifnull(a."Qty",0) - ifnull(d."Quantity",0) - ifnull(h."Quantity",0) - ifnull(d1."Quantity",0) - ifnull(h1."Quantity",0) - ifnull(h2."Quantity",0)) > :zeroSap
		order by a."ItemCode", a."WorkDay";
END




