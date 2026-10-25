
CREATE PROCEDURE apz_EmpData_GetDetail
	(in empID int)
as
BEGIN

 select *,
 		IFNULL((select top 1 "SalTotal" from APZ_OHSI where CURRENT_DATE <= IFNULL("ValidTo", '9999/12/31') and CURRENT_DATE >= IFNULL("ValidFrom", '9999/12/31') and "EmpId" = :empID), 0) "salary",
 		ifnull((select top 1 a."LinkThumbnails" from  APZ_D_UUPIM a join OHEM b on a."UserSign" = b."userId" where a."ObjectType" = '1470000171' and b."empID" = :empID), '') as "CloudImage"
 from OHEM where "empID" = :empID;
 select * from hem6 where "empID"=:empID;
 select * from ATC1 where "AbsEntry"=(select "AtcEntry" from ohem where "empID"=:empID );
 select * from HEM1 where "empID"=:empID;
 select * from HEM2 where "empID"=:empID;
 select * from HEM3 where "empID"=:empID;
 select * from HEM4 where "empID"=:empID;
 select * from HTM1 where "empID"=:empID;
 select * from APZ_OHEH where "empID"=:empID;
 select * from APZ_OHEC where "empID"=:empID;
 END;




