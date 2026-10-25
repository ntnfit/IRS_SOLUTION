

CREATE PROCEDURE APZ_D_Store_GetRevenue
(
	IN cardCode nvarchar(15),
	IN userCode nvarchar(50)
)
AS
BEGIN

	DECLARE storelist ParaStrListType;
  	DECLARE tblValue TABLE 
	(
		"ItemTotal" nvarchar(30), "ItemPerOrder" nvarchar(30), "RevenuePerOrder" nvarchar(30), "TotalRevenue" nvarchar(30), 
		"NumberStore" nvarchar(30), "NumberStoreNew" nvarchar(30), "NumberVisit" nvarchar(30), "NumberVisitHaveOrder" nvarchar(30), 
		"NumberOrder" nvarchar(30), "NumberOrderOpen" nvarchar(30), "DateType" char(1)
	);
	  
	INSERT INTO :storelist("Code") select :cardCode FROM DUMMY;
	 
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from  apz_d_home_summary(:userCode,'W',:storelist);
	
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from  apz_d_home_summary(:userCode,'M',:storelist);
	
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from  apz_d_home_summary(:userCode,'Y',:storelist);

	select * from :tblValue;
	
END




