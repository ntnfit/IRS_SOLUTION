
CREATE PROCEDURE APZ_Employee_All
AS
BEGIN
	
	select "empID", IFNULL("firstName", '') || ' ' || IFNULL("lastName", '') || ' ' || IFNULL("middleName", '') AS "FullName", b."Name" as "Deparment",d."Name" "Branch", a."mobile",a."userId", c.SUPERUSER,c.U_NAME,C.USER_CODE
	from OHEM a 
	left join OUSR c on a."userId" = c.USERID
	left join OUDP b ON a."dept" = b."Code"
	left join oubr d on  a."branch"=d."Code";
	
END;



