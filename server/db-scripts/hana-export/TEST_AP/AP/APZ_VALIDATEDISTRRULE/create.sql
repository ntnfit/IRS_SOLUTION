
CREATE PROCEDURE APZ_ValidateDistrRule(
	in OcrCode1 nvarchar(254) default '',
	in OcrCode2 nvarchar(254) default '',
	in OcrCode3 nvarchar(254) default '',
	in OcrCode4 nvarchar(254) default '',
	in OcrCode5 nvarchar(254) default '')
AS
BEGIN
		--get id from temp table
		--remove in apz_OCLG
		--remove sub task in APZ_OCLG 
		IF IFNULL(:OcrCode1,'') != ''
		THEN
			SELECT IFNULL((SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OcrCode1 AND "DimCode"=1),'') into OcrCode1 FROM DUMMY;
		END IF;

		IF IFNULL(:OcrCode2,'') != ''
		THEN
			SELECT IFNULL((SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OcrCode2 AND "DimCode"=2),'') into OcrCode2 FROM DUMMY;
		END IF;

		IF IFNULL(:OcrCode3,'') != ''
		THEN
			SELECT IFNULL((SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OcrCode3 AND "DimCode"=3),'') into OcrCode3 FROM DUMMY;
		END IF;

		IF IFNULL(:OcrCode4,'') != ''
		THEN
			SELECT IFNULL((SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OcrCode4 AND "DimCode"=4),'') into OcrCode4 FROM DUMMY;
		END IF;

		IF IFNULL(:OcrCode5,'') != ''
		THEN
			SELECT IFNULL((SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OcrCode5 AND "DimCode"=5),'') into OcrCode5 FROM DUMMY;
		END IF;

		SELECT :OcrCode1 "OcrCode" from DUMMY
		UNION ALL
		SELECT :OcrCode2 from DUMMY
		UNION ALL
		SELECT :OcrCode3 from DUMMY
		UNION ALL
		SELECT :OcrCode4 from DUMMY
		UNION ALL
		SELECT :OcrCode5 from DUMMY;
END;



