

CREATE PROCEDURE apz_widget_chooseitem(in widgetId int)
AS
BEGIN

	SELECT T0."Id", T0."Name", CASE WHEN IFNULL(T1."Name", '')='' THEN 'Unassigned' ELSE T1."Name" END AS "Category", T0."Layout" AS "ImageData", '7000030' AS "ObjType",
	CASE WHEN IFNULL(T2."AbsEntry", 0) <> 0 THEN 1 ELSE 0 END Checked, CAST(T0."Id" as varchar) AS "ObjCode"
    FROM APZ_DAB1 T0 
	LEFT JOIN APZ_ODAB T1 ON T0."CatId" = T1."Id" AND T1."Valid" = 1
	LEFT JOIN APZ_WID1 T2 ON T0."Id" = T2."ObjCode" AND T2."ObjType" = '7000030' AND T2."AbsEntry" = :widgetId AND IFNULL(T2."Caption", '') <> '';

END



