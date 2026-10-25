CREATE PROCEDURE ILD_TRUYXUATSSCCFATTHER1
(	
	IN Input_DistNumber TABLE (
        "DistNumber" VARCHAR(36)
    ),
     
    IN pRelationship varchar(1) default 'N',
    
	OUT result_table TABLE 
	(
	 	sscc1 VARCHAR(36),
	 	InDate1 date,
	 	sscc2 VARCHAR(36),
	 	InDate2 date,
	 	sscc3 VARCHAR(36),
	 	InDate3 date,
	 	sscc4 VARCHAR(36),
	 	InDate4 date,
	 	sscc5 VARCHAR(36),
	 	InDate5 date,
	 	sscc6 VARCHAR(36),
	 	InDate6 date
	)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
 	DECLARE tmpfather TABLE (
        SSCC VARCHAR(36),
        ORIBatch VARCHAR(36),
        InDate date,
        "level" INT
    ); 	
 	DECLARE tmp_ORIBatch TABLE (
        "U_ORIBatch" nvarchar(36)
    ); 
   
    DECLARE current_DistNumber TABLE (
        "DistNumber" nvarchar(36)
        
    );
    DECLARE ssccmaping TABLE
 	(
	 	sscc1 VARCHAR(36),
	 	InDate1 date,
	 	sscc2 VARCHAR(36),
	 	InDate2 date,
	 	sscc3 VARCHAR(36),
	 	InDate3 date,
	 	sscc4 VARCHAR(36),
	 	InDate4 date,
	 	sscc5 VARCHAR(36),
	 	InDate5 date,
	 	sscc6 VARCHAR(36),
	 	InDate6 date
 	);

    DECLARE result_maping TABLE
 	(
	 	sscc1 VARCHAR(36),
	 	InDate1 date,
	 	sscc2 VARCHAR(36),
	 	InDate2 date,
	 	sscc3 VARCHAR(36),
	 	InDate3 date,
	 	sscc4 VARCHAR(36),
	 	InDate4 date,
	 	sscc5 VARCHAR(36),
	 	InDate5 date,
	 	sscc6 VARCHAR(36),
	 	InDate6 date
 	);
 	
    DECLARE current_level INT := 1;    
    declare ploop int = 7;
	
	insert into :current_DistNumber
	select "DistNumber" from :Input_DistNumber;
    insert into :tmp_ORIBatch
    select "DistNumber" from :Input_DistNumber ;

    --- vong lap
	 WHILE :current_level < ploop DO
	 	insert into :tmpfather
		SELECT OBTN."DistNumber",OBTN."U_ORIBatch",OBTN."InDate", :current_level as "level"
        FROM OBTN 
        inner join :current_DistNumber ccode on OBTN."DistNumber" = ccode."DistNumber" ;

	        
	 -- Lấy code tiếp theo (tham chieu của dòng hiện tại)
		delete from :current_DistNumber; -- xoa de insert dul ieu moi

		insert into :current_DistNumber
	    SELECT OBTN."U_ORIBatch"
	        FROM OBTN
	        inner join :tmp_ORIBatch tm on tm."U_ORIBatch" = OBTN."DistNumber"	        
	        WHERE tm."U_ORIBatch" <> '' and OBTN."U_ORIBatch" <> '';
    
	    delete from :tmp_ORIBatch;
	    insert into :tmp_ORIBatch
    	select "DistNumber" from :current_DistNumber ;
    	current_level := current_level + 1;
    	
    END WHILE;	---end loop   

	insert into :ssccmaping
	select 
		lv1.sscc as sscc1, lv1.InDate as  InDate1
		,lv2.sscc as sscc2, lv2.InDate as InDate2
		,lv3.sscc as sscc3, lv3.InDate as InDate3
		,lv4.sscc as sscc4, lv4.InDate as InDate4
		,lv5.sscc as sscc5, lv5.InDate as InDate5
		,lv6.sscc as sscc6, lv6.InDate as InDate6
	from (SELECT * FROM :tmpfather where "level" = 1) lv1
	left join (SELECT * FROM :tmpfather where "level" = 2) lv2 on lv2.sscc = lv1.oribatch
	left join (SELECT * FROM :tmpfather where "level" = 3) lv3 on lv3.sscc = lv2.oribatch
	left join (SELECT * FROM :tmpfather where "level" = 4) lv4 on lv4.sscc = lv3.oribatch
	left join (SELECT * FROM :tmpfather where "level" = 5) lv5 on lv5.sscc = lv4.oribatch
	left join (SELECT * FROM :tmpfather where "level" = 6) lv6 on lv6.sscc = lv5.oribatch ;
	
	IF :pRelationship = 'Y' THEN
		result_table = select distinct * from :ssccmaping;
	ELSE
		INSERT INTO :result_maping
		SELECT distinct SSCC1,InDate1,
		  CASE 
		    WHEN SSCC6 IS NOT NULL AND INDATE6 IS NOT NULL THEN SSCC6
		    WHEN SSCC5 IS NOT NULL AND INDATE5 IS NOT NULL THEN SSCC5
		    WHEN SSCC4 IS NOT NULL AND INDATE4 IS NOT NULL THEN SSCC4
		    WHEN SSCC3 IS NOT NULL AND INDATE3 IS NOT NULL THEN SSCC3
		    WHEN SSCC2 IS NOT NULL AND INDATE2 IS NOT NULL THEN SSCC2
		    ELSE SSCC1 
		  END AS SSCC2,
		  CASE 
		    WHEN SSCC6 IS NOT NULL AND INDATE6 IS NOT NULL THEN InDate6
		    WHEN SSCC5 IS NOT NULL AND INDATE5 IS NOT NULL THEN InDate5
		    WHEN SSCC4 IS NOT NULL AND INDATE4 IS NOT NULL THEN InDate4
		    WHEN SSCC3 IS NOT NULL AND INDATE3 IS NOT NULL THEN InDate3
		    WHEN SSCC2 IS NOT NULL AND INDATE2 IS NOT NULL THEN InDate2
		    ELSE InDate1 
		  END AS InDate2,NULL AS S3,NULL AS S4,NULL AS S5,NULL AS S6,NULL AS S7,NULL AS S8,NULL AS S9,NULL AS S10
		FROM :ssccmaping ;
		result_table = SELECT * FROM :result_maping ;
		
	END IF ;
	
END;
