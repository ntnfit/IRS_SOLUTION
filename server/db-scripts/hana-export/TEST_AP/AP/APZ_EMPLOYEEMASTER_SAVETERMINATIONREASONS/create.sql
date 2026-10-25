
CREATE PROCEDURE APZ_EmployeeMaster_SaveTerminationReasons
AS
BEGIN
  declare reasonID int;
  declare maxCode int;
create local temporary table #APZ_TerminationReasons as (select top 0* from OHTR);
	execute immediate 'insert into #APZ_TerminationReasons select * from "#APZ_TerminationReasons"';
 delete from OHTR; 
 
  Insert into OHTR
  Select * from #APZ_TerminationReasons where IFNULL("reasonID",0) <> 0;
  
  --kiểm tra typeID luôn Max
select IFNULL((select "AutoKey" from onnm where "ObjectCode" = '174'),0) into reasonID from DUMMY;
select IFNULL((select Max("reasonID") from OHTR),0) into maxCode from DUMMY;
  if reasonID < maxCode
  then
  select IFNULL((select Max("reasonID") from OHTR),0) into reasonID from DUMMY;
  end if;

  Insert into OHTR
  --lấy nhiều dòng tăng dần theo ID
  Select :reasonID + ROW_NUMBER() OVER(ORDER BY "name" ASC)-1 , "name" ,"descriptio" from #APZ_TerminationReasons where IFNULL("reasonID",0) = 0;

   select IFNULL((select Max("reasonID") from OHTR),0) into reasonID from DUMMY;
  update onnm set "AutoKey"=:reasonID + 1 where "ObjectCode"='174';

END




