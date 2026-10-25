
CREATE PROCEDURE APZ_ItemMaster_GetUoMCombo
 (IN UgpEntry integer, IN ItemCode nvarchar(50), IN Type char(1)) 
 AS 
 BEGIN 
 	SELECT a."UomEntry", "UomCode", "UomName" FROM UGP1 a INNER JOIN OUOM b ON a."UomEntry" = b."UomEntry" WHERE "UgpEntry" = :UgpEntry;
END;




