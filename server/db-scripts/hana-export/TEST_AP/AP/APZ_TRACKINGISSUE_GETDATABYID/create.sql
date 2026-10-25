
create  procedure APZ_TrackingIssue_GetDataById (
	in Id int default 0)
as
Begin

declare attId int=0;
select ifnull((select "AttachedFiled" from APZ_TGIE where "Id"=:Id), 0) into attId from dummy;

select a.*,b."Descriptio" as "SolutionDesc",b."Subject" as "Subject", c."TaskName" as "TaskName" from APZ_TGIE a
	left join oslt b on a."SolutionId"=b."SltCode"
	left join APZ_PRG1 c on c."TaskCode"=a."WbsCode" and c."AbsEntry"=a."ProjectCode" 
	left join APZ_OTIS s on a."Status" = s."Code"
	where "Id"=:Id;
	
 select * from ATC1 where "AbsEntry"=:attId;
 
 select *,1 as "IsDatabase" from APZ_TGIE1 Where "Id"=:Id;
End;





