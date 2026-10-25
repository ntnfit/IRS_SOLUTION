
CREATE  PROCEDURE APZ_RetailStore_GetDoc
(StoreCode nvarchar(100) default '')
AS
BEGIN
	select * from APZ_STORE where ifnull(:StoreCode,'') = '' or "Code" = :StoreCode;
END




