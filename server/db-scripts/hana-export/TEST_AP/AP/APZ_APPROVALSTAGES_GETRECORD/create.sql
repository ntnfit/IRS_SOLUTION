

CREATE PROCEDURE APZ_ApprovalStages_GetRecord(IN position varchar(1), stageCode varchar(50))
AS
BEGIN

	declare code varchar(50) = '';
	declare firstCode varchar(50);
	declare lastCode varchar(50);
	
	select (select top 1 "WstCode" from OWST order by "WstCode") into firstCode from dummy;
	select (select top 1 "WstCode" from OWST order by "WstCode" desc) into lastCode from dummy;
	

	/* Check postition type is valid */
	if (:position not in('F', 'L', 'P', 'N'))
	then
		select '01' as ErrorCode, 'Invalid Navigation type' as ErrorMessage from dummy;
		return;
	end if;

	/* MOVE_FIRST */
	if (:position = 'F')
	then
		select (select top 1 "WstCode" from OWST order by "WstCode") into code from dummy;
	end if;

	/* MOVE_LAST */
	if (:position='L') 
	then
		select (select top 1 "WstCode" from OWST order by "WstCode" desc) into code from dummy;
	end if;

	/*MOVE_PREVIOUS */
	if (:position = 'P')
	then
		
		WITH CTE AS
		(
			select ROW_NUMBER() OVER(ORDER BY "WstCode") as RN, "WstCode" from OWST
		)
		select IFNULL(TO_VARCHAR(prev."WstCode"), '') into Code from CTE 
		left join CTE prev on prev.RN = CTE.RN - 1
		where CTE."WstCode" = :stageCode;
	end if;

	/*MOVE_NEXT */
	if (:position = 'N')
	then
		
		WITH CTE AS
		(
			select ROW_NUMBER() OVER(ORDER BY "WstCode") as RN, "WstCode" from OWST
		)
		select IFNULL(TO_VARCHAR(nxt."WstCode"), '') into code from CTE 
		left join CTE nxt on nxt.RN = CTE.RN + 1
		where CTE."WstCode" = :stageCode;

	end if;

	select * from OWST where "WstCode" = :code;

END



