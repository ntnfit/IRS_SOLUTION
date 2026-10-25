USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Channel_GetData
    @chnCode nvarchar(50),
    @chnName nvarchar(50),
    @status varchar(1)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.APZ_D_OCHN
    WHERE (ISNULL(@chnCode, N'') = N'' OR ChannelCode LIKE @chnCode + N'%')
      AND (ISNULL(@chnName, N'') = N'' OR ChannelName LIKE @chnName + N'%')
      AND (ISNULL(@status, '') = '' OR Status LIKE @status + '%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Channel_GetRecord
    @posValue nvarchar(1),
    @channelCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @posValue NOT IN (N'F', N'L', N'P', N'N')
    BEGIN
        SELECT '01' AS ErrorCode, 'Invalid Navigation type' AS ErrorMessage;
        RETURN;
    END;

    ;WITH records AS
    (
        SELECT ROW_NUMBER() OVER (ORDER BY ChannelCode) AS RowNum, *
        FROM dbo.APZ_D_OCHN
    ), current_record AS
    (
        SELECT RowNum FROM records WHERE ChannelCode = @channelCode
    )
    SELECT TOP (1) records.*
    FROM records
    CROSS JOIN (SELECT MIN(RowNum) AS MinRowNum, MAX(RowNum) AS MaxRowNum FROM records) bounds
    OUTER APPLY (SELECT RowNum AS CurrentRowNum FROM current_record) current_row
    WHERE records.RowNum = CASE @posValue
        WHEN N'F' THEN bounds.MinRowNum
        WHEN N'L' THEN bounds.MaxRowNum
        WHEN N'P' THEN CASE
            WHEN ISNULL(current_row.CurrentRowNum, bounds.MinRowNum) <= bounds.MinRowNum THEN bounds.MaxRowNum
            ELSE (SELECT MAX(previous_record.RowNum) FROM records previous_record WHERE previous_record.RowNum < current_row.CurrentRowNum)
        END
        WHEN N'N' THEN CASE
            WHEN ISNULL(current_row.CurrentRowNum, bounds.MaxRowNum) >= bounds.MaxRowNum THEN bounds.MinRowNum
            ELSE (SELECT MIN(next_record.RowNum) FROM records next_record WHERE next_record.RowNum > current_row.CurrentRowNum)
        END
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SalesRoute_GetData
    @routeCode nvarchar(50),
    @routeName nvarchar(50),
    @status varchar(1)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.APZ_D_OSTR
    WHERE (ISNULL(@routeCode, N'') = N'' OR RouteCode LIKE @routeCode + N'%')
      AND (ISNULL(@routeName, N'') = N'' OR RouteName LIKE @routeName + N'%')
      AND (ISNULL(@status, '') = '' OR Status LIKE @status + '%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SalesRoute_GetRecord
    @posValue nvarchar(1),
    @routeCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @posValue NOT IN (N'F', N'L', N'P', N'N')
    BEGIN
        SELECT '01' AS ErrorCode, 'Invalid Navigation type' AS ErrorMessage;
        RETURN;
    END;

    ;WITH records AS
    (
        SELECT ROW_NUMBER() OVER (ORDER BY RouteCode) AS RowNum, *
        FROM dbo.APZ_D_OSTR
    ), current_record AS
    (
        SELECT RowNum FROM records WHERE RouteCode = @routeCode
    )
    SELECT TOP (1) records.*
    FROM records
    CROSS JOIN (SELECT MIN(RowNum) AS MinRowNum, MAX(RowNum) AS MaxRowNum FROM records) bounds
    OUTER APPLY (SELECT RowNum AS CurrentRowNum FROM current_record) current_row
    WHERE records.RowNum = CASE @posValue
        WHEN N'F' THEN bounds.MinRowNum
        WHEN N'L' THEN bounds.MaxRowNum
        WHEN N'P' THEN CASE
            WHEN ISNULL(current_row.CurrentRowNum, bounds.MinRowNum) <= bounds.MinRowNum THEN bounds.MaxRowNum
            ELSE (SELECT MAX(previous_record.RowNum) FROM records previous_record WHERE previous_record.RowNum < current_row.CurrentRowNum)
        END
        WHEN N'N' THEN CASE
            WHEN ISNULL(current_row.CurrentRowNum, bounds.MaxRowNum) >= bounds.MaxRowNum THEN bounds.MinRowNum
            ELSE (SELECT MIN(next_record.RowNum) FROM records next_record WHERE next_record.RowNum > current_row.CurrentRowNum)
        END
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Store_GetBusinessTypeByCardCode
    @cardCode nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH business_types AS
    (
        SELECT card.CardCode, card.CardName, groups.OrderId, groups.YesNo
        FROM dbo.OCRD card
        CROSS APPLY (VALUES
            (1, card.QryGroup1), (2, card.QryGroup2),
            (3, card.QryGroup3), (4, card.QryGroup4),
            (5, card.QryGroup5), (6, card.QryGroup6),
            (7, card.QryGroup7), (8, card.QryGroup8)
        ) groups(OrderId, YesNo)
        WHERE card.U_SubType = 'S'
          AND card.CardCode = @cardCode
    )
    SELECT selected.CardCode,
           selected.CardName,
           CAST('Y' AS char(1)) AS YesNo,
           STRING_AGG(CONVERT(nvarchar(max), property.GroupName), N', ') WITHIN GROUP (ORDER BY selected.OrderId) AS PropertiesName,
           STRING_AGG(CONVERT(nvarchar(max), selected.OrderId), N', ') WITHIN GROUP (ORDER BY selected.OrderId) AS PropertiesCode
    FROM business_types selected
    INNER JOIN dbo.OCQG property ON property.GroupCode = selected.OrderId
    WHERE selected.YesNo = 'Y'
    GROUP BY selected.CardCode, selected.CardName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetEmployeeRoute
    @empId int,
    @dateValue datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @empName nvarchar(254) = ISNULL((
        SELECT TOP (1) CONCAT(ISNULL(firstName, N''), N' ', ISNULL(middleName, N''), N' ', ISNULL(lastName, N''))
        FROM dbo.OHEM WHERE empID = @empId
    ), N'');
    DECLARE @targetDate date = CONVERT(date, ISNULL(@dateValue, GETDATE()));
    DECLARE @userId int = (SELECT TOP (1) userId FROM dbo.OHEM WHERE empID = @empId);

    ;WITH route_points AS
    (
        SELECT CONCAT(N'(', ISNULL(U_Latitude, N''), N':', ISNULL(U_Longitude, N''), N')') AS Geolocation,
               @empId AS empId, @empName AS empName, CardCode, CardName,
               CONVERT(datetime, CreateDate) AS CreateDate, CreateTS
        FROM dbo.OCRD
        WHERE UserSign = @userId AND CONVERT(date, CreateDate) = @targetDate

        UNION ALL

        SELECT detail.Geolocation, @empId, @empName, visit.StoreCode, visit.StoreName,
               CONVERT(datetime, detail.CreateDate),
               DATEPART(hour, detail.CreateTime) * 10000 + DATEPART(minute, detail.CreateTime) * 100 + DATEPART(second, detail.CreateTime)
        FROM dbo.APZ_D_VISIT visit
        INNER JOIN dbo.APZ_D_VISIT1 detail ON detail.DocEntry = visit.DocEntry AND detail.StepCode = -1
        WHERE visit.UserSign = @userId AND CONVERT(date, detail.CreateDate) = @targetDate
    )
    SELECT *, ROW_NUMBER() OVER (ORDER BY CreateTS) AS RowNum
    FROM route_points
    ORDER BY CreateTS;
END;
GO
