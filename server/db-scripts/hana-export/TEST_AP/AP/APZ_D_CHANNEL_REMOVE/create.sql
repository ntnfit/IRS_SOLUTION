

CREATE PROCEDURE APZ_D_Channel_Remove(in channelCode varchar(50))
AS
BEGIN

	--Delete Item data by channel
	delete from APZ_D_CHN1 where "ChannelCode" = :channelCode;

	--Delete Channel data
	delete from APZ_D_OCHN where "ChannelCode" = :channelCode;

	select '' from dummy;

END



