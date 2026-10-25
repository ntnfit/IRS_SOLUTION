-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE sp_procedure_params_rowset 
      (IN schema_name NVARCHAR(128),
      IN procedure_name NVARCHAR(128)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
READS SQL DATA AS 
BEGIN 
      SELECT s_pp."SCHEMA_NAME", s_pp."PROCEDURE_NAME", s_pp."PARAMETER_NAME", s_pp."DATA_TYPE_ID", s_pp."DATA_TYPE_NAME", s_pp."LENGTH", s_pp."PARAMETER_TYPE"
      FROM "PUBLIC"."PROCEDURE_PARAMETERS" s_pp 
      WHERE s_pp."SCHEMA_NAME" = :schema_name 
      AND s_pp."PROCEDURE_NAME" = :procedure_name;
END;


