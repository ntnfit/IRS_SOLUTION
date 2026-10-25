

CREATE PROCEDURE APZ_Coupon_GetAllCoupon
AS
BEGIN
	
	select a.*, b."ItemName" as "ItemName" from APZ_COUPON a left join OITM b on a."ItemCode" = b."ItemCode";
	
END;



