

CREATE PROCEDURE Apz_D_Delivery_SaveBPNote (IN type char(1) DEFAULT 'A')
AS
BEGIN
	
	create local temporary table #DLO5Local as (select top 0 * from APZ_D_DLO5);
	exec 'drop table #DLO5Local';
	exec 'create local temporary table #DLO5Local as (select * from #APZ_D_DLO5)';
	
	
	IF :type = 'A' THEN 
		INSERT INTO APZ_D_DLO5 ("CardCode", "Note") SELECT "CardCode", "Note" FROM #DLO5Local;
	ELSE IF :type = 'D' THEN 
		DELETE FROM APZ_D_DLO5 WHERE ID IN (SELECT ID FROM #DLO5Local);
	END IF;
	END IF;
	
	SELECT 0, '' FROM DUMMY;

END



