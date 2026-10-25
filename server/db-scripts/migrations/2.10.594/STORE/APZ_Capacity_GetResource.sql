
CALL APZ_EXISTSPROC('APZ_Capacity_GetResource');

GO

CREATE PROCEDURE APZ_Capacity_GetResource
AS 
BEGIN
	DECLARE fromDate TIMESTAMP;
	DECLARE toDate TIMESTAMP;
	
	SELECT ADD_DAYS(CURRENT_TIMESTAMP,-1) Into fromDate From Dummy;
	SELECT ADD_DAYS(CURRENT_TIMESTAMP,1) Into toDate From Dummy;
	
   	SELECT * 
   	FROM ORSC a 
  	WHERE ((A."validFor" = 'Y' AND IFNULL(A."frozenFor", 'N') = 'N' AND (IFNULL(a."validFrom", :fromDate) <= CURRENT_TIMESTAMP AND IFNULL(a."validTo", :toDate) >= CURRENT_TIMESTAMP)) 
	OR (IFNULL(A."validFor", 'N') = 'N' AND A."frozenFor" = 'Y' AND (IFNULL(a."frozenFrom",:fromDate) > CURRENT_TIMESTAMP OR IFNULL(a."frozenTo",:fromDate) < CURRENT_TIMESTAMP)) 
   		OR (A."validFor" = 'Y' AND A."frozenFor" = 'Y' AND (IFNULL(a."validFrom", :fromDate) <= CURRENT_TIMESTAMP AND IFNULL(a."validTo", :toDate) >= CURRENT_TIMESTAMP) 
   			AND (IFNULL(a."frozenFrom", :fromDate) > CURRENT_TIMESTAMP OR IFNULL(a."frozenTo", :toDate) < CURRENT_TIMESTAMP)) 
   		OR (IFNULL(A."validFor", 'N') = 'N' AND IFNULL(A."frozenFor", 'N') = 'N')) 
   	ORDER BY A."ResType", "ResCode";
   	
END;