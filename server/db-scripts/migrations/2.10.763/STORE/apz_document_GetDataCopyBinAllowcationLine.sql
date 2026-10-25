
CALL APZ_EXISTSPROC('apz_document_GetDataCopyBinAllowcationLine');

GO

CREATE  PROCEDURE apz_document_GetDataCopyBinAllowcationLine
(
	in DocEntry varchar(5000),
	in ObjType VARCHAR(20)
)
AS
BEGIN
	IF :ObjType = '112' THEN
		create local temporary table #rootTemp (docentry int);
		call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, :objType, '#rootTemp');
		select * from #rootTemp;
		drop table #rootTemp;
	ELSE
		SELECT TOP 0 T0.*, CAST(NULL AS integer) AS "BinAbs", CAST(NULL AS nvarchar(50)) AS "BinCode", CAST(NULL AS decimal(19, 6)) AS "SelectedQty"
			, CAST(NULL AS nvarchar(50)) AS "WhsCode", CAST(NULL AS nvarchar(254)) AS "BinDescription", CAST(NULL AS integer) AS "BaseLineNumber", CAST(NULL AS integer) AS "BaseDocEntry"
			, CAST(NULL AS char(1)) AS "ManBatchSerial", CAST(NULL AS decimal(19, 6)) AS "LogQty", CAST(NULL AS char(1)) AS "IsExits", CAST(NULL AS integer) AS "TranType"
			, CAST(NULL AS char(1)) AS "IsBase" FROM OBTN T0;
	END IF;
END;