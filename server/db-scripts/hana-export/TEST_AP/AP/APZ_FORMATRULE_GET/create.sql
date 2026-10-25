
CREATE PROCEDURE APZ_FormatRule_Get
(
in userCode nvarchar(50),
in formId nvarchar(50),
in itemId nvarchar(50)
)
AS 
BEGIN
declare exists_table bigint =0;
declare exist bigint =0;
/*select count(1) into exists_table from INFORMATION_SCHEMA.TABLES where TABLE_NAME=:Itemid;
if(ifnull(exists_table,0)=1)
then
	set exists_table=1;
end if;*/

select count(1) into exist from APZ_FCRE where "UserCode" = :userCode and "FormId" = :FormId and "ItemId" = :Itemid;
if(exist=0) then
	select 'manager' into userCode from dummy;
 end if;
SELECT * FROM APZ_FCRE where "UserCode" = :userCode and "FormId" = :formId and "ItemId" = :itemId;
END;




