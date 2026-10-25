
CREATE  PROCEDURE Apz_Cfl_GetListEmployees
as
begin
select "empID","firstName","middleName","lastName" from ohem  where "Active" = 'Y' order by "lastName" asc;
end




