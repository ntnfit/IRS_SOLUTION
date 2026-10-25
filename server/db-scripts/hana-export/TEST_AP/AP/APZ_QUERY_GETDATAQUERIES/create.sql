
CREATE PROCEDURE APZ_QUERY_GetDataQueries
as
BEGIN		
		SELECT a."IntrnalKey", b."CategoryId" AS "QCategory", b."CatName", CASE WHEN a."QName" IS NULL THEN 'No queries belong to this group'
														   ELSE a."QName"
													  END AS "QName", CASE WHEN a."QString" IS NULL THEN cast('' as nvarchar)
																		 ELSE cast(a."QString" as nvarchar)
																	END AS "QString", a."QType", b."PermMask"
			FROM OUQR a
			RIGHT JOIN OQCN b ON a."QCategory" = b."CategoryId"
			--WHERE CHARINDEX('Y', "PermMask") != 0
			ORDER BY b."CategoryId", a."QName";	
END; 




