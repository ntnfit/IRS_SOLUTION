
create procedure SP_APZ_GETINDEXRATES (in Year int default 1990)
As
Begin
declare cnt int = 1;
CREATE local temporary table #tbl 
(
"Month" int NULL,
"Name" nvarchar(20) NULL, 
"Rate" decimal(19, 6) NULL
)
;
WHILE :cnt < 13 DO
IF :cnt = 1
 THEN 
	INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'January', 0);
ELSEIF :cnt = 2 
THEN 
	INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'February', 0);
ELSEIF :cnt = 3 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'March', 0);
ELSEIF :cnt = 4 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'April', 0);
ELSEIF :cnt = 5 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'May', 0);
ELSEIF :cnt = 6 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'June', 0);
ELSEIF :cnt = 7 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'July', 0);
ELSEIF :cnt = 8 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'August', 0);
ELSEIF :cnt = 9 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'September', 0);
ELSEIF :cnt = 10 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'October', 0);
ELSEIF :cnt = 11 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'November', 0);
ELSEIF :cnt = 12 THEN INSERT INTO #tbl ("Month", "Name", "Rate") VALUES (:cnt, 'December', 0);
End If;
cnt := :cnt + 1;

 END WHILE;
 CREATE local temporary  TABLE #tmp AS (select top 0 MONTH("RateDate")  "Month", "Rate"  FROM ORTT);
Insert into #tmp 
SELECT MONTH("RateDate")  "Month", "Rate" FROM ORTT WHERE YEAR("RateDate") = :Year AND "Currency" = 'CP';

UPDATE #tbl SET #tbl."Rate" = b."Rate" FROM #tbl INNER JOIN #tmp b ON #tbl."Month" = b."Month";

SELECT * FROM #tbl;

drop table #tmp;
End;





