CREATE VIEW "TEST_AP"."VBIN_Qty" ( "BinAbs", "ItemQty", "ItemsNum", "SnBNum", "ItemWeight" ) AS SELECT T0."BinAbs", SUM(T0."OnHandQty") AS "ItemQty", COUNT(DISTINCT T0."ItemCode") AS "ItemsNum", 
		COUNT(DISTINCT T0."ItemCode") AS "SnBNum", COUNT(DISTINCT T0."ItemCode") AS "ItemWeight" 
		FROM "OIBQ" T0 
		WHERE T0."OnHandQty" <> (0 ) 
		GROUP BY T0."BinAbs" WITH READ ONLY