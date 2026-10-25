
CALL APZ_EXISTSPROC('APZ_Series_LoadAllByObject');

GO

CREATE PROCEDURE APZ_Series_LoadAllByObject (in objectCode nvarchar(50), in userCode nvarchar(50))
AS
BEGIN
	SELECT a."Series",a."SeriesName",IFNULL(a."BPLId",-1) "BPLId",'N' "IsDefault"
	FROM NNM1 a join NNM3 b on a."Series"=b."Series"
	WHERE b."ObjectCode"=:objectCode AND (IFNULL(a."BPLId",-1)=-1 OR a."BPLId" IN (SELECT "BPLId" FROM USR6 WHERE "UserCode"=:userCode));
END