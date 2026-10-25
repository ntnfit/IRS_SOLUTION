-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ConGrpLine_OACT_recurs(
in acct1 NVARCHAR(15),
in line int,
in counter tinyint,
in ErrNum int,
OUT LineOut int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
tempLine int;
level int;
sonCount int;
brotherCount int;
cur_acct nvarchar(15);
cur_acct2 nvarchar(15);
postable nvarchar(15);
something_left SMALLINT;
brother_exist SMALLINT;
BEGIN

	if :line = 2
	then
		level := 2;
	else
		SELECT "Levels" + 1 INTO level FROM OACT WHERE "AcctCode" = :acct1;
	end if;	

	tempLine := :line;
	cur_acct := :acct1;
	something_left := 1;

	while (something_left = 1) do
		sonCount := 0;		
		SELECT "Postable" INTO postable FROM OACT WHERE "AcctCode" = cur_acct;
		if (postable = 'N') then
			SELECT count("AcctCode") INTO sonCount FROM OACT 
				WHERE "FatherNum" = :cur_acct AND "Levels" = :level AND "Groups" = :ErrNum;
		end if;
		
		if (sonCount > 0) then
-- has son
			SELECT Top 1 "AcctCode" into cur_acct2 FROM OACT
				WHERE "FatherNum" = :cur_acct AND "Levels" = :level AND "Groups" = :ErrNum AND
				"GrpLine" = (SELECT MIN ("GrpLine") from OACT
						WHERE "FatherNum" = :cur_acct AND
							"Levels" = :level and "Groups" = :ErrNum);
			
			UPDATE OACT SET "Groups" = :tempLine, "GroupMask" = :counter
				WHERE "AcctCode" = :cur_acct2;
            
			cur_acct := :cur_acct2;
			tempLine := :tempLine + 1;
			level := :level + 1;

		else
-- do not have son, search brother
			brother_exist := 0;
			while (brother_exist = 0) AND (something_left <> 0) do
				level := :level - 1;
				SELECT "FatherNum" into cur_acct FROM OACT WHERE "AcctCode" = cur_acct;

				SELECT count("AcctCode") into brotherCount FROM OACT
					WHERE "FatherNum" = :cur_acct AND "Levels" = :level AND "Groups" = :ErrNum;

				if ((cur_acct = acct1) AND (brotherCount = 0)) or (cur_acct is NULL) then
					something_left := 0;
				end if;
				if (brotherCount > 0) then 
					brother_exist := 1;
				else
					brother_exist := 0;
				end if;
			end while;
				
		end if;
			
	end while;
	
END;


