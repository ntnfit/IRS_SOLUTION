

CREATE PROCEDURE APZ_ApprovalTemplates_GetRecord(IN position varchar(1), templateCode varchar(50))
AS
BEGIN

	declare code varchar(50) = '';
	declare firstCode varchar(50);
	declare lastCode varchar(50);
	
	select (select top 1 "WtmCode" from OWTM order by "WtmCode") into firstCode from dummy;
	select (select top 1 "WtmCode" from OWTM order by "WtmCode" desc) into lastCode from dummy;
	
	/* Check postition type is valid */
	if (:position not in('F', 'L', 'P', 'N'))
	then
		select '01' as ErrorCode, 'Invalid Navigation type' as ErrorMessage from dummy;
		return;
	end if;

	/* MOVE_FIRST */
	if (:position = 'F')
	then
		select (select top 1 "WtmCode" from OWTM order by "WtmCode") into code from dummy;
	end if;

	/* MOVE_LAST */
	if (:position='L') 
	then
		select (select top 1 "WtmCode" from OWTM order by "WtmCode" desc) into code from dummy;
	end if;

	/*MOVE_PREVIOUS */
	if (:position = 'P')
	then
		WITH CTE AS
		(
			select ROW_NUMBER() OVER(ORDER BY "WtmCode") as RN, "WtmCode" from OWTM
		)
		select IFNULL(TO_VARCHAR(prev."WtmCode"), '') into Code from CTE 
		left join CTE prev on prev.RN = CTE.RN - 1
		where TO_VARCHAR(CTE."WtmCode") = :templateCode;
	end if;

	/*MOVE_NEXT */
	if (:position = 'N')
	then
		WITH CTE AS
		(
			select ROW_NUMBER() OVER(ORDER BY "WtmCode") as RN, "WtmCode" from OWTM
		)
		select IFNULL(TO_VARCHAR(nxt."WtmCode"), '') into code from CTE 
		left join CTE nxt on nxt.RN = CTE.RN + 1
		where CTE."WtmCode" = :templateCode;
	end if;

	select * from OWTM where TO_VARCHAR("WtmCode") = :code;
	
END



