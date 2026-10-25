-- B1 DEPENDS: AFTER:SP:TmSp_ConGrpLine_OACT_recurs AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ConvertGrpLine_OACT (in v_RandomNum nvarchar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
------------------------------------------------------------
acct1 nvarchar(15);
acct2 nvarchar(15);
line int;
lineOut int;
counter tinyint;
countnew tinyint;
maxlevel tinyint;
ErrNum int;
rownum int;
rowcount int;
BEGIN
IF :v_RandomNum <> '' THEN

select "MaxActGrps" INTO maxlevel from CINF;

--maxlevel := 10;
counter := 1;
ErrNum := 10000;

update OACT set "GrpLine" = 0 where "GrpLine" is NULL;

update OACT set "Groups" = :ErrNum;

WHILE :counter <= :maxlevel 
do
 IF :counter <= 9 
 THEN
 SELECT CAST(:counter as nvarchar(1)) into acct1 FROM dummy;
 acct1 := :acct1 || '00000000000000';
   -- acct1 := cast (:counter as nvarchar(1)) + '00000000000000';
 ELSE 
    SELECT ASCII('A') into countnew  FROM dummy;
    countnew := :countnew + :counter - 10;
    select CHAR(:countnew) into acct1 from dummy;
    acct1 := :acct1 || '00000000000000';
  -- acct1 := char(ASCII ('A') + :counter - 10) + '00000000000000';
 END IF;

 update "OACT"
 set "Groups" = 1,
     "GroupMask" = :counter,
     "Levels" =1,
     "FatherNum" = NULL, 
     "Postable" = 'N'
 where "AcctCode" = :acct1;

 line := 2;
 
 CALL TmSp_ConGrpLine_OACT_recurs(:acct1,:line,:counter,:ErrNum, lineOut);

 counter := :counter + 1 ;

END WHILE;





update "OACT"
set "GrpLine" = "Groups",
    "Groups" = NULL;

counter := 1;

WHILE :counter <= :maxlevel
  do
 
 select MAX ("GrpLine") into line from OACT
                          where "GroupMask" =:counter and
                          "GrpLine" <> :ErrNum ;
 line := :line + 1; 
 IF :counter <= 9
  then
   SELECT CAST(:counter as nvarchar(1)) into acct1 FROM dummy;
   acct1 := :acct1 || '00000000000000';
 --set @acct1= cast (@counter as nvarchar(1)) + '00000000000000'
 ELSE 
    SELECT ASCII('A') into countnew  FROM dummy;
    countnew := :countnew + :counter - 10;
    select CHAR(:countnew) into acct1 from dummy;
    acct1 := :acct1 || '00000000000000';
  --set @acct1= char (ASCII ('A') + @counter - 10) + '00000000000000'
end if;
select count(1) into rownum from "OACT"
               where "GrpLine" = :ErrNum and
               "GroupMask" = :counter;
 WHILE :rownum > 0
 do
 select count("AcctCode") into rowcount 
         from "OACT"
         where "GrpLine" = :ErrNum and
               "GroupMask" = :counter;
 if rowcount > 0
 then
   select Top 1 "AcctCode" into acct2
         from "OACT"
         where "GrpLine" = :ErrNum and
               "GroupMask" = :counter
		ORDER BY "AcctCode";
  else acct2 := '';
  end if;
  
  
 
  if :acct2 <> ''
  then
  update "OACT"
  set "Levels" = 2,
      "FatherNum" = :acct1,
      "GrpLine" = :line
  where "AcctCode" = :acct2;
  end if;
  
  
  line := :line + 1;
  select count(1) into rownum from "OACT"
               where "GrpLine" = :ErrNum and
               "GroupMask" = :counter;
 END while;
 counter := :counter + 1;
  --select :acct2 from dummy;
END while;

END IF;
end;


