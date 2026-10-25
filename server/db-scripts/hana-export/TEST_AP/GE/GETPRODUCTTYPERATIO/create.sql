CREATE  FUNCTION GetProductTypeRatio (
	period 	nvarchar(20)
)
RETURNS TABLE (
	"ItemCode" nvarchar(50),
	"Code" nvarchar(50),
	"Name" NVARCHAR(100),
	"LabourRatio" DECIMAL(21,6),
	"OverheadRatio" DECIMAL(21,6)
)
AS
BEGIN
	declare ProductTypeRatio TABLE 
	(
	"ItemCode" nvarchar(50),
	"Code" nvarchar(50),
	"Name" NVARCHAR(100),
	"LabourRatio" DECIMAL(21,6),
	"OverheadRatio" DECIMAL(21,6)
	);
	
	insert into :ProductTypeRatio("ItemCode" ,"Code" ,"Name" ,"LabourRatio","OverheadRatio")
	select 
		tm."ItemCode", pr."Code", pr."Name"
		,pr."U_LabourRatio" "LabourRatio"
		,pr."U_OverheadRatio" "OverheadRatio"
	from OITM tm	
	left join "@PRODUCT_TYPE" pr on pr."Code" = tm."U_ProductType"
	where 
		pr."U_LabourRatio" is not null and pr."U_OverheadRatio" is not null;
	
	return :ProductTypeRatio;
END
