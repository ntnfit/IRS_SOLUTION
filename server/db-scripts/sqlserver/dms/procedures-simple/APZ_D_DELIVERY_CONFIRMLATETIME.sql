USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_Delivery_ConfirmLateTime]
    @docentry INT,
    @cardcode NVARCHAR(50),
    @newTime DATETIME,
    @reason NVARCHAR(254)
AS
BEGIN
    SET NOCOUNT ON;
UPDATE APZ_D_ODLO SET [LateTime] = @newTime, [LateReason] = @reason WHERE [DocEntry] = @docentry AND [CardCode] = @cardcode;
END;
GO
