USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_FilterEmployees
    @userId int = -1,
    @tempTable nvarchar(128) = N''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userEmpId int = ISNULL((SELECT TOP (1) empID FROM dbo.OHEM WHERE userId = @userId), -1);
    CREATE TABLE #result (EmpId int NOT NULL PRIMARY KEY);

    ;WITH route_tree AS
    (
        SELECT route.Id, route.RouteCode, route.SuperVisor, route.SalesAdmin, route.FatherId, 0 AS depth
        FROM dbo.APZ_D_STR1 route
        INNER JOIN dbo.APZ_D_OSTR header ON header.RouteCode = route.RouteCode AND header.Status = 'Y'
        WHERE route.SuperVisor = @userEmpId
           OR dbo.CompareSplit(CONVERT(nvarchar(20), @userEmpId), route.SalesAdmin, N';') = 1

        UNION ALL

        SELECT child.Id, child.RouteCode, child.SuperVisor, child.SalesAdmin, child.FatherId, parent.depth + 1
        FROM dbo.APZ_D_STR1 child
        INNER JOIN route_tree parent ON parent.Id = child.FatherId AND parent.RouteCode = child.RouteCode
        WHERE parent.depth < 100
    ), employee_candidates AS
    (
        SELECT assignment.EmpId
        FROM route_tree route
        INNER JOIN dbo.APZ_D_STR4 assignment ON assignment.Id = route.Id AND assignment.RouteCode = route.RouteCode
        UNION
        SELECT route.SuperVisor FROM route_tree route WHERE route.SuperVisor IS NOT NULL
        UNION
        SELECT TRY_CONVERT(int, split.DATA)
        FROM route_tree route
        CROSS APPLY dbo.Split(route.SalesAdmin, N';') split
        WHERE TRY_CONVERT(int, split.DATA) IS NOT NULL
        UNION
        SELECT @userEmpId WHERE @userEmpId >= 0
    )
    INSERT INTO #result (EmpId)
    SELECT DISTINCT EmpId FROM employee_candidates WHERE EmpId IS NOT NULL
    OPTION (MAXRECURSION 101);

    IF ISNULL(@tempTable, N'') = N''
        SELECT EmpId FROM #result ORDER BY EmpId;
    ELSE
    BEGIN
        IF LEFT(@tempTable, 1) <> N'#' OR @tempTable LIKE N'%[^#A-Za-z0-9_]%' ESCAPE N'\'
            THROW 50001, 'Invalid temporary table name.', 1;

        DECLARE @insertEmployees nvarchar(400) = N'INSERT INTO ' + QUOTENAME(@tempTable) + N' (EmpId) SELECT EmpId FROM #result;';
        EXEC sys.sp_executesql @insertEmployees;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_FilterStores
    @cardType nvarchar(50) = N'',
    @cardName nvarchar(254) = N'',
    @subType char(1),
    @saleUser nvarchar(50),
    @tempTable nvarchar(128) = N''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @fromDate date = DATEADD(day, -1, CONVERT(date, GETDATE()));
    DECLARE @toDate date = DATEADD(day, 1, CONVERT(date, GETDATE()));
    DECLARE @userId int = ISNULL((SELECT TOP (1) USERID FROM dbo.OUSR WHERE USER_CODE = @saleUser), 0);
    DECLARE @empID int = ISNULL((SELECT TOP (1) employee.empID FROM dbo.OHEM employee INNER JOIN dbo.OUSR users ON users.USERID = employee.userId WHERE users.USER_CODE = @saleUser), 0);
    DECLARE @doFilter char(1) = ISNULL((SELECT TOP (1) DoFilter FROM dbo.CINF), 'N');
    DECLARE @doMngMth char(1) = ISNULL((SELECT TOP (1) DoMngMth FROM dbo.OADM), '');
    DECLARE @authMode char(1) = ISNULL(NULLIF((SELECT TOP (1) AuthorizationMode FROM dbo.APZ_D_OAPC), ''), 'R');

    CREATE TABLE #tmpEmployees (EmpId int);
    EXEC dbo.APZ_D_FilterEmployees @userId, N'#tmpEmployees';

    CREATE TABLE #routeCards (CardCode nvarchar(15) COLLATE SQL_Latin1_General_CP850_CI_AS PRIMARY KEY);
    ;WITH route_tree AS
    (
        SELECT route.Id, route.RouteCode, route.FatherId, 0 AS depth
        FROM dbo.APZ_D_STR1 route
        INNER JOIN dbo.APZ_D_OSTR header ON header.RouteCode = route.RouteCode AND header.Status = 'Y'
        WHERE route.Status = 'Y'
          AND (route.SuperVisor = @empID OR dbo.CompareSplit(CONVERT(nvarchar(20), @empID), route.SalesAdmin, N';') = 1)

        UNION ALL

        SELECT child.Id, child.RouteCode, child.FatherId, parent.depth + 1
        FROM dbo.APZ_D_STR1 child
        INNER JOIN route_tree parent ON parent.Id = child.FatherId AND parent.RouteCode = child.RouteCode
        WHERE child.Status = 'Y' AND parent.depth < 100
    )
    INSERT INTO #routeCards (CardCode)
    SELECT DISTINCT assignment.CardCode
    FROM dbo.APZ_D_STR2 assignment
    INNER JOIN route_tree route ON route.Id = assignment.Id AND route.RouteCode = assignment.RouteCode
    INNER JOIN dbo.APZ_D_STR4 employee ON employee.Id = assignment.Id AND employee.RouteCode = assignment.RouteCode AND employee.EmpId = assignment.EmpId
    WHERE assignment.CardCode IS NOT NULL
    OPTION (MAXRECURSION 101);

    INSERT INTO #routeCards (CardCode)
    SELECT DISTINCT assignment.CardCode
    FROM dbo.APZ_D_STR2 assignment
    INNER JOIN dbo.APZ_D_STR1 route ON route.Id = assignment.Id AND route.RouteCode = assignment.RouteCode
    INNER JOIN dbo.APZ_D_STR4 employee ON employee.Id = assignment.Id AND employee.RouteCode = assignment.RouteCode AND employee.EmpId = assignment.EmpId
    WHERE assignment.EmpId = @empID AND assignment.CardCode IS NOT NULL
      AND NOT EXISTS (SELECT 1 FROM #routeCards existing WHERE existing.CardCode = assignment.CardCode);

    CREATE TABLE #result
    (
        CardCode nvarchar(15) COLLATE SQL_Latin1_General_CP850_CI_AS,
        CardName nvarchar(100) COLLATE SQL_Latin1_General_CP850_CI_AS
    );
    INSERT INTO #result (CardCode, CardName)
    SELECT partner.CardCode, partner.CardName
    FROM dbo.OCRD partner
    WHERE (ISNULL(@subType, '') = '' OR partner.U_SubType = @subType)
      AND (ISNULL(@cardName, N'') = N'' OR partner.CardName LIKE N'%' + @cardName + N'%')
      AND (ISNULL(@cardType, N'') = N'' OR ISNULL(partner.CardType, '') = @cardType)
      AND
      (
          (@authMode = 'R' AND
              (partner.CardCode IN (SELECT CardCode FROM #routeCards)
               OR (ISNULL(@saleUser, N'') = N''
                   OR (partner.UserSign IN (SELECT employee.userId FROM dbo.OHEM employee INNER JOIN #tmpEmployees allowed ON allowed.EmpId = employee.empID)
                       AND NOT EXISTS (SELECT 1 FROM dbo.APZ_D_STR2 assigned WHERE assigned.CardCode = partner.CardCode)))))
          OR
          (@authMode = 'O' AND
              (@doFilter = 'N'
               OR (@doFilter = 'Y' AND @doMngMth NOT IN ('A', 'B'))
               OR (@doFilter = 'Y' AND @doMngMth IN ('A', 'B')
                   AND (ISNULL(partner.OwnerCode, -1) = -1
                        OR partner.OwnerCode IN (SELECT OwnerId FROM dbo.apz_func_OwnerOverShip(@saleUser, N'2'))))))
      )
      AND
      (
          (partner.validFor = 'Y' AND ISNULL(partner.frozenFor, 'N') = 'N'
             AND ISNULL(partner.validFrom, @fromDate) <= CONVERT(date, GETDATE())
             AND ISNULL(partner.validTo, @toDate) >= CONVERT(date, GETDATE()))
          OR (ISNULL(partner.validFor, 'N') = 'N' AND partner.frozenFor = 'Y'
             AND (ISNULL(partner.frozenFrom, @fromDate) > CONVERT(date, GETDATE()) OR ISNULL(partner.frozenTo, @toDate) < CONVERT(date, GETDATE())))
          OR (partner.validFor = 'Y' AND partner.frozenFor = 'Y'
             AND ISNULL(partner.validFrom, @fromDate) <= CONVERT(date, GETDATE())
             AND ISNULL(partner.validTo, @toDate) >= CONVERT(date, GETDATE())
             AND (ISNULL(partner.frozenFrom, @fromDate) > CONVERT(date, GETDATE()) OR ISNULL(partner.frozenTo, @toDate) < CONVERT(date, GETDATE())))
          OR (partner.validFor = 'N' AND partner.frozenFor = 'N')
      );

    IF ISNULL(@tempTable, N'') = N''
        SELECT CardCode, CardName FROM #result;
    ELSE
    BEGIN
        IF LEFT(@tempTable, 1) <> N'#' OR @tempTable LIKE N'%[^#A-Za-z0-9_]%' ESCAPE N'\'
            THROW 50001, 'Invalid temporary table name.', 1;

        DECLARE @insertStores nvarchar(400) = N'INSERT INTO ' + QUOTENAME(@tempTable) + N' (CardCode, CardName) SELECT CardCode, CardName FROM #result;';
        EXEC sys.sp_executesql @insertStores;
    END;
END;
GO
