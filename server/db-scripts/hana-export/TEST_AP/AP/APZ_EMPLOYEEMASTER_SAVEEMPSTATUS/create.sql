
CREATE PROCEDURE APZ_EmployeeMaster_SaveEmpStatus
AS
BEGIN
declare statusID int;
declare maxCode int;
create local temporary table #APZ_EmpStatus as (select top 0* from OHST);
	execute immediate 'insert into #APZ_EmpStatus select * from "#APZ_EmpStatus"';
 delete from OHST;
 
  Insert into OHST
  Select * from #APZ_EmpStatus where IFNULL("statusID",0) <> 0;
  
  --kiểm tra typeID luôn Max
   select IFNULL((select "AutoKey" from onnm where "ObjectCode" = '173'),0) into statusID from DUMMY;
   select IFNULL((select Max("statusID") from OHST),0) into maxCode from DUMMY;
  if statusID< maxCode
  then
  select IFNULL((select Max("statusID") from OHST),0) into statusID from DUMMY;
end if;

  Insert into OHST 
  --lấy nhiều dòng tăng dần theo ID
  Select :statusID + ROW_NUMBER() OVER(ORDER BY "name" ASC)-1 , "name" ,"descriptio" from #APZ_EmpStatus where IFNULL("statusID",0) = 0;

  select IFNULL((select Max("statusID") from OHST),0) into statusID from DUMMY;
  update onnm set "AutoKey"=:statusID + 1 where "ObjectCode"='173';

END




