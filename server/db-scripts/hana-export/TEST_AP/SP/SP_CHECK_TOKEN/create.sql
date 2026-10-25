
CREATE PROCEDURE SP_CHECK_TOKEN
(in EmpId int)
AS
    BEGIN
        SELECT t."EmpId", t."TokenStr"
            FROM APZ_OHET t
            INNER JOIN OHEM m ON t."EmpId" = m."empID"
            WHERE t."EmpId" = :EmpId;

    END




