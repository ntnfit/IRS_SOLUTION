
CREATE PROCEDURE apz_EmpRelatives_GetDetail
	(in empId int,
	in relaId int)
as
begin
select * from APZ_OHER where "empID" = :empId and "relaID" = :relaId;
select * from ATC1 where "AbsEntry"=(select "AtcEntry" from APZ_OHER where "empID"=:empID and "relaID" = :relaId);
end





