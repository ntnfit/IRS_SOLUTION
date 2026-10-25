

CREATE PROCEDURE APZ_D_ImageAlbum_Remove(in id int)
AS
BEGIN

	if :id in (select "AlbumId" from APZ_D_UPIM)
	then
	 	select 1, 'Cannot remove id because it has been used' from dummy;
	 	return;
	end if;
	
	if :id = -1
	then
	 	select 2, 'Cannot remove id because it default' from dummy;
	 	return;
	end if;
	
	delete from APZ_D_ImageAlbum where "Id" = :id;
	select 0, '' from dummy;

END



