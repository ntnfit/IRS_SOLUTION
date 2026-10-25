

CREATE PROCEDURE APZ_ProductionScheduling_FindDataNextLast(IN id bigint, type int)
AS
BEGIN
	declare minId int;
	declare maxId int;
	
	IF :type = 0 THEN 
		SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" ORDER BY "PSCode" ASC;
	ELSE IF :type = 1 THEN 
		minId := IFNULL((SELECT MIN("PSCode") FROM APZ_OPRS), 0);
		IF :id <= :minId THEN 
			SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" ORDER BY "PSCode" DESC;
		ELSE 
			SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" WHERE "PSCode" < :id ORDER BY "PSCode" DESC;
		END IF;
	
	ELSE IF :type = 2 THEN 
		maxId := IFNULL((SELECT MAX("PSCode") FROM APZ_OPRS), 0);
		IF :id >= :maxId THEN 
			SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" ORDER BY "PSCode" ASC;
		ELSE 
			SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" WHERE "PSCode" > :id ORDER BY "PSCode" ASC;
		END IF;
	ELSE IF :type = 3 THEN 
		SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" ORDER BY "PSCode" DESC;
	ELSE 
		SELECT TOP 1 a.*, b."BeginDate", b."FCstDate", b."FixDate" FROM apz_OPRS a INNER JOIN APZ_OMRP b ON a."MrpId" = b."MrpCode" WHERE "PSCode" = :id;
	END IF;
	END IF;
	END IF;
	END IF;	
END




