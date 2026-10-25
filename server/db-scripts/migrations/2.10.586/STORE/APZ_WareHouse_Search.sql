
CALL APZ_EXISTSPROC('APZ_WareHouse_Search');

GO


CREATE PROCEDURE APZ_WareHouse_Search(
	in WareHouseCode nvarchar(5000) DEFAULT NULL,
	in WareHouseName nvarchar(5000) DEFAULT NULL, 
	in branch int) 
AS 
BEGIN
	--declare newWareHouseCode nvarchar(50) = ltrim(rtrim(IFNULL(:WareHouseCode,'')));
	--declare newWareHouseName nvarchar(50) = ltrim(rtrim(IFNULL(:WareHouseName,'')));

	SELECT a.* ,
	       b."BinCode" AS "DftBinAbsCode" ,
	       ifnull((SELECT COUNT(1)
	                 FROM OBIN t
	                 WHERE t."WhsCode"=a."WhsCode"),0) "NoOfBinLocs"
	FROM OWHS a LEFT JOIN OBIN b ON a."DftBinAbs"=b."AbsEntry"
	WHERE 1=1
	  AND (ifnull(:WareHouseCode, '') = ''
	       OR a."WhsCode" like N'%'||:WareHouseCode||'%')
	  AND (ifnull(:WareHouseName, '') = ''
	       OR a."WhsName" like N'%' || :WareHouseName || '%')
	  AND (ifnull(:branch, 0) = 0
	       OR a."BPLid" = :branch); 
END;