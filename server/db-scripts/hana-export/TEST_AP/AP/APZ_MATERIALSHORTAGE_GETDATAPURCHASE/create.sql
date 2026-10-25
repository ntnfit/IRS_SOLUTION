
create procedure APZ_MaterialShortage_GetDataPurchase
(
in code int, 
in user nvarchar(50), 
in input char(1) default ''
)

as 
begin
	IF :input = 'M' then
		select DISTINCT e."BplId", b."CardCode" "CardCode", g4."CardName" "CardName" ,a."SubCode" "ItemCode", b."ItemName" "Dscription", Po."DueDate" "ShipDate", 
							(Ifnull(a."Qty",0) - IfNULL(d."Quantity",0) - IfNULL(h."Quantity",0) - IFNULL(h2."Quantity",0)) "InvQty",
							(IFnull(a."Qty",0) - IFNULL(d."Quantity",0) - IfNULL(h."Quantity",0) - IFNULL(h2."Quantity",0)) / IFnull(b."NumInBuy",1) as "Quantity", 
							(IFnull(a."Qty",0) - IFNULL(d."Quantity",0) - IFNULL(h."Quantity",0) - IFNULL(h2."Quantity",0)) / IFnull(b."NumInBuy",1) as "MrpOpenQty",
							case 
								when ifnull((Select b1."Warehouse" from OUSR a1 inner join OUDG b1 on a1."DfltsGroup" = b1."Code" where a1.USER_CODE = :user),'') <> ''
									then (Select b."Warehouse" from OUSR a inner join OUDG b on a."DfltsGroup" = b."Code" where USER_CODE = :user)
								when ifnull((select "DfltWH" from OBPL where "BPLId" = e."BplId"),'') <> ''
									then (select "DfltWH" from OBPL where "BPLId" = e."BplId")
								else b."DfltWH"
							end  "WhsCode", 
						cast(0 as numeric(19,6)) "Price",
						cast(0 as numeric(19,6)) "PriceBefDi",
						'' "Currency",
						cast(0 as numeric(19,6)) "DiscPrcnt",
						e."Remarks" as "Remark", '700016' "U_OriginType", a."LineId" as "U_OriginLine", a."Code" as "U_OriginCode", ifnull(g."Series",-1) as "Series", ifnull(g1."Series",-1) as "SeriesRequest",
						g5."Substitute" "SubCatNum",
						case when b."ManBtchNum" = 'Y' then 'B' when b."ManSerNum" = 'Y' then 'S'else 'N' End "ManBtchSerial", 'N' "UseBaseUn", ifnull(b."NumInBuy",0) "NumPerMsr",
					b."SalUnitMsr" as "SalUnitMsr", ifnull(b."NumInSale",0) as "NumInSale", b."BuyUnitMsr",Ifnull(b."NumInBuy",0) "NumInBuy", 
					ifnull(b."UgpEntry",-1) as "UomEntry", b."InvntryUom" as "InvUnitMsr", ifnull(g3."AltQty", 1) / IFNULL(g3."BaseQty", 1) as "NumInInven", b."BuyUnitMsr" "unitMsr",
					case

						when ifnull(b."UgpEntry",-1) = -1 then 'manual'
						else (select "UomCode" from OUOM tt1 inner join UGP1 tt2 on tt1."UomEntry" = tt2."UomEntry"
															inner join OUGP tt3 on tt2."UgpEntry" = tt3."UgpEntry"
															where tt1."UomName" = b."BuyUnitMsr" and tt3."UgpEntry" = b."UgpEntry")

					end "UomCode", b."VatGroupPu" "TaxCode", -1 "SlpCode"
				FROM APZ_MSR1 a 
				inner join apz_OMSR e on e."Code" = a."Code"
				INNER JOIN OITM b ON a."SubCode" = b."ItemCode" 
				INNER JOIN OWOR Po ON a.WOID = Po."DocEntry"

				--Join purchase have been created
				left join (Select "ItemCode","U_OriginCode","U_OriginLine", SUM(ifnull("InvQty",0)) "Quantity" from POR1 
									Where ifnull("U_OriginType",'') = '700016' GROUP BY "ItemCode","U_OriginCode","U_OriginLine") d on a."Code" = d."U_OriginCode" and a."LineId" = d."U_OriginLine"
	
				--Join purchase order have been created
				left join (Select "ItemCode","U_OriginCode","U_OriginLine", SUM(ifnull("InvQty",0)) "Quantity" from PRQ1 t1
									where ifnull("U_OriginType",'') = '700016' and ("LineStatus" = 'O' or ("LineStatus" = 'C' and "DocEntry" in (select "BaseEntry" from POR1 where "ItemCode" = t1."ItemCode"))) 
									Group by "ItemCode","U_OriginCode","U_OriginLine") h on a."Code" = h."U_OriginCode" and h."U_OriginLine" = a."LineId"

				--JOIN Purachase order  and purcahse request have been created
				LEFT JOIN (Select "ItemCode","U_OriginCode","U_OriginLine", SUM(ifnull("InvQty",0)) "Quantity" from DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry"
								where ifnull("U_OriginType",'') = '700016' and t2."ObjType" in ('22', '1470000113') and t2."WddStatus" = 'W' Group by "ItemCode","U_OriginCode","U_OriginLine") h2
							on a."Code" = h2."U_OriginCode" and h2."U_OriginLine" = a."LineId"

				left join (Select max("Series") "Series", "BPLId" from NNM1 where "ObjectCode" = '22' group by "BPLId") g on IFNULL(g."BPLId",-1) = ifnull(e."BplId",-1)
				left join (Select max("Series") "Series", "BPLId" from NNM1 where "ObjectCode" = '1470000113' group by "BPLId") g1 on IFNULL(g1."BPLId",-1) = ifnull(e."BplId",-1)
				left join UGP1 g3 on b."UgpEntry" = g3."UgpEntry" and g3."UomEntry" in (select "UomEntry" from OUOM where "UomName" = b."InvntryUom")
				left join OCRD g4 on b."CardCode" = g4."CardCode"
				left join OSCN g5 on a."PrdNo" = g5."ItemCode"
		   WHERE e."Code" = :code and a."Note" = 'Shortage' AND (Ifnull(a."Qty",0) - IFNULL(d."Quantity",0) - IFNULL(h."Quantity",0) - IFNULL(h2."Quantity",0)) > 0.00005
			order by a."SubCode";
	end if;
end;




