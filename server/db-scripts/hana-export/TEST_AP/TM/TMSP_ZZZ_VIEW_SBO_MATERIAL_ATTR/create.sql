-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_ZZZ_VIEW_SBO_MATERIAL_ATTR
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
cnt int;
sqlStr nvarchar(4000);
BEGIN
	select count (*) into cnt from "PUBLIC"."VIEWS" where "VIEW_NAME" = 'SBO_MATERIAL_ATTR' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :cnt > 0 then
		sqlStr := ('drop view SBO_MATERIAL_ATTR');
		exec (:sqlStr);
	end if;
	
	sqlStr := 'CREATE VIEW SBO_MATERIAL_ATTR
				AS
				SELECT "ItemCode" AS "ITEMCODE", 
      			TO_CHAR(YEAR("CreateDate"))
       				|| case LENGTH(MONTH("CreateDate"))
          		when 1 then ''0'' || TO_CHAR(month("CreateDate"))
          		else TO_CHAR(month("CreateDate"))
         		end
      				|| case LENGTH(EXTRACT(DAY FROM "CreateDate"))
         		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM "CreateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM "CreateDate"))
        		end  AS CREATEDATE,
      			TO_CHAR(YEAR("UpdateDate"))
       				|| case LENGTH(MONTH("UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(month("UpdateDate"))
          		else TO_CHAR(month("UpdateDate"))
         		end
      			|| case LENGTH(EXTRACT(DAY FROM "UpdateDate"))
          		when 1 then ''0'' || TO_CHAR(EXTRACT(DAY FROM "UpdateDate"))
          		else TO_CHAR(EXTRACT(DAY FROM "UpdateDate"))
        		end  AS "UPDATEDATE"
				FROM OITM';
	exec (:sqlStr);
END;


