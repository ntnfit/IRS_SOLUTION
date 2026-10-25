CREATE PROCEDURE TmSp_BOM_IsFather_Group ( IN son nvarchar(50) )
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as

	 	
begin
	-- procedure uses temporary table #TmpInSpIsFatherGrp as source of father nodes to be compared
	-- RETURNS: sigle row <isFatherFound>, <Line Number of Found Father>

	DECLARE actualLayer integer;					-- current layer of the tree nodes
	DECLARE newRows integer;				-- number of new nodes 		
	DECLARE foundLineV integer;

	actualLayer := 0;
	newRows := 1;	
		
	-- check for identical father and son
	foundLineV := 0;
	IF (:foundLineV > 0)
	THEN
		CREATE LOCAL TEMPORARY TABLE "#TmpInSpIsFatherGrp" ( id nvarchar(50), "Line" integer);
	END IF;
			
	SELECT count(*) into foundLineV FROM "#TmpInSpIsFatherGrp" WHERE id = :son;	
	
	IF (:foundLineV > 0)
	THEN
 		SELECT TOP 1 1, "Line" FROM "#TmpInSpIsFatherGrp" WHERE id = :son;
 		RETURN;
	END IF;

	-- insert root node id, start with child on layer 0
    	reached = select :son as "ID", :actualLayer as "LAYER" from dummy;

	-- continu until new layer adds newly discovered nodes
	WHILE (:newRows > 0) 
	DO

		actualLayer := :actualLayer + 1;
		
		reached =
			select * from :reached
			union all
			SELECT DISTINCT id, :actualLayer 
			FROM 
			(
				SELECT DISTINCT "Father" AS id FROM "ITT1"	
					WHERE "Code" IN 
						(SELECT id FROM :reached WHERE layer = :actualLayer-1)	-- select nodes from previous layer
			
				UNION ALL

				SELECT DISTINCT A."ItemCode" FROM "OWOR" A 
					INNER JOIN "WOR1" B ON A."DocEntry" = B."DocEntry" 
					WHERE B."ItemCode" in 
						(SELECT id FROM :reached WHERE layer = :actualLayer-1)	-- select nodes from previous layer
					AND (A."Status" = ('P') OR A."Status" = ('R') ) AND ((A."AsChild" = 'Y' AND B."ItemCode" <> A."ItemCode" ) OR A."AsChild" = 'N')
			) 	Fathers		
				
			WHERE id NOT IN (SELECT id FROM :reached);   -- remove already discovered nodes present in the @reached table  
				
		-- chek if node is present in currently added layer
		SELECT count(*) into newRows FROM "#TmpInSpIsFatherGrp" WHERE id IN (SELECT id FROM :reached WHERE layer = :actualLayer);	
		IF :newRows > 0 
		THEN
			SELECT TOP 1 1, "Line" FROM "#TmpInSpIsFatherGrp" WHERE id IN (SELECT id FROM :reached WHERE layer = :actualLayer);			 
			RETURN;
		END IF;
					
		SELECT count(*) into newRows FROM :reached WHERE layer = :actualLayer;	
							
	END WHILE;

	-- node not found in any of the layers
	SELECT 0, NULL FROM dummy;
	
end;


