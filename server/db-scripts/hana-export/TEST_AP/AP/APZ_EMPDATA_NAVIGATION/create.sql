
CREATE PROCEDURE apz_EmpData_navigation
(
	in Currnt INT,
	in Typ INT,
	in userCode nvarchar(25) default ''
)
AS
BEGIN
DECLARE Mn INT;
DECLARE Mx INT;

declare userId int = ifnull((select USERID from OUSR where USER_CODE = :userCode), -1);
call apz_func_OwnerOverShip(:userCode, '171', ownerView);
SELECT TOP 1 "empID" into Mn default 0 FROM OHEM where "UserSign" = :userId or ifnull("U_OwnerCode", -1) = -1 or "U_OwnerCode" in (select distinct "OwnerId" from :ownerView) ORDER BY "empID" ASC;
SELECT TOP 1 "empID" into Mx default 0 FROM OHEM where "UserSign" = :userId or ifnull("U_OwnerCode", -1) = -1 or "U_OwnerCode" in (select distinct "OwnerId" from :ownerView) ORDER BY "empID" DESC;

if :Typ = 1 or (:Typ = 3 and :Currnt >= :Mx) then
	SELECT :Mn "empID" from DUMMY;
elseif :Typ = 4 or (:Typ = 2 and :Currnt <= :Mn) then
	SELECT :Mx "empID" from DUMMY;
elseif :Typ = 2 then
	SELECT TOP 1 "empID" FROM OHEM
	WHERE "empID"<:Currnt and ("UserSign" = :userId or ifnull("U_OwnerCode", -1) = -1 or "U_OwnerCode" in (select distinct "OwnerId" from :ownerView))
	ORDER BY "empID" desc;
else
	SELECT TOP 1 "empID" FROM OHEM
	WHERE "empID">:Currnt and ("UserSign" = :userId or ifnull("U_OwnerCode", -1) = -1 or "U_OwnerCode" in (select distinct "OwnerId" from :ownerView))
	ORDER BY "empID" asc;
end if;

END




