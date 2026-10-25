
create procedure APZ_ProjectReportCompletion_LoadData
(
in id int,
in projectCode nvarchar(50),
in status char(1),
in startFrom timestamp,
in startTo timestamp
)
as
begin
DECLARE MaxStart timestamp;
	DECLARE MaxEnd timestamp;
	SELECT IFNULL((SELECT MAX("PostingDate") FROM APZ_ORTC),CURRENT_DATE) into MaxStart from DUMMY;
	SELECT IFNULL((SELECT MAX("DueDate") FROM APZ_ORTC), CURRENT_DATE) into MaxEnd FROM DUMMY;
if :id>0
	then
		select * from APZ_ORTC where "DocEntry"=:id;
		return ;
end if;
select * from APZ_ORTC where 
(ifnull(:projectCode,'') ='' or "ProjectId"=:projectCode) and
(ifnull(:status,'') ='' or "Status"=:status) and
(ifnull("PostingDate",'1900-01-01') BETWEEN ifnull(:startFrom,'1900-01-01') AND ifnull(:startTo,:MaxStart))
 order by "DocEntry" DESC;
end;






