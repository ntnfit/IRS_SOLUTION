-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2
--drop procedure TmSp_IVIFillUXILM

CREATE procedure TmSp_IVIFillUXILM(out retCode int)
LANGUAGE SQLSCRIPT AS
ILMbaseTable nvarchar(4);
ILMbaseTableID nvarchar(8);

BEGIN
	DELETE FROM UXILM;
	
	SELECT "MsgSource" INTO ILMbaseTableID FROM CIVI ;
	/*
		UILM	10000162L	
		OILM	10000048L
	*/
	IF (:ILMbaseTableID='10000048' OR :ILMbaseTableID IS NULL) THEN 
		ILMbaseTable := 'OILM';
	ELSE IF (:ILMbaseTableID='10000162') then
		 ILMbaseTable := 'UILM';
	end if;
	end if;
	


	exec ('INSERT INTO UXILM  
             SELECT T0."MessageID", -1, -1, T0."DocEntry", T0."DocLineNum", T0."TransType" 
             FROM ' || :ILMbaseTable || ' T0
             WHERE T0."TransType"=202 AND T0."DocLineNum">-1 
                   AND NOT EXISTS(SELECT U0."LineNum" FROM  "WOR1" U0  WHERE U0."LineNum" = T0."DocLineNum"  AND  U0."DocEntry" = T0."DocEntry"  )  
                   AND NOT EXISTS(SELECT U0."LineNum" FROM  "UWOR1" U0  WHERE U0."LineNum" = T0."DocLineNum"  AND  U0."DocEntry" = T0."DocEntry"  )  				   
                   AND NOT EXISTS (SELECT U0."BaseLine" FROM  "IGE1" U0  WHERE U0."BaseType" = (202)  AND  U0."BaseEntry" = T0."DocEntry"  AND  U0."BaseLine" = T0."DocLineNum"  )  
   		           AND NOT EXISTS (SELECT U0."BaseLine" FROM  "IGN1" U0  WHERE U0."BaseType" = (202)  AND  U0."BaseEntry" = T0."DocEntry"  AND  U0."BaseLine" = T0."DocLineNum"  )');


	exec ('UPDATE T0
           SET T0."MinMsgID"=(SELECT MIN(T1."MessageID") FROM  ' || :ILMbaseTable || ' T1 WHERE T1."DocLineNum"=T0."DocLineNum" AND T1."TransType"=202 AND T1."DocEntry"=T0."DocEntry") FROM UXILM T0');
	exec ('UPDATE T0
           SET T0."MaxMsgID"=(SELECT MAX(T1."MessageID") FROM  ' || :ILMbaseTable || ' T1 WHERE T1."DocLineNum"=T0."DocLineNum" AND T1."TransType"=202 AND T1."DocEntry"=T0."DocEntry") FROM UXILM T0');

	exec ('DELETE FROM UXILM T0
          WHERE EXISTS(SELECT T1."MessageID" 
                       FROM  '|| :ILMbaseTable ||' T1 
                       WHERE T1."TransType"=59 AND T1."BaseType"=202 AND  T1."BaseAbsEnt"=T0."DocEntry" AND 
                       T1."MessageID">=T0."MinMsgID" AND T1."MessageID"<=T0."MaxMsgID")');
   		           

	retCode:=0;
end


