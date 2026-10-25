

CREATE PROCEDURE APZ_ServiceContractTemplate_Search
(
	in Type varchar(1) default '',
	ContractTemplateNum nvarchar(35),
	SLACode nvarchar(50),
	Status varchar(1)
)
AS
BEGIN
	select * from APZ_OCTT
	where (ifnull("Type", '') = '' or "Type" like '%' ||:Type||'%')
	and (ifnull("ContractTemplateNum", '') = '' or "ContractTemplateNum" like '%'|| :contractTemplateNum ||'%')
	and (ifnull("SLACode", 0) = 0 or "SLACode" like '%' || :SLACode || '%')
	and (ifnull("Status", '') = '' or "Status" like '%'||:Status||'%');
 END;




