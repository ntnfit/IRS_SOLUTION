
CREATE  procedure apz_func_OwnerOverShip(in UserName nvarchar(100), in ObjType NVARCHAR(100), 
out result table(
		"UserId" INT,
		"UserCode" NVARCHAR(40),
		"OwnerId" INT,
		"SlpId" INT,
		"ObjType" NVARCHAR(20),
		"Authorize" NVARCHAR(1)
		))

AS
BEGIN
declare dofilter char(1);
DECLARE supper CHAR(1);
DECLARE emp INT;
DECLARE usermanager INT;
DECLARE userId INT;
DECLARE UserDept INT;
DECLARE UserBranch INT;
DECLARE peer CHAR(1);
DECLARE AutManager CHAR(1);
DECLARE Subordinate CHAR(1);
-- Department
DECLARE department CHAR(1);
-- Branch
DECLARE branch CHAR(1);
-- Team
DECLARE Team CHAR(1);
declare company char(1);

declare tbl_funOwn table
(
	"UserId" INT,
	"UserCode" NVARCHAR(40),
	"OwnerId" INT,
	"SlpId" INT,
	"ObjType" NVARCHAR(20),
	"Authorize" NVARCHAR(1)
);

declare tblemp table
(
	"EmpId" INT,
	"UserId" INT,
	"SlpId" INT,
	"Manager" INT,
	"Supper" CHAR(1),
	"Dept" INT,
	"Branch" INT
);
	
select iFnull((select "DoFilter" from cinf),'N') into dofilter from DUMMY;

if :dofilter='N'
then

	INSERT INTO :tbl_funOwn ( "UserId", "UserCode", "OwnerId", "SlpId", "ObjType","Authorize")
	SELECT T0."USERID",T0."USER_CODE",t1."empID",t1."salesPrson",:ObjType,'F' FROM ousr T0 LEFT JOIN  OHEM t1 ON t0."USERID"=t1."userId";

	INSERT INTO :tbl_funOwn ("OwnerId", "SlpId", "ObjType", "Authorize") 
	SELECT t1."empID", t1."salesPrson", :ObjType, 'F' FROM OHEM T1 WHERE T1."userId" IS NULL;
	result=select * from :tbl_funOwn;
	RETURN;
	
end if;
	
	INSERT INTO :tblemp ("EmpId", "UserId", "SlpId", "Manager", "Supper", "Dept", "Branch") 
	SELECT a."empID", a."userId", a."salesPrson", a."manager", b.SUPERUSER, a."dept", a."branch" FROM ohem a INNER JOIN ousr b ON a."userId" = b.USERID WHERE a."userId" = (SELECT TOP 1 USERID FROM ousr WHERE USER_CODE = :UserName);

	select IFNULL((SELECT TOP 1 SUPERUSER FROM OUSR WHERE USER_CODE=:UserName),'N') into supper from DUMMY;
	IF :supper='Y'
	then
		INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
		SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, 'F' FROM OHEM a LEFT OUTER JOIN ousr b ON a."userId" = b.USERID;
    ELSE
		-- Lấy thông tin quyền của User này
		-- Có quyền của Owner chính user
		INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
		SELECT a."userId", :UserName, a."empID", a."salesPrson", :ObjType, 'F' FROM ohem a WHERE a."userId" = (SELECT TOP 1 USERID FROM ousr WHERE USER_CODE = :UserName);

		select (SELECT "EmpId" FROM :tblemp) into emp from DUMMY;
		select (SELECT "Manager" FROM :tblemp) into usermanager from DUMMY;
		select(SELECT "UserId" FROM :tblemp) into userId from DUMMY;
		select (SELECT "Dept" FROM :tblemp) into UserDept from DUMMY;
		select (SELECT "Branch" FROM :tblemp) into UserBranch from DUMMY;
		
		select IFNULL((SELECT "Peer" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into peer from DUMMY;
		IF :peer<>'N'
		then
			-- PEER Ngang hàng. Lấy ra các owner mà có cùng manager	
			INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			SELECT a."userId", c.USER_CODE, a."empID", a."empID", :ObjType, :peer FROM ohem a LEFT JOIN ousr c ON a."userId" = c.USERID WHERE a."manager" = :usermanager AND a."empID" <> :emp;
			
        END if;
		
		select IFNULL((SELECT "Manager" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into AutManager from DUMMY;
		IF :AutManager<>'N'
		then
		
			INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			SELECT a."userId", c.USER_CODE, a."empID", a."salesPrson", :ObjType, :AutManager FROM ohem a LEFT JOIN ousr c ON a."userId" = c.USERID WHERE a."empID" = :usermanager;

        END	if;
		
		select IFNULL((SELECT "Subord" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into Subordinate from DUMMY;
		IF :Subordinate<>'N'
		then
		
			INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, :Subordinate FROM ohem a LEFT OUTER JOIN ousr b ON a."userId" = b.USERID WHERE a."manager" = :emp;

        END if;
        
		select IFNULL((SELECT "Dept" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into department from DUMMY;
		IF :department<>'N'
		then
			 INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			 SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, :department FROM ohem a LEFT OUTER JOIN ousr b ON a."userId" = b.USERID WHERE a."dept" = :UserDept;

        END if;
        
		select IFNULL((SELECT "Branch" FROM hem5 WHERE "empID"=:emp AND "Object"= :ObjType),'N') into branch from DUMMY;
		IF :branch<>'N'
		then
		
			 INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			 SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, :branch FROM ohem a LEFT OUTER JOIN ousr b ON a."userId" = b.USERID WHERE a."branch" = :UserBranch;

        END if;

		select IFNULL((SELECT "Team" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into Team from DUMMY;
		
		IF	:Team<>'N'
		then
			INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, :Team FROM ohem a LEFT JOIN ousr b ON a."userId" = b.USERID 
						WHERE a."empID" IN (SELECT y."empID" FROM htm1 x INNER JOIN htm1 y ON x."teamID" = y."teamID" WHERE x."empID" = :emp);

        END if;
        select IFNULL((SELECT "Cmpny" FROM hem5 WHERE "empID"=:emp AND "Object"=:ObjType),'N') into company from DUMMY;
        IF	:company<>'N'
		then
			INSERT INTO :tbl_funOwn ("UserId", "UserCode", "OwnerId", "SlpId", "ObjType", "Authorize") 
			SELECT a."userId", b.USER_CODE, a."empID", a."salesPrson", :ObjType, :company FROM ohem a LEFT JOIN ousr b ON a."userId" = b.USERID ;

        END if;
        
	END	if;
	
	delete from :tbl_funOwn where "Authorize" <> 'F' and "OwnerId" in (select distinct "OwnerId" from :tbl_funOwn where "Authorize" = 'F');
	
	result = select distinct * from :tbl_funOwn;

	return;
END

