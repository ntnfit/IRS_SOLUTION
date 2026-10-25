USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_Channel_Remove]
    @channelCode varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
--Delete Item data by channel
	delete from APZ_D_CHN1 where [ChannelCode] = @channelCode;

	--Delete Channel data
	delete from APZ_D_OCHN where [ChannelCode] = @channelCode;

	select '';
END;
GO
