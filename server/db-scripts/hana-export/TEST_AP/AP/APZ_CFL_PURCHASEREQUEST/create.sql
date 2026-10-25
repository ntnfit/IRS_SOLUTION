
CREATE PROCEDURE APZ_CFL_PurchaseRequest
AS
BEGIN
	select "DocEntry", "DocNum", cast("DocDate" as  date) as "DocDate",
        CASE WHEN "ReqType" = 12 then 'User'
             WHEN "ReqType" = 171 then 'Employee'
        END AS "ReqTypeName"
         , "ReqName", "Comments", cast("DocDueDate" as date) as "DocDueDate" from OPRQ;
END




