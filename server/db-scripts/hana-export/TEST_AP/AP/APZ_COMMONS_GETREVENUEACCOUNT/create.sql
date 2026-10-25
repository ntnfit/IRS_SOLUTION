
CREATE PROCEDURE APZ_Commons_GetRevenueAccount
AS
BEGIN
	select * from OACT where "GroupMask" = 4 and "Postable" = 'Y';
END




