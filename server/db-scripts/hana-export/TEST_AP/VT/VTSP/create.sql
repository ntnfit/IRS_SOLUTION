CREATE VIEW "TEST_AP"."VTSP" ( "AbsEntry", "TransCode", "TransName", "TransID", "LineNum", "TransMode", "VehicleTyp", "VehicleNo" ) AS SELECT T0."AbsEntry", T0."TransCode", T0."TransName", T0."TransID", IFNULL(T1."LineNum", 0) as "LineNum", T2."ModeName" as "TransMode", T3."TypeName" as "VehicleTyp", T1."VehicleNo"
		FROM "OTSP" T0 
		LEFT OUTER JOIN "TSP1" T1 ON T1."AbsEntry" = T0."AbsEntry"
		LEFT OUTER JOIN "OETM" t2 ON T2."ModeCode" = t1."TransMode"
		LEFT OUTER JOIN "OEVT" t3 ON T3."TypeCode" = t1."VehicleTyp" WITH READ ONLY