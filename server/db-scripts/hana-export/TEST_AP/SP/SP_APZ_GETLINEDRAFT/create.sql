
create PROCEDURE SP_APZ_GetLineDraft(in docEntry int)
AS 
BEGIN
select "PQTReqDate" as "PQTReqDate", * from DRF1 WHERE  "DocEntry" = :docEntry;
END;






