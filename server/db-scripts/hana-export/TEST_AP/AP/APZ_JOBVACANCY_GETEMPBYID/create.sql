
Create procedure APZ_JobVacancy_GetEmpById
(
in id int
)
as
Begin
select "empID","firstName","middleName","lastName"  from OHEM where "empID"=:id;
End;




