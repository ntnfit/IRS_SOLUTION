

CREATE PROCEDURE Apz_D_Delivery_SaveLocationNote (IN type char(1) DEFAULT 'A')
AS
BEGIN
	
	create local temporary table #DLO4Local as (select top 0 * from APZ_D_DLO4);
	exec 'drop table #DLO4Local';
	exec 'create local temporary table #DLO4Local as (select * from #APZ_D_DLO4)';
	
	IF :type = 'A' THEN 
		INSERT INTO APZ_D_DLO4 ("ShipCode", "Note") SELECT "ShipCode", "Note" FROM #DLO4Local;
	ELSE IF :type = 'D' THEN 
		DELETE FROM APZ_D_DLO4 WHERE ID IN (SELECT ID FROM #DLO4Local);
	END IF;
	END IF;
	
	SELECT 0, '' FROM DUMMY;

END



