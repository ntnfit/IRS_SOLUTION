USE [IRS-COMMON];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_LicenseManager_SaveLic
    @bpcode nvarchar(100),
    @InstallNumber nvarchar(100),
    @HardwareKey nvarchar(800),
    @license nvarchar(max),
    @fileId nvarchar(100),
    @licenseType nvarchar(100),
    @dateRequest nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@InstallNumber)), N'') IS NULL
        THROW 50001, 'Install number is required.', 1;
    IF NULLIF(LTRIM(RTRIM(@HardwareKey)), N'') IS NULL
        THROW 50002, 'Hardware key is required.', 1;
    IF NULLIF(@license, N'') IS NULL
        THROW 50003, 'License content is required.', 1;
    IF OBJECT_ID(N'tempdb..#tempLicType') IS NULL
        THROW 50004, 'License type data is required.', 1;
    IF EXISTS (SELECT 1 FROM #tempLicType WHERE NULLIF(LTRIM(RTRIM(TypeCode)), N'') IS NULL OR Amount < 0)
        THROW 50005, 'License type data is invalid.', 1;
    IF EXISTS (SELECT TypeCode FROM #tempLicType GROUP BY TypeCode HAVING COUNT(*) > 1)
        THROW 50006, 'License type data contains duplicates.', 1;

    -- Never silently discard assignments when a renewal reduces its licensed capacity.
    IF EXISTS
    (
        SELECT u.LicenseType
        FROM dbo.APZ_LOU AS u
        LEFT JOIN #tempLicType AS t ON t.TypeCode = u.LicenseType
        WHERE u.InstallNumber = @InstallNumber
        GROUP BY u.LicenseType, t.Amount
        HAVING COUNT(DISTINCT u.[User]) > ISNULL(t.Amount, 0)
    )
        THROW 50007, 'Existing user assignments exceed the new license capacity.', 1;

    -- A different installation for the same machine is replaced as one atomic operation.
    DELETE u
    FROM dbo.APZ_LOU AS u
    INNER JOIN dbo.APZ_OALC AS oldLicense ON oldLicense.InstallNumber = u.InstallNumber
    WHERE oldLicense.HardwareKey = @HardwareKey
      AND oldLicense.InstallNumber <> @InstallNumber;

    DELETE FROM dbo.APZ_OALC
    WHERE HardwareKey = @HardwareKey OR InstallNumber = @InstallNumber;

    INSERT INTO dbo.APZ_OALC (InstallNumber, License, DateRequest, HardwareKey)
    VALUES (@InstallNumber, @license, @dateRequest, @HardwareKey);
END;
GO
