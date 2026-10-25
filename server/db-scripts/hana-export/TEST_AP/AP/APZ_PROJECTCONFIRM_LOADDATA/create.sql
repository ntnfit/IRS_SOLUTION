
create procedure APZ_ProjectConfirm_LoadData
(
in id int,
in projectCode nvarchar(50),
in startFrom timestamp,
in startTo timestamp,
in status char(1)
)
as
begin
	DECLARE MaxStart timestamp;
	DECLARE MaxEnd timestamp;
	SELECT IFNULL((SELECT MAX("PostingDate") FROM APZ_OPJC),CURRENT_DATE) into MaxStart from DUMMY;
	SELECT IFNULL((SELECT MAX("DueDate") FROM APZ_OPJC), CURRENT_DATE) into MaxEnd FROM DUMMY;
if :id>0
	then
		select * from APZ_OPJC where "DocEntry"=:id;
		return ;
end if;
select * from APZ_OPJC where 
(ifnull(:projectCode,'') ='' or "ProjectId"=:projectCode) and
(ifnull("PostingDate",'1900-01-01') BETWEEN ifnull(:startFrom,'1900-01-01') AND ifnull(:startTo,:MaxStart)) and 
(ifnull(:status,'') ='' or "Status"=:status)
 order by "DocEntry" DESC;
end;




