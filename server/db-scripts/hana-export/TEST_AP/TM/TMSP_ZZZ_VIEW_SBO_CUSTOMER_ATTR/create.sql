-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_CUSTOMER_ATTR
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_CUSTOMER_ATTR' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_CUSTOMER_ATTR');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_CUSTOMER_ATTR
				AS
				SELECT TO_CHAR("CardCode") AS "CARDCODE", "City" AS "CITY", "Country" AS "COUNTRY", "Fax" AS "FAX", 
      			"CardName" AS "CARDNAME", "Phone1" AS "PHONE1", "ZipCode" AS "ZIPCODE", 
      			"Address" AS "ADDRESS",
      			TO_CHAR(YEAR("CreateDate"))
       			|| case LENGTH(MONTH("CreateDate"))
          		when 1 then ''0'' || TO_CHAR(MONTH("CreateDate"))
          		else TO_CHAR(MONTH("CreateDate"))
         		end
      			|| case LENGTH(EXTRACT(DAY FROM "CreateDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM "CreateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM "CreateDate"))
        		end  AS "CREATEDATE",
      			TO_CHAR(YEAR("UpdateDate"))
       			|| case LENGTH(MONTH("UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(MONTH("UpdateDate"))
          		else TO_CHAR(MONTH("UpdateDate"))
         		end
      			|| case LENGTH(EXTRACT(DAY FROM "UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM "UpdateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM "UpdateDate"))
        		end  AS "UPDATEDATE"
				FROM OCRD
				WHERE "CardType" = ''C''';
	exec (:sqlStr);
END;


