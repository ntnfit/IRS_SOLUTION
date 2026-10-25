

CREATE PROCEDURE Apz_SerialDetail_GetPage (
	IN 	Current INT
,		Type INT
,		Username NVARCHAR(50) DEFAULT ''
,		Branch INT DEFAULT -1
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	-- First:
	IF (:Type = 1) THEN 
		SELECT TOP 1 a."AbsEntry" FROM OSRN a ORDER BY "AbsEntry" ASC;
	-- Last:
	ELSEIF (:Type = 4) THEN 
		SELECT TOP 1 "AbsEntry" FROM OSRN a ORDER BY "AbsEntry" DESC;
	-- Previous:
	ELSEIF (:Type = 2) THEN 
	BEGIN
		DECLARE Min INT := 0;
		SELECT IFNULL((SELECT TOP 1 "AbsEntry" FROM OSRN a ORDER BY "AbsEntry" ASC), 0) INTO Min from DUMMY;
		-- Nếu đang ở Entry đầu tiên -> chọn đến Entry cuối(last)
		IF (:Current = :Min OR :Current = 0) THEN 
			SELECT TOP 1 "AbsEntry" FROM OSRN a ORDER BY "AbsEntry" DESC;
		ELSE 
			SELECT TOP 1 a."AbsEntry" FROM OSRN a WHERE "AbsEntry" < :Current ORDER BY "AbsEntry" DESC;
		END IF;
	END;
	ELSEIF (:Type = 3) THEN
	BEGIN
		DECLARE Max INT := 0;
		SELECT IFNULL((SELECT TOP 1 a."AbsEntry" FROM OSRN a ORDER BY "AbsEntry" DESC), 0) INTO Max from DUMMY;
		-- Nếu đang ở Entry cuối -> chọn đến entry đầu (first)
		IF (:Current = :Max OR :Current = 0) THEN 
			SELECT TOP 1 a."AbsEntry" FROM OSRN a ORDER BY "AbsEntry" ASC;
		ELSE 
			SELECT TOP 1 a."AbsEntry" FROM OSRN a WHERE "AbsEntry" > :Current ORDER BY "AbsEntry" ASC;
		END IF;
	END;
	END IF;

END;




