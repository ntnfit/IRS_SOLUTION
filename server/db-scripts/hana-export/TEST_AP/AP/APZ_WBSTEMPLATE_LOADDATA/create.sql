

CREATE PROCEDURE APZ_WBSTemplate_LoadData(in code nvarchar(50) default '', fromDate timestamp default '', toDate timestamp default '')
AS
BEGIN
	select * from APZ_OWBS where (ifnull(:code, '') = '' or "Code" = :code) 
	and (ifnull(:fromDate, '') = '' or "CreateDate" >= :fromDate)
	and (ifnull(:toDate, '') = '' or "CreateDate" <= :toDate);
END;



