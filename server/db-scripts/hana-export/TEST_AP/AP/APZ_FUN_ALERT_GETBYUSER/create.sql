
-- apz_alert_getbyuser 'manager',0

CREATE function apz_fun_alert_getbyuser(in UserName VARCHAR(100), in MailType int)
 returns table ("Priority"		char(1)
	            ,"Attachment"	nchar(1)
	            ,"Code"			int	
				,"Name"			NVARCHAR(254)	
	            ,"UserSign"		INT		
	            ,"FromUser"		nvarchar(50)	
	            ,"QString"		Nclob	
	            ,"Opened"		nchar(1)			
	            ,"WasRead"		nchar(1)				
	            ,"Deleted"		nchar(1)			
	            ,"Failed"		nchar(1)			
	            ,"Received"		timestamp)
AS
BEGIN
	DECLARE hhmm int;
	select cast(TO_VARCHAR(current_date, 'hhmm') as integer) into hhmm from DUMMY;
	return SELECT cast(T0."Priority" as VARCHAR) "Priority",'0' "Attachment",T0."Code",T0."Name",T0."UserSign",T5.USER_CODE "FromUser", T4."QString",IFNULL(T6."Opened",'N') "Opened", IFNULL("WasRead",'N') "WasRead",
			IFNULL(T6."Deleted",'N') "Deleted",ifnull(T6."Failed",'N') "Failed",
			cast((cast(ifnull(t6."RecDate",current_date) as DATE) || ' ' || SUBSTRING(cast((10000+IFNULL(t6."RecTime",:hhmm)) as VARCHAR),2,2) ||':' || RIGHT(cast((10000+IFNULL(t6."RecTime",:hhmm)) as VARCHAR),2)) as timestamp) "Received"
		FROM oalt T0 
		--INNER JOIN OALR T1 ON t0."Code"=T1."TCode" AND T1."Type"='A'
		INNER JOIN alt1 T2 ON t0."Code"=T2."Code" AND t2."SendIntrnl"='Y'
		INNER JOIN ousr t3 ON t2."UserSign"=T3."USERID" AND T3.USER_CODE=:UserName 
		INNER JOIN ouqr t4 ON t0."QueryId"=T4."IntrnalKey"
		INNER JOIN ousr t5 ON T0."UserSign"=t5.USERID
		LEFT JOIN oaib T6 ON T0."Code"=T6."AlertCode" AND T2."UserSign"=t6."UserSign" 
		WHERE t0."Active"='Y' and :MailType=0;
END



