-- HANA: procedure apz_func_OwnerOverShip(UserName, ObjType, OUT result TABLE)
-- T-SQL: table-valued function (the SQL Server callers use it as apz_func_OwnerOverShip(@user, objType))
-- Returns the owners (employees) whose documents the user may see for an object type, from CINF.DoFilter and HEM5.
CREATE OR ALTER FUNCTION dbo.apz_func_OwnerOverShip (@UserName nvarchar(100), @ObjType nvarchar(100))
RETURNS @result TABLE (UserId int, UserCode nvarchar(40), OwnerId int, SlpId int, ObjType nvarchar(20), Authorize nvarchar(1))
AS
BEGIN
    DECLARE @own TABLE (UserId int, UserCode nvarchar(40), OwnerId int, SlpId int, ObjType nvarchar(20), Authorize nvarchar(1));
    DECLARE @dofilter char(1) = ISNULL((SELECT TOP 1 DoFilter FROM CINF), 'N');

    IF @dofilter = 'N'
    BEGIN
        INSERT INTO @result (UserId, UserCode, OwnerId, SlpId, ObjType, Authorize)
        SELECT t0.USERID, t0.USER_CODE, t1.empID, t1.salesPrson, @ObjType, 'F'
        FROM OUSR t0 LEFT JOIN OHEM t1 ON t0.USERID = t1.userId;
        INSERT INTO @result (OwnerId, SlpId, ObjType, Authorize)
        SELECT t1.empID, t1.salesPrson, @ObjType, 'F' FROM OHEM t1 WHERE t1.userId IS NULL;
        RETURN;
    END;

    DECLARE @userIdOfName int = (SELECT TOP 1 USERID FROM OUSR WHERE USER_CODE = @UserName);
    DECLARE @supper char(1) = ISNULL((SELECT TOP 1 SUPERUSER FROM OUSR WHERE USER_CODE = @UserName), 'N');

    IF @supper = 'Y'
        INSERT INTO @own (UserId, UserCode, OwnerId, SlpId, ObjType, Authorize)
        SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, 'F' FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID;
    ELSE
    BEGIN
        -- owners = the user himself
        INSERT INTO @own (UserId, UserCode, OwnerId, SlpId, ObjType, Authorize)
        SELECT a.userId, @UserName, a.empID, a.salesPrson, @ObjType, 'F' FROM OHEM a WHERE a.userId = @userIdOfName;

        DECLARE @emp int, @usermanager int, @UserDept int, @UserBranch int;
        SELECT TOP 1 @emp = a.empID, @usermanager = a.manager, @UserDept = a.dept, @UserBranch = a.branch
        FROM OHEM a INNER JOIN OUSR b ON a.userId = b.USERID WHERE a.userId = @userIdOfName;

        DECLARE @peer char(1), @autManager char(1), @subord char(1), @department char(1), @branch char(1), @team char(1), @company char(1);
        SELECT @peer = ISNULL(Peer, 'N'), @autManager = ISNULL(Manager, 'N'), @subord = ISNULL(Subord, 'N'),
               @department = ISNULL(Dept, 'N'), @branch = ISNULL(Branch, 'N'), @team = ISNULL(Team, 'N'), @company = ISNULL(Cmpny, 'N')
        FROM HEM5 WHERE empID = @emp AND [Object] = @ObjType;
        SELECT @peer = ISNULL(@peer, 'N'), @autManager = ISNULL(@autManager, 'N'), @subord = ISNULL(@subord, 'N'),
               @department = ISNULL(@department, 'N'), @branch = ISNULL(@branch, 'N'), @team = ISNULL(@team, 'N'), @company = ISNULL(@company, 'N');

        IF @peer <> 'N'        -- peers: same manager (HANA puts empID into SlpId here; kept)
            INSERT INTO @own SELECT a.userId, c.USER_CODE, a.empID, a.empID, @ObjType, @peer
            FROM OHEM a LEFT JOIN OUSR c ON a.userId = c.USERID WHERE a.manager = @usermanager AND a.empID <> @emp;
        IF @autManager <> 'N'  -- the user's manager
            INSERT INTO @own SELECT a.userId, c.USER_CODE, a.empID, a.salesPrson, @ObjType, @autManager
            FROM OHEM a LEFT JOIN OUSR c ON a.userId = c.USERID WHERE a.empID = @usermanager;
        IF @subord <> 'N'      -- direct subordinates
            INSERT INTO @own SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, @subord
            FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID WHERE a.manager = @emp;
        IF @department <> 'N'
            INSERT INTO @own SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, @department
            FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID WHERE a.dept = @UserDept;
        IF @branch <> 'N'
            INSERT INTO @own SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, @branch
            FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID WHERE a.branch = @UserBranch;
        IF @team <> 'N'
            INSERT INTO @own SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, @team
            FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID
            WHERE a.empID IN (SELECT y.empID FROM HTM1 x INNER JOIN HTM1 y ON x.teamID = y.teamID WHERE x.empID = @emp);
        IF @company <> 'N'
            INSERT INTO @own SELECT a.userId, b.USER_CODE, a.empID, a.salesPrson, @ObjType, @company
            FROM OHEM a LEFT JOIN OUSR b ON a.userId = b.USERID;
    END;

    -- a full ('F') grant wins over partial grants for the same owner
    DELETE FROM @own WHERE Authorize <> 'F' AND OwnerId IN (SELECT OwnerId FROM @own WHERE Authorize = 'F');
    INSERT INTO @result SELECT DISTINCT * FROM @own;
    RETURN;
END;
