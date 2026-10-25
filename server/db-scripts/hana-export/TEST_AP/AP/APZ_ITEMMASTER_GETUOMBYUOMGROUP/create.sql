
CREATE PROCEDURE APZ_ItemMaster_GetUoMByUomGroup (IN UgpEntry integer default 0)
 AS BEGIN 
 SELECT a."UomEntry", "AltQty", "BaseQty" FROM UGP1 a INNER JOIN OUOM b ON a."UomEntry" = b."UomEntry" WHERE "UgpEntry" = :UgpEntry;
END;




