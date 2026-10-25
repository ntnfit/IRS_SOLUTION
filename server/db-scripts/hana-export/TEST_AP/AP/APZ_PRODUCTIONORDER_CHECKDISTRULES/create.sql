
create procedure APZ_ProductionOrder_CheckDistRules
(
in OCode nvarchar(50) default '',
in OCode2 nvarchar(50) default '',
in OCode3 nvarchar(50) default '',
in OCode4 nvarchar(50) default '',
in OCode5 nvarchar(50) default ''
)
As
Begin
IF OCode = '' 
THEN SELECT '' FROM DUMMY;

ELSE 
SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OCode AND "DimCode" = 1 AND "Active" = 'Y';
END IF;
IF OCode2 = '' 
THEN SELECT '' FROM DUMMY;
ELSE 
SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OCode2 AND "DimCode" = 2 AND "Active" = 'Y';
END IF;
IF OCode3 = ''
 THEN SELECT '' FROM DUMMY;
ELSE 
SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OCode3 AND "DimCode" = 3 AND "Active" = 'Y';
END IF;
IF OCode4 = ''
 THEN SELECT '' FROM DUMMY;
ELSE
 SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OCode4 AND "DimCode" = 4 AND "Active" = 'Y';
END IF;
IF OCode5 = '' 
THEN SELECT '' FROM DUMMY;
ELSE 
SELECT "OcrCode" FROM OOCR WHERE "OcrCode" = :OCode5 AND "DimCode" = 5 AND "Active" = 'Y';
END IF;


End;





