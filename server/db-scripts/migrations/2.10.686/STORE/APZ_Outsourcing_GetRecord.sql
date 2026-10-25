
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetRecord');

GO

CREATE PROCEDURE APZ_Outsourcing_GetRecord
(
	in Pos nvarchar(1) default '',
	in Code nvarchar(10) default ''
)
AS
BEGIN

	IF :Pos = 'F'
	THEN 
		SELECT top 1 "Code" FROM "@TBL_OUTS_DOCNUM" ORDER BY "Code" ASC;
		RETURN;
	END IF;
	
	IF :Pos = 'N'
	THEN 
		CREATE LOCAL TEMPORARY TABLE #Temp AS (Select *,ROW_NUMBER() OVER (ORDER BY "Code") as "Rn" FROM "@TBL_OUTS_DOCNUM" ORDER BY "Code" ASC);
		Select CAST("Code" As Int) From #Temp a JOIN (Select "Rn" From #Temp Where "Code" = :Code) b On a."Rn"= b."Rn" + 1;
		DROP TABLE #Temp;
		RETURN;
	END IF;
	
	IF :Pos = 'P'
	THEN 
		CREATE LOCAL TEMPORARY TABLE #Temp AS (Select *,ROW_NUMBER() OVER (ORDER BY "Code") as "Rn" FROM "@TBL_OUTS_DOCNUM" ORDER BY "Code" ASC);
		Select CAST("Code" As Int) From #Temp a JOIN (Select "Rn" From #Temp Where "Code" = :Code) b On a."Rn"= b."Rn" - 1;
		DROP TABLE #Temp;
		RETURN;
	END IF;
	
	IF :Pos = 'L'
	THEN 
		SELECT top 1 CAST("Code" As Int) FROM "@TBL_OUTS_DOCNUM" Order BY CAST("Code" As Int) DESC;
		RETURN;
	END IF;

END