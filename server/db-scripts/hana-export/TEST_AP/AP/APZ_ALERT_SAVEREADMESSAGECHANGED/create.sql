
CREATE PROCEDURE apz_alert_SaveReadMessageChanged(in Code INT, in wasread nvarchar(50), in userName nvarchar(50))
AS
BEGIN
	DECLARE RecTime nvarchar(50)= to_char(current_date, 'HHmm');

    IF :Code = -1 then
    	UPDATE T0 SET "WasRead" = 'Y' FROM OAIB T0 JOIN OUSR T1 ON T0."UserSign"=T1.USERID JOIN ALR3 T2 ON T2."Code"=T0."AlertCode"
        WHERE T1.USER_CODE=:UserName AND T2."ObjType" in ('112','122') AND T0."WasRead" = 'N';
    elseIF exists(select 1 FROM OAIB T0
                INNER JOIN ousr t1 ON t0."UserSign"=T1."USERID" WHERE "AlertCode"=:Code AND T1."USER_CODE"=:UserName) THEN
	    UPDATE T0 SET "WasRead"=:WasRead
	                FROM OAIB T0
	                INNER JOIN ousr t1 ON t0."UserSign"=T1."USERID"
	                WHERE "AlertCode"=:Code AND T1."USER_CODE"=:UserName;

    ELSE
	   INSERT INTO OAIB( "AlertCode" ,
                          "UserSign" ,
                          "Opened" ,
                          "RecDate" ,
                          "RecTime" ,
                          "WasRead" ,
                          "Deleted" ,
                          "Failed"
	                    )
	       SELECT :Code,t1.USERID,'Y',current_date,:RecTime,:WasRead,'N','N'
	                FROM OUSR T1 WHERE T1.USER_CODE=:UserName;
    END IF;
	
end



