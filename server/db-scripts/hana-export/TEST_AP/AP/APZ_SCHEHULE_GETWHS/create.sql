

CREATE PROCEDURE APZ_SCHEHULE_GETWHS (IN inputId int default -1, inputType nvarchar(50) default '')
AS
BEGIN
	Declare whsList nvarchar(4000);
	Select "Whs" into whsList from APZ_OMRP a INNER JOIN APZ_OPRS b ON a."MrpCode" = b."MrpId" where b."PSCode" = :inputId;
	select "WhsCode", "WhsName" from OWHS where "WhsCode" in (select trim(Data) from Split(:whsList, ',')); -- where @branch = -1 or isnull(BPLid,-1) = @branch
END




