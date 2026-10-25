CREATE PROCEDURE TmSP_UpdateESEnabled (Enabled NVARCHAR(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
BEGIN
		DECLARE OldStatus NVARCHAR(1);
		DECLARE Counter INT;
		SELECT "ESEnabled" INTO OldStatus FROM "CINF";
		IF :OldStatus <> :Enabled THEN
			UPDATE "CINF" SET "ESEnabled"=:Enabled;
			--CACHE_OBJECT_CINF 124
			Counter := 0;
			SELECT count("ID") INTO Counter FROM "CHEN" WHERE "ID"=124 AND "Component"='C';
			IF :Counter = 0 THEN 
				INSERT INTO "CHEN"("Component", "ID", "Counter") VALUES('C', 124, 1);
			ELSE 
				UPDATE "CHEN" SET "Counter"="Counter" + 1  WHERE "ID"=124 AND "Component"='C';
			END IF;
		END IF;
		--CACHE_OBJECT_USR 12
		Counter := 0;
		SELECT count("ID") INTO Counter FROM "CHEN" WHERE "ID"=12 AND "Component"='U';
		IF :Counter = 0 THEN
			INSERT INTO "CHEN"("Component", "ID", "Counter") VALUES('U', 12, 1);
		ELSE 
			UPDATE "CHEN" SET "Counter"="Counter" + 1 WHERE "ID"=12 AND "Component"='U';
		END IF;
END;


