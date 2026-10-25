
CALL APZ_EXISTSPROC('APZ_SalesBlanketAgreement_GetDoc');

GO

CREATE PROCEDURE APZ_SalesBlanketAgreement_GetDoc
(
	IN 	FromDate timestamp
, 		ToDate timestamp	
, 		BpCode nvarchar(50) DEFAULT ''
, 		Status char(1) DEFAULT ''
, 		BpType nvarchar(1) DEFAULT ''
, 		UserCode nvarchar(50)
,		Number INT DEFAULT 0
, 	   	AbsID INT DEFAULT 0
)
AS
BEGIN
	DECLARE dofilter char(1);	
	DECLARE DoMngMth char(1);
	DECLARE maxdate timestamp;
	DECLARE mindate timestamp;
	
	DECLARE te int :=1;
	
	SELECT IFNULL((SELECT "DoFilter" FROM CINF), 'N') INTO dofilter FROM DUMMY;
	SELECT IFNULL((SELECT "DoMngMth" FROM OADM), '') INTO DoMngMth FROM DUMMY;
	SELECT IFNULL((SELECT MAX("SignDate") FROM OOAT), NOW()) INTO maxdate FROM DUMMY;
	SELECT IFNULL((SELECT MIN("SignDate") FROM OOAT), NOW()) INTO mindate FROM DUMMY;
	
	CREATE LOCAL TEMPORARY TABLE #tbl AS 
		(SELECT TOP 0 a.*, '' AS "Name", b."Tel1", b."E_MailL" 
			FROM OOAT a 
			LEFT OUTER JOIN OCPR b ON a."CntctCode" <> 0 
						AND a."CntctCode" = b."CntctCode");
	
	INSERT INTO #tbl
		(SELECT a.*, '' AS "Name", b."Tel1", b."E_MailL" 
			FROM OOAT a 
			LEFT OUTER JOIN OCPR b ON a."CntctCode" <> 0 
						AND a."CntctCode" = b."CntctCode"		
			WHERE a."SignDate" BETWEEN IFNULL(FromDate, mindate) AND IFNULL(ToDate, maxdate)
				AND (IFNULL(BpType, '') = '' OR a."BpType" = BpType) 
				AND (IFNULL(BpCode, '') = '' OR a."BpCode" = BpCode) 
				AND (IFNULL(Status, '') = '' OR a."Status" = Status OR (Status = 'C' AND a."Cancelled" = 'Y')) 
				AND (Number = 0 OR Number > 0 AND a."Number" = Number) 
				AND (AbsID = 0 OR AbsID > 0 AND a."AbsID" = AbsID) 
				
				/* chưa convert
				AND
			      (
			          @dofilter='N'
			          OR (@dofilter='Y' AND @DoMngMth<>'B' AND (ISNULL(a.Owner, -1) =-1 OR (a.Owner IN (SELECT DISTINCT x.OwnerId FROM apz_func_OwnerOverShip(@UserCode, 2) x ))))
			      )
				*/
			);
	UPDATE #tbl SET "Status" = 'C' WHERE "Cancelled" = 'Y';
	SELECT * FROM #tbl order by "AbsID" desc;
	DROP TABLE #tbl;
END;