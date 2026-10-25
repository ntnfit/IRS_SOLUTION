

Create PROCEDURE APZ_D_SurveyDMS_Save (IN type char(1) DEFAULT '')
AS
BEGIN
	
	DECLARE	Id INT = 0;
	
	create local temporary table #OSUVLocal as (select top 0 * from APZ_D_OSUV);
	exec 'drop table #OSUVLocal';
	exec 'create local temporary table #OSUVLocal as (select * from #APZ_D_OSUV)';
	
	create local temporary table #SUV1Local as (select top 0 *, 0 as "NumId" from APZ_D_SUV1);
	exec 'drop table #SUV1Local';
	exec 'create local temporary table #SUV1Local as (select *, row_number() over() as "NumId" from #APZ_D_SUV1)';
	
	IF :type = 'U' THEN 
		SELECT TOP 1 "DocEntry" INTO Id FROM #OSUVLocal;
		DELETE FROM APZ_D_SUV1 WHERE "DocEntry" = :Id;
		DELETE FROM APZ_D_OSUV WHERE "DocEntry" = :Id;
	ELSE 
		Id := IFNULL((SELECT MAX("DocEntry") FROM APZ_D_OSUV), 0) + 1;
		UPDATE #OSUVLocal SET "DocEntry" = :Id;
	END IF;

	INSERT INTO APZ_D_OSUV (SELECT * FROM #OSUVLocal);
	INSERT INTO APZ_D_SUV1 ("DocEntry","LineNum","VisOrder","Question","Answer") SELECT :Id,"NumId","VisOrder","Question","Answer" from #SUV1Local;

	SELECT :Id, '' FROM DUMMY;

END




