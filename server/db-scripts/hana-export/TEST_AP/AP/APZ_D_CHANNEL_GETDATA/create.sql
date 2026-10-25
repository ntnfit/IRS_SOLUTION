

CREATE PROCEDURE APZ_D_Channel_GetData(
	in chnCode nvarchar(50), 
	in chnName nvarchar(50), 
	in status varchar(1)
)
AS
BEGIN

	declare query varchar(5000) = '';

	if :chnCode <> '' or :chnName <> '' or :status <> ''
	then
		--Search data
		query := 'select * from APZ_D_OCHN where 
		("ChannelCode" = '''' or "ChannelCode" like ''' || :chnCode || '%''' || ')
		and ("ChannelName" = '''' or "ChannelName" like ''' || :chnName || '%''' || ')
		and ("Status" = '''' or "Status" like ''' || :status || '%''' ||')';
		execute immediate :query;
	else
		--Get data
		select * from APZ_D_OCHN;
	end if;

END




