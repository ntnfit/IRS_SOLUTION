

CREATE PROCEDURE APZ_Coupon_GetDetailByCode(in code nvarchar(50) default '')
AS
BEGIN

	select a.*,b."CardName" as "BPName" from APZ_COUPON1 a left join  OCRD b on b."CardCode" = a."BPCode" where a."CouponCode" = :code;
	
END;



