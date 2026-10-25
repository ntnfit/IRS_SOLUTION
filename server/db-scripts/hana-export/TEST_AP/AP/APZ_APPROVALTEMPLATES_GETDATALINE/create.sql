

CREATE PROCEDURE APZ_ApprovalTemplates_GetDataLine(templateCode int)
AS
BEGIN


	declare docType varchar(50);
	declare chkCount int;
	
	--Get ogininator
	select a."WtmCode", a."UserID", b."U_NAME" as "UserName", c."Code" as "DepartmntCode", c."Name" as "Departmnt"
	from WTM1 a
    inner join OUSR b on a."UserID" = b.USERID
    inner join OUDP c on b."Department" = c."Code"
    where a."WtmCode" = :templateCode;
    
    --Get documents                    
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(23, 17, 15, 16, 203, 13, 165, 166, 14, 540000006, 22, 20, 21, 204, 18, 19, 234000032)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Documents' into docType from dummy;
	end if;
	
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(59, 60, 1250000001, 67, 310000001)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Inventory' into docType from dummy;
	end if;
	
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(1250000026, 1250000027)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Blanket Agreement' into docType from dummy;
	end if;
	
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(1470000113)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Internal Requistion' into docType from dummy;
	end if;
	
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(46)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Payment' into docType from dummy;
	end if;
	
	select (select top 1 1 from WTM3 where "WtmCode" = :templateCode and "TransType" in(10000071, 1470000065)) into chkCount from dummy;                           
	if :chkCount = 1
	then
		select 'Inventory Counting Transaction' into docType from dummy;
	end if;
	
   	select *, :docType as "DocType" from WTM3 where "WtmCode" = :templateCode;                                            
	
	--Get stages                                                
	select a."WtmCode", a."WstCode", b."Name" as "WstName", b."Remarks" as "WstRemarks", a."SortId" from WTM2 a
	inner join OWST b on a."WstCode" = b."WstCode"
	where "WtmCode" = :templateCode order by "SortId";
	
	--Get Terms
	select "Conds" from OWTM where "WtmCode" = :templateCode;
	
	
	select *, '' as "CondName", 'Y' as "Checked" from WTM4 where "WtmCode" = :templateCode;
	
	--Get Queries
	select *, b."QName" as "QueryName" from WTM5 a
	inner join OUQR b on a."QueryId" = b."IntrnalKey"
	where "WtmCode" = :templateCode;

END




