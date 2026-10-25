

CREATE PROCEDURE APZ_Schedule_GetDataPurchaseFormSchedule (IN user nvarchar(25) default '', code int default 0)
AS
BEGIN

	declare zeroSap numeric(19,6);
	zeroSap = ifnull((select power(10.0000000, -1 * "QtyDec") from OADM), 0.0000001);

	SELECT f."BplID", a."CardCode", c."CardName", a."ItemCode", b."ItemName" AS "Dscription", CURRENT_DATE AS "DocDate", a."WorkDay" AS "ShipDate", 
							(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "InvQty",
							(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) * (IFNULL(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1)) / IFNULL(b."NumInBuy", 1) AS "Quantity", 
							(IFNULL(a."Qty", 0) - IFNULL(d."Quantity", 0) - IFNULL(h."Quantity", 0) - IFNULL(d1."Quantity", 0) - IFNULL(h1."Quantity", 0) - IFNULL(h2."Quantity", 0)) AS "MrpOpenQty",
							case 
								when IFNULL((SELECT b1."Warehouse" FROM OUSR a1 INNER JOIN OUDG b1 ON a1."DfltsGroup" = b1."Code" WHERE a1.USER_CODE = :user), '') <> ''
									then (SELECT b."Warehouse" FROM OUSR a INNER JOIN OUDG b ON a."DfltsGroup" = b."Code" WHERE USER_CODE = :user)
								when  IFNULL((SELECT "DfltWH" FROM OBPL WHERE "BPLId" = f."BplID"), '') <> ''
									then (SELECT "DfltWH" FROM OBPL WHERE "BPLId" = f."BplID")
								else b."DfltWH"
							end  "WhsCode", 
						case when t1."Price" is not null then t1."Price" when t."Price" is not null then t."Price" else ifnull(t2."Price",0) END "Price",
						case when t1."Price" is not null then t1."Price" when t."Price" is not null then t."Price" else ifnull(t2."Price",0) END "PriceBefDi",
						case when t1."Price" is not null then t1."Currency" when t."Price" is not null then t."Currency" else t2."Currency" END "Currency",
						case when t1."Price" is not null then t1."Discount" when t."Price" is not null then t."Discount" else 0 END "DiscPrcnt",
						a."Notes" as "Remark",'800004' "U_OriginType", a."Line" as "U_OriginLine", a."PSId" as "U_OriginCode", ifnull(g."Series",-1) as "Series", ifnull(g1."Series",-1) as "SeriesRequest",
						g2."Substitute" "SubCatNum",
						case when b."ManBtchNum" = 'Y' then 'B' when b."ManSerNum" = 'Y' then 'S'else 'N' End "ManBtchSerial", 'N' "UseBaseUn", ifnull(b."NumInBuy",1) "NumPerMsr",
				b."SalUnitMsr" as "SalUnitMsr", ifnull(b."NumInSale",1) as "NumInSale", b."BuyUnitMsr",ifnull(b."NumInBuy",1) "NumInBuy", 
				ifnull(b."UgpEntry",-1) as "UgpEntry", b."InvntryUom" as "InvUnitMsr", ifnull(g3."AltQty", 1) / ifnull(g3."BaseQty", 1) as "NumInInven", b."BuyUnitMsr" "unitMsr",
				case

					when ifnull(b."UgpEntry",-1) = -1 then 'manual'
					else (select "UomCode" from OUOM tt1 inner join UGP1 tt2 on tt1."UomEntry" = tt2."UomEntry"
														inner join OUGP tt3 on tt2."UgpEntry" = tt3."UgpEntry"
														where tt1."UomName" = b."BuyUnitMsr" and tt3."UgpEntry" = b."UgpEntry")

				end "UomCode",
				case

					when ifnull(b."UgpEntry",-1) = -1 then -1
					else (select tt1."UomEntry" from OUOM tt1 inner join UGP1 tt2 on tt1."UomEntry" = tt2."UomEntry"
														inner join OUGP tt3 on tt2."UgpEntry" = tt3."UgpEntry"
														where tt1."UomName" = b."BuyUnitMsr" and tt3."UgpEntry" = b."UgpEntry")

				end "UomEntry", -1 "SlpCode", b."VatGroupPu" "VatGroup", t3."Rate" "VatPrcnt", f."PrjCode" "Project"
				, a."DemandType", a."DemandEntry", a."DemandLine", add_days(a."WorkDay", ifnull(a."LTime",0)) "ReqDate"
			FROM apz_PRS1 a 
			inner join apz_OPRS e on e."PSCode" = a."PSId"
			inner join APZ_OMRP f on f."MrpCode" = e."MrpId"
			--INNER JOIN APZ_MRPS xx ON xx."Code" = f."ScenCode"
			INNER JOIN OITM b ON a."ItemCode"= b."ItemCode"
			left join OCRD c on a."CardCode" = c."CardCode"

			--JOIN purcahse request have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM POR1 
						WHERE ifnull("U_OriginType", '') = '800004' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d on a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine"

			--JOIN Purachase Order have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM PRQ1 t1 
						WHERE ifnull("U_OriginType", '') = '800004' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN (SELECT "BaseEntry" FROM POR1 WHERE "ItemCode" = t1."ItemCode"))) 
						GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h on a."PSId" = h."U_OriginCode" and h."U_OriginLine" = a."Line"

			--JOIN Inventory tranfer have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM WTR1 
						WHERE ifnull("U_OriginType", '') = '800004' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") d1 on a."PSId" = d1."U_OriginCode" and a."Line" = d1."U_OriginLine"

			--JOIN inventory tranfer repuest have been created
			left join (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM WTQ1 t1
								where ifnull("U_OriginType",'') = '800004' and ("LineStatus" = 'O' or ("LineStatus" = 'C' and "DocEntry" in 
								(select "BaseEntry" from WTR1 where "ItemCode" = t1."ItemCode"))) Group by "ItemCode","U_OriginCode","U_OriginLine") h1 on a."PSId" = h1."U_OriginCode" and h1."U_OriginLine" = a."Line"

			--JOIN Purachase order  and purcahse request have been created
			LEFT JOIN (SELECT "ItemCode", "U_OriginCode", "U_OriginLine", SUM(ifnull("InvQty", 0)) AS "Quantity" FROM DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry" 
						WHERE ifnull("U_OriginType", '') = '800004' AND t2."ObjType" IN ('22','1470000113') AND t2."WddStatus" = 'W' GROUP BY "ItemCode", "U_OriginCode", "U_OriginLine") h2 on a."PSId" = h2."U_OriginCode" and h2."U_OriginLine" = a."Line"

			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '22' GROUP BY "BPLId") g on ifnull(g."BPLId",-1) = ifnull(f."BplID",-1)
			left join (SELECT MAX("Series") AS "Series", "BPLId" FROM NNM1 WHERE "ObjectCode" = '1470000113' GROUP BY "BPLId") g1 on ifnull(g1."BPLId",-1) = ifnull(f."BplID",-1)
			LEFT JOIN OSCN g2 ON c."CardCode" = g2."CardCode" and g2."ItemCode" = a."ItemCode"
			left join UGP1 g3 on b."UgpEntry" = g3."UgpEntry" and g3."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")
			Left Join OSPP t ON t."CardCode" = c."CardCode" and t."ItemCode"= b."ItemCode"
			left join SPP1 t1 on t1."CardCode" = c."CardCode" and t1."ItemCode" = b."ItemCode" and a."WorkDay">= t1."FromDate" and (t1."ToDate" = null or t1."ToDate" = '' or a."WorkDay" <= t1."ToDate")
			Left JOIN ITM1 t2 ON t2."PriceList"=c."ListNum" and t2."ItemCode" = a."ItemCode"
			Left JOin OVTG t3 ON ifnull(b."VatGroupPu",'') = t3."Code"
       WHERE e."PSCode" = :code and b."PrchseItem" = 'Y' and "TypePlanning" = 2 and ifnull(a."Shift", '') != '' AND (ifnull(a."Qty",0) - ifnull(d."Quantity",0) - ifnull(h."Quantity",0) - ifnull(d1."Quantity",0) - ifnull(h1."Quantity",0) - ifnull(h2."Quantity",0)) > :zeroSap
		order by a."ItemCode", add_days(current_date, a."LTime");
END




