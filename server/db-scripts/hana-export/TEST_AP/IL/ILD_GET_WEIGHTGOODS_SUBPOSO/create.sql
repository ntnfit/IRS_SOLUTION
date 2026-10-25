CREATE PROCEDURE ILD_GET_WEIGHTGOODS_SUBPOSO
(
	IN fdate date ,
	IN tdate date ,
	IN doctype nvarchar(20)
)
AS
BEGIN
	IF(:doctype = 22) THEN --PO	
		SELECT "DocNum" FROM OPOR t1 WHERE t1."DocDate" between fdate and tdate
		union
		SELECT distinct T1.SOPONO from GOODSWEIGHT t1 WHERE t1.docdate between fdate and tdate;		
	END IF;	
	IF(:doctype = 17) THEN --SO
		SELECT "DocNum" FROM ORDR t1 WHERE t1."DocDate" between fdate and tdate
		union
		SELECT distinct T1.SOPONO from GOODSWEIGHT t1 WHERE t1.docdate between fdate and tdate;	
	END IF;	
END
