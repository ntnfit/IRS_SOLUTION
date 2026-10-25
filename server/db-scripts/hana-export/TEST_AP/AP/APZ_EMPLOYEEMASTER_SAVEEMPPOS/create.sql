
CREATE PROCEDURE APZ_EmployeeMaster_SaveEmpPos
AS
BEGIN
declare posID int;
declare maxCode int;
create local temporary table #APZ_Pos as (select top 0* from OHPS);
	execute immediate 'insert into #APZ_Pos select * from "#APZ_Pos"';
 delete from OHPS;
 
  Insert into OHPS 
  Select * from #APZ_Pos where IFNULL("posID",0) <> 0;
  
  --kiểm tra typeID luôn Max
  select IFNULL((select "AutoKey" from onnm where "ObjectCode" = '210'),0) into posID from DUMMY;
  select IFNULL((select Max("posID") from OHPS),0) into maxCode from DUMMY;
  if posID < maxCode
  then
   select IFNULL((select Max("posID") from OHPS),0) into posID from DUMMY;
end if;

  Insert into OHPS
  --lấy nhiều dòng tăng dần theo ID
  Select :posID + ROW_NUMBER() OVER(ORDER BY "name" ASC)-1 , "name" ,"descriptio",'N' from #APZ_Pos where IFNULL("posID",0) = 0;

   Select IFNULL((select Max("posID") from OHPS),0) into posID	from DUMMY;
  update onnm  set "AutoKey"=:posID + 1 where "ObjectCode"='210';

END




