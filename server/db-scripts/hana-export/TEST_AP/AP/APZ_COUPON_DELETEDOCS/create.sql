

CREATE PROCEDURE APZ_Coupon_DeleteDocs(in code nvarchar(50) default '')
AS
BEGIN
	delete from APZ_COUPON1 where "CouponCode" in (:code);
	delete from APZ_COUPON where "CouponCode" in (:code);
END;





