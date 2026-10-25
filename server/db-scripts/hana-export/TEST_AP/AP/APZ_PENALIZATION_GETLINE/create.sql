
CREATE PROCEDURE APZ_Penalization_GetLine
(in DocNo int)
as
begin
select a.*,b."empID" as "empName",b."dept",b."branch",c."Amount" from APZ_HPT1 a join OHEM b on a."EmpId" = b."empID" join APZ_OHPT c on a."PenalizationType" = c."Code" where a."DocNum" = :DocNo order by b."empID";

end




