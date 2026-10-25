/*
  drop procedure TmSp_GetFifoLayers
  Ussage:  
	call TmSp_GetFifoLayers ('01-SW-550', 'RSFGWH', 64, 6, 'Y', 'OIVQ')
*/

CREATE PROCEDURE TmSp_GetFifoLayers
(
	in ItemCodeStr NVARCHAR(100),
	in LocCodeStr NVARCHAR(100),
	in LocType INT,
	in QuantityRequested DECIMAL(21,6),
	in IsManageByWhsStr VARCHAR(1),
	in IVQ_TableStr VARCHAR(5)
)
LANGUAGE SQLSCRIPT
AS
subStrSQL NVARCHAR(5000);
strSQL NVARCHAR(5000);
BEGIN
	declare QuantityRequestedStr VARCHAR(128);
	declare LocTypeStr VARCHAR(128);
	declare IVL1_TableStr VARCHAR(5);
	
	ItemCodeStr := replace(:ItemCodeStr,'''','''''');
	LocCodeStr := replace(:LocCodeStr,'''','''''');

	QuantityRequestedStr := CAST(:QuantityRequested as VARCHAR(128));
	LocTypeStr := CAST(:LocType as VARCHAR(128));

	IVL1_TableStr := 'IVL1';
	IF :IVQ_TableStr = 'UIVQ' THEN
		IVL1_TableStr := 'UIVL1';
	END IF;

	IF :IVQ_TableStr = 'SIVQ' THEN
		IVL1_TableStr := 'SIVL1';
	END IF;
	
	IF :IsManageByWhsStr = 'Y' THEN
	   subStrSQL :=  ''' AND "LocCode" = ''' || :LocCodeStr || ''' AND "LocType" = ''' || :LocTypeStr; 
	ELSE
	   subStrSQL := '';
	END IF;
	
	strSQL := 'SELECT "SumOpenQty", "ParentID", "TreeID" , t."TransSeq" , t."LayerID" , "OpenQty" , "OpenValue" , "AbsEntry" , "RemMethod", t1."CalcPrice", t1."LayerInQty", t1."LayerOutQ", t1."TransValue" 
	FROM
	(SELECT *, SUM("OpenQty") OVER (ORDER BY "TreeID","TransSeq" DESC, "LayerID" DESC ROWS UNBOUNDED PRECEDING) as "SumOpenQty" from "' || :IVQ_TableStr || '" 
	WHERE "OpenQty">0 AND "ItemCode" = ''' || :ItemCodeStr || :subStrSQL || ''' ) as t 
	JOIN "' || :IVL1_TableStr || '" as t1 on t."TransSeq" = t1."TransSeq" and t."LayerID" = t1."LayerID" 
	where (' || :QuantityRequestedStr || ' >= t."SumOpenQty") OR (' || :QuantityRequestedStr || ' < t."SumOpenQty" AND ' || :QuantityRequestedStr || ' > t."SumOpenQty" - "OpenQty") 
	ORDER BY "TreeID", "TransSeq" DESC, "LayerID" DESC ';
				   
	--select strSQL from dummy;
	EXECUTE IMMEDIATE strSQL;
END


