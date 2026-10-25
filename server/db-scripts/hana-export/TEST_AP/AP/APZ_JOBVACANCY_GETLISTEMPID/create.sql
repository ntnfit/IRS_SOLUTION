
Create procedure APZ_JobVacancy_GetListEmpId

as
Begin
select "EmpID","EmpName" from APZ_HJV1 where "Primary"='Y';
End;




