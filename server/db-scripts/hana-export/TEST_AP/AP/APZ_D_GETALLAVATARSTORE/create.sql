

CREATE PROCEDURE APZ_D_GetAllAvatarStore (IN saleUser nvarchar(50))
AS
BEGIN

	--param from store APZ_D_GetListStore
	declare subType char(1)='S';
	declare cardName nvarchar(100)= '';
	declare cardType nvarchar(50)='';
	declare fromDate date;
	declare toDate date;
	declare empID int;
	
	select add_days(current_date,-1) into fromDate from dummy;
	select add_days(current_date,1) into toDate from dummy;
	
	-- get Stores by sales route
	create local temporary table #tmpStore ("CardCode" nvarchar(15), "CardName" nvarchar(100));
	CALL APZ_D_FilterStores ('','','S',:saleUser, N'#tmpStore');
	
	empID := IFNULL((SELECT TOP 1 "salesPrson" FROM OHEM a LEFT OUTER JOIN OUSR b ON a."userId" = b.USERID WHERE b.USER_CODE = :saleUser), 0);

	SELECT B.* FROM OCRD A INNER JOIN APZ_D_UPIM B ON A."CardCode" = B."CardCode" AND B."IsAvatar" = 'Y' WHERE A."CardCode" IN (SELECT "CardCode" FROM #tmpStore);
	
END




