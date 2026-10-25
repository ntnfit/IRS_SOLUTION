
CREATE PROCEDURE Apz_QueriesCategory_Remove
(in categoryID int)
as
BEGIN
declare QName nvarchar(100);
select IFNULL((Select TOP 1 "QName" From OUQR WHERE "QCategory" = :categoryID),'') into QName from Dummy;
if (ltrim(rtrim(:QName))='' AND :categoryID != 0) /* Query name from OUQR */
	then
		DELETE FROM OQCN WHERE "CategoryId" = :categoryID;
	
else	
	SELECT  
		1 ErrorNumber
		,N'Can not delete cause its queries are in used!' ErrorMessage from Dummy;
		return;
	end if;
END




